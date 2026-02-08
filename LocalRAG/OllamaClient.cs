using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace LocalRAG
{
    internal class OllamaClient
    {
        private readonly HttpClient _http;
        private readonly string _baseUrl;
        private readonly string _embeddingModel;
        private readonly string _chatModel;

        public OllamaClient(string baseUrl = "http://localhost:11434",
        string embeddingModel = "nomic-embed-text",
        string chatModel = "llama3.2")
        {
            _baseUrl = baseUrl.TrimEnd('/');
            _embeddingModel = embeddingModel;
            _chatModel = chatModel;
            _http = new HttpClient { Timeout = TimeSpan.FromMinutes(15) };  // increased for slow CPU inference when using LLM

        }


        // ============Health Check=======

        public async Task<bool> IsAvailable()
        {
            try
            {
                var response = await _http.GetAsync($"{_baseUrl}/api/tags");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        // =============Embedding============

        public async Task<float[]> GetEmbedding(string text)
        {
            var payload = new { model = _embeddingModel, prompt = text };

            var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/api/embeddings")
            {
                Content = new StringContent(
                    JsonSerializer.Serialize(payload),
                    Encoding.UTF8,
                    "application/json")
            };

            using var response = await _http.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync();
                throw new Exception($"Ollama embeddings error {response.StatusCode}: {err}");
            }

            using var stream = await response.Content.ReadAsStreamAsync();
            var json = await JsonSerializer.DeserializeAsync<JsonElement>(stream);

            return json.GetProperty("embedding")
                       .EnumerateArray()
                       .Select(e => (float)e.GetDouble())
                       .ToArray();
        }


        public async Task<float[][]> GetEmbeddings(IEnumerable<string> texts)
        {
            var results = new List<float[]>();
            foreach (var text in texts)
            {
                results.Add(await GetEmbedding(text));
            }
            return results.ToArray();
        }

        // ==============Chat Completion============
        public async Task<string> Chat(string question, string context)
        {
            var systemPrompt = """
            You are a helpful assistant that answers questions based ONLY on the provided context.
            If the context doesn't contain enough information to answer, say so.
            Always cite which document(s) your answer comes from.
            Keep answers concise and factual.
            """;

            var userMessage = $"""
            Context:
            {context}

            Question: {question}
            """;

            var payload = new
            {
                model = _chatModel,
                messages = new[]
                {
                new { role = "system", content = systemPrompt },
                new { role = "user",   content = userMessage }
            },
                stream = false
            };

            var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/api/chat")
            {
                Content = new StringContent(
                    JsonSerializer.Serialize(payload),
                    Encoding.UTF8,
                    "application/json")
            };

            using var response = await _http.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync();
                throw new Exception($"Ollama chat error {response.StatusCode}: {err}");
            }

            using var stream = await response.Content.ReadAsStreamAsync();
            var json = await JsonSerializer.DeserializeAsync<JsonElement>(stream);

            return json.GetProperty("message").GetProperty("content").GetString() ?? "";
        }


    }
}

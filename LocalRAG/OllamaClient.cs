using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        // ==============Chat Completion============



    }
}

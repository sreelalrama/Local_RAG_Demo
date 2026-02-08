using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalRAG
{
    internal class MenuHandler
    {
        private OllamaClient ollama;
        private TextChunker chunker;
        private VectorStore store;

        public MenuHandler(OllamaClient ollama, TextChunker chunker, VectorStore store)
        {
            this.ollama = ollama;
            this.chunker = chunker;
            this.store = store;
        }

        public async Task MenuIngest()
        {
            Console.Write("\n  File or folder path (or press Enter for ./SampleDocs): ");
            var input = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(input))
                input = Path.Combine(Directory.GetCurrentDirectory(), "SampleDocs");

            var files = new List<string>();

            if (File.Exists(input))
            {
                files.Add(input);
            }
            else if (Directory.Exists(input))
            {
                files.AddRange(
                    Directory.GetFiles(input, "*.*")
                             .Where(f => DocumentParser.IsSupported(f)));
            }
            else
            {
                Console.WriteLine("  Path not found.");
                return;
            }

            if (files.Count == 0)
            {
                Console.WriteLine("  No supported files (.txt .md .pdf) found at that path.");
                return;
            }

            Console.WriteLine($"\n  Found {files.Count} file(s). Ingesting…\n");

            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file);
                Console.WriteLine($"  [{fileName}]");

                try
                {
                    // Parse document
                    var text = DocumentParser.Parse(file);
                    Console.WriteLine($"    Parsed: {text.Length:N0} chars");

                    // Chunk
                    var chunks = chunker.Chunk(text);
                    Console.WriteLine($"    Chunks: {chunks.Count}");

                    // Embed and store
                    Console.Write("    Embedding");
                    foreach (var chunk in chunks)
                    {
                        var embedding = await ollama.GetEmbedding(chunk);
                        store.AddChunk(fileName, chunk, embedding);
                        Console.Write(".");
                    }
                    Console.WriteLine(" done");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"    ERROR: {ex.Message}");
                }
            }
        }

        public async Task MenuQuery()
        {
            Console.Write("\n  Your question: ");
            var question = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(question)) return;

            if (store.GetChunkCount() == 0)
            {
                Console.WriteLine("\n  No documents ingested yet. Please ingest some documents first.");
                return;
            }

            Console.WriteLine("\n  Searching…");

            try
            {
                // Embed the question
                var queryEmbedding = await ollama.GetEmbedding(question);

                // Search for relevant chunks
                var results = store.Search(queryEmbedding, topK: 5);

                if (results.Count == 0)
                {
                    Console.WriteLine("  No relevant documents found.");
                    return;
                }

                // Build context from top results
                var context = string.Join("\n\n---\n\n",
                    results.Select(r => $"[Source: {r.Source}]\n{r.Content}"));

                Console.WriteLine("  Generating answer…\n");

                // Get answer from Ollama
                var answer = await ollama.Chat(question, context);

                Console.WriteLine($"  {answer}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n  ERROR: {ex.Message}");
            }
        }

        public void MenuList()
        {
            var sources = store.ListSources();
            if (sources.Count == 0)
            {
                Console.WriteLine("\n  Nothing ingested yet.");
                return;
            }

            Console.WriteLine("\n  Ingested documents:");
            foreach (var s in sources)
                Console.WriteLine($"    - {s}");
        }

        public void MenuClear()
        {
            Console.Write("\n  Delete all chunks from the database? (y/n): ");
            if (Console.ReadLine()?.Trim().Equals("y", StringComparison.OrdinalIgnoreCase) == true)
            {
                store.Clear();
                Console.WriteLine("  Database cleared.");
            }
        }
    }
}

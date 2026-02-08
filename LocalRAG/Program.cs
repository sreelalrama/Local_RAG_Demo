using LocalRAG;

// Title

// Load env variables from .env file
EnvironmentHelper.LoadDotEnv();
static string? Env(string key) => Environment.GetEnvironmentVariable(key);

// read config
var ollamaUrl = Env("OLLAMA_URL") ?? "http://localhost:11434";
var dbPath = Env("DB_PATH") ?? "vectors.db";
var embeddingModel = Env("EMBEDDING_MODEL") ?? "nomic-embed-text";
var chatModel = Env("CHAT_MODEL") ?? "tinyllama";



// wire services
var ollama = new OllamaClient(ollamaUrl, embeddingModel, chatModel);
var chunker = new TextChunker(chunkSize: 512, overlap: 128);
var store = new VectorStore(dbPath);
var menu = new MenuHandler(ollama, chunker, store);

// check ollama loacl server availability
Console.WriteLine("\n  Checking Ollama server...");
if (!await ollama.IsAvailable())
{
    Console.WriteLine($"""

      ERROR: Cannot connect to Ollama at {ollamaUrl}

      Please ensure:
        1. Ollama is installed (https://ollama.com)
        2. Ollama server is running: ollama serve
        3. Required models are pulled:
           - ollama pull {embeddingModel}
           - ollama pull {chatModel}

      """);
    return;
}
Console.WriteLine("  Ollama server is running.");

// run the app menu and logic loop
while (true)
{
    Console.WriteLine();
    Console.WriteLine("════════════════════════════════════════");
    Console.WriteLine("   Local RAG (Ollama Only)");
    Console.WriteLine("════════════════════════════════════════");
    Console.WriteLine($"   DB: {dbPath}  |  chunks: {store.GetChunkCount()}");
    Console.WriteLine($"   Embedding: {embeddingModel}  |  Chat: {chatModel}");
    Console.WriteLine("────────────────────────────────────────");
    Console.WriteLine("  1) Ingest document(s)");
    Console.WriteLine("  2) Ask a question");
    Console.WriteLine("  3) List ingested documents");
    Console.WriteLine("  4) Clear database");
    Console.WriteLine("  5) Exit");
    Console.WriteLine("────────────────────────────────────────");
    Console.Write("  Choice: ");

    // get user input

    // handle choices

}



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

// check ollama loacl server availability

// run the app menu and logic loop




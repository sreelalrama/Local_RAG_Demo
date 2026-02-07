# RAG Assistant - Local Edition (Ollama Only)

A **fully offline** RAG implementation using only local Ollama for both embeddings and LLM inference. No API keys, no cloud services, complete privacy.

## Why Local?

- **Privacy**: Your documents never leave your machine
- **Offline**: Works without internet connection
- **Free**: No API costs, no rate limits
- **Control**: Choose your own models

## Requirements

- [Ollama](https://ollama.com) installed and running
- At least 8GB RAM (16GB recommended)
- GPU recommended for faster inference (NVIDIA/AMD)

## Quick Start

### 1. Install Ollama

**Windows**: Download from https://ollama.com/download/windows


### 2. Start Ollama and pull models

```bash
# Start the server
ollama serve

# In another terminal, pull required models
ollama pull nomic-embed-text   # For embeddings (~274MB)
ollama pull tinyllama          # Fast, small LLM (~637MB)
# OR for better quality (slower on CPU):
ollama pull llama3.2           # Better quality (~2GB)
```

### 3. Run the application

```bash
cd Anthropic_RAG.Local
dotnet run
```

## Configuration

Edit `.env` to customize:

```env
# Ollama server URL
OLLAMA_URL=http://localhost:11434


```

## Model Recommendations

| Hardware | Embedding Model | Chat Model | Notes |
|----------|-----------------|------------|-------|
| CPU only, 8GB RAM | nomic-embed-text | tinyllama | Slow but works |
| CPU only, 16GB RAM | nomic-embed-text | llama3.2 | Better quality |
| GPU (any) | nomic-embed-text | llama3.2 | Good performance |
| GPU (8GB+ VRAM) | nomic-embed-text | mistral | Best quality |

## Usage

```
════════════════════════════════════════
   Local RAG (Ollama Only)
════════════════════════════════════════
   DB: vectors.db  |  chunks: 0
   Embedding: nomic-embed-text  |  Chat: tinyllama
────────────────────────────────────────
  1) Ingest document(s)
  2) Ask a question
  3) List ingested documents
  4) Clear database
  5) Exit
────────────────────────────────────────
  Choice:
```



## Dependencies

- .NET 10.0
- Microsoft.Data.Sqlite
- PdfPig
- Ollama (external)

## License

MIT

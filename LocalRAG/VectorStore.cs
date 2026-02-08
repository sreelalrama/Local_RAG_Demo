using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalRAG
{
    internal class VectorStore
    {
        private readonly SqliteConnection _conn;

        public VectorStore(string dbPath = "vectors.db")
        {
            _conn = new SqliteConnection($"Data Source={dbPath}");
            _conn.Open();
            InitSchema();
        }

        private void InitSchema()
        {
            using var cmd = _conn.CreateCommand();
            cmd.CommandText = """
            CREATE TABLE IF NOT EXISTS chunks (
                id        INTEGER PRIMARY KEY AUTOINCREMENT,
                source    TEXT NOT NULL,
                content   TEXT NOT NULL,
                embedding BLOB NOT NULL
            );
            CREATE INDEX IF NOT EXISTS idx_source ON chunks(source);
            """;
            cmd.ExecuteNonQuery();
        }

        public void AddChunk(string source, string content, float[] embedding)
        {
            using var cmd = _conn.CreateCommand();
            cmd.CommandText = "INSERT INTO chunks (source, content, embedding) VALUES (@s, @c, @e)";
            cmd.Parameters.AddWithValue("@s", source);
            cmd.Parameters.AddWithValue("@c", content);
            cmd.Parameters.AddWithValue("@e", ToBytes(embedding));
            cmd.ExecuteNonQuery();
        }

        public List<(string Source, string Content, double Score)> Search(float[] queryEmbedding, int topK = 5)
        {
            var results = new List<(string, string, double)>();

            using var cmd = _conn.CreateCommand();
            cmd.CommandText = "SELECT source, content, embedding FROM chunks";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var source = reader.GetString(0);
                var content = reader.GetString(1);
                var embedding = FromBytes((byte[])reader["embedding"]);
                var score = CosineSimilarity(queryEmbedding, embedding);
                results.Add((source, content, score));
            }

            return results
                .OrderByDescending(r => r.Item3)
                .Take(topK)
                .ToList();
        }

        public int GetChunkCount()
        {
            using var cmd = _conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM chunks";
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public List<string> ListSources()
        {
            var sources = new List<string>();
            using var cmd = _conn.CreateCommand();
            cmd.CommandText = "SELECT DISTINCT source FROM chunks ORDER BY source";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                sources.Add(reader.GetString(0));

            return sources;
        }

        public void Clear()
        {
            using var cmd = _conn.CreateCommand();
            cmd.CommandText = "DELETE FROM chunks";
            cmd.ExecuteNonQuery();
        }

        public void Dispose()
        {
            _conn?.Close();
            _conn?.Dispose();
        }

        // ─── Helpers ─────────────────────────────────────────────────────────────────

        private static byte[] ToBytes(float[] arr)
        {
            var bytes = new byte[arr.Length * 4];
            Buffer.BlockCopy(arr, 0, bytes, 0, bytes.Length);
            return bytes;
        }

        private static float[] FromBytes(byte[] bytes)
        {
            var arr = new float[bytes.Length / 4];
            Buffer.BlockCopy(bytes, 0, arr, 0, bytes.Length);
            return arr;
        }

        private static double CosineSimilarity(float[] a, float[] b)
        {
            double dot = 0, magA = 0, magB = 0;
            for (int i = 0; i < a.Length; i++)
            {
                dot += a[i] * b[i];
                magA += a[i] * a[i];
                magB += b[i] * b[i];
            }
            return dot / (Math.Sqrt(magA) * Math.Sqrt(magB) + 1e-10);
        }       
    }
}

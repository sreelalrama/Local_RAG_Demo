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
           
        }

        public int GetChunkCount()
        {
            using var cmd = _conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM chunks";
            return Convert.ToInt32(cmd.ExecuteScalar());
        }
    }
}

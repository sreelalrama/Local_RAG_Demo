using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalRAG
{
    internal class TextChunker
    {
        private readonly int _chunkSize;
        private readonly int _overlap;

        public TextChunker(int chunkSize = 512, int overlap = 128)
        {
            _chunkSize = chunkSize;
            _overlap = overlap;
        }

        public List<string> Chunk(string text)
        {
            var words = text.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            var chunks = new List<string>();

            for (int i = 0; i < words.Length; i += _chunkSize - _overlap)
            {
                var chunkWords = words.Skip(i).Take(_chunkSize).ToArray();
                if (chunkWords.Length == 0) break;

                chunks.Add(string.Join(" ", chunkWords));
            }

            return chunks;
        }
    }
}

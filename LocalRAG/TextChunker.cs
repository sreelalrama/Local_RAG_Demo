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
    }
}

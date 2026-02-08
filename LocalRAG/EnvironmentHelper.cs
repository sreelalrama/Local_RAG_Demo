using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalRAG
{
    internal class EnvironmentHelper
    {
        public static void LoadDotEnv(string path = ".env")
        {
            if (!File.Exists(path)) return;

            foreach (var line in File.ReadLines(path))
            {
                var trimmed = line.Trim();
                if (trimmed.Length == 0 || trimmed[0] == '#') continue;

                var eqIdx = trimmed.IndexOf('=');
                if (eqIdx < 1) continue;

                var key = trimmed[..eqIdx].Trim();
                var value = trimmed[(eqIdx + 1)..].Trim().Trim('"').Trim('\'');

                Environment.SetEnvironmentVariable(key, value);
            }
        }
    }
}

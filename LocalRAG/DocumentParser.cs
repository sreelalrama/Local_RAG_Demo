using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyToad.PdfPig;

namespace LocalRAG
{
    internal class DocumentParser
    {
        private static readonly HashSet<string> SupportedExtensions
            = new(StringComparer.OrdinalIgnoreCase){".txt", ".md", ".pdf" };

        public static bool IsSupported(string filePath)
        {
            return SupportedExtensions.Contains(Path.GetExtension(filePath));
        }

        public static string Parse(string filePath)
        {
            var ext = Path.GetExtension(filePath).ToLowerInvariant();

            return ext switch
            {
                ".txt" or ".md" => File.ReadAllText(filePath),
                ".pdf" => ParsePdf(filePath),
                _ => throw new NotSupportedException($"Unsupported file type: {ext}")
            };
        }

        private static string ParsePdf(string filePath)
        {
            using var pdf = PdfDocument.Open(filePath);
            var pages = new List<string>();

            foreach (var page in pdf.GetPages())
            {
                pages.Add(page.Text);
            }

            return string.Join("\n\n", pages);
        }


    }
}

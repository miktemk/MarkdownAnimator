using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Miktemk;

namespace MarkdownUtils.Core
{
    /// <summary>
    /// sc = shortcuts
    /// </summary>
    public class Mdmmsc
    {
        private readonly Dictionary<string, IEnumerable<string>> replacements = new Dictionary<string, IEnumerable<string>>();

        private Mdmmsc() { }

        public static Mdmmsc LoadFromFile(string filename)
        {
            var mdmmsc = new Mdmmsc();
            var textConfigLines = File.ReadAllLines(filename);
            textConfigLines.EnumerateSublistsStartingWith(x => x.StartsWith("a: "), (bsRaw, aRaw, index) =>
            {
                if (aRaw == null || !bsRaw.Any())
                    return;
                var a = aRaw.Substring(3).Trim();
                var bs = bsRaw
                    .Where(x => x.StartsWith("b: "))
                    .Select(b => b.Substring(3).Trim());
                mdmmsc.replacements.Add(a, bs);
            });
            return mdmmsc;
        }

        public string RunReplacements(string text)
        {
            foreach (var entry in replacements)
            {
                var a = entry.Key;
                var bs = entry.Value;
                int countTimeout = 0;
                while (text.Contains(a))
                {
                    var b = bs.Random();
                    text = text.Replace(a, b);
                    countTimeout++;
                    if (countTimeout > 5000) break; // just in case you have recursive replacements
                }
            }
            return text;
        }
    }
}

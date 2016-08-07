using System;
using System.Linq;
using Miktemk;

namespace MarkdownAnimator.Views.Behaviors
{
    public class Utils
    {
        public static int[] StringToIntArray(string s)
        {
            if (String.IsNullOrWhiteSpace(s))
                return new int[] { };
            var splits = s.Split(',');
            return splits
                .Select(x => x.ParseIntOrNull())
                .Where(x => x.HasValue)
                .Select(x => x.Value)
                .ToArray();
        }
    }
}
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using MarkdownUtils.MdAnimated;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MarkdownAnimator.Views.Behaviors
{
    // TODO: sort this out
    // from: http://stackoverflow.com/questions/5029724/avalonedit-wpf-texteditor-sharpdevelop-how-to-highlight-a-specific-range-of-t
    public class AEHighlightTtsKeyPoint : DocumentColorizingTransformer
    {
        private Regex regexRemoveBrackets;
        private Regex regexWhereBrackets;

        public IEnumerable<TtsKeyPoint> KeyPoints { get; set; }

        public AEHighlightTtsKeyPoint()
        {
            regexRemoveBrackets = new Regex(@"([^\\]|^)(\(|\))"); // $1 - char before. $2 - the bracket
            regexWhereBrackets = new Regex(@"(?:[^\\]|^)(\(.*?\))");
        }

        protected override void ColorizeLine(DocumentLine line)
        {
            if (KeyPoints == null)
                return;

            foreach (var kp in KeyPoints)
            {
                if (kp.KeyPointType == MdKeyPointType.String)
                {
                    HighlightString(line, kp.Token);
                }
                else if (kp.KeyPointType == MdKeyPointType.Regex)
                {
                    HighlightRegex(line, kp.Token);
                }
                else if (kp.KeyPointType == MdKeyPointType.HighlightLines)
                {
                    int[] linesToHightlight = Utils.StringToIntArray(kp.Token);
                    HighlightLines(line, linesToHightlight);
                }
            }

        }

        private void HighlightLines(DocumentLine line, int[] linesToHightlight)
        {
            if (linesToHightlight.Any(x => x == line.LineNumber))
            {
                base.ChangeLinePart(line.Offset, line.Offset + line.Length, (VisualLineElement element) => {
                    element.TextRunProperties.SetBackgroundBrush(new SolidColorBrush(Colors.Orange));
                    element.TextRunProperties.SetForegroundBrush(new SolidColorBrush(Colors.White));
                });
            }
        }

        private void HighlightString(DocumentLine line, string token)
        {
            int lineStartOffset = line.Offset;
            string text = CurrentContext.Document.GetText(line);
            int start = 0;
            int index;
            while ((index = text.IndexOf(token, start)) >= 0)
            {
                base.ChangeLinePart(
                    lineStartOffset + index, // startOffset
                    lineStartOffset + index + token.Length, // endOffset
                    (VisualLineElement element) => {
                        element.TextRunProperties.SetBackgroundBrush(new SolidColorBrush(Colors.OrangeRed));
                        element.TextRunProperties.SetForegroundBrush(new SolidColorBrush(Colors.White));
                    });
                start = index + 1; // search for next occurrence
            }
        }

        private void HighlightRegex(DocumentLine line, string token)
        {
            int lineStartOffset = line.Offset;
            string text = CurrentContext.Document.GetText(line);
            int start = 0;
            int index;
            var escapedToken = regexRemoveBrackets.Replace(token, "$1");
            escapedToken = escapedToken
                .Replace("\\(", "(")
                .Replace("\\)", ")");
            var bracketPositions = findBraketPositions(token);
            while ((index = text.IndexOf(escapedToken, start)) >= 0)
            {
                foreach (var bpos in bracketPositions)
                {
                    base.ChangeLinePart(
                        lineStartOffset + index + bpos.Item1, // startOffset
                        lineStartOffset + index + bpos.Item2, // endOffset
                        (VisualLineElement element) => {
                            element.TextRunProperties.SetBackgroundBrush(new SolidColorBrush(Colors.OrangeRed));
                            element.TextRunProperties.SetForegroundBrush(new SolidColorBrush(Colors.White));
                        });
                    start = index + 1; // search for next occurrence
                }
            }
        }

        private Tuple<int, int>[] findBraketPositions(string token)
        {
            token = token
                .Replace("\\(", " ")
                .Replace("\\)", " ");
            MatchCollection matches = regexWhereBrackets.Matches(token);
            var groupPositions = new List<Tuple<int, int>>();
            var adjustment = 0;
            foreach (var match in matches.Cast<Match>())
            {
                groupPositions.Add(new Tuple<int, int>(
                    match.Groups[1].Index - adjustment,
                    match.Groups[1].Index + match.Groups[1].Length - 2 - adjustment));
                adjustment += 2;
            }
            return groupPositions.ToArray();
        }

        //def findBraketPosition(self, token):
        //    token = token.replace('\\(', ' ')
        //    token = token.replace('\\)', ' ')
        //    groupPositions = []
        //    adjustment = 0
        //    for m in self.regexWhereBrackets.finditer(token):
        //        groupPositions += [(m.start(1) - adjustment, m.end(1) - 3 - adjustment)]
        //    adjustment += 2
        //    return groupPositions

    }
}

using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using MarkdownUtils.MdAnimated;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MarkdownAnimator.Views.Behaviors
{
    public static class AEBehaviors
    {
        public static IEnumerable<TtsKeyPoint> GetHightlightTtsKeyPoints(DependencyObject obj)
        {
            return (IEnumerable<TtsKeyPoint>)obj.GetValue(HightlightTtsKeyPointsProperty);
        }

        public static void SetHightlightTtsKeyPoints(DependencyObject obj, IEnumerable<TtsKeyPoint> value)
        {
            obj.SetValue(HightlightTtsKeyPointsProperty, value);
        }

        // Using a DependencyProperty as the backing store for HightlightTtsKeyPoints.  This enables animation, styling, binding, etc…
        public static readonly DependencyProperty HightlightTtsKeyPointsProperty =
            DependencyProperty.RegisterAttached("HightlightTtsKeyPoints", typeof(IEnumerable<TtsKeyPoint>), typeof(AEBehaviors), new PropertyMetadata(OnHightlightTtsKeyPointsChanged));

        private static void OnHightlightTtsKeyPointsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextEditor tEdit = (TextEditor)d;
            tEdit.TextArea.TextView.LineTransformers.Clear(); //TODO: persist old shit but faded???

            var keyPoint = GetHightlightTtsKeyPoints(d);
            if (keyPoint != null)
            {
                var highlightTransformer = new AEHighlightTtsKeyPoint();
                highlightTransformer.KeyPoints = keyPoint;
                tEdit.TextArea.TextView.LineTransformers.Add(highlightTransformer);
            }
        }

    }


    // TODO: sort this out
    // from: http://stackoverflow.com/questions/5029724/avalonedit-wpf-texteditor-sharpdevelop-how-to-highlight-a-specific-range-of-t
    public class AEHighlightTtsKeyPoint : DocumentColorizingTransformer
    {
        public IEnumerable<TtsKeyPoint> KeyPoints { get; set; }

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
    }
}

using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using MarkdownUtils.MdAnimated;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
}

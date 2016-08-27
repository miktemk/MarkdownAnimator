using MarkdownUtils.MdAnimated;
using System.Collections.Generic;

namespace MarkdownUtils.Core
{
    public class MdAnimatedEvent
    {
        public string Text { get; set; }
        public IEnumerable<TtsKeyPoint> KeyPoints { get; set; }
        public string Lang { get; set; }

        public MdAnimatedEvent(string text)
        {
            Text = text;
        }

        public MdAnimatedEvent(string text, IEnumerable<TtsKeyPoint> keyPoints) : this(text)
        {
            this.KeyPoints = keyPoints;
        }
    }
}
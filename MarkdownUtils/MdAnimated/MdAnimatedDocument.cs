using Miktemk.TextToSpeech.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkdownUtils.MdAnimated
{
    public class MdAnimatedDocument
    {
        public List<MdAnimatedBlock> Pages { get; } = new List<MdAnimatedBlock>();
    }
    // TODO: abstract this class for other content such as images, etc
    public class MdAnimatedBlock
    {
        public string Code { get; set; }
        public TtsContentWithKeyPoints TtsContent { get; set; }

        public MdAnimatedBlock()
        {
            TtsContent = new TtsContentWithKeyPoints();
        }
    }
    public class TtsContentWithKeyPoints
    {
        public MultiLanguageText TtsText { get; set; }
        public List<TtsKeyPoint> KeyPoints { get; } = new List<TtsKeyPoint>();
    }
    public class TtsKeyPoint
    {
        public int AtWhatChar { get; set; }
        public string Token { get; set; }
        public MdTokenType TokenType { get; set; }
    }
    public enum MdTokenType
    {
        String,
        Regex,
    }
}

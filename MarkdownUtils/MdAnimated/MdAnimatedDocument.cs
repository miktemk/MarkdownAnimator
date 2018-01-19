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
        public string Title { get; set; }
        public List<MdAnimatedDocumentSection> Sections { get; } = new List<MdAnimatedDocumentSection>();
    }
    public class MdAnimatedDocumentSection
    {
        public string Title { get; set; }
        public List<MdAnimatedBlock> Pages { get; } = new List<MdAnimatedBlock>();
    }
    // TODO: abstract this class for other content such as images, etc
    public class MdAnimatedBlock
    {
        public string Code { get; set; }
        public MultiLangStringPhrased TtsText { get; set; }
        public string TtsTextString { get; set; }
        public List<TtsKeyPoint> KeyPoints { get; set; }

        public MdAnimatedBlock()
        {
            TtsText = new MultiLangStringPhrased();
            KeyPoints = new List<TtsKeyPoint>();
        }
    }
    public class TtsKeyPoint
    {
        public int AtWhatChar { get; set; }
        public string Token { get; set; } // TODO: more ways to identifu tokens
        public MdKeyPointType KeyPointType { get; set; }

        public TtsKeyPoint() { }
        public TtsKeyPoint(string token)
            : this(token, MdKeyPointType.String)
        { }
        public TtsKeyPoint(string token, MdKeyPointType keyPointType)
        {
            Token = token;
            KeyPointType = keyPointType;
        }
    }
    public enum MdKeyPointType
    {
        String,
        Regex,
        HighlightLines,
    }
}

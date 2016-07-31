using CommonMark.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkdownUtils.MdDoc
{
    public class MdDocument
    {
        public List<MdBlock> Blocks { get; } = new List<MdBlock>();
    }

    public class MdBlock
    {
        public BlockTag Tag { get; set; }
        public int HeaderLevel { get; set; }
        public List<MdBlock> SubBlocks { get; } = new List<MdBlock>();
        public List<MdInline> Inline { get; } = new List<MdInline>();
        public string StringContent { get; set; }
    }
    public class MdInline
    {
        public InlineTag Tag { get; set; }
        public string LinkUrl { get; set; }
        public string Text { get; set; }
        public List<MdInline> SubInline { get; } = new List<MdInline>();
    }
}

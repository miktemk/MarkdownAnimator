using CommonMark.Syntax;
using MarkdownUtils.MdDoc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkdownUtils.Core
{
    /// <summary>
    /// Flattens out the MdBlock via recursion
    /// </summary>
    public class MdBlockFlattener
    {
        public IEnumerable<MdInlineFlat> FlattenBlock(MdBlock para)
        {
            var result = new List<MdInlineFlat>();
            RecurseMdBlock(para, result);
            return result;
        }
        private void RecurseMdBlock(MdBlock para, List<MdInlineFlat> list)
        {
            if (para.StringContent != null)
                list.Add(new MdInlineFlat(para.StringContent, para.Tag));
            foreach (var childPara in para.SubBlocks)
            {
                RecurseMdBlock(childPara, list);
            }
            foreach (var inline in para.Inline)
            {
                RecurseInline(inline, list);
            }
            // .... when block ends any chain of inlines should be closed with a line break for our purposes
            if (para.Inline.Any())
                list.Add(new MdInlineFlat("\n", InlineTag.LineBreak));
        }

        private void RecurseInline(MdInline inline, List<MdInlineFlat> list)
        {
            if (inline.Text != null)
                list.Add(new MdInlineFlat(inline.Text, inline.Tag));
            foreach (var inlineSub in inline.SubInline)
            {
                RecurseInline(inlineSub, list);
            }
        }
    }

}

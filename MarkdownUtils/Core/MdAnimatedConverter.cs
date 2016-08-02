using MarkdownUtils.MdDoc;
using MarkdownUtils.MdAnimated;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Miktemk;
using System.Threading.Tasks;
using CommonMark.Syntax;

namespace MarkdownUtils.Core
{
    public class MdAnimatedConverter
    {
        public MdAnimatedConverter()
        {

        }

        public MdAnimatedDocument MdDocument2Animated(MdDocument mdDoc)
        {
            var result = new MdAnimatedDocument();
            var sbPageText = new StringBuilder();

            var splitHeadings = mdDoc.Blocks.SplitEnumerableStartingWith(b => b.HeaderLevel > 0);
            foreach (var sectionBlocks in splitHeadings)
            {
                var splitCodes = SplitMdSectionsIntoCodeBlocksNotesConsidered(sectionBlocks);
                foreach (var pageParagraphs in splitCodes)
                {
                    sbPageText.Clear();
                    var curPage = new MdAnimatedBlock();
                    foreach (var para in pageParagraphs)
                    {
                        if (IsCodeBlock(para))
                        {
                            // this is the end of pageParagraphs
                            curPage.Code = para.StringContent;
                            curPage.TtsContent.TtsText = sbPageText.ToString();
                            // TODO: parse this string
                        }
                        else
                        {
                            var allText = GetAllParaTextInlineAndAll(para);
                            sbPageText.Append(allText);
                            sbPageText.Append("\n");
                        }
                    }
                    result.Pages.Add(curPage);
                }
            }

            return result;
        }

        private IEnumerable<IEnumerable<MdBlock>> SplitMdSectionsIntoCodeBlocksNotesConsidered(IEnumerable<MdBlock> sectionBlocks)
        {
            var codeSplit = sectionBlocks.SplitEnumerableEndingWith(b => IsCodeBlock(b));
            // TODO: move Notes:, etc to prev sublists
            return codeSplit;
        }

        private string GetAllParaTextInlineAndAll(MdBlock para)
        {
            var sb = new StringBuilder();
            if (para.StringContent != null)
                sb.Append(para.StringContent);
            foreach (var inline in para.Inline)
            {
                var inlineAll = GetInlineText(inline);
                sb.Append(inlineAll);
            }
            foreach (var childPara in para.SubBlocks)
            {
                var childText = GetAllParaTextInlineAndAll(childPara);
                sb.Append(childText);
            }
            return sb.ToString();
        }

        private object GetInlineText(MdInline inline)
        {
            var sb = new StringBuilder();
            if (inline.Text != null)
                sb.Append(inline.Text);
            foreach (var inlineSub in inline.SubInline)
            {
                var subInlineAll = GetInlineText(inlineSub);
                sb.Append(subInlineAll);
            }
            return sb.ToString();
        }

        private bool IsCodeBlock(MdBlock b)
        {
            return b.Tag == BlockTag.IndentedCode ||
                    b.Tag == BlockTag.FencedCode;
        }
    }
}

using MarkdownUtils.MdDoc;
using MarkdownUtils.MdAnimated;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Miktemk;
using System.Threading.Tasks;
using CommonMark.Syntax;
using MarkdownUtils.Properties;

namespace MarkdownUtils.Core
{
    public class MdAnimatedConverter
    {
        private readonly string[] NotesHeaders;

        public MdAnimatedConverter()
        {
            NotesHeaders = Settings.Default.NotesText.Split('|');
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
            var codeSplit = sectionBlocks
                .SplitEnumerableEndingWith(b => IsCodeBlock(b))
                .ToArray();
            codeSplit.EnumerateWithIndex((codeBlock, index) =>
            {
                var head = codeBlock.FirstOrDefault();
                if (head == null)
                    return;
                if (IsMdBlockNotesColon(head) && index > 0 && codeBlock.Count() >= 2)
                {
                    // move Notes:, etc to prev sublists
                    var first2 = codeBlock.Take(2);
                    codeSplit[index] = codeSplit[index].Skip(2);
                    codeSplit[index-1] = codeSplit[index].Union(first2);
                }
            });
            return codeSplit.Where(x => x.Any()); // remove empty entries
        }

        private bool IsMdBlockNotesColon(MdBlock head)
        {
            var headInlineFirst = head.Inline.FirstOrDefault();
            if (headInlineFirst == null)
                return false;
            return NotesHeaders.Any(nnn => CompareIgnoreWhitespace(nnn, headInlineFirst.Text));
        }

        private bool CompareIgnoreWhitespace(string a, string b)
        {
            if (a != null)
                a = a.Trim();
            if (b != null)
                b = b.Trim();
            return string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
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

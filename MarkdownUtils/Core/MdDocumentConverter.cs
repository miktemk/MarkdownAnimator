using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonMark.Syntax;
using System.IO;
using MarkdownUtils.MdDoc;

namespace MarkdownUtils.Core
{
    public class MdDocumentConverter
    {
        private StringWriter stringWriter;

        public MdDocumentConverter()
        {
            stringWriter = new StringWriter();
        }

        public MdDocument CommonMark2MdDocument(Block doc)
        {
            var result = new MdDocument();
            ProcessBlocks(result.Blocks, doc.FirstChild);
            return result;
        }

        private void ProcessBlocks(List<MdBlock> blocksToAdd, Block curBlock)
        {
            while (curBlock != null)
            {
                //Debug.WriteLine(howDeepInTheShitAreWe + $"-------- block: {curBlock.Tag} (H{curBlock.HeaderLevel}) --------");
                var newMdBlock = new MdBlock
                {
                    Tag = curBlock.Tag,
                    HeaderLevel = curBlock.Heading.Level,
                };
                blocksToAdd.Add(newMdBlock);
                if (curBlock.StringContent != null)
                {
                    stringWriter.GetStringBuilder().Clear(); // TODO: does this work???
                    curBlock.StringContent.WriteTo(stringWriter);
                    newMdBlock.StringContent = stringWriter.ToString();
                }
                if (curBlock.InlineContent != null)
                {
                    ProcessInline(newMdBlock.Inline, curBlock.InlineContent);
                }
                if (curBlock.FirstChild != null)
                {
                    ProcessBlocks(newMdBlock.SubBlocks, curBlock.FirstChild);
                }
                curBlock = curBlock.NextSibling;
            }
        }

        private void ProcessInline(List<MdInline> inlineToAdd, Inline curInline)
        {
            while (curInline != null)
            {
                var newMdInline = new MdInline
                {
                    Tag = curInline.Tag,
                    LinkUrl = curInline.TargetUrl,
                    Text = curInline.LiteralContent,
                };
                inlineToAdd.Add(newMdInline);
                if (curInline.FirstChild != null)
                {
                    ProcessInline(newMdInline.SubInline, curInline.FirstChild);
                }
                curInline = curInline.NextSibling;
            }
        }
    }
}

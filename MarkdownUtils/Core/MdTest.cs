using CommonMark;
using CommonMark.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MarkdownUtils.Core
{
    public class MdTest
    {
        public MdTest()
        {
        }

        private void LoopThroughInlines(Inline inlineGuy, string howDeepInTheShitAreWe)
        {
            var inlineSibs = inlineGuy;
            while (inlineSibs != null)
            {
                if (inlineSibs.Tag == InlineTag.Link)
                    Debug.WriteLine(howDeepInTheShitAreWe + $"link!!! {inlineSibs.TargetUrl}");
                Debug.WriteLine(howDeepInTheShitAreWe + $"inline: {inlineSibs.Tag} {inlineSibs.LiteralContent}");
                if (inlineSibs.FirstChild != null)
                {
                    Debug.WriteLine(howDeepInTheShitAreWe + " inline-children:");
                    LoopThroughInlines(inlineSibs.FirstChild, " > " + howDeepInTheShitAreWe);
                }
                inlineSibs = inlineSibs.NextSibling;
            }
        }
        private void LoopThroughBlocks(Block curBlock, string howDeepInTheShitAreWe)
        {
            while (curBlock != null)
            {
                Debug.WriteLine(howDeepInTheShitAreWe + $"-------- block: {curBlock.Tag} (H{curBlock.HeaderLevel}) --------");
                if (curBlock.InlineContent != null)
                {
                    LoopThroughInlines(curBlock.InlineContent, howDeepInTheShitAreWe);
                }
                if (curBlock.StringContent != null)
                {
                    var stringWriter = new StringWriter();
                    curBlock.StringContent.WriteTo(stringWriter);
                    var strContent = stringWriter.ToString();
                    Debug.WriteLine(howDeepInTheShitAreWe + "strContent: " + strContent);
                }
                if (curBlock.FirstChild != null)
                {
                    Debug.WriteLine(howDeepInTheShitAreWe + " block-children");
                    LoopThroughBlocks(curBlock.FirstChild, " > " + howDeepInTheShitAreWe);
                }
                curBlock = curBlock.NextSibling;
            }
        }
    }
}

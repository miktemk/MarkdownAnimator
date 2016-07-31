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
            var mdText = File.ReadAllText(@"C:\Users\Mikhail\Google Drive\md-notes\zomp-DataLayerChanges.md");
            var doc = CommonMarkConverter.Parse(mdText);
            //var result = CommonMarkConverter.Convert(mdText);
            //File.WriteAllText(@"C:\Users\Mikhail\Desktop\___test.html", result);

            Debug.WriteLine("\n\n\n\n\n\n=========================================================================\n\n\n");
            CallFuckingLoop2(doc.FirstChild, "");

            //CallFuckingLoop(doc, CommonMarkSettings.Default);
        }

        private void CallFuckingLoop2_inline(Inline inlineGuy, string howDeepInTheShitAreWe)
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
                    CallFuckingLoop2_inline(inlineSibs.FirstChild, " > " + howDeepInTheShitAreWe);
                }
                inlineSibs = inlineSibs.NextSibling;
            }
        }
        private void CallFuckingLoop2(Block curBlock, string howDeepInTheShitAreWe)
        {
            while (curBlock != null)
            {
                Debug.WriteLine(howDeepInTheShitAreWe + $"-------- block: {curBlock.Tag} (H{curBlock.HeaderLevel}) --------");
                if (curBlock.InlineContent != null)
                {
                    CallFuckingLoop2_inline(curBlock.InlineContent, howDeepInTheShitAreWe);
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
                    CallFuckingLoop2(curBlock.FirstChild, " > " + howDeepInTheShitAreWe);
                }
                curBlock = curBlock.NextSibling;
            }
        }
    }
}

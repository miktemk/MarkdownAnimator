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
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace MarkdownUtils.Core
{
    public class MdAnimatedConverter
    {
        private const char TtsKeypointChar = (char)0x0;

        private readonly string[] NotesHeaders;
        private readonly MdBlockFlattener flattener;
        private Regex regexMdHighlight;

        public MdAnimatedConverter()
        {
            NotesHeaders = Settings.Default.NotesText.Split('|');
            flattener = new MdBlockFlattener();
            regexMdHighlight = new Regex("<md-highlight\\slines=\"(.*?)\".*\\/>");
        }

        public MdAnimatedDocument MdDocument2Animated(MdDocument mdDoc)
        {
            var result = new MdAnimatedDocument();

            // .... find H1 heading for a title
            var h1Heading = mdDoc.Blocks.FirstOrDefault(x => x.HeaderLevel == 1);
            if (h1Heading != null)
                result.Title = StringifyBlockSimple(h1Heading);

            var splitSections = mdDoc.Blocks.SplitEnumerableStartingWith(b => b.HeaderLevel > 0);
            foreach (var blocksWithHeadings in splitSections)
            {
                // .... separate into headings and blocks
                var headings = blocksWithHeadings.TakeWhile(x => x.HeaderLevel > 0);
                var sectionBlocks = blocksWithHeadings.SkipWhile(x => x.HeaderLevel > 0);

                // .... if there is no content under this heading, skip it
                if (!sectionBlocks.Any())
                    continue;

                var newSection = new MdAnimatedDocumentSection();

                // .... find the title of this section
                var lastHeading = headings.LastOrDefault();
                if (lastHeading != null)
                    newSection.Title = StringifyBlockSimple(lastHeading);

                // .... split by code...
                var splitCodes = SplitMdSectionsIntoCodeBlocksNotesConsidered(sectionBlocks);

                // .... and make pages
                foreach (var pageParagraphs in splitCodes)
                {
                    var code = "";
                    var TtsFlatParagraphs = new List<IEnumerable<MdInlineFlat>>();
                    foreach (var para in pageParagraphs)
                    {
                        if (IsCodeBlock(para))
                        {
                            // this is the code... potentially some notes after it...
                            code = para.StringContent;
                        }
                        else
                        {
                            var flatInlines = flattener.FlattenBlock(para);
                            TtsFlatParagraphs.Add(flatInlines);
                            //var allText = GetAllParaTextInlineAndAll(para);
                            //sbPageText.Append(allText);
                            //sbPageText.Append("\n");
                        }
                    }
                    var allTtsFlatTexts = TtsFlatParagraphs.SelectMany(x => x);
                    var allTtsFlatTextsStrings = allTtsFlatTexts.Select(x => x.Text).ToArray();
                    var keyPoints = new List<TtsKeyPoint>();
                    //int curChar = 0;
                    allTtsFlatTexts.EnumerateWithIndex3((before, me, after, index) =>
                    {
                        if (me.TagInline == InlineTag.Code)
                        {
                            //if (before != null && before.Text.EndsWith("[r") &&
                            //    after != null && after.Text.StartsWith("]"))
                            if (index >= 2 &&
                                allTtsFlatTextsStrings[index - 2].Equals("[") &&
                                allTtsFlatTextsStrings[index - 1].Equals("r") &&
                                after != null && after.Text.StartsWith("]"))
                            {
                                // .... [r`someregex(findthis)`] TODO: not sure about the format yet
                                //curChar -= 2;
                                keyPoints.Add(new TtsKeyPoint
                                {
                                    //AtWhatChar = curChar,
                                    Token = allTtsFlatTextsStrings[index],
                                    KeyPointType = MdKeyPointType.Regex,
                                });
                                // .... mark token and remove brackets
                                allTtsFlatTextsStrings[index] = TtsKeypointChar.ToString();
                                allTtsFlatTextsStrings[index - 1] = String.Empty;
                                allTtsFlatTextsStrings[index - 2] = allTtsFlatTextsStrings[index - 1].TrimEndN(1);
                                allTtsFlatTextsStrings[index + 1] = allTtsFlatTextsStrings[index + 1].TrimStartN(1);
                            }
                            else if (before != null && before.Text.EndsWith("[") &&
                                after != null && after.Text.StartsWith("]"))
                            {
                                // .... [`somecode`]
                                //curChar -= 1;
                                keyPoints.Add(new TtsKeyPoint
                                {
                                    //AtWhatChar = curChar,
                                    Token = allTtsFlatTextsStrings[index],
                                    KeyPointType = MdKeyPointType.String,
                                });
                                // .... mark token and remove brackets
                                allTtsFlatTextsStrings[index] = TtsKeypointChar.ToString();
                                allTtsFlatTextsStrings[index - 1] = allTtsFlatTextsStrings[index - 1].TrimEndN(1);
                                allTtsFlatTextsStrings[index + 1] = allTtsFlatTextsStrings[index + 1].TrimStartN(1);
                            }
                            else
                            {
                                // .... `somecode` no square brackets, no regex, so just read this aloud
                                keyPoints.Add(new TtsKeyPoint
                                {
                                    //AtWhatChar = curChar,
                                    Token = allTtsFlatTextsStrings[index],
                                    KeyPointType = MdKeyPointType.String,
                                });
                                // .... mark token
                                allTtsFlatTextsStrings[index] = TtsKeypointChar + allTtsFlatTextsStrings[index];
                            }
                        }
                        else if (me.TagInline == InlineTag.RawHtml)
                        {
                            var raw = allTtsFlatTextsStrings[index];
                            allTtsFlatTextsStrings[index] = string.Empty;
                            if (regexMdHighlight.IsMatch(raw))
                            {
                                var moHighlight = regexMdHighlight.Match(raw);
                                keyPoints.Add(new TtsKeyPoint
                                {
                                    //AtWhatChar = curChar,
                                    Token = moHighlight.Groups[1].Value,
                                    KeyPointType = MdKeyPointType.HighlightLines,
                                });
                                // .... mark token
                                allTtsFlatTextsStrings[index] = TtsKeypointChar.ToString();
                            }
                        }
                        //curChar += allTtsFlatTextsStrings[index].Length + 1; // +1 for space! see Join below
                        //Debug.WriteLine(before != null ? before.Text : "");
                        //Debug.WriteLine(me.Text);
                        //Debug.WriteLine(after != null ? after.Text : "");
                        //Debug.WriteLine("--------------------");
                    });
                    // .... join all flat texts to one string
                    var allTtsText = String.Join(" ", allTtsFlatTextsStrings);
                    // .... remove excess whitespace and use TtsKeypointChar to establish AtWhatChar for each aquired keypoint
                    //allTtsText = 
                    var newPage = new MdAnimatedBlock
                    {
                        Code = code,
                        TtsContent = new TtsContentWithKeyPoints
                        {
                            TtsText = allTtsText,
                            KeyPoints = keyPoints,
                        }
                    };
                    newSection.Pages.Add(newPage);
                }
                result.Sections.Add(newSection);
            }

            return result;
        }

        private string StringifyBlockSimple(MdBlock block)
        {
            return String.Join(" ", flattener.FlattenBlock(block).Select(x => x.Text));
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
                    //var oopsTheseBelongToPrev = codeBlock.Take(2); // .... do not skip "Notes:"
                    var oopsTheseBelongToPrev = codeBlock.Skip(1).Take(1); // .... skip "Notes:"
                    codeSplit[index] = codeSplit[index].Skip(2);
                    codeSplit[index-1] = codeSplit[index-1].Union(oopsTheseBelongToPrev).ToArray();
                }
            });
            return codeSplit.Where(x => x.Any()); // .... remove empty entries
            //return codeSplit;
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

        private bool IsCodeBlock(MdBlock b)
        {
            return b.Tag == BlockTag.IndentedCode ||
                    b.Tag == BlockTag.FencedCode;
        }
    }
}

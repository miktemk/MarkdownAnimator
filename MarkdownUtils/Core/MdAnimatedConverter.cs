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
using Miktemk.TextToSpeech.Core;

namespace MarkdownUtils.Core
{
    public class MdAnimatedConverter
    {
        private const char TtsKeypointChar = (char)0x0;

        private readonly string[] NotesHeaders;
        private readonly MdBlockFlattener flattener;
        private Regex regexMdHighlight, regexTtsOpen, regexTtsClose;

        public MdAnimatedConverter()
        {
            NotesHeaders = Settings.Default.NotesText.Split('|');
            flattener = new MdBlockFlattener();
            regexMdHighlight = new Regex("<md-highlight\\slines=\"(.*?)\".*\\/>");
            regexTtsOpen = new Regex("<tts\\slang=\"(.*?)\".*>");
            regexTtsClose = new Regex("<\\/tts>");
        }

        public MdAnimatedDocument MdDocument2Animated(MdDocument mdDoc)
        {
            var result = new MdAnimatedDocument();

            // .... find H1 heading for a title
            var h1Heading = mdDoc.Blocks.FirstOrDefault(x => x.HeaderLevel == 1);
            if (h1Heading != null)
                result.Title = StringifyBlockSimple(h1Heading);

            var splitSections = mdDoc.Blocks.SplitEnumerableStartingWith(b => b.HeaderLevel > 0, true);
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
                    // .... flatten blocks
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
                    // .... 
                    var allTtsFlatTexts = TtsFlatParagraphs.SelectMany(x => x);
                    var eventizedTtsText = allTtsFlatTexts.ConsumeAsTokenChars(
                        MdInlineFlat2Char,
                        new Dictionary<string, Func<IEnumerable<MdInlineFlat>, IEnumerable<MdAnimatedEvent>>>
                        {
                            // .... https://regex101.com/r/lG0oP0/4
                            { @"\[r?t(?:,r?t)*\]", MakeMdEventFromRawTokens },
                            // .... https://regex101.com/r/cQ4tE4/1
                            { @"\{a+\}", MakeMdEventFromTts },
                            { @"t", blocks => blocks.Select(x => new MdAnimatedEvent(x.Text, new [] { new TtsKeyPoint(x.Text) })) },
                            { @"h+", MakeMdEventsFromHighlightLines },
                            { @"a", blocks => blocks.Select(x => new MdAnimatedEvent(x.Text)) },
                        }
                    );

                    var builderTotal = new StringBuilder();
                    var keyPoints = new List<TtsKeyPoint>();
                    var ttsText = new MultiLanguageText();

                    // .... join all flat texts into TTS multilang string and build keypoints where they occur in this sequence
                    eventizedTtsText.EnumerateSublistsEndingWith(x => x.Lang != null,
                        (beforeTTS, ttsEvent, index) =>
                        {
                            var builderUpToTts = new StringBuilder();
                            foreach (var animEvent in beforeTTS)
                            {
                                // .... handle key points
                                if (animEvent.KeyPoints?.Any() ?? false)
                                {
                                    foreach (var kp in animEvent.KeyPoints)
                                    {
                                        kp.AtWhatChar = builderTotal.Length; // .... where they occur in this sequence!
                                        keyPoints.Add(kp);
                                    }
                                }
                                // .... handle text
                                if (animEvent.Text != null)
                                {
                                    builderUpToTts.Append(animEvent.Text);
                                    builderTotal.Append(animEvent.Text);
                                }
                            }
                            if (builderUpToTts.Length > 0)
                            {
                                ttsText.Phrases.Add(new UniLangPhrase
                                {
                                    Lang = "en", //TODO: define default language
                                    Text = builderUpToTts.ToString()
                                });
                            }
                            // .... handle other language segment
                            if (ttsEvent != null)
                            {
                                builderTotal.Append(ttsEvent.Text);
                                ttsText.Phrases.Add(new UniLangPhrase
                                {
                                    Lang = ttsEvent.Lang,
                                    Text = ttsEvent.Text,
                                });
                            }
                        });

                    // .... Old approach: remove excess whitespace and use TtsKeypointChar to establish AtWhatChar for each aquired keypoint
                    //var allTtsText = String.Join(" ", allTtsFlatTextsStrings);
                    //allTtsText = allTtsText
                    //    .ClusterChar(TtsKeypointChar, new[] { ' ' }, true)
                    //    .Trim()
                    //    .MakeCharUnique(' ')
                    //    .TransformWithCallback(
                    //        new[] {
                    //            new CharAndCallback(TtsKeypointChar, (charIndex, index) => {
                    //                keyPoints[index].AtWhatChar = charIndex;
                    //            }),
                    //        });
                    var newPage = new MdAnimatedBlock
                    {
                        Code = code,
                        TtsTextString = builderTotal.ToString(),
                        KeyPoints = keyPoints,
                        TtsText = ttsText,
                    };
                    newSection.Pages.Add(newPage);
                }
                result.Sections.Add(newSection);
            }

            return result;
        }

        /// <summary>
        /// example rawTokens: {aa}
        /// </summary>
        private IEnumerable<MdAnimatedEvent> MakeMdEventFromTts(IEnumerable<MdInlineFlat> rawTokens)
        {
            var moTtsOpen = regexTtsOpen.Match(rawTokens.FirstOrDefault().Text);
            var lang = moTtsOpen.Groups[1].Value;
            var contentStr = rawTokens
                .Skip(1).DropLast()
                .Select(x => x.Text)
                .StringJoin(" ");
            return new[] { new MdAnimatedEvent(contentStr) { Lang = lang } };
        }

        /// <summary>
        /// example rawTokens: [t,rt,t,rt]
        /// </summary>
        private IEnumerable<MdAnimatedEvent> MakeMdEventFromRawTokens(IEnumerable<MdInlineFlat> rawTokens)
        {
            var keyPoints = rawTokens.ConsumeAsTokenChars(
                MdInlineFlat2Char,
                new Dictionary<string, Func<IEnumerable<MdInlineFlat>, IEnumerable<TtsKeyPoint>>>
                {
                    { @"rt", blocks => new [] { new TtsKeyPoint(blocks.Skip(1).FirstOrDefault().Text, MdKeyPointType.Regex) } },
                    { @"t",  blocks => new [] { new TtsKeyPoint(blocks.FirstOrDefault().Text, MdKeyPointType.String) } },
                }
            );
            return new[] { new MdAnimatedEvent("", keyPoints) };
        }

        /// <summary>
        /// example rawTokens: hhh
        /// </summary>
        private IEnumerable<MdAnimatedEvent> MakeMdEventsFromHighlightLines(IEnumerable<MdInlineFlat> rawTokens)
        {
            var keyPoints = rawTokens.Select(token =>
            {
                var moMdHighlight = regexMdHighlight.Match(token.Text);
                return new TtsKeyPoint
                {
                    Token = moMdHighlight.Groups[1].Value,
                    KeyPointType = MdKeyPointType.HighlightLines,
                };
            });
            return new[] { new MdAnimatedEvent("", keyPoints) };
        }

        /// <summary>
        /// example output: aa[rt]a[rt,t]aa[t,rt,t,rt]aaaaahaha{a}
        /// </summary>
        private char MdInlineFlat2Char(MdInlineFlat x)
        {
            if (x.Text == "[") return '[';
            if (x.Text == "]") return ']';
            if (x.Text == "r") return 'r';
            if (x.TagInline == InlineTag.Code) return 't';
            if (x.TagInline == InlineTag.RawHtml)
            {
                if (IsMdHighlight(x)) return 'h';
                if (IsTtsOpen(x)) return '{';
                if (IsTtsClose(x)) return '}';
            }
            if (IsComma(x)) return ',';
            return 'a';
        }

        private bool IsComma(MdInlineFlat x) { return x.Text.Trim() == ","; }
        private bool IsMdHighlight(MdInlineFlat x) { return regexMdHighlight.IsMatch(x.Text); }
        private bool IsTtsOpen(MdInlineFlat x) { return regexTtsOpen.IsMatch(x.Text); }
        private bool IsTtsClose(MdInlineFlat x) { return regexTtsClose.IsMatch(x.Text); }

        private string StringifyBlockSimple(MdBlock block)
        {
            return String.Join(" ", flattener.FlattenBlock(block).Select(x => x.Text));
        }

        private IEnumerable<IEnumerable<MdBlock>> SplitMdSectionsIntoCodeBlocksNotesConsidered(IEnumerable<MdBlock> sectionBlocks)
        {
            var codeSplit = sectionBlocks
                .SplitEnumerableEndingWith(b => IsCodeBlock(b), true)
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
                    codeSplit[index - 1] = codeSplit[index - 1].Union(oopsTheseBelongToPrev).ToArray();
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

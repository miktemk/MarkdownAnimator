using GalaSoft.MvvmLight;
using MarkdownUtils.MdAnimated;
using Miktemk;
using Miktemk.TextToSpeech.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkdownAnimator.ViewModel
{
    public class MdDocumentTtsReader : ViewModelBase
    {
        private Func<MdDocumentEnumerator> funcGetDocPager;
        private ITtsService ttsService;
        private int lengthsOfPreviouslySaidPhrases;

        public bool IsPlaying { get; private set; } = false;

        public MdDocumentTtsReader(Func<MdDocumentEnumerator> funcGetDocPager, ITtsService ttsService)
        {
            this.funcGetDocPager = funcGetDocPager;
            this.ttsService = ttsService;
            ttsService.AddWordCallback(wordCallback);
        }

        public void PlayStop()
        {
            if (ttsService.IsPlaying)
            {
                Stop();
                return;
            }
            IsPlaying = true;
            SayCurPage();
        }

        public void Stop()
        {
            IsPlaying = false;
            ttsService.StopCurrentSynth();
        }

        public void SetSpeed(int speed)
        {
            ttsService.SetVoiceOverrideSpeed(speed);
        }

        private void SayCurPage()
        {
            var docPager = funcGetDocPager();
            if (docPager.CurPage == null)
                return;
            ttsService.SayAsyncMany(docPager.CurPage.TtsText, (phrase, index) =>
            {
                if (index > 0)
                {
                    // if #2 and more, add previous phrase's length to lengthsOfPreviouslySaidPhrases
                    lengthsOfPreviouslySaidPhrases += docPager.CurPage.TtsText.Phrases[index - 1].Text.Length;
                }
                if (phrase == null) // .... AKA last
                    GoToNextPage();
            }, 0, 0);

            Debug.WriteLine("========== page ============");
            Debug.WriteLine($"new-code: {docPager.CurPage.Code}");
            Debug.WriteLine($"saying: {docPager.CurPage.TtsTextString}");
        }

        private void GoToNextPage()
        {
            var docPager = funcGetDocPager();
            var backToStart = docPager.GoToNextPage();
            if (!backToStart)
            {
                lengthsOfPreviouslySaidPhrases = 0;
                SayCurPage();
            }
            else
                IsPlaying = false;
        }

        private void wordCallback(string text, int offset, int start, int length)
        {
            var indexTotal = offset + lengthsOfPreviouslySaidPhrases;
            var docPager = funcGetDocPager();
            docPager.ReadUntilThisPoint(indexTotal);
        }
    }
}

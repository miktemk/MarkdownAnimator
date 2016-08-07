using CommonMark;
using GalaSoft.MvvmLight;
using MarkdownUtils.Core;
using Miktemk.TextToSpeech.Services;
using System;
using System.IO;
using System.Linq;
using MarkdownUtils.MdAnimated;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using System.Diagnostics;
using ICSharpCode.AvalonEdit.Document;
using System.Collections.Generic;

namespace MarkdownAnimator.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private ITtsService ttsService;
        private readonly Stack<TtsKeyPoint> curPageKeyPoints = new Stack<TtsKeyPoint>();

        public ICommand PlayStopCommand { get; }
        public ICommand OpenFileCommand { get; }

        public MdDocumentEnumerator DocPager { get; private set; }
        public IEnumerable<TtsKeyPoint> CurKeyPoints { get; private set; }
        public TextDocument CodeDocument { get; } = new TextDocument();

        public MainViewModel(ITtsService ttsService)
        {
            this.ttsService = ttsService;

            // .... commands
            PlayStopCommand = new RelayCommand(PlayStop);
            OpenFileCommand = new RelayCommand(OpenFile);

            // DEBUG .... load from command line argument
            var argFileToLoad = Environment.GetCommandLineArgs().Skip(1).FirstOrDefault();
            if (argFileToLoad != null)
                LoadFile(argFileToLoad);
        }

        private void OpenFile()
        {

        }

        private void LoadFile(string filename)
        {
            var converter = new MdDocumentConverter();
            var converterAnim = new MdAnimatedConverter();

            var mdText = File.ReadAllText(filename);
            var doc = CommonMarkConverter.Parse(mdText);
            var mdDoc = converter.CommonMark2MdDocument(doc);
            var mdDocAnim = converterAnim.MdDocument2Animated(mdDoc);

            DocPager = new MdDocumentEnumerator(mdDocAnim);

            ttsService.AddWordCallback(wordCallback);
        }

        private void PlayStop()
        {
            if (ttsService.IsPlaying)
            {
                ttsService.StopCurrentSynth();
                return;
            }
            SayCurPage();
        }

        private void SayCurPage()
        {
            var curPage = DocPager.CurPage;
            if (curPage == null)
                return;

            CodeDocument.Text = curPage.Code;
            CurKeyPoints = null;

            curPageKeyPoints.Clear();
            foreach (var keyEvent in curPage.TtsContent.KeyPoints.OrderByDescending(x => x.AtWhatChar))
                curPageKeyPoints.Push(keyEvent);

            Debug.WriteLine("========== page ============");
            Debug.WriteLine($"new-code: {curPage.Code}");
            Debug.WriteLine($"saying: {curPage.TtsContent.TtsText}");
            ttsService.SayAsync("en", curPage.TtsContent.TtsText, () =>
            {
                GoToNextPage();
            });
        }

        private void GoToNextPage()
        {
            var backToStart = DocPager.GoToNextPage();
            if (!backToStart)
                SayCurPage();
        }

        private void wordCallback(string str, int index, int len)
        {
            if (!curPageKeyPoints.Any())
                return;
            var listPoppedKPs = new List<TtsKeyPoint>();
            while (curPageKeyPoints.Any() && curPageKeyPoints.Peek() != null && index >= curPageKeyPoints.Peek().AtWhatChar)
            {
                var kp = curPageKeyPoints.Pop();
                listPoppedKPs.Add(kp);
            }
            CurKeyPoints = listPoppedKPs.ToArray();

            foreach (var kp in CurKeyPoints)
            {
                Debug.WriteLine("------- keypoints -----------");
                Debug.WriteLine(kp.KeyPointType);
                Debug.WriteLine(kp.Token);
            }
        }
    }
}
//#define TMP_TEST

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
using PropertyChanged;
using Miktemk;
using MarkdownAnimator.Code;
using System.Windows;
using MarkdownAnimator.Properties;

namespace MarkdownAnimator.ViewModel
{
    public class MainViewModel : ViewModelBase
    {

        public MdDocumentEnumerator DocPager { get; private set; }
        public MdDocumentTtsReader DocReader { get; private set; }

        // bindables
        public bool IsSidebarVisible { get; private set; } = true;
        public TextDocument CodeDocument { get; } = new TextDocument();
        public IEnumerable<TtsKeyPoint> CurKeyPoints { get; private set; }

        // commands
        public ICommand PlayStopCommand { get; }
        public ICommand OpenFileCommand { get; }
        public ICommand ToggleSidebarCollapseCommand { get; }

        public MainViewModel(ITtsService ttsService)
        {
            // .... commands
            PlayStopCommand = new RelayCommand(PlayStop);
            OpenFileCommand = new RelayCommand(OpenFile);
            ToggleSidebarCollapseCommand = new RelayCommand(() => { IsSidebarVisible = !IsSidebarVisible; });

            DocReader = new MdDocumentTtsReader(() => DocPager, ttsService);

            // DEBUG .... load from command line argument
#if TMP_TEST
            //LoadFile(@"C:\Users\Mikhail\Google Drive\md-notes\mdanim-sample.md");
            LoadFile(@"C:\Users\Mikhail\Google Drive\md-notes\mdanim-csharp.md");
#else
            var argFileToLoad = Environment.GetCommandLineArgs().Skip(1).FirstOrDefault();
            if (argFileToLoad != null)
                LoadFile(argFileToLoad);
#endif
        }

        private void OpenFile()
        {
            //TODO: open file dialog
        }

        private void LoadFile(string filename)
        {
            DocReader.Stop();

            MdAnimatedDocument mdDocAnim = Utils.LoadMdDocAnim(filename);

            UtilsRegistry.OpenUserSoftwareKey(Settings.Default.RegRoot);

            DocPager = new MdDocumentEnumerator(mdDocAnim);
            DocPager.PageChanged += DocPager_PageChanged;
            DocPager.UpdateCurKeyPoints += DocPager_UpdateCurKeyPoints;
            DocPager.ResetToBeginning();
        }

        private void PlayStop()
        {
            if (DocReader != null)
                DocReader.PlayStop();
        }

        private void DocPager_PageChanged()
        {
            CodeDocument.Text = (DocPager.CurPage == null)
                ? string.Empty
                : DocPager.CurPage.Code;
        }

        private void DocPager_UpdateCurKeyPoints(IEnumerable<TtsKeyPoint> keypoints)
        {
            CurKeyPoints = keypoints;
        }

    }
}
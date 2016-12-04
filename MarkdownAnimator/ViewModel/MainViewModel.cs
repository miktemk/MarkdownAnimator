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
using Miktemk.Wpf.ViewModels;
using Miktemk.Wpf.Services;
using Miktemk.TextToSpeech.Wpf.ViewModels;
using MvvmDialogs;
using Miktemk.Wpf.Controls;

namespace MarkdownAnimator.ViewModel
{
    public class MainViewModel : MainViewModelTts
    {
        public MdDocumentEnumerator DocPager { get; private set; }
        private MdDocumentTtsReader _DocReader;
        public MdDocumentTtsReader DocReader => _DocReader ?? (_DocReader = new MdDocumentTtsReader(() => DocPager, ttsService));

        // bindables
        public TextDocument CodeDocument { get; } = new TextDocument();
        public IEnumerable<TtsKeyPoint> CurKeyPoints { get; private set; }

        public MainViewModel(
            ITtsService ttsService,
            IRegistryService registryService,
            IDialogService dialogService)
            : base(ttsService, registryService, dialogService)
        {
            ttsService.SetVoiceOverrideSpeedDefaultOnly(TtsSpeed);
        }

        protected override void PlayPause(PlayPauseButton.PlayState state)
        {
            if (DocReader != null)
                DocReader.PlayStop();
        }

        protected override void LoadFile(string filename)
        {
            DocReader.Stop();

            MdAnimatedDocument mdDocAnim = Utils.LoadMdDocAnim(filename);

            DocPager = new MdDocumentEnumerator(mdDocAnim);
            DocPager.PageChanged += DocPager_PageChanged;
            DocPager.UpdateCurKeyPoints += DocPager_UpdateCurKeyPoints;
            DocPager.ResetToBeginning();
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
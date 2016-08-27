using GalaSoft.MvvmLight;
using MarkdownUtils.MdAnimated;
using Miktemk;
using Miktemk.TextToSpeech.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkdownAnimator.ViewModel
{
    public class MdDocumentEnumerator : ViewModelBase
    {
        //private int curSectionIndex;
        //private int curPageIndex;
        private MdAnimatedBlock[] pagesFlat;
        private readonly Stack<TtsKeyPoint> curPageKeyPoints = new Stack<TtsKeyPoint>();

        public MdAnimatedDocument CurMDDocAnim { get; private set; }
        //public MdAnimatedDocumentSection CurSection { get; private set; }
        public int TotalPages { get; private set; }
        public int IndexGlobal { get; set; }
        public MdAnimatedBlock CurPage {
            get {
                if (IndexGlobal < 0 || IndexGlobal >= pagesFlat.Length)
                    return null;
                return pagesFlat[IndexGlobal];
            }
        }

        // event handlers
        public VoidHandler PageChanged;
        public GenericHandler<IEnumerable<TtsKeyPoint>> UpdateCurKeyPoints;

        public MdDocumentEnumerator(MdAnimatedDocument CurMDDocAnim)
        {
            this.CurMDDocAnim = CurMDDocAnim;
            IndexGlobal = -1;
            // TODO: option to display title pages
            pagesFlat = CurMDDocAnim.Sections.SelectMany(s => s.Pages).ToArray();
            //pagesFlat = CurMDDocAnim.Sections.SelectMany(s => s.Pages.Prepend(MakeTitlePage(s))).ToArray();
            TotalPages = pagesFlat.Count();
        }

        private MdAnimatedBlock MakeTitlePage(MdAnimatedDocumentSection s)
        {
            return new MdAnimatedBlock
            {
                Code = $"\n\n\n\n    {s.Title}",
                TtsText = new MultiLanguageText
                {
                    Phrases = new List<UniLangPhrase> { new UniLangPhrase { Lang = "en2", Text = s.Title } }
                }
            };
        }

        /// <summary>
        /// returns true if we reach the end and go back to start
        /// </summary>
        public bool GoToNextPage()
        {
            if (IndexGlobal >= pagesFlat.Length-1)
            {
                ResetToBeginning();
                return true;
            }
            IndexGlobal++;
            PageChanged?.Invoke();
            CompileCurPage();
            return false;
        }

        public void ResetToBeginning()
        {
            if (IndexGlobal != 0)
            {
                IndexGlobal = 0;
                PageChanged?.Invoke();
                CompileCurPage();
            }
        }

        private void CompileCurPage()
        {
            UpdateCurKeyPoints?.Invoke(null);
            curPageKeyPoints.Clear();

            foreach (var keyEvent in CurPage.KeyPoints.OrderByDescending(x => x.AtWhatChar))
                curPageKeyPoints.Push(keyEvent);
            UpdateUI();
        }

        public void ReadUntilThisPoint(int indexTotal)
        {
            if (!curPageKeyPoints.Any())
                return;
            var listPoppedKPs = new List<TtsKeyPoint>();
            while (curPageKeyPoints.Any() && curPageKeyPoints.Peek() != null && indexTotal >= curPageKeyPoints.Peek().AtWhatChar)
            {
                var kp = curPageKeyPoints.Pop();
                listPoppedKPs.Add(kp);
            }
            if (listPoppedKPs.Any())
            {
                var curKeyPoints = listPoppedKPs.ToArray();
                UpdateCurKeyPoints?.Invoke(curKeyPoints);
                UpdateUI();

                Debug.WriteLine("------- keypoints -----------");
                foreach (var kp in curKeyPoints)
                {
                    Debug.WriteLine(kp.KeyPointType);
                    Debug.WriteLine(kp.Token);
                }
            }
        }

        #region --------------------- bindable properties -----------------------

        public double GlobalProgress
        {
            get
            {
                if (CurPage == null)
                    return 0;
                return IndexGlobal + 1 - UtilsMath.XoY(curPageKeyPoints.Count + 1, CurPage.KeyPoints.Count + 1);
            }
            set
            {
                IndexGlobal = (int)value;
            }
        }
        public string ProgressLabel
        {
            get
            {
                if (CurPage == null)
                    return string.Empty;
                var keyPointTally = CurPage.KeyPoints.Count - curPageKeyPoints.Count;
                return $"{IndexGlobal}:{keyPointTally}";
            }
        }

        private void UpdateUI()
        {
            RaisePropertyChanged("GlobalProgress");
            RaisePropertyChanged("ProgressLabel");
        }

        #endregion
    }
}

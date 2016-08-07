using GalaSoft.MvvmLight;
using MarkdownUtils.MdAnimated;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkdownAnimator.ViewModel
{
    public class MdDocumentEnumerator : ViewModelBase
    {
        private int curSectionIndex;
        private int curPageIndex;
        private int curPageGlobalIndex;
        private int totalPages;

        public MdAnimatedDocument CurMDDocAnim { get; private set; }
        public MdAnimatedDocumentSection CurSection { get; private set; }
        public MdAnimatedBlock CurPage { get; private set; }
        public int TotalPages { get { return totalPages; } }
        public int IndexGlobal { get { return curPageGlobalIndex; } }

        public MdDocumentEnumerator(MdAnimatedDocument CurMDDocAnim)
        {
            this.CurMDDocAnim = CurMDDocAnim;
            ResetToBeginning();
            totalPages = CurMDDocAnim.Sections.SelectMany(s => s.Pages).Count();
        }

        /// <summary>
        /// returns true if we reach the end and go back to start
        /// </summary>
        public bool GoToNextPage()
        {
            curPageIndex++;
            curPageGlobalIndex++;
            if (curPageIndex < CurSection.Pages.Count)
                CurPage = CurSection.Pages[curPageIndex];
            else
            {
                curSectionIndex++;
                if (curSectionIndex < CurMDDocAnim.Sections.Count)
                {
                    CurSection = CurMDDocAnim.Sections[curSectionIndex];
                    CurPage = CurSection.Pages.FirstOrDefault();
                    curPageIndex = 0;
                }
                else
                {
                    ResetToBeginning();
                    return true;
                }
            }
            return false;
        }

        private void ResetToBeginning()
        {
            CurSection = CurMDDocAnim.Sections.FirstOrDefault();
            CurPage = CurSection.Pages.FirstOrDefault();
            curSectionIndex = 0;
            curPageIndex = 0;
            curPageGlobalIndex = 0;
        }
    }
}

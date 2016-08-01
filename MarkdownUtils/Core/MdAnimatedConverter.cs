using MarkdownUtils.MdDoc;
using MarkdownUtils.MdAnimated;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkdownUtils.Core
{
    public class MdAnimatedConverter
    {
        public MdAnimatedConverter()
        {

        }

        public MdAnimatedDocument MdDocument2Animated(MdDocument mdDoc)
        {
            var result = new MdAnimatedDocument();
            foreach (var block in mdDoc.Blocks)
            {
                var curPage = new MdAnimatedBlock();
                result.Pages.Add(curPage);
            }

            return result;
        }
    }
}

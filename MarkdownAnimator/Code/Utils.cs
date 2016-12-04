using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MarkdownUtils.MdAnimated;
using MarkdownUtils.Core;
using CommonMark;
using Microsoft.Win32;
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace MarkdownAnimator.Code
{
    public static class Utils
    {
        public static string GetFileFromThisAppDirectory(string subpath)
        {
            return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), subpath);
        }

        public static MdAnimatedDocument LoadMdDocAnim(string filename)
        {
            var mdmmsc = Mdmmsc.LoadFromFile(Utils.GetFileFromThisAppDirectory("App_Data/mdmmsc.txt"));
            var converter = new MdDocumentConverter();
            var converterAnim = new MdAnimatedConverter();

            var mdText = File.ReadAllText(filename);
            mdText = mdmmsc.RunReplacements(mdText);
            var doc = CommonMarkConverter.Parse(mdText);
            var mdDoc = converter.CommonMark2MdDocument(doc);
            var mdDocAnim = converterAnim.MdDocument2Animated(mdDoc);

            return mdDocAnim;
        }

    }
}

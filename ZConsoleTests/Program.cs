using CommonMark;
using MarkdownUtils.Core;
using MarkdownUtils.MdDoc;
using Miktemk;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZConsoleTests
{
    class Program
    {
        static void Main(string[] args)
        {
            new Program();
        }

        const string userName = "miktemk";
        //const string userName = "Mikhail";

        public Program()
        {
            var mdText = File.ReadAllText($@"C:\Users\{userName}\Google Drive\md-notes\mdanim-sample.md");
            var doc = CommonMarkConverter.Parse(mdText);
            //var result = CommonMarkConverter.Convert(mdText);
            //File.WriteAllText(@"C:\Users\Mikhail\Desktop\___test.html", result);

            var converter = new MdDocumentConverter();
            var mdDoc = converter.CommonMark2MdDocument(doc);

            var converterAnim = new MdAnimatedConverter();
            var mdDocAnim = converterAnim.MdDocument2Animated(mdDoc);

            File.WriteAllText(OnDesktop("mdDoc.json"), JsonConvert.SerializeObject(mdDoc, Formatting.Indented));
            //File.WriteAllText($@"C:\Users\{userName}\Desktop\mdDocAnim.json", JsonConvert.SerializeObject(mdDocAnim, Formatting.Indented));

            XmlFactory.WriteToFile(mdDoc, OnDesktop("mdDoc.xml"));
            XmlFactory.WriteToFile(mdDocAnim, OnDesktop("mdDocAnim.xml"));
        }

        private string OnDesktop(string filename)
        {
            return Path.Combine($@"C:\Users\{userName}\Desktop", filename);
        }
    }
}

using CommonMark;
using MarkdownUtils.Core;
using Miktemk;
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
            var mdText = File.ReadAllText(@"C:\Users\Mikhail\Google Drive\md-notes\js-regex.md");
            var doc = CommonMarkConverter.Parse(mdText);
            //var result = CommonMarkConverter.Convert(mdText);
            //File.WriteAllText(@"C:\Users\Mikhail\Desktop\___test.html", result);

            var converter = new MdDocumentConverter();
            var mdDoc = converter.CommonMark2MdDocument(doc);

            XmlFactory.WriteToFile(mdDoc, @"C:\Users\Mikhail\Desktop\___test.xml");
        }
    }
}

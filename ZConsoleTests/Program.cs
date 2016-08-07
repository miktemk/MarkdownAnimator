﻿using CommonMark;
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
            //const string userName = "miktemk";
            const string userName = "Mikhail";

            var mdText = File.ReadAllText($@"C:\Users\{userName}\Google Drive\md-notes\js-regex.md");
            var doc = CommonMarkConverter.Parse(mdText);
            //var result = CommonMarkConverter.Convert(mdText);
            //File.WriteAllText(@"C:\Users\Mikhail\Desktop\___test.html", result);

            var converter = new MdDocumentConverter();
            var mdDoc = converter.CommonMark2MdDocument(doc);

            var converterAnim = new MdAnimatedConverter();
            var mdDocAnim = converterAnim.MdDocument2Animated(mdDoc);

            File.WriteAllText($@"C:\Users\{userName}\Desktop\mdDoc.json", JsonConvert.SerializeObject(mdDoc, Formatting.Indented));
            //File.WriteAllText($@"C:\Users\{userName}\Desktop\mdDocAnim.json", JsonConvert.SerializeObject(mdDocAnim, Formatting.Indented));

            XmlFactory.WriteToFile(mdDoc, @"C:\Users\Mikhail\Desktop\mdDoc.xml");
            XmlFactory.WriteToFile(mdDocAnim, @"C:\Users\Mikhail\Desktop\mdDocAnim.xml");
        }
    }
}

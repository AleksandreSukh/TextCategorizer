using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Word;
using PdfiumViewer;
using Pri.LongPath;

namespace DataAggregator
{
    public class FileScraper
    {
        private const string PdfExtension = ".pdf";
        private const string MsWordDocExtension = ".doc";
        private const string MsWordDocxExtension = ".docx";
        private const string TextFileExtension = ".txt";
        private const string OutputFileExtension = TextFileExtension;

        public static void ScrapFile(FileInfo inputFilePath, string outputFilePath, bool updateMode)
        {
            outputFilePath += OutputFileExtension;
            if (updateMode && File.Exists(outputFilePath))
            {
                Console.WriteLine($"{nameof(ScrapFile)} Skipping:{inputFilePath} because already scraped {outputFilePath}");
                return;
            }

            Console.WriteLine($"Scraping:{inputFilePath} to {outputFilePath}");
            Func<string, string, bool> actionToUse;

            switch (inputFilePath.Extension.ToLower())
            {
                case PdfExtension:
                    actionToUse = ScrapPdf;
                    break;
                case MsWordDocExtension:
                    actionToUse = ScrapWordDoc;
                    break;
                case MsWordDocxExtension:
                    actionToUse = ScrapWordDocx;
                    break;
                default:
                    return;
            }

            if (actionToUse(inputFilePath.FullName, outputFilePath))
                //Process.Start(outputFilePath);//TODO:Debug
                ;
        }

        private static bool ScrapWordDocx(string fullName, string outputFilePath)
        {
            return ScrapWordDoc(fullName, outputFilePath);
        }
        private static bool ScrapWordDoc(string fullName, string outputFilePath)
        {
            //Skip lock files
            if (Path.GetFileName(fullName).StartsWith("~$")) return false;

            var word = new Application();
            var docs = word.Documents.Open(fullName, ReadOnly: true);
            var totaltext = "";
            for (var i = 0; i < docs.Paragraphs.Count; i++)
            {
                totaltext += " \r\n " + docs.Paragraphs[i + 1].Range.Text;
            }

            try { docs.Close(); } catch (COMException e) {/**/}
            try { word.Quit(); } catch (COMException e) {/**/}
            File.WriteAllText(outputFilePath, totaltext);
            return true;
        }

        private static bool ScrapPdf(string fullName, string outputFilePath)
        {
            var doc = PdfDocument.Load(fullName);
            List<string> pageTexts = new List<string>();
            for (int i = 0; i < doc.PageCount; i++)
            {
                var pageText = doc.GetPdfText(i);
                pageTexts.Add(pageText);
            }

            var scrapedFilePath = outputFilePath;
            File.WriteAllLines(scrapedFilePath, pageTexts);
            return true;
        }
    }
}
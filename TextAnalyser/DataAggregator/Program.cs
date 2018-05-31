using System;
using System.Collections.Generic;
using System.Diagnostics;
using Path = Pri.LongPath.Path;
using Directory = Pri.LongPath.Directory;
using DirectoryInfo = Pri.LongPath.DirectoryInfo;
using File = Pri.LongPath.File;
using FileInfo = Pri.LongPath.FileInfo;
using FileSystemInfo = Pri.LongPath.FileSystemInfo;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Word;
using PdfiumViewer;

namespace DataAggregator
{
    class Program
    {
        private const string PdfExtension = ".pdf";
        private const string MsWordDocExtension = ".doc";
        private const string MsWordDocxExtension = ".docx";
        private const string TextFileExtension = ".txt";
        private const string OutputFileExtension = TextFileExtension;
        static void Main(string[] args)
        {
            string inputDir = @"C:\Users\sandro\Downloads\წიგნები-20180531T092702Z-002";
            string output;

            if (args.Length == 2)
            {
                inputDir = args[0];
                output = args[1];
            }
            if (args.Length == 1)
            {
                inputDir = args[0];
            }

            output = inputDir + "_out";


            if (!Directory.Exists(output))
                Directory.CreateDirectory(output);

            //Recursively
            ScrapFiles(inputDir, output, true);





        }

        private static void ScrapFiles(string inputDir, string output, bool updateMode)
        {
            int maxFileNameLength = 50;
            foreach (var inputFilePath in inputDir.ToDirectoryInfo().EnumerateFiles())
            {
                var inputFileThatMayBeChanged = inputFilePath;
                //TODO:Can be injected
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(inputFilePath.FullName);
                if (fileNameWithoutExtension.Length > maxFileNameLength)
                {
                    var shortenedNameWithoutExtension = new string(fileNameWithoutExtension.ToCharArray().Take(maxFileNameLength).ToArray());
                    var shortenedName = shortenedNameWithoutExtension + inputFilePath.Extension;
                    var shortenedPath = Path.Combine(inputFilePath.Directory.FullName, shortenedName);
                    File.Move(inputFilePath.FullName, shortenedPath);
                    inputFileThatMayBeChanged = new FileInfo(shortenedPath);
                }



                var outputFilePath = Path.Combine(output, inputFileThatMayBeChanged.Name + OutputFileExtension);
                ActuallyScrapFile(inputFileThatMayBeChanged, outputFilePath, updateMode);//TODO:can be made generic
            }

            foreach (var inputDirPath in inputDir.ToDirectoryInfo().EnumerateDirectories())
            {
                var outputDirPath = Path.Combine(output, inputDirPath.Name);
                if (!Directory.Exists(outputDirPath))
                    Directory.CreateDirectory(outputDirPath);
                ScrapFiles(inputDirPath.FullName, outputDirPath, updateMode);
            }
        }

        private static void ActuallyScrapFile(FileInfo inputFilePath, string outputFilePath, bool updateMode)
        {
            if (updateMode && File.Exists(outputFilePath))
            {
                Console.WriteLine($"Skipping:{inputFilePath} because already scraped {outputFilePath}");
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
            var docs = word.Documents.Open(fullName);
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

    public static class IOExtensions
    {
        public static DirectoryInfo ToDirectoryInfo(this string path)
        {
            return new DirectoryInfo(path);
        }
    }
}

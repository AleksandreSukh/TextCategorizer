using System;
using System.Diagnostics;
using Directory = Pri.LongPath.Directory;
using FileSystemInfo = Pri.LongPath.FileSystemInfo;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GeorgianWordDetector;

namespace DataAggregator
{
    class Program
    {
        static void Main(string[] args)
        {
            //new WordsDetector();
            string inputDir = @"C:\Users\sandro\Downloads\წიგნები1";
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

            var output0 = inputDir;


            var output1 = inputDir + "_";

            void PdfAndDocScraper() => IoExtensions.AggregateFilesInDirRecursively(output0, output1, true, FileScraper.ScrapFile);

            var output2 = output1 + "_";
            void SpaceRemover() => IoExtensions.AggregateFilesInDirRecursively(output1, output2, true, DataAggregator.SpaceRemover.CleanUpSpace);

            var output3 = output2 + "_";

            void LatinGeoFixer() => IoExtensions.AggregateFilesInDirRecursively(output2, output3, true, DataAggregator.LatinGeoFixer.FixLatinCharactersOrJustCopy);

            void ModelUpdater() => IoExtensions.AggregateFilesInDirRecursively(output3, output3, true, DataAggregator.GerogianTextModelUpdater.FeedFile);

            ModelUpdater();

            SpaceRemover();

            LatinGeoFixer();


            PdfAndDocScraper();

            //var t3 = RunActionInEvery20Seconds(action);

            //Task.WaitAll(t1, t2, t3);
        }

        private static Task RunActionInEvery20Seconds(Action action)
        {
            return Task.Run(() =>
            {
                while (true)
                {
                    action();
                    Thread.Sleep(20000);
                }
            });
        }
    }
}

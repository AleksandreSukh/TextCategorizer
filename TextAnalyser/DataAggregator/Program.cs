using System;
using System.Diagnostics;
using Directory = Pri.LongPath.Directory;
using FileSystemInfo = Pri.LongPath.FileSystemInfo;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GeorgianWordDetector;
using Pri.LongPath;

namespace DataAggregator
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {


                //new WordsDetector();
                string inputDir = @"D:\Aleks\DataRepository\წიგნები1";
                //string output;

                //if (args.Length == 2)
                //{
                //    inputDir = args[0];
                //    output = args[1];
                //}
                //if (args.Length == 1)
                //{
                //    inputDir = args[0];
                //}

                var sourceDir = new DirectoryInfo(inputDir);


                var scraped = Path.Combine(sourceDir.Parent.FullName, sourceDir.Name + "_1_scraped");

                void PdfAndDocScraper() => IoExtensions.AggregateFilesInDirRecursively(sourceDir.FullName, scraped, true, FileScraper.ScrapFile);

                var spacesClean = Path.Combine(sourceDir.Parent.FullName, sourceDir.Name + "_2_spaces_rem");
                void SpaceRemover() => IoExtensions.AggregateFilesInDirRecursively(scraped, spacesClean, true, DataAggregator.SpaceRemover.CleanUpSpace);

                var latinToGeoFixed = Path.Combine(sourceDir.Parent.FullName, sourceDir.Name + "_3_fix_lat_chars");

                void LatinGeoFixer() => IoExtensions.AggregateFilesInDirRecursively(spacesClean, latinToGeoFixed, true, DataAggregator.LatinGeoFixer.FixLatinCharactersOrJustCopy);

                var onlyGeorgian = Path.Combine(sourceDir.Parent.FullName, sourceDir.Name + "_4_only_geo");

                void GeorgianTextFilter() => IoExtensions.AggregateFilesInDirRecursively(latinToGeoFixed, onlyGeorgian, true, DataAggregator.LatinGeoFixer.FilterNonGeorgianTexts);

                void ModelUpdater() => IoExtensions.AggregateFilesInDirRecursively(onlyGeorgian, onlyGeorgian, true, DataAggregator.GerogianTextModelUpdater.FeedFile);





                //PdfAndDocScraper();

                //SpaceRemover();

                LatinGeoFixer();

                GeorgianTextFilter();

                //ModelUpdater();
                //GerogianTextModelUpdater.Save();

                //var t3 = RunActionInEvery20Seconds(action);

                //Task.WaitAll(t1, t2, t3);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
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

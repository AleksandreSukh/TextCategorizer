using System.Diagnostics;
using Directory = Pri.LongPath.Directory;
using FileSystemInfo = Pri.LongPath.FileSystemInfo;
using System.Text;
using System.Threading.Tasks;
using GeorgianWordDetector;
using Pri.LongPath;

namespace DataAggregator
{
    class Program
    {
        static void Main(string[] args)
        {
            //new WordsDetector();
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

            //Recursively scrap files from dirs
            //IoExtensions.AggregateFilesInDirRecursively(inputDir, output, true,
            //        (inputFile, outputFile, updateMode) => FileScraper.ScrapFile(inputFile, outputFile, updateMode));

            inputDir = output;
            output += "_";

            IoExtensions.AggregateFilesInDirRecursively(inputDir, output, true,
                    (inputFile, outputFile, updateMode) => SpaceRemover.Clean(inputFile, outputFile, updateMode));

            inputDir = output;
            output += "_clean";

            IoExtensions.AggregateFilesInDirRecursively(inputDir, output, true,
                (inputFile, outputFile, updateMode) => LatinGeoFixer.FixLatinCharactersOrSkip(inputFile, outputFile, updateMode));

        }


    }

    internal class SpaceRemover
    {
        public static void Clean(FileInfo inputFile, string outputFile, bool updateMode)
        {
            var oldText = File.ReadAllText(inputFile.FullName);
            var newText = System.Text.RegularExpressions.Regex.Replace(oldText, @"\s+", " ");
            File.WriteAllText(outputFile, newText);
        }
    }
}

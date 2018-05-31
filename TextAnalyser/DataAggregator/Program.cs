using System.Diagnostics;
using Directory = Pri.LongPath.Directory;
using FileSystemInfo = Pri.LongPath.FileSystemInfo;
using System.Text;
using System.Threading.Tasks;
using GeorgianWordDetector;

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
            output += "_clean";

            IoExtensions.AggregateFilesInDirRecursively(inputDir, output, true,
                (inputFile, outputFile, updateMode) => LatinGeoFixer.FixLatinCharactersOrSkip(inputFile, outputFile, updateMode));

        }


    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator
{
    class Program
    {
        static void Main(string[] args)
        {
            string inputDir = Directory.GetCurrentDirectory();
            string output = Path.Combine(inputDir, "output");

            if (args.Length == 2)
            {
                inputDir = args[0];
                output = args[1];
            }

            if (!Directory.Exists(output))
                Directory.CreateDirectory(output);

            //Recursively
            ScrapFiles(inputDir, output);





        }

        private static void ScrapFiles(string inputDir, string output)
        {
            foreach (var inputFilePath in inputDir.ToDirectoryInfo().EnumerateFiles())
            {
                var outputFilePath = Path.Combine(output, inputFilePath.Name);
                ActuallyScrapFile(inputFilePath.FullName, outputFilePath);//TODO:can be made generic
            }

            foreach (var inputDirPath in inputDir.ToDirectoryInfo().EnumerateDirectories())
            {
                var outputDirPath = Path.Combine(inputDir, inputDirPath.Name);
                ScrapFiles(inputDir, outputDirPath);
            }
        }

        private static void ActuallyScrapFile(string inputFilePath, string outputFilePath)
        {
            throw new NotImplementedException();
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

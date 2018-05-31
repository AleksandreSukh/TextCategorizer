using System;
using System.Linq;
using Pri.LongPath;

namespace DataAggregator
{
    public static class IoExtensions
    {
        public static void AggregateFilesInDirRecursively(string inputDir, string output, bool updateMode, Action<FileInfo, string, bool> InputOutputUpdateMode)
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

                var outputFilePath = Path.Combine(output, inputFileThatMayBeChanged.Name);
                InputOutputUpdateMode(inputFileThatMayBeChanged, outputFilePath, updateMode);
            }

            foreach (var inputDirPath in inputDir.ToDirectoryInfo().EnumerateDirectories())
            {
                var outputDirPath = Path.Combine(output, inputDirPath.Name);
                if (!Directory.Exists(outputDirPath))
                    Directory.CreateDirectory(outputDirPath);
                AggregateFilesInDirRecursively(inputDirPath.FullName, outputDirPath, updateMode, InputOutputUpdateMode);
            }
        }

        public static DirectoryInfo ToDirectoryInfo(this string path)
        {
            return new DirectoryInfo(path);
        }
    }
}
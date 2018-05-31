using System;
using System.Linq;
using System.Security.Cryptography;
using Pri.LongPath;
using IOException = System.IO.IOException;

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


                    var originalLongPath = inputFilePath.FullName;
                    if (File.Exists(shortenedPath))
                        if (FileExtensions.FilesAreDifferent(originalLongPath, shortenedPath))
                            FileExtensions.MoveFileWithRenaming(originalLongPath, shortenedPath, shortenedPath);
                        else
                        {
                            File.Delete(shortenedPath);
                            File.Move(originalLongPath, shortenedPath);
                        }
                    else File.Move(originalLongPath, shortenedPath);
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
    public class FileExtensions
    {
        public static void MoveFileWithRenaming(string source, string target, string initialTarget, int ctr = 0)
        {
            if (!File.Exists(source))
            {
                throw new IOException("Source file missing");
            }

            var targetFileExists = File.Exists(target);

            if (!targetFileExists || FilesAreDifferent(source, target))
            {
                if (targetFileExists)
                    MoveFileWithRenaming(source,
                        $"{Path.GetFileNameWithoutExtension(initialTarget)}{ctr++}{Path.GetExtension(initialTarget)}",
                        initialTarget, ctr++);
                else File.Move(source, target);
            }
            else
            {
                File.Delete(source);
            }

        }

        public static bool FilesAreDifferent(string f1, string f2)
        {
            return CalculateMD5(f1) != CalculateMD5(f2);
        }
        static string CalculateMD5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }
    }

}
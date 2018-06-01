using System;
using Pri.LongPath;

namespace DataAggregator
{
    internal class SpaceRemover
    {
        public static void CleanUpSpace(FileInfo inputFile, string outputFile, bool updateMode)
        {
            if (updateMode && File.Exists(outputFile))
            {
                Console.WriteLine($"{nameof(CleanUpSpace)} Skipping:{inputFile} because already exists {outputFile}");
                return;
            }

            var oldText = File.ReadAllText(inputFile.FullName);
            var newText = System.Text.RegularExpressions.Regex.Replace(oldText, @"\s+", " ");
            File.WriteAllText(outputFile, newText);
        }
    }
}
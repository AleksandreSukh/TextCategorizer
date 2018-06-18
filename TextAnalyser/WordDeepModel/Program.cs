using System.Diagnostics;
using System.IO;
using LanguageModelAdapter;

namespace WordDeepModel
{
    class Program
    {
        static void Main(string[] args)
        {
            //var inputPath = @"D:\GeoTxts";
            //var modelPath = inputPath + "\\Model";
            //if (!Directory.Exists(modelPath))
            //    Directory.CreateDirectory(modelPath);
            //var model = new GeorgianLanguageModelDeep();
            //foreach (var file in Directory.EnumerateFiles(inputPath))
            //{
            //    model.Feed(File.ReadAllText(file), true);
            //}


            var model = new GeorgianLanguageModel();
            var random = model.Chain.GenerateSentence();
            var tempPath = Path.GetTempFileName();
            File.WriteAllText(tempPath, random);
            Process.Start("notepad.exe", tempPath);

        }
    }
}

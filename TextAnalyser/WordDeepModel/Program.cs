using System;
using System.Diagnostics;
using System.IO;
using LanguageModelAdapter;
using TextMarkovChains;

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


            var model = new GeorgianLanguageModelOptimized();
            var random = model.Chain.GenerateSentence();
            var tempPath = Path.GetTempFileName();
            File.WriteAllText(tempPath, random);
            Process.Start("notepad.exe", tempPath);

        }
    }
    public class GeorgianLanguageModelOptimized : ChainAdapter<TextMarkovChainOptimized>
    {
        protected override string XmlFileName { get; } = "geo_model.xml";
        protected override Action<int> FileBeingLoadedLogger => i => { Console.WriteLine("File being loaded:" + i); };
        protected override IMarkovChain InitializeChain() => new TextMarkovChainOptimized();
    }
}

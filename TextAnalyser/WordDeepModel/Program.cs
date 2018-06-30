using System;
using System.Diagnostics;
using System.IO;
using LanguageModelAdapter;
using WordDataAdapter;

namespace WordDeepModel
{
    class Program
    {
        static void Main(string[] args)
        {
            new ChainLoaderToDb().Load(i=>Console.WriteLine("Loading file:"+i+" to the db"));
        }

        private static void GenerateSentence(GeorgianLanguageModelDeepOptimized model)
        {
            var random = model.Chain.GenerateSentence();
            var tempPath = Path.GetTempFileName();
            File.WriteAllText(tempPath, random);
            Process.Start("notepad.exe", tempPath);
        }
    }
}

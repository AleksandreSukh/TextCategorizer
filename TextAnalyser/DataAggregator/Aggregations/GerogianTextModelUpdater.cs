using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using LanguageModelAdapter;
using Pri.LongPath;

namespace DataAggregator
{
    public class GerogianTextModelUpdater
    {
        private static readonly GeorgianLanguageModel Model = new GeorgianLanguageModel();

        public static void FeedFile(FileInfo input, string output, bool updateMode)
        {
            Console.WriteLine("Feeding:" + input);

            Model.Feed(File.ReadAllText(input.FullName), false);
        }

        public static void Save()
        {
            Console.WriteLine("Saving");
            Model.Save();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageModelAdapter;
using Pri.LongPath;

namespace DataAggregator
{
    public class GerogianTextModelUpdater
    {
        private static GeorgianLanguageModel model = new GeorgianLanguageModel();

        public static void FeedFile(FileInfo input, string output, bool updateMode)
        {
            model.Feed(File.ReadAllText(input.FullName));
        }
    }
}

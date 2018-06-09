using System.IO;
using System.Xml;
using TextAnalyser;

namespace LanguageModelAdapter
{
    public class GeorgianLanguageModel
    {
        private const string XmlDirectory = "geo_model";
        readonly TextMarkovChain _chain;

        public GeorgianLanguageModel()
        {
            _chain = new TextMarkovChain();
            if (!Directory.Exists(XmlDirectory))
            {
                _chain.SaveToDir(XmlDirectory);
            }
            else
            {
                Load();
            }
        }

        public void Feed(string text, bool save)
        {
            _chain.Feed(text);
            if (save)
                Save();
        }

        void Load()
        {
            _chain.LoadFromDirectory(XmlDirectory);
        }

        public void Save()
        {
            _chain.SaveToDir(XmlDirectory);
        }
    }
}

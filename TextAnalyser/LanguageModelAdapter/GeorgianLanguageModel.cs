using System.IO;
using System.Xml;
using TextAnalyser;

namespace LanguageModelAdapter
{
    public class GeorgianLanguageModel
    {
        private const string XmlFileName = "geo_model.xml";
        readonly TextMarkovChain _chain;

        public GeorgianLanguageModel()
        {
            if (!File.Exists(XmlFileName))
            {
                _chain = new TextMarkovChain();
                _chain.Save(XmlFileName);
            }
            else
            {
                Load();
            }
        }

        public void Feed(string text)
        {
            _chain.Feed(text);
            Save();
        }

        void Load()
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(XmlFileName);
            _chain.Feed(xmlDocument);
        }

        void Save()
        {
            _chain.Save(XmlFileName);
        }
    }
}

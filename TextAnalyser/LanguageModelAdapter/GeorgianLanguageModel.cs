using System.IO;
using System.Xml;
using TextAnalyser;

namespace LanguageModelAdapter
{
    public class GeorgianLanguageModel
    {
        private const string XmlFileName = "geo_model.xml";
        TextMarkovChain _chain;
        private int chunkCounter = 0;

        public GeorgianLanguageModel()
        {
            _chain = new TextMarkovChain();

            if (!File.Exists(XmlFileName) && !File.Exists(FileNamePattern(0)))
            {
                Save();
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
            var xmlDocument = new XmlDocument();
            if (File.Exists(XmlFileName))
            {
                xmlDocument.Load(XmlFileName);
                _chain.Feed(xmlDocument);
            }
            else
            {
                var loadingChunkCounter = 0;
                while (File.Exists(FileNamePattern(loadingChunkCounter)))
                {
                    xmlDocument.Load(FileNamePattern(loadingChunkCounter));
                    _chain.Feed(xmlDocument);
                    loadingChunkCounter++;
                }
            }
        }

        public void Save()
        {
            if (chunkCounter == 0)
            {
                _chain.Save(XmlFileName);
            }
            else
            {
                if (chunkCounter == 1)
                    File.Move(XmlFileName, FileNamePattern(0));

                _chain.Save(FileNamePattern(chunkCounter));
            }
            _chain = new TextMarkovChain();
            chunkCounter++;
        }
        string FileNamePattern(int number)
        {
            var existingFilePath = Path.Combine(Directory.GetCurrentDirectory(), XmlFileName);
            var fileNameWe = Path.GetFileNameWithoutExtension(existingFilePath);
            var e = Path.GetExtension(existingFilePath);

            var dir = Path.GetDirectoryName(existingFilePath);

            var numberField = $"_{number}_";
            return Path.Combine(dir, $"{fileNameWe}{numberField}{e}");
        }
    }
}

using System;
using System.IO;
using System.Xml;
using TextMarkovChains;

namespace LanguageModelAdapter
{
    public abstract class ChainAdapter<T> where T : IMarkovChain
    {
        protected abstract string XmlFileName { get; }
        protected abstract Action<int> FileBeingLoadedLogger { get; }

        public IMarkovChain Chain => _chain;

        IMarkovChain _chain;
        private int chunkCounter = 0;

        public ChainAdapter()
        {
            _chain = InitializeChain();
            if (File.Exists(XmlFileName) && File.Exists(FileNamePattern(0)))
            {
                File.Delete(XmlFileName);
            }
            if (!File.Exists(XmlFileName) && !File.Exists(FileNamePattern(0)))
            {
                Save();
            }
            else
            {
                Load(FileBeingLoadedLogger);
            }
        }

        protected abstract IMarkovChain InitializeChain();

        public void Feed(string text, bool save)
        {
            Chain.Feed(text);
            if (save)
                Save();
        }

        void Load(Action<int> fileBeingLoadedLogger)
        {
            var xmlDocument = new XmlDocument();
            if (File.Exists(XmlFileName))
            {
                xmlDocument.Load(XmlFileName);
                Chain.Feed(xmlDocument);
            }
            else
            {
                var loadingChunkCounter = 0;
                while (File.Exists(FileNamePattern(loadingChunkCounter)))
                {
                    xmlDocument.Load(FileNamePattern(loadingChunkCounter));
                    Chain.Feed(xmlDocument);
                    fileBeingLoadedLogger?.Invoke(loadingChunkCounter);
                    loadingChunkCounter++;
                }
            }
        }

        public void Save()
        {
            if (chunkCounter == 0)
            {
                Chain.Save(XmlFileName);
            }
            else
            {
                if (chunkCounter == 1)
                    File.Move(XmlFileName, FileNamePattern(0));

                Chain.Save(FileNamePattern(chunkCounter));
            }
            _chain = InitializeChain();
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
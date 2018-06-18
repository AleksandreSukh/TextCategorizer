using System;
using System.IO;
using System.Xml;
using TextMarkovChains;
//using TextMarkovChain = TextAnalyser.TextMarkovChain;
using TextMarkovChain = TextMarkovChains.TextMarkovChain;
namespace LanguageModelAdapter
{
    public class GeorgianLanguageModel : ChainAdapter<TextMarkovChain>
    {
        protected override string XmlFileName { get; } = "geo_model.xml";
        protected override Action<int> FileBeingLoadedLogger { get; }

        protected override IMarkovChain InitializeChain() => new TextMarkovChain();
    }
}

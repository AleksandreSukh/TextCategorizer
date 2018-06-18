using System.IO;
using System.Xml;
using TextMarkovChains;
using TextMarkovChain = TextAnalyser.TextMarkovChain;

namespace LanguageModelAdapter
{
    public class GeorgianLanguageModel : ChainAdapter<TextMarkovChain>
    {
        protected override string XmlFileName { get; } = "geo_model.xml";

        protected override IMarkovChain InitializeChain() => new TextMarkovChain();
    }
}

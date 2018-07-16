using System;
using TextMarkovChains;
//using TextMarkovChain = TextAnalyser.TextMarkovChain;
using TextMarkovChain = TextMarkovChains.TextMarkovChain;
namespace GeorgianLanguageUtils
{
    public class GeorgianLanguageModel : ChainAdapter<TextMarkovChain>
    {
        protected override string XmlFileName { get; } = "geo_model.xml";
        protected override Action<int> FileBeingLoadedLogger => i => { Console.WriteLine("File being loaded:" + i); };

        protected override IMarkovChain InitializeChain() => new TextMarkovChain();
    }
    public class GeorgianLanguageModelOptimized : ChainAdapter<TextMarkovChainOptimized>
    {
        protected override string XmlFileName { get; } = "geo_model.xml";
        protected override Action<int> FileBeingLoadedLogger => i => { Console.WriteLine("File being loaded:" + i); };
        protected override IMarkovChain InitializeChain() => new TextMarkovChainOptimized();
    }
    public class GeorgianLanguageModelDeep : ChainAdapter<MultiDeepMarkovChain>
    {
        protected override string XmlFileName { get; } = "geo_model_deep.xml";
        protected override Action<int> FileBeingLoadedLogger => i => { Console.WriteLine("File being loaded:" + i); };
        protected override IMarkovChain InitializeChain() => new MultiDeepMarkovChain(3);
    }
    public class GeorgianLanguageModelDeepOptimized : ChainAdapter<MultiDeepMarkovChainOptimized>
    {
        protected override string XmlFileName { get; } = "geo_model_deep.xml";
        protected override Action<int> FileBeingLoadedLogger => i => { Console.WriteLine("File being loaded:" + i); };
        protected override IMarkovChain InitializeChain() => new MultiDeepMarkovChain(3);
    }
}

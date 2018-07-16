using System.IO;
using System.Xml;
using GeorgianLanguageCore;
using GeorgianWordsDataBase;
using TextAnalyser;

namespace GeorgianWordDetectorCore
{
    public class WordDetector
    {
        private string GeorgianWordStatisticalModelFileName = "GeorgianWordModel.xml";
        private string EnglistWordStatisticalModelFileName = "EnglishWordModel.xml";
        public WordDetector()
        {
            if (!File.Exists(GeorgianWordStatisticalModelFileName))
            {
                var db = new GeorgianWordsDb();
                var chain = new TextMarkovChain();
                foreach (var word in db.AllWords)
                {
                    chain.Feed(string.Join(" ", word.ToCharArray()) + ".");
                }

                chain.Save(GeorgianWordStatisticalModelFileName);
            }

            if (!File.Exists(EnglistWordStatisticalModelFileName))
            {
                var db = new EnglishWordsDb();
                var chain = new TextMarkovChain();
                foreach (var word in db.AllWords)
                {
                    chain.Feed(string.Join(" ", word.ToCharArray()) + ".");
                }

                chain.Save(EnglistWordStatisticalModelFileName);
            }
        }

        public bool LooksMoraLikeGeorgianWordThanEnglish(string initial)
        {
            var initialToGeo = initial.LatinToGeorgian();

            var chainInitial = new TextMarkovChain();
            chainInitial.Feed(string.Join(" ", initial.ToCharArray()) + ".");

            var chainInitialGeo = new TextMarkovChain();
            chainInitialGeo.Feed(string.Join(" ", initialToGeo.ToCharArray()) + ".");

            var georgianWordModelChain = LoadWordModelChain(GeorgianWordStatisticalModelFileName);
            var englistWordModelChain = LoadWordModelChain(EnglistWordStatisticalModelFileName);
            
            var similarityToGeorgian = new ChainSimilarityEvaluator().EvaluateSimilarity(georgianWordModelChain, chainInitialGeo);
            var similarityToEnglish = new ChainSimilarityEvaluator().EvaluateSimilarity(englistWordModelChain, chainInitial);

            return similarityToGeorgian > similarityToEnglish; 
        }
        

        private TextMarkovChain LoadWordModelChain(string modelFileName)
        {
            var wordModelChain = new TextMarkovChain();
            var xd = new XmlDocument();
            xd.Load(modelFileName);
            wordModelChain.Feed(xd);
            return wordModelChain;
        }
    }
}

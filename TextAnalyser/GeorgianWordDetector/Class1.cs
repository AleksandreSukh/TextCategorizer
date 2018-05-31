using System;
using System.IO;
using System.Linq;
using System.Xml;
using GeorgianWordsDataBase;
using TextAnalyser;

namespace GeorgianWordDetector
{
    public class WordsDetector
    {
        private string GeorgianWordStatisticalModelFileName = "GeorgianWordModel.xml";
        public WordsDetector()
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
        }

        public bool LooksLikeGeorgianWord(string initial)
        {
            var chain = new TextMarkovChain();
            var xd = new XmlDocument();
            xd.Load(GeorgianWordStatisticalModelFileName);
            chain.Feed(xd);
        
            var chainInitial = new TextMarkovChain();
            chainInitial.Feed(string.Join(" ", initial.ToCharArray()) + ".");

            var similarity = new ChainSimilarityEvaluator().EvaluateSimilarity(chain, chainInitial);
            return similarity > 4;
            //todo არაქართული მოდელიც გვჭირდეა
            throw new NotImplementedException();

        }
    }
}

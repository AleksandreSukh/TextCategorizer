using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace TextMarkovChains
{
    public class TextMarkovChain : IMarkovChain
    {
        private readonly Dictionary<string, Chain> _chains;
        private readonly Chain _head;

        public Dictionary<string, Chain> Chains => _chains;

        public Chain Head => _head;

        protected virtual string[] WordRefinerBeforeAddingToChain(string[] refineables)
        {
            return refineables;
        }

        public TextMarkovChain()
        {
            _chains = new Dictionary<string, Chain>();
            _head = new Chain("[]");
            _chains.Add("[]", _head);
        }

       

        public void Feed(string s)
        {
            s = s.ToLower();
            s = s.Replace('/', ' ').Replace(',', ' ').Replace("[]", "");
            s = s.Replace(".", " .").Replace("!", " !").Replace("?", " ?");
            s = s.Replace("\r\n", " ").Replace('\r',' ');
            string[] splitValues = s.Split(' ');
            splitValues = WordRefinerBeforeAddingToChain(splitValues);
            AddWord("[]", splitValues[0]);

            for (int i = 0; i < splitValues.Length - 1; i++)
            {
                if (splitValues[i] == "." ||
                    splitValues[i] == "?" ||
                    splitValues[i] == "!")
                    AddWord("[]", splitValues[i + 1]);
                else
                    AddWord(splitValues[i], splitValues[i + 1]);
            }
        }

        private void AddWord(string prev, string next)
        {
            if (_chains.ContainsKey(prev) && _chains.ContainsKey(next))
                _chains[prev].addWord(_chains[next]);
            else if (_chains.ContainsKey(prev))
            {
                _chains.Add(next, new Chain(next));
                _chains[prev].addWord(_chains[next]);
            }
        }

        public void Feed(XmlDocument xd)
        {
            XmlNode root = xd.ChildNodes[0];
            foreach (XmlNode n in root.ChildNodes)
            {
                //First add all chains that are not there already
                Chain nc = new Chain(n);
                if (!_chains.ContainsKey(nc.word))
                    _chains.Add(nc.word, nc);
            }

            foreach (XmlNode n in root.ChildNodes)
            {
                //Now that all words have been added, we can add the probabilities
                XmlNode nextChains = n.ChildNodes[0];
                Chain current = _chains[n.Attributes["Word"].Value.ToString()];
                foreach (XmlNode nc in nextChains)
                {
                    Chain c = _chains[nc.Attributes["Word"].Value.ToString()];
                    current.addWord(c, Convert.ToInt32(nc.Attributes["Count"].Value));
                }
            }
        }

        public void Save(string path)
        {
            XmlDocument xd = GetDataAsXML();
            xd.Save(path);
        }

    

        public XmlDocument GetDataAsXML()
        {
            XmlDocument xd = new XmlDocument();
            XmlElement root = xd.CreateElement("Chains");
            xd.AppendChild(root);

            foreach (string key in _chains.Keys)
                root.AppendChild(_chains[key].getXMLElement(xd));

            return xd;
        }

        public bool readyToGenerate()
        {
            return _head.GetNextChain() != null;
        }

        public string GenerateSentence()
        {
            StringBuilder s = new StringBuilder();
            Chain nextString = _head.GetNextChain();
            while (nextString.word != "!" && nextString.word != "?" && nextString.word != ".")
            {
                s.Append(nextString.word);
                s.Append(" ");
                nextString = nextString.GetNextChain();
                if (nextString == null)
                    return s.ToString();
            }

            s.Append(nextString.word); //Add punctuation at end

            s[0] = char.ToUpper(s[0]);

            return s.ToString();
        }

        public class Chain
        {
            public string word;

            private Dictionary<string, ChainProbability> chains;
            private int fullCount;

            public Dictionary<string, ChainProbability> GetProbabilities()
            {
                return chains;
            } 
            public Chain(string w)
            {
                word = w;
                chains = new Dictionary<string, ChainProbability>();
                fullCount = 0;
            }

            public Chain(XmlNode node)
            {
                word = node.Attributes["Word"].Value.ToString();
                fullCount = 0;  //Full Count is stored, but this will be loaded when adding new words to the chain.  Default to 0 when loading XML
                chains = new Dictionary<string, ChainProbability>();
            }

            public void addWord(Chain chain, int increase = 1)
            {
                fullCount += increase;
                if (chains.ContainsKey(chain.word))
                    chains[chain.word].count += increase;
                else
                    chains.Add(chain.word, new ChainProbability(chain, increase));
            }

            public Chain GetNextChain()
            {
                //Randomly get the next chain
                //Trey:  As this gets bigger, this is a remarkably inefficient way to randomly get the next chain.
                //The reason it is implemented this way is it allows new sentences to be read in much faster
                //since it will not need to recalculate probabilities and only needs to add a counter.  I don't
                //believe the tradeoff is worth it in this case.  I need to do a timed evaluation of this and decide.
                int currentCount = RandomHandler.random.Next(fullCount);
                foreach (string key in chains.Keys)
                {
                    for (int i = 0; i < chains[key].count; i++)
                    {
                        if (currentCount == 0)
                            return chains[key].chain;
                        currentCount--;
                    }
                }
                return null;
            }

            public XmlElement getXMLElement(XmlDocument xd)
            {
                XmlElement e = xd.CreateElement("Chain");
                e.SetAttribute("Word", this.word);
                e.SetAttribute("FullCount", this.fullCount.ToString());

                XmlElement nextChains = xd.CreateElement("NextChains");
                XmlElement nextChain;

                foreach (string key in chains.Keys)
                {
                    nextChain = xd.CreateElement("Chain");
                    nextChain.SetAttribute("Word", chains[key].chain.word);
                    nextChain.SetAttribute("Count", chains[key].count.ToString());
                    nextChains.AppendChild(nextChain);
                }

                e.AppendChild(nextChains);

                return e;
            }
        }
        /// <summary>
        /// სტრუქტურა რომელიც ინახავს ჯაჭვის ერთი მდგომარეობიდან მეორეში გადასვლის ალბათობას
        /// </summary>
        public class ChainProbability
        {
            public Chain chain;
            public int count;

            public ChainProbability(Chain chain, int count)
            {
                this.chain = chain;
                this.count = count;
            }
        }
    }
}

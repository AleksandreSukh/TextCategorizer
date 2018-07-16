using System;
using System.Collections.Generic;
using System.Xml;

namespace TextAnalyser
{
    public class TextMarkovChain 
    {
        //Private fields
        private readonly Dictionary<string, Chain> _chains;
        private readonly Chain _head;

        //სადაც შევინახავთ სიტყვებს და ამ სიტყვით დაწყებულ მიმდევრობებს
        public Dictionary<string, Chain> Chains => _chains;

        //ჯაჭვის სათავე კვანძი
        public Chain Head => _head;

        //გადატვირთეთ ეს მეთოდი შვილ კლასებში თუ გსურთ რომ გაფილტროთ სიტყვები ჯაჭვში დამატებამდე
        protected virtual string[] WordRefinerBeforeAddingToChain(string[] refineables)
        { return refineables; }

        public TextMarkovChain()
        {
            _chains = new Dictionary<string, Chain>();
            _head = new Chain("[]");
            _chains.Add("[]", _head);
        }
        //Just name
        public string Name { get; set; }


        //მეთოდი რომელიც შეავსებს ჯაჭვს ტექსტიდან
        public void Feed(string s)
        {
            //სასვენი ნიშნების განცალკევება სიტყვებისგან
            //s = s.ToLower();
            s = s.Replace('/', ' ').Replace(',', ' ').Replace("[]", "");
            s = s.Replace(".", " .").Replace("!", " !").Replace("?", " ?");
            s = s.Replace("\r\n", " ").Replace('\r', ' ');
            var splitWordsAndPunctuation = s.Split(' ');
            splitWordsAndPunctuation = WordRefinerBeforeAddingToChain(splitWordsAndPunctuation);//სიტყვების გაფილტვრა

            if (splitWordsAndPunctuation.Length == 0) return;//ტექსტში ყველა სიტყვა გაიფილტრა

            AddWordToTheChainAfterNode("[]", splitWordsAndPunctuation[0]);

            for (var i = 0; i < splitWordsAndPunctuation.Length - 1; i++)
            {

                /*თუ ჯაჭვის წინა სიმბოლო იყო წინადადების 
                 * დამასრულებელი სასვენი ნიშანი მაშინ 
                 * უნდა დავიწყოთ ახალი ჯაჭვი
                 * თუ არა და გავაგრძელოთ არსებული
                 * */
                if (splitWordsAndPunctuation[i] == "." ||
                    splitWordsAndPunctuation[i] == "?" ||
                    splitWordsAndPunctuation[i] == "!")
                    AddWordToTheChainAfterNode("[]", splitWordsAndPunctuation[i + 1]);
                else
                    AddWordToTheChainAfterNode(splitWordsAndPunctuation[i], splitWordsAndPunctuation[i + 1]);
            }
        }
        /// <summary>
        /// ეს მეთოდი პოულობს მოცემულ კვანძს ჯაჭვში და მის გაგრძელებად/ქვეკვანძად ამატებს გადაცემულ სიტყვას
        /// </summary>
        /// <param name="prev">მშობელი კვანძი</param>
        /// <param name="next">ქვეკვანძი</param>
        private void AddWordToTheChainAfterNode(string prev, string next)
        {
            if (_chains.ContainsKey(prev) && _chains.ContainsKey(next))
                _chains[prev].AddWord(_chains[next]);
            else if (_chains.ContainsKey(prev))
            {
                _chains.Add(next, new Chain(next));
                _chains[prev].AddWord(_chains[next]);
            }
        }


        /// <summary>
        /// ჯაჭვის Xml დან შევსებისათვის
        /// </summary>
        /// <param name="xd"></param>
        public void Feed(XmlDocument xd)
        {
            var root = xd.ChildNodes[0];
            foreach (XmlNode n in root.ChildNodes)
            {
                //პირველ რიგში დავამატოთ ისეთი სათავე (root) ჯაჭვები რომლებიც ჯერ არ არის ჯაჭვში
                var nc = new Chain(n);
                if (!_chains.ContainsKey(nc.Word))
                    _chains.Add(nc.Word, nc);
            }

            foreach (XmlNode n in root.ChildNodes)
            {
                //შემდეგ ამ სიტყვებს ვამატებთ გადასვლებს
                var nextChains = n.ChildNodes[0];
                var current = _chains[n.Attributes["Word"].Value];
                foreach (XmlNode nc in nextChains)
                {
                    var c = _chains[nc.Attributes["Word"].Value];
                    current.AddWord(c, Convert.ToInt32(nc.Attributes["Count"].Value));
                }
            }
        }
        /// <summary>
        /// XmlDocument ის შენახვა ფაილში
        /// </summary>
        /// <param name="path"></param>
        public void Save(string path)
        {
            var xd = GetDataAsXml();
            xd.Save(path);
        }

        /// <summary>
        /// ჯაჭვის XmlDocument ის გენერირება
        /// </summary>
        /// <returns></returns>
        public XmlDocument GetDataAsXml()
        {
            var xd = new XmlDocument();
            var root = xd.CreateElement("Chains");
            xd.AppendChild(root);

            foreach (var key in _chains.Keys)
                root.AppendChild(_chains[key].GetXmlElement(xd));

            return xd;
        }



        /// <summary>
        /// ჯაჭვის სტრუქტურა რომელსაც გააჩნია კვანძები და ამ კვანძებს შორის გადასვლების ალბათობები
        /// </summary>
        public class Chain
        {
            public string Word;

            private readonly Dictionary<string, ChainProbability> chains;
            private int _fullCount;

            public Dictionary<string, ChainProbability> GetProbabilities()
            {
                return chains;
            }
            /// <summary>
            /// ჯაჭვის შექმნა საწყისი სიტყვით
            /// </summary>
            /// <param name="w">საწყისი სიტყვა</param>
            public Chain(string w)
            {
                Word = w;
                chains = new Dictionary<string, ChainProbability>();
                _fullCount = 0;
            }
            /// <summary>
            /// ჯაჭვის შექმნა XmlNode დან
            /// </summary>
            /// <param name="node">XmlNode ელემენტი</param>
            public Chain(XmlNode node)
            {
                Word = node.Attributes["Word"].Value;
                _fullCount = 0;  //Full Count is stored, but this will be loaded when adding new words to the chain.  Default to 0 when loading XML
                chains = new Dictionary<string, ChainProbability>();
            }
            /// <summary>
            /// ამატებს ახალ კვანძს მიმდინარეს
            /// </summary>
            /// <param name="chain">ახალი კვანძი</param>
            /// <param name="increase">რამდენით გავზარდოთ გადასვლის ალბათობა. 
            /// (თუ ერთი სიტყვის ბევრჯერ დამატება გვიწევს შეგვიძლია გადავცეთ
            ///  სიტყვა და რაოდენობა რამდენჯერაც გვინდა რომ აისახოს გადასვლებზე)</param>
            public void AddWord(Chain chain, int increase = 1)
            {
                _fullCount += increase;
                if (chains.ContainsKey(chain.Word))
                    chains[chain.Word].Count += increase;
                else
                    chains.Add(chain.Word, new ChainProbability(chain, increase));
            }

            /// <summary>
            /// ჯაჭვის XmlElement ის შექმნა წამოღება 
            /// </summary>
            /// <param name="xd">XmlDocument ფესვი (root) სადაც გვინდა რომ დაემატოს გენერირებული ელემენტი</param>
            /// <returns></returns>
            public XmlElement GetXmlElement(XmlDocument xd)
            {
                var e = xd.CreateElement("Chain");
                e.SetAttribute("Word", this.Word);
                e.SetAttribute("FullCount", this._fullCount.ToString());

                var nextChains = xd.CreateElement("NextChains");
                XmlElement nextChain;

                foreach (var key in chains.Keys)
                {
                    nextChain = xd.CreateElement("Chain");
                    nextChain.SetAttribute("Word", chains[key].Chain.Word);
                    nextChain.SetAttribute("Count", chains[key].Count.ToString());
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
            public Chain Chain;//კვანძი
            public int Count;//ხდომილება

            public ChainProbability(Chain chain, int count)
            {
                this.Chain = chain;
                this.Count = count;
            }
        }
    }
}

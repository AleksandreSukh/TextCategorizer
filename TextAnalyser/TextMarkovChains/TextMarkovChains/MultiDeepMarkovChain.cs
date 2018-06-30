using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace TextMarkovChains
{
    public class MultiDeepMarkovChain : IMarkovChain
    {
        public Dictionary<string, Chain> Chains;
        private Chain _head;
        private int _depth;

        /// <summary>
        /// Creates a new multi-deep Markov Chain with the depth passed in
        /// </summary>
        /// <param name="depth">The depth to store information for words.  Higher values mean more consistency but less flexibility.  Minimum value of three.</param>
        public MultiDeepMarkovChain(int depth)
        {
            if (depth < 3)
                throw new ArgumentException("We currently only support Markov Chains 3 or deeper.  Sorry :(");
            Chains = new Dictionary<string, Chain>();
            _head = new Chain() { Text = "[]" };
            Chains.Add("[]", _head);
            _depth = depth;
        }

        /// <summary>
        /// Feed in text that wil be used to create predictive text.
        /// </summary>
        /// <param name="s">The text that this Markov chain will use to generate new sentences</param>
        public void Feed(string s)
        {
            s = s.ToLower();
            s = s.Replace("/", "").Replace("\\", "").Replace("[]", "").Replace(",", "");
            s = s.Replace("\r\n\r\n", " ").Replace("\r", "").Replace("\n", " "); //The first line is a hack to fix two \r\n (usually a <p> on a website)
            s = s.Replace(".", " .").Replace("!", " ! ").Replace("?", " ?");

            var splitValues = s.Split(' ');
            var sentences = GetSentences(splitValues);
            string[] valuesToAdd;

            foreach (var sentence in sentences)
            {
                for (var start = 0; start < sentence.Length - 1; start++)
                {
                    for (var end = 2; end < _depth + 2 && end + start <= sentence.Length; end++)
                    {
                        valuesToAdd = new string[end];
                        for (var j = start; j < start + end; j++)
                            valuesToAdd[j - start] = sentence[j];
                        AddWord(valuesToAdd);
                    }
                }
            }
        }

        /// <summary>
        /// Feed in a saved XML document of values that will be used to generate sentences.  Please note that the depth in the XML document must match the depth created by the constructor of this Markov Chain.
        /// </summary>
        /// <param name="xd">The XML document used to load this Markov Chain.</param>
        public void Feed(XmlDocument xd)
        {
            var root = xd.ChildNodes[0];
            var rootDepth = Convert.ToInt32(root.Attributes["Depth"].Value);
            if (_depth != rootDepth) //Check to make sure the depths line up
                throw new ArgumentException("The passed in XML document does not have the same depth as this MultiMarkovChain.  The depth of the Markov chain is " + _depth + ", the depth of the XML document is " + rootDepth + ".  The Markov Chain depth can be modified in the constructor");

            //First add each word
            foreach (XmlNode xn in root.ChildNodes)
            {
                var text = xn.Attributes["Text"].Value;
                if (!Chains.ContainsKey(text))
                    Chains.Add(text, new Chain() { Text = text });
            }

            //Now add each next word (Trey:  I do not like this backtracking algorithm.  This could be made better.)
            List<string> nextWords;
            foreach (XmlNode xn in root.ChildNodes)
            {
                var topWord = xn.Attributes["Text"].Value;
                var toProcess = new Queue<XmlNode>();
                foreach (XmlNode n in xn.ChildNodes)
                    toProcess.Enqueue(n);

                while (toProcess.Count != 0)
                {
                    var currentNode = toProcess.Dequeue();
                    var currentCount = Convert.ToInt32(currentNode.Attributes["Count"].Value);
                    nextWords = new List<string>();
                    nextWords.Add(topWord);
                    //nextWords.Add(currentNode.Attributes["Text"].Value.ToString());
                    var parentTrackingNode = currentNode;
                    while (parentTrackingNode.Attributes["Text"].Value != topWord)
                    {
                        nextWords.Insert(1, parentTrackingNode.Attributes["Text"].Value);
                        parentTrackingNode = parentTrackingNode.ParentNode;
                    }
                    AddWord(nextWords.ToArray(), currentCount);

                    foreach (XmlNode n in currentNode.ChildNodes)
                        toProcess.Enqueue(n);
                }
            }
        }

        private List<string[]> GetSentences(string[] words)
        {
            var sentences = new List<string[]>();
            var currentSentence = new List<string>();
            currentSentence.Add("[]"); //start of sentence
            for (var i = 0; i < words.Length; i++)
            {
                currentSentence.Add(words[i]);
                if (words[i] == "!" || words[i] == "." || words[i] == "?")
                {
                    sentences.Add(currentSentence.ToArray());
                    currentSentence = new List<string>();
                    currentSentence.Add("[]");
                }
            }
            return sentences;
        }

        private void AddWord(string[] words, int count = 1)
        {
            //Note:  This only adds the last word in the array. The other words should already be added by this point
            var chainsList = new List<Chain>();
            var lastWord = words[words.Length - 1];
            for (var i = 1; i < words.Length - 1; i++)
                chainsList.Add(Chains[words[i]]);
            if (!Chains.ContainsKey(lastWord))
                Chains.Add(lastWord, new Chain() { Text = lastWord });
            chainsList.Add(Chains[lastWord]);
            var firstChainInList = Chains[words[0]];
            firstChainInList.AddWords(chainsList.ToArray(), count);
        }

        /// <summary>
        /// Determines if this Markov Chain is ready to begin generating sentences
        /// </summary>
        /// <returns></returns>
        public bool ReadyToGenerate()
        {
            return (_head.GetNextWord() != null);
        }

        /// <summary>
        /// Generate a sentence based on the data passed into this Markov Chain.
        /// </summary>
        /// <returns></returns>
        public string GenerateSentence()
        {
            var sb = new StringBuilder();
            var currentChains = new string[_depth];
            currentChains[0] = _head.GetNextWord().Text;
            sb.Append(currentChains[0]);
            string[] temp;
            var doneProcessing = false;
            for (var i = 1; i < _depth; i++)
            {
                //Generate the first row
                temp = new string[i];
                for (var j = 0; j < i; j++)
                    temp[j] = currentChains[j];

                var nextWord = _head.GetNextWord(temp)?.Text;
                if (nextWord == null)
                {
                    doneProcessing = true;
                    break;
                }

                currentChains[i] = nextWord;
                if ( currentChains[i] == "."
                    || currentChains[i] == "?"
                    || currentChains[i] == "!")
                {
                    doneProcessing = true;
                    sb.Append(currentChains[i]);
                    break;
                }
                sb.Append(" ");
                sb.Append(currentChains[i]);
            }

            var breakCounter = 0;
            while (!doneProcessing)
            {
                for (var j = 1; j < _depth; j++)
                    currentChains[j - 1] = currentChains[j];
                var newHead = Chains[currentChains[0]];
                temp = new string[_depth - 2];
                for (var j = 1; j < _depth - 1; j++)
                    temp[j - 1] = currentChains[j];

                currentChains[_depth - 1] = newHead.GetNextWord(temp).Text;
                if (currentChains[_depth - 1] == "." ||
                    currentChains[_depth - 1] == "?" ||
                    currentChains[_depth - 1] == "!")
                {
                    sb.Append(currentChains[_depth - 1]);
                    break;
                }
                sb.Append(" ");
                sb.Append(currentChains[_depth - 1]);

                breakCounter++;
                if (breakCounter >= 50) //This is still relatively untested software.  Better safe than sorry :)
                    break;
            }


            sb[0] = char.ToUpper(sb[0]);
            return sb.ToString();
        }

        public List<string> GetNextLikelyWord(string previousText)
        {
            //TODO:  Do a code review of this function, it was written pretty hastily
            //TODO:  Include results that use a chain of less length that the depth.  This will allow for more results when the depth is large
            var results = new List<string>();
            previousText = previousText.ToLower();
            previousText = previousText.Replace("/", "").Replace("\\", "").Replace("[]", "").Replace(",", "");
            previousText = previousText.Replace("\r\n\r\n", " ").Replace("\r", "").Replace("\n", " "); //The first line is a hack to fix two \r\n (usually a <p> on a website)

            if (previousText == string.Empty)
            {
                //Assume start of sentence

                var nextChains = _head.GetPossibleNextWords(new string[0]);
                nextChains.Sort((x, y) =>
                {
                    return x.Count - y.Count;
                });
                foreach (var cp in nextChains)
                    results.Add(cp.Chain.Text);
            }
            else
            {
                var initialSplit = previousText.Split(' ');

                string[] previousWords;
                if (initialSplit.Length > _depth)
                {
                    previousWords = new string[_depth];
                    for (var i = 0; i < _depth; i++)
                        previousWords[i] = initialSplit[initialSplit.Length - _depth + i];
                }
                else
                {
                    previousWords = new string[initialSplit.Length];
                    for (var i = 0; i < initialSplit.Length; i++)
                        previousWords[i] = initialSplit[i];
                }

                if (!Chains.ContainsKey(previousWords[0]))
                    return new List<string>();

                try
                {
                    var headerChain = Chains[previousWords[0]];
                    var sadPreviousWords = new string[previousWords.Length - 1]; //They are sad because I'm allocating extra memory for a slightly different array and there's probably a better way but I'm lazy :(
                    for (var i = 1; i < previousWords.Length; i++)
                        sadPreviousWords[i - 1] = previousWords[i];
                    var nextChains = headerChain.GetPossibleNextWords(sadPreviousWords);
                    nextChains.Sort((x, y) =>
                        {
                            return x.Count - y.Count;
                        });
                    foreach (var cp in nextChains)
                        results.Add(cp.Chain.Text);
                }
                catch (Exception excp)
                {
                    return new List<string>();
                }
            }
            return results;
        }

        /// <summary>
        /// Save the data contained in this Markov Chain to an XML document.
        /// </summary>
        /// <param name="path">The file path to Save to.</param>
        public void Save(string path)
        {
            var xd = GetXmlDocument();
            xd.Save(path);
        }

        /// <summary>
        /// Get the data for this Markov Chain as an XmlDocument object.
        /// </summary>
        /// <returns></returns>
        public XmlDocument GetXmlDocument()
        {
            var xd = new XmlDocument();
            var root = xd.CreateElement("Chains");
            root.SetAttribute("Depth", _depth.ToString());
            xd.AppendChild(root);

            foreach (var key in Chains.Keys)
                root.AppendChild(Chains[key].GetXml(xd));

            return xd;
        }

        public class Chain
        {
            public string Text;
            internal int FullCount;
            public Dictionary<string, ChainProbability> NextNodes;

            internal Chain()
            {
                NextNodes = new Dictionary<string, ChainProbability>();
                FullCount = 0;
            }

            internal void AddWords(Chain[] c, int count = 1)
            {
                if (c.Length == 0)
                    throw new ArgumentException("The array of chains passed in is of zero length.");
                if (c.Length == 1)
                {
                    FullCount += count;
                    if (!NextNodes.ContainsKey(c[0].Text))
                        NextNodes.Add(c[0].Text, new ChainProbability(c[0], count));
                    else
                        NextNodes[c[0].Text].Count += count;
                    return;
                }

                var nextChain = NextNodes[c[0].Text];
                for (var i = 1; i < c.Length - 1; i++)
                    nextChain = nextChain.GetNextNode(c[i].Text);
                nextChain.AddWord(c[c.Length - 1], count);
            }

            internal Chain GetNextWord()
            {
                var currentCount = RandomHandler.random.Next(FullCount) + 1;
                foreach (var key in NextNodes.Keys)
                {
                    currentCount -= NextNodes[key].Count;
                    if (currentCount <= 0)
                        return NextNodes[key].Chain;
                }
                return null;
            }

            internal Chain GetNextWord(string[] words)
            {
                var currentChain = NextNodes[words[0]];
                for (var i = 1; i < words.Length; i++)
                    currentChain = currentChain.GetNextNode(words[i]);

                var currentCount = RandomHandler.random.Next(currentChain.Count) + 1;
                foreach (var key in currentChain.NextNodes.Keys)
                {
                    currentCount -= currentChain.NextNodes[key].Count;
                    if (currentCount <= 0)
                        return currentChain.NextNodes[key].Chain;
                }
                return null;
            }

            internal List<ChainProbability> GetPossibleNextWords(string[] words)
            {
                var results = new List<ChainProbability>();

                if (words.Length == 0)
                {
                    foreach (var key in NextNodes.Keys)
                        results.Add(NextNodes[key]);
                    return results;
                }

                var currentChain = NextNodes[words[0]];
                for (var i = 1; i < words.Length; i++)
                    currentChain = currentChain.GetNextNode(words[i]);

                foreach (var key in currentChain.NextNodes.Keys)
                    results.Add(currentChain.NextNodes[key]);

                return results;
            }

            internal XmlElement GetXml(XmlDocument xd)
            {
                var e = xd.CreateElement("Chain");
                e.SetAttribute("Text", Text);

                foreach (var key in NextNodes.Keys)
                    e.AppendChild(NextNodes[key].GetXml(xd));

                return e;
            }
        }

        public class ChainProbability
        {
            internal Chain Chain;
            public int Count;
            internal Dictionary<string, ChainProbability> NextNodes;

            internal ChainProbability(Chain c, int co)
            {
                Chain = c;
                Count = co;
                NextNodes = new Dictionary<string, ChainProbability>();
            }

            internal void AddWord(Chain c, int count = 1)
            {
                var word = c.Text;
                if (NextNodes.ContainsKey(word))
                    NextNodes[word].Count += count;
                else
                    NextNodes.Add(word, new ChainProbability(c, count));
            }

            internal ChainProbability GetNextNode(string prev)
            {
                return NextNodes[prev];
            }

            internal XmlElement GetXml(XmlDocument xd)
            {
                var e = xd.CreateElement("Chain");
                e.SetAttribute("Text", Chain.Text);
                e.SetAttribute("Count", Count.ToString());

                XmlElement c;
                foreach (var key in NextNodes.Keys)
                {
                    c = NextNodes[key].GetXml(xd);
                    e.AppendChild(c);
                }

                return e;
            }
        }
    }
}

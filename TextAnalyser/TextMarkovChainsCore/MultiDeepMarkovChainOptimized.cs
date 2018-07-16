using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace TextMarkovChains
{
    public class MultiDeepMarkovChainOptimized :IMarkovChain
    {
        public Dictionary<Word, Chain> Chains;
        private Chain _head;
        private int _depth;

        /// <summary>
        /// Creates a new multi-deep Markov Chain with the depth passed in
        /// </summary>
        /// <param name="depth">The depth to store information for words.  Higher values mean more consistency but less flexibility.  Minimum value of three.</param>
        public MultiDeepMarkovChainOptimized(int depth)
        {
            if (depth < 3)
                throw new ArgumentException("We currently only support Markov Chains 3 or deeper.  Sorry :(");
            Chains = new Dictionary<Word, Chain>();
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

            string[] splitValues = s.Split(' ');
            List<string[]> sentences = GetSentences(splitValues);
            string[] valuesToAdd;

            foreach (string[] sentence in sentences)
            {
                for (int start = 0; start < sentence.Length - 1; start++)
                {
                    for (int end = 2; end < _depth + 2 && end + start <= sentence.Length; end++)
                    {
                        valuesToAdd = new string[end];
                        for (int j = start; j < start + end; j++)
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
            XmlNode root = xd.ChildNodes[0];
            int rootDepth = Convert.ToInt32(root.Attributes["Depth"].Value);
            if (_depth != rootDepth) //Check to make sure the depths line up
                throw new ArgumentException("The passed in XML document does not have the same depth as this MultiMarkovChain.  The depth of the Markov chain is " + _depth + ", the depth of the XML document is " + rootDepth + ".  The Markov Chain depth can be modified in the constructor");

            //First add each word
            foreach (XmlNode xn in root.ChildNodes)
            {
                string text = xn.Attributes["Text"].Value;
                if (!Chains.ContainsKey(text))
                    Chains.Add(text, new Chain() { Text = text });
            }

            //Now add each next word (Trey:  I do not like this backtracking algorithm.  This could be made better.)
            List<string> nextWords;
            foreach (XmlNode xn in root.ChildNodes)
            {
                string topWord = xn.Attributes["Text"].Value;
                Queue<XmlNode> toProcess = new Queue<XmlNode>();
                foreach (XmlNode n in xn.ChildNodes)
                    toProcess.Enqueue(n);

                while (toProcess.Count != 0)
                {
                    XmlNode currentNode = toProcess.Dequeue();
                    int currentCount = Convert.ToInt32(currentNode.Attributes["Count"].Value);
                    nextWords = new List<string>();
                    nextWords.Add(topWord);
                    //nextWords.Add(currentNode.Attributes["Text"].Value.ToString());
                    XmlNode parentTrackingNode = currentNode;
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

        public void Save(string path)
        {
            throw new NotImplementedException();
        }

        public string GenerateSentence()
        {
            throw new NotImplementedException();
        }

        private List<string[]> GetSentences(string[] words)
        {
            List<string[]> sentences = new List<string[]>();
            List<string> currentSentence = new List<string>();
            currentSentence.Add("[]"); //start of sentence
            for (int i = 0; i < words.Length; i++)
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
            List<Chain> chainsList = new List<Chain>();
            string lastWord = words[words.Length - 1];
            for (int i = 1; i < words.Length - 1; i++)
                chainsList.Add(Chains[words[i]]);
            if (!Chains.ContainsKey(lastWord))
                Chains.Add(lastWord, new Chain() { Text = lastWord });
            chainsList.Add(Chains[lastWord]);
            Chain firstChainInList = Chains[words[0]];
            firstChainInList.AddWords(chainsList.ToArray(), count);
        }

    }
    public class Chain
    {
        public Word Text;
        internal int FullCount;
        public Dictionary<Word, ChainProbability> NextNodes;

        internal Chain()
        {
            NextNodes = new Dictionary<Word, ChainProbability>();
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

            ChainProbability nextChain = NextNodes[c[0].Text];
            for (int i = 1; i < c.Length - 1; i++)
                nextChain = nextChain.GetNextNode(c[i].Text);
            nextChain.AddWord(c[c.Length - 1], count);
        }

        internal Chain GetNextWord()
        {
            int currentCount = RandomHandler.random.Next(FullCount) + 1;
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
            ChainProbability currentChain = NextNodes[words[0]];
            for (int i = 1; i < words.Length; i++)
                currentChain = currentChain.GetNextNode(words[i]);

            int currentCount = RandomHandler.random.Next(currentChain.Count) + 1;
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

            ChainProbability currentChain = NextNodes[words[0]];
            for (int i = 1; i < words.Length; i++)
                currentChain = currentChain.GetNextNode(words[i]);

            foreach (var key in currentChain.NextNodes.Keys)
                results.Add(currentChain.NextNodes[key]);

            return results;
        }
    }

    public class ChainProbability
    {
        internal Chain Chain;
        public int Count;
        internal Dictionary<Word, ChainProbability> NextNodes;

        internal ChainProbability(Chain c, int co)
        {
            Chain = c;
            Count = co;
            NextNodes = new Dictionary<Word, ChainProbability>();
        }

        internal void AddWord(Chain c, int count = 1)
        {
            var word = c.Text;
            if (NextNodes.ContainsKey(word))
                NextNodes[word].Count += count;
            else
                NextNodes.Add(word, new ChainProbability(c, count));
        }

        internal ChainProbability GetNextNode(Word prev)
        {
            return NextNodes[prev];
        }
    }

}
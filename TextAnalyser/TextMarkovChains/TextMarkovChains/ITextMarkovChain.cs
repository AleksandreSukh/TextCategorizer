using System.Xml;

namespace TextMarkovChains
{
    public interface ITextMarkovChain
    {
        void Feed(XmlDocument xd);
        void Feed(string s);
        string generateSentence();
        XmlDocument GetDataAsXML();
        bool readyToGenerate();
        void Save(string path);
    }
}
using System.Xml;

namespace TextMarkovChains
{
    public interface IMarkovChain
    {
        void Feed(string s);
        void Feed(XmlDocument xd);
        void Save(string path);
        string GenerateSentence();


    }
}
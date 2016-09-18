using stemmer2;

namespace StemmerWrap
{
    public class StemmerWrapper
    {
        public static string[] Stem(string[] args)
        {
            return Stemmer2.LemmatizeOuter(args);
        }
    }
}

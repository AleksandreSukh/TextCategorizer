using System.Collections.Generic;
using GeorgianWordsDataBase;

namespace GeorgianLanguageApi
{
    public interface IWordDataRepository
    {
        List<string> GetAllWords();
    }

    public class WordDataRepository : IWordDataRepository
    {
        static WordDb wordDb = new GeorgianWordsDb();
        public List<string> GetAllWords()
        {
            return wordDb.AllWords;
        }
    }
}
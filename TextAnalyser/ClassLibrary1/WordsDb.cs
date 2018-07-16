using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace GeorgianWordsDataBase
{
    public abstract class WordDb
    {
        public readonly List<string> AllWords;

        public WordDb()
        {
            var resources = EmbeddedResourceReader.ReadAllResources(r => r.Contains(GetSelector()))
                .Select(t =>
                    JsonSerializer.Create().Deserialize<List<string>>(new JsonTextReader(new StringReader(t.Value))))
                .SelectMany(w => w);
            AllWords = resources.ToList();
        }

        protected abstract string GetSelector();
    }
    public class EnglishWordsDb : WordDb
    {
        protected override string GetSelector() => "english_words";
    }
    public class GeorgianWordsDb : WordDb
    {
        protected override string GetSelector() => "wordsChunk";
    }
}
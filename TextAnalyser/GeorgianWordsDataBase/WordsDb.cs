using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace GeorgianWordsDataBase
{
    public class GeorgianWordsDb
    {
        public  readonly List<string> AllWords;
        public GeorgianWordsDb()
        {
            var resources = EmbeddedResourceReader.ReadAllResources(r => r.Contains("wordsChunk"))
                .Select(t =>
                    JsonSerializer.Create().Deserialize<List<string>>(new JsonTextReader(new StringReader(t.Value))))
                .SelectMany(w => w);
            AllWords = resources.ToList();
        }
    }
}
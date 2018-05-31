using System;
using System.Linq;

namespace GeorgianLanguageClasses
{
    /// <summary>
    /// http://www.ice.ge/web/6/____qrammatika.html
    /// სიტყვის დამარცვლისას უნდა დავიმახსოვროთ ერთი წესი: თუ სიტყვაში ხმოვანს ორი ან მეტი თანხმოვანი მოსდევს, პირველი წინა მარცვალთან რჩება, ხოლო დანარჩენი _ მომდევნო მარცვალში გადადის.
    /// </summary>
    public static class SyllableSplitter
    {
        public static string[] Syllables(this string word, bool checkedGeorgian = false)
        {
            if (!checkedGeorgian && !word.IsGeorgianWord()) throw new ArgumentException($"Word:{word} is not a valid georgian word");
            return word.Split(' ', '-').SelectMany(SyllablesFromFiltered).ToArray();
        }

        private static string[] SyllablesFromFiltered(string word)
        {
            var length = word.Vowels().Length;
            var array = new string[length];

            for (var i = 0; i < array.Length; i++)
            {
                for (var j = 0; j < word.Length; j++)
                {
                    if (!word[j].IsVowel()) continue;
                    var chunkLength = j + 1;

                    var afterChunk = word.Remove(0, chunkLength);
                    if (afterChunk.All(c => c.IsConsonant()))
                        chunkLength = word.Length;
                    else if (afterChunk.Length > 2
                             && afterChunk[0].IsConsonant()
                             && afterChunk[1].IsConsonant()
                        )
                        chunkLength += 1;

                    var chunk = word.Substring(0, chunkLength);
                    word = word.Remove(0, chunkLength);
                    array[i] = chunk;
                    break;
                }
            }
            return array;
        }
    }
}

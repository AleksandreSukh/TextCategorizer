using System;
using System.Linq;
using GeorgianLanguageClasses;
using GeorgianWordDetector;
using Pri.LongPath;

namespace DataAggregator
{
    public static class LatinGeoFixer
    {
        static WordDetector _wordDetector = new WordDetector();
        public static void FixLatinCharactersOrJustCopy(FileInfo inputFile, string outputFile, bool updateMode)
        {
            if (updateMode && File.Exists(outputFile))
            {
                Console.WriteLine($"{nameof(FixLatinCharactersOrJustCopy)} Skipping:{inputFile} because already exists {outputFile}");
                return;
            }

            var textFromFile = File.ReadAllText(inputFile.FullName);

            var textIsGeorgianWithLatinCharacters = TextIsGeorgianWithLatinCharacters(textFromFile);

            if (textIsGeorgianWithLatinCharacters)
            {
                File.WriteAllText(outputFile, textFromFile.LatinToGeorgian());
            }
            else
            {
                File.Copy(inputFile.FullName, outputFile);
                Console.WriteLine($"{nameof(FixLatinCharactersOrJustCopy)} Skipping:{inputFile} noting to be done");
            }
        }

        public static bool LooksLikeGeorgian(this string s)
        {
            return _wordDetector.LooksMoraLikeGeorgianWordThanEnglish(s);
        }

        public static bool TextIsGeorgianWithLatinCharacters(string text)
        {
            return CheckTextByRandom10Words(text, (string w) => !w.IsGeorgianWord() && w.LooksLikeGeorgian());
        }

        public static bool TextIsGeorgian(string text)
        {
            return CheckTextByRandom10Words(text, (string w) => w.IsGeorgianWord());
        }

        static bool CheckTextByRandom10Words(string text, Func<string, bool> checker)
        {
            var wordsFromIt = text.Split(' ');

            var randomWordsToCheck = 10;
            var rnd = new Random();
            var random10Words = wordsFromIt.Where(w => w.Length > 5).OrderBy(x => rnd.Next()).Take(randomWordsToCheck);
            var wordsWithCriteria =
                random10Words.Where(checker);
            return wordsWithCriteria.Count() > randomWordsToCheck / 2;
        }
    }
}
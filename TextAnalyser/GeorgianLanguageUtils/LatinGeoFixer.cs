using GeorgianLanguageCore;
using System;
using System.IO;
using System.Linq;
using GeorgianWordDetectorCore;

namespace GeorgianLanguageUtils
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

        public static void FilterNonGeorgianTexts(FileInfo inputFile, string outputFile, bool updateMode)
        {
            if (updateMode && File.Exists(outputFile))
            {
                Console.WriteLine($"{nameof(FilterNonGeorgianTexts)} Skipping:{inputFile} because already exists {outputFile}");
                return;
            }

            var textFromFile = File.ReadAllText(inputFile.FullName);

            var isGeorgian = TextIsGeorgian(textFromFile);

            if (isGeorgian)
            {
                File.WriteAllText(outputFile, textFromFile);
            }
            else
            {
                //TODO:Temp

                try
                {
                    File.Copy(inputFile.FullName, Path.Combine(@"D:\Aleks\DataRepository\nonGeo", Path.GetFileName(outputFile)));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                //!

                Console.WriteLine($"{nameof(FilterNonGeorgianTexts)} Skipping:{inputFile} doesn't contain georgian text");
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

            var randomWordsToCheck = Math.Min(wordsFromIt.Length, 10);
            var rnd = new Random();
            var random10Words = wordsFromIt.Where(w => w.Length > 2)

                .OrderBy(x => rnd.Next())
                .Take(randomWordsToCheck)
                //Clean punctuation
                .Select(st => new string(st.ToCharArray().Where(c => !char.IsPunctuation(c)).ToArray()));

            var wordsWithCriteria =
                random10Words.Where(checker);
            return wordsWithCriteria.Count() > randomWordsToCheck / 2;
        }

        public static string FixLatinCharacters(string input)
        {
            //TODO:we need to add sms style text converter also
            return input.LatinToGeorgian();
        }
    }

}

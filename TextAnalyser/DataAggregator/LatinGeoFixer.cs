﻿using System;
using System.Linq;
using GeorgianLanguageClasses;
using GeorgianWordDetector;
using Pri.LongPath;

namespace DataAggregator
{
    public static class LatinGeoFixer
    {
        static WordsDetector wordsDetector = new WordsDetector();
        public static void FixLatinCharactersOrSkip(FileInfo inputFile, string outputFile, bool updateMode)
        {
            var textFromFile = File.ReadAllText(inputFile.FullName);
            var wordsFromIt = textFromFile.Split(' ');
            var rnd = new Random();
            var random10Words = wordsFromIt.OrderBy(x => rnd.Next()).Take(10);
            var wordsThatCanBecomeGeorgian =
                random10Words.Where(w => !w.IsGeorgianWord() && w.LatinToGeorgian().LooksLikeGeorgian());
            if (wordsThatCanBecomeGeorgian.Count() > 5)
            {

            }
        }

        public static bool LooksLikeGeorgian(this string s)
        {
            return wordsDetector.LooksLikeGeorgianWord(s);
        }
    }
}
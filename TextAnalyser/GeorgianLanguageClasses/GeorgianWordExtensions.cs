using System;
using System.Linq;

namespace GeorgianLanguageClasses
{
    public static class GeorgianWordExtensions
    {
        public static bool IsVowel(this char character)
        {
            return GeorgianAlphabet.Vowels.Contains(character);
        }
        public static bool IsConsonant(this char character)
        {
            return GeorgianAlphabet.Consonants.Contains(character);
        }

        public static string Consonants(this string s)
        {
            return new string(s.Where(c => c.IsConsonant()).ToArray());
        }

        public static string Vowels(this string s)
        {
            return new string(s.Where(c => c.IsVowel()).ToArray());
        }

        public static string Reverse(this string s)
        {
            var charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        const string enabledSymbol = "-";
        static readonly string availableCharsInGeorgianWord = GeorgianAlphabet.Alpabet + enabledSymbol;
        public static bool IsGeorgianWord(this string word)
        {
            return word.Length > 1 && word.Length < 30
                && word.All(c => availableCharsInGeorgianWord.Contains(c))
                && !word.EndsWith(enabledSymbol, StringComparison.InvariantCulture)
                && !word.StartsWith(enabledSymbol, StringComparison.InvariantCulture)
                && word.Any(c=>c.IsVowel())
                && !IncludesThreeSameConsonantsFollowing(word)
                && !IncludesFourSameVowelsFollowing(word);
        }

        static bool IncludesThreeSameConsonantsFollowing(string word)
        {
            if (word.Length <= 3) return false;
            for (var i = 2; i < word.Length; i++)
            {
                var current = word.ElementAt(i);
                if (current.IsConsonant() && current == word.ElementAt(i - 1) && current == word.ElementAt(i - 2))
                    return true;
            }
            return false;
        }
        static bool IncludesFourSameVowelsFollowing(string word)
        {
            if (word.Length < 4) return false;
            for (var i = 3; i < word.Length; i++)
            {
                var current = word.ElementAt(i);
                if (current.IsVowel()
                    && current == word.ElementAt(i - 1)
                    && current == word.ElementAt(i - 2)
                    && current == word.ElementAt(i - 3))
                    return true;
            }
            return false;
        }
    }
}

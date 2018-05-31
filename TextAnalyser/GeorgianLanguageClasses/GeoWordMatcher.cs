using System;
using System.Linq;

namespace GeorgianLanguageClasses
{
    public class WordMatcherSettings
    {
        public double SyllableSimilarityEvaluatorVowelSimilarityPortion { get; set; } = 1 / 2.0;
        public double SyllableSimilarityEvaluatorConsonantsBeforeVowelPortion { get; set; } = 1 / 4.0;
        public double SyllableSimilarityEvaluatorConsonantsAfterVowelPortion { get; set; } = 1 / 4.0;
        public Func<double, double> SyllableLocationAwarePortionEvaluator { get; set; } = d => Math.Pow(2, d);
    }
    /// <summary>
    /// Class which provides word comparer as rhymes 
    /// </summary>
    public static class GeoWordMatcher
    {
        static WordMatcherSettings settings = new WordMatcherSettings();
        /// <summary>
        /// This method compares two words or phrases to each other to determine how much they would sound similar (most importantly at the end)
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="str2"></param>
        /// <returns></returns>
        public static double EvaluateRhymeSimilarity(string str1, string str2, bool checkedGeorgian = false)
        {
            if (!checkedGeorgian)
            {
                if (!str1.Split(' ').All(w => w.IsGeorgianWord())) throw new ArgumentException($"Word:{str1} is not a valid georgian word");
                if (!str2.Split(' ').All(w => w.IsGeorgianWord())) throw new ArgumentException($"Word:{str2} is not a valid georgian word");
            }
            //Sort strings
            var arr = new string[2] { str1, str2 }.OrderBy(s => s).ToArray();

            str1 = arr[0];
            str2 = arr[1];

            var lengthy = str1.Length > str2.Length ? str1 : str2;
            return SyllableComparisonResult(str1, str2) / SyllableComparisonResult(lengthy, lengthy);
        }

        private static double SyllableComparisonResult(string str1, string str2)
        {

            var syll1 = str1.Syllables(true);
            var syll2 = str2.Syllables(true);
            var minSyllableLength = Math.Min(syll1.Length, syll2.Length);
            double nominalCoeficient = 16.0;

            double[] coeficients = new double[minSyllableLength];

            for (int i = 0; i < minSyllableLength; i++)
            {
                coeficients[i] = nominalCoeficient / settings.SyllableLocationAwarePortionEvaluator(i);
            }

            double comparisonResult = 0;
            for (int i = 0; i < minSyllableLength; i++)
            {
                var syllable1 = syll1[syll1.Length - (i + 1)];
                var syllable2 = syll2[syll2.Length - (i + 1)];
                comparisonResult += SyllableComparer(syllable1, syllable2, 10) * coeficients[i];
            }
            return comparisonResult;
        }

        public static double SyllableComparer(string syll1, string syll2, double scale)
        {
            var result = 0.0;

            var vowel1 = syll1.Single(a => a.IsVowel());
            var vowel2 = syll2.Single(a => a.IsVowel());

            //Priority of vovel similarity here is 1/2 
            if (vowel1 == vowel2) result += scale * settings.SyllableSimilarityEvaluatorVowelSimilarityPortion;

            var vowel1Index = syll1.IndexOf(vowel1);
            var vowel2Index = syll2.IndexOf(vowel2);

            var consBefore1 = syll1.Substring(0, vowel1Index);
            var consBefore2 = syll2.Substring(0, vowel2Index);

            var consAfter1 = syll1.Substring(vowel1Index + 1);
            var consAfter2 = syll2.Substring(vowel2Index + 1);

            var left = CompareConsByScale5(consBefore1, consBefore2) / 5;
            var right = CompareConsByScale5(consAfter1, consAfter2) / 5;

            //Priority of consonant similarities before and after the vowel is 1/4
            result += left * (scale * settings.SyllableSimilarityEvaluatorConsonantsBeforeVowelPortion);
            result += right * (scale * settings.SyllableSimilarityEvaluatorConsonantsAfterVowelPortion);
            return result;
        }

        public static double CompareConsByScale5(string a, string b)
        {
            //if both empty return as if they were similar
            if (string.IsNullOrEmpty(a) && string.IsNullOrEmpty(b))
            {
                var fakeCons = GeorgianAlphabet.Consonants.First();
                return ConsonantPhoneticSimilarity.GetSimilarity(fakeCons, fakeCons);
            }

            var result = 0.0;
            var minLength = Math.Min(a.Length, b.Length);
            //If one of them empty they are not similar at all
            if (minLength == 0) return 0;

            for (int i = 0; i < minLength; i++)
                result += ConsonantPhoneticSimilarity.GetSimilarity(a[i], b[i]);
            //TODO:Review the above code because of minlength "დღნაშ, დღნაშვს" result is 5

            //If strings are entirely similar then result would be scaled as max result - scale
            //NOTE! we do not consider string length here because we need to ensure max similarity will always be - scale
            return result / minLength;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace GeorgianLanguageClasses
{
    //TODO:Refactor
    public class GeorgianTextParser
    {
        static string[] splittersForWords = { "\t", "\r\n", ">", "<", ".", ",", "?", "!", " " };
        static char[] trimmers = { '>', '<', ' ', '/', ',', '.', ':', '!', '?', '=', '\\', '"', '-', '_', ';' };
        public static List<string> ParseWords(string html)
        {
            var wordsFromPage = new List<string>();
            if (html != null)
            {
                var trimmableWordsFromPage = html.Split(splittersForWords, StringSplitOptions.None).ToList();
                var tmp = wordsFromPage.Count();
                foreach (var item in trimmableWordsFromPage)
                {
                    var temp = item.Trim(trimmers);
                    if (temp.Length > 1 & temp.IsGeorgianWord())
                    {
                        wordsFromPage.Add(temp);
                    }
                }
            }
            return wordsFromPage;
        }
    }
}

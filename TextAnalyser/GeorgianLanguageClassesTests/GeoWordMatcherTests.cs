using System.Collections.Generic;
using System.Linq;
using GeorgianLanguageClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace GeorgianLanguageClassesTests
{

    [TestClass()]
    public class GeoWordMatcherTests
    {
        [TestMethod()]
        public void CompareConsByScale5Test()
        {
            GeoWordMatcher.CompareConsByScale5(null, null).ShouldBe(5);
            GeoWordMatcher.CompareConsByScale5("", "ცდ").ShouldBe(0);
            GeoWordMatcher.CompareConsByScale5("ცდ", "ცდ").ShouldBe(5);
            GeoWordMatcher.CompareConsByScale5("ც", "ც").ShouldBe(5);
            GeoWordMatcher.CompareConsByScale5("დც", "ცდ").ShouldBe(0);
            GeoWordMatcher.CompareConsByScale5("ცვ", "ცდ").ShouldBe(2.5);
        }

        [TestMethod()]
        public void SyllableComparerTest()
        {
            GeoWordMatcher.SyllableComparer("ცუდ", "ცუდ", 20).ShouldBe(20);
            GeoWordMatcher.SyllableComparer("ცუ", "ცუდ", 20).ShouldBe(15);
            GeoWordMatcher.SyllableComparer("დღნა", "დღნა", 20).ShouldBe(20);
            GeoWordMatcher.SyllableComparer("დღნაშ", "დღნაშვს", 20).ShouldBeGreaterThan(15);
            //GeoWordMatcher.SyllableComparer("დღნაშ", "დღნაშვს", 20).ShouldBeLessThan(20);
        }

        [TestMethod()]
        public void HowMatchStringsTest()
        {
            GeoWordMatcher.EvaluateRhymeSimilarity("ფინჯანი", "ფინჯანი").ShouldBe(1);
            GeoWordMatcher.EvaluateRhymeSimilarity("კატა კლიზმები", "კატა კლიზმები").ShouldBe(1);

        }
        [TestMethod()]
        public void bidirectionally()
        {
            var dir1 =
                GeoWordMatcher.EvaluateRhymeSimilarity("ფინჯანი ყავა", "ფინჯანი");
            var dir2 =
                GeoWordMatcher.EvaluateRhymeSimilarity("ფინჯანი", "ფინჯანი ყავა");
            dir1.ShouldBe(dir2);
        }
        [TestMethod()]
        public void HowMatchStringsTest2()
        {
            var word = "სალამი";
            var set = new List<string>()
            {
                "სალამი"
                ,"კალამი"
                ,"სალათი"
                ,"კალათი"
                ,"სულამი"
                ,"სალუმი"
                ,"სალამო"
            };
            var result = set.Select(w => new { Word = w, Sim = GeoWordMatcher.EvaluateRhymeSimilarity(word, w) }).OrderByDescending(w => w.Sim);
            for (int i = 0; i < set.Count; i++)
            {
                result.ElementAt(i).Word.ShouldBe(set.ElementAt(i));
            }
        }

        [TestMethod()]
        public void HowMatchStringsTest1()
        {
            GeoWordMatcher.EvaluateRhymeSimilarity("სანდრო","სანდრო").ShouldBe(1);
        }
    }
}
using System;
using GeorgianLanguageApi.Controllers;
using GeorgianWordDetectorCore;
using Shouldly;
using Xunit;

namespace GeorgianLanguageApiTests
{
    //TODO
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var con = new LatinGeoFixer();
            con.FixLatinText("iyo da ara iyo ra").ShouldBe("იყო და არა იყო რა");
            con.FixLatinText("salami").ShouldBe("სალამი");
        }
        [Fact]
        public void Test11()
        {
            var con = new LatinGeoFixer();
            con.FixLatinText("Gushindeli dge iyo bolo tarigi me ase miyxres da gushin mivitane")
                   .ShouldBe("გუშინდელი დღე იყო ბოლო თარიღი მე ასე მითხრეს და გუშინ მივიტანე");
        }
        static WordDetector wd = new WordDetector();
        [Fact]
        public void TestMethod1()
        {
            wd.LooksMoraLikeGeorgianWordThanEnglish("wesiT").ShouldBeTrue();
            wd.LooksMoraLikeGeorgianWordThanEnglish("offer").ShouldBeFalse();
            wd.LooksMoraLikeGeorgianWordThanEnglish("salamander").ShouldBeFalse();
            wd.LooksMoraLikeGeorgianWordThanEnglish("test").ShouldBeFalse();
            wd.LooksMoraLikeGeorgianWordThanEnglish("testing").ShouldBeFalse();
            wd.LooksMoraLikeGeorgianWordThanEnglish("testireba").ShouldBeTrue();
        }

        [Fact]
        public void TestMethod31()
        {
            wd.LooksMoraLikeGeorgianWordThanEnglish("west").ShouldBeTrue();
        }
    }
}

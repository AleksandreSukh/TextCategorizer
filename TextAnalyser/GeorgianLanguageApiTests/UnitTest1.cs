using System;
using GeorgianLanguageApi.Controllers;
using GeorgianWordDetectorCore;
using Shouldly;
using Xunit;

namespace GeorgianLanguageApiTests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var con = new LatinGeoFixer();
            con.FixLatinText("iyo da ara iyo ra").ShouldBe("იყო და არა იყო რა");
            con.FixLatinText("salami").ShouldBe("სალამი");
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

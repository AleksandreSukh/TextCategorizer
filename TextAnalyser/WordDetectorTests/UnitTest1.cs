using System;
using GeorgianWordDetector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace WordDetectorTests
{
    [TestClass]
    public class UnitTest1
    {
        static WordDetector wd = new WordDetector();
        [TestMethod]
        public void TestMethod1()
        {
            wd.LooksMoraLikeGeorgianWordThanEnglish("wesiT").ShouldBeTrue();
            wd.LooksMoraLikeGeorgianWordThanEnglish("offer").ShouldBeFalse();
            wd.LooksMoraLikeGeorgianWordThanEnglish("salamander").ShouldBeFalse();
            wd.LooksMoraLikeGeorgianWordThanEnglish("test").ShouldBeFalse();
            wd.LooksMoraLikeGeorgianWordThanEnglish("testing").ShouldBeFalse();
            wd.LooksMoraLikeGeorgianWordThanEnglish("testireba").ShouldBeTrue();
        }
    }
}

using GeorgianLanguageClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace GeorgianLanguageClassesTests
{
    [TestClass()]
    public class SyllableSplitterTests
    {
        [TestMethod()]
        public void SyllablesTest()
        {
            var result = SyllableSplitter.Syllables("დამარცვლა");
            result.Length.ShouldBe(3);
            result[0].ShouldBe("და");
            result[1].ShouldBe("მარ");
            result[2].ShouldBe("ცვლა");
        }
        [TestMethod()]
        public void SyllablesTest2()
        {
            var result = SyllableSplitter.Syllables("დათვისებრ");
            result.Length.ShouldBe(3);
            result[0].ShouldBe("დათ");
            result[1].ShouldBe("ვი");
            result[2].ShouldBe("სებრ");
        }
        [TestMethod()]
        public void SyllablesTest3()
        {
            var result = SyllableSplitter.Syllables("გააათკეცა");
            result.Length.ShouldBe(5);
            result[0].ShouldBe("გა");
            result[1].ShouldBe("ა");
            result[2].ShouldBe("ათ");
            result[3].ShouldBe("კე");
            result[4].ShouldBe("ცა");
        }
    }
}
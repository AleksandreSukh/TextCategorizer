using GeorgianLanguageClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace GeorgianLanguageClassesTests
{
    [TestClass()]
    public class GeorgianWordExtensionsTests
    {
        [TestMethod()]
        public void Consonants() => "ზღარბი".Consonants().ShouldBe("ზღრბ");
        [TestMethod()]
        public void Vovels() => "ზღარბი".Vowels().ShouldBe("აი");
        [TestMethod()]
        public void Veird() => "ზღურბლს".IsGeorgianWord().ShouldBe(true);
    }
}
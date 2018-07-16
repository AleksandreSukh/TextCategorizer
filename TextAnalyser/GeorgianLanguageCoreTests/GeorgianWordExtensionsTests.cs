using GeorgianLanguageCore;
using Shouldly;
using Xunit;

namespace GeorgianLanguageClassesTests
{
    public class GeorgianWordExtensionsTests
    {
        [Fact]
        public void Consonants() => "ზღარბი".Consonants().ShouldBe("ზღრბ");
        [Fact]
        public void Vovels() => "ზღარბი".Vowels().ShouldBe("აი");
        [Fact]
        public void Veird() => "ზღურბლს".IsGeorgianWord().ShouldBe(true);
    }
}
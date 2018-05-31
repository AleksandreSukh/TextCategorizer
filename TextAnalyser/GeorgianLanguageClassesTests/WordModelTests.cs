using System;
using System.Security;
using GeorgianLanguageClasses;
using GeorgianLanguageClasses.WordModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace GeorgianLanguageClassesTests
{
    [TestClass()]
    public class WordModelTests
    {

        [TestMethod()]
        public void DifferentObjectsWithSimilarTextValueShouldBeEqual()
        {
            Word w1 = new Word(
                new TypedWordPart(new WordPart("sa"),TypedWordPart.WordPartType.Prefix),
                new TypedWordPart(new WordPart("dge"),TypedWordPart.WordPartType.Stem),
                new TypedWordPart(new WordPart("iso"),TypedWordPart.WordPartType.Suffix));
            Word w2 = new Word(
                new TypedWordPart(new WordPart("sa"), TypedWordPart.WordPartType.Prefix),
                new TypedWordPart(new WordPart("dge"), TypedWordPart.WordPartType.Stem),
                new TypedWordPart(new WordPart("iso"), TypedWordPart.WordPartType.Suffix));
            w1.ShouldBe(w2);
        }
        [TestMethod()]
        public void DifferentTypedPartsWithSimilarTextValueShouldBeEqual()
        {
            var twp1=new TypedWordPart(new WordPart("sa"),TypedWordPart.WordPartType.Stem);
            var twp2=new TypedWordPart(new WordPart("sa"),TypedWordPart.WordPartType.Stem);
            twp2.ShouldBe(twp1);
        }
        [TestMethod()]
        public void WordPart_RemoveAtTheBeginning()
        {
            Text wp1 = "sandro";
            var droRemoved = wp1.RemoveAtTheBeginning("san");
            droRemoved.ShouldBe("dro");
        }
        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void WordPart_RemoveAtTheBeginning_WrongValue()
        {
            Text wp1 = "sandro";
            var droRemoved = wp1.RemoveAtTheBeginning("sani");
            droRemoved.ShouldBe("dro");
        }
        [TestMethod()]
        public void WordPart_RemoveAtTheEnd()
        {
            Text wp1 = "sandro";
            var droRemoved = wp1.RemoveAtTheEnd("dro");
            droRemoved.ShouldBe("san");
        }
        [TestMethod()]
        [ExpectedException(typeof(InvalidOperationException))]
        public void WordPart_RemoveAtTheEnd_WrongValue()
        {
            Text wp1 = "sandro";
            var droRemoved = wp1.RemoveAtTheEnd("droi");
            droRemoved.ShouldBe("san");
        }
    }
}
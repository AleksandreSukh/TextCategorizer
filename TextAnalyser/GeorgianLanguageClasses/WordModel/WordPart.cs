using System;
using System.Linq;

namespace GeorgianLanguageClasses.WordModel
{
    public struct WordPart
    {
        public Text TextValue { get; }

        public WordPart(Text textValue)
        {
            if (textValue.Any(char.IsWhiteSpace))
                throw new InvalidOperationException($"{nameof(WordPart)} can't be created from a string which contains white space characters in it");
            TextValue = textValue;
        }
    }
}
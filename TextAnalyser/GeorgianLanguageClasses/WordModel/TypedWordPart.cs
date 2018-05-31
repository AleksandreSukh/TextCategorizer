using System;

namespace GeorgianLanguageClasses.WordModel
{
    public struct TypedWordPart : IEquatable<TypedWordPart>
    {
        public WordPart WordPart { get; }
        public WordPartType Type { get; }

        public TypedWordPart(WordPart wordPart, WordPartType type)
        {
            WordPart = wordPart;
            Type = type;
        }

        public enum WordPartType
        {
            Prefix = 1,
            Stem = 2,
            Suffix = 3
        }

        public bool Equals(TypedWordPart other) => Type == other.Type;
    }
}
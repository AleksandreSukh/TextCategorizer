using System;
using System.Linq;

namespace GeorgianLanguageClasses.WordModel
{
    public struct Word : IEquatable<Word>
    {
        private readonly TypedWordPart[] Parts;
        public Word(params TypedWordPart[] parts)
        {
            if (parts.Count(p => p.Type == TypedWordPart.WordPartType.Stem) != 1)
            { throw new InvalidOperationException($"One and single {nameof(TypedWordPart.WordPartType.Stem)} is required to create {nameof(Word)}"); }
            if (parts.Length > 1)
            {
                var partsList = parts.ToList();
                var indexOfStem = partsList.FindIndex(p => p.Type == TypedWordPart.WordPartType.Stem);
                var indexOfPrefix = partsList.FindIndex(p => p.Type == TypedWordPart.WordPartType.Prefix);
                var indexOfSuffix = partsList.FindIndex(p => p.Type == TypedWordPart.WordPartType.Suffix);

                if (indexOfPrefix > indexOfStem || (indexOfSuffix != -1 && indexOfSuffix < indexOfStem))
                    throw new InvalidOperationException($"{nameof(TypedWordPart)}s should be passed with following type order Prefix,Stem,Suffix");
            }
            var val = string.Empty;
            Parts = parts;
            foreach (var part in parts)
            {
                val += part.WordPart.TextValue.StringValue;
            }
            TextValue = new Text(val);
        }

        public Text TextValue { get; }

        public bool Equals(Word other) => Equals(TextValue, other.TextValue);
    }

    public enum MetyvelebisNawili
    {
        არსებითი_სახელი,
        ზედსართავი_სახელი,
        რიცხვითი_სახელი,
        ნაცვალსახელი,
        ზმნა,
        ზმნიზედა,
        თანდებული,
        კავშირი,
        ნაწილაკი,
        შორისდებული
    }
}

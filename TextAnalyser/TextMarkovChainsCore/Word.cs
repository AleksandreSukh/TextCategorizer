using System;
using System.Collections.Generic;
using System.Linq;

namespace TextMarkovChains
{
    public class Word : IEquatable<Word>, IWord
    {
        public static List<string> AllWords = new List<string>();

        private readonly int _index;

        public Word(string value)
        {
            if (!AllWords.Contains(value))
                AllWords.Add(value);
            _index = AllWords.IndexOf(value);
        }
        public static implicit operator Word(string value)
        {
            return new Word(value);
        }

        public string Text => AllWords.ElementAt(_index);

        public bool Equals(Word other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _index == other._index;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Word)obj);
        }

        public override int GetHashCode()
        {
            return _index;
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
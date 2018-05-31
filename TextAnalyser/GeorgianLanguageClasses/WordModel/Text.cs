using System;
using System.Collections;
using System.Collections.Generic;

namespace GeorgianLanguageClasses.WordModel
{
    public struct Text : IEquatable<Text>, IEnumerable<char>
    {
        public Text(string stringValue)
        {
            if (string.IsNullOrWhiteSpace(stringValue))
                throw new InvalidOperationException($"{nameof(Text)} can't be created by an empty string");
            StringValue = stringValue;
        }

        public string StringValue { get; }

        #region Overloads

        public bool Equals(Text other)
        {
            return StringValue.Equals(other.StringValue);
        }


        public static implicit operator Text(string value) => new Text(value);

        public static Text operator +(Text first, Text second)
        {
            return new Text(first.StringValue + second.StringValue);
        }

        #endregion
        /// <summary>
        /// Just for syntax, You can use operator + instead
        /// </summary>
        /// <param name="second"></param>
        /// <returns></returns>
        public Text AddAtBeginnig(Text second) => second + this;

        /// <summary>
        /// Just for syntax, You can use operator + instead
        /// </summary>
        /// <param name="second"></param>
        /// <returns></returns>
        public Text AddToTheEnd(Text second) => this + second;

        public Text RemoveAtTheEnd(Text second)
        {
            if (!StringValue.EndsWith(second.StringValue))
                throw new InvalidOperationException("In order to cut part of text the text must contain the part");
            return new Text(StringValue.Remove(StringValue.Length - second.StringValue.Length));
        }

        public Text RemoveAtTheBeginning(Text second)
        {
            if (!StringValue.StartsWith(second.StringValue))
                throw new InvalidOperationException("In order to cut part of text the text must contain the part");
            return new Text(StringValue.Remove(0, second.StringValue.Length));
        }

        public IEnumerator<char> GetEnumerator() => StringValue.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
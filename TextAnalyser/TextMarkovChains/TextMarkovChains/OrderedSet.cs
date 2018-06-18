using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TextMarkovChains
{
    public class OrderedStringSet : ICollection<string>
    {
        private readonly IDictionary<string, LinkedListNode<string>> m_Dictionary;
        private readonly LinkedList<string> m_LinkedList;

        public OrderedStringSet()
            : this(EqualityComparer<string>.Default)
        {
        }

        public OrderedStringSet(IEqualityComparer<string> comparer)
        {
            m_Dictionary = new Dictionary<string, LinkedListNode<string>>(comparer);
            m_LinkedList = new LinkedList<string>();
        }

        public int Count
        {
            get { return m_Dictionary.Count; }
        }

        public virtual bool IsReadOnly
        {
            get { return m_Dictionary.IsReadOnly; }
        }

        void ICollection<string>.Add(string item)
        {
            Add(item);
        }

        public bool Add(string item)
        {
            if (m_Dictionary.ContainsKey(item)) return false;
            LinkedListNode<string> node = m_LinkedList.AddLast(item);
            m_Dictionary.Add(item, node);
            return true;
        }

        public void Clear()
        {
            m_LinkedList.Clear();
            m_Dictionary.Clear();
        }

        public bool Remove(string item)
        {
            LinkedListNode<string> node;
            bool found = m_Dictionary.TryGetValue(item, out node);
            if (!found) return false;
            m_Dictionary.Remove(item);
            m_LinkedList.Remove(node);
            return true;
        }

        public IEnumerator<string> GetEnumerator()
        {
            return m_LinkedList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Contains(string item)
        {
            return m_Dictionary.ContainsKey(item);
        }

        public void CopyTo(string[] array, int arrayIndex)
        {
            m_LinkedList.CopyTo(array, arrayIndex);
        }
        public int IndexOf(string value)
        {
            var arr = this.ToArray();
            var res = Array.FindIndex(arr, v => v == value);
            return res;
        }
    }
}
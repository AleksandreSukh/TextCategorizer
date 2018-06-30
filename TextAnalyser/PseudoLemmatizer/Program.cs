using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeorgianWordsDataBase;

namespace PseudoLemmatizer
{
    class Program
    {
        static GeorgianWordsDb db = new GeorgianWordsDb();
        static void Main(string[] args)
        {
            var groupedByMiddleParts = db.AllWords.GroupBy(w =>
            {
                var length = w.Length;
                var halfLength = length / 2;
                var oneFourth = halfLength / 2;
                //var middlePart = w.Remove(w.Length - 1 - oneFourth).Remove(0, oneFourth);
                var middlePart = w
                    .TrimEnd('ს')
                    .TrimEnd('თ')
                    .TrimEnd('დ')
                    .TrimEnd('მ')
                    .TrimEnd('ა').TrimEnd('მ')
                    .TrimEnd('ი')
                    .TrimEnd('ო');
                return middlePart;
            });

            var broaderGroups = groupedByMiddleParts.Select(group => new { Key = group.Key, Elements = db.AllWords.Where(w => w.Contains(group.Key)) })
                .OrderByDescending(g=>g.Key.Length)
                .ToArray();
            for (int i1 = 0; i1 < broaderGroups.Length; i1++)
            {
                for (var i2 = 0; i2 < broaderGroups.Length; i2++)
                {
                    if (i1 != i2)
                    {
                        if(
                            broaderGroups[i1].Key.Contains(broaderGroups[i2].Key)
                            ||broaderGroups[i2].Key.Contains(broaderGroups[i1].Key)
                        )
                            continue;
                        var itemsToBeRemovedFromBoth = new List<string>();
                        for (int i = 0; i < broaderGroups[i1].Elements.Count(); i++)
                        {
                            var current = broaderGroups[i1].Elements.ElementAt(i);
                            if (broaderGroups[i2].Elements.Contains(current))
                            {
                                itemsToBeRemovedFromBoth.Add(current);
                            }
                        }

                        if (itemsToBeRemovedFromBoth.Count > 0)
                        {
                            var clean1 = broaderGroups[i1].Elements.ToList();
                            clean1.RemoveAll(itemsToBeRemovedFromBoth.Contains);
                            var clean2 = broaderGroups[i2].Elements.ToList();
                            clean2.RemoveAll(itemsToBeRemovedFromBoth.Contains);
                            broaderGroups[i1] = new { Key = broaderGroups[i1].Key, Elements = clean1.AsEnumerable() };
                            broaderGroups[i2] = new { Key = broaderGroups[i2].Key, Elements = clean2.AsEnumerable() };
                        }
                    }
                }
            }
            //            var poorlyGrouped = groupedByMiddleParts.OrderBy(g => g.Count()).Take(100);


        }
    }
}

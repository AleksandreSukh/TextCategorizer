using System;
using System.Collections.Generic;
using System.Linq;

namespace GeorgianLanguageClasses
{
    public static class ConsonantPhoneticSimilarity
    {
        const string FoneticMatrixString = "5322100021211002010000200020052110201101100023000010003000501101010210000100002000200005100122002000010000000010000050000103110001000030001000000500010001101000220000000000005000100020202000001000000000051000000000000000000000000000520110010000000000000000000005011000000000000000000000000050002000000000000000000000000510000000001000300000000000005000020000100210000000000000050000031300000100000000000000500000000200000000000000000005200001000100000000000000000050001000020100000000000000000510001001100000000000000000005000000000000000000000000000053100110100000000000000000000500021000000000000000000000005021101000000000000000000000051002000000000000000000000000520000000000000000000000000005000000000000000000000000000050000000000000000000000000000500000000000000000000000000005";
        private static readonly int[][] FoneticMatrix;

        static ConsonantPhoneticSimilarity()
        { FoneticMatrix = FoneticMatrixStringToIntArray(); }

        private static int[][] FoneticMatrixStringToIntArray()
        {
            var charArray = new int[GeorgianAlphabet.Consonants.Length][];
            var chunks = ToChunks(FoneticMatrixString, 28).ToList();
            var ctr = 0;
            foreach (var item in chunks)
            {
                charArray[ctr] = item.Select(ch => (int)ch - 48).ToArray();
                ++ctr;
            }
            return charArray;
        }

        static IEnumerable<string> ToChunks(this string str, int chunkSize)
        {
            return Enumerable.Range(0, str.Length / chunkSize)
                .Select(i => str.Substring(i * chunkSize, chunkSize));
        }
        /// <summary>
        /// This method compares two chars and returns point (0 to 5) how much phonetically similar (sound similarly) they ar to each other. 
        /// </summary>
        /// <param name="char1"></param>
        /// <param name="char2"></param>
        /// <returns>returns int (0 to 5)</returns>
        public static int GetSimilarity(char char1, char char2)
        {
            if (char1 > char2)
            {
                var swapper = char1;
                char1 = char2;
                char2 = swapper;
            }
            var index1 = GeorgianAlphabet.Consonants.IndexOf(char1);
            var index2 = GeorgianAlphabet.Consonants.IndexOf(char2);
            return Convert.ToInt32(FoneticMatrix.ElementAt(index1).ElementAt(index2));
        }
    }
}

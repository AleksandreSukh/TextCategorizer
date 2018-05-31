using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Word;

namespace WordTextExtractor
{
    class Program
    {
        static void Main(string[] args)
        {
        }
        /// <summary>
        /// მეთოდი რომელიც წაიკითხავს დოკუმენტს და დააბრუნებს ტექსტს
        /// </summary>
        /// <param name="path">დოკუმენტის მისამართი</param>
        /// <returns></returns>
        public string ParseDocument(string path)
        {
            var word = new Application();
            var docs = word.Documents.Open(path);
            var totaltext = "";
            for (var i = 0; i < docs.Paragraphs.Count; i++)
            {
                totaltext += " \r\n " + docs.Paragraphs[i + 1].Range.Text;
            }
            docs.Close();
            word.Quit();
            return totaltext;
        }
    }
    
}

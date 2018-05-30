using System.Linq;
using GeorgianLanguageClasses;
using Stemmer;

namespace TextAnalyser
{
    //ტექსტის ჯაჭვი რომელსაც დავუმატეთ სიტყვების გაფილტვრის ფუნქცია 
    public class RefinedMarkovChain : TextMarkovChain
    {
        readonly StemmerGeo stemmer = new StemmerGeo();
        //კავშირები რომლებიც გვინდა გავფილტროთ სანამ დავამატებთ ჯაჭვში
        static readonly string[] Kavshirebi = new string[] {
            "და","რომ", "თუ", "არა", "რათა", "რაკი"
            , "ვიდრე", "ვინც", "რაც", "რომელიც",
            "როგორიც", "სადაც", "საიდანაც",
            "საითკენაც", "როდესაც",
         "მაგრამ", "ხოლო", "თორემ", "ან", "ანუ"};
        public TextCategory Category { get; set; }
        /// <summary>
        /// ეს მეთოდი ფილტრავს კავშირებს, ცარიელ ტექსტებს, არაქართულ სიტყვებს
        /// შემდეგ ფუძის ამოღების ალგორითმით იღებს ტექსტიდან სიტყვებს ფუძეებით 
        /// და ამის შემდეგ ამატებს ჯაჭვში
        /// </summary>
        /// <param name="refineable"></param>
        /// <returns></returns>
        protected override string[] WordRefinerBeforeAddingToChain(string[] refineable)
        {
            if (refineable == null || refineable.Length == 0) return refineable;//თუ სიტყვების მასივი ცარიელია ვაბრუნებთ იმავეს
            var filtered = refineable.Where(s => !string.IsNullOrEmpty(s)//გამოვრიცხეთ ცარიელი ტექსტები
            && GeorgianWord.IsGeorgianWord(s)//უნდა იყოს ქართული სიტყვა
            && !Kavshirebi.Contains(s)//არ უნდა იყოს კავშირი
            //&& !s.Contains('-')
            );
            
            var stemmed = stemmer.Lemmatize(filtered.ToArray());//ფუძეების ამოკრება
            return stemmed.Where(s => !string.IsNullOrEmpty(s)).ToArray();//გამოვრიცხეთ რომ ფუძის ამოღების შემდეგ არ გვაქვს ცარიელი ტექსტები
        }
    }
}

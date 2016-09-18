using System;
using System.IO;
using System.Linq;
using Microsoft.Office.Interop.Word;

namespace TextAnalyser
{
    /// <summary>
    /// კლასი Microsoft Word ის ფაილებიდან ტექსტის ამოღების და ბაზაში ჩაწერისთვის
    /// </summary>
    public class WordDocParser
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="directoryName">ფოლდერი საიდანაც წაიკითხავს ყველა .docx ფაილს</param>
        /// <param name="category">კატეგორია რომელსაც მიანიჭებს ბაზაში</param>
        public void ParseCategorisedWordFilesFromDirectory(string directoryName, TextCategory category)
        {
            //დოკუმენტების აღება
            var fileNames =
                Directory.GetFiles(directoryName)
                    .Where(f => f.EndsWith(".docx", StringComparison.InvariantCultureIgnoreCase));
            
            //ყოველი დოკუმენტისთვის
            foreach (var filePath in fileNames)
            {
                var fileName =
                    Path.GetFileNameWithoutExtension(filePath);

                using (var db = new TextDataStoreEntities())
                {
                    //თუ ფაილი ამ სახელით უკვე დაპარსულია გამოვტოვოთ
                    if (db.CategorisedTexts.Any(c => c.Name == fileName))
                        continue;

                    //ფაილიდან ტექსტის წაკითხვა
                    var parsed = ParseDocument(filePath);

                    if (!string.IsNullOrEmpty(parsed))
                    {
                        //ბაზის შესაბამის სტრუქტურაში გადაწერა
                        var categorisedText = new CategorisedText()
                        {
                            Name = fileName,
                            Category = (short)category,
                            Content = parsed
                        };

                        var maxId = db.CategorisedTexts.Any() ? db.CategorisedTexts.Select(t => t.Id).Max() : 0;

                        categorisedText.Id = maxId + 1;
                        db.CategorisedTexts.Add(categorisedText);
                        //ბაზაში შენახვა
                        db.SaveChanges();
                    }
                }
            }
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
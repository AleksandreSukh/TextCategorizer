using System.Linq;

namespace TextAnalyser
{
    /// <summary>
    /// ჯაჭვის შევსება ბაზიდან
    /// </summary>
    public class ChainFeeder
    {
        /// <summary>
        /// ეს მეთოდი ავსებს გადაცემულ ჯაჭვს შესაბამისი კატეგორიის ტექსტებით ბაზიდან
        /// </summary>
        /// <param name="chain">შესავსები ჯაჭვი</param>
        public void Feed(RefinedMarkovChain chain)
        {
            using (var db = new TextDataStoreEntities())
            {
                foreach (var text in db.CategorisedTexts.Where(t => t.Category == (short)chain.Category))
                {
                    chain.Feed(text.Content);
                }
            }
        }
    }
}
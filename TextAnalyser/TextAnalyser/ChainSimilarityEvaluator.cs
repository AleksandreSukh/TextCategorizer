using System.Linq;

namespace TextAnalyser
{
    /// <summary>
    /// კლასი ჯაჭვების ქვემიმდევრობების მიხედვით მსგავსების გამოსათვლელად
    /// </summary>
    public class ChainSimilarityEvaluator
    {
        /// <summary>
        /// მეთოდი რომელიც ითვლის ორ ჯაჭვს შორის მსგავსებას
        /// </summary>
        /// <param name="chain1">პირველი ჯაჭვი</param>
        /// <param name="chain2">მეორე ჯაჭვი</param>
        /// <returns></returns>
        public int EvaluateSimilarity(TextMarkovChain chain1, TextMarkovChain chain2)
        {
            var result = 0;
            /*
             * პირველი ჯაჭვის ყოველი სუბ ჯაჭვისთვის
             * ვპოულობთ ისეთ სუბ ჯაჭვებს მეორე ჯაჭვიდან
             * რომლებიც სათავით არიან მსგავსები,
             * ანუ იმისათვის რომ გამოვითვალოთ მიმდევრობათა 
             * მსგავსებები, აუცილებელია, რომ მიმდევრობებს 
             * დასაწყისები იყოს ერთი და იგივე სიტყვა
             * (მოკლედ, მიმდევრობების შედარებას აზრი არ აქვს 
             * თუ ორ ტექსტში მსგავსი სიტყვები არ გვაქვს)
             */
            foreach (var subChain1 in chain1.Chains)
            {
                result += chain2.Chains.Where(subChain2 => subChain1.Key == subChain2.Key)
                    .Sum(subChain2 => EvaluateChainSimilarity(subChain1.Value, subChain2.Value));
            }
            return result;
        }
        /// <summary>
        /// მეთოდი რომელიც ითვლის ქვემიმდევრობების მსგავსებას
        /// </summary>
        /// <param name="chain1">პირველი ქვემიმდევრობა</param>
        /// <param name="chain2">მეორე ქვემიმდევრობა</param>
        /// <returns></returns>
        public int EvaluateChainSimilarity(TextMarkovChain.Chain chain1, TextMarkovChain.Chain chain2)
        {
            var result = 0;
            if (chain1.Word != chain2.Word)
                return 0;
            //ყოველი გადასვლისთვის პირველი ჯაჭვიდან
            foreach (var prob1 in chain1.GetProbabilities())
            {
                //ყოველი გადასვლისთვის მეორე ჯაჭვიდან
                foreach (var prob2 in chain2.GetProbabilities())
                {
                    /*
                     * 
                     * თუ ჯაჭვებში მოიძებნა მსგავსი მიმდევრობა
                     * მაშინ მსგავსებას ვზრდით ამ მიმდევრობების
                     * ალბათობების ნამრავლით.
                     * იმიტომ რომ თუ ჯაჭვი რომლის წყაროს (რა ტიპის
                     * ტექსტითაც შევავსეთ) კატეგორიაც განსაზღვრული გვაქვს
                     * ედარება ჯაჭვს რომლის კატეგორიაც ჯერ არ ვიცით 
                     * რადგან გადავწყვიტე, რომ მსგავსების უფრო მაღალი
                     * ხარისხის დასტურია როცა არა მხოლოდ მიმდევრობაა
                     * მსგავსი არამედ ამ მიმდევრობათა შეხვედრის ალბათობები 
                     */
                    if (prob1.Key == prob2.Key)
                    {
                        result += prob1.Value.Count * prob2.Value.Count;
                    }

                }
            }
            return result;
        }
    }
}
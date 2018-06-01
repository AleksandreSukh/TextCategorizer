using System;
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
        public double EvaluateSimilarity(TextMarkovChain chain1, TextMarkovChain chain2)
        {
            var result = 0.0;
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
                foreach (var subChain2 in chain2.Chains)
                {
                    if (subChain1.Key == subChain2.Key)
                        result += EvaluateChainSimilarity(subChain1.Value, subChain2.Value);
                }
            }
            return result;
        }
        /// <summary>
        /// მეთოდი რომელიც ითვლის ქვემიმდევრობების მსგავსებას
        /// </summary>
        /// <param name="chain1">პირველი ქვემიმდევრობა</param>
        /// <param name="chain2">მეორე ქვემიმდევრობა</param>
        /// <returns></returns>
        public double EvaluateChainSimilarity(TextMarkovChain.Chain chain1, TextMarkovChain.Chain chain2)
        {
            var result = 0.0;
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
                        var probability = prob1.Value.Count;
                        var probability2 = prob2.Value.Count;
                        var maxProbability = chain1.GetProbabilities().Values.Select(cp => cp.Count).Max();
                        var maxProbability2 = chain2.GetProbabilities().Values.Select(cp => cp.Count).Max();

                        var p1 = Convert.ToDouble(probability) / maxProbability;
                        var p2 = Convert.ToDouble(probability2) / maxProbability2;

                        result += p1 * p2;
                    }

                }
            }
            return result;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace TextAnalyser
{
    public class TextCategoryEvaluatorEntry
    {
        private List<RefinedMarkovChain> _sampleCategorisedChains;
        private bool _dataHasBeenLoaded = false;
        /// <summary>
        /// ეს მეთოდი ადგენს მოცემული ტექსტის კატეგორიას
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public TextCategory EvaluateTextCategory(string text)
        {
            //თუ ჯერ არ ჩაგვიტვირთავს ცოდნის ბაზა მაშინ ჩავტვირთოთ
            if (!_dataHasBeenLoaded)
                LoadXmlsIntoCategorisedSampleChainsList();

            //შევქმნათ გაფილტვრადი (ფილტრიანი :)) ჯაჭვი
            var chain = new RefinedMarkovChain();
            //შევავსოთ ჯაჭვი ტექსტით რომ მივიღოთ მოცემული ტექსტის ჯაჭვი
            chain.Feed(text);

            //შევქმნათ ჯაჭვების შემდარებელი
            var evaluator = new ChainSimilarityEvaluator();

            //შევადაროთ ჯაჭვი ცოდნის ბაზაში არსებულ ჯაჭვებს და ამოვკრიბოთ რომელი კატეგორიის ჯაჭვებს გავს და რამდენად
            var probabilities = _sampleCategorisedChains.Select(c => new { c.Category, Value = evaluator.EvaluateSimilarity(chain, c) });

            //დავალაგოთ მსგავსების კლებადობით
            var mostPossibles = probabilities.OrderByDescending(p => p.Value).ToArray();

            //დავრწმუნდეთ რომ ყველა კატეგორიაზე შემოწმდა (გარდა Undefined ისა)
            if (mostPossibles.Count() < Enum.GetValues(typeof(TextCategory)).Length - 1) throw new Exception("Error! Not all categories were considered");

            //ავიღოთ ყველაზე სავარაუდო კატეგორია სავარაუდობის კოეფიციენტითურთ
            var first = mostPossibles[0];

            //ავიღოთ შემდეგი ყველაზე სავარაუდო კატეგორია სავარაუდობის კოეფიციენტითურთ
            var secondPossible = mostPossibles[1];

            //თუ მოცემული ტექსტი ერთნაირად არ გავს ორ სხვადასხვა კატეგორიის ტექსტს მაშინ დავაბრუნოთ რომ ვერ განვსაზღვრეთ
            if (first.Value == secondPossible.Value) return TextCategory.Undefined;

            //დავაბრუნოთ ყველაზე სავარაუდო კატეგორია
            var result = first.Category;
            return result;
        }

        /// <summary>
        /// XML ფაილებიდან ჯაჭვების ჩატვირთვა 
        /// (ცოდნის ბაზა რომლის მიხედვითაც განვსაზღვრით ახალი ტექსტების კატეგორიას)
        /// </summary>
        public void LoadXmlsIntoCategorisedSampleChainsList()
        {
            //--ცარიელი კატეგორიზებული მარკოვის ჯაჭვების შექმნა
            var chainEconomics = new RefinedMarkovChain() { Category = TextCategory.Economics };
            var chainMedical = new RefinedMarkovChain() { Category = TextCategory.Medical };
            var chainLaw = new RefinedMarkovChain() { Category = TextCategory.Law };

            //--ჯაჭვების შევსება Xml ფაილებიდან
            var xdEcon = new XmlDocument();
            xdEcon.Load($"{nameof(chainEconomics)}.xml");
            chainEconomics.Feed(xdEcon);

            var xdMed = new XmlDocument();
            xdMed.Load($"{nameof(chainMedical)}.xml");
            chainMedical.Feed(xdMed);

            var xdLaw = new XmlDocument();
            xdLaw.Load($"{nameof(chainLaw)}.xml");
            chainLaw.Feed(xdLaw);

            //--შესადარებელი ჯაჭვების სიის შექმნა
            _sampleCategorisedChains = new List<RefinedMarkovChain>();

            //--კატეგორიზებული ჯაჭვების დამატება შესადარებელი ჯაჭვების სიაშ
            _sampleCategorisedChains.Add(chainEconomics);
            _sampleCategorisedChains.Add(chainMedical);
            _sampleCategorisedChains.Add(chainLaw);

            _dataHasBeenLoaded = true;
        }
    }
}
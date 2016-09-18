namespace TextAnalyser
{
    class Program
    {
        static void Main(string[] args)
        {

            //--ტექსტის დაპარსვა ვორდიდან და ბაზაში შენახვა
            //var parser = new WordDocParser();

            //parser.ParseCategorisedWordFilesFromDirectory(@"E:\Proeqti\texts\ეკონომომიკა", TextCategory.Economics);
            //parser.ParseCategorisedWordFilesFromDirectory(@"E:\Proeqti\texts\მედიცინა", TextCategory.Medical);
            //parser.ParseCategorisedWordFilesFromDirectory(@"E:\Proeqti\texts\სამართალი", TextCategory.Law);

            //--ცარიელი კატეგორიზებული მარკოვის ჯაჭვების შექმნა
            var chainEconomics = new RefinedMarkovChain() { Category = TextCategory.Economics };
            var chainMedical = new RefinedMarkovChain() { Category = TextCategory.Medical };
            var chainLaw = new RefinedMarkovChain() { Category = TextCategory.Law };
            
            //--ჯაჭვების შევსება ბაზაში შენახული კატეგორიზებული ტექსტებით 
            //var chainFeeder = new ChainFeeder();
            //chainFeeder.Feed(chainEconomics);
            //chainFeeder.Feed(chainMedical);
            //chainFeeder.Feed(chainLaw);

            //--ჯაჭვების სერიალიზაცია Xml ფაილებში
            //chainEconomics.Save($"{nameof(chainEconomics)}.xml");
            //chainMedical.Save($"{nameof(chainMedical)}.xml");
            //chainLaw.Save($"{nameof(chainLaw)}.xml");

            //--ჯაჭვების შევსება Xml ფაილებიდან
            //var xdEcon = new XmlDocument();
            //xdEcon.Load($"{nameof(chainEconomics)}.xml");
            //chainEconomics.Feed(xdEcon);
            //var xdMed = new XmlDocument();
            //xdMed.Load($"{nameof(chainMedical)}.xml");
            //chainMedical.Feed(xdMed);
            //var xdLaw = new XmlDocument();
            //xdLaw.Load($"{nameof(chainLaw)}.xml");
            //chainLaw.Feed(xdLaw);

             

        }

    }
}

using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Xml;
using Newtonsoft.Json;
using TextMarkovChains;
using WordDataAdapter;

namespace WordDeepModel
{
    //public class ChainLoaderToDb
    //{
    //    protected string XmlFileName { get; } = "geo_model.xml";

    //    private int chunkCounter = 0;



    //    public void Load(Action<int> fileBeingLoadedLogger)
    //    {
    //        var xmlDocument = new XmlDocument();
    //        if (File.Exists(XmlFileName))
    //        {
    //            xmlDocument.Load(XmlFileName);
    //            var Chain = new TextMarkovChain();
    //            Chain.Feed(xmlDocument);
    //        }
    //        else
    //        {
    //            var loadingChunkCounter = 0;
    //            while (File.Exists(FileNamePattern(loadingChunkCounter)))
    //            {
    //                xmlDocument.Load(FileNamePattern(loadingChunkCounter));
    //                var Chain = new TextMarkovChain();

    //                Chain.Feed(xmlDocument);

    //                SaveToDb(Chain, new List<Tag>() { TagSafe.Create(TagNames.Following) });


    //                fileBeingLoadedLogger?.Invoke(loadingChunkCounter);
    //                loadingChunkCounter++;
    //            }
    //        }
    //    }
    //    public class TagSafe
    //    {
    //        private static readonly string[] AvailableNames = Enum.GetNames(typeof(TagNames));
    //        public static Tag Create(string name)
    //        {
    //            if (!AvailableNames.Contains(name))
    //                throw new InvalidDataException("Available tags are:" + string.Join(";", AvailableNames));
    //            var t = new Tag();
    //            t.Name = name;
    //            return t;
    //        }

    //        public static Tag Create(TagNames tagName)
    //        {
    //            var t = new Tag();
    //            t.Name = tagName.ToString();
    //            return t;
    //        }
    //    }

    //    private void SaveToDb(TextMarkovChain chain, List<Tag> tags)
    //    {
    //        using (var context = new WordDbContext())
    //        {
    //            foreach (var c in chain.Chains)
    //            {
    //                if (!context.Nodes.Any(n => n.Data == c.Key))
    //                    context.Nodes.Add(new Node() { Data = c.Key });
    //                context.SaveChanges();
    //                var current = context.Nodes.First(n => n.Data == c.Key);
    //                var probabilities = c.Value.GetProbabilities();
    //                foreach (var p in probabilities)
    //                {
    //                    var target = context.Nodes.FirstOrDefault(n => n.Data == p.Key) ?? new Node() { Data = p.Key };
    //                    foreach (var tag in tags)
    //                    {
    //                        current.Tags.Add(new Tag()
    //                        {
    //                            To = target,
    //                            Occurences = p.Value.count,
    //                            Name = tag.Name
    //                        });
    //                        context.SaveChanges();
    //                    }
    //                }
    //            }
    //        }
    //    }


    //    string FileNamePattern(int number)
    //    {
    //        var existingFilePath = Path.Combine(Directory.GetCurrentDirectory(), XmlFileName);
    //        var fileNameWe = Path.GetFileNameWithoutExtension(existingFilePath);
    //        var e = Path.GetExtension(existingFilePath);

    //        var dir = Path.GetDirectoryName(existingFilePath);

    //        var numberField = $"_{number}_";
    //        return Path.Combine(dir, $"{fileNameWe}{numberField}{e}");
    //    }
    //}
    public class ChainLoaderToDb
    {
        protected string XmlFileName { get; } = "geo_model_deep.xml";

        private int chunkCounter = 0;



        public void Load(Action<int> fileBeingLoadedLogger)
        {
            Directory.SetCurrentDirectory(@"D:\Aleks\TextDataRepository\MarkovChainModels");
            var xmlDocument = new XmlDocument();
            if (File.Exists(XmlFileName))
            {
                xmlDocument.Load(XmlFileName);
                var Chain = new MultiDeepMarkovChainOptimized(3);
                Chain.Feed(xmlDocument);
            }
            else
            {
                var Chain = new MultiDeepMarkovChainOptimized(3);

                var loadingChunkCounter = 0;
                while (File.Exists(FileNamePattern(loadingChunkCounter)))
                {
                    xmlDocument.Load(FileNamePattern(loadingChunkCounter));

                    Chain.Feed(xmlDocument);

                    SaveToDb(Chain, new List<Tag>() { TagSafe.Create(TagNames.Following) },loadingChunkCounter);


                    fileBeingLoadedLogger?.Invoke(loadingChunkCounter);
                    loadingChunkCounter++;
                }
            }
        }
        public class TagSafe
        {
            private static readonly string[] AvailableNames = Enum.GetNames(typeof(TagNames));
            public static Tag Create(string name)
            {
                if (!AvailableNames.Contains(name))
                    throw new InvalidDataException("Available tags are:" + string.Join(";", AvailableNames));
                var t = new Tag();
                t.Name = name;
                return t;
            }

            public static Tag Create(TagNames tagName)
            {
                var t = new Tag();
                t.Name = tagName.ToString();
                return t;
            }
        }

        private void SaveToDb(MultiDeepMarkovChainOptimized chain, List<Tag> tags, int number)
        {
            File.WriteAllText($"DeepChain{number}.json", JsonConvert.SerializeObject(chain));
        }


        string FileNamePattern(int number)
        {
            var existingFilePath = Path.Combine(Directory.GetCurrentDirectory(), XmlFileName);
            var fileNameWe = Path.GetFileNameWithoutExtension(existingFilePath);
            var e = Path.GetExtension(existingFilePath);

            var dir = Path.GetDirectoryName(existingFilePath);

            var numberField = $"_{number}_";
            return Path.Combine(dir, $"{fileNameWe}{numberField}{e}");
        }
    }
}
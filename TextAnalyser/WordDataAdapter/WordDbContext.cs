using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace WordDataAdapter
{
    public class WordDbContext : DbContext
    {
        public WordDbContext()
            : base("name=WordDb")
        {
        }

        public virtual DbSet<Node> Nodes { get; set; }
        public virtual DbSet<Tag> Tags { get; set; }
    }

    public class Node
    {
        [Key]
        public string Data { get; set; }
        public List<Tag> Tags { get; set; } = new List<Tag>();
    }


    public class Tag
    {
        [Key]
        public string Name { get; set; }
        [Key]
        public Node To { get; set; }
        public int Occurences { get; set; }
    }

   public enum TagNames
    {
        Undefined = 0,
        Following = 1
    }

}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TestParserAnimeGo
{
    internal class ApplicationContext:DbContext
    {
        public DbSet<Anime> Anime { get; set; }
        public DbSet<Genre> Genre { get; set; }
        public DbSet<Voiceover> Voiceover { get; set; }

        public ApplicationContext()
        {
           
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=animegodb;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Anime>().HasIndex(a => a.IdFromAnimeGo).IsUnique();
        }
    }
}

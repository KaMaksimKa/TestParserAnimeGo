using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestParserAnimeGo
{
    internal class Genre
    {
        public int GenreId { get; set; }
        public string Name { get; set; }
        public int AnimeId { get; set; }
        public Anime Anime { get; set; }
    }
}

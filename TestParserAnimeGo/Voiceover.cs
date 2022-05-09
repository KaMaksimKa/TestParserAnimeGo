using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestParserAnimeGo
{
    internal class Voiceover
    {
        public int VoiceoverId { get; set; }
        public string Name { get; set; }
        public int AnimeId { get; set; }
        public Anime Anime { get; set; }
    }
}

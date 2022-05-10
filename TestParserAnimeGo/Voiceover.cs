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
        public string NameRu { get; set; }
        public string NameEn { get; set; } = String.Empty;
        public List<Anime> Anime { get; set; }
    }
}

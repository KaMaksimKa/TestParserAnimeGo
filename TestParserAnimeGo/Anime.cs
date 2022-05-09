using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestParserAnimeGo
{
    internal class Anime
    {
        public int AnimeId { get; set; }
        public string? NameEn { get; set; }
        public string? NameRu { get; set; }
        public string? Type { get; set; }
        public int? Year { get; set; }
        public string? Description { get; set; }
        public decimal? Rate { get; set; }
        public string? Status { get; set; }
        public List<Genre> Genres { get; set; } = new List<Genre>();
        public int? CountEpisode { get; set; }
        public string? MpaaRate { get; set; }
        public int? Planned { get; set; }
        public int? Completed { get; set; }
        public int? Watching { get; set; }
        public int? Dropped { get; set; }
        public int? OnHold { get; set; }
        public string? Href { get; set; }
        public string? Studio { get; set; }
        public List<Voiceover> Voiceovers { get; set; } = new List<Voiceover>();
        public string? NextEpisode { get; set; }
        public int? IdFromAnimeGo { get; set; }
        public string? Duration { get; set; }
    }
}

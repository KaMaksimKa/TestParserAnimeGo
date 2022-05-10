using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestParserAnimeGo
{
    internal class AnimeFromParserToDbWriter:IDisposable
    {
        private ApplicationContext _context;
        public AnimeFromParserToDbWriter()
        {
            _context = new ApplicationContext();
        }

        public void AddAnimeRange(List<Anime> animes)
        {
            var genres = _context.Genre.ToList();
            var voiceovers = _context.Voiceover.ToList();

            foreach (var anime in animes)
            {
                var newAnimeGenres = new List<Genre>();
                foreach (var genreOld in anime.Genres)
                {
                    if (!genres.Select(g => g.NameRu).Contains(genreOld.NameRu))
                    {
                        genres.Add(genreOld);
                    }
                    newAnimeGenres.Add(genres.First(g => g.NameRu == genreOld.NameRu));
                }

                anime.Genres = newAnimeGenres;
            }


            foreach (var anime in animes)
            {
                var newAnimeVoiceOver = new List<Voiceover>();
                foreach (var voiceoverOld in anime.Voiceovers)
                {
                    if (!voiceovers.Select(v => v.NameRu).Contains(voiceoverOld.NameRu))
                    {
                        voiceovers.Add(voiceoverOld);
                    }
                    newAnimeVoiceOver.Add(voiceovers.First(v => v.NameRu == voiceoverOld.NameRu));
                }
                anime.Voiceovers = newAnimeVoiceOver;
            }


            _context.Anime.AddRange(animes);
            _context.SaveChanges();
        }
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using Newtonsoft.Json.Linq;

namespace TestParserAnimeGo
{
    internal class ParserAnimeGo: IDisposable
    {
        public string DefaultUrl { get; set; } = "https://animego.org/anime";
        private readonly IBrowsingContext _context;
        private HttpClient _clientJson;
        private HttpClient _clientImg;
        public ParserAnimeGo()
        {
            _context = BrowsingContext.New(Configuration.Default.WithDefaultLoader());
            _clientImg = new HttpClient();
            _clientJson = new HttpClient();
            _clientJson.DefaultRequestHeaders.Accept.Clear();
            _clientJson.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            _clientJson.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("text/javascript"));
            _clientJson.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue(@"*/*"));

            _clientJson.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("ru"));
            _clientJson.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("en"));
            _clientJson.DefaultRequestHeaders.Add("User-Agent",
                @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/98.0.4758.141 YaBrowser/22.3.3.852 Yowser/2.5 Safari/537.36");
            _clientJson.DefaultRequestHeaders.Add("x-requested-with", "XMLHttpRequest");
        }
        public async Task<List<Anime>> GetFullAnimeFromDefaultUrlAsync()
        {
            return await GetFullAnimeFromUrlAsync(DefaultUrl);
        }
        public async Task<List<Anime>> GetFullAnimeFromUrlAsync(string url)
        {
            if (!url.Contains("?"))
            {
                url+="?";
            }
            List<Anime> animeList = new List<Anime>();
            IDocument? page = null;
            
            int numberPage = 1;
            do
            {
                try
                {
                    page = await GetDocumentFromHtmlAsync(url + $"&page={numberPage}");

                    foreach (var e in page.QuerySelectorAll(".animes-list-item"))
                    {
                        string? href = e.QuerySelector(".h5")?.QuerySelector("a")?.GetAttribute("href")?.Trim();
                        int.TryParse(href?.Split("-")[^1], out int idFromAnimeGoResult);
                        int? idFromAnimeGo = idFromAnimeGoResult == 0 ? null : idFromAnimeGoResult;
                        var nameRu = e.QuerySelector(".h5")?.Text().Trim();
                        var nameEn = e.QuerySelector(".text-gray-dark-6 ")?.Text().Trim();
                        var type = e.QuerySelector("span")?.QuerySelector("a")?.Text().Trim();
                        int.TryParse(e.QuerySelector(".anime-year")?.QuerySelector("a")?.Text().Trim(), out int yearResult);
                        int? year = yearResult == 0 ? null : yearResult;
                        var description = e.QuerySelector(".description")?.Text().Trim();
                        animeList.Add(new Anime
                        {
                            Href = href,
                            IdFromAnimeGo = idFromAnimeGo,
                            NameEn = nameEn,
                            NameRu = nameRu,
                            Description = description,
                            Type = type,
                            Year = year,
                        });
                    }
                    
                }
                finally
                {
                    page?.Close();
                }

                Console.WriteLine(numberPage);
                Console.WriteLine(page.StatusCode);
                numberPage++;
            } while (page.StatusCode != HttpStatusCode.NotFound);

            Console.WriteLine(animeList.Count);
            int i = 0;
            foreach (var anime in animeList)
            {
                Console.WriteLine(++i+" - Аниме");
                await UpdateMainDataAnimeAsync(anime);
                await UpdateShowDataAnimeAsync(anime);
                await UpdateVoiceoverDataAnimeFromFirstEpisodeAsync(anime);
            }


            return animeList;
        }
        
        public async Task UpdateMainDataAnimeAsync(Anime anime)
        {
            if (anime.Href == null)
            {
                return;
            }
            
            using var page =await GetDocumentFromHtmlAsync(anime.Href);
            if (page.StatusCode == HttpStatusCode.OK)
            {
                decimal.TryParse(page.QuerySelector(".rating-value")?.Text().Trim(), out decimal rateResult);
                decimal? rate = rateResult == 0 ? null : rateResult;

                Dictionary<string, string?> dictionary = new Dictionary<string, string?>();
                foreach (var e in page.QuerySelector(".anime-info")!.QuerySelectorAll("dt"))
                {
                    dictionary.Add(e.Text().Trim(), e.NextElementSibling?.Text().Trim());
                }

                dictionary.TryGetValue("Эпизоды", out string? countEpisodeValue);
                int.TryParse(countEpisodeValue?.Split("/")[^1], out int countEpisodeResult);
                int? countEpisode = countEpisodeResult == 0 ? null : countEpisodeResult;
                
                dictionary.TryGetValue("Статус", out string? status);

                dictionary.TryGetValue("Жанр", out string? genresValue);
                List<Genre> genres = genresValue?.Split(",").Select(g => new Genre {NameRu = g.Trim()}).ToList() ??
                                     new List<Genre>();

                dictionary.TryGetValue("Рейтинг MPAA", out string? mpaaRate);
                dictionary.TryGetValue("Студия", out string? studio);
                dictionary.TryGetValue("Длительность", out string? duration);
                dictionary.TryGetValue("Следующий эпизод", out string? nextEpisode);

                dictionary.TryGetValue("Озвучка ", out string? voiceoverValue);
                List<Voiceover> voiceovers = voiceoverValue?.Split(",").Select(v => new Voiceover{ NameRu = v.Trim() }).ToList() ??
                                             new List<Voiceover>();

                anime.Rate = rate;
                anime.CountEpisode = countEpisode;
                anime.Status = status;
                anime.Genres = genres;
                anime.MpaaRate = mpaaRate;
                anime.Studio = studio;
                anime.Duration = duration;
                anime.NextEpisode = nextEpisode;
                anime.Voiceovers = anime.Voiceovers.Union(voiceovers).ToList();

            }
            
        }
        public async Task UpdateShowDataAnimeAsync(Anime anime)
        {
            if (anime.IdFromAnimeGo == null)
            {
                return;
            }
            Dictionary<string, string?> dictionary = new Dictionary<string, string?>();

            using var doc = await GetDocumentFromJsonAsync($"https://animego.org/animelist/{anime.IdFromAnimeGo}/show");
            if (doc.StatusCode == HttpStatusCode.OK)
            {
                foreach (var e in doc.QuerySelectorAll("tr").Skip(1))
                {
                    dictionary.Add(e.QuerySelectorAll("td")[2].Text().Trim(), e.QuerySelectorAll("td")[0].Text().Trim());
                }
            }
            

            dictionary.TryGetValue("Смотрю", out string? watchingValue);
            int.TryParse(watchingValue, out int watchingResult);
            int? watching = watchingResult == 0 ? null : watchingResult;

            dictionary.TryGetValue("Просмотрено", out string? completedValue);
            int.TryParse(completedValue, out int completedResult);
            int? completed = completedResult == 0 ? null : completedResult;

            dictionary.TryGetValue("Брошено", out string? droppedValue);
            int.TryParse(droppedValue, out int droppedResult);
            int? dropped = droppedResult == 0 ? null : droppedResult;

            dictionary.TryGetValue("Отложено", out string? onHoldValue);
            int.TryParse(onHoldValue, out int onHoldResult);
            int? onHold = onHoldResult == 0 ? null : onHoldResult;

            dictionary.TryGetValue("Запланировано", out string? plannedValue);
            int.TryParse(plannedValue, out int plannedResult);
            int? planned = plannedResult == 0 ? null : plannedResult;

            anime.Completed = completed;
            anime.Planned = planned;
            anime.Dropped = dropped;
            anime.OnHold = onHold;
            anime.Watching = watching;

        }
        public async Task UpdateVoiceoverDataAnimeFromFirstEpisodeAsync(Anime anime)
        {
            if (anime.IdFromAnimeGo == null)
            {
                return;
            }
            List<Voiceover> list = new List<Voiceover>();
            using var doc = await GetDocumentFromJsonAsync($"https://animego.org/anime/{anime.IdFromAnimeGo}/player?_allow=true");
            if (doc.StatusCode == HttpStatusCode.OK)
            {
                if (doc.QuerySelector("#video-dubbing") is { } selector)
                {
                    foreach (var e in selector.QuerySelectorAll(".video-player-toggle-item"))
                    {
                        list.Add(new Voiceover { NameRu = e.Text().Trim() });
                    }
                }
            }
            anime.Voiceovers = anime.Voiceovers.Union(list).ToList();


        }
        private async Task<IDocument> GetDocumentFromHtmlAsync(string url)
        {
            await Task.Delay(300);
            IDocument document = await _context.OpenAsync(url);
            while (document.StatusCode == HttpStatusCode.TooManyRequests)
            {
                await Task.Delay(5000);
                document = await _context.OpenAsync(url);
            }
            return document;
        }
        private async Task<IDocument> GetDocumentFromJsonAsync(string url)
        {
            await Task.Delay(300);
            HttpResponseMessage message = await _clientJson.GetAsync(url);
            while (message.StatusCode == HttpStatusCode.TooManyRequests)
            {
                await Task.Delay(5000);
                message = await _clientJson.GetAsync(url);
            }

            var text = await message.Content.ReadAsStringAsync();
            JToken jToken = JToken.Parse(text);
            var html = jToken.Last?.Last?.ToString();
            

            return await _context.OpenAsync(req =>
            {
                req.Content(html);
                req.Status(message.StatusCode);
            });
        }

        public async Task<Stream?> GetStreamFromUrl(string? url)
        {
            if (url == null)
            {
                return null;
            }
            await Task.Delay(300);
            var response = await _clientImg.GetAsync(url);
            while (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                await Task.Delay(5000);
                response = await _clientImg.GetAsync(url);
            }
            return await response.Content.ReadAsStreamAsync();
        }
        public async Task<Stream?> GetSteamPhotoFromAnimeHref(string? href)
        {
            if (href == null)
            {
                return null;
            }
            using var page = await GetDocumentFromHtmlAsync(href);
            if (page.StatusCode == HttpStatusCode.OK)
            {
                var url = page.QuerySelector(".anime-poster")?.QuerySelector("img")?.GetAttribute("src")?.Trim();
                return await GetStreamFromUrl(url);
            }
            else
            {
                return null;
            }
        }

        public void Dispose()
        {
            _context.Dispose();
            _clientJson.Dispose();
            _clientImg.Dispose();
        }
    }
}

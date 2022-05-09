using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html;
using TestParserAnimeGo;

using var parser = new ParserAnimeGo();
using var contex = new ApplicationContext();

var t = DateTime.Now;
List<int> ids = new List<int> {1, 2, 3};
var anime = from id1 in ids
    join
        a in contex.Anime on id1 equals a.AnimeId
    select new {a.NameRu};
foreach (var a in anime)
{
    Console.WriteLine(a.NameRu);
}

Console.WriteLine(DateTime.Now-t);
t = DateTime.Now;
ids = new List<int> { 5, 7, 8 };
var an = from id1 in ids
    join
        a in contex.Anime on id1 equals a.AnimeId
    select new Anime
    {
        IdFromAnimeGo = a.IdFromAnimeGo,
        Voiceovers = a.Voiceovers,
        Href = a.Href,
        Watching = a.Watching,
        NameRu = a.NameRu,
        Completed = a.Completed,
        Status = a.Status,
        NameEn = a.NameEn,
        Type = a.Type,
        Description = a.Description,
        CountEpisode = a.CountEpisode,
        AnimeId = a.AnimeId,
        Rate = a.Rate,
        Genres = a.Genres,
        Planned = a.Planned,
        Dropped = a.Dropped,
        OnHold = a.OnHold,
        Studio = a.Studio,
        NextEpisode = a.NextEpisode,
        Duration = a.Duration,
        MpaaRate = a.MpaaRate,
        Year = a.Year
    };
foreach (var a in an)
{
    Console.WriteLine(a.Description);
}

Console.WriteLine(DateTime.Now - t);




async Task DownloadImageAsync(string directoryPath, string fileName, Uri uri)
{
    using var httpClient = new HttpClient();

    // Get the file extension
    var uriWithoutQuery = uri.GetLeftPart(UriPartial.Path);
    var fileExtension = Path.GetExtension(uriWithoutQuery);

    // Create file path and ensure directory exists
    var path = Path.Combine(directoryPath, $"{fileName}{fileExtension}");
    Directory.CreateDirectory(directoryPath);

    // Download the image and write to the file
    var imageBytes = await httpClient.GetByteArrayAsync(uri);
    await File.WriteAllBytesAsync(path, imageBytes);
}
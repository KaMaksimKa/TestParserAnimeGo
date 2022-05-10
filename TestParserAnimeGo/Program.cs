using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html;
using TestParserAnimeGo;

using var parser = new ParserAnimeGo();
using var writer = new AnimeFromParserToDbWriter();

var animes =  await parser.GetFullAnimeFromDefaultUrlAsync();
writer.AddAnimeRange(animes);


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
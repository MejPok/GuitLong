using GuitLong.Code.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Xml;

namespace GuitLong.Code.Parser
{
    public class WebScraper
    {
        public string savedJson;
        private readonly HttpClient _httpClient;

        public WebScraper()
        {
            _httpClient = new HttpClient();

            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36"
            );
        }

        public async Task<string> GetHtmlAsync(string url)
        {

            if (!Uri.TryCreate(url, UriKind.Absolute, out Uri? uri))
                throw new ArgumentException($"Invalid URL: [{url}]");

            return await _httpClient.GetStringAsync(uri);
        }

        public HtmlDocument ParseHtml(string html)
        {
            var document = new HtmlDocument();
            document.LoadHtml(html);

            return document;
        }

        //https://tabs.ultimate-guitar.com/tab/print?app_utm_campaign=Export2pdfDownload&flats=0&font_size=2&id=1237978&is_ukulele=0&simplified=0&transpose=0
        public async Task<Song> ScrapeSongAsync(string url)
        {
            string html = await GetHtmlAsync(url);

            var document = ParseHtml(html);

            string json = ExtractSongContent(html);

            

            var converter = new ConvertToSongData(json);
            string converted = converter.TryConvert();


            if (converted != null)
            {
                savedJson = converted;
            }

            await File.WriteAllTextAsync("extractedNew.html", html);

            return findBasicInfo(document);
        }

        Song findBasicInfo(HtmlDocument html)
        {
            var song = new Song();

            var jsonScripts = html.DocumentNode.SelectNodes(
                "//script[@type='application/ld+json']"
            );

            if (jsonScripts == null)
                return song;

            foreach (var script in jsonScripts)
            {
                try
                {
                    using var json = JsonDocument.Parse(script.InnerText);

                    var root = json.RootElement;

                    // JSON-LD can sometimes be an array of objects
                    if (root.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var item in root.EnumerateArray())
                        {
                            if (TryExtractMusicComposition(item, song))
                                return song;
                        }
                    }
                    else
                    {
                        if (TryExtractMusicComposition(root, song))
                            return song;
                    }
                }
                catch (JsonException)
                {
                    // Ignore invalid JSON-LD blocks
                }
            }

            return song;
        }


        bool TryExtractMusicComposition(JsonElement root, Song song)
        {
            // Make sure this is a MusicComposition
            if (!root.TryGetProperty("@type", out var typeElement))
                return false;

            bool isMusicComposition = false;

            if (typeElement.ValueKind == JsonValueKind.String)
            {
                isMusicComposition =
                    typeElement.GetString() == "MusicComposition";
            }
            else if (typeElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var type in typeElement.EnumerateArray())
                {
                    if (type.ValueKind == JsonValueKind.String &&
                        type.GetString() == "MusicComposition")
                    {
                        isMusicComposition = true;
                        break;
                    }
                }
            }

            if (!isMusicComposition)
                return false;


            // ==========================
            // TITLE
            // ==========================

            if (root.TryGetProperty("name", out var nameElement) &&
                nameElement.ValueKind == JsonValueKind.String)
            {
                string? name = nameElement.GetString();

                if (!string.IsNullOrWhiteSpace(name))
                {
                    // Example:
                    // The Neighbourhood - Sweater Weather (chords)

                    int dashIndex = name.IndexOf(" - ");

                    if (dashIndex >= 0)
                    {
                        string title = name[(dashIndex + 3)..];

                        // Remove "(chords)", "(tabs)", etc.
                        int bracketIndex = title.IndexOf(" (");

                        if (bracketIndex >= 0)
                            title = title[..bracketIndex];

                        song.Title = title.Trim();
                    }
                    else
                    {
                        song.Title = name.Trim();
                    }
                }
            }


            // ==========================
            // AUTHOR
            // ==========================

            if (root.TryGetProperty("composer", out var composer))
            {
                song.Author = ExtractPersonName(composer);
            }


            // ==========================
            // TEXT
            // ==========================

            if (root.TryGetProperty("text", out var textElement) &&
                textElement.ValueKind == JsonValueKind.String)
            {
                string text = textElement.GetString() ?? "";

                // Difficulty
                var difficultyMatch = Regex.Match(
                    text,
                    @"Difficulty:\s*(.*?)\s+Tuning:",
                    RegexOptions.IgnoreCase
                );

                if (difficultyMatch.Success)
                {
                    song.Difficulty =
                        difficultyMatch.Groups[1].Value.Trim();
                }


                // Tuning
                var tuningMatch = Regex.Match(
                    text,
                    @"Tuning:\s*(.*?)\s+Key:",
                    RegexOptions.IgnoreCase
                );

                if (tuningMatch.Success)
                {
                    song.Tuning =
                        tuningMatch.Groups[1].Value.Trim();
                }


                // Capo
                var capoMatch = Regex.Match(
                    text,
                    @"Capo:\s*(\d+)",
                    RegexOptions.IgnoreCase
                );

                if (capoMatch.Success &&
                    int.TryParse(
                        capoMatch.Groups[1].Value,
                        out int capo))
                {
                    song.Capo = capo;
                }
            }

            return true;
        }


        private string ExtractPersonName(JsonElement composer)
        {
            // composer is an object
            if (composer.ValueKind == JsonValueKind.Object)
            {
                if (composer.TryGetProperty("name", out var name) &&
                    name.ValueKind == JsonValueKind.String)
                {
                    return name.GetString()?.Trim() ?? "";
                }
            }

            // composer is an array
            if (composer.ValueKind == JsonValueKind.Array)
            {
                foreach (var person in composer.EnumerateArray())
                {
                    if (person.ValueKind == JsonValueKind.Object &&
                        person.TryGetProperty("name", out var name) &&
                        name.ValueKind == JsonValueKind.String)
                    {
                        return name.GetString()?.Trim() ?? "";
                    }
                }
            }

            return "";
        }


        private string ExtractSongContent(string html)
        {
            const string marker = "&quot;wiki_tab&quot;:{&quot;content&quot;:&quot;";

            int start = html.IndexOf(marker, StringComparison.OrdinalIgnoreCase);

            if (start == -1)
                throw new Exception("Could not find wiki_tab content.");

            start += marker.Length;

            int end = html.IndexOf("&quot;,&quot;", start, StringComparison.OrdinalIgnoreCase);

            if (end == -1)
                throw new Exception("Could not find end of song content.");

            string encodedContent = html.Substring(start, end - start);

            string content = WebUtility.HtmlDecode(encodedContent);

            content = content.Replace("\\r\\n", Environment.NewLine);

            return content;

            return WebUtility.HtmlDecode(encodedContent);
        }

    }
}

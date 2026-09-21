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
using System.Text.RegularExpressions;
using System.Windows;
using System.Xml;

namespace GuitLong.Code.Parser
{
    public class WebScraper
    {
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

            await File.WriteAllTextAsync("extractedNew.json", converted);

            return findBasicInfo(html);
        }

        Song findBasicInfo(string html)
        {
            string pattern = @"<a[^>]*href=""https://www\.ultimate-guitar\.com/artist/[^""]+""[^>]*>(?<artist>.*?)</a>";

            Match match = Regex.Match(html, pattern, RegexOptions.IgnoreCase);

            string artistName = "Not found";
            if (match.Success)
            {
                artistName = match.Groups["artist"].Value;
            }

            var song = new Song();

            song.Author = artistName;

            return song;
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

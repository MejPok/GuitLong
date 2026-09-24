using GuitLong.Code.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GuitLong.Code.Parser
{
    /// <summary>
    /// Interakční logika pro ParsePage.xaml
    /// </summary>
    public partial class ParsePage : Page
    {
        public Song baseSongData;
        public string jsonData;
        public ParsePage()
        {
            InitializeComponent();
        }

        private async void SubmitUrl(object sender, RoutedEventArgs e)
        {
            try
            {
                var scraper = new WebScraper();
                var song = await scraper.ScrapeSongAsync(urlText.Text);

                songName.Text = "Name: " + song.Title;
                songAuthor.Text = "Author: " + song.Author;

                baseSongData = song;
                jsonData = scraper.savedJson;

            }
            catch (Exception ex)
            {
                statusText.Text = $"Status Error: {ex.Message}";
            } 


        }

        private void ConvertSongData(object sender, RoutedEventArgs e)
        {
            if(baseSongData != null)
            {
                var converter = new ConvertToSongData(jsonData);

                var convertedSong = converter.CreateSongData(jsonData, baseSongData);
                

            }
            else
            {
                statusText.Text = "Status Error: No song data to convert. Please submit a URL first.";
            }
        }
    }
}

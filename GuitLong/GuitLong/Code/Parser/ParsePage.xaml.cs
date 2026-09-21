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
        public ParsePage()
        {
            InitializeComponent();
        }

        private async void SubmitUrl(object sender, RoutedEventArgs e)
        {
            try
            {
                var song = new WebScraper().ScrapeSongAsync(urlText.Text).Result;

                songName.Text = "Name: " + song.Title;
                songAuthor.Text = "Author: " + song.Author;


            }
            catch (Exception ex)
            {
                statusText.Text = $"Status Error: {ex.Message}";
            } 


        }
    }
}

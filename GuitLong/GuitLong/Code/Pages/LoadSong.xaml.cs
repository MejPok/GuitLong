using GuitLong.Code.Models;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace GuitLong.Code.Pages
{
    /// <summary>
    /// Interakční logika pro LoadSong.xaml
    /// </summary>
    public partial class LoadSong : Page
    {
        public LoadSong()
        {
            InitializeComponent();
            LoadSongData();
            App.Current.MainWindow.Width = 800;
            App.Current.MainWindow.Height = 450;
        }

        public sealed class songInfo
        {
            public string Title { get; set; }
            public string Author { get; set; }
            public string Difficulty { get; set; }

            public string SongFilePath { get; set; }
        }

        public void LoadSongData()
        {
            string savedSongsFolder = System.IO.Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "SavedSongs"
                );

            string[] files = Directory.GetFiles(savedSongsFolder);
            List<songInfo> songsToLoad = new List<songInfo>();

            foreach (string file in files)
            {
                string json = File.ReadAllText(file);
                Song loadedSong = System.Text.Json.JsonSerializer.Deserialize<Song>(json);
                songsToLoad.Add(new songInfo
                {
                    Title = loadedSong.Title,
                    Author = loadedSong.Author,
                    Difficulty = loadedSong.Difficulty,
                    SongFilePath = file
                });
            }

            

            songsView.ItemsSource = songsToLoad;
        }

        private void LoadSong_Click(object sender, RoutedEventArgs e)
        {
            if(songsView.SelectedItem != null)
            {
                string selectedSong = ((songInfo)songsView.SelectedItem).SongFilePath;
                string savedSongsFolder = System.IO.Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "SavedSongs"
                );
                string songFilePath = System.IO.Path.Combine(savedSongsFolder, selectedSong);

                if (File.Exists(songFilePath))
                {
                    string json = File.ReadAllText(songFilePath);
                    Song loadedSong = System.Text.Json.JsonSerializer.Deserialize<Song>(json);

                    SongView songPage = new SongView(loadedSong);
                    NavigationService.Navigate(songPage);
                }
                else
                {
                    MessageBox.Show("Selected song file does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a song to load.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void EditSong_Click(object sender, RoutedEventArgs e)
        {
            if (songsView.SelectedItem != null)
            {
                string selectedSong = ((songInfo)songsView.SelectedItem).SongFilePath;
                string savedSongsFolder = System.IO.Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "SavedSongs"
                );
                string songFilePath = System.IO.Path.Combine(savedSongsFolder, selectedSong);

                if (File.Exists(songFilePath))
                {
                    string json = File.ReadAllText(songFilePath);
                    Song loadedSong = System.Text.Json.JsonSerializer.Deserialize<Song>(json);

                    EditSong songPage = new EditSong(loadedSong);
                    NavigationService.Navigate(songPage);
                }
                else
                {
                    MessageBox.Show("Selected song file does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a song to load.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


    }
}

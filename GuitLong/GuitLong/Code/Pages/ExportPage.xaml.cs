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
using Microsoft.Win32;

namespace GuitLong.Code.Pages
{
    /// <summary>
    /// Interakční logika pro ExportPage.xaml
    /// </summary>
    public partial class ExportPage : Page
    {
        public ExportPage()
        {
            InitializeComponent();
            LoadSongData();
        }

        public void LoadSongData()
        {
            string savedSongsFolder = System.IO.Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "SavedSongs"
                );

            string[] files = Directory.GetFiles(savedSongsFolder);
            List<string> songNames = new List<string>();

            foreach (string file in files)
            {
                string fileName = System.IO.Path.GetFileName(file);
                songNames.Add(fileName);
            }

            songsView.ItemsSource = songNames;
        }

        private void ExportTheSong(object sender, RoutedEventArgs e)
        {
            if (songsView.SelectedItem != null)
            {
                string selectedSong = songsView.SelectedItem.ToString();
                string savedSongsFolder = System.IO.Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "SavedSongs"
                );
                string songFilePath = System.IO.Path.Combine(savedSongsFolder, selectedSong);

                if (File.Exists(songFilePath))
                {
                    string json = File.ReadAllText(songFilePath);
                    Song loadedSong = System.Text.Json.JsonSerializer.Deserialize<Song>(json);


                    var dialog = new SaveFileDialog
                    {
                        Title = "Save song",
                        FileName = ".json",
                        DefaultExt = ".json",
                        Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*"
                    };

                    if (dialog.ShowDialog() == true)
                    {
                        File.WriteAllText(dialog.FileName, json);
                    }
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

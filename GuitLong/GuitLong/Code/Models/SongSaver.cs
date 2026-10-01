using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;

namespace GuitLong.Code.Models
{
    internal class SongSaver
    {
        public bool SaveSong(Song saveSong, string additionalName = "")
        {
            // Implement the logic to save the song to a file or database
            // For example, you can serialize the song object to JSON and write it to a file
            try
            {
                string savedSongsFolder = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "SavedSongs"
                );



                string json = System.Text.Json.JsonSerializer.Serialize(saveSong);

                System.IO.File.WriteAllText($"Saved_{saveSong.Title}_{additionalName}.json", json);
                return true;
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log the error)
                MessageBox.Show(@$"Error saving song: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }
}

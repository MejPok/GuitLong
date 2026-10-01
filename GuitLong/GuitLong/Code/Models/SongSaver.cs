using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace GuitLong.Code.Models
{
    internal class SongSaver
    {
        public bool SaveSong(Song saveSong)
        {
            // Implement the logic to save the song to a file or database
            // For example, you can serialize the song object to JSON and write it to a file
            try
            {
                string json = System.Text.Json.JsonSerializer.Serialize(saveSong);
                System.IO.File.WriteAllText($"{saveSong.Title}.json", json);
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

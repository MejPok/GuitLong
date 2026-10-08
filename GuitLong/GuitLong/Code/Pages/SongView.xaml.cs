using GuitLong.Code.Models;
using System;
using System.Collections.Generic;
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
    /// Interakční logika pro SongView.xaml
    /// </summary>
    public partial class SongView : Page
    {
        public bool isPlaying = false;
        public delegate void ButtonHandler(bool state);
        public event ButtonHandler PowerButtonClicked;
        public SongView(Song song)
        {
            InitializeComponent();
            songTitle.Text = song.Title;
        }

        private void PowerButton(object sender, RoutedEventArgs e)
        {
            isPlaying = !isPlaying;

            if(isPlaying)
            {
                ((Button)sender).Content = "1";
                PowerButtonClicked?.Invoke(true);
            }
            else
            {
                ((Button)sender).Content = "0";
                PowerButtonClicked?.Invoke(false);
            }
        }
    }
}

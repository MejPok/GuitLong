using GuitLong.Code.Models;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
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
        public Song song;
        public SongView(Song song)
        {
            InitializeComponent();
            this.song = song;
            songTitle.Text = song.Title;
            GenerateAllRows();
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

        void GenerateAllRows()
        {
            List<TextBlock> rowContents = new List<TextBlock>();

            song.Sections.ForEach(section =>
            {
                GenerateSections(section, ref rowContents);
            });
        }

        void GenerateSections(Models.Section section, ref List<TextBlock> rowContents)
        {

            var row = new TextBlock();
            row.Text = section.Name;
            row.HorizontalAlignment = HorizontalAlignment.Center;
            row.Width = 300;
            row.FontSize = 25; 
            rowContents.Add(row);

            foreach (var rowItem in section.Rows)
            {
                var rowTextBlock = new TextBlock();

                string chordText =  
                String.Concat(Enumerable.Repeat(" ", rowItem.Chords[rowItem.Chords.Count - 1].CharacterPosition));

                foreach (var chord in rowItem.Chords)
                {
                    chordText = chordText.Remove(chord.CharacterPosition, chord.Name.Length).Insert(chord.CharacterPosition, chord.Name);
                }

                rowTextBlock.HorizontalAlignment = HorizontalAlignment.Center;
                rowTextBlock.Width = 300;
                rowTextBlock.FontSize = 20;
                rowContents.Add(rowTextBlock);


                var lyrics = new TextBlock();
                

                lyrics.HorizontalAlignment = HorizontalAlignment.Center;
                lyrics.Width = 300;
                lyrics.FontSize = 20;
                rowContents.Add(lyrics);


            }

        }
    }
}

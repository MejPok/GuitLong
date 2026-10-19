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

            for(int i = 0; i < rowContents.Count; i++)
            {
                var row = new RowDefinition();
                row.MinHeight = 30;
                songGrid.RowDefinitions.Add(row);

                Grid.SetRow(rowContents[i], i);
                songGrid.Children.Add(rowContents[i]);
            }

        }

        void GenerateSections(Models.Section section, ref List<TextBlock> rowContents)
        {
            var blockAlign = HorizontalAlignment.Left;
            var textAlign = TextAlignment.Left;
            var fontFamily = new FontFamily("Courier New");

            var row = new TextBlock();
            row.Text = section.Name;
            row.HorizontalAlignment = blockAlign;
            row.TextAlignment = textAlign;
            row.FontSize = 30; 

            row.FontFamily = fontFamily;
            rowContents.Add(row);

            foreach (var rowItem in section.Rows)
            {
                var rowTextBlock = new TextBlock();
                if (rowItem.Chords.Count == 0)
                {
                    continue;
                }


                rowTextBlock.Text = rowItem.GetChordText();
                rowTextBlock.HorizontalAlignment = blockAlign;
                rowTextBlock.TextAlignment = textAlign;
                rowTextBlock.FontFamily = fontFamily;
                rowTextBlock.FontSize = 20;
                rowContents.Add(rowTextBlock);


                var lyrics = new TextBlock();
                

                lyrics.HorizontalAlignment = blockAlign;
                lyrics.TextAlignment = textAlign;
                lyrics.FontFamily = fontFamily;
                lyrics.FontSize = 20;
                lyrics.Text = rowItem.GetLyricsText();
                rowContents.Add(lyrics);


            }

        }
    }
}

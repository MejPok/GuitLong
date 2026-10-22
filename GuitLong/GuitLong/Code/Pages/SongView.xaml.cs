using GuitLong.Code.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
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

        HorizontalAlignment blockAlign = HorizontalAlignment.Left;
        TextAlignment textAlign = TextAlignment.Left;
        System.Windows.Media.FontFamily fontFamily = new System.Windows.Media.FontFamily("Courier New");
        void GenerateSections(Models.Section section, ref List<TextBlock> rowContents)
        {
            var row = new TextBlock();
            row.Text = section.Name;
            
            ConfigureTextBlock(ref row);

            rowContents.Add(row);

            foreach (var rowItem in section.Rows)
            {
                var rowTextBlock = new TextBlock();
                if (rowItem.Chords.Count == 0)
                {
                    continue;
                }


                rowTextBlock.Text = rowItem.GetChordText();
                ConfigureTextBlock(ref rowTextBlock);
                rowContents.Add(rowTextBlock);


                var lyrics = new TextBlock();
                

                ConfigureTextBlock(ref lyrics);
                lyrics.Text = rowItem.GetLyricsText();
                rowContents.Add(lyrics);


            }

        }

        void ConfigureTextBlock(ref TextBlock textBlock)
        {
            textBlock.HorizontalAlignment = blockAlign;
            textBlock.TextAlignment = textAlign;
            textBlock.FontFamily = fontFamily;
            textBlock.FontSize = 20;
            textBlock.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
        }
    }
}

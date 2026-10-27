using GuitLong.Code.Models;
using GuitLong.Code.Models.Converting;
using GuitLong.Code.Models.Strumming;
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
    /// Interakční logika pro EditSong.xaml
    /// </summary>
    public partial class EditSong : Page
    {
        public Song song;
        public EditSong(Song song)
        {
            this.song = song;
            InitializeComponent();
            PopulateSectionNames();
            PopulateSectionStrum();
            PopulateSectionBPM();
        }

        List<TextBlock> sectionTextBlocks = new List<TextBlock>();
        List<TextBox> sectionStrummingTextBoxes = new List<TextBox>();
        List<TextBox> sectionBPMTextBoxes = new List<TextBox>();


        void PopulateSectionNames()
        {
            var nameGrid = SectionStrum;
            
            foreach (var section in song.Sections)
            {
                sectionTextBlocks.Add(createSectionName(section.Name));
            }


            for (int i = 0; i < sectionTextBlocks.Count; i++)
            {
                var row = new RowDefinition();
                row.MinHeight = 30;

                nameGrid.RowDefinitions.Add(row);

                Grid.SetColumn(sectionTextBlocks[i], 0);
                Grid.SetRow(sectionTextBlocks[i], i + 1);

                nameGrid.Children.Add(sectionTextBlocks[i]);
            }

        }

        public TextBlock createSectionName(string name)
        {
            TextBlock sectionName = new TextBlock();
            sectionName.Text = name;
            sectionName.FontSize = 20;
            sectionName.Height = 30;
            sectionName.FontWeight = FontWeights.Bold;
            sectionName.Margin = new Thickness(0, 10, 0, 10);
            sectionName.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
            return sectionName;
        }

        void PopulateSectionStrum()
        {
            var strumGrid = SectionStrum;

            foreach (var section in song.Sections)
            {
                sectionStrummingTextBoxes.Add(createSectionStrum(section.Name));
            }


            for (int i = 0; i < sectionStrummingTextBoxes.Count; i++)
            {
                Grid.SetColumn(sectionStrummingTextBoxes[i], 1);
                Grid.SetRow(sectionStrummingTextBoxes[i], i + 1);

                strumGrid.Children.Add(sectionStrummingTextBoxes[i]);
            }
        }
        public TextBox createSectionStrum(string name, int width = 30)
        {
            TextBox sectionStrum = new TextBox();
            sectionStrum.Tag = name;
            sectionStrum.FontSize = 10;
            sectionStrum.FontWeight = FontWeights.Bold;
            sectionStrum.Margin = new Thickness(0, 10, 0, 10);
            sectionStrum.Height = 30;

            if(width != 30)
            {
                sectionStrum.Width = width;
                sectionStrum.HorizontalAlignment = HorizontalAlignment.Center;
            }
            

            sectionStrum.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(1, 1, 1));
            return sectionStrum;
        }
        void PopulateSectionBPM()
        {
            var strumGrid = SectionStrum;

            foreach (var section in song.Sections)
            {
                sectionBPMTextBoxes.Add(createSectionStrum("bpm:" + section.Name, 50));
            }


            for (int i = 0; i < sectionBPMTextBoxes.Count; i++)
            {


                Grid.SetRow(sectionBPMTextBoxes[i], i + 1);
                Grid.SetColumn(sectionBPMTextBoxes[i], 2);

                strumGrid.Children.Add(sectionBPMTextBoxes[i]);
            }
        }

        private void SaveStrum(object sender, RoutedEventArgs e)
        {
            for(int i = 0; i < sectionStrummingTextBoxes.Count; i++)
            {
                var sectionName = sectionStrummingTextBoxes[i].Tag.ToString();
                var sectionStrum = sectionStrummingTextBoxes[i].Text;
                var sectionBPM = sectionBPMTextBoxes[i].Text;
                var section = song.Sections.Find(s => s.Name == sectionName);

                if (section != null)
                {
                    if (int.TryParse(sectionBPM, out int bpm))
                    {
                        section.Strumming.BPM = bpm;
                    }

                    section.Strumming.Strums.AddRange(loadStrums(sectionStrum));
                }
            }

            var saver = new SongSaver();
            saver.SaveSong(song);

        }

        List<Strum> loadStrums(string strum)
        {
            var result = new List<Strum>();
            double beat = 0;

            foreach (var s in strum.ToArray())
            {
                if (s == 'D' || s == 'd')
                {
                    result.Add(new Strum() { Direction = StrumDirection.Down, Beat = beat});
                }
                else if (s == 'U' || s == 'u')
                {
                    result.Add(new Strum() { Direction = StrumDirection.Up, Beat = beat });
                } else if(s == '-')
                {
                    result.Add(new Strum() { Direction = StrumDirection.None, Beat = beat });
                }
                beat += 0.5;
            }

            return result;
        }
    }
}

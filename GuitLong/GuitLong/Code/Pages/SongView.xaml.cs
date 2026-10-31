using GuitLong.Code.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
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
using System.Windows.Threading;

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
        public int currentSectionIndex = 0;

        int currentRowIndex;
        public int CurrentRowIndex { get { return currentRowIndex; } set { currentRowIndex = value; AllRowsIndex++; } }
        public int currentChordIndex = 0;
        public int currentStrumIndex = 0;

        public float beatTime = 0.0f;
        private PausableTickTimer _timer;

        public SongView(Song song)
        {
            InitializeComponent();
            App.Current.MainWindow.Width = 1800;
            App.Current.MainWindow.Height = 1200;

            this.song = song;
            songTitle.Text = song.Title;
            songTitle.FontSize = 40;
            GenerateAllRows();
            CreateStrummingHelpUI();


            

            _timer = new PausableTickTimer(TimeSpan.FromMilliseconds(beatTime));
            _timer.Tick += Timer_Tick;
            _timer.Pause();

        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            nextStrum(sender, new RoutedEventArgs());
            ShowLyricsProgress();
            if(selectedSongElementExists(currentSectionIndex, CurrentRowIndex, currentChordIndex))
            {
                stateInfo.Text = $"Section: {currentSectionIndex + 1}/{song.Sections.Count}, Row: {CurrentRowIndex + 1}/{song.Sections[currentSectionIndex].Rows.Count}, Chord: {currentChordIndex + 1}/{song.Sections[currentSectionIndex].Rows[CurrentRowIndex].Chords.Count}, Strum: {currentStrumIndex + 1}/{song.Sections[currentSectionIndex].Strumming.Strums.Count}";
            }
        }

        public List<TextBlock> strumDirUIList = new List<TextBlock>();

        void CreateStrummingHelpUI()
        {
            strumDirUIList.Clear();
            for (int i = 0; i < song.Sections[currentSectionIndex].Strumming.Strums.Count; i++)
            {
                TextBlock strumDirUI = new TextBlock();
                strumDirUI.Tag = i;
                if (song.Sections[currentSectionIndex].Strumming.Strums[i].Direction == Models.Strumming.StrumDirection.Down)
                {
                    strumDirUI.Text = "↓";

                }
                else if (song.Sections[currentSectionIndex].Strumming.Strums[i].Direction == Models.Strumming.StrumDirection.Up)
                {
                    strumDirUI.Text = "↑";
                }
                else if (song.Sections[currentSectionIndex].Strumming.Strums[i].Direction == Models.Strumming.StrumDirection.None)
                {
                    strumDirUI.Text = "-";
                } else
                {
                    strumDirUI.Text = ".";
                }

               

                strumDirUI.Foreground = (i == currentStrumIndex) ? new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 0, 0)) : new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
                strumDirUI.Width = 120;
                strumDirUI.Height = 120;
                strumDirUI.FontSize = (i == currentStrumIndex) ? 90 : 80;
                strumDirUI.HorizontalAlignment = HorizontalAlignment.Left;

                strumDirUI.Margin = new Thickness(i * 50 + 150, 0, 0, 0);

                Grid.SetRow(strumDirUI, 1);
                strumGrid.Children.Add(strumDirUI);

                strumDirUIList.Add(strumDirUI);
            }

            TextBlock strumChordUI = new TextBlock();
            strumChordUI.Name = "strumChordUI";
            strumChordUI.Width = 120;
            strumChordUI.Height = 120;
            strumChordUI.FontSize = 40;


            if (selectedSongElementExists(currentSectionIndex, CurrentRowIndex, currentChordIndex))
            {
                strumChordUI.Text = song.Sections[currentSectionIndex].Rows[CurrentRowIndex].Chords[currentChordIndex].Name;
            }
            strumChordUI.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
                    
            strumChordUI.HorizontalAlignment = HorizontalAlignment.Center;
            strumChordUI.VerticalAlignment = VerticalAlignment.Center;

            Grid.SetRow(strumChordUI, 0);
            strumGrid.Children.Add(strumChordUI);

            

        }

        void ShowLyricsProgress()
        {
            for(int i = 0; i < song.Sections[currentSectionIndex].Rows.Count; i++)
            {
                foreach(List<TextBlock> row in sectionRows)
                {
                    foreach(TextBlock line in row)
                    {
                        if (!selectedSongElementExists(currentSectionIndex, currentRowIndex)) continue;
                        if(line.Text == song.Sections[currentSectionIndex].Rows[currentRowIndex].GetChordText() || line.Text == song.Sections[currentSectionIndex].Rows[currentRowIndex].GetLyricsText())
                        {
                            line.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
                            line.FontSize = 25;
                        } else
                        {
                            line.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
                            line.FontSize = 20;
                        }
                    }
                    
                }
            }
        }

        bool selectedSongElementExists(int section = 0, int row = 0, int chord = 0)
        {
            if(song.Sections.Count > section)
            {
                if (song.Sections[section].Rows.Count > row)
                {
                    if (song.Sections[section].Rows[row].Chords.Count > chord)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void PowerButton(object sender, RoutedEventArgs e)
        {
            isPlaying = !isPlaying;

            if(isPlaying)
            {
                ((Button)sender).Content = "1";
                SetButtonState();
            }
            else
            {
                ((Button)sender).Content = "0";
                SetButtonState();
            }
        }

        void SetButtonState() 
        {
            if (isPlaying)
            {
                PowerButtonClicked?.Invoke(true);
                _timer.Resume();
            }
            else
            {
                PowerButtonClicked?.Invoke(false);
                _timer.Pause();
            }
        }

        public int AllRowsIndex = 0;
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
                Grid.SetColumn(rowContents[i], 0);

                songGrid.Children.Add(rowContents[i]);
                
                
            }

        }

        List<List<TextBlock>> sectionRows = new List<List<TextBlock>>();

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

                TextBlock[] combied = { rowTextBlock, lyrics };

                List<TextBlock> rowCombined = combied.ToList();
                sectionRows.Add(rowCombined);

            }

        }

        void ConfigureTextBlock(ref TextBlock textBlock)
        {
            textBlock.HorizontalAlignment = blockAlign;
            textBlock.TextAlignment = textAlign;
            textBlock.FontFamily = fontFamily;
            textBlock.HorizontalAlignment = HorizontalAlignment.Center;
            textBlock.FontSize = 20;
            textBlock.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
        }

        void ReloadSongStrumming()
        {
            if(currentSectionIndex >= song.Sections.Count)
            {
                return;
            }
            beatTime = (float)60000 / (float)song.Sections[currentSectionIndex].Strumming.BPM / (float)2;

            _timer.SetInterval(TimeSpan.FromMilliseconds(beatTime));
            strumGrid.Children.Clear();
            strumDirUIList.Clear();
            CreateStrummingHelpUI();
        }


        private void nextSectionButton_Click(object sender, RoutedEventArgs e)
        {
            currentSectionIndex++;
            CurrentRowIndex = 0;
            currentChordIndex = 0;
            currentStrumIndex = 0;

            if (currentSectionIndex >= song.Sections.Count)
            {
                isPlaying = !isPlaying;
                SetButtonState();
                MessageBox.Show("song finished");
                return;
            }
            ReloadSongStrumming();
        }

        private void nextRowButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentRowIndex++;
            ShowLyricsProgress();
            currentChordIndex = 0;
            currentStrumIndex = 0;

            if (CurrentRowIndex >= song.Sections[currentSectionIndex].Rows.Count)
            {
                CurrentRowIndex = 0;
                nextSectionButton_Click(sender, e);
            }
            ReloadSongStrumming();
        }

        private void nextChordButton_Click(object sender, RoutedEventArgs e)
        {
            currentChordIndex++;
            currentStrumIndex = 0;

            if(!selectedSongElementExists(currentSectionIndex, CurrentRowIndex, currentChordIndex))
            {
                currentChordIndex = 0;
                nextRowButton_Click(sender, e);
                return;
            }
            
            if (currentChordIndex >= song.Sections[currentSectionIndex].Rows[CurrentRowIndex].Chords.Count)
            {
                currentChordIndex = 0;
                nextRowButton_Click(sender, e);
            }
            ReloadSongStrumming();
        }

        private void nextStrum(object sender, RoutedEventArgs e)
        {
            currentStrumIndex++;
            if (currentStrumIndex >= song.Sections[currentSectionIndex].Strumming.Strums.Count)
            {
                currentStrumIndex = 0;
                nextChordButton_Click(sender, e);
            } 
            ReloadSongStrumming();


        }


    }
}

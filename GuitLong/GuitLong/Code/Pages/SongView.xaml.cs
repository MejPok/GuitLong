using GuitLong.Code.Models;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
        public int currentRowIndex = 0;
        public int currentChordIndex = 0;
        public int currentStrumIndex = 0;

        private TextBlock strumChordUI = new TextBlock();

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




            beatTime = (float)60000 / (float)song.Sections[currentSectionIndex].Strumming.BPM / (float)2;
            if (_timer == null)
            {
                _timer = new PausableTickTimer(TimeSpan.FromMilliseconds(beatTime));
                alreadyInterval = TimeSpan.FromMilliseconds(beatTime);
            }

            SetTimer();
            _timer.Tick += timerTick;
            _timer.Pause();

            GenerateAllRows();
            CreateStrummingHelpUI();

        }

        public List<TextBlock> strumDirUIList = new List<TextBlock>();

        void CreateStrummingHelpUI()
        {
            strumDirUIList.Clear();

            for (int i = 0; i < song.Sections[currentSectionIndex].Strumming.Strums.Count; i++)
            {
                var strumDirUI = new TextBlock();
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
                }
                else
                {
                    strumDirUI.Text = ".";
                }

                strumDirUI.Foreground = Brushes.White;
                strumDirUI.Width = 120;
                strumDirUI.Height = 120;
                strumDirUI.FontSize = 80;
                strumDirUI.HorizontalAlignment = HorizontalAlignment.Left;
                strumDirUI.Margin = new Thickness(i * 50 + 150, 0, 0, 0);

                Grid.SetRow(strumDirUI, 1);
                strumGrid.Children.Add(strumDirUI);
                strumDirUIList.Add(strumDirUI);
            }

            strumChordUI = new TextBlock
            {
                Name = "strumChordUI",
                Width = 120,
                Height = 120,
                FontSize = 40,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            Grid.SetRow(strumChordUI, 0);
            strumGrid.Children.Add(strumChordUI);

            displayedSectionIndex = currentSectionIndex;
            displayedStrumCount = song.Sections[currentSectionIndex].Strumming.Strums.Count;

            UpdateStrummingHelpUI();
        }

        void ShowLyricsProgress()
        {
            if (!selectedSongElementExists(currentSectionIndex, currentRowIndex, currentChordIndex))
            {
                return;
            }

            var row = song.Sections[currentSectionIndex].Rows[currentRowIndex];
            string chordText = row.GetChordText();
            string lyricsText = row.GetLyricsText();

            foreach (TextBlock line in sectionRows)
            {
                bool isCurrentLine =
                    line.Text == chordText ||
                    line.Text == lyricsText;

                line.Foreground = isCurrentLine ? Brushes.Red : Brushes.White;
                line.FontSize = isCurrentLine ? 25 : 20;
            }
        }

        bool selectedSongElementExists(int section = 0, int row = 0, int chord = 0)
        {
            if (song.Sections.Count > section)
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

        void PowerButton(object sender, RoutedEventArgs e)
        {
            isPlaying = !isPlaying;

            if (isPlaying)
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

        void GenerateAllRows()
        {
            List<TextBlock> rowContents = new List<TextBlock>();

            song.Sections.ForEach(section =>
            {
                GenerateSections(section, ref rowContents);
            });

            for (int i = 0; i < rowContents.Count; i++)
            {
                var row = new RowDefinition();
                row.MinHeight = 30;
                songGrid.RowDefinitions.Add(row);

                Grid.SetRow(rowContents[i], i);
                Grid.SetColumn(rowContents[i], 0);

                songGrid.Children.Add(rowContents[i]);


            }

        }

        List<TextBlock> sectionRows = new List<TextBlock>();

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

                sectionRows.Add(rowTextBlock);
                sectionRows.Add(lyrics);
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

        private int displayedSectionIndex = -1;
        private int displayedStrumCount = -1;

        void ReloadSongStrumming()
        {
            if (currentSectionIndex >= song.Sections.Count)
            {
                return;
            }

            int strumCount = song.Sections[currentSectionIndex].Strumming.Strums.Count;

            // Rebuild only when the section or its pattern length changes.
            if (displayedSectionIndex != currentSectionIndex ||
                displayedStrumCount != strumCount)
            {
                strumGrid.Children.Clear();
                strumDirUIList.Clear();
                CreateStrummingHelpUI();
            }
            else
            {
                UpdateStrummingHelpUI();
            }
        }

        void UpdateStrummingHelpUI()
        {
            for (int i = 0; i < strumDirUIList.Count; i++)
            {
                bool isCurrentStrum = i == currentStrumIndex;

                strumDirUIList[i].Foreground =
                    isCurrentStrum ? Brushes.Red : Brushes.White;

                strumDirUIList[i].FontSize =
                    isCurrentStrum ? 90 : 80;
            }

            if (selectedSongElementExists(
                currentSectionIndex,
                currentRowIndex,
                currentChordIndex))
            {
                strumChordUI.Text =
                    song.Sections[currentSectionIndex]
                        .Rows[currentRowIndex]
                        .Chords[currentChordIndex]
                        .Name;
            }
            else
            {
                strumChordUI.Text = "";
            }
        }


        TimeSpan alreadyInterval;
        void SetTimer()
        {
            if (!selectedSongElementExists(currentSectionIndex)) return;

            beatTime = (float)60000 / (float)song.Sections[currentSectionIndex].Strumming.BPM / (float)2;
            if (_timer == null)
            {
                _timer = new PausableTickTimer(TimeSpan.FromMilliseconds(beatTime));
                alreadyInterval = TimeSpan.FromMilliseconds(beatTime);
            }

            if (TimeSpan.FromMilliseconds(beatTime).Duration() != alreadyInterval.Duration())
            {
                _timer.SetInterval(TimeSpan.FromMilliseconds(beatTime));
                alreadyInterval = TimeSpan.FromMilliseconds(beatTime);
            }
        }

        Stopwatch clock = Stopwatch.StartNew();
        public long lastTick = 0;

        void timerTick(object? sender, EventArgs e)
        {
            currentStrumIndex++;

            controlPhaseCriteria();
            ReloadSongStrumming();
            ShowLyricsProgress();

            stateInfo.Text = $"Section: {currentSectionIndex + 1}/{song.Sections.Count}, Row: {currentRowIndex + 1}/{song.Sections[currentSectionIndex].Rows.Count}, Chord: {currentChordIndex + 1}/{song.Sections[currentSectionIndex].Rows[currentRowIndex].Chords.Count}, Strum: {currentStrumIndex + 1}/{song.Sections[currentSectionIndex].Strumming.Strums.Count}";
            long now = clock.ElapsedMilliseconds;

            if (lastTick != 0)
            {
                dilationInfo.Text = $"{now - lastTick - beatTime:0} ms";
            }

            lastTick = now;


        }

        void controlPhaseCriteria()
        {
            /*
             jsem na konci strumu
                 prejdu na dalsi chord
             jsem na konci chordu
                     prejdu na dalsi row
             jsem na konci row
                     prejdu na dalsi section
             jsem na konci section
                     jdu na dalsi section
                                jestli jsem na konci vsech section
                                    vypnu prehravani
                                opakuju
             
             */

            if (song.Sections.Count > currentSectionIndex)
            {
                if (currentStrumIndex >= song.Sections[currentSectionIndex].Strumming.Strums.Count)
                {
                    currentStrumIndex = 0;
                    currentChordIndex++;
                }

                if (song.Sections[currentSectionIndex].Rows.Count > currentRowIndex)
                {

                    if (currentChordIndex >= song.Sections[currentSectionIndex].Rows[currentRowIndex].Chords.Count)
                    {
                        currentChordIndex = 0;
                        currentStrumIndex = 0;

                        currentRowIndex++;
                    }

                }

                if (currentRowIndex >= song.Sections[currentSectionIndex].Rows.Count)
                {
                    currentRowIndex = 0;
                    currentChordIndex = 0;
                    currentStrumIndex = 0;

                    currentSectionIndex++;
                }

            }
            else
            {
                Debug.WriteLine("section index out of range");
            }

            if (currentSectionIndex >= song.Sections.Count)
            {
                isPlaying = !isPlaying;
                SetButtonState();
                MessageBox.Show("song finished");
                NavigationService.Navigate(new LoadSong());
                return;
            }
            else
            {
                SetTimer();
            }

        }

        private void nextSectionButton_Click(object sender, RoutedEventArgs e)
        {

            currentSectionIndex++;
            currentRowIndex = 0;
            currentChordIndex = 0;
            currentStrumIndex = 0;

        }
        private void nextRowButton_Click(object sender, RoutedEventArgs e)
        {
            currentRowIndex++;

            currentChordIndex = 0;
            currentStrumIndex = 0;

        }
        private void nextChordButton_Click(object sender, RoutedEventArgs e)
        {
            currentChordIndex++;
            currentStrumIndex = 0;

        }
        private void nextStrum(object sender, RoutedEventArgs e)
        {
            currentStrumIndex++;
        }


    }
}

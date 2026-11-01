using GuitLong.Code.Pages;
using GuitLong.Code.Parser;
using System.Windows;

namespace GuitLong
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Instance { get; private set; }
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            Instance = this;
            CreateSongFolder();


            App.Current.MainWindow.Width = 800;
            App.Current.MainWindow.Height = 450;

            MessageBox.Show("Welcome to GuitLong! To get started, click the 'Import New Song' button and enter a URL from ultimate-guitar.com. The app will scrape the song data and convert it into a format that can be used for practice. You can then edit the song, play it, or export it for later use.");
        }

        void CreateSongFolder()
        {
            string songFolderPath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "SavedSongs");
            if (!System.IO.Directory.Exists(songFolderPath))
            {
                System.IO.Directory.CreateDirectory(songFolderPath);
            }
        }

        private async void ImportNewSong(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ParsePage());
        }

        private async void ExportSong(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ExportPage());
        }

        private void LoadSongs(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new LoadSong());
        }
    }
}
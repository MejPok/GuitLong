using GuitLong.Code.Pages;
using GuitLong.Code.Parser;
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

        private void LoadSongs(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new LoadSong());

        }
    }
}
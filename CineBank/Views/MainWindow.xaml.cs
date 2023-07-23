using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CineBank
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // generate demo data
            List<Movie> movies = new List<Movie>();
            movies.Add(new Movie() { 
                Title = "Test 1",
                Description = "saohf sdhUIBfuzsdgbfuzsdfgb udfgusgfs fas",
                CoverPath = @"E:\Bilder\IFS-Ahorn.PNG",
                Genre = "Horror",
                Duration = "2:34:56",
                Type = "Movie"
            });
            movies.Add(new Movie()
            {
                Title = "Test 2",
                Description = "asdh iAhsdui sdaisgOIG SAuifi agsfgis fg",
                CoverPath = @"E:\Bilder\20190927_135003000_iOS.jpg",
                Genre = "Dokumentation",
                Duration = "21",
                Type = "Series"
            });
            lbMovies.ItemsSource = movies;
        }

        private void lbMovies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // check if selection is valid
            if (lbMovies.SelectedItem == null)
                return;
            // extract selected object
            Movie mov = lbMovies.SelectedItem as Movie;
            // display object in priview
            imgCoverPath.Source = new BitmapImage(new Uri(mov.CoverPath));
            tbTitle.Text = mov.Title;
            tbDescription.Text = mov.Description;
            tbType.Text = mov.Type;
            tbGenre.Text = mov.Genre;
            tbDuration.Text = mov.Duration;
        }

        #region MenuItem
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
        #endregion
    }
}

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

            // load config (checksum and xml-object)
            // checksum
            // xml-object

            // precheck: configfile (if HKLM:\SOFTWARE\CineBank\ConfigCksm exists and not null or whitespace and/or xml/config/validateChecksums is true)
            // checksum of mathes
            // checksum of scripts matches

            // precheck: database (xml/config/dbPath or 'commandline arg 0' existst)
            // load db or open dialog to open/ create one
            
            // precheck: database current version
            // if not open dialog to update [this function must be implemented, when scheme will change in the future]

            // get data
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
            tbType.Text = Enum.GetName(typeof(Movie.MovieType), mov.Type);
            tbGenre.Text = mov.Genre;
            tbDuration.Text = mov.Duration;
            tbRelease.Text = mov.Released;
            tbCast.Text = mov.Cast;
            tbDirector.Text = mov.Director;
            tbScore.Text = mov.Score;
            tbLanguages.Text = mov.Languages;
            tbSubtitles.Text = mov.Subtitles;
            tbAudioDesc.Text = mov.AudioDescription;
            tbResolution.Text = mov.MaxResolution;
            tbFormat.Text = mov.Format;
        }

        #region MenuItem
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
        #endregion
    }
}

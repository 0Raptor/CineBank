using CineBank.Classes;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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
using System.Xml;

namespace CineBank
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Dictionary ciontaining names and filepaths to the scripts used to play files
        /// </summary>
        private Dictionary<string, string> HelperScripts = new Dictionary<string, string>()
        {
            { "Video1", "Scripts/VideoPlayer1.ps1" },
            { "Video2", "Scripts/VideoPlayer2.ps1" },
            { "DVDPlayer", "Scripts/DVDPlayer.ps1" },
            { "BRPlayer", "Scripts/BRPlayer.ps1" },
            { "AudioPlayer", "Scripts/AudioPlayer.ps1" },
            { "Setup", "UpdateConfiguration.ps1" }
        };

        string workingDir = "";
        string baseDir = "";
        string baseDirSource = "";
        string apiKey = "";
        Database db = new Database();
        List<Movie> movies = new List<Movie>();

        /// <summary>
        /// Load UI and initialise application (with prechecks). Must only be run ONCE!
        /// Commandline args: 0 application name, 1 (optional) path to db, 2 (optional) basedir for relaive paths in db
        /// </summary>
        public MainWindow()
        {
            string[] args = Environment.GetCommandLineArgs(); // get args from connadline
            workingDir = (System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).Replace("file:\\", "") + "\\"); //.Replace("\\", "/");

            InitializeComponent();

            // load config (checksum and xml-object)
            if (!File.Exists(workingDir + "config.xml"))
            {
                MessageBox.Show("No configuraton file present.\r\nERROR: File '" + workingDir + "config.xml' does not exist.\r\nPlease check README!", "No configuration present", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(1);
            }
            // checksum
            string confCksm = GetFileHash(workingDir + "config.xml");
            string registryCksm = "";
            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey("Software\\CineBank"))
                {
                    if (key != null)
                    {
                        Object o = key.GetValue("ConfigCksm");
                        if (o != null)
                        {
                            registryCksm = o as String;  //"as" because it's REG_SZ...otherwise ToString() might be safe(r)
                        }
                    }
                }
            } catch { }
            // xml-object
            XmlDocument doc = new XmlDocument();
            doc.Load(workingDir + "config.xml");
            // validate checksums?
            bool validateCksms = Convert.ToBoolean(doc.SelectSingleNode("xml/config/validateChecksums").InnerText);

            // precheck: configfile (if HKLM:\SOFTWARE\CineBank\ConfigCksm exists and not null or whitespace and/or xml/config/validateChecksums is true)
            if (!String.IsNullOrWhiteSpace(registryCksm) || validateCksms)
            {
                // checksum of config
                if (confCksm != registryCksm)
                {
                    MessageBoxResult res = MessageBox.Show("The checksum of your current configuration does not match the checksum stored in the registry.\r\nThis might be caused by a unauthorized config change.\r\nFor additional information to resolve this waring, check the README.\r\nProceede anyway?", "Checksum missmatch", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (res == MessageBoxResult.No)
                        Environment.Exit(2);
                }

                // checksum of scripts matches
                bool failed = false;
                string msg = "The checksum(s) of your file-launch-script(s) does not match the registred values.\r\nThis might be caused by a unauthorized malicious change!\r\n\r\nConflicting files:\r\n";
                string msgTail = "\r\nFor additional information to resolve this waring, check the README.\r\nProceede anyway?";
                foreach (var script in HelperScripts)
                {
                    string expectedCksm = "";
                    try
                    {
                        if (script.Key == "Setup") continue;
                        expectedCksm = doc.SelectSingleNode("xml/checksums/" + script.Key).InnerText;
                    }
                    catch
                    {
                        MessageBoxResult res = MessageBox.Show("Your configuration file does not contain expected information. The checksum validation will be skipped. Please check README to generate a new file.\r\nIgnore error and proceed?", "Invalid config file", MessageBoxButton.YesNo, MessageBoxImage.Error);
                        if (res == MessageBoxResult.No)
                            Environment.Exit(1);
                        failed = false;
                        break;
                    }
                    string cksm = GetFileHash(workingDir + script.Value);

                    if (expectedCksm != cksm)
                    {
                        failed = true;
                        msg += " - " + script.Key + " (" + workingDir + script.Value + ")\r\n";
                    }
                }
                if (failed)
                {
                    MessageBoxResult res = MessageBox.Show(msg + msgTail, "Checksum missmatch", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (res == MessageBoxResult.No)
                        Environment.Exit(2);
                }
            }

            // precheck: database (xml/config/dbPath or 'commandline arg 1' exist)
            string dbPath = "";
            if (args.Length > 2 && !String.IsNullOrWhiteSpace(args[1])) // use db from commandline args
            {
                dbPath = args[1];            
            }
            else // use db from config
            {
                try 
                {
                    dbPath = doc.SelectSingleNode("xml/config/dbPath").InnerText;
                } catch 
                {
                    MessageBox.Show("Your configuration file does not contain required information. Please check README to generate a new file.", "Invalid config file", MessageBoxButton.OK, MessageBoxImage.Error);
                    Environment.Exit(1);
                }
            }
            // precheck: basedir specified via apllication
            if (args.Length > 3 && !String.IsNullOrWhiteSpace(args[2])) // use baseDir from commandline args
            {
                baseDir = args[2];
                baseDirSource = "Arguments";
            }
            else // use db from config
            {
                try
                {
                    baseDir = doc.SelectSingleNode("xml/config/baseDir").InnerText;
                    baseDirSource = "config.xml";
                }
                catch
                {
                    MessageBox.Show("Your configuration file does not contain required information. Please check README to generate a new file.", "Invalid config file", MessageBoxButton.OK, MessageBoxImage.Error);
                    Environment.Exit(1);
                }
            }
            if (!String.IsNullOrWhiteSpace(baseDir) && !Directory.Exists(baseDir))
            {
                MessageBox.Show("The supplied baseDir '" + baseDir + "' does not exist!", "Invalid parameter", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(3);
            }

            // create db if not exisitng
            if (!File.Exists(dbPath))
            {
                MessageBoxResult res = MessageBox.Show("No database found at '" + dbPath + "'.\r\nCreate new database?", "No database", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (res == MessageBoxResult.No)
                    return;

                DatabaseConfig? conf = null;
                if (!String.IsNullOrWhiteSpace(baseDir))
                    conf = new DatabaseConfig(baseDir);

                // get db settings for new db from user
                CreateDbDialog dlg = new CreateDbDialog(dbPath, baseDir);
                if (dlg.ShowDialog() == true)
                {
                    dbPath = dlg.Answer[0];
                    baseDir = dlg.Answer[1];
                    if (!String.IsNullOrWhiteSpace(baseDir))
                        conf = new DatabaseConfig(baseDir);

                    // create new db
                    try
                    {
                        Database.Init(dbPath, conf);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Failed to create new database at '" + dbPath + "'!\r\n" + ex.Message, "Failed to create new database", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // update config
                    res = MessageBox.Show("Database successfully created.\r\nAdd database path to config (this might fail due to lack of permission and need a checksum refresh - check README)?", "New database", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (res == MessageBoxResult.Yes)
                    {
                        try
                        {
                            doc.SelectSingleNode("xml/config/dbPath").InnerText = dbPath;
                            doc.SelectSingleNode("xml/config/baseDir").InnerText = baseDir;
                            doc.Save(workingDir + "config.xml");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Configuration updated failed: " + ex.Message + "\r\nPlease add dbPath amd baseDir via the PowerShell-script.", "Config update failed", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("No database found.\r\nYou can create one via the tooltip.", "No database", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                
            }
            // connect to db
            if (String.IsNullOrWhiteSpace(baseDir)) { baseDir = ""; baseDirSource = "Database"; }
            db = new Database(dbPath, baseDir);

            // precheck: basedir not specified via apllication nor db but required
            if (String.IsNullOrWhiteSpace(db.Config.BaseDir) && String.IsNullOrWhiteSpace(baseDir) && !db.CheckFilesHaveAbsolutePath())
            {
                MessageBox.Show("There is no baseDir specified in the database, config and arguments but some files do not have absolute paths.\r\nUnexpected behavior may occur.", "Found incomplete paths", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            // precheck: database current version
            // if not open dialog to update [this function must be implemented, when scheme will change in the future]

            // get TMDB-API-Key
            try { apiKey = doc.SelectSingleNode("xml/config/tmdbApiKey").InnerText; } catch { }

            // get data
            movies = Movie.GetMovies(db);
            lbMovies.ItemsSource = movies;
        }

        // filter for movies
        private void FilterMovies()
        {
            if (!String.IsNullOrWhiteSpace(tbSearch.Text)) // search string is supplied
            {
                movies = Movie.GetMovies(db, tbSearch.Text);
                lbMovies.ItemsSource = movies;
            }
            else
            {
                movies = Movie.GetMovies(db);
                lbMovies.ItemsSource = movies;
            }
        }

        // user selected a movie from list --> display
        private void lbMovies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // check if selection is valid
            if (lbMovies.SelectedItem == null)
                return;
            // extract selected object
            Movie mov = lbMovies.SelectedItem as Movie;
            // display object in preview
            try
            {
                // list data
                if (!String.IsNullOrWhiteSpace(mov.CoverPath))
                    imgCoverPath.Source = new BitmapImage(new Uri(mov.CoverPath));
                else 
                    imgCoverPath.Source = null;
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
                tbAge.Text = mov.Age;
                tbNotes.Text = mov.Notes;

                // list playable files
                cbPlay.Items.Clear();
                foreach (var item in mov.Files)
                {
                    if (item.Open == LinkedFile.OpenWith.None)
                        continue;
                    cbPlay.Items.Add(item);
                }
            } catch
            {
                imgCoverPath.Source = null;
                tbTitle.Text = "";
                tbDescription.Text = "";
                tbType.Text = "";
                tbGenre.Text = "";
                tbDuration.Text = "";
                tbRelease.Text = "";
                tbCast.Text = "";
                tbDirector.Text = "";
                tbScore.Text = "";
                tbLanguages.Text = "";
                tbSubtitles.Text = "";
                tbAudioDesc.Text = "";
                tbResolution.Text = "";
                tbFormat.Text = "";
                tbAge.Text = "";

                tbNotes.Text = "WARNING: Failed to load data.";

                cbPlay.Items.Clear();
            }
        }

        // play video
        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            // check if selection is valid
            if (cbPlay.SelectedItem == null)
                return;
            // extract selected object
            LinkedFile lf = cbPlay.SelectedItem as LinkedFile;

            // get information and prepare them for script
            string path = lf.Path;
            string dir = System.IO.Path.GetDirectoryName(path);

            // select method to open file
            string script = workingDir;
            switch (lf.Open)
            {
                case LinkedFile.OpenWith.Video1:
                    script += HelperScripts["Video1"];
                    break;
                case LinkedFile.OpenWith.Video2:
                    script += HelperScripts["Video2"];
                    break;
                case LinkedFile.OpenWith.DVDPlayer:
                    script += HelperScripts["DVDPlayer"];
                    break;
                case LinkedFile.OpenWith.BRPlayer:
                    script += HelperScripts["BRPlayer"];
                    break;
                case LinkedFile.OpenWith.AudioPlayer:
                    script += HelperScripts["AudioPlayer"];
                    break;
                case LinkedFile.OpenWith.Undefined: // let os decide how to open this software
                    Process.Start(path);
                    return; // end function
                default:
                    MessageBox.Show("Unable to detect the method to open this file.", "Failed to open", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    break;
            }

            // open file using the selected script
            string scriptArguments = "-File \"" + script + "\" -path \"" + path + "\" -dir \"" + dir + "\""; // -ExecutionPolicy Bypass
            Process.Start("powershell.exe", scriptArguments);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Key == Key.F5) { MenuItem_Reload_Click(null, null); }
            if (e.Key == Key.F1) { MenuItem_Add_Click(null, null); }
            if (e.Key == Key.F2) { MenuItem_Edit_Click(null, null); }
            if (e.Key == Key.F6) { MenuItem_Settings_Click(null, null); }
            if (e.Key == Key.Delete) { MenuItem_Delete_Click(null, null); }
        }

        #region FileHelper
        /// <summary>
        /// Genereates the SHA256-Hash of a file
        /// </summary>
        /// <param name="path">Path to the file to hash</param>
        /// <returns>SHA256-Hash of the file as Base64 string</returns>
        string GetFileHash(string path)
        {
            using (SHA256 sha = SHA256.Create())
            {
                // get filestream from path
                using (FileStream fileStream = File.OpenRead(path))
                {
                    try
                    {
                        // maske sure that filestream is at start
                        fileStream.Position = 0;
                        // Compute the hash of the fileStream.
                        byte[] hashValue = sha.ComputeHash(fileStream);
                        // return hash value of the file
                        return Convert.ToBase64String(hashValue);
                    }
                    catch (IOException e)
                    {
                        Console.WriteLine($"I/O Exception: {e.Message}");
                    }
                    catch (UnauthorizedAccessException e)
                    {
                        Console.WriteLine($"Access Exception: {e.Message}");
                    }
                }
            }
            return "ERROR";
        }
        #endregion

        #region MenuItem
        private void MenuItem_NotImplemented_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Sorry, this feature is not implemented yet.", "Feature not implemented");
        }

        private void MenuItem_Exit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void MenuItem_Delete_Click(object sender, RoutedEventArgs e)
        {
            // check if selection is valid
            if (lbMovies.SelectedItem == null)
                return;
            // extract selected object
            Movie mov = lbMovies.SelectedItem as Movie;
            // delete object
            movies.Remove(mov);
            lbMovies.Items.Refresh();
            mov.Delete(db);
        }

        private void MenuItem_Reload_Click(object sender, RoutedEventArgs e)
        {
            movies = Movie.GetMovies(db);
            lbMovies.ItemsSource = movies;
        }

        private void MenuItem_Settings_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow(db, baseDirSource);
            settingsWindow.Show();
        }

        // display windows to add a new movie. pass null to constructor to ensure a new movie-object will be created.
        private void MenuItem_Add_Click(object sender, RoutedEventArgs e)
        {
            Movie? m = null;
            EntryWindow w = new EntryWindow(ref m, db, apiKey);
            w.Show();
        }

        // display windows to edit a new movie. pass object as reference to apply changes in main window instantanious.
        private void MenuItem_Edit_Click(object sender, RoutedEventArgs e)
        {
            // check if selection is valid
            if (lbMovies.SelectedItem == null)
                return;
            // extract selected object
            Movie mov = lbMovies.SelectedItem as Movie;

            EntryWindow w = new EntryWindow(ref mov, db, apiKey);
            w.Show();
        }

        private void MenuItem_Help_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(@"https://github.com/0Raptor/CineBank");
        }

        private void EditHelperScripts(string path)
        {
            Process.Start(@"C:\WINDOWS\system32\WindowsPowerShell\v1.0\powershell_ise.exe", workingDir + path);
        }

        private void MenuItem_EditV1_Click(object sender, RoutedEventArgs e)
        {
            EditHelperScripts(HelperScripts["Video1"]);
        }

        private void MenuItem_EditV2_Click(object sender, RoutedEventArgs e)
        {
            EditHelperScripts(HelperScripts["Video2"]);
        }

        private void MenuItem_EditDVD_Click(object sender, RoutedEventArgs e)
        {
            EditHelperScripts(HelperScripts["DVDPlayer"]);
        }

        private void MenuItem_EditBR_Click(object sender, RoutedEventArgs e)
        {
            EditHelperScripts(HelperScripts["BRPlayer"]);
        }

        private void MenuItem_EditA_Click(object sender, RoutedEventArgs e)
        {
            EditHelperScripts(HelperScripts["AudioPlayer"]);
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            FilterMovies();
        }
        #endregion

        private void tbSearch_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                FilterMovies();
        }
    }
}

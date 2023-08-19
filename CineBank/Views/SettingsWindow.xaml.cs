using CineBank.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Shapes;

namespace CineBank
{
    /// <summary>
    /// Interaktionslogik für SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        Database db;

        public SettingsWindow(Database _db, string baseDirSource = "?")
        {
            db = _db;
            InitializeComponent();
            
            GetConfig();
            lblBaseDirSource.Text = baseDirSource; // get basedirsource
            if (baseDirSource == "Database" && String.IsNullOrWhiteSpace(db.Config.BaseDir)) lblBaseDirSource.Text = "Not set";
        }

        /// <summary>
        /// Display the values inside of the DatabaseConfig in the UI
        /// </summary>
        private void GetConfig()
        {
            tbBaseDir.Text = db.Config.BaseDir;
            lblDbVersion.Text = db.Config.Version;

            cbLanguage.SelectedIndex = db.Config.ApiLanguage;
            tbCastToSave.Text = db.Config.CastPerMovie.ToString();
            cbCharNames.IsChecked = db.Config.IncludeCharacterWithCast;
            cbDownloadPoster.IsChecked = db.Config.DownloadPosterFromAPI;
        }

        /// <summary>
        /// Save the data in the UI back to the db object and write it to db
        /// </summary>
        private void SetConfig()
        {
            try
            {
                db.Config.BaseDir = tbBaseDir.Text;
                db.Config.Version = lblDbVersion.Text;

                db.Config.ApiLanguage = cbLanguage.SelectedIndex;
                db.Config.CastPerMovie = Convert.ToInt32(tbCastToSave.Text);
                db.Config.IncludeCharacterWithCast = (bool)cbCharNames.IsChecked;
                db.Config.DownloadPosterFromAPI = (bool)cbDownloadPoster.IsChecked;

                db.Config.UpdateInDB(db);
            } catch (Exception ex)
            {
                MessageBox.Show("Failed to update settings. Please validate your inputs.\r\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #region Buttons
        private void btnSelectBaseDir_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog folderBrowserDialog = new VistaFolderBrowserDialog();
            folderBrowserDialog.Multiselect = false;
            if (folderBrowserDialog.ShowDialog() == true)
            {
                tbBaseDir.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void btnOpenConf_Click(object sender, RoutedEventArgs e)
        {
            string workingDir = (System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).Replace("file:\\", "") + "\\");
            Process.Start("powershell.exe", workingDir + "UpdateConfiguration.ps1");
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SetConfig();
            MessageBox.Show("Successfully updated configuration. A restart of the application may be required to apply all changes.", "Successfully saved", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnAbort_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // close window
        }
        #endregion
    }
}

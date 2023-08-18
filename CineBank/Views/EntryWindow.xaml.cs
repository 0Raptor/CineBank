﻿using CineBank.Classes;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaktionslogik für EntryWindow.xaml
    /// </summary>
    public partial class EntryWindow : Window
    {
        Movie movOld;
        Movie mov;
        Database db;
        List<LinkedFile> files;
        List<LinkedFile> files2remove;

        /// <summary>
        /// Open new windows to edit an exising element or create a new one. Use .Show() to open.
        /// </summary>
        /// <param name="editElement">Referene to the element to edit. Pass Null-Reference to create a new object.</param>
        /// <param name="_db">Database to store information of element into</param>
        public EntryWindow(ref Movie? editElement, Database _db)
        {
            if (editElement != null)
            {
                // create deep copy of object to check for changes
                movOld = editElement.DeepCopy();
                // set reference to local var
                mov = editElement;
            }
            else
            {
                // prepare to create a new movie
                mov = new Movie();
                movOld = mov.DeepCopy();
            }

            // display current object information
            //this.DataContext = mov;
            db = _db;

            InitializeComponent();

            // set global datacontext
            this.DataContext = mov;

            // set radiogroup
            if (mov.Type == Movie.MovieType.Movie)
                cbTypeMovie.IsChecked = true;
            else
                cbTypeSeries.IsChecked = true;

            // set context to edit files
            files = mov.Files.ToList();
            lbFiles.ItemsSource = files;

            // prepare list to track files that should be removed from db
            files2remove = new List<LinkedFile>();
        }

        /// <summary>
        /// Sets the values to the current editid object that cannot be set automatically
        /// </summary>
        private void UpdateMov()
        {
            // set type manually
            if ((bool)cbTypeMovie.IsChecked)
                mov.Type = Movie.MovieType.Movie;
            else
                mov.Type = Movie.MovieType.Series;

            // set files manually
            mov.Files = files.ToArray();
        }

        #region Events
        // check changes when closing the window
        protected override void OnClosing(CancelEventArgs e)
        {
            UpdateMov();

            if (mov != movOld) // check if data has been modified
            {
                var res = MessageBox.Show("There are unsaved changes for this entry.\r\nDiscard changes and close window anyway?", "Discard Changes?", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (res == MessageBoxResult.No) 
                {
                    e.Cancel = true; // abort closing event --> window will NOT be closed
                    return;
                }
            }

            mov.SetMembers(movOld); // discard changes
            e.Cancel = false; // do not abort closing event --> window will be close as expected
        }
        #endregion

        #region Buttons
        // search for provided title in IMDB
        private void btnFetchInformation_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnAddContent_Click(object sender, RoutedEventArgs e)
        {
            AddFileDialog dlg = new AddFileDialog(db.Config.BaseDir);
            if (dlg.ShowDialog() == true) // open dialog to select new files
            {
                LinkedFile[] newFiles = dlg.Answer;
                if (newFiles != null && newFiles.Length > 0) // add them to the local list
                    files.AddRange(newFiles);
                lbFiles.Items.Refresh(); // refresh items shown in listbox
            }
        }

        private void btnRemoveContent_Click(object sender, RoutedEventArgs e)
        {
            if (lbFiles.SelectedItem == null) // validate selection
                return;

            LinkedFile lf = lbFiles.SelectedItem as LinkedFile;
            files.Remove(lf);
            files2remove.Add(lf);
            lbFiles.Items.Refresh();
        }

        private void btnSelectCover_Click(object sender, RoutedEventArgs e)
        {
            // supported media types: https://learn.microsoft.com/en-us/dotnet/desktop/wpf/graphics-multimedia/imaging-overview?view=netframeworkdesktop-4.8
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select cover graphic...";
            ofd.Filter = "Image Files(*.JPG;*JPEG;*.PNG;*.BMP;*.GIF;*.TIF;*.TIFF)|*.BMP;*.JPG;*JPEG;*.GIF;*.PNG;*.TIF;*.TIFF";
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == true)
            {
                mov.CoverPath = ofd.FileName;
                LinkedFile lf = new LinkedFile(LinkedFile.FileType.Image, LinkedFile.OpenWith.None, ofd.FileName);
                files.Add(lf);
                lbFiles.Items.Refresh();
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            UpdateMov();

            if (mov != movOld) // check if data has been modified
            {
                foreach (var lf in files2remove) // remove obsolete files
                {
                    lf.Delete(db);
                }

                // check for languages/ genres which have been deleted
                string[] languges = movOld.Languages.Split(',');
                foreach (string lang in languges)
                    if (!mov.Languages.Contains(lang.Trim()))
                        mov.RemoveForeignKeys(db, "language:L", lang.Trim());
                string[] subtitles = movOld.Subtitles.Split(',');
                foreach (string lang in subtitles)
                    if (!mov.Subtitles.Contains(lang.Trim()))
                        mov.RemoveForeignKeys(db, "language:S", lang.Trim());
                string[] audioDesc = movOld.AudioDescription.Split(',');
                foreach (string lang in audioDesc)
                    if (!mov.AudioDescription.Contains(lang.Trim()))
                        mov.RemoveForeignKeys(db, "language:A", lang.Trim());
                string[] genres = movOld.Genre.Split(',');
                foreach (string genre in genres)
                    if (!mov.Genre.Contains(genre.Trim()))
                        mov.RemoveForeignKeys(db, "genre", genre.Trim());

                mov.UpdateInDB(db); // store changes in db or create new element
                movOld = mov.DeepCopy(); // update last status

                MessageBox.Show("Successfully saved element.", "Save", MessageBoxButton.OK);
            }
        }

        private void btnAbort_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // close window
            // onclosing event handler will check if data has been modified
        }
        #endregion
    }
}

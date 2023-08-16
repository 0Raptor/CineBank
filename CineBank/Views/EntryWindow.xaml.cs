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

        }

        private void btnRemoveContent_Click(object sender, RoutedEventArgs e)
        {
            if (lbFiles.SelectedItem == null) // validate selection
                return;

            LinkedFile lf = lbFiles.SelectedItem as LinkedFile;
            files.Remove(lf);
            lbFiles.Items.Refresh();
        }

        private void btnSelectCover_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            UpdateMov();

            if (mov != movOld) // check if data has been modified
            {
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

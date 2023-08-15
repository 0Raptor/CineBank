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
            this.DataContext = mov;
        }

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

        }

        private void btnSelectCover_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (mov != movOld) // check if data has been modified
            {
                mov.UpdateInDB(db); // store changes in db or create new element
                movOld = mov.DeepCopy(); // update last status

                MessageBox.Show("Successfully saved element.", "Save", MessageBoxButton.OK);
            }
        }

        private void btnAbort_Click(object sender, RoutedEventArgs e)
        {
            if (mov != movOld) // check if data has been modified
            {
                var res = MessageBox.Show("There are unsaved changes for this entry.\r\nDiscard changes and close window anyway?", "Discard Changes?", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (res == MessageBoxResult.No)
                    return;
            }
            this.Close();
        }
        #endregion
    }
}

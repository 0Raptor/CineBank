using CineBank.Classes;
using Microsoft.Win32;
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
    /// Interaktionslogik für CreateDbDialog.xaml
    /// </summary>
    public partial class CreateDbDialog : Window
    {
        public CreateDbDialog(string path = "", string basedir = "")
        {
            InitializeComponent();

            tbPath.Text = path;
            tbBaseDir.Text = basedir;
        }

        /// <summary>
        /// 0: Path
        /// 1: BaseDir
        /// </summary>
        public string[] Answer
        {
            get { return new string[] { tbPath.Text, tbBaseDir.Text }; }
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        // select basepath directory
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog folderBrowserDialog = new VistaFolderBrowserDialog();
            folderBrowserDialog.Multiselect = false;
            if (folderBrowserDialog.ShowDialog() == true)
                tbBaseDir.Text = folderBrowserDialog.SelectedPath;
        }

        // select file
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "SQLite Database (*.db)|*.db";
            saveFileDialog.AddExtension = true;
            saveFileDialog.CheckPathExists = true;
            if (saveFileDialog.ShowDialog() == true)
                tbPath.Text = saveFileDialog.FileName;
        }
    }
}

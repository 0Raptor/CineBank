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
    /// Interaktionslogik für AddFileDialog.xaml
    /// </summary>
    public partial class AddFileDialog : Window
    {
        string BaseDir = "";
        List<LinkedFile> Files;
        Dictionary<int, string> TypeEnum;
        Dictionary<int, string> OpenEnum;

        /// <summary>
        /// Dialog to select files from the filesystem and get them as LinkedFile-Object. Use with AddFileDialog.ShowDialog().
        /// </summary>
        /// <param name="baseDir">(optional) specify if the paths should be added without absolut paths. Will warn the user if file-path does not match the baseDir.</param>
        public AddFileDialog(string baseDir = "")
        {
            // set baseDir & init list
            BaseDir = baseDir;
            Files = new List<LinkedFile>();

            // convert LinkedFile-Enums to dictionarys to use them in comboboxes
            TypeEnum = Enum.GetValues(typeof(LinkedFile.FileType))
               .Cast<LinkedFile.FileType>()
               .ToDictionary(t => (int)t, t => t.ToString());
            OpenEnum = Enum.GetValues(typeof(LinkedFile.OpenWith))
               .Cast<LinkedFile.OpenWith>()
               .ToDictionary(t => (int)t, t => t.ToString());

            InitializeComponent();

            // fill comboboxes
            foreach (var elem in TypeEnum)
                cbType.Items.Add(elem.Key + " - " + elem.Value);
            foreach (var elem in OpenEnum)
                if (!elem.Value.Contains("None"))
                    cbOpen.Items.Add(elem.Key + " - " + elem.Value);
            cbType.SelectedIndex = 0;
            cbOpen.SelectedIndex = 0;

            // set default media type
            rbTypeFile.IsChecked = true;
        }

        // get answer from dialog
        public LinkedFile[] Answer
        {
            get { return Files.ToArray(); }
        }

        // button ok
        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            // reapply FileType and OpenWith
            foreach (var file in Files)
            {
                if (!String.IsNullOrWhiteSpace(BaseDir)) // check if database uses relative paths
                {
                    if (!file.Path.StartsWith(BaseDir)) // validate file is located in BaseDir
                    {
                        MessageBox.Show("Added file '" + file.Path + "' is not part of the content root/ basePath '" + BaseDir + "'!\r\nSince your database is configured to use relative paths this file cannot be added.",
                            "New file not part of content root directory", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // remove baseDir from filepath to generate relative path
                    file.Path = file.Path.Substring(BaseDir.Length);
                }

                file.Type = (LinkedFile.FileType)Convert.ToInt32(cbType.Text.Split('-')[0].Trim());
                file.Open = (LinkedFile.OpenWith)Convert.ToInt32(cbOpen.Text.Split('-')[0].Trim());
            }

            // close dialog with status "true"
            this.DialogResult = true;
        }

        // button select file to select file(s)
        private void btnSelectFiles_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)rbTypeFile.IsChecked) // select file(s)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Title = "Select file(s) to add...";
                ofd.CheckFileExists = true;
                ofd.CheckPathExists = true;
                ofd.Multiselect = true;
                if (ofd.ShowDialog() == true)
                {
                    // delete previous selection
                    Files.Clear();
                    // loop over all selected files
                    string txt = "";
                    foreach (var file in ofd.FileNames)
                    {
                        Files.Add(new LinkedFile((LinkedFile.FileType)Convert.ToInt32(cbType.Text.Split('-')[0].Trim()),
                            (LinkedFile.OpenWith)Convert.ToInt32(cbOpen.Text.Split('-')[0].Trim()),
                            file));
                        txt += file + ";";
                    }
                    tbFiles.Text = txt; // display selection to user
                }
            }
            else // select a folder
            {
                VistaFolderBrowserDialog folderBrowserDialog = new VistaFolderBrowserDialog();
                folderBrowserDialog.Multiselect = false;
                if (folderBrowserDialog.ShowDialog() == true)
                {
                    Files.Clear(); // delete previous selection
                    tbFolder.Text = folderBrowserDialog.SelectedPath; // display selection to user
                    Files.Add(new LinkedFile((LinkedFile.FileType)Convert.ToInt32(cbType.Text.Split('-')[0].Trim()),
                            (LinkedFile.OpenWith)Convert.ToInt32(cbOpen.Text.Split('-')[0].Trim()),
                            folderBrowserDialog.SelectedPath)); // create item
                }
            }
        }

        // check if dialog will be used to import files or a folder
        private void rbGroup_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)rbTypeFile.IsChecked)
            {
                btnFiles.IsEnabled = true; // disable button of other method
                btnFolder.IsEnabled = false;

                tbFiles.Text = tbFolder.Text = ""; // clear content
                Files.Clear();
            }
            else
            {
                btnFiles.IsEnabled = false; // disable button of other method
                btnFolder.IsEnabled = true;

                tbFiles.Text = tbFolder.Text = ""; // clear content
                Files.Clear();
            }
        }
    }
}

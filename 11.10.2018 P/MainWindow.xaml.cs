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
using System.IO;
using winForm = System.Windows.Forms;
using System.Diagnostics;

namespace _11._10._2018_P
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void choose_folder(object sender, RoutedEventArgs e)
        {
            string path1 = @"C:\משתמשים\";
            string pathToNewFolder = System.IO.Path.Combine(path1, "BATABASE");
            Directory.CreateDirectory(pathToNewFolder);
            //for choosing file
            //FileDialog filed = new OpenFileDialog();
            //filed.ShowDialog();
            //for choosing folder
            winForm.FolderBrowserDialog folderDialog = new winForm.FolderBrowserDialog();
            folderDialog.ShowNewFolderButton = false;
            folderDialog.SelectedPath = System.AppDomain.CurrentDomain.BaseDirectory;
            winForm.DialogResult result = folderDialog.ShowDialog();

            if (result == winForm.DialogResult.OK)
            {
                string pathS = folderDialog.SelectedPath;
                try
                {
                    DirectoryInfo folder = new DirectoryInfo(pathS);
                    if (folder.Exists)
                    {
                        sortFiles(folder);
                    }
                    MessageBox.Show(" !!!הצליח להעתיק");


                }
                catch (Exception)
                {
                    MessageBox.Show("לא הצליח להעתיק");
                    throw;
                }

            }
        }
        public void sortFiles(DirectoryInfo folder)
        {

            string targetPath = @"C:\משתמשים\BATABASE";
            //אם יש עוד תקיות אז
            foreach (DirectoryInfo folderI in folder.GetDirectories())
            {
                sortFiles(folderI);
            }
            string Path = @"";
            string sourcePath;
            //רק אם יש קבצים בתקיה
            foreach (FileInfo fi in folder.GetFiles())
            {
                string fileName = fi.ToString();
                sourcePath = Path + fi.DirectoryName;
                 string sourceFile = System.IO.Path.Combine(sourcePath, fileName);
                string destFile = System.IO.Path.Combine(targetPath, fileName);
                System.IO.File.Copy(sourceFile, destFile, true);
            }
        }
    }
}

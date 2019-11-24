using System;
using System.IO;
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

namespace Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        System.IO.DirectoryInfo saveFileLocation = new DirectoryInfo(@"C:\Users\Shane\Desktop\Emulator\dev_hdd0\home\00000001\savedata\BLUS30443DEMONSS005");
        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        DirectoryInfo mainFolder;
        List<Folder> folders;
        List<Savefile> savefiles;

        public MainWindow()
        {
            InitializeComponent();
            folders = new List<Folder>();
            savefiles = new List<Savefile>();

            mainFolder = System.IO.Directory.CreateDirectory(desktopPath + "\\Demon's Souls Savefiles");

            string[] foldersInMainFolder =  System.IO.Directory.GetDirectories(mainFolder.FullName);

            for (int i = 0; i < foldersInMainFolder.Length; i++)
            {
                Savefile newsavefile = new Savefile(System.IO.Path.GetFileName(foldersInMainFolder[i]), new DirectoryInfo(foldersInMainFolder[i]));
                savefiles.Add(newsavefile);
            }

            lstBoxSavefiles.ItemsSource = savefiles;

        }

        private void BtnCreateSavefile_Click(object sender, RoutedEventArgs e)
        {
            if(txboxSavefileName.Text != "")
            {
                bool SaveFileAlreadyExists = false;
                Savefile newSaveFile = new Savefile(txboxSavefileName.Text, System.IO.Directory.CreateDirectory(mainFolder.FullName + "\\" + txboxSavefileName.Text));

                foreach (Savefile savefile in savefiles)
                {
                    if (savefile.Name == newSaveFile.Name)
                        SaveFileAlreadyExists = true;
                }

                if(!SaveFileAlreadyExists)
                {
                    savefiles.Add(newSaveFile);

                    lstBoxSavefiles.ItemsSource = null;
                    lstBoxSavefiles.ItemsSource = savefiles;

                    CreateSaveFile(newSaveFile);

                    txboxSavefileName.Text = "";
                    tblkErrorMessage.Text = "";
                }
                else
                {
                    tblkErrorMessage.Text = "Savefile with same name already exists.";
                }

            }
        }

        private void BtnImportSavestate_Click(object sender, RoutedEventArgs e)
        {
            //If save file is selected.
            if(lstBoxSavefiles.SelectedIndex != -1)
            {
                foreach (FileInfo file in saveFileLocation.GetFiles())
                {
                    file.Delete();
                }

                Savefile selectedSaveFile = savefiles[lstBoxSavefiles.SelectedIndex];

                ImportSavestate(selectedSaveFile);

                tblkErrorMessage.Text = "";
            }
            else
            {
                tblkErrorMessage.Text = "No savefile selected.";
            }

        }

        private void ImportSavestate(Savefile savefile)
        {
            string[] files = System.IO.Directory.GetFiles(savefile.directoryInfo.FullName);

            foreach (string nameoffile in files)
            {
                //Get file name.
                string fileName = System.IO.Path.GetFileName(nameoffile);
                //Combines folder location (DeS Folder) and file name (Emulator Folder)
                string destFile = System.IO.Path.Combine(saveFileLocation.FullName, fileName);
                //Copy method needs exact location of source file (Emulator folder) and destination file(Des Folder) and copies file here.
                System.IO.File.Copy(nameoffile, destFile, true);
            }
        }

        //Gets actual files and adds them to folder.
        private void CreateSaveFile(Savefile savefile)
        {
            string[] files = System.IO.Directory.GetFiles(saveFileLocation.FullName);

            // Copy the files and overwrite destination files if they already exist. 
            foreach (string nameoffile in files)
            {
                //Get name and extension.
                string fileName = System.IO.Path.GetFileName(nameoffile);
                //Combines folder location (DeS Folder) and file name (Emulator Folder)
                string destFile = System.IO.Path.Combine(savefile.directoryInfo.FullName, fileName);
                //Copy method needs exact location of source file (Emulator folder) and destination file(Des Folder) and copies file here.
                System.IO.File.Copy(nameoffile, destFile, true);
            }
        }
    }
}

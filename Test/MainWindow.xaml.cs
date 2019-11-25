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
        List<Segment> segments;

        public MainWindow()
        {
            InitializeComponent();
            segments = new List<Segment>();

            mainFolder = System.IO.Directory.CreateDirectory(desktopPath + "\\Demon's Souls Savefiles");

            string[] segmentsInMainFolder =  System.IO.Directory.GetDirectories(mainFolder.FullName);

            //Go through all categories already in main folder and create category objects.
            for (int c = 0; c < segmentsInMainFolder.Length; c++)
            {
                Segment newSegment = new Segment(System.IO.Path.GetFileName(segmentsInMainFolder[c]), new DirectoryInfo(segmentsInMainFolder[c]));
                segments.Add(newSegment);

                //Get all savefiles in category and add them to category savefile list.
                string[] savefilesInCategory = System.IO.Directory.GetDirectories(newSegment.directoryInfo.FullName);
                for (int s = 0; s < savefilesInCategory.Length; s++)
                {
                    newSegment.savefiles.Add(new Savefile(System.IO.Path.GetFileName(savefilesInCategory[s]), new DirectoryInfo(savefilesInCategory[s]), newSegment));
                }

            }
            
            lstboxSegments.ItemsSource = segments;
        }

        private void BtnCreateSavefile_Click(object sender, RoutedEventArgs e)
        {
            //If text is in text box.
            if(txboxSavefileName.Text != "")
            {
                //If category is selected.
                if(lstboxSegments.SelectedIndex != -1)
                {
                    //Checking if savefile with same name already exists in that category.
                    bool SaveFileAlreadyExists = false;                 
                    foreach (Savefile savefile in segments[lstboxSegments.SelectedIndex].savefiles)
                    {
                        if (savefile.Name == txboxSavefileName.Text)
                            SaveFileAlreadyExists = true;
                    }

                    if (!SaveFileAlreadyExists)
                    {
                        //Create savefile and add to selected categories list of savefiles.
                        Savefile newSaveFile = new Savefile
                        (txboxSavefileName.Text, System.IO.Directory.CreateDirectory(segments[lstboxSegments.SelectedIndex].directoryInfo.FullName + "\\" + txboxSavefileName.Text),
                        segments[lstboxSegments.SelectedIndex]);
                        segments[lstboxSegments.SelectedIndex].savefiles.Add(newSaveFile);

                        CreateSaveFile(newSaveFile);

                        lstBoxSavefiles.ItemsSource = null;
                        lstBoxSavefiles.ItemsSource = segments[lstboxSegments.SelectedIndex].savefiles;

                        txboxSavefileName.Text = "";
                        tblkErrorMessage.Text = "";
                    }
                    else
                    {
                        tblkErrorMessage.Text = "Choose different name.";
                    }
                }
                else
                {
                    tblkErrorMessage.Text = "No segment selected.";
                }              
            }
            else
            {
                tblkErrorMessage.Text = "Enter savefile name.";
            }
        }

        private void BtnImportSavestate_Click(object sender, RoutedEventArgs e)
        {
            //If segment is selected.
            if(lstboxSegments.SelectedIndex != -1)
            {
                //If save file is selected.
                if (lstBoxSavefiles.SelectedIndex != -1)
                {
                    //Delete files in actual savefile location.
                    foreach (FileInfo file in saveFileLocation.GetFiles())
                    {
                        file.Delete();
                    }

                    //Add new savefile.
                    Savefile selectedSaveFile = segments[lstboxSegments.SelectedIndex].savefiles[lstBoxSavefiles.SelectedIndex];
                    ImportSavestate(selectedSaveFile);

                    tblkErrorMessage.Text = "";
                }
                else
                {
                    tblkErrorMessage.Text = "No savefile selected.";
                }
            }
            else
            {
                tblkErrorMessage.Text = "No segment selected.";
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

        //Gets actual savefile and adds to desired folder.
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

        private void BtnCreateSegment_Click(object sender, RoutedEventArgs e)
        {
            if(txboxCreateSegment.Text != "")
            {
                bool SegmentAlreadyExists = false;
                Segment newSegment = new Segment(txboxCreateSegment.Text, System.IO.Directory.CreateDirectory(mainFolder.FullName + "\\" + txboxCreateSegment.Text));

                foreach (Segment segment in segments)
                {
                    if (segment.Name == newSegment.Name)
                        SegmentAlreadyExists = true;
                }

                if(!SegmentAlreadyExists)
                {
                    segments.Add(newSegment);

                    lstboxSegments.ItemsSource = null;
                    lstboxSegments.ItemsSource = segments;

                    lstBoxSavefiles.ItemsSource = null;
                    lstBoxSavefiles.ItemsSource = newSegment.savefiles;

                    tblkErrorMessage.Text = "";
                    txboxCreateSegment.Text = "";
                }
                else
                {
                    tblkErrorMessage.Text = "Category already exists.";
                }
            }
            else
            {
                tblkErrorMessage.Text = "Enter segment name.";
            }
        }

        private void LstboxSegments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(lstboxSegments.SelectedIndex != -1)
            {
                int selectedIndex = lstboxSegments.SelectedIndex;

                lstBoxSavefiles.ItemsSource = null;
                lstBoxSavefiles.ItemsSource = segments[selectedIndex].savefiles;
            }
        }

        private void BtnUpdateSave_Click(object sender, RoutedEventArgs e)
        {
            if(lstboxSegments.SelectedIndex != -1)
            {
                if (lstBoxSavefiles.SelectedIndex != -1)
                {
                    Savefile saveFileToUpdate = segments[lstboxSegments.SelectedIndex].savefiles[lstBoxSavefiles.SelectedIndex];

                    CreateSaveFile(saveFileToUpdate);
                }
                else
                {
                    tblkErrorMessage.Text = "No Savefile Selected.";
                }
            }
            else
            {
                tblkErrorMessage.Text = "No Category Selected.";
            }
        }

        private void BtnDeleteSavefile1_Click(object sender, RoutedEventArgs e)
        {
            if(lstBoxSavefiles.SelectedIndex != -1)
            {
                Savefile savefile = segments[lstboxSegments.SelectedIndex].savefiles[lstBoxSavefiles.SelectedIndex];

                System.IO.Directory.Delete(savefile.directoryInfo.FullName, true);
                segments[lstboxSegments.SelectedIndex].savefiles.Remove(savefile);

                lstBoxSavefiles.ItemsSource = null;
                lstBoxSavefiles.ItemsSource = segments[lstboxSegments.SelectedIndex].savefiles;
            }
            else
            {
                tblkErrorMessage.Text = "No savefile selected.";
            }
        }

        private void BtnDeleteSegment_Click(object sender, RoutedEventArgs e)
        {
            if(lstboxSegments.SelectedIndex != -1)
            {
                Segment segmentToDelete = segments[lstboxSegments.SelectedIndex];
                segments.Remove(segmentToDelete);

                System.IO.Directory.Delete(segmentToDelete.directoryInfo.FullName, true);

                lstboxSegments.ItemsSource = null;
                lstboxSegments.ItemsSource = segments;

                lstBoxSavefiles.ItemsSource = null;
            }
            else
            {
                tblkErrorMessage.Text = "No category selected.";
            }

        }
    }
}

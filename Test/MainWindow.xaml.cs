﻿using System;
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
    public partial class MainWindow : Window
    {
        enum TypeOfNotificationMessage
        {
            Success, //0
            Error //1
        }

        //System.IO.DirectoryInfo saveFileLocation = new DirectoryInfo(@"C:\Users\Shane\Desktop\Emulator\dev_hdd0\home\00000001\savedata\BLUS30443DEMONSS005");
        System.IO.DirectoryInfo saveFileLocation = new DirectoryInfo(@"C:\Users\s00189384\Desktop\SaveFileLocation");
        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        DirectoryInfo mainFolder;
        List<Segment> segments;
        List<Category> categories;

        public MainWindow()
        {
            InitializeComponent();

            segments = new List<Segment>();
            categories = new List<Category>();

            mainFolder = System.IO.Directory.CreateDirectory(desktopPath + "\\Savefiles");

            //Categories.
            string[] categoriesInMainFolder = System.IO.Directory.GetDirectories(mainFolder.FullName);
            for (int c = 0; c < categoriesInMainFolder.Length; c++)
            {
                Category currentCategory = new Category(System.IO.Path.GetFileName(categoriesInMainFolder[c]), new DirectoryInfo(categoriesInMainFolder[c]));
                categories.Add(currentCategory);

                //Segments in each category.
                string[] segmentsInCategory = System.IO.Directory.GetDirectories(currentCategory.directoryInfo.FullName);
                for (int s = 0; s < segmentsInCategory.Length; s++)
                {
                    Segment currentSegment = new Segment(System.IO.Path.GetFileName(segmentsInCategory[s]), new DirectoryInfo(segmentsInCategory[s]), currentCategory);
                    currentCategory.segments.Add(currentSegment);

                    //Savefiles in each segment.
                    string[] saveFilesInSegment = System.IO.Directory.GetDirectories(currentSegment.directoryInfo.FullName);
                    for (int x = 0; x < saveFilesInSegment.Length; x++)
                    {
                        Savefile currentSavefile = new Savefile(System.IO.Path.GetFileName(saveFilesInSegment[x]), new DirectoryInfo(saveFilesInSegment[x]), currentSegment);
                        currentSegment.savefiles.Add(currentSavefile);
                    }
                }
            }

            lstBoxCategories.ItemsSource = categories;

        }



        //Categories.
        private void LstBoxCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedCategoryIndex = lstBoxCategories.SelectedIndex;

            if (selectedCategoryIndex != -1)
            {
                lstboxSegments.ItemsSource = null;
                lstboxSegments.ItemsSource = categories[selectedCategoryIndex].segments;
            }
        }

        //Segments.
        //Fix.
        private void BtnCreateSegment_Click(object sender, RoutedEventArgs e)
        {
            //if (txboxCreateSegment.Text != "")
            //{
            //    bool SegmentAlreadyExists = false;
            //    Segment newSegment = new Segment(txboxCreateSegment.Text, System.IO.Directory.CreateDirectory(mainFolder.FullName + "\\" + txboxCreateSegment.Text));

            //    foreach (Segment segment in segments)
            //    {
            //        if (segment.Name == newSegment.Name)
            //            SegmentAlreadyExists = true;
            //    }

            //    if (!SegmentAlreadyExists)
            //    {
            //        segments.Add(newSegment);

            //        lstboxSegments.ItemsSource = null;
            //        lstboxSegments.ItemsSource = segments;

            //        lstBoxSavefiles.ItemsSource = null;
            //        lstBoxSavefiles.ItemsSource = newSegment.savefiles;

            //        UpdateNotificationMessage("Segment created.", TypeOfNotificationMessage.Success);
            //        txboxCreateSegment.Text = "";
            //    }
            //    else
            //    {
            //        UpdateNotificationMessage("Category already exists.", TypeOfNotificationMessage.Error);
            //    }
            //}
            //else
            //{
            //    UpdateNotificationMessage("Enter segment name.", TypeOfNotificationMessage.Error);
            //}
        }


        //Fix next
        private void LstboxSegments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(lstboxSegments.SelectedIndex != -1)
            {
                int selectedIndex = lstboxSegments.SelectedIndex;

                lstBoxSavefiles.ItemsSource = null;
                lstBoxSavefiles.ItemsSource = segments[selectedIndex].savefiles;
            }
        }
        private void BtnDeleteSegment_Click(object sender, RoutedEventArgs e)
        {
            if (lstboxSegments.SelectedIndex != -1)
            {
                Segment segmentToDelete = segments[lstboxSegments.SelectedIndex];
                segments.Remove(segmentToDelete);

                System.IO.Directory.Delete(segmentToDelete.directoryInfo.FullName, true);

                lstboxSegments.ItemsSource = null;
                lstboxSegments.ItemsSource = segments;

                lstBoxSavefiles.ItemsSource = null;

                UpdateNotificationMessage("Segment deleted.", TypeOfNotificationMessage.Success);
            }
            else
            {
                UpdateNotificationMessage("No segment selected.", TypeOfNotificationMessage.Error);
            }

        }

        //Saves.
        private void BtnCreateSavefile_Click(object sender, RoutedEventArgs e)
        {
            //If text is in text box.
            if (txboxSavefileName.Text != "")
            {
                //If category is selected.
                if (lstboxSegments.SelectedIndex != -1)
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
                        UpdateNotificationMessage("Savefile created.", TypeOfNotificationMessage.Success);
                    }
                    else
                    {
                        UpdateNotificationMessage("Savefile with that name already exists.", TypeOfNotificationMessage.Error);
                    }
                }
                else
                {
                    UpdateNotificationMessage("No segment selected.", TypeOfNotificationMessage.Error);
                }
            }
            else
            {
                UpdateNotificationMessage("Enter savefile name.", TypeOfNotificationMessage.Error);
            }
        }
        private void BtnImportSavestate_Click(object sender, RoutedEventArgs e)
        {
            //If segment is selected.
            if (lstboxSegments.SelectedIndex != -1)
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

                    UpdateNotificationMessage("Save imported to game.", TypeOfNotificationMessage.Success);
                }
                else
                {
                    UpdateNotificationMessage("No savefile selected.", TypeOfNotificationMessage.Error);
                }
            }
            else
            {
                UpdateNotificationMessage("No segment selected.", TypeOfNotificationMessage.Error);
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
        private void BtnUpdateSave_Click(object sender, RoutedEventArgs e)
        {
            if(lstboxSegments.SelectedIndex != -1)
            {
                if (lstBoxSavefiles.SelectedIndex != -1)
                {
                    Savefile saveFileToUpdate = segments[lstboxSegments.SelectedIndex].savefiles[lstBoxSavefiles.SelectedIndex];

                    CreateSaveFile(saveFileToUpdate);
                    UpdateNotificationMessage("Savefile updated.", TypeOfNotificationMessage.Success);

                }
                else
                {
                    UpdateNotificationMessage("No savefile selected.", TypeOfNotificationMessage.Error);
                }
            }
            else
            {
                UpdateNotificationMessage("No segment selected.", TypeOfNotificationMessage.Error);
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

                UpdateNotificationMessage("Savefile deleted.", TypeOfNotificationMessage.Success);
            }
            else
            {
                UpdateNotificationMessage("No savefile selected.", TypeOfNotificationMessage.Error);
            }
        }

        //Notifications / Descriptions.
        private async void UpdateNotificationMessage(string message,TypeOfNotificationMessage typeOfMessage)
        {
            //If the user presses the same button dont execute code below.
            if (message == tblkNotificationMessage.Text)
                return;

            //Update the text and colour.
            tblkNotificationMessage.Text = message;

            if (typeOfMessage == TypeOfNotificationMessage.Success)
                tblkNotificationMessage.Foreground = new SolidColorBrush(Colors.Green);
            else
                tblkNotificationMessage.Foreground = new SolidColorBrush(Colors.Red);

            //Wait for 3.5 seconds. 
            await Task.Delay(3500);

            //If the user didn't press another button - i.e the text stays the same then reset the notification message.
            if (message == tblkNotificationMessage.Text)
            {
                tblkNotificationMessage.Text = "";
            }
        }
        private void HideButtonDescription()
        {
            tblkButtonDescription.Text = "";
        }
        //Button mouse over / exit - shows and hides description of what button does.
        private void BtnCreateSegment_MouseEnter(object sender, MouseEventArgs e)
        {
            tblkButtonDescription.Text = "Creates segment where savefiles can be added.";
        }
        private void BtnCreateSegment_MouseLeave(object sender, MouseEventArgs e)
        {
            HideButtonDescription();

        }
        private void BtnCreateSavefile_MouseEnter(object sender, MouseEventArgs e)
        {
            tblkButtonDescription.Text = "Creates savefile based on current savestates in game.";
        }
        private void BtnCreateSavefile_MouseLeave(object sender, MouseEventArgs e)
        {
            HideButtonDescription();
        }
        private void BtnDeleteSegment_MouseEnter(object sender, MouseEventArgs e)
        {
            tblkButtonDescription.Text = "Delete selected segment and all of its savefiles.";
        }
        private void BtnDeleteSegment_MouseLeave(object sender, MouseEventArgs e)
        {
            HideButtonDescription();
        }
        private void BtnDeleteSavefile1_MouseEnter(object sender, MouseEventArgs e)
        {
            tblkButtonDescription.Text = "Delete selected savefile.";
        }
        private void BtnDeleteSavefile1_MouseLeave(object sender, MouseEventArgs e)
        {
            HideButtonDescription();
        }
        private void BtnImportSavestate_MouseEnter(object sender, MouseEventArgs e)
        {
            tblkButtonDescription.Text = "Overites save in game to selected savefile.";
        }
        private void BtnImportSavestate_MouseLeave(object sender, MouseEventArgs e)
        {
            HideButtonDescription();
        }
        private void BtnUpdateSave_MouseEnter(object sender, MouseEventArgs e)
        {
            tblkButtonDescription.Text = "Updates selected savefile to savefile in game.";
        }
        private void BtnUpdateSave_MouseLeave(object sender, MouseEventArgs e)
        {
            HideButtonDescription();
        }
    }
}

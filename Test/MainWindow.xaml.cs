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
    public partial class MainWindow : Window
    {
        enum TypeOfNotificationMessage
        {
            Success, //0
            Error //1
        }

        //System.IO.DirectoryInfo saveFileLocation = new DirectoryInfo(@"C:\Users\Shane\Desktop\Emulator\dev_hdd0\home\00000001\savedata\BLUS30443DEMONSS005");
        //System.IO.DirectoryInfo saveFileLocation = new DirectoryInfo(@"C:\Users\Shane\Desktop\FakeSaveFileLocation");
        System.IO.DirectoryInfo saveFileLocation = new DirectoryInfo(@"C:\Users\Shane\Desktop\Emulator\dev_hdd0\home\00000001\savedata\BLUS30443DEMONSS005");
        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        DirectoryInfo mainFolder;
        List<Category> categoryList = new List<Category>();

        public MainWindow()
        {
            InitializeComponent();
            mainFolder = System.IO.Directory.CreateDirectory(desktopPath + "\\Demon's Souls Savefiles");

            ImportCreatedSavefiles();
            RefreshListBox<Category>(lstBoxCategories, categoryList);
        }

        //Start
        private void ImportCreatedSavefiles()
        {
            //Categories.
            string[] categoriesInMainFolder = System.IO.Directory.GetDirectories(mainFolder.FullName);
            for (int c = 0; c < categoriesInMainFolder.Length; c++)
            {
                Category currentCategory = new Category(System.IO.Path.GetFileName(categoriesInMainFolder[c]), new DirectoryInfo(categoriesInMainFolder[c]));
                categoryList.Add(currentCategory);

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
        }

        //Categories.
        private void LstBoxCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstBoxCategories.SelectedIndex != -1)
            {
                RefreshListBox<Savefile>(lstBoxSavefiles, null);
                RefreshListBox<Segment>(lstboxSegments, GetSelectedCategory().segments);
            }
        }
        private void BtnCreateCategory_Click(object sender, RoutedEventArgs e)
        {
            if (!UserCanCreateObject(typeof(Category), lstBoxCategories, txboxCreateCategory))
                return;

            //Create category and folder.
            string nameOfCategoryToCreate = txboxCreateCategory.Text;
            categoryList.Add(new Category(nameOfCategoryToCreate, System.IO.Directory.CreateDirectory(mainFolder.FullName + "\\" + nameOfCategoryToCreate)));

            //Update window.
            RefreshListBox<Category>(lstBoxCategories, categoryList);
            UpdateNotificationMessage("Category added.", TypeOfNotificationMessage.Success);
            UpdateTextBox(txboxCreateCategory, "");
        }
        private void BtnDeleteCategory_Click(object sender, RoutedEventArgs e)
        {
            if(lstBoxCategories.SelectedIndex != -1)
            {
                //Delete Category and Folder with all its content.
                Category categoryToDelete = categoryList[lstBoxCategories.SelectedIndex];
                System.IO.Directory.Delete(categoryToDelete.directoryInfo.FullName,true);
                categoryList.Remove(categoryToDelete);
                categoryToDelete = null;

                //Update window.
                RefreshListBox<Category>(lstBoxCategories, categoryList);
                RefreshListBox<Segment>(lstboxSegments, null);
                RefreshListBox<Savefile>(lstBoxSavefiles, null);
                UpdateNotificationMessage("Category Deleted.", TypeOfNotificationMessage.Success);
            }
            else
            {
                UpdateNotificationMessage("No category selected", TypeOfNotificationMessage.Error);
            }
        }

        //Segments.
        private void BtnCreateSegment_Click(object sender, RoutedEventArgs e)
        {
            if (!UserCanCreateObject(typeof(Segment), lstboxSegments, txboxCreateSegment))
                return;

            //Create new segment and folder.
            Category selectedCategory = GetSelectedCategory();
            Segment newSegment = new Segment(txboxCreateSegment.Text, System.IO.Directory.CreateDirectory(mainFolder.FullName + "\\" + selectedCategory.Name + "\\" + txboxCreateSegment.Text),selectedCategory);
            selectedCategory.segments.Add(newSegment);
                  
            //Update window.
            RefreshListBox<Segment>(lstboxSegments, selectedCategory.segments);
            RefreshListBox<Savefile>(lstBoxSavefiles, newSegment.savefiles);
            UpdateTextBox(txboxCreateSegment, "");
            UpdateNotificationMessage("Segment created.", TypeOfNotificationMessage.Success);

              
        }
        private void LstboxSegments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(lstboxSegments.SelectedIndex != -1)
            {
                RefreshListBox<Savefile>(lstBoxSavefiles, GetSelectedSegment().savefiles);
            }
        }
        private void BtnDeleteSegment_Click(object sender, RoutedEventArgs e)
        {
            if (lstBoxCategories.SelectedIndex != -1)
            {
                if(lstboxSegments.SelectedIndex != -1)
                {
                    Category selectedCategory = GetSelectedCategory();
                    Segment segmentToDelete = GetSelectedSegment();

                    System.IO.Directory.Delete(segmentToDelete.directoryInfo.FullName, true);
                    selectedCategory.segments.Remove(segmentToDelete);

                    RefreshListBox<Segment>(lstboxSegments, selectedCategory.segments);
                    RefreshListBox<Savefile>(lstBoxSavefiles, null);
                    UpdateNotificationMessage("Segment deleted.", TypeOfNotificationMessage.Success);
                }
                else
                {
                    UpdateNotificationMessage("No segment selected.", TypeOfNotificationMessage.Error);
                }             
            }
            else
            {
                UpdateNotificationMessage("No category selected.", TypeOfNotificationMessage.Error);
            }
        }

        //Saves. 
        private void BtnCreateSavefile_Click(object sender, RoutedEventArgs e)
        {
            if (!UserCanCreateObject(typeof(Savefile), lstBoxSavefiles, txboxSavefileName))
                return;
 
            //Create savefile and folder.
            Segment selectedSegment = GetSelectedSegment();
            Savefile newSaveFile = new Savefile(txboxSavefileName.Text, System.IO.Directory.CreateDirectory(selectedSegment.directoryInfo.FullName + "\\" + txboxSavefileName.Text),selectedSegment);
            selectedSegment.savefiles.Add(newSaveFile);
            CreateSaveFile(newSaveFile);

            //Update Window.
            RefreshListBox<Savefile>(lstBoxSavefiles,selectedSegment.savefiles);
            UpdateTextBox(txboxSavefileName, "");
            UpdateNotificationMessage("Savefile created.", TypeOfNotificationMessage.Success);
        }
        private void BtnImportSavestate_Click(object sender, RoutedEventArgs e)
        {
            if (lstBoxCategories.SelectedIndex != -1)
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
                        ImportSavestate(GetSelectedSavefile());
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
            else
            {
                UpdateNotificationMessage("No category selected.", TypeOfNotificationMessage.Error);
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
            if(lstBoxCategories.SelectedIndex != -1)
            {
                if (lstboxSegments.SelectedIndex != -1)
                {
                    //Update Save.
                    if (lstBoxSavefiles.SelectedIndex != -1)
                    {
                        CreateSaveFile(GetSelectedSavefile());
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
            else
            {
                UpdateNotificationMessage("No category selected.", TypeOfNotificationMessage.Error);
            }         
        }
        private void BtnDeleteSavefile1_Click(object sender, RoutedEventArgs e)
        {
            if (lstBoxSavefiles.SelectedIndex != -1)
            {
                Savefile selectedSavefile = GetSelectedSavefile();

                System.IO.Directory.Delete(selectedSavefile.directoryInfo.FullName, true);
                GetSelectedSegment().savefiles.Remove(selectedSavefile);

                RefreshListBox<Savefile>(lstBoxSavefiles, GetSelectedSegment().savefiles);
                UpdateNotificationMessage("Savefile deleted.", TypeOfNotificationMessage.Success);
            }
            else
            {
                UpdateNotificationMessage("No savefile selected.", TypeOfNotificationMessage.Error);
            }
        }

        //Getting selected savefiles, segments, categories.
        private Savefile GetSelectedSavefile()
        {
            return categoryList[lstBoxCategories.SelectedIndex].segments[lstboxSegments.SelectedIndex].savefiles[lstBoxSavefiles.SelectedIndex];
        }
        private Segment GetSelectedSegment()
        {
            return categoryList[lstBoxCategories.SelectedIndex].segments[lstboxSegments.SelectedIndex];
        }
        private Category GetSelectedCategory()
        {
            return categoryList[lstBoxCategories.SelectedIndex];
        }
        private string GetSelectedItemName(Type typeOfObject, ListBox listBoxContainingItem)
        {
            return listBoxContainingItem.SelectedItem.GetType().ToString();
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
                tblkNotificationMessage.Text = "";

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

        //General
        private void UpdateTextBlock(TextBlock textBlock,string message)
        {
            textBlock.Text = message;
        }
        private void UpdateTextBox(TextBox textBox, string message)
        {
            textBox.Text = message;
        }
        private void RefreshListBox<T>(ListBox listBoxToRefresh,List<T> listToShow)
        {
            listBoxToRefresh.ItemsSource = null;
            listBoxToRefresh.ItemsSource = listToShow;
        }
        private bool UserCanCreateObject(Type typeOfObject, ListBox listBoxToAddObjectTo, TextBox textBoxWithObjectName)
        {
            //Check if text box has name for object.
            if (textBoxWithObjectName.Text == "")
            {
                UpdateNotificationMessage("Enter " + typeOfObject.Name + " Name.", TypeOfNotificationMessage.Error);
                return false;
            }

            //Check if right items are selected in list boxes to create object.

            if (!ListboxHasItemSelected(listBoxToAddObjectTo))
                return false;

            //Check if object with same name already exists.
            if (listBoxToAddObjectTo.ItemsSource != null)
            {
                string nameOfObjectToCreate = textBoxWithObjectName.Text;

                foreach (object listItem in listBoxToAddObjectTo.ItemsSource)
                {
                    if (listItem.ToString() == nameOfObjectToCreate)
                    {
                        UpdateTextBox(textBoxWithObjectName, "");
                        UpdateNotificationMessage(typeOfObject.Name + " With Same Name Already Exists", TypeOfNotificationMessage.Error);
                        return false;
                    }
                }
            }

            //If all checks are successfull return true.
            return true;
        }
        private bool ListboxHasItemSelected(ListBox listBoxToCheck)
        {
            if(listBoxToCheck == lstBoxCategories)
            {
                if (lstBoxCategories.SelectedIndex == -1)
                {
                    UpdateNotificationMessage("No Category Selected", TypeOfNotificationMessage.Error);
                    return false;
                }
                else
                    return true;                        
            }

            else if (listBoxToCheck == lstboxSegments)
            {
                if (lstBoxCategories.SelectedIndex == -1)
                {
                    UpdateNotificationMessage("No Category Selected", TypeOfNotificationMessage.Error);
                    return false;
                }

                else
                    return true;
            }

            else if(listBoxToCheck == lstBoxSavefiles)
            {
                if (lstBoxCategories.SelectedIndex == -1)
                {
                    UpdateNotificationMessage("No Category Selected", TypeOfNotificationMessage.Error);
                    return false;
                }
                else if (lstboxSegments.SelectedIndex == -1)
                {
                    UpdateNotificationMessage("No Segment Selected", TypeOfNotificationMessage.Error);
                    return false;
                }
                else
                    return true;
            }

            return true;
        }
    }
}

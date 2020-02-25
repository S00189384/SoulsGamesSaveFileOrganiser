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


        //Games
        LinkedList<Game> GamesList = new LinkedList<Game>();
        LinkedListNode<Game> selectedGameNode;

        public MainWindow()
        {
            InitializeComponent();
            mainFolder = System.IO.Directory.CreateDirectory(desktopPath + "\\Save File Organiser");
            CreateGames();

            //ImportCreatedSavefiles();
            comboBoxCategory.ItemsSource = categoryList;
        }

        //Start
        private void CreateGames()
        {
            Game DarkSouls3 = new Game("Dark Souls 3", mainFolder);
            System.IO.Directory.CreateDirectory(mainFolder.FullName + "\\" + DarkSouls3.Name);
            GamesList.AddFirst(DarkSouls3);

            Game DarkSouls2 = new Game("Dark Souls 2", mainFolder);
            System.IO.Directory.CreateDirectory(mainFolder.FullName + "\\" + DarkSouls2.Name);
            GamesList.AddFirst(DarkSouls2);

            Game DarkSoulsRemastered = new Game("Dark Souls Remastered", mainFolder);
            System.IO.Directory.CreateDirectory(mainFolder.FullName + "\\" + DarkSoulsRemastered.Name);
            GamesList.AddFirst(DarkSoulsRemastered);

            Game DarkSouls = new Game("Dark Souls", mainFolder);
            System.IO.Directory.CreateDirectory(mainFolder.FullName + "\\" + DarkSouls.Name);
            GamesList.AddFirst(DarkSouls);

            Game DemonsSouls = new Game("Demon's Souls", mainFolder);
            System.IO.Directory.CreateDirectory(mainFolder.FullName + "\\" + DemonsSouls.Name);
            GamesList.AddFirst(DemonsSouls);

            selectedGameNode = GamesList.First;
        }

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
        private void ComboBoxCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxCategory.SelectedIndex != -1)
            {
                RefreshListBox<Savefile>(lstBoxSavefiles, null);
                RefreshListBox(lstboxSegments, GetSelectedCategory().segments);
            }
        }
        private void BtnCreateCategory_Click(object sender, RoutedEventArgs e)
        {
            if (!CanUserCreateObject(typeof(Category), categoryList, txboxCreateCategory))
                return;

            //Create category and folder.
            string nameOfNewCategory = txboxCreateCategory.Text;
            Category newCategory = new Category(nameOfNewCategory, System.IO.Directory.CreateDirectory(mainFolder.FullName + "\\" + nameOfNewCategory));
            categoryList.Add(newCategory);

            //Update window.
            RefreshComboBox(comboBoxCategory, categoryList);
            UpdateNotificationMessage("Category added.", TypeOfNotificationMessage.Success);
            UpdateTextBox(txboxCreateCategory, "");
        }
        private void BtnDeleteCategory_Click(object sender, RoutedEventArgs e)
        {
            if (!IsItemSelected(typeof(Category)))
                return;

            //Delete Category and Folder with all its content.
            Category categoryToDelete = GetSelectedCategory();
            System.IO.Directory.Delete(categoryToDelete.directoryInfo.FullName, true);
            categoryList.Remove(categoryToDelete);
            categoryToDelete = null;

            //Update window.
            RefreshComboBox(comboBoxCategory, categoryList);
            RefreshListBox<Segment>(lstboxSegments, GetSelectedSegments());
            RefreshListBox<Savefile>(lstBoxSavefiles, null);
            UpdateNotificationMessage("Category Deleted.", TypeOfNotificationMessage.Success);
        }
        //Segments.
        private void BtnCreateSegment_Click(object sender, RoutedEventArgs e)
        {
            if (!CanUserCreateObject(typeof(Segment), GetSelectedSegments(), txboxCreateSegment))
                return;

            //Create new segment and folder.
            Category selectedCategory = GetSelectedCategory();
            Segment newSegment = new Segment(txboxCreateSegment.Text, System.IO.Directory.CreateDirectory(mainFolder.FullName + "\\" + selectedCategory.Name + "\\" + txboxCreateSegment.Text), selectedCategory);
            selectedCategory.segments.Add(newSegment);

            //Update window.
            RefreshListBox(lstboxSegments, selectedCategory.segments);
            RefreshListBox(lstBoxSavefiles, newSegment.savefiles);
            UpdateTextBox(txboxCreateSegment, "");
            UpdateNotificationMessage("Segment created.", TypeOfNotificationMessage.Success);
        }
        private void LstboxSegments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(lstboxSegments.SelectedIndex != -1)
            {
                RefreshListBox(lstBoxSavefiles, GetSelectedSavefiles());
            }
        }
        private void BtnDeleteSegment_Click(object sender, RoutedEventArgs e)
        {
            if (!IsItemSelected(typeof(Segment)))
                return;

            //Get selected category and segment.
            Category selectedCategory = GetSelectedCategory();
            Segment segmentToDelete = GetSelectedSegment();

            //Delete segment and folder.
            System.IO.Directory.Delete(segmentToDelete.directoryInfo.FullName, true);
            selectedCategory.segments.Remove(segmentToDelete);

            //Refresh window.
            RefreshListBox(lstboxSegments, selectedCategory.segments);
            RefreshListBox<Savefile>(lstBoxSavefiles, null);
            UpdateNotificationMessage("Segment Deleted.", TypeOfNotificationMessage.Success);
        }

        //Saves. 
        private void BtnCreateSavefile_Click(object sender, RoutedEventArgs e)
        {
            if (!CanUserCreateObject(typeof(Savefile), GetSelectedSavefiles(), txboxSavefileName))
                return;

            //Create savefile and folder.
            Segment selectedSegment = GetSelectedSegment();
            Savefile newSaveFile = new Savefile(txboxSavefileName.Text, System.IO.Directory.CreateDirectory(selectedSegment.directoryInfo.FullName + "\\" + txboxSavefileName.Text), selectedSegment);
            selectedSegment.savefiles.Add(newSaveFile);
            CreateSaveFile(newSaveFile);

            //Update Window.
            RefreshListBox(lstBoxSavefiles, selectedSegment.savefiles);
            UpdateTextBox(txboxSavefileName, "");
            UpdateNotificationMessage("Savefile created.", TypeOfNotificationMessage.Success);
        }
        private void BtnImportSavestate_Click(object sender, RoutedEventArgs e)
        {
            if (!IsItemSelected(typeof(Savefile)))
                return;

            //Delete files in actual savefile location.
            foreach (FileInfo file in saveFileLocation.GetFiles())
            {
                file.Delete();
            }

            //Add new savefile.
            ImportSavestate(GetSelectedSavefile());
            UpdateNotificationMessage("Save imported to game.", TypeOfNotificationMessage.Success);
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
            if (!IsItemSelected(typeof(Savefile)))
                return;

            //Update savefile.
            CreateSaveFile(GetSelectedSavefile());
            UpdateNotificationMessage("Savefile updated.", TypeOfNotificationMessage.Success);
        }
        private void BtnDeleteSavefile1_Click(object sender, RoutedEventArgs e)
        {
            if (!IsItemSelected(typeof(Savefile)))
                return;

            //Delete savefile.
            Savefile selectedSavefile = GetSelectedSavefile();
            System.IO.Directory.Delete(selectedSavefile.directoryInfo.FullName, true);
            GetSelectedSegment().savefiles.Remove(selectedSavefile);

            //Refresh window.
            RefreshListBox(lstBoxSavefiles, GetSelectedSavefiles());
            UpdateNotificationMessage("Savefile deleted.", TypeOfNotificationMessage.Success);
        }

        //Getting selected savefiles, segments, categories.
        private Savefile GetSelectedSavefile()
        {
            return categoryList[comboBoxCategory.SelectedIndex].segments[lstboxSegments.SelectedIndex].savefiles[lstBoxSavefiles.SelectedIndex];
        }
        private Segment GetSelectedSegment()
        {
            return categoryList[comboBoxCategory.SelectedIndex].segments[lstboxSegments.SelectedIndex];
        }
        private Category GetSelectedCategory()
        {
            return comboBoxCategory.SelectedIndex != -1 ? categoryList[comboBoxCategory.SelectedIndex] : null;
        }
        private List<Segment> GetSelectedSegments()
        {
            return comboBoxCategory.SelectedIndex == -1 ? null : GetSelectedCategory().segments;
        }
        private List<Savefile> GetSelectedSavefiles()
        {
            return comboBoxCategory.SelectedIndex == -1 || lstboxSegments.SelectedIndex == -1 ? null : GetSelectedSegment().savefiles;
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
        private void BtnDeleteCategory_MouseEnter(object sender, MouseEventArgs e)
        {
            tblkButtonDescription.Text = "Delete selected category.";
        }
        private void BtnDeleteCategory_MouseLeave(object sender, MouseEventArgs e)
        {
            HideButtonDescription();
        }
        private void BtnCreateCategory_MouseEnter(object sender, MouseEventArgs e)
        {
            tblkButtonDescription.Text = "Create category which segments and savefiles can be added to.";
        }
        private void BtnCreateCategory_MouseLeave(object sender, MouseEventArgs e)
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
        //Test when adding games.
        private void RefreshComboBox<T>(ComboBox comboBoxToRefresh,List<T> listToShow)
        {
            comboBoxToRefresh.ItemsSource = null;
            comboBoxToRefresh.ItemsSource = listToShow;
            if (categoryList.Count != 0)
                comboBoxCategory.SelectedIndex = categoryList.Count - 1;

        }

        //Making program not crash if user hasn't correct objects selected.
        private bool CanUserCreateObject<T>(Type typeOfObject, List<T> listToCheck, TextBox textBoxWithObjectName)
        {
            //Check if text box has name.
            if (textBoxWithObjectName.Text == "")
            {
                UpdateNotificationMessage("Enter " + typeOfObject.Name + " Name.", TypeOfNotificationMessage.Error);
                return false;
            }

            //Check if correct selections have been made.
            if (typeOfObject == typeof(Category))
                return true;
            else if (typeOfObject == typeof(Segment))
            {
                if (comboBoxCategory.SelectedIndex == -1)
                {
                    UpdateNotificationMessage("No Category Selected", TypeOfNotificationMessage.Error);
                    return false;
                }
                else
                    return true;
            }
            else if (typeOfObject == typeof(Savefile))
            {
                if (comboBoxCategory.SelectedIndex == -1)
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

            //Check if name in list already exists.
            else if (listToCheck.Count != 0 || listToCheck != null)
            {
                foreach (object listItem in listToCheck)
                {
                    if (listItem.ToString() == textBoxWithObjectName.Text)
                    {
                        UpdateTextBox(textBoxWithObjectName, "");
                        UpdateNotificationMessage(typeOfObject.Name + " With Same Name Already Exists", TypeOfNotificationMessage.Error);
                        return false;
                    }
                }
            }

            //If all checks are successful return true.
            return true;
        }
        private bool IsItemSelected(Type typeOfObject)
        {
            bool IsItemSelected = true;

            if (typeOfObject == typeof(Category))
                IsItemSelected = comboBoxCategory.SelectedIndex != -1;
            else if(typeOfObject == typeof(Segment))
                IsItemSelected = lstboxSegments.SelectedIndex != -1;
            else if(typeOfObject == typeof(Savefile))
                IsItemSelected = lstBoxSavefiles.SelectedIndex != -1;

            if(IsItemSelected == false)
            {
                UpdateNotificationMessage("No " + typeOfObject.Name + " Selected.", TypeOfNotificationMessage.Error);
                return false;
            }

            //If all checks are successfull return true.
            return true;
        }
    }
}

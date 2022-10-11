using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Test
{
    public partial class MainWindow : Window
    {
        enum TypeOfNotificationMessage
        {
            Success, //0
            Error //1
        }
        private int notificationMessageShowTimeInMilliseconds = 3500;
        Settings settingsWindow;
        public List<Game> GamesList = new List<Game>();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeComponent();
            settingsWindow = new Settings(this);
            CreateGames();
            JsonDirectoryInfoFile.UpdateGameDirectories(GamesList);
            ImportCreatedSavefiles();
        }

        //Start
        private void CreateGames()
        {
            Game DemonsSouls = new Game("Demon's Souls");
            GamesList.Add(DemonsSouls);

            Game DarkSouls = new Game("Dark Souls");
            GamesList.Add(DarkSouls);

            Game DarkSoulsRemastered = new Game("Dark Souls Remastered");
            GamesList.Add(DarkSoulsRemastered);

            Game DarkSouls2 = new Game("Dark Souls 2");
            GamesList.Add(DarkSouls2);

            Game DarkSouls3 = new Game("Dark Souls 3");
            GamesList.Add(DarkSouls3);

            comboBoxGame.ItemsSource = GamesList;
            comboBoxGame.SelectedIndex = 0;           
        }
        public void ImportCreatedSavefiles()
        {
            //For each game.
            for (int g = 0; g < GamesList.Count; g++)
            {
                if(GamesList[g].Directory != null && Directory.Exists(GamesList[g].Directory.FullName))
                {
                    //Categories in each game.
                    string[] categoriesInGame = System.IO.Directory.GetDirectories(GamesList[g].Directory.FullName);
                    for (int c = 0; c < categoriesInGame.Length; c++)
                    {
                        Category currentCategory = new Category(System.IO.Path.GetFileName(categoriesInGame[c]), new DirectoryInfo(categoriesInGame[c]));
                        GamesList[g].Categories.Add(currentCategory);

                        //Segments in each category.
                        string[] segmentsInCategory = System.IO.Directory.GetDirectories(currentCategory.Directory.FullName);

                        for (int s = 0; s < segmentsInCategory.Length; s++)
                        {
                            Segment currentSegment = new Segment(System.IO.Path.GetFileName(segmentsInCategory[s]), new DirectoryInfo(segmentsInCategory[s]), currentCategory);
                            currentCategory.Segments.Add(currentSegment);

                            //Savefiles in each segment.
                            string[] saveFilesInSegment = System.IO.Directory.GetDirectories(currentSegment.Directory.FullName);
                            for (int x = 0; x < saveFilesInSegment.Length; x++)
                            {
                                Savefile currentSavefile = new Savefile(System.IO.Path.GetFileName(saveFilesInSegment[x]), new DirectoryInfo(saveFilesInSegment[x]), currentSegment);
                                currentSegment.Savefiles.Add(currentSavefile);
                            }
                        }
                    }
                }               
            }
        }

        //Games
        private void ComboBoxGame_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(comboBoxGame.SelectedIndex != -1)
            {
                WindowUpdater.RefreshComboBox(comboBoxCategory, GetSelectedGame().Categories);
                WindowUpdater.RefreshListBox<Segment>(lstboxSegments, null);
                WindowUpdater.RefreshListBox<Savefile>(lstBoxSavefiles, null);
                UpdateBackgroundPicture();
            }
        }

        //Categories.
        private void ComboBoxCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxCategory.SelectedIndex != -1)
            {
                WindowUpdater.RefreshListBox(lstboxSegments, GetSelectedCategory().Segments);
                WindowUpdater.RefreshListBox<Savefile>(lstBoxSavefiles, null);
            }
        }
        private void BtnCreateCategory_Click(object sender, RoutedEventArgs e)
        {
            if (!UserCanCreateObject(typeof(Category), GetSelectedGame().Categories, txboxCreateCategory))
                return;

            //Create category and folder.
            Game selectedGame = GetSelectedGame();
            string nameOfNewCategory = txboxCreateCategory.Text;

            Category newCategory = new Category(nameOfNewCategory, System.IO.Directory.CreateDirectory(selectedGame.Directory.FullName + "\\" + nameOfNewCategory));
            selectedGame.Categories.Add(newCategory);

            //Update window.
            WindowUpdater.RefreshComboBox(comboBoxCategory, selectedGame.Categories);
            UpdateNotificationMessage("Category added.", TypeOfNotificationMessage.Success);
            WindowUpdater.UpdateTextBox(txboxCreateCategory, "");
            comboBoxCategory.SelectedIndex = selectedGame.Categories.IndexOf(newCategory);
        }
        private void BtnDeleteCategory_Click(object sender, RoutedEventArgs e)
        {
            if (!IsItemSelected(typeof(Category)))
                return;

            //Delete Category and Folder with all its content.
            Category categoryToDelete = GetSelectedCategory();
            try
            {
                System.IO.Directory.Delete(categoryToDelete.Directory.FullName, true);
            }
            catch (Exception) { }

            GetSelectedGame().Categories.Remove(categoryToDelete);
            categoryToDelete = null;

            //Update window.
            WindowUpdater.RefreshComboBox(comboBoxCategory, GetSelectedGame().Categories);
            WindowUpdater.RefreshListBox<Segment>(lstboxSegments, null);
            WindowUpdater.RefreshListBox<Savefile>(lstBoxSavefiles, null);
            UpdateNotificationMessage("Category Deleted.", TypeOfNotificationMessage.Success);

            //Maybe select category above one deleted if any. Atm combo box has nothing selected when you delete.
        }
        //Segments.
        private void BtnCreateSegment_Click(object sender, RoutedEventArgs e)
        {
            if (!UserCanCreateObject(typeof(Segment), GetSelectedSegments(), txboxCreateSegment))
                return;

            //Create new segment and folder.
            Category selectedCategory = GetSelectedCategory();
            Segment newSegment = new Segment(txboxCreateSegment.Text, System.IO.Directory.CreateDirectory(GetSelectedGame().Directory.FullName + "\\" + selectedCategory.Name + "\\" + txboxCreateSegment.Text), selectedCategory);
            selectedCategory.Segments.Add(newSegment);

            //Update window.
            lstboxSegments.SelectedIndex = selectedCategory.Segments.IndexOf(newSegment);
            WindowUpdater.RefreshListBox(lstboxSegments, selectedCategory.Segments);
            WindowUpdater.RefreshListBox(lstBoxSavefiles, newSegment.Savefiles);
            WindowUpdater.UpdateTextBox(txboxCreateSegment, "");
            UpdateNotificationMessage("Segment Created.", TypeOfNotificationMessage.Success);
        }
        private void LstboxSegments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(lstboxSegments.SelectedIndex != -1)
            {
                WindowUpdater.RefreshListBox(lstBoxSavefiles, GetSelectedSavefiles());
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
            System.IO.Directory.Delete(segmentToDelete.Directory.FullName, true);
            selectedCategory.Segments.Remove(segmentToDelete);

            //Refresh window.
            WindowUpdater.RefreshListBox(lstboxSegments, selectedCategory.Segments);
            WindowUpdater.RefreshListBox<Savefile>(lstBoxSavefiles, null);
            UpdateNotificationMessage("Segment Deleted.", TypeOfNotificationMessage.Success);
        }

        //Saves. 
        private void BtnCreateSavefile_Click(object sender, RoutedEventArgs e)
        {
            if (GameHasSaveFileDirectory(GetSelectedGame()) && UserCanCreateObject(typeof(Savefile), GetSelectedSavefiles(), txboxSavefileName))
            {
                //Create savefile and folder.
                Segment selectedSegment = GetSelectedSegment();
                Savefile newSaveFile = new Savefile(txboxSavefileName.Text, System.IO.Directory.CreateDirectory(selectedSegment.Directory.FullName + "\\" + txboxSavefileName.Text), selectedSegment);
                selectedSegment.Savefiles.Add(newSaveFile);
                CreateSaveFile(newSaveFile);

                //Update Window.
                WindowUpdater.RefreshListBox(lstBoxSavefiles, selectedSegment.Savefiles);
                WindowUpdater.UpdateTextBox(txboxSavefileName, "");
                UpdateNotificationMessage("Savefile created.", TypeOfNotificationMessage.Success);
            }
        }
        private void BtnImportSavestate_Click(object sender, RoutedEventArgs e)
        {
            if (GameHasSaveFileDirectory(GetSelectedGame()) && IsItemSelected(typeof(Savefile)))
            {
                //Delete files in actual savefile location.
                foreach (FileInfo file in GetSelectedGame().SaveFileDirectory.GetFiles())
                {
                    file.Delete();
                }

                //Add new savefile.
                ImportSavestate(GetSelectedSavefile());
                UpdateNotificationMessage("Save imported to game.", TypeOfNotificationMessage.Success);
            }
        }
        private void ImportSavestate(Savefile savefile)
        {
            if(GameHasSaveFileDirectory(GetSelectedGame()))
            {
                string[] files = System.IO.Directory.GetFiles(savefile.Directory.FullName);

                foreach (string nameoffile in files)
                {
                    //Get file name.
                    string fileName = System.IO.Path.GetFileName(nameoffile);
                    //Combines directory location and save file name.
                    string destFile = System.IO.Path.Combine(GetSelectedGame().SaveFileDirectory.FullName, fileName);
                    //Copy method needs exact location of save file and destination file (profiles folder) and copies file here.
                    System.IO.File.Copy(nameoffile, destFile, true);
                }
            }
        }
        private void CreateSaveFile(Savefile savefile)
        {
            Game selectedGame = GetSelectedGame();

            string[] files = System.IO.Directory.GetFiles(selectedGame.SaveFileDirectory.FullName);

            // Copy the files and overwrite destination files if they already exist. 
            foreach (string nameoffile in files)
            {
                //Get name and extension.
                string fileName = System.IO.Path.GetFileName(nameoffile);
                //Combines folder location (DeS Folder) and file name (Emulator Folder)
                string destFile = System.IO.Path.Combine(savefile.Directory.FullName, fileName);
                //Copy method needs exact location of source file (Emulator folder) and destination file(Des Folder) and copies file here.
                System.IO.File.Copy(nameoffile, destFile, true);
            }
        }
        private void BtnUpdateSave_Click(object sender, RoutedEventArgs e)
        {
            if (GameHasSaveFileDirectory(GetSelectedGame()) && IsItemSelected(typeof(Savefile)))
            {
                //Update savefile.
                CreateSaveFile(GetSelectedSavefile());
                UpdateNotificationMessage("Savefile updated.", TypeOfNotificationMessage.Success);
            }        
        }
        private void BtnDeleteSavefile1_Click(object sender, RoutedEventArgs e)
        {
            if (IsItemSelected(typeof(Savefile)))
            {
                //Delete savefile.
                Savefile selectedSavefile = GetSelectedSavefile();
                System.IO.Directory.Delete(selectedSavefile.Directory.FullName, true);
                GetSelectedSegment().Savefiles.Remove(selectedSavefile);

                //Refresh window.
                WindowUpdater.RefreshListBox(lstBoxSavefiles, GetSelectedSavefiles());
                UpdateNotificationMessage("Savefile deleted.", TypeOfNotificationMessage.Success);
            }
        }

        //Getting selected games, savefiles, segments, categories.
        public Game GetSelectedGame()
        {
            return comboBoxGame.SelectedIndex != -1 ? GamesList[comboBoxGame.SelectedIndex] : new Game();
        }
        private Savefile GetSelectedSavefile()
        {
            return GamesList[comboBoxGame.SelectedIndex].Categories[comboBoxCategory.SelectedIndex].Segments[lstboxSegments.SelectedIndex].Savefiles[lstBoxSavefiles.SelectedIndex];
        }
        private Segment GetSelectedSegment()
        {
            return GamesList[comboBoxGame.SelectedIndex].Categories[comboBoxCategory.SelectedIndex].Segments[lstboxSegments.SelectedIndex];
        }
        private Category GetSelectedCategory()
        {
            return comboBoxCategory.SelectedIndex != -1 ? GetSelectedGame().Categories[comboBoxCategory.SelectedIndex] : null;
        }
        private List<Segment> GetSelectedSegments()
        {
            return comboBoxCategory.SelectedIndex == -1 ? new List<Segment>() : GetSelectedCategory().Segments;
        }
        private List<Savefile> GetSelectedSavefiles()
        {
            return comboBoxCategory.SelectedIndex == -1 || lstboxSegments.SelectedIndex == -1 ? new List<Savefile>() : GetSelectedSegment().Savefiles;
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
            await Task.Delay(notificationMessageShowTimeInMilliseconds);

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
            WindowUpdater.UpdateTextBlock(tblkButtonDescription, "Creates segment where savefiles can be added.");
        }
        private void BtnCreateSegment_MouseLeave(object sender, MouseEventArgs e)
        {
            HideButtonDescription();
        }
        private void BtnCreateSavefile_MouseEnter(object sender, MouseEventArgs e)
        {
            WindowUpdater.UpdateTextBlock(tblkButtonDescription, "Creates savefile based on current savestates in game.");
        }
        private void BtnCreateSavefile_MouseLeave(object sender, MouseEventArgs e)
        {
            HideButtonDescription();
        }
        private void BtnDeleteSegment_MouseEnter(object sender, MouseEventArgs e)
        {
            WindowUpdater.UpdateTextBlock(tblkButtonDescription, "Delete selected segment and all of its savefiles.");
        }
        private void BtnDeleteSegment_MouseLeave(object sender, MouseEventArgs e)
        {
            HideButtonDescription();
        }
        private void BtnDeleteSavefile1_MouseEnter(object sender, MouseEventArgs e)
        {
            WindowUpdater.UpdateTextBlock(tblkButtonDescription, "Delete selected savefile.");
        }
        private void BtnDeleteSavefile1_MouseLeave(object sender, MouseEventArgs e)
        {
            HideButtonDescription();
        }
        private void BtnImportSavestate_MouseEnter(object sender, MouseEventArgs e)
        {
            WindowUpdater.UpdateTextBlock(tblkButtonDescription, "Overwrites save in game to selected savefile.");
        }
        private void BtnImportSavestate_MouseLeave(object sender, MouseEventArgs e)
        {
            HideButtonDescription();
        }
        private void BtnUpdateSave_MouseEnter(object sender, MouseEventArgs e)
        {
            WindowUpdater.UpdateTextBlock(tblkButtonDescription, "Updates selected savefile to savefile in game.");
        }
        private void BtnUpdateSave_MouseLeave(object sender, MouseEventArgs e)
        {
            HideButtonDescription();
        }
        private void BtnDeleteCategory_MouseEnter(object sender, MouseEventArgs e)
        {
            WindowUpdater.UpdateTextBlock(tblkButtonDescription, "Delete selected category.");
        }
        private void BtnDeleteCategory_MouseLeave(object sender, MouseEventArgs e)
        {
            HideButtonDescription();
        }
        private void BtnCreateCategory_MouseEnter(object sender, MouseEventArgs e)
        {
            WindowUpdater.UpdateTextBlock(tblkButtonDescription, "Create category which segments and savefiles can be added to.");
        }
        private void BtnCreateCategory_MouseLeave(object sender, MouseEventArgs e)
        {
            HideButtonDescription();
        }
        private void BtnEditSettings_MouseEnter_1(object sender, MouseEventArgs e)
        {
            WindowUpdater.UpdateTextBlock(tblkButtonDescription, "Edit Directory Settings");
        }
        private void BtnEditSettings_MouseLeave_1(object sender, MouseEventArgs e)
        {
            HideButtonDescription();
        }

        //Updating window.
        private void UpdateBackgroundPicture()
        {
            //Background images are indexed to represent the souls games 0 - Demon's Souls / 1 - Dark Souls etc.
            //Using this index to update background image.
            //Picture name format must have "BackgroundImage + index +.png" 

            int selectedGameIndex = GamesList.IndexOf(GetSelectedGame());

            ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource = new BitmapImage(new Uri(string.Format(@"pack://application:,,,/Test;component/Pictures\BackgroundImage" + selectedGameIndex.ToString() + ".png")));
            GridBackground.Background = imageBrush;
        }

        //Making program not crash if user hasn't correct objects selected.
        private bool UserCanCreateObject<T>(Type typeOfObject, List<T> listToCheck, TextBox textBoxWithObjectName)
        {
            if (GetSelectedGame().Directory == null)
            {
                System.Windows.MessageBox.Show("Before you create a " + typeOfObject.Name.ToLower() + " you need to select a location for where you want to store the savefiles. You can do this in settings.", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                return false;
            }

            if(!Directory.Exists(GetSelectedGame().Directory.FullName))
            {
                System.Windows.MessageBox.Show("Directory - " + GetSelectedGame().Directory.FullName + " - dooesn't exist. Please select another folder to store the files.", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                return false;
            }

            //Check if correct selections have been made.
            if (typeOfObject == typeof(Category))
            {
                if (comboBoxGame.SelectedIndex == -1)
                {
                    UpdateNotificationMessage("No Game Selected", TypeOfNotificationMessage.Error);
                    return false;
                }
            }

            else if (typeOfObject == typeof(Segment))
            {
                if (comboBoxGame.SelectedIndex == -1)
                {
                    UpdateNotificationMessage("No Game Selected", TypeOfNotificationMessage.Error);
                    return false;
                }
                else if (comboBoxCategory.SelectedIndex == -1)
                {
                    UpdateNotificationMessage("No Category Selected", TypeOfNotificationMessage.Error);
                    return false;
                }
            }
            else if (typeOfObject == typeof(Savefile))
            {
                if (comboBoxGame.SelectedIndex == -1)
                {
                    UpdateNotificationMessage("No Game Selected", TypeOfNotificationMessage.Error);
                    return false;
                }
                else if (comboBoxCategory.SelectedIndex == -1)
                {
                    UpdateNotificationMessage("No Category Selected", TypeOfNotificationMessage.Error);
                    return false;
                }
                else if (lstboxSegments.SelectedIndex == -1)
                {
                    UpdateNotificationMessage("No Segment Selected", TypeOfNotificationMessage.Error);
                    return false;
                }
            }

            //Check if text box has name.
            if (textBoxWithObjectName.Text == "")
            {
                UpdateNotificationMessage("Enter " + typeOfObject.Name + " Name.", TypeOfNotificationMessage.Error);
                return false;
            }

            //Check if name in list already exists.
            if (listToCheck.Count != 0 || listToCheck != null)
            {
                foreach (object listItem in listToCheck)
                {
                    if (listItem.ToString() == textBoxWithObjectName.Text)
                    {
                        WindowUpdater.UpdateTextBox(textBoxWithObjectName, "");
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

            //If all checks are successful return true.
            return true;
        }
        private bool GameHasSaveFileDirectory(Game gameToCheck)
        {
            bool GameHasSaveDirectory = gameToCheck.SaveFileDirectory != null;

            if(!GameHasSaveDirectory)
            {
                System.Windows.MessageBox.Show("You have to select a folder where the savefiles for " + gameToCheck.Name + " is located. You can do this in settings.", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                return false;
            }

            return true;
        }

        //Settings.
        private void BtnEditSettings_Click_1(object sender, RoutedEventArgs e)
        {
            settingsWindow = new Settings(this);
            settingsWindow.Show();
        }
    }
}
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using Microsoft.VisualBasic.FileIO;

namespace Test
{
    /* Settings window where user can choose where they want to store the directories for each game
     * Each game has two directories -
     * One is the folder where the save file is located - the game reads from this folder and updates it to change the games savestate. 
     * The user would have to research where this folder is in the games files themselves.
     * 
     * The other is the "profiles" directory -  where all the different savefiles the user copies over from the savefile folder to folders named and created by themselves.
     * They can easily move savestates in any of these folders to the games savefile folder to update the savestate in game.     
     * 
     * Whole settings window would have to be changed if user had option of adding their own games and not just having the souls games to choose from but it works for now
         */
    public partial class Settings : Window
    {
        MainWindow mainWindow;
   
        public Settings(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
        }

        private void GamesTab_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Game selectedGame = mainWindow.GamesList[GamesTab.SelectedIndex];

            //Update save file location text.
            string selectedGameSaveFileLocation = selectedGame.SaveFileDirectory == null ? "" : selectedGame.SaveFileDirectory.FullName;
            WindowUpdater.UpdateTextBlock(tblkGameSaveFileDirectory, selectedGameSaveFileLocation);

            //Update user game saves folder location text.
            string selectedGameDirectory = selectedGame.Directory == null ? "" : selectedGame.Directory.FullName;
            WindowUpdater.UpdateTextBlock(tblkGameDirectory, selectedGameDirectory);
        }
        private void BtnBrowseSaveFileLocation_Click(object sender, RoutedEventArgs e)
        {
            //Ask user for folder where the games savefile is located.
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            DialogResult dialogResult =  folderBrowserDialog.ShowDialog();

            if(dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                Game selectedGame = mainWindow.GamesList[GamesTab.SelectedIndex];

                string saveFileLocation = folderBrowserDialog.SelectedPath;
                selectedGame.SaveFileDirectory = new DirectoryInfo(saveFileLocation);

                JsonDirectoryInfoFile.UpdateFolderPathInJsonFile(mainWindow.GamesList, selectedGame, "SaveFileDirectory", saveFileLocation);
                WindowUpdater.UpdateTextBlock(tblkGameSaveFileDirectory, saveFileLocation);
            }       
        }
        private void BtnBrowseGameProfilesDirectory_Click(object sender, RoutedEventArgs e)
        {
            //Ask user for folder where the save profiles will be.
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            DialogResult dialogResult = folderBrowserDialog.ShowDialog();

            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                //Get the location of where the folder should be.
                string userSelectedLocation = folderBrowserDialog.SelectedPath;
                Game selectedGame = mainWindow.GamesList[GamesTab.SelectedIndex];
                string newProfilesLocation = userSelectedLocation;

                //If Current Game already has a directory) Get this directory and move it to the new location.
                //Update the games directory.
                if (selectedGame.Directory != null)
                {
                    //User doesn't select same location to move folder.
                    if (selectedGame.Directory.FullName != newProfilesLocation)
                    {    
                        FileSystem.CopyDirectory(selectedGame.Directory.FullName, newProfilesLocation);
                        foreach (DirectoryInfo directory in selectedGame.Directory.GetDirectories())
                        {
                            directory.Delete(true);
                        }

                        selectedGame.Directory = new DirectoryInfo(newProfilesLocation);
                    }
                }
                //If the game doesn't have any directory just make a new one.
                else
                {
                    selectedGame.Directory = System.IO.Directory.CreateDirectory(newProfilesLocation);
                }

                //Update json file and settings window.
                JsonDirectoryInfoFile.UpdateFolderPathInJsonFile(mainWindow.GamesList, selectedGame, "Directory", newProfilesLocation);
                WindowUpdater.UpdateTextBlock(tblkGameDirectory, newProfilesLocation);
                mainWindow.ImportCreatedSavefiles();
            }
        }
    }
}

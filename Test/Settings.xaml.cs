using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Test
{
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
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            DialogResult dialogResult =  folderBrowserDialog.ShowDialog();

            if(dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                Game selectedGame = mainWindow.GamesList[GamesTab.SelectedIndex];

                string saveFileLocation = folderBrowserDialog.SelectedPath;
                selectedGame.SaveFileDirectory = new DirectoryInfo(saveFileLocation);

                //Updating the new save file location in the json file. 

                //string jsonContents = JsonConvert.SerializeObject(mainWindow.GamesList.ToArray(),Formatting.Indented, new JsonSerializerSettings{PreserveReferencesHandling = PreserveReferencesHandling.Objects});
                //JArray jsonArray = JArray.Parse(jsonContents);
                //int indexOfGame = mainWindow.GamesList.IndexOf(selectedGame);
                //jsonArray[indexOfGame]["SaveFileDirectory"] = saveFileLocation.Replace(@"\","\\");
                //File.WriteAllText("gameinfo.json", string.Empty);
                //File.WriteAllText("gameinfo.json", jsonArray.ToString());

                JsonGameInfo.UpdateFolderPathInJsonFile(mainWindow.GamesList, selectedGame, "SaveFileDirectory", saveFileLocation);
                WindowUpdater.UpdateTextBlock(tblkGameSaveFileDirectory, saveFileLocation);
            }       
        }
        private void BtnBrowseGameProfilesDirectory_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            DialogResult dialogResult = folderBrowserDialog.ShowDialog();

            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                //Get the location of where the folder should be.
                string userSelectedLocation = folderBrowserDialog.SelectedPath;
                Game selectedGame = mainWindow.GamesList[GamesTab.SelectedIndex];
                string newProfilesLocation = userSelectedLocation + "\\" + selectedGame.Name;

                //If Current Game already has a directory) Get this directory and move it to the new location.
                //Update the games directory.
                if (selectedGame.Directory != null)
                {
                    //User doesn't select same location to move folder.
                    if (selectedGame.Directory.FullName != newProfilesLocation)
                    {
                        //Crashes if user selects folder that is the selected game's directory. Fix**
                        Directory.Move(selectedGame.Directory.FullName, newProfilesLocation);
                        selectedGame.Directory = new DirectoryInfo(newProfilesLocation);
                    }
                }
                //If the game doesn't have any directory just make a new one.
                else
                {
                    selectedGame.Directory = System.IO.Directory.CreateDirectory(newProfilesLocation);
                }

                JsonGameInfo.UpdateFolderPathInJsonFile(mainWindow.GamesList, selectedGame, "Directory", newProfilesLocation);
                WindowUpdater.UpdateTextBlock(tblkGameDirectory, newProfilesLocation);
            }
        }
    }
}

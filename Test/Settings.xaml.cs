using System;
using System.Collections.Generic;
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Game selectedGame = mainWindow.GamesList[GamesTab.SelectedIndex];

            WindowUpdater.UpdateTextBlock(tblkSaveFileLocation, selectedGame.SaveFileLocation);
            WindowUpdater.UpdateTextBlock(tblkUserSavesLocation, selectedGame.UserSavesLocation);
        }

        private void GamesTab_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Game selectedGame = mainWindow.GamesList[GamesTab.SelectedIndex];

            WindowUpdater.UpdateTextBlock(tblkSaveFileLocation, selectedGame.SaveFileLocation);
            WindowUpdater.UpdateTextBlock(tblkUserSavesLocation, selectedGame.UserSavesLocation);
        }

        private void BtnBrowseSaveFileLocation_Click(object sender, RoutedEventArgs e)
        {
            //Get the user chosen location for games savefile location.
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.ShowDialog();
            string saveFileLocation = folderBrowserDialog.SelectedPath;

            //Update the games save file location.
            Game selectedGame = mainWindow.GamesList[GamesTab.SelectedIndex];
            selectedGame.SaveFileLocation = saveFileLocation;
            WindowUpdater.UpdateTextBlock(tblkSaveFileLocation, saveFileLocation);
        }

        private void BtnBrowseUserSavesFolderLocation_Click(object sender, RoutedEventArgs e)
        {
            //Get the user chosen location for folder.
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.ShowDialog();
            string userGameSavesFolderLocation = folderBrowserDialog.SelectedPath;

            //Create folder and update selected game.
            Game selectedGame = mainWindow.GamesList[GamesTab.SelectedIndex];
            System.IO.Directory.CreateDirectory(userGameSavesFolderLocation + "\\" + selectedGame.Name);
            selectedGame.UserSavesLocation = userGameSavesFolderLocation;
            WindowUpdater.UpdateTextBlock(tblkUserSavesLocation, userGameSavesFolderLocation);

            //System.Windows.MessageBox.Show("Blablah", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Hide();
        }
    }
}

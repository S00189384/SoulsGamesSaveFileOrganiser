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
        List<Category> categories;

        public MainWindow()
        {
            InitializeComponent();
            categories = new List<Category>();

            mainFolder = System.IO.Directory.CreateDirectory(desktopPath + "\\Demon's Souls Savefiles");

            string[] categoriesInMainFolder =  System.IO.Directory.GetDirectories(mainFolder.FullName);

            //Go through all categories already in main folder and create category objects.
            for (int c = 0; c < categoriesInMainFolder.Length; c++)
            {
                Category newCategory = new Category(System.IO.Path.GetFileName(categoriesInMainFolder[c]), new DirectoryInfo(categoriesInMainFolder[c]));
                categories.Add(newCategory);

                //Get all savefiles in category and add them to category savefile list.
                string[] savefilesInCategory = System.IO.Directory.GetDirectories(newCategory.directoryInfo.FullName);
                for (int s = 0; s < savefilesInCategory.Length; s++)
                {
                    newCategory.savefiles.Add(new Savefile(System.IO.Path.GetFileName(savefilesInCategory[s]), new DirectoryInfo(savefilesInCategory[s]), newCategory));
                }

            }
            


            lstboxCategories.ItemsSource = categories;
        }

        private void BtnCreateSavefile_Click(object sender, RoutedEventArgs e)
        {
            //If text is in text box.
            if(txboxSavefileName.Text != "")
            {
                //If category is selected.
                if(lstboxCategories.SelectedIndex != -1)
                {
                    //Checking if savefile with same name already exists in that category.
                    bool SaveFileAlreadyExists = false;                 
                    foreach (Savefile savefile in categories[lstboxCategories.SelectedIndex].savefiles)
                    {
                        if (savefile.Name == txboxSavefileName.Text)
                            SaveFileAlreadyExists = true;
                    }

                    if (!SaveFileAlreadyExists)
                    {
                        //Create savefile and add to selected categories list of savefiles.
                        Savefile newSaveFile = new Savefile
                        (txboxSavefileName.Text, System.IO.Directory.CreateDirectory(categories[lstboxCategories.SelectedIndex].directoryInfo.FullName + "\\" + txboxSavefileName.Text),
                        categories[lstboxCategories.SelectedIndex]);
                        categories[lstboxCategories.SelectedIndex].savefiles.Add(newSaveFile);

                        CreateSaveFile(newSaveFile);

                        lstBoxSavefiles.ItemsSource = null;
                        lstBoxSavefiles.ItemsSource = categories[lstboxCategories.SelectedIndex].savefiles;

                        txboxSavefileName.Text = "";
                        tblkErrorMessage.Text = "";
                    }
                    else
                    {
                        tblkErrorMessage.Text = "Savefile with same name already exists.";
                    }
                }
                else
                {
                    tblkErrorMessage.Text = "No category selected.";
                }              
            }
            else
            {
                tblkErrorMessage.Text = "You must have a name for savefile.";
            }
        }

        private void BtnImportSavestate_Click(object sender, RoutedEventArgs e)
        {
            //If save file is selected.
            if(lstboxCategories.SelectedIndex != -1)
            {
                if (lstBoxSavefiles.SelectedIndex != -1)
                {
                    foreach (FileInfo file in saveFileLocation.GetFiles())
                    {
                        file.Delete();
                    }

                    Savefile selectedSaveFile = categories[lstboxCategories.SelectedIndex].savefiles[lstBoxSavefiles.SelectedIndex];
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
                tblkErrorMessage.Text = "No category selected.";
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

        private void BtnCreateCategory_Click(object sender, RoutedEventArgs e)
        {
            if(txboxCreateCategory.Text != "")
            {
                bool CategoryAlreadyExists = false;
                Category newCategory = new Category(txboxCreateCategory.Text, System.IO.Directory.CreateDirectory(mainFolder.FullName + "\\" + txboxCreateCategory.Text));

                foreach (Category category in categories)
                {
                    if (category.Name == newCategory.Name)
                        CategoryAlreadyExists = true;
                }

                if(!CategoryAlreadyExists)
                {
                    categories.Add(newCategory);

                    lstboxCategories.ItemsSource = null;
                    lstboxCategories.ItemsSource = categories;

                    tblkErrorMessage.Text = "";
                    txboxCreateCategory.Text = "";
                }
                else
                {
                    tblkErrorMessage.Text = "Category already exists.";
                }
            }
        }

        private void LstboxCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(lstboxCategories.SelectedIndex != -1)
            {
                int selectedIndex = lstboxCategories.SelectedIndex;

                lstBoxSavefiles.ItemsSource = null;
                lstBoxSavefiles.ItemsSource = categories[selectedIndex].savefiles;
            }
        }

        private void BtnUpdateSave_Click(object sender, RoutedEventArgs e)
        {
            if(lstboxCategories.SelectedIndex != -1)
            {
                if (lstBoxSavefiles.SelectedIndex != -1)
                {
                    Savefile saveFileToUpdate = categories[lstboxCategories.SelectedIndex].savefiles[lstBoxSavefiles.SelectedIndex];

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
                Savefile savefile = categories[lstboxCategories.SelectedIndex].savefiles[lstBoxSavefiles.SelectedIndex];

                System.IO.Directory.Delete(savefile.directoryInfo.FullName, true);
                categories[lstboxCategories.SelectedIndex].savefiles.Remove(savefile);

                lstBoxSavefiles.ItemsSource = null;
                lstBoxSavefiles.ItemsSource = categories[lstboxCategories.SelectedIndex].savefiles;
            }
            else
            {
                tblkErrorMessage.Text = "No Savefile Selected.";
            }
        }

        private void BtnDeleteCategory_Click(object sender, RoutedEventArgs e)
        {
            if(lstboxCategories.SelectedIndex != -1)
            {
                Category categoryToDelete = categories[lstboxCategories.SelectedIndex];
                categories.Remove(categoryToDelete);

                System.IO.Directory.Delete(categoryToDelete.directoryInfo.FullName, true);

                lstboxCategories.ItemsSource = null;
                lstboxCategories.ItemsSource = categories;

                lstBoxSavefiles.ItemsSource = null;
                //lstBoxSavefiles.ItemsSource = categories[lstboxCategories.SelectedIndex].savefiles;
            }
            else
            {
                tblkErrorMessage.Text = "No Category Selected.";
            }

        }
    }
}

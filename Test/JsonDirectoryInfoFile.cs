using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    /* Class which deals with updating the directories (savefiles and profiles locations on PC) for each game. 
     * It updates a json file that is in the bin directory of the program files so the program can store the locations of directories chosen by the user.
     * Otherwise directory information would get reset everytime the user closes the program.
     * */

    public static class JsonDirectoryInfoFile
    {
        private static string gameInfoFileName = "gameinfo.json";
        private static JsonSerializerSettings serializerSettings = new JsonSerializerSettings() { PreserveReferencesHandling = PreserveReferencesHandling.None};

        //On choosing a new directory location. Update json file.
        public static void UpdateFolderPathInJsonFile(List<Game> gamesListToUpdate, Game gameToUpdate, string key, string path)
        {
            string jsonContents = JsonConvert.SerializeObject(gamesListToUpdate.ToArray(),Formatting.Indented,serializerSettings);
            JArray jsonArray = JArray.Parse(jsonContents, new JsonLoadSettings {LineInfoHandling = LineInfoHandling.Ignore});
            int indexOfGame = gamesListToUpdate.IndexOf(gameToUpdate);
            jsonArray[indexOfGame][key] = path.Replace(@"\", "\\"); // Having a single \ in json file was giving issues.
            File.WriteAllText(gameInfoFileName, string.Empty);
            File.WriteAllText(gameInfoFileName, jsonArray.ToString());
        }

        //Called at start.
        public static void UpdateGameDirectories(List<Game> gamesListToUpdate)
        {
            if (File.Exists(gameInfoFileName)) //User has already run program - update all game objects with their correct directories.
            {
                string jsonContents = File.ReadAllText(gameInfoFileName);
                JArray jsonArray = JArray.Parse(jsonContents);

                for (int i = 0; i < jsonArray.Count; i++)
                {
                    string gameProfilesDirectory = jsonArray[i]["directoryName"].ToString();
                    string gameSavesDirectory = jsonArray[i]["saveFileDirectoryName"].ToString();

                    if (gameProfilesDirectory != "")
                        gamesListToUpdate[i].Directory = new DirectoryInfo(gameProfilesDirectory);

                    if (gameSavesDirectory != "")
                        gamesListToUpdate[i].SaveFileDirectory = new DirectoryInfo(gameSavesDirectory);
                }
            }
            else //First time user started program - create json file.
            {
                string json = JsonConvert.SerializeObject(gamesListToUpdate.ToArray());
                File.WriteAllText(gameInfoFileName, json);
            }
        }
    }
}

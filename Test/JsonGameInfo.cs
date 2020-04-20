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
    public static class JsonGameInfo
    {
        private static string gameInfoFileName = "gameinfo.json";
        private static JsonSerializerSettings serializerSettings = new JsonSerializerSettings() { PreserveReferencesHandling = PreserveReferencesHandling.Objects};

        public static void UpdateFolderPathInJsonFile(List<Game> gamesListToUpdate, Game gameToUpdate, string key, string path)
        {
            string jsonContents = JsonConvert.SerializeObject(gamesListToUpdate.ToArray(), Formatting.Indented, serializerSettings);
            JArray jsonArray = JArray.Parse(jsonContents);
            int indexOfGame = gamesListToUpdate.IndexOf(gameToUpdate);
            jsonArray[indexOfGame][key] = path.Replace(@"\", "\\");
            File.WriteAllText(gameInfoFileName, string.Empty);
            File.WriteAllText(gameInfoFileName, jsonArray.ToString());
        }

        public static void UpdateGameDirectories(List<Game> gamesListToUpdate)
        {
            if (File.Exists(gameInfoFileName)) //User has already run program - update directories.
            {
                string jsonContents = File.ReadAllText(gameInfoFileName);
                JArray jsonArray = JArray.Parse(jsonContents);

                for (int i = 0; i < jsonArray.Count; i++)
                {
                    string gameProfilesDirectory = jsonArray[i]["Directory"].ToString();
                    string gameSavesDirectory = jsonArray[i]["SaveFileDirectory"].ToString();

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

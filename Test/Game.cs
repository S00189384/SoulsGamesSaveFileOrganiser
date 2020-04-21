using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Game
    {
        [JsonProperty]
        public string Name { get; set; }

        private DirectoryInfo directory;
        public DirectoryInfo Directory
        {   get { return directory; }
            set { directory = value; directoryName = value.FullName;}
        }
        private DirectoryInfo savefiledirectory;
        public DirectoryInfo SaveFileDirectory
        {
            get { return savefiledirectory; }
            set { savefiledirectory = value; saveFileDirectoryName = value.FullName; }
        }

        public List<Category> Categories { get; set; }
     
        [JsonProperty]
        string directoryName;
        [JsonProperty]
        string saveFileDirectoryName;

        public Game() { }
        public Game(string Name)
        {
            this.Name = Name;
            Categories = new List<Category>();
        }

        public override string ToString()
        {
            return string.Format("{0}", Name);
        }
    }
}

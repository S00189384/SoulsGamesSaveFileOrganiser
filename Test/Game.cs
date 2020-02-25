using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Game
    {
        public string Name { get; set; }
        public DirectoryInfo directoryInfo { get; set; }
        public List<Category> Categories { get; set; }

        public Game(string Name,DirectoryInfo directoryInfo)
        {
            this.Name = Name;
            this.directoryInfo = directoryInfo;
        }

        public override string ToString()
        {
            return string.Format("{0}", Name);
        }
    }
}

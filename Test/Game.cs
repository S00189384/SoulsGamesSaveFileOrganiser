using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    public class Game
    {
        public string Name { get; set; }
        public DirectoryInfo Directory { get; set; }
        public DirectoryInfo SaveFileDirectory { get; set; }
        public List<Category> Categories { get; set; }

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

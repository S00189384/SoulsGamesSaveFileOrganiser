using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    public class Segment
    {
        public string Name { get; set; }
        public DirectoryInfo Directory { get; set; }
        public List<Savefile> Savefiles { get; set; }
        public Category Category { get; set; }

        public Segment(string Name,DirectoryInfo Directory,Category Category)
        {
            this.Name = Name;
            this.Directory = Directory;
            this.Category = Category;
            Savefiles = new List<Savefile>();
        }

        public override string ToString()
        {
            return string.Format("{0}", Name);
        }
    }
}

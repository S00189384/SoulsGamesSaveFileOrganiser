using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Test
{
    public class Savefile
    {
        public string Name { get; set; }
        public DirectoryInfo Directory { get; set; }
        public Segment Segment { get; set; }

        public Savefile(string Name,DirectoryInfo Directory,Segment Segment)
        {
            this.Name = Name;
            this.Directory = Directory;
            this.Segment = Segment;
        }

        public override string ToString()
        {
            return string.Format("{0}", Name);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Segment
    {
        public string Name { get; set; }
        public DirectoryInfo directoryInfo { get; set; }
        public List<Savefile> savefiles { get; set; }

        public Segment(string Name,DirectoryInfo directoryInfo)
        {
            this.Name = Name;
            this.directoryInfo = directoryInfo;
            savefiles = new List<Savefile>();
        }

        public override string ToString()
        {
            return string.Format("{0}", Name);
        }



    }
}

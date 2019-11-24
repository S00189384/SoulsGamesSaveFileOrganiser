using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Test
{
    class Savefile
    {
        public string Name { get; set; }
        public DirectoryInfo directoryInfo { get; set; }

        public Savefile(string Name,DirectoryInfo directoryInfo)
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

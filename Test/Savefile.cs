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
        public DirectoryInfo directoryInfo { get; set; }
        public Segment segment { get; set; }

        public Savefile(string Name,DirectoryInfo directoryInfo,Segment segment)
        {
            this.Name = Name;
            this.directoryInfo = directoryInfo;
            this.segment = segment;
        }

        public override string ToString()
        {
            return string.Format("{0}", Name);
        }
    }
}

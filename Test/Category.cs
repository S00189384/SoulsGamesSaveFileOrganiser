using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    public class Category
    {
        public string Name { get; set; }
        public DirectoryInfo directoryInfo { get; set; }
        public List<Segment> segments { get; set; }

        public Category(string Name, DirectoryInfo directoryInfo)
        {
            this.Name = Name;
            this.directoryInfo = directoryInfo;
            segments = new List<Segment>();
        }

        public override string ToString()
        {
            return string.Format("{0}", Name);
        }
    }
}

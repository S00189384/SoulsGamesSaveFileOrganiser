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
        public DirectoryInfo Directory { get; set; }
        public List<Segment> Segments { get; set; }

        public Category(string Name, DirectoryInfo Directory)
        {
            this.Name = Name;
            this.Directory = Directory;
            Segments = new List<Segment>();
        }

        public override string ToString()
        {
            return string.Format("{0}", Name);
        }
    }
}

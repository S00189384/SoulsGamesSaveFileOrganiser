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
        public static string ClassName = "Segment";
        public string Name { get; set; }
        public DirectoryInfo directoryInfo { get; set; }
        public List<Savefile> savefiles { get; set; }
        public Category Category { get; set; }

        public Segment(string Name,DirectoryInfo directoryInfo,Category Category)
        {
            this.Name = Name;
            this.directoryInfo = directoryInfo;
            this.Category = Category;
            savefiles = new List<Savefile>();
        }

        public override string ToString()
        {
            return string.Format("{0}", Name);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Category
    {
        public string Name { get; set; }
        List<Savefile> savefiles { get; set; }

        public Category(string Name)
        {
            this.Name = Name;
            savefiles = new List<Savefile>();
        }





    }
}

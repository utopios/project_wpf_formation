using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo01_ArbreVisuel.Models
{
    public class DemoClass
    {
        private string? name;
        public int Id { get; set; }
        public string? Name { get => name; }

        public DemoClass(string n) {
            name = n;
        }
    }
}

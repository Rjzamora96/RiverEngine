using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    public class ComponentProperty
    {
        public string Name;
        public string Value;
        public ComponentProperty(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}

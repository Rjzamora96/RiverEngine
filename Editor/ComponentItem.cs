using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor
{
    public struct ComponentItem
    {
        public string Name { get; set; }
        public string Script { get; set; }
        public List<ComponentProperty> Properties { get; set; }
        public override string ToString()
        {
            string result = "{";
            result += "script=\"" + Script + "\",";
            result += "componentName=\"" + Name + "\"";
            for(int i = 0; i < Properties.Count; i++)
            {
                result += "," + Properties[i].Name + "=" + Properties[i].Value;
            }
            result += "}";
            return result;
        }
    }
}

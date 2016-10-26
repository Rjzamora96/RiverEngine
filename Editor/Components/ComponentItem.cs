using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            if(Script != null) result += "script=\"" + Script + "\",";
            result += "componentName=\"" + Name + "\"";
            for(int i = 0; i < Properties.Count; i++)
            {
                result += "," + Properties[i].Name + "=" + Properties[i].Value;
            }
            result += "}";
            return result;
        }

        public void SetProperties(List<string> strings)
        {
            for(int i = 0; i < Properties.Count; i++)
            {
                for(int j = 0; j < strings.Count; j++)
                {
                    Match match = Regex.Match(strings[j], Properties[i].Name + "=(.*)");
                    if (match.Success)
                    {
                        Properties[i].Value = match.Groups[1].ToString();
                    }
                }
            }
        }
    }
}

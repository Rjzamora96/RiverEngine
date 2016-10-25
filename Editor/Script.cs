using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Editor
{
    public class Script : MenuItem
    {
        public string Name { get; set; }
        public FileInfo File { get; set; }
        public bool IsUserDefined { get; set; }
    }
}

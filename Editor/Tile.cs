using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace Editor
{
    public class Tile
    {
        public Rectangle Rect { get; set; }
        public bool IsFilled { get; set; }
        public string Source { get; set; }
        public Point Position { get; set; }
        public Point Origin { get; set; }
        public Point Dimensions { get; set; }
    }
}

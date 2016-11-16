using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Editor
{
    public class TileItem : ListBoxItem
    {
        public string OriginalFile { get; set; }
        public Image Image { get; set; }
        public Point Origin { get; set; }
        public Point Dimensions { get; set; }
        public TileItem() : base()
        {
            Image = new Image();
            Content = Image;
        }
    }
}

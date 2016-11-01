using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Editor
{
    public class PreviewItem
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Rotation { get; set; }
        public double Scale { get; set; }
        public Image Sprite { get; set; }
        public EntityItem Owner { get; set; }
        public void Update()
        {

        }
    }
}

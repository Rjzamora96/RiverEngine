using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

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
            foreach (ComponentItem comp in Owner.Components)
            {
                if (comp.Name.Equals("transform"))
                {
                    foreach (ComponentProperty property in comp.Properties)
                    {
                        if (property.Name.Equals("position"))
                        {
                            Match match = Regex.Match(property.Value, "{(.*),(.*)}");
                            if (match.Success)
                            {
                                double x = 0;
                                double y = 0;
                                if (double.TryParse(match.Groups[1].ToString(), out x)) X = x;
                                if (double.TryParse(match.Groups[2].ToString(), out y)) Y = y;
                            }
                        }
                        else if(property.Name.Equals("rotation"))
                        {
                            double value = 0;
                            if (double.TryParse(property.Value, out value)) Rotation = value;
                        }
                        else if(property.Name.Equals("scale"))
                        {
                            double value = 0;
                            if (double.TryParse(property.Value, out value)) Scale = value;
                        }
                    }
                }
                else if(comp.Name.Equals("sprite"))
                {
                    foreach (ComponentProperty property in comp.Properties)
                    {
                        if (property.Name.Equals("origin"))
                        {
                            Match match = Regex.Match(property.Value, "{(.*),(.*)}");
                            if (match.Success)
                            {
                                double x = 0;
                                double y = 0;
                                if (double.TryParse(match.Groups[1].ToString(), out x)) X = x;
                                if (double.TryParse(match.Groups[2].ToString(), out y)) Y = y;
                            }
                        }
                        else if (property.Name.Equals("sprite"))
                        {
                            Match match = Regex.Match(property.Value, "\"(.*)\"");
                            if (match.Success)
                            {
                                if(File.Exists("..\\..\\..\\RenderEngineDX12\\" + match.Groups[1].ToString()))
                                {
                                    FileInfo file = new FileInfo("..\\..\\..\\RenderEngineDX12\\" + match.Groups[1].ToString());
                                    if(file.Extension.Equals(".png"))
                                    {
                                        Sprite = new Image();
                                        Sprite.Source = new BitmapImage(new Uri(file.FullName, UriKind.Relative));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}

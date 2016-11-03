using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
namespace Editor
{
    public class PreviewItem
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double OriginX { get; set; }
        public double OriginY { get; set; }
        public double Rotation { get; set; }
        public double Scale { get; set; }
        public Image Sprite { get; set; }
        public string FileName { get; set; }
        public EntityItem Owner { get; set; }
        private Point LastMousePosition { get; set; }
        private bool Moving { get; set; }
        public PreviewItem()
        {
            Sprite = new Image();
            Sprite.PreviewMouseMove += MoveItem;
            FileName = "";
        }

        private void MoveItem(object sender, MouseEventArgs args)
        {
            if(args.LeftButton == MouseButtonState.Pressed)
            {
                if(!Moving)
                {
                    LastMousePosition = args.GetPosition(MainWindow.Window.scenePreview);
                    Moving = true;
                }
                else
                {
                    Point mousePosition = args.GetPosition(MainWindow.Window.scenePreview);
                    X += mousePosition.X - LastMousePosition.X;
                    Y += mousePosition.Y - LastMousePosition.Y;
                    LastMousePosition = mousePosition;
                    foreach (ComponentItem comp in Owner.Components)
                    {
                        if (comp.Name.Equals("transform"))
                        {
                            foreach (ComponentProperty property in comp.Properties)
                            {
                                if (property.Name.Equals("position"))
                                {
                                    property.Value = "{" + X + "," + Y + "}";
                                }
                            }
                        }
                    }
                    Owner.Editor.SyncProperties();
                }
            }
            if(args.LeftButton == MouseButtonState.Released)
            {
                Moving = false;
            }
        }
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
                    TransformGroup transform = new TransformGroup();
                    transform.Children.Add(new ScaleTransform(Scale, Scale));
                    transform.Children.Add(new RotateTransform(Rotation));
                    transform.Children.Add(new TranslateTransform((X - OriginX) + MainWindow.CameraPosition.X, (Y - OriginY) + MainWindow.CameraPosition.Y));
                    Sprite.RenderTransform = transform;
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
                                if (double.TryParse(match.Groups[1].ToString(), out x)) OriginX = x;
                                if (double.TryParse(match.Groups[2].ToString(), out y)) OriginY = y;
                            }
                        }
                        else if (property.Name.Equals("sprite"))
                        {
                            Match match = Regex.Match(property.Value, "\"(.*)\"");
                            if (match.Success)
                            {
                                if(File.Exists(MainWindow.AssetPath + match.Groups[1].ToString()))
                                {
                                    FileInfo file = new FileInfo(MainWindow.AssetPath + match.Groups[1].ToString());
                                    if(file.Extension.Equals(".png"))
                                    {
                                        if(!file.FullName.Equals(FileName))
                                        {
                                            BitmapImage image = new BitmapImage(new Uri(file.FullName, UriKind.RelativeOrAbsolute));
                                            Sprite.Source = image;
                                            FileName = file.FullName;
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
}

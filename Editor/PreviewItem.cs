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
        public Point Position {
            get
            {
                Point parentPosition = new Point(0, 0);
                if (Owner.EParent != null) parentPosition = Owner.EParent.Preview.Position;
                double scale = (Owner.EParent != null) ? Owner.EParent.Preview.Scale : 1.0;
                Point unrotatedPoint = new Point((_localPosition.X * scale) + parentPosition.X, (_localPosition.Y * scale) + parentPosition.Y);
                double cs = Math.Cos((Math.PI * Rotation) / 180);
                double sn = Math.Sin((Math.PI * Rotation) / 180);
                return new Point((unrotatedPoint.X * cs) - (unrotatedPoint.Y * sn), (unrotatedPoint.X * sn) + (unrotatedPoint.Y * cs));
            }
        }
        private Point _localPosition;
        public double OriginX { get; set; }
        public double OriginY { get; set; }
        public double Rotation { get { return (Owner.EParent != null) ? _localRotation + Owner.EParent.Preview.Rotation : _localRotation; } }
        private double _localRotation;
        public double Scale {
            get
            {
                return (Owner.EParent != null) ? _localScale * Owner.EParent.Preview.Scale : _localScale;
            }
        }
        private double _localScale;
        public Image Sprite { get; set; }
        public string FileName { get; set; }
        public EntityItem Owner { get; set; }
        private Point LastMousePosition { get; set; }
        private bool Moving { get; set; }
        public PreviewItem()
        {
            Sprite = new Image();
            _localPosition = new Point();
            Sprite.PreviewMouseDown += MoveItem;
            FileName = "";
        }
        private void CheckMouse()
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                if (!Moving)
                {
                    LastMousePosition = Mouse.GetPosition(MainWindow.Window.scenePreview);
                    Moving = true;
                }
                else
                {
                    Point mousePosition = Mouse.GetPosition(MainWindow.Window.scenePreview);
                    _localPosition = new Point(_localPosition.X + (mousePosition.X - LastMousePosition.X), _localPosition.Y + (mousePosition.Y - LastMousePosition.Y));
                    LastMousePosition = mousePosition;
                    foreach (ComponentItem comp in Owner.Components)
                    {
                        if (comp.Name.Equals("transform"))
                        {
                            foreach (ComponentProperty property in comp.Properties)
                            {
                                if (property.Name.Equals("position"))
                                {
                                    property.Value = "{" + _localPosition.X + "," + _localPosition.Y + "}";
                                }
                            }
                        }
                    }
                    Owner.Editor.SyncProperties();
                }
            }
            if (Mouse.LeftButton == MouseButtonState.Released)
            {
                Moving = false;
            }
        }
        private void MoveItem(object sender, MouseEventArgs args)
        {
            CheckMouse();
        }
        public void Update()
        {
            if (Moving) CheckMouse();
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
                                if (double.TryParse(match.Groups[1].ToString(), out x)) _localPosition = new Point(x, _localPosition.Y);
                                if (double.TryParse(match.Groups[2].ToString(), out y)) _localPosition = new Point(_localPosition.X, y);
                            }
                        }
                        else if(property.Name.Equals("rotation"))
                        {
                            double value = 0;
                            if (double.TryParse(property.Value, out value)) _localRotation = value;
                        }
                        else if(property.Name.Equals("scale"))
                        {
                            double value = 0;
                            if (double.TryParse(property.Value, out value)) _localScale = value;
                        }
                    }
                    TransformGroup transform = new TransformGroup();
                    transform.Children.Add(new ScaleTransform(Scale, Scale));
                    transform.Children.Add(new RotateTransform(Rotation));
                    transform.Children.Add(new TranslateTransform((Position.X - OriginX) + MainWindow.CameraPosition.X, (Position.Y - OriginY) + MainWindow.CameraPosition.Y));
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

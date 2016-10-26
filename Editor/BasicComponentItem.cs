using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Editor
{
    public class BasicComponentItem : ListBoxItem
    {
        public ComponentItem Component { get; set; }
        public BasicComponentItem(ComponentItem component)
        {
            Component = component;
            Width = 100;
            StackPanel content = new StackPanel();
            Image image = new Image();
            image.Source = new BitmapImage(new Uri("entity-icon.ico", UriKind.Relative));
            Label label = new Label();
            label.Content = component.Name;
            label.HorizontalAlignment = HorizontalAlignment.Center;
            content.Children.Add(image);
            content.Children.Add(label);
            Content = content;
            MouseMove += MoveAsset;
        }
        private void MoveAsset(object sender, MouseEventArgs e)
        {
            BasicComponentItem item = sender as BasicComponentItem;
            if (item != null && e.LeftButton == MouseButtonState.Pressed)
            {
                DragDrop.DoDragDrop(item, Component, DragDropEffects.All);
            }
        }
    }
}

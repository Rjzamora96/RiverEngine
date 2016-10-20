using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Editor
{
    public class EntityItem : ListBoxItem
    {
        private Label nameLabel;
        public string EName { get { return _name; } set { _name = value; nameLabel.Content = _name; } }
        private string _name;
        public EntityEditor Editor { get; set; }

        public EntityItem() : base()
        {
            nameLabel = new Label();
            EName = "Entity";
            Content = nameLabel;
            ContextMenu cm = new ContextMenu();
            MenuItem rename = new MenuItem();
            rename.Header = "Rename";
            cm.Items.Add(rename);
            ContextMenu = cm;
            Editor = new EntityEditor();
            Editor.Owner = this;
            Selected += DisplayEditor;
        }

        private void DisplayEditor(object sender, RoutedEventArgs e)
        {
            UIElement elementToRemove = null;
            foreach(UIElement element in MainWindow.Window.masterGrid.Children)
            {
                if (element is EntityEditor) elementToRemove = element;
            }
            if(elementToRemove != null)
            {
                MainWindow.Window.masterGrid.Children.Remove(elementToRemove);
            }
            Grid.SetColumn(Editor, 4);
            MainWindow.Window.masterGrid.Children.Add(Editor);
        }
    }
}
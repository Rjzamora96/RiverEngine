using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Editor
{
    public class EntityItem : ListBoxItem
    {
        private Label nameLabel;
        public string EName { get { return _name; } set { _name = value; nameLabel.Content = _name; } }
        public List<ComponentItem> Components { get; set; }
        private string _name;
        public EntityEditor Editor { get; set; }

        public EntityItem() : base()
        {
            Components = new List<ComponentItem>();
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
            MouseMove += MoveEntity;
        }
        private void MoveEntity(object sender, MouseEventArgs e)
        {
            EntityItem item = sender as EntityItem;
            if (item != null && e.LeftButton == MouseButtonState.Pressed)
            {
                DragDrop.DoDragDrop(item, this, DragDropEffects.All);
            }
        }
        public override string ToString()
        {
            string result = "entity={";
            for(int i = 0; i < Components.Count; i++)
            {
                result += Components[i].ToString();
                if (i < Components.Count - 1) result += ",";
            }
            result += "}";
            return result;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Editor
{
    public class EntityItem : ListBoxItem
    {
        Label nameLabel;
        public string Name { get { return _name } set { _name = value; nameLabel.Content = _name; } }
        private string _name;
        public EntityItem() : base()
        {
            nameLabel = new Label();
            Name = "Entity";
            Content = nameLabel;
            ContextMenu cm = new ContextMenu();
            MenuItem rename = new MenuItem();
            rename.Header = "Rename";
            cm.Items.Add(rename);
            ContextMenu = cm;
        }
    }
}
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
        public EntityItem() : base()
        {
            Label name = new Label();
            name.Content = "Entity";
            Content = name;
        }
    }
}

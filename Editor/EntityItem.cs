using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Editor
{
    public class EntityItem : ListBoxItem
    {
        private Label nameLabel;
        public string EName { get { return _name; } set { _name = value; nameLabel.Content = _name; } }
        public string Tags { get; set; }
        public List<ComponentItem> Components { get; set; }
        private string _name;
        public EntityEditor Editor { get; set; }
        public PreviewItem Preview { get; set; }
        public EntityItem EParent { get; set; }
        public List<EntityItem> Children { get; set; }
        private Grid _contentDisplay;
        private Separator _depthDisplay;
        private Button _toggleShow;
        private bool _showingChildren = false;
        public EntityItem() : base()
        {
            Components = new List<ComponentItem>();
            nameLabel = new Label();
            _contentDisplay = new Grid();
            _contentDisplay.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.0, GridUnitType.Auto) });
            _contentDisplay.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.0, GridUnitType.Auto) });
            _contentDisplay.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.1, GridUnitType.Star) });
            _toggleShow = new Button();
            _toggleShow.Background = Brushes.Transparent;
            _toggleShow.BorderBrush = Brushes.Transparent;
            _toggleShow.BorderThickness = new Thickness(0);
            _toggleShow.Padding = new Thickness(-4);
            _toggleShow.Content = "+";
            _toggleShow.Width = 30;
            _toggleShow.HorizontalAlignment = HorizontalAlignment.Right;
            _toggleShow.Click += ToggleShowChildren;
            _depthDisplay = new Separator();
            _depthDisplay.Background = Brushes.Transparent;
            Grid.SetColumn(_depthDisplay, 0);
            Grid.SetColumn(nameLabel, 1);
            Grid.SetColumn(_toggleShow, 2);
            _contentDisplay.Children.Add(_depthDisplay);
            _contentDisplay.Children.Add(_toggleShow);
            _contentDisplay.Children.Add(nameLabel);
            EName = "Entity";
            Tags = "{}";
            Content = _contentDisplay;
            ContextMenu cm = new ContextMenu();
            MenuItem rename = new MenuItem();
            rename.Header = "Rename";
            cm.Items.Add(rename);
            ContextMenu = cm;
            Editor = new EntityEditor();
            Editor.Owner = this;
            Preview = new PreviewItem();
            Preview.Owner = this;
            Selected += DisplayEditor;
            MouseMove += MoveEntity;
            AllowDrop = true;
            Drop += AddChild;
            MainWindow.Window.scenePreview.Children.Add(Preview.Sprite);
            EParent = null;
            Children = new List<EntityItem>();

        }
        private void MoveEntity(object sender, MouseEventArgs e)
        {
            EntityItem item = sender as EntityItem;
            if (item != null && e.LeftButton == MouseButtonState.Pressed)
            {
                DragDrop.DoDragDrop(item, this, DragDropEffects.All);
            }
        }
        private void ToggleShowChildren(object sender, EventArgs e)
        {
            _showingChildren = !_showingChildren;
            if (_showingChildren) _toggleShow.Content = "-";
            else _toggleShow.Content = "+";
            ReloadScene();
        }
        public void AddChild(EntityItem item)
        {
            if (item.EParent != null)
            {
                EParent.Children.Remove(item);
            }
            item.EParent = this;
            Children.Add(item);
            ReloadScene();
        }
        private void AddChild(object sender, DragEventArgs e)
        {
            EntityItem parent = sender as EntityItem;
            if(parent != null)
            {
                if(e.Data.GetDataPresent(typeof(EntityItem)))
                {
                    EntityItem entity = (EntityItem)e.Data.GetData(typeof(EntityItem));
                    parent.AddChild(entity);
                }
            }
        }
        private void ReloadScene()
        {
            List<EntityItem> sceneClone = new List<EntityItem>();
            foreach (UIElement element in MainWindow.Window.sceneDisplay.Items)
            {
                EntityItem item = element as EntityItem;
                if (item != null)
                {
                    if (item.EParent == null)
                    {
                        sceneClone.Add(item);
                        item._depthDisplay.Width = 0;
                        item.AddChildrenToList(sceneClone, 1);
                    }
                }
            }
            MainWindow.Window.sceneDisplay.Items.Clear();
            foreach (EntityItem item in sceneClone) MainWindow.Window.sceneDisplay.Items.Add(item);
        }
        public void Update()
        {
            Preview.Update();
            foreach (EntityItem child in Children) child.Update();
        }
        private void AddChildrenToList(List<EntityItem> curretList, int depth)
        {
            foreach(EntityItem item in Children)
            {
                if(!curretList.Contains(item) && _showingChildren)
                {
                    item._depthDisplay.Width = 20 * depth;
                    curretList.Add(item);
                    item.AddChildrenToList(curretList, depth+1);
                }
            }
        }
        public override string ToString()
        {
            string result = "{";
            result += "name=\"" + EName + "\",";
            result += "tags=" + Tags + ",";
            result += "components={";
            for(int i = 0; i < Components.Count; i++)
            {
                result += Components[i].ToString();
                result += ",";
            }
            result = result.TrimEnd(',') + "}" + ",";
            result += "children={";
            for(int i = 0; i < Children.Count; i++)
            {
                result += Children[i].ToString();
                result += ",";
            }
            result = result.TrimEnd(',') + "}" + "}";
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
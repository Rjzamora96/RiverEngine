using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Editor
{
    public class AssetItem : ListBoxItem
    {
        private FileInfo _file;
        private TextBox _fileName;
        public TabControl EditorTabs { get; set; }
        public AssetItem(FileInfo file) : base()
        {
            _file = file;
            Width = 100;
            StackPanel content = new StackPanel();
            Image fileImage = new Image();
            if (_file.Extension.Equals(".lua")) fileImage.Source = new BitmapImage(new Uri("lua-icon.png", UriKind.Relative));
            else if (_file.Extension.Equals(".entity")) fileImage.Source = new BitmapImage(new Uri("entity-icon.ico", UriKind.Relative));
            else if (_file.Extension.Equals(".scene")) fileImage.Source = new BitmapImage(new Uri("scene-icon.png", UriKind.Relative));
            else if (_file.Extension.Equals(".png")) fileImage.Source = new BitmapImage(new Uri(_file.FullName, UriKind.RelativeOrAbsolute));
            fileImage.Height = 100.0;
            fileImage.MaxHeight = 100.0;
            TextBox fileName = new TextBox();
            fileName.BorderThickness = new Thickness(0);
            fileName.TextWrapping = TextWrapping.Wrap;
            fileName.IsReadOnly = true;
            fileName.Text = Path.GetFileNameWithoutExtension(file.Name);
            fileName.HorizontalAlignment = HorizontalAlignment.Center;
            fileName.KeyDown += SubmitRename;
            _fileName = fileName;
            content.Children.Add(fileImage);
            content.Children.Add(fileName);
            Content = content;
            MouseDoubleClick += OpenScript;
            MouseDoubleClick += OpenScene;
            ContextMenu cm = new ContextMenu();
            MenuItem menuItem = new MenuItem();
            menuItem.Header = "Rename";
            menuItem.Click += RenameFile;
            if(_file.Extension.Equals(".scene"))
            {
                MenuItem setAsStart = new MenuItem();
                setAsStart.Header = "Set as Initial Scene";
                setAsStart.Click += SetStartScene;
                cm.Items.Add(setAsStart);
            }
            cm.Items.Add(menuItem);
            ContextMenu = cm;
            MouseMove += MoveAsset;
        }
        private void SetStartScene(object sender, EventArgs e)
        {
            foreach(ComponentProperty property in MainWindow.Window.GameProperties)
            {
                if(property.Name.Equals("startScene"))
                {
                    property.Value = "\"" + _file.Name + "\"";
                }
            }
            MainWindow.Window.SaveProperties();
        }
        private void MoveAsset(object sender, MouseEventArgs e)
        {
            AssetItem item = sender as AssetItem;
            if(item != null && e.LeftButton == MouseButtonState.Pressed)
            {
                DragDrop.DoDragDrop(item, _file, DragDropEffects.All);
            }
        }
        private void SubmitRename(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                File.Move(_file.FullName, _file.DirectoryName + "//" + _fileName.Text + _file.Extension);
                _file = new FileInfo(_file.DirectoryName + "//" + _fileName.Text + _file.Extension);
            }
        }

        private void RenameFile(object sender, EventArgs e)
        {
            _fileName.IsReadOnly = false;
            Keyboard.Focus(_fileName);
        }
        public EntityItem EntityFromScript(string entityScript)
        {
            EntityItem entity = new EntityItem();
            Match eMatch = Regex.Match(entityScript, "{(.*)}");
            Group entityGroup = eMatch.Groups[0];  //AllComponents
            string entityString = entityGroup.ToString();
            List<string> entityProperties = MainWindow.Window.DivideStrings(entityString);
            Match eNameMatch = Regex.Match(entityProperties[0], "name=\"(.*?)\"");
            entity.EName = eNameMatch.Groups[1].ToString();
            Match tagsMatch = Regex.Match(entityProperties[1], "tags=(.*)");
            entity.Tags = tagsMatch.Groups[1].ToString();
            Match cMatch = Regex.Match(entityProperties[2], "{(.*)}");
            Group compGroup = cMatch.Groups[1];  //AllComponents
            string compString = compGroup.ToString();
            int level = 0;
            List<string> componentStrings = new List<string>();
            string propertyString = "";
            for (int i = 0; i < compString.Length; i++)
            {
                if (compString[i].Equals('{')) level++;
                if (level != 0) propertyString += compString[i];
                if (compString[i].Equals('}')) level--;
                if (level == 0 && !compString[i].Equals(','))
                {
                    componentStrings.Add(propertyString);
                    propertyString = "";
                }
            }
            for (int i = 0; i < componentStrings.Count; i++)
            {
                string current = componentStrings[i];
                List<string> savedValues = MainWindow.Window.DivideStrings(current);
                Match scriptMatch = Regex.Match(current, "script=\"(.*?)\"");
                if (scriptMatch.Success)
                {
                    FileInfo compFile = new FileInfo(MainWindow.AssetPath + scriptMatch.Groups[1].ToString());
                    entity.Editor.AddComponent(compFile);
                    entity.Editor.Owner.Components.Last().SetProperties(savedValues);
                }
                else
                {
                    Match nameMatch = Regex.Match(current, "componentName=\"(.*?)\"");
                    if (nameMatch.Groups[1].ToString().Equals("transform"))
                    {
                        ComponentItem transformItem = new ComponentItem
                        {
                            Name = "transform",
                            Properties = new List<ComponentProperty>
                                            {
                                                new ComponentProperty("position","{0,0}"),
                                                new ComponentProperty("rotation","0.0"),
                                                new ComponentProperty("scale","1.0")
                                            }
                        };
                        entity.Editor.AddComponent(transformItem);
                        transformItem.SetProperties(savedValues);
                    }
                    else if (nameMatch.Groups[1].ToString().Equals("sprite"))
                    {
                        ComponentItem transformItem = new ComponentItem
                        {
                            Name = "sprite",
                            Properties = new List<ComponentProperty>
                                            {
                                                new ComponentProperty("sprite","\"\""),
                                                new ComponentProperty("origin","{0,0}"),
                                                new ComponentProperty("rectangle","{0,0,0,0}")
                                            }
                        };
                        entity.Editor.AddComponent(transformItem);
                        transformItem.SetProperties(savedValues);
                    }
                }
            }
            Match childrenMatch = Regex.Match(entityProperties[3], "children=(.*)");
            Group childGroup = childrenMatch.Groups[1];  //AllComponents
            string childString = childGroup.ToString();
            List<string> children = MainWindow.Window.DivideStrings(childString);
            foreach(string child in children)
            {
                if (child.Equals("")) continue;
                EntityItem item = EntityFromScript(child);
                entity.AddChild(item);
            }
            entity.Editor.SyncProperties();
            return entity;
        }
        public void OpenScene(object sender, RoutedEventArgs e)
        {
            if (!_file.Extension.Equals(".scene")) return;
            MainWindow.Window.SceneFile = _file.FullName;
            MainWindow.Window.sceneDisplay.Items.Clear();
            MainWindow.Window.scenePreview.Children.Clear();
            string scene = File.ReadAllText(_file.FullName);
            List<string> sceneArray = MainWindow.Window.DivideStrings(scene);
            List<string> entities = MainWindow.Window.DivideStrings(sceneArray[0]);
            List<string> tiles = MainWindow.Window.DivideStrings(sceneArray[1]);
            foreach (string entityScript in entities)
            {
                if (entityScript.Equals("")) continue;
                MainWindow.Window.sceneDisplay.Items.Add(EntityFromScript(entityScript));
            }
            List<EntityItem> tileItems = new List<EntityItem>();
            foreach (string tileScript in tiles)
            {
                if (tileScript.Equals("")) continue;
                tileItems.Add(EntityFromScript(tileScript));
            }
            MainWindow.Window.LoadTiles(tileItems);
        }
        public void OpenScript(object sender, RoutedEventArgs e)
        {
            if (!_file.Extension.Equals(".lua")) return;
            TextEditor activeEditor = new TextEditor();
            activeEditor.OpenFile(_file.FullName);
            Grid headerContainer = new Grid();
            headerContainer.ColumnDefinitions.Add(new ColumnDefinition());
            headerContainer.ColumnDefinitions.Add(new ColumnDefinition());
            Button closeButton = new Button();
            closeButton.Content = "x";
            closeButton.Width = 20;
            closeButton.Height = 20;
            closeButton.Padding = new Thickness(0);
            closeButton.VerticalAlignment = VerticalAlignment.Center;
            closeButton.VerticalContentAlignment = VerticalAlignment.Center;
            closeButton.HorizontalContentAlignment = HorizontalAlignment.Center;
            closeButton.FontSize = 12;
            closeButton.Click += CloseTab;
            Label panelLabel = new Label();
            panelLabel.Content = _file.Name;
            Grid.SetColumn(panelLabel, 0);
            Grid.SetRow(panelLabel, 0);
            Grid.SetColumn(closeButton, 1);
            Grid.SetRow(closeButton, 0);
            headerContainer.Children.Add(closeButton);
            headerContainer.Children.Add(panelLabel);
            activeEditor.Header = headerContainer;
            EditorTabs.Items.Add(activeEditor);
            EditorTabs.SelectedItem = activeEditor;
        }
        private void CloseTab(object sender, RoutedEventArgs e)
        {
            var target = (FrameworkElement)sender;
            while (!(target is TabItem)) target = (FrameworkElement)target.Parent;
            TabItem tabItem = (TabItem)target;
            tabItem.Template = null;
            EditorTabs.Items.Remove(tabItem);
        }
    }
}

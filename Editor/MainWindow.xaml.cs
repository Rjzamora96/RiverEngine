﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Window { get; set; }
        public List<Script> Scripts { get; set; }
        private DispatcherTimer timer { get; set; }
        public static Point CameraPosition { get; set; }
        public Point LastMousePoint { get; set; }
        public static bool Panning { get; set; }
        public string SceneFile { get; set; }
        public List<Tile> _map;
        private List<Line> _mapGrid;
        private Point _mapSize { get; set; }
        public int TileSize { get; set; }
        public List<ComponentProperty> GameProperties { get; set; }
        public static string AssetPath { get; set; }
        public static string DirectoryPath { get; set; }
        public MainWindow()
        {
            MainWindow.Window = this;
            AssetPath = "..\\..\\..\\RenderEngineDX12\\Assets\\";
            DirectoryPath = "..\\..\\..\\RenderEngineDX12\\";
            SceneFile = "";
            GameProperties = new List<ComponentProperty>
            {
                new ComponentProperty("startScene", "")
            };
            Scripts = new List<Script>();
            InitializeComponent();
            UpdateAssetDisplay();
            ContextMenu cm = new ContextMenu();
            MenuItem create = new MenuItem();
            create.Header = "Create Entity";
            create.Click += CreateEntity;
            cm.Items.Add(create);
            sceneDisplay.ContextMenu = cm;
            assetDisplay.AllowDrop = true;
            assetDisplay.Drop += DropEntity;
            sceneDisplay.AllowDrop = true;
            sceneDisplay.Drop += DropPrefab;
            scenePreview.PreviewMouseMove += PanScene;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromTicks(0);
            timer.Tick += UpdatePreview;
            timer.Start();
            _mapGrid = new List<Line>();
            _map = new List<Tile>();
            _mapSize = new Point(800, 600);
            TileSize = 1;
            DrawMapBorder();
        }
        private void DrawMapBorder()
        {
            foreach(Tile square in _map)
            {
                Canvas.SetTop(square.Rect, CameraPosition.Y);
                Canvas.SetLeft(square.Rect, CameraPosition.X);
            }
        }
        public void SaveProperties()
        {
            string result = "properties={";
            foreach(ComponentProperty property in GameProperties)
            {
                result += property.Name + "=" + property.Value + ",";
            }
            result = result.TrimEnd(',') + "}";
            File.WriteAllText(AssetPath + "Properties.assets", result);
        }
        private void PanScene(object sender, MouseEventArgs args)
        {
            if(args.RightButton == MouseButtonState.Pressed)
            {
                if(!Panning)
                {
                    LastMousePoint = args.GetPosition(sender as IInputElement);
                    Panning = true;
                }
                else
                {
                    Point current = args.GetPosition(sender as IInputElement);
                    CameraPosition = new Point((current.X - LastMousePoint.X) + CameraPosition.X, (current.Y - LastMousePoint.Y) + CameraPosition.Y);
                    LastMousePoint = current;
                }
            }
            else if(args.RightButton == MouseButtonState.Released && Panning)
            {
                Panning = false;
            }
        }
        private void UpdatePreview(object sender, EventArgs e)
        {
            DrawMapBorder();
            foreach (UIElement element in sceneDisplay.Items)
            {
                EntityItem entity = element as EntityItem;
                if(entity != null)
                {
                    if(entity.EParent == null) entity.Update();
                }
            }
        }
        private void DropPrefab(object sender, DragEventArgs e)
        {
            ListBox sceneBox = sender as ListBox;
            if (sceneBox != null)
            {
                if(e.Data.GetDataPresent(typeof(FileInfo)))
                {
                    FileInfo file = (FileInfo)e.Data.GetData(typeof(FileInfo));
                    if(file != null)
                    {
                        if(file.Extension.Equals(".entity"))
                        {
                            EntityItem entity = new EntityItem();
                            sceneDisplay.Items.Add(entity);
                            string entityScript = File.ReadAllText(file.FullName);
                            Match eMatch = Regex.Match(entityScript, "{(.*)}");
                            Group entityGroup = eMatch.Groups[0];  //AllComponents
                            string entityString = entityGroup.ToString();
                            List<string> entityProperties = DivideStrings(entityString);
                            Match eNameMatch = Regex.Match(entityProperties[0], "name=\"(.*?)\"");
                            entity.EName = eNameMatch.Groups[1].ToString();
                            Match tagsMatch = Regex.Match(entityProperties[1], "tags=(.*)");
                            entity.Tags = tagsMatch.Groups[1].ToString();
                            Match cMatch = Regex.Match(entityProperties[2], "{(.*)}");
                            Group  compGroup = cMatch.Groups[1];  //AllComponents
                            string compString = compGroup.ToString();
                            int level = 0;
                            List<string> componentStrings = new List<string>();
                            string propertyString = "";
                            for(int i = 0; i < compString.Length; i++)
                            {
                                if (compString[i].Equals('{')) level++;
                                if(level != 0) propertyString += compString[i];
                                if (compString[i].Equals('}')) level--;
                                if (level == 0 && !compString[i].Equals(','))
                                {
                                    componentStrings.Add(propertyString);
                                    propertyString = "";
                                }
                            }
                            for(int i = 0; i < componentStrings.Count; i++)
                            {
                                string current = componentStrings[i];
                                List<string> savedValues = DivideStrings(current);
                                Match scriptMatch = Regex.Match(current, "script=\"(.*?)\"");
                                if (scriptMatch.Success)
                                {
                                    FileInfo compFile = new FileInfo(AssetPath + scriptMatch.Groups[1].ToString());
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
                                                new ComponentProperty("origin","{0,0}")
                                            }
                                        };
                                        entity.Editor.AddComponent(transformItem);
                                        transformItem.SetProperties(savedValues);
                                    }
                                }
                            }
                            entity.Editor.SyncProperties();
                            //{(?>[^{}]+|(?R))*}
                        }
                    }
                }
            }
        }
        public List<string> DivideStrings(string value)
        {
            List<string> result = new List<string>();
            int level = 0;
            string currentString = "";
            for(int i = 0; i < value.Length; i++)
            {
                if (value[i].Equals('}')) level--;
                if (level > 1 || (level == 1 && !value[i].Equals(','))) currentString += value[i]; 
                if (value[i].Equals('{')) level++;
                if(level == 1 && value[i].Equals(','))
                {
                    result.Add(currentString);
                    currentString = "";
                }
            }
            result.Add(currentString);
            return result;
        }
        private void DropEntity(object sender, DragEventArgs e)
        {
            ListBox assetBox = sender as ListBox;
            if (assetBox != null)
            {
                if (e.Data.GetDataPresent(typeof(EntityItem)))
                {
                    EntityItem entity = (EntityItem)e.Data.GetData(typeof(EntityItem));
                    if(entity != null)
                    {
                        File.WriteAllText(AssetPath + entity.EName + ".entity", "entity=" + entity.ToString());
                        UpdateAssetDisplay();
                    }
                }
            }
        }
        private void CreateEntity(object sender, EventArgs e)
        {
            EntityItem entity = new EntityItem();
            sceneDisplay.Items.Add(entity);
        }

        private void UpdateAssetDisplay()
        {
            assetDisplay.Items.Clear();
            basicsDisplay.Items.Clear();
            Scripts.Clear();
            DirectoryInfo dir = new DirectoryInfo(AssetPath);
            IEnumerable<FileInfo> files = dir.EnumerateFiles();
            string spriteList = "sprites={";
            foreach (FileInfo file in files)
            {
                if (file.Extension != ".lua" && file.Extension != ".entity" && file.Extension != ".png" && file.Extension != ".scene") continue;
                if (file.Extension == ".lua")
                {
                    Script script = new Script
                    {
                        File = file,
                        Name = System.IO.Path.GetFileNameWithoutExtension(file.Name),
                        IsUserDefined = false
                    };
                    script.Header = script.Name;
                    Scripts.Add(script);
                }
                if(file.Extension == ".png")
                {
                    spriteList += "\"" + file.Name + "\"" + ",";
                }
                AssetItem item = new AssetItem(file);
                item.EditorTabs = editorTabs;
                assetDisplay.Items.Add(item);
            }
            spriteList = spriteList.TrimEnd(',') + "}";
            File.WriteAllText(AssetPath + "\\Sprites.assets", spriteList);
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
            BasicComponentItem transform = new BasicComponentItem(transformItem);
            basicsDisplay.Items.Add(transform);
            ComponentItem spriteItem = new ComponentItem
            {
                Name = "sprite",
                Properties = new List<ComponentProperty>
                {
                    new ComponentProperty("sprite","\"\""),
                    new ComponentProperty("origin","{0,0}"),
                    new ComponentProperty("rectangle","{0,0,0,0}")
                }
            };
            BasicComponentItem sprite = new BasicComponentItem(spriteItem);
            basicsDisplay.Items.Add(sprite);
            ComponentItem boxColliderItem = new ComponentItem
            {
                Name = "boxCollider",
                Properties = new List<ComponentProperty>
                {
                    new ComponentProperty("rectangle","{0,0,0,0}")
                }
            };
            BasicComponentItem boxCollider = new BasicComponentItem(boxColliderItem);
            basicsDisplay.Items.Add(boxCollider);
        }

        private void OpenFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Script files (*.lua)|*.lua";
            if (openFileDialog.ShowDialog() == true)
            {
                TextEditor activeEditor = new TextEditor();
                activeEditor.OpenFile(openFileDialog.FileName);
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
                panelLabel.Content = openFileDialog.SafeFileName;
                Grid.SetColumn(panelLabel, 0);
                Grid.SetRow(panelLabel, 0);
                Grid.SetColumn(closeButton, 1);
                Grid.SetRow(closeButton, 0);
                headerContainer.Children.Add(closeButton);
                headerContainer.Children.Add(panelLabel);
                activeEditor.Header = headerContainer;
                editorTabs.Items.Add(activeEditor);
                editorTabs.SelectedItem = activeEditor;
            }
        }
        private void CloseTab(object sender, RoutedEventArgs e)
        {
            var target = (FrameworkElement)sender;
            while (!(target is TabItem)) target = (FrameworkElement)target.Parent;
            TabItem tabItem = (TabItem)target;
            tabItem.Template = null;
            editorTabs.Items.Remove(tabItem);
        }
        private void SaveFile(object sender, RoutedEventArgs e)
        {
            if(editorTabs.SelectedItem != null)
            {
                TextEditor activeEditor = (TextEditor)editorTabs.SelectedItem;
                activeEditor.SaveFile();
            }
        }
        private void PlayGame(object sender, RoutedEventArgs e)
        {
            Process game = new Process();
            game.StartInfo.FileName = DirectoryPath + "RenderEngineDX12.exe";
            game.StartInfo.UseShellExecute = false;
            game.StartInfo.WorkingDirectory = System.IO.Path.GetFullPath(AssetPath);
            game.Start();
        }
        private void SaveScene(object sender, RoutedEventArgs e)
        {
            string result = "scene={entities={";
            foreach (UIElement element in sceneDisplay.Items)
            {
                EntityItem entity = element as EntityItem;
                if (entity != null && entity.EParent == null)
                {
                    result += entity.ToString() + ",";
                }
            }
            result = result.TrimEnd(',') + "}";
            result += ",map={";
            foreach (Tile tiles in _map)
            {
                EntityItem entity = new EntityItem();
                entity.EName = "Tile";
                entity.Tags = "{\"Tile\"}";
                ComponentItem transformItem = new ComponentItem
                {
                    Name = "transform",
                    Properties = new List<ComponentProperty>
                    {
                        new ComponentProperty("position","{" + tiles.Position.X +  "," + tiles.Position.Y + "}"),
                        new ComponentProperty("rotation","0.0"),
                        new ComponentProperty("scale","1.0")
                    }
                };
                entity.Components.Add(transformItem);
                ComponentItem spriteItem = new ComponentItem
                {
                    Name = "sprite",
                    Properties = new List<ComponentProperty>
                    {
                        new ComponentProperty("sprite","\"" + tiles.Source + "\""),
                        new ComponentProperty("origin","{0,0}"),
                        new ComponentProperty("rectangle", "{" + tiles.Origin.X + "," + tiles.Origin.Y + "," + tiles.Dimensions.X + "," + tiles.Dimensions.Y + "}")
                    }
                };
                entity.Components.Add(spriteItem);
                result += entity.ToString() + ",";
            }
            result = result.TrimEnd(',') + "}}";
            if (SceneFile.Equals(""))
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Scene files (*.scene)|*.scene";
                if (saveFileDialog.ShowDialog() == true)
                {
                    File.WriteAllText(saveFileDialog.FileName, result);
                    SceneFile = saveFileDialog.FileName;
                    UpdateAssetDisplay();
                }
            }
            else
            {
                File.WriteAllText(SceneFile, result);
                UpdateAssetDisplay();
            }
        }
        private void NewFile(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Script files (*.lua)|*.lua";
            if(saveFileDialog.ShowDialog() == true)
            {
                Uri uri = new Uri("BaseComponent.lua", UriKind.RelativeOrAbsolute);
                File.WriteAllText(saveFileDialog.FileName, 
@"function Initialize(self)
end

function Update(self, dt)
end

function OnMessage(self, id, message, sender)
end");
                TextEditor activeEditor = new TextEditor();
                activeEditor.OpenFile(saveFileDialog.FileName);
                UpdateAssetDisplay();
            }
        }
        private void NewScene(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Scene files (*.scene)|*.scene";
            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, "scene={entities={},map={}}");
                SceneFile = saveFileDialog.FileName;
                MainWindow.Window.sceneDisplay.Items.Clear();
                MainWindow.Window.scenePreview.Children.Clear();
                UpdateAssetDisplay();
            }
        }

        private void ApplyMapChanges(object sender, RoutedEventArgs e)
        {
            int newMapX = 0;
            int newMapY = 0;
            if(int.TryParse(mapSizeX.Text, out newMapX) && int.TryParse(mapSizeY.Text, out newMapY)) _mapSize = new Point(newMapX, newMapY);
            int newTileSize = 0;
            if (int.TryParse(tileSize.Text, out newTileSize)) TileSize = newTileSize;
            foreach (Tile square in _map)
            {
                scenePreview.Children.Remove(square.Rect);
            }
            _map.Clear();
            for(int i = 0; i < _mapSize.X; i++)
            {
                for(int j = 0; j < _mapSize.Y; j++)
                {
                    Rectangle square = new Rectangle();
                    square.Width = TileSize;
                    square.Height = TileSize;
                    TransformGroup transform = new TransformGroup();
                    transform.Children.Add(new TranslateTransform(i * TileSize, j * TileSize));
                    square.RenderTransform = transform;
                    square.Stroke = Brushes.Black;
                    square.Fill = Brushes.Gray;
                    square.StrokeThickness = 1;
                    square.MouseEnter += OnMouseEnterTile;
                    square.MouseEnter += OnMouseDownTile;
                    square.MouseLeave += OnMouseLeaveTile;
                    square.MouseDown += OnMouseDownTile;
                    Canvas.SetZIndex(square, -1000);
                    scenePreview.Children.Add(square);
                    Tile tile = new Tile { Rect = square, Position = new Point(i * TileSize, j * TileSize), Origin = new Point(0, 0), Dimensions = new Point(TileSize, TileSize), Source = "", IsFilled = false };
                    _map.Add(tile);
                }
            }
        }
        private void OnMouseEnterTile(object sender, MouseEventArgs e)
        {
            Rectangle square = sender as Rectangle;
            if(square != null)
            {
                TileSetTab tileSet = tileSets.SelectedItem as TileSetTab;
                if (tileSet != null)
                {
                    if(tileSet.SelectedTile != null)
                    {
                        Tile tile = _map.Where(x => x.Rect == sender as Rectangle).First();
                        if(!tile.IsFilled)
                        {
                            square.StrokeThickness = 0;
                            square.Fill = new ImageBrush(tileSet.SelectedTile.Image.Source);
                        }
                    }
                }
            }
        }
        private void OnMouseDownTile(object sender, MouseEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                TileSetTab tileSet = tileSets.SelectedItem as TileSetTab;
                if (tileSet != null)
                {
                    if (tileSet.SelectedTile != null)
                    {
                        Tile tile = _map.Where(x => x.Rect == sender as Rectangle).FirstOrDefault();
                        tile.Rect.StrokeThickness = 0;
                        tile.Rect.Fill = new ImageBrush(tileSet.SelectedTile.Image.Source);
                        tile.IsFilled = true;
                        tile.Source = tileSet.Source;
                        tile.Origin = tileSet.SelectedTile.Origin;
                        tile.Dimensions = tileSet.SelectedTile.Dimensions;
                    }
                }
            }
        }
        private void OnMouseLeaveTile(object sender, MouseEventArgs e)
        {
            Rectangle square = sender as Rectangle;
            if (square != null)
            {
                Tile tile = _map.Where(x => x.Rect == sender as Rectangle).FirstOrDefault();
                if(!tile.IsFilled)
                {
                    square.Fill = Brushes.Gray;
                    square.StrokeThickness = 1;
                }
            }
        }
        private void AddTileSet(object sender, RoutedEventArgs e)
        {
            TileSetTab item = new TileSetTab();
            item.Header = "Set " + tileSets.Items.Count;
            tileSets.Items.Insert(tileSets.Items.Count - 1, item);
            tileSets.SelectedItem = item;
        }

        private void NewProject(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "River files (*.river)|*.river";
            if (saveFileDialog.ShowDialog() == true)
            {
                string directory = System.IO.Path.GetDirectoryName(saveFileDialog.FileName) + "\\" + System.IO.Path.GetFileNameWithoutExtension(saveFileDialog.SafeFileName) + "\\";
                Directory.CreateDirectory(directory);
                DirectoryPath = directory;
                AssetPath = directory + "Assets\\";
                string[] properties =
                {
                    DirectoryPath,
                    AssetPath
                };
                Directory.CreateDirectory(AssetPath);
                File.WriteAllLines(directory + saveFileDialog.SafeFileName, properties);
                File.Copy("..\\..\\..\\Release\\RenderEngineDX12.exe", directory + /*System.IO.Path.GetFileNameWithoutExtension(saveFileDialog.SafeFileName) + */"RenderEngineDX12.exe");
                MainWindow.Window.sceneDisplay.Items.Clear();
                MainWindow.Window.scenePreview.Children.Clear();
                UpdateAssetDisplay();
            }
        }
        private void OpenProject(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "River files (*.river)|*.river";
            if (openFileDialog.ShowDialog() == true)
            {
                DirectoryPath = System.IO.Path.GetDirectoryName(openFileDialog.FileName) + "\\";
                AssetPath = DirectoryPath + "Assets\\";
                MainWindow.Window.sceneDisplay.Items.Clear();
                MainWindow.Window.scenePreview.Children.Clear();
                UpdateAssetDisplay();
            }
        }
        public void LoadTiles(List<EntityItem> tiles)
        {
            foreach (Tile square in _map)
            {
                scenePreview.Children.Remove(square.Rect);
            }
            _map.Clear();
            for (int i = 0; i < tiles.Count; i++)
            {
                Point position = new Point();
                double[] rectangle = { 0, 0, 0, 0 };
                bool filled = false;
                string source = "\"\"";
                foreach (ComponentItem component in tiles[i].Components)
                {
                    if(component.Name.Equals("transform"))
                    {
                        foreach(ComponentProperty property in component.Properties)
                        {
                            if(property.Name.Equals("position"))
                            {
                                Match match = Regex.Match(property.Value, "{(.*),(.*)}");
                                if (match.Success)
                                {
                                    double x = 0;
                                    double y = 0;
                                    if (double.TryParse(match.Groups[1].ToString(), out x)) position = new Point(x, position.Y);
                                    if (double.TryParse(match.Groups[2].ToString(), out y)) position = new Point(position.X, y);
                                }
                            }
                        }
                    }
                    else if (component.Name.Equals("sprite"))
                    {
                        foreach (ComponentProperty property in component.Properties)
                        {
                            if (property.Name.Equals("rectangle"))
                            {
                                Match match = Regex.Match(property.Value, "{(.*),(.*),(.*),(.*)}");
                                if (match.Success)
                                {
                                    double x = 0;
                                    double y = 0;
                                    double dimensionX = 0;
                                    double dimensionY = 0;
                                    if (double.TryParse(match.Groups[1].ToString(), out x)) rectangle[0] = x;
                                    if (double.TryParse(match.Groups[2].ToString(), out y)) rectangle[1] = y;
                                    if (double.TryParse(match.Groups[3].ToString(), out dimensionX)) rectangle[2] = dimensionX;
                                    if (double.TryParse(match.Groups[4].ToString(), out dimensionY)) rectangle[3] = dimensionY;
                                }
                            }
                            else if (property.Name.Equals("sprite"))
                            {
                                Match match = Regex.Match(property.Value, "\"(.*)\"");
                                if (match.Success)
                                {
                                    if (!match.Groups[0].ToString().Equals("\"\""))
                                    {
                                        source = match.Groups[1].ToString();
                                        filled = true;
                                    }
                                }
                            }
                        }
                    }
                }
                Rectangle square = new Rectangle();
                square.Width = rectangle[2];
                square.Height = rectangle[3];
                TileSize = (int)square.Width;
                TransformGroup transform = new TransformGroup();
                transform.Children.Add(new TranslateTransform(position.X, position.Y));
                square.RenderTransform = transform;
                square.Stroke = Brushes.Black;
                square.Fill = Brushes.Gray;
                square.StrokeThickness = 1;
                square.MouseEnter += OnMouseEnterTile;
                square.MouseEnter += OnMouseDownTile;
                square.MouseLeave += OnMouseLeaveTile;
                square.MouseDown += OnMouseDownTile;
                Canvas.SetZIndex(square, -1000);
                scenePreview.Children.Add(square);
                if(filled)
                {
                    BitmapImage bitmap = new BitmapImage(new Uri(MainWindow.AssetPath + source, UriKind.RelativeOrAbsolute));
                    CroppedBitmap cropped = new CroppedBitmap(bitmap, new Int32Rect((int)rectangle[0], (int)rectangle[1], (int)rectangle[2], (int)rectangle[3]));
                    square.Fill = new ImageBrush(cropped);
                    square.StrokeThickness = 0;
                }
                Tile tile = new Tile { Rect = square, Position = new Point(position.X, position.Y), Origin = new Point(rectangle[0], rectangle[1]), Dimensions = new Point(rectangle[2], rectangle[3]), Source = source, IsFilled = filled };
                _map.Add(tile);
            }
        }
    }
}

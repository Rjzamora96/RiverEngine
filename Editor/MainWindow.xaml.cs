using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Window { get; set; }
        public List<Script> Scripts { get; set; }
        public MainWindow()
        {
            MainWindow.Window = this;
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
                            Match match = Regex.Match(entityScript, "{(.*)}");
                            Group compGroup = match.Groups[1];  //AllComponents
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
                                if (scriptMatch != null)
                                {
                                    FileInfo compFile = new FileInfo("..\\..\\..\\RenderEngineDX12\\" + scriptMatch.Groups[1].ToString());
                                    entity.Editor.AddComponent(compFile);
                                    entity.Editor.Owner.Components.Last().SetProperties(savedValues);
                                }
                            }
                            entity.Editor.SyncProperties();
                            //{(?>[^{}]+|(?R))*}
                        }
                    }
                }
            }
        }
        private List<string> DivideStrings(string value)
        {
            List<string> result = new List<string>();
            int level = 0;
            string currentString = "";
            for(int i = 0; i < value.Length; i++)
            {
                if (value[i].Equals('}')) level--;
                if (level >= 1 && (level == 1 && !value[i].Equals(','))) currentString += value[i]; 
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
                        File.WriteAllText("..\\..\\..\\RenderEngineDX12\\" + entity.EName + ".entity", entity.ToString());
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
            Scripts.Clear();
            DirectoryInfo dir = new DirectoryInfo("..\\..\\..\\RenderEngineDX12");
            IEnumerable<FileInfo> files = dir.EnumerateFiles();
            foreach (FileInfo file in files)
            {
                if (file.Extension != ".lua" && file.Extension != ".entity") continue;
                if (file.Extension == ".lua")
                {
                    Script script = new Script
                    {
                        File = file,
                        Name = Path.GetFileNameWithoutExtension(file.Name),
                        IsUserDefined = false
                    };
                    script.Header = script.Name;
                    Scripts.Add(script);
                }
                AssetItem item = new AssetItem(file);
                item.EditorTabs = editorTabs;
                assetDisplay.Items.Add(item);
            }
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
            TextEditor activeEditor = (TextEditor)editorTabs.SelectedItem;
            activeEditor.SaveFile();
        }

        private void NewFile(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Script files (*.lua)|*.lua";
            if(saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, "");
                TextEditor activeEditor = new TextEditor();
                activeEditor.OpenFile(saveFileDialog.FileName);
                UpdateAssetDisplay();
            }
        }
    }
}

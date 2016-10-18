using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DirectoryInfo dir = new DirectoryInfo("..\\..\\..\\RenderEngineDX12");
            IEnumerable<FileInfo> files = dir.EnumerateFiles();
            foreach(FileInfo file in files)
            {
                if (file.Extension != ".lua") continue;
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
            }
        }
    }
}

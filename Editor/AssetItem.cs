using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
            ContextMenu cm = new ContextMenu();
            MenuItem menuItem = new MenuItem();
            menuItem.Header = "Rename";
            menuItem.Click += RenameFile;
            cm.Items.Add(menuItem);
            ContextMenu = cm;
            MouseMove += MoveAsset;
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

        public void OpenScript(object sender, RoutedEventArgs e)
        {
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

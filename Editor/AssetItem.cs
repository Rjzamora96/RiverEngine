﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Editor
{
    public class AssetItem : ListBoxItem
    {
        private FileInfo _file;
        public TabControl EditorTabs { get; set; }
        public AssetItem(FileInfo file) : base()
        {
            _file = file;
            Width = 100;
            StackPanel content = new StackPanel();
            Image fileImage = new Image();
            fileImage.Source = new BitmapImage(new Uri("lua-icon.png", UriKind.Relative));
            TextBox fileName = new TextBox();
            fileName.BorderThickness = new Thickness(0);
            fileName.TextWrapping = TextWrapping.Wrap;
            fileName.IsReadOnly = true;
            fileName.Text = System.IO.Path.GetFileNameWithoutExtension(file.Name);
            fileName.HorizontalAlignment = HorizontalAlignment.Center;
            content.Children.Add(fileImage);
            content.Children.Add(fileName);
            Content = content;
            MouseDoubleClick += OpenScript;
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
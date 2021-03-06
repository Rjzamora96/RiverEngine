﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Editor
{
    public class EntityEditor : Grid
    {
        public EntityItem Owner { get; set; }
        private Grid _entityProperties;
        private class PropertyLine : Grid
        {
            public TextBox Text { get; set; }
            public ComponentProperty Bound { get; set; }
            public PropertyLine(string name) : base()
            {
                ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(75.0) });
                ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.1, GridUnitType.Star) });
                Label label = new Label();
                label.Content = name;
                Grid.SetColumn(label, 0);
                Children.Add(label);
                Text = new TextBox();
                Text.VerticalContentAlignment = VerticalAlignment.Center;
                Grid.SetColumn(Text, 1);
                Children.Add(Text);
                Text.TextChanged += UpdateBound;
            }
            public void SyncToValue()
            {
                if (Bound != null) Text.Text = Bound.Value;
            }
            private void UpdateBound(object sender, RoutedEventArgs e)
            {
                if (Bound != null) Bound.Value = Text.Text;
            }
        }
        private PropertyLine _nameLine;
        private PropertyLine _tagLine;
        public EntityEditor() : base()
        {
            RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.0, GridUnitType.Auto) });
            RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.1, GridUnitType.Star) });
            RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.0, GridUnitType.Auto) });
            Label title = new Label();
            title.Content = "Entity";
            Grid.SetRow(title, 0);
            Children.Add(title);
            Border border = new Border();
            border.BorderBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#AAAAAA"));
            border.BorderThickness = new Thickness(1);
            Grid.SetRow(border, 1);
            Children.Add(border);
            Label addComponent = new Label();
            addComponent.Content = "Drop Component";
            addComponent.HorizontalContentAlignment = HorizontalAlignment.Center;
            Grid.SetRow(addComponent, 2);
            Children.Add(addComponent);
            _entityProperties = new Grid();
            _entityProperties.Margin = new Thickness(3);
            _entityProperties.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.0, GridUnitType.Auto) });
            _entityProperties.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.0, GridUnitType.Auto) });
            _nameLine = new PropertyLine("Name: ");
            _nameLine.Text.Text = "Entity";
            _nameLine.Text.TextChanged += NameChanged;
            _tagLine = new PropertyLine("Tags: ");
            _tagLine.Text.Text = "{}";
            _tagLine.Text.TextChanged += TagsChanged;
            Grid.SetRow(_nameLine, 0);
            Grid.SetRow(_tagLine, 1);
            _entityProperties.Children.Add(_nameLine);
            _entityProperties.Children.Add(_tagLine);
            border.Child = _entityProperties;
            AllowDrop = true;
            Drop += DropComponent;
            //addComponent.Click += ClickAddComponent;
        }
        private void ClickAddComponent(object sender, RoutedEventArgs args)
        {
            ContextMenu menu = new ContextMenu();
            foreach(Script script in MainWindow.Window.Scripts)
            {
                script.Click -= ScriptClicked;
                script.Click += ScriptClicked;
                menu.Items.Add(script);
            }
            ContextMenu = menu;
            ContextMenu.IsOpen = true;
            ContextMenuClosing += Reset;
        }
        private void Reset(object sender, RoutedEventArgs args)
        {
            ContextMenu.Items.Clear();
        }
        private void ScriptClicked(object sender, RoutedEventArgs args)
        {
            Script script = sender as Script;
            if(script != null)
            {
                AddComponent(script.File);
            }
        }
        public void SyncProperties()
        {
            _nameLine.Text.Text = Owner.EName;
            _tagLine.Text.Text = Owner.Tags;
            foreach(UIElement element in _entityProperties.Children)
            {
                PropertyLine property = element as PropertyLine;
                if(property != null)
                {
                    property.SyncToValue();
                }
            }
        }
        public void AddComponent(ComponentItem item)
        {
            ComponentItem component = new ComponentItem();
            component.Name = item.Name;
            component.Properties = new List<ComponentProperty>(item.Properties);
            _entityProperties.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.0, GridUnitType.Auto) });
            Label compName = new Label();
            compName.Content = component.Name;
            Grid.SetRow(compName, _entityProperties.Children.Count);
            _entityProperties.Children.Add(compName);
            foreach (ComponentProperty prop in component.Properties)
            {
                _entityProperties.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.0, GridUnitType.Auto) });
                PropertyLine line = new PropertyLine(prop.Name + ": ");
                line.Text.Text = prop.Value.TrimStart();
                Grid.SetRow(line, _entityProperties.Children.Count);
                _entityProperties.Children.Add(line);
                line.Bound = prop;
            }
            Owner.Components.Add(component);
        }
        public void AddComponent(FileInfo file)
        {
            ComponentItem component = new ComponentItem();
            component.Name = Path.GetFileNameWithoutExtension(file.Name);
            component.Script = file.Name;
            component.Properties = new List<ComponentProperty>();
            _entityProperties.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.0, GridUnitType.Auto) });
            Label compName = new Label();
            compName.Content = component.Name;
            Grid.SetRow(compName, _entityProperties.Children.Count);
            _entityProperties.Children.Add(compName);
            MatchCollection propertyMatches = Regex.Matches(File.ReadAllText(file.FullName), "Component\\.Property\\(\"(.+)\",(.+)\\)");
            foreach (Match match in propertyMatches)
            {
                _entityProperties.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.0, GridUnitType.Auto) });
                PropertyLine line = new PropertyLine(match.Groups[1].ToString() + ": ");
                line.Text.Text = match.Groups[2].ToString().TrimStart();
                Grid.SetRow(line, _entityProperties.Children.Count);
                _entityProperties.Children.Add(line);
                ComponentProperty compProperty = new ComponentProperty(match.Groups[1].ToString(), match.Groups[2].ToString().TrimStart());
                line.Bound = compProperty;
                component.Properties.Add(compProperty);
            }
            Owner.Components.Add(component);
        }
        private void DropComponent(object sender, DragEventArgs e)
        {
            EntityEditor editor = sender as EntityEditor;
            if(editor != null)
            {
                if(e.Data.GetDataPresent(typeof(FileInfo)))
                {
                    FileInfo file = (FileInfo)e.Data.GetData(typeof(FileInfo));
                    if(file.Extension == ".lua")
                    {
                        AddComponent(file);
                    }
                }
                else if(e.Data.GetDataPresent(typeof(ComponentItem)))
                {
                    ComponentItem item = (ComponentItem)e.Data.GetData(typeof(ComponentItem));
                    AddComponent(item);
                }
            }
        }
        //Component\.Property\("(.+)",(.+)\)
        private void NameChanged(object sender, RoutedEventArgs e)
        {
            Owner.EName = _nameLine.Text.Text;
        }
        private void TagsChanged(object sender, RoutedEventArgs e)
        {
            Owner.Tags = _tagLine.Text.Text;
        }
        /*
         <Grid Grid.Column="4" Grid.Row="0">
                <Grid.RowDefinitions>
                    <RowDefinition Height="Auto"/>
                    <RowDefinition Height="*"/>
                    <RowDefinition Height="Auto"/>
                </Grid.RowDefinitions>
                <Label Content="Entity" Grid.Row="0"/>
                <Border BorderBrush="#AAAAAA" BorderThickness="1" Grid.Row="1" >
                    <Grid x:Name="entityDisplay" ScrollViewer.VerticalScrollBarVisibility="Auto" Margin="3" >
                        <Grid.RowDefinitions>
                            <RowDefinition Height="Auto"/>
                        </Grid.RowDefinitions>
                        <Grid Grid.Row="0">
                            <Grid.ColumnDefinitions>
                                <ColumnDefinition Width="Auto"/>
                                <ColumnDefinition Width="*"/>
                            </Grid.ColumnDefinitions>
                            <Label Content="Name: " Grid.Column="0"/>
                            <TextBox Grid.Column="1" VerticalContentAlignment="Center"/>
                        </Grid>
                    </Grid>
                </Border>
                <Button x:Name="addComponent" Grid.Row="2" Content="Add Component"/>
            </Grid>
         */
    }
}

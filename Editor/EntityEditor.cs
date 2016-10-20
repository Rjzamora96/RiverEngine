using System;
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
            }
        }
        private PropertyLine _nameLine;
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
            Button addComponent = new Button();
            addComponent.Content = "Add Component";
            Grid.SetRow(addComponent, 2);
            Children.Add(addComponent);
            _entityProperties = new Grid();
            _entityProperties.Margin = new Thickness(3);
            _entityProperties.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.0, GridUnitType.Auto) });
            _nameLine = new PropertyLine("Name: ");
            _nameLine.Text.TextChanged += NameChanged;
            Grid.SetRow(_nameLine, 0);
            _entityProperties.Children.Add(_nameLine);
            border.Child = _entityProperties;
            AllowDrop = true;
            Drop += DropComponent;
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
                        _entityProperties.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.0, GridUnitType.Auto) });
                        Label compName = new Label();
                        compName.Content = Path.GetFileNameWithoutExtension(file.Name);
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
                        }
                    }
                }
            }
        }
        //Component\.Property\("(.+)",(.+)\)
        private void NameChanged(object sender, RoutedEventArgs e)
        {
            Owner.EName = _nameLine.Text.Text;
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

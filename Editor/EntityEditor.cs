using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Editor
{
    public class EntityEditor : Grid
    {
        private class PropertyLine : Grid
        {
            public PropertyLine(string name) : base()
            {
                ColumnDefinitions.Add(new ColumnDefinition());
                ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(Double.NaN) });
            }
        }
        public EntityEditor() : base()
        {
            RowDefinitions.Add(new RowDefinition());
            RowDefinitions.Add(new RowDefinition { Height = new GridLength(Double.NaN) });
            RowDefinitions.Add(new RowDefinition());
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
            Grid entityProperties = new Grid();
            entityProperties.RowDefinitions.Add(new RowDefinition());
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

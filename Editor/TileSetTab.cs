using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Editor
{
    public class TileSetTab : TabItem
    {
        public ListBox Tiles { get; set; }
        public TileItem SelectedTile { get; set; }
        public string Source { get; set; }
        private Grid _grid;
        public TileSetTab() : base()
        {
            _grid = new Grid();
            _grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.1, GridUnitType.Star) });
            _grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0.0, GridUnitType.Auto) });
            Tiles = new ListBox();
            Grid.SetRow(Tiles, 0);
            _grid.Children.Add(Tiles);
            Tiles.SelectionChanged += OnChange;
            Button loadImage = new Button();
            loadImage.Content = "Load Image";
            loadImage.Click += LoadImage;
            Grid.SetRow(loadImage, 1);
            _grid.Children.Add(loadImage);
            Content = _grid;
        }
        private void OnChange(object sender, EventArgs e)
        {
            SelectedTile = Tiles.SelectedItem as TileItem;
        }
        private void LoadImage(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png)|*.png";
            if (openFileDialog.ShowDialog() == true)
            {
                BitmapImage bitmap = new BitmapImage(new Uri(openFileDialog.FileName, UriKind.RelativeOrAbsolute));
                Source = openFileDialog.SafeFileName;
                int columns = ((int)bitmap.PixelWidth / MainWindow.Window.TileSize);
                int rows = ((int)bitmap.PixelHeight / MainWindow.Window.TileSize);
                FrameworkElementFactory gridPanel = new FrameworkElementFactory(typeof(Grid));
                for(int i = 0; i < columns; i++)
                {
                    FrameworkElementFactory column = new FrameworkElementFactory(typeof(ColumnDefinition));
                    column.SetValue(ColumnDefinition.WidthProperty, new GridLength(0.0, GridUnitType.Auto));
                    gridPanel.AppendChild(column);
                }
                for(int i = 0; i < rows; i++)
                {
                    FrameworkElementFactory row = new FrameworkElementFactory(typeof(RowDefinition));
                    row.SetValue(RowDefinition.HeightProperty, new GridLength(0.0, GridUnitType.Auto));
                    gridPanel.AppendChild(row);
                }
                ItemsPanelTemplate template = new ItemsPanelTemplate();
                template.VisualTree = gridPanel;
                Tiles.ItemsPanel = template;
                for (int x = 0; x < columns; x++)
                {
                    for(int y = 0; y < rows; y++)
                    {
                        CroppedBitmap cropped = new CroppedBitmap(bitmap, new Int32Rect(MainWindow.Window.TileSize * x, MainWindow.Window.TileSize * y, MainWindow.Window.TileSize, MainWindow.Window.TileSize));
                        TileItem item = new TileItem();
                        item.Padding = new Thickness(0);
                        item.Dimensions = new Point(MainWindow.Window.TileSize, MainWindow.Window.TileSize);
                        item.Origin = new Point(MainWindow.Window.TileSize * x, MainWindow.Window.TileSize * y);
                        item.OriginalFile = openFileDialog.FileName;
                        item.Image.Source = cropped;
                        Grid.SetColumn(item, x);
                        Grid.SetRow(item, y);
                        Tiles.Items.Add(item);
                    }
                }
            }
        }
    }
}

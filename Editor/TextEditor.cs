using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace Editor
{
    public class TextEditor : TabItem
    {
        private TextBox _textBox;
        private string _openFile;
        /*
         *             <TextBox Grid.Row="0" Grid.Column="2" x:Name="textEditor" Margin="3"
                             TextWrapping="Wrap" Text="TextBlock"  AcceptsReturn="True" AcceptsTab="True"
                             Background="#252525" FontFamily="Comic Sans MS" FontSize="12" Foreground="White"/>
         */
        public TextEditor() : base()
        {
            _textBox = new TextBox();
            _textBox.TextWrapping = System.Windows.TextWrapping.NoWrap;
            _textBox.AcceptsReturn = true;
            _textBox.AcceptsTab = true;
            _textBox.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#252525"));
            _textBox.Foreground = new SolidColorBrush(Colors.White);
            _textBox.FontFamily = new FontFamily("Comic Sans MS");
            _textBox.FontSize = 12;
            _textBox.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            _textBox.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            Content = _textBox;
            Header = "Script";
        }

        public void OpenFile(string path)
        {
            _openFile = path;
            _textBox.Text = File.ReadAllText(_openFile);
        }

        public void SaveFile()
        {
            File.WriteAllText(_openFile, _textBox.Text);
        }
    }
}

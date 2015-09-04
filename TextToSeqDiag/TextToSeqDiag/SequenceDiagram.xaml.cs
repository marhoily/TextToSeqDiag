using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TextToSeqDiag
{
    public partial class SequenceDiagram
    {
        public SequenceDiagram()
        {
            InitializeComponent();
        }

        public void AddActor(string name)
        {
            LayoutRoot.ColumnDefinitions.Add(
                new ColumnDefinition {Width = GridLength.Auto});

            var header = new Border
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                Margin = new Thickness(5),
                CornerRadius = new CornerRadius(3),
                Padding = new Thickness(15, 2, 15, 2),
                Child = new TextBlock { Text = name }
            };
            Grid.SetColumn(header, LayoutRoot.ColumnDefinitions.Count - 1);
            LayoutRoot.Children.Add(header);
        }
    }
}

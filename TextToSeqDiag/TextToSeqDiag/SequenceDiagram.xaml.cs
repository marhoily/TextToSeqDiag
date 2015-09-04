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

            var line = new Line
            {
                StrokeThickness = 1,
                Y1 = 0,
                Y2 = 1,
                X1 = 0,
                X2 = 0,
                Stroke = Brushes.Black,
                Stretch = Stretch.Fill,
            };
            Grid.SetColumn(line, LayoutRoot.ColumnDefinitions.Count - 1);
            Grid.SetRow(line, 1);
            Grid.SetRowSpan(line, 1000);
            LayoutRoot.Children.Add(line);
        }
    }
}

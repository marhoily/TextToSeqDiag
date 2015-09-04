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
            ObjectsLayer.ColumnDefinitions.Add(
                new ColumnDefinition { Width = GridLength.Auto });

            var header = new Border
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                Margin = new Thickness(5),
                CornerRadius = new CornerRadius(3),
                Padding = new Thickness(15, 2, 15, 2),
                Child = new TextBlock { Text = name },
                SnapsToDevicePixels = true,
            };
            Grid.SetColumn(header, ObjectsLayer.ColumnDefinitions.Count - 1);
            ObjectsLayer.Children.Add(header);

            var line = new Line
            {
                StrokeThickness = 1,
                Y1 = 0,
                Y2 = 1,
                X1 = 0,
                X2 = 0,
                Stroke = Brushes.Black,
                Stretch = Stretch.Fill,
                SnapsToDevicePixels = true,
            };
            Grid.SetColumn(line, ObjectsLayer.ColumnDefinitions.Count - 1);
            Grid.SetRow(line, 1);
            Grid.SetRowSpan(line, 1000);
            ObjectsLayer.Children.Add(line);
        }

        public void AddMessage(int source, int destination, string message)
        {
            ObjectsLayer.RowDefinitions.Insert(1,
                new RowDefinition { Height = GridLength.Auto });

            var sourceAnchor = new Ellipse
            {
                Width = 5,
                Height = 5,
                Fill = Brushes.GreenYellow,
                StrokeThickness = 1,
                Stroke = Brushes.Black,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(10)
            };
            Grid.SetColumn(sourceAnchor, source);
            Grid.SetRow(sourceAnchor, ObjectsLayer.RowDefinitions.Count - 2);
            ObjectsLayer.Children.Add(sourceAnchor);

            var destinationAnchor = new Ellipse
            {
                Width = 5,
                Height = 5,
                Fill = Brushes.GreenYellow,
                StrokeThickness = 1,
                Stroke = Brushes.Black,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(10)
            };
            Grid.SetColumn(destinationAnchor, destination);
            Grid.SetRow(destinationAnchor, ObjectsLayer.RowDefinitions.Count - 2);
            ObjectsLayer.Children.Add(destinationAnchor);

            var arrow = new Arrow
            {
                HeadWidth = 10,
                HeadHeight = 5 * 2 / 3.0,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };

            Anchor.SetSource(arrow, sourceAnchor);
            Anchor.SetDestination(arrow, destinationAnchor);
         //   ArrowsLayer.Children.Add(arrow);
        }
    }
}

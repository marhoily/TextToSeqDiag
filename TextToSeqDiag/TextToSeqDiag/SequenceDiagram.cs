using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TextToSeqDiag
{
    public partial class SequenceDiagram
    {
        private int _column;

        public SequenceDiagram()
        {
            InitializeComponent();
        }

        public void AddActor(string name)
        {
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
            SeqDiagPanel.SetPosition(header, new Position
            {
                Column = _column,
                Kind = PositionKind.OneColumn,
                Row = 0,
            });
            LayoutRoot.Children.Add(header);

            var line = new Line
            {
                StrokeThickness = 1,
                Y1 = 0,
                Y2 = 75,
                X1 = 0,
                X2 = 0,
                MinHeight = 75,
                Stroke = Brushes.Black,
                Stretch = Stretch.Fill,
                SnapsToDevicePixels = true,
            };
            SeqDiagPanel.SetPosition(line, new Position
            {
                Column = _column,
                Kind = PositionKind.Body,
            });
            LayoutRoot.Children.Add(line);
            _column++;
        }
/*
        private static void AddColumn(Grid headers)
        {
            headers.ColumnDefinitions.Add(
                new ColumnDefinition
                {
                    Width = GridLength.Auto,
                    SharedSizeGroup = "b" + headers.ColumnDefinitions.Count
                });
        }

        public void AddMessage(int source, int destination, string message)
        {
            Bodies.RowDefinitions.Add(
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
            Grid.SetRow(sourceAnchor, Bodies.RowDefinitions.Count - 1);
            Bodies.Children.Add(sourceAnchor);

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
            Grid.SetRow(destinationAnchor, Bodies.RowDefinitions.Count - 1);
            Bodies.Children.Add(destinationAnchor);

            var arrow = new Arrow
            {
                HeadWidth = 10,
                HeadHeight = 5 * 2 / 3.0,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };

            Anchor.SetSource(arrow, sourceAnchor);
            Anchor.SetDestination(arrow, destinationAnchor);
        }*/
    }
}

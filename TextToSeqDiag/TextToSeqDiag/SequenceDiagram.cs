using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TextToSeqDiag
{
    public partial class SequenceDiagram
    {
        private int _column;
        private int _row;

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
        public void AddMessage(int source, int destination, string message)
        {
            _row++;
            var arrow = new ArrowElement { Margin = new Thickness(0, 5,0,5)};

            SeqDiagPanel.SetPosition(arrow, new Position
            {
                Column = source,
                Column2 = destination,
                Row = _row,
                Kind = PositionKind.Message,
            });
            LayoutRoot.Children.Add(arrow);

        }
    }
}

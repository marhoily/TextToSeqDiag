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
            SeqDiagPanel.SetPosition(header, 
                Position.OneColumn(_column, 0));
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
            SeqDiagPanel.SetPosition(line, Position.Body(_column));
            LayoutRoot.Children.Add(line);
            _column++;
        }
        public void AddMessage(int source, int destination, string message)
        {
            _row++;
            var direction = MessageDirection.LeftToRight;
            if (source > destination)
            {
                var tmp = source;
                source = destination;
                destination = tmp;
                direction = MessageDirection.RightToLeft;
            }

            var arrowElement = new ArrowElement { Direction = direction };
            Grid.SetRow(arrowElement, 2);
            var grid = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition {Height = GridLength.Auto},
                    new RowDefinition {Height = GridLength.Auto},
                },

                Children =
                {
                    new TextBlock
                    {
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(0,0,0,-2),
                        Text = message,
                        Background = Brushes.White,
                    },
                    arrowElement
                }
            };

            SeqDiagPanel.SetPosition(grid,
                Position.Message(source, destination, _row));
            LayoutRoot.Children.Add(grid);

        }
    }
}

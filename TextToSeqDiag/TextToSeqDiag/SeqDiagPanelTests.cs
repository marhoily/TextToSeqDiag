using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using ApprovalTests.Reporters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TextToSeqDiag
{
    [TestClass]
    [UseReporter(typeof(AraxisMergeReporter))]
    public class SeqDiagPanelTests
    {
        private SeqDiagPanel _view;

        [TestInitialize]
        public void TestInitialize()
        {
            _view = new SeqDiagPanel
            {
                Background = Brushes.White
            };
        }

        [TestMethod]
        public void Verify_One_Column()
        {
            _view.Children.Add(CreateRect(0));
            _view.VerifySnapshot();
        }

        private static Border CreateRect(int column)
        {
            var border = new Border
            {
                Margin = new Thickness(3),
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                Child = new TextBlock
                {
                    Text = "col: " + column,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                }
            };
            SeqDiagPanel.SetPosition(border, new Position { Column = column });
            return border;
        }
    }
    public sealed class Position
    {
        public int Column { get; set; }
    }
    public sealed class SeqDiagPanel : Panel
    {
        public static readonly DependencyProperty PositionProperty = DependencyProperty.RegisterAttached(
            "Position", typeof(Position), typeof(SeqDiagPanel), new PropertyMetadata(default(Position)));

        public static void SetPosition(DependencyObject element, Position value)
        {
            element.SetValue(PositionProperty, value);
        }

        public static Position GetPosition(DependencyObject element)
        {
            return (Position)element.GetValue(PositionProperty);
        }
        protected override Size MeasureOverride(Size availableSize)
        {
            var size = default(Size);
            foreach (UIElement child in Children)
            {
                child.Measure(availableSize);
                size = child.DesiredSize;
            }
            return size;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (UIElement child in Children)
            {
                child.Arrange(new Rect(finalSize));
            }
            return base.ArrangeOverride(finalSize);
        }
    }
}

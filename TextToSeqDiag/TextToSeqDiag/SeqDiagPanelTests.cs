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
            _view.Children.Add(CreateRect("1"));
            _view.VerifySnapshot();
        }

        private static Border CreateRect(string text)
        {
            return new Border
            {
                Margin = new Thickness(3),
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                Child = new TextBlock
                {
                    Text = text,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                }
            };
        }
    }

    public sealed class SeqDiagPanel : Panel
    {
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

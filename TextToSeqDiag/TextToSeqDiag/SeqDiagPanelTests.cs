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
                Width = 100,
                Height = 100,
                Background = Brushes.White
            };
        }

        [TestMethod]
        public void Verify_One_Column()
        {
            _view.Columns = 1;
            _view.Children.Add(new Border
            {
                Margin = new Thickness(3),
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                Child = new TextBlock
                {
                    Text = "1",
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                }
            });
            _view.VerifySnapshot();
        }
    }

    public sealed class SeqDiagPanel : Panel
    {
        public int Columns { get; set; }
        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (UIElement child in Children)
            {
                child.Measure(availableSize);
            }
            return base.MeasureOverride(availableSize);
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

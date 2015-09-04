using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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
            _view.Children.Add(CreateOneColumnRect(0, 0));
            _view.VerifySnapshot();
        }

        [TestMethod]
        public void Verify_Two_Columns()
        {
            _view.Children.Add(CreateOneColumnRect(0, 0));
            _view.Children.Add(CreateOneColumnRect(1, 0));
            _view.VerifySnapshot();
        }

        [TestMethod]
        public void Verify_Second_Row()
        {
            _view.Children.Add(CreateOneColumnRect(0, 0));
            _view.Children.Add(CreateOneColumnRect(1, 0));
            _view.Children.Add(CreateOneColumnRect(0, 1));
            _view.Children.Add(CreateOneColumnRect(1, 1));
            _view.VerifySnapshot();
        }

        [TestMethod]
        public void Verify_Message_Rect()
        {
            _view.Children.Add(CreateOneColumnRect(0, 0));
            _view.Children.Add(CreateOneColumnRect(1, 0));
            _view.Children.Add(CreateMessageRect(0, 1, 1));
            _view.VerifySnapshot();
        }

        private static Border CreateOneColumnRect(int column, int row)
        {
            var border = new Border
            {
                Margin = new Thickness(3),
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Child = new TextBlock
                {
                    Text = string.Format("({0}, {1})", column, row),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                }
            };
            SeqDiagPanel.SetPosition(border, new Position
            {
                Column = column,
                Row = row,
                Kind = PositionKind.OneColumn,
            });
            return border;
        }
        private static Border CreateMessageRect(int column, int column2, int row)
        {
            var border = new Border
            {
                Margin = new Thickness(3),
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                Child = new TextBlock
                {
                    Text = string.Format("({0} -> {1}, {2})", column, column2, row),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                }
            };
            SeqDiagPanel.SetPosition(border, new Position
            {
                Column = column,
                Column2 = column2,
                Row = row,
                Kind = PositionKind.Message,
            });
            return border;
        }
    }
}
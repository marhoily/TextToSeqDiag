using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ApprovalTests.Reporters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TextToSeqDiag
{
    [TestClass]
    [UseReporter(typeof (AraxisMergeReporter))]
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

        [TestMethod]
        public void Verify_Body_Rect()
        {
            _view.Children.Add(CreateOneColumnRect(0, 0));
            _view.Children.Add(CreateOneColumnRect(0, 1));
            _view.Children.Add(CreateBodyRect(1));
            _view.VerifySnapshot();
        }

        private static Border CreateOneColumnRect(int column, int row)
        {
            return CreateRect(
                string.Format("({0}, {1})", column, row),
                Position.OneColumn(column, row));
        }

        private static Border CreateMessageRect(int column, int column2, int row)
        {
            return CreateRect(
                string.Format("({0} -> {1}, {2})", column, column2, row),
                Position.Message(column, column2, row));
        }

        private static Border CreateBodyRect(int column)
        {
            return CreateRect(
                string.Format("({0}, all)", column),
                Position.Body(column));
        }

        private static Border CreateRect(string format, Position position)
        {
            var border = new Border
            {
                Margin = new Thickness(3),
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                VerticalAlignment = VerticalAlignment.Stretch,
                Child = new TextBlock
                {
                    Text = format,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                }
            };
            SeqDiagPanel.SetPosition(border, position);
            return border;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
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
            _view.Children.Add(CreateRect(0));
            _view.VerifySnapshot();
        }

        [TestMethod]
        public void Verify_Two_Columns()
        {
            _view.Children.Add(CreateRect(0));
            _view.Children.Add(CreateRect(1));
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
            SeqDiagPanel.SetPosition(border, new Position {Column = column});
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
            "Position", typeof (Position), typeof (SeqDiagPanel), new PropertyMetadata(default(Position)));

        private Dictionary<int, Column> _columns;

        public static void SetPosition(DependencyObject element, Position value)
        {
            element.SetValue(PositionProperty, value);
        }

        public static Position GetPosition(DependencyObject element)
        {
            return (Position) element.GetValue(PositionProperty);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            _columns = GroupByColumns();

            var size = default(Size);
            foreach (UIElement child in Children)
            {
                child.Measure(availableSize);
                size = child.DesiredSize;
                _columns[GetPosition(child).Column].Update(size.Width);
            }
            return new Size(_columns.Values.Sum(c => c.Width), size.Height);
        }

        private Dictionary<int, Column> GroupByColumns()
        {
            return Children.OfType<UIElement>()
                .GroupBy(c => GetPosition(c).Column)
                .OrderBy(group => group.Key)
                .Select(group => new Column(group))
                .ToDictionary(c => c.ColumnIndex, c => c);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            UpdateColumnOffsets();
            foreach (UIElement child in Children)
            {
                var column = GetPosition(child).Column;
                child.Arrange(new Rect(
                    new Point(_columns[column].Left, 0), 
                    child.DesiredSize));
            }
            return base.ArrangeOverride(finalSize);
        }

        private void UpdateColumnOffsets()
        {
            var accumulator = 0.0;
            foreach (var column in _columns.Values)
            {
                column.Left = accumulator;
                accumulator += column.Width;
            }
        }

        private sealed class Column
        {
            public Column(IGrouping<int, UIElement> group)
            {
                Elements = group.ToArray();
                ColumnIndex = group.Key;
            }

            public int ColumnIndex { get; private set; }

            public UIElement[] Elements { get; private set; }
            public double Width { get; private set; }
            public double Left { get; set; }

            public void Update(double width)
            {
                Width = Math.Max(width, Width);
            }
        }
    }
}
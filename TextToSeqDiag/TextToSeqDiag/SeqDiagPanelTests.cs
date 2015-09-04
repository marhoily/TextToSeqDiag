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
            _view.Children.Add(CreateRect(0, 0));
            _view.VerifySnapshot();
        }

        [TestMethod]
        public void Verify_Two_Columns()
        {
            _view.Children.Add(CreateRect(0, 0));
            _view.Children.Add(CreateRect(1, 0));
            _view.VerifySnapshot();
        }

        [TestMethod]
        public void Verify_Second_Row()
        {
            _view.Children.Add(CreateRect(0, 0));
            _view.Children.Add(CreateRect(1, 0));
            _view.Children.Add(CreateRect(0, 1));
            _view.Children.Add(CreateRect(1, 1));
            _view.VerifySnapshot();
        }

        private static Border CreateRect(int column, int row)
        {
            var border = new Border
            {
                Margin = new Thickness(3),
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
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
            });
            return border;
        }
    }

    public sealed class Position
    {
        public int Column { get; set; }
        public int Row { get; set; }
    }

    public sealed class SeqDiagPanel : Panel
    {
        public static readonly DependencyProperty PositionProperty = DependencyProperty.RegisterAttached(
            "Position", typeof (Position), typeof (SeqDiagPanel), new PropertyMetadata(default(Position)));

        private Dictionary<int, Column> _columns;
        private Dictionary<int, Row> _rows;

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
            _rows = GroupByRows();

            foreach (UIElement child in Children)
            {
                child.Measure(availableSize);
                var size = child.DesiredSize;
                _columns[GetPosition(child).Column].Update(size.Width);
                _rows[GetPosition(child).Row].Update(size.Height);
            }
            return new Size(
                _columns.Values.Sum(c => c.Width),
                _rows.Values.Sum(c => c.Height));
        }

        private Dictionary<int, Column> GroupByColumns()
        {
            return Children.OfType<UIElement>()
                .GroupBy(c => GetPosition(c).Column)
                .OrderBy(group => group.Key)
                .Select(group => new Column(group))
                .ToDictionary(c => c.ColumnIndex, c => c);
        }

        private Dictionary<int, Row> GroupByRows()
        {
            return Children.OfType<UIElement>()
                .GroupBy(c => GetPosition(c).Row)
                .OrderBy(group => group.Key)
                .Select(group => new Row(group))
                .ToDictionary(c => c.RowIndex, c => c);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            UpdateColumnOffsets();
            UpdateRowOffsets();
            foreach (UIElement child in Children)
            {
                var position = GetPosition(child);
                var topLeft = new Point(
                    _columns[position.Column].Left,
                    _rows[position.Row].Top);
                child.Arrange(new Rect(topLeft, child.DesiredSize));
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
        private void UpdateRowOffsets()
        {
            var accumulator = 0.0;
            foreach (var row in _rows.Values)
            {
                row.Top = accumulator;
                accumulator += row.Height;
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

        private sealed class Row
        {
            public Row(IGrouping<int, UIElement> group)
            {
                Elements = group.ToArray();
                RowIndex = group.Key;
            }

            public int RowIndex { get; private set; }

            public UIElement[] Elements { get; private set; }
            public double Height { get; private set; }
            public double Top { get; set; }

            public void Update(double height)
            {
                Height = Math.Max(height, Height);
            }
        }
    }
}
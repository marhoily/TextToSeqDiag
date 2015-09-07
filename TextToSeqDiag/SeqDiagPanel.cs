using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace TextToSeqDiag
{
    public sealed class SeqDiagPanel : Panel
    {
        public static readonly DependencyProperty PositionProperty = DependencyProperty.RegisterAttached(
            "Position", typeof(Position), typeof(SeqDiagPanel), new PropertyMetadata(default(Position)));

        private readonly GrpStore<int> _columns = new GrpStore<int>(
            c => c.Kind == PositionKind.OneColumn || c.Kind == PositionKind.Body,
            c => c.Column);

        private readonly GrpStore<Tuple<int, int>> _gaps = new GrpStore<Tuple<int, int>>(
            c => c.Kind == PositionKind.Message,
            c => Tuple.Create(c.Column, c.Column2));

        private readonly GrpStore<int> _rows = new GrpStore<int>(c => true, c => c.Row);

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
            _columns.Set(Children.OfType<UIElement>());
            _rows.Set(Children.OfType<UIElement>());
            _gaps.Set(Children.OfType<UIElement>());
            var bodyHeight = 0.0;
            foreach (UIElement child in Children)
            {
                var position = GetPosition(child);
                if (position.Kind == PositionKind.Body) continue;
                child.Measure(availableSize);
                var size = child.DesiredSize;
                var intersections = _rows[position.Row].Intersections;
                switch (position.Kind)
                {
                    case PositionKind.OneColumn:
                    case PositionKind.Body:
                        _columns[position.Column].Update(size.Width);
                        intersections.Add(position.Column*2);
                        intersections.Add(position.Column*2+1);
                        break;
                    case PositionKind.Message:
                        _gaps[Tuple.Create(position.Column, position.Column2)]
                            .Update(size.Width);
                        for (var i = position.Column*2+1; i < position.Column2*2; i++)
                            intersections.Add(i);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                _rows[position.Row].Update(size.Height);
            }
            _rows.UpdateOffsets();
            foreach (UIElement child in Children)
            {
                var position = GetPosition(child);
                if (position.Kind != PositionKind.Body) continue;
                var availableHeight = 0.0;
                var frameworkElement = child as FrameworkElement;
                if (frameworkElement != null)
                {
                    availableHeight = Math.Max(frameworkElement.MinHeight, _rows.TotalSpan);
                }
                child.Measure(new Size(availableSize.Width, availableHeight));
                var size = child.DesiredSize;
                bodyHeight = Math.Max(bodyHeight, size.Height);

                _columns[position.Column].Update(size.Width);
            }
            _columns.UpdateOffsets();

            foreach (var gap in _gaps)
            {
                var gi = gap.Index;
                var totalSpan = _columns[gi.Item2].Midlle - _columns[gi.Item1].Midlle;
                if (!(totalSpan < gap.Span)) continue;
                var increment = (gap.Span - totalSpan) / (gi.Item2 - gi.Item1);
                _columns.IncrementRange(gi, increment);
            }
            CompactifyRows();
            return new Size(_columns.TotalSpan, Math.Max(
                _rows.RowSpanOrDefault(0) + bodyHeight, _rows.TotalSpan));
        }

        private void CompactifyRows()
        {
            var groups = GetCompactGroups();
            var offset = 0.0;
            var fullOffset = 0.0;
            foreach (var g in groups)
            {
                foreach (var row in g)
                {
                    row.Offset = offset;
                    offset += 10;
                    fullOffset = row.Offset + row.Span;
                }
                offset = fullOffset;
            }
        }

        private List<List<Grp<int>>> GetCompactGroups()
        {
            var touchedRows = new HashSet<int>();
            var currentGroup = new List<Grp<int>>();
            var groups = new List<List<Grp<int>>>();
            foreach (var row in _rows)
            {
                if (row.Intersections.All(touchedRows.Add))
                {
                    currentGroup.Add(row);
                }
                else
                {
                    groups.Add(currentGroup);
                    currentGroup = new List<Grp<int>> { row };
                    touchedRows = new HashSet<int>(row.Intersections);
                }
            }
            if (currentGroup.Count > 0)
            {
                groups.Add(currentGroup);
            }
            return groups;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (UIElement child in Children)
            {
                var position = GetPosition(child);
                var c1 = _columns[position.Column];
                switch (position.Kind)
                {
                    case PositionKind.OneColumn:
                        var topLeft = new Point(c1.Offset, _rows[position.Row].Offset);
                        child.Arrange(new Rect(topLeft, new Size(c1.Span, child.DesiredSize.Height)));
                        break;
                    case PositionKind.Message:
                        var c2 = _columns[position.Column2];
                        var topLeft2 = new Point(c1.Midlle, _rows[position.Row].Offset);
                        child.Arrange(new Rect(topLeft2,
                            new Size(c2.Midlle - c1.Midlle, child.DesiredSize.Height)));
                        break;
                    case PositionKind.Body:
                        var topLeft3 = new Point(c1.Offset, _rows.RowSpanOrDefault(0));
                        child.Arrange(new Rect(topLeft3,
                            new Size(c1.Span, child.DesiredSize.Height)));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            return base.ArrangeOverride(finalSize);
        }
    }
}
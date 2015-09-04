using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace TextToSeqDiag
{
    public sealed class SeqDiagPanel : Panel
    {
        public static readonly DependencyProperty PositionProperty = DependencyProperty.RegisterAttached(
            "Position", typeof (Position), typeof (SeqDiagPanel), new PropertyMetadata(default(Position)));

        private GrpStore<int> _columns;
        private GrpStore<Tuple<int, int>> _gaps;
        private GrpStore<int> _rows;

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
            _columns = new GrpStore<int>(
                Children.OfType<UIElement>().Where(
                    c => GetPosition(c).Kind == PositionKind.OneColumn),
                c => GetPosition(c).Column);

            _rows = new GrpStore<int>(
                Children.OfType<UIElement>(),
                c => GetPosition(c).Row);

            _gaps = new GrpStore<Tuple<int, int>>(
                Children.OfType<UIElement>().Where(
                    c => GetPosition(c).Kind == PositionKind.Message),
                c => Tuple.Create(GetPosition(c).Column, GetPosition(c).Column2));

            foreach (UIElement child in Children)
            {
                child.Measure(availableSize);
                var size = child.DesiredSize;
                var position = GetPosition(child);
                switch (position.Kind)
                {
                    case PositionKind.OneColumn:
                        _columns[position.Column].Update(size.Width);
                        break;
                    case PositionKind.Message:
                        var c1 = position.Column;
                        var c2 = position.Column2;
                        _gaps[Tuple.Create(c1, c2)].Update(size.Width);
                        break;
                }
                _rows[position.Row].Update(size.Height);
            }
            _columns.UpdateOffsets();
            _rows.UpdateOffsets();
            foreach (var gap in _gaps)
            {
                var gi = gap.Index;
                var c1 = _columns[gi.Item1];
                var c2 = _columns[gi.Item2];
                var totalSpan = c2.Midlle - c1.Midlle;
                if (totalSpan < gap.Span)
                {
                    var increment = (gap.Span - totalSpan)/(gi.Item2 - gi.Item1);
                    foreach (var column in _columns)
                        if (column.Index >= gi.Item1 && column.Index <= gi.Item2)
                            column.Span += increment;
                    _columns.UpdateOffsets();
                }
            }
            return new Size(_columns.TotalSpan, _rows.TotalSpan);
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
                        var topLeft = new Point(
                            c1.Offset,
                            _rows[position.Row].Offset);
                        child.Arrange(new Rect(topLeft,
                            new Size(c1.Span, child.DesiredSize.Height)));
                        break;
                    case PositionKind.Message:
                        var c2 = _columns[position.Column2];
                        var left = c1.Midlle;
                        var right = c2.Midlle;
                        var topLeft2 = new Point(left, _rows[position.Row].Offset);
                        child.Arrange(new Rect(topLeft2,
                            new Size(right - left, child.DesiredSize.Height)));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            return base.ArrangeOverride(finalSize);
        }
    }
}
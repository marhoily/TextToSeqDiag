using System;
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
                if (position.Kind != PositionKind.Body)
                    MeasureChild(availableSize, child, position);
            }
            _rows.UpdateOffsets();
            foreach (UIElement child in Children)
            {
                var position = GetPosition(child);
                if (position.Kind != PositionKind.Body) continue;
                var availableHeight = _rows.TotalSpan;
                var frameworkElement = child as FrameworkElement;
                if (frameworkElement != null)
                {
                    availableHeight = Math.Max(frameworkElement.MinHeight, availableHeight);
                }
                bodyHeight = Math.Max(bodyHeight, availableHeight);
                child.Measure(new Size(availableSize.Width, availableHeight));
                var size = child.DesiredSize;

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
            return new Size(_columns.TotalSpan, Math.Max(bodyHeight, _rows.TotalSpan));
        }

        private void MeasureChild(Size availableSize, UIElement child, Position position)
        {
            child.Measure(availableSize);
            var size = child.DesiredSize;
            switch (position.Kind)
            {
                case PositionKind.OneColumn:
                case PositionKind.Body:
                    _columns[position.Column].Update(size.Width);
                    break;
                case PositionKind.Message:
                    _gaps[Tuple.Create(position.Column, position.Column2)]
                        .Update(size.Width);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            _rows[position.Row].Update(size.Height);
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
                        var topLeft3 = new Point(c1.Offset, 0);
                        child.Arrange(new Rect(topLeft3,
                            new Size(c1.Span, finalSize.Height)));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            return base.ArrangeOverride(finalSize);
        }
    }
}
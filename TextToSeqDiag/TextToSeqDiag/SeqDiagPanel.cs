using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace TextToSeqDiag
{
    public sealed class SeqDiagPanel : Panel
    {
        public static readonly DependencyProperty PositionProperty = DependencyProperty.RegisterAttached(
            "Position", typeof(Position), typeof(SeqDiagPanel), new PropertyMetadata(default(Position)));

        private GrpStore<int> _columns;
        private GrpStore<int> _rows;
        private Dictionary<Tuple<int, int>, Gap> _gaps;

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
            _columns = new GrpStore<int>(
                Children.OfType<UIElement>().Where(
                    c => GetPosition(c).Kind == PositionKind.OneColumn),
                c => GetPosition(c).Column);

            _rows = new GrpStore<int>(
                Children.OfType<UIElement>(),
                c => GetPosition(c).Row);

            _gaps = GroupByGaps();

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
            foreach (var gap in _gaps.Values)
            {
                var gi = gap.GapIndex;
                var c1 = _columns[gi.Item1];
                var c2 = _columns[gi.Item2];
                var totalSpan = c2.Midlle - c1.Midlle;
                if (totalSpan < gap.Width)
                {
                    var increment = (gap.Width - totalSpan) / (gi.Item2 - gi.Item1);
                    foreach (var column in _columns)
                        if (column.Index >= gi.Item1 && column.Index <= gi.Item2)
                            column.Span += increment;
                    _columns.UpdateOffsets();
                }
            }
            return new Size(_columns.TotalSpan, _rows.TotalSpan);
        }

     
        private Dictionary<Tuple<int, int>, Gap> GroupByGaps()
        {
            return Children.OfType<UIElement>()
                .Where(c => GetPosition(c).Kind == PositionKind.Message)
                .GroupBy(c => Tuple.Create(GetPosition(c).Column, GetPosition(c).Column2))
                .Select(group => new Gap(group))
                .ToDictionary(c => c.GapIndex, c => c);
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


        private sealed class Gap
        {
            public Tuple<int, int> GapIndex { get; private set; }
            public Gap(IGrouping<Tuple<int, int>, UIElement> @group)
            {
                GapIndex = group.Key;
            }

            public double Width { get; private set; }

            public void Update(double width)
            {
                Width = Math.Max(width, Width);
            }
        }

    }

    sealed class Grp<T>
    {
        public Grp(IGrouping<T, UIElement> group)
        {
            Elements = group.ToArray();
            Index = group.Key;
        }

        public T Index { get; private set; }
        public UIElement[] Elements { get; private set; }
        public double Span { get; set; }
        public double Offset { get; set; }
        public double Midlle { get { return Offset + Span/2; } }

        public void Update(double span)
        {
            Span = Math.Max(span, Span);
        }
    }

    sealed class GrpStore<T> : IEnumerable<Grp<T>>
    {
        private readonly Dictionary<T, Grp<T>> _byIndex;

        public double TotalSpan { get { return _byIndex.Values.Sum(c => c.Span); }}

        public GrpStore(IEnumerable<UIElement> source, Func<UIElement, T> getKey)
        {
            _byIndex = source
                .GroupBy(getKey)
                .Select(group => new Grp<T>(group))
                .ToDictionary(c => c.Index, c => c);
        }

        public void UpdateOffsets()
        {
            var accumulator = 0.0;
            foreach (var grp in _byIndex.Values)
            {
                grp.Offset = accumulator;
                accumulator += grp.Span;
            }
        }

        public Grp<T> this[T index]
        {
            get { return _byIndex[index]; }
        }

        public IEnumerator<Grp<T>> GetEnumerator()
        {
            return _byIndex.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
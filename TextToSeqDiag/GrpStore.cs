using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace TextToSeqDiag
{
    internal sealed class GrpStore<T> : IEnumerable<Grp<T>>
    {
        private readonly Func<Position, bool> _filter;
        private readonly Func<Position, T> _getKey;
        private Dictionary<T, Grp<T>> _byIndex;

        public GrpStore(Func<Position, bool> filter, Func<Position, T> getKey)
        {
            _filter = filter;
            _getKey = getKey;
        }

        public void Set(IEnumerable<UIElement> source)
        {
            _byIndex = source
                .Where(x => _filter(SeqDiagPanel.GetPosition(x)))
                .GroupBy(x => _getKey(SeqDiagPanel.GetPosition(x)))
                .Select(group => new Grp<T>(group))
                .ToDictionary(c => c.Index, c => c);
        }

        public void Reset() { _byIndex = null; }

        public double TotalSpan
        {
            get { return _byIndex.Values.Sum(c => c.Span); }
        }
       // public T MaxIndex { get { return _byIndex.Keys.Max(); } }

        public Grp<T> this[T index]
        {
            get { return _byIndex[index]; }
        }

        public IEnumerator<Grp<T>> GetEnumerator()
        {
            return _byIndex.Values
                .OrderBy(x => x.Index, Comparer<T>.Default)
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void UpdateOffsets()
        {
            var accumulator = 0.0;
            foreach (var grp in this)
            {
                grp.Offset = accumulator;
                accumulator += grp.Span;
            }
        }

        public void IncrementRange(Tuple<T, T> range, double increment)
        {
            var c = Comparer<T>.Default;
            foreach (var grp in this)
                if (c.Compare(grp.Index, range.Item1) >= 0 
                    && c.Compare(grp.Index, range.Item2) <= 0)
                    grp.Span += increment;
            UpdateOffsets();
        }

        public double RowSpanOrDefault(T index)
        {
            Grp<T> value;
            return _byIndex.TryGetValue(index, out value) ? value.Span : 0.0;
        }
    }
}
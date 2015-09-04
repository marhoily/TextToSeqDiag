using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace TextToSeqDiag
{
    internal sealed class GrpStore<T> : IEnumerable<Grp<T>>
    {
        private readonly Dictionary<T, Grp<T>> _byIndex;

        public GrpStore(IEnumerable<UIElement> source, Func<UIElement, T> getKey)
        {
            _byIndex = source
                .GroupBy(getKey)
                .Select(group => new Grp<T>(group))
                .ToDictionary(c => c.Index, c => c);
        }

        public double TotalSpan
        {
            get { return _byIndex.Values.Sum(c => c.Span); }
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

        public void UpdateOffsets()
        {
            var accumulator = 0.0;
            foreach (var grp in _byIndex.Values)
            {
                grp.Offset = accumulator;
                accumulator += grp.Span;
            }
        }
    }
}
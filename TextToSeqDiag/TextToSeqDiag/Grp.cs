using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace TextToSeqDiag
{
    internal sealed class Grp<T>
    {
        public Grp(IGrouping<T, UIElement> group)
        {
            Elements = group.ToArray();
            Index = group.Key;
            Intersections = new HashSet<int>();
        }

        public T Index { get; private set; }
        public UIElement[] Elements { get; private set; }
        public double Span { get; set; }
        public double Offset { get; set; }
        public HashSet<int> Intersections { get; private set; }

        public double Midlle
        {
            get { return Offset + Span/2; }
        }

        public void Update(double span)
        {
            Span = Math.Max(span, Span);
            Span = Math.Round(Span/2)*2;
        }
    }
}
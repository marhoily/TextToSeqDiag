using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TextToSeqDiag
{
    public sealed class ArrowElement : UserControl
    {
        private readonly Arrow _arrow;
        private readonly double _ceiling;

        public ArrowElement()
        {
            _arrow = new Arrow
            {
                SnapsToDevicePixels = true,
                HeadWidth = 10,
                HeadHeight = 5 * 2 / 3.0,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };
            _ceiling = Math.Ceiling(_arrow.HeadHeight);
            Content = _arrow;
        }

        public MessageDirection Direction { get; set; }

        protected override Size MeasureOverride(Size constraint)
        {
            return new Size(0, _ceiling * 2);
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            switch (Direction)
            {
                case MessageDirection.LeftToRight:
                    _arrow.X2 = arrangeBounds.Width;
                    break;
                case MessageDirection.RightToLeft:
                    _arrow.X1 = arrangeBounds.Width;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            _arrow.Y1 = _arrow.Y2 = _ceiling;
            return base.ArrangeOverride(arrangeBounds);
        }
    }
}
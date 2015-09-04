using System.Windows;

namespace TextToSeqDiag
{
    public static class Anchor
    {
        public static readonly DependencyProperty SourceProperty = DependencyProperty.RegisterAttached(
            "Source", typeof (FrameworkElement), typeof (Anchor), new PropertyMetadata(default(FrameworkElement)));

        public static void SetSource(DependencyObject element, FrameworkElement value)
        {
            element.SetValue(SourceProperty, value);
        }

        public static FrameworkElement GetSource(DependencyObject element)
        {
            return (FrameworkElement) element.GetValue(SourceProperty);
        }

        public static readonly DependencyProperty DestinationProperty = DependencyProperty.RegisterAttached(
            "Destination", typeof (FrameworkElement), typeof (Anchor), new PropertyMetadata(default(FrameworkElement)));

        public static void SetDestination(DependencyObject element, FrameworkElement value)
        {
            element.SetValue(DestinationProperty, value);
        }

        public static FrameworkElement GetDestination(DependencyObject element)
        {
            return (FrameworkElement) element.GetValue(DestinationProperty);
        }
    }
}
using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TextToSeqDiag
{
    public static class LightweigtTestsUtils
    {
        public static bool Hook(this UIElement source)
        {
            var root = source.VisualRoot<Visual>();
            Assert.IsNotNull(root);
            var result = HwndSource.RootVisual == root;
            if (!result) HwndSource.RootVisual = root;
            return result;
        }

        private static readonly HwndSource HwndSource = new HwndSource(
            new HwndSourceParameters
            {
                HwndSourceHook = ApplicationMessageFilter,
            });

        private static IntPtr ApplicationMessageFilter(
            IntPtr hwnd, int message, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            return IntPtr.Zero;
        }

        public static void RunInNewDispathcerFrame(UIElement source, Action action)
        {
            var renderFrame = new DispatcherFrame(true);
            EventHandler handler = null;
            handler = (s, e) =>
            {
                source.LayoutUpdated -= handler;
                Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.DataBind,
                    new Action(() =>
                    {
                        action();
                        renderFrame.Continue = false;
                    }));
            };
            source.LayoutUpdated += handler;
            Dispatcher.PushFrame(renderFrame);
        }
    }
}
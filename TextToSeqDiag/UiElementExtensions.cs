using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ApprovalTests;

namespace TextToSeqDiag
{
    public static class UiElementExtensions
    {
        /// <summary>
        /// Gets a JPG "screenshot" of the current UIElement
        /// </summary>
        /// <param name="source">UIElement to screenshot</param>
        /// <param name="scale">Scale to render the screenshot</param>
        /// <returns>Byte array of JPG data</returns>
        public static byte[] GetJpgImage(this UIElement source, double scale)
        {
            var actualHeight = source.RenderSize.Height;
            var actualWidth = source.RenderSize.Width;

            var renderHeight = actualHeight * scale;
            var renderWidth = actualWidth * scale;

            var renderTarget = new RenderTargetBitmap(
                (int)renderWidth, (int)renderHeight,
                96, 96, PixelFormats.Pbgra32);
            var sourceBrush = new VisualBrush(source);

            var drawingVisual = new DrawingVisual();
            var drawingContext = drawingVisual.RenderOpen();

            using (drawingContext)
            {
                drawingContext.PushTransform(new ScaleTransform(scale, scale));
                var rectangle = new Rect(new Size(actualWidth, actualHeight));
                drawingContext.DrawRectangle(Brushes.White, null, rectangle);
                drawingContext.DrawRectangle(sourceBrush, null, rectangle);
            }
            renderTarget.Render(drawingVisual);

            var bytes = new byte[renderTarget.PixelWidth * renderTarget.PixelHeight * 4];
            renderTarget.CopyPixels(bytes, renderTarget.PixelWidth * 4, 0);

            for (var i = 0; i < bytes.Length; i++)
             //   if (i % 4 != 3)
                bytes[i] = (byte) (bytes[i] & 0xf0);

            var rendered = BitmapSource.Create(
                renderTarget.PixelWidth,
                renderTarget.PixelHeight,
                96, 96, PixelFormats.Pbgra32, 
                BitmapPalettes.BlackAndWhite, 
                bytes, renderTarget.PixelWidth * 4);

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rendered));

            using (var outputStream = new MemoryStream())
            {
                encoder.Save(outputStream);
                return outputStream.ToArray();
            }
        }
     
        public static byte[] TakeSnapshot(this UIElement source)
        {
            if (source.Hook()) return source.GetJpgImage(2);

            source.Measure(new Size(2560, 1440));
            source.Arrange(new Rect(source.DesiredSize));

            byte[] snapshot = null;
            LightweigtTestsUtils.RunInNewDispathcerFrame(source, () =>
            {
                snapshot = source.GetJpgImage(2);
            });

            if (snapshot == null) throw new Exception(":(");
            return snapshot;
        }

        public static byte[] TakeSnapshot(this UIElement source, Size size)
        {
            if (source.Hook()) return source.GetJpgImage(2);

            source.Measure(size);
            source.Arrange(new Rect(size));

            byte[] snapshot = null;
            LightweigtTestsUtils.RunInNewDispathcerFrame(source, () =>
            {
                snapshot = source.GetJpgImage(2);
            });

            if (snapshot == null) throw new Exception(":(");
            return snapshot;
        }

        public static void VerifySnapshot(this UIElement source)
        {
            Approvals.VerifyBinaryFile(source.TakeSnapshot(), ".png");
        }
        public static void VerifySnapshot(this UIElement source, Size size)
        {
            Approvals.VerifyBinaryFile(source.TakeSnapshot(size), ".png");
        }
        public static void ShowSnapshot(this UIElement source)
        {
            var temp = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".png");
            File.WriteAllBytes(temp, source.TakeSnapshot());
            Process.Start(temp);
        }
        public static void ShowSnapshot(this UIElement source, Size size)
        {
            var temp = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".png");
            File.WriteAllBytes(temp, source.TakeSnapshot(size));
            Process.Start(temp);
        }

        public static void Press(this Button btn)
        {
            const BindingFlags bindingFlags =
                BindingFlags.Instance |
                BindingFlags.NonPublic |
                BindingFlags.InvokeMethod;
            btn.GetType().InvokeMember("OnClick", bindingFlags, null, btn, null);
        }

        public static DependencyObject ItemAt(this ItemsControl listBox, int index)
        {
            return listBox.ItemContainerGenerator.ContainerFromIndex(index);
        }
        public static T ItemAt<T>(this ItemsControl listBox, int index)
            where T : DependencyObject
        {
            var dependencyObject = listBox.ItemAt(index);
            return dependencyObject.FindVisualChildren<T>().Single();
        }

        public static T Find<T>(this DependencyObject root, string name)
            where T : FrameworkElement
        {
            return root.FindVisualChildren<T>().Single(c => c.Name == name);
        }

        public static void Energize(this UIElement source)
        {
            source.Hook();
            source.Measure(new Size(2560, 1440));
            source.Arrange(new Rect(source.DesiredSize));

            LightweigtTestsUtils.RunInNewDispathcerFrame(source, () => { });
        }
    }
}
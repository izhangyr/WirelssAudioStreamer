using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace WirelessAudioServer.Wpf
{
    public class TextFetchTextBlockBehavior : Behavior<TextBlock>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += FetchText;
        }

        private static void FetchText(object sender, RoutedEventArgs e)
        {
            var textBlock = sender as TextBlock;
            if (textBlock == null)
            {
                return;
            }

            textBlock.UpdateLayout();
            try
            {
                using (var bitmabObject = new Bitmap(1, 1))
                {
                    using (var graphicsObject = Graphics.FromImage(bitmabObject))
                    {
                        using (var currentFont = new Font(textBlock.FontFamily.ToString(), (float)textBlock.FontSize, System.Drawing.FontStyle.Regular, GraphicsUnit.Pixel))
                        {
                            var textDimensions = graphicsObject.MeasureString(textBlock.Text, currentFont);
                            var fillCoefficient = (int)Math.Ceiling(textDimensions.Width / textBlock.ActualWidth);
                            while ((textDimensions.Height + (textDimensions.Height / 2)) * fillCoefficient >= textBlock.MaxHeight && textDimensions.Width >= textBlock.ActualWidth)
                            {
                                textBlock.FontSize--;
                                using (var newFont = new Font(textBlock.FontFamily.ToString(), (float)textBlock.FontSize, System.Drawing.FontStyle.Regular, GraphicsUnit.Pixel))
                                {
                                    fillCoefficient = (int)Math.Ceiling(textDimensions.Width / textBlock.ActualWidth);
                                    textDimensions = graphicsObject.MeasureString(textBlock.Text, newFont);
                                }
                            }
                        }
                    }
                }
            }
            catch (ArgumentException)
            {
            }
        }
    }
}

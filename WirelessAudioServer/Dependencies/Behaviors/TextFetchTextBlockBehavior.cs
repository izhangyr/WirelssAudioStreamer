/*
    Copyright 2013 Roman Fortunatov

    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.
    You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.
*/

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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Interactivity;

namespace WirelessAudioServer.Wpf
{
    public class TextFormattingTagsParseBehavior : Behavior<TextBlock>
    {
        private readonly Dictionary<Regex, Func<string, TextBlock>> _patterns = new Dictionary<Regex, Func<string, TextBlock>>
            {
                {new Regex(@"<I>(?<value>[\w\s\d.,:;]*)</I>"), GetItalicTextBlock},
                {new Regex(@"<B>(?<value>[\w\s\d.,:;]*)</B>"), GetBoldTextBlock},
                {new Regex(@"<U>(?<value>[\w\s\d.,:;]*)</U>"), GetUnderlineTextBlock},
                {new Regex(@"<Url>(?<value>[\w\s\d.,:;]*)</Url>"), GetUrlTextBlock}
            };

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += ParseTags;
        }

        protected virtual void ParseTags(object sender, RoutedEventArgs e)
        {
            var textBlock = (TextBlock)sender;
            var text = textBlock.Text;
            var textBlocks = new List<Tuple<TextBlock, int>>();
            foreach (var pattern in _patterns)
            {
                var matches = pattern.Key.Matches(text);
                foreach (Match match in matches)
                {
                    var inline = pattern.Value(match.Groups["value"].Captures[0].Value);
                    inline.Tag = match.Value;
                    textBlocks.Add(new Tuple<TextBlock, int>(inline, match.Index));
                }
            }

            if (textBlocks.Count == 0)
            {
                return;
            }

            textBlock.Inlines.Clear();
            textBlock.Text = string.Empty;
            textBlocks = (from item in textBlocks orderby item.Item2 select item).ToList();
            var previousMatch = 0;
            foreach (var item in textBlocks)
            {
                var plainText = text.Substring(previousMatch, item.Item2 - previousMatch);
                if (!string.IsNullOrEmpty(plainText))
                {
                    textBlock.Inlines.Add(new Run(plainText));
                }

                textBlock.Inlines.Add(item.Item1);
                previousMatch = item.Item2 + item.Item1.Tag.ToString().Length;
            }

            if (previousMatch < text.Length)
            {
                textBlock.Inlines.Add(new Run(text.Substring(previousMatch, text.Length - previousMatch)));
            }

            textBlock.UpdateLayout();
        }

        private static TextBlock GetUrlTextBlock(string match)
        {
            var link = new Hyperlink(new Run(match))
            {
                NavigateUri = new Uri(match),
            };

            var linkTextBlock = new TextBlock(link) { TextWrapping = TextWrapping.NoWrap, Visibility = Visibility.Visible };

            link.Click += (sender, eventArgs) =>
            {
                var hyperLink = (Hyperlink)sender;

                try
                {
                    Process.Start(hyperLink.NavigateUri.ToString());
                }
                catch (Win32Exception)
                {
                    // No browser on client's machine. Do nothing.
                }

                eventArgs.Handled = true;
            };

            return linkTextBlock;
        }

        private static TextBlock GetItalicTextBlock(string match)
        {
            return new TextBlock { FontStyle = FontStyles.Italic, TextWrapping = TextWrapping.NoWrap, Text = match };
        }
        
        private static TextBlock GetBoldTextBlock(string match)
        {
            return new TextBlock {FontWeight = FontWeights.Bold, TextWrapping = TextWrapping.NoWrap, Text = match};
        }

        private static TextBlock GetUnderlineTextBlock(string match)
        {
            return new TextBlock { TextDecorations = TextDecorations.Underline, TextWrapping = TextWrapping.NoWrap, Text = match };            
        }
    }
}
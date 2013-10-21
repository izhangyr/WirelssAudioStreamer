using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace WirelessAudioServer.Wpf
{
    public class FocusDefaultButtonBehavior : Behavior<Button>
    {
        private static readonly List<Button> DefaultButtons = new List<Button>();

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Click += FocusOnDefaultButton;
        }

        private static void FocusOnDefaultButton(object sender, RoutedEventArgs e)
        {
            var senderButton = sender as Button;
            if (senderButton == null || senderButton.IsDefault)
            {
                if (!DefaultButtons.Contains(senderButton))
                {
                    DefaultButtons.Add(senderButton);
                }

                return;
            }

            Button currentDefaultButton;
            if ((currentDefaultButton = DefaultButtons.FirstOrDefault(x => x.IsEnabled)) != null)
            {
                currentDefaultButton.Focus();
            }
        }
    }
}

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






using System.Windows;
using System.Windows.Media;

namespace Replay.Common.Wpf.CustomControls.WpfMessageBox
{
    public static class WpfMessageBoxPresenter
    {
        private const string EmptyString = "";

        public static ImageSource DefaultIcon { get; set; }

        public static MessageBoxResult Show(string messageBoxText)
        {
            return ShowCore(null, messageBoxText);
        }

        public static MessageBoxResult Show(string messageBoxText, string caption)
        {
            return ShowCore(null, messageBoxText, caption);
        }

        public static MessageBoxResult Show(Window owner, string messageBoxText)
        {
            return ShowCore(owner, messageBoxText);
        }

        public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButtonOptions button)
        {
            return ShowCore(null, messageBoxText, caption, button);
        }

        public static MessageBoxResult Show(Window owner, string messageBoxText, string caption)
        {
            return ShowCore(owner, messageBoxText, caption);
        }

        public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButtonOptions button, MessageBoxImage icon)
        {
            return ShowCore(null, messageBoxText, caption, button, icon);
        }

        public static MessageBoxResult Show(Window owner, string messageBoxText, string caption, MessageBoxButtonOptions button)
        {
            return ShowCore(owner, messageBoxText, caption, button);
        }

        public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButtonOptions button, MessageBoxImage icon, MessageBoxResult defaultResult)
        {
            return ShowCore(null, messageBoxText, caption, button, icon, defaultResult);
        }

        public static MessageBoxResult Show(Window owner, string messageBoxText, string caption, MessageBoxButtonOptions button, MessageBoxImage icon)
        {
            return ShowCore(owner, messageBoxText, caption, button, icon);
        }

        public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButtonOptions button, MessageBoxImage icon, MessageBoxResult defaultResult, MessageBoxOptions options)
        {
            return ShowCore(null, messageBoxText, caption, button, icon, defaultResult, options);
        }

        public static MessageBoxResult Show(Window owner, string messageBoxText, string caption, MessageBoxButtonOptions button, MessageBoxImage icon, MessageBoxResult defaultResult)
        {
            return ShowCore(owner, messageBoxText, caption, button, icon, defaultResult);
        }

        public static MessageBoxResult Show(Window owner, string messageBoxText, string caption, MessageBoxButtonOptions button, MessageBoxImage icon, MessageBoxResult defaultResult, MessageBoxOptions options)
        {
            return ShowCore(owner, messageBoxText, caption, button, icon, defaultResult, options);
        }

        private static MessageBoxResult ShowCore(
            Window owner,
            string messageBoxText,
            string caption = EmptyString,
            MessageBoxButtonOptions button = MessageBoxButtonOptions.Ok,
            MessageBoxImage icon = MessageBoxImage.None,
            MessageBoxResult defaultResult = MessageBoxResult.None,
            MessageBoxOptions options = MessageBoxOptions.None)
        {
            return WpfMessageBoxWindow.Show(messageBoxWindow => ShowInternal(messageBoxWindow, owner), messageBoxText, caption, button, icon, defaultResult, options);
        }

        private static void ShowInternal(Window messageBoxWindow, Window owner)
        {
            if (owner == null)
            {
                messageBoxWindow.ShowInTaskbar = true;
                messageBoxWindow.Icon = DefaultIcon;
            }
            else
            {
                messageBoxWindow.ShowInTaskbar = false;
                messageBoxWindow.Owner = owner;
            }
        }
    }
}

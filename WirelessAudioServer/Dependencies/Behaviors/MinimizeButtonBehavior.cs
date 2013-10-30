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
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace WirelessAudioServer.Wpf
{
    public class MinimizeButtonBehavior : Behavior<Button>
    {
        private Window _parentWindow;
        private bool _isMinimized;
        
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Click += (sender, args) => MinimizeWindow();
            _parentWindow = (Window)WpfElementsHelper.GetTopParent(AssociatedObject, typeof(Window));
        }

        private void MinimizeWindow()
        {
            _parentWindow.WindowState = WindowState.Minimized;
        }
    }
}

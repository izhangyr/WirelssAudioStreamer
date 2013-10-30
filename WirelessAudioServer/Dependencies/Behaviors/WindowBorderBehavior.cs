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
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace WirelessAudioServer.Wpf
{
    public class WindowBorderBehavior : Behavior<FrameworkElement>
    {
        private bool _isUserDragWindow;
        private List<Tuple<Window, Point>> _dragStartPoint;

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.MouseDown += OnDragClickDown;
            AssociatedObject.MouseUp += OnDragClickUp;
            AssociatedObject.MouseMove += OnDragMouseMove;
        }

        private void OnDragClickDown(object sender, MouseButtonEventArgs e)
        {
            _dragStartPoint = new List<Tuple<Window, Point>>();
            var mainWindow = (Window)WpfElementsHelper.GetTopParent(AssociatedObject, typeof(Window));
            _isUserDragWindow = true;
            _dragStartPoint.Add(new Tuple<Window, Point>(mainWindow, e.GetPosition(mainWindow)));
            foreach (Window ownedWindow in mainWindow.OwnedWindows)
            {
                _dragStartPoint.Add(new Tuple<Window, Point>(ownedWindow, e.GetPosition(ownedWindow)));
            }

            var element = sender as FrameworkElement;
            if (element != null)
            {
                element.CaptureMouse();
            }
        }

        private void OnDragMouseMove(object sender, MouseEventArgs e)
        {
            if (!_isUserDragWindow)
            {
                return;
            }

            foreach (var pair in _dragStartPoint)
            {
                pair.Item1.Left = pair.Item1.PointToScreen(e.GetPosition(pair.Item1)).X - pair.Item2.X;
                pair.Item1.Top = pair.Item1.PointToScreen(e.GetPosition(pair.Item1)).Y - pair.Item2.Y;
            }
        }

        private void OnDragClickUp(object sender, MouseButtonEventArgs e)
        {
            _isUserDragWindow = false;
            var element = sender as FrameworkElement;
            if (element != null)
            {
                element.ReleaseMouseCapture();
            }
        }
    }
}

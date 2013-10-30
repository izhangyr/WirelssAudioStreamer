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
using System.Windows.Interactivity;
using System.Windows.Media;
using WirelessAudioServer.Wpf.CustomControls.Styles;

namespace WirelessAudioServer.Wpf.CustomControls.Control
{
    /// <summary>
    /// Interaction logic for ImageBasedButton.xaml
    /// </summary>
    public partial class ImageBasedButton
    {
#region DependencyProperties
        public static readonly DependencyProperty DefaultBackgroundProperty =
            DependencyProperty.Register("DefaultBackground", typeof(Brush), typeof(ImageBasedButton), null, null);

        public static readonly DependencyProperty PressedBackgroundProperty =
            DependencyProperty.Register("PressedBackground", typeof(Brush), typeof(ImageBasedButton), null, null);

        public static readonly DependencyProperty MouseOverBackgroundProperty =
            DependencyProperty.Register("MouseOverBackground", typeof(Brush), typeof(ImageBasedButton), null, null);

        public static readonly DependencyProperty DefaultImageProperty =
            DependencyProperty.Register("DefaultImage", typeof(ImageSource), typeof(ImageBasedButton), null, null);

        public static readonly DependencyProperty PressedImageProperty =
           DependencyProperty.Register("PressedImage", typeof(ImageSource), typeof(ImageBasedButton), null, null);

        public static readonly DependencyProperty MouseOverImageProperty =
            DependencyProperty.Register("MouseOverImage", typeof(ImageSource), typeof(ImageBasedButton), null, null);

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(ImageBasedButton), null, null);

        public static readonly DependencyProperty ContentCornerRadiusProperty =
            DependencyProperty.Register("ContentCornerRadius", typeof(CornerRadius), typeof(ImageBasedButton), null, null);

        public static readonly DependencyProperty ContentVisibilityProperty =
            DependencyProperty.Register("ContentVisibility", typeof(Visibility), typeof(ImageBasedButton), null, null);

        public static readonly DependencyProperty StyleTypeProperty =
            DependencyProperty.Register("StyleType", typeof(StyleTypes), typeof(ImageBasedButton), null, null);

        public static readonly DependencyProperty BehaviorProperty =
            DependencyProperty.Register("Behavior", typeof(object), typeof(ImageBasedButton), null, null);

        public ImageBasedButton()
        {
            InitializeComponent();
            if (Behavior != null && Behavior is Behavior<System.Windows.Controls.Button>)
            {
                ((Behavior<System.Windows.Controls.Button>)Behavior).Attach(this);
            }
        }

        public Brush DefaultBackground
        {
            get { return (Brush)GetValue(DefaultBackgroundProperty); }
            set { SetValue(DefaultBackgroundProperty, value); }
        }

        public Brush PressedBackground
        {
            get { return (Brush)GetValue(PressedBackgroundProperty); }
            set { SetValue(PressedBackgroundProperty, value); }
        }

        public Brush MouseOverBackground
        {
            get { return (Brush)GetValue(MouseOverBackgroundProperty); }
            set { SetValue(MouseOverBackgroundProperty, value); }
        }

        public ImageSource DefaultImage
        {
            get { return (ImageSource)GetValue(DefaultImageProperty); }
            set { SetValue(DefaultImageProperty, value); }
        }

        public ImageSource PressedImage
        {
            get { return (ImageSource)GetValue(PressedImageProperty); }
            set { SetValue(PressedImageProperty, value); }
        }

        public ImageSource MouseOverImage
        {
            get { return (ImageSource)GetValue(MouseOverImageProperty); }
            set { SetValue(MouseOverImageProperty, value); }
        }

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public CornerRadius ContentCornerRadius
        {
            get { return (CornerRadius)GetValue(ContentCornerRadiusProperty); }
            set { SetValue(ContentCornerRadiusProperty, value); }
        }

        public Visibility ContentVisibility
        {
            get { return (Visibility)GetValue(ContentVisibilityProperty); }
            set { SetValue(ContentVisibilityProperty, value); }
        }

        public StyleTypes StyleType
        {
            get { return (StyleTypes)GetValue(StyleTypeProperty); }
            set { SetValue(StyleTypeProperty, value); }
        }

        public object Behavior
        {
            get { return GetValue(BehaviorProperty); }
            set { SetValue(BehaviorProperty, value); }
        }

#endregion
    }
}

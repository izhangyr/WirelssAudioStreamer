using System.Windows;
using System.Windows.Media;
using WirelessAudioServer.Wpf.CustomControls.Styles;

namespace WirelessAudioServer.Wpf.CustomControls.Control
{
    public partial class LightWindowBorder
    {
        public static readonly DependencyProperty HeadingProperty =
            DependencyProperty.Register("Heading", typeof(string), typeof(LightWindowBorder), null, null);

        public static readonly DependencyProperty HeadingBackgroundProperty =
            DependencyProperty.Register("HeadingBackground", typeof(ImageSource), typeof(LightWindowBorder), null, null);

        public static readonly DependencyProperty OuterBorderThicknessProperty =
            DependencyProperty.Register("OuterBorderThickness", typeof(Thickness), typeof(LightWindowBorder), null, null);

        public static readonly DependencyProperty InnerBorderThicknessProperty =
            DependencyProperty.Register("InnerBorderThickness", typeof(Thickness), typeof(LightWindowBorder), null, null);

        public static readonly DependencyProperty WatermarkVisibilityProperty =
            DependencyProperty.Register("WatermarkVisibility", typeof(Visibility), typeof(LightWindowBorder), null, null);

        public static readonly DependencyProperty MinimizeButtonVisibilityProperty =
            DependencyProperty.Register("MinimizeButtonVisibility", typeof(Visibility), typeof(LightWindowBorder), null, null);

        public static readonly DependencyProperty CloseButtonStyleProperty =
            DependencyProperty.Register("CloseButtonStyle", typeof(StyleTypes), typeof(LightWindowBorder), null, null);

        public static readonly DependencyProperty MinimizeButtonStyleProperty =
            DependencyProperty.Register("MinimizeButtonStyle", typeof(StyleTypes), typeof(LightWindowBorder), null, null);

        public static readonly DependencyProperty StyleTypeProperty =
            DependencyProperty.Register("StyleType", typeof(BorderStyle), typeof(LightWindowBorder), null, null);

        public static readonly DependencyProperty IsCloseButtonEnabledProperty =
            DependencyProperty.Register("IsCloseButtonEnabled", typeof(bool), typeof(LightWindowBorder), null, null);

        public static readonly DependencyProperty InformationButtonProperty =
            DependencyProperty.Register("InformationButton", typeof(ImageBasedButton), typeof(LightWindowBorder), null, null);

        public LightWindowBorder()
        {
            InitializeComponent();
        }

        public string Heading
        {
            get { return (string)GetValue(HeadingProperty); }
            set { SetValue(HeadingProperty, value); }
        }

        public ImageSource HeadingBackground
        {
            get { return (ImageSource)GetValue(HeadingBackgroundProperty); }
            set { SetValue(HeadingBackgroundProperty, value); }
        }

        public Thickness OuterBorderThickness
        {
            get { return (Thickness)GetValue(OuterBorderThicknessProperty); }
            set { SetValue(OuterBorderThicknessProperty, value); }
        }

        public Thickness InnerBorderThickness
        {
            get { return (Thickness)GetValue(InnerBorderThicknessProperty); }
            set { SetValue(InnerBorderThicknessProperty, value); }
        }

        public Visibility WatermarkVisibility
        {
            get { return (Visibility)GetValue(WatermarkVisibilityProperty); }
            set { SetValue(WatermarkVisibilityProperty, value); }
        }

        public Visibility MinimizeButtonVisibility
        {
            get { return (Visibility)GetValue(MinimizeButtonVisibilityProperty); }
            set { SetValue(MinimizeButtonVisibilityProperty, value); }
        }

        public StyleTypes CloseButtonStyle
        {
            get { return (StyleTypes)GetValue(CloseButtonStyleProperty); }
            set { SetValue(CloseButtonStyleProperty, value); }
        }

        public StyleTypes MinimizeButtonStyle
        {
            get { return (StyleTypes)GetValue(MinimizeButtonStyleProperty); }
            set { SetValue(MinimizeButtonStyleProperty, value); }
        }

        public BorderStyle StyleType
        {
            get { return (BorderStyle)GetValue(StyleTypeProperty); }
            set { SetValue(StyleTypeProperty, value); }
        }

        public bool IsCloseButtonEnabled
        {
            get { return (bool)GetValue(IsCloseButtonEnabledProperty); }
            set { SetValue(IsCloseButtonEnabledProperty, value); }
        }

        public ImageBasedButton InformationButton
        {
            get { return (ImageBasedButton)GetValue(InformationButtonProperty); }
            set { SetValue(InformationButtonProperty, value); }
        }
    }
}

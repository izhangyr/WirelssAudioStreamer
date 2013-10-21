namespace WirelessAudioServer.Wpf.CustomControls
{
    public partial class SpinningWaitWindowView
    {
        public SpinningWaitWindowView()
        {
            InitializeComponent();
        }

        public string WaitTextValue
        {
            get { return WaitText.Text; }
            set { WaitText.Text = value; }
        }
    }
}






using System.Windows.Media;

namespace Replay.Common.Wpf.CustomControls.WpfFolderBrowseDialog.CustomControls
{
    public partial class ListViewItemLabeled
    {
        public ListViewItemLabeled()
        {
            InitializeComponent();
        }

        public string Header
        {
            get { return HeaderText.Text; }
            set { HeaderText.Text = value; }
        }

        public ImageSource IconSource
        {
            get { return Icon.Source; }
            set { Icon.Source = value; }
        }
    }
}

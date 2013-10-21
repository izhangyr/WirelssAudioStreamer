




using System.Windows.Media;

namespace Replay.Common.Wpf.CustomControls.WpfFolderBrowseDialog.CustomControls
{
    public partial class TreeViewItemLabeled
    {
        public TreeViewItemLabeled()
        {
            InitializeComponent();
        }

        public new string Header
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

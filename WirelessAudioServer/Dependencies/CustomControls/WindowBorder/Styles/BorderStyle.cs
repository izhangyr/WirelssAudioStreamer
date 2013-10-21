using System.Diagnostics.CodeAnalysis;

namespace WirelessAudioServer.Wpf.CustomControls.Styles
{
    public enum BorderStyle
    {
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Urc", Justification = "It's a product name.")]
        UrcBorder = 0,
        GeneralBorder = 1,
        MessageBoxBorder = 2,
        InstallerBorder = 3,
        MinimizedStyle = 4
    }
}
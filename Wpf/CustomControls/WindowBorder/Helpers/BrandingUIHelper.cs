using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WirelessAudioServer.Wpf.CustomControls.Helpers
{
    public class BrandingUIHelper
    {
        private const string BrandingFolder = "Resources";
        private const string WaterMarkFileName = "LicenseKeyEntryLogo.png";

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "That's correct, it's used as view model.")]
        public ImageSource WatermarkPath
        {
            get
            {
                var waterMarkPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), BrandingFolder, WaterMarkFileName);
                return File.Exists(waterMarkPath) ? new BitmapImage(new Uri(waterMarkPath)) : default(BitmapImage);
            }
        }
    }
}
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

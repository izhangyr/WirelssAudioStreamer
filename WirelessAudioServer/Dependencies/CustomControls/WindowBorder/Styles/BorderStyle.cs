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

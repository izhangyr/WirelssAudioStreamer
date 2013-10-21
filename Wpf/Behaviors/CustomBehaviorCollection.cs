using System.Windows;
using System.Windows.Interactivity;

namespace WirelessAudioServer.Wpf
{
    public class CustomBehaviorCollection : FreezableCollection<Behavior>
    {
        protected override Freezable CreateInstanceCore()
        {
            return new CustomBehaviorCollection();
        }
    }
}

using System;
using System.Windows;
using System.Windows.Media;

namespace WirelessAudioServer.Wpf
{
    public static class WpfElementsHelper
    {
         public static DependencyObject GetTopParent(DependencyObject child, Type parentType)
         {
             var parent = VisualTreeHelper.GetParent(child);
             while (!parent.GetType().IsSubclassOf(parentType) && parent.GetType() != parentType)
             {
                 parent = VisualTreeHelper.GetParent(parent);
             }

             return parent;
         }
    }
}
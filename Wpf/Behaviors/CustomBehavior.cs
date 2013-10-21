using System.Linq;
using System.Windows;
using System.Windows.Interactivity;

namespace WirelessAudioServer.Wpf
{
    public static class CustomBehavior
    {
        public static readonly DependencyProperty BehaviorsProperty = DependencyProperty.RegisterAttached(
            @"Behaviors",
            typeof(CustomBehaviorCollection),
            typeof(CustomBehavior),
            new FrameworkPropertyMetadata(null, OnPropertyChanged));

        private static readonly DependencyProperty OriginalBehaviorProperty = DependencyProperty.RegisterAttached(@"OriginalBehaviorInternal", typeof(Behavior), typeof(CustomBehavior), new UIPropertyMetadata(null));

        public static CustomBehaviorCollection GetBehaviors(DependencyObject element)
        {
            return (CustomBehaviorCollection)element.GetValue(BehaviorsProperty);
        }

        public static void SetBehaviors(DependencyObject element, CustomBehaviorCollection value)
        {
            element.SetValue(BehaviorsProperty, value);
        }

        private static Behavior GetOriginalBehavior(DependencyObject obj)
        {
            return obj.GetValue(OriginalBehaviorProperty) as Behavior;
        }

        private static int GetIndexOf(BehaviorCollection itemBehaviors, Behavior behavior)
        {
            var index = -1;

            var orignalBehavior = GetOriginalBehavior(behavior);

            for (var i = 0; i < itemBehaviors.Count; i++)
            {
                var currentBehavior = itemBehaviors[i];

                if (Equals(currentBehavior, behavior)
                    || Equals(currentBehavior, orignalBehavior))
                {
                    index = i;
                    break;
                }

                var currentOrignalBehavior = GetOriginalBehavior(currentBehavior);

                if (!Equals(currentOrignalBehavior, behavior) && !Equals(currentOrignalBehavior, orignalBehavior))
                {
                    continue;
                }

                index = i;
                break;
            }

            return index;
        }

        private static void OnPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var element = dependencyObject as UIElement;

            if (element == null)
            {
                return;
            }

            BehaviorCollection itemBehaviors = Interaction.GetBehaviors(element);

            var newBehaviors = e.NewValue as CustomBehaviorCollection;
            var oldBehaviors = e.OldValue as CustomBehaviorCollection;

            if (Equals(newBehaviors, oldBehaviors))
            {
                return;
            }

            if (oldBehaviors != null)
            {
                var reduntantBehaviors = oldBehaviors.Select(behavior => GetIndexOf(itemBehaviors, behavior)).Where(index => index >= 0);
                foreach (var index in reduntantBehaviors)
                {
                    itemBehaviors.RemoveAt(index);
                }
            }

            if (newBehaviors == null)
            {
                return;
            }

            foreach (var behavior in newBehaviors)
            {
                var index = GetIndexOf(itemBehaviors, behavior);

                if (index >= 0)
                {
                    continue;
                }

                var clone = (Behavior)behavior.Clone();
                SetOriginalBehavior(clone, behavior);
                itemBehaviors.Add(clone);
            }
        }

        private static void SetOriginalBehavior(DependencyObject obj, Behavior value)
        {
            obj.SetValue(OriginalBehaviorProperty, value);
        }
    }
}

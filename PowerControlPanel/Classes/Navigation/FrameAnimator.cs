using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using MahApps.Metro.Controls;

namespace Power_Control_Panel.PowerControlPanel.Classes.Navigation

{
    public class FrameAnimator
    {
        public static readonly DependencyProperty FrameNavigationStoryboardProperty
            = DependencyProperty.RegisterAttached(
                "FrameNavigationStoryboard",
                typeof(Storyboard),
                typeof(FrameAnimator),
                new FrameworkPropertyMetadata(null, OnFrameNavigationStoryboardChanged));

        private static void OnFrameNavigationStoryboardChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Frame frame && e.OldValue != e.NewValue)
            {
                frame.Navigating -= Frame_Navigating;
                if (e.NewValue is Storyboard)
                {
                    frame.Navigating += Frame_Navigating;
                }
            }
        }

        private static void Frame_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            if (sender is Frame frame)
            {
                var sb = GetFrameNavigationStoryboard(frame);
                if (sb != null)
                {
                    var presenter = frame.FindChild<ContentPresenter>();
                    sb.Begin((FrameworkElement)presenter ?? frame);
                }
            }
        }

        /// <summary>Helper for setting <see cref="FrameNavigationStoryboardProperty"/> on <paramref name="control"/>.</summary>
        /// <param name="control"><see cref="DependencyObject"/> to set <see cref="FrameNavigationStoryboardProperty"/> on.</param>
        /// <param name="storyboard">FrameNavigationStoryboard property value.</param>
        public static void SetFrameNavigationStoryboard(DependencyObject control, Storyboard storyboard)
        {
            control.SetValue(FrameNavigationStoryboardProperty, storyboard);
        }

        /// <summary>Helper for getting <see cref="FrameNavigationStoryboardProperty"/> from <paramref name="control"/>.</summary>
        /// <param name="control"><see cref="DependencyObject"/> to read <see cref="FrameNavigationStoryboardProperty"/> from.</param>
        /// <returns>FrameNavigationStoryboard property value.</returns>
        [AttachedPropertyBrowsableForType(typeof(DependencyObject))]
        public static Storyboard GetFrameNavigationStoryboard(DependencyObject control)
        {
            return (Storyboard)control.GetValue(FrameNavigationStoryboardProperty);
        }
    }
}
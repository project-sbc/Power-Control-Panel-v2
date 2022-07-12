using System;
using System.Windows;
using MahApps.Metro.Controls;

namespace Power_Control_Panel.PowerControlPanel.Classes.VMQAM
{
    public class MI: HamburgerMenuIconItem
    {
        /// <summary>Identifies the <see cref="NavigationDestination"/> dependency property.</summary>
        public static readonly DependencyProperty NavigationDestinationProperty
            = DependencyProperty.Register(
                nameof(NavigationDestination),
                typeof(Uri),
                typeof(MI),
                new PropertyMetadata(default(Uri)));

        public Uri NavigationDestination
        {
            get => (Uri)this.GetValue(NavigationDestinationProperty);
            set => this.SetValue(NavigationDestinationProperty, value);
        }

        /// <summary>Identifies the <see cref="NavigationType"/> dependency property.</summary>
        public static readonly DependencyProperty NavigationTypeProperty
            = DependencyProperty.Register(
                nameof(NavigationType),
                typeof(Type),
                typeof(MI),
                new PropertyMetadata(default(Type)));

        public Type NavigationType
        {
            get => (Type)this.GetValue(NavigationTypeProperty);
            set => this.SetValue(NavigationTypeProperty, value);
        }

        public bool IsNavigation => this.NavigationDestination != null;
    }
}

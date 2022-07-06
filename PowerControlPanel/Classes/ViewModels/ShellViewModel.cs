using System;
using System.Collections.ObjectModel;
using MahApps.Metro.IconPacks;
using Power_Control_Panel.PowerControlPanel.Classes.Mvvm;
using Power_Control_Panel.PowerControlPanel.Classes;
using Power_Control_Panel.PowerControlPanel.Pages;

namespace Power_Control_Panel.PowerControlPanel.Classes.ViewModels
{
    public class ShellViewModel : BindableBase
    {
        public ObservableCollection<MenuItem> Menu { get; } = new();

        public ObservableCollection<MenuItem> OptionsMenu { get; } = new();

        public ShellViewModel()
        {
            // Build the menus
            this.Menu.Add(new MenuItem()
            {
                Icon = new PackIconFontAwesome() { Kind = PackIconFontAwesomeKind.HomeSolid  },
                Label = "Home",
                NavigationType = typeof(MainPage),
                NavigationDestination = new Uri("PowerControlPanel/Pages/HomePage.xaml", UriKind.RelativeOrAbsolute)
            });
            this.Menu.Add(new MenuItem()
            {
                Icon = new PackIconFontAwesome() { Kind = PackIconFontAwesomeKind.BookSolid },
                Label = "Profiles",
                NavigationType = typeof(ProfilesPage),
                NavigationDestination = new Uri("PowerControlPanel/Pages/ProfilesPage.xaml", UriKind.RelativeOrAbsolute)
            });
            this.Menu.Add(new MenuItem()
            {
                Icon = new PackIconFontAwesome() { Kind = PackIconFontAwesomeKind.WindowRestoreSolid },
                Label = "Overlay",
               
            });
            this.Menu.Add(new MenuItem()
            {
                Icon = new PackIconFontAwesome() { Kind = PackIconFontAwesomeKind.KeyboardRegular },
                Label = "On Screen Keyboard",

            });
            this.OptionsMenu.Add(new MenuItem()
            {
                Icon = new PackIconFontAwesome() { Kind = PackIconFontAwesomeKind.InfoCircleSolid },
                Label = "About",
                NavigationType = typeof(AboutPage),
                NavigationDestination = new Uri("PowerControlPanel/Pages/AboutPage.xaml", UriKind.RelativeOrAbsolute)
            });
            this.OptionsMenu.Add(new MenuItem()
            {
                Icon = new PackIconFontAwesome() { Kind = PackIconFontAwesomeKind.CogSolid },
                Label = "Settings",
                NavigationType = typeof(SettingsPage),
                NavigationDestination = new Uri("PowerControlPanel/Pages/SettingsPage.xaml", UriKind.RelativeOrAbsolute)
            });
    
        }
    }
}
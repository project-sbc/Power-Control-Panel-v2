using System;
using System.Collections.ObjectModel;
using MahApps.Metro.IconPacks;
using Power_Control_Panel.PowerControlPanel.Classes.Mvvm;
using Power_Control_Panel.PowerControlPanel.Classes;
using Power_Control_Panel.PowerControlPanel.Pages;

namespace Power_Control_Panel.PowerControlPanel.Classes.VMQAM
{
    public class SVM : BindableBase
    {
        public ObservableCollection<MI> Menu { get; } = new();

        public ObservableCollection<MI> OptionsMenu { get; } = new();

        public SVM()
        {
            // Build the menus
            this.Menu.Add(new MI()
            {


                Icon = new PackIconFontAwesome() { Kind = PackIconFontAwesomeKind.SignOutAltSolid },

                Label = "Hide",

            });
            if (Properties.Settings.Default.homePageTypeQAM == "Slider")
            {
                this.Menu.Add(new MI()
                {
                    Icon = new PackIconFontAwesome() { Kind = PackIconFontAwesomeKind.HddSolid },

                    Label = "System",
                    NavigationType = typeof(MainPage),

                    NavigationDestination = new Uri("PowerControlPanel/Pages/HomePage.xaml", UriKind.RelativeOrAbsolute),


                });
            }
            else
            {
                this.Menu.Add(new MI()
                {
                    Icon = new PackIconFontAwesome() { Kind = PackIconFontAwesomeKind.HddSolid },

                    Label = "System",
                    NavigationType = typeof(MainPage),

                    NavigationDestination = new Uri("PowerControlPanel/Pages/QAMHomePage.xaml", UriKind.RelativeOrAbsolute),


                });
            }
 
            this.Menu.Add(new MI()
            {
                Icon = new PackIconFontAwesome() { Kind = PackIconFontAwesomeKind.GamepadSolid },
                Label = "Games",
                NavigationType = typeof(ProfilesPage),
                NavigationDestination = new Uri("PowerControlPanel/Pages/TileViewer.xaml", UriKind.RelativeOrAbsolute)
            });
            this.OptionsMenu.Add(new MI()
            {
                Icon = new PackIconFontAwesome() { Kind = PackIconFontAwesomeKind.CogSolid },
                Label = "Settings",
                NavigationType = typeof(SettingsPage),
                NavigationDestination = new Uri("PowerControlPanel/Pages/SettingsPage.xaml", UriKind.RelativeOrAbsolute)
            });

        }
    }
}

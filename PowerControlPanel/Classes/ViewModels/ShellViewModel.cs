using System;
using System.Collections.ObjectModel;
using MahApps.Metro.IconPacks;
using Power_Control_Panel.PowerControlPanel.Classes.Mvvm;
using Power_Control_Panel.PowerControlPanel.Classes;
using Power_Control_Panel.PowerControlPanel.Pages;
using System.Resources;
using System.Windows;

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
                Icon = new PackIconFontAwesome() { Kind = PackIconFontAwesomeKind.HomeSolid },
                Label = Application.Current.Resources["MainWindow_Menu_Home"].ToString(),
                NavigationType = typeof(MainPage),
                NavigationDestination = new Uri("PowerControlPanel/Pages/HomePage.xaml", UriKind.RelativeOrAbsolute),
            });

            this.Menu.Add(new MenuItem()
            {
                Icon = new PackIconFontAwesome() { Kind = PackIconFontAwesomeKind.BookSolid },
                Label = Application.Current.Resources["MainWindow_Menu_Profiles"].ToString(),
                NavigationType = typeof(ProfilesPage),
                NavigationDestination = new Uri("PowerControlPanel/Pages/ProfilesPage.xaml", UriKind.RelativeOrAbsolute)
            });
            this.Menu.Add(new MenuItem()
            {
                Icon = new PackIconFontAwesome() { Kind = PackIconFontAwesomeKind.ListAltRegular },
                Label = Application.Current.Resources["MainWindow_Menu_AppSettings"].ToString(),
                NavigationType = typeof(ProfilesPage),
                NavigationDestination = new Uri("PowerControlPanel/Pages/AppSettingsPage.xaml", UriKind.RelativeOrAbsolute)
            });
            this.Menu.Add(new MenuItem()
            {
                Icon = new PackIconFontAwesome() { Kind = PackIconFontAwesomeKind.WindowRestoreSolid },
                Label = Application.Current.Resources["MainWindow_Menu_QAM"].ToString(),

            });
            this.Menu.Add(new MenuItem()
            {
                Icon = new PackIconFontAwesome() { Kind = PackIconFontAwesomeKind.KeyboardRegular },
                Label = Application.Current.Resources["MainWindow_Menu_OSK"].ToString(),

            });
            //this.Menu.Add(new MenuItem()
            //{
                //Icon = new PackIconFontAwesome() { Kind = PackIconFontAwesomeKind.ThermometerThreeQuartersSolid },
                //Label = Application.Current.Resources["MainWindow_Menu_FanCurve"].ToString(),
               // NavigationType = typeof(FanCurvePage),
               // NavigationDestination = new Uri("PowerControlPanel/Pages/FanCurvePage.xaml", UriKind.RelativeOrAbsolute)
           // });
            this.OptionsMenu.Add(new MenuItem()
            {
                Icon = new PackIconFontAwesome() { Kind = PackIconFontAwesomeKind.InfoCircleSolid },
                Label = Application.Current.Resources["MainWindow_Menu_About"].ToString(),
                NavigationType = typeof(AboutPage),
                NavigationDestination = new Uri("PowerControlPanel/Pages/AboutPage.xaml", UriKind.RelativeOrAbsolute)
            });
            this.OptionsMenu.Add(new MenuItem()
            {
                Icon = new PackIconFontAwesome() { Kind = PackIconFontAwesomeKind.CogSolid },
                Label = Application.Current.Resources["MainWindow_Menu_Settings"].ToString(),
                NavigationType = typeof(SettingsPage),
                NavigationDestination = new Uri("PowerControlPanel/Pages/SettingsPage.xaml", UriKind.RelativeOrAbsolute)
            });
    
        }

       
    }
}
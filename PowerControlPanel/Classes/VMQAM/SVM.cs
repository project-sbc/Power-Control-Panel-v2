using System;
using System.Collections.ObjectModel;
using MahApps.Metro.IconPacks;
using Power_Control_Panel.PowerControlPanel.Classes.Mvvm;
using Power_Control_Panel.PowerControlPanel.Classes;
using Power_Control_Panel.PowerControlPanel.Pages;
using System.Windows;

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

                Label = Application.Current.Resources["QAM_Menu_Hide"].ToString(),

            });
        
                this.Menu.Add(new MI()
                {
                    Icon = new PackIconFontAwesome() { Kind = PackIconFontAwesomeKind.HddSolid },

                    Label = Application.Current.Resources["QAM_Menu_System"].ToString(),
                    NavigationType = typeof(MainPage),

                    NavigationDestination = new Uri("PowerControlPanel/Pages/QAMHomePage.xaml", UriKind.RelativeOrAbsolute),


                });
         

            this.OptionsMenu.Add(new MI()
            {
                Icon = new PackIconFontAwesome() { Kind = PackIconFontAwesomeKind.CogSolid },
                Label = Application.Current.Resources["QAM_Menu_Settings"].ToString(),
                NavigationType = typeof(SettingsPage),
                NavigationDestination = new Uri("PowerControlPanel/Pages/SettingsPage.xaml", UriKind.RelativeOrAbsolute)
            });

        }
    }
}

using ControlzEx.Theming;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Power_Control_Panel.PowerControlPanel.Pages
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();

            loadSettings();

            ThemeManager.Current.ChangeTheme(this, Properties.Settings.Default.systemTheme);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            //Save Settings

            //General settings
            Properties.Settings.Default.systemTheme = cboLightDarkTheme.Text + "." + cboAccentTheme.Text;






            //Save
            Properties.Settings.Default.Save();

            //Reapply theme
            ThemeManager.Current.ChangeTheme(this, Properties.Settings.Default.systemTheme);
        }

        private void loadSettings()
        {
            //Load general settings


            //handle theme
            int intPeriodLocation = Properties.Settings.Default.systemTheme.IndexOf(".");
            int intLengthTheme = Properties.Settings.Default.systemTheme.Length;
            cboAccentTheme.Text = Properties.Settings.Default.systemTheme.Substring(intPeriodLocation+1,intLengthTheme-(intPeriodLocation+1) );
            cboLightDarkTheme.Text = Properties.Settings.Default.systemTheme.Substring(0, intPeriodLocation);
            
        }

    }
}

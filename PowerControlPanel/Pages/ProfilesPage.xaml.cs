using ControlzEx.Theming;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Controls;
using System.Windows.Forms;

namespace Power_Control_Panel.PowerControlPanel.Pages
{
    /// <summary>
    /// Interaction logic for ProfilesPage.xaml
    /// </summary>
    public partial class ProfilesPage : Page
    {
        private Classes.ManageXML.ManageXML_Profiles xmlP;
        public ProfilesPage()
        {
            InitializeComponent();

            xmlP = new Classes.ManageXML.ManageXML_Profiles();
            loadListView();

            ThemeManager.Current.ChangeTheme(this, Properties.Settings.Default.systemTheme);
        }

        private void loadListView()
        {
            
            DataTable dt = xmlP.profileList();
            profileDataGrid.DataContext = dt.DefaultView;
           
        }

        private void btnAddProfile_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            xmlP.createProfile();
            loadListView();
        }
    }
}

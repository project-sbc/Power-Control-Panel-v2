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
        }

        private void loadListView()
        {
            profileDataGrid.Items.Clear();
            DataTable dt = xmlP.profileList();
            profileDataGrid.DataContext = dt.DefaultView;
           
        }
    }
}

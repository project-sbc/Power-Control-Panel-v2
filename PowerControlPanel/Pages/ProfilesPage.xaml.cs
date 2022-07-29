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
        public ProfilesPage()
        {
            InitializeComponent();
            loadListView();
        }

        private void loadListView()
        {
     

         

            DataSet dataSet = new DataSet();
            StringReader theReader = new StringReader(Properties.Resources.Profiles);

            dataSet.ReadXml(theReader);

            profileDataGrid.DataContext = dataSet.Tables[1].DefaultView;
       

        }
    }
}

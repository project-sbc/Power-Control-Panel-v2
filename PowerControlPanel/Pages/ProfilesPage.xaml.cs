using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
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
        }

        private void loadListView()
        {
          

            lvProfiles.View = View.Details;
            lvProfiles.GridLines = true;
            lvProfiles.Sorting = SortOrder.Descending;
            lvProfiles.Columns.Add("Active", 80);
            lvProfiles.Columns.Add("username", 120);
            lvProfiles.Columns.Add("Last Logon", 120);

            lvProfiles.Items.Clear();


            var doc = XDocument.Parse(Properties.Resources.Profiles);
            var output = from x in doc.Root.Elements("user")
                         select new ListViewItem(new[]
                         {
                             x.Element("USERID").Value,
                             x.Element("username").Value,
                             x.Element("lastlogon").Value

                         });
            lvProfiles.Items.AddRange(output.ToArray());

        }
    }
}

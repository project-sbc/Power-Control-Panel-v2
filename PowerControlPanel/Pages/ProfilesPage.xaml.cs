using ControlzEx.Theming;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using MahApps.Metro.Controls;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows;
using System.Windows.Media;

namespace Power_Control_Panel.PowerControlPanel.Pages
{
    /// <summary>
    /// Interaction logic for ProfilesPage.xaml
    /// </summary>
    public partial class ProfilesPage : Page
    {
        private Classes.ManageXML.ManageXML_Profiles xmlP;
        private string ProfileName = "";
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

        private void btnDeleteProfile_Click(object sender, System.Windows.RoutedEventArgs e)
        {
           

            xmlP.deleteProfile(ProfileName);
            loadListView();

            if (GlobalVariables.ActiveProfile == ProfileName) 
            { 
                GlobalVariables.ActiveProfile = "None";
 


            }
        }

        private void ToggleSwitch_Toggled(object sender, System.Windows.RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch != null)
            {
                string command = toggleSwitch.CommandParameter.ToString();
                string charger = command.Substring(0, command.IndexOf("_"));
                string parameter = command.Substring(command.IndexOf("_") + 1, command.Length - command.IndexOf("_") - 1);
                if (ProfileName != "")
                {



                    if (!toggleSwitch.IsOn)
                    {
                        DockPanel parentDP = getParentDockPanel(toggleSwitch);
                      

                        foreach (System.Windows.Controls.Control child in parentDP.Children)
                        {
                            if (child is Slider)
                            {
                                child.IsEnabled = false;
                                child.Opacity = 0.3;
                            }
                        }



                    }
                    else
                    {
                        DockPanel parentDP = getParentDockPanel(toggleSwitch);
        
                        xmlP.changeProfileParameter(charger, parameter, ProfileName, "");
                        foreach (System.Windows.Controls.Control child in parentDP.Children)
                        {
                            if (child is Slider)
                            {
                                child.IsEnabled = true;
                                child.Opacity = 1;
                            }
                        }

                    }



                }


            }


        }


        private DockPanel getParentDockPanel(DependencyObject toggle)
        {

            DockPanel returnDP = null;
            DependencyObject parent;
            if (toggle != null)
            {
                parent = VisualTreeHelper.GetParent(toggle);

                if (parent is not DockPanel)
                {
                    parent = VisualTreeHelper.GetParent(parent);
                    if (parent is not DockPanel)
                    {
                        parent = VisualTreeHelper.GetParent(parent);
                    }
                }

                returnDP = (DockPanel)parent;



            }

            return returnDP;


        }

        private void profileDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
     
            object item = profileDataGrid.SelectedItem;
            if (item != null)
            {
                string profileName = (profileDataGrid.SelectedCells[0].Column.GetCellContent(item) as TextBlock).Text;

                if (profileName != null)
                { ProfileSP.IsEnabled = true;
                    ProfileName = profileName;
                }
                else { ProfileSP.IsEnabled = false;
                    ProfileName = "";
                }

            }

        }

 
    }
}

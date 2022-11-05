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
using System.Windows.Controls.Primitives;
using System.Threading.Tasks;
using System.Windows.Threading;
using Power_Control_Panel.PowerControlPanel.Classes.ManageXML;

namespace Power_Control_Panel.PowerControlPanel.Pages
{
    /// <summary>
    /// Interaction logic for ProfilesPage.xaml
    /// </summary>
    public partial class ProfilesPage : Page
    {

        private string ProfileName = "";
        private string AppName = "";
        public ProfilesPage()
        {
            InitializeComponent();

            offline_sliderActiveCores.Maximum = GlobalVariables.maxCpuCores;
            online_sliderActiveCores.Maximum = GlobalVariables.maxCpuCores;
            offline_sliderMAXCPU.Minimum = GlobalVariables.baseCPUSpeed;
            online_sliderMAXCPU.Minimum = GlobalVariables.baseCPUSpeed;
            offline_sliderTDP1.Minimum = Properties.Settings.Default.minTDP;
            offline_sliderTDP2.Minimum = Properties.Settings.Default.minTDP;
            offline_sliderTDP1.Maximum = Properties.Settings.Default.maxTDP;
            offline_sliderTDP2.Maximum = Properties.Settings.Default.maxTDP;

            online_sliderTDP1.Minimum = Properties.Settings.Default.minTDP;
            offline_sliderTDP2.Minimum = Properties.Settings.Default.minTDP;
            online_sliderTDP1.Maximum = Properties.Settings.Default.maxTDP;
            offline_sliderTDP2.Maximum = Properties.Settings.Default.maxTDP;

            //populate profile list
            loadProfileListView();

            //change theme to match general theme
            ThemeManager.Current.ChangeTheme(this, Properties.Settings.Default.systemTheme);

            //hide AMD specific stuff if cpu is intel
            if (GlobalVariables.cpuType =="Intel")
            {
                online_GPUCLK_DP.Visibility = Visibility.Collapsed;
                offline_GPUCLK_DP.Visibility = Visibility.Collapsed;
            }


            //set max tdp on sliders
            offline_sliderTDP1.Maximum = Properties.Settings.Default.maxTDP;
            offline_sliderTDP2.Maximum = Properties.Settings.Default.maxTDP;
            online_sliderTDP1.Maximum = Properties.Settings.Default.maxTDP;
            online_sliderTDP2.Maximum = Properties.Settings.Default.maxTDP;
        }

        private void loadProfileListView()
        {
            
            DataTable dt = ManageXML_Profiles.profileList();
            profileDataGrid.DataContext = dt.DefaultView;
           
        }


        private void btnAddProfile_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ManageXML_Profiles.createProfile();
            loadProfileListView();
        }

        private void btnDeleteProfile_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            bool deleteProfile = true;



            if (ProfileName == "Default")
            {
                if (System.Windows.Forms.MessageBox.Show("Deleting the Default profile will disable having a default. Do you still want to delete it?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    deleteProfile = false;
                }


            }
            
            if (deleteProfile == true)
            {
                ManageXML_Profiles.deleteProfile(ProfileName);
                loadProfileListView();
                clearProfile();
                if (GlobalVariables.ActiveProfile == ProfileName)
                {
                    GlobalVariables.ActiveProfile = "None";

                }
            }

        }
        private void saveProfile()
        {
            if (ProfileName != "")
            {


              
                string[] result = new string[10];


                if (toggle_Online_TDP1.IsOn == true)
                {
                    result[0] = online_sliderTDP1.Value.ToString();

                }
                else { result[0] = ""; }

                if (toggle_Online_TDP2.IsOn == true)
                {
                    result[1] = online_sliderTDP2.Value.ToString();

                }
                else { result[1] = ""; }


                if (toggle_Online_GPUCLK.IsOn == true)
                {
                    result[2] = online_sliderGPUCLK.Value.ToString();

                }
                else { result[2] = ""; }




                if (toggle_Offline_TDP1.IsOn == true)
                {
                    result[3] = offline_sliderTDP1.Value.ToString();

                }
                else { result[3] = ""; }

                if (toggle_Offline_TDP2.IsOn == true)
                {
                    result[4] = offline_sliderTDP2.Value.ToString();

                }
                else { result[4] = ""; }

                if (toggle_Offline_GPUCLK.IsOn == true)
                {
                    result[5] = offline_sliderGPUCLK.Value.ToString();

                }
                else { result[5] = ""; }
                //
             
                if (toggle_Online_MAXCPU.IsOn == true)
                {
                    if (online_sliderMAXCPU.Value == online_sliderMAXCPU.Maximum) { result[6] = "0"; }
                    else { result[6] = online_sliderMAXCPU.Value.ToString(); }
                 }
                else { result[6] = ""; }

                if (toggle_Offline_MAXCPU.IsOn == true)
                {
                    if (offline_sliderMAXCPU.Value == offline_sliderMAXCPU.Maximum) { result[8] = "0"; }
                    else { result[8] = offline_sliderMAXCPU.Value.ToString(); }
                }
                else { result[8] = ""; }


                if (toggle_Online_ActiveCores.IsOn == true)
                {
                    result[7] = online_sliderActiveCores.Value.ToString();

                }
                else { result[7] = ""; }
                if (toggle_Offline_ActiveCores.IsOn == true)
                {
                    result[9] = offline_sliderActiveCores.Value.ToString();

                }
                else { result[9] = ""; }


                ManageXML_Profiles.saveProfileArray(result,ProfileName);

                //check if profile name has changed! if yes, update any applications or active profiles with new name
                if (ProfileName != txtbxProfileName.Text)
                {
                    //if not match, then name was changed. Update profile name in profiles  section of XML. Update all apps with profilename
                    ManageXML_Profiles.changeProfileName(ProfileName, txtbxProfileName.Text);
                    ManageXML_Apps.changeProfileNameInApps(ProfileName, txtbxProfileName.Text);

                    loadProfileListView();

                    //make app profile reload just in case for picking active profile
                    GlobalVariables.updateProfileAppTable = true;

                    //if active profile name is the one changed, then update profile
                    if (GlobalVariables.ActiveProfile == ProfileName) 
                    { 
                        GlobalVariables.ActiveProfile = txtbxProfileName.Text;
                        System.Windows.MessageBox.Show("MAKE CODE TO RUN PROFILE AFTER PROFILE UPDATE");
                    }
                }

                savedMessage();
            }

        }

        DispatcherTimer dispatcherTimer = new DispatcherTimer();
        private void savedMessage()
        {
            lblSaved.Visibility = Visibility.Visible;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 4);
            dispatcherTimer.Tick += timerTickHideLabel;
            dispatcherTimer.Start();


        }
        private void timerTickHideLabel(object sender, EventArgs e)
        {
            dispatcherTimer.Stop();
            lblSaved.Visibility = Visibility.Collapsed;
        }
        private void loadProfile()
        {
            if (ProfileName != "")
            {

                txtbxProfileName.Text = ProfileName;
                string[] result = ManageXML_Profiles.loadProfileArray(ProfileName);


                loadProfileAppList();   

                if (result[0] != string.Empty)
                {
                    toggle_Online_TDP1.IsOn= true;
                    online_sliderTDP1.Value = Int32.Parse(result[0]);
                }
                else
                {
                    toggle_Online_TDP1.IsOn = false;
                    online_sliderTDP1.Value = online_sliderTDP1.Minimum;
                }



                if (result[1] != string.Empty)
                {
                    toggle_Online_TDP2.IsOn = true;
                    online_sliderTDP2.Value = Int32.Parse(result[1]);
                }
                else
                {
                    toggle_Online_TDP2.IsOn = false;
                    online_sliderTDP2.Value = online_sliderTDP2.Minimum;
                }

                if (result[2] != string.Empty)
                {
                    toggle_Online_GPUCLK.IsOn = true;
                    online_sliderGPUCLK.Value = Int32.Parse(result[2]);
                }
                else
                {
                    toggle_Online_GPUCLK.IsOn = false;
                    online_sliderGPUCLK.Value = online_sliderTDP2.Minimum;
                }



                if (result[3] != string.Empty)
                {
                    toggle_Offline_TDP1.IsOn = true;
                    offline_sliderTDP1.Value = Int32.Parse(result[3]);
                }
                else
                {
                    toggle_Offline_TDP1.IsOn = false;
                    offline_sliderTDP1.Value = offline_sliderTDP1.Minimum;
                }


                if (result[4] != string.Empty)
                {
                    toggle_Offline_TDP2.IsOn = true;
                    offline_sliderTDP2.Value = Int32.Parse(result[4]);
                }
                else
                {
                    toggle_Offline_TDP2.IsOn = false;
                    offline_sliderTDP2.Value = offline_sliderTDP1.Minimum;
                }

                if (result[5] != string.Empty)
                {
                    toggle_Offline_GPUCLK.IsOn = true;
                    offline_sliderGPUCLK.Value = Int32.Parse(result[5]);
                }
                else
                {
                    toggle_Offline_GPUCLK.IsOn = false;
                    offline_sliderGPUCLK.Value = offline_sliderTDP2.Minimum;
                }
                //
                if (result[6] != string.Empty)
                {
                    toggle_Online_MAXCPU.IsOn = true;
                    if (result[6] == "0") { online_sliderMAXCPU.Value = online_sliderMAXCPU.Maximum; }
                    else { online_sliderMAXCPU.Value = Int32.Parse(result[6]); }
                }
                else
                {
                    toggle_Online_MAXCPU.IsOn = false;
                    online_sliderMAXCPU.Value = online_sliderMAXCPU.Minimum;
                }
                if (result[8] != string.Empty)
                {
                    toggle_Offline_MAXCPU.IsOn = true;
                    if (result[8] == "0") { offline_sliderMAXCPU.Value = offline_sliderMAXCPU.Maximum; }
                    else { offline_sliderMAXCPU.Value = Int32.Parse(result[8]); }
                   
                }
                else
                {
                    toggle_Offline_MAXCPU.IsOn = false;
                    offline_sliderMAXCPU.Value = offline_sliderMAXCPU.Minimum;
                }




                if (result[7] != string.Empty)
                {
                    toggle_Online_ActiveCores.IsOn = true;
                    online_sliderActiveCores.Value = Int32.Parse(result[7]);
                }
                else
                {
                    toggle_Online_ActiveCores.IsOn = false;
                    online_sliderActiveCores.Value = online_sliderActiveCores.Minimum;
                }
                if (result[9] != string.Empty)
                {
                    toggle_Offline_ActiveCores.IsOn = true;
                    offline_sliderActiveCores.Value = Int32.Parse(result[9]);
                }
                else
                {
                    toggle_Offline_ActiveCores.IsOn = false;
                    offline_sliderActiveCores.Value = offline_sliderActiveCores.Minimum;
                }

            }

        }

    
        private void clearProfile()
        {
            txtbxProfileName.Text = string.Empty;
            offline_sliderTDP1.Value = offline_sliderTDP1.Minimum;
            offline_sliderTDP2.Value = offline_sliderTDP2.Minimum;
            online_sliderTDP1.Value = online_sliderTDP1.Minimum;
            online_sliderTDP2.Value = online_sliderTDP2.Minimum;
            offline_sliderGPUCLK.Value = offline_sliderGPUCLK.Minimum;
            online_sliderGPUCLK.Value = online_sliderGPUCLK.Minimum;
            offline_sliderActiveCores.Value = offline_sliderActiveCores.Minimum;
            online_sliderActiveCores.Value = online_sliderActiveCores.Minimum;
            offline_sliderMAXCPU.Value = offline_sliderMAXCPU.Minimum;
            online_sliderMAXCPU.Value = online_sliderMAXCPU.Minimum;
            toggle_Offline_TDP1.IsOn = false;
            toggle_Offline_TDP2.IsOn = false;
            toggle_Online_TDP1.IsOn = false;
            toggle_Online_TDP2.IsOn=false;
            toggle_Offline_GPUCLK.IsOn = false;
            toggle_Online_GPUCLK.IsOn=false;

            toggle_Offline_ActiveCores.IsOn=false;
            toggle_Online_ActiveCores.IsOn = false;
            toggle_Offline_MAXCPU.IsOn=false;
            toggle_Online_MAXCPU.IsOn = false;
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

                        ManageXML_Profiles.changeProfileParameter(charger, parameter, ProfileName, "");
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
        private DependencyObject GetElementFromParent(DependencyObject parent, string childname)
        {

            //Use element parent for thumb size control on slider
            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is FrameworkElement childframeworkelement && childframeworkelement.Name == childname)
                    return child;

                var FindRes = GetElementFromParent(child, childname);
                if (FindRes != null)
                    return FindRes;
            }
            return null;
        }
        private void Slider_Loaded(object sender, RoutedEventArgs e)
        {
            var SliderThumb = GetElementFromParent(sender as DependencyObject, "HorizontalThumb"); //Make sure to put the right name for your slider layout options are: ("VerticalThumb", "HorizontalThumb")
            if (SliderThumb != null)
            {
                if (SliderThumb is Thumb thumb)
                {
                    thumb.Width = 20;
                    thumb.Height = 25;
                }
                else { }
            }
            else { }
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

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;
            string dgName = dataGrid.Name;
            object item = dataGrid.SelectedItem;
            
            if (item != null)
            {

                string objectName = (dataGrid.SelectedCells[0].Column.GetCellContent(item) as TextBlock).Text;
                if (objectName != null)
                {
                    ProfileSP.IsEnabled = true;
                    ProfileName = objectName;
                    loadProfile();
                }
                else
                {
                    ProfileSP.IsEnabled = false;
                    ProfileName = "";
                    clearProfile();
                }

            }

    
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            saveProfile();
        }

        private void loadProfileAppList()
        {
            DataTable dt = ManageXML_Apps.appListByProfile(ProfileName);
            profileAppDataGrid.DataContext = dt.DefaultView;
        }
        private void btnAddAppProfile_Click(object sender, RoutedEventArgs e)
        {
            ManageXML_Apps.createApp(ProfileName);
            loadProfileAppList();
        }

        private void btnDeleteAppProfile_Click(object sender, RoutedEventArgs e)
        {
            object item = profileAppDataGrid.SelectedItem;

            if (item != null)
            {
                string objectName = (profileAppDataGrid.SelectedCells[0].Column.GetCellContent(item) as TextBlock).Text;
                ManageXML_Apps.changeAppParameter("Profile", objectName, "");
                loadProfileAppList();
            }
           
        }

        private void sliderMAXCPU_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider slider = (Slider)sender;
            switch(slider.Name)
            {
                case "offline_sliderMAXCPU":
                    if (slider.Value == slider.Maximum)
                    {
                        offline_txtMAXCPUAuto.Visibility = Visibility.Visible;
                        offline_txtMAXCPU.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        offline_txtMAXCPUAuto.Visibility = Visibility.Collapsed;
                        offline_txtMAXCPU.Visibility = Visibility.Visible;
                    }
                    break;
                case "online_sliderMAXCPU":
                    if (slider.Value == slider.Maximum)
                    {
                        online_txtMAXCPUAuto.Visibility = Visibility.Visible;
                        online_txtMAXCPU.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        online_txtMAXCPUAuto.Visibility = Visibility.Collapsed;
                        online_txtMAXCPU.Visibility = Visibility.Visible;
                    }
                    break;
                default:
                    break;

            }
                
        }
    }
}

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
using System.Windows.Threading;
using Power_Control_Panel.PowerControlPanel.Classes.ManageXML;
namespace Power_Control_Panel.PowerControlPanel.Pages
{
    /// <summary>
    /// Interaction logic for AppSettingsPage.xaml
    /// </summary>
    public partial class AppSettingsPage : Page
    {
  


        private string AppName = "";
        public AppSettingsPage()
        {
            InitializeComponent();

            //initialize xml management class

            //populate profile list
   
            loadAppListView();
            //change theme to match general theme
            ThemeManager.Current.ChangeTheme(this, Properties.Settings.Default.systemTheme);

        }


        private void loadAppListView()
        {

            DataTable dt = ManageXML_Apps.appList();
            appDataGrid.DataContext = dt.DefaultView;

        }
        private void loadProcessNameCBO()
        {
            List<string> listProcesses = new List<string>();

            Process[] pList = Process.GetProcesses();
            foreach (Process p in pList)
            {
                if (p.MainWindowHandle != IntPtr.Zero)
                {
                    if (!listProcesses.Contains(p.ProcessName))
                    {
                        listProcesses.Add(p.ProcessName);

                    }

                }

            }

            cboProcessName.ItemsSource = listProcesses;

        }


        private void loadApp()
        {
            if (AppName != "")
            {
                loadProcessNameCBO();

                txtbxAppName.Text = AppName;
                string[] result = ManageXML_Apps.loadAppArray(AppName);


                List<string> listProfiles = ManageXML_Profiles.profileListForAppCBO();
                cboAppProfile.ItemsSource = listProfiles;

                if (result[0] != string.Empty)
                {
                    cboProcessName.Text = result[0];
                }
                else
                {
                    cboProcessName.Text = String.Empty;
                }
                if (result[5] != string.Empty)
                {
                    cboAppProfile.Text = result[5];
                }
                else
                {
                    cboAppProfile.Text = String.Empty;
                }


                if (result[2] != string.Empty)
                {
                    cboAppType.Text = result[2];
                    if (result[2] == "Game") { dpGameType.Visibility = Visibility.Visible; } else { dpGameType.Visibility = Visibility.Collapsed; }
 
                    dpImagePath.Visibility = Visibility.Visible;
                    dpExePath.Visibility = Visibility.Visible;
                }
                else
                {
                    cboAppType.Text = String.Empty;
                    dpGameType.Visibility = Visibility.Collapsed;
                    dpImagePath.Visibility = Visibility.Collapsed;
                    dpExePath.Visibility = Visibility.Collapsed;


                }

                if (result[3] != string.Empty)
                {
                    cboGameType.Text = result[3];
                }
                else
                {
                    cboGameType.Text = String.Empty;
                }



                if (result[1] != string.Empty)
                {
                    txtbxExePath.Text = result[1];
                }
                else
                {
                    txtbxExePath.Text = String.Empty;
                }


                if (result[4] != string.Empty)
                {
                    txtbxImagePath.Text = result[4];
                }
                else
                {
                    txtbxImagePath.Text = String.Empty;
                }
                if (result[2] == string.Empty & result[1] == string.Empty & result[4] == string.Empty & result[3] == string.Empty)
                {
                    enableExeStart.IsOn = false;
                }
                else
                {
                    enableExeStart.IsOn = true;

                }

            }

        }
       
        private void clearApp()
        {
            txtbxAppName.Text = string.Empty;
            cboAppProfile.Text = string.Empty;
            cboAppType.Text = string.Empty;
            cboGameType.Text = string.Empty;
            txtbxExePath.Text = string.Empty;
            txtbxImagePath.Text = string.Empty;
            cboProcessName.Text = string.Empty;
            enableExeStart.IsOn = false;


        }
      
      
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
         
            object item = appDataGrid.SelectedItem;
   
            if (item != null)
            {
                string objectName = (appDataGrid.SelectedCells[0].Column.GetCellContent(item) as TextBlock).Text;
                if (objectName != null)
                {
                    AppSP.IsEnabled = true;
                    AppName = objectName;
                    loadApp();
                }
                else
                {
                    AppSP.IsEnabled = false;
                    AppName = "";
                    clearApp();
                }

            }

        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            saveProfile();
        }
        private void saveProfile()
        {
            if (AppName != "")
            {

                string[] result = new string[6];

                result[0] = cboProcessName.Text;
                result[1] = txtbxExePath.Text;
                result[2] = cboAppType.Text;
                result[3] = cboGameType.Text;
                result[4] = txtbxImagePath.Text;
                result[5] = cboAppProfile.Text;

                GlobalVariables.updateProfileAppTable = true;

                ManageXML_Apps.saveAppArray(result, AppName);

                //check if profile name has changed! if yes, update any applications or active profiles with new name
                if (AppName != txtbxAppName.Text)
                {
                    //if not match, then name was changed. Update profile name in profiles  section of XML. Update all apps with profilename
                    ManageXML_Apps.changeAppParameter("DisplayName", AppName, txtbxAppName.Text);

                    loadAppListView();

                    //if active profile name is the one changed, then update profile
                    if (GlobalVariables.ActiveApp == AppName)
                    {
                        GlobalVariables.ActiveApp = txtbxAppName.Text;
                        
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
        private void btnAddProfile_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ManageXML_Apps.createApp();
            loadAppListView();
        }

        private void btnDeleteProfile_Click(object sender, System.Windows.RoutedEventArgs e)
        {

            ManageXML_Apps.deleteApp(AppName);
            loadAppListView();
            clearApp();
          
        }

        private void enableExeStart_Toggled(object sender, RoutedEventArgs e)
        {
            if (this.IsLoaded)
            {
                if (enableExeStart.IsOn)
                {
                    GB_AppExePath.Height = Double.NaN;

                }
                else
                {
                    GB_AppExePath.Height = 40;

                }

            }
        }

        private void cboAppType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem cboI = cboAppType.SelectedItem as ComboBoxItem;

            if (cboI != null)
            {
                string value = (string)cboI.Content;

                if (value == "Game")
                {
                    dpGameType.Visibility = Visibility.Visible;
                    dpImagePath.Visibility = Visibility.Visible;
                    dpExePath.Visibility = Visibility.Visible;
                }
                else
                {
                    dpGameType.Visibility = Visibility.Collapsed;
                }
                if (value == "")
                {
                    dpGameType.Visibility = Visibility.Collapsed;
                    dpImagePath.Visibility = Visibility.Collapsed;
                    dpExePath.Visibility = Visibility.Collapsed;
                }
                if (value == "Application")
                {
                    dpImagePath.Visibility = Visibility.Visible;
                    dpExePath.Visibility = Visibility.Visible;
                }


            }
         
        }

        private void db_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button button = (System.Windows.Controls.Button)sender;

            // Create OpenFileDialog
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();
      
            openFileDlg.InitialDirectory = @"C:\";
    
            // Launch OpenFileDialog by calling ShowDialog method
            
            // Get the selected file name and display in a TextBox.
            // Load content of file in a TextBlock

            if (button.Name == "dbExePath")
            {
                openFileDlg.DefaultExt = ".exe";
                openFileDlg.Filter = "Executables (.exe)|*.exe";

            }
            if (button.Name == "dbImagePath")
            {
                openFileDlg.Filter = "Image jpeg(*.jpg)|*.jpg|Image png(*.png)|*.png";
                openFileDlg.DefaultExt = ".jpeg";

            }
   

            Nullable<bool> result = openFileDlg.ShowDialog();

            if (result == true)
            {

                if (button.Name == "dbExePath")
                {
                    txtbxExePath.Text = openFileDlg.FileName;

                }
                if (button.Name == "dbImagePath")
                {
                    txtbxImagePath.Text = openFileDlg.FileName;

                }

            }

        }
    }
}


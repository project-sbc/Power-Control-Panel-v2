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
    /// Interaction logic for AppSettingsPage.xaml
    /// </summary>
    public partial class AppSettingsPage : Page
    {
  

        private Classes.ManageXML.ManageXML_Profiles xmlP;
        private Classes.ManageXML.ManageXML_Apps xmlA;

        private string AppName = "";
        public AppSettingsPage()
        {
            InitializeComponent();

            //initialize xml management class
            xmlP = new Classes.ManageXML.ManageXML_Profiles();
            xmlA = new Classes.ManageXML.ManageXML_Apps();
            //populate profile list
   
            loadAppListView();
            //change theme to match general theme
            ThemeManager.Current.ChangeTheme(this, Properties.Settings.Default.systemTheme);

        }


        private void loadAppListView()
        {

            DataTable dt = xmlA.appList();
            appDataGrid.DataContext = dt.DefaultView;

        }

    

        private void loadApp()
        {
            if (AppName != "")
            {

                txtbxAppName.Text = AppName;
                string[] result = xmlA.loadAppArray(AppName);


                List<string> listProfiles = xmlP.profileListForAppCBO();
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
                }
                else
                {
                    cboAppType.Text = String.Empty;
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


        }
        private void btnAddProfile_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            xmlA.createApp();
            loadAppListView();
        }

        private void btnDeleteProfile_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            bool deleteApp = true;

            xmlA.deleteApp(AppName);
            loadAppListView();
            clearApp();
          
        }

        private void enableExeStart_Toggled(object sender, RoutedEventArgs e)
        {
            if (this.IsLoaded)
            {
                if (enableExeStart.IsOn)
                {
                    GB_AppExePath.Height = 100;

                }
                else
                {
                    GB_AppExePath.Height = 40;

                }

            }
        }
    }
}


using AutoUpdaterDotNET;
using ControlzEx.Theming;
using MahApps.Metro.Controls;
using Microsoft.Win32.TaskScheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

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

            hideSettings();
        }

        private void hideSettings()
        {
            if (GlobalVariables.cpuType == "AMD")
            {
                GB_INTEL_TDP.Visibility = Visibility.Collapsed;
            }
            if (GlobalVariables.cpuType == "Intel")
            {
                GB_AMD_GPUCLK.Visibility = Visibility.Collapsed;
            }
        }
     

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            //Save Settings

            //General settings
            Properties.Settings.Default.systemTheme = cboLightDarkTheme.Text + "." + cboAccentTheme.Text;

            if (Properties.Settings.Default.systemAutoStart != cboAutoStart.Text)
            {
                Properties.Settings.Default.systemAutoStart = cboAutoStart.Text;
                changeTaskService(cboAutoStart.Text);
            }
            

            if (Properties.Settings.Default.Language != cboLanguage.Text)
            {
                System.Windows.Application.Current.Resources.MergedDictionaries.Remove(GlobalVariables.languageDict);
                switch (cboLanguage.Text)
                {
                    default:
                    case "English":
                        GlobalVariables.languageDict.Source = new Uri("PowerControlPanel/Classes/StartUp/Resources/StringResources.xaml", UriKind.RelativeOrAbsolute);
                        break;

                    case "中文":
                        GlobalVariables.languageDict.Source = new Uri("PowerControlPanel/Classes/StartUp/Resources/StringResources.zh-Hans.xaml", UriKind.RelativeOrAbsolute);
                        break;
                }
                System.Windows.Application.Current.Resources.MergedDictionaries.Add(GlobalVariables.languageDict);
                Properties.Settings.Default.Language = cboLanguage.Text;
            }


            Properties.Settings.Default.maxTDP = (int)TDPMAX.Value;
            Properties.Settings.Default.minTDP = (int)TDPMIN.Value;
            Properties.Settings.Default.sizeQAM = cboQAMSize.Text;

            Properties.Settings.Default.IntelMMIOMSR = cboTDPTypeIntel.Text;

            Properties.Settings.Default.maxGPUCLK = (int)GPUCLKMAX.Value;

            Properties.Settings.Default.enableCombineTDP = cboCombineTDP.Text;

            Properties.Settings.Default.homePageTypeMW = cboMWHomePageStyle.Text;

            Properties.Settings.Default.homePageTypeQAM = cboQAMHomePageStyle.Text;
            //Save
            Properties.Settings.Default.Save();

            //Reapply theme
            ThemeManager.Current.ChangeTheme(this, Properties.Settings.Default.systemTheme);
            
            savedMessage();

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
        private void loadSettings()
        {
            //Load general settings

            //handle theme
            int intPeriodLocation = Properties.Settings.Default.systemTheme.IndexOf(".");
            int intLengthTheme = Properties.Settings.Default.systemTheme.Length;
            cboAccentTheme.Text = Properties.Settings.Default.systemTheme.Substring(intPeriodLocation+1,intLengthTheme-(intPeriodLocation+1) );
            cboLightDarkTheme.Text = Properties.Settings.Default.systemTheme.Substring(0, intPeriodLocation);
            TDPMAX.Value = Properties.Settings.Default.maxTDP;
            TDPMIN.Value = Properties.Settings.Default.minTDP;
            GPUCLKMAX.Value = Properties.Settings.Default.maxGPUCLK;
            cboQAMSize.Text = Properties.Settings.Default.sizeQAM;
            cboCombineTDP.Text = Properties.Settings.Default.enableCombineTDP;
            cboAutoStart.Text = Properties.Settings.Default.systemAutoStart;
            cboLanguage.Text = Properties.Settings.Default.Language;
            cboMWHomePageStyle.Text = Properties.Settings.Default.homePageTypeMW;

            cboQAMHomePageStyle.Text = Properties.Settings.Default.homePageTypeQAM;
            cboTDPTypeIntel.Text = Properties.Settings.Default.IntelMMIOMSR;
        }
        private void changeTaskService(string systemAutoStart)
        {
            Microsoft.Win32.TaskScheduler.TaskService ts = new Microsoft.Win32.TaskScheduler.TaskService();
            Microsoft.Win32.TaskScheduler.Task task = ts.GetTask("Power_Control_Panel");
            string BaseDir = AppDomain.CurrentDomain.BaseDirectory;
            if (task == null)
            {
                if (systemAutoStart == "Enable")
                {
                    TaskDefinition td = ts.NewTask();

                    td.RegistrationInfo.Description = "Power Control Panel";
                    td.Triggers.AddNew(TaskTriggerType.Logon);
                    td.Principal.RunLevel = TaskRunLevel.Highest;
                    td.Settings.DisallowStartIfOnBatteries = false;
                    td.Settings.StopIfGoingOnBatteries = false;
                    td.Settings.RunOnlyIfIdle = false;

                    td.Actions.Add(new ExecAction(BaseDir + "\\Power Control Panel.exe"));

                    Microsoft.Win32.TaskScheduler.TaskService.Instance.RootFolder.RegisterTaskDefinition("Power_Control_Panel", td);

                }
            }

            else
            {
                if (systemAutoStart == "Disable")
                {
                    task.RegisterChanges();
                    ts.RootFolder.DeleteTask("Power_Control_Panel");
                }
            }

        }


        private DependencyObject GetElementFromParent(DependencyObject parent, string childname)
        {
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
                else
                {
                    //SliderThumb is not an object of type Thumb
                }
            }
            else
            {
                //SliderThumb is null
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            
           
            AutoUpdater.CheckForUpdateEvent += AutoUpdaterOnCheckForUpdateEvent;
            AutoUpdater.Start("https://raw.githubusercontent.com/project-sbc/Power-Control-Panel-v2/master/Update.xml");


        }

        private void AutoUpdaterOnCheckForUpdateEvent(UpdateInfoEventArgs args)
        {
            if (args.Error == null)
            {
                if (args.IsUpdateAvailable)
                {
                    DialogResult dialogResult;
                    if (args.Mandatory.Value)
                    {
                        dialogResult =
                            System.Windows.Forms.MessageBox.Show(
                                $@"There is new version {args.CurrentVersion} available. You are using version {args.InstalledVersion}. This is required update. Press Ok to begin updating the application.", @"Update Available",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                    }
                    else
                    {
                        dialogResult =
                            System.Windows.Forms.MessageBox.Show(
                                $@"There is new version {args.CurrentVersion} available. You are using version {
                                        args.InstalledVersion
                                    }. Do you want to update the application now?", @"Update Available",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Information);
                    }

                    // Uncomment the following line if you want to show standard update dialog instead.
                    // AutoUpdater.ShowUpdateForm(args);

                    if (dialogResult.Equals(System.Windows.Forms.DialogResult.Yes) || dialogResult.Equals(System.Windows.Forms.DialogResult.OK))
                    {
                        try
                        {


                            if (AutoUpdater.DownloadUpdate(args))
                            {
                                GlobalVariables.closeForUpdate = true;
                            }
                        }
                        catch (Exception exception)
                        {
                            System.Windows.Forms.MessageBox.Show(exception.Message, exception.GetType().ToString(), MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show(@"There is no update available please try again later.", @"No update available",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                if (args.Error is WebException)
                {
                    System.Windows.Forms.MessageBox.Show(
                        @"There is a problem reaching update server. Please check your internet connection and try again later.",
                        @"Update Check Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show(args.Error.Message,
                        args.Error.GetType().ToString(), MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        private void toggleGeneral_Toggled(object sender, RoutedEventArgs e)
        {
            if (toggleGeneral.IsOn)
            {
                GB_General_Settings.Height = double.NaN;
            }
            else
            {
                GB_General_Settings.Height = 40;
            }
            
        }
    }
}

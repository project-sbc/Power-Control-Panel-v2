using MahApps.Metro.Controls;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using Power_Control_Panel.PowerControlPanel.Classes.Navigation;
using Power_Control_Panel.PowerControlPanel.Classes.ChangeTDP;
using MenuItem = Power_Control_Panel.PowerControlPanel.Classes.ViewModels.MenuItem;
using System.Windows.Threading;
using System.Threading;

namespace Power_Control_Panel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public static class GlobalVariables
    {
        public static double PL1 = 0;
        public static double PL2 = 0;
        public static bool needTDPRead = false;
        

    }

    public partial class MainWindow : MetroWindow
    {
        private NavigationServiceEx navigationServiceEx;
        public Window overlay = new Overlay();
        public DispatcherTimer inputCheck=new DispatcherTimer();
        public SecretNest.TaskSchedulers.SequentialScheduler scheduler = new SecretNest.TaskSchedulers.SequentialScheduler();
        public Thread taskScheduler;
        public MainWindow()
        {
            this.InitializeComponent();

            startScheduler();

  
            //Run code to set up hamburger menu
            initializeNavigationFrame();

            //Run code to set up dispatch timers, one for inputcheck (i.e. xinput or keyboard prompts) and one for updating TDP values
            initializeDispatchTimersAndBackgroundThread();


           runTask(() => ChangeTDP.readTDP2());
        }

        void startScheduler()
        {
            scheduler = new SecretNest.TaskSchedulers.SequentialScheduler(true);

            taskScheduler = new Thread(MyThreadJob);
            taskScheduler.Start();
        }

        void MyThreadJob()
        {
            //...

            scheduler.Run(); //This will block this thread until the scheduler disposed.
        }
        public void runTask(Action action)
        {
            var taskFactory = new TaskFactory(scheduler);
            var result = taskFactory.StartNew(action);

        }
        void initializeDispatchTimersAndBackgroundThread()
        {
 
            //Set up timespan for timers
            inputCheck.Interval= new TimeSpan(0, 0, 1); 
       
            //Add the event handlers to the timers
            inputCheck.Tick += inputCheck_Tick;
            
            //Start timers
            inputCheck.Start();


            
        }

        void inputCheck_Tick(object sender, EventArgs e)
        {

        }

      
        private async void updateTDP()
        {
            Task<string> taskTDP = ChangeTDP.readTDP();
            string tdp = await taskTDP;
            if (tdp != null)
            {
                GlobalVariables.PL1 = Convert.ToDouble(tdp.Substring(0, tdp.IndexOf(";")));
                GlobalVariables.PL2 = Convert.ToDouble(tdp.Substring(tdp.IndexOf(";") + 1, tdp.Length - tdp.IndexOf(";") - 1));
            }
        }

        void initializeNavigationFrame()
        {
            navigationServiceEx = new NavigationServiceEx();
            navigationServiceEx.Navigated += this.NavigationServiceEx_OnNavigated;
            HamburgerMenuControl.Content = this.navigationServiceEx.Frame;



            // Navigate to the home page.
         
            this.Loaded += (sender, args) => this.navigationServiceEx.Navigate(new Uri("PowerControlPanel/Pages/HomePage.xaml", UriKind.RelativeOrAbsolute));

        }


        private void HamburgerMenuControl_OnItemInvoked(object sender, HamburgerMenuItemInvokedEventArgs e)
        {

            if (e.InvokedItem is MenuItem menuItem)
            {
                if (menuItem.Label == "Overlay")
                {
                    overlay.Show();

                }
                if (menuItem.IsNavigation)
                {
                    this.navigationServiceEx.Navigate(menuItem.NavigationDestination);
                }

            }
        }

        private void NavigationServiceEx_OnNavigated(object sender, NavigationEventArgs e)
        {
            // select the menu item
            this.HamburgerMenuControl.SetCurrentValue(HamburgerMenu.SelectedItemProperty,
                this.HamburgerMenuControl.Items
                    .OfType<MenuItem>()
                    .FirstOrDefault(x => x.NavigationDestination == e.Uri));
            this.HamburgerMenuControl.SetCurrentValue(HamburgerMenu.SelectedOptionsItemProperty,
                this.HamburgerMenuControl
                    .OptionsItems
                    .OfType<MenuItem>()
                    .FirstOrDefault(x => x.NavigationDestination == e.Uri));

            // or when using the NavigationType on menu item
            // this.HamburgerMenuControl.SelectedItem = this.HamburgerMenuControl
            //                                              .Items
            //                                              .OfType<MenuItem>()
            //                                              .FirstOrDefault(x => x.NavigationType == e.Content?.GetType());
            // this.HamburgerMenuControl.SelectedOptionsItem = this.HamburgerMenuControl
            //                                                     .OptionsItems
            //                                                     .OfType<MenuItem>()
            //                                                     .FirstOrDefault(x => x.NavigationType == e.Content?.GetType());

            // update back button
            this.GoBackButton.SetCurrentValue(VisibilityProperty, this.navigationServiceEx.CanGoBack ? Visibility.Visible : Visibility.Collapsed);
        }

        private void GoBack_OnClick(object sender, RoutedEventArgs e)
        {
            this.navigationServiceEx.GoBack();

        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //Close overlay when main window is closed
            overlay.Close();


        }




    }
}
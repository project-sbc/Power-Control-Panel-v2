using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using Power_Control_Panel.PowerControlPanel.Classes.Navigation;
using Power_Control_Panel.PowerControlPanel.Classes.ChangeTDP;
using MenuItem = Power_Control_Panel.PowerControlPanel.Classes.ViewModels.MenuItem;

namespace Power_Control_Panel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public static class GlobalVariables
    {
        public static double PL1;
        public static double PL2;
   
    }
    public partial class MainWindow : MetroWindow
    {
        private readonly NavigationServiceEx navigationServiceEx;
        public Window overlay = new Overlay();
        public DispatchTimer inputCheck = new DispatchTimer();
        public DispatchTimer valueUpdate = new DispatchTimer();
        public MainWindow()
        {
            this.InitializeComponent();

            //Run code to set up hamburger menu
            initializeNavigationFrame();

            //Run code to set up dispatch timers, one for inputcheck (i.e. xinput or keyboard prompts) and one for updating TDP values
           
        }
        void initializeDispatchTimers()
        {
            //Set up timespan for timers
            inputCheck.Timespan.FromSeconds(2);
            valueUpdate.Timespan.FromSeconds(10);

            //Add the event handlers to the timers
            inputCheck += inputCheck_Tick;
            valueUpdate += valueUpdate_Tick;

            //Start timers
            inputCheck.Start();
            valueUpdate.Start();
        }

        void inputCheck_Tick(object sender, EventArgs e)
        {

        }

        void valueUpdate_Tick(object sender, EventArgs e)
        {
            //Check settings for enabled values like TDP, CPU, etc.
            if (Properties.Settings.Default.enabledTDP)
            {

            }
        }

        void initializeNavigationFrame()
        {
            this.navigationServiceEx = new NavigationServiceEx();
            this.navigationServiceEx.Navigated += this.NavigationServiceEx_OnNavigated;
            this.HamburgerMenuControl.Content = this.navigationServiceEx.Frame;



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
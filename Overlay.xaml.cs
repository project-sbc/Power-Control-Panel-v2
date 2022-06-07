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
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using Power_Control_Panel.PowerControlPanel.Classes.ChangeTDP;

namespace Power_Control_Panel
{
    /// <summary>
    /// Interaction logic for Overlay.xaml
    /// </summary>
    public partial class Overlay : MetroWindow
    {
        WindowSinker sinker;
        public Overlay()
        {
            sinker = new WindowSinker(this);
            sinker.Sink();
            InitializeComponent();
            this.ShowInTaskbar = false;
            this.Topmost = true;
            this.WindowStyle = WindowStyle.None;
            this.AllowsTransparency = true;
        }


        private void HideOverlay_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Left = System.Windows.SystemParameters.PrimaryScreenWidth - 380;
            this.Top = 0;
        }

        private bool dragStartedTDP1 = true;
        private bool dragStartedTDP2 = true;
        private bool changingTDP = false;

        private void TDP1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!dragStartedTDP1 && !changingTDP)
            {
               
                HandleChangingTDP((int)TDP1.Value, (int)TDP2.Value, true);

            }

        }

        private async void updateTDP()
        {
            Task<string> taskTDP = ChangeTDP.readTDP();
            string tdp = await taskTDP;
            if (tdp != null)
            {
                changingTDP = true;
                TDP1.Value = Math.Round(Convert.ToDouble(tdp.Substring(0, tdp.IndexOf(";"))), 0, MidpointRounding.AwayFromZero);

                TDP2.Value = Math.Round(Convert.ToDouble(tdp.Substring(tdp.IndexOf(";") + 1, tdp.Length - tdp.IndexOf(";") - 1)), 0, MidpointRounding.AwayFromZero);
                changingTDP = false;
            }
        }

        private void TDP1_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            dragStartedTDP1 = false;
          
            HandleChangingTDP((int)TDP1.Value, (int)TDP2.Value, true);
        }

        private void TDP1_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            dragStartedTDP1 = true;
          
        }

        private void TDP1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            dragStartedTDP1 = false;
         
            HandleChangingTDP((int)TDP1.Value, (int)TDP2.Value, true);
        }
        private void TDP1_TouchUp(object sender, TouchEventArgs e)
        {
            dragStartedTDP1 = false;
            HandleChangingTDP((int)TDP1.Value, (int)TDP2.Value, true);
        }
        private void TDP1_TouchDown(object sender, TouchEventArgs e)
        {
            dragStartedTDP1 = true;
        }

        private void TDP2_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!dragStartedTDP2 && !changingTDP)
            {
                HandleChangingTDP((int)TDP1.Value, (int)TDP2.Value, false);

            }
        }
        private void TDP2_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            dragStartedTDP2 = false;
            HandleChangingTDP((int)TDP1.Value, (int)TDP2.Value, false);
        }

        private void TDP2_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            dragStartedTDP2 = true;
        }


        private void TDP2_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            dragStartedTDP2 = false;
            HandleChangingTDP((int)TDP1.Value, (int)TDP2.Value, false);
        }
        private void HandleChangingTDP(int tdpPL1, int tdpPL2, bool PL1started)
        {

            if (!changingTDP)
            {
                changingTDP = true;
                if (PL1started)
                {
                    //If PL1 is greater than PL2 then PL2 needs to be set to the PL1 value

                    if (tdpPL1 < tdpPL2) { ChangeTDP.changeTDP(tdpPL1, tdpPL2); }
                    else
                    {
                        TDP2.Value = tdpPL1;
                        ChangeTDP.changeTDP(tdpPL1, tdpPL1);


                    };

                }
                else
                {
                    //If PL2 is less than PL1 drop PL1 down to PL2 new value
                    if (tdpPL1 < tdpPL2) { ChangeTDP.changeTDP(tdpPL1, tdpPL2); }
                    else
                    {
                        TDP1.Value = tdpPL2;
                        ChangeTDP.changeTDP(tdpPL2, tdpPL2);

                    };
                }

                changingTDP = false;
            }


        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            


        }
    }
}

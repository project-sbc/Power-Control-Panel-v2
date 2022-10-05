using System;
using System.Collections.Generic;
using System.Linq;
using LiveCharts;
using LiveCharts.Defaults;
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
using ControlzEx.Theming;

namespace Power_Control_Panel.PowerControlPanel.Pages
{
    /// <summary>
    /// Interaction logic for FanCurvePage.xaml
    /// </summary>
    public partial class FanCurvePage : Page
    {
        private ChartValues<ObservablePoint> CPUTempFanPercentagePoints;

        public FanCurvePage()
        {
            InitializeComponent();
            //apply theme
            ThemeManager.Current.ChangeTheme(this, Properties.Settings.Default.systemTheme);

            CPUTempFanPercentagePoints = new();
            CPUTempFanPercentagePoints.Add(new ObservablePoint() { X = 0, Y = 50 });
            CPUTempFanPercentagePoints.Add(new ObservablePoint() { X = 40, Y = 50 });
            CPUTempFanPercentagePoints.Add(new ObservablePoint() { X = 70, Y = 80 });
            CPUTempFanPercentagePoints.Add(new ObservablePoint() { X = 100, Y = 80 });

            lvLineSeriesValues.Values = CPUTempFanPercentagePoints;
        }
    }
}

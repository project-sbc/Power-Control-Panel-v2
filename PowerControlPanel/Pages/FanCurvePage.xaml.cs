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

            // Todo, load from settings file here
            CPUTempFanPercentagePoints.Add(new ObservablePoint() { X = 0, Y = 50 });
            CPUTempFanPercentagePoints.Add(new ObservablePoint() { X = 40, Y = 50 });
            CPUTempFanPercentagePoints.Add(new ObservablePoint() { X = 70, Y = 80 });
            CPUTempFanPercentagePoints.Add(new ObservablePoint() { X = 100, Y = 80 });

            lvLineSeriesValues.Values = CPUTempFanPercentagePoints;

            NumericUpDown_FanControl0X.Value = CPUTempFanPercentagePoints[0].X;
            NumericUpDown_FanControl1X.Value = CPUTempFanPercentagePoints[1].X;
            NumericUpDown_FanControl2X.Value = CPUTempFanPercentagePoints[2].X;
            NumericUpDown_FanControl3X.Value = CPUTempFanPercentagePoints[3].X;

            NumericUpDown_FanControl0Y.Value = CPUTempFanPercentagePoints[0].Y;
            NumericUpDown_FanControl1Y.Value = CPUTempFanPercentagePoints[1].Y;
            NumericUpDown_FanControl2Y.Value = CPUTempFanPercentagePoints[2].Y;
            NumericUpDown_FanControl3Y.Value = CPUTempFanPercentagePoints[3].Y;
        }

        private void NumericUpDown_FanControl0X_ValueChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            double Value = (double)NumericUpDown_FanControl0X.Value;
            if (double.IsNaN(Value))
                return;

            lvLineSeriesValues.Values = UpdatePoints(Value, 0, true);
        }
        private void NumericUpDown_FanControl1X_ValueChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            double Value = (double)NumericUpDown_FanControl1X.Value;
            if (double.IsNaN(Value))
                return;

            NumericUpDown_FanControl2X.Minimum = Value + 1;

            lvLineSeriesValues.Values = UpdatePoints(Value, 1, true);
        }
        private void NumericUpDown_FanControl2X_ValueChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            double Value = (double)NumericUpDown_FanControl2X.Value;
            if (double.IsNaN(Value))
                return;

            NumericUpDown_FanControl1X.Maximum = Value - 1;

            lvLineSeriesValues.Values = UpdatePoints(Value, 2, true);
        }
        private void NumericUpDown_FanControl3X_ValueChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            double Value = (double)NumericUpDown_FanControl3X.Value;
            if (double.IsNaN(Value))
                return;

            lvLineSeriesValues.Values = UpdatePoints(Value, 3, true);
        }
        private void NumericUpDown_FanControl0Y_ValueChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            double Value = (double)NumericUpDown_FanControl0Y.Value;
            if (double.IsNaN(Value))
                return;

            lvLineSeriesValues.Values = UpdatePoints(Value, 0, false);
        }
        private void NumericUpDown_FanControl1Y_ValueChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            double Value = (double)NumericUpDown_FanControl1Y.Value;
            if (double.IsNaN(Value))
                return;

            lvLineSeriesValues.Values = UpdatePoints(Value, 1, false);
        }
        private void NumericUpDown_FanControl2Y_ValueChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            double Value = (double)NumericUpDown_FanControl2Y.Value;
            if (double.IsNaN(Value))
                return;

            lvLineSeriesValues.Values = UpdatePoints(Value, 2, false);
        }
        private void NumericUpDown_FanControl3Y_ValueChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            double Value = (double)NumericUpDown_FanControl3Y.Value;
            if (double.IsNaN(Value))
                return;

            lvLineSeriesValues.Values = UpdatePoints(Value, 3, false);
        }

        private ChartValues<ObservablePoint> UpdatePoints(double Value, int index, bool x_or_y)
        {
            if ((Value < 0) || (Value > 100)) { return CPUTempFanPercentagePoints; }
            if ((index < 0) || (index > 4)) { return CPUTempFanPercentagePoints; }
            if (CPUTempFanPercentagePoints == null) { return CPUTempFanPercentagePoints; }


            // Todo, add bounds check, add min max check

            if (x_or_y) { 
                CPUTempFanPercentagePoints[index].X = Value;
            }
            else { 
                CPUTempFanPercentagePoints[index].Y = Value;
            }

            return CPUTempFanPercentagePoints;
        }

        // Fanspeed % based on temperature deg Celsisus
        // Interpolation function (linear), takes list of
        // coordinates and temperature returns fan speed

        // @Chris, this needs to be moved some place else where you have access to live temperature, fan setpoint and configuration
        private void DetermineFanspeedBasedOnTemperature(float TemperatureDegCelsius, float[,] Nodes)
        {
            int NodeAmount = 4;
            float FanSpeed = 100.0f; // Use safe default value, not zero!

            // Convert xy list to separate single lists
            float[] X = new float[NodeAmount];
            float[] Y = new float[NodeAmount];

            for (int node_index = 0; node_index < NodeAmount; node_index++)
            {
                X[node_index] = Nodes[node_index, 0];
                Y[node_index] = Nodes[node_index, 1];
            }

            // Figure out between which two nodes the temperature is
            int i = Array.FindIndex(X, k => TemperatureDegCelsius <= k);

            // Interpolate between those two points
            FanSpeed = Y[i - 1] + (TemperatureDegCelsius - X[i - 1]) * (Y[i] - Y[i - 1]) / (X[i] - X[i - 1]);

            // Graph for testing
            // Update
            //lvLineSeriesActual.Values = new ChartValues<double>() { TemperatureDegCelsius, FanSpeed };

            //CPUTempFanPercentagePoints = new();
            //CPUTempFanPercentagePoints.Add(new ObservablePoint() { X = TemperatureDegCelsius, Y = FanSpeed });

            //lvLineSeriesActual.Values = CPUTempFanPercentagePoints;
        }
    }
}

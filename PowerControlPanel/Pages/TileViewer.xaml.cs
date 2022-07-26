using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Power_Control_Panel.PowerControlPanel.Pages
{
    /// <summary>
    /// Interaction logic for TileViewer.xaml
    /// </summary>
    public partial class TileViewer : UserControl
    {
        public TileViewer()
        {
            InitializeComponent();


            wrapPanel.Children.Add(tileMaker("C:\\Program Files (x86)\\Steam\\steam.exe"));

        }

        private MahApps.Metro.Controls.Tile tileMaker(string exe)
        {
            MahApps.Metro.Controls.Tile tile = new MahApps.Metro.Controls.Tile();
            tile.CommandParameter = exe;

            System.Windows.Controls.Image image = new System.Windows.Controls.Image();
            using (Icon ico = System.Drawing.Icon.ExtractAssociatedIcon(exe)) 
            {
                image.Source = Imaging.CreateBitmapSourceFromHIcon(ico.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }

            tile.Content = image;

            return tile;
        }
    }
}

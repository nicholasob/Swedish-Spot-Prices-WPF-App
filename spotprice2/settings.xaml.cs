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

namespace spotprice2
{
    /// <summary>
    /// Interaction logic for settings.xaml
    /// </summary>
    public partial class settings : Window
    {
        public bool boolReturnValue { get; set; } = false;
        public settings()
        {
            InitializeComponent();
            powerAreaComboBox.Text = userSettings.Default.PowerArea;
            var color = userSettings.Default.Color;
            ColorPicker.SelectedColor = System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B); ;
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            userSettings.Default.PowerArea = powerAreaComboBox.Text;
            System.Windows.Media.Color? color = ColorPicker.SelectedColor;
            if (color.HasValue)
            {
                userSettings.Default.Color = System.Drawing.Color.FromArgb(color.Value.A, color.Value.R, color.Value.G, color.Value.B);
            }
            boolReturnValue = true;
            userSettings.Default.Save();
            this.Close();
        }
    }
}

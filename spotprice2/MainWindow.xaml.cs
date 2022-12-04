using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
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


namespace spotprice2
{
    public enum PowerArea
    {
        SN1,
        SN2,
        SN3,
        SN4
    }




    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        protected API _api = new(stringToEnum<PowerArea>(userSettings.Default.PowerArea));
        private DateTime? currentTime = DateTime.Now;
        private System.Windows.Threading.DispatcherTimer spotTimer;
        private int timeInMiliSeconds = 500;

        /*
           If passed string is invalid, the default enum value will be returned.  
         */
        public static T stringToEnum<T>(string str) where T : struct, Enum
        {
            T returnValue;
            Enum.TryParse(str, out returnValue);
            return returnValue;
        }

        public MainWindow()
        {
            InitializeComponent();

            this.Topmost = true;

            setColors();
            _api.DataChanged += update;

            spotTimer = new System.Windows.Threading.DispatcherTimer();

            spotTimer.Tick += spotTimer_Tick;
            spotTimer.Interval = TimeSpan.FromMilliseconds(timeInMiliSeconds);
            spotTimer.Start();
        }

        private void spotTimer_Tick(object? sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            Time.Content = now.ToString("HH:mm");
            if ((now - currentTime)?.Hours >= 1)
            {
                update();
                currentTime = now;
            }
        }
        private void updateLabel()
        {
            /*
            if (spotLabel.InvokeRequired)
            {
                spotLabel.Invoke(new Action(updateLabel));
                return;
            }
            */
            if (_api.data == null) return;
            var spotPrice = _api.data.First(item => item.TimeStampDay == DateTime.Now.ToString("yyyy-MM-dd") && item.TimeStampHour == DateTime.Now.ToString("HH:00")).Value;
            spotLabel.Content = spotPrice.ToString();
        }

        private void updateTomorrowLabel()
        {
            /*
            if (label1.InvokeRequired)
            {
                label1.Invoke(new Action(updateTomorrowLabel));
                return;
            }
            */

            if (_api.data == null) return;
            var spotPrice = _api.data[_api.data.FindIndex(item => item.TimeStampDay == DateTime.Now.ToString("yyyy-MM-dd") && item.TimeStampHour == DateTime.Now.ToString("HH:00")) + 1].Value;
            nextHourLabel.Content = spotPrice.ToString();
        }

        private void update()
        {
            powerAreaLabel.Content = nextHourLabel.Content = spotLabel.Content = "-";
            powerAreaLabel.Content = userSettings.Default.PowerArea;
            updateLabel();
            updateTomorrowLabel();

        }

        private void setColors()
        {
            var settingsColor = userSettings.Default.Color;
            System.Windows.Media.Color color = System.Windows.Media.Color.FromArgb(settingsColor.A, settingsColor.R, settingsColor.G, settingsColor.B);
            Brush colorBrush = new SolidColorBrush(color);

            Debug.WriteLine(color);

            powerAreaLabel.Foreground   = colorBrush;
            nextHourLabel.Foreground    = colorBrush;
            spotLabel.Foreground        = colorBrush;
            Time.Foreground             = colorBrush;
            


        }

        private void UpdatedConfiguration()
        {
            setColors();
            _api.Area = stringToEnum<PowerArea>(userSettings.Default.PowerArea);
        }

        private void settingsButton_Click(object sender, RoutedEventArgs e)
        {
            settings settingsWindow = new settings();

            var result = settingsWindow.ShowDialog();
            bool val = settingsWindow.boolReturnValue;
            if (val == true)
            {
                UpdatedConfiguration();
                
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
    }
}

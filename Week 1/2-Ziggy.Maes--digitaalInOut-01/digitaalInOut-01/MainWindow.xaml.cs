using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using USB_DAQBoard;

namespace digitaalInOut-01
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void UpdateUI()
        {
            int value = MPUSB.ReadDigitalOutPortD();
            string binary = Convert.ToString(value, 2).PadLeft(8, '0');
            int l = binary.Length;

            for (int i = 0; i < l; i++)
            {
                CheckBox chk = this.stpChk.Children[i] as CheckBox;
                chk.Tag = i;
                var b = binary.Substring(7 - i, 1);

                chk.Checked -= chkChanged;
                chk.Unchecked -= chkChanged;

                if (b == "1")
                    chk.IsChecked = true;
                else
                    chk.IsChecked = false;

                chk.Checked += chkChanged;
                chk.Unchecked += chkChanged;
            }
        }

        private void btnOpen(object sender, RoutedEventArgs e)
        {
            MPUSB.OpenMPUSBDevice();

            DispatcherTimer UITimer = new DispatcherTimer();
            UITimer.Interval = TimeSpan.FromMilliseconds(1);
            UITimer.Tick += UITimer_Tick;
            UITimer.IsEnabled = true;

            DispatcherTimer ButtonTimer = new DispatcherTimer();
            ButtonTimer.Interval = TimeSpan.FromMilliseconds(1);
            ButtonTimer.Tick += ButtonTimer_Tick;
            ButtonTimer.IsEnabled = true;
        }

        void ButtonTimer_Tick(object sender, EventArgs e)
        {
            var b = MPUSB.ReadDigitalInPortB();
        }

        void UITimer_Tick(object sender, EventArgs e)
        {
            UpdateUI();
        }

        private void btnClose(object sender, RoutedEventArgs e)
        {
            MPUSB.CloseMPUSBDevice();
        }

        private void btnVersie(object sender, RoutedEventArgs e)
        {
            txtVersie.Text = MPUSB.GetVersion();
        }

        private void btnLedsAan(object sender, RoutedEventArgs e)
        {
            MPUSB.WriteDigitalOutPortD(255);
            UpdateUI();
        }

        private void btnLedsUit(object sender, RoutedEventArgs e)
        {
            MPUSB.WriteDigitalOutPortD(0);
            UpdateUI();
        }

        private void btnOmhoog(object sender, RoutedEventArgs e)
        {

        }

        private void btnOmlaag(object sender, RoutedEventArgs e)
        {

        }

        private void btnTeller(object sender, RoutedEventArgs e)
        {
            for (short i = 0; i <= 255; i++)
            {
                MPUSB.WriteDigitalOutPortD(i);
            }
        }

        private void btn7_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn6_Click(object sender, RoutedEventArgs e)
        {

        }

        private void chkChanged(object sender, RoutedEventArgs e)
        {
            CheckBox chk = sender as CheckBox;
            int current = MPUSB.ReadDigitalOutPortD();
            string currentString = Convert.ToString(current, 2).PadLeft(8, '0');
            int change = (int)Math.Pow(2, (int)chk.Tag);
            string changeString = Convert.ToString(change, 2).PadLeft(8, '0');

            int newValue = current ^ change;
            MPUSB.WriteDigitalOutPortD((short)newValue);
        }
    }
}

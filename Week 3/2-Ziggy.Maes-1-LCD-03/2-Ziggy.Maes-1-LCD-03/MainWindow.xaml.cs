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
using USB_DAQBoard;

namespace _2_Ziggy.Maes_1_LCD_03
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        short displaySettings = 15; //alles aan

        public MainWindow()
        {
            InitializeComponent();
        }
        private void Wait()
        {
            MPUSB.Wait(1);
        }

        private void EHoogData()
        {
            MPUSB.WriteDigitalOutPortD(1280);
        }

        private void ELaagData()
        {
            MPUSB.WriteDigitalOutPortD(1024);
        }

        private void EHoogInstructie()
        {
            var x = MPUSB.ReadDigitalOutPortD();
            var y = x & 0xF8FF;
            var z = y | 0x0100;
            MPUSB.WriteDigitalOutPortD((short)z);
        }

        private void ELaagInstructie()
        {
            var x = MPUSB.ReadDigitalOutPortD();
            var y = x & 0xF8FF;
            var z = y | 0x0000;
            MPUSB.WriteDigitalOutPortD((short)z);
        }

        private void WriteLCDBus(char input)
        {
            EHoogData();

            int v = (int)Convert.ToChar(input);
            int letter = v ^ 1280;
            MPUSB.WriteDigitalOutPortD((short)letter);

            Wait();
            ELaagData();
            Wait();
        }

        private void functionSet()
        {
            EHoogInstructie();
            Wait();
            MPUSB.WriteDigitalOutPortD(312);//00100000
            Wait();
            ELaagInstructie();

            Wait();

            EHoogInstructie();
            Wait();
            MPUSB.WriteDigitalOutPortD(271);
            Wait();
            ELaagInstructie();

            Wait();

            EHoogInstructie();
            Wait();
            MPUSB.WriteDigitalOutPortD(257);
            Wait();
            ELaagInstructie();
        }

        private void btnInit_Click(object sender, RoutedEventArgs e)
        {
            MPUSB.OpenMPUSBDevice();

            functionSet();
        }

        private void clearLCD()
        {
            EHoogInstructie();
            Wait();
            MPUSB.WriteDigitalOutPortD(257);
            ELaagInstructie();
            Wait();
        }
        private void nextLine()
        {
            EHoogInstructie();
            Wait();
            var mw = 192 ^ 256;
            MPUSB.WriteDigitalOutPortD((short)mw);
            ELaagInstructie();
            Wait();
        }

        private void displayState(bool state)
        {
            EHoogInstructie();
            Wait();

            var x = MPUSB.ReadDigitalOutPortD();
            var y = 0;
            if (state) y = 15; else y = 8;
            var z = x | y;

            MPUSB.WriteDigitalOutPortD((short)z);
            ELaagInstructie();
            Wait(); 
        }

        private void btnWrite_Click(object sender, RoutedEventArgs e)
        {
            clearLCD();
            int i = 0;
            foreach( char c in txtInput.Text)
            {
                i++;
                if (i == 17) nextLine();          
                WriteLCDBus(c);
            }
        }

        private void chkDisplay_Unchecked(object sender, RoutedEventArgs e)
        {
            displayState(false);
        }
        private void chkDisplay_Checked(object sender, RoutedEventArgs e)
        {
            displayState(true);
        }
    }
}

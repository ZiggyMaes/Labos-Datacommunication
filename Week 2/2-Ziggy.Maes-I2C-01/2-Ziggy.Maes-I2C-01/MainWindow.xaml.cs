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

namespace _2_Ziggy.Maes_I2C_01
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

        private void startCondition()
        {
            MPUSB.WriteDigitalOutPortD(3);
            MPUSB.Wait(1);
            MPUSB.WriteDigitalOutPortD(2);
            MPUSB.Wait(1);
            MPUSB.WriteDigitalOutPortD(0);
            MPUSB.Wait(1);
        }

        private void stopCondition()
        {
            MPUSB.WriteDigitalOutPortD(0);
            MPUSB.Wait(1);
            MPUSB.WriteDigitalOutPortD(2);
            MPUSB.Wait(1);
            MPUSB.WriteDigitalOutPortD(3);
            MPUSB.Wait(1);
        }

        private void zendNul()
        {
            MPUSB.WriteDigitalOutPortD(0);
            MPUSB.WriteDigitalOutPortD(2);
            MPUSB.WriteDigitalOutPortD(0);
        }

        private void zendEen()
        {
            MPUSB.WriteDigitalOutPortD(1);
            MPUSB.WriteDigitalOutPortD(3);
            MPUSB.WriteDigitalOutPortD(0);
        }

        private void btnInit_Click(object sender, RoutedEventArgs e)
        {
            MPUSB.OpenMPUSBDevice();
            startCondition();
        }

        private void testDisplay()
        {
            startCondition();

            IC(); // aanspreken van de IC

            //Waarde 8 doorsturen
            zendEen();//ACK
            zendNul();
            zendNul();
            zendNul();
            zendNul();
            zendNul();
            zendNul();
            zendNul();
            zendEen();
            zendEen();//ACK

            stopCondition();
        }

        private void btnZend_Click(object sender, RoutedEventArgs e)
        {
            int datawaarde = Convert.ToInt16(txtData.Text);

            switch (datawaarde)
            {
                case 0:
                    zendGetal(33);
                    break;
                case 1:
                    zendGetal(189);
                    break;
                case 2:
                    zendGetal(19);
                    break;
                case 3:
                    zendGetal(25);
                    break;
                case 4:
                    zendGetal(141);
                    break;
                case 5:
                    zendGetal(73);
                    break;
                case 6:
                    zendGetal(65);
                    break;
                case 7:
                    zendGetal(61);
                    break;
                case 8:
                    zendGetal(1);
                    break;
                case 9:
                    zendGetal(9);
                    break;
                default:
                    zendGetal(66);
                    break;
            }

        }

        private void zendGetal(int getal)
        {
            string waarde = Convert.ToString(getal, 2).PadLeft(8, '0');
            
            startCondition();
            IC();
            zendEen();

            foreach(char c in waarde)
            {
                if (c == '0') zendNul();
                else if (c == '1') zendEen();
            }

            zendEen();
            getAck();
            stopCondition();
        }

    
        private void IC()
        {
            //Aanspreken van de IC
            zendNul();
            zendEen();
            zendNul();
            zendNul();
            zendNul();
            zendNul();
            zendNul();
            zendNul();
        }

        private void getAck()
        {
            MPUSB.WriteDigitalOutPortD(1);
            MPUSB.Wait(1);
            MPUSB.WriteDigitalOutPortD(3);
            MPUSB.Wait(1);
            short ack = MPUSB.ReadDigitalInPortB();
            string waarde = Convert.ToString(ack, 2).PadLeft(8, '0');

            MPUSB.WriteDigitalOutPortD(0);
            MPUSB.Wait(1);
        }
    }
}

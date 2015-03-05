using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace _02_Ziggy.Maes_Analoog
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    ///
    public partial class MainWindow : Window
    {
        BackgroundWorker bw = new BackgroundWorker();

        public MainWindow()
        {
            InitializeComponent();
            MPUSB.OpenMPUSBDevice();
            bw.WorkerSupportsCancellation = true;
            bw.WorkerReportsProgress = true;

            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.ProgressChanged +=  new ProgressChangedEventHandler(bw_ProgressChanged);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);

            bw.RunWorkerAsync();
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            System.Threading.Thread.Sleep(50);
            worker.ReportProgress(100);
        }

        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var valbar1 = MPUSB.ReadAnalogIn(0);
            //Console.WriteLine(valbar1);
            bar1.Value = valbar1;

            var valbar2 = MPUSB.ReadAnalogIn(1);
            Console.WriteLine(valbar2);
            bar2.Value = valbar2;

            var valbar3 = MPUSB.ReadAnalogIn(2);
            Console.WriteLine(valbar3);
            bar3.Value = valbar3;

            var valbar4 = MPUSB.ReadAnalogIn(3);
            Console.WriteLine(valbar4);
            bar4.Value = valbar4;

        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            bw.RunWorkerAsync();
        }

        private void slider1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MPUSB.WriteAnalogOut(0, (short)slider1.Value);
        }

        private void slider2_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MPUSB.WriteAnalogOut(1, (short)slider2.Value);
        }

    }
}

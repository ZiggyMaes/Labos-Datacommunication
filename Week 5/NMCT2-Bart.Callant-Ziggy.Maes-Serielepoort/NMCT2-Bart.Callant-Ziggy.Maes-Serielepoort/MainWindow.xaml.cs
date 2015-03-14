using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
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
using System.Windows.Threading;
using TechUSBLCDSimulator.Lib;

namespace NMCT2_Bart.Callant_Ziggy.Maes_Serielepoort
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private BackgroundWorker bw = new BackgroundWorker();

		public MainWindow()
		{
			InitializeComponent();

			var coms = SerialPort.GetPortNames();
			cboCOM.ItemsSource = coms;

			MPUSB.OpenMPUSBDevice();
			LCD.FunctionSet();

			bw.RunWorkerCompleted += bw_RunWorkerCompleted;
		}

		int oldDigital, oldAnalog1, oldAnalog2, oldAnalog3, oldAnalog4;
		void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			int newDigital = MPUSB.ReadDigitalOutPortD();
			int newAnalog1 = MPUSB.ReadAnalogIn(0);
			int newAnalog2 = MPUSB.ReadAnalogIn(1);
			int newAnalog3 = MPUSB.ReadAnalogIn(2);
			int newAnalog4 = MPUSB.ReadAnalogIn(3);

			if(oldDigital != newDigital)
			{
				sp.WriteLine("Digital In changed!");
				oldDigital = newDigital;
			}
			if(oldAnalog1 != newAnalog1)
			{
				sp.WriteLine("Analog In 1 changed!");
				oldAnalog1 = newAnalog1;
			}
			if(oldAnalog2 != newAnalog2)
			{
				sp.WriteLine("Analog In 2 changed!");
				oldAnalog2 = newAnalog2;
			}
			if (oldAnalog3 != newAnalog3)
			{
				sp.WriteLine("Analog In 3 changed!");
				oldAnalog3 = newAnalog3;
			}
			if (oldAnalog4 != newAnalog4)
			{
				sp.WriteLine("Analog In 4 changed!");
				oldAnalog4 = newAnalog4;
			}

			bw.RunWorkerAsync();
		}

		SerialPort sp;
		DispatcherTimer tReadExisting = new DispatcherTimer();
		DispatcherTimer tRead = new DispatcherTimer();

		private void cboCOM_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			sp = new SerialPort((string)cboCOM.SelectedItem, 9600, Parity.None, 8, StopBits.One);
			sp.Open();
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			sp.Close();
		}

		private void ReadExisting_Checked(object sender, RoutedEventArgs e)
		{
			tReadExisting.Interval = new TimeSpan(500);
			tReadExisting.Tick += tReadExisting_Tick;
			tReadExisting.Start();
		}

		void tReadExisting_Tick(object sender, EventArgs e)
		{
			var message = sp.ReadExisting();
			txtReadExisting.Text += message;
		}

		private void Read_Checked(object sender, RoutedEventArgs e)
		{
			tRead.Interval = new TimeSpan(500);
			tRead.Tick += tRead_Tick;
			tRead.Start();
		}

		string readInput = "";
		void tRead_Tick(object sender, EventArgs e)
		{
			char[] data = new char[sp.BytesToRead];
			string text = "";
			sp.Read(data, 0, data.Length);

			if (data.Length != 0)
					text = new string(data);

			if (text != "")
				readInput += text;

			txtRead.Text = readInput;
		}

		private void DataRecieved_Checked(object sender, RoutedEventArgs e)
		{
			sp.DataReceived += new SerialDataReceivedEventHandler(sp_DataReceived);
			bw.RunWorkerAsync();
		}

		private delegate void UpdateUiTextDelegate(string text);
		bool IsCommandStarted = false;
		string command = "";
		string input = "";
		private void DisplayText(string dataString)
		{
			char commandChar = '%';

			char inp = Convert.ToChar(dataString);

			if(inp == commandChar)
			{
				if(IsCommandStarted)
				{
					IsCommandStarted = false;

					sp.WriteLine("Command received: '" + command + "' ! ");

					ExecuteCommand(command);

					command = "";
				}
				else
				{
					IsCommandStarted = true;
				}
			}
			else if(IsCommandStarted)
			{
				if (inp == 127)
				{
					if (command.Length > 0) command = command.Substring(0, command.Length - 1);
				}
				else
				{
					command += dataString;
				}
			}


			if (inp == 127)
			{
				if (input.Length > 0) input = input.Substring(0, input.Length - 1);
			}
			else
			{
				input += dataString;
			}

			txtDataRecieved.Text = input;
		}

		void sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
		{
			SerialPort s = sender as SerialPort;
			if(s != null)
			{
				string text = s.ReadExisting();
				//txtDataRecieved.Text = text;
				Dispatcher.Invoke(DispatcherPriority.Send, new UpdateUiTextDelegate(DisplayText), text);
			}
		}

		private void ExecuteCommand(string command)
		{
			if(command.Contains(':'))
			{
				string com = command.Split(':')[0];
				string value = command.Split(':')[1];

				switch (com)
				{
					case "led":
						PowerLed(Convert.ToInt32(value));
						break;
					case "leds":
						PowerLeds(Convert.ToInt32(value));
						break;
					case "analog0":
						MPUSB.WriteAnalogOut(0, (short)Convert.ToInt32(value));
						break;
					case "analog1":
						MPUSB.WriteAnalogOut(1, (short)Convert.ToInt32(value));
						break;
					case "lcd":
						switch (value)
						{
							case "displayoff":
								LCD.DisplayOff();
								break;
							case "displayon":
								LCD.DisplayOn();
								break;
							case "cursorblink":
								LCD.CursorBlink();
								break;
							case "cursornoblink":
								LCD.CursorNoBlink();
								break;
							case "cursorvisible":
								LCD.CursorVisible();
								break;
							case "cursorinvisible":
								LCD.CursorInvisible();
								break;
							default:
								LCD.WriteText(value);
								break;
						}
						break;
					default:
						sp.WriteLine("Command not recognized!");
						break;
				}
			}
		}

		private void PowerLed(int ledNumber)
		{
			short currentValue = MPUSB.ReadDigitalOutPortD();
			int newValue = 0; ;

			switch (ledNumber)
			{
				case 1:
					newValue = currentValue | 1;
					break;
				case 2:
					newValue = currentValue | 2;
					break;
				case 3:
					newValue = currentValue | 4;
					break;
				case 4:
					newValue = currentValue | 8;
					break;
				case 5:
					newValue = currentValue | 16;
					break;
				case 6:
					newValue = currentValue | 32;
					break;
				case 7:
					newValue = currentValue | 64;
					break;
				case 8:
					newValue = currentValue | 128;
					break;
				default:
					sp.WriteLine("Invalid Led Number! (1-8)");
					break;
			}

			if (newValue != 0)
				MPUSB.WriteDigitalOutPortD((short)newValue);
		}
		private void PowerLeds(int value)
		{
			MPUSB.WriteDigitalOutPortD((short)value);
		}
	}
}

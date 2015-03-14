using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechUSBLCDSimulator.Lib;

namespace NMCT2_Bart.Callant_Ziggy.Maes_Serielepoort
{
	public class LCD
	{
		private static void Wait()
		{
			MPUSB.Wait(50);
		}

		private static void EHoogData()
		{
			MPUSB.WriteDigitalOutPortD(1280);
		}
		private static void ELaagData()
		{
			MPUSB.WriteDigitalOutPortD(1024);
		}
		private static void EHoogInstructie()
		{
			var x = MPUSB.ReadDigitalOutPortD();
			var y = x & 0xF8FF;
			var z = y | 0x0100;
			MPUSB.WriteDigitalOutPortD((short)z);
		}
		private static void ELaagInstructie()
		{
			var x = MPUSB.ReadDigitalOutPortD();
			var y = x & 0xF8FF;
			var z = y | 0x0000;
			MPUSB.WriteDigitalOutPortD((short)z);
		}

		private static void WriteLCDBus(char input)
		{
			EHoogData();

			int v = (int)Convert.ToChar(input);
			int letter = v ^ 1280;
			MPUSB.WriteDigitalOutPortD((short)letter);

			Wait();

			ELaagData();

			Wait();
		}

		public static void FunctionSet()
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

			ClearLCD();
		}

		private static void ClearLCD()
		{
			Wait();

			// CLEAR LCD
			EHoogInstructie();
			Wait();
			MPUSB.WriteDigitalOutPortD(257);
			Wait();
			ELaagInstructie();

			Wait();
		}

		public static void WriteText(string text)
		{
			ClearLCD();

			int i = 0;
			foreach (char c in text)
			{
				i++;

				if (i == 17)
					NewLine();

				WriteLCDBus(c);
			}
		}

		private static void NewLine()
		{
			Wait();

			EHoogInstructie();
			var nw = 192 ^ 256;
			MPUSB.WriteDigitalOutPortD((short)nw);
			ELaagInstructie();
		}

		static int displaySettings = 15;
		public static void DisplayOn()
		{
			displaySettings = displaySettings | 4;
			SendDisplaySettings();
		}
		public static void DisplayOff()
		{
			displaySettings = displaySettings & 251;
			SendDisplaySettings();
		}
		public static void CursorBlink()
		{
			displaySettings = displaySettings | 1;
			SendDisplaySettings();
		}
		public static void CursorNoBlink()
		{
			displaySettings = displaySettings & 254;
			SendDisplaySettings();
		}
		public static void CursorVisible()
		{
			displaySettings = displaySettings | 2;
			SendDisplaySettings();
		}
		public static void CursorInvisible()
		{
			displaySettings = displaySettings & 253;
			SendDisplaySettings();
		}
		private static void SendDisplaySettings()
		{
			EHoogInstructie();
			var nw = displaySettings ^ 256;
			MPUSB.WriteDigitalOutPortD((short)nw);
			ELaagInstructie();
		}
	}
}

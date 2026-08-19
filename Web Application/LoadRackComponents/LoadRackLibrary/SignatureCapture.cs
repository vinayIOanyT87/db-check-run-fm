/******************************************************************************
	FILE NAME:		SignatureCapture.cs
	PURPOSE:		SignatureCaptureClass

	COMMENTS:
		Copyright (C) Varec, Inc. Norcross, GA, USA, 2000
		This file shall not be copied or reproduced in any form without
		the express written consent of Varec.


	AUTHOR(S):	W. Gray
	VERSION:		7.4.0.0  Current version

	MODIFICATION HISTORY:
		Date:		By:					Reason:
		----------	-----------------	-------------------------------------------
		18-Apr-2008	C. Knight			7.4.0.0	- Make directory to signature device drivers
													configurable.  The setting "HandHeldUposPath" should
													be added to the appSettings section of the configuration
													file for the FuelsManager Terminal Automation service
													("loadrackservice.exe.config")

		11-Jul-08	B. Schaal			7.4.0.1 - Removed references and code to deal with HandHeld Products
													signature capture device. Added references and code to handle Topaz
													signature capture device
 
		13-Nov-09	W.Gray				7.5.1.0 Revised to expect Varec.bmp in \Program Files\FuelsManager (WI 9268)
*******************************************************************************/
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Reflection;

namespace LoadRackLibrary
{
	public interface ISigPlus
	{
		void SetTabletXStart(int a);
		void SetTabletXStop(int a);
		void SetTabletYStart(int a);
		void SetTabletYStop(int a);
		void SetTabletLogicalXSize(int a);
		void SetTabletLogicalYSize(int a);
		void SetTabletComPort(int a);
		void SetTabletBaudRate(int a);
		bool LCDRefresh(int a, int b, int c, int d, int e);
		void ClearTablet();
		void KeyPadAddHotSpot(int a, int b, int c, int d, int e, int f);
		void ClearSigWindow(int a);
		void SetTabletState(int a);
		void SetLCDCaptureMode(int a);
		void LCDSendGraphic(int a, int b, int c, int d, Bitmap bitMap);
		void LCDWriteString(int a, int b, int c, int d, Font font, string text);
		void LCDSetWindow(int a, int b, int c, int d);
		void SetSigWindow(int a, int b, int c, int d, int e);
		int KeyPadQueryHotSpot(int a);
		int NumberOfTabletPoints();
		void SetImageXSize(int a);
		void SetImageYSize(int a);
		void SetJustifyMode(int a);
		void SetImagePenWidth(int a);
		void SetImageFileFormat(int a);
		Bitmap GetSigImage();
	}

	/// <summary>
	/// Summary description for SignatureCaptureClass.
	/// </summary>
	public class SignatureCaptureClass
	{
		protected const int SignatureTimeout = 60;   // one minute timeout
		protected EventLog EventLog = null;
		protected string SignatureDevice;
		protected byte[] Signature = null;
		protected Bitmap img4x5;
		public string Error = null;

		public SignatureCaptureClass(EventLog EventLog)
		{
			this.EventLog = EventLog;
		}

		public byte[] Get(string SignatureDevice, int SerialPort, int BaudRate)
		{
			String strImg4x5;
			bool bExitLoop = false;
			bool bTabletAvailable = false;

			this.SignatureDevice = SignatureDevice;

			NameValueCollection appSettings = ConfigurationManager.AppSettings;
			string BMPDirectory = appSettings.Get("SigCaptureBMPPath");

			if (BMPDirectory == null)
			{
				Error = "SigCaptureBMPPath Configuration Parameter not found in LoadRackService.Exe.Config file. Please add and try again.";
				if (Error != null
						  && EventLog != null)
					EventLog.WriteEntry(Error, EventLogEntryType.Error);
				throw new Exception(Error);
			}

			if (BMPDirectory.IndexOf("varec.bmp", StringComparison.CurrentCultureIgnoreCase) <= 0)
			{
				if (!BMPDirectory.EndsWith("\\"))
				{
					BMPDirectory += "\\";
				}
				BMPDirectory += "varec.bmp";
			}

			//verify that the path and file exist
			if(File.Exists(BMPDirectory) == false)
			{
				Error = "Varec.Bmp File Not Located at " + BMPDirectory;
				if (Error != null
						  && EventLog != null)
					EventLog.WriteEntry(Error, EventLogEntryType.Error);
				throw new Exception(Error);
			}
			var SigPlusNET = new Topaz.SigPlusNET();

			//Sets up SigPlus for LCD 4X5 tablet
			SigPlusNET.SetTabletXStart(500);
			SigPlusNET.SetTabletXStop(2600);
			SigPlusNET.SetTabletYStart(500);
			SigPlusNET.SetTabletYStop(2100);
			SigPlusNET.SetTabletLogicalXSize(2100);
			SigPlusNET.SetTabletLogicalYSize(1600);

			// set the configured comm port and baud rate
			SigPlusNET.SetTabletComPort(SerialPort);

			SigPlusNET.SetTabletBaudRate(BaudRate);

			bTabletAvailable = SigPlusNET.LCDRefresh(0, 0, 0, 320, 240); //Refresh entire LCD
			if (!bTabletAvailable)
			{
				Error = "Error Connecting To Signature Device " + SignatureDevice;
				if (Error != null
						  && EventLog != null)
					EventLog.WriteEntry(Error, EventLogEntryType.Error);
				throw new Exception(Error);
			}

			SigPlusNET.ClearTablet(); //clears the SigPlus object

			//adds the hotspots on lcd
			SigPlusNET.KeyPadAddHotSpot(0, 1, 243, 25, 68, 22);   // done
			SigPlusNET.KeyPadAddHotSpot(1, 1, 243, 63, 68, 22);   // cancel
			SigPlusNET.KeyPadAddHotSpot(2, 1, 243, 100, 68, 22);  // clear

			// set the background bmp display
			strImg4x5 = BMPDirectory;
			img4x5 = new System.Drawing.Bitmap(strImg4x5);

			SigPlusNET.ClearSigWindow(1);
			SigPlusNET.SetTabletState(1);
			SigPlusNET.SetLCDCaptureMode(2); //Sets up LCD to retain text/graphics/ink
			SigPlusNET.LCDSendGraphic(1, 2, 0, 0, img4x5); //load bmp into background memory for display on lcd

			// Bring stored background image to foreground
			SigPlusNET.LCDRefresh(2, 0, 0, 320, 240);

			Font header;

			using (header = new Font("Arial", 28.0F, FontStyle.Bold | FontStyle.Italic))
			{
				SigPlusNET.LCDWriteString(0, 2, 47, 2, header, "Varec");
			}

			using (header = new Font("Arial", 8.0F, FontStyle.Regular | FontStyle.Italic))
			{
				SigPlusNET.LCDWriteString(0, 2, 90, 36, header, "A Leidos Company");
			}

			using (header = new Font("Arial Black", 18.0F, FontStyle.Bold))
			{
				SigPlusNET.LCDWriteString(0, 2, 10, 60, header, "FuelsManager");
			}

			using (header = new Font("Arial", 8.0F, FontStyle.Regular))
			{
				SigPlusNET.LCDWriteString(0, 2, 160, 95, header, "Oil & Gas");
			}

			using (header = new Font("Arial", 12.0F, FontStyle.Regular))
			{
				SigPlusNET.LCDWriteString(0, 2, 40, 110, header, "Terminal Automation");
			}

			SigPlusNET.LCDSetWindow(3, 150, 313, 65); //Permits only the section on lcd to ink
			SigPlusNET.SetSigWindow(1, 3, 150, 313, 65);//12, 176, 318, 55); //permits ink only in the section specified in sigplus object
			SigPlusNET.SetTabletState(1);
			SigPlusNET.SetLCDCaptureMode(2);

			int CaptureCount = 0;

			while (bExitLoop == false)
			{
				Thread.Sleep(1000); // check for a change every one second

				if (SigPlusNET.KeyPadQueryHotSpot(1) > 0)//cancel
				{
					SigPlusNET.ClearSigWindow(1);
					SigPlusNET.ClearTablet();
					SigPlusNET.LCDRefresh(1, 246, 63, 68, 21); //invert px at cancel so user knows its been tapped
					Thread.Sleep(600);
					SigPlusNET.LCDRefresh(2, 240, 63, 70, 23); //refresh lcd with background ONLY at bottom of LCD
					SigPlusNET.ClearSigWindow(1);
					bExitLoop = true;
				}
				else if (SigPlusNET.KeyPadQueryHotSpot(2) > 0) //clear
				{
					SigPlusNET.ClearSigWindow(1);
					SigPlusNET.ClearTablet();
					SigPlusNET.LCDRefresh(1, 246, 100, 68, 21); //invert px at clear so user knows its been tapped
					Thread.Sleep(600);
					SigPlusNET.LCDRefresh(2, 240, 100, 70, 23);
					SigPlusNET.LCDRefresh(2, 0, 150, 320, 70);
					SigPlusNET.ClearSigWindow(1);
				}
				else if (SigPlusNET.KeyPadQueryHotSpot(0) > 0) //Done
				{
					Image SignatureBitmap;

					SigPlusNET.ClearSigWindow(1);
					SigPlusNET.LCDRefresh(1, 246, 26, 68, 21); //invert px at done so user knows its been tapped

					if (SigPlusNET.NumberOfTabletPoints() > 0) //if there is a signature
					{
						SigPlusNET.SetImageXSize(500);
						SigPlusNET.SetImageYSize(150);
						SigPlusNET.SetJustifyMode(5);
						SigPlusNET.SetImagePenWidth(7);

						SigPlusNET.SetImageFileFormat(0);
						SignatureBitmap = SigPlusNET.GetSigImage();


						MemoryStream Stream = new MemoryStream();
						SignatureBitmap.Save(Stream, ImageFormat.Jpeg);
						Signature = Stream.GetBuffer();
						SigPlusNET.LCDRefresh(2, 240, 25, 70, 23);

						using (var my12font = new Font("Arial", 12.0F, System.Drawing.FontStyle.Regular))
						{
							SigPlusNET.SetTabletState(1);
							SigPlusNET.LCDWriteString(0, 2, 46, 166, my12font, "Signature Successfully Captured.");
						}

						Thread.Sleep(2000);
						bExitLoop = true;
					}
					else
					{
						using (var my12font = new Font("Arial", 12.0F, System.Drawing.FontStyle.Regular))
						{
							SigPlusNET.SetTabletState(1);
							SigPlusNET.LCDWriteString(0, 2, 46, 166, my12font, "Please Sign Before Continuing...");
						}

						Thread.Sleep(2000);
						SigPlusNET.LCDRefresh(2, 0, 150, 320, 90);
						SigPlusNET.SetTabletState(1);
						SigPlusNET.SetLCDCaptureMode(2);
					}
					if (!bExitLoop)
					{
						SigPlusNET.LCDRefresh(2, 240, 25, 70, 23);
						SigPlusNET.ClearSigWindow(1);
					}
				}
				else if (CaptureCount >= SignatureTimeout)
				{
					using (var my12font = new Font("Arial", 12.0F, System.Drawing.FontStyle.Regular))
					{
						SigPlusNET.SetTabletState(1);
						SigPlusNET.LCDWriteString(0, 2, 46, 166, my12font, "Signature Timeout has Expired !!!");
					}

					Thread.Sleep(2000);
					SigPlusNET.LCDRefresh(2, 0, 150, 320, 59);
					bExitLoop = true;
				}
				else
				{
					++CaptureCount;
				}
			}

			// clear the display and reset the mode before exiting
			SigPlusNET.LCDRefresh(0, 0, 0, 320, 240);
			SigPlusNET.SetLCDCaptureMode(1);
			SigPlusNET.SetTabletState(0);

			return Signature;
		}
	}
}



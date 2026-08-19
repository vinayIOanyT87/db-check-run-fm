/******************************************************************************

	FILE NAME:		LogFile.cs


	PURPOSE:			LogFile Class


	COMMENTS:

		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2000

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+Hauser.


	AUTHOR(S):	W. Gray


	VERSION:		1.0.0  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		---------	----------  -------------------------------------------
		12/04/2008	W.Gray		7.4.6.0 - Revised to issue \r\n rather than \n\r
										so that output formats properly with NotePad.  (CSI 6323)
*******************************************************************************/
using System;

namespace LogClient
{
	/// <summary>
	/// Summary description for LogFile.
	/// </summary>
	internal class LogFile : BaseTarget
	{
		#region Attributes
		protected System.IO.FileStream writer;
//		protected string appName;
		protected int index = 0;
		#endregion Attributes

		public LogFile(string appName) : base(appName)
		{
			Open();
		}

		~LogFile()
		{
			Close();
		}

		protected internal void StringToByteArray(string theString, ref byte[] data, int offset) 
		{ 
			int realI = 0; 
			for(int i = 0; i < theString.Length; ++i) 
			{ 
				realI = (i*2) + offset;
				data[realI] = (byte)(theString[i] & 0xFF);
				data[realI + 1] = (byte)((theString[i] & 0xFF00) >> 16); 
			} 
		}

		protected void Open()
		{
			const string logDirDefault = "C:\\Program Files\\FuelsManager\\Logs";
			string logDir = logDirDefault;
			Microsoft.Win32.RegistryKey Key =
				Microsoft.Win32.Registry.LocalMachine.OpenSubKey("Software\\Varec\\Logger",true);
			if(Key != null)
			{
				logDir = (string) Key.GetValue("LogDir", logDirDefault);
			}
			string dateStamp = System.DateTime.Now.Year.ToString() + "-" + System.DateTime.Now.Month.ToString("00") +
					"-" + System.DateTime.Now.Day.ToString("00");
			string fileName = logDir + "\\" + appName + "_" + dateStamp + ".log";
			writer = new System.IO.FileStream(fileName, System.IO.FileMode.Append, System.IO.FileAccess.Write,
				System.IO.FileShare.Read, 1, false);
		}

		#region Overrides
		override protected void Write(string buffer) 
		{
			++index;

			System.Text.ASCIIEncoding encoder = new System.Text.ASCIIEncoding();

			byte [] byteArray = new Byte[buffer.Length * 2 + 8];
			encoder.GetBytes(buffer, 0, buffer.Length, byteArray, 0);

			writer.Write(byteArray, 0, buffer.Length);
			writer.Flush();
		}
		protected override string Format(LogMessage message)
		{
			return base.Format (message) + "\r\n";
		}
		internal override void RollLog()
		{
			Close();
			Open();
		}

		internal override void Close()
		{
			if(writer != null)
			{
				writer.Close();
			}
		}

		#endregion Overrides
	}
}

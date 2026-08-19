//******************************************************************************
//	FILE NAME:		ASCReporter.cs
//	PURPOSE:			The application class for the ASC Reporting Interface
//
//	COMMENTS:
//		Copyright (C) Varec, Inc. Norcross, GA, USA, 2007
//		This file shall not be copied or reproduced in any form without
//		the express written consent of Varec, Inc.
//
//	AUTHOR(S):	Chris Knight
//	VERSION:		1.0.0.0  Current version
//
//	MODIFICATION HISTORY:
//		Date:			By:				Reason:
//		---------	-------------- -------------------------------------------
//		04-May-2007	C. Knight		1.0.0.0	- Initial Creation
//
//*******************************************************************************       
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace ASCReporter
{
	/// <summary>
	/// Application class
	/// </summary>
	static class ASCReporter
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main ( )
		{
#if !DEBUG
			long parentProcessID;
#endif
			int processID = Process.GetCurrentProcess ( ).Id;
			MainForm mainForm;
#if !DEBUG
			PerformanceCounter myself = new PerformanceCounter("Process", "Creating Process ID", "ASCReporter");
			try
			{
				parentProcessID = myself.RawValue;
				Process parentProcess = Process.GetProcessById((int)parentProcessID);
			
			if (!parentProcess.ProcessName.ToLowerInvariant().Equals("dispatch".ToLowerInvariant()))
				{
					MessageBox.Show(ErrorMessages.ASCNotInContainer);
					return;
				}
			}
			catch (InvalidOperationException e)
			{
				// will come here if unable to find ASCReporter in the Process performance counter collection.
				// This will happen if debugging in the IDE!
				e.ToString();
				MessageBox.Show(ErrorMessages.ASCNotInContainer);
				return;
			}
#endif

			// Check that we are not a second (or further) instance
			Process[] activeReporterCollection = Process.GetProcessesByName ( "ASCReporter" );
			if (activeReporterCollection.Length > 1)
			{
				//Attempt to set the focus to the already active report interface
				// It's currently not working and it is not a requirement, so revisit later.
				//foreach (Process activeReporter in activeReporterCollection)
				//{
				//   if (activeReporter.Id != processID)
				//   {
				//      ShowWindow(activeReporter.MainWindowHandle, SW_SHOW);
				//      SetFocus(activeReporter.MainWindowHandle);
				//   }
				//}
				MessageBox.Show ( ErrorMessages.ASCAlreadyRunning );
				return;
			}

			try
			{

				Application.EnableVisualStyles ( );
				Application.SetCompatibleTextRenderingDefault ( false );
				mainForm = new MainForm ( );
				Application.Run ( mainForm );
			}
			catch (COMException e)
			{
				e.ToString ( );
				MessageBox.Show ( ErrorMessages.ASCNotInContainer );
			}
		}

		#region Win32 P/Invoke calls
		const int SW_HIDE = 0;
		const int SW_SHOWNORMAL = 1;
		const int SW_NORMAL = 1;
		const int SW_SHOWMINIMIZED = 2;
		const int SW_SHOWMAXIMIZED = 3;
		const int SW_MAXIMIZE = 3;
		const int SW_SHOWNOACTIVATE = 4;
		const int SW_SHOW = 5;
		const int SW_MINIMIZE = 6;
		const int SW_SHOWMINNOACTIVE = 7;
		const int SW_SHOWNA = 8;
		const int SW_RESTORE = 9;
		const int SW_SHOWDEFAULT = 10;
		const int SW_FORCEMINIMIZE = 11;
		const int SW_MAX = 11;

		[DllImport ( "user32.dll" )]
		static extern bool ShowWindow ( IntPtr hWnd, int nCmdShow );

		[DllImport ( "user32.dll" )]
		static extern IntPtr SetFocus ( IntPtr hWnd );
		#endregion

	}
}
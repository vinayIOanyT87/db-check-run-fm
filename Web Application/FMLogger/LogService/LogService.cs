/******************************************************************************

	FILE NAME:		LogService.cs


	PURPOSE:			LogService Class


	COMMENTS:

		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2000

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+Hauser.


	AUTHOR(S):	W. Gray


	VERSION:		1.0.0  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		---------	----------  -------------------------------------------
		12/04/2008	W.Gray		7.4.6.0 - Revised set AutoLog = false and
										move Event Logging to LogServer (CSI 6323)
        12/02/2009  S.Jiang     Added new thread for Dormant Accounts Management
*******************************************************************************/
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;


namespace LogService
{
	public class LogService : System.ServiceProcess.ServiceBase
	{
		private System.ServiceProcess.ServiceController serviceController;

        private LogServer logServer;
        private UserRecoveryServer userRecovery;
        private UserAccountCleanup userAccountCleanup;
		  private Thread UserAccountStartupThread = null;

      /// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public LogService()
		{
			AutoLog=false;

			// This call is required by the Windows.Forms Component Designer.
			InitializeComponent();

			logServer = new LogServer();
            userRecovery = new UserRecoveryServer();
            userAccountCleanup = new UserAccountCleanup();
		}

		// The main entry point for the process
		static void Main()
		{
			System.ServiceProcess.ServiceBase[] ServicesToRun;
	
			// More than one user Service may run within the same process. To add
			// another service to this process, change the following line to
			// create a second service object. For example,
			//
			//   ServicesToRun = new System.ServiceProcess.ServiceBase[] {new Service1(), new MySecondUserService()};
			//
			ServicesToRun = new System.ServiceProcess.ServiceBase[] { new LogService() };

			System.ServiceProcess.ServiceBase.Run(ServicesToRun);
		}

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.serviceController = new System.ServiceProcess.ServiceController();
			// 
			// LogService
			// 
			this.ServiceName = "FuelsManager Service";

		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		/// <summary>
		/// Set things in motion so your service can do its work.
		/// </summary>
		protected override void OnStart(string[] args)
		{
			userRecovery.Start();
			logServer.Start();
			ThreadStart UserAccountStart = new ThreadStart(UserAccount);
			UserAccountStartupThread = new Thread(UserAccountStart);
			UserAccountStartupThread.Start();
		}

		public void UserAccount()
		{
			userAccountCleanup.Start();
		}

 
		/// <summary>
		/// Stop this service.
		/// </summary>
		protected override void OnStop()
		{
            for (int tries = 0; tries < 10 && !userRecovery.Stop(); tries++)
            {
                this.RequestAdditionalTime(10000);
                System.Threading.Thread.Sleep(10000);
            }
            for (int tries = 0; tries < 10 && !userAccountCleanup.Stop(); tries++)
            {
                this.RequestAdditionalTime(10000);
                System.Threading.Thread.Sleep(10000);
            }
			logServer.Stop();
		}



	}
}

namespace FMBackupUtilityConfiguration
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Printing;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Runtime.Remoting;
    using System.Runtime.Remoting.Channels;
    using System.Runtime.Remoting.Channels.Tcp;
    using System.Runtime.Remoting.Messaging;
    using System.Security.Cryptography;
    using System.Text;
    using System.Timers;
    using System.Windows.Forms;

    using Crypt;

    using FMBackupLibrary;

    using Properties;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;

    using FMSYSTEMMANAGERLib;

    using Microsoft.VisualBasic.ApplicationServices;

    public partial class MainForm : Form
	{
		const int FuelsmanagerMinimumSentinelRevision = 751;

        /// <summary>
        /// The well-known Guid which identifies the Administrative Site in FuelsManager.
        /// We have to know the Site Admin Guid to login. 
        /// </summary>
        private readonly Guid siteAdminGuid = new Guid("00000000-0000-0000-0000-000000000001");

        /// <summary>
        /// A security object used to interact with FMBusinessServices
        /// </summary>
        private SecurityClass security;

		private string certificateName = null;

        private bool bBuRunning;
		private bool bSendingBackupRequest;

		bool bSecurityKey;
		bool bCanExit;
		bool bLogDirChanged;
		bool bSaveSize;
		bool bViewLog;
		int iFormLeft, iFormTop;
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
		int iFormWidthNoView, iFormWidthWithView;
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
		int iFormHeightNoView, iFormHeightWithView;
		FormWindowState wndStateBeforeMin;

		private int iIconIndex;
		private readonly Icon[] icons;

		string sLogFullPath;

		// ==================================================================================================
		// < BUC APPLICATION AS SERVER >
		// Server - BUC (this MainForm object)
		// Client - BU
		private FMBUCRemote roBuc; // The remote object created here.
		// Delegate for asynchronously running method in UI thread.
		private delegate void ProcessBuMessageDelegate ( MessageEventArgs msgEventArgs ); // Message from BU.
		// ==================================================================================================

		// ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
		// < BU SERVICE AS REMOTE SERVER >
		// Server - BU 
		// Client - BUC (this MainForm object)
		private FMBURemote roBu; // The remote object created in BU.
		// Delegate for asynchronous call, same signature as FMBURemote.SendMessageToBU().
		private delegate void SendBuMessageDelegate ( MessageToBUEventArgs.MsgType msgType );//, string sMessage);
		// ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

		public bool IsBuRunning
		{
			get { lock (this) { return this.bBuRunning; } }
			set { lock (this) { this.bBuRunning = value; } }
		}

		public bool IsSendingBackupRequest
		{
			get { lock (this) { return this.bSendingBackupRequest; } }
			set { lock (this) { this.bSendingBackupRequest = value; } }
		}

		public MainForm ( )
		{
		    this.InitializeComponent();
			var certificateName = FMChannelHelper.MakeCall<IConfigurationSettings, string>(x => x.GetKeyValueByKey(this.security, ConfigurationSettingDOClass.Key_BKUtility_Certificate));
			
			if (string.IsNullOrEmpty(certificateName))
			{
				this.DecryptZipFileButton.Visible = false;
			}

			this.IsSendingBackupRequest = false;

			this.iIconIndex = 0;
			this.icons = new Icon[3];
			this.icons[0] = Resources.DBsClock1;
			this.icons[1] = Resources.DBsClock2;
			this.icons[2] = Resources.DBsClock3;
			this.notifyIconBUC.Icon = this.icons[0];

			this.bSecurityKey = false;
			this.bCanExit = false;
			this.bLogDirChanged = false;
			this.bSaveSize = true;

			// Window size calculation.
			this.bViewLog = true;
			this.iFormLeft = this.Left; // Original value.
			this.iFormTop = this.Top;  // Original value.
			this.iFormWidthWithView = this.Width; // Original value.
			this.iFormWidthNoView = this.Width - this.splitContainer1.Panel2.Width - 3;

			// FormBorderStyle.Sizable
			this.iFormHeightNoView = this.Height - 2; // Original value.
			this.iFormHeightWithView = this.Height; // Original value.

			this.MinimumSize = new Size ( this.iFormWidthNoView + 150, this.Height );

			this.wndStateBeforeMin = FormWindowState.Normal;

			try
			{
				this.tbLogFileLocation.Text = Path.Combine ( Application.StartupPath, "Log" );
				this.tbZipFileLocation.Text = Path.Combine ( Application.StartupPath, "Zip" );
			}
			catch { }

			this.sLogFullPath = null;

		    LoginToFuelsManager();

			this.UpdateLogView ( );

			this.InitializeRemoting ( );
		}

		public void InitializeRemoting ( )
		{
			//            System.Diagnostics.Trace.WriteLine("< InitializeRemoting >");

			if (this.RegistryReadBUCInstance())
			{
				return;
			}

			// ==================================================================================================
			// < BUC APPLICATION AS SERVER >

			try
			{
				// Register a TCP channel.

				// Since default constructor of the channel creates a channel with a name "tcp", let's use a new name.
				IDictionary properties = new Hashtable ( );

				properties["port"] = 50905;
				properties["name"] = "BUCTcp";

				ChannelServices.RegisterChannel ( new TcpChannel ( properties, null, null ), false );

				// Create a remotable object and register it with the remoting service.
				this.roBuc = new FMBUCRemote ( );
				if (this.roBuc == null)
				{
					MessageBox.Show ( this,
								   "Null BUC service.",
								   "FuelsManager Backup Utility",
								   MessageBoxButtons.OK,
								   MessageBoxIcon.Exclamation );
				}
				else
				{
					ObjRef orFMBUCRemote = RemotingServices.Marshal ( this.roBuc, "FMBUCRemote" );

					// Subscribe to message event raised by BUC remote object.
					this.roBuc.MessageEvent += this.roBUC_MessageEvent;
				}
			}
			catch (Exception ex)
			{
				Trace.WriteLine ( ex.Message );
			}
			// ==================================================================================================


			// ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
			// < BU SERVICE REMOTE SERVER RELATED CODE >
			try
			{
				// Register a TCP channel.
				//                ChannelServices.RegisterChannel(new TcpChannel(), false);

				// Since default constructor of the channel creates a channel with a name "tcp", let's use a new name.
				IDictionary prop = new Hashtable ( );
				//                prop["port"] = 50906;
				prop["name"] = "BUTcp";

				ChannelServices.RegisterChannel ( new TcpChannel ( prop, null, null ), false );

				this.roBu = (FMBURemote) Activator.GetObject (
										  typeof ( FMBURemote ),
										  "tcp://localhost:50906/FMBURemote" );
			}
			catch (Exception ex)
			{
				Trace.WriteLine ( ex.Message );
			}
		}

		public void StartupNextInstanceHandler ( object sender, StartupNextInstanceEventArgs e )
		{
			string[] commandLine = new string[e.CommandLine.Count];
			e.CommandLine.CopyTo ( commandLine, 0 );
			Trace.WriteLine ( DateTime.Now.ToShortTimeString ( ) + commandLine[0] );

			this.Show ( );
		}

		// ==================================================================================================
		// < BUC APPLICATION AS SERVER >
		// MessageEvent handler.
		void roBUC_MessageEvent ( object sender, MessageEventArgs e )
		{
			// Use the form's thread.
			this.BeginInvoke ( new ProcessBuMessageDelegate ( ProcessBUMessage ), e);
		}
		// ==================================================================================================

		#region Private Methods

		// ==================================================================================================
		// < BUC APPLICATION AS SERVER >
		private void ProcessBUMessage ( MessageEventArgs msgEventArgs )
		{
			string str = "";
			switch (msgEventArgs.MessageType)
			{
				case MessageEventArgs.MsgType.MSG_STARTED:
                    this.btnBackUpNow.Enabled = false;
					this.IsBuRunning = true;
					str = String.Format ( "{0} {1}",
										msgEventArgs.EventDateTime.ToString ( "yyyy/MM/dd HH:mm:ss" ),
										msgEventArgs.Message );
					this.toolStripStatusLabelMsg.Text = str;
					this.notifyIconBUC.ShowBalloonTip (	1500,
														"FuelsManager Backup Utility",
														str,
														ToolTipIcon.Info );
					// Start tray icon animation.
					this.timerNotifyIcon.Start ( );
					break;

				case MessageEventArgs.MsgType.MSG_COMPLETE:
					this.IsBuRunning = false;

					// Stop tray icon animation.
					this.timerNotifyIcon.Stop ( );
					// Show the regular icon.
					this.notifyIconBUC.Icon = this.icons[0];

					str = String.Format ( "{0} {1}",
										msgEventArgs.EventDateTime.ToString ( "yyyy/MM/dd HH:mm:ss" ),
										msgEventArgs.Message );
					this.toolStripStatusLabelMsg.Text = str;
					notifyIconBUC.ShowBalloonTip ( 1500,
												 "FuelsManager Backup Utility",
												 str,
												 ToolTipIcon.Info );

					this.UpdateLogView ( );
					this.btnBackUpNow.Enabled = true;
					break;

				case MessageEventArgs.MsgType.MSG_FAIL:
					this.IsBuRunning = false;

					// Stop tray icon animation.
					this.timerNotifyIcon.Stop ( );
					// Show the regular icon.
					this.notifyIconBUC.Icon = icons[0];

					str = String.Format ( "{0} {1}",
										msgEventArgs.EventDateTime.ToString ( "yyyy/MM/dd HH:mm:ss" ),
										msgEventArgs.Message );
					this.toolStripStatusLabelMsg.Text = str;

					// Restore, bring to front, and activate window.
					if (this.WindowState == FormWindowState.Minimized) RestoreMainWindow ( );

					this.BringToFront ( );
					this.Activate ( );
					this.Refresh ( );

					notifyIconBUC.ShowBalloonTip ( 1500,
												 "FuelsManager Backup Utility",
												 str,
												 ToolTipIcon.Info );

					MessageBox.Show ( this,
									str,
									"FuelsManager Backup Utility",
									MessageBoxButtons.OK,
									MessageBoxIcon.Error );
					this.UpdateLogView ( );
                    this.btnBackUpNow.Enabled = true;
					break;

				case MessageEventArgs.MsgType.MSG_STATUS:
					break;

				case MessageEventArgs.MsgType.MSG_ERROR:
					break;
			}
		}

		void LogMessage(string msg)
		{
			lvLog.BeginUpdate ( );


			string str = String.Format("{0} {1}",
								DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
								msg);
			lvLog.Items.Add ( str );
			lvLog.EndUpdate();
			lvLog.Update();


		}
		// ==================================================================================================

		private bool IsSecurityKeyPresent ( bool bDisplayMessage )
		{
			ushort usProgramVersion = 0;

			try
			{
                usProgramVersion = FMChannelHelper.MakeCall<IHardwareKey, ushort>(x => x.GetProgramVersionLIN());
                if(usProgramVersion == 0)
                    usProgramVersion = FMChannelHelper.MakeCall<IHardwareKey, ushort>(x => x.GetProgramVersion());

				if (usProgramVersion > 0)
				{
					// the following check is not correct. There are dependencies between versions like the volumecorrection.dll
					// there has already beed one abort in the field because of this check but I am not allowed to change it
					// and have been told to leave it the way it is.
					if (usProgramVersion < FuelsmanagerMinimumSentinelRevision)
					{
						if (bDisplayMessage)
							MessageBox.Show ( this,
											"Installed Hardware key is not for this version of FuelsManager.",
											"FuelsManager Backup Utility",
											MessageBoxButtons.OK,
											MessageBoxIcon.Error );

						return false;
					}
				}
				else
				{
					if (bDisplayMessage)
						MessageBox.Show ( this,
										"Hardware key not found.",
										"FuelsManager Backup Utility",
										MessageBoxButtons.OK,
										MessageBoxIcon.Error );

					return false;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show ( this, ex.Message, "FuelsManager Backup Utility",
								MessageBoxButtons.OK, MessageBoxIcon.Error );
				return false;
			}
			return true;
		}

		private void UpdateLogView ( )
		{
			RegistryReadLogFullPath ( );

			if (!File.Exists ( sLogFullPath )) return;

			try
			{
				// Open the file in read-only mode.
				using (FileStream fs = new FileStream ( sLogFullPath, FileMode.Open, FileAccess.Read, FileShare.Read ))
				{
					using (StreamReader sr = new StreamReader ( fs, Encoding.UTF8 ))
					{
						lvLog.BeginUpdate ( );
						lvLog.Items.Clear ( );

						String str;
						// Read and display lines from the file until the end of 
						// the file is reached.
						while (( str = sr.ReadLine ( ) ) != null)
						{
							lvLog.Items.Add ( str );
						}

						if (lvLog.Items.Count > 2)
						{
							lvLog.Items.Add ( "" );
							lvLog.Items.Add ( "Unclassified/For Official Use Only" );

							lvLog.Items[0].Font = new Font ( lvLog.Items[0].Font,
														   lvLog.Items[0].Font.Style | FontStyle.Bold );

							lvLog.Items[lvLog.Items.Count - 1].Font = new Font ( lvLog.Items[0].Font,
														   lvLog.Items[0].Font.Style | FontStyle.Bold );
						}

						/*
											   if (lvLog.Items.Count > 0)
											   {
												   lvLog.Items.Insert(0, "Unclassified/For Official Use Only");
												   lvLog.Items.Add("Unclassified/For Official Use Only");
							
												   lvLog.Items[0].Font = new Font(lvLog.Items[0].Font, 
																				  lvLog.Items[0].Font.Style | FontStyle.Bold);

												   lvLog.Items[lvLog.Items.Count - 1].Font = new Font(lvLog.Items[0].Font, 
																				  lvLog.Items[0].Font.Style | FontStyle.Bold);
											   }
					   */
						lvLog.EndUpdate ( );
					}
				}
			}
			catch (Exception ex)
			{
				Trace.WriteLine ( ex.Message );
			}
		}

		private void RestoreMainWindow ( )
		{
			this.Show ( );
			this.WindowState = wndStateBeforeMin;
		}

		// Save window size before hiding log view.
		private void SaveWindowSizeBeforeHideLog ( )
		{
			if (this.bSaveSize &&
				this.WindowState == FormWindowState.Normal &&
				this.FormBorderStyle == FormBorderStyle.Sizable)
			{
				if (this.Width > iFormWidthNoView)
				{
					iFormWidthWithView = this.Width;
				}

				if (this.Height > iFormHeightNoView)
				{
					iFormHeightWithView = this.Height;
				}
				else if (this.Height < iFormHeightNoView)
				{
					iFormHeightWithView = iFormHeightNoView;
				}
			}
		}

		private void ReadWindowData ( )
		{
			ConfigurationSettingDOClass configSettingX = null;
			ConfigurationSettingDOClass configSettingY = null;
			ConfigurationSettingDOClass configSettingZX = null;
			ConfigurationSettingDOClass configSettingZY = null;

			FMChannelHelper.MakeCall<IConfigurationSettings>(
				x =>
				{
					configSettingX = x.GetByKey(security, ConfigurationSettingDOClass.Key_BKUtility_xPosition);
					configSettingY = x.GetByKey(security, ConfigurationSettingDOClass.Key_BKUtility_yPosition);
					configSettingZX = x.GetByKey(security, ConfigurationSettingDOClass.Key_BKUtility_zxWidth);
					configSettingZY = x.GetByKey(security, ConfigurationSettingDOClass.Key_BKUtility_zyWidth);
				});


			// If any of the numbers is invalid, don't change the defaults.
			if (string.IsNullOrEmpty(configSettingX.SettingValue) ||
				string.IsNullOrEmpty ( configSettingY.SettingValue ) ||
				string.IsNullOrEmpty ( configSettingZX.SettingValue ) ||
				string.IsNullOrEmpty ( configSettingZY.SettingValue ))
			{
				return;
			}

			int iLeft	= configSettingX.GetIntegerValue ( ).Value;
			int iTop	= configSettingY.GetIntegerValue ( ).Value;
			int iWidth	= configSettingZX.GetIntegerValue ( ).Value;
			int iHeight = configSettingZY.GetIntegerValue ( ).Value;

			Rectangle workingRectangle = Screen.PrimaryScreen.WorkingArea;

			if (iLeft + iWidth < workingRectangle.Left + 20 ||
				iLeft > ( workingRectangle.Right - 20 ))
			{
				return;
			}

			if (iTop + iHeight < workingRectangle.Top + 20 ||
				iTop > ( workingRectangle.Bottom - 40 ))
			{
				return;
			}

			if (iWidth < this.MinimumSize.Width || iWidth > workingRectangle.Width)
			{
				return;
			}

			if (iHeight < this.MinimumSize.Height || iHeight > workingRectangle.Height)
			{
				return;
			}

			iFormLeft			= iLeft;
			iFormTop			= iTop;
			iFormWidthWithView	= iWidth;
			iFormHeightWithView = iHeight;

			this.SetBounds ( iLeft, iTop, iWidth, iHeight );
		}

		private void WriteWindowData ( )
		{
			this.SaveWindowSizeBeforeHideLog ( );

			FMChannelHelper.MakeCall<IConfigurationSettings>(
				configSettings =>
				{
					configSettings.Modify(security, ConfigurationSettingDOClass.Key_BKUtility_xPosition, this.iFormLeft.ToString());
					configSettings.Modify(security, ConfigurationSettingDOClass.Key_BKUtility_yPosition, this.iFormTop.ToString());
					configSettings.Modify(security, ConfigurationSettingDOClass.Key_BKUtility_zxWidth, this.iFormWidthWithView.ToString());
					configSettings.Modify(security, ConfigurationSettingDOClass.Key_BKUtility_zyWidth, this.iFormHeightWithView.ToString());
				});
		}

		private bool RegistryReadBUCInstance ( )
		{
			bool exists = false;

			ConfigurationSettingDOClass configSetting =
				FMChannelHelper.MakeCall<IConfigurationSettings, ConfigurationSettingDOClass>(
					x => x.GetByKey(security, ConfigurationSettingDOClass.Key_BKUtility_BUC));

			if (string.IsNullOrEmpty ( configSetting.SettingValue ) == false) 
			{
				int? iExist = configSetting.GetIntegerValue();

				if (iExist != null)
				{
					exists = iExist == 1 ? true : false;
				}
			}

			return exists;
		}

		private void RegistryWriteBUCInstance ( bool bRunning )
		{
			int buc = bRunning ? 1 : 0;
            FMChannelHelper.MakeCall<IConfigurationSettings>(x => x.Modify( security, ConfigurationSettingDOClass.Key_BKUtility_BUC, buc.ToString()));
		}

		private void ReadConfiguration ( )
		{
			string strTicks = null;
			string strLogFilePath = null;
			string strZipFilePath = null;
			ConfigurationSettingDOClass addFilePaths = null;

			FMChannelHelper.MakeCall<IConfigurationSettings>(
				x =>
				{
					strTicks = x.GetKeyValueByKey(security, ConfigurationSettingDOClass.Key_BKUtility_Ticks);
					strLogFilePath = x.GetKeyValueByKey(security, ConfigurationSettingDOClass.Key_BKUtility_LogFilePath);
					strZipFilePath = x.GetKeyValueByKey(security, ConfigurationSettingDOClass.Key_BKUtility_ZipFilePath);
					addFilePaths = x.GetByKey(security, ConfigurationSettingDOClass.Key_BKUtility_AdditionalFilesPaths);
				});

			if (string.IsNullOrEmpty ( strTicks ) == false)
			{
				Int64 i64Val;
				Int64.TryParse ( strTicks, NumberStyles.Integer, null, out i64Val );

				// The "Ticks" value represents the time of day.
				TimeSpan tsTimeOfDay = new TimeSpan ( i64Val );

				// If registry data is invalid, default to 1:00 AM.
				if (tsTimeOfDay < TimeSpan.Zero || tsTimeOfDay > TimeSpan.FromDays ( 1 ))
				{
					tsTimeOfDay = TimeSpan.FromHours ( 1 );
				}

				DateTime dt = DateTime.Today + tsTimeOfDay;
				dtpStartTime.Value = dt;
			}

			if (string.IsNullOrEmpty ( strLogFilePath ) == false)
			{
				tbLogFileLocation.Text = strLogFilePath;
			}

			if (string.IsNullOrEmpty ( strZipFilePath ) == false)
			{
				tbZipFileLocation.Text = strZipFilePath;
			}

			lbFilesLocations.Items.Clear ( );

			if (string.IsNullOrEmpty ( addFilePaths.SettingValue ) == false)
			{
				string[] sPaths = addFilePaths.GetStringArray ( );

				for (int i = 0; i < sPaths.Length; i++)
				{
					if (!String.IsNullOrEmpty ( sPaths[i] ))
					{
						lbFilesLocations.Items.Add ( sPaths[i] );
					}
				}

				if (lbFilesLocations.Items.Count > 0)
				{
					lbFilesLocations.TopIndex = lbFilesLocations.Items.Count - 1;
				}
			}
		}

		private void WriteConfiguration ( )
		{
			long ticks = dtpStartTime.Value.TimeOfDay.Ticks;

			FMChannelHelper.MakeCall<IConfigurationSettings>(
				x =>
				{
					x.Modify(security, ConfigurationSettingDOClass.Key_BKUtility_Ticks, ticks.ToString());
					x.Modify(security, ConfigurationSettingDOClass.Key_BKUtility_LogFilePath, this.tbLogFileLocation.Text);
					x.Modify(security, ConfigurationSettingDOClass.Key_BKUtility_ZipFilePath, this.tbZipFileLocation.Text);
				});

			if (lbFilesLocations.Items.Count > 0)
			{
				string additionalPaths = "";

				for (int i = 0; i < lbFilesLocations.Items.Count; i++)
				{
					additionalPaths = additionalPaths + lbFilesLocations.Items[i].ToString ( ) + ";";
				}

				FMChannelHelper.MakeCall<IConfigurationSettings>(
					x => x.Modify(security, ConfigurationSettingDOClass.Key_BKUtility_AdditionalFilesPaths, additionalPaths));
			}

			if (this.bLogDirChanged)
			{
				this.bLogDirChanged = false;
				MoveLogFile ( );
			}
		}

		// This can only be called when change is confirmed.
		private void MoveLogFile ( )
		{
			RegistryReadLogFullPath ( );

			try
			{
				if (File.Exists ( sLogFullPath ))
				{
					if (!Directory.Exists ( tbLogFileLocation.Text ))
					{
						Directory.CreateDirectory ( tbLogFileLocation.Text );
					}

					string sFileName = Path.GetFileName ( sLogFullPath );
					string sNewLogFullPath = Path.Combine ( tbLogFileLocation.Text, sFileName );
					File.Move ( sLogFullPath, sNewLogFullPath );
					sLogFullPath = sNewLogFullPath;
					RegistryWriteLogFullPath ( );
				}
			}
			catch (Exception ex)
			{
				Trace.WriteLine ( ex.Message );
			}
		}

	    private bool LoginToFuelsManager()
	    {
	        var loginSecurity = new SecurityClass
	                            {
	                                UserGuid = Guid.Empty,
	                                LoginSiteGuid = this.siteAdminGuid,
	                                SiteGuid = this.siteAdminGuid
	                            };

	        loginSecurity.UserID =
	            FMChannelHelper.MakeCall<IDBAccess, string>(
	                fuelsManagerDatabaseAccess => fuelsManagerDatabaseAccess.ServiceLogin(loginSecurity));

	        loginSecurity.AddRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS);
	        loginSecurity.AddRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS);
	        loginSecurity.AddRight(RIGHT.MODIFY_CONFIGURATION_SETTINGS);

	        this.security = loginSecurity;

	        return true;
	    }

	    private void RegistryReadLogFullPath ( )
		{
			SecurityClass security = new SecurityClass ( );

			string strLogFileFullPath = FMChannelHelper.MakeCall<IConfigurationSettings,string>(x => x.GetKeyValueByKey(security, ConfigurationSettingDOClass.Key_BKUtility_LogFileFullPath));

			if (!string.IsNullOrEmpty ( strLogFileFullPath )) 
			{
				this.sLogFullPath = strLogFileFullPath;
			}
		}

		private void RegistryWriteLogFullPath ( )
		{
			SecurityClass security = new SecurityClass ( );

            FMChannelHelper.MakeCall<IConfigurationSettings>( x => x.Modify( security, ConfigurationSettingDOClass.Key_BKUtility_LogFileFullPath, this.sLogFullPath ));
		}

		private void EnableControls ( bool bEnable )
		{
			this.dtpStartTime.Enabled = bEnable;
			this.btnBackUpNow.Enabled = bEnable;
			this.btnBrowseLogLocation.Enabled = bEnable;

			if (bEnable)
			{
				if (lbFilesLocations.Items.Count > 0)
				{
					this.btnRemove.Enabled = true;
				}

				this.dtpStartTime.Select(); // Set focus on the next control after disabling Log in button.
			}
			else
			{
				this.btnRemove.Enabled = false;
			}

			this.btnBrowseFilesLocation.Enabled = bEnable;
			this.btnZipLocation.Enabled = bEnable;
			this.Refresh ( );
		}

		private void UpdateControlStatusOnDataChange ( bool bEnable )
		{
			this.btnApply.Enabled = bEnable;
			this.Refresh ( );
		}

		private void OpenHelpFile ( )
		{
			string sDir = new FileInfo ( Assembly.GetExecutingAssembly ( ).Location ).DirectoryName;
			string sFullPath = Path.Combine ( sDir, "FMBackupUtility.chm" );
			Help.ShowHelp ( this, sFullPath );
		}

		#endregion

		#region BU Service As Remoting Server

		// ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
		// < BU SERVICE AS REMOTE SERVER >
		private void SendBUMessage ( MessageToBUEventArgs.MsgType msgType )
		{
			if (this.roBu == null)
			{
				Trace.WriteLine ( "Null BU service." );
				MessageBox.Show ( this,
								"Null BU service.",
								"FuelsManager Backup Utility",
								MessageBoxButtons.OK,
								MessageBoxIcon.Exclamation );
			}
			else
			{
				// Asynchronous remote call to BU Service.
				AsyncCallback callback = this.SendBUMessageCallBack;
				SendBuMessageDelegate del = this.roBu.SendMessageToBU;
				IAsyncResult ar = del.BeginInvoke ( msgType, /*sMsg,*/ callback, this );
			}
		}

		// Callback method that is called when SendBUMessageDelegate completes its async call.
		private void SendBUMessageCallBack ( IAsyncResult ar )
		{
			// Obtains the last parameter of the delegate call.
			MainForm mainform = (MainForm) ar.AsyncState;
			// Get the delegate object on which the asynchronous call was invoked.
			SendBuMessageDelegate del = (SendBuMessageDelegate) ( (AsyncResult) ar ).AsyncDelegate;

			try
			{
				del.EndInvoke ( ar ); // No return value.
			}
			catch (Exception ex)
			{
				// BU Service Remoting Server is not available.

				Trace.WriteLine ( ex.Message );

				if (IsSendingBackupRequest)
				{
					MessageBox.Show ( this,
									"Could not communicate with the Backup Utility service.\nMake sure that the service is running.",
									"FuelsManager Backup Utility",
									MessageBoxButtons.OK,
									MessageBoxIcon.Exclamation );
				}
			}
			finally
			{
				IsSendingBackupRequest = false;
			}
		}
		// ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

		#endregion

		private void MainForm_Load ( object sender, EventArgs e )
		{
			this.Cursor = Cursors.WaitCursor;
			this.bSecurityKey = IsSecurityKeyPresent ( true );
			this.Cursor = Cursors.Default;
			if (!this.bSecurityKey)
			{
				this.Close ( );
				return;
			}

			ReadWindowData ( );
			ReadConfiguration ( );

			RegistryWriteBUCInstance ( true );

			// For some reason, the App appears in the taskbar eventhough Hide()
			// was called in MainForm_Resize(), so we call it again here.
			this.Hide ( );

			this.notifyIconBUC.Visible = true;

			EnableControls(true);
		}

		private void MainForm_FormClosing ( object sender, FormClosingEventArgs e )
		{
			// Let the OS close this application.
			if (e.CloseReason == CloseReason.WindowsShutDown)
			{
				return;
			}

			// Security key not present, return to close the form.
			if (!this.bSecurityKey)
			{
				return;
			}

			if (!this.bCanExit)
			{
				// User clicked on Close button in top right corner.
				// Implement this case like Minimize button.
				e.Cancel = true; // Do not close form.
				this.WindowState = FormWindowState.Minimized;
			}
			else
			{
				// User clicked on Exit menuitem in tray icon popup menu.
				this.WriteWindowData ( );
			}
		}

		private void MainForm_FormClosed ( object sender, FormClosedEventArgs e )
		{
			this.RegistryWriteBUCInstance ( false );
		}

		private void MainForm_Resize ( object sender, EventArgs e )
		{
			switch (this.WindowState)
			{
				case FormWindowState.Minimized:
					this.Hide ( );
					break;

				case FormWindowState.Maximized:
					this.btnViewLog.Enabled = false;
					wndStateBeforeMin = this.WindowState;
					break;

				default:
					this.btnViewLog.Enabled = true;
					wndStateBeforeMin = this.WindowState;
					this.SaveWindowSizeBeforeHideLog ( );
					break;
			}
		}

		// Tray Icon - Mouse Double Click.
		private void notifyIconBUC_MouseDoubleClick ( object sender, MouseEventArgs e )
		{
			RestoreMainWindow ( );
		}

		// Tray Icon - Open Backup Utility Configuration menu item.
		private void tsmiOpenBackupUtilityConfiguration_Click ( object sender, EventArgs e )
		{
			RestoreMainWindow ( );
		}

		// Tray Icon - Help menu item.
		private void tsmiHelp_Click ( object sender, EventArgs e )
		{
			OpenHelpFile ( );
		}

		// Tray Icon - About menu item.
		private void tsmiAbout_Click ( object sender, EventArgs e )
		{
			tsmiAbout.Enabled = false;
			AppAboutBox about = new AppAboutBox ( );

			if (this.WindowState == FormWindowState.Minimized)
				about.StartPosition = FormStartPosition.CenterScreen;

			about.ShowDialog ( this );
			tsmiAbout.Enabled = true;
		}

		// Tray Icon - Exit menu item.
		private void tsmiExit_Click ( object sender, EventArgs e )
		{
			this.bCanExit = true;
			this.Close ( );
		}

		// Timer for Tray Icon animation.
		private void timerNotifyIcon_Elapsed ( object sender, ElapsedEventArgs e )
		{
			this.notifyIconBUC.Icon = this.icons[this.iIconIndex];
			this.iIconIndex++;
			if (this.iIconIndex > 2)
			{
				this.iIconIndex = 0;
			}
		}

		private void btnViewLog_Click ( object sender, EventArgs e )
		{
			this.bViewLog = !this.bViewLog;
			if (this.bViewLog)
			{
				// Changing from NO log view to log view (from fix border to sizeable border).

				this.bSaveSize = false;
				this.FormBorderStyle = FormBorderStyle.Sizable;
				this.bSaveSize = true;

				this.Size = new Size ( iFormWidthWithView, iFormHeightWithView );
				this.MaximizeBox = true;
				this.statusStripMain.SizingGrip = true;

				this.MinimumSize = new Size ( iFormWidthNoView + 150, iFormHeightNoView + 2 );

				this.btnViewLog.Text = "<< Hide Log";
			}
			else
			{
				// Changing from log view to NO log view (from sizeable border to fix border).

				this.MinimumSize = new Size ( iFormWidthNoView, iFormHeightNoView );

				// Save window size before hiding log view.
				this.SaveWindowSizeBeforeHideLog ( );

				// Resize window to hide log view.
				this.Size = new Size ( iFormWidthNoView, iFormHeightNoView );
				this.FormBorderStyle = FormBorderStyle.FixedSingle;
				this.MaximizeBox = false;
				this.statusStripMain.SizingGrip = false;

				this.btnViewLog.Text = "View Log >>";
			}
		}

		// Validate user against Inventory Management.
		private bool IMAuthenticate ( )
		{
			bool bSuccess = false;

			try
			{
				SECURITY_DATA sd;
				FMSecurityServerClass fmSecuritySvr = new FMSecurityServerClass ( );
				fmSecuritySvr.GetSecurityData ( Environment.UserName, out sd );

				if (sd.SystemPermissions.bConfigure_Database == 1) bSuccess = true;
			}
			catch (Exception ex)
			{
				Trace.WriteLine ( String.Format ( "Could not IMAuthenticate: {0}", ex.Message ) );
			}/**/
			return bSuccess;
		}

		private void btnBackUpNow_Click ( object sender, EventArgs e )
		{
			if (!IsSecurityKeyPresent ( true ))
			{
				this.Close ( );
			}
		    if (this.IsBuRunning)
		    {
		        MessageBox.Show ( "A database backup operation is in progress.",
		            "FuelsManager Backup Utility", MessageBoxButtons.OK,
		            MessageBoxIcon.Exclamation );
		        return;
		    }

		    this.Cursor = Cursors.WaitCursor;
		    this.IsSendingBackupRequest = true;
		    this.SendBUMessage ( MessageToBUEventArgs.MsgType.MSG_BACKUPNOW );
		    this.Cursor = Cursors.Default;
		}

		private void btnBrowseLogLocation_Click ( object sender, EventArgs e )
		{
			FolderBrowserDialog folderBrowserDlg = new FolderBrowserDialog ( );
			folderBrowserDlg.Description = "Select the folder to save the log file.";
			//            folderBrowserDlg.ShowNewFolderButton = false;
			folderBrowserDlg.RootFolder = Environment.SpecialFolder.MyComputer;//.Personal;
			folderBrowserDlg.SelectedPath = tbLogFileLocation.Text;

			DialogResult result = folderBrowserDlg.ShowDialog ( );
			if (result == DialogResult.OK)
			{
				if (tbLogFileLocation.Text.CompareTo ( folderBrowserDlg.SelectedPath ) != 0)
				{
					tbLogFileLocation.Text = folderBrowserDlg.SelectedPath;
					UpdateControlStatusOnDataChange ( true );
					this.bLogDirChanged = true;
				}
			}

		}

		private void btnRemove_Click ( object sender, EventArgs e )
		{
			if (lbFilesLocations.SelectedIndices.Count > 0 &&
				MessageBox.Show("Do you want to remove the selected folder(s) from being backed up?",
							"FuelsManager Backup Utility", MessageBoxButtons.OKCancel,
							MessageBoxIcon.Exclamation ) == DialogResult.OK)
			{
				while (lbFilesLocations.SelectedIndices.Count > 0)
					lbFilesLocations.Items.RemoveAt ( lbFilesLocations.SelectedIndices[0] );

				if (lbFilesLocations.Items.Count < 1) btnRemove.Enabled = false;
				UpdateControlStatusOnDataChange ( true );
			}
		}

		private void btnBrowseFilesLocation_Click ( object sender, EventArgs e )
		{
			FolderBrowserDialog folderBrowserDlg = new FolderBrowserDialog ( );
			folderBrowserDlg.Description = "Select the folder that contains additional files that is to be backed up";
			folderBrowserDlg.ShowNewFolderButton = false;
			folderBrowserDlg.RootFolder = Environment.SpecialFolder.MyComputer;

			DialogResult result = folderBrowserDlg.ShowDialog ( );
			if (result == DialogResult.OK)
			{
				if (folderBrowserDlg.SelectedPath.Length > 0)
				{
					lbFilesLocations.Items.Add ( folderBrowserDlg.SelectedPath );
					lbFilesLocations.TopIndex = lbFilesLocations.Items.Count - 1;
					btnRemove.Enabled = true;
					UpdateControlStatusOnDataChange ( true );
				}
			}
		}

		private void btnZipLocation_Click ( object sender, EventArgs e )
		{
			FolderBrowserDialog folderBrowserDlg = new FolderBrowserDialog ( );
			folderBrowserDlg.Description = "Select the folder to save the zip file.";
			folderBrowserDlg.SelectedPath = tbZipFileLocation.Text;

			DialogResult result = folderBrowserDlg.ShowDialog ( );
			if (result == DialogResult.OK)
			{
				if (tbZipFileLocation.Text.CompareTo ( folderBrowserDlg.SelectedPath ) != 0)
				{
					tbZipFileLocation.Text = folderBrowserDlg.SelectedPath;
					UpdateControlStatusOnDataChange ( true );
				}
			}
		}

		private void btnOK_Click ( object sender, EventArgs e )
		{
			// Exit configure mode.
			this.WindowState = FormWindowState.Minimized;

			if (this.btnApply.Enabled) // Indicates configuration has been changed.
			{
				btnApply.Refresh ( );
				this.Cursor = Cursors.WaitCursor;

				this.WriteConfiguration ( );

				SendBUMessage ( MessageToBUEventArgs.MsgType.MSG_UPDATECONFIG );
				this.Cursor = Cursors.Default;
			}
		}

		private void btnCancel_Click ( object sender, EventArgs e )
		{
			if (this.btnApply.Enabled) // Indicates configuration has been changed.
			{
				DialogResult result;
				result = MessageBox.Show ( this,
										 "Do you want to save the configuration data?",
										 "FuelsManager Backup Utility",
										 MessageBoxButtons.YesNo,
										 MessageBoxIcon.Question );
				switch (result)
				{
					case DialogResult.Yes:      // Same as selecting OK button.
						// Exit configure mode.
						this.WindowState = FormWindowState.Minimized;

						btnApply.Refresh ( );
						this.Cursor = Cursors.WaitCursor;

						this.WriteConfiguration ( );

						SendBUMessage ( MessageToBUEventArgs.MsgType.MSG_UPDATECONFIG );
						this.Cursor = Cursors.Default;

						return;

					case DialogResult.No: // Don't save, proceed to the code below.
					default:
						break;
				}
			}

			// Exit configure mode.
			this.WindowState = FormWindowState.Minimized;

			if (this.btnApply.Enabled) // Indicates configuration has been changed.
			{
				btnApply.Enabled = false;
				btnApply.Refresh ( );

				// Read registry to restore configuration.
				ReadConfiguration ( );
			}
		}

		private void btnApply_Click ( object sender, EventArgs e )
		{
			this.btnApply.Enabled = false;
			this.Refresh ( );
			this.Cursor = Cursors.WaitCursor;

			// Exit configure mode.
			this.WriteConfiguration ( );

			SendBUMessage ( MessageToBUEventArgs.MsgType.MSG_UPDATECONFIG );

			btnBackUpNow.Enabled = true;
			this.Refresh ( );

			this.Cursor = Cursors.Default;
			this.btnApply.Enabled = true;
		}

		private void btnHelp_Click ( object sender, EventArgs e )
		{
			OpenHelpFile ( );
		}

		private void Doc_PrintPage ( object sender, PrintPageEventArgs e )
		{
			// Retrieve the document that sent this event.
			TextDocument doc = (TextDocument) sender;

			// Define the font and determine the line height.
			using (Font font = new Font ( "Arial", 10 ))
			{
				float lineHeight = font.GetHeight ( e.Graphics );

				// Create variables to hold position on page.
				float x = e.MarginBounds.Left;
				float y = e.MarginBounds.Top;

				// Increment the page counter (to reflect the page that 
				// is about to be printed).
				doc.PageNumber += 1;

				// Print all the information that can fit on the page.        
				// This loop ends when the next line would go over the
				// margin bounds, or there are no more lines to print.

				while (( y + lineHeight ) < e.MarginBounds.Bottom &&
				  doc.Offset <= doc.Text.GetUpperBound ( 0 ))
				{
					e.Graphics.DrawString ( doc.Text[doc.Offset], font,
					  Brushes.Black, x, y );

					// Move to the next line of data.
					doc.Offset += 1;

					// Move the equivalent of one line down the page.
					y += lineHeight;
				}

				if (doc.Offset < doc.Text.GetUpperBound ( 0 ))
				{
					// There is still at least one more page.
					// Signal this event to fire again.
					e.HasMorePages = true;
				}
				else
				{
					// Printing is complete.
					doc.Offset = 0;
				}
			}
		}

		private void btnPrint_Click ( object sender, EventArgs e )
		{
			if (lvLog.Items.Count > 0)
			{
				// Create a document with 100 lines.
				string[] printText = new string[lvLog.Items.Count];

				for (int i = 0; i < printText.Length; i++)
				{
					printText[i] = lvLog.Items[i].Text;
				}

				PrintDocument doc = new TextDocument ( printText );
				doc.PrintPage += this.Doc_PrintPage;

				using (var dlgSettings = new PrintDialog())
				{
					dlgSettings.Document = doc;

					// If the user clicked OK, print the document.
					if (dlgSettings.ShowDialog() == DialogResult.OK)
					{
						doc.Print();
					}
				}
			}
		}

		private void lbFilesLocations_MouseMove ( object sender, MouseEventArgs e )
		{
			// Show a tooltip.
			string sTip = "";

			int iIndex = lbFilesLocations.IndexFromPoint ( e.Location );
			if (( iIndex >= 0 ) && ( iIndex < lbFilesLocations.Items.Count ))
			{
				sTip = lbFilesLocations.Items[iIndex].ToString ( );
				toolTipMain.SetToolTip ( lbFilesLocations, sTip );
			}
		}

		private void MainForm_LocationChanged ( object sender, EventArgs e )
		{
			if (this.WindowState == FormWindowState.Normal)
			{
				iFormLeft = this.Left;
				iFormTop = this.Top;
			}
		}

		private void MainForm_Shown ( object sender, EventArgs e )
		{
			//            System.Diagnostics.Trace.WriteLine("<MainForm_Shown.>");
		}

		private void dtpStartTime_ValueChanged ( object sender, EventArgs e )
		{
			if (this.dtpStartTime.Enabled)
			{
				UpdateControlStatusOnDataChange ( true );
			}
		}



		private void Decrypt_Click(object sender, EventArgs e)
		{
			var unzipDlg = new System.Windows.Forms.OpenFileDialog();
			unzipDlg.Title = "Select the file to unzip";
			unzipDlg.CheckFileExists = true;

			DialogResult result = unzipDlg.ShowDialog();
			if (result == DialogResult.OK)
			{
				this.UseWaitCursor = true;
				
				if (unzipDlg.FileName.Length > 0 && unzipDlg.FileName.Right(4) == ".vef")
				{

					string password = "testing";

					string sTargetFullPath = unzipDlg.FileName ;
					var decryptor = new FMBusinessObjects.UtilityObjects.Decryption(Encoding.UTF8);
					try
					{
						//Encoding encoding = Encoding.UTF8;

						RSACrypt cryptor = new RSACrypt();
						using (RSACertificate theCert = new RSACertificate(certificateName))
						{
							if (theCert.Certificate == null)
							{
								LogMessage(string.Format("Certificate with name {0} not found. Could not extract password file.", certificateName));
								return;
							}
							if (theCert.Certificate.PrivateKey == null)
							{
								LogMessage("Certificate missing a private key. Could not extract password file.");
								return;
							}

							LogMessage("Extracting zip file.");
							FileStream fs = new FileStream(sTargetFullPath, FileMode.Open);

							byte[] signature = new byte[256];
							fs.Read(signature, 0, 256);

							byte[] encryptedPassword = new byte[256];
							fs.Read(encryptedPassword, 0, 256);

							sTargetFullPath = sTargetFullPath.Left(sTargetFullPath.Length - 4);
							FileStream plainFs = new FileStream(sTargetFullPath, FileMode.CreateNew);

							byte[] buf = new byte[256];
							int l = 0;
							while ((l = fs.Read(buf, 0, 256)) > 0)
							{
								plainFs.Write(buf, 0, l);
							}

							plainFs.Close();
							
							fs.Close();
							LogMessage("Successfully extracted zip file.\n\rCreating password file.");

							var p = new RSACryptoServiceProvider();
							RSAParameters rp = new RSAParameters();
							rp = ((RSACryptoServiceProvider)theCert.Certificate.PrivateKey).ExportParameters(true);
							p.ImportParameters(rp);
							p.PersistKeyInCsp = false;
							if (!p.VerifyData(encryptedPassword, new SHA256CryptoServiceProvider(), signature))
							{

								LogMessage("Password signature verification failed.\n\rCould not extract password file.");
								return;
							}


							byte[] valueDecrypted = cryptor.Decrypt(encryptedPassword, theCert);
							password = System.Text.Encoding.UTF8.GetString(valueDecrypted);
							sTargetFullPath += ".password";

							File.WriteAllLines(sTargetFullPath, new string[]{password});
							LogMessage("Successfully extracted password file.");

						}
					}
					catch (Exception ex)
					{
						string msg = ex.Message;
						LogMessage("Exception: " + msg);
						MessageBox.Show(this,
										msg,
										"FuelsManager Backup Utility",
										MessageBoxButtons.OK,
										MessageBoxIcon.Error);
					}
				}
				this.UseWaitCursor = false;
			}

		}


	}
}
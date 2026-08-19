/******************************************************************************

	FILE NAME:		BUService.cs


	PURPOSE:			Automatic Backup Utility service for FuelsManager Defense


	COMMENTS:

		Copyright (C) Varec, Inc. Norcross, GA, USA

		This file shall not be copied or reproduced in any form without
				the express written consent of Varec.


	AUTHOR(S):	A. Chan


	VERSION:		1.0.0.1  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		---------	----------  -------------------------------------------
		??-???-????	A. Chan		1.0.0.0	- Initial creation
 
		08-Apr-2009 C. Knight	1.0.0.1	- Extended SQLCommand timeouts from 60 seconds to 300
											seconds to provide an interim fix to Bug 3067
*******************************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using System.Linq;

using System.IO;
using System.Threading;
using System.Runtime.Remoting; // For ObjRef
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Messaging; // For AsyncResult
using FMBackupLibrary;
using System.Data.SqlClient;
using ICSharpCode.SharpZipLib.Zip;
using System.Globalization;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;

using Crypt;
using System.Security.Cryptography;

namespace FMBackupUtility
{
	public partial class BUService : ServiceBase
	{
		const int FUELSMANAGER_MINIMUM_SENTINEL_REVISION = 751;

        /// <summary>
        /// The well-known Guid which identifies the Administrative Site in FuelsManager.
        /// We have to know the Site Admin Guid to login. 
        /// </summary>
        private readonly Guid siteAdminGuid = new Guid("00000000-0000-0000-0000-000000000001");

        /// <summary>
        /// A security object used to interact with FMBusinessServices
        /// </summary>
        private SecurityClass security;

		private static AutoResetEvent eventRun = new AutoResetEvent ( false );
		private bool bRunning;
		private bool bLogInfo;

		//        DateTime dtStartTime;
		TimeSpan tsTimeOfDay;
		string sLogFileLocation;
		string sZipFileLocation;
		string[] sPaths;

		string sCurrentLog;

		string certificateName = null;

		string sSQLPath;       // Audit trace files (FMDAuditTrace*.trc).
		string sFMProjectPath; // Inventory Management real-time DB files.
		string sADCFDCPath;    // Syn-Tech Systems ADC-FDC DB.
		string sDoDFMAEPath;   // Syn-Tech Systems DodFM AE DBs (FuelMaster).

		string sHoldingDir;

		Timer timerBU;
		Timer timerCreateLog;
		Timer timerCheckKey;
		Timer timerStatus;

		string progressMessage = "";

		FileStream fs;
		StreamWriter sw;
		//        FileStream fsBULog;
		//        StreamWriter swBULog;

		// ==================================================================================================
		// < BUC APPLICATION AS REMOTE SERVER >
		// Server - BUC
		// Client - BU (this BUService object)
		private FMBUCRemote roBUC; // The remote object created in BUC.
		// Delegate for asynchronous call, same signature as FMBUCRemote.UpdateMessage().
		private delegate void SendBUCMessageDelegate ( MessageEventArgs.MsgType msgType, string sMessage, DateTime dt );
		// ==================================================================================================


		// ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
		// < BU SERVICE AS SERVER >
		// Server - BU (this BUService object)
		// Client - BUC
		private FMBURemote roBU; // The remote object created here.
		//        private delegate void ProcessMessageFromBUCDelegate(MessageToBUEventArgs msgToBUEventArgs); // Message from BUC.
		// ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::


		static bool bTerminate = false;
		private Thread threadBackup = null;

		public bool IsRunning
		{
			get { lock (this) { return bRunning; } }
			set { lock (this) { bRunning = value; } }
		}

		// A new BUService object is created whenever the service is STARTed,
		// and disposed of whenever the service is STOPped.
		public BUService ( )
		{
			InitializeComponent ( );

			bRunning = false;
			//
			bLogInfo = false;

			sHoldingDir = null;

			sLogFileLocation = null;
			sZipFileLocation = null;
			sPaths = null;

			sCurrentLog = null;

			sSQLPath = null;       // Audit trace files (FMDAuditTrace*.trc).
			sFMProjectPath = null; // Inventory Management real-time DB files.
			sADCFDCPath = null;    // Syn-Tech Systems ADC-FDC DB.
			sDoDFMAEPath = null;   // Syn-Tech Systems DodFM AE DBs (FuelMaster).

			fs = null;
			sw = null;

		    
			ReadConfiguration ( );
		}

		#region Private Methods

		#region Log

		public void LogInfo ( string sCaller, string sInfo )
		{
			LogInfo ( sCaller, sInfo, DateTime.Now );
		}

		// Log trace info to a text file to aid development.
		public void LogInfo ( string sCaller, string sInfo, DateTime dt )
		{
			if (bLogInfo != true) return;

			//            if (dt == null) dt = DateTime.Now;

			if (sw == null)
			{
				// In Windows Service, must provide full path to FileStream.
				string sDir = new FileInfo ( System.Reflection.Assembly.GetExecutingAssembly ( ).Location ).DirectoryName;
				string sFileName = String.Format ( "Info{0}.txt", dt.ToString ( "yyyyMMdd-HHmmss" ) );
				string sFullPath = Path.Combine ( sDir, sFileName );
				fs = new FileStream ( sFullPath, FileMode.Append, FileAccess.Write );
				sw = new StreamWriter ( fs, Encoding.UTF8 );
				// Make StreamWriter flush its buffer to the underlying stream after every call to StreamWriter.Write().
				sw.AutoFlush = true;
			}

			if (sw != null)
			{
				try
				{
					string str;
					if (sCaller.Length == 0)
						str = String.Format ( "{0} {1}", dt.ToString ( "yyyy/MM/dd HH:mm:ss" ), sInfo );
					else
						str = String.Format ( "{0} [{1}] {2}", dt.ToString ( "yyyy/MM/dd HH:mm:ss" ), sCaller, sInfo );

					sw.WriteLine ( str );
				}
				catch (Exception ex)
				{
					EventLog.WriteEntry ( ex.Message );
				}
			}
		}

		// Check if an existing log file exists.
		// Create a new log if there isn't one.
		private void OpenBULog ( DateTime dt )
		{
			//            if (swBULog == null)
			//            {
			bool bUseExisting = false;
			string sFullPathFromReg = RegistryReadLogFullPath ( );
			string sFileNameFromReg;

			try
			{
				if (File.Exists ( sFullPathFromReg ))
				{
					// Check to see if the filename contains today's date.
					sFileNameFromReg = Path.GetFileName ( sFullPathFromReg );
					if (sFileNameFromReg.Contains ( dt.ToString ( "yyyyMMdd" ) ))
					{
						// Use the existing log file.
						bUseExisting = true;
					}
					else
					{
						// Create a new log file.

						// But first, append the footer to the existing file.
						try
						{
							using (StreamWriter sw = File.AppendText ( sFullPathFromReg ))
							{
								sw.WriteLine ( "" );
								sw.WriteLine ( "Unclassified/For Official Use Only" );
							}
						}
						catch (Exception ex)
						{
							LogInfo ( "OpenBULog", ex.Message );
						}
					}
				}
			}
			catch { }

			if (bUseExisting) sCurrentLog = sFullPathFromReg;
			else
			{
				// Use new file name.

				string sDir;

				try
				{
					// Create the Log file directory if it does not exist.

					if (!Directory.Exists ( sLogFileLocation ))
						Directory.CreateDirectory ( sLogFileLocation );
					sDir = sLogFileLocation;
				}
				catch
				{
					// If failed to create directory, use EXE directory.
					sDir = new FileInfo ( System.Reflection.Assembly.GetExecutingAssembly ( ).Location ).DirectoryName;
				}

				string sFileName = String.Format ( "BackupLog{0}.txt", dt.ToString ( "yyyyMMdd-HHmmss" ) );
				//                    sFullPath
				sCurrentLog = Path.Combine ( sDir, sFileName );

				try
				{
					// Creates a new file if it doesn't exists.
					using (StreamWriter sw = File.AppendText ( sCurrentLog ))
					{
						sw.WriteLine ( "Unclassified/For Official Use Only" );
						sw.WriteLine ( "" );
					}
				}
				catch (Exception ex)
				{
					LogInfo ( "OpenBULog", ex.Message );
				}

				// Save full log file name in registry.
				RegistryWriteLogFullPath ( sCurrentLog );//sFullPath);
			}

			LogInfo ( "OpenBULog", sCurrentLog );//sFullPath);
			
		}

		public void LogBUStep ( string sStep )
		{
			LogBUStep ( sStep, DateTime.Now );
		}

		// Log backup operation infomation to a text file.
		public void LogBUStep ( string sStep, DateTime dt )
		{

			try
			{
				if (!File.Exists ( sCurrentLog )) OpenBULog ( DateTime.Now );

				using (StreamWriter sw = File.AppendText ( sCurrentLog ))
				{
					string str = String.Format ( "{0} {1}", dt.ToString ( "yyyy/MM/dd HH:mm:ss" ), sStep );
					sw.WriteLine ( str );
				}
			}
			catch (Exception ex)
			{
				EventLog.WriteEntry ( ex.Message );
			}
		}

		#endregion // Log

		#region Registry


        private bool LoginToFuelsManager()
        {
            try
            {
                var loginSecurity = new SecurityClass
                {
                    UserGuid = Guid.Empty,
                    LoginSiteGuid = this.siteAdminGuid,
                    SiteGuid = this.siteAdminGuid
                };

                loginSecurity.UserID = FMChannelHelper.MakeCall<IDBAccess, string>(fuelsManagerDatabaseAccess => fuelsManagerDatabaseAccess.ServiceLogin(loginSecurity));

                loginSecurity.AddRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS);
                loginSecurity.AddRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS);
                loginSecurity.AddRight(RIGHT.MODIFY_CONFIGURATION_SETTINGS);

                this.security = loginSecurity;

                return true;
            }
            catch (Exception ex)
            {
                LogInfo("LoginToFuelsManager", ex.Message);
                EventLog.WriteEntry(ex.Message);
                return false;
            }
        }

		//private IConfigurationSettings GetConfigurationSettings()
		//{
		//	FMChannelFactory<IConfigurationSettings> configSettingsClient = new FMChannelFactory<IConfigurationSettings>();
		//	FMChannelFactoryConfigInfo<IConfigurationSettings> ChannelConfigInfo = new FMChannelFactoryConfigInfo<IConfigurationSettings>();
		//	System.ServiceModel.EndpointAddress address = ChannelConfigInfo.GetEndPointAddress();
		//	if (address != null)
		//	{
		//		System.ServiceModel.Channels.Binding binding = ChannelConfigInfo.GetBinding(address);
		//		if (binding != null)
		//		{
		//			configSettingsClient = new FMChannelFactory<IConfigurationSettings>(binding, address);
		//		}
		//	}
		//	EventLog.WriteEntry(string.Format("Endpoint address: {0}\nBinding Name: {1}", configSettingsClient.Address.Uri.AbsoluteUri, configSettingsClient.Binding.Name), EventLogEntryType.Information); // Log address and binding info.
		//	return configSettingsClient.CreateProxy();

		//}
		//private IHardwareKey GetHardwareKey()
		//{
		//	FMChannelFactory<IHardwareKey> hardwareKeyClient = new FMChannelFactory<IHardwareKey>();
		//	FMChannelFactoryConfigInfo<IHardwareKey> ChannelConfigInfo = new FMChannelFactoryConfigInfo<IHardwareKey>();
		//	System.ServiceModel.EndpointAddress address = ChannelConfigInfo.GetEndPointAddress();
		//	if (address != null)
		//	{
		//		System.ServiceModel.Channels.Binding binding = ChannelConfigInfo.GetBinding(address);
		//		if (binding != null)
		//		{
		//			hardwareKeyClient = new FMChannelFactory<IHardwareKey>(binding, address);
		//		}
		//	}
		//	EventLog.WriteEntry(string.Format("Endpoint address: {0}\nBinding Name: {1}", hardwareKeyClient.Address.Uri.AbsoluteUri, hardwareKeyClient.Binding.Name), EventLogEntryType.Information); // Log address and binding info.
		//	return hardwareKeyClient.CreateProxy();

		//}
		// Read backup start time from registry.
		private void ReadScheduledTime ( )
		{
			bool bWriteConfig = false;


			string strTicks =
				FMChannelHelper.MakeCall<IConfigurationSettings, string>(x => x.GetKeyValueByKey(security, ConfigurationSettingDOClass.Key_BKUtility_Ticks));


			if (string.IsNullOrEmpty ( strTicks ) == false)
			{
				Int64 i64Val;
				Int64.TryParse ( strTicks, NumberStyles.Integer, null, out i64Val );

				tsTimeOfDay = new TimeSpan ( i64Val );

				if (tsTimeOfDay < TimeSpan.Zero || tsTimeOfDay > TimeSpan.FromDays ( 1 ))
				{
					tsTimeOfDay = TimeSpan.FromHours ( 1 );
				}
			}
			else
			{
				tsTimeOfDay = TimeSpan.FromHours ( 1 );
				bWriteConfig = true;
			}

			if (bWriteConfig == true)
			{
				WriteScheduledTime ( );
			}
		}

		private void WriteScheduledTime ( )
		{
			FMChannelHelper.MakeCall<IConfigurationSettings>(x => x.Modify( security, ConfigurationSettingDOClass.Key_BKUtility_Ticks, tsTimeOfDay.Ticks.ToString()));
		}

		// Read file paths from registry.
		private void ReadConfiguration ( )
		{
			bool bWriteConfig = false;
			string strLogPath = null;
			string strZipPath = null;
			string strSqlDataRoot = null;
			string strTraceFolder = null;
			string strProjectPath = null;
			string strCurrDB = null;
			string strSyncTechSysHome = null;
			ConfigurationSettingDOClass configSetting = null;

			FMChannelHelper.MakeCall<IConfigurationSettings>(
				x =>
				{
					certificateName = x.GetKeyValueByKey(security, ConfigurationSettingDOClass.Key_BKUtility_Certificate);
					strLogPath = x.GetKeyValueByKey ( security, ConfigurationSettingDOClass.Key_BKUtility_LogFilePath );
					strZipPath = x.GetKeyValueByKey ( security, ConfigurationSettingDOClass.Key_BKUtility_ZipFilePath );
					strSqlDataRoot = x.GetKeyValueByKey(security, ConfigurationSettingDOClass.Key_BKUtility_SQLDataRoot);
					strTraceFolder = x.GetKeyValueByKey(security, ConfigurationSettingDOClass.Key_BKUtility_SQLTraceFolder);
					strProjectPath = x.GetKeyValueByKey(security, ConfigurationSettingDOClass.Key_BKUtility_Project);
					strCurrDB = x.GetKeyValueByKey(security, ConfigurationSettingDOClass.Key_BKUtility_CurrDB);
					strSyncTechSysHome = x.GetKeyValueByKey(security, ConfigurationSettingDOClass.Key_BKUtility_SyncTechSystemHome);
					configSetting = x.GetByKey(security, ConfigurationSettingDOClass.Key_BKUtility_AdditionalFilesPaths);
				});



			if (string.IsNullOrEmpty ( strLogPath ) == false)
			{
				sLogFileLocation = strLogPath;
			}
			else
			{
				// If no path in Registry, use ExeDir\Log directory.
				try
				{
					sLogFileLocation = new FileInfo ( System.Reflection.Assembly.GetExecutingAssembly ( ).Location ).DirectoryName;
					sLogFileLocation = Path.Combine ( sLogFileLocation, "Log" );

					if (!Directory.Exists ( sLogFileLocation )) Directory.CreateDirectory ( sLogFileLocation );
					{
						bWriteConfig = true;
					}
				}
				catch 
				{ 
				}
			}
			 
			
			if (string.IsNullOrEmpty(strZipPath) == false)
			{
				sZipFileLocation = strZipPath;
			}
			else
			{
				// If no path in Registry, use ExeDir\Zip directory.
				try
				{
					sZipFileLocation = new FileInfo ( System.Reflection.Assembly.GetExecutingAssembly ( ).Location ).DirectoryName;
					sZipFileLocation = Path.Combine ( sZipFileLocation, "Zip" );

					if (!Directory.Exists ( sZipFileLocation )) Directory.CreateDirectory ( sZipFileLocation );
					{
						bWriteConfig = true;
					}
				}
				catch 
				{ 
				}
			}

			

			if (string.IsNullOrEmpty ( configSetting.SettingValue ) == false)
			{
				sPaths = configSetting.GetStringArray();
			}

			if (bWriteConfig == true)
			{
				WriteConfiguration ( );
			}

			// Get path for audit trace files (FMDAuditTrace*.trc).
			// SQL data root is the location of where microsoft sql service is installed.
			

			if (string.IsNullOrEmpty ( strSqlDataRoot ) == false)
			{
				if (string.IsNullOrEmpty(strTraceFolder))
					strTraceFolder = "Log";
				try
				{
					sSQLPath = Path.Combine(strSqlDataRoot, strTraceFolder);
				}
				catch
				{
					sSQLPath = null;
				}
			}


			// Get path for Inventory Management real-time DB files.
			

			if (string.IsNullOrEmpty ( strProjectPath ) == false)
			{
				sFMProjectPath = strProjectPath;
			}

			// Get path for Syn-Tech Systems ADC-FDC DB.
			

			if (string.IsNullOrEmpty ( strCurrDB ) == false)
			{
				try
				{
					sADCFDCPath = strCurrDB;

					// Remove filename.
					sADCFDCPath = Path.GetDirectoryName ( sADCFDCPath );
				}
				catch
				{
					sADCFDCPath = null;
				}
			}

			// Get path for Syn-Tech Systems DodFM AE DBs (FuelMaster).
			// @"SOFTWARE\Syn-Tech Systems\DoDFM Adv"

			if (string.IsNullOrEmpty ( strSyncTechSysHome ) == false) 
			{
				sDoDFMAEPath =strSyncTechSysHome;
			}
		}

		private void WriteConfiguration ( )
		{

			FMChannelHelper.MakeCall<IConfigurationSettings>(
				x =>
				{
					x.Modify(security, ConfigurationSettingDOClass.Key_BKUtility_LogFilePath, this.sLogFileLocation);
					x.Modify(security, ConfigurationSettingDOClass.Key_BKUtility_ZipFilePath, this.sZipFileLocation);
				});
		}

		// Read file paths from registry.
		private string RegistryReadLogFullPath ( )
		{
			string str = "";


			string strLogFileFullPath = FMChannelHelper.MakeCall<IConfigurationSettings, string>(x => x.GetKeyValueByKey ( security, ConfigurationSettingDOClass.Key_BKUtility_LogFileFullPath ));

			if (string.IsNullOrEmpty ( strLogFileFullPath ) == false)
			{
				str = strLogFileFullPath;
			}

			return str;
		}

		// Write the Log filename and location in the registry.
		private void RegistryWriteLogFullPath ( string sFullPath )
		{
			if (File.Exists ( sFullPath ))
			{
				FMChannelHelper.MakeCall<IConfigurationSettings>( x => x.Modify( security, ConfigurationSettingDOClass.Key_BKUtility_LogFileFullPath, sFullPath ));
			}
		}

		#endregion // Registry

		private bool BackUpSQLDB ( string dbname )
		{
			using (SqlConnection con = new SqlConnection ( ))
			{
				// Configure the SqlConnection object's connection string.
				con.ConnectionString = DBAdminConnect.getConnectionString (  dbname );

				try
				{
					// Open the database connection.
					con.Open ( );

					if (con.State == ConnectionState.Open)
					{
						// Create and configure a new command.
						IDbCommand com = con.CreateCommand ( );
						com.CommandType = CommandType.Text;
						com.CommandTimeout = 300;
						string sPath = Path.Combine ( sHoldingDir, dbname + ".bak" );
						com.CommandText = String.Format ( "backup database {0} to disk = '{1}' with init", dbname, sPath );

						com.ExecuteNonQuery ( );
						LogBUStep ( "Successfully backed up " + dbname + "." );
						LogInfo ( "BackUpSQLDBs", "Successfully backed up " + dbname + "." );
					}
					else
					{
						LogBUStep ( "Could not open SQL connection to back up " + dbname + "." );
						LogInfo ( "BackUpSQLDBs", "Could not open SqlConnection for " + dbname + "." );
					}
				}
				catch (System.Exception ex)
				{
					LogBUStep ( String.Format ( "Could not back up {0} database: {1}", dbname, ex.Message ) );
					LogInfo ( "BackUpSQLDBs - " + dbname, ex.Message );
				}

				// At the end of the using block Dispose() calls Close().
			}
			return true;

		}

		// Back up SQL DBs using internal SQL Server backup functionality to holding directory.
		// AccountingDB
		// AviationDB
		// ConsolidatedDB
		// Master
		// Model
		// MSDB
		// FMMovementLog
		// FMArchive
		private bool BackUpSQLDBs ( )
		{
			LogBUStep ( "Backing up SQL databases." );



			SendBUCMessage(MessageEventArgs.MsgType.MSG_PROGRESS, "Backing up SQL database FuelsManagerDB", DateTime.Now);
			// FuelsManagerDB
			BackUpSQLDB ( "FuelsManagerDB" );

			SendBUCMessage(MessageEventArgs.MsgType.MSG_PROGRESS, "Backing up SQL database Master", DateTime.Now);
			// Master
			BackUpSQLDB ( "Master" );


			SendBUCMessage(MessageEventArgs.MsgType.MSG_PROGRESS, "Backing up SQL database Model", DateTime.Now);
			// Model
			BackUpSQLDB ( "Model" );


			SendBUCMessage(MessageEventArgs.MsgType.MSG_PROGRESS, "Backing up SQL database MSDB", DateTime.Now);
			// MSDB
			BackUpSQLDB ( "MSDB" );


			SendBUCMessage(MessageEventArgs.MsgType.MSG_PROGRESS, "Backing up SQL database FMMovementLog", DateTime.Now);
			// FMMovementLog
			BackUpSQLDB ( "FMMovementLog" );


			SendBUCMessage(MessageEventArgs.MsgType.MSG_PROGRESS, "Backing up SQL database FMArchive", DateTime.Now);
			// FMArchive
			BackUpSQLDB ( "FMArchive" );


			EventLog.WriteEntry ( "administrator was used for database backup." );

			return true;
		}

		private long CalculateSpecificTypeFilesSize ( string sDirectory, string sSearchPattern )
		{
			long lSize = 0;
			try
			{
				DirectoryInfo dir = new DirectoryInfo ( sDirectory );
				if (dir.Exists)
				{
					FileInfo[] files = dir.GetFiles ( sSearchPattern );
					foreach (FileInfo file in files)
					{
						lSize += file.Length;
					}
				}
			}
			catch (Exception ex)
			{
				LogInfo ( "CalculateSpecificTypeFilesSize", ex.Message );
			}
			//            LogInfo("CalculateSpecificTypeFilesSize", String.Format("{0} - {1} - {2}", lSize, sSearchPattern, sDirectory));
			return lSize;
		}

		private long CalculateDirectorySize ( string sDirectory, bool includeSubdirectories )
		{
			long lSize = 0;
			try
			{
				DirectoryInfo dir = new DirectoryInfo ( sDirectory );
				if (dir.Exists)
				{
					lSize = CalculateDirectorySize ( dir, true );
					//                    LogInfo("CalculateDirectorySize", String.Format("{0} - {1}", lSize, sDirectory));
				}
			}
			catch (Exception ex)
			{
				LogInfo ( "CalculateDirectorySize", ex.Message );
			}
			return lSize;
		}

		private long CalculateDirectorySize ( DirectoryInfo directory, bool includeSubdirectories )
		{
			long totalSize = 0;

			// Examine all contained files.
			FileInfo[] files = directory.GetFiles ( );
			foreach (FileInfo file in files)
			{
				totalSize += file.Length;
			}

			// Examine all contained directories.
			if (includeSubdirectories)
			{
				DirectoryInfo[] dirs = directory.GetDirectories ( );
				foreach (DirectoryInfo dir in dirs)
				{
					totalSize += CalculateDirectorySize ( dir, true );
				}
			}
			return totalSize;
		}

		private void CopySpecificTypeFiles ( string sourceDirectory, string targetDirectory, string sSearchPattern )
		{
			DirectoryInfo diSource = new DirectoryInfo ( sourceDirectory );
			if (!diSource.Exists) return;

			string sPathNoRoot = diSource.FullName;
			if (sPathNoRoot.Length > 3)
			{
				sPathNoRoot = diSource.FullName.Remove ( 0, 3 );

				// Preserve the directory structure.
				targetDirectory = Path.Combine ( targetDirectory, sPathNoRoot );//diSource.Name);
			}
			DirectoryInfo diTarget = new DirectoryInfo ( targetDirectory );

			if (!diTarget.Exists)
			{
				diTarget.Create ( );
			}

			// Copy all files.
			FileInfo[] files = diSource.GetFiles ( sSearchPattern );
			foreach (FileInfo file in files)
			{
				try
				{
					// Overwrite existing files.
					file.CopyTo ( Path.Combine ( diTarget.FullName, file.Name ), true );
				}
				catch { }
			}
		}

		private void CopyDirContents ( string sourceDirectory, string targetDirectory )
		{
			DirectoryInfo diSource = new DirectoryInfo ( sourceDirectory );
			if (!diSource.Exists) return;

			string sPathNoRoot = diSource.FullName;
			if (sPathNoRoot.Length > 3)
			{
				targetDirectory = Path.Combine(targetDirectory, diSource.FullName.Left(1));//diSource.Name);
				sPathNoRoot = diSource.FullName.Remove(0, 3);

				// Preserve the directory structure.
				targetDirectory = Path.Combine ( targetDirectory, sPathNoRoot );//diSource.Name);
			}
			DirectoryInfo diTarget = new DirectoryInfo ( targetDirectory );

			CopyDirectory ( diSource, diTarget );
		}

		private void CopyDirectory ( DirectoryInfo source, DirectoryInfo destination )
		{
			if (!source.Exists) return;
			if (!destination.Exists)
			{
				destination.Create ( );
			}

			// Copy all files.
			FileInfo[] files = source.GetFiles ( );
			foreach (FileInfo file in files)
			{
				try
				{
					// Overwrite existing files.
					file.CopyTo ( Path.Combine ( destination.FullName, file.Name ), true );
				}
				catch { }
			}

			// Process subdirectories.
			DirectoryInfo[] dirs = source.GetDirectories ( );
			foreach (DirectoryInfo dir in dirs)
			{
				// Get destination directory.
				string destinationDir = Path.Combine ( destination.FullName, dir.Name );

				// Call CopyDirectory() recursively.
				CopyDirectory ( dir, new DirectoryInfo ( destinationDir ) );
			}
		}

		private bool CopyDBFiles ( )
		{
			LogBUStep ( "Copying databases and files to a holding directory." );
			SendBUCMessage(MessageEventArgs.MsgType.MSG_PROGRESS, "Copying databases and files to a holding directory", DateTime.Now);

			try
			{
				// Determine the required disk space.

				long lRequiredSpace = 0;

				SendBUCMessage(MessageEventArgs.MsgType.MSG_PROGRESS, "Copying databases and files to a holding directory", DateTime.Now);
				// Check holding directory after SQL Databases backup process is done.
				if (sHoldingDir != null)
					lRequiredSpace += CalculateSpecificTypeFilesSize ( sHoldingDir, "*.bak" );

				SendBUCMessage(MessageEventArgs.MsgType.MSG_PROGRESS, "Copying databases and files to a holding directory", DateTime.Now);
				// Audit trace files (...\MSSQL\DATA\FMDAuditTrace*.trc).
				if (sSQLPath != null)
					lRequiredSpace += CalculateSpecificTypeFilesSize ( sSQLPath, "*.trc" );

				if (sFMProjectPath != null)
				{
					//                    lRequiredSpace += CalculateDirectorySize(sFMProjectPath, true);

					// Inventory Management real-time DB files.  Add the sizes of:
					// \Archives
					// \CM_Data
					// \DBBackUps
					// \Details
					// \Graphics
					// \Log
					// \Reports
					// \RTU
					// \Straps
					try
					{
						SendBUCMessage(MessageEventArgs.MsgType.MSG_PROGRESS, "Copying databases and files to a holding directory", DateTime.Now);
						string sPath = Path.Combine(sFMProjectPath, "Archives");
						lRequiredSpace += CalculateDirectorySize ( sPath, true );

						SendBUCMessage(MessageEventArgs.MsgType.MSG_PROGRESS, "Copying databases and files to a holding directory", DateTime.Now);
						sPath = Path.Combine(sFMProjectPath, "CM_Data");
						lRequiredSpace += CalculateDirectorySize ( sPath, true );

						SendBUCMessage(MessageEventArgs.MsgType.MSG_PROGRESS, "Copying databases and files to a holding directory", DateTime.Now);
						sPath = Path.Combine(sFMProjectPath, "DBBackUps");
						lRequiredSpace += CalculateDirectorySize ( sPath, true );

						SendBUCMessage(MessageEventArgs.MsgType.MSG_PROGRESS, "Copying databases and files to a holding directory", DateTime.Now);
						sPath = Path.Combine(sFMProjectPath, "Details");
						lRequiredSpace += CalculateDirectorySize ( sPath, true );

						SendBUCMessage(MessageEventArgs.MsgType.MSG_PROGRESS, "Copying databases and files to a holding directory", DateTime.Now);
						sPath = Path.Combine(sFMProjectPath, "Graphics");
						lRequiredSpace += CalculateDirectorySize ( sPath, true );

						SendBUCMessage(MessageEventArgs.MsgType.MSG_PROGRESS, "Copying databases and files to a holding directory", DateTime.Now);
						sPath = Path.Combine(sFMProjectPath, "Log");
						lRequiredSpace += CalculateDirectorySize ( sPath, true );

						SendBUCMessage(MessageEventArgs.MsgType.MSG_PROGRESS, "Copying databases and files to a holding directory", DateTime.Now);
						sPath = Path.Combine(sFMProjectPath, "Reports");
						lRequiredSpace += CalculateDirectorySize ( sPath, true );

						SendBUCMessage(MessageEventArgs.MsgType.MSG_PROGRESS, "Copying databases and files to a holding directory", DateTime.Now);
						sPath = Path.Combine(sFMProjectPath, "RTU");
						lRequiredSpace += CalculateDirectorySize ( sPath, true );

						SendBUCMessage(MessageEventArgs.MsgType.MSG_PROGRESS, "Copying databases and files to a holding directory", DateTime.Now);
						sPath = Path.Combine(sFMProjectPath, "Straps");
						lRequiredSpace += CalculateDirectorySize ( sPath, true );
					}
					catch { }
				}

				if (sADCFDCPath != null)
				{
					// Syn-Tech Systems ADC-FDC DB.  Add sizes of:
					// \Archive
					// \Reports
					// *.mdb

					try
					{
						SendBUCMessage(MessageEventArgs.MsgType.MSG_PROGRESS, "Copying databases and files to a holding directory", DateTime.Now);
						lRequiredSpace += CalculateSpecificTypeFilesSize(sADCFDCPath, "*.mdb");

						SendBUCMessage(MessageEventArgs.MsgType.MSG_PROGRESS, "Copying databases and files to a holding directory", DateTime.Now);
						string sPath = Path.Combine(sADCFDCPath, "Archive");
						lRequiredSpace += CalculateDirectorySize ( sPath, true );

						SendBUCMessage(MessageEventArgs.MsgType.MSG_PROGRESS, "Copying databases and files to a holding directory", DateTime.Now);
						sPath = Path.Combine(sADCFDCPath, "Reports");
						lRequiredSpace += CalculateDirectorySize ( sPath, true );
					}
					catch { }
				}

				if (sDoDFMAEPath != null)
				{
					// Syn-Tech Systems DodFM AE DBs (FuelMaster).  Add sizes of:
					// \Archive
					// \RawData
					// \Reports
					// \Transactions
					// *.mdb

					try
					{
						SendBUCMessage(MessageEventArgs.MsgType.MSG_PROGRESS, "Copying databases and files to a holding directory", DateTime.Now);
						lRequiredSpace += CalculateSpecificTypeFilesSize(sDoDFMAEPath, "*.mdb");

						SendBUCMessage(MessageEventArgs.MsgType.MSG_PROGRESS, "Copying databases and files to a holding directory", DateTime.Now);
						string sPath = Path.Combine(sDoDFMAEPath, "Archive");
						lRequiredSpace += CalculateDirectorySize ( sPath, true );

						SendBUCMessage(MessageEventArgs.MsgType.MSG_PROGRESS, "Copying databases and files to a holding directory", DateTime.Now);
						sPath = Path.Combine(sDoDFMAEPath, "RawData");
						lRequiredSpace += CalculateDirectorySize ( sPath, true );

						SendBUCMessage(MessageEventArgs.MsgType.MSG_PROGRESS, "Copying databases and files to a holding directory", DateTime.Now);
						sPath = Path.Combine(sDoDFMAEPath, "Reports");
						lRequiredSpace += CalculateDirectorySize ( sPath, true );

						SendBUCMessage(MessageEventArgs.MsgType.MSG_PROGRESS, "Copying databases and files to a holding directory", DateTime.Now);
						sPath = Path.Combine(sDoDFMAEPath, "Transactions");
						lRequiredSpace += CalculateDirectorySize ( sPath, true );
					}
					catch { }
				}

				// Additional files specified by the user.
				// Add the sizes of the directories.
				if (sPaths != null)
				{
					LogBUStep(string.Format("Copying additional files from {0} folders to a holding directory", sPaths.Length));
					SendBUCMessage(MessageEventArgs.MsgType.MSG_PROGRESS, string.Format("Copying additional files from {0} folders to a holding directory", sPaths.Length), DateTime.Now);
					for (int i = 0; i < sPaths.Length; i++)
					{
						if (!String.IsNullOrEmpty(sPaths[i]))
						{
							LogBUStep(string.Format("Copying additional files from folder {0} to a holding directory", sPaths[i]));
							SendBUCMessage(MessageEventArgs.MsgType.MSG_PROGRESS, string.Format("Copying additional files from folder {0} to a holding directory", sPaths[i]), DateTime.Now);
							lRequiredSpace += CalculateDirectorySize(sPaths[i], true);
						}
					}
				}
				else
				{
					LogBUStep("No additional files will be copied to a holding directory");
					SendBUCMessage(MessageEventArgs.MsgType.MSG_PROGRESS, "No additional files will be copied to a holding directory", DateTime.Now);

				}

				//                LogInfo("CopyDBFiles", lRequiredSpace.ToString());

				lRequiredSpace *= (long) 4;

				//                LogInfo("CopyDBFiles 4X", lRequiredSpace.ToString());

				// Check local drive available space.

				DirectoryInfo diExeDir = new FileInfo ( System.Reflection.Assembly.GetExecutingAssembly ( ).Location ).Directory;
				DriveInfo drive = new DriveInfo ( diExeDir.Root.FullName );

				//                LogInfo("CopyDBFiles", lRequiredSpace.ToString());
				//                LogInfo("CopyDBFiles", drive.AvailableFreeSpace.ToString());

				if (drive.AvailableFreeSpace < lRequiredSpace) // drive.AvailableFreeSpace could throw IOException
				{
					throw new Exception ( "Insufficient local disk space." );
				}

				// Copy from all directories.

				// Audit trace files (...\MSSQL\DATA\FMDAuditTrace*.trc).
				if (sSQLPath != null)
				{
					SendBUCMessage(MessageEventArgs.MsgType.MSG_PROGRESS, "Copying trc files.", DateTime.Now);
					CopySpecificTypeFiles(sSQLPath, sHoldingDir, "*.trc");
				}

				if (sFMProjectPath != null)
				{
					// Inventory Management real-time DB files.  Copy sub-directories:
					// \Archives
					// \CM_Data
					// \DBBackUps
					// \Details
					// \Graphics
					// \Log
					// \Reports
					// \RTU
					// \Straps

					//                    CopyDirContents(sFMProjectPath, sHoldingDir);
					try
					{
						SendBUCMessage(MessageEventArgs.MsgType.MSG_PROGRESS, "Copying Archives.", DateTime.Now);
						string sPath = Path.Combine(sFMProjectPath, "Archives");
						CopyDirContents ( sPath, sHoldingDir );

						SendBUCMessage(MessageEventArgs.MsgType.MSG_PROGRESS, "Copying CM_Data.", DateTime.Now);
						sPath = Path.Combine(sFMProjectPath, "CM_Data");
						CopyDirContents ( sPath, sHoldingDir );

						SendBUCMessage(MessageEventArgs.MsgType.MSG_PROGRESS, "Copying DBBackUps.", DateTime.Now);
						sPath = Path.Combine(sFMProjectPath, "DBBackUps");
						CopyDirContents ( sPath, sHoldingDir );

						SendBUCMessage(MessageEventArgs.MsgType.MSG_PROGRESS, "Copying Details.", DateTime.Now);
						sPath = Path.Combine(sFMProjectPath, "Details");
						CopyDirContents ( sPath, sHoldingDir );

						SendBUCMessage(MessageEventArgs.MsgType.MSG_PROGRESS, "Copying Graphics.", DateTime.Now);
						sPath = Path.Combine(sFMProjectPath, "Graphics");
						CopyDirContents ( sPath, sHoldingDir );

						SendBUCMessage(MessageEventArgs.MsgType.MSG_PROGRESS, "Copying Log.", DateTime.Now);
						sPath = Path.Combine(sFMProjectPath, "Log");
						CopyDirContents ( sPath, sHoldingDir );

						SendBUCMessage(MessageEventArgs.MsgType.MSG_PROGRESS, "Copying Reports.", DateTime.Now);
						sPath = Path.Combine(sFMProjectPath, "Reports");
						CopyDirContents ( sPath, sHoldingDir );

						SendBUCMessage(MessageEventArgs.MsgType.MSG_PROGRESS, "Copying RTU.", DateTime.Now);
						sPath = Path.Combine(sFMProjectPath, "RTU");
						CopyDirContents ( sPath, sHoldingDir );

						SendBUCMessage(MessageEventArgs.MsgType.MSG_PROGRESS, "Copying Straps.", DateTime.Now);
						sPath = Path.Combine(sFMProjectPath, "Straps");
						CopyDirContents ( sPath, sHoldingDir );
					}
					catch { }
				}

				if (sADCFDCPath != null)
				{
					// Syn-Tech Systems ADC-FDC DB.  Copy subdirectories and mdb files:
					// \Archive
					// \Reports
					// *.mdb
					try
					{
						SendBUCMessage(MessageEventArgs.MsgType.MSG_PROGRESS, "Copying  ADC-FDC DB mdb.", DateTime.Now);
						CopySpecificTypeFiles(sADCFDCPath, sHoldingDir, "*.mdb");

						SendBUCMessage(MessageEventArgs.MsgType.MSG_PROGRESS, "Copying  ADC-FDC DB Archive.", DateTime.Now);
						string sPath = Path.Combine(sADCFDCPath, "Archive");
						CopyDirContents ( sPath, sHoldingDir );

						SendBUCMessage(MessageEventArgs.MsgType.MSG_PROGRESS, "Copying  ADC-FDC DB reports.", DateTime.Now);
						sPath = Path.Combine(sADCFDCPath, "Reports");
						CopyDirContents ( sPath, sHoldingDir );
					}
					catch { }
				}

				if (sDoDFMAEPath != null)
				{
					// Syn-Tech Systems DodFM AE DBs (FuelMaster).  Copy:
					// \Archive
					// \RawData
					// \Reports
					// \Transactions
					// *.mdb
					try
					{
						SendBUCMessage(MessageEventArgs.MsgType.MSG_PROGRESS, "Copying  DodFM AE DB mdb.", DateTime.Now);
						CopySpecificTypeFiles(sDoDFMAEPath, sHoldingDir, "*.mdb");

						SendBUCMessage(MessageEventArgs.MsgType.MSG_PROGRESS, "Copying  DodFM AE DB Archive.", DateTime.Now);
						string sPath = Path.Combine(sDoDFMAEPath, "Archive");
						CopyDirContents ( sPath, sHoldingDir );

						SendBUCMessage(MessageEventArgs.MsgType.MSG_PROGRESS, "Copying  DodFM AE DB RawData.", DateTime.Now);
						sPath = Path.Combine(sDoDFMAEPath, "RawData");
						CopyDirContents ( sPath, sHoldingDir );

						SendBUCMessage(MessageEventArgs.MsgType.MSG_PROGRESS, "Copying  DodFM AE DB Reports.", DateTime.Now);
						sPath = Path.Combine(sDoDFMAEPath, "Reports");
						CopyDirContents ( sPath, sHoldingDir );

						SendBUCMessage(MessageEventArgs.MsgType.MSG_PROGRESS, "Copying  DodFM AE DB Transactions.", DateTime.Now);
						sPath = Path.Combine(sDoDFMAEPath, "Transactions");
						CopyDirContents ( sPath, sHoldingDir );
					}
					catch { }
				}

				// Copy additional files.
				if (sPaths != null)
				{
					for (int i = 0; i < sPaths.Length; i++)
					{
						if (!String.IsNullOrEmpty(sPaths[i]))
						{
							SendBUCMessage(MessageEventArgs.MsgType.MSG_PROGRESS, "Copying  additional files.", DateTime.Now);
							CopyDirContents(sPaths[i], sHoldingDir);
						}
					}
				}
			}

			catch (System.IO.IOException exIO)
			{
				// Drive not ready.
				string sErr = String.Format ( "Could not copy files: {0}", exIO.Message );
				throw new Exception ( sErr, exIO );
			}
			catch (System.Exception ex)
			{
				string sErr = String.Format ( "Could not copy files: {0}", ex.Message );
				throw new Exception ( sErr, ex );
			}
			return true;
		}

		private bool ZipFiles ( )
		{
			// Compress and package files according to Zip standard.
			LogBUStep("Compressing databases and files into a zip file.");
			SendBUCMessage(MessageEventArgs.MsgType.MSG_PROGRESS, "Compressing databases and files into a zip file.", DateTime.Now);

			bool bDeleteTempDir = false;

			try
			{
				// This shouldn't happen, but just in case.
				if (!Directory.Exists ( sHoldingDir ))
				{
					throw new Exception ( "Database holding directory does not exist." );
				}

				string sTempZipDir;
				string sTime = DateTime.Now.ToString ( "yyyyMMdd-HHmmss" );
				FileInfo fiExe = new FileInfo ( System.Reflection.Assembly.GetExecutingAssembly ( ).Location );

				try
				{
					// Create temp zip directory.

					//                    DirectoryInfo diExeDir = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).Directory;
					DirectoryInfo diExeDir = fiExe.Directory;
					string sDir = String.Format ( "BackupTempDir\\TempZipDir{0}", sTime );
					sTempZipDir = Path.Combine ( diExeDir.FullName, sDir );

					//                        if (!Directory.Exists(sTempZipDir))
					CreateDirectory ( sTempZipDir );
					bDeleteTempDir = true;
				}
				catch
				{
					// If failed to create directory, use EXE directory.
					sTempZipDir = fiExe.DirectoryName;
					//                    sTempZipDir = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).DirectoryName;
				}

				// Zip filename consists of machine name, date, and time.
				string sFileName = String.Format ( "{0}-{1}.zip", Environment.MachineName, sTime );
				string sFullPath = System.IO.Path.Combine ( sTempZipDir, sFileName );


				// Create zip in temp zip dir.
				FastZip zip = new FastZip ( );

				if (!string.IsNullOrEmpty(certificateName))
					zip.Password = Guid.NewGuid().ToString();

				zip.CreateZip ( sFullPath, sHoldingDir, true, null );

				string sTargetFullPath;

				// Copy zip file to zip target directory.
				SendBUCMessage(MessageEventArgs.MsgType.MSG_PROGRESS, "Copy zip file to zip target directory.", DateTime.Now);
				try
				{
					// Create the zip target directory if it does not exist.
					if (!Directory.Exists ( sZipFileLocation ))
					{
						Directory.CreateDirectory ( sZipFileLocation );
					}

					sTargetFullPath = System.IO.Path.Combine ( sZipFileLocation, sFileName );

					// Copy from temp zip dir to target zip dir.
					// Should throw exception if not enough space.
					File.Copy ( sFullPath, sTargetFullPath, true );
					EventLog.WriteEntry(string.Format("zip file path: {0}", sTargetFullPath), EventLogEntryType.Information); // Log address and binding info.

					string sPasswordFileFullPath = sTargetFullPath + ".password";
					EncryptAndSignZipFile(sTargetFullPath, zip.Password);
				}
				catch (Exception ex)
				{
					// Delete holding directory and its contents.
					try
					{
						DeleteDirectory ( sHoldingDir );
					}
					catch (Exception ex1)
					{
						LogInfo ( "ZipFiles", ex1.Message );
					}
					finally
					{
						sHoldingDir = null;
					}

					string sErr = String.Format ( "{0}: {1}  {2} {3}",
												"Could not copy zip file to target directory",
												ex.Message,
												"Zip file saved in",
												sFullPath );
					throw new CreateDirectoryException ( sErr, ex );
				}

				// If temp zip directory is not EXE directory, delete temp zip directory.
				if (bDeleteTempDir)
				{
					try
					{
						Directory.Delete ( sTempZipDir, true );
					}
					catch (Exception ex0)
					{
						LogInfo ( "ZipFiles", ex0.Message );
					}
				}

				// Log completion of zip file creation.
				DateTime dt = DateTime.Now;
				string str = String.Format ( "Created zip file: {0}", sTargetFullPath );
				LogBUStep ( str, dt );
				LogInfo ( "ZipFiles", str, dt );
			}

			catch (CreateDirectoryException ex)
			{
				throw ( ex );
			}
			catch (System.Exception ex)
			{
				string sErr = String.Format ( "Could not create zip file: {0}", ex.Message );
				throw new Exception ( sErr, ex );
			}
			return true;
		}

		//If certicate name is configured in database tblConfigurationSetting table, encrypt file.
		private void EncryptAndSignZipFile(string sTargetFullPath, string password)
		{

			if (!string.IsNullOrEmpty(certificateName) &&
				!string.IsNullOrEmpty(sTargetFullPath) &&
				!string.IsNullOrEmpty(password))
			{
				string sEncryptedSignedTargetFullPath = sTargetFullPath + ".vef";
				var encryptor = new FMBusinessObjects.UtilityObjects.Encryption(Encoding.UTF8);
				try
				{
					Encoding encoding = Encoding.UTF8;
					byte[] valueToBeEncrypted = encoding.GetBytes(password);

					RSACrypt cryptor = new RSACrypt();
					using (RSACertificate theCert = new RSACertificate(certificateName))
					{
						if (theCert.Certificate == null)
						{
							string msg = string.Format("Certificate with name {0} not found. Could not extract password file.", certificateName);
							LogInfo("ZipFiles", "Certificate error: " + msg);
							LogBUStep("Certificate error: " + msg, DateTime.Now);
							EventLog.WriteEntry("Encryption.EncryptKeyAsymmetric: " + msg);
							return;
						}
						if (theCert.Certificate.PrivateKey == null)
						{
							string msg = "Certificate missing a private key. Could not extract password file.";
							LogInfo("ZipFiles", "Certificate error: " + msg);
							LogBUStep("Certificate error: " + msg, DateTime.Now);
							EventLog.WriteEntry("Encryption.EncryptKeyAsymmetric: " + msg);
							return;
						}

						//Encrypt password
						byte[] encryptedPassword = cryptor.Encrypt(valueToBeEncrypted, theCert);
							
						//Sign password. Append signature to encrypted password
						var p = new RSACryptoServiceProvider();
						RSAParameters rp = new RSAParameters();

		
						rp = ((RSACryptoServiceProvider)theCert.Certificate.PrivateKey).ExportParameters(true);
						p.ImportParameters(rp);
						p.PersistKeyInCsp = false;

						SHA256CryptoServiceProvider hashAlg = new SHA256CryptoServiceProvider();


						//int hashSize = hashAlg.HashSize >> 3;//hash size in bytes
						byte[] signature = p.SignData(encryptedPassword, hashAlg);
						if (!p.VerifyData(encryptedPassword, new SHA256CryptoServiceProvider(), signature))
						{

							EventLog.WriteEntry("Unable to encrypt and sign zip file. Issue with the certificate.", EventLogEntryType.Information); // Log address and binding info.
							LogBUStep("Unable to encrypt and sign zip file. Issue with the certificate.", DateTime.Now);
							LogInfo("ZipFiles:", "Unable to encrypt and sign zip file. Issue with the certificate.");
							return;
						}
						FileStream fs = new FileStream(sEncryptedSignedTargetFullPath, FileMode.CreateNew);

						fs.Write(signature, 0, 256);

						fs.Write(encryptedPassword, 0, 256);
						FileStream plainFs = new FileStream(sTargetFullPath, FileMode.Open);
			
						byte []buf = new byte[256];
						int l = 0;
						while ((l = plainFs.Read(buf, 0, 256)) > 0)
						{
							fs.Write(buf, 0, l);
						}
						fs.Close();

						plainFs.Close();

						File.Delete(sTargetFullPath);

						EventLog.WriteEntry("zip file encrypted and signed.", EventLogEntryType.Information); // Log address and binding info.
						LogBUStep("zip file encrypted and signed.", DateTime.Now);
						LogInfo("ZipFiles:", "zip file encrypted and signed.");
						
					}
				}
				catch (Exception ex)
				{
					string msg = ex.Message;
					LogInfo("ZipFiles", "Encryption and signature error: " + ex.Message);
					LogBUStep("Encryption and signature error: " + ex.Message, DateTime.Now);
					EventLog.WriteEntry("Encryption.EncryptKeyAsymmetric: " + msg);
				}


			}
			else
			{
				if (string.IsNullOrEmpty(certificateName))
				{

					LogBUStep("Certificate name missing", DateTime.Now);
					LogInfo("ZipFiles", "Certificate name missing");
				}
				if (string.IsNullOrEmpty(sTargetFullPath))
				{
					LogBUStep("Zip file name missing", DateTime.Now);
					LogInfo("ZipFiles", "Zip file name missing");

				}
				if (string.IsNullOrEmpty(password))
				{
					LogBUStep("Password missing", DateTime.Now);
					LogInfo("ZipFiles", "Password missing");

				}

			}
		}
		//If certicate name is configured in database tblConfigurationSetting table, encrypt file.
		//private void EncryptPassword(string sTargetFullPath, string password)
		//{
		//	if (!string.IsNullOrEmpty(certificateName) && 
		//		!string.IsNullOrEmpty(sTargetFullPath) && 
		//		!string.IsNullOrEmpty(password))
		//	{
		//		var encryptor = new FMBusinessObjects.UtilityObjects.Encryption(Encoding.UTF8);
		//		try
		//		{
		//			Encoding encoding = Encoding.UTF8;
		//			byte[] valueToBeEncrypted = encoding.GetBytes(password);

		//			RSACrypt cryptor = new RSACrypt();
		//			using (RSACertificate theCert = new RSACertificate(certificateName))
		//			{
		//				if (theCert.Certificate != null)
		//				{
		//					//Encrypt password
		//					byte[] encryptedPassword = cryptor.Encrypt(valueToBeEncrypted, theCert);
		//					var p = new RSACryptoServiceProvider();
		//					RSAParameters rp = new RSAParameters();
		//					rp = ((RSACryptoServiceProvider)theCert.Certificate.PrivateKey).ExportParameters(true);
		//					p.ImportParameters(rp);
		//					p.PersistKeyInCsp = false;

		//					//Sign password. Append signature to encrypted password
		//					SHA256CryptoServiceProvider hashAlg = new SHA256CryptoServiceProvider();
		//					int hashSize = hashAlg.HashSize >> 3;//hash size in bytes
		//					byte[] signature = p.SignData(encryptedPassword, hashAlg);
		//					//string len = signature.Length.PadLeft0(4);
		//					//byte[] lbyte = encoding.GetBytes(len);

		//					FileStream fs = new FileStream(sTargetFullPath, FileMode.CreateNew);
		//					//fs.Write(lbyte, 0, lbyte.Length);
		//					fs.Write(signature, 0, 256);//signature.Length);

		//					//len = encryptedPassword.Length.PadLeft0(4);
		//					//lbyte = encoding.GetBytes(len);

		//					//fs.Write(lbyte, 0, lbyte.Length);

		//					fs.Write(encryptedPassword, 0, 256);//encryptedPassword.Length);
		//					fs.Close();
		//					EventLog.WriteEntry("zip file encrypted", EventLogEntryType.Information); // Log address and binding info.
		//				}
		//			}
		//		}
		//		catch (Exception ex)
		//		{
		//			string msg = ex.Message;
		//			EventLog.WriteEntry("Encryption.EncryptKeyAsymmetric: " + msg);
		//		}


		//	}
		//}

		#endregion // Private Methods

		#region Multithread

		// ==================================================================================================
		// < BUC APPLICATION AS REMOTE SERVER >

		private void SendBUCMessage ( MessageEventArgs.MsgType msgType, string sMsg, DateTime dt )
		{
			progressMessage = sMsg;
			if (roBUC != null)
			{
				// Asynchronous remote call to BUC.
				AsyncCallback callback = new AsyncCallback ( this.SendBUCMessageCallBack );
				SendBUCMessageDelegate del = new SendBUCMessageDelegate ( roBUC.UpdateMessage );
				IAsyncResult ar = del.BeginInvoke ( msgType, sMsg, dt, callback, this );
			}
		}

		// Callback method that is called when SendBUCMessageDelegate completes its async call.
		private void SendBUCMessageCallBack ( IAsyncResult ar )
		{
			// Obtains the last parameter of the delegate call.
			BUService service = (BUService) ar.AsyncState;
			// Get the delegate object on which the asynchronous call was invoked.
			SendBUCMessageDelegate del = (SendBUCMessageDelegate) ( (AsyncResult) ar ).AsyncDelegate;

			try
			{
				del.EndInvoke ( ar ); // No return value.
			}
			catch (Exception ex)
			{
				// BUC Server is not available.

				service.LogInfo ( "SendBUCMessageCallBack", ex.Message );
			}
		}
		// ==================================================================================================



		// ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
		// < BU SERVICE AS SERVER >

		// Handling messages from BUC.
		void roBU_MessageToBUEvent ( object sender, MessageToBUEventArgs e )
		{
			if (IsRunning)
			{
				LogInfo ( "roBU_MessageToBUEvent", "A session is already in progress." );
				return;
			}

			switch (e.MessageType)
			{
				case MessageToBUEventArgs.MsgType.MSG_BACKUPNOW:
					LogInfo ( "roBU_MessageToBUEvent", "Back up now." );
					eventRun.Set ( );
					break;

				case MessageToBUEventArgs.MsgType.MSG_UPDATECONFIG:
					ReadScheduledTime ( );
					// Create a timer to run TimerProc() at specific time.
					SetTimer ( );

					// This timer setting depends on the backup process timer, so,
					// rescheduling the backup process also requires a call to this method.
					SetCreateLogTimer ( );

					// This creates a log file if there isn't one.
					OpenBULog ( DateTime.Now );
					break;
			}
		}
		// ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

		// Callback method that runs on a threadpool thread.
		private void CreateLogTimerProc ( object state )
		{
			BUService service = (BUService) state;

			service.LogInfo ( "CreateLogTimerProc", "." );

			if (service.IsRunning)
			{
				service.LogInfo("CreateLogTimerProc", "A backup session is in progress.");
				return;
			}
			// This creates a log file if there isn't one.
			OpenBULog ( DateTime.Now );
		}
		// Callback method that runs on a threadpool thread.
		private void StatusTimerProc(object state)
		{
			// The callback method executed by the timer is reentrant, because
			// it is called on ThreadPool threads.  The callback can be executed
			// simultaneously on two thread pool threads if the timer interval
			// is less than the time required to execute the callback, or if
			// all thread pool threads are in use and the callback is queued multiple times.

			BUService service = (BUService)state;

			if (service.IsRunning)
			{
				SendBUCMessage(MessageEventArgs.MsgType.MSG_PROGRESS, service.progressMessage, DateTime.Now);
				return;
			}


		}
		// Callback method that runs on a threadpool thread.
		private void TimerProc ( object state )
		{
			// The callback method executed by the timer is reentrant, because
			// it is called on ThreadPool threads.  The callback can be executed
			// simultaneously on two thread pool threads if the timer interval
			// is less than the time required to execute the callback, or if
			// all thread pool threads are in use and the callback is queued multiple times.

			BUService service = (BUService) state;

			service.LogInfo ( "TimerProc", "Entered." );

			if (service.IsRunning)
			{
				service.LogInfo("TimerProc", "A session is already in progress.");
				return;
			}

			eventRun.Set ( );
		}

		// Callback method that runs on a threadpool thread.
		private void CheckKeyTimerProc ( object state )
		{
			BUService service = (BUService) state;

			if (IsSecurityKeyPresent ( )) return;

			if (timerCheckKey != null)
			{
				timerCheckKey.Dispose ( );
				timerCheckKey = null;
			}
			// Request to stop backup service immediately.
			service.Stop ( );
		}

		private bool IsSecurityKeyPresent ( )
		{
			ushort usProgramVersion = 0;

			try
			{
                usProgramVersion = FMChannelHelper.MakeCall<IHardwareKey, ushort>(x => x.GetProgramVersionLIN());
                if(usProgramVersion == 0)
                    usProgramVersion = FMChannelHelper.MakeCall<IHardwareKey, ushort>(x => x.GetProgramVersion());
			}
			catch (Exception e)
			{
				DateTime dt = DateTime.Now;
				string str = e.Message;
				LogBUStep ( str, dt );
				LogInfo ( "IsSecurityKeyPresent", str, dt );
				EventLog.WriteEntry ( str, EventLogEntryType.Error ); // Log failure with reason.
				return false;
			}

			if (usProgramVersion > 0)
			{
				// the following check is not correct. There are dependencies between versions like the volumecorrection.dll
				// there has already beed one abort in the field because of this check but I am not allowed to change it
				// and have been told to leave it the way it is.
				if (usProgramVersion < FUELSMANAGER_MINIMUM_SENTINEL_REVISION)
				{
					DateTime dt = DateTime.Now;
					string str = "Installed Hardware key is not for this version of FuelsManager.";
					LogBUStep ( str, dt );
					LogInfo ( "IsSecurityKeyPresent", str, dt );
					EventLog.WriteEntry ( str, EventLogEntryType.Error ); // Log failure with reason.
					return false;
				}
			}
			else
			{
				DateTime dt = DateTime.Now;
				string str = "Hardware key not found.";
				LogBUStep ( str, dt );
				LogInfo ( "IsSecurityKeyPresent", str, dt );
				EventLog.WriteEntry ( str, EventLogEntryType.Error ); // Log failure with reason.
				return false;
			}
			return true;
		}

		private void DeleteDirectory ( string sDir )
		{
			DirectoryInfo diDir = new DirectoryInfo ( sDir );

			foreach (DirectoryInfo subDirInfo in diDir.GetDirectories ( ))
			{
				DeleteDirectory ( subDirInfo.FullName );
			}

			foreach (FileInfo fileInfo in diDir.GetFiles ( ))
			{
				try
				{
					if (( fileInfo.Attributes & FileAttributes.ReadOnly ) != 0)
					{
						fileInfo.Attributes = fileInfo.Attributes & ~FileAttributes.ReadOnly;
					}
					fileInfo.Delete ( );
				}
				catch { }
			}
			try
			{
				diDir.Attributes = diDir.Attributes & ~FileAttributes.ReadOnly;
				diDir.Delete ( true );
			}
			catch { }
		}

		private void ReportFailedBackup ( BUService service, string sReason, EventLogEntryType eventType )
		{
			// Asynchronous remote call to BUC.
			DateTime dt = DateTime.Now;

			string str = "Backup operation incomplete.";
			string str1 = String.Format ( "{0}  {1}", str, sReason );
			service.SendBUCMessage ( MessageEventArgs.MsgType.MSG_FAIL, str, dt );
			service.LogBUStep ( str1, dt );
			service.LogInfo ( "RunBackup", str1, dt );

			service.EventLog.WriteEntry ( str1, eventType ); // Log failure with reason.
		}

		// Thread method.
		private static void RunBackup ( object obj )
		{
			BUService service = (BUService) obj;
			service.progressMessage = "";

			while (!bTerminate)
			{
				string str;
				try
				{
					// Wait here til receive run backup request.
					eventRun.WaitOne ( );
					// Prevent another run while executing the current backup process.
					service.IsRunning = true;

					// Request to terminate thread loop may be initiated by OnStop or OnShutdown.
					if (bTerminate) break;

					service.ReadConfiguration ( );

					// Asynchronous remote call to BUC.
					DateTime dt = DateTime.Now;
					str = "Backup operation started.";
					service.LogBUStep ( str, dt );
					service.LogInfo ( "RunBackup", str, dt );
					service.SendBUCMessage ( MessageEventArgs.MsgType.MSG_STARTED, str, dt );

					try
					{
						service.SendBUCMessage(MessageEventArgs.MsgType.MSG_PROGRESS, "cleanup old temp directories", DateTime.Now);
						//cleanup old temp directories.
						DirectoryInfo diExeDir = new FileInfo ( System.Reflection.Assembly.GetExecutingAssembly ( ).Location ).Directory;
						try
						{
							string sDir = Path.Combine ( diExeDir.FullName, "BackupTempDir" );
							Directory.Delete ( sDir, true );
						}
						catch (Exception ex)
						{
							service.LogBUStep ( ex.Message, dt );
							service.LogInfo ( "RunBackup", ex.Message, dt );
						}

						service.SendBUCMessage(MessageEventArgs.MsgType.MSG_PROGRESS, "Create holding directory", DateTime.Now);
						try
						{
							// Create holding directory.
							string sDir = String.Format ( "BackupTempDir\\TempFileDir{0}", DateTime.Now.ToString ( "yyyyMMdd-HHmmss" ) );
							service.sHoldingDir = Path.Combine ( diExeDir.FullName, sDir );

							if (!Directory.Exists ( service.sHoldingDir ))
							{
								CreateDirectory ( service.sHoldingDir );
							}
						}
						catch (Exception ex)
						{
							service.ReportFailedBackup ( service, ex.Message, EventLogEntryType.Error );
							service.IsRunning = false;
							continue;
						}
					}
					catch
					{
					}

					try
					{
						service.BackUpSQLDBs ( );
					}
					catch (System.Exception ex)
					{
						service.ReportFailedBackup ( service, ex.Message, EventLogEntryType.Error );
						service.IsRunning = false;
						continue;
					}

					try
					{
						service.CopyDBFiles ( );
					}
					catch (System.Exception ex)
					{
						service.ReportFailedBackup ( service, ex.Message, EventLogEntryType.Error );
						service.IsRunning = false;
						continue;
					}

					try
					{
						service.ZipFiles ( );
					}
					catch (System.Exception ex)
					{
						service.ReportFailedBackup ( service, ex.Message, EventLogEntryType.Warning );
						service.IsRunning = false;
						continue;
					}


					// Delete holding directory.
					try
					{
						service.DeleteDirectory ( service.sHoldingDir );
					}
					catch (Exception ex)
					{
						service.LogInfo ( "RunBackup", ex.Message );
					}
					finally
					{
						service.sHoldingDir = null;
						dt = DateTime.Now;
						str = "Backup operation completed.";
						service.LogBUStep(str, dt);
						service.LogInfo("RunBackup", str, dt);
						service.SendBUCMessage(MessageEventArgs.MsgType.MSG_COMPLETE, str, dt);

						service.EventLog.WriteEntry(str); // Log final success.

					}

					// Normal backup operation completed.
					service.IsRunning = false;
				}
				catch (ThreadAbortException ex)
				{
					str = "Received abort Backup Utility request.  Terminating thread.\n" + ex.Message;
					service.LogInfo ( "RunBackup", str );
					service.LogBUStep ( str );
					service.IsRunning = false;
				}
			}
			service.LogInfo ( "RunBackup", "Terminating." );
		}

		#endregion // Multithread
		private static void CreateDirectory ( string dirName )
		{
			Directory.CreateDirectory ( dirName );
			DirectoryInfo dInfo = new DirectoryInfo ( dirName );
			System.Security.AccessControl.DirectorySecurity dSecurity = dInfo.GetAccessControl ( );
			dSecurity.AddAccessRule ( new System.Security.AccessControl.FileSystemAccessRule ( "NT AUTHORITY\\NETWORK SERVICE",
															 System.Security.AccessControl.FileSystemRights.WriteData,
															 System.Security.AccessControl.AccessControlType.Allow ) );
			dInfo.SetAccessControl ( dSecurity );

		}

		private void SetStatusTimer()
		{
			if (timerStatus != null)
			{
				timerStatus.Dispose();
				timerStatus = null;
			}
			timerStatus = new Timer(new TimerCallback(StatusTimerProc), this, 1000, 1000);
		}


		private void SetCheckKeyTimer ( )
		{
			if (timerCheckKey != null)
			{
				timerCheckKey.Dispose ( );
				timerCheckKey = null;
			}
			timerCheckKey = new Timer ( new TimerCallback ( CheckKeyTimerProc ), this, 60000, 60000 );
		}

		// Set a timer to create a log file every midnight.
		// This timer setting depends on the backup process timer, so,
		// rescheduling the backup process also requires a call to this method.
		private void SetCreateLogTimer ( )
		{
			if (timerCreateLog != null)
			{
				timerCreateLog.Dispose ( );
				timerCreateLog = null;
			}

			DateTime dtNextCreateLogTime;// = DateTime.Today.AddDays(1).AddSeconds(5); // Tomorrow 12:00:05 AM

			TimeSpan ts235500 = new TimeSpan ( 23, 55, 00 );

			if (tsTimeOfDay >= ts235500 || tsTimeOfDay <= TimeSpan.FromMinutes ( 1 ))
			{
				// Backup process is scheduled to start at/after 11:55 PM or at/before 12:01 AM.

				dtNextCreateLogTime = DateTime.Today.AddDays ( 1 ).AddMinutes ( 5 ); // Tomorrow 12:05 AM
			}
			else
				dtNextCreateLogTime = DateTime.Today.AddDays ( 1 ).AddSeconds ( 5 ); // Tomorrow 12:00:05 AM

			// Calculate the difference between the next execution time and the current time.
			TimeSpan tsWait = dtNextCreateLogTime - DateTime.Now;

			timerCreateLog = new Timer ( new TimerCallback ( CreateLogTimerProc ), this, tsWait, TimeSpan.FromDays ( 1 ) );
		}

		// Create a timer to run TimerProc() at specific time.
		private void SetTimer ( )
		{
			if (timerBU != null)
			{
				timerBU.Dispose ( );
				timerBU = null;
			}
			DateTime today = DateTime.Today;
			DateTime now = DateTime.Now;
			DateTime dtStart = today + tsTimeOfDay;

			if (dtStart <= DateTime.Now) dtStart += TimeSpan.FromDays ( 1 );

			// Calculate the difference between the specified execution time and the current time.
			TimeSpan tsWait = dtStart - now;

			timerBU = new Timer ( new TimerCallback ( TimerProc ), this, tsWait, TimeSpan.FromDays ( 1 ) );

		}

		protected override void OnStart ( string[] args )
		{
			// OnStart must return within 30 seconds.

			LogInfo ( "OnStart", "Entered." );

            LoginToFuelsManager();
            ReadConfiguration();

			// This creates a log file if there isn't one.
			OpenBULog ( DateTime.Now );

			if (!IsSecurityKeyPresent ( ))
			{
				Stop ( );
				return;
			}

			// =================================================================================================
			// < BUC APPLICATION REMOTE SERVER RELATED CODE >
			try
			{
				// Register a TCP channel.
				ChannelServices.RegisterChannel ( new TcpChannel ( ), false );

				roBUC = (FMBUCRemote) Activator.GetObject (
										  typeof ( FMBUCRemote ),
										  "tcp://localhost:50905/FMBUCRemote" );
			}
			catch (Exception ex)
			{
				LogInfo ( "OnStart", ex.Message );
				EventLog.WriteEntry ( ex.Message );
			}
			// =================================================================================================


			// ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
			// < BU SERVICE REMOTE SERVER RELATED CODE >
			try
			{
				// Register a TCP channel.

				// Default constructor of the channel creates a channel with a name "tcp", better to use a new name.
				System.Collections.IDictionary properties = new System.Collections.Hashtable ( );
				properties["port"] = 50906;
				properties["name"] = "BUTcp";

				ChannelServices.RegisterChannel ( new TcpChannel ( properties, null, null ), false );

				// Create a remotable object and register it with the remoting service.
				roBU = new FMBURemote ( );
				if (roBU == null)
				{
					LogInfo ( "OnStart", "Null Remotable object." );
					EventLog.WriteEntry ( "Null Remotable object." );
					return;
				}
				else
				{
					ObjRef orFMBURemote = RemotingServices.Marshal ( roBU, "FMBURemote" );

					// Subscribe to message event raised by BU remote object.
					roBU.MessageToBUEvent += new FMBURemote.MessageToBUEventHandler ( roBU_MessageToBUEvent );
				}
			}
			catch (Exception ex)
			{
				LogInfo ( "OnStart", ex.Message );
				EventLog.WriteEntry ( ex.Message );
			}
			// ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::


			bTerminate = false;
			threadBackup = null;

			// Create a new thread and pass this service object to it.
			threadBackup = new Thread ( RunBackup );
			if (threadBackup != null)
			{
				ReadScheduledTime ( );

				threadBackup.Start ( this );

				// Create timers to run timer procedures at specific times.
				SetTimer ( );
				SetCheckKeyTimer ( );
				SetCreateLogTimer();
				SetStatusTimer();
			}
		}

		protected override void OnStop ( )
		{
			LogInfo ( "OnStop", "Received Stop Service request." );
			if (timerStatus != null)
			{
				timerStatus.Dispose();
				timerStatus = null;
			}

			if (timerCheckKey != null)
			{
				timerCheckKey.Dispose ( );
				timerCheckKey = null;
			}

			if (timerBU != null)
			{
				timerBU.Dispose ( );
				timerBU = null;
			}

			if (timerCreateLog != null)
			{
				timerCreateLog.Dispose ( );
				timerCreateLog = null;
			}

			this.RequestAdditionalTime ( 6000 );

			bTerminate = true;
			eventRun.Set ( );
			if (threadBackup != null && !threadBackup.Join ( 3000 ))
			{
				LogInfo ( "OnStop", "Timed out waiting for thread to exit." );
				threadBackup.Abort ( ); //Abort(this);
				threadBackup.Join ( 2000 );
			}

		}

		protected override void OnShutdown ( )
		{
			LogInfo ( "OnShutdown", "Received Shutdown Service request." );

			if (timerStatus != null)
			{
				timerStatus.Dispose();
				timerStatus = null;
			}

			if (timerCheckKey != null)
			{
				timerCheckKey.Dispose ( );
				timerCheckKey = null;
			}

			if (timerBU != null)
			{
				timerBU.Dispose ( );
				timerBU = null;
			}

			if (timerCreateLog != null)
			{
				timerCreateLog.Dispose ( );
				timerCreateLog = null;
			}

			this.RequestAdditionalTime ( 6000 );

			bTerminate = true;
			eventRun.Set ( );
			if (threadBackup != null && !threadBackup.Join ( 3000 ))
			{
				LogInfo ( "OnShutdown", "Timed out waiting for thread to exit." );
				threadBackup.Abort ( ); //Abort(this);
				threadBackup.Join ( 2000 );
			}

		}
	}
}

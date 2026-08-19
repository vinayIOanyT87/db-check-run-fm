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

using Microsoft.Win32; // RegistryKey
using System.IO;
using System.Threading;
using System.Runtime.Remoting; // For ObjRef
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Messaging; // For AsyncResult
using FMBUC;
using FMBU;
using System.Data.SqlClient;
using FMUTILLib;

using Ionic.Utils.Zip;

namespace FMBackupUtility
{
    public partial class BUService : ServiceBase
    {
        const int FUELSMANAGER_SENTINEL_REVISION = 600;
        const int DEVELOPER_KEY = 9999;

        private static AutoResetEvent eventRun = new AutoResetEvent(false);
        private bool bRunning;
        private bool bLogInfo;

//        DateTime dtStartTime;
        TimeSpan tsTimeOfDay;
        string sLogFileLocation;
        string sZipFileLocation;
        string[] sPaths;

        string sCurrentLog;

        string sSQLPath;       // Audit trace files (FMDAuditTrace*.trc).
        string sFMProjectPath; // Inventory Management real-time DB files.
        string sADCFDCPath;    // Syn-Tech Systems ADC-FDC DB.
        string sDoDFMAEPath;   // Syn-Tech Systems DodFM AE DBs (FuelMaster).
        
        string sHoldingDir;

        Timer timerBU;
        Timer timerCreateLog;
        Timer timerCheckKey;
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
        private delegate void SendBUCMessageDelegate(MessageEventArgs.MsgType msgType, string sMessage, DateTime dt);
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
            get { lock(this){ return bRunning; } }
            set { lock(this){ bRunning = value; } }
        }

        // A new BUService object is created whenever the service is STARTed,
        // and disposed of whenever the service is STOPped.
        public BUService()
        {
            InitializeComponent();
            
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
//            fsBULog = null;
//            swBULog = null;

            ReadConfiguration();
        }

        #region Private Methods

            #region Log

        public void LogInfo(string sCaller, string sInfo)
        {
            LogInfo(sCaller, sInfo, DateTime.Now);
        }

        // Log trace info to a text file to aid development.
        public void LogInfo(string sCaller, string sInfo, DateTime dt)
        {
            if (bLogInfo != true) return;

//            if (dt == null) dt = DateTime.Now;
            
            if (sw == null)
            {
                // In Windows Service, must provide full path to FileStream.
                string sDir = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).DirectoryName;
                string sFileName = String.Format("Info{0}.txt", dt.ToString("yyyyMMdd-HHmmss"));
                string sFullPath = Path.Combine(sDir, sFileName);
                fs = new FileStream(sFullPath, FileMode.Append, FileAccess.Write);
                sw = new StreamWriter(fs, Encoding.UTF8);
                // Make StreamWriter flush its buffer to the underlying stream after every call to StreamWriter.Write().
                sw.AutoFlush = true;
            }

            if (sw != null)
            {
                try
                {
                    string str;
                    if (sCaller.Length == 0)
                        str = String.Format("{0} {1}", dt.ToString("yyyy/MM/dd HH:mm:ss"), sInfo);
                    else
                        str = String.Format("{0} [{1}] {2}", dt.ToString("yyyy/MM/dd HH:mm:ss"), sCaller, sInfo);
                        
                    sw.WriteLine(str);
                }
                catch (Exception ex)
                {
                    EventLog.WriteEntry(ex.Message);
                }
            }
        }

        // Check if an existing log file exists.
        // Create a new log if there isn't one.
        private void OpenBULog(DateTime dt)
        {
//            if (swBULog == null)
//            {
                bool bUseExisting = false;
                string sFullPathFromReg = RegistryReadLogFullPath();
                string sFileNameFromReg;

                try
                {
                    if (File.Exists(sFullPathFromReg)) 
                    {
                        // Check to see if the filename contains today's date.
                        sFileNameFromReg = Path.GetFileName(sFullPathFromReg);
                        if (sFileNameFromReg.Contains(dt.ToString("yyyyMMdd")))
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
                                using (StreamWriter sw = File.AppendText(sFullPathFromReg))
                                {
                                    sw.WriteLine("");
                                    sw.WriteLine("Unclassified/For Official Use Only");
                                }
                            }
                            catch (Exception ex)
                            {
                                LogInfo("OpenBULog", ex.Message);
                            }
                        }
                    } 
                } 
                catch {}

//                string sFullPath;

//                if (bUseExisting) sFullPath = sFullPathFromReg;
                if (bUseExisting) sCurrentLog = sFullPathFromReg;
                else
                {
                    // Use new file name.
                
                    string sDir;

                    try
                    {
                        // Create the Log file directory if it does not exist.

                        if (!Directory.Exists(sLogFileLocation))
                            Directory.CreateDirectory(sLogFileLocation);
                        sDir = sLogFileLocation;
                    }
                    catch
                    {
                        // If failed to create directory, use EXE directory.
                        sDir = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).DirectoryName;
                    }

                    string sFileName = String.Format("BackupLog{0}.txt", dt.ToString("yyyyMMdd-HHmmss"));
//                    sFullPath
                    sCurrentLog = Path.Combine(sDir, sFileName);

                    try
                    {
                        // Creates a new file if it doesn't exists.
                        using (StreamWriter sw = File.AppendText(sCurrentLog))
                        {
                            sw.WriteLine("Unclassified/For Official Use Only");
                            sw.WriteLine("");
                        }
                    }
                    catch (Exception ex)
                    {
                        LogInfo("OpenBULog", ex.Message);
                    }

                    // Save full log file name in registry.
                    RegistryWriteLogFullPath(sCurrentLog);//sFullPath);
                }
//
/*
                if (!bUseExisting)
                {
                    try
                    {
                        // Creates a new file if it doesn't exists.
                        using (StreamWriter sw = File.AppendText(sCurrentLog))
                        {
                            sw.WriteLine("Unclassified/For Official Use Only");
                            sw.WriteLine("");
                        }
                    }
                    catch (Exception ex)
                    {
                        LogInfo("OpenBULog", ex.Message);
                    }
                }
*/
                // In Windows Service, must provide full path to FileStream.
//                fsBULog = new FileStream(sFullPath, FileMode.Append, FileAccess.Write, FileShare.Read);
//                swBULog = new StreamWriter(fsBULog, Encoding.UTF8);
                // Make StreamWriter flush its buffer to the underlying stream after every call to StreamWriter.Write().
//                swBULog.AutoFlush = true;
/*
                if (!bUseExisting)
                {
                    // Save full log file name in registry.
                    RegistryWriteLogFullPath(sCurrentLog);//sFullPath);
                }
*/
            LogInfo("OpenBULog", sCurrentLog);//sFullPath);
//            }
        }

        public void LogBUStep(string sStep)
        {
            LogBUStep(sStep, DateTime.Now);
        }

        // Log backup operation infomation to a text file.
        public void LogBUStep(string sStep, DateTime dt)
        {
//            if (dt == null) dt = DateTime.Now;

//            if (swBULog == null)
//            {
//                OpenBULog(dt);

/*
                string sDir;

                try
                {
                    // Create the Log file directory if it does not exist.

                    if (!Directory.Exists(sLogFileLocation))
                        Directory.CreateDirectory(sLogFileLocation);
                    sDir = sLogFileLocation;
                }
                catch
                {
                    // If failed to create directory, use EXE directory.
                    sDir = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).DirectoryName;
                }

                string sFileName = String.Format("BackupLog{0}.txt", dt.ToString("yyyyMMdd-HHmmss"));
                string sFullPath = Path.Combine(sDir, sFileName);

                // In Windows Service, must provide full path to FileStream.
                fsBULog = new FileStream(sFullPath, FileMode.Append, FileAccess.Write, FileShare.Read);
                swBULog = new StreamWriter(fsBULog, Encoding.UTF8);
                // Make StreamWriter flush its buffer to the underlying stream after every call to StreamWriter.Write().
                swBULog.AutoFlush = true;
                // Save full log file name in registry.
                RegistryWriteLogFullPath(sFullPath);
*/
//            }
/*
            if (swBULog != null)
            {
                try
                {
                    string str = String.Format("{0} {1}", dt.ToString("yyyy/MM/dd HH:mm:ss"), sStep);
                    swBULog.WriteLine(str);
                }
                catch (Exception ex)
                {
                    EventLog.WriteEntry(ex.Message);
                }
            }
*/
            try
            {
                if (!File.Exists(sCurrentLog)) OpenBULog(DateTime.Now);

                using (StreamWriter sw = File.AppendText(sCurrentLog))
                {
                    string str = String.Format("{0} {1}", dt.ToString("yyyy/MM/dd HH:mm:ss"), sStep);
                    sw.WriteLine(str);
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(ex.Message);
            }
        }

            #endregion // Log

            #region Registry

        // Read backup start time from registry.
        private void ReadScheduledTime()
        {
            bool bWriteConfig = false;

            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\FuelsManager\FMBackupUtility");
            if (key != null)
            {
                using (key)
                {
                    object obj = key.GetValue("Ticks");
                    if (obj != null)
                    {
                        Int64 i64Val;
                        Int64.TryParse((string)obj, System.Globalization.NumberStyles.Integer, null, out i64Val);

                        tsTimeOfDay = new TimeSpan(i64Val);

                        LogInfo("ReadScheduledTime - Ticks", tsTimeOfDay.ToString());

                        if (tsTimeOfDay < TimeSpan.Zero || tsTimeOfDay > TimeSpan.FromDays(1))
                            tsTimeOfDay = TimeSpan.FromHours(1);

                        LogInfo("ReadScheduledTime - Ticks - New", tsTimeOfDay.ToString());
                    }
                    else
                    {
                        tsTimeOfDay = TimeSpan.FromHours(1);
                        bWriteConfig = true;
                    }
                }
            }
            else
            {
                tsTimeOfDay = TimeSpan.FromHours(1);
                bWriteConfig = true;
            }
            if (bWriteConfig == true) WriteScheduledTime();
        }

        private void WriteScheduledTime()
        {
            using (RegistryKey key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\FuelsManager\FMBackupUtility"))
            {
                key.SetValue("Ticks", tsTimeOfDay.Ticks.ToString(), RegistryValueKind.String);
            }
        }

        // Read file paths from registry.
        private void ReadConfiguration()
        {
            bool bWriteConfig = false;
            object obj;

            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\FuelsManager\FMBackupUtility");
            if (key != null)
            {
                using (key)
                {
//                    object obj;
/*
                    object obj = key.GetValue("Ticks");
                    if (obj != null)
                    {
                        Int64 i64Val;
                        Int64.TryParse((string)obj, System.Globalization.NumberStyles.Integer, null, out i64Val);
                        dtStartTime = new DateTime(i64Val);
//                        LogInfo("ReadConfiguration - Time", dtStartTime.ToString());
                    }
*/
                    obj = key.GetValue("LogFilePath");
                    if (obj != null)
                    {
                        sLogFileLocation = (string)obj;
                    }
                    if (obj == null || String.IsNullOrEmpty(sLogFileLocation) == true)
                    {
                        // If no path in Registry, use ExeDir\Log directory.
                        try
                        {
                            sLogFileLocation = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).DirectoryName;
                            sLogFileLocation = Path.Combine(sLogFileLocation, "Log");
                            if (!Directory.Exists(sLogFileLocation)) Directory.CreateDirectory(sLogFileLocation);
                            bWriteConfig = true;
                        }
                        catch {}
                    }

                    obj = key.GetValue("ZipFilePath");
                    if (obj != null)
                    {
                        sZipFileLocation = (string)obj;
                    }
                    if (obj == null || String.IsNullOrEmpty(sZipFileLocation) == true)
                    {
                        // If no path in Registry, use ExeDir\Zip directory.
                        try
                        {
                            sZipFileLocation = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).DirectoryName;
                            sZipFileLocation = Path.Combine(sZipFileLocation, "Zip");
                            if (!Directory.Exists(sZipFileLocation)) Directory.CreateDirectory(sZipFileLocation);
                            bWriteConfig = true;
                        }
                        catch {}
                    }

                    obj = key.GetValue("AdditionalFilesPaths"); // Multi-strings
                    if (obj != null)
                    {
                        sPaths = (string[])obj;
/*
                        for (int i = 0; i < sPaths.Length; i++)
                        {
                            if (!String.IsNullOrEmpty(sPaths[i]))
                            {
                                LogInfo("ReadConfiguration - Additnl", sPaths[i]);
                            }
                        }
*/
                    }
                }
            }
            else
            {
                // Create ExeDir\Log directory as default.
                try
                {
                    sLogFileLocation = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).DirectoryName;
                    sLogFileLocation = Path.Combine(sLogFileLocation, "Log");
                    if (!Directory.Exists(sLogFileLocation)) Directory.CreateDirectory(sLogFileLocation);
                    bWriteConfig = true;
                }
                catch {}

                // Create ExeDir\Zip directory as default.
                try
                {
                    sZipFileLocation = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).DirectoryName;
                    sZipFileLocation = Path.Combine(sZipFileLocation, "Zip");
                    if (!Directory.Exists(sZipFileLocation)) Directory.CreateDirectory(sZipFileLocation);
                    bWriteConfig = true;
                }
                catch {}
            }

            if (bWriteConfig == true) WriteConfiguration();

            key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\MSSQLServer\Setup");
            if (key != null)
            {
                using (key)
                {
                    // Get path for audit trace files (FMDAuditTrace*.trc).
                    obj = key.GetValue("SQLDataRoot");
                    if (obj != null)
                    {
                        try
                        {
                            string str = (string)obj;
                            sSQLPath = Path.Combine(str, "Data");
                        }
                        catch
                        {
                            sSQLPath = null;
                        }
//                        LogInfo("ReadConfiguration - SQL", sSQLPath);
                    }
                }
            }


            key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\FuelsManager");
            if (key != null)
            {
                using (key)
                {
                    // Get path for Inventory Management real-time DB files.
                    obj = key.GetValue("Project");
                    if (obj != null)
                    {
                        sFMProjectPath = (string)obj;
//                        LogInfo("ReadConfiguration - FMProject", sFMProjectPath);
                    }
                }
            }

            key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Syn-Tech Systems\ADC-FDS\Config\Path");
            if (key != null)
            {
                using (key)
                {
                    // Get path for Syn-Tech Systems ADC-FDC DB.
                    obj = key.GetValue("CURRDB");
                    if (obj != null)
                    {
                        try
                        {
                            sADCFDCPath = (string)obj;
                            // Remove filename.
                            sADCFDCPath = Path.GetDirectoryName(sADCFDCPath);
                        }
                        catch
                        {
                            sADCFDCPath = null;
                        }
//                        LogInfo("ReadConfiguration - ADC-FDS", sADCFDCPath);
                    }
                }
            }

            key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Syn-Tech Systems\DoDFM Adv");
            if (key != null)
            {
                using (key)
                {
                    // Get path for Syn-Tech Systems DodFM AE DBs (FuelMaster).
                    obj = key.GetValue("HOME");
                    if (obj != null)
                    {
                        sDoDFMAEPath = (string)obj;
//                        LogInfo("ReadConfiguration - DoDFM AE", sDoDFMAEPath);
                    }
                }
            }
        }

        private void WriteConfiguration()
        {
            using (RegistryKey key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\FuelsManager\FMBackupUtility"))
            {
                key.SetValue("LogFilePath", sLogFileLocation, RegistryValueKind.String);
                key.SetValue("ZipFilePath", sZipFileLocation, RegistryValueKind.String);
            }
        }

        // Read file paths from registry.
        private string RegistryReadLogFullPath()
        {
            string str = "";
            object obj;

            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\FuelsManager\FMBackupUtility");
            if (key != null)
            {
                using (key)
                {
                    obj = key.GetValue("LogFileFullPath");
                    if (obj != null) str = (string)obj;
                }
            }
            return str;
        }

        // Write the Log filename and location in the registry.
        private void RegistryWriteLogFullPath(string sFullPath)
        {
            if (File.Exists(sFullPath))
            {
                using (RegistryKey key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\FuelsManager\FMBackupUtility"))
                {
                    key.SetValue("LogFileFullPath", sFullPath, RegistryValueKind.String);
                }
            }
        }

            #endregion // Registry
            
        // Back up SQL DBs using internal SQL Server backup functionality to holding directory.
        // AccountingDB
        // AviationDB
        // ConsolidatedDB
        // Master
        // Model
        // MSDB
        // FMMovementLog
        // FMArchive
        private bool BackUpSQLDBs()
        {
            LogBUStep("Backing up SQL databases.");

            // AccountingDB
            using (SqlConnection con = new SqlConnection()) // Create an empty SqlConnection object.
            {
                // Configure the SqlConnection object's connection string.
                con.ConnectionString = DBAdminConnect.getConnectionString("FMDAdmin", "AccountingDB");

                try
                {
                    // Open the database connection.
                    con.Open();

                    if (con.State == ConnectionState.Open)
                    {
                        // Create and configure a new command.
                        IDbCommand com = con.CreateCommand();
                        com.CommandType = CommandType.Text;
                        com.CommandTimeout = 300;
                        string sPath = Path.Combine(sHoldingDir, "AccountingDB.bak");
                        com.CommandText = String.Format("backup database AccountingDB to disk = '{0}' with init", sPath);

                        com.ExecuteNonQuery();
                    }
                    else
                    {
                        LogBUStep("Could not open SQL connection to back up AccountingDB.");
                        LogInfo("BackUpSQLDBs", "Could not open SqlConnection for AccountingDB.");
                    }
                }
                catch (System.Exception ex)
                {
                    LogBUStep(String.Format("Could not back up AccountingDB database: {0}", ex.Message));
                    LogInfo("BackUpSQLDBs - AccountingDB", ex.Message);
                }
                // At the end of the using block Dispose() calls Close().
            }

            // AviationDB
            using (SqlConnection con = new SqlConnection())
            {
                // Configure the SqlConnection object's connection string.
                con.ConnectionString = DBAdminConnect.getConnectionString("FMDAdmin", "AviationDB");

                try
                {
                    // Open the database connection.
                    con.Open();

                    if (con.State == ConnectionState.Open)
                    {
                        // Create and configure a new command.
                        IDbCommand com = con.CreateCommand();
                        com.CommandType = CommandType.Text;
                        com.CommandTimeout = 300;
                        string sPath = Path.Combine(sHoldingDir, "AviationDB.bak");
                        com.CommandText = String.Format("backup database AviationDB to disk = '{0}' with init", sPath);

                        com.ExecuteNonQuery();
                    }
                    else
                    {
                        LogBUStep("Could not open SQL connection to back up AviationDB.");
                        LogInfo("BackUpSQLDBs", "Could not open SqlConnection for AviationDB.");
                    }
                }
                catch (System.Exception ex)
                {
                    LogBUStep(String.Format("Could not back up AviationDB database: {0}", ex.Message));
                    LogInfo("BackUpSQLDBs - AviationDB", ex.Message);
                }
                // At the end of the using block Dispose() calls Close().
            }

            // ConsolidatedDB
            using (SqlConnection con = new SqlConnection())
            {
                // Configure the SqlConnection object's connection string.
                con.ConnectionString = DBAdminConnect.getConnectionString("FMDAdmin", "ConsolidatedDB");

                try
                {
                    // Open the database connection.
                    con.Open();

                    if (con.State == ConnectionState.Open)
                    {
                        // Create and configure a new command.
                        IDbCommand com = con.CreateCommand();
                        com.CommandType = CommandType.Text;
                        com.CommandTimeout = 300;
                        string sPath = Path.Combine(sHoldingDir, "ConsolidatedDB.bak");
                        com.CommandText = String.Format("backup database ConsolidatedDB to disk = '{0}' with init", sPath);

                        com.ExecuteNonQuery();
                    }
                    else
                    {
                        LogBUStep("Could not open SQL connection to back up ConsolidatedDB.");
                        LogInfo("BackUpSQLDBs", "Could not open SqlConnection for ConsolidatedDB.");
                    }
                }
                catch (System.Exception ex)
                {
                    LogBUStep(String.Format("Could not back up ConsolidatedDB database: {0}", ex.Message));
                    LogInfo("BackUpSQLDBs - ConsolidatedDB", ex.Message);
                }
                // At the end of the using block Dispose() calls Close().
            }

            // Master
            using (SqlConnection con = new SqlConnection())
            {
                // Configure the SqlConnection object's connection string.
                con.ConnectionString = DBAdminConnect.getConnectionString("FMDAdmin", "Master");

                try
                {
                    // Open the database connection.
                    con.Open();

                    if (con.State == ConnectionState.Open)
                    {
                        // Create and configure a new command.
                        IDbCommand com = con.CreateCommand();
                        com.CommandType = CommandType.Text;
                        com.CommandTimeout = 300;
                        string sPath = Path.Combine(sHoldingDir, "Master.bak");
                        com.CommandText = String.Format("backup database Master to disk = '{0}' with init", sPath);

                        com.ExecuteNonQuery();
                    }
                    else
                    {
                        LogBUStep("Could not open SQL connection to back up Master database.");
                        LogInfo("BackUpSQLDBs", "Could not open SqlConnection for Master database.");
                    }
                }
                catch (System.Exception ex)
                {
                    LogBUStep(String.Format("Could not back up Master database: {0}", ex.Message));
                    LogInfo("BackUpSQLDBs - Master", ex.Message);
                }
                // At the end of the using block Dispose() calls Close().
            }

            // Model
            using (SqlConnection con = new SqlConnection())
            {
                // Configure the SqlConnection object's connection string.
                con.ConnectionString = DBAdminConnect.getConnectionString("FMDAdmin", "Model");

                try
                {
                    // Open the database connection.
                    con.Open();

                    if (con.State == ConnectionState.Open)
                    {
                        // Create and configure a new command.
                        IDbCommand com = con.CreateCommand();
                        com.CommandType = CommandType.Text;
                        com.CommandTimeout = 300;
                        string sPath = Path.Combine(sHoldingDir, "Model.bak");
                        com.CommandText = String.Format("backup database Model to disk = '{0}' with init", sPath);

                        com.ExecuteNonQuery();
                    }
                    else
                    {
                        LogBUStep("Could not open SQL connection to back up Model database.");
                        LogInfo("BackUpSQLDBs", "Could not open SqlConnection for Model database.");
                    }
                }
                catch (System.Exception ex)
                {
                    LogBUStep(String.Format("Could not back up Model database: {0}", ex.Message));
                    LogInfo("BackUpSQLDBs - Model", ex.Message);
                }
                // At the end of the using block Dispose() calls Close().
            }

            // MSDB
            using (SqlConnection con = new SqlConnection())
            {
                // Configure the SqlConnection object's connection string.
                con.ConnectionString = DBAdminConnect.getConnectionString("FMDAdmin", "MSDB");

                try
                {
                    // Open the database connection.
                    con.Open();

                    if (con.State == ConnectionState.Open)
                    {
                        // Create and configure a new command.
                        IDbCommand com = con.CreateCommand();
                        com.CommandType = CommandType.Text;
                        com.CommandTimeout = 300;
                        string sPath = Path.Combine(sHoldingDir, "MSDB.bak");
                        com.CommandText = String.Format("backup database MSDB to disk = '{0}' with init", sPath);

                        com.ExecuteNonQuery();
                    }
                    else
                    {
                        LogBUStep("Could not open SQL connection to back up MSDB database.");
                        LogInfo("BackUpSQLDBs", "Could not open SqlConnection for MSDB database.");
                    }
                }
                catch (System.Exception ex)
                {
                    LogBUStep(String.Format("Could not back up MSDB database: {0}", ex.Message));
                    LogInfo("BackUpSQLDBs - MSDB", ex.Message);
                }
                // At the end of the using block Dispose() calls Close().
            }

            // FMMovementLog
            using (SqlConnection con = new SqlConnection())
            {
                // Configure the SqlConnection object's connection string.
                con.ConnectionString = DBAdminConnect.getConnectionString("FMDAdmin", "FMMovementLog");

                try
                {
                    // Open the database connection.
                    con.Open();

                    if (con.State == ConnectionState.Open)
                    {
                        // Create and configure a new command.
                        IDbCommand com = con.CreateCommand();
                        com.CommandType = CommandType.Text;
                        com.CommandTimeout = 300;
                        string sPath = Path.Combine(sHoldingDir, "FMMovementLog.bak");
                        com.CommandText = String.Format("backup database FMMovementLog to disk = '{0}' with init", sPath);

                        com.ExecuteNonQuery();
                    }
                    else
                    {
                        LogBUStep("Could not open SQL connection to back up FMMovementLog database.");
                        LogInfo("BackUpSQLDBs", "Could not open SqlConnection for FMMovementLog database.");
                    }
                }
                catch (System.Exception ex)
                {
                    LogBUStep(String.Format("Could not back up ConsolidatedDB database: {0}", ex.Message));
                    LogInfo("BackUpSQLDBs - FMMovementLog", ex.Message);
                }
                // At the end of the using block Dispose() calls Close().
            }

            // FMArchive
            using (SqlConnection con = new SqlConnection())
            {
                // Configure the SqlConnection object's connection string.
                con.ConnectionString = DBAdminConnect.getConnectionString("FMDAdmin", "FMArchive");

                try
                {
                    // Open the database connection.
                    con.Open();

                    if (con.State == ConnectionState.Open)
                    {
                        // Create and configure a new command.
                        IDbCommand com = con.CreateCommand();
                        com.CommandType = CommandType.Text;
                        com.CommandTimeout = 300;
                        string sPath = Path.Combine(sHoldingDir, "FMArchive.bak");
                        com.CommandText = String.Format("backup database FMArchive to disk = '{0}' with init", sPath);

                        com.ExecuteNonQuery();
                    }
                    else
                    {
                        LogBUStep("Could not open SQL connection to back up FMArchive database.");
                        LogInfo("BackUpSQLDBs", "Could not open SqlConnection for FMArchive database.");
                    }
                }
                catch (System.Exception ex)
                {
                    LogBUStep(String.Format("Could not back up FMArchive database: {0}", ex.Message));
                    LogInfo("BackUpSQLDBs - FMArchive", ex.Message);
                }
                // At the end of the using block Dispose() calls Close().
            }

            EventLog.WriteEntry("FMDAdmin was used for database backup.");

            return true;
        }

        private long CalculateSpecificTypeFilesSize(string sDirectory, string sSearchPattern)
        {
            long lSize = 0;
            try
            {
                DirectoryInfo dir = new DirectoryInfo(sDirectory);
                if (dir.Exists)
                {
                    FileInfo[] files = dir.GetFiles(sSearchPattern);
                    foreach (FileInfo file in files)
                    {
                        lSize += file.Length;
                    }
                }
            }
            catch (Exception ex)
            {
                LogInfo("CalculateSpecificTypeFilesSize", ex.Message);
            }
//            LogInfo("CalculateSpecificTypeFilesSize", String.Format("{0} - {1} - {2}", lSize, sSearchPattern, sDirectory));
            return lSize;
        }

        private long CalculateDirectorySize(string sDirectory, bool includeSubdirectories)
        {
            long lSize = 0;
            try
            {
                DirectoryInfo dir = new DirectoryInfo(sDirectory);
                if (dir.Exists)
                {
                    lSize = CalculateDirectorySize(dir, true);
//                    LogInfo("CalculateDirectorySize", String.Format("{0} - {1}", lSize, sDirectory));
                }
            }
            catch (Exception ex)
            {
                LogInfo("CalculateDirectorySize", ex.Message);
            }
            return lSize;
        }

        private long CalculateDirectorySize(DirectoryInfo directory, bool includeSubdirectories)
        {
            long totalSize = 0;

            // Examine all contained files.
            FileInfo[] files = directory.GetFiles();
            foreach (FileInfo file in files)
            {
                totalSize += file.Length;
            }

            // Examine all contained directories.
            if (includeSubdirectories)
            {
                DirectoryInfo[] dirs = directory.GetDirectories();
                foreach (DirectoryInfo dir in dirs)
                {
                    totalSize += CalculateDirectorySize(dir, true);
                }
            }
            return totalSize;
        }

        private void CopySpecificTypeFiles(string sourceDirectory, string targetDirectory, string sSearchPattern)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            if (!diSource.Exists) return;

            string sPathNoRoot = diSource.FullName;
            if (sPathNoRoot.Length > 3)
            {
                sPathNoRoot = diSource.FullName.Remove(0, 3);

                // Preserve the directory structure.
                targetDirectory = Path.Combine(targetDirectory, sPathNoRoot);//diSource.Name);
            }
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);

            if (!diTarget.Exists)
            {
                diTarget.Create();
            }

            // Copy all files.
            FileInfo[] files = diSource.GetFiles(sSearchPattern);
            foreach (FileInfo file in files)
            {
                try
                {
                    // Overwrite existing files.
                    file.CopyTo(Path.Combine(diTarget.FullName, file.Name), true);
                }
                catch {}
            }
        }

        private void CopyDirContents(string sourceDirectory, string targetDirectory)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            if (!diSource.Exists) return;

            string sPathNoRoot = diSource.FullName;
            if (sPathNoRoot.Length > 3)
            {
                sPathNoRoot = diSource.FullName.Remove(0, 3);

                // Preserve the directory structure.
                targetDirectory = Path.Combine(targetDirectory, sPathNoRoot);//diSource.Name);
            }
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);

            CopyDirectory(diSource, diTarget);
        }

        private void CopyDirectory(DirectoryInfo source, DirectoryInfo destination)
        {
            if (!source.Exists) return;
            if (!destination.Exists)
            {
                destination.Create();
            }

            // Copy all files.
            FileInfo[] files = source.GetFiles();
            foreach (FileInfo file in files)
            {
                try
                {
                    // Overwrite existing files.
                    file.CopyTo(Path.Combine(destination.FullName, file.Name), true);
                }
                catch {}
            }

            // Process subdirectories.
            DirectoryInfo[] dirs = source.GetDirectories();
            foreach (DirectoryInfo dir in dirs)
            {
                // Get destination directory.
                string destinationDir = Path.Combine(destination.FullName, dir.Name);

                // Call CopyDirectory() recursively.
                CopyDirectory(dir, new DirectoryInfo(destinationDir));
            }
        }

        private bool CopyDBFiles()
        {
            LogBUStep("Copying databases and files to a holding directory.");

            try
            {
                // Determine the required disk space.

                long lRequiredSpace = 0;

                // Check holding directory after SQL Databases backup process is done.
                if (sHoldingDir != null)
                    lRequiredSpace += CalculateSpecificTypeFilesSize(sHoldingDir, "*.bak");

                // Audit trace files (...\MSSQL\DATA\FMDAuditTrace*.trc).
                if (sSQLPath != null)
                    lRequiredSpace += CalculateSpecificTypeFilesSize(sSQLPath, "*.trc");

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
                        string sPath = Path.Combine(sFMProjectPath, "Archives");
                        lRequiredSpace += CalculateDirectorySize(sPath, true);

                        sPath = Path.Combine(sFMProjectPath, "CM_Data");
                        lRequiredSpace += CalculateDirectorySize(sPath, true);

                        sPath = Path.Combine(sFMProjectPath, "DBBackUps");
                        lRequiredSpace += CalculateDirectorySize(sPath, true);

                        sPath = Path.Combine(sFMProjectPath, "Details");
                        lRequiredSpace += CalculateDirectorySize(sPath, true);

                        sPath = Path.Combine(sFMProjectPath, "Graphics");
                        lRequiredSpace += CalculateDirectorySize(sPath, true);

                        sPath = Path.Combine(sFMProjectPath, "Log");
                        lRequiredSpace += CalculateDirectorySize(sPath, true);

                        sPath = Path.Combine(sFMProjectPath, "Reports");
                        lRequiredSpace += CalculateDirectorySize(sPath, true);

                        sPath = Path.Combine(sFMProjectPath, "RTU");
                        lRequiredSpace += CalculateDirectorySize(sPath, true);

                        sPath = Path.Combine(sFMProjectPath, "Straps");
                        lRequiredSpace += CalculateDirectorySize(sPath, true);
                    }
                    catch {}
                }

                if (sADCFDCPath != null)
                {
                    // Syn-Tech Systems ADC-FDC DB.  Add sizes of:
                    // \Archive
                    // \Reports
                    // *.mdb

                    try
                    {
                        lRequiredSpace += CalculateSpecificTypeFilesSize(sADCFDCPath, "*.mdb");

                        string sPath = Path.Combine(sADCFDCPath, "Archive");
                        lRequiredSpace += CalculateDirectorySize(sPath, true);

                        sPath = Path.Combine(sADCFDCPath, "Reports");
                        lRequiredSpace += CalculateDirectorySize(sPath, true);
                    }
                    catch {}
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
                        lRequiredSpace += CalculateSpecificTypeFilesSize(sDoDFMAEPath, "*.mdb");

                        string sPath = Path.Combine(sDoDFMAEPath, "Archive");
                        lRequiredSpace += CalculateDirectorySize(sPath, true);

                        sPath = Path.Combine(sDoDFMAEPath, "RawData");
                        lRequiredSpace += CalculateDirectorySize(sPath, true);

                        sPath = Path.Combine(sDoDFMAEPath, "Reports");
                        lRequiredSpace += CalculateDirectorySize(sPath, true);

                        sPath = Path.Combine(sDoDFMAEPath, "Transactions");
                        lRequiredSpace += CalculateDirectorySize(sPath, true);
                    }
                    catch {}
                }

                // Additional files specified by the user.
                // Add the sizes of the directories.
                if (sPaths != null)
                {
                    for (int i = 0; i < sPaths.Length; i++)
                    {
                        if (!String.IsNullOrEmpty(sPaths[i]))
                            lRequiredSpace += CalculateDirectorySize(sPaths[i], true);
                    }
                }

//                LogInfo("CopyDBFiles", lRequiredSpace.ToString());

                lRequiredSpace *= (long)4;

//                LogInfo("CopyDBFiles 4X", lRequiredSpace.ToString());

                // Check local drive available space.

                DirectoryInfo diExeDir = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).Directory;
                DriveInfo drive = new DriveInfo(diExeDir.Root.FullName);

//                LogInfo("CopyDBFiles", lRequiredSpace.ToString());
//                LogInfo("CopyDBFiles", drive.AvailableFreeSpace.ToString());
                
                if (drive.AvailableFreeSpace < lRequiredSpace) // drive.AvailableFreeSpace could throw IOException
                {
                    throw new Exception("Insufficient local disk space.");
                }

                // Copy from all directories.

                // Audit trace files (...\MSSQL\DATA\FMDAuditTrace*.trc).
                if (sSQLPath != null) CopySpecificTypeFiles(sSQLPath, sHoldingDir, "*.trc");

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
                        string sPath = Path.Combine(sFMProjectPath, "Archives");
                        CopyDirContents(sPath, sHoldingDir);

                        sPath = Path.Combine(sFMProjectPath, "CM_Data");
                        CopyDirContents(sPath, sHoldingDir);

                        sPath = Path.Combine(sFMProjectPath, "DBBackUps");
                        CopyDirContents(sPath, sHoldingDir);

                        sPath = Path.Combine(sFMProjectPath, "Details");
                        CopyDirContents(sPath, sHoldingDir);

                        sPath = Path.Combine(sFMProjectPath, "Graphics");
                        CopyDirContents(sPath, sHoldingDir);

                        sPath = Path.Combine(sFMProjectPath, "Log");
                        CopyDirContents(sPath, sHoldingDir);

                        sPath = Path.Combine(sFMProjectPath, "Reports");
                        CopyDirContents(sPath, sHoldingDir);

                        sPath = Path.Combine(sFMProjectPath, "RTU");
                        CopyDirContents(sPath, sHoldingDir);

                        sPath = Path.Combine(sFMProjectPath, "Straps");
                        CopyDirContents(sPath, sHoldingDir);
                    }
                    catch {}
                }

                if (sADCFDCPath != null)
                {
                    // Syn-Tech Systems ADC-FDC DB.  Copy subdirectories and mdb files:
                    // \Archive
                    // \Reports
                    // *.mdb
                    try
                    {
                        CopySpecificTypeFiles(sADCFDCPath, sHoldingDir, "*.mdb");
                        
                        string sPath = Path.Combine(sADCFDCPath, "Archive");
                        CopyDirContents(sPath, sHoldingDir);

                        sPath = Path.Combine(sADCFDCPath, "Reports");
                        CopyDirContents(sPath, sHoldingDir);
                    }
                    catch {}
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
                        CopySpecificTypeFiles(sDoDFMAEPath, sHoldingDir, "*.mdb");

                        string sPath = Path.Combine(sDoDFMAEPath, "Archive");
                        CopyDirContents(sPath, sHoldingDir);

                        sPath = Path.Combine(sDoDFMAEPath, "RawData");
                        CopyDirContents(sPath, sHoldingDir);

                        sPath = Path.Combine(sDoDFMAEPath, "Reports");
                        CopyDirContents(sPath, sHoldingDir);

                        sPath = Path.Combine(sDoDFMAEPath, "Transactions");
                        CopyDirContents(sPath, sHoldingDir);
                    }
                    catch {}
                }

                // Copy additional files.
                if (sPaths != null)
                {
                    for (int i = 0; i < sPaths.Length; i++)
                    {
                        if (!String.IsNullOrEmpty(sPaths[i]))
                            CopyDirContents(sPaths[i], sHoldingDir);
                    }
                }
            }

            catch (System.IO.IOException exIO)
            {
                // Drive not ready.
                string sErr = String.Format("Could not copy files: {0}", exIO.Message);
                throw new Exception(sErr, exIO);
            }
            catch (System.Exception ex)
            {
                string sErr = String.Format("Could not copy files: {0}", ex.Message);
                throw new Exception(sErr, ex);
            }
            return true;
        }

        private bool ZipFiles()
        {
            // Compress and package files according to Zip standard.

            LogBUStep("Compressing databases and files into a zip file.");

            bool bDeleteTempDir = false;

            try
            {
                // This shouldn't happen, but just in case.
                if (!Directory.Exists(sHoldingDir))
                {
                    throw new Exception("Database holding directory does not exist.");
                }

                string sTempZipDir;
                string sTime = DateTime.Now.ToString("yyyyMMdd-HHmmss");
                FileInfo fiExe = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
                
                try
                {
                    // Create temp zip directory.

//                    DirectoryInfo diExeDir = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).Directory;
                    DirectoryInfo diExeDir = fiExe.Directory;
                    string sDir = String.Format("TempZipDir{0}", sTime);
                    sTempZipDir = Path.Combine(diExeDir.FullName, sDir);

//                        if (!Directory.Exists(sTempZipDir))
                    Directory.CreateDirectory(sTempZipDir);
                    bDeleteTempDir = true;
                }
                catch
                {
                    // If failed to create directory, use EXE directory.
                    sTempZipDir = fiExe.DirectoryName;
//                    sTempZipDir = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).DirectoryName;
                }

                // Zip filename consists of machine name, date, and time.
                string sFileName = String.Format("{0}-{1}.zip", Environment.MachineName, sTime);
                string sFullPath = System.IO.Path.Combine(sTempZipDir, sFileName);

                // Create zip in temp zip dir.
                using (ZipFile zip = new ZipFile(sFullPath))
                { 
                    zip.AddDirectory(sHoldingDir); // Keep dir structure.
                    zip.Save();
                }

                string sTargetFullPath;
                
                // Copy zip file to zip target directory.
                try
                {
                    // Create the zip target directory if it does not exist.
                    if (!Directory.Exists(sZipFileLocation))
                        Directory.CreateDirectory(sZipFileLocation);

                    sTargetFullPath = System.IO.Path.Combine(sZipFileLocation, sFileName);

                    // Copy from temp zip dir to target zip dir.
                    // Should throw exception if not enough space.
                    File.Copy(sFullPath, sTargetFullPath, true);
                }
                catch (Exception ex)
                {
                    // Delete holding directory and its contents.
                    try
                    {
                        DeleteDirectory(sHoldingDir);
                    }
                    catch (Exception ex1)
                    {
                        LogInfo("ZipFiles", ex1.Message);
                    }
                    finally
                    {
                        sHoldingDir = null;
                    }

                    string sErr = String.Format("{0}: {1}  {2} {3}",
                                                "Could not copy zip file to target directory",
                                                ex.Message,
                                                "Zip file saved in",
                                                sFullPath);
                    throw new CreateDirectoryException(sErr, ex);
                }

                // If temp zip directory is not EXE directory, delete temp zip directory.
                if (bDeleteTempDir)
                {
                    try
                    {
                        Directory.Delete(sTempZipDir, true);
                    }
                    catch (Exception ex0)
                    {
                        LogInfo("ZipFiles", ex0.Message);
                    }
                }

                // Log completion of zip file creation.
                DateTime dt = DateTime.Now;
                string str = String.Format("Created zip file: {0}", sTargetFullPath);
                LogBUStep(str, dt);
                LogInfo("ZipFiles", str, dt);
            }
            
            catch (CreateDirectoryException ex)
            {
                throw (ex);
            }
            catch (System.Exception ex)
            {
                string sErr = String.Format("Could not create zip file: {0}", ex.Message);
                throw new Exception(sErr, ex);
            }
            return true;
        }

        #endregion // Private Methods
        
        #region Multithread

        // ==================================================================================================
        // < BUC APPLICATION AS REMOTE SERVER >

        private void SendBUCMessage(MessageEventArgs.MsgType msgType, string sMsg, DateTime dt)
        {
            // Asynchronous remote call to BUC.
            AsyncCallback callback = new AsyncCallback(this.SendBUCMessageCallBack);
            SendBUCMessageDelegate del = new SendBUCMessageDelegate(roBUC.UpdateMessage);
            IAsyncResult ar = del.BeginInvoke(msgType, sMsg, dt, callback, this);
        }

        // Callback method that is called when SendBUCMessageDelegate completes its async call.
        private void SendBUCMessageCallBack(IAsyncResult ar)
        {
            // Obtains the last parameter of the delegate call.
            BUService service = (BUService)ar.AsyncState;
            // Get the delegate object on which the asynchronous call was invoked.
            SendBUCMessageDelegate del = (SendBUCMessageDelegate)((AsyncResult)ar).AsyncDelegate;
            
            try
            {
                del.EndInvoke(ar); // No return value.
            }
            catch (Exception ex)
            {
                // BUC Server is not available.

                service.LogInfo("SendBUCMessageCallBack", ex.Message);
            }
        }
        // ==================================================================================================



        // ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        // < BU SERVICE AS SERVER >
        
        // Handling messages from BUC.
        void roBU_MessageToBUEvent(object sender, MessageToBUEventArgs e)
        {
            if (IsRunning)
            {
                LogInfo("roBU_MessageToBUEvent", "A session is already in progress.");
                return;
            }

            switch (e.MessageType)
            {
                case MessageToBUEventArgs.MsgType.MSG_BACKUPNOW:
                    LogInfo("roBU_MessageToBUEvent", "Back up now.");
                    eventRun.Set();
                    break;

                case MessageToBUEventArgs.MsgType.MSG_UPDATECONFIG:
                    ReadScheduledTime();
                    // Create a timer to run TimerProc() at specific time.
                    SetTimer();

                    // This timer setting depends on the backup process timer, so,
                    // rescheduling the backup process also requires a call to this method.
                    SetCreateLogTimer();

                    // This creates a log file if there isn't one.
                    OpenBULog(DateTime.Now);
                    break;
            }
        }
        // ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        // Callback method that runs on a threadpool thread.
        private void CreateLogTimerProc(object state)
        {
            BUService service = (BUService)state;

            service.LogInfo("CreateLogTimerProc", ".");

            if (service.IsRunning)
            {
                service.LogInfo("CreateLogTimerProc", "A backup session is in progress.");
                return;
            }
            // This creates a log file if there isn't one.
            OpenBULog(DateTime.Now);
        }

        // Callback method that runs on a threadpool thread.
        private void TimerProc(object state)
        {
            // The callback method executed by the timer is reentrant, because
            // it is called on ThreadPool threads.  The callback can be executed
            // simultaneously on two thread pool threads if the timer interval
            // is less than the time required to execute the callback, or if
            // all thread pool threads are in use and the callback is queued multiple times.

            BUService service = (BUService)state;

            service.LogInfo("TimerProc", "Entered.");

            if (service.IsRunning)
            {
                service.LogInfo("TimerProc", "A session is already in progress.");
                return;
            }

            eventRun.Set();
        }

        // Callback method that runs on a threadpool thread.
        private void CheckKeyTimerProc(object state)
        {
            BUService service = (BUService)state;

            if (IsSecurityKeyPresent()) return;
            
            if (timerCheckKey != null)
            {
                timerCheckKey.Dispose();
                timerCheckKey = null;
            }
            // Request to stop backup service immediately.
            service.Stop();
        }

        private bool IsSecurityKeyPresent()
        {
            byte byFuelsManagerType = 0;
            int iKeyFound           = 0; // false
            ushort usProgramVersion = 0;

            FMAccessClass fmAccess = new FMAccessClass();

            fmAccess.GetIMType(ref byFuelsManagerType, ref iKeyFound, ref usProgramVersion);
            if (iKeyFound == 1) // true
            {
		        if (usProgramVersion != FUELSMANAGER_SENTINEL_REVISION &&
			        usProgramVersion != DEVELOPER_KEY)
		        {
                    DateTime dt = DateTime.Now;
                    string str = "Installed Hardware key is not for this version of FuelsManager.";
                    LogBUStep(str, dt);
                    LogInfo("IsSecurityKeyPresent", str, dt);
                    EventLog.WriteEntry(str, EventLogEntryType.Error); // Log failure with reason.
			        return false;
		        }

            }
            else
            {
                DateTime dt = DateTime.Now;
                string str = "Hardware key not found.";
                LogBUStep(str, dt);
                LogInfo("IsSecurityKeyPresent", str, dt);
                EventLog.WriteEntry(str, EventLogEntryType.Error); // Log failure with reason.
                return false;
            }
            return true;
        }

        private void DeleteDirectory(string sDir)
        {
            DirectoryInfo diDir = new DirectoryInfo(sDir);

            foreach (DirectoryInfo subDirInfo in diDir.GetDirectories())
            {
                DeleteDirectory(subDirInfo.FullName);
            }

            foreach (FileInfo fileInfo in diDir.GetFiles())
            {
                try
                {
                    if ((fileInfo.Attributes & FileAttributes.ReadOnly) != 0)
                    {
                        fileInfo.Attributes = fileInfo.Attributes & ~FileAttributes.ReadOnly;
                    }
                    fileInfo.Delete();
                }
                catch {}
            }
            try
            {
                diDir.Attributes = diDir.Attributes & ~FileAttributes.ReadOnly;
                diDir.Delete(true);
            }
            catch {}
        }

        private void ReportFailedBackup(BUService service, string sReason, EventLogEntryType eventType)
        {
            // Asynchronous remote call to BUC.
            DateTime dt = DateTime.Now;

            string str = "Backup operation incomplete.";
            string str1 = String.Format("{0}  {1}", str, sReason);
            service.SendBUCMessage(MessageEventArgs.MsgType.MSG_FAIL, str, dt);
            service.LogBUStep(str1, dt);
            service.LogInfo("RunBackup", str1, dt);

            service.EventLog.WriteEntry(str1, eventType); // Log failure with reason.
        }

        // Thread method.
        private static void RunBackup(object obj)
        {
            BUService service = (BUService)obj;

            while (!bTerminate)
            {
                string str;
                try
                {
                    // Wait here til receive run backup request.
                    eventRun.WaitOne();
                    // Prevent another run while executing the current backup process.
                    service.IsRunning = true;

                    // Request to terminate thread loop may be initiated by OnStop or OnShutdown.
                    if (bTerminate) break;

                    service.ReadConfiguration();
                    
                    // Asynchronous remote call to BUC.
                    DateTime dt = DateTime.Now;
                    str = "Backup operation started.";
                    service.LogBUStep(str, dt);
                    service.LogInfo("RunBackup", str, dt);
                    service.SendBUCMessage(MessageEventArgs.MsgType.MSG_STARTED, str, dt);

                    try
                    {
                        // Create holding directory.

                        DirectoryInfo diExeDir = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).Directory;
                        string sDir = String.Format("TempFileDir{0}", DateTime.Now.ToString("yyyyMMdd-HHmmss"));
                        service.sHoldingDir = Path.Combine(diExeDir.FullName, sDir);

                        if (!Directory.Exists(service.sHoldingDir)) Directory.CreateDirectory(service.sHoldingDir);
                    }
                    catch (Exception ex)
                    {
                        service.ReportFailedBackup(service, ex.Message, EventLogEntryType.Error);
                        service.IsRunning = false;
                        continue;
                    }

                    try
                    {
                        service.BackUpSQLDBs();
                    }
                    catch (System.Exception ex)
                    {
                        service.ReportFailedBackup(service, ex.Message, EventLogEntryType.Error);
                        service.IsRunning = false;
                        continue;
                    }

                    try
                    {
                        service.CopyDBFiles();
                    }
                    catch (System.Exception ex)
                    {
                        service.ReportFailedBackup(service, ex.Message, EventLogEntryType.Error);
                        service.IsRunning = false;
                        continue;
                    }

                    try
                    {
                        service.ZipFiles();
                    }
                    catch (System.Exception ex)
                    {
                        service.ReportFailedBackup(service, ex.Message, EventLogEntryType.Warning);
                        service.IsRunning = false;
                        continue;
                    }

                    dt = DateTime.Now;
                    str = "Backup operation completed.";
                    service.LogBUStep(str, dt);
                    service.LogInfo("RunBackup", str, dt);
                    service.SendBUCMessage(MessageEventArgs.MsgType.MSG_COMPLETE, str, dt);

                    service.EventLog.WriteEntry(str); // Log final success.

                    // Delete holding directory.
                    try
                    {
                        service.DeleteDirectory(service.sHoldingDir);
                    }
                    catch (Exception ex)
                    {
                        service.LogInfo("RunBackup", ex.Message);
                    }
                    finally
                    {
                        service.sHoldingDir = null;
                    }

                    // Normal backup operation completed.
                    service.IsRunning = false;
                }
                catch (ThreadAbortException ex)
                {
                    str = "Received abort Backup Utility request.  Terminating thread.";
                    service.LogInfo("RunBackup", str);
                    service.LogBUStep(str);
                    service.IsRunning = false;
                }
            }
            service.LogInfo("RunBackup", "Terminating.");
        }

        #endregion // Multithread

        private void SetCheckKeyTimer()
        {
            if (timerCheckKey != null)
            {
                timerCheckKey.Dispose();
                timerCheckKey = null;
            }
            timerCheckKey = new Timer(new TimerCallback(CheckKeyTimerProc), this, 60000, 60000);
        }

        // Set a timer to create a log file every midnight.
        // This timer setting depends on the backup process timer, so,
        // rescheduling the backup process also requires a call to this method.
        private void SetCreateLogTimer()
        {
            if (timerCreateLog != null)
            {
                timerCreateLog.Dispose();
                timerCreateLog = null;
            }

            DateTime dtNextCreateLogTime;// = DateTime.Today.AddDays(1).AddSeconds(5); // Tomorrow 12:00:05 AM

            TimeSpan ts235500 = new TimeSpan(23, 55, 00);
            
            if (tsTimeOfDay >= ts235500 || tsTimeOfDay <= TimeSpan.FromMinutes(1))
            {
                // Backup process is scheduled to start at/after 11:55 PM or at/before 12:01 AM.

                dtNextCreateLogTime = DateTime.Today.AddDays(1).AddMinutes(5); // Tomorrow 12:05 AM
            }
            else
                dtNextCreateLogTime = DateTime.Today.AddDays(1).AddSeconds(5); // Tomorrow 12:00:05 AM

            // Calculate the difference between the next execution time and the current time.
            TimeSpan tsWait = dtNextCreateLogTime - DateTime.Now;

            timerCreateLog = new Timer(new TimerCallback(CreateLogTimerProc), this, tsWait, TimeSpan.FromDays(1));
        }

        // Create a timer to run TimerProc() at specific time.
        private void SetTimer()
        {
            if (timerBU != null)
            {
                timerBU.Dispose();
                timerBU = null;
            }

            DateTime dtStart = DateTime.Today + tsTimeOfDay;

            if (dtStart <= DateTime.Now) dtStart += TimeSpan.FromDays(1);

            // Calculate the difference between the specified execution time and the current time.
            TimeSpan tsWait = dtStart - DateTime.Now;

            timerBU = new Timer(new TimerCallback(TimerProc), this, tsWait, TimeSpan.FromDays(1));
        }

        protected override void OnStart(string[] args)
        {
            // OnStart must return within 30 seconds.

            LogInfo("OnStart", "Entered.");

            // This creates a log file if there isn't one.
            OpenBULog(DateTime.Now);

            if (!IsSecurityKeyPresent())
            {
                Stop();
                return;
            }

            // =================================================================================================
            // < BUC APPLICATION REMOTE SERVER RELATED CODE >
            try
            {
                // Register a TCP channel.
                ChannelServices.RegisterChannel(new TcpChannel(), false);

                roBUC = (FMBUCRemote)Activator.GetObject(
                                          typeof(FMBUCRemote),
                                          "tcp://localhost:50905/FMBUCRemote");
            }
            catch (Exception ex)
            {
                LogInfo("OnStart", ex.Message);
                EventLog.WriteEntry(ex.Message);
            }
            // =================================================================================================


            // ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
            // < BU SERVICE REMOTE SERVER RELATED CODE >
            try
            {
                // Register a TCP channel.
                
                // Default constructor of the channel creates a channel with a name "tcp", better to use a new name.
                System.Collections.IDictionary properties = new System.Collections.Hashtable();
                properties["port"] = 50906;
                properties["name"] = "BUTcp";

                ChannelServices.RegisterChannel(new TcpChannel(properties, null, null), false);
                
                // Create a remotable object and register it with the remoting service.
                roBU = new FMBURemote();
                ObjRef orFMBURemote = RemotingServices.Marshal(roBU, "FMBURemote");
                
                // Subscribe to message event raised by BU remote object.
                roBU.MessageToBUEvent += new FMBURemote.MessageToBUEventHandler(roBU_MessageToBUEvent);
            }
            catch (Exception ex)
            {
                LogInfo("OnStart", ex.Message);
                EventLog.WriteEntry(ex.Message);
            }
            // ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::


            bTerminate = false;
            threadBackup = null;

            // Create a new thread and pass this service object to it.
            threadBackup = new Thread(RunBackup);
            if (threadBackup != null)
            {
                ReadScheduledTime();

                threadBackup.Start(this);

                // Create timers to run timer procedures at specific times.
                SetTimer();
                SetCheckKeyTimer();
                SetCreateLogTimer();
            }
        }

        protected override void OnStop()
        {
//            LogBUStep("Received Stop Service request.");
            LogInfo("OnStop", "Received Stop Service request.");

            if (timerCheckKey != null)
            {
                timerCheckKey.Dispose();
                timerCheckKey = null;
            }
            
            if (timerBU != null)
            {
                timerBU.Dispose();
                timerBU = null;
            }

            if (timerCreateLog != null)
            {
                timerCreateLog.Dispose();
                timerCreateLog = null;
            }

            this.RequestAdditionalTime(6000);

            bTerminate = true;
            eventRun.Set();
            if (threadBackup != null && !threadBackup.Join(3000))
            {
                LogInfo("OnStop", "Timed out waiting for thread to exit.");
                threadBackup.Abort(); //Abort(this);
                threadBackup.Join(2000);
            }

            // Close() closes the current StreamWriter object and the underlying stream.
            // This calls the Dispose method passing a true value.
//            sw.Close();
//            sw = null;

//            swBULog.Close();
//            swBULog = null;
        }

        protected override void OnShutdown()
        {
//            LogBUStep("Received Shutdown Service request.");
            LogInfo("OnShutdown", "Received Shutdown Service request.");

            if (timerCheckKey != null)
            {
                timerCheckKey.Dispose();
                timerCheckKey = null;
            }

            if (timerBU != null)
            {
                timerBU.Dispose();
                timerBU = null;
            }

            if (timerCreateLog != null)
            {
                timerCreateLog.Dispose();
                timerCreateLog = null;
            }

            this.RequestAdditionalTime(6000);

            bTerminate = true;
            eventRun.Set();
            if (threadBackup != null && !threadBackup.Join(3000))
            {
                LogInfo("OnShutdown", "Timed out waiting for thread to exit.");
                threadBackup.Abort(); //Abort(this);
                threadBackup.Join(2000);
            }

            // Close() closes the current StreamWriter object and the underlying stream.
            // This calls the Dispose method passing a true value.
//            sw.Close();
//            sw = null;

//            swBULog.Close();
//            swBULog = null;
        }
    }
}

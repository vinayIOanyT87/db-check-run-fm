namespace DataMigration
{
	using System;
	using System.Data;
	using System.Data.SqlClient;
	using System.Diagnostics;
	using System.IO;
	using System.Windows.Forms;

	using FMBusinessObjects.LogClient;

	using ICSharpCode.SharpZipLib.Zip;

	using Microsoft.SqlServer.Management.Smo;
	using Microsoft.Win32;

	class DAService
    {
        #region Constructors
        /// <summary>
        /// This is the default constructor for the ARTS Processor.
        /// </summary>
        private Logger logger;
        public DAService()
		{
            logger = new Logger("Data Migration DAService");
		}
		#endregion

        private int dataMigrationType = 0;
        public int DataMigrationType
        {
            get { return dataMigrationType; }
            set { dataMigrationType = value; }
        }
        private string error = "";
        public string Error
        {
            get { return error; }
            set { error = value; }
        }

        private static string site;
        public static string Site
        {
            get { return site; }
            set { site = value; }
        }

        public static string ConsolidatedDBBackupFile
        {
            get
            {
                DirectoryInfo diExeDir = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).Directory;
                return Path.Combine(diExeDir.FullName, "ConsolidatedDB.bak");
            }
        }

        public static string ServerName 
        {
           get
           {
                string serverName;
                int iIndex = DAService.ConnectionString.ToUpper().IndexOf("SERVER=");
                if (iIndex < 0)
                    iIndex = DAService.ConnectionString.ToUpper().IndexOf("DATA SOURCE=");
                if (iIndex < 0)
                    serverName = "127.0.0.1";
                else
                {
                    serverName = DAService.ConnectionString.Substring(iIndex);
                    iIndex = serverName.IndexOf(';');
                    serverName = serverName.Remove(iIndex);
                    if (serverName.ToUpper().IndexOf("SERVER=") >= 0) 
                        serverName = serverName.ToUpper().Replace("SERVER=", "");
                    else
                        serverName = serverName.ToUpper().Replace("DATA SOURCE=", "");
                }
                return serverName.Trim();
            }
        }

        public static string ConnectionString
        {
           get
           {
              string ValueString = "ConsolidatedConnectionString";
              RegistryKey Key = Registry.LocalMachine.CreateSubKey("Software\\Varec\\SharedComponents");
              string Connect = (string)Key.GetValue(ValueString);
              if (Connect == null)
              {
                 Connect = "Persist Security Info=False;Integrated Security=SSPI;database=ConsolidatedDB;Server=127.0.0.1;Connect Timeout=30";
                 Key.SetValue(ValueString, Connect);
              }
              return Connect;
           }
        }
       
        public static string getConnectionString(string db)
        {
            string cnnectionString = DAService.ConnectionString;
            if (db != "ConsolidatedDB")
                return cnnectionString.Replace("ConsolidatedDB", db);
            return cnnectionString;
        }

        public DataSet GetSites(string db)
        {
            string strSQL = "";
            if (db == "ConsolidatedDB6")
                strSQL = "SELECT SiteID FROM tblSites ";
            else
                strSQL = "SELECT ID FROM tblSites ";
            
            return ExcuteSQL(db, strSQL);
        }

        public void RestoreDB(string dbName, string dbFile)
        {
            Cursor.Current = Cursors.WaitCursor;
            System.Diagnostics.Trace.WriteLine(String.Format("RestoreDB. {0}", dbName));

            Microsoft.SqlServer.Management.Smo.Server server =
                new Microsoft.SqlServer.Management.Smo.Server(DAService.ServerName);

            Microsoft.SqlServer.Management.Smo.Database database = 
                new Microsoft.SqlServer.Management.Smo.Database(server, dbName);
            if (server != null && database != null)
            {

                try
                {
                    //If Need
                    if (server.Databases[dbName] == null)
                    {
                        database.Create();
                        database.Refresh();
                    }
                    //Restoring
                    Restore restore = new Restore();
                    restore.NoRecovery = false;
                    restore.Action = RestoreActionType.Database;
                    BackupDeviceItem bdi = default(BackupDeviceItem);
                    bdi = new BackupDeviceItem(dbFile, DeviceType.File);
                    restore.Devices.Add(bdi);
                    restore.Database = dbName;
                    restore.ReplaceDatabase = true; 

                    restore.PercentCompleteNotification = 10;
                    restore.SqlRestore(server);
                }
                catch (Exception ex)
                {
                    this.Error = ex.Message;
                    this.logger.Error("DAService: RestoreDB. " + ex.Message);        
                    System.Diagnostics.Trace.WriteLine(String.Format("RestoreDB. {0}", ex.Message));
                }
                finally
                {
                    database.Refresh();
                    server.Refresh();                    
                    Cursor.Current = Cursors.Default;
                }
            }
        }

        public void RestoreBaseDB(string FileName)
        {
            try
            {
                FastZip fz = new FastZip();
                DirectoryInfo diExeDir = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).Directory;
                string sUnzipDir = "TempUnZipDir";
                string sTempZipDir = Path.Combine(diExeDir.FullName, sUnzipDir);
                if (!Directory.Exists(sTempZipDir))
                    Directory.CreateDirectory(sTempZipDir);
                fz.ExtractZip(FileName, sTempZipDir, "");

                ProcessRestoreDB(Path.Combine(sTempZipDir, "ConsolidatedDB.bak"));
                ProcessRestoreDB(Path.Combine(sTempZipDir, "AccountingDB.bak"));
                ProcessRestoreDB(Path.Combine(sTempZipDir, "AviationDB.bak"));

                if (dataMigrationType == 0)
                {
                    ProcessRestoreDB(Path.Combine(sTempZipDir, "FMArchive.bak"));
                    ProcessRestoreDB(Path.Combine(sTempZipDir, "Movement.bak"));
                }

                Directory.Delete(sTempZipDir, true);
            }
            catch (Exception ex)
            {
                this.logger.Error("DAService: RestoreBaseDB. " + ex.Message);     
            }
        }

        public void ProcessRestoreDB(string FileName)
        {
            if (File.Exists(FileName))
            {
                string dbname = Path.GetFileNameWithoutExtension(FileName);
                RestoreDB(dbname + '6', FileName);
            }
        }

        public DataSet DuplicatedUsers(string db)
        {
            string strSQL = ""; 

            if(db == "ConsolidatedDB")
            {
                if (DAService.Site == null || DAService.Site.ToUpper() == "ALL SITES" )
                    strSQL = "SELECT distinct a.UserID FROM ConsolidatedDB6.dbo.tblUsers a inner join ConsolidatedDB.dbo.tblUsers b ON a.UserID=b.UserID WHERE a.DeleteFlag=0 ";
                else if (DAService.Site != null)
                {
                    strSQL = "SELECT distinct a.UserID FROM ConsolidatedDB6.dbo.tblUsers a inner join ConsolidatedDB.dbo.tblUsers b ON a.UserID=b.UserID " +
                              "WHERE a.DeleteFlag=0 and a.SiteIndex in " +
                              "( SELECT a.SiteIndex FROM ConsolidatedDB6.dbo.tblUsers JOIN ConsolidatedDB6.dbo.tblSites ON ConsolidatedDB6.dbo.tblSites.SiteIndex = a.SiteIndex " +
                              "WHERE ConsolidatedDB6.dbo.tblSites.SiteID = '";
                    strSQL += DAService.Site;
                    strSQL += "') ";
                }
            }
            else
                strSQL = "SELECT UserID, COUNT(UserID) AS Num FROM tblUsers WHERE DeleteFlag= 0 GROUP BY UserID HAVING ( COUNT(UserID) > 1 )";

            return ExcuteSQL(db, strSQL);
        }

        public DataSet DuplicatedSites(string site)
        {
            string strSQL = "";
            if (site.ToUpper() == "ALL SITES")
                strSQL = "SELECT a.SiteID FROM ConsolidatedDB6.dbo.tblSites a inner join ConsolidatedDB.dbo.tblSites b  ON a.SiteID=b.ID where a.SiteID <> 'SiteAdmin'";
            else
            {
                strSQL = "SELECT ID FROM tblSites where ID = '";
                strSQL += site;
                strSQL += "'";
            }
            
            return ExcuteSQL("ConsolidatedDB", strSQL);
        }
        public DataSet ExcuteSQL(string db, string strSQL)
        {
            System.Diagnostics.Trace.WriteLine(String.Format("ExcuteSQL. {0}", strSQL));
            DataSet dataSet = new DataSet();
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = DAService.getConnectionString(db);
                System.Diagnostics.Trace.WriteLine(String.Format("ExcuteSQL. {0}", connection.ConnectionString));
                try
                {
                    // Open the database connection.
                    connection.Open();

                    if (connection.State == ConnectionState.Open)
                    {
                        // Create and configure a new command.                       
                        SqlDataAdapter Adapter = new SqlDataAdapter(strSQL, connection);
                        Adapter.Fill(dataSet);
                    }
                    else
                    {
                        string msg;
                        msg = "Could not open Database ";
                        msg += db;
                        MessageBox.Show(msg, "FuelsManager Data Migration", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);                        
                        this.logger.Error("DAService: ExcuteSQL. " + "Could not open Database.");  
                        System.Diagnostics.Trace.WriteLine("Could not open Database.");
                    }
                }
                catch (SqlException e)
                {
                    this.logger.Error("DAService: ExcuteStoredProcedure. " + e.Message);
                    System.Diagnostics.Trace.WriteLine(String.Format("ExcuteStoredProcedure. {0}", e.Message));
                }
                catch (System.Exception ex)
                {
                    this.logger.Error("DAService: ExcuteSQL. " + ex.Message);     
                    System.Diagnostics.Trace.WriteLine(String.Format("ExcuteSQL. {0}", ex.Message));
                }
                connection.Close();

                // At the end of the using block Dispose() calls Close().
            }
            return dataSet;
        }

        public string ExcuteStoredProcedure(string Procedure, string Parm, string DBName, int iDropProcedure)
        {
            System.Diagnostics.Trace.WriteLine(String.Format("ExcuteStoredProcedure. {0}", Procedure));
            string err = "";
            SqlConnection connection = new SqlConnection(DAService.getConnectionString(DBName));
            SqlConnection.ClearAllPools();
            SqlCommand command = new SqlCommand();
            int iSel = (int)DataMigrationWizardSheet.DataMigrationSel;
            try
            {
                connection.Open();
                if (connection.State == ConnectionState.Open)
                {
                    command.Connection = connection;
                    command.CommandTimeout = 0;
                    command.CommandText = Procedure;
                    command.CommandType = CommandType.StoredProcedure;
                    if (DBName == "Master")
                    {
                        command.Parameters.AddWithValue("@dbName", Parm);
                        command.Parameters.AddWithValue("@bDropProcedure", iDropProcedure);
                    }
                    else
                    {
                        if (Parm != "")
                        {
                            command.Parameters.AddWithValue("@SiteID", Parm);
                            command.Parameters.AddWithValue("@IsBaseDB", iSel);
                        }
                    }

                    if ((Procedure == "dbo.Migration_DropStuff") && (iSel == 2))
                        command.Parameters.AddWithValue("@IsBaseDB", false);

                    command.ExecuteNonQuery();
                }
                else
                {
                    this.logger.Error("DAService: ExcuteStoredProcedure. " + "Could not open Database.");
                    System.Diagnostics.Trace.WriteLine("Could not open Database.");
                }
            }
            catch (SqlException e)
            {
                err = e.Message;
                this.logger.Error("DAService: ExcuteStoredProcedure. " + e.Message);
                System.Diagnostics.Trace.WriteLine(String.Format("ExcuteStoredProcedure. {0}", e.Message));
            }
            catch (Exception e)
            {
                err = e.Message;
                this.logger.Error("DAService: ExcuteStoredProcedure. " + e.Message);
                System.Diagnostics.Trace.WriteLine(String.Format("ExcuteStoredProcedure. {0}", e.Message));
            }
            finally
            {
                command.Connection.Close();
            }
            return err;
        }

        public void BackupDabase8(string BackupFile)
        {
            System.Diagnostics.Trace.WriteLine(String.Format("BackupDabase8. {0}", BackupFile));
            Backup sqlBackup = new Backup();
            sqlBackup.Action = BackupActionType.Database;
            sqlBackup.BackupSetDescription = "ArchiveDataBase:" + DateTime.Now.ToShortDateString();
            sqlBackup.BackupSetName = "ConsolidatedDBArchive";
            sqlBackup.Database = "ConsolidatedDB";
            BackupDeviceItem deviceItem = new BackupDeviceItem(BackupFile, DeviceType.File);
            Microsoft.SqlServer.Management.Smo.Server sqlServer = 
                                                      new Microsoft.SqlServer.Management.Smo.Server(DAService.ServerName);
            if (sqlServer != null && sqlServer.Databases["ConsolidatedDB"] != null)
            {
                try
                {
                    sqlBackup.Initialize = true;
                    sqlBackup.Checksum = true;
                    sqlBackup.ContinueAfterError = true;
                    sqlBackup.Devices.Add(deviceItem);
                    sqlBackup.Incremental = false;
                    sqlBackup.ExpirationDate = DateTime.Now.AddDays(3);
                    sqlBackup.LogTruncation = BackupTruncateLogType.Truncate;
                    sqlBackup.FormatMedia = false;
                    sqlBackup.SqlBackup(sqlServer);
                }
                catch (Exception ex)
                {
                    this.Error = ex.Message;
                    this.logger.Error("DAService: BackupDabase8. " + ex.Message); 
                    System.Diagnostics.Trace.WriteLine(String.Format("BackupDabase8. {0}", ex.Message));                   
                }
                finally
                {
                    sqlServer.Refresh();
                }
            }

        }

        public void RestoreStoredProcedures(bool all)
        {
            try
            {
               if(all)
                   RestoreStoredProceduresProcess("for %f in (*.sql) do sqlcmd /S \"" + ServerName + "\" -i %f -t 0");           
               else
               {
                   RestoreStoredProceduresProcess("sqlcmd /S \"" + ServerName + "\" -i Migration_ClearDBUsers.sql");      
                   RestoreStoredProceduresProcess("sqlcmd /S \"" + ServerName + "\" -i Migration_DropStuff.sql");   
               }
               
            }

            catch (Exception ex)
            {
                this.logger.Error("DAService: RestoreStoredProcedures. " + ex.Message);
                System.Diagnostics.Trace.WriteLine(String.Format("RestoreStoredProcedures. {0}", ex.Message));
                throw ex;
            }
        }

        public void RestoreStoredProceduresProcess(string cmd)
        {
            try
            {
                //  Directory.SetCurrentDirectory(@"C:\FuelsManager Envision\Web Application\Database - Development\Data Migration");
                // RegistryKey Key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Varec\\InstallDetails", false);
                DirectoryInfo diExeDir = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).Directory;
                string DataMigrationProceduresPath = Path.Combine(diExeDir.FullName, "Data Migration");
                Directory.SetCurrentDirectory(DataMigrationProceduresPath);

                ProcessStartInfo procStartInfo = new ProcessStartInfo("cmd", "/c " + cmd);                
                procStartInfo.RedirectStandardOutput = true;
                procStartInfo.UseShellExecute = false;
                procStartInfo.CreateNoWindow = true;
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo = procStartInfo;
                proc.Start();
                string result = proc.StandardOutput.ReadToEnd();
            }

            catch (Exception ex)
            {
                this.logger.Error("DAService: RestoreStoredProceduresProcess. " + ex.Message);
                System.Diagnostics.Trace.WriteLine(String.Format("RestoreStoredProceduresProcess. {0}", ex.Message));
                throw ex;
            }
        }
    }
}

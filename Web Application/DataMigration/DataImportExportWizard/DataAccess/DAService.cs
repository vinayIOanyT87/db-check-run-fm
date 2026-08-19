// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DAService.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   The da service.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DataImportExportWizard.DataAccess
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Windows.Forms;

    using DataImportExportWizard.Constants;
    using DataImportExportWizard.InternalClasses;
    using DataImportExportWizard.InternalClasses.LogClient;

    using ICSharpCode.SharpZipLib.Zip;

    using Microsoft.SqlServer.Management.Smo;
    using Microsoft.Win32;

    /// <summary>
    /// The da service.
    /// </summary>
    internal class DAService
    {
        #region Static Attributes
        /// <summary>
        /// The siteId.
        /// </summary>
        private static string siteId;

        /// <summary>
        /// The database backed up.
        /// </summary>
        private static bool databaseBackedUpFlag = false;

        #endregion Static Attributes

        #region Attributes

        /// <summary>
        /// The loggerInstance.
        /// </summary>
        private readonly Logger loggerInstance;

        /// <summary>
        /// The errorMessage.
        /// </summary>
        private string errorMessage = string.Empty;

        #endregion Attributes

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DAService"/> class.
        /// </summary>
        public DAService()
        {
            this.SelectedImportExportType = DataImportExportActionType.ExportKeys;
            this.loggerInstance = new Logger(string.Format("{0}_DAService", StringConstants.ApplicationShortName));
        }

        #endregion Constructors

        #region Properties

        #region Static Properties
        /// <summary>
        /// Gets or sets the site.
        /// </summary>
        public static string SiteId
        {
            get
            {
                return DAService.siteId;
            }

            set
            {
                DAService.siteId = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether database backed up flag.
        /// </summary>
        public static bool DatabaseBackedUpFlag
        {
            get
            {
                return DAService.databaseBackedUpFlag;
            }

            internal set
            {
                DAService.databaseBackedUpFlag = value;
            }
        }

        /// <summary>
        /// Gets the consolidated databaseName backup file.
        /// </summary>
        public static string ConsolidatedDatabaseBackupFile
        {
            get
            {
                DirectoryInfo directoryInfo = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).Directory;
                string filename = string.Format("{0}.bak", DAService.DatabaseName);

                if (null != directoryInfo)
                {
                    return Path.Combine(directoryInfo.FullName, filename);
                }
                else
                {
                    return filename;
                }
            }
        }

        /// <summary>
        /// Gets the server name.
        /// </summary>
        public static string ServerName
        {
            get
            {
                string instance = DataImportExportWizardOption.InstanceName;

                if (string.IsNullOrEmpty(instance))
                {
                    string defaultConnection = VarecRegistryConnectionString;

                    if (!string.IsNullOrEmpty(defaultConnection))
                    {
                        DbConnectionStringBuilder builder = new DbConnectionStringBuilder();
                        builder.ConnectionString = defaultConnection.ToUpper();

                        if (null != builder["SERVER"])
                        {
                            instance = builder["SERVER"].ToString();
                        }
                        else if (null != builder["DATA SOURCE"])
                        {
                            instance = builder["DATA SOURCE"].ToString();
                        }

                        builder = null;
                    }
                }

                if (string.IsNullOrEmpty(instance))
                {
                    instance = StringConstants.InstanceName;
                }

                return instance;
            }
        }

        /// <summary>
        /// Gets the database name.
        /// </summary>
        public static string DatabaseName
        {
            get
            {
                string database = DataImportExportWizardOption.DatabaseName;

                if (string.IsNullOrEmpty(database))
                {
                    string defaultConnection = VarecRegistryConnectionString;

                    if (!string.IsNullOrEmpty(defaultConnection))
                    {
                        DbConnectionStringBuilder builder = new DbConnectionStringBuilder();
                        builder.ConnectionString = defaultConnection.ToUpper();

                        if (null != builder["DATABASE"])
                        {
                            database = builder["DATABASE"].ToString();
                        }

                        builder = null;
                    }
                }

                if (string.IsNullOrEmpty(database))
                {
                    database = StringConstants.DatabaseName;
                }

                return database;
            }
        }

        private static string VarecRegistryConnectionString
        {
            get
            {
                const string ValueString = "ConsolidatedConnectionString";
                RegistryKey key = Registry.LocalMachine.CreateSubKey("Software\\Varec\\SharedComponents");

                string connString = string.Empty;

                if (null != key)
                {
                    connString = (string)key.GetValue(ValueString);
                    key.Close();
                }

                return connString;
            }
        }
        #endregion Static Properties

        #region Non-Static Properties

        /// <summary>
        /// Gets or sets the selected import export type
        /// </summary>
        public DataImportExportActionType SelectedImportExportType { get; set; }

        /// <summary>
        /// Gets or sets the error.
        /// </summary>
        public string ErrorMessage
        {
            get
            {
                return this.errorMessage;
            }

            set
            {
                this.errorMessage = value;
            }
        }
        #endregion Non-Static Properties

        #endregion Properties

        /// <summary>
        /// The get connection string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetConnectionString()
        {
            return GetConnectionString(ServerName, DatabaseName);
        }

        /// <summary>
        /// The get connection string.
        /// </summary>
        /// <param name="databaseName">
        /// The database name on the default server instance.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetConnectionString(string databaseName)
        {
            return GetConnectionString(ServerName, databaseName);
        }

        /// <summary>
        /// The get connection string.
        /// </summary>
        /// <param name="serverName">
        /// The server name.
        /// </param>
        /// <param name="databaseName">
        /// The database name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Throws an exception if either one of the passed in arguments are null.
        /// </exception>
        public static string GetConnectionString(string serverName, string databaseName)
        {
            if (string.IsNullOrEmpty(serverName))
            {
                throw new ArgumentNullException("serverName");
            }

            if (string.IsNullOrEmpty(databaseName))
            {
                throw new ArgumentNullException("databaseName");
            }

            return string.Format(@"Persist Security Info=False;Integrated Security=SSPI;server={0};database={1};Connect Timeout=60", serverName, databaseName);
        }

        /// <summary>
        /// The get sites.
        /// </summary>
        /// <returns>
        /// The <see cref="DataSet"/>.
        /// </returns>
        public DataSet GetSites()
        {
            return this.ExecuteSql(DAService.DatabaseName, "SELECT ID FROM [dbo].[tblSites] ORDER BY ID ");
        }

        /// <summary>
        /// The get site GUID.
        /// </summary>
        /// <param name="siteId">
        /// The site id.
        /// </param>
        /// <returns>
        /// The <see cref="Guid"/>.
        /// </returns>
        public Guid GetSiteGuid(string siteId)
        {
            Guid siteGuid = Guid.Empty;

            Trace.WriteLine(string.Format("GetSiteGuid. {0}", siteId));
            DataSet dataSet = new DataSet();

            using (var connection = new SqlConnection())
            {
                connection.ConnectionString = DAService.GetConnectionString();
                Trace.WriteLine(string.Format("GetSiteGuid. {0}", connection.ConnectionString));
                try
                {
                    // Open the database connection.
                    connection.Open();

                    if (connection.State == ConnectionState.Open)
                    {
                        // Create and configure a new command.                       
                        SqlCommand cmd = new SqlCommand("SELECT SiteGuid FROM [dbo].[tblSites] WHERE ID = @ID");
                        cmd.Connection = connection;
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@ID", siteId);

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        adapter.Fill(dataSet);

                        if (dataSet.Tables[0].Rows.Count == 0)
                        {
                            siteGuid = Guid.Empty;
                        }
                        else
                        {
                            DataRow row = dataSet.Tables[0].Rows[0];
                            siteGuid = DAService.GetGuid(row["SiteGuid"]);
                        }
                    }
                    else
                    {
                        string msg = "Could not open Database ";
                        msg += DAService.DatabaseName;
                        MessageBox.Show(msg, StringConstants.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        this.loggerInstance.Error("DAService: ExecuteSql. " + "Could not open Database.");
                        Trace.WriteLine("Could not open Database.");
                    }
                }
                catch (SqlException e)
                {
                    this.loggerInstance.Error("DAService: ExecuteStoredProcedure. " + e.Message);
                    Trace.WriteLine(string.Format("ExecuteStoredProcedure. {0}", e.Message));
                }
                catch (Exception ex)
                {
                    this.loggerInstance.Error("DAService: ExecuteSql. " + ex.Message);
                    Trace.WriteLine(string.Format("ExecuteSql. {0}", ex.Message));
                }

                connection.Close();

                // At the end of the using block Dispose() calls Close().
            }

            return siteGuid;
        }

        /// <summary>
        /// The execute sql statement.
        /// </summary>
        /// <param name="databaseName">
        /// Name of the database to execute the sql statement against.
        /// </param>
        /// <param name="strSql">
        /// Sql statement that needs to be executed.
        /// </param>
        /// <returns>
        /// The <see cref="DataSet"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        public DataSet ExecuteSql(string databaseName, string strSql)
        {
            Trace.WriteLine(string.Format("ExecuteSql. {0}", strSql));
            DataSet dataSet = new DataSet();

            using (var connection = new SqlConnection())
            {
                connection.ConnectionString = DAService.GetConnectionString(databaseName);
                Trace.WriteLine(string.Format("ExecuteSql. {0}", connection.ConnectionString));
                try
                {
                    // Open the database connection.
                    connection.Open();

                    if (connection.State == ConnectionState.Open)
                    {
                        // Create and configure a new command.                       
                        SqlDataAdapter adapter = new SqlDataAdapter(strSql, connection);
                        adapter.Fill(dataSet);
                    }
                    else
                    {
                        string msg = "Could not open Database ";
                        msg += databaseName;
                        MessageBox.Show(msg, StringConstants.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        this.loggerInstance.Error("DAService: ExecuteSql. " + "Could not open Database.");
                        Trace.WriteLine("Could not open Database.");
                    }
                }
                catch (SqlException e)
                {
                    this.loggerInstance.Error("DAService: ExecuteStoredProcedure. " + e.Message);
                    Trace.WriteLine(string.Format("ExecuteStoredProcedure. {0}", e.Message));
                    throw new Exception("ExecuteSql SQLException", e);
                }
                catch (Exception ex)
                {
                    this.loggerInstance.Error("DAService: ExecuteSql. " + ex.Message);
                    Trace.WriteLine(string.Format("ExecuteSql. {0}", ex.Message));
                    throw new Exception("Execute Sql Exception", ex);
                }

                connection.Close();

                // At the end of the using block Dispose() calls Close().
            }

            return dataSet;
        }

        /// <summary>
        /// The execute stored procedure.
        /// </summary>
        /// <param name="procedureName">
        /// The procedure.
        /// </param>
        /// <param name="parameterList">
        /// The parameters
        /// </param>
        /// <param name="databaseName">
        /// The database name.
        /// </param>
        /// <param name="removeOnExitFlag">
        /// A flag that indicates whether the stored procedure should be automatically removed upon completion.  The stored procedure should be scripted so that it 
        /// accepts this flag and removes itself prior to exiting.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string ExecuteStoredProcedure(string procedureName, string parameterList, string databaseName, bool removeOnExitFlag)
        {
            Trace.WriteLine(string.Format("ExecuteStoredProcedure. {0}", procedureName));
            string err = string.Empty;
            SqlConnection connection = new SqlConnection(DAService.GetConnectionString(databaseName));
            SqlConnection.ClearAllPools();
            SqlCommand command = new SqlCommand();

            try
            {
                connection.Open();
                if (connection.State == ConnectionState.Open)
                {
                    command.Connection = connection;
                    command.CommandTimeout = 0;
                    command.CommandText = procedureName;
                    command.CommandType = CommandType.StoredProcedure;
                    if (databaseName.Equals("Master", StringComparison.InvariantCultureIgnoreCase))
                    {
                        command.Parameters.AddWithValue("@databaseName", parameterList);
                        command.Parameters.AddWithValue("@removeOnExitFlag", removeOnExitFlag ? 1 : 0);
                    }
                    else
                    {
                        if (parameterList != string.Empty)
                        {
                            command.Parameters.AddWithValue("@SiteID", parameterList);

                            // command.Parameters.AddWithValue("@IsBaseDB", sel);
                        }
                    }

                    // if ((procedureName == "dbo.Migration_DropStuff") && (sel == 2))
                    // {
                    //     command.Parameters.AddWithValue("@IsBaseDB", false);
                    // }
                    command.ExecuteNonQuery();
                }
                else
                {
                    this.loggerInstance.Error("DAService: ExecuteStoredProcedure. " + "Could not open Database.");
                    Trace.WriteLine("Could not open Database.");
                }
            }
            catch (SqlException e)
            {
                err = e.Message;
                this.loggerInstance.Error("DAService: ExecuteStoredProcedure. " + e.Message);
                Trace.WriteLine(string.Format("ExecuteStoredProcedure. {0}", e.Message));
            }
            catch (Exception e)
            {
                err = e.Message;
                this.loggerInstance.Error("DAService: ExecuteStoredProcedure. " + e.Message);
                Trace.WriteLine(string.Format("ExecuteStoredProcedure. {0}", e.Message));
            }
            finally
            {
                command.Connection.Close();
            }

            return err;
        }

        /// <summary>
        /// Performs a backup of the default database (ConsolidatedDB).
        /// </summary>
        public void BackupDatabase()
        {
            this.BackupDatabase(DAService.DatabaseName, DAService.ConsolidatedDatabaseBackupFile);
        }

        /// <summary>
        /// Performs a backup of the default database (ConsolidatedDB).
        /// </summary>
        /// <param name="backupFile">
        /// Name of the backup file.
        /// </param>
        public void BackupDatabase(string backupFile)
        {
            this.BackupDatabase(DAService.DatabaseName, backupFile);
        }

        /// <summary>
        /// Performs a backup of the specified database.
        /// </summary>
        /// <param name="databaseName">
        /// Name of the database to backup.
        /// </param>
        /// <param name="backupFile">
        /// Name of the backup file.
        /// </param>
        public void BackupDatabase(string databaseName, string backupFile)
        {
            if (DAService.DatabaseBackedUpFlag)
            {
                Trace.WriteLine(string.Format("BackupDatabase. {0}", backupFile));

                Backup sqlBackup = new Backup();
                sqlBackup.Action = BackupActionType.Database;
                sqlBackup.BackupSetDescription = "ArchiveDataBase:" + DateTime.Now.ToShortDateString();
                sqlBackup.BackupSetName = string.Format("{0}Archive", databaseName);
                sqlBackup.Database = databaseName;

                BackupDeviceItem deviceItem = new BackupDeviceItem(backupFile, DeviceType.File);
                Server sqlServer = new Server(DAService.ServerName);

                if (sqlServer.Databases[databaseName] != null)
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

                        DAService.DatabaseBackedUpFlag = true;
                    }
                    catch (Exception ex)
                    {
                        DAService.DatabaseBackedUpFlag = false;

                        this.errorMessage = ex.Message;
                        this.loggerInstance.Error("DAService: BackupDatabase. " + ex.Message);
                        Trace.WriteLine(string.Format("BackupDatabase. {0}", ex.Message));
                    }
                    finally
                    {
                        sqlServer.Refresh();
                    }
                }
            }
        }

        /// <summary>
        /// The restore databaseName.
        /// </summary>
        /// <param name="databaseName">
        /// The databaseName name.
        /// </param>
        /// <param name="filename">
        /// The databaseName file.
        /// </param>
        public void RestoreDatabase(string databaseName, string filename)
        {
            if (DAService.DatabaseBackedUpFlag)
            {
                Cursor.Current = Cursors.WaitCursor;
                Trace.WriteLine(string.Format("RestoreDatabase. {0}", databaseName));

                Server server = new Server(DAService.ServerName);
                Database database = new Database(server, databaseName);

                try
                {
                    // If Need
                    if (server.Databases[databaseName] == null)
                    {
                        database.Create();
                        database.Refresh();
                    }

                    // Restoring
                    Restore restore = new Restore { NoRecovery = false, Action = RestoreActionType.Database };
                    BackupDeviceItem bdi = default(BackupDeviceItem);
                    bdi = new BackupDeviceItem(filename, DeviceType.File);
                    restore.Devices.Add(bdi);
                    restore.Database = databaseName;
                    restore.ReplaceDatabase = true;

                    restore.PercentCompleteNotification = 10;
                    restore.SqlRestore(server);

                    DAService.DatabaseBackedUpFlag = false;
                }
                catch (Exception ex)
                {
                    this.errorMessage = ex.Message;
                    this.loggerInstance.Error("DAService: RestoreDatabase. " + ex.Message);
                    Trace.WriteLine(string.Format("RestoreDatabase. {0}", ex.Message));
                }
                finally
                {
                    database.Refresh();
                    server.Refresh();
                    Cursor.Current = Cursors.Default;
                }
            }
        }

        /// <summary>
        /// The restore base database.
        /// </summary>
        /// <param name="fileName">
        /// The filename.
        /// </param>
        public void RestoreBaseDatabase(string fileName)
        {
            if (DAService.DatabaseBackedUpFlag)
            {
                try
                {
                    const string UnzipDir = "TempUnZipDir";

                    FastZip fz = new FastZip();
                    DirectoryInfo directoryInfo =
                        new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).Directory;

                    if (null != directoryInfo)
                    {
                        string tempZipDir = Path.Combine(directoryInfo.FullName, UnzipDir);

                        if (!Directory.Exists(tempZipDir))
                        {
                            Directory.CreateDirectory(tempZipDir);
                        }

                        fz.ExtractZip(fileName, tempZipDir, string.Empty);

                        this.ProcessRestoreDatabase(
                            Path.Combine(tempZipDir, string.Format("{0}.bak", DAService.DatabaseName)));

                        Directory.Delete(tempZipDir, true);
                    }
                    else
                    {
                        throw new Exception("Unable to retrieve working directory.");
                    }
                }
                catch (Exception ex)
                {
                    this.loggerInstance.Error("DAService: RestoreBaseDatabase. " + ex.Message);
                }
            }
        }

        /// <summary>
        /// The process restore databaseName.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        public void ProcessRestoreDatabase(string fileName)
        {
            if (File.Exists(fileName))
            {
                string dbname = Path.GetFileNameWithoutExtension(fileName);
                this.RestoreDatabase(dbname + "8", fileName);
            }
        }

        /// <summary>
        /// The restore stored procedures.
        /// </summary>
        /// <param name="all">
        /// The all.
        /// </param>
        public void RestoreStoredProcedures(bool all)
        {
            try
            {
                if (all)
                {
                    this.RestoreStoredProceduresProcess(
                        "for %f in (*.sql) do sqlcmd /S \"" + ServerName + "\" -i %f -t 0");
                }
                else
                {
                    this.RestoreStoredProceduresProcess(
                        "sqlcmd /S \"" + ServerName + "\" -i Migration_ClearDBUsers.sql");

                    this.RestoreStoredProceduresProcess("sqlcmd /S \"" + ServerName + "\" -i Migration_DropStuff.sql");
                }

            }
            catch (Exception ex)
            {
                this.loggerInstance.Error("DAService: RestoreStoredProcedures. " + ex.Message);
                Trace.WriteLine(string.Format("RestoreStoredProcedures. {0}", ex.Message));
                throw;
            }
        }

        /// <summary>
        /// The restore stored procedures process.
        /// </summary>
        /// <param name="commandString">
        /// The commandString.
        /// </param>
        public void RestoreStoredProceduresProcess(string commandString)
        {
            try
            {
                // Directory.SetCurrentDirectory(@"C:\FuelsManager Envision\Web Application\Database - Development\Data Migration");
                // RegistryKey Key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Varec\\InstallDetails", false);
                DirectoryInfo directoryInfo = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).Directory;
                
                if (null != directoryInfo)
                {
                    string dataMigrationProceduresPath = Path.Combine(directoryInfo.FullName, "DBScripts");
                    Directory.SetCurrentDirectory(dataMigrationProceduresPath);

                    ProcessStartInfo procStartInfo = new ProcessStartInfo("commandString", "/c " + commandString);
                    procStartInfo.RedirectStandardOutput = true;
                    procStartInfo.UseShellExecute = false;
                    procStartInfo.CreateNoWindow = true;
                    Process proc = new Process();
                    proc.StartInfo = procStartInfo;
                    proc.Start();
                    string result = proc.StandardOutput.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                this.loggerInstance.Error("DAService: RestoreStoredProceduresProcess. " + ex.Message);
                Trace.WriteLine(string.Format("RestoreStoredProceduresProcess. {0}", ex.Message));
                throw;
            }
        }

        public static Guid GetGuid(object data)
        {
            if (data == DBNull.Value)
            {
                return Guid.Empty;
            }

            return (Guid)data;
        }
    }
}

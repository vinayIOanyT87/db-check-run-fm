// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.Options.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the Program.Options type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DataImportExportWizard
{
    using System.Diagnostics.CodeAnalysis;

    using CmdLine;

    /// <summary>
    /// The program.
    /// </summary>
    public partial class Program
    {
        /// <summary>
        /// The options.
        /// </summary>
        [CommandLineArguments(Program = "DataImportExportWizard", Title = "Data Import Export Wizard",
            Description = "Data Migration Wizard v8.0 SP4 to Cirrus")]
        public class Options
        {
            #region Attributes

            /// <summary>
            /// A valid action (ExportKeys, ImportKeys, ExportData, ImportData, EncryptOnly, Other)
            /// </summary>
            private string action = string.Empty;

            /// <summary>
            /// Specifies the site ID that the data migration utility should perform the specific action on.
            /// </summary>
            private string siteId = string.Empty;

            /// <summary>
            /// Name of the SQL Server instance to connect to.
            /// </summary>
            private string instanceName = string.Empty;

            /// <summary>
            /// Name of the database to connect to.
            /// </summary>
            private string databaseName = string.Empty;

            /// <summary>
            /// Default path to the export / import folder where the data files should be exported to or imported from.
            /// </summary>
            private string path = string.Empty;

            /// <summary>
            /// Default export / import file to process.
            /// </summary>
            private string fileName = string.Empty;

            /// <summary>
            /// Command line option that indicates any stored passwords should be re-encrypt with AESCrypt.
            /// </summary>
            [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed. Suppression is OK here.")]
            private bool reEncryptFlag = false;

            /// <summary>
            /// Command line option that indicates a database backup should be performed before any changes are made.
            /// </summary>
            private bool backupDatabaseFlag = false;

            /// <summary>
            /// Command line option that indicates whether or not the application will run in interactive mode or without a user interface.
            /// </summary>
            private bool quietFlag = false;

            /// <summary>
            /// Command line option that indicates whether or not a status window is displayed to the user during execution.
            /// </summary>
            private bool showStatusFlag = false;

            #endregion Attributes

            /// <summary>
            /// Gets a value indicating whether command line arguments were found.
            /// </summary>
            public bool CmdLineArgumentFound { get; private set; }

            /// <summary>
            /// Gets or sets the action.
            /// </summary>
            [CommandLineParameter(Command = "a", Name = "Action", Required = true, Description = "Action to perform.", ValueExample = "ExportKeys, ImportKeys, ExportData, ImportData, EncryptOnly")]
            public string Action
            {
                get
                {
                    return this.action;
                }

                set
                {
                    this.CmdLineArgumentFound = true;
                    this.action = value;
                }
            }

            /// <summary>
            /// Gets or sets the site id.
            /// </summary>
            [CommandLineParameter(Command = "sid", Name = "SiteId", Required = false, Description = "Site ID")]
            public string SiteId
            {
                get
                {
                    return this.siteId;
                }

                set
                {
                    this.CmdLineArgumentFound = true;
                    this.siteId = value;
                }
            }

            /// <summary>
            /// Gets or sets the SQL Server instance name.
            /// </summary>
            [CommandLineParameter(Command = "i", Name = "Instance", Required = false, Description = "Name of the SQL Server instance to connect to.", ValueExample = @"ServerName\InstanceName")]
            public string InstanceName
            {
                get
                {
                    return this.instanceName;
                }

                set
                {
                    this.CmdLineArgumentFound = true;
                    this.instanceName = value;
                }
            }

            /// <summary>
            /// Gets or sets the database name.
            /// </summary>
            [CommandLineParameter(Command = "db", Name = "Database", Required = false, Description = "Name of the database to connect to.", ValueExample = "FuelsManagerDB")]
            public string DatabaseName
            {
                get
                {
                    return this.databaseName;
                }

                set
                {
                    this.CmdLineArgumentFound = true;
                    this.databaseName = value;
                }
            }

            /// <summary>
            /// Gets or sets the path.
            /// </summary>
            [CommandLineParameter(Command = "p", Name = "Path", Required = false, Description = "Path that contains that import or output file(s).")]
            public string Path
            {
                get
                {
                    return this.path;
                }

                set
                {
                    this.CmdLineArgumentFound = true;
                    this.path = value;
                }
            }

            /// <summary>
            /// Gets or sets the file to process.
            /// </summary>
            [CommandLineParameter(Command = "f", Name = "File", Required = false, Description = "Filename that should be created or imported.")]
            public string File
            {
                get
                {
                    return this.fileName;
                }

                set
                {
                    this.CmdLineArgumentFound = true;
                    this.fileName = value;
                }
            }

            /// <summary>
            /// Gets or sets a value indicating whether or not the utility should perform a re-encryption on the passwords stored in the database using AES Crypt.
            /// </summary>
            [CommandLineParameter(Command = "aes", Name = "AesEncrypt", Required = false, Description = "Upgrade encrypted data with AESCrypt")]
            public bool ReEncryptFlag
            {
                get
                {
                    return this.reEncryptFlag;
                }

                set
                {
                    this.CmdLineArgumentFound = true;
                    this.reEncryptFlag = value;
                }
            }

            /// <summary>
            /// Gets or sets a value indicating whether or not the utility should perform a database backup prior to performing any tasks.
            /// </summary>
            [CommandLineParameter(Command = "backup", Default = true, Required = false, Description = "Backup database prior to doing any work.")]
            public bool BackupDatabaseFlag
            {
                get
                {
                    return this.backupDatabaseFlag;
                }

                set
                {
                    this.CmdLineArgumentFound = true;
                    this.backupDatabaseFlag = value;
                }
            }

            /// <summary>
            /// Gets or sets a value indicating whether or not the utility should automatically execute and exit.
            /// </summary>
            [CommandLineParameter(Command = "q", Name = "Quiet", Default = false, Required = false, Description = "Perform action and exit.  Note: Provide necessary command line arguments.")]
            public bool QuietFlag
            {
                get
                {
                    return this.quietFlag;
                }

                set
                {
                    this.CmdLineArgumentFound = true;
                    this.quietFlag = value;
                }
            }

            /// <summary>
            /// Gets or sets a value indicating whether or not the utility should display the user interface while processing.
            /// </summary>
            [CommandLineParameter(Command = "status", Name = "ShowStatus", Default = true, Required = false, Description = "Force the status window to be shown while processing.  Note: Typically used with -q option.")]
            public bool ShowStatusFlag
            {
                get
                {
                    return this.showStatusFlag;
                }

                set
                {
                    this.CmdLineArgumentFound = true;
                    this.showStatusFlag = value;
                }
            }

            /// <summary>
            /// Gets or sets a value indicating whether help.
            /// </summary>
            [CommandLineParameter(Command = "?", Required = false, Description = "Show Help", Name = "Help", IsHelp = true)]
            public bool Help { get; set; }
        }
    }
}

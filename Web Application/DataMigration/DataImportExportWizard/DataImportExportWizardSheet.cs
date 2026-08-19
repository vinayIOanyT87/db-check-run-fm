// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataMigrationWizardSheet.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the DataMigrationWizardSheet type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DataImportExportWizard
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Windows.Forms;

    using DataImportExportWizard.Constants;
    using DataImportExportWizard.DataAccess;
    using DataImportExportWizard.InternalClasses.LogClient;

    public partial class DataImportExportWizardSheet : Wizard.UI.WizardSheet
    {
        #region Attributes
        /// <summary>
        /// The logger.
        /// </summary>
        private Logger loggerInstance;

        /// <summary>
        /// The error.
        /// </summary>
        private string error = string.Empty;

        #endregion Attributes

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="DataImportExportWizardSheet"/> class.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        public DataImportExportWizardSheet()
        {
            this.InitializeComponent();

            this.Text = string.Format(
                "{0} (Version {1})", StringConstants.ApplicationName, Application.ProductVersion);

            this.Pages.Add(new WelcomePage());
            this.Pages.Add(new FirstPage());
            this.Pages.Add(new SecondPage());
            this.Pages.Add(new ThirdPage());
            this.Pages.Add(new FourthPage());
            this.Pages.Add(new FifthPage());
            this.Pages.Add(new SixthPage());
            this.Pages.Add(new SeventhPage());
            this.Pages.Add(new FinalPage());

            this.loggerInstance = new Logger(string.Format("{0}_DataImportExportWizard", StringConstants.ApplicationShortName));
        }
        #endregion Constructors

        #region Properties
        /// <summary>
        /// Gets or sets the error.
        /// </summary>
        public string Error
        {
            get
            {
                return this.error;
            }

            set
            {
                this.error = value;
            }
        }

        /// <summary>
        /// The loggerInstance.
        /// </summary>
        public Logger LoggerInstance
        {
            get
            {
                return this.loggerInstance;
            }

            set
            {
                this.loggerInstance = value;
            }
        }
        #endregion Properties

        #region UI Event Handlers

        /// <summary>
        /// The data migration wizard sheet_ form closing.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void DataMigrationWizardSheet_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                DAService adminConnect = new DAService();
                string err = string.Empty;

                // err = adminConnect.ExecuteStoredProcedure("dbo.Migration_DropStuff", string.Empty, "ConsolidatedDB", false);
                // err = adminConnect.ExecuteStoredProcedure("dbo.Migration_ClearDBUsers", "ConsolidatedDB", "Master", true);
                if (File.Exists(DAService.ConsolidatedDatabaseBackupFile))
                {
                    File.Delete(DAService.ConsolidatedDatabaseBackupFile);
                }
            }
            catch (Exception ex)
            {
                this.Error = ex.Message;
                this.LoggerInstance.Error("DataMigrationWizardSheet: DataMigrationWizardSheet_FormClosing. " + ex.Message);
                Trace.WriteLine(string.Format("DataMigrationWizardSheet: DataMigrationWizardSheet_FormClosing. {0}", ex.Message));
            }
        }

        /// <summary>
        /// The data migration wizard sheet_ load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void DataMigrationWizardSheet_Load(object sender, EventArgs e)
        {
            try
            {
                DAService adminConnect = new DAService();
                //adminConnect.RestoreStoredProcedures(false);
            }
            catch (Exception ex)
            {
                this.Error = ex.Message;
                this.LoggerInstance.Error("DataMigrationWizardSheet: DataMigrationWizardSheet_Load. " + ex.Message);
                Trace.WriteLine(string.Format("DataMigrationWizardSheet: DataMigrationWizardSheet_Load. {0}", ex.Message));
                MessageBox.Show(ex.Message);
            }
        }

        #endregion UI Event Handlers
    }
}

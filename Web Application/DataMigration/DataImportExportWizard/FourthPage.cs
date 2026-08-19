// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FourthPage.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the FourthPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DataImportExportWizard
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;

    using DataImportExportWizard.DataAccess;

    using Microsoft.Win32;

    using Wizard.UI;

    /// <summary>
    /// The six page.
    /// </summary>
    public partial class FourthPage : InternalWizardPage
    {
        #region Attributes
        /// <summary>
        /// The da service admin connect.
        /// </summary>
        private readonly DAService daServiceAdminConnect;

        #endregion Attributes

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="FourthPage"/> class.
        /// </summary>
        public FourthPage()
        {
            this.InitializeComponent();
            this.daServiceAdminConnect = new DAService();
        }
        #endregion Constructors

        #region Public Delegates
        /// <summary>
        /// The invoke delegate.
        /// </summary>
        public delegate void InvokeDelegate();

        #endregion Public Delegates

        #region EventHandlers
        /// <summary>
        /// The six page_ set active.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void FourthPage_SetActive(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.SetWizardButtons(WizardButtons.Back | WizardButtons.Finish);

            // ((Wizard.UI.WizardSheet)GetWizard()).cancelButton.Enabled = true;
            ((Wizard.UI.WizardSheet)GetWizard()).finishButton.Enabled = false;
            ((Wizard.UI.WizardSheet)GetWizard()).backButton.Enabled = false;

            this.BeginInvoke(new InvokeDelegate(this.InvokeMethod));
        }

        /// <summary>
        /// The six page_ wizard back.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void FourthPage_WizardBack(object sender, WizardPageEventArgs e)
        {
            e.NewPage = "ThirdPage";
        }

        /// <summary>
        /// The six page_ wizard finish.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void FourthPage_WizardFinish(object sender, CancelEventArgs e)
        {
            try
            {
                string text = this.ProceesInfoLbl.Text;
                text = text.Replace("Migrating data... \n", string.Empty);

                RegistryKey key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Varec\\InstallDetails", false);
                string logFile = (string)key.GetValue("CommonDir");
                logFile = Path.Combine(logFile, "DataMigrationLog.txt");

                StreamWriter tw;
                FileInfo fileInfo = new FileInfo(logFile);

                if (File.Exists(logFile))
                {
                    tw = File.AppendText(logFile);
                }
                else
                {
                    tw = fileInfo.CreateText();
                }

                text = text.Replace("\n", tw.NewLine);
                tw.WriteLine(text);
                tw.Close();

                Process p = new Process();
                p.StartInfo.FileName = logFile;
                p.Start();
                p.WaitForExit();

            }
            catch (Exception ex)
            {
                ((DataImportExportWizardSheet)GetWizard()).LoggerInstance.Error("FourthPage_WizardFinish. " + ex.Message);
                System.Diagnostics.Trace.WriteLine(string.Format("FourthPage_WizardFinish. {0}", ex.Message));
            }
        }

        #endregion EventHandlers

        #region Methods
        /// <summary>
        /// The error handler.
        /// </summary>
        /// <param name="err">
        /// The err.
        /// </param>
        private void ErrorHandler(string err)
        { 
            this.ProceesInfoLbl.AppendText("\n\n**** ERROR : " + err + ".\n");
            this.ProceesInfoLbl.AppendText("Migrating data is terminated with error.\n");

            // ((Wizard.UI.WizardSheet)GetWizard()).cancelButton.Enabled = false;
            ((Wizard.UI.WizardSheet)GetWizard()).finishButton.Enabled = true;            

            DAService adminConnect = new DAService();
            adminConnect.ExecuteStoredProcedure("dbo.Migration_ClearDBUsers", "ConsolidatedDB", "Master", false);

            if (File.Exists(DAService.ConsolidatedDatabaseBackupFile))
            {
                adminConnect.RestoreDatabase("ConsolidatedDB", DAService.ConsolidatedDatabaseBackupFile);
            }
        }

        /// <summary>
        /// The invoke method.
        /// </summary>
        public void InvokeMethod()
        {
            System.Diagnostics.Trace.WriteLine("InvokeMethod.");
            ((Wizard.UI.WizardSheet)GetWizard()).finishButton.Refresh();

            this.ProceesInfoLbl.Refresh();
            this.ProcessDataMigration();
        }

        /// <summary>
        /// The process data migration.
        /// </summary>
        private void ProcessDataMigration()
        {
            // this.Sidebar.BackgroundImage = new Bitmap(this.GetType(), "Bitmaps.Sidebar.bmp");
            try
            {
                this.ProceesInfoLbl.Text = @"Migrating data... \n";

                if (File.Exists(DAService.ConsolidatedDatabaseBackupFile))
                {
                    File.Delete(DAService.ConsolidatedDatabaseBackupFile);
                }

                this.daServiceAdminConnect.BackupDatabase(DAService.ConsolidatedDatabaseBackupFile);

                if (DAService.SiteId != null)
                {
                    this.ProceesInfoLbl.AppendText("SiteId: " + DAService.SiteId + " " + DateTime.Now.ToString() + " \n");
                }
                else
                {
                    this.ProceesInfoLbl.AppendText(DateTime.Now.ToString() + " \n");
                }

                // if (this.ProcessingDataMigration("[dbo].Migration_DisableTriggers") != string.Empty)
                // {
                //     return;
                // }

                // if (this.ProcessingDataMigration("[dbo].Migrate_SetBaseLevelSiteID") != string.Empty)
                // {
                //     return;
                // }
                this.ProceesInfoLbl.AppendText("Migrating data is successfully finished. \n");
            }
            catch (Exception ex)
            {
                ((DataImportExportWizardSheet)GetWizard()).LoggerInstance.Error("FourthPage: ProcessDataMigration. " + ex.Message);
                System.Diagnostics.Trace.WriteLine(string.Format("FourthPage: ProcessDataMigration. {0}", ex.Message));
                try
                {
                    this.ProcessingDataMigration("[dbo].Migrate_EnableTriggers");
                }
                catch (Exception ex2)
                {
                    ((DataImportExportWizardSheet)GetWizard()).LoggerInstance.Error("FourthPage: ProcessDataMigration. " + ex2.Message);
                    Trace.WriteLine(string.Format("FourthPage: ProcessDataMigration. {0}", ex2.Message));
                }
            }

            this.GetWizard().finishButton.Enabled = true;
            this.GetWizard().backButton.Enabled = true;
        }

        /// <summary>
        /// The processing data migration.
        /// </summary>
        /// <param name="storedProcedure">
        /// The stored procedure.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string ProcessingDataMigration(string storedProcedure)
        {
            string[] msg = storedProcedure.Split('_');
            string start = "Start migrating " + msg[1] + " " + DateTime.Now.ToShortTimeString() + ". \n";
            this.ProceesInfoLbl.AppendText(start);
            string err = this.daServiceAdminConnect.ExecuteStoredProcedure(storedProcedure, DAService.SiteId, "ConsolidatedDB", false);
            string end = "End migrating " + msg[1] + " " + DateTime.Now.ToShortTimeString() + ". \n";
            this.ProceesInfoLbl.AppendText(end);

            if (err != string.Empty)
            {
                this.ErrorHandler(err);
                return err;
            }
            else
            {
                this.ProceesInfoLbl.AppendText(msg[1] + " is finished. \n");
            }
            return string.Empty;
        }

        #endregion Methods
    }
}

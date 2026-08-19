// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FirstPage.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the FirstPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DataImportExportWizard
{
    using System;
    using System.ComponentModel;
    using System.Windows.Forms;

    using DataImportExportWizard.Constants;
    using DataImportExportWizard.DataAccess;
    using DataImportExportWizard.InternalClasses;

    using Wizard.UI;

    /// <summary>
    /// The first page.
    /// </summary>
    public partial class FirstPage : InternalWizardPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FirstPage"/> class.
        /// </summary>
        public FirstPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Processing import/export operations on the base server for a specific site.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// Event argument information.
        /// </param>
        private void BaseServer_CheckedChanged(object sender, EventArgs e)
        {
            DataImportExportWizardOption.SelectedInstallationType = InstallationType.BaseServer;
            DataImportExportWizardOption.CurrentImportExportStep = WizardStepType.ImportingKeys; 
            DAService.SiteId = null;
        }

        /// <summary>
        /// Processing import/export operations on the enterprise server for a specific site.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// Event argument information.
        /// </param>
        private void EnterpriseServer_CheckedChanged(object sender, EventArgs e)
        {
            DataImportExportWizardOption.SelectedInstallationType = InstallationType.EnterpriseServer;
            DataImportExportWizardOption.CurrentImportExportStep = WizardStepType.ExportingKeys;
            DAService.SiteId = null;
        }

        /// <summary>
        /// The first page_ set active.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void FirstPage_SetActive(object sender, CancelEventArgs e)
        {
            ((DataImportExportWizardSheet)GetWizard()).Error = string.Empty;
        }

        /// <summary>
        /// The first page wizard next.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void FirstPage_WizardNext(object sender, Wizard.UI.WizardPageEventArgs e)
        {
            // If they are not importing or exporting data then don't check these values.
            if (DataImportExportWizardOption.Action != DataImportExportActionType.Other)
            {
                string msg = "Please make sure you have ConsolidatedDB v8.0 SP4 installed. \n Do you want to continue Data Migration?";
                DialogResult result = MessageBox.Show(msg, @"FuelsManager Data Migration", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            }
        }
    }
}

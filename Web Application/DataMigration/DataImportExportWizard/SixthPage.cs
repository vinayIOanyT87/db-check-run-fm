// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SixthPage.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the SixthPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DataImportExportWizard
{
    using System.ComponentModel;

    using DataImportExportWizard.Constants;
    using DataImportExportWizard.DataAccess;
    using DataImportExportWizard.InternalClasses;

    using Wizard.UI;

    /// <summary>
    /// The fifth page.
    /// </summary>
    public partial class SixthPage : InternalWizardPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SixthPage"/> class.
        /// </summary>
        public SixthPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// The fifth page_ wizard back.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void SixthPage_WizardBack(object sender, Wizard.UI.WizardPageEventArgs e)
        {
            string err = ((DataImportExportWizardSheet)GetWizard()).Error;

            if (err == string.Empty)
            {
                e.NewPage = "FifthPage";
            }
        }

        /// <summary>
        /// The fifth page_ set active.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void SixthPage_SetActive(object sender, CancelEventArgs e)
        {
            ((Wizard.UI.WizardSheet)GetWizard()).backButton.Enabled = false;

            /*            
            ((Wizard.UI.WizardSheet)GetWizard()).nextButton.Text = "&Migrate >";
             */
            string stype = "Exporting Migration Data";

            if (DataImportExportWizardOption.SelectedInstallationType == InstallationType.EnterpriseServer)
            {
                this.SummaryDescriptionLabel.Text = @"Current Import/Export Location: Enterprise Server\n";

                if (DataImportExportWizardOption.CurrentImportExportStep == WizardStepType.ImportingData)
                {
                    stype = "Importing Migration Data for Site";
                    this.PromptLabel.Text = @"Press Next to Start Data Import.";
                }
            }
            else if (DataImportExportWizardOption.SelectedInstallationType == InstallationType.BaseServer)
            {
                this.SummaryDescriptionLabel.Text = @"Current Import/Export Location: Base Server\n";

                if (DataImportExportWizardOption.CurrentImportExportStep == WizardStepType.ExportingData)
                {
                    stype = "Exporting Migration Data for Site";
                    this.PromptLabel.Text = @"Press Next to Start Data Export.";
                }
            }

            string summary = string.Empty;
            summary = string.Format("Import/Export Action: \n\n     {0}. \n\n", stype );

            if (DAService.SiteId != null)
            {
                summary += string.Format("Selected SiteId: \n\n     {0}. \n\n", DAService.SiteId);
            }

            this.SummaryDescriptionLabel.Text = summary;
        }
    }
}

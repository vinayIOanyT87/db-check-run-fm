// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ErrorPage.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   The error page.
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
    /// The error page.
    /// </summary>
    public partial class ErrorPage : Wizard.UI.InternalWizardPage
    {
        public ErrorPage()
        {
            InitializeComponent();
        }

        private void ErrorPage_WizardBack(object sender, WizardPageEventArgs e)
        {
            if (DataImportExportWizardOption.CurrentImportExportStep != WizardStepType.ExportingKeys)
            {
                e.NewPage = "SecondPage";
            }
        }

        private void ErrorPage_SetActive(object sender, CancelEventArgs e)
        {
            this.ErrorMessageLabel.Text = ((DataImportExportWizardSheet)GetWizard()).Error;
            if (this.ErrorMessageLabel.Text != "")
                ((Wizard.UI.WizardSheet)GetWizard()).nextButton.Enabled = false;
            else
                ((Wizard.UI.WizardSheet)GetWizard()).nextButton.Enabled = true;
        }
    }
}

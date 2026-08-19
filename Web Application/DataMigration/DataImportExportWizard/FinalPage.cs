// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FinalPage.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   The welcome page.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DataImportExportWizard
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Text;
    using System.Windows.Forms;

    using Wizard.UI;

    /// <summary>
    /// The welcome page.
    /// </summary>
    public partial class FinalPage : ExternalWizardPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FinalPage"/> class.
        /// </summary>
        public FinalPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// The welcome page_ set active.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void FinalPage_SetActive(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.SetWizardButtons(WizardButtons.Back | WizardButtons.Finish);

            ((Wizard.UI.WizardSheet)GetWizard()).finishButton.Enabled = true;
            ((Wizard.UI.WizardSheet)GetWizard()).backButton.Enabled = false;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.DescriptionTextBox.Text =
                @"IDs and GUIDs: Yes\nImported IDs and GUIDs: Yes\nExported Migration Data: Yes\r\nImported Migration Data: Yes";
        }
    }
}

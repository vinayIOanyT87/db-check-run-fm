// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WelcomePage.cs" company="Varec, Inc.">
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
    public partial class WelcomePage : ExternalWizardPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WelcomePage"/> class.
        /// </summary>
        public WelcomePage()
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
        private void WelcomePage_SetActive(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.SetWizardButtons(WizardButtons.Next);
        }
    }
}

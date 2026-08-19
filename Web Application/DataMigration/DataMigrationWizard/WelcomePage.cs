using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Wizard.UI;

namespace DataMigration
{
    public partial class WelcomePage : Wizard.UI.ExternalWizardPage
    {
        public WelcomePage()
        {
            InitializeComponent();
        }

        private void WelcomePage_SetActive(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SetWizardButtons(WizardButtons.Next);
        }
    }
}

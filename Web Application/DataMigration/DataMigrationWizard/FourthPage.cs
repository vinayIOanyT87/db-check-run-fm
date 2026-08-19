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
    public partial class FourthPage : Wizard.UI.InternalWizardPage
    {
        public FourthPage()
        {
            InitializeComponent();
        }

        private void FourthPage_WizardBack(object sender, WizardPageEventArgs e)
        {
            int iSel = (int)DataMigrationWizardSheet.DataMigrationSel;
            if (iSel != 2)
            {
                e.NewPage = "SecondPage";
            }
        }

        private void FourthPage_SetActive(object sender, CancelEventArgs e)
        {
            Error.Text = ((DataMigrationWizardSheet)GetWizard()).Error;
            if (Error.Text != "")
                ((Wizard.UI.WizardSheet)GetWizard()).nextButton.Enabled = false;
            else
                ((Wizard.UI.WizardSheet)GetWizard()).nextButton.Enabled = true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DataMigration
{
    public partial class FirstPage : Wizard.UI.InternalWizardPage
    {
        public FirstPage()
        {
            InitializeComponent();
        }

        private void BaseEnterprise_CheckedChanged(object sender, EventArgs e)
        {
            DataMigrationWizardSheet.DataMigrationSel = DataMigrationOpt.BaseToEnterprise;
            DAService.Site = null;
        }

        private void EnterpriseEnterprise_CheckedChanged(object sender, EventArgs e)
        {
            DataMigrationWizardSheet.DataMigrationSel = DataMigrationOpt.EnterpriseToEnterprise;
            DAService.Site = null;
        }

        private void BaseBase_CheckedChanged(object sender, EventArgs e)
        {
            DataMigrationWizardSheet.DataMigrationSel = DataMigrationOpt.BaseToBase;
            DAService.Site = null;
        }

        private void FirstPage_SetActive(object sender, CancelEventArgs e)
        {
            ((DataMigrationWizardSheet)GetWizard()).Error = "";
        }

        private void FirstPage_WizardNext(object sender, Wizard.UI.WizardPageEventArgs e)
        {
            if ((int)DataMigrationWizardSheet.DataMigrationSel == 2)
            {
                string msg = "Please make sure you have ConsolidatedDB 6.0 and AccountingDB 6.0 installed. \n Do you want to continue Data Migration?";
                DialogResult result = MessageBox.Show(msg, "FuelsManager Data Migration", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            }
        }
    }
}

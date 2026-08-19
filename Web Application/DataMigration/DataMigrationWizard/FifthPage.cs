using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DataMigration
{
    public partial class FifthPage : Wizard.UI.InternalWizardPage
    {
        public FifthPage()
        {
            InitializeComponent();
        }

        private void FifthPage_WizardBack(object sender, Wizard.UI.WizardPageEventArgs e)
        {
            int iSel = (int)DataMigrationWizardSheet.DataMigrationSel;
            string err = ((DataMigrationWizardSheet)GetWizard()).Error;
            if (err == "")
            {
                if (iSel != 2)
                    e.NewPage = "SecondPage";
                else
                    e.NewPage = "ThirdPage";
            }
        }

        private void FifthPage_SetActive(object sender, CancelEventArgs e)
        {
            ((Wizard.UI.WizardSheet)GetWizard()).backButton.Enabled = false;

            /*            
            ((Wizard.UI.WizardSheet)GetWizard()).nextButton.Text = "&Migrate >";
             */
            Summarylbl.Text = "The type of Migration: \n";
            string stype = "Base to Base Migration";
            if ((int)DataMigrationWizardSheet.DataMigrationSel == 1)
                stype = "Base to Enterprise Migration";
            else if((int)DataMigrationWizardSheet.DataMigrationSel == 2)
                stype = "Enterprise to Enterprise Migration";
            string summary = "";
            summary = string.Format("Migration Type: \n\n     {0}. \n\n", stype );
            if(DAService.Site != null)
                summary += string.Format("Selected Site: \n\n     {0}. \n\n", DAService.Site);
            Summarylbl.Text = summary;
        }
    }
}

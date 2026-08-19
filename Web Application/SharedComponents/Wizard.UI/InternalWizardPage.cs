using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Wizard.UI
{
    public partial class InternalWizardPage : Wizard.UI.WizardPage
    {
        public InternalWizardPage()
        {
            InitializeComponent();
        }

        private void InternalWizardPage_SetActive(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SetWizardButtons(WizardButtons.Back | WizardButtons.Next);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace FMControls
{
    public class FMReadyForPaymentCheckBox : FMCheckBox
    {
        protected void Page_Load(object sender, System.EventArgs e)
        {
            Attributes.Add("onClick", "javascript:ProcessReadyForPayment();");
        }

        protected override void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        private void InitializeComponent()
        {
            this.Load += new System.EventHandler(this.Page_Load);
        }
    }
}

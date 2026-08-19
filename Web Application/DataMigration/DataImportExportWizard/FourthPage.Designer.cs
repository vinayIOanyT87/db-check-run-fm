namespace DataImportExportWizard
{
    partial class FourthPage
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ProceesInfoLbl = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // ProceesInfoLbl
            // 
            this.ProceesInfoLbl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ProceesInfoLbl.Location = new System.Drawing.Point(3, 46);
            this.ProceesInfoLbl.Multiline = true;
            this.ProceesInfoLbl.Name = "ProceesInfoLbl";
            this.ProceesInfoLbl.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.ProceesInfoLbl.Size = new System.Drawing.Size(444, 205);
            this.ProceesInfoLbl.TabIndex = 1;
            this.ProceesInfoLbl.Text = "Migrating data...";
            // 
            // FourthPage
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.AutoScroll = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.ProceesInfoLbl);
            this.Name = "FourthPage";
            this.Size = new System.Drawing.Size(450, 254);
            this.SetActive += new System.ComponentModel.CancelEventHandler(this.FourthPage_SetActive);
            this.WizardBack += new Wizard.UI.WizardPageEventHandler(this.FourthPage_WizardBack);
            this.WizardFinish += new System.ComponentModel.CancelEventHandler(this.FourthPage_WizardFinish);
            this.Controls.SetChildIndex(this.ProceesInfoLbl, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox ProceesInfoLbl;

    }
}

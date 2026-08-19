namespace DataMigration
{
    partial class SixPage
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
            // Sidebar
            // 
            this.Sidebar.Dock = System.Windows.Forms.DockStyle.None;
            this.Sidebar.Size = new System.Drawing.Size(165, 250);
            // 
            // ProceesInfoLbl
            // 
            this.ProceesInfoLbl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ProceesInfoLbl.Location = new System.Drawing.Point(166, 0);
            this.ProceesInfoLbl.Multiline = true;
            this.ProceesInfoLbl.Name = "ProceesInfoLbl";
            this.ProceesInfoLbl.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.ProceesInfoLbl.Size = new System.Drawing.Size(285, 250);
            this.ProceesInfoLbl.TabIndex = 1;
            this.ProceesInfoLbl.Text = "Migrating data...";
            // 
            // SixPage
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.AutoScroll = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.ProceesInfoLbl);
            this.Name = "SixPage";
            this.WizardBack += new Wizard.UI.WizardPageEventHandler(this.SixPage_WizardBack);
            this.WizardFinish += new System.ComponentModel.CancelEventHandler(this.SixPage_WizardFinish);
            this.SetActive += new System.ComponentModel.CancelEventHandler(this.SixPage_SetActive);
            this.Controls.SetChildIndex(this.ProceesInfoLbl, 0);
            this.Controls.SetChildIndex(this.Sidebar, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox ProceesInfoLbl;

    }
}

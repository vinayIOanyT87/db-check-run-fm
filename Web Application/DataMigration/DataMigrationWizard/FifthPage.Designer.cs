namespace DataMigration
{
    partial class FifthPage
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
            this.Summarylbl = new System.Windows.Forms.Label();
            this.title = new System.Windows.Forms.Label();
            this.promptLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Summarylbl
            // 
            this.Summarylbl.Location = new System.Drawing.Point(0, 98);
            this.Summarylbl.Name = "Summarylbl";
            this.Summarylbl.Size = new System.Drawing.Size(440, 149);
            this.Summarylbl.TabIndex = 1;
            this.Summarylbl.Text = "This is Summary of your back up";
            // 
            // title
            // 
            this.title.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.title.Location = new System.Drawing.Point(2, 65);
            this.title.Name = "title";
            this.title.Size = new System.Drawing.Size(438, 23);
            this.title.TabIndex = 2;
            this.title.Text = "Summary";
            this.title.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // promptLabel
            // 
            this.promptLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.promptLabel.Location = new System.Drawing.Point(254, 231);
            this.promptLabel.Name = "promptLabel";
            this.promptLabel.Size = new System.Drawing.Size(183, 16);
            this.promptLabel.TabIndex = 4;
            this.promptLabel.Text = "Press Next to Start Data Migration.";
            // 
            // FifthPage
            // 
            this.Controls.Add(this.promptLabel);
            this.Controls.Add(this.Summarylbl);
            this.Controls.Add(this.title);
            this.Name = "FifthPage";
            this.Size = new System.Drawing.Size(440, 250);
            this.WizardBack += new Wizard.UI.WizardPageEventHandler(this.FifthPage_WizardBack);
            this.SetActive += new System.ComponentModel.CancelEventHandler(this.FifthPage_SetActive);
            this.Controls.SetChildIndex(this.title, 0);
            this.Controls.SetChildIndex(this.Summarylbl, 0);
            this.Controls.SetChildIndex(this.promptLabel, 0);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label Summarylbl;
        private System.Windows.Forms.Label title;
        private System.Windows.Forms.Label promptLabel;
    }
}

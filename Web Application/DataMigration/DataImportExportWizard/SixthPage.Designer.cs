namespace DataImportExportWizard
{
    partial class SixthPage
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
            this.SummaryDescriptionLabel = new System.Windows.Forms.Label();
            this.TitleLabel = new System.Windows.Forms.Label();
            this.PromptLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // SummaryDescriptionLabel
            // 
            this.SummaryDescriptionLabel.Location = new System.Drawing.Point(0, 88);
            this.SummaryDescriptionLabel.Name = "SummaryDescriptionLabel";
            this.SummaryDescriptionLabel.Size = new System.Drawing.Size(440, 147);
            this.SummaryDescriptionLabel.TabIndex = 1;
            this.SummaryDescriptionLabel.Text = "This is Summary of your Import/Export options";
            // 
            // TitleLabel
            // 
            this.TitleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TitleLabel.Location = new System.Drawing.Point(3, 54);
            this.TitleLabel.Name = "TitleLabel";
            this.TitleLabel.Size = new System.Drawing.Size(437, 23);
            this.TitleLabel.TabIndex = 2;
            this.TitleLabel.Text = "Summary";
            this.TitleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // PromptLabel
            // 
            this.PromptLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PromptLabel.Location = new System.Drawing.Point(254, 235);
            this.PromptLabel.Name = "PromptLabel";
            this.PromptLabel.Size = new System.Drawing.Size(193, 16);
            this.PromptLabel.TabIndex = 4;
            this.PromptLabel.Text = "Press Next to Start.";
            // 
            // SixthPage
            // 
            this.Controls.Add(this.PromptLabel);
            this.Controls.Add(this.SummaryDescriptionLabel);
            this.Controls.Add(this.TitleLabel);
            this.Name = "SixthPage";
            this.Size = new System.Drawing.Size(450, 254);
            this.SetActive += new System.ComponentModel.CancelEventHandler(this.SixthPage_SetActive);
            this.WizardBack += new Wizard.UI.WizardPageEventHandler(this.SixthPage_WizardBack);
            this.Controls.SetChildIndex(this.TitleLabel, 0);
            this.Controls.SetChildIndex(this.SummaryDescriptionLabel, 0);
            this.Controls.SetChildIndex(this.PromptLabel, 0);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label SummaryDescriptionLabel;
        private System.Windows.Forms.Label TitleLabel;
        private System.Windows.Forms.Label PromptLabel;
    }
}

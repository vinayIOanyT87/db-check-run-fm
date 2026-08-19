namespace DataImportExportWizard
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
            this.SiteIdTextBox = new System.Windows.Forms.TextBox();
            this.SiteIdDropDown = new System.Windows.Forms.ComboBox();
            this.DescriptionLabel = new System.Windows.Forms.Label();
            this.ImportExportFilenameLabel = new System.Windows.Forms.Label();
            this.SiteLabel = new System.Windows.Forms.Label();
            this.ImportExportFilename = new System.Windows.Forms.TextBox();
            this.BrowseImportExportFileButton = new System.Windows.Forms.Button();
            this.SkipStepCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // SiteIdTextBox
            // 
            this.SiteIdTextBox.Enabled = false;
            this.SiteIdTextBox.Location = new System.Drawing.Point(133, 88);
            this.SiteIdTextBox.Name = "SiteIdTextBox";
            this.SiteIdTextBox.Size = new System.Drawing.Size(233, 20);
            this.SiteIdTextBox.TabIndex = 19;
            this.SiteIdTextBox.Visible = false;
            // 
            // SiteIdDropDown
            // 
            this.SiteIdDropDown.FormattingEnabled = true;
            this.SiteIdDropDown.Location = new System.Drawing.Point(133, 88);
            this.SiteIdDropDown.Name = "SiteIdDropDown";
            this.SiteIdDropDown.Size = new System.Drawing.Size(233, 21);
            this.SiteIdDropDown.TabIndex = 18;
            // 
            // DescriptionLabel
            // 
            this.DescriptionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DescriptionLabel.Location = new System.Drawing.Point(24, 58);
            this.DescriptionLabel.Name = "DescriptionLabel";
            this.DescriptionLabel.Size = new System.Drawing.Size(312, 23);
            this.DescriptionLabel.TabIndex = 17;
            this.DescriptionLabel.Text = "Import Migration Data for Site";
            // 
            // ImportExportFilenameLabel
            // 
            this.ImportExportFilenameLabel.AutoSize = true;
            this.ImportExportFilenameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ImportExportFilenameLabel.Location = new System.Drawing.Point(52, 118);
            this.ImportExportFilenameLabel.Name = "ImportExportFilenameLabel";
            this.ImportExportFilenameLabel.Size = new System.Drawing.Size(31, 13);
            this.ImportExportFilenameLabel.TabIndex = 16;
            this.ImportExportFilenameLabel.Text = "file1";
            // 
            // SiteLabel
            // 
            this.SiteLabel.AutoSize = true;
            this.SiteLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SiteLabel.Location = new System.Drawing.Point(52, 91);
            this.SiteLabel.Name = "SiteLabel";
            this.SiteLabel.Size = new System.Drawing.Size(50, 13);
            this.SiteLabel.TabIndex = 15;
            this.SiteLabel.Text = "Site ID:";
            // 
            // ImportExportFilename
            // 
            this.ImportExportFilename.Location = new System.Drawing.Point(133, 115);
            this.ImportExportFilename.Name = "ImportExportFilename";
            this.ImportExportFilename.Size = new System.Drawing.Size(233, 20);
            this.ImportExportFilename.TabIndex = 14;
            // 
            // BrowseImportExportFileButton
            // 
            this.BrowseImportExportFileButton.Location = new System.Drawing.Point(366, 113);
            this.BrowseImportExportFileButton.Name = "BrowseImportExportFileButton";
            this.BrowseImportExportFileButton.Size = new System.Drawing.Size(28, 23);
            this.BrowseImportExportFileButton.TabIndex = 13;
            this.BrowseImportExportFileButton.Text = "...";
            this.BrowseImportExportFileButton.UseVisualStyleBackColor = true;
            this.BrowseImportExportFileButton.Click += new System.EventHandler(this.BrowseImportExportFileButton_Click);
            // 
            // SkipStepCheckBox
            // 
            this.SkipStepCheckBox.AutoSize = true;
            this.SkipStepCheckBox.Location = new System.Drawing.Point(133, 142);
            this.SkipStepCheckBox.Name = "SkipStepCheckBox";
            this.SkipStepCheckBox.Size = new System.Drawing.Size(89, 17);
            this.SkipStepCheckBox.TabIndex = 20;
            this.SkipStepCheckBox.Text = "Skip this step";
            this.SkipStepCheckBox.UseVisualStyleBackColor = true;
            // 
            // FifthPage
            // 
            this.Controls.Add(this.SkipStepCheckBox);
            this.Controls.Add(this.SiteIdTextBox);
            this.Controls.Add(this.SiteIdDropDown);
            this.Controls.Add(this.DescriptionLabel);
            this.Controls.Add(this.ImportExportFilenameLabel);
            this.Controls.Add(this.SiteLabel);
            this.Controls.Add(this.ImportExportFilename);
            this.Controls.Add(this.BrowseImportExportFileButton);
            this.Name = "FifthPage";
            this.Size = new System.Drawing.Size(450, 254);
            this.WizardNext += new Wizard.UI.WizardPageEventHandler(this.FifthPage_WizardNext);
            this.Load += new System.EventHandler(this.FifthPage_Load);
            this.Controls.SetChildIndex(this.BrowseImportExportFileButton, 0);
            this.Controls.SetChildIndex(this.ImportExportFilename, 0);
            this.Controls.SetChildIndex(this.SiteLabel, 0);
            this.Controls.SetChildIndex(this.ImportExportFilenameLabel, 0);
            this.Controls.SetChildIndex(this.DescriptionLabel, 0);
            this.Controls.SetChildIndex(this.SiteIdDropDown, 0);
            this.Controls.SetChildIndex(this.SiteIdTextBox, 0);
            this.Controls.SetChildIndex(this.SkipStepCheckBox, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox SiteIdTextBox;
        private System.Windows.Forms.ComboBox SiteIdDropDown;
        private System.Windows.Forms.Label DescriptionLabel;
        private System.Windows.Forms.Label ImportExportFilenameLabel;
        private System.Windows.Forms.Label SiteLabel;
        private System.Windows.Forms.TextBox ImportExportFilename;
        private System.Windows.Forms.Button BrowseImportExportFileButton;
        private System.Windows.Forms.CheckBox SkipStepCheckBox;

    }
}

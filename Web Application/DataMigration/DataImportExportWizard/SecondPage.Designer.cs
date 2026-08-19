namespace DataImportExportWizard
{
    partial class SecondPage
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
            this.ImportExportFilenameLabel = new System.Windows.Forms.Label();
            this.ImportExportFilename = new System.Windows.Forms.TextBox();
            this.BrowseImportExportFileButton = new System.Windows.Forms.Button();
            this.SiteLabel = new System.Windows.Forms.Label();
            this.DescriptionLabel = new System.Windows.Forms.Label();
            this.SiteIdDropDown = new System.Windows.Forms.ComboBox();
            this.SiteIdTextBox = new System.Windows.Forms.TextBox();
            this.SkipStepCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // ImportExportFilenameLabel
            // 
            this.ImportExportFilenameLabel.AutoSize = true;
            this.ImportExportFilenameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ImportExportFilenameLabel.Location = new System.Drawing.Point(52, 118);
            this.ImportExportFilenameLabel.Name = "ImportExportFilenameLabel";
            this.ImportExportFilenameLabel.Size = new System.Drawing.Size(31, 13);
            this.ImportExportFilenameLabel.TabIndex = 6;
            this.ImportExportFilenameLabel.Text = "file1";
            // 
            // ImportExportFilename
            // 
            this.ImportExportFilename.Location = new System.Drawing.Point(133, 115);
            this.ImportExportFilename.Name = "ImportExportFilename";
            this.ImportExportFilename.Size = new System.Drawing.Size(233, 20);
            this.ImportExportFilename.TabIndex = 4;
            // 
            // BrowseImportExportFileButton
            // 
            this.BrowseImportExportFileButton.Location = new System.Drawing.Point(366, 113);
            this.BrowseImportExportFileButton.Name = "BrowseImportExportFileButton";
            this.BrowseImportExportFileButton.Size = new System.Drawing.Size(28, 23);
            this.BrowseImportExportFileButton.TabIndex = 3;
            this.BrowseImportExportFileButton.Text = "...";
            this.BrowseImportExportFileButton.UseVisualStyleBackColor = true;
            this.BrowseImportExportFileButton.Click += new System.EventHandler(this.BrowseImportExportFileButton_Click);
            // 
            // SiteLabel
            // 
            this.SiteLabel.AutoSize = true;
            this.SiteLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SiteLabel.Location = new System.Drawing.Point(52, 91);
            this.SiteLabel.Name = "SiteLabel";
            this.SiteLabel.Size = new System.Drawing.Size(50, 13);
            this.SiteLabel.TabIndex = 5;
            this.SiteLabel.Text = "Site ID:";
            // 
            // DescriptionLabel
            // 
            this.DescriptionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DescriptionLabel.Location = new System.Drawing.Point(24, 58);
            this.DescriptionLabel.Name = "DescriptionLabel";
            this.DescriptionLabel.Size = new System.Drawing.Size(312, 23);
            this.DescriptionLabel.TabIndex = 7;
            this.DescriptionLabel.Text = "Export Keys (IDs && GUIDs) for Site";
            // 
            // SiteIdDropDown
            // 
            this.SiteIdDropDown.FormattingEnabled = true;
            this.SiteIdDropDown.Location = new System.Drawing.Point(133, 88);
            this.SiteIdDropDown.Name = "SiteIdDropDown";
            this.SiteIdDropDown.Size = new System.Drawing.Size(233, 21);
            this.SiteIdDropDown.TabIndex = 11;
            // 
            // SiteIdTextBox
            // 
            this.SiteIdTextBox.Enabled = false;
            this.SiteIdTextBox.Location = new System.Drawing.Point(133, 88);
            this.SiteIdTextBox.Name = "SiteIdTextBox";
            this.SiteIdTextBox.Size = new System.Drawing.Size(233, 20);
            this.SiteIdTextBox.TabIndex = 12;
            this.SiteIdTextBox.Visible = false;
            // 
            // SkipStepCheckBox
            // 
            this.SkipStepCheckBox.AutoSize = true;
            this.SkipStepCheckBox.Location = new System.Drawing.Point(133, 142);
            this.SkipStepCheckBox.Name = "SkipStepCheckBox";
            this.SkipStepCheckBox.Size = new System.Drawing.Size(89, 17);
            this.SkipStepCheckBox.TabIndex = 13;
            this.SkipStepCheckBox.Text = "Skip this step";
            this.SkipStepCheckBox.UseVisualStyleBackColor = true;
            this.SkipStepCheckBox.CheckedChanged += new System.EventHandler(this.SkipStepCheckBox_CheckedChanged);
            // 
            // SecondPage
            // 
            this.Controls.Add(this.SkipStepCheckBox);
            this.Controls.Add(this.SiteIdTextBox);
            this.Controls.Add(this.SiteIdDropDown);
            this.Controls.Add(this.DescriptionLabel);
            this.Controls.Add(this.ImportExportFilenameLabel);
            this.Controls.Add(this.SiteLabel);
            this.Controls.Add(this.ImportExportFilename);
            this.Controls.Add(this.BrowseImportExportFileButton);
            this.Name = "SecondPage";
            this.Size = new System.Drawing.Size(450, 254);
            this.SetActive += new System.ComponentModel.CancelEventHandler(this.SecondPage_SetActive);
            this.WizardNext += new Wizard.UI.WizardPageEventHandler(this.SecondPage_WizardNext);
            this.Load += new System.EventHandler(this.SecondPage_Load);
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

        private System.Windows.Forms.Button BrowseImportExportFileButton;
        private System.Windows.Forms.TextBox ImportExportFilename;
        private System.Windows.Forms.Label SiteLabel;
        private System.Windows.Forms.Label ImportExportFilenameLabel;
        private System.Windows.Forms.Label DescriptionLabel;
        private System.Windows.Forms.ComboBox SiteIdDropDown;
        private System.Windows.Forms.TextBox SiteIdTextBox;
        private System.Windows.Forms.CheckBox SkipStepCheckBox;

    }
}

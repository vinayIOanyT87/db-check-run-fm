namespace DataImportExportWizard
{
    partial class FinalPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FinalPage));
            this.PromptLabel = new System.Windows.Forms.Label();
            this.TitleLabel = new System.Windows.Forms.Label();
            this.DescriptionTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // Sidebar
            // 
            this.Sidebar.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("Sidebar.BackgroundImage")));
            this.Sidebar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Sidebar.Enabled = false;
            this.Sidebar.Margin = new System.Windows.Forms.Padding(0);
            this.Sidebar.Size = new System.Drawing.Size(165, 203);
            // 
            // PromptLabel
            // 
            this.PromptLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PromptLabel.Location = new System.Drawing.Point(220, 187);
            this.PromptLabel.Name = "PromptLabel";
            this.PromptLabel.Size = new System.Drawing.Size(173, 16);
            this.PromptLabel.TabIndex = 3;
            this.PromptLabel.Text = "Press Finish to exit the utility.";
            // 
            // TitleLabel
            // 
            this.TitleLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.TitleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TitleLabel.Location = new System.Drawing.Point(196, 9);
            this.TitleLabel.Name = "TitleLabel";
            this.TitleLabel.Size = new System.Drawing.Size(232, 35);
            this.TitleLabel.TabIndex = 4;
            this.TitleLabel.Text = "Completed";
            this.TitleLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // DescriptionTextBox
            // 
            this.DescriptionTextBox.AcceptsReturn = true;
            this.DescriptionTextBox.BackColor = System.Drawing.Color.White;
            this.DescriptionTextBox.Location = new System.Drawing.Point(199, 47);
            this.DescriptionTextBox.Multiline = true;
            this.DescriptionTextBox.Name = "DescriptionTextBox";
            this.DescriptionTextBox.ReadOnly = true;
            this.DescriptionTextBox.Size = new System.Drawing.Size(229, 123);
            this.DescriptionTextBox.TabIndex = 5;
            // 
            // FinalPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.DescriptionTextBox);
            this.Controls.Add(this.TitleLabel);
            this.Controls.Add(this.PromptLabel);
            this.Name = "FinalPage";
            this.Size = new System.Drawing.Size(450, 203);
            this.SetActive += new System.ComponentModel.CancelEventHandler(this.FinalPage_SetActive);
            this.Controls.SetChildIndex(this.PromptLabel, 0);
            this.Controls.SetChildIndex(this.TitleLabel, 0);
            this.Controls.SetChildIndex(this.Sidebar, 0);
            this.Controls.SetChildIndex(this.DescriptionTextBox, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label PromptLabel;
        private System.Windows.Forms.Label TitleLabel;
        private System.Windows.Forms.TextBox DescriptionTextBox;
    }
}

namespace DataImportExportWizard
{
    partial class WelcomePage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WelcomePage));
            this.promptLabel = new System.Windows.Forms.Label();
            this.titleLabel = new System.Windows.Forms.Label();
            this.useLabel = new System.Windows.Forms.Label();
            this.descriptionLabel = new System.Windows.Forms.Label();
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
            // promptLabel
            // 
            this.promptLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.promptLabel.Location = new System.Drawing.Point(220, 187);
            this.promptLabel.Name = "promptLabel";
            this.promptLabel.Size = new System.Drawing.Size(173, 16);
            this.promptLabel.TabIndex = 3;
            this.promptLabel.Text = "Press Next to continue.";
            // 
            // titleLabel
            // 
            this.titleLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.titleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.titleLabel.Location = new System.Drawing.Point(196, 9);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(232, 35);
            this.titleLabel.TabIndex = 4;
            this.titleLabel.Text = "Welcome to the Data Migration Import/Export Utility";
            this.titleLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // useLabel
            // 
            this.useLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.useLabel.Location = new System.Drawing.Point(193, 151);
            this.useLabel.Name = "useLabel";
            this.useLabel.Size = new System.Drawing.Size(235, 19);
            this.useLabel.TabIndex = 5;
            this.useLabel.Text = "(Use when migrating Base Servers)";
            this.useLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // descriptionLabel
            // 
            this.descriptionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.descriptionLabel.Location = new System.Drawing.Point(193, 76);
            this.descriptionLabel.Name = "descriptionLabel";
            this.descriptionLabel.Size = new System.Drawing.Size(235, 55);
            this.descriptionLabel.TabIndex = 6;
            this.descriptionLabel.Text = "Use this utility to import / export migration data between Base and Enterprise Se" +
    "rvers.";
            // 
            // WelcomePage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.descriptionLabel);
            this.Controls.Add(this.titleLabel);
            this.Controls.Add(this.useLabel);
            this.Controls.Add(this.promptLabel);
            this.Name = "WelcomePage";
            this.Size = new System.Drawing.Size(450, 203);
            this.SetActive += new System.ComponentModel.CancelEventHandler(this.WelcomePage_SetActive);
            this.Controls.SetChildIndex(this.promptLabel, 0);
            this.Controls.SetChildIndex(this.useLabel, 0);
            this.Controls.SetChildIndex(this.titleLabel, 0);
            this.Controls.SetChildIndex(this.Sidebar, 0);
            this.Controls.SetChildIndex(this.descriptionLabel, 0);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label promptLabel;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Label useLabel;
        private System.Windows.Forms.Label descriptionLabel;
    }
}

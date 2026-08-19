namespace DataImportExportWizard
{
    partial class ExistingSites
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
			this.CloseButton = new System.Windows.Forms.Button();
			this.Siteslst = new System.Windows.Forms.ListBox();
			this.SuspendLayout();
			// 
			// Close
			// 
			this.CloseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.CloseButton.Location = new System.Drawing.Point(102, 338);
			this.CloseButton.Name = "Close";
			this.CloseButton.Size = new System.Drawing.Size(75, 23);
			this.CloseButton.TabIndex = 1;
			this.CloseButton.Text = "Close";
			this.CloseButton.UseVisualStyleBackColor = true;
			this.CloseButton.Click += new System.EventHandler(this.Close_Click);
			// 
			// Siteslst
			// 
			this.Siteslst.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Siteslst.FormattingEnabled = true;
			this.Siteslst.Location = new System.Drawing.Point(0, 0);
			this.Siteslst.Name = "Siteslst";
			this.Siteslst.Size = new System.Drawing.Size(201, 368);
			this.Siteslst.Sorted = true;
			this.Siteslst.TabIndex = 2;
			// 
			// ExistingSites
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(201, 373);
			this.Controls.Add(this.Siteslst);
			this.Controls.Add(this.CloseButton);
			this.Name = "ExistingSites";
			this.Text = "ExistingSites";
			this.Load += new System.EventHandler(this.ExistingSites_Load);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button CloseButton;
        private System.Windows.Forms.ListBox Siteslst;
    }
}
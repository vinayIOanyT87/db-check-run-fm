namespace ASCReporter
{
	partial class AboutBox
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
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
			this.okButton = new System.Windows.Forms.Button();
			this.textBoxDescription = new System.Windows.Forms.TextBox();
			this.labelCopyright = new System.Windows.Forms.Label();
			this.labelVersion = new System.Windows.Forms.Label();
			this.labelProductName = new System.Windows.Forms.Label();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// okButton
			// 
			this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.okButton.Location = new System.Drawing.Point(150, 224);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(75, 23);
			this.okButton.TabIndex = 24;
			this.okButton.Text = "&OK";
			// 
			// textBoxDescription
			// 
			this.textBoxDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxDescription.Location = new System.Drawing.Point(12, 66);
			this.textBoxDescription.Margin = new System.Windows.Forms.Padding(6, 3, 3, 3);
			this.textBoxDescription.Multiline = true;
			this.textBoxDescription.Name = "textBoxDescription";
			this.textBoxDescription.ReadOnly = true;
			this.textBoxDescription.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBoxDescription.Size = new System.Drawing.Size(351, 68);
			this.textBoxDescription.TabIndex = 23;
			this.textBoxDescription.TabStop = false;
			this.textBoxDescription.Text = "Description";
			// 
			// labelCopyright
			// 
			this.labelCopyright.Location = new System.Drawing.Point(15, 149);
			this.labelCopyright.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
			this.labelCopyright.Name = "labelCopyright";
			this.labelCopyright.Size = new System.Drawing.Size(348, 40);
			this.labelCopyright.TabIndex = 21;
			this.labelCopyright.Text = "Copyright";
			this.labelCopyright.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labelVersion
			// 
			this.labelVersion.Location = new System.Drawing.Point(12, 26);
			this.labelVersion.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
			this.labelVersion.Name = "labelVersion";
			this.labelVersion.Size = new System.Drawing.Size(351, 17);
			this.labelVersion.TabIndex = 0;
			this.labelVersion.Text = "Version";
			this.labelVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labelProductName
			// 
			this.labelProductName.Location = new System.Drawing.Point(12, 9);
			this.labelProductName.Margin = new System.Windows.Forms.Padding(6, 0, 3, 0);
			this.labelProductName.Name = "labelProductName";
			this.labelProductName.Size = new System.Drawing.Size(351, 17);
			this.labelProductName.TabIndex = 19;
			this.labelProductName.Text = "Product Name";
			this.labelProductName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// pictureBox1
			// 
			this.pictureBox1.Location = new System.Drawing.Point(12, 12);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(32, 32);
			this.pictureBox1.TabIndex = 25;
			this.pictureBox1.TabStop = false;
			// 
			// AboutBox
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(375, 259);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.labelProductName);
			this.Controls.Add(this.labelVersion);
			this.Controls.Add(this.labelCopyright);
			this.Controls.Add(this.textBoxDescription);
			this.Controls.Add(this.okButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AboutBox";
			this.Padding = new System.Windows.Forms.Padding(9);
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "AboutBox";
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.TextBox textBoxDescription;
		private System.Windows.Forms.Label labelCopyright;
		private System.Windows.Forms.Label labelVersion;
		private System.Windows.Forms.Label labelProductName;
		private System.Windows.Forms.PictureBox pictureBox1;

	}
}

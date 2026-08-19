namespace Dispatch
{
	partial class FMBAddressConfiguration
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose( bool disposing )
		{
			if ( disposing && ( components != null ) )
			{
				components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.OKBtn = new System.Windows.Forms.Button();
			this.CancelBtn = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.EndPointAddressTextBox = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// OKBtn
			// 
			this.OKBtn.Location = new System.Drawing.Point(115, 104);
			this.OKBtn.Name = "OKBtn";
			this.OKBtn.Size = new System.Drawing.Size(75, 23);
			this.OKBtn.TabIndex = 0;
			this.OKBtn.Text = "OK";
			this.OKBtn.UseVisualStyleBackColor = true;
			this.OKBtn.Click += new System.EventHandler(this.OkButtonClick);
			// 
			// CancelBtn
			// 
			this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.CancelBtn.Location = new System.Drawing.Point(238, 104);
			this.CancelBtn.Name = "CancelBtn";
			this.CancelBtn.Size = new System.Drawing.Size(75, 23);
			this.CancelBtn.TabIndex = 1;
			this.CancelBtn.Text = "Cancel";
			this.CancelBtn.UseVisualStyleBackColor = true;
			this.CancelBtn.Click += new System.EventHandler(this.CancelButtonClick);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(25, 22);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(372, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "Contact your system administrator to verify the address of the Dispatch server.";
			// 
			// EndPointAddressTextBox
			// 
			this.EndPointAddressTextBox.Location = new System.Drawing.Point(28, 47);
			this.EndPointAddressTextBox.Name = "EndPointAddressTextBox";
			this.EndPointAddressTextBox.Size = new System.Drawing.Size(369, 20);
			this.EndPointAddressTextBox.TabIndex = 3;
			// 
			// FMBAddressConfiguration
			// 
			this.AcceptButton = this.OKBtn;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.CancelBtn;
			this.ClientSize = new System.Drawing.Size(431, 160);
			this.Controls.Add(this.EndPointAddressTextBox);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.CancelBtn);
			this.Controls.Add(this.OKBtn);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FMBAddressConfiguration";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "FMBAddressConfiguration";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button OKBtn;
		private System.Windows.Forms.Button CancelBtn;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox EndPointAddressTextBox;
	}
}
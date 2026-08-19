namespace Dispatch
{
	partial class ChangePasswordForm
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
			if (disposing && (this.components != null))
			{
				this.components.Dispose();
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
			this.label1 = new System.Windows.Forms.Label();
			this.passwordTextBox = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.newPasswordTextBox = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.reenterPasswordTextBox = new System.Windows.Forms.TextBox();
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(41, 25);
			this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(96, 13);
			this.label1.TabIndex = 5;
			this.label1.Text = "Current Password: ";
			// 
			// passwordTextBox
			// 
			this.passwordTextBox.Location = new System.Drawing.Point(160, 22);
			this.passwordTextBox.Margin = new System.Windows.Forms.Padding(2);
			this.passwordTextBox.Name = "passwordTextBox";
			this.passwordTextBox.PasswordChar = '*';
			this.passwordTextBox.Size = new System.Drawing.Size(159, 20);
			this.passwordTextBox.TabIndex = 1;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(42, 58);
			this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(84, 13);
			this.label2.TabIndex = 7;
			this.label2.Text = "New Password: ";
			// 
			// newPasswordTextBox
			// 
			this.newPasswordTextBox.Location = new System.Drawing.Point(160, 54);
			this.newPasswordTextBox.Margin = new System.Windows.Forms.Padding(2);
			this.newPasswordTextBox.Name = "newPasswordTextBox";
			this.newPasswordTextBox.PasswordChar = '*';
			this.newPasswordTextBox.Size = new System.Drawing.Size(159, 20);
			this.newPasswordTextBox.TabIndex = 2;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(42, 92);
			this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(103, 13);
			this.label3.TabIndex = 9;
			this.label3.Text = "Re-enter Password: ";
			// 
			// reenterPasswordTextBox
			// 
			this.reenterPasswordTextBox.Location = new System.Drawing.Point(159, 88);
			this.reenterPasswordTextBox.Margin = new System.Windows.Forms.Padding(2);
			this.reenterPasswordTextBox.Name = "reenterPasswordTextBox";
			this.reenterPasswordTextBox.PasswordChar = '*';
			this.reenterPasswordTextBox.Size = new System.Drawing.Size(159, 20);
			this.reenterPasswordTextBox.TabIndex = 3;
			// 
			// okButton
			// 
			this.okButton.Location = new System.Drawing.Point(158, 128);
			this.okButton.Margin = new System.Windows.Forms.Padding(2);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(76, 24);
			this.okButton.TabIndex = 4;
			this.okButton.Text = "OK";
			this.okButton.UseVisualStyleBackColor = true;
			this.okButton.Click += new System.EventHandler(this.OkButtonClick);
			// 
			// cancelButton
			// 
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(243, 128);
			this.cancelButton.Margin = new System.Windows.Forms.Padding(2);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(76, 24);
			this.cancelButton.TabIndex = 5;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			this.cancelButton.Click += new System.EventHandler(this.CancelButtonClick);
			// 
			// ChangePasswordForm
			// 
			this.AcceptButton = this.okButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(365, 182);
			this.ControlBox = false;
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.reenterPasswordTextBox);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.newPasswordTextBox);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.passwordTextBox);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ChangePasswordForm";
			this.ShowIcon = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "FuelsManager - Change Password";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox passwordTextBox;
		private System.Windows.Forms.Label label2;
		public System.Windows.Forms.TextBox newPasswordTextBox;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox reenterPasswordTextBox;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
	}
}
namespace ExStarsUI
{
	partial class frmExStarsUIConfig
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
			this.label1 = new System.Windows.Forms.Label();
			this.tbUserLogin = new System.Windows.Forms.TextBox();
			this.tbPassword = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.tbSite = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.btnSave = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(43, 35);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(94, 17);
			this.label1.TabIndex = 0;
			this.label1.Text = "User Login ID";
			// 
			// tbUserLogin
			// 
			this.tbUserLogin.Location = new System.Drawing.Point(189, 32);
			this.tbUserLogin.Name = "tbUserLogin";
			this.tbUserLogin.Size = new System.Drawing.Size(153, 22);
			this.tbUserLogin.TabIndex = 1;
			// 
			// tbPassword
			// 
			this.tbPassword.Location = new System.Drawing.Point(189, 68);
			this.tbPassword.Name = "tbPassword";
			this.tbPassword.Size = new System.Drawing.Size(153, 22);
			this.tbPassword.TabIndex = 3;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(43, 71);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(142, 17);
			this.label2.TabIndex = 2;
			this.label2.Text = "User Login Password";
			// 
			// tbSite
			// 
			this.tbSite.Location = new System.Drawing.Point(189, 104);
			this.tbSite.Name = "tbSite";
			this.tbSite.Size = new System.Drawing.Size(153, 22);
			this.tbSite.TabIndex = 5;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(43, 104);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(32, 17);
			this.label3.TabIndex = 4;
			this.label3.Text = "Site";
			// 
			// btnSave
			// 
			this.btnSave.Location = new System.Drawing.Point(86, 160);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(75, 23);
			this.btnSave.TabIndex = 0;
			this.btnSave.Text = "Save";
			this.btnSave.UseVisualStyleBackColor = true;
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(189, 160);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 7;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// frmExStarsUIConfig
			// 
			this.AcceptButton = this.btnSave;
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(376, 217);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnSave);
			this.Controls.Add(this.tbSite);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.tbPassword);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.tbUserLogin);
			this.Controls.Add(this.label1);
			this.Name = "frmExStarsUIConfig";
			this.Text = "Edit Configuration";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox tbUserLogin;
		private System.Windows.Forms.TextBox tbPassword;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox tbSite;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button btnSave;
		private System.Windows.Forms.Button btnCancel;
	}
}
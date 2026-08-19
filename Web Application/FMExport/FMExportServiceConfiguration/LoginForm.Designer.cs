namespace FMExportServiceConfiguration
{
    partial class LoginForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
			this.LoginButton = new System.Windows.Forms.Button();
			this.UserNameTextBox = new System.Windows.Forms.TextBox();
			this.PasswordTextBox = new System.Windows.Forms.TextBox();
			this.UserLabel = new System.Windows.Forms.Label();
			this.PasswordLabel = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// LoginButton
			// 
			this.LoginButton.Location = new System.Drawing.Point(185, 214);
			this.LoginButton.Margin = new System.Windows.Forms.Padding(2);
			this.LoginButton.Name = "LoginButton";
			this.LoginButton.Size = new System.Drawing.Size(88, 24);
			this.LoginButton.TabIndex = 3;
			this.LoginButton.Text = "Login Now";
			this.LoginButton.UseVisualStyleBackColor = true;
			this.LoginButton.Click += new System.EventHandler(this.LoginButtonClick);
			// 
			// UserNameTextBox
			// 
			this.UserNameTextBox.Location = new System.Drawing.Point(114, 132);
			this.UserNameTextBox.Margin = new System.Windows.Forms.Padding(2);
			this.UserNameTextBox.Name = "UserNameTextBox";
			this.UserNameTextBox.Size = new System.Drawing.Size(159, 20);
			this.UserNameTextBox.TabIndex = 1;
			// 
			// PasswordTextBox
			// 
			this.PasswordTextBox.Location = new System.Drawing.Point(114, 173);
			this.PasswordTextBox.Margin = new System.Windows.Forms.Padding(2);
			this.PasswordTextBox.Name = "PasswordTextBox";
			this.PasswordTextBox.PasswordChar = '*';
			this.PasswordTextBox.Size = new System.Drawing.Size(159, 20);
			this.PasswordTextBox.TabIndex = 2;
			// 
			// UserLabel
			// 
			this.UserLabel.AutoSize = true;
			this.UserLabel.BackColor = System.Drawing.Color.Transparent;
			this.UserLabel.Location = new System.Drawing.Point(52, 135);
			this.UserLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.UserLabel.Name = "UserLabel";
			this.UserLabel.Size = new System.Drawing.Size(35, 13);
			this.UserLabel.TabIndex = 4;
			this.UserLabel.Text = "User: ";
			// 
			// PasswordLabel
			// 
			this.PasswordLabel.AutoSize = true;
			this.PasswordLabel.BackColor = System.Drawing.Color.Transparent;
			this.PasswordLabel.Location = new System.Drawing.Point(52, 176);
			this.PasswordLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.PasswordLabel.Name = "PasswordLabel";
			this.PasswordLabel.Size = new System.Drawing.Size(56, 13);
			this.PasswordLabel.TabIndex = 5;
			this.PasswordLabel.Text = "Password:";
			// 
			// LoginForm
			// 
			this.AcceptButton = this.LoginButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
			this.ClientSize = new System.Drawing.Size(461, 335);
			this.Controls.Add(this.PasswordLabel);
			this.Controls.Add(this.UserLabel);
			this.Controls.Add(this.PasswordTextBox);
			this.Controls.Add(this.UserNameTextBox);
			this.Controls.Add(this.LoginButton);
			this.Margin = new System.Windows.Forms.Padding(2);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "LoginForm";
			this.ShowIcon = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "FuelsManager - Login";
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button LoginButton;
        private System.Windows.Forms.TextBox UserNameTextBox;
        private System.Windows.Forms.TextBox PasswordTextBox;
        private System.Windows.Forms.Label UserLabel;
        private System.Windows.Forms.Label PasswordLabel;
    }
}


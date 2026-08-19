namespace AfssRICE.ServiceProcess
{
    partial class ServiceUI
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
			this.components = new System.ComponentModel.Container();
			this.btnStop = new System.Windows.Forms.Button();
			this.lblRunning = new System.Windows.Forms.Label();
			this.btnStart = new System.Windows.Forms.Button();
			this.checkServiceTimer = new System.Windows.Forms.Timer(this.components);
			this.TestConnectBtn = new System.Windows.Forms.Button();
			this.GetTransBtn = new System.Windows.Forms.Button();
			this.LoginBtn = new System.Windows.Forms.Button();
			this.ServiceControlGroup = new System.Windows.Forms.GroupBox();
			this.StationCommandsGroup = new System.Windows.Forms.GroupBox();
			this.StationIPAddressLabel = new System.Windows.Forms.Label();
			this.StationURLTextBox = new System.Windows.Forms.TextBox();
			this.richTextBox1 = new System.Windows.Forms.RichTextBox();
			this.ServiceControlGroup.SuspendLayout();
			this.StationCommandsGroup.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnStop
			// 
			this.btnStop.Location = new System.Drawing.Point(87, 19);
			this.btnStop.Name = "btnStop";
			this.btnStop.Size = new System.Drawing.Size(75, 23);
			this.btnStop.TabIndex = 0;
			this.btnStop.Text = "S&top";
			this.btnStop.UseVisualStyleBackColor = true;
			this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
			// 
			// lblRunning
			// 
			this.lblRunning.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblRunning.ForeColor = System.Drawing.Color.DarkGreen;
			this.lblRunning.Location = new System.Drawing.Point(6, 45);
			this.lblRunning.Name = "lblRunning";
			this.lblRunning.Size = new System.Drawing.Size(485, 23);
			this.lblRunning.TabIndex = 1;
			this.lblRunning.Text = "FuelsManager Automated Fuel Service Station Service running...";
			this.lblRunning.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// btnStart
			// 
			this.btnStart.Location = new System.Drawing.Point(6, 19);
			this.btnStart.Name = "btnStart";
			this.btnStart.Size = new System.Drawing.Size(75, 23);
			this.btnStart.TabIndex = 2;
			this.btnStart.Text = "&Start";
			this.btnStart.UseVisualStyleBackColor = true;
			this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
			// 
			// checkServiceTimer
			// 
			this.checkServiceTimer.Enabled = true;
			this.checkServiceTimer.Interval = 6000;
			this.checkServiceTimer.Tick += new System.EventHandler(this.checkServiceTimer_Tick);
			// 
			// TestConnectBtn
			// 
			this.TestConnectBtn.Location = new System.Drawing.Point(9, 48);
			this.TestConnectBtn.Name = "TestConnectBtn";
			this.TestConnectBtn.Size = new System.Drawing.Size(106, 23);
			this.TestConnectBtn.TabIndex = 3;
			this.TestConnectBtn.Text = "Test Connect";
			this.TestConnectBtn.UseVisualStyleBackColor = true;
			this.TestConnectBtn.Click += new System.EventHandler(this.TestConnectBtn_Click);
			// 
			// GetTransBtn
			// 
			this.GetTransBtn.Location = new System.Drawing.Point(9, 77);
			this.GetTransBtn.Name = "GetTransBtn";
			this.GetTransBtn.Size = new System.Drawing.Size(106, 23);
			this.GetTransBtn.TabIndex = 4;
			this.GetTransBtn.Text = "Get Transactions";
			this.GetTransBtn.UseVisualStyleBackColor = true;
			this.GetTransBtn.Click += new System.EventHandler(this.GetTransBtn_Click);
			// 
			// LoginBtn
			// 
			this.LoginBtn.Location = new System.Drawing.Point(416, 19);
			this.LoginBtn.Name = "LoginBtn";
			this.LoginBtn.Size = new System.Drawing.Size(74, 23);
			this.LoginBtn.TabIndex = 5;
			this.LoginBtn.Text = "Login";
			this.LoginBtn.UseVisualStyleBackColor = true;
			this.LoginBtn.Click += new System.EventHandler(this.LoginBtn_Click);
			// 
			// ServiceControlGroup
			// 
			this.ServiceControlGroup.Controls.Add(this.btnStart);
			this.ServiceControlGroup.Controls.Add(this.btnStop);
			this.ServiceControlGroup.Controls.Add(this.lblRunning);
			this.ServiceControlGroup.Location = new System.Drawing.Point(13, 12);
			this.ServiceControlGroup.Name = "ServiceControlGroup";
			this.ServiceControlGroup.Size = new System.Drawing.Size(497, 74);
			this.ServiceControlGroup.TabIndex = 6;
			this.ServiceControlGroup.TabStop = false;
			this.ServiceControlGroup.Text = "Service Control";
			// 
			// StationCommandsGroup
			// 
			this.StationCommandsGroup.Controls.Add(this.StationIPAddressLabel);
			this.StationCommandsGroup.Controls.Add(this.StationURLTextBox);
			this.StationCommandsGroup.Controls.Add(this.richTextBox1);
			this.StationCommandsGroup.Controls.Add(this.TestConnectBtn);
			this.StationCommandsGroup.Controls.Add(this.GetTransBtn);
			this.StationCommandsGroup.Controls.Add(this.LoginBtn);
			this.StationCommandsGroup.Location = new System.Drawing.Point(13, 92);
			this.StationCommandsGroup.Name = "StationCommandsGroup";
			this.StationCommandsGroup.Size = new System.Drawing.Size(496, 234);
			this.StationCommandsGroup.TabIndex = 7;
			this.StationCommandsGroup.TabStop = false;
			this.StationCommandsGroup.Text = "Station Commands";
			// 
			// StationIPAddressLabel
			// 
			this.StationIPAddressLabel.AutoSize = true;
			this.StationIPAddressLabel.Location = new System.Drawing.Point(51, 22);
			this.StationIPAddressLabel.Name = "StationIPAddressLabel";
			this.StationIPAddressLabel.Size = new System.Drawing.Size(61, 13);
			this.StationIPAddressLabel.TabIndex = 8;
			this.StationIPAddressLabel.Text = "IP Address:";
			// 
			// StationURLTextBox
			// 
			this.StationURLTextBox.Location = new System.Drawing.Point(118, 19);
			this.StationURLTextBox.Name = "StationURLTextBox";
			this.StationURLTextBox.Size = new System.Drawing.Size(292, 20);
			this.StationURLTextBox.TabIndex = 7;
			this.StationURLTextBox.Text = "10.33.18.181";
			// 
			// richTextBox1
			// 
			this.richTextBox1.Location = new System.Drawing.Point(118, 48);
			this.richTextBox1.Name = "richTextBox1";
			this.richTextBox1.Size = new System.Drawing.Size(372, 180);
			this.richTextBox1.TabIndex = 6;
			this.richTextBox1.Text = "";
			// 
			// ServiceUI
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(522, 338);
			this.Controls.Add(this.StationCommandsGroup);
			this.Controls.Add(this.ServiceControlGroup);
			this.Name = "ServiceUI";
			this.Text = "FuelsManager Autoamted Fuel Service Station Service";
			this.ServiceControlGroup.ResumeLayout(false);
			this.StationCommandsGroup.ResumeLayout(false);
			this.StationCommandsGroup.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Label lblRunning;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Timer checkServiceTimer;
        private System.Windows.Forms.Button TestConnectBtn;
        private System.Windows.Forms.Button GetTransBtn;
        private System.Windows.Forms.Button LoginBtn;
        private System.Windows.Forms.GroupBox ServiceControlGroup;
        private System.Windows.Forms.GroupBox StationCommandsGroup;
        private System.Windows.Forms.TextBox StationURLTextBox;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Label StationIPAddressLabel;
    }
}
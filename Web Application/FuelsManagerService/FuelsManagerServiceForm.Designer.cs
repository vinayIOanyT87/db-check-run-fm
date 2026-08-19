namespace FuelsManagerService
{
	partial class FuelsManagerServiceForm
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
			this.btnStop = new System.Windows.Forms.Button();
			this.btnStart = new System.Windows.Forms.Button();
			this.labelMaxLoginAttempts = new System.Windows.Forms.Label();
			this.textBoxMaxNumLogins = new System.Windows.Forms.TextBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.textBoxFMaePing = new System.Windows.Forms.TextBox();
			this.textBoxSessionCleanup = new System.Windows.Forms.TextBox();
			this.textBoxAlarmAndEventLogCleanup = new System.Windows.Forms.TextBox();
			this.textBoxAlarmAndEventProcessing = new System.Windows.Forms.TextBox();
			this.checkBoxFMaePing = new System.Windows.Forms.CheckBox();
			this.checkBoxSessionCleanup = new System.Windows.Forms.CheckBox();
			this.checkBoxAlarmAndEventLogCleanup = new System.Windows.Forms.CheckBox();
			this.checkBoxAlarmAndEventProcessing = new System.Windows.Forms.CheckBox();
			this.checkBoxAuditProcessing = new System.Windows.Forms.CheckBox();
			this.checkBoxUserAccountCleanup = new System.Windows.Forms.CheckBox();
			this.textBoxAuditProcessing = new System.Windows.Forms.TextBox();
			this.textBoxUserAccountCleanup = new System.Windows.Forms.TextBox();
			this.checkBoxFCEEMessagesCleanup = new System.Windows.Forms.CheckBox();
			this.textBoxFCEEMessagesCleanup = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnStop
			// 
			this.btnStop.Enabled = false;
			this.btnStop.Location = new System.Drawing.Point(163, 234);
			this.btnStop.Name = "btnStop";
			this.btnStop.Size = new System.Drawing.Size(142, 43);
			this.btnStop.TabIndex = 2;
			this.btnStop.Text = "Stop";
			this.btnStop.UseVisualStyleBackColor = true;
			this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
			// 
			// btnStart
			// 
			this.btnStart.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnStart.Location = new System.Drawing.Point(13, 234);
			this.btnStart.Name = "btnStart";
			this.btnStart.Size = new System.Drawing.Size(142, 43);
			this.btnStart.TabIndex = 1;
			this.btnStart.Text = "Start";
			this.btnStart.UseVisualStyleBackColor = true;
			this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
			// 
			// labelMaxLoginAttempts
			// 
			this.labelMaxLoginAttempts.AutoSize = true;
			this.labelMaxLoginAttempts.Location = new System.Drawing.Point(19, 9);
			this.labelMaxLoginAttempts.Name = "labelMaxLoginAttempts";
			this.labelMaxLoginAttempts.Size = new System.Drawing.Size(164, 13);
			this.labelMaxLoginAttempts.TabIndex = 0;
			this.labelMaxLoginAttempts.Text = "Maximum Num of Login Attempts:";
			// 
			// textBoxMaxNumLogins
			// 
			this.textBoxMaxNumLogins.Location = new System.Drawing.Point(186, 6);
			this.textBoxMaxNumLogins.Name = "textBoxMaxNumLogins";
			this.textBoxMaxNumLogins.Size = new System.Drawing.Size(29, 20);
			this.textBoxMaxNumLogins.TabIndex = 3;
			this.textBoxMaxNumLogins.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.textBoxMaxNumLogins.TextChanged += new System.EventHandler(this.textBoxMaxNumLogins_TextChanged);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.textBoxFMaePing);
			this.groupBox1.Controls.Add(this.textBoxSessionCleanup);
			this.groupBox1.Controls.Add(this.textBoxAlarmAndEventLogCleanup);
			this.groupBox1.Controls.Add(this.textBoxAlarmAndEventProcessing);
			this.groupBox1.Controls.Add(this.checkBoxFMaePing);
			this.groupBox1.Controls.Add(this.checkBoxSessionCleanup);
			this.groupBox1.Controls.Add(this.checkBoxAlarmAndEventLogCleanup);
			this.groupBox1.Controls.Add(this.checkBoxAlarmAndEventProcessing);
			this.groupBox1.Controls.Add(this.checkBoxAuditProcessing);
			this.groupBox1.Controls.Add(this.checkBoxUserAccountCleanup);
			this.groupBox1.Controls.Add(this.textBoxAuditProcessing);
			this.groupBox1.Controls.Add(this.textBoxUserAccountCleanup);
			this.groupBox1.Controls.Add(this.checkBoxFCEEMessagesCleanup);
			this.groupBox1.Controls.Add(this.textBoxFCEEMessagesCleanup);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label6);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Location = new System.Drawing.Point(15, 32);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(290, 196);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Process Interval Minutes";
			// 
			// textBoxFMaePing
			// 
			this.textBoxFMaePing.Location = new System.Drawing.Point(171, 140);
			this.textBoxFMaePing.Name = "textBoxFMaePing";
			this.textBoxFMaePing.Size = new System.Drawing.Size(29, 20);
			this.textBoxFMaePing.TabIndex = 14;
			this.textBoxFMaePing.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.textBoxFMaePing.TextChanged += new System.EventHandler(this.textBoxFMaePing_TextChanged);
			// 
			// textBoxSessionCleanup
			// 
			this.textBoxSessionCleanup.Location = new System.Drawing.Point(171, 116);
			this.textBoxSessionCleanup.Name = "textBoxSessionCleanup";
			this.textBoxSessionCleanup.Size = new System.Drawing.Size(29, 20);
			this.textBoxSessionCleanup.TabIndex = 12;
			this.textBoxSessionCleanup.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.textBoxSessionCleanup.TextChanged += new System.EventHandler(this.textBoxSessionCleanup_TextChanged);
			// 
			// textBoxAlarmAndEventLogCleanup
			// 
			this.textBoxAlarmAndEventLogCleanup.Location = new System.Drawing.Point(171, 92);
			this.textBoxAlarmAndEventLogCleanup.Name = "textBoxAlarmAndEventLogCleanup";
			this.textBoxAlarmAndEventLogCleanup.Size = new System.Drawing.Size(29, 20);
			this.textBoxAlarmAndEventLogCleanup.TabIndex = 10;
			this.textBoxAlarmAndEventLogCleanup.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.textBoxAlarmAndEventLogCleanup.TextChanged += new System.EventHandler(this.textBoxAlarmAndEventLogCleanup_TextChanged);
			// 
			// textBoxAlarmAndEventProcessing
			// 
			this.textBoxAlarmAndEventProcessing.Location = new System.Drawing.Point(171, 68);
			this.textBoxAlarmAndEventProcessing.Name = "textBoxAlarmAndEventProcessing";
			this.textBoxAlarmAndEventProcessing.Size = new System.Drawing.Size(29, 20);
			this.textBoxAlarmAndEventProcessing.TabIndex = 8;
			this.textBoxAlarmAndEventProcessing.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.textBoxAlarmAndEventProcessing.TextChanged += new System.EventHandler(this.textBoxAlarmAndEventProcessing_TextChanged);
			// 
			// checkBoxFMaePing
			// 
			this.checkBoxFMaePing.AutoSize = true;
			this.checkBoxFMaePing.Location = new System.Drawing.Point(206, 142);
			this.checkBoxFMaePing.Name = "checkBoxFMaePing";
			this.checkBoxFMaePing.Size = new System.Drawing.Size(64, 17);
			this.checkBoxFMaePing.TabIndex = 15;
			this.checkBoxFMaePing.Text = "enabled";
			this.checkBoxFMaePing.UseVisualStyleBackColor = true;
			this.checkBoxFMaePing.CheckedChanged += new System.EventHandler(this.checkBoxFMaePing_CheckedChanged);
			// 
			// checkBoxSessionCleanup
			// 
			this.checkBoxSessionCleanup.AutoSize = true;
			this.checkBoxSessionCleanup.Location = new System.Drawing.Point(206, 118);
			this.checkBoxSessionCleanup.Name = "checkBoxSessionCleanup";
			this.checkBoxSessionCleanup.Size = new System.Drawing.Size(64, 17);
			this.checkBoxSessionCleanup.TabIndex = 13;
			this.checkBoxSessionCleanup.Text = "enabled";
			this.checkBoxSessionCleanup.UseVisualStyleBackColor = true;
			this.checkBoxSessionCleanup.CheckedChanged += new System.EventHandler(this.checkBoxSessionCleanup_CheckedChanged);
			// 
			// checkBoxAlarmAndEventLogCleanup
			// 
			this.checkBoxAlarmAndEventLogCleanup.AutoSize = true;
			this.checkBoxAlarmAndEventLogCleanup.Location = new System.Drawing.Point(206, 94);
			this.checkBoxAlarmAndEventLogCleanup.Name = "checkBoxAlarmAndEventLogCleanup";
			this.checkBoxAlarmAndEventLogCleanup.Size = new System.Drawing.Size(64, 17);
			this.checkBoxAlarmAndEventLogCleanup.TabIndex = 11;
			this.checkBoxAlarmAndEventLogCleanup.Text = "enabled";
			this.checkBoxAlarmAndEventLogCleanup.UseVisualStyleBackColor = true;
			this.checkBoxAlarmAndEventLogCleanup.CheckedChanged += new System.EventHandler(this.checkBoxAlarmAndEventLogCleanup_CheckedChanged);
			// 
			// checkBoxAlarmAndEventProcessing
			// 
			this.checkBoxAlarmAndEventProcessing.AutoSize = true;
			this.checkBoxAlarmAndEventProcessing.Location = new System.Drawing.Point(206, 70);
			this.checkBoxAlarmAndEventProcessing.Name = "checkBoxAlarmAndEventProcessing";
			this.checkBoxAlarmAndEventProcessing.Size = new System.Drawing.Size(64, 17);
			this.checkBoxAlarmAndEventProcessing.TabIndex = 9;
			this.checkBoxAlarmAndEventProcessing.Text = "enabled";
			this.checkBoxAlarmAndEventProcessing.UseVisualStyleBackColor = true;
			this.checkBoxAlarmAndEventProcessing.CheckedChanged += new System.EventHandler(this.checkBoxAlarmAndEventProcessing_CheckedChanged);
			// 
			// checkBoxAuditProcessing
			// 
			this.checkBoxAuditProcessing.AutoSize = true;
			this.checkBoxAuditProcessing.Location = new System.Drawing.Point(206, 46);
			this.checkBoxAuditProcessing.Name = "checkBoxAuditProcessing";
			this.checkBoxAuditProcessing.Size = new System.Drawing.Size(64, 17);
			this.checkBoxAuditProcessing.TabIndex = 7;
			this.checkBoxAuditProcessing.Text = "enabled";
			this.checkBoxAuditProcessing.UseVisualStyleBackColor = true;
			this.checkBoxAuditProcessing.CheckedChanged += new System.EventHandler(this.checkBoxAuditProcessing_CheckedChanged);
			// 
			// checkBoxUserAccountCleanup
			// 
			this.checkBoxUserAccountCleanup.AutoSize = true;
			this.checkBoxUserAccountCleanup.Location = new System.Drawing.Point(206, 22);
			this.checkBoxUserAccountCleanup.Name = "checkBoxUserAccountCleanup";
			this.checkBoxUserAccountCleanup.Size = new System.Drawing.Size(64, 17);
			this.checkBoxUserAccountCleanup.TabIndex = 5;
			this.checkBoxUserAccountCleanup.Text = "enabled";
			this.checkBoxUserAccountCleanup.UseVisualStyleBackColor = true;
			this.checkBoxUserAccountCleanup.CheckedChanged += new System.EventHandler(this.checkBoxUserAccountCleanup_CheckedChanged);
			// 
			// textBoxAuditProcessing
			// 
			this.textBoxAuditProcessing.Location = new System.Drawing.Point(171, 44);
			this.textBoxAuditProcessing.Name = "textBoxAuditProcessing";
			this.textBoxAuditProcessing.Size = new System.Drawing.Size(29, 20);
			this.textBoxAuditProcessing.TabIndex = 6;
			this.textBoxAuditProcessing.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.textBoxAuditProcessing.TextChanged += new System.EventHandler(this.textBoxAuditProcessing_TextChanged);
			// 
			// textBoxUserAccountCleanup
			// 
			this.textBoxUserAccountCleanup.Location = new System.Drawing.Point(171, 20);
			this.textBoxUserAccountCleanup.Name = "textBoxUserAccountCleanup";
			this.textBoxUserAccountCleanup.Size = new System.Drawing.Size(29, 20);
			this.textBoxUserAccountCleanup.TabIndex = 4;
			this.textBoxUserAccountCleanup.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.textBoxUserAccountCleanup.TextChanged += new System.EventHandler(this.textBoxUserAccountCleanup_TextChanged);
            //
            // checkBoxFCEEMessagesCleanup
            //
            this.checkBoxFCEEMessagesCleanup.AutoSize = true;
            this.checkBoxFCEEMessagesCleanup.Location = new System.Drawing.Point(206, 166);
            this.checkBoxFCEEMessagesCleanup.Name = "checkBoxFCEEMessagesCleanup";
            this.checkBoxFCEEMessagesCleanup.Size = new System.Drawing.Size(64, 17);
            this.checkBoxFCEEMessagesCleanup.TabIndex = 16;
            this.checkBoxFCEEMessagesCleanup.Text = "enabled";
            this.checkBoxFCEEMessagesCleanup.UseVisualStyleBackColor = true;
            this.checkBoxFCEEMessagesCleanup.CheckedChanged += new System.EventHandler(this.checkBoxFCEEMessagesCleanup_CheckedChanged);
            //
            // textBoxFCEEMessagesCleanup
            //
            this.textBoxFCEEMessagesCleanup.Location = new System.Drawing.Point(171, 164);
            this.textBoxFCEEMessagesCleanup.Name = "textBoxFCEEMessagesCleanup";
            this.textBoxFCEEMessagesCleanup.Size = new System.Drawing.Size(29, 20);
            this.textBoxFCEEMessagesCleanup.TabIndex = 17;
            this.textBoxFCEEMessagesCleanup.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBoxFCEEMessagesCleanup.TextChanged += new System.EventHandler(this.textBoxFCEEMessagesCleanup_TextChanged);
            //
            // label7
            //
            this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(18, 167);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(120, 13);
			this.label7.TabIndex = 0;
			this.label7.Text = "FCEE Messages Cleanup:";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(18, 143);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(137, 13);
			this.label6.TabIndex = 0;
			this.label6.Text = "FM AE Import Service Ping:";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(18, 119);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(89, 13);
			this.label5.TabIndex = 0;
			this.label5.Text = "Session Cleanup:";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(18, 95);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(152, 13);
			this.label4.TabIndex = 0;
			this.label4.Text = "Alarm And Event Log Cleanup:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(18, 71);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(144, 13);
			this.label3.TabIndex = 0;
			this.label3.Text = "Alarm And Event Processing:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(18, 47);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(89, 13);
			this.label2.TabIndex = 0;
			this.label2.Text = "Audit Processing:";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(18, 23);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(117, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "User Account Cleanup:";
			// 
			// FuelsManagerServiceForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(326, 288);
			this.Controls.Add(this.textBoxMaxNumLogins);
			this.Controls.Add(this.labelMaxLoginAttempts);
			this.Controls.Add(this.btnStop);
			this.Controls.Add(this.btnStart);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MinimizeBox = false;
			this.Name = "FuelsManagerServiceForm";
			this.Text = "FuelsManager Service";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnStop;
		private System.Windows.Forms.Button btnStart;
		private System.Windows.Forms.Label labelMaxLoginAttempts;
		private System.Windows.Forms.TextBox textBoxMaxNumLogins;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.TextBox textBoxFMaePing;
		private System.Windows.Forms.TextBox textBoxSessionCleanup;
		private System.Windows.Forms.TextBox textBoxAlarmAndEventLogCleanup;
		private System.Windows.Forms.TextBox textBoxAlarmAndEventProcessing;
		private System.Windows.Forms.CheckBox checkBoxFMaePing;
		private System.Windows.Forms.CheckBox checkBoxSessionCleanup;
		private System.Windows.Forms.CheckBox checkBoxAlarmAndEventLogCleanup;
		private System.Windows.Forms.CheckBox checkBoxAlarmAndEventProcessing;
		private System.Windows.Forms.CheckBox checkBoxAuditProcessing;
		private System.Windows.Forms.CheckBox checkBoxUserAccountCleanup;
		private System.Windows.Forms.TextBox textBoxAuditProcessing;
		private System.Windows.Forms.TextBox textBoxUserAccountCleanup;
		private System.Windows.Forms.CheckBox checkBoxFCEEMessagesCleanup;
		private System.Windows.Forms.TextBox textBoxFCEEMessagesCleanup;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
	}
}
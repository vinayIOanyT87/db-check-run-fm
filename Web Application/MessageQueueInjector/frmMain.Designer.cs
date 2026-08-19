namespace MessageQueueInjector {
	partial class frmMain {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.txtMessage = new System.Windows.Forms.TextBox();
			this.tabSendMessage = new System.Windows.Forms.TabControl();
			this.tabMSMQ = new System.Windows.Forms.TabPage();
			this.btnSendMSMQ = new System.Windows.Forms.Button();
			this.chkTransactional = new System.Windows.Forms.CheckBox();
			this.txtQueuePath = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.tabWebSphere = new System.Windows.Forms.TabPage();
			this.txtPort = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.txtQueueName = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.btnSendWebSphere = new System.Windows.Forms.Button();
			this.txtChannel = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.txtHostName = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.txtQueueManager = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.tabSendMessage.SuspendLayout();
			this.tabMSMQ.SuspendLayout();
			this.tabWebSphere.SuspendLayout();
			this.SuspendLayout();
			// 
			// txtMessage
			// 
			this.txtMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtMessage.Location = new System.Drawing.Point(12, 12);
			this.txtMessage.Multiline = true;
			this.txtMessage.Name = "txtMessage";
			this.txtMessage.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.txtMessage.Size = new System.Drawing.Size(485, 370);
			this.txtMessage.TabIndex = 0;
			// 
			// tabSendMessage
			// 
			this.tabSendMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabSendMessage.Controls.Add(this.tabMSMQ);
			this.tabSendMessage.Controls.Add(this.tabWebSphere);
			this.tabSendMessage.Location = new System.Drawing.Point(12, 388);
			this.tabSendMessage.Name = "tabSendMessage";
			this.tabSendMessage.SelectedIndex = 0;
			this.tabSendMessage.Size = new System.Drawing.Size(485, 185);
			this.tabSendMessage.TabIndex = 1;
			// 
			// tabMSMQ
			// 
			this.tabMSMQ.BackColor = System.Drawing.SystemColors.Control;
			this.tabMSMQ.Controls.Add(this.btnSendMSMQ);
			this.tabMSMQ.Controls.Add(this.chkTransactional);
			this.tabMSMQ.Controls.Add(this.txtQueuePath);
			this.tabMSMQ.Controls.Add(this.label1);
			this.tabMSMQ.Location = new System.Drawing.Point(4, 25);
			this.tabMSMQ.Name = "tabMSMQ";
			this.tabMSMQ.Padding = new System.Windows.Forms.Padding(3);
			this.tabMSMQ.Size = new System.Drawing.Size(477, 156);
			this.tabMSMQ.TabIndex = 0;
			this.tabMSMQ.Text = "Microsoft MQ";
			// 
			// btnSendMSMQ
			// 
			this.btnSendMSMQ.Location = new System.Drawing.Point(182, 109);
			this.btnSendMSMQ.Name = "btnSendMSMQ";
			this.btnSendMSMQ.Size = new System.Drawing.Size(113, 27);
			this.btnSendMSMQ.TabIndex = 3;
			this.btnSendMSMQ.Text = "Send Message";
			this.btnSendMSMQ.UseVisualStyleBackColor = true;
			this.btnSendMSMQ.Click += new System.EventHandler(this.btnSendMSMQ_Click);
			// 
			// chkTransactional
			// 
			this.chkTransactional.AutoSize = true;
			this.chkTransactional.Location = new System.Drawing.Point(100, 34);
			this.chkTransactional.Name = "chkTransactional";
			this.chkTransactional.Size = new System.Drawing.Size(116, 21);
			this.chkTransactional.TabIndex = 2;
			this.chkTransactional.Text = "Transactional";
			this.chkTransactional.UseVisualStyleBackColor = true;
			// 
			// txtQueuePath
			// 
			this.txtQueuePath.Location = new System.Drawing.Point(100, 6);
			this.txtQueuePath.Name = "txtQueuePath";
			this.txtQueuePath.Size = new System.Drawing.Size(371, 22);
			this.txtQueuePath.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(6, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(88, 17);
			this.label1.TabIndex = 0;
			this.label1.Text = "Queue Path:";
			// 
			// tabWebSphere
			// 
			this.tabWebSphere.BackColor = System.Drawing.SystemColors.Control;
			this.tabWebSphere.Controls.Add(this.txtPort);
			this.tabWebSphere.Controls.Add(this.label6);
			this.tabWebSphere.Controls.Add(this.txtQueueName);
			this.tabWebSphere.Controls.Add(this.label5);
			this.tabWebSphere.Controls.Add(this.btnSendWebSphere);
			this.tabWebSphere.Controls.Add(this.txtChannel);
			this.tabWebSphere.Controls.Add(this.label4);
			this.tabWebSphere.Controls.Add(this.txtHostName);
			this.tabWebSphere.Controls.Add(this.label3);
			this.tabWebSphere.Controls.Add(this.txtQueueManager);
			this.tabWebSphere.Controls.Add(this.label2);
			this.tabWebSphere.Location = new System.Drawing.Point(4, 25);
			this.tabWebSphere.Name = "tabWebSphere";
			this.tabWebSphere.Padding = new System.Windows.Forms.Padding(3);
			this.tabWebSphere.Size = new System.Drawing.Size(477, 156);
			this.tabWebSphere.TabIndex = 1;
			this.tabWebSphere.Text = "IBM WebSphere MQ";
			// 
			// txtPort
			// 
			this.txtPort.Location = new System.Drawing.Point(324, 34);
			this.txtPort.Name = "txtPort";
			this.txtPort.Size = new System.Drawing.Size(147, 22);
			this.txtPort.TabIndex = 2;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(280, 37);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(38, 17);
			this.label6.TabIndex = 11;
			this.label6.Text = "Port:";
			// 
			// txtQueueName
			// 
			this.txtQueueName.Location = new System.Drawing.Point(127, 90);
			this.txtQueueName.Name = "txtQueueName";
			this.txtQueueName.Size = new System.Drawing.Size(344, 22);
			this.txtQueueName.TabIndex = 4;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(6, 93);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(96, 17);
			this.label5.TabIndex = 9;
			this.label5.Text = "Queue Name:";
			// 
			// btnSendWebSphere
			// 
			this.btnSendWebSphere.Location = new System.Drawing.Point(180, 123);
			this.btnSendWebSphere.Name = "btnSendWebSphere";
			this.btnSendWebSphere.Size = new System.Drawing.Size(113, 27);
			this.btnSendWebSphere.TabIndex = 5;
			this.btnSendWebSphere.Text = "Send Message";
			this.btnSendWebSphere.UseVisualStyleBackColor = true;
			this.btnSendWebSphere.Click += new System.EventHandler(this.btnSendWebSphere_Click);
			// 
			// txtChannel
			// 
			this.txtChannel.Location = new System.Drawing.Point(127, 62);
			this.txtChannel.Name = "txtChannel";
			this.txtChannel.Size = new System.Drawing.Size(344, 22);
			this.txtChannel.TabIndex = 3;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(6, 65);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(64, 17);
			this.label4.TabIndex = 6;
			this.label4.Text = "Channel:";
			// 
			// txtHostName
			// 
			this.txtHostName.Location = new System.Drawing.Point(127, 34);
			this.txtHostName.Name = "txtHostName";
			this.txtHostName.Size = new System.Drawing.Size(147, 22);
			this.txtHostName.TabIndex = 1;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(6, 37);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(82, 17);
			this.label3.TabIndex = 4;
			this.label3.Text = "Host Name:";
			// 
			// txtQueueManager
			// 
			this.txtQueueManager.Location = new System.Drawing.Point(127, 6);
			this.txtQueueManager.Name = "txtQueueManager";
			this.txtQueueManager.Size = new System.Drawing.Size(344, 22);
			this.txtQueueManager.TabIndex = 0;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(6, 9);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(115, 17);
			this.label2.TabIndex = 2;
			this.label2.Text = "Queue Manager:";
			// 
			// frmMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(509, 585);
			this.Controls.Add(this.tabSendMessage);
			this.Controls.Add(this.txtMessage);
			this.Name = "frmMain";
			this.Text = "Message Queue Injector";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMain_Close);
			this.Load += new System.EventHandler(this.frmMain_Load);
			this.tabSendMessage.ResumeLayout(false);
			this.tabMSMQ.ResumeLayout(false);
			this.tabMSMQ.PerformLayout();
			this.tabWebSphere.ResumeLayout(false);
			this.tabWebSphere.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox txtMessage;
		private System.Windows.Forms.TabControl tabSendMessage;
		private System.Windows.Forms.TabPage tabMSMQ;
		private System.Windows.Forms.TabPage tabWebSphere;
		private System.Windows.Forms.Button btnSendMSMQ;
		private System.Windows.Forms.CheckBox chkTransactional;
		private System.Windows.Forms.TextBox txtQueuePath;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnSendWebSphere;
		private System.Windows.Forms.TextBox txtChannel;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox txtHostName;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox txtQueueManager;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox txtQueueName;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox txtPort;
		private System.Windows.Forms.Label label6;
	}
}


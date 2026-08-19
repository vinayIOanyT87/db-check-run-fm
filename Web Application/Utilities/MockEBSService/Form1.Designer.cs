namespace MockEBSService
{
	partial class Form1
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.lblRespStatus = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.lblRecStatus = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.grpSuccess = new System.Windows.Forms.GroupBox();
            this.txtSAPDoc = new System.Windows.Forms.TextBox();
            this.txtMilStrip = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtSuccessTransID = new System.Windows.Forms.TextBox();
            this.TransID = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rboFailure = new System.Windows.Forms.RadioButton();
            this.rboSuccess = new System.Windows.Forms.RadioButton();
            this.grpFailure = new System.Windows.Forms.GroupBox();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.txtParam = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtFailTransID = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnCreate = new System.Windows.Forms.Button();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.btnCreateMulti = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MilStrip = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.grpSuccess.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.grpFailure.SuspendLayout();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(687, 319);
            this.tabControl1.TabIndex = 3;
            this.tabControl1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.dataGridView1_KeyUp);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.lblRespStatus);
            this.tabPage1.Controls.Add(this.button2);
            this.tabPage1.Controls.Add(this.lblRecStatus);
            this.tabPage1.Controls.Add(this.button1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(679, 293);
            this.tabPage1.TabIndex = 2;
            this.tabPage1.Text = "Service";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // lblRespStatus
            // 
            this.lblRespStatus.AutoSize = true;
            this.lblRespStatus.BackColor = System.Drawing.Color.Red;
            this.lblRespStatus.Location = new System.Drawing.Point(154, 74);
            this.lblRespStatus.Name = "lblRespStatus";
            this.lblRespStatus.Size = new System.Drawing.Size(158, 13);
            this.lblRespStatus.TabIndex = 3;
            this.lblRespStatus.Text = "Response service is not running";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(27, 64);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(104, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "Start Response";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // lblRecStatus
            // 
            this.lblRecStatus.AutoSize = true;
            this.lblRecStatus.BackColor = System.Drawing.Color.Red;
            this.lblRecStatus.Location = new System.Drawing.Point(154, 45);
            this.lblRecStatus.Name = "lblRecStatus";
            this.lblRecStatus.Size = new System.Drawing.Size(150, 13);
            this.lblRecStatus.TabIndex = 1;
            this.lblRecStatus.Text = "Receive service is not running";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(27, 35);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(104, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Start Receive";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox1);
            this.tabPage2.Controls.Add(this.btnCreate);
            this.tabPage2.Controls.Add(this.grpFailure);
            this.tabPage2.Controls.Add(this.grpSuccess);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(679, 293);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Create Single Response";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // grpSuccess
            // 
            this.grpSuccess.Controls.Add(this.txtSAPDoc);
            this.grpSuccess.Controls.Add(this.txtMilStrip);
            this.grpSuccess.Controls.Add(this.label2);
            this.grpSuccess.Controls.Add(this.label1);
            this.grpSuccess.Controls.Add(this.txtSuccessTransID);
            this.grpSuccess.Controls.Add(this.TransID);
            this.grpSuccess.Location = new System.Drawing.Point(5, 79);
            this.grpSuccess.Name = "grpSuccess";
            this.grpSuccess.Size = new System.Drawing.Size(559, 144);
            this.grpSuccess.TabIndex = 12;
            this.grpSuccess.TabStop = false;
            this.grpSuccess.Visible = false;
            // 
            // txtSAPDoc
            // 
            this.txtSAPDoc.Location = new System.Drawing.Point(61, 70);
            this.txtSAPDoc.Name = "txtSAPDoc";
            this.txtSAPDoc.Size = new System.Drawing.Size(484, 20);
            this.txtSAPDoc.TabIndex = 5;
            // 
            // txtMilStrip
            // 
            this.txtMilStrip.Location = new System.Drawing.Point(61, 44);
            this.txtMilStrip.Name = "txtMilStrip";
            this.txtMilStrip.Size = new System.Drawing.Size(484, 20);
            this.txtMilStrip.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(-3, 73);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "SAP Doc #:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 47);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "MilStrip:";
            // 
            // txtSuccessTransID
            // 
            this.txtSuccessTransID.Location = new System.Drawing.Point(61, 18);
            this.txtSuccessTransID.Name = "txtSuccessTransID";
            this.txtSuccessTransID.Size = new System.Drawing.Size(484, 20);
            this.txtSuccessTransID.TabIndex = 1;
            // 
            // TransID
            // 
            this.TransID.AutoSize = true;
            this.TransID.Location = new System.Drawing.Point(13, 21);
            this.TransID.Name = "TransID";
            this.TransID.Size = new System.Drawing.Size(48, 13);
            this.TransID.TabIndex = 0;
            this.TransID.Text = "TransID:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rboFailure);
            this.groupBox1.Controls.Add(this.rboSuccess);
            this.groupBox1.Location = new System.Drawing.Point(5, 14);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 48);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Type";
            // 
            // rboFailure
            // 
            this.rboFailure.AutoSize = true;
            this.rboFailure.Location = new System.Drawing.Point(113, 20);
            this.rboFailure.Name = "rboFailure";
            this.rboFailure.Size = new System.Drawing.Size(56, 17);
            this.rboFailure.TabIndex = 1;
            this.rboFailure.TabStop = true;
            this.rboFailure.Text = "Failure";
            this.rboFailure.UseVisualStyleBackColor = true;
            this.rboFailure.CheckedChanged += new System.EventHandler(this.rboFailure_CheckedChanged);
            // 
            // rboSuccess
            // 
            this.rboSuccess.AutoSize = true;
            this.rboSuccess.Location = new System.Drawing.Point(23, 20);
            this.rboSuccess.Name = "rboSuccess";
            this.rboSuccess.Size = new System.Drawing.Size(66, 17);
            this.rboSuccess.TabIndex = 0;
            this.rboSuccess.TabStop = true;
            this.rboSuccess.Text = "Success";
            this.rboSuccess.UseVisualStyleBackColor = true;
            this.rboSuccess.CheckedChanged += new System.EventHandler(this.rboSuccess_CheckedChanged);
            // 
            // grpFailure
            // 
            this.grpFailure.Controls.Add(this.txtMessage);
            this.grpFailure.Controls.Add(this.txtParam);
            this.grpFailure.Controls.Add(this.label3);
            this.grpFailure.Controls.Add(this.label4);
            this.grpFailure.Controls.Add(this.txtFailTransID);
            this.grpFailure.Controls.Add(this.label5);
            this.grpFailure.Location = new System.Drawing.Point(5, 79);
            this.grpFailure.Name = "grpFailure";
            this.grpFailure.Size = new System.Drawing.Size(559, 144);
            this.grpFailure.TabIndex = 10;
            this.grpFailure.TabStop = false;
            this.grpFailure.Visible = false;
            // 
            // txtMessage
            // 
            this.txtMessage.Location = new System.Drawing.Point(61, 70);
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.Size = new System.Drawing.Size(484, 20);
            this.txtMessage.TabIndex = 5;
            // 
            // txtParam
            // 
            this.txtParam.Location = new System.Drawing.Point(61, 44);
            this.txtParam.Name = "txtParam";
            this.txtParam.Size = new System.Drawing.Size(484, 20);
            this.txtParam.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 73);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Message:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 47);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "MilStrip:";
            // 
            // txtFailTransID
            // 
            this.txtFailTransID.Location = new System.Drawing.Point(61, 18);
            this.txtFailTransID.Name = "txtFailTransID";
            this.txtFailTransID.Size = new System.Drawing.Size(484, 20);
            this.txtFailTransID.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 21);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(48, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "TransID:";
            // 
            // btnCreate
            // 
            this.btnCreate.Location = new System.Drawing.Point(56, 245);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(75, 23);
            this.btnCreate.TabIndex = 9;
            this.btnCreate.Text = "Create";
            this.btnCreate.UseVisualStyleBackColor = true;
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.btnCreateMulti);
            this.tabPage3.Controls.Add(this.dataGridView1);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(679, 293);
            this.tabPage3.TabIndex = 3;
            this.tabPage3.Text = "Create Multiple Responses";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // btnCreateMulti
            // 
            this.btnCreateMulti.Enabled = false;
            this.btnCreateMulti.Location = new System.Drawing.Point(294, 267);
            this.btnCreateMulti.Name = "btnCreateMulti";
            this.btnCreateMulti.Size = new System.Drawing.Size(75, 23);
            this.btnCreateMulti.TabIndex = 1;
            this.btnCreateMulti.Text = "Create";
            this.btnCreateMulti.UseVisualStyleBackColor = true;
            this.btnCreateMulti.Click += new System.EventHandler(this.btnCreateMulti_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.MilStrip,
            this.Column3,
            this.Column5});
            this.dataGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dataGridView1.Location = new System.Drawing.Point(6, 6);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.Size = new System.Drawing.Size(667, 255);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.dataGridView1_KeyUp);
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // Column1
            // 
            this.Column1.HeaderText = "Type";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "TransID";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            // 
            // MilStrip
            // 
            this.MilStrip.HeaderText = "MilStrip";
            this.MilStrip.Name = "MilStrip";
            this.MilStrip.ReadOnly = true;
            // 
            // Column3
            // 
            this.Column3.HeaderText = "SAPDoc#";
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            // 
            // Column5
            // 
            this.Column5.HeaderText = "Message";
            this.Column5.Name = "Column5";
            this.Column5.ReadOnly = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(711, 344);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
            this.Text = "EBS Service Mock";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.dataGridView1_KeyUp);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.grpSuccess.ResumeLayout(false);
            this.grpSuccess.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.grpFailure.ResumeLayout(false);
            this.grpFailure.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.Label lblRecStatus;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.GroupBox grpSuccess;
		private System.Windows.Forms.TextBox txtSAPDoc;
		private System.Windows.Forms.TextBox txtMilStrip;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtSuccessTransID;
		private System.Windows.Forms.Label TransID;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton rboFailure;
		private System.Windows.Forms.RadioButton rboSuccess;
		private System.Windows.Forms.GroupBox grpFailure;
		private System.Windows.Forms.TextBox txtMessage;
		private System.Windows.Forms.TextBox txtParam;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox txtFailTransID;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Button btnCreate;
		private System.Windows.Forms.Label lblRespStatus;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.TabPage tabPage3;
		private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btnCreateMulti;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn MilStrip;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;

	}
}
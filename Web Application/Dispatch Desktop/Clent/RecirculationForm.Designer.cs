namespace Dispatch
{
	partial class RecirculationForm
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
			this.Closebutton = new System.Windows.Forms.Button();
			this.SaveandClosebutton = new System.Windows.Forms.Button();
			this.Applybutton = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.RegistrationIDDropDownList = new System.Windows.Forms.ComboBox();
			this.OperatorDropDownList = new System.Windows.Forms.ComboBox();
			this.ProductDropDownList = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.NetVolumeTextBox = new System.Windows.Forms.TextBox();
			this.GrossVolumetextBox = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.CardNumbertextBox = new System.Windows.Forms.TextBox();
			this.label10 = new System.Windows.Forms.Label();
			this.ServiceBranchtextBox = new System.Windows.Forms.TextBox();
			this.label11 = new System.Windows.Forms.Label();
			this.SerialNumbertextBox = new System.Windows.Forms.TextBox();
			this.label12 = new System.Windows.Forms.Label();
			this.IssuePointNumbertextBox = new System.Windows.Forms.TextBox();
			this.label13 = new System.Windows.Forms.Label();
			this.IssuePointtextBox = new System.Windows.Forms.TextBox();
			this.label14 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.StartTimeSelection = new System.Windows.Forms.DateTimePicker();
			this.TransactionDescription = new System.Windows.Forms.ComboBox();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.StopTimeSelection = new System.Windows.Forms.DateTimePicker();
			this.label7 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.TransactionTypeLabel = new System.Windows.Forms.Label();
			this.MemotextBox = new System.Windows.Forms.TextBox();
			this.label15 = new System.Windows.Forms.Label();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// Closebutton
			// 
			this.Closebutton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.Closebutton.Location = new System.Drawing.Point(80, 421);
			this.Closebutton.Name = "Closebutton";
			this.Closebutton.Size = new System.Drawing.Size(99, 23);
			this.Closebutton.TabIndex = 15;
			this.Closebutton.Text = "&Close";
			this.Closebutton.UseVisualStyleBackColor = true;
			this.Closebutton.Click += new System.EventHandler(this.OnCloseClicked);
			// 
			// SaveandClosebutton
			// 
			this.SaveandClosebutton.Location = new System.Drawing.Point(226, 421);
			this.SaveandClosebutton.Name = "SaveandClosebutton";
			this.SaveandClosebutton.Size = new System.Drawing.Size(99, 23);
			this.SaveandClosebutton.TabIndex = 16;
			this.SaveandClosebutton.Text = "&Save and Close";
			this.SaveandClosebutton.UseVisualStyleBackColor = true;
			this.SaveandClosebutton.Click += new System.EventHandler(this.OnSaveAndCloseClicked);
			// 
			// Applybutton
			// 
			this.Applybutton.Location = new System.Drawing.Point(380, 421);
			this.Applybutton.Name = "Applybutton";
			this.Applybutton.Size = new System.Drawing.Size(99, 23);
			this.Applybutton.TabIndex = 17;
			this.Applybutton.Text = "&Apply";
			this.Applybutton.UseVisualStyleBackColor = true;
			this.Applybutton.Click += new System.EventHandler(this.OnApplyClick);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 37);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(66, 13);
			this.label1.TabIndex = 3;
			this.label1.Text = "Transaction:";
			// 
			// RegistrationIDDropDownList
			// 
			this.RegistrationIDDropDownList.DisplayMember = "ID";
			this.RegistrationIDDropDownList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.RegistrationIDDropDownList.FormattingEnabled = true;
			this.RegistrationIDDropDownList.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.RegistrationIDDropDownList.Location = new System.Drawing.Point(92, 61);
			this.RegistrationIDDropDownList.Name = "RegistrationIDDropDownList";
			this.RegistrationIDDropDownList.Size = new System.Drawing.Size(147, 21);
			this.RegistrationIDDropDownList.TabIndex = 2;
			this.RegistrationIDDropDownList.ValueMember = "Index";
			// 
			// OperatorDropDownList
			// 
			this.OperatorDropDownList.DisplayMember = "FullName";
			this.OperatorDropDownList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.OperatorDropDownList.FormattingEnabled = true;
			this.OperatorDropDownList.Location = new System.Drawing.Point(92, 88);
			this.OperatorDropDownList.Name = "OperatorDropDownList";
			this.OperatorDropDownList.Size = new System.Drawing.Size(147, 21);
			this.OperatorDropDownList.TabIndex = 4;
			this.OperatorDropDownList.ValueMember = "PersonIndex";
			this.OperatorDropDownList.SelectedIndexChanged += new System.EventHandler(this.OperatorDropDownListSelectedIndexChanged);
			// 
			// ProductDropDownList
			// 
			this.ProductDropDownList.DisplayMember = "ID";
			this.ProductDropDownList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ProductDropDownList.FormattingEnabled = true;
			this.ProductDropDownList.Location = new System.Drawing.Point(92, 115);
			this.ProductDropDownList.Name = "ProductDropDownList";
			this.ProductDropDownList.Size = new System.Drawing.Size(147, 21);
			this.ProductDropDownList.TabIndex = 6;
			this.ProductDropDownList.ValueMember = "ProductIndex";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 64);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(80, 13);
			this.label2.TabIndex = 5;
			this.label2.Text = "Registration ID:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(12, 91);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(51, 13);
			this.label3.TabIndex = 7;
			this.label3.Text = "Operator:";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(12, 118);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(47, 13);
			this.label4.TabIndex = 9;
			this.label4.Text = "Product:";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(14, 21);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(27, 13);
			this.label8.TabIndex = 9;
			this.label8.Text = "Net:";
			// 
			// NetVolumeTextBox
			// 
			this.NetVolumeTextBox.Location = new System.Drawing.Point(84, 19);
			this.NetVolumeTextBox.Name = "NetVolumeTextBox";
			this.NetVolumeTextBox.Size = new System.Drawing.Size(147, 20);
			this.NetVolumeTextBox.TabIndex = 12;
			this.NetVolumeTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.NetVolumeTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OnNetVolumeTextBoxKeyPress);
			// 
			// GrossVolumetextBox
			// 
			this.GrossVolumetextBox.Location = new System.Drawing.Point(367, 20);
			this.GrossVolumetextBox.Name = "GrossVolumetextBox";
			this.GrossVolumetextBox.Size = new System.Drawing.Size(146, 20);
			this.GrossVolumetextBox.TabIndex = 13;
			this.GrossVolumetextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.GrossVolumetextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OnGrossVolumeTextBoxKeyPress);
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(276, 23);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(37, 13);
			this.label9.TabIndex = 10;
			this.label9.Text = "Gross:";
			// 
			// CardNumbertextBox
			// 
			this.CardNumbertextBox.Location = new System.Drawing.Point(91, 144);
			this.CardNumbertextBox.MaxLength = 24;
			this.CardNumbertextBox.Name = "CardNumbertextBox";
			this.CardNumbertextBox.Size = new System.Drawing.Size(147, 20);
			this.CardNumbertextBox.TabIndex = 8;
			this.CardNumbertextBox.TextChanged += new System.EventHandler(this.OnCardNumberTextChanged);
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(11, 147);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(72, 13);
			this.label10.TabIndex = 11;
			this.label10.Text = "Card Number:";
			// 
			// ServiceBranchtextBox
			// 
			this.ServiceBranchtextBox.Location = new System.Drawing.Point(374, 116);
			this.ServiceBranchtextBox.Name = "ServiceBranchtextBox";
			this.ServiceBranchtextBox.Size = new System.Drawing.Size(147, 20);
			this.ServiceBranchtextBox.TabIndex = 7;
			this.ServiceBranchtextBox.TextChanged += new System.EventHandler(this.OnServiceBranchTextChanged);
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(268, 120);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(32, 13);
			this.label11.TabIndex = 12;
			this.label11.Text = "BOS:";
			// 
			// SerialNumbertextBox
			// 
			this.SerialNumbertextBox.Location = new System.Drawing.Point(374, 173);
			this.SerialNumbertextBox.Name = "SerialNumbertextBox";
			this.SerialNumbertextBox.Size = new System.Drawing.Size(147, 20);
			this.SerialNumbertextBox.TabIndex = 11;
			this.SerialNumbertextBox.TextChanged += new System.EventHandler(this.OnSerialNumberTextChanged);
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(268, 176);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(76, 13);
			this.label12.TabIndex = 15;
			this.label12.Text = "Serial Number:";
			// 
			// IssuePointNumbertextBox
			// 
			this.IssuePointNumbertextBox.Location = new System.Drawing.Point(374, 145);
			this.IssuePointNumbertextBox.Name = "IssuePointNumbertextBox";
			this.IssuePointNumbertextBox.Size = new System.Drawing.Size(147, 20);
			this.IssuePointNumbertextBox.TabIndex = 9;
			this.IssuePointNumbertextBox.TextChanged += new System.EventHandler(this.OnIssuePointNumberTextChanged);
			// 
			// label13
			// 
			this.label13.AutoSize = true;
			this.label13.Location = new System.Drawing.Point(268, 149);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(102, 13);
			this.label13.TabIndex = 14;
			this.label13.Text = "Issue Point Number:";
			// 
			// IssuePointtextBox
			// 
			this.IssuePointtextBox.Location = new System.Drawing.Point(91, 173);
			this.IssuePointtextBox.Name = "IssuePointtextBox";
			this.IssuePointtextBox.Size = new System.Drawing.Size(147, 20);
			this.IssuePointtextBox.TabIndex = 10;
			this.IssuePointtextBox.TextChanged += new System.EventHandler(this.OnIssuePointTextChanged);
			// 
			// label14
			// 
			this.label14.AutoSize = true;
			this.label14.Location = new System.Drawing.Point(11, 176);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(62, 13);
			this.label14.TabIndex = 13;
			this.label14.Text = "Issue Point:";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.NetVolumeTextBox);
			this.groupBox2.Controls.Add(this.label8);
			this.groupBox2.Controls.Add(this.label9);
			this.groupBox2.Controls.Add(this.GrossVolumetextBox);
			this.groupBox2.Location = new System.Drawing.Point(8, 226);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(541, 53);
			this.groupBox2.TabIndex = 12;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Volumes";
			// 
			// StartTimeSelection
			// 
			this.StartTimeSelection.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.StartTimeSelection.Location = new System.Drawing.Point(375, 61);
			this.StartTimeSelection.Name = "StartTimeSelection";
			this.StartTimeSelection.ShowUpDown = true;
			this.StartTimeSelection.Size = new System.Drawing.Size(146, 20);
			this.StartTimeSelection.TabIndex = 3;
			// 
			// TransactionDescription
			// 
			this.TransactionDescription.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.TransactionDescription.FormattingEnabled = true;
			this.TransactionDescription.Location = new System.Drawing.Point(374, 33);
			this.TransactionDescription.Name = "TransactionDescription";
			this.TransactionDescription.Size = new System.Drawing.Size(147, 21);
			this.TransactionDescription.TabIndex = 1;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(268, 91);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(58, 13);
			this.label6.TabIndex = 7;
			this.label6.Text = "Stop Time:";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(268, 64);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(58, 13);
			this.label5.TabIndex = 6;
			this.label5.Text = "Start Time:";
			this.label5.Click += new System.EventHandler(this.Label5Click);
			// 
			// StopTimeSelection
			// 
			this.StopTimeSelection.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.StopTimeSelection.Location = new System.Drawing.Point(375, 88);
			this.StopTimeSelection.Name = "StopTimeSelection";
			this.StopTimeSelection.ShowUpDown = true;
			this.StopTimeSelection.Size = new System.Drawing.Size(146, 20);
			this.StopTimeSelection.TabIndex = 5;
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(268, 37);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(34, 13);
			this.label7.TabIndex = 4;
			this.label7.Text = "Type:";
			// 
			// groupBox1
			// 
			this.groupBox1.Location = new System.Drawing.Point(8, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(541, 206);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Details";
			// 
			// TransactionTypeLabel
			// 
			this.TransactionTypeLabel.AutoSize = true;
			this.TransactionTypeLabel.Location = new System.Drawing.Point(89, 37);
			this.TransactionTypeLabel.Name = "TransactionTypeLabel";
			this.TransactionTypeLabel.Size = new System.Drawing.Size(69, 13);
			this.TransactionTypeLabel.TabIndex = 16;
			this.TransactionTypeLabel.Text = "Recirculation";
			// 
			// MemotextBox
			// 
			this.MemotextBox.Location = new System.Drawing.Point(8, 316);
			this.MemotextBox.Multiline = true;
			this.MemotextBox.Name = "MemotextBox";
			this.MemotextBox.Size = new System.Drawing.Size(539, 77);
			this.MemotextBox.TabIndex = 14;
			// 
			// label15
			// 
			this.label15.AutoSize = true;
			this.label15.Location = new System.Drawing.Point(11, 294);
			this.label15.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(39, 13);
			this.label15.TabIndex = 35;
			this.label15.Text = "Memo:";
			// 
			// RecirculationForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.Closebutton;
			this.ClientSize = new System.Drawing.Size(565, 459);
			this.ControlBox = false;
			this.Controls.Add(this.TransactionTypeLabel);
			this.Controls.Add(this.IssuePointtextBox);
			this.Controls.Add(this.label15);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.MemotextBox);
			this.Controls.Add(this.CardNumbertextBox);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.StopTimeSelection);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label14);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.ProductDropDownList);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.OperatorDropDownList);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.RegistrationIDDropDownList);
			this.Controls.Add(this.IssuePointNumbertextBox);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.TransactionDescription);
			this.Controls.Add(this.Applybutton);
			this.Controls.Add(this.label11);
			this.Controls.Add(this.SaveandClosebutton);
			this.Controls.Add(this.StartTimeSelection);
			this.Controls.Add(this.Closebutton);
			this.Controls.Add(this.label13);
			this.Controls.Add(this.SerialNumbertextBox);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.ServiceBranchtextBox);
			this.Controls.Add(this.label12);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "RecirculationForm";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Miscellaneous Transactions";
			this.Load += new System.EventHandler(this.OnLoad);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button Closebutton;
		private System.Windows.Forms.Button SaveandClosebutton;
      private System.Windows.Forms.Button Applybutton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox RegistrationIDDropDownList;
		private System.Windows.Forms.ComboBox OperatorDropDownList;
		private System.Windows.Forms.ComboBox ProductDropDownList;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox NetVolumeTextBox;
		private System.Windows.Forms.TextBox GrossVolumetextBox;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox CardNumbertextBox;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.TextBox ServiceBranchtextBox;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.TextBox SerialNumbertextBox;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.TextBox IssuePointNumbertextBox;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.TextBox IssuePointtextBox;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DateTimePicker StartTimeSelection;
        private System.Windows.Forms.ComboBox TransactionDescription;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DateTimePicker StopTimeSelection;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox MemotextBox;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label TransactionTypeLabel;
	}
}
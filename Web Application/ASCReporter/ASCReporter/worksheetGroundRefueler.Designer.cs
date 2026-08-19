namespace ASCReporter
{
	partial class GroundRefuelerWorksheet
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
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.groundVBBD = new System.Windows.Forms.ComboBox();
			this.groundVQWS = new System.Windows.Forms.ComboBox();
			this.authorizedDieselUnits = new System.Windows.Forms.TextBox();
			this.authorizedLeadedUnits = new System.Windows.Forms.TextBox();
			this.authorizedUnleadedUnits = new System.Windows.Forms.TextBox();
			this.authorizedWasteUnits = new System.Windows.Forms.TextBox();
			this.authorizedUTCJFDFUnits = new System.Windows.Forms.TextBox();
			this.authorizedTotalUnits = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.calcDiesel = new System.Windows.Forms.Button();
			this.calcUnleaded = new System.Windows.Forms.Button();
			this.calcLeaded = new System.Windows.Forms.Button();
			this.maxDiesel = new System.Windows.Forms.MaskedTextBox();
			this.maxUnleaded = new System.Windows.Forms.MaskedTextBox();
			this.maxLeaded = new System.Windows.Forms.MaskedTextBox();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 25);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(167, 26);
			this.label1.TabIndex = 0;
			this.label1.Text = "Max 1 Day of Heating/Diesel Fuel\r\nGallon Amount";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 80);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(177, 26);
			this.label2.TabIndex = 1;
			this.label2.Text = "Max 1 Day of Mogas Leaded Gallon\r\nAmount";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(12, 129);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(187, 26);
			this.label3.TabIndex = 2;
			this.label3.Text = "Max 1 Day of Mogas Unleaded Gallon\r\nAmount";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(12, 175);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(194, 26);
			this.label4.TabIndex = 3;
			this.label4.Text = "Do you require a Ground Product Truck\r\nfor Reclaimed Waste Fuel?";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(12, 219);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(194, 26);
			this.label5.TabIndex = 4;
			this.label5.Text = "Do you require a Ground Product Truck\r\nfor UTC-JFDEF?";
			// 
			// groundVBBD
			// 
			this.groundVBBD.FormattingEnabled = true;
			this.groundVBBD.Items.AddRange(new object[] {
            "Yes",
            "No"});
			this.groundVBBD.Location = new System.Drawing.Point(279, 175);
			this.groundVBBD.Name = "groundVBBD";
			this.groundVBBD.Size = new System.Drawing.Size(80, 21);
			this.groundVBBD.TabIndex = 5;
			this.groundVBBD.SelectedIndexChanged += new System.EventHandler(this.groundVBBD_SelectedIndexChanged);
			// 
			// groundVQWS
			// 
			this.groundVQWS.FormattingEnabled = true;
			this.groundVQWS.Items.AddRange(new object[] {
            "Yes",
            "No"});
			this.groundVQWS.Location = new System.Drawing.Point(279, 219);
			this.groundVQWS.Name = "groundVQWS";
			this.groundVQWS.Size = new System.Drawing.Size(79, 21);
			this.groundVQWS.TabIndex = 6;
			this.groundVQWS.SelectedIndexChanged += new System.EventHandler(this.groundVQWS_SelectedIndexChanged);
			// 
			// authorizedDieselUnits
			// 
			this.authorizedDieselUnits.Location = new System.Drawing.Point(328, 275);
			this.authorizedDieselUnits.Name = "authorizedDieselUnits";
			this.authorizedDieselUnits.ReadOnly = true;
			this.authorizedDieselUnits.Size = new System.Drawing.Size(31, 20);
			this.authorizedDieselUnits.TabIndex = 7;
			this.authorizedDieselUnits.Text = "0";
			// 
			// authorizedLeadedUnits
			// 
			this.authorizedLeadedUnits.Location = new System.Drawing.Point(328, 302);
			this.authorizedLeadedUnits.Name = "authorizedLeadedUnits";
			this.authorizedLeadedUnits.ReadOnly = true;
			this.authorizedLeadedUnits.Size = new System.Drawing.Size(31, 20);
			this.authorizedLeadedUnits.TabIndex = 8;
			this.authorizedLeadedUnits.Text = "0";
			// 
			// authorizedUnleadedUnits
			// 
			this.authorizedUnleadedUnits.Location = new System.Drawing.Point(328, 328);
			this.authorizedUnleadedUnits.Name = "authorizedUnleadedUnits";
			this.authorizedUnleadedUnits.ReadOnly = true;
			this.authorizedUnleadedUnits.Size = new System.Drawing.Size(31, 20);
			this.authorizedUnleadedUnits.TabIndex = 9;
			this.authorizedUnleadedUnits.Text = "0";
			// 
			// authorizedWasteUnits
			// 
			this.authorizedWasteUnits.Location = new System.Drawing.Point(328, 355);
			this.authorizedWasteUnits.Name = "authorizedWasteUnits";
			this.authorizedWasteUnits.ReadOnly = true;
			this.authorizedWasteUnits.Size = new System.Drawing.Size(31, 20);
			this.authorizedWasteUnits.TabIndex = 10;
			this.authorizedWasteUnits.Text = "0";
			// 
			// authorizedUTCJFDFUnits
			// 
			this.authorizedUTCJFDFUnits.Location = new System.Drawing.Point(328, 381);
			this.authorizedUTCJFDFUnits.Name = "authorizedUTCJFDFUnits";
			this.authorizedUTCJFDFUnits.ReadOnly = true;
			this.authorizedUTCJFDFUnits.Size = new System.Drawing.Size(31, 20);
			this.authorizedUTCJFDFUnits.TabIndex = 11;
			this.authorizedUTCJFDFUnits.Text = "0";
			// 
			// authorizedTotalUnits
			// 
			this.authorizedTotalUnits.Location = new System.Drawing.Point(328, 419);
			this.authorizedTotalUnits.Name = "authorizedTotalUnits";
			this.authorizedTotalUnits.ReadOnly = true;
			this.authorizedTotalUnits.Size = new System.Drawing.Size(31, 20);
			this.authorizedTotalUnits.TabIndex = 12;
			this.authorizedTotalUnits.Text = "0";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(158, 278);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(164, 13);
			this.label6.TabIndex = 13;
			this.label6.Text = "Authorized Diesel Refueling Units";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(128, 305);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(194, 13);
			this.label7.TabIndex = 14;
			this.label7.Text = "Authorized Leaded Fuel Refueling Units";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(118, 331);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(204, 13);
			this.label8.TabIndex = 15;
			this.label8.Text = "Authorized Unleaded Fuel Refueling Units";
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(94, 358);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(228, 13);
			this.label9.TabIndex = 16;
			this.label9.Text = "Authorized Waste Reclamation Refueling Units";
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(130, 384);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(192, 13);
			this.label10.TabIndex = 17;
			this.label10.Text = "Authorized UTC-JFDEF Refueling Units";
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(85, 422);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(237, 13);
			this.label11.TabIndex = 18;
			this.label11.Text = "Total Authorized Ground Product Refueling Units";
			// 
			// buttonOK
			// 
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point(107, 457);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 19;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(188, 457);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 20;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// calcDiesel
			// 
			this.calcDiesel.Location = new System.Drawing.Point(295, 31);
			this.calcDiesel.Name = "calcDiesel";
			this.calcDiesel.Size = new System.Drawing.Size(61, 20);
			this.calcDiesel.TabIndex = 24;
			this.calcDiesel.Text = "Calculate";
			this.calcDiesel.UseVisualStyleBackColor = true;
			this.calcDiesel.Click += new System.EventHandler(this.calcDiesel_Click);
			// 
			// calcUnleaded
			// 
			this.calcUnleaded.Location = new System.Drawing.Point(294, 135);
			this.calcUnleaded.Name = "calcUnleaded";
			this.calcUnleaded.Size = new System.Drawing.Size(61, 20);
			this.calcUnleaded.TabIndex = 25;
			this.calcUnleaded.Text = "Calculate";
			this.calcUnleaded.UseVisualStyleBackColor = true;
			this.calcUnleaded.Click += new System.EventHandler(this.calcUnleaded_Click);
			// 
			// calcLeaded
			// 
			this.calcLeaded.Location = new System.Drawing.Point(294, 86);
			this.calcLeaded.Name = "calcLeaded";
			this.calcLeaded.Size = new System.Drawing.Size(61, 20);
			this.calcLeaded.TabIndex = 26;
			this.calcLeaded.Text = "Calculate";
			this.calcLeaded.UseVisualStyleBackColor = true;
			this.calcLeaded.Click += new System.EventHandler(this.calcLeaded_Click);
			// 
			// maxDiesel
			// 
			this.maxDiesel.HidePromptOnLeave = true;
			this.maxDiesel.Location = new System.Drawing.Point(243, 31);
			this.maxDiesel.Mask = "00000";
			this.maxDiesel.Name = "maxDiesel";
			this.maxDiesel.Size = new System.Drawing.Size(45, 20);
			this.maxDiesel.TabIndex = 27;
			this.maxDiesel.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.maxDiesel.TextMaskFormat = System.Windows.Forms.MaskFormat.ExcludePromptAndLiterals;
			this.maxDiesel.TextChanged += new System.EventHandler(this.maxDiesel_TextChanged);
			// 
			// maxUnleaded
			// 
			this.maxUnleaded.HidePromptOnLeave = true;
			this.maxUnleaded.Location = new System.Drawing.Point(243, 135);
			this.maxUnleaded.Mask = "00000";
			this.maxUnleaded.Name = "maxUnleaded";
			this.maxUnleaded.Size = new System.Drawing.Size(45, 20);
			this.maxUnleaded.TabIndex = 28;
			this.maxUnleaded.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.maxUnleaded.TextMaskFormat = System.Windows.Forms.MaskFormat.ExcludePromptAndLiterals;
			this.maxUnleaded.TextChanged += new System.EventHandler(this.maxUnleaded_TextChanged);
			// 
			// maxLeaded
			// 
			this.maxLeaded.HidePromptOnLeave = true;
			this.maxLeaded.Location = new System.Drawing.Point(243, 86);
			this.maxLeaded.Mask = "00000";
			this.maxLeaded.Name = "maxLeaded";
			this.maxLeaded.Size = new System.Drawing.Size(45, 20);
			this.maxLeaded.TabIndex = 29;
			this.maxLeaded.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.maxLeaded.TextMaskFormat = System.Windows.Forms.MaskFormat.ExcludePromptAndLiterals;
			this.maxLeaded.TextChanged += new System.EventHandler(this.maxLeaded_TextChanged);
			// 
			// GroundRefuelerWorksheet
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(371, 493);
			this.Controls.Add(this.maxLeaded);
			this.Controls.Add(this.maxUnleaded);
			this.Controls.Add(this.maxDiesel);
			this.Controls.Add(this.calcLeaded);
			this.Controls.Add(this.calcUnleaded);
			this.Controls.Add(this.calcDiesel);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.label11);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.authorizedTotalUnits);
			this.Controls.Add(this.authorizedUTCJFDFUnits);
			this.Controls.Add(this.authorizedWasteUnits);
			this.Controls.Add(this.authorizedUnleadedUnits);
			this.Controls.Add(this.authorizedLeadedUnits);
			this.Controls.Add(this.authorizedDieselUnits);
			this.Controls.Add(this.groundVQWS);
			this.Controls.Add(this.groundVBBD);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "GroundRefuelerWorksheet";
			this.Text = "Ground Refueler Authorization Worksheet";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.ComboBox groundVBBD;
		private System.Windows.Forms.ComboBox groundVQWS;
		private System.Windows.Forms.TextBox authorizedDieselUnits;
		private System.Windows.Forms.TextBox authorizedLeadedUnits;
		private System.Windows.Forms.TextBox authorizedUnleadedUnits;
		private System.Windows.Forms.TextBox authorizedWasteUnits;
		private System.Windows.Forms.TextBox authorizedUTCJFDFUnits;
		private System.Windows.Forms.TextBox authorizedTotalUnits;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button calcDiesel;
		private System.Windows.Forms.Button calcUnleaded;
		private System.Windows.Forms.Button calcLeaded;
		private System.Windows.Forms.MaskedTextBox maxDiesel;
		private System.Windows.Forms.MaskedTextBox maxUnleaded;
		private System.Windows.Forms.MaskedTextBox maxLeaded;
	}
}
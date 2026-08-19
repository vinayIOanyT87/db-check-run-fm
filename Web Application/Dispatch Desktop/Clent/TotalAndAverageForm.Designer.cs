namespace Dispatch
{
	partial class TotalAndAverageForm
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
			this.QuantityradioButton = new System.Windows.Forms.RadioButton();
			this.ResponseTimeradioButton = new System.Windows.Forms.RadioButton();
			this.VarianceradioButton = new System.Windows.Forms.RadioButton();
			this.FuelTimeradioButton = new System.Windows.Forms.RadioButton();
			this.IncludeFillStandcheckBox = new System.Windows.Forms.CheckBox();
			this.AveragetextBox = new System.Windows.Forms.TextBox();
			this.TotaltextBox = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.AverageUnitslabel = new System.Windows.Forms.Label();
			this.TotalUnitslabel = new System.Windows.Forms.Label();
			this.Closebutton = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.IncludeRTBcheckBox = new System.Windows.Forms.CheckBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// QuantityradioButton
			// 
			this.QuantityradioButton.AutoSize = true;
			this.QuantityradioButton.Location = new System.Drawing.Point(25,68);
			this.QuantityradioButton.Name = "QuantityradioButton";
			this.QuantityradioButton.Size = new System.Drawing.Size(64,17);
			this.QuantityradioButton.TabIndex = 2;
			this.QuantityradioButton.TabStop = true;
			this.QuantityradioButton.Text = "Quantity";
			this.QuantityradioButton.UseVisualStyleBackColor = true;
			this.QuantityradioButton.CheckedChanged += new System.EventHandler(this.QuantityradioButtonCheckedChanged);
			// 
			// ResponseTimeradioButton
			// 
			this.ResponseTimeradioButton.AutoSize = true;
			this.ResponseTimeradioButton.Location = new System.Drawing.Point(25,88);
			this.ResponseTimeradioButton.Name = "ResponseTimeradioButton";
			this.ResponseTimeradioButton.Size = new System.Drawing.Size(99,17);
			this.ResponseTimeradioButton.TabIndex = 3;
			this.ResponseTimeradioButton.TabStop = true;
			this.ResponseTimeradioButton.Text = "Response Time";
			this.ResponseTimeradioButton.UseVisualStyleBackColor = true;
			this.ResponseTimeradioButton.CheckedChanged += new System.EventHandler(this.ResponseTimeradioButtonCheckedChanged);
			// 
			// VarianceradioButton
			// 
			this.VarianceradioButton.AutoSize = true;
			this.VarianceradioButton.Location = new System.Drawing.Point(25,108);
			this.VarianceradioButton.Name = "VarianceradioButton";
			this.VarianceradioButton.Size = new System.Drawing.Size(67,17);
			this.VarianceradioButton.TabIndex = 4;
			this.VarianceradioButton.TabStop = true;
			this.VarianceradioButton.Text = "Variance";
			this.VarianceradioButton.UseVisualStyleBackColor = true;
			this.VarianceradioButton.CheckedChanged += new System.EventHandler(this.VarianceradioButtonCheckedChanged);
			// 
			// FuelTimeradioButton
			// 
			this.FuelTimeradioButton.AutoSize = true;
			this.FuelTimeradioButton.Location = new System.Drawing.Point(25,128);
			this.FuelTimeradioButton.Name = "FuelTimeradioButton";
			this.FuelTimeradioButton.Size = new System.Drawing.Size(71,17);
			this.FuelTimeradioButton.TabIndex = 5;
			this.FuelTimeradioButton.TabStop = true;
			this.FuelTimeradioButton.Text = "Fuel Time";
			this.FuelTimeradioButton.UseVisualStyleBackColor = true;
			this.FuelTimeradioButton.CheckedChanged += new System.EventHandler(this.FuelTimeradioButtonCheckedChanged);
			// 
			// IncludeFillStandcheckBox
			// 
			this.IncludeFillStandcheckBox.AutoSize = true;
			this.IncludeFillStandcheckBox.Location = new System.Drawing.Point(13,12);
			this.IncludeFillStandcheckBox.Name = "IncludeFillStandcheckBox";
			this.IncludeFillStandcheckBox.Size = new System.Drawing.Size(161,17);
			this.IncludeFillStandcheckBox.TabIndex = 1;
			this.IncludeFillStandcheckBox.Text = "Include Fill-Stand Operations";
			this.IncludeFillStandcheckBox.UseVisualStyleBackColor = true;
			this.IncludeFillStandcheckBox.CheckedChanged += new System.EventHandler(this.IncludeFillStandcheckBoxCheckedChanged);
			// 
			// AveragetextBox
			// 
			this.AveragetextBox.Location = new System.Drawing.Point(274,41);
			this.AveragetextBox.Name = "AveragetextBox";
			this.AveragetextBox.ReadOnly = true;
			this.AveragetextBox.Size = new System.Drawing.Size(100,20);
			this.AveragetextBox.TabIndex = 5;
			this.AveragetextBox.TabStop = false;
			// 
			// TotaltextBox
			// 
			this.TotaltextBox.Location = new System.Drawing.Point(274,101);
			this.TotaltextBox.Name = "TotaltextBox";
			this.TotaltextBox.ReadOnly = true;
			this.TotaltextBox.Size = new System.Drawing.Size(100,20);
			this.TotaltextBox.TabIndex = 6;
			this.TotaltextBox.TabStop = false;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(271,25);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(50,13);
			this.label1.TabIndex = 7;
			this.label1.Text = "Average:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(271,78);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(34,13);
			this.label2.TabIndex = 8;
			this.label2.Text = "Total:";
			// 
			// AverageUnitslabel
			// 
			this.AverageUnitslabel.AutoSize = true;
			this.AverageUnitslabel.Location = new System.Drawing.Point(380,44);
			this.AverageUnitslabel.Name = "AverageUnitslabel";
			this.AverageUnitslabel.Size = new System.Drawing.Size(74,13);
			this.AverageUnitslabel.TabIndex = 9;
			this.AverageUnitslabel.Text = "Average Units";
			// 
			// TotalUnitslabel
			// 
			this.TotalUnitslabel.AutoSize = true;
			this.TotalUnitslabel.Location = new System.Drawing.Point(380,105);
			this.TotalUnitslabel.Name = "TotalUnitslabel";
			this.TotalUnitslabel.Size = new System.Drawing.Size(58,13);
			this.TotalUnitslabel.TabIndex = 10;
			this.TotalUnitslabel.Text = "Total Units";
			// 
			// Closebutton
			// 
			this.Closebutton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.Closebutton.Location = new System.Drawing.Point(206,173);
			this.Closebutton.Name = "Closebutton";
			this.Closebutton.Size = new System.Drawing.Size(75,23);
			this.Closebutton.TabIndex = 0;
			this.Closebutton.Text = "&Close";
			this.Closebutton.UseVisualStyleBackColor = true;
			this.Closebutton.Click += new System.EventHandler(this.ClosebuttonClick);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.IncludeRTBcheckBox);
			this.groupBox1.Controls.Add(this.IncludeFillStandcheckBox);
			this.groupBox1.Location = new System.Drawing.Point(12,13);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(213,141);
			this.groupBox1.TabIndex = 11;
			this.groupBox1.TabStop = false;
			// 
			// IncludeRTBcheckBox
			// 
			this.IncludeRTBcheckBox.AutoSize = true;
			this.IncludeRTBcheckBox.Location = new System.Drawing.Point(13,33);
			this.IncludeRTBcheckBox.Name = "IncludeRTBcheckBox";
			this.IncludeRTBcheckBox.Size = new System.Drawing.Size(140,17);
			this.IncludeRTBcheckBox.TabIndex = 2;
			this.IncludeRTBcheckBox.Text = "Include RTB Operations";
			this.IncludeRTBcheckBox.UseVisualStyleBackColor = true;
			this.IncludeRTBcheckBox.CheckedChanged += new System.EventHandler(this.IncludeFillStandcheckBoxCheckedChanged);
			// 
			// groupBox2
			// 
			this.groupBox2.Location = new System.Drawing.Point(255,12);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(213,141);
			this.groupBox2.TabIndex = 12;
			this.groupBox2.TabStop = false;
			// 
			// TotalAndAverageForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F,13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.Closebutton;
			this.ClientSize = new System.Drawing.Size(481,218);
			this.ControlBox = false;
			this.Controls.Add(this.Closebutton);
			this.Controls.Add(this.TotalUnitslabel);
			this.Controls.Add(this.AverageUnitslabel);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.TotaltextBox);
			this.Controls.Add(this.AveragetextBox);
			this.Controls.Add(this.FuelTimeradioButton);
			this.Controls.Add(this.VarianceradioButton);
			this.Controls.Add(this.ResponseTimeradioButton);
			this.Controls.Add(this.QuantityradioButton);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.groupBox2);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Name = "TotalAndAverageForm";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Total and Average Calculations";
			this.Load += new System.EventHandler(this.TotalAndAverageOnLoad);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.RadioButton QuantityradioButton;
		private System.Windows.Forms.RadioButton ResponseTimeradioButton;
		private System.Windows.Forms.RadioButton VarianceradioButton;
		private System.Windows.Forms.RadioButton FuelTimeradioButton;
		private System.Windows.Forms.CheckBox IncludeFillStandcheckBox;
		private System.Windows.Forms.TextBox AveragetextBox;
		private System.Windows.Forms.TextBox TotaltextBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label AverageUnitslabel;
		private System.Windows.Forms.Label TotalUnitslabel;
		private System.Windows.Forms.Button Closebutton;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.CheckBox IncludeRTBcheckBox;
	}
}
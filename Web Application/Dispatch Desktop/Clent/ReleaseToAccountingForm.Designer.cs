namespace Dispatch
{
	partial class ReleaseToAccountingForm
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
			this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.SpecialFuelFlag = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.InService = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.FuelingState = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Isspt = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.IssPtNum = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.applybutton = new System.Windows.Forms.Button();
			this.closebutton = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.lockoutdateTimePicker = new System.Windows.Forms.DateTimePicker();
			this.lockoutTimePicker = new System.Windows.Forms.DateTimePicker();
			this.SetToCurrentDateTimebutton = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// Column5
			// 
			this.Column5.Name = "Column5";
			// 
			// Column1
			// 
			this.Column1.Name = "Column1";
			// 
			// Column2
			// 
			this.Column2.Name = "Column2";
			// 
			// Column3
			// 
			this.Column3.Name = "Column3";
			// 
			// Column4
			// 
			this.Column4.Name = "Column4";
			// 
			// SpecialFuelFlag
			// 
			this.SpecialFuelFlag.Name = "SpecialFuelFlag";
			// 
			// InService
			// 
			this.InService.Name = "InService";
			// 
			// FuelingState
			// 
			this.FuelingState.Name = "FuelingState";
			// 
			// Isspt
			// 
			this.Isspt.Name = "Isspt";
			// 
			// IssPtNum
			// 
			this.IssPtNum.Name = "IssPtNum";
			// 
			// applybutton
			// 
			this.applybutton.Location = new System.Drawing.Point(160, 106);
			this.applybutton.Name = "applybutton";
			this.applybutton.Size = new System.Drawing.Size(75, 23);
			this.applybutton.TabIndex = 0;
			this.applybutton.Text = "Apply";
			this.applybutton.UseVisualStyleBackColor = true;
			this.applybutton.Click += new System.EventHandler(this.ApplybuttonClick);
			// 
			// closebutton
			// 
			this.closebutton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.closebutton.Location = new System.Drawing.Point(41, 106);
			this.closebutton.Name = "closebutton";
			this.closebutton.Size = new System.Drawing.Size(75, 23);
			this.closebutton.TabIndex = 1;
			this.closebutton.Text = "Cancel";
			this.closebutton.UseVisualStyleBackColor = true;
			this.closebutton.Click += new System.EventHandler(this.ClosebuttonClick);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(32, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(72, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "Release Date";
			// 
			// lockoutdateTimePicker
			// 
			this.lockoutdateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
			this.lockoutdateTimePicker.Location = new System.Drawing.Point(32, 27);
			this.lockoutdateTimePicker.Name = "lockoutdateTimePicker";
			this.lockoutdateTimePicker.Size = new System.Drawing.Size(105, 20);
			this.lockoutdateTimePicker.TabIndex = 3;
			this.lockoutdateTimePicker.ValueChanged += new System.EventHandler(this.LockoutdateTimePickerValueChanged);
			// 
			// lockoutTimePicker
			// 
			this.lockoutTimePicker.CustomFormat = "HH:mm:ss";
			this.lockoutTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.lockoutTimePicker.Location = new System.Drawing.Point(154, 27);
			this.lockoutTimePicker.Margin = new System.Windows.Forms.Padding(2);
			this.lockoutTimePicker.Name = "lockoutTimePicker";
			this.lockoutTimePicker.ShowUpDown = true;
			this.lockoutTimePicker.Size = new System.Drawing.Size(90, 20);
			this.lockoutTimePicker.TabIndex = 4;
			this.lockoutTimePicker.ValueChanged += new System.EventHandler(this.LockoutdateTimePickerValueChanged);
			// 
			// SetToCurrentDateTimebutton
			// 
			this.SetToCurrentDateTimebutton.Location = new System.Drawing.Point(71, 53);
			this.SetToCurrentDateTimebutton.Name = "SetToCurrentDateTimebutton";
			this.SetToCurrentDateTimebutton.Size = new System.Drawing.Size(133, 23);
			this.SetToCurrentDateTimebutton.TabIndex = 5;
			this.SetToCurrentDateTimebutton.Text = "Use Current Date Time";
			this.SetToCurrentDateTimebutton.UseVisualStyleBackColor = true;
			this.SetToCurrentDateTimebutton.Click += new System.EventHandler(this.SetToCurrentDateTimebutton_OnClick);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(154, 9);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(72, 13);
			this.label2.TabIndex = 6;
			this.label2.Text = "Release Time";
			// 
			// DispatchReleaseToAccountingForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.closebutton;
			this.ClientSize = new System.Drawing.Size(276, 141);
			this.ControlBox = false;
			this.Controls.Add(this.label2);
			this.Controls.Add(this.SetToCurrentDateTimebutton);
			this.Controls.Add(this.lockoutTimePicker);
			this.Controls.Add(this.lockoutdateTimePicker);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.closebutton);
			this.Controls.Add(this.applybutton);
			this.MaximizeBox = false;
			this.Name = "DispatchReleaseToAccountingForm";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Release to Accounting";
			this.Load += new System.EventHandler(this.FormLoad);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button applybutton;
		private System.Windows.Forms.Button closebutton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.DateTimePicker lockoutdateTimePicker;

		private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
		private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
		private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
		private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
		private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
		private System.Windows.Forms.DataGridViewCheckBoxColumn SpecialFuelFlag;
		private System.Windows.Forms.DataGridViewCheckBoxColumn InService;
		private System.Windows.Forms.DataGridViewTextBoxColumn FuelingState;
		private System.Windows.Forms.DataGridViewTextBoxColumn Isspt;
		private System.Windows.Forms.DataGridViewTextBoxColumn IssPtNum;
		private System.Windows.Forms.DateTimePicker lockoutTimePicker;
		private System.Windows.Forms.Button SetToCurrentDateTimebutton;
		private System.Windows.Forms.Label label2;
	}
}
namespace Dispatch
{
	partial class OptionalTimesForm
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
			if(disposing && (this.components != null))
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
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.useStartTimeCheckBox = new System.Windows.Forms.CheckBox();
			this.useStopTimeCheckBox = new System.Windows.Forms.CheckBox();
			this.useArrivalTimeCheckBox = new System.Windows.Forms.CheckBox();
			this.selecctOptionalTimesGroupBox = new System.Windows.Forms.GroupBox();
			this.SuspendLayout();
			// 
			// okButton
			// 
			this.okButton.Location = new System.Drawing.Point(56, 168);
			this.okButton.Margin = new System.Windows.Forms.Padding(2);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(65, 28);
			this.okButton.TabIndex = 4;
			this.okButton.Text = "&Ok";
			this.okButton.UseVisualStyleBackColor = true;
			this.okButton.Click += new System.EventHandler(this.OkButtonClick);
			// 
			// cancelButton
			// 
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(172, 168);
			this.cancelButton.Margin = new System.Windows.Forms.Padding(2);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(65, 28);
			this.cancelButton.TabIndex = 5;
			this.cancelButton.Text = "&Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			this.cancelButton.Click += new System.EventHandler(this.CancelButtonClick);
			// 
			// useStartTimeCheckBox
			// 
			this.useStartTimeCheckBox.AutoSize = true;
			this.useStartTimeCheckBox.Location = new System.Drawing.Point(39, 71);
			this.useStartTimeCheckBox.Margin = new System.Windows.Forms.Padding(2);
			this.useStartTimeCheckBox.Name = "useStartTimeCheckBox";
			this.useStartTimeCheckBox.Size = new System.Drawing.Size(96, 17);
			this.useStartTimeCheckBox.TabIndex = 2;
			this.useStartTimeCheckBox.Text = "Use Start Time";
			this.useStartTimeCheckBox.UseVisualStyleBackColor = true;
			// 
			// useStopTimeCheckBox
			// 
			this.useStopTimeCheckBox.AutoSize = true;
			this.useStopTimeCheckBox.Location = new System.Drawing.Point(39, 107);
			this.useStopTimeCheckBox.Margin = new System.Windows.Forms.Padding(2);
			this.useStopTimeCheckBox.Name = "useStopTimeCheckBox";
			this.useStopTimeCheckBox.Size = new System.Drawing.Size(96, 17);
			this.useStopTimeCheckBox.TabIndex = 3;
			this.useStopTimeCheckBox.Text = "Use Stop Time";
			this.useStopTimeCheckBox.UseVisualStyleBackColor = true;
			// 
			// useArrivalTimeCheckBox
			// 
			this.useArrivalTimeCheckBox.AutoSize = true;
			this.useArrivalTimeCheckBox.Location = new System.Drawing.Point(39, 38);
			this.useArrivalTimeCheckBox.Margin = new System.Windows.Forms.Padding(2);
			this.useArrivalTimeCheckBox.Name = "useArrivalTimeCheckBox";
			this.useArrivalTimeCheckBox.Size = new System.Drawing.Size(103, 17);
			this.useArrivalTimeCheckBox.TabIndex = 1;
			this.useArrivalTimeCheckBox.Text = "Use Arrival Time";
			this.useArrivalTimeCheckBox.UseVisualStyleBackColor = true;
			// 
			// selecctOptionalTimesGroupBox
			// 
			this.selecctOptionalTimesGroupBox.Location = new System.Drawing.Point(12, 10);
			this.selecctOptionalTimesGroupBox.Name = "selecctOptionalTimesGroupBox";
			this.selecctOptionalTimesGroupBox.Size = new System.Drawing.Size(268, 143);
			this.selecctOptionalTimesGroupBox.TabIndex = 18;
			this.selecctOptionalTimesGroupBox.TabStop = false;
			this.selecctOptionalTimesGroupBox.Text = "Select Optional Times to Use";
			// 
			// OptionalTimesForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(292, 211);
			this.ControlBox = false;
			this.Controls.Add(this.useStartTimeCheckBox);
			this.Controls.Add(this.useStopTimeCheckBox);
			this.Controls.Add(this.useArrivalTimeCheckBox);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.selecctOptionalTimesGroupBox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "OptionalTimesForm";
			this.ShowIcon = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Optional Times";
			this.Load += new System.EventHandler(this.OptionalTimesFormLoad);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.CheckBox useStartTimeCheckBox;
		private System.Windows.Forms.CheckBox useStopTimeCheckBox;
		private System.Windows.Forms.CheckBox useArrivalTimeCheckBox;
		private System.Windows.Forms.GroupBox selecctOptionalTimesGroupBox;
	}
}
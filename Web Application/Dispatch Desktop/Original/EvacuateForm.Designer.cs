namespace DispatchPrototype
{
	partial class EvacuateForm
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
			this.Evacuatebutton = new System.Windows.Forms.Button();
			this.Mergebutton = new System.Windows.Forms.Button();
			this.Exitbutton = new System.Windows.Forms.Button();
			this.StatustextBox = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.FromDatedateTimePicker = new System.Windows.Forms.DateTimePicker();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.TargetSourcetextBox = new System.Windows.Forms.TextBox();
			this.ToDatedateTimePicker = new System.Windows.Forms.DateTimePicker();
			this.label4 = new System.Windows.Forms.Label();
			this.Browsebutton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// Evacuatebutton
			// 
			this.Evacuatebutton.Location = new System.Drawing.Point(542, 128);
			this.Evacuatebutton.Name = "Evacuatebutton";
			this.Evacuatebutton.Size = new System.Drawing.Size(75, 23);
			this.Evacuatebutton.TabIndex = 0;
			this.Evacuatebutton.Text = "Evacuate";
			this.Evacuatebutton.UseVisualStyleBackColor = true;
			this.Evacuatebutton.Click += new System.EventHandler(this.Evacuatebutton_Click);
			// 
			// Mergebutton
			// 
			this.Mergebutton.Location = new System.Drawing.Point(542, 175);
			this.Mergebutton.Name = "Mergebutton";
			this.Mergebutton.Size = new System.Drawing.Size(75, 23);
			this.Mergebutton.TabIndex = 1;
			this.Mergebutton.Text = "Merge";
			this.Mergebutton.UseVisualStyleBackColor = true;
			this.Mergebutton.Click += new System.EventHandler(this.Mergebutton_Click);
			// 
			// Exitbutton
			// 
			this.Exitbutton.Location = new System.Drawing.Point(542, 225);
			this.Exitbutton.Name = "Exitbutton";
			this.Exitbutton.Size = new System.Drawing.Size(75, 23);
			this.Exitbutton.TabIndex = 2;
			this.Exitbutton.Text = "Exit";
			this.Exitbutton.UseVisualStyleBackColor = true;
			this.Exitbutton.Click += new System.EventHandler(this.Exitbutton_Click);
			// 
			// StatustextBox
			// 
			this.StatustextBox.AcceptsReturn = true;
			this.StatustextBox.BackColor = System.Drawing.SystemColors.Window;
			this.StatustextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.StatustextBox.ForeColor = System.Drawing.SystemColors.WindowText;
			this.StatustextBox.Location = new System.Drawing.Point(32, 128);
			this.StatustextBox.Multiline = true;
			this.StatustextBox.Name = "StatustextBox";
			this.StatustextBox.ReadOnly = true;
			this.StatustextBox.Size = new System.Drawing.Size(486, 273);
			this.StatustextBox.TabIndex = 3;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(29, 112);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(37, 13);
			this.label1.TabIndex = 4;
			this.label1.Text = "Status";
			// 
			// FromDatedateTimePicker
			// 
			this.FromDatedateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
			this.FromDatedateTimePicker.Location = new System.Drawing.Point(32, 33);
			this.FromDatedateTimePicker.Name = "FromDatedateTimePicker";
			this.FromDatedateTimePicker.Size = new System.Drawing.Size(102, 20);
			this.FromDatedateTimePicker.TabIndex = 5;
			this.FromDatedateTimePicker.ValueChanged += new System.EventHandler(this.FromDatedateTimePickerValueChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(29, 17);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(59, 13);
			this.label2.TabIndex = 6;
			this.label2.Text = "From Date:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(29, 64);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(125, 13);
			this.label3.TabIndex = 7;
			this.label3.Text = "Target/Source Directory:";
			// 
			// TargetSourcetextBox
			// 
			this.TargetSourcetextBox.Location = new System.Drawing.Point(32, 81);
			this.TargetSourcetextBox.Name = "TargetSourcetextBox";
			this.TargetSourcetextBox.Size = new System.Drawing.Size(405, 20);
			this.TargetSourcetextBox.TabIndex = 8;
			// 
			// ToDatedateTimePicker
			// 
			this.ToDatedateTimePicker.Enabled = false;
			this.ToDatedateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
			this.ToDatedateTimePicker.Location = new System.Drawing.Point(211, 33);
			this.ToDatedateTimePicker.Name = "ToDatedateTimePicker";
			this.ToDatedateTimePicker.Size = new System.Drawing.Size(102, 20);
			this.ToDatedateTimePicker.TabIndex = 9;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(208, 17);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(49, 13);
			this.label4.TabIndex = 10;
			this.label4.Text = "To Date:";
			// 
			// Browsebutton
			// 
			this.Browsebutton.Location = new System.Drawing.Point(461, 81);
			this.Browsebutton.Name = "Browsebutton";
			this.Browsebutton.Size = new System.Drawing.Size(38, 23);
			this.Browsebutton.TabIndex = 11;
			this.Browsebutton.Text = "...";
			this.Browsebutton.UseVisualStyleBackColor = true;
			this.Browsebutton.Click += new System.EventHandler(this.Browsebutton_Click);
			// 
			// EvacuateForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(639, 429);
			this.ControlBox = false;
			this.Controls.Add(this.Browsebutton);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.ToDatedateTimePicker);
			this.Controls.Add(this.TargetSourcetextBox);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.FromDatedateTimePicker);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.StatustextBox);
			this.Controls.Add(this.Exitbutton);
			this.Controls.Add(this.Mergebutton);
			this.Controls.Add(this.Evacuatebutton);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "EvacuateForm";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Evacuate";
			this.Load += new System.EventHandler(this.EvacuateForm_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button Evacuatebutton;
		private System.Windows.Forms.Button Mergebutton;
		private System.Windows.Forms.Button Exitbutton;
		private System.Windows.Forms.TextBox StatustextBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.DateTimePicker FromDatedateTimePicker;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox TargetSourcetextBox;
		private System.Windows.Forms.DateTimePicker ToDatedateTimePicker;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button Browsebutton;
	}
}
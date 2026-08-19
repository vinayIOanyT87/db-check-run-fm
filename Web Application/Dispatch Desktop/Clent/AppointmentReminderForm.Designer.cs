namespace Dispatch
{
	partial class AppointmentReminderForm
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
         this.components = new System.ComponentModel.Container();
         this.buttonCancel = new System.Windows.Forms.Button();
         this.AppointmentlistView = new System.Windows.Forms.ListView();
         this.SleepNumber = new System.Windows.Forms.TextBox();
         this.SleepTimeValue = new System.Windows.Forms.ComboBox();
         this.SleepButton = new System.Windows.Forms.Button();
         this.Dismissbutton = new System.Windows.Forms.Button();
         this.groupBox1 = new System.Windows.Forms.GroupBox();
         this.timer1 = new System.Windows.Forms.Timer(this.components);
         this.groupBox1.SuspendLayout();
         this.SuspendLayout();
         // 
         // buttonCancel
         // 
         this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.buttonCancel.Location = new System.Drawing.Point(344, 433);
         this.buttonCancel.Name = "buttonCancel";
         this.buttonCancel.Size = new System.Drawing.Size(75, 23);
         this.buttonCancel.TabIndex = 6;
         this.buttonCancel.Text = "&Close";
         this.buttonCancel.UseVisualStyleBackColor = true;
         this.buttonCancel.Click += new System.EventHandler(this.OnCancelClick);
         // 
         // AppointmentlistView
         // 
         this.AppointmentlistView.FullRowSelect = true;
         this.AppointmentlistView.Location = new System.Drawing.Point(18, 16);
         this.AppointmentlistView.Name = "AppointmentlistView";
         this.AppointmentlistView.Size = new System.Drawing.Size(726, 259);
         this.AppointmentlistView.TabIndex = 1;
         this.AppointmentlistView.UseCompatibleStateImageBehavior = false;
         this.AppointmentlistView.View = System.Windows.Forms.View.Details;
         this.AppointmentlistView.ColumnWidthChanged += new System.Windows.Forms.ColumnWidthChangedEventHandler(this.OnAppointmentColumnWidthChanged);
         this.AppointmentlistView.SelectedIndexChanged += new System.EventHandler(this.OnSelectedIndexChanged);
         // 
         // SleepNumber
         // 
         this.SleepNumber.Location = new System.Drawing.Point(203, 33);
         this.SleepNumber.MaxLength = 3;
         this.SleepNumber.Name = "SleepNumber";
         this.SleepNumber.Size = new System.Drawing.Size(42, 20);
         this.SleepNumber.TabIndex = 3;
         this.SleepNumber.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
         // 
         // SleepTimeValue
         // 
         this.SleepTimeValue.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
         this.SleepTimeValue.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
         this.SleepTimeValue.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.SleepTimeValue.FormattingEnabled = true;
         this.SleepTimeValue.Location = new System.Drawing.Point(266, 32);
         this.SleepTimeValue.Name = "SleepTimeValue";
         this.SleepTimeValue.Size = new System.Drawing.Size(89, 21);
         this.SleepTimeValue.TabIndex = 4;
         // 
         // SleepButton
         // 
         this.SleepButton.Location = new System.Drawing.Point(47, 33);
         this.SleepButton.Name = "SleepButton";
         this.SleepButton.Size = new System.Drawing.Size(124, 23);
         this.SleepButton.TabIndex = 2;
         this.SleepButton.Text = "&Notify Again In:";
         this.SleepButton.UseVisualStyleBackColor = true;
         this.SleepButton.Click += new System.EventHandler(this.OnSleepButtonClick);
         // 
         // Dismissbutton
         // 
         this.Dismissbutton.Location = new System.Drawing.Point(47, 78);
         this.Dismissbutton.Name = "Dismissbutton";
         this.Dismissbutton.Size = new System.Drawing.Size(124, 23);
         this.Dismissbutton.TabIndex = 5;
         this.Dismissbutton.Text = "&Dismiss Appointment";
         this.Dismissbutton.UseVisualStyleBackColor = true;
         this.Dismissbutton.Click += new System.EventHandler(this.OnDismissSelectedItemClicked);
         // 
         // groupBox1
         // 
         this.groupBox1.Controls.Add(this.SleepTimeValue);
         this.groupBox1.Controls.Add(this.SleepButton);
         this.groupBox1.Controls.Add(this.SleepNumber);
         this.groupBox1.Controls.Add(this.Dismissbutton);
         this.groupBox1.Location = new System.Drawing.Point(194, 288);
         this.groupBox1.Name = "groupBox1";
         this.groupBox1.Size = new System.Drawing.Size(374, 118);
         this.groupBox1.TabIndex = 7;
         this.groupBox1.TabStop = false;
         this.groupBox1.Text = "Selected Items";
         // 
         // AppointmentReminderForm
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.CancelButton = this.buttonCancel;
         this.ClientSize = new System.Drawing.Size(762, 468);
         this.ControlBox = false;
         this.Controls.Add(this.AppointmentlistView);
         this.Controls.Add(this.buttonCancel);
         this.Controls.Add(this.groupBox1);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.Name = "AppointmentReminderForm";
         this.ShowIcon = false;
         this.ShowInTaskbar = false;
         this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "Appointment Reminder";
         this.Activated += new System.EventHandler(this.OnActivated);
         this.groupBox1.ResumeLayout(false);
         this.groupBox1.PerformLayout();
         this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.ListView AppointmentlistView;
		private System.Windows.Forms.TextBox SleepNumber;
		private System.Windows.Forms.ComboBox SleepTimeValue;
		private System.Windows.Forms.Button SleepButton;
		private System.Windows.Forms.Button Dismissbutton;
		private System.Windows.Forms.GroupBox groupBox1;
      private System.Windows.Forms.Timer timer1;
	}
}

namespace DispatchPrototype
{
	partial class ControlLogForm
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
         this.ControlLogCancelButton = new System.Windows.Forms.Button();
         this.StartDatePicker = new System.Windows.Forms.DateTimePicker();
         this.StopDatePicker = new System.Windows.Forms.DateTimePicker();
         this.label1 = new System.Windows.Forms.Label();
         this.label2 = new System.Windows.Forms.Label();
         this.ControllersLogListView = new System.Windows.Forms.ListView();
         this.AddButton = new System.Windows.Forms.Button();
         this.Editbutton = new System.Windows.Forms.Button();
         this.Deletebutton = new System.Windows.Forms.Button();
         this.ShowDeletedcheckBox = new System.Windows.Forms.CheckBox();
         this.PrintButton = new System.Windows.Forms.Button();
         this.SuspendLayout();
         // 
         // ControlLogCancelButton
         // 
         this.ControlLogCancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.ControlLogCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.ControlLogCancelButton.Location = new System.Drawing.Point(526, 325);
         this.ControlLogCancelButton.Name = "ControlLogCancelButton";
         this.ControlLogCancelButton.Size = new System.Drawing.Size(75, 23);
         this.ControlLogCancelButton.TabIndex = 9;
         this.ControlLogCancelButton.Text = "&Close";
         this.ControlLogCancelButton.UseVisualStyleBackColor = true;
         this.ControlLogCancelButton.Click += new System.EventHandler(this.OnCancelClicked);
         // 
         // StartDatePicker
         // 
         this.StartDatePicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
         this.StartDatePicker.Location = new System.Drawing.Point(15, 47);
         this.StartDatePicker.Name = "StartDatePicker";
         this.StartDatePicker.Size = new System.Drawing.Size(109, 20);
         this.StartDatePicker.TabIndex = 1;
         // 
         // StopDatePicker
         // 
         this.StopDatePicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
         this.StopDatePicker.Location = new System.Drawing.Point(149, 47);
         this.StopDatePicker.Name = "StopDatePicker";
         this.StopDatePicker.Size = new System.Drawing.Size(109, 20);
         this.StopDatePicker.TabIndex = 2;
         // 
         // label1
         // 
         this.label1.AutoSize = true;
         this.label1.Location = new System.Drawing.Point(12, 28);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(58, 13);
         this.label1.TabIndex = 4;
         this.label1.Text = "Start Date:";
         // 
         // label2
         // 
         this.label2.AutoSize = true;
         this.label2.Location = new System.Drawing.Point(146, 28);
         this.label2.Name = "label2";
         this.label2.Size = new System.Drawing.Size(58, 13);
         this.label2.TabIndex = 5;
         this.label2.Text = "Stop Date:";
         // 
         // ControllersLogListView
         // 
         this.ControllersLogListView.FullRowSelect = true;
         this.ControllersLogListView.GridLines = true;
         this.ControllersLogListView.HideSelection = false;
         this.ControllersLogListView.Location = new System.Drawing.Point(15, 73);
         this.ControllersLogListView.Name = "ControllersLogListView";
         this.ControllersLogListView.Size = new System.Drawing.Size(497, 285);
         this.ControllersLogListView.TabIndex = 4;
         this.ControllersLogListView.UseCompatibleStateImageBehavior = false;
         this.ControllersLogListView.View = System.Windows.Forms.View.List;
         this.ControllersLogListView.ColumnWidthChanged += new System.Windows.Forms.ColumnWidthChangedEventHandler(this.OnColumnWidthChanged);
         this.ControllersLogListView.SelectedIndexChanged += new System.EventHandler(this.SelectedIndexChanged);
         this.ControllersLogListView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.OnControllerLogColumnClick);
         // 
         // AddButton
         // 
         this.AddButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.AddButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.AddButton.Location = new System.Drawing.Point(526, 73);
         this.AddButton.Name = "AddButton";
         this.AddButton.Size = new System.Drawing.Size(75, 23);
         this.AddButton.TabIndex = 5;
         this.AddButton.Text = "&Add Memo";
         this.AddButton.UseVisualStyleBackColor = true;
         this.AddButton.Click += new System.EventHandler(this.OnAddMemo);
         // 
         // Editbutton
         // 
         this.Editbutton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.Editbutton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.Editbutton.Location = new System.Drawing.Point(526, 112);
         this.Editbutton.Name = "Editbutton";
         this.Editbutton.Size = new System.Drawing.Size(75, 23);
         this.Editbutton.TabIndex = 6;
         this.Editbutton.Text = "&Edit";
         this.Editbutton.UseVisualStyleBackColor = true;
         this.Editbutton.Click += new System.EventHandler(this.OnEditClicked);
         // 
         // Deletebutton
         // 
         this.Deletebutton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.Deletebutton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.Deletebutton.Location = new System.Drawing.Point(526, 154);
         this.Deletebutton.Name = "Deletebutton";
         this.Deletebutton.Size = new System.Drawing.Size(75, 23);
         this.Deletebutton.TabIndex = 7;
         this.Deletebutton.Text = "&Delete";
         this.Deletebutton.UseVisualStyleBackColor = true;
         this.Deletebutton.Click += new System.EventHandler(this.OnDeleteButtonClicked);
         // 
         // ShowDeletedcheckBox
         // 
         this.ShowDeletedcheckBox.AutoSize = true;
         this.ShowDeletedcheckBox.Location = new System.Drawing.Point(391, 47);
         this.ShowDeletedcheckBox.Name = "ShowDeletedcheckBox";
         this.ShowDeletedcheckBox.Size = new System.Drawing.Size(121, 17);
         this.ShowDeletedcheckBox.TabIndex = 3;
         this.ShowDeletedcheckBox.Text = "Show Deleted Items";
         this.ShowDeletedcheckBox.UseVisualStyleBackColor = true;
         this.ShowDeletedcheckBox.CheckStateChanged += new System.EventHandler(this.OnShowDeletedItemsCheckBoxStateChanged);
         // 
         // PrintButton
         // 
         this.PrintButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.PrintButton.Location = new System.Drawing.Point(527, 196);
         this.PrintButton.Name = "PrintButton";
         this.PrintButton.Size = new System.Drawing.Size(75, 23);
         this.PrintButton.TabIndex = 8;
         this.PrintButton.Text = "&Print";
         this.PrintButton.UseVisualStyleBackColor = true;
         this.PrintButton.Click += new System.EventHandler(this.PrintButton_Click);
         // 
         // ControlLogForm
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.CancelButton = this.ControlLogCancelButton;
         this.ClientSize = new System.Drawing.Size(612, 376);
         this.ControlBox = false;
         this.Controls.Add(this.PrintButton);
         this.Controls.Add(this.ShowDeletedcheckBox);
         this.Controls.Add(this.Deletebutton);
         this.Controls.Add(this.Editbutton);
         this.Controls.Add(this.AddButton);
         this.Controls.Add(this.ControllersLogListView);
         this.Controls.Add(this.label2);
         this.Controls.Add(this.label1);
         this.Controls.Add(this.StopDatePicker);
         this.Controls.Add(this.StartDatePicker);
         this.Controls.Add(this.ControlLogCancelButton);
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "ControlLogForm";
         this.ShowInTaskbar = false;
         this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "Control Log";
         this.Load += new System.EventHandler(this.ControlLogForm_Load);
         this.ResumeLayout(false);
         this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button ControlLogCancelButton;
		private System.Windows.Forms.DateTimePicker StartDatePicker;
		private System.Windows.Forms.DateTimePicker StopDatePicker;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ListView ControllersLogListView;
		private System.Windows.Forms.Button AddButton;
		private System.Windows.Forms.Button Editbutton;
		private System.Windows.Forms.Button Deletebutton;
		private System.Windows.Forms.CheckBox ShowDeletedcheckBox;
      private System.Windows.Forms.Button PrintButton;
	}
}
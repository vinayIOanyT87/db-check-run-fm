namespace Dispatch
{
	partial class AddMemoForm
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
         this.AddDialogCancelButton = new System.Windows.Forms.Button();
         this.label1 = new System.Windows.Forms.Label();
         this.ControllerTextBox = new System.Windows.Forms.TextBox();
         this.MemoDateTimeSelection = new System.Windows.Forms.DateTimePicker();
         this.label2 = new System.Windows.Forms.Label();
         this.label3 = new System.Windows.Forms.Label();
         this.MemotextBox = new System.Windows.Forms.TextBox();
         this.OKbutton = new System.Windows.Forms.Button();
         this.SuspendLayout();
         // 
         // AddDialogCancelButton
         // 
         this.AddDialogCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.AddDialogCancelButton.Location = new System.Drawing.Point( 328, 104 );
         this.AddDialogCancelButton.Name = "AddDialogCancelButton";
         this.AddDialogCancelButton.Size = new System.Drawing.Size( 75, 23 );
         this.AddDialogCancelButton.TabIndex = 5;
         this.AddDialogCancelButton.Text = "&Cancel";
         this.AddDialogCancelButton.UseVisualStyleBackColor = true;
         this.AddDialogCancelButton.Click += new System.EventHandler( this.OnCancelClicked );
         // 
         // label1
         // 
         this.label1.AutoSize = true;
         this.label1.Location = new System.Drawing.Point( 15, 23 );
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size( 54, 13 );
         this.label1.TabIndex = 1;
         this.label1.Text = "Controller:";
         // 
         // ControllerTextBox
         // 
         this.ControllerTextBox.Location = new System.Drawing.Point( 84, 20 );
         this.ControllerTextBox.Name = "ControllerTextBox";
         this.ControllerTextBox.ReadOnly = true;
         this.ControllerTextBox.Size = new System.Drawing.Size( 173, 20 );
         this.ControllerTextBox.TabIndex = 1;
         // 
         // MemoDateTimeSelection
         // 
         this.MemoDateTimeSelection.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
         this.MemoDateTimeSelection.Location = new System.Drawing.Point( 392, 20 );
         this.MemoDateTimeSelection.Name = "MemoDateTimeSelection";
         this.MemoDateTimeSelection.ShowUpDown = true;
         this.MemoDateTimeSelection.Size = new System.Drawing.Size( 173, 20 );
         this.MemoDateTimeSelection.TabIndex = 2;
         // 
         // label2
         // 
         this.label2.AutoSize = true;
         this.label2.Location = new System.Drawing.Point( 326, 23 );
         this.label2.Name = "label2";
         this.label2.Size = new System.Drawing.Size( 61, 13 );
         this.label2.TabIndex = 4;
         this.label2.Text = "Date/Time:";
         // 
         // label3
         // 
         this.label3.AutoSize = true;
         this.label3.Location = new System.Drawing.Point( 16, 66 );
         this.label3.Name = "label3";
         this.label3.Size = new System.Drawing.Size( 39, 13 );
         this.label3.TabIndex = 5;
         this.label3.Text = "Memo:";
         // 
         // MemotextBox
         // 
         this.MemotextBox.Location = new System.Drawing.Point( 84, 63 );
         this.MemotextBox.MaxLength = 150;
         this.MemotextBox.Name = "MemotextBox";
         this.MemotextBox.Size = new System.Drawing.Size( 483, 20 );
         this.MemotextBox.TabIndex = 3;
         // 
         // OKbutton
         // 
         this.OKbutton.DialogResult = System.Windows.Forms.DialogResult.OK;
         this.OKbutton.Location = new System.Drawing.Point( 183, 104 );
         this.OKbutton.Name = "OKbutton";
         this.OKbutton.Size = new System.Drawing.Size( 75, 23 );
         this.OKbutton.TabIndex = 4;
         this.OKbutton.Text = "&OK";
         this.OKbutton.UseVisualStyleBackColor = true;
         this.OKbutton.Click += new System.EventHandler( this.OnOkClicked );
         // 
         // AddMemoForm
         // 
         this.AcceptButton = this.OKbutton;
         this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.CancelButton = this.AddDialogCancelButton;
         this.ClientSize = new System.Drawing.Size( 586, 155 );
         this.ControlBox = false;
         this.Controls.Add( this.OKbutton );
         this.Controls.Add( this.MemotextBox );
         this.Controls.Add( this.label3 );
         this.Controls.Add( this.label2 );
         this.Controls.Add( this.MemoDateTimeSelection );
         this.Controls.Add( this.ControllerTextBox );
         this.Controls.Add( this.label1 );
         this.Controls.Add( this.AddDialogCancelButton );
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.Name = "AddMemoForm";
         this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "Add Memo";
		 this.Shown += new System.EventHandler(this.OnShown);
         this.ResumeLayout( false );
         this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button AddDialogCancelButton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox ControllerTextBox;
		private System.Windows.Forms.DateTimePicker MemoDateTimeSelection;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox MemotextBox;
		private System.Windows.Forms.Button OKbutton;
	}
}
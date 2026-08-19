namespace DispatchPrototype
{
   partial class StandbyRegistrationSelectionForm
   {
      /// <summary>
      /// Required designer variable.
      /// </summary>
      private System.ComponentModel.IContainer components = null;

      /// <summary>
      /// Clean up any resources being used.
      /// </summary>
      /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
      protected override void Dispose ( bool disposing )
      {
         if (disposing && (components != null))
         {
            components.Dispose();
         }
         base.Dispose( disposing );
      }

      #region Windows Form Designer generated code

      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent ()
      {
			this.OKButton = new System.Windows.Forms.Button();
			this.CancelBtn = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.OperatorNameTextBox = new System.Windows.Forms.TextBox();
			this.EmployeeIDTextBox = new System.Windows.Forms.TextBox();
			this.RegistrationIDComboBox = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// OKButton
			// 
			this.OKButton.Location = new System.Drawing.Point(73, 164);
			this.OKButton.Name = "OKButton";
			this.OKButton.Size = new System.Drawing.Size(75, 23);
			this.OKButton.TabIndex = 0;
			this.OKButton.Text = "&OK";
			this.OKButton.UseVisualStyleBackColor = true;
			this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
			// 
			// CancelBtn
			// 
			this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.CancelBtn.Location = new System.Drawing.Point(204, 164);
			this.CancelBtn.Name = "CancelButton";
			this.CancelBtn.Size = new System.Drawing.Size(75, 23);
			this.CancelBtn.TabIndex = 1;
			this.CancelBtn.Text = "&Cancel";
			this.CancelBtn.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(29, 35);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(82, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "Operator Name:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(29, 68);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(70, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "Employee ID:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(29, 103);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(80, 13);
			this.label3.TabIndex = 4;
			this.label3.Text = "Registration ID:";
			// 
			// OperatorNameTextBox
			// 
			this.OperatorNameTextBox.Location = new System.Drawing.Point(146, 30);
			this.OperatorNameTextBox.Name = "OperatorNameTextBox";
			this.OperatorNameTextBox.ReadOnly = true;
			this.OperatorNameTextBox.Size = new System.Drawing.Size(177, 20);
			this.OperatorNameTextBox.TabIndex = 5;
			// 
			// EmployeeIDTextBox
			// 
			this.EmployeeIDTextBox.Location = new System.Drawing.Point(146, 64);
			this.EmployeeIDTextBox.Name = "EmployeeIDTextBox";
			this.EmployeeIDTextBox.ReadOnly = true;
			this.EmployeeIDTextBox.Size = new System.Drawing.Size(177, 20);
			this.EmployeeIDTextBox.TabIndex = 6;
			// 
			// RegistrationIDComboBox
			// 
			this.RegistrationIDComboBox.FormattingEnabled = true;
			this.RegistrationIDComboBox.Location = new System.Drawing.Point(146, 99);
			this.RegistrationIDComboBox.Name = "RegistrationIDComboBox";
			this.RegistrationIDComboBox.Size = new System.Drawing.Size(177, 21);
			this.RegistrationIDComboBox.TabIndex = 7;
			// 
			// StandbyRegistrationSelectionForm
			// 
			this.AcceptButton = this.OKButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(362, 216);
			this.ControlBox = false;
			this.Controls.Add(this.RegistrationIDComboBox);
			this.Controls.Add(this.EmployeeIDTextBox);
			this.Controls.Add(this.OperatorNameTextBox);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.CancelBtn);
			this.Controls.Add(this.OKButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "StandbyRegistrationSelectionForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Select Registration ID";
			this.Load += new System.EventHandler(this.StandbyRegistrationSelectionForm_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.Button OKButton;
      private System.Windows.Forms.Button CancelBtn;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.Label label3;
      private System.Windows.Forms.TextBox OperatorNameTextBox;
      private System.Windows.Forms.TextBox EmployeeIDTextBox;
      private System.Windows.Forms.ComboBox RegistrationIDComboBox;
   }
}
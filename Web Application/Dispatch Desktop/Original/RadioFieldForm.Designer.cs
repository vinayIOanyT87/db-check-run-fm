namespace DispatchPrototype
{
   partial class RadioFieldForm
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
			this.RadioNumberTextBox = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// OKButton
			// 
			this.OKButton.Location = new System.Drawing.Point(45, 94);
			this.OKButton.Name = "OKButton";
			this.OKButton.Size = new System.Drawing.Size(75, 23);
			this.OKButton.TabIndex = 1;
			this.OKButton.Text = "&OK";
			this.OKButton.UseVisualStyleBackColor = true;
			this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
			// 
			// CancelBtn
			// 
			this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.CancelBtn.Location = new System.Drawing.Point(190, 94);
			this.CancelBtn.Name = "CancelBtn";
			this.CancelBtn.Size = new System.Drawing.Size(75, 23);
			this.CancelBtn.TabIndex = 2;
			this.CancelBtn.Text = "&Cancel";
			this.CancelBtn.UseVisualStyleBackColor = true;
			this.CancelBtn.Click += new System.EventHandler(this.CancelBtn_Click);
			// 
			// RadioNumberTextBox
			// 
			this.RadioNumberTextBox.Location = new System.Drawing.Point(45, 39);
			this.RadioNumberTextBox.Name = "RadioNumberTextBox";
			this.RadioNumberTextBox.Size = new System.Drawing.Size(220, 20);
			this.RadioNumberTextBox.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(45, 20);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(75, 13);
			this.label1.TabIndex = 3;
			this.label1.Text = "Radio Number";
			// 
			// RadioFieldForm
			// 
			this.AcceptButton = this.OKButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.CancelBtn;
			this.ClientSize = new System.Drawing.Size(314, 157);
			this.ControlBox = false;
			this.Controls.Add(this.label1);
			this.Controls.Add(this.RadioNumberTextBox);
			this.Controls.Add(this.CancelBtn);
			this.Controls.Add(this.OKButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "RadioFieldForm";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Radio Number";
			this.Load += new System.EventHandler(this.RadioFieldForm_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.Button OKButton;
      private System.Windows.Forms.Button CancelBtn;
      private System.Windows.Forms.TextBox RadioNumberTextBox;
      private System.Windows.Forms.Label label1;
   }
}
namespace DataImportExportWizard
{
    partial class ReEncryptPasswordForm
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
            this.StartButton = new System.Windows.Forms.Button();
            this.StatusLabel = new System.Windows.Forms.Label();
            this.ExitButton = new System.Windows.Forms.Button();
            this.statusProcessingUserControl = new DataImportExportWizard.StatusProcessingUserControl();
            this.SuspendLayout();
            // 
            // StartButton
            // 
            this.StartButton.Location = new System.Drawing.Point(337, 386);
            this.StartButton.Name = "StartButton";
            this.StartButton.Size = new System.Drawing.Size(75, 23);
            this.StartButton.TabIndex = 0;
            this.StartButton.Text = "&Start";
            this.StartButton.UseVisualStyleBackColor = true;
            this.StartButton.Click += new System.EventHandler(this.StartButton_Click);
            // 
            // StatusLabel
            // 
            this.StatusLabel.AutoSize = true;
            this.StatusLabel.Location = new System.Drawing.Point(12, 9);
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(40, 13);
            this.StatusLabel.TabIndex = 1;
            this.StatusLabel.Text = "Status:";
            // 
            // ExitButton
            // 
            this.ExitButton.Location = new System.Drawing.Point(256, 386);
            this.ExitButton.Name = "ExitButton";
            this.ExitButton.Size = new System.Drawing.Size(75, 23);
            this.ExitButton.TabIndex = 3;
            this.ExitButton.Text = "E&xit";
            this.ExitButton.UseVisualStyleBackColor = true;
            this.ExitButton.Click += new System.EventHandler(this.ExitButton_Click);
            // 
            // statusProcessingUserControl
            // 
            this.statusProcessingUserControl.Location = new System.Drawing.Point(12, 28);
            this.statusProcessingUserControl.Name = "statusProcessingUserControl";
            this.statusProcessingUserControl.Size = new System.Drawing.Size(400, 352);
            this.statusProcessingUserControl.TabIndex = 4;
            // 
            // ReEncryptPasswordForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(426, 415);
            this.Controls.Add(this.statusProcessingUserControl);
            this.Controls.Add(this.ExitButton);
            this.Controls.Add(this.StatusLabel);
            this.Controls.Add(this.StartButton);
            this.Name = "ReEncryptPasswordForm";
            this.Text = "ReEncrypt Encrypted Data";
            this.Load += new System.EventHandler(this.ReEncryptPasswordForm_Load);
            this.Shown += new System.EventHandler(this.ReEncryptPasswordForm_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button StartButton;
        private System.Windows.Forms.Label StatusLabel;
        private System.Windows.Forms.Button ExitButton;
        private StatusProcessingUserControl statusProcessingUserControl;
    }
}
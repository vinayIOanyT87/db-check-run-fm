namespace DataImportExportWizard
{
    partial class StatusProcessingUserControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.StatusTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // StatusTextBox
            // 
            this.StatusTextBox.AcceptsReturn = true;
            this.StatusTextBox.AcceptsTab = true;
            this.StatusTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.StatusTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.StatusTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.StatusTextBox.Location = new System.Drawing.Point(0, 0);
            this.StatusTextBox.Multiline = true;
            this.StatusTextBox.Name = "StatusTextBox";
            this.StatusTextBox.Size = new System.Drawing.Size(400, 352);
            this.StatusTextBox.TabIndex = 1;
            // 
            // StatusProcessingUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.StatusTextBox);
            this.Name = "StatusProcessingUserControl";
            this.Size = new System.Drawing.Size(400, 352);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox StatusTextBox;
    }
}

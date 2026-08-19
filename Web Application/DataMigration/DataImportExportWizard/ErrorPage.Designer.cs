namespace DataImportExportWizard
{
    partial class ErrorPage
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
            this.ErrorMessageLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ErrorMessageLabel
            // 
            this.ErrorMessageLabel.AutoSize = true;
            this.ErrorMessageLabel.Location = new System.Drawing.Point(14, 53);
            this.ErrorMessageLabel.Name = "Error";
            this.ErrorMessageLabel.Size = new System.Drawing.Size(28, 13);
            this.ErrorMessageLabel.TabIndex = 1;
            this.ErrorMessageLabel.Text = "error";
            // 
            // FourthPage
            // 
            this.Controls.Add(this.ErrorMessageLabel);
            this.Name = "ErrorPage";
            this.Size = new System.Drawing.Size(450, 203);
            this.SetActive += new System.ComponentModel.CancelEventHandler(this.ErrorPage_SetActive);
            this.WizardBack += new Wizard.UI.WizardPageEventHandler(this.ErrorPage_WizardBack);
            this.Controls.SetChildIndex(this.ErrorMessageLabel, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label ErrorMessageLabel;
    }
}

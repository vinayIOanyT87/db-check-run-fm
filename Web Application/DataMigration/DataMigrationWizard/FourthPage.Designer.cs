namespace DataMigration
{
    partial class FourthPage
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
            this.Error = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Error
            // 
            this.Error.AutoSize = true;
            this.Error.Location = new System.Drawing.Point(14, 53);
            this.Error.Name = "Error";
            this.Error.Size = new System.Drawing.Size(28, 13);
            this.Error.TabIndex = 1;
            this.Error.Text = "error";
            // 
            // FourthPage
            // 
            this.Controls.Add(this.Error);
            this.Name = "FourthPage";
            this.WizardBack += new Wizard.UI.WizardPageEventHandler(this.FourthPage_WizardBack);
            this.SetActive += new System.ComponentModel.CancelEventHandler(this.FourthPage_SetActive);
            this.Controls.SetChildIndex(this.Error, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Error;
    }
}

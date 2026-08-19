namespace DataMigration
{
    partial class DataMigrationWizardSheet
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataMigrationWizardSheet));
            this.SuspendLayout();
            // 
            // DataMigrationWizardSheet
            // 
            this.AcceptButton = this.nextButton;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(384, 141);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DataMigrationWizardSheet";
            this.Load += new System.EventHandler(this.DataMigrationWizardSheet_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DataMigrationWizardSheet_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion
    }
}

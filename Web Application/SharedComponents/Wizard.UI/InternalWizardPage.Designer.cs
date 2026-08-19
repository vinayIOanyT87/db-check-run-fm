namespace Wizard.UI
{
    partial class InternalWizardPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InternalWizardPage));
            this.wizardBanner1 = new Wizard.UI.WizardBanner();
            this.SuspendLayout();
            // 
            // wizardBanner1
            // 
            this.wizardBanner1.BackColor = System.Drawing.SystemColors.Window;
            this.wizardBanner1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("wizardBanner1.BackgroundImage")));
            this.wizardBanner1.Dock = System.Windows.Forms.DockStyle.Top;
            this.wizardBanner1.Location = new System.Drawing.Point(0, 0);
            this.wizardBanner1.Name = "wizardBanner1";
            this.wizardBanner1.Size = new System.Drawing.Size(432, 40);
            this.wizardBanner1.TabIndex = 0;
            // 
            // InternalWizardPage
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.AutoSize = false;
            this.Controls.Add(this.wizardBanner1);
            this.Name = "InternalWizardPage";
            this.Size = new System.Drawing.Size(432, 150);
            this.SetActive += new System.ComponentModel.CancelEventHandler(this.InternalWizardPage_SetActive);
            this.ResumeLayout(false);

        }

        #endregion

        private WizardBanner wizardBanner1;
    }
}

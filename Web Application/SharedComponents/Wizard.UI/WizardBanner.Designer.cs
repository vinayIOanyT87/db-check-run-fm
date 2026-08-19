namespace Wizard.UI
{
    partial class WizardBanner
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
            this.etchedLine1 = new Wizard.Controls.EtchedLine();
            this.SuspendLayout();
            // 
            // etchedLine1
            // 
            this.etchedLine1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.etchedLine1.Edge = Wizard.Controls.EtchEdge.Bottom;
            this.etchedLine1.Location = new System.Drawing.Point(0, 39);
            this.etchedLine1.Name = "etchedLine1";
            this.etchedLine1.Size = new System.Drawing.Size(456, 1);
            this.etchedLine1.TabIndex = 0;
            // 
            // WizardBanner
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.etchedLine1);
            this.Name = "WizardBanner";
            this.Size = new System.Drawing.Size(456, 40);
            this.ResumeLayout(false);

        }

        #endregion

        private Wizard.Controls.EtchedLine etchedLine1;
    }
}

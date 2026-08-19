namespace DataImportExportWizard
{
    partial class FirstPage
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
            this.BaseServer = new System.Windows.Forms.RadioButton();
            this.EnterpriseServer = new System.Windows.Forms.RadioButton();
            this.PromptLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // BaseServer
            // 
            this.BaseServer.AutoSize = true;
            this.BaseServer.Checked = true;
            this.BaseServer.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BaseServer.Location = new System.Drawing.Point(152, 91);
            this.BaseServer.Name = "BaseServer";
            this.BaseServer.Size = new System.Drawing.Size(94, 17);
            this.BaseServer.TabIndex = 0;
            this.BaseServer.TabStop = true;
            this.BaseServer.Text = "Base Server";
            this.BaseServer.UseVisualStyleBackColor = true;
            this.BaseServer.CheckedChanged += new System.EventHandler(this.BaseServer_CheckedChanged);
            // 
            // EnterpriseServer
            // 
            this.EnterpriseServer.AutoSize = true;
            this.EnterpriseServer.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EnterpriseServer.Location = new System.Drawing.Point(152, 114);
            this.EnterpriseServer.Name = "EnterpriseServer";
            this.EnterpriseServer.Size = new System.Drawing.Size(123, 17);
            this.EnterpriseServer.TabIndex = 1;
            this.EnterpriseServer.Text = "Enterprise Server";
            this.EnterpriseServer.UseVisualStyleBackColor = true;
            this.EnterpriseServer.CheckedChanged += new System.EventHandler(this.EnterpriseServer_CheckedChanged);
            // 
            // PromptLabel
            // 
            this.PromptLabel.AutoSize = true;
            this.PromptLabel.Location = new System.Drawing.Point(24, 58);
            this.PromptLabel.Name = "PromptLabel";
            this.PromptLabel.Size = new System.Drawing.Size(212, 13);
            this.PromptLabel.TabIndex = 3;
            this.PromptLabel.Text = "Server to perform Import/Export processing:";
            // 
            // FirstPage
            // 
            this.Controls.Add(this.PromptLabel);
            this.Controls.Add(this.BaseServer);
            this.Controls.Add(this.EnterpriseServer);
            this.Name = "FirstPage";
            this.Size = new System.Drawing.Size(450, 254);
            this.SetActive += new System.ComponentModel.CancelEventHandler(this.FirstPage_SetActive);
            this.WizardNext += new Wizard.UI.WizardPageEventHandler(this.FirstPage_WizardNext);
            this.Controls.SetChildIndex(this.EnterpriseServer, 0);
            this.Controls.SetChildIndex(this.BaseServer, 0);
            this.Controls.SetChildIndex(this.PromptLabel, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton BaseServer;
        private System.Windows.Forms.RadioButton EnterpriseServer;
        private System.Windows.Forms.Label PromptLabel;

    }
}

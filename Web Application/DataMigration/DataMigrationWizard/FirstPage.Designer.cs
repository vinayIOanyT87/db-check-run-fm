namespace DataMigration
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
            this.BaseBase = new System.Windows.Forms.RadioButton();
            this.BaseEnterprise = new System.Windows.Forms.RadioButton();
            this.EnterpriseEnterprise = new System.Windows.Forms.RadioButton();
            this.label = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // BaseBase
            // 
            this.BaseBase.AutoSize = true;
            this.BaseBase.Checked = true;
            this.BaseBase.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BaseBase.Location = new System.Drawing.Point(113, 101);
            this.BaseBase.Name = "BaseBase";
            this.BaseBase.Size = new System.Drawing.Size(156, 17);
            this.BaseBase.TabIndex = 0;
            this.BaseBase.TabStop = true;
            this.BaseBase.Text = "Base to Base Migration";
            this.BaseBase.UseVisualStyleBackColor = true;
            this.BaseBase.CheckedChanged += new System.EventHandler(this.BaseBase_CheckedChanged);
            // 
            // BaseEnterprise
            // 
            this.BaseEnterprise.AutoSize = true;
            this.BaseEnterprise.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BaseEnterprise.Location = new System.Drawing.Point(113, 148);
            this.BaseEnterprise.Name = "BaseEnterprise";
            this.BaseEnterprise.Size = new System.Drawing.Size(185, 17);
            this.BaseEnterprise.TabIndex = 1;
            this.BaseEnterprise.Text = "Base to Enterprise Migration";
            this.BaseEnterprise.UseVisualStyleBackColor = true;
            this.BaseEnterprise.CheckedChanged += new System.EventHandler(this.BaseEnterprise_CheckedChanged);
            // 
            // EnterpriseEnterprise
            // 
            this.EnterpriseEnterprise.AutoSize = true;
            this.EnterpriseEnterprise.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EnterpriseEnterprise.Location = new System.Drawing.Point(113, 195);
            this.EnterpriseEnterprise.Name = "EnterpriseEnterprise";
            this.EnterpriseEnterprise.Size = new System.Drawing.Size(214, 17);
            this.EnterpriseEnterprise.TabIndex = 2;
            this.EnterpriseEnterprise.Text = "Enterprise to Enterprise Migration";
            this.EnterpriseEnterprise.UseVisualStyleBackColor = true;
            this.EnterpriseEnterprise.CheckedChanged += new System.EventHandler(this.EnterpriseEnterprise_CheckedChanged);
            // 
            // label
            // 
            this.label.AutoSize = true;
            this.label.Location = new System.Drawing.Point(65, 63);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(138, 13);
            this.label.TabIndex = 3;
            this.label.Text = "Select the type of migration:";
            // 
            // FirstPage
            // 
            this.Controls.Add(this.label);
            this.Controls.Add(this.BaseBase);
            this.Controls.Add(this.EnterpriseEnterprise);
            this.Controls.Add(this.BaseEnterprise);
            this.Name = "FirstPage";
            this.Size = new System.Drawing.Size(440, 250);
            this.WizardNext += new Wizard.UI.WizardPageEventHandler(this.FirstPage_WizardNext);
            this.SetActive += new System.ComponentModel.CancelEventHandler(this.FirstPage_SetActive);
            this.Controls.SetChildIndex(this.BaseEnterprise, 0);
            this.Controls.SetChildIndex(this.EnterpriseEnterprise, 0);
            this.Controls.SetChildIndex(this.BaseBase, 0);
            this.Controls.SetChildIndex(this.label, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton BaseBase;
        private System.Windows.Forms.RadioButton BaseEnterprise;
        private System.Windows.Forms.RadioButton EnterpriseEnterprise;
        private System.Windows.Forms.Label label;

    }
}

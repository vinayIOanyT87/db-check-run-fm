namespace DataMigration
{
    partial class ThirdPage
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
            this.SitesList = new System.Windows.Forms.ComboBox();
            this.ShowSitesBtn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // SitesList
            // 
            this.SitesList.AllowDrop = true;
            this.SitesList.DropDownHeight = 206;
            this.SitesList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SitesList.FormattingEnabled = true;
            this.SitesList.IntegralHeight = false;
            this.SitesList.Location = new System.Drawing.Point(62, 106);
            this.SitesList.Name = "SitesList";
            this.SitesList.Size = new System.Drawing.Size(191, 21);
            this.SitesList.TabIndex = 2;
            // 
            // ShowSitesBtn
            // 
            this.ShowSitesBtn.Location = new System.Drawing.Point(270, 106);
            this.ShowSitesBtn.Name = "ShowSitesBtn";
            this.ShowSitesBtn.Size = new System.Drawing.Size(108, 23);
            this.ShowSitesBtn.TabIndex = 3;
            this.ShowSitesBtn.Text = "Show Existing Sites";
            this.ShowSitesBtn.UseVisualStyleBackColor = true;
            this.ShowSitesBtn.Click += new System.EventHandler(this.ShowSitesBtn_Click);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(62, 66);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(316, 23);
            this.label1.TabIndex = 4;
            this.label1.Text = "Enterprise to Enterprise Migration";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ThirdPage
            // 
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ShowSitesBtn);
            this.Controls.Add(this.SitesList);
            this.Name = "ThirdPage";
            this.Size = new System.Drawing.Size(440, 250);
            this.Load += new System.EventHandler(this.ThirdPage_Load);
            this.WizardNext += new Wizard.UI.WizardPageEventHandler(this.ThirdPage_WizardNext);
            this.Controls.SetChildIndex(this.SitesList, 0);
            this.Controls.SetChildIndex(this.ShowSitesBtn, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox SitesList;
        private System.Windows.Forms.Button ShowSitesBtn;
        private System.Windows.Forms.Label label1;
    }
}

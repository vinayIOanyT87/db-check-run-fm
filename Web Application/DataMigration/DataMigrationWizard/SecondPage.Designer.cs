namespace DataMigration
{
    partial class SecondPage
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
            this.BaseFileName = new System.Windows.Forms.TextBox();
            this.BaseBrowseBtn = new System.Windows.Forms.Button();
            this.EnterpriseBrowseBtn = new System.Windows.Forms.Button();
            this.EnterpriseFileName = new System.Windows.Forms.TextBox();
            this.FirstLbl = new System.Windows.Forms.Label();
            this.Secendlbl = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.Thirdlbl = new System.Windows.Forms.Label();
            this.AviationDBFilename = new System.Windows.Forms.TextBox();
            this.AviationDBBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // BaseFileName
            // 
            this.BaseFileName.Location = new System.Drawing.Point(106, 135);
            this.BaseFileName.Name = "BaseFileName";
            this.BaseFileName.Size = new System.Drawing.Size(203, 20);
            this.BaseFileName.TabIndex = 1;
            // 
            // BaseBrowseBtn
            // 
            this.BaseBrowseBtn.Location = new System.Drawing.Point(308, 134);
            this.BaseBrowseBtn.Name = "BaseBrowseBtn";
            this.BaseBrowseBtn.Size = new System.Drawing.Size(28, 23);
            this.BaseBrowseBtn.TabIndex = 2;
            this.BaseBrowseBtn.Text = "...";
            this.BaseBrowseBtn.UseVisualStyleBackColor = true;
            this.BaseBrowseBtn.Click += new System.EventHandler(this.BaseBrowseBtn_Click);
            // 
            // EnterpriseBrowseBtn
            // 
            this.EnterpriseBrowseBtn.Location = new System.Drawing.Point(308, 179);
            this.EnterpriseBrowseBtn.Name = "EnterpriseBrowseBtn";
            this.EnterpriseBrowseBtn.Size = new System.Drawing.Size(28, 23);
            this.EnterpriseBrowseBtn.TabIndex = 3;
            this.EnterpriseBrowseBtn.Text = "...";
            this.EnterpriseBrowseBtn.UseVisualStyleBackColor = true;
            this.EnterpriseBrowseBtn.Click += new System.EventHandler(this.EnterpriseBrowseBtn_Click);
            // 
            // EnterpriseFileName
            // 
            this.EnterpriseFileName.Location = new System.Drawing.Point(106, 180);
            this.EnterpriseFileName.Name = "EnterpriseFileName";
            this.EnterpriseFileName.Size = new System.Drawing.Size(203, 20);
            this.EnterpriseFileName.TabIndex = 4;
            // 
            // FirstLbl
            // 
            this.FirstLbl.AutoSize = true;
            this.FirstLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FirstLbl.Location = new System.Drawing.Point(24, 111);
            this.FirstLbl.Name = "FirstLbl";
            this.FirstLbl.Size = new System.Drawing.Size(41, 13);
            this.FirstLbl.TabIndex = 5;
            this.FirstLbl.Text = "label1";
            // 
            // Secendlbl
            // 
            this.Secendlbl.AutoSize = true;
            this.Secendlbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Secendlbl.Location = new System.Drawing.Point(24, 157);
            this.Secendlbl.Name = "Secendlbl";
            this.Secendlbl.Size = new System.Drawing.Size(41, 13);
            this.Secendlbl.TabIndex = 6;
            this.Secendlbl.Text = "label1";
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(24, 58);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(312, 23);
            this.label1.TabIndex = 7;
            this.label1.Text = "Please select database backup file.";
            // 
            // Thirdlbl
            // 
            this.Thirdlbl.AutoSize = true;
            this.Thirdlbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Thirdlbl.Location = new System.Drawing.Point(24, 203);
            this.Thirdlbl.Name = "Thirdlbl";
            this.Thirdlbl.Size = new System.Drawing.Size(41, 13);
            this.Thirdlbl.TabIndex = 10;
            this.Thirdlbl.Text = "label1";
            // 
            // AviationDBFilename
            // 
            this.AviationDBFilename.Location = new System.Drawing.Point(106, 225);
            this.AviationDBFilename.Name = "AviationDBFilename";
            this.AviationDBFilename.Size = new System.Drawing.Size(203, 20);
            this.AviationDBFilename.TabIndex = 9;
            // 
            // AviationDBBtn
            // 
            this.AviationDBBtn.Location = new System.Drawing.Point(308, 224);
            this.AviationDBBtn.Name = "AviationDBBtn";
            this.AviationDBBtn.Size = new System.Drawing.Size(28, 23);
            this.AviationDBBtn.TabIndex = 8;
            this.AviationDBBtn.Text = "...";
            this.AviationDBBtn.UseVisualStyleBackColor = true;
            this.AviationDBBtn.Click += new System.EventHandler(this.AviationDBBtn_Click);
            // 
            // SecondPage
            // 
            this.Controls.Add(this.Thirdlbl);
            this.Controls.Add(this.AviationDBFilename);
            this.Controls.Add(this.AviationDBBtn);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Secendlbl);
            this.Controls.Add(this.FirstLbl);
            this.Controls.Add(this.EnterpriseFileName);
            this.Controls.Add(this.EnterpriseBrowseBtn);
            this.Controls.Add(this.BaseFileName);
            this.Controls.Add(this.BaseBrowseBtn);
            this.Name = "SecondPage";
            this.Size = new System.Drawing.Size(440, 250);
            this.WizardNext += new Wizard.UI.WizardPageEventHandler(this.SecondPage_WizardNext);
            this.SetActive += new System.ComponentModel.CancelEventHandler(this.SecondPage_SetActive);
            this.Controls.SetChildIndex(this.BaseBrowseBtn, 0);
            this.Controls.SetChildIndex(this.BaseFileName, 0);
            this.Controls.SetChildIndex(this.EnterpriseBrowseBtn, 0);
            this.Controls.SetChildIndex(this.EnterpriseFileName, 0);
            this.Controls.SetChildIndex(this.FirstLbl, 0);
            this.Controls.SetChildIndex(this.Secendlbl, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.AviationDBBtn, 0);
            this.Controls.SetChildIndex(this.AviationDBFilename, 0);
            this.Controls.SetChildIndex(this.Thirdlbl, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox BaseFileName;
        private System.Windows.Forms.Button BaseBrowseBtn;
        private System.Windows.Forms.Button EnterpriseBrowseBtn;
        private System.Windows.Forms.TextBox EnterpriseFileName;
        private System.Windows.Forms.Label FirstLbl;
        private System.Windows.Forms.Label Secendlbl;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label Thirdlbl;
        private System.Windows.Forms.TextBox AviationDBFilename;
        private System.Windows.Forms.Button AviationDBBtn;

    }
}

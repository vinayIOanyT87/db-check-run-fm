namespace DataMigration
{
    partial class WelcomePage
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
            this.promptLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Sidebar
            // 
            this.Sidebar.Size = new System.Drawing.Size(165, 250);
            // 
            // promptLabel
            // 
            this.promptLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.promptLabel.Location = new System.Drawing.Point(220, 234);
            this.promptLabel.Name = "promptLabel";
            this.promptLabel.Size = new System.Drawing.Size(173, 16);
            this.promptLabel.TabIndex = 3;
            this.promptLabel.Text = "Press Next to continue.";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(220, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(187, 19);
            this.label1.TabIndex = 4;
            this.label1.Text = "Welcome to the Data Migration";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(220, 147);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(181, 52);
            this.label2.TabIndex = 5;
            this.label2.Text = "This wizard will guide you through the importing of your existing FuelsManager 6." +
                "0 data to FuelsManager 8.0";
            // 
            // WelcomePage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.promptLabel);
            this.Name = "WelcomePage";
            this.SetActive += new System.ComponentModel.CancelEventHandler(this.WelcomePage_SetActive);
            this.Controls.SetChildIndex(this.promptLabel, 0);
            this.Controls.SetChildIndex(this.label2, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.Sidebar, 0);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label promptLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}

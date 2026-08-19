namespace ExStarsUI
{
	partial class frmExStars
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
			this.menuExStarsMain = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.configurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.reportsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.stdMonthlyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.viewStatusToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.downLoadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.eDIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.easyReadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.acknowledgmentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.browserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.menuExStarsMain.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuExStarsMain
			// 
			this.menuExStarsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.reportsToolStripMenuItem,
            this.downLoadToolStripMenuItem});
			this.menuExStarsMain.Location = new System.Drawing.Point(0, 0);
			this.menuExStarsMain.Name = "menuExStarsMain";
			this.menuExStarsMain.Size = new System.Drawing.Size(634, 28);
			this.menuExStarsMain.TabIndex = 0;
			this.menuExStarsMain.Text = "menuExStarsMain";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem,
            this.configurationToolStripMenuItem,
            this.browserToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(44, 24);
			this.fileToolStripMenuItem.Text = "File";
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(169, 24);
			this.exitToolStripMenuItem.Text = "Exit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
			// 
			// configurationToolStripMenuItem
			// 
			this.configurationToolStripMenuItem.Name = "configurationToolStripMenuItem";
			this.configurationToolStripMenuItem.Size = new System.Drawing.Size(169, 24);
			this.configurationToolStripMenuItem.Text = "Configuration";
			this.configurationToolStripMenuItem.Click += new System.EventHandler(this.configurationToolStripMenuItem_Click);
			// 
			// reportsToolStripMenuItem
			// 
			this.reportsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.stdMonthlyToolStripMenuItem,
            this.viewStatusToolStripMenuItem});
			this.reportsToolStripMenuItem.Name = "reportsToolStripMenuItem";
			this.reportsToolStripMenuItem.Size = new System.Drawing.Size(72, 24);
			this.reportsToolStripMenuItem.Text = "Reports";
			// 
			// stdMonthlyToolStripMenuItem
			// 
			this.stdMonthlyToolStripMenuItem.Name = "stdMonthlyToolStripMenuItem";
			this.stdMonthlyToolStripMenuItem.Size = new System.Drawing.Size(158, 24);
			this.stdMonthlyToolStripMenuItem.Text = "Std Monthly";
			this.stdMonthlyToolStripMenuItem.Click += new System.EventHandler(this.stdMonthlyToolStripMenuItem_Click);
			// 
			// viewStatusToolStripMenuItem
			// 
			this.viewStatusToolStripMenuItem.Name = "viewStatusToolStripMenuItem";
			this.viewStatusToolStripMenuItem.Size = new System.Drawing.Size(158, 24);
			this.viewStatusToolStripMenuItem.Text = "View Status";
			// 
			// downLoadToolStripMenuItem
			// 
			this.downLoadToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.eDIToolStripMenuItem,
            this.easyReadToolStripMenuItem,
            this.acknowledgmentToolStripMenuItem});
			this.downLoadToolStripMenuItem.Name = "downLoadToolStripMenuItem";
			this.downLoadToolStripMenuItem.Size = new System.Drawing.Size(93, 24);
			this.downLoadToolStripMenuItem.Text = "DownLoad";
			// 
			// eDIToolStripMenuItem
			// 
			this.eDIToolStripMenuItem.Name = "eDIToolStripMenuItem";
			this.eDIToolStripMenuItem.Size = new System.Drawing.Size(222, 24);
			this.eDIToolStripMenuItem.Text = "EDI";
			// 
			// easyReadToolStripMenuItem
			// 
			this.easyReadToolStripMenuItem.Name = "easyReadToolStripMenuItem";
			this.easyReadToolStripMenuItem.Size = new System.Drawing.Size(222, 24);
			this.easyReadToolStripMenuItem.Text = "Easy Read";
			// 
			// acknowledgmentToolStripMenuItem
			// 
			this.acknowledgmentToolStripMenuItem.Name = "acknowledgmentToolStripMenuItem";
			this.acknowledgmentToolStripMenuItem.Size = new System.Drawing.Size(222, 24);
			this.acknowledgmentToolStripMenuItem.Text = "151 Acknowledgment";
			// 
			// browserToolStripMenuItem
			// 
			this.browserToolStripMenuItem.Name = "browserToolStripMenuItem";
			this.browserToolStripMenuItem.Size = new System.Drawing.Size(169, 24);
			this.browserToolStripMenuItem.Text = "Browser";
			this.browserToolStripMenuItem.Click += new System.EventHandler(this.browserToolStripMenuItem_Click);
			// 
			// frmExStars
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(634, 309);
			this.Controls.Add(this.menuExStarsMain);
			this.MainMenuStrip = this.menuExStarsMain;
			this.Name = "frmExStars";
			this.Text = "ExSTARS - IRS Reporting";
			this.menuExStarsMain.ResumeLayout(false);
			this.menuExStarsMain.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip menuExStarsMain;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem reportsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem stdMonthlyToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem viewStatusToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem downLoadToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem eDIToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem easyReadToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem acknowledgmentToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem configurationToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem browserToolStripMenuItem;
	}
}


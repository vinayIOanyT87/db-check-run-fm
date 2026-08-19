namespace Dispatch
{
	partial class AddInsForm
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
			this.Closebutton = new System.Windows.Forms.Button();
			this.AssignedAddInslistView = new System.Windows.Forms.ListView();
			this.MenuNametextBox = new System.Windows.Forms.TextBox();
			this.ApplicationtextBox = new System.Windows.Forms.TextBox();
			this.Browsebutton = new System.Windows.Forms.Button();
			this.Addbutton = new System.Windows.Forms.Button();
			this.Modifybutton = new System.Windows.Forms.Button();
			this.Deletebutton = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.OKbutton = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.SuspendLayout();
			// 
			// Closebutton
			// 
			this.Closebutton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.Closebutton.Location = new System.Drawing.Point(301,295);
			this.Closebutton.Name = "Closebutton";
			this.Closebutton.Size = new System.Drawing.Size(75,23);
			this.Closebutton.TabIndex = 0;
			this.Closebutton.Text = "&Cancel";
			this.Closebutton.UseVisualStyleBackColor = true;
			this.Closebutton.Click += new System.EventHandler(this.Closebutton_Click);
			// 
			// AssignedAddInslistView
			// 
			this.AssignedAddInslistView.FullRowSelect = true;
			this.AssignedAddInslistView.GridLines = true;
			this.AssignedAddInslistView.HideSelection = false;
			this.AssignedAddInslistView.Location = new System.Drawing.Point(30,28);
			this.AssignedAddInslistView.MultiSelect = false;
			this.AssignedAddInslistView.Name = "AssignedAddInslistView";
			this.AssignedAddInslistView.Size = new System.Drawing.Size(485,97);
			this.AssignedAddInslistView.TabIndex = 1;
			this.AssignedAddInslistView.UseCompatibleStateImageBehavior = false;
			this.AssignedAddInslistView.View = System.Windows.Forms.View.List;
			this.AssignedAddInslistView.SelectedIndexChanged += new System.EventHandler(this.OnListViewSelectedIndexChanged);
			// 
			// MenuNametextBox
			// 
			this.MenuNametextBox.Location = new System.Drawing.Point(14,214);
			this.MenuNametextBox.Name = "MenuNametextBox";
			this.MenuNametextBox.Size = new System.Drawing.Size(164,20);
			this.MenuNametextBox.TabIndex = 2;
			this.MenuNametextBox.TextChanged += new System.EventHandler(this.OnTextChanged);
			// 
			// ApplicationtextBox
			// 
			this.ApplicationtextBox.Location = new System.Drawing.Point(14,259);
			this.ApplicationtextBox.Name = "ApplicationtextBox";
			this.ApplicationtextBox.Size = new System.Drawing.Size(336,20);
			this.ApplicationtextBox.TabIndex = 3;
			this.ApplicationtextBox.TextChanged += new System.EventHandler(this.OnTextChanged);
			// 
			// Browsebutton
			// 
			this.Browsebutton.Location = new System.Drawing.Point(367,257);
			this.Browsebutton.Name = "Browsebutton";
			this.Browsebutton.Size = new System.Drawing.Size(75,23);
			this.Browsebutton.TabIndex = 4;
			this.Browsebutton.Text = "&Browse";
			this.Browsebutton.UseVisualStyleBackColor = true;
			this.Browsebutton.Click += new System.EventHandler(this.Browsebutton_Click);
			// 
			// Addbutton
			// 
			this.Addbutton.Location = new System.Drawing.Point(103,140);
			this.Addbutton.Name = "Addbutton";
			this.Addbutton.Size = new System.Drawing.Size(75,23);
			this.Addbutton.TabIndex = 5;
			this.Addbutton.Text = "&Add";
			this.Addbutton.UseVisualStyleBackColor = true;
			this.Addbutton.Click += new System.EventHandler(this.Addbutton_Click);
			// 
			// Modifybutton
			// 
			this.Modifybutton.Location = new System.Drawing.Point(235,140);
			this.Modifybutton.Name = "Modifybutton";
			this.Modifybutton.Size = new System.Drawing.Size(75,23);
			this.Modifybutton.TabIndex = 6;
			this.Modifybutton.Text = "&Modify";
			this.Modifybutton.UseVisualStyleBackColor = true;
			this.Modifybutton.Click += new System.EventHandler(this.Modifybutton_Click);
			// 
			// Deletebutton
			// 
			this.Deletebutton.Location = new System.Drawing.Point(367,140);
			this.Deletebutton.Name = "Deletebutton";
			this.Deletebutton.Size = new System.Drawing.Size(75,23);
			this.Deletebutton.TabIndex = 7;
			this.Deletebutton.Text = "&Delete";
			this.Deletebutton.UseVisualStyleBackColor = true;
			this.Deletebutton.Click += new System.EventHandler(this.Deletebutton_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(14,195);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(57,13);
			this.label1.TabIndex = 8;
			this.label1.Text = "Menu Item";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(14,243);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(59,13);
			this.label2.TabIndex = 9;
			this.label2.Text = "Application";
			// 
			// OKbutton
			// 
			this.OKbutton.Location = new System.Drawing.Point(169,295);
			this.OKbutton.Name = "OKbutton";
			this.OKbutton.Size = new System.Drawing.Size(75,23);
			this.OKbutton.TabIndex = 10;
			this.OKbutton.Text = "&OK";
			this.OKbutton.UseVisualStyleBackColor = true;
			this.OKbutton.Click += new System.EventHandler(this.OKbutton_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Location = new System.Drawing.Point(14,2);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(516,176);
			this.groupBox1.TabIndex = 11;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Add-Ins";
			// 
			// AddInsForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F,13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.Closebutton;
			this.ClientSize = new System.Drawing.Size(544,334);
			this.ControlBox = false;
			this.Controls.Add(this.OKbutton);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.Deletebutton);
			this.Controls.Add(this.Modifybutton);
			this.Controls.Add(this.Addbutton);
			this.Controls.Add(this.Browsebutton);
			this.Controls.Add(this.ApplicationtextBox);
			this.Controls.Add(this.MenuNametextBox);
			this.Controls.Add(this.AssignedAddInslistView);
			this.Controls.Add(this.Closebutton);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AddInsForm";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Add-Ins Configuration";
			this.Load += new System.EventHandler(this.OnLoadDialog);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button Closebutton;
		private System.Windows.Forms.ListView AssignedAddInslistView;
		private System.Windows.Forms.TextBox MenuNametextBox;
		private System.Windows.Forms.TextBox ApplicationtextBox;
		private System.Windows.Forms.Button Browsebutton;
		private System.Windows.Forms.Button Addbutton;
		private System.Windows.Forms.Button Modifybutton;
		private System.Windows.Forms.Button Deletebutton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button OKbutton;
        private System.Windows.Forms.GroupBox groupBox1;
	}
}
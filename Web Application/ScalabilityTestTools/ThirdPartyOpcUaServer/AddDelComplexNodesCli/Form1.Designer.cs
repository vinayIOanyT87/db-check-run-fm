namespace AddDelComplexNodesCli
{
    partial class Form1
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nodeXMLTextBox = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.addButton = new System.Windows.Forms.Button();
            this.addNodeIdTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.nodeNameTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.delNodeIDTextBox = new System.Windows.Forms.TextBox();
            this.deleteButton = new System.Windows.Forms.Button();
            this.openNodeDefFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.dynamicEntityTypeListView = new System.Windows.Forms.ListView();
            this.label5 = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(926, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // nodeXMLTextBox
            // 
            this.nodeXMLTextBox.Location = new System.Drawing.Point(12, 256);
            this.nodeXMLTextBox.Name = "nodeXMLTextBox";
            this.nodeXMLTextBox.Size = new System.Drawing.Size(911, 120);
            this.nodeXMLTextBox.TabIndex = 1;
            this.nodeXMLTextBox.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 235);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Node Object XML";
            // 
            // addButton
            // 
            this.addButton.Location = new System.Drawing.Point(808, 391);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(115, 31);
            this.addButton.TabIndex = 3;
            this.addButton.Text = "ADD";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // addNodeIdTextBox
            // 
            this.addNodeIdTextBox.Location = new System.Drawing.Point(12, 41);
            this.addNodeIdTextBox.Name = "addNodeIdTextBox";
            this.addNodeIdTextBox.Size = new System.Drawing.Size(146, 20);
            this.addNodeIdTextBox.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Node ID";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 68);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Node Name";
            // 
            // nodeNameTextBox
            // 
            this.nodeNameTextBox.Location = new System.Drawing.Point(12, 84);
            this.nodeNameTextBox.Name = "nodeNameTextBox";
            this.nodeNameTextBox.Size = new System.Drawing.Size(911, 20);
            this.nodeNameTextBox.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 422);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Node ID";
            // 
            // delNodeIDTextBox
            // 
            this.delNodeIDTextBox.Location = new System.Drawing.Point(12, 438);
            this.delNodeIDTextBox.Name = "delNodeIDTextBox";
            this.delNodeIDTextBox.Size = new System.Drawing.Size(146, 20);
            this.delNodeIDTextBox.TabIndex = 8;
            // 
            // deleteButton
            // 
            this.deleteButton.Location = new System.Drawing.Point(217, 438);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(115, 31);
            this.deleteButton.TabIndex = 10;
            this.deleteButton.Text = "DELETE";
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // openNodeDefFileDialog
            // 
            this.openNodeDefFileDialog.FileName = "openFileDialog1";
            // 
            // dynamicEntityTypeListView
            // 
            this.dynamicEntityTypeListView.AllowDrop = true;
            this.dynamicEntityTypeListView.AutoArrange = false;
            this.dynamicEntityTypeListView.HideSelection = false;
            this.dynamicEntityTypeListView.LabelWrap = false;
            this.dynamicEntityTypeListView.Location = new System.Drawing.Point(12, 131);
            this.dynamicEntityTypeListView.MultiSelect = false;
            this.dynamicEntityTypeListView.Name = "dynamicEntityTypeListView";
            this.dynamicEntityTypeListView.RightToLeftLayout = true;
            this.dynamicEntityTypeListView.Size = new System.Drawing.Size(414, 101);
            this.dynamicEntityTypeListView.TabIndex = 11;
            this.dynamicEntityTypeListView.UseCompatibleStateImageBehavior = false;
            this.dynamicEntityTypeListView.View = System.Windows.Forms.View.List;
            this.dynamicEntityTypeListView.SelectedIndexChanged += new System.EventHandler(this.dynamicEntityTypeListView_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 115);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(104, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Dynamic Entity Type";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(926, 477);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.dynamicEntityTypeListView);
            this.Controls.Add(this.deleteButton);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.delNodeIDTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.nodeNameTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.addNodeIdTextBox);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.nodeXMLTextBox);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "AddDelComplexNodesCli";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.RichTextBox nodeXMLTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.TextBox addNodeIdTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox nodeNameTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox delNodeIDTextBox;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.OpenFileDialog openNodeDefFileDialog;
        private System.Windows.Forms.ListView dynamicEntityTypeListView;
        private System.Windows.Forms.Label label5;
    }
}


namespace GenerateRightScripts
{
    partial class GenerateRightScriptsForm
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
            this.InputFileNameLabel = new System.Windows.Forms.Label();
            this.OutputFileNameLabel = new System.Windows.Forms.Label();
            this.OutFileNameTB = new System.Windows.Forms.TextBox();
            this.InputFileNameTB = new System.Windows.Forms.TextBox();
            this.InputFileNameBrowseBtn = new System.Windows.Forms.Button();
            this.OutputFileNameBrowseBtn = new System.Windows.Forms.Button();
            this.ResultLabel = new System.Windows.Forms.Label();
            this.ResultTB = new System.Windows.Forms.TextBox();
            this.RunBtn = new System.Windows.Forms.Button();
            this.ClearBtn = new System.Windows.Forms.Button();
            this.CloseBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // InputFileNameLabel
            // 
            this.InputFileNameLabel.AutoSize = true;
            this.InputFileNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.InputFileNameLabel.Location = new System.Drawing.Point(13, 13);
            this.InputFileNameLabel.Name = "InputFileNameLabel";
            this.InputFileNameLabel.Size = new System.Drawing.Size(130, 18);
            this.InputFileNameLabel.TabIndex = 0;
            this.InputFileNameLabel.Text = "Input File Name:";
            // 
            // OutputFileNameLabel
            // 
            this.OutputFileNameLabel.AutoSize = true;
            this.OutputFileNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OutputFileNameLabel.Location = new System.Drawing.Point(13, 52);
            this.OutputFileNameLabel.Name = "OutputFileNameLabel";
            this.OutputFileNameLabel.Size = new System.Drawing.Size(144, 18);
            this.OutputFileNameLabel.TabIndex = 1;
            this.OutputFileNameLabel.Text = "Output File Name:";
            // 
            // OutFileNameTB
            // 
            this.OutFileNameTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OutFileNameTB.Location = new System.Drawing.Point(164, 52);
            this.OutFileNameTB.Name = "OutFileNameTB";
            this.OutFileNameTB.Size = new System.Drawing.Size(434, 26);
            this.OutFileNameTB.TabIndex = 2;
            this.OutFileNameTB.TextChanged += new System.EventHandler(this.OutFileNameTbChange);
            // 
            // InputFileNameTB
            // 
            this.InputFileNameTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.InputFileNameTB.Location = new System.Drawing.Point(164, 13);
            this.InputFileNameTB.Name = "InputFileNameTB";
            this.InputFileNameTB.Size = new System.Drawing.Size(434, 26);
            this.InputFileNameTB.TabIndex = 3;
            this.InputFileNameTB.TextChanged += new System.EventHandler(this.InputFileNameTbChange);
            // 
            // InputFileNameBrowseBtn
            // 
            this.InputFileNameBrowseBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.InputFileNameBrowseBtn.Location = new System.Drawing.Point(633, 12);
            this.InputFileNameBrowseBtn.Name = "InputFileNameBrowseBtn";
            this.InputFileNameBrowseBtn.Size = new System.Drawing.Size(75, 27);
            this.InputFileNameBrowseBtn.TabIndex = 4;
            this.InputFileNameBrowseBtn.Text = "Browse";
            this.InputFileNameBrowseBtn.UseVisualStyleBackColor = true;
            this.InputFileNameBrowseBtn.Click += new System.EventHandler(this.InputFileNameBrowseBtnClick);
            // 
            // OutputFileNameBrowseBtn
            // 
            this.OutputFileNameBrowseBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OutputFileNameBrowseBtn.Location = new System.Drawing.Point(633, 51);
            this.OutputFileNameBrowseBtn.Name = "OutputFileNameBrowseBtn";
            this.OutputFileNameBrowseBtn.Size = new System.Drawing.Size(75, 27);
            this.OutputFileNameBrowseBtn.TabIndex = 5;
            this.OutputFileNameBrowseBtn.Text = "Browse";
            this.OutputFileNameBrowseBtn.UseVisualStyleBackColor = true;
            this.OutputFileNameBrowseBtn.Click += new System.EventHandler(this.OutFileNameBrowseBtnClick);
            // 
            // ResultLabel
            // 
            this.ResultLabel.AutoSize = true;
            this.ResultLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ResultLabel.Location = new System.Drawing.Point(13, 116);
            this.ResultLabel.Name = "ResultLabel";
            this.ResultLabel.Size = new System.Drawing.Size(61, 18);
            this.ResultLabel.TabIndex = 6;
            this.ResultLabel.Text = "Result:";
            // 
            // ResultTB
            // 
            this.ResultTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ResultTB.Location = new System.Drawing.Point(16, 138);
            this.ResultTB.Multiline = true;
            this.ResultTB.Name = "ResultTB";
            this.ResultTB.ReadOnly = true;
            this.ResultTB.Size = new System.Drawing.Size(582, 136);
            this.ResultTB.TabIndex = 7;
            // 
            // RunBtn
            // 
            this.RunBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RunBtn.Location = new System.Drawing.Point(633, 149);
            this.RunBtn.Name = "RunBtn";
            this.RunBtn.Size = new System.Drawing.Size(75, 27);
            this.RunBtn.TabIndex = 8;
            this.RunBtn.Text = "Run";
            this.RunBtn.UseVisualStyleBackColor = true;
            this.RunBtn.Click += new System.EventHandler(this.RunBtnClick);
            // 
            // ClearBtn
            // 
            this.ClearBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ClearBtn.Location = new System.Drawing.Point(633, 197);
            this.ClearBtn.Name = "ClearBtn";
            this.ClearBtn.Size = new System.Drawing.Size(75, 27);
            this.ClearBtn.TabIndex = 9;
            this.ClearBtn.Text = "Clear";
            this.ClearBtn.UseVisualStyleBackColor = true;
            this.ClearBtn.Click += new System.EventHandler(this.ClearBtnClick);
            // 
            // CloseBtn
            // 
            this.CloseBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CloseBtn.Location = new System.Drawing.Point(633, 247);
            this.CloseBtn.Name = "CloseBtn";
            this.CloseBtn.Size = new System.Drawing.Size(75, 27);
            this.CloseBtn.TabIndex = 10;
            this.CloseBtn.Text = "Close";
            this.CloseBtn.UseVisualStyleBackColor = true;
            this.CloseBtn.Click += new System.EventHandler(this.CloseBtnClick);
            // 
            // GenerateRightScriptsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(772, 354);
            this.Controls.Add(this.CloseBtn);
            this.Controls.Add(this.ClearBtn);
            this.Controls.Add(this.RunBtn);
            this.Controls.Add(this.ResultTB);
            this.Controls.Add(this.ResultLabel);
            this.Controls.Add(this.OutputFileNameBrowseBtn);
            this.Controls.Add(this.InputFileNameBrowseBtn);
            this.Controls.Add(this.InputFileNameTB);
            this.Controls.Add(this.OutFileNameTB);
            this.Controls.Add(this.OutputFileNameLabel);
            this.Controls.Add(this.InputFileNameLabel);
            this.Name = "GenerateRightScriptsForm";
            this.Text = "Generate Right Scripts";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label InputFileNameLabel;
        private System.Windows.Forms.Label OutputFileNameLabel;
        private System.Windows.Forms.TextBox OutFileNameTB;
        private System.Windows.Forms.TextBox InputFileNameTB;
        private System.Windows.Forms.Button InputFileNameBrowseBtn;
        private System.Windows.Forms.Button OutputFileNameBrowseBtn;
        private System.Windows.Forms.Label ResultLabel;
        private System.Windows.Forms.TextBox ResultTB;
        private System.Windows.Forms.Button RunBtn;
        private System.Windows.Forms.Button ClearBtn;
        private System.Windows.Forms.Button CloseBtn;
    }
}


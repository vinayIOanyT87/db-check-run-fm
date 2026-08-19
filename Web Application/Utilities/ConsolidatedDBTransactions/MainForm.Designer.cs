namespace ConsolidatedDBTransactions
{
    partial class MainForm
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
            this.btnWriteTransaction = new System.Windows.Forms.Button();
            this.lblWriteTransactions = new System.Windows.Forms.Label();
            this.lblDataSource = new System.Windows.Forms.Label();
            this.txtBoxDataSource = new System.Windows.Forms.TextBox();
            this.lblInitialCatalog = new System.Windows.Forms.Label();
            this.txtBoxInitialCatalog = new System.Windows.Forms.TextBox();
            this.grpBoxDataBaseConnectionSettings = new System.Windows.Forms.GroupBox();
            this.btnWriteSchema = new System.Windows.Forms.Button();
            this.lblWriteXActXSD = new System.Windows.Forms.Label();
            this.btnWriteDataSet = new System.Windows.Forms.Button();
            this.lblWriteDataSet = new System.Windows.Forms.Label();
            this.btnWriteResultsSchema = new System.Windows.Forms.Button();
            this.lblResultsSchema = new System.Windows.Forms.Label();
            this.grpBoxDataBaseConnectionSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnWriteTransaction
            // 
            this.btnWriteTransaction.Location = new System.Drawing.Point(12, 175);
            this.btnWriteTransaction.Name = "btnWriteTransaction";
            this.btnWriteTransaction.Size = new System.Drawing.Size(167, 23);
            this.btnWriteTransaction.TabIndex = 5;
            this.btnWriteTransaction.Text = "Write Transactions";
            this.btnWriteTransaction.UseVisualStyleBackColor = true;
            this.btnWriteTransaction.Click += new System.EventHandler(this.btnWriteTransaction_Click);
            // 
            // lblWriteTransactions
            // 
            this.lblWriteTransactions.AutoSize = true;
            this.lblWriteTransactions.Location = new System.Drawing.Point(185, 180);
            this.lblWriteTransactions.Name = "lblWriteTransactions";
            this.lblWriteTransactions.Size = new System.Drawing.Size(307, 13);
            this.lblWriteTransactions.TabIndex = 6;
            this.lblWriteTransactions.Text = "Select one or more transactions and persist them to an XML file.";
            // 
            // lblDataSource
            // 
            this.lblDataSource.AutoSize = true;
            this.lblDataSource.Location = new System.Drawing.Point(6, 28);
            this.lblDataSource.Name = "lblDataSource";
            this.lblDataSource.Size = new System.Drawing.Size(70, 13);
            this.lblDataSource.TabIndex = 0;
            this.lblDataSource.Text = "Data Source:";
            // 
            // txtBoxDataSource
            // 
            this.txtBoxDataSource.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBoxDataSource.Location = new System.Drawing.Point(82, 25);
            this.txtBoxDataSource.Name = "txtBoxDataSource";
            this.txtBoxDataSource.Size = new System.Drawing.Size(406, 20);
            this.txtBoxDataSource.TabIndex = 1;
            this.txtBoxDataSource.Text = "localhost";
            this.txtBoxDataSource.TextChanged += new System.EventHandler(this.txtBoxDataSource_TextChanged);
            // 
            // lblInitialCatalog
            // 
            this.lblInitialCatalog.AutoSize = true;
            this.lblInitialCatalog.Location = new System.Drawing.Point(6, 54);
            this.lblInitialCatalog.Name = "lblInitialCatalog";
            this.lblInitialCatalog.Size = new System.Drawing.Size(73, 13);
            this.lblInitialCatalog.TabIndex = 2;
            this.lblInitialCatalog.Text = "Initial Catalog:";
            // 
            // txtBoxInitialCatalog
            // 
            this.txtBoxInitialCatalog.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBoxInitialCatalog.Location = new System.Drawing.Point(82, 51);
            this.txtBoxInitialCatalog.Name = "txtBoxInitialCatalog";
            this.txtBoxInitialCatalog.Size = new System.Drawing.Size(406, 20);
            this.txtBoxInitialCatalog.TabIndex = 3;
            this.txtBoxInitialCatalog.Text = "ConsolidatedDB";
            this.txtBoxInitialCatalog.TextChanged += new System.EventHandler(this.txtBoxInitialCatalog_TextChanged);
            // 
            // grpBoxDataBaseConnectionSettings
            // 
            this.grpBoxDataBaseConnectionSettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grpBoxDataBaseConnectionSettings.Controls.Add(this.lblDataSource);
            this.grpBoxDataBaseConnectionSettings.Controls.Add(this.txtBoxInitialCatalog);
            this.grpBoxDataBaseConnectionSettings.Controls.Add(this.txtBoxDataSource);
            this.grpBoxDataBaseConnectionSettings.Controls.Add(this.lblInitialCatalog);
            this.grpBoxDataBaseConnectionSettings.Location = new System.Drawing.Point(12, 12);
            this.grpBoxDataBaseConnectionSettings.Name = "grpBoxDataBaseConnectionSettings";
            this.grpBoxDataBaseConnectionSettings.Size = new System.Drawing.Size(494, 86);
            this.grpBoxDataBaseConnectionSettings.TabIndex = 0;
            this.grpBoxDataBaseConnectionSettings.TabStop = false;
            this.grpBoxDataBaseConnectionSettings.Text = "Database Connection Settings";
            // 
            // btnWriteSchema
            // 
            this.btnWriteSchema.Location = new System.Drawing.Point(12, 117);
            this.btnWriteSchema.Name = "btnWriteSchema";
            this.btnWriteSchema.Size = new System.Drawing.Size(167, 23);
            this.btnWriteSchema.TabIndex = 1;
            this.btnWriteSchema.Text = "Write Schema";
            this.btnWriteSchema.UseVisualStyleBackColor = true;
            this.btnWriteSchema.Click += new System.EventHandler(this.btnWriteSchema_Click);
            // 
            // lblWriteXActXSD
            // 
            this.lblWriteXActXSD.AutoSize = true;
            this.lblWriteXActXSD.Location = new System.Drawing.Point(185, 122);
            this.lblWriteXActXSD.Name = "lblWriteXActXSD";
            this.lblWriteXActXSD.Size = new System.Drawing.Size(210, 13);
            this.lblWriteXActXSD.TabIndex = 2;
            this.lblWriteXActXSD.Text = "Persist Transaction DataSet schema to file.";
            // 
            // btnWriteDataSet
            // 
            this.btnWriteDataSet.Location = new System.Drawing.Point(12, 146);
            this.btnWriteDataSet.Name = "btnWriteDataSet";
            this.btnWriteDataSet.Size = new System.Drawing.Size(167, 23);
            this.btnWriteDataSet.TabIndex = 3;
            this.btnWriteDataSet.Text = "Write DataSet";
            this.btnWriteDataSet.UseVisualStyleBackColor = true;
            this.btnWriteDataSet.Click += new System.EventHandler(this.btnWriteDataSet_Click);
            // 
            // lblWriteDataSet
            // 
            this.lblWriteDataSet.AutoSize = true;
            this.lblWriteDataSet.Location = new System.Drawing.Point(185, 151);
            this.lblWriteDataSet.Name = "lblWriteDataSet";
            this.lblWriteDataSet.Size = new System.Drawing.Size(170, 13);
            this.lblWriteDataSet.TabIndex = 4;
            this.lblWriteDataSet.Text = "Persist Transaction DataSet to file.";
            // 
            // btnWriteResultsSchema
            // 
            this.btnWriteResultsSchema.Location = new System.Drawing.Point(12, 204);
            this.btnWriteResultsSchema.Name = "btnWriteResultsSchema";
            this.btnWriteResultsSchema.Size = new System.Drawing.Size(167, 23);
            this.btnWriteResultsSchema.TabIndex = 7;
            this.btnWriteResultsSchema.Text = "Write Results Schema";
            this.btnWriteResultsSchema.UseVisualStyleBackColor = true;
            this.btnWriteResultsSchema.Click += new System.EventHandler(this.btnWriteResultsSchema_Click);
            // 
            // lblResultsSchema
            // 
            this.lblResultsSchema.AutoSize = true;
            this.lblResultsSchema.Location = new System.Drawing.Point(185, 209);
            this.lblResultsSchema.Name = "lblResultsSchema";
            this.lblResultsSchema.Size = new System.Drawing.Size(189, 13);
            this.lblResultsSchema.TabIndex = 8;
            this.lblResultsSchema.Text = "Persist Results DataSet schema to file.";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(518, 238);
            this.Controls.Add(this.lblResultsSchema);
            this.Controls.Add(this.btnWriteResultsSchema);
            this.Controls.Add(this.lblWriteDataSet);
            this.Controls.Add(this.btnWriteDataSet);
            this.Controls.Add(this.lblWriteXActXSD);
            this.Controls.Add(this.btnWriteSchema);
            this.Controls.Add(this.grpBoxDataBaseConnectionSettings);
            this.Controls.Add(this.lblWriteTransactions);
            this.Controls.Add(this.btnWriteTransaction);
            this.MinimumSize = new System.Drawing.Size(480, 240);
            this.Name = "MainForm";
            this.Text = "ConsolidatedDB Transactions";
            this.grpBoxDataBaseConnectionSettings.ResumeLayout(false);
            this.grpBoxDataBaseConnectionSettings.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnWriteTransaction;
        private System.Windows.Forms.Label lblWriteTransactions;
        private System.Windows.Forms.Label lblDataSource;
        private System.Windows.Forms.TextBox txtBoxDataSource;
        private System.Windows.Forms.Label lblInitialCatalog;
        private System.Windows.Forms.TextBox txtBoxInitialCatalog;
        private System.Windows.Forms.GroupBox grpBoxDataBaseConnectionSettings;
        private System.Windows.Forms.Button btnWriteSchema;
        private System.Windows.Forms.Label lblWriteXActXSD;
        private System.Windows.Forms.Button btnWriteDataSet;
        private System.Windows.Forms.Label lblWriteDataSet;
        private System.Windows.Forms.Button btnWriteResultsSchema;
        private System.Windows.Forms.Label lblResultsSchema;
    }
}


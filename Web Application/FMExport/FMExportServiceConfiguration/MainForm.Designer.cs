namespace FMExportServiceConfiguration
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.label1 = new System.Windows.Forms.Label();
            this.txtRequestName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbInterfaceNames = new System.Windows.Forms.ComboBox();
            this.txtFTPServer = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtFTPUser = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtFTPPassword = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtCompanyCode = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtRowVersion = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.dtpExportTime = new System.Windows.Forms.DateTimePicker();
            this.label11 = new System.Windows.Forms.Label();
            this.lbCompanies = new System.Windows.Forms.ListBox();
            this.RequestGrid = new System.Windows.Forms.DataGridView();
            this.NewButton = new System.Windows.Forms.Button();
            this.ApplyButton = new System.Windows.Forms.Button();
            this.RefreshButton = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.CloseButton = new System.Windows.Forms.Button();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.DeleteButton = new System.Windows.Forms.Button();
            this.txtExportFrequency = new System.Windows.Forms.TextBox();
            this.dtBaselineDate = new System.Windows.Forms.DateTimePicker();
            this.label13 = new System.Windows.Forms.Label();
            this.lblFrequencyCalc = new System.Windows.Forms.Label();
            this.chkExcludeEmptyFiles = new System.Windows.Forms.CheckBox();
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chkUsePassiveMode = new System.Windows.Forms.CheckBox();
            this.txtOwnerCode = new System.Windows.Forms.TextBox();
            this.lblOwnerCode = new System.Windows.Forms.Label();
            this.chkUseTimeOfDay = new System.Windows.Forms.CheckBox();
            this.dtpTimeToExport = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.lblSendMethod = new System.Windows.Forms.Label();
            this.cmbSendMethod = new System.Windows.Forms.ComboBox();
            this.grpFtpSettings = new System.Windows.Forms.GroupBox();
            this.grpWebServiceSettings = new System.Windows.Forms.GroupBox();
            this.txtWebServiceConfiguration = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbWebServicePlugin = new System.Windows.Forms.ComboBox();
            this.lblWebServicePlugin = new System.Windows.Forms.Label();
            this.RequestNameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.InterfaceNameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OwnerCodeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SendingCompanyCodeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SendMethodColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ExcludeEmptyFilesColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.UseTimeOfDayColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ExportFrequencyColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NextExportTimeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LastExportTimeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LatestRowVersionColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BaselineDateColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UploadStagingFolderColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ArchiveFolderColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CreatedDateColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CreatedByColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UpdatedDateColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UpdatedByColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.RequestGrid)).BeginInit();
            this.mainMenu.SuspendLayout();
            this.grpFtpSettings.SuspendLayout();
            this.grpWebServiceSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 39);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(106, 17);
            this.label1.TabIndex = 17;
            this.label1.Text = "Request Name:";
            // 
            // txtRequestName
            // 
            this.txtRequestName.Location = new System.Drawing.Point(153, 36);
            this.txtRequestName.Margin = new System.Windows.Forms.Padding(4);
            this.txtRequestName.Name = "txtRequestName";
            this.txtRequestName.Size = new System.Drawing.Size(427, 22);
            this.txtRequestName.TabIndex = 1;
            this.txtRequestName.TextChanged += new System.EventHandler(this.ControlValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(589, 38);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(108, 17);
            this.label2.TabIndex = 18;
            this.label2.Text = "Interface Name:";
            // 
            // cmbInterfaceNames
            // 
            this.cmbInterfaceNames.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbInterfaceNames.FormattingEnabled = true;
            this.cmbInterfaceNames.Location = new System.Drawing.Point(715, 34);
            this.cmbInterfaceNames.Margin = new System.Windows.Forms.Padding(4);
            this.cmbInterfaceNames.Name = "cmbInterfaceNames";
            this.cmbInterfaceNames.Size = new System.Drawing.Size(497, 24);
            this.cmbInterfaceNames.Sorted = true;
            this.cmbInterfaceNames.TabIndex = 2;
            this.cmbInterfaceNames.SelectedIndexChanged += new System.EventHandler(this.ControlValueChanged);
            // 
            // txtFTPServer
            // 
            this.txtFTPServer.Location = new System.Drawing.Point(140, 22);
            this.txtFTPServer.Margin = new System.Windows.Forms.Padding(4);
            this.txtFTPServer.Name = "txtFTPServer";
            this.txtFTPServer.Size = new System.Drawing.Size(146, 22);
            this.txtFTPServer.TabIndex = 5;
            this.txtFTPServer.TextChanged += new System.EventHandler(this.ControlValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 25);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(84, 17);
            this.label5.TabIndex = 21;
            this.label5.Text = "FTP Server:";
            // 
            // txtFTPUser
            // 
            this.txtFTPUser.Location = new System.Drawing.Point(140, 52);
            this.txtFTPUser.Margin = new System.Windows.Forms.Padding(4);
            this.txtFTPUser.Name = "txtFTPUser";
            this.txtFTPUser.Size = new System.Drawing.Size(146, 22);
            this.txtFTPUser.TabIndex = 6;
            this.txtFTPUser.TextChanged += new System.EventHandler(this.ControlValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 55);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(72, 17);
            this.label6.TabIndex = 22;
            this.label6.Text = "FTP User:";
            // 
            // txtFTPPassword
            // 
            this.txtFTPPassword.Location = new System.Drawing.Point(140, 82);
            this.txtFTPPassword.Margin = new System.Windows.Forms.Padding(4);
            this.txtFTPPassword.Name = "txtFTPPassword";
            this.txtFTPPassword.PasswordChar = '*';
            this.txtFTPPassword.Size = new System.Drawing.Size(146, 22);
            this.txtFTPPassword.TabIndex = 7;
            this.txtFTPPassword.TextChanged += new System.EventHandler(this.ControlValueChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(7, 85);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(103, 17);
            this.label7.TabIndex = 23;
            this.label7.Text = "FTP Password:";
            // 
            // txtCompanyCode
            // 
            this.txtCompanyCode.Location = new System.Drawing.Point(149, 264);
            this.txtCompanyCode.Margin = new System.Windows.Forms.Padding(4);
            this.txtCompanyCode.Name = "txtCompanyCode";
            this.txtCompanyCode.Size = new System.Drawing.Size(156, 22);
            this.txtCompanyCode.TabIndex = 6;
            this.txtCompanyCode.TextChanged += new System.EventHandler(this.ControlValueChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 269);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(108, 17);
            this.label8.TabIndex = 24;
            this.label8.Text = "Company Code:";
            // 
            // txtRowVersion
            // 
            this.txtRowVersion.Location = new System.Drawing.Point(149, 328);
            this.txtRowVersion.Margin = new System.Windows.Forms.Padding(4);
            this.txtRowVersion.Name = "txtRowVersion";
            this.txtRowVersion.Size = new System.Drawing.Size(156, 22);
            this.txtRowVersion.TabIndex = 8;
            this.txtRowVersion.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtRowVersion.TextChanged += new System.EventHandler(this.ControlValueChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(12, 333);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(91, 17);
            this.label9.TabIndex = 25;
            this.label9.Text = "Row Version:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(12, 441);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(118, 17);
            this.label10.TabIndex = 26;
            this.label10.Text = "Last Export Time:";
            // 
            // dtpExportTime
            // 
            this.dtpExportTime.CustomFormat = "yyyy/MM/dd HH:mm";
            this.dtpExportTime.Enabled = false;
            this.dtpExportTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpExportTime.Location = new System.Drawing.Point(149, 436);
            this.dtpExportTime.Margin = new System.Windows.Forms.Padding(4);
            this.dtpExportTime.MaxDate = new System.DateTime(2050, 12, 31, 0, 0, 0, 0);
            this.dtpExportTime.MinDate = new System.DateTime(1980, 1, 1, 0, 0, 0, 0);
            this.dtpExportTime.Name = "dtpExportTime";
            this.dtpExportTime.ShowUpDown = true;
            this.dtpExportTime.Size = new System.Drawing.Size(156, 22);
            this.dtpExportTime.TabIndex = 11;
            this.dtpExportTime.ValueChanged += new System.EventHandler(this.ControlValueChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(12, 472);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(122, 17);
            this.label11.TabIndex = 27;
            this.label11.Text = "Frequency (secs):";
            // 
            // lbCompanies
            // 
            this.lbCompanies.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lbCompanies.FormattingEnabled = true;
            this.lbCompanies.ItemHeight = 16;
            this.lbCompanies.Location = new System.Drawing.Point(21, 565);
            this.lbCompanies.Margin = new System.Windows.Forms.Padding(4);
            this.lbCompanies.Name = "lbCompanies";
            this.lbCompanies.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.lbCompanies.Size = new System.Drawing.Size(281, 244);
            this.lbCompanies.TabIndex = 14;
            this.lbCompanies.SelectedIndexChanged += new System.EventHandler(this.ControlValueChanged);
            this.lbCompanies.SelectedValueChanged += new System.EventHandler(this.lbCompanies_SelectedValueChanged);
            // 
            // RequestGrid
            // 
            this.RequestGrid.AllowUserToAddRows = false;
            this.RequestGrid.AllowUserToDeleteRows = false;
            this.RequestGrid.AllowUserToOrderColumns = true;
            this.RequestGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RequestGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.RequestGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.RequestGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.RequestNameColumn,
            this.InterfaceNameColumn,
            this.OwnerCodeColumn,
            this.SendingCompanyCodeColumn,
            this.SendMethodColumn,
            this.ExcludeEmptyFilesColumn,
            this.UseTimeOfDayColumn,
            this.ExportFrequencyColumn,
            this.NextExportTimeColumn,
            this.LastExportTimeColumn,
            this.LatestRowVersionColumn,
            this.BaselineDateColumn,
            this.UploadStagingFolderColumn,
            this.ArchiveFolderColumn,
            this.CreatedDateColumn,
            this.CreatedByColumn,
            this.UpdatedDateColumn,
            this.UpdatedByColumn});
            this.RequestGrid.Location = new System.Drawing.Point(320, 68);
            this.RequestGrid.Margin = new System.Windows.Forms.Padding(4);
            this.RequestGrid.MultiSelect = false;
            this.RequestGrid.Name = "RequestGrid";
            this.RequestGrid.ReadOnly = true;
            this.RequestGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.RequestGrid.Size = new System.Drawing.Size(893, 752);
            this.RequestGrid.TabIndex = 19;
            this.RequestGrid.SelectionChanged += new System.EventHandler(this.RequestGrid_SelectionChanged);
            // 
            // NewButton
            // 
            this.NewButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.NewButton.Location = new System.Drawing.Point(20, 825);
            this.NewButton.Margin = new System.Windows.Forms.Padding(4);
            this.NewButton.Name = "NewButton";
            this.NewButton.Size = new System.Drawing.Size(76, 28);
            this.NewButton.TabIndex = 15;
            this.NewButton.Text = "New";
            this.NewButton.UseVisualStyleBackColor = true;
            this.NewButton.Click += new System.EventHandler(this.NewButton_Click);
            // 
            // ApplyButton
            // 
            this.ApplyButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ApplyButton.Enabled = false;
            this.ApplyButton.Location = new System.Drawing.Point(119, 825);
            this.ApplyButton.Margin = new System.Windows.Forms.Padding(4);
            this.ApplyButton.Name = "ApplyButton";
            this.ApplyButton.Size = new System.Drawing.Size(79, 28);
            this.ApplyButton.TabIndex = 16;
            this.ApplyButton.Text = "Apply";
            this.ApplyButton.UseVisualStyleBackColor = true;
            this.ApplyButton.Click += new System.EventHandler(this.ApplyButton_Click);
            // 
            // RefreshButton
            // 
            this.RefreshButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.RefreshButton.Location = new System.Drawing.Point(220, 825);
            this.RefreshButton.Margin = new System.Windows.Forms.Padding(4);
            this.RefreshButton.Name = "RefreshButton";
            this.RefreshButton.Size = new System.Drawing.Size(76, 28);
            this.RefreshButton.TabIndex = 17;
            this.RefreshButton.Text = "Refresh";
            this.RefreshButton.UseVisualStyleBackColor = true;
            this.RefreshButton.Click += new System.EventHandler(this.RefreshButton_Click);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(18, 544);
            this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(121, 17);
            this.label12.TabIndex = 28;
            this.label12.Text = "Select Companies";
            // 
            // CloseButton
            // 
            this.CloseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CloseButton.Location = new System.Drawing.Point(1113, 825);
            this.CloseButton.Margin = new System.Windows.Forms.Padding(4);
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.Size = new System.Drawing.Size(100, 28);
            this.CloseButton.TabIndex = 22;
            this.CloseButton.Text = "Close";
            this.CloseButton.UseVisualStyleBackColor = true;
            this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
            // 
            // DeleteButton
            // 
            this.DeleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.DeleteButton.Enabled = false;
            this.DeleteButton.Location = new System.Drawing.Point(319, 825);
            this.DeleteButton.Margin = new System.Windows.Forms.Padding(4);
            this.DeleteButton.Name = "DeleteButton";
            this.DeleteButton.Size = new System.Drawing.Size(84, 28);
            this.DeleteButton.TabIndex = 18;
            this.DeleteButton.Text = "Delete";
            this.DeleteButton.UseVisualStyleBackColor = true;
            this.DeleteButton.Click += new System.EventHandler(this.DeleteButton_Click);
            // 
            // txtExportFrequency
            // 
            this.txtExportFrequency.Location = new System.Drawing.Point(149, 467);
            this.txtExportFrequency.Margin = new System.Windows.Forms.Padding(4);
            this.txtExportFrequency.Name = "txtExportFrequency";
            this.txtExportFrequency.Size = new System.Drawing.Size(156, 22);
            this.txtExportFrequency.TabIndex = 12;
            this.txtExportFrequency.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtExportFrequency.TextChanged += new System.EventHandler(this.txtExportFrequency_TextChanged);
            // 
            // dtBaselineDate
            // 
            this.dtBaselineDate.CustomFormat = "yyyy/MM/dd";
            this.dtBaselineDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtBaselineDate.Location = new System.Drawing.Point(148, 515);
            this.dtBaselineDate.Margin = new System.Windows.Forms.Padding(4);
            this.dtBaselineDate.MaxDate = new System.DateTime(2050, 12, 31, 0, 0, 0, 0);
            this.dtBaselineDate.MinDate = new System.DateTime(1980, 1, 1, 0, 0, 0, 0);
            this.dtBaselineDate.Name = "dtBaselineDate";
            this.dtBaselineDate.Size = new System.Drawing.Size(156, 22);
            this.dtBaselineDate.TabIndex = 13;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(12, 520);
            this.label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(100, 17);
            this.label13.TabIndex = 36;
            this.label13.Text = "Baseline Date:";
            // 
            // lblFrequencyCalc
            // 
            this.lblFrequencyCalc.AutoSize = true;
            this.lblFrequencyCalc.ForeColor = System.Drawing.Color.Red;
            this.lblFrequencyCalc.Location = new System.Drawing.Point(145, 495);
            this.lblFrequencyCalc.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblFrequencyCalc.Name = "lblFrequencyCalc";
            this.lblFrequencyCalc.Size = new System.Drawing.Size(86, 17);
            this.lblFrequencyCalc.TabIndex = 37;
            this.lblFrequencyCalc.Text = "1d 1h 1m 1s";
            // 
            // chkExcludeEmptyFiles
            // 
            this.chkExcludeEmptyFiles.AutoSize = true;
            this.chkExcludeEmptyFiles.Location = new System.Drawing.Point(12, 241);
            this.chkExcludeEmptyFiles.Margin = new System.Windows.Forms.Padding(4);
            this.chkExcludeEmptyFiles.Name = "chkExcludeEmptyFiles";
            this.chkExcludeEmptyFiles.Size = new System.Drawing.Size(155, 21);
            this.chkExcludeEmptyFiles.TabIndex = 5;
            this.chkExcludeEmptyFiles.Text = "Exclude Empty Files";
            this.chkExcludeEmptyFiles.UseVisualStyleBackColor = true;
            this.chkExcludeEmptyFiles.CheckedChanged += new System.EventHandler(this.ControlValueChanged);
            // 
            // mainMenu
            // 
            this.mainMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.mainMenu.Location = new System.Drawing.Point(0, 0);
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
            this.mainMenu.Size = new System.Drawing.Size(1235, 28);
            this.mainMenu.TabIndex = 0;
            this.mainMenu.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(44, 24);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(108, 26);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // chkUsePassiveMode
            // 
            this.chkUsePassiveMode.AutoSize = true;
            this.chkUsePassiveMode.Checked = true;
            this.chkUsePassiveMode.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUsePassiveMode.Location = new System.Drawing.Point(140, 111);
            this.chkUsePassiveMode.Margin = new System.Windows.Forms.Padding(4);
            this.chkUsePassiveMode.Name = "chkUsePassiveMode";
            this.chkUsePassiveMode.Size = new System.Drawing.Size(147, 21);
            this.chkUsePassiveMode.TabIndex = 38;
            this.chkUsePassiveMode.Text = "Use Passive Mode";
            this.chkUsePassiveMode.UseVisualStyleBackColor = true;
            this.chkUsePassiveMode.CheckedChanged += new System.EventHandler(this.ControlValueChanged);
            // 
            // txtOwnerCode
            // 
            this.txtOwnerCode.Location = new System.Drawing.Point(149, 296);
            this.txtOwnerCode.Margin = new System.Windows.Forms.Padding(4);
            this.txtOwnerCode.Name = "txtOwnerCode";
            this.txtOwnerCode.Size = new System.Drawing.Size(156, 22);
            this.txtOwnerCode.TabIndex = 7;
            this.txtOwnerCode.TextChanged += new System.EventHandler(this.ControlValueChanged);
            // 
            // lblOwnerCode
            // 
            this.lblOwnerCode.AutoSize = true;
            this.lblOwnerCode.Location = new System.Drawing.Point(12, 301);
            this.lblOwnerCode.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblOwnerCode.Name = "lblOwnerCode";
            this.lblOwnerCode.Size = new System.Drawing.Size(90, 17);
            this.lblOwnerCode.TabIndex = 40;
            this.lblOwnerCode.Text = "Owner Code:";
            // 
            // chkUseTimeOfDay
            // 
            this.chkUseTimeOfDay.AutoSize = true;
            this.chkUseTimeOfDay.Location = new System.Drawing.Point(12, 376);
            this.chkUseTimeOfDay.Margin = new System.Windows.Forms.Padding(4);
            this.chkUseTimeOfDay.Name = "chkUseTimeOfDay";
            this.chkUseTimeOfDay.Size = new System.Drawing.Size(200, 21);
            this.chkUseTimeOfDay.TabIndex = 9;
            this.chkUseTimeOfDay.Text = "Use Time of Day for Export";
            this.chkUseTimeOfDay.UseVisualStyleBackColor = true;
            this.chkUseTimeOfDay.CheckedChanged += new System.EventHandler(this.chkUseTimeOfDay_CheckedChanged);
            // 
            // dtpTimeToExport
            // 
            this.dtpTimeToExport.CustomFormat = "yyyy/MM/dd HH:mm";
            this.dtpTimeToExport.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpTimeToExport.Location = new System.Drawing.Point(149, 403);
            this.dtpTimeToExport.Margin = new System.Windows.Forms.Padding(4);
            this.dtpTimeToExport.MaxDate = new System.DateTime(2050, 12, 31, 0, 0, 0, 0);
            this.dtpTimeToExport.MinDate = new System.DateTime(1980, 1, 1, 0, 0, 0, 0);
            this.dtpTimeToExport.Name = "dtpTimeToExport";
            this.dtpTimeToExport.ShowUpDown = true;
            this.dtpTimeToExport.Size = new System.Drawing.Size(156, 22);
            this.dtpTimeToExport.TabIndex = 10;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 408);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(135, 17);
            this.label3.TabIndex = 43;
            this.label3.Text = "Next Time to Export:";
            // 
            // lblSendMethod
            // 
            this.lblSendMethod.AutoSize = true;
            this.lblSendMethod.Location = new System.Drawing.Point(12, 71);
            this.lblSendMethod.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSendMethod.Name = "lblSendMethod";
            this.lblSendMethod.Size = new System.Drawing.Size(96, 17);
            this.lblSendMethod.TabIndex = 44;
            this.lblSendMethod.Text = "Send Method:";
            // 
            // cmbSendMethod
            // 
            this.cmbSendMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSendMethod.FormattingEnabled = true;
            this.cmbSendMethod.Items.AddRange(new object[] {
            "None",
            "FTP (Insecure)",
            "FTPS (Secure)",
            "Web Service"});
            this.cmbSendMethod.Location = new System.Drawing.Point(149, 68);
            this.cmbSendMethod.Name = "cmbSendMethod";
            this.cmbSendMethod.Size = new System.Drawing.Size(156, 24);
            this.cmbSendMethod.TabIndex = 3;
            this.cmbSendMethod.SelectedIndexChanged += new System.EventHandler(this.cmbSendMethod_SelectedIndexChanged);
            // 
            // grpFtpSettings
            // 
            this.grpFtpSettings.Controls.Add(this.txtFTPServer);
            this.grpFtpSettings.Controls.Add(this.label5);
            this.grpFtpSettings.Controls.Add(this.label6);
            this.grpFtpSettings.Controls.Add(this.txtFTPUser);
            this.grpFtpSettings.Controls.Add(this.label7);
            this.grpFtpSettings.Controls.Add(this.txtFTPPassword);
            this.grpFtpSettings.Controls.Add(this.chkUsePassiveMode);
            this.grpFtpSettings.Location = new System.Drawing.Point(12, 95);
            this.grpFtpSettings.Name = "grpFtpSettings";
            this.grpFtpSettings.Size = new System.Drawing.Size(293, 139);
            this.grpFtpSettings.TabIndex = 0;
            this.grpFtpSettings.TabStop = false;
            this.grpFtpSettings.Text = "FTP Settings";
            // 
            // grpWebServiceSettings
            // 
            this.grpWebServiceSettings.Controls.Add(this.txtWebServiceConfiguration);
            this.grpWebServiceSettings.Controls.Add(this.label4);
            this.grpWebServiceSettings.Controls.Add(this.cmbWebServicePlugin);
            this.grpWebServiceSettings.Controls.Add(this.lblWebServicePlugin);
            this.grpWebServiceSettings.Location = new System.Drawing.Point(12, 95);
            this.grpWebServiceSettings.Name = "grpWebServiceSettings";
            this.grpWebServiceSettings.Size = new System.Drawing.Size(293, 139);
            this.grpWebServiceSettings.TabIndex = 4;
            this.grpWebServiceSettings.TabStop = false;
            this.grpWebServiceSettings.Text = "Web Service Settings";
            // 
            // txtWebServiceConfiguration
            // 
            this.txtWebServiceConfiguration.Location = new System.Drawing.Point(7, 102);
            this.txtWebServiceConfiguration.MaxLength = 512;
            this.txtWebServiceConfiguration.Name = "txtWebServiceConfiguration";
            this.txtWebServiceConfiguration.Size = new System.Drawing.Size(277, 22);
            this.txtWebServiceConfiguration.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 82);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(137, 17);
            this.label4.TabIndex = 2;
            this.label4.Text = "Configuration String:";
            // 
            // cmbWebServicePlugin
            // 
            this.cmbWebServicePlugin.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbWebServicePlugin.FormattingEnabled = true;
            this.cmbWebServicePlugin.Location = new System.Drawing.Point(8, 52);
            this.cmbWebServicePlugin.Name = "cmbWebServicePlugin";
            this.cmbWebServicePlugin.Size = new System.Drawing.Size(276, 24);
            this.cmbWebServicePlugin.TabIndex = 1;
            // 
            // lblWebServicePlugin
            // 
            this.lblWebServicePlugin.AutoSize = true;
            this.lblWebServicePlugin.Location = new System.Drawing.Point(7, 27);
            this.lblWebServicePlugin.Name = "lblWebServicePlugin";
            this.lblWebServicePlugin.Size = new System.Drawing.Size(140, 17);
            this.lblWebServicePlugin.TabIndex = 0;
            this.lblWebServicePlugin.Text = "Web Service Plug-in:";
            // 
            // RequestNameColumn
            // 
            this.RequestNameColumn.DataPropertyName = "RequestId";
            this.RequestNameColumn.HeaderText = "Request Name";
            this.RequestNameColumn.Name = "RequestNameColumn";
            this.RequestNameColumn.ReadOnly = true;
            this.RequestNameColumn.Width = 120;
            // 
            // InterfaceNameColumn
            // 
            this.InterfaceNameColumn.DataPropertyName = "InterfaceId";
            this.InterfaceNameColumn.HeaderText = "Interface Name";
            this.InterfaceNameColumn.Name = "InterfaceNameColumn";
            this.InterfaceNameColumn.ReadOnly = true;
            this.InterfaceNameColumn.Width = 122;
            // 
            // OwnerCodeColumn
            // 
            this.OwnerCodeColumn.DataPropertyName = "OwnerCode";
            this.OwnerCodeColumn.HeaderText = "Owner Code";
            this.OwnerCodeColumn.Name = "OwnerCodeColumn";
            this.OwnerCodeColumn.ReadOnly = true;
            this.OwnerCodeColumn.Width = 106;
            // 
            // SendingCompanyCodeColumn
            // 
            this.SendingCompanyCodeColumn.DataPropertyName = "SendingCompanyCode";
            this.SendingCompanyCodeColumn.HeaderText = "Sending Company Code";
            this.SendingCompanyCodeColumn.Name = "SendingCompanyCodeColumn";
            this.SendingCompanyCodeColumn.ReadOnly = true;
            this.SendingCompanyCodeColumn.Width = 143;
            // 
            // SendMethodColumn
            // 
            this.SendMethodColumn.DataPropertyName = "SendMethod";
            this.SendMethodColumn.HeaderText = "Send Method";
            this.SendMethodColumn.Name = "SendMethodColumn";
            this.SendMethodColumn.ReadOnly = true;
            this.SendMethodColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.SendMethodColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.SendMethodColumn.Width = 88;
            // 
            // ExcludeEmptyFilesColumn
            // 
            this.ExcludeEmptyFilesColumn.DataPropertyName = "ExcludeEmptyFiles";
            this.ExcludeEmptyFilesColumn.HeaderText = "Exclude Empty Files";
            this.ExcludeEmptyFilesColumn.Name = "ExcludeEmptyFilesColumn";
            this.ExcludeEmptyFilesColumn.ReadOnly = true;
            this.ExcludeEmptyFilesColumn.Width = 99;
            // 
            // UseTimeOfDayColumn
            // 
            this.UseTimeOfDayColumn.DataPropertyName = "UseTimeOfDay";
            this.UseTimeOfDayColumn.HeaderText = "Use Time Of Day";
            this.UseTimeOfDayColumn.Name = "UseTimeOfDayColumn";
            this.UseTimeOfDayColumn.ReadOnly = true;
            this.UseTimeOfDayColumn.Width = 87;
            // 
            // ExportFrequencyColumn
            // 
            this.ExportFrequencyColumn.DataPropertyName = "ExportFrequency";
            this.ExportFrequencyColumn.HeaderText = "Export Frequency";
            this.ExportFrequencyColumn.Name = "ExportFrequencyColumn";
            this.ExportFrequencyColumn.ReadOnly = true;
            this.ExportFrequencyColumn.Width = 136;
            // 
            // NextExportTimeColumn
            // 
            this.NextExportTimeColumn.DataPropertyName = "NextExportTime";
            this.NextExportTimeColumn.HeaderText = "Next Export Time";
            this.NextExportTimeColumn.Name = "NextExportTimeColumn";
            this.NextExportTimeColumn.ReadOnly = true;
            this.NextExportTimeColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.NextExportTimeColumn.Width = 132;
            // 
            // LastExportTimeColumn
            // 
            this.LastExportTimeColumn.DataPropertyName = "LastExportTime";
            this.LastExportTimeColumn.HeaderText = "Last Export Time";
            this.LastExportTimeColumn.Name = "LastExportTimeColumn";
            this.LastExportTimeColumn.ReadOnly = true;
            this.LastExportTimeColumn.Width = 131;
            // 
            // LatestRowVersionColumn
            // 
            this.LatestRowVersionColumn.DataPropertyName = "LatestRowVersion";
            this.LatestRowVersionColumn.HeaderText = "Latest Row Version";
            this.LatestRowVersionColumn.Name = "LatestRowVersionColumn";
            this.LatestRowVersionColumn.ReadOnly = true;
            this.LatestRowVersionColumn.Width = 145;
            // 
            // BaselineDateColumn
            // 
            this.BaselineDateColumn.DataPropertyName = "BaselineDate";
            this.BaselineDateColumn.HeaderText = "Baseline Date";
            this.BaselineDateColumn.Name = "BaselineDateColumn";
            this.BaselineDateColumn.ReadOnly = true;
            this.BaselineDateColumn.Width = 115;
            // 
            // UploadStagingFolderColumn
            // 
            this.UploadStagingFolderColumn.DataPropertyName = "UploadStagingFolder";
            this.UploadStagingFolderColumn.HeaderText = "Upload Staging Folder";
            this.UploadStagingFolderColumn.Name = "UploadStagingFolderColumn";
            this.UploadStagingFolderColumn.ReadOnly = true;
            this.UploadStagingFolderColumn.Width = 163;
            // 
            // ArchiveFolderColumn
            // 
            this.ArchiveFolderColumn.DataPropertyName = "ArchiveFolder";
            this.ArchiveFolderColumn.HeaderText = "Archive Folder";
            this.ArchiveFolderColumn.Name = "ArchiveFolderColumn";
            this.ArchiveFolderColumn.ReadOnly = true;
            this.ArchiveFolderColumn.Width = 118;
            // 
            // CreatedDateColumn
            // 
            this.CreatedDateColumn.DataPropertyName = "CreatedDate";
            this.CreatedDateColumn.HeaderText = "Created";
            this.CreatedDateColumn.Name = "CreatedDateColumn";
            this.CreatedDateColumn.ReadOnly = true;
            this.CreatedDateColumn.Width = 87;
            // 
            // CreatedByColumn
            // 
            this.CreatedByColumn.DataPropertyName = "CreatedBy";
            this.CreatedByColumn.HeaderText = "Created By";
            this.CreatedByColumn.Name = "CreatedByColumn";
            this.CreatedByColumn.ReadOnly = true;
            this.CreatedByColumn.Width = 99;
            // 
            // UpdatedDateColumn
            // 
            this.UpdatedDateColumn.DataPropertyName = "UpdatedDate";
            this.UpdatedDateColumn.HeaderText = "Updated";
            this.UpdatedDateColumn.Name = "UpdatedDateColumn";
            this.UpdatedDateColumn.ReadOnly = true;
            this.UpdatedDateColumn.Width = 91;
            // 
            // UpdatedByColumn
            // 
            this.UpdatedByColumn.DataPropertyName = "UpdatedBy";
            this.UpdatedByColumn.HeaderText = "Updated By";
            this.UpdatedByColumn.Name = "UpdatedByColumn";
            this.UpdatedByColumn.ReadOnly = true;
            this.UpdatedByColumn.Width = 102;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1235, 854);
            this.Controls.Add(this.grpFtpSettings);
            this.Controls.Add(this.cmbSendMethod);
            this.Controls.Add(this.lblSendMethod);
            this.Controls.Add(this.dtpTimeToExport);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.chkUseTimeOfDay);
            this.Controls.Add(this.txtOwnerCode);
            this.Controls.Add(this.lblOwnerCode);
            this.Controls.Add(this.chkExcludeEmptyFiles);
            this.Controls.Add(this.lblFrequencyCalc);
            this.Controls.Add(this.dtBaselineDate);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.txtExportFrequency);
            this.Controls.Add(this.DeleteButton);
            this.Controls.Add(this.CloseButton);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.RefreshButton);
            this.Controls.Add(this.ApplyButton);
            this.Controls.Add(this.NewButton);
            this.Controls.Add(this.lbCompanies);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.dtpExportTime);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.txtRowVersion);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.txtCompanyCode);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.cmbInterfaceNames);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtRequestName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.mainMenu);
            this.Controls.Add(this.RequestGrid);
            this.Controls.Add(this.grpWebServiceSettings);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.mainMenu;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MainForm";
            this.Text = "FuelsManager Export Configuration";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.RequestGrid)).EndInit();
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            this.grpFtpSettings.ResumeLayout(false);
            this.grpFtpSettings.PerformLayout();
            this.grpWebServiceSettings.ResumeLayout(false);
            this.grpWebServiceSettings.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtRequestName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbInterfaceNames;
        private System.Windows.Forms.TextBox txtFTPServer;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtFTPUser;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtFTPPassword;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtCompanyCode;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtRowVersion;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.DateTimePicker dtpExportTime;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ListBox lbCompanies;
        private System.Windows.Forms.DataGridView RequestGrid;
        private System.Windows.Forms.Button NewButton;
        private System.Windows.Forms.Button ApplyButton;
        private System.Windows.Forms.Button RefreshButton;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button CloseButton;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.Button DeleteButton;
        private System.Windows.Forms.TextBox txtExportFrequency;
        private System.Windows.Forms.DateTimePicker dtBaselineDate;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label lblFrequencyCalc;
        private System.Windows.Forms.CheckBox chkExcludeEmptyFiles;
        private System.Windows.Forms.MenuStrip mainMenu;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.CheckBox chkUsePassiveMode;
        private System.Windows.Forms.TextBox txtOwnerCode;
        private System.Windows.Forms.Label lblOwnerCode;
        private System.Windows.Forms.CheckBox chkUseTimeOfDay;
        private System.Windows.Forms.DateTimePicker dtpTimeToExport;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblSendMethod;
        private System.Windows.Forms.ComboBox cmbSendMethod;
        private System.Windows.Forms.GroupBox grpFtpSettings;
        private System.Windows.Forms.GroupBox grpWebServiceSettings;
        private System.Windows.Forms.TextBox txtWebServiceConfiguration;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbWebServicePlugin;
        private System.Windows.Forms.Label lblWebServicePlugin;
        private System.Windows.Forms.DataGridViewTextBoxColumn RequestNameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn InterfaceNameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn OwnerCodeColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn SendingCompanyCodeColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn SendMethodColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ExcludeEmptyFilesColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn UseTimeOfDayColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn ExportFrequencyColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn NextExportTimeColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn LastExportTimeColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn LatestRowVersionColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn BaselineDateColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn UploadStagingFolderColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn ArchiveFolderColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn CreatedDateColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn CreatedByColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn UpdatedDateColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn UpdatedByColumn;
    }
}


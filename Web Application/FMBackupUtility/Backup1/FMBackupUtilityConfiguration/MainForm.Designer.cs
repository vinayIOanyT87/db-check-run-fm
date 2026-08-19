namespace FMBackupUtilityConfiguration
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.notifyIconBUC = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStripNotifyIcon = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiOpenBackupUtilityConfiguration = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiExit = new System.Windows.Forms.ToolStripMenuItem();
            this.timerNotifyIcon = new System.Timers.Timer();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.btnViewLog = new System.Windows.Forms.Button();
            this.btnHelp = new System.Windows.Forms.Button();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.gbZip = new System.Windows.Forms.GroupBox();
            this.btnZipLocation = new System.Windows.Forms.Button();
            this.tbZipFileLocation = new System.Windows.Forms.TextBox();
            this.gbAdditionalFiles = new System.Windows.Forms.GroupBox();
            this.btnBrowseFilesLocation = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.lbFilesLocations = new System.Windows.Forms.ListBox();
            this.lblAddtionalFilesLocations = new System.Windows.Forms.Label();
            this.gbLogFile = new System.Windows.Forms.GroupBox();
            this.btnBrowseLogLocation = new System.Windows.Forms.Button();
            this.tbLogFileLocation = new System.Windows.Forms.TextBox();
            this.gbBackup = new System.Windows.Forms.GroupBox();
            this.btnBackUpNow = new System.Windows.Forms.Button();
            this.dtpStartTime = new System.Windows.Forms.DateTimePicker();
            this.lblStartTime = new System.Windows.Forms.Label();
            this.gbLogIn = new System.Windows.Forms.GroupBox();
            this.btnLogIn = new System.Windows.Forms.Button();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.lblHeader = new System.Windows.Forms.Label();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.lvLog = new System.Windows.Forms.ListView();
            this.columnHeaderDateTime = new System.Windows.Forms.ColumnHeader();
            this.btnPrint = new System.Windows.Forms.Button();
            this.statusStripMain = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelMsg = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolTipMain = new System.Windows.Forms.ToolTip(this.components);
            this.contextMenuStripNotifyIcon.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.timerNotifyIcon)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.gbZip.SuspendLayout();
            this.gbAdditionalFiles.SuspendLayout();
            this.gbLogFile.SuspendLayout();
            this.gbBackup.SuspendLayout();
            this.gbLogIn.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.statusStripMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // notifyIconBUC
            // 
            this.notifyIconBUC.ContextMenuStrip = this.contextMenuStripNotifyIcon;
            this.notifyIconBUC.Text = "FuelsManager Backup Utility";
            this.notifyIconBUC.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIconBUC_MouseDoubleClick);
            // 
            // contextMenuStripNotifyIcon
            // 
            this.contextMenuStripNotifyIcon.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiOpenBackupUtilityConfiguration,
            this.toolStripSeparator1,
            this.tsmiHelp,
            this.tsmiAbout,
            this.toolStripSeparator2,
            this.tsmiExit});
            this.contextMenuStripNotifyIcon.Name = "contextMenuStripNotifyIcon";
            this.contextMenuStripNotifyIcon.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.contextMenuStripNotifyIcon.ShowImageMargin = false;
            this.contextMenuStripNotifyIcon.Size = new System.Drawing.Size(250, 104);
            // 
            // tsmiOpenBackupUtilityConfiguration
            // 
            this.tsmiOpenBackupUtilityConfiguration.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.tsmiOpenBackupUtilityConfiguration.Name = "tsmiOpenBackupUtilityConfiguration";
            this.tsmiOpenBackupUtilityConfiguration.Size = new System.Drawing.Size(249, 22);
            this.tsmiOpenBackupUtilityConfiguration.Text = "Open Backup Utility Configuration";
            this.tsmiOpenBackupUtilityConfiguration.Click += new System.EventHandler(this.tsmiOpenBackupUtilityConfiguration_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(246, 6);
            // 
            // tsmiHelp
            // 
            this.tsmiHelp.Name = "tsmiHelp";
            this.tsmiHelp.Size = new System.Drawing.Size(249, 22);
            this.tsmiHelp.Text = "Help";
            this.tsmiHelp.Click += new System.EventHandler(this.tsmiHelp_Click);
            // 
            // tsmiAbout
            // 
            this.tsmiAbout.Name = "tsmiAbout";
            this.tsmiAbout.Size = new System.Drawing.Size(249, 22);
            this.tsmiAbout.Text = "About";
            this.tsmiAbout.Click += new System.EventHandler(this.tsmiAbout_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(246, 6);
            // 
            // tsmiExit
            // 
            this.tsmiExit.Name = "tsmiExit";
            this.tsmiExit.Size = new System.Drawing.Size(249, 22);
            this.tsmiExit.Text = "Exit";
            this.tsmiExit.Click += new System.EventHandler(this.tsmiExit_Click);
            // 
            // timerNotifyIcon
            // 
            this.timerNotifyIcon.Interval = 500;
            this.timerNotifyIcon.SynchronizingObject = this;
            this.timerNotifyIcon.Elapsed += new System.Timers.ElapsedEventHandler(this.timerNotifyIcon_Elapsed);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.btnViewLog);
            this.splitContainer1.Panel1.Controls.Add(this.btnHelp);
            this.splitContainer1.Panel1.Controls.Add(this.btnApply);
            this.splitContainer1.Panel1.Controls.Add(this.btnCancel);
            this.splitContainer1.Panel1.Controls.Add(this.btnOK);
            this.splitContainer1.Panel1.Controls.Add(this.gbZip);
            this.splitContainer1.Panel1.Controls.Add(this.gbAdditionalFiles);
            this.splitContainer1.Panel1.Controls.Add(this.gbLogFile);
            this.splitContainer1.Panel1.Controls.Add(this.gbBackup);
            this.splitContainer1.Panel1.Controls.Add(this.gbLogIn);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(769, 629);
            this.splitContainer1.SplitterDistance = 353;
            this.splitContainer1.TabIndex = 1;
            // 
            // btnViewLog
            // 
            this.btnViewLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnViewLog.Location = new System.Drawing.Point(263, 571);
            this.btnViewLog.Name = "btnViewLog";
            this.btnViewLog.Size = new System.Drawing.Size(75, 23);
            this.btnViewLog.TabIndex = 9;
            this.btnViewLog.Text = "<< Hide Log";
            this.btnViewLog.UseVisualStyleBackColor = true;
            this.btnViewLog.Click += new System.EventHandler(this.btnViewLog_Click);
            // 
            // btnHelp
            // 
            this.btnHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnHelp.Location = new System.Drawing.Point(263, 542);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(75, 23);
            this.btnHelp.TabIndex = 8;
            this.btnHelp.Text = "Help";
            this.btnHelp.UseVisualStyleBackColor = true;
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // btnApply
            // 
            this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnApply.Enabled = false;
            this.btnApply.Location = new System.Drawing.Point(182, 542);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 7;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(101, 542);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOK.Location = new System.Drawing.Point(20, 542);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // gbZip
            // 
            this.gbZip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.gbZip.Controls.Add(this.btnZipLocation);
            this.gbZip.Controls.Add(this.tbZipFileLocation);
            this.gbZip.Location = new System.Drawing.Point(12, 431);
            this.gbZip.Name = "gbZip";
            this.gbZip.Size = new System.Drawing.Size(335, 103);
            this.gbZip.TabIndex = 4;
            this.gbZip.TabStop = false;
            this.gbZip.Text = "Zip file location";
            // 
            // btnZipLocation
            // 
            this.btnZipLocation.Enabled = false;
            this.btnZipLocation.Location = new System.Drawing.Point(199, 72);
            this.btnZipLocation.Name = "btnZipLocation";
            this.btnZipLocation.Size = new System.Drawing.Size(75, 23);
            this.btnZipLocation.TabIndex = 3;
            this.btnZipLocation.Text = "Browse...";
            this.btnZipLocation.UseVisualStyleBackColor = true;
            this.btnZipLocation.Click += new System.EventHandler(this.btnZipLocation_Click);
            // 
            // tbZipFileLocation
            // 
            this.tbZipFileLocation.Location = new System.Drawing.Point(11, 20);
            this.tbZipFileLocation.Multiline = true;
            this.tbZipFileLocation.Name = "tbZipFileLocation";
            this.tbZipFileLocation.ReadOnly = true;
            this.tbZipFileLocation.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbZipFileLocation.Size = new System.Drawing.Size(312, 46);
            this.tbZipFileLocation.TabIndex = 2;
            // 
            // gbAdditionalFiles
            // 
            this.gbAdditionalFiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.gbAdditionalFiles.Controls.Add(this.btnBrowseFilesLocation);
            this.gbAdditionalFiles.Controls.Add(this.btnRemove);
            this.gbAdditionalFiles.Controls.Add(this.lbFilesLocations);
            this.gbAdditionalFiles.Controls.Add(this.lblAddtionalFilesLocations);
            this.gbAdditionalFiles.Location = new System.Drawing.Point(12, 268);
            this.gbAdditionalFiles.Name = "gbAdditionalFiles";
            this.gbAdditionalFiles.Size = new System.Drawing.Size(335, 157);
            this.gbAdditionalFiles.TabIndex = 3;
            this.gbAdditionalFiles.TabStop = false;
            this.gbAdditionalFiles.Text = "Additional files to back up";
            // 
            // btnBrowseFilesLocation
            // 
            this.btnBrowseFilesLocation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnBrowseFilesLocation.Enabled = false;
            this.btnBrowseFilesLocation.Location = new System.Drawing.Point(199, 124);
            this.btnBrowseFilesLocation.Name = "btnBrowseFilesLocation";
            this.btnBrowseFilesLocation.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseFilesLocation.TabIndex = 3;
            this.btnBrowseFilesLocation.Text = "Browse...";
            this.btnBrowseFilesLocation.UseVisualStyleBackColor = true;
            this.btnBrowseFilesLocation.Click += new System.EventHandler(this.btnBrowseFilesLocation_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRemove.Enabled = false;
            this.btnRemove.Location = new System.Drawing.Point(52, 124);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(75, 23);
            this.btnRemove.TabIndex = 2;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // lbFilesLocations
            // 
            this.lbFilesLocations.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.lbFilesLocations.FormattingEnabled = true;
            this.lbFilesLocations.HorizontalScrollbar = true;
            this.lbFilesLocations.Location = new System.Drawing.Point(11, 36);
            this.lbFilesLocations.Name = "lbFilesLocations";
            this.lbFilesLocations.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbFilesLocations.Size = new System.Drawing.Size(312, 82);
            this.lbFilesLocations.TabIndex = 1;
            this.lbFilesLocations.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lbFilesLocations_MouseMove);
            // 
            // lblAddtionalFilesLocations
            // 
            this.lblAddtionalFilesLocations.AutoSize = true;
            this.lblAddtionalFilesLocations.Location = new System.Drawing.Point(8, 20);
            this.lblAddtionalFilesLocations.Name = "lblAddtionalFilesLocations";
            this.lblAddtionalFilesLocations.Size = new System.Drawing.Size(76, 13);
            this.lblAddtionalFilesLocations.TabIndex = 0;
            this.lblAddtionalFilesLocations.Text = "Files locations:";
            // 
            // gbLogFile
            // 
            this.gbLogFile.Controls.Add(this.btnBrowseLogLocation);
            this.gbLogFile.Controls.Add(this.tbLogFileLocation);
            this.gbLogFile.Location = new System.Drawing.Point(12, 159);
            this.gbLogFile.Name = "gbLogFile";
            this.gbLogFile.Size = new System.Drawing.Size(335, 103);
            this.gbLogFile.TabIndex = 2;
            this.gbLogFile.TabStop = false;
            this.gbLogFile.Text = "Log file location";
            // 
            // btnBrowseLogLocation
            // 
            this.btnBrowseLogLocation.Enabled = false;
            this.btnBrowseLogLocation.Location = new System.Drawing.Point(199, 72);
            this.btnBrowseLogLocation.Name = "btnBrowseLogLocation";
            this.btnBrowseLogLocation.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseLogLocation.TabIndex = 1;
            this.btnBrowseLogLocation.Text = "Browse...";
            this.btnBrowseLogLocation.UseVisualStyleBackColor = true;
            this.btnBrowseLogLocation.Click += new System.EventHandler(this.btnBrowseLogLocation_Click);
            // 
            // tbLogFileLocation
            // 
            this.tbLogFileLocation.Location = new System.Drawing.Point(11, 20);
            this.tbLogFileLocation.Multiline = true;
            this.tbLogFileLocation.Name = "tbLogFileLocation";
            this.tbLogFileLocation.ReadOnly = true;
            this.tbLogFileLocation.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbLogFileLocation.Size = new System.Drawing.Size(312, 46);
            this.tbLogFileLocation.TabIndex = 0;
            // 
            // gbBackup
            // 
            this.gbBackup.Controls.Add(this.btnBackUpNow);
            this.gbBackup.Controls.Add(this.dtpStartTime);
            this.gbBackup.Controls.Add(this.lblStartTime);
            this.gbBackup.Location = new System.Drawing.Point(12, 71);
            this.gbBackup.Name = "gbBackup";
            this.gbBackup.Size = new System.Drawing.Size(335, 82);
            this.gbBackup.TabIndex = 1;
            this.gbBackup.TabStop = false;
            this.gbBackup.Text = "Perform database backup";
            // 
            // btnBackUpNow
            // 
            this.btnBackUpNow.AutoSize = true;
            this.btnBackUpNow.Enabled = false;
            this.btnBackUpNow.Location = new System.Drawing.Point(11, 50);
            this.btnBackUpNow.Name = "btnBackUpNow";
            this.btnBackUpNow.Size = new System.Drawing.Size(80, 23);
            this.btnBackUpNow.TabIndex = 2;
            this.btnBackUpNow.Text = "Back up now";
            this.btnBackUpNow.UseVisualStyleBackColor = true;
            this.btnBackUpNow.Click += new System.EventHandler(this.btnBackUpNow_Click);
            // 
            // dtpStartTime
            // 
            this.dtpStartTime.CustomFormat = "h:mm tt";
            this.dtpStartTime.Enabled = false;
            this.dtpStartTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpStartTime.Location = new System.Drawing.Point(151, 22);
            this.dtpStartTime.Name = "dtpStartTime";
            this.dtpStartTime.ShowUpDown = true;
            this.dtpStartTime.Size = new System.Drawing.Size(172, 20);
            this.dtpStartTime.TabIndex = 1;
            this.dtpStartTime.ValueChanged += new System.EventHandler(this.dtpStartTime_ValueChanged);
            // 
            // lblStartTime
            // 
            this.lblStartTime.AutoSize = true;
            this.lblStartTime.Location = new System.Drawing.Point(8, 22);
            this.lblStartTime.Name = "lblStartTime";
            this.lblStartTime.Size = new System.Drawing.Size(117, 13);
            this.lblStartTime.TabIndex = 0;
            this.lblStartTime.Text = "Daily backup start time:";
            // 
            // gbLogIn
            // 
            this.gbLogIn.Controls.Add(this.btnLogIn);
            this.gbLogIn.Location = new System.Drawing.Point(12, 12);
            this.gbLogIn.Name = "gbLogIn";
            this.gbLogIn.Size = new System.Drawing.Size(335, 53);
            this.gbLogIn.TabIndex = 0;
            this.gbLogIn.TabStop = false;
            this.gbLogIn.Text = "Log in";
            // 
            // btnLogIn
            // 
            this.btnLogIn.Location = new System.Drawing.Point(11, 20);
            this.btnLogIn.Name = "btnLogIn";
            this.btnLogIn.Size = new System.Drawing.Size(75, 23);
            this.btnLogIn.TabIndex = 0;
            this.btnLogIn.Text = "Log in";
            this.btnLogIn.UseVisualStyleBackColor = true;
            this.btnLogIn.Click += new System.EventHandler(this.btnLogIn_Click);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer2.IsSplitterFixed = true;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.lblHeader);
            this.splitContainer2.Panel1Collapsed = true;
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.splitContainer3);
            this.splitContainer2.Size = new System.Drawing.Size(412, 629);
            this.splitContainer2.SplitterDistance = 37;
            this.splitContainer2.TabIndex = 0;
            // 
            // lblHeader
            // 
            this.lblHeader.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeader.Location = new System.Drawing.Point(117, 12);
            this.lblHeader.MinimumSize = new System.Drawing.Size(199, 13);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(199, 13);
            this.lblHeader.TabIndex = 0;
            this.lblHeader.Text = "Unclassified/For Official Use Only";
            this.lblHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblHeader.Visible = false;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.lvLog);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.btnPrint);
            this.splitContainer3.Size = new System.Drawing.Size(412, 629);
            this.splitContainer3.SplitterDistance = 551;
            this.splitContainer3.TabIndex = 0;
            // 
            // lvLog
            // 
            this.lvLog.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderDateTime});
            this.lvLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvLog.Location = new System.Drawing.Point(0, 0);
            this.lvLog.Name = "lvLog";
            this.lvLog.ShowItemToolTips = true;
            this.lvLog.Size = new System.Drawing.Size(412, 551);
            this.lvLog.TabIndex = 0;
            this.lvLog.UseCompatibleStateImageBehavior = false;
            this.lvLog.View = System.Windows.Forms.View.Details;
            // 
            // columnHeaderDateTime
            // 
            this.columnHeaderDateTime.Text = "Date/Time                  Message";
            this.columnHeaderDateTime.Width = 409;
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(18, 16);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(75, 23);
            this.btnPrint.TabIndex = 0;
            this.btnPrint.Text = "Print";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // statusStripMain
            // 
            this.statusStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelMsg});
            this.statusStripMain.Location = new System.Drawing.Point(0, 607);
            this.statusStripMain.Name = "statusStripMain";
            this.statusStripMain.Size = new System.Drawing.Size(769, 22);
            this.statusStripMain.TabIndex = 0;
            this.statusStripMain.Text = "statusStripMain";
            // 
            // toolStripStatusLabelMsg
            // 
            this.toolStripStatusLabelMsg.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.toolStripStatusLabelMsg.Name = "toolStripStatusLabelMsg";
            this.toolStripStatusLabelMsg.Size = new System.Drawing.Size(754, 17);
            this.toolStripStatusLabelMsg.Spring = true;
            this.toolStripStatusLabelMsg.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(772, 629);
            this.Controls.Add(this.statusStripMain);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Padding = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FuelsManager Backup Utility Configuration";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.LocationChanged += new System.EventHandler(this.MainForm_LocationChanged);
            this.contextMenuStripNotifyIcon.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.timerNotifyIcon)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.gbZip.ResumeLayout(false);
            this.gbZip.PerformLayout();
            this.gbAdditionalFiles.ResumeLayout(false);
            this.gbAdditionalFiles.PerformLayout();
            this.gbLogFile.ResumeLayout(false);
            this.gbLogFile.PerformLayout();
            this.gbBackup.ResumeLayout(false);
            this.gbBackup.PerformLayout();
            this.gbLogIn.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            this.splitContainer3.ResumeLayout(false);
            this.statusStripMain.ResumeLayout(false);
            this.statusStripMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifyIconBUC;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripNotifyIcon;
        private System.Windows.Forms.ToolStripMenuItem tsmiOpenBackupUtilityConfiguration;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem tsmiHelp;
        private System.Windows.Forms.ToolStripMenuItem tsmiAbout;


        private System.Timers.Timer timerNotifyIcon;
        private System.Windows.Forms.StatusStrip statusStripMain;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox gbLogIn;
        private System.Windows.Forms.Button btnLogIn;
        private System.Windows.Forms.GroupBox gbBackup;
        private System.Windows.Forms.Button btnBackUpNow;
        private System.Windows.Forms.DateTimePicker dtpStartTime;
        private System.Windows.Forms.Label lblStartTime;
        private System.Windows.Forms.GroupBox gbLogFile;
        private System.Windows.Forms.TextBox tbLogFileLocation;
        private System.Windows.Forms.Button btnBrowseLogLocation;
        private System.Windows.Forms.GroupBox gbAdditionalFiles;
        private System.Windows.Forms.Label lblAddtionalFilesLocations;
        private System.Windows.Forms.ListBox lbFilesLocations;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnBrowseFilesLocation;
        private System.Windows.Forms.GroupBox gbZip;
        private System.Windows.Forms.Button btnZipLocation;
        private System.Windows.Forms.TextBox tbZipFileLocation;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnHelp;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnViewLog;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.ListView lvLog;
        private System.Windows.Forms.ColumnHeader columnHeaderDateTime;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelMsg;
        private System.Windows.Forms.ToolTip toolTipMain;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem tsmiExit;
    }
}


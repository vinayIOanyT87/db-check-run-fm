namespace DispatchPrototype
{
    partial class DispatchContainerForm
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
			  // kill the scheduler thread
				if (KillEvent != null)
					KillEvent.Set();
				if (SchedulerMessageThread != null)
					SchedulerMessageThread.Join();
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
			  System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DispatchContainerForm));
			  System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			  System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
			  System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			  System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
			  System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
			  System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
			  System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
			  System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
			  System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
			  this.menuStrip = new System.Windows.Forms.MenuStrip();
			  this.fileMenu = new System.Windows.Forms.ToolStripMenuItem();
			  this.optionalTimesToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			  this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			  this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			  this.viewMenu = new System.Windows.Forms.ToolStripMenuItem();
			  this.toolBarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			  this.statusBarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			  this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			  this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
			  this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
			  this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
			  this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
			  this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripMenuItem();
			  this.toolStripMenuItem7 = new System.Windows.Forms.ToolStripMenuItem();
			  this.toolStripMenuItem8 = new System.Windows.Forms.ToolStripMenuItem();
			  this.toolStripMenuItem10 = new System.Windows.Forms.ToolStripMenuItem();
			  this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			  this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			  this.toolsMenu = new System.Windows.Forms.ToolStripMenuItem();
			  this.reportsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			  this.queryWriterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			  this.operationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			  this.requestRefuelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			  this.transientToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			  this.fastLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			  this.fastLogFillstandToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			  this.recirculationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			  this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
			  this.dispatchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			  this.standByToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			  this.flightLineStatusToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			  this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
			  this.relogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			  this.cancelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			  this.arrivalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			  this.startOfServiceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			  this.stopOfServiceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			  this.serviceCompletionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			  this.fillstandCompletionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			  this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
			  this.changeOfOperatorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			  this.totalAndAverageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			  this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
			  this.controllersLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			  this.toolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
			  this.exportToAccountingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			  this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			  this.evacuateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			  this.addInsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			  this.configurationToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			  this.helpMenu = new System.Windows.Forms.ToolStripMenuItem();
			  this.contentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			  this.indexToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			  this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
			  this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			  this.toolStrip = new System.Windows.Forms.ToolStrip();
			  this.helpToolStripButton = new System.Windows.Forms.ToolStripButton();
			  this.toolStripRequestRefuelButton = new System.Windows.Forms.ToolStripButton();
			  this.toolStripTransientButton = new System.Windows.Forms.ToolStripButton();
			  this.toolStripFastLogButton = new System.Windows.Forms.ToolStripButton();
			  this.toolStripFastLofFillstandButton = new System.Windows.Forms.ToolStripButton();
			  this.toolStripRelogButton = new System.Windows.Forms.ToolStripButton();
			  this.toolStripDispatchButton = new System.Windows.Forms.ToolStripButton();
			  this.toolStripControllersLogButton = new System.Windows.Forms.ToolStripButton();
			  this.toolStripStandbyButton = new System.Windows.Forms.ToolStripButton();
			  this.FlightLineButton = new System.Windows.Forms.ToolStripButton();
			  this.toolStripCancelButton = new System.Windows.Forms.ToolStripButton();
			  this.AverageAndTotalMenu = new System.Windows.Forms.ToolStripButton();
			  this.statusStrip = new System.Windows.Forms.StatusStrip();
			  this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
			  this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			  this.dataGridView1 = new System.Windows.Forms.DataGridView();
			  this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			  this.RequestType = new System.Windows.Forms.DataGridViewTextBoxColumn();
			  this.Requested = new System.Windows.Forms.DataGridViewTextBoxColumn();
			  this.Column8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			  this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			  this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			  this.AircraftID = new System.Windows.Forms.DataGridViewTextBoxColumn();
			  this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			  this.MDS = new System.Windows.Forms.DataGridViewTextBoxColumn();
			  this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			  this.Column9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			  this.Column10 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			  this.Column11 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			  this.Column12 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			  this.Grade = new System.Windows.Forms.DataGridViewTextBoxColumn();
			  this.Column14 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			  this.Column13 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			  this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			  this.Column16 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			  this.Column15 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			  this.RequestedBy = new System.Windows.Forms.DataGridViewTextBoxColumn();
			  this.Cancelled = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			  this.DODAAC = new System.Windows.Forms.DataGridViewTextBoxColumn();
			  this.SuppDODAAC = new System.Windows.Forms.DataGridViewTextBoxColumn();
			  this.BOS = new System.Windows.Forms.DataGridViewTextBoxColumn();
			  this.SignalCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
			  this.USECode = new System.Windows.Forms.DataGridViewTextBoxColumn();
			  this.FundCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
			  this.RPTTECAPC = new System.Windows.Forms.DataGridViewTextBoxColumn();
			  this.CardNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
			  this.RecirculationType = new System.Windows.Forms.DataGridViewTextBoxColumn();
			  this.REFID = new System.Windows.Forms.DataGridViewTextBoxColumn();
			  this.SerialNum = new System.Windows.Forms.DataGridViewTextBoxColumn();
			  this.IssPt = new System.Windows.Forms.DataGridViewTextBoxColumn();
			  this.IssPtNum = new System.Windows.Forms.DataGridViewTextBoxColumn();
			  this.Activity = new System.Windows.Forms.DataGridViewTextBoxColumn();
			  this.GrossGal = new System.Windows.Forms.DataGridViewTextBoxColumn();
			  this.FuelAdditiveFlag = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			  this.OnHandQuantity = new System.Windows.Forms.DataGridViewTextBoxColumn();
			  this.DifPress = new System.Windows.Forms.DataGridViewTextBoxColumn();
			  this.panel3 = new System.Windows.Forms.Panel();
			  this.StatusCombo = new System.Windows.Forms.ComboBox();
			  this.label1 = new System.Windows.Forms.Label();
			  this.label2 = new System.Windows.Forms.Label();
			  this.label3 = new System.Windows.Forms.Label();
			  this.BeginDatePicker = new System.Windows.Forms.DateTimePicker();
			  this.EndDatePicker = new System.Windows.Forms.DateTimePicker();
			  this.label4 = new System.Windows.Forms.Label();
			  this.RequestTypeCombo = new System.Windows.Forms.ComboBox();
			  this.panel1 = new System.Windows.Forms.Panel();
			  this.label5 = new System.Windows.Forms.Label();
			  this.vehicleComboBox = new System.Windows.Forms.ComboBox();
			  this.julianDateLabel = new System.Windows.Forms.Label();
			  this.julianDateTimer = new System.Windows.Forms.Timer(this.components);
			  this.helpProvider1 = new System.Windows.Forms.HelpProvider();
			  this.menuStrip.SuspendLayout();
			  this.toolStrip.SuspendLayout();
			  this.statusStrip.SuspendLayout();
			  ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
			  this.panel3.SuspendLayout();
			  this.panel1.SuspendLayout();
			  this.SuspendLayout();
			  // 
			  // menuStrip
			  // 
			  this.menuStrip.Font = new System.Drawing.Font("MS Reference Sans Serif", 8F);
			  this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileMenu,
            this.viewMenu,
            this.toolsMenu,
            this.operationToolStripMenuItem,
            this.addInsToolStripMenuItem,
            this.helpMenu});
			  this.menuStrip.Location = new System.Drawing.Point(0, 0);
			  this.menuStrip.Name = "menuStrip";
			  this.menuStrip.Size = new System.Drawing.Size(974, 24);
			  this.menuStrip.TabIndex = 0;
			  this.menuStrip.Text = "Operation";
			  // 
			  // fileMenu
			  // 
			  this.fileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionalTimesToolStripMenuItem1,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem});
			  this.fileMenu.ImageTransparentColor = System.Drawing.SystemColors.ActiveBorder;
			  this.fileMenu.Name = "fileMenu";
			  this.fileMenu.Size = new System.Drawing.Size(38, 20);
			  this.fileMenu.Text = "&File";
			  // 
			  // optionalTimesToolStripMenuItem1
			  // 
			  this.optionalTimesToolStripMenuItem1.Name = "optionalTimesToolStripMenuItem1";
			  this.optionalTimesToolStripMenuItem1.Size = new System.Drawing.Size(159, 22);
			  this.optionalTimesToolStripMenuItem1.Text = "Optional Times";
			  this.optionalTimesToolStripMenuItem1.Click += new System.EventHandler(this.optionalTimesToolStripMenuItem1_Click);
			  // 
			  // toolStripMenuItem1
			  // 
			  this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			  this.toolStripMenuItem1.Size = new System.Drawing.Size(156, 6);
			  // 
			  // exitToolStripMenuItem
			  // 
			  this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			  this.exitToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
			  this.exitToolStripMenuItem.Text = "E&xit";
			  this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolsStripMenuItem_Click);
			  // 
			  // viewMenu
			  // 
			  this.viewMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolBarToolStripMenuItem,
            this.statusBarToolStripMenuItem,
            this.toolStripSeparator1,
            this.toolStripMenuItem2,
            this.toolStripMenuItem3,
            this.toolStripMenuItem4,
            this.toolStripMenuItem5,
            this.toolStripMenuItem6,
            this.toolStripMenuItem7,
            this.toolStripMenuItem8,
            this.toolStripMenuItem10,
            this.toolStripSeparator2,
            this.refreshToolStripMenuItem});
			  this.viewMenu.Name = "viewMenu";
			  this.viewMenu.Size = new System.Drawing.Size(46, 20);
			  this.viewMenu.Text = "&View";
			  // 
			  // toolBarToolStripMenuItem
			  // 
			  this.toolBarToolStripMenuItem.Checked = true;
			  this.toolBarToolStripMenuItem.CheckOnClick = true;
			  this.toolBarToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			  this.toolBarToolStripMenuItem.Name = "toolBarToolStripMenuItem";
			  this.toolBarToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
			  this.toolBarToolStripMenuItem.Text = "&Toolbar";
			  this.toolBarToolStripMenuItem.Click += new System.EventHandler(this.ToolBarToolStripMenuItem_Click);
			  // 
			  // statusBarToolStripMenuItem
			  // 
			  this.statusBarToolStripMenuItem.Checked = true;
			  this.statusBarToolStripMenuItem.CheckOnClick = true;
			  this.statusBarToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			  this.statusBarToolStripMenuItem.Name = "statusBarToolStripMenuItem";
			  this.statusBarToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
			  this.statusBarToolStripMenuItem.Text = "&Status Bar";
			  this.statusBarToolStripMenuItem.Click += new System.EventHandler(this.StatusBarToolStripMenuItem_Click);
			  // 
			  // toolStripSeparator1
			  // 
			  this.toolStripSeparator1.Name = "toolStripSeparator1";
			  this.toolStripSeparator1.Size = new System.Drawing.Size(224, 6);
			  // 
			  // toolStripMenuItem2
			  // 
			  this.toolStripMenuItem2.Name = "toolStripMenuItem2";
			  this.toolStripMenuItem2.ShortcutKeys = System.Windows.Forms.Keys.F10;
			  this.toolStripMenuItem2.Size = new System.Drawing.Size(227, 22);
			  this.toolStripMenuItem2.Text = "Show All";
			  this.toolStripMenuItem2.Click += new System.EventHandler(this.ViewAll_Click);
			  // 
			  // toolStripMenuItem3
			  // 
			  this.toolStripMenuItem3.Name = "toolStripMenuItem3";
			  this.toolStripMenuItem3.ShortcutKeys = System.Windows.Forms.Keys.F11;
			  this.toolStripMenuItem3.Size = new System.Drawing.Size(227, 22);
			  this.toolStripMenuItem3.Text = "Show Requested";
			  this.toolStripMenuItem3.Click += new System.EventHandler(this.ViewRequested_Click);
			  // 
			  // toolStripMenuItem4
			  // 
			  this.toolStripMenuItem4.Name = "toolStripMenuItem4";
			  this.toolStripMenuItem4.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F3)));
			  this.toolStripMenuItem4.Size = new System.Drawing.Size(227, 22);
			  this.toolStripMenuItem4.Text = "Show Dispatched";
			  this.toolStripMenuItem4.Click += new System.EventHandler(this.ViewDispatched_Click);
			  // 
			  // toolStripMenuItem5
			  // 
			  this.toolStripMenuItem5.Name = "toolStripMenuItem5";
			  this.toolStripMenuItem5.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F7)));
			  this.toolStripMenuItem5.Size = new System.Drawing.Size(227, 22);
			  this.toolStripMenuItem5.Text = "Show Arrived";
			  this.toolStripMenuItem5.Click += new System.EventHandler(this.ViewArrived_Click);
			  // 
			  // toolStripMenuItem6
			  // 
			  this.toolStripMenuItem6.Name = "toolStripMenuItem6";
			  this.toolStripMenuItem6.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F10)));
			  this.toolStripMenuItem6.Size = new System.Drawing.Size(227, 22);
			  this.toolStripMenuItem6.Text = "Show Started";
			  this.toolStripMenuItem6.Click += new System.EventHandler(this.ViewStarted_Click);
			  // 
			  // toolStripMenuItem7
			  // 
			  this.toolStripMenuItem7.Name = "toolStripMenuItem7";
			  this.toolStripMenuItem7.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F4)));
			  this.toolStripMenuItem7.Size = new System.Drawing.Size(227, 22);
			  this.toolStripMenuItem7.Text = "Show Stopped";
			  this.toolStripMenuItem7.Click += new System.EventHandler(this.ViewStopped_Click);
			  // 
			  // toolStripMenuItem8
			  // 
			  this.toolStripMenuItem8.Name = "toolStripMenuItem8";
			  this.toolStripMenuItem8.ShortcutKeys = System.Windows.Forms.Keys.F12;
			  this.toolStripMenuItem8.Size = new System.Drawing.Size(227, 22);
			  this.toolStripMenuItem8.Text = "Show Completed";
			  this.toolStripMenuItem8.Click += new System.EventHandler(this.ViewCompleted_Click);
			  // 
			  // toolStripMenuItem10
			  // 
			  this.toolStripMenuItem10.Name = "toolStripMenuItem10";
			  this.toolStripMenuItem10.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F10)));
			  this.toolStripMenuItem10.Size = new System.Drawing.Size(227, 22);
			  this.toolStripMenuItem10.Text = "Show Cancelled";
			  this.toolStripMenuItem10.Click += new System.EventHandler(this.ViewCanceled_Click);
			  // 
			  // toolStripSeparator2
			  // 
			  this.toolStripSeparator2.Name = "toolStripSeparator2";
			  this.toolStripSeparator2.Size = new System.Drawing.Size(224, 6);
			  // 
			  // refreshToolStripMenuItem
			  // 
			  this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
			  this.refreshToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
			  this.refreshToolStripMenuItem.Text = "Refresh";
			  this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
			  // 
			  // toolsMenu
			  // 
			  this.toolsMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.reportsToolStripMenuItem,
            this.queryWriterToolStripMenuItem});
			  this.toolsMenu.Name = "toolsMenu";
			  this.toolsMenu.Size = new System.Drawing.Size(63, 20);
			  this.toolsMenu.Text = "&Reports";
			  // 
			  // reportsToolStripMenuItem
			  // 
			  this.reportsToolStripMenuItem.Name = "reportsToolStripMenuItem";
			  this.reportsToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
			  this.reportsToolStripMenuItem.Text = "&Reports";
			  this.reportsToolStripMenuItem.Click += new System.EventHandler(this.reportsToolStripMenuItem_Click);
			  // 
			  // queryWriterToolStripMenuItem
			  // 
			  this.queryWriterToolStripMenuItem.Name = "queryWriterToolStripMenuItem";
			  this.queryWriterToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
			  this.queryWriterToolStripMenuItem.Text = "&Query Writer";
			  this.queryWriterToolStripMenuItem.Click += new System.EventHandler(this.queryWriterToolStripMenuItem_Click);
			  // 
			  // operationToolStripMenuItem
			  // 
			  this.operationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.requestRefuelToolStripMenuItem,
            this.transientToolStripMenuItem,
            this.fastLogToolStripMenuItem,
            this.fastLogFillstandToolStripMenuItem,
            this.recirculationToolStripMenuItem,
            this.toolStripSeparator9,
            this.dispatchToolStripMenuItem,
            this.standByToolStripMenuItem,
            this.flightLineStatusToolStripMenuItem,
            this.toolStripSeparator6,
            this.relogToolStripMenuItem,
            this.cancelToolStripMenuItem,
            this.arrivalToolStripMenuItem,
            this.startOfServiceToolStripMenuItem,
            this.stopOfServiceToolStripMenuItem,
            this.serviceCompletionToolStripMenuItem,
            this.fillstandCompletionToolStripMenuItem,
            this.toolStripSeparator10,
            this.changeOfOperatorToolStripMenuItem,
            this.totalAndAverageToolStripMenuItem,
            this.toolStripSeparator12,
            this.controllersLogToolStripMenuItem,
            this.toolStripSeparator13,
            this.exportToAccountingToolStripMenuItem,
            this.toolStripSeparator3,
            this.evacuateToolStripMenuItem});
			  this.operationToolStripMenuItem.Name = "operationToolStripMenuItem";
			  this.operationToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
			  this.operationToolStripMenuItem.Size = new System.Drawing.Size(75, 20);
			  this.operationToolStripMenuItem.Text = "Operation";
			  this.operationToolStripMenuItem.DropDownOpening += new System.EventHandler(this.operationToolStripMenuItem_DropDownOpening);
			  this.operationToolStripMenuItem.Click += new System.EventHandler(this.operationToolStripMenuItem_Click);
			  // 
			  // requestRefuelToolStripMenuItem
			  // 
			  this.requestRefuelToolStripMenuItem.Name = "requestRefuelToolStripMenuItem";
			  this.requestRefuelToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F2;
			  this.requestRefuelToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
			  this.requestRefuelToolStripMenuItem.Text = "Request";
			  this.requestRefuelToolStripMenuItem.Click += new System.EventHandler(this.requestRefuelToolStripMenuItem_Click);
			  // 
			  // transientToolStripMenuItem
			  // 
			  this.transientToolStripMenuItem.Name = "transientToolStripMenuItem";
			  this.transientToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F4;
			  this.transientToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
			  this.transientToolStripMenuItem.Text = "Transient";
			  this.transientToolStripMenuItem.Click += new System.EventHandler(this.transientToolStripMenuItem_Click);
			  // 
			  // fastLogToolStripMenuItem
			  // 
			  this.fastLogToolStripMenuItem.Name = "fastLogToolStripMenuItem";
			  this.fastLogToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F9;
			  this.fastLogToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
			  this.fastLogToolStripMenuItem.Text = "Fast Log";
			  this.fastLogToolStripMenuItem.Click += new System.EventHandler(this.fastLogToolStripMenuItem_Click);
			  // 
			  // fastLogFillstandToolStripMenuItem
			  // 
			  this.fastLogFillstandToolStripMenuItem.Name = "fastLogFillstandToolStripMenuItem";
			  this.fastLogFillstandToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F9)));
			  this.fastLogFillstandToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
			  this.fastLogFillstandToolStripMenuItem.Text = "Fast Log Fillstand";
			  this.fastLogFillstandToolStripMenuItem.Click += new System.EventHandler(this.fastLogFillstandToolStripMenuItem_Click);
			  // 
			  // recirculationToolStripMenuItem
			  // 
			  this.recirculationToolStripMenuItem.Name = "recirculationToolStripMenuItem";
			  this.recirculationToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F3;
			  this.recirculationToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
			  this.recirculationToolStripMenuItem.Text = "Recirculation";
			  this.recirculationToolStripMenuItem.Click += new System.EventHandler(this.recirculationToolStripMenuItem_Click);
			  // 
			  // toolStripSeparator9
			  // 
			  this.toolStripSeparator9.Name = "toolStripSeparator9";
			  this.toolStripSeparator9.Size = new System.Drawing.Size(241, 6);
			  // 
			  // dispatchToolStripMenuItem
			  // 
			  this.dispatchToolStripMenuItem.Name = "dispatchToolStripMenuItem";
			  this.dispatchToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F6;
			  this.dispatchToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
			  this.dispatchToolStripMenuItem.Text = "Dispatch";
			  this.dispatchToolStripMenuItem.Click += new System.EventHandler(this.dispatchToolStripMenuItem_Click);
			  // 
			  // standByToolStripMenuItem
			  // 
			  this.standByToolStripMenuItem.Name = "standByToolStripMenuItem";
			  this.standByToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F5)));
			  this.standByToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
			  this.standByToolStripMenuItem.Text = "Stand By";
			  this.standByToolStripMenuItem.Click += new System.EventHandler(this.standByToolStripMenuItem_Click);
			  // 
			  // flightLineStatusToolStripMenuItem
			  // 
			  this.flightLineStatusToolStripMenuItem.Name = "flightLineStatusToolStripMenuItem";
			  this.flightLineStatusToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F3)));
			  this.flightLineStatusToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
			  this.flightLineStatusToolStripMenuItem.Text = "Flight Line Status";
			  this.flightLineStatusToolStripMenuItem.Click += new System.EventHandler(this.flightLineStatusToolStripMenuItem_Click);
			  // 
			  // toolStripSeparator6
			  // 
			  this.toolStripSeparator6.Name = "toolStripSeparator6";
			  this.toolStripSeparator6.Size = new System.Drawing.Size(241, 6);
			  // 
			  // relogToolStripMenuItem
			  // 
			  this.relogToolStripMenuItem.Name = "relogToolStripMenuItem";
			  this.relogToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F5)));
			  this.relogToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
			  this.relogToolStripMenuItem.Text = "Relog";
			  this.relogToolStripMenuItem.Click += new System.EventHandler(this.relogToolStripMenuItem_Click);
			  // 
			  // cancelToolStripMenuItem
			  // 
			  this.cancelToolStripMenuItem.Name = "cancelToolStripMenuItem";
			  this.cancelToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F4)));
			  this.cancelToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
			  this.cancelToolStripMenuItem.Text = "Cancel";
			  this.cancelToolStripMenuItem.Click += new System.EventHandler(this.cancelToolStripMenuItem_Click);
			  // 
			  // arrivalToolStripMenuItem
			  // 
			  this.arrivalToolStripMenuItem.Name = "arrivalToolStripMenuItem";
			  this.arrivalToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F7;
			  this.arrivalToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
			  this.arrivalToolStripMenuItem.Text = "Arrival";
			  this.arrivalToolStripMenuItem.Click += new System.EventHandler(this.arrivalToolStripMenuItem_Click);
			  // 
			  // startOfServiceToolStripMenuItem
			  // 
			  this.startOfServiceToolStripMenuItem.Name = "startOfServiceToolStripMenuItem";
			  this.startOfServiceToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F7)));
			  this.startOfServiceToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
			  this.startOfServiceToolStripMenuItem.Text = "Start of Service";
			  this.startOfServiceToolStripMenuItem.Click += new System.EventHandler(this.startOfServiceToolStripMenuItem_Click);
			  // 
			  // stopOfServiceToolStripMenuItem
			  // 
			  this.stopOfServiceToolStripMenuItem.Name = "stopOfServiceToolStripMenuItem";
			  this.stopOfServiceToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F8)));
			  this.stopOfServiceToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
			  this.stopOfServiceToolStripMenuItem.Text = "Stop of Service";
			  this.stopOfServiceToolStripMenuItem.Click += new System.EventHandler(this.stToolStripMenuItem_Click);
			  // 
			  // serviceCompletionToolStripMenuItem
			  // 
			  this.serviceCompletionToolStripMenuItem.Name = "serviceCompletionToolStripMenuItem";
			  this.serviceCompletionToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F8;
			  this.serviceCompletionToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
			  this.serviceCompletionToolStripMenuItem.Text = "Service Completion";
			  this.serviceCompletionToolStripMenuItem.Click += new System.EventHandler(this.serviceCompletionToolStripMenuItem_Click);
			  // 
			  // fillstandCompletionToolStripMenuItem
			  // 
			  this.fillstandCompletionToolStripMenuItem.Name = "fillstandCompletionToolStripMenuItem";
			  this.fillstandCompletionToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F8)));
			  this.fillstandCompletionToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
			  this.fillstandCompletionToolStripMenuItem.Text = "Fillstand Completion";
			  this.fillstandCompletionToolStripMenuItem.Click += new System.EventHandler(this.fillstandCompletionToolStripMenuItem_Click);
			  // 
			  // toolStripSeparator10
			  // 
			  this.toolStripSeparator10.Name = "toolStripSeparator10";
			  this.toolStripSeparator10.Size = new System.Drawing.Size(241, 6);
			  // 
			  // changeOfOperatorToolStripMenuItem
			  // 
			  this.changeOfOperatorToolStripMenuItem.Name = "changeOfOperatorToolStripMenuItem";
			  this.changeOfOperatorToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
			  this.changeOfOperatorToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
			  this.changeOfOperatorToolStripMenuItem.Text = "Change Operator Status";
			  this.changeOfOperatorToolStripMenuItem.Click += new System.EventHandler(this.changeOfOperatorToolStripMenuItem_Click);
			  // 
			  // totalAndAverageToolStripMenuItem
			  // 
			  this.totalAndAverageToolStripMenuItem.Name = "totalAndAverageToolStripMenuItem";
			  this.totalAndAverageToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
			  this.totalAndAverageToolStripMenuItem.Text = "Total and Average";
			  this.totalAndAverageToolStripMenuItem.Click += new System.EventHandler(this.totalAndAverageToolStripMenuItem_Click);
			  // 
			  // toolStripSeparator12
			  // 
			  this.toolStripSeparator12.Name = "toolStripSeparator12";
			  this.toolStripSeparator12.Size = new System.Drawing.Size(241, 6);
			  // 
			  // controllersLogToolStripMenuItem
			  // 
			  this.controllersLogToolStripMenuItem.Name = "controllersLogToolStripMenuItem";
			  this.controllersLogToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F9)));
			  this.controllersLogToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
			  this.controllersLogToolStripMenuItem.Text = "Controllers Log";
			  this.controllersLogToolStripMenuItem.Click += new System.EventHandler(this.OnClickViewControlLog);
			  // 
			  // toolStripSeparator13
			  // 
			  this.toolStripSeparator13.Name = "toolStripSeparator13";
			  this.toolStripSeparator13.Size = new System.Drawing.Size(241, 6);
			  // 
			  // exportToAccountingToolStripMenuItem
			  // 
			  this.exportToAccountingToolStripMenuItem.Name = "exportToAccountingToolStripMenuItem";
			  this.exportToAccountingToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
			  this.exportToAccountingToolStripMenuItem.Text = "Export to Accounting";
			  this.exportToAccountingToolStripMenuItem.Click += new System.EventHandler(this.exportToAccountingToolStripMenuItem_Click);
			  // 
			  // toolStripSeparator3
			  // 
			  this.toolStripSeparator3.Name = "toolStripSeparator3";
			  this.toolStripSeparator3.Size = new System.Drawing.Size(241, 6);
			  // 
			  // evacuateToolStripMenuItem
			  // 
			  this.evacuateToolStripMenuItem.Name = "evacuateToolStripMenuItem";
			  this.evacuateToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
			  this.evacuateToolStripMenuItem.Text = "Evacuate";
			  this.evacuateToolStripMenuItem.Click += new System.EventHandler(this.evacuateToolStripMenuItemOnClick);
			  // 
			  // addInsToolStripMenuItem
			  // 
			  this.addInsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.configurationToolStripMenuItem1});
			  this.addInsToolStripMenuItem.Name = "addInsToolStripMenuItem";
			  this.addInsToolStripMenuItem.Size = new System.Drawing.Size(64, 20);
			  this.addInsToolStripMenuItem.Text = "Add-Ins";
			  // 
			  // configurationToolStripMenuItem1
			  // 
			  this.configurationToolStripMenuItem1.Name = "configurationToolStripMenuItem1";
			  this.configurationToolStripMenuItem1.Size = new System.Drawing.Size(151, 22);
			  this.configurationToolStripMenuItem1.Text = "Configuration";
			  this.configurationToolStripMenuItem1.Click += new System.EventHandler(this.OnAddInsConfigurationClicked);
			  // 
			  // helpMenu
			  // 
			  this.helpMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.contentsToolStripMenuItem,
            this.indexToolStripMenuItem,
            this.toolStripSeparator8,
            this.aboutToolStripMenuItem});
			  this.helpMenu.Name = "helpMenu";
			  this.helpMenu.Size = new System.Drawing.Size(44, 20);
			  this.helpMenu.Text = "&Help";
			  // 
			  // contentsToolStripMenuItem
			  // 
			  this.contentsToolStripMenuItem.Name = "contentsToolStripMenuItem";
			  this.contentsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F1)));
			  this.contentsToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
			  this.contentsToolStripMenuItem.Text = "&Contents";
			  this.contentsToolStripMenuItem.Click += new System.EventHandler(this.contentsToolStripMenuItem_Click);
			  // 
			  // indexToolStripMenuItem
			  // 
			  this.indexToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("indexToolStripMenuItem.Image")));
			  this.indexToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
			  this.indexToolStripMenuItem.Name = "indexToolStripMenuItem";
			  this.indexToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F1;
			  this.indexToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
			  this.indexToolStripMenuItem.Text = "&Index";
			  this.indexToolStripMenuItem.Click += new System.EventHandler(this.indexToolStripMenuItem_Click);
			  // 
			  // toolStripSeparator8
			  // 
			  this.toolStripSeparator8.Name = "toolStripSeparator8";
			  this.toolStripSeparator8.Size = new System.Drawing.Size(239, 6);
			  // 
			  // aboutToolStripMenuItem
			  // 
			  this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
			  this.aboutToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
			  this.aboutToolStripMenuItem.Text = "&About FuelsManager Dispatch";
			  this.aboutToolStripMenuItem.Click += new System.EventHandler(this.HelpAboutClicked);
			  // 
			  // toolStrip
			  // 
			  this.toolStrip.Font = new System.Drawing.Font("MS Reference Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			  this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpToolStripButton,
            this.toolStripRequestRefuelButton,
            this.toolStripTransientButton,
            this.toolStripFastLogButton,
            this.toolStripFastLofFillstandButton,
            this.toolStripRelogButton,
            this.toolStripDispatchButton,
            this.toolStripControllersLogButton,
            this.toolStripStandbyButton,
            this.FlightLineButton,
            this.toolStripCancelButton,
            this.AverageAndTotalMenu});
			  this.toolStrip.Location = new System.Drawing.Point(0, 24);
			  this.toolStrip.Name = "toolStrip";
			  this.toolStrip.Size = new System.Drawing.Size(974, 25);
			  this.toolStrip.TabIndex = 1;
			  this.toolStrip.Text = "ToolStrip";
			  this.toolStrip.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.toolStrip_ItemClicked);
			  // 
			  // helpToolStripButton
			  // 
			  this.helpToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("helpToolStripButton.Image")));
			  this.helpToolStripButton.ImageTransparentColor = System.Drawing.Color.Black;
			  this.helpToolStripButton.Name = "helpToolStripButton";
			  this.helpToolStripButton.Size = new System.Drawing.Size(52, 22);
			  this.helpToolStripButton.Text = "Help";
			  this.helpToolStripButton.Click += new System.EventHandler(this.indexToolStripMenuItem_Click);
			  // 
			  // toolStripRequestRefuelButton
			  // 
			  this.toolStripRequestRefuelButton.Image = ((System.Drawing.Image)(resources.GetObject("toolStripRequestRefuelButton.Image")));
			  this.toolStripRequestRefuelButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			  this.toolStripRequestRefuelButton.Name = "toolStripRequestRefuelButton";
			  this.toolStripRequestRefuelButton.Size = new System.Drawing.Size(73, 22);
			  this.toolStripRequestRefuelButton.Text = "Request";
			  this.toolStripRequestRefuelButton.ToolTipText = "Request F2";
			  this.toolStripRequestRefuelButton.Click += new System.EventHandler(this.requestRefuelToolStripMenuItem_Click);
			  // 
			  // toolStripTransientButton
			  // 
			  this.toolStripTransientButton.Image = ((System.Drawing.Image)(resources.GetObject("toolStripTransientButton.Image")));
			  this.toolStripTransientButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			  this.toolStripTransientButton.Name = "toolStripTransientButton";
			  this.toolStripTransientButton.Size = new System.Drawing.Size(80, 22);
			  this.toolStripTransientButton.Text = "Transient";
			  this.toolStripTransientButton.ToolTipText = "Transient F4";
			  this.toolStripTransientButton.Click += new System.EventHandler(this.transientToolStripMenuItem_Click);
			  // 
			  // toolStripFastLogButton
			  // 
			  this.toolStripFastLogButton.Image = ((System.Drawing.Image)(resources.GetObject("toolStripFastLogButton.Image")));
			  this.toolStripFastLogButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			  this.toolStripFastLogButton.Name = "toolStripFastLogButton";
			  this.toolStripFastLogButton.Size = new System.Drawing.Size(74, 22);
			  this.toolStripFastLogButton.Text = "Fast Log";
			  this.toolStripFastLogButton.ToolTipText = "Fast Log F9";
			  this.toolStripFastLogButton.Click += new System.EventHandler(this.fastLogToolStripMenuItem_Click);
			  // 
			  // toolStripFastLofFillstandButton
			  // 
			  this.toolStripFastLofFillstandButton.Image = ((System.Drawing.Image)(resources.GetObject("toolStripFastLofFillstandButton.Image")));
			  this.toolStripFastLofFillstandButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			  this.toolStripFastLofFillstandButton.Name = "toolStripFastLofFillstandButton";
			  this.toolStripFastLofFillstandButton.Size = new System.Drawing.Size(124, 22);
			  this.toolStripFastLofFillstandButton.Text = "Fast Log Fillstand";
			  this.toolStripFastLofFillstandButton.ToolTipText = "Fast Log Fillstand Shift+F9";
			  this.toolStripFastLofFillstandButton.Click += new System.EventHandler(this.fastLogFillstandToolStripMenuItem_Click);
			  // 
			  // toolStripRelogButton
			  // 
			  this.toolStripRelogButton.Image = ((System.Drawing.Image)(resources.GetObject("toolStripRelogButton.Image")));
			  this.toolStripRelogButton.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			  this.toolStripRelogButton.Name = "toolStripRelogButton";
			  this.toolStripRelogButton.Size = new System.Drawing.Size(59, 22);
			  this.toolStripRelogButton.Text = "Relog";
			  this.toolStripRelogButton.ToolTipText = "Relog Ctrl+F5";
			  this.toolStripRelogButton.Click += new System.EventHandler(this.relogToolStripMenuItem_Click);
			  // 
			  // toolStripDispatchButton
			  // 
			  this.toolStripDispatchButton.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDispatchButton.Image")));
			  this.toolStripDispatchButton.ImageTransparentColor = System.Drawing.Color.Fuchsia;
			  this.toolStripDispatchButton.Name = "toolStripDispatchButton";
			  this.toolStripDispatchButton.Size = new System.Drawing.Size(76, 22);
			  this.toolStripDispatchButton.Text = "Dispatch";
			  this.toolStripDispatchButton.ToolTipText = "Dispatch F6";
			  this.toolStripDispatchButton.Click += new System.EventHandler(this.dispatchToolStripMenuItem_Click);
			  // 
			  // toolStripControllersLogButton
			  // 
			  this.toolStripControllersLogButton.Image = ((System.Drawing.Image)(resources.GetObject("toolStripControllersLogButton.Image")));
			  this.toolStripControllersLogButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			  this.toolStripControllersLogButton.Name = "toolStripControllersLogButton";
			  this.toolStripControllersLogButton.Size = new System.Drawing.Size(114, 22);
			  this.toolStripControllersLogButton.Text = "Controllers Log";
			  this.toolStripControllersLogButton.ToolTipText = "Controllers Log Ctrl+F9";
			  this.toolStripControllersLogButton.Click += new System.EventHandler(this.OnClickViewControlLog);
			  // 
			  // toolStripStandbyButton
			  // 
			  this.toolStripStandbyButton.Image = ((System.Drawing.Image)(resources.GetObject("toolStripStandbyButton.Image")));
			  this.toolStripStandbyButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			  this.toolStripStandbyButton.Name = "toolStripStandbyButton";
			  this.toolStripStandbyButton.Size = new System.Drawing.Size(74, 22);
			  this.toolStripStandbyButton.Text = "Standby";
			  this.toolStripStandbyButton.Click += new System.EventHandler(this.standByToolStripMenuItem_Click);
			  // 
			  // FlightLineButton
			  // 
			  this.FlightLineButton.Image = ((System.Drawing.Image)(resources.GetObject("FlightLineButton.Image")));
			  this.FlightLineButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			  this.FlightLineButton.Name = "FlightLineButton";
			  this.FlightLineButton.Size = new System.Drawing.Size(84, 22);
			  this.FlightLineButton.Text = "Flight Line";
			  this.FlightLineButton.ToolTipText = "Flight Line (CTRL+F3)";
			  this.FlightLineButton.Click += new System.EventHandler(this.FlightLineButton_Click);
			  // 
			  // toolStripCancelButton
			  // 
			  this.toolStripCancelButton.Image = ((System.Drawing.Image)(resources.GetObject("toolStripCancelButton.Image")));
			  this.toolStripCancelButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			  this.toolStripCancelButton.Name = "toolStripCancelButton";
			  this.toolStripCancelButton.Size = new System.Drawing.Size(66, 22);
			  this.toolStripCancelButton.Text = "Cancel";
			  this.toolStripCancelButton.ToolTipText = "Cancel Ctrl+F4";
			  this.toolStripCancelButton.Click += new System.EventHandler(this.cancelToolStripMenuItem_Click);
			  // 
			  // AverageAndTotalMenu
			  // 
			  this.AverageAndTotalMenu.Image = ((System.Drawing.Image)(resources.GetObject("AverageAndTotalMenu.Image")));
			  this.AverageAndTotalMenu.ImageTransparentColor = System.Drawing.Color.Magenta;
			  this.AverageAndTotalMenu.Name = "AverageAndTotalMenu";
			  this.AverageAndTotalMenu.Size = new System.Drawing.Size(132, 20);
			  this.AverageAndTotalMenu.Text = "Total and Average";
			  this.AverageAndTotalMenu.Click += new System.EventHandler(this.totalAndAverageToolStripMenuItem_Click);
			  // 
			  // statusStrip
			  // 
			  this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel});
			  this.statusStrip.Location = new System.Drawing.Point(0, 651);
			  this.statusStrip.Name = "statusStrip";
			  this.statusStrip.Size = new System.Drawing.Size(974, 22);
			  this.statusStrip.TabIndex = 2;
			  this.statusStrip.Text = "StatusStrip";
			  // 
			  // toolStripStatusLabel
			  // 
			  this.toolStripStatusLabel.Name = "toolStripStatusLabel";
			  this.toolStripStatusLabel.Size = new System.Drawing.Size(38, 17);
			  this.toolStripStatusLabel.Text = "Status";
			  // 
			  // dataGridView1
			  // 
			  this.dataGridView1.AllowUserToAddRows = false;
			  this.dataGridView1.AllowUserToDeleteRows = false;
			  this.dataGridView1.AllowUserToOrderColumns = true;
			  this.dataGridView1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
			  this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			  dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			  dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
			  dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			  dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
			  dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			  dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			  dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			  this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			  this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
			  this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.RequestType,
            this.Requested,
            this.Column8,
            this.Column3,
            this.Column4,
            this.AircraftID,
            this.Column5,
            this.MDS,
            this.Column6,
            this.Column9,
            this.Column10,
            this.Column11,
            this.Column12,
            this.Grade,
            this.Column14,
            this.Column13,
            this.Column2,
            this.Column16,
            this.Column15,
            this.RequestedBy,
            this.Cancelled,
            this.DODAAC,
            this.SuppDODAAC,
            this.BOS,
            this.SignalCode,
            this.USECode,
            this.FundCode,
            this.RPTTECAPC,
            this.CardNumber,
            this.RecirculationType,
            this.REFID,
            this.SerialNum,
            this.IssPt,
            this.IssPtNum,
            this.Activity,
            this.GrossGal,
            this.FuelAdditiveFlag,
            this.OnHandQuantity,
            this.DifPress});
			  dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			  dataGridViewCellStyle9.BackColor = System.Drawing.SystemColors.Window;
			  dataGridViewCellStyle9.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.8F);
			  dataGridViewCellStyle9.ForeColor = System.Drawing.SystemColors.ControlText;
			  dataGridViewCellStyle9.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			  dataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			  dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			  this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle9;
			  this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
			  this.dataGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
			  this.dataGridView1.Location = new System.Drawing.Point(0, 0);
			  this.dataGridView1.Margin = new System.Windows.Forms.Padding(2);
			  this.dataGridView1.Name = "dataGridView1";
			  this.dataGridView1.ReadOnly = true;
			  this.dataGridView1.RowHeadersWidth = 50;
			  this.dataGridView1.RowTemplate.Height = 24;
			  this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			  this.dataGridView1.ShowEditingIcon = false;
			  this.dataGridView1.Size = new System.Drawing.Size(974, 573);
			  this.dataGridView1.TabIndex = 5;
			  this.dataGridView1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.dataGridView1_MouseClick);
			  this.dataGridView1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridView1_OnKeyDown);
			  // 
			  // Column1
			  // 
			  this.Column1.DataPropertyName = "TransactionStatus";
			  this.Column1.HeaderText = "Status";
			  this.Column1.Name = "Column1";
			  this.Column1.ReadOnly = true;
			  this.Column1.Width = 62;
			  // 
			  // RequestType
			  // 
			  this.RequestType.DataPropertyName = "AliasName";
			  this.RequestType.HeaderText = "Request Type";
			  this.RequestType.Name = "RequestType";
			  this.RequestType.ReadOnly = true;
			  this.RequestType.Width = 99;
			  // 
			  // Requested
			  // 
			  this.Requested.DataPropertyName = "RequestedDateTime";
			  dataGridViewCellStyle2.Format = "HH:mm";
			  this.Requested.DefaultCellStyle = dataGridViewCellStyle2;
			  this.Requested.HeaderText = "Requested";
			  this.Requested.Name = "Requested";
			  this.Requested.ReadOnly = true;
			  this.Requested.Width = 84;
			  // 
			  // Column8
			  // 
			  this.Column8.DataPropertyName = "DispatchedDateTime";
			  dataGridViewCellStyle3.Format = "HH:mm";
			  this.Column8.DefaultCellStyle = dataGridViewCellStyle3;
			  this.Column8.HeaderText = "Dispatched";
			  this.Column8.Name = "Column8";
			  this.Column8.ReadOnly = true;
			  this.Column8.Width = 86;
			  // 
			  // Column3
			  // 
			  this.Column3.DataPropertyName = "OperatorName";
			  this.Column3.HeaderText = "Operator";
			  this.Column3.Name = "Column3";
			  this.Column3.ReadOnly = true;
			  this.Column3.Width = 73;
			  // 
			  // Column4
			  // 
			  this.Column4.DataPropertyName = "VehicleID";
			  this.Column4.HeaderText = "Vehicle ID";
			  this.Column4.Name = "Column4";
			  this.Column4.ReadOnly = true;
			  this.Column4.Width = 81;
			  // 
			  // AircraftID
			  // 
			  this.AircraftID.DataPropertyName = "AircraftID";
			  this.AircraftID.HeaderText = "AircraftID";
			  this.AircraftID.Name = "AircraftID";
			  this.AircraftID.ReadOnly = true;
			  this.AircraftID.Width = 76;
			  // 
			  // Column5
			  // 
			  this.Column5.DataPropertyName = "UserData7";
			  this.Column5.HeaderText = "Location";
			  this.Column5.Name = "Column5";
			  this.Column5.ReadOnly = true;
			  this.Column5.Width = 73;
			  // 
			  // MDS
			  // 
			  this.MDS.DataPropertyName = "Model";
			  this.MDS.HeaderText = "MDS";
			  this.MDS.Name = "MDS";
			  this.MDS.ReadOnly = true;
			  this.MDS.Width = 56;
			  // 
			  // Column6
			  // 
			  this.Column6.DataPropertyName = "GrossQuantity";
			  this.Column6.HeaderText = "Quantity";
			  this.Column6.Name = "Column6";
			  this.Column6.ReadOnly = true;
			  this.Column6.Width = 71;
			  // 
			  // Column9
			  // 
			  this.Column9.DataPropertyName = "TimeIn";
			  dataGridViewCellStyle4.Format = "HH:mm";
			  dataGridViewCellStyle4.NullValue = null;
			  this.Column9.DefaultCellStyle = dataGridViewCellStyle4;
			  this.Column9.HeaderText = "Arrival";
			  this.Column9.Name = "Column9";
			  this.Column9.ReadOnly = true;
			  this.Column9.Width = 61;
			  // 
			  // Column10
			  // 
			  this.Column10.DataPropertyName = "FST";
			  dataGridViewCellStyle5.Format = "HH:mm";
			  this.Column10.DefaultCellStyle = dataGridViewCellStyle5;
			  this.Column10.HeaderText = "Started";
			  this.Column10.Name = "Column10";
			  this.Column10.ReadOnly = true;
			  this.Column10.Width = 66;
			  // 
			  // Column11
			  // 
			  this.Column11.DataPropertyName = "TimeEnd";
			  dataGridViewCellStyle6.Format = "HH:mm";
			  this.Column11.DefaultCellStyle = dataGridViewCellStyle6;
			  this.Column11.HeaderText = "Stopped";
			  this.Column11.Name = "Column11";
			  this.Column11.ReadOnly = true;
			  this.Column11.Width = 72;
			  // 
			  // Column12
			  // 
			  this.Column12.DataPropertyName = "TimeOut";
			  dataGridViewCellStyle7.Format = "HH:mm";
			  this.Column12.DefaultCellStyle = dataGridViewCellStyle7;
			  this.Column12.HeaderText = "Departed";
			  this.Column12.Name = "Column12";
			  this.Column12.ReadOnly = true;
			  this.Column12.Width = 76;
			  // 
			  // Grade
			  // 
			  this.Grade.DataPropertyName = "ProductID";
			  this.Grade.HeaderText = "Grade";
			  this.Grade.Name = "Grade";
			  this.Grade.ReadOnly = true;
			  this.Grade.Width = 61;
			  // 
			  // Column14
			  // 
			  this.Column14.DataPropertyName = "ResponseTime";
			  this.Column14.HeaderText = "Response Time";
			  this.Column14.Name = "Column14";
			  this.Column14.ReadOnly = true;
			  this.Column14.Width = 106;
			  // 
			  // Column13
			  // 
			  this.Column13.DataPropertyName = "Notes";
			  this.Column13.HeaderText = "Memo";
			  this.Column13.Name = "Column13";
			  this.Column13.ReadOnly = true;
			  this.Column13.Width = 61;
			  // 
			  // Column2
			  // 
			  this.Column2.DataPropertyName = "RequestedDateTime";
			  dataGridViewCellStyle8.Format = "d";
			  dataGridViewCellStyle8.NullValue = null;
			  this.Column2.DefaultCellStyle = dataGridViewCellStyle8;
			  this.Column2.HeaderText = "Date";
			  this.Column2.MinimumWidth = 4;
			  this.Column2.Name = "Column2";
			  this.Column2.ReadOnly = true;
			  this.Column2.Width = 55;
			  // 
			  // Column16
			  // 
			  this.Column16.DataPropertyName = "Variance";
			  this.Column16.HeaderText = "Variance";
			  this.Column16.Name = "Column16";
			  this.Column16.ReadOnly = true;
			  this.Column16.Width = 74;
			  // 
			  // Column15
			  // 
			  this.Column15.DataPropertyName = "FuelTime";
			  this.Column15.HeaderText = "Fuel Time";
			  this.Column15.Name = "Column15";
			  this.Column15.ReadOnly = true;
			  this.Column15.Width = 78;
			  // 
			  // RequestedBy
			  // 
			  this.RequestedBy.DataPropertyName = "ContactSurname";
			  this.RequestedBy.HeaderText = "RequestedBy";
			  this.RequestedBy.Name = "RequestedBy";
			  this.RequestedBy.ReadOnly = true;
			  this.RequestedBy.Width = 96;
			  // 
			  // Cancelled
			  // 
			  this.Cancelled.DataPropertyName = "TransactionStatusCancelled";
			  this.Cancelled.HeaderText = "Cancelled";
			  this.Cancelled.Name = "Cancelled";
			  this.Cancelled.ReadOnly = true;
			  this.Cancelled.Width = 60;
			  // 
			  // DODAAC
			  // 
			  this.DODAAC.DataPropertyName = "ShipToID";
			  this.DODAAC.HeaderText = "DODAAC";
			  this.DODAAC.Name = "DODAAC";
			  this.DODAAC.ReadOnly = true;
			  this.DODAAC.Width = 77;
			  // 
			  // SuppDODAAC
			  // 
			  this.SuppDODAAC.DataPropertyName = "BillToID";
			  this.SuppDODAAC.HeaderText = "Supp DODAAC";
			  this.SuppDODAAC.Name = "SuppDODAAC";
			  this.SuppDODAAC.ReadOnly = true;
			  this.SuppDODAAC.Width = 105;
			  // 
			  // BOS
			  // 
			  this.BOS.DataPropertyName = "UserData19";
			  this.BOS.HeaderText = "BOS";
			  this.BOS.Name = "BOS";
			  this.BOS.ReadOnly = true;
			  this.BOS.Width = 54;
			  // 
			  // SignalCode
			  // 
			  this.SignalCode.DataPropertyName = "UserData20";
			  this.SignalCode.HeaderText = "Signal Code";
			  this.SignalCode.Name = "SignalCode";
			  this.SignalCode.ReadOnly = true;
			  this.SignalCode.Width = 89;
			  // 
			  // USECode
			  // 
			  this.USECode.DataPropertyName = "UserData21";
			  this.USECode.HeaderText = "USE Code";
			  this.USECode.Name = "USECode";
			  this.USECode.ReadOnly = true;
			  this.USECode.Width = 82;
			  // 
			  // FundCode
			  // 
			  this.FundCode.DataPropertyName = "UserData5";
			  this.FundCode.HeaderText = "Fund Code";
			  this.FundCode.Name = "FundCode";
			  this.FundCode.ReadOnly = true;
			  this.FundCode.Width = 84;
			  // 
			  // RPTTECAPC
			  // 
			  this.RPTTECAPC.DataPropertyName = "UserData3";
			  this.RPTTECAPC.HeaderText = "RPT/TEC/APC";
			  this.RPTTECAPC.Name = "RPTTECAPC";
			  this.RPTTECAPC.ReadOnly = true;
			  this.RPTTECAPC.Width = 106;
			  // 
			  // CardNumber
			  // 
			  this.CardNumber.DataPropertyName = "CardNumber";
			  this.CardNumber.HeaderText = "Card Number";
			  this.CardNumber.Name = "CardNumber";
			  this.CardNumber.ReadOnly = true;
			  this.CardNumber.Width = 94;
			  // 
			  // RecirculationType
			  // 
			  this.RecirculationType.DataPropertyName = "UserData2";
			  this.RecirculationType.HeaderText = "Recirculation Type";
			  this.RecirculationType.Name = "RecirculationType";
			  this.RecirculationType.ReadOnly = true;
			  this.RecirculationType.Width = 121;
			  // 
			  // REFID
			  // 
			  this.REFID.DataPropertyName = "SourceXREF";
			  this.REFID.HeaderText = "Ref ID";
			  this.REFID.Name = "REFID";
			  this.REFID.ReadOnly = true;
			  this.REFID.Width = 63;
			  // 
			  // SerialNum
			  // 
			  this.SerialNum.DataPropertyName = "UserData4";
			  this.SerialNum.HeaderText = "Serial Num";
			  this.SerialNum.Name = "SerialNum";
			  this.SerialNum.ReadOnly = true;
			  this.SerialNum.Width = 83;
			  // 
			  // IssPt
			  // 
			  this.IssPt.DataPropertyName = "IssuePoint";
			  this.IssPt.HeaderText = "Iss Pt";
			  this.IssPt.Name = "IssPt";
			  this.IssPt.ReadOnly = true;
			  this.IssPt.Width = 58;
			  // 
			  // IssPtNum
			  // 
			  this.IssPtNum.DataPropertyName = "IssuePointNumber";
			  this.IssPtNum.HeaderText = "IssPtNum";
			  this.IssPtNum.Name = "IssPtNum";
			  this.IssPtNum.ReadOnly = true;
			  this.IssPtNum.Width = 77;
			  // 
			  // Activity
			  // 
			  this.Activity.DataPropertyName = "FuelCardID";
			  this.Activity.HeaderText = "Activity";
			  this.Activity.Name = "Activity";
			  this.Activity.ReadOnly = true;
			  this.Activity.Width = 66;
			  // 
			  // GrossGal
			  // 
			  this.GrossGal.DataPropertyName = "Number03";
			  this.GrossGal.HeaderText = "Gross Gal";
			  this.GrossGal.Name = "GrossGal";
			  this.GrossGal.ReadOnly = true;
			  this.GrossGal.Width = 78;
			  // 
			  // FuelAdditiveFlag
			  // 
			  this.FuelAdditiveFlag.DataPropertyName = "Flag04";
			  this.FuelAdditiveFlag.HeaderText = "Fuel Additive Flag";
			  this.FuelAdditiveFlag.Name = "FuelAdditiveFlag";
			  this.FuelAdditiveFlag.ReadOnly = true;
			  this.FuelAdditiveFlag.Width = 97;
			  // 
			  // OnHandQuantity
			  // 
			  this.OnHandQuantity.DataPropertyName = "EquipmentVolume";
			  this.OnHandQuantity.HeaderText = "On Hand Qty";
			  this.OnHandQuantity.Name = "OnHandQuantity";
			  this.OnHandQuantity.ReadOnly = true;
			  this.OnHandQuantity.Width = 94;
			  // 
			  // DifPress
			  // 
			  this.DifPress.DataPropertyName = "UserData10";
			  this.DifPress.HeaderText = "DP";
			  this.DifPress.Name = "DifPress";
			  this.DifPress.ReadOnly = true;
			  this.DifPress.Width = 47;
			  // 
			  // panel3
			  // 
			  this.panel3.Controls.Add(this.dataGridView1);
			  this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
			  this.panel3.Location = new System.Drawing.Point(0, 78);
			  this.panel3.Margin = new System.Windows.Forms.Padding(2);
			  this.panel3.Name = "panel3";
			  this.panel3.Size = new System.Drawing.Size(974, 573);
			  this.panel3.TabIndex = 8;
			  // 
			  // StatusCombo
			  // 
			  this.StatusCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			  this.StatusCombo.FormattingEnabled = true;
			  this.StatusCombo.Items.AddRange(new object[] {
            "{All}",
            "Requested",
            "Dispatched",
            "Arrived",
            "Started",
            "Stopped",
            "Completed",
            "Flight Line"});
			  this.StatusCombo.Location = new System.Drawing.Point(306, 5);
			  this.StatusCombo.Margin = new System.Windows.Forms.Padding(2);
			  this.StatusCombo.Name = "StatusCombo";
			  this.StatusCombo.Size = new System.Drawing.Size(92, 21);
			  this.StatusCombo.TabIndex = 2;
			  this.StatusCombo.SelectedIndexChanged += new System.EventHandler(this.StatusCombo_SelectedIndexChanged);
			  // 
			  // label1
			  // 
			  this.label1.AutoSize = true;
			  this.label1.Location = new System.Drawing.Point(7, 9);
			  this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			  this.label1.Name = "label1";
			  this.label1.Size = new System.Drawing.Size(37, 13);
			  this.label1.TabIndex = 4;
			  this.label1.Text = "Begin:";
			  // 
			  // label2
			  // 
			  this.label2.AutoSize = true;
			  this.label2.Location = new System.Drawing.Point(136, 9);
			  this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			  this.label2.Name = "label2";
			  this.label2.Size = new System.Drawing.Size(29, 13);
			  this.label2.TabIndex = 5;
			  this.label2.Text = "End:";
			  // 
			  // label3
			  // 
			  this.label3.AutoSize = true;
			  this.label3.Location = new System.Drawing.Point(265, 9);
			  this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			  this.label3.Name = "label3";
			  this.label3.Size = new System.Drawing.Size(40, 13);
			  this.label3.TabIndex = 7;
			  this.label3.Text = "Status:";
			  // 
			  // BeginDatePicker
			  // 
			  this.BeginDatePicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
			  this.BeginDatePicker.Location = new System.Drawing.Point(47, 5);
			  this.BeginDatePicker.Margin = new System.Windows.Forms.Padding(2);
			  this.BeginDatePicker.Name = "BeginDatePicker";
			  this.BeginDatePicker.Size = new System.Drawing.Size(83, 20);
			  this.BeginDatePicker.TabIndex = 0;
			  this.BeginDatePicker.ValueChanged += new System.EventHandler(this.BeginDatePicker_ValueChanged);
			  this.BeginDatePicker.Leave += new System.EventHandler(this.BeginDatePicker_Leave);
			  // 
			  // EndDatePicker
			  // 
			  this.EndDatePicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
			  this.EndDatePicker.Location = new System.Drawing.Point(170, 5);
			  this.EndDatePicker.Margin = new System.Windows.Forms.Padding(2);
			  this.EndDatePicker.Name = "EndDatePicker";
			  this.EndDatePicker.Size = new System.Drawing.Size(82, 20);
			  this.EndDatePicker.TabIndex = 1;
			  this.EndDatePicker.ValueChanged += new System.EventHandler(this.EndDatePicker_ValueChanged);
			  this.EndDatePicker.Leave += new System.EventHandler(this.EndDatePicker_Leave);
			  // 
			  // label4
			  // 
			  this.label4.AutoSize = true;
			  this.label4.Location = new System.Drawing.Point(410, 9);
			  this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			  this.label4.Name = "label4";
			  this.label4.Size = new System.Drawing.Size(77, 13);
			  this.label4.TabIndex = 9;
			  this.label4.Text = "Request Type:";
			  // 
			  // RequestTypeCombo
			  // 
			  this.RequestTypeCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			  this.RequestTypeCombo.FormattingEnabled = true;
			  this.RequestTypeCombo.Items.AddRange(new object[] {
            "{All}",
            "Refuel",
            "Defuel",
            "Fill Stand",
            "Return to Bulk",
            "Recirculation"});
			  this.RequestTypeCombo.Location = new System.Drawing.Point(488, 5);
			  this.RequestTypeCombo.Margin = new System.Windows.Forms.Padding(2);
			  this.RequestTypeCombo.Name = "RequestTypeCombo";
			  this.RequestTypeCombo.Size = new System.Drawing.Size(92, 21);
			  this.RequestTypeCombo.TabIndex = 3;
			  this.RequestTypeCombo.SelectedIndexChanged += new System.EventHandler(this.RequestTypeCombo_SelectedIndexChanged);
			  // 
			  // panel1
			  // 
			  this.panel1.Controls.Add(this.label5);
			  this.panel1.Controls.Add(this.vehicleComboBox);
			  this.panel1.Controls.Add(this.julianDateLabel);
			  this.panel1.Controls.Add(this.RequestTypeCombo);
			  this.panel1.Controls.Add(this.label4);
			  this.panel1.Controls.Add(this.EndDatePicker);
			  this.panel1.Controls.Add(this.BeginDatePicker);
			  this.panel1.Controls.Add(this.label3);
			  this.panel1.Controls.Add(this.label2);
			  this.panel1.Controls.Add(this.label1);
			  this.panel1.Controls.Add(this.StatusCombo);
			  this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			  this.panel1.Location = new System.Drawing.Point(0, 49);
			  this.panel1.Margin = new System.Windows.Forms.Padding(2);
			  this.panel1.Name = "panel1";
			  this.panel1.Size = new System.Drawing.Size(974, 29);
			  this.panel1.TabIndex = 6;
			  // 
			  // label5
			  // 
			  this.label5.AutoSize = true;
			  this.label5.Location = new System.Drawing.Point(607, 9);
			  this.label5.Name = "label5";
			  this.label5.Size = new System.Drawing.Size(45, 13);
			  this.label5.TabIndex = 14;
			  this.label5.Text = "Vehicle:";
			  // 
			  // vehicleComboBox
			  // 
			  this.vehicleComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			  this.vehicleComboBox.FormattingEnabled = true;
			  this.vehicleComboBox.Location = new System.Drawing.Point(659, 5);
			  this.vehicleComboBox.Name = "vehicleComboBox";
			  this.vehicleComboBox.Size = new System.Drawing.Size(113, 21);
			  this.vehicleComboBox.TabIndex = 4;
			  this.vehicleComboBox.SelectedIndexChanged += new System.EventHandler(this.vehicleComboBox_SelectedIndexChanged);
			  // 
			  // julianDateLabel
			  // 
			  this.julianDateLabel.AutoSize = true;
			  this.julianDateLabel.Location = new System.Drawing.Point(787, 9);
			  this.julianDateLabel.Name = "julianDateLabel";
			  this.julianDateLabel.Size = new System.Drawing.Size(35, 13);
			  this.julianDateLabel.TabIndex = 12;
			  this.julianDateLabel.Text = "label5";
			  // 
			  // julianDateTimer
			  // 
			  this.julianDateTimer.Enabled = true;
			  this.julianDateTimer.Tick += new System.EventHandler(this.julianDateTimer_Tick);
			  // 
			  // DispatchContainerForm
			  // 
			  this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			  this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			  this.ClientSize = new System.Drawing.Size(974, 673);
			  this.Controls.Add(this.panel3);
			  this.Controls.Add(this.panel1);
			  this.Controls.Add(this.statusStrip);
			  this.Controls.Add(this.toolStrip);
			  this.Controls.Add(this.menuStrip);
			  this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			  this.IsMdiContainer = true;
			  this.MainMenuStrip = this.menuStrip;
			  this.Name = "DispatchContainerForm";
			  this.Text = "FuelsManager Dispatch";
			  this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			  this.Load += new System.EventHandler(this.DispatchContainerForm_Load);
			  this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DispatchContainerForm_FormClosing);
			  this.menuStrip.ResumeLayout(false);
			  this.menuStrip.PerformLayout();
			  this.toolStrip.ResumeLayout(false);
			  this.toolStrip.PerformLayout();
			  this.statusStrip.ResumeLayout(false);
			  this.statusStrip.PerformLayout();
			  ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
			  this.panel3.ResumeLayout(false);
			  this.panel1.ResumeLayout(false);
			  this.panel1.PerformLayout();
			  this.ResumeLayout(false);
			  this.PerformLayout();

        }

        #endregion


        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStrip toolStrip;
		  private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
		  private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
		  private System.Windows.Forms.ToolStripMenuItem fileMenu;
		  private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewMenu;
        private System.Windows.Forms.ToolStripMenuItem toolBarToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem statusBarToolStripMenuItem;
		  private System.Windows.Forms.ToolStripMenuItem toolsMenu;
        private System.Windows.Forms.ToolStripMenuItem helpMenu;
        private System.Windows.Forms.ToolStripMenuItem contentsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem indexToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton helpToolStripButton;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.ToolStripMenuItem operationToolStripMenuItem;
		  private System.Windows.Forms.ToolStripMenuItem addInsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dispatchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem queryWriterToolStripMenuItem;
        private System.Windows.Forms.DataGridView dataGridView1;
		  private System.Windows.Forms.Panel panel3;
		  private System.Windows.Forms.ComboBox StatusCombo;
		  private System.Windows.Forms.Label label1;
		  private System.Windows.Forms.Label label2;
		  private System.Windows.Forms.Label label3;
		  private System.Windows.Forms.DateTimePicker BeginDatePicker;
		  private System.Windows.Forms.DateTimePicker EndDatePicker;
		  private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox RequestTypeCombo;
		  private System.Windows.Forms.Panel panel1;
          private System.Windows.Forms.ToolStripMenuItem requestRefuelToolStripMenuItem;
          private System.Windows.Forms.ToolStripMenuItem transientToolStripMenuItem;
          private System.Windows.Forms.ToolStripMenuItem fastLogToolStripMenuItem;
          private System.Windows.Forms.ToolStripMenuItem fastLogFillstandToolStripMenuItem;
          private System.Windows.Forms.ToolStripMenuItem recirculationToolStripMenuItem;
			 private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
          private System.Windows.Forms.ToolStripMenuItem standByToolStripMenuItem;
          private System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
			 private System.Windows.Forms.ToolStripMenuItem configurationToolStripMenuItem1;
          private System.Windows.Forms.ToolStripMenuItem controllersLogToolStripMenuItem;
          private System.Windows.Forms.ToolStripMenuItem changeOfOperatorToolStripMenuItem;
          private System.Windows.Forms.ToolStripMenuItem totalAndAverageToolStripMenuItem;
          private System.Windows.Forms.ToolStripSeparator toolStripSeparator12;
          private System.Windows.Forms.ToolStripSeparator toolStripSeparator13;
          private System.Windows.Forms.ToolStripMenuItem exportToAccountingToolStripMenuItem;
			 private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
          private System.Windows.Forms.ToolStripMenuItem relogToolStripMenuItem;
          private System.Windows.Forms.ToolStripMenuItem cancelToolStripMenuItem;
			 private System.Windows.Forms.ToolStripButton toolStripRequestRefuelButton;
			 private System.Windows.Forms.ToolStripButton toolStripTransientButton;
			 private System.Windows.Forms.ToolStripButton toolStripFastLogButton;
			 private System.Windows.Forms.ToolStripButton toolStripFastLofFillstandButton;
			 private System.Windows.Forms.ToolStripButton toolStripRelogButton;
			 private System.Windows.Forms.ToolStripButton toolStripDispatchButton;
			 private System.Windows.Forms.ToolStripButton toolStripControllersLogButton;
			 private System.Windows.Forms.ToolStripButton toolStripCancelButton;
          private System.Windows.Forms.ToolStripButton toolStripStandbyButton;
			 private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
			 private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
			 private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
			 private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
			 private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem5;
			 private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem6;
			 private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem7;
          private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem8;
			 private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem10;
          private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
          private System.Windows.Forms.ToolStripMenuItem reportsToolStripMenuItem;
			 private System.Windows.Forms.ToolStripButton AverageAndTotalMenu;
			 private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
			 private System.Windows.Forms.ToolStripMenuItem evacuateToolStripMenuItem;
          private System.Windows.Forms.ToolStripMenuItem flightLineStatusToolStripMenuItem;
          private System.Windows.Forms.ToolStripButton FlightLineButton;
          private System.Windows.Forms.ToolStripMenuItem arrivalToolStripMenuItem;
          private System.Windows.Forms.ToolStripMenuItem startOfServiceToolStripMenuItem;
          private System.Windows.Forms.ToolStripMenuItem stopOfServiceToolStripMenuItem;
          private System.Windows.Forms.ToolStripMenuItem serviceCompletionToolStripMenuItem;
          private System.Windows.Forms.ToolStripMenuItem fillstandCompletionToolStripMenuItem;
          private System.Windows.Forms.ToolStripMenuItem optionalTimesToolStripMenuItem1;
          private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
          private System.Windows.Forms.Label julianDateLabel;
          private System.Windows.Forms.Timer julianDateTimer;
          private System.Windows.Forms.HelpProvider helpProvider1;
          private System.Windows.Forms.ComboBox vehicleComboBox;
          private System.Windows.Forms.Label label5;
          private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
          private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
          private System.Windows.Forms.DataGridViewTextBoxColumn RequestType;
          private System.Windows.Forms.DataGridViewTextBoxColumn Requested;
          private System.Windows.Forms.DataGridViewTextBoxColumn Column8;
          private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
          private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
          private System.Windows.Forms.DataGridViewTextBoxColumn AircraftID;
          private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
          private System.Windows.Forms.DataGridViewTextBoxColumn MDS;
          private System.Windows.Forms.DataGridViewTextBoxColumn Column6;
          private System.Windows.Forms.DataGridViewTextBoxColumn Column9;
          private System.Windows.Forms.DataGridViewTextBoxColumn Column10;
          private System.Windows.Forms.DataGridViewTextBoxColumn Column11;
          private System.Windows.Forms.DataGridViewTextBoxColumn Column12;
          private System.Windows.Forms.DataGridViewTextBoxColumn Grade;
          private System.Windows.Forms.DataGridViewTextBoxColumn Column14;
          private System.Windows.Forms.DataGridViewTextBoxColumn Column13;
          private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
          private System.Windows.Forms.DataGridViewTextBoxColumn Column16;
          private System.Windows.Forms.DataGridViewTextBoxColumn Column15;
          private System.Windows.Forms.DataGridViewTextBoxColumn RequestedBy;
          private System.Windows.Forms.DataGridViewCheckBoxColumn Cancelled;
          private System.Windows.Forms.DataGridViewTextBoxColumn DODAAC;
          private System.Windows.Forms.DataGridViewTextBoxColumn SuppDODAAC;
          private System.Windows.Forms.DataGridViewTextBoxColumn BOS;
          private System.Windows.Forms.DataGridViewTextBoxColumn SignalCode;
          private System.Windows.Forms.DataGridViewTextBoxColumn USECode;
          private System.Windows.Forms.DataGridViewTextBoxColumn FundCode;
          private System.Windows.Forms.DataGridViewTextBoxColumn RPTTECAPC;
          private System.Windows.Forms.DataGridViewTextBoxColumn CardNumber;
          private System.Windows.Forms.DataGridViewTextBoxColumn RecirculationType;
          private System.Windows.Forms.DataGridViewTextBoxColumn REFID;
          private System.Windows.Forms.DataGridViewTextBoxColumn SerialNum;
          private System.Windows.Forms.DataGridViewTextBoxColumn IssPt;
          private System.Windows.Forms.DataGridViewTextBoxColumn IssPtNum;
          private System.Windows.Forms.DataGridViewTextBoxColumn Activity;
          private System.Windows.Forms.DataGridViewTextBoxColumn GrossGal;
          private System.Windows.Forms.DataGridViewCheckBoxColumn FuelAdditiveFlag;
          private System.Windows.Forms.DataGridViewTextBoxColumn OnHandQuantity;
			 private System.Windows.Forms.DataGridViewTextBoxColumn DifPress;
    }
}




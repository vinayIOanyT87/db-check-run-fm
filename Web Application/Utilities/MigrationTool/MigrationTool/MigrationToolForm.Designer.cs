
namespace MigrationTool
{
    partial class MigrationToolForm
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
            this.MainTabControl = new System.Windows.Forms.TabControl();
            this.DbConnectionTab = new System.Windows.Forms.TabPage();
            this.LoadConfigurationBtn = new System.Windows.Forms.Button();
            this.DbConfigExitBtn = new System.Windows.Forms.Button();
            this.DbConfigSaveBtn = new System.Windows.Forms.Button();
            this.TabDbConnectionConfigLbl = new System.Windows.Forms.Label();
            this.TargetDbLbl = new System.Windows.Forms.Label();
            this.TargetPanel = new System.Windows.Forms.Panel();
            this.TargetTestConnectionBtn = new System.Windows.Forms.Button();
            this.TargetSecurityTb = new System.Windows.Forms.TextBox();
            this.TargetTestConnectionTb = new System.Windows.Forms.TextBox();
            this.TargetDbServerTb = new System.Windows.Forms.TextBox();
            this.TargetSecurityLbl = new System.Windows.Forms.Label();
            this.TargetDbServerLbl = new System.Windows.Forms.Label();
            this.TargetInSecondsLbl = new System.Windows.Forms.Label();
            this.TargetDbNameLbl = new System.Windows.Forms.Label();
            this.TargetConnectTimeoutSpinner = new System.Windows.Forms.NumericUpDown();
            this.TargetDbNameTb = new System.Windows.Forms.TextBox();
            this.TargetConnectTimeoutLbl = new System.Windows.Forms.Label();
            this.SourceDbLbl = new System.Windows.Forms.Label();
            this.SourcePanel = new System.Windows.Forms.Panel();
            this.SourceTestConnectionBtn = new System.Windows.Forms.Button();
            this.SourceTestConnectionTb = new System.Windows.Forms.TextBox();
            this.SourceSecurityTb = new System.Windows.Forms.TextBox();
            this.SourceSecurityLbl = new System.Windows.Forms.Label();
            this.SourceInSecondsLbl = new System.Windows.Forms.Label();
            this.SourceConnectTimeoutSpinner = new System.Windows.Forms.NumericUpDown();
            this.SourceConnectTimeoutLbl = new System.Windows.Forms.Label();
            this.SourceDbNameTb = new System.Windows.Forms.TextBox();
            this.SourceDbNameLbl = new System.Windows.Forms.Label();
            this.SourceDbServerTb = new System.Windows.Forms.TextBox();
            this.SourceDbServerLbl = new System.Windows.Forms.Label();
            this.MigrationToolTab = new System.Windows.Forms.TabPage();
            this.ApplicationStringToolCb = new System.Windows.Forms.CheckBox();
            this.QualificationToolCb = new System.Windows.Forms.CheckBox();
            this.EquipmentTypeToolCb = new System.Windows.Forms.CheckBox();
            this.QueriesToolCb = new System.Windows.Forms.CheckBox();
            this.StationsToolCb = new System.Windows.Forms.CheckBox();
            this.FootnoteToolCb = new System.Windows.Forms.CheckBox();
            this.PersonnelToolCb = new System.Windows.Forms.CheckBox();
            this.EquipmentToolCb = new System.Windows.Forms.CheckBox();
            this.PersonnelSelectToolsLbl = new System.Windows.Forms.Label();
            this.PersonnelTargetSiteIdTb = new System.Windows.Forms.TextBox();
            this.PersonnelTargetSiteIdLbel = new System.Windows.Forms.Label();
            this.PersonnelSourceSiteIdTb = new System.Windows.Forms.TextBox();
            this.PersonnelSourceSiteIdLbl = new System.Windows.Forms.Label();
            this.PersonnelClearResultBtn = new System.Windows.Forms.Button();
            this.PersonnelResultsTb = new System.Windows.Forms.TextBox();
            this.PersonnelResultsLbl = new System.Windows.Forms.Label();
            this.PersonnelStartMigrationBtn = new System.Windows.Forms.Button();
            this.PersonnelMigrationVersionLbl = new System.Windows.Forms.Label();
            this.PersonnelMigrationVersionCB = new System.Windows.Forms.ComboBox();
            this.MainTabControl.SuspendLayout();
            this.DbConnectionTab.SuspendLayout();
            this.TargetPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TargetConnectTimeoutSpinner)).BeginInit();
            this.SourcePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SourceConnectTimeoutSpinner)).BeginInit();
            this.MigrationToolTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainTabControl
            // 
            this.MainTabControl.Controls.Add(this.DbConnectionTab);
            this.MainTabControl.Controls.Add(this.MigrationToolTab);
            this.MainTabControl.Location = new System.Drawing.Point(13, 13);
            this.MainTabControl.Name = "MainTabControl";
            this.MainTabControl.SelectedIndex = 0;
            this.MainTabControl.Size = new System.Drawing.Size(932, 672);
            this.MainTabControl.TabIndex = 0;
            // 
            // DbConnectionTab
            // 
            this.DbConnectionTab.Controls.Add(this.LoadConfigurationBtn);
            this.DbConnectionTab.Controls.Add(this.DbConfigExitBtn);
            this.DbConnectionTab.Controls.Add(this.DbConfigSaveBtn);
            this.DbConnectionTab.Controls.Add(this.TabDbConnectionConfigLbl);
            this.DbConnectionTab.Controls.Add(this.TargetDbLbl);
            this.DbConnectionTab.Controls.Add(this.TargetPanel);
            this.DbConnectionTab.Controls.Add(this.SourceDbLbl);
            this.DbConnectionTab.Controls.Add(this.SourcePanel);
            this.DbConnectionTab.Location = new System.Drawing.Point(4, 22);
            this.DbConnectionTab.Name = "DbConnectionTab";
            this.DbConnectionTab.Padding = new System.Windows.Forms.Padding(3);
            this.DbConnectionTab.Size = new System.Drawing.Size(924, 646);
            this.DbConnectionTab.TabIndex = 0;
            this.DbConnectionTab.Text = "DB Connection";
            this.DbConnectionTab.UseVisualStyleBackColor = true;
            // 
            // LoadConfigurationBtn
            // 
            this.LoadConfigurationBtn.Location = new System.Drawing.Point(17, 580);
            this.LoadConfigurationBtn.Name = "LoadConfigurationBtn";
            this.LoadConfigurationBtn.Size = new System.Drawing.Size(157, 23);
            this.LoadConfigurationBtn.TabIndex = 7;
            this.LoadConfigurationBtn.Text = "Load Saved Configuration";
            this.LoadConfigurationBtn.UseVisualStyleBackColor = true;
            this.LoadConfigurationBtn.Click += new System.EventHandler(this.LoadConfigurationOnClickBtn);
            // 
            // DbConfigExitBtn
            // 
            this.DbConfigExitBtn.Location = new System.Drawing.Point(595, 581);
            this.DbConfigExitBtn.Name = "DbConfigExitBtn";
            this.DbConfigExitBtn.Size = new System.Drawing.Size(75, 23);
            this.DbConfigExitBtn.TabIndex = 6;
            this.DbConfigExitBtn.Text = "Exit";
            this.DbConfigExitBtn.UseVisualStyleBackColor = true;
            this.DbConfigExitBtn.Click += new System.EventHandler(this.DbConfigExitOnClickBtn);
            // 
            // DbConfigSaveBtn
            // 
            this.DbConfigSaveBtn.Location = new System.Drawing.Point(499, 581);
            this.DbConfigSaveBtn.Name = "DbConfigSaveBtn";
            this.DbConfigSaveBtn.Size = new System.Drawing.Size(75, 23);
            this.DbConfigSaveBtn.TabIndex = 5;
            this.DbConfigSaveBtn.Text = "Save";
            this.DbConfigSaveBtn.UseVisualStyleBackColor = true;
            this.DbConfigSaveBtn.Click += new System.EventHandler(this.SaveDbConfigOnClickBtn);
            // 
            // TabDbConnectionConfigLbl
            // 
            this.TabDbConnectionConfigLbl.AutoSize = true;
            this.TabDbConnectionConfigLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TabDbConnectionConfigLbl.Location = new System.Drawing.Point(9, 7);
            this.TabDbConnectionConfigLbl.Name = "TabDbConnectionConfigLbl";
            this.TabDbConnectionConfigLbl.Size = new System.Drawing.Size(296, 20);
            this.TabDbConnectionConfigLbl.TabIndex = 4;
            this.TabDbConnectionConfigLbl.Text = "Database Connection Configuration";
            // 
            // TargetDbLbl
            // 
            this.TargetDbLbl.AutoSize = true;
            this.TargetDbLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TargetDbLbl.Location = new System.Drawing.Point(6, 314);
            this.TargetDbLbl.Name = "TargetDbLbl";
            this.TargetDbLbl.Size = new System.Drawing.Size(82, 17);
            this.TargetDbLbl.TabIndex = 3;
            this.TargetDbLbl.Text = "Target DB";
            // 
            // TargetPanel
            // 
            this.TargetPanel.Controls.Add(this.TargetTestConnectionBtn);
            this.TargetPanel.Controls.Add(this.TargetSecurityTb);
            this.TargetPanel.Controls.Add(this.TargetTestConnectionTb);
            this.TargetPanel.Controls.Add(this.TargetDbServerTb);
            this.TargetPanel.Controls.Add(this.TargetSecurityLbl);
            this.TargetPanel.Controls.Add(this.TargetDbServerLbl);
            this.TargetPanel.Controls.Add(this.TargetInSecondsLbl);
            this.TargetPanel.Controls.Add(this.TargetDbNameLbl);
            this.TargetPanel.Controls.Add(this.TargetConnectTimeoutSpinner);
            this.TargetPanel.Controls.Add(this.TargetDbNameTb);
            this.TargetPanel.Controls.Add(this.TargetConnectTimeoutLbl);
            this.TargetPanel.Location = new System.Drawing.Point(9, 334);
            this.TargetPanel.Name = "TargetPanel";
            this.TargetPanel.Size = new System.Drawing.Size(661, 223);
            this.TargetPanel.TabIndex = 2;
            // 
            // TargetTestConnectionBtn
            // 
            this.TargetTestConnectionBtn.Location = new System.Drawing.Point(7, 135);
            this.TargetTestConnectionBtn.Name = "TargetTestConnectionBtn";
            this.TargetTestConnectionBtn.Size = new System.Drawing.Size(86, 42);
            this.TargetTestConnectionBtn.TabIndex = 13;
            this.TargetTestConnectionBtn.Text = "Test Connection";
            this.TargetTestConnectionBtn.UseVisualStyleBackColor = true;
            this.TargetTestConnectionBtn.Click += new System.EventHandler(this.TargetTestConnectionOnClickBtn);
            // 
            // TargetSecurityTb
            // 
            this.TargetSecurityTb.Location = new System.Drawing.Point(99, 100);
            this.TargetSecurityTb.Name = "TargetSecurityTb";
            this.TargetSecurityTb.Size = new System.Drawing.Size(405, 20);
            this.TargetSecurityTb.TabIndex = 17;
            this.TargetSecurityTb.TextChanged += new System.EventHandler(this.TargetSecurityOnChange);
            // 
            // TargetTestConnectionTb
            // 
            this.TargetTestConnectionTb.Location = new System.Drawing.Point(99, 135);
            this.TargetTestConnectionTb.Multiline = true;
            this.TargetTestConnectionTb.Name = "TargetTestConnectionTb";
            this.TargetTestConnectionTb.ReadOnly = true;
            this.TargetTestConnectionTb.Size = new System.Drawing.Size(404, 68);
            this.TargetTestConnectionTb.TabIndex = 12;
            // 
            // TargetDbServerTb
            // 
            this.TargetDbServerTb.Location = new System.Drawing.Point(99, 12);
            this.TargetDbServerTb.Name = "TargetDbServerTb";
            this.TargetDbServerTb.Size = new System.Drawing.Size(405, 20);
            this.TargetDbServerTb.TabIndex = 10;
            this.TargetDbServerTb.TextChanged += new System.EventHandler(this.TargetDbServerOnChange);
            // 
            // TargetSecurityLbl
            // 
            this.TargetSecurityLbl.AutoSize = true;
            this.TargetSecurityLbl.Location = new System.Drawing.Point(5, 100);
            this.TargetSecurityLbl.Name = "TargetSecurityLbl";
            this.TargetSecurityLbl.Size = new System.Drawing.Size(48, 13);
            this.TargetSecurityLbl.TabIndex = 16;
            this.TargetSecurityLbl.Text = "Security:";
            // 
            // TargetDbServerLbl
            // 
            this.TargetDbServerLbl.AutoSize = true;
            this.TargetDbServerLbl.Location = new System.Drawing.Point(4, 12);
            this.TargetDbServerLbl.Name = "TargetDbServerLbl";
            this.TargetDbServerLbl.Size = new System.Drawing.Size(59, 13);
            this.TargetDbServerLbl.TabIndex = 9;
            this.TargetDbServerLbl.Text = "DB Server:";
            // 
            // TargetInSecondsLbl
            // 
            this.TargetInSecondsLbl.AutoSize = true;
            this.TargetInSecondsLbl.Location = new System.Drawing.Point(161, 73);
            this.TargetInSecondsLbl.Name = "TargetInSecondsLbl";
            this.TargetInSecondsLbl.Size = new System.Drawing.Size(61, 13);
            this.TargetInSecondsLbl.TabIndex = 15;
            this.TargetInSecondsLbl.Text = "In Seconds";
            // 
            // TargetDbNameLbl
            // 
            this.TargetDbNameLbl.AutoSize = true;
            this.TargetDbNameLbl.Location = new System.Drawing.Point(4, 40);
            this.TargetDbNameLbl.Name = "TargetDbNameLbl";
            this.TargetDbNameLbl.Size = new System.Drawing.Size(56, 13);
            this.TargetDbNameLbl.TabIndex = 11;
            this.TargetDbNameLbl.Text = "DB Name:";
            // 
            // TargetConnectTimeoutSpinner
            // 
            this.TargetConnectTimeoutSpinner.Location = new System.Drawing.Point(99, 70);
            this.TargetConnectTimeoutSpinner.Name = "TargetConnectTimeoutSpinner";
            this.TargetConnectTimeoutSpinner.Size = new System.Drawing.Size(55, 20);
            this.TargetConnectTimeoutSpinner.TabIndex = 14;
            this.TargetConnectTimeoutSpinner.ValueChanged += new System.EventHandler(this.TargetConnectSpinnerOnChange);
            // 
            // TargetDbNameTb
            // 
            this.TargetDbNameTb.Location = new System.Drawing.Point(99, 40);
            this.TargetDbNameTb.Name = "TargetDbNameTb";
            this.TargetDbNameTb.Size = new System.Drawing.Size(405, 20);
            this.TargetDbNameTb.TabIndex = 12;
            this.TargetDbNameTb.TextChanged += new System.EventHandler(this.TargetDbNameOnChange);
            // 
            // TargetConnectTimeoutLbl
            // 
            this.TargetConnectTimeoutLbl.AutoSize = true;
            this.TargetConnectTimeoutLbl.Location = new System.Drawing.Point(5, 73);
            this.TargetConnectTimeoutLbl.Name = "TargetConnectTimeoutLbl";
            this.TargetConnectTimeoutLbl.Size = new System.Drawing.Size(91, 13);
            this.TargetConnectTimeoutLbl.TabIndex = 13;
            this.TargetConnectTimeoutLbl.Text = "Connect Timeout:";
            // 
            // SourceDbLbl
            // 
            this.SourceDbLbl.AutoSize = true;
            this.SourceDbLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SourceDbLbl.Location = new System.Drawing.Point(6, 44);
            this.SourceDbLbl.Name = "SourceDbLbl";
            this.SourceDbLbl.Size = new System.Drawing.Size(85, 17);
            this.SourceDbLbl.TabIndex = 1;
            this.SourceDbLbl.Text = "Source DB";
            // 
            // SourcePanel
            // 
            this.SourcePanel.Controls.Add(this.SourceTestConnectionBtn);
            this.SourcePanel.Controls.Add(this.SourceTestConnectionTb);
            this.SourcePanel.Controls.Add(this.SourceSecurityTb);
            this.SourcePanel.Controls.Add(this.SourceSecurityLbl);
            this.SourcePanel.Controls.Add(this.SourceInSecondsLbl);
            this.SourcePanel.Controls.Add(this.SourceConnectTimeoutSpinner);
            this.SourcePanel.Controls.Add(this.SourceConnectTimeoutLbl);
            this.SourcePanel.Controls.Add(this.SourceDbNameTb);
            this.SourcePanel.Controls.Add(this.SourceDbNameLbl);
            this.SourcePanel.Controls.Add(this.SourceDbServerTb);
            this.SourcePanel.Controls.Add(this.SourceDbServerLbl);
            this.SourcePanel.Location = new System.Drawing.Point(6, 65);
            this.SourcePanel.Name = "SourcePanel";
            this.SourcePanel.Size = new System.Drawing.Size(664, 226);
            this.SourcePanel.TabIndex = 0;
            // 
            // SourceTestConnectionBtn
            // 
            this.SourceTestConnectionBtn.Location = new System.Drawing.Point(11, 136);
            this.SourceTestConnectionBtn.Name = "SourceTestConnectionBtn";
            this.SourceTestConnectionBtn.Size = new System.Drawing.Size(86, 42);
            this.SourceTestConnectionBtn.TabIndex = 11;
            this.SourceTestConnectionBtn.Text = "Test Connection";
            this.SourceTestConnectionBtn.UseVisualStyleBackColor = true;
            this.SourceTestConnectionBtn.Click += new System.EventHandler(this.SourceTestConnectionOnClickBtn);
            // 
            // SourceTestConnectionTb
            // 
            this.SourceTestConnectionTb.Location = new System.Drawing.Point(103, 136);
            this.SourceTestConnectionTb.Multiline = true;
            this.SourceTestConnectionTb.Name = "SourceTestConnectionTb";
            this.SourceTestConnectionTb.ReadOnly = true;
            this.SourceTestConnectionTb.Size = new System.Drawing.Size(404, 68);
            this.SourceTestConnectionTb.TabIndex = 10;
            // 
            // SourceSecurityTb
            // 
            this.SourceSecurityTb.Location = new System.Drawing.Point(102, 101);
            this.SourceSecurityTb.Name = "SourceSecurityTb";
            this.SourceSecurityTb.Size = new System.Drawing.Size(405, 20);
            this.SourceSecurityTb.TabIndex = 8;
            this.SourceSecurityTb.TextChanged += new System.EventHandler(this.SourceSecurityOnChange);
            // 
            // SourceSecurityLbl
            // 
            this.SourceSecurityLbl.AutoSize = true;
            this.SourceSecurityLbl.Location = new System.Drawing.Point(8, 101);
            this.SourceSecurityLbl.Name = "SourceSecurityLbl";
            this.SourceSecurityLbl.Size = new System.Drawing.Size(48, 13);
            this.SourceSecurityLbl.TabIndex = 7;
            this.SourceSecurityLbl.Text = "Security:";
            // 
            // SourceInSecondsLbl
            // 
            this.SourceInSecondsLbl.AutoSize = true;
            this.SourceInSecondsLbl.Location = new System.Drawing.Point(164, 74);
            this.SourceInSecondsLbl.Name = "SourceInSecondsLbl";
            this.SourceInSecondsLbl.Size = new System.Drawing.Size(61, 13);
            this.SourceInSecondsLbl.TabIndex = 6;
            this.SourceInSecondsLbl.Text = "In Seconds";
            // 
            // SourceConnectTimeoutSpinner
            // 
            this.SourceConnectTimeoutSpinner.Location = new System.Drawing.Point(102, 71);
            this.SourceConnectTimeoutSpinner.Name = "SourceConnectTimeoutSpinner";
            this.SourceConnectTimeoutSpinner.Size = new System.Drawing.Size(55, 20);
            this.SourceConnectTimeoutSpinner.TabIndex = 5;
            this.SourceConnectTimeoutSpinner.ValueChanged += new System.EventHandler(this.SourceConnectSpinnerOnChange);
            // 
            // SourceConnectTimeoutLbl
            // 
            this.SourceConnectTimeoutLbl.AutoSize = true;
            this.SourceConnectTimeoutLbl.Location = new System.Drawing.Point(8, 74);
            this.SourceConnectTimeoutLbl.Name = "SourceConnectTimeoutLbl";
            this.SourceConnectTimeoutLbl.Size = new System.Drawing.Size(91, 13);
            this.SourceConnectTimeoutLbl.TabIndex = 4;
            this.SourceConnectTimeoutLbl.Text = "Connect Timeout:";
            // 
            // SourceDbNameTb
            // 
            this.SourceDbNameTb.Location = new System.Drawing.Point(102, 41);
            this.SourceDbNameTb.Name = "SourceDbNameTb";
            this.SourceDbNameTb.Size = new System.Drawing.Size(405, 20);
            this.SourceDbNameTb.TabIndex = 3;
            this.SourceDbNameTb.TextChanged += new System.EventHandler(this.SourceDbNameOnChange);
            // 
            // SourceDbNameLbl
            // 
            this.SourceDbNameLbl.AutoSize = true;
            this.SourceDbNameLbl.Location = new System.Drawing.Point(7, 41);
            this.SourceDbNameLbl.Name = "SourceDbNameLbl";
            this.SourceDbNameLbl.Size = new System.Drawing.Size(56, 13);
            this.SourceDbNameLbl.TabIndex = 2;
            this.SourceDbNameLbl.Text = "DB Name:";
            // 
            // SourceDbServerTb
            // 
            this.SourceDbServerTb.Location = new System.Drawing.Point(102, 13);
            this.SourceDbServerTb.Name = "SourceDbServerTb";
            this.SourceDbServerTb.Size = new System.Drawing.Size(405, 20);
            this.SourceDbServerTb.TabIndex = 1;
            this.SourceDbServerTb.TextChanged += new System.EventHandler(this.SourceDbServerOnChange);
            // 
            // SourceDbServerLbl
            // 
            this.SourceDbServerLbl.AutoSize = true;
            this.SourceDbServerLbl.Location = new System.Drawing.Point(7, 13);
            this.SourceDbServerLbl.Name = "SourceDbServerLbl";
            this.SourceDbServerLbl.Size = new System.Drawing.Size(59, 13);
            this.SourceDbServerLbl.TabIndex = 0;
            this.SourceDbServerLbl.Text = "DB Server:";
            // 
            // MigrationToolTab
            // 
            this.MigrationToolTab.Controls.Add(this.ApplicationStringToolCb);
            this.MigrationToolTab.Controls.Add(this.QualificationToolCb);
            this.MigrationToolTab.Controls.Add(this.EquipmentTypeToolCb);
            this.MigrationToolTab.Controls.Add(this.QueriesToolCb);
            this.MigrationToolTab.Controls.Add(this.StationsToolCb);
            this.MigrationToolTab.Controls.Add(this.FootnoteToolCb);
            this.MigrationToolTab.Controls.Add(this.PersonnelToolCb);
            this.MigrationToolTab.Controls.Add(this.EquipmentToolCb);
            this.MigrationToolTab.Controls.Add(this.PersonnelSelectToolsLbl);
            this.MigrationToolTab.Controls.Add(this.PersonnelTargetSiteIdTb);
            this.MigrationToolTab.Controls.Add(this.PersonnelTargetSiteIdLbel);
            this.MigrationToolTab.Controls.Add(this.PersonnelSourceSiteIdTb);
            this.MigrationToolTab.Controls.Add(this.PersonnelSourceSiteIdLbl);
            this.MigrationToolTab.Controls.Add(this.PersonnelClearResultBtn);
            this.MigrationToolTab.Controls.Add(this.PersonnelResultsTb);
            this.MigrationToolTab.Controls.Add(this.PersonnelResultsLbl);
            this.MigrationToolTab.Controls.Add(this.PersonnelStartMigrationBtn);
            this.MigrationToolTab.Controls.Add(this.PersonnelMigrationVersionLbl);
            this.MigrationToolTab.Controls.Add(this.PersonnelMigrationVersionCB);
            this.MigrationToolTab.Location = new System.Drawing.Point(4, 22);
            this.MigrationToolTab.Name = "MigrationToolTab";
            this.MigrationToolTab.Padding = new System.Windows.Forms.Padding(3);
            this.MigrationToolTab.Size = new System.Drawing.Size(924, 646);
            this.MigrationToolTab.TabIndex = 1;
            this.MigrationToolTab.Text = "Migration Tools";
            this.MigrationToolTab.UseVisualStyleBackColor = true;
            // 
            // ApplicationStringToolCb
            // 
            this.ApplicationStringToolCb.AutoSize = true;
            this.ApplicationStringToolCb.Location = new System.Drawing.Point(136, 114);
            this.ApplicationStringToolCb.Name = "ApplicationStringToolCb";
            this.ApplicationStringToolCb.Size = new System.Drawing.Size(108, 17);
            this.ApplicationStringToolCb.TabIndex = 19;
            this.ApplicationStringToolCb.Text = "Application String";
            this.ApplicationStringToolCb.UseVisualStyleBackColor = true;
            this.ApplicationStringToolCb.Click += new System.EventHandler(this.SelectToolsCheckboxOnClick);
            // 
            // QualificationToolCb
            // 
            this.QualificationToolCb.AutoSize = true;
            this.QualificationToolCb.Location = new System.Drawing.Point(245, 115);
            this.QualificationToolCb.Name = "QualificationToolCb";
            this.QualificationToolCb.Size = new System.Drawing.Size(89, 17);
            this.QualificationToolCb.TabIndex = 18;
            this.QualificationToolCb.Text = "Qualifications";
            this.QualificationToolCb.UseVisualStyleBackColor = true;
            this.QualificationToolCb.Click += new System.EventHandler(this.SelectToolsCheckboxOnClick);
            // 
            // EquipmentTypeToolCb
            // 
            this.EquipmentTypeToolCb.AutoSize = true;
            this.EquipmentTypeToolCb.Location = new System.Drawing.Point(336, 115);
            this.EquipmentTypeToolCb.Name = "EquipmentTypeToolCb";
            this.EquipmentTypeToolCb.Size = new System.Drawing.Size(103, 17);
            this.EquipmentTypeToolCb.TabIndex = 17;
            this.EquipmentTypeToolCb.Text = "Equipment Type";
            this.EquipmentTypeToolCb.UseVisualStyleBackColor = true;
            this.EquipmentTypeToolCb.Click += new System.EventHandler(this.EquipmentTypeCheckboxOnClick);
            // 
            // QueriesToolCb
            // 
            this.QueriesToolCb.AutoSize = true;
            this.QueriesToolCb.Location = new System.Drawing.Point(740, 115);
            this.QueriesToolCb.Name = "QueriesToolCb";
            this.QueriesToolCb.Size = new System.Drawing.Size(62, 17);
            this.QueriesToolCb.TabIndex = 16;
            this.QueriesToolCb.Text = "Queries";
            this.QueriesToolCb.UseVisualStyleBackColor = true;
            this.QueriesToolCb.Click += new System.EventHandler(this.SelectToolsCheckboxOnClick);
            // 
            // StationsToolCb
            // 
            this.StationsToolCb.AutoSize = true;
            this.StationsToolCb.Location = new System.Drawing.Point(593, 115);
            this.StationsToolCb.Name = "StationsToolCb";
            this.StationsToolCb.Size = new System.Drawing.Size(64, 17);
            this.StationsToolCb.TabIndex = 15;
            this.StationsToolCb.Text = "Stations";
            this.StationsToolCb.UseVisualStyleBackColor = true;
            this.StationsToolCb.Click += new System.EventHandler(this.StationToolsCheckboxOnClick);
            // 
            // FootnoteToolCb
            // 
            this.FootnoteToolCb.AutoSize = true;
            this.FootnoteToolCb.Location = new System.Drawing.Point(661, 115);
            this.FootnoteToolCb.Name = "FootnoteToolCb";
            this.FootnoteToolCb.Size = new System.Drawing.Size(73, 17);
            this.FootnoteToolCb.TabIndex = 14;
            this.FootnoteToolCb.Text = "Footnotes";
            this.FootnoteToolCb.UseVisualStyleBackColor = true;
            this.FootnoteToolCb.Click += new System.EventHandler(this.SelectToolsCheckboxOnClick);
            // 
            // PersonnelToolCb
            // 
            this.PersonnelToolCb.AutoSize = true;
            this.PersonnelToolCb.Location = new System.Drawing.Point(520, 115);
            this.PersonnelToolCb.Name = "PersonnelToolCb";
            this.PersonnelToolCb.Size = new System.Drawing.Size(73, 17);
            this.PersonnelToolCb.TabIndex = 13;
            this.PersonnelToolCb.Text = "Personnel";
            this.PersonnelToolCb.UseVisualStyleBackColor = true;
            this.PersonnelToolCb.Click += new System.EventHandler(this.SelectToolsCheckboxOnClick);
            // 
            // EquipmentToolCb
            // 
            this.EquipmentToolCb.AutoSize = true;
            this.EquipmentToolCb.Location = new System.Drawing.Point(444, 115);
            this.EquipmentToolCb.Name = "EquipmentToolCb";
            this.EquipmentToolCb.Size = new System.Drawing.Size(76, 17);
            this.EquipmentToolCb.TabIndex = 12;
            this.EquipmentToolCb.Text = "Equipment";
            this.EquipmentToolCb.UseVisualStyleBackColor = true;
            this.EquipmentToolCb.Click += new System.EventHandler(this.SelectToolsCheckboxOnClick);
            // 
            // PersonnelSelectToolsLbl
            // 
            this.PersonnelSelectToolsLbl.AutoSize = true;
            this.PersonnelSelectToolsLbl.Location = new System.Drawing.Point(12, 115);
            this.PersonnelSelectToolsLbl.Name = "PersonnelSelectToolsLbl";
            this.PersonnelSelectToolsLbl.Size = new System.Drawing.Size(75, 13);
            this.PersonnelSelectToolsLbl.TabIndex = 10;
            this.PersonnelSelectToolsLbl.Text = "Select Tool(s):";
            // 
            // PersonnelTargetSiteIdTb
            // 
            this.PersonnelTargetSiteIdTb.Location = new System.Drawing.Point(136, 83);
            this.PersonnelTargetSiteIdTb.Name = "PersonnelTargetSiteIdTb";
            this.PersonnelTargetSiteIdTb.Size = new System.Drawing.Size(331, 20);
            this.PersonnelTargetSiteIdTb.TabIndex = 9;
            this.PersonnelTargetSiteIdTb.TextChanged += new System.EventHandler(this.PersonnelTargetSiteIdOnChange);
            // 
            // PersonnelTargetSiteIdLbel
            // 
            this.PersonnelTargetSiteIdLbel.AutoSize = true;
            this.PersonnelTargetSiteIdLbel.Location = new System.Drawing.Point(9, 87);
            this.PersonnelTargetSiteIdLbel.Name = "PersonnelTargetSiteIdLbel";
            this.PersonnelTargetSiteIdLbel.Size = new System.Drawing.Size(76, 13);
            this.PersonnelTargetSiteIdLbel.TabIndex = 8;
            this.PersonnelTargetSiteIdLbel.Text = "Target Site ID:";
            // 
            // PersonnelSourceSiteIdTb
            // 
            this.PersonnelSourceSiteIdTb.Location = new System.Drawing.Point(136, 53);
            this.PersonnelSourceSiteIdTb.Name = "PersonnelSourceSiteIdTb";
            this.PersonnelSourceSiteIdTb.Size = new System.Drawing.Size(331, 20);
            this.PersonnelSourceSiteIdTb.TabIndex = 7;
            this.PersonnelSourceSiteIdTb.TextChanged += new System.EventHandler(this.PersonnelSourceSiteIdOnChange);
            // 
            // PersonnelSourceSiteIdLbl
            // 
            this.PersonnelSourceSiteIdLbl.AutoSize = true;
            this.PersonnelSourceSiteIdLbl.Location = new System.Drawing.Point(9, 57);
            this.PersonnelSourceSiteIdLbl.Name = "PersonnelSourceSiteIdLbl";
            this.PersonnelSourceSiteIdLbl.Size = new System.Drawing.Size(79, 13);
            this.PersonnelSourceSiteIdLbl.TabIndex = 6;
            this.PersonnelSourceSiteIdLbl.Text = "Source Site ID:";
            // 
            // PersonnelClearResultBtn
            // 
            this.PersonnelClearResultBtn.Location = new System.Drawing.Point(12, 607);
            this.PersonnelClearResultBtn.Name = "PersonnelClearResultBtn";
            this.PersonnelClearResultBtn.Size = new System.Drawing.Size(98, 23);
            this.PersonnelClearResultBtn.TabIndex = 5;
            this.PersonnelClearResultBtn.Text = "Clear Results";
            this.PersonnelClearResultBtn.UseVisualStyleBackColor = true;
            this.PersonnelClearResultBtn.Click += new System.EventHandler(this.PersonnelClearBtnOnClick);
            // 
            // PersonnelResultsTb
            // 
            this.PersonnelResultsTb.Location = new System.Drawing.Point(12, 161);
            this.PersonnelResultsTb.Multiline = true;
            this.PersonnelResultsTb.Name = "PersonnelResultsTb";
            this.PersonnelResultsTb.ReadOnly = true;
            this.PersonnelResultsTb.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.PersonnelResultsTb.Size = new System.Drawing.Size(835, 435);
            this.PersonnelResultsTb.TabIndex = 4;
            // 
            // PersonnelResultsLbl
            // 
            this.PersonnelResultsLbl.AutoSize = true;
            this.PersonnelResultsLbl.Location = new System.Drawing.Point(9, 140);
            this.PersonnelResultsLbl.Name = "PersonnelResultsLbl";
            this.PersonnelResultsLbl.Size = new System.Drawing.Size(45, 13);
            this.PersonnelResultsLbl.TabIndex = 3;
            this.PersonnelResultsLbl.Text = "Results:";
            // 
            // PersonnelStartMigrationBtn
            // 
            this.PersonnelStartMigrationBtn.Location = new System.Drawing.Point(484, 21);
            this.PersonnelStartMigrationBtn.Name = "PersonnelStartMigrationBtn";
            this.PersonnelStartMigrationBtn.Size = new System.Drawing.Size(121, 23);
            this.PersonnelStartMigrationBtn.TabIndex = 2;
            this.PersonnelStartMigrationBtn.Text = "Start Migration";
            this.PersonnelStartMigrationBtn.UseVisualStyleBackColor = true;
            this.PersonnelStartMigrationBtn.Click += new System.EventHandler(this.PersonnelStartMigrationBtnOnClick);
            // 
            // PersonnelMigrationVersionLbl
            // 
            this.PersonnelMigrationVersionLbl.AutoSize = true;
            this.PersonnelMigrationVersionLbl.Location = new System.Drawing.Point(6, 26);
            this.PersonnelMigrationVersionLbl.Name = "PersonnelMigrationVersionLbl";
            this.PersonnelMigrationVersionLbl.Size = new System.Drawing.Size(124, 13);
            this.PersonnelMigrationVersionLbl.TabIndex = 1;
            this.PersonnelMigrationVersionLbl.Text = "Select Migration Version:";
            // 
            // PersonnelMigrationVersionCB
            // 
            this.PersonnelMigrationVersionCB.FormattingEnabled = true;
            this.PersonnelMigrationVersionCB.Location = new System.Drawing.Point(136, 23);
            this.PersonnelMigrationVersionCB.Name = "PersonnelMigrationVersionCB";
            this.PersonnelMigrationVersionCB.Size = new System.Drawing.Size(331, 21);
            this.PersonnelMigrationVersionCB.TabIndex = 0;
            this.PersonnelMigrationVersionCB.SelectedIndexChanged += new System.EventHandler(this.PersonnelMigrationCbOnChange);
            // 
            // MigrationToolForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(998, 702);
            this.Controls.Add(this.MainTabControl);
            this.Name = "MigrationToolForm";
            this.Text = "Form1";
            this.MainTabControl.ResumeLayout(false);
            this.DbConnectionTab.ResumeLayout(false);
            this.DbConnectionTab.PerformLayout();
            this.TargetPanel.ResumeLayout(false);
            this.TargetPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TargetConnectTimeoutSpinner)).EndInit();
            this.SourcePanel.ResumeLayout(false);
            this.SourcePanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SourceConnectTimeoutSpinner)).EndInit();
            this.MigrationToolTab.ResumeLayout(false);
            this.MigrationToolTab.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl MainTabControl;
        private System.Windows.Forms.TabPage DbConnectionTab;
        private System.Windows.Forms.TabPage MigrationToolTab;
        private System.Windows.Forms.Label SourceDbLbl;
        private System.Windows.Forms.Panel SourcePanel;
        private System.Windows.Forms.TextBox SourceDbNameTb;
        private System.Windows.Forms.Label SourceDbNameLbl;
        private System.Windows.Forms.TextBox SourceDbServerTb;
        private System.Windows.Forms.Label SourceDbServerLbl;
        private System.Windows.Forms.Label TargetDbLbl;
        private System.Windows.Forms.Panel TargetPanel;
        private System.Windows.Forms.TextBox TargetSecurityTb;
        private System.Windows.Forms.TextBox TargetDbServerTb;
        private System.Windows.Forms.Label TargetSecurityLbl;
        private System.Windows.Forms.Label TargetDbServerLbl;
        private System.Windows.Forms.Label TargetDbNameLbl;
        private System.Windows.Forms.TextBox TargetDbNameTb;
        private System.Windows.Forms.TextBox SourceSecurityTb;
        private System.Windows.Forms.Label SourceSecurityLbl;
        private System.Windows.Forms.Label SourceInSecondsLbl;
        private System.Windows.Forms.NumericUpDown SourceConnectTimeoutSpinner;
        private System.Windows.Forms.Label SourceConnectTimeoutLbl;
        private System.Windows.Forms.Button SourceTestConnectionBtn;
        private System.Windows.Forms.TextBox SourceTestConnectionTb;
        private System.Windows.Forms.Label TabDbConnectionConfigLbl;
        private System.Windows.Forms.Button DbConfigExitBtn;
        private System.Windows.Forms.Button DbConfigSaveBtn;
        private System.Windows.Forms.Button LoadConfigurationBtn;
        private System.Windows.Forms.Button TargetTestConnectionBtn;
        private System.Windows.Forms.TextBox TargetTestConnectionTb;
        private System.Windows.Forms.Label TargetInSecondsLbl;
        private System.Windows.Forms.NumericUpDown TargetConnectTimeoutSpinner;
        private System.Windows.Forms.Label TargetConnectTimeoutLbl;
        private System.Windows.Forms.Button PersonnelClearResultBtn;
        private System.Windows.Forms.TextBox PersonnelResultsTb;
        private System.Windows.Forms.Label PersonnelResultsLbl;
        private System.Windows.Forms.Button PersonnelStartMigrationBtn;
        private System.Windows.Forms.Label PersonnelMigrationVersionLbl;
        private System.Windows.Forms.ComboBox PersonnelMigrationVersionCB;
        private System.Windows.Forms.TextBox PersonnelTargetSiteIdTb;
        private System.Windows.Forms.Label PersonnelTargetSiteIdLbel;
        private System.Windows.Forms.TextBox PersonnelSourceSiteIdTb;
        private System.Windows.Forms.Label PersonnelSourceSiteIdLbl;
        private System.Windows.Forms.CheckBox PersonnelToolCb;
        private System.Windows.Forms.CheckBox EquipmentToolCb;
        private System.Windows.Forms.Label PersonnelSelectToolsLbl;
        private System.Windows.Forms.CheckBox QueriesToolCb;
        private System.Windows.Forms.CheckBox StationsToolCb;
        private System.Windows.Forms.CheckBox FootnoteToolCb;
        private System.Windows.Forms.CheckBox EquipmentTypeToolCb;
        private System.Windows.Forms.CheckBox QualificationToolCb;
        private System.Windows.Forms.CheckBox ApplicationStringToolCb;
    }
}


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;
using System.Windows.Forms;

using System.IO;
using Microsoft.VisualBasic.ApplicationServices;
using Microsoft.Win32;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Messaging; // For AsyncResult
using FMBUC;
using FMBU;
using System.Data.SqlClient;
//using System.Data.OleDb;
using FMUTILLib;
using FMSYSTEMMANAGERLib;

namespace FMBackupUtilityConfiguration
{
    public partial class MainForm : Form
    {
        const int FUELSMANAGER_SENTINEL_REVISION = 600;
        const int DEVELOPER_KEY = 9999;

        private bool bBURunning;
        private bool bSendingBackupRequest;

        bool bSecurityKey;
        bool bCanExit;
        bool bLogDirChanged;
        bool bSaveSize;
        bool bViewLog;
        int iFormLeft, iFormTop;
        int iFormWidthNoView, iFormWidthWithView;
        int iFormHeightNoView, iFormHeightWithView;
        FormWindowState wndStateBeforeMin;

        private int iIconIndex;
        private System.Drawing.Icon[] icons;
        
        string sLogFullPath;
        
        // ==================================================================================================
        // < BUC APPLICATION AS SERVER >
        // Server - BUC (this MainForm object)
        // Client - BU
        private FMBUCRemote roBUC; // The remote object created here.
        // Delegate for asynchronously running method in UI thread.
        private delegate void ProcessBUMessageDelegate(MessageEventArgs msgEventArgs); // Message from BU.
        // ==================================================================================================

        // ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        // < BU SERVICE AS REMOTE SERVER >
        // Server - BU 
        // Client - BUC (this MainForm object)
        private FMBURemote roBU; // The remote object created in BU.
        // Delegate for asynchronous call, same signature as FMBURemote.SendMessageToBU().
        private delegate void SendBUMessageDelegate(MessageToBUEventArgs.MsgType msgType);//, string sMessage);
        // ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        public bool IsBURunning
        {
            get { lock(this){ return bBURunning; } }
            set { lock(this){ bBURunning = value; } }
        }

        public bool IsSendingBackupRequest
        {
            get { lock(this){ return bSendingBackupRequest; } }
            set { lock(this){ bSendingBackupRequest = value; } }
        }

        public MainForm()
        {
            InitializeComponent();

            IsSendingBackupRequest = false;

            iIconIndex = 0;
            icons = new Icon[3];
            icons[0] = FMBackupUtilityConfiguration.Properties.Resources.DBsClock1;
            icons[1] = FMBackupUtilityConfiguration.Properties.Resources.DBsClock2;
            icons[2] = FMBackupUtilityConfiguration.Properties.Resources.DBsClock3;
            this.notifyIconBUC.Icon = icons[0];

            bSecurityKey = false;
            bCanExit = false;
            bLogDirChanged = false;
            bSaveSize = true;

            // Window size calculation.
            bViewLog = true;
            iFormLeft = this.Left; // Original value.
            iFormTop  = this.Top;  // Original value.
            iFormWidthWithView = this.Width; // Original value.
            iFormWidthNoView = this.Width - splitContainer1.Panel2.Width - 3;

            // FormBorderStyle.Sizable
            iFormHeightNoView = this.Height - 2; // Original value.
            iFormHeightWithView = this.Height; // Original value.

            this.MinimumSize = new Size(iFormWidthNoView + 150, this.Height);

            wndStateBeforeMin = FormWindowState.Normal;

            try
            {
                tbLogFileLocation.Text = Path.Combine(Application.StartupPath, "Log");
                tbZipFileLocation.Text = Path.Combine(Application.StartupPath, "Zip");
            }
            catch {}

            sLogFullPath = null;

            UpdateLogView();

            InitializeRemoting();
        }

        public void InitializeRemoting()
        {
//            System.Diagnostics.Trace.WriteLine("< InitializeRemoting >");

            if (RegistryReadBUCInstance()) return;

            // ==================================================================================================
            // < BUC APPLICATION AS SERVER >

            try
            {
                // Register a TCP channel.

                // Since default constructor of the channel creates a channel with a name "tcp", let's use a new name.
                System.Collections.IDictionary properties = new System.Collections.Hashtable();

                properties["port"] = 50905;
                properties["name"] = "BUCTcp";

                ChannelServices.RegisterChannel(new TcpChannel(properties, null, null), false);
                
                // Create a remotable object and register it with the remoting service.
                roBUC = new FMBUCRemote();
                ObjRef orFMBUCRemote = RemotingServices.Marshal(roBUC, "FMBUCRemote");
                
                // Subscribe to message event raised by BUC remote object.
                roBUC.MessageEvent += new FMBUCRemote.MessageEventHandler(roBUC_MessageEvent);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
//                EventLog.WriteEntry(ex.Message);
            }
            // ==================================================================================================


            // ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
            // < BU SERVICE REMOTE SERVER RELATED CODE >
            try
            {
                // Register a TCP channel.
//                ChannelServices.RegisterChannel(new TcpChannel(), false);

                // Since default constructor of the channel creates a channel with a name "tcp", let's use a new name.
                System.Collections.IDictionary prop = new System.Collections.Hashtable();
//                prop["port"] = 50906;
                prop["name"] = "BUTcp";

                ChannelServices.RegisterChannel(new TcpChannel(prop, null, null), false);

                roBU = (FMBURemote)Activator.GetObject(
                                          typeof(FMBURemote),
                                          "tcp://localhost:50906/FMBURemote");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
//                EventLog.WriteEntry(ex.Message);
            }
            // ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        }

        public void StartupNextInstanceHandler(object sender, StartupNextInstanceEventArgs e)
        {
            string[] commandLine = new string[e.CommandLine.Count];
            e.CommandLine.CopyTo(commandLine, 0);
            System.Diagnostics.Trace.WriteLine(DateTime.Now.ToShortTimeString() + commandLine[0]);

            this.Show();
        }

        // ==================================================================================================
        // < BUC APPLICATION AS SERVER >
        // MessageEvent handler.
        void roBUC_MessageEvent(object sender, MessageEventArgs e)
        {
            // Use the form's thread.
            this.BeginInvoke(new ProcessBUMessageDelegate(ProcessBUMessage), new object[]{e});
        }
        // ==================================================================================================
        
        #region Private Methods

        // ==================================================================================================
        // < BUC APPLICATION AS SERVER >
        private void ProcessBUMessage(MessageEventArgs msgEventArgs)
        {
            string str = "";
            switch (msgEventArgs.MessageType)
            {
                case MessageEventArgs.MsgType.MSG_STARTED:
                    IsBURunning = true;
                    str = String.Format("{0} {1}",
                                        msgEventArgs.EventDateTime.ToString("yyyy/MM/dd HH:mm:ss"),
                                        msgEventArgs.Message);
                    toolStripStatusLabelMsg.Text = str;
                    notifyIconBUC.ShowBalloonTip(1500,
                                                 "FuelsManager Backup Utility",
                                                 str,
                                                 ToolTipIcon.Info);
                    // Start tray icon animation.
                    timerNotifyIcon.Start();
                    break;

                case MessageEventArgs.MsgType.MSG_COMPLETE:
                    IsBURunning = false;

                    // Stop tray icon animation.
                    timerNotifyIcon.Stop();
                    // Show the regular icon.
                    this.notifyIconBUC.Icon = icons[0];

                    str = String.Format("{0} {1}",
                                        msgEventArgs.EventDateTime.ToString("yyyy/MM/dd HH:mm:ss"),
                                        msgEventArgs.Message);
                    toolStripStatusLabelMsg.Text = str;
                    notifyIconBUC.ShowBalloonTip(1500,
                                                 "FuelsManager Backup Utility",
                                                 str,
                                                 ToolTipIcon.Info);

                    UpdateLogView();
                    break;

                case MessageEventArgs.MsgType.MSG_FAIL:
                    IsBURunning = false;

                    // Stop tray icon animation.
                    timerNotifyIcon.Stop();
                    // Show the regular icon.
                    this.notifyIconBUC.Icon = icons[0];

                    str = String.Format("{0} {1}",
                                        msgEventArgs.EventDateTime.ToString("yyyy/MM/dd HH:mm:ss"),
                                        msgEventArgs.Message);
                    toolStripStatusLabelMsg.Text = str;

                    // Restore, bring to front, and activate window.
                    if (this.WindowState == FormWindowState.Minimized) RestoreMainWindow();

                    this.BringToFront();
                    this.Activate();
                    this.Refresh();

                    notifyIconBUC.ShowBalloonTip(1500,
                                                 "FuelsManager Backup Utility",
                                                 str,
                                                 ToolTipIcon.Info);

                    MessageBox.Show(this,
                                    str,
                                    "FuelsManager Backup Utility",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                    UpdateLogView();
                    break;

                case MessageEventArgs.MsgType.MSG_STATUS:
                    break;

                case MessageEventArgs.MsgType.MSG_ERROR:
                    break;
            }
        }
        // ==================================================================================================

        private bool IsSecurityKeyPresent(bool bDisplayMessage)
        {
            byte byFuelsManagerType = 0;
            int iKeyFound           = 0; // false
            ushort usProgramVersion = 0;

            try
            {
                FMAccessClass fmAccess = new FMAccessClass();

                fmAccess.GetIMType(ref byFuelsManagerType, ref iKeyFound, ref usProgramVersion);
                if (iKeyFound == 1) // true
                {
		            if (usProgramVersion != FUELSMANAGER_SENTINEL_REVISION &&
			            usProgramVersion != DEVELOPER_KEY)
		            {
                        if (bDisplayMessage)
                            MessageBox.Show(this,
                                            "Installed Hardware key is not for this version of FuelsManager.",
                                            "FuelsManager Backup Utility",
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Error);

			            return false;
		            }

                }
                else
                {
                    if (bDisplayMessage)
                        MessageBox.Show(this,
                                        "Hardware key not found.",
                                        "FuelsManager Backup Utility",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);

                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "FuelsManager Backup Utility",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        private void UpdateLogView()
        {
            RegistryReadLogFullPath();

            if (!File.Exists(sLogFullPath)) return;

            try
            {
                // Open the file in read-only mode.
                using (FileStream fs = new FileStream(sLogFullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
                    {
                        lvLog.BeginUpdate();
                        lvLog.Items.Clear();
                        
                        String str;
                        // Read and display lines from the file until the end of 
                        // the file is reached.
                        while ((str = sr.ReadLine()) != null) 
                        {
                            lvLog.Items.Add(str);
                        }

                        if (lvLog.Items.Count > 2)
                        {
                            lvLog.Items.Add("");
                            lvLog.Items.Add("Unclassified/For Official Use Only");
                            
                            lvLog.Items[0].Font = new Font(lvLog.Items[0].Font, 
                                                           lvLog.Items[0].Font.Style | FontStyle.Bold);

                            lvLog.Items[lvLog.Items.Count - 1].Font = new Font(lvLog.Items[0].Font, 
                                                           lvLog.Items[0].Font.Style | FontStyle.Bold);
                        }

 /*
                        if (lvLog.Items.Count > 0)
                        {
                            lvLog.Items.Insert(0, "Unclassified/For Official Use Only");
                            lvLog.Items.Add("Unclassified/For Official Use Only");
                            
                            lvLog.Items[0].Font = new Font(lvLog.Items[0].Font, 
                                                           lvLog.Items[0].Font.Style | FontStyle.Bold);

                            lvLog.Items[lvLog.Items.Count - 1].Font = new Font(lvLog.Items[0].Font, 
                                                           lvLog.Items[0].Font.Style | FontStyle.Bold);
                        }
*/
                        lvLog.EndUpdate();
                    }
                }
            }
            catch (Exception ex) 
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }
        }

        private void RestoreMainWindow()
        {
            this.Show();
            this.WindowState = wndStateBeforeMin;
        }

        // Save window size before hiding log view.
        private void SaveWindowSizeBeforeHideLog()
        {
            if (bSaveSize &&
                this.WindowState == FormWindowState.Normal &&
                this.FormBorderStyle == FormBorderStyle.Sizable)
            {
                if (this.Width > iFormWidthNoView)
                {
                    iFormWidthWithView = this.Width;
                }

                if (this.Height > iFormHeightNoView)
                {
                    iFormHeightWithView = this.Height;
                }
                else if (this.Height < iFormHeightNoView)
                {
                    iFormHeightWithView = iFormHeightNoView;
                }
            }
        }

        private void ReadWindowData()
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\FuelsManager\FMBackupUtility\Window");
            if (key != null)
            {
                using (key)
                {
                    object objL = key.GetValue("X");
                    object objT = key.GetValue("Y");
                    object objW = key.GetValue("ZXWidth");
                    object objH = key.GetValue("ZYHeight");

                    // If any of the numbers is invalid, don't change the defaults.
                    
                    if (objL == null || objT == null || objW == null || objH == null) return;

                    int iLeft = (int)objL;
                    int iTop = (int)objT;
                    int iWidth = (int)objW;
                    int iHeight = (int)objH;

                    System.Drawing.Rectangle workingRectangle = Screen.PrimaryScreen.WorkingArea;

                    if (iLeft + iWidth < workingRectangle.Left + 20 ||
                        iLeft > (workingRectangle.Right - 20)) return;
//
                    if (iTop + iHeight < workingRectangle.Top + 20 ||
                        iTop > (workingRectangle.Bottom - 40)) return;

                    if (iWidth < this.MinimumSize.Width || iWidth > workingRectangle.Width) return;

                    if (iHeight < this.MinimumSize.Height || iHeight > workingRectangle.Height) return;

                    iFormLeft = iLeft;
                    iFormTop = iTop;
                    iFormWidthWithView = iWidth;
                    iFormHeightWithView = iHeight;
                    
                    this.SetBounds(iLeft, iTop, iWidth, iHeight);
                }
            }
        }

        private void WriteWindowData()
        {
            SaveWindowSizeBeforeHideLog();

            using (RegistryKey key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\FuelsManager\FMBackupUtility\Window"))
            {
                key.SetValue("X", iFormLeft, RegistryValueKind.DWord);
                key.SetValue("Y", iFormTop, RegistryValueKind.DWord);
                key.SetValue("ZXWidth", iFormWidthWithView, RegistryValueKind.DWord);
                key.SetValue("ZYHeight", iFormHeightWithView, RegistryValueKind.DWord);
            }
        }

        private bool RegistryReadBUCInstance()
        {
            int iExist = 0;
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\FuelsManager\FMBackupUtility\Window");
            if (key != null)
            {
                using (key)
                {
                    object obj = key.GetValue("BUC", 0);
                    if (obj != null) iExist = (int)obj;
                }
            }
            return (iExist == 1 ? true : false);
        }

        private void RegistryWriteBUCInstance(bool bRunning)
        {
            using (RegistryKey key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\FuelsManager\FMBackupUtility\Window"))
            {
                key.SetValue("BUC", (bRunning ? 1 : 0), RegistryValueKind.DWord);
            }
        }

        private void ReadConfiguration()
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\FuelsManager\FMBackupUtility");
            if (key != null)
            {
                using (key)
                {
                    object obj = key.GetValue("Ticks");
                    if (obj != null)
                    {
                        Int64 i64Val;
                        Int64.TryParse((string)obj, System.Globalization.NumberStyles.Integer, null, out i64Val);

                        // The "Ticks" value represents the time of day.
                        TimeSpan tsTimeOfDay = new TimeSpan(i64Val);

                        // If registry data is invalid, default to 1:00 AM.
                        if (tsTimeOfDay < TimeSpan.Zero || tsTimeOfDay > TimeSpan.FromDays(1))
                            tsTimeOfDay = TimeSpan.FromHours(1);

                        DateTime dt = DateTime.Today + tsTimeOfDay;

                        dtpStartTime.Value = dt;
                    }

                    obj = key.GetValue("LogFilePath");
                    if (obj != null) tbLogFileLocation.Text = (string)obj;

                    obj = key.GetValue("ZipFilePath");
                    if (obj != null) tbZipFileLocation.Text = (string)obj;

                    lbFilesLocations.Items.Clear();
                    obj = key.GetValue("AdditionalFilesPaths");
                    if (obj != null)
                    {
                        string[] sPaths = (string[])obj;
                        
                        for (int i = 0; i < sPaths.Length; i++)
                        {
                            if (!String.IsNullOrEmpty(sPaths[i])) lbFilesLocations.Items.Add(sPaths[i]);
                        }

                        if (lbFilesLocations.Items.Count > 0) lbFilesLocations.TopIndex = lbFilesLocations.Items.Count - 1;
                    }
                }
            }
        }

        private void WriteConfiguration()
        {
            using (RegistryKey key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\FuelsManager\FMBackupUtility"))
            {
                key.SetValue("Ticks", dtpStartTime.Value.TimeOfDay.Ticks.ToString(), RegistryValueKind.String);

                key.SetValue("LogFilePath", tbLogFileLocation.Text, RegistryValueKind.String);
                key.SetValue("ZipFilePath", tbZipFileLocation.Text, RegistryValueKind.String);

                if (lbFilesLocations.Items.Count > 0)
                {
                    string[] sPaths = new string[lbFilesLocations.Items.Count];
                    
                    for (int i = 0; i < lbFilesLocations.Items.Count; i++)
                    {
                        sPaths[i] = lbFilesLocations.Items[i].ToString();
                    }
                    key.SetValue("AdditionalFilesPaths", sPaths, RegistryValueKind.MultiString);
                }
                else
                {
                    key.SetValue("AdditionalFilesPaths", new string[]{""}, RegistryValueKind.MultiString);
                }
            }
            if (bLogDirChanged)
            {
                bLogDirChanged = false;
                MoveLogFile();
            }
        }

        // This can only be called when change is confirmed.
        private void MoveLogFile()
        {
            RegistryReadLogFullPath();

            try
            {
                if (File.Exists(sLogFullPath))
                {
                    if (!Directory.Exists(tbLogFileLocation.Text))
                        Directory.CreateDirectory(tbLogFileLocation.Text);

                    string sFileName = Path.GetFileName(sLogFullPath);
                    string sNewLogFullPath = Path.Combine(tbLogFileLocation.Text, sFileName); 
                    File.Move(sLogFullPath, sNewLogFullPath);
                    sLogFullPath = sNewLogFullPath;
                    RegistryWriteLogFullPath();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }
        }

        private void RegistryReadLogFullPath()
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\FuelsManager\FMBackupUtility");
            if (key != null)
            {
                using (key)
                {
                    object obj = key.GetValue("LogFileFullPath");
                    if (obj != null) sLogFullPath = (string)obj;
                }
            }
        }

        private void RegistryWriteLogFullPath()
        {
            using (RegistryKey key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\FuelsManager\FMBackupUtility"))
            {
                key.SetValue("LogFileFullPath", sLogFullPath, RegistryValueKind.String);
            }
        }

        private void EnableControls(bool bEnable)
        {
            btnLogIn.Enabled  = !bEnable;
            dtpStartTime.Enabled = bEnable;
            btnBackUpNow.Enabled = bEnable;
            btnBrowseLogLocation.Enabled = bEnable;

            if (bEnable)
            {
                if (lbFilesLocations.Items.Count > 0) btnRemove.Enabled = bEnable;
                dtpStartTime.Select(); // Set focus on the next control after disabling Log in button.
            }
            else                                      btnRemove.Enabled = bEnable;

            btnBrowseFilesLocation.Enabled = bEnable;
            btnZipLocation.Enabled = bEnable;
            this.Refresh();
        }

        private void UpdateControlStatusOnDataChange(bool bEnable)
        {
            btnApply.Enabled = bEnable;
            btnBackUpNow.Enabled = !bEnable;
            this.Refresh();
        }

        private void OpenHelpFile()
        {
            string sDir = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).DirectoryName;
            string sFullPath = Path.Combine(sDir, "FMBackupUtility.chm");
            Help.ShowHelp(this, sFullPath);
        }

        #endregion

        #region BU Service As Remoting Server

        // ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        // < BU SERVICE AS REMOTE SERVER >
        private void SendBUMessage(MessageToBUEventArgs.MsgType msgType)
        {
            // Asynchronous remote call to BU Service.
            AsyncCallback callback = new AsyncCallback(this.SendBUMessageCallBack);
            SendBUMessageDelegate del = new SendBUMessageDelegate(roBU.SendMessageToBU);
            IAsyncResult ar = del.BeginInvoke(msgType, /*sMsg,*/ callback, this);
        }

        // Callback method that is called when SendBUMessageDelegate completes its async call.
        private void SendBUMessageCallBack(IAsyncResult ar)
        {
            // Obtains the last parameter of the delegate call.
            MainForm mainform = (MainForm)ar.AsyncState;
            // Get the delegate object on which the asynchronous call was invoked.
            SendBUMessageDelegate del = (SendBUMessageDelegate)((AsyncResult)ar).AsyncDelegate;
            
            try
            {
                del.EndInvoke(ar); // No return value.
            }
            catch (Exception ex)
            {
                // BU Service Remoting Server is not available.

                System.Diagnostics.Trace.WriteLine(ex.Message);

                if (IsSendingBackupRequest)
                {
                    MessageBox.Show(this,
                                    "Could not communicate with the Backup Utility service.\nMake sure that the service is running.",
                                    "FuelsManager Backup Utility",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Exclamation);
                }
            }
            finally
            {
                IsSendingBackupRequest = false;
            }
        }
        // ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        #endregion

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            bSecurityKey = IsSecurityKeyPresent(true);
            this.Cursor = Cursors.Default;
            if (!bSecurityKey)
            {
                this.Close();
                return;
            }

            ReadWindowData();
            ReadConfiguration();

            RegistryWriteBUCInstance(true);

            // For some reason, the App appears in the taskbar eventhough Hide()
            // was called in MainForm_Resize(), so we call it again here.
            this.Hide();

            this.notifyIconBUC.Visible = true;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Let the OS close this application.
            if (e.CloseReason == CloseReason.WindowsShutDown) return;

            // Security key not present, return to close the form.
            if (!bSecurityKey) return;

            if (!bCanExit)
            {
                // User clicked on Close button in top right corner.
                // Implement this case like Minimize button.
            
                e.Cancel = true; // Do not close form.
                this.WindowState = FormWindowState.Minimized;
/*
                // If data have changed, ask user if he wants to save.
                // Yes - implement like OK handler.
                // No  - implement like Cancel handler.
                if (btnApply.Enabled == true) // Indicates configuration has been changed.
                {
                    DialogResult result;
                    result = MessageBox.Show(this,
                                             "Do you want to save the configuration data?",
                                             "FuelsManager Backup Utility",
                                             MessageBoxButtons.YesNo,
                                             MessageBoxIcon.Question);
                    switch (result)
                    {
                        case DialogResult.Yes:      // Same as selecting OK button.
                            // Exit configure mode.
                            EnableControls(false);
                            this.WindowState = FormWindowState.Minimized;

                            btnApply.Enabled = false;
                            btnApply.Refresh();
                            this.Cursor = Cursors.WaitCursor;

                            WriteConfiguration();

                            SendBUMessage(MessageToBUEventArgs.MsgType.MSG_UPDATECONFIG);
                            this.Cursor = Cursors.Default;

                            return;

                        case DialogResult.No: // Don't save, proceed to the code below.
                        default:
                            break;
                    }
                }

                // Exit configure mode.
                EnableControls(false);
                this.WindowState = FormWindowState.Minimized;

                if (btnApply.Enabled == true) // Indicates configuration has been changed.
                {            
                    btnApply.Enabled = false;
                    btnApply.Refresh();

                    // Read registry to restore configuration.
                    ReadConfiguration();
                }
*/
            }   // End of "if (!bCanExit)"
            else
            {
                // User clicked on Exit menuitem in tray icon popup menu.

                if (btnApply.Enabled == true) // Indicates configuration has been changed.
                {
                    DialogResult result;
                    result = MessageBox.Show(this,
                                             "Do you want to save the configuration data?",
                                             "FuelsManager Backup Utility",
                                             MessageBoxButtons.YesNoCancel,
                                             MessageBoxIcon.Question);
                    switch (result)
                    {
                        case DialogResult.Yes:
                            // Exit configure mode.
                            EnableControls(false);
                            btnApply.Enabled = false;
                            btnApply.Refresh();
                            this.Cursor = Cursors.WaitCursor;

                            WriteConfiguration();

                            SendBUMessage(MessageToBUEventArgs.MsgType.MSG_UPDATECONFIG);
                            this.Cursor = Cursors.Default;

                            break;

                        case DialogResult.Cancel: // Do not proceed with canceling the main app.
                            e.Cancel = true;      // Do not close form.
                            bCanExit = false;
                            return;

                        case DialogResult.No: // Don't save, proceed to the code below.
                        default:
                            break;
                    }
                }
                WriteWindowData();
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            RegistryWriteBUCInstance(false);
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            switch (this.WindowState)
            {
                case FormWindowState.Minimized:
                    this.Hide();
                    break;

                case FormWindowState.Maximized:
                    btnViewLog.Enabled = false;
                    wndStateBeforeMin = this.WindowState;
                    break;

                default:
                    btnViewLog.Enabled = true;
                    wndStateBeforeMin = this.WindowState;
                    SaveWindowSizeBeforeHideLog();
                    break;
            }
        }

        // Tray Icon - Mouse Double Click.
        private void notifyIconBUC_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            RestoreMainWindow();
        }

        // Tray Icon - Open Backup Utility Configuration menu item.
        private void tsmiOpenBackupUtilityConfiguration_Click(object sender, EventArgs e)
        {
            RestoreMainWindow();
        }

        // Tray Icon - Help menu item.
        private void tsmiHelp_Click(object sender, EventArgs e)
        {
            OpenHelpFile();
        }

        // Tray Icon - About menu item.
        private void tsmiAbout_Click(object sender, EventArgs e)
        {
            tsmiAbout.Enabled = false;
            AppAboutBox about = new AppAboutBox();

            if (this.WindowState == FormWindowState.Minimized)
                about.StartPosition = FormStartPosition.CenterScreen;

            about.ShowDialog(this);
            tsmiAbout.Enabled = true;
        }

        // Tray Icon - Exit menu item.
        private void tsmiExit_Click(object sender, EventArgs e)
        {
            bCanExit = true;
            this.Close();
        }

        // Timer for Tray Icon animation.
        private void timerNotifyIcon_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.notifyIconBUC.Icon = icons[iIconIndex];
            iIconIndex++;
            if (iIconIndex > 2) iIconIndex = 0;
        }

        private void btnViewLog_Click(object sender, EventArgs e)
        {
            bViewLog = !bViewLog;
            if (bViewLog)
            {
                // Changing from NO log view to log view (from fix border to sizeable border).

                bSaveSize = false;
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
                bSaveSize = true;
                
                this.Size = new Size(iFormWidthWithView, iFormHeightWithView);
                this.MaximizeBox = true;
                this.statusStripMain.SizingGrip = true;

                this.MinimumSize = new Size(iFormWidthNoView + 150, iFormHeightNoView + 2);

                this.btnViewLog.Text = "<< Hide Log";
            }
            else
            {
                // Changing from log view to NO log view (from sizeable border to fix border).

                this.MinimumSize = new Size(iFormWidthNoView, iFormHeightNoView);

                // Save window size before hiding log view.
                SaveWindowSizeBeforeHideLog();

                // Resize window to hide log view.
                this.Size = new Size(iFormWidthNoView, iFormHeightNoView);
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
                this.MaximizeBox = false;
                this.statusStripMain.SizingGrip = false;

                this.btnViewLog.Text = "View Log >>";
            }
        }
/*
        // Log In
        public static string getConnectionString(string userID, string db)
        {
//            SqlConnectionStringBuilder connectionString = new SqlConnectionStringBuilder("Data Source = localhost; Initial Catalog = ConsolidatedDB;");
            SqlConnectionStringBuilder connectionString = new SqlConnectionStringBuilder("Data Source = localhost;");
            connectionString.Add("Initial Catalog", db);
            connectionString.Add("Integrated Security", "SSPI");
//            connectionString.Add("Integrated Security", "false");
            connectionString.Add("Network Library", "dbmssocn");
            connectionString.Add("pwd", getDBPassword(userID));
            connectionString.Add("User ID", userID);
//            connectionString.AsynchronousProcessing = true;
            return connectionString.ToString();
        }

        // Log In
        static public string getDBPassword(string userID)
        {
            // Algorithm is to take a SHA-1 hash of the bytes of the ASCII representation
            // of the user ID followed by the bytes of the salt "{01AFEBD3-78CD-4B15-AB9B-F4AA1C0E2D9B}"
            ASCIIEncoding encoding = new ASCIIEncoding();
            System.Security.Cryptography.SHA1 sha = new System.Security.Cryptography.SHA1CryptoServiceProvider();

            // Split out for obfuscation purposes
            // Probably something more thorough required later

            //Updated to ensure that UserID is always uppercase.
            //resolves CSI #5049
            StringBuilder newData = new StringBuilder(userID.ToUpper());
            newData.Append('{');
            newData.Append('0');
            newData.Append('1');
            newData.Append('A');
            newData.Append('F');
            newData.Append('E');
            newData.Append('B');
            newData.Append('D');
            newData.Append('3');
            newData.Append('-');
            newData.Append('7');
            newData.Append('8');
            newData.Append('C');
            newData.Append('D');
            newData.Append('-');
            newData.Append('4');
            newData.Append('B');
            newData.Append('1');
            newData.Append('5');
            newData.Append('-');
            newData.Append('A');
            newData.Append('B');
            newData.Append('9');
            newData.Append('B');
            newData.Append('-');
            newData.Append('F');
            newData.Append('4');
            newData.Append('A');
            newData.Append('A');
            newData.Append('1');
            newData.Append('C');
            newData.Append('0');
            newData.Append('E');
            newData.Append('2');
            newData.Append('D');
            newData.Append('9');
            newData.Append('B');
            newData.Append('}');
            byte[] userIDBytes = encoding.GetBytes(newData.ToString());
            //byte[]	saltBytes = encoding.GetBytes("{01AFEBD3-78CD-4B15-AB9B-F4AA1C0E2D9B}");

            byte[] pwdBytes = sha.ComputeHash(userIDBytes);

            newData.Length = 0;
            foreach (byte pwdByte in pwdBytes)
            {
                newData.Append(pwdByte.ToString("x2")); // x indicates hexidecimal integer, 2 (the precision) is
                // the minimum number of digits.  Output will be zero
                // padded on the left as necessary
            }
            return newData.ToString();
        }

        public static void SqlConnect()
        {
            // Create an empty SqlConnection object.
            using (SqlConnection con = new SqlConnection())
            {
                // Configure the SqlConnection object's connection string.
                con.ConnectionString = getConnectionString("FMDService", "ConsolidatedDB");
//                    @"Data Source=127.0.0.1;" + // local SQL Server instance
//                    "Database=ConsolidatedDB;" +        // the DB
//                    "Integrated Security=SSPI";    // integrated Windows security

                // Open the database connection.
                con.Open();

                // Display information about the connection.
                if (con.State == ConnectionState.Open)
                {
                    System.Diagnostics.Trace.WriteLine("SqlConnection Information:");
                    System.Diagnostics.Trace.WriteLine(String.Format("  Connection State = {0}", con.State));
                }
                else
                {
                    System.Diagnostics.Trace.WriteLine("SqlConnection failed to open.");
                    System.Diagnostics.Trace.WriteLine(String.Format("  Connection State = {0}", con.State));
                }
                // At the end of the using block Dispose() calls Close().
            }
        }
*/
/*
        // Log In
        private bool WindowsAuthenticate()
        {
            bool bSuccess = false;
            // Create an empty SqlConnection object.
            using (SqlConnection con = new SqlConnection())
            {
                // Configure the SqlConnection object's connection string.
                con.ConnectionString = @"Data Source = localhost;" +      // local SQL Server instance
                                        "Database = ConsolidatedDB;" +    // the DB
                                        "Integrated Security = SSPI";     // integrated Windows security
                try
                {
                    // Open the database connection.
                    con.Open();

                    // Display information about the connection.
                    if (con.State == ConnectionState.Open)
                    {
                        bSuccess = true;
                    }
                    else
                    {
                        System.Diagnostics.Trace.WriteLine("SqlConnection failed to open.");
//                        System.Diagnostics.Trace.WriteLine(String.Format("  Connection State = {0}", con.State));
                    }
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(String.Format("Could not WindowsAuthenticate: {0}", ex.Message));
                }

                // At the end of the using block Dispose() calls Close().
            }
            return bSuccess;
        }
*/
        // Validate user against Inventory Management.
        private bool IMAuthenticate()
        {
            bool bSuccess = false;

            try
            {
                FMSYSTEMMANAGERLib.SECURITY_DATA sd;
                FMSecurityServerClass fmSecuritySvr = new FMSecurityServerClass();
                fmSecuritySvr.GetSecurityData(Environment.UserName, out sd);
                
                if (sd.SystemPermissions.bConfigure_Database == 1) bSuccess = true;
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(String.Format("Could not IMAuthenticate: {0}", ex.Message));
            }
            return bSuccess;
        }

        private void btnLogIn_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            bSecurityKey = IsSecurityKeyPresent(true);
            this.Cursor = Cursors.Default;
            if (!bSecurityKey)
            {
                this.Close();
                return;
            }

//            if (!WindowsAuthenticate())
            if (!IMAuthenticate())
            {
                // Accounting Database (ConsolidatedDB) Authentication.

                LoginDialogForm dlg = new LoginDialogForm();
                dlg.ShowDialog(this);
                if (!dlg.IsLoggedIn) return;
            }
            EnableControls(true);
        }

        private void btnBackUpNow_Click(object sender, EventArgs e)
        {
            if (IsBURunning)
            {
                MessageBox.Show("A database backup operation is in progress.",
                                "FuelsManager Backup Utility", MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);
                return;
            }

            this.Cursor = Cursors.WaitCursor;
            IsSendingBackupRequest = true;
            SendBUMessage(MessageToBUEventArgs.MsgType.MSG_BACKUPNOW);
            this.Cursor = Cursors.Default;
        }

        private void btnBrowseLogLocation_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDlg = new FolderBrowserDialog();
            folderBrowserDlg.Description = "Select the folder to save the log file.";
//            folderBrowserDlg.ShowNewFolderButton = false;
            folderBrowserDlg.RootFolder = Environment.SpecialFolder.MyComputer;//.Personal;
            folderBrowserDlg.SelectedPath = tbLogFileLocation.Text;

            DialogResult result = folderBrowserDlg.ShowDialog();
            if (result == DialogResult.OK)
            {
                if (tbLogFileLocation.Text.CompareTo(folderBrowserDlg.SelectedPath) != 0)
                {
                    tbLogFileLocation.Text = folderBrowserDlg.SelectedPath;
                    UpdateControlStatusOnDataChange(true);
                    bLogDirChanged = true;
                }
            }

        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lbFilesLocations.SelectedIndices.Count > 0 &&
                MessageBox.Show("Do you want to remove the selected folder(s)?",
                            "FuelsManager Backup Utility", MessageBoxButtons.OKCancel,
                            MessageBoxIcon.Exclamation) == DialogResult.OK)
            {
                while (lbFilesLocations.SelectedIndices.Count > 0)
                    lbFilesLocations.Items.RemoveAt(lbFilesLocations.SelectedIndices[0]);

                if (lbFilesLocations.Items.Count < 1) btnRemove.Enabled = false;
                UpdateControlStatusOnDataChange(true);
            }
        }

        private void btnBrowseFilesLocation_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDlg = new FolderBrowserDialog();
            folderBrowserDlg.Description = "Select the folder that contains additional files to back up.";
            folderBrowserDlg.ShowNewFolderButton = false;
            folderBrowserDlg.RootFolder = Environment.SpecialFolder.MyComputer;//.Personal;
//            folderBrowserDlg.SelectedPath = Application.StartupPath;

            DialogResult result = folderBrowserDlg.ShowDialog();
            if (result == DialogResult.OK)
            {
                if (folderBrowserDlg.SelectedPath.Length > 0)
                {
                    lbFilesLocations.Items.Add(folderBrowserDlg.SelectedPath);
                    lbFilesLocations.TopIndex = lbFilesLocations.Items.Count - 1;
                    btnRemove.Enabled = true;
                    UpdateControlStatusOnDataChange(true);
                }
            }
        }

        private void btnZipLocation_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDlg = new FolderBrowserDialog();
            folderBrowserDlg.Description = "Select the folder to save the zip file.";
//            folderBrowserDlg.ShowNewFolderButton = false;
//            folderBrowserDlg.RootFolder = Environment.SpecialFolder.Personal;
            folderBrowserDlg.SelectedPath = tbZipFileLocation.Text;

            DialogResult result = folderBrowserDlg.ShowDialog();
            if (result == DialogResult.OK)
            {
                if (tbZipFileLocation.Text.CompareTo(folderBrowserDlg.SelectedPath) != 0)
                {
                    tbZipFileLocation.Text = folderBrowserDlg.SelectedPath;
                    UpdateControlStatusOnDataChange(true);
                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            // Exit configure mode.
            EnableControls(false);
            this.WindowState = FormWindowState.Minimized;

            if (btnApply.Enabled == true) // Indicates configuration has been changed.
            {            
                btnApply.Enabled = false;
                btnApply.Refresh();
                this.Cursor = Cursors.WaitCursor;

                WriteConfiguration();

                SendBUMessage(MessageToBUEventArgs.MsgType.MSG_UPDATECONFIG);
                this.Cursor = Cursors.Default;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (btnApply.Enabled == true) // Indicates configuration has been changed.
            {
                DialogResult result;
                result = MessageBox.Show(this,
                                         "Do you want to save the configuration data?",
                                         "FuelsManager Backup Utility",
                                         MessageBoxButtons.YesNo,
                                         MessageBoxIcon.Question);
                switch (result)
                {
                    case DialogResult.Yes:      // Same as selecting OK button.
                        // Exit configure mode.
                        EnableControls(false);
                        this.WindowState = FormWindowState.Minimized;

                        btnApply.Enabled = false;
                        btnApply.Refresh();
                        this.Cursor = Cursors.WaitCursor;

                        WriteConfiguration();

                        SendBUMessage(MessageToBUEventArgs.MsgType.MSG_UPDATECONFIG);
                        this.Cursor = Cursors.Default;

                        return;

//                    case DialogResult.Cancel: // Do not proceed with canceling the main app.
//                        return;

                    case DialogResult.No: // Don't save, proceed to the code below.
                    default:
                        break;
                }
            }

            // Exit configure mode.
            EnableControls(false);
            this.WindowState = FormWindowState.Minimized;

            if (btnApply.Enabled == true) // Indicates configuration has been changed.
            {            
                btnApply.Enabled = false;
                btnApply.Refresh();

                // Read registry to restore configuration.
                ReadConfiguration();
            }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            btnApply.Enabled = false;
            this.Refresh();
            this.Cursor = Cursors.WaitCursor;
            
            // Exit configure mode.
//            EnableControls(false);

            WriteConfiguration();

            SendBUMessage(MessageToBUEventArgs.MsgType.MSG_UPDATECONFIG);

            btnBackUpNow.Enabled = true;
            this.Refresh();

            this.Cursor = Cursors.Default;
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            OpenHelpFile();
        }

        private void Doc_PrintPage(object sender, PrintPageEventArgs e)
        {
            // Retrieve the document that sent this event.
            TextDocument doc = (TextDocument)sender;

            // Define the font and determine the line height.
            using (Font font = new Font("Arial", 10))
            {
                float lineHeight = font.GetHeight(e.Graphics);

                // Create variables to hold position on page.
                float x = e.MarginBounds.Left;
                float y = e.MarginBounds.Top;

                // Increment the page counter (to reflect the page that 
                // is about to be printed).
                doc.PageNumber += 1;

                // Print all the information that can fit on the page.        
                // This loop ends when the next line would go over the
                // margin bounds, or there are no more lines to print.

                while ((y + lineHeight) < e.MarginBounds.Bottom &&
                  doc.Offset <= doc.Text.GetUpperBound(0))
                {
                    e.Graphics.DrawString(doc.Text[doc.Offset], font,
                      Brushes.Black, x, y);

                    // Move to the next line of data.
                    doc.Offset += 1;

                    // Move the equivalent of one line down the page.
                    y += lineHeight;
                }

                if (doc.Offset < doc.Text.GetUpperBound(0))
                {
                    // There is still at least one more page.
                    // Signal this event to fire again.
                    e.HasMorePages = true;
                }
                else
                {
                    // Printing is complete.
                    doc.Offset = 0;
                }
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (lvLog.Items.Count > 0)
            {
                // Create a document with 100 lines.
                string[] printText = new string[lvLog.Items.Count];

                for (int i = 0; i < printText.Length; i++)
                {
                    printText[i] = lvLog.Items[i].Text;
                }

                PrintDocument doc = new TextDocument(printText);
                doc.PrintPage += this.Doc_PrintPage;

                PrintDialog dlgSettings = new PrintDialog();
                dlgSettings.Document = doc;

                // If the user clicked OK, print the document.
                if (dlgSettings.ShowDialog() == DialogResult.OK)
                {
                    doc.Print();
                }
            }
            else
            {
            }
        }

        private void lbFilesLocations_MouseMove(object sender, MouseEventArgs e)
        {
            // Show a tooltip.
            string sTip = "";

            int iIndex = lbFilesLocations.IndexFromPoint(e.Location);
            if ((iIndex >= 0) && (iIndex < lbFilesLocations.Items.Count))
            {
                sTip = lbFilesLocations.Items[iIndex].ToString();
                toolTipMain.SetToolTip(lbFilesLocations, sTip);
            }
        }

        private void MainForm_LocationChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                iFormLeft = this.Left;
                iFormTop = this.Top;
            }
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
//            System.Diagnostics.Trace.WriteLine("<MainForm_Shown.>");
        }

        private void dtpStartTime_ValueChanged(object sender, EventArgs e)
        {
            if (dtpStartTime.Enabled == true)
            {
                UpdateControlStatusOnDataChange(true);
            }
        }

    }
}
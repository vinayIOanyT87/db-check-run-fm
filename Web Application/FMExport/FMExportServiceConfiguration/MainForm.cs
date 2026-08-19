// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainForm.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the main form for the FMExport service configuration utility.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMExportServiceConfiguration
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Windows.Forms;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.Exceptions;
    using FMBusinessObjects.UtilityObjects;
    using FMBusinessObjects.Constants;

    using FMExportService;

    public partial class MainForm : Form
    {
        /// <summary>
        /// Indicates whether any changes have been made that have not been saved
        /// </summary>
        private bool hasUnsavedChanges;

        private bool disregardCompanySelect;

        private List<string> listOfSelectedCompanies;

        /// <summary>
        /// The identity guid of the currently selected request
        /// </summary>
        private Guid selectedExportRequestIdentityGuid = Guid.Empty;

        /// <summary>
        /// The FuelsManager security object
        /// </summary>
        private SecurityClass security;

        /// <summary>
        /// Constructor for the form
        /// </summary>
        public MainForm()
        {
            this.InitializeComponent();
            this.hasUnsavedChanges = false;
            this.disregardCompanySelect = false;
            this.listOfSelectedCompanies = new List<string>();
            this.RequestGrid.AutoGenerateColumns = false;
        }

        /// <summary>
        /// The WCF binding type we should use, e.g. netTcpBinding
        /// </summary>
        public static string BindingType
        {
            get
            {
                return AppSettingsHelper.GetKeyValue("FMExportServiceBindingType", string.Empty);
            }
        }

        /// <summary>
        /// The WCF binding configuration we should use
        /// </summary>
        public static string BindingConfiguration
        {
            get
            {
                return AppSettingsHelper.GetKeyValue("FMExportServiceBindingConfiguration", string.Empty);
            }
        }

        /// <summary>
        /// The address of the FMExportService
        /// </summary>
        public static string FMExportServiceAddress
        {
            get
            {
                return AppSettingsHelper.GetKeyValue("FMExportServiceAddress", string.Empty);
            }
        }

        /// <summary>
        /// Enable or disable the controls on the screen. When the form is ReadOnly,
        /// only the New and Refresh buttons should be enabled.
        /// </summary>
        private bool ReadOnly
        {
            set
            {
                this.txtRequestName.Enabled = !value;
                this.txtCompanyCode.Enabled = !value;
                this.txtOwnerCode.Enabled = !value;
                this.txtRowVersion.Enabled = !value;
                this.dtpExportTime.Enabled = !value;
                this.txtExportFrequency.Enabled = !value;
                this.dtBaselineDate.Enabled = !value;
                this.chkExcludeEmptyFiles.Enabled = !value;
                this.cmbInterfaceNames.Enabled = !value;
                this.lbCompanies.Enabled = !value;
                cmbSendMethod.Enabled = !value;
                this.ApplyButton.Enabled = !value;
                this.DeleteButton.Enabled = !value;
            }
        }

        /// <summary>
        /// Retrieve requests from the database and display them on the grid
        /// </summary>
        private void GetRequests()
        {
            this.RequestGrid.DataSource = FMChannelHelper.MakeCall<IFMExportService, List<ExportRequestClass>>(
                BindingType,
                BindingConfiguration,
                FMExportServiceAddress,
                exportRequests => exportRequests.GetRequests(this.security));

            // If there are no rows in the grid, reset the controls to their initial states
            if (this.RequestGrid.Rows.Count <= 0)
            {
                this.ResetControls();
            }
        }

        private void InitializeCompaniesList(bool unSelectAll)
        {
            if (unSelectAll)
            {
                this.ClearSelectedCompanies();
                return;
            }

            var cmd = new SerializableSqlCommand("SELECT DISTINCT ID FROM tblCompanies ORDER BY ID");
            DataTable table = FMChannelHelper.MakeCall<IFMExportService, DataTable>(
                BindingType,
                BindingConfiguration,
                FMExportServiceAddress,
                exportRequests => exportRequests.GetDataTable(this.security, cmd));

            this.ClearCompanies();
            this.lbCompanies.Items.Add(Constants.AllCustomers);
            foreach (DataRow row in table.Rows)
            {
                string companyId = FMBusinessObjects.DataObjects.DataObject.getValue(row["ID"], string.Empty);
                if (!string.IsNullOrWhiteSpace(companyId))
                {
                    this.lbCompanies.Items.Add(companyId);
                }
            }
        }

        private void PopulateInterfaceIDs()
        {
            this.cmbInterfaceNames.Items.Clear();
            this.cmbInterfaceNames.Items.Add(string.Empty);

            List<string> supportedInterfaceIDs =
                FMChannelHelper.MakeCall<IFMExportService, List<string>>(
                    BindingType,
                    BindingConfiguration,
                    FMExportServiceAddress,
                    exportService => exportService.GetSupportedInterfaceIDs(this.security));

            supportedInterfaceIDs.ForEach(interfaceID => this.cmbInterfaceNames.Items.Add(interfaceID));
        }

        private void PopulateWebServicePluginIDs()
        {
            cmbWebServicePlugin.Items.Clear();

            List<string> objInterfaceNames =
                FMChannelHelper.MakeCall<IFMExportService, List<string>>(
                    BindingType,
                    BindingConfiguration,
                    FMExportServiceAddress,
                    exportService => exportService.GetSupportedWebServicePluginIDs(this.security));
            objInterfaceNames.ForEach(strPlugin => cmbWebServicePlugin.Items.Add(strPlugin));
            cmbWebServicePlugin.SelectedIndex = -1;
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.GetRequests();
            }
            catch (Exception ex)
            {
                FMExportServiceLogger.Instance.LogError(ex.ToString());
                MessageBox.Show(ex.Message, "Refresh Button Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ResetControls()
        {
            try
            {
                this.txtRequestName.Text = string.Empty;
                this.cmbSendMethod.SelectedIndex = 0;
                this.txtCompanyCode.Text = string.Empty;
                this.txtOwnerCode.Text = string.Empty;
                this.txtFTPUser.Text = string.Empty;
                this.txtFTPServer.Text = string.Empty;
                this.txtFTPPassword.Text = string.Empty;
                this.cmbWebServicePlugin.SelectedIndex = -1;
                this.txtWebServiceConfiguration.Text = string.Empty;
                this.PopulateInterfaceIDs();
                this.txtRowVersion.Text = "0";
                this.dtpExportTime.Value = DateTime.UtcNow;
                this.txtExportFrequency.Text = (24 * 3600).ToString(CultureInfo.InvariantCulture);
                this.dtBaselineDate.Value = DateTime.UtcNow.Date;
                this.cmbSendMethod.SelectedIndex = 0;
                this.chkExcludeEmptyFiles.Checked = false;
                PopulateWebServicePluginIDs();
                chkUseTimeOfDay.Checked = true;
                dtpExportTime.Enabled = !chkUseTimeOfDay.Checked;
                dtpTimeToExport.Enabled = chkUseTimeOfDay.Checked;

                this.InitializeCompaniesList(true);
                this.hasUnsavedChanges = false;
                this.ReadOnly = true;
                this.selectedExportRequestIdentityGuid = Guid.Empty;
            }
            catch (Exception e)
            {
                FMExportServiceLogger.Instance.LogError(e.ToString());
                MessageBox.Show(e.Message, "Reset Controls Errors", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddSelectedCompany(string company)
        {
            if (this.listOfSelectedCompanies == null)
            {
                this.listOfSelectedCompanies = new List<string>();
            }

            if (!this.listOfSelectedCompanies.Contains(company))
            {
                this.listOfSelectedCompanies.Add(company);
            }

            if (this.lbCompanies.Items.Contains(company) && !this.lbCompanies.SelectedItems.Contains(company))
            {
                this.disregardCompanySelect = true;
                this.lbCompanies.SelectedItems.Add(company);
                this.disregardCompanySelect = false;
            }
        }

        private void RemoveSelectedCompany(string company)
        {
            this.listOfSelectedCompanies.Remove(company);
            this.lbCompanies.SelectedItems.Remove(company);
        }

        private void ClearSelectedCompanies()
        {
            this.listOfSelectedCompanies.Clear();
            this.lbCompanies.SelectedItems.Clear();
        }

        private void ClearCompanies()
        {
            this.listOfSelectedCompanies.Clear();
            this.lbCompanies.Items.Clear();
        }

        private void PopulateControls(ExportRequestClass request)
        {
            if (request == null)
            {
                return;
            }

            this.selectedExportRequestIdentityGuid = request.IdentityGuid;

            // You can't modify the request name if the request already exists
            this.txtRequestName.Enabled = this.selectedExportRequestIdentityGuid == Guid.Empty;
            this.txtRequestName.Text = request.RequestId;
            this.txtCompanyCode.Text = request.SendingCompanyCode;
            this.txtOwnerCode.Text = request.OwnerCode;
            this.txtExportFrequency.Text = request.ExportFrequency.ToString(CultureInfo.InvariantCulture);
            this.dtpExportTime.Value = request.LastExportTime.DateTime;
            this.txtRowVersion.Text = request.LatestRowVersion.ToString(CultureInfo.InvariantCulture);
            this.cmbInterfaceNames.Text = request.InterfaceId;
            this.cmbSendMethod.SelectedIndex = (int)request.SendMethod;
            this.chkExcludeEmptyFiles.Checked = request.ExcludeEmptyFiles;
            this.dtBaselineDate.Value = request.BaselineDate.DateTime;
            this.chkUseTimeOfDay.Checked = request.UseTimeOfDay;
            this.dtpTimeToExport.Value = request.NextExportTime.DateTime;
            this.txtWebServiceConfiguration.Text = request.WebServiceConfiguration;
            for (int intIndex = 0; intIndex < cmbWebServicePlugin.Items.Count; intIndex++)
                if (cmbWebServicePlugin.Items[intIndex].ToString() == request.WebServicePluginType)
                    cmbWebServicePlugin.SelectedIndex = intIndex;

            this.ClearSelectedCompanies();
            foreach (string companyName in request.CompanyNames)
            {
                this.AddSelectedCompany(companyName);
            }

            try
            {
                if (string.IsNullOrEmpty(request.ConnectionInfo))
                {
                    return;
                }

                FTPConnectionClass connectionInfo = (FTPConnectionClass)XmlObjConverter.FromXml(request.ConnectionInfo, typeof(FTPConnectionClass));
                this.txtFTPServer.Text = connectionInfo.Server;
                this.txtFTPUser.Text = connectionInfo.User;
                this.txtFTPPassword.Text = connectionInfo.Password;
                this.chkUsePassiveMode.Checked = connectionInfo.UsePassiveMode;
            }
            catch (FMXmlException)
            {
            }
        }

        private void NewButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.hasUnsavedChanges
                    && MessageBox.Show(
                        "Do you really want to create a new request record? By clicking yes you will lose all of your unsaved changes.",
                        "Confirm New",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                {
                    return;
                }

                this.ResetControls();
                this.ReadOnly = false;
                txtRequestName.Focus();
            }
            catch (Exception ex)
            {
                FMExportServiceLogger.Instance.LogError(e.ToString());
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Login to FuelsManager
        /// </summary>
        /// <returns>
        /// True if the login was successful
        /// </returns>
        private bool LoginToFuelsManager()
        {
            try
            {
                LoginForm loginForm = new LoginForm();
                DialogResult result = loginForm.ShowDialog();
                if (result == DialogResult.Cancel)
                {
                    return false;
                }

                this.security = loginForm.Security;

                return true;
            }
            catch (Exception ex)
            {
                FMExportServiceLogger.Instance.LogError(ex.ToString());
                MessageBox.Show(ex.Message, "FuelsManager Login Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                if (!this.LoginToFuelsManager())
                {
                    this.Close();
                    return;
                }

                this.Text = "FuelsManager Export Configuration Version " + Application.ProductVersion;

                this.InitializeCompaniesList(false);
                this.ResetControls();
                this.GetRequests();
            }
            catch (Exception ex)
            {
                FMExportServiceLogger.Instance.LogError(ex.ToString());
                MessageBox.Show(ex.Message, "Loading Main Window Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Log the user out when the form is closed
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                if (this.security != null)
                {
                    FMChannelHelper.MakeCall<IFMExportService>(
                        BindingType,
                        BindingConfiguration,
                        FMExportServiceAddress,
                        exportService => exportService.Logout(this.security));
                }
            }
            catch (Exception ex)
            {
                FMExportServiceLogger.Instance.LogError(ex.ToString());
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
                FMExportServiceLogger.Instance.LogError(ex.ToString());
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Check to make sure that required fields are present and the data provided is valid.
        /// If the data are not valid, display an error message
        /// </summary>
        /// <returns>True if all required fields are present and the values are valid</returns>
        private bool IsValidData()
        {
            Control controlToFocusOn = null;
            List<string> errorMessages = new List<string>();

            if (string.IsNullOrEmpty(this.txtRequestName.Text))
            {
                controlToFocusOn = this.txtRequestName;
                errorMessages.Add("Request Name must be provided");
            }

            if (string.IsNullOrEmpty(this.cmbInterfaceNames.Text))
            {
                controlToFocusOn = this.cmbInterfaceNames;
                errorMessages.Add("Interface name must be provided");
            }

            if (cmbSendMethod.SelectedIndex == (int)FileSendMethodEnum.FTP || cmbSendMethod.SelectedIndex == (int)FileSendMethodEnum.FTPS)
            {
                if (string.IsNullOrEmpty(this.txtFTPServer.Text))
                {
                    controlToFocusOn = this.txtFTPServer;
                    errorMessages.Add("The FTP server must be provided");
                }

                if (string.IsNullOrEmpty(this.txtFTPUser.Text))
                {
                    controlToFocusOn = this.txtFTPUser;
                    errorMessages.Add("The FTP user name must be provided");
                }

                if (string.IsNullOrEmpty(this.txtFTPPassword.Text))
                {
                    controlToFocusOn = this.txtFTPPassword;
                    errorMessages.Add("The FTP password must be provided");
                }
            }
            else if (cmbSendMethod.SelectedIndex == (int)FileSendMethodEnum.WebService)
            {
                if (cmbWebServicePlugin.SelectedIndex == -1)
                {
                    controlToFocusOn = cmbWebServicePlugin;
                    errorMessages.Add("You must select a web service plug-in when sending via web service.");
                }
            }

            long latestRowVersion;
            if (!long.TryParse(this.txtRowVersion.Text, out latestRowVersion) || latestRowVersion < 0)
            {
                controlToFocusOn = this.txtRowVersion;
                errorMessages.Add("Row Version must be provided and must be numeric and greater than or equal to zero");
            }

            int exportFrequency;
            if (!int.TryParse(this.txtExportFrequency.Text, out exportFrequency) || exportFrequency <= 0)
            {
                controlToFocusOn = this.txtExportFrequency;
                errorMessages.Add("Export Frequency must be provided and must be numeric and greater than zero");
            }

            if (errorMessages.Count > 0)
            {
                MessageBox.Show(string.Join(Environment.NewLine, errorMessages), "Applying Changes Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (controlToFocusOn != null)
                {
                    controlToFocusOn.Focus();
                    TextBox controlAsTextBox = controlToFocusOn as TextBox;
                    if (controlAsTextBox != null)
                    {
                        controlAsTextBox.SelectAll();
                    }
                }

                return false;
            }

            return true;
        }

        private void ApplyButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (!this.IsValidData())
                {
                    return;
                }

                ExportRequestClass exportRequest = new ExportRequestClass
                {
                    IdentityGuid = this.selectedExportRequestIdentityGuid,
                    RequestId = this.txtRequestName.Text,
                    InterfaceId = this.cmbInterfaceNames.Text,
                    CompanyNames = this.listOfSelectedCompanies,
                    ExcludeEmptyFiles = this.chkExcludeEmptyFiles.Checked,
                    ArchiveFolder = this.txtRequestName.Text + "_Archive",
                    UploadStagingFolder = this.txtRequestName.Text + "_Staging",
                    LatestRowVersion = long.Parse(this.txtRowVersion.Text),
                    LastExportTime = this.dtpExportTime.Value,
                    ExportFrequency = int.Parse(this.txtExportFrequency.Text),
                    BaselineDate = this.dtBaselineDate.Value,
                    SendingCompanyCode = this.txtCompanyCode.Text,
                    OwnerCode = this.txtOwnerCode.Text,
                    NextExportTime = this.dtpTimeToExport.Value,
                    UseTimeOfDay = this.chkUseTimeOfDay.Checked,
                    SendMethod = (FileSendMethodEnum)this.cmbSendMethod.SelectedIndex,
                    WebServicePluginType = this.cmbWebServicePlugin.Text,
                    WebServiceConfiguration = this.txtWebServiceConfiguration.Text.Trim()
                };

                if (cmbSendMethod.SelectedIndex == (int)FileSendMethodEnum.FTP || cmbSendMethod.SelectedIndex == (int)FileSendMethodEnum.FTPS)
                {
                    FTPConnectionClass ftp = new FTPConnectionClass
                    {
                        Password = this.txtFTPPassword.Text,
                        Server = this.txtFTPServer.Text,
                        User = this.txtFTPUser.Text,
                        UsePassiveMode = this.chkUsePassiveMode.Checked,
                        EnableSSL = (cmbSendMethod.SelectedIndex == (int)FileSendMethodEnum.FTPS)
                    };
                    exportRequest.ConnectionInfo = XmlObjConverter.ToXml(ftp, typeof(FTPConnectionClass));
                }
                else
                {
                    exportRequest.ConnectionInfo = string.Empty;
                }

                if (exportRequest.IdentityGuid != Guid.Empty)
                {
                    FMChannelHelper.MakeCall<IFMExportService>(
                        BindingType,
                        BindingConfiguration,
                        FMExportServiceAddress,
                        exportRequests => exportRequests.Update(this.security, exportRequest));
                }
                else
                {
                    FMChannelHelper.MakeCall<IFMExportService>(
                        BindingType,
                        BindingConfiguration,
                        FMExportServiceAddress,
                        exportRequests => exportRequests.Add(this.security, exportRequest));
                }

                this.GetRequests();
                this.hasUnsavedChanges = false;
                MessageBox.Show("Changes applied to request \"" + exportRequest.RequestId + "\"", "Confirm Request Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                FMExportServiceLogger.Instance.LogError(ex.ToString());
                MessageBox.Show(ex.Message, "Applying Changes Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Do you really want to delete the selected record?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
                {
                    return;
                }

                FMChannelHelper.MakeCall<IFMExportService>(
                    BindingType,
                    BindingConfiguration,
                    FMExportServiceAddress,
                    exportRequests => exportRequests.Delete(this.security, this.selectedExportRequestIdentityGuid));

                this.ResetControls();
                this.GetRequests();
            }
            catch (Exception ex)
            {
                FMExportServiceLogger.Instance.LogError(ex.ToString());
                MessageBox.Show(ex.Message, "Delete Operation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CalculateFrequencyDuration()
        {
            int secs;
            if (!int.TryParse(this.txtExportFrequency.Text, out secs))
            {
                this.lblFrequencyCalc.Text = "Invalid Duration Value";
                return;
            }

            TimeSpan span = new TimeSpan(0, 0, secs);
            this.lblFrequencyCalc.Text = span.Days + " Day(s) [" +
                                    span.Hours.ToString("00") + ":" +
                                    span.Minutes.ToString("00") + ":" +
                                    span.Seconds.ToString("00") + "]";
        }

        /// <summary>
        /// When the user changes the value of a control, set a flag 
        /// indicating that there are unsaved changes
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void ControlValueChanged(object sender, EventArgs e)
        {
            try
            {
                this.hasUnsavedChanges = true;
            }
            catch (Exception ex)
            {
                FMExportServiceLogger.Instance.LogError(ex.ToString());
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtExportFrequency_TextChanged(object sender, EventArgs e)
        {
            try
            {
                this.ControlValueChanged(sender, e);
                this.CalculateFrequencyDuration();
            }
            catch (Exception ex)
            {
                FMExportServiceLogger.Instance.LogError(ex.ToString());
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lbCompanies_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.disregardCompanySelect)
                {
                    return;
                }

                if (this.lbCompanies.SelectedItem == null || this.lbCompanies.SelectedItems.Count == 0)
                {
                    this.listOfSelectedCompanies.Clear();
                    return;
                }

                if (this.lbCompanies.SelectedItem.ToString() == Constants.AllCustomers &&
                    !this.listOfSelectedCompanies.Contains(Constants.AllCustomers))
                {
                    if (this.listOfSelectedCompanies.Count > 0)
                    {
                        if (DialogResult.No == MessageBox.Show("Selecting the <All> companies option will clear all previous company selections. Are you sure you want to do this?", "Confirm <All> Selection", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
                        {
                            this.disregardCompanySelect = true;
                            this.RemoveSelectedCompany(Constants.AllCustomers);
                            this.disregardCompanySelect = false;
                            return;
                        }
                    }

                    this.disregardCompanySelect = true;
                    string[] companies = new string[this.listOfSelectedCompanies.Count];
                    this.listOfSelectedCompanies.CopyTo(companies);

                    foreach (string company in companies)
                    {
                        if (company != Constants.AllCustomers)
                        {
                            this.RemoveSelectedCompany(company);
                        }
                    }

                    this.disregardCompanySelect = false;
                }
                else
                {
                    if (this.listOfSelectedCompanies.Contains(Constants.AllCustomers))
                    {
                        string selectedCompany = this.lbCompanies.SelectedItems[1].ToString();
                        if (DialogResult.No == MessageBox.Show("Selecting the '" + selectedCompany + "' company will clear the <All> companies option. Are you sure you want to do this?", "Confirm <All> Selection", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
                        {
                            this.disregardCompanySelect = true;
                            this.RemoveSelectedCompany(selectedCompany);
                            this.disregardCompanySelect = false;
                            return;
                        }
                    }

                    this.disregardCompanySelect = true;

                    if (this.lbCompanies.SelectedItems.Contains(Constants.AllCustomers))
                    {
                        this.lbCompanies.SelectedItems.Remove(Constants.AllCustomers);
                    }

                    this.disregardCompanySelect = false;
                }

                foreach (object o in this.lbCompanies.SelectedItems)
                {
                    if (!this.listOfSelectedCompanies.Contains(o.ToString()))
                    {
                        this.listOfSelectedCompanies.Add(o.ToString());
                    }
                }

                string[] arrayCompanies = new string[this.listOfSelectedCompanies.Count];

                this.listOfSelectedCompanies.CopyTo(arrayCompanies);

                foreach (string s in arrayCompanies)
                {
                    if (!this.lbCompanies.SelectedItems.Contains(s))
                    {
                        this.listOfSelectedCompanies.Remove(s);
                    }
                }

                this.ControlValueChanged(sender, e);
            }
            catch (Exception ex)
            {
                FMExportServiceLogger.Instance.LogError(ex.ToString());
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Application.Exit();
            }
            catch (Exception ex)
            {
                FMExportServiceLogger.Instance.LogError(ex.ToString());
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// When a row is selected, populate the controls on the screen with information from the selected request
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        private void RequestGrid_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.RequestGrid.CurrentRow == null || this.RequestGrid.Rows.Count <= 0)
                {
                    return;
                }

                DataGridViewRow row = this.RequestGrid.CurrentRow;
                ExportRequestClass exportRequestClass = row.DataBoundItem as ExportRequestClass;

                if (exportRequestClass != null)
                {
                    Guid identityGuid = exportRequestClass.IdentityGuid;

                    // This call to Get() is not completely necessary since the data 
                    // is already stored in the grid. However, this helps ensure that the user
                    // is seeing the most recent version of the request possible
                    ExportRequestClass exportRequest = FMChannelHelper.MakeCall<IFMExportService, ExportRequestClass>(
                        BindingType,
                        BindingConfiguration,
                        FMExportServiceAddress,
                        exportRequests => exportRequests.Get(this.security, identityGuid));

                    this.ReadOnly = false;
                    this.PopulateControls(exportRequest);
                    this.hasUnsavedChanges = false;
                }
            }
            catch (Exception ex)
            {
                FMExportServiceLogger.Instance.LogError(ex.ToString());
                MessageBox.Show(ex.Message, "Selecting Item from Grid Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void chkUseTimeOfDay_CheckedChanged(object sender, EventArgs e)
        {
            dtpExportTime.Enabled = !chkUseTimeOfDay.Checked;
            dtpTimeToExport.Enabled = chkUseTimeOfDay.Checked;
        }

        private void cmbSendMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSendMethod.SelectedIndex == (int)FileSendMethodEnum.None)
            {
                grpFtpSettings.Enabled = false;
                grpWebServiceSettings.Enabled = false;
                cmbWebServicePlugin.SelectedIndex = -1;
                txtWebServiceConfiguration.Text = string.Empty;
                txtFTPServer.Text = string.Empty;
                txtFTPPassword.Text = string.Empty;
                txtFTPUser.Text = string.Empty;
                chkUsePassiveMode.Checked = true;
            }
            else if (cmbSendMethod.SelectedIndex == (int)FileSendMethodEnum.FTP || cmbSendMethod.SelectedIndex == (int)FileSendMethodEnum.FTPS)
            {
                grpFtpSettings.Enabled = true;
                grpWebServiceSettings.Enabled = false;
                grpFtpSettings.BringToFront();
            }
            else if (cmbSendMethod.SelectedIndex == (int)FileSendMethodEnum.WebService)
            {
                grpWebServiceSettings.Enabled = true;
                grpFtpSettings.Enabled = false;
                grpWebServiceSettings.BringToFront();
            }
        }

    }
}
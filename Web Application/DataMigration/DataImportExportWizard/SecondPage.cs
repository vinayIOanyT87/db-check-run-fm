// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SecondPage.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the SecondPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DataImportExportWizard
{
    using System;
    using System.ComponentModel;
    using System.Data;
    using System.Diagnostics;
    using System.Text;
    using System.Windows.Forms;

    using DataImportExportWizard.Constants;
    using DataImportExportWizard.DataAccess;
    using DataImportExportWizard.InternalClasses;

    using Wizard.UI;

    /// <summary>
    /// The second page.
    /// </summary>
    public partial class SecondPage : Wizard.UI.InternalWizardPage
    {
        #region Attributes

        /// <summary>
        /// The site list.
        /// </summary>
        private DataSet siteList = new DataSet();

        #endregion Attributes

        /// <summary>
        /// Initializes a new instance of the <see cref="SecondPage"/> class.
        /// </summary>
        public SecondPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// The enterprise browse btn_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void BrowseImportExportFileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog OpenFileDlg = new OpenFileDialog();
            OpenFileDlg.Filter = @"dat files (*.dat)|*.dat|All files(*.*)|*.*";

            if (this.SiteIdDropDown.SelectedIndex > 0)
            {
                OpenFileDlg.FileName = string.Format(
                    StringConstants.DefaultKeyFilenameFormat, this.SiteIdDropDown.SelectedValue);
            }

            if (OpenFileDlg.ShowDialog() == DialogResult.OK)
            {
                this.ImportExportFilename.Text = OpenFileDlg.FileName;
            }
        }

        private void SkipStepCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (this.SkipStepCheckBox.Checked)
            {
                this.ImportExportFilename.Enabled = false;
                this.BrowseImportExportFileButton.Enabled = false;
            }
            else
            {
                this.ImportExportFilename.Enabled = true;
                this.BrowseImportExportFileButton.Enabled = true;
            }
        }

        /// <summary>
        /// The second page_ wizard next.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void SecondPage_WizardNext(object sender, WizardPageEventArgs e)
        {
            StringBuilder validationError = new StringBuilder();

            if (DataImportExportWizardOption.SelectedInstallationType == InstallationType.EnterpriseServer)
            {
                if (this.SiteIdDropDown.SelectedIndex == 0)
                {
                    validationError.Append("Please select a Site.");
                }

                if (string.IsNullOrEmpty(this.ImportExportFilename.Text))
                {
                    validationError.Append("Please provide a location for the save file.");
                }
            }
            else if (DataImportExportWizardOption.SelectedInstallationType == InstallationType.BaseServer)
            {
                if (this.SiteIdDropDown.SelectedIndex == 0)
                {
                    validationError.Append("Unable to determine Site.");
                }

                if (string.IsNullOrEmpty(this.ImportExportFilename.Text))
                {
                    validationError.Append("Please select the ID && GUID Import file.");
                }
            }           

            if (validationError.Length > 0)
            {
                MessageBox.Show(validationError.ToString(), StringConstants.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Cancel = true;
                return;
            }

            string err = ((DataImportExportWizardSheet)GetWizard()).Error;

            DAService adminConnect = new DAService();

            Cursor.Current = Cursors.WaitCursor;
            try
            {
                // If we're on the Enterprise Server and the user has not opted to Skip this step, we need to set the ImportExport option to ExportKeys
                if (DataImportExportWizardOption.SelectedInstallationType == InstallationType.EnterpriseServer)
                {
                    DataImportExportWizardOption.CurrentImportExportStep = WizardStepType.ExportingKeys;

                    if (this.SkipStepCheckBox.Checked)
                    {
                        DataImportExportWizardOption.CurrentImportExportStep = WizardStepType.ImportingData;
                    }
                }

                if (adminConnect.ErrorMessage != string.Empty)
                {
                    ((DataImportExportWizardSheet)GetWizard()).Error = adminConnect.ErrorMessage;
                }
                else
                {
                    ((DataImportExportWizardSheet)GetWizard()).Error = err;
                }
            }
            catch (Exception ex)
            {
                ((DataImportExportWizardSheet)GetWizard()).LoggerInstance.Error("SecondPage_WizardNext. " + ex.Message);
                Trace.WriteLine(string.Format("SecondPage_WizardNext. {0}", ex.Message));
            }

            Cursor.Current = Cursors.Default;

            if (((DataImportExportWizardSheet)GetWizard()).Error == string.Empty)
            {
                e.NewPage = "FifthPage";
            }
            else
            {
                e.NewPage = "FourthPage";
            }
        }

        /// <summary>
        /// This event is called when the UI Wizard activates this form.  We can perform any initialization logic here.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void SecondPage_SetActive(object sender, CancelEventArgs e)
        {
            this.SiteLabel.Text = @"Site ID:";

            switch (DataImportExportWizardOption.SelectedInstallationType)
            {
                case InstallationType.EnterpriseServer:
                    this.SiteIdDropDown.Visible = true;
                    this.SiteIdTextBox.Visible = false;
                    this.ImportExportFilenameLabel.Text = @"Save File:";
                    break;
                case InstallationType.BaseServer:
                    this.SiteIdDropDown.Visible = false;
                    this.SiteIdTextBox.Visible = true;
                    this.ImportExportFilenameLabel.Text = @"ID && GUID File:";
                    break;
            }

            // this.ImportExportFilename.Text = string.Empty;
            this.ImportExportFilenameLabel.Visible = true;
            this.ImportExportFilename.Visible = true;
            this.BrowseImportExportFileButton.Visible = true;

            try
            {

            }
            catch (Exception ex)
            {
                ((DataImportExportWizardSheet)GetWizard()).LoggerInstance.Error("SecondPage_SetActive. " + ex.Message);
                Trace.WriteLine(string.Format("SecondPage_SetActive. {0}", ex.Message));
                MessageBox.Show(ex.Message);
            }
        }

        private void SecondPage_Load(object sender, EventArgs e)
        {
            this.SiteIdDropDown.Items.Clear();

            DAService adminConnect = new DAService();
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                DataSet dataSet = adminConnect.GetSites();
                DataTable dataTable = dataSet.Tables[0];
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    if (dataTable.Rows[i]["ID"].ToString() == "SiteAdmin")
                    {
                        continue;
                    }

                    this.SiteIdDropDown.Items.Add(dataTable.Rows[i]["ID"].ToString());
                }

                this.SiteIdDropDown.Items.Insert(0, "{Not Selected}");

                if (!string.IsNullOrEmpty(DAService.SiteId))
                {
                    if (this.SiteIdDropDown.FindStringExact(DAService.SiteId) >= 0)
                    {
                        this.SiteIdDropDown.SelectedItem = DAService.SiteId;
                    }
                }
            }
            catch (Exception ex)
            {

                ((DataImportExportWizardSheet)GetWizard()).LoggerInstance.Error(
                    "SecondPage: SecondPage_Load. " + ex.Message);

                Trace.WriteLine(string.Format("SecondPage: SecondPage_Load. {0}", ex.Message));
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }
    }
}

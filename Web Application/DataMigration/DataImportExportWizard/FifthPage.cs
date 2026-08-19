// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FifthPage.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the FifthPage type.
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
    /// The fifth page.
    /// </summary>
    public partial class FifthPage : InternalWizardPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FifthPage"/> class.
        /// </summary>
        public FifthPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// The fifth page_ wizard next.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void FifthPage_WizardNext(object sender, Wizard.UI.WizardPageEventArgs e)
        {
            StringBuilder validationError = new StringBuilder();

            if (DataImportExportWizardOption.SelectedInstallationType == InstallationType.EnterpriseServer)
            {
                if (this.SiteIdDropDown.SelectedIndex == 0)
                {
                    validationError.Append("Unable to determine Site.");
                }

                if (string.IsNullOrEmpty(this.ImportExportFilename.Text))
                {
                    validationError.Append("Please select the Data Migration Import file.");
                }
            }
            else if (DataImportExportWizardOption.SelectedInstallationType == InstallationType.BaseServer)
            {
                if (this.SiteIdDropDown.SelectedIndex == 0)
                {
                    validationError.Append("Please select a Site.");
                }

                if (string.IsNullOrEmpty(this.ImportExportFilename.Text))
                {
                    validationError.Append("Please provide a location for the export file.");
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
                e.NewPage = "SixthPage";
            }
            else
            {
                e.NewPage = "FourthPage";
            }
        }

        /// <summary>
        /// The browse import export file button_ click.
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

            OpenFileDlg.Filter = @"zip files (*.zip)|*.zip|All files(*.*)|*.*";

            if (DataImportExportWizardOption.SelectedInstallationType == InstallationType.BaseServer)
            {
                OpenFileDlg.FileName = string.Format(StringConstants.DefaultDataZipFilenameFormat, DAService.SiteId);
            }

            if (OpenFileDlg.ShowDialog() == DialogResult.OK)
            {
                // this.BaseFileName.Text = OpenFileDlg.FileName;
            }
        }

        /// <summary>
        /// The fifth page_ load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void FifthPage_Load(object sender, EventArgs e)
        {
            DAService adminConnect = new DAService();

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
            }
            catch (Exception ex)
            {

                ((DataImportExportWizardSheet)GetWizard()).LoggerInstance.Error("FifthPage: FifthPage_Load. " + ex.Message);
                Trace.WriteLine(string.Format("FifthPage: FifthPage_Load. {0}", ex.Message));
            }

            this.SiteIdDropDown.SelectedItem = "{Not Selected}";
        }
    }
}

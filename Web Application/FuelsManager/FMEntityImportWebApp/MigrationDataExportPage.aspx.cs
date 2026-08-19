// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MigrationDataExportPage.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the MigrationDataExportPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMEntityImportWebApp
{
    using System;
    using System.IO;
    using System.Web.UI.WebControls;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.UtilityObjects;

    using FuelsManager.FMWebApp;

	/// <summary>
    /// The migration data import export.
    /// </summary>
    public partial class MigrationDataExportPage : FMFormBase
    {
        /// <summary>
        /// The migration import export history
        /// </summary>
        public const string MigrationImportExportStatus = "Operations.MigrationImportExportStatus";
        public const string MigrationSelectedSiteGuid = "Operations.MigrationSelectedSiteGuid";

        public const string MigrationCachedDownloadFilename = "Operations.MigrationCachedDownloadFilename";

        #region Properties
        /// <summary>
        /// Gets or sets the selected Site ID.
        /// </summary>
        public string SelectedSiteId { get; set; }

        /// <summary>
        /// Gets or sets the migration import/export history collection object from Session.
        /// </summary>
        private MigrationDataExportImportLogCollection MigrationDataExportImportLogList
        {
            // ReSharper disable once UnusedMember.Local
            get
            {
                return this.Session[MigrationImportExportStatus] as MigrationDataExportImportLogCollection;
            }

	        set
            {
                this.Session.Add(MigrationImportExportStatus, value);
            }
        }

        /// <summary>
        /// Gets or sets the selected site GUID.
        /// </summary>
        private Guid? SelectedSiteGuid
        {
            get
            {
                return this.Session[MigrationSelectedSiteGuid] as Guid?;
            }

	        set
            {
                this.Session.Add(MigrationSelectedSiteGuid, value);
            }
        }

        /// <summary>
        /// Gets or sets the cached download filename.
        /// </summary>
        private string CachedDownloadFilename
        {
            // ReSharper disable once UnusedMember.Local
            get
            {
                return this.Session[MigrationCachedDownloadFilename] as string;
            }

	        set
            {
                this.Session.Add(MigrationCachedDownloadFilename, value);
            }
        }
        #endregion Properties

        #region Methods and Operators

        /// <summary>
        /// This method will either enable or disable controls.  It is called by the individual tabs associated to the site form.
        /// </summary>
        /// <param name="enable">
        /// True to enable the page controls, otherwise; false to disable the page controls.
        /// </param>
        public void EnableControls(bool enable)
        {
            if (this.Security.HasRight(RIGHT.MIGRATION_PERFORM_IMPORT_EXPORT))
            {
                // this.OK.Enabled = enable;
            }

            // this.Cancel.Enabled = enable;
        }

        /// <summary>
        /// Update all object(s) in session with any data the user has entered on the page
        /// </summary>
        public void UpdateData()
        {
            //this.PeriodicSyncSettingsPage.UpdateData();
        }

        /// <summary>
        /// Populate the fields on the screen with data
        /// </summary>
        private void UpdateView()
        {
            try
            {
                if (null == this.SelectedSiteGuid)
                {
                    this.SelectedSiteText.Text = this.Security.SiteID;
                    this.SelectedSiteGuid = this.Security.SiteGuid;
                }

                // Get the stored Synchronization Settings
                MigrationDataExportImportLogCollection migrationHistory =
                    FMChannelHelper
                        .MakeCall<IMigrationDataExportImportLog, MigrationDataExportImportLogCollection>(
                            history => history.EnumerateBySiteGuid(this.Security, this.SelectedSiteGuid.Value));

                this.MigrationDataExportImportLogList = migrationHistory;
            }
            catch (Exception ex)
            {
                this.ErrorHandler(ex);
            }
        }

/*
        /// <summary>
        /// The encrypt stream.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <returns>
        /// The <see cref="MemoryStream"/>.
        /// </returns>
        private MemoryStream EncryptStream(MemoryStream stream)
        {
            // get certificate name
            string certificateName =
                FMChannelHelper.MakeCall<IConfigurationSettings, string>(
                    x => x.GetKeyValueByKey(this.Security, ConfigurationSettingDOClass.Key_InstallDetails_EnterpriseCertificateName));

            // Compress and encrypt the stream
            var compressionProcessor = new CompressionProcessor();
            var encryption = new Encryption(System.Text.Encoding.Unicode);
            encryption.CertificateName = certificateName;
            byte[] data = compressionProcessor.Compress(stream.ToArray());
            return encryption.Package(data);
        }
*/

        /// <summary>
        /// The generate GUID mapping information.
        /// </summary>
        private void GenerateGuidMappingInformation()
        {
            if (null != this.SelectedSiteGuid)
            {
                SiteClass site =
                    FMChannelHelper.MakeCall<ISites, SiteClass>(
                        x => x.GetUsingGuid(this.Security, this.SelectedSiteGuid.Value));

                MigrationDataExportImportSettingDO settings =
                    FMChannelHelper.MakeCall<IMigrationDataExportImport, MigrationDataExportImportSettingDO>(
                        s => s.GetMigrationDataExportImportConfiguration(this.Security, string.Empty));

                FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
                    alarmAndEventChannel =>
                        {
                            MigrationDataExportImportEvents migrationEvents = new MigrationDataExportImportEvents();
                            alarmAndEventChannel.Add(
                                this.Security,
                                migrationEvents.MigrationExportGuidMappingEvent(site.ID, this.Security.UserID));
                        });

                string fileName = string.Empty;

                this.ResultsTB.Text = @"Generating migration export file." + Environment.NewLine;

                byte[] mappingData =
                    FMChannelHelper.MakeCall<IMigrationDataExportImport, byte[]>(
                        x => x.GetGuidMappingZipArchive(this.Security, site, string.Empty, out fileName));

                this.ResultsTB.Text += @"Migration export file generated." + Environment.NewLine;

                if (mappingData.Length > 0)
                {
                    if (!settings.ExportArchivePath.EndsWith(@"\"))
                    {
                        settings.ExportArchivePath += @"\";
                    }

                    string cachedFilename = settings.ExportArchivePath + fileName;

                    // Don't cross the streams
                    // Response.ClearContent();
                    // Response.ClearHeaders();

                    // Response.AddHeader("Content-disposition", "attachment; filename=" + fileName);
                    // Response.Buffer = false;
                    // Response.ContentType = "application/octet-stream";
                    // Response.AddHeader("cache-control", "private");
                    // Response.AddHeader("Connection", "Keep-Alive");

                    // Read the memory stream and stream it back to the client in chunks
                    byte[] buffer = new byte[131072];

	                using (var cachedFile = new FileStream(cachedFilename, FileMode.CreateNew))
                    {
                        using (var streamReader = new MemoryStream(mappingData))
                        {
	                        int byteCount;

	                        while ((byteCount = streamReader.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                cachedFile.Write(buffer, 0, byteCount);
                            }
                        }

	                    cachedFile.Flush();
                    }

                    this.ResultsTB.Text +=
                        string.Format(@"Migration export file ready: {0}." + Environment.NewLine, fileName);

                    this.CachedDownloadFilename = cachedFilename;

                    this.ClientScript.RegisterStartupScript(typeof(string), "downloadFile", @"<script type=""text/javascript"">downloadFile();</script>");
                }
                else
                {
                    this.ResultsTB.Text += @"Migration export file has no data." + Environment.NewLine;
                }
            }
            else
            {
                this.ResultsTB.Text = @"Unable to determine selected Site." + Environment.NewLine;
            }
        }
        #endregion Methods and Operators

        #region Page Events and Overrides

        /// <summary>
        /// Overrides the virtual <code>OnInit</code> framework method.  Any special page specific initialization can be performed here.
        /// </summary>
        /// <param name="e">
        /// Event arguments of type <see cref="EventArgs"/> for the <code>OnInit</code> method.
        /// </param>
        protected override void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            this.InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        ///    Required method for Designer support - do not modify
        ///    the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            // this.OK.Command += new System.Web.UI.WebControls.CommandEventHandler(this.OK_Command);
            // this.Cancel.Command += new System.Web.UI.WebControls.CommandEventHandler(this.Cancel_Command);
        }

        /// <summary>
        /// The page_ load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        /// <exception cref="Exception">
        /// Throws an exception for Insufficient Rights if the user does not have ANY View rights for Client, Server and Periodic Synchronization Settings.
        /// </exception>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                this.GetSecurity();

                if (!this.Security.HasRight(RIGHT.MIGRATION_PERFORM_IMPORT_EXPORT))
                {
                    throw new Exception("Insufficient Rights");
                }

                if (!this.Page.IsPostBack)
                {
                    this.UpdateView();
                }

                if (!this.Security.HasRight(RIGHT.MIGRATION_PERFORM_IMPORT_EXPORT))
                {
                    // this.OK.Enabled = false;
                }

                if (this.Security.HasRight(RIGHT.MIGRATION_PERFORM_IMPORT_EXPORT))
                {
                    this.ExportBtn.Attributes.Add(
                        "onclick", "this.disabled=true;" + this.Page.ClientScript.GetPostBackEventReference(this.ExportBtn, string.Empty));
                }
            }
            catch (Exception except)
            {
                this.ErrorHandler(except);
            }
        }
        #endregion Page Events and Overrides

        #region Control  Event Handlers
/*
        /// <summary>
        /// The cancel_ command.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void CancelCommand(object sender, CommandEventArgs e)
        {
            this.Session.Remove(MigrationImportExportStatus);
            this.Session.Remove(MigrationSelectedSiteGuid);

            this.Redirect("MigrationDataExportPage.aspx");
        }
*/

        /// <summary>
        /// The export btn_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void ExportBtnClick(object sender, EventArgs e)
        {
            try
            {
                this.GenerateGuidMappingInformation();
            }
            catch (Exception except)
            {
                this.ErrorHandler(except);
            }
        }

        #endregion Control  Event Handlers
    }
}
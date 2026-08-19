// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OfflineSynchronizationExportPage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the OfflineSynchronizationExportPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMEntityImportWebApp
{
    using System;
    using System.IO;
    using System.Security;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.Constants;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.UtilityObjects;

    using FuelsManager.FMWebApp;

	/// <summary>
    /// Code behind for OfflineSynchronizationExportPage that allows a user to initiate an offline synchronization request by creating the client side file.
    /// </summary>
    public partial class OfflineSynchronizationExportPage : FMUserControlBase
    {
        #region Constants and Fields
        public static string AllChangesValue = "AllChanges";
        #endregion Constants and Fields

        #region Properties
        /// <summary>
        /// Gets or sets the Client Sync Configuration object from Session.
        /// </summary>
        private SyncClientConfigurationDO SessionSyncClientConfig
        {
            get
            {
	            if (this.Session[PageSessionKeyConstants.SYNC_CONFIG_CLIENT_SETTINGS] != null 
					&& this.Session[PageSessionKeyConstants.SYNC_CONFIG_CLIENT_SETTINGS] is SyncClientConfigurationDO)
                {
                    return (SyncClientConfigurationDO)this.Session[PageSessionKeyConstants.SYNC_CONFIG_CLIENT_SETTINGS];
                }
	            return null;
            }
	        set
            {
                this.Session.Add(PageSessionKeyConstants.SYNC_CONFIG_CLIENT_SETTINGS, value);
            }
        }
        #endregion Properties

        #region Public Methods and Operators

        /// <summary>
        /// Called when the user clicks a button to extract the current values from the user interface.
        /// </summary>
        public void UpdateData()
        {
            try
            {
                var syncClientConfig = this.SessionSyncClientConfig;
            }
            catch (Exception except)
            {
                this.ErrorHandler(except);
            }
        }

        /// <summary>
        /// The update view.
        /// </summary>
        public void UpdateView()
        {
            try
            {
                this.ExportTypeRadioBtnList.SelectedValue = AllChangesValue;
                this.FMLabelFromDate.Enabled = false;
                this.FMDateFromDate.Enabled = false;
                this.FMLabelToDate.Enabled = false;
                this.FMDateToDate.Enabled = false;
            }
            catch (Exception except)
            {
                this.ErrorHandler(except);
            }
        }
        #endregion Public Methods and Operators

        #region Page Event Handlers and Overrides
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
        /// The page_ load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        [SecurityCritical]
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!this.Page.IsPostBack)
                {
                    this.UpdateView();
                }

                // If a successful complete synchronization has already taken place, then we will not allow the user to change the synchronization Site/SiteGroup ID.
                // This prevents nodes from cycling through Sites/SiteGroups and pulling down all data from the enterprise.
            }
            catch (Exception except)
            {
                this.ErrorHandler(except);
            }
        }

        /// <summary>
        ///    Required method for Designer support - do not modify
        ///    the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
        }
        #endregion Page Event Handlers and Overrides

        #region Control  Event Handlers
        /// <summary>
        /// The Export button click event handler.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void ExportBtnClickCommand(object sender, EventArgs e)
        {
            try
            {
                // What type of export are we doing?
                if (this.ExportTypeRadioBtnList.SelectedValue.Equals(AllChangesValue))
                {
                    var dt = new DataTransmission(this.Security.SiteID, this.Security.UserID);
                    AlarmAndEventLogClass alarmAndEventLog = dt.TransmissionExportEventLog;

                    this.ExportAllChanges(alarmAndEventLog);
                }
                else
                {
                    var dt = new DataTransmission(this.Security.SiteID, this.Security.UserID, this.FMDateFromDate.Text);
                    AlarmAndEventLogClass alarmAndEventLog = dt.TransmissionExportReProcessEventLog;

                    this.ExportSelectedDateRange(alarmAndEventLog);
                }
            }
            catch (Exception except)
            {
                this.ErrorHandler(except);
            }
        }

        /// <summary>
        /// Selected index change event handler for the export type radio button controls.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void ExportTypeRadioBtnListSelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.ExportTypeRadioBtnList.SelectedIndex == 1)
            {
                this.FMLabelFromDate.Enabled = true;
                this.FMDateFromDate.Enabled = true;
                this.FMLabelToDate.Enabled = true;
                this.FMDateToDate.Enabled = true;
            }
            else
            {
                this.FMLabelFromDate.Enabled = false;
                this.FMDateFromDate.Enabled = false;
                this.FMLabelToDate.Enabled = false;
                this.FMDateToDate.Enabled = false;
            }
        }
        #endregion Control  Event Handlers

        #region Private Processing Methods
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
            var encryption = new Encryption(System.Text.Encoding.Unicode) { CertificateName = certificateName };
            byte[] data = compressionProcessor.Compress(stream.ToArray());

            return encryption.Package(data);
        }

        /// <summary>
        /// The export all changes.
        /// </summary>
        /// <param name="alarmAndEventLog">
        /// The alarm and event log.
        /// </param>
        private void ExportAllChanges(AlarmAndEventLogClass alarmAndEventLog)
        {
            // Instruct the synchronization service to generate an offline synchronization file.
            SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
                                                             x => x.GetUsingGuid(this.Security, this.Security.SiteGuid));

            SiteTimeConverter timeConverter = new SiteTimeConverter(site);

            string fileName = site.ID + "_FMDT_" +
                  timeConverter.Now().ToString("yyyyMMdd_HHmmss") +
                  ".syncvcef";

            this.ProcessRecords(alarmAndEventLog, fileName, site);
        }

        /// <summary>
        /// The export selected date range.
        /// </summary>
        /// <param name="alarmAndEventLog">
        /// The alarm and event log.
        /// </param>
        /// <exception cref="Exception">
        /// Throws an exception if the end date precedes the start date.
        /// </exception>
        private void ExportSelectedDateRange(AlarmAndEventLogClass alarmAndEventLog)
        {
            // Get date from form
            DateTimeOffset startDateTime = this.FMDateFromDate.CurrentValue;
            DateTimeOffset endDateTime = this.FMDateToDate.CurrentValue;

            if (startDateTime > endDateTime)
            {
                throw new Exception("Start Date must not exceed End Date");
            }

            SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
                                                 x => x.GetUsingGuid(this.Security, this.Security.SiteGuid));

            string fileName = site.ID + "_FMDT_" +
                startDateTime.ToString("yyyyMMdd_HHmmss") + "_TO_" +
                endDateTime.ToString("yyyyMMdd_HHmmss") +
                ".syncvcef";

            // Use that collection to process the output file
            this.ProcessRecords(alarmAndEventLog, fileName, site);
        }

        /// <summary>
        /// The process records.
        /// </summary>
        /// <param name="alarmAndEventLog">
        /// The alarm and event log.
        /// </param>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <param name="site">
        /// The site.
        /// </param>
        /// <exception cref="Exception">
        /// </exception>
        private void ProcessRecords(AlarmAndEventLogClass alarmAndEventLog, string fileName, SiteClass site)
        {
            FileStream fstream = null;

            try
            {
                // If the Export Archive Directory exist save the file to the directory.
                if (!string.IsNullOrEmpty(site.ExportArchiveDir))
                {
                    string strPath = site.ExportArchiveDir.Trim();

                    var directoryInfo = new DirectoryInfo(strPath);
                    if (!directoryInfo.Exists)
                    {
                        throw new Exception("Export Archive Directory Error, check Site configuration.");
                    }

                    const string StrBackSlash = "\\";

                    if (!strPath.EndsWith(StrBackSlash))
                    {
                        strPath += StrBackSlash;
                    }
                    
                    string strExportArchiveDirAndFileName = strPath + fileName;

                    // This will be a synchronous call to the Synchronization Service.  If it returns successfully, we can stream the generated
                    // file to the end user.

                    fstream = new FileStream(strExportArchiveDirAndFileName, FileMode.Open);

                    // fstream = new FileStream(strExportArchiveDirAndFileName, FileMode.Create);
                }
                else
                {
                    throw new Exception("Export Archive Directory Error, check Site configuration.");
                }

                // Don't cross the streams
	            this.Response.ClearContent();
	            this.Response.ClearHeaders();

	            this.Response.AddHeader("Content-disposition", "attachment; filename=" + fileName);
	            this.Response.Buffer = false;
	            this.Response.ContentType = "application/octet-stream";
	            this.Response.AddHeader("cache-control", "private");
	            this.Response.AddHeader("Connection", "Keep-Alive");

                byte[] buffer = new byte[4096];
                int byteCount;

                while ((byteCount = fstream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    this.Response.OutputStream.Write(buffer, 0, byteCount);
                    this.Response.Flush();
                }
                
                //var transmissionCollection = new DataTransmissionRecordCollectionClass();

                //foreach (ChangeQueueRecordClass record in recordCollection)
                //{
                //    if (record.Duplicate)
                //    {
                //        continue;
                //    }

                //    // If the record is not from the current site, check to make sure it is assigned.
                //    // If it is not assigned, we can skip it.
                //    if (this.CheckEntityAssignmentStatus(record) == false)
                //    {
                //        continue;
                //    }

                //    var transmissionRecord = new DataTransmissionRecordClass();

                //    if (record.IsDeletion)
                //    {
                //        // Add the change queue record as a delete indicator
                //        transmissionRecord.ChangeQueueRecord = record;
                //        transmissionRecord.OriginatingSiteID = FMChannelHelper.MakeCall<ISites, string>(
                //                                                         x =>
                //                                                         x.GetIDNoRefresh(this.Security, record.SiteGuid)
                //                                                    );

                //        transmissionCollection.Add(transmissionRecord);
                //    }
                //    else
                //    {
                //        transmissionRecord.ChangeQueueRecord = record;

                //        // Get the specified entity and add it to the export 
                //        switch (record.RecordType)
                //        {
                //            case ChangeQueueRecordType.Companies:
                //                this.AddCompanyRecord(transmissionCollection, record, transmissionRecord);
                //                break;

                //            case ChangeQueueRecordType.Equipment:
                //                this.AddEquipmentRecord(transmissionCollection, record, transmissionRecord);
                //                break;

                //            case ChangeQueueRecordType.FuelCards:
                //                this.AddFuelCardRecord(transmissionCollection, record, transmissionRecord);
                //                break;

                //            case ChangeQueueRecordType.Personnel:
                //                this.AddPersonRecord(transmissionCollection, record, transmissionRecord);
                //                break;

                //            case ChangeQueueRecordType.Products:
                //                this.AddProductRecord(transmissionCollection, record, transmissionRecord);
                //                break;

                //            case ChangeQueueRecordType.Transactions:
                //                this.AddTransactionRecord(transmissionCollection, record, transmissionRecord);
                //                break;

                //            case ChangeQueueRecordType.Groups:
                //                this.AddGroupRecord(transmissionCollection, record, transmissionRecord);
                //                break;

                //            case ChangeQueueRecordType.TransactionAliases:
                //                this.AddAliasRecord(transmissionCollection, record, transmissionRecord);
                //                break;

                //            case ChangeQueueRecordType.CloseoutDO:
                //                this.AddCloseoutRecord(this.closeout, transmissionCollection, record, transmissionRecord);
                //                break;
                //        }
                //    }

                //    if (transmissionCollection.Count > 1000)
                //    {
                //        var stream = new MemoryStream();

                //        var serializer = new XmlSerializer(transmissionCollection.GetType());
                //        serializer.Serialize(stream, transmissionCollection);
                //        transmissionCollection.Clear();

                //        MemoryStream encryptedStream = this.EncryptStream(stream);

                //        encryptedStream.WriteTo(this.Response.OutputStream);

                //        this.Response.Flush();

                //        encryptedStream.WriteTo(fstream);
                //    }
                // }

                //if (transmissionCollection.Count != 0)
                //{
                //    var stream = new MemoryStream();

                //    var serializer = new XmlSerializer(transmissionCollection.GetType());
                //    serializer.Serialize(stream, transmissionCollection);
                //    transmissionCollection.Clear();

                //    MemoryStream encryptedStream = this.EncryptStream(stream);

                //    encryptedStream.WriteTo(this.Response.OutputStream);
                //    this.Response.Flush();

                //    if (fstream != null)
                //    {
                //        encryptedStream.WriteTo(fstream);
                //    }
                //}

                // Complete request and stop more than the file from rendering to the client
                this.Response.SuppressContent = true;
            }
            finally
            {
                if (fstream != null)
                {
                    fstream.Flush();
                    fstream.Close();
                }
            }
        }
        #endregion Private Processing Methods
    }
}
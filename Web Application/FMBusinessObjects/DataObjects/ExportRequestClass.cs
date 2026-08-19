// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportRequestClass.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ExportRequestClass type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Runtime.Serialization;

	using FMBusinessObjects.UtilityObjects;

	/// <summary>
	/// Definition of the ExportRequestClass
	/// </summary>
	[DataContract]
	[Serializable]
	public class ExportRequestClass : BaseDataObject
	{
		/// <summary>
		/// The default interface ID
		/// </summary>
		public const string DefaultInterfaceId = "UNDEFINED";

		/// <summary>
		/// The default archive folder
		/// </summary>
		public const string DefaultArchiveFolder = @"C:\Archive\";

		/// <summary>
		/// The default upload staging folder
		/// </summary>
		public const string DefaultUploadStagingFolder = @"C:\Archive\UploadStaging";

		/// <summary>
		/// The default export frequency in seconds 
		/// Number of seconds in a day = 24 hours/day * 60 minutes/hour * 60 seconds/minute
		/// </summary>
		public const int DefaultExportFrequency = 24 * 60 * 60;

        /// <summary>
        /// The Next Export Time
        /// </summary>
        private DateTimeOffset nextExportTime;


		/// <summary>
		/// List of company names in Xml format.
		/// </summary>
		[DataMember]
		private string companyNames;

		/// <summary>
		/// The archive folder.
		/// </summary>
		[DataMember]
		private string archiveFolder;

		/// <summary>
		/// The upload staging folder.
		/// </summary>
		[DataMember]
		private string uploadStagingFolder;

		/// <summary>
		/// Initializes a new instance of the <see cref="ExportRequestClass"/> class
		/// </summary>
		public ExportRequestClass()
		{
			this.Reset();
		}

		#region Properties

		/// <summary>
		/// Gets or sets the RequestId
		/// </summary>
		[DataMember]
		public string RequestId
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the InterfaceId
		/// </summary>
		[DataMember]
		public string InterfaceId
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the owner code
		/// </summary>
		[DataMember]
		public string OwnerCode
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the UploadStagingFolder
		/// </summary>
		public string UploadStagingFolder
		{
			get
			{
				if (string.IsNullOrWhiteSpace(this.uploadStagingFolder))
				{
					return string.Empty;
				}

				return this.uploadStagingFolder.TrimEnd("\\".ToCharArray()) + "\\";
			}

			set
			{
				if (string.IsNullOrWhiteSpace(value))
				{
					this.uploadStagingFolder = string.Empty;
				}
				else
				{
					this.uploadStagingFolder = value.TrimEnd("\\".ToCharArray()) + "\\";
				}
			}
		}

		/// <summary>
		/// Gets or sets the ArchiveFolder
		/// </summary>
		public string ArchiveFolder
		{
			get
			{
				if (string.IsNullOrWhiteSpace(this.archiveFolder))
				{
					return string.Empty;
				}

				return this.archiveFolder.TrimEnd("\\".ToCharArray()) + "\\";
			}

			set
			{
				if (string.IsNullOrWhiteSpace(value))
				{
					this.archiveFolder = string.Empty;
				}
				else
				{
					this.archiveFolder = value.TrimEnd("\\".ToCharArray()) + "\\";
				}
			}
		}

		/// <summary>
		/// Gets or sets the FTP connection info
		/// </summary>
		[DataMember]
		public string ConnectionInfo
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the SendingCompanyCode
		/// </summary>
		[DataMember]
		public string SendingCompanyCode
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to send via FTP
		/// </summary>
		[DataMember]
		public bool SendViaFTP
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to send secure
		/// </summary>
		[DataMember]
		public bool SendSecure
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the CompanyNames
		/// </summary>
		public List<string> CompanyNames
		{
			get
			{
				try
				{
					if (!string.IsNullOrEmpty(this.companyNames))
					{
						return (List<string>)XmlObjConverter.FromXml(this.companyNames, typeof(List<string>));
					}
				}
				catch
				{			
				}

				return new List<string>();
			}

			set
			{
				try
				{
					if (value != null && value.Count > 0)
					{
						this.companyNames = XmlObjConverter.ToXml(value, typeof(List<string>));
					}
					else
					{
						this.companyNames = string.Empty;
					}
				}
				catch
				{
					this.companyNames = string.Empty;
				}
			}
		}

		/// <summary>
		/// Gets or sets the LatestRowVersion
		/// </summary>
		[DataMember]
		public long LatestRowVersion
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the LastExportTime
		/// </summary>
		[DataMember]
		public DateTimeOffset LastExportTime
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the ExportFrequency
		/// </summary>
		[DataMember]
		public int ExportFrequency
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the BaselineDate
		/// </summary>
		[DataMember]
		public DateTimeOffset BaselineDate
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether to exclude empty files
		/// </summary>
		[DataMember]
		public bool ExcludeEmptyFiles
		{
			get;
			set;
		}

		/// <summary>
		/// Gets a value indicating whether to initiate send.
		/// </summary>
		public bool InitiateSend
		{
			get
			{
                bool bReturnValue = false;
                if (UseTimeOfDay)
                {
                    bReturnValue = DateTime.Now >= nextExportTime;
                }
                else
                {
                    TimeSpan duration = new TimeSpan(0, 0, 0, (int)ExportFrequency, 0);
                    bReturnValue = (DateTimeOffset.Now - LastExportTime) >= duration;
                }
                return bReturnValue;
			}
		}

        /// <summary>
        /// Gets a value indicating whether to use the time of day for sending.
        /// </summary>
        [DataMember]
        public bool UseTimeOfDay
        {
            get; 
            set;
        }

        /// <summary>
        /// Gets a value for the Next Export time.
        /// </summary>
        [DataMember]
        public DateTimeOffset NextExportTime
        {
            get { return nextExportTime; }
            set { nextExportTime = value; }
        }

        /// <summary>
		/// Indicates how to transfer a file once it's been exported.
		/// </summary>
		[DataMember]
        public Constants.FileSendMethodEnum SendMethod
        {
            get;
            set;
        }

        /// <summary>
        /// If the SendMethod is WebService, this property specifies the ID of the web service plug-in type to use in transferring the export file.
        /// </summary>
        [DataMember]
        public string WebServicePluginType
        {
            get;
            set;
        }

        /// <summary>
        /// If the SendMethod is WebService, this property specifies the web service plug-in configuration string.
        /// </summary>
        [DataMember]
        public string WebServiceConfiguration
        {
            get;
            set;
        }

        #endregion

        /// <summary>
        /// Generates the dynamic SQL to select an ExportRequestClass object from
        /// the database with the specified request ID
        /// </summary>
        /// <param name="cmd">The SqlCommand object to be populated with the generated select command text </param>
        /// <param name="requestId">The specified request ID</param>
        public static void PrepareGetRequestByIDSqlCommand(SqlCommand cmd, string requestId)
		{
			cmd.CommandText = "SELECT * FROM tblExportRequest WHERE RequestID = @RequestID";
			var param = new SqlParameter("@RequestID", SqlDbType.NVarChar) { Value = requestId };
			cmd.Parameters.Add(param);
		}

		/// <summary>
		/// Generates the dynamic SQL to select an ExportRequestClass object from
		/// the database with the specified identity guid
		/// </summary>
		/// <param name="cmd">The SqlCommand object to be populated with the generated select command text</param>
		/// <param name="identityGuid">Identifies the record to retrieve from tblExportRequest</param>
		public static void SelectSQL(SqlCommand cmd, Guid identityGuid)
		{
			cmd.CommandText = "SELECT * FROM tblExportRequest WHERE ExportRequestGuid = @ExportRequestGuid";
			var param = new SqlParameter("@ExportRequestGuid", SqlDbType.UniqueIdentifier) { Value = identityGuid };
			cmd.Parameters.Add(param);
		}

		/// <summary>
		/// Generates the dynamic SQL to select all ExportRequestClass objects from the database
		/// </summary>
		/// <param name="cmd">The SqlCommand object to be populated with the generated select command text</param>
		public static void PrepareSelectAllSqlCommand(SqlCommand cmd)
		{
			cmd.CommandText = "SELECT * FROM tblExportRequest";
		}

		/// <summary>
		/// Resets the ExportRequestClass object to its initial state
		/// </summary>
		public override sealed void Reset()
		{
			base.Reset();
			this.RequestId = string.Empty;
			this.InterfaceId = DefaultInterfaceId;
			this.OwnerCode = string.Empty;
			this.uploadStagingFolder = DefaultUploadStagingFolder;
			this.archiveFolder = DefaultArchiveFolder;
			this.ConnectionInfo = string.Empty;
			this.SendingCompanyCode = string.Empty;
			this.SendViaFTP = false;
			this.SendSecure = false;
			this.companyNames = string.Empty;
			this.LatestRowVersion = 0;
			this.LastExportTime = TimeConverter.MinFMDate;
			this.ExportFrequency = DefaultExportFrequency;
			this.BaselineDate = TimeConverter.MinFMDate;
			this.ExcludeEmptyFiles = false;
            this.nextExportTime = TimeConverter.MinFMDate + new TimeSpan(2, 0, 0);
            UseTimeOfDay = true;
            this.SendMethod = Constants.FileSendMethodEnum.None;
            this.WebServicePluginType = null;
            this.WebServiceConfiguration = null;
        }

		/// <summary>
		/// Loads the ExportRequestClass data retrieved from the database
		/// </summary>
		/// <param name="set">The DataSet retrieved from the database</param>
		public void Load(DataSet set)
		{
			if (set == null)
			{
				throw new ArgumentNullException("set");
			}

			this.Reset();

			DataTable table = set.Tables[0];

			if (table.Rows.Count == 0)
			{
				return;
			}

			DataRow row = table.Rows[0];

			this.Load(row);
		}

		/// <summary>
		/// Loads the ExportRequestClass data retrieved from the database
		/// </summary>
		/// <param name="row">The DataRow retrieved from the database</param>
		public void Load(DataRow row)
		{
			if (row == null)
			{
				throw new ArgumentNullException("row");
			}

			this.Reset();

			this.IdentityGuid = DataObject.getValue<Guid>(row["ExportRequestGuid"], Guid.Empty);

			this.RequestId = DataObject.getValue<string>(row["RequestID"], string.Empty);
			this.InterfaceId = DataObject.getValue<string>(row["InterfaceID"], DefaultInterfaceId);
			this.OwnerCode = DataObject.getValue<string>(row["OwnerCode"], string.Empty);
			this.uploadStagingFolder = DataObject.getValue<string>(row["UploadStagingFolder"], DefaultUploadStagingFolder);
			this.archiveFolder = DataObject.getValue<string>(row["ArchiveFolder"], DefaultArchiveFolder);
			this.ConnectionInfo = DataObject.getValue<string>(row["ConnectionInfo"], string.Empty);
			this.SendingCompanyCode = DataObject.getValue<string>(row["SendingCompanyCode"], string.Empty);
			this.SendViaFTP = DataObject.getValue<bool>(row["SendViaFTP"], false);
			this.SendSecure = DataObject.getValue<bool>(row["SendSecure"], false);
			this.companyNames = DataObject.getValue<string>(row["CompanyNames"], string.Empty);
			this.LatestRowVersion = DataObject.getValue<long>(row["LatestRowVersion"], 0);
			this.LastExportTime = DataObject.getValue<DateTimeOffset>(row["LastExportTime"], TimeConverter.MinFMDate);
			this.ExportFrequency = DataObject.getValue<int>(row["ExportFrequency"], DefaultExportFrequency);
			this.BaselineDate = DataObject.getValue<DateTimeOffset>(row["BaselineDate"], TimeConverter.MinFMDate);
			this.ExcludeEmptyFiles = DataObject.getValue<bool>(row["ExcludeEmptyFiles"], false);
            this.UseTimeOfDay = DataObject.getValue<bool>(row["UseTimeOfDay"], false);
            this.nextExportTime = DataObject.getValue<DateTimeOffset>(row["NextExportTime"], TimeConverter.MinFMDate);
            this.SendMethod = DataObject.getValue<Constants.FileSendMethodEnum>(row["SendMethod"], Constants.FileSendMethodEnum.None);
            this.WebServicePluginType = DataObject.getValue<string>(row["WebServicePluginType"], null);
            this.WebServiceConfiguration = DataObject.getValue<string>(row["WebServiceConfiguration"], null);

            this._CreatedDate = DataObject.getValue<DateTimeOffset>(row["CreatedDate"], DateTimeOffset.Now);
			this._CreatedBy = DataObject.getValue<string>(row["CreatedBy"], BaseDataObject.ADMIN);
			this._UpdatedDate = DataObject.getValue<DateTimeOffset>(row["UpdatedDate"], this._CreatedDate);
			this._UpdatedBy = DataObject.getValue<string>(row["UpdatedBy"], BaseDataObject.ADMIN);
		}

        public void SetNextExportTime()
        {
            DateTimeOffset dtNow = DateTimeOffset.Now;
            TimeSpan duration = new TimeSpan(0, 0, 0, ExportFrequency);
            while (nextExportTime < dtNow)
            {
                nextExportTime += duration;
            }
        }


		#region paramaterized SQL

		/// <summary>
		/// Generates the dynamic SQL to insert an ExportRequestClass object into the database
		/// </summary>
		/// <param name="cmd">The SqlCommand object to be populated with the generated insert command text</param>
		public void PrepareInsertSqlCommand(SqlCommand cmd)
		{
			cmd.CommandText = "INSERT INTO tblExportRequest (" +
								"RequestID," +
								"InterfaceID," +
								"OwnerCode," +
								"UploadStagingFolder," +
								"ArchiveFolder," +
								"ConnectionInfo," +
								"SendingCompanyCode," +
								"SendViaFTP," +
								"SendSecure," +
								"CompanyNames," +
								"LatestRowVersion," +
								"LastExportTime," +
								"ExportFrequency," +
								"BaselineDate," +
								"ExcludeEmptyFiles," +
                                "UseTimeOfDay," +
                                "NextExportTime," +
								"CreatedDate," +
								"CreatedBy," +
								"UpdatedDate," +
                                "UpdatedBy," +
                                "SendMethod," +
                                "WebServicePluginType," +
                                "WebServiceConfiguration" +
                            ") VALUES (" +
								"@RequestID," +
								"@InterfaceID," +
								"@OwnerCode," +
								"@UploadStagingFolder," +
								"@ArchiveFolder," +
								"@ConnectionInfo," +
								"@SendingCompanyCode," +
								"@SendViaFTP," +
								"@SendSecure," +
								"@CompanyNames," +
								"@LatestRowVersion," +
								"@LastExportTime," +
								"@ExportFrequency," +
								"@BaselineDate," +
								"@ExcludeEmptyFiles," +
                                "@UseTimeOfDay," +
                                "@NextExportTime," +
								"@CreatedDate," +
								"@CreatedBy," +
								"@UpdatedDate," +
                                "@UpdatedBy," +
                                "@SendMethod," +
                                "@WebServicePluginType," +
                                "@WebServiceConfiguration)";

            SqlParameter param = null;

			param = new SqlParameter("@RequestID", SqlDbType.NVarChar) { Value = this.RequestId };
			cmd.Parameters.Add(param);

			param = new SqlParameter("@InterfaceID", SqlDbType.NVarChar) { Value = this.InterfaceId };
			cmd.Parameters.Add(param);

			param = new SqlParameter("@OwnerCode", SqlDbType.NVarChar) { Value = this.OwnerCode };
			cmd.Parameters.Add(param);

			param = new SqlParameter("@UploadStagingFolder", SqlDbType.NVarChar) { Value = this.UploadStagingFolder };
			cmd.Parameters.Add(param);

			param = new SqlParameter("@ArchiveFolder", SqlDbType.NVarChar) { Value = this.ArchiveFolder };
			cmd.Parameters.Add(param);

			param = new SqlParameter("@ConnectionInfo", SqlDbType.NVarChar, -1) { Value = this.ConnectionInfo };
			cmd.Parameters.Add(param);

			param = new SqlParameter("@SendingCompanyCode", SqlDbType.NVarChar) { Value = this.SendingCompanyCode };
			cmd.Parameters.Add(param);

			param = new SqlParameter("@SendViaFTP", SqlDbType.Bit) { Value = this.SendViaFTP };
			cmd.Parameters.Add(param);

			param = new SqlParameter("@SendSecure", SqlDbType.Bit) { Value = this.SendSecure };
			cmd.Parameters.Add(param);

			param = new SqlParameter("@CompanyNames", SqlDbType.NVarChar, -1) { Value = this.companyNames };
			cmd.Parameters.Add(param);

			param = new SqlParameter("@LatestRowVersion", SqlDbType.BigInt) { Value = this.LatestRowVersion };
			cmd.Parameters.Add(param);

			param = new SqlParameter("@LastExportTime", SqlDbType.DateTimeOffset) { Value = this.LastExportTime };
			cmd.Parameters.Add(param);

			param = new SqlParameter("@ExportFrequency", SqlDbType.Int) { Value = this.ExportFrequency };
			cmd.Parameters.Add(param);

			param = new SqlParameter("@BaselineDate", SqlDbType.DateTimeOffset) { Value = this.BaselineDate };
			cmd.Parameters.Add(param);

			param = new SqlParameter("@ExcludeEmptyFiles", SqlDbType.Bit) { Value = this.ExcludeEmptyFiles };
			cmd.Parameters.Add(param);

            param = new SqlParameter("@UseTimeOfDay", SqlDbType.Bit) { Value = this.UseTimeOfDay };
            cmd.Parameters.Add(param);

            param = new SqlParameter("@NextExportTime", SqlDbType.DateTimeOffset) { Value = this.NextExportTime };
            cmd.Parameters.Add(param);

			param = new SqlParameter("@CreatedDate", SqlDbType.DateTimeOffset) { Value = this.CreatedDate };
			cmd.Parameters.Add(param);

			param = new SqlParameter("@CreatedBy", SqlDbType.NVarChar) { Value = this.CreatedBy };
			cmd.Parameters.Add(param);

			param = new SqlParameter("@UpdatedDate", SqlDbType.DateTimeOffset) { Value = this.UpdatedDate };
			cmd.Parameters.Add(param);

			param = new SqlParameter("@UpdatedBy", SqlDbType.NVarChar) { Value = this.UpdatedBy };
			cmd.Parameters.Add(param);

            param = new SqlParameter("@SendMethod", SqlDbType.Int) { Value = this.SendMethod };
            cmd.Parameters.Add(param);

            param = new SqlParameter("@WebServicePluginType", SqlDbType.NVarChar) { Value = this.WebServicePluginType };
            if (this.WebServicePluginType == null)
                param.Value = DBNull.Value;
            cmd.Parameters.Add(param);

            param = new SqlParameter("@WebServiceConfiguration", SqlDbType.NVarChar) { Value = this.WebServiceConfiguration };
            if (this.WebServiceConfiguration == null)
                param.Value = DBNull.Value;
            cmd.Parameters.Add(param);
        }

		/// <summary>
		/// Generates the dynamic SQL to update an ExportRequestClass object in the database
		/// </summary>
		/// <param name="cmd">The SqlCommand object to be populated with the generated update command text</param>
		public void PrepareUpdateSqlCommand(SqlCommand cmd)
		{
			cmd.CommandText = "UPDATE tblExportRequest SET " +
								"InterfaceID = @InterfaceID," +
								"OwnerCode = @OwnerCode," +
								"UploadStagingFolder = @UploadStagingFolder," +
								"ArchiveFolder = @ArchiveFolder," +
								"ConnectionInfo = @ConnectionInfo," +
								"SendingCompanyCode = @SendingCompanyCode," +
								"SendViaFTP = @SendViaFTP," +
								"SendSecure = @SendSecure," +
								"CompanyNames = @CompanyNames," +
								"LatestRowVersion = @LatestRowVersion," +
								"LastExportTime = @LastExportTime," +
								"ExportFrequency = @ExportFrequency," +
								"BaselineDate = @BaselineDate," +
								"ExcludeEmptyFiles = @ExcludeEmptyFiles," +
                                "UseTimeOfDay = @useTimeOfDay, " +
                                "NextExportTime = @nextExportTime, " + 
								"UpdatedDate = @UpdatedDate," +
                                "UpdatedBy = @UpdatedBy, " +
                                "SendMethod = @SendMethod, " +
                                "WebServicePluginType = @WebServicePluginType, " +
                                "WebServiceConfiguration = @WebServiceConfiguration " +
                            "WHERE ExportRequestGuid = @ExportRequestGuid";

			SqlParameter param = new SqlParameter("@InterfaceID", SqlDbType.NVarChar) { Value = this.InterfaceId };
			cmd.Parameters.Add(param);

			param = new SqlParameter("@OwnerCode", SqlDbType.NVarChar) { Value = this.OwnerCode };
			cmd.Parameters.Add(param);

			param = new SqlParameter("@UploadStagingFolder", SqlDbType.NVarChar) { Value = this.UploadStagingFolder };
			cmd.Parameters.Add(param);

			param = new SqlParameter("@ArchiveFolder", SqlDbType.NVarChar) { Value = this.ArchiveFolder };
			cmd.Parameters.Add(param);

			param = new SqlParameter("@ConnectionInfo", SqlDbType.NVarChar, -1) { Value = this.ConnectionInfo };
			cmd.Parameters.Add(param);

			param = new SqlParameter("@SendingCompanyCode", SqlDbType.NVarChar) { Value = this.SendingCompanyCode };
			cmd.Parameters.Add(param);

			param = new SqlParameter("@SendViaFTP", SqlDbType.Bit) { Value = this.SendViaFTP };
			cmd.Parameters.Add(param);

			param = new SqlParameter("@SendSecure", SqlDbType.Bit) { Value = this.SendSecure };
			cmd.Parameters.Add(param);

			param = new SqlParameter("@CompanyNames", SqlDbType.NVarChar, -1) { Value = this.companyNames };
			cmd.Parameters.Add(param);

			param = new SqlParameter("@LatestRowVersion", SqlDbType.BigInt) { Value = this.LatestRowVersion };
			cmd.Parameters.Add(param);

			param = new SqlParameter("@LastExportTime", SqlDbType.DateTimeOffset) { Value = this.LastExportTime };
			cmd.Parameters.Add(param);

			param = new SqlParameter("@ExportFrequency", SqlDbType.Int) { Value = this.ExportFrequency };
			cmd.Parameters.Add(param);

			param = new SqlParameter("@BaselineDate", SqlDbType.DateTimeOffset) { Value = this.BaselineDate };
			cmd.Parameters.Add(param);

			param = new SqlParameter("@ExcludeEmptyFiles", SqlDbType.Bit) { Value = this.ExcludeEmptyFiles };
			cmd.Parameters.Add(param);

            param = new SqlParameter("@UseTimeOfDay", SqlDbType.Bit) { Value = this.UseTimeOfDay };
            cmd.Parameters.Add(param);

            param = new SqlParameter("@NextExportTime", SqlDbType.DateTimeOffset) { Value = this.NextExportTime };
            cmd.Parameters.Add(param);

			param = new SqlParameter("@UpdatedDate", SqlDbType.DateTimeOffset) { Value = this.UpdatedDate };
			cmd.Parameters.Add(param);

			param = new SqlParameter("@UpdatedBy", SqlDbType.NVarChar) { Value = this.UpdatedBy };
			cmd.Parameters.Add(param);

			param = new SqlParameter("@ExportRequestGuid", SqlDbType.UniqueIdentifier) { Value = this.IdentityGuid };
			cmd.Parameters.Add(param);

            param = new SqlParameter("@SendMethod", SqlDbType.Int) { Value = this.SendMethod };
            cmd.Parameters.Add(param);

            param = new SqlParameter("@WebServicePluginType", SqlDbType.NVarChar) { Value = this.WebServicePluginType };
            if (this.WebServicePluginType == null)
                param.Value = DBNull.Value;
            cmd.Parameters.Add(param);

            param = new SqlParameter("@WebServiceConfiguration", SqlDbType.NVarChar) { Value = this.WebServiceConfiguration };
            if (this.WebServiceConfiguration == null)
                param.Value = DBNull.Value;
            cmd.Parameters.Add(param);
        }

		/// <summary>
		/// Generates the dynamic SQL to delete an ExportRequestClass object from the database
		/// </summary>
		/// <param name="cmd">The SqlCommand object to be populated with the generated delete command text</param>
		public void PreparePurgeSingleSqlCommand(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM tblExportRequest WHERE ExportRequestGuid = @ExportRequestGuid";
			var param = new SqlParameter("@ExportRequestGuid", SqlDbType.UniqueIdentifier) { Value = this.IdentityGuid };
			cmd.Parameters.Add(param);
		}

		#endregion
	}
}

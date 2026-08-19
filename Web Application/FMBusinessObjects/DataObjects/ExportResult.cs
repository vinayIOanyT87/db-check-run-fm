// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportResult.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the EXPORT_INTERFACE_RESULT_TYPE type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections;
    using System.Collections.Generic;
    using System.Data;
	using System.Data.SqlClient;
	using System.Runtime.Serialization;
	using System.Xml.Serialization;

	using FMBusinessObjects.UtilityObjects;

	#region Public enumeration
	/// <summary>
	/// The expor t_ resul t_ type.
	/// </summary>
	public enum EXPORT_RESULT_TYPE
	{
		CLOSEOUT,
		TRANSACTION,
		TANK,
		MAINTENANCE,
		QUALITY,
		EXPORT_RESULT,
		MAX
	}
	#endregion

	#region Export Result Collection Class
	/// <summary>
	/// The export result collection class.
	/// </summary>
	[Serializable]
	[KnownType(typeof(ExportResultClass))]
	[CollectionDataContract]
    public class ExportResultCollectionClass : List<ExportResultClass>
	{
 
	}
	#endregion

	/// <summary>
	/// The export result class.
	/// </summary>
	[DataContract]
	[Serializable( )]
	[XmlRoot("ExportResult")]
	[XmlType("ExportResult")]
	public class ExportResultClass : BaseDataObject
	{
		#region Private data members
		[DataMember] private string								batchId;
		[DataMember] private EXPORT_RESULT_TYPE					type;
		[DataMember] private string								interfaceName;
		[DataMember] private long?								transVersion;
		[DataMember] private int								failedCount;
		[DataMember] private int								successCount;
		[DataMember] private DateTimeOffset						transDateTime;
		[DataMember] protected ExportResultDetailCollectionClass	exportResultDetailCollection;
		[DataMember] private string								archiveFileName;
		#endregion

		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="ExportResultClass"/> class.
		/// </summary>
		public ExportResultClass( )
		{
			this.Init( );
		}
		#endregion

		#region Properties
		// TODO: Use when updated for synchronization.
		///// <summary>
		///// Gets or sets the export result GUID as a string.
		///// </summary>
		//public string ExportResultGuidString
		//{
		//	get
		//	{
		//		return this._IdentityGuid.ToString();
		//	}

		//	set
		//	{
		//		this._IdentityGuid = Guid.Empty;

		//		if (string.IsNullOrEmpty(value) == false)
		//		{
		//			this._IdentityGuid = Guid.Parse(value);
		//		}
		//	}
		//}

		/// <summary>
		/// Gets or sets the batch id.
		/// </summary>
		public string BatchId
		{
			get { return this.batchId; }
			set { this.SetString("BatchId", 64, value, ref this.batchId); }
		}

		/// <summary>
		/// Gets or sets the type.
		/// </summary>
		public EXPORT_RESULT_TYPE Type
		{
			get { return this.type; }
			set { this.type = value; }
		}

		/// <summary>
		/// Gets or sets the interface name.
		/// </summary>
		public string InterfaceName
		{
			get { return this.interfaceName; }
			set { this.SetString("Interface Name", 150, value, ref this.interfaceName); }
		}

		/// <summary>
		/// Gets or sets the trans version.
		/// </summary>
		public long? TransVersion
		{
			get { return this.transVersion; }
			set { this.transVersion = value; }
		}

		/// <summary>
		/// Gets or sets the failed count.
		/// </summary>
		public int FailedCount
		{
			get { return this.failedCount; }
			set { this.failedCount = value; }
		}

		/// <summary>
		/// Gets or sets the success count.
		/// </summary>
		public int SuccessCount
		{
			get { return this.successCount; }
			set { this.successCount = value; }
		}

		/// <summary>
		/// Gets or sets the trans date time.
		/// </summary>
		public DateTimeOffset TransDateTime
		{
			get { return this.transDateTime; }
			set { this.transDateTime = value; }
		}

		/// <summary>
		/// Gets or sets the export result detail collection.
		/// </summary>
		public ExportResultDetailCollectionClass ExportResultDetailCollection
		{
			get { return this.exportResultDetailCollection; }
			set { this.exportResultDetailCollection = value; }
		}

		/// <summary>
		/// Gets or sets the archive file name.
		/// </summary>
		public string ArchiveFileName
		{
			get { return this.archiveFileName; } 	
			set { this.archiveFileName = value; }
		}

		/// <summary>
		/// Gets or sets the entity type.
		/// </summary>
		[XmlIgnoreAttribute]
		public override ENTITY_TYPE EntityType
		{
			get { return ENTITY_TYPE.EXPORT_RESULT; }
		}

		/// <summary>
		/// Gets the parent entity type.
		/// </summary>
		[XmlIgnoreAttribute]
		public override ENTITY_TYPE ParentEntityType
		{
			get { return ENTITY_TYPE.NONE; }
		}
		#endregion


		#region public methods
		/// <summary>
		/// The insert SQL.
		/// </summary>
		/// <param name="cmd">
		/// The SQL command object.
		/// </param>
		public void InsertSql(SqlCommand cmd)
		{
			cmd.CommandText = "INSERT INTO tblExportResults " +
				"(SiteGuid, " +
				"BatchId, " +
				"LookupExportResultTypeIndex, " +
				"InterfaceName, " +
				"TransVersion, " +
				"FailedCount, " +
				"SuccessCount, " +
				"TransDateTime, " +
				"CreatedDate, " +
				"CreatedBy, " +
				"UpdatedDate, " +
				"UpdatedBy, " +
				"ExportResultGuid, " +
				"ArchiveFileName" +
				") VALUES (" +
				"@SiteGuid, " +
				"@BatchId, " +
				"@LookupExportResultTypeIndex, " +
				"@InterfaceName, " +
				"@TransVersion, " +
				"@FailedCount, " +
				"@SuccessCount, " +
				"@TransDateTime, " +
				"@CreatedDate, " +
				"@CreatedBy, " +
				"@UpdatedDate, " +
				"@UpdatedBy, " +
				"@ExportResultGuid, " +
				"@ArchiveFileName" +
				")";

			cmd.Parameters.Add("@ExportResultGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@BatchId", SqlDbType.NVarChar, 64);
			cmd.Parameters.Add("@LookupExportResultTypeIndex", SqlDbType.Int);
			cmd.Parameters.Add("@InterfaceName", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@TransVersion", SqlDbType.BigInt);
			cmd.Parameters.Add("@FailedCount", SqlDbType.Int);
			cmd.Parameters.Add("@SuccessCount", SqlDbType.Int);
			cmd.Parameters.Add("@TransDateTime", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@ArchiveFileName", SqlDbType.NVarChar, 150);

			cmd.Parameters["@ExportResultGuid"].Value				= this._IdentityGuid;
			cmd.Parameters["@SiteGuid"].Value						= this.SiteGuid;
			cmd.Parameters["@BatchId"].Value						= this.BatchId;
			cmd.Parameters["@LookupExportResultTypeIndex"].Value	= (int) this.Type;
			cmd.Parameters["@InterfaceName"].Value					= this.InterfaceName;

			if ( this.TransVersion == null )
			{
				cmd.Parameters["@TransVersion"].Value = DBNull.Value;
			}
			else
			{
				cmd.Parameters["@TransVersion"].Value = this.TransVersion;
			}

			cmd.Parameters["@FailedCount"].Value		= this.FailedCount;
			cmd.Parameters["@SuccessCount"].Value		= this.SuccessCount;
			cmd.Parameters["@TransDateTime"].Value		= this.TransDateTime;
			cmd.Parameters["@CreatedDate"].Value		= this.CreatedDate;
			cmd.Parameters["@CreatedBy"].Value			= this.CreatedBy;
			cmd.Parameters["@UpdatedDate"].Value		= this.UpdatedDate;
			cmd.Parameters["@UpdatedBy"].Value			= this.UpdatedBy;
			cmd.Parameters["@ArchiveFileName"].Value	= this.archiveFileName;
		}

		/// <summary>
		/// The select most recent SQL.
		/// </summary>
		/// <param name="cmd">
		/// The SQL command object.
		/// </param>
		public void SelectMostRecentSql(SqlCommand cmd)
		{
			cmd.CommandText = "SELECT TOP 1 *"
							+ " FROM tblExportResults"
							+ " WHERE LookupExportResultTypeIndex = @LookupExportResultTypeIndex "
							+ " AND InterfaceName = @InterfaceName "
							+ " AND SiteGuid = @SiteGuid "
							+ " ORDER BY ExportResultGuid DESC";

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@LookupExportResultTypeIndex", SqlDbType.Int);
			cmd.Parameters.Add("@InterfaceName", SqlDbType.NVarChar, 50);

			cmd.Parameters["@InterfaceName"].Value = this.InterfaceName;
			cmd.Parameters["@SiteGuid"].Value = this.SiteGuid;
			cmd.Parameters["@LookupExportResultTypeIndex"].Value = (int) this.Type;
		}

		/// <summary>
		/// The select maximum transaction version SQL.
		/// </summary>
		/// <param name="cmd">
		/// The SQL command.
		/// </param>
		public void SelectMaxTransVersionSql(SqlCommand cmd)
		{
			cmd.CommandText = "Select TOP 1 *"
				+ " FROM tblExportResults WITH (NOLOCK) "
				+ " WHERE LookupExportResultTypeIndex = @LookupExportResultTypeIndex "
				+ " AND InterfaceName = @InterfaceName "
				+ " AND SiteGuid = @SiteGuid "
				+ " ORDER BY TransVersion DESC";

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@LookupExportResultTypeIndex", SqlDbType.Int);
			cmd.Parameters.Add("@InterfaceName", SqlDbType.NVarChar, 50);

			cmd.Parameters["@InterfaceName"].Value					= this.InterfaceName;
			cmd.Parameters["@SiteGuid"].Value						= this.SiteGuid;
			cmd.Parameters["@LookupExportResultTypeIndex"].Value	= (int) this.Type;
		}

		/// <summary>
		/// The get GUID by interface name SQL.
		/// </summary>
		/// <param name="sqlCommand">
		/// The SQL command.
		/// </param>
		public void GetGuidByInterfaceNameSql(SqlCommand sqlCommand)
		{
			if ( sqlCommand != null )
			{
				sqlCommand.Parameters.Clear( );

				string sql = "SELECT ExportResultGuid FROM tblExportResults WITH (NOLOCK) WHERE InterfaceName = @InterfaceName ";
				sqlCommand.CommandText = sql;

				var parm = new SqlParameter("@InterfaceName", SqlDbType.NVarChar, 50) { Value = this.InterfaceName };
				sqlCommand.Parameters.Add(parm);
			}
		}

		/// <summary>
		/// The reset.
		/// </summary>
		public override void Reset( )
		{
			this.Init();
		}

		/// <summary>
		/// The load.
		/// </summary>
		/// <param name="dataSet">
		/// The data set.
		/// </param>
		/// <exception cref="ArgumentNullException">Null dataset parameter.
		/// </exception>
		public void Load(DataSet dataSet)
		{
			if ( dataSet == null )
			{
				throw new ArgumentNullException("dataSet");
			}

			this.Reset( );
			DataTable table = dataSet.Tables[0];

			if ( table.Rows.Count == 0 )
			{
				return;
			}

			DataRow row = table.Rows[0];

			this._IdentityGuid		= DataObject.getValue<Guid>(row["ExportResultGuid"], Guid.Empty);
			this._SiteGuid			= DataObject.getValue<Guid>(row["SiteGuid"], Guid.Empty);
			this.batchId			= DataObject.getValue<string>(row["BatchId"], string.Empty);
			this.type				= DataObject.getValue<EXPORT_RESULT_TYPE>(row["LookupExportResultTypeIndex"], EXPORT_RESULT_TYPE.MAX);
			this.interfaceName		= DataObject.getValue<string>(row["InterfaceName"], string.Empty);
			this.transVersion		= DataObject.getValue<long?>(row["TransVersion"], null);
			this.failedCount		= DataObject.getValue<int>(row["FailedCount"], 0);
			this.successCount		= DataObject.getValue<int>(row["SuccessCount"], 0);
			this.transDateTime		= DataObject.getValue<DateTimeOffset>(row["TransDateTime"], TimeConverter.Today( ));
			this.archiveFileName	= DataObject.getValue(row["ArchiveFileName"], string.Empty);
			this._CreatedDate		= DataObject.getValue<DateTimeOffset>(row["CreatedDate"], DateTimeOffset.Now);
			this._CreatedBy			= DataObject.getValue(row["CreatedBy"], BaseDataObject.ADMIN);
			this._UpdatedDate		= DataObject.getValue<DateTimeOffset>(row["UpdatedDate"], this._CreatedDate);
			this._UpdatedBy			= DataObject.getValue(row["UpdatedBy"], BaseDataObject.ADMIN);
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method initializes the object to its initial state.
		/// </summary>
		private void Init()
		{
			//base.Reset();
			this.batchId						= string.Empty;
			this.type							= EXPORT_RESULT_TYPE.MAX;
			this.interfaceName					= string.Empty;
			this.transVersion					= null;
			this.failedCount					= 0;
			this.successCount					= 0;
			this.transDateTime					= TimeConverter.Today( );
			this.exportResultDetailCollection	= new ExportResultDetailCollectionClass( );
			this.archiveFileName				= string.Empty;
		}
		#endregion
	}
}

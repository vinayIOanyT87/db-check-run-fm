// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportResultDetail.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ExportResultDetailCollectionClass type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Globalization;
	using System.Runtime.Serialization;
	using System.Xml.Serialization;

	#region Export Result Detail Collection Class
	/// <summary>
	/// The export result detail collection class.
	/// </summary>
	[XmlType("ResultDetails")]
	[Serializable]
	[KnownType(typeof(ExportResultDetailClass))]
	[CollectionDataContract]
    public class ExportResultDetailCollectionClass : List<ExportResultDetailClass>
	{

	}
	#endregion

	/// <summary>
	/// Summary description for ExportResultDetailClass.
	/// </summary>
	[DataContract]
	[Serializable]
	[XmlType("ResultDetail")]
	public class ExportResultDetailClass : BaseDataObject
	{
		#region Public data members
		/// <summary>
		/// The max export.
		/// </summary>
		public const int MaxExport = 10000;
		#endregion

		#region Private data member
		[DataMember] private Guid exportResultGuid;
		[DataMember] private string recordId;
		[DataMember] private bool fail;
		[DataMember] private long? transVersion;
		[DataMember] private string error;
		[DataMember] private string interfaceData01;
		[DataMember] private string interfaceData02;
		[DataMember] private string interfaceData03;
		[DataMember] private string interfaceData04;
		[DataMember] private string interfaceData05;
		[DataMember] private string interfaceData06;
		[DataMember] private string interfaceData07;
		[DataMember] private string interfaceData08;
		[DataMember] private List<string> interfaceNameList; 
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="ExportResultDetailClass"/> class.
		/// </summary>
		public ExportResultDetailClass( )
		{
			this.Init();
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the export result GUID.
		/// </summary>
		[XmlIgnoreAttribute]
		public Guid ExportResultGuid
		{
			get { return this.exportResultGuid; }
			set { this.exportResultGuid = value; }
		}

		// TODO: Use when updated for synchronization.
		///// <summary>
		///// Gets or sets the export result GUID as a string.
		///// </summary>
		//public string ExportResultGuidString
		//{
		//	get
		//	{
		//		return this.exportResultGuid.ToString();
		//	}

		//	set
		//	{
		//		this.exportResultGuid = Guid.Empty;

		//		if (string.IsNullOrEmpty(value) == false)
		//		{
		//			this.exportResultGuid = Guid.Parse(value);
		//		}
		//	}
		//}

		// TODO: Use when updated for synchronization.
		///// <summary>
		///// Gets or sets the export result detail GUID as a string.
		///// </summary>
		//public string ExportResultDetailGuidString
		//{
		//	get
		//	{
		//		return this.IdentityGuid.ToString();
		//	}

		//	set
		//	{
		//		this.IdentityGuid = Guid.Empty;

		//		if (string.IsNullOrEmpty(value) == false)
		//		{
		//			this.IdentityGuid = Guid.Parse(value);
		//		}
		//	}
		//}

		/// <summary>
		/// Gets or sets the record ID.
		/// </summary>
		[XmlElement("RecordID")]
		public string RecordId
		{
			get { return this.recordId; }
			set { this.SetString("RecordId", 64, value, ref this.recordId); }
		}

		/// <summary>
		/// Gets or sets a value indicating whether fail.
		/// </summary>
		public bool Fail
		{
			get { return this.fail; }
			set { this.fail = value; }
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
		/// Gets or sets the error.
		/// </summary>
		public string Error
		{
			get { return this.error; }
			set { this.error = value; }
		}

		/// <summary>
		/// Gets or sets the interface data 01.
		/// </summary>
		public string InterfaceData01
		{
			get { return this.interfaceData01; }
			set { this.interfaceData01 = value; }
		}

		/// <summary>
		/// Gets or sets the interface data 02.
		/// </summary>
		public string InterfaceData02
		{
			get { return this.interfaceData02; }
			set { this.interfaceData02 = value; }
		}

		/// <summary>
		/// Gets or sets the interface data 03.
		/// </summary>
		public string InterfaceData03
		{
			get { return this.interfaceData03; }
			set { this.interfaceData03 = value; }
		}

		/// <summary>
		/// Gets or sets the interface data 04.
		/// </summary>
		public string InterfaceData04
		{
			get { return this.interfaceData04; }
			set { this.interfaceData04 = value; }
		}

		/// <summary>
		/// Gets or sets the interface data 05.
		/// </summary>
		public string InterfaceData05
		{
			get { return this.interfaceData05; }
			set { this.interfaceData05 = value; }
		}

		/// <summary>
		/// Gets or sets the interface data 06.
		/// </summary>
		public string InterfaceData06
		{
			get { return this.interfaceData06; }
			set { this.interfaceData06 = value; }
		}

		/// <summary>
		/// Gets or sets the interface data 07.
		/// </summary>
		public string InterfaceData07
		{
			get { return this.interfaceData07; }
			set { this.interfaceData07 = value; }
		}

		/// <summary>
		/// Gets or sets the interface data 08.
		/// </summary>
		public string InterfaceData08
		{
			get { return this.interfaceData08; }
			set { this.interfaceData08 = value; }
		}

		/// <summary>
		/// Gets or sets the interface name collection.
		/// </summary>
		[XmlIgnoreAttribute]
		public List<string> InterfaceNameList
		{
			get { return this.interfaceNameList; }
			set { this.interfaceNameList = value; }
		}

		/// <summary>
		/// Gets the entity type.
		/// </summary>
		[XmlIgnoreAttribute]
		public override ENTITY_TYPE EntityType
		{
			get { return ENTITY_TYPE.EXPORT_RESULT_DETAIL;  }
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

		#region Public and internal methods
		/// <summary>
		/// This method will populate the SQL command with the insert SQL and 
		/// parameters.
		/// </summary>
		/// <param name="cmd">
		/// The SQL command.
		/// </param>
		public void InsertSql(SqlCommand cmd)
		{
			cmd.CommandText = "INSERT INTO tblExportResultDetails " +
				"(ExportResultGuid," +
				"RecordId," +
				"Fail," +
				"TransVersion," +
				"Error," +
				"CreatedDate," +
				"CreatedBy," +
				"UpdatedDate," +
				"UpdatedBy," +
				"ExportResultDetailGuid," +
				"InterfaceData01," +
				"InterfaceData02," +
				"InterfaceData03," +
				"InterfaceData04," +
				"InterfaceData05," +
				"InterfaceData06," +
				"InterfaceData07," +
				"InterfaceData08" +
				") VALUES (" +
				"@ExportResultGuid," +
				"@RecordId," +
				"@Fail," +
				"@TransVersion," +
				"@Error," +
				"@CreatedDate," +
				"@CreatedBy," +
				"@UpdatedDate," +
				"@UpdatedBy," +
				"@ExportResultDetailGuid," +
				"@InterfaceData01," +
				"@InterfaceData02," +
				"@InterfaceData03," +
				"@InterfaceData04," +
				"@InterfaceData05," +
				"@InterfaceData06," +
				"@InterfaceData07," +
				"@InterfaceData08" +
				")";

			cmd.Parameters.Add("@ExportResultGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@RecordId", SqlDbType.NVarChar, 64);
			cmd.Parameters.Add("@Fail", SqlDbType.Bit);
			cmd.Parameters.Add("@TransVersion", SqlDbType.BigInt);
			cmd.Parameters.Add("@Error", SqlDbType.NVarChar, 250);
			cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@ExportResultDetailGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@InterfaceData01", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@InterfaceData02", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@InterfaceData03", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@InterfaceData04", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@InterfaceData05", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@InterfaceData06", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@InterfaceData07", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@InterfaceData08", SqlDbType.NVarChar, 100);

			cmd.Parameters["@ExportResultGuid"].Value = this.ExportResultGuid;
			cmd.Parameters["@RecordId"].Value = this.RecordId;

			if ( this.Fail )
			{
				cmd.Parameters["@Fail"].Value = 1;
			}
			else
			{
				cmd.Parameters["@Fail"].Value = 0;
			}

			if ( this.TransVersion == null )
			{
				cmd.Parameters["@TransVersion"].Value = DBNull.Value;
			}
			else
			{
				cmd.Parameters["@TransVersion"].Value = this.TransVersion;
			}

			cmd.Parameters["@Error"].Value					= this.Error;
			cmd.Parameters["@CreatedDate"].Value			= this.CreatedDate;
			cmd.Parameters["@CreatedBy"].Value				= this.CreatedBy;
			cmd.Parameters["@UpdatedDate"].Value			= this.UpdatedDate;
			cmd.Parameters["@UpdatedBy"].Value				= this.UpdatedBy;
			cmd.Parameters["@ExportResultDetailGuid"].Value = this.IdentityGuid;
			cmd.Parameters["@InterfaceData01"].Value		= this.interfaceData01;
			cmd.Parameters["@InterfaceData02"].Value		= this.interfaceData02;
			cmd.Parameters["@InterfaceData03"].Value		= this.interfaceData03;
			cmd.Parameters["@InterfaceData04"].Value		= this.interfaceData04;
			cmd.Parameters["@InterfaceData05"].Value		= this.interfaceData05;
			cmd.Parameters["@InterfaceData06"].Value		= this.interfaceData06;
			cmd.Parameters["@InterfaceData07"].Value		= this.interfaceData07;
			cmd.Parameters["@InterfaceData08"].Value		= this.interfaceData08;
		}

		/// <summary>
		/// This method creates a SQL statement to update the export result detail
		/// record in the database.  It populates the SQL command parameter with
		/// the appropriate SQL and parameters.
		/// </summary>
		/// <param name="cmd">
		/// The SQL command object.
		/// </param>
		public void ModifySql(SqlCommand cmd)
		{
			cmd.Parameters.Clear( );

			const string SQL = "UPDATE tblExportResultDetails SET " +
			                   "ExportResultGuid = @ExportResultGuid, " +
			                   "RecordId = @RecordId, " +
			                   "Fail = @Fail, " +
			                   "TransVersion = @TransVersion, " +
			                   "Error = @Error, " +
			                   "CreatedDate = @CreatedDate, " +
			                   "CreatedBy = @CreatedBy, " +
			                   "UpdatedDate = @UpdatedDate, " +
			                   "UpdatedBy = @UpdatedBy, " +
			                   "InterfaceData01 = @InterfaceData01, " +
			                   "InterfaceData02 = @InterfaceData02, " +
			                   "InterfaceData03 = @InterfaceData03, " +
			                   "InterfaceData04 = @InterfaceData04, " +
			                   "InterfaceData05 = @InterfaceData05, " +
			                   "InterfaceData06 = @InterfaceData06, " +
			                   "InterfaceData07 = @InterfaceData07, " +
			                   "InterfaceData08 = @InterfaceData08 " +
			                   "WHERE ExportResultDetailGuid = @ExportResultDetailGuid ";

			cmd.CommandText = SQL;
			cmd.Parameters.Add("@ExportResultDetailGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@ExportResultGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@RecordId", SqlDbType.NVarChar, 64);
			cmd.Parameters.Add("@Fail", SqlDbType.Bit);
			cmd.Parameters.Add("@TransVersion", SqlDbType.BigInt);
			cmd.Parameters.Add("@Error", SqlDbType.NVarChar, 250);
			cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@InterfaceData01", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@InterfaceData02", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@InterfaceData03", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@InterfaceData04", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@InterfaceData05", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@InterfaceData06", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@InterfaceData07", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@InterfaceData08", SqlDbType.NVarChar, 100);

			cmd.Parameters["@ExportResultDetailGuid"].Value = this.IdentityGuid;
			cmd.Parameters["@ExportResultGuid"].Value		= this.exportResultGuid;
			cmd.Parameters["@RecordId"].Value				= this.RecordId;
			cmd.Parameters["@Fail"].Value					= this.fail;
			cmd.Parameters["@TransVersion"].Value			= this.transVersion;
			cmd.Parameters["@Error"].Value					= this.error;
			cmd.Parameters["@CreatedDate"].Value			= this._CreatedDate;
			cmd.Parameters["@CreatedBy"].Value				= this._CreatedBy;
			cmd.Parameters["@UpdatedDate"].Value			= this._UpdatedDate;
			cmd.Parameters["@UpdatedBy"].Value				= this._UpdatedBy;
			cmd.Parameters["@InterfaceData01"].Value		= this.interfaceData01;
			cmd.Parameters["@InterfaceData02"].Value		= this.interfaceData02;
			cmd.Parameters["@InterfaceData03"].Value		= this.interfaceData03;
			cmd.Parameters["@InterfaceData04"].Value		= this.interfaceData04;
			cmd.Parameters["@InterfaceData05"].Value		= this.interfaceData05;
			cmd.Parameters["@InterfaceData06"].Value		= this.interfaceData06;
			cmd.Parameters["@InterfaceData07"].Value		= this.interfaceData07;
			cmd.Parameters["@InterfaceData08"].Value		= this.interfaceData08;
		}

		/// <summary>
		/// This method creates a SQL statement to delete an export result detail
		/// record in the database.  It populates the SQL command parameter with
		/// the appropriate SQL and parameters.
		/// </summary>
		/// <param name="sqlCommand">
		/// The SQL command.
		/// </param>
		public void PurgeSql(SqlCommand sqlCommand)
		{
			sqlCommand.Parameters.Clear( );
			const string SQL = "DELETE FROM tblExportResultDetails WHERE ExportResultDetailGuid = @ExportResultDetailGuid ";

			sqlCommand.CommandText = SQL;
			sqlCommand.Parameters.Add("@ExportResultDetailGuid", SqlDbType.UniqueIdentifier);

			sqlCommand.Parameters["@ExportResultDetailGuid"].Value = this.IdentityGuid;
		}

		/// <summary>
		/// This method creates a SQL statement to get an export result detail
		/// record from the database based on the Export Result Detail GUID value.  It populates 
		/// the SQL command parameter with the appropriate SQL and parameters.
		/// </summary>
		/// <param name="sqlCommand">
		/// The SQL command.
		/// </param>
		public void GetByGuidSql(SqlCommand sqlCommand)
		{
			sqlCommand.Parameters.Clear( );
			const string SQL = "SELECT * FROM tblExportResultDetails WITH (NOLOCK) WHERE ExportResultDetailGuid = @ExportResultDetailGuid ";

			sqlCommand.CommandText = SQL;
			sqlCommand.Parameters.Add("@ExportResultDetailGuid", SqlDbType.UniqueIdentifier);

			sqlCommand.Parameters["@ExportResultDetailGuid"].Value = this.IdentityGuid;
		}

		/// <summary>
		/// This method will populate the SQL Command with the SQL to retrieve the
		/// results detail information based on the Record ID and TransVersion.
		/// </summary>
		/// <param name="sqlCommand">
		/// The SQL command.
		/// </param>
		public void GetGuidByRecordIdAndTransVersionSql(SqlCommand sqlCommand)
		{
			// Must have a Record ID and TransVersion.
			if ( string.IsNullOrEmpty(this.RecordId) || (this.transVersion == null) )
			{
				return;
			}

			sqlCommand.Parameters.Clear( );
			const string SQL = "SELECT ExportResultDetailGuid FROM tblExportResultDetails WITH (NOLOCK) " +
			                   "WHERE RecordId = @RecordId AND TransVersion = @TransVersion ";

			sqlCommand.CommandText = SQL;

			var parm = new SqlParameter("@RecordId", SqlDbType.NVarChar, 64) { Value = this.RecordId };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@TransVersion", SqlDbType.BigInt);
			if (this.TransVersion == null)
			{
				parm.Value = DBNull.Value;
			}
			else
			{
				parm.Value = this.TransVersion.Value;
			}
			
			sqlCommand.Parameters.Add(parm);
		}

		/// <summary>
		/// This method populates the SQL command to update the transaction status
		/// and flags that is related to the export result.
		/// </summary>
		/// <param name="sqlCommand">
		/// The SQL command.
		/// </param>
		public virtual void UpdateTransactionFlagsAndStatus(SqlCommand sqlCommand)
		{
			const int PostedStatus = 11;
			const int PendingStatus = 16;

			int transStatus;
			bool errorFlag;

			sqlCommand.Parameters.Clear();
			sqlCommand.CommandText = "UPDATE tblTransactions SET TransactionStatus = @TransStatus, ErrorFlag = @ErrorFlag " +
			                         "WHERE TransID = @RecordId ";

			if (this.fail)
			{
				transStatus = PendingStatus;
				errorFlag = true;
			}
			else
			{
				transStatus = PostedStatus;
				errorFlag = false;
			}

			var parm = new SqlParameter("@RecordId", SqlDbType.NVarChar, 64)
			{
				Value = this.RecordId
			};
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@TransStatus", SqlDbType.Int)
			{
				Value = transStatus
			};
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@ErrorFlag", SqlDbType.Bit)
			{
				Value = errorFlag ? 1 : 0
			};
			sqlCommand.Parameters.Add(parm);
		}

		/// <summary>
		/// This method creates a SQL statement to get history for a given transaction.  
		/// It populates the SQL command parameter with the appropriate SQL and parameters. 
		/// </summary>
		/// <param name="sqlCommand">
		/// The SQL command.
		/// </param>
		/// <param name="startDate">
		/// The start date.
		/// </param>
		/// <param name="endDate">
		/// The end date.
		/// </param>
		/// <param name="orderBy">
		/// The order by.
		/// </param>
		public void GetTransHistoryByRecordId(SqlCommand sqlCommand, DateTime? startDate, DateTime? endDate, string orderBy)
		{
			sqlCommand.Parameters.Clear( );
			string sql = "SELECT UpdatedDate, UpdatedBy, Error, Fail, TransVersion, InterfaceData01, InterfaceData02, " +
						 "InterfaceData03, InterfaceData04, InterfaceData05, InterfaceData06, InterfaceData07, InterfaceData08 " +
						 "FROM tblExportResultDetails WITH (NOLOCK) " +
						 "WHERE RecordId = @RecordId ";

			if ( (startDate != null) && (endDate != null) )
			{
				sql = sql + "AND UpdatedDate >= @StartDate AND UpdatedDate <= @EndDate ";
			}

			if ( string.IsNullOrEmpty(orderBy) == false )
			{
				sql = sql + " ORDER BY " + orderBy;
			}

			sqlCommand.CommandText = sql;

			var parm = new SqlParameter("@RecordId", SqlDbType.NVarChar, 64) { Value = this.RecordId };
			sqlCommand.Parameters.Add(parm);

			if ( (startDate != null) && (endDate != null) )
			{
				parm = new SqlParameter("@StartDate", SqlDbType.DateTime) { Value = startDate.Value };
				sqlCommand.Parameters.Add(parm);

				parm = new SqlParameter("@EndDate", SqlDbType.DateTime) { Value = endDate.Value };
				sqlCommand.Parameters.Add(parm);
			}
		}

		/// <summary>
		/// This method creates a SQL statement to get an export result detail
		/// record from the database based on the Record ID value and transVersion 
		/// It populates the SQL command parameter with the 
		/// appropriate SQL and parameters.
		/// </summary>
		/// <param name="sqlCommand">
		/// The SQL command.
		/// </param>
		public void GetByRecordIdAndTransVersion(SqlCommand sqlCommand)
		{
			sqlCommand.Parameters.Clear( );

			const string Select = "SELECT TOP(1) ERD.* ";
			const string From = "FROM tblExportResultDetails ERD WITH (NOLOCK) " +
								"INNER JOIN tblExportResults E ON ERD.ExportResultGuid = E.ExportResultGuid ";

			string where = "WHERE ERD.RecordId = @RecordId AND ERD.TransVersion = @TransVersion ";
			const string OrderBy = "ORDER BY ERD.UpdatedDate DESC ";

			bool noNulls = true;

			// Cannot have a null value for an interface name.
			foreach (string interfaceName in this.interfaceNameList)
			{
				if (string.IsNullOrEmpty(interfaceName))
				{
					noNulls = false;
				}
			}

			if (this.interfaceNameList.Count > 0 && noNulls)
			{
				const string InterfaceNameParm	= "@InterfaceName";
				int interfaceCount				= 0;
				string where2					= "AND E.InterfaceName IN ( ";

				foreach (string interfaceName in this.interfaceNameList)
				{
					string interfaceParm = InterfaceNameParm + interfaceCount;

					if (interfaceCount == 0)
					{
						where2 = where2 + interfaceParm;
					}
					else
					{
						where2 = where2 + ", " + interfaceParm;
					}

					sqlCommand.Parameters.Add(interfaceParm, SqlDbType.NVarChar, 150);
					sqlCommand.Parameters[interfaceParm].Value = interfaceName;

					interfaceCount++;
				}

				where = where + where2 + " ) ";
			}

			sqlCommand.CommandText = Select + From + where + OrderBy;

			sqlCommand.Parameters.Add("@RecordId", SqlDbType.NVarChar, 64);
			sqlCommand.Parameters["@RecordId"].Value = this.RecordId;
			sqlCommand.Parameters.Add("@TransVersion", SqlDbType.BigInt);

			if (this.transVersion == null)
			{
				sqlCommand.Parameters["@TransVersion"].Value = DBNull.Value;
			}
			else
			{
			    var version = this.TransVersion;
			    if (version != null)
			    {
			        sqlCommand.Parameters["@TransVersion"].Value = version.Value;
			    }
			}
		}

		/// <summary>
		/// This method creates a SQL statement to get an export result detail
		/// record from the database based on the Record ID value and the most 
		/// current date.  It populates the SQL command parameter with the 
		/// appropriate SQL and parameters.
		/// </summary>
		/// <param name="sqlCommand">
		/// The SQL command.
		/// </param>
		public void GetByRecordIdAndMostCurrent(SqlCommand sqlCommand)
		{
			sqlCommand.Parameters.Clear( );
			sqlCommand.CommandText = "SELECT TOP(1) * FROM tblExportResultDetails WITH (NOLOCK) " +
			                         "WHERE RecordId = @RecordId ORDER BY UpdatedDate DESC ";

			sqlCommand.Parameters.Add("@RecordId", SqlDbType.NVarChar, 64);
			sqlCommand.Parameters["@RecordId"].Value = this.RecordId;
		}

	    /// <summary>
	    /// This method creates a SQL command that will retrieve an error transactions and
	    /// error text.
	    /// </summary>
	    /// <param name="sqlCommand">
	    ///     The SQL command.
	    /// </param>
	    /// <param name="interfaceName"></param>
	    /// <param name="startDate">
	    ///     The start date.
	    /// </param>
	    /// <param name="endDate">
	    ///     The end date.
	    /// </param>
	    /// <param name="siteList">
	    ///     The site list.
	    /// </param>
	    /// <param name="orderBy">
	    ///     The order by.
	    /// </param>
	    /// <param name="userGuid">
	    ///     The user GUID.
	    /// </param>
	    public void GetErrorTransactions(
										SqlCommand sqlCommand,
										string interfaceName,
										DateTime? startDate,
										DateTime? endDate,
										List<Guid> siteList,
										string orderBy,
										Guid userGuid)
		{
			sqlCommand.Parameters.Clear( );

            // Only select the transaction alias that the user has the right to view or modify.
	        string sql = @"SELECT DISTINCT erd.UpdatedDate, t.TransDateTime, t.AliasName, erd.Error, t.TransID, t.Site, tli.Product, t.DocumentNumber,
                        dbo.udf_ConvertFromSIUnits( tli.NetQuantity, COALESCE( p.VolumeUnitIndex, aa.VolumeUnitIndex, ts.VolumeUnitIndex ), COALESCE( p.VolumeDecimalPlaces, aa.VolumeDecimalPlaces, ts.VolumeDecimalPlaces )  ) as NetQuantity 
                        FROM tblExportResultDetails erd ";

            // if we have a specific interface we need to apply the filter
	        if (!string.IsNullOrEmpty(interfaceName))
	        {
	            sql += @"JOIN tblExportResults er
                        ON erd.ExportResultGuid = er.ExportResultGuid
                        AND er.InterfaceName = @Interface ";
	        }
 
            sql += @"LEFT OUTER JOIN tblTransactions t WITH (NOLOCK) 
                        ON erd.RecordId = t.TransID 
                        AND erd.TransVersion = t.TransVersion 
                        AND erd.Fail = CAST(1 AS BIT) 
                        LEFT JOIN tblTransactionLineItems tli 
                        ON t.TransactionGuid = tli.TransactionGuid 
                        LEFT JOIN tblProducts p
                        ON p.ProductGuid = tli.ProductGuid
                        LEFT JOIN tblsites ts
                        ON ts.SiteGuid = t.SiteGuid
                        INNER JOIN  ( 
		                        SELECT DISTINCT a.AliasName, g.SiteGuid AS AliasSiteGuid, a.VolumeUnitIndex, a.VolumeDecimalPlaces 
		                        FROM tblUsers u
		                        INNER JOIN map.tblUserToGroup ug
		                        ON ug.UserGuid = u.UserGuid 
		                        INNER JOIN dbo.tblGroups g
		                        ON ug.GroupGuid = g.GroupGuid 
		                        INNER JOIN map.tblGroupToTransactionAlias gta
		                        ON gta.GroupGuid = ug.GroupGuid 
		                        INNER JOIN tblTransactionAliases  a
		                        ON a.TransactionAliasGuid = gta.TransactionAliasGuid  
		                        INNER JOIN lookup.tblRight r
		                        ON gta.LookupRightIndex = r.RightIndex 
		                        WHERE u.UserGuid = @UserGuid 
		                        AND (r.RightIndex = @ModifyRight 
		                        OR r.RightIndex = @ViewRight)) AS AA
                        ON AA.AliasName = t.AliasName 
                        WHERE t.ErrorFlag = CAST(1 AS BIT)
                        AND t.DeleteFlag = CAST(0 AS BIT) ";

			var parm = new SqlParameter("@UserGuid", SqlDbType.UniqueIdentifier) { Value = userGuid };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@ModifyRight", SqlDbType.Int) { Value = (int) GroupTransactionAliasMapClass.RIGHT.MODIFY };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@ViewRight", SqlDbType.Int) { Value = (int) GroupTransactionAliasMapClass.RIGHT.VIEW };
			sqlCommand.Parameters.Add(parm);

	        if (!string.IsNullOrEmpty(interfaceName))
	        {
	            parm = new SqlParameter("@Interface", SqlDbType.NVarChar, 150) { Value = interfaceName };
	            sqlCommand.Parameters.Add(parm);
	        }

	        if ( (startDate != null) && (endDate != null) )
			{
				sql = sql + "AND t.TransDateTime >= @StartDate AND t.TransDateTime <= @EndDate ";
			}

			if (siteList[0] != Guid.Empty)
			{
				sql = sql + "AND t.SiteGuid IN (";

				for ( int siteCount = 0; siteCount < siteList.Count; siteCount++ )
				{
					sql = sql + "@SiteGuid" + siteCount.ToString(CultureInfo.InvariantCulture) + ", ";
				}

				int lastComma = sql.LastIndexOf(",", StringComparison.Ordinal);
				sql = sql.Remove(lastComma);
				sql = sql + ") ";
			}



			// Add the order by expression if present.
			if ( string.IsNullOrEmpty(orderBy) == false )
			{
				sql = sql + " ORDER BY " + orderBy;
			}

			sqlCommand.CommandText = sql;

			if ( (startDate != null) && (endDate != null) )
			{
				// Ensure that the start date is at the very beginning of the day and
				// the end date is at the very end of the day, so if the start and end
				// dates are the same, it will pickup all transaction for that day.
				var dateStartDate = new DateTime(startDate.Value.Year, startDate.Value.Month, startDate.Value.Day, 0, 0, 0);
				var dateEndDate = new DateTime(endDate.Value.Year, endDate.Value.Month, endDate.Value.Day, 23, 59, 59);

				parm = new SqlParameter("@StartDate", SqlDbType.DateTime) { Value = dateStartDate };
				sqlCommand.Parameters.Add(parm);

				parm = new SqlParameter("@EndDate", SqlDbType.DateTime) { Value = dateEndDate };
				sqlCommand.Parameters.Add(parm);
			}

			if (siteList[0] != Guid.Empty)
			{
				int siteCount = 0;

				foreach ( Guid siteGuid in siteList )
				{
					string parmName = "@SiteGuid" + siteCount.ToString(CultureInfo.InvariantCulture);
					parm = new SqlParameter(parmName, SqlDbType.UniqueIdentifier) { Value = siteGuid };
					sqlCommand.Parameters.Add(parm);

					siteCount++;
				}
			}
		}

		public void GetUnacknowledgedTransactions(	SqlCommand sqlCommand,
													DateTime? startDate,
													DateTime? endDate,
													List<Guid> siteList,
													string orderBy,
													Guid userGuid)
		{
			sqlCommand.Parameters.Clear();

			const string Select = "SELECT t.TransDateTime, t.AliasName, t.TransID, t.Site, tli.Product ";
			const string From = "FROM tblEnterpriseQueue q "
			                     + "INNER JOIN tblTransactions t on q.sourceid = t.TransID AND q.SourceType = 1 AND q.[Status] = 2 "
			                     + "INNER JOIN tblTransactionLineItems tli ON t.TransactionGuid = tli.TransactionGuid "
			                     + "INNER JOIN map.tblGroupToTransactionAlias gtam on gtam.TransactionAliasGuid = t.TransactionAliasGuid "
			                     + " AND (gtam.LookupRightIndex = @ModifyRight or gtam.LookupRightIndex = @ViewRight) "
			                     + "INNER JOIN map.tblUserToGroup ugm ON gtam.GroupGuid = ugm.GroupGuid and ugm.UserGuid = @UserGuid and ugm.SiteGuid = t.SiteGuid ";
			string where = "WHERE t.DeleteFlag = CAST(0 AS BIT) ";

			if ((startDate != null) && (endDate != null))
			{
				where = where + "AND t.TransDateTime >= @StartDate AND t.TransDateTime <= @EndDate ";
			}

			if ((siteList != null) && (siteList[0] != Guid.Empty))
			{
				where = where + "AND t.SiteGuid IN (";

				for (int siteCount = 0; siteCount < siteList.Count; siteCount++)
				{
					where = where + "@SiteGuid" + siteCount + ", ";
				}

				int lastComma = where.LastIndexOf(",", StringComparison.Ordinal);
				where = where.Remove(lastComma);
				where = where + ") ";
			}

			string sql = Select + From + where;

			// Add the order by expression if present.
			if (string.IsNullOrEmpty(orderBy) == false)
			{
				sql = sql + " ORDER BY " + orderBy;
			}

			sqlCommand.CommandText = sql;

			var parm = new SqlParameter()
			{
				SqlDbType = SqlDbType.UniqueIdentifier,
				ParameterName = "@UserGuid",
				Value = userGuid
			};
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter()
			{
				SqlDbType = SqlDbType.Int,
				ParameterName = "@ModifyRight",
				Value = (int) GroupTransactionAliasMapClass.RIGHT.MODIFY
			};
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter()
			{
				SqlDbType = SqlDbType.Int,
				ParameterName = "@ViewRight",
				Value = (int) GroupTransactionAliasMapClass.RIGHT.VIEW
			};
			sqlCommand.Parameters.Add(parm);

			if ((startDate != null) && (endDate != null))
			{
				// Ensure that the start date is at the very beginning of the day and
				// the end date is at the very end of the day, so if the start and end
				// dates are the same, it will pickup all transaction for that day.
				var dateStartDate = new DateTime(startDate.Value.Year, startDate.Value.Month, startDate.Value.Day, 0, 0, 0);
				var dateEndDate = new DateTime(endDate.Value.Year, endDate.Value.Month, endDate.Value.Day, 23, 59, 59);

				parm = new SqlParameter("@StartDate", SqlDbType.DateTime)
				{
					Value = dateStartDate
				};
				sqlCommand.Parameters.Add(parm);

				parm = new SqlParameter("@EndDate", SqlDbType.DateTime)
				{
					Value = dateEndDate
				};
				sqlCommand.Parameters.Add(parm);
			}

			if ((siteList != null) && (siteList[0] != Guid.Empty))
			{
				for (int siteCount = 0; siteCount < siteList.Count; siteCount++)
				{
					string parmName = "@SiteGuid" + siteCount;
					parm = new SqlParameter(parmName, SqlDbType.UniqueIdentifier)
					{
						Value = siteList[siteCount]
					};
					sqlCommand.Parameters.Add(parm);
				}
			}
		}

		/// <summary>
		/// The enumerate SQL.
		/// </summary>
		/// <param name="sqlCommand">
		/// The SQL command.
		/// </param>
		/// <param name="siteGuid">
		/// The site GUID.
		/// </param>
		public void EnumerateSql(SqlCommand sqlCommand, Guid siteGuid)
		{
			sqlCommand.Parameters.Clear();
			sqlCommand.CommandText = 
							"SELECT TOP(@maxcount) * FROM " +
							"( " +
							"SELECT ERD.*, ROW_NUMBER() OVER (PARTITION BY ERD.RecordId ORDER BY ERD.RecordId, E.CreatedDate DESC) 'rownum' " +
							"FROM tblExportResultDetails ERD  " +
							"INNER JOIN tblTransactions T ON T.SiteGuid = @SiteGuid AND T.Flag06 <> CAST(1 AS BIT) AND ERD.RecordId = T.TransID AND ERD.TransVersion = T.TransVersion " +
							"INNER JOIN tblExportResults E ON ERD.ExportResultGuid = E.ExportResultGuid " +
							"AND E.InterfaceName in ( 'EBSTransactionResult') " +
							"UNION " +
							"SELECT ERD.*, ROW_NUMBER() OVER (PARTITION BY ERD.RecordId ORDER BY ERD.RecordId, E.CreatedDate DESC) 'rownum' " +
							"FROM tblExportResultDetails ERD  " +
							"INNER JOIN tblExportResults E ON ERD.ExportResultGuid = E.ExportResultGuid AND E.SiteGuid = @SiteGuid " +
							"LEFT JOIN tblTransactions T ON T.TransID = ERD.RecordId " +
							"WHERE T.TransID IS NULL " +
							") AS Results " +
							"WHERE rownum = 1";

			sqlCommand.Parameters.Add(new SqlParameter("@maxcount", MaxExport));
			sqlCommand.Parameters.Add(new SqlParameter("@SiteGuid", siteGuid));
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
		/// <exception cref="ArgumentNullException">Parameter must not be null.
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

			this.IdentityGuid		= DataObject.getValue(row["ExportResultDetailGuid"], Guid.Empty);
			this.exportResultGuid	= DataObject.getValue(row["ExportResultGuid"], Guid.Empty);
			this.recordId			= DataObject.getValue(row["RecordId"], string.Empty);
			this.fail				= DataObject.getValue(row["Fail"], false);
			this.transVersion		= DataObject.getValue<long?>(row["TransVersion"], null);
			this.error				= DataObject.getValue(row["Error"], string.Empty);
			this.interfaceData01	= DataObject.getValue(row["InterfaceData01"], string.Empty);
			this.interfaceData02	= DataObject.getValue(row["InterfaceData02"], string.Empty);
			this.interfaceData03	= DataObject.getValue(row["InterfaceData03"], string.Empty);
			this.interfaceData04	= DataObject.getValue(row["InterfaceData04"], string.Empty);
			this.interfaceData05	= DataObject.getValue(row["InterfaceData05"], string.Empty);
			this.interfaceData06	= DataObject.getValue(row["InterfaceData06"], string.Empty);
			this.interfaceData07	= DataObject.getValue(row["InterfaceData07"], string.Empty);
			this.interfaceData08	= DataObject.getValue(row["InterfaceData08"], string.Empty);
			this._CreatedDate		= DataObject.getValue(row["CreatedDate"], DateTimeOffset.Now);
			this._CreatedBy			= DataObject.getValue(row["CreatedBy"], BaseDataObject.ADMIN);
			this._UpdatedDate		= DataObject.getValue(row["UpdatedDate"], this._CreatedDate);
			this._UpdatedBy			= DataObject.getValue(row["UpdatedBy"], BaseDataObject.ADMIN);
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method initializes the object to its initial state.
		/// </summary>
		protected void Init()
		{
			base.Reset( );
			this.exportResultGuid	= Guid.Empty;
			this.recordId			= string.Empty;
			this.transVersion		= null;
			this.error				= string.Empty;
			this.interfaceData01	= string.Empty;
			this.interfaceData02	= string.Empty;
			this.interfaceData03	= string.Empty;
			this.interfaceData04	= string.Empty;
			this.interfaceData05	= string.Empty;
			this.interfaceData06	= string.Empty;
			this.interfaceData07	= string.Empty;
			this.interfaceData08	= string.Empty;
			this.interfaceNameList	= new List<string>();
		}
		#endregion
	}
}

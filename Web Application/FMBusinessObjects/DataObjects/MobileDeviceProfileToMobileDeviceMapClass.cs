// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MobileDeviceProfileToMobileDeviceMapClass.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the MobileDeviceProfileToMobileDeviceMapClass type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Runtime.Serialization;

	/// <summary>
	/// The purpose of the Mobile Device Profile to Mobile Device data object is to contain data and SQL to add,
	/// modify, and delete profile information from the database.
	/// </summary>
	[Serializable]
	[DataContract]
	public class MobileDeviceProfileToMobileDeviceMapClass : DataObject
	{
		#region Private data members
		[DataMember] private Guid mobileDeviceProfileGuid;
		[DataMember] private Guid mobileDeviceProfileToMobileDeviceGuid;
		[DataMember] private Guid assignedToMobileDeviceGuid;
		[DataMember] private DateTimeOffset? createdDate;
		[DataMember] private DateTimeOffset? updatedDate;
		[DataMember] private string createdBy;
		[DataMember] private string updatedBy;
		[DataMember] private string mobileDeviceId;
		[DataMember] private string mobileDeviceProfileId;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="MobileDeviceProfileToMobileDeviceMapClass"/> class.
		/// </summary>
		public MobileDeviceProfileToMobileDeviceMapClass()
		{
			this.Reset();
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the mobile device profile guid.
		/// </summary>
		public Guid MobileDeviceProfileGuid
		{
			get { return this.mobileDeviceProfileGuid; }
			set { this.mobileDeviceProfileGuid = value; }
		}

		/// <summary>
		/// Gets or sets the mobile device profile to Mobile device guid.
		/// </summary>
		public Guid MobileDeviceProfileToMobileDeviceGuid
		{
			get { return this.mobileDeviceProfileToMobileDeviceGuid; }
			set { this.mobileDeviceProfileToMobileDeviceGuid = value; }
		}

		/// <summary>
		/// Gets or sets the assigned to Mobile device guid.
		/// </summary>
		public Guid AssignedToMobileDeviceGuid
		{
			get { return this.assignedToMobileDeviceGuid; }
			set { this.assignedToMobileDeviceGuid = value; }
		}

		/// <summary>
		/// Gets or sets the created date.
		/// </summary>
		public DateTimeOffset? CreatedDate
		{
			get { return this.createdDate; }
			set { this.createdDate = value; }
		}

		/// <summary>
		/// Gets or sets the updated date.
		/// </summary>
		public DateTimeOffset? UpdatedDate
		{
			get { return this.updatedDate; }
			set { this.updatedDate = value; }
		}

		/// <summary>
		/// Gets or sets the createdby.
		/// </summary>
		public string CreatedBy
		{
			get { return this.createdBy; }
			set { this.createdBy = value; }
		}

		/// <summary>
		/// Gets or sets the updatedby.
		/// </summary>
		public string UpdatedBy
		{
			get { return this.updatedBy; }
			set { this.updatedBy = value; }
		}

		/// <summary>
		/// Gets or sets the Mobile device id.
		/// </summary>
		public string MobileDeviceId
		{
			get { return this.mobileDeviceId; }
			set { this.mobileDeviceId = value; }
		}

		/// <summary>
		/// Gets or sets the mobile device profile id.
		/// </summary>
		public string MobileDeviceProfileId
		{
			get { return this.mobileDeviceProfileId; }
			set { this.mobileDeviceProfileId = value; }
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// This method will load a single row from a dataset that only has
		/// one row.
		/// </summary>
		/// <param name="dataSet">
		/// The data set.
		/// </param>
		public void LoadSingle(DataSet dataSet)
		{
			if ( dataSet != null && dataSet.Tables.Count > 0 )
			{
				DataTable table = dataSet.Tables[0];

				if ( (table != null) && (table.Rows != null) && (table.Rows.Count > 0) )
				{
					DataRow row = table.Rows[0];
					this.LoadRow(row);
				}
			}
		}

		/// <summary>
		/// This method will load a row of data into the object.
		/// </summary>
		/// <param name="row">
		/// The row.
		/// </param>
		public void Load(DataRow row)
		{
			this.LoadRow(row);
		}
		#endregion

		#region SQL Methods
		/// <summary>
		/// This method will populate the SQL Command with the Insert SQL for inserting
		/// into the tblMobileDeviceProfileToMobileDevice map table.
		/// </summary>
		/// <param name="sqlCommand">
		/// The sql command.
		/// </param>
		public void InsertSql(SqlCommand sqlCommand)
		{
			string insert = "INSERT INTO map.tblMobileDeviceProfileToMobileDevice (" 
							+ "MobileDeviceProfileGuid, "
			                + "MobileDeviceProfileToMobileDeviceGuid, " 
							+ "AssignedToMobileDeviceGuid, " 
							+ "CreatedDate, "
			                + "CreatedBy, " 
							+ "UpdatedDate, " 
							+ "UpdatedBy) ";

			string insertValues = "VALUES ("
								+ "@MobileDeviceProfileGuid, "
								+ "@MobileDeviceProfileToMobileDeviceGuid, "
								+ "@AssignedToMobileDeviceGuid, "
								+ "@CreatedDate, " 
								+ "@CreatedBy, " 
								+ "@UpdatedDate, "
								+ "@UpdatedBy"
								+ ")";

			sqlCommand.CommandText = insert + insertValues;

			var parm = new SqlParameter("@MobileDeviceProfileGuid", SqlDbType.UniqueIdentifier) { Value = this.mobileDeviceProfileGuid };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@MobileDeviceProfileToMobileDeviceGuid", SqlDbType.UniqueIdentifier) { Value = this.mobileDeviceProfileToMobileDeviceGuid };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@AssignedToMobileDeviceGuid", SqlDbType.UniqueIdentifier) { Value = this.assignedToMobileDeviceGuid };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@CreatedBy", SqlDbType.NVarChar, 100) { Value = this.createdBy };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@UpdatedBy", SqlDbType.NVarChar, 100) { Value = this.updatedBy };
			sqlCommand.Parameters.Add(parm);

			if ( this.createdDate != null )
			{
				parm = new SqlParameter("@CreatedDate", SqlDbType.DateTimeOffset) { Value = this.createdDate };
				sqlCommand.Parameters.Add(parm);
			}

			if ( this.updatedDate != null )
			{
				parm = new SqlParameter("@UpdatedDate", SqlDbType.DateTimeOffset) { Value = this.updatedDate };
				sqlCommand.Parameters.Add(parm);
			}
		}

		/// <summary>
		/// This method will populate the SQL Command with the Purge SQL for removing
		/// a map from the tblMobileDeviceProfileToMobileDevice map table.
		/// </summary>
		/// <param name="sqlCommand">
		/// The sql command.
		/// </param>
		public void PurgeSql(SqlCommand sqlCommand)
		{
			string delete = "DELETE FROM map.tblMobileDeviceProfileToMobileDevice ";
			string where = "WHERE MobileDeviceProfileGuid = @MobileDeviceProfileGuid AND assignedToMobileDeviceGuid = @assignedToMobileDeviceGuid";

			sqlCommand.CommandText = delete + where;

			var parm = new SqlParameter("@MobileDeviceProfileGuid", SqlDbType.UniqueIdentifier) { Value = this.mobileDeviceProfileGuid };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@assignedToMobileDeviceGuid", SqlDbType.UniqueIdentifier) { Value = this.assignedToMobileDeviceGuid };
			sqlCommand.Parameters.Add(parm);
		}

		/// <summary>
		/// This method will populate the SQL Command with the Purge All By Profile GUID SQL for removing
		/// all maps from the tblMobileDeviceProfileToMobileDevice map table.
		/// </summary>
		/// <param name="sqlCommand">
		/// The sql command.
		/// </param>
		public void PurgeAllByProfileGuidSql(SqlCommand sqlCommand)
		{
			string delete = "DELETE FROM map.tblMobileDeviceProfileToMobileDevice ";
			string where  = "WHERE MobileDeviceProfileGuid = @MobileDeviceProfileGuid ";

			sqlCommand.CommandText = delete + where;

			var parm = new SqlParameter("@MobileDeviceProfileGuid", SqlDbType.UniqueIdentifier) { Value = this.MobileDeviceProfileGuid };
			sqlCommand.Parameters.Add(parm);
		}

		/// <summary>
		/// This method will populate the SQL Command with the Purge All By Mobile Device GUID SQL for removing
		/// all maps from the tblMobileDeviceProfileToMobileDevice map table.
		/// </summary>
		/// <param name="sqlCommand">
		/// The sql command.
		/// </param>
		public void PurgeAllByMobileDeviceGuidSql(SqlCommand sqlCommand)
		{
			string delete = "DELETE FROM map.tblMobileDeviceProfileToMobileDevice ";
			string where  = "WHERE AssignedToMobileDeviceGuid = @AssignedToMobileDeviceGuid ";

			sqlCommand.CommandText = delete + where;

			var parm = new SqlParameter("@AssignedToMobileDeviceGuid", SqlDbType.UniqueIdentifier) { Value = this.AssignedToMobileDeviceGuid };
			sqlCommand.Parameters.Add(parm);
		}

		/// <summary>
		/// This method will populate the SQL Command with the Select SQL for retrieving
		/// a map from the tblMobileDeviceProfileToMobileDevice map table.
		/// </summary>
		/// <param name="sqlCommand">
		/// The sql command.
		/// </param>
		/// <param name="inTransaction">
		/// The in transaction.
		/// </param>
		public void SelectSql(SqlCommand sqlCommand, bool inTransaction)
		{
            // 9/8/2016 - TLH - removing all UPLOCK hints, allowing SQL Server to determine best lock.
			string select = "SELECT * FROM map.tblMobileDeviceProfileToMobileDevice ";
			string where = " WHERE MobileDeviceProfileGuid = @MobileDeviceProfileGuid AND assignedToMobileDeviceGuid = @assignedToMobileDeviceGuid";

			sqlCommand.CommandText = select + where;

			var parm = new SqlParameter("@MobileDeviceProfileGuid", SqlDbType.UniqueIdentifier) { Value = this.mobileDeviceProfileGuid };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@assignedToMobileDeviceGuid", SqlDbType.UniqueIdentifier) { Value = this.assignedToMobileDeviceGuid };
			sqlCommand.Parameters.Add(parm);
		}

		/// <summary>
		/// This method will populate the SQL command with a select statement to 
		/// retrieve all the Mobile devices assigned to a profile.
		/// </summary>
		/// <param name="sqlCommand">
		/// The sql command.
		/// </param>
		/// <param name="inTransaction">
		/// The in transaction.
		/// </param>
		public void EnumerateMobileDeviceByProfileGuidSql(SqlCommand sqlCommand, bool inTransaction)
		{
            // 9/8/2016 - TLH - removing all UPLOCK hints, allowing SQL Server to determine best lock.
			string select	= "SELECT pmap.*, md.MobileDeviceID ";
			string from		= "FROM map.tblMobileDeviceProfileToMobileDevice AS pmap "
							  + "LEFT OUTER JOIN tblMobileDevice AS md ON pmap.assignedToMobileDeviceGuid = md.MobileDeviceGuid ";
			string where	= "WHERE pmap.MobileDeviceProfileGuid = @MobileDeviceProfileGuid ";

			sqlCommand.CommandText = select + from + where;

			var parm = new SqlParameter("@MobileDeviceProfileGuid", SqlDbType.UniqueIdentifier) { Value = this.mobileDeviceProfileGuid };
			sqlCommand.Parameters.Add(parm);
		}

		/// <summary>
		/// This method will populate the SQL command with a select statement to 
		/// retrieve all the profiles assigned to a Mobile device.
		/// </summary>
		/// <param name="sqlCommand">
		/// The sql command.
		/// </param>
		/// <param name="inTransaction">
		/// The in transaction.
		/// </param>
		public void EnumerateMobileDeviceByMobileDeviceGuidSql(SqlCommand sqlCommand, bool inTransaction)
		{
            // 9/8/2016 - TLH - removing all UPLOCK hints, allowing SQL Server to determine best lock.
			string select  = "SELECT pmap.*, mdp.ProfileID AS MobileDeviceProfileID ";
			string from    = "FROM map.tblMobileDeviceProfileToMobileDevice AS pmap "
							  + "LEFT OUTER JOIN tblMobileDeviceProfile AS mdp ON pmap.MobileDeviceProfileGuid = mdp.MobileDeviceProfileGuid ";
			string where   = "WHERE pmap.AssignedToMobileDeviceGuid = @AssignedToMobileDeviceGuid ";

			sqlCommand.CommandText = select + from + where;

			var parm = new SqlParameter("@AssignedToMobileDeviceGuid", SqlDbType.UniqueIdentifier) { Value = this.assignedToMobileDeviceGuid };
			sqlCommand.Parameters.Add(parm);
		}

		/// <summary>
		/// This method will populate the SQL command with a select statement to 
		/// retrieve all the profiles that are not assigned to a Mobile device.
		/// </summary>
		/// <param name="sqlCommand">
		/// The sql command.
		/// </param>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="siteGuid">
		/// The site Guid.
		/// </param>
		/// <param name="inTransaction">
		/// The in transaction.
		/// </param>
		public void EnumerateUnassignedProfilesSql(SqlCommand sqlCommand, SecurityClass security, Guid siteGuid, bool inTransaction)
		{
			if ( siteGuid == Guid.Empty )
			{
				sqlCommand.CommandText = null;
				return;
			}

            // 9/8/2016 - TLH - removing all UPLOCK hints, allowing SQL Server to determine best lock.
			string select = "SELECT NULL AS MobileDeviceProfileToMobileDeviceGuid, " 
								+ "MobileDeviceProfileGuid, "
								+ "@AssignedToMobileDeviceGuid AS AssignedToMobileDeviceGuid, " 
								+ "CreatedDate, "
								+ "CreatedBy, " 
								+ "UpdatedDate, " 
								+ "UpdatedBy, "
								+ "ProfileID AS MobileDeviceProfileID ";
			string from  = "FROM dbo.tblMobileDeviceProfile ";
			string where = "WHERE MobileDeviceProfileGuid NOT IN (SELECT MobileDeviceProfileGuid from map.tblMobileDeviceProfileToMobileDevice "
							+ "WHERE AssignedToMobileDeviceGuid = @AssignedToMobileDeviceGuid) "
							+ "AND (SiteGuid = @SiteGuid "
							+ "OR " + this.AppendSiteWhereClause(sqlCommand, security, "dbo.tblMobileDeviceProfile", "MobileDeviceProfileGuid", ENTITY_TYPE.MOBILE_DEVICE_PROFILE)
							+ ") ";

			sqlCommand.CommandText = select + from + where;

			var parm = new SqlParameter("@AssignedToMobileDeviceGuid", SqlDbType.UniqueIdentifier) { Value = this.assignedToMobileDeviceGuid };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@SiteGuid", SqlDbType.UniqueIdentifier) { Value = siteGuid };
			sqlCommand.Parameters.Add(parm);
		}

		/// <summary>
		/// This method will populate the SQL command with a select statement to 
		/// retrieve all the mobile devices that are not assigned to a profile.
		/// </summary>
		/// <param name="sqlCommand">
		/// The sql command.
		/// </param>
		/// <param name="siteGuid">
		/// The site Guid.
		/// </param>
		/// <param name="inTransaction">
		/// The in transaction.
		/// </param>
		public void EnumerateUnassignedMobileDevicesSql(SqlCommand sqlCommand, Guid siteGuid, bool inTransaction)
		{
			if ( siteGuid == Guid.Empty )
			{
				sqlCommand.CommandText = null;
				return;
			}

            // 9/8/2016 - TLH - removing all UPLOCK hints, allowing SQL Server to determine best lock.
			string select = "SELECT NULL AS MobileDeviceProfileToMobileDeviceGuid, "
								+ "@MobileDeviceProfileGuid AS MobileDeviceProfileGuid, "
								+ "MobileDeviceGuid AS AssignedToMobileDeviceGuid, "
								+ "CreatedDate, "
								+ "CreatedBy, "
								+ "UpdatedDate, "
								+ "UpdatedBy, "
								+ "MobileDeviceId ";
			string from = "FROM dbo.tblMobileDevice ";
			string where = "WHERE MobileDeviceGuid NOT IN (SELECT AssignedToMobileDeviceGuid from map.tblMobileDeviceProfileToMobileDevice "
							+ "WHERE MobileDeviceProfileGuid = @MobileDeviceProfileGuid) AND SiteGuid = @SiteGuid ";


			sqlCommand.CommandText = select + from + where;

			var parm = new SqlParameter("@MobileDeviceProfileGuid", SqlDbType.UniqueIdentifier) { Value = this.MobileDeviceProfileGuid };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@SiteGuid", SqlDbType.UniqueIdentifier) { Value = siteGuid };
			sqlCommand.Parameters.Add(parm);
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method will load the information from the data row into the object's
		/// properties.
		/// </summary>
		/// <param name="row">
		/// The row.
		/// </param>
		private void LoadRow(DataRow row)
		{
			if ( row != null )
			{
				this.mobileDeviceProfileToMobileDeviceGuid	= row.IsNull("MobileDeviceProfileToMobileDeviceGuid") ? Guid.Empty : (Guid) row["MobileDeviceProfileToMobileDeviceGuid"];
				this.mobileDeviceProfileGuid				= row.IsNull("MobileDeviceProfileGuid") ? Guid.Empty : (Guid) row["MobileDeviceProfileGuid"];
				this.assignedToMobileDeviceGuid				= row.IsNull("AssignedToMobileDeviceGuid") ? Guid.Empty : (Guid) row["AssignedToMobileDeviceGuid"];
				this.createdBy								= row.IsNull("CreatedBy") ? string.Empty : (string) row["CreatedBy"];
				this.updatedBy								= row.IsNull("UpdatedBy") ? string.Empty : (string) row["UpdatedBy"];

				// Depending on the query, we may not have the MobileDeviceID column present.
				// Therefore, we want to continue.
				try
				{
					this.mobileDeviceId = row.IsNull("MobileDeviceID") ? string.Empty : (string) row["MobileDeviceID"];
				}
				catch (Exception)
				{
					// Ignore
				}

				// Depending on the query, we may not have the MobileDeviceProfileID column present.
				// Therefore, we want to continue.
				try
				{
					this.MobileDeviceProfileId = row.IsNull("MobileDeviceProfileID") ? string.Empty : (string) row["MobileDeviceProfileID"];
				}
				catch ( Exception )
				{
					// Ignore
				}


				if ( row.IsNull("CreatedDate") == false )
				{
					this.createdDate = (DateTimeOffset) row["CreatedDate"];
				}

				if ( row.IsNull("UpdatedDate") == false )
				{
					this.updatedDate = (DateTimeOffset) row["UpdatedDate"];
				}
			}
		}

		/// <summary>
		/// This method sets the object to its initial state;
		/// </summary>
		public void Reset()
		{
			this.mobileDeviceProfileGuid			= Guid.Empty;
			this.mobileDeviceProfileToMobileDeviceGuid = Guid.Empty;
			this.assignedToMobileDeviceGuid			= Guid.Empty;
			this.createdDate						= null;
			this.createdBy							= string.Empty;
			this.updatedDate						= null;
			this.updatedBy							= string.Empty;
			this.mobileDeviceId						= string.Empty;
			this.mobileDeviceProfileId				= string.Empty;
		}
		#endregion

		#region Overrides

		/// <summary>
		/// The get update command.
		/// </summary>
		/// <returns>
		/// The System.String.
		/// </returns>
		override public string getUpdateCommand( )
		{
			return null;
		}

		/// <summary>
		/// The get delete command.
		/// </summary>
		/// <returns>
		/// The System.String.
		/// </returns>
		override public string getDeleteCommand( )
		{
			return null;
		}

		/// <summary>
		/// The get insert command.
		/// </summary>
		/// <returns>
		/// The System.String.
		/// </returns>
		override public string getInsertCommand( )
		{
			return null;
		}

		/// <summary>
		/// The get select command.
		/// </summary>
		/// <returns>
		/// The System.String.
		/// </returns>
		override public string getSelectCommand( )
		{
			return null;
		}
		#endregion
	}

	#region Mobile Device Profile To Mobile Device Map class collection
	/// <summary>
	/// This class contains a collection of mobile device profile to Mobile Device mapping
	/// objects.
	/// </summary>
	[Serializable]
	[CollectionDataContract]
	public class MobileDeviceProfileToMobileDeviceMapCollection : List<MobileDeviceProfileToMobileDeviceMapClass>
	{
		#region Public methods
		/// <summary>
		/// This method will load the collection using a dataset. It will separate
		/// each row and create a new datarow with the row. This is for each
		/// analog input object.
		/// </summary>
		/// <param name="dataSet">
		/// The data set.
		/// </param>
		public void Load(DataSet dataSet)
		{
			if ( (dataSet != null) && (dataSet.Tables.Count > 0) )
			{
				var table = dataSet.Tables[0];

				foreach ( DataRow row in table.Rows )
				{
					var singleRowDataSet = dataSet.Clone( );
					var newTable = singleRowDataSet.Tables[0];
					var newRow = newTable.NewRow( );

					newRow.ItemArray = row.ItemArray;
					newTable.Rows.Add(newRow);

					var profileToMobileDeviceMap = new MobileDeviceProfileToMobileDeviceMapClass( );
					profileToMobileDeviceMap.LoadSingle(singleRowDataSet);
					this.Add(profileToMobileDeviceMap);
				}
			}
		}
		#endregion
	}
	#endregion
}

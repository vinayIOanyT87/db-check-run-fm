// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MobileDeviceClass.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the MobileDeviceClass type.
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
	/// The purpose of the Mobile device data object is to contain data and SQL to add,
	/// modify, and delete Mobile device information from the database.
	/// </summary>
	[Serializable]
	[DataContract]
	public class MobileDeviceClass : DataObject
	{
		#region Public data members
		/// <summary>
		/// The Mobile device types.
		/// </summary>
		public enum MobileDeviceTypes { None, Handheld };
		#endregion

		#region Private data members
		[DataMember] private Guid mobileDeviceGuid;
		[DataMember] private string mobileDeviceId;
		[DataMember] private string description;
		[DataMember] private int? mobileDeviceType;
		[DataMember] private Guid siteGuid;
		[DataMember] private DateTimeOffset? createdDate;
		[DataMember] private DateTimeOffset? updatedDate;
		[DataMember] private string createdBy;
		[DataMember] private string updatedBy;
		[DataMember] private MobileDeviceProfileToMobileDeviceMapCollection assignedProfileCollection;
		[DataMember] private MobileDeviceProfileToMobileDeviceMapCollection unassignedProfileCollection;
		[DataMember] private MobileDeviceProfileToMobileDeviceMapCollection removedAssignedCollection;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="MobileDeviceClass"/> class. 
		/// This is the default constructor for the Mobile Device class.
		/// </summary>
		public MobileDeviceClass( )
		{
			this.Reset ( );
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the mobile device guid.
		/// </summary>
		public Guid MobileDeviceGuid
		{
			get { return this.mobileDeviceGuid; }
			set { this.mobileDeviceGuid = value; }
		}

		/// <summary>
		/// Gets or sets the mobile device id.
		/// </summary>
		public string MobileDeviceId
		{
			get { return this.mobileDeviceId; }
			set { this.mobileDeviceId = value; }
		}

		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		public string Description
		{
			get { return this.description; }
			set { this.description = value; }
		}

		/// <summary>
		/// Gets or sets the mobile device type.
		/// </summary>
		public int? MobileDeviceType
		{
			get { return this.mobileDeviceType; }
			set { this.mobileDeviceType = value; }
		}

		/// <summary>
		/// Gets or sets the site guid.
		/// </summary>
		public Guid SiteGuid
		{
			get { return this.siteGuid; }
			set { this.siteGuid = value; }
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
		/// Gets or sets the created by.
		/// </summary>
		public string CreatedBy
		{
			get { return this.createdBy; }
			set { this.createdBy = value; }
		}

		/// <summary>
		/// Gets or sets the updated by.
		/// </summary>
		public string UpdatedBy
		{
			get { return this.updatedBy; }
			set { this.updatedBy = value; }
		}

		/// <summary>
		/// Gets or sets the assigned profile collection.
		/// </summary>
		public MobileDeviceProfileToMobileDeviceMapCollection AssignedProfileCollection
		{
			get { return this.assignedProfileCollection; }
			set { this.assignedProfileCollection = value; }
		}

		/// <summary>
		/// Gets or sets the unassigned profile collection.
		/// </summary>
		public MobileDeviceProfileToMobileDeviceMapCollection UnassignedProfileCollection
		{
			get { return this.unassignedProfileCollection; }
			set { this.unassignedProfileCollection = value; }
		}

		/// <summary>
		/// Gets or sets the removed assigned collection.
		/// </summary>
		public MobileDeviceProfileToMobileDeviceMapCollection RemovedAssignedCollection
		{
			get { return this.removedAssignedCollection; }
			set { this.removedAssignedCollection = value; }
		}
		#endregion

		#region Public methods
		/// <summary>
		/// This method loads the retrieved data from the database and loads the data into the
		/// object.
		/// </summary>
		/// <param name="dataSet">
		/// The data set.
		/// </param>
		public void Load(DataSet dataSet)
		{
			if ( (dataSet != null) && (dataSet.Tables.Count > 0) )
			{
				DataTable table = dataSet.Tables[0];

				if ( (table != null) && (table.Rows != null) && (table.Rows.Count > 0) )
				{
					DataRow row = table.Rows[0];
					this.LoadRow(row);

					// Serialize the dataset to be used later for comparison.
					this.SerializeData(dataSet);
				}
			}
		}

		/// <summary>
		/// This method will load a single record only.
		/// </summary>
		/// <param name="dataSet">
		/// The data set.
		/// </param>
		public void LoadSingle(DataSet dataSet)
		{
			if ( (dataSet != null) && (dataSet.Tables.Count > 0) )
			{
				DataTable table = dataSet.Tables[0];

				if ( (table != null) && (table.Rows != null) && (table.Rows.Count > 0) )
				{
					DataRow row = table.Rows[0];
					this.LoadRow(row);

					// Serialize the dataset to be used later for comparison.
					this.SerializeData(dataSet);
				}
			}
		}

		/// <summary>
		/// This method will load the dataset that contains whether the mobile device ID
		/// is unique within a site. It will return true if it is unique. Othewise, it
		/// returns false.
		/// </summary>
		/// <param name="dataSet">
		/// The data set.
		/// </param>
		/// <returns>
		/// The System.Boolean.
		/// </returns>
		public bool LoadIsMobileDeviceUnique(DataSet dataSet)
		{
			bool isUnique = true;

			if ( (dataSet != null) && (dataSet.Tables.Count > 0) )
			{
				DataTable table = dataSet.Tables[0];

				if ( (table != null) && (table.Rows != null) && (table.Rows.Count > 0) )
				{
					DataRow row = table.Rows[0];

					if ( row != null )
					{
						int mobileDeviceCount = row.IsNull("NumberOfMobileDevices") ? 0 : (int) row["NumberOfMobileDevices"];

						if ( mobileDeviceCount > 0 )
						{
							isUnique = false;
						}
					}
				}
			}

			return isUnique;
		}

		/// <summary>
		/// This method sets the object to its initial state.
		/// </summary>
		public void Reset( )
		{
			this.mobileDeviceGuid				= Guid.Empty;
			this.mobileDeviceId					= string.Empty;
			this.description					= string.Empty;
			this.mobileDeviceType				= null;
			this.siteGuid						= Guid.Empty;
			this.createdDate					= null;
			this.updatedDate					= null;
			this.createdBy						= string.Empty;
			this.updatedBy						= string.Empty;
			this.assignedProfileCollection		= new MobileDeviceProfileToMobileDeviceMapCollection();
			this.unassignedProfileCollection	= new MobileDeviceProfileToMobileDeviceMapCollection();
			this.removedAssignedCollection		= new MobileDeviceProfileToMobileDeviceMapCollection();
		}
		#endregion

		#region SQL Statements
		/// <summary>
		/// This method will populate the sql command with the SQL text
		/// to determine if there are more than one Mobile device for
		/// the given site.
		/// </summary>
		/// <param name="sqlCommand">
		/// The sql command.
		/// </param>
		/// <param name="security">
		/// The security.
		/// </param>
		public void IsMobileDeviceUniqueSql(SqlCommand sqlCommand, SecurityClass security)
		{
			string select = "SELECT COUNT(*) AS NumberOfMobileDevices ";
			string from   = "FROM tblMobileDevice ";
			string where  = "WHERE SiteGuid = @SiteGuid AND MobileDeviceId = @MobileDeviceId ";

			sqlCommand.CommandText = select + from + where;

			var parm = new SqlParameter("@SiteGuid", SqlDbType.UniqueIdentifier) { Value = security.SiteGuid };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@MobileDeviceId", SqlDbType.NVarChar, 50) { Value = this.mobileDeviceId };
			sqlCommand.Parameters.Add(parm);
		}

		/// <summary>
		/// This method will populate the sql command with the SQL text
		/// to retrieve all Mobile devices.
		/// </summary>
		/// <param name="sqlCommand">
		/// The sql command.
		/// </param>
		/// <param name="security">
		/// The security.
		/// </param>
		public void EnumerateAllSql(SqlCommand sqlCommand, SecurityClass security)
		{
			string select	= "SELECT * ";
			string from		= "FROM tblMobileDevice WITH ( NOLOCK ) ";
			string where	= "WHERE SiteGuid = @SiteGuid ";

			sqlCommand.CommandText = select + from + where;

			var parm = new SqlParameter("@SiteGuid", SqlDbType.UniqueIdentifier) { Value = security.SiteGuid };
			sqlCommand.Parameters.Add(parm);
		}

		/// <summary>
		/// This method will populate the sql command with the SQL text
		/// to retrieve all mobile devices based on the find filter.
		/// </summary>
		/// <param name="sqlCommand">
		/// The sql command.
		/// </param>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="findFilter">
		/// The find filter.
		/// </param>
		public void EnumerateByFindFilterSql(SqlCommand sqlCommand, SecurityClass security, string findFilter)
		{
			string select	= "SELECT * ";
			string from		= "FROM tblMobileDevice WITH ( NOLOCK ) ";

			if ( string.IsNullOrEmpty(findFilter) == false )
			{
				string where2 = "WHERE (MobileDeviceID LIKE (@FindFilter1) OR Description LIKE (@FindFilter2)) AND " +
								"SiteGuid = @SiteGuid ";

				sqlCommand.CommandText = select + from + where2;

				string idFindFilter = findFilter;
				string descFindFilter = findFilter;

				if ( findFilter.Length > 50 )
				{
					idFindFilter = findFilter.Substring(0, 50);
				}

				if ( findFilter.Length > 200 )
				{
					descFindFilter = findFilter.Substring(0, 200);
				}

				idFindFilter = "%" + idFindFilter + "%";
				descFindFilter = "%" + descFindFilter + "%";

				var parm = new SqlParameter("@FindFilter1", SqlDbType.NVarChar, 50) { Value = idFindFilter };
				sqlCommand.Parameters.Add(parm);

				parm = new SqlParameter("@FindFilter2", SqlDbType.NVarChar, 200) { Value = descFindFilter };
				sqlCommand.Parameters.Add(parm);

				parm = new SqlParameter("@SiteGuid", SqlDbType.UniqueIdentifier) { Value = security.SiteGuid };
				sqlCommand.Parameters.Add(parm);
			}
			else
			{
				string where1 = "WHERE SiteGuid = @SiteGuid ";
				sqlCommand.CommandText = select + from + where1;

				var parm = new SqlParameter("@SiteGuid", SqlDbType.UniqueIdentifier) { Value = security.SiteGuid };
				sqlCommand.Parameters.Add(parm);
			}
		}


		/// <summary>
		/// This method will populate the sql command to retrieve the GUID based
		/// on a Mobile Device ID.
		/// </summary>
		/// <param name="sqlCommand">
		/// The sql command.
		/// </param>
		/// <param name="security">
		/// The security.
		/// </param>
		public void GetGuidSql(SqlCommand sqlCommand, SecurityClass security)
		{
			string select	= "SELECT MobileDeviceGuid ";
			string from		= "FROM tblMobileDevice WITH ( NOLOCK ) ";
			string where	= "WHERE MobileDeviceID = @MobileDeviceID AND SiteGuid = @SiteGuid ";

			sqlCommand.CommandText = select + from + where;

			var parm = new SqlParameter("@MobileDeviceID", SqlDbType.NVarChar, 50) { Value = this.mobileDeviceId };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@SiteGuid", SqlDbType.UniqueIdentifier) { Value = security.SiteGuid };
			sqlCommand.Parameters.Add(parm);
		}

		/// <summary>
		/// This method will populate the sql command to retrieve Mobile device data
		/// based on the Mobile Device ID.
		/// </summary>
		/// <param name="sqlCommand">
		/// The sql command.
		/// </param>
		/// <param name="security">
		/// The security.
		/// </param>
		public void GetByMobileDeviceIdSql(SqlCommand sqlCommand, SecurityClass security)
		{
			string select	= "SELECT * ";
			string from		= "FROM tblMobileDevice WITH ( NOLOCK ) ";
			string where	= "WHERE MobileDeviceID = @MobileDeviceID AND SiteGuid = @SiteGuid ";

			sqlCommand.CommandText = select + from + where;

			var parm = new SqlParameter("@MobileDeviceID", SqlDbType.NVarChar, 50) { Value = this.mobileDeviceId };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@SiteGuid", SqlDbType.UniqueIdentifier) { Value = security.SiteGuid };
			sqlCommand.Parameters.Add(parm);
		}

		/// <summary>
		/// This method will populate the sql command to retrieve Mobile device data
		/// based on the GUID.
		/// </summary>
		/// <param name="sqlCommand">
		/// The sql command.
		/// </param>
		/// <param name="security">
		/// The security.
		/// </param>
		public void GetByMobileDeviceGuidSql(SqlCommand sqlCommand, SecurityClass security)
		{
			string select = "SELECT * ";
			string from   = "FROM tblMobileDevice WITH ( NOLOCK ) ";
			string where  = "WHERE MobileDeviceGuid = @MobileDeviceGuid AND SiteGuid = @SiteGuid ";

			sqlCommand.CommandText = select + from + where;

			var parm = new SqlParameter("@MobileDeviceGuid", SqlDbType.UniqueIdentifier) { Value = this.mobileDeviceGuid };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@SiteGuid", SqlDbType.UniqueIdentifier) { Value = security.SiteGuid };
			sqlCommand.Parameters.Add(parm);
		}

		/// <summary>
		/// This method will populate the sql command to remove a Mobile device record
		/// from the database based on the GUID.
		/// </summary>
		/// <param name="sqlCommand">
		/// The sql command.
		/// </param>
		public void PurgeSql(SqlCommand sqlCommand)
		{
			string select = "DELETE FROM  tblMobileDevice ";
			string where = "WHERE MobileDeviceGuid = @MobileDeviceGuid ";

			sqlCommand.CommandText = select + where;

			var parm = new SqlParameter("@mobileDeviceGuid", SqlDbType.UniqueIdentifier) { Value = this.mobileDeviceGuid };
			sqlCommand.Parameters.Add(parm);
		}

		/// <summary>
		/// This method will populate the sql command with the insert data.
		/// </summary>
		/// <param name="sqlCommand">
		/// The sql command.
		/// </param>
		public void InsertSql(SqlCommand sqlCommand)
		{
			string insert = "INSERT INTO tblMobileDevice ( " 
							+ "mobileDeviceGuid, " 
							+ "SiteGuid,  " 
							+ "MobileDeviceID,  " 
							+ "Description,  " 
							+ "mobileDeviceType, "
							+ "CreatedBy, " 
							+ "CreatedDate, " 
							+ "UpdatedBy, " 
							+ "UpdatedDate ) ";

			string insertValues = "VALUES ( " 
									+ "@mobileDeviceGuid, " 
									+ "@SiteGuid,  " 
									+ "@MobileDeviceID,  " 
									+ "@Description,  " 
									+ "@mobileDeviceType, "
									+ "@CreatedBy, " 
									+ "@CreatedDate, " 
									+ "@UpdatedBy, " 
									+ "@UpdatedDate ) ";

			sqlCommand.CommandText = insert + insertValues;

			var parm = new SqlParameter("@mobileDeviceGuid", SqlDbType.UniqueIdentifier) { Value = this.mobileDeviceGuid };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@SiteGuid", SqlDbType.UniqueIdentifier) { Value = this.siteGuid };
			sqlCommand.Parameters.Add(parm);

			parm = string.IsNullOrEmpty(this.mobileDeviceId)
				? new SqlParameter("@MobileDeviceID", SqlDbType.NVarChar, 50) { Value = DBNull.Value }
				: new SqlParameter("@MobileDeviceID", SqlDbType.NVarChar, 50) { Value = this.mobileDeviceId };
			sqlCommand.Parameters.Add(parm);

			parm = string.IsNullOrEmpty(this.description) 
				? new SqlParameter("@Description", SqlDbType.NVarChar, 200) { Value = DBNull.Value } 
				: new SqlParameter("@Description", SqlDbType.NVarChar, 200) { Value = this.description };
			sqlCommand.Parameters.Add(parm);

			parm = this.mobileDeviceType == null
				? new SqlParameter("@mobileDeviceType", SqlDbType.Int) { Value = DBNull.Value }
				: new SqlParameter("@mobileDeviceType", SqlDbType.Int) { Value = this.mobileDeviceType };
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
		/// This method will populate the sql command with on the columns that 
		/// have changed. It will set the sqlCommand to null if there are no columns
		/// that have changed.
		/// </summary>
		/// <param name="sqlCommand">
		/// The sql command. Will be null if no column changes.
		/// </param>
		public void UpdateSql(SqlCommand sqlCommand)
		{
			string update = "UPDATE tblMobileDevice SET ";
			string where = "WHERE MobileDeviceGuid = @MobileDeviceGuid ";

			// Will return a list of property names that their values changed.
			List<string> changedProperties = this.CompareForChanges( );

			if ( (changedProperties == null) || (changedProperties.Count == 0) )
			{
				sqlCommand.CommandText = string.Empty;
			}
			else
			{
				bool firstTime = true;
				List<string> updateVariables = this.BuildUpdateSql(sqlCommand, changedProperties);

				foreach ( string setCommand in updateVariables )
				{
					if ( firstTime )
					{
						update = update + setCommand;
						firstTime = false;
					}
					else
					{
						update = update + ", " + setCommand;
					}
				}

				if ( updateVariables.Count > 0 )
				{
					var parm = new SqlParameter("@MobileDeviceGuid", SqlDbType.UniqueIdentifier) { Value = this.mobileDeviceGuid };
					sqlCommand.Parameters.Add(parm);

					sqlCommand.CommandText = update + " " + where;
				}
			}
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method will load the information from the data row into the object's
		/// properties.
		/// </summary>
		/// <param name="row">The data row to be loaded.</param>
		private void LoadRow(DataRow row)
		{
			if ( row != null )
			{
				this.mobileDeviceGuid	= row.IsNull("MobileDeviceGuid") ? Guid.Empty : (Guid) row["MobileDeviceGuid"];
				this.siteGuid			= row.IsNull("SiteGuid") ? Guid.Empty : (Guid) row["SiteGuid"];
				this.mobileDeviceId		= row.IsNull("MobileDeviceId") ? string.Empty : (string) row["MobileDeviceId"];
				this.description		= row.IsNull("Description") ? string.Empty : (string) row["Description"];
				this.createdBy			= row.IsNull("CreatedBy") ? string.Empty : (string) row["CreatedBy"];
				this.updatedBy			= row.IsNull("UpdatedBy") ? string.Empty : (string) row["UpdatedBy"];

				this.mobileDeviceType = null;
				if ( row.IsNull("MobileDeviceType") == false )
				{
					this.mobileDeviceType = (int)row["MobileDeviceType"];
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
		/// This method will compare the current property values with the old values. If there
		/// are changes the name of the property is added to a change list.
		/// </summary>
		/// <returns>A change list that contains the properties that have changed.
		/// </returns>
		private List<string> CompareForChanges( )
		{
			var oldDataSet = (DataSet) this.DeserializeData( );

			var oldMobileDevice = new MobileDeviceClass( );
			oldMobileDevice.Load(oldDataSet);

			List<string> changedProperties = this.GetChangedColumns(this, oldMobileDevice);
			return changedProperties;
		}

		/// <summary>
		/// This method will build an update statement on the columns that changed.
		/// </summary>
		/// <param name="sqlCommand">
		/// The sql command.
		/// </param>
		/// <param name="changedProperties">
		/// The changed properties.
		/// </param>
		/// <returns>Returns a collection of update statements to be updated.
		/// </returns>
		private List<string> BuildUpdateSql(SqlCommand sqlCommand, List<string> changedProperties)
		{
			var updateVariables = new List<string>( );
			bool hasOtherChanges = false;
			SqlParameter parm;

			foreach ( string propertyName in changedProperties )
			{
				if ( propertyName.Equals("SiteGuid") )
				{
					updateVariables.Add(" SiteGuid = @SiteGuid");
					parm = new SqlParameter("@SiteGuid", SqlDbType.UniqueIdentifier) { Value = this.siteGuid };
					sqlCommand.Parameters.Add(parm);
					hasOtherChanges = true;
				}

				if ( propertyName.Equals("mobileDeviceId") )
				{
					updateVariables.Add(" MobileDeviceID = @MobileDeviceID");
					parm = string.IsNullOrEmpty(this.mobileDeviceId)
						? new SqlParameter("@MobileDeviceID", SqlDbType.NVarChar, 50) { Value = DBNull.Value }
						: new SqlParameter("@MobileDeviceID", SqlDbType.NVarChar, 50) { Value = this.mobileDeviceId };
					sqlCommand.Parameters.Add(parm);
					hasOtherChanges = true;
				}

				if ( propertyName.Equals("Description") )
				{
					updateVariables.Add(" Description = @Description");
					parm = string.IsNullOrEmpty(this.description) 
						? new SqlParameter("@Description", SqlDbType.NVarChar, 200) { Value = DBNull.Value } 
						: new SqlParameter("@Description", SqlDbType.NVarChar, 200) { Value = this.description };
					sqlCommand.Parameters.Add(parm);
					hasOtherChanges = true;
				}

				if ( propertyName.Equals("mobileDeviceType") )
				{
					updateVariables.Add(" mobileDeviceType = @mobileDeviceType");
					parm = this.mobileDeviceType == null
						? new SqlParameter("@mobileDeviceType", SqlDbType.Int) { Value = DBNull.Value }
						: new SqlParameter("@mobileDeviceType", SqlDbType.Int) { Value = this.mobileDeviceType };
					sqlCommand.Parameters.Add(parm);
					hasOtherChanges = true;
				}
			}

			if ( hasOtherChanges )
			{
				updateVariables.Add(" CreatedBy = @CreatedBy");
				parm = new SqlParameter("@CreatedBy", SqlDbType.NVarChar, 100) { Value = this.createdBy };
				sqlCommand.Parameters.Add(parm);
			}

			if ( hasOtherChanges )
			{
				updateVariables.Add(" UpdatedBy = @UpdatedBy");
				parm = new SqlParameter("@UpdatedBy", SqlDbType.NVarChar, 100) { Value = this.updatedBy };
				sqlCommand.Parameters.Add(parm);
			}

			if ( hasOtherChanges )
			{
				updateVariables.Add(" CreatedDate = @CreatedDate");
				parm = new SqlParameter("@CreatedDate", SqlDbType.DateTimeOffset) { Value = this.createdDate };
				sqlCommand.Parameters.Add(parm);
			}

			if ( hasOtherChanges )
			{
				updateVariables.Add(" UpdatedDate = @UpdatedDate");
				parm = new SqlParameter("@UpdatedDate", SqlDbType.DateTimeOffset) { Value = this.updatedDate };
				sqlCommand.Parameters.Add(parm);
			}

			return updateVariables;
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

	#region Mobile Device Collection Class
	/// <summary>
	/// This class contains a collection of Mobile device objects.
	/// </summary>
	[Serializable]
	[CollectionDataContract]
	public class MobileDeviceCollection : List<MobileDeviceClass>
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

					var mobileDevice = new MobileDeviceClass( );
					mobileDevice.LoadSingle(singleRowDataSet);
					this.Add(mobileDevice);
				}
			}
		}
		#endregion
	}
	#endregion
}

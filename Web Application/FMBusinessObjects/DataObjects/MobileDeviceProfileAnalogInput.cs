// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MobileDeviceProfileAnalogInput.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the MobileDeviceProfileAnalogInput type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Globalization;
	using System.Runtime.Serialization;

	/// <summary>
	/// The purpose of the Mobile Device Profile Analog Input data object is to contain data and SQL to add,
	/// modify, and delete profile information from the database.
	/// </summary>
	[Serializable]
	[DataContract]
	public class MobileDeviceProfileAnalogInput : DataObject
	{
		#region Private data memember
		[DataMember] private Guid mobileDeviceProfileAnalogInputGuid;
		[DataMember] private Guid mobileDeviceProfileGuid;
		[DataMember] private double lowLimit;
		[DataMember] private double highLimit;
		[DataMember] private string parameterA;
		[DataMember] private string parameterB;
		[DataMember] private string parameterC;
		[DataMember] private string analogFormula;
		[DataMember] private string createdBy;
		[DataMember] private string updatedBy;
		[DataMember] private DateTimeOffset? createdDate;
		[DataMember] private DateTimeOffset? updatedDate;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="MobileDeviceProfileAnalogInput"/> class.
		/// </summary>
		public MobileDeviceProfileAnalogInput ( )
		{
			this.Reset();
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the mobile device profile analog input guid.
		/// </summary>
		public Guid MobileDeviceProfileAnalogInputGuid
		{
			get { return this.mobileDeviceProfileAnalogInputGuid; }
			set { this.mobileDeviceProfileAnalogInputGuid = value; }
		}

		/// <summary>
		/// Gets or sets the mobile device profile guid.
		/// </summary>
		public Guid MobileDeviceProfileGuid
		{
			get { return this.mobileDeviceProfileGuid; }
			set { this.mobileDeviceProfileGuid = value; }
		}

		/// <summary>
		/// Gets or sets the low limit.
		/// </summary>
		public double LowLimit
		{
			get { return this.lowLimit; }
			set { this.lowLimit = value; }
		}

		/// <summary>
		/// Gets or sets the high limit.
		/// </summary>
		public double HighLimit
		{
			get { return this.highLimit; }
			set { this.highLimit = value; }
		}

		/// <summary>
		/// Gets or sets the parameter a.
		/// </summary>
		public string ParameterA
		{
			get { return this.parameterA; }
			set { this.parameterA = value; }
		}

		/// <summary>
		/// Gets or sets the parameter b.
		/// </summary>
		public string ParameterB
		{
			get { return this.parameterB; }
			set { this.parameterB = value; }
		}

		/// <summary>
		/// Gets or sets the parameter c.
		/// </summary>
		public string ParameterC
		{
			get { return this.parameterC; }
			set { this.parameterC = value; }
		}

		/// <summary>
		/// Gets or sets the analog formula.
		/// </summary>
		public string AnalogFormula
		{
			get { return this.analogFormula; }
			set { this.analogFormula = value; }
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
		#endregion

		#region Public methods
		/// <summary>
		/// This method initializes the object to initial state.
		/// </summary>
		public void Reset ( )
		{
			this.mobileDeviceProfileAnalogInputGuid = Guid.Empty;
			this.mobileDeviceProfileGuid			= Guid.Empty;
			this.lowLimit							= 0.0;
			this.highLimit							= 0.0;
			this.parameterA							= string.Empty;
			this.parameterB							= string.Empty;
			this.parameterC							= string.Empty;
			this.analogFormula						= string.Empty;
			this.createdBy							= string.Empty;
			this.updatedBy							= string.Empty;
			this.createdDate						= null;
			this.updatedDate						= null;
		}
		#endregion

		#region SQL Statements
		/// <summary>
		/// This method will populate the sql command with the SQL text
		/// to retrieve all analog input profiles for a given profile.
		/// </summary>
		/// <param name="sqlCommand">
		/// The sql command.
		/// </param>
		/// <param name="profileGuid">
		/// The mobile Device Profile Guid.
		/// </param>
		public void EnumerateByMobileDeviceProfileGuidSql ( SqlCommand sqlCommand, Guid profileGuid )
		{
			string select = "SELECT * ";
			string from   = "FROM tblMobileDeviceProfileAnalogInput WITH ( NOLOCK ) ";
			string where  = "WHERE MobileDeviceProfileGuid = @MobileDeviceProfileGuid";

			sqlCommand.CommandText = select + from + where;

			var parm = new SqlParameter ( "@MobileDeviceProfileGuid", SqlDbType.UniqueIdentifier ) { Value = profileGuid };
			sqlCommand.Parameters.Add(parm);
		}

		/// <summary>
		/// This method will populate the sql command to retrieve mobile device profile
		/// analog input data based on the GUID.
		/// </summary>
		/// <param name="sqlCommand">
		/// The sql command.
		/// </param>
		public void GetByAnalogInputProfileByGuidSql ( SqlCommand sqlCommand )
		{
			string select = "SELECT * ";
			string from   = "FROM tblMobileDeviceProfileAnalogInput WITH ( NOLOCK ) ";
			string where  = "WHERE MobileDeviceProfileAnalogInputGuid = @MobileDeviceProfileAnalogInputGuid ";

			sqlCommand.CommandText = select + from + where;

			SqlParameter parm = new SqlParameter ( "@MobileDeviceProfileAnalogInputGuid", SqlDbType.UniqueIdentifier ) { Value = this.mobileDeviceProfileAnalogInputGuid };
			sqlCommand.Parameters.Add ( parm );
		}

		/// <summary>
		/// This method will populate the sql command to remove a mobile device profile
		/// analog input record from the database based on the GUID.
		/// </summary>
		/// <param name="sqlCommand">
		/// The sql command.
		/// </param>
		/// <param name="deleteList">
		/// The delete List.
		/// </param>
		public void PurgeSql ( SqlCommand sqlCommand, List<MobileDeviceProfileAnalogInput> deleteList )
		{
			if ( deleteList == null || deleteList.Count <= 0 )
			{
				sqlCommand.CommandText = string.Empty;
			}
			else
			{
				bool firstEntry = true;
				int parameterCount = 0;

				string select = "DELETE FROM  tblMobileDeviceProfileAnalogInput ";
				string where1 = "WHERE MobileDeviceProfileAnalogInputGuid IN ( ";
				string where2 = " ) ";

				foreach (MobileDeviceProfileAnalogInput analogInput in deleteList)
				{
					string parmName = "@MobileDeviceProfileAnalogInputGuid" + parameterCount.ToString(CultureInfo.InvariantCulture);

					if (firstEntry)
					{
						where1 = where1 + parmName;

						var parm = new SqlParameter(parmName, SqlDbType.UniqueIdentifier) { Value = analogInput.mobileDeviceProfileAnalogInputGuid };
						sqlCommand.Parameters.Add(parm);

						firstEntry = false;
					}
					else
					{
						where1 = where1 + ", " + parmName;
					}

					parameterCount++;
				}


				sqlCommand.CommandText = select + where1 + where2;
			}
		}

		/// <summary>
		/// This method will populate the sql command to remove mobile device profile
		/// analog input records from the database based on the profile GUID.
		/// </summary>
		/// <param name="sqlCommand">
		/// The sql command.
		/// </param>
		public void PurgeByProfileGuidSql ( SqlCommand sqlCommand )
		{
			string select = "DELETE FROM  tblMobileDeviceProfileAnalogInput ";
			string where  = "WHERE MobileDeviceProfileGuid = @MobileDeviceProfileGuid ";

			sqlCommand.CommandText = select + where;

			SqlParameter parm = new SqlParameter ( "@MobileDeviceProfileGuid", SqlDbType.UniqueIdentifier ) { Value = this.mobileDeviceProfileGuid };
			sqlCommand.Parameters.Add ( parm );
		}

		/// <summary>
		/// This method will populate the sql command with the insert data.
		/// </summary>
		/// <param name="sqlCommand">
		/// The sql command.
		/// </param>
		public void InsertSql ( SqlCommand sqlCommand )
		{
			string insert = "INSERT INTO tblMobileDeviceProfileAnalogInput ( " +
							"MobileDeviceProfileAnalogInputGuid, " +
							"MobileDeviceProfileGuid, " +
							"LowLimit, " +
							"HighLimit, " +
							"ParameterA, " +
							"ParameterB, " +
							"ParameterC, " +
							"AnalogFormula, " +
							"CreatedDate, " +
							"CreatedBy, " +
							"UpdatedDate, " +
							"UpdatedBy ) ";

			string insertValues = "VALUES ( " +
								"@MobileDeviceProfileAnalogInputGuid, " +
								"@MobileDeviceProfileGuid, " +
								"@LowLimit, " +
								"@HighLimit, " +
								"@ParameterA, " +
								"@ParameterB, " +
								"@ParameterC, " +
								"@AnalogFormula, " +
								"@CreatedDate, " +
								"@CreatedBy, " +
								"@UpdatedDate, " +
								"@UpdatedBy ) ";

			sqlCommand.CommandText = insert + insertValues;

			SqlParameter parm = new SqlParameter ( "@MobileDeviceProfileAnalogInputGuid", SqlDbType.UniqueIdentifier ) { Value = Guid.NewGuid() };
			sqlCommand.Parameters.Add ( parm );
			parm = new SqlParameter ( "@MobileDeviceProfileGuid", SqlDbType.UniqueIdentifier ) { Value = this.mobileDeviceProfileGuid };
			sqlCommand.Parameters.Add ( parm );
			parm = new SqlParameter ( "@LowLimit", SqlDbType.Float ) { Value = this.lowLimit };
			sqlCommand.Parameters.Add ( parm );
			parm = new SqlParameter ( "@HighLimit", SqlDbType.Float ) { Value = this.highLimit };
			sqlCommand.Parameters.Add ( parm );
			parm = new SqlParameter ( "@ParameterA", SqlDbType.NVarChar, 20 ) { Value = this.parameterA };
			sqlCommand.Parameters.Add ( parm );
			parm = new SqlParameter ( "@ParameterB", SqlDbType.NVarChar, 20 ) { Value = this.parameterB };
			sqlCommand.Parameters.Add ( parm );
			parm = new SqlParameter ( "@ParameterC", SqlDbType.NVarChar, 20 ) { Value = this.parameterC };
			sqlCommand.Parameters.Add ( parm );
			parm = new SqlParameter ( "@AnalogFormula", SqlDbType.NVarChar, 50 ) { Value = this.analogFormula };
			sqlCommand.Parameters.Add ( parm );		
			parm = new SqlParameter ( "@CreatedBy", SqlDbType.NVarChar, 100 ) { Value = this.createdBy };
			sqlCommand.Parameters.Add ( parm );
			parm = new SqlParameter ( "@UpdatedBy", SqlDbType.NVarChar, 100 ) { Value = this.updatedBy };
			sqlCommand.Parameters.Add ( parm );

			if ( this.createdDate != null )
			{
				parm = new SqlParameter ( "@CreatedDate", SqlDbType.DateTimeOffset ) { Value = this.createdDate.Value };
				sqlCommand.Parameters.Add ( parm );
			}

			if ( this.updatedDate != null )
			{
				parm = new SqlParameter ( "@UpdatedDate", SqlDbType.DateTimeOffset ) { Value = this.updatedDate.Value };
				sqlCommand.Parameters.Add ( parm );
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
		public void UpdateSql ( SqlCommand sqlCommand )
		{
			string update = "UPDATE tblMobileDeviceProfileAnalogInput SET ";
			string where  = "WHERE MobileDeviceProfileAnalogInputGuid = @MobileDeviceProfileAnalogInputGuid ";

			// Will return a list of property names that their values changed.
			List<string> changedProperties = this.CompareForChanges ( );

			if ( ( changedProperties == null ) || ( changedProperties.Count == 0 ) )
			{
				sqlCommand.CommandText = string.Empty;
			}
			else
			{
				bool firstTime = true;
				List<string> updateVariables = this.BuildUpdateSql ( sqlCommand, changedProperties );

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

				sqlCommand.CommandText = update + " " + where;

				var parm = new SqlParameter ( "@MobileDeviceProfileAnalogInputGuid", SqlDbType.UniqueIdentifier ) { Value = this.mobileDeviceProfileAnalogInputGuid };
				sqlCommand.Parameters.Add ( parm );
			}
		}

		/// <summary>
		/// This method will load a single record only.
		/// </summary>
		/// <param name="dataSet">
		/// The data set.
		/// </param>
		public void LoadSingle ( DataSet dataSet )
		{
			if ( ( dataSet != null ) && ( dataSet.Tables.Count > 0 ) )
			{
				DataTable table = dataSet.Tables[0];

				if ( ( table != null ) && ( table.Rows != null ) && ( table.Rows.Count > 0 ) )
				{
					DataRow row = table.Rows[0];
					this.LoadRow(row);

					// Serialize the dataset to be used later for comparison.
					this.SerializeData ( dataSet );
				}
			}
		}
		#endregion

		#region Private methods

		/// <summary>
		/// This method will compare the current property values with the old values. If there
		/// are changes the name of the property is added to a change list.
		/// </summary>
		/// <returns>A change list that contains the properties that have changed.
		/// </returns>
		private List<string> CompareForChanges ( )
		{
			DataSet oldDataSet = ( DataSet ) this.DeserializeData ( );

			var oldMobileDeviceProfileAnalogInput = new MobileDeviceProfileAnalogInput ( );
			oldMobileDeviceProfileAnalogInput.LoadSingle ( oldDataSet );

			List<string> changedProperties = this.GetChangedColumns ( this, oldMobileDeviceProfileAnalogInput );

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
		private List<string> BuildUpdateSql ( SqlCommand sqlCommand, List<string> changedProperties )
		{
			var updateVariables = new List<string>( );
			bool hasOtherChanges = false;
			SqlParameter parm;

			foreach ( string propertyName in changedProperties )
			{
				if ( propertyName.Equals ( "LowLimit" ) )
				{
					updateVariables.Add ( " LowLimit = @LowLimit" );
					parm = new SqlParameter ( "@LowLimit", SqlDbType.Float ) { Value = this.lowLimit };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "HighLimit" ) )
				{
					updateVariables.Add ( " HighLimit = @HighLimit" );
					parm = new SqlParameter ( "@HighLimit", SqlDbType.Float ) { Value = this.highLimit };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "ParameterA" ) )
				{
					updateVariables.Add ( " ParameterA = @ParameterA" );
					parm = new SqlParameter ( "@ParameterA", SqlDbType.NVarChar, 20 ) { Value = this.parameterA };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "ParameterB" ) )
				{
					updateVariables.Add ( " ParameterB = @ParameterB" );
					parm = new SqlParameter ( "@ParameterB", SqlDbType.NVarChar, 20 ) { Value = this.parameterB };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "ParameterC" ) )
				{
					updateVariables.Add ( " ParameterC = @ParameterC" );
					parm = new SqlParameter ( "@ParameterC", SqlDbType.NVarChar, 20 ) { Value = this.parameterC };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "AnalogFormula" ) )
				{
					updateVariables.Add ( " AnalogFormula = @AnalogFormula" );
					parm = new SqlParameter ( "@AnalogFormula", SqlDbType.NVarChar, 50 ) { Value = this.analogFormula };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}
			}

			if ( hasOtherChanges )
			{
				updateVariables.Add ( " CreatedBy = @CreatedBy" );
				parm = new SqlParameter ( "@CreatedBy", SqlDbType.NVarChar, 100 ) { Value = this.createdBy };
				sqlCommand.Parameters.Add ( parm );

				updateVariables.Add ( " UpdatedBy = @UpdatedBy" );
				parm = new SqlParameter ( "@UpdatedBy", SqlDbType.NVarChar, 100 ) { Value = this.updatedBy };
				sqlCommand.Parameters.Add ( parm );

				updateVariables.Add ( " CreatedDate = @CreatedDate" );
				parm = new SqlParameter ( "@CreatedDate", SqlDbType.DateTimeOffset ) { Value = this.createdDate };
				sqlCommand.Parameters.Add ( parm );

				updateVariables.Add ( " UpdatedDate = @UpdatedDate" );
				parm = new SqlParameter ( "@UpdatedDate", SqlDbType.DateTimeOffset ) { Value = this.updatedDate };
				sqlCommand.Parameters.Add ( parm );
			}

			return updateVariables;
		}

		/// <summary>
		/// This method will load the information from the data row into the object's
		/// properties.
		/// </summary>
		/// <param name="row">
		/// The row.
		/// </param>
		private void LoadRow ( DataRow row )
		{
			if ( row != null )
			{
				this.mobileDeviceProfileAnalogInputGuid = row.IsNull("MobileDeviceProfileAnalogInputGuid") ? Guid.Empty : (Guid)row["MobileDeviceProfileAnalogInputGuid"];
				this.mobileDeviceProfileGuid			= row.IsNull("MobileDeviceProfileGuid") ? Guid.Empty : (Guid)row["mobileDeviceProfileGuid"];
				this.lowLimit							= row.IsNull("LowLimit") ? 0.0 : (double)row["LowLimit"];
				this.highLimit							= row.IsNull("HighLimit") ? 0.0 : (double)row["HighLimit"];
				this.parameterA							= row.IsNull("ParameterA") ? string.Empty : (string)row["ParameterA"];
				this.parameterB							= row.IsNull("ParameterB") ? string.Empty : (string)row["ParameterB"];
				this.parameterC							= row.IsNull("ParameterC") ? string.Empty : (string)row["ParameterC"];
				this.analogFormula						= row.IsNull("AnalogFormula") ? string.Empty : (string)row["AnalogFormula"];
				this.createdBy							= row.IsNull("CreatedBy") ? string.Empty : (string)row["CreatedBy"];
				this.updatedBy							= row.IsNull("UpdatedBy") ? string.Empty : (string)row["UpdatedBy"];

				if ( row.IsNull ( "CreatedDate" ) == false )
				{
					this.createdDate = (DateTimeOffset)row["CreatedDate"];
				}

				if ( row.IsNull ( "UpdatedDate" ) == false )
				{
					this.updatedDate = (DateTimeOffset)row["UpdatedDate"];
				}
			}
		}
		#endregion

		#region Overrides
		/// <summary>
		/// The get update command.
		/// </summary>
		/// <returns>
		/// The System.String.
		/// </returns>
		override public string getUpdateCommand ( )
		{
			return null;
		}

		/// <summary>
		/// The get delete command.
		/// </summary>
		/// <returns>
		/// The System.String.
		/// </returns>
		override public string getDeleteCommand ( )
		{
			return null;
		}

		/// <summary>
		/// The get insert command.
		/// </summary>
		/// <returns>
		/// The System.String.
		/// </returns>
		override public string getInsertCommand ( )
		{
			return null;
		}

		/// <summary>
		/// The get select command.
		/// </summary>
		/// <returns>
		/// The System.String.
		/// </returns>
		override public string getSelectCommand ( )
		{
			return null;
		}
		#endregion
	}

	#region Mobile Device Profile Analog Input Collection
	/// <summary>
	/// The mobile device profile analog input collection.
	/// </summary>
	[Serializable]
	[CollectionDataContract]
	public class MobileDeviceProfileAnalogInputCollection : List<MobileDeviceProfileAnalogInput>
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
		public void Load (DataSet dataSet)
		{
			if ( ( dataSet != null ) && ( dataSet.Tables.Count > 0 ) )
			{
				var table = dataSet.Tables[0];

				foreach (DataRow row in table.Rows)
				{
					var singleRowDataSet = dataSet.Clone();
					var newTable = singleRowDataSet.Tables[0];
					var newRow = newTable.NewRow();

					newRow.ItemArray = row.ItemArray;
					newTable.Rows.Add ( newRow );

					var analogInput = new MobileDeviceProfileAnalogInput();
					analogInput.LoadSingle(singleRowDataSet);
					this.Add(analogInput);
				}
			}
		}
		#endregion
	}
	#endregion
}

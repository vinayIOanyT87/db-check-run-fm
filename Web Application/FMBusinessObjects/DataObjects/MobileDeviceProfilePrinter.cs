// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MobileDeviceProfilePrinter.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the MobileDeviceProfilePrinter type.
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
	/// The purpose of the Mobile Device Profile Printer data object is to contain data and SQL to add,
	/// modify, and delete profile information from the database.
	/// </summary>
	[Serializable]
	[DataContract]
	public class MobileDeviceProfilePrinter : DataObject
	{
		#region Private data members
		[DataMember] private Guid mobileDeviceProfilePrinterGuid;
		[DataMember] private Guid mobileDeviceProfileGuid;
		[DataMember] private string printerId;
		[DataMember] private string printerBaudRate;
		[DataMember] private string printerComPort;
		[DataMember] private string printerDataBits;
		[DataMember] private string printerStopBits;
		[DataMember] private string printerUseXonXoff;
		[DataMember] private string printerXonChar;
		[DataMember] private string printerXoffChar;
		[DataMember] private string printerBufferSize;
		[DataMember] private string printerParity;
		[DataMember] private string createdBy;
		[DataMember] private string updatedBy;
		[DataMember] private DateTimeOffset? createdDate;
		[DataMember] private DateTimeOffset? updatedDate;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="MobileDeviceProfilePrinter"/> class.
		/// </summary>
		public MobileDeviceProfilePrinter()
		{
			this.Reset();
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the mobile device profile printer guid.
		/// </summary>
		public Guid MobileDeviceProfilePrinterGuid
		{
			get { return this.mobileDeviceProfilePrinterGuid; }
			set { this.mobileDeviceProfilePrinterGuid = value; }
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
		/// Gets or sets the printer baud rate.
		/// </summary>
		public string PrinterBaudRate
		{
			get { return this.printerBaudRate; }
			set { this.printerBaudRate = value; }
		}

		/// <summary>
		/// Gets or sets the printer com port.
		/// </summary>
		public string PrinterComPort
		{
			get { return this.printerComPort; }
			set { this.printerComPort = value; }
		}

		/// <summary>
		/// Gets or sets the printer id.
		/// </summary>
		public string PrinterId
		{
			get { return this.printerId; }
			set { this.printerId = value; }
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

		/// <summary>
		/// Gets or sets the printer data bits.
		/// </summary>
		public string PrinterDataBits
		{
			get { return this.printerDataBits; }
			set { this.printerDataBits = value; }
		}

		/// <summary>
		/// Gets or sets the printer stops bits.
		/// </summary>
		public string PrinterStopBits
		{
			get { return this.printerStopBits; }
			set { this.printerStopBits = value; }
		}

		/// <summary>
		/// Gets or sets the printer use xon xoff.
		/// </summary>
		public string PrinterUseXonXoff
		{
			get { return this.printerUseXonXoff; }
			set { this.printerUseXonXoff = value; }
		}

		/// <summary>
		/// Gets or sets the printer xon char.
		/// </summary>
		public string PrinterXonChar
		{
			get { return this.printerXonChar; }
			set { this.printerXonChar = value; }
		}

		/// <summary>
		/// Gets or sets the printer xoff char.
		/// </summary>
		public string PrinterXoffChar
		{
			get { return this.printerXoffChar; }
			set { this.printerXoffChar = value; }
		}

		/// <summary>
		/// Gets or sets the printer buffer size.
		/// </summary>
		public string PrinterBufferSize
		{
			get { return this.printerBufferSize; }
			set { this.printerBufferSize = value; }
		}

		/// <summary>
		/// Gets or sets the printer parity.
		/// </summary>
		public string PrinterParity
		{
			get { return this.printerParity; }
			set { this.printerParity = value; }
		}
		#endregion

		#region Public methods
		/// <summary>
		/// This method initializes the object to initial state.
		/// </summary>
		public void Reset()
		{
			this.mobileDeviceProfilePrinterGuid = Guid.Empty;
			this.mobileDeviceProfileGuid		= Guid.Empty;
			this.printerBaudRate				= string.Empty;
			this.printerComPort					= string.Empty;
			this.printerId						= string.Empty;
			this.createdBy						= string.Empty;
			this.updatedBy						= string.Empty;
			this.createdDate					= null;
			this.updatedDate					= null;
			this.printerDataBits				= "00000000";
			this.printerStopBits				= "00000000";
			this.printerUseXonXoff				= "00000000";
			this.printerXonChar					= "00000000";
			this.printerXoffChar				= "00000000";
			this.printerBufferSize				= "00000000";
			this.printerParity					= string.Empty;
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
		/// This method will return true if there is a Printer ID that is a duplicate within
		/// the same Mobile Device Profile GUID.  Otherwise, it returns false.
		/// </summary>
		/// <param name="dataSet">
		/// The data set.
		/// </param>
		/// <returns>
		/// The System.Boolean.
		/// </returns>
		public bool DuplicatePrinterId(DataSet dataSet)
		{
			bool duplicateId = false;

			if ( (dataSet != null) && (dataSet.Tables.Count > 0) )
			{
				DataTable table = dataSet.Tables[0];

				if ( (table != null) && (table.Rows != null) && (table.Rows.Count > 0) )
				{
					DataRow row = table.Rows[0];
					int printerIdCount = row.IsNull("PrinterIdCount") ? 0 : (int)row["PrinterIdCount"];

					if ( printerIdCount > 0 )
					{
						duplicateId = true;
					}
				}
			}

			return duplicateId;
		}
		#endregion

		#region SQL Methods
		/// <summary>
		/// This method populates the SQL Command with a select that will check for 
		/// duplicate profile IDs.
		/// </summary>
		/// <param name="sqlCommand">
		/// The sql command.
		/// </param>
		public void CheckForDuplicatePrinterIDs(SqlCommand sqlCommand)
		{
			if ( string.IsNullOrEmpty(this.printerId) || this.MobileDeviceProfileGuid == Guid.Empty )
			{
				sqlCommand.CommandText = string.Empty;
			}

			string select = "SELECT COUNT(*) AS PrinterIdCount ";
			string from   = "FROM tblMobileDeviceProfilePrinter WITH ( NOLOCK ) ";
			string where  = "WHERE MobileDeviceProfileGuid = @MobileDeviceProfileGuid AND PrinterID = @PrinterID ";

			sqlCommand.CommandText = select + from + where;

			var parm = new SqlParameter("@MobileDeviceProfileGuid", SqlDbType.UniqueIdentifier) { Value = this.mobileDeviceProfileGuid };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@PrinterID", SqlDbType.NVarChar, 30) { Value = this.printerId };
			sqlCommand.Parameters.Add(parm);
		}

		/// <summary>
		/// This method will populate the sql command with the SQL text
		/// to retrieve all printer profiles for a given profile.
		/// </summary>
		/// <param name="sqlCommand">
		/// The sql command.
		/// </param>
		/// <param name="profileGuid">
		/// The profile guid.
		/// </param>
		public void EnumerateByMobileDeviceProfileGuidSql(SqlCommand sqlCommand, Guid profileGuid)
		{
			string select = "SELECT * ";
			string from   = "FROM tblMobileDeviceProfilePrinter WITH ( NOLOCK ) ";
			string where  = "WHERE MobileDeviceProfileGuid = @MobileDeviceProfileGuid";

			sqlCommand.CommandText = select + from + where;

			var parm = new SqlParameter("@MobileDeviceProfileGuid", SqlDbType.UniqueIdentifier) { Value = profileGuid };
			sqlCommand.Parameters.Add(parm);
		}

		/// <summary>
		/// This method will populate the sql command to retrieve mobile device profile
		/// printer data based on the GUID.
		/// </summary>
		/// <param name="sqlCommand">
		/// The sql command.
		/// </param>
		public void GetByPrinterProfileByGuidSql(SqlCommand sqlCommand)
		{
			string select	= "SELECT * ";
			string from		= "FROM tblMobileDeviceProfilePrinter WITH ( NOLOCK ) ";
			string where	= "WHERE MobileDeviceProfilePrinterGuid = @MobileDeviceProfilePrinterGuid ";

			sqlCommand.CommandText = select + from + where;

			var parm = new SqlParameter("@MobileDeviceProfilePrinterGuid", SqlDbType.UniqueIdentifier) { Value = this.mobileDeviceProfilePrinterGuid };
			sqlCommand.Parameters.Add(parm);
		}

		/// <summary>
		/// This method will populate the sql command to remove a mobile device profile
		/// printer records from the database based on the GUID.
		/// </summary>
		/// <param name="sqlCommand">
		/// The sql command.
		/// </param>
		/// <param name="deleteList">
		/// The delete List.
		/// </param>
		public void PurgeSql(SqlCommand sqlCommand, List<MobileDeviceProfilePrinter> deleteList )
		{
			if ( deleteList == null || deleteList.Count <= 0 )
			{
				sqlCommand.CommandText = string.Empty;
			}
			else
			{
				bool firstEntry = true;
				int parameterCount = 0;

				string select = "DELETE FROM  tblMobileDeviceProfilePrinter ";
				string where1 = "WHERE MobileDeviceProfilePrinterGuid IN ( ";
				string where2 = " ) ";

				foreach ( MobileDeviceProfilePrinter printer in deleteList )
				{
					string parmName = "@MobileDevicePrinterGuid" + parameterCount.ToString(CultureInfo.InvariantCulture);

					if ( firstEntry )
					{
						where1 = where1 + parmName;

						var parm = new SqlParameter(parmName, SqlDbType.UniqueIdentifier) { Value = printer.mobileDeviceProfilePrinterGuid };
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
		/// printer records from the database based on the profile GUID.
		/// </summary>
		/// <param name="sqlCommand">
		/// The sql command.
		/// </param>
		public void PurgeByProfileGuidSql(SqlCommand sqlCommand)
		{
			string select = "DELETE FROM  tblMobileDeviceProfilePrinter ";
			string where  = "WHERE MobileDeviceProfileGuid = @MobileDeviceProfileGuid ";

			sqlCommand.CommandText = select + where;

			var parm = new SqlParameter("@MobileDeviceProfileGuid", SqlDbType.UniqueIdentifier) { Value = this.mobileDeviceProfileGuid };
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
			string insert = "INSERT INTO tblMobileDeviceProfilePrinter ( " +
							"MobileDeviceProfilePrinterGuid, " +
							"MobileDeviceProfileGuid, " +
							"PrinterID, " +
							"BaudRate, " +
							"COMPort, " +
							"DataBits, " +
							"StopBits, " +
							"UseXonXoff, " +
							"XonChar, " +
							"XoffChar, " +
							"BufferSize, " +
							"Parity, " +
							"CreatedBy, " +
							"UpdatedBy, " +
							"CreatedDate, " +
							"UpdatedDate ) ";

			string insertValues = "VALUES ( " +
								"@MobileDeviceProfilePrinterGuid, " +
								"@MobileDeviceProfileGuid, " +
								"@PrinterID, " +
								"@BaudRate, " +
								"@COMPort, " +
								"@DataBits, " +
								"@StopBits, " +
								"@UseXonXoff, " +
								"@XonChar, " +
								"@XoffChar, " +
								"@BufferSize, " +
								"@Parity, " +
								"@CreatedBy, " +
								"@UpdatedBy, " +
								"@CreatedDate, " +
								"@UpdatedDate ) ";

			sqlCommand.CommandText = insert + insertValues;

			var parm = new SqlParameter("@MobileDeviceProfilePrinterGuid", SqlDbType.UniqueIdentifier) { Value = Guid.NewGuid() };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@MobileDeviceProfileGuid", SqlDbType.UniqueIdentifier) { Value = this.mobileDeviceProfileGuid };
			sqlCommand.Parameters.Add(parm);

			parm = string.IsNullOrEmpty(this.printerId) 
				? new SqlParameter("@PrinterID", SqlDbType.NVarChar, 50) { Value = DBNull.Value } 
				: new SqlParameter("@PrinterID", SqlDbType.NVarChar, 50) { Value = this.printerId };
			sqlCommand.Parameters.Add(parm);

			parm = string.IsNullOrEmpty(this.printerBaudRate) 
				? new SqlParameter("@BaudRate", SqlDbType.NVarChar, 8) { Value = "00000000" } 
				: new SqlParameter("@BaudRate", SqlDbType.NVarChar, 8) { Value = this.printerBaudRate };
			sqlCommand.Parameters.Add(parm);

			parm = string.IsNullOrEmpty(this.printerComPort) 
				? new SqlParameter("@COMPort", SqlDbType.NVarChar, 4) { Value = DBNull.Value } 
				: new SqlParameter("@COMPort", SqlDbType.NVarChar, 4) { Value = this.printerComPort };
			sqlCommand.Parameters.Add(parm);

			parm = string.IsNullOrEmpty(this.printerDataBits) 
				? new SqlParameter("@DataBits", SqlDbType.NVarChar, 8) { Value = "00000000" } 
				: new SqlParameter("@DataBits", SqlDbType.NVarChar, 8) { Value = this.printerDataBits };
			sqlCommand.Parameters.Add(parm);

			parm = string.IsNullOrEmpty(this.printerStopBits) 
				? new SqlParameter("@StopBits", SqlDbType.NVarChar, 8) { Value = "00000000" } 
				: new SqlParameter("@StopBits", SqlDbType.NVarChar, 8) { Value = this.printerStopBits };
			sqlCommand.Parameters.Add(parm);

			parm = string.IsNullOrEmpty(this.printerUseXonXoff) 
				? new SqlParameter("@UseXonXoff", SqlDbType.NVarChar, 8) { Value = "00000000" } 
				: new SqlParameter("@UseXonXoff", SqlDbType.NVarChar, 8) { Value = this.printerUseXonXoff };
			sqlCommand.Parameters.Add(parm);

			parm = string.IsNullOrEmpty(this.printerXonChar) 
				? new SqlParameter("@XonChar", SqlDbType.NVarChar, 8) { Value = "00000000" } 
				: new SqlParameter("@XonChar", SqlDbType.NVarChar, 8) { Value = this.printerXonChar };
			sqlCommand.Parameters.Add(parm);

			parm = string.IsNullOrEmpty(this.printerXoffChar) 
				? new SqlParameter("@XoffChar", SqlDbType.NVarChar, 8) { Value = "00000000" } 
				: new SqlParameter("@XoffChar", SqlDbType.NVarChar, 8) { Value = this.printerXoffChar };
			sqlCommand.Parameters.Add(parm);

			parm = string.IsNullOrEmpty(this.printerBufferSize) 
				? new SqlParameter("@BufferSize", SqlDbType.NVarChar, 8) { Value = "00000000" } 
				: new SqlParameter("@BufferSize", SqlDbType.NVarChar, 8) { Value = this.printerBufferSize };
			sqlCommand.Parameters.Add(parm);

			parm = string.IsNullOrEmpty(this.printerParity) 
				? new SqlParameter("@Parity", SqlDbType.NVarChar, 12) { Value = DBNull.Value } 
				: new SqlParameter("@Parity", SqlDbType.NVarChar, 12) { Value = this.printerParity };
			sqlCommand.Parameters.Add(parm);

			parm = string.IsNullOrEmpty(this.createdBy) 
				? new SqlParameter("@CreatedBy", SqlDbType.NVarChar, 100) { Value = DBNull.Value } 
				: new SqlParameter("@CreatedBy", SqlDbType.NVarChar, 100) { Value = this.createdBy };
			sqlCommand.Parameters.Add(parm);

			parm = string.IsNullOrEmpty(this.updatedBy) 
				? new SqlParameter("@UpdatedBy", SqlDbType.NVarChar, 100) { Value = DBNull.Value } 
				: new SqlParameter("@UpdatedBy", SqlDbType.NVarChar, 100) { Value = this.updatedBy };
			sqlCommand.Parameters.Add(parm);

			if ( this.createdDate != null )
			{
				parm = new SqlParameter("@CreatedDate", SqlDbType.DateTimeOffset) { Value = this.createdDate.Value };
				sqlCommand.Parameters.Add(parm);
			}

			if ( this.updatedDate != null )
			{
				parm = new SqlParameter("@UpdatedDate", SqlDbType.DateTimeOffset) { Value = this.updatedDate.Value };
				sqlCommand.Parameters.Add(parm);
			}
		}

		/// <summary>
		/// This method will populate the sql command with on the columns that 
		/// have changed. It will set the sqlCommand to null if there are no columns
		/// that have changed.
		/// </summary>
		/// <param name="sqlCommand">
		/// The sql command.
		/// </param>
		public void UpdateSql(SqlCommand sqlCommand)
		{
			string update = "UPDATE tblMobileDeviceProfilePrinter SET ";
			string where  = "WHERE MobileDeviceProfilePrinterGuid = @MobileDeviceProfilePrinterGuid ";

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

				sqlCommand.CommandText = update + " " + where;

				var parm = new SqlParameter("@MobileDeviceProfilePrinterGuid", SqlDbType.UniqueIdentifier) { Value = this.mobileDeviceProfilePrinterGuid };
				sqlCommand.Parameters.Add(parm);
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
		private List<string> CompareForChanges( )
		{
			DataSet oldDataSet = (DataSet) this.DeserializeData( );

			var oldMobileDeviceProfilePrinter = new MobileDeviceProfilePrinter( );
			oldMobileDeviceProfilePrinter.LoadSingle(oldDataSet);

			List<string> changedProperties = this.GetChangedColumns(this, oldMobileDeviceProfilePrinter);

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
				if ( propertyName.Equals("PrinterId") )
				{
					updateVariables.Add(" PrinterID = @PrinterID");
					parm = new SqlParameter("@PrinterID", SqlDbType.NVarChar, 50) { Value = this.printerId };
					sqlCommand.Parameters.Add(parm);
					hasOtherChanges = true;
				}

				if ( propertyName.Equals("PrinterBaudRate") )
				{
					updateVariables.Add(" BaudRate = @BaudRate");
					parm = string.IsNullOrEmpty(this.printerBaudRate)
							? new SqlParameter("@BaudRate", SqlDbType.NVarChar, 8) { Value = "00000000" }
							: new SqlParameter("@BaudRate", SqlDbType.NVarChar, 8) { Value = this.printerBaudRate };
					sqlCommand.Parameters.Add(parm);
					hasOtherChanges = true;
				}

				if ( propertyName.Equals("PrinterComPort") )
				{
					updateVariables.Add(" COMPort = @COMPort");
					parm = string.IsNullOrEmpty(this.printerComPort)
							? new SqlParameter("@COMPort", SqlDbType.NVarChar, 4) { Value = DBNull.Value }
							: new SqlParameter("@COMPort", SqlDbType.NVarChar, 4) { Value = this.printerComPort };
					sqlCommand.Parameters.Add(parm);
					hasOtherChanges = true;
				}

				if ( propertyName.Equals("PrinterDataBits") )
				{
					updateVariables.Add(" DataBits = @DataBits");
					parm = string.IsNullOrEmpty(this.printerDataBits)
							? new SqlParameter("@DataBits", SqlDbType.NVarChar, 8) { Value = "00000000" }
							: new SqlParameter("@DataBits", SqlDbType.NVarChar, 8) { Value = this.printerDataBits };
					sqlCommand.Parameters.Add(parm);
					hasOtherChanges = true;
				}

				if ( propertyName.Equals("PrinterStopBits") )
				{
					updateVariables.Add(" StopBits = @StopBits");
					parm = string.IsNullOrEmpty(this.printerStopBits)
							? new SqlParameter("@StopBits", SqlDbType.NVarChar, 8) { Value = "00000000" }
							: new SqlParameter("@StopBits", SqlDbType.NVarChar, 8) { Value = this.printerStopBits };
					sqlCommand.Parameters.Add(parm);
					hasOtherChanges = true;
				}

				if ( propertyName.Equals("PrinterUseXonXoff") )
				{
					updateVariables.Add(" UseXonXoff = @UseXonXoff");
					parm = string.IsNullOrEmpty(this.printerUseXonXoff)
							? new SqlParameter("@UseXonXoff", SqlDbType.NVarChar, 8) { Value = "00000000" }
							: new SqlParameter("@UseXonXoff", SqlDbType.NVarChar, 8) { Value = this.printerUseXonXoff };
					sqlCommand.Parameters.Add(parm);
					hasOtherChanges = true;
				}

				if ( propertyName.Equals("PrinterXonChar") )
				{
					updateVariables.Add(" XonChar = @XonChar");
					parm = string.IsNullOrEmpty(this.printerXonChar)
							? new SqlParameter("@XonChar", SqlDbType.NVarChar, 8) { Value = "00000000" }
							: new SqlParameter("@XonChar", SqlDbType.NVarChar, 8) { Value = this.printerXonChar };
					sqlCommand.Parameters.Add(parm);
					hasOtherChanges = true;
				}

				if ( propertyName.Equals("PrinterXoffChar") )
				{
					updateVariables.Add(" XoffChar = @XoffChar");
					parm = string.IsNullOrEmpty(this.printerXoffChar)
							? new SqlParameter("@XoffChar", SqlDbType.NVarChar, 8) { Value = "00000000" }
							: new SqlParameter("@XoffChar", SqlDbType.NVarChar, 8) { Value = this.printerXoffChar };
					sqlCommand.Parameters.Add(parm);
					hasOtherChanges = true;
				}

				if ( propertyName.Equals("PrinterBufferSize") )
				{
					updateVariables.Add(" BufferSize = @BufferSize");
					parm = string.IsNullOrEmpty(this.printerBufferSize)
							? new SqlParameter("@BufferSize", SqlDbType.NVarChar, 8) { Value = "00000000" }
							: new SqlParameter("@BufferSize", SqlDbType.NVarChar, 8) { Value = this.printerBufferSize };
					sqlCommand.Parameters.Add(parm);
					hasOtherChanges = true;
				}

				if ( propertyName.Equals("PrinterParity") )
				{
					updateVariables.Add(" Parity = @Parity");
					parm = string.IsNullOrEmpty(this.printerParity)
							? new SqlParameter("@Parity", SqlDbType.NVarChar, 12) { Value = DBNull.Value }
							: new SqlParameter("@Parity", SqlDbType.NVarChar, 12) { Value = this.printerParity};
					sqlCommand.Parameters.Add(parm);
					hasOtherChanges = true;
				}
			}

			if ( hasOtherChanges )
			{
				updateVariables.Add(" CreatedBy = @CreatedBy");
				parm = new SqlParameter("@CreatedBy", SqlDbType.NVarChar, 100) { Value = this.createdBy };
				sqlCommand.Parameters.Add(parm);

				updateVariables.Add(" UpdatedBy = @UpdatedBy");
				parm = new SqlParameter("@UpdatedBy", SqlDbType.NVarChar, 100) { Value = this.updatedBy };
				sqlCommand.Parameters.Add(parm);

				updateVariables.Add(" CreatedDate = @CreatedDate");
				parm = new SqlParameter("@CreatedDate", SqlDbType.DateTimeOffset) { Value = this.createdDate };
				sqlCommand.Parameters.Add(parm);

				updateVariables.Add(" UpdatedDate = @UpdatedDate");
				parm = new SqlParameter("@UpdatedDate", SqlDbType.DateTimeOffset) { Value = this.updatedDate };
				sqlCommand.Parameters.Add(parm);
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
		private void LoadRow(DataRow row)
		{
			if ( row != null )
			{
				this.mobileDeviceProfilePrinterGuid = row.IsNull("MobileDeviceProfilePrinterGuid") ? Guid.Empty : (Guid) row["MobileDeviceProfilePrinterGuid"];
				this.mobileDeviceProfileGuid		= row.IsNull("MobileDeviceProfileGuid") ? Guid.Empty : (Guid) row["mobileDeviceProfileGuid"];
				this.printerBaudRate				= row.IsNull("BaudRate") ? string.Empty : (string) row["BaudRate"];
				this.printerComPort					= row.IsNull("COMPort") ? string.Empty : (string) row["COMPort"];
				this.printerId						= row.IsNull("PrinterId") ? string.Empty : (string) row["PrinterId"];
				this.printerDataBits				= row.IsNull("DataBits") ? "00000000" : (string)row["DataBits"];
				this.printerStopBits				= row.IsNull("StopBits") ? "00000000" : (string) row["StopBits"];
				this.printerUseXonXoff				= row.IsNull("UseXonXoff") ? "00000000" : (string) row["UseXonXoff"];
				this.printerXonChar					= row.IsNull("XonChar") ? "00000000" : (string) row["XonChar"];
				this.printerXoffChar				= row.IsNull("XoffChar") ? "00000000" : (string) row["XoffChar"];
				this.printerBufferSize				= row.IsNull("BufferSize") ? "00000000" : (string) row["BufferSize"];
				this.printerParity					= row.IsNull("Parity") ? string.Empty : (string) row["Parity"];
				this.createdBy						= row.IsNull("CreatedBy") ? string.Empty : (string) row["CreatedBy"];
				this.updatedBy						= row.IsNull("UpdatedBy") ? string.Empty : (string) row["UpdatedBy"];

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

	#region Mobile Device Profile Printer Collection
	/// <summary>
	/// The mobile device profile analog input collection.
	/// </summary>
	[Serializable]
	[CollectionDataContract]
	public class MobileDeviceProfilePrinterCollection : List<MobileDeviceProfilePrinter>
	{
		#region Private data members
		/// <summary>
		/// The delete list.
		/// </summary>
		[DataMember]
		private List<MobileDeviceProfilePrinter> deleteList;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="FMBusinessObjects.DataObjects.MobileDeviceProfilePrinterCollection"/> class.
		/// </summary>
		public MobileDeviceProfilePrinterCollection( )
		{
			this.Reset( );
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the delete list.
		/// </summary>
		public List<MobileDeviceProfilePrinter> DeleteList
		{
			get { return this.deleteList; }
			set { this.deleteList = value; }
		}
		#endregion

		#region Public methods
		/// <summary>
		/// This method initializes the object to its initial state.
		/// </summary>
		public void Reset( )
		{
			this.deleteList = new List<MobileDeviceProfilePrinter>( );
		}

		/// <summary>
		/// This method will remove a given analog input record from the collection
		/// based on the printer GUID.
		/// </summary>
		/// <param name="printer">
		/// The printer.
		/// </param>
		public void RemoveByIdentityGuid(MobileDeviceProfilePrinter printer)
		{
			int itemIndex = 0;

			foreach ( MobileDeviceProfilePrinter item in this )
			{
				if ( item.MobileDeviceProfilePrinterGuid == printer.MobileDeviceProfilePrinterGuid )
				{
					this.deleteList.Add(printer);
					this.RemoveAt(itemIndex);
					return;
				}

				itemIndex++;
			}
		}

		/// <summary>
		/// This method will remove a given printer record from the collection
		/// based on the collection index.
		/// </summary>
		/// <param name="itemIndex">
		/// The item index.
		/// </param>
		public void RemoveByCollectionIndex(int itemIndex)
		{
			if ( (itemIndex >= 0) && (itemIndex < this.Count) )
			{
				this.deleteList.Add(this[itemIndex]);
				this.RemoveAt(itemIndex);
			}
		}

		/// <summary>
		/// This method will load the collection using a dataset. It will separate
		/// each row and create a new datarow with the row. This is for each
		/// printer object.
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

					var printer = new MobileDeviceProfilePrinter( );
					printer.LoadSingle(singleRowDataSet);
					this.Add(printer);
				}
			}
		}
		#endregion
	}
	#endregion
}

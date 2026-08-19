// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomToolbarClass.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the CustomToolbarClass type.
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
	/// Definition of the CustomToolbarClass.
	/// </summary>
	[DataContract]
	[Serializable]
	public class CustomToolbarClass : BaseDataObject
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CustomToolbarClass"/> class.
		/// </summary>
		public CustomToolbarClass()
		{
			this.Reset();
		}

		#region Properties

		/// <summary>
		/// Gets or sets the dispatch configuration guid.
		/// </summary>
		[DataMember]
		public Guid DispatchConfigurationGuid
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the toolbar type.
		/// </summary>
		[DataMember]
		public int ToolbarType
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the toolbar command list.
		/// </summary>
		[DataMember]
		public CustomToolbarCommandCollectionClass ToolbarCommandList
		{
			get;
			set;
		}
		#endregion

		/// <summary>
		/// Resets the Custom Toolbar object to its initial state.
		/// </summary>
		public override sealed void Reset()
		{
			base.Reset();
			this.DispatchConfigurationGuid = Guid.Empty;
			this.ToolbarType = CustomToolbarType.UnknownToolbarType;
			this.ToolbarCommandList = new CustomToolbarCommandCollectionClass();
		}

		/// <summary>
		/// Loads the Custom Toolbar data retrieved from the database.
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

			this._IdentityGuid = DataObject.getValue<Guid>(row["CustomToolbarGuid"], Guid.Empty);
			this.DispatchConfigurationGuid = DataObject.getValue<Guid>(row["DispatchConfigurationGuid"], Guid.Empty);
			this._ID = DataObject.getValue<string>(row["ID"], string.Empty);
			this.ToolbarType = DataObject.getValue<int>(row["LookupCustomToolbarTypeIndex"], CustomToolbarType.UnknownToolbarType);
			this._CreatedDate = DataObject.getValue<DateTimeOffset>(row["CreatedDate"], DateTimeOffset.Now);
			this._CreatedBy = DataObject.getValue<string>(row["CreatedBy"], BaseDataObject.ADMIN);
			this._UpdatedDate = DataObject.getValue<DateTimeOffset>(row["UpdatedDate"], this._CreatedDate);
			this._UpdatedBy = DataObject.getValue<string>(row["UpdatedBy"], BaseDataObject.ADMIN);
		}

		#region paramaterized SQL

		/// <summary>
		/// Generates the dynamic SQL to insert a CustomToolbarClass object into the database
		/// </summary>
		/// <param name="cmd">The SqlCommand object</param>
		public void InsertSql(SqlCommand cmd)
		{
			cmd.CommandText = "INSERT INTO tblCustomToolbar " +
				"(DispatchConfigurationGuid," +
				"ID," +
				"LookupCustomToolbarTypeIndex," +
				"CreatedDate," +
				"CreatedBy," +
				"CustomToolbarGuid" +
				") VALUES (" +
				"@DispatchConfigurationGuid," +
				"@ID," +
				"@ToolbarType," +
				"@CreatedDate," +
				"@CreatedBy," +
				"@CustomToolbarGuid)";

			cmd.Parameters.AddWithValue("@DispatchConfigurationGuid", this.DispatchConfigurationGuid);
			cmd.Parameters.AddWithValue("@ID", this._ID);
			cmd.Parameters.AddWithValue("@ToolbarType", this.ToolbarType);
			cmd.Parameters.AddWithValue("@CreatedDate", this._CreatedDate);
			cmd.Parameters.AddWithValue("@CreatedBy", this._CreatedBy);
			cmd.Parameters.AddWithValue("@CustomToolbarGuid", this._IdentityGuid);
		}

		/// <summary>
		/// Generates the dynamic SQL to update a CustomToolbarClass object in the database
		/// </summary>
		/// <param name="cmd">The SqlCommand object</param>
		public void UpdateSql(SqlCommand cmd)
		{
			cmd.CommandText = "UPDATE tblCustomToolbar SET " +
					"UpdatedDate = @UpdatedDate," +
					"UpdatedBy = @UpdatedBy" +
					" WHERE CustomToolbarGuid = @CustomToolbarGuid";

			cmd.Parameters.AddWithValue("@UpdatedDate", this._UpdatedDate);
			cmd.Parameters.AddWithValue("@UpdatedBy", this._UpdatedBy);
			cmd.Parameters.AddWithValue("@CustomToolbarGuid", this._IdentityGuid);
		}

		/// <summary>
		/// Generates the dynamic SQL to delete a CustomToolbarClass object from the database
		/// </summary>
		/// <param name="cmd">The SqlCommand object</param>
		public void PurgeSql(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM tblCustomToolbar" +
				" WHERE CustomToolbarGuid = @CustomToolbarGuid";

			cmd.Parameters.AddWithValue("@CustomToolbarGuid", this._IdentityGuid);
		}

		/// <summary>
		/// The dynamic SQL SELECT prefix
		/// </summary>
		private const string SelectClause = "SELECT tblCustomToolbar.*";

		/// <summary>
		/// Generates the dynamic SQL to select a CustomToolbarClass object from the database
		/// </summary>
		/// <param name="cmd">The SqlCommand object</param>
		/// <param name="inTransaction">Flag indicating if in a transaction</param>
		public void SelectSql(SqlCommand cmd, bool inTransaction)
		{
			cmd.CommandText = SelectClause +
				" FROM tblCustomToolbar " + BaseDataObject.SQLUpdateLock(inTransaction) +
				" WHERE CustomToolbarGuid = @CustomToolbarGuid";

			cmd.Parameters.AddWithValue("@CustomToolbarGuid", this._IdentityGuid);
		}

		/// <summary>
		/// Generates the dynamic SQL to select a CustomToolbarClass object from the database by ID
		/// </summary>
		/// <param name="cmd">The SqlCommand object</param>
		/// <param name="inTransaction">Flag indicating if in a transaction</param>
		public void SelectByIdsql(SqlCommand cmd, bool inTransaction)
		{
			cmd.CommandText = SelectClause +
				" FROM tblCustomToolbar " + BaseDataObject.SQLUpdateLock(inTransaction) +
				" WHERE DispatchConfigurationGuid = @DispatchConfigurationGuid" +
				" AND ID = @ID";

			cmd.Parameters.AddWithValue("@DispatchConfigurationGuid", this.DispatchConfigurationGuid);
			cmd.Parameters.AddWithValue("@ID", this._ID);
		}

		/// <summary>
		/// Generates the dynamic SQL to select a list of CustomToolbarClass objects from the database
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="cmd">The SqlCommand object</param>
		public void EnumerateSql(SecurityClass security, SqlCommand cmd)
		{
			cmd.CommandText = SelectClause +
				" FROM tblCustomToolbar" +
				" WHERE DispatchConfigurationGuid = @DispatchConfigurationGuid" +
				" ORDER BY ID";

			cmd.Parameters.AddWithValue("@DispatchConfigurationGuid", this.DispatchConfigurationGuid);
		}

		#endregion
	}

	/// <summary>
	/// Definition of the CustomToolbarType.  Used to hold a record from the
	/// toolbar type lookup table associated with the CustomToolbarClass.
	/// </summary>
	[DataContract]
	[Serializable]
	public class CustomToolbarType
	{
		/// <summary>
		/// Indicates unknown toolbar type
		/// </summary>
		public const int UnknownToolbarType = -1;

		/// <summary>
		/// Gets or sets the lookup index.
		/// </summary>
		[DataMember]
		public int LookupIndex
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the id.
		/// </summary>
		[DataMember]
		public string Id
		{
			get;
			set;
		}
	}

	#region Collection Classes
	/// <summary>
	/// Defines a list of CustomToolbarClass objects.
	/// </summary>
	[Serializable]
	[CollectionDataContract]
	[KnownType(typeof(CustomToolbarClass))]
	public class CustomToolbarCollectionClass : List<CustomToolbarClass>
	{
	}

	/// <summary>
	/// Defines a list of CustomToolbarType objects.
	/// </summary>
	[Serializable]
	[CollectionDataContract]
	[KnownType(typeof(CustomToolbarType))]
	public class CustomToolbarTypeList : List<CustomToolbarType>
	{
	}
	#endregion
}

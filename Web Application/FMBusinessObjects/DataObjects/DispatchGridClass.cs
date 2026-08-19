// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DispatchGridClass.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the DispatchGridClass type.
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
	/// Definition of the DispatchGridClass.
	/// </summary>
	[DataContract]
	[Serializable]
	public class DispatchGridClass : BaseDataObject
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DispatchGridClass"/> class.
		/// </summary>
		public DispatchGridClass()
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
		/// Gets or sets the grid type.
		/// </summary>
		[DataMember]
		public int GridType
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the grid column list.
		/// </summary>
		[DataMember]
		public DispatchGridColumnCollectionClass GridColumnList
		{
			get;
			set;
		}
		#endregion

		/// <summary>
		/// Resets the Dispatch Grid object to its initial state.
		/// </summary>
		public override sealed void Reset()
		{
			base.Reset();
			this.DispatchConfigurationGuid = Guid.Empty;
			this.GridType = DispatchGridType.UnknownGridType;
			this.GridColumnList = new DispatchGridColumnCollectionClass();
		}

		/// <summary>
		/// Loads the Dispatch Grid data retrieved from the database.
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

			this._IdentityGuid = DataObject.getValue<Guid>(row["DispatchGridGuid"], Guid.Empty);
			this.DispatchConfigurationGuid = DataObject.getValue<Guid>(row["DispatchConfigurationGuid"], Guid.Empty);
			this.GridType = DataObject.getValue<int>(row["LookupDispatchGridTypeIndex"], DispatchGridType.UnknownGridType);
			this._ID = DataObject.getValue<string>(row["ID"], string.Empty);
			this._CreatedDate = DataObject.getValue<DateTimeOffset>(row["CreatedDate"], DateTimeOffset.Now);
			this._CreatedBy = DataObject.getValue<string>(row["CreatedBy"], BaseDataObject.ADMIN);
			this._UpdatedDate = DataObject.getValue<DateTimeOffset>(row["UpdatedDate"], this._CreatedDate);
			this._UpdatedBy = DataObject.getValue<string>(row["UpdatedBy"], BaseDataObject.ADMIN);
		}

		#region paramaterized SQL

		/// <summary>
		/// Generates the dynamic SQL to insert a DispatchGridClass object into the database
		/// </summary>
		/// <param name="cmd">The SqlCommand object</param>
		public void InsertSql(SqlCommand cmd)
		{
			cmd.CommandText = "INSERT INTO tblDispatchGrid " +
				"(DispatchConfigurationGuid," +
				"LookupDispatchGridTypeIndex," +
				"ID," +
				"CreatedDate," +
				"CreatedBy," +
				"DispatchGridGuid" +
				") VALUES (" +
				"@DispatchConfigurationGuid," +
				"@GridType," +
				"@ID," +
				"@CreatedDate," +
				"@CreatedBy," +
				"@DispatchGridGuid)";

			cmd.Parameters.AddWithValue("@DispatchConfigurationGuid", this.DispatchConfigurationGuid);
			cmd.Parameters.AddWithValue("@GridType", this.GridType);
			cmd.Parameters.AddWithValue("@ID", this._ID);
			cmd.Parameters.AddWithValue("@CreatedDate", this._CreatedDate);
			cmd.Parameters.AddWithValue("@CreatedBy", this._CreatedBy);
			cmd.Parameters.AddWithValue("@DispatchGridGuid", this._IdentityGuid);
		}

		/// <summary>
		/// Generates the dynamic SQL to update a DispatchGridClass object in the database
		/// </summary>
		/// <param name="cmd">The SqlCommand object</param>
		public void UpdateSql(SqlCommand cmd)
		{
			cmd.CommandText = "UPDATE tblDispatchGrid SET " +
					"UpdatedDate = @UpdatedDate," +
					"UpdatedBy = @UpdatedBy" +
					" WHERE DispatchGridGuid = @DispatchGridGuid";

			cmd.Parameters.AddWithValue("@UpdatedDate", this._UpdatedDate);
			cmd.Parameters.AddWithValue("@UpdatedBy", this._UpdatedBy);
			cmd.Parameters.AddWithValue("@DispatchGridGuid", this._IdentityGuid);
		}

		/// <summary>
		/// Generates the dynamic SQL to delete a DispatchGridClass object from the database
		/// </summary>
		/// <param name="cmd">The SqlCommand object</param>
		public void PurgeSql(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM tblDispatchGrid" +
				" WHERE DispatchGridGuid = @DispatchGridGuid";

			cmd.Parameters.AddWithValue("@DispatchGridGuid", this._IdentityGuid);
		}

		/// <summary>
		/// The dynamic SQL SELECT prefix
		/// </summary>
		private const string SelectClause = "SELECT tblDispatchGrid.*";

		/// <summary>
		/// Generates the dynamic SQL to select a DispatchGridClass object from the database
		/// </summary>
		/// <param name="cmd">The SqlCommand object</param>
		/// <param name="inTransaction">Flag indicating if in a transaction</param>
		public void SelectSql(SqlCommand cmd, bool inTransaction)
		{
			cmd.CommandText = SelectClause +
				" FROM tblDispatchGrid " + BaseDataObject.SQLUpdateLock(inTransaction) +
				" WHERE DispatchGridGuid = @DispatchGridGuid";

			cmd.Parameters.AddWithValue("@DispatchGridGuid", this._IdentityGuid);
		}

		/// <summary>
		/// Generates the dynamic SQL to select a DispatchGridClass object from the database by ID
		/// </summary>
		/// <param name="cmd">The SqlCommand object</param>
		/// <param name="inTransaction">Flag indicating if in a transaction</param>
		public void SelectByIdSql(SqlCommand cmd, bool inTransaction)
		{
			cmd.CommandText = SelectClause +
				" FROM tblDispatchGrid " + BaseDataObject.SQLUpdateLock(inTransaction) +
				" WHERE DispatchConfigurationGuid = @DispatchConfigurationGuid" +
				" AND ID = @ID";

			cmd.Parameters.AddWithValue("@DispatchConfigurationGuid", this.DispatchConfigurationGuid);
			cmd.Parameters.AddWithValue("@ID", this._ID);
		}

		/// <summary>
		/// Generates the dynamic SQL to select a list of DispatchGridClass objects from the database
		/// </summary>
		/// <param name="security">The security object</param>
		/// <param name="cmd">The SqlCommand object</param>
		public void EnumerateSql(SecurityClass security, SqlCommand cmd)
		{
			cmd.CommandText = SelectClause +
				" FROM tblDispatchGrid" +
				" WHERE DispatchConfigurationGuid = @DispatchConfigurationGuid" +
				" ORDER BY ID";

			cmd.Parameters.AddWithValue("@DispatchConfigurationGuid", this.DispatchConfigurationGuid);
		}

		#endregion
	}

	/// <summary>
	/// Definition of the DispatchGridType.  Used to hold a record from the
	/// grid type lookup table associated with the DispatchGridClass.
	/// </summary>
	[DataContract]
	[Serializable]
	public class DispatchGridType
	{
		/// <summary>
		/// Indicates unknown grid type
		/// </summary>
		public const int UnknownGridType = -1;

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
	/// Defines a list of DispatchGridClass objects.
	/// </summary>
	[Serializable]
	[CollectionDataContract]
	[KnownType(typeof(DispatchGridClass))]
	public class DispatchGridCollectionClass : List<DispatchGridClass>
	{
	}

	/// <summary>
	/// Defines a list of DispatchGridType objects.
	/// </summary>
	[Serializable]
	[CollectionDataContract]
	[KnownType(typeof(DispatchGridType))]
	public class DispatchGridTypeList : List<DispatchGridType>
	{
	}
	#endregion
}

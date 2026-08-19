// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DispatchGridColumnClass.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the DispatchGridColumnClass type.
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
	/// Definition of the DispatchGridColumnClass.
	/// </summary>
	[DataContract]
	[Serializable]
	public class DispatchGridColumnClass : BaseDataObject
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DispatchGridColumnClass"/> class.
		/// </summary>
		public DispatchGridColumnClass()
		{
			this.Reset();
		}

		#region Properties

		/// <summary>
		/// Gets or sets the grid column type.
		/// </summary>
		[DataMember]
		public int GridColumnType
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the dispatch grid guid.
		/// </summary>
		[DataMember]
		public Guid DispatchGridGuid
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the user guid.
		/// </summary>
		[DataMember]
		public Guid UserGuid
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the dispatch grid id.
		/// </summary>
		[DataMember]
		public string DispatchGridId
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the transaction alias name associated with the user data field.
		/// </summary>
		[DataMember]
		public string AliasName
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the user data field transaction alias guid.
		/// </summary>
		[DataMember]
		public Guid UserDataFieldTransactionAliasGuid
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the user data field transaction alias line item guid.
		/// </summary>
		[DataMember]
		public Guid UserDataFieldTransactionAliasLineItemGuid
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the user data number.
		/// </summary>
		[DataMember]
		public int UserDataNumber
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the column order.
		/// </summary>
		[DataMember]
		public int ColumnOrder
		{
			get;
			set;
		}
		#endregion

		/// <summary>
		/// Resets the object to its initial state.
		/// </summary>
		public override sealed void Reset()
		{
			base.Reset();

			this.GridColumnType = DispatchGridColumnType.UnknownColumnType;
			this.DispatchGridId = string.Empty;
			this.DispatchGridGuid = Guid.Empty;
			this.UserGuid = Guid.Empty;
			this.AliasName = string.Empty;
			this.UserDataFieldTransactionAliasGuid = Guid.Empty;
			this.UserDataFieldTransactionAliasLineItemGuid = Guid.Empty;
			this.UserDataNumber = 0;
			this.ColumnOrder = 0;
		}

		/// <summary>
		/// Loads the Dispatch Grid Column data retrieved from the database.
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

			this._IdentityGuid = DataObject.getValue<Guid>(row["DispatchGridColumnGuid"], Guid.Empty);
			this.DispatchGridGuid = DataObject.getValue<Guid>(row["DispatchGridGuid"], Guid.Empty);
			this.UserGuid = DataObject.getValue<Guid>(row["UserGuid"], Guid.Empty);
			this.DispatchGridId = DataObject.getValue<string>(row["DispatchGridID"], string.Empty);
			this.GridColumnType = DataObject.getValue<int>(row["LookupDispatchGridColumnTypeIndex"], DispatchGridColumnType.UnknownColumnType);
			this._ID = DataObject.getValue<string>(row["ID"], string.Empty);
			this.AliasName = DataObject.getValue<string>(row["AliasName"], string.Empty);
			this.UserDataFieldTransactionAliasGuid = DataObject.getValue<Guid>(row["UserDataFieldTransactionAliasGuid"], Guid.Empty);
			this.UserDataFieldTransactionAliasLineItemGuid = DataObject.getValue<Guid>(row["UserDataFieldTransactionAliasLineItemGuid"], Guid.Empty);
			this.UserDataNumber = DataObject.getValue<int>(row["UserDataNumber"], 0);
			this.ColumnOrder = DataObject.getValue<int>(row["ColumnOrder"], 0);
			this._CreatedDate = DataObject.getValue<DateTimeOffset>(row["CreatedDate"], DateTimeOffset.Now);
			this._CreatedBy = DataObject.getValue<string>(row["CreatedBy"], BaseDataObject.ADMIN);
			this._UpdatedDate = DataObject.getValue<DateTimeOffset>(row["UpdatedDate"], this._CreatedDate);
			this._UpdatedBy = DataObject.getValue<string>(row["UpdatedBy"], BaseDataObject.ADMIN);
		}

		#region paramaterized SQL

		/// <summary>
		/// Generates the dynamic SQL to insert a DispatchGridColumnClass object into the database
		/// </summary>
		/// <param name="cmd">The SqlCommand object</param>
		public void InsertSql(SqlCommand cmd)
		{
			cmd.CommandText = "INSERT INTO tblDispatchGridColumn " +
				"(DispatchGridGuid," +
				"UserGuid," +
				"DispatchGridID," +
				"LookupDispatchGridColumnTypeIndex," +
				"ID," +
				"AliasName," +
				"UserDataFieldTransactionAliasGuid," +
				"UserDataFieldTransactionAliasLineItemGuid," +
				"UserDataNumber," +
				"ColumnOrder," +
				"CreatedDate," +
				"CreatedBy," +
				"DispatchGridColumnGuid" +
				") VALUES (" +
				"@DispatchGridGuid," +
				"@UserGuid," +
				"@DispatchGridID," +
				"@GridColumnType," +
				"@ID," +
				"@AliasName," +
				"@UserDataFieldTransactionAliasGuid," +
				"@UserDataFieldTransactionAliasLineItemGuid," +
				"@UserDataNumber," +
				"@ColumnOrder," +
				"@CreatedDate," +
				"@CreatedBy," +
				"@DispatchGridColumnGuid)";

			cmd.Parameters.AddWithValue("@DispatchGridGuid", this.DispatchGridGuid);
			cmd.Parameters.AddWithValue("@UserGuid", this.UserGuid);
			cmd.Parameters.AddWithValue("@DispatchGridID", this.DispatchGridId);
			cmd.Parameters.AddWithValue("@GridColumnType", this.GridColumnType);
			cmd.Parameters.AddWithValue("@ID", this._ID);
			cmd.Parameters.AddWithValue("@AliasName", this.AliasName);
			if (this.UserDataFieldTransactionAliasGuid == Guid.Empty)
			{
				cmd.Parameters.AddWithValue("@UserDataFieldTransactionAliasGuid", DBNull.Value);
			}
			else
			{
				cmd.Parameters.AddWithValue("@UserDataFieldTransactionAliasGuid", this.UserDataFieldTransactionAliasGuid);
			}
			if (this.UserDataFieldTransactionAliasLineItemGuid == Guid.Empty)
			{
				cmd.Parameters.AddWithValue("@UserDataFieldTransactionAliasLineItemGuid", DBNull.Value);
			}
			else
			{
				cmd.Parameters.AddWithValue("@UserDataFieldTransactionAliasLineItemGuid", this.UserDataFieldTransactionAliasLineItemGuid);
			}
			cmd.Parameters.AddWithValue("@UserDataNumber", this.UserDataNumber);
			cmd.Parameters.AddWithValue("@ColumnOrder", this.ColumnOrder);
			cmd.Parameters.AddWithValue("@CreatedDate", this._CreatedDate);
			cmd.Parameters.AddWithValue("@CreatedBy", this._CreatedBy);
			cmd.Parameters.AddWithValue("@DispatchGridColumnGuid", this._IdentityGuid);
		}

		/// <summary>
		/// Generates the dynamic SQL to update a DispatchGridColumnClass object in the database
		/// </summary>
		/// <param name="cmd">The SqlCommand object</param>
		public void UpdateSql(SqlCommand cmd)
		{
			cmd.CommandText = "UPDATE tblDispatchGridColumn SET " +
				"ColumnOrder = @ColumnOrder," +
				"UpdatedDate = @UpdatedDate," +
				"UpdatedBy = @UpdatedBy" +
				" WHERE DispatchGridColumnGuid = @DispatchGridColumnGuid";

			cmd.Parameters.AddWithValue("@ColumnOrder", this.ColumnOrder);
			cmd.Parameters.AddWithValue("@UpdatedDate", this._UpdatedDate);
			cmd.Parameters.AddWithValue("@UpdatedBy", this._UpdatedBy);
			cmd.Parameters.AddWithValue("@DispatchGridColumnGuid", this._IdentityGuid);
		}

		/// <summary>
		/// Generates the dynamic SQL to delete a DispatchGridColumnClass object from the database
		/// </summary>
		/// <param name="cmd">The SqlCommand object</param>
		public void PurgeSql(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM tblDispatchGridColumn WHERE DispatchGridColumnGuid = @DispatchGridColumnGuid";
			cmd.Parameters.AddWithValue("@DispatchGridColumnGuid", this._IdentityGuid);
		}


		/// <summary>
		/// Generates the dynamic SQL to delete a DispatchGridColumnClass object from the database by user
		/// </summary>
		/// <param name="cmd">The SqlCommand object</param>
		public void PurgeSqlByUser(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM tblDispatchGridColumn WHERE UserGuid = @UserGuid";
			cmd.Parameters.AddWithValue("@UserGuid", this.UserGuid);
		}

		/// <summary>
		/// Generates the dynamic SQL to select a DispatchGridColumnClass object from the database
		/// </summary>
		/// <param name="cmd">The SqlCommand object</param>
		/// <param name="inTransaction">Flag indicating if in a transaction</param>
		public void SelectSql(SqlCommand cmd, bool inTransaction)
		{
			cmd.CommandText = "SELECT tblDispatchGridColumn.*" +
				" FROM tblDispatchGridColumn " + BaseDataObject.SQLUpdateLock(inTransaction) +
				" WHERE DispatchGridColumnGuid = @DispatchGridColumnGuid";
			cmd.Parameters.AddWithValue("@DispatchGridColumnGuid", this._IdentityGuid);
		}

		/// <summary>
		/// Generates the dynamic SQL to select a list of DispatchGridColumnClass objects from the database
		/// </summary>
		/// <param name="cmd">The SqlCommand object</param>
		/// <param name="inTransaction">Flag indicating if in a transaction</param>
		public void EnumerateSql(SqlCommand cmd, bool inTransaction)
		{
			cmd.CommandText = "SELECT tblDispatchGridColumn.*" +
				" FROM tblDispatchGridColumn " + BaseDataObject.SQLUpdateLock(inTransaction) +
				" WHERE DispatchGridGuid = @DispatchGridGuid" +
				"	AND UserGuid = @UserGuid" +
				" ORDER BY ColumnOrder";

			cmd.Parameters.AddWithValue("@DispatchGridGuid", this.DispatchGridGuid);
			cmd.Parameters.AddWithValue("@UserGuid", this.UserGuid);
		}

		#endregion
	}

	/// <summary>
	/// Definition of the DispatchGridColumnType class.  Used to hold a record from the dispatch
	/// grid column type lookup table associated with the DispatchGridColumnClass.
	/// </summary>
	[DataContract]
	[Serializable]
	public class DispatchGridColumnType
	{
		/// <summary>
		/// Indicates unknown column type
		/// </summary>
		public const int UnknownColumnType = -1;

		/// <summary>
		/// Indicates transaction alias user data column type
		/// </summary>
		public const int TransactionAliasUserDataColumnType = 1;

		/// <summary>
		/// Indicates transaction alias line item user data column type
		/// </summary>
		public const int TransactionAliasLineItemUserDataColumnType = 2;

		/// <summary>
		/// Indicates the default column width
		/// </summary>
		public const int DefaultColumnWidth = 90;

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

		/// <summary>
		/// Gets or sets the display name.
		/// </summary>
		[DataMember]
		public string DisplayName
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the data field.
		/// </summary>
		[DataMember]
		public string DataField
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the width.
		/// </summary>
		[DataMember]
		public int Width
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the default column order.
		/// </summary>
		[DataMember]
		public int DefaultColumnOrder
		{
			get;
			set;
		}
	}

	#region Collection Classes
	/// <summary>
	/// Defines a list of DispatchGridColumnClass objects.
	/// </summary>
	[Serializable]
	[CollectionDataContract]
	[KnownType(typeof(DispatchGridColumnClass))]
	public class DispatchGridColumnCollectionClass : List<DispatchGridColumnClass>
	{
	}

	/// <summary>
	/// Defines a list of DispatchGridColumnType objects.
	/// </summary>
	[Serializable]
	[CollectionDataContract]
	[KnownType(typeof(DispatchGridColumnType))]
	public class DispatchGridColumnTypeList : List<DispatchGridColumnType>
	{
	}
	#endregion
}

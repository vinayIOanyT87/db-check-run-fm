// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomToolbarCommandClass.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the CustomToolbarCommandClass type.
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
	/// Definition of the CustomToolbarCommandClass.
	/// </summary>
	[DataContract]
	[Serializable]
	public class CustomToolbarCommandClass : BaseDataObject
	{
		/// <summary>
		/// String appended to command name indicating that command is a transaction alias
		/// </summary>
		public const string TransactionAliasDesignator = " Transaction Alias";

		/// <summary>
		/// Initializes a new instance of the <see cref="CustomToolbarCommandClass"/> class. 
		/// </summary>
		public CustomToolbarCommandClass()
		{
			this.Reset();
		}

		#region Properties

		/// <summary>
		/// Gets or sets the toolbar command type.
		/// </summary>
		[DataMember]
		public int ToolbarCommandType
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the custom toolbar id.
		/// </summary>
		[DataMember]
		public string CustomToolbarId
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the custom toolbar guid.
		/// </summary>
		[DataMember]
		public Guid CustomToolbarGuid
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the transaction alias guid.
		/// </summary>
		[DataMember]
		public Guid TransactionAliasGuid
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
		/// Resets the Custom Toolbar Command object to its initial state.
		/// </summary>
		public override sealed void Reset()
		{
			base.Reset();

			this.ToolbarCommandType = CustomToolbarCommandType.UnknownCommandType;
			this.CustomToolbarId = string.Empty;
			this.CustomToolbarGuid = Guid.Empty;
			this.TransactionAliasGuid = Guid.Empty;
			this.ColumnOrder = 0;
		}

		/// <summary>
		/// Loads the Custom Toolbar Command data retrieved from the database.
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

			this._IdentityGuid = DataObject.getValue<Guid>(row["CustomToolbarCommandGuid"], Guid.Empty);
			this.CustomToolbarGuid = DataObject.getValue<Guid>(row["CustomToolbarGuid"], Guid.Empty);
			this.CustomToolbarId = DataObject.getValue<string>(row["CustomToolbarID"], string.Empty);
			this.ToolbarCommandType = DataObject.getValue<int>(row["LookupCustomToolbarCommandTypeIndex"], CustomToolbarCommandType.UnknownCommandType);
			this.TransactionAliasGuid = DataObject.getValue<Guid>(row["TransactionAliasGuid"], Guid.Empty);
			this._ID = DataObject.getValue<string>(row["ID"], string.Empty);
			this.ColumnOrder = DataObject.getValue<int>(row["ColumnOrder"], 0);
			this._CreatedDate = DataObject.getValue<DateTimeOffset>(row["CreatedDate"], DateTimeOffset.Now);
			this._CreatedBy = DataObject.getValue<string>(row["CreatedBy"], BaseDataObject.ADMIN);
			this._UpdatedDate = DataObject.getValue<DateTimeOffset>(row["UpdatedDate"], this._CreatedDate);
			this._UpdatedBy = DataObject.getValue<string>(row["UpdatedBy"], BaseDataObject.ADMIN);
		}

		#region paramaterized SQL

		/// <summary>
		/// Generates the dynamic SQL to insert a CustomToolbarCommandClass object into the database
		/// </summary>
		/// <param name="cmd">The SqlCommand object</param>
		public void InsertSql(SqlCommand cmd)
		{
			cmd.CommandText = "INSERT INTO tblCustomToolbarCommand " +
				"(CustomToolbarGuid," +
				"CustomToolbarID," +
				"LookupCustomToolbarCommandTypeIndex," +
				"TransactionAliasGuid," +
				"ID," +
				"ColumnOrder," +
				"CreatedDate," +
				"CreatedBy," +
				"CustomToolbarCommandGuid" +
				") VALUES (" +
				"@CustomToolbarGuid," +
				"@CustomToolbarID," +
				"@ToolbarCommandType," +
				"@TransactionAliasGuid," +
				"@ID," +
				"@ColumnOrder," +
				"@CreatedDate," +
				"@CreatedBy," +
				"@CustomToolbarCommandGuid)";

			cmd.Parameters.AddWithValue("@CustomToolbarGuid", this.CustomToolbarGuid);
			cmd.Parameters.AddWithValue("@CustomToolbarID", this.CustomToolbarId);
			cmd.Parameters.AddWithValue("@ToolbarCommandType", this.ToolbarCommandType);
			if (this.TransactionAliasGuid == Guid.Empty)
			{
				cmd.Parameters.AddWithValue("@TransactionAliasGuid", DBNull.Value);
			}
			else
			{
				cmd.Parameters.AddWithValue("@TransactionAliasGuid", this.TransactionAliasGuid);
			}

			cmd.Parameters.AddWithValue("@ID", this._ID);
			cmd.Parameters.AddWithValue("@ColumnOrder", this.ColumnOrder);
			cmd.Parameters.AddWithValue("@CreatedDate", this._CreatedDate);
			cmd.Parameters.AddWithValue("@CreatedBy", this._CreatedBy);
			cmd.Parameters.AddWithValue("@CustomToolbarCommandGuid", this._IdentityGuid);
		}

		/// <summary>
		/// Generates the dynamic SQL to update a CustomToolbarCommandClass object in the database
		/// </summary>
		/// <param name="cmd">The SqlCommand object</param>
		public void UpdateSql(SqlCommand cmd)
		{
			cmd.CommandText = "UPDATE tblCustomToolbarCommand SET " +
				"ColumnOrder = @ColumnOrder," +
				"UpdatedDate = @UpdatedDate," +
				"UpdatedBy = @UpdatedBy" +
				" WHERE CustomToolbarCommandGuid = @CustomToolbarCommandGuid";

			cmd.Parameters.AddWithValue("@ColumnOrder", this.ColumnOrder);
			cmd.Parameters.AddWithValue("@UpdatedDate", this._UpdatedDate);
			cmd.Parameters.AddWithValue("@UpdatedBy", this._UpdatedBy);
			cmd.Parameters.AddWithValue("@CustomToolbarCommandGuid", this._IdentityGuid);
		}

		/// <summary>
		/// Generates the dynamic SQL to delete a CustomToolbarCommandClass object from the database
		/// </summary>
		/// <param name="cmd">The SqlCommand object</param>
		public void PurgeSql(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM tblCustomToolbarCommand WHERE CustomToolbarCommandGuid = @CustomToolbarCommandGuid";
			cmd.Parameters.AddWithValue("@CustomToolbarCommandGuid", this._IdentityGuid);
		}

		/// <summary>
		/// The dynamic SQL SELECT prefix
		/// </summary>
		private const string SelectClause = "SELECT tblCustomToolbarCommand.*";

		/// <summary>
		/// Generates the dynamic SQL to select a CustomToolbarCommandClass object from the database
		/// </summary>
		/// <param name="cmd">The SqlCommand object</param>
		/// <param name="inTransaction">Flag indicating if in a transaction</param>
		public void SelectSql(SqlCommand cmd, bool inTransaction)
		{
			cmd.CommandText = SelectClause +
				" FROM tblCustomToolbarCommand " + BaseDataObject.SQLUpdateLock(inTransaction) +
				" WHERE CustomToolbarCommandGuid = @CustomToolbarCommandGuid";
			cmd.Parameters.AddWithValue("@CustomToolbarCommandGuid", this._IdentityGuid);
		}

		/// <summary>
		/// Generates the dynamic SQL to select a list of CustomToolbarCommandClass objects from the database
		/// </summary>
		/// <param name="cmd">The SqlCommand object</param>
		/// <param name="inTransaction">Flag indicating if in a transaction</param>
		public void EnumerateSql(SqlCommand cmd, bool inTransaction)
		{
			cmd.CommandText = SelectClause +
				" FROM tblCustomToolbarCommand " + BaseDataObject.SQLUpdateLock(inTransaction) +
				" WHERE CustomToolbarGuid = @CustomToolbarGuid" +
				" ORDER BY ColumnOrder";

			cmd.Parameters.AddWithValue("@CustomToolbarGuid", this.CustomToolbarGuid);
		}

		#endregion
	}

	/// <summary>
	/// Definition of the CustomToolbarCommandType.  Used to hold a record from the toolbar
	/// command type lookup table associated with the CustomToolbarCommandClass.
	/// </summary>
	[DataContract]
	[Serializable]
	public class CustomToolbarCommandType
	{
		/// <summary>
		/// Indicates unknown toolbar command type
		/// </summary>
		public const int UnknownCommandType = -1;

		/// <summary>
		/// Indicates transaction alias command type
		/// </summary>
		public const int TransactionAliasCommandType = 1;

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

		[DataMember]
		public bool IsDefault
		{
			get; 
			set; 
		}

		[DataMember]
		public int? DefaultOrder { get; set; }

		[DataMember]
		public string ImageSource
		{
			get;
			set;
		}
	}

	#region Collection Classes
	/// <summary>
	/// Defines a list of CustomToolbarCommandClass objects.
	/// </summary>
	[Serializable]
	[CollectionDataContract]
	[KnownType(typeof(CustomToolbarCommandClass))]
	public class CustomToolbarCommandCollectionClass : List<CustomToolbarCommandClass>
	{
	}

	/// <summary>
	/// Defines a list of CustomToolbarCommandType objects.
	/// </summary>
	[Serializable]
	[CollectionDataContract]
	[KnownType(typeof(CustomToolbarCommandType))]
	public class CustomToolbarCommandTypeList : List<CustomToolbarCommandType>
	{
	}
	#endregion
}

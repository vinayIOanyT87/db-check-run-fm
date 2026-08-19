namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Runtime.Serialization;
	using System.Data.SqlClient;

	#region Maintenance Reason Collection class.
	[Serializable]
	[CollectionDataContract]
	public class MaintenanceReasonCollectionClass : List<MaintenanceReasonClass>
	{
	}
	#endregion

	[Serializable]
	[DataContract]
	public class MaintenanceReasonClass : BaseDataObject, IComparable
	{
		#region Data bound properties
		[DataMember] private string description;

		public string Description
		{
			get { return this.description; }
			set {
				this.SetString("Description", 50, value, ref this.description); }
		}

		public static readonly string QUALITY_TAG_DESCRIPTION = "Triggered by Quality Tag assignment.";
		public static readonly string QUALITY_TAG_ASSIGNMENT = "QUALITY_TAG_ASSIGNEMENT";
		#endregion

		#region Interface implementations
		int IComparable.CompareTo(object obj)
		{
			var maintenanceReason = obj as MaintenanceReasonClass;

			if (maintenanceReason == null)
			{
				throw new Exception("Invalid MaintenanceReason");
			}

			return this.ID.CompareTo(maintenanceReason.ID);
		}

		public override ENTITY_TYPE EntityType
		{
			get { return ENTITY_TYPE.MAINTENANCE_REASON; }
			set { }
		}

		public override ENTITY_TYPE ParentEntityType
		{
			get { return ENTITY_TYPE.NONE; }
		}
		#endregion


		#region Ctors and initialization
		/// <summary>
		/// This is the default constuctor for the maintenance reason class.
		/// </summary>
		public MaintenanceReasonClass()
		{
			this.Init();
		}

		public override void Reset()
		{
			this.Init();
		}

		/// <summary>
		/// This method will load the data object with data.
		/// </summary>
		/// <param name="o">Data from DB.</param>
		public override void Load(Object o)
		{
			this.Init();

			var obj = o as DataSet;

			if (obj != null)
			{
				var set = obj;
				DataTable table = set.Tables[0];

				if (table.Rows.Count == 0)
				{
					return;
				}

				DataRow row = table.Rows[0];

				this.IdentityGuid	= DataObject.getValue<Guid>(row["MaintenanceReasonGuid"], Guid.Empty);
				this.ID				= DataObject.getValue<string>(row["ID"], string.Empty);
				this.Description	= DataObject.getValue<string>(row["Description"], string.Empty);
				this.SiteGuid		= DataObject.getValue<Guid>(row["SiteGuid"], Guid.Empty);
			}
		}

		/// <summary>
		/// Calls the base class to reset.
		/// </summary>
		private void Init()
		{
			base.Reset();
		}
		#endregion

		#region SQL DML statements
		public string EnumerateSQL(SecurityClass security, bool inTransaction)
		{
			string sql = ""
			             + "SELECT "
			             + "   MaintenanceReasonGuid, "
						 + "   SiteGuid, "
			             + "   ID, "
			             + "   Description, "
			             + "   CreatedDate, "
			             + "   CreatedBy ,"
			             + "   UpdatedDate, "
			             + "   UpdatedBy "
			             + " FROM dbo.tblMaintenanceReasons " + SQLUpdateLock(inTransaction)
			             + " ORDER BY ID";

			return sql;
		}
		#endregion

		#region SQL Paramaterized Query Text
		public void InsertSQL(SqlCommand cmd)
		{
			cmd.CommandText = "INSERT INTO dbo.tblMaintenanceReasons ( " +
				"ID, " +
				"Description, " +
				"CreatedDate, " +
				"CreatedBy, " +
				"UpdatedDate, " +
				"UpdatedBy, " +
				"MaintenanceReasonGuid, " +
				"SiteGuid " +
				") VALUES ( " +
				"@ID, " +
				"@Description, " +
				"@CreatedDate, " +
				"@CreatedBy, " +
				"@UpdatedDate, " +
				"@UpdatedBy, " +
				"@MaintenanceReasonGuid, " +
				"@SiteGuid)";

			cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 30);
			cmd.Parameters.Add("@Description", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@MaintenanceReasonGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@ID"].Value = this.ID;
			cmd.Parameters["@Description"].Value			= this.Description;
			cmd.Parameters["@CreatedDate"].Value			= this.CreatedDate;
			cmd.Parameters["@CreatedBy"].Value				= this.CreatedBy;
			cmd.Parameters["@UpdatedDate"].Value			= this.UpdatedDate;
			cmd.Parameters["@UpdatedBy"].Value				= this.UpdatedBy;
			cmd.Parameters["@MaintenanceReasonGuid"].Value	= this.IdentityGuid;
			cmd.Parameters["@SiteGuid"].Value				= this.SiteGuid;
		}

		public void UpdateSQL(SqlCommand cmd)
		{
			cmd.CommandText = "UPDATE dbo.tblMaintenanceReasons SET " +
				"ID = @ID, " +
				"SiteGuid = @SiteGuid, " +
				"Description = @Description, " +
				"UpdatedDate = @UpdatedDate, " +
				"UpdatedBy   = @UpdatedBy " +
				"WHERE MaintenanceReasonGuid = @MaintenanceReasonGuid";

			cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 30);
			cmd.Parameters.Add("@Description", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@MaintenanceReasonGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@ID"].Value						= this.ID;
			cmd.Parameters["@Description"].Value			= this.Description;
			cmd.Parameters["@UpdatedDate"].Value			= this.UpdatedDate;
			cmd.Parameters["@UpdatedBy"].Value				= this.UpdatedBy;
			cmd.Parameters["@MaintenanceReasonGuid"].Value	= this.IdentityGuid;
			cmd.Parameters["@SiteGuid"].Value				= this.SiteGuid;
		}

		public void PurgeSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM dbo.tblMaintenanceReasons WHERE MaintenanceReasonGuid = @MaintenanceReasonGuid";
			cmd.Parameters.Add("@MaintenanceReasonGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@MaintenanceReasonGuid"].Value = this.IdentityGuid;
		}

		public void SelectSQL(SqlCommand cmd, bool bInTransaction)
		{
			cmd.CommandText = "SELECT MaintenanceReasonGuid, " +
				"ID, " +
				"SiteGuid, " +
				"Description, " +
				"CreatedDate, " +
				"CreatedBy, " +
				"UpdatedDate, " +
				"UpdatedBy " +
				"FROM dbo.tblMaintenanceReasons " + SQLUpdateLock(bInTransaction) + " " +
				"WHERE MaintenanceReasonGuid = @MaintenanceReasonGuid";

			cmd.Parameters.Add("@MaintenanceReasonGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@MaintenanceReasonGuid"].Value = this.IdentityGuid;
		}

		public void SelectIDSQL(SecurityClass security, SqlCommand cmd, bool inTransaction)
		{
			cmd.CommandText = "SELECT " +
				"MaintenanceReasonGuid, " +
				"SiteGuid, " +
				"ID, " +
				"Description, " +
				"CreatedDate, " +
				"CreatedBy, " +
				"UpdatedDate, " +
				"UpdatedBy " +
				"FROM dbo.tblMaintenanceReasons " + SQLUpdateLock(inTransaction) + " " +
				"WHERE ID = @ID" +
				" AND " + this.AppendSiteWhereClause(cmd, security, "tblMaintenanceReasons", "MaintenanceReasonGuid");

			cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 30);
			cmd.Parameters["@ID"].Value = this.ID;
		}

		public void EnumerateSQL(SqlCommand cmd, bool inTransaction)
		{
			cmd.CommandText = "SELECT " +
				"MaintenanceReasonGuid, " +
				"SiteGuid, " +
				"ID, " +
				"Description, " +
				"CreatedDate, " +
				"CreatedBy ," +
				"UpdatedDate, " +
				"UpdatedBy " +
				"FROM dbo.tblMaintenanceReasons " + SQLUpdateLock(inTransaction) + " " +
				"ORDER BY ID";
		}

		public void EnumerateBySiteSQL(SqlCommand cmd, bool inTransaction)
		{
			cmd.CommandText = "SELECT " 
				+ "m.MaintenanceReasonGuid, "
				+ "m.SiteGuid, "
				+ "m.ID, "
				+ "m.Description, "
				+ "m.CreatedDate, "
				+ "m.CreatedBy ,"
				+ "m.UpdatedDate, "
				+ "m.UpdatedBy " 
				+ "FROM dbo.tblMaintenanceReasons m " + SQLUpdateLock(inTransaction) + " "
				+ "JOIN [map].tblEntityMaintenanceReasonToSite e ON m.MaintenanceReasonGuid = e.MaintenanceReasonGuid "
				+ "WHERE e.SiteGuid = @SiteGuid "
				+ "ORDER BY m.ID";

			var parm = new SqlParameter("@SiteGuid", SqlDbType.UniqueIdentifier) { Value = this.SiteGuid };
			cmd.Parameters.Add(parm);
		}
		#endregion
	}
}
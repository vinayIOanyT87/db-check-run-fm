namespace FMBusinessObjects.DataObjects
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;

    using FMCore;

    public enum QUALITY_SEVERITY_LEVELS
	{
		DANGER = 0,
		WARNING = 1,
		CAUTION = 2,
	};

	/// <summary>
	/// Summary description for QualityTagCollectionClass.
	/// </summary>
   [Serializable]
   [CollectionDataContract]
	public class QualityTagCollectionClass : List<QualityTagClass>
	{
		public void RemoveByGuid(QualityTagClass qualityTag)
		{
			int index = 0;

			foreach (QualityTagClass item in this)
			{
				if (item.IdentityGuid == qualityTag.IdentityGuid)
				{
					this.RemoveAt(index);
					return;
				}

				index++;
			}
		}
	}


	/// <summary>
	/// Summary description for QualityTagClass.
	/// </summary>
   [Serializable]
   [DataContract]
	public class QualityTagClass : BaseDataObject
	{
		#region Public Constants
		public const string ENTITY_TYPE_ID = "Quality Tag";
		#endregion

		#region Data members

		// Fields.
		[DataMember] protected QUALITY_SEVERITY_LEVELS _Severity;
		[DataMember] protected bool _Active;
		[DataMember] public bool AuditLog = false;
		#endregion

		public QualityTagClass()
		{
			Reset();
		}

		public QUALITY_SEVERITY_LEVELS Severity
		{
			get { return _Severity; }
			set { _Severity = value; }
		}

		public bool Active
		{
			get { return _Active; }
			set { _Active = value; }
		}

		[XmlIgnoreAttribute]
		public override ENTITY_TYPE EntityType
		{
			get { return ENTITY_TYPE.QUALITY_TAG; }
		}

		[XmlIgnoreAttribute]
		public override ENTITY_TYPE ParentEntityType
		{
			get { return ENTITY_TYPE.NONE; }
		}

		public override void Reset()
		{
			base.Reset();
			_Severity = QUALITY_SEVERITY_LEVELS.CAUTION;
		}

		public void Load(DataSet Set)
		{
			if (Set == null)
			{
				throw new ArgumentNullException("set");
			}

			Reset();

			DataTable Table = Set.Tables[0];

			if (Table.Rows.Count == 0)
			{
				return;
			}

			DataRow Row = Table.Rows[0];

			IdentityGuid = DataObject.getValue<Guid>(Row["QualityTagGuid"], Guid.Empty);
			SiteGuid = DataObject.getValue<Guid>(Row["SiteGuid"], Guid.Empty);
			ID = DataObject.getValue<string>(Row["Name"], "");
			Severity = (QUALITY_SEVERITY_LEVELS)DataObject.getValue<short>(Row["Severity"], (short)QUALITY_SEVERITY_LEVELS.CAUTION);
			Active = DataObject.getValue<bool>(Row["Active"], true);
			CreatedDate = DataObject.getValue<DateTimeOffset>(Row["CreatedDate"], DateTimeOffset.Now);
			CreatedBy = DataObject.getValue<string>(Row["CreatedBy"], ADMIN);
			UpdatedDate = DataObject.getValue<DateTimeOffset>(Row["UpdatedDate"], CreatedDate);
			UpdatedBy = DataObject.getValue<string>(Row["UpdatedBy"], ADMIN);
		}

		public void InsertSQL(SqlCommand cmd)
		{
			cmd.CommandText = "INSERT INTO tblQualityTags (" +
			"Name," +
			"SiteGuid," +
			"Severity," +
			"Active," +
			"CreatedDate," +
			"CreatedBy," +
			"UpdatedDate," +
			"UpdatedBy," +
			"QualityTagGuid" +
			") VALUES (" +
			"@Name," +
			"@SiteGuid," +
			"@Severity," +
			"@Active," +
			"@CreatedDate," +
			"@CreatedBy," +
			"@UpdatedDate," +
			"@UpdatedBy," +
			"@QualityTagGuid)";

			cmd.Parameters.AddWithValue("@Name", ID);
			cmd.Parameters.AddWithValue("@SiteGuid", SiteGuid);
			cmd.Parameters.AddWithValue("@Severity", (int)_Severity);
			cmd.Parameters.AddWithValue("@Active", (_Active ? 1 : 0));
			cmd.Parameters.AddWithValue("@CreatedDate", CreatedDate);
			cmd.Parameters.AddWithValue("@CreatedBy", CreatedBy);
			cmd.Parameters.AddWithValue("@UpdatedDate", UpdatedDate);
			cmd.Parameters.AddWithValue("@UpdatedBy", UpdatedBy);
			cmd.Parameters.AddWithValue("@QualityTagGuid", _IdentityGuid);
		}

		public void UpdateSQL(SqlCommand cmd)
		{
			cmd.CommandText = "UPDATE tblQualityTags SET " +
				"Name = @Name, " +
				"SiteGuid = @SiteGuid, " +
				"Severity = @Severity, " +
				"Active = @Active, " +
				"UpdatedDate = @UpdatedDate, " +
				"UpdatedBy = @UpdatedBy " +
				"WHERE QualityTagGuid = @QualityTagGuid";

			cmd.Parameters.AddWithValue("@Name", ID);
			cmd.Parameters.AddWithValue("@SiteGuid", SiteGuid);
			cmd.Parameters.AddWithValue("@Severity", (int)_Severity);
			cmd.Parameters.AddWithValue("@Active", (_Active ? 1 : 0));
			cmd.Parameters.AddWithValue("@UpdatedDate", UpdatedDate);
			cmd.Parameters.AddWithValue("@UpdatedBy", UpdatedBy);
			cmd.Parameters.AddWithValue("@QualityTagGuid", IdentityGuid);
		}

		public void PurgeSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM tblQualityTags WHERE QualityTagGuid = @QualityTagGuid";
			cmd.Parameters.AddWithValue("@QualityTagGuid", IdentityGuid);
		}

		public void SelectSQL(SqlCommand cmd, bool bInTransaction)
		{
			cmd.CommandText = "SELECT tblQualityTags.* FROM tblQualityTags " + SQLUpdateLock(bInTransaction) + " WHERE QualityTagGuid = @QualityTagGuid";
			cmd.Parameters.AddWithValue("@QualityTagGuid", IdentityGuid);
		}

		public void EnumerateSQL(SqlCommand cmd, SecurityClass security, string filter, string order, bool activeTagsOnly)
		{
			string whereFilter = this.AppendSiteWhereClause(cmd, security, "tblQualityTags", "QualityTagGuid");

			if (filter != null)
			{
				filter = FuelsManagerExtensions.EscapeLikeClauseCharacters(filter.Trim());
				whereFilter += " AND (Name LIKE '%" + filter + "%' ";

				// If filter matches or partially matches one of the severity
				// levels, search on that. Must convert to int for DB
				string[] names = Enum.GetNames(typeof(QUALITY_SEVERITY_LEVELS));

				foreach (string name in names)
				{
					if (name.ToUpper().Contains(filter.ToUpper()))
					{
						QUALITY_SEVERITY_LEVELS severityFilter;
						if (Enum.TryParse<QUALITY_SEVERITY_LEVELS>(name, out severityFilter))
						{
							whereFilter += " OR Severity = " + ((int)severityFilter).ToString();
						}
					}
				}

				whereFilter += " ) ";
			}

			if (activeTagsOnly)
			{
				whereFilter += " AND Active = 1 ";
			}

			string orderClause = "Name ASC";

			if (order != null)
			{
				orderClause = order;
			}

			string sql = "SELECT tblQualityTags.*  FROM tblQualityTags " +
			             " WHERE " +
			             whereFilter +
			             " ORDER BY " + orderClause;

			cmd.CommandText = sql;
		}

		public void SelectByIDSQL(SqlCommand cmd, SecurityClass security, bool bInTransaction)
		{
			cmd.CommandText = " SELECT tblQualityTags.*  FROM tblQualityTags " + SQLUpdateLock(bInTransaction) +
				" WHERE " + this.AppendSiteWhereClause(cmd, security, "tblQualityTags", "QualityTagGuid") + 
				" AND tblQualityTags.Name = @Name";

			cmd.Parameters.AddWithValue("@Name", ID);
		}
	}
}

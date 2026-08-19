namespace FMBusinessObjects.DataObjects
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;

    using FMBusinessObjects.Constants;

    public class WeightedAverageCostCollectionClass : List<WeightedAverageCostClass> { }

   [Serializable]
   [DataContract]
	[KnownType(typeof(GregorianCalendar))]
	public class WeightedAverageCostClass : BaseDataObject
	{
		#region Properties
		[DataMember]
		public Guid WeightedAverageCostGuid
		{
			get
			{
				return base.IdentityGuid;
			}
			set
			{
				base.IdentityGuid = value;
			}
		}
		[DataMember]
		public Guid ProductGuid { get; set; }
		[DataMember]
		public double WacValue { get; set; }
		[DataMember]
		public bool IsManualOverride { get; set; }
		[DataMember]
		public string Source { get; set; }
		[DataMember]
		public string Notes { get; set; }
		[DataMember]
		public DateTime? InventoryDate { get; set; }
		[DataMember]
		public string Alias { get; set; }
		#endregion // Properties

		#region Constructors
		public WeightedAverageCostClass()
		{
			this.Reset();
		}
		#endregion // Constructors

		#region Overrides
		public override void Reset()
		{
			base.Reset();

			this.WeightedAverageCostGuid = Guid.Empty;
			this.SiteGuid = Guids.SiteAdminGuid;
			this.ProductGuid = Guid.Empty;
			this.WacValue = -1.0;
			this.IsManualOverride = true;
			this.Source = "Error: uninitialized";
			this.Notes = "";
			this.Alias = "";
			this.InventoryDate = null;
		}

		[XmlIgnore]
		public override ENTITY_TYPE EntityType
		{
			get
			{
				return ENTITY_TYPE.WEIGHT_AVERAGE_COST;
			}
		}

		[XmlIgnore]
		public override ENTITY_TYPE ParentEntityType
		{
			get
			{
				return ENTITY_TYPE.NONE;
			}
		}

		#endregion // Overrides

		#region Database Interactions
		public void Load(DataRow a_row)
		{
			if (null == a_row)
			{
				throw new ArgumentNullException(MethodBase.GetCurrentMethod().ToString());
			}

			this.WeightedAverageCostGuid = DataObject.getValue<Guid>(a_row["WeightedAverageCostGuid"], Guid.Empty);
			this.SiteGuid = DataObject.getValue<Guid>(a_row["SiteGuid"], Guids.SiteAdminGuid);
			this.ProductGuid = DataObject.getValue<Guid>(a_row["ProductGuid"], Guid.Empty);
			this.WacValue = DataObject.getValue<double>(a_row["WacValue"], -1.0);
			this.IsManualOverride = DataObject.getValue<bool>(a_row["IsManualOverride"], true);
			this.Source = DataObject.getValue<string>(a_row["Source"], "Error: uninitialized");
			this.Notes = DataObject.getValue<string>(a_row["Notes"], "");
			base.CreatedBy = DataObject.getValue<string>(a_row["CreatedBy"], ADMIN);
			base.CreatedDate = DataObject.getValue<DateTimeOffset>(a_row["CreatedDate"], DateTimeOffset.Now);
			base.UpdatedBy = DataObject.getValue<string>(a_row["UpdatedBy"], ADMIN);
			base.UpdatedDate = DataObject.getValue<DateTimeOffset>(a_row["UpdatedDate"], CreatedDate);

			// readonly
			this.Alias = DataObject.getValue<string>(a_row["AliasName"], "");

			if (a_row.IsNull("InventoryDate") == false)
			{
				this.InventoryDate = (DateTime)a_row["InventoryDate"];
			}
		}

		public void Load(DataSet dataSet)
		{
			if (dataSet == null)
			{
				throw new ArgumentNullException(MethodBase.GetCurrentMethod().ToString());
			}

			this.Reset();

			DataTable table = dataSet.Tables[0];

			if (table.Rows.Count == 0)
			{
				return;
			}

			Load(table.Rows[0]);
		}

		#region Enumerators
		static public void EnumerateSQL(SqlCommand cmd)
		{
			// gets all records
			cmd.CommandText =
				"SELECT   w.*, t.AliasName " +
				"FROM     tblWeightedAverageCosts w LEFT OUTER JOIN tblTransactions t ON w.Source = t.TransID";
		}

		static public void EnumerateSQLBySiteProduct(SqlCommand cmd, SecurityClass a_security, Guid a_productGuid)
		{
			// by site and product
			EnumerateSQL(cmd);

			cmd.CommandText += " WHERE w.SiteGuid = " + a_security.SiteGuid + " AND w.ProductGuid = " + a_productGuid;
		}

		static public void SelectIdentityGuidBySiteProduct(SqlCommand cmd, SecurityClass a_security, Guid a_siteGuid, Guid a_productGuid)
		{
			// by site and product
			string sql = "SELECT WeightedAverageCostGuid FROM tblWeightedAverageCosts w" +
				 " WHERE w.SiteGuid = " + a_siteGuid + " AND w.ProductGuid = " + a_productGuid +
				 " ORDER BY WeightedAverageCostGuid DESC";

			cmd.CommandText = sql;
		}

		static public void EnumerateSQLBySiteDateProduct(SqlCommand cmd, SecurityClass a_security, Guid a_siteGuid, Guid a_productGuid, DateTimeOffset a_startDate, DateTimeOffset a_endDate)
		{
			Guid oldSiteGuid = a_security.SiteGuid;

			// overwrite
			a_security.SiteGuid = a_siteGuid;

			EnumerateSQLBySiteProduct(cmd, a_security, a_productGuid);

			// restore
			a_security.SiteGuid = oldSiteGuid;

			string startDateStr = a_startDate.ToString("\\{\\t\\s\\ \\'yyyy\\-MM\\-dd\\ HH:mm:ss\\'\\}");
			string endDateStr = a_endDate.ToString("\\{\\t\\s\\ \\'yyyy\\-MM\\-dd\\ HH:mm:ss\\'\\}");

			cmd.CommandText += " AND w.CreatedDate BETWEEN " + startDateStr + " AND " + endDateStr;

			cmd.CommandText += " ORDER BY WeightedAverageCostGuid DESC";
		}

		static public void SelectByIdentityGuid(SqlCommand cmd, Guid a_weightedAverageCostGuid)
		{
			EnumerateSQL(cmd);

			cmd.CommandText += " WHERE w.WeightedAverageCostGuid = " + a_weightedAverageCostGuid;
		}

		public void InsertSQL(SqlCommand cmd)
		{
			string sql = "INSERT INTO tblWeightedAverageCosts " +
				"(" +
				"SiteGuid, " +
				"ProductGuid, " +
				"WacValue, " +
				"IsManualOverride, " +
				"Source, " +
				"Notes, " +
				"CreatedBy, " +
				"CreatedDate, " +
				"UpdatedBy, " +
				"UpdatedDate, " +
				"InventoryDate " +
				")" +
				"VALUES " +
				"(" +
				"@SiteGuid, " +
				"@ProductGuid, " +
				"@WacValue, " +
				"@IsManualOverride, " +
				"@Source, " +
				"@Notes, " +
				"@CreatedBy, " +
				"@CreatedDate, " +
				"@UpdatedBy, " +
				"@UpdatedDate, " +
				"@InventoryDate)";

			cmd.CommandText = sql;

			cmd.Parameters.AddWithValue("@SiteGuid", SiteGuid);
			cmd.Parameters.AddWithValue("@ProductGuid", ProductGuid);
			cmd.Parameters.AddWithValue("@WacValue", WacValue);
			cmd.Parameters.AddWithValue("@IsManualOverride", IsManualOverride);
			cmd.Parameters.AddWithValue("@Source", Source);

			if (this.IsManualOverride)
			{
				cmd.Parameters.AddWithValue("@Notes", Notes);
				cmd.Parameters.AddWithValue("@CreatedBy", CreatedBy);
				cmd.Parameters.AddWithValue("@CreatedDate", CreatedDate);
				cmd.Parameters.AddWithValue("@UpdatedBy", UpdatedBy);
				cmd.Parameters.AddWithValue("@UpdatedDate", UpdatedDate);
				cmd.Parameters.AddWithValue("@InventoryDate", InventoryDate.HasValue ? InventoryDate.Value.Date : InventoryDate);
			}
			else
			{
				cmd.Parameters.Add("@Notes", SqlDbType.NVarChar).Value = DBNull.Value;
				cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar).Value = DBNull.Value;
				cmd.Parameters.AddWithValue("@CreatedDate", CreatedDate);
				cmd.Parameters.AddWithValue("@UpdatedBy", SqlDbType.NVarChar).Value = DBNull.Value;
				cmd.Parameters.AddWithValue("@UpdatedDate", UpdatedDate);
				cmd.Parameters.AddWithValue("@InventoryDate", InventoryDate.HasValue ? InventoryDate.Value.Date : InventoryDate);
			}
		}

		// usually here will be an update, but we need database level restrictions on updating an
		// existing WAC, you can't delete WACs either for auditing purposes
		#endregion // Enumerators

		#endregion // Database Interactions
	}
}

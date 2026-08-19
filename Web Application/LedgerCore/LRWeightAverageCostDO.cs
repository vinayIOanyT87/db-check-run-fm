namespace LedgerCore
{
	using System;
	using System.Data;
	using System.Data.SqlClient;

	[System.Serializable]
	public class LRWeightAverageCostDO
	{
		#region Private data members
		private Guid weightedAverageCostGuid;
		private Guid productGuid;
		private Guid siteGuid;
		private double wacValue;
		private bool isManualOverride;
		private string source;
		private string notes;
		private string createdBy;
		private string updatedBy;
		private DateTimeOffset createdDate;
		private DateTimeOffset updatedDate;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Weight Average Cost data object class.
		/// </summary>
		public LRWeightAverageCostDO()
		{
			this.Init();
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property returns and sets the WAC Guid.
		/// </summary>
		public Guid WeightedAverageCostGuid
		{
			get { return this.weightedAverageCostGuid; }
			set { this.weightedAverageCostGuid = value; }
		}

		/// <summary>
		/// This property returns and sets the Site Guid.
		/// </summary>
		public Guid SiteGuid
		{
			get { return this.siteGuid; }
			set { this.siteGuid = value; }
		}

		/// <summary>
		/// This property returns and sets the Product Guid.
		/// </summary>
		public Guid ProductGuid
		{
			get { return this.productGuid; }
			set { this.productGuid = value; }
		}

		/// <summary>
		/// This property returns and sets the WAC value.
		/// </summary>
		public double WacValue
		{
			get { return this.wacValue; }
			set { this.wacValue = value; }
		}

		/// <summary>
		/// This property returns true if it is a manual override.
		/// </summary>
		public bool IsManualOverride
		{
			get { return this.isManualOverride; }
			set { this.isManualOverride = value; }
		}

		/// <summary>
		/// This property returns and sets the source of the WAC change.
		/// </summary>
		public string Source
		{
			get { return this.source; }
			set { this.source = value; }
		}

		/// <summary>
		/// This property returns and sets the reason for the WAC change.
		/// </summary>
		public string Notes
		{
			get { return this.notes; }
			set { this.notes = value; }
		}

		/// <summary>
		/// This property returns and sets the Created By value.
		/// </summary>
		public string CreatedBy
		{
			get { return this.createdBy; }
			set { this.createdBy = value; }
		}

		/// <summary>
		/// This property returns and sets the Updated By value.
		/// </summary>
		public string UpdatedBy
		{
			get { return this.updatedBy; }
			set { this.updatedBy = value; }
		}

		/// <summary>
		/// This property returns and sets the Created Date value.
		/// </summary>
		public DateTimeOffset CreatedDate
		{
			get { return this.createdDate; }
			set { this.createdDate = value; }
		}

		/// <summary>
		/// This property returns and sets the Updated Date value.
		/// </summary>
		public DateTimeOffset UpdatedDate
		{
			get { return this.updatedDate; }
			set { this.updatedDate = value; }
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// This method will initial the WAC DO to its initial state.
		/// </summary>
		private void Init()
		{
			this.WeightedAverageCostGuid	= Guid.Empty;
			this.SiteGuid					= Guid.Empty;
			this.ProductGuid				= Guid.Empty;
			this.WacValue					= 0;
			this.IsManualOverride			= true;
			this.Source						= "Error: empty";
			this.Notes						= string.Empty;
			this.CreatedBy					= "SYSTEM";
			this.CreatedDate				= DateTimeOffset.Now;
			this.UpdatedBy					= this.CreatedBy;
			this.UpdatedDate				= this.CreatedDate;
		}
		#endregion

		#region Load methods
		/// <summary>
		/// This method will load the object based on one row.
		/// </summary>
		/// <param name="row"></param>
		public void Load(DataRow row)
		{
			if (null == row)
			{
				return;
			}

			this.WeightedAverageCostGuid	= row.IsNull("WeightedAverageCostGuid") ? Guid.Empty : (Guid)row["WeightedAverageCostGuid"];
			this.SiteGuid					= row.IsNull("SiteGuid") ? Guid.Empty : (Guid)row["SiteGuid"];
			this.ProductGuid				= row.IsNull("ProductGuid") ? Guid.Empty : (Guid)row["ProductGuid"];
			this.WacValue					= row.IsNull("WacValue") ? 0.0 : (double)row["WacValue"];
			this.IsManualOverride			= row.IsNull("IsManualOverride") ? false : (bool)row["IsManualOverride"];
			this.Source						= row.IsNull("Source") ? string.Empty : (string) row["Source"];
			this.Notes						= row.IsNull("Notes") ? string.Empty : (string) row["Notes"];
			this.CreatedBy					= row.IsNull("CreatedBy") ? string.Empty : (string) row["CreatedBy"];
			this.UpdatedBy					= row.IsNull("UpdatedBy") ? string.Empty : (string)row["UpdatedBy"];
			this.CreatedDate				= row.IsNull("CreatedDate") ? DateTimeOffset.Now : (DateTimeOffset) row["CreatedDate"];
			this.UpdatedDate				= row.IsNull("UpdatedDate") ? this.CreatedDate : (DateTimeOffset) row["UpdatedDate"];
		}

		/// <summary>
		/// This method will load the object based on a data set.
		/// </summary>
		/// <param name="dataSet"></param>
		public void Load(DataSet dataSet)
		{
			if (dataSet == null)
			{
				return;
			}

			this.Init();

			DataTable table = dataSet.Tables[0];

			if (table.Rows.Count == 0)
			{
				return;
			}

			this.Load(table.Rows[0]);
		}
		#endregion

		#region SQL Methods
		/// <summary>
		/// This method will create a WAC query to get the most recent WAC for a given
		/// product, site, and date.
		/// </summary>
		/// <returns></returns>
		public string EnumerateSQLBySiteDateProduct()
		{
			const string SQL = "SELECT TOP (1) * FROM tblWeightedAverageCosts " +
			                   "WHERE SiteGuid = @SiteGuid AND ProductGuid = @ProductGuid " +
			                   "AND InventoryDate <= @StartDate " +
			                   "ORDER BY InventoryDate DESC, CreatedDate DESC ";

			return SQL;
		}

		/// <summary>
		/// This method will retrieve the most recent WAC for the site, product, and 
		/// date combination.
		/// </summary>
		/// <param name="ledgerConnection"></param>
		/// <param name="inSiteGuid"></param>
		/// <param name="inProductGuid"></param>
		/// <param name="startDate"></param>
		public void PerformWacQuery(LedgerConnection ledgerConnection, Guid inSiteGuid, Guid inProductGuid, DateTimeOffset startDate)
		{
			using (SqlCommand command = new SqlCommand())
			{
				// Retrieve the most recent WAC SQL
				command.CommandText = this.EnumerateSQLBySiteDateProduct();

				command.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
				command.Parameters.Add("@ProductGuid", SqlDbType.UniqueIdentifier);
				command.Parameters.Add("@StartDate", SqlDbType.DateTimeOffset);

				command.Parameters["@SiteGuid"].Value = inSiteGuid;
				command.Parameters["@ProductGuid"].Value = inProductGuid;
				command.Parameters["@StartDate"].Value = startDate;

				DataSet dataSet = ledgerConnection.GetDataSet(command);

				// Load the retrieve data set.
				this.Load(dataSet);
			}
		}
		#endregion
	}
}
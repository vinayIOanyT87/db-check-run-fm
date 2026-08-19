namespace FMBusinessObjects.DataObjects
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Runtime.Serialization;

    [DataContract]
   [Serializable]
	[KnownType(typeof(CloseoutDO))]
	[KnownType(typeof(BaseCollections))]
	public class CloseoutListDO : DataObject
	{
		#region Attributes

		[DataMember]
		protected CloseoutDO priorCloseout;
		[DataMember]
		protected BaseCollections closeoutList;
		[DataMember]
		protected CloseoutDO subsequentCloseout;
        [DataMember]
        protected string siteID;
		[DataMember]
		protected Guid siteGuid;
		[DataMember]
		protected string productName;
		[DataMember]
		protected string managerName;
		[DataMember]
		protected DateTime? toDate;
		[DataMember]
		protected DateTime? fromDate;
		#endregion Attributes

		#region Properties

		public CloseoutDO PriorCloseout
		{
			get { return this.priorCloseout; }
			set {
			    this.priorCloseout = value; }
		}

		public BaseCollections CloseoutList
		{
			get { return this.closeoutList; }
			set {
			    this.closeoutList = value; }
		}

		public CloseoutDO SubsequentCloseout
		{
			get { return this.subsequentCloseout; }
			set {
			    this.subsequentCloseout = value; }
		}

		public Guid SiteGuid
		{
			get { return this.siteGuid; }
			set {
			    this.siteGuid = value; }
		}

        [DataMember]
        public Guid ManagerGuid { get; set; }

        [DataMember]
        public Guid ProductGuid { get; set; }

        public string SiteID
        {
            get { return this.siteID; }
            set {
                this.siteID = value; }
        }

		public string ProductName
		{
			get { return this.productName; }
			set { this.productName = value; }
		}

		public string ManagerName
		{
			get { return this.managerName; }
			set { this.managerName = value; }
		}

		public DateTime? ToDate
		{
			get { return this.toDate; }
			set {
			    this.toDate = value; }
		}

		public DateTime? FromDate
		{
			get { return this.fromDate; }
			set {
			    this.fromDate = value; }
		}

		#endregion Properties


		public CloseoutListDO()
		{
			this.closeoutList = new BaseCollections();
		}

		#region Overrides

		public override void GetSelectCommand(SqlCommand cmd)
		{
			string select = "SELECT Site, CloseoutDate, ProductName, ManagerName, " +
			"GrossBookInventory, NetBookInventory, GrossPhysicalInventory, " +
			"NetPhysicalInventory, GrossVariance, NetVariance, " +
			"CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, MassBookInventory, MassPhysicalInventory, MassVariance "
			+ " FROM tblCloseoutInventory ";
            string where = "WHERE SiteGuid = @SiteGuid AND ProductGuid = @ProductGuid ";

			if (this.ManagerGuid != Guid.Empty)
			{
				where += " AND ManagerCompanyGuid = @ManagerCompanyGuid ";

				cmd.Parameters.Add("@ManagerCompanyGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@ManagerCompanyGuid"].Value = this.ManagerGuid;
			}

			string orderby = "ORDER BY CloseoutDate DESC";

			if (this.toDate != null)
			{
				where += " AND CloseoutDate <= @ToDate ";

				cmd.Parameters.Add("@ToDate", SqlDbType.DateTime);
				cmd.Parameters["@ToDate"].Value = this.ToDate;
			}
			if (this.fromDate != null)
			{
				where += " AND CloseoutDate >= @FromDate ";

				cmd.Parameters.Add("@FromDate", SqlDbType.DateTime);
				cmd.Parameters["@FromDate"].Value = this.FromDate;
			}

			cmd.CommandText = (select + where + orderby);

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@ProductGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@SiteGuid"].Value = this.SiteGuid;
			cmd.Parameters["@ProductGuid"].Value = this.ProductGuid;
			
		}

		public override string getSelectCommand()
		{
			return null;
		}
		public override string getInsertCommand()
		{
			return null;
		}
		public override string getDeleteCommand()
		{
			return null;
		}
		public override string getUpdateCommand()
		{
			return null;
		}
		#endregion Overrides

		public void GetPreviousSelectCommand(SqlCommand cmd)
		{
			string select = "SELECT TOP 1 Site, CloseoutDate, ProductName, ManagerName, " +
			"GrossBookInventory, NetBookInventory, GrossPhysicalInventory, " +
			"NetPhysicalInventory, GrossVariance, NetVariance, " +
			"CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, MassBookInventory, MassPhysicalInventory, MassVariance "
			+ "FROM tblCloseoutInventory ";
			string where = "WHERE SiteGuid = @SiteGuid AND ProductGuid = @ProductGuid ";
			string orderBy = "ORDER BY CloseoutDate DESC";

			if (this.ManagerGuid != Guid.Empty)
			{
				where += " AND ManagerCompanyGuid = @ManagerGuid ";

				cmd.Parameters.Add("@ManagerGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@ManagerGuid"].Value = this.ManagerGuid;
			}

			if (this.toDate != null)
			{
				where += " AND CloseoutDate < @ToDate ";

				cmd.Parameters.Add("@ToDate", SqlDbType.DateTime);
				cmd.Parameters["@ToDate"].Value = this.ToDate;
			}

			cmd.CommandText = (select + where + orderBy);

            cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@ProductGuid", SqlDbType.UniqueIdentifier);

            cmd.Parameters["@SiteGuid"].Value = this.SiteGuid;
            cmd.Parameters["@ProductGuid"].Value = this.ProductGuid;
		}

		public void GetSubsequentSelectCommand(SqlCommand cmd)
		{
            // Only get the oldest subsequent closeout record. We don't use closeouts that occurred after the subsequent closeout.
			string select = "SELECT TOP(1) Site, CloseoutDate, ProductName, ManagerName, " +
			"GrossBookInventory, NetBookInventory, GrossPhysicalInventory, " +
			"NetPhysicalInventory, GrossVariance, NetVariance, " +
			"CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, MassBookInventory, MassPhysicalInventory, MassVariance "
			+ "FROM tblCloseoutInventory ";
			string where = "WHERE SiteGuid = @SiteGuid AND ProductGuid = @ProductGuid ";
			string orderBy = "ORDER BY CloseoutDate ASC";

			if (this.ManagerGuid != Guid.Empty)
			{
				where += " AND ManagerCompanyGuid = @ManagerGuid ";

				cmd.Parameters.Add("@ManagerGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@ManagerGuid"].Value = this.ManagerGuid;
			}

			if (this.fromDate != null)
			{
				where += " AND CloseoutDate > @FromDate ";

				cmd.Parameters.Add("@FromDate", SqlDbType.DateTime);
				cmd.Parameters["@FromDate"].Value = this.FromDate;
			}

			cmd.CommandText = (select + where + orderBy);

            cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@ProductGuid", SqlDbType.UniqueIdentifier);

            cmd.Parameters["@SiteGuid"].Value = this.SiteGuid;
            cmd.Parameters["@ProductGuid"].Value = this.ProductGuid;
		}

        public void Load(DataSet dataSet)
		{
			if (dataSet != null)
			{
				DataTable table = dataSet.Tables[0];

				for (int i = 0; i < table.Rows.Count; ++i)
				{
					DataRow row = table.Rows[i];

					CloseoutDO closeoutRecord = new CloseoutDO();
					this.CloseoutList.Add(closeoutRecord);

					closeoutRecord.SiteID = getValue<string>(row["Site"], "");
					closeoutRecord.CloseoutDate = getValue<DateTime>(row["CloseoutDate"], DateTime.Today);
					closeoutRecord.ProductName = getValue<string>(row["ProductName"], "");
					closeoutRecord.ManagerName = getValue<string>(row["ManagerName"], "");
					closeoutRecord.BookInventory.GrossInventoryChange = getValue<double>(row["GrossBookInventory"], 0.0);
					closeoutRecord.BookInventory.NetInventoryChange = getValue<double>(row["NetBookInventory"], 0.0);
					closeoutRecord.BookInventory.MassInventoryChange = getValue<double>(row["MassBookInventory"], 0.0);
					closeoutRecord.TotalPhysicalInventory.GrossInventoryChange = getValue<double>(row["GrossPhysicalInventory"], 0.0);
					closeoutRecord.TotalPhysicalInventory.NetInventoryChange = getValue<double>(row["NetPhysicalInventory"], 0.0);
					closeoutRecord.TotalPhysicalInventory.MassInventoryChange = getValue<double>(row["MassPhysicalInventory"], 0.0);
					closeoutRecord.TotalVariance.GrossInventoryChange = getValue<double>(row["GrossVariance"], 0.0);
					closeoutRecord.TotalVariance.NetInventoryChange = getValue<double>(row["NetVariance"], 0.0);
					closeoutRecord.TotalVariance.MassInventoryChange = getValue<double>(row["MassVariance"], 0.0);

					closeoutRecord.CloseoutRecordFound = true;
				}
			}
		}

		public void LoadPrevious(DataSet dataSet)
		{
			if (dataSet.Tables[0].Rows.Count > 0)
			{
				DataRow row = dataSet.Tables[0].Rows[0];

			    this.priorCloseout = new CloseoutDO
			                         {
			                             SiteID = getValue<string>(row["Site"], ""),
			                             CloseoutDate = getValue<DateTime>(row["CloseoutDate"], DateTime.Today),
			                             ProductName = getValue<string>(row["ProductName"], ""),
			                             ManagerName = getValue<string>(row["ManagerName"], ""),
			                             BookInventory =
			                             {
														GrossInventoryChange = getValue<double>(row["GrossBookInventory"], 0.0),
														NetInventoryChange = getValue<double>(row["NetBookInventory"], 0.0),
														MassInventoryChange = getValue<double>(row["MassBookInventory"], 0.0)
			                             },
			                             TotalPhysicalInventory =
			                             {
														GrossInventoryChange = getValue<double>(row["GrossPhysicalInventory"], 0.0),
														NetInventoryChange = getValue<double>(row["NetPhysicalInventory"], 0.0),
														MassInventoryChange = getValue<double>(row["MassPhysicalInventory"], 0.0)
			                             }
			                         };
			    this.priorCloseout.TotalVariance.GrossInventoryChange = getValue<double>(row["GrossVariance"], 0.0);
			    this.priorCloseout.TotalVariance.NetInventoryChange = getValue<double>(row["NetVariance"], 0.0);
			    this.priorCloseout.TotalVariance.MassInventoryChange = getValue<double>(row["MassVariance"], 0.0);
			}
		}

		public void LoadSubsequent(DataSet dataSet)
		{
			if (dataSet.Tables[0].Rows.Count > 0)
			{
				DataRow row = dataSet.Tables[0].Rows[0];

			    this.subsequentCloseout = new CloseoutDO
			                              {
			                                  SiteID = getValue<string>(row["Site"], ""),
			                                  CloseoutDate = getValue<DateTime>(row["CloseoutDate"], DateTime.Today),
			                                  ProductName = getValue<string>(row["ProductName"], ""),
			                                  ManagerName = getValue<string>(row["ManagerName"], ""),
			                                  BookInventory =
			                                  {
															  GrossInventoryChange = getValue<double>(row["GrossBookInventory"], 0.0),
															  NetInventoryChange = getValue<double>(row["NetBookInventory"], 0.0),
															  MassInventoryChange = getValue<double>(row["MassBookInventory"], 0.0)
			                                  },
			                                  TotalPhysicalInventory =
			                                  {
															  GrossInventoryChange = getValue<double>(row["GrossPhysicalInventory"], 0.0),
															  NetInventoryChange = getValue<double>(row["NetPhysicalInventory"], 0.0),
															  MassInventoryChange = getValue<double>(row["MassPhysicalInventory"], 0.0)
			                                  }
			                              };
			    this.subsequentCloseout.TotalVariance.GrossInventoryChange = getValue<double>(row["GrossVariance"], 0.0);
			    this.subsequentCloseout.TotalVariance.NetInventoryChange = getValue<double>(row["NetVariance"], 0.0);
			    this.subsequentCloseout.TotalVariance.MassInventoryChange = getValue<double>(row["MassVariance"], 0.0);
			    this.subsequentCloseout.CloseoutRecordFound = true;
			}
		}
	}
}

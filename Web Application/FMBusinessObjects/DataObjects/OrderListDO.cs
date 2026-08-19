using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

using FMBusinessObjects.UtilityObjects;

namespace FMBusinessObjects.DataObjects
{
   [Serializable]
   [DataContract]
	[KnownType(typeof(OrderListLineItemDO))]
	public class OrderListDO : DataObject
	{
		#region Private data members
		[DataMember]
		private ArrayList orderStatusList = null;
		[DataMember]
		private ArrayList productList = null;
		[DataMember]
		private ArrayList orderTypeList = null;
		[DataMember]
		private ArrayList locationList = null;
		[DataMember]
		private BaseCollections lineItems = null;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the order list 
		/// data object class.
		/// </summary>
		public OrderListDO()
		{
			this.lineItems = new BaseCollections();
			this.orderStatusList = new ArrayList();
			this.productList = new ArrayList();
			this.orderTypeList = new ArrayList();
			this.locationList = new ArrayList();
		}
		#endregion

		#region Properties

		public ArrayList OrderStatusList
		{
			get { return this.orderStatusList; }
			set { this.orderStatusList = value; }
		}

		public ArrayList ProductList
		{
			get { return this.productList; }
			set { this.productList = value; }
		}

		public ArrayList OrderTypeList
		{
			get { return this.orderTypeList; }
			set { this.orderTypeList = value; }
		}

		public ArrayList LocationList
		{
			get { return this.locationList; }
			set { this.locationList = value; }
		}

		public BaseCollections LineItems
		{
			get { return this.lineItems; }
			set { this.lineItems = value; }
		}

		#endregion

		#region Override methods
		public override string getDeleteCommand()
		{
			return null;
		}

		public override string getInsertCommand()
		{
			return null;
		}

		public override string getSelectCommand()
		{
			return null;
		}


		public override string getUpdateCommand()
		{
			return null;
		}
		#endregion

		#region Public methods
		public SqlCommand getSelectCommand(OrderListFilterCriteria Criteria)
		{
			const string PARAM_NAME_ORDERTYPE = "@OrderType";
			const SqlDbType PARAM_TYPE_ORDERTYPE = SqlDbType.NVarChar;
			const int PARAM_SIZE_ORDERTYPE = 200;
			const string PARAM_NAME_MANAGERID = "@ManagerID";
			const SqlDbType PARAM_TYPE_MANAGERID = SqlDbType.NVarChar;
			const int PARAM_SIZE_MANAGERID = 100;
			const string PARAM_NAME_OWNERID = "@OwnerID";
			const SqlDbType PARAM_TYPE_OWNERID = SqlDbType.NVarChar;
			const int PARAM_SIZE_OWNERID = 100;
			const string PARAM_NAME_PRODUCT = "@Product";
			const SqlDbType PARAM_TYPE_PRODUCT = SqlDbType.NVarChar;
			const int PARAM_SIZE_PRODUCT = 30;
			const string PARAM_NAME_CARRIER = "@Carrier";
			const SqlDbType PARAM_TYPE_CARRIER = SqlDbType.NVarChar;
			const int PARAM_SIZE_CARRIER = 100;
			const string PARAM_NAME_SHIPTO = "@ShipTo";
			const SqlDbType PARAM_TYPE_SHIPTO = SqlDbType.NVarChar;
			const int PARAM_SIZE_SHIPTO = 100;
			const string PARAM_NAME_BILLTO = "@BillTo";
			const SqlDbType PARAM_TYPE_BILLTO = SqlDbType.NVarChar;
			const int PARAM_SIZE_BILLTO = 100;
			const string PARAM_NAME_SHIPPER = "@Shipper";
			const SqlDbType PARAM_TYPE_SHIPPER = SqlDbType.NVarChar;
			const int PARAM_SIZE_SHIPPER = 100;
			const string PARAM_NAME_STATUS = "@Status";
			const SqlDbType PARAM_TYPE_STATUS = SqlDbType.Int;
			const string PARAM_NAME_TRANSTYPEID = "@LookupTransTypeIndex";
			const SqlDbType PARAM_TYPE_TRANSTYPEID = SqlDbType.SmallInt;
			const string PARAM_NAME_LOGINSITEGUID = "@LoginSiteGuid";
			const SqlDbType PARAM_TYPE_LOGINSITEGUID = SqlDbType.UniqueIdentifier;
			const string PARAM_NAME_SITEGUID = "@SiteGuid";
			const SqlDbType PARAM_TYPE_SITEGUID = SqlDbType.UniqueIdentifier;
			const string PARAM_NAME_USERGUID = "@UserGuid";
			const SqlDbType PARAM_TYPE_USERGUID = SqlDbType.UniqueIdentifier;
			const string PARAM_NAME_MOREWHERECLAUSE = "@MoreWhereClause";
			const SqlDbType PARAM_TYPE_MOREWHERECLAUSE = SqlDbType.NVarChar;
			const int PARAM_SIZE_MOREWHERECLAUSE = -1;


			SqlCommand cmd = new SqlCommand();

			string SQL = "dbo.usp_OrderSummaryList ";

			bool hasOrderNumber = !string.IsNullOrEmpty(Criteria.OrderNumber);

			// Alias Name
			SQL += AddParameter(cmd, string.Empty, PARAM_NAME_ORDERTYPE, PARAM_TYPE_ORDERTYPE, PARAM_SIZE_ORDERTYPE, Criteria.OrderType);

			// Manager
			SQL += AddParameter(cmd, ",", PARAM_NAME_MANAGERID, PARAM_TYPE_MANAGERID, PARAM_SIZE_MANAGERID, hasOrderNumber ? string.Empty : Criteria.Manager) +
					AddParameter(cmd, ",", PARAM_NAME_OWNERID, PARAM_TYPE_OWNERID, PARAM_SIZE_OWNERID, hasOrderNumber ? string.Empty : Criteria.Owner) +
					AddParameter(cmd, ",", PARAM_NAME_PRODUCT, PARAM_TYPE_PRODUCT, PARAM_SIZE_PRODUCT, hasOrderNumber ? string.Empty : Criteria.Product) +
					AddParameter(cmd, ",", PARAM_NAME_CARRIER, PARAM_TYPE_CARRIER, PARAM_SIZE_CARRIER, hasOrderNumber ? string.Empty : Criteria.Carrier) +
					AddParameter(cmd, ",", PARAM_NAME_SHIPTO, PARAM_TYPE_SHIPTO, PARAM_SIZE_SHIPTO, hasOrderNumber ? string.Empty : Criteria.ShipTo) +
					AddParameter(cmd, ",", PARAM_NAME_BILLTO, PARAM_TYPE_BILLTO, PARAM_SIZE_BILLTO, hasOrderNumber ? string.Empty : Criteria.BillTo) +
					AddParameter(cmd, ",", PARAM_NAME_SHIPPER, PARAM_TYPE_SHIPPER, PARAM_SIZE_SHIPPER, hasOrderNumber ? string.Empty : Criteria.Shipper);


			int statusValue = -1;
			if (!string.IsNullOrEmpty(Criteria.Status) && !hasOrderNumber)
			{
				statusValue = (int)Enum.Parse(typeof(TransactionStatus), Criteria.Status);
			}
			SQL += AddParameter(cmd, ",", PARAM_NAME_STATUS, PARAM_TYPE_STATUS, statusValue);

			SQL += AddParameter(cmd, ",", PARAM_NAME_TRANSTYPEID, PARAM_TYPE_TRANSTYPEID, TransactionTypes.T17_Order) +
						AddParameter(cmd, ",", PARAM_NAME_LOGINSITEGUID, PARAM_TYPE_LOGINSITEGUID, Criteria.Security.LoginSiteGuid) +
						AddParameter(cmd, ",", PARAM_NAME_SITEGUID, PARAM_TYPE_SITEGUID, Criteria.Security.SiteGuid) +
						AddParameter(cmd, ",", PARAM_NAME_USERGUID, PARAM_TYPE_USERGUID, Criteria.Security.UserGuid);



			// Build next part
			string SQL2 = "";

			if (hasOrderNumber)
			{
				SQL2 = " AND A.DocumentNumber = '" + Criteria.OrderNumber + "'";
			}
			else
			{
				// Date range
				SQL2 += AddDateRange(Criteria);

				// Honor the "include deleted" flag from Accounting general configuration
				if (Criteria.ShowDeleted == false)
				{
					SQL2 += " AND A.DeleteFlag = 0";
				}

				// Add sites
				if (Criteria.SiteList != null && Criteria.SiteList.Count > 0)
				{
					SQL2 += " AND A.SiteGuid IN ( '" + Criteria.SiteList[0] + "'";

					for (int nLoop = 1; nLoop < Criteria.SiteList.Count; ++nLoop)
					{
						SQL2 += ",'" + Criteria.SiteList[nLoop] + "'";
					}

					SQL2 += ")";
				}

				// Sort Clause
				if (Criteria.SortExpression != null && Criteria.SortExpression != "")
				{
					SQL2 += " ORDER BY " + Criteria.SortExpression;
				}
			}
			SQL += AddParameter(cmd, ",", PARAM_NAME_MOREWHERECLAUSE, PARAM_TYPE_MOREWHERECLAUSE, PARAM_SIZE_MOREWHERECLAUSE, SQL2);

			// Done
			cmd.CommandText = SQL;
			return cmd;

		}
		#endregion

		#region Private methods
		private string AddDateRange(OrderListFilterCriteria Criteria)
		{
			string SQL = "";

			string FieldName = "";
			string formatString = "\\{\\t\\s\\ \\'yyyy\\-MM\\-dd\\ HH:mm:ss\\'\\}";

			switch (Criteria.DateFilterType)
			{
				case OrderListFilterCriteria.OrderDateFilterType.NONE:
					return "";

				case OrderListFilterCriteria.OrderDateFilterType.SCHEDULED_DATE:
					FieldName = "ScheduledDate";
					break;

				case OrderListFilterCriteria.OrderDateFilterType.TRANSACTION_DATE:
					FieldName = "TransactionDate";
					break;

				case OrderListFilterCriteria.OrderDateFilterType.EFFECTIVE_DATE:
					FieldName = "EffectiveDate";
					formatString = "\\'yyyy\\-MM\\-dd\\'";
					break;

				case OrderListFilterCriteria.OrderDateFilterType.EXPIRATION_DATE:
					FieldName = "ExpirationDate";
					formatString = "\\'yyyy\\-MM\\-dd\\'";
					break;

				case OrderListFilterCriteria.OrderDateFilterType.ETA:
					FieldName = "ETA";
					break;

				case OrderListFilterCriteria.OrderDateFilterType.REQUESTED_DELIVERY_DATE:
					FieldName = "RequestedDeliveryDate";
					break;

				case OrderListFilterCriteria.OrderDateFilterType.DET:
					// DET Sends StartDate and EndDate equal to DateTimeOffset.Now
					SQL += " AND A.LookupTransactionStatusIndex = " + ((int)TransactionStatus.Scheduled).ToString();

					SQL += " AND ";
					SQL += " (A.ScheduledDate IS NULL ";
					SQL += " OR (A.ScheduledDate >= " + Criteria.StartDate.AddHours(-1).ToString("\\{\\t\\s\\ \\'yyyy\\-MM\\-dd\\ HH:mm:ss\\'\\}");
					SQL += " AND A.ScheduledDate <= " + Criteria.EndDate.AddHours(1).ToString("\\{\\t\\s\\ \\'yyyy\\-MM\\-dd\\ HH:mm:ss\\'\\}") + "))";

					SQL += " AND ";
					SQL += " (A.EffectiveDate IS NULL";
					SQL += " OR A.EffectiveDate <= " + Criteria.StartDate.AddHours(1).ToString("\\{\\t\\s\\ \\'yyyy\\-MM\\-dd\\ HH:mm:ss\\'\\}") + ")";

					SQL += " AND ";
					SQL += " (A.ExpirationDate IS NULL";
					SQL += " OR A.ExpirationDate > " + Criteria.EndDate.AddHours(-1).ToString("\\{\\t\\s\\ \\'yyyy\\-MM\\-dd\\ HH:mm:ss\\'\\}") + ")";

					return SQL;

				default:
					throw new Exception("Unknown OrderDateFilterType type");

			}

			SQL += " AND (";
			SQL += "(A." + FieldName + " < " + Criteria.EndDate.ToString(formatString) + ")";
			SQL += " AND (A." + FieldName + " >= " + Criteria.StartDate.ToString(formatString) + ")";

			SQL += ")";

			return SQL;

		}
		#endregion
	}

	#region Order List Filter Criteria class
   [Serializable]
   [DataContract]
	public class OrderListFilterCriteria
	{
		public enum OrderDateFilterType
		{
			NONE,
			SCHEDULED_DATE,
			EFFECTIVE_DATE,
			EXPIRATION_DATE,
			TRANSACTION_DATE,
			ETA,
			DET,
			REQUESTED_DELIVERY_DATE
		};

		[DataMember]
		public OrderDateFilterType DateFilterType = OrderDateFilterType.NONE;

		[DataMember]
		public string Product = "";
		[DataMember]
		public string OrderType = "";
		[DataMember]
		public string Status = "";

		[DataMember]
		public string Manager = "";
		[DataMember]
		public string Carrier = "";
		[DataMember]
		public string ShipTo = "";
		[DataMember]
		public string BillTo = "";
		[DataMember]
		public string Owner = "";
		[DataMember]
		public string Shipper = "";
		[DataMember]
		public string OrderNumber = "";

		[DataMember]
		public bool ShowDeleted = false;

		[DataMember]
		public string SortExpression = null;

		[DataMember]
		public DateTimeOffset StartDate = TimeConverter.MinFMDate;
		[DataMember]
		public DateTimeOffset EndDate = TimeConverter.MaxFMDate;

		[DataMember]
		public SecurityClass Security;

		[DataMember]
		public ArrayList SiteList = null;
	}
	#endregion
}

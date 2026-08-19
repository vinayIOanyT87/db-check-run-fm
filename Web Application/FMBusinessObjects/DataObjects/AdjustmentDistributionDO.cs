using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using FMBusinessObjects.ServiceRequests;

namespace FMBusinessObjects.DataObjects
{
	[DataContract]
   [Serializable]
   public class AdjustmentDistributionDO : DataObject
	{
		#region public attributes
		#endregion

		#region Private Attributes
		[DataMember]
		private ArrayList managerList;
		[DataMember]
		private ArrayList productList;
		[DataMember]
		private ArrayList transactionAliasList;
		[DataMember]
		private ArrayList userFields;
		[DataMember]
		private ArrayList ownerList;
		[DataMember]
		private Hashtable transactionList;
		[DataMember]
		private AdjustmentDistributionConfigurationDO adjustConfigDO;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Adjustment Distribution data object.
		/// </summary>
		public AdjustmentDistributionDO()
		{
			this.Init();
		}
		#endregion

		#region properties
		/// <summary>
		/// This property sets and gets the owner array list attribute.
		/// </summary>
		public ArrayList OwnerList
		{
			get { return this.ownerList; }
		}

		/// <summary>
		/// This property sets and gets the manager array list attribute.
		/// </summary>
		public ArrayList ManagerList
		{
			get { return this.managerList; }
		}

		/// <summary>
		/// This property sets and gets the product array list attribute.
		/// </summary>
		public ArrayList ProductList
		{
			get { return this.productList; }
		}

		/// <summary>
		/// This property sets and gets the transaction alias array list attribute.
		/// </summary>
		public ArrayList TransactionAliasList
		{
			get { return this.transactionAliasList; }
		}

		/// <summary>
		/// This property sets and gets the user field array list attribute.
		/// </summary>
		public ArrayList UserFields
		{
			get { return this.userFields; }
		}

		/// <summary>
		/// This property sets and gets the transaction list attribute.
		/// </summary>
		public Hashtable TransactionList
		{
			get { return this.transactionList; }
		}

		/// <summary>
		/// This property sets and gets the adjustment distribute configuration data 
		/// object attribute.
		/// </summary>
		public AdjustmentDistributionConfigurationDO AdjustConfigurationDO
		{
			get { return this.adjustConfigDO; }
			set { this.adjustConfigDO = value; }
		}
		#endregion

		#region SQL methods
		/// <summary>
		/// This method will return the SqlCommand that will get a list day that have been
		/// closed out record for a given manager, product, and site.
		/// </summary>
		/// <param name="adjustDistSR"></param>
		/// <returns></returns>
		public SqlCommand GetLatestCloseoutDateSelectSQL(AdjustmentDistributionSR adjustDistSR)
		{
			const string PARAM_NAME_INVENTORYDATE = "@InventoryDate";
			const SqlDbType PARAM_TYPE_INVENTORYDATE = SqlDbType.DateTimeOffset;
			const string PARAM_NAME_SITEID = "@SiteID";
			const SqlDbType PARAM_TYPE_SITEID = SqlDbType.NVarChar;
			const int PARAM_SIZE_SITEID = 30;
			const string PARAM_NAME_MANAGERID = "@ManagerID";
			const SqlDbType PARAM_TYPE_MANAGERID = SqlDbType.NVarChar;
			const int PARAM_SIZE_MANAGERID = 100;
			const string PARAM_NAME_PRODUCTID = "@ProductID";
			const SqlDbType PARAM_TYPE_PRODUCTID = SqlDbType.NVarChar;
			const int PARAM_SIZE_PRODUCTID = 30;

			SqlCommand cmd = new SqlCommand();
			string select = "SELECT MAX(CloseoutDate) ";
			string from = "FROM tblOwnerCloseout ";
			//Eric Simmons - (11/23/2007)
			//Updated Call of ToString() to ToString("s") to resolve CSI#5381
			string where = AddParameter(cmd, "WHERE", "CloseoutDate", "<=", PARAM_NAME_INVENTORYDATE, PARAM_TYPE_INVENTORYDATE, adjustDistSR.InventoryDate) +
								AddParameter(cmd, true, "Site", PARAM_NAME_SITEID, PARAM_TYPE_SITEID, PARAM_SIZE_SITEID, adjustDistSR.Security.SiteID);

			if (!string.IsNullOrEmpty(adjustDistSR.ManagerID))
			{
				where += AddParameter(cmd, true, "ManagerName", PARAM_NAME_MANAGERID, PARAM_TYPE_MANAGERID, PARAM_SIZE_MANAGERID, adjustDistSR.ManagerID);
			}

			if (!string.IsNullOrEmpty(adjustDistSR.ProductID))
			{
				where += AddParameter(cmd, true, "ProductName", PARAM_NAME_PRODUCTID, PARAM_TYPE_PRODUCTID, PARAM_SIZE_PRODUCTID, adjustDistSR.ProductID);
			}

			cmd.CommandText = select + from + where;
			return cmd;
		}

		/// <summary>
		/// This method will return a SqlCommand that retrieves the quantity fields for
		/// transactions of type 5 and 6 for a date range.
		/// </summary>
		/// <param name="adjustDistSR"></param>
		/// <param name="closeoutDate"></param>
		/// <returns></returns>
		public SqlCommand GetTransactionsForDateRangeSQL(AdjustmentDistributionSR adjustDistSR, string closeoutDate)
		{
			const string PARAM_NAME_CLOSEOUTDATE = "@CloseoutDate";
			const SqlDbType PARAM_TYPE_CLOSEOUTDATE = SqlDbType.Date;
			const string PARAM_NAME_INVENTORYDATE = "@InventoryDate";
			const SqlDbType PARAM_TYPE_INVENTORYDATE = SqlDbType.Date;

			SqlCommand cmd = new SqlCommand();
			string select = "SELECT l.GrossQuantity, l.NetQuantity, t.OwnerID ";
			string from = "FROM tblTransactions t INNER JOIN tblTransactionLineItems l ON t.TransactionGuid = l.TransactionGuid ";
			string where = "WHERE ";

			if (!string.IsNullOrEmpty(closeoutDate))
			{
				where += AddParameter(cmd, string.Empty, "t.InventoryDate", ">=", PARAM_NAME_CLOSEOUTDATE, PARAM_TYPE_CLOSEOUTDATE, closeoutDate) +
							" AND ";
			}

			//Eric Simmons - (11/23/2007)
			//Updated Call of ToString() to ToString("s") to resolve CSI#5381
			where += AddParameter(cmd, string.Empty, "t.InventoryDate", "<=", PARAM_NAME_INVENTORYDATE, PARAM_TYPE_INVENTORYDATE, adjustDistSR.InventoryDate) +
						this.BuildAliasList(adjustDistSR, cmd);

			cmd.CommandText = select + from + where;
			return cmd;
		}
		#endregion

		#region Load methods
		/// <summary>
		/// This method will load the closed out record for a given manager, 
		/// product, and site. If a closeout date is not found, then and empty
		/// date string is returned.
		/// </summary>
		/// <param name="dataSet"></param>
		public string LoadLatestCloseoutDate(System.Data.DataSet dataSet)
		{
			DateTimeOffset closeoutDate;
			string closeoutDateStr = "";

			if (dataSet != null)
			{
				System.Data.DataTable table = dataSet.Tables[0];

				if (table.Rows.Count > 0)
				{
					System.Data.DataRow row = table.Rows[0];
					object dateObj = row["CloseoutDate"];
					bool dateExists = !isNull(dateObj);

					if (dateExists == true)
					{
						closeoutDate = (DateTimeOffset)dateObj;
						closeoutDateStr = closeoutDate.ToString("s");
					}
				}
			}

			return closeoutDateStr;
		}

		/// <summary>
		/// This method will load the transaction data (quantities) and sum up the
		/// quantities for each owner.  It will return a hash table that contains
		/// the throughput for each owner.
		/// </summary>
		/// <param name="dataSet"></param>
		/// <returns></returns>
		public void LoadTransactionsForDateRangeSQL(System.Data.DataSet dataSet)
		{
			string owner = "";
			this.transactionList = new Hashtable();

			if (dataSet != null)
			{
				System.Data.DataTable table = dataSet.Tables[0];

				foreach (System.Data.DataRow row in table.Rows)
				{
					owner = DataObject.getValue<string>(row["OwnerID"], "");

					if (this.transactionList.Contains(owner) == true)
					{
						QuantityDO existingQuantity = (QuantityDO)this.transactionList[owner];
						existingQuantity.Gross = existingQuantity.Gross + DataObject.getValue<double>(row["GrossQuantity"], 0.0);
						existingQuantity.Net = existingQuantity.Net + DataObject.getValue<double>(row["NetQuantity"], 0.0);
					}
					else
					{
						QuantityDO quantity = new QuantityDO();
						quantity.Gross = DataObject.getValue<double>(row["GrossQuantity"], 0.0);
						quantity.Net = DataObject.getValue<double>(row["NetQuantity"], 0.0);

						this.transactionList.Add(owner, quantity);
					}
				}
			}
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// This method will build an IN clause that contains a list of aliases that should be used
		/// to calculate throughput.  If the list is empty, then the default is transaction types
		/// 5 and 6. It will return a WHERE clause string.
		/// </summary>
		/// <param name="adjustDistSR"></param>
		/// <returns></returns>
		private string BuildAliasList(AdjustmentDistributionSR adjustDistSR, SqlCommand cmd)
		{
			const string PARAM_NAME_PRIMARYDISBURSEMENT = "@TransTypeID1";
			const string PARAM_NAME_SECONDARYDISBURSEMENT = "@TransTypeID2";
			const SqlDbType PARAM_TYPE_TRANSTYPEID = SqlDbType.SmallInt;

			const SqlDbType PARAM_TYPE_ALIASNAME = SqlDbType.NVarChar;
			const int PARAM_SIZE_ALIASNAME = 32;

			string where = " AND t.LookupTransTypeIndex IN (";
			where += AddParameter(cmd, string.Empty, PARAM_NAME_PRIMARYDISBURSEMENT, PARAM_TYPE_TRANSTYPEID, TransactionTypes.T5_PrimaryDisbursement) +
						AddParameter(cmd, ",", PARAM_NAME_SECONDARYDISBURSEMENT, PARAM_TYPE_TRANSTYPEID, TransactionTypes.T6_SecondaryDisbursement) +
						") ";

			if ((adjustDistSR.AffectsInventoryAliasList != null) && (adjustDistSR.AffectsInventoryAliasList.Count > 0))
			{
				// Open the IN clause.
				where = " AND t.AliasName IN (";

				// Add aliases to the IN clause.
				for (int idx = 0; idx < adjustDistSR.AffectsInventoryAliasList.Count; idx++)
				{
					string paramName = "@AliasName" + idx.ToString();
					where += AddParameter(cmd, idx == 0 ? string.Empty : ",", paramName,
									PARAM_TYPE_ALIASNAME, PARAM_SIZE_ALIASNAME, adjustDistSR.AffectsInventoryAliasList[idx].AliasName);
				}

				// Close the IN clause.
				where = where + ") ";
			}

			return where;
		}

		/// <summary>
		/// This method initializes the object to its initial state.
		/// </summary>
		private void Init()
		{
			this.managerList = new ArrayList();
			this.productList = new ArrayList();
			this.userFields = new ArrayList();
			this.ownerList = new ArrayList();
			this.transactionList = new Hashtable();
			this.transactionAliasList = new ArrayList();
		}
		#endregion

		#region Override Methods
		override public string getUpdateCommand()
		{
			return null;
		}

		override public string getDeleteCommand()
		{
			return null;
		}

		override public string getInsertCommand()
		{
			return null;
		}

		override public string getSelectCommand()
		{
			return null;
		}
		#endregion
	}
}

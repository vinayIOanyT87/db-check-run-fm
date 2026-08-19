using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace FMBusinessObjects.DataObjects
{
   [Serializable]
   [DataContract]
	public class PhysicalInventoryListDO : DataObject
	{
		#region Attributes
		[DataMember]
		protected string siteName;
		[DataMember]
		protected string manager;
		[DataMember]
		protected string product;
		[DataMember]
		protected DateTimeOffset? firstDate;
		[DataMember]
		protected DateTimeOffset? lastDate;
		[DataMember]
		protected Guid loginSiteGuid;
		[DataMember]
		protected Guid siteGuid;
		[DataMember]
		protected Guid userGuid;
		[DataMember]
		protected ArrayList lineItems;
		#endregion Attributes

		#region Constructors
		public PhysicalInventoryListDO()
		{
			this.lineItems = new ArrayList();
		}
		#endregion

		#region Properties

		public ArrayList LineItems
		{
			get { return lineItems; }
			private set { this.lineItems = value; }
		}
		#endregion Properties

		#region Public methods

		/// <summary>
		/// This method is an update to the getSelect command above. It passes in an additional parameter
		/// for a subquery to retrieve a list of assign alias names.
		/// </summary>
		/// <param name="siteName"></param>
		/// <param name="manager"></param>
		/// <param name="product"></param>
		/// <param name="firstDate"></param>
		/// <param name="lastDate"></param>
		/// <param name="userCompanyList"></param>
		/// <param name="subquery"></param>
		/// <returns></returns>
		public void GetPhysicalInventorySelectSQL(string siteName, string manager, string product, DateTimeOffset? firstDate,
													DateTimeOffset? lastDate, Guid loginSiteGuid, Guid siteGuid, Guid userGuid, SqlCommand cmd)
		{
			const string PARAM_NAME_SITENAME = "@SiteName";
			const int PARAM_SIZE_SITENAME = 30;
			const SqlDbType PARAM_TYPE_SITENAME = SqlDbType.NVarChar;
			const string PARAM_NAME_MANAGERID = "@ManagerID";
			const int PARAM_SIZE_MANAGERID = 100;
			const SqlDbType PARAM_TYPE_MANAGERID = SqlDbType.NVarChar;
			const string PARAM_NAME_PRODUCTID = "@ProductID";
			const int PARAM_SIZE_PRODUCTID = 30;
			const SqlDbType PARAM_TYPE_PRODUCTID = SqlDbType.NVarChar;
			const string PARAM_NAME_DELETEFLAG = "@DeleteFlag";
			const SqlDbType PARAM_TYPE_DELETEFLAG = SqlDbType.Bit;
			const string PARAM_NAME_STARTDATE = "@StartDate";
			const SqlDbType PARAM_TYPE_STARTDATE = SqlDbType.Date;
			const string PARAM_NAME_ENDDATE = "@EndDate";
            const SqlDbType PARAM_TYPE_ENDDATE = SqlDbType.Date;

			this.siteName = siteName;
			this.manager = manager;
			this.product = product;
			this.firstDate = firstDate;
			this.lastDate = lastDate;
			this.loginSiteGuid = loginSiteGuid;
			this.siteGuid = siteGuid;
			this.userGuid = userGuid;

			int deleted = 0;
			string select = "SELECT a.InventoryDate, SUM(b.GrossQuantity) AS GrossQuantity, SUM(b.NetQuantity) AS NetQuantity, SUM(b.MassQuantity) AS MassQuantity ";
			string from = "FROM tblTransactions a INNER JOIN  tblTransactionLineItems b ON a.TransactionGuid = b.TransactionGuid ";

			string where = AddParameter(cmd, "WHERE", "a.Site", "=", PARAM_NAME_SITENAME, PARAM_TYPE_SITENAME, PARAM_SIZE_SITENAME, siteName) +
								AddParameter(cmd, true, "a.ManagerID", PARAM_NAME_MANAGERID, PARAM_TYPE_MANAGERID, PARAM_SIZE_MANAGERID, manager) +
								AddParameter(cmd, true, "b.Product", PARAM_NAME_PRODUCTID, PARAM_TYPE_PRODUCTID, PARAM_SIZE_PRODUCTID, product) +
								AddParameter(cmd, true, "a.DeleteFlag", PARAM_NAME_DELETEFLAG, PARAM_TYPE_DELETEFLAG, deleted);

			if (firstDate != null)
			{
				where += AddParameter(cmd, "AND", "a.InventoryDate", ">=", PARAM_NAME_STARTDATE, PARAM_TYPE_STARTDATE, firstDate.Value.Date);
			}

			if (lastDate != null)
			{
                where += AddParameter(cmd, "AND", "a.InventoryDate", "<=", PARAM_NAME_ENDDATE, PARAM_TYPE_ENDDATE, lastDate.Value.Date);
			}

			// Get the "where" clause that filters on the user's permission to view
			// transaction data. Pass in the table name. It may return an empty string.
			//bypass this line if userIndex = 0 - from LoadRackService to do the AutoCloseout CSI5172
			if (userGuid != Guid.Empty)
			{
				where += PhysicalInventoryDO.GetUserToCompanyWhereClause("a", loginSiteGuid, siteGuid, userGuid, cmd);
			}

			where = where + "AND AliasName IN (" + cmd.CommandText + ")";

			string groupBy = "GROUP BY InventoryDate ";
			string orderBy = "ORDER BY InventoryDate ASC";

			cmd.CommandText = select + from + where + groupBy + orderBy;
		}
		#endregion

		#region Overrides

		/// <summary>
		/// Temp to be deleted later
		/// </summary>
		/// <returns></returns>
		public override string getSelectCommand()
		{
			throw new NotImplementedException();
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
	}
}

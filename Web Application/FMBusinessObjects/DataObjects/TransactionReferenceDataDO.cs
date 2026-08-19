using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
   [Serializable]
   [DataContract]
	public class TransactionReferenceDataDO : DataObject
	{
		#region Attributes
		[DataMember]
		private string owner;
		[DataMember]
		private string manager;
		[DataMember]
		private string product;

		private enum LineItemListType { PRODUCTS, MANAGERS, OWNERS };
		#endregion

		#region Constructor
		/// <summary>
		/// This is the default constructor for the transaction line item
		/// data object class.
		/// </summary>
		public TransactionReferenceDataDO ( )
		{
			this.reset ( );
		}
		#endregion

		#region SQL Methods
		/// <summary>
		/// This method will return a SQL that is used to return a list of distinct
		/// products from the transaction line item table.  A list of sites are
		/// used to filter the products per site and a list of existing products are
		/// used to filter the products not to get.
		/// </summary>
		/// <param name="siteList"></param>
		/// <param name="existingProducList"></param>
		/// <returns></returns>
		public string productListSelectSQL ( ArrayList siteList, ArrayList existingProductList )
		{
			int deleted = 0;
			string select = "SELECT distinct l.Product ";
			string from = "FROM tblTransactionLineItems l, tblTransactions t ";
			string where = "WHERE t.TransactionGuid = l.TransactionGuid AND t.DeleteFlag = " + deleted + " AND " +
							 this.getWhereClauseFilterOnSites ( siteList );
			string productClause = this.getWhereClauseFilterExisting ( existingProductList, LineItemListType.PRODUCTS );

			if (productClause.Length > 0)
			{
				where = where + " AND " + productClause;
			}

			string sql = select + from + where;
			return sql;
		}

		/// <summary>
		/// This method will return a SQL that is used to return a list of distinct
		/// maanagers from the transaction line item table.  A list of sites are
		/// used to filter the managers per site and a list of existing managers are
		/// used to filter the managers not to get.
		/// </summary>
		/// <param name="siteList"></param>
		/// <param name="existingProducList"></param>
		/// <returns></returns>
		public string managerListSelectSQL ( ArrayList siteList, ArrayList existingManagerList )
		{
			int deleted = 0;
			string select = "SELECT distinct t.ManagerID ";
			string from = "FROM tblTransactions t ";
			string where = "WHERE t.DeleteFlag = " + deleted + " AND " + this.getWhereClauseFilterOnSites ( siteList );

			string managerClause = this.getWhereClauseFilterExisting ( existingManagerList, LineItemListType.MANAGERS );
			if (managerClause.Length > 0)
			{
				where = where + " AND " + managerClause;
			}

			string sql = select + from + where;
			return sql;
		}

		/// <summary>
		/// This method will return a SQL that is used to return a list of distinct
		/// owners from the transaction line item table.  A list of sites are
		/// used to filter the owners per site and a list of existing owners are
		/// used to filter the owners not to get.
		/// </summary>
		/// <param name="siteList"></param>
		/// <param name="existingOwnerList"></param>
		/// <returns></returns>
		public string ownerListSelectSQL ( ArrayList siteList, ArrayList existingOwnerList )
		{
			int deleted = 0;
			string select = "SELECT distinct t.OwnerID ";
			string from = "FROM tblTransactions t ";
			string where = "WHERE t.DeleteFlag = " + deleted + " AND " + this.getWhereClauseFilterOnSites ( siteList );

			string ownerClause = this.getWhereClauseFilterExisting ( existingOwnerList, LineItemListType.OWNERS );
			if (ownerClause.Length > 0)
			{
				where = where + " AND " + ownerClause;
			}

			string sql = select + from + where;

			return sql;
		}

		/// <summary>
		/// This method will return a WHERE clause that is used to filter on not
		/// returning existing products/owners/managers.
		/// </summary>
		/// <param name="existingList"></param>
		/// <returns></returns>
		private string getWhereClauseFilterExisting ( ArrayList existingList, LineItemListType listType )
		{
			string where = "";
			string columnName = "";

			switch (listType)
			{
				case LineItemListType.MANAGERS:
					columnName = "t.ManagerID";
					break;
				case LineItemListType.OWNERS:
					columnName = "t.OwnerID";
					break;
				case LineItemListType.PRODUCTS:
					columnName = "l.Product";
					break;
			}

			if (existingList != null)
			{
				// Build a where clause that filters out existing owners/products/managers.
				for (int nextItem = 0; nextItem < existingList.Count; nextItem++)
				{
					if (nextItem == 0)
						where = where + " " + columnName + " NOT IN ('" + existingList[nextItem];
					else
						where = where + "', '" + existingList[nextItem];

					if (nextItem == ( existingList.Count - 1 ))
						where = where + "') ";
				}
			}

			return where;
		}

		/// <summary>
		/// This method will return a WHERE clause that is used to filter on sites.
		/// </summary>
		/// <param name="siteList"></param>
		/// <returns></returns>
		private string getWhereClauseFilterOnSites ( ArrayList siteList )
		{
			string where = "";

			if (siteList != null)
			{
				// Build a where clause that filters the products by the given
				// sites.
				for (int nextSite = 0; nextSite < siteList.Count; nextSite++)
				{
					if (nextSite == 0)
						where = where + " t.Site IN ('" + siteList[nextSite];
					else
						where = where + "', '" + siteList[nextSite];

					if (nextSite == ( siteList.Count - 1 ))
						where = where + "') ";
				}
			}

			return where;
		}
		#endregion

		#region Load Methods
		/// <summary>
		/// This method loads the "From Product" attribute with the value from
		/// the database. The use of this method is to contain a product name
		/// for a list of products.
		/// </summary>
		/// <param name="dataSet"></param>
		public void loadProduct ( System.Data.DataSet dataSet )
		{
			this.product = "";

			if (dataSet != null)
			{
				System.Data.DataTable table = dataSet.Tables[0];

				if (table.Rows.Count > 0)
				{
					System.Data.DataRow row = table.Rows[0];
					this.product = DataObject.getValue<string>(row["Product"], "");
				}
			}
		}

		/// <summary>
		/// This method loads the "From Owner" attribute with the value from
		/// the database. The use of this method is to contain a owner name
		/// for a list of owners.
		/// </summary>
		/// <param name="dataSet"></param>
		public void loadOwner ( System.Data.DataSet dataSet )
		{
			this.owner = "";

			if (dataSet != null)
			{
				System.Data.DataTable table = dataSet.Tables[0];

				if (table.Rows.Count > 0)
				{
					System.Data.DataRow row = table.Rows[0];
					this.owner = DataObject.getValue<string>(row["OwnerID"], "");
				}
			}
		}

		/// <summary>
		/// This method loads the "From Manager" attribute with the value from
		/// the database. The use of this method is to contain a manager name
		/// for a list of managers.
		/// </summary>
		/// <param name="dataSet"></param>
		public void loadManager ( System.Data.DataSet dataSet )
		{
			this.manager = "";

			if (dataSet != null)
			{
				System.Data.DataTable table = dataSet.Tables[0];

				if (table.Rows.Count > 0)
				{
					System.Data.DataRow row = table.Rows[0];
					this.manager = DataObject.getValue<string>(row["ManagerID"], "");
				}
			}
		}
		#endregion

		#region General Public Methods
		/// <summary>
		/// This method resets this object to its initial state.
		/// </summary>
		public void reset ( )
		{
			owner = "";
			manager = "";
			product = "";
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property will get and set the owner attribute.
		/// </summary>
		public string Owner
		{
			get { return this.owner; }
			set { this.owner = value; }
		}

		/// <summary>
		/// This property will get and set the manager attribute.
		/// </summary>
		public string Manager
		{
			get { return this.manager; }
			set { this.manager = value; }
		}

		/// <summary>
		/// This property will get and set the product attribute.
		/// </summary>
		public string Product
		{
			get { return this.product; }
			set { this.product = value; }
		}
		#endregion

		#region Override Methods
		override public string getSelectCommand ( )
		{
			return null;
		}

		override public string getInsertCommand ( )
		{
			return null;
		}

		override public string getDeleteCommand ( )
		{
			return null;
		}

		override public string getUpdateCommand ( )
		{
			return null;
		}

		#endregion
	}
}

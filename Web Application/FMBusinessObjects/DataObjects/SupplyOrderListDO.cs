/// <summary>
/// SupplyOrderListDO
///
/// Original Author: Ivan Orndorff
/// Revisions: See source control comments
///
/// (C) Copyright 2007 by Varec, Inc.  All rights reserved.
///
///	MODIFICATION HISTORY:
///		Date:		   By:					Reason:
///		----------	-----------------	-------------------------------------------
///		2007-09-24	I. Orndorff			- Initial Revision based off v7.3.0.0 of
///										      OrderListDO.
///		2007-11-23	E. Simmons			Updated Calls of ToShortDateString() to ToString("s") 
///										      to resolve CSI#5381		
///		2008-04-17	I.Orndorff			7.4.3.0 - Modified "getSelectCommand()" to filter by Order Number only
///												  if the Order Number is not empty.
///												  This fixes CSI #5428.
///
///		08/27/2008	W.Gray				7.4.5.0 - Revised AddDateRange to not add a day to the EndDate (CSI 6113) 
///
///		05/19/2009	A. Coker				Fixed defect 3611. Rearranged site list in query to fit string passed into sql execute.
/// 
///      2009-06-23  Richard Panachida WI#4092: Added code to set the begin time to zeroes and the end time to 23:59:59 on the
///                                    date range.
///                                    
///      2009-07-09  Richard Panachida WI# 4092: Moved the begin time to zeroes and end time to 23:59:59 method to the 
///                                    SupplyOrderSummaryForm.aspx.cs file prior to the time zone conversion.
/// </summary>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using FMBusinessObjects.UtilityObjects;

namespace FMBusinessObjects.DataObjects
{
   [Serializable]
   [DataContract]
	[KnownType(typeof(SupplyOrderListLineItemDO))]
	public class SupplyOrderListDO : DataObject
	{
		#region Private data members
		[DataMember]
		private ArrayList orderStatusList = null;
		[DataMember]
		private ArrayList productList = null;
		[DataMember]
		private ArrayList orderTypeList = null;
		[DataMember]
		private BaseCollections lineItems;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Supply Order List DO class.
		/// </summary>
		public SupplyOrderListDO()
		{
			this.lineItems = new BaseCollections();
			this.orderStatusList = new ArrayList();
			this.productList = new ArrayList();
			this.orderTypeList = new ArrayList();
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

		public BaseCollections LineItems
		{
			get { return this.lineItems; }
			set { this.lineItems = value; }
		}
		#endregion

		#region Public methods
		public override string getDeleteCommand()
		{
			return null;
		}


		public override string getInsertCommand()
		{
			return null;
		}

		/// <summary>
		/// This method will build the select statement to retrieve the supply order
		/// summary.
		/// </summary>
		/// <param name="Criteria"></param>
		/// <returns></returns>
		public void GetSelectCommand(SqlCommand cmd, SupplyOrderListFilterCriteria Criteria)
		{
			cmd.CommandText = "dbo.usp_SupplyOrderSummaryList";
			cmd.CommandType = CommandType.StoredProcedure;

			// If the order number is empty honor all filter criteria
			if ("" == Criteria.OrderNumber)
			{
				// Alias Name

				cmd.Parameters.AddWithValue("@AliasName", Criteria.OrderType);

				// Manager
				cmd.Parameters.AddWithValue("@ManagerID", Criteria.Manager);
				cmd.Parameters.AddWithValue("@OwnerID", Criteria.Owner);
				cmd.Parameters.AddWithValue("@Product", Criteria.Product);
				cmd.Parameters.AddWithValue("@ShipperID", Criteria.Shipper);
				cmd.Parameters.AddWithValue("@SupplierID", Criteria.Supplier);

				if (Criteria.Status == "")
				{
					cmd.Parameters.AddWithValue("@Status", -1);
				}
				else
				{
					cmd.Parameters.AddWithValue("@Status",
						((int)Enum.Parse(typeof(TransactionStatus), Criteria.Status)));
				}

				cmd.Parameters.AddWithValue("@LookupTransTypeIndex", (int)TransactionTypes.T18_SupplyOrder);
				cmd.Parameters.AddWithValue("@LoginSiteGuid", (Guid)Criteria.Security.LoginSiteGuid);
				cmd.Parameters.AddWithValue("@SiteGuid", (Guid)Criteria.Security.SiteGuid);

				cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier);

				if (Criteria.Security.UserGuid == Guid.Empty)
				{
					cmd.Parameters["@UserGuid"].Value = DBNull.Value;
				}
				else
				{
					cmd.Parameters["@UserGuid"].Value = Criteria.Security.UserGuid;
				}

				// Build next part
				string SQL2 = "";

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

				cmd.Parameters.AddWithValue("@MoreWhereClause", SQL2);
			}
			// build up sql with the order number being the only criteria.
			// This fixes CSI #5428
			else
			{
				// Alias Name
				cmd.Parameters.AddWithValue("@AliasName", Criteria.OrderType);

				// Manager, Owner, Product, Shipper, Supplier, Status
				cmd.Parameters.AddWithValue("@ManagerID", string.Empty);
				cmd.Parameters.AddWithValue("@OwnerID", string.Empty);
				cmd.Parameters.AddWithValue("@Product", string.Empty);
				cmd.Parameters.AddWithValue("@ShipperID", string.Empty);
				cmd.Parameters.AddWithValue("@SupplierID", string.Empty);
				cmd.Parameters.AddWithValue("@Status", -1);

				// Transaction Type
				cmd.Parameters.AddWithValue("@LookupTransTypeIndex", (int)TransactionTypes.T18_SupplyOrder);

				// Login Site Guid
				cmd.Parameters.AddWithValue("@LoginSiteGuid", (Guid)Criteria.Security.LoginSiteGuid);

				// Site Guid
				cmd.Parameters.AddWithValue("@SiteGuid", (Guid)Criteria.Security.SiteGuid);

				// User Guid
				cmd.Parameters.AddWithValue("@UserGuid", Criteria.Security.UserGuid);

				// More Where Clause
				cmd.Parameters.AddWithValue("@MoreWhereClause",
					" AND A.DocumentNumber = '" + Criteria.OrderNumber + "'");
			}
		}

		public override string getSelectCommand()
		{
			return null;
		}

		public override string getUpdateCommand()
		{
			return null;
		}

		public override void GetSelectCommand(SqlCommand cmd)
		{
			throw new NotImplementedException();
		}

		public override void GetInsertCommand(SqlCommand cmd)
		{
			throw new NotImplementedException();
		}

		public override void GetDeleteCommand(SqlCommand cmd)
		{
			throw new NotImplementedException();
		}

		public override void GetUpdateCommand(SqlCommand cmd)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method will add the appropriate date range to the WHERE clause.
		/// </summary>
		/// <param name="Criteria"></param>
		/// <returns></returns>
		private string AddDateRange(SupplyOrderListFilterCriteria Criteria)
		{
			string SQL = "";

			string FieldName = "";

			switch (Criteria.DateFilterType)
			{
				case SupplyOrderListFilterCriteria.SupplyOrderDateFilterType.NONE:
					return "";

				case SupplyOrderListFilterCriteria.SupplyOrderDateFilterType.ESTIMATED_DATE:
					SQL += " AND ";
					SQL += " A.EstimatedDeliveryDateFrom >= " +
						   Criteria.StartDate.ToString("\\{\\t\\s\\ \\'yyyy\\-MM\\-dd\\ HH:mm:ss\\'\\}");

					SQL += " AND ";
					SQL += " A.EstimatedDeliveryDateTo <= " +
						   Criteria.EndDate.ToString("\\{\\t\\s\\ \\'yyyy\\-MM\\-dd\\ HH:mm:ss\\'\\}");

					return SQL;

				case SupplyOrderListFilterCriteria.SupplyOrderDateFilterType.REQUIRED_DATE:
					FieldName = "RequiredDeliveryDate";
					break;

				case SupplyOrderListFilterCriteria.SupplyOrderDateFilterType.TRANSACTION_DATE:
					FieldName = "TransactionDate";
					break;

				default:
					throw new Exception("Unknown OrderDateFilterType type");

			}

			SQL += " AND (";
			SQL += "(A." + FieldName + " <= " +
				   Criteria.EndDate.ToString("\\{\\t\\s\\ \\'yyyy\\-MM\\-dd\\ HH:mm:ss\\'\\}") + ")";
			SQL += " AND (A." + FieldName + " >= " +
				   Criteria.StartDate.ToString("\\{\\t\\s\\ \\'yyyy\\-MM\\-dd\\ HH:mm:ss\\'\\}") + ")";

			SQL += ")";

			return SQL;
		}
		#endregion
	}

	#region Supply Order List Filter Criteria class
   [Serializable]
   [DataContract]
	public class SupplyOrderListFilterCriteria
	{
		public enum SupplyOrderDateFilterType
		{
			NONE,
			ESTIMATED_DATE,
			REQUIRED_DATE,
			TRANSACTION_DATE,
		};

		[DataMember]
		public SupplyOrderDateFilterType DateFilterType = SupplyOrderDateFilterType.NONE;

		[DataMember]
		public string Product = "";
		[DataMember]
		public string OrderType = "";
		[DataMember]
		public string Status = "";
		[DataMember]
		public string Manager = "";
		[DataMember]
		public string Owner = "";
		[DataMember]
		public string Shipper = "";
		[DataMember]
		public string Supplier = "";
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

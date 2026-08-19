using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Collections;
using FMBusinessObjects.ServiceRequests;

using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;

namespace FMBusinessObjects.DataObjects
{
	[DataContract]
   [Serializable]
	[KnownType(typeof(DropdownValuePairDO))]
	[KnownType(typeof(InvoiceListLineItemDO))]
	public class InvoiceListDO : DataObject
	{
		#region Private data members
		[DataMember]
		private ArrayList accountCodeList;
		[DataMember]
		private ArrayList costCenterCodeList;
		[DataMember]
		private ArrayList productList;
		[DataMember]
		private BaseCollections lineItems;
		private const int NOT_DELETED = 0;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Invoice List Data Object class.
		/// </summary>
		public InvoiceListDO()
		{
			// Initialize the object to its initial state.
			this.Initialize();
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property returns the line items for the invoice.
		/// </summary>
		public BaseCollections LineItems
		{
			get { return this.lineItems; }
			private set { this.lineItems = value; }
		}

		/// <summary>
		/// This property returns the an array of account codes value pairs.
		/// </summary>
		public ArrayList AccountCodeList
		{
			get { return this.accountCodeList; }
			private set { this.accountCodeList = value; }
		}

		/// <summary>
		/// This property returns the an array of cost center codes value pairs.
		/// </summary>
		public ArrayList CostCenterCodeList
		{
			get { return this.costCenterCodeList; }
			private set { this.costCenterCodeList = value; }
		}

		/// <summary>
		/// This property returns the an array of product value pairs.
		/// </summary>
		public ArrayList ProductList
		{
			get { return this.productList; }
			private set { this.productList = value; }
		}
		#endregion

		#region Public SQL methods
		/// <summary>
		/// This method build an invoice summary SqlCommand select command using the filters
		/// in the InvoiceListSR sevice request.
		/// </summary>
		/// <param name="invoiceListSR"></param>
		/// <returns></returns>
		public void GetInvoiceSelectCommand(SqlCommand cmd, InvoiceListSR invoiceListSR)
		{
			cmd.CommandType = CommandType.StoredProcedure;

			if (FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsADFKey()))
			{
				cmd.CommandText = "dbo.usp_InvoiceLineItemSummaryList ";
			}
			else
			{
				cmd.CommandText = "dbo.usp_InvoiceSummaryList ";
			}

			// This would be an error if the invoice list SR is null.
			if (invoiceListSR == null)
			{
				return;
			}

			cmd.Parameters.Add("@InvoiceNumber", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@InvoiceType", SqlDbType.Int);
			cmd.Parameters.Add("@AccountCode", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@CostCenterCode", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@ShipToID", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@SupplierID", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@ProductID", SqlDbType.NVarChar, 30);
			cmd.Parameters.Add("@LoginSiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@DeleteFlag", SqlDbType.Bit);
			cmd.Parameters.Add("@StartDate", SqlDbType.NVarChar, 10);
			cmd.Parameters.Add("@EndDate", SqlDbType.NVarChar, 10);
			cmd.Parameters.Add("@SortExpression", SqlDbType.NVarChar, -1);

			if (string.IsNullOrEmpty(invoiceListSR.InvoiceNumber))
			{
				cmd.Parameters["@InvoiceNumber"].Value = "N/A";
				cmd.Parameters["@AccountCode"].Value = invoiceListSR.AccountCode;
				cmd.Parameters["@CostCenterCode"].Value = invoiceListSR.CostCenterCode;
				cmd.Parameters["@ShipToID"].Value = invoiceListSR.ShipToID;
				cmd.Parameters["@SupplierID"].Value = invoiceListSR.SupplierID;
				cmd.Parameters["@ProductID"].Value = invoiceListSR.ProductID;

				if (invoiceListSR.StartDateSet)
				{
					cmd.Parameters["@StartDate"].Value = this.GetDateValue(invoiceListSR.StartDate);
				}
				else
				{
					cmd.Parameters["@StartDate"].Value = string.Empty;
				}

				if (invoiceListSR.EndDateSet)
				{
					cmd.Parameters["@EndDate"].Value = this.GetDateValue(invoiceListSR.EndDate);
				}
				else
				{
					cmd.Parameters["@EndDate"].Value = string.Empty;
				}

				if (!string.IsNullOrEmpty(invoiceListSR.SortExpression))
				{
					cmd.Parameters["@SortExpression"].Value = "ORDER BY " + invoiceListSR.SortExpression;
				}
				else
				{
					cmd.Parameters["@SortExpression"].Value = string.Empty;
				}
			}
			else
			{
				cmd.Parameters["@InvoiceNumber"].Value = invoiceListSR.InvoiceNumber;
				cmd.Parameters["@AccountCode"].Value = string.Empty;
				cmd.Parameters["@CostCenterCode"].Value = string.Empty;
				cmd.Parameters["@ShipToID"].Value = string.Empty;
				cmd.Parameters["@SupplierID"].Value = string.Empty;
				cmd.Parameters["@ProductID"].Value = string.Empty;
				cmd.Parameters["@StartDate"].Value = string.Empty;
				cmd.Parameters["@EndDate"].Value = string.Empty;
				cmd.Parameters["@SortExpression"].Value = "ORDER BY " + invoiceListSR.SortExpression;
			}

			cmd.Parameters["@InvoiceType"].Value = System.Convert.ToInt32(invoiceListSR.InvoiceType);
			cmd.Parameters["@LoginSiteGuid"].Value = invoiceListSR.Security.LoginSiteGuid;
			cmd.Parameters["@SiteGuid"].Value = invoiceListSR.Security.SiteGuid;
			cmd.Parameters["@UserGuid"].Value = invoiceListSR.Security.UserGuid;
			cmd.Parameters["@DeleteFlag"].Value = InvoiceListDO.NOT_DELETED;
		}

		/// <summary>
		/// This method will return a select statement that retrieves a list of
		/// account codes.
		/// </summary>
		/// <returns></returns>
		public string GetAccountCodeSelectCommand()
		{
			string sql = "";
			return sql;
		}

		/// <summary>
		/// This method will return a select statement that retrieves a list of
		/// cost center codes.
		/// </summary>
		/// <returns></returns>
		public string GetCostCenterCodeSelectCommand()
		{
			string sql = "";
			return sql;
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method will initialize the object to its initial state.
		/// </summary>
		private void Initialize()
		{
			this.lineItems = new BaseCollections();
			this.accountCodeList = new ArrayList();
			this.costCenterCodeList = new ArrayList();
			this.productList = new ArrayList();
		}

		/// <summary>
		/// This method will return the transaction type in string format. The default is
		/// set to 21 (account payable invoice).
		/// </summary>
		/// <param name="transType"></param>
		/// <returns></returns>
		private string ConvertTransType(TransactionTypes transType)
		{
			string transTypeStr = "21";

			try
			{
				transTypeStr = (System.Convert.ToInt16(transType)).ToString();
			}
			catch (Exception)
			{
			}

			return transTypeStr;
		}
		/// <summary>
		/// This method will return the date in a YYYY-MM-DD string
		/// format. If the Date is null an empty string is returned.
		/// </summary>
		/// <param name="inDate"></param>
		/// <returns></returns>
		private string GetDateValue(DateTimeOffset inDate)
		{
			string strDate = "";
			strDate = this.FormattedDate(inDate);

			return strDate;
		}

		/// <summary>
		/// This method will return the date in YYYY-MM-DD string format.
		/// </summary>
		/// <param name="inDate"></param>
		/// <returns></returns>
		private string FormattedDate(DateTimeOffset inDate)
		{
			string outDate = inDate.Year.ToString() + "-";

			int month = inDate.Month;
			if (month < 10)
			{
				outDate = outDate + "0" + month.ToString() + "-";
			}
			else
			{
				outDate = outDate + month.ToString() + "-";
			}

			int day = inDate.Day;
			if (day < 10)
			{
				outDate = outDate + "0" + day.ToString();
			}
			else
			{
				outDate = outDate + day.ToString();
			}

			return outDate;
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
	}
}

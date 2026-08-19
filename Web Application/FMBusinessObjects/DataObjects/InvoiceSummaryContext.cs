using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
	[DataContract]
   [Serializable]
	public class InvoiceSummaryContext
	{
		#region Private data members
		[DataMember]
		private string product;
		[DataMember]
		private string accountCode;
		[DataMember]
		private string costCentreCode;
		[DataMember]
		private string invoiceNumber;
		[DataMember]
		private string shipTo;
		[DataMember]
		private string shipToTip;
		[DataMember]
		private string supplier;
		[DataMember]
		private string supplierTip;
		[DataMember]
		private string sortExpression;

		[DataMember]
		private DateTimeOffset startDate;
		[DataMember]
		private DateTimeOffset endDate;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the invoice summary context class.
		/// </summary>
		public InvoiceSummaryContext()
		{
			this.Initialize();
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property sets and gets the product data member.
		/// </summary>
		public string Product
		{
			get { return this.product; }
			set
			{
				if (value == null)
				{
					this.product = "";
				}
				else
				{
					this.product = value;
				}
			}
		}

		/// <summary>
		/// This property sets and gets the accounting code data member.
		/// </summary>
		public string AccountCode
		{
			get { return this.accountCode; }
			set
			{
				if (value == null)
				{
					this.accountCode = "";
				}
				else
				{
					this.accountCode = value;
				}
			}
		}

		/// <summary>
		/// This property sets and gets the cost center code data member.
		/// </summary>
		public string CostCentreCode
		{
			get { return this.costCentreCode; }
			set
			{
				if (value == null)
				{
					this.costCentreCode = "";
				}
				else
				{
					this.costCentreCode = value;
				}
			}
		}

		/// <summary>
		/// This property sets and gets the invoice number data member.
		/// </summary>
		public string InvoiceNumber
		{
			get { return this.invoiceNumber; }
			set
			{
				if (value == null)
				{
					this.invoiceNumber = "";
				}
				else
				{
					this.invoiceNumber = value;
				}
			}
		}

		/// <summary>
		/// This property sets and gets the ship-to data member.
		/// </summary>
		public string ShipTo
		{
			get { return this.shipTo; }
			set
			{
				if (value == null)
				{
					this.shipTo = "";
				}
				else
				{
					this.shipTo = value;
				}
			}
		}

		/// <summary>
		/// This property sets and gets the ship-to tip data member.
		/// </summary>
		public string ShipToTip
		{
			get { return this.shipToTip; }
			set
			{
				if (value == null)
				{
					this.shipToTip = "";
				}
				else
				{
					this.shipToTip = value;
				}
			}
		}

		/// <summary>
		/// This property sets and gets the supplier data member.
		/// </summary>
		public string Supplier
		{
			get { return this.supplier; }
			set
			{
				if (value == null)
				{
					this.supplier = "";
				}
				else
				{
					this.supplier = value;
				}
			}
		}

		/// <summary>
		/// This property sets and gets the supplier tip data member.
		/// </summary>
		public string SupplierTip
		{
			get { return this.supplierTip; }
			set
			{
				if (value == null)
				{
					this.supplierTip = "";
				}
				else
				{
					this.supplierTip = value;
				}
			}
		}

		/// <summary>
		/// This property sets and gets the start date data member.
		/// </summary>
		public DateTimeOffset StartDate
		{
			get { return this.startDate; }
			set { this.startDate = value; }
		}

		/// <summary>
		/// This property sets and gets the end date data member.
		/// </summary>
		public DateTimeOffset EndDate
		{
			get { return this.endDate; }
			set { this.endDate = value; }
		}

		/// <summary>
		/// This property sets and gets the sort expression data member.
		/// </summary>
		public string SortExpression
		{
			get { return this.sortExpression; }
			set
			{
				if (value == null)
				{
					this.sortExpression = "";
				}
				else
				{
					this.sortExpression = value;
				}
			}
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method set the invoice summary context object to its initial state.
		/// </summary>
		private void Initialize()
		{
			this.product = "";
			this.accountCode = "";
			this.costCentreCode = "";
			this.invoiceNumber = "";
			this.shipTo = "";
			this.shipToTip = "";
			this.supplier = "";
			this.supplierTip = "";
			this.sortExpression = "";
		}
		#endregion
	}
}

/// <summary>
/// File name:	InvoiceListSR.cs
/// Purpose:	The purpose of the Invoice Service Request is to request invoice data
///				from the invoice summary processor.
///				
///	Comments:	Copyright (C) Varec, Inc. Norcross, GA, USA, 
///				2005.  This file shall not be copied or reproduced in any form 
///				without the express written consent of Endress+Hauser.
///				
///	Author(s):	Richard R. Panachida
///	Version:	1.0.0  Current version
///	
///	Modification History:
///		Date:			By:						Reason:
///		----------	--------------------	----------------------------------
///		2008-12-17  Richard Panachida    Updated for defect 865.
///		2009-02-24  A. Coker             Moved INVOICE_... constants here from InvoiceSummary.aspx.cs.
///		
/// </summary>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using FMBusinessObjects.DataObjects;

namespace FMBusinessObjects.ServiceRequests
{
    [Serializable]
    [DataContract]
	[KnownType(typeof(AccountingSite))]
	public class InvoiceListSR : AccountingServiceRequest
	{
		#region Public data members
		public enum RequestTypes { GET_HEADER_DATA, GET_DETAIL, NONE };
		public const string INVOICE_PAYABLE = "21";
		public const string INVOICE_RECEIVABLE = "22";
		public const string INVOICE_NONE = "99";

		#endregion

		#region Private data members

		[DataMember]
		private RequestTypes subRequest;

		[DataMember]
		private string allText;
		[DataMember]
		private string invoiceNumber;
		[DataMember]
		private string invoiceType;
		[DataMember]
		private string accountCode;
		[DataMember]
		private string costCenterCode;
		[DataMember]
		private string productID;
		[DataMember]
		private string shipToID;
		[DataMember]
		private string supplierID;
		[DataMember]
		private string sortExpression;

		[DataMember]
		private bool startDateSet;
		[DataMember]
		private bool endDateSet;

		[DataMember]
		private DateTimeOffset startDate;
		[DataMember]
		private DateTimeOffset endDate;

		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the InvoiceListSR class.
		/// </summary>
		public InvoiceListSR()
		{
			this.Initialize();
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property gets and sets the sub-request type for invoices.
		/// </summary>
		public RequestTypes SubRequest
		{
			get { return this.subRequest; }
			set { this.subRequest = value; }
		}

		/// <summary>
		/// This property stores the AccountingSite for the request
		/// </summary>
		[DataMember]
		public object AccountingSite
		{
			get;
			set;
		}

		/// <summary>
		/// This property gets and sets the All Text string.  Defaults to "{All}".
		/// </summary>
		public string AllText
		{
			get { return this.allText; }
			set
			{
				if ((value == null) || (value.Length <= 0))
				{
					this.allText = "{All}";
				}
				else
				{
					this.allText = value;
				}
			}
		}

		/// <summary>
		/// This property gets and sets the invoice number.  The default is an
		/// empty string.
		/// </summary>
		public string InvoiceNumber
		{
			get { return this.invoiceNumber; }
			set
			{
				if ((value == null) || (value.Length <= 0))
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
		/// This property gets and sets the invoice type.  The default is an
		/// empty string.
		/// </summary>
		public string InvoiceType
		{
			get { return this.invoiceType; }
			set
			{
				if ((value == null) || (value.Length <= 0))
				{
					try
					{
						this.invoiceType = System.Convert.ToInt32(TransactionTypes.T21_AccountPayableInvoice).ToString();
					}
					catch (Exception)
					{
						this.invoiceType = "21";
					}
				}
				else
				{
					this.invoiceType = value;
				}
			}
		}

		/// <summary>
		/// This property gets and sets the Account Code.  The default is an
		/// empty string.
		/// </summary>
		public string AccountCode
		{
			get { return this.accountCode; }
			set
			{
				if ((value == null) || (value.Length <= 0))
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
		/// This property gets and sets the Cost Center Code.  The default is an
		/// empty string.
		/// </summary>
		public string CostCenterCode
		{
			get { return this.costCenterCode; }
			set
			{
				if ((value == null) || (value.Length <= 0))
				{
					this.costCenterCode = "";
				}
				else
				{
					this.costCenterCode = value;
				}
			}
		}

		/// <summary>
		/// This property gets and sets the Product ID.  The default is an
		/// empty string.
		/// </summary>
		public string ProductID
		{
			get { return this.productID; }
			set
			{
				if ((value == null) || (value.Length <= 0))
				{
					this.productID = "";
				}
				else
				{
					this.productID = value;
				}
			}
		}

		/// <summary>
		/// This property gets and sets the Ship-To ID.  The default is an
		/// empty string.
		/// </summary>
		public string ShipToID
		{
			get { return this.shipToID; }
			set
			{
				if ((value == null) || (value.Length <= 0))
				{
					this.shipToID = "";
				}
				else
				{
					this.shipToID = value;
				}
			}
		}

		/// <summary>
		/// This property gets and sets the Supplier ID.  The default is an
		/// empty string.
		/// </summary>
		public string SupplierID
		{
			get { return this.supplierID; }
			set
			{
				if ((value == null) || (value.Length <= 0))
				{
					this.supplierID = "";
				}
				else
				{
					this.supplierID = value;
				}
			}
		}

		/// <summary>
		/// This property gets and sets the sort expression.  The default is an
		/// empty string.
		/// </summary>
		public string SortExpression
		{
			get { return this.sortExpression; }
			set
			{
				if ((value == null) || (value.Length <= 0))
				{
					this.sortExpression = "";
				}
				else
				{
					this.sortExpression = value;
				}
			}
		}

		/// <summary>
		/// This property gets and sets the Start date.  The default is an
		/// empty string.
		/// </summary>
		public DateTimeOffset StartDate
		{
			get { return this.startDate; }
			set
			{
				this.startDate = value;
				this.startDateSet = true;
			}
		}

		/// <summary>
		/// This property gets and sets the End date.  The default is an
		/// empty string.
		/// </summary>
		public DateTimeOffset EndDate
		{
			get { return this.endDate; }
			set
			{
				this.endDate = value;
				this.endDateSet = true;
			}
		}

		/// <summary>
		/// This property gets and sets the Start date set flag.  If the date
		/// is set, then the flag is true.
		/// </summary>
		public bool StartDateSet
		{
			get { return this.startDateSet; }
			set { this.startDateSet = value; }
		}

		/// <summary>
		/// This property gets and sets the End date set flag.  If the date
		/// is set, then the flag is true.
		/// </summary>
		public bool EndDateSet
		{
			get { return this.endDateSet; }
			set { this.endDateSet = value; }
		}

		#endregion

		#region Private methods
		/// <summary>
		/// This method initializes the object to its initial state.
		/// </summary>
		private void Initialize()
		{
			this.subRequest = InvoiceListSR.RequestTypes.NONE;
			this.allText = "{All}";
			this.invoiceNumber = "";
			this.invoiceType = "";
			this.accountCode = "";
			this.costCenterCode = "";
			this.productID = "";
			this.shipToID = "";
			this.supplierID = "";
			this.sortExpression = "";
			this.startDateSet = false;
			this.endDateSet = false;
			base.Security = null;
		}
		#endregion
	}
}

/// <summary>
/// File name:	InvoiceListLineItemDO.cs
/// Purpose:	The purpose of the Invoice List Line Item data object is to store
///				invoice line item information.
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
///		2009-02-24  A. Coker             Updated code so that Account Code and Cost Centre Code filters are populated.
///		2009-03-11  Richard Panachida    Change Request 1903: Updated the List for the grid to handle the Rebate.
///		
/// </summary>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
	[DataContract]
   [Serializable]
	public class InvoiceListLineItemDO : BaseLineItemDO
	{
		#region Private data members

		[DataMember]
		private string transID = "";
		[DataMember]
		private string invoiceNumber = "";
		[DataMember]
		private string invoiceLineNumber = "";
		[DataMember]
		private string poNumber = "";  // Order number
		[DataMember]
		private string exciseTax = "";  // Tax1
		[DataMember]
		private string gstTax = "";  // Tax2
		[DataMember]
		private string legacyNumber = "";  // Payment (ROMAN) number
		[DataMember]
		private string accountNumber = "";  // Account code
		[DataMember]
		private string costCenterNumber = "";  // Cost Center code
		[DataMember]
		private string transactionAlias = "";
		[DataMember]
		private string owner = "";
		[DataMember]
		private string manager = "";
		[DataMember]
		private string shipTo = "";
		[DataMember]
		private string supplier = "";
		[DataMember]
		private string batchNumber = "";
		[DataMember]
		private string shipmentNumber = ""; // Receipt Number
		[DataMember]
		private string grossQuantity = "";
		[DataMember]
		private string netQuantity = "";
		[DataMember]
		private string product = "";
		[DataMember]
		private string productPrice = "";
		[DataMember]
		private string totalAmount = "";
		[DataMember]
		private string rebateFlag = ""; // Rebate Flag
		[DataMember]
		private string userData1 = "";
		[DataMember]
		private string userData2 = "";
		[DataMember]
		private string userData3 = "";
		[DataMember]
		private string userData4 = "";
		[DataMember]
		private string userData5 = "";
		[DataMember]
		private string userData6 = "";
		[DataMember]
		private string userData7 = "";
		[DataMember]
		private string userData8 = "";
		[DataMember]
		private string userData9 = "";
		[DataMember]
		private string userData10 = "";
		[DataMember]
		private string userData11 = "";
		[DataMember]
		private string userData12 = "";
		[DataMember]
		private string userData13 = "";
		[DataMember]
		private string userData14 = "";
		[DataMember]
		private string userData15 = "";
		[DataMember]
		private string userData16 = "";
		[DataMember]
		private string userData17 = "";
		[DataMember]
		private string userData18 = "";
		[DataMember]
		private string userData19 = "";
		[DataMember]
		private string userData20 = "";
		[DataMember]
		private string userData21 = "";
		[DataMember]
		private string userData22 = "";
		[DataMember]
		private string userData23 = "";
		[DataMember]
		private string userData24 = "";

		[DataMember]
		private DateTimeOffset transactionDateTime;
		[DataMember]
		private DateTime inventoryDate;

		[DataMember]
		private TransactionStatus transactionStatus;

		[DataMember]
		private string shipToName = "";
		[DataMember]
		private string shipToAddress = "";
		[DataMember]
		private string shipToCity = "";
		[DataMember]
		private string shipToState = "";

		[DataMember]
		private string supplierName = "";
		[DataMember]
		private string supplierAddress = "";
		[DataMember]
		private string supplierCity = "";
		[DataMember]
		private string supplierState = "";

		[DataMember]
		private string documentNumber = "";
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the invoice list line item data object.
		/// </summary>
		public InvoiceListLineItemDO ( )
		{
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property sets and gets the transaction ID data member.
		/// </summary>
		public string TransID
		{
			get { return this.transID; }
			set
			{
				if (value != null)
				{
					this.transID = value;
				}
			}
		}

		/// <summary>
		/// This property sets and gets the ship-to name data member.
		/// </summary>
		public string ShipToName
		{
			get { return this.shipToName; }
			set
			{
				if (value != null)
				{
					this.shipToName = value;
				}
			}
		}

		/// <summary>
		/// This property sets and gets the ship-to address data member.
		/// </summary>
		public string ShipToAddress
		{
			get { return this.shipToAddress; }
			set
			{
				if (value != null)
				{
					this.shipToAddress = value;
				}
			}
		}

		/// <summary>
		/// This property sets and gets the ship-to city data member.
		/// </summary>
		public string ShipToCity
		{
			get { return this.shipToCity; }
			set
			{
				if (value != null)
				{
					this.shipToCity = value;
				}
			}
		}

		/// <summary>
		/// This property sets and gets the ship-to state data member.
		/// </summary>
		public string ShipToState
		{
			get { return this.shipToState; }
			set
			{
				if (value != null)
				{
					this.shipToState = value;
				}
			}
		}

		/// <summary>
		/// This property sets and gets the supplier name data member.
		/// </summary>
		public string SupplierName
		{
			get { return this.supplierName; }
			set
			{
				if (value != null)
				{
					this.supplierName = value;
				}
			}
		}

		/// <summary>
		/// This property sets and gets the supplier address data member.
		/// </summary>
		public string SupplierAddress
		{
			get { return this.supplierAddress; }
			set
			{
				if (value != null)
				{
					this.supplierAddress = value;
				}
			}
		}

		/// <summary>
		/// This property sets and gets the supplier city data member.
		/// </summary>
		public string SupplierCity
		{
			get { return this.supplierCity; }
			set
			{
				if (value != null)
				{
					this.supplierCity = value;
				}
			}
		}

		/// <summary>
		/// This property sets and gets the supplier state data member.
		/// </summary>
		public string SupplierState
		{
			get { return this.supplierState; }
			set
			{
				if (value != null)
				{
					this.supplierState = value;
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
				this.invoiceNumber = value;

				if (value == null)
				{
					this.invoiceNumber = "";
				}
			}
		}

		/// <summary>
		/// This property sets and gets the invoice line number data member.
		/// </summary>
		public string InvoiceLineNumber
		{
			get { return this.invoiceLineNumber; }
			set
			{
				this.invoiceLineNumber = value;

				if (value == null)
				{
					this.invoiceLineNumber = "";
				}
			}
		}

		/// <summary>
		/// This property sets and gets the order number data member.
		/// </summary>
		public string OrderNumber
		{
			get { return this.poNumber; }
			set
			{
				this.poNumber = value;

				if (value == null)
				{
					this.poNumber = "";
				}
			}
		}

		/// <summary>
		/// This property sets and gets the excise tax (tax1) data member.
		/// </summary>
		public string Excise
		{
			get { return this.exciseTax; }
			set
			{
				this.exciseTax = value;

				if (value == null)
				{
					this.exciseTax = "";
				}
			}
		}

		/// <summary>
		/// This property sets and gets the GST tax (tax2) data member.
		/// </summary>
		public string GST
		{
			get { return this.gstTax; }
			set
			{
				this.gstTax = value;

				if (value == null)
				{
					this.gstTax = "";
				}
			}
		}

		/// <summary>
		/// This property sets and gets the payment number (legacy number) data member.
		/// </summary>
		public string PaymentNumber
		{
			get { return this.legacyNumber; }
			set
			{
				this.legacyNumber = value;

				if (value == null)
				{
					this.legacyNumber = "";
				}
			}
		}

		/// <summary>
		/// This property sets and gets the account code or number data member.
		/// </summary>
		public string AccountCode
		{
			get { return this.accountNumber; }
			set
			{
				this.accountNumber = value;

				if (value == null)
				{
					this.accountNumber = "";
				}
			}
		}

		/// <summary>
		/// This property sets and gets the cost center code or number data member.
		/// </summary>
		public string CostCentreCode
		{
			get { return this.costCenterNumber; }
			set
			{
				this.costCenterNumber = value;

				if (value == null)
				{
					this.costCenterNumber = "";
				}
			}
		}

		/// <summary>
		/// This property sets and gets the transaction alias data member.
		/// </summary>
		public string TransactionAlias
		{
			get { return this.transactionAlias; }
			set
			{
				this.transactionAlias = value;

				if (value == null)
				{
					this.transactionAlias = "";
				}
			}
		}

		/// <summary>
		/// This property sets and gets the owner data member.
		/// </summary>
		public string Owner
		{
			get { return this.owner; }
			set
			{
				this.owner = value;

				if (value == null)
				{
					this.owner = "";
				}
			}
		}

		/// <summary>
		/// This property sets and gets the manager data member.
		/// </summary>
		public string Manager
		{
			get { return this.manager; }
			set
			{
				this.manager = value;

				if (value == null)
				{
					this.manager = "";
				}
			}
		}

		/// <summary>
		/// This property sets and gets the ship-to data member.
		/// </summary>
		public string ShipToID
		{
			get { return this.shipTo; }
			set
			{
				this.shipTo = value;

				if (value == null)
				{
					this.shipTo = "";
				}
			}
		}

		/// <summary>
		/// This property sets and gets the supplier data member.
		/// </summary>
		public string SupplierID
		{
			get { return this.supplier; }
			set
			{
				this.supplier = value;

				if (value == null)
				{
					this.supplier = "";
				}
			}
		}

		/// <summary>
		/// This property sets and gets the batch number data member.
		/// </summary>
		public string BatchNumber
		{
			get { return this.batchNumber; }
			set
			{
				this.batchNumber = value;

				if (value == null)
				{
					this.batchNumber = "";
				}
			}
		}

		/// <summary>
		/// This property sets and gets the shipment number (receipt number)
		/// data member.  The default is set to an empty string.
		/// </summary>
		public string ShipmentNumber
		{
			get { return this.shipmentNumber; }
			set
			{
				this.shipmentNumber = value;

				if (value == null)
				{
					this.shipmentNumber = "";
				}
			}
		}

		/// <summary>
		/// This property sets and gets the gross quantity data member.
		/// </summary>
		public string GrossQuantity
		{
			get { return this.grossQuantity; }
			set
			{
				this.grossQuantity = value;

				if (value == null)
				{
					this.grossQuantity = "";
				}
			}
		}

		/// <summary>
		/// This property sets and gets the net quantity data member.
		/// </summary>
		public string NetQuantity
		{
			get { return this.netQuantity; }
			set
			{
				this.netQuantity = value;

				if (value == null)
				{
					this.netQuantity = "";
				}
			}
		}

		/// <summary>
		/// This property sets and gets the product data member.
		/// </summary>
		public string Product
		{
			get { return this.product; }
			set
			{
				this.product = value;

				if (value == null)
				{
					this.product = "";
				}
			}
		}

		/// <summary>
		/// This property sets and gets the product price data member.
		/// </summary>
		public string ProductPrice
		{
			get { return this.productPrice; }
			set
			{
				this.productPrice = value;

				if (value == null)
				{
					this.productPrice = "";
				}
			}
		}

		/// <summary>
		/// This property sets and gets the total amount (price * quantity) data member.
		/// </summary>
		public string TotalAmount
		{
			get { return this.totalAmount; }
			set
			{
				this.totalAmount = value;

				if (value == null)
				{
					this.totalAmount = "";
				}
			}
		}

		/// <summary>
		/// This property sets and gets the Rebate flag data member.
		/// </summary>
		public string Rebate
		{
			get { return this.rebateFlag; }
			set { this.rebateFlag = value; }
		}

		/// <summary>
		/// This property sets and gets the userdata data member.
		/// </summary>
		public string UserData1
		{
			get { return this.userData1; }
			set
			{
				this.userData1 = value;

				if (value == null)
				{
					this.userData1 = "";
				}
			}
		}

		/// <summary>
		/// This property sets and gets the userdata data member.
		/// </summary>
		public string UserData2
		{
			get { return this.userData2; }
			set
			{
				this.userData2 = value;

				if (value == null)
				{
					this.userData2 = "";
				}
			}
		}

		/// <summary>
		/// This property sets and gets the userdata data member.
		/// </summary>
		public string UserData3
		{
			get { return this.userData3; }
			set
			{
				this.userData3 = value;

				if (value == null)
				{
					this.userData3 = "";
				}
			}
		}

		/// <summary>
		/// This property sets and gets the userdata data member.
		/// </summary>
		public string UserData4
		{
			get { return this.userData4; }
			set
			{
				this.userData4 = value;

				if (value == null)
				{
					this.userData4 = "";
				}
			}
		}

		/// <summary>
		/// This property sets and gets the userdata data member.
		/// </summary>
		public string UserData5
		{
			get { return this.userData5; }
			set
			{
				this.userData5 = value;

				if (value == null)
				{
					this.userData5 = "";
				}
			}
		}

		/// <summary>
		/// This property sets and gets the userdata data member.
		/// </summary>
		public string UserData6
		{
			get { return this.userData6; }
			set
			{
				this.userData6 = value;

				if (value == null)
				{
					this.userData6 = "";
				}
			}
		}

		/// <summary>
		/// This property sets and gets the userdata data member.
		/// </summary>
		public string UserData7
		{
			get { return this.userData7; }
			set
			{
				this.userData7 = value;

				if (value == null)
				{
					this.userData7 = "";
				}
			}
		}

		/// <summary>
		/// This property sets and gets the userdata data member.
		/// </summary>
		public string UserData8
		{
			get { return this.userData8; }
			set
			{
				this.userData8 = value;

				if (value == null)
				{
					this.userData8 = "";
				}
			}
		}

		/// <summary>
		/// This property sets and gets the userdata data member.
		/// </summary>
		public string UserData9
		{
			get { return this.userData9; }
			set
			{
				this.userData9 = value;

				if (value == null)
				{
					this.userData9 = "";
				}
			}
		}

		/// <summary>
		/// This property sets and gets the userdata data member.
		/// </summary>
		public string UserData10
		{
			get { return this.userData10; }
			set
			{
				this.userData10 = value;

				if (value == null)
				{
					this.userData10 = "";
				}
			}
		}

		/// <summary>
		/// This property sets and gets the userdata data member.
		/// </summary>
		public string UserData11
		{
			get { return this.userData11; }
			set
			{
				this.userData11 = value;

				if (value == null)
				{
					this.userData11 = "";
				}
			}
		}

		/// <summary>
		/// This property sets and gets the userdata data member.
		/// </summary>
		public string UserData12
		{
			get { return this.userData12; }
			set
			{
				this.userData12 = value;

				if (value == null)
				{
					this.userData12 = "";
				}
			}
		}

		/// <summary>
		/// This property sets and gets the userdata data member.
		/// </summary>
		public string UserData13
		{
			get { return this.userData13; }
			set
			{
				this.userData13 = value;

				if (value == null)
				{
					this.userData13 = "";
				}
			}
		}

		/// <summary>
		/// This property sets and gets the userdata data member.
		/// </summary>
		public string UserData14
		{
			get { return this.userData14; }
			set
			{
				this.userData14 = value;

				if (value == null)
				{
					this.userData14 = "";
				}
			}
		}

		/// <summary>
		/// This property sets and gets the userdata data member.
		/// </summary>
		public string UserData15
		{
			get { return this.userData15; }
			set
			{
				this.userData15 = value;

				if (value == null)
				{
					this.userData15 = "";
				}
			}
		}

		/// <summary>
		/// This property sets and gets the userdata data member.
		/// </summary>
		public string UserData16
		{
			get { return this.userData16; }
			set
			{
				this.userData16 = value;

				if (value == null)
				{
					this.userData16 = "";
				}
			}
		}

		/// <summary>
		/// This property sets and gets the userdata data member.
		/// </summary>
		public string UserData17
		{
			get { return this.userData17; }
			set
			{
				this.userData17 = value;

				if (value == null)
				{
					this.userData17 = "";
				}
			}
		}

		/// <summary>
		/// This property sets and gets the userdata data member.
		/// </summary>
		public string UserData18
		{
			get { return this.userData18; }
			set
			{
				this.userData18 = value;

				if (value == null)
				{
					this.userData18 = "";
				}
			}
		}

		/// <summary>
		/// This property sets and gets the userdata data member.
		/// </summary>
		public string UserData19
		{
			get { return this.userData19; }
			set
			{
				this.userData19 = value;

				if (value == null)
				{
					this.userData19 = "";
				}
			}
		}

		/// <summary>
		/// This property sets and gets the userdata data member.
		/// </summary>
		public string UserData20
		{
			get { return this.userData20; }
			set
			{
				this.userData20 = value;

				if (value == null)
				{
					this.userData20 = "";
				}
			}
		}

		/// <summary>
		/// This property sets and gets the userdata data member.
		/// </summary>
		public string UserData21
		{
			get { return this.userData21; }
			set
			{
				this.userData21 = value;

				if (value == null)
				{
					this.userData21 = "";
				}
			}
		}

		/// <summary>
		/// This property sets and gets the userdata data member.
		/// </summary>
		public string UserData22
		{
			get { return this.userData22; }
			set
			{
				this.userData22 = value;

				if (value == null)
				{
					this.userData22 = "";
				}
			}
		}

		/// <summary>
		/// This property sets and gets the userdata data member.
		/// </summary>
		public string UserData23
		{
			get { return this.userData23; }
			set
			{
				this.userData23 = value;

				if (value == null)
				{
					this.userData23 = "";
				}
			}
		}

		/// <summary>
		/// This property sets and gets the userdata data member.
		/// </summary>
		public string UserData24
		{
			get { return this.userData24; }
			set
			{
				this.userData24 = value;

				if (value == null)
				{
					this.userData24 = "";
				}
			}
		}

		/// <summary>
		/// This property sets and gets the transaction date/time data member.
		/// </summary>
		public DateTimeOffset TransactionDateTime
		{
			get { return this.transactionDateTime; }
			set { this.transactionDateTime = value; }
		}

		/// <summary>
		/// This property sets and gets the inventory date data member.
		/// </summary>
		public DateTime InventoryDate
		{
			get { return this.inventoryDate; }
			set { this.inventoryDate = value; }
		}

		/// <summary>
		/// This property sets and gets the transaction status data member.
		/// </summary>
		public TransactionStatus TransactionStatus
		{
			get { return this.transactionStatus; }
			set { this.transactionStatus = value; }
		}

		/// <summary>
		/// This property sets and gets the ship-to address data member.
		/// </summary>
		public string DocumentNumber
		{
			get { return this.documentNumber; }
			set
			{
				this.documentNumber = value;

				if (value == null)
				{
					this.documentNumber = "";
				}
			}
		}
		#endregion

		#region Override methods
		public override string getDeleteCommand ( )
		{
			return null;
		}

		public override string getInsertCommand ( )
		{
			return null;
		}

		public override string getSelectCommand ( )
		{
			return null;
		}

		public override string getUpdateCommand ( )
		{
			return null;
		}
		#endregion
	}
}

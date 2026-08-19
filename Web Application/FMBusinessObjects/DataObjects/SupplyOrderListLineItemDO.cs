using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
   [Serializable]
   [DataContract]
	public class SupplyOrderListLineItemDO : BaseLineItemDO
	{
		#region Private data members
		[DataMember]
		private string sTransID = "";
		[DataMember]
		private string sOrderStatus = "";
		[DataMember]
		private string sTransactionDate = "";
		[DataMember]
		private string sInventoryDate = "";
		[DataMember]
		private string sDocumentNumber = "";
		[DataMember]
		private string sTransactionAlias = "";
		[DataMember]
		private string sPONumber = "";
		[DataMember]
		private string sSiteID = "";
		[DataMember]
		private string sSupplierID = "";
		[DataMember]
		private string sManager = "";
		[DataMember]
		private string sOwner = "";
		[DataMember]
		private string sBillToID = "";
		[DataMember]
		private string sShipperID = "";
		[DataMember]
		private string sShipToID = "";
		[DataMember]
		private string sCarrierID = "";
		[DataMember]
		private string sConfirmationNumber = "";
		[DataMember]
		private string sStandingOfferNumber = "";
		[DataMember]
		private string sRequiredDate = "";
		[DataMember]
		private string sEstimatedDateFrom = "";
		[DataMember]
		private string sEstimatedDateTo = "";

		[DataMember]
		private DateTimeOffset transactionDateTime;
		[DataMember]
		private DateTimeOffset inventoryDateTime;
		[DataMember]
		private DateTimeOffset requiredDeliveryDateTime;
		[DataMember]
		private DateTimeOffset estimatedDeliveryDateFromTime;
		[DataMember]
		private DateTimeOffset estimatedDeliveryDateToTime;

		[DataMember]
		private TransactionStatus eTransactionStatus;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the supply order list line
		/// item data object class.
		/// </summary>
		public SupplyOrderListLineItemDO ( )
		{
		}
		#endregion

		#region Override methods
		public override string getSelectCommand ( )
		{
			return null;
		}

		public override string getInsertCommand ( )
		{
			return null;
		}

		public override string getUpdateCommand ( )
		{
			return null;
		}

		public override string getDeleteCommand ( )
		{
			return null;
		}
		#endregion

		#region properties

		public string TransactionAlias
		{
			get { return this.sTransactionAlias; }
			set { this.sTransactionAlias = value; }
		}

		public string TransactionID
		{
			get { return this.sTransID; }
			set { this.sTransID = value; }
		}

		public string OrderStatus
		{
			get { return this.sOrderStatus; }
			set { this.sOrderStatus = value; }
		}

		public string TransactionDate
		{
			get { return this.sTransactionDate; }
			set { this.sTransactionDate = value; }
		}

		public string InventoryDate
		{
			get { return this.sInventoryDate; }
			set { this.sInventoryDate = value; }
		}

		public string DocumentNumber
		{
			get { return this.sDocumentNumber; }
			set { this.sDocumentNumber = value; }
		}

		public DateTimeOffset TransactionDateTime
		{
			get { return this.transactionDateTime; }
			set { this.transactionDateTime = value; }
		}

		public DateTimeOffset InventoryDateTime
		{
			get { return this.inventoryDateTime; }
			set { this.inventoryDateTime = value; }
		}

		public string PONumber
		{
			get { return this.sPONumber; }
			set { this.sPONumber = value; }
		}

		public string SiteID
		{
			get { return this.sSiteID; }
			set { this.sSiteID = value; }
		}

		public string SupplierID
		{
			get { return this.sSupplierID; }
			set { this.sSupplierID = value; }
		}

		public string Manager
		{
			get { return this.sManager; }
			set { this.sManager = value; }
		}

		public string Owner
		{
			get { return this.sOwner; }
			set { this.sOwner = value; }
		}

		public string BillToID
		{
			get { return this.sBillToID; }
			set { this.sBillToID = value; }
		}

		public string ShipperID
		{
			get { return this.sShipperID; }
			set { this.sShipperID = value; }
		}

		public string ShipToID
		{
			get { return this.sShipToID; }
			set { this.sShipToID = value; }
		}

		public string CarrierID
		{
			get { return this.sCarrierID; }
			set { this.sCarrierID = value; }
		}

		public string ConfirmationNumber
		{
			get { return this.sConfirmationNumber; }
			set { this.sConfirmationNumber = value; }
		}

		public string StandingOfferNumber
		{
			get { return this.sStandingOfferNumber; }
			set { this.sStandingOfferNumber = value; }
		}

		public TransactionStatus TransactionStatus
		{
			get { return this.eTransactionStatus; }
			set { this.eTransactionStatus = value; }
		}

		public string RequiredDeliveryDate
		{
			get { return this.sRequiredDate; }
			set { this.sRequiredDate = value; }
		}

		public DateTimeOffset RequiredDeliveryDateTime
		{
			get { return this.requiredDeliveryDateTime; }
			set { this.requiredDeliveryDateTime = value; }
		}

		public string EstimatedDeliveryDateFrom
		{
			get { return this.sEstimatedDateFrom; }
			set { this.sEstimatedDateFrom = value; }
		}

		public DateTimeOffset EstimatedDeliveryDateFromTime
		{
			get { return this.estimatedDeliveryDateFromTime; }
			set { this.estimatedDeliveryDateFromTime = value; }
		}

		public string EstimatedDeliveryDateTo
		{
			get { return this.sEstimatedDateTo; }
			set { this.sEstimatedDateTo = value; }
		}

		public DateTimeOffset EstimatedDeliveryDateToTime
		{
			get { return this.estimatedDeliveryDateToTime; }
			set { this.estimatedDeliveryDateToTime = value; }
		}

		#endregion
	}
}

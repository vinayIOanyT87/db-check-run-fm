using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
   [Serializable]
   [DataContract]
	public class OrderListLineItemDO : BaseLineItemDO
	{
		#region private data members
	   [DataMember]
	   private bool bDeleteFlag = false;

		[DataMember]
		private string sTransID = "";
		[DataMember]
		private string sOrderStatus = "";
		[DataMember]
		private string sTransactionDate = "";
		[DataMember]
		private string sInventoryDate = "";
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
		private string sDocumentNumber = "";
		[DataMember]
		private string sTransactionAlias = "";
		[DataMember]
		private string sPONumber = "";
		[DataMember]
		private string sScheduledDate = "";
		[DataMember]
		private string sSiteID = "";
		[DataMember]
		private string sRequestedDeliveryDate = "";
		[DataMember]
		private string sShipmentNumber = "";
		[DataMember]
		private string sOperatorID = "";
		[DataMember]
		private string sDestRegistrationID1 = "";
		[DataMember]
		private string sDestRegistrationID2 = "";
		[DataMember]
		private string sDestRegistrationID3 = "";
		[DataMember]
		private string sUserData1 = "";
		[DataMember]
		private string sUserData2 = "";
		[DataMember]
		private string sUserData3 = "";
		[DataMember]
		private string sUserData4 = "";
		[DataMember]
		private string sUserData5 = "";
		[DataMember]
		private string sUserData6 = "";
		[DataMember]
		private string sUserData7 = "";
		[DataMember]
		private string sUserData8 = "";
		[DataMember]
		private string sUserData9 = "";
		[DataMember]
		private string sUserData10 = "";
		[DataMember]
		private string sUserData11 = "";
		[DataMember]
		private string sUserData12 = "";
		[DataMember]
		private string sUserData13 = "";
		[DataMember]
		private string sUserData14 = "";
		[DataMember]
		private string sUserData15 = "";
		[DataMember]
		private string sUserData16 = "";
		[DataMember]
		private string sUserData17 = "";
		[DataMember]
		private string sUserData18 = "";
		[DataMember]
		private string sUserData19 = "";
		[DataMember]
		private string sUserData20 = "";
		[DataMember]
		private string sUserData21 = "";
		[DataMember]
		private string sUserData22 = "";
		[DataMember]
		private string sUserData23 = "";
		[DataMember]
		private string sUserData24 = "";

		[DataMember]
		private DateTimeOffset dTransactionDateTime;
		[DataMember]
		private DateTimeOffset dInventoryDateTime;
		[DataMember]
		private DateTimeOffset dScheduledDateTime;
		[DataMember]
		private DateTimeOffset dEffectiveDateTime;
		[DataMember]
		private DateTimeOffset dExpirationDateTime;
		[DataMember]
		private DateTimeOffset dETADateTime;
		[DataMember]
		private DateTimeOffset dRequestedDeliveryDateTime;

		[DataMember]
		private TransactionStatus eTransactionStatus;

		[DataMember]
		private string sBillToName = "";
		[DataMember]
		private string sBillToAddress = "";
		[DataMember]
		private string sBillToCity = "";
		[DataMember]
		private string sBillToState = "";

		[DataMember]
		private string sShipToName = "";
		[DataMember]
		private string sShipToAddress = "";
		[DataMember]
		private string sShipToCity = "";
		[DataMember]
		private string sShipToState = "";

		[DataMember]
		private string sCarrierName = "";
		[DataMember]
		private string sCarrierAddress = "";
		[DataMember]
		private string sCarrierCity = "";
		[DataMember]
		private string sCarrierState = "";

		[DataMember]
		private string sEffectiveDate = "";
		[DataMember]
		private string sExpirationDate = "";
		[DataMember]
		private string sETA = "";
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the order list line item
		/// data object class.
		/// </summary>
		public OrderListLineItemDO ( )
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

		#region Properties
		public bool DeleteFlag
		{
			get { return this.bDeleteFlag; }
			set { this.bDeleteFlag = value; }
		}

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

		public string DocumentNumber
		{
			get { return this.sDocumentNumber; }
			set { this.sDocumentNumber = value; }
		}

		public DateTimeOffset TransactionDateTime
		{
			get { return this.dTransactionDateTime; }
			set { this.dTransactionDateTime = value; }
		}

		public DateTimeOffset InventoryDateTime
		{
			get { return this.dInventoryDateTime; }
			set { this.dInventoryDateTime = value; }
		}

		public string PONumber
		{
			get { return this.sPONumber; }
			set { this.sPONumber = value; }
		}

		public string ScheduledDate
		{
			get { return this.sScheduledDate; }
			set { this.sScheduledDate = value; }
		}

		public DateTimeOffset ScheduledDateTime
		{
			get { return this.dScheduledDateTime; }
			set { this.dScheduledDateTime = value; }
		}

		public string SiteID
		{
			get { return this.sSiteID; }
			set { this.sSiteID = value; }
		}

		public TransactionStatus TransactionStatus
		{
			get { return this.eTransactionStatus; }
			set { this.eTransactionStatus = value; }
		}

		public string BillToName
		{
			get { return this.sBillToName; }
			set { this.sBillToName = value; }
		}

		public string BillToAddress
		{
			get { return this.sBillToAddress; }
			set { this.sBillToAddress = value; }
		}

		public string BillToCity
		{
			get { return this.sBillToCity; }
			set { this.sBillToCity = value; }
		}

		public string BillToState
		{
			get { return this.sBillToState; }
			set { this.sBillToState = value; }
		}

		public string ShipToName
		{
			get { return this.sShipToName; }
			set { this.sShipToName = value; }
		}

		public string ShipToAddress
		{
			get { return this.sShipToAddress; }
			set { this.sShipToAddress = value; }
		}

		public string ShipToCity
		{
			get { return this.sShipToCity; }
			set { this.sShipToCity = value; }
		}

		public string ShipToState
		{
			get { return this.sShipToState; }
			set { this.sShipToState = value; }
		}

		public string CarrierName
		{
			get { return this.sCarrierName; }
			set { this.sCarrierName = value; }
		}

		public string CarrierAddress
		{
			get { return this.sCarrierAddress; }
			set { this.sCarrierAddress = value; }
		}

		public string CarrierCity
		{
			get { return this.sCarrierCity; }
			set { this.sCarrierCity = value; }
		}

		public string CarrierState
		{
			get { return this.sCarrierState; }
			set { this.sCarrierState = value; }
		}

		public string EffectiveDate
		{
			get { return this.sEffectiveDate; }
			set { this.sEffectiveDate = value; }
		}

		public DateTimeOffset EffectiveDateTime
		{
			get { return this.dEffectiveDateTime; }
			set { this.dEffectiveDateTime = value; }
		}

		public string ExpirationDate
		{
			get { return this.sExpirationDate; }
			set { this.sExpirationDate = value; }
		}

		public DateTimeOffset ExpirationDateTime
		{
			get { return this.dExpirationDateTime; }
			set { this.dExpirationDateTime = value; }
		}

		public string ETA
		{
			get { return this.sETA; }
			set { this.sETA = value; }
		}

		public DateTimeOffset ETADateTime
		{
			get { return this.dETADateTime; }
			set { this.dETADateTime = value; }
		}

		public string RequestedDeliveryDate
		{
			get { return this.sRequestedDeliveryDate; }
			set { this.sRequestedDeliveryDate = value; }
		}

		public DateTimeOffset RequestedDeliveryDateTime
		{
			get { return this.dRequestedDeliveryDateTime; }
			set { this.dRequestedDeliveryDateTime = value; }
		}

		public string ShipmentNumber
		{
			get { return this.sShipmentNumber; }
			set { this.sShipmentNumber = value; }
		}

		public string OperatorID
		{
			get { return this.sOperatorID; }
			set { this.sOperatorID = value; }
		}

		public string DestRegistrationID1
		{
			get { return this.sDestRegistrationID1; }
			set { this.sDestRegistrationID1 = value; }
		}

		public string DestRegistrationID2
		{
			get { return this.sDestRegistrationID2; }
			set { this.sDestRegistrationID2 = value; }
		}

		public string DestRegistrationID3
		{
			get { return this.sDestRegistrationID3; }
			set { this.sDestRegistrationID3 = value; }
		}

		public string UserData1
		{
			get { return this.sUserData1; }
			set { this.sUserData1 = value; }
		}

		public string UserData2
		{
			get { return this.sUserData2; }
			set { this.sUserData2 = value; }
		}

		public string UserData3
		{
			get { return this.sUserData3; }
			set { this.sUserData3 = value; }
		}

		public string UserData4
		{
			get { return this.sUserData4; }
			set { this.sUserData4 = value; }
		}

		public string UserData5
		{
			get { return this.sUserData5; }
			set { this.sUserData5 = value; }
		}

		public string UserData6
		{
			get { return this.sUserData6; }
			set { this.sUserData6 = value; }
		}

		public string UserData7
		{
			get { return this.sUserData7; }
			set { this.sUserData7 = value; }
		}

		public string UserData8
		{
			get { return this.sUserData8; }
			set { this.sUserData8 = value; }
		}

		public string UserData9
		{
			get { return this.sUserData9; }
			set { this.sUserData9 = value; }
		}

		public string UserData10
		{
			get { return this.sUserData10; }
			set { this.sUserData10 = value; }
		}

		public string UserData11
		{
			get { return this.sUserData11; }
			set { this.sUserData11 = value; }
		}

		public string UserData12
		{
			get { return this.sUserData12; }
			set { this.sUserData12 = value; }
		}

		public string UserData13
		{
			get { return this.sUserData13; }
			set { this.sUserData13 = value; }
		}

		public string UserData14
		{
			get { return this.sUserData14; }
			set { this.sUserData14 = value; }
		}

		public string UserData15
		{
			get { return this.sUserData15; }
			set { this.sUserData15 = value; }
		}

		public string UserData16
		{
			get { return this.sUserData16; }
			set { this.sUserData16 = value; }
		}

		public string UserData17
		{
			get { return this.sUserData17; }
			set { this.sUserData17 = value; }
		}

		public string UserData18
		{
			get { return this.sUserData18; }
			set { this.sUserData18 = value; }
		}

		public string UserData19
		{
			get { return this.sUserData19; }
			set { this.sUserData19 = value; }
		}

		public string UserData20
		{
			get { return this.sUserData20; }
			set { this.sUserData20 = value; }
		}

		public string UserData21
		{
			get { return this.sUserData21; }
			set { this.sUserData21 = value; }
		}

		public string UserData22
		{
			get { return this.sUserData22; }
			set { this.sUserData22 = value; }
		}

		public string UserData23
		{
			get { return this.sUserData23; }
			set { this.sUserData23 = value; }
		}

		public string UserData24
		{
			get { return this.sUserData24; }
			set { this.sUserData24 = value; }
		}

		#endregion
	}
}

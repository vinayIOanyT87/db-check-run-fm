using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.UtilityObjects;

namespace FMBusinessObjects.ServiceRequests
{
    [Serializable]
    [DataContract]
	public class AdjustmentDistributionSR : AccountingServiceRequest
	{
		#region Public Attributes
		public enum RequestTypes
		{
			GET_LIST_DATA, GET_USER_DATA, CREATE_ADJUSTMENTS,
			GET_CONFIGURATION_DATA, GET_OWNERS, GET_TRANSACTIONS, NONE
		};
		#endregion

		#region Private Attibutes

		[DataMember]
		private DateTime inventoryDate;
		[DataMember]
		private string productID;
		[DataMember]
		private string managerID;
		[DataMember]
		private string transactionType;
		[DataMember]
		private bool consortiumFlag;
		[DataMember]
		private Guid transactionAliasGuid;
		[DataMember]
		private RequestTypes subrequest;

		[DataMember]
		private List<AdjustmentOwnerRecord> adjOwnerRecordList;

		[DataMember]
		private Hashtable userDataList;
		[DataMember]
		private string notes;
		[DataMember]
		private List<GeneralConfigAlias> affectsInventoryAliasList;
		#endregion

		#region Constructor
		/// <summary>
		/// This is the default constructor for the adjustment distribution object.
		/// </summary>
		public AdjustmentDistributionSR()
		{
			this.Init();
		}

		//De-serialization constructor
		public AdjustmentDistributionSR(SerializationInfo info, StreamingContext context)
		{
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property sets and gets the manager ID attribute.
		/// </summary>
		public string ManagerID
		{
			get { return this.managerID; }
			set { this.managerID = value; }
		}

		/// <summary>
		/// This property sets and gets the product ID attribute.
		/// </summary>
		public string ProductID
		{
			get { return this.productID; }
			set { this.productID = value; }
		}

		/// <summary>
		/// This property sets and gets the inventory date attribute.
		/// </summary>
		public DateTime InventoryDate
		{
			get { return this.inventoryDate; }
			set { this.inventoryDate = value; }
		}

		/// <summary>
		/// This property sets and gets the adjustment distribution sub-request attribute.
		/// </summary>
		public AdjustmentDistributionSR.RequestTypes Subrequest
		{
			get { return this.subrequest; }
			set { this.subrequest = value; }
		}

		/// <summary>
		/// This property sets and gets the transaction type attribute.
		/// </summary>
		public string TransactionType
		{
			get { return this.transactionType; }
			set { this.transactionType = value; }
		}

		/// <summary>
		/// This property sets and gets the transaction alias ID attribute.
		/// </summary>
		public Guid TransactionAliasGuid
		{
			get { return this.transactionAliasGuid; }
			set { this.transactionAliasGuid = value; }
		}

		/// <summary>
		/// This property sets and gets the consortium flag attribute.
		/// </summary>
		public bool IsConsortium
		{
			get { return this.consortiumFlag; }
			set { this.consortiumFlag = value; }
		}

		/// <summary>
		/// This property sets and gets the adjustment owner record list attribute.
		/// </summary>
		public List<AdjustmentOwnerRecord> AdjustmentOwnerRecordList
		{
			get { return this.adjOwnerRecordList; }
			set { this.adjOwnerRecordList = value; }
		}

		/// <summary>
		/// This property sets and gets the user data attribute.
		/// </summary>
		public Hashtable UserDataList
		{
			get { return this.userDataList; }
			set { this.userDataList = value; }
		}

		/// <summary>
		/// This property sets and gets the notes attribute.
		/// </summary>
		public string Notes
		{
			get { return this.notes; }
			set { this.notes = value; }
		}

		/// <summary>
		/// This property sets and gets the list of aliases that affect inventory attribute.
		/// </summary>
		public List<GeneralConfigAlias> AffectsInventoryAliasList
		{
			get { return this.affectsInventoryAliasList; }
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// This method will initialize the object to its initial state.
		/// </summary>
		private void Init()
		{
			this.inventoryDate = DateTime.Today;
			this.productID = "";
			this.managerID = "";
			this.transactionType = "";
			this.consortiumFlag = false;
			this.subrequest = AdjustmentDistributionSR.RequestTypes.NONE;
			this.transactionAliasGuid = Guid.Empty;
			this.adjOwnerRecordList = null;
			this.userDataList = null;
			this.notes = "";
			this.affectsInventoryAliasList = null;
		}
		#endregion
	}
}

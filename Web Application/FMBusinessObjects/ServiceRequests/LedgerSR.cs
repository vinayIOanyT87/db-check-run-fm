using System;
using System.Collections;
using System.Runtime.Serialization;

using FMBusinessObjects.DataObjects;

namespace FMBusinessObjects.ServiceRequests
{
    using UtilityObjects;

    [Serializable]
    [DataContract]
    [KnownType(typeof(Site))]
    public class LedgerSR : AccountingServiceRequest
	{
		#region Attributes
		public enum LedgerRequests { Refresh, ManagerLedger };
		[DataMember]
		private string manager;
		[DataMember]
		private string owner;
		[DataMember]
		private string product;
		[DataMember]
		private string month;
		[DataMember]
		private string tankId;
		[DataMember]
		private bool showCost;
		[DataMember]
		private BsmeLedgerDateType.DateProcessTypes dateType;
		[DataMember]
		private QuantityDisplay units;
		[DataMember]
		private LedgerRequests request;

		[DataMember]
		public Guid ManagerMasterGuid { get; set; }

		[DataMember]
		public Guid OwnerMasterGuid { get; set; }

		[DataMember]
		private ArrayList aliasList;
		#endregion

		#region Constructor
		/// <summary>
		/// This is the default constructor for the Ledger Service Request class.
		/// </summary>
		public LedgerSR()
		{
			this.Site = "";
			this.month = "";
			this.product = "";
			this.manager = "";
			this.owner = "";
			this.aliasList = null;
			this.showCost = false;
			this.ManagerMasterGuid = Guid.Empty;
			this.OwnerMasterGuid = Guid.Empty;
			this.dateType = BsmeLedgerDateType.DateProcessTypes.ByInventoryDate;
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property sets and gets the Manager name.
		/// </summary>
		public string Manager
		{
			get { return this.manager; }
			set { this.manager = value; }
		}

		/// <summary>
		/// This property sets and gets the owner name.
		/// </summary>
		public string Owner
		{
			get { return this.owner; }
			set { this.owner = value; }
		}

		/// <summary>
		/// This property sets and gets the Product name.
		/// </summary>
		public string Product
		{
			get { return this.product; }
			set { this.product = value; }
		}

		/// <summary>
		/// This property sets and gets the month/year value.
		/// </summary>
		public string Month
		{
			get { return this.month; }
			set { this.month = value; }
		}

		/// <summary>
		/// This property sets and gets the show cost flag. If
		/// true, then show cost.
		/// </summary>
		public bool ShowCost
		{
			get { return this.showCost; }
			set { this.showCost = value; }
		}

		/// <summary>
		/// This property sets and gets the date type value.
		/// </summary>
		public BsmeLedgerDateType.DateProcessTypes DateType
		{
			get { return this.dateType; }
			set { this.dateType = value; }
		}

		/// <summary>
		/// This property sets and gets the Gross, Net, Gross/Net, Mass, or Package flag.
		/// </summary>
		public QuantityDisplay Units
		{
			get { return this.units; }
			set { this.units = value; }
		}

		/// <summary>
		/// This property limits the results to relate to a single tank.
		/// Leaving this an empty string results in no filter applied.
		/// </summary>
		public string TankId
		{
			get { return this.tankId; }
			set {
			    this.tankId = value; }
		}

		/// <summary>
		/// This property sets and gets the a list of aliases.
		/// </summary>
		public ArrayList AliasList
		{
			get { return this.aliasList; }
			set { this.aliasList = value; }
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// This method sets the sub-type Ledger Request to be performed (i.e. Refresh,
		/// link ...).
		/// </summary>
		/// <param name="requestParam"></param>
		public void SetRequestType(LedgerRequests requestParam)
		{
			this.request = requestParam;
		}

		/// <summary>
		/// This method will return the ledger sub-type requestParam. 
		/// </summary>
		/// <returns></returns>
		public LedgerRequests GetRequestType()
		{
			return this.request;
		}

		/// <summary>
		/// This method will return the first date of the month/year..
		/// </summary>
		/// <returns></returns>
		public string GetLedgerStartDate()
		{
			return DateEfficacy.getFirstDayOfMonth(this.month);
		}

		/// <summary>
		/// This method will return the last date of the month/year.
		/// </summary>
		/// <returns></returns>
		public string GetLedgerEndDate()
		{
			return DateEfficacy.getLastDayOfMonth(this.month);
		}
		#endregion
	}
}

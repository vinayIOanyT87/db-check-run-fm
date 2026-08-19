/// <summary>
/// File name:	ReportingServiceRequest.cs
/// Purpose:	This is the base class for all reporting service requests.
///	Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 
///				2000.  This file shall not be copied or reproduced in any form 
///				without the express written consent of Endress+Hauser.
///	Author(s):	Richard R. Panachida
///	Version:	1.0.0  Current version
///	
///	Modification History:
///		Date:			By:						Reason:
///		----------		--------------------	----------------------------------
///		
/// </summary>
using System;
using System.Collections;
using FMCommon;

namespace ReportingServices
{
	[System.Serializable]
	public class ReportServiceRequest
	{
		#region Attributes
		protected string    siteID;
		protected int       siteIndex;
		protected ArrayList siteList;
		protected string    securityToken;
		protected string    sessionID;
		protected string    startDate;
		protected string    endDate;
		protected string    managerID;
		protected string    ownerID;
		protected string    productID;
		protected string    billToID;
		protected string    shipToID;
		protected string    carrierID;
		protected string    supplierID;
		protected string    month;
		protected string    grossNet;
		protected bool      useDataDictionary;
		private const int   EMPTY_STRING = 0;

        private SecurityClass security;

		#endregion
	
		#region Constructor
		/// <summary>
		/// This is the default constructor for the accounting service
		/// request base class.
		/// </summary>
		public ReportServiceRequest()
		{
			this.Init();
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// This method will add the site to a list of sites to be
		/// used as a criterion during a query.
		/// </summary>
		/// <param name="site"></param>
		public void AddSiteToList(string site)
		{
			if ((site != null) && (site.Length > EMPTY_STRING))
			{
				this.SiteList.Add(site);
			}
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property gets and sets an array list of sites.
		/// </summary>
		public ArrayList SiteList
		{
			get { return this.siteList; }
			set { this.siteList = value; }
		}

		/// <summary>
		/// This property sets and gets the site information.
		/// </summary>
		public string Site
		{
			get { return this.siteID; }
			set { this.siteID = value; }
		}

		/// <summary>
		/// This property gets and sets the current site index.
		/// </summary>
		public int CurrentSiteIndex
		{
			get { return this.siteIndex; }
			set { this.siteIndex = value; }
		}

 		/// <summary>
		/// This property sets and gets the security token.
		/// </summary>
		public string SecurityToken
		{
			get { return this.securityToken; }
			set { this.securityToken = value; }
		}

		/// <summary>
		/// TThis property sets and gets the processing starting date.
		/// </summary>
		public string StartDate
		{
			get { return this.startDate; }
			set { this.startDate = value; }
		}

		/// <summary>
		/// This property sets and gets the processing end date.
		/// </summary>
		public string EndDate
		{
			get { return this.endDate; }
			set { this.endDate = value; }
		}

		/// <summary>
		/// This property sets and gets the Manager name.
		/// </summary>
		public string ManagerID
		{
			get {return this.managerID;}
			set {this.managerID = value;}
		}

		/// <summary>
		/// This property sets and gets the owner name.
		/// </summary>
		public string OwnerID
		{
			get {return this.ownerID;}
			set {this.ownerID = value;}
		}

		/// <summary>
		/// This property sets and gets the Product name.
		/// </summary>
		public string ProductID
		{
			get { return this.productID; }
			set { this.productID = value; }
		}

		/// <summary>
		/// This property sets and gets the Equipment name.
		/// </summary>
		public string BillToID
		{
			get { return this.billToID; }
			set { this.billToID = value; }
		}

		/// <summary>
		/// This property sets and gets the ship to name.
		/// </summary>
		public string ShipToID
		{
			get { return this.shipToID; }
			set { this.shipToID = value; }
		}

		/// <summary>
		/// This property sets and gets the carrier name.
		/// </summary>
		public string CarrierID
		{
			get { return this.carrierID; }
			set { this.carrierID = value; }
		}

		/// <summary>
		/// This property sets and gets the supplier name.
		/// </summary>
		public string SupplierID
		{
			get { return this.supplierID; }
			set { this.supplierID = value; }
		}

		/// <summary>
		/// This property sets and gets the shipper name.
		/// </summary>
		public string SessionID
		{
			get { return this.sessionID; }
			set { this.sessionID = value; }
		}

		/// <summary>
		/// This property sets and gets the use data dictionary
		/// flag.  True means to use the data dictionary and false
		/// means do not use.
		/// </summary>
		public bool UseDataDictionary
		{
			get { return this.useDataDictionary; }
			set { this.useDataDictionary = value; }
		}

		/// <summary>
		/// This property sets and gets the month value. It should be in
		/// the following format: MonthName Year; i.e. "January 2004".
		/// </summary>
		public string Month
		{
			get { return this.month; }
			set { this.month = value; }
		}

		/// <summary>
		/// This property sets and gets the gross/net value to either GROSS or NET.
		/// </summary>
		public string GrossNet
		{
			get { return this.grossNet; }
			set { this.grossNet = value; }
		}

        public SecurityClass Security
        {
            get { return security; }
            set { security = value; }
        }
        #endregion

		#region Protected Methods
		/// <summary>
		/// This method will set this object to its initial state.
		/// </summary>
		protected void Init()
		{
			this.siteID        = "";
			this.siteIndex     = -1;
			this.securityToken = "";
			this.startDate     = "";
			this.endDate       = "";
			this.productID     = "";
			this.managerID     = "";
			this.ownerID       = "";
			this.billToID      = "";
			this.shipToID      = "";
			this.carrierID     = "";
			this.supplierID    = "";
			this.sessionID     = "";
			this.month         = "";
			this.grossNet      = "GROSS";
			this.useDataDictionary = true;

			this.siteList = new ArrayList();
		}
		#endregion

		#region ISerializable Members
		public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			// TODO:  Add AccountingServiceRequest.GetObjectData implementation
		}
		#endregion
	}
}

/// <summary>
/// File name:	ReportListSR.cs
/// Purpose:	Contains the report list service request parameters and special request
///				to generate report data.
///	Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 
///				2000.  This file shall not be copied or reproduced in any form 
///				without the express written consent of Endress+Hauser.
///	Author(s):	Richard R. Panachida
///	Version:	1.0.0  Current version
///	
///	Modification History:
///		Date:			By:				Reason:
///		----------	-------------	-------------------------------------------
///		11-Nov-05	I.Orndorff		7.0.0.1 - Added "HasAllItem" property to the 
///														 ReportListSR class.
///		
/// </summary>
/// 
using System;
using System.Collections;

namespace ReportingServices
{
	[System.Serializable]
	public class ReportListSR : ReportServiceRequest
	{
		#region Attributes
		// Public...
		public enum SubReportTypes {PRODUCT_LIST, CARRIER_LIST, SHIPPER_LIST, SHIPTO_LIST, BILLTO_LIST, SUPPLIER_LIST,
									MANAGER_LIST, OWNER_LIST, MONTH_YEAR_LIST, NONE};

		// Private...
		private SubReportTypes subRptType;
		private const int      EMPTY_STRING = 0;
		private bool hasAllItem;
		#endregion

		#region Constructor
		/// <summary>
		/// This is the default constructor for the report list service 
		/// request class.
		/// </summary>
		public ReportListSR()
		{
			this.Initialize();
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property sets and gets the sub report type (product,
		/// customer, ...).
		/// </summary>
		public SubReportTypes SubReportType
		{
			get { return this.subRptType; }
			set { this.subRptType = value; }
		}

		public bool HasAllItem
		{
			get { return this.hasAllItem; }
			set { this.hasAllItem = value; }
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// This method will set this object to its initial state.
		/// </summary>
		private void Initialize()
		{
			base.Init();
			this.subRptType = SubReportTypes.NONE;
			this.hasAllItem = false;
		}
		#endregion
	}
}

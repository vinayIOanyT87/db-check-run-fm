/// <summary>
/// File name:	ReportConfigurationDetailSR.cs
/// Purpose:	The purpose of this class is to contain the request information to be 
///				processed.
///	Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 
///				2000.  This file shall not be copied or reproduced in any form 
///				without the express written consent of Endress+Hauser.
///	Author(s):	Richard R. Panachida
///	Version:	1.0.0  Current version
///	
///	Modification History:
///		Date:			By:						Reason:
///		----------		--------------------	----------------------------------
///		2005/7/15		Richard Panachida		Added feature to auto print reports
///		
/// </summary>
/// 
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using FMBusinessObjects.DataObjects;

namespace FMBusinessObjects.ServiceRequests
{
    [Serializable]
    [DataContract]
	[KnownType(typeof(ReportConfigurationDetailListDO))]
	public class ReportConfigurationDetailSR : AccountingServiceRequest
	{
		#region Public attributes
		public enum RequestTypes { SAVE, DELETE, GET, GET_ALL, GET_ALL_NON_PRINT, GET_PRINT_TYPE, GET_PRINT_AT_END_OF_DAY_TYPE, GET_PRINT_AT_END_OF_MONTH_TYPE, UPDATE_ORDER, NONE };
		#endregion

		#region Private attributes
		[DataMember]
		private ReportConfigurationDetailDO reportDetailDO;
		[DataMember]
		private RequestTypes requestType;
		[DataMember]
		private List<ReportConfigurationDetailDO> reportConfigurationDetailList;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the report configuration detail service request.
		/// </summary>
		public ReportConfigurationDetailSR ( )
		{
			this.init ( );
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property gets and sets the type of request (save, delete, get, or none).
		/// </summary>
		public RequestTypes RequestType
		{
			get { return this.requestType; }
			set { this.requestType = value; }
		}

		/// <summary>
		/// This property gets and sets the report configuration detail data object.
		/// </summary>
		public ReportConfigurationDetailDO ReportConfigurationDetailDO
		{
			get { return this.reportDetailDO; }
			set { this.reportDetailDO = value; }
		}

		/// <summary>
		/// This property gets and sets the report configuration detail list.
		/// </summary>
		public List<ReportConfigurationDetailDO> ReportConfigurationDetailList
		{
			get { return this.reportConfigurationDetailList; }
			set { this.reportConfigurationDetailList = value; }
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// This method initializes the report configuration detail service request to its initial state.
		/// </summary>
		private void init ( )
		{
			this.reportDetailDO					= null;
			this.reportConfigurationDetailList	= null;
			this.requestType					= RequestTypes.NONE;
		}
		#endregion
	}
}

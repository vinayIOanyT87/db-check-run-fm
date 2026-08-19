/// <summary>
/// File name:	ReportConfigurationGroupSR.cs
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
///		
/// </summary>
/// 
using System;
using System.Collections;

namespace ReportingServices
{
	public class ReportConfigurationGroupSR : ReportServiceRequest
	{
		#region Public attributes
		public enum RequestTypes {SAVE, DELETE, GET, GET_BY_NAME, GET_ALL, UPDATE_ORDER, NONE};
		#endregion

		#region Private attributes
		private ReportConfigurationGroupDO reportGroupDO;
		private ArrayList reportGroupList;
		private RequestTypes requestType;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the report configuration group service request.
		/// </summary>
		public ReportConfigurationGroupSR()
		{
			this.init();
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
		public ReportConfigurationGroupDO ReportConfigurationGroupDO
		{
			get { return this.reportGroupDO; }
			set { this.reportGroupDO = value; }
		}

		/// <summary>
		/// This property gets and sets the report group list with a list of report group DOs.
		/// </summary>
		public ArrayList ReportGroupList
		{
			get { return this.reportGroupList; }
			set { this.reportGroupList = value; }
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// This method initializes the report configuration detail service request to its initial state.
		/// </summary>
		private void init()
		{
			this.reportGroupDO   = null;
			this.reportGroupList = null;
			this.requestType     = RequestTypes.NONE;
		}
		#endregion
	}
}
/// <summary>
/// File name:	ReportService.cs
/// Purpose:	This is the entry point to access the report service implementation.  This is
///				the object that the client will utilize to retrieve their data.
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
using System.Data;
using ReportingServices;

namespace ReportingBLL
{
	/// <summary>
	/// Summary description for ReportService.
	/// </summary>
	public class ReportService
	{
		#region Attributes
		private ReportServiceImpl reportSrvImpl;
		#endregion

		#region Constructor
		/// <summary>
		/// This is the default constructor.  It sets the reference to the
		/// report service implementation.
		/// </summary>
		/// <param name="rptImpl"></param>
		public ReportService(ReportServiceImpl rptImpl)
		{
			this.reportSrvImpl = rptImpl;
		}
		#endregion

		#region Methods
		/// <summary>
		/// This method invokes the report service implement to remotely
		/// retrieve the requested report data.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public DataSet Request(ReportServiceRequest request)
		{
			return this.reportSrvImpl.ProcessRequest(request);
		}

		public DataObjectBase Request2(ReportServiceRequest request)
		{
			return (DataObjectBase) this.reportSrvImpl.ProcessRequest2(request);
		}
		#endregion
	}
}

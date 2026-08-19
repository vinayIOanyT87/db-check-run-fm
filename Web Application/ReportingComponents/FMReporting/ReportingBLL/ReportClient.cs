/// <summary>
/// File name:	ReportClient.cs
/// Purpose:	Allows a client to request reports from the BLL.
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

namespace ReportingBLL
{
	public class ReportClient
	{
		#region Attributes
		private ReportService reportService;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the report client class.
		/// It initializes the class to its initial state.
		/// </summary>
		public ReportClient()
		{
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// This method will return the reporting service object.
		/// </summary>
		/// <returns></returns>
		public ReportService connect()
		{
			this.reportService = new ReportService(new ReportServiceImpl());
			return this.reportService;
		}
		#endregion
	}
}

/// <summary>
/// File name:	ReportingRequestProcessor.cs
/// Purpose:	The main entry point into the processors.  This is the base
///				class to all processors.
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
using ReportingServices;

namespace ReportingBLL
{
	public abstract class ReportRequestProcessor
	{
		#region Attributes
		private ReportServiceImpl reportingServerImpl;
		protected ReportSecurity  reportSecurity;
		#endregion

		#region Constructor
		/// <summary>
		/// This is the default constructor for the Request Processor
		/// class.
		/// </summary>
		/// <param name="reportingServerImpl"></param>
		public ReportRequestProcessor(ReportServiceImpl reportingServerImpl)
		{
			this.reportingServerImpl = reportingServerImpl;
			this.reportSecurity      = null;
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property returns the reporting service implementation object
		/// that is used by the client to perform reporting business logic.
		/// </summary>
		protected ReportServiceImpl ReportingService
		{
			get { return this.reportingServerImpl; }
		}
		#endregion

		#region Methods
		/// <summary>
		/// This abstract method ensuring that the derive classes
		/// implement the get command method.  Each processor is required
		/// to have this method and they shall return a string identifying
		/// which service request was sent.
		/// </summary>
		/// <returns></returns>
		public abstract string GetCommand();

		virtual public System.Data.DataSet Process(ReportingServices.ReportServiceRequest reportingSR)
		{
			return null;
		}

		virtual public DataObjectBase Process2(ReportingServices.ReportServiceRequest reportingSR)
		{
			return (DataObjectBase) new ErrorObject();
		}
		#endregion

	}
}

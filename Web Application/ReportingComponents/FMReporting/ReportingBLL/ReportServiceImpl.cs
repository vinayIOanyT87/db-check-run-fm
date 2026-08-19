/// <summary>
/// File name:	ReportingServerImpl.cs
/// Purpose:	To implement the reporting server.
///	Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 
///				2000.  This file shall not be copied or reproduced in any form 
///				without the express written consent of Endress+Hauser.
///	Author(s):	Richard R. Panachida
///	Version:	1.0.0  Current version
///	
///	Modification History:
///		Date:			By:						Reason:
///		----------		--------------------	----------------------------------
///		2006-09-22		Richard Panachida		Added code to register the new report
///												URL/Directory processor.
/// </summary>
using System;
using System.Collections;
using ReportingDAL;
using ReportingServices;

namespace ReportingBLL
{
	public class ReportServiceImpl : System.MarshalByRefObject
	{
		#region Attributes
		private Hashtable requestProcessorList;
		private ReportingDA rptDA;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Reporting Service Implementation
		/// class.
		/// </summary>
		public ReportServiceImpl()
		{
			this.Init();
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property will return the Reporting Data Access object.
		/// </summary>
		protected internal ReportingDA RptDA
		{
			get { return this.rptDA; }
		}

		#endregion

		#region Public Methods
		/// <summary>
		/// This method will determine the type of the service request and call the
		/// process method on the appropriate processor to handle the request.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public System.Data.DataSet ProcessRequest(ReportServiceRequest request)
		{
			string requestType = request.GetType().ToString();
			ReportRequestProcessor processor = (ReportRequestProcessor) requestProcessorList[requestType];

			if(processor == null)
			{
				//Log error/debug messages
			}

			return processor.Process(request);
		}

		/// <summary>
		/// This method will determine the type of the service request and call the
		/// process method on the appropriate processor to handle the request. This is 
		/// a temp solution!!!!!!!!
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public DataObjectBase ProcessRequest2(ReportServiceRequest request)
		{
			string requestType = request.GetType().ToString();
			ReportRequestProcessor processor = (ReportRequestProcessor) requestProcessorList[requestType];

			if(processor == null)
			{
				//Log error/debug messages
			}

			return (DataObjectBase) processor.Process2(request);
		}

		#endregion

		#region Private Methods
		/// <summary>
		/// This method will instantiate all the accounting processors and
		/// create a list.
		/// </summary>
		private void Init()
		{
			this.rptDA = new ReportingDA();
			this.requestProcessorList = new Hashtable();

			this.Register(new ReportListProcessor(this));
		}

		/// <summary>
		/// This method will add the processor and key to the processor
		/// list.
		/// </summary>
		/// <param name="processor"></param>
		private void Register(ReportRequestProcessor processor)
		{
			string command = processor.GetCommand();
			this.requestProcessorList.Add(command, processor);
		}
		#endregion
	}
}

/// <summary>
/// File name:	ReportUrlPathProcessor.cs
/// Purpose:	Handles the report url/path service request to retrieve the report system
///				information. It will return a data set object that contains
///				a view of the data.
///	Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 
///				2000.  This file shall not be copied or reproduced in any form 
///				without the express written consent of Endress+Hauser.
///	Author(s):	Richard R. Panachida
///	Version:	1.0.0  Current version
///	
///	Modification History:
///		Date:		By:					Reason:
///		----------	------------------	-------------------------------------------
///		2006-09-22	Richard Panachida	Initial version. Used to retrieve the report system
///										information (URL and Directory) and determine whether
///										the report proxy needs to be used or skip. It looks in
///										the registry to determine if the report proxy is to be
///										ignored or used.
///		2006-10-13	Richard Panachida	Removed the check to see if the report proxy is needed.
///										The report proxy has been removed from the application.
/// </summary>
/// 
using System;
using System.Data;
using ReportingServices;
using ConsolidatedBLL;
using ConsolidatedDataObjects;
using LogClient;
using Microsoft.Win32;

namespace ReportingBLL
{
	/// <summary>
	/// Summary description for ReportUrlPathProcessor.
	/// </summary>
	public class ReportUrlDirProcessor : ReportRequestProcessor
	{
		#region Attributes
		private string          requestCommand;
		private ReportUrlDirSR  reportUrlDirSR;
		private const int       EMPTY_STRING = 0;
		private Logger          logger;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the report url/path processor class.
		/// It must initialize the reporting service implemention class in the base class
		/// and set the request command with the service request string.
		/// </summary>
		/// <param name="reportingServiceImpl"></param>
		public ReportUrlDirProcessor(ReportServiceImpl reportingServerImpl) : base (reportingServerImpl)
		{
			this.requestCommand = typeof(ReportUrlDirSR).ToString();
		}
		#endregion

		#region Override Public Methods
		/// <summary>
		/// This method implements the base class get command method.  It will
		/// return the report url/path request command (class name string).
		/// This is used during the registrations of the processors in the reporting
		/// service object.
		/// </summary>
		/// <returns></returns>
		override public string GetCommand()
		{
			return requestCommand;
		}

		/// <summary>
		/// This method starts the processing of gathering all the report system data.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		override public ReportingServices.DataObjectBase Process2(ReportServiceRequest request)
		{
			ReportUrlDirDO rptUrlDirDO = new ReportUrlDirDO();
			this.reportUrlDirSR = (ReportUrlDirSR) request;

			logger = new Logger("ReportingBLL");
			logger.Debug("Entered report url/path Process method.");

			// Create the security object that will be used to access the appropriate
			// data.  The request object should have the token, site index, and
			// whether or not to use the data dictionary.
			base.reportSecurity = new ReportSecurity(this.reportUrlDirSR.SecurityToken, 
				                                     this.reportUrlDirSR.CurrentSiteIndex, 
				                                     this.reportUrlDirSR.UseDataDictionary);
            base.reportSecurity.Security = this.reportUrlDirSR.Security;
			
			// Get the system report URL and directory information.
			this.GetSystemReportUrlAndDirectory(rptUrlDirDO);

			return rptUrlDirDO;
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// This method will retrieve the report URL from either the system settings or
		/// use the report proxy URL. This is determine by a Key in the registry and
		/// defaults to using the proxy. In addition, the report directory is retrieved
		/// from the site.
		/// </summary>
		/// <param name="rptDO"></param>
		private void GetSystemReportUrlAndDirectory(ReportUrlDirDO rptDO)
		{
			SystemSettingsClass systemSettings = new SystemSettingsClass();
			SystemSettingClass  systemSetting  = systemSettings.Get(base.reportSecurity.Security);
			rptDO.URL = systemSetting.ReportServerURL;
				
			SitesClass Sites = new SitesClass();
			SiteClass  Site  = Sites.Get(base.reportSecurity.Security, base.reportSecurity.Security.SiteIndex);
			rptDO.Directory  = Site.ReportDirectory.Replace(" ", "+");	
		}
		#endregion
	}
}

 #pragma warning disable 1587
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
#pragma warning restore 1587
namespace FMBusinessServices.ServiceClasses
{
    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.LogClient;
    using FMBusinessObjects.ServiceRequests;

    public class ReportUrlDirProcessorClass : IReportUrlDirProcessor
	{
		#region Attributes

        private ReportUrlDirSR reportUrlDirSR;

        private Logger logger;
		#endregion

		#region Public Methods
		/// <summary>
		/// This method starts the processing of gathering all the report system data.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public ReportUrlDirDO Process ( ReportUrlDirSR request )
		{
			ReportUrlDirDO rptUrlDirDO = new ReportUrlDirDO ( );
			this.reportUrlDirSR = request;

			this.logger = new Logger ( "ReportingBLL" );
			this.logger.Debug ( "Entered report url/path Process method." );

			// Get the system report URL and directory information.
			this.GetSystemReportUrlAndDirectory ( rptUrlDirDO );

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
		private void GetSystemReportUrlAndDirectory ( ReportUrlDirDO rptDO )
		{
			SystemSettingsClass systemSettings = new SystemSettingsClass ( );
			SystemSettingClass systemSetting = systemSettings.Get (this.reportUrlDirSR.Security );
			rptDO.URL = systemSetting.ReportServerUrl;

			SitesClass sites = new SitesClass ( );
			rptDO.Directory = sites.GetReportDirectory(this.reportUrlDirSR.Security, this.reportUrlDirSR.ReportGuid).Replace(" ", "+");
		}
		#endregion
	}
}
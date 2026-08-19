// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReportLocation.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ReportLocation type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMReportWebMain
{
	using System;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	public class ReportLocation
	{
		#region Constants and Fields

		private string reportPath = "";

		private string reportServerUri = "";

		#endregion

		#region Constructors and Destructors

		/// <summary>
		///    This is the default constructor for the Report Location object.
		/// </summary>
		/// <param name="manageSecurity"></param>
		public ReportLocation(ManageSecurity manageSecurity)
		{
			this.GetReportLocationInfo(manageSecurity);
		}

		#endregion

		#region Public Properties

		/// <summary>
		///    This property returns the report path for a given site.
		/// </summary>
		public string ReportPath
		{
			get
			{
				return this.reportPath;
			}
		}

		/// <summary>
		///    This property returns the report URI for the system.
		/// </summary>
		public string ReportServerUri
		{
			get
			{
				return this.reportServerUri;
			}
		}

		#endregion

		#region Methods

		/// <summary>
		///    This method is called by the constructor to retrieve the report URI and path for the reports.
		///    If any errors occur, then the URI and Path are defaulted.
		/// </summary>
		/// <param name="managerSecurity"></param>
		private void GetReportLocationInfo(ManageSecurity managerSecurity)
		{
			try
			{
				SystemSettingClass sysSetting = FMChannelHelper.MakeCall<ISystemSettings, SystemSettingClass>(x => x.Get(managerSecurity.Security));

				this.reportServerUri = sysSetting.ReportServerUrl;
			}
			catch (Exception)
			{
				this.reportServerUri = "http://LocalHost/ReportServer";
			}

			try
			{
				//SiteClass site = managerSecurity.Sites.GetByID (managerSecurity.Security, managerSecurity.Security.SiteID,false);
				//SiteClass site = managerSecurity.Sites.GetUsingGuid(managerSecurity.Security, managerSecurity.Security.SiteGuid);

				SiteClass site =
					FMChannelHelper.MakeCall<ISites, SiteClass>(
						x => x.GetUsingGuid(managerSecurity.Security, managerSecurity.Security.SiteGuid));

				this.reportPath = site.ReportDirectory;
			}
			catch (Exception)
			{
				this.reportPath = "/Standard Reports";
			}
		}

		#endregion
	}
}
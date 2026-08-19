using System;
using System.Configuration;

using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.ServiceRequests;

namespace AccountingEOM
{
	/// <summary>
	/// Summary description for AccountingEOM.
	/// </summary>
	class AccountingEOM
	{
		#region Attributes
		protected SecurityClass security;
		#endregion Attributes

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			AccountingEOM eom = new AccountingEOM(args);
			eom.Closeout();
		}

		protected AccountingEOM(string [] args)
		{
			Init();
		}

		protected void Init()
		{
			security = CreateSecurity();
		}

		protected void Closeout()
		{
			FMChannelFactory<ISites> sitesClient = new FMChannelFactory<ISites>();
			ISites sites = sitesClient.CreateProxy();

			SiteCollectionClass siteCollection = sites.Enumerate(security);

			foreach(SiteClass site in siteCollection)
			{
				if(site.SiteGroup == false)
				{
					CloseoutSite(site);
				}
			}
		}

		protected void CloseoutSite(SiteClass site)
		{
			CloseoutSiteSR sr = new CloseoutSiteSR();
			sr.Security = CreateSecurity();
			sr.Security.SiteGuid = site.IdentityGuid;
			sr.Security.SiteID = site.ID;

			FMChannelFactory<ICloseoutSiteProcessor> clientCloseoutSiteProcessor = new FMChannelFactory<ICloseoutSiteProcessor>();
			ICloseoutSiteProcessor closeoutSiteProcessor = clientCloseoutSiteProcessor.CreateProxy();

			closeoutSiteProcessor.Process( sr );
		}

		private SecurityClass CreateSecurity()
		{
			string userName    = ConfigurationManager.AppSettings["UserName"];
			string loginSiteID = ConfigurationManager.AppSettings["LoginSiteID"];
			string siteID      = ConfigurationManager.AppSettings["SiteID"];

			string userGuid      = ConfigurationManager.AppSettings["UserGuid"];
			string loginSiteGuid = ConfigurationManager.AppSettings["LoginSiteGuid"];
			string siteGuid      = ConfigurationManager.AppSettings["SiteGuid"];

			security = new SecurityClass();
			security.SiteID = siteID;
			security.SiteGuid = Guid.Parse(siteGuid);
			security.LoginSiteID = loginSiteID;
			security.LoginSiteGuid = Guid.Parse(loginSiteGuid);
			security.UserID = userName;
			security.UserGuid = Guid.Parse(userGuid);
			security.RightCollection.Add(RIGHT.VIEW_SITES_AND_SITE_GROUPS);
			security.RightCollection.Add(RIGHT.VIEW_PRODUCTS);
			security.RightCollection.Add(RIGHT.VIEW_COMPANY_DATA);
			security.RightCollection.Add(RIGHT.MODIFY_TRANSACTION_DATA);
			security.RightCollection.Add(RIGHT.VIEW_TRANSACTION_DATA);
			security.RightCollection.Add(RIGHT.VIEW_EQUIPMENT_DATA);
			security.RightCollection.Add(RIGHT.PERFORM_CLOSEOUT);

			return security;
		}
	}
}

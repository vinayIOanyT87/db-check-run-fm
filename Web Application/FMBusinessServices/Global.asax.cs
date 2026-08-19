// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Global.asax.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the Global type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices
{
    using FMBusinessObjects.Constants;
    using FMBusinessObjects.DataObjects;
    using FMBusinessServices.ServiceClasses;
    using System;
    using System.Data;
    using System.Web;

    /// <summary>
    /// HttpApplication implemention for FMBusinessServices
    /// </summary>
    public class Global : HttpApplication
	{
		#region Methods

		protected void Application_AuthenticateRequest(object sender, EventArgs e)
		{
		}

		protected void Application_BeginRequest(object sender, EventArgs e)
		{
			// The following header additions are required to allow Chrome clients to run Web Dispatch.
			HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*");
			HttpContext.Current.Response.AddHeader("Access-Control-Allow-Methods", "PUT, GET, POST, OPTIONS");
			HttpContext.Current.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type");
		}

		protected void Application_End(object sender, EventArgs e)
		{
		}

		protected void Application_Error(object sender, EventArgs e)
		{
		}

		protected void Application_Start(object sender, EventArgs e)
		{
			//if (Azure.IsRunningInAzure())
			//    this.StartFmBusinessServices();
			RunRequiredUpdates();

		}

		protected void Session_End(object sender, EventArgs e)
		{
		}

		protected void Session_Start(object sender, EventArgs e)
		{
		}

        /// <summary>
        /// Starts the communication points for FM business services.
        /// </summary>
        private void StartFmBusinessServices()
        {
            //RoleInstanceEndpoint defaultEndpoint = RoleEnvironment.CurrentRoleInstance.InstanceEndpoints[Azure.FmBusinessServicesDefaultEndpoint];

            //// setup addresses
            //bool usingHttp = defaultEndpoint.Protocol.ToUpper() == "HTTP";
            //string urlPrefix = usingHttp ? "http" : "net.tcp";
            //string defaultBaseAddress = string.Format(urlPrefix + "://{0}:{1}", defaultEndpoint.IPEndpoint.Address, defaultEndpoint.IPEndpoint.Port);
        }

		private void RunRequiredUpdates()
        {
            SecurityClass security = new SecurityClass();
			security.AddRight(RIGHT.MODIFY_CONFIGURATION_SETTINGS);
			security.AddRight(RIGHT.MODIFY_MAP_CONFIGURATION);
			security.SiteGuid = Guids.SiteAdminGuid;
			security.UserID = DBAccess.ServiceLoginAccess;
			security.SiteID = "SiteAdmin";

			ConfigurationSettingsClass configurationSettings = new ConfigurationSettingsClass();
			var shouldUpdate = configurationSettings.GetByKey(security, "DatabaseUpdated");
			if (shouldUpdate != null && shouldUpdate.SettingValue == "1")
			{
				// Update all template PointLogicScript's after a database update
				PointTemplates.UpdatePointLogicScripts(security);
                security.SiteGuid = Guids.SiteAdminGuid;

                UpdateProducts(security);

                configurationSettings.Modify(security, "DatabaseUpdated", "0");
            }
        }

        // Enumerate all products and save to fix _VcfModuleSettings due to change in serilizer from 
        // CachingXmlSerializerFactory to DataContractSerializer
        // Can be removed after FM12 SP3 as it will be fixed on first start after upgrade to SP3
        internal static void UpdateProducts(SecurityClass security)
        {
            security.AddRight(RIGHT.MODIFY_PRODUCTS);
            security.AddRight(RIGHT.IMPORT_TRANSACTION);
            var productClass = new ProductsClass();
            DataSet set = productClass.EnumerateProductsAtAllSites(security);
            foreach(DataRow Row in set.Tables[0].Rows)
            {
                var identityGuid = DataObject.getValue<Guid>(Row["ProductGuid"], Guid.Empty);
                security.SiteGuid = DataObject.getValue<Guid>(Row["SiteGuid"], Guid.Empty);
                ProductClass product = productClass.Get(security,identityGuid);
                productClass.Modify(security, product);
            }
            security.SiteGuid = Guids.SiteAdminGuid;
        }

        #endregion
    }
}
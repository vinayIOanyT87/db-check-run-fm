// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnterpriseDataTreeNav.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the EnterpriseDataTreeNav type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMEntityImportWebApp
{
	using System.Collections.Generic;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using FuelsManager.FMWebApp;

	/// <summary>
	/// The enterprise data menu navigation.
	/// </summary>
	public class EnterpriseDataTreeNav : IMenuDiscovery
    {
        #region Public Methods and Operators
        /// <summary>
        ///    Gets a list of menu items that should be displayed for the current user.
        /// </summary>
        /// <param name="security">The security object of the current session</param>
        /// <param name="siteGroup">Whether the current logged-in site is a site group</param>
        /// <param name="options">Hardware key options</param>
        /// <returns>
        ///    List of menu items to be displayed
        /// </returns>
        public List<FMMenuItem> GetMenuItems(SecurityClass security, bool siteGroup, ushort word1, ushort word2, ushort useNewLicenseKey, uint options)
        {
            var menuItems = new List<FMMenuItem>();
            bool addMenuOptions = true;
            bool synchronizationdisabled = false;


            const string EnterpriseDataUrl = "../FMEntityImportWebApp/";
            if (useNewLicenseKey == 1)
            {
                if ((word1 & 0x100) != 0x100)
                {
                    synchronizationdisabled = true;
                    addMenuOptions = false;
                }
            }
            else
            {
                if (!this.HasHardwareKey(options))
                    addMenuOptions = false;
            }
            if(addMenuOptions == true)
            { 
                // TODO: Temporary commented out so that QA does not test Enterprise export/import features.
                //if (this.HasDataTransmissionExportPermission(security))
                //{
                //	var exportMenuItem = new FMMenuItem
                //		{
                //			MenuItemType = FMMenuItemType.OPERATIONS_ENTERPRISE_DATA_DATA_TRANSMISSION_EXPORT,
                //			RootMenuName = "Operations",
                //			CategoryName = "Enterprise Data",
                //			ItemName = "Data Transmission Export",
                //			NavigateUrl = EnterpriseDataUrl + "DataTransmissionExport.aspx"
                //		};
                //	menuItems.Add(exportMenuItem);
                //}

                //if (this.HasDataTransmissionImportPermission(security))
                //{
                //	var importMenuItem = new FMMenuItem
                //		{
                //			MenuItemType = FMMenuItemType.OPERATIONS_ENTERPRISE_DATA_DATA_TRANSMISSION_IMPORT,
                //			RootMenuName = "Operations",
                //			CategoryName = "Enterprise Data",
                //			ItemName = "Data Transmission Import",
                //			NavigateUrl = EnterpriseDataUrl + "DataTransmissionImport.aspx"
                //		};
                //	menuItems.Add(importMenuItem);
                //}

                // Keep in mind that the same system can function as a synchronization client or server.  Depending on what's been configured we will show or hide
                // different menu options.
                if (this.HasSynchronizationProcessPermission(security))
                {
                    // Check client side configuration options
                    SyncClientConfigurationDO clientConfig = FMChannelHelper.MakeCall<ISyncClientConfigurations, SyncClientConfigurationDO>(x => x.Get(security));

                    if (synchronizationdisabled == false && null != clientConfig && clientConfig.IdentityGuid != System.Guid.Empty)
                    {
                        if (!string.IsNullOrEmpty(clientConfig.EnterpriseURL))
                        {
                            var syncMenuItem = new FMMenuItem
                                {
                                    MenuItemType = FMMenuItemType.OPERATIONS_ENTERPRISE_DATA_ONLINE_SYNCHRONIZATION,
                                    RootMenuName = "Operations",
                                    CategoryName = "Enterprise Data",
                                    ItemName = "Online Synchronization",
                                    NavigateUrl = EnterpriseDataUrl + "OnlineSynchronization.aspx",
                                    ApplyDataDictionary = ApplyDataDictionary.Apply
                                };
                            menuItems.Add(syncMenuItem);
                        }

                        //if (!clientConfig.SuspendSynchronizationFlag)
                        //{
                        //    var exportMenuItem = new FMMenuItem
                        //    {
                        //        MenuItemType = FMMenuItemType.OPERATIONS_ENTERPRISE_DATA_OFFLINE_SYNCHRONIZATION,
                        //        RootMenuName = "Operations",
                        //        CategoryName = "Enterprise Data",
                        //        ItemName = "Offline Synchronization",
                        //        NavigateUrl = EnterpriseDataUrl + "OfflineSynchronization.aspx"
                        //    };
                        //    menuItems.Add(exportMenuItem);
                        //}

                        if (synchronizationdisabled == false
                        && (!string.IsNullOrEmpty(clientConfig.EnterpriseURL)
                        || (word1 & 0x2) == 0x2))
                        {
                            var exportMenuItem = new FMMenuItem
                            {
                                MenuItemType = FMMenuItemType.OPERATIONS_SYNC_LOGS_CONFLICTS_ERRORS,
                                RootMenuName = "Operations",
                                CategoryName = "Synchronization Logs",
                                ItemName = "Sync Session Summary",
                                NavigateUrl = EnterpriseDataUrl + "SynchronizationSessionSummary.aspx",
                                ApplyDataDictionary = ApplyDataDictionary.Apply
                            };
                            menuItems.Add(exportMenuItem);
                        }
                    }

                    // Check server side configuration options.
                    SyncServerConfigurationDO serverConfig = FMChannelHelper.MakeCall<ISyncServerConfigurations, SyncServerConfigurationDO>(x => x.Get(security));

                    if (null != serverConfig && serverConfig.IdentityGuid != System.Guid.Empty)
                    {
                        //if (serverConfig.AllowSynchronizationFlag)
                        //{
                        //    var syncImportMenuItem = new FMMenuItem
                        //    {
                        //        MenuItemType = FMMenuItemType.OPERATIONS_ENTERPRISE_DATA_OFFLINE_SYNCHRONIZATION_ENTERPRISE,
                        //        RootMenuName = "Operations",
                        //        CategoryName = "Enterprise Data",
                        //        ItemName = "Offline Synchronization (Enterprise)",
                        //        NavigateUrl = EnterpriseDataUrl + "OfflineSynchronizationEnterprise.aspx"
                        //    };
                        //    menuItems.Add(syncImportMenuItem);
                        //}
                    }
                }

                if (synchronizationdisabled == false && this.HasSynchronizationSettingsPermission(security))
                {
                    var onlineSynchronizationMenuItem = new FMMenuItem
                        {
                            MenuItemType = FMMenuItemType.CONFIG_SYSTEM_SYNCHRONIZATION_SETTINGS,
                            RootMenuName = "Configuration",
                            CategoryName = "System",
                            ItemName = "Synchronization Settings",
                            NavigateUrl = EnterpriseDataUrl + "SynchronizationConfigForm.aspx",
                            ApplyDataDictionary = ApplyDataDictionary.Apply
                        };
                    menuItems.Add(onlineSynchronizationMenuItem);
                }

                if (this.HasSyncDashboardPermission(security))
                {
                    var adminDashboardMenuItem = new FMMenuItem
                    {
                        MenuItemType = FMMenuItemType.OPERATIONS_SYNC_DASHBOARD,
                        RootMenuName = "Administration",
                        CategoryName = "System",
                        ItemName = "Synchronization Dashboard",
                        NavigateUrl = EnterpriseDataUrl + "SynchronizationDashboard.aspx",
                        ApplyDataDictionary = ApplyDataDictionary.Apply
                    };
                    menuItems.Add(adminDashboardMenuItem);
                }
            }

            // This menu option is only available at the Site for users that have migration permissions.  
            // Migration permissions should only be given to Varec field engineers.
            //if (!siteGroup && this.HasMigrationPermission(security))
            //{
            //    var syncMenuItem = new FMMenuItem
            //                            {
            //                                MenuItemType = FMMenuItemType.OPERATIONS_MIGRATION_DATA_EXPORT_IMPORT,
            //                                RootMenuName = "Operations",
            //                                CategoryName = "Software Migration",
            //                                ItemName = "Export Mapping Data",
            //                                NavigateUrl = EnterpriseDataUrl + "MigrationDataExportPage.aspx",
            //                                ApplyDataDictionary = ApplyDataDictionary.Apply
            //                            };
            //    menuItems.Add(syncMenuItem);
            //}

            return menuItems;
        }

		/// <summary>
		/// The has data transmission export permission.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool HasDataTransmissionExportPermission(SecurityClass security)
        {
            return security.HasRight(RIGHT.EXPORT_ENTERPRISE_DATA);
        }

		/// <summary>
		/// The has data transmission import or export permission.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool HasDataTransmissionImportOrExportPermission(SecurityClass security)
        {
            return security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA) || security.HasRight(RIGHT.EXPORT_ENTERPRISE_DATA);
        }

		/// <summary>
		/// The has data transmission import permission.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool HasDataTransmissionImportPermission(SecurityClass security)
        {
            return security.HasRight(RIGHT.IMPORT_ENTERPRISE_DATA);
        }

		/// <summary>
		/// The has enterprise base export permission.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool HasEnterpriseBaseExportPermission(SecurityClass security)
        {
            return security.HasRight(RIGHT.BASE_EXPORT);
        }

        /// <summary>
        /// Determines if the current security context can initiate a manual synchronization request.
        /// </summary>
        /// <param name="security">
        /// Current security context of the caller.
        /// </param>
        /// <returns>
        /// Returns true if the security context has rights, otherwise; false. 
        /// </returns>
        public bool HasSynchronizationProcessPermission(SecurityClass security)
        {
            return security.HasRight(RIGHT.PERFORM_SYNCHRONIZATION);
        }

        /// <summary>
        /// Determines if the current security context can perform migration related tasks
        /// </summary>
        /// <param name="security">
        /// Current security context of the caller.
        /// </param>
        /// <returns>
        /// Returns true if the security context has rights, otherwise; false. 
        /// </returns>
        public bool HasMigrationPermission(SecurityClass security)
        {
            return security.HasRight(RIGHT.MIGRATION_PERFORM_IMPORT_EXPORT);
        }

        /// <summary>
        /// This method will return true if there is a valid hardware key for Shared Components Configuration.
        ///    Otherwise, it will return false. The key is located in the lower word of a 32 bit word
        ///    and the value is 0x4000.
        /// </summary>
        /// <param name="options">
        /// The options.
        /// </param>
        /// <returns>
        /// </returns>
        public bool HasHardwareKey(uint options)
        {
	        bool hasKey = (options & 0x4000) != 0;

	        return hasKey;
        }

        /// <summary>
        /// Determines if the current security context can modify any of the synchronization configuration settings.
        /// </summary>
        /// <param name="security">
        /// Current security context of the caller.
        /// </param>
        /// <returns>
        /// Returns true if the security context has rights, otherwise; false. 
        /// </returns>
        public bool HasSynchronizationSettingsPermission(SecurityClass security)
        {
            return security.HasRight(RIGHT.VIEW_SYNC_CONFIG_CLIENT_SETTINGS)
                    || security.HasRight(RIGHT.VIEW_SYNC_CONFIG_SERVER_SETTINGS)
                    || security.HasRight(RIGHT.VIEW_SYNC_CONFIG_SITE_SETTINGS);
        }

		public bool HasSyncDashboardPermission(SecurityClass security)
		{
			return security.HasRight(RIGHT.ACCESS_SYNC_DASHBOARD);
		}
        #endregion
    }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FuelsManagerService.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Performs automatic closeouts daily for all sites that aren't site groups. 
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FuelsManagerService
{
    using System;
    using System.Linq;
    using System.ServiceModel;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.ServiceRequests;
    using FMBusinessObjects.UtilityObjects;

    /// <summary>
    /// Performs automatic closeouts daily for all sites that aren't site groups. 
    /// </summary>
    public class AutoCloseout
    {
        /// <summary>
        /// The number of minutes we should give the auto closeout process to complete for each site before timing out.
        /// </summary>
        private const int AutoCloseoutOperationTimeoutMinutes = 15;

        /// <summary>
        /// The last time this process was run, regardless of whether it failed or succeeded. 
        /// We want to only do automatic closeouts once a day.
        /// </summary>
        private static DateTimeOffset? lastRunTime;

        /// <summary>
        /// The auto closeout feature runs once a day at or around a specified time, based on system time.
        /// This method checks to see that the current time is past the preferred run time and that the process hasn't already run today.
        /// An alternative to this approach is to use Windows Task Scheduler.
        /// </summary>
        /// <param name="configuredRunTime">The time specified in app.config for the auto closeout process to run</param>
        /// <returns>True if the autocloseout process should be run. False otherwise</returns>
        public static bool ShouldAutoCloseoutRun(DateTime configuredRunTime)
        {
            DateTimeOffset currentDateTime = DateTimeOffset.Now;

            // Don't do anything if this process has already been run today
            if (lastRunTime.HasValue && (currentDateTime.Date - lastRunTime.Value.Date).Days < 1)
            {
                return false;
            }

            // Only allow the process to run if the time is past the preferred run time
            if (currentDateTime.TimeOfDay < configuredRunTime.TimeOfDay)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Run the auto closeout process for all sites, products, and managers that are configured to auto closeout.
        /// </summary>
        /// <param name="security">Contains security information</param>
        public static void PerformAutoCloseouts(SecurityClass security)
        {
            lastRunTime = DateTimeOffset.Now;

            try
            {
                // Get sites that are not groups
                SiteCollectionClass sites = FMChannelHelper.MakeCall<ISites, SiteCollectionClass>(
                        siteChannel => siteChannel.EnumerateBySiteGroup(security, false));

                // Clone the security object used by the service. We need to modify the SiteGuid and SiteID
                // for processing to work correctly, but this could potentially affect other logic executed by the service.
                SecurityClass siteSecurity = security.Clone();

                foreach (SiteClass site in sites.Where(site => !site.InhibitAutomaticCloseout))
                {
                    // Set the cloned security object's site to the site we're doing closeouts for.                
                    siteSecurity.SiteGuid = site.IdentityGuid;
                    siteSecurity.SiteID = site.ID;

                    CloseoutSiteSR closeoutSiteSR = new CloseoutSiteSR
                                                        {
                                                            Security = siteSecurity,
                                                            Site = site.ID
                                                        };
                 
                    // Get today's date in site time. The closeout date we'll use is the end of the month before the current month - the site's open transaction window setting.
                    DateTime convertedDate = TimeConverter.Today(site).Date;
                    DateTime closeoutDate = convertedDate.AddMonths(-site._OpenTransactionWindow);
                    closeoutDate = closeoutDate.AddDays(-closeoutDate.Day);

                    // Get the products assigned to or owned by the site.
                    ProductCollectionClass productCollection = FMChannelHelper.MakeCall<IProducts, ProductCollectionClass>(productsService => productsService.Enumerate(closeoutSiteSR.Security)); 

                    // Get the managers assigned to or owned by the site.
                    CompanyCollectionClass managerCollection = FMChannelHelper.MakeCall<ICompanies, CompanyCollectionClass>(companiesService => companiesService.EnumerateByRoleGetIDCodeTypesIdentityGuidOnly(closeoutSiteSR.Security, new[] { COMPANY_ROLE.MANAGER })); 

                    // For each manager, for each product, calculate the ledger and then create the closeout record.
                    foreach (CompanyClass manager in managerCollection)
                    {
                        foreach (ProductClass product in productCollection)
                        {
                            // If the product is not configured to automatically closeout, skip it.
                            if (!product.AutomaticCloseout)
                            {
                                continue;
                            }

                            var localManager = manager;
                            var localProduct = product;
                            var localSite = site;

                            FMChannelHelper.MakeCall<ICloseoutSiteProcessor>(
                                closeoutSiteProcessor =>
                                    {
                                        ((IClientChannel)closeoutSiteProcessor).OperationTimeout = new TimeSpan(0, AutoCloseoutOperationTimeoutMinutes, 0);
                                        closeoutSiteProcessor.ProcessForSiteManagerAndProduct(closeoutSiteSR, localManager, localProduct, closeoutDate, localSite.ShortDatePattern);
                                    });

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                FuelsManagerServiceLogger.Instance.LogError(ex);
            }
        }
    }
}

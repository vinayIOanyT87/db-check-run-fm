/******************************************************************************

	FILE NAME:		ReportService.cs


	PURPOSE:	  List available reports


	COMMENTS:

		Copyright (C) Varec, Inc.  All rights reserved.

		This file shall not be copied or reproduced in any form without
				the express written consent of Varec.


	AUTHOR(S):	P Reynolds


	VERSION:		1.0.0  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		---------	----------  -------------------------------------------


*******************************************************************************/


namespace FMBusinessObjects.UtilityObjects
{
    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.ReportSvr2005;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;

    public class ReportService
    {

        #region Properties
        /// <summary>
        /// This will get a list of reports available at a site
        /// </summary>
        public static List<string> GetReportsList(SecurityClass security, SiteClass site)
        {
            var reports = new List<string>();
            SystemSettingClass systemSetting = FMChannelHelper.MakeCall<ISystemSettings, SystemSettingClass>(
                                                             x =>
                                                             x.Get(security)
                                                        );

            if (!string.IsNullOrEmpty(systemSetting.ReportServerUrl))
            {
                var reportingService = new ReportingService2005
                {
                    Url = systemSetting.ReportServerUrl + "/ReportService2005.asmx",
                    CookieContainer = new CookieContainer()
                };

                if (!string.IsNullOrEmpty(systemSetting.ReportServerUserName))
                {
                    string[] userName = systemSetting.ReportServerUserName.Split('\\');
                    if (userName.Length > 1)
                    {
                        reportingService.Credentials = new NetworkCredential(
                            userName[1],
                            systemSetting.ReportServerPassword,
                            userName[0]);
                    }
                    else
                    {
                        reportingService.Credentials = new NetworkCredential(
                            userName[0],
                            systemSetting.ReportServerPassword,
                            ".");
                    }
                }
                else
                {
                    reportingService.Credentials = CredentialCache.DefaultCredentials;
                }
                //replace // with / if necessary.  ReportPath in db may or may not have preceeding /
                string tempPath = ("/" + site.ReportDirectory).Replace("//", "/");
                //remove trailing / if necessary
                if (tempPath.Substring(tempPath.Length - 1) == "/")
                {
                    tempPath = tempPath.Substring(0, tempPath.Length - 1);
                }

                // If tempPath is empty or is not a valid item on the Reports Server, do not check for reports to add to the dropdown list
                if (tempPath != string.Empty)
                {
                    bool hasPath = false;

                    try
                    {
                        hasPath =
                            reportingService.ListChildren("/", false)
                                .Any(x => x.Name == tempPath.Replace("/", String.Empty));
                    }
                    catch (Exception)
                    {
                        // Ignore.
                    }

                    if (hasPath)
                    {
                        CatalogItem[] items = reportingService.ListChildren(tempPath, false);

                        foreach (CatalogItem item in items)
                        {
                            if ((item.Type != ItemTypeEnum.Report) && (item.Type != ItemTypeEnum.LinkedReport))
                            {
                                continue;
                            }
                            reports.Add(item.Name);
                        }
                    }
                }
            }
            return reports;
        }

        #endregion
    }

}

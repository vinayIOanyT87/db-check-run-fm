// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GasboyStationLogController.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Controller for the Gasboy Station Log summary and detail pages
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.Afss.WebApp.Areas.AutomatedFuelStation.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Web.Mvc;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.UtilityObjects;

    using FuelsManager.Areas.Controllers;

    using FuelsManager.Afss.Module.Gasboy.BusinessObjects.BusinessInterfaces;
    using FuelsManager.Afss.Module.Gasboy.BusinessObjects.ChannelFactories;
    using FuelsManager.Afss.Module.Gasboy.BusinessObjects.DataObjects;
    using FuelsManager.Afss.WebApp.Areas.AutomatedFuelStation.Models;

	/// <summary>
    /// Contains the search parameters last used by the user when searching from the Gasboy Station Log Summary page
    /// </summary>
    [Serializable]
    public class GasboyStationLogSummarySearchParameters
    {
        /// <summary>
        /// The beginning date / time of the date range search parameter
        /// </summary>
        public DateTimeOffset BeginDateTime { get; set; }

        /// <summary>
        /// The ending date / time of the date range search parameter
        /// </summary>
        public DateTimeOffset EndDateTime { get; set; }

        /// <summary>
        /// The Log type searched for by the user. A null value indicates all log types
        /// </summary>
        public ExternalStationLogType? LogType { get; set; }

        /// <summary>
        /// The Station searched for by the user in the log. An empty value indicates all stations
        /// </summary>
        public Guid ExternalStationGuid { get; set; }
    }

    /// <summary>
    /// Controller for the Gasboy Station Log summary and detail pages
    /// </summary>
	[RouteArea("AutomatedFuelStation")]
    [RoutePrefix("GasboyStationLog")]
    public class GasboyStationLogController : FMBaseController, IDataDictionary
    {
        /// <summary>
        /// The session key used to store the user's last search parameters on the log summary form.
        /// </summary>
        public const string LogSummarySearchParametersSessionKey = "GasboyStationLogSummarySearchParameters";

        /// <summary>
        /// Contains data dictionary values for the form which should be translated
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <returns>Data dictionary values for the form which should be translated</returns>
        [NonAction]
        public new string[] Keys(SecurityClass security)
        {
            return new[]
			       {
                    "Automated Fuel Service Station Exception Logs",
                    "View Details",
                    "Timestamp",
                    "Automated Fuel Service Station",
                    "Log Entry",
                    "Log Type",
                    "Fueling Station",
                    "Date Range",
                    "Automated Fuel Service Station Exception Log",
                    "Close"
			       };
        }

        #region Gasboy Station Log Summary Page Actions

        /// <summary>
        /// Get a view of the Gasboy station logs in the system
        /// </summary>
        /// <returns>A view of the Gasboy station logs in the system</returns>
        [HttpGet]
        public ActionResult GasboyStationLogSummaryIndex()
        {
            var model = new GasboyStationLogSummaryModel();

            try
            {
                SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(sites => sites.Get(
                    this.Security,
                    this.Security.SiteGuid,
                    getMemberSites: false,
                    getSchedulesAndProcessVariables: false,
                    bGetAssociatedAliases: false));

                model.ExternalStations = GasboyChannelHelper.MakeCall<IGasboyStations, List<GasboyStation>>(externalStationsService => externalStationsService.Enumerate(this.Security));
             
                model.ShortDatePattern = site.ShortDatePattern;
                model.TimePattern = site.TimePattern;

                // Get the search parameters last used by the user on this screen from Session
                GasboyStationLogSummarySearchParameters searchParameters = this.Session[LogSummarySearchParametersSessionKey] as GasboyStationLogSummarySearchParameters;

                // If there are search parameters stored in Session, use them
                if (searchParameters != null)
                {
                    DateTimeOffset sessionBeginDate = searchParameters.BeginDateTime;
                    model.BeginDate = sessionBeginDate.Date.ToString(site.ShortDatePattern);
                    model.BeginTime = sessionBeginDate.ToString(site.TimePattern);

                    DateTimeOffset sessionEndDate = searchParameters.EndDateTime;
                    model.EndDate = sessionEndDate.Date.ToString(site.ShortDatePattern);
                    model.EndTime = sessionEndDate.ToString(site.TimePattern);

                    model.SelectedLogType = searchParameters.LogType;

                    // Populate the selected Gasboy station, but only if it exists in the collection of stations. Keep in mind that the site may change
                    // and along with it the stations configured for the site.
                    if (searchParameters.ExternalStationGuid != Guid.Empty
                        && model.ExternalStations.ToList().Find(externalStation => externalStation.IdentityGuid == searchParameters.ExternalStationGuid) != null)
                    {
                        model.SelectedExternalStationGuid = searchParameters.ExternalStationGuid;
                    }
                }
                else
                {
                    var converter = new SiteTimeConverter(site);
                    string todaysDate = converter.Today().Date.ToString(site.ShortDatePattern);
                    model.BeginDate = todaysDate;
                    model.BeginTime = TimeConverter.MinFMTime.ToString(site.TimePattern);
                    model.EndDate = todaysDate;
                    model.EndTime = TimeConverter.MaxFMTime.ToString(site.TimePattern);
                }

                model.ExternalStationLogs = this.GetGasboyStationLogs(model);
            }
            catch (Exception ex)
            {
                this.ErrorHandler(ex);
            }

            return this.View(model);
        }

        /// <summary>
        /// The post action for the Gasboy station log summary page. Gets Gasboy station logs filtered by the parameters provided on the screen
        /// </summary>
        /// <param name="model">
        /// The model of the Gasboy station log summary screen which will contain search parameter values.
        /// </param>
        /// <returns>
        /// A view of the Gasboy station logs filtered by the parameters provided on the screen
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GasboyStationLogSummaryIndex(GasboyStationLogSummaryModel model)
        {
            try
            {
                model.ExternalStations = GasboyChannelHelper.MakeCall<IGasboyStations, List<GasboyStation>>(externalStationsService => externalStationsService.Enumerate(this.Security));
                model.ExternalStationLogs = this.GetGasboyStationLogs(model);
            }
            catch (Exception ex)
            {
                this.ErrorHandler(ex);
            }

            return this.View(model);
        }

        #endregion

        #region Gasboy Station Log Detail Page Actions

        /// <summary>
        /// Get a view of a single Gasboy station log record
        /// </summary>
        /// <param name="externalStationLogGuid">Identifies the log record to retrieve</param>
        /// <param name="logType">Identifies the type of log we're viewing. For station events, we want to return a different view</param>
        /// <returns>A view of the gasboy station log record identified by the provided guid</returns>
        [HttpGet]
        public ActionResult GasboyStationLog(Guid externalStationLogGuid, ExternalStationLogType logType)
        {
            var model = new GasboyStationLogModel();

            try
            {
                SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(sites => sites.Get(
                   this.Security,
                   this.Security.SiteGuid,
                   getMemberSites: false,
                   getSchedulesAndProcessVariables: false,
                   bGetAssociatedAliases: false));

                // If the log record is a station event, get the event record and return a view of the event detail page.
                // Otherwise, get the log record and display it on the regular log detail page.
                if (logType == ExternalStationLogType.StationEvent)
                {
                    GasboyStationEvent gasboyStationEvent = GasboyChannelHelper.MakeCall<IGasboyStations, GasboyStationEvent>(externalStationsService => externalStationsService.GetGasboyStationEvent(this.Security, externalStationLogGuid));

                    var eventModel = new GasboyStationEventModel
                                         {
                                             ShortDatePattern = site.ShortDatePattern,
                                             TimePattern = site.TimePattern,
                                             Log = gasboyStationEvent,
															Description = gasboyStationEvent.ErrorCode.GetDescription()
                                         };

                    return this.View("GasboyStationEvent", eventModel);
                }
                else
                {
                    GasboyStationLog log = GasboyChannelHelper.MakeCall<IGasboyStations, GasboyStationLog>(externalStationsService => externalStationsService.GetLog(this.Security, externalStationLogGuid));
                    model.ShortDatePattern = site.ShortDatePattern;
                    model.TimePattern = site.TimePattern;
                    model.Log = log;
                }
            }
            catch (Exception ex)
            {
                this.ErrorHandler(ex);
            }

            return this.View(model);
        }

        #endregion

        /// <summary>
        /// Returns the Gasboy stations logs configured for the site, filtered by the find text if it was provided
        /// </summary>
        /// <param name="model">Contains the find text value and potentially any other values we need</param>
        /// <returns>The Gasboy stations configured for the site, filtered by the find text if it was provided</returns>
        [NonAction]
        private List<GasboyStationLog> GetGasboyStationLogs(GasboyStationLogSummaryModel model)
        {
            DateTimeOffset beginDate;
            DateTimeOffset endDate;

            SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(sites => sites.Get(
                        this.Security,
                        this.Security.SiteGuid,
                        getMemberSites: false,
                        getSchedulesAndProcessVariables: false,
                        bGetAssociatedAliases: false));

            // Make sure the dates and times are provided. If only the date or only the time is provided, TryParse will actually succeed
            // but will use today's date or midnight, which might not be intuitive.
            if (string.IsNullOrEmpty(model.BeginDate))
            {
                throw new Exception("Begin Date must be provided");
            }

            if (string.IsNullOrEmpty(model.BeginTime))
            {
                throw new Exception("Begin Time must be provided");
            }

            if (string.IsNullOrEmpty(model.EndDate))
            {
                throw new Exception("End Date must be provided");
            }

            if (string.IsNullOrEmpty(model.EndTime))
            {
                throw new Exception("End Time must be provided");
            }

            if (!DateTimeOffset.TryParse(model.BeginDate + " " + model.BeginTime, site.GetDateTimeFormatInfo(), DateTimeStyles.None, out beginDate))
            {
                throw new Exception("Begin Date must be a valid date and time");
            }

            if (!DateTimeOffset.TryParse(model.EndDate + " " + model.EndTime, site.GetDateTimeFormatInfo(), DateTimeStyles.None, out endDate))
            {
                throw new Exception("End Date must be a valid date and time");
            }

            if (beginDate > endDate)
            {
                throw new Exception("The Ending Date and Time must be greater than or equal to the Beginning Date and Time");
            }

            this.Session[LogSummarySearchParametersSessionKey] = new GasboyStationLogSummarySearchParameters
                                                                            {
                                                                                BeginDateTime = beginDate,
                                                                                EndDateTime = endDate,
                                                                                ExternalStationGuid = model.SelectedExternalStationGuid,
                                                                                LogType = model.SelectedLogType
                                                                            };

            return GasboyChannelHelper.MakeCall<IGasboyStations, List<GasboyStationLog>>(
                    externalStationsService =>
                    externalStationsService.EnumerateLogs(
                        this.Security,
                        model.SelectedExternalStationGuid,
                        beginDate,
                        endDate,
                        model.SelectedLogType.HasValue ? model.SelectedLogType.Value : (ExternalStationLogType?)null));
        }
    }
}
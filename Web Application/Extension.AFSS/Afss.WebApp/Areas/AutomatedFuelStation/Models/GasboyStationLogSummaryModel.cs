// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExternalStationLogSummaryModel.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   The model for the External Station Log Summary Page
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FuelsManager.Afss.WebApp.Areas.AutomatedFuelStation.Models
{
    using System;
    using System.Collections.Generic;

    using FuelsManager.Afss.Module.Gasboy.BusinessObjects.DataObjects;

    /// <summary>
    /// The model for the External Station Log Summary Page
    /// </summary>
    public class GasboyStationLogSummaryModel
    {
        /// <summary>
        /// Initialize collections so that if an error occurs the collections aren't null
        /// </summary>
        public GasboyStationLogSummaryModel()
        {
            this.ExternalStationLogs = new List<GasboyStationLog>();
            this.ExternalStations = new List<GasboyStation>();
        }

        /// <summary>
        /// Log records to display in the grid, filtered by the criteria provided
        /// </summary>
        public List<GasboyStationLog> ExternalStationLogs { get; set; }

        /// <summary>
        /// A list of the configured external stations to display in the drop down so users can filter the results to only those for a particular station
        /// </summary>
        public IEnumerable<GasboyStation> ExternalStations { get; set; }

        /// <summary>
        /// The guid of the external station the user selected in the station drop down
        /// </summary>
        public Guid SelectedExternalStationGuid { get; set; }

        /// <summary>
        /// The log type filter value the user selected, if any
        /// </summary>
        public ExternalStationLogType? SelectedLogType { get; set; }

        /// <summary>
        /// The beginning date range to get logs for
        /// </summary>
        public string BeginDate { get; set; }

        /// <summary>
        /// The time portion of the beginning date range to get logs for
        /// </summary>
        public string BeginTime { get; set; }

        /// <summary>
        /// The ending date range to get logs for
        /// </summary>
        public string EndDate { get; set; }

        /// <summary>
        /// The time portion of the ending date range to get logs for
        /// </summary>
        public string EndTime { get; set; }

        /// <summary>
        /// The date pattern configured for the site
        /// </summary>
        public string ShortDatePattern { get; set; }

        /// <summary>
        /// The time pattern configured for the site
        /// </summary>
        public string TimePattern { get; set; }
    }
}
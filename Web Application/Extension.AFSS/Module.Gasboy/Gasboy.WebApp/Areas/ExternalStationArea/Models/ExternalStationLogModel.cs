// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExternalStationLogModel.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   The model for the External Station Log Detail Page
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.Afss.Module.Gasboy.WebApp.Areas.ExternalStationArea.Models
{
    using FuelsManager.Afss.Module.Gasboy.BusinessObjects.DataObjects;

    /// <summary>
    /// Represents the External Station Log Detail page
    /// </summary>
    public class ExternalStationLogModel
    {
        /// <summary>
        /// Initialize the log with a new record to avoid potential null reference errors should an error occur.
        /// </summary>
        public ExternalStationLogModel()
        {
            this.Log = new GasboyStationLog();
        }

        /// <summary>
        /// The log record to display
        /// </summary>
        public GasboyStationLog Log { get; set; }

        /// <summary>
        /// The format to use when displaying dates
        /// </summary>
        public string ShortDatePattern { get; set; }

        /// <summary>
        /// The format to use when displaying times
        /// </summary>
        public string TimePattern { get; set; }
    }
}
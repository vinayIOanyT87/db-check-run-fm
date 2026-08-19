// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExternalStationSummaryModel.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   The model for the External Station Summary Page
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.Afss.Module.Gasboy.WebApp.Areas.ExternalStationArea.Models
{
    using System;
    using System.Collections.Generic;

    using FuelsManager.Afss.Module.Gasboy.BusinessObjects.DataObjects;

    /// <summary>
    ///  The model for the External Station Summary Page
    /// </summary>
    public class ExternalStationSummaryModel
    {
        /// <summary>
        /// Initialize collections to avoid potential null reference errors should an error occur.
        /// </summary>
        public ExternalStationSummaryModel()
        {
            this.ExternalStations = new List<GasboyStation>();
        }

        /// <summary>
        /// The text provided by the user in the find text box to filter the results on the summary page
        /// </summary>
        public string FindText { get; set; }

        /// <summary>
        /// External stations to display
        /// </summary>
        public List<GasboyStation> ExternalStations { get; set; }

        /// <summary>
        /// True if the user has permission to add new external stations from the external station summary page
        /// </summary>
        public bool IsEditable { get; set; }

        /// <summary>
        /// Identifies the current site. Used to determine if the delete button should be disabled for an external station in the grid.
        /// </summary>
        public Guid SiteGuid { get; set; }
    }
}
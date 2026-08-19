// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GasboyDeviceSummaryModel.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   The model for the Gasboy Device Summary Page
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.Afss.WebApp.Areas.AutomatedFuelStation.Models
{
    using System;
    using System.Collections.Generic;

    using FuelsManager.Afss.Module.Gasboy.BusinessObjects.DataObjects;

    /// <summary>
    ///  The model for the Gasboy Device Summary Page
    /// </summary>
    public class GasboyDeviceSummaryModel
    {
        /// <summary>
        /// Initialize collections to avoid potential null reference errors should an error occur.
        /// </summary>
        public GasboyDeviceSummaryModel()
        {
            this.GasboyDevices = new List<GasboyDevice>();
        }

        /// <summary>
        /// The text provided by the user in the find text box to filter the results on the summary page
        /// </summary>
        public string FindText { get; set; }

        /// <summary>
        /// Gasboy devices to display
        /// </summary>
        public List<GasboyDevice> GasboyDevices { get; set; }

        /// <summary>
        /// True if the user has permission to add new gasboy device from the gasboy device summary page
        /// </summary>
        public bool IsEditable { get; set; }

        /// <summary>
        /// Identifies the current site. Used to determine if the delete button should be disabled for an gasboy device in the grid.
        /// </summary>
        public Guid SiteGuid { get; set; }
    }
}
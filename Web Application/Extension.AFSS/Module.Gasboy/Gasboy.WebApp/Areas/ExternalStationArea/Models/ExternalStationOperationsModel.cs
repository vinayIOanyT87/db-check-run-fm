// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExternalStationOperationsModel.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Represents the data displayed on the External Station Operations page.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FuelsManager.Afss.Module.Gasboy.WebApp.Areas.ExternalStationArea.Models
{
    using System.Collections.Generic;

    using FuelsManager.Afss.Module.Gasboy.BusinessObjects.DataObjects;

    /// <summary>
    /// Represents the data displayed on the External Station Operations page.
    /// </summary>
    public class ExternalStationOperationsModel
    {
        /// <summary>
        /// Initialize collections to avoid potential null reference errors should an error occur.
        /// </summary>
        public ExternalStationOperationsModel()
        {
            this.ExternalStations = new List<GasboyStation>();
        }

        /// <summary>
        /// The external stations to display on the page
        /// </summary>
        public List<GasboyStation> ExternalStations { get; set; }

        /// <summary>
        /// If the user wants to download a range of transactions, this is the first transaction ID we should attempt to download
        /// </summary>
        public long? TransactionIDRangeStart { get; set; }

        /// <summary>
        /// If the user wants to download a range of transactions, this is the last transaction ID we should attempt to download
        /// </summary>
        public long? TransactionIDRangeEnd { get; set; }

        /// <summary>
        /// The format to use when displaying dates
        /// </summary>
        public string ShortDatePattern { get; set; }

        /// <summary>
        /// The format to use when displaying times
        /// </summary>
        public string TimePattern { get; set; }

        /// <summary>
        /// True if the page is editable (i.e. the user can initiate download requests)
        /// </summary>
        public bool IsEditable { get; set; }
    }
}
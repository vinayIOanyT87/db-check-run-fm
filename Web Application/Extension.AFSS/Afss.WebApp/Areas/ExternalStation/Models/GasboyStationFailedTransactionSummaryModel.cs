// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExternalStationFailedTransactionSummaryModel.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   The model for the failed transaction summary screen
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.Afss.WebApp.Areas.ExternalStation.Models
{
    using System;
    using System.Collections.Generic;

    using FuelsManager.Afss.Module.Gasboy.BusinessObjects.DataObjects;

    /// <summary>
    /// The model for the failed transaction summary screen
    /// </summary>
    public class GasboyStationFailedTransactionSummaryModel
    {
        /// <summary>
        /// Initialize reference objects to avoid null reference exceptions if something goes wrong when populating the model
        /// </summary>
        public GasboyStationFailedTransactionSummaryModel()
        {
            this.FailedTransactions = new List<GasboyStationTransaction>();
            this.ExternalStations = new List<GasboyStation>();
        }

        /// <summary>
        /// Identifies the external station to display failed transactions for
        /// </summary>
        public Guid SelectedExternalStationGuid { get; set; }

        /// <summary>
        /// A list of the configured external stations to display in the drop down so users can filter the results to only those for a particular station
        /// </summary>
        public IEnumerable<GasboyStation> ExternalStations { get; set; }

        /// <summary>
        /// Failed transactions to display on the screen
        /// </summary>
        public List<GasboyStationTransaction> FailedTransactions { get; set; }

        /// <summary>
        /// The transaction ID search parameter value
        /// </summary>
        public string TransactionID { get; set; }

        /// <summary>
        /// The beginning date range to get failed transactions for
        /// </summary>
        public string BeginDate { get; set; }

        /// <summary>
        /// The time portion of the beginning date range to get failed transactions for
        /// </summary>
        public string BeginTime { get; set; }

        /// <summary>
        /// The ending date range to get failed transactions for
        /// </summary>
        public string EndDate { get; set; }

        /// <summary>
        /// The time portion of the ending date range to get failed transactions for
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

        /// <summary>
        /// True if the user has permission to delete failed transactions from the summary page
        /// </summary>
        public bool IsEditable { get; set; }
    }
}
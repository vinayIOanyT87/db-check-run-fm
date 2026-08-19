// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExternalStationFailedTransactionModel.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   The model for the External Station Failed Tranasction Detail Page
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.Afss.WebApp.Areas.AutomatedFuelStation.Models
{
    using FuelsManager.Afss.Module.Gasboy.BusinessObjects.DataObjects;

    /// <summary>
    /// The model for the External Station Failed Tranasction Detail Page
    /// </summary>
    public class GasboyStationFailedTransactionModel
    {
        /// <summary>
        /// Initialize reference types to avoid potential null reference exceptions
        /// </summary>
        public GasboyStationFailedTransactionModel()
        {
            this.FailedTransaction = new GasboyStationTransaction();
        }

        /// <summary>
        /// The failed transaction to display
        /// </summary>
        public GasboyStationTransaction FailedTransaction { get; set; }

        /// <summary>
        /// True if the user has permission to edit failed transactions
        /// </summary>
        public bool IsEditable { get; set; }
    }
}
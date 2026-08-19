// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GasboyStationGeneralConfiguration.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
// Represents configuration values that apply to all external stations
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FuelsManager.Afss.Module.Gasboy.BusinessObjects.DataObjects
{
    using System;
    using System.Runtime.Serialization;

    using FMBusinessObjects.DataObjects;

    /// <summary>
    /// Represents configuration values that apply to all external stations
    /// </summary>
    [DataContract]
    [Serializable]
    public class GasboyStationGeneralConfiguration : BaseDataObject
    {     
        /// <summary>
        /// The Transaction Alias Guid to use when creating FuelsManager transactions
        /// </summary>
        [DataMember]
        public Guid? RetailSaleTransactionAliasGuid { get; set; }

        /// <summary>
        /// The Name of the Transaction Alias to use when creating FuelsManager transactions
        /// </summary>
        [DataMember]
        public string RetailSaleTransactionAliasName { get; set; }

        /// <summary>
        /// The amount of time that should elapse between attempts to download transactions from the External Station
        /// </summary>
        [DataMember]
        public int? DownloadTransactionsIntervalMinutes { get; set; }

        /// <summary>
        /// The amount of time that should elapse between attempts to download events from the External Station
        /// </summary>
        [DataMember]
        public int? DownloadEventsIntervalMinutes { get; set; }

        /// <summary>
        /// Return the values in the External Station General Configuration Record to their original values
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            this.RetailSaleTransactionAliasGuid = Guid.Empty;
            this.RetailSaleTransactionAliasName = string.Empty;
            this.DownloadTransactionsIntervalMinutes = null;
            this.DownloadEventsIntervalMinutes = null;
        }
    }
}

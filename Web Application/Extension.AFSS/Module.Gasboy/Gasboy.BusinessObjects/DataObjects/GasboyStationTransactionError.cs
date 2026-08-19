// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GasboyStationFailedTransactionError.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Represents an error encountered when attempting to save an gasboy station transaction to FuelsManager
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FuelsManager.Afss.Module.Gasboy.BusinessObjects.DataObjects
{
    using System;
    using System.Runtime.Serialization;

    using FMBusinessObjects.DataObjects;

    /// <summary>
    /// Represents an error encountered when attempting to save an gasboy station transaction to FuelsManager
    /// </summary>
    [DataContract]
    public class GasboyStationTransactionError : BaseDataObject
    {
        /// <summary>
        /// The error message we received from FuelsManager Transaction Validation
        /// </summary>
        [DataMember]
        public string ErrorMessage { get; set; }

        /// <summary>
        /// The failed transaction this error message corresponds to
        /// </summary>
        [DataMember]
        public Guid ExternalStationTransactionGuid { get; set; }
    }
}

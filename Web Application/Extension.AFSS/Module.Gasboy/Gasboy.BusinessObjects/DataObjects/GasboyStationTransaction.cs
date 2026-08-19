// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GasboyStationTransaction.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Represents a transaction received from a Gasboy Station
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FuelsManager.Afss.Module.Gasboy.BusinessObjects.DataObjects
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Runtime.Serialization;
    using FMBusinessObjects.DataObjects;

    using FuelsManager.Afss.BusinessObjects.Constants;

    using Microsoft.SqlServer.Server;

    /// <summary>
    /// Represents a transaction received from a Gasboy Station
    /// </summary>
    [DataContract]
    [Serializable]
    public class GasboyStationTransaction : BaseDataObject
    {
        /// <summary>
        /// Constructor for an External Station Transaction object
        /// </summary>
        public GasboyStationTransaction()
        {
            this.TransactionErrors = new List<GasboyStationTransactionError>();
        }

        /// <summary>
        /// Identifies the external station which this transaction came from
        /// </summary>
        [DataMember]
        public Guid ExternalStationGuid { get; set; }

        /// <summary>
        /// The user friendly ID the external station which this transaction came from
        /// </summary>
        [DataMember]
        public string ExternalStationID { get; set; }

        /// <summary>
        /// The message as received from the external station
        /// </summary>
        [DataMember]
        public string RawTransactionData { get; set; }

        /// <summary>
        /// Internal id of Fleet that this device belongs to 
        /// </summary>
        [DataMember]
        public string FleetID { get; set; }

        /// <summary>
        /// Code of fueling device fleet 
        /// </summary>
        [DataMember]
        public string FleetCode { get; set; }

        /// <summary>
        /// Name of fueling device fleet 
        /// </summary>
        [DataMember]
        public string FleetName { get; set; }

        /// <summary>
        /// Code of product used 
        /// </summary>
        [DataMember]
        public string ProductCode { get; set; }

        /// <summary>
        /// Name of product used 
        /// </summary>
        [DataMember]
        public string ProductName { get; set; }

        /// <summary>
        /// Internal id of fueling device 
        /// </summary>
        [DataMember]
        public string MeanID { get; set; }

        /// <summary>
        /// Name of device that fueled 
        /// </summary>
        [DataMember]
        public string MeanName { get; set; }

        /// <summary>
        /// The plate of the fueling vehicle 
        /// </summary>
        [DataMember]
        public string FuelingVehiclePlate { get; set; }

        /// <summary>
        /// For 2-stage transactions, this is the driver portion of the authorization- Internal Id of Driver Device 
        /// </summary>
        [DataMember]
        public string DriverMeanID { get; set; }

        /// <summary>
        /// For 2-stage transactions, this is the driver portion of the authorization- Device Plate 
        /// </summary>
        [DataMember]
        public string DriverPlate { get; set; }

        /// <summary>
        /// For 2-stage transactions, this is the driver portion of the authorization- Device Card # 
        /// </summary>
        [DataMember]
        public string DriverTag { get; set; }

        /// <summary>
        /// External Number received from another system  
        /// </summary>
        [DataMember]
        public string ExternalAuthorizationNumber { get; set; }

        /// <summary>
        /// Density 
        /// </summary>
        [DataMember]
        public string Density { get; set; }

        /// <summary>
        /// Temperature 
        /// </summary>
        [DataMember]
        public string Temperature { get; set; }

        /// <summary>
        /// Reading as recorded by VIS equipment or entered by OrPT user 
        /// </summary>
        [DataMember]
        public string EngineHours { get; set; }

        /// <summary>
        /// Internal id of pump used 
        /// </summary>
        [DataMember]
        public string PumpID { get; set; }

        /// <summary>
        /// Pump number 
        /// </summary>
        [DataMember]
        public string Pump { get; set; }

        /// <summary>
        /// Internal id of nozzle used 
        /// </summary>
        [DataMember]
        public string NozzleID { get; set; }

        /// <summary>
        /// Relative number of nozzle within pump 
        /// </summary>
        [DataMember]
        public string Nozzle { get; set; }

        /// <summary>
        /// Unique number of nozzle across all pumps in station  
        /// </summary>
        [DataMember]
        public string HoseNumber { get; set; }

        /// <summary>
        /// Name of tank used 
        /// </summary>
        [DataMember]
        public string TankName { get; set; }

        /// <summary>
        /// Which shift was open at time of this transaction 
        /// </summary>
        [DataMember]
        public string ShiftID { get; set; }

        /// <summary>
        /// Reading as recorded by VIS equipment or entered by OrPT user 
        /// </summary>
        [DataMember]
        public string Odometer { get; set; }

        /// <summary>
        /// This is the volume of fuel sold, or the number of dry-product units sold  
        /// </summary>
        [DataMember]
        public string Quantity { get; set; }

        /// <summary>
        /// Price per unit volume for fuel, or price per item for dry product transaction  
        /// </summary>
        [DataMember]
        public string PricePerVolume { get; set; }

        /// <summary>
        /// This is quantity*ppv  
        /// </summary>
        [DataMember]
        public string TotalPrice { get; set; }

        /// <summary>
        /// Unique ID of proxy device (0 if none) 
        /// </summary>
        [DataMember]
        public string ProxyDeviceID { get; set; }

        /// <summary>
        /// When transaction was completed (YYYY-MM-DD HH:MM:SS)  
        /// </summary>
        [DataMember]
        public string TransactionTimeStamp { get; set; }

        /// <summary>
        /// Rough categorization of type of transaction: ATDNT=Attendant; AUTO=auto-authorize; 
        /// BOS=self-service card or auth from screen; CSTMR=customer tag or vehicle mounted; FPOS=Axalto
        /// </summary>
        [DataMember]
        public string TransactionType { get; set; }

        /// <summary>
        /// Raw track data encoded on tag, magnetic card, VIS or other device 
        /// </summary>
        [DataMember]
        public string TrackData1 { get; set; }

        /// <summary>
        /// Raw track data encoded on tag, magnetic card, VIS or other device 
        /// </summary>
        [DataMember]
        public string TrackData2 { get; set; }

        /// <summary>
        /// This is the string of the mean as was encoded in the tracks of the tag, magnetic card, VIS or other authorization devices 
        /// </summary>
        [DataMember]
        public string Tag { get; set; }

        /// <summary>
        /// Id of cash customer for this transaction 
        /// </summary>
        [DataMember]
        public string CashCustomerID { get; set; }

        /// <summary>
        /// Status of the external station transaction
        /// </summary>
        [DataMember]
        public ExternalStationTransactionStatus ExternalStationTransactionStatus { get; set; }

        /// <summary>
        /// Failed status of the external station transaction that is in a failed state
        /// </summary>
        [DataMember]
        public ExternalStationTransactionFailedStatus ExternalStationTransactionFailedStatus { get; set; }

        /// <summary>
        /// Validation errors generated by FuelsManager when attempting to save the transaction
        /// </summary>
        [DataMember]
        public List<GasboyStationTransactionError> TransactionErrors { get; set; }

		/// <summary>
		/// The Driver name, to be mapped to customer name in FMD transaction
		/// </summary>
		[DataMember]
		public string DriverName { get; set; }

        /// <summary>
        /// Return the values in the External Station Transaction to their original values
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            this.ExternalStationGuid = Guid.Empty;
            this.ExternalStationID = string.Empty;
            this.RawTransactionData = string.Empty;
            this.FleetID = string.Empty;
            this.FleetCode = string.Empty;
            this.FleetName = string.Empty;
            this.ProductCode = string.Empty;
            this.ProductName = string.Empty;
            this.MeanID = string.Empty;
            this.MeanName = string.Empty;
            this.FuelingVehiclePlate = string.Empty;
            this.DriverMeanID = string.Empty;
            this.DriverPlate = string.Empty;
            this.DriverTag = string.Empty;
	         this.DriverName = string.Empty;
            this.ExternalAuthorizationNumber = string.Empty;
            this.Density = string.Empty;
            this.Temperature = string.Empty;
            this.EngineHours = string.Empty;
            this.PumpID = string.Empty;
            this.Pump = string.Empty;
            this.NozzleID = string.Empty;
            this.Nozzle = string.Empty;
            this.HoseNumber = string.Empty;
            this.TankName = string.Empty;
            this.ShiftID = string.Empty;
            this.Odometer = string.Empty;
            this.Quantity = string.Empty;
            this.PricePerVolume = string.Empty;
            this.TotalPrice = string.Empty;
            this.ProxyDeviceID = string.Empty;
            this.TransactionTimeStamp = string.Empty;
            this.TransactionType = string.Empty;
            this.TrackData1 = string.Empty;
            this.TrackData2 = string.Empty;
            this.Tag = string.Empty;
            this.CashCustomerID = string.Empty;
            this.ExternalStationTransactionStatus = ExternalStationTransactionStatus.None;
            this.ExternalStationTransactionFailedStatus = ExternalStationTransactionFailedStatus.None;
            this.TransactionErrors = new List<GasboyStationTransactionError>();
        }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GasboyStationEvent.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Represents an event downloaded from the gasboy station
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.Afss.Module.Gasboy.BusinessObjects.DataObjects
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;

    /// <summary>
    /// The object types an event can be associated with
    /// </summary>
    public enum GasboyEventObjectType
    {
        Generic = 0,
        Tank = 1,
        Probe = 2,
        Product = 3,
        Station = 4,
        Mean = 5,
        Shift = 6,
        Bus = 7,
        Nozzle = 8,
        Sensor = 9,
        Input = 10
    }

    /// <summary>
    /// The values of the error class code field in the event
    /// </summary>
    public enum GasboyEventErrorClassCode
    {
        All = 0,
        Authorization = 1,
        CMSPull = 2,
        Communication = 3,
        HeadOffice = 4,
        Screens = 5,
        Operation = 6, 
        System = 7
    }

	/// <summary>
	/// The values of the error code field in the event
	/// </summary>
	public enum ErrorCode
	{
		[Description("Station Time Out")]TimeOut = -107,
		[Description("Driver Active")]DriverActive = -921,
		[Description("Communication Error with Device")]DeviceDefunct = -940,
		[Description("Communication Restored with device")]DeviceNormal = -941,
		[Description("STX Expected")]STXExpected = -104,
		[Description("Failed to Authorize")]FailedAuthorize = -411,
		[Description("Shift Start")]ShiftStart = -403,
		[Description("Shift End")]ShiftEnd = -404,
		[Description("Failed to authorize fuel type check")]FailedOnFuel = -412,
		[Description("Failed to authorize fuel limit for vehicle and ID")]FailedOnLimit = -413,
		[Description("SysInit Called")]SysinitCalled = -306,
		[Description("Reload Called")]ReloadCalled = -307,
		[Description("Pump Initialized")]InitPump = -308,
		[Description("Process Code is out of sync. Transaction may not have been written")]InvalidProcessCode = -405,
		[Description("Mean has reached its fueling limit")]MaxRefuelingReached=-414,
		[Description("Failed to Authorize Orsan Resson Communication")]OrsanCommError= -407,
		[Description("Price update failed")]PriceUpdateFail= -415,
		[Description("Fueling Card Authorization failed")]FuelingCardAuthFail= -408,
		[Description("Price update sent")]PriceUpdateSent = -410,
		[Description("Pump ack new price")]PumpAckNewPrice = -416,
		[Description("Pump nack new price")]PumpNackNewPrice= -417,
		[Description("Pump Price Incorrect")]PumpPriceIncorrect= -418,
		[Description("Pump authorize command failed")]PumpAuthSendFail= -419,
		[Description("Incorrect nozzle was lifted and transaction canceled")]IncorrectNozzle= -420,
		[Description("Pump ready status timeout")]PumpReadyTimeout = -421,
		[Description("Fleet head office service started")]FHOServiceStarted= -801,
		[Description("Station was added to HeadOffice")]StationAddedFHO= -802,
		[Description("Station was deleted from HeadOffice")]StationDeletedFHO= -803,
		[Description("Station properties updated")]StationPropertyUpdated= -804,
		[Description("Fleet Credit has been exceeded")]FleetCreditExceeded= -430,
		[Description("Vehicle blocked or not found")]VehicleBlocked= -431,
		[Description("Not allowed to fuel in this time range")]Range= -432,
		[Description("Fleet blocked or not found")]FleetBlocked= -436,
		[Description("Number of vists has been exceeded")]Visit= -437,
		[Description("Cluster not allowed to fuel at this station")]Cluster= -438,
		[Description("Device credit has been exceeded")]RecCredit= -439,
		[Description("Fleet HeadOffice is offline")]FHOOffline= -440,
		[Description("Internal Trace")]Trace= -500,
		[Description("Too many digits from pump")]TooManyDigitsFromPump= -442,
		[Description("Pump nozzle was not lifted, fueling canceled")]NozzleNotLifted= -423,
		[Description("Multiple Nozzles Lifted")]MultipleNozzlesLifted= -443,
		[Description("No Nozzle Lifted")]NoNozzleLifted= -444,
		[Description("Pump not in Open Shift")]PumpNotOpenShift= -445,
		[Description("Pump not available for fueling")]PumpNotAvailable= -446,
		[Description("Attendant Tag not in shift")]AttendantTagNotOpenShift= -447,
		[Description("Pump not in Open Shift")]PumpNotInOpenShift= -448,
		[Description("Product not authorized")]ProductNotAuthorized= -449,
		[Description("Fueling not authorized")]FuelingNotAuthorized= -450,
		[Description("No Pump Assigned")]NoPumpAssigned= -451,
		[Description("Assigned pump not on tag reader")]PumpNotOnTagReader= -452,
		[Description("Multiple Fueling not allowed")]NoMultipleFueling= -453,
		[Description("Unrecognized Tag")]UnrecognizedTag= -454,
		[Description("Bad tag format")]BadTagFormat= -455,
		[Description("Preset Fueling incomplete")]PresetFuelingIncomplete= -441,
		[Description("Blacklisted mean" )]BlacklistedMean = -457,
		[Description("Blocked mean")]BlockedMean= -458,
		[Description("Blocked Fleet")]BlockedFleet= -459,
		[Description("Transaction rejected by flow rate")]FlowRate= -465,
		[Description("Wrong PIN was entered")]WrongPIN= -466,
		[Description("Tag blocked due to wrong PIN")]TagBlockedByPIN= -467,
		[Description("Pump is busy, cannot authorize")]PumpBusy= -468,

		[Description("Bus updated during setup")]
		BusUpdated = -959,
		[Description("Device updated during setup")]
		DeviceUpdated = -956,

		[Description("Pump Bypass Off")]
		BypassOff = -476,
		[Description("Pump Bypass On")]
		BypassOn = -475,
	}

    /// <summary>
    /// Represents an event downloaded from the gasboy station
    /// </summary>
    [DataContract]
    [Serializable]
    public class GasboyStationEvent : GasboyStationLog
    {
        /// <summary>
        /// Identifies the log record this event is associated with
        /// </summary>
        [DataMember]
        public Guid ExternalStationLogGuid { get; set; }

        /// <summary>
        /// An external unique identifier which uniquely identifies the event on the station
        /// </summary>
        [DataMember]
        public int? EventID { get; set; }

        /// <summary>
        /// Identifies the type of error we've received, e.g. Authorization or Communication
        /// </summary>
        [DataMember]
        public GasboyEventErrorClassCode? ErrorClassCode { get; set; }

        /// <summary>
        /// This number, usually negative, identifies the error. 
        /// </summary>
        [DataMember]
        public ErrorCode? ErrorCode { get; set; }


        /// <summary>
        /// An external unique identifier which uniquely identifies the fleet the event is associated with on the station
        /// </summary>
        [DataMember]
        public int? FleetID { get; set; }

        /// <summary>
        /// An external unique identifier which uniquely identifies the object the event is associated with on the station
        /// </summary>
        [DataMember]
        public int? ObjectID { get; set; }

        /// <summary>
        /// An additional categorization of the type of alarm. See the enumeration for the potential values.
        /// </summary>
        [DataMember]
        public GasboyEventObjectType? EventObjectType { get; set; }

        /// <summary>
        /// Additional textual information about the error/event 
        /// </summary>
        [DataMember]
        public string DeviceName { get; set; }

        /// <summary>
        /// Fields 1 through 8 are additional fields that help describe the error/event. 
        /// The parameters are usually plugged in to slots (#!#, #2#, etc.) in the text of the error found in META_DATA.event_language_matrix 
        /// </summary>
        [DataMember]
        public string Field1 { get; set; }

        /// <summary>
        /// Fields 1 through 8 are additional fields that help describe the error/event. 
        /// The parameters are usually plugged in to slots (#!#, #2#, etc.) in the text of the error found in META_DATA.event_language_matrix 
        /// </summary>
        [DataMember]
        public string Field2 { get; set; }

        /// <summary>
        /// Fields 1 through 8 are additional fields that help describe the error/event. 
        /// The parameters are usually plugged in to slots (#!#, #2#, etc.) in the text of the error found in META_DATA.event_language_matrix 
        /// </summary>
        [DataMember]
        public string Field3 { get; set; }

        /// <summary>
        /// Fields 1 through 8 are additional fields that help describe the error/event. 
        /// The parameters are usually plugged in to slots (#!#, #2#, etc.) in the text of the error found in META_DATA.event_language_matrix 
        /// </summary>
        [DataMember]
        public string Field4 { get; set; }

        /// <summary>
        /// Fields 1 through 8 are additional fields that help describe the error/event. 
        /// The parameters are usually plugged in to slots (#!#, #2#, etc.) in the text of the error found in META_DATA.event_language_matrix 
        /// </summary>
        [DataMember]
        public string Field5 { get; set; }

        /// <summary>
        /// Fields 1 through 8 are additional fields that help describe the error/event. 
        /// The parameters are usually plugged in to slots (#!#, #2#, etc.) in the text of the error found in META_DATA.event_language_matrix 
        /// </summary>
        [DataMember]
        public string Field6 { get; set; }

        /// <summary>
        /// Fields 1 through 8 are additional fields that help describe the error/event. 
        /// The parameters are usually plugged in to slots (#!#, #2#, etc.) in the text of the error found in META_DATA.event_language_matrix 
        /// </summary>
        [DataMember]
        public string Field7 { get; set; }

        /// <summary>
        /// Fields 1 through 8 are additional fields that help describe the error/event. 
        /// The parameters are usually plugged in to slots (#!#, #2#, etc.) in the text of the error found in META_DATA.event_language_matrix 
        /// </summary>
        [DataMember]
        public string Field8 { get; set; }

        /// <summary>
        /// Return the values in the Gasboy station event to their original values
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            this.ExternalStationLogGuid = Guid.Empty;
            this.EventID = null;
            this.ErrorClassCode = null;
            this.ErrorCode = null;
            this.FleetID = null;
            this.ObjectID = null;
            this.EventObjectType = null;
            this.DeviceName = string.Empty;
            this.Field1 = string.Empty;
            this.Field2 = string.Empty;
            this.Field3 = string.Empty;
            this.Field4 = string.Empty;
            this.Field5 = string.Empty;
            this.Field6 = string.Empty;
            this.Field7 = string.Empty;
            this.Field8 = string.Empty;
        }
    }
}

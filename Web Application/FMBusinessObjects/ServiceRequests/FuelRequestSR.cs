// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FuelRequestSR.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
// Represents the types of requests that can be initiated through the Fuel Request Form
// Each type of request also has a subtype, for example, you can have a 
// Fast Log Refuel request or a Transient Defuel request
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FMBusinessObjects.ServiceRequests
{
	using System.Runtime.Serialization;
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Represents the types of requests that can be initiated through the Fuel Request Form
	/// Each type of request also has a subtype, for example, you can have a 
	/// Fast Log Refuel request or a Transient Defuel request
	/// </summary>
	public enum FuelRequestType
	{
		/// <summary>
		/// The initial setting, which we should never get when the page is in use.
		/// </summary>
		Unknown = 0,

		/// <summary>
		/// A regular fuel request, typically entered in real time with dispatching personnel. Either a Refuel or Defuel.
		/// </summary>
		RequestFuel = 1,

		/// <summary>
		/// A fuel request for an aircraft not stationed at your site. Either a Refuel or Defuel.
		/// </summary>
		Transient = 2,

		/// <summary>
		/// A fuel request where all information is already known, typically entered after the request is complete. Either a Refuel or Defuel.
		/// </summary>
		FastLog = 3,

		/// <summary>
		/// A Fill, a Partial Fill, or a Return to Bulk
		/// </summary>
		FillStand = 4,

		/// <summary>
		/// A Fill, a Partial Fill, or a Return to Bulk
		/// </summary>
		FastLogFillStand = 5
	}

	/// <summary>
	/// Represents a Fuel Request. Contains transaction data from the Fuel Request form,
	/// information about the type of request, and configuration information
	/// </summary>
	[DataContract]
	public class FuelRequestSR
	{
		// These constants represent the different sub types of requests that can be made. 
		// This value is selected on the Service Request tab of the Fuel Request Form
		public const string RefuelRequestSubType = "Refuel";
		public const string DefuelRequestSubType = "Defuel";
		public const string ReturnToBulkRequestSubType = "Return to Bulk";
		public const string FillRequestSubType = "Fill";
		public const string PartialFillRequestSubType = "Partial Fill";
		public const string PartialReturnToBulkSubType = "Partial Return to Bulk";

		/// <summary>
		/// When the object is constructed, set initial values for the properties contained in the class
		/// </summary>
		public FuelRequestSR()
		{
			this.Transaction = null;
			this.RequestType = FuelRequestType.Unknown;
			this.RequestSubType = string.Empty;
			this.CurrentConsecutiveOOSVariance = 0;
			this.TransactionOriginallyCompleted = false;
		}

		/// <summary>
		/// Contains data entered by the user on the form.
		/// </summary>
		[DataMember]
		public TransactionDO Transaction { get; set; }

		/// <summary>
		/// The type of fuel request which corresponds to the current transaction
		/// </summary>
		[DataMember]
		public FuelRequestType RequestType { get; set; }

		/// <summary>
		/// Represents the request subtype (e.g. Refuel, defuel, fill, return to bulk)
		/// </summary>
		[DataMember]
		public string RequestSubType { get; set; }

		/// <summary>
		/// Used to determine if the variance is out of tolerance. If it is, we may add notes
		/// to the transaction describing the problem
		/// </summary>
		[DataMember]
		public int CurrentConsecutiveOOSVariance { get; set; }
		
		/// <summary>
		/// Was the transaction completed when it was first retrieved (before any changes by the user)?
		/// </summary>
		[DataMember]
		public bool TransactionOriginallyCompleted { get; set; }
	}

	/// <summary>
	/// Represents a result returned by processing a Fuel Request.
	/// Sometimes we return a warning that the variance is out of tolerance,
	/// this class contains that warning.
	/// </summary>
	[DataContract]
	public class FuelRequestResult
	{
		/// <summary>
		/// When the object is constructed, set initial values for the properties contained in the class
		/// </summary>
		public FuelRequestResult()
		{
			this.AlertMessage = string.Empty;
		}

		/// <summary>
		/// Contains a warning message that we need to display to the user
		/// </summary>
		[DataMember]
		public string AlertMessage { get; set; }
	}
}

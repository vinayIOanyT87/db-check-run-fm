///***************************************************************************
/// Module Name:  AutoDistributionThruputSR
/// Author:       Daniel Or
/// Copyright (c) Varec, Inc.  All rights reserved.
///***************************************************************************

using System;
using System.Runtime.Serialization;

namespace FMBusinessObjects.ServiceRequests
{
	/// <summary>
	/// This is the service request data block for the AutoDistributionProcessor.
	/// </summary>
	[DataContract]
	public class AutoDistributionThruputSR
	{
		[DataMember]
		public Guid RuleGuid { get; set; }

		[DataMember]
		public Guid ManagerGuid { get; set; }

		[DataMember]
		public Guid ProductGuid { get; set; }

		[DataMember]
		public DateTimeOffset StartDate { get; set; }

		[DataMember]
		public DateTimeOffset EndDate { get; set; }
	}
}

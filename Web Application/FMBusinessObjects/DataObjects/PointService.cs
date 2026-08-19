// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PointService.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Runtime.Serialization;

	using FMBusinessObjects.Attributes;

	[DataContract]
	[Serializable]
	public class PointService : BaseDataObject
	{
		[FMPersistedField]
		public Guid PointServiceGuid
		{
			get
			{
				return this.IdentityGuid;
			}
			set
			{
				this.IdentityGuid = value;
			}
		}

		[DataMember]
		[FMPersistedField]
		public string Hostname { get; set; }

		[DataMember]
		[FMPersistedField]
		public DateTimeOffset LastPingTime { get; set; }

		[DataMember]
		[FMPersistedField]
		public int PingIntervalInSeconds { get; set; }

		[DataMember]
		[FMPersistedField]
		public int HealthStatusIndex { get; set; }

		[DataMember]
		[FMPersistedField]
		public double PercentCpuUtilization { get; set; }

		[DataMember]
		[FMPersistedField]
		public double PercentCpuUtilizationThrottleLevel { get; set; }

		[DataMember]
		[FMPersistedField]
		public double PercentMemoryUtilization { get; set; }

		[DataMember]
		[FMPersistedField]
		public double PercentMemoryUtilizationThrottleLevel { get; set; }

        [DataMember]
        [FMPersistedField]
        public int MaxNumberOfPoints { get; set; }

        public override Guid SiteGuid { get; set; }

		public override string ID { get; set; }
	}
}
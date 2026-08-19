// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AlarmAndEventDescriptorClass.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the AlarmAndEventDescriptorClass type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Runtime.Serialization;

	/// <summary>
	/// The AlarmAndEventDiscovery interface.
	/// </summary>
	public interface IAlarmAndEventDiscovery
	{
		/// <summary>
		/// Gets the alarm and events.
		/// </summary>
		AlarmAndEventDescriptorClass[] AlarmAndEvents { get; }
	}

	/// <summary>
	/// The alarm and event descriptor class.
	/// </summary>
	[DataContract]
	[Serializable]
	public class AlarmAndEventDescriptorClass : BaseObjectClass
	{
		/// <summary>
		/// Gets or sets a value indicating whether alarm.
		/// </summary>
		[DataMember]
		public bool Alarm { get; set; }

		/// <summary>
		/// Gets or sets the source.
		/// </summary>
		[DataMember]
		public string Source { get; set; }

		/// <summary>
		/// Gets or sets the id.
		/// </summary>
		[DataMember]
		public string ID { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="AlarmAndEventDescriptorClass"/> class.
		/// </summary>
		/// <param name="alarm">
		/// The alarm.
		/// </param>
		/// <param name="source">
		/// The source.
		/// </param>
		/// <param name="id">
		/// The ID.
		/// </param>
		public AlarmAndEventDescriptorClass(bool alarm, string source, string id)
		{
			this.Alarm = alarm;
			this.Source = source;
			this.ID = id;
		}
	}
}
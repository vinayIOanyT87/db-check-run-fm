// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SynchronizationElement.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FMBusinessObjects.DataObjects
{
	using System;

	public sealed class AlarmAndEventSynchronizationElement
	{
		public string TableName;
		public Guid SiteGuid;
		public DateTimeOffset LastAlarmAndEventTimeStamp;
		public int NumberOfRecordsSynchronized;
	}
}
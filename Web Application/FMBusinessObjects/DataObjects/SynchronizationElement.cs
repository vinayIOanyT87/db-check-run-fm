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

	public sealed class SynchronizationElement
	{
		public string TableName;
		public Guid SiteGuid;
		public DateTimeOffset LastValueTimeStamp;
		public Guid LastPointValueGuid;
		public string LastPointValuePropertyID;
		public int NumberOfRecordsSynchronized;
	}
}
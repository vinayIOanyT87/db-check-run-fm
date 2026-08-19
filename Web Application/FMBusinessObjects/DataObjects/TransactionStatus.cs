// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactionStatus.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Summary description for TransactionStatus.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.DataObjects
{
	/// <summary>
	/// Enumeration of values for Transaction Status.
	/// </summary>
	public enum TransactionStatus
	{
		Completed,
		InProgress,
		Dispatched,
		Requested,
		Closed,
		OnHold,
		Scheduled,
		Cancelled,
		Acknowledged,
		LoadPending,
		WeighOutPending,
		Posted,
		Arrived,
		Started,
		Stopped,
		Suspended,
		Pending,
		Updated,
		Superseded,
        Enterprise,
		Pushed,
		Pulled
	}
}

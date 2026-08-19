// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFMEventLog.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the Event log interface type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.BusinessInterfaces
{
	using System.ServiceModel;

	using FMBusinessObjects.Constants;

	/// <summary>
	/// The FM Event log interface definition
	/// </summary>
	[ServiceContract]
	public interface IFMEventLog
	{
		#region Public Methods and Operators

		/// <summary>
		/// Writes the entry.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="entryType">Type of the entry.</param>
		[OperationContract]
		void WriteEntry(string message, FMEventLogEntryType entryType);

		#endregion
	}
}
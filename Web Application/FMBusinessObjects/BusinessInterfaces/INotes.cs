// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INotes.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the INotes type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Interface for using the Notes service class.
	/// </summary>
	[ServiceContract]
	public interface INotes
	{
		#region Public Methods and Operators

		/// <summary>
		/// Adds the specified note object.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="note">The note object to add.</param>
		/// <returns>The identity Guid of the newly added note record.</returns>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Guid Add(SecurityClass security, NoteClass note);

		/// <summary>
		/// Gets the specified security.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="identityGuid">The identity GUID of the note to get.</param>
		/// <returns>The request note object or null if not found.</returns>
		[OperationContract]
		NoteClass Get(SecurityClass security, Guid identityGuid);

		/// <summary>
		/// Modifies the specified security.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="note">The note to save.</param>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, NoteClass note);

		/// <summary>
		/// Purges the specified security.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="noteGuid">The identity GUID of the note to purge.</param>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, Guid noteGuid);

		#endregion
	}
}
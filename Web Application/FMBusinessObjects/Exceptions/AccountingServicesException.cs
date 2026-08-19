// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccountingServicesException.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the AccountingServicesException type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.Exceptions
{
	using System;
	using System.Runtime.Serialization;

	/// <summary>
	/// An exception wrapper for reporting accounting services errors.
	/// </summary>
	[Serializable]
	public class AccountingServicesException : Exception
	{
		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="AccountingServicesException"/> class.
		/// </summary>
		public AccountingServicesException()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AccountingServicesException"/> class.
		/// </summary>
		/// <param name="msg">
		/// The msg to include in the exception.
		/// </param>
		public AccountingServicesException(string msg)
			: base(msg)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AccountingServicesException"/> class.
		/// </summary>
		/// <param name="info">
		/// The serialization info object.
		/// </param>
		/// <param name="context">
		/// The streaming context object.
		/// </param>
		protected AccountingServicesException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		#endregion
	}
}
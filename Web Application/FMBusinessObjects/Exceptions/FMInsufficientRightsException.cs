// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMInsufficientRightsException.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Exception thrown when user does not have sufficient rights for requested operation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FMBusinessObjects.Exceptions
{
	using System;
	using System.Runtime.Serialization;

	/// <summary>
	/// Exception thrown when user does not have sufficient rights for requested operation.
	/// </summary>
	[Serializable]
	public class FMInsufficientRightsException : UnauthorizedAccessException
	{
		#region Constants

		/// <summary>
		/// The exception message text.
		/// </summary>
		public const string ExceptionMessage = "Access Denied";

		public const int StatusCode = 550;

		public const string StatusCodeName = "AccessDenied";

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="FMInsufficientRightsException"/> class.
		/// </summary>
		public FMInsufficientRightsException()
			: base( ExceptionMessage )
		{
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="FMInsufficientRightsException"/> class with 
        /// a specified message.
        /// </summary>
        /// <param name="message">The object that holds the serialized object data.</param>
        public FMInsufficientRightsException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FMInsufficientRightsException"/> class.
        /// </summary>
        /// <param name="info">The object that holds the serialized object data.</param>
        /// <param name="context">The contextual information about the source or destination.</param>
        public FMInsufficientRightsException( SerializationInfo info, StreamingContext context )
			: base( info, context )
		{
		}

		#endregion
	}
}
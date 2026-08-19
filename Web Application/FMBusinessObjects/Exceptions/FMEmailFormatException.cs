// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMEmailFormatException.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Exception thrown when an email format validation fails.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FMBusinessObjects.Exceptions
{
	using System;
	using System.Runtime.Serialization;

	/// <summary>
	/// Exception thrown when an email format validation fails.
	/// </summary>
	[Serializable]
	public class FMEmailFormatException : ApplicationException
	{
		#region Constants

		/// <summary>
		/// The exception message text.
		/// </summary>
		public const string ExceptionMessage = "Email address is not recognized as a valid format.";

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="FMEmailFormatException"/> class.
		/// </summary>
		public FMEmailFormatException()
			: base( ExceptionMessage )
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FMEmailFormatException"/> class.
		/// </summary>
		public FMEmailFormatException( string fieldName )
			: base( string.Format("({0}) " + ExceptionMessage, fieldName))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FMEmailFormatException"/> class.
		/// </summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		public FMEmailFormatException( SerializationInfo info, StreamingContext context )
			: base( info, context )
		{
		}

		#endregion
	}
}
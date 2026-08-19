// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMNoQueryTypesAvailableException.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Exception thrown when no topics are found for query definitions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FMBusinessObjects.Exceptions
{
	using System;
	using System.Runtime.Serialization;

	/// <summary>
	/// Exception thrown when no topics are found for query definitions.
	/// </summary>
	[Serializable]
	public class FMNoQueryTypesAvailableException : ApplicationException
	{
		#region Constants

		/// <summary>
		/// The exception message text.
		/// </summary>
		public const string ExceptionMessage = "No query topic types available.";

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="FMNoQueryTypesAvailableException"/> class.
		/// </summary>
		public FMNoQueryTypesAvailableException()
			: base( ExceptionMessage )
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FMNoQueryTypesAvailableException"/> class.
		/// </summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		public FMNoQueryTypesAvailableException( SerializationInfo info, StreamingContext context )
			: base( info, context )
		{
		}

		#endregion
	}
}
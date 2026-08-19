// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IndexNotFoundException.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Exception used to indicate that a key index was not found.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FMBusinessObjects.Exceptions
{
	using System;

	/// <summary>
	/// Exception used to indicate that a key index was not found.
	/// </summary>
	public class IndexNotFoundException : Exception
	{
		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="IndexNotFoundException"/> class. 
		///     This is the default constructor for the index not found exception class.
		/// </summary>
		public IndexNotFoundException()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="IndexNotFoundException"/> class. 
		/// This constructor initializes the message.
		/// </summary>
		/// <param name="message">
		/// Custom error message for the exception.
		/// </param>
		public IndexNotFoundException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="IndexNotFoundException"/> class. 
		/// This constructor initializes the message and the inner exception
		/// </summary>
		/// <param name="message">Custom error message for the exception.</param>
		/// <param name="innerException">Associated exception.</param>
		public IndexNotFoundException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		#endregion
	}
}
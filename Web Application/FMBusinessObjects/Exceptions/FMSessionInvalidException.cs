// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMSessionInvalidException.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Exception thrown when session is deteremined to be invalid.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FMBusinessObjects.Exceptions
{
	using System;
	using System.Runtime.Serialization;

	/// <summary>
	/// Exception thrown when session is deteremined to be invalid.
	/// </summary>
	[Serializable]
	public class FMSessionInvalidException : ApplicationException
	{
		#region Constants

		/// <summary>
		/// The exception message text.
		/// </summary>
		public const string SessionNotFoundExceptionMessage = "Session not found or timed-out.";
      public const string SessionTimedOutExceptionMessage = "Session timed-out.";

      private const int StatusCode = 550;

      private const string StatusCodeName = "SessionInvalid";

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="FMSessionInvalidException"/> class.
		/// </summary>
		public FMSessionInvalidException()
			: base(SessionNotFoundExceptionMessage)
		{
		}

        public FMSessionInvalidException(String ExceptionMessage)
            : base(ExceptionMessage)
        {
        }
			public FMSessionInvalidException(String ExceptionMessage, Exception inner)
				 : base(ExceptionMessage, inner)
			{
			}
      /// <summary>
      /// Initializes a new instance of the <see cref="FMSessionInvalidException"/> class.
      /// </summary>
      /// <param name="info">The object that holds the serialized object data.</param>
      /// <param name="context">The contextual information about the source or destination.</param>
      public FMSessionInvalidException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		#endregion
	}
}
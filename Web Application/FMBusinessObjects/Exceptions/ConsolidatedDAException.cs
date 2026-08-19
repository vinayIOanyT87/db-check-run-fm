// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsolidatedDAException.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FMBusinessObjects.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Specialization of ApplicationException for error encountered by the ConsolidatedDAL
    /// </summary>
    /// <remarks>
    /// Currently does nothing beyond the base ApplicationException class
    /// </remarks>
    [Serializable]
    public class ConsolidatedDAException : ApplicationException
    {
        /// <summary>
        /// Initializes a new instance of the Exception class.
        /// </summary>
        public ConsolidatedDAException()
            : base("Database error")
        {
        }

        /// <summary>
        /// Initializes a new instance of the Exception class with a specified error message. 
        /// </summary>
        /// <param name="msg">Error message</param>
        public ConsolidatedDAException(string msg)
            : base(msg)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Exception class with a specified error message and 
        /// a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="msg">Error message</param>
        /// <param name="innerException">inner exception that is the cause of this exception</param>
        public ConsolidatedDAException(string msg, Exception innerException)
            : base(msg, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Exception class with serialized data
        /// </summary>
        /// <param name="info">The SerializationInfo that holds the serialized object data about the exception being thrown</param>
        /// <param name="context">The StreamingContext that contains contextual information about the source or destination</param>
        protected ConsolidatedDAException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FMBusinessObjects.Exceptions
{
	[Serializable]
	public class AccountingDAException : ApplicationException
    {
        /// <summary>
        /// Initializes a new instance of the Exception class.
        /// </summary>
        public AccountingDAException()
            : base( "Database error" )
        {
        }

        /// <summary>
        /// Initializes a new instance of the Exception class with a specified error message. 
        /// </summary>
        /// <param name="msg">Error message</param>
        public AccountingDAException( string msg )
            : base( msg )
        {
        }

        /// <summary>
        /// Initializes a new instance of the Exception class with a specified error message and 
        /// a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="msg">Error message</param>
        /// <param name="innerException">inner exception that is the cause of this exception</param>
        public AccountingDAException( string msg, Exception innerException )
            : base( msg, innerException )
        {
        }

        /// <summary>
        /// Initializes a new instance of the Exception class with serialized data
        /// </summary>
        /// <param name="info">The SerializationInfo that holds the serialized object data about the exception being thrown</param>
        /// <param name="context">The StreamingContext that contains contextual information about the source or destination</param>
        protected AccountingDAException( System.Runtime.Serialization.SerializationInfo info,
                                                     System.Runtime.Serialization.StreamingContext context )
            : base( info, context )
        {
        }
    }
}
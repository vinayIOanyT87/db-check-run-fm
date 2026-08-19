// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMDataExchangeBaseException.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the FMDataExchangeBaseException type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.Exceptions
{
	using System;
	using System.Runtime.Serialization;

	/// <summary>
	/// The FM data exchange base exception.
	/// </summary>
	[Serializable]
	public abstract class FMDataExchangeBaseException : ApplicationException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FMDataExchangeBaseException"/> class.
		/// </summary>
		protected FMDataExchangeBaseException ( )
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FMDataExchangeBaseException"/> class.
		/// </summary>
		/// <param name="msg">
		/// The message.
		/// </param>
		protected FMDataExchangeBaseException ( string msg ) : base ( msg )
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FMDataExchangeBaseException"/> class.
		/// </summary>
		/// <param name="msg">
		/// The message.
		/// </param>
		/// <param name="innerException">
		/// The inner exception.
		/// </param>
		protected FMDataExchangeBaseException ( string msg, Exception innerException ) : base ( msg, innerException )
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FMDataExchangeBaseException"/> class.
		/// </summary>
		/// <param name="info">
		/// The info.
		/// </param>
		/// <param name="context">
		/// The context.
		/// </param>
		protected FMDataExchangeBaseException ( SerializationInfo info, StreamingContext context ) : base ( info, context )
		{
		}
	}

	/// <summary>
	/// The FM data exchange interface assembly not found exception.
	/// </summary>
	[Serializable]
	public class FMDataExchangeInterfaceAssemblyNotFoundException : FMDataExchangeBaseException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FMDataExchangeInterfaceAssemblyNotFoundException"/> class.
		/// </summary>
		public FMDataExchangeInterfaceAssemblyNotFoundException ( )
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FMDataExchangeInterfaceAssemblyNotFoundException"/> class.
		/// </summary>
		/// <param name="errorText">
		/// The error text.
		/// </param>
		public FMDataExchangeInterfaceAssemblyNotFoundException ( string errorText ) : base ( errorText )
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FMDataExchangeInterfaceAssemblyNotFoundException"/> class.
		/// </summary>
		/// <param name="errorText">
		/// The error text.
		/// </param>
		/// <param name="innerException">
		/// The inner exception.
		/// </param>
		public FMDataExchangeInterfaceAssemblyNotFoundException ( string errorText, Exception innerException )
			: base ( errorText, innerException )
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FMDataExchangeInterfaceAssemblyNotFoundException"/> class.
		/// </summary>
		/// <param name="info">
		/// The info.
		/// </param>
		/// <param name="context">
		/// The context.
		/// </param>
		public FMDataExchangeInterfaceAssemblyNotFoundException ( SerializationInfo info, StreamingContext context )
			: base ( info, context )
		{
		}
	}

	/// <summary>
	/// The FM data exchange interface folder not found exception.
	/// </summary>
	[Serializable]
	public class FMDataExchangeInterfaceFolderNotFoundException : FMDataExchangeBaseException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FMDataExchangeInterfaceFolderNotFoundException"/> class.
		/// </summary>
		public FMDataExchangeInterfaceFolderNotFoundException ( )
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FMDataExchangeInterfaceFolderNotFoundException"/> class.
		/// </summary>
		/// <param name="errorText">
		/// The error text.
		/// </param>
		public FMDataExchangeInterfaceFolderNotFoundException ( string errorText ) : base ( errorText )
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FMDataExchangeInterfaceFolderNotFoundException"/> class.
		/// </summary>
		/// <param name="errorText">
		/// The error text.
		/// </param>
		/// <param name="innerException">
		/// The inner exception.
		/// </param>
		public FMDataExchangeInterfaceFolderNotFoundException ( string errorText, Exception innerException )
			: base ( errorText, innerException )
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FMDataExchangeInterfaceFolderNotFoundException"/> class.
		/// </summary>
		/// <param name="info">
		/// The info.
		/// </param>
		/// <param name="context">
		/// The context.
		/// </param>
		public FMDataExchangeInterfaceFolderNotFoundException ( SerializationInfo info, StreamingContext context )
			: base ( info, context )
		{
		}
	}

	/// <summary>
	/// The FM data exchange null data exception.
	/// </summary>
	[Serializable]
	public class FMDataExchangeNullDataException : FMDataExchangeBaseException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FMDataExchangeNullDataException"/> class.
		/// </summary>
		public FMDataExchangeNullDataException ( )
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FMDataExchangeNullDataException"/> class.
		/// </summary>
		/// <param name="errorText">
		/// The error text.
		/// </param>
		public FMDataExchangeNullDataException ( string errorText ) : base ( errorText )
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FMDataExchangeNullDataException"/> class.
		/// </summary>
		/// <param name="errorText">
		/// The error text.
		/// </param>
		/// <param name="innerException">
		/// The inner exception.
		/// </param>
		public FMDataExchangeNullDataException ( string errorText, Exception innerException )
			: base ( errorText, innerException )
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FMDataExchangeNullDataException"/> class.
		/// </summary>
		/// <param name="info">
		/// The info.
		/// </param>
		/// <param name="context">
		/// The context.
		/// </param>
		public FMDataExchangeNullDataException (SerializationInfo info, StreamingContext context )
			: base ( info, context )
		{
		}
	}

	/// <summary>
	/// The FM data exchange data type mismatch exception.
	/// </summary>
	[Serializable]
	public class FMDataExchangeDataTypeMismatchException : FMDataExchangeBaseException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FMDataExchangeDataTypeMismatchException"/> class.
		/// </summary>
		public FMDataExchangeDataTypeMismatchException ( )
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FMDataExchangeDataTypeMismatchException"/> class.
		/// </summary>
		/// <param name="errorText">
		/// The error text.
		/// </param>
		public FMDataExchangeDataTypeMismatchException ( string errorText ) : base ( errorText )
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FMDataExchangeDataTypeMismatchException"/> class.
		/// </summary>
		/// <param name="errorText">
		/// The error text.
		/// </param>
		/// <param name="innerException">
		/// The inner exception.
		/// </param>
		public FMDataExchangeDataTypeMismatchException ( string errorText, Exception innerException )
			: base ( errorText, innerException )
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FMDataExchangeDataTypeMismatchException"/> class.
		/// </summary>
		/// <param name="info">
		/// The info.
		/// </param>
		/// <param name="context">
		/// The context.
		/// </param>
		public FMDataExchangeDataTypeMismatchException (SerializationInfo info, StreamingContext context )
			: base ( info, context )
		{
		}
	}

	/// <summary>
	/// The FM data exchange xml exception.
	/// </summary>
	[Serializable]
	public class FMDataExchangeXmlException : FMDataExchangeBaseException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FMDataExchangeXmlException"/> class.
		/// </summary>
		public FMDataExchangeXmlException( )
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FMDataExchangeXmlException"/> class.
		/// </summary>
		/// <param name="errorText">
		/// The error text.
		/// </param>
		public FMDataExchangeXmlException(string errorText) : base(errorText)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FMDataExchangeXmlException"/> class.
		/// </summary>
		/// <param name="errorText">
		/// The error text.
		/// </param>
		/// <param name="innerException">
		/// The inner exception.
		/// </param>
		public FMDataExchangeXmlException(string errorText, Exception innerException)
			: base(errorText, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FMDataExchangeXmlException"/> class.
		/// </summary>
		/// <param name="info">
		/// The info.
		/// </param>
		/// <param name="context">
		/// The context.
		/// </param>
		public FMDataExchangeXmlException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}

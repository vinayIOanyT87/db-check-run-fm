// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMExceptions.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the FMExport exceptions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMExportService
{
	using System;
	using System.Runtime.Serialization;

	[Serializable]
	public abstract class FMExportServiceBaseException : ApplicationException
	{
		public FMExportServiceBaseException()
		{
		}

		public FMExportServiceBaseException(string msg)
			: base(msg)
		{
		}

		public FMExportServiceBaseException(string msg, Exception innerException)
			: base(msg, innerException)
		{
		}

		public FMExportServiceBaseException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}

	[Serializable]
	public class FMExportServiceInterfaceAssemblyNotFoundException : FMExportServiceBaseException
	{
		public FMExportServiceInterfaceAssemblyNotFoundException()
			: base()
		{
		}

		public FMExportServiceInterfaceAssemblyNotFoundException(string errorText)
			: base(errorText)
		{
		}

		public FMExportServiceInterfaceAssemblyNotFoundException(string errorText, Exception innerException)
			: base(errorText, innerException)
		{
		}

		public FMExportServiceInterfaceAssemblyNotFoundException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}

	[Serializable]
	public class FMExportServiceInterfaceFolderNotFoundException : FMExportServiceBaseException
	{
		public FMExportServiceInterfaceFolderNotFoundException()
			: base()
		{
		}

		public FMExportServiceInterfaceFolderNotFoundException(string errorText)
			: base(errorText)
		{
		}

		public FMExportServiceInterfaceFolderNotFoundException(string errorText, Exception innerException)
			: base(errorText, innerException)
		{
		}

		public FMExportServiceInterfaceFolderNotFoundException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}

	[Serializable]
	public class FMExportServiceInterfaceNameNotSpecifiedException : FMExportServiceBaseException
	{
		public FMExportServiceInterfaceNameNotSpecifiedException()
			: base()
		{
		}

		public FMExportServiceInterfaceNameNotSpecifiedException(string errorText)
			: base(errorText)
		{
		}

		public FMExportServiceInterfaceNameNotSpecifiedException(string errorText, Exception innerException)
			: base(errorText, innerException)
		{
		}

		public FMExportServiceInterfaceNameNotSpecifiedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}

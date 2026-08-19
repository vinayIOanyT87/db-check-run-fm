// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMFatalErrorException.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the FMFatalErrorException type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.Exceptions
{
	using System;

	using System.Runtime.Serialization;
	using System.Security.Permissions;

	[Serializable]
	public class FMFatalErrorException : ApplicationException
	{
		public int ErrorCount
		{
			get;
			private set;
		}

		public FMFatalErrorException(int errorCount, string errorDescription)
			: base(errorDescription)
		{
			ErrorCount = errorCount;
		}

		protected FMFatalErrorException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this.ErrorCount = info.GetInt32("ErrorCount");
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}

			info.AddValue("ErrorCount", this.ErrorCount);

			base.GetObjectData(info, context);
		}
	}
}

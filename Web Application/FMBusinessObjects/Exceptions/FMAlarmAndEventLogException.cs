// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMAlarmAndEventLogException.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the FMAlarmAndEventLogException type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.Exceptions
{
	using System;
	using System.Runtime.Serialization;

	[Serializable]
	public class FMAlarmAndEventLogException : FMFatalErrorException
	{
		public FMAlarmAndEventLogException(int errorCount)
			: base(errorCount, "Fatal Error writing to Alarm And Event Log table.")
		{
		}

		protected FMAlarmAndEventLogException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}

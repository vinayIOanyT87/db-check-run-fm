// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FuelsManagerServiceCommunication.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Allows for other processes to communicate with the FuelsManagerService. Implements the methods described by the IFuelsManagerService contract.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManagerService
{
	using System.ServiceModel;
	using FMBusinessObjects.BusinessInterfaces;

	/// <summary>
	/// Allows for other processes to communicate with the FuelsManagerService. Implements the methods described by the IFuelsManagerService contract.
	/// </summary>
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Single)]
	public class FuelsManagerServiceCommunication : IFuelsManagerService
	{
		/// <summary>
		/// Signal the alarm and event processing thread so it knows that there 
		/// are new alarm and event logs to process
		/// </summary>
		[OperationBehavior(TransactionScopeRequired = false)]
		public void SignalAlarmAndEventLogAddedEvent()
		{
			AlarmAndEventProcessing.SetEventOrAlarmEvent();
		}
	}
}

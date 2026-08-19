// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFuelsManagerService.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//    The service contract for methods exposed by the FuelsManager service
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.BusinessInterfaces
{
	using System.ServiceModel;

	/// <summary>
	/// The service contract for methods exposed by the FuelsManager service
	/// </summary>
	[ServiceContract]
	public interface IFuelsManagerService
	{
		/// <summary>
		/// Signal the alarm and event processing thread so it knows that there 
		/// are new alarm and event logs to process
		/// </summary>
		[OperationContract(IsOneWay = true)]
		[TransactionFlow(TransactionFlowOption.NotAllowed)]
		void SignalAlarmAndEventLogAddedEvent();
	}
}

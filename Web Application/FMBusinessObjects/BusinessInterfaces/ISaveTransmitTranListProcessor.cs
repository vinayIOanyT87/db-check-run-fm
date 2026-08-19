// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISaveTransmitTranListProcessor.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ISaveTransmitTranListProcessor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.BusinessInterfaces
{
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;

	/// <summary>
	/// Defines the ISaveTransmitTranListProcessor type.
	/// </summary>
	[ServiceContract]
	public interface ISaveTransmitTranListProcessor
	{
		/// <summary>
		/// Processes the specified sr.
		/// </summary>
		/// <param name="sr">The sr.</param>
		/// <returns>An object containing the results of the import.</returns>
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		SaveTransmitTranListResultDO Process(SaveTransmitTranListSR sr);
	}
}

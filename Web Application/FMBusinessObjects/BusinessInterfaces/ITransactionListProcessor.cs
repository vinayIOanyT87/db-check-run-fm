// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITransactionListProcessor.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ITransactionListProcessor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.BusinessInterfaces
{
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;

	[ServiceContract]
	public interface ITransactionListProcessor
	{
		#region Public Methods and Operators

		[OperationContract]
		TransactionListDO Process(TransactionListSR sr);

		#endregion
	}
}
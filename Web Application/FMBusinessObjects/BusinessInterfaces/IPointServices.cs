// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPointServices.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.Collections.Generic;
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;

	[ServiceContract]
	public interface IPointServices
	{
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Guid Add(SecurityClass security, PointService pointService);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, PointService pointService);

		[OperationContract]
		void Purge(SecurityClass security, Guid pointServiceGuid);

		[OperationContract]
		PointService Get(SecurityClass security, string hostName);

		[OperationContract]
		List<PointService> Enumerate(SecurityClass security);
	}
}
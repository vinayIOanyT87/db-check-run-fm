// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IReserveLevels.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Interface definition for reserve levels service class
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;

	[ServiceContract]
	public interface IReserveLevels
	{
		#region Public Methods and Operators

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Guid Add(SecurityClass security, ReserveLevelClass reserveLevel);

		[OperationContract]
		ReserveLevelCollectionClass Enumerate(SecurityClass security);

		[OperationContract]
		ReserveLevelClass GetByIdentityGuid(SecurityClass security, Guid reserveLevelGuid);

		[OperationContract]
		ReserveLevelClass GetByProductID(SecurityClass security, string productID);

		[OperationContract]
		Guid GetIdentityGuid(SecurityClass security, string productID);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, ReserveLevelClass reserveLevel);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, Guid reserveLevelGuid);

		#endregion
	}
}
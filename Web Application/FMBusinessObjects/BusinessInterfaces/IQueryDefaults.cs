// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IQueryDefaults.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the IQueryDefaults type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;

	[ServiceContract]
	public interface IQueryDefaults
	{
		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Add ( SecurityClass security, QueryDefaultClass queryDefault );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Modify ( SecurityClass security, QueryDefaultClass queryDefault );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Update ( SecurityClass security, QueryDefaultClass queryDefault );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Purge ( SecurityClass security, Guid identityGuid );

		[OperationContract]
		QueryDefaultClass Get(SecurityClass security, Guid identityGuid);

		[OperationContract]
		QueryDefaultClass Enumerate ( SecurityClass security );

		[OperationContract]
		QueryDefaultClass EnumerateBySite ( SecurityClass security );
	}
}

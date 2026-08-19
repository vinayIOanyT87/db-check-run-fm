// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFMAETranslations.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
// Describes methods for retrieving, creating, updating, and deleting
// records that define translations between values in the legacy aviation application's transaction records
// and in FuelsManager when the transactions are imported through the FMAE interface
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.Collections.Generic;
	using System.ServiceModel;
	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Describes methods for retrieving, creating, updating, and deleting
	/// records that define translations between values in the legacy aviation application's transaction records
	/// and in FuelsManager when the transactions are imported through the FMAE interface
	/// </summary>
	[ServiceContract]
	public interface IFMAETranslations
	{
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.NotAllowed)]
		List<FMAETranslation> Enumerate(SecurityClass security, FMAETranslationType translationType);

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.NotAllowed)]
        List<FMAETranslation> EnumerateAndFilter(SecurityClass security, FMAETranslationType translationType, string searchFilter);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Guid Add(SecurityClass security, FMAETranslation fmaeTranslation);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, FMAETranslation fmaeTranslation);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, FMAETranslation fmaeTranslation);

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        List<string> Import(SecurityClass security, List<FMAETranslation> fmaeTranslations);
	}
}

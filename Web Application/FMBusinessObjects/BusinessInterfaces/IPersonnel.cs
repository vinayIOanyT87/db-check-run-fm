// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPersonnel.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the IPersonnel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.Data;
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// The Personnel service class interface definition.
	/// </summary>
	[ServiceContract]
	public interface IPersonnel
	{
		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		Guid Add ( SecurityClass security, PersonClass person );

		[OperationContract]
		PersonClass Get ( SecurityClass security, Guid targetGuid );

		[OperationContract]
		PersonCollectionClass Enumerate ( SecurityClass security, bool hideHiddenPersonnel = false );

		[OperationContract]
		PersonCollectionClass EnumerateByRole ( SecurityClass security, PERSON_ROLE role, bool hideHiddenPersonnel = false );

        [OperationContract]
        PersonCollectionClass EnumerateByRoleAndCompanyGuid(SecurityClass security, PERSON_ROLE role, Guid companyGuid, bool hideHiddenPersonnel = false);

        [OperationContract]
		DataSet EnumerateByRole1 ( SecurityClass security, PERSON_ROLE role );

		[OperationContract]
		PersonCollectionClass EnumerateByRoleSortByName ( SecurityClass security, PERSON_ROLE role );

		[OperationContract]
		PersonCollectionClass EnumerateUndelegated(SecurityClass security);

        /// <summary>
        /// Enumerate all personnel for the site, retrieving only basic information like the ID, SiteGuid, MasterRecordGuid, and IdentityGuid
        /// </summary>
        /// <param name="security">Contains Security information</param>
        /// <returns>All personnel for the site, with basic information like the ID, SiteGuid, MasterRecordGuid, and IdentityGuid populated</returns>
        [OperationContract]
        PersonCollectionClass EnumerateBasicInformationOnly(SecurityClass security);

		[OperationContract]
		PersonClass GetByID(SecurityClass security, string id);

		[OperationContract]
		Guid GetGuidByCardNumber( SecurityClass security, string cardNumber );

		[OperationContract]
		Guid GetGuidByID ( SecurityClass security, string ID );

		[OperationContract]
		Guid GetMasterRecordGuid(SecurityClass security, string id);

		[OperationContract]
		Guid GetGuidByShortCardNumber( SecurityClass security, string shortCardNumber );

		[OperationContract]
		string GetLatestRowVersionByRole( SecurityClass security, PERSON_ROLE role );

		[OperationContract]
		string GetNextShortCardNumber ( SecurityClass security );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Import ( SecurityClass security, PersonClass person );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Modify ( SecurityClass security, DATA_TYPE Type, PersonClass person );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Purge ( SecurityClass security, Guid targetGuid);

		[OperationContract]
		PersonCollectionClass EnumerateByRoleAndFilter (SecurityClass security, PERSON_ROLE role, string filter, string order, bool hideHiddenPersonnel = false);

		[OperationContract]
		DataSet EnumerateUpdateVersions( SecurityClass security );

		[OperationContract]
		PersonClass PrepareForExport ( SecurityClass security, PersonClass person );

		[OperationContract]
		PersonClass GetBasicInfo(SecurityClass security, Guid personnelGuid, Guid siteGuid);

		[OperationContract]
		PersonCollectionClass EnumerateByCompany(SecurityClass security, Guid companyGuid);

	    [OperationContract]
	    DataSet EnumerateCardedInPersonnelPartTimeoutPeriod(SecurityClass security, DateTimeOffset timeOutPeriod);
	}
}

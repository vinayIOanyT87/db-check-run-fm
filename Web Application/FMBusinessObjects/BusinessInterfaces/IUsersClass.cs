// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IUsersClass.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.Data;
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;

	[ServiceContract]
	public interface IUsers
	{
		#region Public Methods and Operators

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Guid Add(SecurityClass security, UserClass user);

		[OperationContract]
		UserCollectionClass Enumerate(SecurityClass security);

        [OperationContract]
        DataSet EnumerateActiveDirectoryUsers(SecurityClass security);

        [OperationContract]
		UserCollectionClass EnumerateAndFilter(SecurityClass security, string filter);

        [OperationContract]
        UserCollectionClass GetUsersByIDWithoutSite(SecurityClass security, string userID);

        [OperationContract]
		UserCollectionClass EnumerateByGroup(SecurityClass security, Guid groupGuid);

		[OperationContract]
		UserCollectionClass EnumerateByGroupAndSite(SecurityClass security, Guid groupGuid, Guid siteGuid);

		[OperationContract]
		UserCollectionClass EnumerateForParentSiteByAssignedUser(SecurityClass security, Guid siteGuid);

		[OperationContract]
		UserCollectionClass EnumerateForSiteByAssignedUser(SecurityClass security, Guid siteGuid);

		[OperationContract]
		UserClass Get(SecurityClass security, Guid userGuid);

		[OperationContract]
		UserClass GetBySite(SecurityClass security, Guid userGuid, Guid siteGuid);

		[OperationContract]
		UserClass GetByID(SecurityClass security, string userID);

        [OperationContract]
		UserClass GetByIDForLogOn(SecurityClass security, string userID);

		[OperationContract]
		UserClass GetDuringLogOn(SecurityClass security, Guid guid);

		[OperationContract]
		Guid GetIdentityGuid(SecurityClass security, string userID);

		[OperationContract]
		Guid GetIdentityGuidBySevice(SecurityClass security, string userID, bool service);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, UserClass user);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void ModifyPasswordCount(SecurityClass security, UserClass user);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void ModifyWithPasswordHistory(SecurityClass security, UserClass user, string oldPassword);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, Guid userGuid);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void DisableUser(SecurityClass security);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void ArchiveUser(SecurityClass security);

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void DeleteOrphanUserRecords(SecurityClass security, bool activeDirectoryUsers = true);
        #endregion
    }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IUserGroupMaps.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Interface definition for IUserGroupMaps
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.Data;
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;

	[ServiceContract]
	public interface IUserGroupMaps
	{
		#region Public Methods and Operators

		[OperationContract]
		DataSet EnumerateByUserPermissionGrid(
			SecurityClass security, Guid modifyUser, Guid siteGuid, bool loadChildrenSites, string filter);

		[OperationContract]
		UserGroupMapCollectionClass EnumerateBySite(SecurityClass security, Guid siteGuid);

		[OperationContract]
		UserGroupMapCollectionClass EnumerateByGroupAndSite(SecurityClass security, Guid groupGuid, Guid siteGuid);

		[OperationContract]
		UserGroupMapCollectionClass EnumerateByUserAndSite(SecurityClass security, Guid userGuid, Guid siteGuid);

		[OperationContract]
		UserGroupMapCollectionClass EnumerateByGroup(SecurityClass security, Guid groupGuid);

		[OperationContract]
		UserGroupMapCollectionClass EnumerateByUser(SecurityClass security, Guid userGuid);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Add(SecurityClass security, UserGroupMapClass userGroupMap);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, Guid userGuid, Guid groupGuid, Guid siteGuid);

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void UpdateDenyFlag(SecurityClass security, UserGroupMapClass userGroupMap);
        #endregion
    }
}
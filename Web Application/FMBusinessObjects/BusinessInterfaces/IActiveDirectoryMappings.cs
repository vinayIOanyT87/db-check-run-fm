namespace FMBusinessObjects.BusinessInterfaces
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.ServiceModel;

    using FMBusinessObjects.DataObjects;

    [ServiceContract]
    public interface IActiveDirectoryMappings
    {
        [OperationContract]
        List<ActiveDirectorySiteGroup> EnumerateActiveDirectorySiteList(SecurityClass security, Guid sitesActiveDirectoryGuid);

        [OperationContract]
        List<ActiveDirectoryUserGroup> EnumerateActiveDirectoryUserList(SecurityClass security, Guid usersActiveDirectoryGuid);

        [OperationContract]
        List<ActiveDirectorySiteGroup> EnumerateAllActiveDirectorySites(SecurityClass security);

        [OperationContract]
        List<ActiveDirectoryUserGroup> EnumerateAllActiveDirectoryUser(SecurityClass security);

        [OperationContract]
        DataSet EnumerateSiteToActiveDirectorySiteMapping(SecurityClass security);

        [OperationContract]
        DataSet EnumerateUserGroupToActiveDirectoryUserGroupMapping(SecurityClass security);

        [OperationContract]
        DataSet EnumerateAllSiteIdAndGuid(SecurityClass security);

        [OperationContract]
        DataSet GetUserMappingChangePlan(SecurityClass security, DataTable userInfoTable, bool deleteMappingsNonExistingUsers);

        [OperationContract]
        DataSet GetUserGroupMappingChangePlan(SecurityClass security, DataTable userGroupInfoTable, bool deleteMappingsNonExistingUsers);

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void DeleteUserFromSite(SecurityClass security, Guid userGuid, Guid? assignedToSiteGuid, bool deleteBaseMapping);

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void UpdateUsersOwner(SecurityClass security, Guid userGuid, Guid siteGuid);

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void DeleteUserFromGroups(SecurityClass security, Guid userGuid, Guid? siteGuid, Guid? userGroupGuid);
    }
}

using FMBusinessObjects.DataObjects;
using System.Collections.Generic;
using System.ServiceModel;
using System.DirectoryServices.AccountManagement;

namespace FMBusinessObjects.BusinessInterfaces
{
    [ServiceContract]
    public interface IActiveDirectoryService
    {
        [OperationContract]
        bool ConfirmUser(string userName);

        [OperationContract]
        bool AuthenticateUser(string userName, string password);

        [OperationContract]
        void RefreshSites(SecurityClass security);

        [OperationContract]
        void RefreshUserGroups(SecurityClass security);

        [OperationContract]
        void GetGroup(string domainUserName);

        [OperationContract]
        List<ActiveDirectoryUserDTO> GetUsersAndGroupAssociations(SecurityClass security);

        [OperationContract]
        void GetOrganizationalUnits();

        [OperationContract]
        List<ActiveDirectorySiteGroup> GetActiveDirectorySitesFromFuelsManager(SecurityClass security);

    }
}

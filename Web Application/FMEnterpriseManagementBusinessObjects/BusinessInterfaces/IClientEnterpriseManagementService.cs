namespace FMEnterpriseManagementBusinessObjects.BusinessInterfaces
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.ServiceModel;

    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.ServiceRequests;

    [ServiceContract]
    public interface IClientEnterpriseManagementService
    {
        [OperationContract]
        SecurityLoginResponse Login(SecurityLoginRequest sr);

        [OperationContract]
        void PingSession(SecurityClass security);

        [OperationContract]
        void Logout(SecurityClass security);

        [OperationContract]
        DataSet EnumerateEquipmentDataSet(SecurityClass security,
                                                bool managedEquipmentOnly,
                                                bool secondaryStorageOnly,
                                                Guid equipmentTypeGuid,
                                                EQUIPMENT_TYPE equipmentType,
                                                string translatedUnassigned,
                                                string filter,
                                                bool isDefense,
                                                bool hideHiddenEquipmentRecords = false,
                                                int limit = 1500);

        [OperationContract]
        EquipmentClass GetEquipment(SecurityClass enterpriseSecurity, Guid equipmentGuid);

        [OperationContract]
        Guid GetEquipmentMasterGuid(SecurityClass enterpriseSecurity, string equipmentId);

        [OperationContract]
        EquipmentCollectionClass EnumerateEquipment(SecurityClass security);

        [OperationContract]
        void RequestEnterpriseEquipmentAssignment(SecurityClass enterpriseSecurity, Guid assignedToSiteGuid, Guid equipmentMasterGuid);

        [OperationContract]
        PersonCollectionClass EnumeratePersonnel(SecurityClass security);

        [OperationContract]
        PersonCollectionClass EnumeratePersonnelByRoleAndFilter(SecurityClass security, PERSON_ROLE role, string filterString, string orderBy, bool hideHiddenPersonnel);

        [OperationContract]
        void RequestEnterprisePersonAssignment(SecurityClass enterpriseSecurity, Guid assignedToSiteGuid, Guid personMasterGuid);

        [OperationContract]
        PersonClass GetPerson(SecurityClass enterpriseSecurity, Guid personMasterGuid);

        [OperationContract]
        Guid GetPersonMasterGuid(SecurityClass enterpriseSecurity, string personId);
    }
}

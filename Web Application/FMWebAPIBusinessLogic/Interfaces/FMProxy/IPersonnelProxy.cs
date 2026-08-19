using System;
using System.Data;
using FMBusinessObjects.DataObjects;

namespace FMWebAPIBusinessLogic.Interfaces.FMProxy
{
    public interface IPersonnelProxy
    {
        Guid Add(PersonClass person);
        PersonCollectionClass Enumerate(bool hideHiddenPersonnel = false);
        PersonCollectionClass EnumerateBasicInformationOnly();
        PersonCollectionClass EnumerateByCompany(Guid companyGuid);
        PersonCollectionClass EnumerateByRole(PERSON_ROLE role, bool hideHiddenPersonnel = false);
        DataSet EnumerateByRole1(PERSON_ROLE role);
        PersonCollectionClass EnumerateByRoleAndFilter(PERSON_ROLE role, string filter, string order, bool hideHiddenPersonnel = false);
        PersonCollectionClass EnumerateByRoleSortByName(PERSON_ROLE role);
        PersonCollectionClass EnumerateUndelegated();
        DataSet EnumerateUpdateVersions();
        PersonClass Get(Guid targetGuid);
        PersonClass GetBasicInfo(Guid personnelGuid, Guid siteGuid);
        PersonClass GetByID(string id);
        Guid GetGuidByCardNumber(string cardNumber);
        Guid GetGuidByID(string ID);
        Guid GetGuidByShortCardNumber(string shortCardNumber);
        string GetLatestRowVersionByRole(PERSON_ROLE role);
        Guid GetMasterRecordGuid(string id);
        string GetNextShortCardNumber();
        void Import(PersonClass person);
        void Modify(DATA_TYPE Type, PersonClass person);
        PersonClass PrepareForExport(PersonClass person);
        void Purge(Guid targetGuid);
    }
}
using System;
using System.Data;
using FMBusinessObjects.DataObjects;

namespace FMWebAPIBusinessLogic.Interfaces.FMProxy
{
    public interface IEquipmentsProxy
    {
        Guid Add(EquipmentClass equipment);
        EquipmentCollectionClass Enumerate(bool hideHiddenEquipmentRecords = false);
        EquipmentCollectionClass EnumerateByCompany(Guid companyGuid, SiteClass site, bool hideHiddenEquipmentRecords = false);
        EquipmentCollectionClass EnumerateByCompanyAndLocalize(Guid companyGuid, bool localize, bool hideHiddenEquipmentRecords = false);
        EquipmentCollectionClass EnumerateByCompanyAndSite(Guid companyGuid, SiteClass site, bool hideHiddenEquipmentRecords = false);
        EquipmentCollectionClass EnumerateByFuelCard(Guid fuelCardGuid, bool hideHiddenEquipmentRecords = false);
        EquipmentCollectionClass EnumerateByManagedFillstand();
        EquipmentCollectionClass EnumerateBySource();
        EquipmentCollectionClass EnumerateByTypeAndFilterAndProduct(EQUIPMENT_TYPE type, string filter, Guid productGuid, bool excludeNonEditableCompanyGuid, bool excludeNonEditableFuelCardGuid, bool hideHiddenEquipmentRecords = false);
        EquipmentCollectionClass EnumerateByTypeAndProduct(EQUIPMENT_TYPE type, Guid productGuid, bool excludeNonEditableCompanyGuid, bool excludeNonEditableFuelCardGuid, bool hideHiddenEquipmentRecords = false);
        DataSet EnumerateByTypesCompanyFuelCardProductAndSecondaryStorage1(EQUIPMENT_TYPE[] types, object companyGuid, object fuelCardGuid, object productGuid, object secondaryStorage);
        DataSet EnumerateDataSet(bool managedEquipmentOnly, bool secondaryStorageOnly, Guid equipmentTypeGuid, EQUIPMENT_TYPE equipmentType, string translatedUnassigned, string filter, bool isDefense, bool hideHiddenEquipmentRecords = false);
        EquipmentCollectionClass EnumerateExt(Guid equipmentTypeGuid, bool managedEquipmentOnly = false, bool secondaryStorageOnly = false, EQUIPMENT_TYPE equipmentType = EQUIPMENT_TYPE.MAX_EQUIPMENT_TYPE, string filter = null, int limit = 0, bool hideHiddenEquipmentRecords = false);
        EquipmentInfo[] EnumerateInfo();
        EquipmentInfo[] EnumerateInfoByTypesCompanyFuelCardProductAndSecondaryStorage(EQUIPMENT_TYPE[] types, object companyGuid, object fuelCardGuid, object productIndexGuid, object secondaryStorage, bool hideHiddenEquipmentRecords = false);
        EquipmentInfo[] EnumerateInfoUndelegated();
        EquipmentCollectionClass EnumerateManagedEquipment();
        EquipmentCollectionClass EnumerateManagedEquipmentWithoutQualityTag();
        DataSet EnumerateUpdateVersions();
        DataSet EnumerateUpdateVersionsForOpc();
        EquipmentClass Get(Guid equipmentGuid);
        EquipmentClass GetBasicInfo(Guid equipmentGuid, Guid siteGuid);
        EquipmentClass GetBySite(Guid equipmentGuid, SiteClass site);
        Guid GetIdentityGuid(string id);
        Guid GetIdentityGuidByCardNumberAndEquipmentID(Guid companyGuid, string truckCardNumber);
        Guid GetIdentityGuidByCompanyGuidAndEquipmentID(Guid companyGuid, string companyEquipmentID);
        string GetLatestRowVersionBySource();
        Guid GetMasterRecordGuid(string id);
        void Import(EquipmentClass equipment);
        void Modify(EquipmentClass equipment);
        void Purge(Guid equipmentGuid);
        EquipmentClass GetByMeterGuid(Guid meterGuid);
    }
}
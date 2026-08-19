using FMBusinessObjects.DataObjects;
using System;
using System.Data;
using Varec.CommonComponents.EngineeringUnitsLibrary;


namespace FMWebAPIBusinessLogic.Interfaces.FMProxy
{
    public interface IEquipmentTypesProxy
    {
        Guid Add(EquipmentTypeClass equipmentType);
        EquipmentTypeCollectionClass Enumerate(string filter, string order);
        AirplaneTankCollectionClass EnumerateByEquipmentType(Guid equipmentTypeGuid,
            EngineeringUnit capacityUnits, int capacityDecimalPlaces);
        AirplaneTankCollectionClass EnumerateByEquipmentType(Guid equipmentTypeGuid);
        DataSet EnumerateDataSet(string filter, string order);
        EquipmentTypeClass Get(Guid equipmentTypeGuid);
        EquipmentTypeClass Get(Guid equipmentTypeGuid, SiteClass site);
        Guid GetIdentityGuid(string id);
        void Import(EquipmentTypeClass equipmentType);
        void Modify(EquipmentTypeClass equipmentType);
        void Purge(Guid equipmentTypeGuid);

    }
}

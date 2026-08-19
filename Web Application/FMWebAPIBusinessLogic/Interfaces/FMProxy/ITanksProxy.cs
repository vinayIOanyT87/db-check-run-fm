using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.DataObjects;
using System;

namespace FMWebAPIBusinessLogic.Interfaces.FMProxy
{
    public interface ITanksProxy
    {
        Guid Add(TankClass tank);
        TankCollectionClass Enumerate(bool hideHiddenTanks = false);
        TankCollectionClass EnumerateAuthorized(bool hideHiddenTanks = false);
        TankCollectionClass EnumerateWhereCoordinatesExist();
        TankCollectionClass EnumerateTanksWithoutQualityTag();
        TankCollectionClass EnumerateByFilter(string filter, bool hideHiddenTanks = false);
        TankCollectionClass EnumerateByProduct(Guid productGuid, bool hideHiddenTanks = false);
        TankCollectionClass EnumerateByProductAndFilter(Guid productGuid, string filter, bool hideHiddenTanks = false);
        TankCollectionClass EnumerateByManager(Guid managerGuid);
        TankCollectionClass EnumerateBasicInformation();
        TankCollectionClass EnumerateBasicInfoLinkedToAssetTrackingDevices(string assetTrackingDeviceId);
        Guid GetIdentityGuid(string id);
        TankClass Get(Guid tankGuid);
        int TankConfigurationNumberBeingUsed(Guid tankGuid, Guid assetTrackingDeviceGuid, int tankConfigurationNumber);
        void Modify(TankClass tank);
        void Purge(Guid tankGuid);
    }
}

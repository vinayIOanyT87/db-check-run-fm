using System;
using System.Data;
using FMBusinessObjects.DataObjects;
using System.Collections.Generic;

namespace FMWebAPIBusinessLogic.Interfaces.FMProxy
{
    public interface IMetersProxy
    {
        Guid Add(MeterClass meter);
        IEnumerable<MeterClass> Enumerate();
        IEnumerable<MeterClass> EnumerateAndFilter(string meterIDFilterValue);
        IEnumerable<MeterClass> EnumerateByAssetGuid(Guid assetGuid);
        IEnumerable<MeterClass> EnumerateByAssetGuidAndFilter(Guid assetGuid, string meterIDFilterValue);
        IEnumerable<MeterAssetClass> EnumerateAssets();
        IEnumerable<MeterAssetClass> EnumerateAssetsAndFilter(string assetIDFilterValue);
        MeterClass Get(Guid identityGuid);
        Guid GetIdentityGuid(string id);
        List<string> GetMeterIdsByAssetGuids(List<EquipmentClass> assets);
    }
}
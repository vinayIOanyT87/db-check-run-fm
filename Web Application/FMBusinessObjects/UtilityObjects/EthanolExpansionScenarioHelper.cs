namespace FMBusinessObjects.UtilityObjects
{
    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class EthanolExpansionScenarioHelper {
        public EthanolExpansionScenarioHelper() { }
        public bool isEEScenario2(LineItemDO lineItem, SecurityClass security)
        {
            if ((lineItem.LoadingLocationStationGuid.IsEmpty()) || (lineItem.ArmNumber == null))
            {
                return false;
            }
            if ((lineItem.SubLineItems == null) || (lineItem.SubLineItems.Count == 0))
            {
                return false;
            }

            SubLineItemDO ethanolSubLineItem = null;
            foreach (SubLineItemDO subLineItem in lineItem.SubLineItems)
            {
                if (subLineItem.IsEthanol)
                {
                    ethanolSubLineItem = subLineItem;
                    break;
                }
            }

            if (ethanolSubLineItem == null)
            {
                return false;
            }

            StationClass station = FMChannelHelper.MakeCall<IStations, StationClass>(x => x.Get(security, lineItem.LoadingLocationStationGuid));
            if (station == null || station.EthanolExcess == false || station.LoadArmCollection == null)
            {
                return false;
            }

            foreach (LoadArmClass loadArm in station.LoadArmCollection)
            {
                if (((loadArm.BayAStationGuid == station.IdentityGuid) && (loadArm.BayAArmNumber == lineItem.ArmNumber))
                    || ((loadArm.BayBStationGuid == station.IdentityGuid) && (loadArm.BayBArmNumber == lineItem.ArmNumber)))
                {
                    if (loadArm.ExternalComponentCollection == null)
                    {
                        return false;
                    }

                    foreach (ProductMapClass externalComponent in loadArm.ExternalComponentCollection)
                    {
                        if (externalComponent.AssignedGuid == ethanolSubLineItem.ProductGuid)
                        {
                            return true;
                        }
                    }

                    return false;
                }
            }

            return false;
        }

        public Dictionary<Guid, double> getBobBlendPercentages(LineItemDO lineItem, SecurityClass security)
        {
            Dictionary<Guid, double> bobBlendPercentagesByProductGuid = new Dictionary<Guid, double>();
            ProductClass blendProduct = FMChannelHelper.MakeCall<IProducts, ProductClass>(
                x => x.Get(security, lineItem.ProductGuid, false));

            if (blendProduct != null
                && blendProduct.ProductType == ProductType.BlendProduct
                && blendProduct.ComponentCollection != null
                && blendProduct.ComponentCollection.Count > 0)
            {
                var subLineItemsBob = lineItem.SubLineItems
                    .Where(x => !x.IsEthanol
                        && x.ProductType == ProductClass.ProductTypeID(ProductType.ComponentProduct))
                    .ToList();

                foreach (SubLineItemDO subLineItemBob in subLineItemsBob)
                {
                    ProductMapClass blendComponent = blendProduct.ComponentCollection
                        .FirstOrDefault(x => x.AssignedGuid == subLineItemBob.ProductGuid);

                    if (blendComponent == null)
                    {
                        Guid bobMasterRecordGuid = FMChannelHelper.MakeCall<IProducts, Guid>(
                            x => x.GetMasterRecordGuid(security, subLineItemBob.ProductGuid));

                        blendComponent = blendProduct.ComponentCollection
                            .FirstOrDefault(x => x.AssignedGuid == bobMasterRecordGuid);
                    }

                    if (blendComponent != null)
                    {
                        bobBlendPercentagesByProductGuid[subLineItemBob.ProductGuid] = blendComponent.BlendPercentage;
                    }
                }
            }

            return bobBlendPercentagesByProductGuid;
        }
    }
}
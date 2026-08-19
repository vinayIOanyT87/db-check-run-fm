namespace FMEnterpriseManagementBusinessObjects.ChannelFactories
{
    using FMBusinessObjects.ChannelFactories;

    public class EnterpriseManagementChannelHelper : BaseChannelHelper<EnterpriseManagementChannelHelper>
    {
        public override FMChannelFactoryConfigInfo CreateChannelFactoryConfigInfo<TServiceContractType>()
        {
            var tmpFactoryConfig = new FMChannelFactoryConfigInfo(GetServiceName<TServiceContractType>())
                                   {
                                       EndPointAddressSetByConfigFile = true,
                                       EndpointAddressConfigKey = "EnterpriseManagementEndPointAddress",
                                       EndPointConfigurationSetByConfigFile = true,
                                       EndPointConfigurationConfigKey = "EnterpriseManagementBindingName",
                                       EndPointBehaviorNameConfigKey = "EnterpriseManagementBehaviorName"
            };

            return tmpFactoryConfig;
        }
    }
}

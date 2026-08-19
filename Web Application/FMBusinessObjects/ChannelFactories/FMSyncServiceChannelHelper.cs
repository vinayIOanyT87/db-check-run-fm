using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMBusinessObjects.ChannelFactories
{
	public class FMSyncServiceChannelHelper : BaseChannelHelper<FMSyncServiceChannelHelper>
	{
		public const string BindingTypeConfigKey = "syncServiceBindingType";

		public const string BindingConfigurationConfigKey = "syncServiceBindingConfiguration";

		public const string BindingEndpointAddressConfigKey = "syncServiceBindingEndPointAddress";

		public override FMChannelFactoryConfigInfo CreateChannelFactoryConfigInfo<TServiceContractType>()
		{
			var tmpFactoryConfig = new FMChannelFactoryConfigInfo(GetServiceName<TServiceContractType>());
			tmpFactoryConfig.EndPointAddressSetByConfigFile = true;
			tmpFactoryConfig.EndpointAddressConfigKey = BindingEndpointAddressConfigKey;
			tmpFactoryConfig.EndPointBindingTypeSetByConfigFile = true;
			tmpFactoryConfig.EndPointBindingTypeConfigKey = BindingTypeConfigKey;
			tmpFactoryConfig.EndPointConfigurationSetByConfigFile = true;
			tmpFactoryConfig.EndPointConfigurationConfigKey = BindingConfigurationConfigKey;

			return tmpFactoryConfig;
		}
	}
}

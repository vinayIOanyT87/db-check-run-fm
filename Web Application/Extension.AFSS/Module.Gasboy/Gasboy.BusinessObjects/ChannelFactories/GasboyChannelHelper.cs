// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GasboyChannelHelper.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//  The Gasboy channel helper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.Afss.Module.Gasboy.BusinessObjects.ChannelFactories
{
    using System;
    using System.Configuration;

    using FMBusinessObjects.ChannelFactories;

	/// <summary>
	/// The Gasboy channel helper.
	/// </summary>
	public class GasboyChannelHelper : BaseChannelHelper<GasboyChannelHelper>
	{


		public override FMChannelFactoryConfigInfo CreateChannelFactoryConfigInfo<TServiceContractType>()
		{
			var tmpFactoryConfig = new FMChannelFactoryConfigInfo(GetServiceName<TServiceContractType>());
			tmpFactoryConfig.EndPointAddressSetByConfigFile = true;
			tmpFactoryConfig.EndpointAddressConfigKey = "GasboyEndPointAddress";
			tmpFactoryConfig.EndPointConfigurationSetByConfigFile = true;
			tmpFactoryConfig.EndPointConfigurationConfigKey = "GasboyBindingName";

			return tmpFactoryConfig;
		}
	}
}

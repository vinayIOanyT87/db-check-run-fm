// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FuelsManagerServiceHelper.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
// Contains methods to help communicate with the FuelsManager Service 
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.InternalClasses
{
	using System;
	using System.Linq;

	using FMBusinessObjects.UtilityObjects;

	/// <summary>
	/// Contains methods to help communicate with the FuelsManager Service 
	/// </summary>
	public class FuelsManagerServiceHelper
	{
		/// <summary>
		/// The name of the FuelsManager Service Worker Role. We use this to get the endpoints exposed by the service
		/// </summary>
		//private const string FuelsManagerServiceWorkerRoleName = "FuelsManagerServiceWorkerRole";

		/// <summary>
		/// The name of the FuelsManager Service's endpoint in the Service definition file. We use this to get
		/// the address of the FuelsManager service.
		/// </summary>
		public const string FuelsManagerServiceEndpointName = "FuelsManagerServiceEndpoint";

		/// <summary>
		/// The name of the application setting which contains the FuelsManager service address. This is used when running on premises (and not in the cloud)
		/// </summary>
		private const string FuelsManagerServiceAppSettingKeyName = "FuelsManagerServiceAddress";

		/// <summary>
		/// Identifies the name of the WCF binding configuration to use when communicating with the FuelsManagerService
		/// </summary>
		public const string FuelsManagerServiceBindingConfigurationName = "FuelsManagerServiceBinding";

		/// <summary>
		/// Return the address the FuelsManager service is running at, which depends on whether we are running in Azure
		/// </summary>
		/// <returns>The address the FuelsManager service is running at</returns>
		public static string FuelsManagerServiceAddress
		{
			get
			{
				string fuelsManagerServiceAddress = string.Empty;

				// Get the FuelsManager service address from RoleEnvironment if we're running in Azure,
				// or get them from the application settings in web.config if we're running locally
                //if (RoleEnvironment.IsAvailable)
                //{
                //    // In azure, we have to get the endpoint address from the role instance.
                //    var endpoints = from role in RoleEnvironment.Roles[FuelsManagerServiceWorkerRoleName].Instances
                //                    select role.InstanceEndpoints[FuelsManagerServiceEndpointName];

                //    // If there is more than one instance of the role pick a random one.
                //    RoleInstanceEndpoint randomInstanceEndpoint =
                //        endpoints.Skip(new Random(Environment.TickCount).Next(endpoints.Count())).First();

                //    fuelsManagerServiceAddress = randomInstanceEndpoint.IPEndpoint.ToString();

                //    fuelsManagerServiceAddress = "net.tcp://" + fuelsManagerServiceAddress + "/";

                //    if (!string.IsNullOrEmpty(fuelsManagerServiceAddress))
                //    {
                //        return fuelsManagerServiceAddress + "FuelsManagerService";
                //    }
                //}
                //else
                //{
					fuelsManagerServiceAddress = AppSettingsHelper.GetKeyValue(
						FuelsManagerServiceAppSettingKeyName, string.Empty);

					if (!string.IsNullOrEmpty(fuelsManagerServiceAddress))
					{
						return fuelsManagerServiceAddress;
					}
                //}

				return string.Empty;
			}
		}
	}
}
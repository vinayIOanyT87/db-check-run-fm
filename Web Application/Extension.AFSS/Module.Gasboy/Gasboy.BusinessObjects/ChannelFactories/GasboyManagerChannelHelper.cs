// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GasboyManagerChannelHelper.cs" company="Varec, Inc.">
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
    using System.Diagnostics.Contracts;
    using System.Security.Cryptography.X509Certificates;
    using System.ServiceModel.Description;

    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.UtilityObjects;

    /// <summary>
    /// The GasboyManager channel helper.
    /// </summary>
    public class GasboyManagerChannelHelper : BaseChannelHelper<GasboyManagerChannelHelper>
    {
        #region Public Static Methods

	



        /// <summary>
        /// Creates a channel factory to communicate with remote Gasboy Stations.
        /// </summary>
        /// <typeparam name="TServiceContractType">The type of the t service contract type.</typeparam>
        /// <param name="clientCredentials">The client credentials information configuration.</param>
        /// <param name="bindingTypeName">Name of the binding type.</param>
        /// <param name="bindingConfigName">Name of the binding configuration.</param>
        /// <param name="pURI">The p URI.</param>
        /// <param name="serviceMaximumRetryAttempts">Maximum number of retry attempts.</param>
        /// <param name="serviceRetryWaitTime">Retry delay time between each retry attempt.</param>
        /// <returns>FMChannelFactory{``0}.</returns>
        public static FMChannelFactory<TServiceContractType> GasboyStationChannelFactory<TServiceContractType>(ClientServiceCredentials clientCredentials, string pURI, int? serviceMaximumRetryAttempts, int? serviceRetryWaitTime)
            where TServiceContractType : class
        {
	        var gasBoyChannelHelper = new GasboyChannelHelper();

	        var tmpFactoryConfig = gasBoyChannelHelper.CreateChannelFactoryConfigInfo<TServiceContractType>();

	        tmpFactoryConfig.EndPointAddress = pURI;
	        tmpFactoryConfig.EndPointAddressContainsServiceName = false;

            FMChannelFactory<TServiceContractType> channelFactory = new FMChannelFactory<TServiceContractType>(tmpFactoryConfig);

            if (null != clientCredentials)
            {
                // In order to set the client authentication credentials based on a value configured through the FuelsManager UI (and not in the Web.Config), 
                // we must remove the default client credentials within the endpoint behavior and replace it with a new one that contains our 
                // application defined credentials.
                // Even though the factory exposes a SetCertificate, it can be flagged as readonly and will throw an exception if you try to update it.
                // This approach is reliable.
                bool addCredentials = false;

                ClientCredentials authCredentials =
                    channelFactory.Factory.Endpoint.Behaviors.Find<ClientCredentials>();

                if (null == authCredentials)
                {
                    addCredentials = true;
                    authCredentials = new ClientCredentials();
                }

                if (!string.IsNullOrEmpty(clientCredentials.NetworkAuthUserName))
                {
                    authCredentials.Windows.ClientCredential.UserName = clientCredentials.NetworkAuthUserName;
                }

                if (!string.IsNullOrEmpty(clientCredentials.NetworkAuthPassword))
                {
                    authCredentials.Windows.ClientCredential.Password = clientCredentials.NetworkAuthPassword;
                }

                if (!string.IsNullOrEmpty(clientCredentials.NetworkAuthDomain))
                {
                    authCredentials.Windows.ClientCredential.Domain = clientCredentials.NetworkAuthDomain;
                }

                if (!string.IsNullOrEmpty(clientCredentials.NetworkAuthClientCertificate))
                {
                    authCredentials.ClientCertificate.Certificate =
                        GetAuthenticationCertificate(clientCredentials.NetworkAuthClientCertificate);
                }

                // If there were no credentials to begin with, we need to add them.
                if (addCredentials)
                {
                    channelFactory.Factory.Endpoint.Behaviors.Add(authCredentials);
                }
            }

            if (serviceMaximumRetryAttempts.HasValue && serviceMaximumRetryAttempts.Value > 0)
            {
                channelFactory.MaximumRetryAttempts = serviceMaximumRetryAttempts.Value;
            }
            else
            {
                channelFactory.MaximumRetryAttempts = FMChannelHelper.DefaultRetryAttempts;
            }

            if (serviceRetryWaitTime.HasValue && serviceRetryWaitTime.Value > 0)
            {
                channelFactory.RetryWaitTime = serviceRetryWaitTime.Value;
            }
            else
            {
                channelFactory.RetryWaitTime = FMChannelHelper.DefaultRetryAttempts;
            }

            return (channelFactory);
        }

        /// <summary>
        /// Use the provided method to make a call over the appropriate channel using the provided binding information.
        /// </summary>
        /// <typeparam name="TServiceContractType">The type of service contract that we'd like to call</typeparam>
        /// <param name="clientCredentials">The client synchronize configuration.</param>
        /// <param name="bindingTypeName">Name of the binding type.</param>
        /// <param name="bindingConfigName">Name of the binding configuration.</param>
        /// <param name="bindingURI">The binding URI.</param>
        /// <param name="serviceMaximumRetryAttempts">Maximum number of retry attempts.</param>
        /// <param name="serviceRetryWaitTime">Retry delay time between each retry attempt.</param>
        /// <param name="channelMethod">The channel method.</param>
        /// <remarks>A new FMChannelFactory instance is created for every call.  This can impact performance if
        /// multiple calls to the same endpoint are required. Consider using a cached factory and call <see cref="M:FMChannelHelper.MakeCall`1{ServiceContractType}`2{ResultType}(FMChannelFactory`1{ServiceContractType}, Func`1{ServiceContractType}`2{ResultType})" /></remarks>
        public static void MakeCall<TServiceContractType>(ClientServiceCredentials clientCredentials, string bindingURI, int? serviceMaximumRetryAttempts, int? serviceRetryWaitTime, Action<TServiceContractType> channelMethod)
            where TServiceContractType : class
        {
            // Rather than calling directly to MakeCallProxy, we're calling the overloaded public method so that parameter validation is applied.
            MakeCall<TServiceContractType, bool>(clientCredentials
                                                , bindingURI
                                                , serviceMaximumRetryAttempts
                                                , serviceRetryWaitTime
                                                , (parameter) =>
                                                {
                                                    channelMethod(parameter);
                                                    return true;
                                                }
                );
        }

        /// <summary>
        /// Use the provided method to make a call over the appropriate channel using the provided binding information.
        /// </summary>
        /// <typeparam name="TServiceContractType">The type of service contract that we'd like to call</typeparam>
        /// <typeparam name="TResultType">The type of the result</typeparam>
        /// <param name="clientCredentials">The client synchronize configuration.</param>
        /// <param name="bindingTypeName">The binding to use when creating the specified endpoint.  ie: basicHttpBinding, wsHttpBinding, etc</param>
        /// <param name="bindingConfigName">Specify the binding configuration to use if the multiple endpoints share the same service type.</param>
        /// <param name="bindingURI">The endpoint address for the service call.</param>
        /// <param name="serviceMaximumRetryAttempts">Maximum number of retry attempts.</param>
        /// <param name="serviceRetryWaitTime">Retry delay time between each retry attempt.</param>
        /// <param name="channelMethod">The service contract method to call</param>
        /// <returns>The type of result identified by TResult</returns>
        /// <exception cref="System.ArgumentNullException">
        /// BindingTypeName and BindingURI must be provided.
        /// or
        /// bindingTypeName;The binding type must be provided.
        /// or
        /// bindingURI;The endpoint URI must be provided.
        /// </exception>
        /// <remarks>A new FMChannelFactory instance is created for every call.  This can impact performance if
        /// multiple calls to the same endpoint are required. Consider using a cached factory and call <see cref="M:FMChannelHelper.MakeCall`1{ServiceContractType}`2{ResultType}(FMChannelFactory`1{ServiceContractType}, Func`1{ServiceContractType}`2{ResultType})" /></remarks>
        public static TResultType MakeCall<TServiceContractType, TResultType>(ClientServiceCredentials clientCredentials, string bindingURI, int? serviceMaximumRetryAttempts, int? serviceRetryWaitTime, Func<TServiceContractType, TResultType> channelMethod)
            where TServiceContractType : class
        {
			if (string.IsNullOrEmpty(bindingURI))
                throw new ArgumentNullException(@"bindingURI", @"The endpoint URI must be provided.");

            return (MakeProxyCall<TServiceContractType, TResultType>(GasboyManagerChannelHelper.GasboyStationChannelFactory<TServiceContractType>(clientCredentials, bindingURI, serviceMaximumRetryAttempts, serviceRetryWaitTime), channelMethod));
        }
        #endregion Public Static Methods

        #region Private Static Methods
        /// <summary>
        /// The get transport authentication certificate.
        /// </summary>
        /// <param name="findBySubjectName">
        /// Subject Name of the certificate to load.
        /// </param>
        /// <returns>
        /// The <see cref="X509Certificate2"/>.
        /// </returns>
        private static X509Certificate2 GetAuthenticationCertificate(string findBySubjectName)
        {
            X509Certificate2 authCertificate = null;

            if (!string.IsNullOrEmpty(findBySubjectName))
            {
                X509Store certStore = new X509Store(StoreLocation.LocalMachine);
                certStore.Open(OpenFlags.ReadOnly);
                X509Certificate2Collection certColl = certStore.Certificates.Find(X509FindType.FindBySubjectName, findBySubjectName, true);

                if (certColl.Count > 0)
                {
                    authCertificate = certColl[0];
                }

                certStore.Close();
            }

            return authCertificate;
        }

        
        #endregion Private Static Methods

	    public override FMChannelFactoryConfigInfo CreateChannelFactoryConfigInfo<TServiceContractType>()
	    {
			var tmpFactoryConfig = new FMChannelFactoryConfigInfo(GetServiceName<TServiceContractType>());
			tmpFactoryConfig.EndPointAddressSetByConfigFile = true;
			tmpFactoryConfig.EndpointAddressConfigKey = "AfssServiceProcessEndPointAddress";
			tmpFactoryConfig.EndPointConfigurationSetByConfigFile = true;
			tmpFactoryConfig.EndPointConfigurationConfigKey = "AfssServiceProcessBindingName";
		    tmpFactoryConfig.EndPointAddress = "net.tcp://localhost:8733/AfssServiceProcess"; //default service in case not found in config file
		    tmpFactoryConfig.EndPointAddressSetByConfigFile = true;

			return tmpFactoryConfig;
	    }

    }
}

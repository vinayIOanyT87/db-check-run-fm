///***************************************************************************
/// Module Name:  FMSyncChannelHelper
/// Author:       George Peters
/// Copyright (c) Varec, Inc.  All rights reserved.
///***************************************************************************

namespace FMBusinessObjects.ChannelFactories
{
	using System;
	using System.Configuration;
	using System.IO;
	using System.Security.Cryptography.X509Certificates;
	using System.ServiceModel.Description;

	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;


	public class FMSyncChannelHelper : BaseChannelHelper<FMSyncChannelHelper>
	{
		private static string userAgent;

		private static bool userAgentSet;

		//public override string EndPointBindingConfigKey
		//{
		//	get
		//	{
		//		return "syncEnterpriseBusinessBindingType";
		//	}
		//}

		//public override string EndPointAddressConfigKey
		//{
		//	get
		//	{
		//		return "syncEnterpriseBusinessBindingConfiguration";
		//	}
		//}

		public const string BindingConfigurationConfigKey = "syncEnterpriseBusinessBindingConfiguration";
		public const string BindingTypeConfigKey = "syncEnterpriseBusinessBindingType";

		#region Public Static Methods
		/// <summary>
		/// Synchronizes the channel factory.
		/// </summary>
		/// <typeparam name="TServiceContractType">The type of the t service contract type.</typeparam>
		/// <param name="clientSyncConfig">The client synchronize configuration.</param>
		/// <param name="bindingTypeName">Name of the binding type.</param>
		/// <param name="bindingConfigName">Name of the binding configuration.</param>
		/// <param name="bindingUri">The endpoint address for the service call.</param>
		/// <returns>FMChannelFactory{``0}.</returns>
		public static FMChannelFactory<TServiceContractType> SyncChannelFactory<TServiceContractType>(SyncClientConfigurationDO clientSyncConfig, string bindingUri)
			where TServiceContractType : class
		{
			SetUserAgent();

			FMSyncChannelHelper tmp = new FMSyncChannelHelper();
			FMChannelFactoryConfigInfo tmpChannelConfig = tmp.CreateChannelFactoryConfigInfo<TServiceContractType>();

			tmpChannelConfig.EndPointAddressSetByConfigFile = false;
			tmpChannelConfig.EndPointAddress = bindingUri;

			FMChannelFactory<TServiceContractType> syncChannelFactory = new FMChannelFactory<TServiceContractType>(tmpChannelConfig);

			// In order to set the client authentication credentials based on a value configured through the FuelsManager UI (and not in the Web.Config), 
			// we must remove the default client credentials within the endpoint behavior and replace it with a new one that contains our 
			// application defined credentials.
			// Even though the factory exposes a SetCertificate, it can be flagged as readonly and will throw an exception if you try to update it.
			// This approach works as long as the channel factory has not been used.  Once the channel factory's CreateChannel method has been called, you must 
			// destroy the existing channel and recreate it in order to update the user credentials.
			ClientCredentials existingCredentials = syncChannelFactory.Factory.Endpoint.Behaviors.Find<ClientCredentials>();
			ClientCredentials newCredentials = new ClientCredentials();

			// If Server authentication credentials have been defined, we need to add them.
			// If the endpoint already has ClientCredentials associated with it AND they're different than the new credentials; we'll need to destroy the existing channel and recreate it.
			//
			if (clientSyncConfig.HasServerAuthenticationCredentials)
			{
				X509Certificate2 newServerAuthCertificate = LoadCertificate(clientSyncConfig.ServerAuthClientCertificate);
				bool credentialsChanged = CredentialsModified(existingCredentials, clientSyncConfig, newServerAuthCertificate);

				// If we're configured to authenticate at the Transport layer, make sure we add the new or updated credentials 
				if (credentialsChanged)
				{
					// If there were existing credentials, we're required to destroy the old channel factory and create a new one with the updated information.
					if (null != existingCredentials)
					{
						syncChannelFactory.RemoveChannelFactory();
						syncChannelFactory = new FMChannelFactory<TServiceContractType>(tmpChannelConfig);

						// Remove the default one on the newly created one.
						existingCredentials = syncChannelFactory.Factory.Endpoint.Behaviors.Find<ClientCredentials>();
						syncChannelFactory.Factory.Endpoint.EndpointBehaviors.Remove(existingCredentials);
					}

					//  Always default to using certificates if one has been specified.  Otherwise check for UserName and Password.
					if (null != newServerAuthCertificate)
					{
						newCredentials.ClientCertificate.Certificate = newServerAuthCertificate;
					}
					else
					{
						if (!string.IsNullOrEmpty(clientSyncConfig.ServerAuthUserName))
						{
							newCredentials.Windows.ClientCredential.UserName = clientSyncConfig.ServerAuthUserName;
						}

						if (!string.IsNullOrEmpty(clientSyncConfig.ServerAuthPassword))
						{
							newCredentials.Windows.ClientCredential.Password = clientSyncConfig.ServerAuthPassword;
						}

						if (!string.IsNullOrEmpty(clientSyncConfig.ServerAuthDomain))
						{
							newCredentials.Windows.ClientCredential.Domain = clientSyncConfig.ServerAuthDomain;
						}
					}

					syncChannelFactory.Factory.Endpoint.Behaviors.Add(newCredentials);
				}
			}

			// A custom user-agent can be provided by applying the behavior extension to a behavior configuration in 
			// either the web.config or app.config.  The resulting behavior configuration definition can then be associated to the service configuration.
			//
			// Optionally, a user-agent can be passed in and the user agent behavior will get attached to the endpoint.  This allows the user-agent to be
			// defined / configured via the application user interface.
			//
			if (!string.IsNullOrEmpty(userAgent))
			{
				HttpUserAgentEndpointBehavior userAgentBehavior = syncChannelFactory.Factory.Endpoint.Behaviors.Find<HttpUserAgentEndpointBehavior>();

				if (null == userAgentBehavior)
				{
					syncChannelFactory.Factory.Endpoint.Behaviors.Add(new HttpUserAgentEndpointBehavior(userAgent));
				}
				else
				{
					userAgentBehavior.UserAgent = userAgent;
				}
			}

			syncChannelFactory.MaximumRetryAttempts = clientSyncConfig.ServiceMaximumRetryAttempts;

			syncChannelFactory.RetryWaitTime = clientSyncConfig.ServiceRetryWaitTime;

			return (syncChannelFactory);
		}

		private static void SetUserAgent()
		{
			if (userAgentSet)
			{
				return;
			}
			userAgent = ConfigurationManager.AppSettings["UserAgentValue"];
			userAgentSet = true;
		}

		/// <summary>
		/// Use the provided method to make a call over the appropriate channel using the provided binding information.
		/// </summary>
		/// <typeparam name="TServiceContractType">The type of service contract that we'd like to call</typeparam>
		/// <param name="clientSyncConfig">The client synchronize configuration.</param>
		/// <param name="bindingTypeName">Name of the binding type.</param>
		/// <param name="bindingConfigName">Name of the binding configuration.</param>
		/// <param name="bindingUri">The endpoint address for the service call.</param>
		/// <param name="channelMethod">The channel method.</param>
		/// <remarks>A new FMChannelFactory instance is created for every call.  This can impact performance if
		/// multiple calls to the same endpoint are required. Consider using a cached factory and call <see cref="M:FMChannelHelper.MakeCall`1{ServiceContractType}`2{ResultType}(FMChannelFactory`1{ServiceContractType}, Func`1{ServiceContractType}`2{ResultType})" /></remarks>
		public static void MakeCall<TServiceContractType>(SyncClientConfigurationDO clientSyncConfig, string bindingUri, Action<TServiceContractType> channelMethod)
			where TServiceContractType : class
		{
			// Rather than calling directly to MakeCallProxy, we're calling the overloaded public method so that parameter validation is applied.
			MakeCall<TServiceContractType, bool>(clientSyncConfig
												, bindingUri
												, (parameter) => { 
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
		/// <param name="clientSyncConfig">The client synchronize configuration.</param>
		/// <param name="bindingTypeName">The binding to use when creating the specified endpoint.  ie: basicHttpBinding, wsHttpBinding, etc</param>
		/// <param name="bindingConfigName">Specify the binding configuration to use if the multiple endpoints share the same service type.</param>
		/// <param name="bindingUri">The endpoint address for the service call.</param>
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
		public static TResultType MakeCall<TServiceContractType, TResultType>(SyncClientConfigurationDO clientSyncConfig, string bindingUri, Func<TServiceContractType, TResultType> channelMethod)
			where TServiceContractType : class
		{
			if (string.IsNullOrEmpty(bindingUri))
				throw new ArgumentNullException(@"bindingUri", @"The endpoint URI must be provided.");

			return (MakeProxyCall<TServiceContractType, TResultType>(FMSyncChannelHelper.SyncChannelFactory<TServiceContractType>(clientSyncConfig, bindingUri), channelMethod));
		}
		#endregion Public Static Methods

		#region Private Static Methods
		/// <summary>
		/// The get transport authentication certificate.
		/// </summary>
		/// <param name="subjectNameOrThumbprint">
		/// Subject Name of the certificate to load.
		/// </param>
		/// <returns>
		/// The <see cref="X509Certificate2"/>.
		/// </returns>
		private static X509Certificate2 LoadCertificate(string subjectNameOrThumbprint)
		{
			if (string.IsNullOrEmpty(subjectNameOrThumbprint))
			{
				return null;
			}

			var certStore = new X509Store(StoreLocation.LocalMachine);

			try
			{
				certStore.Open(OpenFlags.ReadOnly);

				var authenticationCert = certStore.Certificates.Find(X509FindType.FindByThumbprint, subjectNameOrThumbprint, false);

				if (authenticationCert.Count == 0) //otherwise, it should be the subject name
				{
					authenticationCert = certStore.Certificates.Find(X509FindType.FindBySubjectName, subjectNameOrThumbprint, true);
				}

				if (authenticationCert.Count == 0)
				{
					throw new FileNotFoundException(string.Format("Cert with name or thumbprint: '{0}' not found in local machine cert store.", subjectNameOrThumbprint));
				}

				return authenticationCert[0];
			}
			finally
			{
				certStore.Close();
			}
		}

		private static bool CredentialsModified(
			ClientCredentials previousClientCredentials,
			SyncClientConfigurationDO clientConfigurationDO,
			X509Certificate2 newServerAuthCertificate
			)
		{
			bool changeDetected = false;

			if (previousClientCredentials.ClientCertificate.Certificate != null || newServerAuthCertificate != null)
			{
				// If a certificate was either added or being removed; we've changed.
				changeDetected = ((previousClientCredentials.ClientCertificate.Certificate == null
										&& newServerAuthCertificate != null)
									|| (previousClientCredentials.ClientCertificate.Certificate != null && newServerAuthCertificate == null));
			}

			if (!changeDetected)
			{
				int hash1 = (previousClientCredentials.ClientCertificate.Certificate != null)
					? previousClientCredentials.ClientCertificate.Certificate.GetHashCode()
					: 0;

				int hash2 = (newServerAuthCertificate != null) ? newServerAuthCertificate.GetHashCode() : 0;

				// If the Hash values don't match then something's different
				changeDetected = !hash1.Equals(hash2);
			}

			if (!changeDetected)
			{
				changeDetected = (previousClientCredentials.Windows.ClientCredential.Domain == null && clientConfigurationDO.ServerAuthDomain != null)
										|| (previousClientCredentials.Windows.ClientCredential.Domain != null
										&& clientConfigurationDO.ServerAuthDomain != null
										&& !previousClientCredentials.Windows.ClientCredential.Domain.Equals(clientConfigurationDO.ServerAuthDomain.Replace(".", string.Empty).Trim()));


				if (!changeDetected)
				{
					changeDetected = (previousClientCredentials.Windows.ClientCredential.UserName == null && clientConfigurationDO.ServerAuthUserName != null)
											|| (previousClientCredentials.Windows.ClientCredential.UserName != null
											&& clientConfigurationDO.ServerAuthUserName != null
											&& !previousClientCredentials.Windows.ClientCredential.UserName.Equals(clientConfigurationDO.ServerAuthUserName.Trim()));

					if (!changeDetected)
					{
						changeDetected = (previousClientCredentials.Windows.ClientCredential.Password == null && clientConfigurationDO.ServerAuthPassword != null)
												|| (previousClientCredentials.Windows.ClientCredential.Password != null
												&& clientConfigurationDO.ServerAuthPassword != null
												&& !previousClientCredentials.Windows.ClientCredential.Password.Equals(clientConfigurationDO.ServerAuthPassword.Trim()));
					}
				}
			}

			return changeDetected;
		}

		#endregion Private Static Methods


		public override FMChannelFactoryConfigInfo CreateChannelFactoryConfigInfo<TServiceContractType>()
		{
			var tmpFactoryConfig = new FMChannelFactoryConfigInfo(GetServiceName<TServiceContractType>());
			tmpFactoryConfig.EndPointBindingTypeSetByConfigFile = true;
			tmpFactoryConfig.EndPointBindingTypeConfigKey = BindingTypeConfigKey;
			tmpFactoryConfig.EndPointConfigurationSetByConfigFile = true;
			tmpFactoryConfig.EndPointConfigurationConfigKey = BindingConfigurationConfigKey;

			return tmpFactoryConfig;
		}
	}
}

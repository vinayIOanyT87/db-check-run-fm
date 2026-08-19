///***************************************************************************
/// Module Name:  FMChannelHelper
/// Author:	   Daniel Or
/// Copyright (c) Varec, Inc.  All rights reserved.
///***************************************************************************

namespace FMBusinessObjects.ChannelFactories
{
	using System;
	using System.ServiceModel;

	using FMBusinessObjects.Exceptions;

	public abstract class BaseChannelHelper<T> where T : BaseChannelHelper<T>, new()
	{


		/// <summary>
		/// The number of times that we should retry a channel operation if the result was a transient error
		/// for example, retry later or timeouts
		/// </summary>
		public const int DefaultRetryAttempts = 2;

		/// <summary>
		/// The amount of time, in milliseconds, that we should wait before retrying a channel operation
		/// </summary>
		public const int DefaultRetryWaitTime = 1000;

		public const int DefaultNodeHealthCriticalThresholdHours = 24;

		public const int DefaultNodeHealthCautionThresholdHours = 12;



		//public virtual string EndPointAddress
		//{
		//	get
		//	{
		//		return "";
		//	}
		//}

		/// <summary>
		/// Gracefully close a channel
		/// </summary>
		/// <param name="channel">The channel to close</param>
		public static void CloseChannel(object channel)
		{
			IClientChannel clientChannel = channel as IClientChannel;
			if (clientChannel != null)
			{
				if (clientChannel.State.Equals(CommunicationState.Faulted))
				{
					clientChannel.Abort();
				}
				else
				{
					try
					{
						clientChannel.Close();
					}
					catch (TimeoutException)
					{
						clientChannel.Abort();
					}
					catch (CommunicationException)
					{
						clientChannel.Abort();
					}
				}
			}
		}


		/// <summary>
		/// Abort the channel, for example when we encounter an exception
		/// </summary>
		/// <param name="channel">The channel to abort</param>
		public static void AbortChannel(object channel)
		{
			IClientChannel clientChannel = channel as IClientChannel;
			if (clientChannel != null)
			{
				clientChannel.Abort();
			}
		}

		protected static string GetServiceName<TServiceContractType>()
		{
			string serviceName = typeof(TServiceContractType).ToString();
			return serviceName.Substring(serviceName.LastIndexOf('.') + 1);

		}

		//protected abstract FMChannelFactoryConfigInfo CreateChannelFactoryConfigInfo<TServiceContractType>();

		public virtual FMChannelFactoryConfigInfo CreateChannelFactoryConfigInfo<TServiceContractType>()
		{
			var tmpFactoryConfig = new FMChannelFactoryConfigInfo(GetServiceName<TServiceContractType>());
			//tmpFactoryConfig.EndPointAddressSetByConfigFile = true;
			//tmpFactoryConfig.EndpointAddressConfigKey = "endPointAddress";
			//tmpFactoryConfig.endpoint = true;
			//tmpFactoryConfig.EndPointConfigurationConfigKey = "bindingName";

			return tmpFactoryConfig;
		}

		#region Public Static Methods

		/// <summary>
		/// Use the provided method to make a call over the appropriate channel
		/// </summary>
		/// <typeparam name="TServiceContractType">The type of service contract that we'd like to call</typeparam>
		/// <param name="channelMethod">The service contract method to call</param>
		/// <remarks>
		/// A new FMChannelFactory instance is created for every call.  This can impact performance if 
		/// multiple calls to the same endpoint are required. Consider using a cached factory and call <see cref="M:FMChannelHelper.MakeCall`1{ServiceContractType}`2{ResultType}(FMChannelFactory`1{ServiceContractType}, Func`1{ServiceContractType}`2{ResultType})"/>
		/// </remarks>
		public static void MakeCall<TServiceContractType>(Action<TServiceContractType> channelMethod)
			where TServiceContractType : class
		{
			MakeCall<TServiceContractType, bool>(
					(parameter) =>
					{
						channelMethod(parameter);
						return true;
					}
				);
		}

        /// <summary>
        /// For custom implementation directly with the channel.  
        /// Returns config information along with the channel.
        /// </summary>
        /// <typeparam name="TServiceContractType"></typeparam>
        /// <returns></returns>
        public static FMChannelInfo<TServiceContractType> GetChannel<TServiceContractType>() where TServiceContractType : class
        {
            var parent = new T();
            FMChannelFactoryConfigInfo tmpConfig = parent.CreateChannelFactoryConfigInfo<TServiceContractType>();
            var channelFactory = new FMChannelFactory<TServiceContractType>(tmpConfig);
            return new FMChannelInfo<TServiceContractType>()
            {
                Channel = channelFactory.CreateProxy(),
                NumberOfAttemptsConfigured = channelFactory.MaximumRetryAttempts,
                RetryWaitTime = channelFactory.RetryWaitTime
            };
        }

		/// <summary>
		/// Use the provided method to make a call over the appropriate channel using the provided binding information.
		/// </summary>
		/// <typeparam name="TServiceContractType">The type of service contract that we'd like to call</typeparam>
		/// <param name="bindingTypeName">The binding to use when creating the specified endpoint.  ie: basicHttpBinding, wsHttpBinding, etc</param>
		/// <param name="bindingConfigName">Specify the binding configuration to use if the multiple endpoints share the same service type.</param>
		/// <param name="bindingURI">The endpoint address for the service call.</param>
		/// <param name="channelMethod">The service contract method to call</param>
		/// <remarks>
		/// A new FMChannelFactory instance is created for every call.  This can impact performance if 
		/// multiple calls to the same endpoint are required. Consider using a cached factory and call <see cref="M:FMChannelHelper.MakeCall`1{ServiceContractType}`2{ResultType}(FMChannelFactory`1{ServiceContractType}, Func`1{ServiceContractType}`2{ResultType})"/>
		/// </remarks>
		public static void MakeCall<TServiceContractType>(string bindingTypeName, string bindingConfigName, string bindingURI, Action<TServiceContractType> channelMethod)
			where TServiceContractType : class
		{
			// Rather than calling directly to MakeCallProxy, we're calling the overloaded public method so that parameter validation is applied.
			MakeCall<TServiceContractType, bool>(bindingTypeName,
												bindingConfigName,
												bindingURI,
												(parameter) =>
												{
													channelMethod(parameter);
													return true;
												}
				);
		}

		/// <summary>
		/// Using an existing Channel Factory, use the provided method to make a call over the appropriate channel.
		/// </summary>
		/// <typeparam name="TServiceContractType">The type of service contract that we'd like to call</typeparam>
		/// <param name="channelFactory">An instance of an existing FMChannelFactory.</param>
		/// <param name="channelMethod">The service contract method to call</param>
		/// <remarks>
		/// Performance can be improved by utilizing a cached FMChannelFactory instance so call this method 
		/// when multiple calls to the same endpoint are needed within a single 'logical' session.
		/// </remarks>
		public static void MakeCall<TServiceContractType>(FMChannelFactory<TServiceContractType> channelFactory, Action<TServiceContractType> channelMethod)
			where TServiceContractType : class
		{
			// Rather than calling directly to MakeCallProxy, we're calling the overloaded public method so that parameter validation is applied.
			MakeCall<TServiceContractType, bool>(channelFactory,
												(parameter) =>
												{
													channelMethod(parameter);
													return true;
												}
				);
		}

		/// <summary>
		/// Use the provided method to make a call over the appropriate channel
		/// </summary>
		/// <typeparam name="TServiceContractType">The type of service contract that we'd like to call</typeparam>
		/// <typeparam name="TResultType">The type of the result</typeparam>
		/// <param name="channelMethod">The service contract method to call</param>
		/// <returns>The type of result identified by TResult</returns>
		/// <remarks>
		/// A new FMChannelFactory instance is created for every call.  This can impact performance if 
		/// multiple calls to the same endpoint are required. Consider using a cached factory and call <see cref="M:FMChannelHelper.MakeCall`1{ServiceContractType}`2{ResultType}(FMChannelFactory`1{ServiceContractType}, Func`1{ServiceContractType}`2{ResultType})"/>
		/// </remarks>
		public static TResultType MakeCall<TServiceContractType, TResultType>(Func<TServiceContractType, TResultType> channelMethod)
			where TServiceContractType : class
		{

			var newHelper = new T();

			FMChannelFactoryConfigInfo tmpConfig = newHelper.CreateChannelFactoryConfigInfo<TServiceContractType>();

			return (MakeProxyCall<TServiceContractType, TResultType>(new FMChannelFactory<TServiceContractType>(tmpConfig), channelMethod));
		}

		/// <summary>
		/// Use the provided method to make a call over the appropriate channel using the provided binding information.
		/// </summary>
		/// <typeparam name="TServiceContractType">The type of service contract that we'd like to call</typeparam>
		/// <typeparam name="TResultType">The type of the result</typeparam>
		/// <param name="bindingTypeName">The binding to use when creating the specified endpoint.  ie: basicHttpBinding, wsHttpBinding, etc</param>
		/// <param name="bindingConfigName">Specify the binding configuration to use if the multiple endpoints share the same service type.</param>
		/// <param name="bindingURI">The endpoint address for the service call.</param>
		/// <param name="channelMethod">The service contract method to call</param>
		/// <returns>The type of result identified by TResult</returns>
		/// <remarks>
		/// A new FMChannelFactory instance is created for every call.  This can impact performance if 
		/// multiple calls to the same endpoint are required. Consider using a cached factory and call <see cref="M:FMChannelHelper.MakeCall`1{ServiceContractType}`2{ResultType}(FMChannelFactory`1{ServiceContractType}, Func`1{ServiceContractType}`2{ResultType})"/>
		/// </remarks>
		public static TResultType MakeCall<TServiceContractType, TResultType>(string bindingTypeName, string bindingConfigName, string bindingURI, Func<TServiceContractType, TResultType> channelMethod)
			where TServiceContractType : class
		{
            if (string.IsNullOrEmpty(bindingTypeName))
				throw new ArgumentNullException("bindingTypeName", "The binding type must be provided.");
			else if (string.IsNullOrEmpty(bindingURI))
				throw new ArgumentNullException("bindingURI", "The endpoint URI must be provided.");

			var newHelper = new T();

			FMChannelFactoryConfigInfo tmpConfig = newHelper.CreateChannelFactoryConfigInfo<TServiceContractType>();
			tmpConfig.EndPointAddressSetByConfigFile = false;
			tmpConfig.EndPointAddress = bindingURI;
			tmpConfig.EndPointBindingTypeSetByConfigFile = false;
			tmpConfig.EndPointBindingType = bindingTypeName;
			tmpConfig.EndPointConfigurationSetByConfigFile = false;
			tmpConfig.EndPointConfiguration = bindingConfigName;

            return (MakeProxyCall<TServiceContractType, TResultType>(new FMChannelFactory<TServiceContractType>(tmpConfig), channelMethod));
		}

		/// <summary>
		/// Using an existing Channel Factory, use the provided method to make a call over the appropriate channel.
		/// </summary>
		/// <typeparam name="TServiceContractType">The type of service contract that we'd like to call</typeparam>
		/// <typeparam name="TResultType">The type of the result</typeparam>
		/// <param name="channelFactory">An instance of an existing FMChannelFactory.</param>
		/// <param name="channelMethod">The service contract method to call</param>
		/// <returns>The type of result identified by TResult</returns>
		/// <remarks>
		/// Performance can be improved by utilizing a cached FMChannelFactory instance so call this method 
		/// when multiple calls to the same endpoint are needed within a single 'logical' session.
		/// </remarks>
		public static TResultType MakeCall<TServiceContractType, TResultType>(FMChannelFactory<TServiceContractType> channelFactory, Func<TServiceContractType, TResultType> channelMethod)
			where TServiceContractType : class
		{
			return (MakeProxyCall<TServiceContractType, TResultType>(channelFactory, channelMethod));
		}

		#endregion Public Static Methods


		#region Private Static Proxy Call Methods
		/// <summary>
		/// An internal method that makes a call over the appropriate channel using either the default proxy or 
		/// a proxy created based on the binding information passed in.
		/// </summary>
		/// <typeparam name="TServiceContractType">The type of service contract that we'd like to call</typeparam>
		/// <typeparam name="TResultType">The type of the result</typeparam>
		/// <param name="fmChannelFactory">An instance of a channel factory on which the channel method should be executed.</param>
		/// <param name="channelMethod">The service contract method to call.</param>
		/// <returns>The type of result identified by TResult</returns>
		/// <remarks>If a transient error is encountered, this method will retry the service contract method up to the maximum number specified by the RetryAttempts property.</remarks>
		protected static TResultType MakeProxyCall<TServiceContractType, TResultType>(FMChannelFactory<TServiceContractType> fmChannelFactory, Func<TServiceContractType, TResultType> channelMethod)
			where TServiceContractType : class
		{
			TResultType retValue = default(TResultType);

			TServiceContractType myChannel = null;

			for (int idx = 0; idx < fmChannelFactory.MaximumRetryAttempts; idx++)
			{
				try
				{
					myChannel = fmChannelFactory.CreateProxy();

					if (myChannel != null)
					{
						// Open the channel
						((IClientChannel)myChannel).Open();

						// call the method or segment of code
						retValue = channelMethod(myChannel);

						// close the channel
						CloseChannel(myChannel);
					}
					else
					{
						throw new Exception("Channel creation failed.");
					}

					break;
                }
                catch(System.ServiceModel.ProtocolException exception)
				{
                    System.Diagnostics.Trace.TraceError(exception.ToString());
                    throw new Exception("Cannot communicate with the requested service.");
				}
				catch (Exception error)
				{
					AbortChannel(myChannel);

					// If the error is a transient error, retry 
					if ((idx + 1 < fmChannelFactory.MaximumRetryAttempts) &&
						((error is EndpointNotFoundException) ||
						(error is ChannelTerminatedException) ||
						(error is ServerTooBusyException) ||
						(error is SyncCommunicationException)))
					{
						System.Threading.Thread.Sleep(fmChannelFactory.RetryWaitTime);
					}
					else
					{
						System.Diagnostics.Trace.TraceError(error.ToString());
						throw;
					}
				}
			}

			return retValue;
		}

		#endregion Private Static Proxy Call Methods
		
	}

	public class FMChannelHelper : BaseChannelHelper<FMChannelHelper>
	{

		public override FMChannelFactoryConfigInfo CreateChannelFactoryConfigInfo<TServiceContractType>()
		{
			var tmpFactoryConfig = new FMChannelFactoryConfigInfo(GetServiceName<TServiceContractType>());
			tmpFactoryConfig.EndPointAddressSetByConfigFile= true;
			tmpFactoryConfig.EndpointAddressConfigKey = "endPointAddress";
			tmpFactoryConfig.EndPointConfigurationSetByConfigFile = true;
			tmpFactoryConfig.EndPointConfigurationConfigKey = "bindingName";
         tmpFactoryConfig.EndPointBehaviorNameConfigKey = "behaviorName";

         return tmpFactoryConfig;
		}
	}

    public class FMChannelInfo<T>
    {
        public T Channel { get; set; }
        public int NumberOfAttemptsConfigured { get; set; }
        public int RetryWaitTime { get; set; }
    }
}

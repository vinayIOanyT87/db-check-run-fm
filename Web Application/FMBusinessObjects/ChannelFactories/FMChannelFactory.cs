// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMChannelFactory.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace FMBusinessObjects.ChannelFactories
{
	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.ServiceModel;
	using System.ServiceModel.Configuration;
	using System.ServiceModel.Description;

	using Binding = System.ServiceModel.Channels.Binding;

	/// <summary>
	/// A channel factory for FMBusinessServices. Underneath, it caches actual channel
	///     factories for performance reasons.
	/// </summary>
	/// <typeparam name="T">
	/// The interface name to build channels for.
	/// </typeparam>
	public class FMChannelFactory<T>
		where T : class
	{
		#region Constants and Fields

		/// <summary>
		///     Interface for reading configuration information for channel factory proxy generation.
		/// </summary>
		private FMChannelFactoryConfigInfo channelConfigInfo;

		/// <summary>
		/// A reference to the actual factory
		/// </summary>
		private readonly ChannelFactory<T> theFactory;

		/// <summary>
		/// The number of times that we should retry a channel operation if the result was a transient error
		/// for example, retry later or timeouts
		/// </summary>
		private int maximumRetryAttempts = 2;

		/// <summary>
		/// The amount of time, in milliseconds, that we should wait before retrying a channel operation
		/// </summary>
		private int retryWaitTime = 2000;

		#endregion Constants and Fields

		#region Constructors and Destructors

		//public FMChannelFactory() : this(new FMChannelFactoryConfigInfo(ServiceName))
		//{

		//}

		/// <summary>
		///     Initializes a new instance of the <see cref="FMChannelFactory{T}" /> class.
		///     Default constructor
		/// </summary>
		public FMChannelFactory(FMChannelFactoryConfigInfo channelConfigInfo)
		{
			this.channelConfigInfo = channelConfigInfo;
			this.Binding = null;
			this.Address = null;

			// Check if underlying factory has already been created
			// Set defaults if necessary
			this.Address = this.channelConfigInfo.GetEndPointAddress();
			this.Binding = this.channelConfigInfo.GetBinding();

			// Attempt to retrieve a matching factory from the cache. If none exists, delegate
			// sent to GetOrAdd() will create new one and add it to the cache. Must use delegate
			// to avoid unnecessarily creating a new one every time
			this.theFactory =
				FMChannelFactoryCache.Instance.ChannelFactories.GetOrAdd(this.Key, constructChannelFactoryDelegate => ConstructFactory(this.Binding, this.Address, this.channelConfigInfo.EndPointBehaviorName)) as
				ChannelFactory<T>;
		}

		///////// <summary>
		///////// Initializes a new instance of the <see cref="FMChannelFactory{T}"/> class.
		/////////     Constructor with binding and address
		///////// </summary>
		///////// <param name="binding">
		///////// Binding
		///////// </param>
		///////// <param name="address">
		///////// Endpoint Address
		///////// </param>
		//////public FMChannelFactory(Binding binding, EndpointAddress address, FMChannelFactoryConfigInfo channelConfigInfo)
		//////{
		//////	this.channelConfigInfo = channelConfigInfo;
		//////	this.Binding = binding;
		//////	this.Address = address;

		//////	// Attempt to retrieve a matching factory from the cache. If none exists, delegate
		//////	// sent to GetOrAdd() will create new one and add it to the cache. Must use delegate
		//////	// to avoid unnecessarily creating a new one every time
		//////	this.theFactory =
		//////		FMChannelFactoryCache.Instance.ChannelFactories.GetOrAdd(this.Key, constructChannelFactoryDelegate => ConstructFactory(this.Binding, this.Address)) as
		//////		ChannelFactory<T>;
		//////}

		/////// <summary>
		/////// Initializes a new instance of the <see cref="FMChannelFactory{T}"/> class.
		///////     Constructor with binding and uri
		/////// </summary>
		/////// <param name="binding">
		/////// Binding
		/////// </param>
		/////// <param name="uri">
		/////// string
		/////// </param>
		////public FMChannelFactory(Binding binding, string uri, FMChannelFactoryConfigInfo channelConfigInfo)
		////{
		////	this.channelConfigInfo = channelConfigInfo;
		////	this.channelConfigInfo.EndPointAddress = uri;
		////	this.Binding = binding;
		////	this.Address = this.channelConfigInfo.GetEndPointAddress();

		////	// Attempt to retrieve a matching factory from the cache. If none exists, delegate
		////	// sent to GetOrAdd() will create new one and add it to the cache. Must use delegate
		////	// to avoid unnecessarily creating a new one every time
		////	this.theFactory =
		////		FMChannelFactoryCache.Instance.ChannelFactories.GetOrAdd(this.Key, constructChannelFactoryDelegate => ConstructFactory(this.Binding, this.Address)) as
		////		ChannelFactory<T>;
		////}

		///////// <summary>
		///////// Initializes a new instance of the <see cref="FMChannelFactory{T}"/> class.
		/////////     Constructor with BindingType Name, BindingConfig Name and URI
		///////// </summary>
		///////// <param name="bindingTypeName">
		///////// The binding Type Name.
		///////// </param>
		///////// <param name="bindingConfigName">
		///////// The binding Config Name.
		///////// </param>
		///////// <param name="uri">
		///////// string
		///////// </param>
		//////public FMChannelFactory(string bindingTypeName, string bindingConfigName, string uri, FMChannelFactoryConfigInfo channelConfigInfo)
		//////{
		//////	this.channelConfigInfo = channelConfigInfo;
		//////	this.Binding = this.channelConfigInfo.GetBinding(bindingTypeName, bindingConfigName);
		//////	this.Address = this.channelConfigInfo.CreateEndPointAddress(uri);

		//////	// Attempt to retrieve a matching factory from the cache. If none exists, delegate
		//////	// sent to GetOrAdd() will create new one and add it to the cache. Must use delegate
		//////	// to avoid unnecessarily creating a new one every time
		//////	this.theFactory =
		//////		FMChannelFactoryCache.Instance.ChannelFactories.GetOrAdd(this.Key, constructChannelFactoryDelegate => ConstructFactory(this.Binding, this.Address)) as
		//////		ChannelFactory<T>;
		//////}

		#endregion Constructors and Destructors

		#region Public Properties

		/// <summary>
		/// Gets or sets the location of the service
		/// </summary>
		/// <value>
		/// The address.
		/// </value>
		public EndpointAddress Address { get; set; }

		/// <summary>
		/// Gets or sets the binding objectg that specifies protocols, transports, and message encoders used for communication
		/// </summary>
		public Binding Binding { get; set; }

		/// <summary>
		/// Gets or sets the key.
		/// </summary>
		/// <value>
		/// The key.
		/// </value>
		private string Key
		{
			get
			{
				return typeof(T) + "|" + this.Binding.GetType() + "|" + this.Address;
			}
		}

		/// <summary>
		/// Gets the factory object.
		/// </summary>
		public ChannelFactory<T> Factory
		{
			get
			{
				return this.theFactory;
			}
		}

		/// <summary>
		/// Gets or sets the maximum retry attempts.
		/// </summary>
		/// <value>The maximum retry attempts.</value>
		public int MaximumRetryAttempts
		{
			get
			{
				return this.maximumRetryAttempts;
			}
			set
			{
				this.maximumRetryAttempts = value;
			}
		}

		/// <summary>
		/// Gets or sets the retry wait time.
		/// </summary>
		/// <value>The retry wait time.</value>
		public int RetryWaitTime
		{
			get
			{
				return this.retryWaitTime;
			}
			set
			{
				this.retryWaitTime = value;
			}
		}

		#endregion Public Properties

		#region Public Methods and Operators

		public void RemoveChannelFactory()
		{
			FMChannelFactoryCache.Instance.RemoveChannelFactory(this.Key);
		}

		/// <summary>
		///     This method will return a channel proxy for a
		///     given WCF service interface (i.e. Companies, Products, etc.)
		/// </summary>
		/// <returns>the proxy</returns>
		public T CreateProxy()
		{
			return this.theFactory.CreateChannel();
		}

		#endregion Public Methods and Operators

		#region Static Methods

		public void RefreshConfiguration()
		{
			this.channelConfigInfo.RefreshConfiguration();
		}

		/// <summary>
		/// Static delegate to create a new factory when none exists in the cache.
		/// </summary>
		/// <param name="binding">
		/// Binding
		/// </param>
		/// <param name="address">
		/// Endpoint Address
		/// </param>
		/// <returns>
		/// New channel factory
		/// </returns>
		private static ChannelFactory<T> ConstructFactory(Binding binding, EndpointAddress address, string endPointBehaviorName)
		{
			var newFactory = new ChannelFactory<T>(binding, address);

			var behaviors = GetServiceBehavior(endPointBehaviorName);

			if (behaviors != null)
			{
				foreach (Type bType in behaviors.Keys)
				{
					newFactory.Endpoint.Behaviors.Remove(bType);
					newFactory.Endpoint.Behaviors.Add((IEndpointBehavior)behaviors[bType]);
				}
			}

			// Set the max data graph length
			foreach (OperationDescription op in newFactory.Endpoint.Contract.Operations)
			{
				var dataContractBehavior = op.Behaviors.Find<DataContractSerializerOperationBehavior>();
				if (dataContractBehavior != null)
				{
					dataContractBehavior.MaxItemsInObjectGraph = 2147483647;
				}
			}

			return newFactory;
		}

		private static string ServiceName
		{
			get
			{
				string serviceName = typeof(T).ToString();
				return serviceName.Substring(serviceName.LastIndexOf('.') + 1);
			}
		}

		private static Dictionary<Type, object> GetServiceBehavior(string name)
		{
			Dictionary<Type, object> behaviorList = new Dictionary<Type, object>();

			BehaviorsSection behaviorData = (BehaviorsSection)ConfigurationManager.GetSection("system.serviceModel/behaviors");

			if (name == null || !behaviorData.EndpointBehaviors.ContainsKey(name))
			{
				return null;
			}

			EndpointBehaviorElement serviceElement = behaviorData.EndpointBehaviors[name];
			foreach (BehaviorExtensionElement behaviorPart in serviceElement)
			{
				behaviorList.Add(behaviorPart.BehaviorType, CreateBehavior(behaviorPart));
			}

			return behaviorList;
		}

		private static object CreateBehavior(BehaviorExtensionElement element)
		{
			return element.GetType().GetMethod("CreateBehavior",
				System.Reflection.BindingFlags.Instance |
				System.Reflection.BindingFlags.NonPublic).Invoke(
					element, new object[0] { });
		}

		#endregion Static Methods
	}
}
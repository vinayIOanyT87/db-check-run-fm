namespace FMBusinessObjects.ChannelFactories
{
	using System;
	using System.Collections.Concurrent;
	using System.ServiceModel;

	/// <summary>
	/// Manages RSA certificates, this class interacts with the internal RSA key store
	/// </summary>
	internal class FMChannelFactoryCache
	{
		#region Constants and Fields

		/// <summary>
		/// The is disposed.
		/// </summary>
		private bool isDisposed = false;

		/// <summary>
		/// Mechanism for safe operation in a multi-threaded environment
		/// </summary>
		private static Object singleton = new Object();

		/// <summary>
		/// Keeps track of the singleton instance in the scope of the running thread
		/// </summary>
		protected static FMChannelFactoryCache instance = null;

		/// <summary>
		/// Private member for ChannelFactories property
		/// </summary>
		// ReSharper disable StaticFieldInGenericType
		// Static member in generic type OK here since the types do not involve the generic type.
		private static ConcurrentDictionary<string, ChannelFactory> channelFactories = new ConcurrentDictionary<string, ChannelFactory>();


		#endregion Constants and Fields

		#region Constructors and Destructors

		/// <summary>
		/// Default constructor, cannot be constructed by any callers other than itself
		/// </summary>
		private FMChannelFactoryCache()
		{
			this.isDisposed = false;
		}


		#endregion Constructors and Destructors

		#region Properties

		/// <summary>
		/// Gets the cached Channel Factories
		/// </summary>
		public ConcurrentDictionary<string, ChannelFactory> ChannelFactories
		{
			get
			{
				return channelFactories ?? (channelFactories = new ConcurrentDictionary<string, ChannelFactory>());
			}
		}

		#endregion Properties

		#region Public Properties

		/// <summary>
		/// Gets the singleton instance, if it doesn't exist then it will be created in a mutually exclusive way
		/// </summary>
		public static FMChannelFactoryCache Instance
		{
			get
			{
				if (null != instance)
				{
					return instance;
				}
				else
				{
					lock (singleton)
					{
						if (null == instance)
						{
							instance = new FMChannelFactoryCache();
						}
					}

					return instance;
				}
			}
		}

		#endregion Public Properties


		#region Public Methods

		public void RemoveChannelFactory(string key)
		{
			if (channelFactories != null)
			{
				ChannelFactory factory = null;
				channelFactories.TryRemove(key, out factory);
				if (factory != null && factory.State == CommunicationState.Opened)
				{
					try
					{
						factory.Close();
					}
					catch (TimeoutException)
					{
						factory.Abort();
					}
					catch (CommunicationException)
					{
						factory.Abort();
					}
				}
			}
		}

		#endregion Public Methods
		private void RemoveChannelFactories()
		{
			if (channelFactories != null)
			{
				foreach (string key in channelFactories.Keys)
				{
					this.RemoveChannelFactory(key);
				}
			}
		}
		#region Private Methods

		#endregion Private Methods

		#region IDisposable Interface Implementation

		/// <summary>
		/// Disposes the FMChannelFactory for a specific type 
		/// </summary>
		/// <param name="disposing">True if explicit finalization, false if through GC</param>
		protected virtual void Dispose(bool disposing)
		{
			if (this.isDisposed)
			{
				return;
			}

			if (disposing)
			{
				this.RemoveChannelFactories();
			}

			this.isDisposed = true;
		}

		/// <summary>
		/// Disposes this Client Sync Provider instance 
		/// </summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion IDisposable Interface Implementation
	}
}

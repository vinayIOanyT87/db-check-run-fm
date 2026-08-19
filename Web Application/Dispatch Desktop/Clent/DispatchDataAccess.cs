// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DispatchDataAccess.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Dispatch
{
	using System;
	using System.Configuration;
	using System.Data;
	using System.Diagnostics;
	using System.Linq;
	using System.Threading;
	using System.Timers;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;

	using FMDispatchBusinessObjects.BusinessInterfaces;
	using FMDispatchBusinessObjects.ChannelFactories;

	using Timer = System.Timers.Timer;

	public enum DispatchDependencyType
	{
		Requests, 
		Equipment, 
		Personnel
	}

	public class DispatchDataAccess : IDisposable
	{
		#region Fields
		private readonly object lockingEquObject = new object();
		private readonly object lockingPerObject = new object();
		private readonly SecurityClass security;
		private EquipmentCollectionClass equipmentCache;
		private EntityVersion equipmentVersion;
		private DateTime lastChangeTime = DateTime.MinValue;
		private long lastTransactionVersion;
		private EntityVersion personVersion;
		private PersonCollectionClass personnelCache;
		private Timer pollingTimer;
		#endregion

		#region Constructors and Destructors
		public DispatchDataAccess(SecurityClass security)
		{
			this.AlreadyDisposed = false;
			this.security = security;

			this.equipmentVersion = new EntityVersion();
			this.personVersion = new EntityVersion();

			// Set up data engine polling timer
			this.pollingTimer = new Timer(this.GetPollingIntervalInSeconds() * 1000);
			this.pollingTimer.Elapsed += this.PollingTimerElapsed;
		}

		~DispatchDataAccess()
		{
			this.Dispose();
		}
		#endregion

		#region Delegates
		public delegate void OnDataUpdatedHandler(object data, DateTime queryTime);
		public delegate void OnErrorHandler(Exception except, bool bFatalError);
		#endregion

		#region Public Events
		public event OnDataUpdatedHandler OnDataUpdated;
		public event OnDataUpdatedHandler OnEquipmentUpdated;
		public event OnErrorHandler OnError;
		public event OnDataUpdatedHandler OnPersonnelUpdated;
		#endregion

		#region Properties
		protected bool AlreadyDisposed { get; set; }
		#endregion

		#region Public Methods and Operators
		public virtual void Dispose()
		{
			if (!this.AlreadyDisposed)
			{
				if (this.pollingTimer != null)
				{
					this.pollingTimer.Enabled = false;
				}

				this.AlreadyDisposed = true;
			}
		}

		public EquipmentCollectionClass GetEquipment()
		{
			this.GetEquipmentFromDb();
			return this.equipmentCache;
		}

		public void GetEquipmentFromDb()
		{
			lock (this.lockingEquObject)
			{
					this.equipmentCache =
						FMChannelHelper.MakeCall<IClientDispatchService, EquipmentCollectionClass>(x => x.EnumerateEquipmentBySource(this.security));
			}
		}

		public EquipmentCollectionClass GetEquipmentNoUpdateConnection()
		{
			this.GetEquipmentFromDb();
			return this.equipmentCache;
		}

		public PersonCollectionClass GetPersonnel()
		{
			return this.InternalGetPersonnel();
		}

		public PersonCollectionClass GetPersonnelFromDb()
		{
		    PersonCollectionClass offloaderCollection;
		    PersonCollectionClass loaderCollection;
		    PersonCollectionClass fullCollection = new PersonCollectionClass();

			loaderCollection =
				FMChannelHelper.MakeCall<IClientDispatchService, PersonCollectionClass>(
					x => x.EnumeratePersonnelByRole(this.security, PERSON_ROLE.LOADER_ROLE));
            offloaderCollection =
				FMChannelHelper.MakeCall<IClientDispatchService, PersonCollectionClass>(
                    x => x.EnumeratePersonnelByRole(this.security, PERSON_ROLE.OFFLOADER_ROLE));

            loaderCollection.Union(offloaderCollection).ToList().ForEach(x => fullCollection.Add(x));
		    return fullCollection;
		}

        public PersonCollectionClass GetPersonnelNoUpdateConnection()
		{
			return this.InternalGetPersonnel();
		}

		public DispatchTransactionsDO GetTransactions(DispatchTransactionsSR sr)
		{
			return this.GetTransactions(sr, false); // by default, we dont want to reset the version
		}

		public DispatchTransactionsDO GetTransactions(DispatchTransactionsSR sr, bool resetPollingCheck)
		{
			if (resetPollingCheck)
			{
				this.lastTransactionVersion = 0;
			}
			else
			{
				this.lastTransactionVersion = this.GetLatestTransactionVersion();
			}

			return FMChannelHelper.MakeCall<IClientDispatchService, DispatchTransactionsDO>(x => x.GetLineItems(sr));
		}

		public DispatchTransactionsDO GetTransactionsNoUpdateConnection(DispatchTransactionsSR sr)
		{
			return this.GetTransactions(sr);
		}

		public void PausePolling()
		{
			lock (this.pollingTimer)
			{
				this.pollingTimer.Enabled = false;
			}
		}

		public void StartPolling()
		{
			lock (this.pollingTimer)
			{
				this.pollingTimer.Enabled = true;
			}
		}
		#endregion

		#region Methods
		private bool CheckForEquipmentUpdates()
		{
			EntityVersion latestEquipmentVersion = this.GetLatestEquipmentVersion();

			if (latestEquipmentVersion.IsDifferent(this.equipmentVersion))
			{
				this.equipmentVersion = latestEquipmentVersion;
				return true;
			}

			return false;
		}

		private bool CheckForPersonnelUpdates()
		{
			EntityVersion latestPersonVersion = this.GetLatestPersonVersion();

			if (latestPersonVersion.IsDifferent(this.personVersion))
			{
				this.personVersion = latestPersonVersion;
				return true;
			}

			return false;
		}

		private bool CheckForTransactionUpdates()
		{
			long transVersion = this.GetLatestTransactionVersion();

			if (transVersion > this.lastTransactionVersion || (DateTime.Now - this.lastChangeTime).TotalMinutes > 1)
			{
				this.lastChangeTime = DateTime.Now;
				this.lastTransactionVersion = transVersion;
				return true;
			}

			return false;
		}

		private void DispatchDataAccessInternalOnEquipmentUpdated()
		{
			lock (this.lockingEquObject)
			{
				this.GetEquipmentFromDb();
				EquipmentCollectionClass local = this.equipmentCache;
				Interlocked.Exchange(ref this.equipmentCache, local);
			}

			if (this.OnEquipmentUpdated != null)
			{
				this.OnEquipmentUpdated.Invoke(null, DateTime.Now);
			}
		}

		private void DispatchDataAccessInternalOnPersonnelUpdated()
		{
			lock (this.lockingPerObject)
			{
				PersonCollectionClass local = this.GetPersonnelFromDb();

				Interlocked.Exchange(ref this.personnelCache, local);
			}

			if (this.OnPersonnelUpdated != null)
			{
				this.OnPersonnelUpdated.Invoke(null, DateTime.Now);
			}

			// Set up data engine polling timer
			this.pollingTimer = new Timer(this.GetPollingIntervalInSeconds() * 1000);
			this.pollingTimer.Elapsed += this.PollingTimerElapsed;
		}

		private EntityVersion GetLatestEquipmentVersion()
		{
			var version = new EntityVersion();

			DataSet dataSet = FMChannelHelper.MakeCall<IClientDispatchService, DataSet>(x => x.EnumerateEquipmentUpdateVersions(this.security));

			if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
			{
				DataRow row = dataSet.Tables[0].Rows[0];
				version.Count = row["Count"] as int?;
				version.TopIndex = row["TopIndex"] as int?;
				version.TopDate = row["TopDate"] as DateTimeOffset?;
			}

			version.SetDefaultsIfNull();
			return version;
		}

		private EntityVersion GetLatestPersonVersion()
		{
			var version = new EntityVersion();

			DataSet dataSet = FMChannelHelper.MakeCall<IClientDispatchService, DataSet>(x => x.EnumeratePersonUpdateVersions(this.security));

			if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
			{
				DataRow row = dataSet.Tables[0].Rows[0];
				version.Count = row["Count"] as int?;
				version.TopIndex = row["TopIndex"] as int?;
				version.TopDate = row["TopDate"] as DateTimeOffset?;
			}

			version.SetDefaultsIfNull();
			return version;
		}

		private long GetLatestTransactionVersion()
		{
			// Determine if we have any transaction updates to communicate
			long result = FMChannelHelper.MakeCall<IClientDispatchService, long>(
				accountingInterface =>
					{
						var sr = new DispatchTransactionsSR
							         {
								         SubCommand = DispatchTransactionsSR.SubCommands.GetVersion, 
								         Security = this.security
							         };

						DispatchTransactionsDO results = accountingInterface.ProcessDispatchTransactionServiceRequest(sr);

						// Get the version and determine if it changed.
						if (results.Transactions != null 
							&& results.Transactions.Tables.Count > 0
						    && results.Transactions.Tables[0].Rows.Count > 0)
						{
							var version = BitConverter.ToInt64( (byte[])results.Transactions.Tables[0].Rows[0]["_RowVersion"], 0 );
							
							return version;
						}

						return 0;
					});

			return result;
		}

		/// <summary>
		///     This method returns the polling interval to use in seconds.  It attempts to read a valid setting from
		///     the application configuration file.  If no valid setting is found, it defaults to 10 second intervals.
		/// </summary>
		/// <returns>The polling interval in seconds.</returns>
		private int GetPollingIntervalInSeconds()
		{
			// Default to 5 seconds 
			int pollingInterval = 5;

			// Get polling configuration value
			string pollingIntervalText = ConfigurationManager.AppSettings["PollingInterval"];

			try
			{
				if (string.IsNullOrEmpty(pollingIntervalText) == false)
				{
					pollingInterval = Convert.ToInt32(pollingIntervalText);
				}
			}
			catch (FormatException)
			{
				// do nothing - go with default
			}
			catch (OverflowException)
			{
				// do nothing - go with default
			}

			// Enforce a minimum polling time of 2 seconds
			if (pollingInterval < 2)
			{
				pollingInterval = 2;
			}

			return pollingInterval;
		}

		private PersonCollectionClass InternalGetPersonnel()
		{
			lock (this.lockingPerObject)
			{
				if (this.personnelCache == null)
				{
					Debug.WriteLine("person was null have to get from db");

					this.personnelCache = this.GetPersonnelFromDb();
				}
			}

			return this.personnelCache;
		}

		private void InvokeError(Exception e, bool bFatalError)
		{
			if (this.OnError != null)
			{
				this.OnError.Invoke(e, bFatalError);
			}
			else
			{
				throw new ApplicationException("Exception from SqlDependency processing");
			}
		}

		private void PollingTimerElapsed(object sender, ElapsedEventArgs e)
		{
			lock (this.pollingTimer)
			{
				try
				{
					// Turn off the timer so we won't get another one while we are doing this one
					this.pollingTimer.Enabled = false;

					// Call the notification routine
					if (this.OnDataUpdated != null)
					{
						// Check for Transaction updates.  
						if (this.CheckForTransactionUpdates())
						{
							this.OnDataUpdated.Invoke(null, DateTime.Now);
						}
					}

					//// Only check if equipment callback set.  Unlike transactions, Not everyone who uses a
					//// DispatchDataAccess classa actually defines one of these callbacks.
					if (this.CheckForEquipmentUpdates())
					{
						this.DispatchDataAccessInternalOnEquipmentUpdated();
					}

					// Only check if personnel callback set.  Unlike transactions, Not everyone who uses a
					// DispatchDataAccess classa actually defines one of these callbacks.
					if (this.CheckForPersonnelUpdates())
					{
						this.DispatchDataAccessInternalOnPersonnelUpdated();
					}
				}
				catch (Exception except)
				{
					// Call the registered error handling routine
					this.InvokeError(except, false);
				}
				finally
				{
					this.pollingTimer.Enabled = true;
				}
			}
		}
		#endregion

		public class EntityVersion
		{
			#region Fields
			public int? Count;
			public DateTimeOffset? TopDate;
			public long? TopIndex;
			#endregion

			#region Constructors and Destructors
			public EntityVersion()
			{
				this.SetDefaultsIfNull();
			}
			#endregion

			#region Public Methods and Operators
			public bool IsDifferent(EntityVersion entityVersion)
			{
				return entityVersion.TopIndex != this.TopIndex || entityVersion.Count != this.Count
				       || entityVersion.TopDate != this.TopDate;
			}

			public void SetDefaultsIfNull()
			{
				// Set default values if the properties are null
				this.Count = this.Count ?? 0;
				this.TopIndex = this.TopIndex ?? 0;
				this.TopDate = this.TopDate ?? DateTimeOffset.MinValue;
			}
			#endregion
		}
	}
}
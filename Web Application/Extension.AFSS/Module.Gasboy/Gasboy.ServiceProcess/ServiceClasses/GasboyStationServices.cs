// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GasboyStationServices.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Provides an implementation of the GasboyStationServices Interface which provides APIs to manage the synchronization
//   of data between Remote Gasboy Fuel Service Stations and the FuelsManager software.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FuelsManager.Afss.Module.Gasboy.ServiceProcess.ServiceClasses
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.ServiceModel;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;

	using FuelsManager.Afss.BusinessObjects.Constants;
	using FuelsManager.Afss.Module.Gasboy.BusinessObjects.BusinessInterfaces;
	using FuelsManager.Afss.Module.Gasboy.BusinessObjects.ChannelFactories;
	using FuelsManager.Afss.Module.Gasboy.BusinessObjects.Constants;
	using FuelsManager.Afss.Module.Gasboy.BusinessObjects.DataObjects;
	using FuelsManager.Afss.Module.Gasboy.BusinessObjects.ServiceProcessInterfaces;
	using FuelsManager.Afss.Module.Gasboy.OrCU;

	/// <summary>
	/// Class GasboyStationServices.
	/// </summary>
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = true)]
	public class GasboyStationServices : IGasboyStationServices
	{
		#region Attributes

		// This service is a single instance service so we want to make sure it's idle before performing certain activities
		private bool WorkInProgress = false;

		/// <summary>
		/// The Gasboy communications controller
		/// </summary>
		private readonly GasboyController GasboyController = new GasboyController();

		/// <summary>
		/// Contains the security context before the station connection test started
		/// </summary>
		private static SecurityClass _GasboyTestConnectionSecurity = null;

		/// <summary>
		/// Contains the security context before the station connection test started
		/// </summary>
		private static SecurityClass _GasboyDownloadTransactionsSecurity = null;

		/// <summary>
		/// Contains the security context before the station connection test started
		/// </summary>
		private static SecurityClass _GasboyPushDevicesSecurity = null;

		#endregion Attributes

		/// <summary>
		/// The service name
		/// </summary>
		public static string ServiceName = "GasboyServices";

		#region Operational APIs
		/// <summary>
		/// This method performs the default tasks associated with normal communication sessions with a remote fuel service station.  It controls the sequence of
		/// operations and activities unique to each type of remote fuel service station.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <returns>A list of external station GUIDs and the results of the work</returns>
		/// <exception cref="System.NotImplementedException"></exception>
		public void DoWork(SecurityClass security)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Update the configuration data stored at one or more stations
		/// </summary>
		/// <param name="security">Contains security information</param>
		/// <param name="externalStationGuids">Identifies the external stations to download transactions for</param>
		/// <returns>A list of external station GUIDs and the results of the download</returns>
		/// <exception cref="System.NotImplementedException"></exception>
		public Dictionary<Guid, object> UpdateStationConfiguration(SecurityClass security, List<Guid> externalStationGuids)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Downloads all new transactions for all configured stations
		/// </summary>
		/// <param name="security">The security.</param>
		/// <returns>A list of external station GUIDs and the results of the transaction download</returns>
		/// <exception cref="System.NotImplementedException"></exception>
		public Dictionary<Guid, string> DownloadAllNewTransactions(SecurityClass security)
		{
			var downloadResults = new Dictionary<Guid, string>();

			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			var gasboyStationList =
				GasboyChannelHelper.MakeCall<IGasboyStations, List<GasboyStation>>(x => x.Enumerate(security));

			foreach (var gasboyStation in gasboyStationList)
			{
				if (this.GasboyController.GetTransactions(security, gasboyStation, ExternalStationSessionType.Manual, null))
				{
					downloadResults.Add(gasboyStation.IdentityGuid, "SUCCESS!");
				}
				else
				{
					downloadResults.Add(gasboyStation.IdentityGuid, "FAILED!");
				}
			}

			return downloadResults;
		}

		/// <summary>
		/// Download transactions based on the provided range of Transaction IDs
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="externalStationGuid">The external station unique identifier.</param>
		/// <param name="beginTransactionID">The begin transaction identifier.</param>
		/// <param name="endTransactionID">The end transaction identifier.</param>
		/// <returns>The results of the download</returns>
		/// <exception cref="System.ArgumentNullException">security</exception>
		/// <exception cref="FMInsufficientRightsException"></exception>
		public string DownloadSelectedTransaction(SecurityClass security, Guid externalStationGuid, long? beginTransactionID, long? endTransactionID)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_AUTOMATED_FUEL_SERVICE_STATION))
			{
				throw new FMInsufficientRightsException();
			}

			return "";
		}

		/// <summary>
		/// Download new events from the specified stations
		/// </summary>
		/// <param name="security">Contains security information</param>
		/// <param name="externalStationGuids">The stations to get events for</param>
		/// <returns>The result of the download for each station</returns>
		public Dictionary<Guid, string> GetNewEventsForStations(SecurityClass security, List<Guid> externalStationGuids)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_AUTOMATED_FUEL_SERVICE_STATION))
			{
				throw new FMInsufficientRightsException();
			}

			Dictionary<Guid, string> results = new Dictionary<Guid, string>();

			try
			{
				foreach (Guid externalStationGuid in externalStationGuids)
				{
					try
					{
						// Get the station object
						Guid stationGuid = externalStationGuid;

						var station = GasboyChannelHelper.MakeCall<IGasboyStations, GasboyStation>(
								gasboyStations => gasboyStations.Get(security, stationGuid));

						if (station != null)
						{

							station.LastConnectionAttempt = DateTimeOffset.Now;
							GasboyChannelHelper.MakeCall<IGasboyStations>(externalStationsService => externalStationsService.Modify(security, station));

							// Give it to the controller to process
							if (this.GasboyController.GetEvents(security, station, ExternalStationSessionType.Manual, null))
							{
								results.Add(externalStationGuid, "Event Download Successful");
								station.LastSuccessfulConnection = DateTimeOffset.Now;
								GasboyChannelHelper.MakeCall<IGasboyStations>(externalStationsService => externalStationsService.Modify(security, station));
							}
						}
					}
					catch (Exception e)
					{
						//_EventLog.WriteEntry(e.ToString(), EventLogEntryType.Error);
						//results.Add(externalStationGuid, "Event Download Failed");
					}
				}
			}
			finally
			{
			}

			return results;
		}

		/// <summary>
		/// Download all new transactions for the Gasboy Stations listed.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="gasboyStationTransactions"></param>
		/// <returns>
		/// A list of Failed Transaction GUIDs and the results of each reprocessing attempt.s
		/// </returns>
		public Dictionary<Guid, string> ReprocessFailedTransactions(SecurityClass security, List<GasboyStationTransaction> gasboyStationTransactions)
		{
			var reprocessResults = new Dictionary<Guid, string>();

			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			foreach (var gasboyTransaction in gasboyStationTransactions)
			{
				// Using a local variable satisfies the warning: "Access to foreach variable in closure. May have different behaviour 
				// when compiled with different versions of compiler." when referencing the Station GUID directly.
				GasboyStationTransaction transaction = gasboyTransaction;

				var gasboyStation =
					GasboyChannelHelper.MakeCall<IGasboyStations, GasboyStation>(x => x.Get(security, transaction.ExternalStationGuid));

				if (this.GasboyController.ReprocessTransaction(security, gasboyStation, gasboyTransaction, null))
				{
					reprocessResults.Add(gasboyTransaction.IdentityGuid, "SUCCESS!");
				}
				else
				{
					reprocessResults.Add(gasboyTransaction.IdentityGuid, "FAILED!");
				}
			}

			return reprocessResults;
		}

		/// <summary>
		/// Download all new transactions for the Gasboy Stations listed.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="externalStationGuidList">The external station unique identifier list.</param>
		/// <returns>A list of external station GUIDs and the results of the download</returns>
		/// <exception cref="System.ArgumentNullException">security</exception>
		public Dictionary<Guid, string> DownloadNewTransactionsForStations(SecurityClass security, List<Guid> externalStationGuidList)
		{

			GasboyEvents gasboyEvents = new GasboyEvents();
			GasboyStationServices._GasboyDownloadTransactionsSecurity = security;


			var downloadResults = new Dictionary<Guid, string>();

			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			foreach (var stationGuid in externalStationGuidList)
			{
				Guid currentGuid = stationGuid;

				var gasboyStation =
					GasboyChannelHelper.MakeCall<IGasboyStations, GasboyStation>(x => x.Get(security, currentGuid));
				FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
				alarmAndEventChannel =>
				{
					alarmAndEventChannel.Add(
					GasboyStationServices._GasboyDownloadTransactionsSecurity,
					gasboyEvents.GasboyManualTransactionDownloadInitiatedEvent(gasboyStation.ID));
				}
				);

				if (this.GasboyController.GetTransactions(security, gasboyStation, ExternalStationSessionType.Manual, null))
				{
					downloadResults.Add(currentGuid, "SUCCESS!");
				}
				else
				{

					downloadResults.Add(currentGuid, "FAILED!");
				}
			}

			return downloadResults;
		}

		/// <summary>
		/// Downloads all gasboy devices to the specified stations.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="externalStationList">The external station list.</param>
		/// <returns>Dictionary&lt;Guid, System.String&gt;.</returns>
		/// <exception cref="System.ArgumentNullException">security</exception>
		/// <exception cref="FMBusinessObjects.Exceptions.FMInsufficientRightsException"></exception>
		public Dictionary<Guid, string> SendAllDevicesToStations(
			SecurityClass security,
			List<GasboyStation> externalStationList)
		{

			GasboyEvents gasboyEvents = new GasboyEvents();
			GasboyStationServices._GasboyPushDevicesSecurity = security;
			var deviceDownloadResults = new Dictionary<Guid, string>();

			if (security == null)
			{
				FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
					alarmAndEventChannel =>
					{
						alarmAndEventChannel.Add(
							GasboyStationServices._GasboyPushDevicesSecurity,
							gasboyEvents.GasboyDevicePushErrorEvent("ExternalStation", "Invalid Security Context."));
					});
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_AUTOMATED_FUEL_SERVICE_STATION))
			{
				FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
					alarmAndEventChannel =>
					{
						alarmAndEventChannel.Add(
							GasboyStationServices._GasboyPushDevicesSecurity,
							gasboyEvents.GasboyDevicePushErrorEvent("ExternalStation", "Insufficient rights to perform the operation."));
					});
				throw new FMInsufficientRightsException();
			}

			// Get a list of all the Gasboy Devices configured for the Site
			List<GasboyDevice> gasboyDeviceList = this.GasboyController.GetGasboyDeviceList(security, ExternalStationSessionType.Manual, null);

			if (gasboyDeviceList.Count == 0)
			{
				FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
				alarmAndEventChannel =>
				{
				alarmAndEventChannel.Add(
				GasboyStationServices._GasboyPushDevicesSecurity,
				gasboyEvents.GasboyDevicePushErrorEvent("External Station", "No devices could be loaded for the device push event."));
			});
			}

			// Update each station with the devices.
			foreach (var station in externalStationList)
			{

				station.LastConnectionAttempt = DateTimeOffset.Now;
				GasboyChannelHelper.MakeCall<IGasboyStations>(externalStationsService => externalStationsService.Modify(security, station));
				FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
					alarmAndEventChannel =>
					{
						alarmAndEventChannel.Add(
							GasboyStationServices._GasboyPushDevicesSecurity,
							gasboyEvents.GasboyDevicePushInitiatedEvent(station.ID));
					});

				if (!this.GasboyController.PushDefaultFleetAndDepartments(security, station, ExternalStationSessionType.Manual, null))
				{
					FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
						alarmAndEventChannel =>
							{
								alarmAndEventChannel.Add(
									GasboyStationServices._GasboyPushDevicesSecurity,
									gasboyEvents.GasboyDevicePushErrorEvent(station.ID, "Warning! The Gasboy Fleets and Departments could not be configured. Blacklisting may not function correctly unless the pedestal was manually configured."));
							});
				}

				if (this.GasboyController.UpdateStationDevices(
					security,
					station,
					gasboyDeviceList,
					ExternalStationSessionType.Manual,
					null))
				{
					station.LastSuccessfulConnection = DateTimeOffset.Now;
					GasboyChannelHelper.MakeCall<IGasboyStations>(externalStationsService => externalStationsService.Modify(security, station));
					FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
					alarmAndEventChannel =>
					{
						alarmAndEventChannel.Add(
			GasboyStationServices._GasboyPushDevicesSecurity,
			gasboyEvents.GasboyDevicePushCompleteEvent(station.ID, gasboyDeviceList.Count.ToString()));
					}
		);
					deviceDownloadResults.Add(station.IdentityGuid, @"SUCCESS!");
				}
				else
				{
					FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
					alarmAndEventChannel =>
					{
						alarmAndEventChannel.Add(
			GasboyStationServices._GasboyPushDevicesSecurity,
			gasboyEvents.GasboyDevicePushErrorEvent(station.ID, "The device push failed. The response from the Gasboy Station was malformed."));
					}
		);
					deviceDownloadResults.Add(station.IdentityGuid, @"FAILED!");
				}
			}

			return deviceDownloadResults;
		}


		/// <summary>
		/// Downloads the specified devices to the specified stations.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="externalStationList">The external station list.</param>
		/// <param name="gasboyDeviceList">The gasboy device list.</param>
		/// <returns>Dictionary&lt;Guid, System.String&gt;.</returns>
		/// <exception cref="System.ArgumentNullException">security</exception>
		/// <exception cref="FMBusinessObjects.Exceptions.FMInsufficientRightsException"></exception>
		public Dictionary<Guid, string> SendSelectedDevicesToStations(
			SecurityClass security, 
			List<GasboyStation> externalStationList, 
			List<GasboyDevice> gasboyDeviceList)
		{

			GasboyEvents gasboyEvents = new GasboyEvents();
			GasboyStationServices._GasboyPushDevicesSecurity = security;

			var deviceDownloadResults = new Dictionary<Guid, string>();

			if (security == null)
			{
				FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
					alarmAndEventChannel =>
					{
						alarmAndEventChannel.Add(
							GasboyStationServices._GasboyPushDevicesSecurity,
							gasboyEvents.GasboyDevicePushErrorEvent("ExternalStation", "Invalid Security Context."));
					});
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_AUTOMATED_FUEL_SERVICE_STATION))
			{

				FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
					alarmAndEventChannel =>
					{
						alarmAndEventChannel.Add(
							GasboyStationServices._GasboyPushDevicesSecurity,
							gasboyEvents.GasboyDevicePushErrorEvent("ExternalStation", "Insufficient rights to perform the operation."));
					});
				throw new FMInsufficientRightsException();
			}

			foreach (var station in externalStationList)
			{

				FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
					alarmAndEventChannel =>
						{
							alarmAndEventChannel.Add(
								GasboyStationServices._GasboyPushDevicesSecurity,
								gasboyEvents.GasboyDevicePushInitiatedEvent(station.ID));
						});

					if (this.GasboyController.UpdateStationDevices(
					security,
					station,
					gasboyDeviceList,
					ExternalStationSessionType.Manual,
					null))
				{
					FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
					alarmAndEventChannel =>
					{
						alarmAndEventChannel.Add(
			GasboyStationServices._GasboyPushDevicesSecurity,
			gasboyEvents.GasboyDevicePushCompleteEvent(station.ID, gasboyDeviceList.Count.ToString()));
					}
		);
					deviceDownloadResults.Add(station.IdentityGuid, @"SUCCESS!");
				}
				else
				{
					FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
					alarmAndEventChannel =>
					{
						alarmAndEventChannel.Add(
			GasboyStationServices._GasboyPushDevicesSecurity,
			gasboyEvents.GasboyDevicePushErrorEvent(station.ID, "The device push failed."));
					}
		);
					deviceDownloadResults.Add(station.IdentityGuid, @"FAILED!");
				}
			}

			return deviceDownloadResults;
		}

		/// <summary>
		/// Downloads the specified devices to the specified stations.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="externalStationGuids">The external station list.</param>
		/// <returns>Dictionary&lt;Guid, System.String&gt;.</returns>
		/// <exception cref="System.ArgumentNullException">security</exception>
		/// <exception cref="FMBusinessObjects.Exceptions.FMInsufficientRightsException"></exception>
		public Dictionary<Guid, string> SendBlacklistedDevicesToStations(
			SecurityClass security,
			List<Guid> externalStationGuids)
		{

			GasboyEvents gasboyEvents = new GasboyEvents();
			GasboyStationServices._GasboyPushDevicesSecurity = security;

			Dictionary<Guid, string> results = new Dictionary<Guid, string>();
			var deviceDownloadResults = new Dictionary<Guid, string>();



			if (security == null)
			{
				FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
					alarmAndEventChannel =>
						{
							alarmAndEventChannel.Add(
								GasboyStationServices._GasboyPushDevicesSecurity,
								gasboyEvents.GasboyDevicePushErrorEvent("ExternalStation", "Invalid Security Context."));
						});
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_AUTOMATED_FUEL_SERVICE_STATION))
			{
				FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
					alarmAndEventChannel =>
					{
						alarmAndEventChannel.Add(
							GasboyStationServices._GasboyPushDevicesSecurity,
							gasboyEvents.GasboyDevicePushErrorEvent("ExternalStation", "Insufficient rights to perform the operation."));
					});
				throw new FMInsufficientRightsException();
			}

			try
			{
				foreach (Guid externalStationGuid in externalStationGuids)
				{
					try
					{
						// Get the station object
						Guid stationGuid = externalStationGuid;

						var station = GasboyChannelHelper.MakeCall<IGasboyStations, GasboyStation>(
								gasboyStations => gasboyStations.Get(security, stationGuid));
						FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
						alarmAndEventChannel =>
						{
							alarmAndEventChannel.Add(
				GasboyStationServices._GasboyPushDevicesSecurity,
				gasboyEvents.GasboyDevicePushInitiatedEvent(station.ID));
						}
			);

						var gasboyDevicesUnfiltered = GasboyChannelHelper.MakeCall<IGasboyDevices, List<GasboyDevice>>(
								x => x.GetByDepartment(security, security.SiteGuid, GasboySpecialConstants.BlacklistDepartmentGuid));
                        //Only provide items that have a deviceID greater than or equal to 9000000001
					    var gasboyDevices = gasboyDevicesUnfiltered.FindAll(x => x.DeviceID >= 900000001);

						if (station != null)
						{
							// Give it to the controller to process
							if (this.GasboyController.UpdateStationDevices(security, station, gasboyDevices, ExternalStationSessionType.Manual, null))
							{
								FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
								alarmAndEventChannel =>
								{
									alarmAndEventChannel.Add(
						GasboyStationServices._GasboyPushDevicesSecurity,
						gasboyEvents.GasboyDevicePushCompleteEvent(station.ID, gasboyDevices.Count.ToString()));
								}
					);
								results.Add(externalStationGuid, "\n " + gasboyDevices.Count() + " blacklisted devices were pushed.");
							}
						}
					}
					catch (Exception e)
					{

						FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
						alarmAndEventChannel =>
						{
							alarmAndEventChannel.Add(
				GasboyStationServices._GasboyPushDevicesSecurity,
				gasboyEvents.GasboyDevicePushErrorEvent("ExternalStation", e.ToString()));
						}
			);
						//_EventLog.WriteEntry(e.ToString(), EventLogEntryType.Error);
						//results.Add(externalStationGuid, "Event Download Failed");
					}
				}
			}
			finally
			{
			}


			return results;
		}

		/// <summary>
		/// Downloads the specified devices to the specified stations.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="externalStationGuids">The external station list.</param>
		/// <returns>Dictionary&lt;Guid, System.String&gt;.</returns>
		/// <exception cref="System.ArgumentNullException">security</exception>
		/// <exception cref="FMBusinessObjects.Exceptions.FMInsufficientRightsException"></exception>
		public Dictionary<Guid, string> AllowBlacklistedDevicesToStations(
			SecurityClass security,
			List<Guid> externalStationGuids)
		{
			Dictionary<Guid, string> results = new Dictionary<Guid, string>();
			var deviceDownloadResults = new Dictionary<Guid, string>();

			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_AUTOMATED_FUEL_SERVICE_STATION))
			{
				throw new FMInsufficientRightsException();
			}

			try
			{
				foreach (Guid externalStationGuid in externalStationGuids)
				{
					try
					{
						// Get the station object
						Guid stationGuid = externalStationGuid;

						var station = GasboyChannelHelper.MakeCall<IGasboyStations, GasboyStation>(
								gasboyStations => gasboyStations.Get(security, stationGuid));

                        var gasboyDevicesUnfiltered = GasboyChannelHelper.MakeCall<IGasboyDevices, List<GasboyDevice>>(
								x => x.GetByDepartment(security, security.SiteGuid, GasboySpecialConstants.BlacklistDepartmentGuid));
                        //Only provide items that have a deviceID greater than or equal to 9000000001
                        var gasboyDevices = gasboyDevicesUnfiltered.FindAll(x => x.DeviceID >= 900000001);

						foreach (var device in gasboyDevices)
						{
							device.DepartmentID = GasboySpecialConstants.DefaultDepartmentID;}

						if (station != null)
						{
							// Give it to the controller to process
							if (this.GasboyController.UpdateStationDevices(security, station, gasboyDevices, ExternalStationSessionType.Manual, null))
							{
								results.Add(externalStationGuid, "\n " + gasboyDevices.Count() + " blacklisted devices were pushed.");
							}
						}
					}
					catch (Exception e)
					{
						//_EventLog.WriteEntry(e.ToString(), EventLogEntryType.Error);
						//results.Add(externalStationGuid, "Event Download Failed");
					}
				}
			}
			finally
			{
			}


			return results;
		}


		/// <summary>
		/// Try to download a list of products configured in the specified station
		/// </summary>
		/// <param name="security">Contains security information</param>
		/// <param name="externalStationList">Identifies the Gasboy station to download the product configuration information for</param>
		/// <returns>A list of external station ids and the results of the download</returns>
		/// <exception cref="System.ArgumentNullException">security</exception>
		/// <exception cref="FMInsufficientRightsException"></exception>
		public Dictionary<Guid, List<GasboyStationProduct>> GetProductList(SecurityClass security, List<GasboyStation> externalStationList)
		{
			var productListByStation = new Dictionary<Guid, List<GasboyStationProduct>>();

			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_AUTOMATED_FUEL_SERVICE_STATION))
			{
				throw new FMInsufficientRightsException();
			}

			foreach (var station in externalStationList)
			{
				productListByStation.Add(station.IdentityGuid, this.GasboyController.GetProductList(security, station, ExternalStationSessionType.Manual, null));
			}

			return productListByStation;
		}

		/// <summary>
		/// Try to download a list of products configured in the specified station
		/// </summary>
		/// <param name="security">Contains security information</param>
		/// <param name="externalStation">Identifies the Gasboy station to download the product configuration information for</param>
		/// <returns>A list of external station ids and the results of the download</returns>
		/// <exception cref="System.ArgumentNullException">security</exception>
		/// <exception cref="FMInsufficientRightsException"></exception>
		public List<GasboyStationProduct> GetStationProductList(SecurityClass security, GasboyStation externalStation)
		{
			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_AUTOMATED_FUEL_SERVICE_STATION))
			{
				throw new FMInsufficientRightsException();
			}

			return this.GasboyController.GetProductList(
				security,
				externalStation,
				ExternalStationSessionType.Manual,
				null);
		}


		#endregion Operational APIs

		#region Diagnostic APIs
		/// <summary>
		/// Test the connection to the provided external station
		/// </summary>
		/// <param name="security">Contains security information</param>
		/// <param name="externalStation">The external station to test the connection for</param>
		/// <returns>The results of the connection test</returns>
		/// <exception cref="System.ArgumentNullException">security
		/// or
		/// externalStation</exception>
		/// <exception cref="FMInsufficientRightsException"></exception>
		//public string TestConnection(SecurityClass security, GasboyStation externalStation)
		//{
		//    if (security == null)
		//    {
		//        throw new ArgumentNullException("security");
		//    }

		//    if (!security.HasRight(RIGHT.VIEW_EXTERNAL_STATION))
		//    {
		//        throw new FMInsufficientRightsException();
		//    }

		//    if (externalStation == null)
		//    {
		//        throw new ArgumentNullException("externalStation");
		//    }

		//    // We handle tests synchronously 
		//    var stations = new List<GasboyStation>();
		//    stations.Add(externalStation);
			
		//    var result = this.TestConnectionsAsyncInternal(security, stations);
		//    result.RunSynchronously();
			
		//    return (result.Result.Results.Values.First());
		//}

		/// <summary>
		/// Test the connection to the provided external station
		/// </summary>
		/// <param name="security">Contains security information</param>
		/// <param name="externalStation">The external station to test the connection for</param>
		/// <returns>The results of the connection test</returns>
		/// <exception cref="System.ArgumentNullException">security
		/// or
		/// externalStationList</exception>
		/// <exception cref="FMInsufficientRightsException"></exception>
		public string TestConnection(SecurityClass security, GasboyStation externalStation)
		{
			GasboyEvents gasboyEvents = new GasboyEvents();
			GasboyStationServices._GasboyTestConnectionSecurity = security;
			if (this.WorkInProgress)
			{
				FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
				alarmAndEventChannel =>
					{
					alarmAndEventChannel.Add(
					GasboyStationServices._GasboyTestConnectionSecurity,
					gasboyEvents.GasboyTestConnectionErrorEvent(externalStation.ID, "Test connection to Gasboy station failed because the system was busy."));
					}
				);
			return "SYSTEM BUSY";
			}

			if (security == null)
			{
				FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
				alarmAndEventChannel =>
				{
					alarmAndEventChannel.Add(
					GasboyStationServices._GasboyTestConnectionSecurity,
					gasboyEvents.GasboyTestConnectionErrorEvent(externalStation.ID, "Test connection to Gasboy station failed because the security context was invalid"));
				}
				);
				throw new ArgumentNullException("security");

			}

			if (!security.HasRight(RIGHT.VIEW_AUTOMATED_FUEL_SERVICE_STATION))
			{
				FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
				alarmAndEventChannel =>
				{
					alarmAndEventChannel.Add(
					GasboyStationServices._GasboyTestConnectionSecurity,
					gasboyEvents.GasboyTestConnectionErrorEvent(externalStation.ID, "Test connection to Gasboy station failed because the user did not have sufficient rights to perform the operation."));
				}
				);
				throw new FMInsufficientRightsException();
			}

			if (externalStation == null)
			{
				FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
				alarmAndEventChannel =>
				{
					alarmAndEventChannel.Add(
					GasboyStationServices._GasboyTestConnectionSecurity,
					gasboyEvents.GasboyTestConnectionErrorEvent(externalStation.ID, "Test connection to Gasboy station failed because the external station was invalid or null."));
				}
				);
				throw new ArgumentNullException("externalStation");
			}

			var stations = new List<GasboyStation>();
			stations.Add(externalStation);

			var results = this.TestConnectionsInternal(security, stations);



			if (results.Count > 0)
			{
				FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
				alarmAndEventChannel =>
			{
				alarmAndEventChannel.Add(
				GasboyStationServices._GasboyTestConnectionSecurity,
				gasboyEvents.GasboyTestConnectionSuccessEvent(externalStation.ID));
			});
			}
			else
			{
				FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
alarmAndEventChannel =>
{
	alarmAndEventChannel.Add(
					GasboyStationServices._GasboyTestConnectionSecurity,
					gasboyEvents.GasboyTestConnectionErrorEvent(externalStation.ID, "Test connection to Gasboy station failed.  The response from the Gasboy Station was malformed. "));
});
			}

			return (results.Count > 0) ? results.Values.First() : "FAILED!";
		}

		/// <summary>
		/// Test the connection to the provided external station
		/// </summary>
		/// <param name="security">Contains security information</param>
		/// <param name="externalStations">The external station to test the connection for</param>
		/// <returns>The results of the connection test</returns>
		/// <exception cref="System.ArgumentNullException">security
		/// or
		/// externalStationList</exception>
		/// <exception cref="FMInsufficientRightsException"></exception>
		public Dictionary<Guid, string> TestConnections(SecurityClass security, List<GasboyStation> externalStations)
		{
			var results = new Dictionary<Guid, string>();

			if (this.WorkInProgress)
			{
				results.Add(Guid.NewGuid(), "SYSTEM BUSY!");

				return results;
			}

			if (security == null)
			{
				throw new ArgumentNullException("security");
			}

			if (!security.HasRight(RIGHT.VIEW_AUTOMATED_FUEL_SERVICE_STATION))
			{
				throw new FMInsufficientRightsException();
			}

			if (externalStations == null)
			{
				throw new ArgumentNullException("externalStations");
			}

			return this.TestConnectionsInternal(security, externalStations);
		}

		private Dictionary<Guid, string> TestConnectionsInternal(
			SecurityClass security,
			IEnumerable<GasboyStation> externalStations)
		{
			var results = new Dictionary<Guid, string>();

			try
			{
				this.WorkInProgress = true;

				var testConnectionTasks = new List<Dictionary<Guid, string>>();

				var gasboyStations = externalStations as GasboyStation[] ?? externalStations.ToArray();

				int nextIndex = 0;

				while (nextIndex < gasboyStations.Count())
				{
					var station = gasboyStations.ElementAt(nextIndex);

					station.LastConnectionAttempt = DateTimeOffset.Now;
					GasboyChannelHelper.MakeCall<IGasboyStations>(externalStationsService => externalStationsService.Modify(security, station));

					var testResult = GasboyController.TestConnection(security, station);

					if (testResult)
					{
						station.LastSuccessfulConnection = DateTimeOffset.Now;
						GasboyChannelHelper.MakeCall<IGasboyStations>(
							externalStationsService => externalStationsService.Modify(security, station));
					}

					results.Add(station.IdentityGuid, testResult ? "SUCCESS!" : "FAILED!");
					nextIndex++;
				}
			}
			catch (Exception e)
			{
				var gasboyStations = externalStations as GasboyStation[] ?? externalStations.ToArray();
				GasboyEvents gasboyEvents = new GasboyEvents();
				FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
				alarmAndEventChannel =>
				{
					alarmAndEventChannel.Add(
									GasboyStationServices._GasboyTestConnectionSecurity,
									gasboyEvents.GasboyTestConnectionErrorEvent(gasboyStations.ElementAt(0).ID, e.ToString()));
				});

				throw;

			}
			finally
			{
				this.WorkInProgress = false;
			}

			return results;
		}
		#endregion Diagnostic APIs

		#region Standard APIs
		/// <summary>
		/// Gets the name of the service.
		/// </summary>
		/// <returns>System.String.</returns>
		public string GetServiceName()
		{
			return GasboyStationServices.ServiceName;
		}
		#endregion Standard APIs

		#region Internal APIs

		private int ReloadMaxStationConcurrencyLevel()
		{
			return 15;
		}

		/// <summary>
		/// The get service security instance.
		/// </summary>
		/// <returns>The <see cref="SecurityClass" />.</returns>
		private SecurityClass GetServiceSecurityInstance()
		{
			SecurityClass serviceProcessSecurity = new SecurityClass();
			serviceProcessSecurity.LoginSiteGuid = Guids.SiteAdminGuid;
			serviceProcessSecurity.LoginSiteID = "SiteAdmin";
			serviceProcessSecurity.SiteGuid = Guids.SiteAdminGuid;
			serviceProcessSecurity.SiteID = "SiteAdmin";
			serviceProcessSecurity.UserGuid = Guids.UserAdminGuid;
			serviceProcessSecurity.UserID = "GasboyService";
			serviceProcessSecurity.AddRight(RIGHT.BASE_EXPORT);
			serviceProcessSecurity.AddRight(RIGHT.INTERFACE_IMPORT);

			return serviceProcessSecurity;
		}
		#endregion Internal APIs

	}
}

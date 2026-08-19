/******************************************************************************

	FILE NAME:		SiteManager.cs


	PURPOSE:			SiteManagerClass


	COMMENTS:

		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2000

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+Hauser.


	AUTHOR(S):	W. Gray


	VERSION:		1.0.0  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		---------	----------  -------------------------------------------
		01/08/07		W.Gray		7.1.0.1 - Corrected error in ModifyTankGroup CSI 3950

		01/22/07		W.Gray		7.1.0.2 - Corrected CreatePhysicalInventory CSI 4062

		04/04/07		W.Gray		7.1.0.3 - Changed to Create Physical Inventory with
										current date for compatibility with Inventory Reconciliation
										and Ledger and changed to not preclude multiple physical
										inventories.  Also added GetInventoryDate to return
										InventoryDate for StationManager. (CSI 4418)

		05/31/07		W.Gray		7.1.0.4 - Changed to not clear tank and tank group
										from station load arm configuration on modify or purge.
										(CSI 4669)

		08/30/07		W.Gray		7.1.1.5 - Changed to send one PhysicalInventory transaction
										per Manager Product if MultipleLineItems set or one
										PhysicalInventory transactions otherwise.
										
		2008-04-07	C. Knight	7.3.0.0	- Add Net Capacity and Bottom Volume to PhysicalInventory
										transaction

		2008-04-17	C. Knight	7.4.0.0 - Add instantiation of Signature Stations.
		
		2008-04-21	C. Knight	7.4.0.1	- Add instantiation of Meter Stations.
										Use meter stations to read meters for end-of-day processing
										This is currently disabled, and meter station definitions
										will result in an exception being thrown

		2008-04-22	W.Gray		7.4.0.2 - Change GetInventoryDate to function around 12 midday
										rather than midnight CSI 5744	
		
		2008-05-06	V. Thompson	7.4.3.0 - Updated UpdateWatchdogOutput function.  The function was never detecting
										a numeric value (it detected a string) which prevented the counter value
										from incrementing.  There will probably be a CSI created for this.

		2008-05-15	W.Gray		7.4.3.1 - Correction to End Of Day Inventory Date

		2008-05-27	W.Gray		7.4.4.1 - Change to process End Of Day on the last day of month
										when Inhibit End Of Month is set. (CSI 5905)

		06/12/2008	W.Gray		7.4.5.0 - Change to include ItemName on
										OPC Quality Bad Messages (CSI 5961)

		06/30/2008	W.Gray		7.4.5.1 - Changed UpdateStationPermissive to only call LoadRack
										stations. (CSI 6004)

		08/18/2008	W.Gray		7.4.5.2 - Correction to End Of Day processing, error introduced
										7.4.4.1 (CSI 5905)

		08/19/2008	W.Gray		7.4.5.3 - Correction to CreatePhysicalInventoryTransactions to set
										TransactionDateTime (CSI 6093)

		08/22/2008	W.Gray		7.4.6.1 - Change to CretePhysicalInventoryTransactions to set
										TankStatus (CSI 6072)

		9/09/2008	W.Gray		7.4.6.0 - Revised to support external components (CSI 5581)

		2008-09-19	W.Gray		7.4.6.0 - Added ALLOCATION_RESET_METHOD.BOOK_MINUS_UNAVAILABLE_METHOD
										and method ResetOwnerAllocations (CSI 5558)

		09/25/2008	W.Gray		7.4.5.4 - Correction to GetTank to evaluate Site.UseLastKnownGood (CSI 6174)

		10/02/2008	W.Gray		7.4.6.1 - Revised Watchdog logic (CSI 6004)
 
		04/20/2009	W.Gray		7.5.0.1 - Revised to test for null AlarmOutputPV and WatchDogOutputPV (CSI 3296)

		10/14/2008	W.Gray		7.4.6.2 - Correction to ResetOwnerAllocations to utilize BookInventory+Loaded

		10/22/2008	W.Gray		7.4.6.3 - Correction to PurgeStation and AddStation (CSI 3509)

		07/07/2009	W.Gray		7.4.6.4 - Correction to store Standard Density in physical inventory
										transactions (CSI 4169)								

		2009-10-27	W.Gray		7.5.1.0 - Changed UserID test from "LoadRack.NET" to DBAccess.ServiceLogin for
										compatibility with BSME security requirements

		12/1/2009	W.Gray		7.5.1.1 - Revised to handle OPCServerManager CancelSubscriptions within OPCServerManager dispose
 
		12/7/2009	W.Gray		7.5.1.2 - Revised EndOfDayProcessing to properly skip End Of Day on the End of Month.
										The change is that if the End Of Day is prior to noon and the date is the 1st of the month
										then skip end of day processing.

		12/16/2009	W.Gray		7.5.1.3 - Change to service WatchDogUpdate in separate thread from
										TankScan (WI 10090)
 
		12/29/2009	W.Gray		7.5.1.4 - Change to UpdateWatchDog and UpdataAlarm to test for null process variables (WI 10269)
  
		01/28/2010	W.Gray		8.0.0.0 - Change to periodically update OPC Client with Secondary Storage Volumes (WI 10197)
*******************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Reflection;
using System.Net;
using System.Diagnostics;
using Opc;
using Opc.Da;
using Opc.Ua;

using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.Exceptions;
using ReportSvr2005 = FMBusinessObjects.ReportSvr2005;
using FMBusinessObjects.ServiceRequests;
using FMBusinessObjects.UtilityObjects;
using FMBusinessObjects.Constants;
using System.ServiceModel;

namespace LoadRackLibrary
{
	using FMBusinessObjects.Interfaces;
	using System.Configuration;

	using FMBusinessObjects.LogClient;

	public enum StateEndOfDay
	{
		Inactive = 1,
		WarningInterval = 2,
		WaitingForLoadingToStop = 3,
		PerformingEndOfDay = 4
	}

	public enum StateEndOfMonth
	{
		Inactive = 1,
		WarningInterval = 2,
		WaitingForLoadingToStop = 3,
		PerformingEndOfMonth = 4
	}

	/// <summary>
	/// Summary description for SiteManagerCollectionClass.
	/// </summary>
	/// 
	public class ProcessVariableCollectionCollectionClass : CollectionBase
	{
		public void Add(ProcessVariableCollectionClass ProcessVariableCollection)
		{
			this.List.Add(ProcessVariableCollection);
		}

		public void Remove(int index)
		{
			if (index > this.Count - 1 || index < 0)
			{
				throw (new Exception("Invalid Index"));
			}
			else
			{
				this.List.RemoveAt(index);
			}
		}

		public void Remove(ProcessVariableCollectionClass ProcessVariableCollection)
		{
			int index = 0;
			foreach (ProcessVariableCollectionClass Item in this.List)
			{
				if (Item.Equals(ProcessVariableCollection))
				{
					this.List.RemoveAt(index);
					return;
				}
				index++;
			}
		}

		public ProcessVariableCollectionClass Item(int Index)
		{
			return (ProcessVariableCollectionClass)this.List[Index];
		}
	}

	public class SiteManagerCollectionClass : CollectionBase
	{
		public void Add(SiteManagerClass SiteManager)
		{
			this.List.Add(SiteManager);
		}

		public void Remove(int index)
		{
			if (index > this.Count - 1 || index < 0)
			{
				throw (new Exception("Invalid Index"));
			}
			else
			{
				this.List.RemoveAt(index);
			}
		}

		public void Remove(SiteManagerClass SiteManager)
		{
			int index = 0;
			foreach (SiteManagerClass Item in this.List)
			{
				if (Item.Site.SiteGuid == SiteManager.Site.SiteGuid)
				{
					this.List.RemoveAt(index);
					return;
				}
				index++;
			}
		}

		public SiteManagerClass Item(int Index)
		{
			return (SiteManagerClass)this.List[Index];
		}

		public SiteManagerClass FindBySiteGuid(Guid guid)
		{
			foreach (SiteManagerClass Item in this.List)
			{
				if (Item.Site.SiteGuid == guid)
				{
					return Item;
				}
			}

			return null;
		}
	}

	/// <summary>
	/// Summary description for SiteManagerClass.
	/// </summary>
	public class SiteManagerClass : IDisposable
	{
		public AutoResetEvent PermissiveEvent = null;
		public SiteClass Site;
		public StateEndOfDay EndOfDayState;
		public StateEndOfMonth EndOfMonthState;
		public string endOfDayMessage = string.Empty;
		public int endOfDayProcessingPercentage = 0;
		public DateTimeOffset? lastSuccessfulEndOfDayTime;
		public string endOfDayError = string.Empty;

		protected const int EndOfDayUpdateInterval = 10;
		protected const int TankUpdateInterval = 10;
		protected const int PermissiveUpdateInterval = 10;
		protected const int VruUpdateInterval = 30;

		protected EventLog EventLog;
		public LoadRackManagerClass LoadRackManager;
		protected bool AlreadyDisposed = false;
		protected ManualResetEvent KillEvent = null;
		protected Thread IOThread = null;
		protected Thread TankAndEquipmentThread = null;
		protected Thread EndOfDayAndEndOfMonthThread = null;
		protected TankCollectionClass TankCollection = new TankCollectionClass();
		protected TankGroupCollectionClass TankGroupCollection = new TankGroupCollectionClass();
		protected SecurityClass security = null;
		public StationManagerCollectionClass StationManagerCollection = new StationManagerCollectionClass();
		protected ProcessVariableCollectionCollectionClass ProcessVariableCollectionCollection = new ProcessVariableCollectionCollectionClass();
		protected OPCServerManagerClass OPCServerManager;
		protected ProcessVariableClass AlarmOutputPV;
		protected ProcessVariableClass WatchdogOutputPV;
		protected ProcessVariableClass VRUSetpointPV;
		protected ProcessVariableClass VRUDeadbandPV;
		protected int PermissiveUpdateCounter = PermissiveUpdateInterval;
		protected int WatchdogUpdateCounter;
		private int vruUpdateCounter = VruUpdateInterval;
		private bool vruThresholdExceededOnPreviousCheck;
		protected DateTimeOffset EndOfDayStartTime;
		protected DateTimeOffset EndOfDayWarningTime;
		protected DateTime EndOfDayInventoryDate;
		protected DateTimeOffset EndOfMonthStartTime;
		protected DateTimeOffset EndOfMonthWarningTime;
		protected DateTime EndOfMonthInventoryDate;
		protected int EndOfDayUpdateCounter = EndOfDayUpdateInterval;
		public bool StationPermissive = true;
		protected EquipmentCollectionClass EquipmentCollection = new EquipmentCollectionClass();
		protected bool queueEquipmentNotification = true;
		protected bool ManuallyInitiatedEod;

		public SecurityClass Security => this.security;

		public SiteManagerClass(EventLog eventLog,
									  LoadRackManagerClass loadRackManager,
									  SiteClass site)
		{
			this.EventLog = eventLog;
			this.LoadRackManager = loadRackManager;
			this.Site = site;

			this.WatchdogUpdateCounter = site.WatchdogPeriod;

			this.EndOfDayState = StateEndOfDay.Inactive;
			this.EndOfMonthState = StateEndOfMonth.Inactive;
			this.endOfDayMessage = "";
			this.endOfDayProcessingPercentage = 0;
			this.lastSuccessfulEndOfDayTime = null;

			this.security = loadRackManager.Security.Clone();
			this.security.LoginSiteID = Site.ID;
			this.security.LoginSiteGuid = site.SiteGuid;
			this.security.SiteID = Site.ID;
			this.security.SiteGuid = site.SiteGuid;

			this.KillEvent = new ManualResetEvent(false);
			this.PermissiveEvent = new AutoResetEvent(false);

			this.OPCServerManager = new OPCServerManagerClass(eventLog);
			this.OPCServerManager.Invoke += new InvokeEventHandler(this.OnInvoke);

			ThreadStart IOScanStart = new ThreadStart(this.IoScan);
			this.IOThread = new Thread(IOScanStart);
			this.IOThread.Start();
			this.IOThread.Priority = ThreadPriority.Highest;

			ThreadStart TankAndEquipmentScanStart = new ThreadStart(this.TankAndEquipmentScan);
			this.TankAndEquipmentThread = new Thread(TankAndEquipmentScanStart);
			this.TankAndEquipmentThread.Start();
			this.TankAndEquipmentThread.Priority = ThreadPriority.AboveNormal;

			ThreadStart EndOfDayAndEndOfMonthScanStart = new ThreadStart(this.EndOfDayAndEndOfMonthScan);
			this.EndOfDayAndEndOfMonthThread = new Thread(EndOfDayAndEndOfMonthScanStart);
			this.EndOfDayAndEndOfMonthThread.Start();
			this.EndOfDayAndEndOfMonthThread.Priority = ThreadPriority.AboveNormal;
		}

		~SiteManagerClass()
		{
			this.Dispose();
		}

		public virtual void Dispose()
		{
			if (!this.AlreadyDisposed)
			{
				this.KillEvent?.Set();
				this.IOThread?.Join();
				this.TankAndEquipmentThread?.Join();
				this.EndOfDayAndEndOfMonthThread?.Join();

				this.OPCServerManager.CancelSubscriptions();
				this.OPCServerManager.Invoke -= new InvokeEventHandler(this.OnInvoke);
				this.OPCServerManager.Dispose();

				GC.SuppressFinalize(this);
				this.AlreadyDisposed = true;
			}
		}

		/// <summary>
		/// The purpose of this method is to allow Site to be modified while loading is in progress.
		/// </summary>
		/// <param name="Site"></param>
		public void ModifySite(SiteClass Site)
		{
			bool ChangeFromToMilitary = false;

			Monitor.Enter(this);

			try
			{
				this.UninitializeSite();

				if ((Site.TimePattern.IndexOf("H") == -1
				&& this.Site.TimePattern.IndexOf("H") != -1)
				|| (Site.TimePattern.IndexOf("H") != -1
				&& this.Site.TimePattern.IndexOf("H") == -1))
				{
					ChangeFromToMilitary = true;
				}

				this.Site = Site;

				this.WatchdogUpdateCounter = Site.WatchdogPeriod;

				this.Security.LoginSiteID = Site.ID;
				this.Security.SiteID = Site.ID;

				this.EndOfDayState = StateEndOfDay.Inactive;
				this.EndOfMonthState = StateEndOfMonth.Inactive;
				this.endOfDayMessage = "";
				this.endOfDayProcessingPercentage = 0;

				this.InitializeSite();
			}
			catch (Exception e)
			{
				this.EventLog.WriteEntry(e.ToString(), EventLogEntryType.Error);
			}
			finally
			{
				Monitor.Exit(this);
			}

			if (ChangeFromToMilitary)
			{
				Monitor.Enter(this.StationManagerCollection);
				try
				{
					foreach (StationManagerClass StationManager in this.StationManagerCollection)
					{
						StationManager.SyncDateAndTime();
					}
				}
				finally
				{
					Monitor.Exit(this.StationManagerCollection);
				}
			}
		}

		public StationManagerClass CreateStationManager(StationClass Station)
		{
			StationManagerClass StationManager;

			switch (Station.InterfaceType)
			{
				case STATION_INTERFACE_TYPE.MANUAL:
					StationManager = new ManualStationManagerClass(this.EventLog, this.LoadRackManager, Station, this, this.Security);
					break;
				case STATION_INTERFACE_TYPE.ACCULOADIII_Q:
				case STATION_INTERFACE_TYPE.ACCULOADIII_SA:
					StationManager = new AcculoadIIIStationManagerClass(this.EventLog, this.LoadRackManager, Station, this, this.Security);
					break;
				case STATION_INTERFACE_TYPE.PASS_CONTROLLER:
					StationManager = new PASSControllerStationManagerClass(this.EventLog, this.LoadRackManager, Station, this, this.Security);
					break;
				case STATION_INTERFACE_TYPE.VAREC_DET:
					StationManager = new VarecDETStationManagerClass(this.EventLog, this.LoadRackManager, Station, this, this.Security);
					break;
				case STATION_INTERFACE_TYPE.PROXIMITY_CARD_READER:
					StationManager = new ProximityCardReaderStationManagerClass(this.EventLog, this.LoadRackManager, Station, this, this.Security);
					break;
				case STATION_INTERFACE_TYPE.MICROLOAD_NET:
					StationManager = new MicroloadNetStationManager(this.EventLog, this.LoadRackManager, Station, this, this.Security);
					break;
				case STATION_INTERFACE_TYPE.DANLOAD6000:
					StationManager = new Danload6000StationManagerClass(this.EventLog, this.LoadRackManager, Station, this, this.Security);
					break;
				case STATION_INTERFACE_TYPE.MULTILOAD_II_SMP:
					StationManager = new MultiloadIISMPStationManagerClass(this.EventLog, this.LoadRackManager, Station, this, this.Security);
					break;
				case STATION_INTERFACE_TYPE.SIGNATURE:
					StationManager = new SignatureStationManagerClass(this.EventLog, this.LoadRackManager, Station, this, this.Security);
					break;
				case STATION_INTERFACE_TYPE.METER:
					StationManager = new MeterStationManagerClass(this.EventLog, this.LoadRackManager, Station, this, this.Security);
					break;
				case STATION_INTERFACE_TYPE.CONTREC1010:
					StationManager = new Contrec1010StationManagerClass(this.EventLog, this.LoadRackManager, Station, this, this.Security);
					break;
				case STATION_INTERFACE_TYPE.CONTREC1010_RA:
					StationManager = new Contrec1010RAStationManagerClass(this.EventLog, this.LoadRackManager, Station, this, this.Security);
					break;
				case STATION_INTERFACE_TYPE.MULTILOAD_II:
					StationManager = new MultiloadIIStationManagerClass(this.EventLog, this.LoadRackManager, Station, this, this.Security);
					break;
				case STATION_INTERFACE_TYPE.RCU_II_RCU:
					StationManager = new RcuIIProtocolStationManagerClass(this.EventLog, this.LoadRackManager, Station, this, this.Security);
					break;
				case STATION_INTERFACE_TYPE.OSDP_CARD_READER:
					StationManager = new OsdpStationManagerClass(this.EventLog, this.LoadRackManager, Station, this, this.Security);
					break;
				default:
					throw new Exception("Unsupported Station Type " + StationClass.InterfaceTypeID(Station.InterfaceType));
			}

			this.StationManagerCollection.Add(StationManager);

			return StationManager;
		}

		public void RemoveSiteTransactionAliasAssignment(Guid targetGuid)
		{
			if (this.Site.InventoryTransactionAliasGuid == targetGuid)
			{
				this.Site.InventoryTransactionAliasGuid = Guid.Empty;
			}

			if (this.Site.AdjustmentTransactionAliasGuid == targetGuid)
			{
				this.Site.AdjustmentTransactionAliasGuid = Guid.Empty;
			}

			foreach (StationManagerClass StationManager in this.StationManagerCollection)
			{
				if (StationManager.Station.IssueByVolumeTransactionAliasGuid == targetGuid)
				{
					StationManager.Station.IssueByVolumeTransactionAliasGuid = Guid.Empty;
				}

				if (StationManager.Station.IssueByWeightTransactionAliasGuid == targetGuid)
				{
					StationManager.Station.IssueByWeightTransactionAliasGuid = Guid.Empty;
				}

				if (StationManager.Station.ReceiptByVolumeTransactionAliasGuid == targetGuid)
				{
					StationManager.Station.ReceiptByVolumeTransactionAliasGuid = Guid.Empty;
				}

				if (StationManager.Station.ReceiptByWeightTransactionAliasGuid == targetGuid)
				{
					StationManager.Station.ReceiptByWeightTransactionAliasGuid = Guid.Empty;
				}
			}
		}

		public void RemoveTankManagerAssignment(Guid identityGuid)
		{
			foreach (TankClass Tank in this.TankCollection)
			{
				if (Tank.ManagerGuid == identityGuid)
				{
					Tank.ManagerGuid = Guid.Empty;
				}
			}
		}

		public void InitializeSite()
		{
			foreach (ProcessVariableClass ProcessVariable in this.Site.ProcessVariableCollection)
			{
				if (ProcessVariable.ProcessVariableType == PROCESS_VARIABLE_TYPE.SITE_ALARM_OUTPUT_PV)
				{
					this.AlarmOutputPV = ProcessVariable;
				}
				else if (ProcessVariable.ProcessVariableType == PROCESS_VARIABLE_TYPE.SITE_WATCHDOG_OUTPUT_PV)
				{
					this.WatchdogOutputPV = ProcessVariable;
				}
				else if (ProcessVariable.ProcessVariableType == PROCESS_VARIABLE_TYPE.VRU_SETPOINT_PV)
				{
					this.VRUSetpointPV = ProcessVariable;
				}
				else if (ProcessVariable.ProcessVariableType == PROCESS_VARIABLE_TYPE.VRU_DEADBAND_PV)
				{
					this.VRUDeadbandPV = ProcessVariable;
				}
				else if (ProcessVariable.ProcessVariableType == PROCESS_VARIABLE_TYPE.SITE_PERMISSIVE_PV)
				{
					this.OPCServerManager.AddProcessVariable(ProcessVariable);
				}
			}
		}

		public void UninitializeSite()
		{
			this.AlarmOutputPV = null;
			this.WatchdogOutputPV = null;
			this.VRUSetpointPV = null;
			this.VRUDeadbandPV = null;

			foreach (ProcessVariableClass processVariable in this.Site.ProcessVariableCollection)
			{
				if (processVariable.ProcessVariableType == PROCESS_VARIABLE_TYPE.SITE_PERMISSIVE_PV) this.OPCServerManager.RemoveProcessVariable(processVariable);
			}
		}

		void OnInvoke(ProcessVariableClass pv)
		{
			Monitor.Enter(this);

			try
			{
				if (pv == null)
				{
					throw new ArgumentNullException(nameof(pv));
				}

				if (!pv.IsQualityGood
				|| (pv.ServerValue is bool v
						&& !v))
				{
					FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
																	 x =>
																	 x.Add(this.Security, pv.PermissiveLostAlarm)
																);
				}
				else
				{
					FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
																	 x =>
																	 x.Add(this.Security, pv.PermissiveRestoredEvent)
																);
				}

				this.PermissiveEvent.Set();
			}
			catch (Exception e)
			{
				this.EventLog.WriteEntry(e.ToString(), EventLogEntryType.Error);
			}

			finally
			{
				Monitor.Exit(this);
			}
		}

		public TankClass GetTank(Guid tankGuid)
		{
			foreach (TankClass tank in this.TankCollection)
			{
				if (tank.IdentityGuid != tankGuid)
				{
					continue;
				}

				return tank;
			}

			return null;
		}

		public TankClass GetTank(ProductMapClass productMap, CompanyClass manager)
		{
			// Tanks refer back to the product via the master record guid, not the assigned entity guid.
			Guid productMasterGuid =
				 FMChannelHelper.MakeCall<IProducts, Guid>(x => x.GetMasterRecordGuid(this.Security, productMap.AssignedGuid));

			if (productMap.Type == PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANK_MAP
				|| productMap.Type == PRODUCT_MAP_TYPE.PRESET_INJECTOR_MAP
				|| productMap.Type == PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_MAP
				|| productMap.Type == PRODUCT_MAP_TYPE.PRESET_FLOW_CONTROLLED_ADDITIVE_MAP
				|| productMap.Type == PRODUCT_MAP_TYPE.OFFLOAD_EXTERNAL_METER_MAP)
			{
				foreach (TankClass tank in this.TankCollection)
				{
					if (tank.IdentityGuid != productMap.TankOrGroupGuid
					|| tank.ProductGuid != productMasterGuid)
					{
						continue;
					}

					return tank;
				}
			}
			// Get the Market Tank
			else if (productMap.Type == PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANKGROUP_MAP
					 || productMap.Type == PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_TANKGROUP_MAP)
			{
				if (productMap.TankOrGroupGuid != Guid.Empty)
				{
					foreach (TankGroupClass tankGroup in this.TankGroupCollection)
					{
						if (tankGroup.IdentityGuid != productMap.TankOrGroupGuid)
						{
							continue;
						}

						if (tankGroup.ProductGuid != productMasterGuid)
						{
							break;
						}

						foreach (TankMapClass tankMap in tankGroup.TankMapCollection)
						{
							foreach (TankClass tank in this.TankCollection)
							{
								if (manager != null && manager.MasterRecordGuid != tank.ManagerGuid)
								{
									// If we have a manager object passed in (we generally will), ensure that the manager 
									// matches the manager on the potential market tank.  If they don't match, skip and go to next tank
									continue;
								}

								if (tank.IdentityGuid == tankMap.TankGuid)
								{
									ProcessVariableClass tankOperationPv = tank.ProcessVariableCollection[PROCESS_VARIABLE_TYPE.TANK_OPERATION_PV];
									if (tankOperationPv != null && (tankOperationPv.IsQualityGood || this.Site.UseLastKnownGoodTankData)
										 && (tankOperationPv.ServerValue as string == "Market"
											  || (tankOperationPv.ServerValue is bool boolServerValue && boolServerValue)))
									{
										return tank;
									}

									break;
								}
							}
						}

						break;
					}
				}
			}

			return null;
		}

		void InitializeTanks()
		{
			TankCollectionClass tankCollection = FMChannelHelper.MakeCall<ITanks, TankCollectionClass>(
																	 x =>
																	 x.Enumerate(this.Security)
																);

			foreach (TankClass tank in tankCollection)
			{
				tank.Load(this.GetTanks(this.Security, tank.IdentityGuid));
				this.AddTank(tank);
			}
		}

		private object GetTanks(SecurityClass securityParam, Guid guid)
		{
			return FMChannelHelper.MakeCall<ITanks, TankClass>(
																	 x =>
																	 x.Get(securityParam, guid)
																);
		}

		void UnintializeTanks()
		{
			while (this.TankCollection.Count != 0) this.PurgeTank(this.TankCollection[0].IdentityGuid);
		}

		//void InitializeEquipment()
		//{
		//	this.EquipmentCollection.Clear();

		//	EquipmentCollectionClass tempEquipmentCollection = FMChannelHelper.MakeCall<IEquipments, EquipmentCollectionClass>(
		//															 x =>
		//															 x.EnumerateExt(this.Security, Guid.Empty, false, true, EQUIPMENT_TYPE.MAX_EQUIPMENT_TYPE, null, 0)
		//														);

		//	foreach (EquipmentClass equipment in tempEquipmentCollection) this.EquipmentCollection.Add(this.GetEquipments(this.security, equipment.IdentityGuid));
		//}

		//private EquipmentClass GetEquipments(SecurityClass security, Guid guid)
		//{
		//	return FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(
		//															 x =>
		//															 x.Get(security, guid)
		//														);

		//}

		void AddEquipment(EquipmentClass equipment)
		{
			if (equipment.SecondaryStorageFlag)
			{
				Monitor.Enter(this.EquipmentCollection);
				try
				{
					this.EquipmentCollection.Add(equipment);
				}
				finally
				{
					Monitor.Exit(this.EquipmentCollection);
				}
			}
		}

		void AddTank(TankClass Tank)
		{
			Monitor.Enter(this.TankCollection);
			try
			{
				this.TankCollection.Add(Tank);

				foreach (ProcessVariableClass ProcessVariable in Tank.ProcessVariableCollection)
				{
					if (ProcessVariable.OPCItemID == string.Empty)
					{
						continue;
					}

					int Count = 0;
					foreach (ProcessVariableCollectionClass ProcessVariableCollection in this.ProcessVariableCollectionCollection)
					{
						if (ProcessVariableCollection[0].OPCConnectionGuid == ProcessVariable.OPCConnectionGuid)
						{
							ProcessVariableCollection.Add(ProcessVariable);
							break;
						}
						Count++;
					}

					if (Count == this.ProcessVariableCollectionCollection.Count)
					{
						ProcessVariableCollectionClass ProcessVariableCollection = new ProcessVariableCollectionClass
						{
							ProcessVariable
						};
						this.ProcessVariableCollectionCollection.Add(ProcessVariableCollection);
					}
				}
			}
			finally
			{
				Monitor.Exit(this.TankCollection);
			}
		}

		void PurgeTank(Guid identityGuid)
		{
			Monitor.Enter(this.TankCollection);
			try
			{
				foreach (TankClass Tank in this.TankCollection)
				{
					if (Tank.IdentityGuid == identityGuid)
					{
						foreach (ProcessVariableClass ProcessVariable in Tank.ProcessVariableCollection)
						{
							if (ProcessVariable.OPCItemID == string.Empty)
							{
								continue;
							}

							foreach (ProcessVariableCollectionClass ProcessVariableCollection in this.ProcessVariableCollectionCollection)
							{
								if (ProcessVariableCollection[0].OPCConnectionGuid == ProcessVariable.OPCConnectionGuid)
								{
									ProcessVariableCollection.Remove(ProcessVariable);
									if (ProcessVariableCollection.Count == 0)
									{
										this.ProcessVariableCollectionCollection.Remove(ProcessVariableCollection);
									}
									break;
								}
							}
						}

						this.TankCollection.Remove(Tank);

						break;
					}
				}
			}
			finally
			{
				Monitor.Exit(this.TankCollection);
			}
		}

		void TanksScan()
		{
			if (this.Site.InhibitTankScan)
			{
				return;
			}

			Monitor.Enter(this.TankCollection);

			try
			{
				for (int Collection = 0; Collection < this.ProcessVariableCollectionCollection.Count; Collection++)
				{
					var processVariableCollection = this.ProcessVariableCollectionCollection.Item(Collection);

					// IM/SCADA Process Variables
					if (processVariableCollection[0].OPCConnectionGuid == Guid.Empty)
					{
						var pointValueIdentifierList = new List<PointValueIdentifier>(processVariableCollection.Count);

						foreach (ProcessVariableClass processVariable in processVariableCollection)
						{
							pointValueIdentifierList.Add(new PointValueIdentifier(new Guid(processVariable.OPCItemID), PointValueType.Tag, string.Empty));
						}

						try
						{
							var pointValueList = FMChannelHelper.MakeCall<IPointServiceManager, List<PointValue>>(
																					 x =>
																					 x.GetPointValueData(this.Security, pointValueIdentifierList, false)
																				);

							if (pointValueList != null)
							{
								int index = 0;
								foreach (var pointValue in pointValueList)
								{
									var processVariable = processVariableCollection[index];
									bool Changed = false;
									var modifyType = DATA_TYPE.DYNAMIC;

									try
									{
										if (pointValue.IsGood())
										{
											if (pointValue.OpcStatusCodeBits == StatusCodes.GoodLocalOverride
											&& processVariable.OPCQuality != (short)qualityBits.goodLocalOverride)
											{
												processVariable.OPCQuality = (short)qualityBits.goodLocalOverride;
												Changed = true;
											}
											else if (processVariable.OPCQuality != (short)qualityBits.good)
											{
												processVariable.OPCQuality = (short)qualityBits.good;
												Changed = true;
											}
										}
										else if (pointValue.IsUncertain())
										{
											if (processVariable.OPCQuality != (short)qualityBits.uncertain)
											{
												processVariable.OPCQuality = (short)qualityBits.uncertain;
												Changed = true;
											}
										}
										else if (processVariable.OPCQuality != (short)qualityBits.bad)
										{
											processVariable.OPCQuality = (short)qualityBits.bad;
											Changed = true;
										}


										if (pointValue.Units != processVariable.ServerUnits)
										{
											processVariable.ServerUnits = pointValue.Units;
											modifyType = DATA_TYPE.CONFIG;
											Changed = true;
										}

										var minimum = processVariable.GetMinimum(processVariable.ServerUnits, pointValue.DecimalPlaces);
										if (!minimum.Equals(pointValue.Minimum))
										{
											processVariable.SetMinimum(pointValue.Minimum, processVariable.ServerUnits);
											modifyType = DATA_TYPE.CONFIG;
											Changed = true;
										}

										var maximum = processVariable.GetMaximum(processVariable.ServerUnits, pointValue.DecimalPlaces);
										if (!maximum.Equals(pointValue.Maximum))
										{
											processVariable.SetMaximum(pointValue.Maximum, processVariable.ServerUnits);
											modifyType = DATA_TYPE.CONFIG;
											Changed = true;
										}

										if (pointValue.IsGood()
										|| pointValue.IsUncertain())
										{
											object value;

											if (pointValue.Value is Enum)
											{
												value = pointValue.Value.ToString();
											}
											else
											{
												value = pointValue.Value;
											}

											if (processVariable.ServerValue == null
											|| !processVariable.ServerValue.Equals(value))
											{
												processVariable.ServerValue = value;
												Changed = true;
											}
										}

										processVariable.DateTimeStamp = pointValue.ServerTimeStamp;
									}
									catch(Exception)
									{
										if (processVariable.OPCQuality != (short)qualityBits.bad)
										{
											processVariable.OPCQuality = (short)qualityBits.bad;
											Changed = true;
										}
									}

									if (Changed)
									{
										this.ModifyProcessVariables(this.Security, modifyType, processVariable);
									}

									index++;
								}
							}
						}
						catch (Exception e1)
						{
							this.EventLog.WriteEntry(e1.ToString(), EventLogEntryType.Error);
						}
					}
					// OPC DA Process Variables
					else
					{
						Item[] Items = new Item[processVariableCollection.Count];
						int Index = 0;
						foreach (ProcessVariableClass ProcessVariable in processVariableCollection)
						{
							Items[Index] = new Item(new ItemIdentifier(ProcessVariable.OPCItemID))
							{
								ClientHandle = ProcessVariable,
								Active = false
							};
							Index++;
						}

						try
						{
							// Create Server
							Opc.Da.Server Server = new Opc.Da.Server(new OpcCom.Factory(), new URL(processVariableCollection[0].URL));
							NetworkCredential Credentials = null;
							Server.Connect(new ConnectData(Credentials));

							// Read Items and update Database
							ItemValueResult[] Values = Server.Read(Items);
							Index = 0;
							foreach (Item Item in Items)
							{
								ProcessVariableClass ProcessVariable = (ProcessVariableClass)Item.ClientHandle;

								try
								{
									bool Changed = false;
									if (Values[Index].ResultID == ResultID.S_OK)
									{
										if (ProcessVariable.OPCQuality != Values[Index].Quality.GetCode())
										{
											ProcessVariable.OPCQuality = Values[Index].Quality.GetCode();
											Changed = true;
										}

										if (Values[Index].Quality.QualityBits == qualityBits.good
										|| Values[Index].Quality.QualityBits == qualityBits.goodLocalOverride)
										{

											if (ProcessVariable.ServerValue == null
											|| !ProcessVariable.ServerValue.Equals(Values[Index].Value))
											{
												ProcessVariable.ServerValue = Values[Index].Value;
												Changed = true;
											}
										}

										ProcessVariable.DateTimeStamp = Values[Index].Timestamp;
									}
									else
									{
										if (ProcessVariable.OPCQuality != Quality.Bad.GetCode())
										{
											ProcessVariable.OPCQuality = Quality.Bad.GetCode();
											Changed = true;
										}
										ProcessVariable.DateTimeStamp = DateTimeOffset.Now;
									}

									if (Changed)
									{
										this.ModifyProcessVariables(this.Security, DATA_TYPE.DYNAMIC, ProcessVariable);
									}
								}
								catch (Exception e3)
								{
									this.EventLog.WriteEntry("Process Variable Update Error : " + ProcessVariable.OPCItemID + " " + e3.ToString(), EventLogEntryType.Error);
									if (ProcessVariable.OPCQuality != Quality.Bad.GetCode())
									{
										ProcessVariable.OPCQuality = Quality.Bad.GetCode();
										ProcessVariable.DateTimeStamp = DateTimeOffset.Now;
										try
										{
											this.ModifyProcessVariables(this.Security, DATA_TYPE.DYNAMIC, ProcessVariable);
										}
										catch (Exception e4)
										{
											this.EventLog.WriteEntry(e4.ToString(), EventLogEntryType.Error);
										}
									}
								}

								Index++;
							}

							// Destroy Server
							Server.Disconnect();
							Server.Dispose();
						}
						catch
						{
							foreach (Item Item in Items)
							{
								ProcessVariableClass PV = (ProcessVariableClass)Item.ClientHandle;
								if (PV.OPCQuality != Quality.Bad.GetCode())
								{
									PV.OPCQuality = Quality.Bad.GetCode();
									PV.DateTimeStamp = DateTimeOffset.Now;
									try
									{
										this.ModifyProcessVariables(this.Security, DATA_TYPE.DYNAMIC, PV);
									}
									catch (Exception e2)
									{
										this.EventLog.WriteEntry(e2.ToString(), EventLogEntryType.Error);
									}
								}
							}
						}
					}
				}
			}
			catch (Exception e1)
			{
				this.EventLog.WriteEntry(e1.ToString(), EventLogEntryType.Error);
			}
			finally
			{
				Monitor.Exit(this.TankCollection);
			}
		}

		private void ModifyProcessVariables(SecurityClass securityParam, DATA_TYPE dataType, ProcessVariableClass processVariable)
		{
			FMChannelHelper.MakeCall<IProcessVariables>(
																	 x =>
																	 x.Modify(securityParam, dataType, processVariable)
																);
		}

		void InitializeTankGroups()
		{
			TankGroupCollectionClass tankGroupCollection = FMChannelHelper.MakeCall<ITankGroups, TankGroupCollectionClass>(
																	 x =>
																	 x.Enumerate(this.Security)
																);

			foreach (TankGroupClass tankGroup in tankGroupCollection)
			{
				tankGroup.Load(this.GetTankGroups(this.Security, tankGroup.IdentityGuid));
				this.AddTankGroup(tankGroup);
			}
		}

		private object GetTankGroups(SecurityClass securityParam, Guid guid)
		{
			return FMChannelHelper.MakeCall<ITankGroups, TankGroupClass>(
																	 x =>
																	 x.Get(securityParam, guid)
																);
		}

		void UnintializeTankGroups()
		{
			while (this.TankGroupCollection.Count != 0) this.PurgeTankGroup(this.TankGroupCollection[0].IdentityGuid);
		}

		void AddTankGroup(TankGroupClass tankGroup)
		{
			Monitor.Enter(this.TankGroupCollection);
			try
			{
				this.TankGroupCollection.Add(tankGroup);
			}
			finally
			{
				Monitor.Exit(this.TankGroupCollection);
			}
		}

		//void ModifyTankGroup(TankGroupClass TankGroup)
		//{
		//	this.PurgeTankGroup(TankGroup.IdentityGuid);
		//	this.AddTankGroup(TankGroup);
		//}

		void PurgeEquipment(Guid identityGuid)
		{
			Monitor.Enter(this.EquipmentCollection);
			try
			{
				for (int item = 0; item < this.EquipmentCollection.Count; item++)
				{
					if (this.EquipmentCollection[item].IdentityGuid == identityGuid)
					{
						this.EquipmentCollection.RemoveAt(item);
						break;
					}
				}
			}
			finally
			{
				Monitor.Exit(this.EquipmentCollection);
			}
		}

		void PurgeTankGroup(Guid identityGuid)
		{
			Monitor.Enter(this.TankGroupCollection);
			try
			{
				foreach (TankGroupClass TankGroup in this.TankGroupCollection)
				{
					if (TankGroup.IdentityGuid == identityGuid)
					{
						this.TankGroupCollection.Remove(TankGroup);

						break;
					}
				}
			}
			finally
			{
				Monitor.Exit(this.TankGroupCollection);
			}
		}

		void InitializeStations()
		{
			StationCollectionClass StationCollection = FMChannelHelper.MakeCall<IStations, StationCollectionClass>(
																	 x =>
																	 x.Enumerate(this.Security)
																);

			for (int Item = 0; Item < StationCollection.Count; Item++)
			{
				StationClass Station = this.GetStations(this.Security, StationCollection[Item].IdentityGuid);

				if (!Station.Enabled)
				{
					continue;
				}

				try
				{
					this.CreateStationManager(Station);
				}
				catch (Exception e)
				{
					this.EventLog.WriteEntry(e.ToString(), EventLogEntryType.Error);
				}
			}
		}

		private StationClass GetStations(SecurityClass Security, Guid guid)
		{
			return FMChannelHelper.MakeCall<IStations, StationClass>(
																	 x =>
																	 x.Get(Security, guid)
																);
		}

		public bool LoadingInProgress()
		{
			foreach (StationManagerClass StationManager in this.StationManagerCollection)
			{
				if (StationManager.LoadingInProgress())
				{
					return true;
				}
			}

			return false;
		}

		void UninitializeStations()
		{
			while (this.StationManagerCollection.Count != 0)
			{
				this.StationManagerCollection.Item(0).Dispose();
				this.StationManagerCollection.Remove(0);
			}
		}

		void AddStation(StationClass Station)
		{
			Monitor.Enter(this.StationManagerCollection);

			try
			{
				StationManagerClass StationManager = this.StationManagerCollection.FindByStationIdentityGuid(Station.IdentityGuid);
				if (StationManager != null)
				{
					this.PurgeStation(Station.IdentityGuid);
					if (StationManager.DeferredPurge)
					{
						StationManager.DeferredAdd = true;
						return;
					}
				}

				ArrayList StationList = new ArrayList();

				// Any Stations that had common swing arms also must be reloaded
				foreach (LoadArmClass LoadArm in Station.LoadArmCollection)
				{
					if (LoadArm.BayAStationGuid != Station.IdentityGuid
					&& LoadArm.BayAStationGuid != Guid.Empty)
					{
						if (!StationList.Contains(LoadArm.BayAStationGuid))
						{
							StationList.Add(LoadArm.BayAStationGuid);
						}
					}

					if (LoadArm.BayBStationGuid != Station.IdentityGuid
						&& LoadArm.BayBStationGuid != Guid.Empty)
					{
						if (!StationList.Contains(LoadArm.BayBStationGuid))
						{
							StationList.Add(LoadArm.BayBStationGuid);
						}
					}
				}

				// If any associated stations cannot be reloaded then loading of this
				// station will be deferred until one of the associated stations is reloaded
				bool DeferredAdd = false;

				if (StationList.Count > 0)
				{
					foreach (Guid stationGuid in StationList)
					{
						StationManager = this.StationManagerCollection.FindByStationIdentityGuid(stationGuid);
						if (StationManager != null)
						{
							if (StationManager.DeferredPurge)
							{
								StationManager.DeferredAdd = true;
								DeferredAdd = true;
								continue;
							}

							this.PurgeStation(StationManager.Station.IdentityGuid);
							if (StationManager.DeferredPurge)
							{
								StationManager.DeferredAdd = true;
								DeferredAdd = true;
								continue;
							}
						}

						StationClass AssociatedStation = this.GetStations(this.Security, stationGuid);
						if (AssociatedStation.Enabled)
						{
							this.CreateStationManager(AssociatedStation);
						}
					}
				}

				if (!DeferredAdd
					&& Station.Enabled)
				{
					this.CreateStationManager(Station);
				}
			}
			finally
			{
				Monitor.Exit(this.StationManagerCollection);
			}
		}

		void PurgeStation(Guid identityGuid)
		{
			Monitor.Enter(this.StationManagerCollection);
			try
			{
				StationManagerClass StationManager = this.StationManagerCollection.FindByStationIdentityGuid(identityGuid);
				if (StationManager == null)
					return;

				if (StationManager.StationState != StationState.IDLE
				&& StationManager.StationState != StationState.ENTER_DRIVER_ID_PROMPT)
				{
					StationManager.DeferredPurge = true;
					return;
				}

				// If Station has SwingArms need to reload associated stations
				foreach (LoadArmManagerClass LoadArmManager in StationManager.LoadArmManagerCollection)
				{
					if (LoadArmManager.LoadArm.SwingArm)
					{
						if (LoadArmManager.BayA.StationManager == StationManager)
						{
							if (LoadArmManager.BayB.StationManager != null)
							{
								LoadArmManager.BayB.StationManager.DeferredPurge = true;
								LoadArmManager.BayB.StationManager.DeferredAdd = true;
								this.PermissiveEvent.Set();
							}
						}
						else
						{
							if (LoadArmManager.BayA.StationManager != null)
							{
								LoadArmManager.BayA.StationManager.DeferredPurge = true;
								LoadArmManager.BayA.StationManager.DeferredAdd = true;
								this.PermissiveEvent.Set();
							}
						}
					}
				}

				foreach (LoadArmManagerClass LoadArmManager in StationManager.LoadArmManagerDisabledCollection)
				{
					if (LoadArmManager.LoadArm.SwingArm)
					{
						if (LoadArmManager.BayA.StationManager == StationManager)
						{
							if (LoadArmManager.BayB.StationManager != null)
							{
								LoadArmManager.BayB.StationManager.DeferredPurge = true;
								LoadArmManager.BayB.StationManager.DeferredAdd = true;
								this.PermissiveEvent.Set();
							}
						}
						else
						{
							if (LoadArmManager.BayA.StationManager != null)
							{
								LoadArmManager.BayA.StationManager.DeferredPurge = true;
								LoadArmManager.BayA.StationManager.DeferredAdd = true;
								this.PermissiveEvent.Set();
							}
						}
					}
				}

				StationManager.Dispose();
				this.StationManagerCollection.Remove(StationManager);
			}
			finally
			{
				Monitor.Exit(this.StationManagerCollection);
			}
		}

		protected bool SitePermissive
		{
			get
			{
				Monitor.Enter(this);

				try
				{
					bool SitePermissive = true;

					foreach (ProcessVariableClass ProcessVariable in this.Site.ProcessVariableCollection)
					{
						if (ProcessVariable.ProcessVariableType != PROCESS_VARIABLE_TYPE.SITE_PERMISSIVE_PV)
						{
							continue;
						}

						if (ProcessVariable.URL == "")
						{
							continue;
						}

						if (!ProcessVariable.IsQualityGood)
						{
							SitePermissive = false;
							break;
						}

						if (System.Convert.ToBoolean(ProcessVariable.ServerValue) != true)
						{
							SitePermissive = false;
							break;
						}
					}
					return SitePermissive;
				}
				finally
				{
					Monitor.Exit(this);
				}
			}
		}

		protected void UpdateAlarmOutput()
		{
			Monitor.Enter(this);

			try
			{
				if (this.AlarmOutputPV != null
				&& this.AlarmOutputPV.URL != "")
				{
					this.AlarmOutputPV.ServerValue = this.SitePermissive;
					this.OPCServerManager.Write(this.AlarmOutputPV);
				}
			}
			finally
			{
				Monitor.Exit(this);
			}
		}

		protected void UpdateWatchdogOutput()
		{
			if (this.WatchdogOutputPV == null)
			{
				return;
			}

			Monitor.Enter(this);

			try
			{
				if (this.WatchdogOutputPV != null
				&& this.WatchdogOutputPV.URL != "")
				{
					if (this.Site.WatchdogMode == WATCHDOG_MODE.TOGGLE)
					{
						if (this.WatchdogOutputPV.ServerValue is bool boolWatchdogValue)
						{
							this.WatchdogOutputPV.ServerValue = boolWatchdogValue == true ? false : (object)true;
						}
						else this.WatchdogOutputPV.ServerValue = false;
					}
					else
					{
						// Van Thompson - The code commented out below was never returning true when checking
						// to see if the ServerValue was an int.  This change is to get that check working.
						try
						{
							int iServerValue = System.Convert.ToInt32(this.WatchdogOutputPV.ServerValue.ToString());
							if (iServerValue++ < this.Site._WatchdogCounterEnd)
							{
								this.WatchdogOutputPV.ServerValue = iServerValue;
							}
							else
							{
								this.WatchdogOutputPV.ServerValue = this.Site._WatchdogCounterStart;
							}
						}
						catch
						{
							this.WatchdogOutputPV.ServerValue = this.Site.WatchdogCounterStart;
						}
					}

					this.OPCServerManager.Write(this.WatchdogOutputPV);
				}
			}
			finally
			{
				Monitor.Exit(this);
			}
		}

		protected void UpdateStationPermissive()
		{
			this.StationPermissive = true;

			if (this.EndOfDayState >= StateEndOfDay.WaitingForLoadingToStop
			|| this.EndOfMonthState >= StateEndOfMonth.WaitingForLoadingToStop)
			{
				this.StationPermissive = false;
			}

			if (this.StationPermissive)
			{
				this.StationPermissive = this.SitePermissive;
			}

			if (this.StationPermissive)
			{
				if (this.ActiveArms >= this.Site._MaximumNumberOfActiveArms)
				{
					this.StationPermissive = false;
				}
			}

			Monitor.Enter(this.StationManagerCollection);

			try
			{
				foreach (StationManagerClass StationManager in this.StationManagerCollection)
				{
					if (StationManager.Station.Type != STATION_TYPE.LOAD_RACK)
					{
						continue;
					}

					StationManager.UpdateStationPermissives(this.StationPermissive);
				}
			}
			finally
			{
				Monitor.Exit(this.StationManagerCollection);
			}
		}

		private void Closeout()
		{
			if (this.Site.InhibitAutomaticCloseout)
			{
				return;
			}

			CloseoutSiteSR closeoutSiteSR = new CloseoutSiteSR();
			closeoutSiteSR.Security = this.Security;
			closeoutSiteSR.Site = this.Site.ID;

			FMChannelHelper.MakeCall<ICloseoutSiteProcessor>(x => x.ProcessForSite(closeoutSiteSR));
		}

		/// <summary>
		/// This method will create adjustment type transactions based on manager and
		/// product collections.
		/// </summary>
		/// <param name="inventoryDate">The inventory date to create the transaction.</param>
		private void CreateAdjustmentDistributionTransactions(DateTime inventoryDate)
		{
			if (this.Site.InhibitAutomaticAdjustmentDistribution)
			{
				return;
			}

			// Retrieve Managers
			CompanyCollectionClass managerCollection = FMChannelHelper.MakeCall<ICompanies, CompanyCollectionClass>(
																	 x =>
																	 x.EnumerateByRole(this.Security, COMPANY_ROLE.MANAGER, false, true)
																);
			// Retrieve Products
			ProductCollectionClass productCollection = FMChannelHelper.MakeCall<IProducts, ProductCollectionClass>(
																	 x =>
																	 x.EnumerateByType(this.Security, ProductType.ComponentProduct)
																);
			// Retrieve the General Configuration
			var generalConfigSR = new GeneralConfigSR
			{
				Security = this.Security,
				Request = GeneralConfigSR.GeneralConfigurationRequests.GET_CONFIGURATION
			};

			GeneralConfigDO generalConfigDO = FMChannelHelper.MakeCall<IGeneralConfigProcessor, GeneralConfigDO>(
																	 x =>
																	 x.Get(generalConfigSR)
																);
			AccountingSite accountingSite = FMChannelHelper.MakeCall<IAccountingSites, AccountingSite>(
																	 x =>
																	 x.LoadSiteInfo(this.Security, this.Security.SiteGuid)
																);

			TransactionAliasClass adjustmentTransactionAlias = FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasClass>(
																	 x =>
																	 x.Get(this.Security, this.Site.AdjustmentTransactionAliasGuid, false)
																);

			var unitsHelper = new UnitsHelperClass(this.Security, this.Site, adjustmentTransactionAlias, null);

			foreach (CompanyClass manager in managerCollection)
			{
				foreach (ProductClass product in productCollection)
				{
					// Retrieve the owner info from inventory list
					var adjustmentDistSR = new AdjustmentDistributionSR
					{
						Security = this.Security,
						Subrequest = AdjustmentDistributionSR.RequestTypes.GET_OWNERS,
						ManagerID = manager.ID,
						ProductID = product.ID,
						IsConsortium = generalConfigDO.UseConsortium
					};

					adjustmentDistSR.AffectsInventoryAliasList.Clear();
					adjustmentDistSR.AffectsInventoryAliasList.AddRange(generalConfigDO.AdjustmentAliasList);

					AdjustmentDistributionDO adjustDistDO = this.ProcessAdjustmentDistributionDo(adjustmentDistSR);

					// Retrieve adjustment distribution for automatic adjustments.
					AdjustmentDistributorClass adjustDistributor = null;
					switch (generalConfigDO.AdjustmentMethod)
					{
						case GeneralConfigSR.GeneralConfigAdjustMethod.ALLOCATION:
							{
								adjustDistributor = new AdjustmentDistributorClass(AdjustmentDistributionConfigurationDO.AdjustmentDistributionMethods.ALLOCATION)
								{
									AdjustmentMethod = AdjustmentDistributionConfigurationDO.AdjustmentDistributionMethods.ALLOCATION,
									OwnerList = adjustDistDO.OwnerList
								};
								adjustDistributor.PerformDistribution(accountingSite);
								break;
							}

						// The throughput case will retrieve transactions for a date range, manager, and product.
						// Once the data is retrieve, then the adjustment distribution is calculated.
						case GeneralConfigSR.GeneralConfigAdjustMethod.THROUGHPUT:
							{
								adjustmentDistSR.Subrequest = AdjustmentDistributionSR.RequestTypes.GET_TRANSACTIONS;

								adjustmentDistSR.InventoryDate = inventoryDate.Date;

								adjustDistDO = this.ProcessAdjustmentDistributionDo(adjustmentDistSR);

								adjustDistributor = new AdjustmentDistributorClass(AdjustmentDistributionConfigurationDO.AdjustmentDistributionMethods.THROUGHPUT)
								{
									AdjustmentMethod = AdjustmentDistributionConfigurationDO.AdjustmentDistributionMethods.THROUGHPUT,
									OwnerList = adjustDistDO.OwnerList,
									TransactionList = adjustDistDO.TransactionList
								};

								try
								{
									adjustDistributor.PerformDistribution(accountingSite);
								}
								catch (Exception exp)
								{
									string invalidCalculation = "Invalid calculation in throughput! " + exp;
									this.EventLog.WriteEntry(invalidCalculation, EventLogEntryType.Error);
								}
								break;
							}
					}

					if (adjustDistributor != null && adjustDistributor.OwnerAdjustmentDistributions.Count != 0)
					{
						var saveTransactionsSR = new SaveTransactionsSR
						{
							UseAutoComplete = true,
							ConvertUnits = true,
							Security = this.Security,
							CurrentSiteGuid = this.Security.SiteGuid,
							BOLFromLoadRackFlag = true
						};

						foreach (AdjustmentOwnerRecord adjustmentPerOwner in adjustDistributor.OwnerAdjustmentDistributions)
						{
							var transaction = new TransactionDO
							{
								Alias = this.Site.AdjustmentTransactionAliasID,
								TransTypeID = TransactionTypes.T1_PrimaryAdjustment,
								TransactionDateTime = TimeConverter.Now(this.Site),
								InventoryDate = inventoryDate.Date,
								Site = this.Site.ID,
								ManagerID = manager.ID,
								OwnerID = adjustmentPerOwner.OwnerName,
								OriginApplication = TransactionOrigin.TerminalAutomationService
							};

							var lineItem = new LineItemDO { Product = product.ID, ProductGuid = product.IdentityGuid };
							unitsHelper.SetUnits(lineItem, ProductType.ComponentProduct, product);
							lineItem.Quantity.Gross = adjustmentPerOwner.GrossValue;
							lineItem.Quantity.Net = adjustmentPerOwner.GrossValue;

							transaction.LineItems.Add(lineItem);
							saveTransactionsSR.Transactions.Add(transaction);
						}

						FMChannelHelper.MakeCall<ISaveTransactionsProcessor, SaveTransactionsResultDO>(
																	 x =>
																	 x.SaveTransactions(saveTransactionsSR)
																);
					}
				}
			}
		}

		private AdjustmentDistributionDO ProcessAdjustmentDistributionDo(AdjustmentDistributionSR adjustmentDistSR)
		{
			return FMChannelHelper.MakeCall<IAdjustmentDistributionProcessor, AdjustmentDistributionDO>(
																	 x =>
																	 x.Process(adjustmentDistSR)
																);
		}

		private void PrintReports(ReportConfigurationDetailSR.RequestTypes Type, DateTimeOffset InventoryDate)
		{
			if (this.Site.InhibitAutomaticReportGeneration)
			{
				return;
			}

			ReportConfigurationDetailSR reportDetailSR = new ReportConfigurationDetailSR
			{
				CurrentSiteGuid = this.Site.IdentityGuid,
				Site = this.Site.ID,
				RequestType = Type
			};

			DataObject dataObj = FMChannelHelper.MakeCall<IReportConfigurationDetailProcessor, DataObject>(
																	 x =>
																	 x.GetAll(reportDetailSR)
																);

			if (typeof(ErrorObject).IsInstanceOfType(dataObj))
			{
				ErrorObject errorObj = (ErrorObject)dataObj;

				if (errorObj.HasErrors == true)
				{
					this.EventLog.WriteEntry(errorObj.ErrorMessage);
				}
				return;
			}

			// All reports processed by EndOfDay and EndOfMonth must have the
			// following parameters.
			//
			// Note: this is compatible with ReportWebApp
			// All reports processed by EndOfDay and EndOfMonth must have the
			// following parameters.
			//
			ReportSvr2005.ParameterValue[] parameterValues = new ReportSvr2005.ParameterValue[4];

			parameterValues[0] = new ReportSvr2005.ParameterValue
			{
				Name = "SiteGuid",
				Value = this.Site.IdentityGuid.ToString()
			};
			parameterValues[1] = new ReportSvr2005.ParameterValue { Name = "InventoryDate", Value = InventoryDate.ToString() };
			parameterValues[2] = new ReportSvr2005.ParameterValue { Name = "FromSiteManager", Value = "True" };
			parameterValues[3] = new ReportSvr2005.ParameterValue
			{
				Name = "EndOfMonth",
				Value = (Type == ReportConfigurationDetailSR.RequestTypes.GET_PRINT_AT_END_OF_MONTH_TYPE) ? "True" : "False"
			};

			ReportConfigurationDetailListDO reportDetailListDO = (ReportConfigurationDetailListDO)dataObj;
			foreach (ReportConfigurationDetailDO reportConfigurationDetailDO in reportDetailListDO.ReportDetailDOList)
			{
				if (reportConfigurationDetailDO.PrimaryPrinterName != "" && reportConfigurationDetailDO.PrimaryPrinterName != "{None}")
				{
					ReportServicePrintService printService = new ReportServicePrintService(this.EventLog)
					{
						ReportingServiceUrl = this.LoadRackManager.SystemSetting.ReportServerUrl,
						ReportName = reportConfigurationDetailDO.ReportDirectory + "/" + reportConfigurationDetailDO.ReportPath,
						PrinterName = reportConfigurationDetailDO.PrimaryPrinterName,
						ParameterValues = parameterValues,
						Security = this.Security
					};

					try
					{
						printService.PrintReport();
					}
					catch (Exception e)
					{
						this.EventLog.WriteEntry(e.ToString(), EventLogEntryType.Error);
					}
				}

				if (reportConfigurationDetailDO.SecondaryPrinterName != "{None}")
				{
					ReportServicePrintService printService = new ReportServicePrintService(this.EventLog)
					{
						ReportingServiceUrl = this.LoadRackManager.SystemSetting.ReportServerUrl,
						ReportName = reportConfigurationDetailDO.ReportDirectory + "/" + reportConfigurationDetailDO.ReportPath,
						PrinterName = reportConfigurationDetailDO.SecondaryPrinterName,
						ParameterValues = parameterValues,
						Security = this.Security
					};

					try
					{
						printService.PrintReport();
					}
					catch (Exception e)
					{
						this.EventLog.WriteEntry(e.ToString(), EventLogEntryType.Error);
					}
				}
			}
		}

		/// <summary>
		/// This method will create a meter type transaction.
		/// </summary>
		/// <param name="inventoryDateTime">The inventory date of the transaction.</param>
		private void CreateMeterReadingTransactions(DateTimeOffset inventoryDateTime)
		{
			if (this.Site.InhibitAutomaticMeterCloseout)
			{
				return;
			}

			// Determine the TransactionAlias
			TransactionAliasCollectionClass transactionAliasCollection = FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasCollectionClass>(
																	 x =>
																	 x.EnumerateByTransTypeID(this.Security, TransactionTypes.T12_InventoryNotAffected)
																);

			TransactionAliasClass meterReadingTransactionAlias = null;

			foreach (TransactionAliasClass transactionAlias in transactionAliasCollection)
			{
				if (!transactionAlias.MeterCloseout)
				{
					continue;
				}

				meterReadingTransactionAlias = transactionAlias;
				break;
			}

			if (meterReadingTransactionAlias == null)
			{
				return;
			}

			var saveTransactionsSR = new SaveTransactionsSR
			{
				UseAutoComplete = true,
				ConvertUnits = true,
				Security = this.Security,
				CurrentSiteGuid = this.Security.SiteGuid,
				BOLFromLoadRackFlag = true
			};

			Monitor.Enter(this.StationManagerCollection);
			try
			{
				foreach (StationManagerClass stationManager in this.StationManagerCollection)
				{
					if ((stationManager.Station.Type != STATION_TYPE.LOAD_RACK) && (stationManager.Station.Type != STATION_TYPE.METER))
					{
						continue;
					}

					Monitor.Enter(stationManager);
					try
					{
						stationManager.CreateMeterReadingTransactions(saveTransactionsSR, meterReadingTransactionAlias, inventoryDateTime);
					}
					finally
					{
						Monitor.Exit(stationManager);
					}
				}
			}
			finally
			{
				Monitor.Exit(this.StationManagerCollection);
			}

			if (saveTransactionsSR.Transactions.Count != 0)
			{
				FMChannelHelper.MakeCall<ISaveTransactionsProcessor, SaveTransactionsResultDO>(
																	 x =>
																	 x.SaveTransactions(saveTransactionsSR)
																);
			}
		}

		public DateTime GetInventoryDate()
		{
			return this.LoadRackManager.GetCurrentInventoryDate(this.Security);
		}

		/// <summary>
		/// This method will create physical inventory transaction based on the tanks in the tank collection.
		/// </summary>
		/// <param name="inventoryDateTime">The inventory date of the transaction.</param>
		private void CreatePhysicalInventoryTransactions(DateTimeOffset inventoryDateTime, AlarmAndEventDescriptorClass endOfDayMonthDescriptor)
		{
			var saveTransactionsSR = new SaveTransactionsSR
			{
				UseAutoComplete = true,
				ConvertUnits = false,
				Security = this.Security,
				CurrentSiteGuid = this.Security.SiteGuid,
				BOLFromLoadRackFlag = true
			};

			Monitor.Enter(this.TankCollection);
			try
			{
				if (this.Site.InhibitAutomaticPhysicalInventory || this.Site.InventoryTransactionAliasGuid.IsEmpty())
				{
					return;
				}

				TransactionAliasClass transactionAlias = FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasClass>(
																	 x =>
																	 x.Get(this.Security, this.Site.InventoryTransactionAliasGuid, false)
																);
				string infoStr = string.Empty;

				foreach (TankClass tank in this.TankCollection)
				{
					if (tank.ProductGuid == Guid.Empty || tank.ManagerGuid == Guid.Empty)
					{
						infoStr = "Create Physical Inventory Transaction for tank '" + tank.ID + "' has no product or manager assigned to this tank.  Will be skipped.";
						FMChannelHelper.MakeCall<IAlarmAndEventLogs>(x => x.Add(this.Security,
																			this.Site.TankIgnoredPhysInvCreateEvent(infoStr)));
						continue;
					}

					TransactionDO physicalTransaction = null;

					if (transactionAlias.MultipleLineItems)
					{
						foreach (TransactionDO transaction in saveTransactionsSR.Transactions)
						{
							if (tank.ManagerGuid != transaction.ManagerCompanyGuid)
							{
								continue;
							}

							if (transaction.LineItems.Count > 0 && tank.ProductGuid != transaction.LineItems[0].ProductGuid)
							{
								continue;
							}

							physicalTransaction = transaction;
							break;
						}
					}

					if (physicalTransaction == null)
					{
						physicalTransaction = new TransactionDO
						{
							Alias					= this.Site.InventoryTransactionAliasID,
							TransactionAliasGuid	= this.Site.InventoryTransactionAliasGuid,
							TransTypeID				= TransactionTypes.T14_PhysicalInventory,
							TransactionDateTime		= DateTimeOffset.Now,
							InventoryDate			= TimeConverter.ToDate(inventoryDateTime).Date,
							Site					= this.Site.ID,
							SiteGuid				= this.Site.IdentityGuid,
							ManagerID				= tank.ManagerID,
							ManagerCode				= tank.ManagerCode,
							ManagerCompanyGuid		= tank.ManagerGuid,
							OriginApplication		= TransactionOrigin.TerminalAutomationService
						};
						saveTransactionsSR.Transactions.Add(physicalTransaction);
					}

					var lineItem = new LineItemDO
					{
						StorageLocationID		= tank.ID,
						StorageLocationTankGuid = tank.IdentityGuid,
						Product					= tank.ProductID,
						ProductCode				= tank.ProductCode,
						ProductGuid				= tank.ProductGuid,
						LineNumber				= physicalTransaction.LineItems.Count + 1
					};

					ProductClass product = this.GetProductsByID(this.Security, tank.ProductID);

					if (product != null)
					{
						lineItem.ProductType = ProductClass.ProductTypeID(product.ProductType);
					}

					//temporary variables used to calculate net capacity and bottom volume
					double availableNetVolume = 0.0;
					double remainingNetVolume = 0.0;
					string errorStr = string.Empty;

					foreach (ProcessVariableClass PV in tank.ProcessVariableCollection)
					{
						// double type variables that are part of transaction
						if (PV.ProcessVariableType == PROCESS_VARIABLE_TYPE.GROSS_VOLUME_PV
							|| PV.ProcessVariableType == PROCESS_VARIABLE_TYPE.NET_VOLUME_PV
							|| PV.ProcessVariableType == PROCESS_VARIABLE_TYPE.TEMPERATURE_PV
							|| PV.ProcessVariableType == PROCESS_VARIABLE_TYPE.STANDARD_DENSITY_PV
							|| PV.ProcessVariableType == PROCESS_VARIABLE_TYPE.VCF_PV
							|| PV.ProcessVariableType == PROCESS_VARIABLE_TYPE.AVAILABLE_NET_VOLUME_PV
							|| PV.ProcessVariableType == PROCESS_VARIABLE_TYPE.REMAINING_NET_VOLUME_PV
							|| PV.ProcessVariableType == PROCESS_VARIABLE_TYPE.MASS_PV)
						{
							if (!PV.IsQualityGood && !PV.IsQualityUncertain && !this.Site.UseLastKnownGoodTankData)
							{
								errorStr = "Create Physical Inventory Transaction failed for tank '" + tank.ID + "'. Error: OPC Quality Bad " + PV.OPCItemID;

								this.EventLog.WriteEntry(errorStr, EventLogEntryType.Error);
								FMChannelHelper.MakeCall<IAlarmAndEventLogs>(x => x.Add(this.Security, 
																			this.Site.EndOfDayMonthFailedPhysInvCreateEvent(errorStr, endOfDayMonthDescriptor)));
								
								lineItem = null;
								break;
							}

							if (!typeof(double).IsInstanceOfType(PV.SIValue))
							{
								errorStr = "Create Physical Inventory Transaction for tank '" + tank.ID 
											+ "'. Process Variable type is not a type of double: " + ProcessVariableClass.ProcessVariableTypeID(PV.ProcessVariableType) 
											+ ".";

								this.EventLog.WriteEntry(errorStr, EventLogEntryType.Error);
								FMChannelHelper.MakeCall<IAlarmAndEventLogs>(x => x.Add(this.Security, 
																			this.Site.EndOfDayMonthFailedPhysInvCreateEvent(errorStr, endOfDayMonthDescriptor)));

								lineItem = null;
								break;
							}
						}
						// string type variables that are part of transaction
						else if (PV.ProcessVariableType == PROCESS_VARIABLE_TYPE.TANK_STATUS_PV)
						{
							if (!PV.IsQualityGood && !PV.IsQualityUncertain && !this.Site.UseLastKnownGoodTankData)
							{
								errorStr = "Create Physical Inventory Transaction for tank '" + tank.ID + "'. Error: OPC Quality Bad " + PV.OPCItemID;

								this.EventLog.WriteEntry( errorStr, EventLogEntryType.Error);
								FMChannelHelper.MakeCall<IAlarmAndEventLogs>(x => x.Add(this.Security, 
																		this.Site.EndOfDayMonthFailedPhysInvCreateEvent(errorStr, endOfDayMonthDescriptor)));

								lineItem = null;
								break;
							}

							if (!typeof(string).IsInstanceOfType(PV.SIValue))
							{
								errorStr = "Create Physical Inventory Transaction for tank '" + tank.ID
											+ "'. Process Variable type is not a type of double: " + ProcessVariableClass.ProcessVariableTypeID(PV.ProcessVariableType)
											+ ".";

								this.EventLog.WriteEntry(errorStr, EventLogEntryType.Error);
								FMChannelHelper.MakeCall<IAlarmAndEventLogs>(x => x.Add(this.Security, this.Site.EndOfDayMonthFailedPhysInvCreateEvent(errorStr, endOfDayMonthDescriptor)));

								lineItem = null;
								break;
							}
						}
						else
						{
							// variables that aren't part of transaction
							continue;
						}

						switch (PV.ProcessVariableType)
						{
							case PROCESS_VARIABLE_TYPE.GROSS_VOLUME_PV:
								lineItem.Quantity.Gross = (double)PV.SIValue;
								break;
							case PROCESS_VARIABLE_TYPE.NET_VOLUME_PV:
								lineItem.Quantity.Net = (double)PV.SIValue;
								break;
							case PROCESS_VARIABLE_TYPE.TEMPERATURE_PV:
								lineItem.Temperature = System.Convert.ToDouble(PV.SIValue);
								break;
							case PROCESS_VARIABLE_TYPE.STANDARD_DENSITY_PV:
								lineItem.Density = System.Convert.ToDouble(PV.SIValue);
								break;
							case PROCESS_VARIABLE_TYPE.VCF_PV:
								lineItem.VCF = System.Convert.ToDouble(PV.SIValue);
								break;
							case PROCESS_VARIABLE_TYPE.AVAILABLE_NET_VOLUME_PV:
								availableNetVolume = (double)PV.SIValue;
								break;
							case PROCESS_VARIABLE_TYPE.REMAINING_NET_VOLUME_PV:
								remainingNetVolume = (double)PV.SIValue;
								break;
							case PROCESS_VARIABLE_TYPE.TANK_STATUS_PV:
								lineItem.TankStatus = PV.SIValue as string;
								break;
							case PROCESS_VARIABLE_TYPE.MASS_PV:
								lineItem.Quantity.Mass = (double)PV.SIValue;
								break;
						}
					}

					if (lineItem != null)
					{
						lineItem.BottomVolume = lineItem.Quantity.Net - availableNetVolume;
						lineItem.NetCapacity = lineItem.Quantity.Net + remainingNetVolume;
						physicalTransaction.LineItems.Add(lineItem);
					}
				}
			}
			finally
			{
				Monitor.Exit(this.TankCollection);
			}

			if (saveTransactionsSR.Transactions.Count != 0)
			{
				try
				{
					FMChannelHelper.MakeCall<ISaveTransactionsProcessor, SaveTransactionsResultDO>(
																	 x =>
																	 x.SaveTransactions(saveTransactionsSR)
																);
				}
				catch (FaultException<SaveTransactionsException> e)
				{
					if (e.Detail.Results.Count >= 1 && typeof(TransactionValidationResult).IsInstanceOfType(e.Detail.Results[0])
						 && e.Detail.Results[0].ErrorList.Count >= 1)
					{
						this.EventLog.WriteEntry(e.Detail.Results[0].ErrorList[0], EventLogEntryType.Error);
					}
					else
					{
						throw new Exception("Unknown SaveTransactionException");
					}
				}
			}
		}

		private ProductClass GetProductsByID(SecurityClass Security, string id)
		{
			return FMChannelHelper.MakeCall<IProducts, ProductClass>(
																	 x =>
																	 x.GetByID(Security, id)
																);

		}

		/// <summary>
		/// This method will create end of day transactions.
		/// </summary>
		/// <param name="inventoryDateTime">The inventory date of the transactions.</param>
		protected void CreateEndOfDayTransaction(DateTimeOffset inventoryDateTime)
		{
			var transaction = new TransactionDO
			{
				TransTypeID = TransactionTypes.T19_EndOfDay,
				TransactionDateTime = DateTimeOffset.Now,
				InventoryDate = TimeConverter.ToDate(inventoryDateTime).Date,
				Site = this.Site.ID,
				SiteGuid = this.Site.IdentityGuid,
				OriginApplication = TransactionOrigin.TerminalAutomationService
			};

			if (this.ManuallyInitiatedEod)
			{
				transaction.Alias = "Manual End Of Day";
				this.ManuallyInitiatedEod = false;
			}
			else
			{
				transaction.Alias = "End Of Day";
			}

			var saveTransactionsSR = new SaveTransactionsSR
			{
				UseAutoComplete = true,
				ConvertUnits = false,
				Security = this.Security,
				CurrentSiteGuid = this.Security.SiteGuid,
				BOLFromLoadRackFlag = true
			};

			saveTransactionsSR.Transactions.Add(transaction);

			try
			{
				FMChannelHelper.MakeCall<ISaveTransactionsProcessor, SaveTransactionsResultDO>(
																	 x =>
																	 x.SaveTransactions(saveTransactionsSR)
																);
			}
			catch (FaultException<SaveTransactionsException> e)
			{
				if (e.Detail.Results.Count >= 1
					&& e.Detail.Results[0] != null
					 && e.Detail.Results[0].ErrorList.Count >= 1)
				{
					this.EventLog.WriteEntry(e.Detail.Results[0].ErrorList[0], EventLogEntryType.Error);
				}
				else
				{
					throw new Exception("Unknown SaveTransactionException");
				}
			}
		}

		public void ResetOwnerAllocations(DateTimeOffset inventoryDate)
		{
			var logger = new Logger(StopWatch.Appnames.LoadRackService.ToString());
			logger.Debug("In SiteManager.ResetOwnerAllocations()");
			CompanyMapCollectionClass ownerManagerMapCollection = FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapCollectionClass>(
																	 x =>
																	 x.EnumerateByType(this.Security, COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP)
																);
			logger.Debug("Gotten Owner-Manager Map collection SiteManager.ResetOwnerAllocations()");

			foreach (CompanyMapClass ownerManagerMap in ownerManagerMapCollection)
			{
				Guid allocationGuid = this.GetIdentityGuid(this.Security, ownerManagerMap.IdentityGuid, DateTimeOffset.Now, DateTimeOffset.Now, COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP);
				if (allocationGuid == Guid.Empty)
				{
					continue;
				}

				logger.Debug($"Found allocation {allocationGuid} for Owner-Manager Map in SiteManager.ResetOwnerAllocations()");

				var owner = FMChannelHelper.MakeCall<ICompanies, CompanyClass>(x => x.GetBasicInfo(this.Security, ownerManagerMap.AssignedGuid, this.Security.SiteGuid));

				ProductMapCollectionClass unavailableProductMapCollection = FMChannelHelper.MakeCall<IProductMaps, ProductMapCollectionClass>(
																x =>
																x.EnumerateByAssignedToGuidAndType(this.Security, owner.IdentityGuid, PRODUCT_MAP_TYPE.UNAVAILABLE_INVENTORY_COMPANY_MAP)
														  );

				logger.Debug($"Got unavailable product map collection for owner {ownerManagerMap.AssignedGuid}");

				AllocationClass allocation = this.GetAllocationClass(this.Security, allocationGuid, STATION_TYPE.LOAD_RACK, "");
				logger.Debug($"Got allocation for guid {allocationGuid}");

				bool changed = false;

				foreach (AllocationLineItemClass lineItem in allocation.LineItemCollection)
				{
					logger.Debug($"Checking allocation line item {lineItem.IdentityGuid}");
					if (lineItem.ResetMethod != ALLOCATION_RESET_METHOD.BOOK_MINUS_UNAVAILABLE_METHOD)
					{
						continue;
					}

					logger.Debug($"Allocation line item {lineItem.IdentityGuid} is book - unavailable");

					lineItem.ResetDate.Value = inventoryDate;

					lineItem.Loaded.Value = this.GetAmountLoaded(this.Security,
						allocation.ID,
						lineItem.AssignedGuid,
						lineItem.Type,
						lineItem.ResetPeriod,
						lineItem.ResetMultiple,
						lineItem.ResetDate.Value,
						allocation.LastAllocationResetDate.Value,
						allocation._ExpirationDate.Value,
						lineItem.SiteGuid,
						STATION_TYPE.MAX_STATION_TYPE,
						"");

					logger.Debug("Found loaded amount {lineItem.Loaded.Value} against allocation line item");
					changed = true;

					// Get the ledger data
					LedgerSR ledgerSR = new LedgerSR
					{
						Security = this.Security,
						Site = this.Site.ID,
						CurrentSiteGuid = this.Site.IdentityGuid,
						Manager = ownerManagerMap.AssignedToID,
						Owner = ownerManagerMap.AssignedID,
						Product = lineItem.AssignedID,
						Month = inventoryDate.ToString("MMMM yyyy"),
						Units = QuantityDisplay.NET,
						ShowCost = false
					};
					ledgerSR.SetRequestType(LedgerSR.LedgerRequests.Refresh);

					LedgerDO ledgerDO = this.ProcessLedgerDo(ledgerSR);

					LedgerLineItemDO ledgerLineItemDO = ledgerDO.LedgerLineItems[lineItem.ResetDate.Value.Day - 1] as LedgerLineItemDO;
					logger.Debug($"Found ledger quantity of {ledgerLineItemDO.BookInventory.NetInventoryChange}");

					lineItem.Limit.Value = ledgerLineItemDO.BookInventory.NetInventoryChange;

					lineItem.Limit.Value += lineItem.Loaded.Value;

					ProductMapClass unavailableProductMap = unavailableProductMapCollection.Find(x => x.AssignedGuid == lineItem.AssignedGuid);
					logger.Debug("Got the unavailable product map for this line item");

					if (unavailableProductMap != null)
					{
						lineItem.Limit.Value -= unavailableProductMap._UnavailableInventoryNet.Value;
					}
				}

				if (changed)
				{
					logger.Debug("About to update allocation");
					this.ModifyAllocation(this.Security, allocation);
					logger.Debug("Updated allocation");
				}
				else
				{
					logger.Debug("No change to allocation");
				}
			}
		}

		public void ResetOwnerAllocationsForSingleProduct(SecurityClass passedInSecurity, DateTime inventoryDate, string productId)
		{
			long elapsed_time = 0;

			var stopwatch = new Stopwatch();
			stopwatch.Start();
			CompanyMapCollectionClass ownerManagerMapCollection = FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapCollectionClass>(
																						x =>
																						x.EnumerateByType(this.Security, COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP)
																				 );

			stopwatch.Stop();
			elapsed_time = stopwatch.ElapsedMilliseconds;
			foreach (CompanyMapClass ownerManagerMap in ownerManagerMapCollection)
			{
				stopwatch.Restart();
				Guid allocationGuid = FMChannelHelper.MakeCall<IAllocations, Guid>(
																							x =>
																							x.GetIdentityGuid(this.Security, ownerManagerMap.IdentityGuid, DateTimeOffset.Now, DateTimeOffset.Now, ownerManagerMap.Type)
																					 );
				if (allocationGuid == Guid.Empty)
				{
					continue;
				}

				var owner = FMChannelHelper.MakeCall<ICompanies, CompanyClass>(x => x.GetBasicInfo(this.Security, ownerManagerMap.AssignedGuid, this.Security.SiteGuid));

				ProductMapCollectionClass unavailableProductMapCollection = FMChannelHelper.MakeCall<IProductMaps, ProductMapCollectionClass>(
																					  x =>
																					  x.EnumerateByAssignedToGuidAndType(this.Security, owner.IdentityGuid, PRODUCT_MAP_TYPE.UNAVAILABLE_INVENTORY_COMPANY_MAP)
																				);
				// 20 seconds bds
				AllocationClass allocation = FMChannelHelper.MakeCall<IAllocations, AllocationClass>(
																					  x =>
																					  x.Get(this.Security, allocationGuid, STATION_TYPE.LOAD_RACK, string.Empty)
																				);

				bool changed = false;

				stopwatch.Stop();
				elapsed_time = stopwatch.ElapsedMilliseconds;

				foreach (AllocationLineItemClass lineItem in allocation.LineItemCollection)
				{
					if (lineItem.ResetMethod != ALLOCATION_RESET_METHOD.BOOK_MINUS_UNAVAILABLE_METHOD)
					{
						continue;
					}

					if (lineItem.AssignedID != productId)
					{
						continue;
					}

					stopwatch.Restart();

					lineItem.ResetDate.Value = inventoryDate;

					lineItem.Loaded.Value = this.GetAmountLoaded(this.Security,
																				allocation.ID,
																				lineItem.AssignedGuid,
																				lineItem.Type,
																				lineItem.ResetPeriod,
																				lineItem.ResetMultiple,
																				lineItem.ResetDate.Value,
																				allocation.LastAllocationResetDate.Value,
																				allocation._ExpirationDate.Value,
																				this.Security.SiteGuid,
																				STATION_TYPE.MAX_STATION_TYPE,
																				"");


					changed = true;
					stopwatch.Stop();
					elapsed_time = stopwatch.ElapsedMilliseconds;

					stopwatch.Restart();
					// Get the ledger data
					var ledgerSR = new LedgerSR
					{
						Security = passedInSecurity,
						Site = this.Site.ID,
						CurrentSiteGuid = this.Site.IdentityGuid,
						Manager = ownerManagerMap.AssignedToID,
						Owner = ownerManagerMap.AssignedID,
						Product = lineItem.AssignedID,
						Month = inventoryDate.ToString("MMMM yyyy"),
						Units = QuantityDisplay.NET,
						ShowCost = false
					};
					ledgerSR.SetRequestType(LedgerSR.LedgerRequests.Refresh);
					// 35 seconds
					LedgerDO ledgerDO = this.ProcessLedgerDo(ledgerSR);
					if (ledgerDO == null)
					{
						continue;
					}

					if (!(ledgerDO.LedgerLineItems[lineItem.ResetDate.Value.Day - 1] is LedgerLineItemDO ledgerLineItemDO))
					{
						continue;
					}
					lineItem.Limit.Value = ledgerLineItemDO.BookInventory.NetInventoryChange;

					lineItem.Limit.Value += lineItem.Loaded.Value;

					ProductMapClass unavailableProductMap = unavailableProductMapCollection.Find(x => x.AssignedGuid == lineItem.AssignedGuid);

					if (unavailableProductMap != null)
					{
						lineItem.Limit.Value -= unavailableProductMap._UnavailableInventoryNet.Value;
					}
					stopwatch.Stop();
					elapsed_time = stopwatch.ElapsedMilliseconds;
				}

				if (changed)
				{
					// 12 seconds
					this.ModifyAllocation(this.Security, allocation);
				}
			}
		}
		private LedgerDO ProcessLedgerDo(LedgerSR ledgerSR)
		{
			return FMChannelHelper.MakeCall<ILedgerProcessor, LedgerDO>(x =>
			  {
				  ((System.ServiceModel.IClientChannel)x).OperationTimeout = new TimeSpan(0, 15, 0);
				  return x.Process(ledgerSR);
			  }
			);

		}

		private double GetAmountLoaded(SecurityClass Security, string param1, Guid guid1, ALLOCATION_TYPE allocationType,
			ALLOCATION_RESET_PERIOD allocationResetPeriod, int param2, DateTimeOffset dateTimeOffset1, DateTimeOffset dateTimeOffset2,
			DateTimeOffset dateTimeOffset3, Guid guid2, STATION_TYPE stationType, string param3)
		{
			return FMChannelHelper.MakeCall<IAllocationLineItems, double>(
																	 x =>
																	 x.GetAmountLoaded(
																								Security,
																								param1,
																								guid1,
																								allocationType,
																								allocationResetPeriod,
																								param2,
																								dateTimeOffset1,
																								dateTimeOffset2,
																								dateTimeOffset3,
																								guid2,
																								stationType,
																								param3)
																	);
		}

		private void ModifyAllocation(SecurityClass Security, AllocationClass Allocation)
		{
			FMChannelHelper.MakeCall<IAllocations>(
																	 x =>
																	 x.Modify(Security, Allocation)
																);
		}

		private AllocationClass GetAllocationClass(SecurityClass Security, Guid allocationGuid, STATION_TYPE stationType, string param)
		{
			return FMChannelHelper.MakeCall<IAllocations, AllocationClass>(
																	 x =>
																	 x.Get(Security, allocationGuid, stationType, param)
																);
		}

		private Guid GetIdentityGuid(SecurityClass Security, Guid guid, DateTimeOffset dateTimeOffset1, DateTimeOffset dateTimeOffset2, COMPANY_MAP_TYPE companyMapType)
		{
			return FMChannelHelper.MakeCall<IAllocations, Guid>(
																	 x =>
																	 x.GetIdentityGuid(Security, guid, dateTimeOffset1, dateTimeOffset2, companyMapType)
																);
		}

		protected void EnterpriseEndOfDay(DateTimeOffset inventoryDate)
		{
			try
			{
				string enterpriseIF = FMChannelHelper.MakeCall<IConfigurationSettings, string>(
																	 x =>
																	 x.GetKeyValueByKey(this.Security, ConfigurationSettingDOClass.Key_AccountingEnterpriseInterface)
																);

				if (string.IsNullOrEmpty(enterpriseIF) == false)
				{
					char[] separator = { ';' };
					string[] enterpriseIFList = enterpriseIF.Split(separator, StringSplitOptions.RemoveEmptyEntries);

					foreach (string assemblyName in enterpriseIFList)
					{
						try
						{
							Assembly DLL = null;
							if (!AssemblyDictionary.ContainsKey(assemblyName.ToLower()))
							{
								try
								{
									DLL = Assembly.LoadFrom(assemblyName);
								}
								catch
								{
									try
									{
										DLL = Assembly.Load(assemblyName);
									}
									catch (Exception ex)
									{
										string message = "Assembly Load Error in Enterprise End Of Day Processing. " + ex.Message;
										FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(message, FMEventLogEntryType.Warning));
									}
								}

								if (DLL != null)
								{
									AssemblyDictionary.Add(assemblyName.ToLower(), DLL);
								}
							}
							else
							{
								DLL = AssemblyDictionary.Get(assemblyName.ToLower());
							}
							if (DLL == null)
							{
								continue;
							}

							try
							{
								System.Type[] Types = DLL.GetTypes();

								foreach (System.Type Module in Types)
								{
									System.Type IEnterprise = Module.GetInterface("IEnterprise");

									if (IEnterprise != null)
									{
										object Engine = Activator.CreateInstance(Module);
										IEnterprise Enterprise = (IEnterprise)Engine;

										Enterprise?.EndOfDay(this.Security, inventoryDate);
									}
								}
							}
							catch { }
						}
						catch (Exception e)
						{
							EventLog eventLog = new EventLog("Application", ".", "SiteManager");
							eventLog.WriteEntry(e.Message, EventLogEntryType.Error);
						}
					}
				}
			}
			catch (Exception e)
			{
				EventLog eventLog = new EventLog("Application", ".", "SiteManager");
				eventLog.WriteEntry(e.Message, EventLogEntryType.Error);
			}
		}

		protected void EndOfDayProcessing()
		{
			if (this.EndOfMonthState != StateEndOfMonth.Inactive)
			{
				return;
			}

			DateTimeOffset siteTimeNow = TimeConverter.Now(this.Site);
			DateTimeOffset siteTimeToday = TimeConverter.ToDate(siteTimeNow);

			Restart:

			switch (this.EndOfDayState)
			{
				case StateEndOfDay.Inactive:
					{
						if (this.Site.InhibitEndOfDayOperations)
						{
							return;
						}

						DayOfWeek day = siteTimeToday.DayOfWeek;
						int oaDay = System.Convert.ToInt32(siteTimeToday.Date.ToOADate());
						ScheduleClass activeSchedule = null;

						// Check for Holiday Schedule
						foreach (ScheduleClass schedule in this.Site.HolidayScheduleCollection)
						{
							if (schedule.Day == oaDay)
							{
								activeSchedule = schedule;
								break;
							}
						}

						if (activeSchedule == null)
						{
							activeSchedule = this.Site.OperatingScheduleCollection[(int)day];
						}


						// Test to see if current time is within the EndOfDayWarningPeriod to the EndOfDayTime
						// This thread runs every 10 seconds so there are edge cases near midnight.
						if (activeSchedule.EndOfDayEnabled
							&& siteTimeNow.TimeOfDay <= activeSchedule.EndOfDayTime.Value.TimeOfDay
							&& (siteTimeNow.AddMinutes(this.Site._EndOfDayWarningPeriod).TimeOfDay >= activeSchedule.EndOfDayTime.Value.TimeOfDay
							|| siteTimeNow.AddMinutes(1).TimeOfDay > activeSchedule.EndOfDayTime.Value.TimeOfDay
							|| siteTimeNow.AddMinutes(this.Site._EndOfDayWarningPeriod).Day != siteTimeNow.Day
							|| siteTimeNow.AddMinutes(1).Day != siteTimeNow.Day))
						{
							// End Of Day Schedule After Noon
							if (activeSchedule.EndOfDayTime.Value.TimeOfDay > new TimeSpan(12, 0, 0))
							{
								// Do not perform End Of Day processing at End of Month
								if (!this.Site.InhibitEndOfMonthOperations && siteTimeNow.AddDays(1).Day == 1)
								{
									return;
								}
							}
							else
							{
								// Do not perform End Of Day processing at End of Month
								if (!this.Site.InhibitEndOfMonthOperations && siteTimeNow.Day == 1)
								{
									return;
								}
							}

							this.EndOfDayStartTime = siteTimeToday + activeSchedule.EndOfDayTime.Value.TimeOfDay;

							this.EndOfDayInventoryDate = this.GetInventoryDate();

							// check if the inventory date will change and do not run if it will be the same
							DateTimeOffset currentDate = siteTimeNow;
							if (currentDate.Date == this.EndOfDayInventoryDate.Date && currentDate.TimeOfDay <= new TimeSpan(12, 0, 0))
							{
								return;
							}
							else if (currentDate.Date < this.EndOfDayInventoryDate.Date)
							{
								return;
							}

							this.EndOfDayState = StateEndOfDay.WarningInterval;
							this.endOfDayMessage = "Warning Interval";
							this.endOfDayProcessingPercentage = 0;

							FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
																	 x =>
																	 x.Add(this.Security, this.Site.AutomaticEndOfDayEvent)
																);

						}

						// Check schedule for tomorrow to handle case where EndOfDay is scheduled
						// within the EndOfDayWarningPeriod of midnight
						else
						{
							day = siteTimeToday.AddDays(1).DayOfWeek;
							oaDay = System.Convert.ToInt32(siteTimeToday.AddDays(1).Date.ToOADate());
							activeSchedule = null;

							// Check for Holiday Schedule
							foreach (ScheduleClass schedule in this.Site.HolidayScheduleCollection)
							{
								if (schedule.Day == oaDay)
								{
									activeSchedule = schedule;
									break;
								}
							}

							if (activeSchedule == null)
							{
								activeSchedule = this.Site.OperatingScheduleCollection[(int)day];
							}

							// Test to see if current time is within the EndOfDayWarningPeriod to the EndOfDayTime
							if (activeSchedule.EndOfDayEnabled
								&& siteTimeNow.TimeOfDay <= activeSchedule.EndOfDayTime.Value.TimeOfDay.Add(new TimeSpan(24, 0, 0))
								&& (siteTimeNow.TimeOfDay.Add(new TimeSpan(0, this.Site._EndOfDayWarningPeriod, 0)) >= activeSchedule.EndOfDayTime.Value.TimeOfDay.Add(new TimeSpan(24, 0, 0))
								|| siteTimeNow.TimeOfDay.Add(new TimeSpan(0, 1, 0)) >= activeSchedule.EndOfDayTime.Value.TimeOfDay.Add(new TimeSpan(24, 0, 0))))
							{
								// Do not perform End Of Day processing at End of Month
								if (!this.Site.InhibitEndOfMonthOperations && siteTimeNow.AddDays(1).Day == 1)
								{
									return;
								}

								this.EndOfDayStartTime = siteTimeToday + activeSchedule.EndOfDayTime.Value.TimeOfDay.Add(new TimeSpan(24, 0, 0));
								this.EndOfDayInventoryDate = this.GetInventoryDate();

								// check if the inventory date will change and do not run if it will be the same
								DateTimeOffset currentDate = siteTimeNow;
								if (currentDate.Date == this.EndOfDayInventoryDate.Date && currentDate.TimeOfDay <= new TimeSpan(12, 0, 0))
								{
									return;
								}
								else if (currentDate.Date < this.EndOfDayInventoryDate.Date)
								{
									return;
								}

								this.EndOfDayState = StateEndOfDay.WarningInterval;
								this.endOfDayMessage = "Warning Interval";
								this.endOfDayProcessingPercentage = 0;

								FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
																	 x =>
																	 x.Add(this.Security, this.Site.AutomaticEndOfDayEvent)
																);
							}
						}

						return;
					}

				case StateEndOfDay.WarningInterval:
					{
						if (siteTimeNow > this.EndOfDayStartTime)
						{
							this.EndOfDayState = StateEndOfDay.WaitingForLoadingToStop;
							this.endOfDayMessage = "Waiting For Loading to Stop";
							this.endOfDayProcessingPercentage = 0;
							goto Restart;
						}
						else
						{
							if (siteTimeNow > this.EndOfDayWarningTime)
							{
								this.EndOfDayWarningTime = siteTimeNow.AddSeconds(10);
								Monitor.Enter(this.StationManagerCollection);

								try
								{
									foreach (StationManagerClass stationManager in this.StationManagerCollection)
									{
										if (!stationManager.Station.Enabled)
										{
											continue;
										}

										Monitor.Enter(stationManager);

										try
										{
											if ((stationManager.StationState == StationState.AUTHORIZING
												|| stationManager.StationState == StationState.AUTHORIZED
												|| stationManager.StationState == StationState.TRANSACTION_IN_PROGRESS)
												&& stationManager.SendEndOfDayOrMonthWarningMessagesDuringLoading)
											{
												this.endOfDayMessage = "End Of Day in " + ((this.EndOfDayStartTime - siteTimeNow).Minutes + 1) + " " + "Minutes";
												stationManager.DisplayMessage("[LoadRack|End Of Day in]" + " " + ((this.EndOfDayStartTime - siteTimeNow).Minutes + 1) + " " + "[LoadRack|Minutes]", null, 0, this.LoadRackManager.SystemSetting.StationMessageTimeout);
											}
										}
										catch (Exception ex)
										{
											string errorStr = "SiteManager : Display Message Error Station " + stationManager.Station.ID + ". Exception: " + ex.Message;
											this.EventLog.WriteEntry(errorStr, EventLogEntryType.Error);
										}
										finally
										{
											Monitor.Exit(stationManager);
										}
									}
								}
								finally
								{
									Monitor.Exit(this.StationManagerCollection);
								}
							}
						}
						return;
					}

				case StateEndOfDay.WaitingForLoadingToStop:
					{
						Monitor.Enter(this.StationManagerCollection);

						try
						{
							if (siteTimeNow > this.EndOfDayWarningTime)
							{
								this.EndOfDayWarningTime = siteTimeNow.AddSeconds(10);

								foreach (StationManagerClass stationManager in this.StationManagerCollection)
								{
									if (!stationManager.Station.Enabled)
									{
										continue;
									}

									Monitor.Enter(stationManager);

									if (stationManager.StationState == StationState.TRANSACTION_IN_PROGRESS
										&& stationManager.SendEndOfDayOrMonthWarningMessagesDuringLoading)
									{
										try
										{
											this.endOfDayMessage = "Waiting For Loading to Stop";
											this.endOfDayProcessingPercentage = 0;
											stationManager.DisplayMessage("LoadRack|Waiting for End Of Load", null, 0, this.LoadRackManager.SystemSetting.StationMessageTimeout);

										}
										catch (Exception ex)
										{
											string errorStr = "SiteManager : Display Message Error Station " + stationManager.Station.ID + ". Exception: " + ex.Message;
											this.EventLog.WriteEntry(errorStr, EventLogEntryType.Error);
										}
									}

									Monitor.Exit(stationManager);
								}
							}

							foreach (StationManagerClass stationManager in this.StationManagerCollection)
							{
								if (stationManager.StationState == StationState.TRANSACTION_IN_PROGRESS)
								{
									return;
								}
							}
						}
						finally
						{
							Monitor.Exit(this.StationManagerCollection);
						}

						this.EndOfDayState = StateEndOfDay.PerformingEndOfDay;
						this.endOfDayMessage = "Processing";
						this.endOfDayProcessingPercentage = 0;
						goto Restart;
					}

				case StateEndOfDay.PerformingEndOfDay:
					{
						FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
																	 x =>
																	 x.Add(this.Security, this.Site.EndOfDayProcessingBeginEvent)
																);

						Monitor.Enter(this.StationManagerCollection);

						try
						{
							foreach (StationManagerClass stationManager in this.StationManagerCollection)
							{
								if (stationManager.Station.Type != STATION_TYPE.LOAD_RACK
									&& stationManager.Station.Type != STATION_TYPE.OFF_LOADING)
								{
									continue;
								}

								if (stationManager.Station.InterfaceType == STATION_INTERFACE_TYPE.MANUAL)
								{
									continue;
								}

								if (0 == stationManager.AvailableLoadArms)
								{
									continue;
								}

								Monitor.Enter(stationManager);
								try
								{
									stationManager.DisplayMessage("LoadRack|End Of Day Commenced", null, 0, 999);
								}
								catch (Exception ex)
								{
									string errorStr = "SiteManager : Display Message Error Station " + stationManager.Station.ID + ". Exception: " + ex.Message;
									this.EventLog.WriteEntry(errorStr, EventLogEntryType.Error);
								}
								finally
								{
									Monitor.Exit(stationManager);
								}
							}
						}
						finally
						{
							Monitor.Exit(this.StationManagerCollection);
						}

						try
						{
							this.endOfDayMessage = "Refreshing Tank State";
							this.endOfDayProcessingPercentage = 1;
							this.RefreshTankState();

							this.endOfDayMessage = "Creating Physical Inventory Transactions";
							this.endOfDayProcessingPercentage = 5; 
							this.CreatePhysicalInventoryTransactions(this.EndOfDayInventoryDate, SiteClass.EndOfDayFailedPhysInvCreateEventDescriptor);

							this.endOfDayMessage = "Creating Meter Reading Transactions";
							this.endOfDayProcessingPercentage = 15;
							this.CreateMeterReadingTransactions(this.EndOfDayInventoryDate);

							this.endOfDayMessage = "Printing Reports";
							this.endOfDayProcessingPercentage = 25;
							this.PrintReports(ReportConfigurationDetailSR.RequestTypes.GET_PRINT_AT_END_OF_DAY_TYPE, this.EndOfDayStartTime.AddDays(1));

							this.endOfDayMessage = "Creating Adjustment Distribution Transactions";
							this.endOfDayProcessingPercentage = 40; 
							this.CreateAdjustmentDistributionTransactions(this.EndOfDayInventoryDate);

							this.endOfDayMessage = "Creating End Of Day Transactions";
							this.endOfDayProcessingPercentage = 60;
							this.CreateEndOfDayTransaction(this.EndOfDayInventoryDate);

							this.endOfDayMessage = "Resetting Owner Allocations";
							this.endOfDayProcessingPercentage = 75;
							this.ResetOwnerAllocations(this.EndOfDayInventoryDate.AddDays(1));

							this.endOfDayMessage = "Processing Enterprise End Of Day";
							this.endOfDayProcessingPercentage = 85;
							this.EnterpriseEndOfDay(this.EndOfDayInventoryDate);

							try
							{
								this.endOfDayMessage = "Aggregating Daily BOL Totals";
								this.endOfDayProcessingPercentage = 90;
								FMChannelHelper.MakeCall<IVruTrackings>(x => x.AggregateDailyBolTotals(this.Security));
							}
							catch (ConsolidatedDAException ex)
							{
								this.endOfDayMessage = "";
								this.endOfDayProcessingPercentage = 0;
								this.endOfDayError = "Error processing End of Day";
								string errorStr = "Attempt to call VRU Support scripts failed.Please check that they have been installed. Exception: " + ex.Message;
								this.EventLog.WriteEntry(errorStr, EventLogEntryType.Warning);
							}
						}
						catch (Exception e)
						{
							this.endOfDayMessage = "";
							this.endOfDayProcessingPercentage = 0;
							this.endOfDayError = "Error processing End of Day";
							this.EventLog.WriteEntry(e.ToString(), EventLogEntryType.Error);
						}

						this.EndOfDayState = StateEndOfDay.Inactive;
						this.endOfDayMessage = "";
						this.endOfDayProcessingPercentage = 0;
						this.lastSuccessfulEndOfDayTime = DateTimeOffset.Now;

						Monitor.Enter(this.StationManagerCollection);

						try
						{

							foreach (StationManagerClass stationManager in this.StationManagerCollection)
							{
								if (stationManager.Station.Type != STATION_TYPE.LOAD_RACK)
								{
									continue;
								}

								if (stationManager.Station.InterfaceType == STATION_INTERFACE_TYPE.MANUAL)
								{
									continue;
								}

								if (0 == stationManager.AvailableLoadArms)
								{
									continue;
								}

								Monitor.Enter(stationManager);
								try
								{
									stationManager.ResetStationDevice();
								}
								catch (Exception e)
								{
									this.EventLog.WriteEntry("EndOfDayProcessing : " + e.Message, EventLogEntryType.Error);
								}
								finally
								{
									Monitor.Exit(stationManager);
								}
							}
						}
						finally
						{
							Monitor.Exit(this.StationManagerCollection);
						}

						FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
																	 x =>
																	 x.Add(this.Security, this.Site.EndOfDayProcessingEndEvent)
																);
						return;
					}
			}
		}

		private void RefreshTankState()
		{
			FMChannelHelper.MakeCall<IAlarmAndEventLogs>(x => x.Add(this.Security, this.Site.ReloadTanksEvent));
			this.LoadRackManager.EventOrAlarmEvent.Set();
			this.UnintializeTanks();
			this.InitializeTanks();
		}

		/// <summary>
		/// This method will create the end of month type of transaction.
		/// </summary>
		/// <param name="inventoryDateTime">The inventory date of the transaction.</param>
		protected void CreateEndOfMonthTransaction(DateTimeOffset inventoryDateTime)
		{
			var transaction = new TransactionDO
			{
				TransTypeID = TransactionTypes.T20_EndOfMonth,
				Alias = "End Of Month",
				TransactionDateTime = DateTimeOffset.Now,
				InventoryDate = TimeConverter.ToDate(inventoryDateTime).Date,
				Site = this.Site.ID,
				SiteGuid = this.Site.IdentityGuid,
				OriginApplication = TransactionOrigin.TerminalAutomationService
			};

			var saveTransactionsSR = new SaveTransactionsSR
			{
				UseAutoComplete = true,
				ConvertUnits = false,
				Security = this.Security,
				CurrentSiteGuid = this.Security.SiteGuid,
				BOLFromLoadRackFlag = true
			};

			saveTransactionsSR.Transactions.Add(transaction);

			try
			{
				FMChannelHelper.MakeCall<ISaveTransactionsProcessor, SaveTransactionsResultDO>(
																	 x =>
																	 x.SaveTransactions(saveTransactionsSR)
																);
			}
			catch (FaultException<SaveTransactionsException> e)
			{
				if (e.Detail.Results.Count >= 1
					&& e.Detail.Results[0] != null
					 && e.Detail.Results[0].ErrorList.Count >= 1)
				{
					this.EventLog.WriteEntry(e.Detail.Results[0].ErrorList[0], EventLogEntryType.Error);
				}

				else
				{
					throw new Exception("Unknown SaveTransactionException");
				}
			}
		}

		protected void EnterpriseEndOfMonth(DateTimeOffset inventoryDate)
		{
			try
			{
				string enterpriseIf = FMChannelHelper.MakeCall<IConfigurationSettings, string>(
																	 x =>
																	 x.GetKeyValueByKey(this.Security, ConfigurationSettingDOClass.Key_AccountingEnterpriseInterface)
																);

				if (string.IsNullOrEmpty(enterpriseIf) == false)
				{
					char[] separator = { ';' };
					string[] enterpriseIfList = enterpriseIf.Split(separator, StringSplitOptions.RemoveEmptyEntries);

					foreach (string assemblyName in enterpriseIfList)
					{
						try
						{
							Assembly dll = null;
							if (!AssemblyDictionary.ContainsKey(assemblyName.ToLower()))
							{
								try
								{
									dll = Assembly.LoadFrom(assemblyName);
								}
								catch
								{
									try
									{
										dll = Assembly.Load(assemblyName);
									}
									catch (Exception ex)
									{
										string message = "Assembly Load Error in Enterprise End Of Month Processing. " + ex.Message;
										FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(message, FMEventLogEntryType.Warning));
									}
								}

								if (dll != null)
								{
									AssemblyDictionary.Add(assemblyName.ToLower(), dll);
								}
							}
							else
							{
								dll = AssemblyDictionary.Get(assemblyName.ToLower());
							}
							
							if (dll == null)
							{
								continue;
							}

							try
							{
								System.Type[] types = dll.GetTypes();

								foreach (System.Type module in types)
								{
									System.Type iEnterprise = module.GetInterface("IEnterprise");

									if (iEnterprise != null)
									{
										object engine = Activator.CreateInstance(module);
										IEnterprise enterprise = (IEnterprise)engine;
										enterprise.EndOfMonth(this.Security, inventoryDate);
									}
								}
							}
							catch { }
						}
						catch (Exception e)
						{
							EventLog eventLog = new EventLog("Application", ".", "SiteManager");
							eventLog.WriteEntry(e.Message, EventLogEntryType.Error);
						}
					}
				}
			}
			catch (Exception e)
			{
				EventLog eventLog = new EventLog("Application", ".", "SiteManager");
				eventLog.WriteEntry(e.Message, EventLogEntryType.Error);
			}
		}

		protected void EndOfMonthProcessing()
		{
			// Do not start EndOfMonth during EndOfDay
			// If they overlap EndOfDay is EndOfMonth
			if (this.EndOfDayState != StateEndOfDay.Inactive)
			{
				return;
			}

			DateTimeOffset siteTimeNow = TimeConverter.Now(this.Site);
			DateTimeOffset siteTimeToday = TimeConverter.ToDate(siteTimeNow);

			Restart:

			switch (this.EndOfMonthState)
			{
				case StateEndOfMonth.Inactive:
					{
						if (this.Site.InhibitEndOfMonthOperations)
						{
							return;
						}

						// Last Day of the Month
						if (siteTimeNow.Month != siteTimeNow.AddDays(1).Month)
						{
							if (siteTimeNow.AddMinutes(this.Site._EndOfDayWarningPeriod).Month == siteTimeNow.AddDays(1).Month
								|| siteTimeNow.AddMinutes(1).Month == siteTimeNow.AddDays(1).Month)
							{
								this.EndOfMonthStartTime = siteTimeToday.AddDays(1);
								this.EndOfMonthInventoryDate = this.GetInventoryDate();
								this.EndOfMonthState = StateEndOfMonth.WarningInterval;
								this.endOfDayMessage = "Warning Interval";
								this.endOfDayProcessingPercentage = 0;

								FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
																	 x =>
																	 x.Add(this.Security, this.Site.AutomaticEndOfMonthEvent)
																);
							}
						}

						return;
					}
				case StateEndOfMonth.WarningInterval:
					{
						if (siteTimeNow > this.EndOfMonthStartTime)
						{
							this.EndOfMonthState = StateEndOfMonth.WaitingForLoadingToStop;
							goto Restart;
						}
						else
						{
							if (siteTimeNow > this.EndOfMonthWarningTime)
							{
								this.EndOfMonthWarningTime = siteTimeNow.AddSeconds(10);

								Monitor.Enter(this.StationManagerCollection);

								try
								{
									foreach (StationManagerClass stationManager in this.StationManagerCollection)
									{
										if (!stationManager.Station.Enabled)
											continue;

										Monitor.Enter(stationManager);

										try
										{
											if ((stationManager.StationState == StationState.AUTHORIZING
												|| stationManager.StationState == StationState.AUTHORIZED
												|| stationManager.StationState == StationState.TRANSACTION_IN_PROGRESS)
												&& stationManager.SendEndOfDayOrMonthWarningMessagesDuringLoading)
											{
												stationManager.DisplayMessage("[LoadRack|End Of Month in]" + " " + ((this.EndOfMonthStartTime - siteTimeNow).Minutes + 1) + " " + "[LoadRack|Minutes]", null, 0, this.LoadRackManager.SystemSetting.StationMessageTimeout);
											}
										}
										catch (Exception ex)
										{
											string errorStr = "SiteManager : Display Message Error Station " + stationManager.Station.ID + ". Exception: " + ex.Message;
											this.EventLog.WriteEntry(errorStr, EventLogEntryType.Error);
										}
										finally
										{
											Monitor.Exit(stationManager);
										}
									}
								}
								finally
								{
									Monitor.Exit(this.StationManagerCollection);
								}
							}
						}
						return;
					}
				case StateEndOfMonth.WaitingForLoadingToStop:
					{
						Monitor.Enter(this.StationManagerCollection);

						try
						{
							if (siteTimeNow > this.EndOfDayWarningTime)
							{
								this.EndOfDayWarningTime = siteTimeNow.AddSeconds(10);
								foreach (StationManagerClass stationManager in this.StationManagerCollection)
								{
									if (!stationManager.Station.Enabled)
									{
										continue;
									}

									Monitor.Enter(stationManager);

									try
									{
										if (stationManager.StationState == StationState.TRANSACTION_IN_PROGRESS
											&& stationManager.SendEndOfDayOrMonthWarningMessagesDuringLoading)
										{
											stationManager.DisplayMessage("LoadRack|Waiting for End Of Load", null, 0, this.LoadRackManager.SystemSetting.StationMessageTimeout);
										}
									}
									catch (Exception ex)
									{
										string errorStr = "SiteManager : Display Message Error Station " + stationManager.Station.ID + ". Exception: " + ex.Message;
										this.EventLog.WriteEntry(errorStr, EventLogEntryType.Error);
									}
									finally
									{
										Monitor.Exit(stationManager);
									}
								}
							}

							foreach (StationManagerClass stationManager in this.StationManagerCollection)
							{
								if (stationManager.StationState == StationState.TRANSACTION_IN_PROGRESS)
								{
									return;
								}
							}
						}
						finally
						{
							Monitor.Exit(this.StationManagerCollection);
						}

						this.EndOfMonthState = StateEndOfMonth.PerformingEndOfMonth;
						goto Restart;
					}

				case StateEndOfMonth.PerformingEndOfMonth:
					{
						FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
																	 x =>
																	 x.Add(this.Security, this.Site.EndOfMonthProcessingBeginEvent)
																);

						Monitor.Enter(this.StationManagerCollection);
						try
						{
							foreach (StationManagerClass stationManager in this.StationManagerCollection)
							{
								if (stationManager.Station.Type != STATION_TYPE.LOAD_RACK)
								{
									continue;
								}

								if (stationManager.Station.InterfaceType == STATION_INTERFACE_TYPE.MANUAL)
								{
									continue;
								}

								if (0 == stationManager.AvailableLoadArms)
								{
									continue;
								}

								Monitor.Enter(stationManager);

								try
								{
									stationManager.DisplayMessage("LoadRack|End Of Month Commenced", null, 0, 999);
								}
								catch (Exception ex)
								{
									string errorStr = "SiteManager : Display Message Error Station " + stationManager.Station.ID + ". Exception: " + ex.Message;
									this.EventLog.WriteEntry(errorStr, EventLogEntryType.Error);
								}
								finally
								{
									Monitor.Exit(stationManager);
								}
							}
						}
						finally
						{
							Monitor.Exit(this.StationManagerCollection);
						}

						try
						{
							this.endOfDayMessage = "Refreshing Tank State";
							this.endOfDayProcessingPercentage = 1;
							this.RefreshTankState();

							this.endOfDayMessage = "Creating Physical Inventory Transactions";
							this.endOfDayProcessingPercentage = 5;
							this.CreatePhysicalInventoryTransactions(this.EndOfMonthInventoryDate, SiteClass.EndOfMonthFailedPhysInvCreateEventDescriptor);

							this.endOfDayMessage = "Creating Meter Reading Transactions";
							this.endOfDayProcessingPercentage = 15;
							this.CreateMeterReadingTransactions(this.EndOfMonthInventoryDate);

							this.endOfDayMessage = "Printing Reports";
							this.endOfDayProcessingPercentage = 25;
							this.PrintReports(ReportConfigurationDetailSR.RequestTypes.GET_PRINT_AT_END_OF_MONTH_TYPE, this.EndOfMonthStartTime);

							this.endOfDayMessage = "Creating Adjustment Distribution Transactions";
							this.endOfDayProcessingPercentage = 40;
							this.CreateAdjustmentDistributionTransactions(this.EndOfDayInventoryDate);

							this.endOfDayMessage = "Closing Out";
							this.endOfDayProcessingPercentage = 60;
							this.Closeout();

							this.endOfDayMessage = "Creating End Of Month Transactions";
							this.endOfDayProcessingPercentage = 70;
							this.CreateEndOfMonthTransaction(this.EndOfMonthInventoryDate);

							this.endOfDayMessage = "Resetting Owner Allocations";
							this.endOfDayProcessingPercentage = 80;
							this.ResetOwnerAllocations(this.EndOfDayInventoryDate.AddDays(1));

							this.endOfDayMessage = "Processing Enterprise End Of Month";
							this.endOfDayProcessingPercentage = 90;
							this.EnterpriseEndOfMonth(this.EndOfMonthInventoryDate);

							try
							{
								this.endOfDayMessage = "Aggregating Daily BOL Totals";
								this.endOfDayProcessingPercentage = 95;
								FMChannelHelper.MakeCall<IVruTrackings>(x => x.AggregateDailyBolTotals(this.Security));
							}
							catch (ConsolidatedDAException ex)
							{
								this.endOfDayMessage = "";
								this.endOfDayProcessingPercentage = 0;
								this.endOfDayError = "Error processing End of Month";
								string errorStr = "Attempt to call VRU Support scripts failed.  Please check that they have been installed. Exception: " + ex.Message;
								this.EventLog.WriteEntry(errorStr, EventLogEntryType.Warning);
							}
						}
						catch (Exception e)
						{
							this.endOfDayMessage = "";
							this.endOfDayProcessingPercentage = 0;
							this.endOfDayError = "Error processing End of Month";
							this.EventLog.WriteEntry(e.ToString(), EventLogEntryType.Error);
						}

						this.EndOfMonthState = StateEndOfMonth.Inactive;

						Monitor.Enter(this.StationManagerCollection);

						try
						{
							foreach (StationManagerClass stationManager in this.StationManagerCollection)
							{
								if (stationManager.Station.Type != STATION_TYPE.LOAD_RACK)
								{
									continue;
								}

								if (stationManager.Station.InterfaceType == STATION_INTERFACE_TYPE.MANUAL)
								{
									continue;
								}

								if (0 == stationManager.AvailableLoadArms)
								{
									continue;
								}

								Monitor.Enter(stationManager);

								try
								{
									stationManager.ResetStationDevice();
								}
								catch (Exception e)
								{
									this.EventLog.WriteEntry("EndOfMonthProcessing : " + e.Message, EventLogEntryType.Error);
								}
								finally
								{
									Monitor.Exit(stationManager);
								}
							}
						}
						finally
						{
							Monitor.Exit(this.StationManagerCollection);
						}

						FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
																	 x =>
																	 x.Add(this.Security, this.Site.EndOfMonthProcessingEndEvent)
																);
						return;
					}
			}
		}

		protected void UpdateStations()
		{
			Monitor.Enter(this.StationManagerCollection);

			try
			{
				foreach (StationManagerClass stationManager in this.StationManagerCollection)
				{
					if (stationManager.DeferredPurge)
					{
						stationManager.DeferredPurge = false;
						this.Purge(this.Security, typeof(StationClass), stationManager.Station.IdentityGuid);
						if (stationManager.DeferredPurge)
							return;

						if (stationManager.DeferredAdd) this.Add(this.Security, typeof(StationClass), stationManager.Station.IdentityGuid);

						this.PermissiveEvent.Set();
						return;
					}
				}
			}
			finally
			{
				Monitor.Exit(this.StationManagerCollection);
			}
		}

		public void IoScan()
		{
			try
			{
				this.InitializeSite();
				this.InitializeStations();
			}
			catch (Exception e)
			{
				this.EventLog.WriteEntry(e.ToString(), EventLogEntryType.Error);
			}

			WaitHandle[] events = { this.KillEvent, this.PermissiveEvent };

			int waitResult;
			while (0 != (waitResult = WaitHandle.WaitAny(events, 1000, true)))
			{
				try
				{
					switch (waitResult)
					{
						// PermissiveEvent
						case 1:

							this.UpdateStations();
							this.PermissiveUpdateCounter = PermissiveUpdateInterval;
							this.UpdateStationPermissive();
							this.UpdateAlarmOutput();
							break;

						case WaitHandle.WaitTimeout:
							{
								if (--this.PermissiveUpdateCounter == 0)
								{
									this.PermissiveUpdateCounter = PermissiveUpdateInterval;
									this.UpdateStationPermissive();
								}

								if (--this.WatchdogUpdateCounter == 0)
								{
									this.WatchdogUpdateCounter = this.Site.WatchdogPeriod;
									this.UpdateWatchdogOutput();
									this.UpdateAlarmOutput();
								}

								if (--this.vruUpdateCounter <= 0)
								{
									try
									{
										this.vruUpdateCounter = VruUpdateInterval;
										if (FMChannelHelper.MakeCall<IVruTrackings, bool>(x => x.IsThresholdExceeded(this.Security)))
										{
											if (!this.vruThresholdExceededOnPreviousCheck)
											{
												VRUTrackingCollectionClass vruTrackingCollection = FMChannelHelper.MakeCall<IVruTrackings, VRUTrackingCollectionClass>(x => x.EnumerateThresholdsExceeded(this.Security));
												foreach (VruTrackingClass vruRuleViolated in vruTrackingCollection)
												{
													FMChannelHelper.MakeCall<IAlarmAndEventLogs>(x => x.Add(this.Security, vruRuleViolated.VruThresholdExceededAlarm));
												}

												this.LoadRackManager.EventOrAlarmEvent.Set();
											}

											this.vruThresholdExceededOnPreviousCheck = true;
										}
										else
										{
											this.vruThresholdExceededOnPreviousCheck = false;
										}
									}
									catch (ConsolidatedDAException)
									{
										this.EventLog.WriteEntry(
											 "Attempt to call VRU Support scripts failed.  Please check that they have been installed",
											 EventLogEntryType.Warning);
									}
								}

								break;
							}
					}
				}
				catch (Exception e)
				{
					this.EventLog.WriteEntry(e.Message, EventLogEntryType.Error);
				}
			}

			this.UninitializeStations();
			this.UninitializeSite();
		}

		//void OnEquipmentChange(object sender, SqlNotificationEventArgs e)
		//{
		//    this.queueEquipmentNotification = true;
		//}

		protected void EquipmentScan()
		{
			Monitor.Enter(this.EquipmentCollection);
			try
			{
				if (this.queueEquipmentNotification)
				{
					//TODO: Convert SqlDependency to something new
					//ConsolidatedDAClass consolidatedDA=new ConsolidatedDAClass();
					//SqlConnection connection = new SqlConnection(consolidatedDA.ConnectionString);
					//SqlCommand cmd = new SqlCommand("SELECT [Index],Volume FROM [dbo].tblEquipment WHERE SiteIndex="+Site.Index.ToString()+" AND SecondaryStorageFlag=CAST(1 as bit)",connection);
					//cmd.CommandType = CommandType.Text;
					//cmd.Notification = null;

					//SqlDependency dependency = new SqlDependency(cmd);
					//dependency.OnChange += new OnChangeEventHandler( OnEquipmentChange );
					//connection.Open();

					//DataTable dt = new DataTable();
					//SqlDataReader reader = cmd.ExecuteReader( CommandBehavior.CloseConnection );
					//dt.Load( reader );
					//reader.Close();

					//InitializeEquipment();
					//queueEquipmentNotification=false;
				}

				foreach (EquipmentClass equipment in this.EquipmentCollection)
				{
					// This shouldn't happen.  If Equipment has SecondaryStorageFlag set it should be owned by site.
					if (equipment.SiteGuid != this.Site.IdentityGuid)
					{
						continue;
					}

					if (equipment.VolumeProcessVariable.URL == null
					|| equipment.VolumeProcessVariable.URL == ""
					|| equipment.VolumeProcessVariable.OPCItemID == null
					|| equipment.VolumeProcessVariable.OPCItemID == "")
					{
						continue;
					}

					equipment.VolumeProcessVariable.SetValue(equipment.Volume, equipment.VolumeUnits);
					try
					{
						this.OPCServerManager.Write(equipment.VolumeProcessVariable);
					}
					catch (Exception e)
					{
						this.EventLog.WriteEntry("Equipment Scan : " + equipment.VolumeProcessVariable.OPCItemID + " " + e.ToString(), EventLogEntryType.Error);
					}
				}
			}
			finally
			{
				Monitor.Exit(this.EquipmentCollection);
			}
		}

		public void TankAndEquipmentScan()
		{
			try
			{
				this.InitializeTanks();
				this.InitializeTankGroups();
			}
			catch (Exception e)
			{
				this.EventLog.WriteEntry(e.ToString(), EventLogEntryType.Error);
			}

			WaitHandle[] Events = { this.KillEvent };

			int WaitResult;
			while (0 != (WaitResult = WaitHandle.WaitAny(Events, 10000, true)))
			{
				try
				{
					switch (WaitResult)
					{

						case WaitHandle.WaitTimeout:
							{
								if (this.TankCollection.Count != 0) this.TanksScan();

								this.EquipmentScan();

								break;
							}
					}
				}
				catch (Exception e)
				{
					this.EventLog.WriteEntry(e.Message, EventLogEntryType.Error);
				}
			}

			this.UnintializeTanks();
			this.UnintializeTankGroups();
		}

		// Responsible for End Of Day and End Of Month Processing
		public void EndOfDayAndEndOfMonthScan()
		{
			try
			{
			}
			catch (Exception e)
			{
				this.EventLog.WriteEntry(e.ToString(), EventLogEntryType.Error);
			}

			WaitHandle[] Events = { this.KillEvent };

			int WaitResult;
			while (0 != (WaitResult = WaitHandle.WaitAny(Events, 1000, true)))
			{
				try
				{
					switch (WaitResult)
					{

						case WaitHandle.WaitTimeout:
							{
								if (--this.EndOfDayUpdateCounter == 0)
								{
									this.EndOfDayUpdateCounter = EndOfDayUpdateInterval;
									this.EndOfMonthProcessing();
									this.EndOfDayProcessing();
								}
								break;
							}
					}
				}

				catch (Exception e)
				{
					this.EndOfDayState = StateEndOfDay.Inactive;
					this.endOfDayMessage = "";
					this.endOfDayProcessingPercentage = 0;
					this.EndOfMonthState = StateEndOfMonth.Inactive;
					this.EventLog.WriteEntry(e.ToString(), EventLogEntryType.Error);
				}
			}
		}


		public void Add(SecurityClass Security, System.Type Type, Guid guid)
		{
			if (Type == typeof(TankClass))
			{
				TankClass Tank = FMChannelHelper.MakeCall<ITanks, TankClass>(
																	 x =>
																	 x.Get(Security, guid)
																);

				this.AddTank(Tank);
			}
			else if (Type == typeof(TankGroupClass))
			{
				TankGroupClass TankGroup = FMChannelHelper.MakeCall<ITankGroups, TankGroupClass>(
																	 x =>
																	 x.Get(Security, guid)
																);
				this.AddTankGroup(TankGroup);
			}
			else if (Type == typeof(StationClass))
			{
				StationManagerClass StationManager = this.StationManagerCollection.FindByStationIdentityGuid(guid);

				// StationManager will be purged before an add
				if (StationManager != null
				&& StationManager.DeferredPurge)
				{
					StationManager.DeferredAdd = true;
					return;
				}

				StationClass Station = FMChannelHelper.MakeCall<IStations, StationClass>(
																	 x =>
																	 x.Get(Security, guid)
																);

				this.AddStation(Station);
			}
			else if (Type == typeof(EquipmentClass))
			{
				EquipmentClass Equipment = FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(
																	 x =>
																	 x.Get(Security, guid)
																);
				this.AddEquipment(Equipment);
			}
		}

		public void Modify(SecurityClass Security, System.Type Type, Guid identityGuid)
		{
			if (Type == typeof(TransactionAliasClass))
			{
				TransactionAliasClass TransactionAlias = FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasClass>(
																	 x =>
																	 x.Get(Security, identityGuid, false)
																);
				if (this.Site.InventoryTransactionAliasGuid == TransactionAlias.MasterRecordGuid) this.Site.InventoryTransactionAliasID = TransactionAlias.ID;

				if (this.Site.AdjustmentTransactionAliasGuid == TransactionAlias.MasterRecordGuid) this.Site.AdjustmentTransactionAliasID = TransactionAlias.ID;

				foreach (StationManagerClass StationManager in this.StationManagerCollection)
					StationManager.ModifyTransactionAlias(TransactionAlias);
			}
			else if (Type == typeof(ProductClass))
			{
				ProductClass Product = FMChannelHelper.MakeCall<IProducts, ProductClass>(
																	 x =>
																	 x.GetByInfoAuthorizedCompanies(Security, identityGuid, false, true)
																);

				this.StationManagerCollection.ModifyProduct(Product);
			}
			else if (Type == typeof(ApplicationStringClass))
			{
				ApplicationStringClass Message = FMChannelHelper.MakeCall<IApplicationStrings, ApplicationStringClass>(
																	 x =>
																	 x.Get(Security, identityGuid)
																);

				foreach (ProcessVariableClass PV in this.Site.ProcessVariableCollection)
				{
					if (PV.MessageApplicationStringGuid == identityGuid)
						PV.MessageID = Message.ID;
				}

				this.StationManagerCollection.Lock();

				try
				{
					this.StationManagerCollection.ModifyProcessVariableMessage(Message);
				}
				finally
				{
					this.StationManagerCollection.UnLock();
				}
			}
			else if (Type == typeof(TankClass))
			{
				TankClass Tank = FMChannelHelper.MakeCall<ITanks, TankClass>(
																	 x =>
																	 x.Get(Security, identityGuid)
																);

				this.StationManagerCollection.ModifyTank(Tank);
				this.PurgeTank(Tank.IdentityGuid);
				this.AddTank(Tank);
			}
			else if (Type == typeof(TankGroupClass))
			{
				TankGroupClass TankGroup = FMChannelHelper.MakeCall<ITankGroups, TankGroupClass>(
																	 x =>
																	 x.Get(Security, identityGuid)
																);

				this.StationManagerCollection.ModifyTankGroup(TankGroup);
				this.PurgeTankGroup(TankGroup.IdentityGuid);
				this.AddTankGroup(TankGroup);
			}
			else if (Type == typeof(EquipmentClass))
			{
				EquipmentClass Equipment = FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(
																	 x =>
																	 x.Get(Security, identityGuid)
																);

				this.PurgeEquipment(Equipment.IdentityGuid);
				this.AddEquipment(Equipment);
			}
			else
			{
				throw new Exception("Unsupported Object Type in Modify request");
			}
		}

		public void Purge(SecurityClass Security, System.Type Type, Guid identityGuid)
		{
			_ = Security;
			if (typeof(TankClass) == Type)
			{
				this.StationManagerCollection.Lock();
				try
				{
					if (this.StationManagerCollection.IsTankInUse(identityGuid))
					{
						throw new Exception("LoadRack|Cannot purge tank while loading in progress");
					}

					foreach (TankGroupClass TankGroup in this.TankGroupCollection)
					{
						foreach (TankMapClass TankMap in TankGroup.TankMapCollection)
						{
							if (TankMap.TankGuid == identityGuid)
							{
								TankGroup.TankMapCollection.Remove(TankMap);
								break;
							}
						}
					}

					this.PurgeTank(identityGuid);
				}
				finally
				{
					this.StationManagerCollection.UnLock();
				}
			}
			else if (typeof(TankGroupClass) == Type)
			{
				this.StationManagerCollection.Lock();
				try
				{
					if (this.StationManagerCollection.IsTankGroupInUse(identityGuid))
					{
						throw new Exception("LoadRack|Cannot purge tankgroup while loading in progress");
					}

					this.PurgeTankGroup(identityGuid);
				}
				finally
				{
					this.StationManagerCollection.UnLock();
				}
			}
			else if (typeof(EquipmentClass) == Type)
			{
				this.PurgeEquipment(identityGuid);
			}
			else if (typeof(StationClass) == Type)
			{
				this.PurgeStation(identityGuid);
			}
			else if (typeof(ApplicationStringClass) == Type)
			{
				foreach (ProcessVariableClass PV in this.Site.ProcessVariableCollection)
				{
					if (PV.MessageApplicationStringGuid == identityGuid)
					{
						PV.MessageApplicationStringGuid = Guid.Empty;
						PV.MessageID = "";
					}
				}

				this.StationManagerCollection.Lock();

				try
				{
					this.StationManagerCollection.PurgeProcessVariableMessage(identityGuid);
				}
				finally
				{
					this.StationManagerCollection.UnLock();
				}
			}
			else if (typeof(ProductClass) == Type)
			{
				this.StationManagerCollection.Lock();

				try
				{
					if (this.StationManagerCollection.IsProductInUse(identityGuid))
						throw new Exception("LoadRack|Cannot purge products while loading in progress");

					this.StationManagerCollection.PurgeProduct(identityGuid);

					foreach (TankClass Tank in this.TankCollection)
					{
						if (Tank.ProductGuid == identityGuid)
							Tank.ProductGuid = Guid.Empty;
					}

					foreach (TankGroupClass TankGroup in this.TankGroupCollection)
					{
						if (TankGroup.ProductGuid == identityGuid)
							TankGroup.ProductGuid = Guid.Empty;
					}
				}
				finally
				{
					this.StationManagerCollection.UnLock();
				}
			}
		}

		public int ActiveArms
		{
			get
			{
				int ActiveArms = 0;

				Monitor.Enter(this.StationManagerCollection);
				try
				{
					foreach (StationManagerClass StationManager in this.StationManagerCollection)
					{
						ActiveArms += StationManager.ActiveArms;
					}
				}
				finally
				{
					Monitor.Exit(this.StationManagerCollection);
				}
				return ActiveArms;
			}
		}

		public LoadArmManagerClass GetLoadArmManager(LoadArmClass LoadArm)
		{
			LoadArmManagerClass LoadArmManager = null;

			Monitor.Enter(this.StationManagerCollection);
			try
			{
				foreach (StationManagerClass StationManager in this.StationManagerCollection)
				{
					LoadArmManager = StationManager.GetLoadArmManager(LoadArm);
					if (LoadArmManager != null)
					{
						break;
					}
				}
			}
			finally
			{
				Monitor.Exit(this.StationManagerCollection);
			}

			return LoadArmManager;
		}

		public void InitiateEndOfDay(SecurityClass Security)
		{
			if (this.EndOfDayState != StateEndOfDay.Inactive)
			{
				throw new Exception("LoadRack|End Of Day is Active");
			}

			if (this.EndOfMonthState != StateEndOfMonth.Inactive)
			{
				throw new Exception("LoadRack|End Of Month is Active");
			}

			if (this.Site.InhibitEndOfDayOperations)
			{
				throw new Exception("LoadRack|End Of Day Inhibited");
			}

			DateTime InventoryDate = this.GetInventoryDate();

			DateTimeOffset CurrentDate = TimeConverter.Now(this.Site);
			if (CurrentDate.Date == InventoryDate.Date
			&& CurrentDate.TimeOfDay <= new TimeSpan(12, 0, 0))
			{
				throw new Exception("LoadRack|End Of Day has already been processed today.");
			}
			else if (CurrentDate.Date < InventoryDate.Date)
			{
				throw new Exception("LoadRack|End Of Day has already been processed today.");
			}

			this.ManuallyInitiatedEod = true;

			this.EndOfDayStartTime = CurrentDate;
			this.EndOfDayInventoryDate = InventoryDate;
			this.EndOfDayState = StateEndOfDay.WarningInterval;
			this.endOfDayMessage = "Starting End of Day";
			this.endOfDayProcessingPercentage = 0;
			this.endOfDayError = string.Empty;
			FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
																	 x =>
																	 x.Add(Security, this.Site.ManualEndOfDayEvent(Security.UserID))
																);
		}

		public StationClass GetStation(Guid identityGuid)
		{
			StationManagerClass StationManager = this.StationManagerCollection.FindByStationIdentityGuid(identityGuid);
			return StationManager?.Station;
		}

		public TransactionDO GetStationTransaction(Guid identityGuid)
		{
			StationManagerClass StationManager = this.StationManagerCollection.FindByStationIdentityGuid(identityGuid);
			if (StationManager == null)
			{
				return null;
			}

			if (StationManager.Station.Type != STATION_TYPE.LOAD_RACK &&
				StationManager.Station.Type != STATION_TYPE.OFF_LOADING)
			{
				return null;
			}

			return StationManager.Transaction;
		}

		public SaveTransactionsResultDO SaveTransaction(SaveTransactionsSR sr)
		{
			foreach (StationManagerClass StationManager in this.StationManagerCollection)
			{
				Monitor.Enter(StationManager);
			}

			try
			{
				SaveTransactionsSR saveTransactionSR = sr;
				saveTransactionSR.BOLFromLoadRackFlag = true;
				foreach (TransactionDO transactionDO in saveTransactionSR.Transactions)
				{
					foreach (StationManagerClass StationManager in this.StationManagerCollection)
						if (StationManager.IsTransactionInUse(transactionDO.TransID))
						{
							SaveTransactionsResultDO result = new SaveTransactionsResultDO();
							TransactionValidationResult transResult = new TransactionValidationResult();
							var loadStr = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(sr.Security.SiteGuid, "[LoadRack|Transaction In Use at Station : ]" + StationManager.Station.ID)
																);

							transResult.ErrorList.Add(loadStr);
							result.Results.Add(transResult);
							SaveTransactionsException saveException = new SaveTransactionsException(result.Results);
							throw new FaultException<SaveTransactionsException>(saveException, SaveTransactionsException.FaultExceptionReason);
						}
				}

				return FMChannelHelper.MakeCall<ISaveTransactionsProcessor, SaveTransactionsResultDO>(
																	 x =>
																	 x.SaveTransactions(sr)
																);
			}
			finally
			{
				foreach (StationManagerClass StationManager in this.StationManagerCollection)
				{
					Monitor.Exit(StationManager);
				}
			}
		}

		public bool AnyEntryGates
		{
			get
			{
				return this.StationManagerCollection.AnyEntryGates;
			}
		}

		public bool AnyExitGates
		{
			get
			{
				return this.StationManagerCollection.AnyExitGates;
			}
		}

		public bool TransactionLoading(string transId)
		{
			// The try catch is in case the collection is modified during
			// the iteration.  Can't lock the collection because
			// it can cause a deadlock.
			try
			{
				foreach (StationManagerClass stationManager in this.StationManagerCollection)
				{
					if (stationManager.Station.Type != STATION_TYPE.LOAD_RACK)
					{
						continue;
					}

					TransactionDO transaction = stationManager.Transaction;
					if (transaction != null && transaction.TransID == transId)
					{
						return true;
					}
				}
			}
			catch (Exception)
			{
				// Must not break loading.  If we can't read the transaction, just let it ride.
				// If it's something other than the station being modified while we're looking at it,
				// the error WILL appear somewhere else.
			}

			return false;
		}

		public bool CardedInAtLoadRack(Guid driverGuid)
		{
			// The try catch is in case the collection is modified during
			// the iteration.  Can't lock the collection because
			// it can cause a deadlock.
			try
			{
				foreach (StationManagerClass StationManager in this.StationManagerCollection)
				{
					if (StationManager.Station.Type != STATION_TYPE.LOAD_RACK &&
						StationManager.Station.Type != STATION_TYPE.OFF_LOADING)
					{
						continue;
					}

					PersonClass Driver = StationManager.Driver;
					if (Driver != null && Driver.IdentityGuid.IsNotEmptyAndEqualTo(driverGuid))
					{
						return true;
					}
				}
			}
			catch
			{
			}
			return false;
		}
	}
}

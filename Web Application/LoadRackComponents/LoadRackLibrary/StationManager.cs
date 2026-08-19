// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StationManager.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the StationManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LoadRackLibrary
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Data;
	using System.Diagnostics;
	using System.Globalization;
	using System.IO;
	using System.Net;
	using System.Linq;
	using System.Reflection;
	using System.Runtime.CompilerServices;
	using System.Runtime.InteropServices;
	using System.Security;
	using System.ServiceModel;
	using System.Threading;

	using Varec.CommonComponents.EngineeringUnitsLibrary;
	using Varec.CommonComponents.VolumeCorrection;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;
	using FMBusinessObjects.Interfaces;
	using FMBusinessObjects.LogClient;
	using FMBusinessObjects.PIDXTransactions;
	using FMBusinessObjects.ReportSvr2005;
	using FMBusinessObjects.ServiceRequests;
	using FMBusinessObjects.UtilityObjects;

	using Microsoft.Win32;

	using Opc;
	using Opc.Da;

	using Factory = OpcCom.Factory;
	using Server = Opc.Da.Server;
	using Type = System.Type;

    internal enum PreloadSelectMethod
	{
		ORDER = 0,

		LOADID = 1,

		DOCUMENT = 2
	}

	public enum StationState
	{
		IDLE = 0,

		PIN_PROMPT = 1,

		ENTER_TRACTOR_PROMPT = 2,

		ENTER_TRAILER1_PROMPT = 3,

		ENTER_TRAILER2_PROMPT = 4,

		LOADID_PROMPT = 5,

		DRIVER_QUALIFICATION_WARNING = 6,

		DRIVER_LICENSE_WARNING = 7,

		COMPANY_CERTIFICATE_OR_PERMIT_WARNING = 8,

		COMPANY_LICENSE_WARNING = 9,

		COMPANY_INSURANCE_WARNING = 10,

		TRACTOR_TAG_OR_LICENSE_WARNING = 11,

		TRACTOR_TEST_OR_INSPECTION_WARNING = 12,

		TRAILER1_TAG_OR_LICENSE_WARNING = 13,

		TRAILER1_TEST_OR_INSPECTION_WARNING = 14,

		TRAILER2_TAG_OR_LICENSE_WARNING = 15,

		TRAILER2_TEST_OR_INSPECTION_WARNING = 16,

		DRIVER_MESSAGE_PROMPT = 17,

		PURCHASE_ORDER_PROMPT = 18,

		PRODUCT_AVAILABILITY_MESSAGE = 19,

		AUTHORIZED = 20,

		TRANSACTION_IN_PROGRESS = 21,

		LOADID_CARD_PROMPT = 22,

		OPENING_GATE = 23,

		OPERATING_MODE_PROMPT = 24,

		USE_ORDER_PROMPT = 25,

		ENTER_ORDER_PROMPT = 26,

		ENTER_CUSTOMER_SHIPTO_PROMPT = 27,

		SELECT_ORDER_PROMPT = 28,

		SELECT_CUSTOMER_SHIPTO_FILTER_PROMPT = 29,

		ENTER_ZIP_PROMPT = 30,

		SELECT_CUSTOMER_SHIPTO_FILTER_VALUE_PROMPT = 31,

		SELECT_CUSTOMER_SHIPTO_PROMPT = 32,

		SELECT_COMPANY_HIERARCHY_PROMPT = 33,

		SUMMARY_PROMPT = 34,

		COMPARTMENT_SUMMARY_PROMPT = 35,

		PRODUCT_PROMPT = 36,

		PRESET_PROMPT = 37,

		CAPTURE_TARE_WEIGHT_PROMPT = 38,

		CAPTURE_EXIT_WEIGHT_PROMPT = 39,

		ADDITIONAL_ORDERS_PROMPT = 40,

		ENTRY_INSTRUCTION_PROMPT = 41,

		EXIT_INSTRUCTION_PROMPT = 42,

		PRODUCT_UNAVAILABLE_MESSAGE = 43,

		INVALID_PRELOAD_ORDER_SELECTION_MSG = 44,

		PRELOAD_ORDER_PROMPT = 45,

		AUTHORIZING = 46,

		PRELOAD_TYPE_PROMPT = 47,

		INVALID_PRELOAD_TYPE_SELECTION_MSG = 48,

		PRELOAD_LOADID_PROMPT = 49,

		INVALID_PRELOAD_LOADID_SELECTION_MSG = 50,

		PRELOAD_DOCNUMBER_PROMPT = 51,

		INVALID_PRELOAD_DOCUMENT_SELECTION_MSG = 52,

		COMPARTMENTS_NOT_CONFIGURED = 53,

		COMPANY_INVALID = 54,

		RESET_ON_TIMEOUT = 55,

		TANK_NOT_CERTIFIED_MSG = 56,

		ENTER_DRIVER_ID_PROMPT = 57,

		FAILED_CERTIFICATE_OF_ANALYSIS_MSG = 58,

		BUILD_PLAN = 59,

		INVALID_COMPANY_ON_ORDER = 60,

		NO_SHOPTO_MSG = 61,

		ENTER_2ND_TRAILER_PROMPT = 62,

		ENTER_1ST_TRAILER_PROMPT = 63,

		TRANSACTION_ALIAS_INVALID_MSG = 64,

		CANCEL_TRANSACTION_PROMPT = 65,

		BUILD_RECIPE_MAP = 66,

		NOT_AVAILABLE_SUMMARY_PROMPT = 67,

		NOT_AVAILABLE_TRACTOR_OR_TANKER_PROMPT = 68,

		NOT_AVAILABLE_ENTER_TRAILER1_PROMPT = 69,

		NOT_AVAILABLE_ENTER_TRAILER2_PROMPT = 70,

		NOT_AVAILABLE_TRAILER1_PROMPT = 71,

		NOT_AVAILABLE_TRAILER2_PROMPT = 72,

		CARRIER_INVALID = 73,

		NO_PRODUCTS_MSG = 74,

		OVERWEIGHT_MSG = 75,

		UPDATE_RECIPE_ERROR_MSG = 76,

		UPDATE_DENSITY_ERROR_MSG = 77,

		SELECT_TRACTOR_PROMPT = 78,

		SELECT_TRAILER1_PROMPT = 79,

		SELECT_TRAILER2_PROMPT = 80,

		ADDITIVE_PROFILE_UNAVAILABLE_MESSAGE = 81,

		SIGNATURE_CAPTURE = 82,

		NO_PIDX_AUTHORIZATION_MSG = 83,

		PIDX_UNAVAILABLE_MSG = 84,

		PIDX_DENIAL_MSG = 85,

		VERIFY_SHIPTO_MSG = 86,

		BROKEN_BLEND = 87,

		IMPROPER_ADDITIZATION = 88,

		BROKEN_BLEND_WEIGHTOUT = 89,

		IMPROPER_ADDITIZATION_WEIGHTOUT = 90,

		ENTER_SHIPMENT_NUMBER_PROMPT = 91,

		SHIPMENTNUMBER_NOTFOUND = 92,

		VERIFY_SHIPTO_MSG_PRELOAD = 93,

		SELECT_SHIPMENT_LOADID_PROMPT = 94,

		CONTAMINATION_PROMPT = 95,

		COMPARTMENTS_PREVIOUSLY_LOADED_PROMPT = 96,

		COMPARTMENTS_EMPTY_PROMPT = 97,

		PRODUCT_ALLOCATION_MESSAGE = 98,

		INVALID_SHIPTO_PROMPT_RESPONSE_MESSAGE = 99,

		INVALID_ENTER_TRAILER1_PROMPT_RESPONSE_MESSAGE = 100,

		INVALID_ENTER_TRAILER2_PROMPT_RESPONSE_MESSAGE = 101,

		ENTER_TRACTOR_CARDIN_PROMPT = 102,

		PROMPT_FOR_RETURNS = 103,

		ENTER_SUPPLY_ORDER_NUMBER = 104,

		ENTER_UNLOAD_DENSITY = 105,

		ENTER_UNLOAD_AMOUNT = 106,

		PROMPT_FOR_RECIRC_CONFIMATION = 107,

		PROMPT_FOR_SOURCE_TANK = 108,

		PROMPT_FOR_DESTINATION_TANK = 109,

		ENTER_METER_RECIRC_AMOUNT = 110,

		DRIVER_TRAINING_WARNING = 111,

		OFFLOADID_PROMPT = 112,

		ENTER_MANUAL_METER_START_VALUE = 113,

		MANUAL_OFFLOAD_INPROGRESS = 114,

		ENTER_MANUAL_METER_STOP_VALUE = 115,

		SELECT_OFFLOAD_PRODUCT = 116,

		VERIFY_OFFLOAD_SUPPLIER = 117,

		INVALID_SUPPLIER_PROMPT_RESPONSE_MESSAGE = 118,

		PROMPT_FOR_OFFLOAD_COMPLETE = 119,

		INVALID_OFFLOAD_COMPLETE_TYPE_SELECTION_MSG = 120,

		USE_SUPPLYORDER_PROMPT = 121,

		ENTER_SUPPLY_ORDER_NUMBER_LIST = 122,

		SELECT_SUPPLIER_OFFLOADID_FILTER_PROMPT = 123,

		ENTER_OFFLOADID_ZIP_PROMPT = 124,

		NO_SUPPLIER_MSG = 125,

		SELECT_SUPPLIER_PROMPT = 126,

		SELECT_SUPPLIER_FILTER_VALUE_PROMPT = 127,

		SELECT_DESTINATION_SUPPLIER_PROMPT = 128,

		ENTER_BOL_NUMBER = 129,
		// the following are required for the off loading station using a Varec DET
		AUTHORIZED_PERMISSIVE_PROMPT = 130,

		INPROGRESS_PERMISSIVE_PROMPT = 131,

		SELECT_DRIVER_COMPANY = 132,
		ENTER_TANKER_PROMPT = 133,

		SELECT_GATE = 134,

		LINEITEM_SUMMARY_PROMPT = 135,

		ENTER_UNLOAD_TEMPERATURE,

		ENTER_TRAILER3_PROMPT,

		SELECT_TRACTOR_OR_TANKER_PROMPT,

		TRACTOR_OR_TANKER_PROMPT,

		ENTER_3RD_TRAILER_PROMPT,

		NOT_AVAILABLE_ENTER_TRAILER3_PROMPT,

		INVALID_ENTER_TRAILER3_PROMPT_RESPONSE_MESSAGE,

		SELECT_TRAILER3_PROMPT,

		NOT_AVAILABLE_TRAILER3_PROMPT,

		TRAILER3_TAG_OR_LICENSE_WARNING,

		TRAILER3_TEST_OR_INSPECTION_WARNING,

		OFFLOAD_PRODUCT_AVAILABILITY_MESSAGE,

		OFFLOAD_PRODUCT_PROMPT,

        CHECK_PIDX_AUTHORIZATIONS

    };


	/// <summary>
	///    Summary description for StationManagerCollectionClass.
	/// </summary>
	public class StationManagerCollectionClass : CollectionBase
	{
		#region Constants and Fields

		protected int NumberOfEntryGateStations;

		protected int NumberOfExitGateStations;

		#endregion

		#region Public Properties

		public bool AnyEntryGates
		{
			get
			{
				return this.NumberOfEntryGateStations != 0;
			}
		}

		public bool AnyExitGates
		{
			get
			{
				return this.NumberOfExitGateStations != 0;
			}
		}

		#endregion

		#region Public Methods and Operators

		public void Add(StationManagerClass stationManager)
		{
			this.List.Add(stationManager);
			if (stationManager.Station.Type == STATION_TYPE.ENTRY_GATE)
			{
				this.NumberOfEntryGateStations++;
			}
			if (stationManager.Station.Type == STATION_TYPE.EXIT_GATE)
			{
				this.NumberOfExitGateStations++;
			}
		}

		public StationManagerClass FindByStationIdentityGuid(Guid targetGuid)
		{
			foreach (StationManagerClass Item in this.List)
			{
				if (Item.Station.IdentityGuid == targetGuid)
				{
					return Item;
				}
			}

			return null;
		}

		public bool IsProductInUse(Guid identityGuid)
		{
			foreach (StationManagerClass item in this.List)
			{
				if (item.IsProductInUse(identityGuid))
				{
					return true;
				}
			}

			return false;
		}

		public bool IsTankGroupInUse(Guid identityGuid)
		{
			foreach (StationManagerClass item in this.List)
			{
				if (item.IsTankGroupInUse(identityGuid))
				{
					return true;
				}
			}

			return false;
		}

		public bool IsTankInUse(Guid identityGuid)
		{
			foreach (StationManagerClass item in this.List)
			{
				if (item.IsTankInUse(identityGuid))
				{
					return true;
				}
			}

			return false;
		}

		public StationManagerClass Item(int index)
		{
			return (StationManagerClass)this.List[index];
		}

		public void Lock()
		{
			foreach (StationManagerClass item in this.List)
			{
				Monitor.Enter(item);
			}
		}

		public void ModifyProcessVariableMessage(ApplicationStringClass message)
		{
			foreach (StationManagerClass item in this.List)
			{
				item.ModifyProcessVariableMessage(message);
			}
		}

		public void ModifyProduct(ProductClass product)
		{
			foreach (StationManagerClass item in this.List)
			{
				item.ModifyProduct(product);
			}
		}

		public void ModifyTank(TankClass tank)
		{
			foreach (StationManagerClass item in this.List)
			{
				item.ModifyTank(tank);
			}
		}

		public void ModifyTankGroup(TankGroupClass tankGroup)
		{
			foreach (StationManagerClass item in this.List)
			{
				item.ModifyTankGroup(tankGroup);
			}
		}

		public void PurgeProcessVariableMessage(Guid identityGuid)
		{
			foreach (StationManagerClass item in this.List)
			{
				item.PurgeProcessVariableMessage(identityGuid);
			}
		}

		public void PurgeProduct(Guid identityGuid)
		{
			foreach (StationManagerClass item in this.List)
			{
				item.PurgeProduct(identityGuid);
			}
		}

		public void Remove(int index)
		{
			if (index > this.Count - 1 || index < 0)
			{
				throw new IndexOutOfRangeException("Invalid Index");
			}

			StationManagerClass stationManager = this.List[index] as StationManagerClass;
			if (stationManager == null)
			{
				return;
			}

			if (stationManager.Station.Type == STATION_TYPE.ENTRY_GATE)
			{
				this.NumberOfEntryGateStations--;
			}
			if (stationManager.Station.Type == STATION_TYPE.EXIT_GATE)
			{
				this.NumberOfExitGateStations--;
			}

			this.List.RemoveAt(index);
		}

		public void Remove(StationManagerClass stationManager)
		{
			int index = 0;
			foreach (StationManagerClass item in this.List)
			{
				if (item.Station.IdentityGuid == stationManager.Station.IdentityGuid)
				{
					if (stationManager.Station.Type == STATION_TYPE.ENTRY_GATE)
					{
						this.NumberOfEntryGateStations--;
					}
					if (stationManager.Station.Type == STATION_TYPE.EXIT_GATE)
					{
						this.NumberOfExitGateStations--;
					}

					this.List.RemoveAt(index);
					return;
				}
				index++;
			}
		}

		public void UnLock()
		{
			foreach (StationManagerClass item in this.List)
			{
				Monitor.Exit(item);
			}
		}

		#endregion
	}

	public class OrderComparer : IComparer
	{
		#region Explicit Interface Methods

		int IComparer.Compare(object x, object y)
		{
			return string.Compare(((GetTransactionTypeDO)x)?.DocumentNumber, ((GetTransactionTypeDO)y)?.DocumentNumber, StringComparison.Ordinal);
		}

		#endregion
	}

	/// <summary>
	///    Summary description for StationManagerClass.
	/// </summary>
	public abstract class StationManagerClass : MarshalByRefObject, IDisposable
	{
		#region Constants and Fields
		internal const string EscapeString = "Escape";
		internal const string TimeoutString = "Timeout";

		public bool CommunicationsFailure;

		public ArrayList CompartmentList;

		public ArrayList ContaminationPromptStatusList;

		public TransactionAliasClass CurrentTransactionAlias;

		public string CustomerShipToFilterColumn = "";

		public string CustomerShipToFilterValue = "";

		public bool DeferredAdd = false;

		public bool DeferredPurge = false;

		public ManualResetEvent DownloadConfigurationEvent;

		public PersonClass Driver;

		public string FromStorageLocationID = null; // source tank ID

		public Guid FromStorageLocationTankGuid; // source tank index

		public LoadArmManagerCollectionClass LoadArmManagerCollection = new LoadArmManagerCollectionClass();

		public LoadArmManagerCollectionClass LoadArmManagerDisabledCollection = new LoadArmManagerCollectionClass();

		public OperatingMode Mode;

		public double OffLoadPresetAmount = 0.0;

		public TransactionDO Order;

		public TransactionAliasClass OrderTransactionAlias;

		public bool RemoteAuthorized;

		public SubLineItemDO RemoteSubLineItem = null;

		public SecurityClass Security;

		public CompanyClass ShipTo;

		public SiteManagerClass SiteManager;

		public StationClass Station;

		public ProcessVariableClass StationPv;

		public virtual StationState StationState { get; set; } = StationState.IDLE;

		public TransactionDO SupplyOrder;

		public TransactionAliasClass SupplyOrderTransactionAlias;

		public DateTimeOffset TimeIn;

		public string ToStorageLocationID = null; // destination tank ID

		public Guid ToStorageLocationTankGuid; // destination tank index

		public EquipmentClass TractorOrTanker;

		public EquipmentClass Trailer1;

		public EquipmentClass Trailer2;

		public EquipmentClass Trailer3;

		public TransactionDO Transaction;

		public bool TransactionSupportsMultipleLineItems = true;

		public ManualResetEvent UploadStoredTransactionsEvent;

		public bool InRecircMode = false;

		public bool ShuttingDown;
		// set at true when shutting down so the loadarmmanagers can control the load rack display

		public bool UseOffLoadSupplyOrders;

		protected const int PermissivesUpdateInterval = 10;

		protected const int PinLength = 4;

		protected const int PromptLength = 10;

		protected const int TransactionUpdateInterval = 10;

		protected AllocationClass[] AllocationArray = new AllocationClass[4];

		protected bool AlreadyDisposed;

		protected int AuthorizedProductIndex;

		protected CompanyClass BillTo;

		protected int BuildPlanLineItemIndex;

		protected int BuildRecipeMapAuthorizedProductIndex;

		protected bool ByWeight;

		protected string ByWeightProduct = "";

		protected string CardID;

		protected CompanyClass Carrier;

		protected int CarrierCompanyIndex; // selected company index for the driver

		protected bool CarrierMatchesAndPreloadIsSelected = false;

		protected string CommLogFileName = "";

		protected CompanyMapCollectionClass CompanyMapCollection;

		protected byte ConsecutivePrompts;

		protected COMPANY_MAP_TYPE CurrentCompanyHierarchyType = COMPANY_MAP_TYPE.SHIPTO_BILLTO_MAP;

		protected int CurrentCompartmentNumber;

		protected EquipmentClass CurrentEquipment;

		protected int CurrentLineItemBaseIndex = 0;

		protected double CurrentMaximum;

		protected DisplayMenuParameters CurrentMenuParameters;

		protected ItemValueResult CurrentWeight;

		protected int DriverMessageIndex;

		protected EventLog eventLog;

		protected ProcessVariableClass GatePV;

		protected int GateTimer = 0;

		protected CompanyCollectionClass HierarchyCompanyCollection = new CompanyCollectionClass();

		protected int InstructionIndex;

		protected ArrayList Instructions = new ArrayList();

		protected ManualResetEvent KillEvent;

		protected DateTimeOffset LastActivityDateTime;

		protected DateTimeOffset LastScanDateTime;

		protected string LoadID;

		protected ArrayList LoadIDList;

		protected LoadRackManagerClass LoadRackManager;

		protected bool LoadSummaryIssued;

		internal CompanyClass Manager;

		protected MessageCollectionClass MessageCollection;

		protected OPCServerManagerClass OPCServerManager;

		protected ArrayList OrderList;

		protected bool Orders;

		protected CompanyClass Owner;

		protected PIDXAuthorizationBase[] PIDXAuthorizationArray;

		protected PIDXProfileCompanyMapCollectionClass PIDXProfileCompanyMapCollection;

		protected string PONumber;

		protected ArrayList PendingTransactions = new ArrayList();

		protected AutoResetEvent PermissiveEvent;

		protected DataSet PreloadDataSet;

		protected PRELOAD_SELECT_METHOD PreloadSelectMethod = PRELOAD_SELECT_METHOD.DOCUMENT;

		protected int PriorMessageTimeout;

		protected int PriorResponseLength;

		protected StationState PriorStationState = StationState.IDLE;

		protected string PriorStockMessage = "";

		protected string SelectedBOLNumber = "";

		public bool bInRecircMode = false;
		protected string SelectedSupplyOrder = "";

		protected CompanyClass Shipper;

		protected bool SingleProduct;

		protected DateTimeOffset StartDateTime;

		protected Thread StationScanThread;

		protected CompanyClass Supplier;

		protected int UpdatePermissivesCounter = PermissivesUpdateInterval;

		protected int UpdateTransactionCounter = TransactionUpdateInterval;

		protected int WatchdogCounter = 1;

		protected TransactionDO[] inprogressTransaction;

		protected bool bScullyBypass = false;

		protected bool bScullyFailMannualEnter = false;

		string TIN = string.Empty;

		static Assembly QAAssembly = null;

		private readonly Dictionary<Guid, bool> productClosedOutCache = new Dictionary<Guid, bool>();

		internal Dictionary<int, ProductMapClass> RecipeInternalNumberMap = null;
		internal int LastDownloadedRecipe = 0;
		#endregion

		#region Constructors and Destructors

		public StationManagerClass(
		 EventLog eventLog,
		 LoadRackManagerClass loadRackManager,
		 StationClass station,
		 SiteManagerClass siteManager,
		 SecurityClass security)
		{
			Monitor.Enter(this);
			try
			{
				this.Transaction = null;

				this.eventLog = eventLog;
				this.LoadRackManager = loadRackManager;
				this.Station = station;
				this.SiteManager = siteManager;
				this.Security = security;

				// Get the GatePV
				foreach (ProcessVariableClass pv in station.ProcessVariableCollection)
				{
					if (pv.ProcessVariableType == PROCESS_VARIABLE_TYPE.STATION_PV)
					{
						this.StationPv = pv;
					}

					if (pv.ProcessVariableType == PROCESS_VARIABLE_TYPE.GATE_CONTROL_PV)
					{
						this.GatePV = pv;
					}
				}

				// On Startup retrieve any active BOL for this Station
				if (station.Type == STATION_TYPE.LOAD_RACK)
				{
					GetTransactionSR getTransactionSR = new GetTransactionSR
					{
						Security = security,
						Request = GetTransactionRequest.SITE_TYPEID_ALIAS_STATUS_LOCATION_LINEITEMSTATUS,
						Site = siteManager.Site.ID,
						TransTypeID = TransactionTypes.T5_PrimaryDisbursement,
						AliasName = station.IssueByVolumeTransactionAliasID,
						Location = station.ID,
						Status = ((int)TransactionStatus.InProgress).ToString(),
						LineItemStatus = ((int)TransactionStatus.InProgress).ToString()
					};

					GetTransactionDO getTransactionDO =
					  FMChannelHelper.MakeCall<IGetTransactionProcessor, GetTransactionDO>(x => x.Process(getTransactionSR));

					if (getTransactionDO != null && getTransactionDO.TransactionDataSet != null
						 && getTransactionDO.TransactionDataSet.Tables.Count != 0)
					{
						if (getTransactionDO.TransactionDataSet.Tables[0].Rows.Count == 1)
						{
							this.Transaction = this.GetTransaction((string)getTransactionDO.TransactionDataSet.Tables[0].Rows[0]["TransID"]);

							if (this.Transaction != null)
							{
								eventLog.WriteEntry(
									"StationManager startup processing transaction already in progress (" + this.Transaction.TransID + ")",
									EventLogEntryType.Information);

								bool bAuthorized = true;
								foreach (LineItemDO LineItem in this.Transaction.LineItems)
								{
									if (LineItem.Status == TransactionStatus.InProgress)
									{
										bAuthorized = false;
										break;
									}
								}

								if (bAuthorized)
								{
									this.CompleteTransaction();
									this.CheckForBrokenBlends(this.Transaction);
									this.CheckForImproperAdditization(this.Transaction);
								}
								else
								{
									this.StationState = StationState.TRANSACTION_IN_PROGRESS;

									this.LastActivityDateTime = DateTimeOffset.Now;
									this.StartDateTime = DateTimeOffset.Now;

									// Retrieve the ShipTo Company
									if (this.Transaction.ShipToCompanyGuid != Guid.Empty)
									{
										this.ShipTo =
											FMChannelHelper.MakeCall<ICompanies, CompanyClass>(x => x.Get(security, this.Transaction.ShipToCompanyGuid));
									}

									// Retrieve the Carrier Company
									if (this.Transaction.CarrierCompanyGuid != Guid.Empty)
									{
										this.Carrier =
											FMChannelHelper.MakeCall<ICompanies, CompanyClass>(x => x.Get(security, this.Transaction.CarrierCompanyGuid));
									}

									// Reset Lockouts for Hazardous Material Exclusion
									if (this.Carrier != null && this.Carrier.HazardousMaterialExclusion)
									{
										foreach (ProductMapClass AuthorizedProduct in this.ShipTo.AuthorizedProductCollection)
										{
											if (AuthorizedProduct.HazardousMaterial)
											{
												AuthorizedProduct.LockedOut = true;
											}
										}
									}

									// Retrieve the Tractor and Trailer(s)
									if (this.Transaction.DestinationEQ1.EquipmentGuid != Guid.Empty)
									{
										this.TractorOrTanker =
											FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(
												x => x.Get(security, this.Transaction.DestinationEQ1.EquipmentGuid));
									}

									if (this.Transaction.DestinationEQ2.EquipmentGuid != Guid.Empty)
									{
										this.Trailer1 =
											FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(
												x => x.Get(security, this.Transaction.DestinationEQ2.EquipmentGuid));
									}

									if (this.Transaction.DestinationEQ3.EquipmentGuid != Guid.Empty)
									{
										this.Trailer2 =
											FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(
												x => x.Get(security, this.Transaction.DestinationEQ3.EquipmentGuid));
									}
								}
							}
						}
						else if (getTransactionDO.TransactionDataSet.Tables[0].Rows.Count > 1)
						{
							// Reset the transactions and bail out
							foreach (DataRow Row in getTransactionDO.TransactionDataSet.Tables[0].Rows)
							{
								this.Transaction = this.GetTransaction((string)Row["TransID"]);
								if (this.Transaction != null)
								{
									this.Transaction.Status = TransactionStatus.Cancelled;
									foreach (LineItemDO LineItem in this.Transaction.LineItems)
									{
										// Cancel Line Items for which no loading occured
										if (LineItem.Status == TransactionStatus.LoadPending
											 || (LineItem.Status == TransactionStatus.InProgress && LineItem.Quantity.GrossInventoryChange == 0
												  && LineItem.Quantity.NetInventoryChange == 0 && LineItem.Quantity.MassInventoryChange == 0))
										{
											LineItem.Status = TransactionStatus.Cancelled;
											foreach (SubLineItemDO SubLineItem in LineItem.SubLineItems)
											{
												SubLineItem.Status = TransactionStatus.Cancelled;
											}
										}

										// Otherwise complete them
										else
										{
											LineItem.Status = TransactionStatus.Completed;
											foreach (SubLineItemDO SubLineItem in LineItem.SubLineItems)
											{
												SubLineItem.Status = TransactionStatus.Completed;
											}
										}
									}

									this.CheckForBrokenBlends(this.Transaction);
									this.CheckForImproperAdditization(this.Transaction);

									this.SaveTransaction();
									this.Transaction = null;
								}
							}
						}
					}
				}

				// if configured for comm logging open and create the file first
				this.OpenAndCreateCommLogFile(true);

				siteManager.PermissiveEvent.Set();

				this.OPCServerManager = new OPCServerManagerClass(eventLog);
				this.OPCServerManager.Invoke += this.OnInvoke;

				station.StationPermissives.Enabled = false;

				foreach (ProcessVariableClass PV in station.StationPermissives.Outputs)
				{
					this.OPCServerManager.AddProcessVariable(PV);
				}

				foreach (ProcessVariableClass PV in station.StationPermissives.Inputs)
				{
					this.OPCServerManager.AddProcessVariable(PV);
				}

				// Create Load Arm Managers for each Load Arm
				foreach (LoadArmClass LoadArm in station.LoadArmCollection)
				{
					this.CreateLoadArmManager(LoadArm);
				}

				// Launch a thread to periodically update Active BOL Line Items
				// Not appropriate for Signature Stations
				if (station.Type != STATION_TYPE.SIGNATURE)
				{
					ThreadStart StationScanStart = this.StationScan;

					this.KillEvent = new ManualResetEvent(false);
					this.PermissiveEvent = new AutoResetEvent(false);
					this.DownloadConfigurationEvent = new ManualResetEvent(false);
					this.UploadStoredTransactionsEvent = new ManualResetEvent(false);

					this.StationScanThread = new Thread(StationScanStart);
					this.StationScanThread.Start();
				}

				// In order for this event registration to work, you have to set both this service and the FMDataManager
				// service logon checkbox called "Allow service to interact with desktop" to true.
				try
				{
					SystemEvents.TimeChanged += this.SystemEventsTimeChanged;
				}
				catch
				{
				}

				// If we have a transaction that is allegedly in progress, check the station for what it is doing
				this.CheckDeviceStatus();
			}
			catch (Exception e)
			{
				eventLog.WriteEntry("StationManager Constructor : " + e.Message, EventLogEntryType.Error);
			}
			finally
			{
				Monitor.Exit(this);
			}
		}

		~StationManagerClass()
		{
			this.Dispose();
		}

		#endregion

		#region Enums

		protected enum PRELOAD_SELECT_METHOD
		{
			ORDER = 0,

			LOADID = 1,

			DOCUMENT = 2
		}

		 internal enum CommLogDirection
		 {
			  None = 0,
				In = 1,
				Out = 2
		 }

		public enum OperatingMode
		{
			Loading = 0,
			Unloading = 1
		}

		 public int CurrentBatchNumber { get; set; }

		protected double OffloadDensity { get; set; }
		protected double OffloadTemperature { get; set; }

		public int AvailableLoadArms
		{
			get
			{
				int Arms = 0;
				foreach (LoadArmManagerClass LoadArmManager in this.LoadArmManagerCollection)
				{
					if (this != LoadArmManager.GetStationManager())
					{
						continue;
					}

					Arms++;
				}

				return Arms;
			}
		}

		public CompanyClass CompanyShipTo
		{
			get
			{
				return this.ShipTo;
			}
		}

		protected bool PinRequired
		{
			get
			{
				return (this.Driver?.PINRequired ?? false) || (this.Carrier?.PINRequired ?? false); 
			}
		}

		public int CompartmentsLeftToLoad
		{
			get
			{
				int CompartmentsToLoad = 0;
				if (this.CompartmentList == null)
				{
					return CompartmentsToLoad;
				}

				foreach (CompartmentInfo Info in this.CompartmentList)
				{
					if (Info.Loaded == false)
					{
						CompartmentsToLoad++;
					}
				}

				return CompartmentsToLoad;
			}
		}

		public int CountOfEquipmentWithCompartmentsToLoad
		{
			get
			{
				int nCount = 0;

				// Check the TractorOrTanker equipment
				if (this.TractorOrTanker != null && this.TractorOrTanker.IsMultiCompartment)
				{
					if (
						this.IsInCompartmentList(
							this.SiteManager.Site.UseCompanyEquipmentIdentifiers
								? this.TractorOrTanker.CompanyEquipmentID
								: this.TractorOrTanker.ID))
					{
						++nCount;
					}
				}

				// Check Trailer1
				if (this.Trailer1 != null)
				{
					if (
						this.IsInCompartmentList(
							this.SiteManager.Site.UseCompanyEquipmentIdentifiers ? this.Trailer1.CompanyEquipmentID : this.Trailer1.ID))
					{
						++nCount;
					}
				}

				// Check Trailer2
				if (this.Trailer2 != null)
				{
					if (
						this.IsInCompartmentList(
							this.SiteManager.Site.UseCompanyEquipmentIdentifiers ? this.Trailer2.CompanyEquipmentID : this.Trailer2.ID))
					{
						++nCount;
					}
				}

				return nCount;
			}
		}

		public LoadArmManagerCollectionClass FullLoadArmCollection
		{
			get
			{
				LoadArmManagerCollectionClass fullList = new LoadArmManagerCollectionClass
				{
					this.LoadArmManagerCollection,
					this.LoadArmManagerDisabledCollection
				};
				return fullList;
			}
		}

		public bool HasSwingArms
		{
			get
			{
				foreach (LoadArmManagerClass LoadArmManager in this.LoadArmManagerCollection)
				{
					if (LoadArmManager.LoadArm.SwingArm)
					{
						return true;
					}
				}

				return false;
			}
		}

		public int AvailableLoadArmManagers
		{
			get
			{
				int number = 0;
				foreach (LoadArmManagerClass loadArmManager in this.LoadArmManagerCollection)
				{
					if (this == loadArmManager.GetStationManager())
					{
						number++;
					}
				}

				return number;
			}
		}

		public bool IsRemoteAuthorized
		{
			get { return this.RemoteAuthorized; }
		}

		public int MESSAGE_TIMEOUT
		{
			get
			{
				return this.Station.StationMessageTimeout;
			}
		}

		public int NumberOfAuthorizedLoadArms
		{
			get
			{
				int NumberOfAuthorizedLoadArms = 0;
				foreach (LoadArmManagerClass LoadArmManager in this.LoadArmManagerCollection)
				{
					if (this == LoadArmManager.GetStationManager() && LoadArmManager.Bay(this).RecipeMap != 0)
					{
						NumberOfAuthorizedLoadArms++;
					}
				}
				return NumberOfAuthorizedLoadArms;
			}
		}

		public int PROMPT_TIMEOUT
		{
			get
			{
				return this.Station.StationPromptTimeout;
			}
		}

		public bool PreloadInProgress
		{
			get
			{
				return this.PreloadDataSet != null || this.IsRemoteAuthorized;
			}
		}

		protected LineItemDO CurrentLineItem
		{
			get
			{
				foreach (LineItemDO lineItem in this.Transaction.LineItems)
				{
					if (lineItem.DestinationEQ.RegistrationID == this.CurrentEquipment.ID
						 && lineItem.DestinationCompartmentID == this.CurrentCompartmentNumber.ToString())
					{
						return lineItem;
					}
				}

				return null;
			}
		}

		protected bool ProductsConfigured
		{
			get
			{
				foreach (LineItemDO lineItem in this.Transaction.LineItems)
				{
					if (lineItem.Product != null)
					{
						return true;
					}
				}
				return false;
			}
		}

		protected int TotalAvailableCompartments
		{
			get
			{
				return this.AvailableCompartments(this.TractorOrTanker) + this.AvailableCompartments(this.Trailer1)
							+ this.AvailableCompartments(this.Trailer2);
			}
		}

		protected int TotalCompartmentsInUseCurrentTransaction
		{
			get
			{
				return this.CompartmentsInUseCurrentTransaction(this.TractorOrTanker)
								 + this.CompartmentsInUseCurrentTransaction(this.Trailer1)
								 + this.CompartmentsInUseCurrentTransaction(this.Trailer2);
			}
		}

		protected virtual bool PromptForTractorOrTanker
		{
			get
			{
				return this.SiteManager.Site.PromptForTractorOrTanker;
			}
		}

		protected virtual bool PromptForFirstTrailer
		{
			get
			{
				return this.SiteManager.Site.PromptForFirstTrailer;
			}
		}

		protected virtual bool PromptForSecondTrailer
		{
			get
			{
				// ReSharper disable once ConvertPropertyToExpressionBody
				return this.SiteManager.Site.PromptForSecondTrailer
				&& (this.TractorOrTanker == null
				|| this.TractorOrTanker.Type == EQUIPMENT_TYPE.TRACTOR_TYPE)
				&& (!this.IsScheduledOrder
				|| this.TransportEquipmentOnOrder > 1);
			}
		}

		protected virtual bool PromptForThirdTrailer
		{
			get
			{
				return this.SiteManager.Site.PromptForThirdTrailer;
			}
		}

		public bool IsScheduledOrder
		{
			get
			{
				if (this.Order == null
				|| this.Order.CarrierCompanyGuid == Guid.Empty)
				{
					return false;
				}

				foreach (LineItemDO orderLineItem in this.Order.LineItems)
				{
					if (orderLineItem.DestinationEQ.EquipmentGuid == Guid.Empty)
					{
						return false;
					}

					try
					{
						// ReSharper disable once ReturnValueOfPureMethodIsNotUsed
						System.Convert.ToInt32(orderLineItem.DestinationCompartmentID);
					}
					catch
					{
						return false;
					}
				}

				return true;
			}
		}

		public virtual bool PromptForTransactionCompletion
		  {
				get
				{
					 if (!this.PreloadInProgress
					 && this.Transaction != null
					 && !this.Transaction.DeleteFlag
					 && (this.Transaction.TransPIDXCollection == null || this.Transaction.TransPIDXCollection.Count == 0)
				&& this.SiteManager.Site.PromptForTransactionCompletion)
					 {
						  if (this.Order == null)
						  {
								return true;
						  }

						  if (!this.IsScheduledOrder)
						  {
								foreach (LineItemDO orderLineItem in this.Order.LineItems)
								{
									 if (orderLineItem.Status == TransactionStatus.Completed)
									 {
										  continue;
									 }

									 double lineItemVolume = 0;

									 if (this.Transaction != null)
									 {
										  foreach (LineItemDO lineItem in this.Transaction.LineItems)
										  {
												if (orderLineItem.ProductGuid == lineItem.ProductGuid)
												{
													 if (this.SiteManager.Site.LoadByNet)
													 {
														  lineItemVolume += lineItem.Quantity.Net;
													 }
													 else
													 {
														  lineItemVolume += lineItem.Quantity.Gross;
													 }
												}
										  }

										  if (this.SiteManager.Site.LoadByNet)
										  {
												if (orderLineItem.Quantity.Net > lineItemVolume)
												{
													 return true;
												}
										  }
										  else
										  {
												if (orderLineItem.Quantity.Gross > lineItemVolume)
												{
													 return true;
												}
										  }
									 }
								}

								return false;
						  }

						  // Scheduled Order
						  foreach (LineItemDO orderLineItem in this.Order.LineItems)
						  {
								if (orderLineItem.Status == TransactionStatus.Completed)
								{
									 continue;
								}

								if (this.Transaction != null)
								{
									 bool orderLineItemComplete = false;
									 foreach (LineItemDO lineItem in this.Transaction.LineItems)
									 {
										  if (orderLineItem.DestinationEQ.EquipmentGuid == lineItem.DestinationEQ.EquipmentGuid
												&& orderLineItem.DestinationCompartmentID == lineItem.DestinationCompartmentID)
										  {
												orderLineItemComplete = true;
												break;
										  }
									 }

									 if (!orderLineItemComplete)
									 {
										  return true;
									 }
								}
						  }

						  return false;
					 }

					 return false;
				}
		  }

		private int TransportEquipmentOnOrder
		{
			get
			{
				int equipmentCount = 0;
				if (this.Order != null)
				{
					EquipmentDO[] equipmentArray = { this.Order.DestinationEQ1, this.Order.DestinationEQ2, this.Order.DestinationEQ3/*, this.Order.DestinationEQ4*/ };

					foreach (EquipmentDO equipment in equipmentArray)
					{
						if (equipment != null
						&& (equipment.EquipmentType == EquipmentTypeClass.TypeID(EQUIPMENT_TYPE.TANKER_TYPE)
						|| equipment.EquipmentType == EquipmentTypeClass.TypeID(EQUIPMENT_TYPE.TRAILER_TYPE)))
						{
							foreach (LineItemDO orderLineItem in this.Order.LineItems)
							{
								if (orderLineItem.Status == TransactionStatus.Completed)
								{
									continue;
								}

								if (orderLineItem.DestinationEQ.EquipmentGuid != equipment.EquipmentGuid)
								{
									continue;
								}

								if (this.Transaction != null)
								{
									bool orderLineItemComplete = false;
									foreach (LineItemDO lineItem in this.Transaction.LineItems)
									{
										if (orderLineItem.DestinationEQ.EquipmentGuid == lineItem.DestinationEQ.EquipmentGuid
										&& orderLineItem.DestinationCompartmentID == lineItem.DestinationCompartmentID)
										{
											orderLineItemComplete = true;
											break;
										}
									}

									if (!orderLineItemComplete)
									{
										equipmentCount++;
										break;
									}
								}
								else
								{
									equipmentCount++;
									break;
								}
							}
						}
					}
				}

				return equipmentCount;
			}
		}

		/// <summary>
		/// Returns the number of actual load arms on a preset.
		/// 
		/// In the general case, we use the number of load arm managers as a proxy.
		/// Non-preset devices that don't have load arms report 0
		/// </summary>
		internal virtual int PhysicalArmsOnPreset
		{
			get { return this.LoadArmManagerCollection?.Count ?? 0; }
		}

		protected void CheckDeviceStatus()
		{
			// Attempting to connect to in progress transaction is not currently supported.
			// This function only gets called on station startup; just clear transaction if it's not null,
			// set station to idle, and make sure the actual load computer gets reset.
			this.Transaction = null;
			this.StationState = StationState.IDLE;
			this.RecipeInternalNumberMap = new Dictionary<int, ProductMapClass>();
			this.LastDownloadedRecipe = 0;
			this.CancelUnauthorizedTransaction();
			this.ClearRecipes(true);
		}

		public virtual void CancelUnauthorizedTransaction()
		{
		}

		protected virtual bool IsTransactionInProgress()
		{
			foreach (LoadArmManagerClass loadArmManager in this.LoadArmManagerCollection)
			{
				if (this != loadArmManager.GetStationManager())
				{
					continue;
				}

				if (loadArmManager.IsTransactionInProgress())
				{
					return true;
				}
			}

			return false;
		}

		public void SystemEventsTimeChanged(object sender, EventArgs e)
		{
			this.SyncDateAndTime();
		}

		public virtual void Dispose()
		{
			if (!this.AlreadyDisposed)
			{
				if (this.KillEvent != null)
				{
					this.KillEvent.Set();
				}

				// Terminate the Scan Thread
				if (this.StationScanThread != null)
				{
					this.StationScanThread.Join();
				}

				this.Station.StationPermissives.Enabled = false;

				// The OPCServerManager can be null if the station did not create properly (e.g. error condition).  It then causes
				// NULL object reference errors when trying to disable the station to correct the problems.
				if (this.OPCServerManager != null)
				{
					this.OPCServerManager.Update(true);
				}

				while (this.LoadArmManagerCollection.Count != 0)
				{
					LoadArmManagerClass loadArmManager;
					// Lock so LoadArmManagerCollection isn't manipulated during removal
					Monitor.Enter(this);
					try
					{
						loadArmManager = this.LoadArmManagerCollection.Item(0);
						this.LoadArmManagerCollection.Remove(0);
					}
					finally
					{
						Monitor.Exit(this);
					}

					if (this == loadArmManager.GetStationManager())
					{
						loadArmManager.ReleaseKeyPad();
					}

					if (this.Station.SwingArmPosition == "A")
					{
						loadArmManager.BayA.StationManager = null;
					}
					else
					{
						loadArmManager.BayB.StationManager = null;
					}

					// Dispose of LoadArmManager when not in use by any station
					if (loadArmManager.BayA.StationManager == null && loadArmManager.BayB.StationManager == null)
					{
						loadArmManager.Dispose();
					}
				}

				while (this.LoadArmManagerDisabledCollection.Count != 0)
				{
					// Lock so LoadArmManagerCollection isn't manipulated during removal
					LoadArmManagerClass loadArmManager;
					Monitor.Enter(this);
					try
					{
						loadArmManager = this.LoadArmManagerDisabledCollection.Item(0);
						this.LoadArmManagerDisabledCollection.Remove(0);
					}
					finally
					{
						Monitor.Exit(this);
					}

					if (this.Station.SwingArmPosition == "A")
					{
						loadArmManager.BayA.StationManager = null;
					}
					else
					{
						loadArmManager.BayB.StationManager = null;
					}

					// Dispose of LoadArmManager when not in use by any station
					if (loadArmManager.BayA.StationManager == null && loadArmManager.BayB.StationManager == null)
					{
						loadArmManager.Dispose();
					}
				}

				OPCServerManagerClass opcServerManager = this.OPCServerManager;
				if (opcServerManager != null)
				{
					opcServerManager.Invoke -= this.OnInvoke;
					opcServerManager.Dispose();
				}

				GC.SuppressFinalize(this);
				this.AlreadyDisposed = true;
			}
		}

		public virtual void ResetStationDevice()
		{
			try
			{
				if (this.DeferredPurge || this.DeferredAdd)
				{
					this.SiteManager.PermissiveEvent.Set();
				}

				this.StationState = StationState.IDLE;
				this.Driver = null;

				this.RemoteAuthorized = false;
				this.PreloadDataSet = null;
				this.ConsecutivePrompts = 0;

				this.CurrentMenuParameters = null;
				this.PriorStockMessage = string.Empty;
				this.PriorResponseLength = 0;
				this.PriorMessageTimeout = 0;

				this.ClearRecipes(false);
			}
			catch (Exception e)
			{
				this.eventLog.WriteEntry("StationManager ResetStationDevice : " + e.Message, EventLogEntryType.Error);
			}
		}

		public void CreateLoadArmManager(LoadArmClass loadArm)
		{
			try
			{
				// If swing arm may already exist in another station
				LoadArmManagerClass loadArmManager = this.SiteManager.GetLoadArmManager(loadArm);

				if (loadArmManager != null)
				{
					if (this.Station.SwingArmPosition == "A")
					{
						loadArmManager.BayA.StationManager = this;
					}
					else
					{
						loadArmManager.BayB.StationManager = this;
					}
				}
				else
				{
					switch (loadArm.PresetType)
					{
						case PRESET_TYPE.MANUAL:
							loadArmManager = new ManualLoadArmManagerClass(this.eventLog, this.SiteManager, this, loadArm, this.Security);
							break;
						case PRESET_TYPE.ACCULOADIII_SA:
						case PRESET_TYPE.ACCULOADIII_S:
						case PRESET_TYPE.ACCULOADIII_Q:
							loadArmManager = new AcculoadIIILoadArmManagerClass(this.eventLog, this.SiteManager, this, loadArm, this.Security);
							break;
						case PRESET_TYPE.MICROLOAD_NET:
							loadArmManager = new MicroloadNetLoadArmManagerClass(this.eventLog, this.SiteManager, this, loadArm, this.Security);
							break;
						case PRESET_TYPE.DANLOAD6000:
							loadArmManager = new Danload6000LoadArmManagerClass(this.eventLog, this.SiteManager, this, loadArm, this.Security);
							break;
						case PRESET_TYPE.MULTILOAD_II_SMP:
							loadArmManager = new MultiloadIISMPLoadArmManagerClass(this.eventLog, this.SiteManager, this, loadArm, this.Security);
							break;
						case PRESET_TYPE.CONTREC1010_RA:
							loadArmManager = new Contrec1010RaLoadArmManagerClass(this.eventLog, this.SiteManager, this, loadArm, this.Security);
							break;
						case PRESET_TYPE.MULTILOAD_II:
							loadArmManager = new MultiloadIILoadArmManagerClass(this.eventLog, this.SiteManager, this, loadArm, this.Security);
							break;
						case PRESET_TYPE.VARECDET:
							loadArmManager = new VarecDETLoadArmManagerClass(this.eventLog, this.SiteManager, this, loadArm, this.Security);
							break;
						default:
							throw new Exception("Unsupported Load Arm Type " + LoadArmClass.PresetTypeID(loadArm.PresetType));
					}
				}

				// sijuan: it is time to clear alarms
				loadArmManager.ResetCommunicationsFailAlarm();
				loadArmManager.ResetPowerFailAlarm();

				// If the load arm is disabled, we want to load it into a separate collection so the station does not
				// consider the arm's properties when authorizing loading.  The separate collection is important so
				// meter closeout can include the arm in meter closeout records.
				if (loadArm.Enabled)
				{
					this.LoadArmManagerCollection.Add(loadArmManager);
				}
				else
				{
					this.LoadArmManagerDisabledCollection.Add(loadArmManager);
				}
			}
			catch (Exception e)
			{
				this.eventLog.WriteEntry("StationManager CreateLoadArmManager : " + e.Message + "\n" + e.StackTrace, EventLogEntryType.Error);
			}
		}

		protected void OnInvoke(ProcessVariableClass pv)
		{
			Monitor.Enter(this);

			try
			{
				if (pv == null)
				{
					throw new ArgumentNullException(nameof(pv));
				}

				switch (this.Station.Type)
				{
					case STATION_TYPE.ENTRY_GATE:
						this.EntryGateProcessing(pv);
						break;
					case STATION_TYPE.LOAD_RACK:
						this.LoadRackProcessing(pv);
						break;
					case STATION_TYPE.EXIT_GATE:
						this.ExitGateProcessing(pv);
						break;
					case STATION_TYPE.BOL:
						this.BolProcessing(pv);
						break;
					case STATION_TYPE.OFF_LOADING:
						this.LoadRackProcessing(pv);
						break;
					case STATION_TYPE.WEIGHT_SCALE:
						this.WeightScaleProcessing(pv);
						break;
					case STATION_TYPE.PRELOAD:
						this.PreloadProcessing(pv);
						break;
				}
			}
			catch (OpcException e)
			{
				this.eventLog.WriteEntry("StationManager OnInvoke :" + e.Message + "\n\n" + e.StackTrace, EventLogEntryType.Error);
				this.CommunicationsFailure = true;
			}
			catch (RuntimeWrappedException e)
			{
				this.eventLog.WriteEntry("Unknown Exception : StationManager OnInvoke" + e.Message + "\n\n" + e.StackTrace, EventLogEntryType.Error);
			}
			catch (Exception e)
			{
				this.eventLog.WriteEntry(e.ToString(), EventLogEntryType.Error);
				try
				{
					this.ResetStationDevice();
				}
				catch (Exception e1)
				{
					this.eventLog.WriteEntry(e1.ToString(), EventLogEntryType.Error);
				}
			}
			finally
			{
				Monitor.Exit(this);
			}
		}

		protected virtual void EntryGateProcessing(ProcessVariableClass pv)
		{
		}

		protected virtual void LoadRackProcessing(ProcessVariableClass pv)
		{
			switch (pv.ProcessVariableType)
			{
				case PROCESS_VARIABLE_TYPE.INPUT_PERMISSIVE_PV:
					{
						PermissivesClass permissives = pv.Parent;
						if (permissives == null)
						{
							break;
						}

						permissives.Update();

						this.OPCServerManager.Update(true);

						if (!pv.IsQualityGood || !(bool)pv.ServerValue)
						{
							this.LoadArmManagerCollection.IssuePermissiveMessage(this);
						}

						break;
					}

				default:
					this.eventLog.WriteEntry("StationManager OnInvoke : Unknown PV : " + pv.OPCItemID);
					break;
			}
		}

		protected virtual void ExitGateProcessing(ProcessVariableClass pv)
		{
		}

		protected virtual void BolProcessing(ProcessVariableClass pv)
		{
		}

		protected virtual void PreloadProcessing(ProcessVariableClass pv)
		{
		}

		protected virtual void WeightScaleProcessing(ProcessVariableClass pv)
		{
		}

		public virtual bool ProcessMeterRecircConfirmation(string response)
		{
			return false;
		}

		public virtual bool ProcessPromptForReturns(string response)
		{
			return false;
		}

		public virtual bool SendEndOfDayOrMonthWarningMessagesDuringLoading
		{
			get { return false; }
		}

		public virtual bool SetDensityInUnit(string density)
		{
			return true;
		}

		public virtual void SetUnloadPresetAmount(string response)
		{
		}

		public virtual void SetMeterRecircPresetAmount(string response)
		{
		}

		public virtual void ProcessPromptForSourceTank(string response)
		{
		}

		public virtual void ProcessMeterRecircAmount(string response)
		{
		}

		public virtual void ProcessPromptForDestinationTank(string response)
		{
		}

		public virtual int DisplayMessage(
			 string stockMessage,
			 string defaultResponse,
			 int responseLength,
			 int messageTimeout,
			 bool saveForCancelProcessing)
		{
			return 0;
		}

		public virtual int DisplayMessage(string stockMessage, string defaultResponse, int responseLength, int messageTimeout)
		{
			return this.DisplayMessage(stockMessage, defaultResponse, responseLength, messageTimeout, true);
		}

		protected void SaveMessageValues(string stockMessage, int responseLength, int messageTimeout)
		{
			this.CurrentMenuParameters = null;

			// Save the previous values for use by "cancel transaction" menu
			this.PriorStockMessage = stockMessage;
			this.PriorResponseLength = responseLength;
			this.PriorMessageTimeout = messageTimeout;
		}

		public virtual string AcknowledgementMessage
		{
			get { return "[LoadRack|Press Enter to Acknowledge]"; }
		}

		public virtual int AcknowledgementResponseLength
		{
			get { return 0; }
		}

		public virtual bool NumericMenuSelection
		{
			get { return false; }
		}

		public virtual void DisplayMenu(DisplayMenuParameters parameters)
		{
		}

		protected virtual void PromptForPin(string stockMessage, int responseLength, int messageTimeout)
		{
		}

		public void SyncDateAndTime()
		{
			try
			{
				this.LoadArmManagerCollection.SyncDateAndTime(this);
			}
			catch (Exception e)
			{
				this.eventLog.WriteEntry("StationManager + SyncDateAndTime : " + e.Message, EventLogEntryType.Error);
			}
		}

		public virtual string GetDateTimeSettingCommand()
		{
			return null;
		}

		protected virtual void ResetCardReaderData()
		{
		}

		public virtual void ReleaseKeyPad()
		{
		}

		protected virtual void OpenGate()
		{
		}

		protected virtual void ProcessSelectGate(string response)
		{
		}

		public virtual void ProcessMessageTimeout()
		{
			switch (this.StationState)
			{
				case StationState.DRIVER_MESSAGE_PROMPT:
					this.CheckDriverMessages(false);
					break;

				case StationState.PRODUCT_AVAILABILITY_MESSAGE:
					this.CheckProductAvailability(false);
					break;

				case StationState.OFFLOAD_PRODUCT_AVAILABILITY_MESSAGE:
					this.CheckOffloadProductAvailability(false);
					break;

				case StationState.PRODUCT_ALLOCATION_MESSAGE:
					this.CheckProductAllocations(false);
					break;

				case StationState.DRIVER_LICENSE_WARNING:
				case StationState.DRIVER_QUALIFICATION_WARNING:
				case StationState.COMPANY_CERTIFICATE_OR_PERMIT_WARNING:
				case StationState.COMPANY_LICENSE_WARNING:
				case StationState.COMPANY_INSURANCE_WARNING:
				case StationState.DRIVER_TRAINING_WARNING:
					this.CompleteDriverProcessing(false);
					break;

				case StationState.TRACTOR_TAG_OR_LICENSE_WARNING:
				case StationState.TRACTOR_TEST_OR_INSPECTION_WARNING:
					this.CompleteTractorOrTankerProcessing(false);
					break;

				case StationState.TRAILER1_TAG_OR_LICENSE_WARNING:
				case StationState.TRAILER1_TEST_OR_INSPECTION_WARNING:
					this.CompleteTrailer1Processing(false);
					break;

				case StationState.TRAILER2_TAG_OR_LICENSE_WARNING:
				case StationState.TRAILER2_TEST_OR_INSPECTION_WARNING:
					this.CompleteTrailer2Processing(false);
					break;

				case StationState.TRAILER3_TAG_OR_LICENSE_WARNING:
				case StationState.TRAILER3_TEST_OR_INSPECTION_WARNING:
					this.CompleteTrailer3Processing(false);
					break;

				case StationState.ENTRY_INSTRUCTION_PROMPT:
					this.CheckEntryInstructions(false);
					break;

				case StationState.EXIT_INSTRUCTION_PROMPT:
					this.CheckExitInstructions(false);
					break;

				case StationState.INVALID_COMPANY_ON_ORDER:
					if (this.SiteManager.Site.PromptForShipmentNumber)
					{
						this.IssueEnterShipmentNumberPrompt();
					}
					else
					{
						this.IssueEnterOrderNumberPrompt();
					}

					break;

				case StationState.INVALID_PRELOAD_ORDER_SELECTION_MSG:
					this.IssueSelectPreloadOrder();
					break;

				case StationState.INVALID_PRELOAD_LOADID_SELECTION_MSG:
					this.IssueSelectPreloadLoadID();
					break;

				case StationState.INVALID_PRELOAD_TYPE_SELECTION_MSG:
					this.IssueSelectPreloadBy();
					break;

				case StationState.INVALID_PRELOAD_DOCUMENT_SELECTION_MSG:
					this.IssueSelectPreloadDocument();
					break;

				case StationState.ENTER_DRIVER_ID_PROMPT:
					if (this.Station.CardReader)
					{
						this.IssuePleaseCardIn();
					}
					//else if (this.Station.TouchKeyReader)
					//{
					//	this.IssueTouchKeyPleaseCardIn();
					//}
					else
					{
						this.IssueDriverIDPrompt();
					}

					break;

				case StationState.NO_SHOPTO_MSG:
					this.IssueSelectCustomerShipToFilterColumnPrompt();
					break;

				case StationState.BUILD_PLAN:
					this.BuildPlan(false);
					break;

				case StationState.BUILD_RECIPE_MAP:
					this.BuildRecipeMapForAllLoadArms(false);
					break;

				case StationState.TRANSACTION_ALIAS_INVALID_MSG:
					this.IssueSelectProductPrompt();
					break;

				case StationState.NOT_AVAILABLE_ENTER_TRAILER1_PROMPT:
				case StationState.INVALID_ENTER_TRAILER1_PROMPT_RESPONSE_MESSAGE:
					this.IssueEnterTrailer1Prompt();
					break;

				case StationState.NOT_AVAILABLE_ENTER_TRAILER2_PROMPT:
				case StationState.INVALID_ENTER_TRAILER2_PROMPT_RESPONSE_MESSAGE:
					this.IssueEnterTrailer2Prompt();
					break;

				case StationState.NOT_AVAILABLE_ENTER_TRAILER3_PROMPT:
				case StationState.INVALID_ENTER_TRAILER3_PROMPT_RESPONSE_MESSAGE:
					this.IssueEnterTrailer3Prompt();
					break;

				case StationState.NOT_AVAILABLE_TRACTOR_OR_TANKER_PROMPT:
					this.IssueTractorOrTankerPrompt();
					break;

				case StationState.NOT_AVAILABLE_TRAILER1_PROMPT:
					this.IssueTrailer1Prompt();
					break;

				case StationState.NOT_AVAILABLE_TRAILER2_PROMPT:
					this.IssueTrailer2Prompt();
					break;

				case StationState.NOT_AVAILABLE_TRAILER3_PROMPT:
					this.IssueTrailer3Prompt();
					break;

				case StationState.NOT_AVAILABLE_SUMMARY_PROMPT:
					this.IssueLoadSummaryPrompt();
					break;

				case StationState.COMPANY_INVALID:
					this.ResetStationDevice();
					break;

				case StationState.NO_PRODUCTS_MSG:
					if (this.Order != null)
					{
						if (this.SiteManager.Site.PromptForShipmentNumber)
						{
							this.IssueEnterShipmentNumberPrompt();
						}
						else
						{
							this.IssueEnterOrderNumberPrompt();
						}
					}
					else
					{
						this.IssueLoadIDPrompt();
					}

					break;

				case StationState.OVERWEIGHT_MSG:
					if (this.SiteManager.Site.InhibitOverweightBOL)
					{
						this.StationState = StationState.IDLE;
					}
					else
					{
						this.IssueCaptureExitWeightPrompt(false);
					}

					break;

				case StationState.AUTHORIZING:
				case StationState.AUTHORIZED:
				case StationState.TRANSACTION_IN_PROGRESS:
					break;

				case StationState.BROKEN_BLEND:
				case StationState.BROKEN_BLEND_WEIGHTOUT:
					this.DisplayMessageWithAcknowledge("LoadRack|Broken Blend Detected.");
					break;

				case StationState.IMPROPER_ADDITIZATION:
				case StationState.IMPROPER_ADDITIZATION_WEIGHTOUT:
					this.DisplayMessageWithAcknowledge("LoadRack|Improper Additization Detected.");
					break;

				case StationState.INVALID_SHIPTO_PROMPT_RESPONSE_MESSAGE:
					this.IssueShipToMenu(false);
					break;

				case StationState.PROMPT_FOR_RETURNS:
					this.IssuePromptForReturnsPrompt();
					break;

				case StationState.INVALID_SUPPLIER_PROMPT_RESPONSE_MESSAGE:
					this.IssueVerifySupplierMenu();
					break;

				case StationState.INVALID_OFFLOAD_COMPLETE_TYPE_SELECTION_MSG:
					this.CheckForEndOfOffLoadingOperation();
					break;

				case StationState.MANUAL_OFFLOAD_INPROGRESS:
					this.ProcessManualMeterStopData(TimeoutString);
					break;

				case StationState.NO_SUPPLIER_MSG:
					this.IssueSelectSupplierOffLoadIDFilterColumnPrompt();
					break;

				case StationState.SELECT_DRIVER_COMPANY:
					this.DisplayMenu(this.CurrentMenuParameters);
					break;
				default:
					this.StationState = StationState.IDLE;
					break;
			}

			if (this.StationState == StationState.IDLE)
			{
				this.ResetStationDevice();
			}
		}

		public void ProcessResponseData(string response)
		{
			 if (response.Length > 0)
			 {
				  this.WriteLogDataToCommFile(response, CommLogDirection.In);
			 }

			switch (this.StationState)
			{
				case StationState.DRIVER_LICENSE_WARNING:
				case StationState.DRIVER_QUALIFICATION_WARNING:
				case StationState.COMPANY_CERTIFICATE_OR_PERMIT_WARNING:
				case StationState.COMPANY_LICENSE_WARNING:
				case StationState.COMPANY_INSURANCE_WARNING:
				case StationState.DRIVER_TRAINING_WARNING:
					this.CompleteDriverProcessing(true);
					break;

				case StationState.TRACTOR_TAG_OR_LICENSE_WARNING:
				case StationState.TRACTOR_TEST_OR_INSPECTION_WARNING:
					this.CompleteTractorOrTankerProcessing(true);
					break;

				case StationState.TRAILER1_TAG_OR_LICENSE_WARNING:
				case StationState.TRAILER1_TEST_OR_INSPECTION_WARNING:
					this.CompleteTrailer1Processing(true);
					break;

				case StationState.TRAILER2_TAG_OR_LICENSE_WARNING:
				case StationState.TRAILER2_TEST_OR_INSPECTION_WARNING:
					this.CompleteTrailer2Processing(true);
					break;

				case StationState.TRAILER3_TAG_OR_LICENSE_WARNING:
				case StationState.TRAILER3_TEST_OR_INSPECTION_WARNING:
					this.CompleteTrailer3Processing(true);
					break;

				case StationState.DRIVER_MESSAGE_PROMPT:
					this.CheckDriverMessages(true);
					break;

				case StationState.PRODUCT_AVAILABILITY_MESSAGE:
					this.CheckProductAvailability(true);
					break;

				case StationState.OFFLOAD_PRODUCT_AVAILABILITY_MESSAGE:
					this.CheckOffloadProductAvailability(true);
					break;

				case StationState.PRODUCT_ALLOCATION_MESSAGE:
					this.CheckProductAllocations(true);
					break;

				case StationState.ENTRY_INSTRUCTION_PROMPT:
					this.CheckEntryInstructions(true);
					break;

				case StationState.EXIT_INSTRUCTION_PROMPT:
					this.CheckExitInstructions(true);
					break;

				case StationState.PRODUCT_UNAVAILABLE_MESSAGE:
				case StationState.ADDITIVE_PROFILE_UNAVAILABLE_MESSAGE:
				case StationState.TANK_NOT_CERTIFIED_MSG:
				case StationState.FAILED_CERTIFICATE_OF_ANALYSIS_MSG:
					if (this.ByWeight)
					{
						if (this.AvailableCompartments(this.CurrentEquipment) > 1)
						{
							this.IssueCompartmentSummaryPrompt();
						}
						else
						{
							this.IssueLoadSummaryPrompt();
						}
					}
					else
					{
						this.IssuePresetPrompt();
					}
					break;

				case StationState.PIN_PROMPT:
					this.ProcessPIN(response);
					break;

				case StationState.ENTER_TRACTOR_PROMPT:
				case StationState.SELECT_TRACTOR_PROMPT:
				case StationState.TRACTOR_OR_TANKER_PROMPT:
					this.ProcessTractorOrTankerID(response);
					break;

				case StationState.ENTER_TRACTOR_CARDIN_PROMPT:
					this.ProcessTractorCardIn(response);
					break;

				case StationState.ENTER_TRAILER1_PROMPT:
				case StationState.SELECT_TRAILER1_PROMPT:
					this.ProcessTrailer1ID(response);
					break;

				case StationState.ENTER_TRAILER2_PROMPT:
				case StationState.SELECT_TRAILER2_PROMPT:
					this.ProcessTrailer2ID(response);
					break;

				case StationState.ENTER_TRAILER3_PROMPT:
				case StationState.SELECT_TRAILER3_PROMPT:
					this.ProcessTrailer3ID(response);
					break;

				case StationState.LOADID_PROMPT:
					this.ProcessLoadID(response);
					break;

				case StationState.VERIFY_SHIPTO_MSG:
					this.ProcessShipTo(response);
					break;

				case StationState.VERIFY_SHIPTO_MSG_PRELOAD:
					this.ProcessShipToShipmentNumberResponse(response);
					break;

				case StationState.PURCHASE_ORDER_PROMPT:
					this.ProcessPurchaseOrder(response);
					break;

				case StationState.ENTER_ORDER_PROMPT:
					this.ProcessOrder(response);
					break;

				case StationState.ENTER_CUSTOMER_SHIPTO_PROMPT:
					this.ProcessCustomerShipTo(response);
					break;

				case StationState.PRESET_PROMPT:
					this.ProcessPreset(response);
					break;

				case StationState.ENTER_ZIP_PROMPT:
					this.ProcessCustomerShipToFilterValue(response);
					break;

				case StationState.ENTER_DRIVER_ID_PROMPT:
					this.ProcessDriverID(response);
					break;

				case StationState.ENTER_SHIPMENT_NUMBER_PROMPT:
					this.ProcessShipmentNumber(response);
					break;

				case StationState.OPERATING_MODE_PROMPT:
					this.ProcessOperatingMode(response);
					break;

				case StationState.USE_ORDER_PROMPT:
					this.ProcessUseOrder(response);
					break;

				case StationState.SELECT_CUSTOMER_SHIPTO_FILTER_PROMPT:
					this.ProcessCustomerShipToFilterColumn(response);
					break;

				case StationState.SELECT_CUSTOMER_SHIPTO_FILTER_VALUE_PROMPT:
					this.ProcessCustomerShipToFilterValue(response);
					break;

				case StationState.SELECT_CUSTOMER_SHIPTO_PROMPT:
					this.ProcessCustomerShipTo(response);
					break;

				case StationState.SELECT_COMPANY_HIERARCHY_PROMPT:
					this.ProcessCompanyHierarchy(response);
					break;

				case StationState.SELECT_ORDER_PROMPT:
					this.ProcessOrder(response);
					break;

				case StationState.SUMMARY_PROMPT:
					switch (this.Mode)
					{
						case OperatingMode.Loading:
							this.ProcessLoadSummary(response);
							break;
						case OperatingMode.Unloading:
							this.ProcessOffloadSummary(response);
							break;
					}
					break;

				case StationState.COMPARTMENT_SUMMARY_PROMPT:
					this.ProcessCompartmentSummary(response);
					break;

				case StationState.PRODUCT_PROMPT:
					this.ProcessProduct(response);
					break;

				case StationState.OFFLOAD_PRODUCT_PROMPT:
					this.ProcessOffloadProduct(response);
					break;

				case StationState.CAPTURE_TARE_WEIGHT_PROMPT:
					this.ProcessTareWeight(response);
					break;

				case StationState.ADDITIONAL_ORDERS_PROMPT:
					this.ProcessAdditionalOrders(response);
					break;

				case StationState.CAPTURE_EXIT_WEIGHT_PROMPT:
					this.ProcessExitWeight(response);
					break;

				case StationState.PRELOAD_ORDER_PROMPT:
					this.ProcessPreloadOrderSelection(response);
					break;

				case StationState.PRELOAD_LOADID_PROMPT:
					this.ProcessPreloadLoadIDSelection(response);
					break;

				case StationState.PRELOAD_TYPE_PROMPT:
					this.ProcessPreloadSelectMethod(response);
					break;

				case StationState.PRELOAD_DOCNUMBER_PROMPT:
					this.ProcessPreloadDocumentSelection(response);
					break;

				case StationState.COMPARTMENTS_NOT_CONFIGURED:
					this.ProcessCompartmentsNotConfiguredPrompt();
					break;

				case StationState.BUILD_PLAN:
					this.BuildPlan(true);
					break;

				case StationState.BUILD_RECIPE_MAP:
					this.BuildRecipeMapForAllLoadArms(true);
					break;

				case StationState.ENTER_1ST_TRAILER_PROMPT:
					this.ProcessEnterTrailer1Prompt(response);
					break;

				case StationState.ENTER_2ND_TRAILER_PROMPT:
					this.ProcessEnterTrailer2Prompt(response);
					break;

				case StationState.ENTER_3RD_TRAILER_PROMPT:
					this.ProcessEnterTrailer3Prompt(response);
					break;

				case StationState.CANCEL_TRANSACTION_PROMPT:
					this.ProcessCancelTransactionPrompt(response);
					break;

				case StationState.OVERWEIGHT_MSG:
					if (this.SiteManager.Site.InhibitOverweightBOL)
					{
						this.StationState = StationState.IDLE;
					}
					else
					{
						this.IssueCaptureExitWeightPrompt(true);
					}

					break;

				case StationState.UPDATE_RECIPE_ERROR_MSG:
					this.StationState = StationState.IDLE;
					break;

				case StationState.UPDATE_DENSITY_ERROR_MSG:
					this.StationState = StationState.IDLE;
					break;

				case StationState.NO_PIDX_AUTHORIZATION_MSG:
					this.StationState = StationState.IDLE;
					break;

				case StationState.PIDX_UNAVAILABLE_MSG:
					this.StationState = StationState.IDLE;
					break;

				case StationState.PIDX_DENIAL_MSG:
					this.StationState = StationState.IDLE;
					break;

				case StationState.BROKEN_BLEND:
				case StationState.IMPROPER_ADDITIZATION:
					this.CompleteOrderProcessing();
					break;

				case StationState.BROKEN_BLEND_WEIGHTOUT:
				case StationState.IMPROPER_ADDITIZATION_WEIGHTOUT:
					this.CompleteOrderProcessingWeightOut();
					break;

				case StationState.SHIPMENTNUMBER_NOTFOUND:
					this.ProcessShipmentNumber(response);
					break;

				case StationState.SELECT_SHIPMENT_LOADID_PROMPT:
					this.ProcessShipmentLoadidResponse(response);
					break;

				case StationState.CONTAMINATION_PROMPT:
					this.ProcessContaminationPromptResponse(response);
					break;

				case StationState.COMPARTMENTS_PREVIOUSLY_LOADED_PROMPT:
					this.ProcessCompartmentsPreviouslyLoadedPromptResponse(response);
					break;

				case StationState.COMPARTMENTS_EMPTY_PROMPT:
					this.ProcessCompartmentsEmptyPromptResponse(response);
					break;

				case StationState.PROMPT_FOR_RETURNS:
					this.ProcessPromptForReturns(response);
					break;

				case StationState.ENTER_SUPPLY_ORDER_NUMBER:
				case StationState.ENTER_SUPPLY_ORDER_NUMBER_LIST:
					this.ProcessEnterSupplyOrderNumber(response);
					break;

				case StationState.SELECT_OFFLOAD_PRODUCT:
					this.ProcessOffLoadProductSelect(response);
					break;

				case StationState.ENTER_UNLOAD_DENSITY:
					this.ProcessUnloadDensity(response);
					break;

				case StationState.ENTER_UNLOAD_TEMPERATURE:
					this.ProcessUnloadTemperature(response);
					break;

				case StationState.ENTER_UNLOAD_AMOUNT:
					this.ProcessUnloadAmount(response);
					break;

				case StationState.ENTER_BOL_NUMBER:
					this.ProcessBolNumber(response);
					break;

				case StationState.PROMPT_FOR_RECIRC_CONFIMATION:
					this.ProcessMeterRecircConfirmation(response);
					break;

				case StationState.PROMPT_FOR_SOURCE_TANK:
					this.ProcessPromptForSourceTank(response);
					break;

				case StationState.PROMPT_FOR_DESTINATION_TANK:
					this.ProcessPromptForDestinationTank(response);
					break;

				case StationState.ENTER_METER_RECIRC_AMOUNT:
					this.ProcessMeterRecircAmount(response);
					break;

				case StationState.OFFLOADID_PROMPT:
					this.ProcessOffLoadID(response);
					break;

				case StationState.ENTER_MANUAL_METER_START_VALUE:
					this.ProcessManualMeterStartData(response);
					break;

				case StationState.MANUAL_OFFLOAD_INPROGRESS:
					this.ProcessOffLoadInProgress(response);
					break;

				case StationState.ENTER_MANUAL_METER_STOP_VALUE:
					this.ProcessManualMeterStopData(response);
					break;

				case StationState.VERIFY_OFFLOAD_SUPPLIER:
					this.ProcessVerifySupplier(response);
					break;

				case StationState.PROMPT_FOR_OFFLOAD_COMPLETE:
					this.ProcessOffLoadComplete(response);
					break;

				case StationState.USE_SUPPLYORDER_PROMPT:
					this.ProcessUseSupplyOrder(response);
					break;

				case StationState.SELECT_SUPPLIER_OFFLOADID_FILTER_PROMPT:
					this.ProcessSelectSupplierOffLoadIDFilterColumn(response);
					break;

				case StationState.ENTER_OFFLOADID_ZIP_PROMPT:
				case StationState.SELECT_SUPPLIER_FILTER_VALUE_PROMPT:
					this.ProcessSupplierOffLoadIDFilterValue(response);
					break;

				case StationState.SELECT_SUPPLIER_PROMPT:
					this.ProcessSupplierPrompt(response);
					break;

				case StationState.SELECT_DESTINATION_SUPPLIER_PROMPT:
					this.ProcessSupplierPrompt(response);
					break;

				case StationState.AUTHORIZED_PERMISSIVE_PROMPT:
				case StationState.INPROGRESS_PERMISSIVE_PROMPT:
					this.ProcessPermissiveMessageAcknowledge(response);
					break;

				case StationState.SELECT_DRIVER_COMPANY:
					this.ProcessSelectCarrierCompany(response);
					break;
				case StationState.SELECT_GATE:
					this.ProcessSelectGate(response);
					break;
			}

			if (this.StationState == StationState.IDLE)
			{
				this.ResetStationDevice();
			}
		}

		public void UpdateStationPermissives(bool enable)
		{
			if (this.Station.StationPermissives.Enabled != enable)
			{
				this.PermissiveEvent.Set();
			}
		}

		public int ActiveArms
		{
			get
			{
				int activeArms = 0;
				foreach (LoadArmManagerClass LoadArmManager in this.LoadArmManagerCollection)
				{
					if (this != LoadArmManager.GetStationManager())
					{
						continue;
					}

					if (LoadArmManager.LoadArmState == LOADARM_STATE.INPROGRESS
						 || LoadArmManager.LoadArmState == LOADARM_STATE.INPROGRESS_PERMISSIVE_PROMPT
						 || LoadArmManager.LoadArmState == LOADARM_STATE.END_BATCH_PROMPT
						 || LoadArmManager.LoadArmState == LOADARM_STATE.BATCH_STOPPED_PROMPT)
					{
						activeArms++;
					}
				}
				return activeArms;
			}
		}

		private void AssociateWithOrderLineItems(TransactionDO trans, TransactionDO order)
		{
			foreach (LineItemDO lineItem in trans.LineItems)
			{
				LineItemDO orderLineItem = this.FindMatchingOrderLineItem(order, lineItem.Product);

				if (orderLineItem != null)
				{
					lineItem.OrderReferenceTransactionLineItemGuid = orderLineItem.TransactionLineItemGuid;
				}
			}
		}

		public LineItemDO FindMatchingOrderLineItem(TransactionDO order, string product)
		{
			foreach (LineItemDO lineItem in order.LineItems)
			{
				if (lineItem.Product == product)
				{
					return lineItem;
				}
			}

			return null;
		}

		public void SaveTransaction()
		{
			this.SaveTransaction(true);
		}

		public void SaveTransaction(bool updateTransactionDateTime)
		{
			if (this.Transaction == null)
			{
				return;
			}

			this.Transaction.SubmittedToAccounting = true;

			if (!this.Transaction.DeleteFlag
				 && string.IsNullOrEmpty(this.Transaction.DocumentNumber))
			{
				for (int retry = 0; retry < 3; retry++)
				{
					try
					{
						this.Transaction.DocumentNumber = this.GetNextDocumentNumberForSite(
							this.Security, DOCUMENT_TYPE.AUTOMATIC_BOL, this.Security.SiteGuid);
						break;
					}
					catch (Exception sitesError)
					{
						this.eventLog.WriteEntry(sitesError.Message, EventLogEntryType.Error);
						if (retry == 2)
						{
							throw new Exception("Station Manager SaveTransaction : Max Retries GetNextDocumentNumber");
						}
					}
				}
			}

			if (this.Transaction.TransPIDXCollection == null)
			{
				this.SetPIDXAuthorizations();
			}

			if (string.IsNullOrEmpty(this.Transaction.Alias))
			{
				if (this.Order != null)
				{
					this.OrderTransactionAlias =
						FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasClass>(
							x => x.Get(this.Security, this.Order.TransactionAliasGuid, false));

					this.CurrentTransactionAlias =
						FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasClass>(
							x => x.Get(this.Security, this.OrderTransactionAlias.AssociatedTransactionAliasGuid, false));

					this.Transaction.Alias = this.CurrentTransactionAlias.ID;
					this.Transaction.TransactionAliasGuid = this.CurrentTransactionAlias.MasterRecordGuid;
					this.Transaction.TransTypeID = this.CurrentTransactionAlias.TransTypeID;
					this.TransactionSupportsMultipleLineItems = this.CurrentTransactionAlias.MultipleLineItems;

					// Save the order reference id as well
					this.Transaction.TransRefID = this.Order.TransID;

					// Each line item of the transaction has to reference an Order line item
					this.AssociateWithOrderLineItems(this.Transaction, this.Order);
				}
				else if (this.SupplyOrder != null)
				{
					this.SupplyOrderTransactionAlias =
						FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasClass>(
							x => x.Get(this.Security, this.SupplyOrder.TransactionAliasGuid, false));

					this.CurrentTransactionAlias =
						FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasClass>(
							x => x.Get(this.Security, this.SupplyOrderTransactionAlias.AssociatedTransactionAliasGuid, false));

					this.Transaction.Alias = this.CurrentTransactionAlias.ID;
					this.Transaction.TransactionAliasGuid = this.CurrentTransactionAlias.MasterRecordGuid;
					this.Transaction.TransTypeID = this.CurrentTransactionAlias.TransTypeID;
					this.TransactionSupportsMultipleLineItems = this.CurrentTransactionAlias.MultipleLineItems;

					// Save the Supply order reference id as well
					this.Transaction.TransRefID = this.SupplyOrder.TransID;

					// Each line item of the transaction has to reference an Order line item
					this.AssociateWithOrderLineItems(this.Transaction, this.SupplyOrder);
				}
				else
				{
					if (this.InRecircMode) // meter recirc or proving
					{
						this.Transaction.Alias = this.Station.RecircTransactionAliasID;
						this.Transaction.TransactionAliasGuid = this.Station.RecircTransactionAliasGuid;
						this.Transaction.TransTypeID = TransactionTypes.T23_StorageTransfer;
					}
					else if (this.Station.Type == STATION_TYPE.WEIGHT_SCALE || this.Station.Type == STATION_TYPE.PRELOAD)
					{
						if (this.Mode == OperatingMode.Loading)
						{
							if (!this.ByWeight)
							{
								this.Transaction.Alias = this.Station.IssueByVolumeTransactionAliasID;
								this.Transaction.TransactionAliasGuid = this.Station.IssueByVolumeTransactionAliasGuid;
							}
							else
							{
								this.Transaction.Alias = this.Station.IssueByWeightTransactionAliasID;
								this.Transaction.TransactionAliasGuid = this.Station.IssueByWeightTransactionAliasGuid;
							}

							this.Transaction.TransTypeID = TransactionTypes.T5_PrimaryDisbursement;
						}
						else
						{
							if (!this.ByWeight)
							{
								this.Transaction.Alias = this.Station.ReceiptByVolumeTransactionAliasID;
								this.Transaction.TransactionAliasGuid = this.Station.ReceiptByVolumeTransactionAliasGuid;
							}
							else
							{
								this.Transaction.Alias = this.Station.ReceiptByWeightTransactionAliasID;
								this.Transaction.TransactionAliasGuid = this.Station.ReceiptByWeightTransactionAliasGuid;
							}
							this.Transaction.TransTypeID = TransactionTypes.T8_Receipt;
						}
					}
					else if (this.Station.Type == STATION_TYPE.OFF_LOADING)
					{
						this.Transaction.Alias = this.Station.ReceiptByVolumeTransactionAliasID;
						this.Transaction.TransactionAliasGuid = this.Station.ReceiptByVolumeTransactionAliasGuid;
						this.Transaction.TransTypeID = TransactionTypes.T8_Receipt;
					}
					else
					{
						this.Transaction.Alias = this.Station.IssueByVolumeTransactionAliasID;
						this.Transaction.TransactionAliasGuid = this.Station.IssueByVolumeTransactionAliasGuid;
						this.Transaction.TransTypeID = TransactionTypes.T5_PrimaryDisbursement;
					}

					this.CurrentTransactionAlias =
						FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasClass>(
							x => x.Get(this.Security, this.Transaction.TransactionAliasGuid, false));

					this.TransactionSupportsMultipleLineItems = this.CurrentTransactionAlias.MultipleLineItems;
				}

				if (this.Transaction.TransactionAliasGuid == Guid.Empty)
				{
					throw new Exception("LoadRack|Transaction Alias Invalid");
				}

				UnitsHelperClass unitHelper = new UnitsHelperClass(this.Security, this.SiteManager.Site, this.CurrentTransactionAlias, null);
				unitHelper.SetUnits(this.Transaction, 0);
			}

			DateTimeOffset siteTimeNow = TimeConverter.Now(this.SiteManager.Site);

			if (updateTransactionDateTime)
			{
				this.Transaction.TransactionDateTime = siteTimeNow;
			}

			this.Transaction.UpdatedDate = DateTimeOffset.Now;
			if (this.Transaction.TimeOut == null)
			{
				this.Transaction.TimeOut = siteTimeNow;
			}

			// Don't save a deleted transaction with no TransPIDXCollection
			if (this.Transaction.TransPIDXCollection == null && this.Transaction.DeleteFlag)
			{
				return;
			}

			SaveTransactionsSR saveTransactionsSR = new SaveTransactionsSR
			{
				Security = this.Security,
				ConvertUnits = true,
				UseAutoComplete = true,
				CurrentSiteGuid = this.Security.SiteGuid,
				BOLFromLoadRackFlag = true
			};
			saveTransactionsSR.Transactions.Add(this.Transaction);

			for (int retry = 0; retry < 3; retry++)
			{
				try
				{
					if (this.Transaction.Status == TransactionStatus.InProgress)
					{
						this.Security.EnableChangeTracking = false;
					}

					this.SaveTransactionProcessor(saveTransactionsSR);
					if (this.Transaction.TransactionGuid == Guid.Empty)
					{
						// Need to load the Transaction Guid back.
						TransactionSR sr = new TransactionSR { Security = this.Security, TransID = this.Transaction.TransID };
						TransactionDO trans = FMChannelHelper.MakeCall<ITransactionProcessor, TransactionDO>(x => x.Process(sr));
						this.Transaction.TransactionGuid = trans.TransactionGuid;
					}

					this.UpdateTransactionCounter = TransactionUpdateInterval;
					break;
				}
				catch (Exception e)
				{
					FaultException<SaveTransactionsException> exception = e as FaultException<SaveTransactionsException>;
					if (exception != null)
					{
						FaultException<SaveTransactionsException> saveTransactionsException = exception;
						if (saveTransactionsException.Detail.Results.Count >= 1
							 && saveTransactionsException.Detail.Results[0] != null
							 && saveTransactionsException.Detail.Results[0].ErrorList.Count >= 1)
						{
							this.eventLog.WriteEntry(saveTransactionsException.Detail.Results[0].ErrorList[0], EventLogEntryType.Error);
						}

						else
						{
							this.eventLog.WriteEntry("Unknown SaveTransactionException", EventLogEntryType.Error);
						}
					}
					else
					{
						this.eventLog.WriteEntry("StationManager SaveTransaction : " + e.Message, EventLogEntryType.Error);
					}

					if (retry < 2)
					{
						continue;
					}

					if (this.Station.Type == STATION_TYPE.LOAD_RACK || this.Station.Type == STATION_TYPE.OFF_LOADING)
					{
						try
						{
							this.Unauthorize();
						}
						catch (Exception e1)
						{
							this.eventLog.WriteEntry(e1.Message, EventLogEntryType.Error);
						}
					}

					this.Transaction = null;
					this.StationState = StationState.RESET_ON_TIMEOUT;
					this.DisplayMessage("LoadRack|Save Transaction Failure", null, 0, this.MESSAGE_TIMEOUT);
					break;
				}
				finally
				{
					this.Security.EnableChangeTracking = true;
				}
			}
		}

		public void SaveArbitraryTransaction(TransactionDO transaction)
		{
			if (transaction == null)
			{
				return;
			}

			Logger logger = new Logger(StopWatch.Appnames.LoadRackService.ToString());
			logger.Debug("StationManagerClass.SaveTransaction - saving transaction " + this.Transaction.TransID + ":" + this.Transaction.DocumentNumber + " as " + this.Transaction.Status);
			SaveTransactionsSR saveTransactionsSR = new SaveTransactionsSR
			{
				Security = this.Security,
				ConvertUnits = true,
				CurrentSiteGuid = this.Security.SiteGuid,
				BOLFromLoadRackFlag = true
			};
			saveTransactionsSR.Transactions.Add(transaction);

			for (int retry = 0; retry < 3; retry++)
			{
				try
				{
					FMChannelHelper.MakeCall<ISaveTransactionsProcessor, SaveTransactionsResultDO>(x => x.SaveTransactions(saveTransactionsSR));
					break;
				}
				catch (Exception e)
				{
					SaveTransactionsException se = e as SaveTransactionsException;
					if (se != null)
					{
						if (se.Results.Count >= 1 && se.Results[0] != null
							 && se.Results[0].ErrorList.Count >= 1)
						{
							this.eventLog.WriteEntry(se.Results[0].ErrorList[0], EventLogEntryType.Error);
						}
						else
						{
							this.eventLog.WriteEntry("Unknown SaveTransactionException", EventLogEntryType.Error);
						}
					}
					else
					{
						this.eventLog.WriteEntry("StationManager SaveTransaction : " + e.Message, EventLogEntryType.Error);
					}

					if (retry < 2)
					{
						continue;
					}

					break;
				}
			}
		}

		/// <summary>
		/// Iterates through the line items of the transaction, then tries to ensure
		/// that the load arm assigned to the line item is still servicing this station.
		/// If so, then update the particular line item.
		/// </summary>
		/// <returns>
		/// <see cref="bool"/>True if at least one line item is updated; false otherwise.
		/// </returns>
		private bool UpdateLineItems()
		{
			if (this.Transaction == null)
			{
				return false;
			}

			bool save = false;

			foreach (LineItemDO lineItem in this.Transaction.LineItems)
			{
				if (lineItem.Status != TransactionStatus.InProgress)
				{
					continue;
				}

				if (lineItem.ArmNumber == null)
				{
					continue;
				}

				LoadArmClass loadArm = this.Station.LoadArmCollection[lineItem.ArmNumber.Value - 1];
				LoadArmManagerClass loadArmManager = this.GetLoadArmManager(loadArm);
				if (loadArmManager.GetStationManager() != this)
				{
					continue;
				}

				this.UpdateLineItem(lineItem);
				save = true;
			}

			return save;
		}

		private void StationScan()
		{
			try
			{
				this.LastScanDateTime = DateTimeOffset.Now;

				WaitHandle[] events =
					{
						this.KillEvent, this.PermissiveEvent, this.DownloadConfigurationEvent,
						this.UploadStoredTransactionsEvent
					};

				int waitResult;
				while (0 != (waitResult = WaitHandle.WaitAny(events, 1000, true)))
				{
					Monitor.Enter(this);
					try
					{
						switch (waitResult)
						{
							// PermissiveEvent
							case 1:
								{
									if (this.InRecircMode == false && this.Station.Type == STATION_TYPE.LOAD_RACK)
									{
										if (this.Station.StationPermissives.Enabled != this.SiteManager.StationPermissive)
										{
											this.Station.StationPermissives.Enabled = this.SiteManager.StationPermissive;

											this.OPCServerManager.Update(true);

											if (!this.SiteManager.StationPermissive)
											{
												this.LoadArmManagerCollection.StopIfInProgress(this);
												this.LoadArmManagerCollection.IssuePermissiveMessage(this);
											}
										}
									}

									break;
								}

							case 2: //DownloadConfigurationEvent
								{
									if ((this.Station.Type == STATION_TYPE.LOAD_RACK || this.Station.Type == STATION_TYPE.OFF_LOADING)
										 && this.Station.InterfaceType == STATION_INTERFACE_TYPE.CONTREC1010_RA)
									{
										this.DownloadConfigurationData();
									}
									this.DownloadConfigurationEvent.Reset();
									break;
								}

							case 3: //UploadStoredTransactionsEvent
								{
									if ((this.Station.Type == STATION_TYPE.LOAD_RACK || this.Station.Type == STATION_TYPE.OFF_LOADING)
										 && this.Station.InterfaceType == STATION_INTERFACE_TYPE.CONTREC1010_RA)
									{
										this.CheckAndUploadStoreTransactions();
									}
									this.UploadStoredTransactionsEvent.Reset();
									break;
								}
							case WaitHandle.WaitTimeout:
								{
									if (this.InRecircMode == false && (this.Station.Type == STATION_TYPE.LOAD_RACK || this.Station.Type == STATION_TYPE.OFF_LOADING))
									{
										// Evaluate MaxIdleTime and MaxLoadTime
										if (this.Transaction != null
											 && (this.StationState == StationState.AUTHORIZING
													|| this.StationState == StationState.AUTHORIZED
													|| this.StationState == StationState.TRANSACTION_IN_PROGRESS))
										{
											if (this.LoadArmManagerCollection.CheckAvailableVolume(this))
											{
												this.SaveTransaction();
											}

											if (DateTimeOffset.Now - this.LastScanDateTime > new TimeSpan(0, 0, 60))
											{
												this.StartDateTime = DateTimeOffset.Now;
												this.LastActivityDateTime = DateTimeOffset.Now;
											}
											else
											{
												if (DateTimeOffset.Now - this.LastActivityDateTime
													 > new TimeSpan(0, this.SiteManager.Site._MaximumIdleTime, 0))
												{
													if (this.Transaction != null)
													{
														this.Unauthorize();

														this.AddAlarmAndEventLogs(this.Security, this.Station.MaximumIdleTimeAlarm(this.Transaction.TransID));

														this.CompleteTransaction();
														this.ResetStationDevice();
													}
												}

												if ((this.StartDateTime != DateTimeOffset.MinValue) 
													&& (DateTimeOffset.Now - this.StartDateTime > new TimeSpan(0, this.SiteManager.Site._MaximumLoadTime, 0)))
												{
													this.Unauthorize();

													this.AddAlarmAndEventLogs(this.Security, this.Station.MaximumLoadTimeAlarm(this.Transaction.TransID));

													this.CompleteTransaction();
													this.ResetStationDevice();
												}
											}
										}
									}

									if ((this.InRecircMode == false && this.Station.Type == STATION_TYPE.LOAD_RACK)
										 || this.Station.Type == STATION_TYPE.OFF_LOADING)
									{
										if (--this.UpdatePermissivesCounter == 0)
										{
											this.OPCServerManager.Update(false);

											if (this.Station.Type == STATION_TYPE.OFF_LOADING
												 && this.Station.InterfaceType == STATION_INTERFACE_TYPE.VAREC_DET)
											{
												this.OPCServerManager.Update(false);
											}
											else
											{
												this.LoadArmManagerCollection.RefreshPermissives(this);
											}

											this.UpdatePermissivesCounter = PermissivesUpdateInterval;
										}

										if (--this.UpdateTransactionCounter == 0)
										{
											this.UpdateTransactionCounter = TransactionUpdateInterval;
											if (this.Transaction != null)
											{
												if (this.UpdateLineItems())
												{
													this.SaveTransaction();
												}
											}
										}
									}

									this.LastScanDateTime = DateTimeOffset.Now;

									break;
								} // end
						}
					}
					catch (Exception e)
					{
						this.eventLog.WriteEntry("StationManager StationScan : " + e.Message + "\n" + e.StackTrace, EventLogEntryType.Error);
					}
					finally
					{
						Monitor.Exit(this);
					}
				}

				// On shutdown terminate any active transaction
				this.ShuttingDown = true;
				if (this.ActiveArms == 0
					 && (this.StationState == StationState.AUTHORIZING || this.StationState == StationState.AUTHORIZED
						  || this.StationState == StationState.TRANSACTION_IN_PROGRESS))
				{
					if (this.Transaction != null)
					{
						FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
							x => x.Add(this.Security, this.Station.ShutdownAlarm(this.Transaction.TransID)));
					}

					this.LoadArmManagerCollection.Stop(this);
					this.Unauthorize();
					this.CompleteTransaction();
				}

				try
				{
					if (this.Station.CardReader)
					{
						this.ResetCardReaderData();
					}
				}
				catch (Exception e)
				{
					this.eventLog.WriteEntry("StationManager StationScan : " + e.Message, EventLogEntryType.Error);
				}
			}
			catch (Exception e)
			{
				this.eventLog.WriteEntry(e.ToString(), EventLogEntryType.Error);
			}
		}

		public virtual void IssueDriverIDPrompt()
		{
			this.Driver = null;
			this.DisplayMessage("[LoadRack|Enter Driver ID", null, 10, 999);
			this.PriorStationState = StationState.IDLE;
			this.StationState = StationState.ENTER_DRIVER_ID_PROMPT;
		}

		public void IssueEnterShipmentNumberPrompt()
		{
			this.ConsecutivePrompts = 0;
			this.DisplayMessage("[LoadRack|Enter Shipment Number", null, 10, this.PROMPT_TIMEOUT);
			this.PriorStationState = StationState.IDLE;
			this.StationState = StationState.ENTER_SHIPMENT_NUMBER_PROMPT;
		}

		public void IssuePromptForReturnsPrompt()
		{
			this.ConsecutivePrompts = 0;
			this.DisplayMenu(
				 new DisplayMenuParameters(
					  "LoadRack|Any Returns?",
					  new[] { "LoadRack|Yes", "LoadRack|No" },
					  true,
					  -1,
			  this.PROMPT_TIMEOUT));
			this.StationState = StationState.PROMPT_FOR_RETURNS;
		}

		protected LoadArmClass GetLoadArm(Guid identityGuid)
		{
			foreach (LoadArmClass LoadArm in this.Station.LoadArmCollection)
			{
				if (LoadArm.IdentityGuid == identityGuid)
				{
					return LoadArm;
				}
			}

			return null;
		}

		public LoadArmManagerClass GetLoadArmManager(LoadArmClass loadArm)
		{
			Monitor.Enter(this);

			try
			{
				foreach (LoadArmManagerClass loadArmManager in this.LoadArmManagerCollection)
				{
					if (loadArm.IdentityGuid == loadArmManager.LoadArm.IdentityGuid)
					{
						return loadArmManager;
					}
				}

				foreach (LoadArmManagerClass loadArmManager in this.LoadArmManagerDisabledCollection)
				{
					if (loadArm.IdentityGuid == loadArmManager.LoadArm.IdentityGuid)
					{
						return loadArmManager;
					}
				}
			}
			finally
			{
				Monitor.Exit(this);
			}

			return null;
		}

		protected virtual byte ReadAdditiveProductsUsingInjector(
			ProductMapClass AdditiveInjector, Server Server, LoadArmManagerClass LoadArmManager)
		{
			return 0xFF;
		}

		protected int ReadBatchRecipe(LineItemDO LineItem, Server Server, LoadArmManagerClass LoadArmManager)
		{
			if (LoadArmManager == null)
			{
				throw new ArgumentNullException("LoadArmManager");
			}

			LoadArmClass LoadArm = LoadArmManager.LoadArm;

			ItemValueResult BatchRecipe;

			LoadArmManager.ReadBatchRecipe(LineItem.BatchNumber, Server, out BatchRecipe);

			if (BatchRecipe.Quality != Quality.Good)
			{
				throw new Exception("ReadBatchRecipe : Batch Recipe OPC Quality Bad " + BatchRecipe);
			}

			else
			{
				// Get Product
				if (LineItem.ProductGuid == Guid.Empty)
				{
					int Index = 0;

					if (this.Station.Type == STATION_TYPE.LOAD_RACK)
					{
						try
						{
							ProductMapClass batchRecipeToArmMap = this.RecipeInternalNumberMap[(int)BatchRecipe.Value];
							LoadArmManager.CurrentLineItemProduct = this.GetByProductAuthorizedCompanies(this.Security, batchRecipeToArmMap.AssignedGuid, false);
							//foreach (ProductMapClass Recipe in LoadArm.ProductRecipeCollection)
							//{
							//	if (LoadArmManager.GetRecipeNumber(Recipe) == (int)BatchRecipe.Value)
							//	{
							//		LoadArmManager.CurrentLineItemProduct = this.GetByProductAuthorizedCompanies(
							//			this.Security, Recipe.AssignedGuid, false);
							//		break;
							//	}
							//}
						}
						catch (KeyNotFoundException knfe)
						{
							_ = knfe;
							LoadArmManager.CurrentLineItemProduct = null;

							this.eventLog.WriteEntry($"Error reading recipe for a given preset# {(int)BatchRecipe.Value}", EventLogEntryType.Error);
						}
					}
					else
					{
						// Expect this to be an OFFLOAD station
						foreach (ProductMapClass Recipe in LoadArm.ProductRecipeCollection)
						{
							if (LoadArmManager.GetRecipeNumber(Recipe) == (int)BatchRecipe.Value)
							{
								LoadArmManager.CurrentLineItemProduct = this.GetByProductAuthorizedCompanies(
									this.Security, Recipe.AssignedGuid, false);
								break;
							}
						}
					}

                    if (LoadArmManager.CurrentLineItemProduct == null)
					{
						throw new Exception("Batch Recipe not found in LoadArm Recipe Configuration");
					}

					UnitsHelperClass UnitsHelper = new UnitsHelperClass(
						this.Security, this.SiteManager.Site, this.CurrentTransactionAlias, LoadArmManager.CurrentLineItemProduct);

					LineItem.ProductType = ProductClass.ProductTypeID(LoadArmManager.CurrentLineItemProduct.ProductType);
					LineItem.ProductCode = LoadArmManager.CurrentLineItemProduct.Code;
					LineItem.Product = LoadArmManager.CurrentLineItemProduct.ID;
					LineItem.ProductGuid = LoadArmManager.CurrentLineItemProduct.MasterRecordGuid;
					UnitsHelper.SetUnits(LineItem, 0, null);

					if (LoadArmManager.CurrentLineItemProduct.ProductType == ProductType.ComponentProduct)
					{
						foreach (ProductMapClass Component in LoadArm.ComponentCollection)
						{
							if (Component.AssignedGuid == LoadArmManager.CurrentLineItemProduct.MasterRecordGuid)
							{
								LineItem.LineNumber = Component.PresetNumber;
								TankClass Tank = this.SiteManager.GetTank(Component, this.Manager);
								if (Tank != null)
								{
									LineItem.StorageLocationID = Tank.ID;
									LineItem.StorageLocationTankGuid = Tank.IdentityGuid;
								}
								break;
							}
							Index++;
						}

						if (Index == LoadArm.ComponentCollection.Count)
						{
							throw new Exception("Component not found in LoadArm Configuration");
						}
					}
				}
			}
			return (int)BatchRecipe.Value;
		}

		protected void ReadPresetAmount(LineItemDO lineItem, Server server, LoadArmManagerClass loadArmManager)
		{
			if (loadArmManager == null)
			{
				throw new ArgumentNullException(nameof(loadArmManager));
			}

			ItemValueResult presetAmount;

			loadArmManager.ReadPresetAmount(server, out presetAmount);

			if (lineItem.PresetAmount == null)
			{
				lineItem.PresetAmount = 0.0;
			}

			try
			{
				ProcessVariableClass pv = new ProcessVariableClass();
				if (presetAmount.Quality != Quality.Good)
				{
					if (!lineItem.PresetAmount_BadQualityLogged)
					{
						this.eventLog.WriteEntry(
							"ReadPresetAmount : Preset Amount OPC Quality Bad " + presetAmount.ItemName, EventLogEntryType.Error);
						lineItem.PresetAmount_BadQualityLogged = true;
					}
				}
				else
				{
					EngineeringUnit units = lineItem.VolumeUnits;
					byte decimalPlaces = lineItem.VolumeDecimalPlaces;

					pv.ProcessVariableType = PROCESS_VARIABLE_TYPE.GROSS_VOLUME_PV;
					pv.ServerUnits = units; //sijuan: Preset unit should match BOL unit
					pv.DataType = VarEnum.VT_R8;
					pv.ServerValue = System.Convert.ToDouble(presetAmount.Value);
					lineItem.PresetAmount = (double)pv.GetValue(units, decimalPlaces);
					lineItem.PresetAmount_BadQualityLogged = false;
				}
			}
			catch (Exception e)
			{
				this.eventLog.WriteEntry("ReadPresetAmount : Exception reading preset amoutn: " + e.Message + e.StackTrace, EventLogEntryType.Error);
			}
		}

		public virtual void ReadWeight(out ItemValueResult weight, out bool weightScaleInMotion, out bool weightScaleMotionReadingInValid)
		{
			weightScaleInMotion = false;
			weightScaleMotionReadingInValid = false;
			weight = new ItemValueResult();
		}

		public EquipmentClass GetEquipmentClass(string EquipmentID)
		{
			if (this.TractorOrTanker != null && this.TractorOrTanker.Type == EQUIPMENT_TYPE.TANKER_TYPE
				 && ((!this.SiteManager.Site.UseCompanyEquipmentIdentifiers && this.TractorOrTanker.ID == EquipmentID)
					  || (this.SiteManager.Site.UseCompanyEquipmentIdentifiers
							&& this.TractorOrTanker.CompanyEquipmentID == EquipmentID)))
			{
				return this.TractorOrTanker;
			}

			if (this.Trailer1 != null
				 && ((!this.SiteManager.Site.UseCompanyEquipmentIdentifiers && this.Trailer1.ID == EquipmentID)
					  || (this.SiteManager.Site.UseCompanyEquipmentIdentifiers && this.Trailer1.CompanyEquipmentID == EquipmentID)))
			{
				return this.Trailer1;
			}

			if (this.Trailer2 != null
				 && ((!this.SiteManager.Site.UseCompanyEquipmentIdentifiers && this.Trailer2.ID == EquipmentID)
					  || (this.SiteManager.Site.UseCompanyEquipmentIdentifiers && this.Trailer2.CompanyEquipmentID == EquipmentID)))
			{
				return this.Trailer2;
			}

			return null;
		}

		public EquipmentClass GetCompartment(EquipmentClass equipment, int compartment)
		{
			EquipmentTypeClass equipmentType = FMChannelHelper.MakeCall<IEquipmentTypes, EquipmentTypeClass>(x => x.Get(this.Security, equipment.EquipmentTypeGuid));
			if (!equipmentType.IsMultiCompartment
			&& compartment == 1)
			{
				return equipment;
			}

			if (equipment.CompartmentCollection.Count < compartment)
			{
				return null;
			}

			return equipment.CompartmentCollection[compartment - 1];
		}

		public LineItemDO GetLineItem(Guid loadArmGuid)
		{
			if (this.Transaction == null)
			{
				return null;
			}

			int armNumber = 1;
			foreach (LoadArmClass loadArm in this.Station.LoadArmCollection)
			{
				if (loadArm.IdentityGuid == loadArmGuid)
				{
					break;
				}

				armNumber++;
			}

			foreach (LineItemDO lineItem in this.Transaction.LineItems)
			{
				if (lineItem.Status != TransactionStatus.InProgress)
				{
					continue;
				}

				if (lineItem.ArmNumber == null || (lineItem.ArmNumber.Value != armNumber))
				{
					continue;
				}

				return lineItem;
			}

			return null;
		}

		public virtual void ReadLineItemData(
			LineItemDO lineItem,
			Server server,
			LoadArmManagerClass loadArmManager)
		{
		}

		public void CreateBlendSubLineItems(LineItemDO lineItem, ProductClass lineItemProduct, LoadArmManagerClass loadArmManager, Server server)
		{
			foreach (ProductMapClass blendComponent in lineItemProduct.ComponentCollection)
			{
				ProductMapClass armComponent = loadArmManager.GetComponent(blendComponent.AssignedGuid);
				if (armComponent == null)
				{
					throw new Exception("Component not found in LoadArm Configuration");
				}

				if (armComponent.Type == PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_MAP
						  || armComponent.Type == PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_TANKGROUP_MAP)
				{
					continue;
				}

				this.CreateSubLineItem(loadArmManager, lineItem, armComponent, blendComponent, server);
			}
		}

		public void CreateAdditiveSubLineItems(LineItemDO lineItem, int subLineIndex, Server server, LoadArmManagerClass loadArmManager)
		{
			if (loadArmManager.AdditiveProfile != null)
			{
				UnitsHelperClass unitsHelper = new UnitsHelperClass(this.Security, this.SiteManager.Site, this.CurrentTransactionAlias, null);

				foreach (ProductMapClass additive in loadArmManager.AdditiveProfile.AdditiveCollection)
				{
					ProductMapClass additiveInjector = loadArmManager.GetAdditive(additive.AssignedGuid);
					if (additiveInjector == null)
					{
						throw new Exception("Additive not found in LoadArm Configuration");
					}

					SubLineItemDO subLineItem = new SubLineItemDO
					{
						Status = lineItem.Status,
						ArmNumber = lineItem.ArmNumber.Value,
						ProductGuid = additive.AssignedGuid,
						Product = additive.AssignedID,
						ProductCode = additive.AssignedCode,
						ProductType = ProductClass.ProductTypeID(ProductType.AdditiveProduct),

						Density = 0.0,
						Temperature = 0.0,
						PresetAmount = 0.0
					};

					double CycleVolume = 0.0;
					double Rate = 0.0;

					ProductClass product = this.GetProduct(this.Security, subLineItem.ProductGuid);
					unitsHelper.SetUnits(subLineItem, ProductType.AdditiveProduct, product);

					// Units helper sets units for transaction detail, for load rack the AdditiveProfileCycleAmountUnits must match the Preset
					// All Presets must match the Site Configuration.
					subLineItem.VolumeUnits = this.SiteManager.Site.AdditiveProfileCycleAmountUnits;
					subLineItem.VolumeDecimalPlaces = this.SiteManager.Site._AdditiveProfileCycleAmountDecimalPlaces;

					CycleVolume = StationManagerClass.Convert(additive._AdditiveCycleVolume.SIValue, EngineeringUnit.FmvMeter3, subLineItem.VolumeUnits);
					Rate = StationManagerClass.Convert(additive._AdditiveRate.SIValue, EngineeringUnit.FmvMeter3, subLineItem.VolumeUnits);


					CycleVolume = StationManagerClass.Convert(
						additive._AdditiveCycleVolume.SIValue, EngineeringUnit.FmvMeter3, subLineItem.VolumeUnits);

					Rate = StationManagerClass.Convert(additive._AdditiveRate.SIValue, EngineeringUnit.FmvMeter3, subLineItem.VolumeUnits);

					// For Splash Blend the Preset amount is based upon the sublineitem preset	
					if (lineItem.SplashBlendingMap != null)
					{
						SubLineItemDO ComponentSubLineItem = lineItem.SubLineItems[subLineIndex] as SubLineItemDO;
						double presetAmount = ComponentSubLineItem.PresetAmount.Value;
						if (subLineItem.VolumeUnits != ComponentSubLineItem.VolumeUnits)
						{
							presetAmount = StationManagerClass.Convert(
								ComponentSubLineItem.PresetAmount.Value, ComponentSubLineItem.VolumeUnits, subLineItem.VolumeUnits);
						}
						subLineItem.PresetAmount = presetAmount / Rate * CycleVolume;
						subLineItem.BatchNumber = ComponentSubLineItem.BatchNumber;
					}

					else
					{
						// For Blends need to consider Products Using Injector, we must
						// assume that the Blend Component Configuration will match
						if (loadArmManager.CurrentLineItemProduct.ProductType == ProductType.BlendProduct)
						{
							byte ProductsUsingInjector = this.ReadAdditiveProductsUsingInjector(additiveInjector, server, loadArmManager);

							double TotalExternalComponentPercentage = 0;
							double TotalComponentPercentage = 0;
							foreach (ProductMapClass Component in loadArmManager.CurrentLineItemProduct.ComponentCollection)
							{
								ProductMapClass ArmComponent = loadArmManager.GetComponent(Component.AssignedGuid);
								if (ArmComponent == null)
								{
									throw new Exception("Component not found in LoadArm Configuration");
								}

								if (ArmComponent.Type == PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_MAP)
								{
									TotalExternalComponentPercentage += Component.BlendPercentage;
								}
								else
								{
									TotalComponentPercentage += Component.BlendPercentage;
								}
							}

							foreach (ProductMapClass Component in loadArmManager.CurrentLineItemProduct.ComponentCollection)
							{
								ProductMapClass ArmComponent = loadArmManager.GetComponent(Component.AssignedGuid);
								if (ArmComponent == null)
								{
									throw new Exception("Component not found in LoadArm Configuration");
								}

								if (ArmComponent.Type == PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_MAP)
								{
									continue;
								}

								if ((ProductsUsingInjector & (1 << (ArmComponent.PresetNumber - 1))) != 0)
								{
									double PresetComponentPercentage = Component.BlendPercentage
																				  + Component.BlendPercentage / TotalComponentPercentage
																				  * TotalExternalComponentPercentage;
									double presetAmount = lineItem.PresetAmount.Value;
									if (subLineItem.VolumeUnits != lineItem.VolumeUnits)
									{
										presetAmount = StationManagerClass.Convert(lineItem.PresetAmount.Value, lineItem.VolumeUnits, subLineItem.VolumeUnits);
									}
									subLineItem.PresetAmount += (int)(presetAmount * PresetComponentPercentage / 100.0 / Rate) * CycleVolume;
								}
							}
						}

						else
						{
							double presetAmount = lineItem.PresetAmount.Value;
							if (subLineItem.VolumeUnits != lineItem.VolumeUnits)
							{
								presetAmount = StationManagerClass.Convert(lineItem.PresetAmount.Value, lineItem.VolumeUnits, subLineItem.VolumeUnits);
							}
							subLineItem.PresetAmount = (int)(presetAmount / Rate) * CycleVolume;
						}

						subLineItem.BatchNumber = lineItem.BatchNumber;
					}

					subLineItem.MeterID = additiveInjector.Meter.ID;
					subLineItem.MeterGuid = additiveInjector.AssignedToMeterGuid;

					TankClass Tank = this.SiteManager.GetTank(additiveInjector, this.Manager);
					if (Tank != null)
					{
						subLineItem.StorageLocationID = Tank.ID;
						subLineItem.StorageLocationTankGuid = Tank.IdentityGuid;
					}

					lineItem.SubLineItems.Add(subLineItem);
				}
			}
		}

		protected int GetSplashBlendSubLineItem(
			Server server,
			LoadArmManagerClass loadArmManager,
			LineItemDO lineItem)
		{
			// First look to see if we have a sub line item already
			for (int nLoop = 0; nLoop < lineItem.SubLineItems.Count; ++nLoop)
			{
				SubLineItemDO checkItem = lineItem.SubLineItems[nLoop] as SubLineItemDO;

				if (checkItem.ProductGuid != Guid.Empty && checkItem.ProductGuid == lineItem.SplashBlendingMap.AssignedGuid)
				{
					return nLoop;
				}
			}

			// If we did not find one, figure out the arm component we are using
			ProductMapClass armComponent = loadArmManager.GetComponent(lineItem.SplashBlendingMap.AssignedGuid);

			// Create the new sublineitem first
			int subLineIndex = this.CreateSubLineItem(loadArmManager, lineItem, armComponent, lineItem.SplashBlendingMap, server);

			SubLineItemDO subLineItem = lineItem.SubLineItems[subLineIndex];
			subLineItem.BatchNumber = loadArmManager.GetBatchNumber(this);

			// Then create any additive lines
			this.CreateSplashAdditiveLines(lineItem, subLineIndex, server, loadArmManager);

			// Now create a new sub line item
			return subLineIndex;
		}

		protected int CreateSubLineItem(
			LoadArmManagerClass LoadArmManager,
			LineItemDO LineItem,
			ProductMapClass ArmComponent,
			ProductMapClass BlendComponent,
			Server Server)
		{
			SubLineItemDO SubLineItem = new SubLineItemDO
			{
				Status = LineItem.Status,
				BatchNumber = LineItem.BatchNumber,

				Density = 0.0,
				Temperature = 0.0,
				PresetAmount = 0.0,

				ArmNumber = this.Station.SwingArmPosition == "A" ? LoadArmManager.LoadArm.BayAArmNumber : LoadArmManager.LoadArm.BayBArmNumber,

				LineNumber = ArmComponent.PresetNumber,

				Product = ArmComponent.AssignedID,
				ProductCode = ArmComponent.AssignedCode,
				ProductType = ProductClass.ProductTypeID(ArmComponent.AssignedProductType),
				ProductGuid = ArmComponent.AssignedGuid
			};

			ProductClass product =
				FMChannelHelper.MakeCall<IProducts, ProductClass>(
					x => x.GetByProductAuthorizedCompanies(this.Security, ArmComponent.AssignedGuid, true));

			UnitsHelperClass unitsHelper = new UnitsHelperClass(this.Security, this.SiteManager.Site, this.CurrentTransactionAlias, product);

			unitsHelper.SetUnits(SubLineItem, ProductType.ComponentProduct, product);

			double presetAmount = LineItem.PresetAmount.Value;
			if (SubLineItem.VolumeUnits != LineItem.VolumeUnits)
			{
				presetAmount = StationManagerClass.Convert(LineItem.PresetAmount.Value, LineItem.VolumeUnits, SubLineItem.VolumeUnits);
			}

			SubLineItem.PresetAmount = presetAmount * BlendComponent.BlendPercentage / 100.0;

			SubLineItem.MeterID = ArmComponent.Meter.ID;
			SubLineItem.MeterGuid = ArmComponent.AssignedToMeterGuid;

			TankClass Tank = this.SiteManager.GetTank(ArmComponent, this.Manager);
			if (Tank != null)
			{
				SubLineItem.StorageLocationID = Tank.ID;
				SubLineItem.StorageLocationTankGuid = Tank.IdentityGuid;
			}

			SubLineItem.IsEthanol = product.IsEthanol;
			SubLineItem.VcfModuleSettings = product._VcfModuleSettings;
			if(SubLineItem.IsEthanol)
			{
				LineItem.IsEthanolBlend = true;
			}

			LineItem.SubLineItems.Add(SubLineItem);

			return LineItem.SubLineItems.Count - 1;
		}

		public void UpdateLineItem(LineItemDO lineItem)
		{
			// If the arm number is not set, this line item is only set to status InProgress but is not actually loading
			if (lineItem.ArmNumber == null)
			{
				return;
			}

			LoadArmClass loadArm = this.Station.LoadArmCollection[lineItem.ArmNumber.Value - 1];
			ProcessVariableClass loadArmPv = loadArm.ProcessVariableCollection[0];
			LoadArmManagerClass loadArmManager = this.GetLoadArmManager(loadArm);

			if (loadArmManager == null)
			{
				return;
			}

			if (loadArmManager.LoadArmState != LOADARM_STATE.INPROGRESS
				 && loadArmManager.LoadArmState != LOADARM_STATE.INPROGRESS_PERMISSIVE_PROMPT
				 && loadArmManager.LoadArmState != LOADARM_STATE.END_BATCH_PROMPT
				 && loadArmManager.LoadArmState != LOADARM_STATE.BATCH_STOPPED_PROMPT)
			{
				return;
			}

			lineItem.LoadingLocationID = this.Station.ID;
			lineItem.LoadingLocationStationGuid = this.Station.IdentityGuid;

			Server server = new Server(new Factory(), new URL(loadArmPv.URL));
			server.Connect(new ConnectData(null));

			if (!this.RemoteAuthorized && lineItem.SplashBlendingMap != null)
			{
				// Create the sub line item if necessary
				int subItemIndex = this.GetSplashBlendSubLineItem(server, loadArmManager, lineItem);

				loadArmManager.SplashSubLineItem = lineItem.SubLineItems[subItemIndex];
				loadArmManager.SplashSubLineItem.Status = TransactionStatus.InProgress;
			}

			// Remote Load
			if ((this.RemoteAuthorized && this.RemoteSubLineItem != null) || lineItem.SplashBlendingMap != null)
			{
				SubLineItemDO subLineItem = this.RemoteSubLineItem ?? loadArmManager.SplashSubLineItem;

				ProductMapClass armComponent = loadArmManager.GetComponent(subLineItem.ProductGuid);
				if (armComponent == null)
				{
					throw new Exception("Component not found in LoadArm Component Configuration");
				}

				subLineItem.MeterID = armComponent.Meter.ID;
				subLineItem.MeterGuid = armComponent.AssignedToMeterGuid;

				if (string.IsNullOrEmpty(subLineItem.BatchNumber))
				{
					subLineItem.BatchNumber = loadArmManager.GetBatchNumber(this);
				}

				if (string.IsNullOrEmpty(subLineItem.BatchNumber))
				{
					return;
				}

				// Set the arm number
				if (subLineItem.ArmNumber == null)
				{
					subLineItem.ArmNumber = loadArmManager.GetArmNumber(this);
				}

				TankClass tank = this.SiteManager.GetTank(armComponent, this.Manager);
				if (tank != null)
				{
					subLineItem.StorageLocationID = tank.ID;
					subLineItem.StorageLocationTankGuid = tank.IdentityGuid;
				}
			}

			else
			{
				// Must have Batch Number, Normally this is determined in the AddLineItem
				// for Preload Line Items it must be determined here 
				if (string.IsNullOrEmpty(lineItem.BatchNumber))
				{
					lineItem.BatchNumber = loadArmManager.GetBatchNumber(this);
				}

				if (string.IsNullOrEmpty(lineItem.BatchNumber))
				{
					return;
				}

				if (loadArmManager.CurrentLineItemProduct != null)
				{
					if (loadArmManager.CurrentLineItemProduct.ProductType == ProductType.ComponentProduct)
					{
						ProductMapClass armComponent = loadArmManager.GetComponent(loadArmManager.CurrentLineItemProduct.IdentityGuid);
						if (armComponent == null)
						{
							throw new Exception("Component not found in LoadArm Configuration");
						}

						lineItem.MeterID = armComponent.Meter.ID;
						lineItem.MeterGuid = armComponent.AssignedToMeterGuid;

						TankClass Tank = this.SiteManager.GetTank(armComponent, this.Manager);
						if (Tank != null)
						{
							lineItem.StorageLocationID = Tank.ID;
							lineItem.StorageLocationTankGuid = Tank.IdentityGuid;
						}
					}

					if (lineItem.SubLineItems.Count == 0)
					{
						// Add Blend Components
						if (loadArmManager.CurrentLineItemProduct.ProductType == ProductType.BlendProduct)
						{
							this.CreateBlendSubLineItems(lineItem, loadArmManager.CurrentLineItemProduct, loadArmManager, server);
						}

						// Add the Additives	
						if (loadArmManager.AdditiveProfile != null)
						{
							this.CreateAdditiveSubLineItems(lineItem, 0, server, loadArmManager);
						}
					}
				}
			}

			this.ReadLineItemData(lineItem, server, loadArmManager);


			// If we are loading a splash blend, roll the latest totals up into the main line item
			if (lineItem.SplashBlendingMap != null)
			{
				this.RollUpSplashBlendTotals(lineItem);
			}

			lineItem.UpdateDeliveredQuantities(this.Station.EthanolExcess, loadArmManager.LoadArm, true);

			server.Disconnect();
			server.Dispose();
		}

		protected void CreateSplashAdditiveLines(LineItemDO lineItem, int subLineIndex, Server server, LoadArmManagerClass loadArmManager)
		{
			loadArmManager.AdditiveProfile = null;
			if (lineItem.AdditiveProfileGuid != Guid.Empty)
			{
				loadArmManager.AdditiveProfile =
					FMChannelHelper.MakeCall<IAdditiveProfiles, AdditiveProfileClass>(
						x => x.Get(this.Security, lineItem.AdditiveProfileGuid));
				this.CreateAdditiveSubLineItems(lineItem, subLineIndex, server, loadArmManager);
			}
		}

		protected virtual void Unauthorize()
		{
			foreach (LoadArmManagerClass loadArmManager in this.LoadArmManagerCollection)
			{
				if (this != loadArmManager.GetStationManager())
				{
					continue;
				}

				loadArmManager.Unauthorize();
			}

			this.SendEndTransaction();
		}

		private bool CheckForBrokenBlends(TransactionDO transaction)
		{
			bool brokenBlends = false;

			int itemNumber = 0;
			foreach (LineItemDO lineItem in transaction.LineItems)
			{
				itemNumber++;
				if (lineItem.ProductType != ProductClass.ProductTypeID(ProductType.BlendProduct))
				{
					continue;
				}

				ProductClass product = this.GetByProductAuthorizedCompanies(this.Security, lineItem.ProductGuid, false);

				if (product.ComponentCollection.Count == 1)
				{
					continue;
				}

				bool brokenBlend = false;

				foreach (SubLineItemDO subLineItem in lineItem.SubLineItems)
				{
					if (subLineItem.ProductType != ProductClass.ProductTypeID(ProductType.ComponentProduct))
					{
						continue;
					}

					ProductMapClass component = product.ComponentCollection.Find(x => x.AssignedGuid == subLineItem.ProductGuid);
					if (component == null)
					{
						continue;
					}

					// Total the Gross & Net volume for the component because for external components
					// there may be multiple subline items
					double grossVolume = 0;
					double netVolume = 0;
					foreach (SubLineItemDO secondSubLineItem in lineItem.SubLineItems)
					{
						if (subLineItem.ProductGuid == secondSubLineItem.ProductGuid)
						{
							if (secondSubLineItem.Quantity != null)
							{
								grossVolume += StationManagerClass.Convert(secondSubLineItem.Quantity.Gross, secondSubLineItem.VolumeUnits, lineItem.VolumeUnits);
								netVolume += StationManagerClass.Convert(secondSubLineItem.Quantity.Net, secondSubLineItem.VolumeUnits, lineItem.VolumeUnits);
							}
						}
					}

					if (this.SiteManager.Site.LoadByNet)
					{
						double requiredAmount = Math.Round(lineItem.Quantity.Net * component.BlendPercentage / 100.0, subLineItem.VolumeDecimalPlaces, MidpointRounding.AwayFromZero);
						if (requiredAmount != 0.0 && Math.Abs(requiredAmount - netVolume) / requiredAmount > (double)product._ComponentTolerance.Value / 100.0)
						{
							this.AddAlarmAndEventLogs(this.Security, this.Station.BrokenBlendAlarm(itemNumber, lineItem.Product));
							brokenBlend = true;
							break;
						}
					}
					else
					{
						double requiredAmount = Math.Round(lineItem.Quantity.Gross * component.BlendPercentage / 100.0, subLineItem.VolumeDecimalPlaces, MidpointRounding.AwayFromZero);
						if (requiredAmount != 0.0 && Math.Abs(requiredAmount - grossVolume) / requiredAmount > (double)product._ComponentTolerance.Value / 100.0)
						{
							this.AddAlarmAndEventLogs(this.Security, this.Station.BrokenBlendAlarm(itemNumber, lineItem.Product));
							brokenBlend = true;
							break;
						}
					}
				}

				if (brokenBlend)
				{
					lineItem.BrokenBlend = true;
					brokenBlends = true;
				}
				else
				{
					lineItem.BrokenBlend = false;
				}

				foreach (SubLineItemDO subLineItem in lineItem.SubLineItems)
				{
					if (subLineItem.ProductType != ProductClass.ProductTypeID(ProductType.ComponentProduct))
					{
						continue;
					}

					subLineItem.BrokenBlend = lineItem.BrokenBlend;
				}
			}

			if (transaction.TransPIDXCollection != null)
			{
				foreach (TransactionPIDXDO transactionPidxdo in transaction.TransPIDXCollection)
				{
					transactionPidxdo.BrokenBlend = brokenBlends;
				}
			}

			return brokenBlends;
		}

		protected bool CheckForImproperAdditization(TransactionDO transaction)
		{
			bool improperAdditization = false;


			int itemNumber = 0;
			foreach (LineItemDO lineItem in transaction.LineItems)
			{
				itemNumber++;
				if (lineItem.AdditiveProfileGuid != Guid.Empty)
				{
					bool improperlyAdditized = false;

					AdditiveProfileClass additiveProfile = this.GetAdditiveProfiles(this.Security, lineItem.AdditiveProfileGuid);
					foreach (SubLineItemDO subLineItem in lineItem.SubLineItems)
					{
						if (subLineItem.ProductType != ProductClass.ProductTypeID(ProductType.AdditiveProduct))
						{
							continue;
						}

						ProductMapClass additive = additiveProfile.AdditiveCollection.Find(x => x.AssignedGuid == subLineItem.ProductGuid);
						if (additive == null)
						{
							continue;
						}

						double requiredAmount = 0.0;

						if (subLineItem.PresetAmount != null)
						{
							requiredAmount = subLineItem.PresetAmount.Value;
						}

						if (this.SiteManager.Site.LoadByNet)
						{
							// ReSharper disable once CompareOfFloatsByEqualityOperator
							if (lineItem.PresetAmount == null || lineItem.PresetAmount.Value == 0.0)
							{
								requiredAmount = 0;
							}
							else
							{
								requiredAmount *= lineItem.Quantity.Net / lineItem.PresetAmount.Value;
							}

							// When Product Is Delivered the Required  Amount must be non zero
							// && the delivered amount must be within the Additive Tolerance
							// ReSharper disable CompareOfFloatsByEqualityOperator
							if (lineItem.PresetAmount != null && ((lineItem.PresetAmount.Value != 0.0 && requiredAmount == 0.0)
																			  || Math.Abs(requiredAmount - subLineItem.Quantity.Net) / requiredAmount > additive.Tolerance / 100.0))
							{
								if (!improperlyAdditized)
								{
									FMChannelHelper.MakeCall<IAlarmAndEventLogs>(alarmAndEventLogs => alarmAndEventLogs.Add(
										 this.Security,
										 this.Station.ImproperAdditizationAlarm(itemNumber, lineItem.AdditiveProfileID)));
								}

								subLineItem.ImproperAdditization = true;
								improperlyAdditized = true;
							}
							else
							{
								subLineItem.ImproperAdditization = false;
							}
							// ReSharper restore CompareOfFloatsByEqualityOperator
						}
						else
						{
							// ReSharper disable once CompareOfFloatsByEqualityOperator
							if (lineItem.PresetAmount == null || lineItem.PresetAmount.Value == 0.0)
							{
								requiredAmount = 0;
							}
							else
							{
								requiredAmount *= lineItem.Quantity.Gross / lineItem.PresetAmount.Value;
							}

							// When Product Is Delivered the Required  Amount must be non zero
							// && the delivered amount must be within the Additive Tolerance
							// ReSharper disable CompareOfFloatsByEqualityOperator
							if (lineItem.PresetAmount != null && ((lineItem.PresetAmount.Value != 0.0 && requiredAmount == 0.0)
																			  || Math.Abs(requiredAmount - subLineItem.Quantity.Gross) / requiredAmount > additive.Tolerance / 100.0))
							{
								if (!improperlyAdditized)
								{
									FMChannelHelper.MakeCall<IAlarmAndEventLogs>(alarmAndEventLogs => alarmAndEventLogs.Add(
										 this.Security,
										 this.Station.ImproperAdditizationAlarm(itemNumber, lineItem.AdditiveProfileID)));
								}

								subLineItem.ImproperAdditization = true;
								improperlyAdditized = true;
							}
							else
							{
								subLineItem.ImproperAdditization = false;
							}
							// ReSharper restore CompareOfFloatsByEqualityOperator
						}
					}

					if (improperlyAdditized)
					{
						lineItem.ImproperAdditization = true;
						improperAdditization = true;
					}
					else
					{
						lineItem.ImproperAdditization = false;
					}
				}
			}

			return improperAdditization;
		}

		[SecurityCritical]
		public void PrintCertificateOfAnalysis()
		{
			try
			{
				bool AnyFailures = false;

				// Check Quality AssuranceInterface
				IQualityAssurance QAInterface = this.GetQualityAssuranceInterface();
				if (QAInterface == null)
				{
					return;
				}

				// Get Associated Order
				TransactionDO Order = null;
				if (string.IsNullOrEmpty(this.Transaction.TransRefID) == false)
				{
					Order = this.GetTransaction(this.Transaction.TransRefID);
				}

				foreach (LineItemDO LineItem in this.Transaction.LineItems)
				{
					string COAID;

					if (LineItem.ProductType != ProductClass.ProductTypeID(ProductType.BlendProduct)
						 || !QAInterface.BlendComponentsCOA(this.Security, LineItem.ProductGuid))
					{
						string AbbrevString = EngineeringUnits.GetUnitAbbreviation(LineItem.VolumeUnits);

						try
						{
							QAInterface.CreateCertificateOfAnalysis(
								this.Security,
								LineItem.StorageLocationTankGuid,
								LineItem.ProductGuid,
								this.Transaction.OwnerCompanyGuid,
								this.Transaction.BillToCompanyGuid,
								this.Transaction.ShipToCompanyGuid,
								this.Transaction.DocumentNumber,
								LineItem.Quantity.Net,
								AbbrevString,
								LineItem.Temperature.Value,
								(Order == null) ? "" : Order.DocumentNumber,
								this.Transaction.InventoryDate,
								this.Transaction.TimeOut.Value,
								this.Transaction.CarrierCompanyGuid,
								LineItem.DestinationCompartmentID,
								LineItem.DestinationEQ.RegistrationID,
								this.Transaction.PONumber,
								this.Station.BOLPrinter,
								LineItem.COAWaiver,
								LineItem.COANote,
								out COAID);
							LineItem.COAID = COAID;
						}
						catch (Exception e)
						{
							AnyFailures = true;
							this.eventLog.WriteEntry(e.ToString(), EventLogEntryType.Error);
						}
					}
					else
					{
						foreach (SubLineItemDO SubLineItem in LineItem.SubLineItems)
						{
							if (SubLineItem.ProductType != ProductClass.ProductTypeID(ProductType.ComponentProduct))
							{
								string AbbrevString = EngineeringUnits.GetUnitAbbreviation(LineItem.VolumeUnits);

								try
								{
									QAInterface.CreateCertificateOfAnalysis(
										this.Security,
										SubLineItem.StorageLocationTankGuid,
										SubLineItem.ProductGuid,
										this.Transaction.OwnerCompanyGuid,
										this.Transaction.BillToCompanyGuid,
										this.Transaction.ShipToCompanyGuid,
										this.Transaction.DocumentNumber,
										SubLineItem.Quantity.Net,
										AbbrevString,
										SubLineItem.Temperature.Value,
										(Order == null) ? "" : Order.DocumentNumber,
										this.Transaction.InventoryDate,
										this.Transaction.TimeOut.Value,
										this.Transaction.CarrierCompanyGuid,
										LineItem.DestinationCompartmentID,
										LineItem.DestinationEQ.RegistrationID,
										this.Transaction.PONumber,
										this.Station.BOLPrinter,
										LineItem.COAWaiver,
										LineItem.COANote,
										out COAID);
									SubLineItem.COAID = COAID;
								}
								catch (Exception e)
								{
									AnyFailures = true;
									this.eventLog.WriteEntry(e.ToString(), EventLogEntryType.Error);
								}
							}
						}
					}
				}

				this.SaveTransaction();

				if (AnyFailures)
				{
					FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
						x => x.Add(this.Security, this.Station.CreateCertificateOfAnalysisFailedAlarm(this.Transaction.TransID)));
				}
			}
			catch (Exception e)
			{
				this.eventLog.WriteEntry(e.ToString(), EventLogEntryType.Error);
			}
		}

		public void PrintTransaction()
		{
			try
			{
				// Do not print when AutomaticBOLPrinting is disabled at Site
				if (!this.SiteManager.Site.EnableAutomaticBOLPrinting)
				{
					return;
				}

				this.CurrentTransactionAlias = FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasClass>(
																	 x =>
																	 x.Get(this.Security, this.Transaction.TransactionAliasGuid, false)
																);

				if (this.CurrentTransactionAlias == null)
				{
					return;
				}

				if (this.CurrentTransactionAlias.AssociatedReport == string.Empty
					 || this.CurrentTransactionAlias.AssociatedReport == "(None)")
				{
					return;
				}

				// Check for Broken Blends or Improper Additization
				bool brokenBlends = false;
				bool improperAdditization = false;

				foreach (LineItemDO lineItem in this.Transaction.LineItems)
				{
					if (lineItem.BrokenBlend != null && lineItem.BrokenBlend.Value && this.SiteManager.Site.InhibitBOLWithBrokenBlends)
					{
						brokenBlends = true;
						break;
					}

					if (lineItem.ImproperAdditization != null && lineItem.ImproperAdditization.Value
						 && this.SiteManager.Site.InhibitBOLWithImproperAdditization)
					{
						improperAdditization = true;
						break;
					}
				}

				if (brokenBlends
				|| improperAdditization)
				{
					// If we have exceptions to print but no exception printer defined, bail out
					if (string.IsNullOrEmpty(this.SiteManager.Site.ExceptionBOLPrinter)
					|| this.SiteManager.Site.ExceptionBOLPrinter == "(None)")
					{
						return;
					}
				}
				else
				{
					// if we do not have exceptions to print and no main printer defined, bail out					
					if (string.IsNullOrEmpty(this.Station.BOLPrinter)
					|| this.Station.BOLPrinter == "(None)")
					{
						return;
					}
				}

				ParameterValue[] parameterValues = new ParameterValue[3];

				parameterValues[0] = new ParameterValue { Name = "TransID", Value = this.Transaction.TransID };
				parameterValues[1] = new ParameterValue
				{
					Name = "SiteGuid",
					Value = this.SiteManager.Site.IdentityGuid.ToString()
				};
				parameterValues[2] = new ParameterValue { Name = "FromStation", Value = "True" };

				string rptDir = FMChannelHelper.MakeCall<ISites, string>(
																					  x =>
																					  x.GetReportDirectory(this.Security, this.CurrentTransactionAlias.AssociatedReport)
																				);
				ReportServicePrintService printService =
					 new ReportServicePrintService(this.eventLog)
					 {
						 ReportingServiceUrl = this.SiteManager.LoadRackManager.SystemSetting.ReportServerUrl,
						 ReportName = rptDir + "/" + this.CurrentTransactionAlias.AssociatedReport,
						 ParameterValues = parameterValues,
						 Security = this.Security,
						 EnableBOLPDFArchiving = this.SiteManager.Site.EnableBOLPDFArchiving,
						 BOLPDFArchivingFileName = this.SiteManager.Site.Number + "." + this.Transaction.DocumentNumber + "." + DateTime.Now.ToString("yyyyMMdd.HHmmss") + ".None" + ".pdf",
						 BOLPDFArchivingPath = this.SiteManager.Site.BOLPDFArchivingPath
					 };


				if (!brokenBlends
		  && !improperAdditization)
				{
					printService.PrinterName = this.Station.BOLPrinter;
					printService.NumberOfCopies = this.Station.NumberOfCopies;
					printService.PrintReport();
					this.PrintCertificateOfAnalysis();
				}
				else
				{
					printService.PrinterName = this.SiteManager.Site.ExceptionBOLPrinter;
					printService.PrintReport();
				}
			}
			catch (Exception e)
			{
				this.eventLog.WriteEntry(e.ToString(), EventLogEntryType.Error);
			}
		}

		public void PrintPreload()
		{
			try
			{
				this.CurrentTransactionAlias = FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasClass>(
																	 x =>
																	 x.Get(this.Security, this.Transaction.TransactionAliasGuid, false)
																);
				if (this.CurrentTransactionAlias == null)
				{
					return;
				}

				if (this.CurrentTransactionAlias.AssociatedPreloadReport == string.Empty
					 || this.CurrentTransactionAlias.AssociatedPreloadReport == "(None)")
				{
					return;
				}

				// if we do not have preload printer defined, bail out					
				if (string.IsNullOrEmpty(this.Station.PreloadPrinter)
				|| this.Station.PreloadPrinter == "(None)")
				{
					return;
				}

				ParameterValue[] parameterValues = new ParameterValue[2];

				parameterValues[0] = new ParameterValue
				{
					Name = "TransID",
					Value = this.Transaction.TransID
				};

				parameterValues[1] = new ParameterValue
				{
					Name = "SiteGuidStr",
					Value = this.SiteManager.Site.IdentityGuid.ToString()
				};

				ReportServicePrintService printService = new ReportServicePrintService(this.eventLog)
				{
					ReportingServiceUrl = this.SiteManager.LoadRackManager.SystemSetting.ReportServerUrl
				};
				string rptDir = FMChannelHelper.MakeCall<ISites, string>(
																	 x =>
																	 x.GetReportDirectory(this.Security, this.CurrentTransactionAlias.AssociatedPreloadReport)
																);

				printService.ReportName = rptDir + "/" + this.CurrentTransactionAlias.AssociatedPreloadReport;
				printService.ParameterValues = parameterValues;
				printService.Security = this.Security;

				printService.PrinterName = this.Station.PreloadPrinter; //need to change to preload report
				printService.NumberOfCopies = this.Station.NumberOfPreloadCopies; //need to change to preload report
				printService.PrintReport();
			}
			catch (Exception e)
			{
				this.eventLog.WriteEntry(e.ToString(), EventLogEntryType.Error);
			}
		}

		public void CompleteTransaction()
		{
			this.CompleteTransaction(true);
		}

		public void CompleteTransaction(bool transactionComplete)
		{
			if (this.Transaction != null)
			{
				bool setStatus = true;
				bool allZeroes = true;

				if (this.Transaction.Status == TransactionStatus.LoadPending
				|| this.Transaction.Status == TransactionStatus.InProgress
					 || this.Transaction.Status == TransactionStatus.WeighOutPending)
				{
					if (this.Transaction.LineItems.Count == 0)
					{
						// if we are finishing and there are no line items, cancel the transaction
						this.Transaction.Status = TransactionStatus.Cancelled;
						setStatus = false;
					}
					else
					{
						foreach (LineItemDO lineItem in this.Transaction.LineItems)
						{
							if (lineItem.Status == TransactionStatus.InProgress)
							{
								// If the item has no recoreded volume applied to it, set the line status so that
								// the driver can pick the line up by carding back in
								if (lineItem.Quantity != null && lineItem.Quantity.NetInventoryChange == 0
									 && lineItem.Quantity.GrossInventoryChange == 0 && lineItem.Quantity.MassInventoryChange == 0)
								{
									lineItem.Status = this.PreloadInProgress ? TransactionStatus.LoadPending : TransactionStatus.Cancelled;

									// Also set the sublineitems
									foreach (SubLineItemDO subLineItem in lineItem.SubLineItems)
									{
										subLineItem.Status = this.PreloadInProgress ? TransactionStatus.LoadPending : TransactionStatus.Cancelled;
									}
								}
								else
								{
									// because the system has no provision to resume an interrupted batch
									// A partially loaded line item must be considered completed
									this.CloseOutLineItem(lineItem);
								}
							}

							// Are any of the line items still listed as being pending loading?
							if (lineItem.Status == TransactionStatus.LoadPending)
							{
								// If weightout is required, we need to set to the weighout state because they may be going to weigh out
								// and are not intending to pick up unfinished items; otherwise, we need to set the status to load pending
								if (this.Transaction.WeightReadings.Count == 0)
								{
									this.Transaction.Status = TransactionStatus.LoadPending;
								}
								else
								{
									this.Transaction.Status = TransactionStatus.WeighOutPending;
								}

								setStatus = false;
							}

							// Need to check independent of status
							if (lineItem.Quantity != null
							&& (lineItem.Quantity.NetInventoryChange != 0
							|| lineItem.Quantity.GrossInventoryChange != 0
							|| (lineItem.Quantity.MassInventoryChange != 0.0)))
							{
								allZeroes = false;
							}
						}
					}
				}

				if (setStatus)
				{
					if (transactionComplete)
					{
						if (this.Transaction.WeightReadings.Count == 0)
						{
							this.Transaction.Status = allZeroes ? TransactionStatus.Cancelled : TransactionStatus.Completed;
						}
						else
						{
							this.Transaction.Status = allZeroes ? TransactionStatus.Cancelled : TransactionStatus.WeighOutPending;
						}
					}
					else
					{
						if (this.Transaction.WeightReadings.Count == 0)
						{
							this.Transaction.Status = allZeroes ? TransactionStatus.Cancelled : TransactionStatus.InProgress;
						}
						else
						{
							this.Transaction.Status = allZeroes ? TransactionStatus.Cancelled : TransactionStatus.WeighOutPending;
						}
					}
				}

				DateTimeOffset siteTimeNow = TimeConverter.Now(this.SiteManager.Site);
				this.Transaction.TimeEnd = siteTimeNow;
				this.Transaction.TimeOut = siteTimeNow;

					bool brokenBlends = this.CheckForBrokenBlends(this.Transaction);

					bool improperAdditization = this.CheckForImproperAdditization(this.Transaction);

					// depending on the transaction type there may or may not be a driver
					if (this.Driver?.OnFileSignature != null && this.Driver.OnFileSignature.Length > 0)
					{
						this.Transaction.Signature = this.Driver.OnFileSignature;
					}

					this.SaveTransaction();

				// When Transaction is Completed then print.  Also close out scheduled orders.
				if (this.Transaction.Status == TransactionStatus.Completed)
				{
					 if (!string.IsNullOrEmpty(this.Transaction.TransRefID))
					 {
						  TransactionDO order = this.GetTransaction(this.Transaction.TransRefID);
						  // ReSharper disable once RedundantNameQualifier
						  if (StationManagerClass.IsTransactionScheduledOrder(order) && order.Status == TransactionStatus.Scheduled)
						  {
								order.Status = TransactionStatus.Completed;
								this.SaveArbitraryTransaction(order);
						  }
					 }
					this.PrintTransaction();

					if (TransactionTypes.T8_Receipt == this.Transaction.TransTypeID)
					{
						//this.SiteManager.ResetOwnerAllocations(this.Transaction.InventoryDate);
						this.SiteManager.LoadRackManager.ResetOwnerAllocationsInventoryDate(this.Security,this.Transaction.InventoryDate);
					}

					if (brokenBlends)
					{
						this.StationState = StationState.BROKEN_BLEND;
						this.DisplayMessageWithAcknowledge("LoadRack|Broken Blend Detected.");
					}
					else if (improperAdditization)
					{
						this.StationState = StationState.IMPROPER_ADDITIZATION;
						this.DisplayMessageWithAcknowledge("LoadRack|Improper Additization Detected.");
					}
				}
				else
				{
					this.SaveTransaction();
				}

				 this.Transaction = null;
				 this.SupplyOrder = null;
				 this.Order = null;

				this.RemoteAuthorized = false;
			}

			if (this.StationState != StationState.BROKEN_BLEND && this.StationState != StationState.IMPROPER_ADDITIZATION)
			{
				this.StationState = StationState.IDLE;
			}

			this.LoadArmManagerCollection.SetState(this, LOADARM_STATE.NORMAL);
		}

		 protected virtual void IssueBrokenBlendMessage()
		 {
			  this.StationState = StationState.BROKEN_BLEND;
			  this.DisplayMessageWithAcknowledge("LoadRack|Broken Blend Detected.");
		 }

		 protected virtual void IssueImproperAdditization()
		 {
			  this.StationState = StationState.IMPROPER_ADDITIZATION;
			  this.DisplayMessageWithAcknowledge("LoadRack|Improper Additization Detected.");
		 }

		protected bool ValidateCompany(CompanyClass company, COMPANY_ROLE role)
		{
			if (company == null || company.IdentityGuid == Guid.Empty)
			{
				switch (role)
				{
					case COMPANY_ROLE.MANAGER:
					case COMPANY_ROLE.OWNER:
					case COMPANY_ROLE.CUSTOMER_SHIPTO:
					case COMPANY_ROLE.SUPPLIER:
						this.DisplayMessage("[LoadRack|" + CompanyRoleMapClass.RoleID(role) + "] [LoadRack|Invalid]", null, 0, this.MESSAGE_TIMEOUT);
						this.StationState = StationState.COMPANY_INVALID;
						return false;

					default:
						return true;
				}
			}

			// Check if is Locked Out
			if (company.LockedOut)
			{
				this.AddAlarmAndEventLogs(this.Security, company.LockedOutStationAlarm(this.Driver.FirstLastName, this.Station.ID));
				this.DisplayMessage("[LoadRack|" + CompanyRoleMapClass.RoleID(role) + "] [LoadRack|Locked Out]", null, 0, this.MESSAGE_TIMEOUT);
				this.StationState = role == COMPANY_ROLE.CARRIER ? StationState.CARRIER_INVALID : StationState.COMPANY_INVALID;

				return false;
			}

			// Check if Inactive
			if (!company.Active)
			{
				this.AddAlarmAndEventLogs(this.Security, company.InactiveStationAlarm(this.Driver.FirstLastName, this.Station.ID));
				this.DisplayMessage("[LoadRack|" + CompanyRoleMapClass.RoleID(role) + "] [LoadRack|Inactive]", null, 0, this.MESSAGE_TIMEOUT);
				this.StationState = role == COMPANY_ROLE.CARRIER ? StationState.CARRIER_INVALID : StationState.COMPANY_INVALID;

				return false;
			}

			// Check if Credit OK
			if (!company.CreditOK)
			{
				this.AddAlarmAndEventLogs(this.Security, company.CreditAlarm);
				this.DisplayMessage("[LoadRack|" + CompanyRoleMapClass.RoleID(role) + "] [LoadRack|Credit]", null, 0, this.MESSAGE_TIMEOUT);
				switch (role)
				{
					case COMPANY_ROLE.CARRIER:
						this.StationState = StationState.CARRIER_INVALID;
						break;
					default:
						this.StationState = StationState.COMPANY_INVALID;
						break;
				}

				return false;
			}

			// Check if Carrier AccessSchedule precludes access
			if (role == COMPANY_ROLE.CARRIER
				&& this.SiteManager.Site.InhibitAccessAfterHours)
			{
				DateTimeOffset now = TimeConverter.Now(this.SiteManager.Site);
				int index = (int)now.DayOfWeek;
				if (!this.Carrier.AccessScheduleCollection[index].Enabled
					 || this.Carrier.AccessScheduleCollection[index].OpeningTime.Value.TimeOfDay > now.TimeOfDay
					 || this.Carrier.AccessScheduleCollection[index].ClosingTime.Value.TimeOfDay < now.TimeOfDay)
				{
					this.AddAlarmAndEventLogs(this.Security, this.Carrier.AccessScheduleAlarm);
					this.DisplayMessage("[LoadRack|Carrier Access Not Scheduled]", null, 0, this.MESSAGE_TIMEOUT);
					this.StationState = StationState.CARRIER_INVALID;
					return false;
				}
			}

			// Check Authorized Carrier
			if (role == COMPANY_ROLE.CUSTOMER_SHIPTO)
			{
				if (this.Carrier != null)
				{
					bool found = false;
					foreach (CompanyMapClass authorizedCarrier in company.AuthorizedCarrierCollection)
					{
						if (authorizedCarrier.AssignedGuid == this.Carrier.IdentityGuid)
						{
							found = true;
							break;
						}
					}

					if (!found)
					{
						this.AddAlarmAndEventLogs(this.Security, company.UnauthorizedCarrierAlarm);
						this.DisplayMessage("LoadRack|Unauthorized Carrier", null, 0, this.MESSAGE_TIMEOUT);
						this.StationState = StationState.COMPANY_INVALID;
						return false;
					}
				}
			}

			return true;
		}

		protected bool ValidateLoadID(Guid loadIDToCompanyShipToMapGuid)
		{
			bool bReturn = false;

			if (this.Manager != null &&
			this.Owner != null &&
			this.Shipper != null &&
			this.BillTo != null &&
			this.ShipTo != null &&
			this.Carrier != null)
			{
				bReturn = true;
			}
			else
			{
				CompanyMapClass companyMap = FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapClass>(
							x =>
							x.GetLoadIdMapWithoutPersonnelCheck(this.Security, loadIDToCompanyShipToMapGuid)
					); // .2 sec

				this.LoadID = companyMap.MapID;
				companyMap = FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapClass>(
						x =>
						x.Get(this.Security, companyMap.AssignedToGuid, COMPANY_MAP_TYPE.SHIPTO_BILLTO_MAP)
				);

				bReturn = this.ValidateCompanyHierarchyLoadRack(companyMap);
				if (!bReturn)
				{
					this.StationState = StationState.RESET_ON_TIMEOUT;
					this.DisplayMessage("[LoadRack|Invalid] [LoadRack|Load ID]", null, 0, this.MESSAGE_TIMEOUT);
				}
			}
			return bReturn;

		}

		protected bool ValidateOffLoadID(Guid loadIDToCompanyMapGuid)
		{
			CompanyMapClass companyMap = FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapClass>(
																	 x =>
																	 x.Get(this.Security, loadIDToCompanyMapGuid, COMPANY_MAP_TYPE.OFFLOADID_SUPPLIER_MAP)
																);

			this.LoadID = companyMap.MapID;
			companyMap = FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapClass>(
																	 x =>
																	 x.Get(this.Security, companyMap.AssignedToGuid, COMPANY_MAP_TYPE.SUPPLIER_OWNER_MAP)
																);

			return this.ValidateOffLoadingCompanyHierarchy(companyMap);
		}

		protected PIDXAuthorizationBase GetPIDXAuthorization(
			PIDXProfileClass pidxProfile,
			PIDXProfileCompanyMapClass pidxProfileCompanyMap)
		{
			if (pidxProfile == null || pidxProfileCompanyMap == null)
			{
				return null;
			}

			TcpCommBase pidxCommunicationsInterface;
			PIDXRecordBase pidxRecord = null;

			switch (pidxProfile.Type)
			{
				case PIDXType.Dtn:
					pidxCommunicationsInterface = new TcpCommDtn();

					if (pidxProfile.Version == PIDXVersion.OneDotZeroTwo)
					{
						pidxRecord = new CreditAuthorizationRecordDTN();
					}
					else if (pidxProfile.Version == PIDXVersion.FourDotZeroOne)
					{
						pidxRecord = new LoadAuthorizationRecordDTN();
					}

					break;

				case PIDXType.Tds:
					pidxCommunicationsInterface = new TcpCommTds();

					if (pidxProfile.Version == PIDXVersion.OneDotZeroTwo)
					{
						pidxRecord = new CreditAuthorizationRecord();
					}
					else if (pidxProfile.Version == PIDXVersion.FourDotZeroOne)
					{
						pidxRecord = new LoadAuthorizationRecord();
					}

					break;

				default:
					return null;
			}

			pidxCommunicationsInterface.LogFileNameandPath = pidxProfile.LoggingEnabled ? pidxProfile.LogFilePath : string.Empty;
			pidxCommunicationsInterface.HostName = pidxProfile.IPAddress;
			pidxCommunicationsInterface.Port = pidxProfile.Port;
			pidxCommunicationsInterface.LoginName = pidxProfile.UserID;
			pidxCommunicationsInterface.LoginPassword = pidxProfile.Password;
			pidxCommunicationsInterface.Version = pidxProfile.Version;

			pidxCommunicationsInterface.PidxRecord = pidxRecord;

			pidxRecord.CarrierID = this.Carrier.SCACCode;
			pidxRecord.ConsigneeNumber = pidxProfileCompanyMap.ConsigneeNumber;

			try
			{
				pidxRecord.FinalShipperIDDigit = System.Convert.ToInt32(pidxProfileCompanyMap.ShipperID);
			}
			catch
			{
				this.eventLog.WriteEntry("StationManager GetPIDXAuthorization invalid Shipper ID : " + pidxProfileCompanyMap.ShipperID, EventLogEntryType.Error);
				return null;
			}

			try
			{
				pidxRecord.SellerIDDigit = System.Convert.ToInt32(pidxProfileCompanyMap.SellerID);
			}
			catch
			{
				this.eventLog.WriteEntry("StationManager GetPIDXAuthorization invalid Seller ID : " + pidxProfileCompanyMap.SellerID, EventLogEntryType.Error);
				return null;
			}

			try
			{
				pidxRecord.SPLCCodeDigit = System.Convert.ToInt32(this.SiteManager.Site.SPLCCode);
			}
			catch
			{
				this.eventLog.WriteEntry("StationManager GetPIDXAuthorization invalid Site SPLCCode : " + this.SiteManager.Site.SPLCCode, EventLogEntryType.Error);
				return null;
			}

			try
			{
				pidxRecord.TerminalOperatorDigit = System.Convert.ToInt32(pidxProfile.TerminalID);
			}
			catch
			{
				this.eventLog.WriteEntry("StationManager GetPIDXAuthorization invalid Terminal ID : " + pidxProfile.TerminalID, EventLogEntryType.Error);
				return null;
			}

			// TruckNumber is applicable to PIDXR 1.02
			if (pidxProfile.Version == PIDXVersion.OneDotZeroTwo)
			{
				if (this.Shipper.IdentityGuid == this.Carrier.IdentityGuid)
				{
					EquipmentClass equipment = null;

					if (this.TractorOrTanker != null && this.TractorOrTanker.Type == EQUIPMENT_TYPE.TANKER_TYPE)
					{
						equipment = this.TractorOrTanker;
					}

					if (this.Trailer1 != null)
					{
						equipment = this.Trailer1;
					}

					if (equipment == null)
					{
						((CreditAuthorizationRecord)pidxRecord).TruckNumberDigit = 0;
					}
					else
					{
						try
						{
							// ReSharper disable once PossibleNullReferenceException
							((CreditAuthorizationRecord)pidxRecord).TruckNumberDigit = System.Convert.ToInt32(equipment.CompanyEquipmentID);
						}
						catch
						{
							this.eventLog.WriteEntry(
								 "StationManager GetPIDXAuthorization invalid Equipmen ID : " + equipment.CompanyEquipmentID,
								 EventLogEntryType.Error);
						}
					}
				}
				else
				{
					((CreditAuthorizationRecord)pidxRecord).TruckNumberDigit = 0;
				}
			}

			pidxRecord.RackDriverID = this.Driver.ID;
			pidxRecord.TerminalControlNumber = this.SiteManager.Site.TerminalControlNumber;
			if (this.PONumber != null)
			{
				pidxRecord.ReleaseOrderNumber = this.PONumber;
			}

			if (!pidxCommunicationsInterface.SendTransaction())
			{
				this.eventLog.WriteEntry("StationManager GetPIDXAuthorization : " + pidxCommunicationsInterface.ExceptionString, EventLogEntryType.Error);
				return null;
			}

			return pidxCommunicationsInterface.PidxAuth;
		}

		protected bool PIDXAuthorizations
		{
			get
			{
				if (this.PIDXAuthorizationArray == null)
				{
					return false;
				}

				// ReSharper disable once ForCanBeConvertedToForeach
				for (int index = 0; index < this.PIDXAuthorizationArray.Length; index++)
				{
					if (this.PIDXAuthorizationArray[index] != null)
					{
						return true;
					}
				}

				return false;
			}
		}

		protected bool GetPIDXAuthorizations()
		{
			if (string.IsNullOrEmpty(this.LoadID))
			{
				this.PIDXAuthorizationArray = null;
				return true;
			}

			Guid companyPersonnelToShipToBillToGuid = FMChannelHelper.MakeCall<ICompanyMaps, Guid>(
																	 x =>
																	 x.GetIdentityGuidByMapID(this.Security, this.LoadID)
																);
			this.PIDXProfileCompanyMapCollection =
				FMChannelHelper.MakeCall<IPIDXProfileCompanyMaps, PIDXProfileCompanyMapCollectionClass>(
					x =>
					x.EnumerateSiteAndCompanyPersonnelToShipToBillToGuid(this.Security, companyPersonnelToShipToBillToGuid)
				);

			if (this.PIDXProfileCompanyMapCollection.Count == 0)
			{
				this.PIDXAuthorizationArray = null;
				return true;
			}

			PIDXProfileCollectionClass pidxProfileCollection = FMChannelHelper.MakeCall<IPIDXProfiles, PIDXProfileCollectionClass>(
																	 x =>
																	 x.Enumerate(this.Security)
																);
			this.PIDXAuthorizationArray = new PIDXAuthorizationBase[this.PIDXProfileCompanyMapCollection.Count];

			int authorizationIndex = -1;

			foreach (PIDXProfileCompanyMapClass pidxProfileCompanyMap in this.PIDXProfileCompanyMapCollection)
			{
				authorizationIndex++;

				PIDXProfileClass pidxProfile = pidxProfileCollection.Find(pidxProfileCompanyMap.PIDXProfileGuid);
				if (pidxProfile == null || !pidxProfile.Enabled)
				{
					continue;
				}

				this.PIDXAuthorizationArray[authorizationIndex] = this.GetPIDXAuthorization(pidxProfile, pidxProfileCompanyMap);

				if (this.PIDXAuthorizationArray[authorizationIndex] == null)
				{
					this.AddAlarmAndEventLogs(this.Security, this.Station.PIDXUnavailableAlarm(pidxProfile.ID));

					if (!pidxProfileCompanyMap.UnavailableOverride)
					{
						if (this.PIDXAuthorizations)
						{
							this.InitializeTransaction();
							this.Transaction.DeleteFlag = true;
							this.Transaction.Status = TransactionStatus.Cancelled;
							this.SaveTransaction();
						}

						this.StationState = StationState.PIDX_UNAVAILABLE_MSG;
						this.DisplayMessageWithAcknowledge("[LoadRack|PIDX Unavailable] : " + pidxProfile.ID);
						return false;
					}
				}

				if (this.PIDXAuthorizationArray[authorizationIndex] is AuthorizationDenyBase)
				{
					AuthorizationDenyBase denial = this.PIDXAuthorizationArray[authorizationIndex] as AuthorizationDenyBase;

					string localID = pidxProfile.Type == PIDXType.Dtn ? "DTN - " + pidxProfile.ID : "TDS - " + pidxProfile.ID;
					FMChannelHelper.MakeCall<IAlarmAndEventLogs>(alarmAndEventLogs => alarmAndEventLogs.Add(this.Security, this.Station.PIDXDenialAlarm(localID, denial.DenyReason, this.Driver.FirstLastName, this.ShipTo.ID)));
					this.LoadRackManager.EventOrAlarmEvent.Set();

					if (!pidxProfileCompanyMap.DenialOverride)
					{
						if (this.PIDXAuthorizations)
						{
							this.InitializeTransaction();
							this.Transaction.DeleteFlag = true;
							this.Transaction.Status = TransactionStatus.Cancelled;
							this.SaveTransaction();
						}

						this.StationState = StationState.PIDX_DENIAL_MSG;
						this.DisplayMessageWithAcknowledge("[LoadRack|PIDX Denial] : " + denial.DenyReason);
						return false;
					}
				}
			}

			return true;
		}

		public static string GetLoadRackDisplayText(ProductMapClass authorizedProduct)
		{
			return string.IsNullOrEmpty(authorizedProduct.ShipToLoadRackDisplayText) == false
				? authorizedProduct.ShipToLoadRackDisplayText
				: string.IsNullOrEmpty(authorizedProduct.AssignedLoadRackDisplayText) == false
					 ? authorizedProduct.AssignedLoadRackDisplayText
					 : authorizedProduct.AssignedID;
		}

		public LineItemDO AddLineItem(Guid loadArmGuid)
		{
			this.Transaction.DeleteFlag = false;

			if (this.Transaction.RouteSchedule.FST == null)
			{
				this.Transaction.RouteSchedule.FST = TimeConverter.Now(this.SiteManager.Site);
			}

			LoadArmClass loadArm = this.GetLoadArm(loadArmGuid);
			ProcessVariableClass loadArmPv = loadArm.ProcessVariableCollection[0];
			LoadArmManagerClass loadArmManager = this.GetLoadArmManager(loadArm);

			if (loadArmManager == null)
			{
				return null;
			}

			Server server = new Server(new Factory(), new URL(loadArmPv.URL));
			server.Connect(new ConnectData(null));

			UnitsHelperClass unitHelper = new UnitsHelperClass(this.Security, this.SiteManager.Site, this.CurrentTransactionAlias, null);

			if (this.InRecircMode)
			{
				StorageTransferLineItemDO lineItemDO = new StorageTransferLineItemDO
				{
					Status = TransactionStatus.InProgress,
					PresetAmount = 0.0,
					OperatorID = this.Transaction.OperatorID,
					OperatorPersonnelGuid = this.Transaction.OperatorPersonnelGuid,
					BatchNumber = loadArmManager.GetBatchNumber(this)
				};




				this.ReadBatchRecipe(lineItemDO, server, loadArmManager);

				lineItemDO.StorageLocationID = this.FromStorageLocationID;
				lineItemDO.StorageLocationTankGuid = this.FromStorageLocationTankGuid;

				lineItemDO.ToStorageLocation = this.ToStorageLocationID;
				lineItemDO.ToStorageLocationTankGuid = this.ToStorageLocationTankGuid;
				lineItemDO.ArmNumber = loadArm.BayAArmNumber;

				ProductMapClass component = loadArmManager.GetComponent(loadArmManager.CurrentLineItemProduct.IdentityGuid);
				if (component == null)
				{
					throw new Exception("Component not found in LoadArm Configuration");
				}

				lineItemDO.ProductType = ProductClass.ProductTypeID(ProductType.ComponentProduct);
				lineItemDO.Product = component.AssignedID;
				lineItemDO.ProductCode = component.AssignedCode;
				lineItemDO.ProductGuid = component.AssignedGuid;

				unitHelper.SetUnits(lineItemDO, ProductType.ComponentProduct, null);

				this.Transaction.LineItems.Add(lineItemDO);

				this.ReadLineItemData(lineItemDO, server, loadArmManager);

				lineItemDO.UpdateDeliveredQuantities(this.Station.EthanolExcess, loadArmManager.LoadArm, true);

				server.Disconnect();
				server.Dispose();

				this.LastActivityDateTime = DateTimeOffset.Now;

				return lineItemDO;
			}

			LineItemDO lineItem = new LineItemDO { Status = TransactionStatus.InProgress, PresetAmount = 0.0 };

			if (this.Station.SwingArmPosition == "A")
			{
				lineItem.ArmNumber = loadArm.BayAArmNumber;
			}
			else
			{
				lineItem.ArmNumber = loadArm.BayBArmNumber;
			}

			// Set the equipment id
			if (string.IsNullOrEmpty(loadArmManager.NonPreloadEquipmentSelection) == false)
			{
				EquipmentClass equipment = this.GetEquipmentClass(loadArmManager.NonPreloadEquipmentSelection);

				if (equipment != null)
				{
					lineItem.DestinationEQ = new EquipmentDO
					{
						RegistrationID = equipment.ID,
						EquipmentGuid = equipment.MasterRecordGuid,
						SerialNumber = equipment.SerialNumber,
						EquipmentModel = equipment.Model,
						EquipmentType = EquipmentTypeClass.TypeID(equipment.Type),
						CompanyEquipmentID = equipment.CompanyEquipmentID
					};
				}
			}

			// Set the compartment selection
			if (loadArmManager.NonPreloadCompartmentSelection > 0)
			{
				lineItem.DestinationCompartmentID = loadArmManager.NonPreloadCompartmentSelection.ToString();
			}

			lineItem.OperatorID = this.Transaction.OperatorID;
			lineItem.OperatorPersonnelGuid = this.Transaction.OperatorPersonnelGuid;

			lineItem.BatchNumber = loadArmManager.GetBatchNumber(this);

			this.ReadBatchRecipe(lineItem, server, loadArmManager);

			this.ReadPresetAmount(lineItem, server, loadArmManager);

			// Recipe is a component, determine preset number and get Non-Resettable Totals
			if (loadArmManager.CurrentLineItemProduct.ProductType == ProductType.ComponentProduct)
			{
				ProductMapClass component = loadArmManager.GetComponent(loadArmManager.CurrentLineItemProduct.IdentityGuid);
				if (component == null)
				{
					throw new Exception("Component not found in LoadArm Configuration");
				}

				lineItem.MeterID = component.MeterID;

				TankClass tank = this.SiteManager.GetTank(component, this.Manager);
				if (tank != null)
				{
					lineItem.StorageLocationID = tank.ID;
					lineItem.StorageLocationTankGuid = tank.IdentityGuid;
				}
			}

			this.Transaction.LineItems.Add(lineItem);

			if (this.Station.Type == STATION_TYPE.OFF_LOADING)
			{
				this.ReadLineItemData(lineItem, server, loadArmManager);

				lineItem.UpdateDeliveredQuantities(this.Station.EthanolExcess, loadArmManager.LoadArm, true);

				server.Disconnect();
				server.Dispose();

				this.LastActivityDateTime = DateTimeOffset.Now;

				if (this.SupplyOrder != null)
				{
					LineItemDO orderLineItem = this.FindMatchingOrderLineItem(this.SupplyOrder, lineItem.Product);

					if (orderLineItem != null)
					{
						lineItem.OrderReferenceTransactionLineItemGuid = orderLineItem.TransactionLineItemGuid;
						// store the supply order po number in the receipt transaction po number field citgo requirement
						this.Transaction.PONumber = this.SupplyOrder.PONumber;
					}
				}

				return lineItem;
			}

			if (this.Order != null)
			{
				LineItemDO orderLineItem = this.FindMatchingOrderLineItem(this.Order, lineItem.Product);

				if (orderLineItem != null)
				{
					lineItem.OrderReferenceTransactionLineItemGuid = orderLineItem.TransactionLineItemGuid;
				}
			}

			// Add Blend Components
			if (loadArmManager.CurrentLineItemProduct.ProductType == ProductType.BlendProduct)
			{
				this.CreateBlendSubLineItems(lineItem, loadArmManager.CurrentLineItemProduct, loadArmManager, server);
			}

			var productGuid = FMChannelHelper.MakeCall<IProducts, Guid>(x => x.GetIdentityGuid(this.Security, lineItem.Product));

			ProductMapClass authorizedProduct =	this.ShipTo.AuthorizedProductCollection.Find(x => x.AssignedGuid == productGuid);
			if (authorizedProduct == null)
			{
				throw new Exception("Authorized Product not found in Ship To Authorized Products");
			}

			ContaminationPromptStatus contaminationPromptStatus =
				this.GetContaminationPromptStatus(authorizedProduct.ContaminationPromptLoadRackText);
			if (contaminationPromptStatus != null)
			{
				lineItem.ContaminatePrompt = contaminationPromptStatus.ContaminatePrompt;
				lineItem.CompartmentsPreviouslyLoaded = contaminationPromptStatus.CompartmentsPreviouslyLoaded;
				lineItem.CompartmentsEmpty = contaminationPromptStatus.CompartmentsEmpty;
			}

			// Add the Additive Volumes
			loadArmManager.AdditiveProfile = null;

			if (authorizedProduct.AdditiveProfileGuid != Guid.Empty)
			{
				lineItem.AdditiveProfileID = authorizedProduct.AdditiveProfileID;
				lineItem.AdditiveProfileGuid = authorizedProduct.AdditiveProfileGuid;
				loadArmManager.AdditiveProfile =
					FMChannelHelper.MakeCall<IAdditiveProfiles, AdditiveProfileClass>(
						x => x.Get(this.Security, authorizedProduct.AdditiveProfileGuid));
			}

			if (loadArmManager.AdditiveProfile != null)
			{
				foreach (ProductMapClass additive in loadArmManager.AdditiveProfile.AdditiveCollection)
				{
					ProductMapClass additiveInjector = loadArmManager.GetAdditive(additive.AssignedGuid);
					if (additiveInjector == null)
					{
						throw new Exception("Additive not found in LoadArm Additive Configuration");
					}

					SubLineItemDO subLineItem = new SubLineItemDO
					{
						Status = lineItem.Status,
						BatchNumber = lineItem.BatchNumber,
						Density = 0.0,
						Temperature = 0.0,
						PresetAmount = 0.0,
						LineNumber = additiveInjector.PresetNumber,
						ProductType = ProductClass.ProductTypeID(ProductType.AdditiveProduct),
						Product = additiveInjector.AssignedID,
						ProductCode = additiveInjector.AssignedCode,
						ProductGuid = additiveInjector.AssignedGuid
					};

					if (this.Station.SwingArmPosition == "A")
					{
						subLineItem.ArmNumber = loadArm.BayAArmNumber;
					}
					else
					{
						subLineItem.ArmNumber = loadArm.BayBArmNumber;
					}

					unitHelper.SetUnits(subLineItem, ProductType.AdditiveProduct, null);

					// Units helper sets units for transaction detail, for load rack the AdditiveVolumeUnits must match the Preset for reported volumes
					// All Presets must match the Site Configuration.
					subLineItem.VolumeUnits = this.SiteManager.Site.AdditiveVolumeUnits;
					subLineItem.VolumeDecimalPlaces = this.SiteManager.Site._AdditiveVolumeDecimalPlaces;

					// Cycle Volume and rate use the AdditiveProfileUnits, but the actual volumes use the Accounting units.
					// These are for setting the Preset Volume on the transaction, so use the AdditiveVolumeUnits as above
					double cycleVolume = StationManagerClass.Convert(additive._AdditiveCycleVolume.SIValue, EngineeringUnit.FmvMeter3, subLineItem.VolumeUnits);
					double rate = StationManagerClass.Convert(additive._AdditiveRate.SIValue, EngineeringUnit.FmvMeter3, subLineItem.VolumeUnits);

					// For Blends need to consider Products Using Injector, we must
					// assume that the Blend Component Configuration will match
					if (loadArmManager.CurrentLineItemProduct.ProductType == ProductType.BlendProduct)
					{
						byte productsUsingInjector = this.ReadAdditiveProductsUsingInjector(additiveInjector, server, loadArmManager);

						double totalExternalComponentPercentage = 0;
						double totalComponentPercentage = 0;
						foreach (ProductMapClass component in loadArmManager.CurrentLineItemProduct.ComponentCollection)
						{
							ProductMapClass armComponent = loadArmManager.GetComponent(component.AssignedGuid);
							if (armComponent == null)
							{
								throw new Exception("Component not found in LoadArm Configuration");
							}

							if (armComponent.Type == PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_MAP
										  || armComponent.Type == PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_TANKGROUP_MAP)
							{
								totalExternalComponentPercentage += component.BlendPercentage;
							}
							else
							{
								totalComponentPercentage += component.BlendPercentage;
							}
						}

						foreach (ProductMapClass component in loadArmManager.CurrentLineItemProduct.ComponentCollection)
						{
							ProductMapClass armComponent = loadArmManager.GetComponent(component.AssignedGuid);
							if (armComponent == null)
							{
								throw new Exception("Component not found in LoadArm Configuration");
							}

							if (armComponent.Type == PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_MAP)
							{
								continue;
							}

							if ((productsUsingInjector & (1 << (armComponent.PresetNumber - 1))) != 0)
							{
								double presetComponentPercentage = component.BlendPercentage
																			  + component.BlendPercentage / totalComponentPercentage
																			  * totalExternalComponentPercentage;
								double lineItemPresetAmount = lineItem.PresetAmount ?? 0.0;
								if (subLineItem.VolumeUnits != lineItem.VolumeUnits)
								{
									lineItemPresetAmount = StationManagerClass.Convert(lineItem.PresetAmount ?? 0.0, lineItem.VolumeUnits, subLineItem.VolumeUnits);
								}

								subLineItem.PresetAmount += (int)(lineItemPresetAmount * presetComponentPercentage / 100.0 / rate)
																	 * cycleVolume;
							}
						}
					}

					else
					{
						double lineItemPresetAmount = lineItem.PresetAmount.Value;
						if (subLineItem.VolumeUnits != lineItem.VolumeUnits)
						{
							lineItemPresetAmount = StationManagerClass.Convert(lineItem.PresetAmount.Value, lineItem.VolumeUnits, subLineItem.VolumeUnits);
						}

						subLineItem.PresetAmount = (int)(lineItemPresetAmount / rate) * cycleVolume;
					}

					subLineItem.MeterID = additiveInjector.Meter.ID;

					TankClass tank = this.SiteManager.GetTank(additiveInjector, this.Manager);
					if (tank != null)
					{
						subLineItem.StorageLocationID = tank.ID;
						subLineItem.StorageLocationTankGuid = tank.IdentityGuid;
					}

					subLineItem.DosageRate = additive._AdditiveCycleVolume.Value * 1000 / additive._AdditiveRate.Value;

					lineItem.SubLineItems.Add(subLineItem);
				}
			}

			this.ReadLineItemData(lineItem, server, loadArmManager);

			lineItem.UpdateDeliveredQuantities(this.Station.EthanolExcess, loadArmManager.LoadArm, true);


			server.Disconnect();
			server.Dispose();

			this.LastActivityDateTime = DateTimeOffset.Now;

			return lineItem;
		}

		public bool AuthorizeLoadArm(LoadArmManagerClass loadArmManager, LineItemDO lineItem, double preset, ulong recipeMap)
		{
			// We need to update the recipe map if we are doing a preload since we need to only
			// allocate a single product for this batch
			if (this.PreloadInProgress)
			{
				ProductMapClass recipe;

				if (this.RemoteAuthorized && this.RemoteSubLineItem != null && this.RemoteSubLineItem.ProductGuid != Guid.Empty)
				{
					recipe = loadArmManager.GetRecipe(this.RemoteSubLineItem.ProductGuid);
				}
				else if (lineItem.ProductGuid != Guid.Empty
							&& (lineItem.SplashBlendingMap == null || loadArmManager.Bay(this).SplashProducts == null
								 || loadArmManager.Bay(this).SplashProducts.Count == 0))
				{
					recipe = loadArmManager.GetRecipe(lineItem.ProductGuid);
				}
				else
				{
					recipe = loadArmManager.GetRecipe(lineItem.SplashBlendingMap.AssignedGuid);
				}

				if (recipe != null)
				{
					ProductMapClass authorizedProduct = this.GetAuthorizedProduct(recipe.AssignedID);

					if (lineItem.AdditiveProfileGuid != Guid.Empty)
					{
						loadArmManager.AdditiveProfile = FMChannelHelper.MakeCall<IAdditiveProfiles, AdditiveProfileClass>(
																	 x =>
																	 x.Get(this.Security, lineItem.AdditiveProfileGuid)
																);
					}
					else
					{
						loadArmManager.AdditiveProfile = null;
					}

                    int downloadedRecipe = 0;
                    try
                    {
                        downloadedRecipe = this.WriteSingleRecipe(loadArmManager, recipe);
								if (downloadedRecipe == 0)
								{
                            this.DisplayMessageWithAcknowledge("LoadRack|Recipe Write Error");
									 this.AddAlarmAndEventLogs(this.Security, this.Station.DynamicRecipeDownloadErrorAlarm(this.Station.ID));
									 WriteLogDataToCommFile($"Unable to write recipe {recipe.AssignedID} to preset", CommLogDirection.None);
 									 if (this == loadArmManager.GetStationManager())
									 {
										loadArmManager.LogOutOfProgramMode();
									 }
									 return false;
                        }
                        loadArmManager.Bay(this).RecipeMap |= (ulong)0x1 << (downloadedRecipe - 1);
                        this.RecipeInternalNumberMap.Add(downloadedRecipe, recipe);
                    }
                    catch (ArgumentException ae)
                    {
                        _ = ae;
                        this.DisplayMessageWithAcknowledge("LoadRack|Recipe Write Error");
                        WriteLogDataToCommFile("Error writing recipe to preset: duplicate internal recipe number", CommLogDirection.None);

						this.eventLog.WriteEntry($"Error writing recipe {recipe.ID} to preset# {downloadedRecipe}, duplicate internal recipe number", EventLogEntryType.Error);

                  return false;
               }

               string name = GetLoadRackDisplayText(authorizedProduct);

					ProductClass product =
						 FMChannelHelper.MakeCall<IProducts, ProductClass>(
							  x => x.Get(this.Security, authorizedProduct.AssignedGuid));

					if (!loadArmManager.UpdateRecipe(name, recipe, product, loadArmManager.AdditiveProfile, downloadedRecipe))
					{
						if (!loadArmManager.LogOutOfProgramMode())
						{
							return false;
						}

						this.StationState = StationState.UPDATE_RECIPE_ERROR_MSG;
						loadArmManager.DisplayMessage("LoadRack|Update Recipe Error", 0, this.MESSAGE_TIMEOUT);
						return false;
					}

					if (this.Station.SynchronizeReferenceDensity && !loadArmManager.UpdateReferenceDensity(this))
					{
						if (!loadArmManager.LogOutOfProgramMode())
						{
							return false;
						}

						loadArmManager.DisplayMessage("LoadRack|Update Density Error", 0, this.MESSAGE_TIMEOUT);
						return false;
					}

					if (!loadArmManager.LogOutOfProgramMode())
					{
						return false;
					}

					recipeMap = (ulong)0x1 << (loadArmManager.GetRecipeNumber(recipe) - 1);
				}
			}

			if (recipeMap != 0)
			{
				if (!loadArmManager.AllocateRecipes(recipeMap))
				{
					loadArmManager.DisplayMessage("LoadRack|Allocate Recipe Error", 0, this.MESSAGE_TIMEOUT);

					if (this.RemoteAuthorized)
					{
						throw new Exception("Allocate Recipe Error");
					}

					return false;
				}

				loadArmManager.CaptureMeterValues();

				 if (loadArmManager.IssueSelectRecipePrompt(this, preset))
				 {
					  return true;
				 }

					 if (!this.LateLoadArmAuthorizedProductCheck(loadArmManager))
					 {
						  loadArmManager.DisplayMessage("[LoadRack|No Authorized Products]", 0);

						  return false;
					 }

				if (loadArmManager.IssuePresetPrompt(this, preset))
				{
					return true;
				}

				if (!loadArmManager.Authorize(this, preset))
				{
					if (this.RemoteAuthorized)
					{
						throw new Exception("Authorize To Preset Error");
					}

					return false;
				}

				if (this.Transaction != null && this.Transaction.RouteSchedule.FST == null)
				{
					this.Transaction.RouteSchedule.FST = TimeConverter.Now(this.SiteManager.Site);
				}

				this.LastActivityDateTime = DateTimeOffset.Now;
				this.StartDateTime = DateTimeOffset.Now;

				if (this.Transaction != null)
				{
					this.Transaction.Status = TransactionStatus.InProgress;
				}

				if (lineItem != null)
				{
					lineItem.Status = TransactionStatus.InProgress;
				}

				return true;
			}

			 if (this.RemoteAuthorized)
			 {
				  throw new Exception("Could not authorize product on load arm.");
			 }
			 
				loadArmManager.DisplayMessage("[LoadRack|No Authorized Products]", 0);

			 return false;
		}

		 protected virtual bool LateLoadArmAuthorizedProductCheck(LoadArmManagerClass loadArmManager)
		 {
				// Most stations can ignore this check; exceptions
				// are stations where TAS controls product selection after arm selection.
			  return true;
		 }

		public bool AuthorizeLoadArm(LoadArmManagerClass loadArmManager, LineItemDO lineItem)
		{
			// Prepare Load Arm
			double PresetAmount = lineItem.PresetAmount.Value;
			if (this.RemoteAuthorized && this.RemoteSubLineItem != null)
			{
				PresetAmount = this.RemoteSubLineItem.PresetAmount.Value;
			}
			else if (lineItem.SplashBlendingMap != null)
			{
				PresetAmount = PresetAmount * lineItem.SplashBlendingMap.BlendPercentage / 100;
			}

			lineItem.ArmNumber = loadArmManager.GetArmNumber(this);
			lineItem.Density = 0.0;
			lineItem.Temperature = 0.0;

			return this.AuthorizeLoadArm(loadArmManager, lineItem, PresetAmount, 0);
		}

		protected void EquipmentCardLogIn(EquipmentClass trailer)
		{
			this.AddAlarmAndEventLogs(this.Security, this.Station.EquipmentCardLogInEvent(trailer));
			this.LoadRackManager.EventOrAlarmEvent.Set();
		}

		public virtual void CheckAndUploadStoreTransactions()
		{
		}

		public bool CheckDriverEquipmentQualsAndTraining(EquipmentClass EquipmentToCheck)
		{
			EquipmentTypeClass equipmentType = FMChannelHelper.MakeCall<IEquipmentTypes, EquipmentTypeClass>(
																	 x =>
																	 x.Get(this.Security, EquipmentToCheck.EquipmentTypeGuid)
																);
			if (this.Driver.QualificationCollection.Count == 0 && equipmentType.ReqQualificationsCollection.Count > 0)
			{
				this.DisplayMessage("[LoadRack|Driver] [LoadRack|Not Qualified]", null, 0, this.MESSAGE_TIMEOUT);
				this.StationState = StationState.RESET_ON_TIMEOUT;
				return false;
			}
			else if (this.Driver.TrainingCollection.Count == 0 && equipmentType.ReqTrainingCollection.Count > 0)
			{
				this.DisplayMessage("[LoadRack|Driver] [LoadRack|Not Trained]", null, 0, this.MESSAGE_TIMEOUT);
				this.StationState = StationState.RESET_ON_TIMEOUT;
				return false;
			}
			else if (equipmentType.ReqQualificationsCollection.Count > 0 || equipmentType.ReqTrainingCollection.Count > 0)
			{
				bool bQualifaicationsOK = false;
				bool bTrainingOK = false;
				// check the qualifications
				foreach (QualificationMapClass ReqQualification in equipmentType.ReqQualificationsCollection)
				{
					bQualifaicationsOK = false;
					foreach (QualificationMapClass Qualification in this.Driver.QualificationCollection)
					{
						// check eack station qualification and if not accessed inform the driver
						if (ReqQualification.AssignedGuid == Qualification.AssignedGuid)
						{
							bQualifaicationsOK = true;
						}
					}
					if (bQualifaicationsOK == false)
					{
						this.DisplayMessage("[LoadRack|Driver] [LoadRack|Not Qualified]", null, 0, this.MESSAGE_TIMEOUT);
						this.StationState = StationState.RESET_ON_TIMEOUT;
						return false;
					}
				}
				// check the training
				foreach (QualificationMapClass ReqQualification in equipmentType.ReqTrainingCollection)
				{
					bTrainingOK = false;
					foreach (QualificationMapClass Qualification in this.Driver.TrainingCollection)
					{
						// check eack station qualification and if not accessed inform the driver
						if (ReqQualification.AssignedGuid == Qualification.AssignedGuid)
						{
							bTrainingOK = true;
						}
					}
					if (bTrainingOK == false)
					{
						this.DisplayMessage("[LoadRack|Driver] [LoadRack|Not Trained]", null, 0, this.MESSAGE_TIMEOUT);
						this.StationState = StationState.RESET_ON_TIMEOUT;
						return false;
					}
				}
			}
			return true;
		}

		public virtual void CheckDriverMessages(bool acknowledged)
		{
			if (this.StationState != StationState.DRIVER_MESSAGE_PROMPT)
			{
				this.DriverMessageIndex = 0;

				// In entity record versioning terms, Messages are an external client of both Companies
				// and Personnel.  External clients use Master Record Guids for references.
				this.MessageCollection = FMChannelHelper.MakeCall<IMessages, MessageCollectionClass>(
																	 x =>
																	 x.EnumerateByGuids(this.Security, this.Carrier.MasterRecordGuid, this.Driver.MasterRecordGuid)
																);
			}

			for (; this.DriverMessageIndex < this.MessageCollection.Count; this.DriverMessageIndex++)
			{
				MessageClass message = this.MessageCollection[this.DriverMessageIndex];

				// Note: Exception handling incase someone sends a message
				//			that cannot be output to the station
				try
				{
					if (this.StationState == StationState.DRIVER_MESSAGE_PROMPT)
					{
						if (acknowledged)
						{
							MessageLogClass messageLog = new MessageLogClass
							{
								MessageGuid = message.IdentityGuid,
								CompanyGuid = this.Carrier != null ? this.Carrier.MasterRecordGuid : Guid.Empty,
								PersonnelGuid = this.Driver.MasterRecordGuid
							};
							this.AddMessageLogs(this.Security, messageLog);
							this.StationState = StationState.IDLE;
							continue;
						}

						this.StationState = StationState.IDLE;
						this.DisplayMessage("LoadRack|Message Timeout", null, 0, this.MESSAGE_TIMEOUT);
						this.ConsecutivePrompts = 0;
						return;
					}

					if (message._LocationType == MessageLocationType.LoadRack && (this.Station.Type == STATION_TYPE.ENTRY_GATE || this.Station.Type == STATION_TYPE.EXIT_GATE))
					{
						continue;
					}

					if (message._LocationType == MessageLocationType.Gate && (this.Station.Type == STATION_TYPE.LOAD_RACK || this.Station.Type == STATION_TYPE.OFF_LOADING))
					{
						continue;
					}

					if (message._FrequencyType == MessageFrequencyType.Once)
					{
						MessageLogClass messageLog = this.GetMessageLogs(
							this.Security, message.IdentityGuid, this.Carrier.MasterRecordGuid, this.Driver.MasterRecordGuid);
						if (messageLog.MessageGuid != Guid.Empty)
						{
							continue;
						}
					}

					if (message._FrequencyType == MessageFrequencyType.OncePerDay)
					{
						MessageLogClass messageLog = this.GetTodaysMessageLogs(
							this.Security, message.IdentityGuid, this.Carrier.MasterRecordGuid, this.Driver.MasterRecordGuid);
						if (messageLog.MessageGuid != Guid.Empty)
						{
							continue;
						}
					}

					this.StationState = StationState.DRIVER_MESSAGE_PROMPT;
					this.DisplayMessageWithAcknowledge(message.ID);
					return;
				}
				catch (Exception e)
				{
					this.eventLog.WriteEntry(e.ToString(), EventLogEntryType.Error);
				}
			}

			if (this.Station.Type == STATION_TYPE.ENTRY_GATE)
			{
				this.CardIn();
				this.OpenGate();
			}

			else if (this.Station.Type == STATION_TYPE.EXIT_GATE)
			{
				this.OpenGate();
			}

			else if (this.Station.Type == STATION_TYPE.LOAD_RACK)
			{
				this.ConsecutivePrompts = 0;

				if (this.PromptForPreLoadSelection())
				{
					return;
				}


				if (this.Station.InhibitLoadingByLoadID &&
				!this.SiteManager.Site.PromptForShipmentNumber)
				{
					FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
						 x => x.Add(this.Security, this.Station.LoadbyLoadidInhibitedEvent(this.Station.ID)));
					this.LoadRackManager.EventOrAlarmEvent.Set();
					this.StationState = StationState.RESET_ON_TIMEOUT;
					this.DisplayMessage("LoadRack|Load by LoadID Inhibited", null, 0, this.MESSAGE_TIMEOUT);
					return;
				}

				this.LoadInprogressTransactions();

				if (this.SiteManager.Site.PromptForShipmentNumber &&
				!this.Station.InhibitLoadingByLoadID)
				{
					this.IssueLoadByShipmentOrLoadIDPrompt();
					return;
				}

				if (this.Station.InhibitLoadingByLoadID &&
				this.SiteManager.Site.PromptForShipmentNumber)
				{
					// prompt for shipment number
					this.IssueEnterShipmentNumberPrompt();
					return;
				}

				// Prompt for LoadID
				this.IssueLoadIDPrompt();
			}
			else if (this.Station.Type == STATION_TYPE.WEIGHT_SCALE || this.Station.Type == STATION_TYPE.PRELOAD)
			{
				this.CheckForActiveTransactions();
			}
			else if (this.Station.Type == STATION_TYPE.OFF_LOADING)
			{
				string response = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(this.SiteManager.Site.SiteGuid, "LoadRack|UnLoading")
																);
				this.ProcessOperatingMode(response);
			}
		}

		protected void LoadInprogressTransactions()
		{
			this.inprogressTransaction = new TransactionDO[0];

			// TODO: Load in progress transactions to be implemented later
			////if (!this.SiteManager.Site.PromptForTransactionCompletion)
			////{
			////    return;
			////}

			////var getTransactionSR = new GetTransactionSR
			////{
			////    Security = this.Security,
			////    Request =
			////                                   GetTransactionRequest.SITE_TYPEID_TRANSDATE_STATUS_CARRIERINDEX_OPERATORINDEX,
			////    BeginningDate = new DateTime(1753, 1, 1),
			////    EndingDate = new DateTime(9999, 12, 31),
			////    Site = this.SiteManager.Site.ID,
			////    TransTypeID = TransactionTypes.T5_PrimaryDisbursement,
			////    OperatorIndex = this.Driver.Index,
			////    CarrierIndex = this.Carrier.Index,
			////    Status = ((int)TransactionStatus.InProgress).ToString(CultureInfo.InvariantCulture)
			////};

			////var accountingService = new AccountingServiceImpl();
			////var getTransactionDO = (GetTransactionDO)accountingService.processRequest(getTransactionSR);

			////if (getTransactionDO != null
			////&& getTransactionDO.TransactionDataSet != null
			////&& getTransactionDO.TransactionDataSet.Tables.Count != 0
			////&& getTransactionDO.TransactionDataSet.Tables[0].Rows.Count != 0)
			////{
			////    this.inprogressTransaction = new TransactionDO[getTransactionDO.TransactionDataSet.Tables[0].Rows.Count];

			////    int index = 0;
			////    foreach (DataRow row in getTransactionDO.TransactionDataSet.Tables[0].Rows)
			////    {
			////        this.inprogressTransaction[index] = this.GetTransaction((string)row["TransID"]);
			////        index++;
			////    }
			////}
		}

		private MessageLogClass GetTodaysMessageLogs(SecurityClass securityClass, Guid guid1, Guid guid2, Guid guid3)
		{
			return FMChannelHelper.MakeCall<IMessageLogs, MessageLogClass>(
																	 x =>
																	 x.GetToday(securityClass, guid1, guid2, guid3)
																);
		}

		private MessageLogClass GetMessageLogs(SecurityClass securityClass, Guid guid1, Guid guid2, Guid guid3)
		{
			return FMChannelHelper.MakeCall<IMessageLogs, MessageLogClass>(
																	 x =>
																	 x.Get(securityClass, guid1, guid2, guid3)
																);
		}

		private void AddMessageLogs(SecurityClass securityClass, MessageLogClass MessageLog)
		{
			FMChannelHelper.MakeCall<IMessageLogs>(
																	 x =>
																	 x.Add(securityClass, MessageLog)
																);
		}

		public void CheckForActiveTransactions()
		{
			GetTransactionSR getTransactionSR = new GetTransactionSR
			{
				Security = this.Security,
				Request = GetTransactionRequest.SITE_TYPEID_TRANSDATE_STATUS_OPERATORPERSONNELGUID,
				Site = this.SiteManager.Site.ID,
				TransTypeID = TransactionTypes.T5_PrimaryDisbursement,
				BeginningDate = TimeConverter.MinFMDate,
				EndingDate = TimeConverter.MaxFMDate,
				OperatorPersonnelGuid = this.Driver.MasterRecordGuid
			};
			GetTransactionDO getTransactionDO;

			if (this.Station.Type == STATION_TYPE.WEIGHT_SCALE)
			{
				// First get WeightOutPending,only check for Weight_Scale type
				getTransactionSR.Status = ((int)TransactionStatus.WeighOutPending).ToString();

				getTransactionDO = FMChannelHelper.MakeCall<IGetTransactionProcessor, GetTransactionDO>(
																	 x =>
																	 x.Process(getTransactionSR)
																);
				// Second get LoadPending
				if (getTransactionDO == null || getTransactionDO.TransactionDataSet == null
					 || getTransactionDO.TransactionDataSet.Tables.Count == 0
					 || getTransactionDO.TransactionDataSet.Tables[0].Rows.Count == 0)
				{
					getTransactionSR.Status = ((int)TransactionStatus.LoadPending).ToString();
					getTransactionDO = FMChannelHelper.MakeCall<IGetTransactionProcessor, GetTransactionDO>(
																	 x =>
																	 x.Process(getTransactionSR)
																);
				}
			}
			else //for Preload station, only check if there is a LoadingPending
			{
				getTransactionSR.Status = ((int)TransactionStatus.LoadPending).ToString();

				getTransactionDO = FMChannelHelper.MakeCall<IGetTransactionProcessor, GetTransactionDO>(
																	 x =>
																	 x.Process(getTransactionSR)
																);
			}

			// Load the Transactions
			if (getTransactionDO != null && getTransactionDO.TransactionDataSet != null
				 && getTransactionDO.TransactionDataSet.Tables.Count != 0
				 && getTransactionDO.TransactionDataSet.Tables[0].Rows.Count != 0)
			{
				for (int Index = 0; Index < getTransactionDO.TransactionDataSet.Tables[0].Rows.Count; Index++)
				{
					this.Transaction = this.GetTransaction(
						(string)getTransactionDO.TransactionDataSet.Tables[0].Rows[Index]["TransID"]);
					if (this.Transaction != null)
					{
						this.PendingTransactions.Add(this.Transaction);
					}
				}

				// Retrieve the Tractor and Trailer(s)
				if (string.IsNullOrEmpty(this.Transaction.DestinationEQ1.RegistrationID) == false)
				{
					EquipmentClass Equipment = FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(
																	 x =>
																	 x.Get(this.Security, x.GetIdentityGuid(this.Security, this.Transaction.DestinationEQ1.RegistrationID))
																);

					if (Equipment.Type == EQUIPMENT_TYPE.TRAILER_TYPE)
					{
						this.Trailer1 = Equipment;
					}
					else
					{
						this.TractorOrTanker = Equipment;
					}
				}

				if (string.IsNullOrEmpty(this.Transaction.DestinationEQ2.RegistrationID) == false)
				{
					EquipmentClass Equipment = FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(
																	 x =>
																	 x.Get(this.Security, x.GetIdentityGuid(this.Security, this.Transaction.DestinationEQ2.RegistrationID))
																);
					if (this.Trailer1 != null)
					{
						this.Trailer2 = Equipment;
					}
					else
					{
						this.Trailer1 = Equipment;
					}
				}

				if (string.IsNullOrEmpty(this.Transaction.DestinationEQ3.RegistrationID) == false)
				{
					this.Trailer2 = FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(
																	 x =>
																	 x.Get(this.Security, x.GetIdentityGuid(this.Security, this.Transaction.DestinationEQ3.RegistrationID))
																);
				}

				if (this.Transaction.TransTypeID == TransactionTypes.T5_PrimaryDisbursement)
				{
					this.Mode = OperatingMode.Loading;
				}
				else
				{
					this.Mode = OperatingMode.Unloading;
				}

				this.Transaction = null;

				if (getTransactionSR.Status == ((int)TransactionStatus.WeighOutPending).ToString())
				{
					this.IssueCaptureExitWeightPrompt(false);
					return;
				}

				// Driver has carded in at the Weight Scale again before loading
				// this might happen if someone modifies Site while Driver is configuring a load
				else
				{
					if (this.TotalAvailableCompartments == 0)
					{
						this.StationState = StationState.RESET_ON_TIMEOUT;
						this.DisplayMessage("LoadRack|No Compartments to Load", null, 0, this.MESSAGE_TIMEOUT);
						return;
					}

					this.IssueUseOrderNumberPrompt();
				}
			}
			else
			{
				if (this.Station.Type == STATION_TYPE.WEIGHT_SCALE)
				{
					this.IssueOperatingModePrompt();
				}
				else if (this.Station.Type == STATION_TYPE.PRELOAD)
				{
					string response = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(this.SiteManager.Site.SiteGuid, "LoadRack|Loading")
																);
					this.ProcessOperatingMode(response);
					//for preload, skip the "loading/unloading" prompt, go to processingOperatinMode directly
				}
			}

			FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
																	 x =>
																	 x.Add(this.Security, this.Driver.CardUseSuccessfulEventEvent(this.Station.ID))
																);
		}

		protected virtual bool IsIncludeFlowControlledAdditivesInProductTotals(Server server)
		{
			return false;
		}

		public void CloseOutLineItem(LineItemDO lineItem)
		{
			LoadArmClass loadArm = this.Station.LoadArmCollection[lineItem.ArmNumber.Value - 1];
			LoadArmManagerClass loadArmManager = this.GetLoadArmManager(loadArm);

			// If the arm number is not set, this line item is only set to status InProgress but is not actually loading
			if (lineItem.ArmNumber == null)
			{
				return;
			}

			// If we are splash blending, we only want to set the line to completed if all the components
			// have been loaded.
			if (lineItem.SplashBlendingMap != null)
			{
				this.CloseOutSplashBlendLineItem(lineItem);
			}

			else
			{

				// When LineItem Product is a Blend add subline items for external components.
				ProductClass product = FMChannelHelper.MakeCall<IProducts, ProductClass>(
																	 x =>
																	 x.GetByProductAuthorizedCompanies(this.Security, lineItem.ProductGuid, false)


																							);

				if (product.ProductType == ProductType.BlendProduct && loadArmManager != null)
				{
					ArrayList externalComponentSubLineItems = new ArrayList();

					double totalComponentPercent = 0.0;

					foreach (ProductMapClass blendComponent in product.ComponentCollection)
					{
						ProductMapClass component = loadArmManager.GetComponent(blendComponent.AssignedGuid);
						if (component == null)
						{
							throw new Exception("Component not found in LoadArm Configuration");
						}

						if (component.Type == PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_MAP
									 || component.Type == PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_TANKGROUP_MAP)
						{
							continue;
						}

						totalComponentPercent += blendComponent.BlendPercentage;
					}

					double totalExternalComponentPercent = 0;

					foreach (ProductMapClass externalBlendComponent in product.ComponentCollection)
					{
						ProductMapClass armComponent = loadArmManager.GetComponent(externalBlendComponent.AssignedGuid);
						if (armComponent == null)
						{
							throw new Exception("Component not found in LoadArm Configuration");
						}

						if (armComponent.Type != PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_MAP
									 && armComponent.Type != PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_TANKGROUP_MAP)
						{
							continue;
						}

						ProductClass externalProduct = this.GetByProductAuthorizedCompanies(
							this.Security, externalBlendComponent.AssignedGuid, false);

						ProcessVariableClass blendPercentagePv =
							armComponent.ProcessVariableCollection[PROCESS_VARIABLE_TYPE.BLEND_PERCENTAGE_PV];

						double blendPercentage = 0.0;
						try
						{
							this.OPCServerManager.Read(blendPercentagePv);
							if (!blendPercentagePv.IsQualityGood)
							{
								throw new Exception(
									 "CreateExternalComponentSubLineItem : External Component Blend Percentage OPC Quality Bad "
									 + blendPercentagePv.OPCItemID);
							}

							blendPercentage = System.Convert.ToDouble(blendPercentagePv.ServerValue) / 10000;
						}
						catch (Exception)
						{
							this.AddAlarmAndEventLogs(this.Security, this.Station.ExternalComponentPercentageBadAlarm(loadArm.ID, externalProduct.ID, blendPercentagePv.OPCItemID));
						}

						totalExternalComponentPercent += blendPercentage;

						foreach (SubLineItemDO subLineItem in lineItem.SubLineItems)
						{
							if (subLineItem.ProductType == ProductClass.ProductTypeID(ProductType.AdditiveProduct))
							{
								continue;
							}

							ProductMapClass blendComponent = product.ComponentCollection.Find(x => x.AssignedGuid == subLineItem.ProductGuid);

							double externalComponentPercent = externalBlendComponent.BlendPercentage * blendComponent.BlendPercentage
																		 / (totalComponentPercent * 100);

							externalComponentSubLineItems.Add(
								this.CreateExternalComponentSubLineItem(
									loadArmManager, lineItem, subLineItem, armComponent, externalComponentPercent, blendPercentage, externalProduct));
						}
					}

					if (externalComponentSubLineItems.Count != 0)
					{
						// Adjust the SublineItem MeterTotalizers by the ExternalComponentVolumes
						foreach (SubLineItemDO externalComponentSubLineItem in externalComponentSubLineItems)
						{
							foreach (SubLineItemDO subLineItem in lineItem.SubLineItems)
							{
								if (subLineItem.ProductType == ProductClass.ProductTypeID(ProductType.AdditiveProduct))
								{
									continue;
								}

								if (subLineItem.MeterID != externalComponentSubLineItem.MeterID)
								{
									continue;
								}

								if (subLineItem.MeterReading.StartDateTime != null)
								{
									externalComponentSubLineItem.MeterReading.StartDateTime = subLineItem.MeterReading.StartDateTime.Value;
								}

								if (subLineItem.MeterReading.StopDateTime != null)
								{
									externalComponentSubLineItem.MeterReading.StopDateTime = subLineItem.MeterReading.StopDateTime.Value;
								}

								if (subLineItem.MeterReading.MeterStop != null && subLineItem.MeterReading.MeterStart != null)
								{
									externalComponentSubLineItem.MeterReading.MeterStop = subLineItem.MeterReading.MeterStop.Value;
									if (this.SiteManager.Site.LoadByNet)
									{
										subLineItem.MeterReading.MeterStop += externalComponentSubLineItem.Quantity.NetInventoryChange;
									}
									else
									{
										subLineItem.MeterReading.MeterStop += externalComponentSubLineItem.Quantity.GrossInventoryChange;
									}

									// Check for meter roll over
									if ((subLineItem.MeterReading.MeterStop ?? 0) < 0)
									{
										double rollover = 9;
										while (subLineItem.MeterReading.MeterStart.Value < rollover * 10 + 9)
										{
											rollover = rollover * 10 + 9;
										}

										subLineItem.MeterReading.MeterStop = rollover + (subLineItem.MeterReading.MeterStop ?? 0);
									}

									externalComponentSubLineItem.MeterReading.MeterStart = subLineItem.MeterReading.MeterStop ?? 0;
								}
							}
						}

						// Adjust Gross & Net for TotalExternalComponentPercentage;
						foreach (SubLineItemDO subLineItem in lineItem.SubLineItems)
						{
							if (subLineItem.ProductType == ProductClass.ProductTypeID(ProductType.AdditiveProduct))
							{
								continue;
							}

							subLineItem.Quantity.GrossInventoryChange *= 1 - totalExternalComponentPercent;
							subLineItem.Quantity.NetInventoryChange *= 1 - totalExternalComponentPercent;
							subLineItem.Quantity.MassInventoryChange *= 1 - totalExternalComponentPercent;

							subLineItem.Quantity.GrossInventoryChange = Math.Round(
								subLineItem.Quantity.GrossInventoryChange, subLineItem.VolumeDecimalPlaces, MidpointRounding.AwayFromZero);
							subLineItem.Quantity.NetInventoryChange = Math.Round(
								subLineItem.Quantity.NetInventoryChange, subLineItem.VolumeDecimalPlaces, MidpointRounding.AwayFromZero);
							subLineItem.Quantity.MassInventoryChange = Math.Round(
								subLineItem.Quantity.MassInventoryChange, subLineItem.MassDecimalPlaces, MidpointRounding.AwayFromZero);
						}

						// Add the ExternalSubLineItems to the LineItem
						foreach (SubLineItemDO externalComponentSubLineItem in externalComponentSubLineItems)
						{
							lineItem.SubLineItems.Add(externalComponentSubLineItem);
						}

						// Total the LineItem
						if (this.SiteManager.Site.LoadByNet)
						{
							lineItem.Quantity.GrossInventoryChange = 0;
						}
						else
						{
							lineItem.Quantity.NetInventoryChange = 0;
						}

						SIDouble lineItemVolume = new SIDouble(lineItem.VolumeUnits, lineItem.VolumeDecimalPlaces, 0);

						foreach (SubLineItemDO subLineItem in lineItem.SubLineItems)
						{
							if (subLineItem.ProductType == ProductClass.ProductTypeID(ProductType.AdditiveProduct))
							{
								continue;
							}

							if (lineItem.VolumeUnits == subLineItem.VolumeUnits)
							{
								if (this.SiteManager.Site.LoadByNet)
								{
									lineItem.Quantity.GrossInventoryChange += subLineItem.Quantity.GrossInventoryChange;
								}
								else
								{
									lineItem.Quantity.NetInventoryChange += subLineItem.Quantity.NetInventoryChange;
								}
							}
							else
							{
								SIDouble sublineItemVolume = new SIDouble(subLineItem.VolumeUnits, subLineItem.VolumeDecimalPlaces, 0);
								if (this.SiteManager.Site.LoadByNet)
								{
									sublineItemVolume.Value = subLineItem.Quantity.GrossInventoryChange;
								}
								else
								{
									sublineItemVolume.Value = subLineItem.Quantity.NetInventoryChange;
								}
								lineItemVolume.SIValue += sublineItemVolume.SIValue;
							}
						}
						if (this.SiteManager.Site.LoadByNet)
						{
							lineItem.Quantity.GrossInventoryChange += lineItemVolume.Value;
						}
						else
						{
							lineItem.Quantity.NetInventoryChange += lineItemVolume.Value;
						}
					}
				}
			}

			// For Blends, verify the Sum of the SubLineItems equals the LineItem
			if (lineItem.ProductType == ProductClass.ProductTypeID(ProductType.BlendProduct))
			{
				SIDouble grossInventoryChange = new SIDouble(lineItem.VolumeUnits, lineItem.VolumeDecimalPlaces, 0);
				SIDouble netInventoryChange = new SIDouble(lineItem.VolumeUnits, lineItem.VolumeDecimalPlaces, 0);
				SIDouble massInventoryChange = new SIDouble(lineItem.MassUnits, lineItem.MassDecimalPlaces, 0);
				foreach (SubLineItemDO subLineItem in lineItem.SubLineItems)
				{
					if (subLineItem.ProductType == ProductClass.ProductTypeID(ProductType.AdditiveProduct))
					{
						continue;
					}
					if (subLineItem.VolumeUnits == lineItem.VolumeUnits)
					{
						grossInventoryChange.Value += subLineItem.Quantity.GrossInventoryChange;
						netInventoryChange.Value += subLineItem.Quantity.NetInventoryChange;
					}
					else
					{
						SIDouble volume = new SIDouble(subLineItem.VolumeUnits, subLineItem.VolumeDecimalPlaces, 0)
						{
							Value = subLineItem.Quantity.GrossInventoryChange
						};
						grossInventoryChange.SIValue += volume.SIValue;
						volume.Value = subLineItem.Quantity.NetInventoryChange;
						netInventoryChange.SIValue += volume.SIValue;
					}

					if (subLineItem.MassUnits == lineItem.MassUnits)
					{
						massInventoryChange.Value += subLineItem.Quantity.MassInventoryChange;
					}
					else
					{
						SIDouble mass = new SIDouble(subLineItem.MassUnits, subLineItem.MassDecimalPlaces, 0) { Value = subLineItem.Quantity.MassInventoryChange };
						massInventoryChange.SIValue += mass.SIValue;
					}
				}

				double grossDiscrepency = Math.Abs(lineItem.Quantity.GrossInventoryChange - grossInventoryChange.Value);
				double netDiscrepency = Math.Abs(lineItem.Quantity.NetInventoryChange - netInventoryChange.Value);
				double massDiscrepency = 0.0;

				if (grossDiscrepency > 1 || netDiscrepency > 1 || massDiscrepency > 1)
				{
					double discrepency;
					if (netDiscrepency > grossDiscrepency)
					{
						discrepency = netDiscrepency;
					}
					else if (massDiscrepency > grossDiscrepency)
					{
						discrepency = massDiscrepency;
					}
					else
					{
						discrepency = grossDiscrepency;
					}

					FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
							x =>
							x.Add(this.Security, this.Station.BatchTotalDiscrepencyEvent(this.Transaction.DocumentNumber, lineItem.BatchNumber, discrepency))
					);
				}

				lineItem.Quantity.GrossInventoryChange = grossInventoryChange.Value;
				lineItem.Quantity.NetInventoryChange = netInventoryChange.Value;
				lineItem.Quantity.MassInventoryChange = massInventoryChange.Value;
			}

			if(lineItem.ProductType == ProductClass.ProductTypeID(ProductType.BlendProduct)
			&& lineItem.Quality != 0.0)
			{
				lineItem.VCF = Math.Round(lineItem.Quantity.NetInventoryChange / lineItem.Quantity.GrossInventoryChange, 5, MidpointRounding.AwayFromZero);
			}

			EthanolExpansionScenarioHelper ee = new EthanolExpansionScenarioHelper();
			if (ee.isEEScenario2(lineItem, this.Security))
			{
				int ethanolSubLineItemCount = lineItem.SubLineItems.Count(x => x.IsEthanol);
				if (ethanolSubLineItemCount == 1)
				{
					Dictionary<Guid, double> bobBlendPercentages = ee.getBobBlendPercentages(lineItem, this.Security);
					lineItem.UpdateDeliveredQuantities(this.Station.EthanolExcess, loadArmManager.LoadArm, true, bobBlendPercentages);
				}
				else if (ethanolSubLineItemCount > 1)
				{
					lineItem.UpdateLoadRackScenario2DeliveredQuantities(this.Station.EthanolExcess, loadArmManager.LoadArm, true);
				}
			}
			else
			{
				lineItem.UpdateDeliveredQuantities(this.Station.EthanolExcess, loadArmManager.LoadArm, true);
			}

			// Set the LineItem Status if we are not splash blending
			if (lineItem.SplashBlendingMap == null)
			{
				// ReSharper disable CompareOfFloatsByEqualityOperator
				TransactionStatus status;
				if (lineItem.Quantity == null
					|| (lineItem.Quantity.NetInventoryChange == 0.0 && lineItem.Quantity.GrossInventoryChange == 0.0
						 && lineItem.Quantity.MassInventoryChange == 0.0))
				{
					if (this.PreloadInProgress)
					{
						status = TransactionStatus.LoadPending;
					}
					else
					{
						status = TransactionStatus.Cancelled;
					}
				}
				else
				{
					status = TransactionStatus.Completed;
				}
				// ReSharper restore CompareOfFloatsByEqualityOperator

				lineItem.Status = status;

				foreach (SubLineItemDO subLineItem in lineItem.SubLineItems)
				{
					subLineItem.Status = status;
				}
			}

			// Check the lineitems now to see if the transaction status should be changed
			foreach (LineItemDO li in this.Transaction.LineItems)
			{
				if (li.Status == TransactionStatus.InProgress)
				{
					this.Transaction.Status = TransactionStatus.InProgress;
					break;
				}
				else if (li.Status == TransactionStatus.LoadPending)
				{
					this.Transaction.Status = TransactionStatus.LoadPending;
				}
			}

			this.LastActivityDateTime = DateTimeOffset.Now;

			lineItem.SplashBlendingMap = null;

			this.SiteManager.PermissiveEvent.Set();
		}

		protected ProductClass GetByProductAuthorizedCompanies(SecurityClass securityClass, Guid guid, bool getAuthorizedCompanies)
		{
			return FMChannelHelper.MakeCall<IProducts, ProductClass>(
																	 x =>
																	 x.GetByProductAuthorizedCompanies(securityClass, guid, getAuthorizedCompanies)
																);
		}

		public virtual void CompleteOffLoadingTransaction()
		{
			if (this.Transaction != null)
			{
				if (this.Transaction.Status == TransactionStatus.LoadPending
					 || this.Transaction.Status == TransactionStatus.InProgress)
				{
					if (this.Transaction.LineItems.Count == 0)
					{
						// if we are finishing and there are no line items, cancel the transaction
						this.Transaction.Status = TransactionStatus.Cancelled;
					}
					else
					{
						// Save the density and temperature in the one line item if we captured them
						if (this.Transaction.LineItems.Count > 0)
						{
							if (this.Station.PromptForGravity)
							{
										  this.Transaction.LineItems[0].Density = this.Transaction.LineItems[0].Density == null ? this.OffloadDensity : this.Transaction.LineItems[0].Density;
									 }

							if (this.Station.PromptForTemperature)
							{
										  this.Transaction.LineItems[0].Temperature = this.Transaction.LineItems[0].Temperature == null ? this.OffloadTemperature : this.Transaction.LineItems[0].Temperature;
							}
						}

						if (this.Station.PromptForBOLNumber)
						{
							this.Transaction.DocumentNumber = this.SelectedBOLNumber;
						}

						foreach (LineItemDO LineItem in this.Transaction.LineItems)
						{
							if (LineItem.Status == TransactionStatus.InProgress)
							{
								// closeout any current line items
								this.CloseOutLineItem(LineItem);
							}
						}
					}
				}

				this.Transaction.Status = TransactionStatus.Completed;

				DateTimeOffset siteTimeNow = TimeConverter.Now(this.SiteManager.Site);
				this.Transaction.TimeEnd = siteTimeNow;
				this.Transaction.TimeOut = siteTimeNow;

				// depending on the transaction type there may or may not be a driver
				if (this.Driver != null && this.Driver.OnFileSignature != null && this.Driver.OnFileSignature.Length > 0)
				{
					this.Transaction.Signature = this.Driver.OnFileSignature;
				}

				this.SaveTransaction();

				this.PrintTransaction();
				if (TransactionTypes.T8_Receipt == this.Transaction.TransTypeID)
				{
					this.SiteManager.LoadRackManager.ResetOwnerAllocationsInventoryDate(this.Security, this.Transaction.InventoryDate);
				}

				this.Transaction = null;
				this.SupplyOrder = null;
				this.Order = null;

				this.RemoteAuthorized = false;
			}

			this.LoadArmManagerCollection.SetState(this, LOADARM_STATE.NORMAL);
		}

		[SecurityCritical]
		public static double Convert(double source, EngineeringUnit sourceUnits, EngineeringUnit resultUnits)
		{
			// Use the accounting site conversion functions to convert
			double result = 0;

			EngineeringUnits.Convert(source, sourceUnits, ref result, resultUnits, 0);

			return result;
		}

		public virtual void CreateMeterReadingTransactions(
			SaveTransactionsSR saveTransactionsSR,
			TransactionAliasClass meterReadingTransactionAlias,
			DateTimeOffset inventoryDateTime)
		{
		}

		public
			void DetermineTypeOfOffLoadingOperation()
		{
			this.UseOffLoadSupplyOrders = false;
			if (this.Station.OffLoadByOffLoadID) // if the user designated off load id just do that and return
			{
				this.IssueOffLoadIDPrompt();
			}
			else
			{
				// determine if offloadids and supply orders both exist
				SupplyOrderListDO supplyorderListDO = this.GetSupplyOrders();

				if (supplyorderListDO != null && supplyorderListDO.LineItems.Count > 0)
				{
					this.StationState = StationState.USE_SUPPLYORDER_PROMPT;
					DisplayMenuParameters parameters = new DisplayMenuParameters(
						"Use Supply Order Number?", new[] { "LoadRack|Yes", "LoadRack|No" }, true, -1, this.PROMPT_TIMEOUT);
					this.DisplayMenu(parameters);
				}
				else
				{
					this.IssueOffLoadIDPrompt();
				}
			}
		}

		public virtual void DisplayOffLoadProductSelect()
		{
			// check that the supplier has authorized products configured
			if (this.Supplier.SupplierAuthorizedProductCollection.Count == 0)
			{
				this.AddAlarmAndEventLogs(this.Security, this.Station.NoProductsAvailableEvent(this.Station.ID));
				this.LoadRackManager.EventOrAlarmEvent.Set();

				this.StationState = StationState.RESET_ON_TIMEOUT;
				this.DisplayMessage("LoadRack|No Products Available", null, 0, this.MESSAGE_TIMEOUT);
				return;
			}

			// Build menu parameter set
			DisplayMenuParameters parameters = new DisplayMenuParameters
			{
				ApplyDataDictionary = true,
				DefaultItem = 0,
				MenuTimeout = this.PROMPT_TIMEOUT,
				SaveForCancelProcessing = false,
				Caption = "LoadRack|Select Off Load Product"
			};

			var menu = new List<string>();

			// Save last station state
			this.PriorStationState = this.StationState;

			ProductCollectionClass productCollection = FMChannelHelper.MakeCall<IProducts, ProductCollectionClass>(
				 x =>
					  x.Enumerate(this.Security)
				 );

			foreach (ProductClass product in productCollection)
			{
				// only add the products that are configured for the supplier
				foreach (ProductMapClass supplierProduct in this.Supplier.SupplierAuthorizedProductCollection)
				{
					if (supplierProduct.AssignedID == product.ID)
					{
						menu.Add(product.ID);
					}
				}
			}
			if (menu.Count == 0)
			{
				this.AddAlarmAndEventLogs(this.Security, this.Station.NoProductsAvailableEvent(this.Station.ID));
				this.LoadRackManager.EventOrAlarmEvent.Set();

				this.StationState = StationState.RESET_ON_TIMEOUT;
				this.DisplayMessage("LoadRack|No Products Available", null, 0, this.MESSAGE_TIMEOUT);
				return;
			}

			parameters.Menu = menu.ToArray();

			this.StationState = StationState.SELECT_OFFLOAD_PRODUCT;

			this.DisplayMenu(parameters);
		}

		public virtual void DisplayVerifySupplyOrderProduct()
		{
			//SelectedSupplyOrder
			bool bProductFound = false;
			//			CardID=Response;
			// Check for preloads for the current driver
			GetTransactionSR getTransactionSR = new GetTransactionSR
			{
				Security = this.Security,
				Request = GetTransactionRequest.SITE_TYPEID_ALIAS_DOCUMENTNUMBER,
				Site = this.SiteManager.Site.ID,
				TransTypeID = TransactionTypes.T18_SupplyOrder,
				Status = ((int)TransactionStatus.Scheduled).ToString(),
				DocumentNumber = this.SelectedSupplyOrder
			};

			GetTransactionDO getTransactionDO = FMChannelHelper.MakeCall<IGetTransactionProcessor, GetTransactionDO>(
																	x =>
																	x.Process(getTransactionSR)
															  );
			// Build menu parameter set
			DisplayMenuParameters parameters = new DisplayMenuParameters
			{
				ApplyDataDictionary = true,
				DefaultItem = 0,
				MenuTimeout = this.PROMPT_TIMEOUT,
				SaveForCancelProcessing = false,
				Caption = "LoadRack|Select Off Load Product"
			};


			ArrayList menu = new ArrayList();

			// Save last station state
			this.PriorStationState = this.StationState;

			if (getTransactionDO?.TransactionDataSet != null && getTransactionDO.TransactionDataSet.Tables.Count != 0 && getTransactionDO.TransactionDataSet.Tables[0].Rows.Count != 0)
			{
				foreach (DataRow row in getTransactionDO.TransactionDataSet.Tables[0].Rows)
				{
					string documentNumber = row["TransID"] as string;
					if (string.IsNullOrEmpty(documentNumber) == false)
					{
						this.SupplyOrder = this.GetTransaction(documentNumber);

						// check for multiple line items to verify product
						if (this.SupplyOrder.LineItems.Count > 0)
						{
							// check for different products in the line items and present the user with a selection
							foreach (LineItemDO lineItem in this.SupplyOrder.LineItems)
							{
								if (lineItem.Product != null && lineItem.Status == TransactionStatus.Scheduled)
								{
									bProductFound = true;
									// we do not need to worry about duplicate products since the save transaction will not allow
									// a supply order with a lineitem with a duplicate product
									menu.Add(lineItem.Product);

									parameters.Menu = (string[])menu.ToArray(typeof(string));
								}
							}
						}

						if (bProductFound == false)
						{
							this.AddAlarmAndEventLogs(this.Security, this.Station.NoProductsAvailableEvent(this.Station.ID));
							this.LoadRackManager.EventOrAlarmEvent.Set();

							this.StationState = StationState.RESET_ON_TIMEOUT;
							this.DisplayMessage("LoadRack|No Products Available", null, 0, this.MESSAGE_TIMEOUT);
							return;
						}

						if (this.SupplyOrder.ShipToCompanyGuid != Guid.Empty)
						{
							this.ShipTo = this.GetCompany(this.Security, this.SupplyOrder.ShipToCompanyGuid);
						}
						if (this.SupplyOrder.BillToCompanyGuid != Guid.Empty)
						{
							this.BillTo = this.GetCompany(this.Security, this.SupplyOrder.BillToCompanyGuid);
						}
						if (this.SupplyOrder.ShipperCompanyGuid != Guid.Empty)
						{
							this.Shipper = this.GetCompany(this.Security, this.SupplyOrder.ShipperCompanyGuid);
						}
						if (this.SupplyOrder.OwnerCompanyGuid != Guid.Empty)
						{
							this.Owner = this.GetCompany(this.Security, this.SupplyOrder.OwnerCompanyGuid);
						}
						if (this.SupplyOrder.ManagerCompanyGuid != Guid.Empty)
						{
							this.Manager = this.GetCompany(this.Security, this.SupplyOrder.ManagerCompanyGuid);
						}
						if (this.SupplyOrder.SupplierCompanyGuid != Guid.Empty)
						{
							this.Supplier = this.GetCompany(this.Security, this.SupplyOrder.SupplierCompanyGuid);
						}

						this.StationState = StationState.SELECT_OFFLOAD_PRODUCT;

						this.DisplayMenu(parameters);
						return;
					}
				}
				// default error message if any of the above does not complete
				this.DisplayMessage("[LoadRack|Invalid Selection]", null, 0, this.MESSAGE_TIMEOUT);
				this.StationState = StationState.INVALID_SUPPLIER_PROMPT_RESPONSE_MESSAGE;
			}
		}

		protected CompanyClass GetCompany(SecurityClass securityClass, Guid guid)
		{
			return FMChannelHelper.MakeCall<ICompanies, CompanyClass>(
																	 x =>
																	 x.Get(securityClass, guid)
																);
		}

		public virtual void DownloadConfigurationData()
		{
		}

		public virtual void EvaluateLoadArmStatus()
		{
			// Loop through the load arms and finish up if they are all at a good stopping point.
			foreach (LoadArmManagerClass loadArmManager in this.LoadArmManagerCollection)
			{
				if (this != loadArmManager.GetStationManager())
				{
					continue;
				}

				if (loadArmManager.LoadArmState == LOADARM_STATE.INPROGRESS
					 || loadArmManager.LoadArmState == LOADARM_STATE.INPROGRESS_PERMISSIVE_PROMPT
					 || loadArmManager.LoadArmState == LOADARM_STATE.END_BATCH_PROMPT
					 || loadArmManager.LoadArmState == LOADARM_STATE.BATCH_STOPPED_PROMPT)
				{
					return;
				}
			}

			bool AllArmsFinished = true;
			foreach (LoadArmManagerClass LAM in this.LoadArmManagerCollection)
			{
				if (this != LAM.GetStationManager())
				{
					continue;
				}

				if (LAM.LoadArmState != LOADARM_STATE.FINISHED
					 && LAM.LoadArmState != LOADARM_STATE.FINISHED_WITH_NO_PRODUCTS_TO_LOAD)
				{
					AllArmsFinished = false;
					break;
				}
			}

			// When all arms are idle it may be possible to complete the transaction
			if (!AllArmsFinished)
			{
				// In preload situation transaction can complete when all line items completed.
				if (this.PreloadDataSet != null && this.PreloadDataSet.Tables[0].Rows.Count != 0)
				{
					foreach (LineItemDO LineItem in this.Transaction.LineItems)
					{
						if (LineItem.Status != TransactionStatus.Completed)
						{
							return;
						}
					}
				}

				// In non preload situation transaciton can complete when all compartments loaded.
				else
				{
					if (this.CompartmentList != null && this.SiteManager.Site.PromptForCompartment)
					{
						foreach (CompartmentInfo Compartment in this.CompartmentList)
						{
							bool Found = false;
							foreach (LineItemDO LineItem in this.Transaction.LineItems)
							{
								if (LineItem.Status != TransactionStatus.Completed)
								{
									continue;
								}

								if (LineItem.DestinationEQ.EquipmentGuid == Guid.Empty)
								{
									continue;
								}

								if (LineItem.DestinationEQ.EquipmentGuid != Compartment.EquipmentGuid)
								{
									continue;
								}

								int CompartmentID = 0;
								try
								{
									CompartmentID = System.Convert.ToInt32(LineItem.DestinationCompartmentID);
								}
								catch
								{
									continue;
								}

								if (Compartment.CompartmentNumber == CompartmentID)
								{
									Found = true;
									break;
								}
							}

							if (!Found)
							{
								return;
							}
						}
					}
				}
			}

			this.SendEndTransaction();
			this.CompleteTransaction();
			if (this.StationState != StationState.BROKEN_BLEND && this.StationState != StationState.IMPROPER_ADDITIZATION)
			{
				this.ResetStationDevice();
			}
		}

		public bool IsEquipmentAvailableOnOrderForLoadArm(EquipmentClass equipment, LoadArmManagerClass loadArmManager)
		{
			foreach (LineItemDO orderLineItem in this.Order.LineItems)
			{
				if (orderLineItem.Status == TransactionStatus.Completed)
				{
					continue;
				}

				ProductMapClass recipe = loadArmManager.GetRecipe(orderLineItem.ProductGuid);
				if (recipe == null)
				{
					continue;
				}

					 if (!recipe.EnableRecipe)
					 {
						  continue;
					 }

				if (0 == (loadArmManager.Bay(this).RecipeMap & (ulong)0x1 << (loadArmManager.GetRecipeNumber(recipe) - 1)))
				{
					continue;
				}

				if (!this.IsOrderLineItemCompartmentAvailable(orderLineItem))
				{
					continue;
				}

				return true;
			}

			return false;
		}

		public virtual bool FinishLoading(bool transactionComplete)
		{
			foreach (LoadArmManagerClass loadArmManager in this.LoadArmManagerCollection)
			{
				if (this != loadArmManager.GetStationManager())
				{
					continue;
				}

				if (loadArmManager.LoadArmState == LOADARM_STATE.INPROGRESS)
				{
					if (!loadArmManager.IsInAlarm)
					{
						return false;
					}

					LineItemDO LineItem = this.GetLineItem(loadArmManager.LoadArm.IdentityGuid);

					if (LineItem != null)
					{
						loadArmManager.Unauthorize();
						this.UpdateLineItem(LineItem);
						this.CloseOutLineItem(LineItem);
						loadArmManager.LoadArmState = LOADARM_STATE.FINISHED;
					}
				}

				if (loadArmManager.LoadArmState == LOADARM_STATE.INPROGRESS_PERMISSIVE_PROMPT
					 || loadArmManager.LoadArmState == LOADARM_STATE.END_BATCH_PROMPT
					 || loadArmManager.LoadArmState == LOADARM_STATE.BATCH_STOPPED_PROMPT)
				{
					return false;
				}
			}

			this.LoadArmManagerCollection.ReleaseKeyPad(this);

			try
			{
				this.SendEndTransaction();
			}
			catch (Exception e)
			{
				this.eventLog.WriteEntry("StationManager FinishLoading " + e.Message, EventLogEntryType.Error);
				return false;
			}

			this.CompleteTransaction(transactionComplete);
			if (this.StationState != StationState.BROKEN_BLEND && this.StationState != StationState.IMPROPER_ADDITIZATION)
			{
				this.ResetStationDevice();
			}
			return true;
		}

		public ProductMapClass GetAuthorizedOffloadProduct(Guid productGuid)
		{
			foreach (ProductMapClass productMap in this.Supplier.SupplierAuthorizedProductCollection)
			{
				if (productMap.AssignedGuid == productGuid)
				{
					return productMap;
				}
			}

			return null;
		}

		public ProductMapClass GetAuthorizedProduct(string productID)
		{
			var productGuid = FMChannelHelper.MakeCall<IProducts, Guid>(x => x.GetIdentityGuid(this.Security, productID));

			foreach (ProductMapClass productMap in this.ShipTo.AuthorizedProductCollection)
			{
				if (productMap.AssignedGuid == productGuid)
				{
					return productMap;
				}
			}

			return null;
		}

		public string GetLoadRackDisplayText(Guid productGuid)
		{
			foreach (ProductMapClass authorizedProduct in this.ShipTo.AuthorizedProductCollection)
			{
				// Need to take in to account that the Authorized Product Collection for 
				// ship-tos uses the record version guid while the product mappings on the 
				// station and its load arms use master record guids
				ProductClass product = this.GetProductMinimalInfo(this.Security, authorizedProduct.AssignedGuid);

				if (productGuid == product.MasterRecordGuid)
				{
					return GetLoadRackDisplayText(authorizedProduct);
				}
			}

			return "";
		}

		public string GetOffloadStationDisplayText(Guid productGuid)
		{
			foreach (ProductMapClass authorizedProduct in this.Supplier.SupplierAuthorizedProductCollection)
			{
				// Need to take in to account that the Authorized Product Collection for 
				// ship-tos uses the record version guid while the product mappings on the 
				// station and its load arms use master record guids
				ProductClass product = this.GetProductMinimalInfo(this.Security, authorizedProduct.AssignedGuid);

				if (productGuid == product.MasterRecordGuid)
				{
					return GetLoadRackDisplayText(authorizedProduct);
				}
			}

			return "";
		}

		/// <summary>
		///    Returns a signature from the HandHeld Products signature pad
		/// </summary>
		/// <returns>Signature bytes.  Note that this is academic, as this function will always throw unless overridden</returns>
		/// <exception cref="NotImplementedException">Always.  This function must be overridden by derived classes</exception>
		/// <remarks>
		///    This function has a body to avoid breaking existing derived classes
		///    This function must be overridden to do anything other than throw a not implemented exception
		/// </remarks>
		public virtual byte[] GetSignature()
		{
			throw new NotImplementedException("GetSignature is not supported for this station type");
		}

		public string GetStationName()
		{
			return this.Station.ID;
		}

		public ArrayList GetUniqueEquipmentList()
		{
			ArrayList EquipmentList = new ArrayList();

			foreach (CompartmentInfo Info in this.CompartmentList)
			{
				if (this.InEquipmentList(EquipmentList, Info.EquipmentID) == false)
				{
					EquipmentList.Add(Info.EquipmentID);
				}
			}

			return EquipmentList;
		}

		protected bool AddEquipmentToTransaction(EquipmentClass equipment)
		{
			EquipmentDO equipmentDO = null;
			if (this.Transaction.DestinationEQ1.EquipmentGuid == Guid.Empty)
			{
				equipmentDO = this.Transaction.DestinationEQ1;
			}
			else if (this.Transaction.DestinationEQ2.EquipmentGuid == Guid.Empty)
			{
				equipmentDO = this.Transaction.DestinationEQ2;
			}
			else if (this.Transaction.DestinationEQ3.EquipmentGuid == Guid.Empty)
			{
				equipmentDO = this.Transaction.DestinationEQ3;
			}
			else if (this.Transaction.DestinationEQ4.EquipmentGuid == Guid.Empty)
			{
				equipmentDO = this.Transaction.DestinationEQ4;
			}

			if (equipmentDO == null)
			{
				return false;
			}

			equipmentDO.EquipmentModel = equipment.Model;
			equipmentDO.EquipmentType = EquipmentTypeClass.TypeID(equipment.Type);
			equipmentDO.RegistrationID = equipment.ID;
			equipmentDO.SerialNumber = equipment.SerialNumber;
			equipmentDO.EquipmentGuid = equipment.IdentityGuid;
			equipmentDO.CompanyEquipmentID = equipment.CompanyEquipmentID;

			return true;
		}

		public void InitializeTransaction()
		{
			this.CurrentBatchNumber = 0;
			this.ByWeight = false;
				this.ByWeightProduct = string.Empty;
				this.LoadSummaryIssued = false;
				this.SingleProduct = false;

			if (this.InRecircMode)
			{
				this.Transaction = new StorageTransferDO();
			}
			else
			{
				this.Transaction = new TransactionDO();
			}

			this.Transaction.TransID = FuelsManagerId.NewId();
			this.Transaction.TransTypeID = TransactionTypes.T5_PrimaryDisbursement;
			this.Transaction.Site = this.SiteManager.Site.ID;
			this.Transaction.SiteGuid = this.SiteManager.Site.IdentityGuid;
			this.Transaction.InventoryDate = this.SiteManager.GetInventoryDate().Date;
			this.Transaction.ManagerID = this.Manager.ID;
			this.Transaction.ManagerCode = this.Manager.Code;
			this.Transaction.ManagerCompanyGuid = this.Manager.MasterRecordGuid;
			this.Transaction.OwnerID = this.Owner.ID;
			this.Transaction.OwnerCode = this.Owner.Code;
			this.Transaction.OwnerCompanyGuid = this.Owner.MasterRecordGuid;

			UnitsHelperClass unitHelper = new UnitsHelperClass(this.Security, this.SiteManager.Site, this.CurrentTransactionAlias, null);
			unitHelper.SetUnits(this.Transaction, 0);

			if (this.Shipper != null)
			{
				this.Transaction.ShipperID = this.Shipper.ID;
				this.Transaction.ShipperCode = this.Shipper.Code;
				this.Transaction.ShipperCompanyGuid = this.Shipper.MasterRecordGuid;
			}

			if (this.BillTo != null)
			{
				this.Transaction.BillToID = this.BillTo.ID;
				this.Transaction.BillToCode = this.BillTo.Code;
				this.Transaction.BillToCompanyGuid = this.BillTo.MasterRecordGuid;
			}
			if (this.ShipTo != null)
			{
				this.Transaction.ShipToID = this.ShipTo.ID;
				this.Transaction.ShipToCode = this.ShipTo.Code;
				this.Transaction.ShipToCompanyGuid = this.ShipTo.MasterRecordGuid;
			}
			if (this.Carrier != null)
			{
				this.Transaction.CarrierID = this.Carrier.ID;
				this.Transaction.CarrierCode = this.Carrier.Code;
				this.Transaction.CarrierCompanyGuid = this.Carrier.MasterRecordGuid;
				this.Transaction.SCACCode = this.Carrier.SCACCode;
			}
			if (this.Supplier != null)
			{
				this.Transaction.SupplierID = this.Supplier.ID;
				this.Transaction.SupplierCode = this.Supplier.Code;
				this.Transaction.SupplierCompanyGuid = this.Supplier.MasterRecordGuid;
			}
			if (this.Driver != null) // for meter recirc there is no driver involved
			{
				this.Transaction.OperatorID = this.Driver.ID;
				this.Transaction.OperatorPersonnelGuid = this.Driver.MasterRecordGuid;
				this.Transaction.DriverIDNumber = this.CardID;
			}
			this.Transaction.Status = TransactionStatus.InProgress;
			this.Transaction.PONumber = this.PONumber;
			this.Transaction.LoadID = this.LoadID;
			this.Transaction.TimeIn = this.TimeIn;
			this.Transaction.OriginApplication = TransactionOrigin.TerminalAutomationService;

			if (this.TractorOrTanker != null)
			{
				this.Transaction.DestinationEQ1.EquipmentModel = this.TractorOrTanker.Model;
				this.Transaction.DestinationEQ1.EquipmentType = EquipmentTypeClass.TypeID(this.TractorOrTanker.Type);
				this.Transaction.DestinationEQ1.RegistrationID = this.TractorOrTanker.ID;
				this.Transaction.DestinationEQ1.SerialNumber = this.TractorOrTanker.SerialNumber;
				this.Transaction.DestinationEQ1.EquipmentGuid = this.TractorOrTanker.MasterRecordGuid;
				this.Transaction.DestinationEQ1.CompanyEquipmentID = this.TractorOrTanker.CompanyEquipmentID;

				if ((this.Station.Type == STATION_TYPE.WEIGHT_SCALE || this.Station.Type == STATION_TYPE.PRELOAD)
					 && this.TractorOrTanker.Type == EQUIPMENT_TYPE.TANKER_TYPE)
				{
					for (int CompartmentNumber = 1;
						  CompartmentNumber <= this.TractorOrTanker.CompartmentCollection.Count;
						  CompartmentNumber++)
					{
						if (this.CompartmentLoadPending(this.TractorOrTanker.IdentityGuid, CompartmentNumber.ToString()))
						{
							continue;
						}

						LineItemDO lineItemDO = new LineItemDO
						{
							Status = TransactionStatus.LoadPending
						};
						lineItemDO.DestinationEQ.EquipmentModel = this.TractorOrTanker.Model;
						lineItemDO.DestinationEQ.EquipmentType = EquipmentTypeClass.TypeID(this.TractorOrTanker.Type);
						lineItemDO.DestinationEQ.RegistrationID = this.TractorOrTanker.ID;
						lineItemDO.DestinationEQ.SerialNumber = this.TractorOrTanker.SerialNumber;
						lineItemDO.DestinationEQ.EquipmentGuid = this.TractorOrTanker.MasterRecordGuid;
						lineItemDO.DestinationEQ.CompanyEquipmentID = this.TractorOrTanker.CompanyEquipmentID;
						lineItemDO.DestinationCompartmentID = CompartmentNumber.ToString();
						lineItemDO.DestinationCompartmentEquipmentGuid =
							this.TractorOrTanker.CompartmentCollection[CompartmentNumber - 1].IdentityGuid;
						unitHelper.SetUnits(lineItemDO, 0, null);
						this.Transaction.LineItems.Add(lineItemDO);
					}
				}
			}

			if (this.Trailer1 != null)
			{
				if (this.TractorOrTanker != null)
				{
					this.Transaction.DestinationEQ2.EquipmentModel = this.Trailer1.Model;
					this.Transaction.DestinationEQ2.EquipmentType = EquipmentTypeClass.TypeID(this.Trailer1.Type);
					this.Transaction.DestinationEQ2.RegistrationID = this.Trailer1.ID;
					this.Transaction.DestinationEQ2.SerialNumber = this.Trailer1.SerialNumber;
					this.Transaction.DestinationEQ2.EquipmentGuid = this.Trailer1.MasterRecordGuid;
					this.Transaction.DestinationEQ2.CompanyEquipmentID = this.Trailer1.CompanyEquipmentID;
				}
				else
				{
					this.Transaction.DestinationEQ1.EquipmentModel = this.Trailer1.Model;
					this.Transaction.DestinationEQ1.EquipmentType = EquipmentTypeClass.TypeID(this.Trailer1.Type);
					this.Transaction.DestinationEQ1.RegistrationID = this.Trailer1.ID;
					this.Transaction.DestinationEQ1.SerialNumber = this.Trailer1.SerialNumber;
					this.Transaction.DestinationEQ1.EquipmentGuid = this.Trailer1.MasterRecordGuid;
					this.Transaction.DestinationEQ1.CompanyEquipmentID = this.Trailer1.CompanyEquipmentID;
				}

				if (this.Station.Type == STATION_TYPE.WEIGHT_SCALE || this.Station.Type == STATION_TYPE.PRELOAD)
				{
					for (int CompartmentNumber = 1;
						  CompartmentNumber <= this.Trailer1.CompartmentCollection.Count;
						  CompartmentNumber++)
					{
						if (this.CompartmentLoadPending(this.Trailer1.IdentityGuid, CompartmentNumber.ToString()))
						{
							continue;
						}

						LineItemDO lineItemDO = new LineItemDO
						{
							Status = TransactionStatus.LoadPending
						};
						lineItemDO.DestinationEQ.EquipmentModel = this.Trailer1.Model;
						lineItemDO.DestinationEQ.EquipmentType = EquipmentTypeClass.TypeID(this.Trailer1.Type);
						lineItemDO.DestinationEQ.RegistrationID = this.Trailer1.ID;
						lineItemDO.DestinationEQ.SerialNumber = this.Trailer1.SerialNumber;
						lineItemDO.DestinationEQ.EquipmentGuid = this.Trailer1.MasterRecordGuid;
						lineItemDO.DestinationEQ.CompanyEquipmentID = this.Trailer1.CompanyEquipmentID;
						lineItemDO.DestinationCompartmentID = CompartmentNumber.ToString();
						lineItemDO.DestinationCompartmentEquipmentGuid =
							this.Trailer1.CompartmentCollection[CompartmentNumber - 1].IdentityGuid;
						unitHelper.SetUnits(lineItemDO, 0, null);
						this.Transaction.LineItems.Add(lineItemDO);
					}
				}
			}

			if (this.Trailer2 != null)
			{
				if (this.TractorOrTanker != null)
				{
					this.Transaction.DestinationEQ3.EquipmentModel = this.Trailer2.Model;
					this.Transaction.DestinationEQ3.EquipmentType = EquipmentTypeClass.TypeID(this.Trailer2.Type);
					this.Transaction.DestinationEQ3.RegistrationID = this.Trailer2.ID;
					this.Transaction.DestinationEQ3.SerialNumber = this.Trailer2.SerialNumber;
					this.Transaction.DestinationEQ3.EquipmentGuid = this.Trailer2.MasterRecordGuid;
					this.Transaction.DestinationEQ3.CompanyEquipmentID = this.Trailer2.CompanyEquipmentID;
				}
				else
				{
					this.Transaction.DestinationEQ2.EquipmentModel = this.Trailer2.Model;
					this.Transaction.DestinationEQ2.EquipmentType = EquipmentTypeClass.TypeID(this.Trailer2.Type);
					this.Transaction.DestinationEQ2.RegistrationID = this.Trailer2.ID;
					this.Transaction.DestinationEQ2.SerialNumber = this.Trailer2.SerialNumber;
					this.Transaction.DestinationEQ2.EquipmentGuid = this.Trailer2.MasterRecordGuid;
					this.Transaction.DestinationEQ2.CompanyEquipmentID = this.Trailer2.CompanyEquipmentID;
				}

				if (this.Station.Type == STATION_TYPE.WEIGHT_SCALE || this.Station.Type == STATION_TYPE.PRELOAD)
				{
					for (int CompartmentNumber = 1;
						  CompartmentNumber <= this.Trailer2.CompartmentCollection.Count;
						  CompartmentNumber++)
					{
						if (this.CompartmentLoadPending(this.Trailer2.IdentityGuid, CompartmentNumber.ToString()))
						{
							continue;
						}

						LineItemDO lineItemDO = new LineItemDO
						{
							Status = TransactionStatus.LoadPending
						};
						lineItemDO.DestinationEQ.EquipmentModel = this.Trailer2.Model;
						lineItemDO.DestinationEQ.EquipmentType = EquipmentTypeClass.TypeID(this.Trailer2.Type);
						lineItemDO.DestinationEQ.RegistrationID = this.Trailer2.ID;
						lineItemDO.DestinationEQ.SerialNumber = this.Trailer2.SerialNumber;
						lineItemDO.DestinationEQ.EquipmentGuid = this.Trailer2.MasterRecordGuid;
						lineItemDO.DestinationEQ.CompanyEquipmentID = this.Trailer2.CompanyEquipmentID;
						lineItemDO.DestinationCompartmentID = CompartmentNumber.ToString();
						lineItemDO.DestinationCompartmentEquipmentGuid =
							this.Trailer2.CompartmentCollection[CompartmentNumber - 1].IdentityGuid;
						unitHelper.SetUnits(lineItemDO, 0, null);
						this.Transaction.LineItems.Add(lineItemDO);
					}
				}
			}
			if (this.Station.Type == STATION_TYPE.WEIGHT_SCALE)
			{
				WeightReadingDO WeightReading = new WeightReadingDO();
				if (this.CurrentWeight == null)
				{
					bool WeightScaleInMotion = false;
					bool WeightScaleMotionReadingInValid = false;
					this.ReadWeight(out this.CurrentWeight, out WeightScaleInMotion, out WeightScaleMotionReadingInValid);
				}
				WeightReading.BeginQuantity = System.Convert.ToDouble(this.CurrentWeight.Value);
				this.Transaction.WeightReadings.Add(WeightReading);
			}
		}

		public bool IsInCompartmentList(string ID)
		{
			if (this.CompartmentList != null && this.CompartmentList.Count > 0)
			{
				foreach (CompartmentInfo Info in this.CompartmentList)
				{
					if (Info.EquipmentID == ID && Info.Loaded == false)
					{
						return true;
					}
				}
			}

			return false;
		}

		public bool IsProductAvailable(
			ProductClass Product,
			LoadArmManagerClass LoadArmManager,
			double MaximumLoadAmount,
			TransactionAliasClass currentTransactionAlias)
		{
			if (Product.ProductType == ProductType.ComponentProduct)
			{
				ProductMapClass Component = LoadArmManager.GetComponent(Product.IdentityGuid);
				if (Component == null)
				{
					this.eventLog.WriteEntry("No Component for Recipe : " + Product.ID, EventLogEntryType.Error);
					return false;
				}

				if (!Component.Permissives.Permitted)
				{
					this.eventLog.WriteEntry("Component Disabled by Permissives : " + Component.AssignedID, EventLogEntryType.Error);
					return false;
				}

				TankClass Tank = this.SiteManager.GetTank(Component, this.Manager);
				if (Tank == null)
				{
					this.eventLog.WriteEntry("No Tank for Component : " + Component.AssignedID, EventLogEntryType.Error);
					return false;
				}

				ProcessVariableClass PV;
				if (!this.SiteManager.Site.LoadByNet)
				{
					PV = Tank.ProcessVariableCollection[PROCESS_VARIABLE_TYPE.AVAILABLE_GROSS_VOLUME_PV];
				}
				else
				{
					PV = Tank.ProcessVariableCollection[PROCESS_VARIABLE_TYPE.AVAILABLE_NET_VOLUME_PV];
				}

				if (PV == null)
				{
					this.eventLog.WriteEntry(
						"No Tank Available Volume Process Variable for Tank : " + Tank.ID, EventLogEntryType.Error);
					return false;
				}

				if ((!this.SiteManager.Site.UseLastKnownGoodTankData && !PV.IsQualityGood)
					 || !typeof(double).IsInstanceOfType(PV.SIValue))
				{
					this.eventLog.WriteEntry("Available Gross Volume OPC Quality Bad for Tank : " + Tank.ID, EventLogEntryType.Error);
					return false;
				}

				EngineeringUnit units = (currentTransactionAlias.VolumeUnits != 0)
													 ? currentTransactionAlias.VolumeUnits
													 : this.SiteManager.Site.VolumeUnits;
				byte decimalPlaces = (currentTransactionAlias.VolumeUnits != 0)
												? currentTransactionAlias._VolumeDecimalPlaces
												: this.SiteManager.Site._VolumeDecimalPlaces;

				double MaximumAvailable = (double)PV.GetValue(units, decimalPlaces);

				if (MaximumLoadAmount > MaximumAvailable)
				{
					return false;
				}
			}

			// Recipe is a Blend
			else
			{
				foreach (ProductMapClass BlendComponent in Product.ComponentCollection)
				{
					ProductMapClass Component = LoadArmManager.GetComponent(BlendComponent.AssignedGuid);
					if (Component == null)
					{
						this.eventLog.WriteEntry(
							"No Component for Blend Component : " + BlendComponent.AssignedID, EventLogEntryType.Error);
						return false;
					}

					if (!Component.Permissives.Permitted)
					{
						this.eventLog.WriteEntry(
							"Blend Component Disabled by Permissives : " + BlendComponent.AssignedID, EventLogEntryType.Error);
						return false;
					}

					TankClass Tank = this.SiteManager.GetTank(Component, this.Manager);
					if (Tank == null)
					{
						this.eventLog.WriteEntry("No Tank for Component : " + Component.AssignedID, EventLogEntryType.Error);
						return false;
					}

					ProcessVariableClass PV;
					if (!this.SiteManager.Site.LoadByNet)
					{
						PV = Tank.ProcessVariableCollection[PROCESS_VARIABLE_TYPE.AVAILABLE_GROSS_VOLUME_PV];
					}
					else
					{
						PV = Tank.ProcessVariableCollection[PROCESS_VARIABLE_TYPE.AVAILABLE_NET_VOLUME_PV];
					}

					if (PV == null)
					{
						this.eventLog.WriteEntry(
							"No Tank Available Volume Process Variable for Tank : " + Tank.ID, EventLogEntryType.Error);
						return false;
					}

					if ((!this.SiteManager.Site.UseLastKnownGoodTankData && !PV.IsQualityGood)
						 || !typeof(double).IsInstanceOfType(PV.SIValue))
					{
						this.eventLog.WriteEntry("Available Gross Volume OPC Quality Bad for Tank : " + Tank.ID, EventLogEntryType.Error);
						return false;
					}

					EngineeringUnit units = (currentTransactionAlias.VolumeUnits != 0)
														 ? currentTransactionAlias.VolumeUnits
														 : this.SiteManager.Site.VolumeUnits;
					byte decimalPlaces = (currentTransactionAlias.VolumeUnits != 0)
													? currentTransactionAlias._VolumeDecimalPlaces
													: this.SiteManager.Site._VolumeDecimalPlaces;

					double MaximumAvailable = (double)PV.GetValue(units, decimalPlaces);

					if (MaximumLoadAmount * BlendComponent.BlendPercentage / 100 > MaximumAvailable)
					{
						return false;
					}
				}
			}

			return true;
		}

		protected void IssueLineItemSummaryPrompt()
		{
			string dataDictionaryProduct = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(x => x.Get(this.SiteManager.Site.SiteGuid, "LoadRack|Product"));
			string dataDictionaryQuantity = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(x => x.Get(this.SiteManager.Site.SiteGuid, "LoadRack|Quantity"));

			List<string> batches = new List<string>();
			int lineItemNumber;
			for (lineItemNumber = 1; lineItemNumber <= this.Transaction.LineItems.Count; lineItemNumber++)
			{
				LineItemDO lineItem = this.Transaction.LineItems[lineItemNumber - 1];
				if (lineItem == null || lineItem.ProductGuid.IsEmpty())
				{
					batches.Add(lineItemNumber.ToString("D", this.SiteManager.Site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT)) +
						 "  " +
						 dataDictionaryProduct +
						 ": " +
						 FMChannelHelper.MakeCall<IDataDictionariesClass, string>(x => x.Get(this.SiteManager.Site.SiteGuid, "LoadRack|None")) +
						 "  " +
						 dataDictionaryQuantity +
						 ": " +
						 (lineItem?.Quantity.Gross.ToString("N", this.SiteManager.Site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME)) ?? string.Empty));
				}
				else
				{
					batches.Add(lineItemNumber.ToString("D", this.SiteManager.Site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT)) +
						 "  " +
						 dataDictionaryProduct +
						 ": " + this.GetLoadRackDisplayText(lineItem.ProductGuid) +
						 "  " +
						 dataDictionaryQuantity +
						 ": " +
						 lineItem.Quantity.Gross.ToString("N", this.SiteManager.Site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME)));
				}
			}

			batches.Add(
				 lineItemNumber.ToString("D", this.SiteManager.Site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT)) + "  "
				 + FMChannelHelper.MakeCall<IDataDictionariesClass, string>(x => x.Get(this.SiteManager.Site.SiteGuid, "LoadRack|<Add New Batch>")));
			batches.Add(
				 (lineItemNumber + 1).ToString("D", this.SiteManager.Site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT))
				 + "  " + FMChannelHelper.MakeCall<IDataDictionariesClass, string>(x => x.Get(this.SiteManager.Site.SiteGuid, "LoadRack|<Finish Loading>")));

			// ReSharper disable PossibleUnintendedReferenceComparison
			string prompt;
			if (this.CurrentEquipment == this.TractorOrTanker)
			{
				prompt = "[LoadRack|Tanker] " + this.EquipmentID(this.TractorOrTanker) + " [LoadRack|Batch(es)]";
			}
			else if (this.CurrentEquipment == this.Trailer1)
			{
				prompt = "[LoadRack|Trailer] " + this.EquipmentID(this.Trailer1) + " [LoadRack|Batch(es)]";
			}
			else if (this.CurrentEquipment == this.Trailer2)
			{
				prompt = "[LoadRack|Trailer] " + this.EquipmentID(this.Trailer2) + " [LoadRack|Batch(es)]";
			}
			else
			{
				prompt = "[LoadRack|Trailer] " + this.EquipmentID(this.Trailer3) + " [LoadRack|Batch(es)]";
			}

			// ReSharper restore PossibleUnintendedReferenceComparison
			int defaultItem = batches.Count - 1;
			this.StationState = StationState.LINEITEM_SUMMARY_PROMPT;

			DisplayMenuParameters parameters = new DisplayMenuParameters(prompt, batches.ToArray(), false, defaultItem, this.PROMPT_TIMEOUT);
			this.DisplayMenu(parameters);
		}

		public bool IsProductInUse(Guid identityGuid)
		{
			if (this.Station.Type != STATION_TYPE.LOAD_RACK)
			{
				return false;
			}

			if (this.StationState == StationState.IDLE || this.StationState == StationState.ENTER_DRIVER_ID_PROMPT)
			{
				return false;
			}

			foreach (LoadArmManagerClass LoadArmManager in this.LoadArmManagerCollection)
			{
				ProductMapClass Component = LoadArmManager.GetComponent(identityGuid);
				if (Component != null)
				{
					return true;
				}

				ProductMapClass Additive = LoadArmManager.GetAdditive(identityGuid);
				if (Additive != null)
				{
					return true;
				}

				ProductMapClass Recipe = LoadArmManager.GetRecipe(identityGuid);
				if (Recipe != null)
				{
					return true;
				}
			}

			return false;
		}

		public bool IsTankGroupInUse(Guid identityGuid)
		{
			if (this.Station.Type != STATION_TYPE.LOAD_RACK)
			{
				return false;
			}

			if (this.StationState == StationState.IDLE || this.StationState == StationState.ENTER_DRIVER_ID_PROMPT)
			{
				return false;
			}

			foreach (LoadArmManagerClass LoadArmManager in this.LoadArmManagerCollection)
			{
				foreach (ProductMapClass Component in LoadArmManager.LoadArm.ComponentCollection)
				{
					if (Component.Type == PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANKGROUP_MAP && Component.TankOrGroupGuid == identityGuid)
					{
						return true;
					}
				}
				foreach (ProductMapClass ExternalComponent in LoadArmManager.LoadArm.ExternalComponentCollection)
				{
					if (ExternalComponent.Type == PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_TANKGROUP_MAP
						 && ExternalComponent.TankOrGroupGuid == identityGuid)
					{
						return true;
					}
				}
			}

			return false;
		}

		public bool IsTankInUse(Guid identityGuid)
		{
			if (this.Station.Type != STATION_TYPE.LOAD_RACK)
			{
				return false;
			}

			if (this.StationState == StationState.IDLE || this.StationState == StationState.ENTER_DRIVER_ID_PROMPT)
			{
				return false;
			}

			foreach (LoadArmManagerClass LoadArmManager in this.LoadArmManagerCollection)
			{
				foreach (ProductMapClass Component in LoadArmManager.LoadArm.ComponentCollection)
				{
					if (Component.Type == PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANK_MAP && Component.TankOrGroupGuid == identityGuid)
					{
						return true;
					}
				}

				foreach (ProductMapClass Additive in LoadArmManager.LoadArm.AdditiveInjectorCollection)
				{
					if (Additive.Type == PRODUCT_MAP_TYPE.PRESET_INJECTOR_MAP && Additive.TankOrGroupGuid == identityGuid)
					{
						return true;
					}
				}
			}

			return false;
		}

		public bool IsTransactionInUse(string TransID)
		{
			if (this.Transaction != null)
			{
				if (TransID == this.Transaction.TransID)
				{
					return true;
				}
			}

			return false;
		}

		public void IssueLoadIDPrompt()
		{
			// This check should happen prior to clearing the companies;
			// if we're an order we've got the correct companies already
			// and should use them.
			if (this.SiteManager.Site.PromptForShipmentNumber
				 && this.Order != null)
			{
				this.StationState = StationState.IDLE;
				this.CheckProductAvailability(false);
				return;
			}

			this.Manager = null;
			this.Owner = null;
			this.Shipper = null;
			this.BillTo = null;
			this.ShipTo = null;

			// if this is a weight scale and the inhibit load id flag is set display an error message and return
			if ((this.Station.Type == STATION_TYPE.WEIGHT_SCALE || this.Station.Type == STATION_TYPE.PRELOAD || this.Station.Type == STATION_TYPE.MANUAL_BOL)
				 && this.Station.InhibitLoadingByLoadID)
			{
				FMChannelHelper.MakeCall<IAlarmAndEventLogs>(x => x.Add(this.Security, this.Station.NoOrdersAvailableEvent(this.Station.ID)));
				this.LoadRackManager.EventOrAlarmEvent.Set();
				this.StationState = StationState.RESET_ON_TIMEOUT;
				this.DisplayMessage("LoadRack|No Orders Available", null, 0, this.MESSAGE_TIMEOUT);
				this.ConsecutivePrompts = 0;
				return;
			}

			this.Transaction = null;

			if (this.SiteManager.Site.PromptForCustomerCard && this.Station.CardReader)
			{
				this.StationState = StationState.LOADID_CARD_PROMPT;
				this.PriorStationState = StationState.LOADID_CARD_PROMPT;
				this.DisplayMessage("LoadRack|Scan Load Card", null, 0, this.PROMPT_TIMEOUT);
			}
			else
			{
				this.StationState = StationState.LOADID_PROMPT;
				this.PriorStationState = StationState.LOADID_PROMPT;
				if (this.Station.Type == STATION_TYPE.LOAD_RACK)
				{
					this.DisplayMessage("[LoadRack|Enter] [LoadRack|Load ID]", null, PromptLength, this.PROMPT_TIMEOUT);
				}
				else
				{
					this.DisplayMessage("[LoadRack|Enter] [LoadRack|Load ID], [LoadRack| or press List]", null, PromptLength, this.PROMPT_TIMEOUT);
				}
			}
		}

		public virtual void IssuePleaseCardIn()
		{
			this.Driver = null;
			this.StationState = StationState.IDLE;
			this.DisplayMessage("LoadRack|Please Card In", null, 0, 0);
		}

		public virtual void IssueTouchKeyPleaseCardIn()
		{
			this.Driver = null;
			this.StationState = StationState.IDLE;
			this.DisplayMessage("[LoadRack|Scan Driver Key]", null, 0, 0);
		}

		public void LoadTransaction(string DocumentNumber)
		{
			foreach (DataRow Row in this.PreloadDataSet.Tables[0].Rows)
			{
				if (DocumentNumber == Row["DocumentNumber"] as string)
				{
					this.Transaction = this.GetTransaction((string)Row["TransID"]);
					break;
				}
			}

			if (this.Transaction != null)
			{
				// Check to make sure the compartments are all configured
				if (this.CheckCompartments(this.Transaction) == false)
				{
					this.Transaction = null;
					this.StationState = StationState.COMPARTMENTS_NOT_CONFIGURED;
					this.DisplayMessageWithAcknowledge("LoadRack|Compartments not configured");
					return;
				}

				// At this point we need to reset the InventoryDate.  It may
				// have the value set when the preload was created which may
				// not be correct.
				this.Transaction.InventoryDate = this.SiteManager.GetInventoryDate().Date;

				this.LoadArmManagerCollection.ResetPreloads(this);
				this.LoadArmManagerCollection.ResetSplashProducts(this);
				this.LoadArmManagerCollection.SetState(this, LOADARM_STATE.NORMAL);

				if (this.Transaction.ShipToCompanyGuid != Guid.Empty)
				{
					this.ShipTo = FMChannelHelper.MakeCall<ICompanies, CompanyClass>(
																	 x =>
																	 x.Get(this.Security, this.Transaction.ShipToCompanyGuid)
																);
					string CompanyHierarchy = this.Transaction.ManagerID + "->" + this.Transaction.OwnerID + "->"
													  + this.Transaction.ShipperID + "->" + this.Transaction.BillToID;
					this.CompanyMapCollection = this.EnumerateCompanyMapsByAssignedGuidAndType(
						this.Security, this.ShipTo.MasterRecordGuid, COMPANY_MAP_TYPE.SHIPTO_BILLTO_MAP);
					foreach (CompanyMapClass CompanyMap in this.CompanyMapCollection)
					{
						if (CompanyMap.AssignedToID == CompanyHierarchy)
						{
							if (!this.ValidateCompanyHierarchyLoadRack(CompanyMap))
							{
								this.AddAlarmAndEventLogs(this.Security, this.Station.CompanyHierarchyInvalidEvent(this.Driver.FirstLastName, this.ShipTo.ID));
								this.LoadRackManager.EventOrAlarmEvent.Set();

								this.StationState = StationState.RESET_ON_TIMEOUT;
								return;
							}

							this.StationState = StationState.IDLE;
							this.CheckProductAvailability(false);
							return;
						}
					}
				}

				// No Company Hierarchy, Validate Companies Individually
				if (this.Transaction.BillToCompanyGuid != Guid.Empty)
				{
					this.BillTo = this.GetCompany(this.Security, this.Transaction.BillToCompanyGuid);
				}
				if (this.Transaction.ShipperCompanyGuid != Guid.Empty)
				{
					this.Shipper = this.GetCompany(this.Security, this.Transaction.ShipperCompanyGuid);
				}
				if (this.Transaction.OwnerCompanyGuid != Guid.Empty)
				{
					this.Owner = this.GetCompany(this.Security, this.Transaction.OwnerCompanyGuid);
				}
				if (this.Transaction.ManagerCompanyGuid != Guid.Empty)
				{
					this.Manager = this.GetCompany(this.Security, this.Transaction.ManagerCompanyGuid);
				}

				if (!this.ValidateCompany(this.ShipTo, COMPANY_ROLE.CUSTOMER_SHIPTO)
					 || !this.ValidateCompany(this.BillTo, COMPANY_ROLE.CUSTOMER_BILLTO)
					 || !this.ValidateCompany(this.Shipper, COMPANY_ROLE.SHIPPER)
					 || !this.ValidateCompany(this.Owner, COMPANY_ROLE.OWNER)
					 || !this.ValidateCompany(this.Manager, COMPANY_ROLE.MANAGER))
				{
					FMChannelHelper.MakeCall<IAlarmAndEventLogs>(x => x.Add(this.Security, this.Station.CompanyHierarchyInvalidEvent(this.Driver.FirstLastName, this.ShipTo.ID)));
					this.LoadRackManager.EventOrAlarmEvent.Set();

					this.Transaction = null;
					this.StationState = StationState.RESET_ON_TIMEOUT;
					return;
				}

				this.StationState = StationState.IDLE;
				this.CheckProductAvailability(false);
			}
		}

		private CompanyMapCollectionClass EnumerateCompanyMapsByAssignedGuidAndType(SecurityClass securityClass, Guid guid, COMPANY_MAP_TYPE companyMapType)
		{
			return FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapCollectionClass>(
																	 x =>
																	 x.EnumerateByAssignedGuidAndType(securityClass, guid, companyMapType)
																);
		}

		public bool LoadingInProgress()
		{
			if (this.StationState == StationState.TRANSACTION_IN_PROGRESS)
			{
				return true;
			}

			if (this.StationState == StationState.AUTHORIZED)
			{
				return true;
			}

			foreach (LoadArmManagerClass LoadArmManager in this.LoadArmManagerCollection)
			{
				if (LoadArmManager.LoadingInProgress())
				{
					return true;
				}
			}

			return false;
		}

		public void ModifyProcessVariableMessage(ApplicationStringClass Message)
		{
			this.Station.StationPermissives.ModifyProcessVariableMessage(Message);
			this.LoadArmManagerCollection.ModifyProcessVariableMessage(Message);
		}

		public void ModifyProduct(ProductClass Product)
		{
			if (this.Station.Type != STATION_TYPE.LOAD_RACK)
			{
				return;
			}

			foreach (LoadArmManagerClass LoadArmManager in this.LoadArmManagerCollection)
			{
				ProductMapClass Component = LoadArmManager.GetComponent(Product.IdentityGuid);
				if (Component != null)
				{
					Component.AssignedID = Product.ID;
					Component.AssignedDescription = Product.Description;
					Component.AssignedCode = Product.Code;
				}

				ProductMapClass Additive = LoadArmManager.GetAdditive(Product.IdentityGuid);
				if (Additive != null)
				{
					Additive.AssignedID = Product.ID;
					Additive.AssignedDescription = Product.Description;
					Additive.AssignedCode = Product.Code;
				}

				ProductMapClass Recipe = LoadArmManager.GetRecipe(Product.IdentityGuid);
				if (Recipe != null)
				{
					Recipe.AssignedID = Product.ID;
					Recipe.AssignedDescription = Product.Description;
					Recipe.AssignedCode = Product.Code;
				}
			}
		}

		public void ModifyTank(TankClass Tank)
		{
			if (this.Station.Type != STATION_TYPE.LOAD_RACK)
			{
				return;
			}

			foreach (LoadArmManagerClass LoadArmManager in this.LoadArmManagerCollection)
			{
				foreach (ProductMapClass Component in LoadArmManager.LoadArm.ComponentCollection)
				{
					if (Component.Type == PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANK_MAP && Component.TankOrGroupGuid == Tank.IdentityGuid)
					{
						//Component.TankOrGroupID = Tank.ID;
						Component.AssignedGuid = Tank.ProductGuid;
						Component.AssignedID = Tank.ProductID;
					}
				}

				foreach (ProductMapClass Additive in LoadArmManager.LoadArm.AdditiveInjectorCollection)
				{
					if (Additive.Type == PRODUCT_MAP_TYPE.PRESET_INJECTOR_MAP && Additive.TankOrGroupGuid == Tank.IdentityGuid)
					{
						//Additive.TankOrGroupID = Tank.ID;
						Additive.AssignedGuid = Tank.ProductGuid;
						Additive.AssignedID = Tank.ProductID;
					}
				}
				// redo the recipe collection for this arm

				LoadArmManager.SetAvailableProductsCollection();
			}
		}

		public void ModifyTankGroup(TankGroupClass TankGroup)
		{
			if (this.Station.Type != STATION_TYPE.LOAD_RACK)
			{
				return;
			}

			foreach (LoadArmManagerClass LoadArmManager in this.LoadArmManagerCollection)
			{
				foreach (ProductMapClass Component in LoadArmManager.LoadArm.ComponentCollection)
				{
					if (Component.Type == PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANKGROUP_MAP
						 && Component.TankOrGroupGuid == TankGroup.IdentityGuid)
					{
						//Component.TankOrGroupID = TankGroup.ID;
						Component.AssignedGuid = TankGroup.ProductGuid;
						Component.AssignedID = TankGroup.ProductID;
					}
				}
				foreach (ProductMapClass ExternalComponent in LoadArmManager.LoadArm.ExternalComponentCollection)
				{
					if (ExternalComponent.Type == PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_TANKGROUP_MAP
						 && ExternalComponent.TankOrGroupGuid == TankGroup.IdentityGuid)
					{
						ExternalComponent.TankOrGroupID = TankGroup.ID;
						ExternalComponent.AssignedGuid = TankGroup.ProductGuid;
						ExternalComponent.AssignedID = TankGroup.ProductID;

						ExternalComponent.AssignedDescription = TankGroup.ProductID;
					}
				}
				// redo the recipe collection for this arm
				LoadArmManager.SetAvailableProductsCollection();
			}
		}

		public void ModifyTransactionAlias(TransactionAliasClass TransactionAlias)
		{
			if (this.Station.IssueByVolumeTransactionAliasGuid == TransactionAlias.MasterRecordGuid)
			{
				this.Station.IssueByVolumeTransactionAliasID = TransactionAlias.ID;
			}

			if (this.Station.IssueByWeightTransactionAliasGuid == TransactionAlias.MasterRecordGuid)
			{
				this.Station.IssueByWeightTransactionAliasID = TransactionAlias.ID;
			}

			if (this.Station.ReceiptByVolumeTransactionAliasGuid == TransactionAlias.MasterRecordGuid)
			{
				this.Station.ReceiptByVolumeTransactionAliasID = TransactionAlias.ID;
			}

			if (this.Station.ReceiptByWeightTransactionAliasGuid == TransactionAlias.MasterRecordGuid)
			{
				this.Station.ReceiptByWeightTransactionAliasID = TransactionAlias.ID;
			}
		}

		public virtual void ProcessPreloadOrderSelection(string response)
		{
			if (response == EscapeString)
			{
				this.ResetStationDevice();
				return;
			}

			foreach (GetTransactionTypeDO getTransactionTypeDO in this.OrderList)
			{
				if (getTransactionTypeDO.DocumentNumber == response)
				{
					// Load the transaction
					TransactionSR transSR = new TransactionSR
					{
						TransID = getTransactionTypeDO.TransID,
						Security = this.Security
					};

					TransactionDO Transaction = this.ProcessTransactionDO(transSR);

					this.Order = Transaction;
					this.IssueSelectPreloadDocument();
					return;
				}
			}

			this.StationState = StationState.INVALID_PRELOAD_ORDER_SELECTION_MSG;
			this.DisplayMessage("LoadRack|Invalid", null, 0, this.MESSAGE_TIMEOUT);
			return;
		}

		private TransactionDO ProcessTransactionDO(TransactionSR transSR)
		{
			return FMChannelHelper.MakeCall<ITransactionProcessor, TransactionDO>(
																	 x =>
																	 x.Process(transSR)
																);
		}

		protected void ProcessSelectCarrierCompany(string response)
		{
			if (EscapeString == response)
			{
				if (this.Station.CardReader)
				{
					this.IssuePleaseCardIn();
				}
				else if (this.Station.TouchKeyReader)
				{
					this.IssueTouchKeyPleaseCardIn();
				}
				else
				{
					this.IssueDriverIDPrompt();
				}
			}
			else
			{
				this.CarrierCompanyIndex = 0;
				if (this.Driver.AssignedCompaniesCollection.Count > 0)
				{
					// since the microload sends numbers we need to convert it here if posible
					try
					{
						int index = System.Convert.ToInt32(response);
						if (index == 0)
						{
							// cancel pressed
							if (this.Station.CardReader)
							{
								this.IssuePleaseCardIn();
							}
							else if (this.Station.TouchKeyReader)
							{
								this.IssueTouchKeyPleaseCardIn();
							}
							else
							{
								this.IssueDriverIDPrompt();
							}

							return;
						}

						// detrmine the name of the company
						response = this.CurrentMenuParameters.Menu[index - 1];
					}
					// ReSharper disable once EmptyGeneralCatchClause
					catch (Exception)
					{
						// if we get this it was not a number or was invalid
					}

					foreach (CompanyMapClass companyMap in this.Driver.AssignedCompaniesCollection)
					{
						if (companyMap.AssignedID == response)
						{
							this.Driver.CompanyGuid = companyMap.AssignedGuid;
							break;
						}
					}
				}
				else
				{
					this.StationState = StationState.RESET_ON_TIMEOUT;
					this.ConsecutivePrompts = 0;
					this.DisplayMessage("[LoadRack|Invalid] [LoadRack|Carrier]", null, 20, this.MESSAGE_TIMEOUT);
					return;
				}

				if (this.Driver.CompanyGuid.IsEmpty())
				{
					this.StationState = StationState.RESET_ON_TIMEOUT;
					this.ConsecutivePrompts = 0;
					this.DisplayMessage("[LoadRack|Invalid] [LoadRack|Carrier]", null, 20, this.MESSAGE_TIMEOUT);
					return;
				}

				this.FinishDriverCarrierProcessing();
			}
		}


		public void ProcessStopKey()
		{
			if (this.StationState != StationState.IDLE)
			{
				if (this.StationState == StationState.ENTER_DRIVER_ID_PROMPT)
				{
					this.IssueDriverIDPrompt();
				}
				else
				{
					this.IssueCancelTransactionMenu();
				}
			}
		}

		public void ProcessVerifySupplier(string response)
		{
			try
			{
				int selection = System.Convert.ToInt32(response);

				if (selection == 0 || selection > this.CurrentMenuParameters.Menu.Length)
				{
					this.IssueAdditionalOrdersPrompt();
					return;
				}

				response = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																					  x =>
																					  x.Get(this.SiteManager.Site.SiteGuid, this.CurrentMenuParameters.Menu[selection - 1])
																				);
			}
			catch (FormatException)
			{
			}
			catch (OverflowException)
			{
			}

			if (FMChannelHelper.MakeCall<IDataDictionariesClass, bool>(
																 x =>
																 x.Get(this.SiteManager.Site.SiteGuid, "LoadRack|Yes") == response))
			{
				if (this.Station.OffLoadByOffLoadID || this.UseOffLoadSupplyOrders == false)
				{
					Guid identityGuid = FMChannelHelper.MakeCall<ICompanyMaps, Guid>(
																	 x =>
																	 x.GetOffLoadIdentityGuidByMapID(this.Security, this.LoadID)
																);
					if (!this.ValidateOffLoadID(identityGuid))
					{
						return;
					}
				}

				switch (this.Station.Type)
				{
					case STATION_TYPE.PRELOAD:
					case STATION_TYPE.WEIGHT_SCALE:
						this.StationState = StationState.IDLE;

						// initialize the transaction here before product select
						if (!string.IsNullOrEmpty(this.SelectedSupplyOrder))
						{
							this.SupplyOrder = this.GetSupplyOrderByDocumentNumber(this.SelectedSupplyOrder);
						}

						this.InitializeTransaction();
						this.SupplyOrderTransactionAlias = FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasClass>(x => x.Get(this.Security, this.SupplyOrder.TransactionAliasGuid, false));
						this.CurrentTransactionAlias = FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasClass>(x => x.Get(this.Security, this.Station.ReceiptByVolumeTransactionAliasGuid, false));
						this.CheckOffloadProductAvailability(false);
						break;
					case STATION_TYPE.OFF_LOADING:
						// initialize the transaction here before product select
						this.InitializeTransaction();
						this.CurrentTransactionAlias = FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasClass>(x => x.Get(this.Security, this.Station.ReceiptByVolumeTransactionAliasGuid, false));

						if (this.Station.OffLoadByOffLoadID || this.UseOffLoadSupplyOrders == false)
						{
							this.DisplayOffLoadProductSelect();
						}
						else
						{
							this.DisplayVerifySupplyOrderProduct();
						}

						break;
				}
			}
			else if (FMChannelHelper.MakeCall<IDataDictionariesClass, string>(x => x.Get(this.SiteManager.Site.SiteGuid, "LoadRack|No")) == response || response == EscapeString)
			{
				if (this.Station.OffLoadByOffLoadID || this.UseOffLoadSupplyOrders == false)
				{
					this.IssueOffLoadIDPrompt();
				}
				else
				{
					this.PromptForSupplyOrderNumber();
				}
			}
			else
			{
				this.DisplayMessage("[LoadRack|Invalid Selection]", null, 0, this.MESSAGE_TIMEOUT);
				this.StationState = StationState.INVALID_SUPPLIER_PROMPT_RESPONSE_MESSAGE;
			}
		}

		public void PurgeProcessVariableMessage(Guid identityGuid)
		{
			this.Station.StationPermissives.PurgeProcessVariableMessage(identityGuid);
			this.LoadArmManagerCollection.PurgeProcessVariableMessage(identityGuid);
		}

		public void PurgeProduct(Guid identityGuid)
		{
			if (this.Station.Type != STATION_TYPE.LOAD_RACK)
			{
				return;
			}

			foreach (LoadArmManagerClass loadArmManager in this.LoadArmManagerCollection)
			{
				ProductMapClass component = loadArmManager.GetComponent(identityGuid);
				if (component != null)
				{
					component.AssignedGuid = Guid.Empty;
				}

				ProductMapClass additive = loadArmManager.GetAdditive(identityGuid);
				if (additive != null)
				{
					additive.AssignedGuid = Guid.Empty;
				}

				ProductMapClass recipe = loadArmManager.GetRecipe(identityGuid);
				if (recipe != null)
				{
					recipe.AssignedGuid = Guid.Empty;
				}
			}
		}

		public virtual bool RegisterPowerfail(string name)
		{
			return false;
		}

		private void SaveTransactionProcessor(SaveTransactionsSR saveTransactionsSR)
		{
			FMChannelHelper.MakeCall<ISaveTransactionsProcessor>(
																	 x =>
																	 x.SaveTransactions(saveTransactionsSR)
																);
		}

		private string GetNextDocumentNumberForSite(SecurityClass securityClass, DOCUMENT_TYPE documentType, Guid guid)
		{
			return FMChannelHelper.MakeCall<ISites, string>(
																	 x =>
																	 x.GetNextDocumentNumber(securityClass, documentType, guid)
																);
		}

		public virtual void SendEndTransaction()
		{
			this.LoadArmManagerCollection.SendEndTransaction(this);
		}

		public void SetAdditiveMeterTotalizer(Guid loadArmGuid, Guid productGuid, double value)
		{
			Monitor.Enter(this);
			try
			{
				LoadArmClass LoadArm = this.GetLoadArm(loadArmGuid);
				if (LoadArm == null)
				{
					throw new Exception("LoadRack|Load Arm Not Found");
				}

				ProductMapClass AdditiveInjector = LoadArm.AdditiveInjectorCollection.Find(x => x.AssignedGuid == productGuid);
				if (AdditiveInjector == null)
				{
					throw new Exception("LoadRack|Additive Injector Not Found");
				}

				ProcessVariableClass InternalPV =
					AdditiveInjector.ProcessVariableCollection[PROCESS_VARIABLE_TYPE.ADDITIVE_METER_FLOW_TOTAL_PV];
				if (InternalPV == null)
				{
					throw new Exception("LoadRack|Additive Injector Not Internal");
				}

				InternalPV.ServerValue = value;
				InternalPV.DateTimeStamp = DateTimeOffset.Now;

				FMChannelHelper.MakeCall<IProcessVariables>(
																	 x =>
																	 x.Modify(this.Security, DATA_TYPE.DYNAMIC, InternalPV)
																);
				if (this.Transaction != null)
				{
					LoadArmManagerClass LoadArmManager = this.GetLoadArmManager(LoadArm);

					foreach (LineItemDO lineItem in this.Transaction.LineItems)
					{
						if (lineItem.ArmNumber == null)
						{
							continue;
						}

						if (lineItem.ArmNumber.Value != LoadArmManager.GetArmNumber(this))
						{
							continue;
						}

						if (lineItem.Status != TransactionStatus.InProgress)
						{
							continue;
						}

						foreach (SubLineItemDO subLineItem in lineItem.SubLineItems)
						{
							if (subLineItem.ProductType != ProductClass.ProductTypeID(ProductType.AdditiveProduct))
							{
								continue;
							}

							if (subLineItem.ProductGuid != AdditiveInjector.AssignedGuid)
							{
								continue;
							}

							if (subLineItem.MeterReading.MeterStart != null && !subLineItem.MeterReading.MeterStart_BadQualityLogged
								 && subLineItem.Quantity != null && !subLineItem.Quantity.BadGrossQualityLogged
								 && !subLineItem.Quantity.BadNetQualityLogged)
							{
								subLineItem.MeterReading.MeterStart = this.SiteManager.Site.LoadByNet
									? value + subLineItem.Quantity.NetInventoryChange
									: value + subLineItem.Quantity.GrossInventoryChange;

								if (subLineItem.MeterReading.MeterStart.Value < 0)
								{
									double RollOver = System.Convert.ToDouble(InternalPV.GetMaximum(InternalPV.ServerUnits, 10));
									subLineItem.MeterReading.MeterStart += RollOver;
								}

								subLineItem.MeterReading.MeterStop = AdditiveInjector.MeterValue;
							}
						}
					}
				}
			}
			finally
			{
				Monitor.Exit(this);
			}
		}

		public void SetTransactionInProgress()
		{
			this.Transaction.Status = TransactionStatus.InProgress;
		}

		public void TransactionDone()
		{
			foreach (LoadArmManagerClass loadArmManager in this.LoadArmManagerCollection)
			{
				if (this != loadArmManager.GetStationManager())
				{
					continue;
				}

				if (this != loadArmManager.GetStationManager())
				{
					continue;
				}

				if (loadArmManager.LoadArmState != LOADARM_STATE.FINISHED
					 && loadArmManager.LoadArmState != LOADARM_STATE.FINISHED_WITH_NO_PRODUCTS_TO_LOAD
					 && loadArmManager.LoadArmState != LOADARM_STATE.SELECT_PROMPT
						  && !(loadArmManager.LoadArmState == LOADARM_STATE.BATCH_COMPLETE_PROMPT && loadArmManager.SuppressLoadFinishedPrompt))
				{
					return;
				}
			}

			this.CompleteTransaction();
			if (this.StationState != StationState.BROKEN_BLEND && this.StationState != StationState.IMPROPER_ADDITIZATION)
			{
				this.ResetStationDevice();
			}

			return;
		}

		public virtual void UpdatePermissives(bool authorized)
		{
		}

		#endregion

		#region Methods

		protected void AddNonDuplicateLoadID(string LoadID)
		{
			foreach (string ExistingLoadID in this.LoadIDList)
			{
				if (ExistingLoadID == LoadID)
				{
					return;
				}
			}

			this.LoadIDList.Add(LoadID);
		}

		protected void AddNonDuplicateOrder(GetTransactionTypeDO getTransactionTypeDO)
		{
			foreach (GetTransactionTypeDO Trans in this.OrderList)
			{
				if (Trans.TransID == getTransactionTypeDO.TransID)
				{
					return;
				}
			}

			this.OrderList.Add(getTransactionTypeDO);
		}

		protected void AddToCompartmentArray(EquipmentClass Equipment)
		{
			if (Equipment.IsMultiCompartment && Equipment.CompartmentCollection != null
				 && Equipment.CompartmentCollection.Count > 0)
			{
				for (int nLoop = 1; nLoop <= Equipment.CompartmentCollection.Count; ++nLoop)
				{
					CompartmentInfo Info = new CompartmentInfo();

					Info.EquipmentID = this.SiteManager.Site.UseCompanyEquipmentIdentifiers ? Equipment.CompanyEquipmentID : Equipment.ID;

					Info.EquipmentGuid = Equipment.IdentityGuid;
					Info.CompartmentNumber = nLoop;
					Info.MaxFill = Equipment.CompartmentCollection[nLoop - 1].SISafeFill.SIValue;

					this.CompartmentList.Add(Info);
				}
			}
		}

		protected int AvailableCompartments(EquipmentClass Equipment)
		{
			int Compartments = 0;

			if (Equipment != null)
			{
				for (int CompartmentNumber = 1; CompartmentNumber <= Equipment.CompartmentCollection.Count; CompartmentNumber++)
				{
					if (!this.CompartmentLoadPending(Equipment.IdentityGuid, CompartmentNumber.ToString()))
					{
						Compartments++;
					}
				}
			}

			return Compartments;
		}

		protected void BuildPlan(bool acknowledged)
		{
			if (this.StationState != StationState.BUILD_PLAN)
			{
				this.BuildPlanLineItemIndex = 0;
			}

			else
			{
				if (acknowledged)
				{
					this.BuildPlanLineItemIndex++;
					this.StationState = StationState.IDLE;
				}
				else
				{
					this.CompleteTransaction();
					this.StationState = StationState.IDLE;
					this.DisplayMessage("[LoadRack|Message Timeout]", null, 0, this.MESSAGE_TIMEOUT);
					this.ConsecutivePrompts = 0;
					return;
				}
			}

			IQualityAssurance qaInterface = this.GetQualityAssuranceInterface();

			for (; this.BuildPlanLineItemIndex < this.Transaction.LineItems.Count; this.BuildPlanLineItemIndex++)
			{
				LineItemDO lineItem = this.Transaction.LineItems[this.BuildPlanLineItemIndex];

				if (lineItem.Status != TransactionStatus.LoadPending)
				{
					continue;
				}

				if (lineItem.ProductGuid == Guid.Empty)
				{
					continue;
				}

				ProductMapClass authorizedProduct = this.GetAuthorizedProduct(lineItem.Product);

				if (authorizedProduct == null)
				{
					continue;
				}

				// Locked Out originates from the Product but is set by CheckProductAvailability
				// and within this BuildPlan.  Skip Locked Out Products to prevent
				// multiple messages.
				if (authorizedProduct.LockedOut)
				{
					continue;
				}

				ProductClass product = this.GetByProductAuthorizedCompanies(this.Security, lineItem.ProductGuid, false);

				AdditiveProfileClass additiveProfile = null;

				if (!this.IsLoadablePreloadLineItem(product, lineItem))
				{
					continue;
				}

				ProductClass componentProduct = new ProductClass();
				bool productAvailable = false;
				bool additiveProfileAvailable = false;
				bool tankCertified = false;
				bool certificateOfAnalysis = false;

				// Update the Additive Profile, this could have been set while preloading
				// but doing it here gets the latest configuration from Ship To Authorized Products 
				if (authorizedProduct.AdditiveProfileGuid != Guid.Empty)
				{
					lineItem.AdditiveProfileID = authorizedProduct.AdditiveProfileID;
					lineItem.AdditiveProfileGuid = authorizedProduct.AdditiveProfileGuid;
					additiveProfile = this.GetAdditiveProfiles(this.Security, authorizedProduct.AdditiveProfileGuid);
				}

				// Build a list of arms that can serve this line item
				if (product.ProductType == ProductType.ComponentProduct)
				{
					foreach (LoadArmManagerClass loadArmManager in this.LoadArmManagerCollection)
					{
						if (this != loadArmManager.GetStationManager())
						{
							continue;
						}

						if (!loadArmManager.IsProductServedByLoadArm(product))
						{
							continue;
						}

						if (!this.IsProductAvailable(product, loadArmManager, lineItem.PresetAmount.Value, this.CurrentTransactionAlias))
						{
							continue;
						}

						productAvailable = true;

						if (!loadArmManager.IsAdditiveProfileServedByLoadArm(additiveProfile))
						{
							continue;
						}

						additiveProfileAvailable = true;

						// Avoid repeated checks for the same product
						if (loadArmManager.IsProductAvailabilityDetermined(this, product))
						{
							tankCertified = true;
							certificateOfAnalysis = true;
						}

						else
						{
							if (qaInterface == null || lineItem.COAWaiver)
							{
								tankCertified = true;
								certificateOfAnalysis = true;
							}

							else
							{
								ProductMapClass component = loadArmManager.GetComponent(product.IdentityGuid);
								TankClass tank = this.SiteManager.GetTank(component, this.Manager);
								if (tank == null)
								{
									continue;
								}

								if (!qaInterface.GetTankCertification(this.Security, tank.IdentityGuid, product.IdentityGuid))
								{
									continue;
								}

								tankCertified = true;

								FailedTestItem[] testItems;
								if (
									!qaInterface.GetCertificateOfAnalysis(
										this.Security,
										tank.IdentityGuid,
										product.IdentityGuid,
										this.Owner.MasterRecordGuid,
										this.BillTo.MasterRecordGuid,
										this.ShipTo.MasterRecordGuid,
										out testItems))
								{
									continue;
								}

								certificateOfAnalysis = true;
							}
						}

						if (productAvailable && additiveProfileAvailable && tankCertified && certificateOfAnalysis)
						{
							// Add the product to the list of items that can satisfy the preload
							loadArmManager.Bay(this).PreLoads.Add(lineItem);

							// Must set the activity times so the load timeouts will work
							this.LastActivityDateTime = DateTimeOffset.Now;
							this.StartDateTime = DateTimeOffset.Now;
						}
					}

					// if product fails on all load arms
					if (!productAvailable)
					{
						this.AddAlarmAndEventLogs(this.Security, this.Station.ProductUnavailableAlarm(product.ID));
						this.LoadRackManager.EventOrAlarmEvent.Set();
					}

					else if (!additiveProfileAvailable)
					{
						this.AddAlarmAndEventLogs(this.Security, this.Station.AdditiveProfileUnavailableAlarm(additiveProfile.ID));
						this.LoadRackManager.EventOrAlarmEvent.Set();
					}

					else if (!tankCertified)
					{
						this.AddAlarmAndEventLogs(this.Security, this.Station.NoTankCertificationAlarm(product.ID));
						this.LoadRackManager.EventOrAlarmEvent.Set();
					}

					else if (!certificateOfAnalysis)
					{
						this.AddAlarmAndEventLogs(this.Security, this.Station.FailedCertificateOfAnalysisAlarm(this.ShipTo.ID, product.ID));
						this.LoadRackManager.EventOrAlarmEvent.Set();
					}
				}
				else if (product.ProductType == ProductType.BlendProduct)
				{
					// Check to see if the product can be loaded as a recipe or splash blend
					productAvailable = true;
					additiveProfileAvailable = true;
					tankCertified = true;
					certificateOfAnalysis = true;

					bool blendComponentsCoa = false;
					bool loadAsRecipe = this.IsProductAvailable(additiveProfile, authorizedProduct);

					if (qaInterface != null)
					{
						blendComponentsCoa = qaInterface.BlendComponentsCOA(this.Security, product.IdentityGuid);
						if (!blendComponentsCoa)
						{
							FailedTestItem[] testItems;
							certificateOfAnalysis = qaInterface.GetCertificateOfAnalysis(
								this.Security,
								Guid.Empty,
								product.IdentityGuid,
								this.Owner.MasterRecordGuid,
								this.BillTo.MasterRecordGuid,
								this.ShipTo.MasterRecordGuid,
								out testItems);
							if (!certificateOfAnalysis)
							{
								this.AddAlarmAndEventLogs(this.Security, this.Station.FailedCertificateOfAnalysisAlarm(this.ShipTo.ID, product.ID));
								this.LoadRackManager.EventOrAlarmEvent.Set();
							}
						}
					}

					foreach (ProductMapClass component in product.ComponentCollection)
					{
						componentProduct.IdentityGuid = component.AssignedGuid;
						componentProduct.ID = component.AssignedID;
						componentProduct.ProductType = ProductType.ComponentProduct;

						// Make sure the component has not already been filled
						if (!this.SplashBlendComponentNeedsFilling(lineItem, componentProduct))
						{
							continue;
						}

						bool componentProductAvailable = false;
						bool componentAdditiveProfileAvailable = additiveProfile == null;
						bool componentTankCertified = false;
						bool componentCertificateOfAnalysis = false;

						// Build a list of arms that can serve the components of this line item
						foreach (LoadArmManagerClass loadArmManager in this.LoadArmManagerCollection)
						{
							if (this != loadArmManager.GetStationManager())
							{
								continue;
							}

							if (loadAsRecipe && !loadArmManager.IsProductServedByLoadArm(product))
							{
								continue;
							}

							if (!loadAsRecipe && !loadArmManager.IsProductServedByLoadArm(componentProduct))
							{
								continue;
							}

							if (
								!this.IsProductAvailable(
									componentProduct,
									loadArmManager,
									lineItem.PresetAmount.Value * component.BlendPercentage / 100,
									this.CurrentTransactionAlias))
							{
								continue;
							}

							componentProductAvailable = true;

							if (!loadArmManager.IsAdditiveProfileServedByLoadArm(additiveProfile))
							{
								continue;
							}

							componentAdditiveProfileAvailable = true;

							// Avoid repeated checks for the same components
							if (loadArmManager.IsComponentAvailabilityDetermined(this, component))
							{
								componentTankCertified = true;
								componentCertificateOfAnalysis = true;
							}
							else
							{
								if (qaInterface == null || lineItem.COAWaiver)
								{
									componentTankCertified = true;
									componentCertificateOfAnalysis = true;
								}

								else
								{
									ProductMapClass loadArmComponent = loadArmManager.GetComponent(componentProduct.IdentityGuid);
									TankClass tank = this.SiteManager.GetTank(loadArmComponent, this.Manager);
									if (tank == null)
									{
										continue;
									}

									if (!qaInterface.GetTankCertification(this.Security, tank.IdentityGuid, componentProduct.IdentityGuid))
									{
										continue;
									}

									componentTankCertified = true;

									if (blendComponentsCoa)
									{
										FailedTestItem[] testItems;
										if (
											!qaInterface.GetCertificateOfAnalysis(
												this.Security,
												tank.IdentityGuid,
												componentProduct.IdentityGuid,
												this.Owner.MasterRecordGuid,
												this.BillTo.MasterRecordGuid,
												this.ShipTo.MasterRecordGuid,
												out testItems))
										{
											continue;
										}
									}
									componentCertificateOfAnalysis = true;
								}
							}

							if (!loadAsRecipe && !loadArmManager.Bay(this).SplashProducts.Contains(component))
							{
								loadArmManager.Bay(this).SplashProducts.Add(component);
							}

							// Add the LineItem to the list of items that can satisfy the preload
							if (!loadArmManager.Bay(this).PreLoads.Contains(lineItem))
							{
								loadArmManager.Bay(this).PreLoads.Add(lineItem);
							}

							// Set the activity times
							this.LastActivityDateTime = DateTimeOffset.Now;
							this.StartDateTime = DateTimeOffset.Now;
						}

						if (!componentProductAvailable || !componentAdditiveProfileAvailable || !componentTankCertified
							 || !componentCertificateOfAnalysis)
						{
							// Remove the Line Item, so that partial loading will not be permitted
							foreach (LoadArmManagerClass loadArmManager in this.LoadArmManagerCollection)
							{
								loadArmManager.Bay(this).PreLoads.Remove(lineItem);
							}

							// if component fails on all load arms
							if (!componentProductAvailable)
							{
								this.AddAlarmAndEventLogs(this.Security, this.Station.ProductUnavailableAlarm(componentProduct.ID));
								this.LoadRackManager.EventOrAlarmEvent.Set();
								productAvailable = false;
							}

							else if (!componentAdditiveProfileAvailable)
							{
								this.AddAlarmAndEventLogs(this.Security, this.Station.AdditiveProfileUnavailableAlarm(additiveProfile.ID));
								this.LoadRackManager.EventOrAlarmEvent.Set();
								additiveProfileAvailable = false;
							}

							else if (!componentTankCertified)
							{
								this.AddAlarmAndEventLogs(this.Security, this.Station.NoTankCertificationAlarm(componentProduct.ID));
								this.LoadRackManager.EventOrAlarmEvent.Set();
								tankCertified = false;
							}

							else if (!componentCertificateOfAnalysis)
							{
								this.AddAlarmAndEventLogs(
									this.Security, this.Station.FailedCertificateOfAnalysisAlarm(this.ShipTo.ID, componentProduct.ID));
								this.LoadRackManager.EventOrAlarmEvent.Set();
								certificateOfAnalysis = false;
							}

							break;
						}
					}
				}

				if (!productAvailable)
				{
					this.StationState = StationState.BUILD_PLAN;
					this.DisplayMessageWithAcknowledge(
						"[LoadRack|Product is not available] : " + GetLoadRackDisplayText(authorizedProduct));

					// Set Locked Out so the same product will not generate multiple messages
					if (authorizedProduct != null)
					{
						authorizedProduct.LockedOut = true;
					}

					return;
				}

				else if (!additiveProfileAvailable)
				{
					this.StationState = StationState.BUILD_PLAN;
					this.DisplayMessageWithAcknowledge("[LoadRack|Additive Profile is unavailable] : " + additiveProfile.ID);

					// Set Locked Out so the same product will not generate multiple messages
					if (authorizedProduct != null)
					{
						authorizedProduct.LockedOut = true;
					}

					return;
				}

				else if (!tankCertified)
				{
					this.StationState = StationState.BUILD_PLAN;
					this.DisplayMessageWithAcknowledge(
						"[LoadRack|Tank is not Certified] : " + GetLoadRackDisplayText(authorizedProduct));

					// Set Locked Out so the same product will not generate multiple messages
					if (authorizedProduct != null)
					{
						authorizedProduct.LockedOut = true;
					}

					return;
				}

				else if (!certificateOfAnalysis)
				{
					this.StationState = StationState.BUILD_PLAN;
					this.DisplayMessageWithAcknowledge(
						"[LoadRack|Failed Certificate of Analysis] : " + GetLoadRackDisplayText(authorizedProduct));

					// Set Locked Out so the same product will not generate multiple messages
					if (authorizedProduct != null)
					{
						authorizedProduct.LockedOut = true;
					}

					return;
				}
			}

			this.SaveTransaction();

			this.StartPreloadBatches();

			this.EvaluateLoadArmStatus();
		}

		protected AdditiveProfileClass GetAdditiveProfiles(SecurityClass securityClass, Guid guid)
		{
			return FMChannelHelper.MakeCall<IAdditiveProfiles, AdditiveProfileClass>(
																	 x =>
																	 x.Get(securityClass, guid)
																);
		}

		protected void BuildRecipeMapForAllLoadArms(bool acknowledged)
		{
			StopWatch timer = new StopWatch(StopWatch.Appnames.LoadRackService, "Build Recipe Map For All Arms : " + this.Station.ID);

			try
			{
				if (this.StationState != StationState.BUILD_RECIPE_MAP)
				{
					this.BuildRecipeMapAuthorizedProductIndex = 0;
					this.SetProductsInStation();
				}

				else
				{
					if (acknowledged)
					{
						this.BuildRecipeMapAuthorizedProductIndex++;
						this.StationState = StationState.AUTHORIZING;
					}
					else
					{
						this.CompleteTransaction();
						this.DisplayMessage("[LoadRack|Message Timeout]", null, 0, this.MESSAGE_TIMEOUT);
						this.ConsecutivePrompts = 0;
						this.StationState = StationState.IDLE;
						return;
					}
				}

				IQualityAssurance qaInterface = this.GetQualityAssuranceInterface();

				for (;
					this.BuildRecipeMapAuthorizedProductIndex < this.ShipTo.AuthorizedProductCollection.Count;
					this.BuildRecipeMapAuthorizedProductIndex++)
				{
					ProductMapClass authorizedProduct =	this.ShipTo.AuthorizedProductCollection[this.BuildRecipeMapAuthorizedProductIndex];

					// LockedOut can be set for the Product or is the result of Allocation Load Denial
					if (authorizedProduct.LockedOut)
					{
						continue;
					}

					bool productServicedByStation = false;
					bool productAvailable = false;
					bool tankCertified = false;
					bool certificateOfAnalysis = false;

					foreach (LoadArmManagerClass loadArmManager in this.LoadArmManagerCollection)
					{
						if (this.Station.EnableDynamicRecipes && this != loadArmManager.GetStationManager())
						{
							// When we're using dynamic recipes, only process arms that currently are assigned to this station manager
							// When we're not using dynamic recipes, we go ahead and process arms swung away from us in case the arm swings back
							continue;
						}
						
						ProductMapClass recipe = loadArmManager.GetRecipe(authorizedProduct.AssignedGuid);
						if (recipe == null)
						{
							continue;
						}

						if (!recipe.EnableRecipe)
						{
							continue;
						}

						// Check if Additives are available
						AdditiveProfileClass additiveProfile = null;
						if (authorizedProduct.AdditiveProfileGuid != Guid.Empty)
						{
							additiveProfile = this.GetAdditiveProfiles(this.Security, authorizedProduct.AdditiveProfileGuid);
							if (!loadArmManager.LoadArm.IsAdditiveProfileAvailable(additiveProfile))
							{
								continue;
							}
						}
						else
						{
							if (!loadArmManager.LoadArm.NoAdditivePermissives.Permitted)
							{
								continue;
							}
						}

						productServicedByStation = true;

						ProductClass product = this.GetByProductAuthorizedCompanies(this.Security, authorizedProduct.AssignedGuid, false);

						EngineeringUnit volumeUnit = (this.CurrentTransactionAlias.VolumeUnits != 0)
																	? this.CurrentTransactionAlias.VolumeUnits
																	: this.SiteManager.Site.VolumeUnits;

						//Use trailer safefill and site load amount whatever smaller to check with the product availibity.
						SIDouble maximum;
						double safeFillMax = 0;

						if (this.TractorOrTanker != null)
						{
							if (this.TractorOrTanker.CompartmentCollection.Count > 1)
							{
								foreach (EquipmentClass compartment in this.TractorOrTanker.CompartmentCollection)
								{
									safeFillMax += compartment.SISafeFill.Value;
								}
							}
							else
							{
								safeFillMax += this.TractorOrTanker.SISafeFill.Value;
							}
						}

						if (this.Trailer1 != null)
						{
							if (this.Trailer1.CompartmentCollection.Count > 1)
							{
								foreach (EquipmentClass compartment in this.Trailer1.CompartmentCollection)
								{
									safeFillMax += compartment.SISafeFill.Value;
								}
							}
							else
							{
								safeFillMax += this.Trailer1.SISafeFill.Value;
							}
						}

						if (this.Trailer2 != null)
						{
							if (this.Trailer2.CompartmentCollection.Count > 1)
							{
								foreach (EquipmentClass compartment in this.Trailer2.CompartmentCollection)
								{
									safeFillMax += compartment.SISafeFill.Value;
								}
							}
							else
							{
								safeFillMax += this.Trailer2.SISafeFill.Value;
							}
						}

						if (this.Trailer3 != null)
						{
							if (this.Trailer3.CompartmentCollection.Count > 1)
							{
								foreach (EquipmentClass compartment in this.Trailer3.CompartmentCollection)
								{
									safeFillMax += compartment.SISafeFill.Value;
								}
							}
							else
							{
								safeFillMax += this.Trailer3.SISafeFill.Value;
							}
						}

						SIDouble safeFill = new SIDouble { Units = volumeUnit, Value = safeFillMax };
						SIDouble maximumSiteLoadAmount = new SIDouble { Units = volumeUnit, SIValue = this.SiteManager.Site._MaximumLoadAmount.SIValue };

						if (safeFill.Value > maximumSiteLoadAmount.Value)
						{
							maximum = maximumSiteLoadAmount;
						}
						else
						{
							maximum = safeFill;
						}

						if (!this.IsProductAvailable(product, loadArmManager, maximum.Value, this.CurrentTransactionAlias))
						{
							continue;
						}

						productAvailable = true;

						if (qaInterface == null)
						{
							tankCertified = true;
							certificateOfAnalysis = true;
						}
						else
						{
							if (product.ProductType == ProductType.ComponentProduct)
							{
								ProductMapClass component = loadArmManager.GetComponent(product.IdentityGuid);
								TankClass tank = this.SiteManager.GetTank(component, this.Manager);
								if (tank == null)
								{
									continue;
								}

								if (!qaInterface.GetTankCertification(this.Security, tank.IdentityGuid, product.IdentityGuid))
								{
									continue;
								}

								tankCertified = true;

								FailedTestItem[] testItems;
								if (
									!qaInterface.GetCertificateOfAnalysis(
										this.Security,
										tank.IdentityGuid,
										product.IdentityGuid,
										this.Owner.MasterRecordGuid,
										this.BillTo.MasterRecordGuid,
										this.ShipTo.MasterRecordGuid,
										out testItems))
								{
									continue;
								}

								certificateOfAnalysis = true;
							}
							else
							{
								bool componentTankCertified = true;
								bool componentCertificateOfAnalysis = true;

								foreach (ProductMapClass blendComponent in product.ComponentCollection)
								{
									ProductMapClass component = loadArmManager.GetComponent(blendComponent.AssignedGuid);
									TankClass tank = this.SiteManager.GetTank(component, this.Manager);
									if (tank == null)
									{
										break;
									}

									if (!qaInterface.GetTankCertification(this.Security, tank.IdentityGuid, blendComponent.AssignedGuid))
									{
										componentTankCertified = false;
										break;
									}

									FailedTestItem[] testItems;
									if (
										!qaInterface.GetCertificateOfAnalysis(
											this.Security,
											tank.IdentityGuid,
											blendComponent.AssignedGuid,
											this.Owner.MasterRecordGuid,
											this.BillTo.MasterRecordGuid,
											this.ShipTo.MasterRecordGuid,
											out testItems))
									{
										componentCertificateOfAnalysis = false;
										break;
									}
								}

								if (componentTankCertified)
								{
									tankCertified = true;
								}

								if (componentCertificateOfAnalysis)
								{
									certificateOfAnalysis = true;
								}

								if (!componentTankCertified || !componentCertificateOfAnalysis)
								{
									continue;
								}
							}
						}

						int downloadedRecipe;
						downloadedRecipe = this.WriteSingleRecipe(loadArmManager, recipe);
						if (downloadedRecipe == 0)
						{
								this.AddAlarmAndEventLogs(this.Security, this.Station.DynamicRecipeDownloadErrorAlarm(this.Station.ID));
								WriteLogDataToCommFile($"Unable to write recipe {recipe.AssignedID} to preset", CommLogDirection.None);
								if (this == loadArmManager.GetStationManager())
								{
									loadArmManager.LogOutOfProgramMode();
								}
								continue;
						}

						loadArmManager.Bay(this).RecipeMap |= (ulong)0x1 << (downloadedRecipe - 1);
						
						try
						{
							this.RecipeInternalNumberMap.Add(downloadedRecipe, recipe);
						}
						catch (ArgumentException ae)
						{
							_ = ae;
							this.DisplayMessageWithAcknowledge("LoadRack|Recipe Write Error");
							WriteLogDataToCommFile("Error writing recipe to preset: duplicate internal recipe number", CommLogDirection.None);
							
							this.eventLog.WriteEntry($"Error writing recipe {recipe.ID} to preset# {downloadedRecipe}, duplicate internal recipe number", EventLogEntryType.Error);
							return;
						}

                        if (this == loadArmManager.GetStationManager())
						{
							string name = GetLoadRackDisplayText(authorizedProduct);

							if (!loadArmManager.UpdateRecipe(name, recipe, product, additiveProfile, downloadedRecipe))
							{
								if (!loadArmManager.LogOutOfProgramMode())
								{
									return;
								}

								this.StationState = StationState.UPDATE_RECIPE_ERROR_MSG;
								this.DisplayMessageWithAcknowledge("LoadRack|Update Recipe Error");
								return;
							}
						}
					}

					if (productServicedByStation)
					{
						if (!productAvailable)
						{
							this.AddAlarmAndEventLogs(this.Security, this.Station.ProductUnavailableAlarm(authorizedProduct.AssignedID));
							this.StationState = StationState.BUILD_RECIPE_MAP;

							foreach (LoadArmManagerClass loadArmManager in this.LoadArmManagerCollection)
							{
								if (this != loadArmManager.GetStationManager())
								{
									continue;
								}

								if (!loadArmManager.LogOutOfProgramMode())
								{
									return;
								}
							}

							this.DisplayMessageWithAcknowledge(
								"[LoadRack|Product is not available] : " + GetLoadRackDisplayText(authorizedProduct));
							return;
						}

						if (!tankCertified)
						{
							this.AddAlarmAndEventLogs(this.Security, this.Station.NoTankCertificationAlarm(authorizedProduct.AssignedID));
							this.StationState = StationState.BUILD_RECIPE_MAP;

							foreach (LoadArmManagerClass loadArmManager in this.LoadArmManagerCollection)
							{
								if (this != loadArmManager.GetStationManager())
								{
									continue;
								}

								if (!loadArmManager.LogOutOfProgramMode())
								{
									return;
								}
							}

							this.DisplayMessageWithAcknowledge(
								"[LoadRack|Tank is not Certified] : " + GetLoadRackDisplayText(authorizedProduct));
							return;
						}

						if (!certificateOfAnalysis)
						{
							this.AddAlarmAndEventLogs(
								this.Security, this.Station.FailedCertificateOfAnalysisAlarm(this.ShipTo.ID, authorizedProduct.AssignedID));
							this.StationState = StationState.BUILD_RECIPE_MAP;

							foreach (LoadArmManagerClass loadArmManager in this.LoadArmManagerCollection)
							{
								if (this != loadArmManager.GetStationManager())
								{
									continue;
								}

								if (!loadArmManager.LogOutOfProgramMode())
								{
									return;
								}
							}

							this.DisplayMessageWithAcknowledge(
								"[LoadRack|Failed Certificate of Analysis] : " + GetLoadRackDisplayText(authorizedProduct));
							return;
						}
                    }
                }

				// Set Reference Density and Load Out of Program mode
				bool firstArm = true;
				foreach (LoadArmManagerClass loadArmManager in this.LoadArmManagerCollection)
				{
					if (this != loadArmManager.GetStationManager())
					{
						continue;
					}

					if (this.Station.SynchronizeReferenceDensity && loadArmManager.Bay(this).RecipeMap != 0
						 && !loadArmManager.UpdateReferenceDensity(this))
					{
						if (!loadArmManager.LogOutOfProgramMode())
						{
							return;
						}

						loadArmManager.DisplayMessage("LoadRack|Update Density Error", 0, this.MESSAGE_TIMEOUT);

						// Need call CompleteTransaction because of PIDX; if a transaction hasn't been created then
						// it will only set station status back to IDLE, allowing a return to Enter Driver ID/Please Card In
						this.CompleteTransaction();
						return;
					}

					// TODO: Find out if we should do this.
					if (firstArm)
					{
						if (!loadArmManager.UpdateMaximumPreset(this))
						{
							if (!loadArmManager.LogOutOfProgramMode())
							{
								return;
							}

							loadArmManager.DisplayMessage("LoadRack|Update Maximum Preset Error", 0, this.MESSAGE_TIMEOUT);

							// Need call CompleteTransaction because of PIDX; if a transaction hasn't been created then
							// it will only set station status back to IDLE, allowing a return to Enter Driver ID/Please Card In
							this.CompleteTransaction();
							return;
						}
					}

					firstArm = false;
				}

				LoadArmManagerClass firstLoadArmManager = this.LoadArmManagerCollection.Item(0);
				if (!firstLoadArmManager.LogOutOfProgramMode())
				{
					return;
				}

				bool anyArmAuthorized = false;
				foreach (LoadArmManagerClass loadArmManager in this.LoadArmManagerCollection)
				{
					if (this != loadArmManager.GetStationManager())
					{
						continue;
					}

					if (loadArmManager.EnablePreset(this, true))
					{
						// Focus to first arm authorized
						if (anyArmAuthorized == false && !this.HasSwingArms && !loadArmManager.IsInAlarm)
						{
							loadArmManager.SetFocus();
						}

						anyArmAuthorized = true;
					}
				}

				if (anyArmAuthorized)
				{
					if (this.Transaction == null)
					{
						this.InitializeTransaction();
						this.Transaction.DeleteFlag = true;
					}
					else
					{
						if (this.TractorOrTanker != null
						&& !this.CheckEquipmentInTransaction(this.TractorOrTanker))
						{
							this.AddEquipmentToTransaction(this.TractorOrTanker);
						}

						if (this.Trailer1 != null
						&& !this.CheckEquipmentInTransaction(this.Trailer1))
						{
							this.AddEquipmentToTransaction(this.Trailer1);
						}

						if (this.Trailer2 != null
						&& !this.CheckEquipmentInTransaction(this.Trailer2))
						{
							this.AddEquipmentToTransaction(this.Trailer2);
						}

						if (this.Trailer3 != null
						&& !this.CheckEquipmentInTransaction(this.Trailer3))
						{
							this.AddEquipmentToTransaction(this.Trailer3);
						}
					}


					this.StartDateTime = DateTimeOffset.Now;
					this.StationState = StationState.AUTHORIZED;
					this.LastActivityDateTime = DateTimeOffset.Now;

					if (this.Transaction.RouteSchedule.FST == null)
					{
						this.Transaction.RouteSchedule.FST = TimeConverter.Now(this.SiteManager.Site);
					}
				}

				else
				{
					if (this.Transaction != null)
					{
						this.Transaction.Status = TransactionStatus.Cancelled;
						this.SaveTransaction();
						this.Transaction = null;
					}

					this.EndTransaction();
					this.LoadArmManagerCollection.SetState(this, LOADARM_STATE.NORMAL);
					Thread.Sleep(this.MESSAGE_TIMEOUT * 1000);
					this.DisplayMessage("[LoadRack|No Arms Authorized]", null, 0, this.MESSAGE_TIMEOUT);
					this.StationState = StationState.RESET_ON_TIMEOUT;
				}
			}
			finally
			{
				timer.Stop();
			}
		}

		protected virtual void BuildOffloadRecipeMapForAllLoadArms(bool acknowledged)
		{
			StopWatch timer = new StopWatch(StopWatch.Appnames.LoadRackService, "Build Offload Recipe Map For All Arms : " + this.Station.ID);

			try
			{
				if (this.StationState != StationState.BUILD_RECIPE_MAP)
				{
					this.BuildRecipeMapAuthorizedProductIndex = 0;
				}

				else
				{
					if (acknowledged)
					{
						this.BuildRecipeMapAuthorizedProductIndex++;
						this.StationState = StationState.AUTHORIZING;
					}
					else
					{
						this.CompleteTransaction();
						this.DisplayMessage("[LoadRack|Message Timeout]", null, 0, this.MESSAGE_TIMEOUT);
						this.ConsecutivePrompts = 0;
						this.StationState = StationState.IDLE;
						return;
					}
				}

				// TODO: Is quality assurance appropriate for offloads?
				IQualityAssurance qaInterface = this.GetQualityAssuranceInterface();

				for (;
					 this.BuildRecipeMapAuthorizedProductIndex < this.Supplier.SupplierAuthorizedProductCollection.Count;
					 this.BuildRecipeMapAuthorizedProductIndex++)
				{
					ProductMapClass authorizedProduct =
						 this.Supplier.SupplierAuthorizedProductCollection[this.BuildRecipeMapAuthorizedProductIndex];

					// LockedOut can be set for the Product or is the result of Allocation Load Denial
					if (authorizedProduct.LockedOut)
					{
						continue;
					}

					bool productServicedByStation = false;
					bool productAvailable = false;
					bool tankCertified = false;
					bool certificateOfAnalysis = false;

					foreach (LoadArmManagerClass loadArmManager in this.LoadArmManagerCollection)
					{
						ProductMapClass recipe = loadArmManager.GetRecipe(authorizedProduct.AssignedGuid);
						if (recipe == null)
						{
							continue;
						}

						if (!recipe.EnableRecipe)
						{
							continue;
						}

						// TODO: Verify that additives do not apply to received product
						//// Check if Additives are available
						AdditiveProfileClass additiveProfile = null;
						//if (authorizedProduct.AdditiveProfileGuid != Guid.Empty)
						//{
						//    additiveProfile = this.GetAdditiveProfiles(this.Security, authorizedProduct.AdditiveProfileGuid);
						//    if (!loadArmManager.LoadArm.IsAdditiveProfileAvailable(additiveProfile))
						//    {
						//        continue;
						//    }
						//}
						//else
						//{
						//    if (!loadArmManager.LoadArm.NoAdditivePermissives.Permitted)
						//    {
						//        continue;
						//    }
						//}

						productServicedByStation = true;

						ProductClass product = this.GetByProductAuthorizedCompanies(
							 this.Security, authorizedProduct.AssignedGuid, false);

						EngineeringUnit volumeUnit = (this.CurrentTransactionAlias.VolumeUnits != 0)
																				  ? this.CurrentTransactionAlias.VolumeUnits
																				  : this.SiteManager.Site.VolumeUnits;

						//Use trailer safefill and site load amount whatever smaller to check with the product availibity.
						SIDouble maximum = new SIDouble { Units = volumeUnit };
						double safeFillMax = 0;

						if (this.TractorOrTanker != null)
						{
							if (this.TractorOrTanker.CompartmentCollection.Count > 1)
							{
								foreach (EquipmentClass compartment in this.TractorOrTanker.CompartmentCollection)
								{
									safeFillMax += compartment.SISafeFill.Value;
								}
							}
							else
							{
								safeFillMax += this.TractorOrTanker.SISafeFill.Value;
							}
						}

						if (this.Trailer1 != null)
						{
							if (this.Trailer1.CompartmentCollection.Count > 1)
							{
								foreach (EquipmentClass compartment in this.Trailer1.CompartmentCollection)
								{
									safeFillMax += compartment.SISafeFill.Value;
								}
							}
							else
							{
								safeFillMax += this.Trailer1.SISafeFill.Value;
							}
						}

						if (this.Trailer2 != null)
						{
							if (this.Trailer2.CompartmentCollection.Count > 1)
							{
								foreach (EquipmentClass compartment in this.Trailer2.CompartmentCollection)
								{
									safeFillMax += compartment.SISafeFill.Value;
								}
							}
							else
							{
								safeFillMax += this.Trailer2.SISafeFill.Value;
							}
						}

						if (this.Trailer3 != null)
						{
							if (this.Trailer3.CompartmentCollection.Count > 1)
							{
								foreach (EquipmentClass compartment in this.Trailer3.CompartmentCollection)
								{
									safeFillMax += compartment.SISafeFill.Value;
								}
							}
							else
							{
								safeFillMax += this.Trailer3.SISafeFill.Value;
							}
						}

						SIDouble safeFill = new SIDouble { Units = volumeUnit, SIValue = safeFillMax };
						SIDouble maximumSiteLoadAmount = new SIDouble { Units = volumeUnit, SIValue = this.SiteManager.Site._MaximumLoadAmount.SIValue };

						maximum = safeFill.Value > maximumSiteLoadAmount.Value ? maximumSiteLoadAmount : safeFill;

                        if (!this.IsProductAvailable(product, loadArmManager, maximum.Value, this.CurrentTransactionAlias))
						{
							continue;
						}

						productAvailable = true;

						if (qaInterface == null)
						{
							tankCertified = true;
							certificateOfAnalysis = true;
						}
						else
						{
							if (product.ProductType == ProductType.ComponentProduct)
							{
								ProductMapClass component = loadArmManager.GetComponent(product.IdentityGuid);
								TankClass tank = this.SiteManager.GetTank(component, this.Manager);
								if (tank == null)
								{
									continue;
								}

								if (!qaInterface.GetTankCertification(this.Security, tank.IdentityGuid, product.IdentityGuid))
								{
									continue;
								}

								tankCertified = true;

                                if (
                                     !qaInterface.GetCertificateOfAnalysis(
                                          this.Security,
                                          tank.IdentityGuid,
                                          product.IdentityGuid,
                                          this.Owner.MasterRecordGuid,
                                          this.BillTo.MasterRecordGuid,
                                          this.ShipTo.MasterRecordGuid,
                                          out FailedTestItem[] testItems))
                                {
                                    continue;
                                }

                                certificateOfAnalysis = true;
							}
							else
							{
								bool componentTankCertified = true;
								bool componentCertificateOfAnalysis = true;

								foreach (ProductMapClass blendComponent in product.ComponentCollection)
								{
									ProductMapClass component = loadArmManager.GetComponent(blendComponent.AssignedGuid);
									TankClass tank = this.SiteManager.GetTank(component, this.Manager);
									if (tank == null)
									{
										break;
									}

									if (!qaInterface.GetTankCertification(this.Security, tank.IdentityGuid, blendComponent.AssignedGuid))
									{
										componentTankCertified = false;
										break;
									}

                                    if (
                                         !qaInterface.GetCertificateOfAnalysis(
                                              this.Security,
                                              tank.IdentityGuid,
                                              blendComponent.AssignedGuid,
                                              this.Owner.MasterRecordGuid,
                                              this.BillTo.MasterRecordGuid,
                                              this.ShipTo.MasterRecordGuid,
                                              out FailedTestItem[] testItems))
                                    {
                                        componentCertificateOfAnalysis = false;
                                        break;
                                    }
                                }

								if (componentTankCertified)
								{
									tankCertified = true;
								}

								if (componentCertificateOfAnalysis)
								{
									certificateOfAnalysis = true;
								}

								if (!componentTankCertified || !componentCertificateOfAnalysis)
								{
									continue;
								}
							}
						}

                  int downloadedRecipe;
                  downloadedRecipe = this.WriteSingleRecipe(loadArmManager, recipe);
                  if (downloadedRecipe == 0)
                  {
                        WriteLogDataToCommFile($"Unable to write recipe {recipe.AssignedID} to preset", CommLogDirection.None);
								this.AddAlarmAndEventLogs(this.Security, this.Station.DynamicRecipeDownloadErrorAlarm(this.Station.ID));
							if (this == loadArmManager.GetStationManager())
							{
								loadArmManager.LogOutOfProgramMode();
							}
							continue;
						}

						loadArmManager.Bay(this).RecipeMap |= (ulong)0x1 << (downloadedRecipe - 1);
                        
								try
                        {
                            this.RecipeInternalNumberMap.Add(downloadedRecipe, recipe);
                        }
                        catch (ArgumentException ae)
                        {
                            _ = ae;
                            this.DisplayMessageWithAcknowledge("LoadRack|Recipe Write Error");
                            WriteLogDataToCommFile("Error writing recipe to preset: duplicate internal recipe number", CommLogDirection.None);
									
							this.eventLog.WriteEntry($"Error writing recipe {recipe.ID} to preset# {downloadedRecipe}, duplicate internal recipe number", EventLogEntryType.Error);
							return;
                  }

                  if (this == loadArmManager.GetStationManager())
						{
							string name = GetLoadRackDisplayText(authorizedProduct);

							if (!loadArmManager.UpdateRecipe(name, recipe, product, additiveProfile, downloadedRecipe))
							{
								if (!loadArmManager.LogOutOfProgramMode())
								{
									return;
								}

								this.StationState = StationState.UPDATE_RECIPE_ERROR_MSG;
								this.DisplayMessageWithAcknowledge("LoadRack|Update Recipe Error");
								return;
							}
						}
					}

					if (productServicedByStation)
					{
						if (!productAvailable)
						{
							this.AddAlarmAndEventLogs(this.Security, this.Station.ProductUnavailableAlarm(authorizedProduct.AssignedID));
							this.StationState = StationState.BUILD_RECIPE_MAP;

							foreach (LoadArmManagerClass loadArmManager in this.LoadArmManagerCollection)
							{
								if (this != loadArmManager.GetStationManager())
								{
									continue;
								}

								if (!loadArmManager.LogOutOfProgramMode())
								{
									return;
								}
							}

							this.DisplayMessageWithAcknowledge(
								 "[LoadRack|Product is not available] : " + GetLoadRackDisplayText(authorizedProduct));
							return;
						}

						if (!tankCertified)
						{
							this.AddAlarmAndEventLogs(this.Security, this.Station.NoTankCertificationAlarm(authorizedProduct.AssignedID));
							this.StationState = StationState.BUILD_RECIPE_MAP;

							foreach (LoadArmManagerClass loadArmManager in this.LoadArmManagerCollection)
							{
								if (this != loadArmManager.GetStationManager())
								{
									continue;
								}

								if (!loadArmManager.LogOutOfProgramMode())
								{
									return;
								}
							}

							this.DisplayMessageWithAcknowledge(
								 "[LoadRack|Tank is not Certified] : " + GetLoadRackDisplayText(authorizedProduct));
							return;
						}

						if (!certificateOfAnalysis)
						{
							this.AddAlarmAndEventLogs(
								 this.Security, this.Station.FailedCertificateOfAnalysisAlarm(this.ShipTo.ID, authorizedProduct.AssignedID));
							this.StationState = StationState.BUILD_RECIPE_MAP;

							foreach (LoadArmManagerClass loadArmManager in this.LoadArmManagerCollection)
							{
								if (this != loadArmManager.GetStationManager())
								{
									continue;
								}

								if (!loadArmManager.LogOutOfProgramMode())
								{
									return;
								}
							}

							this.DisplayMessageWithAcknowledge(
								 "[LoadRack|Failed Certificate of Analysis] : " + GetLoadRackDisplayText(authorizedProduct));
							return;
						}
					}
				}

				// Set Reference Density and Load Out of Program mode
				foreach (LoadArmManagerClass loadArmManager in this.LoadArmManagerCollection)
				{
					if (this != loadArmManager.GetStationManager())
					{
						continue;
					}

					if (this.Station.SynchronizeReferenceDensity && loadArmManager.Bay(this).RecipeMap != 0
						  && !loadArmManager.UpdateReferenceDensity(this))
					{
						if (!loadArmManager.LogOutOfProgramMode())
						{
							return;
						}

						loadArmManager.DisplayMessage("LoadRack|Update Density Error", 0, this.MESSAGE_TIMEOUT);
						return;
					}

					// TODO: Find out if we should do this.
					if (!loadArmManager.UpdateMaximumPreset(this))
					{
						if (!loadArmManager.LogOutOfProgramMode())
						{
							return;
						}

						loadArmManager.DisplayMessage("LoadRack|Update Maximum Preset Error", 0, this.MESSAGE_TIMEOUT);
						return;
					}

					if (!loadArmManager.LogOutOfProgramMode())
					{
						return;
					}
				}

				bool anyArmAuthorized = false;
				foreach (LoadArmManagerClass loadArmManager in this.LoadArmManagerCollection)
				{
					if (this != loadArmManager.GetStationManager())
					{
						continue;
					}

					if (loadArmManager.EnablePreset(this, true))
					{
						// Focus to first arm authorized
						if (anyArmAuthorized == false && !this.HasSwingArms && !loadArmManager.IsInAlarm)
						{
							loadArmManager.SetFocus();
						}

						anyArmAuthorized = true;
					}
				}

				if (anyArmAuthorized)
				{
					if (this.Transaction == null)
					{
						this.InitializeTransaction();
						this.Transaction.DeleteFlag = true;
					}
					else
					{
						if (this.TractorOrTanker != null
						&& !this.CheckEquipmentInTransaction(this.TractorOrTanker))
						{
							this.AddEquipmentToTransaction(this.TractorOrTanker);
						}

						if (this.Trailer1 != null
						&& !this.CheckEquipmentInTransaction(this.Trailer1))
						{
							this.AddEquipmentToTransaction(this.Trailer1);
						}

						if (this.Trailer2 != null
						&& !this.CheckEquipmentInTransaction(this.Trailer2))
						{
							this.AddEquipmentToTransaction(this.Trailer2);
						}

						if (this.Trailer3 != null
						&& !this.CheckEquipmentInTransaction(this.Trailer3))
						{
							this.AddEquipmentToTransaction(this.Trailer3);
						}
					}


					this.StartDateTime = DateTimeOffset.Now;
					this.StationState = StationState.AUTHORIZED;
					this.LastActivityDateTime = DateTimeOffset.Now;

					if (this.Transaction.RouteSchedule.FST == null)
					{
						this.Transaction.RouteSchedule.FST = TimeConverter.Now(this.SiteManager.Site);
					}
				}

				else
				{
					if (this.Transaction != null)
					{
						this.Transaction.Status = TransactionStatus.Cancelled;
						this.SaveTransaction();
						this.Transaction = null;
					}

					this.EndTransaction();
					this.LoadArmManagerCollection.SetState(this, LOADARM_STATE.NORMAL);
					Thread.Sleep(this.MESSAGE_TIMEOUT * 1000);
					this.DisplayMessage("[LoadRack|No Arms Authorized]", null, 0, this.MESSAGE_TIMEOUT);
					this.StationState = StationState.RESET_ON_TIMEOUT;
				}
			}
			finally
			{
				timer.Stop();
			}
		}

		protected void AddAlarmAndEventLogs(SecurityClass securityClass, AlarmAndEventLogClass alarmAndEventLogClass)
		{
			FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
																	 x =>
																	 x.Add(securityClass, alarmAndEventLogClass)
																);
		}

		protected void CalculateWeighOutVolume(TransactionDO Transaction)
		{
			try
			{
				// Check to make sure the transaction alias we are working with matches the load by
				// weight alias of the station.  If not, we have nothing to do here.
				if (this.Station.IssueByWeightTransactionAliasID != this.Transaction.Alias)
				{
					return;
				}

				// Make sure we have a weight reading
				if (Transaction.WeightReadings.Count == 0)
				{
					return;
				}

				// Make sure the weight reading has a beginning quantity and a final quantity
				WeightReadingDO WeightReading = Transaction.WeightReadings[0];

				if (WeightReading == null || WeightReading.BeginQuantity == null || WeightReading.FinalQuantity == null)
				{
					return;
				}

				// Make sure we have 1 line item to work with
				if (Transaction.LineItems.Count != 1)
				{
					return;
				}

				LineItemDO lineItem = Transaction.LineItems[0];

				if (lineItem.ProductGuid == Guid.Empty)
				{
					return;
				}

				ProductClass Product = FMChannelHelper.MakeCall<IProducts, ProductClass>(
																	 x =>
																	 x.GetByProductAuthorizedCompanies(this.Security, lineItem.ProductGuid, false)
																);
				if (!Product.LoadByWeight)
				{
					return;
				}

				this.CurrentTransactionAlias = FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasClass>(
																	 x =>
																	 x.Get(this.Security, Transaction.TransactionAliasGuid, false)
																);
				EngineeringUnit massUnits = (this.CurrentTransactionAlias.MassUnits != 0)
														  ? this.CurrentTransactionAlias.MassUnits
														  : this.SiteManager.Site.MassUnits;
				EngineeringUnit densityUnits = (this.CurrentTransactionAlias.DensityUnits != 0)
															  ? this.CurrentTransactionAlias.DensityUnits
															  : this.SiteManager.Site.DensityUnits;
				EngineeringUnit temperatureUnits = (this.CurrentTransactionAlias.TemperatureUnits != 0)
																	? this.CurrentTransactionAlias.TemperatureUnits
																	: this.SiteManager.Site.TemperatureUnits;

				// Compute the delta weight for the first weight reading in the list.  This is the weight entry
				// we record weights at for station processing.
				SIDouble deltaWeight = new SIDouble
				{
					Units = massUnits,
					Value = WeightReading.FinalQuantity.Value - WeightReading.BeginQuantity.Value
				};

				TankClass Tank = null;

				// If the Tank cannot be determined the volume will be calculated at Standard Density & Temperature
				double TankDensity = Product._StandardDensity.SIValue;
				double TankTemperature = Product._VcfModuleSettings.BaseTemperature.Value;// _StandardTemperature.SIValue;
				double TankVCF = 1.0;
				bool bTankFound = false;

				if (lineItem.StorageLocationTankGuid != Guid.Empty)
				{
					Tank = this.SiteManager.GetTank(lineItem.StorageLocationTankGuid);
				}
				else
				{
					foreach (StationManagerClass StationManager in this.SiteManager.StationManagerCollection)
					{
						if (!StationManager.Station.Enabled)
						{
							continue;
						}

						if (StationManager.Station.Type != STATION_TYPE.LOAD_RACK)
						{
							continue;
						}

						foreach (LoadArmManagerClass LoadArmManager in StationManager.LoadArmManagerCollection)
						{
							if (StationManager != LoadArmManager.GetStationManager())
							{
								continue;
							}

							if (!LoadArmManager.IsProductServedByLoadArm(Product) && !LoadArmManager.IsProductServedByLoadArm(Product))
							{
								continue;
							}

							ProductMapClass LoadArmComponent = LoadArmManager.GetComponent(Product.IdentityGuid);
							if (LoadArmComponent == null)
							{
								continue;
							}

							Tank = this.SiteManager.GetTank(LoadArmComponent, this.Manager);
							if (Tank != null)
							{
								bTankFound = true;
								break;
							}
						}
						if (bTankFound)
						{
							break;
						}
					}
				}

				// Find the tank used for the transaction
				if (Tank != null)
				{
					// Get tank values for use and storage
					try
					{
						TankDensity = this.GetTankValue(Tank, PROCESS_VARIABLE_TYPE.STANDARD_DENSITY_PV);
						TankTemperature = this.GetTankValue(Tank, PROCESS_VARIABLE_TYPE.TEMPERATURE_PV);
						TankVCF = this.GetTankValue(Tank, PROCESS_VARIABLE_TYPE.VCF_PV);
					}
					catch (Exception e)
					{
						this.eventLog.WriteEntry("StationManager CalculateWeighOutVolume : " + e.Message, EventLogEntryType.Error);
					}
				}

				// Generate Invalid Storage Location Event
				else
				{
					FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
																	 x =>
																	 x.Add(this.Security, this.Station.InvalidStorageLocationEvent(Transaction.TransID))
																);
				}

				// Get the density
				SIDouble density = new SIDouble
				{
					Units = this.SiteManager.Site.DensityUnits,
					SIValue = TankDensity
				};

				// Get the temperature
				SIDouble temperature = new SIDouble
				{
					Units = this.SiteManager.Site.TemperatureUnits,
					SIValue = TankTemperature
				};

				// Calculate the net volume
				SIDouble netVolume = new SIDouble
				{
					Units = lineItem.VolumeUnits
				};

				if (density.SIValue != 0)
				{
					netVolume.SIValue = deltaWeight.SIValue / density.SIValue;
				}

				// Caluclate the gross volume
				double grossVolume = 0;
				if (TankVCF != 0)
				{
					grossVolume = netVolume.Value / TankVCF;
				}

				// Save final values
				double finalGrossVolume = grossVolume;
				double finalNetVolume = netVolume.Value;

				// Make sure they are negative if necessary for saving in transaction line item table
				if (finalGrossVolume > 0 && finalNetVolume > 0)
				{
					finalGrossVolume = -finalGrossVolume;
					finalNetVolume = -finalNetVolume;
				}

				// Otherwise, store the value
				lineItem.Quantity = new QuantityDO
				{
					GrossInventoryChange = finalGrossVolume,
					NetInventoryChange = finalNetVolume
				};

				density.Units = densityUnits;
				temperature.Units = temperatureUnits;

				lineItem.Density = density.Value;
				lineItem.VCF = TankVCF;
				lineItem.Temperature = temperature.Value;

				lineItem.Status = TransactionStatus.Completed;
				foreach (SubLineItemDO SubLineItem in lineItem.SubLineItems)
				{
					SubLineItem.Status = TransactionStatus.Completed;
				}
			}
			catch (Exception e)
			{
				this.eventLog.WriteEntry("StationManager CalculateWeighOutVolume : " + e.Message, EventLogEntryType.Error);
			}
		}

		protected void CardIn()
		{
			// if the driver is not currently carded in add the event log
			if (this.Driver.CardedIn == false)
			{
				FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
																	 x =>
																	 x.Add(this.Security, this.Driver.CardInEvent(this.Station.ID))
																);
				this.Driver.CardedIn = true;
			}
			this.Driver._LastActivityDate.Value = TimeConverter.Now(this.SiteManager.Site);
			FMChannelHelper.MakeCall<IPersonnel>(
																	 x =>
																	 x.Modify(this.Security, DATA_TYPE.DYNAMIC, this.Driver)
																);
		}

		protected void CardOut()
		{
			if (this.Driver.CardedIn)
			{
				this.Driver.CardedIn = false;
				FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
																	 x =>
																	 x.Add(this.Security, this.Driver.CardOutEvent(this.Station.ID))
																);
			}

			// Release Any House Card
			Guid houseCardGuid = FMChannelHelper.MakeCall<IHouseCards, Guid>(
																	 x =>
																	 x.GetIdentityGuidByDriverGuid(this.Security, this.Driver.MasterRecordGuid)
																);
			if (!houseCardGuid.IsEmpty())
			{
				HouseCardClass HouseCard = FMChannelHelper.MakeCall<IHouseCards, HouseCardClass>(
																	 x =>
																	 x.Get(this.Security, houseCardGuid)
																);
				HouseCard.DriverGuid = Guid.Empty;
				FMChannelHelper.MakeCall<IHouseCards>(
																	 x =>
																	 x.Modify(this.Security, HouseCard)
																);
			}

			this.Driver._LastActivityDate.Value = TimeConverter.Now(this.SiteManager.Site);
			FMChannelHelper.MakeCall<IPersonnel>(
																	 x =>
																	 x.Modify(this.Security, DATA_TYPE.DYNAMIC, this.Driver)
																);
		}

		protected bool CheckAndSetSingleOwnerManager()
		{
			if (this.Station.InterfaceType != STATION_INTERFACE_TYPE.CONTREC1010)
			{
				return true;
			}

			CompanyMapCollectionClass OwnerManagerMapCollection = FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapCollectionClass>(
																	 x =>
																	 x.EnumerateByType(this.Security, COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP)
																);
			if (OwnerManagerMapCollection.Count == 1)
			{
				foreach (CompanyMapClass OwnerManagerMap in OwnerManagerMapCollection)
				{
					this.Owner = this.GetCompany(this.Security, OwnerManagerMap.AssignedGuid);
					this.Owner._LastActivityDate.Value = DateTimeOffset.Now;
					this.ModifyCompany(this.Security, DATA_TYPE.DYNAMIC, this.Owner);

					this.Manager = this.GetCompany(this.Security, OwnerManagerMap.AssignedToGuid);
					this.Manager._LastActivityDate.Value = DateTimeOffset.Now;
					this.ModifyCompany(this.Security, DATA_TYPE.DYNAMIC, this.Manager);
				}
				return true;
			}

			return false;
		}

		private void ModifyCompany(SecurityClass securityClass, DATA_TYPE dataType, CompanyClass companyClass)
		{
			FMChannelHelper.MakeCall<ICompanies>(
																	 x =>
																	 x.Modify(securityClass, dataType, companyClass)
																);
		}

		protected bool CheckCompartments(TransactionDO Transaction)
		{
			foreach (LineItemDO LineItem in Transaction.LineItems)
			{
				if (LineItem.Quantity.GrossInventoryChange == 0 && LineItem.Quantity.NetInventoryChange == 0
					 && LineItem.Quantity.MassInventoryChange == 0)
				{
					if (LineItem.DestinationCompartmentID == "")
					{
						return false;
					}
				}
			}

			return true;
		}

		protected void CheckEntryInstructions(bool Acknowledged)
		{
			if (this.StationState != StationState.ENTRY_INSTRUCTION_PROMPT)
			{
				this.GetInstructions(true);
			}

			for (; this.InstructionIndex < this.Instructions.Count; this.InstructionIndex++)
			{
				string Instruction = (string)this.Instructions[this.InstructionIndex];

				if (this.StationState == StationState.ENTRY_INSTRUCTION_PROMPT)
				{
					if (Acknowledged)
					{
						this.StationState = StationState.IDLE;
						continue;
					}

					this.StationState = StationState.IDLE;
					this.DisplayMessage("LoadRack|Message Timeout", null, 0, this.MESSAGE_TIMEOUT);
					this.ConsecutivePrompts = 0;
					return;
				}

				this.StationState = StationState.ENTRY_INSTRUCTION_PROMPT;
				this.DisplayMessageWithAcknowledge(Instruction + " ");
				return;
			}

			if (this.Station.CardReader)
			{
				this.IssuePleaseCardIn();
			}
			else if (this.Station.TouchKeyReader)
			{
				this.IssueTouchKeyPleaseCardIn();
			}
			else
			{
				this.IssueDriverIDPrompt();
			}
		}

		protected void CheckExitInstructions(bool Acknowledged)
		{
			if (this.StationState != StationState.EXIT_INSTRUCTION_PROMPT)
			{
				this.GetInstructions(false);
			}

			for (; this.InstructionIndex < this.Instructions.Count; this.InstructionIndex++)
			{
				string Instruction = (string)this.Instructions[this.InstructionIndex];

				if (this.StationState == StationState.EXIT_INSTRUCTION_PROMPT)
				{
					if (Acknowledged)
					{
						this.StationState = StationState.IDLE;
						continue;
					}

					this.StationState = StationState.IDLE;
					this.DisplayMessage("LoadRack|Message Timeout", null, 0, this.MESSAGE_TIMEOUT);

					this.AddAlarmAndEventLogs(this.Security, this.Driver.CardOutEvent(this.Station.ID));

					this.ConsecutivePrompts = 0;
					return;
				}

				this.StationState = StationState.EXIT_INSTRUCTION_PROMPT;
				this.DisplayMessageWithAcknowledge(Instruction + " ");
				return;
			}

			FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
																	 x =>
																	 x.Add(this.Security, this.Driver.CardOutEvent(this.Station.ID))
																);
			if (this.Station.CardReader)
			{
				this.IssuePleaseCardIn();
			}
			else if (this.Station.TouchKeyReader)
			{
				this.IssueTouchKeyPleaseCardIn();
			}
			else
			{
				this.IssueDriverIDPrompt();
			}
		}

		protected virtual void CheckForEndOfOffLoadingOperation()
		{
			if (this.Station.PromptForBOLNumber) // store the entered bol number in the document number field - citgo requirement
			{
				this.Transaction.DocumentNumber = this.SelectedBOLNumber;
			}
			// check if the transaction allows multiple lineitems
			this.UpdatePermissives(false);
			if (this.TransactionSupportsMultipleLineItems == false)
			{
				this.CompleteOffLoadingTransaction();

				if (this.Station.CardReader)
				{
					this.IssuePleaseCardIn();
				}
				else
				{
					this.IssueDriverIDPrompt();
				}
				return;
			}
			// prompt for an additional off load operation
			DisplayMenuParameters Parameters = new DisplayMenuParameters
			{
				ApplyDataDictionary = true,
				DefaultItem = 0,
				MenuTimeout = 999,

				Caption = "LoadRack|Select",

				Menu = new string[2]
			};
			Parameters.Menu[0] = "LoadRack|Off Load New Batch";
			Parameters.Menu[1] = "LoadRack|Finished Off Loading";

			this.StationState = StationState.PROMPT_FOR_OFFLOAD_COMPLETE;
			this.DisplayMenu(Parameters);
		}

		private ProductGroupClass GetProductGroups(SecurityClass securityClass, Guid guid)
		{
			return FMChannelHelper.MakeCall<IProductGroups, ProductGroupClass>(
																	 x =>
																	 x.Get(securityClass, guid)
																);
		}

		protected void CheckProductAvailability(bool acknowledged)
		{
			if (this.StationState == StationState.IDLE)
			{
				if (!this.CheckProductsInStation())
				{
					this.StationState = StationState.RESET_ON_TIMEOUT;
					this.DisplayMessage("LoadRack|Configuration Mismatch", null, 0, this.MESSAGE_TIMEOUT);
					this.ConsecutivePrompts = 0;
					return;
				}

				this.DisplayPleaseWaitMessage();
				this.ClearProductCloseoutCache();
				this.AuthorizedProductIndex = 0;
			}
			else
			{
				if (acknowledged)
				{
					this.AuthorizedProductIndex++;
					this.StationState = StationState.IDLE;
				}
				else
				{
					this.StationState = StationState.IDLE;
					this.DisplayMessage("[LoadRack|Message Timeout]", null, 0, this.MESSAGE_TIMEOUT);
					this.ConsecutivePrompts = 0;
					return;
				}
			}

			ProductMapCollectionClass byVolumeExcludedProducts = this.EnumerateByAssignedToGuidAndType(
				this.Security, this.Station.IssueByVolumeTransactionAliasGuid, PRODUCT_MAP_TYPE.TRANSACTION_ALIAS_EXCLUSION_MAP);
			ProductMapCollectionClass byWeightExcludedProducts = this.EnumerateByAssignedToGuidAndType(
				this.Security, this.Station.IssueByWeightTransactionAliasGuid, PRODUCT_MAP_TYPE.TRANSACTION_ALIAS_EXCLUSION_MAP);

			if (this.ShipTo == null)
			{
				// This should never happen if the sequencing of prompts works correctly
				// As such, simply print a message and blow back to idle
				this.eventLog.WriteEntry(
					 "Load rack bail out to idle after sequencing error.", EventLogEntryType.Error);
				this.StationState = StationState.RESET_ON_TIMEOUT;
				this.DisplayMessageWithAcknowledge("LoadRack|No Customer to receive delivery");
				return;
			}

			for (; this.AuthorizedProductIndex < this.ShipTo.AuthorizedProductCollection.Count; this.AuthorizedProductIndex++)
			{
				ProductMapClass authorizedProduct = this.ShipTo.AuthorizedProductCollection[this.AuthorizedProductIndex];

				bool available = false;

				if (this.Station.Type == STATION_TYPE.LOAD_RACK)
				{
					// If not a preload Lockout Products excluded in the transaction alias
					if (this.Transaction == null)
					{
						if (authorizedProduct.LoadByWeight)
						{
							if (null != byWeightExcludedProducts.Find(x => x.IdentityGuid == authorizedProduct.AssignedGuid))
							{
								authorizedProduct.LockedOut = true;
								continue;
							}
						}
						else
						{
							if (null != byVolumeExcludedProducts.Find(x => x.IdentityGuid == authorizedProduct.AssignedGuid))
							{
								authorizedProduct.LockedOut = true;
								continue;
							}
						}
					}

					AdditiveProfileClass additiveProfile = null;
					if (authorizedProduct.AdditiveProfileGuid != Guid.Empty)
					{
						additiveProfile = this.GetAdditiveProfiles(this.Security, authorizedProduct.AdditiveProfileGuid);
					}

					// Pick Up against a Preload
					if (this.Transaction != null
						  && (this.Transaction.Status == TransactionStatus.LoadPending
						  || this.Transaction.Status == TransactionStatus.WeighOutPending))
					{
						available = this.IsProductOpenOnTransaction(authorizedProduct.AssignedGuid);

						if (available)
						{
							available = this.IsProductAvailable(additiveProfile, authorizedProduct);
						}

						if (!available && authorizedProduct.AssignedProductType == ProductType.BlendProduct)
						{
							ProductClass blend = this.GetByProductAuthorizedCompanies(
								this.Security, authorizedProduct.AssignedGuid, false);

							foreach (ProductMapClass component in blend.ComponentCollection)
							{
								if (component.LockedOut)
								{
									available = false;
									break;
								}

								if (this.IsProductAvailable(additiveProfile, component))
								{
									available = true;
								}
							}
						}
					}
					else if (this.Order != null)
					{
						// Pick Up against and Order
						available = this.IsProductOpenOnOrder(authorizedProduct.AssignedGuid);

						if (available)
						{
							available = this.IsProductAvailable(additiveProfile, authorizedProduct);

							if (!available
										  && this.IsProductPermissed(authorizedProduct)
										  && authorizedProduct.AssignedProductType == ProductType.BlendProduct)
							{
								ProductClass blend = this.GetByProductAuthorizedCompanies(
									this.Security, authorizedProduct.AssignedGuid, false);

								foreach (ProductMapClass component in blend.ComponentCollection)
								{
									if (component.LockedOut)
									{
										available = false;
										break;
									}

									if (this.IsProductAvailable(additiveProfile, component))
									{
										available = true;
									}
								}
							}
						}
					}
					else
					{
						available = this.IsProductAvailable(additiveProfile, authorizedProduct);
					}

					bool isClosedOut = this.IsProductClosedOut(authorizedProduct);

					if (!available || isClosedOut)
					{
						authorizedProduct.LockedOut = true;
					}
				}

				else if (this.Station.Type == STATION_TYPE.WEIGHT_SCALE
						  || this.Station.Type == STATION_TYPE.PRELOAD
					|| this.Station.Type == STATION_TYPE.MANUAL_BOL)
				{
					if (this.Order != null)
					{
						if (!this.IsProductOpenOnOrder(authorizedProduct.AssignedGuid))
						{
							authorizedProduct.LockedOut = true;
							continue;
						}

						available = true;
					}
					else
					{
						// sijuan: if one of blend component is lockedout, the blend should be blockedout.
						if (authorizedProduct.AssignedProductType == ProductType.BlendProduct)
						{
							ProductClass product = FMChannelHelper.MakeCall<IProducts, ProductClass>(x => x.Get(this.Security, authorizedProduct.AssignedGuid));
							foreach (ProductMapClass component in product.ComponentCollection)
							{
								if (component.LockedOut)
								{
									authorizedProduct.LockedOut = true;
								}
							}
						}

						if (authorizedProduct.LoadByWeight)
						{
							if (null != byWeightExcludedProducts.Find(x => x.IdentityGuid == authorizedProduct.AssignedGuid))
							{
								authorizedProduct.LockedOut = true;
								continue;
							}
						}

						else
						{
							if (null != byVolumeExcludedProducts.Find(x => x.IdentityGuid == authorizedProduct.AssignedGuid))
							{
								authorizedProduct.LockedOut = true;
								continue;
							}
						}

						available = true;
					}
				}

				if (available)
				{
					if (authorizedProduct.LockedOut)
					{
						this.StationState = StationState.PRODUCT_AVAILABILITY_MESSAGE;
						this.AddAlarmAndEventLogs(this.Security, this.Station.ProductLockedOutAlarm(authorizedProduct.AssignedID));
						this.DisplayMessageWithAcknowledge("[LoadRack|Locked Out:] " + GetLoadRackDisplayText(authorizedProduct));
						return;
					}

					// Check Hazardous Material 
					if (this.Carrier.HazardousMaterialExclusion && authorizedProduct.HazardousMaterial)
					{
						authorizedProduct.LockedOut = true;
						this.StationState = StationState.PRODUCT_AVAILABILITY_MESSAGE;
						this.AddAlarmAndEventLogs(this.Security, this.Carrier.HazardousMaterialExclusionEvent(authorizedProduct.AssignedID));
						this.DisplayMessageWithAcknowledge(
							"[LoadRack|Hazardous Material Exclusion:] " + GetLoadRackDisplayText(authorizedProduct));
						return;
					}
				}
			}

			this.StationState = StationState.IDLE;

			this.CheckProductContamination();
		}

		private void ClearProductCloseoutCache()
		{
			this.productClosedOutCache.Clear();
		}

		protected virtual void DisplayPleaseWaitMessage()
		{
			this.DisplayMessage("LoadRack|Please Wait", string.Empty, 0, 999);
		}

		private ProductMapCollectionClass EnumerateByAssignedToGuidAndType(SecurityClass securityClass, Guid guid, PRODUCT_MAP_TYPE productMapType)
		{
			return FMChannelHelper.MakeCall<IProductMaps, ProductMapCollectionClass>(
																	 x =>
																	 x.EnumerateByAssignedToGuidAndType(securityClass, guid, productMapType)
																);
		}

		protected void CheckProductContamination()
		{
			if (this.StationState == StationState.IDLE)
			{
				this.GetContaminationPromptStatusList();
				this.AuthorizedProductIndex = 0;
			}

			for (; this.AuthorizedProductIndex < this.ShipTo.AuthorizedProductCollection.Count; this.AuthorizedProductIndex++)
			{
				ProductMapClass authorizedProduct = this.ShipTo.AuthorizedProductCollection[this.AuthorizedProductIndex];

				if (authorizedProduct.LockedOut)
				{
					continue;
				}

				ContaminationPromptStatus contaminationPromptStatus =
					this.GetContaminationPromptStatus(authorizedProduct.ContaminationPromptLoadRackText);
				if (contaminationPromptStatus == null)
				{
					continue;
				}

				if (contaminationPromptStatus.ContaminatePrompt == null)
				{
					this.IssueContaminationPrompt(contaminationPromptStatus);
					return;
				}

				if (contaminationPromptStatus.ContaminatePrompt.Value)
				{
					if (contaminationPromptStatus.CompartmentsPreviouslyLoaded.HasValue && contaminationPromptStatus.CompartmentsPreviouslyLoaded.Value)
					{
						continue;
					}

					if (contaminationPromptStatus.CompartmentsEmpty.HasValue && contaminationPromptStatus.CompartmentsEmpty.Value)
					{
						continue;
					}
				}

				authorizedProduct.LockedOut = true;
			}

			this.StationState = StationState.IDLE;
			this.CheckProductAllocations(false);
		}

		protected void CheckProductAllocations(bool acknowledged)
		{
			if (this.StationState == StationState.IDLE)
			{
				this.AuthorizedProductIndex = 0;
			}

			else
			{
				if (acknowledged)
				{
					this.AuthorizedProductIndex++;
					this.StationState = StationState.IDLE;
				}
				else
				{
					this.CompleteTransaction();
					this.StationState = StationState.IDLE;
					this.DisplayMessage("[LoadRack|Message Timeout]", null, 0, this.MESSAGE_TIMEOUT);
					this.ConsecutivePrompts = 0;
					return;
				}
			}

			AlarmAndEventLogClass allocationAlarmAndEventLog = null;
			string loadWarningMessage = null;

			for (; this.AuthorizedProductIndex < this.ShipTo.AuthorizedProductCollection.Count; this.AuthorizedProductIndex++)
			{
				ProductMapClass authorizedProduct = this.ShipTo.AuthorizedProductCollection[this.AuthorizedProductIndex];
				if ((authorizedProduct.ProductMasterRecordGuid == null) || (authorizedProduct.ProductMasterRecordGuid == Guid.Empty))
				{
					authorizedProduct.ProductMasterRecordGuid = FMChannelHelper.MakeCall<IProducts, Guid>(
																	 x =>
																	 x.GetMasterRecordGuid(this.Security, authorizedProduct.AssignedGuid)
																);
				}
				if (authorizedProduct.LockedOut)
				{
					continue;
				}

				// Process the first Allocation Denial, if no Allocation Denial process last Allocation Warning
				for (int allocationIndex = 0; allocationIndex < 4; allocationIndex++)
				{
					AllocationClass allocation = this.AllocationArray[allocationIndex];

					if (allocationIndex == 0 && this.ShipTo.DisableShipToAllocationsCheck)
					{
						continue;
					}

					if (allocationIndex == 1 && this.ShipTo.DisableBillToAllocationsCheck)
					{
						continue;
					}

					if (allocationIndex == 2 && this.ShipTo.DisableShipperAllocationsCheck)
					{
						continue;
					}

					if (allocationIndex == 3 && this.ShipTo.DisableOwnerAllocationsCheck)
					{
						continue;
					}

					if (allocation == null)
					{
						continue;
					}

					foreach (AllocationLineItemClass lineItem in allocation.LineItemCollection)
					{
						if (lineItem.Type == ALLOCATION_TYPE.PRODUCT_ALLOCATION && lineItem.AssignedGuid != authorizedProduct.ProductMasterRecordGuid)
						{
							continue;
						}

						if (lineItem.Type == ALLOCATION_TYPE.PRODUCT_GROUP_ALLOCATION)
						{
							ProductGroupClass productGroup = this.GetProductGroups(this.Security, lineItem.AssignedGuid);
							if (!productGroup.IsProductInGroup(authorizedProduct.ProductMasterRecordGuid))
							{
								continue;
							}
						}

						if (lineItem.Limit.Value * allocation.LoadDenial / 100.0 <= lineItem.Loaded.Value)
						{
							authorizedProduct.LockedOut = true;
							this.StationState = StationState.PRODUCT_ALLOCATION_MESSAGE;
							this.AddAlarmAndEventLogs(this.Security, lineItem.AllocationDenialAlarm(allocation.ID, authorizedProduct.AssignedID));
							this.DisplayMessageWithAcknowledge("[LoadRack|Allocation Denial:] " + GetLoadRackDisplayText(authorizedProduct));
							return;
						}

						if (lineItem.Limit.Value * allocation.LoadWarning / 100.0 <= lineItem.Loaded.Value)
						{
							loadWarningMessage = "[LoadRack|Allocation Warning:] " + GetLoadRackDisplayText(authorizedProduct);
							this.StationState = StationState.PRODUCT_ALLOCATION_MESSAGE;
							allocationAlarmAndEventLog = lineItem.AllocationWarningAlarm(allocation.ID, authorizedProduct.AssignedID);
						}
					}

					// For Blends - Check each component
					if (authorizedProduct.AssignedProductType == ProductType.BlendProduct)
					{
						ProductClass product = this.GetByProductAuthorizedCompanies(
							 this.Security, authorizedProduct.AssignedGuid, false);

						foreach (ProductMapClass component in product.ComponentCollection)
						{
							foreach (AllocationLineItemClass lineItem in allocation.LineItemCollection)
							{
								if (lineItem.Type == ALLOCATION_TYPE.PRODUCT_ALLOCATION && lineItem.AssignedGuid != component.AssignedGuid)
								{
									continue;
								}

								if (lineItem.Type == ALLOCATION_TYPE.PRODUCT_GROUP_ALLOCATION)
								{
									ProductGroupClass productGroup = this.GetProductGroups(this.Security, lineItem.AssignedGuid);
									if (!productGroup.IsProductInGroup(component.AssignedGuid))
									{
										continue;
									}
								}

								if (lineItem.Limit.Value * allocation.LoadDenial / 100.0 <= lineItem.Loaded.Value)
								{
									authorizedProduct.LockedOut = true;
									this.StationState = StationState.PRODUCT_ALLOCATION_MESSAGE;
									this.AddAlarmAndEventLogs(
										 this.Security, lineItem.BlendComponentAllocationDenialAlarm(allocation.ID, component.AssignedID));
									this.DisplayMessageWithAcknowledge(
										 "[LoadRack|Allocation Denial:] " + GetLoadRackDisplayText(authorizedProduct));
									return;
								}

								if (lineItem.Limit.Value * allocation.LoadWarning / 100.0 <= lineItem.Loaded.Value)
								{
									loadWarningMessage = "[LoadRack|Allocation Warning:] " + GetLoadRackDisplayText(authorizedProduct);
									this.StationState = StationState.PRODUCT_ALLOCATION_MESSAGE;
									allocationAlarmAndEventLog = lineItem.BlendComponentAllocationWarningAlarm(allocation.ID, component.AssignedID);
								}
							}
						}
					}
				}

				if (this.StationState == StationState.PRODUCT_ALLOCATION_MESSAGE)
				{
					this.AddAlarmAndEventLogs(this.Security, allocationAlarmAndEventLog);
					this.DisplayMessageWithAcknowledge(loadWarningMessage);
					return;
				}
			}

			this.StationState = StationState.CHECK_PIDX_AUTHORIZATIONS;
			this.CheckProductPIDXAuthorization();
		}

		protected void CheckProductPIDXAuthorization()
		{
			if (!this.GetPIDXAuthorizations())
			{
				return;
			}

			if (this.PIDXAuthorizations)
			{
				this.InitializeTransaction();
				this.Transaction.DeleteFlag = true;
				this.SaveTransaction();
			}

			for (this.AuthorizedProductIndex = 0;
				  this.AuthorizedProductIndex < this.ShipTo.AuthorizedProductCollection.Count;
				  this.AuthorizedProductIndex++)
			{
				ProductMapClass authorizedProduct = this.ShipTo.AuthorizedProductCollection[this.AuthorizedProductIndex];

				if (authorizedProduct.LockedOut)
				{
					continue;
				}

				// Check PIDX Authorization
				if (this.PIDXAuthorizationArray != null)
				{
					int authorizationIndex = -1;
					foreach (PIDXProfileCompanyMapClass pidxProfileCompanyMap in this.PIDXProfileCompanyMapCollection)
					{
						authorizationIndex++;

						if (this.PIDXAuthorizationArray[authorizationIndex] == null && pidxProfileCompanyMap.UnavailableOverride)
						{
							continue;
						}

						if (this.PIDXAuthorizationArray[authorizationIndex] is AuthorizationDenyBase
							 && pidxProfileCompanyMap.DenialOverride)
						{
							PIDXProfileCollectionClass pidxProfileCollection = FMChannelHelper.MakeCall<IPIDXProfiles, PIDXProfileCollectionClass>(pidxProfiles => pidxProfiles.Enumerate(this.Security));
							PIDXProfileClass pidxProfile = pidxProfileCollection.Find(pidxProfileCompanyMap.PIDXProfileGuid);
							AuthorizationDenyBase denial = this.PIDXAuthorizationArray[authorizationIndex] as AuthorizationDenyBase;
							FMChannelHelper.MakeCall<IAlarmAndEventLogs>(alarmAndEventLogs => alarmAndEventLogs.Add(this.Security, this.Station.OverridePIDXDenialEvent((pidxProfile == null) ? string.Empty : pidxProfile.ID, denial.DenyReason, this.Driver.FirstLastName, this.ShipTo.ID)));
							this.LoadRackManager.EventOrAlarmEvent.Set();
							continue;
						}

						AuthorizationGrantedBase authorization = this.PIDXAuthorizationArray[authorizationIndex] as AuthorizationGrantedBase;
						if (authorization == null)
						{
							continue;
						}

						if (authorization.PIDXProductAuthorizations.Count == 0)
						{
							continue;
						}

						bool authorized;
						if (authorization is AuthorizationGrantedCA)
						{
							authorized = false;
						}
						else if (authorization is AuthorizationGrantedLA)
						{
							if ((authorization as AuthorizationGrantedLA).ProductAllocationMethod == "2")
							{
								authorized = true;
							}
							else
							{
								authorized = false;
							}
						}
						else
						{
							authorized = false;
						}

						foreach (PIDXProductAuthorization productAuthorization in authorization.PIDXProductAuthorizations)
						{
							if (productAuthorization.ProductTypeIndicator == "F"
							&& authorizedProduct.PIDXFamilyCode == productAuthorization.PidxProductOrFamily)
							{
								authorized = !authorized;

								break;
							}

							if (productAuthorization.ProductTypeIndicator == "P"
							&& authorizedProduct.PIDXProductCode == productAuthorization.PidxProductOrFamily)
							{
								authorized = !authorized;

								break;
							}
						}

						if (pidxProfileCompanyMap.DenialOverride && !authorized)
						{
							FMChannelHelper.MakeCall<IAlarmAndEventLogs>(alarmAndEventLogs => alarmAndEventLogs.Add(this.Security, this.Station.OverrideNoProductPIDXAuthorizationEvent(authorizedProduct.AssignedID, this.Driver.FirstLastName, this.ShipTo.ID)));
							this.LoadRackManager.EventOrAlarmEvent.Set();
							authorized = true;
						}

						if (authorized)
						{
							continue;
						}

						authorizedProduct.LockedOut = true;
						FMChannelHelper.MakeCall<IAlarmAndEventLogs>(alarmAndEventLogs => alarmAndEventLogs.Add(this.Security, this.Station.NoProductPIDXAuthorizationAlarm(authorizedProduct.AssignedID, this.Driver.FirstLastName, this.ShipTo.ID)));
						this.LoadRackManager.EventOrAlarmEvent.Set();
					}
				}
			}

			this.StationState = StationState.IDLE;

			this.ProductAvailabilityCompletion();
		}

		protected void CloseOutSplashBlendLineItem(LineItemDO lineItem)
		{
			if (lineItem == null)
			{
				throw new ArgumentNullException(nameof(lineItem));
			}

			if (lineItem.ArmNumber == null)
			{
				throw new Exception("Expected LineItem.ArmNumber to be non-null");
			}

			// Get the load arm manager
			LoadArmClass loadArm = this.Station.LoadArmCollection[lineItem.ArmNumber.Value - 1];
			LoadArmManagerClass loadArmManager = this.GetLoadArmManager(loadArm);

			if (loadArmManager == null)
			{
				throw new Exception("Could not find load arm manager: " + lineItem.ArmNumber.Value.ToString());
			}

			// Set the status and clear out splash blend processing
			if (loadArmManager.SplashSubLineItem != null)
			{
				loadArmManager.SplashSubLineItem.Status = TransactionStatus.Completed;
				loadArmManager.SplashSubLineItem = null;
			}

			loadArmManager.AdditiveProfile = null;

			// Remove the lineitem from the preloads available on the load arm if the load arm is finished
			// providing splash blend components.
			ArrayList componentList = loadArmManager.GetSplashBlendComponentList();
			if (componentList.Count == 0)
			{
				loadArmManager.Bay(this).PreLoads.Remove(lineItem);
			}

			if (loadArmManager.IsSplashBlendComplete)
			{
				lineItem.Status = TransactionStatus.Completed;
				lineItem.SplashBlendingMap = null;
			}
			else
			{
				lineItem.Status = TransactionStatus.LoadPending;
			}

			lineItem.BatchNumber = "";
			lineItem.ArmNumber = null;

			lineItem.Quantity = new QuantityDO
			{
				GrossInventoryChange = 0,
				NetInventoryChange = 0,
				MassInventoryChange = 0,
				PackageInventoryChange = 0
			};

			this.RollUpSplashBlendTotals(lineItem);

			foreach (SubLineItemDO subLineItem in lineItem.SubLineItems)
			{
				subLineItem.Status = TransactionStatus.Completed;
			}
		}

		protected bool CompartmentLoadPending(Guid equipmentGuid, string compartmentID)
		{
			foreach (TransactionDO transaction in this.PendingTransactions)
			{
				foreach (LineItemDO lineItem in transaction.LineItems)
				{
					if (lineItem.DestinationEQ.EquipmentGuid == equipmentGuid && lineItem.DestinationCompartmentID == compartmentID)
					{
						return true;
					}
				}
			}

			return false;
		}

		protected int CompartmentsInUseCurrentTransaction(EquipmentClass equipment)
		{
			int compartments = 0;

			if (equipment != null)
			{
				foreach (LineItemDO lineItem in this.Transaction.LineItems)
				{
					if (lineItem.DestinationEQ.EquipmentGuid == equipment.IdentityGuid && lineItem.ProductGuid != Guid.Empty)
					{
						compartments++;
					}
				}
			}

			return compartments;
		}

		protected virtual void CompleteDriverProcessing(bool acknowledgement)
		{
			if (this.StationState != StationState.IDLE && !acknowledgement)
			{
				this.DisplayMessage("LoadRack|Message] [LoadRack|Timeout]", null, 0, this.MESSAGE_TIMEOUT);
				this.ConsecutivePrompts = 0;
				this.StationState = StationState.IDLE;
				return;
			}

			DateTimeOffset siteTimeNow = TimeConverter.Now(this.SiteManager.Site);
			DateTimeOffset siteTimeToday = TimeConverter.ToDate(siteTimeNow);

			if (this.StationState == StationState.IDLE)
			{
				// verify that the driver has the training and qualifications required to operate this piece of equipment
				// first check if the driver or station is setup with required access

				// check the qualifications
				foreach (QualificationMapClass reqQualification in this.Station.ReqQualificationsCollection)
				{
					bool qualificationsOk = false;
					foreach (QualificationMapClass qualification in this.Driver.QualificationCollection)
					{
						// check eack station qualification and if not accessed inform the driver
						if (reqQualification.AssignedGuid == qualification.AssignedGuid)
						{
							qualificationsOk = true;
							break;
						}
					}

					if (qualificationsOk == false)
					{
						this.AddAlarmAndEventLogs(this.Security, this.Station.DriverNotQualifiedEvent(this.Driver.ID, reqQualification.ID));
						this.DisplayMessage("[LoadRack|Driver] [LoadRack|Not Qualified]", null, 0, this.MESSAGE_TIMEOUT);
						this.StationState = StationState.RESET_ON_TIMEOUT;
						return;
					}
				}

				// check the training
				foreach (QualificationMapClass reqTraining in this.Station.ReqTrainingCollection)
				{
					bool trainingOk = false;
					foreach (QualificationMapClass qualification in this.Driver.TrainingCollection)
					{
						// check eack station qualification and if not accessed inform the driver
						if (reqTraining.AssignedGuid == qualification.AssignedGuid)
						{
							trainingOk = true;
							break;
						}
					}

					if (trainingOk == false)
					{
						this.AddAlarmAndEventLogs(this.Security, this.Station.DriverNotTrainedEvent(this.Driver.ID, reqTraining.ID));
						this.DisplayMessage("[LoadRack|Driver] [LoadRack|Not Trained]", null, 0, this.MESSAGE_TIMEOUT);
						this.StationState = StationState.RESET_ON_TIMEOUT;
						return;
					}
				}

				// check the license
				foreach (QualificationMapClass reqLicense in this.Station.ReqLicenseCollection)
				{
					bool licenseOk = false;
					foreach (QualificationMapClass qualification in this.Driver.LicenseCollection)
					{
						// check eack station license and if not accessed inform the driver
						if (reqLicense.AssignedGuid == qualification.AssignedGuid)
						{
							licenseOk = true;
							break;
						}
					}

					if (licenseOk == false)
					{
						this.AddAlarmAndEventLogs(this.Security, this.Station.DriverNotLicensedEvent(this.Driver.FirstLastName, reqLicense.ID));
						this.DisplayMessage("[LoadRack|Driver] [LoadRack|Not Licensed]", null, 0, this.MESSAGE_TIMEOUT);
						this.StationState = StationState.RESET_ON_TIMEOUT;
						return;
					}
				}

				// Check for Driver Timed Out
				if (!this.Driver.InhibitInactivityLockout
					 && DateTimeOffset.Now - this.Driver._LastActivityDate.Value
				> new TimeSpan(this.SiteManager.Site._DriverTimeoutPeriod, 0, 0, 0, 0))
				{
					this.AddAlarmAndEventLogs(this.Security, this.Driver.DriverAccessTimedOutAlarm);
					this.DisplayMessage("[LoadRack|Driver Timeout]", null, 0, this.MESSAGE_TIMEOUT);
					this.StationState = StationState.RESET_ON_TIMEOUT;
					return;
				}

				if (this.Station.Type == STATION_TYPE.LOAD_RACK || this.Station.Type == STATION_TYPE.WEIGHT_SCALE
					 || this.Station.Type == STATION_TYPE.PRELOAD || this.Station.Type == STATION_TYPE.OFF_LOADING)
				{
					this.TimeIn = this.Driver.CardedIn ? this.Driver._LastActivityDate.Value : siteTimeNow;
				}

				this.Driver._LastActivityDate.Value = siteTimeNow;
				FMChannelHelper.MakeCall<IPersonnel>(
																	 x =>
																	 x.Modify(this.Security, DATA_TYPE.DYNAMIC, this.Driver)
																);

				// Check if Site AccessSchedule precludes access
				if (this.SiteManager.Site.InhibitAccessAfterHours && !this.Driver.HasRole(PERSON_ROLE.SUPERVISOR_ROLE))
				{
					DateTimeOffset now = siteTimeNow;
					int day = (int)now.Date.ToOADate();
					bool holiday = false;
					foreach (ScheduleClass schedule in this.SiteManager.Site.HolidayScheduleCollection)
					{
						if (schedule.HolidayDate.HasValue && schedule.HolidayDate.Value.Date.ToOADate() == day)
						{
							holiday = true;
							if (!schedule.Enabled || schedule.OpeningTime.Value.TimeOfDay > now.TimeOfDay
								 || schedule.ClosingTime.Value.TimeOfDay < now.TimeOfDay)
							{
								this.AddAlarmAndEventLogs(this.Security, this.Station.TerminalAccessNotScheduledEvent(this.Driver.ID));
								this.DisplayMessage("LoadRack|Terminal Access Not Scheduled", null, 0, this.MESSAGE_TIMEOUT);
								this.StationState = StationState.RESET_ON_TIMEOUT;
								return;
							}
							break;
						}
					}

					if (!holiday)
					{
						int index = (int)now.DayOfWeek;
						if (!this.SiteManager.Site.OperatingScheduleCollection[index].Enabled
							 || this.SiteManager.Site.OperatingScheduleCollection[index].OpeningTime.Value.TimeOfDay > now.TimeOfDay
							 || this.SiteManager.Site.OperatingScheduleCollection[index].ClosingTime.Value.TimeOfDay < now.TimeOfDay)
						{
							this.AddAlarmAndEventLogs(this.Security, this.Station.TerminalAccessNotScheduledEvent(this.Driver.ID));
							this.DisplayMessage("LoadRack|Terminal Access Not Scheduled", null, 0, this.MESSAGE_TIMEOUT);
							this.StationState = StationState.RESET_ON_TIMEOUT;
							return;
						}
					}
				}

				if (!this.ValidateCompany(this.Carrier, COMPANY_ROLE.CARRIER))
				{
					return;
				}

				// Check for Expired Driver Qualifications
				bool qualificationWarning = false;
				foreach (QualificationMapClass qualification in this.Driver.QualificationCollection)
				{
					if (qualification.ExpirationDate.Value <= siteTimeToday)
					{
						this.AddAlarmAndEventLogs(this.Security, qualification.PersonnelQualificationExpiredAlarm(this.Driver.ID));
						this.DisplayMessage("[LoadRack|Qualification Expired]", null, 0, this.MESSAGE_TIMEOUT);
						this.StationState = StationState.RESET_ON_TIMEOUT;
						return;
					}

					if (qualification.ExpirationDate.Value - siteTimeToday
						 < new TimeSpan(this.SiteManager.Site._DriverWarningPeriod, 0, 0, 0, 0))
					{
						this.AddAlarmAndEventLogs(this.Security, qualification.PersonnelQualificationWarningEvent(this.Driver.ID));
						qualificationWarning = true;
					}
				}

				if (qualificationWarning)
				{
					this.IssueQualificationWarningMessage();
					return;
				}

				// check for expired traing requirements
				bool trainingWarning = false;
				foreach (QualificationMapClass reqTraining in this.Driver.TrainingCollection)
				{
					if (reqTraining.ExpirationDate.Value <= siteTimeToday)
					{
						this.AddAlarmAndEventLogs(this.Security, reqTraining.PersonnelTrainingExpiredAlarm(this.Driver.ID));
						this.DisplayMessage("[LoadRack|Training Expired]", null, 0, this.MESSAGE_TIMEOUT);
						this.StationState = StationState.RESET_ON_TIMEOUT;
						return;
					}

					if (reqTraining.ExpirationDate.Value - siteTimeToday
						 < new TimeSpan(this.SiteManager.Site._DriverWarningPeriod, 0, 0, 0, 0))
					{
						this.AddAlarmAndEventLogs(this.Security, reqTraining.PersonnelTrainingWarningEvent(this.Driver.ID));
						trainingWarning = true;
					}
				}

				if (trainingWarning)
				{
					this.IssueTrainingWarningMessage();
					return;
				}
			}

			if (this.StationState == StationState.IDLE || this.StationState == StationState.DRIVER_QUALIFICATION_WARNING)
			{
				this.StationState = StationState.IDLE;

				// Check for Expired Driver Training
				bool trainingWarning = false;
				foreach (QualificationMapClass reqTraining in this.Driver.TrainingCollection)
				{
					if (reqTraining.ExpirationDate.Value <= siteTimeToday)
					{
						this.AddAlarmAndEventLogs(this.Security, reqTraining.PersonnelTrainingExpiredAlarm(this.Driver.ID));
						this.DisplayMessage("[LoadRack|Driver Training Expired]", null, 0, this.MESSAGE_TIMEOUT);
						this.StationState = StationState.RESET_ON_TIMEOUT;
						return;
					}

					if (reqTraining.ExpirationDate.Value - siteTimeToday
						 < new TimeSpan(this.SiteManager.Site._DriverWarningPeriod, 0, 0, 0, 0))
					{
						this.AddAlarmAndEventLogs(this.Security, reqTraining.PersonnelTrainingWarningEvent(this.Driver.ID));
						trainingWarning = true;
					}
				}

				if (trainingWarning)
				{
					this.IssueTrainingWarningMessage();
					return;
				}
			}

			if (this.StationState == StationState.IDLE || this.StationState == StationState.DRIVER_TRAINING_WARNING)
			{
				this.StationState = StationState.IDLE;

				// Check for Expired Driver Licenses
				bool licenseWarning = false;
				foreach (QualificationMapClass license in this.Driver.LicenseCollection)
				{
					if (license.ExpirationDate.Value <= siteTimeToday)
					{
						this.AddAlarmAndEventLogs(this.Security, license.PersonnelLicenseExpiredAlarm(this.Driver.ID));
						this.DisplayMessage("[LoadRack|Driver License Expired]", null, 0, this.MESSAGE_TIMEOUT);
						this.StationState = StationState.RESET_ON_TIMEOUT;
						return;
					}

					if (license.ExpirationDate.Value - siteTimeToday
						 < new TimeSpan(this.SiteManager.Site._DriverWarningPeriod, 0, 0, 0, 0))
					{
						this.AddAlarmAndEventLogs(this.Security, license.PersonnelLicenseWarningEvent(this.Driver.ID));
						licenseWarning = true;
					}
				}

				if (licenseWarning)
				{
					this.IssueDriverLicenseWarningMessage();
					return;
				}
			}

			if (this.StationState == StationState.IDLE || this.StationState == StationState.DRIVER_LICENSE_WARNING)
			{
				this.StationState = StationState.IDLE;

				// Check for Expired Certificate/Permit
				bool certOrPermWarning = false;
				if (this.Carrier != null)
				{
					foreach (QualificationMapClass certificationOrPermit in this.Carrier.CertificateAndPermitCollection)
					{
						if (certificationOrPermit.ExpirationDate.Value <= siteTimeToday)
						{
							this.AddAlarmAndEventLogs(
								this.Security, certificationOrPermit.CompanyCertificateOrPermitExpiredAlarm(this.Carrier.ID));
							this.DisplayMessage("[LoadRack|Cert/Perm Expired]", null, 0, this.MESSAGE_TIMEOUT);
							this.StationState = StationState.RESET_ON_TIMEOUT;
							return;
						}

						if (certificationOrPermit.ExpirationDate.Value - siteTimeToday
							 < new TimeSpan(this.SiteManager.Site._DriverWarningPeriod, 0, 0, 0, 0))
						{
							this.AddAlarmAndEventLogs(
								this.Security, certificationOrPermit.CompanyCertificateOrPermitWarningEvent(this.Carrier.ID));
							certOrPermWarning = true;
						}
					}
				}

				if (certOrPermWarning)
				{
					this.IssueCertPermWarningMessage();
					return;
				}
			}

			if (this.StationState == StationState.IDLE
				 || this.StationState == StationState.COMPANY_CERTIFICATE_OR_PERMIT_WARNING)
			{
				this.StationState = StationState.IDLE;

				// Check for Expired License
				if (this.Carrier != null && this.Carrier.LicenseExpired)
				{
					this.AddAlarmAndEventLogs(this.Security, this.Carrier.LicenseExpiredAlarm);
					this.DisplayMessage("[LoadRack|Carrier License Expired]", null, 0, this.MESSAGE_TIMEOUT);
					this.StationState = StationState.RESET_ON_TIMEOUT;
					return;
				}

				if (this.Carrier != null
					 && this.Carrier.LicenseWarning(new TimeSpan(this.SiteManager.Site._DriverWarningPeriod, 0, 0, 0, 0)))
				{
					this.AddAlarmAndEventLogs(this.Security, this.Carrier.LicenseWarningEvent);
					this.IssueCarrierLicenseWarningMessage();
					return;
				}
			}

			if (this.StationState == StationState.IDLE || this.StationState == StationState.COMPANY_LICENSE_WARNING)
			{
				this.StationState = StationState.IDLE;

				// Check for Expired Insurance
				if (this.Carrier != null && this.Carrier.InsuranceExpired)
				{
					this.AddAlarmAndEventLogs(this.Security, this.Carrier.InsuranceExpiredAlarm);
					this.DisplayMessage("[LoadRack|Carrier Insurance Expired]", null, 0, this.MESSAGE_TIMEOUT);
					this.StationState = StationState.RESET_ON_TIMEOUT;
					return;
				}

				if (this.Carrier != null
					 && this.Carrier.InsuranceWarning(new TimeSpan(this.SiteManager.Site._DriverWarningPeriod, 0, 0, 0, 0)))
				{
					this.AddAlarmAndEventLogs(this.Security, this.Carrier.InsuranceWarningEvent);
					this.IssueCarrierInsuranceWarningMessage();
					return;
				}
			}

			if (this.Station.Type == STATION_TYPE.ENTRY_GATE || this.Station.Type == STATION_TYPE.EXIT_GATE)
			{
				if (this.Station.Type == STATION_TYPE.ENTRY_GATE)
				{
					if (this.SiteManager.Site.InhibitMultipleCardIns && this.Driver.CardedIn
						 && !this.Driver.HasRole(PERSON_ROLE.SUPERVISOR_ROLE))
					{
						this.AddAlarmAndEventLogs(this.Security, this.Driver.MultipleCardInAlarm);
						this.StationState = StationState.RESET_ON_TIMEOUT;
						this.DisplayMessage("LoadRack|Multiple Card-in", null, 0, this.MESSAGE_TIMEOUT);
						return;
					}
				}
				else if (this.Station.Type == STATION_TYPE.EXIT_GATE)
				{
					this.CardOut();
				}
			}

			this.LoadArmManagerCollection.ResetPreloads(this);

			if (this.Station.Type == STATION_TYPE.OFF_LOADING)
			{
				// check if this is a single manager and single owner site and set these values
				if (!this.CheckAndSetSingleOwnerManager())
				{
					this.StationState = StationState.RESET_ON_TIMEOUT;
					this.DisplayMessage("[LoadRack|Single Owner/Manager Only]", null, 0, this.MESSAGE_TIMEOUT);
					this.ConsecutivePrompts = 0;
					return;
				}
			}

			this.CheckDriverMessages(false);
		}

		protected void CompleteOrderProcessing()
		{
			this.Transaction = null;

			this.RemoteAuthorized = false;

			this.StationState = StationState.IDLE;

			this.LoadArmManagerCollection.SetState(this, LOADARM_STATE.NORMAL);
			this.ResetStationDevice();
		}

		protected void CompleteOrderProcessingWeightOut()
		{
			this.CheckExitInstructions(false);
		}

		protected void CompleteTractorOrTankerProcessing(bool acknowledgement)
		{
			if (this.StationState != StationState.IDLE && !acknowledgement)
			{
				this.StationState = StationState.IDLE;
				this.DisplayMessage("LoadRack|Message] [LoadRack|Timeout]", null, 0, this.MESSAGE_TIMEOUT);
				this.ConsecutivePrompts = 0;
				return;
			}

			DateTimeOffset today = TimeConverter.Today(this.SiteManager.Site);

			if (this.StationState == StationState.IDLE)
			{
				// Check if Tractor/Tanker is Locked Out
				if (this.TractorOrTanker.LockedOut)
				{
					this.AddAlarmAndEventLogs(this.Security, this.TractorOrTanker.LockedOutStationAlarm(this.Driver.FirstLastName, this.Station.ID));
					this.StationState = StationState.RESET_ON_TIMEOUT;
					this.DisplayMessage("[LoadRack|Tractor/Tanker] [LoadRack|Locked Out]", null, 0, this.MESSAGE_TIMEOUT);
					return;
				}

				// Check for Expired Tag/License
				bool tagOrLicenseWarning = false;
				foreach (QualificationMapClass tagOrLicense in this.TractorOrTanker.TagAndLicenseCollection)
				{
					if (tagOrLicense.ExpirationDate.Value <= today)
					{
						this.AddAlarmAndEventLogs(this.Security, tagOrLicense.EquipmentTagOrLicenseExpiredAlarm(this.TractorOrTanker.ID, this.Driver.FirstLastName, this.Station.ID));
						this.LoadRackManager.EventOrAlarmEvent.Set();
						this.StationState = StationState.RESET_ON_TIMEOUT;
						this.DisplayMessage("[LoadRack|Tag/License Expired]", null, 0, this.MESSAGE_TIMEOUT);
						return;
					}

					if (tagOrLicense.ExpirationDate.Value - today
						 < new TimeSpan(this.SiteManager.Site._DriverWarningPeriod, 0, 0, 0, 0))
					{
						this.AddAlarmAndEventLogs(this.Security, tagOrLicense.EquipmentTagOrLicenseWarningEvent(this.TractorOrTanker.ID, this.Driver.FirstLastName, this.Station.ID));
						tagOrLicenseWarning = true;
					}
				}

				if (tagOrLicenseWarning)
				{
					this.IssueTagLicenseWarningMessage();
					this.StationState = StationState.TRACTOR_TAG_OR_LICENSE_WARNING;
					return;
				}
			}

			if (this.StationState == StationState.IDLE || this.StationState == StationState.TRACTOR_TAG_OR_LICENSE_WARNING)
			{
				this.StationState = StationState.IDLE;

				// Check for Expired Test/Inspection
				bool testOrInspectionWarning = false;
				foreach (QualificationMapClass testOrInspection in this.TractorOrTanker.TestAndInspectionCollection)
				{
					if (testOrInspection.ExpirationDate.Value <= today)
					{
						this.AddAlarmAndEventLogs(
							this.Security, testOrInspection.EquipmentTestOrInspectionExpiredAlarm(this.TractorOrTanker.ID, this.Driver.ID, this.Station.ID));
						this.LoadRackManager.EventOrAlarmEvent.Set();
						this.StationState = StationState.RESET_ON_TIMEOUT;
						this.DisplayMessage("[LoadRack|Test/Insp Expired]", null, 0, this.MESSAGE_TIMEOUT);
						return;
					}

					if (testOrInspection.ExpirationDate.Value - today
						 < new TimeSpan(this.SiteManager.Site._DriverWarningPeriod, 0, 0, 0, 0))
					{
						this.AddAlarmAndEventLogs(
							this.Security, testOrInspection.EquipmentTestOrInspectionWarningEvent(this.TractorOrTanker.ID, this.Driver.FirstLastName, this.Station.ID));
						this.LoadRackManager.EventOrAlarmEvent.Set();
						testOrInspectionWarning = true;
					}
				}

				if (testOrInspectionWarning)
				{
					this.IssueTestInspWarningMessage();
					this.StationState = StationState.TRACTOR_TEST_OR_INSPECTION_WARNING;
					return;
				}
			}

			this.EquipmentCardLogIn(this.TractorOrTanker);

			switch (this.Station.Type)
			{
				case STATION_TYPE.LOAD_RACK:
					// Prompt for 1st Trailer
					if (this.PromptForFirstTrailer
						 && (!this.IsScheduledOrder
							  || this.TransportEquipmentOnOrder > this.TransportEquipmentSpecified))
					{
						this.ConsecutivePrompts = 0;
						this.IssueEnterTrailer1Prompt();
						return;
					}

					this.StationState = StationState.IDLE;
					this.CheckProductAvailability(false);
					break;
				case STATION_TYPE.MANUAL_BOL:
				case STATION_TYPE.PRELOAD:
					if (this.PromptForFirstTrailer)
					{
						this.ConsecutivePrompts = 0;
						this.IssueEnterTrailer1Prompt();
						return;
					}

					if (!this.CheckCompartmentAvailability())
					{
						return;
					}

					this.IssueUseOrderNumberPrompt();
					break;
				case STATION_TYPE.WEIGHT_SCALE:
					if (this.PromptForFirstTrailer)
					{
						this.ConsecutivePrompts = 0;
						this.IssueEnterTrailer1Prompt();
						return;
					}

					if (!this.CheckCompartmentAvailability())
					{
						return;
					}

					this.IssueCaptureTareWeightPrompt();
					break;
				case STATION_TYPE.ENTRY_GATE:
					if (this.Station.QueryForTrailers && this.PromptForFirstTrailer)
					{
						this.ConsecutivePrompts = 0;
						this.IssueEnterTrailer1Prompt();
						return;
					}

					this.CardIn();
					this.OpenGate();
					break;
			}
		}

		public bool CheckCompartmentLoaded(EquipmentClass equipment, int compartment)
		{
			foreach (LineItemDO lineItemDO in this.Transaction.LineItems)
			{
					 // ReSharper disable CompareOfFloatsByEqualityOperator
					 if (lineItemDO.DestinationEQ.EquipmentGuid == equipment.MasterRecordGuid
					 && !(lineItemDO.GrossInventoryChange == 0.0 && lineItemDO.NetInventoryChange == 0.0 && lineItemDO.Status == TransactionStatus.Cancelled))
				{
					if (compartment.ToString(CultureInfo.InvariantCulture) == lineItemDO.DestinationCompartmentID)
					{
						return true;
					}
				}
				}

			return false;
		}

		public bool CheckCompartmentAvailable(EquipmentClass equipment, int compartment)
		{
			if (this.IsScheduledOrder)
			{
				foreach (LineItemDO orderLineItem in this.Order.LineItems)
				{
					if (orderLineItem.Status == TransactionStatus.Completed)
					{
						continue;
					}

					if (orderLineItem.DestinationEQ.EquipmentGuid == equipment.MasterRecordGuid)
					{
						if (compartment.ToString(CultureInfo.InvariantCulture) == orderLineItem.DestinationCompartmentID)
						{
							return this.IsOrderLineItemCompartmentAvailable(orderLineItem);
						}
					}
				}

				return false;
			}

			if (this.inprogressTransaction != null)
			{
				 var transactions = new TransactionDO[this.inprogressTransaction.Length + (this.Transaction != null ? 1 : 0)];
					 Array.Copy(this.inprogressTransaction, transactions, this.inprogressTransaction.Length);
				 if (this.Transaction != null)
				 {
					  transactions[this.inprogressTransaction.Length] = this.Transaction;
				 }

				 foreach (TransactionDO transactionDO in transactions)
				{
					foreach (LineItemDO lineItemDO in transactionDO.LineItems)
					{
								// ReSharper disable CompareOfFloatsByEqualityOperator
								if (lineItemDO.DestinationEQ.EquipmentGuid == equipment.MasterRecordGuid
								&& !(lineItemDO.GrossInventoryChange == 0.0 && lineItemDO.NetInventoryChange == 0.0 && lineItemDO.Status == TransactionStatus.Cancelled))
								{
							if (compartment.ToString(CultureInfo.InvariantCulture) == lineItemDO.DestinationCompartmentID)
							{
								return false;
							}
						}
								// ReSharper restore CompareOfFloatsByEqualityOperator
						  }
				}
			}

			 return true;
		}

		private bool IsOrderLineItemCompartmentAvailable(LineItemDO orderLineItem)
		{
			bool equipmentAvailable=false;

			// Determine if orderLineItem equipment is available for present card in
			EquipmentClass[] equipmentArray = { TractorOrTanker, Trailer1, Trailer2, Trailer3 };
			foreach (EquipmentClass equipmentItem in equipmentArray)
			{
				if (orderLineItem.DestinationEQ.EquipmentGuid != Guid.Empty
				&& equipmentItem != null
				&& equipmentItem.MasterRecordGuid == orderLineItem.DestinationEQ.EquipmentGuid)
				{
					try
					{
						int compartmentNumber = System.Convert.ToInt32(orderLineItem.DestinationCompartmentID);
						if (compartmentNumber <= equipmentItem.CompartmentCollection.Count)
						{
							equipmentAvailable = true;
							break;
						}
					}
					catch
					{
					}
				}
			}

			if (!equipmentAvailable)
			{
				return false;
			}

			// Determine if orderLineItem equipment compartment has already been loaded on another transaction
			if (this.inprogressTransaction != null)
			{
				foreach (TransactionDO transaction in this.inprogressTransaction)
				{
					if (Transaction != null
					&& Transaction.TransID == transaction.TransID)
					{
						continue;
					}

					foreach (LineItemDO lineItem in transaction.LineItems)
					{
						if (lineItem.DestinationEQ.EquipmentGuid != Guid.Empty
						&& orderLineItem.DestinationEQ.EquipmentGuid != Guid.Empty
						&& lineItem.DestinationEQ.EquipmentGuid == orderLineItem.DestinationEQ.EquipmentGuid
						&& lineItem.DestinationCompartmentID == orderLineItem.DestinationCompartmentID)
						{
							return false;
						}
					}
				}
			}

			// Determine if orderLineItem equipment compartment has already been loaded on current transaction
			if (this.Transaction != null)
			{
				foreach (LineItemDO lineItem in this.Transaction.LineItems)
				{
					if (lineItem.DestinationEQ.EquipmentGuid != Guid.Empty
					&& orderLineItem.DestinationEQ.EquipmentGuid != Guid.Empty
					&& lineItem.DestinationEQ.EquipmentGuid == orderLineItem.DestinationEQ.EquipmentGuid
					&& lineItem.DestinationCompartmentID == orderLineItem.DestinationCompartmentID)
					{
						return false;
					}
				}
			}

			return true;
		}

		protected void CompleteTrailer1Processing(bool acknowledgement)
		{
			if (this.StationState != StationState.IDLE && !acknowledgement)
			{
				this.StationState = StationState.IDLE;
				this.DisplayMessage("LoadRack|Message Timeout", null, 0, this.MESSAGE_TIMEOUT);
				this.ConsecutivePrompts = 0;
				return;
			}

			DateTimeOffset today = TimeConverter.Today(this.SiteManager.Site);

			if (this.Trailer1 != null)
			{
				if (this.StationState == StationState.IDLE)
				{
					// Check if Trailer is Locked Out
					if (this.Trailer1.LockedOut)
					{
						this.AddAlarmAndEventLogs(this.Security, this.Trailer1.LockedOutStationAlarm(this.Driver.FirstLastName, this.Station.ID));
						this.LoadRackManager.EventOrAlarmEvent.Set();
						this.StationState = StationState.RESET_ON_TIMEOUT;
						this.DisplayMessage("[LoadRack|Trailer] [LoadRack|Locked Out]", null, 0, this.MESSAGE_TIMEOUT);
						return;
					}

					// Check for Expired Tag/License
					bool tagOrLicenseWarning = false;
					foreach (QualificationMapClass tagOrLicense in this.Trailer1.TagAndLicenseCollection)
					{
						if (tagOrLicense.ExpirationDate.Value <= today)
						{
							this.AddAlarmAndEventLogs(this.Security, tagOrLicense.EquipmentTagOrLicenseExpiredAlarm(this.Trailer1.ID, this.Driver.FirstLastName, this.Station.ID));
							this.LoadRackManager.EventOrAlarmEvent.Set();
							this.StationState = StationState.RESET_ON_TIMEOUT;
							this.DisplayMessage("[LoadRack|Tag/License Expired]", null, 0, this.MESSAGE_TIMEOUT);
							return;
						}

						if (tagOrLicense.ExpirationDate.Value - today
							 < new TimeSpan(this.SiteManager.Site._DriverWarningPeriod, 0, 0, 0, 0))
						{
							this.AddAlarmAndEventLogs(this.Security, tagOrLicense.EquipmentTagOrLicenseWarningEvent(this.Trailer1.ID, this.Driver.FirstLastName, this.Station.ID));
							this.LoadRackManager.EventOrAlarmEvent.Set();
							tagOrLicenseWarning = true;
						}
					}

					if (tagOrLicenseWarning)
					{
						this.IssueTagLicenseWarningMessage();
						this.StationState = StationState.TRAILER1_TAG_OR_LICENSE_WARNING;
						return;
					}
				}

				if (this.StationState == StationState.IDLE || this.StationState == StationState.TRAILER1_TAG_OR_LICENSE_WARNING)
				{
					this.StationState = StationState.IDLE;

					// Check for Expired Test/Inspection
					bool testOrInspectionWarning = false;
					foreach (QualificationMapClass testOrInspection in this.Trailer1.TestAndInspectionCollection)
					{
						if (testOrInspection.ExpirationDate.Value <= today)
						{
							this.AddAlarmAndEventLogs(this.Security, testOrInspection.EquipmentTestOrInspectionExpiredAlarm(this.Trailer1.ID, this.Driver.FirstLastName, this.Station.ID));
							this.LoadRackManager.EventOrAlarmEvent.Set();
							this.StationState = StationState.RESET_ON_TIMEOUT;
							this.DisplayMessage("[LoadRack|Test/Insp Expired]", null, 0, this.MESSAGE_TIMEOUT);
							return;
						}

						if (testOrInspection.ExpirationDate.Value - today
							 < new TimeSpan(this.SiteManager.Site._DriverWarningPeriod, 0, 0, 0, 0))
						{
							this.AddAlarmAndEventLogs(this.Security, testOrInspection.EquipmentTestOrInspectionWarningEvent(this.Trailer1.ID, this.Driver.FirstLastName, this.Station.ID));
							this.LoadRackManager.EventOrAlarmEvent.Set();
							testOrInspectionWarning = true;
						}
					}

					if (testOrInspectionWarning)
					{
						this.IssueTestInspWarningMessage();
						this.StationState = StationState.TRAILER1_TEST_OR_INSPECTION_WARNING;
						return;
					}
				}
			}

			this.EquipmentCardLogIn(this.Trailer1);

			switch (this.Station.Type)
			{
				case STATION_TYPE.LOAD_RACK:
					// Prompt for 2nd Trailer
					if (this.SiteManager.Site.PromptForSecondTrailer
						 && (this.TractorOrTanker == null
							  || this.TractorOrTanker.Type == EQUIPMENT_TYPE.TRACTOR_TYPE)
						 && (!this.IsScheduledOrder
							  || this.TransportEquipmentOnOrder > this.TransportEquipmentSpecified))
					{
						this.IssueEnterTrailer2Prompt();
						return;
					}

					this.StationState = StationState.IDLE;
					this.CheckProductAvailability(false);
					break;
				case STATION_TYPE.MANUAL_BOL:
				case STATION_TYPE.PRELOAD:
					if (this.Mode != OperatingMode.Unloading || this.Station.Type == STATION_TYPE.MANUAL_BOL)
					{
						if (this.SiteManager.Site.PromptForSecondTrailer
							 && (this.TractorOrTanker == null || this.TractorOrTanker.Type == EQUIPMENT_TYPE.TRACTOR_TYPE))
						{
							this.IssueEnterTrailer2Prompt();
							return;
						}

						if (!this.CheckCompartmentAvailability())
						{
							return;
						}

						this.IssueUseOrderNumberPrompt();
					}
					else
					{
						this.DetermineTypeOfOffLoadingOperation();
					}

					break;
				case STATION_TYPE.WEIGHT_SCALE:
					if (this.Mode != OperatingMode.Unloading)
					{
						if (this.SiteManager.Site.PromptForSecondTrailer
							 && (this.TractorOrTanker == null || this.TractorOrTanker.Type == EQUIPMENT_TYPE.TRACTOR_TYPE))
						{
							this.IssueEnterTrailer2Prompt();
							return;
						}

						if (!this.CheckCompartmentAvailability())
						{
							return;
						}
					}

					this.IssueCaptureTareWeightPrompt();
					break;
				case STATION_TYPE.ENTRY_GATE:
					if (this.Station.QueryForTrailers && this.PromptForSecondTrailer
						 && (this.TractorOrTanker == null
							  || this.TractorOrTanker.Type == EQUIPMENT_TYPE.TRACTOR_TYPE))
					{
						this.IssueEnterTrailer2Prompt();
						return;
					}

					this.CardIn();
					this.OpenGate();
					break;
			}
		}

		protected void CompleteTrailer2Processing(bool acknowledgement)
		{
			if (this.StationState != StationState.IDLE && !acknowledgement)
			{
				this.StationState = StationState.IDLE;
				this.DisplayMessage("[LoadRack|Message Timeout]", null, 0, this.MESSAGE_TIMEOUT);
				this.ConsecutivePrompts = 0;
				return;
			}

			DateTimeOffset today = TimeConverter.Today(this.SiteManager.Site);

			if (this.Trailer2 != null)
			{
				if (this.StationState == StationState.IDLE)
				{
					// Check if Trailer is Locked Out
					if (this.Trailer2.LockedOut)
					{
						this.AddAlarmAndEventLogs(this.Security, this.Trailer2.LockedOutStationAlarm(this.Driver.FirstLastName, this.Station.ID));
						this.LoadRackManager.EventOrAlarmEvent.Set();
						this.StationState = StationState.RESET_ON_TIMEOUT;
						this.DisplayMessage("[LoadRack|Trailer] [LoadRack|Locked Out]", null, 0, this.MESSAGE_TIMEOUT);
						return;
					}

					// Check for Expired Tag/License
					bool tagOrLicenseWarning = false;
					foreach (QualificationMapClass tagOrLicense in this.Trailer2.TagAndLicenseCollection)
					{
						if (tagOrLicense.ExpirationDate.Value <= today)
						{
							this.AddAlarmAndEventLogs(this.Security, tagOrLicense.EquipmentTagOrLicenseExpiredAlarm(this.Trailer2.ID, this.Driver.FirstLastName, this.Station.ID));
							this.LoadRackManager.EventOrAlarmEvent.Set();
							this.StationState = StationState.RESET_ON_TIMEOUT;
							this.DisplayMessage("[LoadRack|Tag/License Expired]", null, 0, this.MESSAGE_TIMEOUT);
							return;
						}

						if (tagOrLicense.ExpirationDate.Value - today
							 < new TimeSpan(this.SiteManager.Site._DriverWarningPeriod, 0, 0, 0, 0))
						{
							this.AddAlarmAndEventLogs(this.Security, tagOrLicense.EquipmentTagOrLicenseWarningEvent(this.Trailer2.ID, this.Driver.FirstLastName, this.Station.ID));
							this.LoadRackManager.EventOrAlarmEvent.Set();
							tagOrLicenseWarning = true;
						}
					}

					if (tagOrLicenseWarning)
					{
						this.IssueTagLicenseWarningMessage();
						this.StationState = StationState.TRAILER2_TAG_OR_LICENSE_WARNING;
						return;
					}
				}

				if (this.StationState == StationState.IDLE || this.StationState == StationState.TRAILER2_TAG_OR_LICENSE_WARNING)
				{
					this.StationState = StationState.IDLE;

					// Check for Expired Test/Inspection
					bool testOrInspectionWarning = false;
					foreach (QualificationMapClass testOrInspection in this.Trailer2.TestAndInspectionCollection)
					{
						if (testOrInspection.ExpirationDate.Value <= today)
						{
							this.AddAlarmAndEventLogs(this.Security, testOrInspection.EquipmentTestOrInspectionExpiredAlarm(this.Trailer2.ID, this.Driver.FirstLastName, this.Station.ID));
							this.LoadRackManager.EventOrAlarmEvent.Set();
							this.StationState = StationState.RESET_ON_TIMEOUT;
							this.DisplayMessage("[LoadRack|Test/Insp Expired]", null, 0, this.MESSAGE_TIMEOUT);
							return;
						}

						if (testOrInspection.ExpirationDate.Value - today
							 < new TimeSpan(this.SiteManager.Site._DriverWarningPeriod, 0, 0, 0, 0))
						{
							this.AddAlarmAndEventLogs(this.Security, testOrInspection.EquipmentTestOrInspectionWarningEvent(this.Trailer2.ID, this.Driver.FirstLastName, this.Station.ID));
							this.LoadRackManager.EventOrAlarmEvent.Set();
							testOrInspectionWarning = true;
						}
					}

					if (testOrInspectionWarning)
					{
						this.IssueTestInspWarningMessage();
						this.StationState = StationState.TRAILER2_TEST_OR_INSPECTION_WARNING;
						return;
					}
				}
			}

			this.EquipmentCardLogIn(this.Trailer2);

			switch (this.Station.Type)
			{
				case STATION_TYPE.LOAD_RACK:
					if (this.SiteManager.Site.PromptForThirdTrailer
						 && (this.TractorOrTanker == null
							  || this.TractorOrTanker.Type == EQUIPMENT_TYPE.TRACTOR_TYPE)
						 && (!this.IsScheduledOrder
							  || this.TransportEquipmentOnOrder > this.TransportEquipmentSpecified))
					{
						this.IssueEnterTrailer3Prompt();
						return;
					}

					this.StationState = StationState.IDLE;
					this.CheckProductAvailability(false);
					break;
				case STATION_TYPE.MANUAL_BOL:
				case STATION_TYPE.PRELOAD:
					if (this.SiteManager.Site.PromptForThirdTrailer
						 && (this.TractorOrTanker == null
							  || this.TractorOrTanker.Type == EQUIPMENT_TYPE.TRACTOR_TYPE))
					{
						this.IssueEnterTrailer3Prompt();
						return;
					}

					if (!this.CheckCompartmentAvailability())
					{
						return;
					}

					this.IssueUseOrderNumberPrompt();
					break;
				case STATION_TYPE.WEIGHT_SCALE:
					if (this.SiteManager.Site.PromptForSecondTrailer
						 && (this.TractorOrTanker == null
							  || this.TractorOrTanker.Type == EQUIPMENT_TYPE.TRACTOR_TYPE))
					{
						this.IssueEnterTrailer3Prompt();
						return;
					}

					if (!this.CheckCompartmentAvailability())
					{
						return;
					}

					this.IssueCaptureTareWeightPrompt();
					break;
				case STATION_TYPE.ENTRY_GATE:
					if (this.Station.QueryForTrailers && this.PromptForThirdTrailer
						 && (this.TractorOrTanker == null
							  || this.TractorOrTanker.Type == EQUIPMENT_TYPE.TRACTOR_TYPE))
					{
						this.IssueEnterTrailer3Prompt();
						return;
					}

					this.CardIn();
					this.OpenGate();
					break;
			}
		}

		protected void CompleteTrailer3Processing(bool acknowledgement)
		{
			if (this.StationState != StationState.IDLE
				 && !acknowledgement)
			{
				this.StationState = StationState.IDLE;
				this.DisplayMessage("LoadRack|Message Timeout", null, 0, this.MESSAGE_TIMEOUT);
				this.ConsecutivePrompts = 0;
				return;
			}

			DateTimeOffset today = TimeConverter.Today(this.SiteManager.Site);

			if (this.Trailer3 != null)
			{
				if (this.StationState == StationState.IDLE)
				{
					// Check if Trailer is Locked Out
					if (this.Trailer3.LockedOut)
					{
						FMChannelHelper.MakeCall<IAlarmAndEventLogs>(x => x.Add(this.Security, this.Trailer3.LockedOutStationAlarm(this.Driver.FirstLastName, this.Station.ID)));
						this.LoadRackManager.EventOrAlarmEvent.Set();
						this.StationState = StationState.RESET_ON_TIMEOUT;
						this.DisplayMessage("[LoadRack|Trailer] [LoadRack|Locked Out]", null, 0, this.MESSAGE_TIMEOUT);
						return;
					}

					// Check for Expired Tag/License
					bool tagOrLicenseWarning = false;
					foreach (QualificationMapClass tagOrLicense in this.Trailer3.TagAndLicenseCollection)
					{
						if (tagOrLicense.ExpirationDate.Value <= today)
						{
							FMChannelHelper.MakeCall<IAlarmAndEventLogs>(x => x.Add(this.Security, tagOrLicense.EquipmentTagOrLicenseExpiredAlarm(this.Trailer3.ID, this.Driver.FirstLastName, this.Station.ID)));
							this.LoadRackManager.EventOrAlarmEvent.Set();
							this.StationState = StationState.RESET_ON_TIMEOUT;
							this.DisplayMessage("[LoadRack|Tag/License Expired]", null, 0, this.MESSAGE_TIMEOUT);
							return;
						}

						if (tagOrLicense.ExpirationDate.Value - today < new TimeSpan(this.SiteManager.Site._DriverWarningPeriod, 0, 0, 0, 0))
						{
							FMChannelHelper.MakeCall<IAlarmAndEventLogs>(x => x.Add(this.Security, tagOrLicense.EquipmentTagOrLicenseWarningEvent(this.Trailer3.ID, this.Driver.FirstLastName, this.Station.ID)));
							this.LoadRackManager.EventOrAlarmEvent.Set();
							tagOrLicenseWarning = true;
						}
					}

					if (tagOrLicenseWarning)
					{
						this.IssueTagLicenseWarningMessage();
						this.StationState = StationState.TRAILER3_TAG_OR_LICENSE_WARNING;
						return;
					}
				}

				if (this.StationState == StationState.IDLE
					 || this.StationState == StationState.TRAILER3_TAG_OR_LICENSE_WARNING)
				{
					this.StationState = StationState.IDLE;

					// Check for Expired Test/Inspection
					bool testOrInspectionWarning = false;
					foreach (QualificationMapClass testOrInspection in this.Trailer3.TestAndInspectionCollection)
					{
						if (testOrInspection.ExpirationDate.Value <= today)
						{
							FMChannelHelper.MakeCall<IAlarmAndEventLogs>(x => x.Add(this.Security, testOrInspection.EquipmentTestOrInspectionExpiredAlarm(this.Trailer3.ID, this.Driver.FirstLastName, this.Station.ID)));
							this.LoadRackManager.EventOrAlarmEvent.Set();
							this.StationState = StationState.RESET_ON_TIMEOUT;
							this.DisplayMessage("[LoadRack|Test/Insp Expired]", null, 0, this.MESSAGE_TIMEOUT);
							return;
						}

						if (testOrInspection.ExpirationDate.Value - today < new TimeSpan(this.SiteManager.Site._DriverWarningPeriod, 0, 0, 0, 0))
						{
							FMChannelHelper.MakeCall<IAlarmAndEventLogs>(x => x.Add(this.Security, testOrInspection.EquipmentTestOrInspectionWarningEvent(this.Trailer3.ID, this.Driver.FirstLastName, this.Station.ID)));
							this.LoadRackManager.EventOrAlarmEvent.Set();
							testOrInspectionWarning = true;
						}
					}

					if (testOrInspectionWarning)
					{
						this.IssueTestInspWarningMessage();
						this.StationState = StationState.TRAILER3_TEST_OR_INSPECTION_WARNING;
						return;
					}
				}
			}

			this.EquipmentCardLogIn(this.Trailer3);

			switch (this.Station.Type)
			{
				case STATION_TYPE.LOAD_RACK:
					this.StationState = StationState.IDLE;
					this.CheckProductAvailability(false);
					break;
				case STATION_TYPE.MANUAL_BOL:
				case STATION_TYPE.PRELOAD:
					if (!this.CheckCompartmentAvailability())
					{
						return;
					}

					this.IssueUseOrderNumberPrompt();
					break;
				case STATION_TYPE.WEIGHT_SCALE:
					if (!this.CheckCompartmentAvailability())
					{
						return;
					}

					this.IssueCaptureTareWeightPrompt();
					break;
				case STATION_TYPE.ENTRY_GATE:
					this.CardIn();
					this.OpenGate();
					break;
			}
		}

		protected SubLineItemDO CreateExternalComponentSubLineItem(
			LoadArmManagerClass loadArmManager,
			LineItemDO lineItem,
			SubLineItemDO componentSubLineItem,
			ProductMapClass armComponent,
			double externalComponentPercent,
			double blendPercentage,
			ProductClass externalProduct)
		{
			SubLineItemDO subLineItem = new SubLineItemDO
			{
				Status = componentSubLineItem.Status,
				BatchNumber = componentSubLineItem.BatchNumber,
				Temperature = componentSubLineItem.Temperature,
				PresetAmount = 0.0
			};

			if (this.Station.SwingArmPosition == "A")
			{
				subLineItem.ArmNumber = loadArmManager.LoadArm.BayAArmNumber;
			}
			else
			{
				subLineItem.ArmNumber = loadArmManager.LoadArm.BayBArmNumber;
			}

			subLineItem.LineNumber = armComponent.PresetNumber;

			subLineItem.Product = armComponent.AssignedID;
			subLineItem.ProductCode = armComponent.AssignedCode;
			subLineItem.ProductType = ProductClass.ProductTypeID(armComponent.AssignedProductType);
			subLineItem.ProductGuid = armComponent.AssignedGuid;
			subLineItem.IsEthanol = externalProduct.IsEthanol;

			if (subLineItem.IsEthanol) {
				lineItem.IsEthanolBlend = true;
				subLineItem.VcfModuleSettings = externalProduct._VcfModuleSettings;
			}

			UnitsHelperClass unitsHelper = new UnitsHelperClass(
				 this.Security,
				 this.SiteManager.Site,
				 this.CurrentTransactionAlias,
				 externalProduct);
			unitsHelper.SetUnits(subLineItem, ProductType.ComponentProduct, null);

			double presetAmount = lineItem.PresetAmount ?? 0.0;
			if (subLineItem.VolumeUnits != lineItem.VolumeUnits)
			{
				presetAmount = StationManagerClass.Convert(lineItem.PresetAmount ?? 0.0, lineItem.VolumeUnits, subLineItem.VolumeUnits);
			}

			subLineItem.PresetAmount = presetAmount * externalComponentPercent;

			subLineItem.MeterID = componentSubLineItem.MeterID;
			subLineItem.MeterGuid = componentSubLineItem.MeterGuid;

			subLineItem.Quantity = new QuantityDO();
			double inventoryChange = this.SiteManager.Site.LoadByNet
				? StationManagerClass.Convert(
							componentSubLineItem.Quantity.NetInventoryChange,
							componentSubLineItem.VolumeUnits,
							subLineItem.VolumeUnits)
				: StationManagerClass.Convert(
							componentSubLineItem.Quantity.GrossInventoryChange,
							componentSubLineItem.VolumeUnits,
							subLineItem.VolumeUnits);
			if (this.SiteManager.Site.LoadByNet)
			{
				subLineItem.Quantity.NetInventoryChange = Math.Round(
					 inventoryChange * blendPercentage,
					 subLineItem.VolumeDecimalPlaces,
					 MidpointRounding.AwayFromZero);
			}
			else
			{
				subLineItem.Quantity.GrossInventoryChange = Math.Round(
					 inventoryChange * blendPercentage,
					 subLineItem.VolumeDecimalPlaces,
					 MidpointRounding.AwayFromZero);
			}

			subLineItem.Temperature = componentSubLineItem.Temperature;

			TankClass tank = this.SiteManager.GetTank(armComponent, this.Manager);
			if (tank != null)
			{
				subLineItem.StorageLocationID = tank.ID;
				subLineItem.StorageLocationTankGuid = tank.IdentityGuid;

				EngineeringUnit densityUnits = subLineItem.DensityUnits;
				byte densityDecimalPlaces = subLineItem.DensityDecimalPlaces;

				if (subLineItem.Density == null)
				{
					subLineItem.Density = 0.0;
				}

				ProcessVariableClass pv = tank.ProcessVariableCollection[PROCESS_VARIABLE_TYPE.STANDARD_DENSITY_PV];
				if (pv == null)
				{
					if (!subLineItem.Density_BadQualityLogged)
					{
						this.eventLog.WriteEntry(
							 "CreateExternalComponentSubLineItem : No Tank Density Process Variable",
							 EventLogEntryType.Error);
						subLineItem.Density_BadQualityLogged = true;
					}
				}

				else if ((!this.SiteManager.Site.UseLastKnownGoodTankData && !pv.IsQualityGood) || !(pv.SIValue is double))
				{
					if (!subLineItem.Density_BadQualityLogged)
					{
						this.eventLog.WriteEntry(
							 "CreateExternalComponentSubLineItem : External Component Density OPC Quality Bad " + pv.OPCItemID,
							 EventLogEntryType.Error);
						subLineItem.Density_BadQualityLogged = true;
					}
				}
				else
				{
					subLineItem.Density = (double)pv.GetValue(densityUnits, densityDecimalPlaces);
					subLineItem.Density_BadQualityLogged = false;
				}

				if (!subLineItem.Density_BadQualityLogged && !subLineItem.Temperature_BadQualityLogged)
				{
					//TODO: Convert Volume Correct to FMBusinessServices
					//double vcf = VolumeCorrectionDotNet.VolumeCorrection.CalculateVCF(
					//               ExternalProduct._MajorCorrectionMethod,
					//               (byte)ExternalProduct._MinorCorrectionMethod,
					//               SubLineItem.Temperature.Value,
					//               temperatureUnits,
					//               ExternalProduct._StandardTemperature.Value,
					//               ExternalProduct._StandardTemperature.Units,
					//               SubLineItem.Density.Value,
					//               densityUnits,
					//               0.0,
					//               SiteManager.Site.PressureUnits,
					//               ExternalProduct._AlternateTemperature.Value,
					//               ExternalProduct._AlternatePressure.Value,
					//               ExternalProduct.CorrectionFactor.CorrectionFactorData);

					//SubLineItem.VCF = vcf;

					if (this.SiteManager.Site.LoadByNet)
					{
						subLineItem.Quantity.GrossInventoryChange =
							 Math.Round(
								  subLineItem.Quantity.NetInventoryChange / (subLineItem.VCF ?? 1.0),
								  subLineItem.VolumeDecimalPlaces,
								  MidpointRounding.AwayFromZero);
					}
					else
					{
						subLineItem.Quantity.NetInventoryChange =
							 Math.Round(
								  subLineItem.Quantity.GrossInventoryChange * (subLineItem.VCF ?? 1.0),
								  subLineItem.VolumeDecimalPlaces,
								  MidpointRounding.AwayFromZero);
					}

					if (this.Station.EthanolExcess && subLineItem.IsEthanol)
					{
						EngineeringUnit temperatureUnits = subLineItem.TemperatureUnits;
						byte temperatureDecimalPlaces = subLineItem.TemperatureDecimalPlaces;

						if (subLineItem.Temperature == null)
						{
							subLineItem.Temperature = 0.0;
						}

						pv = tank.ProcessVariableCollection[PROCESS_VARIABLE_TYPE.TEMPERATURE_PV];
						if (pv == null)
						{
							if (!subLineItem.Temperature_BadQualityLogged)
							{
								this.eventLog.WriteEntry(
									 "CreateExternalComponentSubLineItem : No Tank Temperature Process Variable",
									 EventLogEntryType.Error);
								subLineItem.Temperature_BadQualityLogged = true;
							}
						}
						else if ((!this.SiteManager.Site.UseLastKnownGoodTankData && !pv.IsQualityGood) || !(pv.SIValue is double))
						{
							if (!subLineItem.Temperature_BadQualityLogged)
							{
								this.eventLog.WriteEntry(
									 "CreateExternalComponentSubLineItem : External Component Temperature OPC Quality Bad " + pv.OPCItemID,
									 EventLogEntryType.Error);
								subLineItem.Temperature_BadQualityLogged = true;
							}
						}
						else
						{
							subLineItem.Temperature = (double)pv.GetValue(temperatureUnits, temperatureDecimalPlaces);
							subLineItem.Temperature_BadQualityLogged = false;
						}

						EngineeringUnit pressureUnits = subLineItem.PressureUnits;
						byte pressureDecimalPlaces = subLineItem.PressureDecimalPlaces;

						if (subLineItem.Pressure == null)
						{
							subLineItem.Pressure = 0.0;
						}

						pv = tank.ProcessVariableCollection[PROCESS_VARIABLE_TYPE.VAPOR_PRESSURE_PV];
						if (pv == null)
						{
							if (!subLineItem.Pressure_BadQualityLogged)
							{
								this.eventLog.WriteEntry(
									 "CreateExternalComponentSubLineItem : No Tank Pressure Process Variable",
									 EventLogEntryType.Error);
								subLineItem.Pressure_BadQualityLogged = true;
							}
						}
						else if ((!this.SiteManager.Site.UseLastKnownGoodTankData && !pv.IsQualityGood) || !(pv.SIValue is double))
						{
							if (!subLineItem.Pressure_BadQualityLogged)
							{
								this.eventLog.WriteEntry(
									 "CreateExternalComponentSubLineItem : External Component Pressure OPC Quality Bad " + pv.OPCItemID,
									 EventLogEntryType.Error);
								subLineItem.Pressure_BadQualityLogged = true;
							}
						}
						else
						{
							subLineItem.Pressure = (double)pv.GetValue(pressureUnits, pressureDecimalPlaces);
							subLineItem.Pressure_BadQualityLogged = false;
						}

						if (!subLineItem.Temperature_BadQualityLogged
							|| !subLineItem.Pressure_BadQualityLogged)
						{
							Vcf vcfCalc = new Vcf();
							vcfCalc.VcfSettings.Alpha = externalProduct._VcfModuleSettings.Alpha;

							double vcf = vcfCalc.VcfCalculation(
								(ECorrectionTypeMajor)System.Convert.ToInt32(subLineItem.VcfModuleSettings.CorrectionMethodType),
								(ECorrectionTypeMinor)System.Convert.ToInt32(subLineItem.VcfModuleSettings.CorrectionMethodSpecific),
								subLineItem.Temperature.Value,
								subLineItem.TemperatureUnits,
								subLineItem.VcfModuleSettings.BaseTemperature.Value,
								subLineItem.TemperatureUnits,
								subLineItem.Density.Value,
								subLineItem.DensityUnits,
								subLineItem.Pressure.Value,
								subLineItem.PressureUnits,
								subLineItem.VcfModuleSettings.AlternateTemperature.Value,
								subLineItem.TemperatureUnits,
								subLineItem.VcfModuleSettings.AlternateBasePressure.Value,
								subLineItem.PressureUnits,
								new double[] { externalProduct.CorrectionFactor0, externalProduct.CorrectionFactor1, externalProduct.CorrectionFactor2, externalProduct.CorrectionFactor3, externalProduct.CorrectionFactor4 },
								out double ctlReturn,
								out double cplReturn,
								out double returnedVcf,
								out double returnUncroundedVcf);

							subLineItem.VCF = vcf;
						}
					}
				}
			}

			return subLineItem;
		}

		protected TransactionDO CreateMeterReadingTransaction(
		  DateTimeOffset siteTimeNow,
		  LoadArmManagerClass LoadArmManager,
		  ItemValueResult NonResettableGrossVolume,
		  ProductMapClass ProductMap,
		  TransactionAliasClass MeterReadingTransactionAlias,
		  DateTimeOffset InventoryDateTime)
		{
			// Skip Meter Readings when there isn't a Tank Assignment
			TankClass Tank = this.SiteManager.GetTank(ProductMap, this.Manager);

			if (Tank == null)
			{
				return null;
			}

			if (NonResettableGrossVolume.Quality != Quality.Good)
			{
				this.eventLog.WriteEntry(
					"CreateMeterReadingTransaciton : OPC Quality Bad " + NonResettableGrossVolume.ItemName, EventLogEntryType.Error);
				return null;
			}
			UnitsHelperClass unitsHelper = new UnitsHelperClass(this.Security, this.SiteManager.Site, MeterReadingTransactionAlias, null);

			TransactionDO Transaction = new TransactionDO
			{
				Alias = MeterReadingTransactionAlias.ID,
				TransactionAliasGuid = MeterReadingTransactionAlias.MasterRecordGuid,
				TransTypeID = TransactionTypes.T12_InventoryNotAffected,
				TransactionDateTime = siteTimeNow,
				InventoryDate = TimeConverter.ToDate(InventoryDateTime).Date,
				Site = this.SiteManager.Site.ID,
				SiteGuid = this.SiteManager.Site.IdentityGuid,
				ManagerID = Tank.ManagerID,
				ManagerCompanyGuid = Tank.ManagerGuid,
				OriginApplication = TransactionOrigin.TerminalAutomationService
			};
			unitsHelper.SetUnits(Transaction, 0);

			LineItemDO LineItem = new LineItemDO
			{
				LoadingLocationID = this.Station.ID,
				LoadingLocationStationGuid = this.Station.IdentityGuid,
				StorageLocationID = Tank.ID,
				StorageLocationTankGuid = Tank.IdentityGuid,
				Product = Tank.ProductID,
				ProductCode = Tank.ProductCode,
				ProductGuid = Tank.ProductGuid,
				ProductType = ProductClass.ProductTypeID(ProductMap.AssignedProductType),
				MeterID = ProductMap.Meter.ID,
				MeterGuid = ProductMap.Meter.IdentityGuid
			};

			ProductClass product = FMChannelHelper.MakeCall<IProducts, ProductClass>(
																	 x =>
																	 x.Get(this.Security, LineItem.ProductGuid)
																);
			unitsHelper.SetUnits(LineItem, ProductMap.AssignedProductType, product);

			LineItem.MeterReading.MeterStart = System.Convert.ToDouble(NonResettableGrossVolume.Value);
			LineItem.MeterReading.MeterStop = System.Convert.ToDouble(NonResettableGrossVolume.Value);
			LineItem.MeterReading.StartDateTime = InventoryDateTime;
			LineItem.MeterReading.StopDateTime = InventoryDateTime;

			Transaction.LineItems.Add(LineItem);
			return Transaction;
		}

		protected virtual void EndTransaction()
		{
		}

		protected string EquipmentID(EquipmentClass Equipment)
		{
			if (Equipment == null)
			{
				return "";
			}

			if (this.SiteManager.Site.UseCompanyEquipmentIdentifiers)
			{
				return Equipment.CompanyEquipmentID;
			}
			else
			{
				return Equipment.ID;
			}
		}

		protected int FirstAvailableCompartment(EquipmentClass Equipment)
		{
			if (Equipment != null)
			{
				for (int CompartmentNumber = 1; CompartmentNumber <= Equipment.CompartmentCollection.Count; CompartmentNumber++)
				{
					if (!this.CompartmentLoadPending(Equipment.IdentityGuid, CompartmentNumber.ToString()))
					{
						return CompartmentNumber;
					}
				}
			}

			return 0;
		}

		protected void GenerateCompartmentList()
		{
			this.CompartmentList = new ArrayList();

			// If there is a TractorOrTanker entry, count up the compartments
			if (this.TractorOrTanker != null && this.TractorOrTanker.CompartmentCollection != null)
			{
				this.AddToCompartmentArray(this.TractorOrTanker);
			}

			// If there is a Trailer1 entry, count up the compartments
			if (this.Trailer1 != null && this.Trailer1.CompartmentCollection != null)
			{
				this.AddToCompartmentArray(this.Trailer1);
			}

			// If there is a Trailer2 entry, count up the compartments
			if (this.Trailer2 != null && this.Trailer2.CompartmentCollection != null)
			{
				this.AddToCompartmentArray(this.Trailer2);
			}
		}

		protected virtual GetTransactionDO GetAppropriateWeighOutPreloads()
		{
			DateTimeOffset Today = TimeConverter.Today(this.SiteManager.Site);

			// Check for preloads for the current driver
			GetTransactionSR getTransactionSR = new GetTransactionSR
			{
				Security = this.Security,
				Request =
				GetTransactionRequest.SITE_TYPEID_TRANSDATE_STATUS_OPERATORPERSONNELGUID_LINEITEMSTATUS_INVENTORYDATE,
				Site = this.SiteManager.Site.ID,
				TransTypeID = TransactionTypes.T5_PrimaryDisbursement,
				BeginningDate = Today.AddDays(-1.0),
				EndingDate = Today.AddDays(1.0),
				OperatorPersonnelGuid = this.Driver.MasterRecordGuid,
				Status = ((int)TransactionStatus.WeighOutPending).ToString(),
				LineItemStatus = ((int)TransactionStatus.LoadPending).ToString(),
				InventoryDate = this.SiteManager.GetInventoryDate()
			};

			GetTransactionDO getTransactionDO = FMChannelHelper.MakeCall<IGetTransactionProcessor, GetTransactionDO>(
																	 x =>
																	 x.Process(getTransactionSR)
																);
			return getTransactionDO;
		}

		protected ContaminationPromptStatus GetContaminationPromptStatus(string ContaminationPromptLoadRackText)
		{
			if (this.ContaminationPromptStatusList == null)
			{
				return null;
			}

			foreach (ContaminationPromptStatus contaminationPromptStatus in this.ContaminationPromptStatusList)
			{
				if (contaminationPromptStatus.ContaminationPromptLoadRackText == ContaminationPromptLoadRackText)
				{
					return contaminationPromptStatus;
				}
			}

			return null;
		}

		protected void GetContaminationPromptStatusList()
		{
			this.ContaminationPromptStatusList = new ArrayList();

			foreach (ProductMapClass AuthorizedProduct in this.ShipTo.AuthorizedProductCollection)
			{
				if (AuthorizedProduct.ContaminationPromptLoadRackText == null
					 || AuthorizedProduct.ContaminationPromptLoadRackText == string.Empty)
				{
					continue;
				}

				ContaminationPromptStatus contaminationPromptStatus =
					this.GetContaminationPromptStatus(AuthorizedProduct.ContaminationPromptLoadRackText);
				if (contaminationPromptStatus == null)
				{
					this.ContaminationPromptStatusList.Add(
						new ContaminationPromptStatus(AuthorizedProduct.ContaminationPromptLoadRackText));
				}
			}
		}

		protected void GetInstructions(bool Entry)
		{
			this.Instructions.Clear();
			this.InstructionIndex = 0;

			// Get Unique Products
			ArrayList productGuids = new ArrayList();
			foreach (TransactionDO Transaction in this.PendingTransactions)
			{
				foreach (LineItemDO LineItem in Transaction.LineItems)
				{
					bool Found = false;
					foreach (Guid productGuid in productGuids)
					{
						if (LineItem.ProductGuid == productGuid)
						{
							Found = true;
							break;
						}
					}

					if (!Found)
					{
						productGuids.Add(LineItem.ProductGuid);
					}
				}
			}

			// Get Unique Product Groups
			ArrayList productGroupIdentityGuids = new ArrayList();
			foreach (Guid productGuid in productGuids)
			{
				ProductClass Product = this.GetByProductAuthorizedCompanies(this.Security, productGuid, false);
				ProductMapCollectionClass ProductMapCollection = this.EnumerateProductMapByAssignedGuidAndType(
					this.Security, Product.IdentityGuid, PRODUCT_MAP_TYPE.PRODUCT_GROUP_MAP);
				bool Found = false;
				foreach (ProductMapClass ProductMap in ProductMapCollection)
				{
					foreach (Guid productGroupGuid in productGroupIdentityGuids)
					{
						if (ProductMap.AssignedToGuid == productGroupGuid)
						{
							Found = true;
							break;
						}
					}

					if (!Found)
					{
						productGroupIdentityGuids.Add(ProductMap.AssignedToGuid);
					}
				}
			}

			// Get Unique Instructions
			foreach (Guid productGroupIdentityGuid in productGroupIdentityGuids)
			{
				ProductGroupClass ProductGroup = this.GetProductGroups(this.Security, productGroupIdentityGuid);
				if (Entry)
				{
					foreach (ApplicationStringMapClass Instruction in ProductGroup.EntryMessageCollection)
					{
						if (!this.Instructions.Contains(Instruction.ID))
						{
							this.Instructions.Add(Instruction.ID);
						}
					}
				}

				else
				{
					foreach (ApplicationStringMapClass Instruction in ProductGroup.ExitMessageCollection)
					{
						if (!this.Instructions.Contains(Instruction.ID))
						{
							this.Instructions.Add(Instruction.ID);
						}
					}
				}
			}
		}

		private ProductMapCollectionClass EnumerateProductMapByAssignedGuidAndType(SecurityClass securityClass, Guid guid, PRODUCT_MAP_TYPE productMapType)
		{
			return FMChannelHelper.MakeCall<IProductMaps, ProductMapCollectionClass>(
																	 x =>
																	 x.EnumerateByAssignedGuidAndType(securityClass, guid, productMapType)
																);
		}

		protected bool GetLoadIDList()
		{
			bool allPreloadsAssociatedWithLoadIDs = true;
			this.LoadIDList = new ArrayList();

			foreach (DataRow row in this.PreloadDataSet.Tables[0].Rows)
			{
				string loadID = row["LoadID"] as string;

				if (string.IsNullOrEmpty(loadID))
				{
					allPreloadsAssociatedWithLoadIDs = false;
					continue;
				}

				this.AddNonDuplicateLoadID(loadID);
			}

			this.LoadIDList.Sort();

			return allPreloadsAssociatedWithLoadIDs;
		}

		/// <summary>
		/// Gets the maximum quantity allowed to load.
		/// </summary>
		/// <param name="product">
		/// The product.
		/// </param>
		/// <param name="currentDensity">
		/// The current density.
		/// </param>
		/// <param name="currentVcf">
		/// The current vcf.
		/// </param>
		/// <param name="currentPressure">
		/// The current pressure.
		/// </param>
		/// <param name="component">
		/// The component.
		/// </param>
		/// <param name="lineitem">
		/// The lineitem.
		/// </param>
		/// <returns>
		/// Maximum allowable load<see cref="double"/>.
		/// </returns>
		/// <remarks>
		/// Determines the maximum amount of product allowed to be loaded.
		/// Takes into account:
		/// <list type="bullet">
		/// <item>
		/// <description>Site Maximum</description>
		/// </item>
		/// <item>
		/// <description>Vehicle maximum weight (takes into account projected loads in other compartments)</description>
		/// </item>
		/// <item>
		/// <description>Configured compartment maximum/safe fill</description>
		/// </item>
		/// <item>
		/// <description>Amount remaining on order (if applicable)</description>
		/// </item>
		/// </list>
		/// </remarks>
		protected double GetMaximum(
		 ProductClass product, double? currentDensity, double? currentVcf, double? currentPressure, ProductMapClass component, LineItemDO lineitem)
		{
			//return Maximum.Value;
			SIDouble densityStd;
			SIDouble density15;
			SIDouble density15Air;
			SIDouble degrees15;
			SIDouble densityStdAir;
			double vcf15;

			if (this.Station.Type == STATION_TYPE.MANUAL_BOL)
			{
				if (this.Order == null)
				{
					return this.SiteManager.Site._MaximumLoadAmount.Value;
				}

				LineItemDO associatedOrderLineItem = null;
				if (this.SiteManager.Site.EnforceSalesOrderLimit)
				{
					foreach (LineItemDO orderLineItem in this.Order.LineItems)
					{
						if (orderLineItem.TransactionLineItemGuid == lineitem.OrderReferenceTransactionLineItemGuid)
						{
							associatedOrderLineItem = orderLineItem;
							break;
						}
					}
				}

				if (associatedOrderLineItem == null)
				{
					return this.SiteManager.Site._MaximumLoadAmount.Value;
				}

				double ret = this.SiteManager.Site.LoadByNet ? associatedOrderLineItem.Quantity.NetInventoryChange : associatedOrderLineItem.Quantity.GrossInventoryChange;

				// Check for other transaction line items already present which reference this same order line item.
				// Subtract their quantities from the amount available.
				foreach (LineItemDO otherLineItem in this.Transaction.LineItems)
				{
					if (otherLineItem == lineitem)
					{
						// Don't count ourself
						continue;
					}

					if (otherLineItem.OrderReferenceTransactionLineItemGuid != lineitem.OrderReferenceTransactionLineItemGuid)
					{
						// Different order line item means different product (except for Scheduled Orders,
						// which don't apply for the manual stations)
						continue;
					}

					if (this.SiteManager.Site.LoadByNet)
					{
						ret -= otherLineItem.Quantity.NetInventoryChange;
					}
					else
					{
						ret -= otherLineItem.Quantity.GrossInventoryChange;
					}
				}

				return ret > this.SiteManager.Site._MaximumLoadAmount.Value ? this.SiteManager.Site._MaximumLoadAmount.Value : ret;
			}

			// Not a manual BOL station
			EngineeringUnit volumeUnits = (this.CurrentTransactionAlias.VolumeUnits != EngineeringUnit.FmSiteUnits)
																	  ? this.CurrentTransactionAlias.VolumeUnits
																	  : this.SiteManager.Site.VolumeUnits;

			EngineeringUnit densityUnits = (this.CurrentTransactionAlias.DensityUnits != EngineeringUnit.FmSiteUnits)
																		 ? this.CurrentTransactionAlias.DensityUnits
																		 : this.SiteManager.Site.DensityUnits;

			EngineeringUnit massUnits = (this.CurrentTransactionAlias.MassUnits != EngineeringUnit.FmSiteUnits)
																		 ? this.CurrentTransactionAlias.MassUnits
																		 : this.SiteManager.Site.MassUnits;

			SIDouble maximum = new SIDouble { Units = volumeUnits, SIValue = this.SiteManager.Site._MaximumLoadAmount.SIValue };

			if (component != null)
			{
				maximum.SIValue *= component.BlendPercentage / 100;
			}

			// Limit Maximum based upon vehicle weight
			SIDouble weight = new SIDouble { Units = this.SiteManager.Site.MassUnits, SIValue = 0.0 };
			if (this.CurrentWeight != null)
			{
				weight.Value = System.Convert.ToDouble(this.CurrentWeight.Value);
			}

			SIDouble volume = new SIDouble { Units = volumeUnits };

			SIDouble density = new SIDouble { Units = densityUnits };

			LineItemDO lineItem = this.CurrentLineItem;

			Vcf volumeCorrection = new Vcf();

			// Add the weight that will be contributed by other line items
			foreach (LineItemDO pendingLineItem in this.Transaction.LineItems)
			{
				if (lineItem == pendingLineItem)
				{
					continue;
				}

				if (pendingLineItem.PresetAmount == null)
				{
					continue;
				}

				if (pendingLineItem.Density == null)
				{
					continue;
				}

				// ReSharper disable once CompareOfFloatsByEqualityOperator
				if (pendingLineItem.Density.Value == 0.0)
				{
					continue;
				}

				volume.Value = pendingLineItem.PresetAmount.Value;

				density.Value = pendingLineItem.Density.Value;

				// density is the observed density if load by gross, standard density if load by net.
				// First get standard density.
				densityStd = new SIDouble
				{
					Units = product.DensityUnits == EngineeringUnit.FmSiteUnits ? this.SiteManager.Site.DensityUnits : product.DensityUnits,
					SIValue = this.SiteManager.Site.LoadByNet
															? density.SIValue
															: density.SIValue / pendingLineItem.VCF.Value
				};

				// Next get a vcf to change to density at 15 degrees C
				degrees15 = new SIDouble
				{
					Units = (EngineeringUnit)product._VcfModuleSettings.BaseTemperature.EngineeringUnitsType == EngineeringUnit.FmSiteUnits ? this.SiteManager.Site.TemperatureUnits : (EngineeringUnit)product._VcfModuleSettings.BaseTemperature.EngineeringUnitsType,
					SIValue = 15.0
				};


				volumeCorrection.VcfSettings.UseHydrometerCorrection = product._VcfModuleSettings.UseHydrometerCorrection;
				volumeCorrection.VcfSettings.ForceVcfTo4Digits = product._VcfModuleSettings.ForceVcfTo4Digits;

				vcf15 = volumeCorrection.VcfCalculation(
					 (ECorrectionTypeMajor)System.Convert.ToInt32(product._VcfModuleSettings.CorrectionMethodType),
					 (ECorrectionTypeMinor)System.Convert.ToInt32(product._VcfModuleSettings.CorrectionMethodSpecific),
					 degrees15.Value,
					 degrees15.Units,
					 product._VcfModuleSettings.BaseTemperature.Value,
					 (EngineeringUnit)product._VcfModuleSettings.BaseTemperature.EngineeringUnitsType == EngineeringUnit.FmSiteUnits ? this.SiteManager.Site.TemperatureUnits : (EngineeringUnit)product._VcfModuleSettings.BaseTemperature.EngineeringUnitsType,
					 densityStd.Value,
					 densityStd.Units,
					 0.0,
					 this.SiteManager.Site.PressureUnits,
					 product._VcfModuleSettings.AlternateTemperature.Value,
					 (EngineeringUnit)product._VcfModuleSettings.AlternateTemperature.EngineeringUnitsType == EngineeringUnit.FmSiteUnits ? this.SiteManager.Site.TemperatureUnits : (EngineeringUnit)product._VcfModuleSettings.AlternateTemperature.EngineeringUnitsType,
					 product._VcfModuleSettings.AlternateBasePressure.Value,
					 (EngineeringUnit)product._VcfModuleSettings.AlternateBasePressure.EngineeringUnitsType == EngineeringUnit.FmSiteUnits ? this.SiteManager.Site.PressureUnits : (EngineeringUnit)product._VcfModuleSettings.AlternateBasePressure.EngineeringUnitsType,
					 new[] { product.CorrectionFactor0, product.CorrectionFactor1, product.CorrectionFactor2, product.CorrectionFactor3, product.CorrectionFactor4 });

				//                product.CorrectionFactor.CorrectionFactorData);

				density15 = new SIDouble
				{
					Units = product.DensityUnits == EngineeringUnit.FmSiteUnits ? this.SiteManager.Site.DensityUnits : product.DensityUnits,
					SIValue = density.SIValue / vcf15
				};

				// this step is per API MPMS Ch. 11.5.3-2009
				density15Air = new SIDouble
				{
					Units = product.DensityUnits == EngineeringUnit.FmSiteUnits ? this.SiteManager.Site.DensityUnits : product.DensityUnits,
					SIValue = (1.000149926 * density15.SIValue) - 1.199407795
				};
				densityStdAir = new SIDouble
				{
					Units = product.DensityUnits == EngineeringUnit.FmSiteUnits ? this.SiteManager.Site.DensityUnits : product.DensityUnits,
					SIValue = density15Air.SIValue * vcf15
				};

				if (this.SiteManager.Site.LoadByNet)
				{
					weight.SIValue += volume.SIValue * densityStdAir.SIValue;
				}
				else
				{
					weight.SIValue += volume.SIValue * densityStdAir.SIValue / pendingLineItem.VCF.Value;
				}
			}

			Vcf volumeCorrrection = new Vcf();

			// Add the weight that will be contributed by other transactions
			foreach (TransactionDO pendingTransaction in this.PendingTransactions)
			{
				foreach (LineItemDO pendingLineItem in pendingTransaction.LineItems)
				{
					if (pendingLineItem.PresetAmount == null)
					{
						continue;
					}

					if (pendingLineItem.Density == null)
					{
						continue;
					}

					// ReSharper disable once CompareOfFloatsByEqualityOperator
					if (pendingLineItem.Density.Value == 0.0)
					{
						continue;
					}

					volume.Value = pendingLineItem.PresetAmount.Value;

					density.Value = pendingLineItem.Density.Value;

					// density is the observed density if load by gross, standard density if load by net.
					// First get standard density.
					densityStd = new SIDouble
					{
						Units = product.DensityUnits == EngineeringUnit.FmSiteUnits ? this.SiteManager.Site.DensityUnits : product.DensityUnits,
						SIValue =
							  this.SiteManager.Site.LoadByNet
									? density.SIValue
									: density.SIValue / pendingLineItem.VCF.Value
					};

					// Next get a vcf to change to density at 15 degrees C
					degrees15 = new SIDouble
					{
						Units = (EngineeringUnit)product._VcfModuleSettings.BaseTemperature.EngineeringUnitsType == EngineeringUnit.FmSiteUnits ? this.SiteManager.Site.TemperatureUnits : (EngineeringUnit)product._VcfModuleSettings.BaseTemperature.EngineeringUnitsType,
						SIValue = 15.0
					};

					volumeCorrection.VcfSettings.UseHydrometerCorrection = product._VcfModuleSettings.UseHydrometerCorrection;
					volumeCorrection.VcfSettings.ForceVcfTo4Digits = product._VcfModuleSettings.ForceVcfTo4Digits;

					vcf15 = volumeCorrection.VcfCalculation(
						 (ECorrectionTypeMajor)System.Convert.ToInt32(product._VcfModuleSettings.CorrectionMethodType),
						 (ECorrectionTypeMinor)System.Convert.ToInt32(product._VcfModuleSettings.CorrectionMethodSpecific),
						 degrees15.Value,
						 degrees15.Units,
						 product._VcfModuleSettings.BaseTemperature.Value,
						 (EngineeringUnit)product._VcfModuleSettings.BaseTemperature.EngineeringUnitsType == EngineeringUnit.FmSiteUnits ? this.SiteManager.Site.TemperatureUnits : (EngineeringUnit)product._VcfModuleSettings.BaseTemperature.EngineeringUnitsType,
						 densityStd.Value,
						 densityStd.Units,
						 0.0,
						 this.SiteManager.Site.PressureUnits,
						 product._VcfModuleSettings.AlternateTemperature.Value,
						 (EngineeringUnit)product._VcfModuleSettings.AlternateTemperature.EngineeringUnitsType == EngineeringUnit.FmSiteUnits ? this.SiteManager.Site.TemperatureUnits : (EngineeringUnit)product._VcfModuleSettings.AlternateTemperature.EngineeringUnitsType,
						 product._VcfModuleSettings.AlternateBasePressure.Value,
						 (EngineeringUnit)product._VcfModuleSettings.AlternateBasePressure.EngineeringUnitsType == EngineeringUnit.FmSiteUnits ? this.SiteManager.Site.PressureUnits : (EngineeringUnit)product._VcfModuleSettings.AlternateBasePressure.EngineeringUnitsType,
						 new[] { product.CorrectionFactor0, product.CorrectionFactor1, product.CorrectionFactor2, product.CorrectionFactor3, product.CorrectionFactor4 });
					density15 = new SIDouble
					{
						Units = product.DensityUnits,
						SIValue = density.SIValue / vcf15
					};

					// this step is per API MPMS Ch. 11.5.3-2009
					density15Air = new SIDouble
					{
						Units = product.DensityUnits == EngineeringUnit.FmSiteUnits ? this.SiteManager.Site.DensityUnits : product.DensityUnits,
						SIValue = (1.000149926 * density15.SIValue) - 1.199407795
					};
					densityStdAir = new SIDouble
					{
						Units = product.DensityUnits == EngineeringUnit.FmSiteUnits ? this.SiteManager.Site.DensityUnits : product.DensityUnits,
						SIValue = density15Air.SIValue * vcf15
					};

					if (this.SiteManager.Site.LoadByNet)
					{
						weight.SIValue += volume.SIValue * densityStdAir.SIValue;
					}
					else
					{
						weight.SIValue += volume.SIValue * densityStdAir.SIValue / pendingLineItem.VCF.Value;
					}
				}
			}

			density.Value = currentDensity.Value;

			densityStd = new SIDouble
			{
				Units = product.DensityUnits == EngineeringUnit.FmSiteUnits ? this.SiteManager.Site.DensityUnits : product.DensityUnits,
				SIValue = density.SIValue / currentVcf.Value
			};

			// Next get a vcf to change to density at 15 degrees C
			degrees15 = new SIDouble
			{
				Units = (EngineeringUnit)product._VcfModuleSettings.BaseTemperature.EngineeringUnitsType == EngineeringUnit.FmSiteUnits ? this.SiteManager.Site.TemperatureUnits : (EngineeringUnit)product._VcfModuleSettings.BaseTemperature.EngineeringUnitsType,
				SIValue = 15.0
			};

			volumeCorrection.VcfSettings.UseHydrometerCorrection = product._VcfModuleSettings.UseHydrometerCorrection;
			volumeCorrection.VcfSettings.ForceVcfTo4Digits = product._VcfModuleSettings.ForceVcfTo4Digits;

			vcf15 = volumeCorrection.VcfCalculation(
				 (ECorrectionTypeMajor)System.Convert.ToInt32(product._VcfModuleSettings.CorrectionMethodType),
				 (ECorrectionTypeMinor)System.Convert.ToInt32(product._VcfModuleSettings.CorrectionMethodSpecific),
				 degrees15.Value,
				 degrees15.Units,
				 product._VcfModuleSettings.BaseTemperature.Value,
				 (EngineeringUnit)product._VcfModuleSettings.BaseTemperature.EngineeringUnitsType == EngineeringUnit.FmSiteUnits ? this.SiteManager.Site.TemperatureUnits : (EngineeringUnit)product._VcfModuleSettings.BaseTemperature.EngineeringUnitsType,
				 densityStd.Value,
				 densityStd.Units,
				 0.0,
				 this.SiteManager.Site.PressureUnits,
				 product._VcfModuleSettings.AlternateTemperature.Value,
				 (EngineeringUnit)product._VcfModuleSettings.AlternateTemperature.EngineeringUnitsType == EngineeringUnit.FmSiteUnits ? this.SiteManager.Site.TemperatureUnits : (EngineeringUnit)product._VcfModuleSettings.AlternateTemperature.EngineeringUnitsType,
				 product._VcfModuleSettings.AlternateBasePressure.Value,
				 (EngineeringUnit)product._VcfModuleSettings.AlternateBasePressure.EngineeringUnitsType == EngineeringUnit.FmSiteUnits ? this.SiteManager.Site.PressureUnits : (EngineeringUnit)product._VcfModuleSettings.AlternateBasePressure.EngineeringUnitsType,
				 new[] { product.CorrectionFactor0, product.CorrectionFactor1, product.CorrectionFactor2, product.CorrectionFactor3, product.CorrectionFactor4 });
			density15 = new SIDouble
			{
				Units = product.DensityUnits == EngineeringUnit.FmSiteUnits ? this.SiteManager.Site.DensityUnits : product.DensityUnits,
				SIValue = density.SIValue * vcf15
			};

			// this step is per API MPMS Ch. 11.5.3-2009
			density15Air = new SIDouble
			{
				Units = product.DensityUnits == EngineeringUnit.FmSiteUnits ? this.SiteManager.Site.DensityUnits : product.DensityUnits,
				SIValue = (1.000149926 * density15.SIValue) - 1.199407795
			};
			densityStdAir = new SIDouble
			{
				Units = product.DensityUnits == EngineeringUnit.FmSiteUnits ? this.SiteManager.Site.DensityUnits : product.DensityUnits,
				SIValue = density15Air.SIValue / vcf15
			};
			SIDouble densityCurrentAir = new SIDouble { Units = volumeUnits, SIValue = densityStdAir.SIValue * currentVcf.Value };
			SIDouble capacity = new SIDouble
			{
				Units = volumeUnits,
				SIValue =
					  (this.SiteManager.Site._MaximumVehicleWeight.SIValue - weight.SIValue)
					  / densityCurrentAir.SIValue
			};

			// When getting maximum for a component the Capacity is factored by Blend Percentage
			if (component != null)
			{
				capacity.SIValue *= component.BlendPercentage / 100;
			}

			if (maximum.Value > capacity.Value)
			{
				maximum.Value = capacity.Value;
			}

			// Limit the Maximum to the Compartment Safe Fill adjusted by the currentVCF
			EquipmentClass compartment = this.CurrentEquipment.CompartmentCollection[this.CurrentCompartmentNumber - 1];

			SIDouble safeFill = new SIDouble { Units = volumeUnits, SIValue = compartment.SISafeFill.SIValue };

			// The following isn't exact, Blend Percentages probably are at Standard Temperature.
			// If products with different densities are blended away from standard temperature
			// then a correction would be necessary but at this point in this process
			// the densities for the other components are not available.
			if (component != null)
			{
				safeFill.SIValue *= component.BlendPercentage / 100;
			}

			if (this.SiteManager.Site.LoadByNet)
			{
				safeFill.SIValue *= currentVcf.Value;
			}

			if (maximum.Value > safeFill.Value)
			{
				maximum.Value = safeFill.Value;
			}

			// Limit the Maximum to the what remains on the order less
			// amounts committed on other line items.
			// Only apply this limit if the site is configured to
			if ((this.Order != null || this.SupplyOrder != null) && this.SiteManager.Site.EnforceSalesOrderLimit)
			{
				TransactionAliasClass orderTransactionAlias;
				TransactionDO order;
				if (this.Order != null)
				{
					orderTransactionAlias = this.OrderTransactionAlias;
					order = this.Order;
				}
				else
				{
					orderTransactionAlias = this.SupplyOrderTransactionAlias;
					order = this.SupplyOrder;
				}

				EngineeringUnit orderVolumeUnits = (orderTransactionAlias.VolumeUnits != EngineeringUnit.FmSiteUnits) ? orderTransactionAlias.VolumeUnits : this.SiteManager.Site.VolumeUnits;
				EngineeringUnit orderMassUnits = (orderTransactionAlias.MassUnits != EngineeringUnit.FmSiteUnits) ? orderTransactionAlias.MassUnits : this.SiteManager.Site.MassUnits;

				bool orderByGross = false;
				// ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
				foreach (TransactionAliasFieldClass field in orderTransactionAlias.LineItemFieldCollection)
				{
					if (field.DbName == "GrossQuantity")
					{
						orderByGross = true;
						break;
					}
				}

				if (orderByGross)
				{
					SIDouble grossQuantityRemaining = new SIDouble { Units = orderVolumeUnits };
					SIDouble massQuantityRemaining = new SIDouble { Units = orderMassUnits };

					LineItemDO activeOrderLineItem = null;
					//foreach (LineItemDO orderLineItem in this.Order.LineItems)
					foreach (LineItemDO orderLineItem in order.LineItems)
					{
						if (orderLineItem.Product == lineItem.Product)
						{
							grossQuantityRemaining.Value = orderLineItem.GrossQuantityRemaining;
							massQuantityRemaining.Value = orderLineItem.MassQuantityRemaining;
							activeOrderLineItem = orderLineItem;
							break;
						}
					}

					grossQuantityRemaining.Units = volumeUnits;
					massQuantityRemaining.Units = massUnits;

					// Adjust the Gross Quantity Remaining for existing line items
					foreach (LineItemDO pendingLineItem in this.Transaction.LineItems)
					{
						if (lineItem == pendingLineItem)
						{
							continue;
						}

						if (lineItem.Product != pendingLineItem.Product)
						{
							continue;
						}

						if (pendingLineItem.PresetAmount != null)
						{
							if (this.SiteManager.Site.LoadByNet)
							{
								grossQuantityRemaining.Value -= pendingLineItem.PresetAmount.Value / pendingLineItem.VCF.Value;
							}
							else
							{
								grossQuantityRemaining.Value -= pendingLineItem.PresetAmount.Value;
							}

							massQuantityRemaining.Value -= pendingLineItem.PresetAmount.Value; // Preset may be volume or may be mass; we'll update bothe now and figure out which to use later
						}
					}

					// Adjust the Gross Quantity Remaining for existing transactions against Order

					// When getting Maximum for a Component Gross Remaining is factored by Blend Percentage
					if (component != null)
					{
						grossQuantityRemaining.Value *= component.BlendPercentage / 100;
						massQuantityRemaining.Value *= component.BlendPercentage / 100;
					}

					// ReSharper disable CompareOfFloatsByEqualityOperator
					// If order does not have a gross quantity (it is 0.0) and does have a mass quantity, assume the order is by mass.
					// otherwise, assume by gross
					if (activeOrderLineItem?.Quantity != null
						 && activeOrderLineItem.Quantity.Gross == 0.0
						 && activeOrderLineItem.Quantity.Mass > 0.0)
					{
						if (maximum.Value > massQuantityRemaining.Value)
						{
							maximum.Value = massQuantityRemaining.Value;
						}
					}
					else
					{
						if (this.SiteManager.Site.LoadByNet)
						{
							if (maximum.Value > grossQuantityRemaining.Value * lineItem.VCF.Value)
							{
								maximum.Value = grossQuantityRemaining.Value * lineItem.VCF.Value;
							}
						}
						else
						{
							if (maximum.Value > grossQuantityRemaining.Value)
							{
								maximum.Value = grossQuantityRemaining.Value;
							}
						}
					}
					// ReSharper restore CompareOfFloatsByEqualityOperator
				}
				else
				{
					SIDouble netQuantityRemaining = new SIDouble { Units = orderVolumeUnits };
					SIDouble massQuantityRemaining = new SIDouble { Units = orderMassUnits };

					LineItemDO activeOrderLineItem = null;
					//foreach (LineItemDO orderLineItem in this.Order.LineItems)
					foreach (LineItemDO orderLineItem in order.LineItems)
					{
						if (orderLineItem.Product == lineItem.Product)
						{
							netQuantityRemaining.Value = orderLineItem.NetQuantityRemaining;
							massQuantityRemaining.Value = orderLineItem.MassQuantityRemaining;
							activeOrderLineItem = orderLineItem;
							break;
						}
					}

					netQuantityRemaining.Units = volumeUnits;
					massQuantityRemaining.Units = massUnits;

					// Adjust the NetQuantity Remaining for existing line items
					foreach (LineItemDO pendingLineItem in this.Transaction.LineItems)
					{
						if (lineItem == pendingLineItem)
						{
							continue;
						}

						if (lineItem.Product != pendingLineItem.Product)
						{
							continue;
						}

						if (pendingLineItem.PresetAmount != null)
						{
							if (this.SiteManager.Site.LoadByNet)
							{
								netQuantityRemaining.Value -= pendingLineItem.PresetAmount.Value;
							}
							else
							{
								netQuantityRemaining.Value -= pendingLineItem.PresetAmount.Value * pendingLineItem.VCF.Value;
							}

							massQuantityRemaining.Value -= pendingLineItem.PresetAmount.Value; // Preset may be volume or may be mass; we'll update bothe now and figure out which to use later							
						}
					}

					// When getting Maximum for a Component Net Remaining is factored by Blend Percentage
					if (component != null)
					{
						netQuantityRemaining.Value *= component.BlendPercentage / 100;
						massQuantityRemaining.Value *= component.BlendPercentage / 100;
					}

					// ReSharper disable CompareOfFloatsByEqualityOperator
					// If order does not have a net quantity (it is 0.0) and does have a mass quantity, assume the order is by mass.
					// otherwise, assume by gross
					if (activeOrderLineItem?.Quantity != null
						 && activeOrderLineItem.Quantity.Net == 0.0
						 && activeOrderLineItem.Quantity.Mass > 0.0)
					{
						if (maximum.Value > massQuantityRemaining.Value)
						{
							maximum.Value = massQuantityRemaining.Value;
						}
					}
					else
					{
						if (this.SiteManager.Site.LoadByNet)
						{
							if (maximum.Value > netQuantityRemaining.Value)
							{
								maximum.Value = netQuantityRemaining.Value;
							}
						}
						else
						{
							if (maximum.Value > netQuantityRemaining.Value / lineItem.VCF.Value)
							{
								maximum.Value = netQuantityRemaining.Value / lineItem.VCF.Value;
							}
						}
					}
					// ReSharper restore CompareOfFloatsByEqualityOperator
				}
			}

			return maximum.Value;
		}

		protected bool GetOrderList()
		{
			bool AllPreloadsAssociatedWithOrders = true;

			GetTransactionTypeSR SR = new GetTransactionTypeSR
			{
				Security = this.Security
			};

			this.OrderList = new ArrayList();

			foreach (DataRow Row in this.PreloadDataSet.Tables[0].Rows)
			{
				// First, the row needs a valid TransReferenceID
				string TransReferenceID = Row["TransReferenceID"] as string;

				if (TransReferenceID == null || TransReferenceID == "")
				{
					AllPreloadsAssociatedWithOrders = false;
					continue;
				}

				// Look up the transaction and see if the associated transaction is an Order type.
				SR.TransID = TransReferenceID;

				GetTransactionTypeDO TransTypeDO = this.ProcessGetTransactionTypeDO(SR);

				// If we did not find the referenced transaction or it is not an Order type, we cannot
				// automatically prompt by Order Number.
				if (TransTypeDO != null && TransTypeDO.TransType != TransactionTypes.T17_Order)
				{
					AllPreloadsAssociatedWithOrders = false;
					continue;
				}

				// Save the transaction for later
				this.AddNonDuplicateOrder(TransTypeDO);
			}

			this.OrderList.Sort(new OrderComparer());

			return AllPreloadsAssociatedWithOrders;
		}

		private GetTransactionTypeDO ProcessGetTransactionTypeDO(GetTransactionTypeSR serviceRequest)
		{
			return FMChannelHelper.MakeCall<IGetTransactionTypeProcessor, GetTransactionTypeDO>(
																	 x =>
																	 x.Process(serviceRequest)
																);
		}

		protected OrderListDO GetOrders()
		{
			DateTimeOffset siteTimeNow = TimeConverter.Now(this.SiteManager.Site);

			OrderListSR orderListSR = new OrderListSR
			{
				Security = this.Security,
				SubRequest = OrderListSR.RequestTypes.GET_DETAIL
			};
			orderListSR.Criteria.Security = this.Security;
			orderListSR.Criteria.DateFilterType = OrderListFilterCriteria.OrderDateFilterType.DET;
			orderListSR.Criteria.StartDate = siteTimeNow;
			orderListSR.Criteria.EndDate = siteTimeNow;
			orderListSR.Criteria.Status = ((int)TransactionStatus.Scheduled).ToString();
			orderListSR.Criteria.SortExpression = "ScheduledDate, EffectiveDate";

			return FMChannelHelper.MakeCall<IOrderListProcessor, OrderListDO>(
																	 x =>
																	 x.Process(orderListSR)
																);
		}

		protected IQualityAssurance GetQualityAssuranceInterface()
		{
			IQualityAssurance QAObject = null;

			try
			{
				string QAInterface = "";

				string qualAssurIF = FMChannelHelper.MakeCall<IConfigurationSettings, string>(
																	 x =>
																	 x.GetKeyValueByKey(this.Security, ConfigurationSettingDOClass.Key_LR_QualityAssuranceInterface)
																);
				if (string.IsNullOrEmpty(qualAssurIF) == false)
				{
					if (!typeof(string).IsInstanceOfType(qualAssurIF))
					{
						QAInterface = "";
					}
				}

				if (string.IsNullOrEmpty(QAInterface) == false)
				{
					if(QAAssembly == null)
					{
						QAAssembly = Assembly.LoadFrom(QAInterface);
					}
					if (QAAssembly != null)
					{
						Type[] Types = QAAssembly.GetTypes();

						foreach (Type Module in Types)
						{
							Type QAInterfaceType = Module.GetInterface("IQualityAssurance");

							if (QAInterfaceType != null)
							{
								QAObject = (IQualityAssurance)Activator.CreateInstance(Module);
								break;
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				this.eventLog.WriteEntry("StationManager GetQualityAssuranceInterface : " + e.Message);
			}

			return QAObject;
		}

		protected SupplyOrderListDO GetSupplyOrders()
		{
			DateTimeOffset siteTimeNow = TimeConverter.Now(this.SiteManager.Site);

			SupplyOrderListSR sr = new SupplyOrderListSR
			{
				Security = this.Security,
				SubRequest = SupplyOrderListSR.RequestTypes.GET_DETAIL,
				Criteria =
							  {
									Security = this.Security,
									DateFilterType = SupplyOrderListFilterCriteria.SupplyOrderDateFilterType.NONE,
									StartDate = siteTimeNow,
									EndDate = siteTimeNow,
									Status = ((int)TransactionStatus.Scheduled).ToString()
							  }
			};

			return FMChannelHelper.MakeCall<ISupplyOrderListProcessor, SupplyOrderListDO>(
																	x =>
																	x.Process(sr)
															  );
		}

		protected double GetTankValue(TankClass tank, PROCESS_VARIABLE_TYPE type)
		{
			if (tank == null)
			{
				throw new Exception("Invalid Tank");
			}

			ProcessVariableClass PV = tank.ProcessVariableCollection[type];
			if (PV == null)
			{
				throw new Exception("Invalid Process Variable");
			}

			if (!this.SiteManager.Site.UseLastKnownGoodTankData && !PV.IsQualityGood)
			{
				throw new Exception("Tank " + tank.ID + " " + type.ToString() + " Bad Quality");
			}

			try
			{
				return System.Convert.ToDouble(PV.SIValue);
			}
			catch
			{
				throw new Exception("Tank " + tank.ID + " " + type.ToString() + " Bad Data");
			}
		}

		protected TransactionDO GetTransaction(string TransID)
		{
			TransactionSR transactionSR = new TransactionSR
			{
				Security = this.Security,
				TransID = TransID
			};

			return FMChannelHelper.MakeCall<ITransactionProcessor, TransactionDO>(
																	 x =>
																	 x.Process(transactionSR)
																);
		}

		protected string[] HeirarchicalCompanyList()
		{
			ArrayList CompaniesList = new ArrayList();

			this.HierarchyCompanyCollection.Clear();

			if (this.Station.Type == STATION_TYPE.OFF_LOADING)
			{
				foreach (CompanyMapClass CompanyMap in this.CompanyMapCollection)
				{
					int ManagerDelimiter = CompanyMap.AssignedToID.IndexOf("->");
					string ManagerID = CompanyMap.AssignedToID.Substring(0, ManagerDelimiter);
					// the off load is returned manager->owner so the ownerdelimiter is manager + 2
					int OwnerDelimiter = CompanyMap.AssignedToID.Length - (ManagerDelimiter + 2);
					string OwnerID = CompanyMap.AssignedToID.Substring(ManagerDelimiter + 2, OwnerDelimiter);

					string CompanyID = null;
					switch (this.CurrentCompanyHierarchyType)
					{
						case COMPANY_MAP_TYPE.SUPPLIER_OWNER_MAP:
							CompanyID = OwnerID;
							break;

						case COMPANY_MAP_TYPE.OFFLOAD_OWNER_MANAGER_MAP:
							if (OwnerID != this.Owner.ID)
							{
								continue;
							}
							CompanyID = ManagerID;
							break;

						default:
							throw new Exception("Invalid Company Map Type");
					}
					Guid guid = this.GetCompanyGuidByCompanyId(this.Security, CompanyID);
					CompanyClass company = this.GetCompany(this.Security, guid);

					if (CompaniesList.IndexOf(company.CompanyToolTip) != -1)
					{
						continue;
					}

					this.HierarchyCompanyCollection.Add(company);
					CompaniesList.Add(company.CompanyToolTip);
				}
			}
			else
			{
				foreach (CompanyMapClass CompanyMap in this.CompanyMapCollection)
				{
					int ManagerDelimiter = CompanyMap.AssignedToID.IndexOf("->");
					string ManagerID = CompanyMap.AssignedToID.Substring(0, ManagerDelimiter);
					int OwnerDelimiter = CompanyMap.AssignedToID.IndexOf("->", ManagerDelimiter + 2);
					string OwnerID = CompanyMap.AssignedToID.Substring(ManagerDelimiter + 2, OwnerDelimiter - ManagerDelimiter - 2);
					int ShipperDelimiter = CompanyMap.AssignedToID.IndexOf("->", OwnerDelimiter + 2);
					string ShipperID = CompanyMap.AssignedToID.Substring(OwnerDelimiter + 2, ShipperDelimiter - OwnerDelimiter - 2);
					string BillToID = CompanyMap.AssignedToID.Substring(ShipperDelimiter + 2);

					string CompanyID = null;
					switch (this.CurrentCompanyHierarchyType)
					{
						case COMPANY_MAP_TYPE.SHIPTO_BILLTO_MAP:
							CompanyID = BillToID;
							break;

						case COMPANY_MAP_TYPE.BILLTO_SHIPPER_MAP:
							if (BillToID != this.BillTo.ID)
							{
								continue;
							}
							CompanyID = ShipperID;
							break;

						case COMPANY_MAP_TYPE.SHIPPER_OWNER_MAP:
							if (BillToID != this.BillTo.ID || ShipperID != this.Shipper.ID)
							{
								continue;
							}
							CompanyID = OwnerID;
							break;

						case COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP:
							if (BillToID != this.BillTo.ID || ShipperID != this.Shipper.ID || OwnerID != this.Owner.ID)
							{
								continue;
							}
							CompanyID = ManagerID;
							break;

						default:
							throw new Exception("Invalid Company Map Type");
					}
					Guid guid = this.GetCompanyGuidByCompanyId(this.Security, CompanyID);
					CompanyClass Company = this.GetCompany(this.Security, guid);

					if (CompaniesList.IndexOf(Company.CompanyToolTip) != -1)
					{
						continue;
					}

					this.HierarchyCompanyCollection.Add(Company);
					CompaniesList.Add(Company.CompanyToolTip);
				}
			}

			CompaniesList.Sort();

			return (string[])CompaniesList.ToArray(typeof(string));
		}

		private Guid GetCompanyGuidByCompanyId(SecurityClass securityClass, string CompanyID)
		{
			return FMChannelHelper.MakeCall<ICompanies, Guid>(
																	 x =>
																	 x.GetIdentityGuid(securityClass, CompanyID)
																);
		}

		protected bool InEquipmentList(ArrayList EquipmentList, string CheckID)
		{
			foreach (string ID in EquipmentList)
			{
				if (ID == CheckID)
				{
					return true;
				}
			}

			return false;
		}

		protected bool IsEquipmentValidForThisStation(EquipmentClass EquipmentToCheck)
		{
			foreach (QualificationMapClass ReqTestOrInspection in this.Station.ReqTestsandInspectionsCollection)
			{
				bool bTestAndInspectionsOK = false;
				foreach (QualificationMapClass TestOrInspection in EquipmentToCheck.TestAndInspectionCollection)
				{
					if (ReqTestOrInspection.AssignedGuid == TestOrInspection.AssignedGuid)
					{
						bTestAndInspectionsOK = true;
						break;
					}
				}

				if (bTestAndInspectionsOK == false)
				{
					this.AddAlarmAndEventLogs(
						this.Security, this.Station.EquipmentNotAuthorizedEvent(EquipmentToCheck.ID, ReqTestOrInspection.ID));
					this.DisplayMessage("[LoadRack|Equipment] [LoadRack|Not Authorized]", null, 0, this.MESSAGE_TIMEOUT);
					this.StationState = StationState.RESET_ON_TIMEOUT;
					return false;
				}
			}

			return true;
		}

		//this function will check if the station required license tags and licenses are attached to the equipment 
		protected bool IsEquipmentLicenseValidForThisStation(EquipmentClass EquipmentToCheck)
		{
			foreach (QualificationMapClass ReqEquipmentTagAndLicense in this.Station.ReqEquipmentTagAndLicenseCollection)
			{
				bool bEquipmentTagAndLicenseOK = false;
				foreach (QualificationMapClass TagAndLicense in EquipmentToCheck.TagAndLicenseCollection)
				{
					if (ReqEquipmentTagAndLicense.AssignedGuid == TagAndLicense.AssignedGuid)
					{
						bEquipmentTagAndLicenseOK = true;
						break;
					}
				}

				if (bEquipmentTagAndLicenseOK == false)
				{
					this.AddAlarmAndEventLogs(
						 this.Security, this.Station.EquipmentNotAuthorizedEvent(EquipmentToCheck.ID, ReqEquipmentTagAndLicense.ID));
					this.DisplayMessage("[LoadRack|Equipment] [LoadRack|Not Licensed]", null, 0, this.MESSAGE_TIMEOUT);
					this.StationState = StationState.RESET_ON_TIMEOUT;
					return false;
				}
			}

			return true;
		}

		protected bool IsLoadablePreloadLineItem(ProductClass Product, LineItemDO LineItem)
		{
			// The volume on the line item needs to be valid and zero
			if (LineItem.Quantity == null
				 || (LineItem.Quantity.GrossInventoryChange == 0 && LineItem.Quantity.NetInventoryChange == 0
					  && LineItem.Quantity.MassInventoryChange == 0))
			{
				// Don't allow zero preset value for preload line items; otherwise, the driver
				// could load any amount at a Smith Meter preset.
				if (LineItem.PresetAmount != null && LineItem.PresetAmount.Value != 0.0)
				{
					return true;
				}
			}

			// Check for incomplete splash blend - Only need to check blend products
			if (Product.ProductType == ProductType.BlendProduct)
			{
				// If there is a sub line item for each component, this line item is complete; otherwise,
				// there are still items to load and we should authorize the line.
				int LineCount = 0;

				foreach (ProductMapClass Component in Product.ComponentCollection)
				{
					foreach (SubLineItemDO SubLineItem in LineItem.SubLineItems)
					{
						if (SubLineItem.ProductGuid != Guid.Empty && SubLineItem.ProductGuid == Component.AssignedGuid)
						{
							if (SubLineItem.Status != TransactionStatus.Completed && SubLineItem.Quantity != null
								 && (SubLineItem.Quantity.GrossInventoryChange != 0 || SubLineItem.Quantity.NetInventoryChange != 0
									  || SubLineItem.Quantity.MassInventoryChange != 0))
							{
								++LineCount;
							}
						}
					}

					// If we did not find a sublineitem for each component, we should continue with authorizing this line
					return LineCount != Product.ComponentCollection.Count;
				}
			}

			return false;
		}

		internal bool IsProductAvailable(AdditiveProfileClass additiveProfile, ProductMapClass product)
		{
			// Look for Load Arm through which product is available
			foreach (LoadArmManagerClass loadArmManager in this.LoadArmManagerCollection)
			{
				if (!loadArmManager.LoadArm.IsProductAvailable(FMChannelHelper.MakeCall<IProducts, Guid>(x => x.GetMasterRecordGuid(this.Security, product.AssignedGuid))))
				{
					continue;
				}

				if (additiveProfile != null)
				{
					if (!loadArmManager.LoadArm.IsAdditiveProfileAvailable(additiveProfile))
					{
						continue;
					}
				}
				else
				{
					if (!loadArmManager.LoadArm.NoAdditivePermissives.Permitted)
					{
						continue;
					}
				}

				return true;
			}

			return false;
		}

		protected bool IsProductOpenOnOrder(Guid productGuid)
		{
			// productGuid passed in is from ship-to authorized products, which is
			// a child-record to child-record mapping.  
			// Orders, however, have master record guids.
			ProductClass product = FMChannelHelper.MakeCall<IProducts, ProductClass>(x => x.GetMinimalProductData(this.Security, productGuid));

			foreach (LineItemDO lineItem in this.Order.LineItems)
			{
				if (lineItem.ProductGuid == product.MasterRecordGuid && lineItem.NetQuantityRemaining > 0.0)
				{
					return true;
				}
			}

			return false;
		}

		protected bool IsProductOpenOnSupplyOrder(Guid productMasterRecordGuid)
		{
			foreach (LineItemDO supplyOrderLineItem in this.SupplyOrder.LineItems)
			{
				if (supplyOrderLineItem.ProductGuid != productMasterRecordGuid)
				{
					continue;
				}

				if (supplyOrderLineItem.Status == TransactionStatus.Completed)
				{
					continue;
				}

				// Open Order
				if (this.SiteManager.Site.LoadByNet)
				{
					if (supplyOrderLineItem.NetQuantityRemaining > 0.0)
					{
						return true;
					}
				}
				else
				{
					if (supplyOrderLineItem.GrossQuantityRemaining > 0.0)
					{
						return true;
					}
				}

				// Check if there is remaining mass on the order 
				// if and only if there is no net volume (net volume = 0.0) on the order (that's our flag that we're doing this by mass) AND
				// we do have a mass quantity.
				if (supplyOrderLineItem.Quantity.Net == 0 && supplyOrderLineItem.Quantity.Mass > 0)
				{
					return true;
				}
			}

			return false;
		}

		protected bool IsProductOpenOnTransaction(Guid productGuid)
		{
			foreach (LineItemDO lineItem in this.Transaction.LineItems)
			{
				if (lineItem.ProductGuid == FMChannelHelper.MakeCall<IProducts, Guid>(x => x.GetMasterRecordGuid(this.Security, productGuid)) 
					&& lineItem.Status != TransactionStatus.Completed)
				{
					return true;
				}
			}

			return false;
		}

		protected bool IsProductPermissed(ProductMapClass product)
		{
			// Look for Load Arm through which product is available
			foreach (LoadArmManagerClass loadArmManager in this.LoadArmManagerCollection)
			{
				// productGuid passed in is from ship-to authorized products, which is
				// a child-record to child-record mapping.  
				// Preloads, however, have master record guids.
				ProductMapClass recipe = loadArmManager.AvailableRecipeCollection.Find(x => x.IdentityGuid == product.AssignedGuid);
				if (recipe == null)
				{
					continue;
				}

				if (!recipe.EnableRecipe)
				{
					continue;
				}

				if (!recipe.Permissives.Permitted)
				{
					continue;
				}

				return true;
			}

			return false;
		}

		private bool IsProductClosedOut(ProductMapClass productMap)
		{
			CloseoutListSR closeoutListSR;
			CloseoutListDO closeoutListDO;
			DateTime lastCloseoutDate;
			InventoryDateDO inventoryDateDO;

			// Get current inventory date
			InventoryDateSR inventoryDateSR = new InventoryDateSR()
			{
				Security = this.Security,
				CurrentSiteGuid = this.SiteManager.Site.SiteGuid
			};

			inventoryDateDO =
				 FMChannelHelper.MakeCall<IInventoryDateProcessor, InventoryDateDO>(x => x.Process(inventoryDateSR));

			// Get current product
			ProductClass product = this.GetProductMinimalInfo(this.Security, productMap.AssignedGuid);
			if (this.productClosedOutCache.ContainsKey(productMap.AssignedGuid))
			{
				if (this.productClosedOutCache[productMap.AssignedGuid])
				{
					return true;
				}
			}
			else
			{
				// Get latest closeout date for the current product
				closeoutListSR = new CloseoutListSR()
				{
					ConvertUnits = true,
					CurrentSiteGuid = this.SiteManager.Site.SiteGuid,
					EndDate = null,
					GetPreviousAndSubsequentCloseouts = false,
					ManagerGuid = this.Manager.IdentityGuid,
					ProductGuid = productMap.AssignedGuid,
					ProductType = product.ProductType,
					Security = this.Security,
					Site = this.SiteManager.Site.SiteID,
					StartDate = null
				};

				closeoutListDO =
					FMChannelHelper.MakeCall<ICloseoutListProcessor, CloseoutListDO>(x => x.Process(closeoutListSR));

				if (closeoutListDO.CloseoutList.Count > 0)
				{
					lastCloseoutDate = ((CloseoutDO)closeoutListDO.CloseoutList[0]).CloseoutDate.Date;
					if (lastCloseoutDate >= inventoryDateDO.InventoryDate)
					{
						this.productClosedOutCache[productMap.AssignedGuid] = true;
						return true;
					}
					else
					{
						this.productClosedOutCache[productMap.AssignedGuid] = false;
					}
				}
				else
				{
					this.productClosedOutCache[productMap.AssignedGuid] = false;
				}
			}

			// If we're a blend, check the components
			if (product.ProductType == ProductType.BlendProduct)
			{
				ProductMapCollectionClass componentCollection = this.EnumerateByAssignedToGuidAndType(
																																	 this.Security,
																																	 product.MasterRecordGuid,
																																	 PRODUCT_MAP_TYPE.BLEND_COMPONENT_MAP);

				foreach (ProductMapClass component in componentCollection)
				{
					if (this.productClosedOutCache.ContainsKey(component.AssignedGuid))
					{
						if (this.productClosedOutCache[component.AssignedGuid])
						{
							return true;
						}
					}
					else
					{
						// Get latest closeout date for the current product
						closeoutListSR = new CloseoutListSR()
						{
							ConvertUnits = true,
							CurrentSiteGuid = this.SiteManager.Site.SiteGuid,
							EndDate = null,
							GetPreviousAndSubsequentCloseouts = false,
							ManagerGuid = this.Manager.IdentityGuid,
							ProductGuid = component.AssignedGuid,
							ProductType = ProductType.ComponentProduct,
							Security = this.Security,
							Site = this.SiteManager.Site.SiteID,
							StartDate = null
						};

						closeoutListDO =
							 FMChannelHelper.MakeCall<ICloseoutListProcessor, CloseoutListDO>(x => x.Process(closeoutListSR));

						if (closeoutListDO.CloseoutList.Count > 0)
						{
							lastCloseoutDate = ((CloseoutDO)closeoutListDO.CloseoutList[0]).CloseoutDate.Date;
							if (lastCloseoutDate >= inventoryDateDO.InventoryDate)
							{
								this.productClosedOutCache[component.AssignedGuid] = true;
								return true;
							}
							else
							{
								this.productClosedOutCache[component.AssignedGuid] = false;
							}
						}
						else
						{
							this.productClosedOutCache[component.AssignedGuid] = false;
						}
					}
				}
			}

			// Now check additives
			AdditiveProfileClass additiveProfile = null;
			if (productMap.AdditiveProfileGuid != Guid.Empty)
			{
				additiveProfile = this.GetAdditiveProfiles(this.Security, productMap.AdditiveProfileGuid);
			}

			if (additiveProfile != null)
			{
				foreach (ProductMapClass additive in additiveProfile.AdditiveCollection)
				{
					if (this.productClosedOutCache.ContainsKey(additive.AssignedGuid))
					{
						if (this.productClosedOutCache[additive.AssignedGuid])
						{
							return true;
						}
					}
					else
					{
						// Get latest closeout date for the current product
						closeoutListSR = new CloseoutListSR()
						{
							ConvertUnits = true,
							CurrentSiteGuid = this.SiteManager.Site.SiteGuid,
							EndDate = null,
							GetPreviousAndSubsequentCloseouts = false,
							ManagerGuid = this.Manager.IdentityGuid,
							ProductGuid = additive.AssignedGuid,
							ProductType = ProductType.AdditiveProduct,
							Security = this.Security,
							Site = this.SiteManager.Site.SiteID,
							StartDate = null
						};

						closeoutListDO =
							 FMChannelHelper.MakeCall<ICloseoutListProcessor, CloseoutListDO>(x => x.Process(closeoutListSR));

						if (closeoutListDO.CloseoutList.Count > 0)
						{
							lastCloseoutDate = ((CloseoutDO)closeoutListDO.CloseoutList[0]).CloseoutDate.Date;
							if (lastCloseoutDate >= inventoryDateDO.InventoryDate)
							{
								this.productClosedOutCache[additive.AssignedGuid] = true;
								return true;
							}
							else
							{
								this.productClosedOutCache[additive.AssignedGuid] = false;
							}
						}
						else
						{
							this.productClosedOutCache[additive.AssignedGuid] = false;
						}
					}
				}
			}

			return false;
		}

		protected void IssueAdditionalOrdersPrompt()
		{
			this.StationState = StationState.ADDITIONAL_ORDERS_PROMPT;
			DisplayMenuParameters parameters = new DisplayMenuParameters(
				"LoadRack|Additional Orders?", new[] { "LoadRack|Yes", "LoadRack|No" }, true, -1, this.PROMPT_TIMEOUT);
			this.DisplayMenu(parameters);
		}

		protected void IssueCancelTransactionMenu()
		{
			// Build initial menu parameter set
			DisplayMenuParameters parameters = new DisplayMenuParameters
			{
				ApplyDataDictionary = true,
				DefaultItem = 0,
				MenuTimeout = this.PROMPT_TIMEOUT,
				SaveForCancelProcessing = false,
				Caption = "LoadRack|Cancel?"
			};


			ArrayList menu = new ArrayList { "LoadRack|Yes", "LoadRack|No" };


			// Save last station state
			this.PriorStationState = this.StationState;

			this.StationState = StationState.CANCEL_TRANSACTION_PROMPT;

			parameters.Menu = (string[])menu.ToArray(typeof(string));

			this.DisplayMenu(parameters);
		}

		[SecurityCritical]
		protected void IssueCaptureExitWeightPrompt(bool Acknowledgement)
		{
			bool WeightScaleInMotion = false;
			bool WeightScaleMotionReadingInValid = false;
			this.ReadWeight(out this.CurrentWeight, out WeightScaleInMotion, out WeightScaleMotionReadingInValid);
			string Prompt;
			string[] Menu;
			int DefaultItem = -1;
			if (this.CurrentWeight.Quality == Quality.Good && WeightScaleInMotion == false
				 && WeightScaleMotionReadingInValid == false)
			{
				double currentWeight = System.Convert.ToDouble(this.CurrentWeight.Value);

				// Check for Excess Weight
				if (!Acknowledgement && currentWeight > this.SiteManager.Site._MaximumVehicleWeight.Value)
				{
					string ExcessWeight = (currentWeight - this.SiteManager.Site._MaximumVehicleWeight.Value).ToString(
						"N", this.SiteManager.Site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.MASS));

					string Equipment = "";

					if (this.TractorOrTanker != null)
					{
						Equipment = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
								x =>
								x.Get(this.SiteManager.Site.IdentityGuid, EquipmentTypeClass.TypeID(this.TractorOrTanker.Type)))
										+ " " + this.TractorOrTanker.ID;
					}

					if (this.Trailer1 != null)
					{
						if (string.IsNullOrEmpty(Equipment) == false)
						{
							Equipment += " ";
						}

						Equipment += FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
											x =>
											x.Get(this.SiteManager.Site.IdentityGuid, EquipmentTypeClass.TypeID(this.Trailer1.Type)))
										+ " " + this.Trailer1.ID;
					}

					if (this.Trailer2 != null)
					{
						Equipment += " " + this.Trailer2.ID;
					}

					this.AddAlarmAndEventLogs(this.Security, this.Station.ExcessVehicleWeightAlarm(Equipment, ExcessWeight));

					string AbbrevString = EngineeringUnits.GetUnitAbbreviation(this.SiteManager.Site.MassUnits);

					this.StationState = StationState.OVERWEIGHT_MSG;
					this.DisplayMessageWithAcknowledge(
						"[LoadRack|Maximum Vehicle Weight] " + this.SiteManager.Site.MaximumVehicleWeight + " " + AbbrevString
						+ " [LoadRack|Exceeded by] " + ExcessWeight + " " + AbbrevString);
					return;
				}

				double Value = System.Convert.ToDouble(this.CurrentWeight.Value);
				Prompt = "[LoadRack|Exit Weight]" + " "
							+ Value.ToString("N", this.SiteManager.Site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.MASS));
				Menu = new[] { "LoadRack|Accept", "LoadRack|Refresh Scale Reading" };
				DefaultItem = 0;
			}
			else if (this.CurrentWeight.Quality == Quality.Good && WeightScaleInMotion && WeightScaleMotionReadingInValid == false)
			{
				double currentWeight = System.Convert.ToDouble(this.CurrentWeight.Value);

				// Check for Excess Weight
				if (!Acknowledgement && currentWeight > this.SiteManager.Site._MaximumVehicleWeight.Value)
				{
					string ExcessWeight = (currentWeight - this.SiteManager.Site._MaximumVehicleWeight.Value).ToString(
						"N", this.SiteManager.Site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.MASS));

					string Equipment = "";

					if (this.TractorOrTanker != null)
					{
						Equipment = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(this.SiteManager.Site.IdentityGuid, EquipmentTypeClass.TypeID(this.TractorOrTanker.Type)))

										+ " " + this.TractorOrTanker.ID;
					}

					if (this.Trailer1 != null)
					{
						if (string.IsNullOrEmpty(Equipment) == false)
						{
							Equipment += " ";
						}

						Equipment += FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(this.SiteManager.Site.IdentityGuid, EquipmentTypeClass.TypeID(this.Trailer1.Type)))
											+ " " + this.Trailer1.ID;
					}

					if (this.Trailer2 != null)
					{
						Equipment += " " + this.Trailer2.ID;
					}

					this.AddAlarmAndEventLogs(this.Security, this.Station.ExcessVehicleWeightAlarm(Equipment, ExcessWeight));

					string AbbrevString = EngineeringUnits.GetUnitAbbreviation(this.SiteManager.Site.MassUnits);

					this.StationState = StationState.OVERWEIGHT_MSG;
					this.DisplayMessageWithAcknowledge(
						"[LoadRack|Maximum Vehicle Weight] " + this.SiteManager.Site.MaximumVehicleWeight + " " + AbbrevString
						+ " [LoadRack|Exceeded by] " + ExcessWeight + " " + AbbrevString);
					return;
				}

				double Value = System.Convert.ToDouble(this.CurrentWeight.Value);
				Prompt = "[LoadRack|Weight Scale In Motion. Exit Weight]" + " "
							+ Value.ToString("N", this.SiteManager.Site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.MASS));
				Menu = new[] { "LoadRack|Accept", "LoadRack|Refresh Scale Reading" };
				DefaultItem = 0;
			}
			else if (this.CurrentWeight.Quality == Quality.Good && WeightScaleMotionReadingInValid)
			{
				double currentWeight = System.Convert.ToDouble(this.CurrentWeight.Value);

				// Check for Excess Weight
				if (!Acknowledgement && currentWeight > this.SiteManager.Site._MaximumVehicleWeight.Value)
				{
					string ExcessWeight = (currentWeight - this.SiteManager.Site._MaximumVehicleWeight.Value).ToString(
						"N", this.SiteManager.Site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.MASS));

					string Equipment = "";

					if (this.TractorOrTanker != null)
					{
						Equipment = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
										x =>
										x.Get(this.SiteManager.Site.IdentityGuid, EquipmentTypeClass.TypeID(this.TractorOrTanker.Type)))
										+ " " + this.TractorOrTanker.ID;
					}

					if (this.Trailer1 != null)
					{
						if (Equipment != "")
						{
							Equipment += " ";
						}

						Equipment += FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
											x =>
											x.Get(this.SiteManager.Site.IdentityGuid, EquipmentTypeClass.TypeID(this.Trailer1.Type)))
											+ " " + this.Trailer1.ID;
					}

					if (this.Trailer2 != null)
					{
						Equipment += " " + this.Trailer2.ID;
					}

					this.AddAlarmAndEventLogs(this.Security, this.Station.ExcessVehicleWeightAlarm(Equipment, ExcessWeight));

					string AbbrevString = EngineeringUnits.GetUnitAbbreviation(this.SiteManager.Site.MassUnits);

					this.StationState = StationState.OVERWEIGHT_MSG;
					this.DisplayMessageWithAcknowledge(
						"[LoadRack|Maximum Vehicle Weight] " + this.SiteManager.Site.MaximumVehicleWeight + " " + AbbrevString
						+ " [LoadRack|Exceeded by] " + ExcessWeight + " " + AbbrevString);
					return;
				}

				double Value = System.Convert.ToDouble(this.CurrentWeight.Value);
				Prompt = "[LoadRack|LoadRack|Weight Scale Motion is Invalid. Exit Weight]" + " "
							+ Value.ToString("N", this.SiteManager.Site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.MASS));
				Menu = new[] { "LoadRack|Accept", "LoadRack|Refresh Scale Reading" };
				DefaultItem = 0;
			}
			else
			{
				Prompt = "[LoadRack|Invalid] [LoadRack|Exit Weight]";
				Menu = new[] { "LoadRack|Refresh Scale Reading" };
			}

			this.StationState = StationState.CAPTURE_EXIT_WEIGHT_PROMPT;
			DisplayMenuParameters Parameters = new DisplayMenuParameters(Prompt, Menu, true, DefaultItem, this.PROMPT_TIMEOUT);
			this.DisplayMenu(Parameters);
		}

		protected void IssueCaptureTareWeightPrompt()
		{
			bool WeightScaleInMotion = false;
			bool WeightScaleMotionReadingInValid = false;
			this.ReadWeight(out this.CurrentWeight, out WeightScaleInMotion, out WeightScaleMotionReadingInValid);
			string Prompt;
			string[] Menu;

			int DefaultItem = -1;
			if (this.CurrentWeight.Quality == Quality.Good && WeightScaleInMotion == false
				 && WeightScaleMotionReadingInValid == false)
			{
				double Value = System.Convert.ToDouble(this.CurrentWeight.Value);
				Prompt = "[LoadRack|Tare Weight]" + " "
							+ Value.ToString("N", this.SiteManager.Site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.MASS));
				Menu = new[] { "LoadRack|Accept", "LoadRack|Refresh Scale Reading" };
				DefaultItem = 0;
			}
			else if (this.CurrentWeight.Quality == Quality.Good && WeightScaleInMotion && WeightScaleMotionReadingInValid == false)
			{
				double Value = System.Convert.ToDouble(this.CurrentWeight.Value);
				Prompt = "[LoadRack|Weight Scale In Motion. Tare Weight]" + " "
							+ Value.ToString("N", this.SiteManager.Site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.MASS));
				Menu = new[] { "LoadRack|Accept", "LoadRack|Refresh Scale Reading" };
				DefaultItem = 0;
			}
			else if (this.CurrentWeight.Quality == Quality.Good && WeightScaleMotionReadingInValid)
			{
				double Value = System.Convert.ToDouble(this.CurrentWeight.Value);
				Prompt = "[LoadRack|Weight Scale Motion is Invalid. Tare Weight]" + " "
							+ Value.ToString("N", this.SiteManager.Site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.MASS));
				Menu = new[] { "LoadRack|Accept", "LoadRack|Refresh Scale Reading" };
				DefaultItem = 0;
			}
			else
			{
				Prompt = "[LoadRack|Invalid] [LoadRack|Tare Weight]";
				Menu = new[] { "LoadRack|Refresh Scale Reading" };
			}

			this.StationState = StationState.CAPTURE_TARE_WEIGHT_PROMPT;
			DisplayMenuParameters Parameters = new DisplayMenuParameters(Prompt, Menu, true, DefaultItem, this.PROMPT_TIMEOUT);
			this.DisplayMenu(Parameters);
		}

		protected void IssueCarrierInsuranceWarningMessage()
		{
			this.StationState = StationState.COMPANY_INSURANCE_WARNING;
			this.DisplayMessageWithAcknowledge("[LoadRack|Carrier Insurance Warning]");
		}

		protected void IssueCarrierLicenseWarningMessage()
		{
			this.StationState = StationState.COMPANY_LICENSE_WARNING;
			this.DisplayMessageWithAcknowledge("[LoadRack|Carrier License Warning");
		}

		protected void IssueCertPermWarningMessage()
		{
			this.StationState = StationState.COMPANY_CERTIFICATE_OR_PERMIT_WARNING;
			this.DisplayMessageWithAcknowledge("[LoadRack|Cert/Perm Warning]");
		}

		protected void IssueCompartmentSummaryPrompt()
		{
			string DataDictionaryProduct = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(this.SiteManager.Site.SiteGuid, "LoadRack|Product")
																);

			string DataDictionaryPreset = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(this.SiteManager.Site.SiteGuid, "LoadRack|Preset")
																);
			ArrayList Compartments = new ArrayList();
			for (int CompartmentNumber = 1;
				  CompartmentNumber <= this.CurrentEquipment.CompartmentCollection.Count;
				  CompartmentNumber++)
			{
				if (this.CompartmentLoadPending(this.CurrentEquipment.IdentityGuid, CompartmentNumber.ToString()))
				{
					continue;
				}

				this.CurrentCompartmentNumber = CompartmentNumber;
				LineItemDO lineItem = this.CurrentLineItem;
				if (lineItem == null || lineItem.Product == null)
				{
					// Only one compartment can be loaded by weight at a time.
					if (this.ByWeight)
					{
						continue;
					}

					Compartments.Add(
						CompartmentNumber.ToString() + "  " + DataDictionaryProduct + ": "
						+ this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, "LoadRack|None"));
				}
				else
				{
					if (this.ByWeight)
					{
						Compartments.Add(
							CompartmentNumber.ToString() + "  " + DataDictionaryProduct + ": "
							+ this.GetLoadRackDisplayText(lineItem.ProductGuid));
					}
					else
					{
						Compartments.Add(
							CompartmentNumber.ToString() + "  " + DataDictionaryProduct + ": "
							+ this.GetLoadRackDisplayText(lineItem.ProductGuid) + "  " + DataDictionaryPreset + ": "
							+ lineItem.PresetAmount.Value.ToString("N", this.SiteManager.Site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME)));
					}
				}
			}

			if (Compartments.Count == 0)
			{
				if (this.LoadSummaryIssued)
				{
					this.IssueLoadSummaryPrompt();
				}

				else if (this.CurrentEquipment == this.TractorOrTanker && this.Trailer1 != null)
				{
					this.CurrentEquipment = this.Trailer1;
					this.IssueCompartmentSummaryPrompt();
				}

				else if (this.Trailer2 != null)
				{
					this.CurrentEquipment = this.Trailer2;
					this.IssueCompartmentSummaryPrompt();
				}

				else
				{
					this.IssueLoadSummaryPrompt();
				}
			}

			else if (Compartments.Count == 1 && this.AvailableCompartments(this.CurrentEquipment) == 1)
			{
				this.ProcessCompartmentSummary((string)Compartments[0]);
			}

			else
			{
				string Prompt = "";
				if (this.CurrentEquipment == this.TractorOrTanker)
				{
					Prompt = "[LoadRack|Tanker] " + this.EquipmentID(this.TractorOrTanker) + " [LoadRack|Compartment(s)]";
				}
				else if (this.CurrentEquipment == this.Trailer1)
				{
					Prompt = "[LoadRack|Trailer] " + this.EquipmentID(this.Trailer1) + " [LoadRack|Compartment(s)]";
				}
				else
				{
					Prompt = "[LoadRack|Trailer] " + this.EquipmentID(this.Trailer2) + " [LoadRack|Compartment(s)]";
				}

				int DefaultItem = Compartments.Count;
				string objValue = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(this.SiteManager.Site.SiteGuid, "LoadRack|Accept")
																);
				Compartments.Add(objValue);
				this.StationState = StationState.COMPARTMENT_SUMMARY_PROMPT;

				DisplayMenuParameters Parameters = new DisplayMenuParameters(
					Prompt, (string[])Compartments.ToArray(typeof(string)), false, DefaultItem, this.PROMPT_TIMEOUT);
				this.DisplayMenu(Parameters);
			}
		}

		public string GetDataDictionaryValueByKey(Guid siteGuid, string key)
		{
			return FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(siteGuid, key)
																);
		}

		protected void IssueCompartmentsEmptyPrompt(ContaminationPromptStatus contaminationPromptStatus)
		{
			this.StationState = StationState.COMPARTMENTS_EMPTY_PROMPT;
			this.DisplayMenu(
				new DisplayMenuParameters(
					"[LoadRack|Is the compartment(s) empty]?", new[] { "LoadRack|Yes", "LoadRack|No" }, true, -1, this.PROMPT_TIMEOUT));
		}

		protected void IssueCompartmentsPreviouslyLoadedPrompt(ContaminationPromptStatus contaminationPromptStatus)
		{
			this.StationState = StationState.COMPARTMENTS_PREVIOUSLY_LOADED_PROMPT;
			this.DisplayMenu(
				new DisplayMenuParameters(
					"[LoadRack|Compartment(s) previously loaded with] " + contaminationPromptStatus.ContaminationPromptLoadRackText
					+ "?",
					new[] { "LoadRack|Yes", "LoadRack|No" },
					true,
					-1,
					this.PROMPT_TIMEOUT));
		}

		protected void IssueContaminationPrompt(ContaminationPromptStatus contaminationPromptStatus)
		{
			contaminationPromptStatus.ContaminatePrompt = null;
			this.StationState = StationState.CONTAMINATION_PROMPT;
			this.DisplayMenu(
				new DisplayMenuParameters(
					"[LoadRack|Are you loading] " + contaminationPromptStatus.ContaminationPromptLoadRackText + "?",
					new[] { "LoadRack|Yes", "LoadRack|No" },
					true,
					-1,
					this.PROMPT_TIMEOUT));
		}

		protected void IssueDriverLicenseWarningMessage()
		{
			this.StationState = StationState.DRIVER_LICENSE_WARNING;
			this.DisplayMessageWithAcknowledge("[LoadRack|Driver License Warning]");
		}

		protected void IssueEnterOrderNumberPrompt()
		{
			this.ConsecutivePrompts = 0;
			this.StationState = StationState.ENTER_ORDER_PROMPT;
			if (this.Station.Type == STATION_TYPE.OFF_LOADING)
			{
				this.DisplayMessage("[LoadRack|Enter] [LoadRack|Order Number]", null, 10, this.PROMPT_TIMEOUT);
			}
			else
			{
				this.DisplayMessage(
					"[LoadRack|Enter] [LoadRack|Order Number] [LoadRack|or press List]", null, 10, this.PROMPT_TIMEOUT);
			}
		}

		protected void IssueEnterPurchaseOrderPrompt()
		{
			if (this.ShipTo.PurchaseOrderRequired)
			{
				// Don't prompt for the PO Number if there is a preload
				// and this is the Load Rack
				if (this.Transaction != null && this.Station.Type == STATION_TYPE.LOAD_RACK)
				{
					this.PONumber = this.Transaction.PONumber;
				}
				else
				{
					int Length = 10;

					this.DisplayMessage("LoadRack|Enter Purchase Order or Hit Enter", null, Length, this.PROMPT_TIMEOUT);
					this.StationState = StationState.PURCHASE_ORDER_PROMPT;
					return;
				}
			}

			this.StationState = StationState.IDLE;

			if (this.Station.Type == STATION_TYPE.PRELOAD
			|| this.Station.Type == STATION_TYPE.WEIGHT_SCALE
				 || this.Station.Type == STATION_TYPE.MANUAL_BOL)
			{
				this.CheckProductAvailability(false);
			}
			else
			{
				if (this.Transaction != null
				&& this.Transaction.DestinationEQ1.EquipmentType == EquipmentTypeClass.TypeID(EQUIPMENT_TYPE.TANKER_TYPE))
				{
					this.IssueEnterTankerPrompt();
				}
				else if (this.PromptForTractorOrTanker)
				{
					this.IssueTractorOrTankerPrompt();
				}
				else if (this.PromptForFirstTrailer)
				{
					this.IssueEnterTrailer1Prompt();
				}
				else
				{
					this.StationState = StationState.IDLE;
					this.CheckProductAvailability(false);
				}
			}
		}

		protected void IssueEnterTrailer1Prompt()
		{
			this.Trailer1 = null;
			if (this.TractorOrTanker == null || this.TractorOrTanker.Type == EQUIPMENT_TYPE.TRACTOR_TYPE)
			{
				this.IssueTrailer1Prompt();
			}

			else
			{
				this.StationState = StationState.ENTER_1ST_TRAILER_PROMPT;
				this.DisplayMenu(
					new DisplayMenuParameters(
						"LoadRack|Enter Trailer?", new[] { "LoadRack|Yes", "LoadRack|No" }, true, -1, this.PROMPT_TIMEOUT));
			}
		}

		protected void IssueEnterTrailer2Prompt()
		{
			this.StationState = StationState.ENTER_2ND_TRAILER_PROMPT;
			this.DisplayMenu(
				new DisplayMenuParameters(
					"LoadRack|Enter 2nd Trailer?", new[] { "LoadRack|Yes", "LoadRack|No" }, true, -1, this.PROMPT_TIMEOUT));
		}

		protected void IssueEnterTrailer3Prompt()
		{
			this.Trailer3 = null;
			this.StationState = StationState.ENTER_3RD_TRAILER_PROMPT;
			this.DisplayMenu(new DisplayMenuParameters("LoadRack|Enter 3rd Trailer?", new[] { "LoadRack|Yes", "LoadRack|No" }, true, -1, this.PROMPT_TIMEOUT));
		}

		protected void IssueEnterZipPrompt()
		{
			this.PriorStationState = StationState.SELECT_CUSTOMER_SHIPTO_FILTER_PROMPT;
			this.StationState = StationState.ENTER_ZIP_PROMPT;
			this.DisplayMessage("[LoadRack|Enter] [LoadRack|Zip], [LoadRack|or press List]", null, 5, this.PROMPT_TIMEOUT);
		}

		protected void IssueLoadByShipmentOrLoadIDPrompt()
		{
			this.StationState = StationState.SELECT_SHIPMENT_LOADID_PROMPT;
			this.DisplayMenu(
				new DisplayMenuParameters(
					"Select Type of Load", new[] { "LoadRack|Load ID", "LoadRack|Shipment Number" }, true, -1, this.PROMPT_TIMEOUT));
		}

		protected void IssueLoadSummaryPrompt()
		{
			this.LoadSummaryIssued = true;

			string DataDictionaryNone = this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, "LoadRack|None");
			string DataDictionaryProduct = this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, "LoadRack|Product");
			string DataDictionaryPreset = this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, "LoadRack|Preset");
			string DataDictionaryCompartments = this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, "LoadRack|Compartment(s)");
			string DataDictionaryCompartment = this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, "LoadRack|Compartment");
			string DataDictionaryTanker = this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, "LoadRack|Tanker");
			string DataDictionaryTrailer = this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, "LoadRack|Trailer");

			ArrayList Summary = new ArrayList();

			if (this.Order == null)
			{
				Summary.Add(this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, "LoadRack|Order]") + " : " + "N/A");
			}
			else
			{
				Summary.Add(this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, "LoadRack|Order]") + " : " + this.Order.DocumentNumber);
				this.Transaction.PONumber = this.Order.PONumber;
			}

			Summary.Add(
				this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, "[LoadRack|Ship To]") + " : "
				+ ((this.ShipTo == null) ? "" : this.ShipTo.ID));
			Summary.Add(
				this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, "[LoadRack|Bill To]") + " : "
				+ ((this.BillTo == null) ? "" : this.BillTo.ID));
			if (this.ShipTo.PurchaseOrderRequired)
			{
				Summary.Add(
					this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, "[LoadRack|Purchase Order]") + " : " + this.Transaction.PONumber);
			}

			EquipmentCollectionClass EquipmentCollection = new EquipmentCollectionClass();

			if (this.TractorOrTanker != null && this.TractorOrTanker.Type == EQUIPMENT_TYPE.TANKER_TYPE)
			{
				EquipmentCollection.Add(this.TractorOrTanker);
			}

			if (this.Trailer1 != null)
			{
				EquipmentCollection.Add(this.Trailer1);
			}

			if (this.Trailer2 != null)
			{
				EquipmentCollection.Add(this.Trailer2);
			}

			string Prompt = "";

			foreach (EquipmentClass Equipment in EquipmentCollection)
			{
				if (this.AvailableCompartments(Equipment) == 0)
				{
					continue;
				}

				if (Equipment.Type == EQUIPMENT_TYPE.TANKER_TYPE)
				{
					Prompt = DataDictionaryTanker;
				}
				else
				{
					Prompt = DataDictionaryTrailer;
				}

				Prompt += " " + this.EquipmentID(Equipment);

				if (this.AvailableCompartments(Equipment) == 1)
				{
					this.CurrentEquipment = Equipment;

					this.CurrentCompartmentNumber = this.FirstAvailableCompartment(Equipment);

					Prompt += " " + DataDictionaryCompartment + " " + this.CurrentCompartmentNumber.ToString() + " "
								 + DataDictionaryProduct + ": ";

					LineItemDO lineItem = this.CurrentLineItem;
					if (lineItem.Product == null)
					{
						Summary.Add(Prompt + DataDictionaryNone);
					}

					else
					{
						if (this.ByWeight)
						{
							Summary.Add(Prompt + this.GetLoadRackDisplayText(lineItem.ProductGuid));
						}
						else
						{
							Summary.Add(
								Prompt + this.GetLoadRackDisplayText(lineItem.ProductGuid) + "  " + DataDictionaryPreset + ": "
								+ lineItem.PresetAmount.Value.ToString(
									"N", this.SiteManager.Site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME)));
						}
					}
				}

				else
				{
					Summary.Add(Prompt + " " + DataDictionaryCompartments);
				}
			}

			this.StationState = StationState.SUMMARY_PROMPT;
			int DefaultItem = -1;
			if (this.ProductsConfigured)
			{
				Prompt = this.Mode + " [LoadRack|Summary]";
				DefaultItem = Summary.Count;
				Summary.Add(this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, "LoadRack|Accept"));
			}
			else
			{
				Prompt = this.Mode + " [LoadRack|Summary] [LoadRack|No Compartments Selected]";
			}

			DisplayMenuParameters Parameters = new DisplayMenuParameters(
				Prompt, (string[])Summary.ToArray(typeof(string)), false, DefaultItem, this.PROMPT_TIMEOUT);
			this.DisplayMenu(Parameters);
		}

		protected void IssueOffLoadEnterZipPrompt()
		{
			this.PriorStationState = StationState.SELECT_SUPPLIER_OFFLOADID_FILTER_PROMPT;
			this.StationState = StationState.ENTER_OFFLOADID_ZIP_PROMPT;
			this.DisplayMessage("[LoadRack|Enter] [LoadRack|Zip], [LoadRack|or press List]", null, 5, this.PROMPT_TIMEOUT);
		}

		protected void IssueOffLoadIDPrompt()
		{
			this.Transaction = null;

			this.StationState = StationState.OFFLOADID_PROMPT;
			this.DisplayMessage("[LoadRack|Enter] [LoadRack|Off Load ID]", null, PromptLength, this.PROMPT_TIMEOUT);
		}

		protected void IssueOperatingModePrompt()
		{
			if (this.Station.InhibitOperatingModePrompt)
			{
				this.ProcessOperatingMode(this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, "LoadRack|Loading"));
			}
			else
			{
				this.StationState = StationState.OPERATING_MODE_PROMPT;
				DisplayMenuParameters parameters = new DisplayMenuParameters(
					"[LoadRack|Select] [LoadRack|Operating Mode]",
					new[] { "LoadRack|Loading", "LoadRack|UnLoading" },
					true,
					-1,
					this.PROMPT_TIMEOUT);
				this.DisplayMenu(parameters);
			}
		}

		protected void IssuePresetPrompt()
		{
			this.ConsecutivePrompts = 0;

			this.StationState = StationState.PRESET_PROMPT;

			LineItemDO lineItem;
			if (this.Station.Type == STATION_TYPE.MANUAL_BOL)
			{
				if (this.CurrentLineItemBaseIndex == -1)
				{
					lineItem = new LineItemDO
					{
						LoadingLocationID = this.Station.ID,
						LoadingLocationStationGuid = this.Station.IdentityGuid
					};

					this.Transaction.LineItems.Add(lineItem);
					this.CurrentLineItemBaseIndex = this.Transaction.LineItems.Count - 1;
				}
				else
				{
					lineItem = this.Transaction.LineItems[this.CurrentLineItemBaseIndex];
				}
			}
			else
			{
				lineItem = this.CurrentLineItem;
			}

			ProductClass product = FMChannelHelper.MakeCall<IProducts, ProductClass>(
																 x =>
																 x.GetByProductAuthorizedCompanies(this.Security, lineItem.ProductGuid, false)
															);
			this.CurrentMaximum = this.GetMaximum(product, lineItem.Density, lineItem.VCF, 0.0, null, lineItem);
			string maximumPreset = this.CurrentMaximum.ToString("N", this.SiteManager.Site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME));
			string presetString;
			if (this.Station.Type == STATION_TYPE.MANUAL_BOL)
			{
				presetString = " [LoadRack|Quantity]";
			}
			else
			{
				presetString = " [LoadRack|Preset]";
			}

			// Recompute the maximum because the previous operation may have rounded up.
			this.CurrentMaximum = System.Convert.ToDouble(maximumPreset, this.SiteManager.Site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME));
			string productText = this.GetLoadRackDisplayText(lineItem.ProductGuid);
			if (this.Station.Type == STATION_TYPE.MANUAL_BOL && lineItem.Quantity.Gross != 0.0)
			{
				string currentPreset = lineItem.Quantity.Gross.ToString("N", this.SiteManager.Site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME));
				this.DisplayMessage(
					 "[LoadRack|Enter] " + productText + presetString + ", [LoadRack|Maximum] " + maximumPreset,
					 currentPreset,
					 10,
					 this.PROMPT_TIMEOUT);
			}
			else if (this.Station.SetDefaultPresetToZero)
			{
				string CurrentPreset = "0";
				this.DisplayMessage(
					"[LoadRack|Enter] " + productText + " [LoadRack|Preset], [LoadRack|Maximum] " + maximumPreset,
					CurrentPreset,
					10,
					this.PROMPT_TIMEOUT);
			}
			else
			{
				this.DisplayMessage(
					"[LoadRack|Enter] " + productText + " [LoadRack|Preset], [LoadRack|Maximum] " + maximumPreset,
					maximumPreset,
					10,
					this.PROMPT_TIMEOUT);
			}
		}

		protected void IssuePromptForPIN()
		{
			this.StationState = StationState.PIN_PROMPT;
			this.PromptForPin("[LoadRack|Enter PIN]", 4, this.PROMPT_TIMEOUT);
		}

		protected void IssueQualificationWarningMessage()
		{
			this.StationState = StationState.DRIVER_QUALIFICATION_WARNING;
			this.DisplayMessageWithAcknowledge("[LoadRack|Qualification Warning]");
		}

		protected void IssueSelectCompanyHierarchyPrompt()
		{
			string[] CompaniesList = this.HeirarchicalCompanyList();
			string Prompt;
			switch (this.CurrentCompanyHierarchyType)
			{
				case COMPANY_MAP_TYPE.SHIPTO_BILLTO_MAP:
					Prompt = "[LoadRack|Select] [LoadRack|Bill To]";
					break;

				case COMPANY_MAP_TYPE.BILLTO_SHIPPER_MAP:
					Prompt = "[LoadRack|Select] [LoadRack|Shipper]";
					break;

				case COMPANY_MAP_TYPE.SHIPPER_OWNER_MAP:
				case COMPANY_MAP_TYPE.SUPPLIER_OWNER_MAP:
					Prompt = "[LoadRack|Select] [LoadRack|Owner]";
					break;

				case COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP:
				case COMPANY_MAP_TYPE.OFFLOAD_OWNER_MANAGER_MAP:
					Prompt = "[LoadRack|Select] [LoadRack|Manager]";
					break;

				default:
					throw new Exception("Invalid Company Map Type");
			}

			if (CompaniesList.Length == 0)
			{
				this.DisplayMessage("LoadRack|No] [LoadRack|Company Hierarchy]", null, 0, this.MESSAGE_TIMEOUT);
				return;
			}

			else if (CompaniesList.Length == 1)
			{
				this.ProcessCompanyHierarchy(CompaniesList[0]);
			}

			else
			{
				this.StationState = StationState.SELECT_COMPANY_HIERARCHY_PROMPT;
				DisplayMenuParameters Parameters = new DisplayMenuParameters(Prompt, CompaniesList, false, -1, this.PROMPT_TIMEOUT);
				this.DisplayMenu(Parameters);
			}
		}

		protected void IssueSelectCustomerShipToFilterColumnPrompt()
		{
			this.StationState = StationState.SELECT_CUSTOMER_SHIPTO_FILTER_PROMPT;
			DisplayMenuParameters Parameters = new DisplayMenuParameters(
				"LoadRack|Select Customer ShipTo By",
				new[] { "[LoadRack|Zip]", "[LoadRack|City]", "[LoadRack|State]", "LoadRack|Destination]" },
				true,
				-1,
				this.PROMPT_TIMEOUT);
			this.DisplayMenu(Parameters);
		}

		protected void IssueSelectCustomerShipToFilterValuePrompt()
		{
			string[] FilterValues = FMChannelHelper.MakeCall<ICompanies, string[]>(
					x =>
					x.EnumerateColumnForAuthorizedCustomerShipTo(this.Security, this.Carrier.IdentityGuid, this.CustomerShipToFilterColumn)
			);

			if (FilterValues.Length == 0)
			{
				this.DisplayMessage(
					"LoadRack|No] [LoadRack|" + this.CustomerShipToFilterColumn + "]", null, 0, this.MESSAGE_TIMEOUT);
			}
			else
			{
				// If StationState is SELECT_CUSTOMER_SHIPTO_PROMPT this is a BACK operation
				if (this.StationState != StationState.SELECT_CUSTOMER_SHIPTO_PROMPT)
				{
					this.PriorStationState = this.StationState;
				}

				this.StationState = StationState.SELECT_CUSTOMER_SHIPTO_FILTER_VALUE_PROMPT;
				DisplayMenuParameters Parameters = new DisplayMenuParameters(
					"[LoadRack|Select] [LoadRack|" + this.CustomerShipToFilterColumn + "]",
					FilterValues,
					false,
					-1,
					this.PROMPT_TIMEOUT);
				this.DisplayMenu(Parameters);
			}
		}

		protected void IssueSelectCustomerShipToFromCarrierShipToCollectionPrompt()
		{
			ArrayList Company = new ArrayList();

			CompanyCollectionClass CarrierCustomerShipToCollection =
				FMChannelHelper.MakeCall<ICompanies, CompanyCollectionClass>(
																	 x =>
																	 x.EnumerateAuthorizedCustomerShipToForColumnValue(this.Security, "", "", this.Carrier.IdentityGuid)
																);
			foreach (CompanyClass CustomerShipTo in CarrierCustomerShipToCollection)
			{
				Company.Add(CustomerShipTo.CompanyToolTip);
			}

			this.StationState = StationState.SELECT_CUSTOMER_SHIPTO_PROMPT;
			DisplayMenuParameters Parameters = new DisplayMenuParameters(
				"[LoadRack|Select] [LoadRack|Ship To]", (string[])Company.ToArray(typeof(string)), false, -1, this.PROMPT_TIMEOUT);
			this.DisplayMenu(Parameters);
		}

		protected void IssueSelectCustomerShipToFromFilterPrompt()
		{
			CompanyCollectionClass CompanyCollection = FMChannelHelper.MakeCall<ICompanies, CompanyCollectionClass>(
					x =>
					x.EnumerateAuthorizedCustomerShipToForColumnValue(this.Security, this.CustomerShipToFilterColumn,
						this.CustomerShipToFilterValue, this.Carrier.IdentityGuid)
			);

			if (CompanyCollection.Count == 0)
			{
				this.StationState = StationState.NO_SHOPTO_MSG;
				this.DisplayMessage("[LoadRack|No] [LoadRack|Ship To]", null, 0, this.MESSAGE_TIMEOUT);
			}

			else
			{
				this.StationState = StationState.SELECT_CUSTOMER_SHIPTO_PROMPT;
				string[] CustomerShipTo = new string[CompanyCollection.Count];
				int Index = 0;
				foreach (CompanyClass Company in CompanyCollection)
				{
					CustomerShipTo[Index++] = Company.CompanyToolTip;
				}

				DisplayMenuParameters Parameters = new DisplayMenuParameters(
					"LoadRack|Select] [LoadRack|Ship To]", CustomerShipTo, false, -1, this.PROMPT_TIMEOUT);
				this.DisplayMenu(Parameters);
			}
		}

		protected void IssueSelectPreloadBy()
		{
			// Build initial menu parameter set
			DisplayMenuParameters parameters = new DisplayMenuParameters
			{
				ApplyDataDictionary = true,
				DefaultItem = 0,
				MenuTimeout = this.PROMPT_TIMEOUT,
				Caption = "[LoadRack|Select By]" + (this.NumericMenuSelection ? " [LoadRack|0=Cancel]" : "")
			};


			ArrayList menu = new ArrayList();

			if (this.OrderList.Count > 0)
			{
				menu.Add("LoadRack|Order");
			}

			if (this.LoadIDList.Count > 0)
			{
				menu.Add("LoadRack|Load ID");
			}

			menu.Add("LoadRack|Document");
			parameters.Menu = (string[])menu.ToArray(typeof(string));

			this.StationState = StationState.PRELOAD_TYPE_PROMPT;
			this.DisplayMenu(parameters);
		}

		protected bool IssueSelectPreloadDocument()
		{
			// Build initial menu parameter set
			DisplayMenuParameters parameters = new DisplayMenuParameters
			{
				ApplyDataDictionary = true,
				DefaultItem = 0,
				MenuTimeout = this.PROMPT_TIMEOUT,
				Caption = "[LoadRack|Document]" + (this.NumericMenuSelection ? " [LoadRack|0=Cancel" : "")
			};

			ArrayList menu = new ArrayList();

			foreach (DataRow row in this.PreloadDataSet.Tables[0].Rows)
			{
				if (this.PreloadSelectMethod == PRELOAD_SELECT_METHOD.ORDER)
				{
					if (this.Order.TransID != row["TransReferenceId"] as string)
					{
						continue;
					}
				}

				else if (this.PreloadSelectMethod == PRELOAD_SELECT_METHOD.LOADID)
				{
					if (this.LoadID != row["LoadID"] as string)
					{
						continue;
					}
				}

				string documentNumber = row["DocumentNumber"] as string;

				if (string.IsNullOrEmpty(documentNumber) == false)
				{
					menu.Add(documentNumber);
				}
			}

			// Bail out if we did not get any valid document numbers to use for prompting
			if (menu.Count == 0)
			{
				return false;
			}

			if (menu.Count == 1)
			{
				this.LoadTransaction((string)menu[0]);
			}

			else
			{
				parameters.Menu = (string[])menu.ToArray(typeof(string));

				this.StationState = StationState.PRELOAD_DOCNUMBER_PROMPT;
				this.DisplayMenu(parameters);
			}

			return true;
		}

		protected void IssueSelectPreloadLoadID()
		{
			// Build initial menu parameter set
			DisplayMenuParameters parameters = new DisplayMenuParameters
			{
				ApplyDataDictionary = true,
				DefaultItem = 0,
				MenuTimeout = this.PROMPT_TIMEOUT,
				Caption = "[LoadRack|Load ID]" + (this.NumericMenuSelection ? " [LoadRack|0=Cancel]" : ""),
				Menu = new string[this.LoadIDList.Count]
			};

			int nItem = 0;
			foreach (string loadID in this.LoadIDList)
			{
				parameters.Menu[nItem++] = loadID;
			}

			this.StationState = StationState.PRELOAD_LOADID_PROMPT;
			this.DisplayMenu(parameters);
			this.PreloadSelectMethod = PRELOAD_SELECT_METHOD.LOADID;
		}

		protected void IssueSelectPreloadOrder()
		{
			// Build initial menu parameter set
			DisplayMenuParameters parameters = new DisplayMenuParameters
			{
				ApplyDataDictionary = true,
				DefaultItem = 0,
				MenuTimeout = this.PROMPT_TIMEOUT,
				Caption = "[LoadRack|Order]" + (this.NumericMenuSelection ? " [LoadRack|0=Cancel]" : ""),
				Menu = new string[this.OrderList.Count]
			};

			int nItem = 0;
			foreach (GetTransactionTypeDO getTransactionTypeDO in this.OrderList)
			{
				parameters.Menu[nItem++] = getTransactionTypeDO.DocumentNumber;
			}

			this.StationState = StationState.PRELOAD_ORDER_PROMPT;
			this.DisplayMenu(parameters);
			this.PreloadSelectMethod = PRELOAD_SELECT_METHOD.ORDER;
		}

		protected void IssueSelectProductPrompt()
		{
			ArrayList products = new ArrayList();

			// When operating By Weight Product is selected, only that product may be selected
			if (!this.ByWeight)
			{
				if (this.Order != null)
				{
					products.Add(this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, "[LoadRack|None]"));
					foreach (ProductMapClass authorizedProduct in this.ShipTo.AuthorizedProductCollection)
					{
						if (authorizedProduct.LockedOut)
						{
							continue;
						}

						foreach (LineItemDO orderLineItem in this.Order.LineItems)
						{
							if (orderLineItem.Product == authorizedProduct.AssignedID)
							{
								if (orderLineItem.Status != TransactionStatus.Scheduled)
								{
									break;
								}

								if (orderLineItem.GrossQuantityRemaining <= 0 && orderLineItem.NetQuantityRemaining <= 0)
								{
									break;
								}

								products.Add(GetLoadRackDisplayText(authorizedProduct));
								break;
							}
						}
					}
				}

				// Available Products based upon current ShipTo Authorized Products
				else
				{
					products.Add(this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, "[LoadRack|None]"));

					foreach (ProductMapClass authorizedProduct in this.ShipTo.AuthorizedProductCollection)
					{
						if (authorizedProduct.LockedOut)
						{
							continue;
						}

						products.Add(GetLoadRackDisplayText(authorizedProduct));
					}
				}
			}
			else
			{
				products.Add(this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, "LoadRack|None"));
				products.Add(this.ByWeightProduct);
			}

			if (products.Count == 1)
			{
				this.StationState = StationState.NO_PRODUCTS_MSG;
				this.DisplayMessage("LoadRack|No Products Available", null, 0, this.MESSAGE_TIMEOUT);
			}

			// When only 1 product is available and there is a single compartment
			else if (products.Count == 2 && this.TotalAvailableCompartments == 1 && !this.LoadSummaryIssued)
			{
				this.SingleProduct = true;
				this.ProcessProduct((string)products[1]);
			}
			else
			{
				this.SingleProduct = false;
				this.StationState = StationState.PRODUCT_PROMPT;

				string prompt;

				if (this.CurrentEquipment.MasterRecordGuid == (this.TractorOrTanker?.MasterRecordGuid ?? Guid.Empty))
				{
					prompt = "[LoadRack|Tanker] ";
				}
				else if (this.CurrentEquipment.MasterRecordGuid == (this.Trailer1?.MasterRecordGuid ?? Guid.Empty))
				{
					prompt = "[LoadRack|Trailer] ";
				}
				else
				{
					prompt = "[LoadRack|Trailer] ";
				}

				prompt += this.EquipmentID(this.CurrentEquipment) + " [LoadRack|Compartment] "
							 + this.CurrentCompartmentNumber.ToString() + " [LoadRack|Product]";

				DisplayMenuParameters parameters = new DisplayMenuParameters(
					prompt, (string[])products.ToArray(typeof(string)), false, -1, this.PROMPT_TIMEOUT);
				this.DisplayMenu(parameters);
			}
		}

		protected void IssueSelectOffloadProductPrompt()
		{
			ArrayList products = new ArrayList();
			this.SetCurrentEquipmentToFirstAvailable();
			this.CurrentCompartmentNumber = 1;

			// When operating By Weight Product is selected, only that product may be selected
			if (!this.ByWeight)
			{
				if (this.SupplyOrder != null)
				{
					foreach (ProductMapClass authorizedProduct in this.Supplier.SupplierAuthorizedProductCollection)
					{
						if (authorizedProduct.LockedOut)
						{
							continue;
						}

						foreach (LineItemDO supplyOrderLineItem in this.SupplyOrder.LineItems)
						{
							if (supplyOrderLineItem.Product == authorizedProduct.AssignedID)
							{
								if (supplyOrderLineItem.Status != TransactionStatus.Scheduled)
								{
									break;
								}

								if (supplyOrderLineItem.GrossQuantityRemaining <= 0 &&
									 supplyOrderLineItem.NetQuantityRemaining <= 0 &&
									 supplyOrderLineItem.MassQuantityRemaining <= 0)
								{
									break;
								}

								products.Add(GetLoadRackDisplayText(authorizedProduct));
								break;
							}
						}
					}

					products.Add(FMChannelHelper.MakeCall<IDataDictionariesClass, string>(x => x.Get(this.SiteManager.Site.SiteGuid, "[LoadRack|None]")));
				}
				else
				{
					// Available Products based upon current Supplier Authorized Products
					foreach (ProductMapClass authorizedProduct in this.Supplier.SupplierAuthorizedProductCollection)
					{
						if (authorizedProduct.LockedOut)
						{
							continue;
						}

						products.Add(GetLoadRackDisplayText(authorizedProduct));
					}

					products.Add(FMChannelHelper.MakeCall<IDataDictionariesClass, string>(x => x.Get(this.SiteManager.Site.SiteGuid, "[LoadRack|None]")));
				}
			}
			else
			{
				products.Add(FMChannelHelper.MakeCall<IDataDictionariesClass, string>(x => x.Get(this.SiteManager.Site.SiteGuid, "LoadRack|None")));
				products.Add(this.ByWeightProduct);
			}

			if (products.Count == 1)
			{
				this.StationState = StationState.NO_PRODUCTS_MSG;
				this.DisplayMessage("LoadRack|No Products Available", null, 0, this.MESSAGE_TIMEOUT);
			}
			else if (products.Count == 2 // When only 1 product is available and there is a single compartment
				 && this.TotalAvailableCompartments == 1
				 && !this.LoadSummaryIssued)
			{
				this.SingleProduct = true;
				this.ProcessOffloadProduct((string)products[0]);
			}
			else
			{
				this.SingleProduct = false;
				this.StationState = StationState.OFFLOAD_PRODUCT_PROMPT;

				const string Prompt = "[LoadRack|Select] [LoadRack|Product] ";

				DisplayMenuParameters parameters = new DisplayMenuParameters(
					 Prompt,
					 (string[])products.ToArray(typeof(string)),
					 false,
					 -1,
					 this.PROMPT_TIMEOUT);
				this.DisplayMenu(parameters);
			}
		}

		protected void IssueSelectSupplierFromFilterPrompt()
		{
			CompanyCollectionClass CompanyCollection =
				FMChannelHelper.MakeCall<ICompanies, CompanyCollectionClass>(
					x =>
					x.EnumerateAuthorizedSupplierForColumnValue(
						this.Security, this.CustomerShipToFilterColumn, this.CustomerShipToFilterValue));
			if (CompanyCollection.Count == 0)
			{
				this.StationState = StationState.NO_SUPPLIER_MSG;
				this.DisplayMessage("[LoadRack|No] [LoadRack|Supplier]", null, 0, this.MESSAGE_TIMEOUT);
			}

			else
			{
				this.StationState = StationState.SELECT_SUPPLIER_PROMPT;
				string[] CustomerShipTo = new string[CompanyCollection.Count];
				int Index = 0;
				foreach (CompanyClass Company in CompanyCollection)
				{
					CustomerShipTo[Index++] = Company.CompanyToolTip;
				}

				DisplayMenuParameters Parameters = new DisplayMenuParameters(
					"LoadRack|Select] [LoadRack|Supplier]", CustomerShipTo, false, -1, this.PROMPT_TIMEOUT);
				this.DisplayMenu(Parameters);
			}
		}

		protected void IssueSelectSupplierOffLoadIDFilterColumnPrompt()
		{
			this.StationState = StationState.SELECT_SUPPLIER_OFFLOADID_FILTER_PROMPT;
			DisplayMenuParameters Parameters = new DisplayMenuParameters(
				"LoadRack|Select Supplier By",
				new[] { "[LoadRack|Zip]", "[LoadRack|City]", "[LoadRack|State]", "[LoadRack|Supplier]" },
				true,
				-1,
				this.PROMPT_TIMEOUT);
			this.DisplayMenu(Parameters);
		}

		protected void IssueSelectSupplierOffLoadIDFilterValuePrompt()
		{
			string[] FilterValues =
				FMChannelHelper.MakeCall<ICompanies, string[]>(
					x => x.EnumerateColumnForAuthorizedSupplierOffLoadID(this.Security, this.CustomerShipToFilterColumn));

			if (FilterValues.Length == 0)
			{
				this.DisplayMessage(
					"LoadRack|No] [LoadRack|" + this.CustomerShipToFilterColumn + "]", null, 0, this.MESSAGE_TIMEOUT);
			}
			else
			{
				// If StationState is SELECT_SUPPLIER_PROMPT this is a BACK operation
				if (this.StationState != StationState.SELECT_SUPPLIER_PROMPT)
				{
					this.PriorStationState = this.StationState;
				}

				this.StationState = StationState.SELECT_SUPPLIER_FILTER_VALUE_PROMPT;
				DisplayMenuParameters Parameters = new DisplayMenuParameters(
					"[LoadRack|Select] [LoadRack|" + this.CustomerShipToFilterColumn + "]",
					FilterValues,
					false,
					-1,
					this.PROMPT_TIMEOUT);
				this.DisplayMenu(Parameters);
			}
		}

		protected void IssueSelectSupplierOffLoadIDFromCarrierShipToCollectionPrompt()
		{
			ArrayList Company = new ArrayList();

			CompanyCollectionClass CarrierCustomerShipToCollection =
				FMChannelHelper.MakeCall<ICompanies, CompanyCollectionClass>(
					x => x.EnumerateAuthorizedSupplierForColumnValue(this.Security, "", ""));

			foreach (CompanyClass CustomerShipTo in CarrierCustomerShipToCollection)
			{
				Company.Add(CustomerShipTo.CompanyToolTip);
			}

			this.StationState = StationState.SELECT_DESTINATION_SUPPLIER_PROMPT;
			DisplayMenuParameters Parameters = new DisplayMenuParameters(
				"[LoadRack|Select] [LoadRack|Supplier]", (string[])Company.ToArray(typeof(string)), false, -1, this.PROMPT_TIMEOUT);
			this.DisplayMenu(Parameters);
		}

		protected void IssueSelectTractorOrTankerPrompt()
		{
			ArrayList equipmentList = new ArrayList();

			foreach (EquipmentClass equipment in this.Carrier.EquipmentCollection)
			{
				if (equipment.Type != EQUIPMENT_TYPE.TANKER_TYPE && equipment.Type != EQUIPMENT_TYPE.TRACTOR_TYPE)
				{
					continue;
				}

				if (equipment.LockedOut)
				{
					continue;
				}

				if (this.SiteManager.Site.UseCompanyEquipmentIdentifiers)
				{
					equipmentList.Add(equipment.CompanyEquipmentID);
				}
				else
				{
					equipmentList.Add(equipment.ID);
				}
			}

			this.StationState = StationState.SELECT_TRACTOR_OR_TANKER_PROMPT;

			string prompt = this.GetDataDictionaryValueByKey(this.Security.SiteGuid, "[LoadRack|Select] [LoadRack|Tractor/Tanker]");

			DisplayMenuParameters parameters = new DisplayMenuParameters(
				prompt, (string[])equipmentList.ToArray(typeof(string)), false, -1, this.PROMPT_TIMEOUT);

			this.DisplayMenu(parameters);
		}

		protected void IssueSelectTrailer1Prompt()
		{
			ArrayList equipmentList = new ArrayList();

			foreach (EquipmentClass Equipment in this.Carrier.EquipmentCollection)
			{
				if (Equipment.Type != EQUIPMENT_TYPE.TRAILER_TYPE)
				{
					continue;
				}

				if (Equipment.LockedOut)
				{
					continue;
				}

				if (this.SiteManager.Site.UseCompanyEquipmentIdentifiers)
				{
					equipmentList.Add(Equipment.CompanyEquipmentID);
				}
				else
				{
					equipmentList.Add(Equipment.ID);
				}
			}

			this.StationState = StationState.SELECT_TRAILER1_PROMPT;

			string Prompt;

			// A Tanker can only have a single trailer
			if (this.SiteManager.Site.PromptForSecondTrailer
				 && (this.TractorOrTanker == null || this.TractorOrTanker.Type == EQUIPMENT_TYPE.TRACTOR_TYPE))
			{
				Prompt = this.GetDataDictionaryValueByKey(this.Security.SiteGuid, "[LoadRack|Select] [LoadRack|1st Trailer]");
			}
			else
			{
				Prompt = this.GetDataDictionaryValueByKey(this.Security.SiteGuid, "[LoadRack|Select] [LoadRack|Trailer]");
			}

			DisplayMenuParameters Parameters = new DisplayMenuParameters(
				Prompt, (string[])equipmentList.ToArray(typeof(string)), false, -1, this.PROMPT_TIMEOUT);
			this.DisplayMenu(Parameters);
		}

		protected void IssueSelectTrailer2Prompt()
		{
			ArrayList equipmentList = new ArrayList();

			foreach (EquipmentClass equipment in this.Carrier.EquipmentCollection)
			{
				if (equipment.Type != EQUIPMENT_TYPE.TRAILER_TYPE)
				{
					continue;
				}

				if (equipment.LockedOut)
				{
					continue;
				}

				if (equipment.IdentityGuid == this.Trailer1.IdentityGuid)
				{
					continue;
				}

				if (this.SiteManager.Site.UseCompanyEquipmentIdentifiers)
				{
					equipmentList.Add(equipment.CompanyEquipmentID);
				}
				else
				{
					equipmentList.Add(equipment.ID);
				}
			}

			this.StationState = StationState.SELECT_TRAILER2_PROMPT;

			string prompt = this.GetDataDictionaryValueByKey(this.Security.SiteGuid, "[LoadRack|Select] [LoadRack|2nd Trailer]");

			DisplayMenuParameters parameters = new DisplayMenuParameters(
				prompt, (string[])equipmentList.ToArray(typeof(string)), false, -1, this.PROMPT_TIMEOUT);

			this.DisplayMenu(parameters);
		}

		protected void IssueSelectTrailer3Prompt()
		{
			ArrayList equipmentList = new ArrayList();

			foreach (EquipmentClass equipment in this.Carrier.EquipmentCollection)
			{
				if (equipment.Type != EQUIPMENT_TYPE.TRAILER_TYPE)
				{
					continue;
				}

				if (equipment.LockedOut)
				{
					continue;
				}

				if (equipment.IdentityGuid == this.Trailer2.IdentityGuid)
				{
					continue;
				}

				if (this.SiteManager.Site.UseCompanyEquipmentIdentifiers)
				{
					equipmentList.Add(equipment.CompanyEquipmentID);
				}
				else
				{
					equipmentList.Add(equipment.ID);
				}
			}

			this.StationState = StationState.SELECT_TRAILER3_PROMPT;

			string prompt = this.GetDataDictionaryValueByKey(this.Security.SiteGuid, "[LoadRack|Select] [LoadRack|3rd Trailer]");

			DisplayMenuParameters parameters = new DisplayMenuParameters(
				 prompt,
				 (string[])equipmentList.ToArray(typeof(string)),
				 false,
				 -1, this.PROMPT_TIMEOUT);

			this.DisplayMenu(parameters);
		}

		protected void IssueShipToMenu(bool preloadShipto)
		{
			string[] menuItems = { "LoadRack|Yes", "LoadRack|No" };
			string shipToLoadRackDisplayText;
			if (string.IsNullOrEmpty(this.ShipTo.LoadRackDisplayText) == false)
			{
				shipToLoadRackDisplayText = this.ShipTo.LoadRackDisplayText;
			}
			else
			{
				shipToLoadRackDisplayText = this.ShipTo.ID;
			}

			DisplayMenuParameters parameters = new DisplayMenuParameters(
				"[LoadRack|Ship To] - " + shipToLoadRackDisplayText, menuItems, true, -1, this.PROMPT_TIMEOUT);
			this.DisplayMenu(parameters);
			if (preloadShipto)
			{
				this.StationState = StationState.VERIFY_SHIPTO_MSG_PRELOAD;
			}
			else
			{
				this.StationState = StationState.VERIFY_SHIPTO_MSG;
			}
		}

		protected void IssueTagLicenseWarningMessage()
		{
			this.DisplayMessageWithAcknowledge("[LoadRack|Tag/License Warning]");
		}

		protected void IssueTestInspWarningMessage()
		{
			this.DisplayMessageWithAcknowledge("[LoadRack|Test/Insp Warning]");
		}

		protected void IssueTractorCardInPrompt()
		{
			// Remove Line Items associated with Tanker
			if (this.TractorOrTanker != null)
			{
				this.TractorOrTanker = null;
				if (this.Transaction != null)
				{
					this.Transaction.LineItems.Clear();
				}
			}

			this.ConsecutivePrompts = 0;
			this.StationState = StationState.ENTER_TRACTOR_CARDIN_PROMPT;
			if (this.Station.TouchKeyReader)
			{
				this.DisplayMessage("[LoadRack|Scan] [LoadRack|Truck Key]", null, PromptLength, this.PROMPT_TIMEOUT);
			}
			else
			{
				this.DisplayMessage("[LoadRack|Scan] [LoadRack|Truck Card]", null, PromptLength, this.PROMPT_TIMEOUT);
			}
		}

		protected void IssueEnterTankerPrompt()
		{
			this.TractorOrTanker = null;
			this.StationState = StationState.ENTER_TANKER_PROMPT;
			this.DisplayMenu(new DisplayMenuParameters("LoadRack|Enter Tanker?", new[] { "LoadRack|Yes", "LoadRack|No" }, true, -1, this.PROMPT_TIMEOUT));
		}

		protected void IssueTractorOrTankerPrompt()
		{
			// Remove Line Items associated with Tanker
			if (this.TractorOrTanker != null)
			{
				this.TractorOrTanker = null;
				this.Transaction?.LineItems.Clear();
			}

			this.ConsecutivePrompts = 0;
			this.StationState = StationState.TRACTOR_OR_TANKER_PROMPT;
			this.DisplayMessage("[LoadRack|Enter] [LoadRack|Tractor/Tanker]", null, PromptLength, this.PROMPT_TIMEOUT);
		}

		protected void IssueTrailer1Prompt()
		{
			this.ConsecutivePrompts = 0;
			this.StationState = StationState.ENTER_TRAILER1_PROMPT;
			this.Trailer1 = null;
			bool present = false;
			bool validData = false;
			this.bScullyFailMannualEnter = false;
			this.bScullyBypass = false;
			if (this.Station.Type == STATION_TYPE.LOAD_RACK && this.Station.EnableScully)
			{
				this.TIN = string.Empty;

				// sijuan: try three time if connection is failure
				for (int i = 0; i < 3; i++)
				{
					this.TruckPresent(out this.bScullyBypass, out present, out this.TIN, out validData);
					if (!this.bScullyBypass)
					{
						if (validData)
						{
							i++;
							continue;
						}

						if (present)
						{
							if (this.TIN == "FFFFFFFFFFFF")
							{
								// sijuan: this indicate that the TIM or communications was faulty
								break;
							}

							this.ProcessTrailer1ID(this.TIN);
							if (!this.bScullyFailMannualEnter)
							{
								return;
							}

							break;
						}

						i++;
						continue;
					}

					break;
				}

				if (!this.bScullyBypass)
				{
					if (validData)
					{
						this.DisplayMessageWithAcknowledge("LoadRack|Communication to Scully failed, ");
					}
					else if (!present)
					{
						this.DisplayMessageWithAcknowledge("LoadRack|Trailer is not detected, ");
					}

					if (validData || !present || (this.TIN == "FFFFFFFFFFFF") || this.bScullyFailMannualEnter)
					{
						if (this.SiteManager.Site.RequireTrailerScully || this.Carrier.ScullyRequired)
						{
							this.ScullyUnavailableMessage();
							return;
						}
					}

					this.bScullyFailMannualEnter = true;
				}
				else
				{
					this.AddAlarmAndEventLogs(this.Security, this.Station.ScullyBypassUsedEvent(this.Driver.FirstLastName, this.Carrier.ID));
				}
			}

			// A Tanker can only have a single trailer
			if (this.SiteManager.Site.PromptForSecondTrailer
			 && (this.TractorOrTanker == null || this.TractorOrTanker.Type == EQUIPMENT_TYPE.TRACTOR_TYPE))
			{
				this.DisplayMessage("[LoadRack|Enter] [LoadRack|1st Trailer]", null, PromptLength, this.PROMPT_TIMEOUT);
			}

			else
			{
				this.DisplayMessage("[LoadRack|Enter] [LoadRack|Trailer]", null, PromptLength, this.PROMPT_TIMEOUT);
			}
		}

		protected void IssueTrailer2Prompt()
		{
			this.ConsecutivePrompts = 0;
			this.StationState = StationState.ENTER_TRAILER2_PROMPT;
			this.Trailer2 = null;
			this.DisplayMessage("[LoadRack|Enter] [LoadRack|2nd Trailer]", null, PromptLength, this.PROMPT_TIMEOUT);
		}

		protected void IssueTrailer3Prompt()
		{
			this.ConsecutivePrompts = 0;
			this.StationState = StationState.ENTER_TRAILER3_PROMPT;
			this.Trailer3 = null;
			this.DisplayMessage("[LoadRack|Enter] [LoadRack|3nd Trailer]", null, PromptLength, this.PROMPT_TIMEOUT);
		}

		protected void IssueTrainingWarningMessage()
		{
			this.StationState = StationState.DRIVER_TRAINING_WARNING;
			this.DisplayMessageWithAcknowledge("[LoadRack|Training Warning]");
		}

		protected void IssueUseOrderNumberPrompt()
		{
			this.Manager = null;
			this.Owner = null;
			this.Shipper = null;
			this.BillTo = null;
			this.ShipTo = null;
			this.Order = null;
			this.LoadID = null;
			this.PONumber = null;

			this.Orders = false;

			OrderListDO orderListDO = this.GetOrders();

			if (orderListDO != null && orderListDO.LineItems.Count > 0)
			{
				foreach (OrderListLineItemDO lineItem in orderListDO.LineItems)
				{
					if (string.IsNullOrEmpty(lineItem.CarrierID) == false && lineItem.CarrierID != this.Carrier.ID)
					{
						continue;
					}

					if (string.IsNullOrEmpty(lineItem.OperatorID) == false && lineItem.OperatorID != this.Driver.ID)
					{
						continue;
					}

					if (string.IsNullOrEmpty(lineItem.DestRegistrationID1) == false)
					{
						if (this.SiteManager.Site.PromptForTractorOrTanker)
						{
							if (this.TractorOrTanker == null || lineItem.DestRegistrationID1 != this.TractorOrTanker.ID)
							{
								continue;
							}
						}

						else if (this.SiteManager.Site.PromptForFirstTrailer)
						{
							if (this.Trailer1 == null || lineItem.DestRegistrationID1 != this.Trailer1.ID)
							{
								continue;
							}
						}

						else
						{
							continue;
						}
					}

					if (string.IsNullOrEmpty(lineItem.DestRegistrationID2) == false)
					{
						if (this.SiteManager.Site.PromptForTractorOrTanker)
						{
							if (this.Trailer1 == null || lineItem.DestRegistrationID2 != this.Trailer1.ID)
							{
								continue;
							}
						}

						else
						{
							if (this.Trailer2 == null || lineItem.DestRegistrationID2 != this.Trailer2.ID)
							{
								continue;
							}
						}
					}

					if (string.IsNullOrEmpty(lineItem.DestRegistrationID3) == false)
					{
						if (this.Trailer2 == null || lineItem.DestRegistrationID3 != this.Trailer2.ID)
						{
							continue;
						}
					}

					foreach (CompanyMapClass authorizedShipTo in this.Carrier.CarrierCustomerShipToCollection)
					{
						if (authorizedShipTo.AssignedToID == lineItem.ShipToID)
						{
							this.Orders = true;
							break;
						}
					}

					if (this.Orders)
					{
						break;
					}
				}
			}

			if (this.Orders)
			{
				if (this.Station.InhibitLoadingByLoadID)
				{
					this.ProcessUseOrder(this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, "LoadRack|Yes"));
				}
				else
				{
					this.StationState = StationState.USE_ORDER_PROMPT;
					DisplayMenuParameters parameters = new DisplayMenuParameters(
						"Use Order Number?", new[] { "LoadRack|Yes", "LoadRack|No" }, true, -1, this.PROMPT_TIMEOUT);
					this.DisplayMenu(parameters);
				}
			}
			else
			{
				this.IssueLoadIDPrompt();
			}
		}

		protected void IssueVerifySupplierMenu()
		{
			string[] menuItems = { "LoadRack|Yes", "LoadRack|No" };

			string supplierDisplayText = this.Supplier.ID;

			DisplayMenuParameters parameters = new DisplayMenuParameters(
				"[LoadRack|Supplier] - " + supplierDisplayText, menuItems, true, -1, this.PROMPT_TIMEOUT);
			this.DisplayMenu(parameters);

			this.StationState = StationState.VERIFY_OFFLOAD_SUPPLIER;
		}

		protected void PrintTransactions()
		{
			StopWatch timer1 = new StopWatch(StopWatch.Appnames.LoadRackService, "PrintTransactions");
			try
			{
				DateTimeOffset now = TimeConverter.Now(this.SiteManager.Site);

				GetTransactionSR getInProgressTransactionSR = new GetTransactionSR
				{
					Security = this.Security,
					Request = GetTransactionRequest.SITE_TYPEID_TRANSDATE_STATUS_OPERATORPERSONNELGUID,
					Site = this.SiteManager.Site.ID,
					TransTypeID = TransactionTypes.T5_PrimaryDisbursement,
					BeginningDate = now.AddMinutes(-this.Station.BOLAgeInMinutes),
					EndingDate = now,
					OperatorPersonnelGuid = this.Driver.MasterRecordGuid,
					Status = ((int)TransactionStatus.InProgress).ToString()
				};

				GetTransactionDO getInProgressTransactionDO = FMChannelHelper.MakeCall<IGetTransactionProcessor, GetTransactionDO>(
																					  x =>
																					  x.Process(getInProgressTransactionSR)
																				);

				GetTransactionSR getCompletedTransactionSR = new GetTransactionSR
				{
					Security = this.Security,
					Request = GetTransactionRequest.SITE_TYPEID_TRANSDATE_STATUS_OPERATORPERSONNELGUID,
					Site = this.SiteManager.Site.ID,
					TransTypeID = TransactionTypes.T5_PrimaryDisbursement,
					BeginningDate = now.AddMinutes(-this.Station.BOLAgeInMinutes),
					EndingDate = now,
					OperatorPersonnelGuid = this.Driver.MasterRecordGuid,
					Status = ((int)TransactionStatus.Completed).ToString()
				};

				GetTransactionDO getCompletedTransactionDO = FMChannelHelper.MakeCall<IGetTransactionProcessor, GetTransactionDO>(
																	x =>
																	x.Process(getCompletedTransactionSR)
															  );
				GetTransactionSR getPostedTransactionSR = new GetTransactionSR
				{
					Security = this.Security,
					Request = GetTransactionRequest.SITE_TYPEID_TRANSDATE_STATUS_OPERATORPERSONNELGUID,
					Site = this.SiteManager.Site.ID,
					TransTypeID = TransactionTypes.T5_PrimaryDisbursement,
					BeginningDate = now.AddMinutes(-this.Station.BOLAgeInMinutes),
					EndingDate = now,
					OperatorPersonnelGuid = this.Driver.MasterRecordGuid,
					Status = ((int)TransactionStatus.Posted).ToString()
				};

				GetTransactionDO getPostedTransactionDO = FMChannelHelper.MakeCall<IGetTransactionProcessor, GetTransactionDO>(
																	x =>
																	x.Process(getPostedTransactionSR)
															  );
				// Print the Transactions
				if ((getInProgressTransactionDO?.TransactionDataSet != null
						  && getInProgressTransactionDO.TransactionDataSet.Tables.Count != 0
						  && getInProgressTransactionDO.TransactionDataSet.Tables[0].Rows.Count != 0)
				|| (getCompletedTransactionDO?.TransactionDataSet != null
						  && getCompletedTransactionDO.TransactionDataSet.Tables.Count != 0
						  && getCompletedTransactionDO.TransactionDataSet.Tables[0].Rows.Count != 0)
				|| (getPostedTransactionDO?.TransactionDataSet != null
						  && getPostedTransactionDO.TransactionDataSet.Tables.Count != 0
						  && getPostedTransactionDO.TransactionDataSet.Tables[0].Rows.Count != 0))
				{
					byte[] signature = null;

					if (this.Driver.OnFileSignature != null && this.Driver.OnFileSignature.Length > 0)
					{
						signature = this.Driver.OnFileSignature;
					}
					else
					{
						if (string.IsNullOrEmpty(this.Station.SignatureDevice) == false)
						{
							if (this.Station.InterfaceType != STATION_INTERFACE_TYPE.PROXIMITY_CARD_READER)
							{
								this.DisplayMessage("LoadRack|Follow Instructions on Signature Pad", "", 0, 999);
								this.StationState = StationState.SIGNATURE_CAPTURE;
							}

							SignatureCaptureClass signatureCapture = new SignatureCaptureClass(this.eventLog);
							signature = signatureCapture.Get(
								this.Station.SignatureDevice, this.Station.SignatureDevicePort, this.Station.SignatureDeviceBaudRate);
							if (signature == null)
							{
								this.StationState = StationState.IDLE;
								return;
							}
						}
					}

					if (getInProgressTransactionDO != null
					&& getInProgressTransactionDO.TransactionDataSet != null
					&& getInProgressTransactionDO.TransactionDataSet.Tables.Count != 0
					&& getInProgressTransactionDO.TransactionDataSet.Tables[0].Rows.Count != 0)
					{
						for (int index = 0; index < getInProgressTransactionDO.TransactionDataSet.Tables[0].Rows.Count; index++)
						{
							string transId = (string)getInProgressTransactionDO.TransactionDataSet.Tables[0].Rows[index]["TransID"];
							if (this.SiteManager.TransactionLoading(transId))
							{
								continue;
							}

							this.Transaction = this.GetTransaction(transId);
							if (this.Transaction != null)
							{
								this.Transaction.Signature = signature;

								// Update TimeOut if and only if it equals TimeEnd (implies that TimeOut was set at batch complete at AccuLoad)
								// OR if TimeOut is null (was never set) - Bug 20369
								// As times are actually measured in ticks (100 ns intervals), actually check that the two times differ by less than
								// one second.
								TimeSpan forwardOneSecond = new TimeSpan(0, 0, 1);
								TimeSpan backOneSecond = new TimeSpan(0, 0, -1);
								if (this.Transaction.TimeOut == null
												|| (this.Transaction.TimeEnd != null &&
										 this.Transaction.TimeOut.Value.Subtract(this.Transaction.TimeEnd.Value) < forwardOneSecond &&
										  this.Transaction.TimeOut.Value.Subtract(this.Transaction.TimeEnd.Value) > backOneSecond))
								{
									this.Transaction.TimeOut = now;
								}

								this.Transaction.Status = TransactionStatus.Completed;
								this.SaveTransaction(true);
								if (!string.IsNullOrEmpty(this.Transaction.TransRefID))
								{
									TransactionDO order = this.GetTransaction(this.Transaction.TransRefID);
									if (IsTransactionScheduledOrder(order) && order.Status == TransactionStatus.Scheduled)
									{
										order.Status = TransactionStatus.Completed;
										this.SaveArbitraryTransaction(order);
									}
								}

								this.PrintTransaction();
							}

							this.Transaction = null;
						}
					}

					if (getCompletedTransactionDO != null && getCompletedTransactionDO.TransactionDataSet != null
						 && getCompletedTransactionDO.TransactionDataSet.Tables.Count != 0
						 && getCompletedTransactionDO.TransactionDataSet.Tables[0].Rows.Count != 0)
					{
						for (int index = 0; index < getCompletedTransactionDO.TransactionDataSet.Tables[0].Rows.Count; index++)
						{
							this.Transaction =
								this.GetTransaction((string)getCompletedTransactionDO.TransactionDataSet.Tables[0].Rows[index]["TransID"]);
							if (this.Transaction != null)
							{
								this.Transaction.Signature = signature;

								// Update TimeOut if and only if it equals TimeEnd (implies that TimeOut was set at batch complete at AccuLoad)
								// OR if TimeOut is null (was never set) - Bug 20369
								// As times are actually measured in ticks (100 ns intervals), actually check that the two times differ by less than
								// one second.
								TimeSpan forwardOneSecond = new TimeSpan(0, 0, 1);
								TimeSpan backOneSecond = new TimeSpan(0, 0, -1);
								if (this.Transaction.TimeOut == null
									 || (this.Transaction.TimeEnd != null &&
									 this.Transaction.TimeOut.Value.Subtract(this.Transaction.TimeEnd.Value) < forwardOneSecond &&
									  this.Transaction.TimeOut.Value.Subtract(this.Transaction.TimeEnd.Value) > backOneSecond))
								{
									this.Transaction.TimeOut = now;
								}
								this.SaveTransaction();
								this.PrintTransaction();
							}

							this.Transaction = null;
						}
					}

					if (getPostedTransactionDO != null && getPostedTransactionDO.TransactionDataSet != null
						 && getPostedTransactionDO.TransactionDataSet.Tables.Count != 0
						 && getPostedTransactionDO.TransactionDataSet.Tables[0].Rows.Count != 0)
					{
						for (int index = 0; index < getPostedTransactionDO.TransactionDataSet.Tables[0].Rows.Count; index++)
						{
							this.Transaction =
								this.GetTransaction((string)getPostedTransactionDO.TransactionDataSet.Tables[0].Rows[index]["TransID"]);
							if (this.Transaction != null)
							{
								this.Transaction.Signature = signature;
								if (this.Transaction.TimeOut == null)
								{
									this.Transaction.TimeOut = now;
								}
								this.SaveTransaction();
								this.PrintTransaction();
							}

							this.Transaction = null;
						}
					}

					this.StationState = StationState.IDLE;
				}

				else
				{
					this.StationState = StationState.RESET_ON_TIMEOUT;
					this.DisplayMessage("LoadRack|No Transactions", "", 0, this.MESSAGE_TIMEOUT);
				}
			}

			finally
			{
				timer1.Stop();
			}
		}

		protected void ProcessAdditionalOrders(string response)
		{
			if (response == EscapeString)
			{
				this.IssueLoadSummaryPrompt();
			}
			else
			{
				// Remove LineItems for which no product was selected
				for (int Index = this.Transaction.LineItems.Count; Index > 0; Index--)
				{
					LineItemDO LineItem = this.Transaction.LineItems[Index - 1];
					if (LineItem.Product == null)
					{
						this.Transaction.LineItems.RemoveAt(Index - 1);
					}
				}

				try
				{
					this.SaveTransaction();
				}
				catch (Exception e)
				{
					this.DisplayMessage(e.Message, null, 0, this.MESSAGE_TIMEOUT);
					this.StationState = StationState.RESET_ON_TIMEOUT;
					return;
				}

				if (this.Station.Type == STATION_TYPE.PRELOAD || this.Station.Type == STATION_TYPE.WEIGHT_SCALE)
				{
					this.PrintPreload();
				}

				this.PendingTransactions.Add(this.Transaction);
				this.Transaction = null;

				if (this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, "LoadRack|Yes") == response)
				{
					this.IssueUseOrderNumberPrompt();
				}

				else if (this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, "LoadRack|No") == response)
				{
					this.CheckEntryInstructions(false);
				}

				else
				{
					this.StationState = StationState.IDLE;
					this.DisplayMessage("Unknown Additional Orders Response", null, 0, this.MESSAGE_TIMEOUT);
				}
			}
		}

		protected virtual void ProcessBolNumber(string response)
		{
			if (response == EscapeString)
			{
				if (this.Station.OffLoadByOffLoadID || this.UseOffLoadSupplyOrders == false)
				{
					this.DisplayOffLoadProductSelect();
				}
				else
				{
					this.DisplayVerifySupplyOrderProduct();
				}
				return;
			}
			else if (response == "")
			{
				PromptForBOLNumber();
				return;
			}

			this.SelectedBOLNumber = response; // Not really sure why we keep this separate, then set the tranaction document number with this 
															// later on DET/RCU paths, but completely lose it on Accuload/Microload paths
															// Bears further investigation, but don't have the bandwidth for it now.
			this.Transaction.DocumentNumber = response;
			this.PromptForOffLoadDensity();
		}

		protected void ProcessCancelTransactionPrompt(string Response)
		{
			if (Response != null)
			{
				if (Response == "1")
				{
					this.CompleteTransaction();
					this.ResetStationDevice();
				}
				else
				{
					this.StationState = this.PriorStationState;

					if (this.CurrentMenuParameters == null)
					{
						if (this.PriorStationState == StationState.PIN_PROMPT)
						{
							this.IssuePromptForPIN();
						}
						else
						{
							this.DisplayMessage(this.PriorStockMessage, null, this.PriorResponseLength, this.PriorMessageTimeout);
						}
					}
					else
					{
						this.DisplayMenu(this.CurrentMenuParameters);
					}
				}
			}
		}

		protected void ProcessCompanyHierarchy(string Response)
		{
			if (Response == EscapeString)
			{
				switch (this.CurrentCompanyHierarchyType)
				{
					case COMPANY_MAP_TYPE.SHIPTO_BILLTO_MAP:
						this.IssueSelectCustomerShipToFilterColumnPrompt();
						break;

					case COMPANY_MAP_TYPE.BILLTO_SHIPPER_MAP:
						{
							this.CurrentCompanyHierarchyType = COMPANY_MAP_TYPE.SHIPTO_BILLTO_MAP;
							string[] CompaniesList = this.HeirarchicalCompanyList();
							if (CompaniesList.Length > 1)
							{
								this.IssueSelectCompanyHierarchyPrompt();
							}
							else
							{
								this.ProcessCompanyHierarchy(Response);
							}
							break;
						}

					case COMPANY_MAP_TYPE.SHIPPER_OWNER_MAP:
						{
							this.CurrentCompanyHierarchyType = COMPANY_MAP_TYPE.BILLTO_SHIPPER_MAP;
							string[] CompaniesList = this.HeirarchicalCompanyList();
							if (CompaniesList.Length > 1)
							{
								this.IssueSelectCompanyHierarchyPrompt();
							}
							else
							{
								this.ProcessCompanyHierarchy(Response);
							}
							break;
						}

					case COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP:
						{
							this.CurrentCompanyHierarchyType = COMPANY_MAP_TYPE.SHIPPER_OWNER_MAP;
							string[] CompaniesList = this.HeirarchicalCompanyList();
							if (CompaniesList.Length > 1)
							{
								this.IssueSelectCompanyHierarchyPrompt();
							}
							else
							{
								this.ProcessCompanyHierarchy(Response);
							}
							break;
						}

					case COMPANY_MAP_TYPE.SUPPLIER_OWNER_MAP:
						this.IssueSelectSupplierOffLoadIDFilterColumnPrompt();
						break;

					case COMPANY_MAP_TYPE.OFFLOAD_OWNER_MANAGER_MAP:
						{
							this.CurrentCompanyHierarchyType = COMPANY_MAP_TYPE.SUPPLIER_OWNER_MAP;
							string[] CompaniesList = this.HeirarchicalCompanyList();
							if (CompaniesList.Length > 1)
							{
								this.IssueSelectCompanyHierarchyPrompt();
							}
							else
							{
								this.ProcessCompanyHierarchy(Response);
							}
							break;
						}

					default:
						throw new Exception("Invalid Company Map Type");
				}
			}

			else
			{
				foreach (CompanyClass Company in this.HierarchyCompanyCollection)
				{
					if (Company.CompanyToolTip != Response)
					{
						continue;
					}

					switch (this.CurrentCompanyHierarchyType)
					{
						case COMPANY_MAP_TYPE.SHIPTO_BILLTO_MAP:
							this.BillTo = Company;
							if (!this.ValidateCompany(this.BillTo, COMPANY_ROLE.CUSTOMER_BILLTO))
							{
								return;
							}
							this.CurrentCompanyHierarchyType = COMPANY_MAP_TYPE.BILLTO_SHIPPER_MAP;
							this.IssueSelectCompanyHierarchyPrompt();
							return;

						case COMPANY_MAP_TYPE.BILLTO_SHIPPER_MAP:
							this.Shipper = Company;
							if (!this.ValidateCompany(this.Shipper, COMPANY_ROLE.SHIPPER))
							{
								return;
							}
							this.CurrentCompanyHierarchyType = COMPANY_MAP_TYPE.SHIPPER_OWNER_MAP;
							this.IssueSelectCompanyHierarchyPrompt();
							return;

						case COMPANY_MAP_TYPE.SHIPPER_OWNER_MAP:
							this.Owner = Company;
							if (!this.ValidateCompany(this.Owner, COMPANY_ROLE.OWNER))
							{
								return;
							}
							this.CurrentCompanyHierarchyType = COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP;
							this.IssueSelectCompanyHierarchyPrompt();
							return;

						case COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP:
							{
								this.Manager = Company;
								if (!this.ValidateCompany(this.Manager, COMPANY_ROLE.MANAGER))
								{
									return;
								}

								// Retrieve the Company Hierarchy
								Guid ownerManagerMapGuid = this.GetIdentityGuidByGuidsAndTypeForCompanyMap(
									this.Security, this.Manager.MasterRecordGuid, this.Owner.MasterRecordGuid, COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP);
								Guid shipperOwnerMapGuid = this.GetIdentityGuidByGuidsAndTypeForCompanyMap(
									this.Security, ownerManagerMapGuid, this.Shipper.MasterRecordGuid, COMPANY_MAP_TYPE.SHIPPER_OWNER_MAP);
								Guid billToShipperMapGuid = this.GetIdentityGuidByGuidsAndTypeForCompanyMap(
									this.Security, shipperOwnerMapGuid, this.BillTo.MasterRecordGuid, COMPANY_MAP_TYPE.BILLTO_SHIPPER_MAP);
								Guid shipToBillToMapGuid = this.GetIdentityGuidByGuidsAndTypeForCompanyMap(
									this.Security, billToShipperMapGuid, this.ShipTo.MasterRecordGuid, COMPANY_MAP_TYPE.SHIPTO_BILLTO_MAP);

								Guid shipToAllocationGuid = this.GetIdentityGuidForAllocations(this.Security, shipToBillToMapGuid, DateTimeOffset.Now,
									DateTimeOffset.Now, COMPANY_MAP_TYPE.SHIPTO_BILLTO_MAP);
								if (shipToAllocationGuid != Guid.Empty)
								{
									this.AllocationArray[0] = this.GetBySiteGuidForAllocations(
										this.Security, shipToAllocationGuid, this.SiteManager.Site.IdentityGuid, this.Station.Type, "");
								}
								else
								{
									this.AllocationArray[0] = null;
								}

								Guid billToAllocationGuid = this.GetIdentityGuidForAllocations(
									this.Security,
									billToShipperMapGuid,
									DateTimeOffset.Now,
									DateTimeOffset.Now,
									COMPANY_MAP_TYPE.BILLTO_SHIPPER_MAP);
								if (billToAllocationGuid != Guid.Empty)
								{
									this.AllocationArray[1] = this.GetBySiteGuidForAllocations(
										this.Security, billToAllocationGuid, this.SiteManager.Site.IdentityGuid, this.Station.Type, "");
								}
								else
								{
									this.AllocationArray[1] = null;
								}

								Guid shipperAllocationGuid = this.GetIdentityGuidForAllocations(
									this.Security, shipperOwnerMapGuid, DateTimeOffset.Now, DateTimeOffset.Now, COMPANY_MAP_TYPE.SHIPPER_OWNER_MAP);
								if (shipperAllocationGuid != Guid.Empty)
								{
									this.AllocationArray[2] = this.GetBySiteGuidForAllocations(
										this.Security, shipperAllocationGuid, this.SiteManager.Site.IdentityGuid, this.Station.Type, "");
								}
								else
								{
									this.AllocationArray[2] = null;
								}

								Guid ownerAllocationGuid = this.GetIdentityGuidForAllocations(
									this.Security,
									ownerManagerMapGuid,
									DateTimeOffset.Now,
									DateTimeOffset.Now,
									COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP);
								if (ownerAllocationGuid != Guid.Empty)
								{
									this.AllocationArray[3] = this.GetBySiteGuidForAllocations(
										this.Security, ownerAllocationGuid, this.SiteManager.Site.IdentityGuid, this.Station.Type, "");
								}
								else
								{
									this.AllocationArray[3] = null;
								}

								this.IssueEnterPurchaseOrderPrompt();
								return;
							}

						case COMPANY_MAP_TYPE.SUPPLIER_OWNER_MAP:
							this.Owner = Company;
							if (!this.ValidateCompany(this.Owner, COMPANY_ROLE.OWNER))
							{
								return;
							}
							this.CurrentCompanyHierarchyType = COMPANY_MAP_TYPE.OFFLOAD_OWNER_MANAGER_MAP;
							this.IssueSelectCompanyHierarchyPrompt();
							return;

						case COMPANY_MAP_TYPE.OFFLOAD_OWNER_MANAGER_MAP:
							{
								this.Manager = Company;
								if (!this.ValidateCompany(this.Manager, COMPANY_ROLE.MANAGER))
								{
									return;
								}

								if (!this.ValidateCompany(this.Owner, COMPANY_ROLE.OWNER))
								{
									return;
								}

								if (!this.ValidateCompany(this.Supplier, COMPANY_ROLE.SUPPLIER))
								{
									return;
								}

								// initialize the transaction here before product select
								this.CurrentTransactionAlias = this.GetTransactionAlias(
									this.Security, this.Station.ReceiptByVolumeTransactionAliasGuid, false);
								this.InitializeTransaction();

								if (this.Station.OffLoadByOffLoadID || this.UseOffLoadSupplyOrders == false)
								{
									this.DisplayOffLoadProductSelect();
								}
								else
								{
									this.DisplayVerifySupplyOrderProduct();
								}
								break;
							}

						default:
							throw new Exception("Invalid Company Map Type");
					}
				}
			}
		}

		protected TransactionAliasClass GetTransactionAlias(SecurityClass securityClass, Guid guid, bool param1)
		{
			return FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasClass>(
																	 x =>
																	 x.Get(securityClass, guid, param1)
																);
		}

		private AllocationClass GetBySiteGuidForAllocations(SecurityClass securityClass, Guid shipToAllocationGuid, Guid guid,
			STATION_TYPE stationType, string param1)
		{
			return FMChannelHelper.MakeCall<IAllocations, AllocationClass>(
																	 x =>
																	 x.GetBySiteGuid(securityClass, shipToAllocationGuid, guid, stationType, param1)
																);
		}

		private Guid GetIdentityGuidForAllocations(SecurityClass securityClass, Guid shipToBillToMapGuid, DateTimeOffset dateTimeOffset1,
			DateTimeOffset dateTimeOffset2, COMPANY_MAP_TYPE companyMapType)
		{
			return FMChannelHelper.MakeCall<IAllocations, Guid>(
																	 x =>
																	 x.GetIdentityGuid(securityClass, shipToBillToMapGuid, dateTimeOffset1,
																		dateTimeOffset2, companyMapType)
																);
		}

		private Guid GetIdentityGuidByGuidsAndTypeForCompanyMap(SecurityClass securityClass, Guid guid1, Guid guid2, COMPANY_MAP_TYPE companyMapType)
		{
			return FMChannelHelper.MakeCall<ICompanyMaps, Guid>(
																	 x =>
																	 x.GetIdentityGuidByGuidsAndType(securityClass, guid1, guid2, companyMapType)
																);
		}

		protected void ProcessCompartmentSummary(string Response)
		{
			if (Response == EscapeString)
			{
				if (this.LoadSummaryIssued)
				{
					this.IssueLoadSummaryPrompt();
				}

				else if (this.CurrentEquipment == this.Trailer2 && this.AvailableCompartments(this.Trailer1) > 0)
				{
					this.CurrentEquipment = this.Trailer1;
					this.IssueCompartmentSummaryPrompt();
				}

				else if (this.CurrentEquipment == this.Trailer1 && this.AvailableCompartments(this.TractorOrTanker) > 0)
				{
					this.CurrentEquipment = this.TractorOrTanker;
					this.IssueCompartmentSummaryPrompt();
				}

				else if (this.ShipTo.PurchaseOrderRequired)
				{
					this.IssueEnterPurchaseOrderPrompt();
				}

				else if (this.Order != null)
				{
					if (this.SiteManager.Site.PromptForShipmentNumber)
					{
						this.IssueEnterShipmentNumberPrompt();
					}
					else
					{
						this.IssueEnterOrderNumberPrompt();
					}
				}
				else
				{
					this.IssueLoadIDPrompt();
				}
			}

			else
			{
				if (Response.StartsWith(this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, "LoadRack|Accept")))
				{
					if (this.LoadSummaryIssued)
					{
						this.IssueLoadSummaryPrompt();
					}

					else if (this.CurrentEquipment == this.TractorOrTanker && this.AvailableCompartments(this.Trailer1) > 0)
					{
						this.CurrentEquipment = this.Trailer1;
						this.IssueCompartmentSummaryPrompt();
					}

					else if (this.CurrentEquipment == this.Trailer1 && this.AvailableCompartments(this.Trailer2) > 0)
					{
						this.CurrentEquipment = this.Trailer2;
						this.IssueCompartmentSummaryPrompt();
					}

					else
					{
						this.IssueLoadSummaryPrompt();
					}
				}

				else
				{
					char[] Delimiter = { ' ' };
					string[] Index = Response.Split(Delimiter, 2);
					this.CurrentCompartmentNumber = System.Convert.ToInt32(Index[0]);
					this.IssueSelectProductPrompt();
				}
			}
		}

		protected void ProcessCompartmentsEmptyPromptResponse(string Response)
		{
			ProductMapClass AuthorizedProduct = this.ShipTo.AuthorizedProductCollection[this.AuthorizedProductIndex];
			ContaminationPromptStatus contaminationPromptStatus =
				this.GetContaminationPromptStatus(AuthorizedProduct.ContaminationPromptLoadRackText);

			try
			{
				int nSelection = System.Convert.ToInt32(Response);

				if (nSelection == 0 || nSelection > this.CurrentMenuParameters.Menu.Length)
				{
					this.IssueContaminationPrompt(contaminationPromptStatus);
					return;
				}

				Response = this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, this.CurrentMenuParameters.Menu[nSelection - 1]);
			}
			catch
			{
			}

			if (Response == EscapeString)
			{
				contaminationPromptStatus.CompartmentsEmpty = null;
				AuthorizedProduct.LockedOut = false;
				this.IssueContaminationPrompt(contaminationPromptStatus);
				return;
			}

			else
			{
				this.AddAlarmAndEventLogs(
					this.Security,
					this.Station.ComparmentEmptyInquiryEvent(
						this.Driver.ID, contaminationPromptStatus.ContaminationPromptLoadRackText, Response));

				if (this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, "LoadRack|Yes") == Response)
				{
					contaminationPromptStatus.CompartmentsEmpty = true;
					this.CheckProductContamination();
				}

				else
				{
					contaminationPromptStatus.CompartmentsEmpty = false;
					AuthorizedProduct.LockedOut = true;
					this.CheckProductContamination();
				}
			}
		}

		protected void ProcessCompartmentsNotConfiguredPrompt()
		{
			// Don't really care what we get here
			this.ReleaseKeyPad();
			this.StationState = StationState.IDLE;
		}

		protected void ProcessCompartmentsPreviouslyLoadedPromptResponse(string Response)
		{
			ProductMapClass AuthorizedProduct = this.ShipTo.AuthorizedProductCollection[this.AuthorizedProductIndex];
			ContaminationPromptStatus contaminationPromptStatus =
				this.GetContaminationPromptStatus(AuthorizedProduct.ContaminationPromptLoadRackText);

			try
			{
				int nSelection = System.Convert.ToInt32(Response);

				if (nSelection == 0 || nSelection > this.CurrentMenuParameters.Menu.Length)
				{
					this.IssueContaminationPrompt(contaminationPromptStatus);
					return;
				}

				Response = this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, this.CurrentMenuParameters.Menu[nSelection - 1]);
			}
			catch
			{
			}

			if (Response == EscapeString)
			{
				contaminationPromptStatus.CompartmentsPreviouslyLoaded = null;
				this.IssueContaminationPrompt(contaminationPromptStatus);
				return;
			}

			else
			{
				this.AddAlarmAndEventLogs(
					this.Security,
					this.Station.PreviouslyLoadedInquiryEvent(
						this.Driver.ID, contaminationPromptStatus.ContaminationPromptLoadRackText, Response));

				if (this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, "LoadRack|Yes") == Response)
				{
					contaminationPromptStatus.CompartmentsPreviouslyLoaded = true;
					this.CheckProductContamination();
				}

				else
				{
					contaminationPromptStatus.CompartmentsPreviouslyLoaded = false;
					this.IssueCompartmentsEmptyPrompt(contaminationPromptStatus);
				}
			}
		}

		protected void ProcessContaminationPromptResponse(string Response)
		{
			ProductMapClass AuthorizedProduct = this.ShipTo.AuthorizedProductCollection[this.AuthorizedProductIndex];
			ContaminationPromptStatus contaminationPromptStatus =
				this.GetContaminationPromptStatus(AuthorizedProduct.ContaminationPromptLoadRackText);

			try
			{
				int nSelection = System.Convert.ToInt32(Response);

				if (nSelection == 0 || nSelection > this.CurrentMenuParameters.Menu.Length)
				{
					this.IssueContaminationPrompt(contaminationPromptStatus);
					return;
				}

				Response = this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, this.CurrentMenuParameters.Menu[nSelection - 1]);
			}
			catch
			{
			}

			if (Response == EscapeString)
			{
				if (this.Station.Type == STATION_TYPE.LOAD_RACK)
				{
					this.IssueLoadIDPrompt();
				}

				else
				{
					if (this.Order != null)
					{
						if (this.SiteManager.Site.PromptForShipmentNumber)
						{
							this.IssueEnterShipmentNumberPrompt();
						}
						else
						{
							this.IssueEnterOrderNumberPrompt();
						}
					}
					else
					{
						this.IssueLoadIDPrompt();
					}
				}
			}

			else
			{
				this.AddAlarmAndEventLogs(
					this.Security,
					this.Station.LoadingInquiryEvent(
						this.Driver.ID, contaminationPromptStatus.ContaminationPromptLoadRackText, Response));

				if (this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, "LoadRack|Yes") == Response)
				{
					contaminationPromptStatus.ContaminatePrompt = true;

					this.IssueCompartmentsPreviouslyLoadedPrompt(contaminationPromptStatus);
				}
				else
				{
					contaminationPromptStatus.ContaminatePrompt = false;
					AuthorizedProduct.LockedOut = true;
					this.CheckProductContamination();
				}
			}
		}

		protected void ProcessCustomerShipTo(string Response)
		{
			if (EscapeString == Response)
			{
				if (this.PriorStationState == StationState.SELECT_CUSTOMER_SHIPTO_FILTER_PROMPT)
				{
					this.IssueSelectCustomerShipToFilterColumnPrompt();
				}
				else
				{
					this.IssueSelectCustomerShipToFilterValuePrompt();
				}
			}

			else
			{
				Guid identityGuid = Guid.Empty;
				foreach (CompanyMapClass ShipToMap in this.Carrier.CarrierCustomerShipToCollection)
				{
					if (ShipToMap.AssignedToToolTip == Response)
					{
						identityGuid = ShipToMap.AssignedToGuid;
						break;
					}
				}

				if (identityGuid == Guid.Empty)
				{
					this.DisplayMessage("[LoadRack|Invalid], [LoadRack|Enter] [LoadRack|Ship To]", null, 20, this.MESSAGE_TIMEOUT);
					return;
				}

				this.ShipTo = FMChannelHelper.MakeCall<ICompanies, CompanyClass>(
																	 x =>
																	 x.Get(this.Security, identityGuid)
																);

				if (!this.ValidateCompany(this.ShipTo, COMPANY_ROLE.CUSTOMER_SHIPTO))
				{
					return;
				}

				this.CompanyMapCollection = FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapCollectionClass>(
						x =>
						x.EnumerateByAssignedGuidAndType(this.Security, this.ShipTo.MasterRecordGuid, COMPANY_MAP_TYPE.SHIPTO_BILLTO_MAP)
				);

				this.CurrentCompanyHierarchyType = COMPANY_MAP_TYPE.SHIPTO_BILLTO_MAP;
				this.IssueSelectCompanyHierarchyPrompt();
			}
		}

		protected void ProcessCustomerShipToFilterColumn(string Response)
		{
			if (Response == EscapeString)
			{
				this.IssueLoadIDPrompt();
			}

			else
			{
				this.PriorStationState = this.StationState;
				if (this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, "[LoadRack|Zip]") == Response)
				{
					this.CustomerShipToFilterColumn = "Zip";
					this.IssueEnterZipPrompt();
				}
				else if (this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, "LoadRack|Destination]") == Response)
				{
					this.IssueSelectCustomerShipToFromCarrierShipToCollectionPrompt();
				}

				else
				{
					if (this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, "[LoadRack|City]") == Response)
					{
						this.CustomerShipToFilterColumn = "City";
					}
					else if (this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, "[LoadRack|State]") == Response)
					{
						this.CustomerShipToFilterColumn = "State";
					}
					this.IssueSelectCustomerShipToFilterValuePrompt();
				}
			}
		}

		protected void ProcessCustomerShipToFilterValue(string response)
		{
			if (response == EscapeString)
			{
				if (this.PriorStationState == StationState.ENTER_ZIP_PROMPT)
				{
					this.IssueEnterZipPrompt();
				}
				else
				{
					this.IssueSelectCustomerShipToFilterColumnPrompt();
				}
			}

			else if (response == "?")
			{
				this.IssueSelectCustomerShipToFilterValuePrompt();
			}

			else
			{
				this.PriorStationState = this.StationState;
				this.CustomerShipToFilterValue = response;
				this.IssueSelectCustomerShipToFromFilterPrompt();
			}
		}

		protected virtual void ProcessDriverID(string response)
		{
			StopWatch timer = new StopWatch(StopWatch.Appnames.LoadRackService, "ProcessDriverID");
			timer.Perform("ProcessDriverID");
			try
			{
				this.Driver = null;

				this.CardID = response;

				Guid personGuid = FMChannelHelper.MakeCall<IPersonnel, Guid>(
																					  x =>
																					  x.GetGuidByCardNumber(this.Security, this.CardID)
																				);

				// If we did not find the ID and we are supporting 35-bit cards
				if (personGuid.IsEmpty() && this.Station.CardReader && this.Station.ThirtyFiveBitCardSupport)
				{
					this.CardID = this.CardID.Substring(1, this.CardID.Length - 1);
					personGuid = FMChannelHelper.MakeCall<IPersonnel, Guid>(
																					 x =>
																					 x.GetGuidByCardNumber(this.Security, this.CardID)
																			  );
				}

				if (personGuid.IsEmpty() && this.Station.CardReader && !this.Station.ThirtyFiveBitCardSupport && this.CardID.Length > 6)
				{
					this.CardID = this.CardID.Remove(0, this.CardID.Length - 6);
					personGuid = FMChannelHelper.MakeCall<IPersonnel, Guid>(
																					 x =>
																					 x.GetGuidByCardNumber(this.Security, this.CardID)
																					 );
				};

				if (personGuid.IsEmpty() && this.Station.CardReader)
				{
					this.CardID = response;
					personGuid = FMChannelHelper.MakeCall<IHouseCards, Guid>(
																					 x =>
																					 x.GetIdentityGuidByNumber(this.Security, this.CardID)
																			  );
					// If we did not find the ID and we are supporting 35-bit cards
					if (personGuid.IsEmpty() && this.Station.ThirtyFiveBitCardSupport)
					{
						this.CardID = this.CardID.Substring(1, this.CardID.Length - 1);
						personGuid = FMChannelHelper.MakeCall<IHouseCards, Guid>(
																					x =>
																					x.GetIdentityGuidByNumber(this.Security, this.CardID)
																			 );
					}

					if (personGuid.IsEmpty() && !this.Station.ThirtyFiveBitCardSupport && this.CardID.Length > 6)
					{
						this.CardID = this.CardID.Remove(0, this.CardID.Length - 6);
						personGuid = FMChannelHelper.MakeCall<IHouseCards, Guid>(
																					x =>
																					x.GetIdentityGuidByNumber(this.Security, this.CardID)
																					);
					}

					if (!personGuid.IsEmpty())
					{
						// ReSharper disable once AccessToModifiedClosure
						HouseCardClass houseCard = FMChannelHelper.MakeCall<IHouseCards, HouseCardClass>(
																					x =>
																					x.Get(this.Security, personGuid)
																			 );

						if (houseCard.DriverGuid.IsEmpty())
						{
							this.AddAlarmAndEventLogs(this.Security, this.Station.InvalidHouseCardNumberEvent(response));
							this.DisplayMessage("[LoadRack|Card Unassigned]", null, 0, this.MESSAGE_TIMEOUT);
							this.StationState = StationState.RESET_ON_TIMEOUT;
							return;
						}

						personGuid = houseCard.DriverGuid;
					}
				}

				if (personGuid.IsEmpty() && this.SiteManager.Site.UseShortCardNumber && !this.Station.CardReader)
				{
					this.CardID = response;
					personGuid = FMChannelHelper.MakeCall<IPersonnel, Guid>(
																					 x =>
																					 x.GetGuidByShortCardNumber(this.Security, this.CardID)
																			  );
				}

				if (personGuid.IsEmpty())
				{
					if (this.Station.CardReader)
					{
						this.AddAlarmAndEventLogs(this.Security, this.Station.InvalidCardNumberEvent(this.CardID));
						this.DisplayMessage("[LoadRack|Invalid Card Number]", null, 0, this.MESSAGE_TIMEOUT);
					}
					else
					{
						this.AddAlarmAndEventLogs(this.Security, this.Station.InvalidDriverIDEvent(this.CardID));
						this.DisplayMessage("[LoadRack|Invalid ID]", null, 0, this.MESSAGE_TIMEOUT);
					}

					this.StationState = StationState.RESET_ON_TIMEOUT;
					return;
				}

				// If a load rack, check VRU.  If it's rules have been reached,
				// tell the driver, write events in the event log, and reset the station
				if (this.Station.Type == STATION_TYPE.LOAD_RACK)
				{
					try
					{
						if (FMChannelHelper.MakeCall<IVruTrackings, bool>(x => x.IsThresholdExceeded(this.Security)))
						{
							VRUTrackingCollectionClass vruTrackingCollection = FMChannelHelper.MakeCall<IVruTrackings, VRUTrackingCollectionClass>(x => x.EnumerateThresholdsExceeded(this.Security));
							foreach (VruTrackingClass vruRuleViolated in vruTrackingCollection)
							{
								FMChannelHelper.MakeCall<IAlarmAndEventLogs>(x => x.Add(this.Security, vruRuleViolated.VruThresholdExceededAlarm));
							}

							this.LoadRackManager.EventOrAlarmEvent.Set();
							this.DisplayMessage(
								 "LoadRack|Loading suspended due to throughput limit",
								 null,
								 0,
								 this.MESSAGE_TIMEOUT);
							this.StationState = StationState.RESET_ON_TIMEOUT;
							return;
						}
					}
					catch (ConsolidatedDAException)
					{
						this.eventLog.WriteEntry(
							 "Attempt to call VRU Support scripts failed.  Please check that they have been installed",
							 EventLogEntryType.Warning);
					}
				}

				// Initialize the load arms
				this.LoadArmManagerCollection.SetState(this, LOADARM_STATE.NORMAL);

				this.ConsecutivePrompts = 0;
				this.StationState = StationState.IDLE;

				if (this.Station.Type == STATION_TYPE.LOAD_RACK || this.Station.Type == STATION_TYPE.OFF_LOADING)
				{
					if (this.SiteManager.Site.InhibitLoadRackCardIns && this.SiteManager.CardedInAtLoadRack(personGuid))
					{
						this.Driver = FMChannelHelper.MakeCall<IPersonnel, PersonClass>(
																					x =>
																					x.Get(this.Security, personGuid)
																			 );

						this.AddAlarmAndEventLogs(this.Security, this.Driver.MultipleCardInAlarm);
						this.StationState = StationState.RESET_ON_TIMEOUT;
						this.DisplayMessage("LoadRack|Multiple Card-in", null, 0, this.MESSAGE_TIMEOUT);
						return;
					}
				}

				this.Driver = FMChannelHelper.MakeCall<IPersonnel, PersonClass>(
																					  x =>
																					  x.Get(this.Security, personGuid)
																				);

				// logged the carded in event
				if (this.Station.CardReader)
				{
					this.AddAlarmAndEventLogs(this.Security, this.Driver.CardPresentedEvent(this.Station.ID));
				}
				else
				{
					this.AddAlarmAndEventLogs(this.Security, this.Driver.DriverLoggedInEvent(this.Station.ID));
				}

				// Check if Driver is Locked Out
				if (this.Driver.LockedOut)
				{
					this.AddAlarmAndEventLogs(this.Security, this.Driver.DriverLockedOutAlarm(response));
					this.DisplayMessage("[LoadRack|Driver] [LoadRack|Locked Out]", null, 0, this.MESSAGE_TIMEOUT);
					this.StationState = StationState.RESET_ON_TIMEOUT;
					return;
				}

				// Check if Driver Access Schedule precludes access
				if (this.SiteManager.Site.InhibitAccessAfterHours)
				{
					DateTimeOffset now = TimeConverter.Now(this.SiteManager.Site);
					int scheduleIndex = (int)now.DayOfWeek;
					if (!this.Driver.AccessScheduleCollection[scheduleIndex].Enabled
						  || this.Driver.AccessScheduleCollection[scheduleIndex].OpeningTime.Value.TimeOfDay > now.TimeOfDay
						  || this.Driver.AccessScheduleCollection[scheduleIndex].ClosingTime.Value.TimeOfDay < now.TimeOfDay)
					{
						this.AddAlarmAndEventLogs(this.Security, this.Driver.AccessScheduleAlarm);
						this.DisplayMessage("[LoadRack|Driver Access Not Scheduled]", null, 0, this.MESSAGE_TIMEOUT);
						this.StationState = StationState.RESET_ON_TIMEOUT;
						return;
					}
				}

				if (this.Station.Type == STATION_TYPE.LOAD_RACK || this.Station.Type == STATION_TYPE.OFF_LOADING)
				{
					if (this.Station.Type == STATION_TYPE.LOAD_RACK && !this.Driver.HasRole(PERSON_ROLE.LOADER_ROLE))
					{
						this.AddAlarmAndEventLogs(this.Security, this.Driver.HaveLoaderRoleEvent(this.Station.ID));
						this.LoadRackManager.EventOrAlarmEvent.Set();

						this.DisplayMessage("LoadRack|Must have Loader Role", null, 0, this.MESSAGE_TIMEOUT);
						this.StationState = StationState.RESET_ON_TIMEOUT;
						return;
					}

					if (this.Station.Type == STATION_TYPE.OFF_LOADING && !this.Driver.HasRole(PERSON_ROLE.OFFLOADER_ROLE))
					{
						this.AddAlarmAndEventLogs(this.Security, this.Driver.HaveOffloaderRoleEvent(this.Station.ID));
						this.LoadRackManager.EventOrAlarmEvent.Set();

						this.DisplayMessage("LoadRack|Must have Offloader Role", null, 0, this.MESSAGE_TIMEOUT);
						this.StationState = StationState.RESET_ON_TIMEOUT;
						return;
					}

					// When driver is not carded in and there is an entry gate station
					if (this.SiteManager.Site.AccessCardInRequired && !this.Driver.CardedIn && this.SiteManager.AnyEntryGates)
					{
						this.AddAlarmAndEventLogs(this.Security, this.Driver.NotCardedInEvent(this.Station.ID));
						this.LoadRackManager.EventOrAlarmEvent.Set();
						this.DisplayMessage("LoadRack|Must Card In at Entry Gate", null, 0, this.MESSAGE_TIMEOUT);
						this.StationState = StationState.RESET_ON_TIMEOUT;
						return;
					}

					if (this.SiteManager.EndOfDayState != StateEndOfDay.Inactive)
					{
						this.DisplayMessage("LoadRack|Disabled due to End Of Day", null, 0, this.MESSAGE_TIMEOUT);
						this.StationState = StationState.RESET_ON_TIMEOUT;
						return;
					}

					if (this.SiteManager.EndOfMonthState != StateEndOfMonth.Inactive)
					{
						this.DisplayMessage("LoadRack|Disabled due to End Of Month", null, 0, this.MESSAGE_TIMEOUT);
						this.StationState = StationState.RESET_ON_TIMEOUT;
						return;
					}

					if (this.Station.Type == STATION_TYPE.LOAD_RACK)
					{
						if (this.Station.IssueByVolumeTransactionAliasGuid.IsEmpty())
						{
							this.AddAlarmAndEventLogs(this.Security, this.Driver.LoadTransAliasInvalidEvent(this.Station.ID));
							this.LoadRackManager.EventOrAlarmEvent.Set();

							this.DisplayMessage("LoadRack|Transaction Alias Invalid", null, 0, this.MESSAGE_TIMEOUT);
							this.StationState = StationState.RESET_ON_TIMEOUT;
							return;
						}
					}
					else if (this.Station.Type == STATION_TYPE.OFF_LOADING)
					{
						if (this.Station.ReceiptByVolumeTransactionAliasGuid.IsEmpty())
						{
							this.AddAlarmAndEventLogs(this.Security, this.Driver.OffLoadTransAliasInvalidEvent(this.Station.ID));
							this.LoadRackManager.EventOrAlarmEvent.Set();

							this.DisplayMessage("LoadRack|Transaction Alias Invalid", null, 0, this.MESSAGE_TIMEOUT);
							this.StationState = StationState.RESET_ON_TIMEOUT;
							return;
						}
					}
				}

				this.FinishDriverCarrierProcessing();
			}
			finally
			{
				timer.Stop();
			}
		}

		protected virtual void FinishDriverCarrierProcessing()
		{
			this.Manager = null;
			this.Owner = null;
			this.Shipper = null;
			this.BillTo = null;
			this.ShipTo = null;
			this.Carrier = null;
			this.TractorOrTanker = null;
			this.Trailer1 = null;
			this.Trailer2 = null;
			this.Trailer3 = null;
			this.Transaction = null;
			this.Order = null;
			this.PONumber = null;
			this.LoadID = null;
			this.ByWeight = false;
			this.ByWeightProduct = "";
			this.PendingTransactions.Clear();
			this.LoadArmManagerCollection.ClearRecipeMap(this);
			this.PIDXAuthorizationArray = null;
			this.PIDXProfileCompanyMapCollection = null;

			if (this.Driver.AssignedCompaniesCollection.Count > 1 &&
				 this.Driver.CompanyGuid.IsEmpty())
			{
				// more then one company so prompt the user for the company he is using
				this.IssueSelectCarrierCompany();
				return;
			}

			if (this.Driver.AssignedCompaniesCollection.Count == 1)
			{
				CompanyMapClass companyMap = this.Driver.AssignedCompaniesCollection[0];
				if (companyMap != null)
				{
					this.Driver.CompanyGuid = companyMap.AssignedGuid;
				}
			}

			if (!this.Driver.CompanyGuid.IsEmpty())
			{
				this.Carrier = FMChannelHelper.MakeCall<ICompanies, CompanyClass>(
														x =>
														x.GetCarrierForLoadRack(this.Security, this.Driver.CompanyGuid)
												);
			}

			// Drivers must be assigned to a Carrier
			if (this.Carrier == null && this.Driver.HasRole(PERSON_ROLE.LOADER_ROLE))
			{
				this.AddAlarmAndEventLogs(this.Security, this.Station.InvalidCarrierEvent(this.Driver.ID));
				this.DisplayMessage("[LoadRack|Invalid Carrier]", null, 0, this.MESSAGE_TIMEOUT);
				this.StationState = StationState.RESET_ON_TIMEOUT;
				return;
			}

			if (this.Carrier != null)
			{
				if (this.Carrier.LockedOut)
				{
					this.AddAlarmAndEventLogs(this.Security, this.Carrier.LockedOutAlarm);
					this.DisplayMessage(
						"[LoadRack|" + CompanyRoleMapClass.RoleID(COMPANY_ROLE.CARRIER) + "] [LoadRack|Locked Out]",
						null,
						0,
						this.MESSAGE_TIMEOUT);
					this.StationState = StationState.RESET_ON_TIMEOUT;
					return;
				}

				this.Carrier._LastActivityDate.Value = TimeConverter.Now(this.SiteManager.Site);
				FMChannelHelper.MakeCall<ICompanies>(
														x =>
														x.Modify(this.Security, DATA_TYPE.DYNAMIC, this.Carrier)
													);
			}

			// Prompt for PIN
			if (this.Station.InterfaceType != STATION_INTERFACE_TYPE.PROXIMITY_CARD_READER
					&& this.PinRequired)
			{
				this.IssuePromptForPIN();
				return;
			}

			if (this.Station.Type == STATION_TYPE.BOL)
			{
				this.CardIn();

				this.PrintTransactions();

				if (!this.SiteManager.AnyExitGates)
				{
					this.CardOut();
				}

				return;
			}

			this.StationState = StationState.IDLE;
			this.CompleteDriverProcessing(false);
		}

		protected void IssueSelectCarrierCompany()
		{
			// Build initial menu parameter set
			DisplayMenuParameters parameters = new DisplayMenuParameters
			{
				ApplyDataDictionary = true,
				DefaultItem = 0,
				MenuTimeout = this.PROMPT_TIMEOUT,
				Caption = "[LoadRack|Select Carrier]" + (this.NumericMenuSelection ? " [LoadRack|0=Cancel]" : string.Empty),
				Menu = new string[this.Driver.AssignedCompaniesCollection.Count],
				SaveForCancelProcessing = true
			};

			int item = 0;

			foreach (CompanyMapClass companyMap in this.Driver.AssignedCompaniesCollection)
			{
				parameters.Menu[item++] = companyMap.AssignedID;
			}

			this.StationState = StationState.SELECT_DRIVER_COMPANY;

			this.DisplayMenu(parameters);
		}

		protected virtual void ProcessEnterSupplyOrderNumber(string response)
		{
			SupplyOrderListDO supplyOrderListDO;

			bool bFound = false;
			if (response == EscapeString)
			{
				if (this.Station.CardReader)
				{
					this.IssuePleaseCardIn();
				}
				else if (this.Station.TouchKeyReader)
				{
					this.IssueTouchKeyPleaseCardIn();
				}
				else
				{
					this.IssueDriverIDPrompt();
				}

				return;
			}

			if ("?" == response) // list command received
			{
				supplyOrderListDO = this.GetSupplyOrders();

				ArrayList supplyOrder = new ArrayList();

				if (supplyOrderListDO != null && supplyOrderListDO.LineItems.Count > 0)
				{
					foreach (SupplyOrderListLineItemDO lineItem in supplyOrderListDO.LineItems)
					{
						{
							{
								supplyOrder.Add(lineItem.DocumentNumber + " - " + lineItem.SupplierID);
							}
						}
					}
				}

				if (supplyOrder.Count == 0)
				{
					this.DisplayMessage("LoadRack|No Supply Orders", null, 0, this.MESSAGE_TIMEOUT);
					return;
				}

				this.StationState = StationState.ENTER_SUPPLY_ORDER_NUMBER_LIST;
				DisplayMenuParameters parameters = new DisplayMenuParameters(
					 "[LoadRack|Select] [LoadRack|Supply Order Number]",
					 (string[])supplyOrder.ToArray(typeof(string)),
					 false,
					 -1,
					 this.PROMPT_TIMEOUT);
				this.DisplayMenu(parameters);
				return;
			}

			supplyOrderListDO = this.GetSupplyOrders();

			if (supplyOrderListDO != null && supplyOrderListDO.LineItems.Count > 0)
			{
				foreach (SupplyOrderListLineItemDO lineItem in supplyOrderListDO.LineItems)
				{
					if (response == lineItem.DocumentNumber || response == lineItem.DocumentNumber + " - " + lineItem.SupplierID) // this will match for order selected from list)
					{
						DateTimeOffset siteTimeNow = TimeConverter.Now(this.SiteManager.Site);

						Guid identityGuid = this.GetIdentityGuidForCompany(this.Security, lineItem.SupplierID);
						this.Supplier = this.GetCompany(this.Security, identityGuid);
						this.Supplier._LastActivityDate.Value = siteTimeNow;
						this.ModifyCompany(this.Security, DATA_TYPE.DYNAMIC, this.Supplier);

						identityGuid = this.GetIdentityGuidForCompany(this.Security, lineItem.Owner);
						this.Owner = this.GetCompany(this.Security, identityGuid);
						this.Owner._LastActivityDate.Value = siteTimeNow;
						this.ModifyCompany(this.Security, DATA_TYPE.DYNAMIC, this.Owner);

						identityGuid = this.GetIdentityGuidForCompany(this.Security, lineItem.Manager);
						this.Manager = this.GetCompany(this.Security, identityGuid);
						this.Manager._LastActivityDate.Value = siteTimeNow;
						this.ModifyCompany(this.Security, DATA_TYPE.DYNAMIC, this.Manager);

						this.SelectedSupplyOrder = lineItem.DocumentNumber;

						bFound = true;
						break;
					}
				}
			}
			if (bFound == false)
			{
				this.ConsecutivePrompts++;
				if (this.ConsecutivePrompts >= this.SiteManager.Site._MaximumPrompts)
				{
					this.StationState = StationState.RESET_ON_TIMEOUT;
					this.DisplayMessage("LoadRack|Max Retries Exceeded", null, 0, this.MESSAGE_TIMEOUT);
					this.ConsecutivePrompts = 0;
					return;
				}

				this.StationState = StationState.ENTER_SUPPLY_ORDER_NUMBER;
				this.DisplayMessage(
					"[LoadRack|Invalid], [LoadRack|Enter] [LoadRack|Supply Order Number]", null, 10, this.PROMPT_TIMEOUT);
			}
			else
			{
				this.IssueVerifySupplierMenu();
			}
		}

		private Guid GetIdentityGuidForCompany(SecurityClass securityClass, string Id)
		{
			return FMChannelHelper.MakeCall<ICompanies, Guid>(
																	 x =>
																	 x.GetIdentityGuid(securityClass, Id)
																);
		}

		protected void ProcessEnterTrailer1Prompt(string response)
		{
			try
			{
				int selection = System.Convert.ToInt32(response);

				if (selection == 0 || selection > this.CurrentMenuParameters.Menu.Length)
				{
					this.IssueAdditionalOrdersPrompt();
					return;
				}

				response = this.GetDataDictionaryValueByKey(
					 this.SiteManager.Site.SiteGuid,
					 this.CurrentMenuParameters.Menu[selection - 1]);
			}
			catch (FormatException)
			{
			}
			catch (OverflowException)
			{
			}

			if (response == EscapeString)
			{
				this.IssueTractorOrTankerPrompt();
				return;
			}

			if (response == "?")
			{
				this.StationState = StationState.NOT_AVAILABLE_ENTER_TRAILER1_PROMPT;
				this.DisplayMessage("[LoadRack|List is not available]", null, 0, this.MESSAGE_TIMEOUT);
			}
			else
			{
				if (this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, "LoadRack|Yes") == response)
				{
					this.IssueTrailer1Prompt();
				}
				else if (this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, "LoadRack|No") == response)
				{
					if (this.Station.Type == STATION_TYPE.PRELOAD || this.Station.Type == STATION_TYPE.MANUAL_BOL)
					{
						this.IssueUseOrderNumberPrompt();
					}
					else if (this.Station.Type == STATION_TYPE.WEIGHT_SCALE)
					{
						this.IssueCaptureTareWeightPrompt();
					}
					else if (this.Station.Type == STATION_TYPE.LOAD_RACK)
					{
						this.StationState = StationState.IDLE;
						this.CheckProductAvailability(false);
					}
					else if (this.Station.Type == STATION_TYPE.ENTRY_GATE)
					{
						this.CardIn();
						this.OpenGate();
					}
				}
				else
				{
					this.DisplayMessage("[LoadRack|Invalid Selection]", null, 0, this.MESSAGE_TIMEOUT);
					this.StationState = StationState.INVALID_ENTER_TRAILER1_PROMPT_RESPONSE_MESSAGE;
				}
			}
		}

		protected void ProcessEnterTrailer2Prompt(string response)
		{
			try
			{
				int selection = System.Convert.ToInt32(response);

				if (selection == 0 || selection > this.CurrentMenuParameters.Menu.Length)
				{
					this.IssueAdditionalOrdersPrompt();
					return;
				}

				response = this.GetDataDictionaryValueByKey(
					 this.SiteManager.Site.SiteGuid,
					 this.CurrentMenuParameters.Menu[selection - 1]);
			}
			catch (FormatException)
			{
			}
			catch (OverflowException)
			{
			}

			if (response == EscapeString)
			{
				this.IssueEnterTrailer1Prompt();
				return;
			}

			else if (response == "?")
			{
				this.StationState = StationState.NOT_AVAILABLE_ENTER_TRAILER2_PROMPT;
				this.DisplayMessage("[LoadRack|List is not available]", null, 0, this.MESSAGE_TIMEOUT);
			}

			else
			{
				if (this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, "LoadRack|Yes") == response)
				{
					this.IssueTrailer2Prompt();
				}
				else if (this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, "LoadRack|No") == response)
				{
					switch (this.Station.Type)
					{
						case STATION_TYPE.PRELOAD:
						case STATION_TYPE.MANUAL_BOL:
							this.IssueUseOrderNumberPrompt();
							break;
						case STATION_TYPE.WEIGHT_SCALE:
							this.IssueCaptureTareWeightPrompt();
							break;
						case STATION_TYPE.LOAD_RACK:
							this.StationState = StationState.IDLE;
							this.CheckProductAvailability(false);
							break;
						case STATION_TYPE.ENTRY_GATE:
							this.CardIn();
							this.OpenGate();
							break;
					}
				}
				else
				{
					this.DisplayMessage("[LoadRack|Invalid Selection]", null, 0, this.MESSAGE_TIMEOUT);
					this.StationState = StationState.INVALID_ENTER_TRAILER2_PROMPT_RESPONSE_MESSAGE;
				}
			}
		}

		protected void ProcessEnterTrailer3Prompt(string response)
		{
			try
			{
				int selection = System.Convert.ToInt32(response);

				if (selection == 0 || selection > this.CurrentMenuParameters.Menu.Length)
				{
					this.IssueAdditionalOrdersPrompt();
					return;
				}

				response = this.GetDataDictionaryValueByKey(
					 this.SiteManager.Site.SiteGuid,
					 this.CurrentMenuParameters.Menu[selection - 1]);
			}
			catch (FormatException)
			{
			}
			catch (OverflowException)
			{
			}

			if (response == EscapeString)
			{
				this.IssueEnterTrailer2Prompt();
				return;
			}

			if (response == "?")
			{
				this.StationState = StationState.NOT_AVAILABLE_ENTER_TRAILER3_PROMPT;
				this.DisplayMessage("[LoadRack|List is not available]", null, 0, this.MESSAGE_TIMEOUT);
			}
			else
			{
				if (this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, "LoadRack|Yes") == response)
				{
					this.IssueTrailer3Prompt();
				}
				else if (this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, "LoadRack|No") == response)
				{
					if (this.Station.Type == STATION_TYPE.PRELOAD || this.Station.Type == STATION_TYPE.MANUAL_BOL)
					{
						this.IssueUseOrderNumberPrompt();
					}
					else if (this.Station.Type == STATION_TYPE.WEIGHT_SCALE)
					{
						this.IssueCaptureTareWeightPrompt();
					}
					else if (this.Station.Type == STATION_TYPE.LOAD_RACK)
					{
						this.StationState = StationState.IDLE;
						this.CheckProductAvailability(false);
					}
					else if (this.Station.Type == STATION_TYPE.ENTRY_GATE)
					{
						this.CardIn();
						this.OpenGate();
					}
				}
				else
				{
					this.DisplayMessage("[LoadRack|Invalid Selection]", null, 0, this.MESSAGE_TIMEOUT);
					this.StationState = StationState.INVALID_ENTER_TRAILER3_PROMPT_RESPONSE_MESSAGE;
				}
			}
		}

		protected void ProcessExitWeight(string response)
		{
			if (response == EscapeString)
			{
				if (this.Station.CardReader)
				{
					this.IssuePleaseCardIn();
				}
				else if (this.Station.TouchKeyReader)
				{
					this.IssueTouchKeyPleaseCardIn();
				}
				else
				{
					this.IssueDriverIDPrompt();
				}
			}
			else
			{
				if (response == this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, "Accept"))
				{
					bool BrokenBlends = false;

					bool ImproperAdditization = false;

					foreach (TransactionDO CompletedTransaction in this.PendingTransactions)
					{
						this.Transaction = CompletedTransaction;
						this.Transaction.Status = TransactionStatus.Completed;
						WeightReadingDO WeightReading = this.Transaction.WeightReadings[0];
						WeightReading.FinalQuantity = System.Convert.ToDouble(this.CurrentWeight.Value);

						this.CalculateWeighOutVolume(CompletedTransaction);

						foreach (LineItemDO LineItem in this.Transaction.LineItems)
						{
							if (LineItem.BrokenBlend != null && LineItem.BrokenBlend.Value)
							{
								BrokenBlends = true;
							}

							if (LineItem.ImproperAdditization != null && LineItem.ImproperAdditization.Value)
							{
								ImproperAdditization = true;
							}

							if (LineItem.Status == TransactionStatus.LoadPending
								 || (LineItem.Status == TransactionStatus.InProgress && LineItem.Quantity != null
									  && LineItem.Quantity.GrossInventoryChange == 0 && LineItem.Quantity.NetInventoryChange == 0
									  && LineItem.Quantity.MassInventoryChange == 0))
							{
								LineItem.Status = TransactionStatus.Cancelled;
								foreach (SubLineItemDO SubLineItem in LineItem.SubLineItems)
								{
									SubLineItem.Status = TransactionStatus.Cancelled;
								}
							}
						}

						if (this.Driver.OnFileSignature != null && this.Driver.OnFileSignature.Length > 0)
						{
							this.Transaction.Signature = this.Driver.OnFileSignature;
						}

						this.Transaction.TimeOut = TimeConverter.Now(this.SiteManager.Site);

						this.SaveTransaction();

						this.PrintTransaction();
					}

					if (BrokenBlends)
					{
						this.StationState = StationState.BROKEN_BLEND_WEIGHTOUT;
						this.DisplayMessageWithAcknowledge("LoadRack|Broken Blend Detected.");
					}
					else if (ImproperAdditization)
					{
						this.StationState = StationState.IMPROPER_ADDITIZATION_WEIGHTOUT;
						this.DisplayMessageWithAcknowledge("LoadRack|Improper Additization Detected.");
					}
					else
					{
						this.CheckExitInstructions(false);
					}

					this.PendingTransactions.Clear();

					this.Transaction = null;
				}
				else
				{
					this.IssueCaptureExitWeightPrompt(false);
				}
			}
		}

		protected virtual void ProcessLoadID(string response)
		{
			if (response == "?")
			{
				this.IssueSelectCustomerShipToFilterColumnPrompt();
			}
			else if (response == EscapeString)
			{
				if (this.Station.Type == STATION_TYPE.LOAD_RACK)
				{
					if (this.PriorStationState == StationState.LOADID_PROMPT)
					{
						this.PriorStationState = StationState.IDLE;
						if (this.SiteManager.Site.PromptForShipmentNumber)
						{
							this.IssueLoadByShipmentOrLoadIDPrompt();
						}
						////else if (this.Driver.PINRequired || this.Carrier.PINRequired)
						////{
						////    // TODO: only jump back to Pin
						////    // this.IssuePromptForPin();
						////}
						else if (this.Station.CardReader)
						{
							this.IssuePleaseCardIn();
						}
						else if (this.Station.TouchKeyReader)
						{
							this.IssueTouchKeyPleaseCardIn();
						}
						else
						{
							this.IssueDriverIDPrompt();
						}
					}
					else if (this.Transaction == null)
					{
						throw new NotImplementedException();
						////if (this.PromptForThirdTrailer
						////    && (this.TractorOrTanker == null || this.TractorOrTanker.Type == EQUIPMENT_TYPE.TRACTOR_TYPE))
						////{
						////    this.IssueEnterTrailer3Prompt();
						////}
						////else if (this.PromptForSecondTrailer
						////         && (this.TractorOrTanker == null
						////             || this.TractorOrTanker.Type == EQUIPMENT_TYPE.TRACTOR_TYPE))
						////{
						////    this.IssueEnterTrailer2Prompt();
						////}
						////else if (this.PromptForFirstTrailer)
						////{
						////    this.IssueEnterTrailer1Prompt();
						////}
						////else if (this.PromptForTractorOrTanker)
						////{
						////    this.IssueTractorOrTankerPrompt();
						////}
						////else if (this.SiteManager.Site.PromptForShipmentNumber)
						////{
						////    this.IssueLoadByShipmentOrLoadIDPrompt();
						////}
						////else if (this.Driver.PINRequired || this.Carrier.PINRequired)
						////{
						////    this.IssuePromptForPin();
						////}
						////else if (this.Station.CardReader)
						////{
						////    this.IssuePleaseCardIn();
						////}
						////else if (this.Station.TouchKeyReader)
						////{
						////    this.IssueTouchKeyPleaseCardIn();
						////}
						////else
						////{
						////    this.IssueDriverIDPrompt();
						////}
					}
				}
				else
				{
					if (this.Orders)
					{
						this.IssueUseOrderNumberPrompt();
					}
					else
					{
						this.ProcessUseOrder(EscapeString);
					}
				}
			}
			else
			{
				Guid companyMapGuid = FMChannelHelper.MakeCall<ICompanyMaps, Guid>(x => x.GetIdentityGuidByMapID(this.Security, response));  // .2 seconds.
				
				// Invalid CompanyMap ID
				if (companyMapGuid == Guid.Empty)
				{
					FMChannelHelper.MakeCall<IAlarmAndEventLogs>(x => x.Add(this.Security, this.Station.InvalidCustomerNumberEvent(response, this.Driver.FirstLastName)));
					this.LoadRackManager.EventOrAlarmEvent.Set();

					if (this.CheckMaximumRetries())
					{
						return;
					}

					this.StationState = StationState.LOADID_PROMPT;
					this.DisplayMessage("[LoadRack|Invalid], [LoadRack|Enter] [LoadRack|Load ID]", null, PromptLength, this.PROMPT_TIMEOUT);
					return;
				}

				bool validateResult;
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				validateResult = this.ValidateLoadID(companyMapGuid); // 13 seconds
				long elapsed = stopwatch.ElapsedMilliseconds;
				if (!validateResult)
				{
					return;
				}

				if (this.Station.Type == STATION_TYPE.PRELOAD || this.Station.Type == STATION_TYPE.WEIGHT_SCALE
					 || this.Station.Type == STATION_TYPE.MANUAL_BOL)
				{
					this.IssueEnterPurchaseOrderPrompt();
				}
				else
				{
					if (!this.SiteManager.Site.InhibitCustomerConfirmationPrompt)
					{
						this.IssueShipToMenu(false); // 1.5-2 seconds
					}
					else
					{
						this.ProcessShipTo("Yes");
					}
				}
			}
		}

		protected void ProcessLoadIDCard(string cardID)
		{
			this.LoadID = cardID;

			Guid identityGuid = FMChannelHelper.MakeCall<ICompanyMaps, Guid>(
																	 x =>
																	 x.GetIdentityGuidByMapID(this.Security, this.LoadID)
																);
			// If we are supposed to support 35-bit cards, strip off the first character
			if (identityGuid == Guid.Empty && this.Station.ThirtyFiveBitCardSupport)
			{
				this.LoadID = this.LoadID.Substring(1, this.LoadID.Length - 1);

				identityGuid = FMChannelHelper.MakeCall<ICompanyMaps, Guid>(
																	 x =>
																	 x.GetIdentityGuidByMapID(this.Security, this.LoadID)
																);
			}

			// Invalid Card Number
			if (identityGuid == Guid.Empty)
			{
				this.AddAlarmAndEventLogs(this.Security, this.Station.InvalidCustomerNumberEvent(this.LoadID));

				this.DisplayMessage("[LoadRack|Invalid Card Number]", null, 0, this.MESSAGE_TIMEOUT);
				this.StationState = StationState.RESET_ON_TIMEOUT;
				return;
			}

			this.ConsecutivePrompts = 0;
			this.StationState = StationState.IDLE;

			if (!this.ValidateLoadID(identityGuid))
			{
				return;
			}

			this.IssueEnterPurchaseOrderPrompt();
		}

		protected void ProcessLoadSummary(string Response)
		{
			if (Response == EscapeString)
			{
				this.LoadSummaryIssued = false;

				if (this.SingleProduct)
				{
					if (!this.ByWeight)
					{
						this.IssuePresetPrompt();
					}

					else if (this.ShipTo.PurchaseOrderRequired)
					{
						this.IssueEnterPurchaseOrderPrompt();
					}

					else if (this.Order != null)
					{
						if (this.SiteManager.Site.PromptForShipmentNumber)
						{
							this.IssueEnterShipmentNumberPrompt();
						}
						else
						{
							this.IssueEnterOrderNumberPrompt();
						}
					}

					else
					{
						this.IssueLoadIDPrompt();
					}
				}
				else
				{
					this.IssueCompartmentSummaryPrompt();
				}
			}

			else
			{
				if (Response.StartsWith(this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, "LoadRack|Tanker")))
				{
					this.CurrentEquipment = this.TractorOrTanker;
					this.IssueCompartmentSummaryPrompt();
				}

				else if (
					Response.StartsWith(
						this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, "[LoadRack|Trailer] " + this.EquipmentID(this.Trailer1))))
				{
					this.CurrentEquipment = this.Trailer1;
					this.IssueCompartmentSummaryPrompt();
				}

				else if (
					Response.StartsWith(
						this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, "[LoadRack|Trailer] " + this.EquipmentID(this.Trailer2))))
				{
					this.CurrentEquipment = this.Trailer2;
					this.IssueCompartmentSummaryPrompt();
				}

				else if (Response.StartsWith(this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, "LoadRack|Accept")))
				{
					if (this.ByWeight)
					{
						this.Transaction.Status = TransactionStatus.WeighOutPending;
					}
					else
					{
						this.Transaction.Status = TransactionStatus.LoadPending;
					}

					// Determine if Split Load is Possible, i.e. any compartment withoug load pending
					if (!this.ByWeight)
					{
						if (this.TotalAvailableCompartments > this.TotalCompartmentsInUseCurrentTransaction)
						{
							this.IssueAdditionalOrdersPrompt();
							return;
						}
					}

					// Remove LineItems for which no product was selected
					for (int Index = this.Transaction.LineItems.Count; Index > 0; Index--)
					{
						LineItemDO LineItem = this.Transaction.LineItems[Index - 1];
						if (LineItem.Product == null)
						{
							this.Transaction.LineItems.RemoveAt(Index - 1);
						}
					}

					try
					{
						this.SaveTransaction();
					}
					catch (Exception e)
					{
						this.DisplayMessage(e.Message, null, 0, this.MESSAGE_TIMEOUT);
						this.StationState = StationState.RESET_ON_TIMEOUT;
						return;
					}

					if (this.Station.Type == STATION_TYPE.WEIGHT_SCALE || this.Station.Type == STATION_TYPE.PRELOAD)
					{
						this.PrintPreload();
					}

					this.PendingTransactions.Add(this.Transaction);
					this.Transaction = null;

					this.CheckEntryInstructions(false);
				}

				else if (Response.StartsWith(this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, "[LoadRack|Purchase Order]")))
				{
					this.IssueEnterPurchaseOrderPrompt();
				}

				else if (this.Order != null)
				{
					if (this.SiteManager.Site.PromptForShipmentNumber)
					{
						this.IssueEnterShipmentNumberPrompt();
					}
					else
					{
						this.IssueEnterOrderNumberPrompt();
					}
				}
				else
				{
					this.IssueLoadIDPrompt();
				}
			}
		}
		protected void ProcessOffloadSummary(string response)
		{
			try
			{
				int selection = System.Convert.ToInt32(response);

				if (selection == 0 || selection > this.CurrentMenuParameters.Menu.Length)
				{
					this.IssueAdditionalOrdersPrompt();
					return;
				}

				response = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
												x =>
												x.Get(this.SiteManager.Site.SiteGuid, this.CurrentMenuParameters.Menu[selection - 1])
									 );
			}
			catch (FormatException)
			{
			}
			catch (OverflowException)
			{
			}

			if (response == EscapeString)
			{
				this.LoadSummaryIssued = false;

				if (this.SingleProduct)
				{
					if (!this.ByWeight)
					{
						this.IssuePresetPrompt();
					}
					else if (this.SupplyOrder != null)
					{
						this.PromptForSupplyOrderNumber();
					}
					else
					{
						this.IssueOffLoadIDPrompt();
					}
				}
				else
				{
					this.IssueCompartmentSummaryPrompt();
				}

				return;
			}

			if (response.StartsWith(this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, "LoadRack|Tanker")))
			{
				this.CurrentEquipment = this.TractorOrTanker;
				this.IssueCompartmentSummaryPrompt();
			}
			else if (response.StartsWith(this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, "[LoadRack|Trailer] " + this.EquipmentID(this.Trailer1))))
			{
				this.CurrentEquipment = this.Trailer1;
				this.IssueCompartmentSummaryPrompt();
			}
			else if (response.StartsWith(this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, "[LoadRack|Trailer] " + this.EquipmentID(this.Trailer2))))
			{
				this.CurrentEquipment = this.Trailer2;
				this.IssueCompartmentSummaryPrompt();
			}
			else if (response.StartsWith(this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, "[LoadRack|Trailer] " + this.EquipmentID(this.Trailer3))))
			{
				this.CurrentEquipment = this.Trailer3;
				this.IssueCompartmentSummaryPrompt();
			}
			else if (response.StartsWith(this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, "LoadRack|Accept")))
			{
				if (this.ByWeight)
				{
					// Save the density and temperature in the one line item if we captured them
					if (this.Transaction.LineItems.Count > 0)
					{
						if (this.Station.PromptForGravity)
						{
							this.Transaction.LineItems[0].Density = this.OffloadDensity;
						}

						if (this.Station.PromptForTemperature)
						{
							this.Transaction.LineItems[0].Temperature = this.OffloadTemperature;
						}
					}

					if (this.Station.PromptForBOLNumber)
					{
						this.Transaction.DocumentNumber = this.SelectedBOLNumber;
					}

					this.Transaction.Status = TransactionStatus.WeighOutPending;
				}
				else
				{
					this.Transaction.Status = TransactionStatus.LoadPending;
				}

				// Remove LineItems for which no product was selected
				for (int index = this.Transaction.LineItems.Count; index > 0; index--)
				{
					LineItemDO lineItem = this.Transaction.LineItems[index - 1];
					if (string.IsNullOrEmpty(lineItem.Product))
					{
						this.Transaction.LineItems.RemoveAt(index - 1);
					}
				}

				try
				{
					this.SaveTransaction();
				}
				catch (Exception e)
				{
					this.DisplayMessage(e.Message, null, 0, this.MESSAGE_TIMEOUT);
					this.StationState = StationState.RESET_ON_TIMEOUT;
					return;
				}

				switch (this.Station.Type)
				{
					case STATION_TYPE.PRELOAD:
						this.PrintPreload();
						break;
					case STATION_TYPE.WEIGHT_SCALE:
						switch (this.Transaction.Status)
						{
							case TransactionStatus.LoadPending:
								this.PrintPreload();
								break;
							case TransactionStatus.WeighOutPending:
								if (!this.ByWeight)
								{
									this.PrintTransaction();
									//this.PrintWeight();
								}

								break;
							case TransactionStatus.Completed:
							case TransactionStatus.Posted:
								this.PrintTransaction();
								//this.PrintWeight();
								break;
						}

						break;
					case STATION_TYPE.MANUAL_BOL:
						this.PrintTransaction();
						break;
				}

				this.PendingTransactions.Add(this.Transaction);
				this.Transaction = null;

				this.CheckEntryInstructions(false);
			}
			else if (this.SupplyOrder != null)
			{
				this.PromptForSupplyOrderNumber();
			}
			else
			{
				this.IssueOffLoadIDPrompt();
			}
		}

		protected virtual void ProcessManualMeterStartData(string response)
		{
		}

		protected virtual void ProcessManualMeterStopData(string Response)
		{
		}

		protected void ProcessOffLoadComplete(string Response)
		{
			if (Response == "1")
			{
				Response = this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, this.CurrentMenuParameters.Menu[0]);
			}

			if (Response == "2")
			{
				Response = this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, this.CurrentMenuParameters.Menu[1]);
			}

			if (Response == this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, "LoadRack|Off Load New Batch"))
			{
				if (this.Station.OffLoadByOffLoadID || this.UseOffLoadSupplyOrders == false)
				{
					this.DisplayOffLoadProductSelect();
				}
				else
				{
					this.DisplayVerifySupplyOrderProduct();
				}
			}
			else if (Response == this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, "LoadRack|Finished Off Loading"))
			{
				this.CompleteOffLoadingTransaction();

				if (this.Station.CardReader)
				{
					this.IssuePleaseCardIn();
				}
				else
				{
					this.IssueDriverIDPrompt();
				}
				return;
			}
			else
			{
				this.StationState = StationState.INVALID_OFFLOAD_COMPLETE_TYPE_SELECTION_MSG;
				this.DisplayMessage("[LoadRack|Invalid Selection]", null, 0, this.MESSAGE_TIMEOUT);
			}
		}

		protected virtual void ProcessOffLoadID(string Response)
		{
			if (Response == EscapeString)
			{
				if (this.Driver.PINRequired || this.Carrier.PINRequired)
				{
					this.IssuePromptForPIN();
				}
				else if (this.Station.CardReader)
				{
					this.IssuePleaseCardIn();
				}
				else if (this.Station.TouchKeyReader)
				{
					this.IssueTouchKeyPleaseCardIn();
				}
				else
				{
					this.IssueDriverIDPrompt();
				}
			}
			else if (Response == "?")
			{
				this.IssueSelectSupplierOffLoadIDFilterColumnPrompt();
			}
			else
			{
				Guid identityGuid = FMChannelHelper.MakeCall<ICompanyMaps, Guid>(
																	 x =>
																	 x.GetOffLoadIdentityGuidByMapID(this.Security, Response)
																);
				// Invalid CompanyMap ID
				if (identityGuid == Guid.Empty)
				{
					this.AddAlarmAndEventLogs(this.Security, this.Station.InvalidCustomerNumberEvent(Response));

					this.ConsecutivePrompts++;
					if (this.ConsecutivePrompts >= this.SiteManager.Site._MaximumPrompts)
					{
						this.StationState = StationState.RESET_ON_TIMEOUT;
						this.DisplayMessage("LoadRack|Max Retries Exceeded", null, 0, this.MESSAGE_TIMEOUT);
						this.ConsecutivePrompts = 0;
						return;
					}

					this.StationState = StationState.OFFLOADID_PROMPT;
					this.DisplayMessage("[LoadRack|Enter] [LoadRack|Off Load ID]", null, PromptLength, this.PROMPT_TIMEOUT);
					return;
				}

				if (!this.ValidateOffLoadID(identityGuid))
				{
					return;
				}

				this.IssueVerifySupplierMenu();
			}
		}

		protected virtual void ProcessOffLoadInProgress(string response)
		{
		}

		protected virtual void ProcessOffLoadProductSelect(string response)
		{
		}

		protected void ProcessOperatingMode(string response)
		{
			if (response == EscapeString)
			{
				if (this.Driver.PINRequired || this.Carrier.PINRequired)
				{
					this.IssuePromptForPIN();
				}

				else
				{
					if (this.Station.CardReader)
					{
						this.IssuePleaseCardIn();
					}
					else if (this.Station.TouchKeyReader)
					{
						this.IssueTouchKeyPleaseCardIn();
					}
					else
					{
						this.IssueDriverIDPrompt();
					}
				}
				return;
			}

			if ((this.Station.Type != STATION_TYPE.OFF_LOADING)
				 && (this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, "LoadRack|Loading") == response
					  || this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, "LoadRack|UnLoading") == response))
			{
				this.Mode = OperatingMode.Loading;
				this.ConsecutivePrompts = 0;

				if (!this.Driver.HasRole(PERSON_ROLE.LOADER_ROLE))
				{
					this.AddAlarmAndEventLogs(this.Security, this.Driver.HaveLoaderRoleEvent(this.Station.ID));
					this.LoadRackManager.EventOrAlarmEvent.Set();
					this.DisplayMessage("LoadRack|Must have Loader Role", null, 0, this.MESSAGE_TIMEOUT);
					this.StationState = StationState.RESET_ON_TIMEOUT;
					return;
				}

				// Prompt for Tractor/Tanker
				if (this.SiteManager.Site.PromptForTractorOrTanker)
				{
					this.IssueTractorOrTankerPrompt();
				}

				// Prompt for Truck Card
				else if (this.SiteManager.Site.PromptForTruckCard)
				{
					this.IssueTractorCardInPrompt();
				}

				// Prompt for First Trailer
				else if (this.SiteManager.Site.PromptForFirstTrailer)
				{
					this.IssueEnterTrailer1Prompt();
				}

				// Prompt for LoadID
				else
				{
					if (this.SiteManager.Site.PromptForCustomerCard && this.Station.CardReader)
					{
						this.StationState = StationState.LOADID_CARD_PROMPT;
						this.DisplayMessage("LoadRack|Scan Load Card", null, 0, this.PROMPT_TIMEOUT);
					}
					else
					{
						this.StationState = StationState.LOADID_PROMPT;
						this.DisplayMessage("[LoadRack|Enter] [LoadRack|Load ID]", null, PromptLength, this.PROMPT_TIMEOUT);
					}
				}
			}
			else if (this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, "[LoadRack|UnLoading]") == response)
			{
				this.Mode = OperatingMode.Unloading;
				this.ConsecutivePrompts = 0;

				if (!this.Driver.HasRole(PERSON_ROLE.OFFLOADER_ROLE))
				{
					this.AddAlarmAndEventLogs(this.Security, this.Driver.HaveOffloaderRoleEvent(this.Station.ID));
					this.LoadRackManager.EventOrAlarmEvent.Set();
					this.DisplayMessage("LoadRack|Must have Offloader Role", null, 0, this.MESSAGE_TIMEOUT);
					this.StationState = StationState.RESET_ON_TIMEOUT;
					return;
				}

				this.DetermineTypeOfOffLoadingOperation();
			}
			else
			{
				this.StationState = StationState.IDLE;
				this.DisplayMessage("Unknown Operating Mode", null, 0, this.MESSAGE_TIMEOUT);
			}
		}

		protected void ProcessOrder(string Response)
		{
			if (Response == EscapeString)
			{
				if (this.Station.InhibitLoadingByLoadID)
				{
					this.ProcessUseOrder(EscapeString);
				}
				else
				{
					this.IssueUseOrderNumberPrompt();
				}
			}

			else if ("?" == Response) // list command received
			{
				OrderListDO orderListDO = this.GetOrders();

				ArrayList Order = new ArrayList();

				if (orderListDO != null && orderListDO.LineItems.Count > 0)
				{
					foreach (OrderListLineItemDO lineItem in orderListDO.LineItems)
					{
						if (string.IsNullOrEmpty(lineItem.CarrierID) == false && lineItem.CarrierID != this.Carrier.ID)
						{
							continue;
						}

						if (string.IsNullOrEmpty(lineItem.OperatorID) == false && lineItem.OperatorID != this.Driver.ID)
						{
							continue;
						}

						if (string.IsNullOrEmpty(lineItem.DestRegistrationID1) == false)
						{
							if (this.SiteManager.Site.PromptForTractorOrTanker)
							{
								if (this.TractorOrTanker == null || lineItem.DestRegistrationID1 != this.TractorOrTanker.ID)
								{
									continue;
								}
							}

							else if (this.SiteManager.Site.PromptForFirstTrailer)
							{
								if (this.Trailer1 == null || lineItem.DestRegistrationID1 != this.Trailer1.ID)
								{
									continue;
								}
							}

							else
							{
								continue;
							}
						}

						if (string.IsNullOrEmpty(lineItem.DestRegistrationID2) == false)
						{
							if (this.SiteManager.Site.PromptForTractorOrTanker)
							{
								if (this.Trailer1 == null || lineItem.DestRegistrationID2 != this.Trailer1.ID)
								{
									continue;
								}
							}

							else
							{
								if (this.Trailer2 == null || lineItem.DestRegistrationID2 != this.Trailer2.ID)
								{
									continue;
								}
							}
						}

						if (string.IsNullOrEmpty(lineItem.DestRegistrationID3) == false)
						{
							if (this.Trailer2 == null || lineItem.DestRegistrationID3 != this.Trailer2.ID)
							{
								continue;
							}
						}

						foreach (CompanyMapClass AuthorizedShipTo in this.Carrier.CarrierCustomerShipToCollection)
						{
							if (AuthorizedShipTo.AssignedToID == lineItem.ShipToID)
							{
								Order.Add(lineItem.DocumentNumber + " - " + lineItem.ShipToID);
								break;
							}
						}
					}
				}

				if (Order.Count == 0)
				{
					this.DisplayMessage("LoadRack|No Orders", null, 0, this.MESSAGE_TIMEOUT);
					return;
				}

				this.StationState = StationState.SELECT_ORDER_PROMPT;
				DisplayMenuParameters Parameters = new DisplayMenuParameters(
					"[LoadRack|Select] [LoadRack|Order Number]",
					(string[])Order.ToArray(typeof(string)),
					false,
					-1,
					this.PROMPT_TIMEOUT);
				this.DisplayMenu(Parameters);
			}

			else
			{
				OrderListDO orderListDO = this.GetOrders();

				foreach (OrderListLineItemDO lineItem in orderListDO.LineItems)
				{
					if (Response == lineItem.DocumentNumber || Response == lineItem.DocumentNumber + " - " + lineItem.ShipToID) // this will match for order selected from list
					{
						this.Order = this.GetTransaction(lineItem.TransactionID);
						if (this.Order != null)
						{
							// If the associated transaction alias is not valid, we will not be successful creating a 
							// preload based on the Order.
							this.OrderTransactionAlias = this.GetTransactionAlias(this.Security, this.Order.TransactionAliasGuid, false);

							if (this.OrderTransactionAlias != null)
							{
								this.CurrentTransactionAlias = this.GetTransactionAlias(
									this.Security, this.OrderTransactionAlias.AssociatedTransactionAliasGuid, false);
							}

							if (this.OrderTransactionAlias == null || this.OrderTransactionAlias.AssociatedTransactionAliasGuid == Guid.Empty
								 || this.CurrentTransactionAlias == null || this.CurrentTransactionAlias.IdentityGuid == Guid.Empty)
							{
								this.AddAlarmAndEventLogs(this.Security, this.Station.OrderAliasInvalidEvent(this.Station.ID));
								this.LoadRackManager.EventOrAlarmEvent.Set();

								this.StationState = StationState.RESET_ON_TIMEOUT;
								this.DisplayMessage("LoadRack|Order associated alias invalid", null, 0, this.MESSAGE_TIMEOUT);
								return;
							}

							if (this.Order.ShipToCompanyGuid != Guid.Empty)
							{
								this.ShipTo = this.GetCompany(this.Security, this.Order.ShipToCompanyGuid);
							}

							string CompanyHierarchy = this.Order.ManagerID + "->" + this.Order.OwnerID + "->" + this.Order.ShipperID + "->"
															  + this.Order.BillToID;
							this.CompanyMapCollection = this.EnumerateByAssignedGuidAndTypeForCompanyMaps(
								this.Security, this.ShipTo.MasterRecordGuid, COMPANY_MAP_TYPE.SHIPTO_BILLTO_MAP);
							foreach (CompanyMapClass CompanyMap in this.CompanyMapCollection)
							{
								if (CompanyMap.AssignedToID == CompanyHierarchy)
								{
									if (!this.ValidateCompanyHierarchyLoadRack(CompanyMap))
									{
										this.StationState = StationState.INVALID_COMPANY_ON_ORDER;
										return;
									}

									this.StationState = StationState.IDLE;
									this.CheckProductAvailability(false);
									return;
								}
							}

							// No Company Hierarchy, Validate Companies Individually
							if (this.Order.BillToCompanyGuid != Guid.Empty)
							{
								this.BillTo = this.GetCompany(this.Security, this.Order.BillToCompanyGuid);
							}
							if (this.Order.ShipperCompanyGuid != Guid.Empty)
							{
								this.Shipper = this.GetCompany(this.Security, this.Order.ShipperCompanyGuid);
							}
							if (this.Order.OwnerCompanyGuid != Guid.Empty)
							{
								this.Owner = this.GetCompany(this.Security, this.Order.OwnerCompanyGuid);
							}
							if (this.Order.ManagerCompanyGuid != Guid.Empty)
							{
								this.Manager = this.GetCompany(this.Security, this.Order.ManagerCompanyGuid);
							}

							if (!this.ValidateCompany(this.ShipTo, COMPANY_ROLE.CUSTOMER_SHIPTO)
								 || !this.ValidateCompany(this.BillTo, COMPANY_ROLE.CUSTOMER_BILLTO)
								 || !this.ValidateCompany(this.Shipper, COMPANY_ROLE.SHIPPER)
								 || !this.ValidateCompany(this.Owner, COMPANY_ROLE.OWNER)
								 || !this.ValidateCompany(this.Manager, COMPANY_ROLE.MANAGER))
							{
								this.StationState = StationState.INVALID_COMPANY_ON_ORDER;
								return;
							}

							this.StationState = StationState.IDLE;
							this.CheckProductAvailability(false);
							return;
						}
					}
				}

				this.ConsecutivePrompts++;
				if (this.ConsecutivePrompts >= this.SiteManager.Site._MaximumPrompts)
				{
					this.StationState = StationState.RESET_ON_TIMEOUT;
					this.DisplayMessage("LoadRack|Max Retries Exceeded", null, 0, this.MESSAGE_TIMEOUT);
					this.ConsecutivePrompts = 0;
					return;
				}

				this.StationState = StationState.ENTER_ORDER_PROMPT;
				this.DisplayMessage("[LoadRack|Invalid], [LoadRack|Enter] [LoadRack|Order Number]", null, 10, this.PROMPT_TIMEOUT);
			}
		}

		private CompanyMapCollectionClass EnumerateByAssignedGuidAndTypeForCompanyMaps(SecurityClass securityClass, Guid guid, COMPANY_MAP_TYPE cOMPANY_MAP_TYPE)
		{
			return FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapCollectionClass>(
						x =>
						x.EnumerateByAssignedGuidAndType(this.Security, this.ShipTo.MasterRecordGuid, COMPANY_MAP_TYPE.SHIPTO_BILLTO_MAP)
				);
		}

		protected virtual void ProcessPIN(string Response)
		{
			StopWatch timer = new StopWatch(StopWatch.Appnames.LoadRackService, "ProcessPIN");
			timer.Perform("ProcessPIN");
			try
			{
				if (Response == EscapeString)
				{
					if (this.Station.CardReader)
					{
						this.IssuePleaseCardIn();
					}
					else if (this.Station.TouchKeyReader)
					{
						this.IssueTouchKeyPleaseCardIn();
					}
					else
					{
						this.IssueDriverIDPrompt();
					}
					return;
				}

				if (Response == "?")
				{
					this.DisplayMessage("[LoadRack|List is not available]", null, 0, this.MESSAGE_TIMEOUT);
					return;
				}

				// Check the driver PIN number against the response.  
				if (Response != this.Driver.PINNumber)
				{
					this.AddAlarmAndEventLogs(this.Security, this.Station.InvalidPinEvent(this.Driver.ID, Response));

					this.ConsecutivePrompts++;
					if (this.ConsecutivePrompts >= this.SiteManager.Site._MaximumPrompts)
					{
						this.StationState = StationState.RESET_ON_TIMEOUT;
						this.DisplayMessage("LoadRack|Max Retries Exceeded", null, 0, this.MESSAGE_TIMEOUT);
						// vthompson CSI 5754 - Lock out the driver after the max number of attempts
						// is exceeded
						this.Driver.LockedOut = true;
						this.Driver.LockedOutDate = TimeConverter.Now(this.SiteManager.Site).ToString("d");
						this.Driver.LockedOutReason = "Maximum number of Card In attempts was exceeded";
						this.ModifyPersonnel(this.Security, DATA_TYPE.DYNAMIC, this.Driver);
						this.ConsecutivePrompts = 0;
						return;
					}

					this.StationState = StationState.PIN_PROMPT;
					this.PromptForPin("[LoadRack|Invalid], [LoadRack|Enter] [LoadRack|PIN]", PinLength, this.PROMPT_TIMEOUT);
					return;
				}

				if (this.Station.Type == STATION_TYPE.BOL)
				{
					this.CardIn();

					this.PrintTransactions();

					if (!this.SiteManager.AnyExitGates)
					{
						this.CardOut();
					}

					return;
				}

				this.StationState = StationState.IDLE;
				this.CompleteDriverProcessing(false);
			}

			finally
			{
				timer.Stop();
			}
		}

		protected void ModifyPersonnel(SecurityClass securityClass, DATA_TYPE dataType, PersonClass personClass)
		{
			FMChannelHelper.MakeCall<IPersonnel>(
																	 x =>
																	 x.Modify(securityClass, dataType, personClass)
																);
		}

		protected virtual void ProcessPermissiveMessageAcknowledge(string Response)
		{
		}

		protected virtual void ProcessPreloadDocumentSelection(string response)
		{
			if (response == EscapeString)
			{
				this.ResetStationDevice();
				return;
			}

			this.LoadTransaction(response);
		}

		protected virtual void ProcessPreloadLoadIDSelection(string response)
		{
			if (response == EscapeString)
			{
				this.ResetStationDevice();
				return;
			}

			foreach (string ID in this.LoadIDList)
			{
				if (ID == response)
				{
					this.LoadID = ID;
					this.IssueSelectPreloadDocument();
					return;
				}
			}

			this.StationState = StationState.INVALID_PRELOAD_LOADID_SELECTION_MSG;
			this.DisplayMessage("LoadRack|Invalid", null, 0, this.MESSAGE_TIMEOUT);
			return;
		}

		protected void ProcessPreloadSelectMethod(string Response)
		{
			if (Response == "1")
			{
				Response = this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, this.CurrentMenuParameters.Menu[0]);
			}

			if (Response == "2")
			{
				Response = this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, this.CurrentMenuParameters.Menu[1]);
			}

			else if (Response == "3")
			{
				Response = this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, this.CurrentMenuParameters.Menu[2]);
			}

			if (Response == EscapeString || Response == "0")
			{
				this.ResetStationDevice();
				return;
			}

			else if (Response == this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, "LoadRack|Order"))
			{
				this.IssueSelectPreloadOrder();
			}
			else if (Response == this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, "LoadRack|Load ID"))
			{
				this.IssueSelectPreloadLoadID();
			}
			else if (Response == this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, "LoadRack|Document"))
			{
				this.IssueSelectPreloadDocument();
			}
			else
			{
				this.StationState = StationState.INVALID_PRELOAD_TYPE_SELECTION_MSG;
				this.DisplayMessage("[LoadRack|Invalid Selection]", null, 0, this.MESSAGE_TIMEOUT);
			}
		}

		protected void ProcessPreset(string Response)
		{
			if (Response == EscapeString)
			{
				LineItemDO LineItem = this.CurrentLineItem;
				if (LineItem.PresetAmount == null)
				{
					LineItem.Product = null;
					LineItem.ProductGuid = Guid.Empty;
					LineItem.ProductCode = null;
					LineItem.ProductType = null;
					LineItem.CustomerProductName = null;
					LineItem.CustomerProductCode = null;
					LineItem.StorageLocationID = null;
					LineItem.StorageLocationTankGuid = Guid.Empty;
				}

				if (!this.SingleProduct)
				{
					this.IssueSelectProductPrompt();
				}

				else if (this.AvailableCompartments(this.CurrentEquipment) > 1)
				{
					this.IssueCompartmentSummaryPrompt();
				}

				else if (this.LoadSummaryIssued)
				{
					this.IssueLoadSummaryPrompt();
				}

				else if (this.CurrentEquipment == this.Trailer2 && this.AvailableCompartments(this.Trailer1) > 0)
				{
					this.CurrentEquipment = this.Trailer1;
					this.IssueCompartmentSummaryPrompt();
				}

				else if (this.CurrentEquipment == this.Trailer1 && this.AvailableCompartments(this.TractorOrTanker) > 0)
				{
					this.CurrentEquipment = this.TractorOrTanker;
					this.IssueCompartmentSummaryPrompt();
				}

				else if (this.Order != null)
				{
					if (this.SiteManager.Site.PromptForShipmentNumber)
					{
						this.IssueEnterShipmentNumberPrompt();
					}
					else
					{
						this.IssueEnterOrderNumberPrompt();
					}
				}
				else
				{
					this.IssueLoadIDPrompt();
				}
			}
			else
			{
				LineItemDO LineItem = this.CurrentLineItem;
				double PresetAmount = 0.0;
				try
				{
					PresetAmount = System.Convert.ToDouble(Response, this.SiteManager.Site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME));
					if (PresetAmount > this.CurrentMaximum)
					{
						this.ConsecutivePrompts++;
						if (this.ConsecutivePrompts >= this.SiteManager.Site._MaximumPrompts)
						{
							this.StationState = StationState.RESET_ON_TIMEOUT;
							this.DisplayMessage("LoadRack|Max Retries Exceeded", null, 0, this.MESSAGE_TIMEOUT);
							this.ConsecutivePrompts = 0;
							return;
						}

						this.StationState = StationState.PRESET_PROMPT;
						string MaximumPreset = this.CurrentMaximum.ToString(
							"N", this.SiteManager.Site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME));
						this.DisplayMessage(
							"[LoadRack|Above] [LoadRack|Maximum] " + MaximumPreset + ", [LoadRack|Enter] [LoadRack|Preset]",
							MaximumPreset,
							10,
							this.PROMPT_TIMEOUT);
						return;
					}
				}
				catch
				{
					this.ConsecutivePrompts++;
					if (this.ConsecutivePrompts >= this.SiteManager.Site._MaximumPrompts)
					{
						this.StationState = StationState.RESET_ON_TIMEOUT;
						this.DisplayMessage("LoadRack|Max Retries Exceeded", null, 0, this.MESSAGE_TIMEOUT);
						this.ConsecutivePrompts = 0;
						return;
					}

					this.StationState = StationState.PRESET_PROMPT;
					this.DisplayMessage("[LoadRack|Invalid], [LoadRack|Enter] [LoadRack|Preset]", null, 10, this.PROMPT_TIMEOUT);
					return;
				}

				LineItem.PresetAmount = PresetAmount;

				if (this.AvailableCompartments(this.CurrentEquipment) > 1)
				{
					this.IssueCompartmentSummaryPrompt();
				}

				else if (this.LoadSummaryIssued)
				{
					this.IssueLoadSummaryPrompt();
				}

				else if (this.CurrentEquipment == this.TractorOrTanker && this.AvailableCompartments(this.Trailer1) > 0)
				{
					this.CurrentEquipment = this.Trailer1;
					this.IssueCompartmentSummaryPrompt();
				}

				else if (this.CurrentEquipment == this.Trailer1 && this.AvailableCompartments(this.Trailer2) > 0)
				{
					this.CurrentEquipment = this.Trailer2;
					this.IssueCompartmentSummaryPrompt();
				}

				else
				{
					this.IssueLoadSummaryPrompt();
				}
			}
		}

		protected virtual void ProcessProduct(string response)
		{
			try
			{
				if (this.CurrentMenuParameters != null)
				{
					int selection = System.Convert.ToInt32(response);

					if (selection == 0 || selection > this.CurrentMenuParameters.Menu.Length)
					{
						this.IssueAdditionalOrdersPrompt();
						return;
					}

					response = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(x => x.Get(this.SiteManager.Site.SiteGuid, this.CurrentMenuParameters.Menu[selection - 1]));
				}
			}
			catch (FormatException)
			{
			}
			catch (OverflowException)
			{
			}

			if (response == EscapeString)
			{
				if (this.AvailableCompartments(this.CurrentEquipment) > 1)
				{
					this.IssueCompartmentSummaryPrompt();
				}
				else
				{
					if (this.AvailableCompartments(this.CurrentEquipment) > 1)
					{
						this.IssueCompartmentSummaryPrompt();
					}

					else if (this.LoadSummaryIssued)
					{
						this.IssueLoadSummaryPrompt();
					}

					else if (this.CurrentEquipment.MasterRecordGuid == (this.Trailer3?.MasterRecordGuid ?? Guid.Empty)
						 && this.AvailableCompartments(this.Trailer2) > 0)
					{
						this.CurrentEquipment = this.Trailer2;
						this.IssueCompartmentSummaryPrompt();
					}

					else if (this.CurrentEquipment.MasterRecordGuid == (this.Trailer2?.MasterRecordGuid ?? Guid.Empty)
						 && this.AvailableCompartments(this.Trailer1) > 0)
					{
						this.CurrentEquipment = this.Trailer1;
						this.IssueCompartmentSummaryPrompt();
					}

					else if (this.CurrentEquipment.MasterRecordGuid == (this.Trailer1?.MasterRecordGuid ?? Guid.Empty)
								&& this.AvailableCompartments(this.TractorOrTanker) > 0)
					{
						this.CurrentEquipment = this.TractorOrTanker;
						this.IssueCompartmentSummaryPrompt();
					}

					else if (this.Order != null)
					{
						if (this.SiteManager.Site.PromptForShipmentNumber)
						{
							this.IssueEnterShipmentNumberPrompt();
						}
						else
						{
							this.IssueEnterOrderNumberPrompt();
						}
					}

					else
					{
						this.IssueLoadIDPrompt();
					}
				}
			}
			else
			{
				LineItemDO lineItem;
				if (this.Station.Type == STATION_TYPE.MANUAL_BOL)
				{
					if (this.CurrentLineItemBaseIndex == -1)
					{
						lineItem = new LineItemDO
						{
							LoadingLocationID = this.Station.ID,
							LoadingLocationStationGuid = this.Station.IdentityGuid
						};
						this.Transaction.LineItems.Add(lineItem);
						this.CurrentLineItemBaseIndex = this.Transaction.LineItems.Count - 1;
					}
					else
					{
						lineItem = this.Transaction.LineItems[this.CurrentLineItemBaseIndex];
					}
				}
				else
				{
					lineItem = this.CurrentLineItem;
				}

				if (response == FMChannelHelper.MakeCall<IDataDictionariesClass, string>(x => x.Get(this.SiteManager.Site.SiteGuid, "LoadRack|None")))
				{
					lineItem.Product = null;
					lineItem.ProductGuid = Guid.Empty;
					lineItem.ProductCode = null;
					lineItem.ProductType = null;
					lineItem.CustomerProductName = null;
					lineItem.CustomerProductCode = null;
					lineItem.PresetAmount = null;
					lineItem.StorageLocationID = null;
					lineItem.StorageLocationTankGuid = Guid.Empty;
					lineItem.Density = null;
					lineItem.VCF = null;
					lineItem.OrderReferenceTransactionLineItemGuid = Guid.Empty;

					if (!this.ProductsConfigured)
					{
						this.ByWeight = false;
						this.ByWeightProduct = string.Empty;
					}

					if (this.Station.Type == STATION_TYPE.MANUAL_BOL)
					{
						this.IssueLineItemSummaryPrompt();
					}
					else if (this.AvailableCompartments(this.CurrentEquipment) > 1)
					{
						this.IssueCompartmentSummaryPrompt();
					}
					else
					{
						this.IssueLoadSummaryPrompt();
					}
				}
				else
				{
					UnitsHelperClass unitsHelper = new UnitsHelperClass(this.Security, this.SiteManager.Site, this.CurrentTransactionAlias, null);

					foreach (ProductMapClass authorizedProduct in this.ShipTo.AuthorizedProductCollection)
					{
						if (response != GetLoadRackDisplayText(authorizedProduct))
						{
							continue;
						}

						ProductClass product = this.GetByProductAuthorizedCompanies(
							this.Security, authorizedProduct.AssignedGuid, false);
						unitsHelper.Product = product;

						this.CurrentTransactionAlias = this.GetTransactionAlias(
							this.Security,
							product.LoadByWeight
								? this.Station.IssueByWeightTransactionAliasGuid
								: this.Station.IssueByVolumeTransactionAliasGuid,
							false);

						AdditiveProfileClass additiveProfile = null;
						if (authorizedProduct.AdditiveProfileGuid != Guid.Empty)
						{
							additiveProfile = this.GetAdditiveProfiles(this.Security, authorizedProduct.AdditiveProfileGuid);
						}

						lineItem.Product = authorizedProduct.AssignedID;
						lineItem.ProductCode = authorizedProduct.AssignedCode;
						lineItem.ProductType = ProductClass.ProductTypeID(authorizedProduct.AssignedProductType);
						lineItem.CustomerProductName = authorizedProduct.ShipToProductID;
						lineItem.CustomerProductCode = authorizedProduct.ShipToProductCode;
						lineItem.ProductGuid = FMChannelHelper.MakeCall<IProducts, Guid>(x => x.GetMasterRecordGuid(this.Security, authorizedProduct.AssignedGuid)); ;

						unitsHelper.SetUnits(lineItem, 0, product);

						// Density and VCF are set to default values.  When Tank is determined
						// these are reset to values from SCADA.
						SIDouble standardDensity = new SIDouble
						{
							Units = lineItem.DensityUnits,
							SIValue = product._StandardDensity.SIValue
						};
						// units;
						lineItem.Density = standardDensity.Value;

						lineItem.VCF = 1.0;

						SIDouble standardTemperature = new SIDouble
						{
							Units = lineItem.TemperatureUnits,
							SIValue = product._VcfModuleSettings.BaseTemperature.Value
						};
						// units;
						lineItem.Temperature = standardTemperature.Value;

						IQualityAssurance qaInterface = this.GetQualityAssuranceInterface();

						bool productAvailable = false;
						bool additiveProfileAvailable = additiveProfile == null;
						bool tankCertified = false;
						bool certificateOfAnalysis = false;

						if (product.ProductType == ProductType.ComponentProduct)
						{
							foreach (StationManagerClass stationManager in this.SiteManager.StationManagerCollection)
							{
								bool stationProductAvailable = false;
								bool stationAdditiveProfileAvailable = additiveProfile == null;
								bool stationTankCertified = false;
								bool stationCertificateOfAnalysis = false;

								if (!stationManager.Station.Enabled)
								{
									continue;
								}

								if (stationManager.Station.Type != STATION_TYPE.LOAD_RACK)
								{
									continue;
								}

								if (product.LoadByWeight)
								{
									// This is a bit confusing because in SiteLoadRackPage the alias is stored in the IssueByVolumeTransactionAliasIndex
									// even though the station may be a manual station created to represent a Load By Weight Station
									if (stationManager.Station.IssueByVolumeTransactionAliasGuid != this.Station.IssueByWeightTransactionAliasGuid)
									{
										continue;
									}
								}
								else
								{
									if (stationManager.Station.IssueByVolumeTransactionAliasGuid != this.Station.IssueByVolumeTransactionAliasGuid)
									{
										continue;
									}
								}

								TankClass tank = null;

								foreach (LoadArmManagerClass loadArmManager in stationManager.LoadArmManagerCollection)
								{
									stationProductAvailable = false;
									stationAdditiveProfileAvailable = additiveProfile == null;
									stationTankCertified = false;
									stationCertificateOfAnalysis = false;

									if (stationManager != loadArmManager.GetStationManager())
									{
										continue;
									}

									if (!loadArmManager.IsProductServedByLoadArm(product))
									{
										continue;
									}

									ProductMapClass loadArmComponent = loadArmManager.GetComponent(product.IdentityGuid);
									if (loadArmComponent == null)
									{
										continue;
									}

									tank = this.SiteManager.GetTank(loadArmComponent, this.Manager);
									if (tank == null)
									{
										continue;
									}

									try
									{
										SIDouble productDensity = new SIDouble
										{
											Units = lineItem.DensityUnits,
											SIValue = this.GetTankValue(tank, PROCESS_VARIABLE_TYPE.DENSITY_PV)
										};

										double productVcf = this.GetTankValue(tank, PROCESS_VARIABLE_TYPE.VCF_PV);

										double productPressure = this.GetTankValue(tank, PROCESS_VARIABLE_TYPE.VAPOR_PRESSURE_PV);

										SIDouble productTemperature = new SIDouble
										{
											Units = lineItem.TemperatureUnits,
											SIValue =
																					this.GetTankValue(tank, PROCESS_VARIABLE_TYPE.TEMPERATURE_PV)
										};
										// units;

										lineItem.Density = productDensity.Value;
										lineItem.VCF = productVcf;
										lineItem.Temperature = productTemperature.Value;
										this.CurrentMaximum = this.GetMaximum(product, lineItem.Density, lineItem.VCF, productPressure, null, lineItem);

										if (
											!stationManager.IsProductAvailable(
												product, loadArmManager, this.CurrentMaximum, this.CurrentTransactionAlias))
										{
											continue;
										}

										productAvailable = true;
										stationProductAvailable = true;

										if (!loadArmManager.IsAdditiveProfileServedByLoadArm(additiveProfile))
										{
											continue;
										}

										additiveProfileAvailable = true;
										stationAdditiveProfileAvailable = true;
									}
									catch (Exception e)
									{
										this.eventLog.WriteEntry("StationManager ProcessProduct : " + e.Message, EventLogEntryType.Error);
										continue;
									}

									if (qaInterface == null)
									{
										tankCertified = true;
										certificateOfAnalysis = true;
										stationTankCertified = true;
										stationCertificateOfAnalysis = true;
									}
									else
									{
										if (!qaInterface.GetTankCertification(this.Security, tank.IdentityGuid, product.IdentityGuid))
										{
											continue;
										}

										tankCertified = true;
										stationTankCertified = true;

										FailedTestItem[] testItems;
										if (
											!qaInterface.GetCertificateOfAnalysis(
												this.Security,
												tank.IdentityGuid,
												product.IdentityGuid,
												this.Owner.MasterRecordGuid,
												this.BillTo.MasterRecordGuid,
												this.ShipTo.MasterRecordGuid,
												out testItems))
										{
											continue;
										}

										certificateOfAnalysis = true;
										stationCertificateOfAnalysis = true;
									}

									break;
								}

								// Break if Load Arm Identified for Loading
								if (stationProductAvailable && stationAdditiveProfileAvailable && stationTankCertified
									 && stationCertificateOfAnalysis)
								{
									// If Loading By Weight Record the Storage Location
									if (product.LoadByWeight && tank != null)
									{
										lineItem.StorageLocationID = tank.ID;
										lineItem.StorageLocationTankGuid = tank.IdentityGuid;
									}

									break;
								}
							}

							// if product fails on all load arms
							if (!productAvailable)
							{
								this.AddAlarmAndEventLogs(this.Security, this.Station.ProductUnavailableAlarm(product.ID));
							}

							else if (!additiveProfileAvailable)
							{
								this.AddAlarmAndEventLogs(this.Security, this.Station.AdditiveProfileUnavailableAlarm(additiveProfile.ID));
							}

							else if (!tankCertified)
							{
								this.AddAlarmAndEventLogs(this.Security, this.Station.NoTankCertificationAlarm(product.ID));
							}

							else if (!certificateOfAnalysis)
							{
								this.AddAlarmAndEventLogs(this.Security, this.Station.FailedCertificateOfAnalysisAlarm(this.ShipTo.ID, product.ID));
							}
						}

						// For Blends see if product is available as a Splash Blend
						else if (product.ProductType == ProductType.BlendProduct)
						{
							// For Blends see if product is available as a Splash Blend
							productAvailable = true;
							additiveProfileAvailable = true;
							tankCertified = true;
							certificateOfAnalysis = true;
							bool blendComponentsCoa = true;
							if (qaInterface != null)
							{
								blendComponentsCoa = qaInterface.BlendComponentsCOA(this.Security, product.MasterRecordGuid);
							}

							SIDouble productDensity = new SIDouble { Units = lineItem.DensityUnits, SIValue = 0 };
							double productVcf = 0;

							SIDouble productTemperature = new SIDouble { Units = lineItem.TemperatureUnits, SIValue = 0 };

							if (!blendComponentsCoa)
							{
								FailedTestItem[] testItems;
								certificateOfAnalysis = qaInterface.GetCertificateOfAnalysis(this.Security,
																													 Guid.Empty,
																													 product.MasterRecordGuid,
																													 this.Owner.MasterRecordGuid,
																													 this.BillTo.MasterRecordGuid,
																													 this.ShipTo.MasterRecordGuid,
																													 out testItems);
								if (!certificateOfAnalysis)
								{
									FMChannelHelper.MakeCall<IAlarmAndEventLogs>(x => x.Add(this.Security, this.Station.FailedCertificateOfAnalysisAlarm(this.ShipTo.ID, product.ID)));
									this.LoadRackManager.EventOrAlarmEvent.Set();
								}
							}

							this.CurrentMaximum = 0;

							foreach (ProductMapClass component in product.ComponentCollection)
							{
								bool componentProductAvailable = false;
								bool componentAdditiveProfileAvailable = additiveProfile == null;
								bool componentTankCertified = false;
								bool componentCertificateOfAnalysis = false;

								ProductClass componentProduct = FMChannelHelper.MakeCall<IProducts, ProductClass>(x => x.Get(this.Security, component.AssignedGuid));

								foreach (StationManagerClass stationManager in this.SiteManager.StationManagerCollection)
								{
									if (component.LockedOut)
									{
										continue;
									}

									if (!stationManager.Station.Enabled)
									{
										continue;
									}

									if (stationManager.Station.Type != STATION_TYPE.LOAD_RACK)
									{
										continue;
									}

									if (product.LoadByWeight)
									{
										// This is a bit confusing because in SiteLoadRackPage the alias is stored in the IssueByVolumeTransactionAliasIndex
										// even though the station may be a manual station created to represent a Load By Weight Station
										if (stationManager.Station.IssueByVolumeTransactionAliasGuid
											 != this.Station.IssueByWeightTransactionAliasGuid)
										{
											continue;
										}
									}
									else
									{
										if (stationManager.Station.IssueByVolumeTransactionAliasGuid
											 != this.Station.IssueByVolumeTransactionAliasGuid)
										{
											continue;
										}
									}

									bool loadAsRecipe = stationManager.IsProductAvailable(additiveProfile, authorizedProduct);

									foreach (LoadArmManagerClass loadArmManager in stationManager.LoadArmManagerCollection)
									{
										if (stationManager != loadArmManager.GetStationManager())
										{
											continue;
										}

										if (loadAsRecipe && !loadArmManager.IsProductServedByLoadArm(product))
										{
											continue;
										}

										if (!loadAsRecipe && !loadArmManager.IsProductServedByLoadArm(componentProduct))
										{
											continue;
										}

										ProductMapClass loadArmComponent = loadArmManager.GetComponent(componentProduct.IdentityGuid);
										if (loadArmComponent == null)
										{
											continue;
										}

										TankClass tank = this.SiteManager.GetTank(loadArmComponent, this.Manager);
										if (tank == null)
										{
											continue;
										}

										SIDouble componentDensity = new SIDouble();
										double componentVcf;
										SIDouble componentTemperature = new SIDouble();

										try
										{
											componentDensity.Units = lineItem.DensityUnits;
											componentDensity.SIValue = this.GetTankValue(tank, PROCESS_VARIABLE_TYPE.DENSITY_PV);

											componentVcf = this.GetTankValue(tank, PROCESS_VARIABLE_TYPE.VCF_PV);

											componentTemperature.Units = lineItem.TemperatureUnits;
											componentTemperature.SIValue = this.GetTankValue(tank, PROCESS_VARIABLE_TYPE.TEMPERATURE_PV);

											double componentPressure = this.GetTankValue(tank, PROCESS_VARIABLE_TYPE.VAPOR_PRESSURE_PV);
											double componentMaximum = this.GetMaximum(componentProduct, componentDensity.Value, componentVcf, componentPressure, component, lineItem);

											if (
												 !stationManager.IsProductAvailable(
													  componentProduct, loadArmManager, componentMaximum, this.CurrentTransactionAlias))
											{
												continue;
											}

											componentProductAvailable = true;

											if (!loadArmManager.IsAdditiveProfileServedByLoadArm(additiveProfile))
											{
												continue;
											}

											componentAdditiveProfileAvailable = true;
										}
										catch (Exception e)
										{
											this.eventLog.WriteEntry("StationManager ProcessProduct : " + e.Message, EventLogEntryType.Error);
											continue;
										}

										if (qaInterface == null)
										{
											componentTankCertified = true;
											componentCertificateOfAnalysis = true;
										}
										else
										{
											if (!qaInterface.GetTankCertification(this.Security, tank.IdentityGuid, componentProduct.MasterRecordGuid))
											{
												continue;
											}

											componentTankCertified = true;

											if (blendComponentsCoa)
											{
												FailedTestItem[] testItems;
												if (
													 !qaInterface.GetCertificateOfAnalysis(
														  this.Security,
														  tank.IdentityGuid,
														  componentProduct.MasterRecordGuid,
														  this.Owner.MasterRecordGuid,
														  this.BillTo.MasterRecordGuid,
														  this.ShipTo.MasterRecordGuid,
														  out testItems))
												{
													continue;
												}
											}

											componentCertificateOfAnalysis = true;
										}

										productDensity.SIValue += componentDensity.SIValue * component.BlendPercentage / 100;
										productVcf += componentVcf * component.BlendPercentage / 100;
										productTemperature.SIValue += componentTemperature.SIValue * component.BlendPercentage / 100;
										break;
									}

									// Break if component succeeeds on any load arm
									if (componentProductAvailable && componentAdditiveProfileAvailable && componentTankCertified
										 && componentCertificateOfAnalysis)
									{
										break;
									}
								}

								// if component fails on all load arms
								if (!componentProductAvailable)
								{
									FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
										 x =>
											  x.Add(
													this.Security,
													this.Station.ProductUnavailableAlarm(
														 componentProduct.ID,
														 this.Driver.ID)));
									this.LoadRackManager.EventOrAlarmEvent.Set();
									productAvailable = false;
								}
								else if (!componentAdditiveProfileAvailable)
								{
									FMChannelHelper.MakeCall<IAlarmAndEventLogs>(x => x.Add(this.Security, this.Station.AdditiveProfileUnavailableAlarm(additiveProfile.ID)));
									this.LoadRackManager.EventOrAlarmEvent.Set();
									additiveProfileAvailable = false;
								}
								else if (!componentTankCertified)
								{
									FMChannelHelper.MakeCall<IAlarmAndEventLogs>(x => x.Add(this.Security, this.Station.NoTankCertificationAlarm(componentProduct.ID)));
									this.LoadRackManager.EventOrAlarmEvent.Set();
									tankCertified = false;
								}
								else if (!componentCertificateOfAnalysis)
								{
									FMChannelHelper.MakeCall<IAlarmAndEventLogs>(x => x.Add(this.Security, this.Station.FailedCertificateOfAnalysisAlarm(this.ShipTo.ID, componentProduct.ID)));
									this.LoadRackManager.EventOrAlarmEvent.Set();
									certificateOfAnalysis = false;
								}
							}

							if (productAvailable)
							{
								lineItem.Density = productDensity.Value;
								lineItem.VCF = productVcf;
								lineItem.Temperature = productTemperature.Value;
							}
						}

						if (product.LoadByWeight)
						{
							this.ByWeight = true;
							this.ByWeightProduct = response;
						}

						if (!productAvailable)
						{
							this.StationState = StationState.PRODUCT_UNAVAILABLE_MESSAGE;
							this.DisplayMessageWithAcknowledge("[LoadRack|Product Unavailable], ");
							return;
						}

						else if (!additiveProfileAvailable)
						{
							this.StationState = StationState.ADDITIVE_PROFILE_UNAVAILABLE_MESSAGE;
							this.DisplayMessageWithAcknowledge("[LoadRack|Additive Profile Unavailable], ");
							return;
						}

						else if (!tankCertified)
						{
							this.StationState = StationState.TANK_NOT_CERTIFIED_MSG;
							this.DisplayMessageWithAcknowledge("[LoadRack|Tank is not Certified], ");
							return;
						}

						else if (!certificateOfAnalysis)
						{
							this.StationState = StationState.FAILED_CERTIFICATE_OF_ANALYSIS_MSG;
							this.DisplayMessageWithAcknowledge("[LoadRack|Failed Certificate of Analysis], ");
							return;
						}

						if (product.LoadByWeight)
						{
							if ((this.Mode == OperatingMode.Loading && this.Station.IssueByWeightTransactionAliasGuid.IsEmpty())
								 || (this.Mode == OperatingMode.Unloading && this.Station.ReceiptByWeightTransactionAliasGuid.IsEmpty()))
							{
								this.ByWeight = false;
								this.ByWeightProduct = "";

								this.StationState = StationState.TRANSACTION_ALIAS_INVALID_MSG;
								this.DisplayMessage("LoadRack|Transaction Alias Invalid", null, 0, this.MESSAGE_TIMEOUT);
								return;
							}

							if (this.AvailableCompartments(this.CurrentEquipment) > 1)
							{
								this.IssueCompartmentSummaryPrompt();
							}

							else if (this.LoadSummaryIssued)
							{
								this.IssueLoadSummaryPrompt();
							}

							else if (this.CurrentEquipment.MasterRecordGuid == (this.TractorOrTanker?.MasterRecordGuid ?? Guid.Empty) && this.AvailableCompartments(this.Trailer1) > 0)
							{
								this.CurrentEquipment = this.Trailer1;
								this.IssueCompartmentSummaryPrompt();
							}

							else if (this.CurrentEquipment.MasterRecordGuid == (this.Trailer1?.MasterRecordGuid ?? Guid.Empty) && this.AvailableCompartments(this.Trailer2) > 0)
							{
								this.CurrentEquipment = this.Trailer2;
								this.IssueCompartmentSummaryPrompt();
							}

							else if (this.CurrentEquipment.MasterRecordGuid == (this.Trailer2?.MasterRecordGuid ?? Guid.Empty) && this.AvailableCompartments(this.Trailer3) > 0)
							{
								this.CurrentEquipment = this.Trailer3;
								this.IssueCompartmentSummaryPrompt();
							}

							else
							{
								this.IssueLoadSummaryPrompt();
							}

							return;
						}

						if ((this.Mode == OperatingMode.Loading && this.Station.IssueByVolumeTransactionAliasGuid.IsEmpty())
					  || (this.Mode == OperatingMode.Unloading && this.Station.ReceiptByVolumeTransactionAliasGuid.IsEmpty()))
						{
							this.StationState = StationState.TRANSACTION_ALIAS_INVALID_MSG;
							this.DisplayMessage("LoadRack|Transaction Alias Invalid", null, 0, this.MESSAGE_TIMEOUT);
							return;
						}

						this.IssuePresetPrompt();
						return;
					}

					this.StationState = StationState.PRODUCT_UNAVAILABLE_MESSAGE;
					this.DisplayMessageWithAcknowledge("[LoadRack|Product Unavailable], ");
				}
			}
		}

		protected ProductClass GetProduct(SecurityClass securityClass, Guid guid)
		{
			return FMChannelHelper.MakeCall<IProducts, ProductClass>(
																	 x =>
																	 x.Get(securityClass, guid)
																);
		}

		private ProductClass GetProductMinimalInfo(SecurityClass securityClass, Guid guid)
		{
			return FMChannelHelper.MakeCall<IProducts, ProductClass>(
																						x =>
																						x.GetByInfoAuthorizedCompanies(securityClass, guid, true, false, false)
																				 );
		}

		  /// <summary>
		  /// Gets the maximum quantity allowed to load from a sales order.
		  /// 
		  /// This only applies on a sales order with a single line item.
		  /// Multiple line item sales orders, we can't determine the line order to
		  /// use prior to product selection.
		  /// </summary>
		  /// <returns>
		  /// Maximum allowable load<see cref="double"/>.
		  /// </returns>
		  /// <remarks>
		  /// Determines the maximum amount of product allowed to be loaded.
		  /// Takes into account:
		  /// <list type="bullet">
		  /// <item>
		  /// <description>Amount remaining on order (if applicable)</description>
		  /// </item>
		  /// </list>
		  /// </remarks>
		  internal double GetMaximumFromSalesOrderOnly()
		  {
				EngineeringUnit volumeUnits = (this.CurrentTransactionAlias.VolumeUnits != 0) ? this.CurrentTransactionAlias.VolumeUnits : this.SiteManager.Site.VolumeUnits;
				EngineeringUnit massUnits = (this.CurrentTransactionAlias.MassUnits != 0)
												? this.CurrentTransactionAlias.MassUnits
												: this.SiteManager.Site.MassUnits;

				var maximum = new SIDouble { Units = volumeUnits, SIValue = this.SiteManager.Site._MaximumLoadAmount.SIValue };

				if (this.Order == null || !this.SiteManager.Site.EnforceSalesOrderLimit)
				{
					 // don't have an order or we're not enforcing the remaining amount as a limit.
					 return maximum.Value;
				}

				if (this.Order.LineItems == null || this.Order.LineItems.Count != 1)
				{
					 // don't have exactly one line item
					 return maximum.Value;
				}

				var volume = new SIDouble { Units = volumeUnits };

				// Limit the Maximum to the what remains on the order less
				// amounts committed on other line items.
				TransactionAliasClass orderTransactionAlias = null;
				TransactionDO order = null;
				orderTransactionAlias = this.OrderTransactionAlias;
				order = this.Order;

				EngineeringUnit orderVolumeUnits = (orderTransactionAlias.VolumeUnits != 0) ? orderTransactionAlias.VolumeUnits : this.SiteManager.Site.VolumeUnits;
				EngineeringUnit orderMassUnits = (orderTransactionAlias.MassUnits != 0) ? orderTransactionAlias.MassUnits : this.SiteManager.Site.MassUnits;

				bool orderByGross = false;
				// ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
				//foreach (TransactionAliasFieldClass field in this.OrderTransactionAlias.LineItemFieldCollection)
				foreach (TransactionAliasFieldClass field in orderTransactionAlias.LineItemFieldCollection)
				{
					 if (field.DbName == "GrossQuantity")
					 {
						  orderByGross = true;
						  break;
					 }
				}

				if (orderByGross)
				{
					 var grossQuantityRemaining = new SIDouble { Units = orderVolumeUnits };
					 var massQuantityRemaining = new SIDouble { Units = orderMassUnits };

					 LineItemDO activeOrderLineItem = order.LineItems[0] as LineItemDO;

					 grossQuantityRemaining.Value = activeOrderLineItem.GrossQuantityRemaining;
					 massQuantityRemaining.Value = activeOrderLineItem.MassQuantityRemaining;

					 // If we're here, we're loading against an order with only one line item, which only
					 // allows loading one product.  Therefore, we can be assured that 
					 // Adjust the Gross Quantity Remaining for existing line items
					 if (this.Transaction != null)
					 {
						  foreach (LineItemDO pendingLineItem in this.Transaction.LineItems)
						  {
								if (pendingLineItem.Status == TransactionStatus.Completed)
								{
									 if (pendingLineItem.Quantity != null)
									 {
										  grossQuantityRemaining.Value -= pendingLineItem.Quantity.Gross;
										  massQuantityRemaining.Value -= pendingLineItem.Quantity.Mass; 
									 }
								}
								else
								{
									 if (pendingLineItem.PresetAmount != null)
									 {
										  if (this.SiteManager.Site.LoadByNet)
										  {
												grossQuantityRemaining.Value -= pendingLineItem.PresetAmount.Value / pendingLineItem.VCF.Value;
										  }
										  else
										  {
												grossQuantityRemaining.Value -= pendingLineItem.PresetAmount.Value;
										  }

										  massQuantityRemaining.Value -= pendingLineItem.PresetAmount.Value; // Preset may be volume or may be mass; we'll update bothe now and figure out which to use later
									 }
								}
						  }
					 }

					 // If order does not have a gross quantity (it is 0.0) and does have a mass quantity, assume the order is by mass.
					 // otherwise, assume by gross
					 if (activeOrderLineItem != null &&
						  activeOrderLineItem.Quantity != null &&
						  activeOrderLineItem.Quantity.Gross == 0.0 &&
						  activeOrderLineItem.Quantity.Mass != 0.0)
					 {
						  if (maximum.Value > massQuantityRemaining.Value)
						  {
								maximum.Value = massQuantityRemaining.Value;
						  }
					 }
					 else
					 {
						  if (maximum.Value > grossQuantityRemaining.Value)
						  {
								maximum.Value = grossQuantityRemaining.Value;
						  }
					 }
					 // ReSharper restore CompareOfFloatsByEqualityOperator
				}
				else
				{
					 var netQuantityRemaining = new SIDouble { Units = orderVolumeUnits };
					 var massQuantityRemaining = new SIDouble { Units = orderMassUnits };

					 LineItemDO activeOrderLineItem = this.Order.LineItems[0] as LineItemDO;
					 
					 netQuantityRemaining.Value = activeOrderLineItem.NetQuantityRemaining;
					 massQuantityRemaining.Value = activeOrderLineItem.MassQuantityRemaining;

					 // Adjust the NetQuantity Remaining for existing line items
					 if (this.Transaction != null)
					 {
						  foreach (LineItemDO pendingLineItem in this.Transaction.LineItems)
						  {
								if (pendingLineItem.Status == TransactionStatus.Completed)
								{
									 if (pendingLineItem.Quantity != null)
									 {
										  netQuantityRemaining.Value -= pendingLineItem.Quantity.Net;
										  massQuantityRemaining.Value -= pendingLineItem.Quantity.Mass;
									 }
								}
								else
								{
									 if (pendingLineItem.PresetAmount != null)
									 {
										  if (this.SiteManager.Site.LoadByNet)
										  {
												netQuantityRemaining.Value -= pendingLineItem.PresetAmount.Value;
										  }
										  else
										  {
												netQuantityRemaining.Value -= pendingLineItem.PresetAmount.Value * pendingLineItem.VCF.Value;
										  }

										  massQuantityRemaining.Value -= pendingLineItem.PresetAmount.Value; // Preset may be volume or may be mass; we'll update bothe now and figure out which to use later							
									 }
								}
						  }
					 }

					 // If order does not have a net quantity (it is 0.0) and does have a mass quantity, assume the order is by mass.
					 // otherwise, assume by gross
					 if (activeOrderLineItem != null &&
						  activeOrderLineItem.Quantity != null &&
						  activeOrderLineItem.Quantity.Net == 0.0 &&
						  activeOrderLineItem.Quantity.Mass != 0.0)
					 {
						  if (maximum.Value > massQuantityRemaining.Value)
						  {
								maximum.Value = massQuantityRemaining.Value;
						  }
					 }
					 else
					 {
						  if (maximum.Value > netQuantityRemaining.Value)
						  {
								maximum.Value = netQuantityRemaining.Value;
						  }
					 }
					 // ReSharper restore CompareOfFloatsByEqualityOperator
				}

				return maximum.Value;
		  }

		protected void ProcessPurchaseOrder(string response)
		{
			if (response == EscapeString)
			{
				if (this.Order != null)
				{
					if (this.SiteManager.Site.PromptForShipmentNumber)
					{
						this.IssueEnterShipmentNumberPrompt();
					}
					else
					{
						this.IssueEnterOrderNumberPrompt();
					}
				}
				else
				{
					this.IssueLoadIDPrompt();
				}
			}
			else
			{
				this.PONumber = response;

				if (this.Station.Type == STATION_TYPE.PRELOAD
				|| this.Station.Type == STATION_TYPE.WEIGHT_SCALE
				|| this.Station.Type == STATION_TYPE.MANUAL_BOL)
				{
					this.StationState = StationState.IDLE;
					this.CheckProductAvailability(false);
				}
				else
				{
					if (this.Transaction != null
						 && this.Transaction.DestinationEQ1.EquipmentType == EquipmentTypeClass.TypeID(EQUIPMENT_TYPE.TANKER_TYPE))
					{
						this.IssueEnterTankerPrompt();
					}
					else if (this.PromptForTractorOrTanker)
					{
						this.IssueTractorOrTankerPrompt();
					}
					else if (this.PromptForFirstTrailer)
					{
						this.IssueEnterTrailer1Prompt();
					}
					else
					{
						this.StationState = StationState.IDLE;
						this.CheckProductAvailability(false);
					}
				}
			}
		}

		protected void ProcessSelectSupplierOffLoadIDFilterColumn(string Response)
		{
			if (Response == EscapeString)
			{
				this.IssueOffLoadIDPrompt();
			}
			else
			{
				this.PriorStationState = this.StationState;
				string loadRackSupplierStr =
					FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
						x => x.Get(this.SiteManager.Site.SiteGuid, "[LoadRack|Supplier]"));
				string loadRackZipStr =
					FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
						x => x.Get(this.SiteManager.Site.SiteGuid, "[LoadRack|Zip]"));
				string loadRackCityStr =
					FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
						x => x.Get(this.SiteManager.Site.SiteGuid, "[LoadRack|City]"));
				string loadRackState =
					FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
						x => x.Get(this.SiteManager.Site.SiteGuid, "[LoadRack|State]"));
				if (loadRackZipStr == Response)
				{
					this.CustomerShipToFilterColumn = "Zip";
					this.IssueOffLoadEnterZipPrompt();
				}
				else if (loadRackSupplierStr == Response)
				{
					this.IssueSelectSupplierOffLoadIDFromCarrierShipToCollectionPrompt();
				}
				else
				{
					if (loadRackCityStr == Response)
					{
						this.CustomerShipToFilterColumn = "City";
					}
					else if (loadRackState == Response)
					{
						this.CustomerShipToFilterColumn = "State";
					}
					this.IssueSelectSupplierOffLoadIDFilterValuePrompt();
				}
			}
		}

		protected virtual void ProcessShipTo(string response)
		{
			try
			{
				int selection = System.Convert.ToInt32(response);

				if (selection == 0 || selection > this.CurrentMenuParameters.Menu.Length)
				{
					this.IssueAdditionalOrdersPrompt();
					return;
				}

				response = this.GetDataDictionaryValueByKey(
					 this.SiteManager.Site.SiteGuid,
					 this.CurrentMenuParameters.Menu[selection - 1]);
			}
			catch (FormatException)
			{
			}
			catch (OverflowException)
			{
			}

			if (response == EscapeString ||
				 this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, "LoadRack|No") == response)
			{
				this.ProcessShipmentLoadidResponse(this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, "LoadRack|Load ID"));
				return;
			}

			if (this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, "LoadRack|Yes") == response)
			{
				Guid companyMapGuid = FMChannelHelper.MakeCall<ICompanyMaps, Guid>(x => x.GetIdentityGuidByMapID(this.Security, this.LoadID));

				if (!this.ValidateLoadID(companyMapGuid))
				{
					return;
				}

				if (this.Station.Type == STATION_TYPE.LOAD_RACK)
				{
					if (this.inprogressTransaction != null)
					{
						foreach (TransactionDO transactionDO in this.inprogressTransaction)
						{
							if (transactionDO.LoadID == this.LoadID)
							{
								this.Transaction = transactionDO;
								break;
							}
						}
					}
				}

				this.IssueEnterPurchaseOrderPrompt();
			}
			else
			{
				this.DisplayMessage("[LoadRack|Invalid Selection]", null, 0, this.MESSAGE_TIMEOUT);
				this.StationState = StationState.INVALID_SHIPTO_PROMPT_RESPONSE_MESSAGE;
			}
		}

		protected void ProcessShipToShipmentNumberResponse(string response)
		{
			if (response == "1")
			{
				response = this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, this.CurrentMenuParameters.Menu[0]);
			}

			if (response == "2")
			{
				response = this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, this.CurrentMenuParameters.Menu[1]);
			}

			if (response == EscapeString || this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, "LoadRack|No") == response)
			{
				this.IssueEnterShipmentNumberPrompt();
				return;
			}
			else if (this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, "LoadRack|Yes") == response)
			{
				string companyHierarchy = this.Order.ManagerID + "->" + this.Order.OwnerID + "->" + this.Order.ShipperID + "->"
												  + this.Order.BillToID;
				this.CompanyMapCollection = FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapCollectionClass>(
						x =>
						x.EnumerateByAssignedGuidAndType(this.Security, this.ShipTo.MasterRecordGuid, COMPANY_MAP_TYPE.SHIPTO_BILLTO_MAP)
				);

				foreach (CompanyMapClass companyMap in this.CompanyMapCollection)
				{
					if (companyMap.AssignedToID == companyHierarchy)
					{
						if (!this.ValidateCompanyHierarchyLoadRack(companyMap))
						{
							this.AddAlarmAndEventLogs(this.Security, this.Station.CompanyHierarchyInvalidEvent(this.Driver.FirstLastName, this.ShipTo.ID));
							this.LoadRackManager.EventOrAlarmEvent.Set();

							this.StationState = StationState.RESET_ON_TIMEOUT;
							return;
						}

						if (this.Station.Type == STATION_TYPE.LOAD_RACK)
						{
							this.SetProductsInStation();
						}

						if (this.Station.Type == STATION_TYPE.PRELOAD || this.Station.Type == STATION_TYPE.WEIGHT_SCALE)
						{
							this.StationState = StationState.IDLE;
							this.CheckProductAvailability(false);
							return;
						}

						if (this.SiteManager.Site.PromptForTractorOrTanker)
						{
							// Prompt for Tractor/Tanker
							this.IssueTractorOrTankerPrompt();
						}

						else if (this.SiteManager.Site.PromptForTruckCard)
						{
							// Prompt for Truck Card
							this.IssueTractorCardInPrompt();
						}

						else if (this.SiteManager.Site.PromptForFirstTrailer)
						{
							// Prompt for First Trailer
							this.IssueEnterTrailer1Prompt();
						}

						else
						{
							// Prompt for LoadID
							this.IssueLoadIDPrompt();
						}
						return;
					}
				}

				if (!this.ValidateCompany(this.ShipTo, COMPANY_ROLE.CUSTOMER_SHIPTO)
					 || !this.ValidateCompany(this.BillTo, COMPANY_ROLE.CUSTOMER_BILLTO)
					 || !this.ValidateCompany(this.Shipper, COMPANY_ROLE.SHIPPER)
					 || !this.ValidateCompany(this.Owner, COMPANY_ROLE.OWNER)
					 || !this.ValidateCompany(this.Manager, COMPANY_ROLE.MANAGER))
				{
					this.AddAlarmAndEventLogs(this.Security, this.Station.CompanyHierarchyInvalidEvent(this.Driver.FirstLastName, this.ShipTo.ID));
					this.LoadRackManager.EventOrAlarmEvent.Set();

					this.Transaction = null;
					this.StationState = StationState.RESET_ON_TIMEOUT;
					return;
				}

				if (this.Station.Type == STATION_TYPE.PRELOAD || this.Station.Type == STATION_TYPE.WEIGHT_SCALE)
				{
					this.StationState = StationState.IDLE;
					this.CheckProductAvailability(false);
					return;
				}

				if (this.SiteManager.Site.PromptForTractorOrTanker)
				{
					// Prompt for Tractor/Tanker
					this.IssueTractorOrTankerPrompt();
				}

				else if (this.SiteManager.Site.PromptForTruckCard)
				{
					// Prompt for Truck Card
					this.IssueTractorCardInPrompt();
				}

				else if (this.SiteManager.Site.PromptForFirstTrailer)
				{
					// Prompt for First Trailer
					this.IssueEnterTrailer1Prompt();
				}

				else
				{
					// Prompt for LoadID
					this.IssueLoadIDPrompt();
				}
			}

			else
			{
				this.DisplayMessage("[LoadRack|Invalid Selection]", null, 0, this.MESSAGE_TIMEOUT);
				this.StationState = StationState.INVALID_PRELOAD_DOCUMENT_SELECTION_MSG;
			}
		}

		protected void ProcessShipmentLoadidResponse(string response)
		{
			try
			{
				int selection = System.Convert.ToInt32(response);

				if (selection == 0 || selection > this.CurrentMenuParameters.Menu.Length)
				{
					this.IssueAdditionalOrdersPrompt();
					return;
				}

				response = this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, this.CurrentMenuParameters.Menu[selection - 1]);
			}
			catch (FormatException)
			{
			}
			catch (OverflowException)
			{
			}

			if (response == EscapeString)
			{
				this.ResetStationDevice();
				return;
			}

			if (this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, "LoadRack|Load ID") == response)
			{
				this.IssueLoadIDPrompt();
			}
			else if (this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, "LoadRack|Shipment Number") == response)
			{
				this.IssueEnterShipmentNumberPrompt();
			}
			else
			{
				this.DisplayMessage("[LoadRack|Invalid Selection]", null, 0, this.MESSAGE_TIMEOUT);
				this.StationState = StationState.INVALID_PRELOAD_DOCUMENT_SELECTION_MSG;
			}
		}

		protected void ProcessShipmentNumber(string Response)
		{
			if (Response == EscapeString || Response == "")
			{
				if (this.Station.Type == STATION_TYPE.LOAD_RACK)
				{
					if (this.SiteManager.Site.PromptForShipmentNumber && !this.Station.InhibitLoadingByLoadID)
					{
						this.IssueLoadByShipmentOrLoadIDPrompt();
					}
					else
					{
						if (this.Station.CardReader)
						{
							this.IssuePleaseCardIn();
						}
						else if (this.Station.TouchKeyReader)
						{
							this.IssueTouchKeyPleaseCardIn();
						}
						else
						{
							this.IssueDriverIDPrompt();
						}
					}
				}
				else
				{
					this.IssueUseOrderNumberPrompt();
				}
				return;
			}

			string DocumentNumber;
			//			CardID=Response;
			// Check for preloads for the current driver
			GetTransactionSR getTransactionSR = new GetTransactionSR
			{
				Security = this.Security,
				Request = GetTransactionRequest.SITE_TYPEID_SHIPMENTNUMBER,
				Site = this.SiteManager.Site.ID,
				TransTypeID = TransactionTypes.T17_Order,
				Status = ((int)TransactionStatus.Scheduled).ToString(),
				ShipmentNumber = Response
			};

			GetTransactionDO getTransactionDO = FMChannelHelper.MakeCall<IGetTransactionProcessor, GetTransactionDO>(
																	 x =>
																	 x.Process(getTransactionSR)
																);

			if (getTransactionDO != null && getTransactionDO.TransactionDataSet != null
				 && getTransactionDO.TransactionDataSet.Tables.Count != 0
				 && getTransactionDO.TransactionDataSet.Tables[0].Rows.Count != 0)
			{
				foreach (DataRow Row in getTransactionDO.TransactionDataSet.Tables[0].Rows)
				{
					DocumentNumber = Row["TransID"] as string; //Row["DocumentNumber"] as string;
					if (string.IsNullOrEmpty(DocumentNumber) == false)
					{
						this.Order = this.GetTransaction(DocumentNumber);
						this.OrderTransactionAlias = FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasClass>(x => x.Get(this.Security, this.Order.TransactionAliasGuid, false));
						if (this.OrderTransactionAlias != null)
						{
							this.CurrentTransactionAlias = FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasClass>(x => x.Get(this.Security, this.OrderTransactionAlias.AssociatedTransactionAliasGuid, false));
						}

						if (this.Order.ShipToCompanyGuid != Guid.Empty)
						{
							this.ShipTo = this.GetCompany(this.Security, this.Order.ShipToCompanyGuid);
						}
						if (this.Order.BillToCompanyGuid != Guid.Empty)
						{
							this.BillTo = this.GetCompany(this.Security, this.Order.BillToCompanyGuid);
						}
						if (this.Order.ShipperCompanyGuid != Guid.Empty)
						{
							this.Shipper = this.GetCompany(this.Security, this.Order.ShipperCompanyGuid);
						}
						if (this.Order.OwnerCompanyGuid != Guid.Empty)
						{
							this.Owner = this.GetCompany(this.Security, this.Order.OwnerCompanyGuid);
						}
						if (this.Order.ManagerCompanyGuid != Guid.Empty)
						{
							this.Manager = this.GetCompany(this.Security, this.Order.ManagerCompanyGuid);
						}

						if (this.Station.Type == STATION_TYPE.WEIGHT_SCALE || this.Station.Type == STATION_TYPE.PRELOAD)
						{
							this.IssueEnterPurchaseOrderPrompt();
						}
						if (this.OrderTransactionAlias == null
							 || this.OrderTransactionAlias.AssociatedTransactionAliasGuid == Guid.Empty
							 || this.CurrentTransactionAlias == null
							 || this.CurrentTransactionAlias.MasterRecordGuid == Guid.Empty)
						{
							this.AddAlarmAndEventLogs(this.Security, this.Station.OrderAliasInvalidEvent(this.Station.ID));
							this.LoadRackManager.EventOrAlarmEvent.Set();

							this.StationState = StationState.RESET_ON_TIMEOUT;
							this.DisplayMessage("LoadRack|Order associated alias invalid", null, 0, this.MESSAGE_TIMEOUT);
							return;
						}
						else
						{
							this.IssueShipToMenu(true);
						}
						return;
					}
				}
			}

			this.ConsecutivePrompts++;
			if (this.ConsecutivePrompts >= this.SiteManager.Site._MaximumPrompts)
			{
				this.StationState = StationState.RESET_ON_TIMEOUT;
				this.DisplayMessage("LoadRack|Max Retries Exceeded", null, 0, this.MESSAGE_TIMEOUT);
				this.ConsecutivePrompts = 0;
				return;
			}
			this.StationState = StationState.SHIPMENTNUMBER_NOTFOUND;
			this.DisplayMessage("LoadRack|Invalid], [LoadRack|Enter Shipment Number]", null, PromptLength, this.PROMPT_TIMEOUT);
			return;
		}

		protected void ProcessSupplierOffLoadIDFilterValue(string Response)
		{
			if (Response == EscapeString)
			{
				if (this.PriorStationState == StationState.ENTER_OFFLOADID_ZIP_PROMPT)
				{
					this.IssueOffLoadEnterZipPrompt();
				}
				else
				{
					this.IssueSelectSupplierOffLoadIDFilterColumnPrompt();
				}
			}

			else if (Response == "?")
			{
				this.IssueSelectSupplierOffLoadIDFilterValuePrompt();
			}

			else
			{
				this.PriorStationState = this.StationState;
				this.CustomerShipToFilterValue = Response;
				this.IssueSelectSupplierFromFilterPrompt();
			}
		}

		protected void ProcessSupplierPrompt(string Response)
		{
			if (EscapeString == Response)
			{
				if (this.PriorStationState == StationState.SELECT_SUPPLIER_OFFLOADID_FILTER_PROMPT)
				{
					this.IssueSelectSupplierOffLoadIDFilterColumnPrompt();
				}
				else
				{
					this.IssueSelectSupplierOffLoadIDFilterValuePrompt();
				}
			}

			else
			{
				Guid identityGuid = Guid.Empty;

				CompanyCollectionClass CompanyCollection =
					FMChannelHelper.MakeCall<ICompanies, CompanyCollectionClass>(
						x =>
						x.EnumerateAuthorizedSupplierForColumnValue(
							this.Security, this.CustomerShipToFilterColumn, this.CustomerShipToFilterValue));

				if (CompanyCollection.Count > 0)
				{
					foreach (CompanyClass Company in CompanyCollection)
					{
						if (Company.CompanyToolTip == Response)
						{
							identityGuid = Company.IdentityGuid;
							break;
						}
					}
				}
				else
				{
					this.DisplayMessage("[LoadRack|Invalid], [LoadRack|Enter] [LoadRack|Supplier]", null, 20, this.MESSAGE_TIMEOUT);
					return;
				}

				if (identityGuid == Guid.Empty)
				{
					this.DisplayMessage("[LoadRack|Invalid], [LoadRack|Enter] [LoadRack|Supplier]", null, 20, this.MESSAGE_TIMEOUT);
					return;
				}

				this.Supplier = FMChannelHelper.MakeCall<ICompanies, CompanyClass>(x => x.Get(this.Security, identityGuid));

				if (!this.ValidateCompany(this.Supplier, COMPANY_ROLE.SUPPLIER))
				{
					return;
				}

				// now that we have the supplier we need to get the owner and manager to determine the off load id
				this.CompanyMapCollection =
					FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapCollectionClass>(
						x =>
						x.EnumerateByAssignedGuidAndType(this.Security, this.Supplier.MasterRecordGuid, COMPANY_MAP_TYPE.SUPPLIER_OWNER_MAP));

				this.CurrentCompanyHierarchyType = COMPANY_MAP_TYPE.SUPPLIER_OWNER_MAP;
				this.IssueSelectCompanyHierarchyPrompt();
			}
		}

		protected void ProcessTareWeight(string Response)
		{
			if (Response == EscapeString)
			{
				if (this.SiteManager.Site.PromptForSecondTrailer
					 && (this.TractorOrTanker == null || this.TractorOrTanker.Type == EQUIPMENT_TYPE.TRACTOR_TYPE))
				{
					this.IssueEnterTrailer2Prompt();
				}
				else if (this.SiteManager.Site.PromptForFirstTrailer)
				{
					this.IssueEnterTrailer1Prompt();
				}
				else if (this.SiteManager.Site.PromptForTractorOrTanker)
				{
					this.IssueTractorOrTankerPrompt();
				}
				else if (this.SiteManager.Site.PromptForTruckCard)
				{
					this.IssueTractorCardInPrompt();
				}
				else if (this.Station.InhibitOperatingModePrompt)
				{
					this.ProcessOperatingMode(Response);
				}
				else
				{
					this.IssueOperatingModePrompt();
				}
			}
			else
			{
				if (Response == this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, "Accept"))
				{
					this.IssueUseOrderNumberPrompt();
				}
				else
				{
					this.IssueCaptureTareWeightPrompt();
				}
			}
		}

		protected void ProcessTractorCardIn(string Response)
		{
			if (Response == EscapeString)
			{
				if (this.StationState == StationState.ENTER_TRACTOR_CARDIN_PROMPT)
				{
					if (this.Station.CardReader)
					{
						this.IssuePleaseCardIn();
					}
					else if (this.Station.TouchKeyReader)
					{
						this.IssueTouchKeyPleaseCardIn();
					}
					else
					{
						this.IssueDriverIDPrompt();
					}
				}
				else
				{
					this.ProcessOperatingMode(Response);
				}
			}
			else
			{
				Guid identityGuid;
				identityGuid = FMChannelHelper.MakeCall<IEquipments, Guid>(
							x =>
							x.GetIdentityGuidByCardNumberAndEquipmentID(this.Security, this.Carrier.MasterRecordGuid, Response)
					);

				if (identityGuid != Guid.Empty)
				{
					this.TractorOrTanker = FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(
																	 x =>
																	 x.Get(this.Security, identityGuid)
																);
				}

				// Invalid Tractor Number
				if (identityGuid == Guid.Empty || this.TractorOrTanker == null
					 || (this.TractorOrTanker.Type != EQUIPMENT_TYPE.TANKER_TYPE
						  && this.TractorOrTanker.Type != EQUIPMENT_TYPE.TRACTOR_TYPE))
				{
					this.AddAlarmAndEventLogs(this.Security, this.Station.InvalidTractorOfTankerIDEvent(Response));

					this.ConsecutivePrompts++;
					if (this.ConsecutivePrompts >= this.SiteManager.Site._MaximumPrompts)
					{
						this.StationState = StationState.RESET_ON_TIMEOUT;
						this.DisplayMessage("LoadRack|Max Retries Exceeded", null, 0, this.MESSAGE_TIMEOUT);
						this.ConsecutivePrompts = 0;
						return;
					}

					this.StationState = StationState.ENTER_TRACTOR_CARDIN_PROMPT;
					if (this.Station.TouchKeyReader)
					{
						this.DisplayMessage("[LoadRack|Scan] [LoadRack|Truck Key]", null, PromptLength, this.PROMPT_TIMEOUT);
					}
					else
					{
						this.DisplayMessage(
							"[LoadRack|Invalid] [LoadRack|Scan] [LoadRack|Truck Card]", null, PromptLength, this.PROMPT_TIMEOUT);
					}
					return;
				}

				// check if this equipment can be used at this station
				if (this.IsEquipmentValidForThisStation(this.TractorOrTanker) == false)
				{
					return;
				}

				//check if the station required equipment tags or licenses are attached
				if (this.IsEquipmentLicenseValidForThisStation(this.TractorOrTanker) == false)
				{
					return;
				}

				// check the qualifications and training required for this piece of equipment
				if (this.CheckDriverEquipmentQualsAndTraining(this.TractorOrTanker) == false)
				{
					return;
				}

				if (this.SiteManager.Site.EnforceDriverEquipmentMatch
					 && this.TractorOrTanker.CompanyGuid != this.Carrier.MasterRecordGuid)
				{
					this.ConsecutivePrompts++;
					if (this.ConsecutivePrompts >= this.SiteManager.Site._MaximumPrompts)
					{
						this.StationState = StationState.RESET_ON_TIMEOUT;
						this.DisplayMessage("LoadRack|Max Retries Exceeded", null, 0, this.MESSAGE_TIMEOUT);
						this.ConsecutivePrompts = 0;
						return;
					}

					this.StationState = StationState.ENTER_TRACTOR_CARDIN_PROMPT;
					if (this.Station.TouchKeyReader)
					{
						this.DisplayMessage("[LoadRack|Scan] [LoadRack|Truck Key]", null, PromptLength, this.PROMPT_TIMEOUT);
					}
					else
					{
						this.DisplayMessage(
							"[LoadRack|Invalid] [LoadRack|Scan] [LoadRack|Truck Card]", null, PromptLength, this.PROMPT_TIMEOUT);
					}
					return;
				}

				this.StationState = StationState.IDLE;
				this.ConsecutivePrompts = 0;
				// change to process card id
				this.CompleteTractorOrTankerProcessing(false);
			}
		}

		protected void ProcessTractorOrTankerID(string response)
		{
			if (string.IsNullOrEmpty(response))
			{
				return;
			}

			if (response == EscapeString)
			{
				if (this.StationState == StationState.SELECT_TRACTOR_OR_TANKER_PROMPT)
				{
					this.IssueTractorOrTankerPrompt();
				}
				else
				{
					if (this.Station.Type == STATION_TYPE.WEIGHT_SCALE || this.Station.Type == STATION_TYPE.PRELOAD)
					{
						if (this.Station.InhibitOperatingModePrompt)
						{
							this.ProcessOperatingMode(response);
						}
						else
						{
							this.IssueOperatingModePrompt();
						}
					}
					else
					{
						this.ProcessOperatingMode(response);
					}
				}
			}
			else if (response == "?")
			{
				if (!this.SiteManager.Site.ListEquipment)
				{
					this.StationState = StationState.NOT_AVAILABLE_TRACTOR_OR_TANKER_PROMPT;
					this.DisplayMessage("[LoadRack|List is not available]", null, 0, this.MESSAGE_TIMEOUT);
				}

				else
				{
					this.IssueSelectTractorOrTankerPrompt();
				}
			}
			else
			{
				Guid identityGuid;
				if (this.SiteManager.Site.UseCompanyEquipmentIdentifiers)
				{
					identityGuid = FMChannelHelper.MakeCall<IEquipments, Guid>(
								x =>
								x.GetIdentityGuidByCompanyGuidAndEquipmentID(this.Security, this.Carrier.MasterRecordGuid, response)
						);
				}
				else
				{
					identityGuid = FMChannelHelper.MakeCall<IEquipments, Guid>(
																	 x =>
																	 x.GetIdentityGuid(this.Security, response)
																);
				}

				if (identityGuid != Guid.Empty)
				{
					this.TractorOrTanker = FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(
																	 x =>
																	 x.Get(this.Security, identityGuid)
																);
				}

				// Invalid Tractor Number
				if (identityGuid == Guid.Empty || this.TractorOrTanker == null
					 || (this.TractorOrTanker.Type != EQUIPMENT_TYPE.TANKER_TYPE
						  && this.TractorOrTanker.Type != EQUIPMENT_TYPE.TRACTOR_TYPE))
				{
					this.AddAlarmAndEventLogs(this.Security, this.Station.InvalidTractorOfTankerIDEvent(response));

					this.ConsecutivePrompts++;
					if (this.ConsecutivePrompts >= this.SiteManager.Site._MaximumPrompts)
					{
						this.StationState = StationState.RESET_ON_TIMEOUT;
						this.DisplayMessage("LoadRack|Max Retries Exceeded", null, 0, this.MESSAGE_TIMEOUT);
						this.ConsecutivePrompts = 0;
						return;
					}

					this.StationState = StationState.TRACTOR_OR_TANKER_PROMPT;
					this.DisplayMessage(
						"[LoadRack|Invalid], [LoadRack|Enter] [LoadRack|Tractor/Tanker]", null, PromptLength, this.PROMPT_TIMEOUT);
					return;
				}

				// check if this equipment can be used at this station
				if (this.IsEquipmentValidForThisStation(this.TractorOrTanker) == false)
				{
					return;
				}

				//check if the station required equipment tags or licenses are attached
				if (this.IsEquipmentLicenseValidForThisStation(this.TractorOrTanker) == false)
				{
					return;
				}

				// check the qualifications and training required for this peice of equipment
				if (this.CheckDriverEquipmentQualsAndTraining(this.TractorOrTanker) == false)
				{
					return;
				}

				if (this.SiteManager.Site.EnforceDriverEquipmentMatch
					 && this.TractorOrTanker.CompanyGuid != this.Carrier.MasterRecordGuid)
				{
					this.ConsecutivePrompts++;
					if (this.ConsecutivePrompts >= this.SiteManager.Site._MaximumPrompts)
					{
						this.StationState = StationState.RESET_ON_TIMEOUT;
						this.DisplayMessage("LoadRack|Max Retries Exceeded", null, 0, this.MESSAGE_TIMEOUT);
						this.ConsecutivePrompts = 0;
						return;
					}

					this.StationState = StationState.ENTER_TRACTOR_PROMPT;
					this.DisplayMessage(
						"[LoadRack|Mismatch], [LoadRack|Enter] [LoadRack|Tractor/Tanker]", null, PromptLength, this.PROMPT_TIMEOUT);
					return;
				}

				if (!this.CheckEquipmentOnOrder(this.TractorOrTanker))
				{
					if (this.CheckMaximumRetries())
					{
						return;
					}

					this.StationState = StationState.TRACTOR_OR_TANKER_PROMPT;
					this.DisplayMessage("[LoadRack|Equipment Not On Order], [LoadRack|Enter] [LoadRack|Tanker]", null, PromptLength, this.PROMPT_TIMEOUT);

					return;
				}

				if (this.TractorOrTanker.Type == EQUIPMENT_TYPE.TANKER_TYPE
				&& !this.CheckCompartmentsAvailable(this.TractorOrTanker))
				{
					if (this.CheckMaximumRetries())
					{
						return;
					}

					this.StationState = StationState.TRACTOR_OR_TANKER_PROMPT;
					this.DisplayMessage("[LoadRack|No Compartments Available], [LoadRack|Enter] [LoadRack|Tractor/Tanker]", null, PromptLength, this.PROMPT_TIMEOUT);

					return;
				}

				this.StationState = StationState.IDLE;
				this.ConsecutivePrompts = 0;
				this.CompleteTractorOrTankerProcessing(false);
			}
		}

		protected void ProcessTrailer1ID(string response)
		{
			if (string.IsNullOrEmpty(response))
			{
				return;
			}

			if (response == EscapeString)
			{
				if (this.StationState == StationState.SELECT_TRAILER1_PROMPT)
				{
					this.IssueTrailer1Prompt();
				}
				else
				{
					switch (this.Station.Type)
					{
						case STATION_TYPE.WEIGHT_SCALE:
							if (this.PromptForTractorOrTanker)
							{
								this.IssueTractorOrTankerPrompt();
							}
							else if (this.Station.InhibitOperatingModePrompt
										|| (this.Station.IssueByVolumeTransactionAliasGuid == Guid.Empty && this.Station.IssueByWeightTransactionAliasGuid == Guid.Empty)
										|| (this.Station.ReceiptByVolumeTransactionAliasGuid == Guid.Empty && this.Station.ReceiptByWeightTransactionAliasGuid == Guid.Empty))
							{
								this.ProcessOperatingMode(response);
							}
							else
							{
								this.IssueOperatingModePrompt();
							}
							break;
						case STATION_TYPE.PRELOAD:
						case STATION_TYPE.MANUAL_BOL:
							if (this.PromptForTractorOrTanker)
							{
								this.IssueTractorOrTankerPrompt();
							}
							else
							{
								this.ProcessOperatingMode(response);
							}
							break;
						case STATION_TYPE.LOAD_RACK:
							if (this.SiteManager.Site.PromptForTractorOrTanker)
							{
								this.IssueTractorOrTankerPrompt();
							}
							else if (this.SiteManager.Site.PromptForTruckCard)
							{
								// Prompt for Truck Card
								this.IssueTractorCardInPrompt();
							}
							else if (this.SiteManager.Site.PromptForShipmentNumber)
							{
								this.IssueLoadByShipmentOrLoadIDPrompt();
							}
							else
							{
								this.ProcessOperatingMode(response);
							}
							break;
						default:
							if (this.Station.Type == STATION_TYPE.LOAD_RACK)
							{
								if (this.TractorOrTanker != null
									 && this.TractorOrTanker.Type == EQUIPMENT_TYPE.TANKER_TYPE)
								{
									this.IssueEnterTrailer1Prompt();
								}
								else if (this.Transaction == null
											&& this.PromptForTractorOrTanker)
								{
									this.IssueTractorOrTankerPrompt();
								}
								else if (this.Transaction != null
											&& this.Transaction.Status == TransactionStatus.InProgress
											&& this.Transaction.DestinationEQ1.EquipmentType == EquipmentTypeClass.TypeID(EQUIPMENT_TYPE.TRACTOR_TYPE)
											&& this.PromptForTractorOrTanker)
								{
									this.IssueTractorOrTankerPrompt();
								}
								else if (this.SiteManager.Site.PromptForShipmentNumber)
								{
									if (this.Station.InhibitLoadingByLoadID)
									{
										this.IssueEnterShipmentNumberPrompt();
									}
									else
									{
										this.IssueLoadByShipmentOrLoadIDPrompt();
									}
								}
								else
								{
									this.ProcessOperatingMode(response);
								}
							}
							else if (this.Station.Type == STATION_TYPE.ENTRY_GATE)
							{
								if (this.PromptForTractorOrTanker)
								{
									this.IssueTractorOrTankerPrompt();
								}
								else
								{
									this.ProcessOperatingMode(response);
								}
							}
							break;
					}
				}
			}
			else if (response == "?")
			{
				if (!this.SiteManager.Site.ListEquipment)
				{
					this.StationState = StationState.NOT_AVAILABLE_TRAILER1_PROMPT;
					this.DisplayMessage("[LoadRack|List is not available]", null, 0, this.MESSAGE_TIMEOUT);
				}
				else
				{
					this.IssueSelectTrailer1Prompt();
				}
			}
			else
			{
				// Must Specify Trailer if TractorOrTanker isn't Tanker
				if (string.IsNullOrEmpty(response) == false
					 || this.TractorOrTanker == null || this.TractorOrTanker.Type != EQUIPMENT_TYPE.TANKER_TYPE)
				{
					Guid identityGuid;

					if (this.Station.EnableScully && !this.bScullyBypass && !this.bScullyFailMannualEnter)
					{
						identityGuid = FMChannelHelper.MakeCall<IEquipments, Guid>(
										x =>
										x.GetIdentityGuidByTruckCardNumber(this.Security, response)
							 );
					}
					else if ((this.Station.EnableScully && (this.bScullyBypass || this.bScullyFailMannualEnter)) ||
									this.SiteManager.Site.UseCompanyEquipmentIdentifiers)
					{
						identityGuid = FMChannelHelper.MakeCall<IEquipments, Guid>(
									x =>
									x.GetIdentityGuidByCompanyGuidAndEquipmentID(this.Security, this.Carrier.MasterRecordGuid, response)
							);
					}
					else
					{
						identityGuid = FMChannelHelper.MakeCall<IEquipments, Guid>(
																	 x =>
																	 x.GetIdentityGuid(this.Security, response)
																);
					}

					if (identityGuid != Guid.Empty)
					{
						this.Trailer1 = FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(
																					x =>
																					x.Get(this.Security, identityGuid)
																			 );

						if (this.Station.EnableScully && this.bScullyFailMannualEnter)
						{
							this.bScullyFailMannualEnter = false;
							if (this.Trailer1.ScullyRequired)
							{
								this.DisplayMessage("[LoadRack|Invalid], [LoadRack|Scully TIM Not Found]",
														  null, 0, this.MESSAGE_TIMEOUT * 3);

								this.AddAlarmAndEventLogs(this.Security, this.Station.TrailerMissingScullyIDEvent(this.Driver.FirstLastName, this.Carrier.ID));
								return;
							}

							if (this.TIN != this.Trailer1.TruckCardNumber)
							{
								this.DisplayMessage("[LoadRack|Invalid], [LoadRack|Scully TIM Not Match Truck Card Number]",
														  null, 0, this.MESSAGE_TIMEOUT * 3);
								this.AddAlarmAndEventLogs(this.Security, this.Station.ScullyTIMNotMatchTruckCardNumberEvent(this.Driver.FirstLastName, this.Carrier.ID));
							}

						}
					}

					// Invalid Trailer Number
					if (identityGuid == Guid.Empty || this.Trailer1 == null || this.Trailer1.Type != EQUIPMENT_TYPE.TRAILER_TYPE)
					{
						if (!this.bScullyBypass && this.Station.EnableScully && !this.bScullyFailMannualEnter)
						{
							this.AddAlarmAndEventLogs(this.Security, this.Station.InvalidTrailerIDEvent("TIM " + response + " Mismatched ", this.Driver.FirstLastName, this.Carrier.ID));
						}
						else
						{
							this.AddAlarmAndEventLogs(this.Security, this.Station.InvalidTrailerIDEvent(response, this.Driver.FirstLastName, this.Carrier.ID));
						}

						this.LoadRackManager.EventOrAlarmEvent.Set();
						this.ConsecutivePrompts++;
						if (this.ConsecutivePrompts >= this.SiteManager.Site._MaximumPrompts)
						{
							this.StationState = StationState.RESET_ON_TIMEOUT;
							this.DisplayMessage("LoadRack|Max Retries Exceeded", null, 0, this.MESSAGE_TIMEOUT);
							this.ConsecutivePrompts = 0;
							return;
						}

						this.StationState = StationState.ENTER_TRAILER1_PROMPT;

						if (!this.bScullyBypass && this.Station.EnableScully)
						{
							if (this.bScullyFailMannualEnter)
							{
								this.DisplayMessage("[LoadRack|Invalid], [LoadRack|Trailer]", null, 0, this.MESSAGE_TIMEOUT);
							}
							else
							{
								this.bScullyFailMannualEnter = true;
							}

							return;
						}

						if (this.SiteManager.Site.PromptForSecondTrailer)
						{
							this.DisplayMessage(
								"[LoadRack|Invalid], [LoadRack|Enter] [LoadRack|1st Trailer]", null, PromptLength, this.PROMPT_TIMEOUT);
						}
						else
						{
							this.DisplayMessage(
								"[LoadRack|Invalid], [LoadRack|Enter] [LoadRack|Trailer]", null, PromptLength, this.PROMPT_TIMEOUT);
						}
						return;
					}

					// check if this equipment can be used at this station
					if (this.IsEquipmentValidForThisStation(this.Trailer1) == false)
					{
						return;
					}

					//check if the station required equipment tags or licenses are attached
					if (this.IsEquipmentLicenseValidForThisStation(this.Trailer1) == false)
					{
						return;
					}

					// check quals and training
					if (this.CheckDriverEquipmentQualsAndTraining(this.Trailer1) == false)
					{
						return;
					}

					this.ConsecutivePrompts = 0;
					this.StationState = StationState.IDLE;

					if (this.SiteManager.Site.EnforceDriverEquipmentMatch && this.Trailer1.CompanyGuid != this.Carrier.MasterRecordGuid)
					{
						this.ConsecutivePrompts++;
						if (this.ConsecutivePrompts >= this.SiteManager.Site._MaximumPrompts)
						{
							this.StationState = StationState.RESET_ON_TIMEOUT;
							this.DisplayMessage("LoadRack|Max Retries Exceeded", null, 0, this.MESSAGE_TIMEOUT);
							this.ConsecutivePrompts = 0;
							return;
						}

						this.StationState = StationState.ENTER_TRAILER1_PROMPT;
						if (this.SiteManager.Site.PromptForSecondTrailer)
						{
							this.DisplayMessage(
								"[LoadRack|Mismatch], [LoadRack|Enter] [LoadRack|1st Trailer]", null, PromptLength, this.PROMPT_TIMEOUT);
						}
						else
						{
							this.DisplayMessage(
								"[LoadRack|Mismatch], [LoadRack|Enter] [LoadRack|Trailer]", null, PromptLength, this.PROMPT_TIMEOUT);
						}

						FMChannelHelper.MakeCall<IAlarmAndEventLogs>(x => x.Add(this.Security, this.Station.MismatchTrailerEvent(response, this.Driver.FirstLastName, this.Carrier.ID)));
						this.LoadRackManager.EventOrAlarmEvent.Set();
						return;
					}

					if (!this.CheckEquipmentOnOrder(this.Trailer1))
					{
						if (this.CheckMaximumRetries())
						{
							return;
						}

						this.StationState = StationState.ENTER_TRAILER1_PROMPT;
						if (this.PromptForSecondTrailer)
						{
							this.DisplayMessage("[LoadRack|Equipment Not On Order], [LoadRack|Enter] [LoadRack|1st Trailer]", null, PromptLength, this.PROMPT_TIMEOUT);
						}
						else
						{
							this.DisplayMessage("[LoadRack|Equipment Not On Order], [LoadRack|Enter] [LoadRack|Trailer]", null, PromptLength, this.PROMPT_TIMEOUT);
						}

						return;
					}

					if (!this.CheckTransactionCanAccomodateEquipment(this.Trailer1))
					{
						if (this.CheckMaximumRetries())
						{
							return;
						}

						this.StationState = StationState.ENTER_TRAILER1_PROMPT;
						if (this.PromptForSecondTrailer)
						{
							this.DisplayMessage("[LoadRack|Maximum Equipment Exceeded], [LoadRack|Enter] [LoadRack|1st Trailer]", null, PromptLength, this.PROMPT_TIMEOUT);
						}
						else
						{
							this.DisplayMessage("[LoadRack|Max Equipment], [LoadRack|Enter] [LoadRack|Trailer]", null, PromptLength, this.PROMPT_TIMEOUT);
						}

						return;
					}

					if (!this.CheckCompartmentsAvailable(this.Trailer1))
					{
						if (this.CheckMaximumRetries())
						{
							return;
						}

						this.StationState = StationState.ENTER_TRAILER1_PROMPT;
						if (this.PromptForSecondTrailer)
						{
							this.DisplayMessage("[LoadRack|No Compartments Available], [LoadRack|Enter] [LoadRack|1st Trailer]", null, PromptLength, this.PROMPT_TIMEOUT);
						}
						else
						{
							this.DisplayMessage("[LoadRack|No Compartments Available], [LoadRack|Enter] [LoadRack|Trailer]", null, PromptLength, this.PROMPT_TIMEOUT);
						}

						return;
					}
				}

				this.ConsecutivePrompts = 0;
				this.StationState = StationState.IDLE;
				this.CompleteTrailer1Processing(false);
			}
		}

		protected void ProcessTrailer2ID(string response)
		{
			if (string.IsNullOrEmpty(response))
			{
				return;
			}

			if (response == EscapeString)
			{
				if (this.StationState == StationState.SELECT_TRAILER2_PROMPT)
				{
					this.IssueTrailer2Prompt();
				}
				else
				{
					this.IssueEnterTrailer2Prompt();
				}
			}
			else if (response == "?")
			{
				if (!this.SiteManager.Site.ListEquipment)
				{
					this.StationState = StationState.NOT_AVAILABLE_TRAILER2_PROMPT;
					this.DisplayMessage("[LoadRack|List is not available]", null, 0, this.MESSAGE_TIMEOUT);
				}
				else
				{
					this.IssueSelectTrailer2Prompt();
				}
			}

			else
			{
				Guid identityGuid;
				if (this.SiteManager.Site.UseCompanyEquipmentIdentifiers)
				{
					identityGuid =
						FMChannelHelper.MakeCall<IEquipments, Guid>(
							x => x.GetIdentityGuidByCompanyGuidAndEquipmentID(this.Security, this.Carrier.MasterRecordGuid, response));
				}
				else
				{
					identityGuid = FMChannelHelper.MakeCall<IEquipments, Guid>(
																	x =>
																	x.GetIdentityGuid(this.Security, response)
															);
				}

				if (identityGuid != Guid.Empty)
				{
					this.Trailer2 = FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(
																	x =>
																	x.Get(this.Security, identityGuid)
															);
				}

				// Invalid Trailer Number
				if (identityGuid == Guid.Empty || this.Trailer2 == null || this.Trailer2.Type != EQUIPMENT_TYPE.TRAILER_TYPE
						|| this.Trailer1.IdentityGuid == this.Trailer2.IdentityGuid)
				{
					this.ConsecutivePrompts++;
					if (this.ConsecutivePrompts >= this.SiteManager.Site._MaximumPrompts)
					{
						this.StationState = StationState.RESET_ON_TIMEOUT;
						this.DisplayMessage("LoadRack|Max Retries Exceeded", null, 0, this.MESSAGE_TIMEOUT);
						this.ConsecutivePrompts = 0;
						return;
					}

					this.StationState = StationState.ENTER_TRAILER2_PROMPT;
					this.DisplayMessage(
						"[LoadRack|Invalid], [LoadRack|Enter] [LoadRack|2nd Trailer]", null, PromptLength, this.PROMPT_TIMEOUT);
					return;
				}

				// check if this equipment can be used at this station
				if (this.IsEquipmentValidForThisStation(this.Trailer2) == false)
				{
					return;
				}

				//check if the station required equipment tags or licenses are attached
				if (this.IsEquipmentLicenseValidForThisStation(this.Trailer2) == false)
				{
					return;
				}

				// check quals and training
				if (this.CheckDriverEquipmentQualsAndTraining(this.Trailer2) == false)
				{
					return;
				}

				this.ConsecutivePrompts = 0;
				this.StationState = StationState.IDLE;

				if (this.SiteManager.Site.EnforceDriverEquipmentMatch && this.Trailer2.CompanyGuid != this.Carrier.MasterRecordGuid)
				{
					this.ConsecutivePrompts++;
					if (this.ConsecutivePrompts >= this.SiteManager.Site._MaximumPrompts)
					{
						this.StationState = StationState.RESET_ON_TIMEOUT;
						this.DisplayMessage("LoadRack|Max Retries Exceeded", null, 0, this.MESSAGE_TIMEOUT);
						this.ConsecutivePrompts = 0;
						return;
					}

					FMChannelHelper.MakeCall<IAlarmAndEventLogs>(x => x.Add(this.Security, this.Station.MismatchTrailerEvent(response, this.Driver.FirstLastName, this.Carrier.ID)));
					this.LoadRackManager.EventOrAlarmEvent.Set();
					this.StationState = StationState.ENTER_TRAILER2_PROMPT;
					this.DisplayMessage(
						"[LoadRack|Mismatch], [LoadRack|Enter] [LoadRack|2nd Trailer]", null, PromptLength, this.PROMPT_TIMEOUT);
					return;
				}

				if (!this.CheckEquipmentOnOrder(this.Trailer2))
				{
					if (this.CheckMaximumRetries())
					{
						return;
					}

					this.StationState = StationState.ENTER_TRAILER2_PROMPT;
					this.DisplayMessage("[LoadRack|Equipment Not On Order], [LoadRack|Enter] [LoadRack|2nd Trailer]", null, PromptLength, this.PROMPT_TIMEOUT);
					return;
				}

				if (!this.CheckTransactionCanAccomodateEquipment(this.Trailer2))
				{
					if (this.CheckMaximumRetries())
					{
						return;
					}

					this.StationState = StationState.ENTER_TRAILER2_PROMPT;
					this.DisplayMessage("[LoadRack|Maximum Equipment Exceeded], [LoadRack|Enter] [LoadRack|2nd Trailer]", null, PromptLength, this.PROMPT_TIMEOUT);
					return;
				}

				if (!this.CheckCompartmentsAvailable(this.Trailer2))
				{
					if (this.CheckMaximumRetries())
					{
						return;
					}

					this.StationState = StationState.ENTER_TRAILER2_PROMPT;
					this.DisplayMessage("[LoadRack|No Compartments Available], [LoadRack|Enter] [LoadRack|2nd Trailer]", null, PromptLength, this.PROMPT_TIMEOUT);
					return;
				}

				this.CompleteTrailer2Processing(false);
			}
		}

		protected void ProcessTrailer3ID(string response)
		{
			if (string.IsNullOrEmpty(response))
			{
				return;
			}

			if (response == EscapeString)
			{
				if (this.StationState == StationState.SELECT_TRAILER3_PROMPT)
				{
					this.IssueTrailer3Prompt();
				}
				else
				{
					this.IssueEnterTrailer3Prompt();
				}
			}
			else if (response == "?")
			{
				if (!this.SiteManager.Site.ListEquipment)
				{
					this.StationState = StationState.NOT_AVAILABLE_TRAILER3_PROMPT;
					this.DisplayMessage("[LoadRack|List is not available]", null, 0, this.MESSAGE_TIMEOUT);
				}
				else
				{
					this.IssueSelectTrailer3Prompt();
				}
			}
			else
			{
				Guid identityGuid = Guid.Empty;

				if (!string.IsNullOrEmpty(response))
				{
					if (this.SiteManager.Site.UseCompanyEquipmentIdentifiers)
					{
						identityGuid = FMChannelHelper.MakeCall<IEquipments, Guid>(x => x.GetIdentityGuidByCompanyGuidAndEquipmentID(this.Security, this.Carrier.IdentityGuid, response));
					}
					else
					{
						identityGuid = FMChannelHelper.MakeCall<IEquipments, Guid>(x => x.GetIdentityGuid(this.Security, response));
					}

					if (identityGuid != Guid.Empty)
					{
						this.Trailer3 = FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(x => x.Get(this.Security, identityGuid));
					}
				}

				// Invalid Trailer Number
				if (identityGuid == Guid.Empty
					 || this.Trailer3 == null
					 || this.Trailer3.Type != EQUIPMENT_TYPE.TRAILER_TYPE)
				{
					if (this.CheckMaximumRetries())
					{
						return;
					}

					this.StationState = StationState.ENTER_TRAILER3_PROMPT;
					this.DisplayMessage("[LoadRack|Invalid], [LoadRack|Enter] [LoadRack|3rd Trailer]", null, PromptLength, this.PROMPT_TIMEOUT);
					return;
				}

				if (this.Trailer3.IdentityGuid == this.Trailer1.IdentityGuid
				|| this.Trailer3.IdentityGuid == this.Trailer2.IdentityGuid)
				{
					this.DisplayMessage("[LoadRack|Duplicate Trailer], [LoadRack|Enter] [LoadRack|3rd Trailer]", null, PromptLength, this.PROMPT_TIMEOUT);
					return;
				}

				// check if this equipment can be used at this station
				if (this.IsEquipmentValidForThisStation(this.Trailer3) == false)
				{
					return;
				}

				//check if the station required equipment tags or licenses are attached
				if (this.IsEquipmentLicenseValidForThisStation(this.Trailer3) == false)
				{
					return;
				}
				// check the qualifications and training required for this peice of equipment
				if (this.CheckDriverEquipmentQualsAndTraining(this.Trailer3) == false)
				{
					return;
				}

				if (this.SiteManager.Site.EnforceDriverEquipmentMatch
				&& this.Trailer3.CompanyGuid != this.Carrier.MasterRecordGuid)
				{
					if (this.CheckMaximumRetries())
					{
						return;
					}

					this.StationState = StationState.ENTER_TRAILER3_PROMPT;
					this.DisplayMessage("[LoadRack|Mismatch], [LoadRack|Enter] [LoadRack|3rd Trailer]", null, PromptLength, this.PROMPT_TIMEOUT);

					FMChannelHelper.MakeCall<IAlarmAndEventLogs>(x => x.Add(this.Security, this.Station.MismatchTrailerEvent(response, this.Driver.FirstLastName, this.Carrier.ID)));
					this.LoadRackManager.EventOrAlarmEvent.Set();
					return;
				}

				if (!this.CheckEquipmentOnOrder(this.Trailer3))
				{
					if (this.CheckMaximumRetries())
					{
						return;
					}

					this.StationState = StationState.ENTER_TRAILER3_PROMPT;
					this.DisplayMessage("[LoadRack|Equipment Not On Order], [LoadRack|Enter] [LoadRack|3rd Trailer]", null, PromptLength, this.PROMPT_TIMEOUT);
					return;
				}

				if (!this.CheckTransactionCanAccomodateEquipment(this.Trailer3))
				{
					if (this.CheckMaximumRetries())
					{
						return;
					}

					this.StationState = StationState.ENTER_TRAILER3_PROMPT;
					this.DisplayMessage("[LoadRack|Maximum Equipment Exceeded], [LoadRack|Enter] [LoadRack|3rd Trailer]", null, PromptLength, this.PROMPT_TIMEOUT);
					return;
				}

				if (!this.CheckCompartmentsAvailable(this.Trailer3))
				{
					if (this.CheckMaximumRetries())
					{
						return;
					}

					this.StationState = StationState.ENTER_TRAILER3_PROMPT;
					this.DisplayMessage("[LoadRack|No Compartments Available], [LoadRack|Enter] [LoadRack|3rd Trailer]", null, PromptLength, this.PROMPT_TIMEOUT);
					return;
				}

				this.ConsecutivePrompts = 0;
				this.StationState = StationState.IDLE;
				this.CompleteTrailer3Processing(false);
			}
		}

		protected virtual void ProcessUnloadAmount(string response)
		{
			if (response == EscapeString)
			{
				if (this.Station.PromptForGravity)
				{
					this.PromptForOffLoadDensity();
				}
				else
				{
					if (this.Station.PromptForBOLNumber)
					{
						this.PromptForBOLNumber();
					}
					else if (this.Station.OffLoadByOffLoadID || this.UseOffLoadSupplyOrders == false)
					{
						this.DisplayOffLoadProductSelect();
					}
					else
					{
						this.DisplayVerifySupplyOrderProduct();
					}
				}
				return;
			}
			else
			{
				this.SetUnloadPresetAmount(response);
			}
		}

		protected virtual void ProcessUnloadDensity(string response)
		{
			if (response == EscapeString)
			{
				if (this.Station.Type == STATION_TYPE.WEIGHT_SCALE && this.ByWeight)
				{
					this.IssueSelectOffloadProductPrompt();
					return;
				}

				if (this.Station.PromptForBOLNumber)
				{
					this.PromptForBOLNumber();
				}
				else if (this.Station.OffLoadByOffLoadID || this.UseOffLoadSupplyOrders == false)
				{
					this.DisplayOffLoadProductSelect();
				}
				else
				{
					this.DisplayVerifySupplyOrderProduct();
				}

				return;
			}

			if (response == string.Empty)
			{
				this.PromptForOffLoadDensity();
				return;
			}

			if (this.SetDensityInUnit(response) == false)
			{
				FMChannelHelper.MakeCall<IAlarmAndEventLogs>(x => x.Add(this.Security, this.Station.FailedToSetDensityEvent(this.Station.ID)));
				this.LoadRackManager.EventOrAlarmEvent.Set();

				this.StationState = StationState.RESET_ON_TIMEOUT;
				this.DisplayMessage("[LoadRack|Failed To Set Density]", null, 0, this.MESSAGE_TIMEOUT);
				this.ConsecutivePrompts = 0;
				return;
			}

			try
			{
				this.OffloadDensity = System.Convert.ToDouble(response);
			}
			catch (FormatException)
			{
				this.PromptForOffLoadDensity();
				return;
			}
			catch (OverflowException)
			{
				this.PromptForOffLoadDensity();
				return;
			}

			if (this.OffloadDensity < 0.0)
			{
				this.PromptForOffLoadDensity();
				return;
			}

			this.PromptForOffLoadTemperature();
			return;
		}

		protected virtual void ProcessUnloadTemperature(string response)
		{
			if (response == EscapeString)
			{
				if (this.Station.Type == STATION_TYPE.WEIGHT_SCALE && this.ByWeight)
				{
					this.IssueSelectOffloadProductPrompt();
					return;
				}

				if (this.Station.PromptForBOLNumber)
				{
					this.PromptForBOLNumber();
				}
				else if (this.Station.OffLoadByOffLoadID || this.UseOffLoadSupplyOrders == false)
				{
					this.DisplayOffLoadProductSelect();
				}
				else
				{
					this.DisplayVerifySupplyOrderProduct();
				}

				return;
			}

			if (response == string.Empty)
			{
				this.PromptForOffLoadDensity();
				return;
			}

			try
			{
				this.OffloadTemperature = System.Convert.ToDouble(response);
			}
			catch (FormatException)
			{
				this.PromptForOffLoadTemperature();
				return;
			}
			catch (OverflowException)
			{
				this.PromptForOffLoadTemperature();
				return;
			}

			PromptForOffLoadAmount();
		}

		protected void ProcessUseOrder(string Response)
		{
			if (Response == EscapeString)
			{
				if (this.Station.Type == STATION_TYPE.WEIGHT_SCALE && this.PendingTransactions.Count == 0)
				{
					this.IssueCaptureTareWeightPrompt();
				}

				else if (this.Station.Type == STATION_TYPE.PRELOAD && this.PendingTransactions.Count == 0)
				{
					this.ProcessTareWeight(Response);
				}

				else
				{
					if (this.Driver.PINRequired || this.Carrier.PINRequired)
					{
						this.IssuePromptForPIN();
					}

					else
					{
						if (this.Station.CardReader)
						{
							this.IssuePleaseCardIn();
						}
						else if (this.Station.TouchKeyReader)
						{
							this.IssueTouchKeyPleaseCardIn();
						}
						else
						{
							this.IssueDriverIDPrompt();
						}
					}
				}
			}
			else
			{
				if (this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, "LoadRack|Yes") == Response)
				{
					if (this.SiteManager.Site.PromptForShipmentNumber)
					{
						this.IssueEnterShipmentNumberPrompt();
					}
					else
					{
						this.IssueEnterOrderNumberPrompt();
					}
				}

				else if (this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, "LoadRack|No") == Response)
				{
					this.IssueLoadIDPrompt();
				}

				else
				{
					this.StationState = StationState.IDLE;
					this.DisplayMessage("Unknown Use Order Response", null, 0, 2);
				}
			}
		}

		protected void ProcessUseSupplyOrder(string response)
		{
			if (response == "1")
			{
				response =
					FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
						x => x.Get(this.SiteManager.Site.SiteGuid, this.CurrentMenuParameters.Menu[0]));
			}

			if (response == "2")
			{
				response =
					FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
						x => x.Get(this.SiteManager.Site.SiteGuid, this.CurrentMenuParameters.Menu[1]));
			}

			if (response == EscapeString)
			{
				if (this.Driver.PINRequired || this.Carrier.PINRequired)
				{
					this.IssuePromptForPIN();
				}

				else
				{
					if (this.Station.CardReader)
					{
						this.IssuePleaseCardIn();
					}
					else if (this.Station.TouchKeyReader)
					{
						this.IssueTouchKeyPleaseCardIn();
					}
					else
					{
						this.IssueDriverIDPrompt();
					}
				}
			}
			else
			{
				if (
					FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
						x => x.Get(this.SiteManager.Site.SiteGuid, "LoadRack|Yes")) == response)
				{
					this.UseOffLoadSupplyOrders = true;
					this.PromptForSupplyOrderNumber();
				}

				else if (
					FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
						x => x.Get(this.SiteManager.Site.SiteGuid, "LoadRack|No")) == response)
				{
					this.IssueOffLoadIDPrompt();
					this.UseOffLoadSupplyOrders = false;
				}

				else
				{
					this.StationState = StationState.IDLE;
					this.DisplayMessage("Unknown Use Supply Order Response", null, 0, 2);
				}
			}
		}

		protected void ProductAvailabilityCompletion()
		{
			if (this.Station.Type == STATION_TYPE.LOAD_RACK)
			{
				if (this.Transaction == null || this.Transaction.DeleteFlag)
				{
					this.CurrentTransactionAlias = FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasClass>(
																	 x =>
																	 x.Get(this.Security, this.Station.IssueByVolumeTransactionAliasGuid, false)
																);
					this.CompartmentList = null;
					this.GenerateCompartmentList();
					this.StationState = StationState.AUTHORIZING;
					this.BuildRecipeMapForAllLoadArms(false);
				}

				else
				{
					this.CurrentTransactionAlias = FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasClass>(
																	 x =>
																	 x.Get(this.Security, this.Transaction.TransactionAliasGuid, false)
																);
					this.BuildPlan(false);
				}
			}

			else if (this.Station.Type == STATION_TYPE.WEIGHT_SCALE || this.Station.Type == STATION_TYPE.PRELOAD)
			{
				this.CurrentEquipment = null;

				if (this.AvailableCompartments(this.TractorOrTanker) > 0)
				{
					this.CurrentEquipment = this.TractorOrTanker;
				}
				else if (this.AvailableCompartments(this.Trailer1) > 0)
				{
					this.CurrentEquipment = this.Trailer1;
				}
				else if (this.AvailableCompartments(this.Trailer2) > 0)
				{
					this.CurrentEquipment = this.Trailer2;
				}

				if (this.CurrentEquipment == null)
				{
					this.StationState = StationState.RESET_ON_TIMEOUT;
					this.DisplayMessage("LoadRack|No Compartments Available", null, 0, this.MESSAGE_TIMEOUT, false);
				}
				else
				{
					this.InitializeTransaction();
					this.IssueCompartmentSummaryPrompt();
				}
			}
		}

		protected virtual void PromptForOffLoadTemperature()
		{
			if (this.Station.PromptForTemperature)
			{
				this.StationState = StationState.ENTER_UNLOAD_TEMPERATURE;
				this.DisplayMessage("[LoadRack|Enter] [LoadRack|Temperature]", null, 10, this.PROMPT_TIMEOUT);
			}
			else
			{
				this.PromptForOffLoadAmount();
			}

			return;
		}

		protected void PromptForBOLNumber()
		{
			if (this.Station.PromptForBOLNumber)
			{
				this.StationState = StationState.ENTER_BOL_NUMBER;
				this.DisplayMessage("[LoadRack|Enter] [LoadRack|BOL Number]", null, 10, this.PROMPT_TIMEOUT);
			}
			else
			{
				PromptForOffLoadDensity();
			}

			return;
		}

		protected virtual void PromptForOffLoadDensity()
		{
			if (this.Station.PromptForGravity)
			{
				this.StationState = StationState.ENTER_UNLOAD_DENSITY;
				this.DisplayMessage("[LoadRack|Enter] [LoadRack|Density]", null, 10, this.PROMPT_TIMEOUT);
			}
			else
			{
				PromptForOffLoadTemperature();
			}

			return;
		}

		protected virtual void PromptForOffLoadAmount()
		{
			this.StationState = StationState.ENTER_UNLOAD_AMOUNT;
			this.DisplayMessage("[LoadRack|Enter] [LoadRack|Qty On BOL]", null, 10, this.PROMPT_TIMEOUT);
		}

		protected virtual bool PromptForPreLoadSelection()
		{
			DateTimeOffset today = TimeConverter.Today(this.SiteManager.Site);

			try
			{
				this.PreloadSelectMethod = PRELOAD_SELECT_METHOD.DOCUMENT;
				this.PreloadDataSet = null;

				// Check for preloads for the current driver
				GetTransactionSR getTransactionSR = new GetTransactionSR
				{
					Security = this.Security,
					Request = GetTransactionRequest.SITE_TYPEID_TRANSDATE_STATUS_OPERATORPERSONNELGUID_LINEITEMSTATUS,
					Site = this.SiteManager.Site.ID,
					TransTypeID = TransactionTypes.T5_PrimaryDisbursement,
					BeginningDate = today.AddDays(-1.0),
					EndingDate = today.AddDays(1.0),
					OperatorPersonnelGuid = this.Driver.MasterRecordGuid,
					Status = ((int)TransactionStatus.LoadPending).ToString(),
					LineItemStatus = ((int)TransactionStatus.LoadPending).ToString()
				};


				GetTransactionDO getTransactionDO = FMChannelHelper.MakeCall<IGetTransactionProcessor, GetTransactionDO>(
																	x =>
																	x.Process(getTransactionSR)
															  );
				GetTransactionDO weighOutDO = this.GetAppropriateWeighOutPreloads();
				if (weighOutDO != null)
				{
					getTransactionDO.TransactionDataSet.Merge(weighOutDO.TransactionDataSet);
				}

				if (getTransactionDO?.TransactionDataSet != null && getTransactionDO.TransactionDataSet.Tables.Count != 0 && getTransactionDO.TransactionDataSet.Tables[0].Rows.Count != 0)
				{
					// Save the set for later
					this.PreloadDataSet = getTransactionDO.TransactionDataSet;

					if (getTransactionDO.TransactionDataSet.Tables[0].Rows.Count == 1)
					{
						return this.IssueSelectPreloadDocument();
					}
					else
					{
						bool allPreloadsAssociatedWithOrders = this.GetOrderList();
						bool allPreloadsAssociatedWithLoadIDs = this.GetLoadIDList();

						// If there is a mix, ask the user how they want to select the preload.
						if (this.OrderList.Count > 0 && this.LoadIDList.Count > 0)
						{
							this.IssueSelectPreloadBy();
						}
						else if (allPreloadsAssociatedWithOrders && this.OrderList.Count > 1)
						{
							// If all the preloads have Order Numbers, we should prompt by Order Number
							this.IssueSelectPreloadOrder();
						}
						else if (allPreloadsAssociatedWithLoadIDs && this.LoadIDList.Count > 1)
						{
							// If all the preloads have LoadIDs, we should prompt by LoadID.
							this.IssueSelectPreloadLoadID();
						}
						else
						{
							// Issue select by Docuement
							return this.IssueSelectPreloadDocument();
						}
					}

					return true;
				}
			}
			catch
			{
				return false;
			}

			return false;
		}

		protected virtual void PromptForSupplyOrderNumber()
		{
			this.ConsecutivePrompts = 0;
			this.StationState = StationState.ENTER_SUPPLY_ORDER_NUMBER;
			this.DisplayMessage("[LoadRack|Enter] [LoadRack|Supply Order Number]", null, PromptLength, this.PROMPT_TIMEOUT);
		}

		protected void RollUpSplashBlendTotals(LineItemDO lineItem)
		{
			if (lineItem.Quantity != null)
			{
				lineItem.Quantity.GrossInventoryChange = 0;
				lineItem.Quantity.NetInventoryChange = 0;
				lineItem.Quantity.MassInventoryChange = 0;
				lineItem.Quantity.PackageInventoryChange = 0;

				string componentProduct = ProductClass.ProductTypeID(ProductType.ComponentProduct);
				SIDouble lineItemGross = new SIDouble(lineItem.VolumeUnits, null, 0);
				SIDouble lineItemNet = new SIDouble(lineItem.VolumeUnits, null, 0);
				SIDouble lineItemMass = new SIDouble(lineItem.MassUnits, null, 0);
				lineItemGross.Units = lineItem.VolumeUnits;
				lineItemNet.Units = lineItem.VolumeUnits;
				lineItemMass.Units = lineItem.MassUnits;

				foreach (SubLineItemDO subLineItem in lineItem.SubLineItems)
				{
					if (subLineItem.ProductType != componentProduct)
					{
						continue;
					}

					if (subLineItem.VolumeUnits != lineItem.VolumeUnits)
					{
						SIDouble subLineItemVolume = new SIDouble(subLineItem.VolumeUnits, null, 0) { Units = subLineItem.VolumeUnits };

						SIDouble subLineItemMass = new SIDouble(subLineItem.MassUnits, null, 0) { Units = subLineItem.MassUnits };

						subLineItemVolume.Value = subLineItem.Quantity.GrossInventoryChange;
						lineItemGross.SIValue += subLineItemVolume.SIValue;

						subLineItemVolume.Value = subLineItem.Quantity.NetInventoryChange;
						lineItemNet.SIValue += subLineItemVolume.SIValue;

						subLineItemMass.Value = subLineItem.Quantity.MassInventoryChange;
						lineItemMass.SIValue += subLineItemMass.SIValue;
					}
					else
					{
						lineItemGross.Value += subLineItem.Quantity.GrossInventoryChange;
						lineItemNet.Value += subLineItem.Quantity.NetInventoryChange;
						lineItemMass.Value += subLineItem.Quantity.MassInventoryChange;
					}
				}
				lineItem.Quantity.GrossInventoryChange += lineItemGross.Value;
				lineItem.Quantity.NetInventoryChange += lineItemNet.Value;
				lineItem.Quantity.MassInventoryChange += lineItemMass.Value;

				if (lineItem.Quantity.NetInventoryChange != 0.0)
				{
					EngineeringUnit units = (this.CurrentTransactionAlias.DensityUnits != 0)
														 ? this.CurrentTransactionAlias.DensityUnits
														 : this.SiteManager.Site.DensityUnits;

					SIDouble lineItemDensity = new SIDouble { Units = units };
					SIDouble subLineItemDensity = new SIDouble { Units = units };

					if (lineItem.Temperature == null)
					{
						lineItem.Temperature = 0.0;
					}

					lineItem.Temperature = 0.0;

					foreach (SubLineItemDO subLineItem in lineItem.SubLineItems)
					{
						if (subLineItem.ProductType != componentProduct)
						{
							continue;
						}

						if (!subLineItem.Density_BadQualityLogged && subLineItem.Density.HasValue && subLineItem.Density.Value != 0
							 && lineItem.Quantity.NetInventoryChange != 0 && subLineItem.Quantity.NetInventoryChange != 0)
						{
							subLineItemDensity.Value = subLineItem.Density.Value;
							lineItemDensity.SIValue += subLineItemDensity.SIValue * subLineItem.Quantity.NetInventoryChange
																/ lineItem.Quantity.NetInventoryChange;
							if (subLineItem.Temperature.HasValue)
							{
								lineItem.Temperature += subLineItem.Temperature.Value * subLineItem.Quantity.NetInventoryChange
												 / lineItem.Quantity.NetInventoryChange;
							}
						}
					}

					if (lineItemDensity.Value != 0)
					{
						if (lineItem.Density == null)
						{
							lineItem.Density = 0.0;
						}

						lineItem.Density = lineItemDensity.Value;
					}
				}
			}
		}

		protected void SetPIDXAuthorizations()
		{
			if (this.PIDXProfileCompanyMapCollection == null || this.PIDXProfileCompanyMapCollection.Count == 0
				 || this.PIDXAuthorizationArray == null
				 || this.PIDXAuthorizationArray.Length != this.PIDXProfileCompanyMapCollection.Count)
			{
				return;
			}

			this.Transaction.TransPIDXCollection = new List<TransactionPIDXDO>();

			PIDXProfileCollectionClass PIDXProfileCollection = FMChannelHelper.MakeCall<IPIDXProfiles, PIDXProfileCollectionClass>(
																	 x =>
																	 x.Enumerate(this.Security)
																);
			int AuthorizationIndex = -1;

			foreach (PIDXProfileCompanyMapClass PIDXProfileCompanyMap in this.PIDXProfileCompanyMapCollection)
			{
				AuthorizationIndex++;

				PIDXProfileClass PIDXProfile = PIDXProfileCollection.Find(PIDXProfileCompanyMap.PIDXProfileGuid);
				if (PIDXProfile == null || !PIDXProfile.Enabled)
				{
					continue;
				}

				TransactionPIDXDO TransactionPIDXDO = new TransactionPIDXDO
				{
					PIDXProfileGuid = PIDXProfileCompanyMap.PIDXProfileGuid,
					CompanyPersonnelToShipToBillToGuid = PIDXProfileCompanyMap.CompanyPersonnelToShipToBillToGuid
				};

				if (this.PIDXAuthorizationArray[AuthorizationIndex] is AuthorizationGrantedBase)
				{
					AuthorizationGrantedBase Authorization = this.PIDXAuthorizationArray[AuthorizationIndex] as AuthorizationGrantedBase;
					TransactionPIDXDO.AuthorizationNumber = Authorization.AuthorizationNumber;
				}

				TransactionPIDXDO.BOLVersion = (int)PIDXProfile.Version;
				this.Transaction.TransPIDXCollection.Add(TransactionPIDXDO);
			}
		}

		protected virtual void SetProductsInStation()
		{
			return;
		}

		protected bool SplashBlendComponentNeedsFilling(LineItemDO LineItem, ProductClass ComponentProduct)
		{
			// Check each sub line item to see if one already exists for this component and if it has been fulfilled
			foreach (SubLineItemDO SubLineItem in LineItem.SubLineItems)
			{
				// Does a sub line item exist for our component product?
				if (SubLineItem.ProductGuid != Guid.Empty && SubLineItem.ProductGuid == ComponentProduct.IdentityGuid)
				{
					return (SubLineItem.Status == TransactionStatus.Completed) ? false : true;
				}
			}

			return true;
		}

		protected virtual void StartPreloadBatches()
		{
			this.StationState = StationState.AUTHORIZING;
			foreach (LoadArmManagerClass LoadArmManager in this.LoadArmManagerCollection)
			{
				if (LoadArmManager.IsInAlarm)
				{
					continue;
				}

				if (this != LoadArmManager.GetStationManager())
				{
					continue;
				}

				if (!LoadArmManager.ShowNoProductsMessage())
				{
					LoadArmManager.EnablePreset(this, false);
				}
			}

			this.StationState = StationState.AUTHORIZED;

			foreach (LineItemDO LineItem in this.Transaction.LineItems)
			{
				if (LineItem.Status != TransactionStatus.LoadPending)
				{
					continue;
				}

				foreach (LoadArmManagerClass LoadArmManager in this.LoadArmManagerCollection)
				{
					if (this != LoadArmManager.GetStationManager())
					{
						continue;
					}

					if (LoadArmManager.IsInAlarm)
					{
						continue;
					}

					foreach (LineItemDO Preload in LoadArmManager.Bay(this).PreLoads)
					{
						if (Preload == LineItem)
						{
							LoadArmManager.SetFocus();
							return;
						}
					}
				}
			}
		}

		protected bool ValidateCompanyHierarchyLoadRack(CompanyMapClass shipToBillToMap)
		{
			Stopwatch stopwatch = new Stopwatch();
			long interval;
			DateTimeOffset siteTimeNow = TimeConverter.Now(this.SiteManager.Site);
			CompanyMapCollectionClass companyMapClassCollection = null;

			// if we already have the objects just validate them
			if (this.Manager == null ||
			this.Owner == null ||
			this.Shipper == null ||
			this.BillTo == null ||
			this.ShipTo == null ||
			this.Carrier == null)
			{
				// get the company map classes
				// order return is billToShipperMap,shipperOwnerMap,ownerManagerMap
				//var stopwatch = new Stopwatch();
				stopwatch.Start();
				companyMapClassCollection = FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapCollectionClass>(
														x =>
														x.GetLoadRackCompanyMapClasses(this.Security, shipToBillToMap.AssignedToGuid)
														);
				stopwatch.Stop();
				if (companyMapClassCollection.Count != 3)
				{
					return false;
				}

				// get the companyclass
				CompanyCollectionClass companyCollectionClass = null;
				stopwatch.Reset();
				stopwatch.Start();
				// returned order shipto,billto,shipper,owner,manager
				stopwatch.Start();
				companyCollectionClass = FMChannelHelper.MakeCall<ICompanies, CompanyCollectionClass>(
															 x =>
															 x.GetLoadRackCompanyClasses(this.Security, companyMapClassCollection, shipToBillToMap.AssignedGuid, siteTimeNow, false)
														);
				interval = stopwatch.ElapsedMilliseconds;

				stopwatch.Stop();
				if (companyCollectionClass.Count != 5)
				{
					return false;
				}

				this.ShipTo = companyCollectionClass[0];
				this.BillTo = companyCollectionClass[1];
				this.Shipper = companyCollectionClass[2];
				this.Owner = companyCollectionClass[3];
				this.Manager = companyCollectionClass[4];

				stopwatch.Restart();
			}
			else
			{
				companyMapClassCollection = FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapCollectionClass>(
														x =>
														x.GetLoadRackCompanyMapClasses(this.Security, shipToBillToMap.AssignedToGuid)
														);
				if (companyMapClassCollection.Count != 3)
				{
					return false;
				}
			}

			if (!this.ValidateCompany(this.ShipTo, COMPANY_ROLE.CUSTOMER_SHIPTO))
			{
				return false;
			}

			if (!this.ValidateCompany(this.BillTo, COMPANY_ROLE.CUSTOMER_BILLTO))
			{
				return false;
			}

			if (!this.ValidateCompany(this.Shipper, COMPANY_ROLE.SHIPPER))
			{
				return false;
			}

			if (!this.ValidateCompany(this.Owner, COMPANY_ROLE.OWNER))
			{
				return false;
			}

			if (!this.ValidateCompany(this.Manager, COMPANY_ROLE.MANAGER))
			{
				return false;
			}
			interval = stopwatch.ElapsedMilliseconds;

			// need to review the following
			Guid shipToAllocationGuid = FMChannelHelper.MakeCall<IAllocations, Guid>(
																	 x =>
																	 x.GetIdentityGuid(
																		this.Security,
																		shipToBillToMap.IdentityGuid,
																		DateTimeOffset.Now,
																		DateTimeOffset.Now,
																		COMPANY_MAP_TYPE.SHIPTO_BILLTO_MAP)
												);

			if (shipToAllocationGuid != Guid.Empty)
			{
				this.AllocationArray[0] = FMChannelHelper.MakeCall<IAllocations, AllocationClass>(
						x =>
						x.GetBySiteGuid(this.Security, shipToAllocationGuid, this.SiteManager.Site.IdentityGuid, this.Station.Type, "")
				);

			}
			else
			{
				this.AllocationArray[0] = null;
			}

			Guid billToAllocationGuid = FMChannelHelper.MakeCall<IAllocations, Guid>(
																	 x =>
																	 x.GetIdentityGuid(
																		this.Security,
																		companyMapClassCollection[0].IdentityGuid,
																		DateTimeOffset.Now,
																		DateTimeOffset.Now,
																		COMPANY_MAP_TYPE.BILLTO_SHIPPER_MAP)
																	);

			if (billToAllocationGuid != Guid.Empty)
			{
				this.AllocationArray[1] = FMChannelHelper.MakeCall<IAllocations, AllocationClass>(
						x =>
						x.GetBySiteGuid(this.Security, billToAllocationGuid, this.SiteManager.Site.IdentityGuid, this.Station.Type, "")
				);

			}
			else
			{
				this.AllocationArray[1] = null;
			}

			Guid shipperAllocationGuid = FMChannelHelper.MakeCall<IAllocations, Guid>(
																	 x =>
																	 x.GetIdentityGuid(
																		this.Security,
																		companyMapClassCollection[1].IdentityGuid,
																		DateTimeOffset.Now,
																		DateTimeOffset.Now,
																		COMPANY_MAP_TYPE.SHIPPER_OWNER_MAP)
													);

			if (shipperAllocationGuid != Guid.Empty)
			{
				this.AllocationArray[2] = FMChannelHelper.MakeCall<IAllocations, AllocationClass>(
						x =>
						x.GetBySiteGuid(this.Security, shipperAllocationGuid, this.SiteManager.Site.IdentityGuid, this.Station.Type, "")
				);

			}
			else
			{
				this.AllocationArray[2] = null;
			}

			Guid ownerAllocationGuid = FMChannelHelper.MakeCall<IAllocations, Guid>(
																	 x =>
																	 x.GetIdentityGuid(
																				this.Security,
																				companyMapClassCollection[2].IdentityGuid,
																				DateTimeOffset.Now,
																				DateTimeOffset.Now,
																				COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP)
																	);

			if (ownerAllocationGuid != Guid.Empty)
			{
				this.AllocationArray[3] = FMChannelHelper.MakeCall<IAllocations, AllocationClass>(
						x =>
						x.GetBySiteGuid(this.Security, ownerAllocationGuid, this.SiteManager.Site.IdentityGuid, this.Station.Type, "")
				);

			}
			else
			{
				this.AllocationArray[3] = null;
			}
			return true;
		}
		protected bool ValidateCompanyHierarchy(CompanyMapClass shipToBillToMap)
		{
			DateTimeOffset siteTimeNow = TimeConverter.Now(this.SiteManager.Site);

			this.ShipTo = FMChannelHelper.MakeCall<ICompanies, CompanyClass>(
																	 x =>
																	 x.Get(this.Security, shipToBillToMap.AssignedGuid)
																);  // 3.1 sec

			this.ShipTo._LastActivityDate.Value = siteTimeNow;
			FMChannelHelper.MakeCall<ICompanies>(
																	 x =>
																	 x.Modify(this.Security, DATA_TYPE.DYNAMIC, this.ShipTo)
																); // .18 sec

			CompanyMapClass billToShipperMap = FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapClass>(
																	 x =>
																	 x.Get(this.Security, shipToBillToMap.AssignedToGuid, COMPANY_MAP_TYPE.BILLTO_SHIPPER_MAP)
																); // .76 sec

			this.BillTo = FMChannelHelper.MakeCall<ICompanies, CompanyClass>(
																	 x =>
																	 x.Get(this.Security, billToShipperMap.AssignedGuid)
																); // 1.2 sec

			this.BillTo._LastActivityDate.Value = siteTimeNow;
			FMChannelHelper.MakeCall<ICompanies>(
																	 x =>
																	 x.Modify(this.Security, DATA_TYPE.DYNAMIC, this.BillTo)
																); // .06 sec

			CompanyMapClass shipperOwnerMap = FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapClass>(
									x =>
									x.Get(this.Security, billToShipperMap.AssignedToGuid, COMPANY_MAP_TYPE.SHIPPER_OWNER_MAP)
							); // .7

			this.Shipper = FMChannelHelper.MakeCall<ICompanies, CompanyClass>(
																	 x =>
																	 x.Get(this.Security, shipperOwnerMap.AssignedGuid)
																); // 1.7 sec

			this.Shipper._LastActivityDate.Value = siteTimeNow;
			FMChannelHelper.MakeCall<ICompanies>(
																	 x =>
																	 x.Modify(this.Security, DATA_TYPE.DYNAMIC, this.Shipper)
																); //.05 sec

			CompanyMapClass ownerManagerMap = FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapClass>(
							x =>
							x.Get(this.Security, shipperOwnerMap.AssignedToGuid, COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP)
					); // 1.1 sec

			this.Owner = FMChannelHelper.MakeCall<ICompanies, CompanyClass>(
																	 x =>
																	 x.Get(this.Security, ownerManagerMap.AssignedGuid)
																);

			this.Owner._LastActivityDate.Value = siteTimeNow;
			FMChannelHelper.MakeCall<ICompanies>(
																	 x =>
																	 x.Modify(this.Security, DATA_TYPE.DYNAMIC, this.Owner)
																);

			this.Manager = FMChannelHelper.MakeCall<ICompanies, CompanyClass>(
																	 x =>
																	 x.Get(this.Security, ownerManagerMap.AssignedToGuid)
																);

			this.Manager._LastActivityDate.Value = siteTimeNow;
			FMChannelHelper.MakeCall<ICompanies>(
																	 x =>
																	 x.Modify(this.Security, DATA_TYPE.DYNAMIC, this.Manager)
																);
			bool validationResult;
			validationResult = this.ValidateCompany(this.ShipTo, COMPANY_ROLE.CUSTOMER_SHIPTO);
			if (!validationResult)
			{
				return false;
			}

			validationResult = this.ValidateCompany(this.BillTo, COMPANY_ROLE.CUSTOMER_BILLTO);
			if (!validationResult)
			{
				return false;
			}

			validationResult = this.ValidateCompany(this.Shipper, COMPANY_ROLE.SHIPPER);
			if (!validationResult)
			{
				return false;
			}

			validationResult = this.ValidateCompany(this.Owner, COMPANY_ROLE.OWNER);
			if (!validationResult)
			{
				return false;
			}

			validationResult = this.ValidateCompany(this.Manager, COMPANY_ROLE.MANAGER);
			if (!validationResult)
			{
				return false;
			}

			Guid shipToAllocationGuid = FMChannelHelper.MakeCall<IAllocations, Guid>(
																	 x =>
																	 x.GetIdentityGuid(
																		this.Security,
																		shipToBillToMap.IdentityGuid,
																		DateTimeOffset.Now,
																		DateTimeOffset.Now,
																		COMPANY_MAP_TYPE.SHIPTO_BILLTO_MAP)
												);

			if (shipToAllocationGuid != Guid.Empty)
			{
				this.AllocationArray[0] = FMChannelHelper.MakeCall<IAllocations, AllocationClass>(
						x =>
						x.GetBySiteGuid(this.Security, shipToAllocationGuid, this.SiteManager.Site.IdentityGuid, this.Station.Type, "")
				);

			}
			else
			{
				this.AllocationArray[0] = null;
			}

			Guid billToAllocationGuid = FMChannelHelper.MakeCall<IAllocations, Guid>(
																	 x =>
																	 x.GetIdentityGuid(
																		this.Security,
																		billToShipperMap.IdentityGuid,
																		DateTimeOffset.Now,
																		DateTimeOffset.Now,
																		COMPANY_MAP_TYPE.BILLTO_SHIPPER_MAP)
																	);

			if (billToAllocationGuid != Guid.Empty)
			{
				this.AllocationArray[1] = FMChannelHelper.MakeCall<IAllocations, AllocationClass>(
						x =>
						x.GetBySiteGuid(this.Security, billToAllocationGuid, this.SiteManager.Site.IdentityGuid, this.Station.Type, "")
				);

			}
			else
			{
				this.AllocationArray[1] = null;
			}

			Guid shipperAllocationGuid = FMChannelHelper.MakeCall<IAllocations, Guid>(
																	 x =>
																	 x.GetIdentityGuid(
																		this.Security,
																		shipperOwnerMap.IdentityGuid,
																		DateTimeOffset.Now,
																		DateTimeOffset.Now,
																		COMPANY_MAP_TYPE.SHIPPER_OWNER_MAP)
													);

			if (shipperAllocationGuid != Guid.Empty)
			{
				this.AllocationArray[2] = FMChannelHelper.MakeCall<IAllocations, AllocationClass>(
						x =>
						x.GetBySiteGuid(this.Security, shipperAllocationGuid, this.SiteManager.Site.IdentityGuid, this.Station.Type, "")
				);

			}
			else
			{
				this.AllocationArray[2] = null;
			}

			Guid ownerAllocationGuid = FMChannelHelper.MakeCall<IAllocations, Guid>(
																	 x =>
																	 x.GetIdentityGuid(
																				this.Security,
																				ownerManagerMap.IdentityGuid,
																				DateTimeOffset.Now,
																				DateTimeOffset.Now,
																				COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP)
																	);

			if (ownerAllocationGuid != Guid.Empty)
			{
				this.AllocationArray[3] = FMChannelHelper.MakeCall<IAllocations, AllocationClass>(
						x =>
						x.GetBySiteGuid(this.Security, ownerAllocationGuid, this.SiteManager.Site.IdentityGuid, this.Station.Type, "") // 1.2
				);

			}
			else
			{
				this.AllocationArray[3] = null;
			}

			return true;
		}

		protected bool ValidateOffLoadingCompanyHierarchy(CompanyMapClass SupplierOwnerMap)
		{
			DateTimeOffset siteTimeNow = TimeConverter.Now(this.SiteManager.Site);

			// off loading hierarchy is manager owner supplier
			this.Supplier = FMChannelHelper.MakeCall<ICompanies, CompanyClass>(
																	 x =>
																	 x.Get(this.Security, SupplierOwnerMap.AssignedGuid)
																);

			this.Supplier._LastActivityDate.Value = siteTimeNow;
			FMChannelHelper.MakeCall<ICompanies>(
																	 x =>
																	 x.Modify(this.Security, DATA_TYPE.DYNAMIC, this.Supplier)
																);

			CompanyMapClass OwnerManagerMap = FMChannelHelper.MakeCall<ICompanyMaps, CompanyMapClass>(
																	 x =>
																	 x.Get(this.Security, SupplierOwnerMap.AssignedToGuid, COMPANY_MAP_TYPE.OFFLOAD_OWNER_MANAGER_MAP)
																);

			this.Owner = FMChannelHelper.MakeCall<ICompanies, CompanyClass>(
																	 x =>
																	 x.Get(this.Security, OwnerManagerMap.AssignedGuid)
																);

			this.Owner._LastActivityDate.Value = siteTimeNow;
			FMChannelHelper.MakeCall<ICompanies>(
																	 x =>
																	 x.Modify(this.Security, DATA_TYPE.DYNAMIC, this.Owner)
																);

			this.Manager = FMChannelHelper.MakeCall<ICompanies, CompanyClass>(
																	 x =>
																	 x.Get(this.Security, OwnerManagerMap.AssignedToGuid)
																);

			this.Manager._LastActivityDate.Value = siteTimeNow;
			FMChannelHelper.MakeCall<ICompanies>(
																	 x =>
																	 x.Modify(this.Security, DATA_TYPE.DYNAMIC, this.Manager)
																);

			if (!this.ValidateCompany(this.Supplier, COMPANY_ROLE.SUPPLIER))
			{
				return false;
			}

			if (!this.ValidateCompany(this.Owner, COMPANY_ROLE.OWNER))
			{
				return false;
			}

			if (!this.ValidateCompany(this.Manager, COMPANY_ROLE.MANAGER))
			{
				return false;
			}

			return true;
		}

		private void OpenAndCreateCommLogFile(bool logStationStartup)
		{
			this.CommLogFileName = string.Empty;
			if (this.Station.LogCommunications == false)
			{
				return;
			}

			// build the path based on the day of the month to write this data in
			string directoryToCreateAndWriteTo = this.Station.LogCommPath + "\\" + DateTime.Now.Day.ToString(CultureInfo.InvariantCulture);

			// build the filename and path based on the station name
			this.CommLogFileName = directoryToCreateAndWriteTo + "\\" + this.Station.ID + "CL.clf";

			string fileStartUpString = "********** Station Startup - " + this.Station.ID + " - " + System.Convert.ToString(DateTime.Now) + " **********\r\n";

			try
			{
				// create the sub directory for the day we are dealing with
				Directory.CreateDirectory(directoryToCreateAndWriteTo);

				// check the last date this file was written to so we can determine if we should over write or not
				DateTime lastDateTime = File.GetLastWriteTime(this.CommLogFileName);

				if (lastDateTime.Month != DateTime.Now.Month ||
					lastDateTime.Day != DateTime.Now.Day)
				{
					FileStream newFileStream = File.Create(this.CommLogFileName);
					newFileStream.Close();
					if (logStationStartup == false)
					{
						fileStartUpString = "********** Station Running - " + this.Station.ID + " Log Continuation - " + System.Convert.ToString(DateTime.Now) + " **********\r\n";
					}
				}

				// write the data to the file
				if (logStationStartup)
				{
					File.AppendAllText(this.CommLogFileName, fileStartUpString);
				}
			}
			catch
			{
				this.eventLog.WriteEntry("Error Creating Comm File " + this.CommLogFileName, EventLogEntryType.Error);
				this.CommLogFileName = string.Empty;
			}
		}

		public void WriteLogDataToCommFile(string dataToWrite, bool dataOut)
		{
			this.WriteLogDataToCommFile(dataToWrite, dataOut ? CommLogDirection.Out : CommLogDirection.In);
		}

		internal void WriteLogDataToCommFile(string dataToWrite, CommLogDirection direction)
		{
			string fileDataString = string.Empty;
			if (this.CommLogFileName.Length == 0 || this.Station.LogCommunications == false)
			{
				return;
			}

			// create a new file and sub directory if required
			this.OpenAndCreateCommLogFile(false);

			// indicate the direction of the data
			switch (direction)
			{
				case CommLogDirection.None:
					fileDataString = "-- ";
					break;
				case CommLogDirection.In:
					fileDataString = "<- ";
					break;
				case CommLogDirection.Out:
					fileDataString = "-> ";
					break;
			}

			// add the current date and time
			fileDataString += System.Convert.ToString(DateTime.Now);
			fileDataString += " - ";
			fileDataString += dataToWrite;
			fileDataString += "\r\n";

			try
			{
				File.AppendAllText(this.CommLogFileName, fileDataString);
			}
			catch
			{
				this.eventLog.WriteEntry("Error Writing Data to Comm File " + this.CommLogFileName, EventLogEntryType.Warning);
			}
		}

		public void WriteMenuLogDataToCommFile(DisplayMenuParameters parameters)
		{
			if (this.CommLogFileName.Length == 0 || this.Station.LogCommunications == false)
			{
				return;
			}

			string dataToWrite = "Menu Items";

			foreach (string value in parameters.Menu)
			{
				dataToWrite += " - " + value;
			}

			this.WriteLogDataToCommFile(dataToWrite, CommLogDirection.Out);
		}

		private bool CheckMaximumRetries()
		{
			this.ConsecutivePrompts++;
			if (this.ConsecutivePrompts >= this.SiteManager.Site._MaximumPrompts)
			{
				FMChannelHelper.MakeCall<IAlarmAndEventLogs>(x => x.Add(this.Security, this.Station.MaxRetriesExceededKeyEvent(this.Station.ID)));
				this.LoadRackManager.EventOrAlarmEvent.Set();

				this.StationState = StationState.RESET_ON_TIMEOUT;
				this.DisplayMessage("LoadRack|Max Retries Exceeded", null, 0, this.MESSAGE_TIMEOUT);
				this.ConsecutivePrompts = 0;
				return true;
			}

			return false;
		}

		public static bool IsTransactionScheduledOrder(TransactionDO order)
		{
			if (order == null || order.TransTypeID != TransactionTypes.T17_Order)
			{
				return false;
			}

			if (order.CarrierCompanyGuid == Guid.Empty)
			{
				return false;
			}

			foreach (LineItemDO orderLineItem in order.LineItems)
			{
				if (orderLineItem.DestinationEQ.EquipmentGuid == Guid.Empty)
				{
					return false;
				}

				try
				{
					// ReSharper disable once ReturnValueOfPureMethodIsNotUsed
					System.Convert.ToInt32(orderLineItem.DestinationCompartmentID);
				}
				catch
				{
					return false;
				}
			}

			return true;
		}


		protected bool CheckEquipmentOnOrder(EquipmentClass equipment)
		{
			if (!this.IsScheduledOrder)
			{
				return true;
			}

			if (equipment.Type == EQUIPMENT_TYPE.TRACTOR_TYPE)
			{
				return true;
			}

			foreach (LineItemDO orderLineItem in this.Order.LineItems)
			{
				bool openItem = true;

				if (orderLineItem.Status == TransactionStatus.Completed)
				{
					continue;
				}

				if (orderLineItem.DestinationEQ.EquipmentGuid == equipment.IdentityGuid)
				{
					int compartmentNumber;
					try
					{
						compartmentNumber = System.Convert.ToInt32(orderLineItem.DestinationCompartmentID);
					}
					catch (FormatException)
					{
						continue;
					}
					catch (OverflowException)
					{
						continue;
					}

					if (compartmentNumber > equipment.CompartmentCollection.Count)
					{
						continue;
					}

					if (this.Transaction == null)
					{
						return true;
					}

					foreach (LineItemDO lineItem in this.Transaction.LineItems)
					{
						if (lineItem.DestinationEQ.EquipmentGuid == equipment.IdentityGuid
						&& orderLineItem.DestinationCompartmentID == lineItem.DestinationCompartmentID)
						{
							openItem = false;
							break;
						}
					}

					if (openItem)
					{
						return true;
					}
				}
			}

			return false;
		}

		private bool CheckCompartmentsAvailable(EquipmentClass equipment)
		{
			if (!this.SiteManager.Site.PromptForCompartment)
			{
				return true;
			}

			byte compartmentMap = 0;
			byte compartmentIndex = 0;

			// ReSharper disable once UnusedVariable
			foreach (EquipmentClass compartment in equipment.CompartmentCollection)
			{
				compartmentMap |= (byte)(1 << compartmentIndex);
				compartmentIndex++;
			}

			if (this.inprogressTransaction != null)
			{
				if (this.Transaction != null)
				{
					foreach (TransactionDO transaction in this.inprogressTransaction)
					{
						if (this.Transaction != null &&
							 this.Transaction.TransID == transaction.TransID)
						{
							continue;
						}

						// ReSharper disable CompareOfFloatsByEqualityOperator
						foreach (LineItemDO lineItem in transaction.LineItems)
						{
							if (lineItem.DestinationEQ.EquipmentGuid == equipment.IdentityGuid
							&& !(lineItem.GrossInventoryChange == 0.0 && lineItem.NetInventoryChange == 0.0 && lineItem.Status == TransactionStatus.Cancelled)
							&& !string.IsNullOrEmpty(lineItem.DestinationCompartmentID))
							{
								compartmentMap &= (byte)(0xFE << (System.Convert.ToInt32(lineItem.DestinationCompartmentID) - 1));
							}
						}
						// ReSharper restore CompareOfFloatsByEqualityOperator
					}
				}
			}

			return compartmentMap != 0;
		}

		private int TransportEquipmentSpecified
		{
			get
			{
				int equipmentCount = 0;

				EquipmentClass[] equipmentArray = { this.TractorOrTanker, this.Trailer1, this.Trailer2, this.Trailer3 };

				foreach (EquipmentClass equipment in equipmentArray)
				{
					if (equipment != null
					&& (equipment.Type == EQUIPMENT_TYPE.TANKER_TYPE
					|| equipment.Type == EQUIPMENT_TYPE.TRAILER_TYPE))
					{
						equipmentCount++;
					}
				}

				return equipmentCount;
			}
		}

		private bool CheckCompartmentAvailability()
		{
			this.SetCurrentEquipmentToFirstAvailable();

			if (this.CurrentEquipment == null)
			{
				FMChannelHelper.MakeCall<IAlarmAndEventLogs>(x => x.Add(this.Security, this.Station.NoCompartmentsToLoadEvent(this.Station.ID)));
				this.LoadRackManager.EventOrAlarmEvent.Set();

				this.StationState = StationState.RESET_ON_TIMEOUT;
				this.DisplayMessage("LoadRack|No Compartments Available", null, 0, this.MESSAGE_TIMEOUT, false);
				return false;
			}

			return true;
		}

		private bool CheckTransactionCanAccomodateEquipment(EquipmentClass equipment)
		{
			if (this.Transaction == null)
			{
				return true;
			}

			int equipmentToAdd = 0;

			EquipmentClass[] equipmentArray = { this.TractorOrTanker, this.Trailer1, this.Trailer2, this.Trailer3 };

			foreach (EquipmentClass equipmentElement in equipmentArray)
			{
				if (equipmentElement != null
				&& equipmentElement.IdentityGuid != equipment.IdentityGuid
				&& !this.CheckEquipmentInTransaction(equipmentElement))
				{
					equipmentToAdd++;
				}
			}

			int checkEquipmentInTransaction = 0;

			EquipmentDO[] equipmentDOArray = { this.Transaction.DestinationEQ1, this.Transaction.DestinationEQ2, this.Transaction.DestinationEQ3, this.Transaction.DestinationEQ4 };

			foreach (EquipmentDO equipmentDOElement in equipmentDOArray)
			{
				if (equipmentDOElement != null
				&& equipmentDOElement.EquipmentGuid != Guid.Empty)
				{
					checkEquipmentInTransaction++;
				}
			}

			return 4 - checkEquipmentInTransaction >= equipmentToAdd;
		}

		protected bool CheckEquipmentInTransaction(EquipmentClass equipment)
		{
			if ((this.Transaction?.DestinationEQ1?.EquipmentGuid ?? Guid.Empty) != Guid.Empty
			&& this.Transaction.DestinationEQ1.EquipmentGuid == equipment.MasterRecordGuid)
			{
				return true;
			}

			if ((this.Transaction?.DestinationEQ2?.EquipmentGuid ?? Guid.Empty) != Guid.Empty
			&& this.Transaction.DestinationEQ2.EquipmentGuid == equipment.MasterRecordGuid)
			{
				return true;
			}

			if ((this.Transaction?.DestinationEQ3?.EquipmentGuid ?? Guid.Empty) != Guid.Empty
			&& this.Transaction.DestinationEQ3.EquipmentGuid == equipment.MasterRecordGuid)
			{
				return true;
			}

			if ((this.Transaction?.DestinationEQ4?.EquipmentGuid ?? Guid.Empty) != Guid.Empty
			&& this.Transaction.DestinationEQ4.EquipmentGuid == equipment.MasterRecordGuid)
			{
				return true;
			}

			return false;
		}

		private void SetCurrentEquipmentToFirstAvailable()
		{
			this.CurrentEquipment = null;

			if (this.AvailableCompartments(this.TractorOrTanker) > 0)
			{
				this.CurrentEquipment = this.TractorOrTanker;
			}
			else if (this.AvailableCompartments(this.Trailer1) > 0)
			{
				this.CurrentEquipment = this.Trailer1;
			}
			else if (this.AvailableCompartments(this.Trailer2) > 0)
			{
				this.CurrentEquipment = this.Trailer2;
			}
			else if (this.AvailableCompartments(this.Trailer3) > 0)
			{
				this.CurrentEquipment = this.Trailer3;
			}
		}

		protected virtual bool CheckProductsInStation()
		{
			return true;
		}
		#endregion

		public virtual void UploadStoredTransactions()
		{
			throw new NotImplementedException();
		}

		protected void DisplayMessageWithAcknowledge(string message)
		{
			this.DisplayMessage(message + " " + this.AcknowledgementMessage, null, this.AcknowledgementResponseLength, this.PROMPT_TIMEOUT);
		}

		public virtual bool SetDownloadDensityInUnitFlag(string density)
		{
			throw new NotImplementedException();
		}

		private void TruckPresent(out bool Bypass, out bool TruckPresent, out string TruckSerialNumber, out bool DataInValid)
		{
			TruckPresent = false;
			Bypass = false;
			TruckSerialNumber = string.Empty;
			DataInValid = false;
			foreach (ProcessVariableClass pv in this.Station.ProcessVariableCollection)
			{
				if (pv.ProcessVariableType == PROCESS_VARIABLE_TYPE.SCULLY_PV)
				{
					string tagPrefix = pv.OPCItemID + ".";

					Item[] TruckPresentItem = { new Item(new ItemIdentifier(tagPrefix + "Truck Present")) };
					Item[] TruckSerialNumberItem = { new Item(new ItemIdentifier(tagPrefix + "Truck Serial Number")) };
					Item[] BypassItem = { new Item(new ItemIdentifier(tagPrefix + "Bypass")) };

					try
					{
						Server server = new Server(new Factory(), new URL(pv.URL));
						NetworkCredential credentials = null;
						server.Connect(new ConnectData(credentials));
						ItemValueResult[] BypassValue = server.Read(BypassItem);

						if (BypassValue[0].ResultID == ResultID.S_OK)
						{
							if (BypassValue[0].Quality == Quality.Good)
							{
								Bypass = System.Convert.ToBoolean(BypassValue[0].Value);
							}
							else
							{
								DataInValid = true;
							}
						}
						else
						{
							DataInValid = true;
							this.eventLog.WriteEntry("Scully Read Item Error:  " + BypassValue[0].ResultID.Code.ToString(), EventLogEntryType.Error);
						}
						if (!Bypass)
						{
							ItemValueResult[] TruckPresentValue = server.Read(TruckPresentItem);

							if (TruckPresentValue[0].ResultID == ResultID.S_OK)
							{
								if (TruckPresentValue[0].Quality == Quality.Good)
								{
									TruckPresent = System.Convert.ToBoolean(TruckPresentValue[0].Value);
								}
								else
								{
									DataInValid = true;
								}
							}
							else
							{
								DataInValid = true;
								this.eventLog.WriteEntry("Scully Read Item Error:  " + TruckPresentValue[0].ResultID.Code.ToString(), EventLogEntryType.Error);
							}
						}
						if (TruckPresent)
						{
							ItemValueResult[] TruckSerialNumberValue = server.Read(TruckSerialNumberItem);
							if (TruckSerialNumberValue[0].ResultID == ResultID.S_OK)
							{
								if (TruckSerialNumberValue[0].Quality == Quality.Good)
								{
									TruckSerialNumber = TruckSerialNumberValue[0].Value.ToString();
									TruckSerialNumber = TruckSerialNumber.TrimStart('0');
								}
								else
								{
									DataInValid = true;
								}
							}
							else
							{
								DataInValid = true;
								this.eventLog.WriteEntry("TIM Read Item Error:  " + TruckSerialNumberValue[0].ResultID.Code.ToString(CultureInfo.InvariantCulture), EventLogEntryType.Error);
							}

						}

						server.Disconnect();
						server.Dispose();
					}
					catch
					{
						// ReSharper restore ImpureMethodCallOnReadonlyValueField
						this.eventLog.WriteEntry("Read : Connection error to " + pv.URL, EventLogEntryType.Error);
					}
					return;
				}
			}
		}

		private void ScullyUnavailableMessage()
		{
			this.DisplayMessage("[LoadRack|Invalid], [LoadRack|Scully TIM Not Found]", null, 0, this.MESSAGE_TIMEOUT * 3); // display this message longer
			if (this.TIN == "FFFFFFFFFFFF")
			{
				this.AddAlarmAndEventLogs(this.Security, this.Station.ScullyTIMNotDetectedEvent(this.Driver.FirstLastName, this.Carrier.ID));
				this.LoadRackManager.EventOrAlarmEvent.Set();
			}
		}

		protected bool IsOffloadProductAvailable(ProductMapClass product)
		{
			// Look for Load Arm through which product is available
			foreach (LoadArmManagerClass loadArmManager in this.LoadArmManagerCollection)
			{
				ProductMapClass recipe = loadArmManager.AvailableRecipeCollection.Find(x => x.AssignedGuid == product.AssignedGuid);
				if (recipe == null)
				{
					continue;
				}

				if (!recipe.EnableRecipe)
				{
					continue;
				}

				if (!recipe.Permissives.Permitted)
				{
					continue;
				}

				return true;
			}

			return false;
		}

		protected void CheckOffloadProductAvailability(bool acknowledged)
		{
			if (this.StationState == StationState.IDLE)
			{
				if (!this.CheckProductsInStation())
				{
					this.StationState = StationState.RESET_ON_TIMEOUT;
					this.DisplayMessage("LoadRack|Configuration Mismatch", null, 0, this.MESSAGE_TIMEOUT);
					this.ConsecutivePrompts = 0;
					return;
				}

				this.DisplayPleaseWaitMessage();
				this.AuthorizedProductIndex = 0;
			}
			else
			{
				if (acknowledged)
				{
					this.AuthorizedProductIndex++;
					this.StationState = StationState.IDLE;
				}
				else
				{
					this.StationState = StationState.IDLE;
					this.DisplayMessage("LoadRack|Message Timeout", null, 0, this.MESSAGE_TIMEOUT);
					this.ConsecutivePrompts = 0;
					return;
				}
			}

			ProductMapCollectionClass byVolumeExcludedProducts = FMChannelHelper.MakeCall<IProductMaps, ProductMapCollectionClass>(x => x.EnumerateByAssignedToGuidAndType(this.Security, this.Station.ReceiptByVolumeTransactionAliasGuid, PRODUCT_MAP_TYPE.TRANSACTION_ALIAS_EXCLUSION_MAP));
			ProductMapCollectionClass byWeightExcludedProducts = FMChannelHelper.MakeCall<IProductMaps, ProductMapCollectionClass>(x => x.EnumerateByAssignedToGuidAndType(this.Security, this.Station.ReceiptByWeightTransactionAliasGuid, PRODUCT_MAP_TYPE.TRANSACTION_ALIAS_EXCLUSION_MAP));

			if (this.Supplier == null)
			{
				// This should never happen if the sequencing of prompts works correctly
				// As such, simply print a message and blow back to idle
				this.eventLog.WriteEntry(
					 "Offload bail out to idle after sequencing error.", EventLogEntryType.Error);
				this.StationState = StationState.RESET_ON_TIMEOUT;
				this.DisplayMessageWithAcknowledge("LoadRack|No Supplier to receive delivery from");
				return;
			}

			for (; this.AuthorizedProductIndex < this.Supplier.SupplierAuthorizedProductCollection.Count; this.AuthorizedProductIndex++)
			{
				ProductMapClass authorizedProduct = this.Supplier.SupplierAuthorizedProductCollection[this.AuthorizedProductIndex];

				bool available = false;

				switch (this.Station.Type)
				{
					case STATION_TYPE.OFF_LOADING:
						if (authorizedProduct.LoadByWeight)
						{
							if (null != byWeightExcludedProducts.Find(x => x.AssignedGuid == authorizedProduct.AssignedGuid))
							{
								authorizedProduct.LockedOut = true;
								continue;
							}
						}
						else
						{
							if (null != byVolumeExcludedProducts.Find(x => x.AssignedGuid == authorizedProduct.AssignedGuid))
							{
								authorizedProduct.LockedOut = true;
								continue;
							}
						}

						if (this.Transaction != null
							 && (this.Transaction.Status == TransactionStatus.LoadPending
								  || this.Transaction.Status == TransactionStatus.WeighOutPending))
						{
							available = this.IsProductOpenOnTransaction(authorizedProduct.AssignedGuid);
							if (available)
							{
								available = this.IsOffloadProductAvailable(authorizedProduct);
							}
						}
						else if (this.SupplyOrder != null)
						{
							// Pick Up against an Order
							available = this.IsProductOpenOnSupplyOrder(authorizedProduct.AssignedGuid);
							if (available)
							{
								available = this.IsOffloadProductAvailable(authorizedProduct);
							}
						}
						else
						{
							available = this.IsOffloadProductAvailable(authorizedProduct);
						}

						if (!available)
						{
							authorizedProduct.LockedOut = true;
						}

						break;
					case STATION_TYPE.PRELOAD:
					case STATION_TYPE.WEIGHT_SCALE:
						if (this.SupplyOrder != null)
						{
							if (!this.IsProductOpenOnSupplyOrder(authorizedProduct.AssignedGuid))
							{
								authorizedProduct.LockedOut = true;
								continue;
							}
						}

						// For offloads, should only be components
						if (authorizedProduct.AssignedProductType != ProductType.ComponentProduct)
						{
							authorizedProduct.LockedOut = true;
						}

						if (authorizedProduct.LoadByWeight)
						{
							if (null != byWeightExcludedProducts.Find(x => x.AssignedGuid == authorizedProduct.AssignedGuid))
							{
								authorizedProduct.LockedOut = true;
								continue;
							}

							if (this.Station.ReceiptByWeightTransactionAliasGuid == Guid.Empty)
							{
								authorizedProduct.LockedOut = true;
								continue;
							}
						}
						else
						{
							if (null != byVolumeExcludedProducts.Find(x => x.AssignedGuid == authorizedProduct.AssignedGuid))
							{
								authorizedProduct.LockedOut = true;
								continue;
							}

							if (this.Station.ReceiptByVolumeTransactionAliasGuid == Guid.Empty)
							{
								authorizedProduct.LockedOut = true;
								continue;
							}
						}

						available = true;

						break;
				}

				if (available)
				{
					if (authorizedProduct.LockedOut)
					{
						this.StationState = StationState.OFFLOAD_PRODUCT_AVAILABILITY_MESSAGE;
						FMChannelHelper.MakeCall<IAlarmAndEventLogs>(x => x.Add(this.Security, this.Station.ProductLockedOutAlarm(authorizedProduct.AssignedID)));
						this.LoadRackManager.EventOrAlarmEvent.Set();
						this.DisplayMessageWithAcknowledge("[LoadRack|Locked Out:] " + GetLoadRackDisplayText(authorizedProduct));
						return;
					}

					// Check Hazardous Material 
					if (this.Carrier.HazardousMaterialExclusion && authorizedProduct.HazardousMaterial)
					{
						authorizedProduct.LockedOut = true;
						this.StationState = StationState.OFFLOAD_PRODUCT_AVAILABILITY_MESSAGE;
						FMChannelHelper.MakeCall<IAlarmAndEventLogs>(x => x.Add(this.Security, this.Carrier.HazardousMaterialExclusionEvent(authorizedProduct.AssignedID, this.Driver.ID, this.GetStationName())));
						this.LoadRackManager.EventOrAlarmEvent.Set();
						this.DisplayMessageWithAcknowledge("[LoadRack|Hazardous Material Exclusion:] " + GetLoadRackDisplayText(authorizedProduct));
						return;
					}
				}
			}

			this.StationState = StationState.IDLE;

			this.OffloadProductAvailabilityCompletion();
		}

		protected void OffloadProductAvailabilityCompletion()
		{
			switch (this.Station.Type)
			{
				case STATION_TYPE.OFF_LOADING:
					{
						if (this.Transaction == null
							 || this.Transaction.Status == TransactionStatus.InProgress
							 || this.Transaction.DeleteFlag)
						{
							this.CurrentTransactionAlias = FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasClass>(x => x.Get(this.Security, this.Station.ReceiptByVolumeTransactionAliasGuid, false));
							this.CompartmentList = null;
							this.GenerateCompartmentList();
							this.StationState = StationState.AUTHORIZING;
							this.BuildOffloadRecipeMapForAllLoadArms(false);
						}
						else
						{
							foreach (LoadArmManagerClass loadArmManager in this.LoadArmManagerCollection)
							{
								if (!loadArmManager.UpdateMaximumPreset(this))
								{
									if (!loadArmManager.LogOutOfProgramMode())
									{
										return;
									}

									loadArmManager.DisplayMessage("LoadRack|Update Maximum Preset Error", 0, this.MESSAGE_TIMEOUT);
									return;
								}

								if (!loadArmManager.LogOutOfProgramMode())
								{
									return;
								}

								break;
							}

							this.CurrentTransactionAlias = FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasClass>(x => x.Get(this.Security, this.Transaction.TransactionAliasGuid, false));
							this.BuildPlan(false);
						}
					}

					break;
				case STATION_TYPE.WEIGHT_SCALE:
				case STATION_TYPE.PRELOAD:
					if (this.Transaction == null)
					{
						this.InitializeTransaction();
					}

					this.IssueSelectOffloadProductPrompt();
					break;
			}
		}

		protected virtual void ProcessOffloadProduct(string response)
		{
			try
			{
				if (this.CurrentMenuParameters != null)
				{
					int selection = System.Convert.ToInt32(response);

					if (selection == 0 || selection > this.CurrentMenuParameters.Menu.Length)
					{
						this.IssueAdditionalOrdersPrompt();
						return;
					}

					response = FMChannelHelper.MakeCall<IDataDictionariesClass,string>(dataDictionaries => dataDictionaries.Get(
						 this.SiteManager.Site.SiteGuid,
						 this.CurrentMenuParameters.Menu[selection - 1]));
				}
			}
			catch (FormatException)
			{
			}
			catch (OverflowException)
			{
			}
			ProcessOffloadProductExt(response);
		}

		protected void ProcessOffloadProductExt(string response)
		{
			if (response == EscapeString || string.IsNullOrEmpty(response))
			{
				this.DetermineTypeOfOffLoadingOperation();
				return;
			}

			LineItemDO lineItem = this.CurrentLineItem;


			if (response == GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, "LoadRack|None"))
			{
				if (lineItem != null)
				{
					lineItem.Product = null;
					lineItem.ProductGuid = Guid.Empty;
					lineItem.ProductCode = null;
					lineItem.ProductType = null;
					lineItem.CustomerProductName = null;
					lineItem.CustomerProductCode = null;
					lineItem.PresetAmount = null;
					lineItem.StorageLocationID = null;
					lineItem.StorageLocationTankGuid = Guid.Empty;
					lineItem.Density = null;
					lineItem.VCF = null;
					lineItem.OrderReferenceTransactionLineItemGuid = Guid.Empty;
				}

				if (!this.ProductsConfigured)
				{
					this.ByWeight = false;
					this.ByWeightProduct = string.Empty;
				}

				if (this.AvailableCompartments(this.CurrentEquipment) > 1)
				{
					this.IssueCompartmentSummaryPrompt();
				}
				else
				{
					this.IssueLoadSummaryPrompt();
				}
			}
			else
			{
				foreach (ProductMapClass authorizedProduct in this.Supplier.SupplierAuthorizedProductCollection)
				{
					if (response != GetLoadRackDisplayText(authorizedProduct))
					{
						continue;
					}

					ProductClass product = GetProduct(this.Security, authorizedProduct.AssignedGuid);

					this.CurrentTransactionAlias = GetTransactionAlias(this.Security, product.LoadByWeight ? this.Station.ReceiptByWeightTransactionAliasGuid : this.Station.ReceiptByVolumeTransactionAliasGuid, false);

					this.Transaction.TransTypeID = this.CurrentTransactionAlias._TransTypeID;
					this.Transaction.TransactionAliasGuid = this.CurrentTransactionAlias.MasterRecordGuid;

					lineItem.Product = authorizedProduct.AssignedID;
					lineItem.ProductCode = authorizedProduct.AssignedCode;
					lineItem.ProductType = ProductClass.ProductTypeID(product.ProductType);
					lineItem.CustomerProductName = authorizedProduct.AssignedID;
					lineItem.CustomerProductCode = authorizedProduct.AssignedCode;
					lineItem.ProductGuid = authorizedProduct.AssignedGuid;

					// Density and VCF are set to default values.  When Tank is determined
					// these are reset to values from SCADA.
					EngineeringUnit units = (this.CurrentTransactionAlias.DensityUnits != 0) ? this.CurrentTransactionAlias.DensityUnits : this.SiteManager.Site.DensityUnits;
					var standardDensity = new SIDouble { Units = units, SIValue = product._StandardDensity.SIValue };
					lineItem.Density = standardDensity.Value;

					lineItem.VCF = 1.0;

					units = (this.CurrentTransactionAlias.TemperatureUnits != 0) ? this.CurrentTransactionAlias.TemperatureUnits : this.SiteManager.Site.TemperatureUnits;
					var standardTemperature = new SIDouble { Units = units, SIValue = product._VcfModuleSettings.BaseTemperature.Value };
					lineItem.Temperature = standardTemperature.Value;

					bool productAvailable = false;

					if (product.ProductType == ProductType.ComponentProduct)
					{
						foreach (StationManagerClass stationManager in this.SiteManager.StationManagerCollection)
						{
							bool stationProductAvailable = false;

							if (!stationManager.Station.Enabled)
							{
								continue;
							}

							if (stationManager.Station.Type != STATION_TYPE.OFF_LOADING)
							{
								continue;
							}

							if (product.LoadByWeight)
							{
								// This is a bit confusing because in SiteLoadRackPage the alias is stored in the IssueByVolumeTransactionAliasIndex
								// even though the station may be a manual station created to represent a Load By Weight Station
								if (stationManager.Station.ReceiptByVolumeTransactionAliasGuid
									 != this.Station.ReceiptByWeightTransactionAliasGuid)
								{
									continue;
								}
							}
							else
							{
								if (stationManager.Station.ReceiptByVolumeTransactionAliasGuid
									 != this.Station.ReceiptByVolumeTransactionAliasGuid)
								{
									continue;
								}
							}

							TankClass tank = null;

							foreach (LoadArmManagerClass loadArmManager in stationManager.LoadArmManagerCollection)
							{
								stationProductAvailable = false;

								if (stationManager != loadArmManager.GetStationManager())
								{
									continue;
								}

								if (!loadArmManager.IsOffloadProductServedByLoadArm(product))
								{
									continue;
								}

								ProductMapClass loadArmComponent = loadArmManager.GetOffloadComponent(product.MasterRecordGuid);
								if (loadArmComponent == null)
								{
									continue;
								}

								tank = this.SiteManager.GetTank(loadArmComponent, this.Manager);
								if (tank == null)
								{
									continue;
								}

								try
								{
									units = (this.CurrentTransactionAlias.DensityUnits != 0) ? this.CurrentTransactionAlias.DensityUnits : this.SiteManager.Site.DensityUnits;

									var productDensity = new SIDouble
									{
										Units = units,
										SIValue = this.GetTankValue(tank, PROCESS_VARIABLE_TYPE.DENSITY_PV)
									};

									double productVcf = this.GetTankValue(tank, PROCESS_VARIABLE_TYPE.VCF_PV);

									double productPressure = this.GetTankValue(tank, PROCESS_VARIABLE_TYPE.VAPOR_PRESSURE_PV);

									units = (this.CurrentTransactionAlias.TemperatureUnits != 0) ? this.CurrentTransactionAlias.TemperatureUnits : this.SiteManager.Site.TemperatureUnits;

									var productTemperature = new SIDouble
									{
										Units = units,
										SIValue =
											  this.GetTankValue(tank, PROCESS_VARIABLE_TYPE.TEMPERATURE_PV)
									};

									lineItem.Density = productDensity.Value;
									lineItem.VCF = productVcf;
									lineItem.Temperature = productTemperature.Value;
									this.CurrentMaximum = this.GetMaximum(product, lineItem.Density, lineItem.VCF, productPressure, null, lineItem);

									if (
										 !stationManager.IsProductAvailable(
											  product, loadArmManager, this.CurrentMaximum, this.CurrentTransactionAlias))
									{
										continue;
									}

									productAvailable = true;
									stationProductAvailable = true;
								}
								catch (Exception e)
								{
									this.eventLog.WriteEntry("StationManager ProcessProduct : " + e.Message, EventLogEntryType.Error);
									continue;
								}

								break;
							}

							// Break if Load Arm Identified for Loading
							if (stationProductAvailable)
							{
								// If Loading By Weight Record the Storage Location
								if (product.LoadByWeight)
								{
									lineItem.StorageLocationID = tank.ID;
									lineItem.StorageLocationTankGuid = tank.IdentityGuid;
								}

								break;
							}
						}

						// if product fails on all load arms
						if (!productAvailable)
						{
							FMChannelHelper.MakeCall<IAlarmAndEventLogs>(alarmAndEventLogs => alarmAndEventLogs.Add(this.Security, this.Station.ProductUnavailableAlarm(product.ID, this.Driver.ID)));
							this.LoadRackManager.EventOrAlarmEvent.Set();
						}
					}

					if (product.LoadByWeight)
					{
						this.ByWeight = true;
						this.ByWeightProduct = response;
					}

					if (!productAvailable)
					{
						this.StationState = StationState.PRODUCT_UNAVAILABLE_MESSAGE;
						this.DisplayMessageWithAcknowledge("[LoadRack|Product Unavailable], ");
						return;
					}

					if (product.LoadByWeight)
					{
						if (this.Station.ReceiptByWeightTransactionAliasGuid == Guid.Empty)
						{
							this.ByWeight = false;
							this.ByWeightProduct = string.Empty;

							this.StationState = StationState.TRANSACTION_ALIAS_INVALID_MSG;
							this.DisplayMessage("LoadRack|Transaction Alias Invalid", null, 0, this.MESSAGE_TIMEOUT);
							return;
						}

						this.PromptForOffLoadDensity();

						return;
					}

					if (this.Station.ReceiptByVolumeTransactionAliasGuid == Guid.Empty)
					{
						this.StationState = StationState.TRANSACTION_ALIAS_INVALID_MSG;
						this.DisplayMessage("LoadRack|Transaction Alias Invalid", null, 0, this.MESSAGE_TIMEOUT);
						return;
					}

					this.IssuePresetPrompt();
					return;
				}

				this.StationState = StationState.PRODUCT_UNAVAILABLE_MESSAGE;
				this.DisplayMessageWithAcknowledge("[LoadRack|Product Unavailable], ");
			}
			// ReSharper restore PossibleUnintendedReferenceComparison
		}

		/// <summary>
		/// Issues the offload summary prompt.
		/// </summary>
		protected void IssueOffloadSummaryPrompt()
		{
			this.LoadSummaryIssued = true;

			string dataDictionaryNone = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(x => x.Get(this.SiteManager.Site.SiteGuid, "LoadRack|None"));
			string dataDictionaryProduct = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(x => x.Get(this.SiteManager.Site.SiteGuid, "LoadRack|Product"));
			string dataDictionaryPreset = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(x => x.Get(this.SiteManager.Site.SiteGuid, "LoadRack|Preset"));
			string dataDictionaryCompartments = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(x => x.Get(this.SiteManager.Site.SiteGuid, "LoadRack|Compartment(s)"));
			string dataDictionaryCompartment = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(x => x.Get(this.SiteManager.Site.SiteGuid, "LoadRack|Compartment"));
			string dataDictionaryTanker = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(x => x.Get(this.SiteManager.Site.SiteGuid, "LoadRack|Tanker"));
			string dataDictionaryTrailer = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(x => x.Get(this.SiteManager.Site.SiteGuid, "LoadRack|Trailer"));

			ArrayList summary = new ArrayList();

			if (this.SupplyOrder == null)
			{
				summary.Add(FMChannelHelper.MakeCall<IDataDictionariesClass, string>(x => x.Get(this.SiteManager.Site.SiteGuid, "LoadRack|Supply Order]")) + " : " + "N/A");
			}
			else
			{
				summary.Add(FMChannelHelper.MakeCall<IDataDictionariesClass, string>(x => x.Get(this.SiteManager.Site.SiteGuid, "LoadRack|Supply Order]")) + " : " + this.SupplyOrder.DocumentNumber);
			}

			summary.Add(FMChannelHelper.MakeCall<IDataDictionariesClass, string>(x => x.Get(this.SiteManager.Site.SiteGuid, "[LoadRack|Supplier]")) + " : " + ((this.Supplier == null) ? string.Empty : this.Supplier.ID));
			string prompt;

			this.StationState = StationState.SUMMARY_PROMPT;
			int defaultItem;

			if (this.ByWeight)
			{
				// For by weight, assume only one line item; this is a safe assumption because we require and 
				// force this to be the case.
				string productId;
				if (this.Transaction.LineItems.Count == 0)
				{
					productId = "[LoadRack|None]";
				}
				else
				{
					productId = this.Transaction.LineItems[0].Product;
				}

				{
					prompt = dataDictionaryProduct + " : " + productId;
					defaultItem = summary.Count;
					summary.Add(FMChannelHelper.MakeCall<IDataDictionariesClass, string>(x => x.Get(this.SiteManager.Site.SiteGuid, "LoadRack|Accept")));
				}
			}
			else
			{
				EquipmentCollectionClass equipmentCollection = new EquipmentCollectionClass();

				if (this.TractorOrTanker != null && this.TractorOrTanker.Type == EQUIPMENT_TYPE.TANKER_TYPE)
				{
					equipmentCollection.Add(this.TractorOrTanker);
				}

				if (this.Trailer1 != null)
				{
					equipmentCollection.Add(this.Trailer1);
				}

				if (this.Trailer2 != null)
				{
					equipmentCollection.Add(this.Trailer2);
				}

				if (this.Trailer3 != null)
				{
					equipmentCollection.Add(this.Trailer3);
				}

				foreach (EquipmentClass equipment in equipmentCollection)
				{
					if (this.AvailableCompartments(equipment) == 0)
					{
						continue;
					}

					if (equipment.Type == EQUIPMENT_TYPE.TANKER_TYPE)
					{
						prompt = dataDictionaryTanker;
					}
					else
					{
						prompt = dataDictionaryTrailer;
					}

					prompt += " " + this.EquipmentID(equipment);

					if (this.AvailableCompartments(equipment) == 1)
					{
						this.CurrentEquipment = equipment;

						this.CurrentCompartmentNumber = this.FirstAvailableCompartment(equipment);

						prompt += " " + dataDictionaryCompartment + " "
									 + this.CurrentCompartmentNumber.ToString(CultureInfo.InvariantCulture) + " "
									 + dataDictionaryProduct + ": ";

						LineItemDO lineItem = this.CurrentLineItem;
						if (string.IsNullOrEmpty(lineItem.Product))
						{
							summary.Add(prompt + dataDictionaryNone);
						}
						else
						{
							if (this.ByWeight)
							{
								if (lineItem.ProductGuid == Guid.Empty)
								{
									summary.Add(prompt + dataDictionaryNone);
								}
								else
								{
									summary.Add(prompt + this.GetLoadRackDisplayText(lineItem.ProductGuid));
								}
							}
							else
							{
								summary.Add(
									 prompt + this.GetLoadRackDisplayText(lineItem.ProductGuid) + "  " + dataDictionaryPreset
									 + ": "
									 + (lineItem.PresetAmount ?? 0.0).ToString("N", this.SiteManager.Site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME)));
							}
						}
					}
					else
					{
						summary.Add(prompt + " " + dataDictionaryCompartments);
					}
				}

				this.StationState = StationState.SUMMARY_PROMPT;
				defaultItem = -1;

				if (this.ProductsConfigured)
				{
					prompt = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(x => x.Get(this.SiteManager.Site.SiteGuid, "[LoadRack|UnLoading]"))
							  + " "
							  + FMChannelHelper.MakeCall<IDataDictionariesClass, string>(x => x.Get(this.SiteManager.Site.SiteGuid, "LoadRack|Summary"));
					defaultItem = summary.Count;
					summary.Add(FMChannelHelper.MakeCall<IDataDictionariesClass, string>(x => x.Get(this.SiteManager.Site.SiteGuid, "LoadRack|Accept")));
				}
				else
				{
					prompt = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(x => x.Get(this.SiteManager.Site.SiteGuid, "[LoadRack|UnLoading]"))
									+ " "
									+ FMChannelHelper.MakeCall<IDataDictionariesClass, string>(x => x.Get(this.SiteManager.Site.SiteGuid, "LoadRack|Summary"))
									+ " "
									+ FMChannelHelper.MakeCall<IDataDictionariesClass, string>(x => x.Get(this.SiteManager.Site.SiteGuid, "LoadRack|No Compartments"));
				}
			}

			DisplayMenuParameters parameters = new DisplayMenuParameters(prompt, (string[])summary.ToArray(typeof(string)), false, defaultItem, this.PROMPT_TIMEOUT);
			this.DisplayMenu(parameters);
		}

		private TransactionDO GetSupplyOrderByDocumentNumber(string documentNumber)
		{
			GetTransactionSR getTransactionSR = new GetTransactionSR
			{
				Security = this.Security,
				Request = GetTransactionRequest.SITE_TYPEID_ALIAS_DOCUMENTNUMBER,
				Site = this.SiteManager.Site.ID,
				TransTypeID = TransactionTypes.T18_SupplyOrder,
				Status = ((int)TransactionStatus.Scheduled).ToString(CultureInfo.InvariantCulture),
				DocumentNumber = documentNumber
			};

			GetTransactionDO getTransactionDO = FMChannelHelper.MakeCall<IGetTransactionProcessor, GetTransactionDO>(x => x.Process(getTransactionSR));

			if (getTransactionDO?.TransactionDataSet != null && getTransactionDO.TransactionDataSet.Tables.Count != 0 && getTransactionDO.TransactionDataSet.Tables[0].Rows.Count != 0)
			{
				foreach (DataRow row in getTransactionDO.TransactionDataSet.Tables[0].Rows)
				{
					string transId = row["TransID"] as string;
					if (string.IsNullOrEmpty(transId) == false)
					{
						return this.GetTransaction(transId);
					}
				}
			}

			return null;
		}

		protected virtual void ClearRecipes(bool clearAll)
		{
            // DynamicRecipes don't apply to the generic case
            this.RecipeInternalNumberMap = new Dictionary<int, ProductMapClass>();
            this.LastDownloadedRecipe = 0;
            return;
        }

        internal virtual void ClearSingleRecipe(int recipeNumber)
		{
			// Default to do nothing
		}

				/// <summary>
		/// Attempt to write a recipe down to the preset if that funciton is supported
		/// 
		/// If not supported just sets up the device recipe number to configured recipe map
		/// </summary>
		/// <param name="loadArmManager">
		/// Load Arm Manager for the load arm the recipe is on</param>
		/// <param name="recipeToArmMap">
		/// The recipe assignment to write to the preset
		/// </param>
		/// <returns>Device product number assigned to the recipe</returns>
		internal virtual int WriteSingleRecipe(LoadArmManagerClass loadArmManager, ProductMapClass recipeToArmMap)
		{
			// Default do nothing beyond map the configured recipe number to itself
			return loadArmManager.GetRecipeNumber(recipeToArmMap);
		}

        /// <summary>
		/// Tries to find the next available (unassigned) recipe
		/// The Accuload III/IV has 50 recipe assignments available
		/// </summary>
		/// <returns>
		/// The recipe number of the next available recipe.
		/// 
		/// 0 on error or no recipes available
		/// </returns>
		protected virtual int GetNextAvailableRecipeNumber()
		{
			return 0;
		}

		/// <summary>
		/// Returns whether the recipce in question belongs to this FuelsManager station
		/// Comes in to play with swing arms and split bays, where two stations in FuelsManager may
		/// address the same physical preset
		/// </summary>
		/// <param name="recipeNumber">Recipe number to check</param>
		/// <returns>true</returns>
		protected virtual bool RecipeBelongsToThisStation(int recipeNumber)
		{
			return true;
		}
	}

	public class CompartmentInfo
	{
		#region Constants and Fields

		public int CompartmentNumber;

		public Guid EquipmentGuid = Guid.Empty;

		public string EquipmentID = "";

		public bool Loaded = false;

		public double MaxFill;

		#endregion
	}

	public class ContaminationPromptStatus
	{
		#region Constants and Fields

		public bool? CompartmentsEmpty;

		public bool? CompartmentsPreviouslyLoaded;

		public bool? ContaminatePrompt;

		public string ContaminationPromptLoadRackText;

		#endregion

		#region Constructors and Destructors

		public ContaminationPromptStatus(string contaminationPromptLoadRackText)
		{
			this.ContaminationPromptLoadRackText = contaminationPromptLoadRackText;
		}
		#endregion
	}
}

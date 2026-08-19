// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProcessVariableClass.cs" company="Varec, Inc.">
//	Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//	Defines the PROCESS_VARIABLE_TYPE type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Data.SqlClient;
	using System.Globalization;
	using System.Linq;
	using System.Reflection;
	using System.Resources;
	using System.Runtime.InteropServices;
	using System.Runtime.Serialization;
	using System.Security;
	using System.Xml.Serialization;

	using Opc.Da;

	using Varec.CommonComponents.EngineeringUnitsLibrary;

	#region Public enumeration.
	/// <summary>
	/// The process variable type enumeration.
	/// </summary>
	public enum PROCESS_VARIABLE_TYPE
	{
		UNDEFINED_PV					= 0,
		LEVEL_PV						= 1,
		TEMPERATURE_PV					= 2,
		GROSS_VOLUME_PV					= 3,
		NET_VOLUME_PV					= 4,
		DENSITY_PV						= 5,
		STANDARD_DENSITY_PV				= 6,
		MASS_PV							= 7,
		SWING_ARM_STATUS_PV				= 8,
		CARDREADER_PV					= 9,
		RESET_CARDREADER_DATA_PV		= 10,
		KEYPAD_DATA_PV					= 11,
		RELEASE_KEYPAD_PV				= 12,
		DISPLAY_PV						= 13,
		PASSWORD_PV						= 14,
		PROMPT_TIMEOUT_PV				= 15,
		START_PERMISSIVE_PV				= 16,
		COMPLETION_PERMISSIVE_PV		= 17,
		GATE_CONTROL_PV					= 18,
		LOADARM_PV						= 19,
		SITE_PERMISSIVE_PV				= 20,
		SITE_ALARM_OUTPUT_PV			= 21,
		SITE_WATCHDOG_OUTPUT_PV			= 22,
		VRU_SETPOINT_PV					= 23,
		VRU_DEADBAND_PV					= 24,
		TRANSACTION_DONE_PV				= 25,
		TRANSACTION_IN_PROGRESS_PV		= 26,
		BATCH_DONE_PV					= 27,
		VCF_PV							= 28,
		LOAD_ARM_RELEASED_PV			= 29,
		AVAILABLE_GROSS_VOLUME_PV		= 30,
		REMAINING_GROSS_VOLUME_PV		= 31,
		FLOWING_PV						= 32,
		STATION_PV						= 33,
		WEIGHT_SCALE_PV					= 34,
		CLEAR_LIST_PV					= 35,
		DISPLAY_LIST_PV					= 36,
		SELECTED_ITEM_PV				= 37,
		SELECT_ITEM_PV					= 38,
		WRITE_ITEM_PV					= 39,
		BATCH_ABORTED_PV				= 40,
		POWER_FAIL_OCCURRED_PV			= 41,
		GET_KEY_PV						= 42,
		PRESETTING_IN_PROGRESS_PV		= 43,
		TANK_OPERATION_PV				= 44,
		VAPOR_PRESSURE_PV				= 45,
		AVAILABLE_NET_VOLUME_PV			= 46,
		REMAINING_NET_VOLUME_PV			= 47,
		AUTHORIZED_PV					= 48,
		BATCH_ENDED_PV					= 49,
		BATCH_IN_PROGRESS_PV			= 50,
		TRANSACTION_END_REQUESTED_PV	= 51,
		RECIPE_SELECTED_PV				= 52,
		PRESET_VOLUME_ENTERED_PV		= 53,
		BATCH_AUTHORIZED_PV				= 54,
		TRANSACTION_AUTHORIZED_PV		= 55,
		KEY_PRESSED_PV					= 56,
		PRIMARY_ALARM_PV				= 57,
		TANK_STATUS_PV					= 58,
		MANUAL_PV						= 59,
		LOAD_ARM_STATE_PV				= 60,
		INPUT_DONE_PV					= 61,
		RECIPE_PV						= 62,
		INPUT_PERMISSIVE_PV				= 63,
		OUTPUT_PERMISSIVE_PV			= 64,
		PERMISSIVE_DELAY_PV				= 65,
		COMPONENT_METER_FLOW_TOTAL_PV	= 66,
		BLEND_PERCENTAGE_PV				= 67,
		ADDITIVE_METER_FLOW_TOTAL_PV	= 68,
		KEYPAD_DATA_PENDING_PV			= 69,
		ALARM_PV						= 70,
		TERMINATION_KEY					= 71,
		SCULLY_PV							= 72,
		WATER_LEVEL_PV = 73,
		WATER_VOLUME_PV = 74,
		MAX_PV = 75
	};

	/// <summary>
	/// The unit type enumeration.
	/// </summary>
	public enum UNIT_TYPE
	{
		UNDEFINED_UNIT							= 0,
		TANK_UNIT								= 1,
		LOADARM_UNIT							= 2,
		STATION_UNIT							= 3,
		SITE_UNIT								= 4,
		COMPONENT_INPUT_PERMISSIVE				= 5,
		COMPONENT_OUTPUT_PERMISSIVE				= 6,
		ADDITIVE_INPUT_PERMISSIVE				= 7,
		ADDITIVE_OUTPUT_PERMISSIVE				= 8,
		RECIPE_INPUT_PERMISSIVE					= 9,
		RECIPE_OUTPUT_PERMISSIVE				= 10,
		LOADARM_INPUT_PERMISSIVE				= 11,
		LOADARM_OUTPUT_PERMISSIVE				= 12,
		NOADDITIVE_INPUT_PERMISSIVE				= 13,
		NOADDITIVE_OUTPUT_PERMISSIVE			= 14,
		STATION_INPUT_PERMISSIVE				= 15,
		STATION_OUTPUT_PERMISSIVE				= 16,
		PRODUCT_MAP_PRESET_EXTERNAL_COMPONENT	= 17,
		EXTERNAL_COMPONENT_INPUT_PERMISSIVE		= 18,
		EXTERNAL_COMPONENT_OUTPUT_PERMISSIVE	= 19,
		EQUIPMENT_UNIT							= 20,
		PRODUCT_MAP_PRESET_INJECTOR				= 21,
		FLOW_CONTROLLED_ADDITIVE_INPUT_PERMISSIVE = 22,
		FLOW_CONTROLLED_ADDITIVE_OUTPUT_PERMISSIVE = 23,
		PRODUCT_MAP_OFFLOAD_EXTERNAL_METER = 24,
		EXTERNAL_METER_INPUT_PERMISSIVE = 25,
		EXTERNAL_METER_OUTPUT_PERMISSIVE = 26,
		MAX_UNIT = 27
	};

	/// <summary>
	/// The data type enumeration.
	/// </summary>
	public enum DATA_TYPE
	{
		CONFIG	= 0,
		DYNAMIC = 1,
		AUTOMIC = 2,
		SYNCCONFIG = 3
	};
	#endregion

	#region Permissives Class
	/// <summary>
	/// Summary description for PermissivesClass.
	/// </summary>
	[Serializable]
	[DataContract]
	public class PermissivesClass
	{
		[DataMember] private bool enabled;
		[DataMember] private UNIT_TYPE inputUnitType;
		[DataMember] private UNIT_TYPE outputUnitType;
		[DataMember] private ProcessVariableCollectionClass outputs;
		[DataMember] private ProcessVariableCollectionClass inputs;

		/// <summary>
		/// Initializes a new instance of the <see cref="PermissivesClass"/> class.
		/// </summary>
		public PermissivesClass()
		{
			this.outputs		= new ProcessVariableCollectionClass();
			this.inputs			= new ProcessVariableCollectionClass();
			this.inputUnitType	= UNIT_TYPE.MAX_UNIT;
			this.outputUnitType = UNIT_TYPE.MAX_UNIT;
		}

		/// <summary>
		/// Gets or sets the input unit type.
		/// </summary>
		public UNIT_TYPE InputUnitType
		{
			get { return this.inputUnitType; }
			set { this.inputUnitType = value; }
		}

		/// <summary>
		/// Gets or sets the output unit type.
		/// </summary>
		public UNIT_TYPE OutputUnitType
		{
			get { return this.outputUnitType; }
			set { this.outputUnitType = value; }
		}

		/// <summary>
		/// Gets or sets the outputs.
		/// </summary>
		public ProcessVariableCollectionClass Outputs
		{
			get
			{
				return this.outputs;
			}

			set
			{
				this.outputs = value;
				foreach (ProcessVariableClass processVariable in this.outputs)
				{
					processVariable.Parent = this;
				}
			}
		}

		/// <summary>
		/// Gets or sets the inputs.
		/// </summary>
		public ProcessVariableCollectionClass Inputs
		{
			get
			{
				return this.inputs;
			}

			set
			{
				this.inputs = value;
				foreach (ProcessVariableClass processVariable in this.inputs)
				{
					processVariable.Parent = this;
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether enabled.
		/// </summary>
		public bool Enabled
		{
			get
			{
				return this.enabled;
			}

			set
			{
				this.enabled = value;
				this.Update();
			}
		}

		/// <summary>
		/// Gets a value indicating whether permitted.
		/// </summary>
		public bool Permitted
		{
			get
			{
				foreach (ProcessVariableClass permissive in this.inputs)
				{
					if (!permissive.IsQualityGood)
					{
						return false;
					}

					if (!(permissive.ServerValue is bool))
					{
						return false;
					}

					if (!((bool)permissive.ServerValue))
					{
						return false;
					}
				}

				return true;
			}
		}

		/// <summary>
		/// The update.
		/// </summary>
		public void Update()
		{
			bool result = this.Permitted && this.enabled;

			foreach (ProcessVariableClass permissive in this.outputs)
			{
				permissive.SetValue(result, 0);
			}
		}

		/// <summary>
		/// The modify process variable message.
		/// </summary>
		/// <param name="message">
		/// The message.
		/// </param>
		public void ModifyProcessVariableMessage(ApplicationStringClass message)
		{
			foreach (ProcessVariableClass permissive in this.inputs)
			{
				if (permissive.MessageApplicationStringGuid == message.IdentityGuid)
				{
					permissive.MessageID = message.ID;
				}
			}
		}

		/// <summary>
		/// The purge process variable message.
		/// </summary>
		/// <param name="identityGuid">
		/// The identity GUID.
		/// </param>
		public void PurgeProcessVariableMessage(Guid identityGuid)
		{
			foreach (ProcessVariableClass permissive in this.inputs)
			{
				if (permissive.MessageApplicationStringGuid == identityGuid)
				{
					permissive.MessageApplicationStringGuid = Guid.Empty;
					permissive.MessageID = string.Empty;
				}
			}
		}

		/// <summary>
		/// The load.
		/// </summary>
		/// <param name="permissiveObject">
		/// The permissive object.
		/// </param>
		public void Load(object permissiveObject)
		{
			if ( permissiveObject is PermissivesClass )
			{
				PermissivesClass permissives = (PermissivesClass) permissiveObject;

				this.enabled		= permissives.Enabled;
				this.inputUnitType	= permissives.OutputUnitType;
				this.outputUnitType = permissives.OutputUnitType;

				this.outputs.Clear();

				foreach (ProcessVariableClass existingProcessVariable in permissives.Outputs)
				{
					var newProcessVariable = new ProcessVariableClass();
					newProcessVariable.Load(existingProcessVariable);
					newProcessVariable.Parent = this;
					this.outputs.Add(newProcessVariable);
				}

				this.inputs.Clear();

				foreach (ProcessVariableClass existingProcessVariable in permissives.Inputs)
				{
					var newProcessVariable = new ProcessVariableClass();
					newProcessVariable.Load(existingProcessVariable);
					newProcessVariable.Parent = this;
					this.inputs.Add(newProcessVariable);
				}
			}
		}
	}
	#endregion

	#region ProcessVariableComparer
	/// <summary>
	/// The process variable comparer.
	/// </summary>
	public class ProcessVariableComparer : IComparer
	{
		/// <summary>
		/// The compare.
		/// </summary>
		/// <param name="x">
		/// The x.
		/// </param>
		/// <param name="y">
		/// The y.
		/// </param>
		/// <returns>
		/// The <see cref="int"/>.
		/// </returns>
		/// <exception cref="Exception">
		/// </exception>
		int IComparer.Compare(object x, object y)
		{
			var pvx = x as ProcessVariableClass;
			var pvy = y as ProcessVariableClass;

			if (pvx == null || pvy == null)
			{
				throw new Exception("ProcessVariableClass : Compare Invalid Object Type");
			}

			return pvx.OPCConnectionGuid.CompareTo(pvy.OPCConnectionGuid);
		}
	}
	#endregion

	#region Process Variable Collection Class.
	/// <summary>
	/// The process variable collection class.
	/// </summary>
	[CollectionDataContract]
	[Serializable]
	[KnownType(typeof(ProcessVariableClass))]
	public class ProcessVariableCollectionClass : CollectionBase
	{
		/// <summary>
		/// The this.
		/// </summary>
		/// <param name="index">
		/// The index.
		/// </param>
		/// <returns>
		/// The <see cref="ProcessVariableClass"/>.
		/// </returns>
		public ProcessVariableClass this[int index]
		{
			get { return (ProcessVariableClass)this.List[index]; }
			set { this.List[index] = value; }
		}

		/// <summary>
		/// The this.
		/// </summary>
		/// <param name="type">
		/// The type.
		/// </param>
		/// <returns>
		/// The <see cref="ProcessVariableClass"/>.
		/// </returns>
		public ProcessVariableClass this[PROCESS_VARIABLE_TYPE type]
		{
			get
			{
				return this.List.Cast<ProcessVariableClass>().FirstOrDefault(item => item.ProcessVariableType == type);
			}
		}

		/// <summary>
		/// Use this only if you know your list has unique GUID value.
		/// </summary>
		/// <param name="processVariableGuid">
		/// The process variable GUID.
		/// </param>
		/// <returns>
		/// The <see cref="ProcessVariableClass"/>.
		/// </returns>
		public ProcessVariableClass this[Guid processVariableGuid]
		{
			get
			{
				return this.List.Cast<ProcessVariableClass>().FirstOrDefault(item => item.IdentityGuid == processVariableGuid);
			}
		}

		/// <summary>
		/// The add.
		/// </summary>
		/// <param name="processVariable">
		/// The process variable.
		/// </param>
		public void Add(ProcessVariableClass processVariable)
		{
			this.List.Add(processVariable);
		}

		/// <summary>
		/// The remove.
		/// </summary>
		/// <param name="index">
		/// The index.
		/// </param>
		/// <exception cref="ArgumentOutOfRangeException">Out of range index.
		/// </exception>
		public void Remove(int index)
		{
			if (index > this.Count - 1 || index < 0)
			{
				throw new ArgumentOutOfRangeException("index", index, "Invalid Index");
			}

			this.List.RemoveAt(index);
		}

		/// <summary>
		/// The remove.
		/// </summary>
		/// <param name="processVariable">
		/// The process variable.
		/// </param>
		public void Remove(ProcessVariableClass processVariable)
		{
			int index = 0;

			foreach (ProcessVariableClass item in this.List)
			{
				if (item.IdentityGuid == processVariable.IdentityGuid
					&& item.ProcessVariableType == processVariable.ProcessVariableType
					&& item.InstanceNumber == processVariable.InstanceNumber)
				{
					this.List.RemoveAt(index);
					return;
				}

				index++;
			}
		}
	}
	#endregion

	#region Process Variable Class.
	/// <summary>
	/// The process variable class.
	/// </summary>
	[Serializable]
	[SecuritySafeCritical]
	[DataContract]
	[KnownType(typeof(DateTimeOffset))]
	[KnownType(typeof(TimeSpan))]
	[KnownType(typeof(CodedVariables.RoofTypeEnum))]
	[KnownType(typeof(PointCommandStatusListReference))]
	[KnownType(typeof(DeviceAlarmMapReference))]
	[KnownType(typeof(PermissivesClass))]
	[KnownType(typeof(DBNull))]
	[KnownType(typeof(CodedVariables.TankStatuses))]
	[KnownType(typeof(CodedVariables.TankCommands))]
	[KnownType(typeof(CodedVariables.TransferModes))]
	[KnownType(typeof(CodedVariables.TankTransferMode))]
	[KnownType(typeof(CodedVariables.VolumeTransferMode))]
	[KnownType(typeof(CodedVariables.TransferStatuses))]
	[KnownType(typeof(CodedVariables.TankOperationalMode))]
	[KnownType(typeof(CodedVariables.MovementCommand))]
	[KnownType(typeof(CodedVariables.MovementStatus))]
	[KnownType(typeof(CodedVariables.StrapTableSelect))]
	[KnownType(typeof(CodedVariables.Reset))]
	[KnownType(typeof(CodedVariables.NodeTransferMode))]
	[KnownType(typeof(CodedVariables.NodeTransferStatus))]
	[KnownType(typeof(VolumeTransferModuleSettings))]
	[KnownType(typeof(MovementModuleSettings))]
	[KnownType(typeof(MovementData))]
	[KnownType(typeof(MovementNodeData))]
	[KnownType(typeof(CodedVariables.TransferDirection))]
	[KnownType(typeof(LeakDetectionSettings))]
	[KnownType(typeof(CodedVariables.MovementType))]
	public class ProcessVariableClass : BaseDataObject, IAlarmAndEventDiscovery
	{
		#region constants
		const string PARAM_NAME_PROCESSVARIABLEGUID = "@ProcessVariableGuid";
		const string PARAM_NAME_PROCESSVARIABLEGUID_WHERE = "@WhereProcessVariableGuid";
		const SqlDbType PARAM_TYPE_PROCESSVARIABLEGUID = SqlDbType.UniqueIdentifier;
		const string PARAM_NAME_PROCESSVARIABLETYPE = "@ProcessVariableType";
		const SqlDbType PARAM_TYPE_PROCESSVARIABLETYPE = SqlDbType.Int;
		const string PARAM_NAME_INSTANCENUMBER = "@InstanceNumber";
		const SqlDbType PARAM_TYPE_INSTANCENUMBER = SqlDbType.Int;
		const string PARAM_NAME_UNITGUID = "@UnitGuid";
		const SqlDbType PARAM_TYPE_UNITGUID = SqlDbType.UniqueIdentifier;
		const string PARAM_NAME_OPCCONNECTIONGUID = "@OPCConnectionGuid";
		const SqlDbType PARAM_TYPE_OPCCONNECTIONGUID = SqlDbType.UniqueIdentifier;
		const string PARAM_NAME_OPCITEMID = "@OPCItemID";
		const SqlDbType PARAM_TYPE_OPCITEMID = SqlDbType.NVarChar;
		const int PARAM_SIZE_OPCITEMID = 255;
		const string PARAM_NAME_DATATYPE = "@DataType";
		const SqlDbType PARAM_TYPE_DATATYPE = SqlDbType.Int;
		const string PARAM_NAME_SERVERENGINEERINGUNITSINDEX = "@ServerEngineeringUnitsIndex";
		const SqlDbType PARAM_TYPE_SERVERENGINEERINGUNITSINDEX = SqlDbType.Int;
		const string PARAM_NAME_QUALITY = "@Quality";
		const SqlDbType PARAM_TYPE_QUALITY = SqlDbType.SmallInt;
		const string PARAM_NAME_SIVALUE = "@SIValue";
		const SqlDbType PARAM_TYPE_SIVALUE = SqlDbType.Variant;
		const string PARAM_NAME_DATETIMESTAMP = "@DateTimeStamp";
		const SqlDbType PARAM_TYPE_DATETIMESTAMP = SqlDbType.DateTimeOffset;
		const string PARAM_NAME_MAXIMUM = "@Maximum";
		const SqlDbType PARAM_TYPE_MAXIMUM = SqlDbType.Variant;
		const string PARAM_NAME_MINIMUM = "@Minimum";
		const SqlDbType PARAM_TYPE_MINIMUM = SqlDbType.Variant;
		const string PARAM_NAME_DATATYPEENABLED = "@DataTypeEnabled";
		const SqlDbType PARAM_TYPE_DATATYPEENABLED = SqlDbType.Bit;
		const string PARAM_NAME_INPUT = "@Input";
		const SqlDbType PARAM_TYPE_INPUT = SqlDbType.Bit;
		const string PARAM_NAME_INPUTENABLED = "@InputEnabled";
		const SqlDbType PARAM_TYPE_INPUTENABLED = SqlDbType.Bit;
		const string PARAM_NAME_MESSAGEAPPLICATIONSTRINGGUID = "@MessageApplicationStringGuid";
		const SqlDbType PARAM_TYPE_MESSAGEAPPLICATIONSTRINGGUID = SqlDbType.UniqueIdentifier;
		const string PARAM_NAME_CREATEDDATE = "@CreatedDate";
		const SqlDbType PARAM_TYPE_CREATEDDATE = SqlDbType.DateTimeOffset;
		const string PARAM_NAME_CREATEDBY = "@CreatedBy";
		const SqlDbType PARAM_TYPE_CREATEDBY = SqlDbType.NVarChar;
		const int PARAM_SIZE_CREATEDBY = 100;
		const string PARAM_NAME_UPDATEDDATE = "@UpdatedDate";
		const SqlDbType PARAM_TYPE_UPDATEDDATE = SqlDbType.DateTimeOffset;
		const string PARAM_NAME_UPDATEDBY = "@UpdatedBy";
		const SqlDbType PARAM_TYPE_UPDATEDBY = SqlDbType.NVarChar;
		const int PARAM_SIZE_UPDATEDBY = 100;
		#endregion constants

		#region Data Members
		[DataMember] public PROCESS_VARIABLE_TYPE ProcessVariableType;

		[DataMember] public int InstanceNumber;

		[XmlIgnore]
		[DataMember] public Guid UnitGuid;

		[DataMember] public UNIT_TYPE UnitType;

		[DataMember]
		[XmlIgnore]
		public Guid OPCConnectionGuid;

		[DataMember]
		[XmlIgnore]
		public string _OPCItemID;

		[DataMember]
		[XmlIgnore]
		public VarEnum _DataType;

		[DataMember]
		public EngineeringUnit ServerUnits;

		[DataMember]
		public short OPCQuality;

		[DataMember]
		private Object siValue;

		[DataMember]
		public DateTimeOffset DateTimeStamp;

		[DataMember]
		[XmlIgnore]
		public Object siMaximum;

		[DataMember]
		[XmlIgnore]
		public Object siMinimum;

		[DataMember]
		public bool DataTypeEnabled;

		[DataMember]
		[XmlIgnore]
		public bool input;

		[DataMember]
		public bool InputEnabled;

		[DataMember]
		[XmlIgnore]
		private Object serverValue;

		[DataMember]
		[XmlIgnore]
		public double ReferenceTemperature;

		[DataMember]
		[XmlIgnore]
		public string _URL;

		[DataMember]
		[XmlIgnore]
		public string _ProgID;

		[DataMember]
		[XmlIgnore]
		public Guid MessageApplicationStringGuid;

		[DataMember]
		[XmlIgnore]
		public PermissivesClass Parent = null;

		[DataMember]
		[XmlIgnore]
		public bool DataChanged = false;

		[DataMember]
		[XmlIgnore]
		public bool OutputFailed = false;

		[DataMember]
		[XmlIgnore]
		public string MessageID;

		public static string PermissiveLostKey = "Permissive Lost";
		public static AlarmAndEventDescriptorClass PermissiveLostAlarmDescriptor = new AlarmAndEventDescriptorClass(true, LoadRackKey, PermissiveLostKey);

		static string PermissiveRestoredKey = "Permissive Restored";
		public static AlarmAndEventDescriptorClass PermissiveRestoredEventDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, PermissiveRestoredKey);
		#endregion

		/// <summary>
		/// Initializes a new instance of the <see cref="ProcessVariableClass"/> class.
		/// </summary>
		public ProcessVariableClass( )
		{
			this.Init();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ProcessVariableClass"/> class.
		/// </summary>
		/// <param name="processVariableType">
		/// The process variable type.
		/// </param>
		/// <param name="unitType">
		/// The unit type.
		/// </param>
		/// <param name="dataType">
		/// The data type.
		/// </param>
		/// <param name="Input">
		/// The input.
		/// </param>
		/// <param name="opcItemId">
		/// The opc item ID.
		/// </param>
		/// <param name="url">
		/// The URL.
		/// </param>
		/// <param name="progId">
		/// The program ID.
		/// </param>
		public ProcessVariableClass(
			PROCESS_VARIABLE_TYPE processVariableType,
			UNIT_TYPE unitType,
			VarEnum dataType,
			bool Input,
			string opcItemId,
			string url,
			string progId)
		{
			this.Init();
			this.ProcessVariableType = processVariableType;
			this.UnitType = unitType;
			this.DataType = dataType;
			this.Input = Input;
			this.OPCItemID = opcItemId;
			this.URL = url;
			this.ProgID = progId;
		}

		/// <summary>
		/// Gets or sets the ID.
		/// </summary>
		public override string ID
		{
			get
			{
				return this.ProcessVariableType.ToString();
			}

			set
			{
				this._ID = value;
			}
		}

		/// <summary>
		/// Gets or sets the opc item id.
		/// </summary>
		[XmlIgnore]
		public string OPCItemID
		{
			get
			{
				return this._OPCItemID;
			}

			set
			{
				this.SetString("Item ID", 256, value, ref this._OPCItemID);
			}
		}

		/// <summary>
		/// Gets or sets the url.
		/// </summary>
		[XmlIgnore]
		public string URL
		{
			get
			{
				return this._URL;
			}

			set
			{
				this.SetString("URL", 100, value, ref this._URL);
			}
		}

		/// <summary>
		/// Gets or sets the program ID.
		/// </summary>
		[XmlIgnore]
		public string ProgID
		{
			get
			{
				return this._ProgID;
			}

			set
			{
				this.SetString("ProgID", 50, value, ref this._ProgID);
			}
		}

		/// <summary>
		/// Gets the entity type.
		/// </summary>
		[XmlIgnore]
		public override ENTITY_TYPE EntityType
		{
			get
			{
				return ENTITY_TYPE.PROCESS_VARIABLE;
			}
		}

		/// <summary>
		/// Gets the parent entity type.
		/// </summary>
		[XmlIgnore]
		public override ENTITY_TYPE ParentEntityType
		{
			get
			{
				return ENTITY_TYPE.TANK;
			}
		}

		/// <summary>
		/// Gets or sets the data type.
		/// </summary>
		public VarEnum DataType
		{
			get
			{
				return this._DataType;
			}
			set
			{
				if ( this._DataType == value )
				{
					return;
				}

				this._DataType = value;
				switch ( this._DataType )
				{
					case VarEnum.VT_I1:
						this.siMinimum = sbyte.MinValue;
						this.siMaximum = sbyte.MaxValue;
						break;

					case VarEnum.VT_I2:
						this.siMinimum = short.MinValue;
						this.siMaximum = short.MaxValue;
						break;

					case VarEnum.VT_I4:
					case VarEnum.VT_INT:
						this.siMinimum = int.MinValue;
						this.siMaximum = int.MaxValue;
						break;

					case VarEnum.VT_I8:
						this.siMinimum = long.MinValue;
						this.siMaximum = long.MaxValue;
						break;

					case VarEnum.VT_UI1:
						this.siMinimum = byte.MinValue;
						this.siMaximum = byte.MaxValue;
						break;

					case VarEnum.VT_UI2:
						this.siMinimum = ushort.MinValue;
						this.siMaximum = ushort.MaxValue;
						break;

					case VarEnum.VT_UI4:
					case VarEnum.VT_UINT:
						this.siMinimum = uint.MinValue;
						this.siMaximum = uint.MaxValue;
						break;

					case VarEnum.VT_UI8:
						this.siMinimum = ulong.MinValue;
						this.siMaximum = ulong.MaxValue;
						break;

					case VarEnum.VT_R4:
						this.siMinimum = float.MaxValue;
						this.siMaximum = float.MinValue;
						break;

					case VarEnum.VT_R8:
						this.siMinimum = double.MinValue;
						this.siMaximum = double.MaxValue;
						break;

					case VarEnum.VT_DATE:
						this.siMinimum = DateTime.MinValue;
						this.siMaximum = DateTime.MaxValue;
						break;
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether input.
		/// </summary>
		public bool Input
		{
			get
			{
				return this.input;
			}

			set
			{
				this.input = value;

				if (!this.input && !(this.siValue is DBNull))
				{
					this.OPCQuality = Quality.Good.GetCode( );
				}
			}
		}

		/// <summary>
		/// Gets or sets the SI value.
		/// </summary>
		public object SIValue
		{
			get
			{
				return this.siValue;
			}

			[SecuritySafeCritical]
			set
			{
				this.siValue = value;

				if ( this.siValue is double
				&& this.UnitsType != EngineeringUnitType.FmuNodim
				&& this.ServerUnits != EngineeringUnit.FmSiteUnits
				&& this.DataType == VarEnum.VT_R8 )
				{
					double Result = 0.0;

					EngineeringUnits.Convert((double)this.siValue, this.SIUnits,
														ref Result, this.ServerUnits, this.ReferenceTemperature);

					this.serverValue = Result;
				}
				else
				{
					this.serverValue = this.siValue;
				}
			}
		}

		/// <summary>
		/// Gets or sets the server value.
		/// </summary>
		[XmlIgnore]
		public object ServerValue
		{
			get
			{
				return this.serverValue;
			}

			[SecuritySafeCritical]
			set
			{
				this.serverValue = value;

				if ( this.serverValue is double
				&& this.UnitsType != EngineeringUnitType.FmuNodim
				&& this.DataType == VarEnum.VT_R8 )
				{
					double result = 0.0;

					EngineeringUnits.Convert((double)this.serverValue, this.ServerUnits,
														ref result, this.SIUnits, this.ReferenceTemperature);

					this.siValue = result;
				}
				else
				{
					this.siValue = this.serverValue;
				}
			}
		}

		/// <summary>
		/// Gets a value indicating whether units enabled.
		/// </summary>
		public bool UnitsEnabled
		{
			get
			{
				switch ( this.ProcessVariableType )
				{
					case PROCESS_VARIABLE_TYPE.LEVEL_PV:
					case PROCESS_VARIABLE_TYPE.TEMPERATURE_PV:
					case PROCESS_VARIABLE_TYPE.GROSS_VOLUME_PV:
					case PROCESS_VARIABLE_TYPE.NET_VOLUME_PV:
					case PROCESS_VARIABLE_TYPE.DENSITY_PV:
					case PROCESS_VARIABLE_TYPE.STANDARD_DENSITY_PV:
					case PROCESS_VARIABLE_TYPE.MASS_PV:
					case PROCESS_VARIABLE_TYPE.AVAILABLE_GROSS_VOLUME_PV:
					case PROCESS_VARIABLE_TYPE.REMAINING_GROSS_VOLUME_PV:
					case PROCESS_VARIABLE_TYPE.VAPOR_PRESSURE_PV:
					case PROCESS_VARIABLE_TYPE.AVAILABLE_NET_VOLUME_PV:
					case PROCESS_VARIABLE_TYPE.REMAINING_NET_VOLUME_PV:
					case PROCESS_VARIABLE_TYPE.COMPONENT_METER_FLOW_TOTAL_PV:
					case PROCESS_VARIABLE_TYPE.ADDITIVE_METER_FLOW_TOTAL_PV:
					case PROCESS_VARIABLE_TYPE.WATER_LEVEL_PV:
					case PROCESS_VARIABLE_TYPE.WATER_VOLUME_PV:
						return true;
					default:
						return false;
				}
			}
		}


		/// <summary>
		/// Gets the site variable type.
		/// </summary>
		public SITE_VARIABLE_TYPE SiteVariableType
		{
			get
			{
				switch ( this.ProcessVariableType )
				{
					case PROCESS_VARIABLE_TYPE.LEVEL_PV:
					case PROCESS_VARIABLE_TYPE.WATER_LEVEL_PV:
						return SITE_VARIABLE_TYPE.LENGTH;

					case PROCESS_VARIABLE_TYPE.TEMPERATURE_PV:
						return SITE_VARIABLE_TYPE.TEMPERATURE;

					case PROCESS_VARIABLE_TYPE.GROSS_VOLUME_PV:
					case PROCESS_VARIABLE_TYPE.NET_VOLUME_PV:
					case PROCESS_VARIABLE_TYPE.AVAILABLE_GROSS_VOLUME_PV:
					case PROCESS_VARIABLE_TYPE.REMAINING_GROSS_VOLUME_PV:
					case PROCESS_VARIABLE_TYPE.AVAILABLE_NET_VOLUME_PV:
					case PROCESS_VARIABLE_TYPE.REMAINING_NET_VOLUME_PV:
					case PROCESS_VARIABLE_TYPE.COMPONENT_METER_FLOW_TOTAL_PV:
					case PROCESS_VARIABLE_TYPE.WATER_VOLUME_PV:
						return SITE_VARIABLE_TYPE.VOLUME;

					case PROCESS_VARIABLE_TYPE.DENSITY_PV:
					case PROCESS_VARIABLE_TYPE.STANDARD_DENSITY_PV:
						return SITE_VARIABLE_TYPE.DENSITY;

					case PROCESS_VARIABLE_TYPE.MASS_PV:
						return SITE_VARIABLE_TYPE.MASS;

					case PROCESS_VARIABLE_TYPE.VRU_DEADBAND_PV:
					case PROCESS_VARIABLE_TYPE.VRU_SETPOINT_PV:
						return SITE_VARIABLE_TYPE.FLOW;

					case PROCESS_VARIABLE_TYPE.VAPOR_PRESSURE_PV:
						return SITE_VARIABLE_TYPE.PRESSURE;

					case PROCESS_VARIABLE_TYPE.VCF_PV:
						return SITE_VARIABLE_TYPE.VCF;

					case PROCESS_VARIABLE_TYPE.ADDITIVE_METER_FLOW_TOTAL_PV:
						return SITE_VARIABLE_TYPE.ADDITIVE_VOLUME;

					default:
						return SITE_VARIABLE_TYPE.DEFAULT;
				}
			}
		}

		/// <summary>
		/// Gets the units type.
		/// </summary>
		public EngineeringUnitType UnitsType
		{
			get
			{
				switch ( this.ProcessVariableType )
				{
					case PROCESS_VARIABLE_TYPE.LEVEL_PV:
					case PROCESS_VARIABLE_TYPE.WATER_LEVEL_PV:
						return EngineeringUnitType.FmuLength;

					case PROCESS_VARIABLE_TYPE.TEMPERATURE_PV:
						return EngineeringUnitType.FmuTemp;

					case PROCESS_VARIABLE_TYPE.GROSS_VOLUME_PV:
					case PROCESS_VARIABLE_TYPE.NET_VOLUME_PV:
					case PROCESS_VARIABLE_TYPE.AVAILABLE_GROSS_VOLUME_PV:
					case PROCESS_VARIABLE_TYPE.REMAINING_GROSS_VOLUME_PV:
					case PROCESS_VARIABLE_TYPE.AVAILABLE_NET_VOLUME_PV:
					case PROCESS_VARIABLE_TYPE.REMAINING_NET_VOLUME_PV:
					case PROCESS_VARIABLE_TYPE.COMPONENT_METER_FLOW_TOTAL_PV:
					case PROCESS_VARIABLE_TYPE.ADDITIVE_METER_FLOW_TOTAL_PV:
					case PROCESS_VARIABLE_TYPE.WATER_VOLUME_PV:
						return EngineeringUnitType.FmuVolume;

					case PROCESS_VARIABLE_TYPE.DENSITY_PV:
					case PROCESS_VARIABLE_TYPE.STANDARD_DENSITY_PV:
						return EngineeringUnitType.FmuDensity;

					case PROCESS_VARIABLE_TYPE.MASS_PV:
						return EngineeringUnitType.FmuMass;

					case PROCESS_VARIABLE_TYPE.VAPOR_PRESSURE_PV:
						return EngineeringUnitType.FmuPressure;

					default:
						return EngineeringUnitType.FmuNodim;
				}
			}
		}

		/// <summary>
		/// Gets the SI units.
		/// </summary>
		EngineeringUnit SIUnits
		{
			get
			{
				switch ( this.UnitsType )
				{
					case EngineeringUnitType.FmuLength:
						return EngineeringUnit.FmlMeter;
					case EngineeringUnitType.FmuTemp:
						return EngineeringUnit.FmtDegC;
					case EngineeringUnitType.FmuVolume:
						return EngineeringUnit.FmvMeter3;
					case EngineeringUnitType.FmuDensity:
						return EngineeringUnit.FmdKgM3;
					case EngineeringUnitType.FmuMass:
						return EngineeringUnit.FmmKg;
					case EngineeringUnitType.FmuArea:
						return EngineeringUnit.FmaMeter2;
					case EngineeringUnitType.FmuPressure:
						return EngineeringUnit.FmpPa;
					case EngineeringUnitType.FmuVolflow:
						return EngineeringUnit.FmvfM3Sec;
					case EngineeringUnitType.FmuMassflow:
						return EngineeringUnit.FmmfKgSec;
					case EngineeringUnitType.FmuVelocity:
						return EngineeringUnit.FmvrMSec;
					default:
						return EngineeringUnit.FmduPCent;
				}
			}
		}

		#region Alarm and event descriptors
		/// <summary>
		/// Gets the alarm and events.
		/// </summary>
		AlarmAndEventDescriptorClass[] IAlarmAndEventDiscovery.AlarmAndEvents
		{
			get
			{
				AlarmAndEventDescriptorClass[ ] descriptors = { PermissiveLostAlarmDescriptor, PermissiveRestoredEventDescriptor };
				return descriptors;
			}
		}
		#endregion

		/// <summary>
		/// The select clause.
		/// </summary>
		/// <param name="unitType">
		/// The unit type.
		/// </param>
		/// <returns>
		/// The <see cref="string"/>.
		/// </returns>
		protected string SelectClause(UNIT_TYPE unitType)
		{
			return "SELECT *," +
					"(SELECT URL FROM tblOPCConnections WITH (NOLOCK) WHERE tblOPCConnections.OPCConnectionGuid = " + GetTableName(unitType) + ".OPCConnectionGuid) AS URL," +
					"(SELECT ProgID FROM tblOPCConnections WITH (NOLOCK) WHERE tblOPCConnections.OPCConnectionGuid = " + GetTableName( unitType ) + ".OPCConnectionGuid) AS ProgID," +
					"(SELECT ID FROM tblApplicationString WITH (NOLOCK) WHERE tblApplicationString.ApplicationStringGuid = " + GetTableName( unitType ) + ".MessageApplicationStringGuid) AS MessageID, " +
					"(SELECT CodeType FROM lookup.tblVariantType WITH (NOLOCK) WHERE lookup.tblVariantType.VariantTypeIndex = " + GetTableName( unitType ) + ".LookupSIValueVariantTypeIndex) SIValueType, " +
					"(SELECT CodeType FROM lookup.tblVariantType WITH (NOLOCK) WHERE lookup.tblVariantType.VariantTypeIndex = " + GetTableName( unitType ) + ".LookupMaximumVariantTypeIndex) MaximumType, " +
					"(SELECT CodeType FROM lookup.tblVariantType WITH (NOLOCK) WHERE lookup.tblVariantType.VariantTypeIndex = " + GetTableName( unitType ) + ".LookupMinimumVariantTypeIndex) MinimumType ";
		}


		/// <summary>
		/// The get table name.
		/// </summary>
		/// <param name="unitType">
		/// The unit type.
		/// </param>
		/// <returns>
		/// The <see cref="string"/>.
		/// </returns>
		public static string GetTableName(UNIT_TYPE unitType)
		{
			switch (unitType)
			{
				case UNIT_TYPE.UNDEFINED_UNIT:
					return "Unknown";
				case UNIT_TYPE.TANK_UNIT:
					return "tblProcessVariableTank";
				case UNIT_TYPE.LOADARM_UNIT:
					return "tblProcessVariableLoadArm";
				case UNIT_TYPE.STATION_UNIT:
					return "tblProcessVariableStation";
				case UNIT_TYPE.SITE_UNIT:
					return "tblProcessVariableSite";
				case UNIT_TYPE.COMPONENT_INPUT_PERMISSIVE:
					return "tblProcessVariableComponentInputPermissive";
				case UNIT_TYPE.COMPONENT_OUTPUT_PERMISSIVE:
					return "tblProcessVariableComponentOutputPermissive";
				case UNIT_TYPE.ADDITIVE_INPUT_PERMISSIVE:
					return "tblProcessVariableAdditiveInputPermissive";
				case UNIT_TYPE.ADDITIVE_OUTPUT_PERMISSIVE:
					return "tblProcessVariableAdditiveOutputPermissive";
				case UNIT_TYPE.RECIPE_INPUT_PERMISSIVE:
					return "tblProcessVariableRecipeInputPermissive";
				case UNIT_TYPE.RECIPE_OUTPUT_PERMISSIVE:
					return "tblProcessVariableRecipeOutputPermissive";
				case UNIT_TYPE.LOADARM_INPUT_PERMISSIVE:
					return "tblProcessVariableLoadArmInputPermissive";
				case UNIT_TYPE.LOADARM_OUTPUT_PERMISSIVE:
					return "tblProcessVariableLoadArmOutputPermissive";
				case UNIT_TYPE.NOADDITIVE_INPUT_PERMISSIVE:
					return "tblProcessVariableNoAdditiveInputPermissive";
				case UNIT_TYPE.NOADDITIVE_OUTPUT_PERMISSIVE:
					return "tblProcessVariableNoAdditiveOutputPermissive";
				case UNIT_TYPE.STATION_INPUT_PERMISSIVE:
					return "tblProcessVariableStationInputPermissive";
				case UNIT_TYPE.STATION_OUTPUT_PERMISSIVE:
					return "tblProcessVariableStationOutputPermissive";
				case UNIT_TYPE.PRODUCT_MAP_PRESET_EXTERNAL_COMPONENT:
					return "tblProcessVariableExternalComponentBlendPercentage";
				case UNIT_TYPE.PRODUCT_MAP_PRESET_INJECTOR:
					return "tblProcessVariablePresetInjector";
				case UNIT_TYPE.EXTERNAL_COMPONENT_INPUT_PERMISSIVE:
					return "tblProcessVariableExternalComponentInputPermissive";
				case UNIT_TYPE.EXTERNAL_COMPONENT_OUTPUT_PERMISSIVE:
					return "tblProcessVariableExternalComponentOutputPermissive";
				case UNIT_TYPE.FLOW_CONTROLLED_ADDITIVE_INPUT_PERMISSIVE:
					return "tblProcessVariableFlowControlledAdditiveInputPermissive";
				case UNIT_TYPE.FLOW_CONTROLLED_ADDITIVE_OUTPUT_PERMISSIVE:
					return "tblProcessVariableFlowControlledAdditiveOutputPermissive";
				case UNIT_TYPE.EQUIPMENT_UNIT:
			return "tblProcessVariableEquipment";
				case UNIT_TYPE.PRODUCT_MAP_OFFLOAD_EXTERNAL_METER:
					return "tblProcessVariableOffloadExternalMeter";
				case UNIT_TYPE.EXTERNAL_METER_INPUT_PERMISSIVE:
					return "tblProcessVariableExternalMeterInputPermissive";
				case UNIT_TYPE.EXTERNAL_METER_OUTPUT_PERMISSIVE:
					return "tblProcessVariableExternalMeterOutputPermissive";
				case UNIT_TYPE.MAX_UNIT:
					return "Unknown";
				default:
					return "Unknown";
			}
		}

		/// <summary>
		/// The get identity column name.
		/// </summary>
		/// <param name="unitType">
		/// The unit type.
		/// </param>
		/// <returns>
		/// The <see cref="string"/>.
		/// </returns>
		public static string GetIdentityColumnName(UNIT_TYPE unitType)
		{
			switch (unitType)
			{
				case UNIT_TYPE.UNDEFINED_UNIT:
					return "Unknown";
				case UNIT_TYPE.TANK_UNIT:
					return "ProcessVariableTankGuid";
				case UNIT_TYPE.LOADARM_UNIT:
					return "ProcessVariableLoadArmGuid";
				case UNIT_TYPE.STATION_UNIT:
					return "ProcessVariableStationGuid";
				case UNIT_TYPE.SITE_UNIT:
					return "ProcessVariableSiteGuid";
				case UNIT_TYPE.COMPONENT_INPUT_PERMISSIVE:
				case UNIT_TYPE.COMPONENT_OUTPUT_PERMISSIVE:
					return "ProcessVariableProductToPresetComponentTankOrTankGroupGuid";
				case UNIT_TYPE.ADDITIVE_INPUT_PERMISSIVE:
				case UNIT_TYPE.ADDITIVE_OUTPUT_PERMISSIVE:
					return "ProcessVariableProductToPresetInjectorGuid";
				case UNIT_TYPE.RECIPE_INPUT_PERMISSIVE:
				case UNIT_TYPE.RECIPE_OUTPUT_PERMISSIVE:
					return "ProcessVariableProductToPresetRecipeGuid";
				case UNIT_TYPE.LOADARM_INPUT_PERMISSIVE:
				case UNIT_TYPE.LOADARM_OUTPUT_PERMISSIVE:
				case UNIT_TYPE.NOADDITIVE_INPUT_PERMISSIVE:
				case UNIT_TYPE.NOADDITIVE_OUTPUT_PERMISSIVE:
					return "ProcessVariableLoadArmGuid";
				case UNIT_TYPE.STATION_INPUT_PERMISSIVE:
				case UNIT_TYPE.STATION_OUTPUT_PERMISSIVE:
					return "ProcessVariableStationGuid";
				case UNIT_TYPE.PRODUCT_MAP_PRESET_EXTERNAL_COMPONENT:
					return "ProcessVariableProductToPresetExternalComponentGuid";
				case UNIT_TYPE.PRODUCT_MAP_PRESET_INJECTOR:
					return "ProcessVariablePresetInjectorGuid";
				case UNIT_TYPE.EXTERNAL_COMPONENT_INPUT_PERMISSIVE:
				case UNIT_TYPE.EXTERNAL_COMPONENT_OUTPUT_PERMISSIVE:
					return "ProcessVariableProductToPresetExternalComponentGuid";
				case UNIT_TYPE.FLOW_CONTROLLED_ADDITIVE_INPUT_PERMISSIVE:
				case UNIT_TYPE.FLOW_CONTROLLED_ADDITIVE_OUTPUT_PERMISSIVE:
					return "ProcessVariableProductToPresetFlowControlledAdditiveGuid";
				case UNIT_TYPE.EQUIPMENT_UNIT:
				return "ProcessVariableEquipmentGuid";
				case UNIT_TYPE.PRODUCT_MAP_OFFLOAD_EXTERNAL_METER:
				case UNIT_TYPE.EXTERNAL_METER_INPUT_PERMISSIVE:
				case UNIT_TYPE.EXTERNAL_METER_OUTPUT_PERMISSIVE:
					return "ProcessVariableProductToOffloadExternalMeterGuid";
				case UNIT_TYPE.MAX_UNIT:
					return "Unknown";
				default:
					return "Unknown";
			}
		}

		/// <summary>
		/// The get unit GUID column name.
		/// </summary>
		/// <param name="unitType">
		/// The unit type.
		/// </param>
		/// <returns>
		/// The <see cref="string"/>.
		/// </returns>
		public static string GetUnitGuidColumnName(UNIT_TYPE unitType)
		{
			switch (unitType)
			{
				case UNIT_TYPE.UNDEFINED_UNIT:
					return "Unknown";
				case UNIT_TYPE.TANK_UNIT:
					return "TankGuid";
				case UNIT_TYPE.LOADARM_UNIT:
					return "LoadArmGuid";
				case UNIT_TYPE.STATION_UNIT:
					return "StationGuid";
				case UNIT_TYPE.SITE_UNIT:
					return "SiteGuid";
				case UNIT_TYPE.COMPONENT_INPUT_PERMISSIVE:
				case UNIT_TYPE.COMPONENT_OUTPUT_PERMISSIVE:
					return "ProductToPresetComponentTankOrTankGroupGuid";
				case UNIT_TYPE.ADDITIVE_INPUT_PERMISSIVE:
				case UNIT_TYPE.ADDITIVE_OUTPUT_PERMISSIVE:
					return "ProductToPresetInjectorGuid";
				case UNIT_TYPE.RECIPE_INPUT_PERMISSIVE:
				case UNIT_TYPE.RECIPE_OUTPUT_PERMISSIVE:
					return "ProductToPresetRecipeGuid";
				case UNIT_TYPE.LOADARM_INPUT_PERMISSIVE:
				case UNIT_TYPE.LOADARM_OUTPUT_PERMISSIVE:
				case UNIT_TYPE.NOADDITIVE_INPUT_PERMISSIVE:
				case UNIT_TYPE.NOADDITIVE_OUTPUT_PERMISSIVE:
					return "LoadArmGuid";
				case UNIT_TYPE.STATION_INPUT_PERMISSIVE:
				case UNIT_TYPE.STATION_OUTPUT_PERMISSIVE:
					return "StationGuid";
				case UNIT_TYPE.PRODUCT_MAP_PRESET_EXTERNAL_COMPONENT:
				case UNIT_TYPE.EXTERNAL_COMPONENT_INPUT_PERMISSIVE:
				case UNIT_TYPE.EXTERNAL_COMPONENT_OUTPUT_PERMISSIVE:
					return "ProductToPresetExternalComponentGuid";
					case UNIT_TYPE.FLOW_CONTROLLED_ADDITIVE_INPUT_PERMISSIVE:
					case UNIT_TYPE.FLOW_CONTROLLED_ADDITIVE_OUTPUT_PERMISSIVE:
					return "ProductToPresetFlowControlledAdditiveGuid";
					case UNIT_TYPE.PRODUCT_MAP_PRESET_INJECTOR:
					return "ProductToPresetInjectorGuid";
				case UNIT_TYPE.EQUIPMENT_UNIT:
					return "EquipmentGuid";
					case UNIT_TYPE.PRODUCT_MAP_OFFLOAD_EXTERNAL_METER:
						return "ProductToOffloadExternalMeterGuid";
					case UNIT_TYPE.EXTERNAL_METER_INPUT_PERMISSIVE:
					case UNIT_TYPE.EXTERNAL_METER_OUTPUT_PERMISSIVE:
						return "ProductToOffloadExternalMeterGuid";
					case UNIT_TYPE.MAX_UNIT:
					return "Unknown";
				default:
					return "Unknown";
			}
		}

		/// <summary>
		/// Get a list of the types of units that process variables can be associated with. 
		/// This is used to help functionality that queries all process variables in the system, not just
		/// the process variable in a particular table
		/// </summary>
		/// <returns>a list of the types of units that process variables can be associated with.</returns>
		public static ArrayList GetProcessVariableUnitTypes()
		{
			ArrayList types = new ArrayList();

			foreach (UNIT_TYPE mapType in Enum.GetValues(typeof(UNIT_TYPE)))
			{
				if (mapType != UNIT_TYPE.MAX_UNIT && mapType != UNIT_TYPE.UNDEFINED_UNIT)
				{
					types.Add(mapType);
				}
			}

			return types;
		}

		public AlarmAndEventLogClass PermissiveLostAlarm
		{
			get
			{
				AlarmAndEventLogClass AlarmAndEventLog = new AlarmAndEventLogClass(PermissiveLostAlarmDescriptor);
				AlarmAndEventLog.AssociatedData = this._OPCItemID + " - " + this.MessageID;
				return AlarmAndEventLog;
			}
		}

		public AlarmAndEventLogClass PermissiveRestoredEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(PermissiveRestoredEventDescriptor)
																		{
																			AssociatedData = this._OPCItemID + " - " + this.MessageID
																		};
				return alarmAndEventLog;
			}
		}

		/// <summary>
		/// The get value.
		/// </summary>
		/// <param name="units">
		/// The units.
		/// </param>
		/// <param name="decimalPlaces">
		/// The decimal places.
		/// </param>
		/// <returns>
		/// The <see cref="object"/>.
		/// </returns>
		[SecurityCritical]
		public object GetValue(EngineeringUnit units, byte decimalPlaces)
		{
			if (this.siValue is double
			&& this.UnitsType != EngineeringUnitType.FmuNodim
			&& units != EngineeringUnit.FmSiteUnits
			&& this.DataType == VarEnum.VT_R8)
			{
				// The following deals with an error in UnitConvert which
				// is not catching divide by 0 
				// ReSharper disable once CompareOfFloatsByEqualityOperator
				if (units == EngineeringUnit.FmdDegApi && (double)this.siValue == 0.0)
				{
					return 1e20;
				}

				double result = 0.0;

				EngineeringUnits.Convert((double)this.siValue, this.SIUnits, ref result, units, this.ReferenceTemperature);

				if (units != EngineeringUnit.FmlFtIn16Th && units != EngineeringUnit.FmlFtIn8Th)
				{
					result = Math.Round(result, decimalPlaces, MidpointRounding.AwayFromZero);
				}

				return result;
			}
			
			return this.siValue;
		}

		[SecurityCritical]
		public void SetValue(object value, EngineeringUnit units)
		{
				this.SetValue(value, units, CultureInfo.InvariantCulture.NumberFormat);
		}

		[SecurityCritical]
		public void SetValue(object value, EngineeringUnit units, NumberFormatInfo numberFormatInfo)
		{
			if (value is string && this.DataType == VarEnum.VT_R8)
			{
				try
				{
					value = Convert.ToDouble(value, numberFormatInfo);
				}
				catch (Exception)
				{
					throw new Exception(ProcessVariableTypeID(this.ProcessVariableType) + " [String Conversion Error]");
				}
			}

			if (value is double
			&& this.UnitsType != EngineeringUnitType.FmuNodim
			&& units != EngineeringUnit.FmSiteUnits
			&& this.DataType == VarEnum.VT_R8)
			{
				double result = 0.0;

				EngineeringUnits.Convert((double)value,
													units,
													ref result, this.SIUnits, this.ReferenceTemperature);

				this.siValue = result;

				EngineeringUnits.Convert((double)value,
													units,
													ref result, this.ServerUnits, this.ReferenceTemperature);

				if (!typeof(double).IsInstanceOfType(this.serverValue)
				|| !result.Equals(this.serverValue)
				|| !this.IsQualityGood)
				{
					this.serverValue = result;
					this.DataChanged = true;
				}
			}
			else
			{
				if ((this.siValue != null && !this.siValue.Equals(value)) || (value != null && !value.Equals(this.siValue)))
				{
					this.siValue = value;
					this.serverValue = value;
					this.DataChanged = true;
				}
			}

			this.OPCQuality = Quality.Good.GetCode();
		}

		[SecurityCritical]
		public object GetMaximum(EngineeringUnit Units, byte DecimalPlaces)
		{
			if (typeof(Double).IsInstanceOfType(this.siMaximum)
			&& this.UnitsType != EngineeringUnitType.FmuNodim
			&& Units != EngineeringUnit.FmSiteUnits
			&& this.DataType == VarEnum.VT_R8)
			{
				// The following deals with an error in UnitConvert which
				// is not catching divide by 0 
				if (Units == EngineeringUnit.FmdDegApi && (double)this.siMaximum == 0.0)
				{
					return 1e20;
				}

				double Result = 0.0;

				EngineeringUnits.Convert((double)this.siMaximum, this.SIUnits, ref Result, Units, this.ReferenceTemperature);

				if (Units != EngineeringUnit.FmlFtIn16Th && Units != EngineeringUnit.FmlFtIn8Th)
				{
					Result = Math.Round(Result, DecimalPlaces, MidpointRounding.AwayFromZero);
				}

				return Result;
			}
			
			return this.siMaximum;
		}

		[SecurityCritical]
		public void SetMaximum(object Value, EngineeringUnit Units)
		{
			try
			{
				if (Value is string)
				{
					Value = this.Decode(Value as string, Units);
				}

				if (Value is double
				&& this.UnitsType != EngineeringUnitType.FmuNodim
				&& Units != EngineeringUnit.FmSiteUnits
				&& this.DataType == VarEnum.VT_R8)
				{
					double Result = 0.0;

					EngineeringUnits.Convert((double)Value, Units, ref Result, this.SIUnits, this.ReferenceTemperature);

					this.siMaximum = Result;
				}
				else
				{
					this.siMaximum = Value;
				}
			}
			catch (Exception)
			{
				throw new Exception(ProcessVariableTypeID(this.ProcessVariableType) + " [Set Maximum Error]");
			}
		}

		[SecurityCritical]
		public object GetMinimum(EngineeringUnit Units, byte DecimalPlaces)
		{
			if (this.siMinimum is double
			&& this.UnitsType != EngineeringUnitType.FmuNodim
			&& Units != EngineeringUnit.FmSiteUnits
			&& this.DataType == VarEnum.VT_R8)
			{
				// The following deals with an error in UnitConvert which
				// is not catching divide by 0 
				if (Units == EngineeringUnit.FmdDegApi && (double)this.siMinimum == 0.0)
				{
					return 1e20;
				}

				double Result = 0.0;

				EngineeringUnits.Convert((double)this.siMinimum, this.SIUnits,
													ref Result,
													Units, this.ReferenceTemperature);

				if (Units != EngineeringUnit.FmlFtIn16Th
				&& Units != EngineeringUnit.FmlFtIn8Th)
					Result = Math.Round(Result, (int)DecimalPlaces, MidpointRounding.AwayFromZero);

				return Result;
			}
			
			return this.siMinimum;
		}

		[SecurityCritical]
		public void SetMinimum(object Value, EngineeringUnit Units)
		{
			try
			{
				if (typeof(string).IsInstanceOfType(Value))
					Value = this.Decode(Value as string, Units);

				if (typeof(Double).IsInstanceOfType(Value)
				&& this.UnitsType != EngineeringUnitType.FmuNodim
				&& Units != EngineeringUnit.FmSiteUnits
				&& this.DataType == VarEnum.VT_R8)
				{
					double Result = 0.0;

					EngineeringUnits.Convert((double)Value,
														Units,
														ref Result, this.SIUnits, this.ReferenceTemperature);

					this.siMinimum = Result;
				}
				else this.siMinimum = Value;
			}

			catch (Exception)
			{
				throw new Exception(ProcessVariableTypeID(this.ProcessVariableType) + " [Set Minimum Error]");
			}
		}

		/// <summary>
		/// The reset.
		/// </summary>
		public override void Reset()
		{
			this.Init();
		}

		/// <summary>
		/// The unit type ID.
		/// </summary>
		/// <param name="type">
		/// The type.
		/// </param>
		/// <returns>
		/// The <see cref="string"/>.
		/// </returns>
		public static string UnitTypeID(UNIT_TYPE type)
		{
			switch (type)
			{
				case UNIT_TYPE.TANK_UNIT:
					return "Tank";
				case UNIT_TYPE.LOADARM_UNIT:
					return "Load Arm";
				case UNIT_TYPE.STATION_UNIT:
					return "Station";
				case UNIT_TYPE.SITE_UNIT:
					return "Site";
				case UNIT_TYPE.COMPONENT_INPUT_PERMISSIVE:
					return "Component";
				case UNIT_TYPE.COMPONENT_OUTPUT_PERMISSIVE:
					return "Component";
				case UNIT_TYPE.ADDITIVE_INPUT_PERMISSIVE:
					return "Additive";
				case UNIT_TYPE.ADDITIVE_OUTPUT_PERMISSIVE:
					return "Additive";
				case UNIT_TYPE.RECIPE_INPUT_PERMISSIVE:
					return "Recipe";
				case UNIT_TYPE.RECIPE_OUTPUT_PERMISSIVE:
					return "Recipe";
				case UNIT_TYPE.NOADDITIVE_INPUT_PERMISSIVE:
					return "No Additive";
				case UNIT_TYPE.NOADDITIVE_OUTPUT_PERMISSIVE:
					return "No Additive";
				case UNIT_TYPE.LOADARM_INPUT_PERMISSIVE:
					return "Load Arm";
				case UNIT_TYPE.LOADARM_OUTPUT_PERMISSIVE:
					return "Load Arm";
				case UNIT_TYPE.STATION_INPUT_PERMISSIVE:
					return "Station";
				case UNIT_TYPE.STATION_OUTPUT_PERMISSIVE:
					return "Station";
				case UNIT_TYPE.PRODUCT_MAP_PRESET_EXTERNAL_COMPONENT:
				case UNIT_TYPE.PRODUCT_MAP_PRESET_INJECTOR:
					case UNIT_TYPE.PRODUCT_MAP_OFFLOAD_EXTERNAL_METER:
					return "Product Map";
				case UNIT_TYPE.EQUIPMENT_UNIT:
					return "Equipment";
				default:
					return "Undefined";
			}
		}

		/// <summary>
		/// The process variable type ID.
		/// </summary>
		/// <param name="type">
		/// The type.
		/// </param>
		/// <returns>
		/// The <see cref="string"/>.
		/// </returns>
		public static string ProcessVariableTypeID(PROCESS_VARIABLE_TYPE type)
		{
			switch (type)
			{
				case PROCESS_VARIABLE_TYPE.LEVEL_PV:
					return "Level";

				case PROCESS_VARIABLE_TYPE.TEMPERATURE_PV:
					return "Temperature";

				case PROCESS_VARIABLE_TYPE.GROSS_VOLUME_PV:
					return "Gross Volume";

				case PROCESS_VARIABLE_TYPE.NET_VOLUME_PV:
					return "Net Volume";

				case PROCESS_VARIABLE_TYPE.DENSITY_PV:
					return "Density";

				case PROCESS_VARIABLE_TYPE.STANDARD_DENSITY_PV:
					return "Standard Density";

				case PROCESS_VARIABLE_TYPE.MASS_PV:
					return "Mass";

				case PROCESS_VARIABLE_TYPE.SWING_ARM_STATUS_PV:
					return "Swing Arm Position";

				case PROCESS_VARIABLE_TYPE.CARDREADER_PV:
					return "Cardreader";

				case PROCESS_VARIABLE_TYPE.RESET_CARDREADER_DATA_PV:
					return "Reset Cardreader";

				case PROCESS_VARIABLE_TYPE.KEYPAD_DATA_PV:
					return "Keypad Data";

				case PROCESS_VARIABLE_TYPE.RELEASE_KEYPAD_PV:
					return "Release Keypad";

				case PROCESS_VARIABLE_TYPE.PROMPT_TIMEOUT_PV:
					return "Prompt Timeout";

				case PROCESS_VARIABLE_TYPE.DISPLAY_PV:
					return "Display";

				case PROCESS_VARIABLE_TYPE.PASSWORD_PV:
					return "Password";

				case PROCESS_VARIABLE_TYPE.START_PERMISSIVE_PV:
					return "Start Permissive";

				case PROCESS_VARIABLE_TYPE.COMPLETION_PERMISSIVE_PV:
					return "Completion Permissive";

				case PROCESS_VARIABLE_TYPE.GATE_CONTROL_PV:
					return "Gate Control";

				case PROCESS_VARIABLE_TYPE.LOADARM_PV:
					return "Load Arm";

				case PROCESS_VARIABLE_TYPE.SITE_PERMISSIVE_PV:
					return "Site Permissive";

				case PROCESS_VARIABLE_TYPE.SITE_ALARM_OUTPUT_PV:
					return "Alarm Output";

				case PROCESS_VARIABLE_TYPE.SITE_WATCHDOG_OUTPUT_PV:
					return "Watchdog Output";

				case PROCESS_VARIABLE_TYPE.VRU_SETPOINT_PV:
					return "VRU Setpoint";

				case PROCESS_VARIABLE_TYPE.VRU_DEADBAND_PV:
					return "VRU Deadband";

				case PROCESS_VARIABLE_TYPE.TRANSACTION_DONE_PV:
					return "Transaction Done";

				case PROCESS_VARIABLE_TYPE.TRANSACTION_IN_PROGRESS_PV:
					return "Transaction In Progress";

				case PROCESS_VARIABLE_TYPE.BATCH_DONE_PV:
					return "Batch Done";

				case PROCESS_VARIABLE_TYPE.VCF_PV:
					return "VCF";

				case PROCESS_VARIABLE_TYPE.LOAD_ARM_RELEASED_PV:
					return "Released";

				case PROCESS_VARIABLE_TYPE.AVAILABLE_GROSS_VOLUME_PV:
					return "Available Gross Volume";

				case PROCESS_VARIABLE_TYPE.REMAINING_GROSS_VOLUME_PV:
					return "Remaining Gross Volume";

				case PROCESS_VARIABLE_TYPE.FLOWING_PV:
					return "Flowing";

				case PROCESS_VARIABLE_TYPE.STATION_PV:
					return "Station";

				case PROCESS_VARIABLE_TYPE.WEIGHT_SCALE_PV:
					return "Weight Scale";

				case PROCESS_VARIABLE_TYPE.CLEAR_LIST_PV:
					return "Clear List";

				case PROCESS_VARIABLE_TYPE.DISPLAY_LIST_PV:
					return "Display List";

				case PROCESS_VARIABLE_TYPE.SELECTED_ITEM_PV:
					return "Selected Item";

				case PROCESS_VARIABLE_TYPE.SELECT_ITEM_PV:
					return "Select Item";

				case PROCESS_VARIABLE_TYPE.WRITE_ITEM_PV:
					return "Write Item";

				case PROCESS_VARIABLE_TYPE.BATCH_ABORTED_PV:
					return "Batch Aborted";

				case PROCESS_VARIABLE_TYPE.POWER_FAIL_OCCURRED_PV:
					return "Power-fail Occurred";

				case PROCESS_VARIABLE_TYPE.GET_KEY_PV:
					return "Get Key";

				case PROCESS_VARIABLE_TYPE.PRESETTING_IN_PROGRESS_PV:
					return "Presetting In Progress";

				case PROCESS_VARIABLE_TYPE.TANK_OPERATION_PV:
					return "Tank Operation";

				case PROCESS_VARIABLE_TYPE.VAPOR_PRESSURE_PV:
					return "Vapor Pressure";

				case PROCESS_VARIABLE_TYPE.AVAILABLE_NET_VOLUME_PV:
					return "Available Net Volume";

				case PROCESS_VARIABLE_TYPE.REMAINING_NET_VOLUME_PV:
					return "Remaining Net Volume";

				case PROCESS_VARIABLE_TYPE.AUTHORIZED_PV:
					return "Authorized";

				case PROCESS_VARIABLE_TYPE.BATCH_ENDED_PV:
					return "Batch Ended";

				case PROCESS_VARIABLE_TYPE.BATCH_IN_PROGRESS_PV:
					return "Batch In Progress";

				case PROCESS_VARIABLE_TYPE.TRANSACTION_END_REQUESTED_PV:
					return "Transaction End Requested";

				case PROCESS_VARIABLE_TYPE.RECIPE_SELECTED_PV:
					return "Recipe Selected";

				case PROCESS_VARIABLE_TYPE.PRESET_VOLUME_ENTERED_PV:
					return "Preset Volume Entered";

				case PROCESS_VARIABLE_TYPE.BATCH_AUTHORIZED_PV:
					return "Batch Authorized";

				case PROCESS_VARIABLE_TYPE.TRANSACTION_AUTHORIZED_PV:
					return "Transaction Authorized";

				case PROCESS_VARIABLE_TYPE.KEY_PRESSED_PV:
					return "Key Pressed";

				case PROCESS_VARIABLE_TYPE.PRIMARY_ALARM_PV:
					return "Primary Alarm";

				case PROCESS_VARIABLE_TYPE.TANK_STATUS_PV:
					return "Tank Status";

				case PROCESS_VARIABLE_TYPE.MANUAL_PV:
					return "Manual";

				case PROCESS_VARIABLE_TYPE.LOAD_ARM_STATE_PV:
					return "Load Arm State";

				case PROCESS_VARIABLE_TYPE.INPUT_DONE_PV:
					return "Input Done";

				case PROCESS_VARIABLE_TYPE.RECIPE_PV:
					return "Recipe";

				case PROCESS_VARIABLE_TYPE.OUTPUT_PERMISSIVE_PV:
					return "Output Permissive";

				case PROCESS_VARIABLE_TYPE.INPUT_PERMISSIVE_PV:
					return "Input Permissive";

				case PROCESS_VARIABLE_TYPE.PERMISSIVE_DELAY_PV:
					return "Permissive Delay";

				case PROCESS_VARIABLE_TYPE.COMPONENT_METER_FLOW_TOTAL_PV:
					return "Meter Flow Total";

				case PROCESS_VARIABLE_TYPE.BLEND_PERCENTAGE_PV:
					return "Blend Percentage";

				case PROCESS_VARIABLE_TYPE.ADDITIVE_METER_FLOW_TOTAL_PV:
					return "Additive Meter Total";

				case PROCESS_VARIABLE_TYPE.KEYPAD_DATA_PENDING_PV:
					return "Keypad Data Pending";

				case PROCESS_VARIABLE_TYPE.ALARM_PV:
					return "Alarm";

				case PROCESS_VARIABLE_TYPE.WATER_LEVEL_PV:
					return "Water Level";

				case PROCESS_VARIABLE_TYPE.WATER_VOLUME_PV:
					return "Water Volume";

				default:
					return "Undefined";
			}
		}

		public SqlDbType GetParameterType(object Value)
		{
			SqlDbType retType = SqlDbType.Variant;

			if (typeof(SByte).IsInstanceOfType(Value))
			{
				retType = SqlDbType.TinyInt;
			}
			else if (typeof(Int16).IsInstanceOfType(Value)
			|| typeof(Byte).IsInstanceOfType(Value))
			{
				retType = SqlDbType.SmallInt;
			}
			else if (typeof(UInt16).IsInstanceOfType(Value)
			|| typeof(Int32).IsInstanceOfType(Value))
			{
				retType = SqlDbType.Int;
			}
			else if (typeof(Int64).IsInstanceOfType(Value)
			|| typeof(UInt32).IsInstanceOfType(Value))
			{
				retType = SqlDbType.BigInt;
			}
			else if (typeof(Single).IsInstanceOfType(Value))
			{
				retType = SqlDbType.Real;
			}
			else if (typeof(Double).IsInstanceOfType(Value))
			{
				retType = SqlDbType.Float;
			}

			else if (typeof(String).IsInstanceOfType(Value))
			{
				retType = SqlDbType.NVarChar;
			}

			return retType;
		}

		/// <summary>
		/// This method serializes object value and assigns byte array to a sql command parameter.
		/// The type of the serialized data will be stored seperatley using method AddVariantParameterType.
		/// Utilized in InsertSQLCmd and UpdateSQLCmd for SIValue, siMaximum and siMinimum.
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="prefix"></param>
		/// <param name="paramName"></param>
		/// <param name="Value"></param>
		/// <returns></returns>
		public string AddVariantParameter(SqlCommand cmd, string prefix, string paramName, object Value)
		{
			if (Value != null)
			{
				cmd.Parameters.Add(paramName, SqlDbType.VarBinary);
				if(typeof(byte).IsInstanceOfType(Value))
					cmd.Parameters[paramName].Value = BitConverter.GetBytes((byte) Value);
				else if (typeof(bool).IsInstanceOfType(Value))
					cmd.Parameters[paramName].Value = BitConverter.GetBytes((bool)Value);
				else if (typeof(short).IsInstanceOfType(Value))
					cmd.Parameters[paramName].Value = BitConverter.GetBytes((short)Value);
				else if (typeof(int).IsInstanceOfType(Value))
					cmd.Parameters[paramName].Value = BitConverter.GetBytes((int)Value);
				else if (typeof(long).IsInstanceOfType(Value))
					cmd.Parameters[paramName].Value = BitConverter.GetBytes((long)Value);
				else if (typeof(char).IsInstanceOfType(Value))
					cmd.Parameters[paramName].Value = BitConverter.GetBytes((char)Value);
				else if (typeof(ushort).IsInstanceOfType(Value))
					cmd.Parameters[paramName].Value = BitConverter.GetBytes((ushort)Value);
				else if (typeof(uint).IsInstanceOfType(Value))
					cmd.Parameters[paramName].Value = BitConverter.GetBytes((uint)Value);
				else if (typeof(ulong).IsInstanceOfType(Value))
					cmd.Parameters[paramName].Value = BitConverter.GetBytes((ulong)Value);
				else if (typeof(float).IsInstanceOfType(Value))
					cmd.Parameters[paramName].Value = BitConverter.GetBytes((float)Value);
				else if (typeof(double).IsInstanceOfType(Value))
					cmd.Parameters[paramName].Value = BitConverter.GetBytes((double)Value);
				else if(Value is Enum)
				{
					var underlyingType = Enum.GetUnderlyingType(Value.GetType());
					if(underlyingType == typeof(int))
						cmd.Parameters[paramName].Value = BitConverter.GetBytes((int)Value);
					else if(underlyingType == typeof(short))
						cmd.Parameters[paramName].Value = BitConverter.GetBytes((short)Value);
					else if (underlyingType == typeof(byte))
						cmd.Parameters[paramName].Value = BitConverter.GetBytes((byte)Value);

				}
				else if (typeof(string).IsInstanceOfType(Value)
				&& (Value as string) != null)
				{
					byte[] bytes = new byte[(Value as string).Length * sizeof(char)];
					Buffer.BlockCopy((Value as string).ToCharArray(), 0, bytes, 0, bytes.Length);
					cmd.Parameters[paramName].Value = bytes;
				}
				else if(typeof(DateTime).IsInstanceOfType(Value))
					cmd.Parameters[paramName].Value = BitConverter.GetBytes(((DateTime) Value).Ticks);
				else
					cmd.Parameters[paramName].Value = DBNull.Value;
			}
			else
			{
				cmd.Parameters.Add(paramName, SqlDbType.VarBinary);
				cmd.Parameters[paramName].Value = DBNull.Value;
			}

			return prefix + paramName + " ";
		}

		/// <summary>
		/// Adds command paramaters for lookupMaximumVariantTypeIndex, lookupSIValueVariantTypeIndex and lookupMinimumVariantTypeIndex
		/// These columns will store the type of variable stored in AddVariantParameter
		/// Value will contain type name (string, float, etc.)  Subquery will return the integer index from lookup.tblVariantType
		/// Utilized by methods InsertSQLCmd and UpdateSQLCmd
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="paramName"></param>
		/// <param name="Value"></param>
		/// <returns></returns>
		public string AddVariantParameterType(SqlCommand cmd, string prefix, string paramName, object Value)
		{
			string returnStr = prefix + " NULL ";

 			if (Value != null)
			{
				cmd.Parameters.Add(paramName + "CodeType", SqlDbType.NVarChar, 100);
				cmd.Parameters[paramName + "CodeType"].Value = Value.GetType().FullName;

				returnStr = prefix + " (SELECT TOP 1 VariantTypeIndex FROM lookup.tblVariantType WHERE CodeType = " + paramName + "CodeType) ";
			}
			return returnStr;
		}

		/// <summary>
		/// Deserializes SIValue, Maximum or Minimum from a Process Variable table.
		/// Utilized by the ProcessVariableLoad method.
		/// Returns an objected typed according to CodeType column in lookup.tblTypeVariant
		/// </summary>
		/// <param name="content"></param>
		/// <param name="ValueType"></param>
		/// <returns></returns>
		public object VariantParameterConversion(byte[] content, string ValueType)
		{
			try
			{
				if ((ValueType != null) & (content != null))
				{
					if (ValueType == "System.Byte")
						return content[0];
					else if (ValueType == "System.Int16")
						return BitConverter.ToInt16(content, 0);
					else if (ValueType == "System.UInt16")
						return BitConverter.ToUInt16(content, 0);
					else if (ValueType == "System.UInt16")
						return BitConverter.ToUInt16(content, 0);
					else if (ValueType == "System.Int32")
						return BitConverter.ToInt32(content, 0);
					else if (ValueType == "System.UInt32")
						return BitConverter.ToUInt32(content, 0);
					else if (ValueType == "System.Int64")
						return BitConverter.ToInt64(content, 0);
					else if (ValueType == "System.UInt64")
						return BitConverter.ToUInt64(content, 0);
					else if (ValueType == "System.Single")
						return BitConverter.ToSingle(content, 0);
					else if (ValueType == "System.Double")
						return BitConverter.ToDouble(content, 0);
					else if (ValueType == "System.String")
					{
						char[] chars = new char[content.Length / sizeof(char)];
						Buffer.BlockCopy(content, 0, chars, 0, content.Length);
						return new string(chars);
					}
					else if (ValueType == "System.DateTime")
						return new DateTime(BitConverter.ToInt64(content, 0));
				}
			}
			catch
			{
			}

			return null;
		}

		/// <summary>
		/// Gets a value indicating whether is quality good.
		/// </summary>
		public bool IsQualityGood
		{
			get
			{
				var quality = new Quality(this.OPCQuality);

				if (quality.QualityBits == qualityBits.good
				|| quality.QualityBits == qualityBits.goodLocalOverride)
				{
					return true;
				}
				
				return false;
			}
		}

		/// <summary>
		/// Gets a value indicating whether is quality uncertain.
		/// </summary>
		public bool IsQualityUncertain
		{
			get
			{
				var quality = new Quality(this.OPCQuality);

				if (quality.QualityBits == qualityBits.uncertain
				|| quality.QualityBits == qualityBits.uncertainLastUsableValue
				)
				{
					return true;
				}

				return false;
			}
		}



		public string Encode(object Value, Quality Quality, EngineeringUnit Units, NumberFormatInfo Format)
		{
			if (Quality.QualityBits == qualityBits.good
			|| Quality.QualityBits == qualityBits.goodLocalOverride
			|| Quality.QualityBits == qualityBits.uncertain
			|| Quality.QualityBits == qualityBits.uncertainLastUsableValue)
			{
				if ((Units == EngineeringUnit.FmlFtIn16Th || Units == EngineeringUnit.FmlFtIn8Th)
					&& Value is double)
				{

					int Feet, Inch, Fract;
					double Fraction, ValueDouble;
					bool Negative;

					ValueDouble = Convert.ToDouble(Value);

					// Get Whole Feet to Integer
					Negative = (ValueDouble < 0.00) ? true : false;
					if (Negative)
					{
						ValueDouble = -ValueDouble;
					}

					Feet = (int)ValueDouble;
					Fraction = ValueDouble - Feet;

					// Convert to Inches
					Fraction *= 12.0000;
					Inch = (int)Fraction;
					Fraction -= Inch;

					int Factor = (Units == EngineeringUnit.FmlFtIn16Th) ? 16 : 8;

					// Convert to Fraction
					Fraction *= Factor;
					Fract = (int)(Fraction + 0.500);

					if (Fract >= Factor)
					{
						Inch++;
						Fract = 0;

						if (Inch >= 12)
						{
							Feet++;
							Inch = 0;
						}
					}

					if (Negative)
					{
						if (Units == EngineeringUnit.FmlFtIn16Th)
						{
							return "-" + Feet.ToString("D2") + "-" + Inch.ToString("D2") + "-" + Fract.ToString("D2");
						}
						
						return "-" + Feet.ToString("D2") + "-" + Inch.ToString("D2") + "-" + Fract.ToString("D1");
					}

					if (Units == EngineeringUnit.FmlFtIn16Th)
					{
						return Feet.ToString("D2") + "-" + Inch.ToString("D2") + "-" + Fract.ToString("D2");
					}
						
					return Feet.ToString("D2") + "-" + Inch.ToString("D2") + "-" + Fract.ToString("D1");
				}

				if (Value is float)
				{
					return ((float)Value).ToString("N", Format);
				}
				
				if (Value is double)
				{
					return ((double)Value).ToString("N", Format);
				}
				
				if (Value is DBNull || Value == null)
				{
					return new Quality(Quality.Bad.GetCode()).ToString();
				}
				
				return Value.ToString();
			}
			
			return Quality.ToString();
		}

		public object Decode(string valueString, EngineeringUnit units)
		{
			if (units == EngineeringUnit.FmlFtIn16Th
			|| units == EngineeringUnit.FmlFtIn8Th)
			{
				double value;
				bool negative = false;

				if (valueString.Length == 0
				|| valueString.Length > 19)
				{
					var resourceManager = new ResourceManager("ConsolidatedDataObjects.DefaultResource", Assembly.GetExecutingAssembly());

					throw new Exception(resourceManager.GetString("IDS_ERROR_INVALID_VALUE"));
				}

				// Trim Leading Spaces if Any
				valueString = valueString.Trim();

				if (valueString[0] == '-')
				{
					negative = true;
					valueString = valueString.Remove(0, 1);
				}

				int iDelimiter = valueString.IndexOf("-", StringComparison.Ordinal);

				if (iDelimiter == -1)
				{
					value = Convert.ToDouble(valueString);
				}
				else
				{
					value = Convert.ToDouble(valueString.Substring(0, iDelimiter));
					valueString = valueString.Substring(iDelimiter + 1);

					iDelimiter = valueString.IndexOf("-", StringComparison.Ordinal);

					if (iDelimiter == -1)
					{
						value += Convert.ToDouble(valueString) / 12;
					}
					else
					{
						value += Convert.ToDouble(valueString.Substring(iDelimiter)) / 12;
						valueString = valueString.Substring(iDelimiter + 1);
						int iFactor = (units == EngineeringUnit.FmlFtIn16Th) ? 192 : 96;
						value += Convert.ToDouble(valueString) / iFactor;
					}
				}

				if (negative)
				{
					value = -value;
				}

				return value;
			}

			switch (this.DataType)
			{
				case VarEnum.VT_I1:
					return Convert.ToSByte(valueString);
				case VarEnum.VT_I2:
					return Convert.ToInt16(valueString);
				case VarEnum.VT_I4:
					return Convert.ToInt32(valueString);
				case VarEnum.VT_UI1:
					return Convert.ToByte(valueString);
				case VarEnum.VT_UI2:
					return Convert.ToUInt16(valueString);
				case VarEnum.VT_UI4:
					return Convert.ToUInt32(valueString);
				case VarEnum.VT_R4:
					return Convert.ToSingle(valueString);
				case VarEnum.VT_R8:
					return Convert.ToDouble(valueString);
				case VarEnum.VT_BSTR:
					return valueString;
				default:
					return null;
			}
		}

		/// <summary>
		/// The load.
		/// </summary>
		/// <param name="o">
		/// The load object.
		/// </param>
		public override void Load(object o)
		{
			UNIT_TYPE unitType = this.UnitType;
			//this.Reset();

			if ( o is DataSet )
			{
				DataSet set = (DataSet) o;

				DataTable Table = set.Tables[0];

				if (Table.Rows.Count == 0)
				{
					return;
				}

				DataRow row = Table.Rows[0];

				this.UnitType = unitType;
				this.IdentityGuid = DataObject.getValue<Guid>(row[GetIdentityColumnName(this.UnitType)], Guid.Empty);
				this.ProcessVariableType = DataObject.getValue<PROCESS_VARIABLE_TYPE>(row["LookupProcessVariableTypeIndex"], PROCESS_VARIABLE_TYPE.MAX_PV);
				this.InstanceNumber = DataObject.getValue<int>(row["InstanceNumber"], 0);
				this.UnitGuid = DataObject.getValue<Guid>(row[GetUnitGuidColumnName(this.UnitType)], Guid.Empty);
				this.OPCConnectionGuid = DataObject.getValue<Guid>(row["OPCConnectionGuid"], Guid.Empty);
				this._OPCItemID = DataObject.getValue<string>(row["OPCItemID"], "");
				this.DataType = DataObject.getValue<VarEnum>(row["DataType"], VarEnum.VT_NULL);
				this.ServerUnits = DataObject.getValue<EngineeringUnit>(row["ServerEngineeringUnitsIndex"], EngineeringUnit.FmduPCent);
				this.OPCQuality = DataObject.getValue<short>(row["Quality"], Quality.Bad.GetCode());
				this.SIValue = this.VariantParameterConversion(DataObject.getValue<byte[]>(row["SIValue"], null), DataObject.getValue<string>(row["SIValueType"], null));
				this.DateTimeStamp = DataObject.getValue<DateTimeOffset>(row["DateTimeStamp"], DateTimeOffset.Now);
				this.siMaximum = this.VariantParameterConversion(DataObject.getValue<byte[]>(row["Maximum"], null), DataObject.getValue<string>(row["MaximumType"], null));
				this.siMinimum = this.VariantParameterConversion(DataObject.getValue<byte[]>(row["Minimum"], null), DataObject.getValue<string>(row["MinimumType"], null));
				this.DataTypeEnabled = DataObject.getValue<bool>(row["DataTypeEnabled"], true);
				this.Input = DataObject.getValue<bool>(row["Input"], true);
				this.InputEnabled = DataObject.getValue<bool>(row["InputEnabled"], true);
				this.MessageApplicationStringGuid = DataObject.getValue<Guid>(row["MessageApplicationStringGuid"], Guid.Empty);
				this.CreatedDate = DataObject.getValue<DateTimeOffset>(row["CreatedDate"], DateTimeOffset.Now);
				this.CreatedBy = DataObject.getValue<string>(row["CreatedBy"], ADMIN);
				this.UpdatedDate = DataObject.getValue<DateTimeOffset>(row["UpdatedDate"], this.CreatedDate);
				this.UpdatedBy = DataObject.getValue<string>(row["UpdatedBy"], ADMIN);
				this._URL = DataObject.getValue<string>(row["URL"], "");
				this._ProgID = DataObject.getValue<string>(row["ProgID"], "");
				this.MessageID = DataObject.getValue<string>(row["MessageID"], "");
			}
			else if (typeof(ProcessVariableClass).IsInstanceOfType(o))
			{
				ProcessVariableClass ProcessVariable = (ProcessVariableClass)o;

				this.IdentityGuid = ProcessVariable.IdentityGuid;
				this.ProcessVariableType = ProcessVariable.ProcessVariableType;
				this.InstanceNumber = ProcessVariable.InstanceNumber;
				this.UnitGuid = ProcessVariable.UnitGuid;
				this.UnitType = ProcessVariable.UnitType;
				this.OPCConnectionGuid = ProcessVariable.OPCConnectionGuid;
				this._OPCItemID = ProcessVariable.OPCItemID;
				this.DataType = ProcessVariable.DataType;
				this.ServerUnits = ProcessVariable.ServerUnits;
				this.OPCQuality = ProcessVariable.OPCQuality;
				this.SIValue = ProcessVariable.SIValue;
				this.DateTimeStamp = ProcessVariable.DateTimeStamp;
				this.siMaximum = ProcessVariable.siMaximum;
				this.siMinimum = ProcessVariable.siMinimum;
				this.DataTypeEnabled = ProcessVariable.DataTypeEnabled;
				this.Input = ProcessVariable.Input;
				this.InputEnabled = ProcessVariable.InputEnabled;
				this.MessageApplicationStringGuid = ProcessVariable.MessageApplicationStringGuid;
				this.CreatedDate = ProcessVariable.CreatedDate;
				this.CreatedBy = ProcessVariable.CreatedBy;
				this.UpdatedDate = ProcessVariable.UpdatedDate;
				this.UpdatedBy = ProcessVariable.UpdatedBy;
				this._URL = ProcessVariable.URL;
				this._ProgID = ProcessVariable.ProgID;
				this.MessageID = ProcessVariable.MessageID;

				this.DataChanged = ProcessVariable.DataChanged;
				this.OutputFailed = ProcessVariable.OutputFailed;

			}
			else
			{
				this.Load(o);
			}
		}

		public string AddWhereIdentityGuid(SqlCommand cmd)
		{
			return " WHERE " + DataObject.AddParameter(cmd, false, GetIdentityColumnName(this.UnitType), PARAM_NAME_PROCESSVARIABLEGUID_WHERE, PARAM_TYPE_PROCESSVARIABLEGUID, this.IdentityGuid);
		}

		public void InsertSQLCmd(SqlCommand cmd)
		{

			cmd.CommandText = "INSERT INTO " + GetTableName(this.UnitType) +
					"(LookupProcessVariableTypeIndex," +
					"InstanceNumber," +
					GetUnitGuidColumnName(this.UnitType) + "," +
					"OPCConnectionGuid," +
					"OPCItemID," +
					"DataType," +
					"ServerEngineeringUnitsIndex," +
					"Quality," +
					"SIValue," +
					"LookupSIValueVariantTypeIndex, " +
					"DateTimeStamp," +
					"Maximum," +
					"LookupMaximumVariantTypeIndex, " +
					"Minimum," +
					"LookupMinimumVariantTypeIndex, " +
					"DataTypeEnabled," +
					"[Input]," +
					"InputEnabled," +
					"MessageApplicationStringGuid," +
					"CreatedDate," +
					"CreatedBy," +
					"UpdatedDate," +
					"UpdatedBy," +
					GetIdentityColumnName(this.UnitType) +
					") VALUES (" +
						DataObject.AddParameter(cmd, string.Empty, PARAM_NAME_PROCESSVARIABLETYPE, PARAM_TYPE_PROCESSVARIABLETYPE, (int)this.ProcessVariableType) +
						DataObject.AddParameter(cmd, ",", PARAM_NAME_INSTANCENUMBER, PARAM_TYPE_INSTANCENUMBER, this.InstanceNumber) +
						DataObject.AddParameter(cmd, ",", PARAM_NAME_UNITGUID, PARAM_TYPE_UNITGUID, this.UnitGuid) +
						DataObject.AddGuidParameter(cmd, ",", PARAM_NAME_OPCCONNECTIONGUID, this.OPCConnectionGuid, true) +
						DataObject.AddParameter(cmd, ",", PARAM_NAME_OPCITEMID, PARAM_TYPE_OPCITEMID, PARAM_SIZE_OPCITEMID, this.OPCItemID) +
						DataObject.AddParameter(cmd, ",", PARAM_NAME_DATATYPE, PARAM_TYPE_DATATYPE, (int)this.DataType) +
						DataObject.AddParameter(cmd, ",", PARAM_NAME_SERVERENGINEERINGUNITSINDEX, PARAM_TYPE_SERVERENGINEERINGUNITSINDEX, (int)this.ServerUnits) +
						DataObject.AddParameter(cmd, ",", PARAM_NAME_QUALITY, PARAM_TYPE_QUALITY, this.OPCQuality) + this.AddVariantParameter(cmd, ",", PARAM_NAME_SIVALUE, this.SIValue) + this.AddVariantParameterType(cmd, ", ", PARAM_NAME_SIVALUE, this.SIValue) +
						DataObject.AddParameter(cmd, ",", PARAM_NAME_DATETIMESTAMP, PARAM_TYPE_DATETIMESTAMP, this.DateTimeStamp) + this.AddVariantParameter(cmd, ",", PARAM_NAME_MAXIMUM, this.siMaximum) + this.AddVariantParameterType(cmd, ", ", PARAM_NAME_MAXIMUM, this.siMaximum) + this.AddVariantParameter(cmd, ",", PARAM_NAME_MINIMUM, this.siMinimum) + this.AddVariantParameterType(cmd, ", ", PARAM_NAME_MINIMUM, this.siMinimum) +
						DataObject.AddParameter(cmd, ",", PARAM_NAME_DATATYPEENABLED, PARAM_TYPE_DATATYPEENABLED, this.DataTypeEnabled) +
						DataObject.AddParameter(cmd, ",", PARAM_NAME_INPUT, PARAM_TYPE_INPUT, this.Input) +
						DataObject.AddParameter(cmd, ",", PARAM_NAME_INPUTENABLED, PARAM_TYPE_INPUTENABLED, this.InputEnabled) +
						DataObject.AddGuidParameter(cmd, ",", PARAM_NAME_MESSAGEAPPLICATIONSTRINGGUID, this.MessageApplicationStringGuid, true) +
						DataObject.AddParameter(cmd, ",", PARAM_NAME_CREATEDDATE, PARAM_TYPE_CREATEDDATE, this.CreatedDate) +
						DataObject.AddParameter(cmd, ",", PARAM_NAME_CREATEDBY, PARAM_TYPE_CREATEDBY, PARAM_SIZE_CREATEDBY, this.CreatedBy) +
						DataObject.AddParameter(cmd, ",", PARAM_NAME_UPDATEDDATE, PARAM_TYPE_UPDATEDDATE, this.UpdatedDate) +
						DataObject.AddParameter(cmd, ",", PARAM_NAME_UPDATEDBY, PARAM_TYPE_UPDATEDBY, PARAM_SIZE_UPDATEDBY, this.UpdatedBy) +
						DataObject.AddParameter(cmd, ",", "@"+GetUnitGuidColumnName(this.UnitType), SqlDbType.UniqueIdentifier, this._IdentityGuid) +
					") ";

		}

		public SqlCommand UpdateSQLCmd(DATA_TYPE Type)
		{
			SqlCommand cmd = new SqlCommand();

			string sql;
			sql = "UPDATE " + GetTableName(this.UnitType) +
					" SET " + this.AddVariantParameter(cmd, " SIValue=", PARAM_NAME_SIVALUE, this.SIValue) + "," + this.AddVariantParameterType(cmd, " LookupSIValueVariantTypeIndex = ", PARAM_NAME_SIVALUE, this.SIValue) + ", " +
					DataObject.AddParameter(cmd, false, "Quality", PARAM_NAME_QUALITY, PARAM_TYPE_QUALITY, this.OPCQuality) + "," +
					DataObject.AddParameter(cmd, false, "DateTimeStamp", PARAM_NAME_DATETIMESTAMP, PARAM_TYPE_DATETIMESTAMP, this.DateTimeStamp) + "," +
					DataObject.AddParameter(cmd, false, "UpdatedDate", PARAM_NAME_UPDATEDDATE, PARAM_TYPE_UPDATEDDATE, this.UpdatedDate) + "," +
					DataObject.AddParameter(cmd, false, "UpdatedBy", PARAM_NAME_UPDATEDBY, PARAM_TYPE_UPDATEDBY, PARAM_SIZE_UPDATEDBY, this.UpdatedBy);
			if (Type == DATA_TYPE.CONFIG)
			{
				sql += "," + DataObject.AddParameter(cmd, false, "LookupProcessVariableTypeIndex", PARAM_NAME_PROCESSVARIABLETYPE, PARAM_TYPE_PROCESSVARIABLETYPE, (int)this.ProcessVariableType) + "," +
						DataObject.AddParameter(cmd, false, "InstanceNumber", PARAM_NAME_INSTANCENUMBER, PARAM_TYPE_INSTANCENUMBER, this.InstanceNumber) + "," +
						DataObject.AddParameter(cmd, false, GetUnitGuidColumnName(this.UnitType), PARAM_NAME_UNITGUID, PARAM_TYPE_UNITGUID, this.UnitGuid) + "," +
						DataObject.AddGuidParameter(cmd, " OPCConnectionGuid=", PARAM_NAME_OPCCONNECTIONGUID, this.OPCConnectionGuid, true) + "," +
						DataObject.AddParameter(cmd, false, "OPCItemID", PARAM_NAME_OPCITEMID, PARAM_TYPE_OPCITEMID, PARAM_SIZE_OPCITEMID, this.OPCItemID) + "," +
						DataObject.AddParameter(cmd, false, "DataType", PARAM_NAME_DATATYPE, PARAM_TYPE_DATATYPE, (int)this.DataType) + "," +
						DataObject.AddParameter(cmd, false, "ServerEngineeringUnitsIndex", PARAM_NAME_SERVERENGINEERINGUNITSINDEX, PARAM_TYPE_SERVERENGINEERINGUNITSINDEX, (int)this.ServerUnits) + "," +
						DataObject.AddParameter(cmd, false, "[Input]", PARAM_NAME_INPUT, PARAM_TYPE_INPUT, this.Input) + "," + this.AddVariantParameter(cmd, " Maximum=", PARAM_NAME_MAXIMUM, this.siMaximum) + "," + this.AddVariantParameterType(cmd, " LookUpMaximumVariantTypeIndex =", PARAM_NAME_MAXIMUM, this.siMaximum) + ", " + this.AddVariantParameter(cmd, " Minimum=", PARAM_NAME_MINIMUM, this.siMinimum) + "," + this.AddVariantParameterType(cmd, " LookUpMinimumVariantTypeIndex =", PARAM_NAME_MINIMUM, this.siMinimum) + ", " +
						DataObject.AddParameter(cmd, false, "DataTypeEnabled", PARAM_NAME_DATATYPEENABLED, PARAM_TYPE_DATATYPEENABLED, this.DataTypeEnabled) + "," +
						DataObject.AddParameter(cmd, false, "InputEnabled", PARAM_NAME_INPUTENABLED, PARAM_TYPE_INPUTENABLED, this.InputEnabled) + "," +
						DataObject.AddGuidParameter(cmd, " MessageApplicationStringGuid=", PARAM_NAME_MESSAGEAPPLICATIONSTRINGGUID, this.MessageApplicationStringGuid, true);
			}

			sql += this.AddWhereIdentityGuid(cmd);

			cmd.CommandText = sql;
			return cmd;
		}

		public SqlCommand PurgeSQLCmd
		{
			get
			{
				SqlCommand cmd = new SqlCommand();

				cmd.CommandText = "DELETE FROM " + GetTableName(this.UnitType) + this.AddWhereIdentityGuid(cmd);

				return cmd;
			}
		}

		public SqlCommand SelectSQLCmd(bool bInTransaction)
		{
			SqlCommand cmd = new SqlCommand();

			cmd.CommandText = this.SelectClause(this.UnitType) +
					" FROM " + GetTableName(this.UnitType) + SQLUpdateLock(bInTransaction) + this.AddWhereIdentityGuid(cmd);

			return cmd;
		}

		public SqlCommand SelectByTypeInstanceUnitSQLCmd(bool bInTransaction)
		{
			SqlCommand cmd = new SqlCommand();

			cmd.CommandText = this.SelectClause(this.UnitType) +
					" FROM " + GetTableName(this.UnitType) + SQLUpdateLock(bInTransaction) +
					" WHERE " +
					DataObject.AddParameter(cmd, false, "LookupProcessVariableTypeIndex", PARAM_NAME_PROCESSVARIABLETYPE, PARAM_TYPE_PROCESSVARIABLETYPE, (int)this.ProcessVariableType) +
					DataObject.AddParameter(cmd, true, "InstanceNumber", PARAM_NAME_INSTANCENUMBER, PARAM_TYPE_INSTANCENUMBER, this.InstanceNumber) +
					DataObject.AddParameter(cmd, true, GetUnitGuidColumnName(this.UnitType), PARAM_NAME_UNITGUID, PARAM_TYPE_UNITGUID, this.UnitGuid);

			return cmd;
		}

		public SqlCommand EnumerateByUnitSQLCmd(bool bInTransaction)
		{
			SqlCommand cmd = new SqlCommand();

			cmd.CommandText = this.SelectClause(this.UnitType) +
					" FROM " + GetTableName(this.UnitType) + SQLUpdateLock(bInTransaction) +
					" WHERE " +
					DataObject.AddParameter(cmd, false, GetUnitGuidColumnName(this.UnitType), PARAM_NAME_UNITGUID, PARAM_TYPE_UNITGUID, this.UnitGuid) +
					" ORDER BY LookupProcessVariableTypeIndex,InstanceNumber";

			return cmd;
		}

		/// <summary>
		/// This function creaates a command to query the count of records which reference the current OPCConnectionGuid
		/// This is used by Modify/Purge ProcessVariables to remove OPCConnection when last reference is deleted.
		/// </summary>
		/// <param name="bInTransaction"></param>
		/// <returns></returns>
		public SqlCommand FindOPCConnectionReferenceCount(bool bInTransaction)
		{
			SqlCommand cmd = new SqlCommand();

			string sql = string.Empty;

			bool isFirstTime = true;

			//We want to find references from each type
			foreach (UNIT_TYPE currentUnitType in Enum.GetValues(typeof(UNIT_TYPE)))
			{
				switch (currentUnitType)
				{
					// skip invalid types
					case UNIT_TYPE.UNDEFINED_UNIT:
					case UNIT_TYPE.MAX_UNIT:
						break;
					default:
						if (isFirstTime)
						{
							isFirstTime = false;
							sql = "SELECT ";
						}
						else
						{
							sql += "+";
						}
						// select the count of each table
						sql += "( SELECT COUNT(*) FROM " + GetTableName(currentUnitType) + SQLUpdateLock(bInTransaction) +
									" WHERE OPCConnectionGuid = @OPCConnectionGuid )";
						break;
				}
			}
			cmd.CommandText = sql;
			cmd.Parameters.AddWithValue("OPCConnectionGuid", this.OPCConnectionGuid);
			return cmd;
		}

		public SqlCommand EnumerateByMessageApplicationStringGuidSQLCmd
		{
			get
			{
				SqlCommand cmd = new SqlCommand();

				cmd.CommandText = this.SelectClause(this.UnitType) +
					" FROM " + GetTableName(this.UnitType) + " WITH (NOLOCK)" +
					" WHERE " +
					DataObject.AddParameter(cmd, false, "MessageApplicationStringGuid", PARAM_NAME_MESSAGEAPPLICATIONSTRINGGUID, PARAM_TYPE_MESSAGEAPPLICATIONSTRINGGUID, this.MessageApplicationStringGuid);

				return cmd;
			}
		}

		/// <summary>
		/// This method initializes the object to initial state.
		/// </summary>
		private void Init()
		{
			base.Reset( );
			this.ProcessVariableType			= PROCESS_VARIABLE_TYPE.MAX_PV;
			this.InstanceNumber					= 0;
			this.UnitGuid						= Guid.Empty;
			this.UnitType						= UNIT_TYPE.MAX_UNIT;
			this.OPCConnectionGuid				= Guid.Empty;
			this._OPCItemID						= string.Empty;
			this.DataType						= VarEnum.VT_NULL;
			this.ServerUnits					= EngineeringUnit.FmduPCent;
			this.OPCQuality						= Quality.Bad.GetCode( );
			this.siValue						= null;
			this.DateTimeStamp					= DateTimeOffset.Now;
			this.siMaximum						= null;
			this.siMinimum						= null;
			this.DataTypeEnabled				= true;
			this.Input							= true;
			this.InputEnabled					= true;
			this.serverValue					= null;
			this.ReferenceTemperature			= 0.0;
			this._URL							= string.Empty;
			this._ProgID						= string.Empty;
			this.MessageApplicationStringGuid	= Guid.Empty;
			this.MessageID						= string.Empty;
			this.DataChanged					= false;
			this.OutputFailed					= false;
		}


		public void FindByUrlAndItemSql(SqlCommand cmd, string url, string opcId, Guid identityGuid)
		{
			bool firstPass = true;
			cmd.CommandText = string.Empty;

			foreach(UNIT_TYPE unitType in Enum.GetValues(typeof(UNIT_TYPE)))
			{
				if (GetTableName(unitType) == "Unknown")
				{
					continue;
				}

				if (!firstPass)
				{
					cmd.CommandText += Environment.NewLine + "UNION" + Environment.NewLine;
				}

				cmd.CommandText += "select 1 "
								+ " from " + GetTableName(unitType) + " PV "
								+ "	inner join dbo.tblOPCConnections O on PV.OPCConnectionGuid = O.[OPCConnectionGuid] "
								+ " where O.URL = @URL "
								+ "	and PV.OPCItemID = @OpcId " 
								+ " and PV."+ GetIdentityColumnName(unitType) + " <> @IdentityGuid ";
				firstPass = false;
			}

			cmd.Parameters.AddWithValue("@URL", url);
			cmd.Parameters.AddWithValue("@OpcId", opcId);
            cmd.Parameters.AddWithValue("@IdentityGuid", identityGuid);
        }
	}
	#endregion
}

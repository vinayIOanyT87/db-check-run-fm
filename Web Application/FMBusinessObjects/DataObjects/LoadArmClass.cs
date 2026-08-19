using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Globalization;

namespace FMBusinessObjects.DataObjects
{
	public enum PRESET_TYPE
	{
		ACCULOAD2_STD = 0,
		ACCULOAD2_SEQ = 1,
		ACCULOAD2_RBU = 2,
		ACCULOAD2_STM = 3,
		ACCULOAD2_SQR = 4,
		ACCULOAD2_RBM = 5,
		ACCULOADIII_S = 6,
		ACCULOADIII_Q = 7,
		MANUAL = 8,
		MICROLOAD_NET = 9,
		DANLOAD6000 = 10,
		MULTILOAD_II_SMP = 11,
		ACCULOADIII_SA = 12,
		CONTREC1010 = 13,
		MULTILOAD_II = 14,
		CONTREC1010_RA = 15,
        VARECDET = 16,
        MAX_PRESET_TYPE = 17
	};

   [Serializable]
   [CollectionDataContract]
	public class LoadArmCollectionClass : List<LoadArmClass>
	{
		public void RemoveByIndex(LoadArmClass loadArm)
		{
			int index = 0;
			foreach (LoadArmClass item in this)
			{
				if (item.IdentityGuid == loadArm.IdentityGuid)
				{
					this.RemoveAt(index);
					return;
				}

				index++;
			}
		}
	}

	/// <summary>
	/// Summary description for LoadArm.
	/// </summary>
	[Serializable()]
	[DataContract]
	public class LoadArmClass : BaseDataObject, IAlarmAndEventDiscovery
	{
		[DataMember]
		public string _LoadRackText;
		[DataMember]
		public Guid BayAStationGuid;
		[DataMember]
		public Guid BayBStationGuid;
		[DataMember]
		public bool Enabled;
		[DataMember]
		public bool SwingArm;
		[DataMember]
		public PRESET_TYPE PresetType;
		[DataMember]
		public int BayAArmNumber;
		[DataMember]
		public int BayBArmNumber;
		[DataMember]
		public ProductMapCollectionClass ProductRecipeCollection;
		[DataMember]
		public ProductMapCollectionClass AdditiveInjectorCollection;
		[DataMember]
		public ProductMapCollectionClass ComponentCollection;
        [DataMember]
		public ProductMapCollectionClass ExternalComponentCollection;
	    [DataMember]
	    public ProductMapCollectionClass FlowControlledAdditiveCollection;
		[DataMember]
		public ProcessVariableCollectionClass ProcessVariableCollection;
		[DataMember]
		public string BayAStationID;
		[DataMember]
		public string BayBStationID;
		[DataMember]
		public PermissivesClass LoadArmPermissives;
		[DataMember]
		public PermissivesClass NoAdditivePermissives;
        [DataMember]
        public ProductMapCollectionClass OffloadExternalProductCollection;

        public string Select = "SELECT tblLoadArms.*," +
									"(Select ID FROM tblStations WHERE tblStations.StationGuid = tblLoadArms.BayAStationGuid) AS BayAStationID," +
									"(Select ID FROM tblStations WHERE tblStations.StationGuid = tblLoadArms.BayBStationGuid) AS BayBStationID";

		public string LoadRackText { get { return this._LoadRackText; } set { this.SetString("Load Rack Text", 9, value, ref this._LoadRackText); } }


		public LoadArmClass()
		{
		    this.Reset();
		}

		public override string ID
		{
			get
			{
				this._ID = "";
				if (this.BayAStationGuid != Guid.Empty) this._ID = this.BayAStationID + " Arm " + this.BayAArmNumber;
				if (this.BayBStationGuid != Guid.Empty) this._ID += this.BayBStationID + " Arm " + this.BayBArmNumber;
				return this._ID;
			}
			set
			{
			    this._ID = value;
			}
		}

        private const string NoTankCapcityReminingKey = "No Tank Capcity Remining";
        private static readonly AlarmAndEventDescriptorClass NoTankCapcityReminingEventDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, NoTankCapcityReminingKey);

        private const string FallbackKey = "Process Variable fell back to last known good";
        private static readonly AlarmAndEventDescriptorClass FallbackAlarmDescriptor = new AlarmAndEventDescriptorClass(true, LoadRackKey, FallbackKey);

        private const string ConfigurationMismatchKey = "Load Arm Configuration Mismatch";
        private static readonly AlarmAndEventDescriptorClass ConfigurationMismatchAlarmDescriptor = new AlarmAndEventDescriptorClass(true, LoadRackKey, ConfigurationMismatchKey);

         #region Alarm and Event Descriptors
        AlarmAndEventDescriptorClass[] IAlarmAndEventDiscovery.AlarmAndEvents
        {
            get
            {
                AlarmAndEventDescriptorClass[] Descriptors = { NoTankCapcityReminingEventDescriptor,
                                                               FallbackAlarmDescriptor,
                                                               ConfigurationMismatchAlarmDescriptor
                                                                            };
                return Descriptors;
            }
        }

        public AlarmAndEventLogClass NoTankCapcityReminingEventString(string stationId, string pv)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(NoTankCapcityReminingEventDescriptor)
            {
                AssociatedData =
                    stationId + " " + this.ID + " - " + pv
            };
            return alarmAndEventLog;
        }

        public AlarmAndEventLogClass FallbackAlarm(string pv, int statusCode)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(FallbackAlarmDescriptor)
            {
                AssociatedData =
                                               this.ID + " - " + pv
                                               + ", Status code = "
                                               + statusCode.ToString(CultureInfo.InvariantCulture)
            };
            return alarmAndEventLog;
        }

        public AlarmAndEventLogClass FallbackAlarm(string stationId, string pv, int statusCode)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(FallbackAlarmDescriptor)
            {
                AssociatedData =
                                               stationId + " " + this.ID + " - " + pv
                                               + ", Status code = "
                                               + statusCode.ToString(CultureInfo.InvariantCulture)
            };
            return alarmAndEventLog;
        }

        public AlarmAndEventLogClass ConfigurationMismatchAlarm(string stationId, string mismatchDetails)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(ConfigurationMismatchAlarmDescriptor)
            {
                AssociatedData =
                    stationId + " " + this.ID + " - " + mismatchDetails
            };
            return alarmAndEventLog;
        }

        #endregion
        [XmlIgnoreAttribute]
		public override ENTITY_TYPE EntityType => ENTITY_TYPE.LOAD_ARM;

	    [XmlIgnoreAttribute]
		public override ENTITY_TYPE ParentEntityType => ENTITY_TYPE.NONE;

	    public static string PresetTypeID(PRESET_TYPE type)
		{
			switch (type)
			{
				case PRESET_TYPE.ACCULOAD2_STD:
					return "Accuload 2 STD";
				case PRESET_TYPE.ACCULOAD2_SEQ:
					return "Accuload 2 SEQ";
				case PRESET_TYPE.ACCULOAD2_RBU:
					return "Accuload 2 RBU";
				case PRESET_TYPE.ACCULOAD2_STM:
					return "Accuload 2 STM";
				case PRESET_TYPE.ACCULOAD2_SQR:
					return "Accuload 2 SQR";
				case PRESET_TYPE.ACCULOAD2_RBM:
					return "Accuload 2 RBM";
				case PRESET_TYPE.ACCULOADIII_S:
					return "Accuload III S";
				case PRESET_TYPE.ACCULOADIII_Q:
					return "Accuload III Q";
				case PRESET_TYPE.MANUAL:
					return "Manual";
				case PRESET_TYPE.MICROLOAD_NET:
					return "Microload.net";
				case PRESET_TYPE.DANLOAD6000:
					return "Danload 6000";
				case PRESET_TYPE.MULTILOAD_II_SMP:
					return "Multiload II SMP";
				case PRESET_TYPE.ACCULOADIII_SA:
					return "Accuload III SA";
				case PRESET_TYPE.CONTREC1010:
					return "Contrec 1010";
				case PRESET_TYPE.MULTILOAD_II:
					return "Multiload II";
				case PRESET_TYPE.CONTREC1010_RA:
					return "Contrec 1010RA";
				default:
					return "Undefined";
			}
		}

		public override void Reset()
		{
			base.Reset();
		    this._LoadRackText = "";
		    this.BayAStationGuid = Guid.Empty;
		    this.BayBStationGuid = Guid.Empty;
		    this.Enabled = true;
		    this.SwingArm = false;
		    this.PresetType = PRESET_TYPE.MAX_PRESET_TYPE;
		    this.ProductRecipeCollection = new ProductMapCollectionClass();
		    this.AdditiveInjectorCollection = new ProductMapCollectionClass();
		    this.ComponentCollection = new ProductMapCollectionClass();
		    this.ExternalComponentCollection = new ProductMapCollectionClass();
            this.FlowControlledAdditiveCollection = new ProductMapCollectionClass();
            this.OffloadExternalProductCollection = new ProductMapCollectionClass();
            this.ProcessVariableCollection = new ProcessVariableCollectionClass();
		    ProcessVariableClass processVariable = new ProcessVariableClass
		                                               {
		                                                   ProcessVariableType = PROCESS_VARIABLE_TYPE.LOADARM_PV,
		                                                   UnitType = UNIT_TYPE.LOADARM_UNIT
		                                               };
		    this.ProcessVariableCollection.Add(processVariable);
		    this.BayAStationID = "";
		    this.BayBStationID = "";
		    this.LoadArmPermissives = new PermissivesClass
		                                  {
		                                      InputUnitType = UNIT_TYPE.LOADARM_INPUT_PERMISSIVE,
		                                      OutputUnitType = UNIT_TYPE.LOADARM_OUTPUT_PERMISSIVE
		                                  };
		    this.NoAdditivePermissives = new PermissivesClass
		                                     {
		                                         InputUnitType = UNIT_TYPE.NOADDITIVE_INPUT_PERMISSIVE,
		                                         OutputUnitType = UNIT_TYPE.NOADDITIVE_OUTPUT_PERMISSIVE
		                                     };
		}

        public override void Load(Object o)
		{
		    this.Reset();

		    var set = o as DataSet;
		    if (set != null)
			{
				DataTable table = set.Tables[0];
				if (table.Rows.Count == 0)
					return;

				DataRow row = table.Rows[0];

			    // ReSharper disable RedundantTypeArgumentsOfMethod
			    this._IdentityGuid = DataObject.getValue<Guid>(row["LoadArmGuid"], Guid.Empty);
			    this._LoadRackText = DataObject.getValue<string>(row["LoadRackText"], "");
			    this.BayAStationGuid = DataObject.getValue<Guid>(row["BayAStationGuid"], Guid.Empty);
			    this.BayBStationGuid = DataObject.getValue<Guid>(row["BayBStationGuid"], Guid.Empty);
			    this.Enabled = DataObject.getValue<bool>(row["Enabled"], true);
			    this.SwingArm = DataObject.getValue<bool>(row["SwingArm"], false);
			    this.PresetType = DataObject.getValue<PRESET_TYPE>(row["LookupPresetTypeIndex"], PRESET_TYPE.MAX_PRESET_TYPE);
			    this.BayAArmNumber = DataObject.getValue<int>(row["BayAArmNumber"], 0);
			    this.BayBArmNumber = DataObject.getValue<int>(row["BayBArmNumber"], 0);
			    this.CreatedDate = DataObject.getValue<DateTimeOffset>(row["CreatedDate"], DateTimeOffset.Now);
			    this.CreatedBy = DataObject.getValue<string>(row["CreatedBy"], ADMIN);
			    this.UpdatedDate = DataObject.getValue<DateTimeOffset>(row["UpdatedDate"], this.CreatedDate);
			    this.UpdatedBy = DataObject.getValue<string>(row["UpdatedBy"], ADMIN);
			    this.BayAStationID = DataObject.getValue<string>(row["BayAStationID"], "");
			    this.BayBStationID = DataObject.getValue<string>(row["BayBStationID"], "");
                // ReSharper restore RedundantTypeArgumentsOfMethod
            }
            else if (o is LoadArmClass)
			{
				LoadArmClass loadArm = (LoadArmClass)o;

			    this._IdentityGuid = loadArm.IdentityGuid;
			    this._LoadRackText = loadArm.LoadRackText;
			    this.BayAStationGuid = loadArm.BayAStationGuid;
			    this.BayBStationGuid = loadArm.BayBStationGuid;
			    this.Enabled = loadArm.Enabled;
			    this.SwingArm = loadArm.SwingArm;
			    this.PresetType = loadArm.PresetType;
			    this.BayAArmNumber = loadArm.BayAArmNumber;
			    this.BayBArmNumber = loadArm.BayBArmNumber;
			    this._CreatedDate = loadArm.CreatedDate;
			    this._CreatedBy = loadArm.CreatedBy;
			    this._UpdatedDate = loadArm.UpdatedDate;
			    this._UpdatedBy = loadArm.UpdatedBy;
			    this.BayAStationID = loadArm.BayAStationID;
			    this.BayBStationID = loadArm.BayBStationID;

				foreach (ProductMapClass existingRecipe in loadArm.ProductRecipeCollection)
				{
					ProductMapClass newRecipe = new ProductMapClass();
					newRecipe.Load(existingRecipe);
				    this.ProductRecipeCollection.Add(newRecipe);
				}

				foreach (ProductMapClass existingComponent in loadArm.ComponentCollection)
				{
					ProductMapClass newComponent = new ProductMapClass(existingComponent);
					//newComponent.Load(existingComponent);
				    this.ComponentCollection.Add(newComponent);
				}

				foreach (ProductMapClass existingAdditive in loadArm.AdditiveInjectorCollection)
				{
					ProductMapClass newAdditive = new ProductMapClass(existingAdditive);
					//newAdditive.Load(existingAdditive);
				    this.AdditiveInjectorCollection.Add(newAdditive);
				}

				foreach (ProductMapClass existingExternalComponent in loadArm.ExternalComponentCollection)
				{
					ProductMapClass newExternalComponent = new ProductMapClass();
					newExternalComponent.Load(existingExternalComponent);
				    this.ExternalComponentCollection.Add(newExternalComponent);
				}

                foreach (ProductMapClass existingFlowControlledAdditive in loadArm.FlowControlledAdditiveCollection)
                {
                    ProductMapClass newFlowControlledAdditive = new ProductMapClass(existingFlowControlledAdditive);
                    //newFlowControlledAdditive.Load(existingFlowControlledAdditive);
                    this.FlowControlledAdditiveCollection.Add(newFlowControlledAdditive);
                }

                foreach (ProductMapClass offloadExternalProduct in loadArm.OffloadExternalProductCollection)
                {
                    var newOffloadExternalProduct = new ProductMapClass();
                    newOffloadExternalProduct.Load(offloadExternalProduct);
                    this.OffloadExternalProductCollection.Add(newOffloadExternalProduct);
                }

                this.ProcessVariableCollection.Clear();
				foreach (ProcessVariableClass existingProcessVariable in loadArm.ProcessVariableCollection)
				{
					ProcessVariableClass newProcessVariable = new ProcessVariableClass();
					newProcessVariable.Load(existingProcessVariable);
				    this.ProcessVariableCollection.Add(newProcessVariable);
				}

			    this.LoadArmPermissives.Load(loadArm.LoadArmPermissives);
			    this.NoAdditivePermissives.Load(loadArm.NoAdditivePermissives);
			}
			else
				base.Load(o);
		}

		public bool IsProductAvailable(Guid identityGuid)
		{
			foreach (ProductMapClass recipe in this.ProductRecipeCollection)
			{
				if (recipe.AssignedGuid == identityGuid
				&& recipe.Permissives.Permitted)
					return true;
			}

			return false;
		}

		public bool IsAdditiveProfileAvailable(AdditiveProfileClass additiveProfile)
		{
			ProductMapCollectionClass availableAdditiveInjectorCollection = new ProductMapCollectionClass();

			foreach (ProductMapClass additive in additiveProfile.AdditiveCollection)
			{
				if (additive.LockedOut)
					return false;

				ProductMapClass additiveInjector =
						  this.AdditiveInjectorCollection.Find(x => x.AssignedGuid == additive.AssignedGuid);

				if (additiveInjector == null)
					return false;

				if (!additiveInjector.Permissives.Permitted)
					return false;

				foreach (ProductMapClass existingAdditiveInjector in availableAdditiveInjectorCollection)
					if (additiveInjector.PresetNumber == existingAdditiveInjector.PresetNumber)
						return false;

				availableAdditiveInjectorCollection.Add(additiveInjector);
			}

			return true;
		}

		#region Paramaterized SQL Queries

		public void InsertSQL(SqlCommand cmd)
		{
			cmd.CommandText = "INSERT INTO tblLoadArms " +
				"(LoadRackText," +
				"BayAStationGuid," +
				"BayBStationGuid," +
				"[Enabled]," +
				"SwingArm," +
				"LookupPresetTypeIndex," +
				"BayAArmNumber," +
				"BayBArmNumber," +
				"CreatedDate," +
				"CreatedBy," +
				"UpdatedDate," +
				"UpdatedBy," +
				"LoadArmGuid" +
				") VALUES (" +
				"@LoadRackText," +
				"@BayAStationGuid," +
				"@BayBStationGuid," +
				"@Enabled," +
				"@SwingArm," +
				"@LookupPresetTypeIndex," +
				"@BayAArmNumber," +
				"@BayBArmNumber," +
				"@CreatedDate," +
				"@CreatedBy," +
				"@UpdatedDate," +
				"@UpdatedBy, " +
				"@LoadArmGuid)";

			cmd.Parameters.Add("@LoadRackText", SqlDbType.NVarChar, 9);
			cmd.Parameters.Add("@BayAStationGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@BayBStationGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@Enabled", SqlDbType.Bit);
			cmd.Parameters.Add("@SwingArm", SqlDbType.Bit);
			cmd.Parameters.Add("@LookupPresetTypeIndex", SqlDbType.Int);
			cmd.Parameters.Add("@BayAArmNumber", SqlDbType.Int);
			cmd.Parameters.Add("@BayBArmNumber", SqlDbType.Int);
			cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@LoadArmGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@LoadRackText"].Value = this._LoadRackText;

			if (this.BayAStationGuid != Guid.Empty)
			{
				cmd.Parameters["@BayAStationGuid"].Value = this.BayAStationGuid;
			}
			else
			{
				cmd.Parameters["@BayAStationGuid"].Value = DBNull.Value;
			}

			if (this.BayBStationGuid != Guid.Empty)
			{
				cmd.Parameters["@BayBStationGuid"].Value = this.BayBStationGuid;
			}
			else
			{
				cmd.Parameters["@BayBStationGuid"].Value = DBNull.Value;
			}

			cmd.Parameters["@Enabled"].Value = (this.Enabled ? 1 : 0);
			cmd.Parameters["@SwingArm"].Value = (this.SwingArm ? 1 : 0);
			cmd.Parameters["@LookupPresetTypeIndex"].Value = this.PresetType;

			if (this.BayAArmNumber != 0)
			{
				cmd.Parameters["@BayAArmNumber"].Value = this.BayAArmNumber;
			}
			else
			{
				cmd.Parameters["@BayAArmNumber"].Value = DBNull.Value;
			}

			if (this.BayBArmNumber != 0)
			{
				cmd.Parameters["@BayBArmNumber"].Value = this.BayBArmNumber;
			}
			else
			{
				cmd.Parameters["@BayBArmNumber"].Value = DBNull.Value;
			}

			cmd.Parameters["@CreatedDate"].Value = this.CreatedDate;
			cmd.Parameters["@CreatedBy"].Value = this.CreatedBy;
			cmd.Parameters["@UpdatedDate"].Value = this.UpdatedDate;
			cmd.Parameters["@UpdatedBy"].Value = this.UpdatedBy;
			cmd.Parameters["@LoadArmGuid"].Value = this._IdentityGuid;
		}

		public void UpdateSQL(SqlCommand cmd)
		{
			cmd.CommandText = "UPDATE tblLoadArms " +
				" SET LoadRackText= @LoadRackText, " +
				" BayAStationGuid = @BayAStationGuid, " +
				" BayBStationGuid = @BayBStationGuid, " +
				" [Enabled] = @Enabled, " +
				" SwingArm = @SwingArm, " +
				" LookupPresetTypeIndex = @LookupPresetTypeIndex, " +
				" BayAArmNumber = @BayAArmNumber, " +
				" BayBArmNumber = @BayBArmNumber, " +
				" UpdatedDate = @UpdatedDate, " +
				" UpdatedBy = @UpdatedBy " +
				" WHERE LoadArmGuid = @LoadArmGuid";

			cmd.Parameters.Add("@LoadRackText", SqlDbType.NVarChar, 9);
			cmd.Parameters.Add("@BayAStationGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@BayBStationGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@Enabled", SqlDbType.Bit);
			cmd.Parameters.Add("@SwingArm", SqlDbType.Bit);
			cmd.Parameters.Add("@LookupPresetTypeIndex", SqlDbType.Int);
			cmd.Parameters.Add("@BayAArmNumber", SqlDbType.Int);
			cmd.Parameters.Add("@BayBArmNumber", SqlDbType.Int);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@LoadArmGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@LoadRackText"].Value = this._LoadRackText;

			if (this.BayAStationGuid != Guid.Empty)
			{ 
				cmd.Parameters["@BayAStationGuid"].Value = this.BayAStationGuid; 
			}
			else
			{ 
				cmd.Parameters["@BayAStationGuid"].Value = DBNull.Value; 
			}

			if (this.BayBStationGuid != Guid.Empty)
			{ 
				cmd.Parameters["@BayBStationGuid"].Value = this.BayBStationGuid; 
			}
			else
			{
				cmd.Parameters["@BayBStationGuid"].Value = DBNull.Value;
			}

			cmd.Parameters["@Enabled"].Value = (this.Enabled ? 1 : 0);
			cmd.Parameters["@SwingArm"].Value = (this.SwingArm ? 1 : 0);
			cmd.Parameters["@LookupPresetTypeIndex"].Value = this.PresetType;

			if (this.BayAArmNumber != 0)
			{ 
				cmd.Parameters["@BayAArmNumber"].Value = this.BayAArmNumber; 
			}
			else
			{ 
				cmd.Parameters["@BayAArmNumber"].Value = DBNull.Value; 
			}

			if (this.BayBArmNumber != 0)
			{ 
				cmd.Parameters["@BayBArmNumber"].Value = this.BayBArmNumber;
			}
			else
			{ 
				cmd.Parameters["@BayBArmNumber"].Value = DBNull.Value; 
			}

			cmd.Parameters["@UpdatedDate"].Value = this.UpdatedDate;
			cmd.Parameters["@UpdatedBy"].Value = this.UpdatedBy;
			cmd.Parameters["@LoadArmGuid"].Value = this.IdentityGuid;
		}

		public void PurgeSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM tblLoadArms WHERE LoadArmGuid = @LoadArmGuid";
			cmd.Parameters.Add("@LoadArmGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@LoadArmGuid"].Value = this.IdentityGuid;
		}

		public void SelectSQL(SqlCommand cmd, bool bInTransaction)
		{
			cmd.CommandText = this.Select + " FROM tblLoadArms " + SQLUpdateLock(bInTransaction) +
				" WHERE LoadArmGuid = @LoadArmGuid";
			cmd.Parameters.Add("@LoadArmGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@LoadArmGuid"].Value = this.IdentityGuid;
		}

		public void SelectByStationGuidsAndArmNumbersSQL(SqlCommand cmd, bool bInTransaction)
		{
			cmd.CommandText = this.Select + " FROM tblLoadArms " + SQLUpdateLock(bInTransaction);

			if (this.BayAStationGuid != Guid.Empty)
			{
				cmd.CommandText = cmd.CommandText + " WHERE BayAStationGuid =  @BayAStationGuid ";
				cmd.Parameters.Add("@BayAStationGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@BayAStationGuid"].Value = this.BayAStationGuid;
			}
			else
			{ cmd.CommandText = cmd.CommandText + " WHERE BayAStationGuid IS NULL "; }

			if (this.BayBStationGuid != Guid.Empty)
			{
				cmd.CommandText = cmd.CommandText + " AND BayBStationGuid =  @BayBStationGuid ";
				cmd.Parameters.Add("@BayBStationGuid", SqlDbType.UniqueIdentifier);
				cmd.Parameters["@BayBStationGuid"].Value = this.BayBStationGuid;
			}
			else
			{ cmd.CommandText = cmd.CommandText + "AND BayBStationGuid IS NULL "; }

			if (this.BayAArmNumber != 0)
			{
				cmd.CommandText = cmd.CommandText + " AND BayAArmNumber = @BayAArmNumber ";
				cmd.Parameters.Add("@BayAArmNumber", SqlDbType.Int);
				cmd.Parameters["@BayAArmNumber"].Value = this.BayAArmNumber;
			}
			else
			{ cmd.CommandText = cmd.CommandText + " AND BayAArmNumber IS NULL"; }

			if (this.BayBArmNumber != 0)
			{
				cmd.CommandText = cmd.CommandText + " AND BayBArmNumber = @BayBArmNumber ";
				cmd.Parameters.Add("@BayBArmNumber", SqlDbType.Int);
				cmd.Parameters["@BayBArmNumber"].Value = this.BayBArmNumber;
			}
			else
			{ cmd.CommandText = cmd.CommandText + " AND BayBArmNumber IS NULL "; }
		}

		public void EnumerateByStationGuidSQL(SqlCommand cmd, bool swingArmPosition, Guid stationGuid, bool bInTransaction)
		{


			if (swingArmPosition)
			{
				cmd.CommandText = this.Select +
						  " FROM tblLoadArms " + SQLUpdateLock(bInTransaction) +
						  " WHERE BayAStationGuid = @StationGuid " +
						  " ORDER BY BayAArmNumber";
			}
			else
			{
				cmd.CommandText = this.Select +
						" FROM tblLoadArms " + SQLUpdateLock(bInTransaction) +
						" WHERE BayBStationGuid = @StationGuid " +
						" ORDER BY BayBArmNumber";
			}
			cmd.Parameters.Add("@StationGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@StationGuid"].Value = stationGuid;
		}

		#endregion

	}
}

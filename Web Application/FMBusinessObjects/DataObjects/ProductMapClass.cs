 #pragma warning disable 1587
///***************************************************************************
/// Module Name:  ProductMapClass.cs
/// Author:       
/// Copyright (c) Varec, Inc.  All rights reserved.
///***************************************************************************
#pragma warning restore 1587


namespace FMBusinessObjects.DataObjects
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Xml;
    using System.Xml.Serialization;

    using Varec.CommonComponents.EngineeringUnitsLibrary;

    public enum PRODUCT_MAP_TYPE
	{
		UNDEFINED_MAP = 0,
		BLEND_COMPONENT_MAP = 1,
		PRODUCT_GROUP_MAP = 2,
		PRESET_RECIPE_MAP = 3,
		PRESET_INJECTOR_MAP = 4,
		ADDITIVE_PROFILE_MAP = 5,
		PRODUCT_COMPANY_MAP = 6,
		PRESET_COMPONENT_TANK_MAP = 7,
		TRANSACTION_ALIAS_EXCLUSION_MAP = 8,
		PRODUCT_COMPANY_GROUP_MAP = 9,
		PRESET_COMPONENT_TANKGROUP_MAP = 10,
		UNAVAILABLE_INVENTORY_COMPANY_MAP = 11,		// added (IGO 02-Sep-2008)
		PRESET_EXTERNAL_COMPONENT_MAP = 12,
		SUPPLIER_PRODUCT_COMPANY_MAP = 13,
		LEDGER_VIEW_MAP = 14,
        PRESET_FLOW_CONTROLLED_ADDITIVE_MAP = 15,
        OFFLOAD_EXTERNAL_METER_MAP = 16,
        VRU_VCU_TRACKING = 17,
        PRESET_EXTERNAL_COMPONENT_TANKGROUP_MAP = 18,
        MAX_MAP = 19,
    };

	[Serializable]
	[CollectionDataContract]
	public class ProductMapCollectionClass : List<ProductMapClass>
	{
    }

		[Serializable]
		[DataContract]
		public class ProductMapClass : BaseDataObject
		{
		[DataMember]
		[XmlIgnore]
		public Guid AssignedGuid { get; set; }

		[EntityImportExport("ID*", 130, "AssignedID")]
		[DataMember]
		public string AssignedID { get; set; }

		[DataMember]
		[XmlIgnore]
		public Guid AssignedToGuid { get; set; }

		[DataMember]
		[XmlIgnore]
		public Guid ProductMasterRecordGuid { get; set; }

		[DataMember]
		private PRODUCT_MAP_TYPE type;

		[DataMember]
		public int Sequence { get; set; }

		[EntityImportExport("PERCENTAGE", 100, "BlendPercentage")]
		[DataMember]
		public double BlendPercentage { get; set; }

		[DataMember]
		public double Ratio { get; set; }

		[DataMember]
		public double Tolerance { get; set; }

		// Recipe, Injector, or Component
		[DataMember]
		public int PresetNumber { get; set; }

		[DataMember]
		[XmlIgnore]
		public Guid AdditiveProfileGuid { get; set; }

		[DataMember]
		[XmlIgnore]
		public Guid TankOrGroupGuid { get; set; }

		[DataMember]
		[XmlIgnore]
		public bool EnableRecipe { get; set; }

		[DataMember]
		[XmlIgnore]
		public string SpecialInstructions { get; set; }

		[DataMember]
		public PermissivesClass Permissives { get; set; }

		[DataMember]
		public ProcessVariableCollectionClass ProcessVariableCollection { get; set; }

		[DataMember]
		[XmlIgnore]
		public string AssignedCode { get; set; }

		[DataMember]
		[XmlIgnore]
		public string AssignedDescription { get; set; }

		[DataMember]
		public ProductType AssignedProductType { get; set; }

		[DataMember]
		public string AssignedLoadRackDisplayText { get; set; }

		[DataMember]
		[XmlIgnore]
		public bool LockedOut { get; set; }

		[DataMember]
		[XmlIgnore]
		public bool HazardousMaterial { get; set; }

		[DataMember]
		[XmlIgnore]
		public bool LoadByWeight { get; set; }

		/// <summary>
		/// Return the MeterID from the meter. 
		/// </summary>
		public string MeterID
		{
			get
			{
					return this._MeterID;
			}
			set
			{
					this.SetString("Meter", 20, value, ref this._MeterID);
			}
		}

		[DataMember]
		public MeterClass Meter;

		[DataMember]
		[XmlIgnore]
		public Guid AssignedToMeterGuid = Guid.Empty;

		[DataMember]
		public string PIDXProductCode { get; set; }

		[DataMember]
		[XmlIgnore]
		public string ContaminationPromptLoadRackText { get; set; }

		[EntityImportExport("ADDITIVEPROFILEID*", 130, "AdditiveProfileID")]
		[DataMember]
		public string AdditiveProfileID { get; set; }

		[DataMember]
		public string TankOrGroupID { get; set; }

		[EntityImportExport("ID*", 130, "AssignedToID")]
		[DataMember]
		public string AssignedToID { get; set; }

		[DataMember]
		[XmlIgnore]
		public string AssignedToName { get; set; }

		[DataMember]
		[XmlIgnore]
		public string AssignedToAddress { get; set; }

		[DataMember]
		[XmlIgnore]
		public string AssignedToCity { get; set; }

		[DataMember]
		[XmlIgnore]
		public string AssignedToState { get; set; }

		// Location to store non-resetable totals during loading
		[DataMember]
		[XmlIgnore]
		public double MeterValue { get; set; }

		[DataMember]
		public SIDouble _UnavailableInventoryGross;		// added (IGO 02-Sep-2008)		

		[DataMember]
		public SIDouble _UnavailableInventoryNet;		// added (IGO 02-Sep-2008)

		[DataMember]
		protected string _MeterID;
		[DataMember]
		protected string _ShipToProductID;
		[DataMember]
		protected string _ShipToProductCode;
		[DataMember]
		public SIDouble _AdditiveRate;
		[DataMember]
		public SIDouble _AdditiveCycleVolume;
		[DataMember]
		[XmlIgnore]
		public double DesiredTreatRate { get; set; }
		[DataMember]
		protected string _ShipToLoadRackDisplayText;

		[XmlIgnore]
		public string AdditiveRate { get { return this._AdditiveRate.ToString(); } set { this.SetSIDouble("Additive Rate", value, ref this._AdditiveRate); } }

		[XmlIgnore]
		public string AdditiveCycleVolume { get { return this._AdditiveCycleVolume.ToString(); } set { this.SetSIDouble("Additive Cycle Volume", value, ref this._AdditiveCycleVolume); } }

		[EntityImportExport("UNAVAILABLEGROSS", 130, "UnavailableInventoryGross")]
		[XmlIgnore]
		public string UnavailableInventoryGross { get { return this._UnavailableInventoryGross.ToString(); } set { this.SetSIDouble("Unavailable Inventory Gross", value, ref this._UnavailableInventoryGross); } }

		[EntityImportExport("UNAVAILABLENET", 130, "UnavailableInventoryNet")]
		[XmlIgnore]
		public string UnavailableInventoryNet { get { return this._UnavailableInventoryNet.ToString(); } set { this.SetSIDouble("Unavailable Inventory Net", value, ref this._UnavailableInventoryNet); } }

		[EntityImportExport("INSTRUCTIONS", 130, "Note")]
		public string Note { get { return this.SpecialInstructions; } set { this.SpecialInstructions = value; } }

		[DataMember]
		public string PIDXFamilyCode { get; set; }

		[DataMember]
		public bool IsEthanol { get; set; }

		public static string GetMappingTableName(PRODUCT_MAP_TYPE productMapType)
		{
			const string SchemaPrefix = "map.";

			switch (productMapType)
			{
				case PRODUCT_MAP_TYPE.UNDEFINED_MAP:
					return "Unknown";
				case PRODUCT_MAP_TYPE.BLEND_COMPONENT_MAP:
					return SchemaPrefix + "tblProductToBlendComponent";
				case PRODUCT_MAP_TYPE.PRODUCT_GROUP_MAP:
					return SchemaPrefix + "tblProductToProductGroup";
				case PRODUCT_MAP_TYPE.PRESET_RECIPE_MAP:
					return SchemaPrefix + "tblProductToPresetRecipe";
				case PRODUCT_MAP_TYPE.PRESET_INJECTOR_MAP:
					return SchemaPrefix + "tblProductToPresetInjector";
				case PRODUCT_MAP_TYPE.ADDITIVE_PROFILE_MAP:
					return SchemaPrefix + "tblProductToAdditiveProfile";
				case PRODUCT_MAP_TYPE.PRODUCT_COMPANY_MAP:
					return SchemaPrefix + "tblProductToCompany";
				case PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANK_MAP:
				case PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANKGROUP_MAP:
					return SchemaPrefix + "tblProductToPresetComponentTankOrTankGroup";
				case PRODUCT_MAP_TYPE.TRANSACTION_ALIAS_EXCLUSION_MAP:
					return SchemaPrefix + "tblProductToTransactionAliasExclusion";
				case PRODUCT_MAP_TYPE.PRODUCT_COMPANY_GROUP_MAP:
					return SchemaPrefix + "tblProductToCompanyGroup";
				case PRODUCT_MAP_TYPE.UNAVAILABLE_INVENTORY_COMPANY_MAP:
					return SchemaPrefix + "tblProductToUnavailableInventoryCompany";
				case PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_MAP:
				case PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_TANKGROUP_MAP:
					return SchemaPrefix + "tblProductToPresetExternalComponent";
				case PRODUCT_MAP_TYPE.SUPPLIER_PRODUCT_COMPANY_MAP:
					return SchemaPrefix + "tblProductToSupplierProductCompany";
				case PRODUCT_MAP_TYPE.LEDGER_VIEW_MAP:
					return SchemaPrefix + "tblProductToLedgerView";
				case PRODUCT_MAP_TYPE.PRESET_FLOW_CONTROLLED_ADDITIVE_MAP:
					return SchemaPrefix + "tblProductToPresetFlowControlledAdditive";
				case PRODUCT_MAP_TYPE.OFFLOAD_EXTERNAL_METER_MAP:
					return SchemaPrefix + "tblProductToOffloadExternalMeter";
				case PRODUCT_MAP_TYPE.VRU_VCU_TRACKING:
					return SchemaPrefix + "tblProductToVruTrackingConfig";
				case PRODUCT_MAP_TYPE.MAX_MAP:
					return "Unknown";
				default:
					return "Unknown";
			}
		}      

      public static string GetIdentityColumnName(PRODUCT_MAP_TYPE productMapType)
		{
			switch (productMapType)
			{
				case PRODUCT_MAP_TYPE.UNDEFINED_MAP:
					return "Unknown";
				case PRODUCT_MAP_TYPE.BLEND_COMPONENT_MAP:
					return "ProductToBlendComponentGuid";
				case PRODUCT_MAP_TYPE.PRODUCT_GROUP_MAP:
					return "ProductToProductGroupGuid";
				case PRODUCT_MAP_TYPE.PRESET_RECIPE_MAP:
					return "ProductToPresetRecipeGuid";
				case PRODUCT_MAP_TYPE.PRESET_INJECTOR_MAP:
					return "ProductToPresetInjectorGuid";
				case PRODUCT_MAP_TYPE.ADDITIVE_PROFILE_MAP:
					return "ProductToAdditiveProfileGuid";
				case PRODUCT_MAP_TYPE.PRODUCT_COMPANY_MAP:
					return "ProductToCompanyGuid";
				case PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANK_MAP:
					return "ProductToPresetComponentTankOrTankGroupGuid";
				case PRODUCT_MAP_TYPE.TRANSACTION_ALIAS_EXCLUSION_MAP:
					return "ProductToTransactionAliasExclusionGuid";
				case PRODUCT_MAP_TYPE.PRODUCT_COMPANY_GROUP_MAP:
					return "ProductToCompanyGroupGuid";
				case PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANKGROUP_MAP:
					return "ProductToPresetComponentTankOrTankGroupGuid";
				case PRODUCT_MAP_TYPE.UNAVAILABLE_INVENTORY_COMPANY_MAP:
					return "ProductToUnavailableInventoryCompanyGuid";
				case PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_MAP:
                case PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_TANKGROUP_MAP:
					return "ProductToPresetExternalComponentGuid";
				case PRODUCT_MAP_TYPE.SUPPLIER_PRODUCT_COMPANY_MAP:
					return "ProductToSupplierProductCompanyGuid";
				case PRODUCT_MAP_TYPE.LEDGER_VIEW_MAP:
					return "ProductToLedgerViewGuid";
                case PRODUCT_MAP_TYPE.PRESET_FLOW_CONTROLLED_ADDITIVE_MAP:
                    return "ProductToPresetFlowControlledAdditiveGuid";
                case PRODUCT_MAP_TYPE.OFFLOAD_EXTERNAL_METER_MAP:
                    return "ProductToOffloadExternalMeterGuid";
                case PRODUCT_MAP_TYPE.VRU_VCU_TRACKING:
			        return "ProductToVruTrackingConfigGuid";
                case PRODUCT_MAP_TYPE.MAX_MAP:
					return "Unknown";
				default:
					return "Unknown";
			}
		}

		public static string GetTankOrTankGroupColumnName(PRODUCT_MAP_TYPE productMapType)
		{
			switch (productMapType)
			{
				case PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANKGROUP_MAP:
                case PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_TANKGROUP_MAP:
					return "TankGroupApplicationStringGuid";
				default:
					return "TankGuid";
			}
		}

		public static string GetAssignedToColumnName(PRODUCT_MAP_TYPE productMapType)
		{
			switch (productMapType)
			{
				case PRODUCT_MAP_TYPE.UNDEFINED_MAP:
					return "Unknown";
				case PRODUCT_MAP_TYPE.BLEND_COMPONENT_MAP:
					return "AssignedToProductGuid";
				case PRODUCT_MAP_TYPE.PRODUCT_GROUP_MAP:
					return "AssignedToApplicationStringGuid";
				case PRODUCT_MAP_TYPE.PRESET_RECIPE_MAP:
					return "AssignedToLoadArmGuid";
				case PRODUCT_MAP_TYPE.PRESET_INJECTOR_MAP:
					return "AssignedToLoadArmGuid";
				case PRODUCT_MAP_TYPE.ADDITIVE_PROFILE_MAP:
					return "AssignedToAdditiveProfileGuid";
				case PRODUCT_MAP_TYPE.PRODUCT_COMPANY_MAP:
					return "AssignedToCompanyGuid";
				case PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANK_MAP:
				case PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANKGROUP_MAP:
					return "AssignedToLoadArmGuid";
				case PRODUCT_MAP_TYPE.TRANSACTION_ALIAS_EXCLUSION_MAP:
					return "AssignedToTransactionAliasGuid";
				case PRODUCT_MAP_TYPE.PRODUCT_COMPANY_GROUP_MAP:
					return "AssignedToApplicationStringGuid";
				case PRODUCT_MAP_TYPE.UNAVAILABLE_INVENTORY_COMPANY_MAP:
					return "AssignedToCompanyGuid";
				case PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_MAP:
                case PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_TANKGROUP_MAP:
					return "AssignedToLoadArmGuid";
				case PRODUCT_MAP_TYPE.SUPPLIER_PRODUCT_COMPANY_MAP:
					return "AssignedToCompanyGuid";
				case PRODUCT_MAP_TYPE.LEDGER_VIEW_MAP:
					return "AssignedToListViewGuid";
                case PRODUCT_MAP_TYPE.PRESET_FLOW_CONTROLLED_ADDITIVE_MAP:
                    return "AssignedToLoadArmGuid";
                case PRODUCT_MAP_TYPE.OFFLOAD_EXTERNAL_METER_MAP:
                    return "AssignedToLoadArmGuid";
                case PRODUCT_MAP_TYPE.VRU_VCU_TRACKING:
			        return "AssignedToSiteGuid";
                case PRODUCT_MAP_TYPE.MAX_MAP:
					return "Unknown";
				default:
					return "Unknown";
			}
		}

		/// <summary>
		/// Determine if this is a type of product map that can have a meter assigned to it.
		/// </summary>
		/// <param name="productMapType">The type of product map to check </param>
		/// <returns>True if the type is a type that can have a meter assigned to it. False otherwise</returns>
		public static bool ContainsMeter(PRODUCT_MAP_TYPE productMapType)
		{
			if (productMapType == PRODUCT_MAP_TYPE.PRESET_INJECTOR_MAP
					|| productMapType == PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANK_MAP
					|| productMapType == PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANKGROUP_MAP
                    || productMapType == PRODUCT_MAP_TYPE.PRESET_FLOW_CONTROLLED_ADDITIVE_MAP
                    || productMapType == PRODUCT_MAP_TYPE.OFFLOAD_EXTERNAL_METER_MAP)
			{
				return true;
			}

			return false;
		}

		public static bool ContainsSpecialInstructions(PRODUCT_MAP_TYPE productMapType)
		{
			if (productMapType == PRODUCT_MAP_TYPE.PRODUCT_COMPANY_MAP
					|| productMapType == PRODUCT_MAP_TYPE.PRODUCT_COMPANY_GROUP_MAP)
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Return the types of product maps which can have a meter
		/// </summary>
		/// <returns>The types of product maps which can have a meter</returns>
		public static ArrayList GetMeterTypes()
		{
		    ArrayList supportedTypes = new ArrayList
		                                   {
		                                       PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANK_MAP,
		                                       PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANKGROUP_MAP,
                                               PRODUCT_MAP_TYPE.PRESET_INJECTOR_MAP,
                                               PRODUCT_MAP_TYPE.PRESET_FLOW_CONTROLLED_ADDITIVE_MAP/*,
                                               PRODUCT_MAP_TYPE.OFFLOAD_EXTERNAL_METER_MAP*/
                                           };


		    return supportedTypes;
		}

		/// <summary>
		/// Return product map types that have an associated database table
		/// </summary>
		/// <returns>product map types that are valid, i.e. they are associated with a database table</returns>
		public static List<PRODUCT_MAP_TYPE> GetValidProductMapTypes()
		{
			List<PRODUCT_MAP_TYPE> productMapTypes = new List<PRODUCT_MAP_TYPE>();
			foreach (PRODUCT_MAP_TYPE mapType in Enum.GetValues(typeof(PRODUCT_MAP_TYPE)))
			{
				if (mapType != PRODUCT_MAP_TYPE.MAX_MAP && mapType != PRODUCT_MAP_TYPE.UNDEFINED_MAP)
				{
					productMapTypes.Add(mapType);
				}
			}

			return productMapTypes;
		}

		private string Select()
		{
			return "SELECT " + GetMappingTableName(this.Type) + ".* , " +
						"tblProducts.ProductID AS AssignedID, " +
						"tblProducts.ProductCode AS AssignedCode, " +
						"tblProducts.Description AS AssignedDescription, " +
						"tblProducts.LookupProductTypeIndex AS AssignedProductType, " +
						"tblProducts.LoadRackDisplayText AS AssignedLoadRackDisplayText, " +
						"tblProducts.LockedOut AS LockedOut, " +
						"tblProducts.HazardousMaterial AS HazardousMaterial, " +
						"tblProducts.LoadByWeight AS LoadByWeight, " +
						"tblProducts.PIDXCode AS PIDXCode, " +
						"tblProducts.PIDXFamilyCode AS PIDXFamilyCode, " +
						"tblProducts.IsEthanol AS IsEthanol, " +
						"tblProducts.ContaminationPromptLoadRackText AS ContaminationPromptLoadRackText, " +
						"tblAdditiveProfiles.ID AS AdditiveProfileID, ";
		}

        /// <summary>
        /// Prepare a statement to append to the FROM to join to tables we need to get additional information from
        /// </summary>
        /// <returns>The join SQL</returns>
        private string Join()
		{
			string tanksJoin = (this.Type != PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANKGROUP_MAP && this.Type != PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_TANKGROUP_MAP) ?
				" LEFT JOIN tblTanks ON tblTanks.TankGuid = " + GetMappingTableName(this.Type) + ".TankGuid " :
				" LEFT JOIN tblTankGroups ON tblTankGroups.TankGroupGuid = " + GetMappingTableName(this.Type) + ".TankGroupApplicationStringGuid ";

            string productTableJoin = " INNER JOIN [erv].[udf_GetProductRecordVersions](@TargetSiteGuid) a ON a.MasterRecordGuid = " + GetMappingTableName(this.Type) + ".ProductGuid " +
                                      " INNER JOIN tblProducts ON tblProducts.ProductGuid = a.ProductGuid ";

			string assignedToIDJoin;

			if (this.Type == PRODUCT_MAP_TYPE.PRODUCT_COMPANY_MAP
				|| this.Type == PRODUCT_MAP_TYPE.SUPPLIER_PRODUCT_COMPANY_MAP
				|| this.Type == PRODUCT_MAP_TYPE.UNAVAILABLE_INVENTORY_COMPANY_MAP)
			{
                //Those mappings are external attributes of Products  and Companies and are supported by RecordVersioning-aware Stored Procedures.
                productTableJoin = "";
                assignedToIDJoin = "";
			}
			else if (this.Type == PRODUCT_MAP_TYPE.BLEND_COMPONENT_MAP)
			{
                productTableJoin = " INNER JOIN tblProducts ON tblProducts._MasterRecordGuid = map.tblProductToBlendComponent.ProductGuid " + 
                                   " AND tblProducts.ProductGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Product', tblProducts._MasterRecordGuid, @TargetSiteGuid) ";
                assignedToIDJoin = " LEFT JOIN tblProducts AssignedToProducts ON AssignedToProducts._MasterRecordGuid = map.tblProductToBlendComponent.AssignedToProductGuid " +
                                   " AND AssignedToProducts.ProductGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Product', AssignedToProducts._MasterRecordGuid, @TargetSiteGuid) ";
			}
			else if (this.Type == PRODUCT_MAP_TYPE.ADDITIVE_PROFILE_MAP)
			{
				assignedToIDJoin = " LEFT JOIN tblAdditiveProfiles AssignedToAdditiveProfiles ON AssignedToAdditiveProfiles.AdditiveProfileGuid = " + GetMappingTableName(this.Type) + "." + GetAssignedToColumnName(this.Type) + " ";
			}
            else if (this.Type == PRODUCT_MAP_TYPE.PRODUCT_GROUP_MAP)
            {
                assignedToIDJoin = " LEFT JOIN tblApplicationString AssignedToApplicationString ON AssignedToApplicationString.ApplicationStringGuid = " + GetMappingTableName(this.Type) + "." + GetAssignedToColumnName(this.Type) + " ";
            }
			else if (this.Type == PRODUCT_MAP_TYPE.PRODUCT_COMPANY_GROUP_MAP)
			{
                //Those mappings are external attributes of Products and use the exact Product child record version Guid instead of the Product MasterRecordGuid.
                productTableJoin = " INNER JOIN [erv].[udf_GetProductRecordVersions](@TargetSiteGuid) a ON a.ProductGuid = " + GetMappingTableName(this.Type) + ".ProductGuid " +
                                     " INNER JOIN tblProducts ON tblProducts.ProductGuid = a.ProductGuid ";
				assignedToIDJoin = " LEFT JOIN tblApplicationString AssignedToApplicationString ON AssignedToApplicationString.ApplicationStringGuid = " + GetMappingTableName(this.Type) + "." + GetAssignedToColumnName(this.Type) + " ";
			}
			else if (this.Type == PRODUCT_MAP_TYPE.TRANSACTION_ALIAS_EXCLUSION_MAP)
			{
                //Those mappings are external attributes of Products and use the exact Product child record version Guid instead of the Product MasterRecordGuid.
                productTableJoin = " INNER JOIN [erv].[udf_GetProductRecordVersions](@TargetSiteGuid) a ON a.ProductGuid = " + GetMappingTableName(this.Type) + ".ProductGuid " +
                                     " INNER JOIN tblProducts ON tblProducts.ProductGuid = a.ProductGuid ";
                assignedToIDJoin = " LEFT OUTER JOIN [erv].[udf_GetTransactionAliasRecordVersions](@TargetSiteGuid) b ON b.TransactionAliasGuid = " + GetMappingTableName(this.Type) + "." + GetAssignedToColumnName(this.Type) +
                                     " LEFT OUTER JOIN tblTransactionAliases AssignedToTransactionAliases ON AssignedToTransactionAliases.TransactionAliasGuid = b.TransactionAliasGuid ";
			}
			else
			{
				assignedToIDJoin = string.Empty;
			}

            return productTableJoin +                
				" LEFT JOIN tblAdditiveProfiles ON tblAdditiveProfiles.AdditiveProfileGuid = " + GetMappingTableName(this.Type) + ".AdditiveProfileGuid "
				+ tanksJoin + assignedToIDJoin;
		}


		public PRODUCT_MAP_TYPE Type
		{
			get
			{
				return this.type;
			}

			set
			{
			    if (this.ProcessVariableCollection == null)
			    {
			        this.ProcessVariableCollection = new ProcessVariableCollectionClass();
			    }
			    else
			    {
			        this.ProcessVariableCollection.Clear();
			    }

			    if (this.Permissives == null)
			    {
			        return;
			    }

				this.type = value;

                switch (this.type)
                {
                    case PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANK_MAP:
                    case PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANKGROUP_MAP:
				        this.Permissives.InputUnitType = UNIT_TYPE.COMPONENT_INPUT_PERMISSIVE;
				        this.Permissives.OutputUnitType = UNIT_TYPE.COMPONENT_OUTPUT_PERMISSIVE;
                        break;
                    case PRODUCT_MAP_TYPE.PRESET_INJECTOR_MAP:
				        this.Permissives.InputUnitType = UNIT_TYPE.ADDITIVE_INPUT_PERMISSIVE;
				        this.Permissives.OutputUnitType = UNIT_TYPE.ADDITIVE_OUTPUT_PERMISSIVE;
                        break;
                    case PRODUCT_MAP_TYPE.PRESET_RECIPE_MAP:
				        this.Permissives.InputUnitType = UNIT_TYPE.RECIPE_INPUT_PERMISSIVE;
				        this.Permissives.OutputUnitType = UNIT_TYPE.RECIPE_OUTPUT_PERMISSIVE;
                        break;
                    case PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_TANKGROUP_MAP:
                    case PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_MAP:
					    ProcessVariableClass percentagePv = new ProcessVariableClass(PROCESS_VARIABLE_TYPE.BLEND_PERCENTAGE_PV, UNIT_TYPE.PRODUCT_MAP_PRESET_EXTERNAL_COMPONENT, VarEnum.VT_I2, true, "", "", "");
				        this.ProcessVariableCollection.Add(percentagePv);
				        this.Permissives.InputUnitType = UNIT_TYPE.EXTERNAL_COMPONENT_INPUT_PERMISSIVE;
				        this.Permissives.OutputUnitType = UNIT_TYPE.EXTERNAL_COMPONENT_OUTPUT_PERMISSIVE;
                        break;
                    case PRODUCT_MAP_TYPE.PRESET_FLOW_CONTROLLED_ADDITIVE_MAP:
                        this.Permissives.InputUnitType = UNIT_TYPE.FLOW_CONTROLLED_ADDITIVE_INPUT_PERMISSIVE;
                        this.Permissives.OutputUnitType = UNIT_TYPE.FLOW_CONTROLLED_ADDITIVE_OUTPUT_PERMISSIVE;
                        break;
                    case PRODUCT_MAP_TYPE.OFFLOAD_EXTERNAL_METER_MAP:
                        var meterPv = new ProcessVariableClass(PROCESS_VARIABLE_TYPE.COMPONENT_METER_FLOW_TOTAL_PV, UNIT_TYPE.PRODUCT_MAP_OFFLOAD_EXTERNAL_METER, VarEnum.VT_R8, true, string.Empty, string.Empty, string.Empty);
                        this.ProcessVariableCollection.Add(meterPv);
                        this.Permissives.InputUnitType = UNIT_TYPE.EXTERNAL_METER_INPUT_PERMISSIVE;
                        this.Permissives.OutputUnitType = UNIT_TYPE.EXTERNAL_METER_OUTPUT_PERMISSIVE;
                        break;
                    default:
				        this.Permissives.InputUnitType = UNIT_TYPE.MAX_UNIT;
				        this.Permissives.OutputUnitType = UNIT_TYPE.MAX_UNIT;
                        break;
                }
			}
		}


		public override string ID
		{
			get
			{
				switch (this.type)
				{
					case PRODUCT_MAP_TYPE.PRESET_RECIPE_MAP:
						return "Recipe " + this.PresetNumber + " - " + this.AssignedID;

					case PRODUCT_MAP_TYPE.PRESET_INJECTOR_MAP:
						return "Injector " + this.PresetNumber + " - " + this.AssignedID;

					case PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANK_MAP:
					case PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANKGROUP_MAP:
						return "Component " + this.PresetNumber + " - " + this.AssignedID;

					case PRODUCT_MAP_TYPE.PRODUCT_COMPANY_MAP:
					case PRODUCT_MAP_TYPE.BLEND_COMPONENT_MAP:
					case PRODUCT_MAP_TYPE.ADDITIVE_PROFILE_MAP:
					case PRODUCT_MAP_TYPE.PRODUCT_GROUP_MAP:
					case PRODUCT_MAP_TYPE.PRODUCT_COMPANY_GROUP_MAP:
					case PRODUCT_MAP_TYPE.TRANSACTION_ALIAS_EXCLUSION_MAP:
					case PRODUCT_MAP_TYPE.UNAVAILABLE_INVENTORY_COMPANY_MAP:	// added (IGO 02-Sep-2008)
					case PRODUCT_MAP_TYPE.SUPPLIER_PRODUCT_COMPANY_MAP:
						return this.AssignedToID + " - " + this.AssignedID;

					case PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_MAP:
                    case PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_TANKGROUP_MAP:
						return "External Component - " + this.AssignedID;

                    case PRODUCT_MAP_TYPE.PRESET_FLOW_CONTROLLED_ADDITIVE_MAP:
                        return "Flow Controlled Additive - " + this.AssignedID;
                    case PRODUCT_MAP_TYPE.OFFLOAD_EXTERNAL_METER_MAP:
                        return "Offload Product Meter - " + this.AssignedID;
                        //return "External Component - " + this.AssignedID;
                    case PRODUCT_MAP_TYPE.LEDGER_VIEW_MAP:
						return "Ledger View - " + this.AssignedID;

					default:
						return "Undefined Product Map";
				}
			}
			set
			{
			}
		}

		[EntityImportExport("SHIPTOPRODUCTID", 125, "ShipToProductID")]
		public string ShipToProductID { get { return this._ShipToProductID; } set { this.SetString("Customer Product ID", 30, value, ref this._ShipToProductID); } }

		[EntityImportExport("SHIPTOPRODUCTCODE", 125, "ShipToProductCode")]
		public string ShipToProductCode { get { return this._ShipToProductCode; } set { this.SetString("Customer Product Code", 15, value, ref this._ShipToProductCode); } }

		[EntityImportExport("SHIPTOLOADRACKDISPLAYTEXT", 165, "ShipToLoadRackDisplayText")]
		public string ShipToLoadRackDisplayText { get { return this._ShipToLoadRackDisplayText; } set { this.SetString("Customer Load Rack Display Text", 10, value, ref this._ShipToLoadRackDisplayText); } }



		public ProductMapClass()
		{
		    this._AdditiveRate = new SIDouble(EngineeringUnit.FmvUsGal, NumberFormatInfo.CurrentInfo, 0.0);
		    this._AdditiveCycleVolume = new SIDouble(EngineeringUnit.FmvCm3, NumberFormatInfo.CurrentInfo, 0.0);
		    this._UnavailableInventoryGross = new SIDouble(EngineeringUnit.FmvUsGal, NumberFormatInfo.CurrentInfo, 0.0);
		    this._UnavailableInventoryNet = new SIDouble(EngineeringUnit.FmvUsGal, NumberFormatInfo.CurrentInfo, 0.0);
		    this.Reset();
		}

		public ProductMapClass(SiteClass site)
		{
		    this._AdditiveRate = new SIDouble(site.AdditiveProfileRateUnits, site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.ADDITIVE_RATE_AMOUNT), 0.0);
		    this._AdditiveCycleVolume = new SIDouble(site.AdditiveProfileCycleAmountUnits, site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.ADDITIVE_CYCLE_AMOUNT), 0.0);
		    this._UnavailableInventoryGross = new SIDouble(site.VolumeUnits, site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME), 0.0);
		    this._UnavailableInventoryNet = new SIDouble(site.VolumeUnits, site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME), 0.0);
		    this.Reset();
		}

		//deep copy constructor
		public ProductMapClass(ProductMapClass map)
		{
			this.AssignedGuid = map.AssignedGuid;
			this.AssignedID = map.AssignedID;
			this.AssignedToGuid = map.AssignedToGuid;
			this.ProductMasterRecordGuid = map.ProductMasterRecordGuid;
			this.type = map.type;
			this.Sequence = map.Sequence;
			this.BlendPercentage = map.BlendPercentage;
			this.Ratio = map.Ratio;
			this.Tolerance = map.Tolerance;
			this.PresetNumber = map.PresetNumber;
			this.AdditiveProfileGuid = map.AdditiveProfileGuid;
			this.TankOrGroupGuid = map.TankOrGroupGuid;
			this.EnableRecipe = map.EnableRecipe;
			this.SpecialInstructions = map.SpecialInstructions;
			this.Permissives = map.Permissives;
			this.ProcessVariableCollection = new ProcessVariableCollectionClass();
			this.AssignedCode = map.AssignedCode;
			this.AssignedDescription = map.AssignedDescription;
			this.AssignedProductType = map.AssignedProductType;
			this.AssignedLoadRackDisplayText = map.AssignedLoadRackDisplayText;
			this.LockedOut = map.LockedOut;
			this.HazardousMaterial = map.HazardousMaterial;
			this.LoadByWeight = map.LoadByWeight;
			this.Meter = new MeterClass(map.Meter);
			this.MeterID = map.Meter.ID;
			this.AssignedToMeterGuid = map.AssignedToMeterGuid;
			this.PIDXProductCode = map.PIDXProductCode;
			this.ContaminationPromptLoadRackText = map.ContaminationPromptLoadRackText;
			this.AdditiveProfileID = map.AdditiveProfileID;
			this.TankOrGroupID = map.TankOrGroupID;
			this.AssignedToID = map.AssignedToID;
			this.AssignedToName = map.AssignedToName;
			this.AssignedToAddress = map.AssignedToAddress;
			this.AssignedToCity = map.AssignedToCity;
			this.AssignedToState = map.AssignedToState;
			this.MeterValue = map.MeterValue;	
			this._MeterID = map.Meter.ID;
			this._ShipToProductID = map._ShipToProductID;
			this._ShipToProductCode = map._ShipToProductCode;
			this._AdditiveRate = map._AdditiveRate;
			this._AdditiveCycleVolume = map._AdditiveCycleVolume;
			this._UnavailableInventoryGross = map._UnavailableInventoryGross;
			this._UnavailableInventoryNet = map._UnavailableInventoryNet;
			this.DesiredTreatRate = map.DesiredTreatRate;
			this._ShipToLoadRackDisplayText = map._ShipToLoadRackDisplayText;
			this.AdditiveCycleVolume = map.AdditiveCycleVolume;
			this.UnavailableInventoryGross = map.UnavailableInventoryGross;
			this.UnavailableInventoryNet = map.UnavailableInventoryNet;
			this.Note = map.Note;
			this.PIDXFamilyCode = map.PIDXFamilyCode;
			this.IsEthanol = map.IsEthanol;
			this._AdditiveRate = new SIDouble(EngineeringUnit.FmvUsGal, NumberFormatInfo.CurrentInfo, 0.0);
			this._AdditiveCycleVolume = new SIDouble(EngineeringUnit.FmvCm3, NumberFormatInfo.CurrentInfo, 0.0);
			this._UnavailableInventoryGross = new SIDouble(EngineeringUnit.FmvUsGal, NumberFormatInfo.CurrentInfo, 0.0);
			this._UnavailableInventoryNet = new SIDouble(EngineeringUnit.FmvUsGal, NumberFormatInfo.CurrentInfo, 0.0);
		}

      [XmlIgnore]
		public override ENTITY_TYPE EntityType
		{
			get
			{
				switch (this.Type)
				{
					case PRODUCT_MAP_TYPE.BLEND_COMPONENT_MAP:
						return ENTITY_TYPE.PRODUCT_MAP_BLEND_COMPONENT;

					case PRODUCT_MAP_TYPE.PRODUCT_GROUP_MAP:
						return ENTITY_TYPE.PRODUCT_MAP_PRODUCT_GROUP;

					case PRODUCT_MAP_TYPE.PRESET_RECIPE_MAP:
						return ENTITY_TYPE.PRODUCT_MAP_PRESET_RECIPE;

					case PRODUCT_MAP_TYPE.PRESET_INJECTOR_MAP:
						return ENTITY_TYPE.PRODUCT_MAP_PRESET_INJECTOR;

					case PRODUCT_MAP_TYPE.ADDITIVE_PROFILE_MAP:
						return ENTITY_TYPE.PRODUCT_MAP_ADDITIVE_PROFILE;

					case PRODUCT_MAP_TYPE.PRODUCT_COMPANY_MAP:
						return ENTITY_TYPE.PRODUCT_MAP_COMPANY;

					case PRODUCT_MAP_TYPE.SUPPLIER_PRODUCT_COMPANY_MAP:
						return ENTITY_TYPE.PRODUCT_MAP_COMPANY_SUPPLIER;

					// added (IGO 02-Sep-2008)
					case PRODUCT_MAP_TYPE.UNAVAILABLE_INVENTORY_COMPANY_MAP:
						return ENTITY_TYPE.PRODUCT_MAP_COMPANY_UNAVAILABLE_INVENTORY;

					case PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANK_MAP:
					case PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANKGROUP_MAP:
						return ENTITY_TYPE.PRODUCT_MAP_PRESET_COMPONENT;

					case PRODUCT_MAP_TYPE.PRODUCT_COMPANY_GROUP_MAP:
						return ENTITY_TYPE.PRODUCT_MAP_COMPANY_GROUP;

					case PRODUCT_MAP_TYPE.TRANSACTION_ALIAS_EXCLUSION_MAP:  //CSI 4693, add TypeID
						return ENTITY_TYPE.PRODUCT_MAP_TRANSACTION_ALIAS_EXCLUSION;

                    case PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_TANKGROUP_MAP:
                    case PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_MAP:
						return ENTITY_TYPE.PRODUCT_MAP_PRESET_EXTERNAL_COMPONENT;

                    case PRODUCT_MAP_TYPE.PRESET_FLOW_CONTROLLED_ADDITIVE_MAP:
                        return ENTITY_TYPE.PRODUCT_MAP_PRESET_FLOW_CONTROLLED_ADDITIVE;

                    case PRODUCT_MAP_TYPE.OFFLOAD_EXTERNAL_METER_MAP:
                        return ENTITY_TYPE.PRODUCT_MAP_OFFLOAD_EXTERNAL_METER;

                    default:
						return ENTITY_TYPE.PRODUCT_MAP_UNDEFINED;
				}
			}
		}

		[XmlIgnore]
		public override ENTITY_TYPE ParentEntityType
		{
			get
			{
				switch (this.Type)
				{
					case PRODUCT_MAP_TYPE.BLEND_COMPONENT_MAP:
						return ENTITY_TYPE.PRODUCT;

					case PRODUCT_MAP_TYPE.PRODUCT_GROUP_MAP:
						return ENTITY_TYPE.PRODUCT_GROUP;

					case PRODUCT_MAP_TYPE.PRESET_RECIPE_MAP:
                    case PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANK_MAP:
                    case PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANKGROUP_MAP:
                    case PRODUCT_MAP_TYPE.PRESET_INJECTOR_MAP:
                    case PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_MAP:
                    case PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_TANKGROUP_MAP:
                    case PRODUCT_MAP_TYPE.PRESET_FLOW_CONTROLLED_ADDITIVE_MAP:
                    case PRODUCT_MAP_TYPE.OFFLOAD_EXTERNAL_METER_MAP:
						return ENTITY_TYPE.STATION;

					case PRODUCT_MAP_TYPE.ADDITIVE_PROFILE_MAP:
						return ENTITY_TYPE.ADDITIVE_PROFILE;

					case PRODUCT_MAP_TYPE.PRODUCT_COMPANY_MAP:
					case PRODUCT_MAP_TYPE.SUPPLIER_PRODUCT_COMPANY_MAP:
						return ENTITY_TYPE.PRODUCT;

					// added (IGO 02-Sep-2008)
					case PRODUCT_MAP_TYPE.UNAVAILABLE_INVENTORY_COMPANY_MAP:
						return ENTITY_TYPE.PRODUCT;

					case PRODUCT_MAP_TYPE.PRODUCT_COMPANY_GROUP_MAP:
						return ENTITY_TYPE.PRODUCT_GROUP;

					case PRODUCT_MAP_TYPE.TRANSACTION_ALIAS_EXCLUSION_MAP:  //CSI 4693, add TypeID
						return ENTITY_TYPE.TRANSACTION;

					default:
						return ENTITY_TYPE.NONE;
				}
			}
		}

		public string AssignedToolTip
		{
			get
			{
				string toolTip = "";
				if (this.AssignedCode != "")
					toolTip = this.AssignedCode;
				if (this.AssignedDescription != "")
					toolTip += ", " + this.AssignedDescription;
				return toolTip;
			}
		}

		public string AssignedToToolTip
		{
			get
			{
				string toolTip;
				if (this.AssignedToName != "")
					toolTip = this.AssignedToName;
				else
					toolTip = this.AssignedToID;
				if (this.AssignedToAddress != "")
					toolTip += ", " + this.AssignedToAddress;
				if (this.AssignedToCity != "")
					toolTip += ", " + this.AssignedToCity;
				if (this.AssignedToState != "")
					toolTip += ", " + this.AssignedToState;
				return toolTip;
			}
		}

		public override void Reset()
		{
			base.Reset();
			this.AssignedToGuid = Guid.Empty;
			this.AssignedGuid = Guid.Empty;
			this.Type = PRODUCT_MAP_TYPE.UNDEFINED_MAP;
			this.Sequence = 0;
			this.BlendPercentage = 0.0;
			this._AdditiveRate.SIValue = 0.0;
			this.Ratio = 0.0;
			this._AdditiveCycleVolume.SIValue = 0.0;
			this.DesiredTreatRate = 0.0;
			this.Tolerance = 0.0;
			this.PresetNumber = 0;
			this.AdditiveProfileGuid = Guid.Empty;
			this.TankOrGroupGuid = Guid.Empty;
			this.ShipToProductID = "";
			this.ShipToProductCode = "";
			this.ShipToLoadRackDisplayText = "";
			this.SpecialInstructions = string.Empty;
			this.Permissives = new PermissivesClass();
			this.ProcessVariableCollection = new ProcessVariableCollectionClass();
			this.AssignedID = "";
			this.AssignedCode = "";
			this.AssignedDescription = "";
			this.AssignedProductType = ProductType.MaxProduct;
			this.AssignedLoadRackDisplayText = "";
			this.LockedOut = false;
			this.HazardousMaterial = false;
			this.LoadByWeight = false;
			this.PIDXProductCode = "";
			this.PIDXFamilyCode = string.Empty;
			this.IsEthanol = false;
			this.ContaminationPromptLoadRackText = "";
			this.AssignedToID = "";
			this.TankOrGroupID = "";
			this.MeterValue = 0.0;
			this.AdditiveProfileID = "";
			this.AssignedToName = "";
			this.AssignedToAddress = "";
			this.AssignedToCity = "";
			this.AssignedToState = "";
			this._UnavailableInventoryGross.SIValue = 0.0;		// added (IGO 02-Sep-2008)
			this._UnavailableInventoryNet.SIValue = 0.0;		// added (IGO 02-Sep-2008)
			this.AssignedToMeterGuid = Guid.Empty;
			this.Meter = new MeterClass();
			this.EnableRecipe = true;
		}
 
        public override void Load(object o)
		{
			PRODUCT_MAP_TYPE mapType = this.Type;

		    this.Reset();

			if (o is DataSet || o is DataRow)
			{
				DataRow row;
			    var set = o as DataSet;
			    if (set != null)
				{
					if (set.Tables.Count == 0)
					{
						throw new Exception("ProductMapClass.cs Load() Set.Tables[0]");
					}
					DataTable table = set.Tables[0];
					if (table.Rows.Count == 0)
						return;
					row = table.Rows[0];

					if (table.Columns.Contains("TankID")) this.TankOrGroupID = DataObject.getValue<string>(row["TankID"], "");
					else if (table.Columns.Contains("TankGroupID")) this.TankOrGroupID = DataObject.getValue<string>(row["TankGroupID"], "");

					if (table.Columns.Contains("AssignedToID")) this.AssignedToID = DataObject.getValue<string>(row["AssignedToID"], "");
					if (table.Columns.Contains("AssignedToName")) this.AssignedToName = DataObject.getValue<string>(row["AssignedToName"], "");
					if (table.Columns.Contains("AssignedToAddress")) this.AssignedToAddress = DataObject.getValue<string>(row["AssignedToAddress"], "");
					if (table.Columns.Contains("AssignedToCity")) this.AssignedToCity = DataObject.getValue<string>(row["AssignedToCity"], "");
					if (table.Columns.Contains("AssignedToState")) this.AssignedToState = DataObject.getValue<string>(row["AssignedToState"], "");
                    if (table.Columns.Contains("ProductMasterRecordGuid")) this.ProductMasterRecordGuid = DataObject.getValue<Guid>(row["ProductMasterRecordGuid"], Guid.Empty);

					this.AssignedID = DataObject.getValue<string>(row["AssignedID"], "");
					this.AssignedCode = DataObject.getValue<string>(row["AssignedCode"], "");
					this.AssignedDescription = DataObject.getValue<string>(row["AssignedDescription"], "");
					this.AssignedProductType = DataObject.getValue<ProductType>(row["AssignedProductType"], ProductType.MaxProduct);
					this.AssignedLoadRackDisplayText = DataObject.getValue<string>(row["AssignedLoadRackDisplayText"], "");
					this.LockedOut = DataObject.getValue<bool>(row["LockedOut"], false);
					this.HazardousMaterial = DataObject.getValue<bool>(row["HazardousMaterial"], false);
					this.LoadByWeight = DataObject.getValue<bool>(row["LoadByWeight"], false);
					this.PIDXProductCode = DataObject.getValue<string>(row["PIDXCode"], "");
					this.PIDXFamilyCode = DataObject.getValue<string>(row["PIDXFamilyCode"], "");
					this.IsEthanol = DataObject.getValue<bool>(row["IsEthanol"], false);
					this.ContaminationPromptLoadRackText = DataObject.getValue<string>(row["ContaminationPromptLoadRackText"], "");
					this.AdditiveProfileID = DataObject.getValue<string>(row["AdditiveProfileID"], "");
					if (table.Columns.Contains("EnableRecipe")) this.EnableRecipe = DataObject.getValue<bool>(row["EnableRecipe"], true);
				}
				else
				{
					row = (DataRow)o;
				}
			    this.Type = mapType;
			    this._IdentityGuid = DataObject.getValue<Guid>(row[GetIdentityColumnName(this.Type)], Guid.Empty);
			    this.AssignedToGuid = DataObject.getValue<Guid>(row[GetAssignedToColumnName(this.Type)], Guid.Empty);
			    this.AssignedGuid = DataObject.getValue<Guid>(row["ProductGuid"], Guid.Empty);
			    this.Sequence = DataObject.getValue<int>(row["Sequence"], 0);
			    this.BlendPercentage = DataObject.getValue<double>(row["BlendPercentage"], 0.0);
			    this._AdditiveRate.SIValue = DataObject.getValue<double>(row["AdditiveRate"], 0.0);
			    this.Ratio = DataObject.getValue<double>(row["Ratio"], 0.0);
			    this._AdditiveCycleVolume.SIValue = DataObject.getValue<double>(row["AdditiveCycleVolume"], 0.0);
                if (mapType == PRODUCT_MAP_TYPE.ADDITIVE_PROFILE_MAP)
                    this.DesiredTreatRate = DataObject.getValue<double>(row["DesiredTreatRate"], 0.0);
                this.Tolerance = DataObject.getValue<double>(row["Tolerance"], 0.0);
			    this.PresetNumber = DataObject.getValue<int>(row["PresetNumber"], 0);
			    this.AdditiveProfileGuid = DataObject.getValue<Guid>(row["AdditiveProfileGuid"], Guid.Empty);
			    this.TankOrGroupGuid = DataObject.getValue<Guid>(row[GetTankOrTankGroupColumnName(this.Type)], Guid.Empty);
			    this._MeterID = DataObject.getValue<string>(row["MeterID"], "");
			    this._ShipToProductID = DataObject.getValue<string>(row["ShipToProductID"], "");
			    this._ShipToProductCode = DataObject.getValue<string>(row["ShipToProductCode"], "");
			    this._ShipToLoadRackDisplayText = DataObject.getValue<string>(row["ShipToLoadRackDisplayText"], "");
                if (ContainsSpecialInstructions(mapType))
				{
				    this.SpecialInstructions = DataObject.getValue<string>(row["SpecialInstructionNote"], string.Empty);
				}
			    this._UnavailableInventoryGross.SIValue = DataObject.getValue<double>(row["UnavailableInventoryGross"], 0.0);
			    this._UnavailableInventoryNet.SIValue = DataObject.getValue<double>(row["UnavailableInventoryNet"], 0.0);
                if (mapType ==  PRODUCT_MAP_TYPE.PRESET_RECIPE_MAP) this.EnableRecipe = DataObject.getValue<bool>(row["EnableRecipe"], true);
			    this._CreatedDate = DataObject.getValue<DateTimeOffset>(row["CreatedDate"], DateTimeOffset.Now);
			    this._CreatedBy = DataObject.getValue<string>(row["CreatedBy"], ADMIN);
			    this._UpdatedDate = DataObject.getValue<DateTimeOffset>(row["UpdatedDate"], this._CreatedDate);
			    this._UpdatedBy = DataObject.getValue<string>(row["UpdatedBy"], ADMIN);

				if (ContainsMeter(this.Type))
				{
				    this.AssignedToMeterGuid = DataObject.getValue<Guid>(row["AssignedToMeterGuid"], Guid.Empty);
				}
			}
			else
			{
				var map = o as ProductMapClass;
				if (map != null)
				{
					ProductMapClass productMap = map;
					this._IdentityGuid = productMap.IdentityGuid;
					this.AssignedToGuid = productMap.AssignedToGuid;
					this.AssignedGuid = productMap.AssignedGuid;
					this.Type = productMap.Type;
					this.Sequence = productMap.Sequence;
					this.BlendPercentage = productMap.BlendPercentage;
					this._AdditiveRate.SIValue = productMap._AdditiveRate.SIValue;
					this.Ratio = productMap.Ratio;
					this._AdditiveCycleVolume.SIValue = productMap._AdditiveCycleVolume.SIValue;
					if (mapType == PRODUCT_MAP_TYPE.ADDITIVE_PROFILE_MAP)
						this.DesiredTreatRate = productMap.DesiredTreatRate;
					this.Tolerance = productMap.Tolerance;
					this.PresetNumber = productMap.PresetNumber;
					this.AdditiveProfileGuid = productMap.AdditiveProfileGuid;
					this.TankOrGroupGuid = productMap.TankOrGroupGuid;
					this._ShipToProductID = productMap.ShipToProductID;
					this._ShipToProductCode = productMap.ShipToProductCode;
					this._ShipToLoadRackDisplayText = productMap.ShipToLoadRackDisplayText;
					this.SpecialInstructions = productMap.SpecialInstructions;
					this._UnavailableInventoryGross.SIValue = productMap._UnavailableInventoryGross.SIValue;	// added (IGO 02-Sep-2008)
					this._UnavailableInventoryNet.SIValue = productMap._UnavailableInventoryNet.SIValue;		// added (IGO 02-Sep-2008)
					this._CreatedDate = productMap.CreatedDate;
					this._CreatedBy = productMap.CreatedBy;
					this._UpdatedDate = productMap.UpdatedDate;
					this._UpdatedBy = productMap.UpdatedBy;
					this.AssignedID = productMap.AssignedID;
					this.AssignedCode = productMap.AssignedCode;
					this.AssignedProductType = productMap.AssignedProductType;
					this.AssignedLoadRackDisplayText = productMap.AssignedLoadRackDisplayText;
					this.LockedOut = productMap.LockedOut;
					this.HazardousMaterial = productMap.HazardousMaterial;
					this.LoadByWeight = productMap.LoadByWeight;
					this.PIDXProductCode = productMap.PIDXProductCode;
					this.PIDXFamilyCode = productMap.PIDXFamilyCode;
					this.IsEthanol = productMap.IsEthanol;
					this.AdditiveProfileID = productMap.AdditiveProfileID;
					this.TankOrGroupID = productMap.TankOrGroupID;
					this.AssignedToID = productMap.AssignedToID;
					this.AssignedToName = productMap.AssignedToName;
					this.AssignedToAddress = productMap.AssignedToAddress;
					this.AssignedToCity = productMap.AssignedToCity;
					this.AssignedToState = productMap.AssignedToState;
					this.AssignedToMeterGuid = productMap.AssignedToMeterGuid;
					this.EnableRecipe = productMap.EnableRecipe;
					this.Permissives.Load(productMap.Permissives);
					this.Meter = new MeterClass(map.Meter);
					this.ProcessVariableCollection.Clear();
					foreach (ProcessVariableClass existingProcessVariable in productMap.ProcessVariableCollection)
					{
					ProcessVariableClass newProcessVariable = new ProcessVariableClass();
					newProcessVariable.Load(existingProcessVariable);
					this.ProcessVariableCollection.Add(newProcessVariable);
					}
			    }
			    else
			    {
			        var xmlNode = o as XmlNode;
			        if (xmlNode != null)
			        {
			            XmlNode node = xmlNode;

			            if (node.Name == "AuthorizedProduct"
			                || node.Name == "AuthorizedCustomer"
			                || node.Name == "AuthorizedCustomerGroup")
			            {
			                // Use the AssignedID if it is an authorized product.
			                if (node.Name == "AuthorizedProduct")
			                {
			                    if (node.Attributes?["ID"] != null) this.AssignedID = node.Attributes["ID"].Value;

			                    this.Type = PRODUCT_MAP_TYPE.PRODUCT_COMPANY_MAP;

			                }

			                // Use the AssignedToID if it is an authorized customer.
			                if (node.Name == "AuthorizedCustomer")
			                {
			                    this.Type = PRODUCT_MAP_TYPE.PRODUCT_COMPANY_MAP;
			                    if (node.Attributes?["ID"] != null) this.AssignedToID = node.Attributes["ID"].Value;
			                }

			                if (node.Name == "AuthorizedCustomerGroup")
			                {
			                    this.Type = PRODUCT_MAP_TYPE.PRODUCT_COMPANY_GROUP_MAP;
			                    if (node.Attributes?["ID"] != null) this.AssignedToID = node.Attributes["ID"].Value;
			                }

			                if (node.Attributes?["AdditiveProfile"] != null) this.AdditiveProfileID = node.Attributes["AdditiveProfile"].Value;

			                if (node.Attributes?["ShipToProductID"] != null) this.ShipToProductID = node.Attributes["ShipToProductID"].Value;

			                if (node.Attributes?["ShipToProductCode"] != null) this.ShipToProductCode = node.Attributes["ShipToProductCode"].Value;

			                if (node.Attributes?["ShipToLoadRackDisplayText"] != null) this.ShipToLoadRackDisplayText = node.Attributes["ShipToLoadRackDisplayText"].Value;
			                if (ContainsSpecialInstructions(mapType))
			                {
			                    if (node.Attributes?["SpecialInstructions"] != null)
			                    {
			                        this.SpecialInstructions = node.Attributes["SpecialInstructions"].Value;
			                    }
			                }

			            }
			            else if (node.Name == "SupplierAuthorizedProduct")
			            {
			                // Use the AssignedID if it is an authorized product.
			                if (node.Attributes?["ID"] != null) this.AssignedID = node.Attributes["ID"].Value;

			                this.Type = PRODUCT_MAP_TYPE.SUPPLIER_PRODUCT_COMPANY_MAP;

			                if (node.Attributes?["ShipToProductID"] != null) this.ShipToProductID = node.Attributes["ShipToProductID"].Value;

			                if (node.Attributes?["ShipToProductCode"] != null) this.ShipToProductCode = node.Attributes["ShipToProductCode"].Value;

			                if (node.Attributes?["ShipToLoadRackDisplayText"] != null) this.ShipToLoadRackDisplayText = node.Attributes["ShipToLoadRackDisplayText"].Value;

			            }
			            else if (node.Name == "UnavailableInventory")
			            {
                            // added (IGO 02-Sep-2008)
                            if (node.Attributes?["ID"] != null) this.AssignedToID = node.Attributes["ID"].Value;

			                this.Type = PRODUCT_MAP_TYPE.UNAVAILABLE_INVENTORY_COMPANY_MAP;

			                if (node.Attributes?["Gross"] != null) this._UnavailableInventoryGross.Value = Convert.ToDouble(node.Attributes["Gross"].Value);

			                if (node.Attributes?["Net"] != null) this._UnavailableInventoryNet.Value = Convert.ToDouble(node.Attributes["Net"].Value);

			            }

			            else if (node.Name == "BlendComponent")
			            {
			                if (node.Attributes?["ID"] != null) this.AssignedID = node.Attributes["ID"].Value;

			                this.Type = PRODUCT_MAP_TYPE.BLEND_COMPONENT_MAP;

			                if (node.Attributes?["BlendPercentage"] != null) this.BlendPercentage = Convert.ToDouble(node.Attributes["BlendPercentage"].Value);
			            }

			            else
			                throw new Exception("Invalid ProductMap Type");
			        }
			    }
			}
		}

		public override void Store(object o)
		{
			if (o == null)
				throw new ArgumentNullException(nameof(o));

		    var node = o as XmlNode;
		    if (node != null)
			{
				XmlAttribute attribute;

				if (this.Type == PRODUCT_MAP_TYPE.PRODUCT_COMPANY_MAP
				|| this.Type == PRODUCT_MAP_TYPE.PRODUCT_COMPANY_GROUP_MAP)
				{
					if (node.Name == "AuthorizedCustomer"
					|| node.Name == "AuthorizedCustomerGroup")
					{
						attribute = node.OwnerDocument?.CreateAttribute("ID");
					    if (attribute != null)
					    {
					        attribute.Value = this.AssignedToID;
					        node.Attributes?.Append(attribute);
					    }
					}
					else
					{
						attribute = node.OwnerDocument?.CreateAttribute("ID");
					    if (attribute != null)
					    {
					        attribute.Value = this.AssignedID;
					        node.Attributes?.Append(attribute);
					    }
					}

					attribute = node.OwnerDocument?.CreateAttribute("AdditiveProfile");
				    if (attribute != null)
				    {
				        attribute.Value = this.AdditiveProfileID;
				        node.Attributes?.Append(attribute);
				    }

				    attribute = node.OwnerDocument?.CreateAttribute("ShipToProductID");
				    if (attribute != null)
				    {
				        attribute.Value = this.ShipToProductID;
				        node.Attributes?.Append(attribute);
				    }

				    attribute = node.OwnerDocument?.CreateAttribute("ShipToProductCode");
				    if (attribute != null)
				    {
				        attribute.Value = this.ShipToProductCode;
				        node.Attributes?.Append(attribute);
				    }

				    attribute = node.OwnerDocument?.CreateAttribute("ShipToLoadRackDisplayText");
				    if (attribute != null)
				    {
				        attribute.Value = this.ShipToLoadRackDisplayText;
				        node.Attributes?.Append(attribute);
				    }

				    attribute = node.OwnerDocument?.CreateAttribute("SpecialInstructions");
				    if (attribute != null)
				    {
				        attribute.Value = this.SpecialInstructions;
				        node.Attributes?.Append(attribute);
				    }
				}
				else if (this.Type == PRODUCT_MAP_TYPE.SUPPLIER_PRODUCT_COMPANY_MAP)
				{
					attribute = node.OwnerDocument?.CreateAttribute("ID");
				    if (attribute != null)
				    {
				        attribute.Value = this.AssignedID;
				        node.Attributes?.Append(attribute);
				    }

				    if (node.OwnerDocument != null)
				    {
				        attribute = node.OwnerDocument.CreateAttribute("ShipToProductID");
				        attribute.Value = this.ShipToProductID;
				        node.Attributes?.Append(attribute);

				        attribute = node.OwnerDocument.CreateAttribute("ShipToProductCode");
				        attribute.Value = this.ShipToProductCode;
				        node.Attributes?.Append(attribute);

				        attribute = node.OwnerDocument.CreateAttribute("ShipToLoadRackDisplayText");
				    }
				    if (attribute != null)
				    {
				        attribute.Value = this.ShipToLoadRackDisplayText;
				        node.Attributes?.Append(attribute);
				    }
				}

				// added (IGO 02-Sep-2008)
				else if (this.Type == PRODUCT_MAP_TYPE.UNAVAILABLE_INVENTORY_COMPANY_MAP)
				{
					attribute = node.OwnerDocument?.CreateAttribute("ID");
				    if (attribute != null)
				    {
				        attribute.Value = this.AssignedToID;
				        node.Attributes?.Append(attribute);
				    }

				    attribute = node.OwnerDocument?.CreateAttribute("Gross");
				    if (attribute != null)
				    {
				        attribute.Value = this.UnavailableInventoryGross;
				        node.Attributes?.Append(attribute);
				    }

				    attribute = node.OwnerDocument?.CreateAttribute("Net");
				    if (attribute != null)
				    {
				        attribute.Value = this.UnavailableInventoryNet;
				        node.Attributes?.Append(attribute);
				    }
				}

				else if (this.Type == PRODUCT_MAP_TYPE.BLEND_COMPONENT_MAP)
				{
					attribute = node.OwnerDocument?.CreateAttribute("ID");
				    if (attribute != null)
				    {
				        attribute.Value = this.AssignedID;
				        node.Attributes?.Append(attribute);
				    }

				    attribute = node.OwnerDocument?.CreateAttribute("BlendPercentage");
				    if (attribute != null)
				    {
				        attribute.Value = this.BlendPercentage.ToString(CultureInfo.InvariantCulture);
				        node.Attributes?.Append(attribute);
				    }
				}
			}
			else
				throw new Exception("Store Error - Invalid Object Type : " + o.GetType());
		}

		public string LocationIDQuery => ((this.Type != PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANKGROUP_MAP 
                                            && this.Type != PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_TANKGROUP_MAP) ?
		                                      " tblTanks.TankID AS TankID " :
		                                      " tblTankGroups.ID AS TankGroupID ");


	    public string GuidIDQuery
		{
			get
			{
				string guidIDQuery;

				if (this.Type == PRODUCT_MAP_TYPE.PRODUCT_COMPANY_MAP
					|| this.Type == PRODUCT_MAP_TYPE.SUPPLIER_PRODUCT_COMPANY_MAP)
				{
					guidIDQuery = ", AssignedToCompanies.ID AS AssignedToID, " +
										"AssignedToCompanies.Name AS AssignedToName, " +
										"AssignedToCompanies.Address1 AS AssignedToAddress, " +
										"AssignedToCompanies.City AS AssignedToCity, " +
										"AssignedToCompanies.State AS AssignedToState ";
				}
				// added (IGO 02-Sep-2008)
				else if (this.Type == PRODUCT_MAP_TYPE.UNAVAILABLE_INVENTORY_COMPANY_MAP)
				{
					guidIDQuery = ", AssignedToCompanies.ID AS AssignedToID ";
				}
				else if (this.Type == PRODUCT_MAP_TYPE.BLEND_COMPONENT_MAP)
				{
					guidIDQuery = ", AssignedToProducts.ProductID AS AssignedToID ";
				}
				else if (this.Type == PRODUCT_MAP_TYPE.ADDITIVE_PROFILE_MAP)
				{
					guidIDQuery = ", AssignedToAdditiveProfiles.ID AS AssignedToID ";
				}
				else if (this.Type == PRODUCT_MAP_TYPE.PRODUCT_GROUP_MAP
					|| this.Type == PRODUCT_MAP_TYPE.PRODUCT_COMPANY_GROUP_MAP)
				{
					guidIDQuery = ", AssignedToApplicationString.ID AS AssignedToID ";
				}
				else if (this.Type == PRODUCT_MAP_TYPE.TRANSACTION_ALIAS_EXCLUSION_MAP)
				{
					guidIDQuery = ", AssignedToTransactionAliases.AliasName AS AssignedToID ";
				}
				else
				{
					guidIDQuery = string.Empty;
				}

				return guidIDQuery;
			}
		}

		public string OrderBy
		{
			get
			{
				string orderBy;

                switch (this.type)
                {
                    case PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANK_MAP:
                    case PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANKGROUP_MAP:
                    case PRODUCT_MAP_TYPE.PRESET_INJECTOR_MAP:
                    case PRODUCT_MAP_TYPE.PRESET_RECIPE_MAP:
                    case PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_MAP :
                    case PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_TANKGROUP_MAP:
                    case PRODUCT_MAP_TYPE.PRESET_FLOW_CONTROLLED_ADDITIVE_MAP:
                    case PRODUCT_MAP_TYPE.OFFLOAD_EXTERNAL_METER_MAP:
                        orderBy = " ORDER BY PresetNumber";
                        break;
                    case PRODUCT_MAP_TYPE.PRODUCT_COMPANY_MAP:
                    case PRODUCT_MAP_TYPE.UNAVAILABLE_INVENTORY_COMPANY_MAP:
                    case PRODUCT_MAP_TYPE.PRODUCT_COMPANY_GROUP_MAP:
                    case PRODUCT_MAP_TYPE.SUPPLIER_PRODUCT_COMPANY_MAP:
                        orderBy = " ORDER BY AssignedID";
                        break;
                    default:
                        orderBy = " ORDER BY Sequence";
                        break;
                }
				return orderBy;
			}
		}

		#region Parameterized SQL Commands

		public void UpdateSQL(SqlCommand cmd)
		{
			string specialInstructionsUpdateString = "";
			if (ContainsSpecialInstructions(this.Type))
			{
				specialInstructionsUpdateString = "SpecialInstructionNote = @SpecialInstructionNote, ";
			}
			cmd.CommandText = "UPDATE " + GetMappingTableName(this.Type) + " SET " +
							GetAssignedToColumnName(this.Type) + " = @AssignedColumn, " +
							"ProductGuid = @ProductGuid, " +
							"Sequence = @Sequence, " +
							"BlendPercentage = @BlendPercentage, " +
							"AdditiveRate = @AdditiveRate, " +
							"Ratio = @Ratio, " +
							"AdditiveCycleVolume = @AdditiveCycleVolume, " +
							"Tolerance = @Tolerance, " +
							"PresetNumber = @PresetNumber, " +
							"AdditiveProfileGuid = @AdditiveProfileGuid, " +
                            "TankGuid = @TankGuid, " +
                            ((this.Type == PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANK_MAP 
                                || this.Type == PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANKGROUP_MAP 
                                || this.Type == PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_MAP 
                                || this.Type == PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_TANKGROUP_MAP) ? "TankGroupApplicationStringGuid = @TankGroupGuid," : "") +
                            "ShipToProductID = @ShipToProductID, " +
							"ShipToProductCode = @ShipToProductCode, " +
							"ShipToLoadRackDisplayText = @ShipToLoadRackDisplayText, " +
							specialInstructionsUpdateString +
							"UnavailableInventoryGross = @UnavailableInventoryGross, " +
							"UnavailableInventoryNet = @UnavailableInventoryNet, " +
                            ((this.Type == PRODUCT_MAP_TYPE.ADDITIVE_PROFILE_MAP)? "DesiredTreatRate = @DesiredTreatRate," : "") +
                            ((this.Type == PRODUCT_MAP_TYPE.PRESET_RECIPE_MAP) ? "EnableRecipe = @EnableRecipe," : "") +
							"UpdatedDate = @UpdatedDate, " +
							"UpdatedBy = @UpdatedBy " +
							(ContainsMeter(this.Type) ? ",AssignedToMeterGuid = @AssignedToMeterGuid" : string.Empty) +
							" WHERE " + GetIdentityColumnName(this.Type) + "= @ColumnNameIdentity";

			cmd.Parameters.Add("@AssignedColumn", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@ProductGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@Sequence", SqlDbType.Int);
			cmd.Parameters.Add("@BlendPercentage", SqlDbType.Float);
			cmd.Parameters.Add("@AdditiveRate", SqlDbType.Float);
			cmd.Parameters.Add("@Ratio", SqlDbType.Float);
			cmd.Parameters.Add("@AdditiveCycleVolume", SqlDbType.Float);
            if(this.Type == PRODUCT_MAP_TYPE.ADDITIVE_PROFILE_MAP)
                cmd.Parameters.Add("@DesiredTreatRate", SqlDbType.Float);
			cmd.Parameters.Add("@Tolerance", SqlDbType.Float);
			cmd.Parameters.Add("@PresetNumber", SqlDbType.Int);
			cmd.Parameters.Add("@AdditiveProfileGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@TankGuid", SqlDbType.UniqueIdentifier);

            if (this.Type == PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANK_MAP 
                || this.Type == PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANKGROUP_MAP 
                || this.Type == PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_TANKGROUP_MAP 
                || this.Type == PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_MAP)
            {
                cmd.Parameters.Add("@TankGroupGuid", SqlDbType.UniqueIdentifier);
            }

            cmd.Parameters.Add("@ShipToProductID", SqlDbType.NVarChar, 30);
			cmd.Parameters.Add("@ShipToProductCode", SqlDbType.NVarChar, 15);
			cmd.Parameters.Add("@ShipToLoadRackDisplayText", SqlDbType.NVarChar, 10);
			if (ContainsSpecialInstructions(this.Type))
			{
				cmd.Parameters.Add("@SpecialInstructionNote", SqlDbType.NVarChar,2000);
			}
			cmd.Parameters.Add("@UnavailableInventoryGross", SqlDbType.Float);
			cmd.Parameters.Add("@UnavailableInventoryNet", SqlDbType.Float);
            if (this.Type == PRODUCT_MAP_TYPE.PRESET_RECIPE_MAP)
                cmd.Parameters.Add("@EnableRecipe", SqlDbType.Bit);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@ColumnNameIdentity", SqlDbType.UniqueIdentifier);

			if (ContainsMeter(this.Type))
			{
				cmd.Parameters.Add("@AssignedToMeterGuid", SqlDbType.UniqueIdentifier);
			}

			cmd.Parameters["@AssignedColumn"].Value = this.AssignedToGuid;
			cmd.Parameters["@ProductGuid"].Value = this.AssignedGuid;
			cmd.Parameters["@Sequence"].Value = this.Sequence;
			cmd.Parameters["@BlendPercentage"].Value = this.BlendPercentage;
			cmd.Parameters["@AdditiveRate"].Value = this._AdditiveRate.SIValue;
			cmd.Parameters["@Ratio"].Value = this.Ratio;
			cmd.Parameters["@AdditiveCycleVolume"].Value = this._AdditiveCycleVolume.SIValue;
            if (this.Type == PRODUCT_MAP_TYPE.ADDITIVE_PROFILE_MAP)
                cmd.Parameters["@DesiredTreatRate"].Value = this.DesiredTreatRate;
            cmd.Parameters["@Tolerance"].Value = this.Tolerance;
			cmd.Parameters["@PresetNumber"].Value = this.PresetNumber;            

            if (this.AdditiveProfileGuid != Guid.Empty)
			{
                cmd.Parameters["@AdditiveProfileGuid"].Value = this.AdditiveProfileGuid;
            }
			else
			{
                cmd.Parameters["@AdditiveProfileGuid"].Value = DBNull.Value;
            }


            if (this.Type == PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANK_MAP 
                || this.Type == PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANKGROUP_MAP
                || this.Type == PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_MAP
                || this.Type == PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_TANKGROUP_MAP)
            {
                if (this.Type == PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANK_MAP
                    || this.Type == PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_MAP)
                {
                    if (this.TankOrGroupGuid != Guid.Empty)
                    {
                        cmd.Parameters["@TankGuid"].Value = this.TankOrGroupGuid;
                    }
                    else
                    {
                        cmd.Parameters["@TankGuid"].Value = DBNull.Value;
                    }
                    cmd.Parameters["@TankGroupGuid"].Value = DBNull.Value;
                }
                else
                {
                    if (this.TankOrGroupGuid != Guid.Empty)
                    {
                        cmd.Parameters["@TankGroupGuid"].Value = this.TankOrGroupGuid;
                    }
                    else
                    {
                        cmd.Parameters["@TankGroupGuid"].Value = DBNull.Value;
                    }
                    cmd.Parameters["@TankGuid"].Value = DBNull.Value;
                }
            }

            else
            {
                if (this.TankOrGroupGuid != Guid.Empty)
                {
                    cmd.Parameters["@TankGuid"].Value = this.TankOrGroupGuid;
                }
                else
                {
                    cmd.Parameters["@TankGuid"].Value = DBNull.Value;
                }
            }

            cmd.Parameters["@ShipToProductID"].Value = (String.IsNullOrEmpty(this._ShipToProductID)) ? "" : this._ShipToProductID;
			cmd.Parameters["@ShipToProductCode"].Value = (String.IsNullOrEmpty(this._ShipToProductCode)) ? "" : this._ShipToProductCode;
			cmd.Parameters["@ShipToLoadRackDisplayText"].Value = (String.IsNullOrEmpty(this._ShipToLoadRackDisplayText)) ? "" : this._ShipToLoadRackDisplayText;
			if (ContainsSpecialInstructions(this.Type))
			{
				if (this.SpecialInstructions != string.Empty)
				{ 
					cmd.Parameters["@SpecialInstructionNote"].Value = this.SpecialInstructions; 
				}
				else
				{ 
					cmd.Parameters["@SpecialInstructionNote"].Value = DBNull.Value; 
				}
			}

			cmd.Parameters["@UnavailableInventoryGross"].Value = this._UnavailableInventoryGross.SIValue;
			cmd.Parameters["@UnavailableInventoryNet"].Value = this._UnavailableInventoryNet.SIValue;
           if(this.Type == PRODUCT_MAP_TYPE.PRESET_RECIPE_MAP) 
               cmd.Parameters["@EnableRecipe"].Value =  this.EnableRecipe ? 1 : 0;
			cmd.Parameters["@UpdatedDate"].Value = this.UpdatedDate;
			cmd.Parameters["@UpdatedBy"].Value = this.UpdatedBy;

			if (ContainsMeter(this.Type))
			{
				if (this.AssignedToMeterGuid == Guid.Empty)
				{
					cmd.Parameters["@AssignedToMeterGuid"].Value = DBNull.Value;
				}
                else
				{
					cmd.Parameters["@AssignedToMeterGuid"].Value = this.AssignedToMeterGuid;
				}
			}

			cmd.Parameters["@ColumnNameIdentity"].Value = this.IdentityGuid;
		}

		public void InsertSQL(SqlCommand cmd)
		{
			string specialInstructionsInsertString1 = "";
			string specialInstructionsInsertString2 = "";
			if (ContainsSpecialInstructions(this.Type))
			{
				specialInstructionsInsertString1 = "SpecialInstructionNote,";
				specialInstructionsInsertString2 = "@SpecialInstructionNote,";
			}

			cmd.CommandText = "INSERT INTO " + GetMappingTableName(this.Type) +
				"(" + GetAssignedToColumnName(this.Type) + "," +
				"ProductGuid," +
				"Sequence," +
				"BlendPercentage," +
				"AdditiveRate," +
				"Ratio," +
				"AdditiveCycleVolume," +
                ((this.Type == PRODUCT_MAP_TYPE.ADDITIVE_PROFILE_MAP) ? "DesiredTreatRate," : "") +
                "Tolerance," +
				"PresetNumber," +
				"AdditiveProfileGuid," +
                "TankGuid," +
                ((this.Type == PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANK_MAP 
                  || this.Type == PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANKGROUP_MAP
                  || this.Type == PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_MAP 
                  || this.Type == PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_TANKGROUP_MAP) ? "TankGroupApplicationStringGuid," : "") +
                "ShipToProductID," +
				"ShipToProductCode," +
				"ShipToLoadRackDisplayText," +
				specialInstructionsInsertString1 +
				"UnavailableInventoryGross," +
				"UnavailableInventoryNet," +
                ((this.Type == PRODUCT_MAP_TYPE.PRESET_RECIPE_MAP) ? "EnableRecipe," : "") +
				"CreatedDate," +
				"CreatedBy," +
				"UpdatedDate," +
				"UpdatedBy," +
				GetIdentityColumnName(this.Type) +
				(ContainsMeter(this.Type) ? ", AssignedToMeterGuid" : string.Empty) +
				") VALUES (" +
				"@AssignedToGuid, " +
				"@ProductGuid," +
				"@Sequence," +
				"@BlendPercentage," +
				"@AdditiveRate," +
				"@Ratio," +
				"@AdditiveCycleVolume," +
                ((this.Type == PRODUCT_MAP_TYPE.ADDITIVE_PROFILE_MAP) ? "@DesiredTreatRate," : "") +
                "@Tolerance," +
				"@PresetNumber," +
				"@AdditiveProfileGuid," +
				"@TankGuid," +
                ((this.Type == PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANK_MAP 
                    || this.Type == PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANKGROUP_MAP
                    || this.Type == PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_MAP
                    || this.Type == PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_TANKGROUP_MAP) ? "@TankGroupGuid," : "") +
                "@ShipToProductID," +
				"@ShipToProductCode," +
				"@ShipToLoadRackDisplayText," +
				specialInstructionsInsertString2 +
				"@UnavailableInventoryGross," +
				"@UnavailableInventoryNet," +
                 ((this.Type == PRODUCT_MAP_TYPE.PRESET_RECIPE_MAP) ? "@EnableRecipe," : "") +
				"@CreatedDate," +
				"@CreatedBy," +
				"@UpdatedDate," +
				"@UpdatedBy," +
				"@" + GetIdentityColumnName(this.Type) +
				(ContainsMeter(this.Type) ? ", @AssignedToMeterGuid" : string.Empty) +
				")";

			cmd.Parameters.Add("@AssignedToGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@ProductGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@Sequence", SqlDbType.Int);
			cmd.Parameters.Add("@BlendPercentage", SqlDbType.Float);
			cmd.Parameters.Add("@AdditiveRate", SqlDbType.Float);
			cmd.Parameters.Add("@Ratio", SqlDbType.Float);
			cmd.Parameters.Add("@AdditiveCycleVolume", SqlDbType.Float);
            if(this.Type == PRODUCT_MAP_TYPE.ADDITIVE_PROFILE_MAP)
                cmd.Parameters.Add("@DesiredTreatRate", SqlDbType.Float);
			cmd.Parameters.Add("@Tolerance", SqlDbType.Float);
			cmd.Parameters.Add("@PresetNumber", SqlDbType.Int);
			cmd.Parameters.Add("@AdditiveProfileGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@TankGuid", SqlDbType.UniqueIdentifier);

            if (this.Type == PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANK_MAP 
                || this.Type == PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANKGROUP_MAP
                || this.Type == PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_MAP 
                || this.Type == PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_TANKGROUP_MAP)
            {
                cmd.Parameters.Add("@TankGroupGuid", SqlDbType.UniqueIdentifier);
            }

            cmd.Parameters.Add("@ShipToProductID", SqlDbType.NVarChar, 30);
			cmd.Parameters.Add("@ShipToProductCode", SqlDbType.NVarChar, 15);
			cmd.Parameters.Add("@ShipToLoadRackDisplayText", SqlDbType.NVarChar, 10);
			if (ContainsSpecialInstructions(this.Type))
			{
				cmd.Parameters.Add("@SpecialInstructionNote", SqlDbType.NVarChar, 2000);
			}
			cmd.Parameters.Add("@UnavailableInventoryGross", SqlDbType.Float);
			cmd.Parameters.Add("@UnavailableInventoryNet", SqlDbType.Float);
            if (ContainsMeter(this.Type))
            {
                cmd.Parameters.Add("@AssignedToMeterGuid", SqlDbType.UniqueIdentifier);
            }
			cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@" + GetIdentityColumnName(this.Type), SqlDbType.UniqueIdentifier);			

            if (this.Type == PRODUCT_MAP_TYPE.PRESET_RECIPE_MAP)
                cmd.Parameters.Add("@EnableRecipe", SqlDbType.Bit);

			cmd.Parameters["@AssignedToGuid"].Value = this.AssignedToGuid;
			cmd.Parameters["@ProductGuid"].Value = this.AssignedGuid;
			cmd.Parameters["@Sequence"].Value = this.Sequence;
			cmd.Parameters["@BlendPercentage"].Value = this.BlendPercentage;
			cmd.Parameters["@AdditiveRate"].Value = this._AdditiveRate.SIValue;
			cmd.Parameters["@Ratio"].Value = this.Ratio;
			cmd.Parameters["@AdditiveCycleVolume"].Value = this._AdditiveCycleVolume.SIValue;
            if (this.Type == PRODUCT_MAP_TYPE.ADDITIVE_PROFILE_MAP)
                cmd.Parameters["@DesiredTreatRate"].Value = this.DesiredTreatRate;
            cmd.Parameters["@Tolerance"].Value = this.Tolerance;
			cmd.Parameters["@PresetNumber"].Value = this.PresetNumber;

            if (this.AdditiveProfileGuid != Guid.Empty)
			{ cmd.Parameters["@AdditiveProfileGuid"].Value = this.AdditiveProfileGuid; }
			else
			{ cmd.Parameters["@AdditiveProfileGuid"].Value = DBNull.Value; }

            if (this.Type == PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANK_MAP 
                || this.Type == PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANKGROUP_MAP
                || this.Type == PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_MAP 
                || this.Type == PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_TANKGROUP_MAP)
            {
                if (this.Type == PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANK_MAP
                    || this.Type == PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_MAP)
                {
                    if (this.TankOrGroupGuid != Guid.Empty)
                    {
                        cmd.Parameters["@TankGuid"].Value = this.TankOrGroupGuid;
                    }
                    else
                    {
                        cmd.Parameters["@TankGuid"].Value = DBNull.Value;
                    }
                    cmd.Parameters["@TankGroupGuid"].Value = DBNull.Value;
                }
                else
                {
                    if (this.TankOrGroupGuid != Guid.Empty)
                    {
                        cmd.Parameters["@TankGroupGuid"].Value = this.TankOrGroupGuid;
                    }
                    else
                    {
                        cmd.Parameters["@TankGroupGuid"].Value = DBNull.Value;
                    }
                    cmd.Parameters["@TankGuid"].Value = DBNull.Value;
                }
            }

            else
            {
                if (this.TankOrGroupGuid != Guid.Empty)
                {
                    cmd.Parameters["@TankGuid"].Value = this.TankOrGroupGuid;
                }
                else
                {
                    cmd.Parameters["@TankGuid"].Value = DBNull.Value;
                }
            }

            cmd.Parameters["@ShipToProductID"].Value = (String.IsNullOrEmpty(this._ShipToProductID)) ? "" : this._ShipToProductID;
			cmd.Parameters["@ShipToProductCode"].Value = (String.IsNullOrEmpty(this._ShipToProductCode)) ? "" : this._ShipToProductCode;
			cmd.Parameters["@ShipToLoadRackDisplayText"].Value = (String.IsNullOrEmpty(this._ShipToLoadRackDisplayText)) ? "" : this._ShipToLoadRackDisplayText;

			//cmd.Parameters["@SpecialInstructionNoteGuid"].Value = SpecialInstructionGuid;
			if (ContainsSpecialInstructions(this.Type))
			{
				if (this.SpecialInstructions != string.Empty)
				{ 
					cmd.Parameters["@SpecialInstructionNote"].Value = this.SpecialInstructions; 
				}
				else
				{ 
					cmd.Parameters["@SpecialInstructionNote"].Value = DBNull.Value; 
				}
			}

			cmd.Parameters["@UnavailableInventoryGross"].Value = this._UnavailableInventoryGross.SIValue;
			cmd.Parameters["@UnavailableInventoryNet"].Value = this._UnavailableInventoryNet.SIValue;
            if (this.Type == PRODUCT_MAP_TYPE.PRESET_RECIPE_MAP)
                cmd.Parameters["@EnableRecipe"].Value = (this.EnableRecipe ? 1 : 0);
			cmd.Parameters["@CreatedDate"].Value = this.CreatedDate;
			cmd.Parameters["@CreatedBy"].Value = this.CreatedBy;
			cmd.Parameters["@UpdatedDate"].Value = this.UpdatedDate;
			cmd.Parameters["@UpdatedBy"].Value = this.UpdatedBy;
			cmd.Parameters["@" + GetIdentityColumnName(this.Type)].Value = this._IdentityGuid;
			if (ContainsMeter(this.Type))
			{
				if (this.AssignedToMeterGuid == Guid.Empty)
				{
					cmd.Parameters["@AssignedToMeterGuid"].Value = DBNull.Value;
				}
                else
				{
					cmd.Parameters["@AssignedToMeterGuid"].Value = this.AssignedToMeterGuid;
				}
			}

		}

		public void PurgeSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM " + GetMappingTableName(this.Type) + " WHERE " + GetIdentityColumnName(this.Type) + " = @Identity";
			cmd.Parameters.Add("@Identity", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@Identity"].Value = this.IdentityGuid;
		}

		public void SelectSQL(SqlCommand cmd, Guid targetSiteGuid)
		{
			cmd.CommandText = this.Select() + this.LocationIDQuery + this.GuidIDQuery +
				" FROM " + GetMappingTableName(this.Type) + this.Join() +
				" WHERE " + GetIdentityColumnName(this.Type) + " = @IdentityGuid";

			cmd.Parameters.Add("@IdentityGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@IdentityGuid"].Value = this.IdentityGuid;
            cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters["@TargetSiteGuid"].Value = targetSiteGuid;
		}

		public void SelectIdentityGuidSQL(SqlCommand cmd)
		{
			cmd.CommandText = "SELECT " + GetIdentityColumnName(this.Type) +
					" FROM " + GetMappingTableName(this.Type) +
					" WHERE " + GetAssignedToColumnName(this.Type) + " = @AssignedToGuid " +
					" AND ProductGuid = @AssignedGuid";

			cmd.Parameters.Add("@AssignedGuid", SqlDbType.UniqueIdentifier).Value = this.AssignedGuid;
            cmd.Parameters.Add("@AssignedToGuid", SqlDbType.UniqueIdentifier).Value = this.AssignedToGuid;
		}

		public void EnumerateByAssignedGuidAndTypeSQL(SqlCommand cmd, Guid targetSiteGuid)
		{
			string where = " WHERE " + GetMappingTableName(this.Type) + ".ProductGuid = @AssignedGuid ";

			// Since tank and tank group components are stored in the same table, we need to add additional
			// logic to select one or the other only, and not both.
			if (this.Type == PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANK_MAP
                || this.Type == PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_MAP)
			{
				where += " AND " + GetMappingTableName(this.Type) + ".TankGuid IS NOT NULL";
			}
			else if (this.Type == PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANKGROUP_MAP
                        || this.Type == PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_TANKGROUP_MAP)
			{
				where += " AND " + GetMappingTableName(this.Type) + ".TankGroupApplicationStringGuid IS NOT NULL";
			}

			cmd.CommandText = this.Select() + this.LocationIDQuery + this.GuidIDQuery +
				" FROM " + GetMappingTableName(this.Type) + this.Join() +
				where + this.OrderBy;

			cmd.Parameters.Add("@AssignedGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@AssignedGuid"].Value = this.AssignedGuid;
            cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters["@TargetSiteGuid"].Value = targetSiteGuid;
		}

		public void EnumerateByMeterGuidSQL(SqlCommand cmd, Guid meterGuid, Guid targetSiteGuid)
		{
			string where = " WHERE AssignedToMeterGuid = @MeterGuid ";

			// Since tank and tank group components are stored in the same table, we need to add additional
			// logic to select one or the other only, and not both.
			if (this.Type == PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANK_MAP
                || this.Type == PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_MAP)
			{
				where += " AND " + GetMappingTableName(this.Type) + ".TankGuid IS NOT NULL";
			}
			else if (this.Type == PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANKGROUP_MAP
                        || this.Type == PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_TANKGROUP_MAP)
			{
				where += " AND " + GetMappingTableName(this.Type) + ".TankGroupApplicationStringGuid IS NOT NULL";
			}

			cmd.CommandText = this.Select() + this.LocationIDQuery + this.GuidIDQuery +
				" FROM " + GetMappingTableName(this.Type) + this.Join() + where;

			cmd.Parameters.Add("@MeterGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@MeterGuid"].Value = meterGuid;
            cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters["@TargetSiteGuid"].Value = targetSiteGuid;
		}

		public void EnumerateByAssignedToGuidAndTypeSQL(SqlCommand cmd, bool bInTransaction, Guid targetSiteGuid, bool hideHiddenProducts = false)
		{
			string where;
            if (this.Type == PRODUCT_MAP_TYPE.TRANSACTION_ALIAS_EXCLUSION_MAP)
                where = GetAssignedToColumnName(this.Type) + " = [erv].[udf_GetFirstParentRecordVersionGuid] ('Transaction_Alias', @AssignedToGuid, @TargetSiteGuid)";
            else
			    where = GetAssignedToColumnName(this.Type) + " = @AssignedToGuid";

			// Since tank and tank group components are stored in the same table, we need to add additional
			// logic to select one or the other only, and not both.
			if (this.Type == PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANK_MAP
                || this.Type == PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_MAP)
			{
				where += " AND " + GetMappingTableName(this.Type) + ".TankGuid IS NOT NULL";
			}
			else if (this.Type == PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANKGROUP_MAP
                || this.Type == PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_TANKGROUP_MAP)
			{
				where += " AND " + GetMappingTableName(this.Type) + ".TankGroupApplicationStringGuid IS NOT NULL";
			}

		    if (hideHiddenProducts)
		    {
		        where += " AND (tblProducts.HiddenDate IS NULL) ";
		    }

			cmd.CommandText = this.Select() + this.LocationIDQuery + this.GuidIDQuery +
				" FROM " + GetMappingTableName(this.Type) + " " + SQLUpdateLock(bInTransaction) + this.Join() + " WHERE " + where + this.OrderBy;

			cmd.Parameters.Add("@AssignedToGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@AssignedToGuid"].Value = this.AssignedToGuid;
            cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters["@TargetSiteGuid"].Value = targetSiteGuid;
		}

		/// <summary>
		/// Populate a SqlCommand object with the SQL and parameters to retrieve 
		/// product maps with special instruction text assigned to the specified company.
		/// Only product maps with special instructions will be returned.
		/// </summary>
		/// <param name="cmd">The SqlCommand to populate</param>
		public void EnumerateSpecialInstructionsByAssignedToCompanySQL(SqlCommand cmd)
		{
			cmd.CommandText = "SELECT map.tblProductToCompany.* " +
				"FROM map.tblProductToCompany " +
				"WHERE map.tblProductToCompany.AssignedToCompanyGuid = @AssignedToGuid";

			cmd.Parameters.Add("@AssignedToGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@AssignedToGuid"].Value = this.AssignedToGuid;
		}

		public void EnumerateByTypeSQL(SqlCommand cmd, SecurityClass security)
		{
			string where = string.Empty;

			// Since tank and tank group components are stored in the same table, we need to add additional
			// logic to select one or the other only, and not both.
			if (this.Type == PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANK_MAP
                || this.Type == PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_MAP)
			{
				where += " AND " + GetMappingTableName(this.Type) + ".TankGuid IS NOT NULL";
			}
			else if (this.Type == PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANKGROUP_MAP
                || this.Type == PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_TANKGROUP_MAP)
			{
				where += " AND " + GetMappingTableName(this.Type) + ".TankGroupApplicationStringGuid IS NOT NULL";
			}

			cmd.CommandText = this.Select() + this.LocationIDQuery + this.GuidIDQuery +
					" FROM " + GetMappingTableName(this.Type) + " " + this.Join() +
					" WHERE " + GetMappingTableName(this.Type) + ".ProductGuid IN (" +
					" SELECT ProductGuid FROM tblProducts" +
                    " WHERE tblProducts.ProductGuid IN (SELECT ProductGuid FROM [erv].[udf_GetProductRecordVersions] (@TargetSiteGuid))" + ")" +
					where + this.OrderBy;
            cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;

		}

		public void EnumerateByTypeAndMasterProductSQL(SqlCommand cmd, SecurityClass security)
		{
			string where = string.Empty;

			cmd.CommandText = this.Select() + this.LocationIDQuery + this.GuidIDQuery +
					" FROM " + GetMappingTableName(this.Type) + " " + this.Join() +
					" WHERE " + GetMappingTableName(this.Type) + ".ProductGuid IN (" +
					" SELECT MasterRecordGuid FROM tblProducts" +
					" WHERE tblProducts.ProductGuid IN (SELECT ProductGuid FROM [erv].[udf_GetProductRecordVersions] (@TargetSiteGuid))" + ")" +
					where + this.OrderBy;
			cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;

		}


		public void EnumerateByAdditiveProfileGuidSQL(SqlCommand cmd, Guid targetSiteGuid)
		{
			cmd.CommandText = this.Select() + this.LocationIDQuery + this.GuidIDQuery 
				+ " FROM " + GetMappingTableName(this.Type) + " " + this.Join() 
				+ " WHERE " + GetMappingTableName(this.Type) + " " + ".AdditiveProfileGuid = @AdditiveProfileGuid ";

			cmd.Parameters.Add("@AdditiveProfileGuid", SqlDbType.UniqueIdentifier).Value = this.AdditiveProfileGuid;
            cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier).Value = targetSiteGuid;
		}

		#endregion
	}
}

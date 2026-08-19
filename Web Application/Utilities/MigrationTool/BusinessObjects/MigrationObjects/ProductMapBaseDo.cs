namespace BusinessObjects.MigrationObjects
{
    using System;
    using System.Data;
    using System.Data.SqlClient;

    public class ProductMapBaseDo : MigrationBaseDo
    {
        #region Data members
        public enum ProductMapTypes
        {
            UNDEFINED_MAP                           = 0,
            BLEND_COMPONENT_MAP                     = 1,
            PRODUCT_GROUP_MAP                       = 2,
            PRESET_RECIPE_MAP                       = 3,
            PRESET_INJECTOR_MAP                     = 4,
            ADDITIVE_PROFILE_MAP                    = 5,
            PRODUCT_COMPANY_MAP                     = 6,
            PRESET_COMPONENT_TANK_MAP               = 7,
            TRANSACTION_ALIAS_EXCLUSION_MAP         = 8,
            PRODUCT_COMPANY_GROUP_MAP               = 9,
            PRESET_COMPONENT_TANKGROUP_MAP          = 10,
            UNAVAILABLE_INVENTORY_COMPANY_MAP       = 11,     
            PRESET_EXTERNAL_COMPONENT_TANK_MAP      = 12,
            SUPPLIER_PRODUCT_COMPANY_MAP            = 13,
            PRESET_FLOW_CONTROLLED_ADDITIVE_MAP     = 14,
            OFFLOAD_EXTERNAL_METER_MAP              = 15,
            VRU_VCU_TRACKING                        = 16,
            PRESET_EXTERNAL_COMPONENT_TANKGROUP_MAP = 17,
            MAX_MAP                                 = 18
        };
        #endregion

        #region Constructors
        /// <summary>
        /// This is the initializer constructor
        /// </summary>
        public ProductMapBaseDo(string sourceDbName, string targetDbName)
        {
            this.SourceDbName = sourceDbName;
            this.TargetDbName = targetDbName;
            this.Init();
        }

        /// <summary>
        /// This is the default constructor
        /// </summary>
        public ProductMapBaseDo()
        {
            this.Init();
        }
        #endregion

        #region Properties
        public int Index { get; set; }
        public int AssignedToIndex { get; set; }    // AssignedTo is what is being mapped to the product (assigned).
        public int AssignedIndex { get; set; }      // Assigned is the product that is being mapped.
        public int Type { get; set; }
        public int Sequence { get; set; }
        public double BlendPercentage { get; set; }
        public double AdditiveRate { get; set; }
        public double Ratio { get; set; }
        public double AdditiveCycleVolume { get; set; }
        public double Tolerance { get; set; }
        public int PresetNumber { get; set; }
        public int? AdditiveProfileIndex { get; set; }
        public int? TankIndex { get; set; }
        public string MeterId { get; set; }
        public string ShipToProductId { get; set; }
        public string ShipToProductCode { get; set; }
        public string ShipToLoadRackDisplayText { get; set; }
        public int? SpecialInstructionIndex { get; set; }
        public double? UnavailableInventoryGross { get; set; }
        public double? UnavailableInventoryNet { get; set; }
        public double? DesiredTreatRate { get; set; }
        public bool EnableRecipe { get; set; }

        public string AdditiveProfileId { get; set; }
        public string LoadRackText { get; set; }
        public string AssignedProductId { get; set; }
        public string AssignedToProductId { get; set; }
        public string TankId { get; set; }

        public string SourceDbName { get; set; }
        public string TargetDbName { get; set; }
        #endregion

        #region public methods
        /// <summary>
        /// This method creates the sql command string to retrieve the data.
        /// </summary>
        /// <param name="command">SQL command object.</param>
        public virtual void EnumerateSourceProductMapSql(SqlCommand command)
        {
            if (string.IsNullOrEmpty(this.SourceDbName))
            {
                return;
            }

            string select = " SELECT PM.*"
                            + ", P1.ProductID AS AssignedProductID"
                            + ", P2.ProductID AS AssignedToProductID"
                            + ", LA.LoadRackText"
                            + ", AP.ID AS AdditiveProfileID"
                            + ", T.TankID";
            string from = " FROM " + this.SourceDbName + ".dbo.tblProductMap PM"
                            + " LEFT JOIN " + this.SourceDbName + ".dbo.tblProducts P1 ON P1.ProductIndex = PM.AssignedIndex"
                            + " LEFT JOIN " + this.SourceDbName + ".dbo.tblProducts P2 ON P2.ProductIndex = PM.AssignedToIndex"
                            + " LEFT JOIN " + this.SourceDbName + ".dbo.tblLoadArms LA ON LA.[Index] = PM.AssignedToIndex"
                            + " LEFT JOIN " + this.SourceDbName + ".dbo.tblAdditiveProfiles AP ON AP.[Index] = PM.AssignedToIndex"
                            + " LEFT JOIN " + this.SourceDbName + ".dbo.tblTanks T ON T.TankIndex = PM.TankIndex";

            string where = " WHERE PM.Type IN ("
                                            + (int)ProductMapTypes.ADDITIVE_PROFILE_MAP
                                            + ", " + (int)ProductMapTypes.BLEND_COMPONENT_MAP
                                            + ", " + (int)ProductMapTypes.OFFLOAD_EXTERNAL_METER_MAP
                                            + ", " + (int)ProductMapTypes.PRESET_COMPONENT_TANKGROUP_MAP
                                            + ", " + (int)ProductMapTypes.PRESET_COMPONENT_TANK_MAP
                                            + ", " + (int)ProductMapTypes.PRESET_EXTERNAL_COMPONENT_TANKGROUP_MAP
                                            + ", " + (int)ProductMapTypes.PRESET_EXTERNAL_COMPONENT_TANK_MAP
                                            + ", " + (int)ProductMapTypes.PRESET_FLOW_CONTROLLED_ADDITIVE_MAP
                                            + ", " + (int)ProductMapTypes.PRESET_INJECTOR_MAP
                                            + ", " + (int)ProductMapTypes.PRESET_RECIPE_MAP
                                            + ")";

            command.CommandText = select + from + where;
        }

        /// <summary>
        /// This method will load one row.
        /// </summary>
        /// <param name="row">The row to be loaded.</param>
        public virtual void Load(DataRow row)
        {
            this.Index                      = row.IsNull("Index") ? -99 : (int)row["Index"];
            this.AssignedToIndex            = row.IsNull("AssignedToIndex") ? -99 : (int)row["AssignedToIndex"];
            this.AssignedIndex              = row.IsNull("AssignedIndex") ? -99 : (int)row["AssignedIndex"];
            this.Type                       = row.IsNull("Type") ? -99 : (int)row["Type"];
            this.Sequence                   = row.IsNull("Sequence") ? -99 : (int)row["Sequence"];
            this.BlendPercentage            = row.IsNull("BlendPercentage") ? 0.0 : (double)row["BlendPercentage"];
            this.AdditiveRate               = row.IsNull("AdditiveRate") ? 0.0 : (double)row["AdditiveRate"];
            this.Ratio                      = row.IsNull("Ratio") ? 0.0 : (double)row["Ratio"];
            this.AdditiveCycleVolume        = row.IsNull("AdditiveCycleVolume") ? 0.0 : (double)row["AdditiveCycleVolume"];
            this.Tolerance                  = row.IsNull("Tolerance") ? 0.0 : (double)row["Tolerance"];
            this.PresetNumber               = row.IsNull("PresetNumber") ? -99 : (int)row["PresetNumber"];
            this.AdditiveProfileIndex       = row.IsNull("AdditiveProfileIndex") ? null : (int?)row["AdditiveProfileIndex"];
            this.TankIndex                  = row.IsNull("TankIndex") ? null : (int?)row["TankIndex"];
            this.MeterId                    = row.IsNull("MeterID") ? string.Empty : (string)row["MeterID"];
            this.ShipToProductId            = row.IsNull("ShipToProductID") ? string.Empty : (string)row["ShipToProductID"];
            this.ShipToProductCode          = row.IsNull("ShipToProductCode") ? string.Empty : (string)row["ShipToProductCode"];
            this.ShipToLoadRackDisplayText  = row.IsNull("ShipToLoadRackDisplayText") ? string.Empty : (string)row["ShipToLoadRackDisplayText"];
            this.SpecialInstructionIndex    = row.IsNull("SpecialInstructionIndex") ? null : (int?)row["SpecialInstructionIndex"];
            this.UnavailableInventoryGross  = row.IsNull("UnavailableInventoryGross") ? null : (double?)row["UnavailableInventoryGross"];
            this.UnavailableInventoryNet    = row.IsNull("UnavailableInventoryNet") ? null : (double?)row["UnavailableInventoryNet"];
            this.DesiredTreatRate           = row.IsNull("DesiredTreatRate") ? null : (double?)row["DesiredTreatRate"];
            this.EnableRecipe               = row.IsNull("EnableRecipe") ? false : (bool)row["EnableRecipe"];
            this.AdditiveProfileId          = row.IsNull("AdditiveProfileID") ? string.Empty : (string)row["AdditiveProfileID"];
            this.LoadRackText               = row.IsNull("LoadRackText") ? string.Empty : (string)row["LoadRackText"];
            this.AssignedProductId          = row.IsNull("AssignedProductID") ? string.Empty : (string)row["AssignedProductID"];
            this.AssignedToProductId        = row.IsNull("AssignedToProductID") ? string.Empty : (string)row["AssignedToProductID"];
            this.TankId                     = row.IsNull("TankID") ? string.Empty : (string)row["TankID"];
        }
        #endregion

        #region Protected methods
        /// <summary>
        /// This method initializes the object to its initial state.
        /// </summary>
        protected void Init()
        {
            this.Index                      = -99;
            this.AssignedToIndex            = -99;
            this.AssignedIndex              = -99;
            this.Type                       = -99;
            this.Sequence                   = -99;
            this.BlendPercentage            = 0.0;
            this.AdditiveRate               = 0.0;
            this.Ratio                      = 0.0;
            this.AdditiveCycleVolume        = 0.0;
            this.Tolerance                  = 0.0;
            this.PresetNumber               = -99;
            this.AdditiveProfileIndex       = null;
            this.TankIndex                  = null;
            this.MeterId                    = string.Empty;
            this.ShipToProductId            = string.Empty;
            this.ShipToProductCode          = string.Empty;
            this.ShipToLoadRackDisplayText  = string.Empty;
            this.SpecialInstructionIndex    = null;
            this.UnavailableInventoryGross  = null;
            this.UnavailableInventoryNet    = null;
            this.DesiredTreatRate           = null;
            this.EnableRecipe               = false;
            this.AdditiveProfileId          = string.Empty;
            this.LoadRackText               = string.Empty;
            this.AssignedProductId          = string.Empty;
            this.AssignedToProductId        = string.Empty;
            this.TankId                     = string.Empty;
    }
        #endregion
    }
}

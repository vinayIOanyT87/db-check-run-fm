namespace BusinessObjects.MigrationObjects
{
    using System.Data;
    using System.Data.SqlClient;
    using Varec.CommonComponents.EngineeringUnitsLibrary;

    public class EquipmentTypeBaseDo
    {
        #region Data members
        enum EquipmentLookupTypes
        {
            TRAILER_TYPE = 0
            , TRACTOR_TYPE = 1
            , AIRCRAFT_TYPE = 2
            , RAILCAR_TYPE = 3
            , BARGE_TYPE = 4
            , COMPARTMENT_TYPE = 5
            , SHIP_TYPE = 6
            , PIPELINE_TYPE = 7
            , HYDRANT_CART_TYPE = 8
            , TANKER_TYPE = 9
            , STATIONARY_CART_TYPE = 10
            , OTHER_TYPE = 11
            , SYSTEM_TYPE = 12
            , TANK_TYPE = 13
            , FILLSTAND_TYPE = 14
            , CONTAINER = 15
            , VEHICLE = 16
            , INFRASTRUCTURE = 17
            , MAX_EQUIPMENT_TYPE = 18
        }
        #endregion

        #region Constructors
        /// <summary>
        /// This is the initializer constructor
        /// </summary>
        public EquipmentTypeBaseDo(string sourceDbName, string targetDbName)
        {
            this.SourceDbName = sourceDbName;
            this.TargetDbName = targetDbName;
            this.Init();
        }

        /// <summary>
        /// This is the default constructor
        /// </summary>
        public EquipmentTypeBaseDo()
        {
            this.Init();
        }
        #endregion

        #region Properties
        public int EqTypeIndex { get; set; }
        public int SiteIndex { get; set; }
        public string EqTypeName { get; set; }
        public string EqTypeDescription { get; set; }
        public double? Capacity { get; set; }
        public double? SafeFill { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public short? Year { get; set; }
        public int? Attribute { get; set; }
        public bool DeleteFlag { get; set; }
        public string IssPt { get; set; }
        public bool MultiCompartment { get; set; }

        public int VolumeUnitIndex { get; set; }
        public int TemperatureUnitIndex { get; set; }
        public int MassUnitIndex { get; set; }
        public int AdditiveVolumeUnitIndex { get; set; }
        public int DensityUnitIndex { get; set; }

        public string SourceDbName { get; set; }
        public string TargetDbName { get; set; }
        #endregion


        #region public methods
        /// <summary>
        /// This method creates the sql command string to retrieve the data.
        /// </summary>
        /// <param name="command">SQL command object.</param>
        public virtual void EnumerateEquipmentTypesSql(SqlCommand command)
        {
            if (string.IsNullOrEmpty(this.SourceDbName))
            {
                return;
            }

            string select = " SELECT ET.*, S.VolumeUnitIndex, S.TemperatureUnitIndex, S.MassUnitIndex, S.DensityUnitIndex, S.AdditiveVolumeUnitIndex";
            string from = " FROM " + this.SourceDbName + ".dbo.tblEquipmentTypes ET"
                            + " LEFT JOIN " + this.SourceDbName + ".dbo.tblSites S ON S.SiteIndex = ET.SiteIndex";

            command.CommandText = select + from;
        }

        /// <summary>
        /// This method will load one row.
        /// </summary>
        /// <param name="row">The row to be loaded.</param>
        public virtual void Load(DataRow row)
        {
            this.EqTypeIndex                = row.IsNull("EqTypeIndex") ? -99 : (int)row["EqTypeIndex"];
            this.SiteIndex                  = row.IsNull("SiteIndex") ? -99 : (int)row["SiteIndex"];
            this.EqTypeName                 = row.IsNull("EqTypeName") ? string.Empty : (string)row["EqTypeName"];
            this.EqTypeDescription          = row.IsNull("EqTypeDescription") ? string.Empty : (string)row["EqTypeDescription"];
            this.EqTypeIndex                = row.IsNull("EqTypeIndex") ? -99 : (int)row["EqTypeIndex"];
            this.EqTypeName                 = row.IsNull("EqTypeName") ? string.Empty : (string)row["EqTypeName"];
            this.Capacity                   = row.IsNull("Capacity") ? null : (double?)row["Capacity"];
            this.SafeFill                   = row.IsNull("SafeFill") ? null : (double?)row["SafeFill"];
            this.Make                       = row.IsNull("Make") ? string.Empty : (string)row["Make"];
            this.Model                      = row.IsNull("Model") ? string.Empty : (string)row["Model"];
            this.Year                       = row.IsNull("Year") ? null : (short?)row["Year"];
            this.Attribute                  = row.IsNull("Attribute") ? null : (int?)row["Attribute"];
            this.DeleteFlag                 = row.IsNull("DeleteFlag") ? false : (bool)row["DeleteFlag"];
            this.IssPt                      = row.IsNull("IssPt") ? string.Empty : (string)row["IssPt"];
            this.MultiCompartment           = row.IsNull("MultiCompartment") ? false : (bool)row["MultiCompartment"];
            this.VolumeUnitIndex            = row.IsNull("VolumeUnitIndex") ? (int)EngineeringUnit.FmvUsGal : (int)row["VolumeUnitIndex"];
            this.TemperatureUnitIndex       = row.IsNull("TemperatureUnitIndex") ? (int)EngineeringUnit.FmtDegF : (int)row["TemperatureUnitIndex"];
            this.MassUnitIndex              = row.IsNull("MassUnitIndex") ? (int)EngineeringUnit.FmmLb : (int)row["MassUnitIndex"];
            this.AdditiveVolumeUnitIndex    = row.IsNull("AdditiveVolumeUnitIndex") ? (int)EngineeringUnit.FmvCm3 : (int)row["AdditiveVolumeUnitIndex"];
            this.DensityUnitIndex           = row.IsNull("DensityUnitIndex") ? (int)EngineeringUnit.FmdUsLbGal : (int)row["DensityUnitIndex"];
        }
        #endregion

        #region Protected methods
        /// <summary>
        /// This method initializes the object to its initial state.
        /// </summary>
        protected void Init()
        {
            this.EqTypeIndex                = -99;
            this.SiteIndex                  = -99;
            this.EqTypeName                 = string.Empty;
            this.EqTypeDescription          = string.Empty;
            this.Capacity                   = null;
            this.SafeFill                   = null;
            this.Make                       = string.Empty;
            this.Model                      = string.Empty;
            this.Year                       = null;
            this.Attribute                  = null;
            this.DeleteFlag                 = false;
            this.IssPt                      = string.Empty;
            this.MultiCompartment           = false;
            this.VolumeUnitIndex            = (int)EngineeringUnit.FmvUsGal;
            this.TemperatureUnitIndex       = (int)EngineeringUnit.FmtDegF;
            this.MassUnitIndex              = (int)EngineeringUnit.FmmLb;
            this.AdditiveVolumeUnitIndex    = (int)EngineeringUnit.FmvCm3;
            this.DensityUnitIndex           = (int)EngineeringUnit.FmdUsLbGal;
    }
        #endregion
    }
}

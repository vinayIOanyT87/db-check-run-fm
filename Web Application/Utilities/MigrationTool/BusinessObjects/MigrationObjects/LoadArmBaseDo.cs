namespace BusinessObjects.MigrationObjects
{
    using System;
    using System.Data;
    using System.Data.SqlClient;

    public class LoadArmBaseDo : MigrationBaseDo
    {
        #region Constructors
        /// <summary>
        /// This is the initializer constructor
        /// </summary>
        public LoadArmBaseDo(string sourceDbName, string targetDbName)
        {
            this.SourceDbName = sourceDbName;
            this.TargetDbName = targetDbName;
            this.Init();
        }

        /// <summary>
        /// This is the default constructor
        /// </summary>
        public LoadArmBaseDo()
        {
            this.Init();
        }
        #endregion

        #region Properties
        public int Index { get; set; }
        public string LoadRackText { get; set; }
        public int? BayAStationIndex { get; set; }
        public int? BayBStationIndex { get; set; }
        public bool Enabled { get; set; }
        public bool SwingArm { get; set; }
        public int PresetType { get; set; }
        public int? BayAArmNumber { get; set; }
        public int? BayBArmNumber { get; set; }
        public string BayAStationId { get; set; }
        public string BayBStationId { get; set; }

        public Guid LoadArmGuid { get; set; }
        public Guid BayAStationGuid { get; set; }
        public Guid BayBStationGuid { get; set; }
        public int LookupPresetTypeIndex { get; set; }

        public string SourceDbName { get; set; }
        public string TargetDbName { get; set; }

        public string StationId
        {
            get
            {
                if(string.IsNullOrEmpty(this.BayAStationId))
                {
                    return this.BayBStationId;
                }

                if (string.IsNullOrEmpty(this.BayBStationId))
                {
                    return this.BayAStationId;
                }

                return string.Empty;
            }
        }

        public string LoadArmMessageId
        {
            get
            {
                if (this.BayAStationGuid == Guid.Empty && this.BayBStationGuid == Guid.Empty)
                {
                    if (BayAStationIndex == -99 && string.IsNullOrEmpty(BayAStationId))
                    {
                        string idStr = "Bay B ID:" + this.BayBStationId + " and Arm Number: " + BayBArmNumber;
                        return idStr;
                    }

                    if (BayBStationIndex == -99 && string.IsNullOrEmpty(BayBStationId))
                    {
                        string idStr = "Bay A ID:" + this.BayAStationId + " and Arm Number: " + BayAArmNumber;
                        return idStr;
                    }
                }
                else
                {
                    if (BayAStationGuid == Guid.Empty && string.IsNullOrEmpty(BayAStationId))
                    {
                        string idStr = "Bay B ID:" + this.BayBStationId + " and Arm Number: " + BayBArmNumber;
                        return idStr;
                    }

                    if (BayBStationGuid == Guid.Empty && string.IsNullOrEmpty(BayBStationId))
                    {
                        string idStr = "Bay A ID:" + this.BayAStationId + " and Arm Number: " + BayAArmNumber;
                        return idStr;
                    }
                }

                return string.Empty;
            }
        }
        #endregion

        #region public methods
        /// <summary>
        /// This method creates the sql command string to retrieve the data.
        /// </summary>
        /// <param name="command">SQL command object.</param>
        public virtual void EnumerateSourceLoadArmsSql(SqlCommand command)
        {
            if (string.IsNullOrEmpty(this.SourceDbName))
            {
                return;
            }

            string select = " SELECT L.*, SA.ID AS BayAStationID, SB.ID AS BayBStationID";
            string from = " FROM " + this.SourceDbName + ".dbo.tblLoadArms L"
                + " LEFT JOIN " + this.SourceDbName + ".dbo.tblStations SA ON SA.[Index] = L.BayAStationIndex"
                + " LEFT JOIN " + this.SourceDbName + ".dbo.tblStations SB ON SB.[Index] = L.BayBStationIndex";

            command.CommandText = select + from;
        }

        /// <summary>
        /// This method creates the sql command string to retrieve the data.
        /// </summary>
        /// <param name="command">SQL command object.</param>
        public virtual void EnumerateTargetLoadArmsSql(SqlCommand command)
        {
            if (string.IsNullOrEmpty(this.TargetDbName))
            {
                return;
            }

            string select = " SELECT L.*, SA.ID AS BayAStationID, SB.ID AS BayBStationID";
            string from = " FROM " + this.TargetDbName + ".dbo.tblLoadArms L"
                + " LEFT JOIN " + this.TargetDbName + ".dbo.tblStations SA ON SA.StationGuid = L.BayAStationGuid"
                + " LEFT JOIN " + this.TargetDbName + ".dbo.tblStations SB ON SB.StationGuid = L.BayBStationGuid";

            command.CommandText = select + from;
        }

        /// <summary>
        /// This method will load one row.
        /// </summary>
        /// <param name="row">The row to be loaded.</param>
        public virtual void Load(DataRow row)
        {
            this.LoadRackText       = row.IsNull("LoadRackText") ? string.Empty : (string)row["LoadRackText"];
            this.Enabled            = row.IsNull("Enabled") ? false : (bool)row["Enabled"];
            this.SwingArm           = row.IsNull("SwingArm") ? false : (bool)row["SwingArm"];
            this.BayAArmNumber      = row.IsNull("BayAArmNumber") ? null : (int?)row["BayAArmNumber"];
            this.BayBArmNumber      = row.IsNull("BayBArmNumber") ? null : (int?)row["BayBArmNumber"];
            this.BayAStationId      = row.IsNull("BayAStationID") ? string.Empty : (string)row["BayAStationID"];
            this.BayBStationId      = row.IsNull("BayBStationID") ? string.Empty : (string)row["BayBStationID"];

            if (row.Table.Columns.Contains("Index"))
            {
                this.Index = row.IsNull("Index") ? -99 : (int)row["Index"];
            }

            if (row.Table.Columns.Contains("BayAStationIndex"))
            {
                this.BayAStationIndex = row.IsNull("BayAStationIndex") ? -99 : (int)row["BayAStationIndex"];
            }

            if (row.Table.Columns.Contains("BayBStationIndex"))
            {
                this.BayBStationIndex = row.IsNull("BayBStationIndex") ? -99 : (int)row["BayBStationIndex"];
            }

            if (row.Table.Columns.Contains("PresetType"))
            {
                this.PresetType = row.IsNull("PresetType") ? -99 : (int)row["PresetType"];
            }

            if (row.Table.Columns.Contains("LoadArmGuid"))
            {
                this.LoadArmGuid = row.IsNull("LoadArmGuid") ? Guid.Empty : (Guid)row["LoadArmGuid"];
            }

            if (row.Table.Columns.Contains("BayAStationGuid"))
            {
                this.BayAStationGuid = row.IsNull("BayAStationGuid") ? Guid.Empty : (Guid)row["BayAStationGuid"];
            }

            if (row.Table.Columns.Contains("BayBStationGuid"))
            {
                this.BayBStationGuid = row.IsNull("BayBStationGuid") ? Guid.Empty : (Guid)row["BayBStationGuid"];
            }

            if (row.Table.Columns.Contains("LookupPresetTypeIndex"))
            {
                this.LookupPresetTypeIndex = row.IsNull("LookupPresetTypeIndex") ? -99 : (int)row["LookupPresetTypeIndex"];
            }
        }
        #endregion

        #region Protected methods
        /// <summary>
        /// This method initializes the object to its initial state.
        /// </summary>
        protected void Init()
        {
            this.Index                  = -99;
            this.LoadRackText           = string.Empty;
            this.BayAStationIndex       = -99;
            this.BayBStationIndex       = -99;
            this.Enabled                = false;
            this.SwingArm               = false;
            this.PresetType             = -99;
            this.BayAArmNumber          = null;
            this.BayBArmNumber          = null;
            this.BayAStationId          = string.Empty;
            this.BayBStationId          = string.Empty;
            this.LoadArmGuid            = Guid.Empty;
            this.BayAStationGuid        = Guid.Empty;
            this.BayBStationGuid        = Guid.Empty;
            this.LookupPresetTypeIndex  = -99;
        }
        #endregion
    }
}

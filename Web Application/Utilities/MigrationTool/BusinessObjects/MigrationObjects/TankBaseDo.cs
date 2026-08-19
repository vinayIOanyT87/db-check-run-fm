namespace BusinessObjects.MigrationObjects
{
    using System.Data;
    using System.Data.SqlClient;

    public class TankBaseDo : MigrationBaseDo
    {
        #region Constructors
        /// <summary>
        /// This is the initializer constructor
        /// </summary>
        public TankBaseDo(string sourceDbName, string targetDbName)
        {
            this.SourceDbName = sourceDbName;
            this.TargetDbName = targetDbName;
            this.Init();
        }

        /// <summary>
        /// This is the default constructor
        /// </summary>
        public TankBaseDo()
        {
            this.Init();
        }
        #endregion

        #region Properties
        public int TankIndex { get; set; }
        public int SiteIndex { get; set; }
        public string TankId { get; set; }
        public int? ProductIndex { get; set; }
        public int? VesselTypeIndex { get; set; }
        public int? ManagerIndex { get; set; }
        public string ManagerId { get; set; }
        public string ProductId { get; set; }

        public string SourceDbName { get; set; }
        public string TargetDbName { get; set; }
        #endregion

        #region public methods
        /// <summary>
        /// This method creates the sql command string to retrieve the data.
        /// </summary>
        /// <param name="command">SQL command object.</param>
        public virtual void EnumerateTanksSql(SqlCommand command, int siteIndex)
        {
            if (string.IsNullOrEmpty(this.SourceDbName))
            {
                return;
            }

            string select = " SELECT T.*, C.ID AS CompanyID, P.ProductID";
            string from = " FROM " + this.SourceDbName + ".dbo.tblTanks T"
                        + " LEFT JOIN " + this.SourceDbName + ".dbo.tblCompanies C ON C.CompanyIndex = T.ManagerIndex"
                        + " LEFT JOIN " + this.SourceDbName + ".dbo.tblProducts P ON P.ProductIndex = T.ProductIndex";
            string where = " WHERE T.SiteIndex = " + siteIndex;

            command.CommandText = select + from + where;
        }

        /// <summary>
        /// This method will load one row.
        /// </summary>
        /// <param name="row">The row to be loaded.</param>
        public virtual void Load(DataRow row)
        {
            this.TankIndex          = row.IsNull("TankIndex") ? -99 : (int)row["TankIndex"];
            this.SiteIndex          = row.IsNull("SiteIndex") ? -99 : (int)row["SiteIndex"];
            this.TankId             = row.IsNull("TankID") ? string.Empty : (string)row["TankID"];
            this.ProductIndex       = row.IsNull("ProductIndex") ? null : (int?)row["ProductIndex"];
            this.VesselTypeIndex    = row.IsNull("VesselTypeIndex") ? null : (int?)row["VesselTypeIndex"];
            this.ManagerIndex       = row.IsNull("ManagerIndex") ? null : (int?)row["ManagerIndex"];
            this.ManagerId          = row.IsNull("CompanyID") ? string.Empty : (string)row["CompanyID"];
            this.ProductId          = row.IsNull("ProductID") ? string.Empty : (string)row["ProductID"];
        }
        #endregion

        #region Protected methods
        /// <summary>
        /// This method initializes the object to its initial state.
        /// </summary>
        protected void Init()
        {
            this.TankIndex          = -99;
            this.SiteIndex          = -99;
            this.TankId             = string.Empty;
            this.ProductIndex       = null;
            this.VesselTypeIndex    = null;
            this.ManagerIndex       = null;
            this.ManagerId          = string.Empty;
            this.ProductId          = string.Empty;
        }
        #endregion
    }
}

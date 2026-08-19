namespace BusinessObjects.MigrationObjects
{
    using System.Data;
    using System.Data.SqlClient;

    public class TankGroupsBaseDo
    {
        #region Constructors
        /// <summary>
        /// This is the initializer constructor
        /// </summary>
        public TankGroupsBaseDo(string sourceDbName, string targetDbName)
        {
            this.SourceDbName = sourceDbName;
            this.TargetDbName = targetDbName;
            this.Init();
        }

        /// <summary>
        /// This is the default constructor
        /// </summary>
        public TankGroupsBaseDo()
        {
            this.Init();
        }
        #endregion

        #region Properties
        public int Index { get; set; }
        public int SiteIndex { get; set; }
        public string Id { get; set; }
        public int? ProductIndex { get; set; }

        public string SourceDbName { get; set; }
        public string TargetDbName { get; set; }
        #endregion

        #region public methods
        /// <summary>
        /// This method creates the sql command string to retrieve the data.
        /// </summary>
        /// <param name="command">SQL command object.</param>
        public virtual void EnumerateTankGroupsSql(SqlCommand command, int siteIndex)
        {
            if (string.IsNullOrEmpty(this.SourceDbName))
            {
                return;
            }

            string select = " SELECT TG.*";
            string from = " FROM " + this.SourceDbName + ".dbo.tblTankGroups TG";
            string where = " WHERE TG.SiteIndex = " + siteIndex;

            command.CommandText = select + from + where;
        }

        /// <summary>
        /// This method will load one row.
        /// </summary>
        /// <param name="row">The row to be loaded.</param>
        public virtual void Load(DataRow row)
        {
            this.Index          = row.IsNull("Index") ? -99 : (int)row["Index"];
            this.SiteIndex      = row.IsNull("SiteIndex") ? -99 : (int)row["SiteIndex"];
            this.Id             = row.IsNull("ID") ? string.Empty : (string)row["ID"];
            this.ProductIndex   = row.IsNull("ProductIndex") ? null : (int?)row["ProductIndex"];
        }
        #endregion

        #region Protected methods
        /// <summary>
        /// This method initializes the object to its initial state.
        /// </summary>
        protected void Init()
        {
            this.Index          = -99;
            this.SiteIndex      = -99;
            this.Id             = string.Empty;
            this.ProductIndex   = null;
        }
        #endregion
    }
}

namespace BusinessObjects.MigrationObjects
{
    using System.Data;
    using System.Data.SqlClient;

    public class QualificationBaseDo : MigrationBaseDo
    {
        #region data members
        #endregion

        #region Constructors
        /// <summary>
        /// This is the initializer constructor
        /// </summary>
        public QualificationBaseDo(string sourceDbName, string targetDbName)
        {
            this.SourceDbName = sourceDbName;
            this.TargetDbName = targetDbName;
            this.Init();
        }

        /// <summary>
        /// This is the default constructor
        /// </summary>
        public QualificationBaseDo()
        {
            this.Init();
        }
        #endregion

        #region Properties
        public int Index { get; set; }
        public int Type { get; set; }
        public string Id { get; set; }
        public string Description { get; set; }
        public int SiteIndex { get; set; }
        public int Duration { get; set; }
        public int Reoccurance { get; set; }
        public string SiteId { get; set; }
        public string SourceDbName { get; set; }
        public string TargetDbName { get; set; }
        #endregion

        #region public methods
        /// <summary>
        /// This method creates the sql command string to retrieve the data.
        /// </summary>
        /// <param name="command">SQL command object.</param>
        public virtual void EnumerateQuantitiesSql(SqlCommand command, int siteIndex)
        {
            if(string.IsNullOrEmpty(this.SourceDbName))
            {
                return;
            }

            string select = " SELECT Q.[Index]"
                            + " , Q.[Type]"
                            + " , Q.ID"
                            + " , Q.[Description]"
                            + " , Q.SiteIndex"
                            + " , Q.Duration"
                            + " , Q.Reoccurrence"
                            + " , S.ID AS SiteID";
            string from = " FROM " + this.SourceDbName + ".dbo.tblQualifications Q INNER JOIN "
                            + this.SourceDbName + ".dbo.tblSites S ON Q.SiteIndex = S.SiteIndex ";

            string where = " WHERE Q.SiteIndex = " + siteIndex;

            command.CommandText = select + from + where;
        }

        /// <summary>
        /// This method will load one row.
        /// </summary>
        /// <param name="row">The row to be loaded.</param>
        public virtual void Load(DataRow row)
        {
            this.Index          = row.IsNull("Index") ? -1 : (int)row["Index"];
            this.Type           = row.IsNull("Type") ? -1 : (int)row["Type"];
            this.Id             = row.IsNull("ID") ? string.Empty : (string)row["ID"];
            this.Description    = row.IsNull("Description") ? string.Empty : (string)row["Description"];
            this.SiteIndex      = row.IsNull("SiteIndex") ? -1 : (int)row["SiteIndex"];
            this.Duration       = row.IsNull("Duration") ? 0 : (int)row["Duration"];
            this.Reoccurance    = row.IsNull("Reoccurrence") ? 0 : (int)row["Reoccurrence"];
            this.SiteId         = row.IsNull("SiteID") ? string.Empty : (string)row["SiteID"];
        }
        #endregion

        #region Protected methods
        /// <summary>
        /// This method initializes the object to its initial state.
        /// </summary>
        protected void Init()
        {
            this.Index          = -1;
            this.Type           = -1;
            this.Id             = string.Empty;
            this.Description    = string.Empty;
            this.SiteIndex      = -1;
            this.Duration       = 0;
            this.Reoccurance    = 0;
            this.SiteId         = string.Empty;
        }
        #endregion
    }
}

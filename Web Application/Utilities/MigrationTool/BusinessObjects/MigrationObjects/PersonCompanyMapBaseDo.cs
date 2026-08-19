namespace BusinessObjects.MigrationObjects
{
    using System.Data;
    using System.Data.SqlClient;

    public class PersonCompanyMapBaseDo
    {
        #region Constructors
        /// <summary>
        /// This is the initializer constructor
        /// </summary>
        public PersonCompanyMapBaseDo(string sourceDbName, string targetDbName)
        {
            this.SourceDbName = sourceDbName;
            this.TargetDbName = targetDbName;
            this.Init();
        }

        /// <summary>
        /// This is the default constructor
        /// </summary>
        public PersonCompanyMapBaseDo()
        {
            this.Init();
        }
        #endregion

        #region public methods
        /// <summary>
        /// This method creates the sql command string to retrieve the person role map data.
        /// </summary>
        /// <param name="command">SQL command object.</param>
        /// <param name="siteIndex">The site index to search on.</param>
        public virtual void EnumeratePersonCompanyMapSql(SqlCommand command, int siteIndex)
        {
            if (string.IsNullOrEmpty(this.SourceDbName))
            {
                return;
            }

            string select = " SELECT C.ID AS CompanyID, P.PersonID, P.PersonIndex, P.FirstName, P.MiddleName, P.LastName ";

            string from = " FROM " + this.SourceDbName + ".dbo.tblCompanyMap CM "
                          + " INNER JOIN " + this.SourceDbName + ".dbo.tblCompanies C ON C.CompanyIndex = CM.AssignedIndex"
                          + " INNER JOIN " + this.SourceDbName + ".dbo.tblPersonnel P ON P.PersonIndex = CM.AssignedToIndex";

            string where = " WHERE CM.SiteIndex = " + siteIndex;

            command.CommandText = select + from + where;
        }

        /// <summary>
        /// This method will load one row.
        /// </summary>
        /// <param name="row">The row to be loaded.</param>
        public virtual void Load(DataRow row)
        {
            this.PersonIndex    = row.IsNull("PersonIndex") ? -99 : (int)row["PersonIndex"];
            this.PersonId       = row.IsNull("PersonID") ? string.Empty : (string)row["PersonID"];
            this.CompanyId      = row.IsNull("CompanyID") ? string.Empty : (string)row["CompanyID"];
            this.FirstName      = row.IsNull("FirstName") ? string.Empty : (string)row["FirstName"];
            this.MiddleName     = row.IsNull("MiddleName") ? string.Empty : (string)row["MiddleName"];
            this.LastName       = row.IsNull("LastName") ? string.Empty : (string)row["LastName"];
        }
        #endregion

        #region Properties      
        public int PersonIndex { get; set; }
        public string CompanyId { get; set; }
        public string PersonId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }

        public string SourceDbName { get; set; }
        public string TargetDbName { get; set; }
        #endregion

        #region Protected methods
        /// <summary>
        /// This method initializes the object to its initial state.
        /// </summary>
        protected void Init()
        {
            this.PersonIndex = -99;
            this.PersonId = string.Empty;
            this.CompanyId = string.Empty;
            this.FirstName = string.Empty;
            this.MiddleName = string.Empty;
            this.LastName = string.Empty;
        }
        #endregion
    }
}

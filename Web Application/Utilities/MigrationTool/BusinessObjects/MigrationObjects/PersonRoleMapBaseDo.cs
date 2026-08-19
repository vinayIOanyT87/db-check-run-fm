namespace BusinessObjects.MigrationObjects
{
    using System.Data;
    using System.Data.SqlClient;

    public class PersonRoleMapBaseDo
    {
        #region Constructors
        /// <summary>
        /// This is the initializer constructor
        /// </summary>
        public PersonRoleMapBaseDo(string sourceDbName, string targetDbName)
        {
            this.SourceDbName = sourceDbName;
            this.TargetDbName = targetDbName;
            this.Init();
        }

        /// <summary>
        /// This is the default constructor
        /// </summary>
        public PersonRoleMapBaseDo()
        {
            this.Init();
        }
        #endregion

        #region public methods
        /// <summary>
        /// This method creates the sql command string to retrieve the person role map data.
        /// </summary>
        /// <param name="command">SQL command object.</param>
        /// <param name="personIndex">The person index to search on.</param>
        public virtual void GetPersonMapByIndexSql(SqlCommand command, int personIndex)
        {
            if (string.IsNullOrEmpty(this.SourceDbName))
            {
                return;
            }

            string select = " SELECT PersonIndex"
                            + " , Role";
            string from = " FROM " + this.SourceDbName + ".dbo.tblPersonRoleMap ";

            string where = " WHERE PersonIndex = " + personIndex;

            command.CommandText = select + from + where;
        }

        /// <summary>
        /// This method will load one row.
        /// </summary>
        /// <param name="row">The row to be loaded.</param>
        public virtual void Load(DataRow row)
        {
            this.PersonIndex    = row.IsNull("PersonIndex") ? -99 : (int)row["PersonIndex"];
            this.Role           = row.IsNull("Role") ? -99 : (int)row["Role"];
        }
        #endregion

        #region Properties      
        public int PersonIndex { get; set; }
        public int Role { get; set; }

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
            this.Role = -99;
        }
        #endregion
    }
}

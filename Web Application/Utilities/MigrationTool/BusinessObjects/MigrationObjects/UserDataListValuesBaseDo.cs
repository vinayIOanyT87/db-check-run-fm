namespace BusinessObjects.MigrationObjects
{
    using System.Data;
    using System.Data.SqlClient;

    public class UserDataListValuesBaseDo
    {
        #region Constructors
        /// <summary>
        /// This is the initializer constructor
        /// </summary>
        public UserDataListValuesBaseDo(string sourceDbName, string targetDbName)
        {
            this.SourceDbName = sourceDbName;
            this.TargetDbName = targetDbName;
            this.Init();
        }

        /// <summary>
        /// This is the default constructor
        /// </summary>
        public UserDataListValuesBaseDo()
        {
            this.Init();
        }
        #endregion

        #region Properties
        public int UserDataFieldIndex { get; set; }
        public string Value { get; set; }

        public string SourceDbName { get; set; }
        public string TargetDbName { get; set; }
        #endregion

        #region public methods
        /// <summary>
        /// This method creates the sql command string to retrieve the data.
        /// </summary>
        /// <param name="command">SQL command object.</param>
        public virtual void EnumerateUserDataListValuesSql(SqlCommand command, int userDataFieldIndex)
        {
            if (string.IsNullOrEmpty(this.SourceDbName))
            {
                return;
            }

            string select = " SELECT UL.*";
            string from = " FROM " + this.SourceDbName + ".dbo.tblUserDataListValues UL";
            string where = " WHERE UL.UserDataFieldIndex = " + userDataFieldIndex;

            command.CommandText = select + from + where;
        }

        /// <summary>
        /// This method will load one row.
        /// </summary>
        /// <param name="row">The row to be loaded.</param>
        public virtual void Load(DataRow row)
        {
            this.UserDataFieldIndex = row.IsNull("UserDataFieldIndex") ? -99 : (int)row["UserDataFieldIndex"];
            this.Value = row.IsNull("Value") ? string.Empty : (string)row["Value"];
        }
        #endregion

        #region Protected methods
        /// <summary>
        /// This method initializes the object to its initial state.
        /// </summary>
        protected void Init()
        {
            this.UserDataFieldIndex = -99;
            this.Value = string.Empty;
        }
        #endregion
    }
}

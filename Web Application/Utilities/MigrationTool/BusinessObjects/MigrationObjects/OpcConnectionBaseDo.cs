namespace BusinessObjects.MigrationObjects
{
    using System;
    using System.Data;
    using System.Data.SqlClient;

    public class OpcConnectionBaseDo
    {
        #region Constructors
        /// <summary>
        /// This is the initializer constructor
        /// </summary>
        public OpcConnectionBaseDo(string sourceDbName, string targetDbName)
        {
            this.SourceDbName = sourceDbName;
            this.TargetDbName = targetDbName;
            this.Init();
        }

        /// <summary>
        /// This is the default constructor
        /// </summary>
        public OpcConnectionBaseDo()
        {
            this.Init();
        }
        #endregion

        #region Properties
        public int Index { get; set; }
        public string Url { get; set; }
        public string ProgId { get; set; }
        public Guid OpcConnectionGuid { get; set; }

        public string SourceDbName { get; set; }
        public string TargetDbName { get; set; }
        #endregion

        #region public methods
        /// <summary>
        /// This method creates the sql command string to retrieve the data.
        /// </summary>
        /// <param name="command">SQL command object.</param>
        public virtual void EnumerateOpcConnectionSql(SqlCommand command)
        {
            if (string.IsNullOrEmpty(this.SourceDbName))
            {
                return;
            }

            string select = " SELECT O.*";
            string from = " FROM " + this.SourceDbName + ".dbo.tblOPCConnections O";

            command.CommandText = select + from;
        }
        public virtual void EnumerateTargetOpcConnectionSql(SqlCommand command)
        {
            if (string.IsNullOrEmpty(this.SourceDbName))
            {
                return;
            }

            string select = " SELECT O.*";
            string from = " FROM " + this.TargetDbName + ".dbo.tblOPCConnections O";

            command.CommandText = select + from;
        }

        /// <summary>
        /// This method will load one row.
        /// </summary>
        /// <param name="row">The row to be loaded.</param>
        public virtual void Load(DataRow row)
        {
            this.Url    = row.IsNull("URL") ? string.Empty : (string)row["URL"];
            this.ProgId = row.IsNull("ProgID") ? string.Empty : (string)row["ProgID"];

            if (row.Table.Columns.Contains("Index"))
            {
                this.Index = row.IsNull("Index") ? -99 : (int)row["Index"];
            }

            if (row.Table.Columns.Contains("OPCConnectionGuid"))
            {
                this.OpcConnectionGuid = row.IsNull("OPCConnectionGuid") ? Guid.Empty : (Guid)row["OPCConnectionGuid"];
            }
        }
        #endregion

        #region Protected methods
        /// <summary>
        /// This method initializes the object to its initial state.
        /// </summary>
        protected void Init()
        {
            this.Index              = -99;
            this.ProgId             = string.Empty;
            this.Url                = string.Empty;
            this.OpcConnectionGuid  = Guid.Empty;
        }
        #endregion
    }
}

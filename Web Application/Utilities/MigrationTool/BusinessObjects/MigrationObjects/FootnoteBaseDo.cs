namespace BusinessObjects.MigrationObjects
{
    using System.Data;
    using System.Data.SqlClient;

    public class FootnoteBaseDo : MigrationBaseDo
    {
        #region Constructors
        /// <summary>
        /// This is the initializer constructor
        /// </summary>
        public FootnoteBaseDo(string sourceDbName, string targetDbName)
        {
            this.SourceDbName = sourceDbName;
            this.TargetDbName = targetDbName;
            this.Init();
        }

        /// <summary>
        /// This is the default constructor
        /// </summary>
        public FootnoteBaseDo()
        {
            this.Init();
        }
        #endregion

        #region Properties
        public int Index { get; set; }
        public int AssignedIndex { get; set; }
        public int Type { get; set; }
        public int Sequence { get; set; }
        public string ApplicationStringId { get; set; }
        public int ApplicationStringType { get; set; }
        public string ProductId { get; set; }
        public string AdditiveProfileId { get; set; }
        public string CompanyShipperId { get; set; }
        public string CompanyShipToId { get; set; }
        public string CompanyShipToState { get; set; }

        public string SourceDbName { get; set; }
        public string TargetDbName { get; set; }
        #endregion

        #region public methods
        /// <summary>
        /// This method creates the sql command string to retrieve the data.
        /// </summary>
        /// <param name="command">SQL command object.</param>
        public virtual void EnumerateFootnotesSql(SqlCommand command)
        {
            if (string.IsNullOrEmpty(this.SourceDbName))
            {
                return;
            }

            string select = " SELECT M.*"
                            + " , A.ID AS ApplicationStringID"
                            + " , A.Type AS ApplicationStringType"
                            + " , (SELECT ProductID FROM " + this.SourceDbName + ".dbo.tblProducts WHERE ProductIndex = M.[Index]) AS ProductID"
                            + " , (SELECT ID FROM " + this.SourceDbName + ".dbo.tblAdditiveProfiles WHERE [Index] = M.[Index]) AS AdditiveProfileID"
                            + " , (SELECT ID FROM " + this.SourceDbName + ".dbo.tblCompanies C"
                            + " INNER JOIN " + this.SourceDbName + ".dbo.tblCompanyRoleMap CRM ON C.CompanyIndex = CRM.CompanyIndex WHERE CRM.Role = 2 AND C.CompanyIndex = M.[Index]) AS CompanyShipperID"
                            + " , (SELECT ID FROM " + this.SourceDbName + ".dbo.tblCompanies C"
                            + " INNER JOIN " + this.SourceDbName + ".dbo.tblCompanyRoleMap CRM ON C.CompanyIndex = CRM.CompanyIndex WHERE CRM.Role = 4 AND C.CompanyIndex = M.[Index]) AS CompanyShipToID"
                            + " , (SELECT[State] FROM " + this.SourceDbName + ".dbo.tblCompanies C"
                            + " INNER JOIN " + this.SourceDbName + ".dbo.tblCompanyRoleMap CRM ON C.CompanyIndex = CRM.CompanyIndex WHERE CRM.Role = 5 AND C.CompanyIndex = M.[Index]) AS CompanyShipToState";
            string from = " FROM " + this.SourceDbName + ".dbo.tblApplicationStringMap M"
                        + " INNER JOIN " + this.SourceDbName + ".dbo.tblApplicationString A ON A.[Index] = M.AssignedIndex";
            string where = " WHERE A.Type = 12";
            string orderBy = " ORDER BY A.ID";

            command.CommandText = select + from + where + orderBy;
        }

        /// <summary>
        /// This method will load one row.
        /// </summary>
        /// <param name="row">The row to be loaded.</param>
        public virtual void Load(DataRow row)
        {
            this.Index                  = row.IsNull("Index") ? -99 : (int)row["Index"];
            this.AssignedIndex          = row.IsNull("AssignedIndex") ? -99 : (int)row["AssignedIndex"];
            this.Type                   = row.IsNull("Type") ? -99 : (int)row["Type"];
            this.Sequence               = row.IsNull("Sequence") ? -99 : (int)row["Sequence"];
            this.ApplicationStringId    = row.IsNull("ApplicationStringID") ? string.Empty : (string)row["ApplicationStringID"];
            this.ApplicationStringType  = row.IsNull("ApplicationStringType") ? -99 : (int)row["ApplicationStringType"];
            this.ProductId              = row.IsNull("ProductID") ? string.Empty : (string)row["ProductID"];
            this.AdditiveProfileId      = row.IsNull("AdditiveProfileID") ? string.Empty : (string)row["AdditiveProfileID"];
            this.CompanyShipperId       = row.IsNull("CompanyShipperID") ? string.Empty : (string)row["CompanyShipperID"];
            this.CompanyShipToId        = row.IsNull("CompanyShipToID") ? string.Empty : (string)row["CompanyShipToID"];
            this.CompanyShipToState     = row.IsNull("CompanyShipToState") ? string.Empty : (string)row["CompanyShipToState"];
        }
        #endregion

        #region Protected methods
        /// <summary>
        /// This method initializes the object to its initial state.
        /// </summary>
        protected void Init()
        {
            this.Index                  = -99;
            this.AssignedIndex          = -99;
            this.Type                   = -99;
            this.Sequence               = -99;
            this.ApplicationStringId    = string.Empty;
            this.ApplicationStringType  = -99;
            this.ProductId              = string.Empty;
            this.AdditiveProfileId      = string.Empty;
            this.CompanyShipperId       = string.Empty;
            this.CompanyShipToId        = string.Empty;
            this.CompanyShipToState     = string.Empty;
        }
        #endregion
    }
}

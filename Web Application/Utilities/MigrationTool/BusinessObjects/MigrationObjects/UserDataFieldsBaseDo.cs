namespace BusinessObjects.MigrationObjects
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;

    public class UserDataFieldsBaseDo : MigrationBaseDo
    {
        #region Data members
        public const string EntityTypeCompanies             = "Companies";
        public const string EntityTypeProducts              = "Products";
        public const string EntityTypeSites                 = "Sites";
        public const string EntityTypePersonnel             = "Personnel";
        public const string EntityTypeEquipment             = "Equipment";
        public const string EntityTypeTransactionAliases    = "Transaction Aliases";
        #endregion

        #region Constructors
        /// <summary>
        /// This is the initializer constructor
        /// </summary>
        public UserDataFieldsBaseDo(string sourceDbName, string targetDbName)
        {
            this.SourceDbName = sourceDbName;
            this.TargetDbName = targetDbName;
            this.Init();
        }

        /// <summary>
        /// This is the default constructor
        /// </summary>
        public UserDataFieldsBaseDo()
        {
            this.Init();
        }
        #endregion

        #region Properties
        public int Index { get; set; }
        public int SiteIndex { get; set; }
        public string EntityTypeId { get; set; }
        public int AliasId { get; set; }
        public int Number { get; set; }
        public int DisplayOrder { get; set; }
        public string DisplayName { get; set; }
        public int Type { get; set; }
        public bool Required { get; set; }
        public int? UserGroupIndex { get; set; }
        public string UserGroupId { get; set; }
        public List<UserDataListValuesBaseDo> UserDataListValueList { get; set; }

        public string SourceDbName { get; set; }
        public string TargetDbName { get; set; }
        #endregion

        #region public methods
        /// <summary>
        /// This method creates the sql command string to retrieve the data.
        /// </summary>
        /// <param name="command">SQL command object.</param>
        public virtual void EnumerateUserDataFieldsSql(SqlCommand command, int siteIndex, string entityTypeId)
        {
            if (string.IsNullOrEmpty(this.SourceDbName))
            {
                return;
            }

            string select = " SELECT U.*, G.GroupID AS UserGroupID";
            string from = " FROM " + this.SourceDbName + ".dbo.tblUserDataFields U"
                        + " LEFT JOIN " + this.SourceDbName + ".dbo.tblGroups G ON G.GroupIndex = U.UserGroupIndex";
            string where = " WHERE U.SiteIndex = " + siteIndex + " AND EntityTypeID = '" + entityTypeId + "'";

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
            this.EntityTypeId   = row.IsNull("EntityTypeID") ? string.Empty : (string)row["EntityTypeID"];
            this.AliasId        = row.IsNull("AliasID") ? -99 : (int)row["AliasID"];
            this.Number         = row.IsNull("Number") ? -9 : (byte)row["Number"];
            this.DisplayOrder   = row.IsNull("DisplayOrder") ? -99 : (int)row["DisplayOrder"];
            this.DisplayName    = row.IsNull("DisplayName") ? string.Empty : (string)row["DisplayName"];
            this.Type           = row.IsNull("Type") ? -9 : (byte)row["Type"];
            this.Required       = row.IsNull("Required") ? false : (bool)row["Required"];
            this.UserGroupIndex = row.IsNull("UserGroupIndex") ? null : (int?)row["UserGroupIndex"];
            this.UserGroupId    = row.IsNull("UserGroupID") ? string.Empty : (string)row["UserGroupID"];
        }
        #endregion

        #region Protected methods
        /// <summary>
        /// This method initializes the object to its initial state.
        /// </summary>
        protected void Init()
        {
            this.Index                  = -99;
            this.SiteIndex              = -99;
            this.EntityTypeId           = string.Empty;
            this.AliasId                = -99;
            this.Number                 = -99;
            this.DisplayOrder           = -99;
            this.DisplayName            = string.Empty;
            this.Type                   = -99;
            this.Required               = false;
            this.UserGroupIndex         = null;
            this.UserGroupId            = string.Empty;
            this.UserDataListValueList  = new List<UserDataListValuesBaseDo>();
        }
        #endregion
    }
}

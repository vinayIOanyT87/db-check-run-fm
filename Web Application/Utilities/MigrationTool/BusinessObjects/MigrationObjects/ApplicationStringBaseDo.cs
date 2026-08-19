namespace BusinessObjects.MigrationObjects
{
    using FMBusinessObjects.DataObjects;
    using System;
    using System.Data;
    using System.Data.SqlClient;

    public class ApplicationStringBaseDo : MigrationBaseDo
    {
        #region Constructors
        /// <summary>
        /// This is the initializer constructor
        /// </summary>
        public ApplicationStringBaseDo(string sourceDbName, string targetDbName)
        {
            this.SourceDbName = sourceDbName;
            this.TargetDbName = targetDbName;
            this.Init();
        }

        /// <summary>
        /// This is the default constructor
        /// </summary>
        public ApplicationStringBaseDo()
        {
            this.Init();
        }
        #endregion

        #region Properties
        public int Index { get; set; }
        public int Type { get; set; }
        public int SiteIndex { get; set; }
        public string ID { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public Guid ApplicationStringGuid { get; set; }
        public int LookupApplicationStringTypeIndex { get; set; }
        public Guid SiteGuid { get; set; }

        public string SourceDbName { get; set; }
        public string TargetDbName { get; set; }
        #endregion

        #region public methods
        /// <summary>
        /// This method creates the sql command string to retrieve the data.
        /// </summary>
        /// <param name="command">SQL command object.</param>
        public virtual void EnumerateSourceApplicationStringSql(SqlCommand command, int siteIndex)
        {
            if (string.IsNullOrEmpty(this.SourceDbName))
            {
                return;
            }

            // Exclude type 12 "footnote" which are handled in the footnote migration.
            string select = " SELECT A.*";
            string from = " FROM " + this.SourceDbName + ".dbo.tblApplicationString A";
            string where = " WHERE A.SiteIndex = " + siteIndex + " AND A.Type <> 12";

            command.CommandText = select + from + where;
        }

        /// <summary>
        /// This method creates the sql command string to retrieve the data.
        /// </summary>
        /// <param name="command">SQL command object.</param>
        public virtual void EnumerateSourceApplicationStringFootnoteSql(SqlCommand command, int siteIndex, STRING_TYPE stringType )
        {
            if (string.IsNullOrEmpty(this.SourceDbName))
            {
                return;
            }

            string select = " SELECT A.*";
            string from = " FROM " + this.SourceDbName + ".dbo.tblApplicationString A";
            string where = " WHERE A.SiteIndex = " + siteIndex + " AND A.Type = " + (int)stringType;

            command.CommandText = select + from + where;
        }

        /// <summary>
        /// This method creates a sql command to retrieve the process variable application strings from the target
        /// DB that have been assigned to the child site.
        /// </summary>
        /// <param name="command">SQL command object.</param>
        /// <param name="targetSiteGuid">The target site guid.</param>
        public virtual void EnumerateTargetApplicationStringProcessVariableSql(SqlCommand command, Guid targetSiteGuid)
        {
            if (string.IsNullOrEmpty(this.TargetDbName))
            {
                return;
            }

            string select = " SELECT A.*";
            string from = " FROM " + this.TargetDbName + ".dbo.tblApplicationString A INNER JOIN "
                + this.TargetDbName + ".map.tblEntityProcessVariableMessageToSite M ON A.ApplicationStringGuid = M.ApplicationStringGuid";
            string where = " WHERE M.SiteGuid = '" + targetSiteGuid + "'";

            command.CommandText = select + from + where;
        }

        /// <summary>
        /// This method will load one row.
        /// </summary>
        /// <param name="row">The row to be loaded.</param>
        public virtual void Load(DataRow row)
        {
            this.ID         = row.IsNull("ID") ? string.Empty : (string)row["ID"];
            this.StartDate  = row.IsNull("StartDate") ? null : (DateTime?)row["StartDate"];
            this.EndDate    = row.IsNull("EndDate") ? null : (DateTime?)row["EndDate"];

            if (row.Table.Columns.Contains("Index"))
            {
                this.Index = row.IsNull("Index") ? -99 : (int)row["Index"];
            }

            if (row.Table.Columns.Contains("Type"))
            {
                this.Type = row.IsNull("Type") ? -99 : (int)row["Type"];
            }

            if (row.Table.Columns.Contains("SiteIndex"))
            {
                this.SiteIndex = row.IsNull("SiteIndex") ? -99 : (int)row["SiteIndex"];
            }

            if (row.Table.Columns.Contains("ApplicationStringGuid"))
            {
                this.ApplicationStringGuid = row.IsNull("ApplicationStringGuid") ? Guid.Empty : (Guid)row["ApplicationStringGuid"];
            }

            if (row.Table.Columns.Contains("LookupApplicationStringTypeIndex"))
            {
                this.LookupApplicationStringTypeIndex = 
                    row.IsNull("LookupApplicationStringTypeIndex") ? (int)STRING_TYPE.MAX_STRING_TYPE : (int)row["LookupApplicationStringTypeIndex"];
            }

            if (row.Table.Columns.Contains("SiteGuid"))
            {
                this.SiteGuid = row.IsNull("SiteGuid") ? Guid.Empty : (Guid)row["SiteGuid"];
            }
        }
        #endregion

        #region Protected methods
        /// <summary>
        /// This method initializes the object to its initial state.
        /// </summary>
        protected void Init()
        {
            this.Index                              = -99;
            this.Type                               = -99;
            this.SiteIndex                          = -99;
            this.ID                                 = string.Empty;
            this.StartDate                          = null;
            this.EndDate                            = null;
            this.SiteGuid                           = Guid.Empty;
            this.ApplicationStringGuid              = Guid.Empty;
            this.LookupApplicationStringTypeIndex   = (int)STRING_TYPE.MAX_STRING_TYPE;
    }
        #endregion
    }
}

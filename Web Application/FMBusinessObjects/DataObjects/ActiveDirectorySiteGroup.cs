namespace FMBusinessObjects.DataObjects
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Runtime.Serialization;

    [DataContract]
    [Serializable]
    public class ActiveDirectorySiteGroup : BaseDataObject
    {
        #region Data members
        [DataMember] private Guid activeDirectorySiteGroupGuid;
        [DataMember] private string name;
        [DataMember] private string ssid;
        #endregion

        #region Constructors
        /// <summary>
        /// This is the default constructor
        /// </summary>
        public ActiveDirectorySiteGroup()
        {
            this.Init();
        }
        #endregion

        #region Properties
        public Guid ActiveDirectorySiteGroupGuid
        {
            get { return this.activeDirectorySiteGroupGuid; }
            set { this.activeDirectorySiteGroupGuid = value; }
        }

        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        public string Ssid
        {
            get { return this.ssid; }
            set { this.ssid = value; }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// This method will set the SQL Command with the insert SQL.
        /// </summary>
        /// <param name="cmd">SQL Command object</param>
        public void InsertSQL(SqlCommand cmd)
        {
            cmd.CommandText = "INSERT INTO tblActiveDirectorySiteGroup " 
                                + "(ActiveDirectorySiteGroupGuid" 
                                + ", Name"
                                + ", Ssid" 
                                + ", CreatedDate" 
                                + ", CreatedBy" 
                                + ", UpdatedDate" 
                                + ", UpdatedBy"
                                + ") VALUES (" 
                                + " @ActiveDirectorySiteGroupGuid" 
                                + ", @Name" 
                                + ", @Ssid"
                                + ", @CreatedDate" 
                                + ", @CreatedBy" 
                                + ", @UpdatedDate" 
                                + ", @UpdatedBy" 
                                + ")";

            cmd.Parameters.Add("@ActiveDirectorySiteGroupGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 50);
            cmd.Parameters.Add("@Ssid", SqlDbType.NVarChar, 50);
            cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset);
            cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
            cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
            cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);

            cmd.Parameters["@ActiveDirectorySiteGroupGuid"].Value = activeDirectorySiteGroupGuid;
            cmd.Parameters["@Name"].Value = name;
            cmd.Parameters["@Ssid"].Value = ssid;
            cmd.Parameters["@CreatedDate"].Value = CreatedDate;
            cmd.Parameters["@CreatedBy"].Value = CreatedBy;
            cmd.Parameters["@UpdatedDate"].Value = UpdatedDate;
            cmd.Parameters["@UpdatedBy"].Value = UpdatedBy;
        }

        /// <summary>
        /// This method will set the SQL Command with the update SQL.
        /// </summary>
        /// <param name="cmd">SQL Command object</param>
        public void UpdateSQL(SqlCommand cmd)
        {
            cmd.CommandText = "UPDATE tblActiveDirectorySiteGroup SET" 
                              + " Name = @Name" 
                              + ", Ssid = @Ssid"
                              + ", UpdatedDate = @UpdatedDate" 
                              + ", UpdatedBy = @UpdatedBy"
                              + " WHERE ActiveDirectorySiteGroupGuid = @ActiveDirectorySiteGroupGuid";

            var parm = new SqlParameter("@ActiveDirectorySiteGroupGuid", SqlDbType.UniqueIdentifier) { Value = ActiveDirectorySiteGroupGuid };
            cmd.Parameters.Add(parm);

            parm = new SqlParameter("@Name", SqlDbType.NVarChar) { Value = name };
            cmd.Parameters.Add(parm);

            parm = new SqlParameter("@Ssid", SqlDbType.NVarChar) { Value = ssid };
            cmd.Parameters.Add(parm);

            parm = new SqlParameter("@UpdatedBy", SqlDbType.NVarChar) { Value = UpdatedBy };
            cmd.Parameters.Add(parm);

            parm = new SqlParameter("@UpdatedDate", SqlDbType.DateTimeOffset) { Value = UpdatedDate };
            cmd.Parameters.Add(parm);
        }

        /// <summary>
        /// This method will set the SQL Command with the delete SQL.
        /// </summary>
        /// <param name="cmd">SQL Command object</param>
        public void DeleteSQL(SqlCommand cmd)
        {
            cmd.CommandText = "UPDATE tblSites SET ActiveDirectorySiteGroupGuid = NULL"
                              + " WHERE ActiveDirectorySiteGroupGuid = @ActiveDirectorySiteGroupGuid"
                              + " DELETE FROM tblActiveDirectorySiteGroup"
                              + " WHERE ActiveDirectorySiteGroupGuid = @ActiveDirectorySiteGroupGuid";

            var parm = new SqlParameter("@ActiveDirectorySiteGroupGuid", SqlDbType.UniqueIdentifier) { Value = this.activeDirectorySiteGroupGuid };
            cmd.Parameters.Add(parm);
        }

        /// <summary>
        /// This method will populate the SQL Command with a query to retrieve all the 
        /// active directory site groups.  It will only return the ones that are not
        /// currently being used by other sites.
        /// </summary>
        /// <param name="cmd">SQL Command object</param>
        /// <param name="sitesActiveDirectoryGuid">The current site GUID</param>
        public void EnumerateSQL(SqlCommand cmd, Guid sitesActiveDirectoryGuid)
        {
            cmd.CommandText = "SELECT * FROM tblActiveDirectorySiteGroup"
                            + " WHERE ActiveDirectorySiteGroupGuid NOT IN (SELECT ActiveDirectorySiteGroupGuid"
                            + " FROM tblSites"
                            + " WHERE ActiveDirectorySiteGroupGuid IS NOT NULL AND ActiveDirectorySiteGroupGuid <> '00000000-0000-0000-0000-000000000000')"
                            + " OR ActiveDirectorySiteGroupGuid = @SitesActiveDirectoryGuid"
                            + " ORDER BY Name ";

            var parm = new SqlParameter("@SitesActiveDirectoryGuid", SqlDbType.UniqueIdentifier) { Value = sitesActiveDirectoryGuid };
            cmd.Parameters.Add(parm);
        }

        /// <summary>
        /// This method will populate the SQL Command with a query to retrieve all the 
        /// active directory site groups.
        /// </summary>
        /// <param name="cmd">SQL Command object</param>
        public void EnumerateAllSQL(SqlCommand cmd)
        {
            cmd.CommandText = "SELECT * FROM tblActiveDirectorySiteGroup ";
        }

        /// <summary>
        /// This method will populate the SQL Command with a query to retrieve the 
        /// site to active directory Site mappings.
        /// </summary>
        /// <param name="cmd">SQL Command object</param>
        public void EnumerateSiteToAdSiteMappingSQL(SqlCommand cmd)
        {
            cmd.CommandText = "SELECT ID AS SiteID, SiteGuid, Ads.Name AS ActiveDirectorySiteName "
                + "FROM tblSites s INNER JOIN tblActiveDirectorySiteGroup ads ON s.ActiveDirectorySiteGroupGuid = ads.ActiveDirectorySiteGroupGuid";
        }

        /// <summary>
        /// This method will load the object with the record from the database.
        /// </summary>
        /// <param name="row">The database row to use for the load process.</param>
        public void LoadRecord(DataRow row)
        {
            if (row.Equals(null)) return;

            this.activeDirectorySiteGroupGuid   = row.IsNull("ActiveDirectorySiteGroupGuid") ? Guid.Empty : (Guid)row["ActiveDirectorySiteGroupGuid"];
            this.name                           = row.IsNull("Name") ? string.Empty : (string)row["Name"];
            this.ssid                           = row.IsNull("Ssid") ? string.Empty : (string)row["Ssid"];
            this.CreatedBy                      = row.IsNull("CreatedBy") ? string.Empty : (string)row["CreatedBy"];
            this.UpdatedBy                      = row.IsNull("UpdatedBy") ? string.Empty : (string)row["UpdatedBy"];

            if (row.IsNull("CreatedDate") == false)
            {
                this.CreatedDate = (DateTimeOffset)row["CreatedDate"];
            }

            if (row.IsNull("UpdatedDate") == false)
            {
                this.CreatedDate = (DateTimeOffset)row["UpdatedDate"];
            }
        }
        #endregion

        #region Private methods
        /// <summary>
        /// This method will initialize the object to its initial state.
        /// </summary>
        private void Init()
        {
            this.activeDirectorySiteGroupGuid = Guid.Empty;
            this.name = string.Empty;
            this.ssid = string.Empty;
        }
        #endregion
    }
}

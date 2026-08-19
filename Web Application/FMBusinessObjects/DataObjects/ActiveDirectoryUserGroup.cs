namespace FMBusinessObjects.DataObjects
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Runtime.Serialization;

    [DataContract]
    [Serializable]
    public class ActiveDirectoryUserGroup : BaseDataObject
    {
        #region Data members
        [DataMember]
        private Guid activeDirectoryUserGroupGuid;
        [DataMember]
        private string name;
        [DataMember]
        private string ssid;
        #endregion

        #region Constructors
        /// <summary>
        /// This is the default constructor
        /// </summary>
        public ActiveDirectoryUserGroup()
        {
            this.Init();
        }
        #endregion

        #region Properties
        public Guid ActiveDirectoryUserGroupGuid
        {
            get { return this.activeDirectoryUserGroupGuid; }
            set { this.activeDirectoryUserGroupGuid = value; }
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
            cmd.CommandText = "INSERT INTO tblActiveDirectoryUserGroup "
                                + "(ActiveDirectoryUserGroupGuid"
                                + ", Name"
                                + ", Ssid"
                                + ", CreatedDate"
                                + ", CreatedBy"
                                + ", UpdatedDate"
                                + ", UpdatedBy"
                                + ") VALUES ("
                                + " @ActiveDirectoryUserGroupGuid"
                                + ", @Name"
                                + ", @Ssid"
                                + ", @CreatedDate"
                                + ", @CreatedBy"
                                + ", @UpdatedDate"
                                + ", @UpdatedBy"
                                + ")";

            cmd.Parameters.Add("@ActiveDirectoryUserGroupGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 50);
            cmd.Parameters.Add("@Ssid", SqlDbType.NVarChar, 50);
            cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset);
            cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
            cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
            cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);

            cmd.Parameters["@ActiveDirectoryUserGroupGuid"].Value = activeDirectoryUserGroupGuid;
            cmd.Parameters["@Name"].Value = this.name;
            cmd.Parameters["@Ssid"].Value = this.ssid;
            cmd.Parameters["@CreatedDate"].Value = this.CreatedDate;
            cmd.Parameters["@CreatedBy"].Value = this.CreatedBy;
            cmd.Parameters["@UpdatedDate"].Value = this.UpdatedDate;
            cmd.Parameters["@UpdatedBy"].Value = this.UpdatedBy;
        }

        /// <summary>
        /// This method will set the SQL Command with the update SQL.
        /// </summary>
        /// <param name="cmd">SQL Command object</param>
        public void UpdateSQL(SqlCommand cmd)
        {
            cmd.CommandText = "UPDATE tblActiveDirectoryUserGroup SET"
                              + " Name = @Name"
                              + ", Ssid = @Ssid"
                              + ", UpdatedDate = @UpdatedDate"
                              + ", UpdatedBy = @UpdatedBy"
                              + " WHERE ActiveDirectoryUserGroupGuid = @ActiveDirectoryUserGroupGuid";

            var parm = new SqlParameter("@ActiveDirectoryUserGroupGuid", SqlDbType.UniqueIdentifier) { Value = this.activeDirectoryUserGroupGuid };
            cmd.Parameters.Add(parm);

            parm = new SqlParameter("@Name", SqlDbType.NVarChar) { Value = this.name };
            cmd.Parameters.Add(parm);

            parm = new SqlParameter("@Ssid", SqlDbType.NVarChar) { Value = this.ssid };
            cmd.Parameters.Add(parm);

            parm = new SqlParameter("@UpdatedBy", SqlDbType.NVarChar) { Value = this.UpdatedBy };
            cmd.Parameters.Add(parm);

            parm = new SqlParameter("@UpdatedDate", SqlDbType.DateTimeOffset) { Value = this.UpdatedDate };
            cmd.Parameters.Add(parm);
        }

        /// <summary>
        /// This method will set the SQL Command with the delete SQL.
        /// </summary>
        /// <param name="cmd">SQL Command object</param>
        public void DeleteSQL(SqlCommand cmd)
        {
            cmd.CommandText = "UPDATE tblGroups SET ActiveDirectoryUserGroupGuid = NULL"
                              + " WHERE ActiveDirectoryUserGroupGuid = @ActiveDirectoryUserGroupGuid"
                              + " DELETE FROM tblActiveDirectoryUserGroup"
                              + " WHERE ActiveDirectoryUserGroupGuid = @ActiveDirectoryUserGroupGuid";

            var parm = new SqlParameter("@ActiveDirectoryUserGroupGuid", SqlDbType.UniqueIdentifier) { Value = this.activeDirectoryUserGroupGuid };
            cmd.Parameters.Add(parm);
        }

        /// <summary>
        /// This method will populate the SQL Command with a query to retrieve all the 
        /// active directory user groups.
        /// </summary>
        /// <param name="cmd">SQL Command object</param>
        /// <param name="usersActiveDirectoryGuid">The user group's active directory Guid.</param>
        public void EnumerateSQL(SqlCommand cmd, Guid usersActiveDirectoryGuid)
        {
            cmd.CommandText = "SELECT * FROM tblActiveDirectoryUserGroup"
                + " WHERE ActiveDirectoryUserGroupGuid NOT IN (SELECT ActiveDirectoryUserGroupGuid"
                + " FROM tblGroups"
                + " WHERE ActiveDirectoryUserGroupGuid IS NOT NULL AND ActiveDirectoryUserGroupGuid <> '00000000-0000-0000-0000-000000000000')"
                + " OR ActiveDirectoryUserGroupGuid = @UsersActiveDirectoryGuid"
                + " ORDER BY Name ";

            var parm = new SqlParameter("@UsersActiveDirectoryGuid", SqlDbType.UniqueIdentifier) { Value = usersActiveDirectoryGuid };
            cmd.Parameters.Add(parm);
        }

        /// <summary>
        /// This method will populate the SQL Command with a query to retrieve all the 
        /// active directory user groups.
        /// </summary>
        /// <param name="cmd">SQL Command object</param>
        public void EnumerateAllSQL(SqlCommand cmd)
        {
            cmd.CommandText = "SELECT * FROM tblActiveDirectoryUserGroup ";
        }

        /// <summary>
        /// This method will populate the SQL Command with a query to retrieve the 
        /// user group to active directory user group mappings.
        /// </summary>
        /// <param name="cmd">SQL Command object</param>
        public void EnumerateUserGroupToAdUserGroupMappingSQL(SqlCommand cmd)
        {
            cmd.CommandText = "SELECT GroupID, GroupGuid, Adu.Name AS ActiveDirectoryUserGroupName "
                + "FROM tblGroups g INNER JOIN tblActiveDirectoryUserGroup adu ON g.ActiveDirectoryUserGroupGuid = adu.ActiveDirectoryUserGroupGuid";
        }

        /// <summary>
        /// This method will load the object with the record from the database.
        /// </summary>
        /// <param name="row">The database row to use for the load process.</param>
        public void LoadRecord(DataRow row)
        {
            if (row.Equals(null)) return;

            this.activeDirectoryUserGroupGuid   = row.IsNull("ActiveDirectoryUserGroupGuid") ? Guid.Empty : (Guid)row["ActiveDirectoryUserGroupGuid"];
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
            this.activeDirectoryUserGroupGuid = Guid.Empty;
            this.name = string.Empty;
            this.ssid = string.Empty;
        }
        #endregion
    }
}

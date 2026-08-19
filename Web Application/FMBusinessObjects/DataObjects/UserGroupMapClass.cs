namespace FMBusinessObjects.DataObjects
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;

    [Serializable]
	[CollectionDataContract]
	[KnownType(typeof(UserDataClass))]
	public class UserGroupMapCollectionClass : List<UserGroupMapClass>
	{
	}


	[Serializable]
	[DataContract]
	public class UserGroupMapClass : BaseDataObject, IEquatable<UserGroupMapClass>
	{
		#region IEquatable
		public bool Equals(UserGroupMapClass other)
		{
			if (other == null)
			{
				return false;
			}

			return other.UserGuid == this.UserGuid && other.GroupGuid == this.GroupGuid && other.SiteGuid == this.SiteGuid;
		}
		#endregion

		public UserGroupMapClass()
		{
			this.Reset();
		}

		[DataMember]
		[XmlIgnore]
		public Guid GroupGuid { get; set; }

		[DataMember]
		[XmlIgnore]
		public Guid UserGuid { get; set; }

		[DataMember]
		public string GroupID { get; set; }

		[DataMember]
		public string UserID { get; set; }

		[DataMember]
		public DateTime ExpirationDate { get; set; }

        [DataMember]
        public bool DenyAdPermission { get; set; }

		public override void Reset()
		{
			base.Reset();
		    this.UserGuid       = Guid.Empty;
		    this.GroupGuid      = Guid.Empty;
		    this.UserID         = string.Empty;
		    this.GroupID        = string.Empty;
		    this.ExpirationDate = DateTime.Today.AddYears(1).Date;
		    this.DenyAdPermission = false;
		}

		public void Load(DataSet set)
		{
			if (set == null)
				throw new ArgumentNullException("set");

		    this.Reset();

			DataTable table = set.Tables[0];
			if (table.Rows.Count == 0)
				return;

			DataRow row = table.Rows[0];

		    this.SiteGuid           = DataObject.getValue<Guid>(row["SiteGuid"], Guid.Empty);
			this.UserGuid           = DataObject.getValue<Guid>(row["UserGuid"], Guid.Empty);
			this.GroupGuid          = DataObject.getValue<Guid>(row["GroupGuid"], Guid.Empty);
		    this.CreatedDate        = DataObject.getValue<DateTimeOffset>(row["CreatedDate"], DateTimeOffset.Now);
		    this.CreatedBy          = DataObject.getValue<string>(row["CreatedBy"], ADMIN);
			this.UserID             = DataObject.getValue<string>(row["UserID"], string.Empty);
			this.GroupID            = DataObject.getValue<string>(row["GroupID"], string.Empty);
			this.ExpirationDate     = DataObject.getValue<DateTime>(row["ExpirationDate"], DateTime.Today.AddYears(1).Date);
		    this.DenyAdPermission   = DataObject.getValue<bool>(row["DenyADPermission"], false);
		}

		public void InsertSQL(SqlCommand cmd)
		{
			cmd.CommandText = "INSERT INTO map.tblUserToGroup " +
				"(SiteGuid," +
				"UserGuid," +
				"GroupGuid," +
				"CreatedDate," +
				"CreatedBy," +
				"ExpirationDate," +
                "DenyADPermission" +
				") VALUES (" +
				"@SiteGuid," +
				"@UserGuid," +
				"@GroupGuid," +
				"@CreatedDate," +
				"@CreatedBy," +
				"@ExpirationDate," +
                "@DenyADPermission" +
				")";

			cmd.Parameters.AddWithValue("@SiteGuid", this.SiteGuid);
			cmd.Parameters.AddWithValue("@UserGuid", this.UserGuid);
			cmd.Parameters.AddWithValue("@GroupGuid", this.GroupGuid);
			cmd.Parameters.AddWithValue("@CreatedDate", this.CreatedDate);
			cmd.Parameters.AddWithValue("@CreatedBy", this.CreatedBy);
			cmd.Parameters.AddWithValue("@ExpirationDate", this.ExpirationDate.Date);
            cmd.Parameters.AddWithValue("@DenyADPermission", this.DenyAdPermission ? 1 : 0);
        }

		public void PurgeSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM map.tblUserToGroup WHERE SiteGuid = @SiteGuid AND UserGuid = @UserGuid AND GroupGuid = @GroupGuid";
			cmd.Parameters.AddWithValue("@SiteGuid", this.SiteGuid);
			cmd.Parameters.AddWithValue("@UserGuid", this.UserGuid);
			cmd.Parameters.AddWithValue("@GroupGuid", this.GroupGuid);
		}

		public void SelectSQL(SqlCommand cmd, bool bInTransaction)
		{
			cmd.CommandText = "SELECT UGM.*,tblUsers.UserID,tblGroups.GroupID FROM map.tblUserToGroup UGM " + SQLUpdateLock(bInTransaction) +
			" JOIN tblUsers ON tblUsers.UserGuid = UGM.UserGuid" +
			" JOIN tblGroups ON tblGroups.GroupGuid = UGM.GroupGuid" +
			 " WHERE UGM.SiteGuid = @SiteGuid AND UGM.UserGuid = @UserGuid AND UGM.GroupGuid = @GroupGuid";

			cmd.Parameters.AddWithValue("@SiteGuid", this.SiteGuid);
			cmd.Parameters.AddWithValue("@UserGuid", this.UserGuid);
			cmd.Parameters.AddWithValue("@GroupGuid", this.GroupGuid);
		}

        public void UpdateDenySQL(SqlCommand cmd)
        {
            cmd.CommandText = "UPDATE map.tblUserToGroup " +
                "SET DenyADPermission = @DenyADPermission, " + 
                "UpdatedDate = @UpdatedDate, " + 
                "UpdatedBy = @UpdatedBy " + 
                "WHERE UserGuid = @UserGuid AND " +
                "SiteGuid = @SiteGuid AND " +
                "GroupGuid = @GroupGuid";

            cmd.Parameters.AddWithValue("@SiteGuid", this.SiteGuid);
            cmd.Parameters.AddWithValue("@UserGuid", this.UserGuid);
            cmd.Parameters.AddWithValue("@GroupGuid", this.GroupGuid);
            cmd.Parameters.AddWithValue("@DenyADPermission", this.DenyAdPermission);
            cmd.Parameters.AddWithValue("@UpdatedBy", this._UpdatedBy);
            cmd.Parameters.AddWithValue("@UpdatedDate", this._UpdatedDate);

        }

        public void EnumerateByUserPermissionGridSQL(	SqlCommand cmd,
														SecurityClass security,
														Guid modifyUser,
														Guid siteGuid,
														bool loadChildrenSites,
														string filter)
		{
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "dbo.usp_GetUserGroupsAcrossSites";
			cmd.Parameters.Add(new SqlParameter("@LoggedInUser", security.UserGuid));
			cmd.Parameters.Add(new SqlParameter("@SiteGuid", siteGuid));
			cmd.Parameters.Add(new SqlParameter("@userToModify", modifyUser));
			cmd.Parameters.Add(new SqlParameter("@loadChildrenSites", loadChildrenSites));
			cmd.Parameters.Add(new SqlParameter("@filter", filter));
		}

		public void EnumerateBySiteSQL(SqlCommand cmd, SecurityClass security, Guid siteGuid, bool bInTransaction)
		{
			cmd.CommandText = "SELECT UGM.*,tblUsers.UserID,tblGroups.GroupID FROM map.tblUserToGroup UGM " + SQLUpdateLock(bInTransaction) +
				" JOIN tblUsers ON tblUsers.UserGuid = UGM.UserGuid" +
				" JOIN tblGroups ON tblGroups.GroupGuid = UGM.GroupGuid" +
				" WHERE UGM.SiteGuid = @SiteGuid";

			cmd.Parameters.AddWithValue("@SiteGuid", siteGuid);
		}

		public void EnumerateByGroupAndSiteSQL(SqlCommand cmd, SecurityClass security, Guid groupGuid, Guid siteGuid, bool inTransaction)
		{
			cmd.CommandText = "SELECT UGM.*,tblUsers.UserID,tblGroups.GroupID FROM map.tblUserToGroup UGM " + SQLUpdateLock(inTransaction) +
				" JOIN tblUsers ON tblUsers.UserGuid = UGM.UserGuid" +
				" JOIN tblGroups ON tblGroups.GroupGuid = UGM.GroupGuid" +
				" WHERE UGM.SiteGuid = @SiteGuid AND UGM.GroupGuid = @GroupGuid";

			cmd.Parameters.AddWithValue("@SiteGuid", siteGuid);
			cmd.Parameters.AddWithValue("@GroupGuid", groupGuid);
		}

		/// <summary>
		/// The enumerate by user and site SQL.
		/// </summary>
		/// <param name="cmd">
		/// The SQL command.
		/// </param>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="userGuid">
		/// The user GUID.
		/// </param>
		/// <param name="siteGuid">
		/// The site GUID.
		/// </param>
		/// <param name="inTransaction">
		/// The transaction.
		/// </param>
		public void EnumerateByUserAndSiteSQL(SqlCommand cmd, SecurityClass security, Guid userGuid, Guid siteGuid, bool inTransaction)
		{
			cmd.CommandText = "SELECT UGM.*,tblUsers.UserID,tblGroups.GroupID FROM map.tblUserToGroup UGM "
			                  + SQLUpdateLock(inTransaction)
			                  + " JOIN tblUsers ON tblUsers.UserGuid = UGM.UserGuid"
			                  + " JOIN tblGroups ON tblGroups.GroupGuid = UGM.GroupGuid"
			                  + " JOIN map.tblEntityUserGroupToSite EUGS ON EUGS.GroupGuid = UGM.GroupGuid"
			                  + " WHERE EUGS.SiteGuid = @SiteGuid AND UGM.UserGuid = @UserGuid AND UGM.SiteGuid = @SiteGuid";

			cmd.Parameters.AddWithValue("@SiteGuid", siteGuid);
			cmd.Parameters.AddWithValue("@UserGuid", userGuid);
		}

		public void EnumerateByGroupSQL(SqlCommand cmd, SecurityClass security, Guid groupGuid, bool inTransaction)
		{
			cmd.CommandText = "SELECT UGM.*,tblUsers.UserID,tblGroups.GroupID FROM map.tblUserToGroup UGM " + SQLUpdateLock(inTransaction) +
				" JOIN tblUsers ON tblUsers.UserGuid = UGM.UserGuid" +
				" JOIN tblGroups ON tblGroups.GroupGuid = UGM.GroupGuid" +
				" WHERE UGM.GroupGuid = @GroupGuid";

			cmd.Parameters.AddWithValue("@GroupGuid", groupGuid);
		}

        /// <summary>
        /// This method is used by the login process. If the Deny AD Permission column is set to 1 (true), then the
        /// Group is excluded from the data set.
        /// </summary>
        /// <param name="cmd">The SQL Command that is populated with the query.</param>
        /// <param name="security">The security object.</param>
        /// <param name="userGuid">The user Guid used to retrieve the user.</param>
        /// <param name="inTransaction">Place the query in a transaction (true).</param>
		public void EnumerateByUserSQL(SqlCommand cmd, SecurityClass security, Guid userGuid, bool inTransaction)
		{
			cmd.CommandText = "SELECT UGM.*,tblUsers.UserID,tblGroups.GroupID FROM map.tblUserToGroup UGM " + SQLUpdateLock(inTransaction) +
				" JOIN tblUsers ON tblUsers.UserGuid = UGM.UserGuid" +
				" JOIN tblGroups ON tblGroups.GroupGuid = UGM.GroupGuid" +
                " WHERE UGM.UserGuid = @UserGuid AND (UGM.DenyADPermission = 0 OR UGM.DenyADPermission IS NULL)";

			cmd.Parameters.AddWithValue("@UserGuid", userGuid);
		}
	}
}

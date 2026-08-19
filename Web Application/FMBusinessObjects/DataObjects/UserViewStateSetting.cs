namespace FMBusinessObjects.DataObjects
{
    using System;
    using System.Data.SqlClient;
    using System.Runtime.Serialization;

    using FMBusinessObjects.Attributes;
	using FMBusinessObjects.UtilityObjects;
    using System.Collections.Generic;

	#region User View State Setting Collection Class
	[Serializable]
    [CollectionDataContract]
    public class UserViewStateSettingCollection : List<UserViewStateSetting>
    {
        public UserViewStateSettingCollection Clone()
        {
            var collection = new UserViewStateSettingCollection();
            foreach (var u in this)
            {
                collection.Add((UserViewStateSetting)u.Clone());
            }
            return collection;
        }
    }
    #endregion

	[KnownType(typeof(TagViewerUserViewStateSettings))]
	[KnownType(typeof(DrawUserViewStateSettings))]
	[KnownType(typeof(AlarmHistoryUserViewStateSettings))]
	[KnownType(typeof(MovementHistoryUserViewStateSettings))]
	[DataContract]
	[Serializable]
   public class UserViewStateSetting : BaseSerializedDataObject, ICloneable
   {
	  private string clientIpAddress = ClientIpAddressUtility.NormalizeToIPv4(string.Empty);

      #region Construction
      public UserViewStateSetting(SecurityClass security)
      {
         if (null == security) return;
         base.SiteGuid = security.SiteGuid;
         this.UserGuid = security.UserGuid;
			this.ClientIpAddress = security.ClientIpAddress;
			this.WindowName = "";
      }

      public UserViewStateSetting()
      {
      }
      #endregion

      #region Properties
      [FMPersistedField]
      [DataMember]
      public Guid UserViewStateSettingGuid
      {
         get
         {
               return this.IdentityGuid;
         }
         set
         {
               this.IdentityGuid = value;
         }
      }

      [FMPersistedField]
      [DataMember]
      public Guid UserGuid { get; set; }


		[FMPersistedField]
		[DataMember]
		public string ClientIpAddress
		{
			get
			{
				return this.clientIpAddress;
			}
			set
			{
				this.clientIpAddress = ClientIpAddressUtility.NormalizeToIPv4(value);
			}
		}

		[FMPersistedField]
		[DataMember]
		public string WindowName { get; set; }



		public string ViewID
      {
         get
         {
               return base.ID;
         } 
         set
         {
               base.ID = value;
         } 
      }
      #endregion

      #region Utility Methods
      public object Clone()
      {
         var u = (UserViewStateSetting)this.MemberwiseClone();
         this.BaseClone(u);
         return u;
      }

      public void GetInsertSQL(SqlCommand cmd)
      {
         cmd.CommandText = "INSERT INTO dbo.tblUserViewStateSettings ("
                           + "ID, ValueType, Value, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, UserViewStateSettingGuid, SiteGuid, UserGuid, ClientIpAddress, WindowName)"
                           + "VALUES ("
                           + "@id, @valueType, @value, @createdDate, @createdBy, @updatedDate, @updatedBy, @userViewStateSettingGuid, @siteGuid, @userGuid, @ClientIpAddress, @WindowName)";
         cmd.Parameters.AddWithValue("@id",this.ID);
         cmd.Parameters.AddWithValue("@valueType", this.ValueTypeString);
         cmd.Parameters.AddWithValue("@value", this.ValueXml);
         cmd.Parameters.AddWithValue("@createdDate", this.CreatedDate);
         cmd.Parameters.AddWithValue("@createdBy", this.CreatedBy);
         cmd.Parameters.AddWithValue("@updatedDate", this.UpdatedDate);
         cmd.Parameters.AddWithValue("@updatedBy", this.UpdatedBy);
         cmd.Parameters.AddWithValue("@userViewStateSettingGuid", this._IdentityGuid);
         cmd.Parameters.AddWithValue("@siteGuid", this.SiteGuid);
         cmd.Parameters.AddWithValue("@userGuid", this.UserGuid);
			cmd.Parameters.AddWithValue("@clientIpAddress", this.ClientIpAddress);
			cmd.Parameters.AddWithValue("@windowName", this.WindowName);
		}

		public void GetUpdateSQL(SqlCommand cmd)
      {
         cmd.CommandText = "UPDATE dbo.tblUserViewStateSettings SET " + "ID = @id, " + "ValueType = @valueType, "
                           + "Value = @value, " 
                           + "UpdatedDate = @updatedDate, " + "UpdatedBy = @updatedBy, " + "SiteGuid = @siteGuid, "
                           + "UserGuid = @userGuid, " + "ClientIpAddress = @clientIpAddress, " + "WindowName = @windowName"
									+ " WHERE UserViewStateSettingGuid = @userViewStateSettingGuid ";
         cmd.Parameters.AddWithValue("@id", this.ID);
         cmd.Parameters.AddWithValue("@valueType", this.ValueTypeString);
         cmd.Parameters.AddWithValue("@value", this.ValueXml);
         cmd.Parameters.AddWithValue("@updatedDate", this.UpdatedDate);
         cmd.Parameters.AddWithValue("@updatedBy", this.UpdatedBy);
         cmd.Parameters.AddWithValue("@siteGuid", this.SiteGuid);
         cmd.Parameters.AddWithValue("@userGuid", this.UserGuid);
			cmd.Parameters.AddWithValue("@clientIpAddress", this.ClientIpAddress);
			cmd.Parameters.AddWithValue("@windowName", this.WindowName);
			cmd.Parameters.AddWithValue("@userViewStateSettingGuid", this.UserViewStateSettingGuid);
      }

      public static void GetPurgeSQL(SqlCommand cmd, Guid userViewStateSettingGuid)
      {
         cmd.CommandText = "DELETE FROM dbo.tblUserViewStateSettings  WHERE UserViewStateSettingGuid = @userViewStateSettingGuid ";
         cmd.Parameters.AddWithValue("@userViewStateSettingGuid", userViewStateSettingGuid);

      }

		public static void GetPurgeBySiteSQL(SqlCommand cmd, Guid siteGuid)
		{
			cmd.CommandText = "DELETE FROM dbo.tblUserViewStateSettings  WHERE SiteGuid = @SiteGuid";
			cmd.Parameters.AddWithValue("@SiteGuid", siteGuid);

		}


		public static void GetPurgeByUserSQL(SqlCommand cmd, Guid userGuid)
		{
			cmd.CommandText = "DELETE FROM dbo.tblUserViewStateSettings  WHERE UserGuid = @UserGuid";
			cmd.Parameters.AddWithValue("@UserGuid", userGuid);

		}

		public static void GetPurgeBySiteAndUserAndWindowNameAndViewIDSQL(SqlCommand cmd, Guid siteGuid, Guid userGuid, string clientIpAddress, string windowName, string viewID)
		{
			cmd.CommandText = @"DELETE FROM dbo.tblUserViewStateSettings
								WHERE SiteGuid = @SiteGuid
									AND UserGuid = @UserGuid
									AND ClientIpAddress = @ClientIpAddress
									AND WindowName = @WindowName
									AND ID = @ViewID";
			cmd.Parameters.AddWithValue("@SiteGuid", siteGuid);
			cmd.Parameters.AddWithValue("@UserGuid", userGuid);
			cmd.Parameters.AddWithValue("@ClientIpAddress", ClientIpAddressUtility.NormalizeToIPv4(clientIpAddress));
			cmd.Parameters.AddWithValue("@WindowName", windowName);
			cmd.Parameters.AddWithValue("@ViewID", viewID);
		}


		public static void GetSQL(SqlCommand cmd, Guid userViewStateSettingGuid)
		{
			cmd.CommandText = "SELECT * FROM dbo.tblUserViewStateSettings WHERE UserViewStateSettingGuid = @UserViewStateSettingGuid";
			cmd.Parameters.AddWithValue("@UserViewStateSettingGuid", userViewStateSettingGuid);
		}

      public static void EnumerateBySiteSQL(SqlCommand cmd, Guid siteGuid)
      {
         cmd.CommandText = "SELECT * FROM dbo.tblUserViewStateSettings WHERE SiteGuid = @SiteGuid";
         cmd.Parameters.AddWithValue("@SiteGuid", siteGuid);
      }

      public static void EnumerateByUserSQL(SqlCommand cmd, Guid userGuid)
      {
         cmd.CommandText = "SELECT * FROM dbo.tblUserViewStateSettings WHERE UserGuid = @UserGuid";
         cmd.Parameters.AddWithValue("@UserGuid", userGuid);
      }

      public static void EnumerateBySiteAndUserSQL(SqlCommand cmd, Guid siteGuid, Guid userGuid)
      {
         cmd.CommandText = "SELECT * FROM dbo.tblUserViewStateSettings WHERE SiteGuid = @SiteGuid AND UserGuid = @UserGuid";
         cmd.Parameters.AddWithValue("@SiteGuid", siteGuid);
         cmd.Parameters.AddWithValue("@UserGuid", userGuid);
      }

      public static void EnumerateBySiteAndUserAndWindowNameAndViewIDSQL(SqlCommand cmd, Guid siteGuid, Guid userGuid, string clientIpAddress, string windowName, string viewID)
      {
         cmd.CommandText = "SELECT * FROM dbo.tblUserViewStateSettings WHERE SiteGuid = @SiteGuid AND UserGuid = @UserGuid AND ClientIpAddress = @ClientIpAddress AND WindowName = @WindowName AND ID = @ViewID";
         cmd.Parameters.AddWithValue("@SiteGuid", siteGuid);
         cmd.Parameters.AddWithValue("@UserGuid", userGuid);
			cmd.Parameters.AddWithValue("@ClientIpAddress", ClientIpAddressUtility.NormalizeToIPv4(clientIpAddress));
			cmd.Parameters.AddWithValue("@WindowName", windowName);
			cmd.Parameters.AddWithValue("@ViewID", viewID);
      }
      #endregion
   }
}

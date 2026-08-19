namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Data.SqlClient;
	using System.Runtime.Serialization;

	using FMBusinessObjects.Attributes;
	using System.Collections.Generic;
	using System.Data;
	using System.Xml.Serialization;
	using System.IO;


	#region Draw User View State Setting Collection Class
	[Serializable]
	[CollectionDataContract]
	public class DrawUserViewStateSettingCollection : List<DrawUserViewStateSetting>
	{
		public DrawUserViewStateSettingCollection Clone()
		{
			var collection = new DrawUserViewStateSettingCollection();
			foreach (var u in this)
			{
				collection.Add((DrawUserViewStateSetting)u.Clone());
			}
			return collection;
		}
	}
	#endregion
	[KnownType(typeof(DrawUserViewStateSettings))]
	[DataContract]
	[Serializable]
	public class DrawUserViewStateSetting : BaseSerializedDataObject, ICloneable
	{
		#region Construction
		public DrawUserViewStateSetting(SecurityClass security)
		{
			if (null == security) return;
			base.SiteGuid = security.SiteGuid;
			this.UserGuid = security.UserGuid;
		}

		public DrawUserViewStateSetting()
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
			var u = (DrawUserViewStateSetting)this.MemberwiseClone();
			this.BaseClone(u);
			return u;
		}

		public void GetInsertSQL(SqlCommand cmd)
		{
			cmd.CommandText = "INSERT INTO dbo.tblUserViewStateSettings ("
								+ "ID, ValueType, Value, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, UserViewStateSettingGuid, SiteGuid, UserGuid)"
								+ "VALUES ("
								+ "@id, @valueType, @value, @createdDate, @createdBy, @updatedDate, @updatedBy, @userViewStateSettingGuid, @siteGuid, @userGuid)";
			cmd.Parameters.AddWithValue("@id", this.ID);
			cmd.Parameters.AddWithValue("@valueType", this.ValueTypeString);
			cmd.Parameters.AddWithValue("@value", this.ValueXml);
			cmd.Parameters.AddWithValue("@createdDate", this.CreatedDate);
			cmd.Parameters.AddWithValue("@createdBy", this.CreatedBy);
			cmd.Parameters.AddWithValue("@updatedDate", this.UpdatedDate);
			cmd.Parameters.AddWithValue("@updatedBy", this.UpdatedBy);
			cmd.Parameters.AddWithValue("@userViewStateSettingGuid", this._IdentityGuid);
			cmd.Parameters.AddWithValue("@siteGuid", this.SiteGuid);
			cmd.Parameters.AddWithValue("@userGuid", this.UserGuid);


		}

		public void GetUpdateSQL(SqlCommand cmd)
		{
			cmd.CommandText = "UPDATE dbo.tblUserViewStateSettings SET " + "ID = @id, " + "ValueType = @valueType, "
								+ "Value = @value, "
								+ "UpdatedDate = @updatedDate, " + "UpdatedBy = @updatedBy, " + "SiteGuid = @siteGuid, "
								+ "UserGuid = @userGuid " + " WHERE UserViewStateSettingGuid = @userViewStateSettingGuid ";
			cmd.Parameters.AddWithValue("@id", this.ID);
			cmd.Parameters.AddWithValue("@valueType", this.ValueTypeString);
			cmd.Parameters.AddWithValue("@value", this.ValueXml);
			cmd.Parameters.AddWithValue("@updatedDate", this.UpdatedDate);
			cmd.Parameters.AddWithValue("@updatedBy", this.UpdatedBy);
			cmd.Parameters.AddWithValue("@siteGuid", this.SiteGuid);
			cmd.Parameters.AddWithValue("@userGuid", this.UserGuid);
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

		public static void EnumerateBySiteAndUserAndViewIDSQL(SqlCommand cmd, Guid siteGuid, Guid userGuid, string viewID)
		{
			cmd.CommandText = "SELECT * FROM dbo.tblUserViewStateSettings WHERE SiteGuid = @SiteGuid AND UserGuid = @UserGuid AND ID = @ViewID";
			cmd.Parameters.AddWithValue("@SiteGuid", siteGuid);
			cmd.Parameters.AddWithValue("@UserGuid", userGuid);
			cmd.Parameters.AddWithValue("@ViewID", viewID);
		}
		#endregion
	}
}

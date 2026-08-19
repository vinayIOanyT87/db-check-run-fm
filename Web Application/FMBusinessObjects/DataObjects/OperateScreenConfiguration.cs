namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Data.SqlClient;
	using System.Runtime.Serialization;

	using FMBusinessObjects.Attributes;
	using FMBusinessObjects.UtilityObjects;

	[DataContract]
	[Serializable]
	public class OperateScreenConfiguration : BaseDataObject, ICloneable
	{
		private string clientIpAddress = ClientIpAddressUtility.NormalizeToIPv4(string.Empty);

		public OperateScreenConfiguration(SecurityClass security)
		{
			if (security == null)
			{
				return;
			}

			this.SiteGuid = security.SiteGuid;
			this.UserGuid = security.UserGuid;
			this.ClientIpAddress = security.ClientIpAddress;
		}

		public OperateScreenConfiguration()
		{
		}

		[FMPersistedField]
		[DataMember]
		public Guid OperateScreenConfigurationGuid
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
		public long ScreenMask { get; set; }

		public object Clone()
		{
			var configuration = (OperateScreenConfiguration)this.MemberwiseClone();
			this.BaseClone(configuration);
			return configuration;
		}

		public static void GetBySiteUserClientIpAddressSQL(SqlCommand cmd, Guid siteGuid, Guid userGuid, string clientIpAddress)
		{
			cmd.CommandText = @"SELECT TOP 1 *
								FROM dbo.tblOperateScreenConfiguration
								WHERE SiteGuid = @SiteGuid
									AND UserGuid = @UserGuid
									AND ClientIpAddress = @ClientIpAddress";
			cmd.Parameters.AddWithValue("@SiteGuid", siteGuid);
			cmd.Parameters.AddWithValue("@UserGuid", userGuid);
			cmd.Parameters.AddWithValue("@ClientIpAddress", ClientIpAddressUtility.NormalizeToIPv4(clientIpAddress));
		}

		public void GetUpsertSQL(SqlCommand cmd)
		{
			cmd.CommandText = @"IF EXISTS (
									SELECT 1
									FROM dbo.tblOperateScreenConfiguration
									WHERE SiteGuid = @SiteGuid
										AND UserGuid = @UserGuid
										AND ClientIpAddress = @ClientIpAddress)
								BEGIN
									UPDATE dbo.tblOperateScreenConfiguration
									SET ScreenMask = @ScreenMask,
										UpdatedDate = @UpdatedDate,
										UpdatedBy = @UpdatedBy
									WHERE SiteGuid = @SiteGuid
										AND UserGuid = @UserGuid
										AND ClientIpAddress = @ClientIpAddress
								END
								ELSE
								BEGIN
									INSERT INTO dbo.tblOperateScreenConfiguration
										(OperateScreenConfigurationGuid, SiteGuid, UserGuid, ClientIpAddress, ScreenMask, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
									VALUES
										(@OperateScreenConfigurationGuid, @SiteGuid, @UserGuid, @ClientIpAddress, @ScreenMask, @CreatedDate, @CreatedBy, @UpdatedDate, @UpdatedBy)
								END";
			cmd.Parameters.AddWithValue("@OperateScreenConfigurationGuid", this.OperateScreenConfigurationGuid);
			cmd.Parameters.AddWithValue("@SiteGuid", this.SiteGuid);
			cmd.Parameters.AddWithValue("@UserGuid", this.UserGuid);
			cmd.Parameters.AddWithValue("@ClientIpAddress", this.ClientIpAddress);
			cmd.Parameters.AddWithValue("@ScreenMask", this.ScreenMask);
			cmd.Parameters.AddWithValue("@CreatedDate", this.CreatedDate);
			cmd.Parameters.AddWithValue("@CreatedBy", this.CreatedBy);
			cmd.Parameters.AddWithValue("@UpdatedDate", this.UpdatedDate);
			cmd.Parameters.AddWithValue("@UpdatedBy", this.UpdatedBy);
		}
	}
}

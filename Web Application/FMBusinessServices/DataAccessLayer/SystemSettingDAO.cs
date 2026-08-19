namespace FMBusinessServices.DataAccessLayer
{
	using System;
	using System.Data;
	using System.Data.SqlClient;

	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;

	internal static class SystemSettingDAO
	{
		internal static void LoadObject( this SystemSettingClass setting, DataSet set )
		{
			if ( set == null )
			{
				throw new ArgumentNullException( "set" );
			}

			setting.Reset();
			DataTable table = set.Tables[0];

			if ( table.Rows.Count == 0 )
			{
				return;
			}

			DataRow row = table.Rows[0];

			setting.IdentityGuid					= DataObject.getValue( row["SystemSettingGuid"], Guids.SystemSettingsGuid );
			setting.ReportServerUrl					= DataObject.getValue( row["ReportServerURL"], "http://localhost/ReportServer" );
			setting.StationMessageTimeout			= DataObject.getValue( row["StationMessageTimeout"], 2 );
			setting.StationPromptTimeout			= DataObject.getValue( row["StationPromptTimeout"], 60 );
			setting.CreatedDate						= DataObject.getValue( row["CreatedDate"], DateTimeOffset.Now );
			setting.CreatedBy						= DataObject.getValue( row["CreatedBy"], BaseDataObject.ADMIN );
			setting.UpdatedDate						= DataObject.getValue( row["UpdatedDate"], setting.CreatedDate );
			setting.UpdatedBy						= DataObject.getValue( row["UpdatedBy"], BaseDataObject.ADMIN );
			setting.ReportServerUserName			= DataObject.getValue( row["ReportServerUserName"], "" );
			setting.ReportServerPassword			= DataObject.getValue( row["ReportServerPassword"] == DBNull.Value ? string.Empty : UserClass.decode( (byte[]) row["ReportServerPassword"], Guids.SiteAdminGuid ), string.Empty );
			setting.ProhibitUpdatingLinkedEquipment = DataObject.getValue(row["ProhibitUpdatingLinkedEquipment"], false);
			setting.UserDataListDefaultToFirstValue = DataObject.getValue(row["UserDataListDefaultToFirstValue"], false);
        }

		internal static void SelectSQL( this SystemSettingClass setting, SqlCommand cmd, bool bInTransaction )
		{
			cmd.CommandText = "SELECT * FROM tblSystemSettings " + BaseDAO.SQLUpdateLock( bInTransaction ) +
									" WHERE SystemSettingGuid = @SystemSettingGuid";

			cmd.Parameters.AddWithValue( "@SystemSettingGuid", Guids.SystemSettingsGuid );
		}

		internal static void UpdateSQL( this SystemSettingClass setting, SqlCommand cmd )
		{
			cmd.CommandText = "UPDATE tblSystemSettings " 
							  + "SET ReportServerURL = @ReportServerURL, " 
							  + "StationMessageTimeout = @StationMessageTimeout, " 
							  + "StationPromptTimeout = @StationPromptTimeout, "
							  + "ProhibitUpdatingLinkedEquipment = @ProhibitUpdatingLinkedEquipment, "
							  + "UserDataListDefaultToFirstValue = @UserDataListDefaultToFirstValue, "
							  + "UpdatedDate = @UpdatedDate, " 
							  + "UpdatedBy = @UpdatedBy, " 
							  + "ReportServerUserName = @ReportServerUserName, " 
							  + "ReportServerPassword = @ReportServerPassword "
                              + " WHERE SystemSettingGuid = @SystemSettingGuid";

			cmd.Parameters.AddWithValue( "@SystemSettingGuid", Guids.SystemSettingsGuid );
			cmd.Parameters.AddWithValue( "@ReportServerURL", setting.ReportServerUrl );
			cmd.Parameters.AddWithValue( "@StationMessageTimeout", setting.StationMessageTimeout );
			cmd.Parameters.AddWithValue( "@StationPromptTimeout", setting.StationPromptTimeout );
			cmd.Parameters.AddWithValue( "@ProhibitUpdatingLinkedEquipment", setting.ProhibitUpdatingLinkedEquipment );
			cmd.Parameters.AddWithValue( "@UserDataListDefaultToFirstValue", setting.UserDataListDefaultToFirstValue );
			cmd.Parameters.AddWithValue( "@UpdatedDate", setting.UpdatedDate );
			cmd.Parameters.AddWithValue( "@UpdatedBy", setting.UpdatedBy );
			cmd.Parameters.AddWithValue( "@ReportServerUserName", ( null != setting.ReportServerUserName ) ? (object) setting.ReportServerUserName : DBNull.Value );

            cmd.Parameters.Add( "@ReportServerPassword", SqlDbType.VarBinary, 256 ).Value =
				!string.IsNullOrEmpty( setting.ReportServerPassword ) ? (object) UserClass.encode( setting.ReportServerPassword, Guids.SiteAdminGuid ) : DBNull.Value;
		}

		internal static void InsertSQL( this SystemSettingClass setting, SqlCommand cmd )
		{
			cmd.CommandText = "INSERT INTO tblSystemSettings " 
							  + "(SystemSettingGuid," 
							  + "ReportServerURL," 
							  + "StationMessageTimeout," 
							  + "StationPromptTimeout,"
							  + "ProhibitUpdatingLinkedEquipment, "
							  + "UserDataListDefaultToFirstValue, "
							  + "CreatedDate," 
							  + "CreatedBy," 
							  + "UpdatedDate," 
							  + "UpdatedBy, " 
                              + "ReportServerUserName, " 
							  + "ReportServerPassword ) " 
							  + "VALUES (" 
							  + "@SystemSettingGuid," 
							  + "@ReportServerURL," 
							  + "@StationMessageTimeout," 
							  + "@StationPromptTimeout,"
							  + "@ProhibitUpdatingLinkedEquipment, "
							  + "@UserDataListDefaultToFirstValue, "
							  + "@CreatedDate," 
							  + "@CreatedBy," 
							  + "@UpdatedDate," 
							  + "@UpdatedBy, "
                              + "@ReportServerUserName, " 
							  + "@ReportServerPassword )";

			cmd.Parameters.AddWithValue( "@SystemSettingGuid", Guids.SystemSettingsGuid );
			cmd.Parameters.AddWithValue( "@ReportServerURL", setting.ReportServerUrl );
			cmd.Parameters.AddWithValue( "@StationMessageTimeout", setting.StationMessageTimeout );
			cmd.Parameters.AddWithValue( "@StationPromptTimeout", setting.StationPromptTimeout );
			cmd.Parameters.AddWithValue( "@ProhibitUpdatingLinkedEquipment", setting.ProhibitUpdatingLinkedEquipment );
			cmd.Parameters.AddWithValue( "@UserDataListDefaultToFirstValue", setting.UserDataListDefaultToFirstValue );
			cmd.Parameters.AddWithValue( "@CreatedDate", setting.CreatedDate);
			cmd.Parameters.AddWithValue( "@CreatedBy", setting.CreatedBy );
			cmd.Parameters.AddWithValue( "@UpdatedDate", setting.UpdatedDate );
			cmd.Parameters.AddWithValue( "@UpdatedBy", setting.UpdatedBy );
            cmd.Parameters.AddWithValue( "@ReportServerUserName", ( null != setting.ReportServerUserName ) ? (object) setting.ReportServerUserName : DBNull.Value );

			cmd.Parameters.Add( "@ReportServerPassword", SqlDbType.VarBinary, 256 ).Value =
				!string.IsNullOrEmpty( setting.ReportServerPassword ) ? (object) UserClass.encode( setting.ReportServerPassword, Guids.SiteAdminGuid ) : DBNull.Value;
		}
	}
}
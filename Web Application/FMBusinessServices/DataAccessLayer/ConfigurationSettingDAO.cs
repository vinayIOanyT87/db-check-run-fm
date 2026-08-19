namespace FMBusinessServices.DataAccessLayer
{
	using System;
	using System.Data;
	using System.Data.SqlClient;

	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;

    internal static class ConfigurationSettingDAO
	{
        #region Public methods
        public static void GetKeyValueByKeySQL(this ConfigurationSettingDOClass setting, SqlCommand cmd, string inKey)
        {
            if (string.IsNullOrEmpty(inKey) == true)
            {
                return;
            }

            cmd.CommandText = "  SELECT ConfigurationSettingGuid, "
                + "			KeyType, "
                + "			SettingKey, "
                + "			SettingValue, "
                + "			CreatedDate, "
                + "			CreatedBy, "
                + "			UpdatedDate, "
                + "			UpdatedBy, "
                + "			UpdatedDate "
                + "	FROM tblConfigurationSetting "
                + "	WHERE SettingKey = @Key ";

            SqlParameter indexParm = cmd.Parameters.Add("@Key", SqlDbType.NVarChar, 50);
            indexParm.Value = inKey;
        }

        public static void GetKeyValueByGuidSQL(this ConfigurationSettingDOClass setting, SqlCommand cmd, string uniqueIdentify)
        {
            if (string.IsNullOrEmpty(uniqueIdentify) == true)
            {
                return;
            }

            cmd.CommandText = "  SELECT ConfigurationSettingGuid, "
                + "			KeyType, "
                + "			SettingKey, "
                + "			SettingValue, "
                + "			CreatedDate, "
                + "			CreatedBy, "
                + "			UpdatedDate, "
                + "			UpdatedBy, "
                + "			UpdatedDate "
                + "	FROM tblConfigurationSetting "
                + "	WHERE ConfigurationSettingGuid = @Guid ";

            SqlParameter indexParm = cmd.Parameters.Add("@Guid", SqlDbType.NVarChar, 50);
            indexParm.Value = uniqueIdentify;
        }

        public static void EnumerateSQL(this ConfigurationSettingDOClass setting, SqlCommand cmd)
        {
            cmd.CommandText = "  SELECT ConfigurationSettingGuid, "
                + "			KeyType, "
                + "			SettingKey, "
                + "			SettingValue, "
                + "			CreatedDate, "
                + "			CreatedBy, "
                + "			UpdatedDate, "
                + "			UpdatedBy, "
                + "			UpdatedDate "
                + "	FROM tblConfigurationSetting ORDER BY SettingKey";
        }

        public static void Load(this ConfigurationSettingDOClass setting, DataTable dataTable)
        {
            if (dataTable != null)
            {
                if ((dataTable.Rows != null) && (dataTable.Rows.Count > 0))
                {
                    DataRow row = dataTable.Rows[0];
                    setting.Load(row);
                }
            }
        }

        public static void Load(this ConfigurationSettingDOClass setting, DataRow row)
        {
            if (row != null)
            {
                setting.Reset();
                setting.ConfigurationSettingGuid = DataObject.getValue<Guid>(row["ConfigurationSettingGuid"], Guid.Empty);
                setting.KeyType = DataObject.getValue<string>(row["KeyType"], "");
                setting.SettingKey = DataObject.getValue<string>(row["SettingKey"], "");
                setting.SettingValue = DataObject.getValue<string>(row["SettingValue"], "");
                if (setting.SettingValue != "")
                    setting.SettingValue = (string.Compare(setting.KeyType, ConfigurationSettingDOClass.Key_Type_Password, StringComparison.CurrentCultureIgnoreCase) == 0 ?
                                    CryptoHelper.DecryptAesSymmetric(Convert.FromBase64String(setting.SettingValue), Guids.SiteAdminGuid) :
                                    setting.SettingValue);
                setting.CreatedDate = DataObject.getValue<DateTimeOffset>(row["CreatedDate"], DateTimeOffset.Now);
                setting.CreatedBy = DataObject.getValue<string>(row["CreatedBy"], BaseDataObject.ADMIN);
                setting.UpdatedDate = DataObject.getValue<DateTimeOffset>(row["UpdatedDate"], setting.CreatedDate.Value);
                setting.UpdatedBy = DataObject.getValue<string>(row["UpdatedBy"], BaseDataObject.ADMIN);
            }
        }
        #endregion Public methods
    }
}
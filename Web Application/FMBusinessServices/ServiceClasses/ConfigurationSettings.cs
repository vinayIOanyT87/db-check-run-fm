using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Data.SqlClient;
using System.Data;
using FMBusinessObjects.Constants;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.UtilityObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessServices.DataAccessLayer;
using FMBusinessServices.InternalClasses;
using FMBusinessObjects.Exceptions;

namespace FMBusinessServices.ServiceClasses
{
    using System.Diagnostics;
    using System.Runtime.Caching;
    using FMBusinessObjects.ChannelFactories;
    using FMCore;

    [ServiceBehavior(TransactionIsolationLevel = System.Transactions.IsolationLevel.ReadCommitted)]
	public class ConfigurationSettingsClass : IConfigurationSettings
	{
		#region Private data members
		private ConsolidatedDAClass consolidatedDA;
      private static readonly EventLogging eventLogging = new EventLogging();
        #endregion

        #region Constructors
      public ConfigurationSettingsClass()
		{
			this.consolidatedDA = new ConsolidatedDAClass();
		}
		#endregion

		#region Public data members
		public string GetKeyValueByKey(SecurityClass security, string key)
		{
			if (string.IsNullOrEmpty(key) == true)
			{
				throw new ArgumentNullException("Invalid Setting Key.");
			}

			// Kendall- need to let a null security object slide here so Query can work
			//if (security == null)
			//{
			//    throw new ArgumentNullException ( "Security" );
			//}

			ConfigurationSettingDOClass configSettingDO = new ConfigurationSettingDOClass();

			try
			{		   
				var cache = MemoryCache.Default;
				string cachedValue = cache[key] as string;
				if (string.IsNullOrEmpty(cachedValue))
	 			{
					 using (SqlCommand cmd = new SqlCommand())
					 {
						  configSettingDO.GetKeyValueByKeySQL(cmd, key);
						  DataSet dataSet = this.consolidatedDA.GetDataSet(cmd, security);

						  if ((dataSet != null) && (dataSet.Tables != null) && (dataSet.Tables.Count > 0))
						  {
								DataTable dataTable = dataSet.Tables[0];
								configSettingDO.Load(dataTable);
						  }
					 }
                var cacheItemPolicy = new CacheItemPolicy()
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(1)
                };
                cache.Set(key, configSettingDO.SettingValue, cacheItemPolicy);
            }
				else 
				{
					 return cachedValue; 
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Error retrieving Configuration Setting for key: " + key + "  " + ex.Message);
			}

			return configSettingDO.SettingValue;
		}

		public ConfigurationSettingDOClass GetByKey(SecurityClass security, string key)
		{
			if (string.IsNullOrEmpty(key) == true)
			{
				throw new ArgumentNullException("Invalid Setting Key.");
			}

			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			ConfigurationSettingDOClass configSettingDO = new ConfigurationSettingDOClass();

			try
			{
				using (SqlCommand cmd = new SqlCommand())
				{
					configSettingDO.GetKeyValueByKeySQL(cmd, key);
					DataSet dataSet = this.consolidatedDA.GetDataSet(cmd, security);

					if ((dataSet != null) && (dataSet.Tables != null) && (dataSet.Tables.Count > 0))
					{
						DataTable dataTable = dataSet.Tables[0];
						configSettingDO.Load(dataTable);
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Error retrieving Configuration Setting for key: " + key + "  " + ex.Message);
			}

			return configSettingDO;
		}

		public ConfigurationSettingDOClass GetByGuid(SecurityClass security, string guid)
		{
			if (string.IsNullOrEmpty(guid) == true)
			{
				throw new ArgumentNullException("Invalid Setting GUID.");
			}

			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			ConfigurationSettingDOClass configSettingDO = new ConfigurationSettingDOClass();

			try
			{
				using (SqlCommand cmd = new SqlCommand())
				{
					configSettingDO.GetKeyValueByGuidSQL(cmd, guid);
					DataSet dataSet = this.consolidatedDA.GetDataSet(cmd, security);

					if ((dataSet != null) && (dataSet.Tables != null) && (dataSet.Tables.Count > 0))
					{
						DataTable dataTable = dataSet.Tables[0];
						configSettingDO.Load(dataTable);
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Error retrieving Configuration Setting for GUID: " + guid + "  " + ex.Message);
			}

			return configSettingDO;
		}

		public ConfigurationSettingDOCollectionClass Enumerate(SecurityClass security)
		{
			if (security == null)
			{
				throw new ArgumentNullException("Security");
			}

			ConfigurationSettingDOCollectionClass configDOCollection = new ConfigurationSettingDOCollectionClass();
			ConfigurationSettingDOClass configSettingDO = new ConfigurationSettingDOClass();

			try
			{
				using (SqlCommand cmd = new SqlCommand())
				{
					configSettingDO.EnumerateSQL(cmd);
					DataSet dataSet = this.consolidatedDA.GetDataSet(cmd, security);

					if ((dataSet != null) && (dataSet.Tables != null) && (dataSet.Tables.Count > 0))
					{
						DataTable dataTable = dataSet.Tables[0];

						if (dataTable.Rows != null)
						{
							foreach (DataRow row in dataTable.Rows)
							{
								configSettingDO = new ConfigurationSettingDOClass();
								configSettingDO.Load(row);
								configDOCollection.Add(configSettingDO);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Error retrieving Configuration Setting items: " + ex.Message);
			}

			return configDOCollection;
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void Modify( SecurityClass security, string key, string keyValue )
		{
        // cache lifetime is only 1 minute. All the same, remove it immediately. 
        var cache = MemoryCache.Default;
        cache.Remove(key);
            if (security == null)
			{
				throw new ArgumentException("Invalid entry.");
			}

            // Allow keyValue to be empty but not null so we can save
			// an empty value.
			if ((string.IsNullOrEmpty(key) == true) || keyValue == null)
			{
				throw new ArgumentException("Invalid arguments.");
			}

            if ( security.HasRight( RIGHT.MODIFY_CONFIGURATION_SETTINGS ) == false )
            {
                throw new ApplicationException( "Access denied." );
            }

			using (SqlCommand cmd = new SqlCommand())
			{
				string sql = "UPDATE tblConfigurationSetting " +
						 "SET SettingValue = @KeyValue, UpdatedBy = @UpdatedBy, UpdatedDate = @UpdatedDate " +
						 "WHERE SettingKey = @Key";

				cmd.CommandText = sql;

				SqlParameter parm = new SqlParameter("@KeyValue", SqlDbType.NVarChar, 1000);
				parm.Value = keyValue;
				cmd.Parameters.Add(parm);

				parm = new SqlParameter("@Key", SqlDbType.NVarChar, 50);
				parm.Value = key;
				cmd.Parameters.Add(parm);

				parm = new SqlParameter("@UpdatedBy", SqlDbType.NVarChar, 50);
				parm.Value = security.UserID;
				cmd.Parameters.Add(parm);

				parm = new SqlParameter("@UpdatedDate", SqlDbType.DateTimeOffset);
				parm.Value = DateTimeOffset.Now;
				cmd.Parameters.Add(parm);

				try
				{
					this.consolidatedDA.ExecuteQuery(security, cmd);
            }
				catch (Exception ex)
				{
					throw new Exception("Error updating key: " + key + " " + ex.Message);
				}
			}
		}

        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void ModifyWithEncryption(SecurityClass security, string key, string keyValue, string keyType)
        {
            // cache lifetime is only 1 minute. All the same, remove it immediately. 
            var cache = MemoryCache.Default;
            cache.Remove(key);
            if (security == null)
            {
                throw new ArgumentException("Invalid entry.");
            }

            // Allow keyValue to be empty but not null so we can save an empty value.
            if ((string.IsNullOrEmpty(key) == true) || keyValue == null || (string.IsNullOrEmpty(keyType) == true))
            {
                throw new ArgumentException("Invalid arguments.");
            }

            if (security.HasRight(RIGHT.MODIFY_CONFIGURATION_SETTINGS) == false)
            {
                throw new ApplicationException("Access denied.");
            }

            if (key == "Cassandra_Password")
            {
                string[] credentials = new string[4];

                //0: Old Username
                //1: Old Password
                //2: New Username
                //3: New Password

                credentials[0] = this.GetByKey(security, "Cassandra_Username").SettingValue;
                credentials[1] = this.GetByKey(security, "Cassandra_Password").SettingValue;

                credentials[2] = credentials[0];
                credentials[3] = keyValue;



                var cassandraCredentialSuccess = FMChannelHelper.MakeCall<ICassandraAdministration, bool>(x => x.CassandraUserUpdate(security, credentials));
                if (!cassandraCredentialSuccess)
                {
                    eventLogging.LogEvent("The Cassandra credentials could not be authenticated", EventLogEntryType.Warning);
                }
                else
                {
                    eventLogging.LogEvent("The Cassandra credentials successfully updated to Cassandra", EventLogEntryType.Information);
                }
            }
            if (key == "Cassandra_Username")
            {
                string[] credentials = new string[4];

                //0: Old Username
                //1: Old Password
                //2: New Username
                //3: New Password

                credentials[0] = this.GetByKey(security, "Cassandra_Username").SettingValue;
                credentials[1] = this.GetByKey(security, "Cassandra_Password").SettingValue;


                credentials[2] = keyValue;
                credentials[3] = credentials[1];


                var cassandraCredentialSuccess = FMChannelHelper.MakeCall<ICassandraAdministration, bool>(x => x.CassandraUserUpdate(security, credentials));
                if (!cassandraCredentialSuccess)
                {
                    eventLogging.LogEvent("The Cassandra credentials could not be authenticated", EventLogEntryType.Warning);
                }
                else
                {
                    eventLogging.LogEvent("The Cassandra credentials successfully updated to Cassandra", EventLogEntryType.Information);
                }
            }

            if (string.Compare(keyType, ConfigurationSettingDOClass.Key_Type_Time, StringComparison.CurrentCultureIgnoreCase).Equals(0))
	        {
				if (keyValue.ToTimeSpan().Equals(TimeSpan.MinValue))
				{
					var exceptionMessage = string.Format("Setting Key '{0}' value '{1}' does not match any of the supported time formats.", key, keyValue);
					throw new ArgumentException(exceptionMessage);
		        }
	        }

            using (SqlCommand cmd = new SqlCommand())
            {
                string sql = "UPDATE tblConfigurationSetting " +
                         "SET SettingValue = @KeyValue, UpdatedBy = @UpdatedBy, UpdatedDate = @UpdatedDate " +
                         "WHERE SettingKey = @Key";

                cmd.CommandText = sql;


                SqlParameter parm = new SqlParameter("@KeyValue", SqlDbType.NVarChar, 1000);
                // if the key is of type password we want to encrypt it before saving it
                if (string.Compare( keyType, ConfigurationSettingDOClass.Key_Type_Password, StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    parm.Value = Convert.ToBase64String(CryptoHelper.EncryptAesSymmetric(keyValue, Guids.SiteAdminGuid));
                }
                else
                {
                    parm.Value = keyValue;
                }
                cmd.Parameters.Add(parm);

                parm = new SqlParameter("@Key", SqlDbType.NVarChar, 50);
                parm.Value = key;
                cmd.Parameters.Add(parm);

                parm = new SqlParameter("@UpdatedBy", SqlDbType.NVarChar, 50);
                parm.Value = security.UserID;
                cmd.Parameters.Add(parm);

                parm = new SqlParameter("@UpdatedDate", SqlDbType.DateTimeOffset);
                parm.Value = DateTimeOffset.Now;
                cmd.Parameters.Add(parm);

                try
                {
                    this.consolidatedDA.ExecuteQuery(security, cmd);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error updating key: " + key + " " + ex.Message);
                }
            }
        }

        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void UpdateIsEnterpriseSetting()
        {
            // cache lifetime is only 1 minute. All the same, remove it immediately. 
            var cache = MemoryCache.Default;
            cache.Remove("IsEnterprise");
            var innerSecurity = new SecurityClass();
            innerSecurity.AddRight(RIGHT.MODIFY_CONFIGURATION_SETTINGS);
            innerSecurity.SiteGuid = Guids.SiteAdminGuid;
            innerSecurity.UserID = DBAccess.ServiceLoginAccess;
            innerSecurity.SiteID = "SiteAdmin";

            HardwareKeyClass hardwareKey = new HardwareKeyClass();
            hardwareKey.ReadHardwareKey();

            ConfigurationSettingsClass configurationSettings = new ConfigurationSettingsClass();
            configurationSettings.Modify(
                innerSecurity,
                ConfigurationSettingDOClass.Key_IsEnterprise,
                hardwareKey.IsEnterpriseKey() ? "1" : "0");
        }

		#endregion
	}
}
namespace FMBusinessServices.InternalClasses
{
    using System;
    using System.Linq;

    using FMBusinessObjects.DataObjects;

    using FMBusinessServices.InternalInterfaces;
    using FMBusinessServices.ServiceClasses;

    internal class CassandraConnectionConfig : ICassandraConnectionConfig
	{
        public string[] GetContactPoints(SecurityClass security)
        {
            ConfigurationSettingsClass configSettings = new ConfigurationSettingsClass();
            ConfigurationSettingDOClass contactPoints = configSettings.GetByKey(security, ConfigurationSettingDOClass.Key_Cassandra_Configuration);
            var cps = new string(contactPoints.SettingValue.ToCharArray().Where(c => !Char.IsWhiteSpace(c)).ToArray()); 
            return cps.Split(',');
        }
		public int GetReplicationFactor(SecurityClass security)
		{
			ConfigurationSettingsClass configSettings = new ConfigurationSettingsClass();
			ConfigurationSettingDOClass replicationFactor = configSettings.GetByKey(security, ConfigurationSettingDOClass.Key_Cassandra_ReplicationFactor);
			return System.Convert.ToInt32(replicationFactor.SettingValue);
		}
		public string GetConsistencyLevel(SecurityClass security)
		{
			ConfigurationSettingsClass configSettings = new ConfigurationSettingsClass();
			ConfigurationSettingDOClass consistencyLevel = configSettings.GetByKey(security, ConfigurationSettingDOClass.Key_Cassandra_ConsistencyLevel);
			return consistencyLevel.SettingValue.ToString();
		}

	    public string[] GetCredentials(SecurityClass security)
	    {
		    ConfigurationSettingsClass configSettings = new ConfigurationSettingsClass();
		    ConfigurationSettingDOClass username = configSettings.GetByKey(
			    security,
			    ConfigurationSettingDOClass.Key_Cassandra_Username);
		    ConfigurationSettingDOClass password = configSettings.GetByKey(
			    security,
			    ConfigurationSettingDOClass.Key_Cassandra_Password);
		    string[] credentials = new string[2] { username.SettingValue, password.SettingValue };
			return credentials;
		}
	}
}

namespace FMBusinessObjects.UtilityObjects
{
    using System;
    using System.Configuration;
    using System.Collections.Specialized;

    public class AppSettingsHelper
	{
		/// <summary>
		/// Reads a given AppSettings key from the config file, and returns the value as a given data type. If the key is not found or the data conversion fails, the supplied default value for the key is returned;
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="appSettingKey"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static T GetKeyValue<T> ( string appSettingKey, T defaultValue )
		{
			T value							= defaultValue;
			object objectValue				= null;
			bool invalidType				= false;
			NameValueCollection appSettings = GetAppSettings();

			if (( appSettings[appSettingKey] == null ) || ( appSettings[appSettingKey].Length == 0 ))
			{
				return defaultValue;
			}

			Type keyDataType = typeof ( T );

			try
			{
				if (keyDataType == typeof ( System.Int32 ))
				{
					objectValue = (object) ( Convert.ToInt32 ( appSettings[appSettingKey] ) );
				}
				else if (keyDataType == typeof ( System.Boolean ))
				{
					objectValue = (object) ( Convert.ToBoolean ( appSettings[appSettingKey] ) );
				}
				else if (keyDataType == typeof ( System.String ))
				{
					objectValue = (object) ( Convert.ToString ( appSettings[appSettingKey] ) );
				}
				else if (keyDataType == typeof ( System.UInt32 ))
				{
					objectValue = (object) ( Convert.ToUInt32 ( appSettings[appSettingKey] ) );
				}
				else if (keyDataType == typeof ( System.UInt16 ))
				{
					objectValue = (object) ( Convert.ToUInt16 ( appSettings[appSettingKey] ) );
				}
				else
				{
					invalidType = true;
				}

				value = (T) objectValue;
			}
			catch (Exception)
			{
				value = defaultValue;
			}

			if (invalidType)
			{
				throw new InvalidOperationException ( "Type " + typeof ( T ).ToString ( ) + " is not supported by operation Logger.GetAppSettingsKeyValue()." );
			}

			return value;
		}

        public static NameValueCollection GetAppSettings()
        {
            return ConfigurationManager.AppSettings;
        }
	}
}

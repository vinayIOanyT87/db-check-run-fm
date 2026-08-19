// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationSettingsHelper.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   The configuration settings helper.  Gets values from the settings configuration table and converts
//   them to the data type indicated by the default value parameter of the getValue() method
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.UtilityObjects
{
   using System;

   using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
   using FMBusinessObjects.DataObjects;

   /// <summary>
   /// The configuration settings helper.  Gets values from the settings configuration table and converts
   /// them to the data type indicated by the default value parameter of the getValue() method
   /// </summary>
	public class ConfigurationSettingsHelper
	{
		/// <summary>
		/// Gets the specified setting value from the database and returns the value as the data type indicated
		/// by the default value parameter. If the setting key is not found or the data conversion fails, the 
		/// supplied default value for the setting key is returned;
		/// </summary>
		/// <typeparam name="T">The type parameter</typeparam>
		/// <param name="settingKey">The setting key</param>
		/// <param name="defaultValue">The default value</param>
		/// <param name="security">The security object</param>
		/// <returns>The setting value</returns>
		public static T GetValue<T>(string settingKey, T defaultValue, SecurityClass security)
		{
			if (string.IsNullOrWhiteSpace(settingKey))
			{
				return defaultValue;
			}

			if (security == null)
			{
				return defaultValue;
			}
         T settingValue = defaultValue;
			bool invalidType = false;
			try
			{
				ConfigurationSettingDOClass configDo =
					FMChannelHelper.MakeCall<IConfigurationSettings, ConfigurationSettingDOClass>(
						configurationSettings => configurationSettings.GetByKey(security, settingKey));

				object objectValue = null;
				Type keyDataType = typeof(T);
				bool validResult = false;
				if (keyDataType == typeof(int))
				{
					int intResult;
					validResult = int.TryParse(configDo.SettingValue, out intResult);
					if (validResult)
					{
						objectValue = intResult;
					}
				}
				else if (keyDataType == typeof(bool))
				{
					bool boolResult;
					validResult = bool.TryParse(configDo.SettingValue, out boolResult);
					if (validResult)
					{
						objectValue = boolResult;
					}
				}
				else if (keyDataType == typeof(string))
				{
					if (configDo.SettingValue is string)
					{
						objectValue = configDo.SettingValue;
						validResult = true;
               }
				}
				else if (keyDataType == typeof(uint))
				{
					uint uintResult;
					validResult = uint.TryParse(configDo.SettingValue, out uintResult);
					if (validResult)
					{
						objectValue = uintResult;
					}
				}
				else if (keyDataType == typeof(ushort))
				{
					ushort ushortResult;
					validResult = ushort.TryParse(configDo.SettingValue, out ushortResult);
					if (validResult)
					{
						objectValue = ushortResult;
					}
				}
				else
				{
					invalidType = true;
				}

				if (validResult)
				{
					settingValue = (T)objectValue;
				}
			}
			catch (Exception)
			{
				settingValue = defaultValue;
			}

			if (invalidType)
			{
				throw new InvalidOperationException(
					"Type " + typeof(T) + " is not supported by ConfigurationSettingsHelper.GetValue<T>().");
			}

			return settingValue;
		}
   }
}

///***************************************************************************
/// Module Name:  ConfigReader.cs
/// Author:       Bryan Ponnwitz
/// Copyright (c) Varec, Inc. 2016 All rights reserved.
///***************************************************************************

using System;
using System.Linq;
using System.IO;
using System.Reflection;
using FMBusinessObjects.UtilityObjects;

namespace FMExportService.Config
{
	public sealed class ConfigReader {

		/// <summary>
		/// Reads the configuration information from the config file provided.
		/// </summary>
		/// <param name="ConfigFileName">The file name of the configuration file. This value is not the full path, just the file name.</param>
		/// <returns>A Configuration object loaded with the data from the config file.</returns>
		/// <remarks>The system assumes that the interfaces, services, and configuration files all exist in the same directory.</remarks>
		public static Configuration LoadConfiguration(string ConfigFileName) {
			Configuration objConfig;
			string strPath, strDir, strFileName, strBuffer;

			strPath = Assembly.GetExecutingAssembly().Location;
			strDir = Path.GetDirectoryName(strPath);
			strFileName = Path.Combine(strDir, ConfigFileName);

			if (!File.Exists(strFileName))
				return new Configuration();

			strBuffer = File.ReadAllText(strFileName);
			objConfig = (Configuration)XmlObjConverter.FromXml(strBuffer, typeof(Configuration));
			return objConfig;
		}

		public static string[] GetConfigSetting(Configuration Config, string SettingName) {
			ConfigStringArraySetting objSetting;
			ConfigStringArraySetting[] objSettings;

			if (Config == null || Config.AppSettings == null)
				throw new ArgumentException("The configuration and application settings from the configuration file cannot be null.");

			objSettings = (from objSet in Config.AppSettings.Settings where objSet.Name == SettingName select objSet).ToArray();
			if (objSettings.GetLength(0) != 1)
				throw new ArgumentException("The setting specified was not found or was found multiple times.\nSettingName: " + SettingName);

			objSetting = objSettings[0];
			return objSetting.Value.ArrayOfString.Values;
		}
		
	}
}
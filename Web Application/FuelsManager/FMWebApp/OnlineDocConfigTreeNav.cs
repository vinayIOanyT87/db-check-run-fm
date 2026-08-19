// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OnlineDocConfigTreeNav.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the OnlineDocConfigTreeNav type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System.Collections.Generic;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	public class OnlineDocConfigTreeNav : IMenuDiscovery
	{
		#region Enums

		public enum TreeNodeLevel
		{
			MAIN,

			CHILD
		};

		#endregion

		#region Public Methods and Operators

		/// <summary>
		///    Gets a list of menu items that should be displayed for the current user.
		/// </summary>
		/// <param name="security">The security object of the current session</param>
		/// <param name="siteGroup">Whether the current logged-in site is a site group</param>
		/// <param name="options">Hardware key options</param>
		/// <returns>
		///    List of menu items to be displayed
		/// </returns>
		public List<FMMenuItem> GetMenuItems(SecurityClass security, bool siteGroup, ushort word1,ushort word2, ushort useNewLicenseKey, uint options)
		{
			return null;
		}

		#endregion

		#region Methods

		/// <summary>
		///    This method will retrieve the online admin manual URL from the web config. It will return
		///    an empty string if the entry does not exists.
		/// </summary>
		/// <returns></returns>
		private string CheckOnlineAdminHelpRegistry(SecurityClass security)
		{
			string onlineAdminDoc = FMChannelHelper.MakeCall<IConfigurationSettings, string>(
																	 x =>
																	 x.GetKeyValueByKey(security, ConfigurationSettingDOClass.Key_OnlineAdminDoc)
																);
			if (string.IsNullOrEmpty(onlineAdminDoc))
			{
				onlineAdminDoc = "";
			}

			return onlineAdminDoc;
		}

		/// <summary>
		///    This method will retrieve the online tutorials URL from the web config. It will return
		///    an empty string if the entry does not exists.
		/// </summary>
		/// <returns></returns>
		private string CheckOnlineAdminTutorialRegistry(SecurityClass security)
		{
			string onlineAdminTutorialDoc = 
				FMChannelHelper.MakeCall<IConfigurationSettings, string>(
						x =>
						x.GetKeyValueByKey(security, ConfigurationSettingDOClass.Key_OnlineAdminTutorialDoc)
				);

			if (string.IsNullOrEmpty(onlineAdminTutorialDoc))
			{
				onlineAdminTutorialDoc = "";
			}

			return onlineAdminTutorialDoc;
		}

		/// <summary>
		///    This method will retrieve the online help URL from the web config file. It will return
		///    an empty string if the entry does not exists.
		/// </summary>
		/// <returns></returns>
		private string CheckOnlineHelpRegistry(SecurityClass security)
		{
			string onlineHelpDoc = FMChannelHelper.MakeCall<IConfigurationSettings, string>(
																	 x =>
																	 x.GetKeyValueByKey(security, ConfigurationSettingDOClass.Key_OnlineHelpDoc)
																);
			if (string.IsNullOrEmpty(onlineHelpDoc))
			{
				onlineHelpDoc = "";
			}

			return onlineHelpDoc;
		}

		/// <summary>
		///    This method will retrieve the online tutorials URL from the web config. It will return
		///    an empty string if the entry does not exists.
		/// </summary>
		/// <returns></returns>
		private string CheckTutorialsRegistry(SecurityClass security)
		{
			string onlineTutorialDoc = FMChannelHelper.MakeCall<IConfigurationSettings, string>(
																	 x =>
																	 x.GetKeyValueByKey(security, ConfigurationSettingDOClass.Key_OnlineTutorialDoc)
																);

			if (string.IsNullOrEmpty(onlineTutorialDoc))
			{
				onlineTutorialDoc = "";
			}

			return onlineTutorialDoc;
		}

		#endregion
	}
}
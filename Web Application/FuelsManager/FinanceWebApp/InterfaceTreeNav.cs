// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InterfaceTreeNav.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the InterfaceTreeNav type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FinanceWebApp
{
	using System.Collections.Generic;
	using System.Configuration;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using FuelsManager.FMWebApp;

	public class InterfaceTreeNav : IMenuDiscovery
	{
		#region Properties

		/// <summary>
		///    This method will return true if the application is being used on the ADF
		///    project.  Otherwise, it returns false.
		/// </summary>
		/// <returns></returns>
		private bool IsADFProject
		{
			get
			{
				bool isADF = false;

				if (ConfigurationManager.AppSettings["AccountingTransactionDetailURL"] != null)
				{
					string accountingURL = ConfigurationManager.AppSettings["AccountingTransactionDetailURL"];

					if ((accountingURL != null) && (accountingURL.Length > 0))
					{
						if (accountingURL.Contains("ADFWebApp"))
						{
							isADF = true;
						}
					}
				}

				return isADF;
			}
		}

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

		/// <summary>
		///    This method will return a true value if the machine is configured with a FuelsManager Defense Professional Key.
		/// </summary>
		/// <param name="SpecialKeyCodes"></param>
		/// <returns></returns>
		public bool HadProfessionalSetting(uint specialKeyCodes)
		{
			bool hasSetting = true;

			if ((specialKeyCodes & 0x00000020) == 0)
			{
				hasSetting = false;
			}

			if (((specialKeyCodes & 0x00000004) == 0) || ((specialKeyCodes & 0x00000008) == 0))
			{
				hasSetting = false;
			}

			return hasSetting;
		}

		/// <summary>
		///    This method will return true if there is a valid hardware key for Enterprise Reports.
		///    Otherwise, it will return false. The key is located in the upper word of a 32 bit word
		///    and the value is 0x10.
		/// </summary>
		/// <returns></returns>
		public bool HasHardwareKey(uint options)
		{
			bool hasKey = true;

			if (FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsDescKey()) || (options & 0x100000) == 0)
			{
				hasKey = false;
			}

			return hasKey;
		}

		/// <summary>
		///    This method will determine if the user has modify permissions. If so,
		///    the method will return true. Otherwise, it returns false.
		/// </summary>
		/// <param name="security"></param>
		/// <returns></returns>
		public bool HasModifyPermissions(SecurityClass security)
		{
			return security.HasRight(RIGHT.INTERFACE_IMPORT);
		}

		/// <summary>
		///    This method will determine if the user has Interface Import/Export permissions. If so,
		///    the method will return true. Otherwise, it returns false.
		/// </summary>
		/// <param name="security"></param>
		/// <returns></returns>
		public bool HasViewPermissions(SecurityClass security)
		{
			return security.HasRight( RIGHT.INTERFACE_IMPORT );
		}

		#endregion
	}
}
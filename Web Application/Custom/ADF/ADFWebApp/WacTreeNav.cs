using System;
using System.Collections.Generic;
using Microsoft.Web.UI.WebControls;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.Interfaces;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMWebApp;

namespace ADFWebApp
{
	public class WacTreeNav : IMenuDiscovery
	{
		protected static string WACTREE_URL = "../ADFWebApp/";

		/// <summary>
		/// Gets a list of menu items that should be displayed for the current user.
		/// </summary>
		/// <param name="security">The security object of the current session</param>
		/// <param name="siteGroup">Whether the current logged-in site is a site group</param>
		/// <param name="options">Hardware key options</param>
		/// <returns>
		/// List of menu items to be displayed
		/// </returns>
		public List<FMMenuItem> GetMenuItems(SecurityClass security, bool siteGroup, uint options)
		{
			var menuItems = new List<FMMenuItem>();

			if (siteGroup &&
				(this.HasHardwareKey(options) == true) &&
				((this.HasViewPermissions(security) == true) || (this.HasModifyPermissions(security) == true)))
			{
				FMChannelFactory<IHardwareKey> hardwareKeyClient = new FMChannelFactory<IHardwareKey> ( );
				IHardwareKey hardWareKey = hardwareKeyClient.CreateProxy ( );

				if (hardWareKey.IsDescProfessionalKey() == false)
				{
					menuItems.Add(new FMMenuItem
					{
						MenuItemType = FMMenuItemType.ACCOUNTING_MAIN_WAC_SUMMARY,
						RootMenuName = "Accounting",
						CategoryName = "Main",
						ItemName = "WAC Summary",
						NavigateUrl = WacTreeNav.WACTREE_URL + "WacSummary.aspx",
						ApplyDataDictionary = ApplyDataDictionary.DoNotApply
					});
				}
			}

			return menuItems;
		}

		/// <summary>
		/// This method will determine if the user has modify permissions. If so,
		/// the method will return true. Otherwise, it returns false.
		/// </summary>
		/// <param name="security"></param>
		/// <returns></returns>
		public bool HasModifyPermissions ( SecurityClass security )
		{
			bool hasPermission = false;
			List<RIGHT> rightList = security.RightCollection;

			foreach (RIGHT right in rightList)
			{
				if (right == RIGHT.OVERRIDE_WAC)
				{
					hasPermission = true;
					break;
				}
			}

			return hasPermission;
		}

		/// <summary>
		/// This method will determine if the user has view only permissions. If so,
		/// the method will return true. Otherwise, it returns false.
		/// </summary>
		/// <param name="security"></param>
		/// <returns></returns>
		public bool HasViewPermissions ( SecurityClass security )
		{
			bool hasViewPermission = false;
			List<RIGHT> rightList = security.RightCollection;

			foreach (RIGHT right in rightList)
			{
				if (right == RIGHT.VIEW_WAC_HISTORY)
				{
					hasViewPermission = true;
					break;
				}

			}

			return hasViewPermission;
		}

		/// <summary>
		/// This method will return true if there is a valid hardware key for Enterprise Reports.
		/// Otherwise, it will return false. The key is located in the upper word of a 32 bit word 
		/// and the value is 0x10.
		/// </summary>
		/// <returns></returns>
		public bool HasHardwareKey ( uint Options )
		{
			bool hasKey = true;

			if (( Options & 0x100000 ) == 0)
			{
				hasKey = false;
			}

			return hasKey;
		}
	}
}

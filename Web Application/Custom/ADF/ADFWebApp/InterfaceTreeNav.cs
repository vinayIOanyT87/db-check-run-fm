/// <summary>
///   File name:	InterfaceTreeNav.cs
///   Purpose:	   Used by Shared Components left tree navigation to discover
///				   the Interface tree structure.
///				
///	Comments:   Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 
///				   2000.  This file shall not be copied or reproduced in any form 
///				   without the express written consent of Endress+Hauser.
///				
///	Author(s):	Richard R. Panachida
///	Version:	   1.0.0  Current version
///	
///	Modification History:
///		Date:			By:						Reason:
///		----------	--------------------	----------------------------------
///		2007-11-15	Richard Panachida		Added a link to a splash page for the main node.
///		2007-11-19	Eric Simmons			Added HadProfessionalSetting() to resolve CSI #5366
///		2008-09-19	Bill Dimovski			Added 4 sub menus. Changed Right check to Interface Import/Export CSI 385
///		2009-03-12  I.Orndorff              - Changed "Ground Fuel Transactions" to "Fuel Transactions", this addresses 
///		                                      change request 1849.
/// </summary>

using Microsoft.Web.UI.WebControls;
using System;
using System.Configuration;
using System.Collections.Generic;
using FMBusinessObjects.Interfaces;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMWebApp;

namespace ADFWebApp
{
	public class InterfaceTreeNav : IMenuDiscovery
	{
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

			if ((this.IsADFProject == true) &&
				((this.HasViewPermissions(security) == true) || (this.HasModifyPermissions(security) == true)))
			{
				menuItems.Add(new FMMenuItem
					{
						MenuItemType = FMMenuItemType.OPERATIONS_INTERFACE_FUEL_TRANSACTIONS,
						RootMenuName = "Operations",
						CategoryName = "Interface",
						ItemName = "Fuel Transactions",
						NavigateUrl = "../ADFWebApp/TFMSImportForm.aspx",
						ApplyDataDictionary = ApplyDataDictionary.DoNotApply
					});

				menuItems.Add(new FMMenuItem
				{
					MenuItemType = FMMenuItemType.OPERATIONS_INTERFACE_ADOFMS_GROUND_FUEL,
					RootMenuName = "Operations",
					CategoryName = "Interface",
					ItemName = "ADOFMS Ground Fuel",
					NavigateUrl = "../ADFWebApp/ADOFMSImportForm.aspx",
					ApplyDataDictionary = ApplyDataDictionary.DoNotApply
				});
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
				if (right == RIGHT.INTERFACE_IMPORT)
				{
					hasPermission = true;
					break;
				}
			}

			return hasPermission;
		}

		/// <summary>
		/// This method will determine if the user has Interface Import/Export permissions. If so,
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
				if (right == RIGHT.INTERFACE_IMPORT)
				{
					hasViewPermission = true;
					break;
				}
			}

			return hasViewPermission;
		}

		/// <summary>
		/// This method will return true if the application is being used on the ADF
		/// project.  Otherwise, it returns false.
		/// </summary>
		/// <returns></returns>
		private bool IsADFProject
		{
			get
			{
				bool isADF = false;

				if (ConfigurationManager.AppSettings["AccountingTransactionDetailURL"] != null)
				{
					string accountingURL = (string) ConfigurationManager.AppSettings["AccountingTransactionDetailURL"];

					if (( accountingURL != null ) && ( accountingURL.Length > 0 ))
					{
						if (accountingURL.Contains ( "ADFWebApp" ) == true)
						{
							isADF = true;
						}
					}
				}

				// Check the hardware key in addition to the application setting.
				if (isADF == true)
				{
					FMChannelFactory<IHardwareKey> hardwareKeyClient = new FMChannelFactory<IHardwareKey> ( );
					IHardwareKey hardwareKey = hardwareKeyClient.CreateProxy ( );

					isADF = hardwareKey.IsADFKey ( );
				}

				return isADF;
			}
		}
	}
}

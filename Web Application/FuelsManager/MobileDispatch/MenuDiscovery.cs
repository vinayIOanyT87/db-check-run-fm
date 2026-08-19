// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MenuDiscovery.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
// Menu discovery class file for Dispatch.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.Wingware
{
	using System.Collections.Generic;

	using FMBusinessObjects.DataObjects;

	using FuelsManager.FMWebApp;

	/// <summary>
	/// Contains menu discovery code for Dispatch.
	/// </summary>
		public class MenuDiscovery : IMenuDiscovery
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
			public List<FMMenuItem> GetMenuItems(SecurityClass security, bool siteGroup, ushort word1, ushort word2, ushort useNewLicenseKey, uint options)
			{
                bool missingLicense = false;

                if (useNewLicenseKey == 1)
                {
                    if ((word1 & 0x40) != 0x40)
                        missingLicense = true;
                }
                else
                {
                    if ((options & 0x1000) != 0x1000)
                        missingLicense = true;
                }

                //missingLicense = true;

                var menuItems = new List<FMMenuItem>();

                // Enforce certain security to access this menu -- Remember: Modify implies View in the HasRight() check
                if (security.HasRight(RIGHT.MOBILE_ROOT_MENU_DISPLAY) == true)
                {
                    if (missingLicense)
                    {
                        var generalMenuItem = new FMMenuItem
                        {
                            MenuItemType = FMMenuItemType.MOBILE_GENERAL,
                            RootMenuName = "Mobile",
                            CategoryName = "General",
                            ItemName = "Feature disabled. Please call Varec sales to enable it.",
                            NavigateUrl = "",
                            ApplyDataDictionary = ApplyDataDictionary.Apply,
                            OpenInSeparateTab = false,
                            IsEnabled = false
                        };
                        menuItems.Add(generalMenuItem);
                        return menuItems;
                    }
                }
                else
                {
                    return null;
                }


                // Enforce certain security to access this menu -- Remember: Modify implies View in the HasRight() check
                if (security.HasRight(RIGHT.MOBILE_LAUNCH) == true)
                {
                var launchMenuItem = new FMMenuItem
                {
                    MenuItemType = FMMenuItemType.MOBILE_LAUNCH,
                    RootMenuName = "Mobile",//"WW-Dispatch",
                    CategoryName = "Launch",
                    ItemName = "Dispatch",
                    NavigateUrl= "javascript:FetchMobileDispatchSite();",
                    ApplyDataDictionary = ApplyDataDictionary.Apply,
                    OpenInSeparateTab = false
                };

                menuItems.Add(launchMenuItem);
                }


                if (security.HasRight(RIGHT.MOBILE_MODIFY_CONFIGURATION))
                {
                    var configMenuItem = new FMMenuItem
                    {
                        MenuItemType = FMMenuItemType.MOBILE_MODIFY_CONFIGURATION,
                        RootMenuName = "Mobile", //"WW-Dispatch",
                        CategoryName = "Configuration",
                        ItemName = "FM & WingWare field map",
                        NavigateUrl = "../DispatchWebApp/DispatchValidationsConfigurationPage.aspx",
                        ApplyDataDictionary = ApplyDataDictionary.Apply
                    };
                    menuItems.Add(configMenuItem);
                }

                if(menuItems.Count == 0)
                {
                    var generalMenuItem = new FMMenuItem
                    {
                        MenuItemType = FMMenuItemType.MOBILE_GENERAL,
                        RootMenuName = "Mobile",
                        CategoryName = "General",
                        ItemName = "Please configure security rights to see the menu.",
                        NavigateUrl = "",
                        ApplyDataDictionary = ApplyDataDictionary.Apply,
                        OpenInSeparateTab = true,
                        IsEnabled = false
                    };
                    menuItems.Add(generalMenuItem);
                }

                return menuItems;
            }

   //     private static string javascriptInvokeUserAuthForm = @"
			//<script type='text/javascript'>
			//	showModalDialogFrame({
			//		url: '../MobileDispatch/UserAuthForm.aspx',
			//		width: 955,
			//		height: 560,
			//		//doPostBackAfterCloseCallback: false,
			//		//onClose: function ()
			//		//{
			//		//	if (this.returnValue != null)
			//		//	{
			//		//		__mydoPostBack( 'COMBINE_TRANSACTION', this.returnValue[0] );	
			//		//	}
			//		//}
			//	});
			//</script>
			//";
        }
	}

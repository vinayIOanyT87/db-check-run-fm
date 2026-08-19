using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FMBusinessObjects.DataObjects;

namespace FuelsManager.FMWebApp
{
	/// <summary>
	/// An interface to assist with dynamic creation of menu items. Web page classes, and
	/// other types of classes, can implement this interface, then FMMenuEngine will use
	/// reflection to find all the implementors, will call each class's GetMenuItems()
	/// method, and will thereby collect all the menu items for the application.
	/// </summary>
	public interface IMenuDiscovery
	{
		/// <summary>
		/// Gets a list of menu items that should be displayed for the current user.
		/// </summary>
		/// <param name="security">The security object of the current session</param>
		/// <param name="siteGroup">Whether the current logged-in site is a site group</param>
		/// <param name="word1">Hardware key options</param>
		/// <param name="word2">Hardware key options</param>
		/// <returns>List of menu items to be displayed</returns>
		List<FMMenuItem> GetMenuItems(SecurityClass security, bool siteGroup, ushort word1,ushort word2,ushort useNewLicenseKey, uint options);
	}
}
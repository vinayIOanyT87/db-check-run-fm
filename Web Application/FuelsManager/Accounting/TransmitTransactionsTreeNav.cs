// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransmitTransactionsTreeNav.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the TransmitTransactionsTreeNav type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.Accounting
{
	using System.Collections.Generic;

	using FMBusinessObjects.DataObjects;

	using FuelsManager.FMWebApp;

	/// <summary>
	/// Menu response class for transmit transactions
	/// </summary>
	public class TransmitTransactionsTreeNav : AccountingTreeNav, IMenuDiscovery
	{
		#region Explicit Interface Methods

		/// <summary>
		/// Gets a list of menu items that should be displayed for the current user.
		/// </summary>
		/// <param name="security">
		/// The security object of the current session 
		/// </param>
		/// <param name="siteGroup">
		/// Whether the current logged-in site is a site group 
		/// </param>
		/// <param name="options">
		/// Hardware key options 
		/// </param>
		/// <returns>
		/// List of menu items to be displayed 
		/// </returns>
		List<FMMenuItem> IMenuDiscovery.GetMenuItems(SecurityClass security, bool siteGroup, ushort word1,ushort word2, ushort useNewLicenseKey, uint options)
		{
			return null;
		}

		#endregion
	}
}

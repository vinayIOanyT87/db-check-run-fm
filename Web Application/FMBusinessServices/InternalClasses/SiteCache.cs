// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SiteCache.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the SiteCache type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessServices.InternalClasses
{
	using System;
	using System.Collections.Generic;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	///   Implements a cache of SiteClass objects as a Dictionary (key = Guid, value = SiteClass) container.
	///   The dictionary container is essentially a hash table with an Guid key and SiteClass value.  The
	///   exposed methods are locked for thread safety and consist of GetSite, AddSite and RemoveSite.
	/// </summary>
	internal static class SiteCache
	{
		#region Constants and Fields

		/// <summary>
		/// Internal site cache table for storing the cached site objects.
		/// </summary>
		private static readonly Dictionary<Guid, SiteClass> SiteTable = new Dictionary<Guid, SiteClass>();

		/// <summary>
		/// Semaphore object.
		/// </summary>
		private static readonly object ThisLock = new object();

		#endregion

		#region Methods

		/// <summary>
		/// Adds a SiteClass to the site cache. The SiteIndex of the SiteClass is used as the site cache key. 
		/// If the site cache already contains a SiteClass at the specified index then it is overwritten. 
		/// During execution the site cache is locked for thread safety.
		/// </summary>
		/// <param name="site">
		/// The SiteClass to add or modify 
		/// </param>
		internal static void AddSite(SiteClass site)
		{
			if (site != null)
			{
				lock (ThisLock)
				{
					SiteTable[site.SiteGuid] = site; // Overwrites site if it already exists in table
				}
			}
		}

		/// <summary>
		/// Gets a SiteClass from the site cache. During execution the site cache is locked for thread safety.
		/// </summary>
		/// <param name="siteGuid">
		/// The siteGuid of the SiteClass to get 
		/// </param>
		/// <returns>
		/// The requested site class object or null if it is not found.
		/// </returns>
		internal static SiteClass GetSite(Guid siteGuid)
		{
			lock (ThisLock)
			{
				if (SiteTable.ContainsKey(siteGuid))
				{
					return SiteTable[siteGuid];
				}

				return null; // Site not found
			}
		}

		/// <summary>
		/// Removes a SiteClass from the site cache. During execution the site cache is locked for thread safety.
		/// </summary>
		/// <param name="siteGuid">
		/// The SiteGuid of the SiteClass to remove 
		/// </param>
		internal static void RemoveSite(Guid siteGuid)
		{
			lock (ThisLock)
			{
				if (SiteTable.ContainsKey(siteGuid))
				{
					SiteTable.Remove(siteGuid);
				}
			}
		}

		#endregion
	}
}
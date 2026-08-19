// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Afss.BusinessObjects.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the additional rights supported by the Automated Fuel Service Station module.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FuelsManager.Afss.BusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	using FMBusinessObjects.Interfaces;
	using FMBusinessObjects.DataObjects;

	public class AFSSRights : ISecurityDiscovery
	{
		#region Public Methods and Operators
		/// <summary>
		/// This method returns the NATO custom rights.
		/// </summary>
		/// <param name="security">The FuelsManager security object.</param>
		/// <param name="options">Hardware key options.</param>
		/// <param name="specialKeyCodes">Hardware key special key codes.</param>
		/// <returns>NATO custom rights.</returns>
		public RightCollectionClass GetSecurityRights(SecurityClass security, uint options, uint specialKeyCodes)
		{
			var rightsCollection = new RightCollectionClass
								   {
									   RIGHT.VIEW_AUTOMATED_FUEL_SERVICE_STATION,
									   RIGHT.MODIFY_AUTOMATED_FUEL_SERVICE_STATION
								   };

			return rightsCollection;
		}
		#endregion
	}
}



namespace NspaBusinessObjects.DataObjects
{
}


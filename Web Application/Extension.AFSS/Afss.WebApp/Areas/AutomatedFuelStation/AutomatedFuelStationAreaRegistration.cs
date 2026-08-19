// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutomatedFuelStationAreaRegistration.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Registers the Automated Fuel Station Area
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FuelsManager.Afss.WebApp.Areas.AutomatedFuelStation
{
	using System.Web.Mvc;

	/// <summary>
	/// Registers the Fueling Station Area
	/// </summary>
	public class AutomatedFuelStationAreaRegistration : AreaRegistration 
	{
		/// <summary>
		/// The name of the area to register
		/// </summary>
		public override string AreaName 
		{
			get 
			{
				return "AutomatedFuelStation";
			}
		}

		/// <summary>
		/// Registers route information for the Fueling Station Area
		/// </summary>
		/// <param name="context">Encapsulates the information that is required in order to register the area.</param>
		public override void RegisterArea(AreaRegistrationContext context) 
		{
			context.MapRoute(
				"AutomatedFuelStation_default",
				"AutomatedFuelStation/{controller}/{action}/{id}",
				new { action = "Index", id = UrlParameter.Optional }
			);
		}
	}
}
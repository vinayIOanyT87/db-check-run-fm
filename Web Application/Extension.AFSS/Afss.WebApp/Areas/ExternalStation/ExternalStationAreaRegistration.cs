// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExternalStationAreaRegistration.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Registers the External Station Area
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FuelsManager.Afss.WebApp.Areas.ExternalStation
{
    using System.Web.Mvc;

    /// <summary>
    /// Registers the External Station Area
    /// </summary>
    public class ExternalStationAreaRegistration : AreaRegistration 
    {
        /// <summary>
        /// The name of the area to register
        /// </summary>
        public override string AreaName 
        {
            get 
            {
                return "ExternalStation";
            }
        }

        /// <summary>
        /// Registers route information for the External Station Area
        /// </summary>
        /// <param name="context">Encapsulates the information that is required in order to register the area.</param>
        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "ExternalStation_default",
                "ExternalStation/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
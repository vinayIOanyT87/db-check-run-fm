// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GasboyStationEventModel.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   The model for the Gasboy Station Event Detail Page, which is accessible from the External Station Log summary page when you view a log that is a station event
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.Afss.WebApp.Areas.AutomatedFuelStation.Models
{
    using FuelsManager.Afss.Module.Gasboy.BusinessObjects.DataObjects;

    /// <summary>
    /// The model for the Gasboy Station Event Detail Page, which is accessible from the External Station Log summary page when you view a log that is a station event
    /// </summary>
    public class GasboyStationEventModel
    {
        /// <summary>
        /// Initialize the event with a new record to avoid potential null reference errors should an error occur.
        /// </summary>
        public GasboyStationEventModel()
        {
            this.Log = new GasboyStationEvent();
        }

        /// <summary>
        /// The event record to display
        /// </summary>
        public GasboyStationEvent Log { get; set; }

        /// <summary>
        /// The format to use when displaying dates
        /// </summary>
        public string ShortDatePattern { get; set; }

        /// <summary>
        /// The format to use when displaying times
        /// </summary>
        public string TimePattern { get; set; }

		/// <summary>
		/// The description of a Gasboy Event
		/// </summary>
		public string Description { get; set; }
	}
}
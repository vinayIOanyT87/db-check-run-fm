// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExternalStationDetailModel.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   The model for the External Station Detail Page
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.Afss.WebApp.Areas.AutomatedFuelStation.Models
{
    using System.Collections.Generic;

    using FMBusinessObjects.DataObjects;

    using FuelsManager.Afss.Module.Gasboy.BusinessObjects.DataObjects;

    /// <summary>
    /// The model for the External Station Detail Page
    /// </summary>
    public class GasboyStationDetailModel
    {
        /// <summary>
        /// Initialize reference types to avoid potential null reference errors should an error occur.
        /// </summary>
        public GasboyStationDetailModel()
        {
            this.ExternalStation = new GasboyStation();
            this.Products = new List<ProductClass>();
        }

        /// <summary>
        /// The External Station to display
        /// </summary>
        public GasboyStation ExternalStation { get; set; }

        /// <summary>
        /// A list of products configured for the site that we can display in the product mapping tab's grid
        /// </summary>
        public IEnumerable<ProductClass> Products { get; set; }

        /// <summary>
        /// True if the user has permission to edit the external station detail page
        /// </summary>
        public bool IsEditable { get; set; }

        /// <summary>
        /// The text provided in the confirm password box
        /// </summary>
        public string ConfirmPasswordText { get; set; }
    }
}
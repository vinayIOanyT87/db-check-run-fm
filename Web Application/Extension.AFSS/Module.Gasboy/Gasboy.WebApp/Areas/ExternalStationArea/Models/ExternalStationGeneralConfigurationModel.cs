// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExternalStationGeneralConfigurationModel.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//  The model for the External Station General Configuration Page
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.Afss.Module.Gasboy.WebApp.Areas.ExternalStationArea.Models
{
    using System.Collections.Generic;

    using FMBusinessObjects.DataObjects;

    using FuelsManager.Afss.Module.Gasboy.BusinessObjects.DataObjects;

    /// <summary>
    /// The model for the External Station General Configuration Page
    /// </summary>
    public class ExternalStationGeneralConfigurationModel
    {
        /// <summary>
        /// Create the general configuration model 
        /// </summary>
        public ExternalStationGeneralConfigurationModel()
        {
            this.GeneralConfiguration = new GasboyStationGeneralConfiguration();
            this.TransactionAliasNames = new List<TransactionAliasNameClass>();
        }

        /// <summary>
        /// The general configuration record to display on the page
        /// </summary>
        public GasboyStationGeneralConfiguration GeneralConfiguration { get; set; }

        /// <summary>
        /// A list of transaction aliases to display in the Retail Sale Transaction Alias drop down
        /// </summary>
        public List<TransactionAliasNameClass> TransactionAliasNames { get; set; }
    }
}
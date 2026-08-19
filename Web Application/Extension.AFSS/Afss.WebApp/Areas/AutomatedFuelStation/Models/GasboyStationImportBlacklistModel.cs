// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GasboyStationImportBlacklistModel.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   The model for the Gasboy Station Import Blacklist Page
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FuelsManager.Afss.WebApp.Areas.AutomatedFuelStation.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.Web;

    /// <summary>
    /// The model used by the Gasboy Station Import Blacklist screen
    /// </summary>
    public class GasboyStationImportBlacklistModel
    {
        /// <summary>
        /// The file we're uploading to import into the black list
        /// </summary>
        [Required]
        public HttpPostedFileBase File { get; set; }

        /// <summary>
        /// The results of uploading the file
        /// </summary>
        public string ImportResults { get; set; }
    }
}
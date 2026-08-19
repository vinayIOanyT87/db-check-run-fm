// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExternalStationDataImportModel.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   The model for the External Station Data Import Page
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FuelsManager.Afss.Module.Gasboy.WebApp.Areas.ExternalStationArea.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.Web;

    /// <summary>
    /// The model used by the External Station Data Import screen
    /// </summary>
    public class ExternalStationDataImportModel
    {
        /// <summary>
        /// The file we're uploading to import transactions from
        /// </summary>
        [Required]
        public HttpPostedFileBase File { get; set; }

        /// <summary>
        /// The results of uploading the file
        /// </summary>
        public string ImportResults { get; set; }
    }
}
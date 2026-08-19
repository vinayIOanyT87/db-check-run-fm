// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GasboyDeviceDetailModel.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   The model for the Gasboy Device Detail Page
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.Afss.WebApp.Areas.AutomatedFuelStation.Models
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web.Mvc;

	using FMBusinessObjects.Exceptions;

	using FuelsManager.Afss.Module.Gasboy.BusinessObjects.DataObjects;

    /// <summary>
    /// The model for the Gasboy Device Detail Page
    /// </summary>
    public class GasboyDeviceDetailModel
    {
        /// <summary>
        /// Initialize reference types to avoid potential null reference errors should an error occur.
        /// </summary>
        public GasboyDeviceDetailModel()
        {
            this.GasboyDevice = new GasboyDevice();
				this.Departments  = new List<GasboyDepartment>();
        }

        /// <summary>
        /// The Gasboy Device to display
        /// </summary>
        public GasboyDevice GasboyDevice { get; set; }

		/// <summary>
		/// A list of products configured for the site that we can display in the product mapping tab's grid
		/// </summary>
		public List<GasboyDepartment> Departments { get; set; }

		public String SelectedDepartment { get; set; }

	    public IEnumerable<SelectListItem> DepartmentList
	    {
		    get
		    {
			    return
				    this.Departments.Select(f => new SelectListItem { Value = f.IdentityGuid.ToString(), Text = f.DepartmentName, Selected = (f.IdentityGuid == this.GasboyDevice.DepartmentIdentityGuid ? true:false)});
			    //return from s in this.Departments
			    //       select new SelectListItem() { Text = s.DepartmentName, Value = s.IdentityGuid.ToString() };
		    } 
	    }

		/// <summary>
		/// True if the user has permission to edit the Gasboy Device detail page
		/// </summary>
		public bool IsEditable { get; set; }
    }
}
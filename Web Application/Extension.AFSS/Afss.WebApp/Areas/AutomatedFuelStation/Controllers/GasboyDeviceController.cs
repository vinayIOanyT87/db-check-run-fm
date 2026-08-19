// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GasboyDeviceController.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Controller for the Gasboy Device functionality. This includes the Gasboy Device Detail Page,
//   The Gasboy Device Summary Page, The Gasboy Device Operations Page, and the Gasboy Device Operations Page.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.Afss.WebApp.Areas.AutomatedFuelStation.Controllers
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.IO;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using System.Web;
	using System.Web.Mvc;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;
	using FMBusinessObjects.UtilityObjects;

	using BusinessObjects.Constants;

	using FuelsManager.Afss.Module.Gasboy.BusinessObjects.Constants;
	using FuelsManager.Areas.Controllers;
	using Module.Gasboy.BusinessObjects.BusinessInterfaces;
	using Module.Gasboy.BusinessObjects.ChannelFactories;
	using Module.Gasboy.BusinessObjects.DataObjects;
	using Module.Gasboy.BusinessObjects.ServiceProcessInterfaces;
	using Models;

	/// <summary>
	/// Controller for the Gasboy Device functionality. This includes the Gasboy Device Detail Page,
	/// The Gasboy Device Summary Page, The Gasboy Device Operations Page, The Gasboy Device Failed Transactions Page,
	/// and the Gasboy Device Operations Page.
	/// </summary>
	[RouteArea("AutomatedFuelStation")]
	[RoutePrefix("AutomatedFuelStation")]
	public class GasboyDeviceController : FMBaseController, IDataDictionary, IEntityDiscovery
	{
		/// <summary>
		/// The value of the Download Products button. This is used to determine if the Download Products button was pressed.
		/// </summary>
		public const string DownloadBlacklistButtonValue = "DownloadBlacklist";

		/// <summary>
		/// The maximum number of transactions you can attempt to download from the operations page
		/// </summary>
		private const long MaximumNumberOfTransactionsToDownload = 250;

		/// <summary>
		/// The type of file accepted by the data import screen
		/// </summary>
		private const string PermittedImportFileExtension = "txt";

		private string UploadDirectoryPath = "~/App_Data/GasboyBlacklist/";

		/// <summary>
		/// Contains data dictionary values for the form which should be translated
		/// </summary>
		/// <param name="security">Contains security information</param>
		/// <returns>Data dictionary values for the form which should be translated</returns>
		[NonAction]
		public new string[] Keys(SecurityClass security)
		{
			return new[]
				   {
					   "Payment Card Configuration",  //Changed from Gasboy Device to All Payment Card since we are only using the Gasboy devices for payment cards at this point.
					   "General",
					   "Card Number",
					   "OK",
					   "Cancel",
					   "Download Blacklist",
					   "Edit",
					   "Delete",
					   "Add",
					   "Import Blacklist",
					   "Payment Cards Configuration", //Changed from Gasboy Devices to All Payment Cards since we are only using the Gasboy devices for payment cards at this point.
					   "Refresh",
					   "Find Text",
					   "Select",
					   "Gasboy Device",
					   "Status",
					   "Import File",
					   "Import",
					   "Import Results",
					   "{All}",
					   "Date Range",
				   };
		}

		#region Gasboy Device Summary Page Actions

		/// <summary>
		/// Populates the model with the Gasboy Devices for the site and returns the view based on that model.
		/// </summary>
		/// <returns>A view with a model populated with Gasboy Devices for the site</returns>
		[HttpGet]
		public ActionResult GasboyDeviceSummaryIndex(Guid? id)
		{
			var model = new GasboyDeviceSummaryModel();

			try
			{
				model.IsEditable = this.Security.HasRight(RIGHT.VIEW_AUTOMATED_FUEL_SERVICE_STATION);
				model.SiteGuid = this.Security.SiteGuid;
				model.GasboyDevices = GasboyChannelHelper.MakeCall<IGasboyDevices, List<GasboyDevice>>(gasboyDevicesService => gasboyDevicesService.EnumerateAndFilter(this.Security, model.FindText));
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}

			return this.View(model);
		}

		/// <summary>
		/// The Post action for the Gasboy Device summary page, which handles things like deletes or filtering on the find text
		/// </summary>
		/// <param name="model">
		/// The model, which contains the find text provided by the user
		/// </param>
		/// <param name="deleteButton">
		/// The Guid of the Gasboy Device the user clicked delete for, if any
		/// </param>
		/// <returns>
		/// A view with a model populated with Gasboy Devices for the site after the delete or filter is applied
		/// </returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult GasboyDeviceSummaryIndex(GasboyDeviceSummaryModel model, Guid? deleteButton)
		{
			try
			{
				model.IsEditable = this.Security.HasRight(RIGHT.MODIFY_AUTOMATED_FUEL_SERVICE_STATION);
				model.SiteGuid = this.Security.SiteGuid;

				if (this.ModelState.IsValid)
				{
					if (deleteButton.HasValue)
					{
						GasboyChannelHelper.MakeCall<IGasboyDevices>(gasboyDevicesService => gasboyDevicesService.Purge(this.Security, deleteButton.Value));
					}

					model.GasboyDevices = GasboyChannelHelper.MakeCall<IGasboyDevices, List<GasboyDevice>>(gasboyDevicesService => gasboyDevicesService.EnumerateAndFilter(this.Security, model.FindText));
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}

			return this.View(model);
		}

		#endregion

		#region Gasboy Device Detail Page Actions

		/// <summary>
		/// Gets the Gasboy Device identified by the provided guid and returns a view of the station
		/// If the guid is empty, a new Gasboy Device will be created
		/// </summary>
		/// <param name="gasboyDeviceGuid">The Gasboy Device to get</param>
		/// <returns>A view of the gasboy device identified by gasboyDeviceGuid</returns>
		[HttpGet]
		public ActionResult GasboyDeviceDetail(Guid gasboyDeviceGuid)
		{
			GasboyDeviceDetailModel model = new GasboyDeviceDetailModel();

			try
			{



				// Create a new Gasboy Device if the guid is empty, otherwise load the Gasboy Device identified by the provided guid
				if (gasboyDeviceGuid == Guid.Empty)
				{
					model.GasboyDevice = new GasboyDevice { SiteGuid = this.Security.SiteGuid };
				}
				else
				{
					model.GasboyDevice = GasboyChannelHelper.MakeCall<IGasboyDevices, GasboyDevice>(
							gasboyDevicesService => gasboyDevicesService.Get(this.Security, gasboyDeviceGuid));
				}

				model.Departments = GasboyChannelHelper.MakeCall<IGasboyDepartments, GasboyDepartmentCollection>(
							gasboyDepartmentsService => gasboyDepartmentsService.Enumerate(this.Security));

				model.IsEditable = this.IsEditable(model.GasboyDevice.SiteGuid);


			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}

			return this.View(model);
		}

		/// <summary>
		/// The POST action for the Gasboy Device detail page. Depending on which button was pressed, 
		/// we either test the connection or add/modify the Gasboy Device
		/// </summary>
		/// <param name="model">Contains model information including the Gasboy Device</param>
		/// <param name="submitButton">Identifies the button that was pressed</param>
		/// <returns>A view of the Gasboy Device detail page or the Gasboy Device summary depending on which button was pressed and whether the save was successful</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult GasboyDeviceDetail(GasboyDeviceDetailModel model, string submitButton)
		{
			try
			{
				model.IsEditable = this.IsEditable(model.GasboyDevice.SiteGuid);

				if (this.ModelState.IsValid)
				{
					if (string.IsNullOrEmpty(model.GasboyDevice.CardNumber))
					{
						model.GasboyDevice.CardNumber = string.Empty;
						throw new Exception("The Device's Card Number must be specified.");
					}

					if (model.GasboyDevice.IdentityGuid == Guid.Empty)
					{
						GasboyChannelHelper.MakeCall<IGasboyDevices>(gasboyDevicesSvc => gasboyDevicesSvc.Add(this.Security, model.GasboyDevice));
					}
					else
					{

						model.GasboyDevice.DepartmentIdentityGuid = Guid.Parse(model.SelectedDepartment);

						GasboyChannelHelper.MakeCall<IGasboyDevices>(gasboyDevicesSvc => gasboyDevicesSvc.Modify(this.Security, model.GasboyDevice));
					}
				}
				else
				{
					string errors = HttpUtility.JavaScriptStringEncode(string.Join(Environment.NewLine, this.ModelState.Values.SelectMany(value => value.Errors.Select(error => error.ErrorMessage))));
					throw new Exception(errors);
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			  
				return this.View("GasboyDeviceDetail", model);
			}

			return this.RedirectToAction("GasboyDeviceSummaryIndex", "GasboyDevice");
		}

		#endregion Gasboy Device Summary Page Actions

		/// <summary>
		/// Check if the user has rights to modify Gasboy Devices and the Gasboy Device is owned by the current site
		/// </summary>
		/// <param name="gasboyDeviceSiteGuid">
		/// The Gasboy Device's owning site.
		/// </param>
		/// <returns>
		/// True if the user has rights to modify Gasboy Devices and the Gasboy Device is owned by the current site. False otherwise
		/// </returns>
		[NonAction]
		private bool IsEditable(Guid gasboyDeviceSiteGuid)
		{
			return this.Security.HasRight(RIGHT.MODIFY_AUTOMATED_FUEL_SERVICE_STATION) && this.Security.SiteGuid == gasboyDeviceSiteGuid;
		}

		#region Entity Assignment and Ownership Support

		/// <summary>
		/// Can you assign Gasboy Devices to sites other than the site which owns the Station? 
		/// No, you can't. A station should exist at one and only one site, otherwise we won't know
		/// which site to save transactions downloaded from the station in.
		/// </summary>
		bool IEntityDiscovery.EntityAssignable
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Get the service class which supports entity assignment for Gasboy Devices.
		/// This doesn't appear to be used in any meaningful way but must be implemented to satisfy IEntityDiscovery
		/// </summary>
		Type IEntityDiscovery.EntityEngineType
		{
			get
			{
				return typeof(IGasboyDevices);
			}
		}

		/// <summary>
		/// The type of entity we are supporting entity assignment for (Gasboy Devices)
		/// </summary>
		ENTITY_TYPE IEntityDiscovery.EntityType
		{
			get
			{
				return ENTITY_TYPE.EXTERNAL_STATION_DEVICE;
			}
		}

		/// <summary>
		/// Enumerate entity maps for Gasboy Devices. This is used by the entity ownership form to show Gasboy Devices
		/// owned by the site.
		/// </summary>
		/// <param name="security">Contains security information.</param>
		/// <param name="type">The type of entity assignment to enumerate. For Gasboy Devices this appears to only be OWNED, which is 
		/// used by the entity ownership form</param>
		/// <returns>Entity to site mappings for Gasboy Devices depending on the type provided.</returns>
		EntityToSiteMapCollectionClass IEntityDiscovery.EnumerateEntityMaps(SecurityClass security, ENTITY_ASSIGNMENT_TYPE type)
		{
			List<GasboyDevice> gasboyDevices = GasboyChannelHelper.MakeCall<IGasboyDevices, List<GasboyDevice>>(
																	 gasboyDevicesService =>
																	 gasboyDevicesService.Enumerate(security));

			EntityToSiteMapCollectionClass entityToSiteMapCollection = new EntityToSiteMapCollectionClass();

			if (!security.HasRight(RIGHT.MODIFY_AUTOMATED_FUEL_SERVICE_STATION))
			{
				return entityToSiteMapCollection;
			}

			if (type == ENTITY_ASSIGNMENT_TYPE.OWNED)
			{
			}
			else
			{
				EntityToSiteMapClass entityToSiteMap =
					FMChannelHelper.MakeCall<IEntityToSiteMaps, EntityToSiteMapClass>(
						x => x.Get(this.Security, ((IEntityDiscovery)this).EntityType, this.Security.LoginSiteGuid));

				if (type == ENTITY_ASSIGNMENT_TYPE.ASSIGNED)
				{
					if (this.Security.LoginSiteGuid == entityToSiteMap.IdentityGuid)
					{
                        entityToSiteMap.ID = "All Payment Cards"; //Changed from Gasboy Devices to All Payment Cards since we are only using the Gasboy devices for payment cards at this point.
						entityToSiteMapCollection.Add(entityToSiteMap);
					}
				}
				else
				{
					if (entityToSiteMap.IdentityGuid == Guid.Empty)
					{
						entityToSiteMap = new EntityToSiteMapClass
										{
											SiteGuid = Guid.Empty,
                                            ID = "All Payment Cards", //Changed from Gasboy Devices to All Payment Cards since we are only using the Gasboy devices for payment cards at this point.
											TypeID = ((IEntityDiscovery)this).EntityType,
											IdentityGuid = this.Security.SiteGuid
										};

						entityToSiteMapCollection.Add(entityToSiteMap);
					}
				}
			}
			return entityToSiteMapCollection;
		}

		/// <summary>
		/// Get the primary key (aka Identity Guid) of the Gasboy Device matching the provided ID
		/// </summary>
		/// <param name="security">Contains Security Information</param>
		/// <param name="id">Identifies the Gasboy Device to retrieve</param>
		/// <returns>The primary key (aka Identity Guid) of the Gasboy Device matching the provided ID</returns>
		Guid IEntityDiscovery.GetIdentityGuid(SecurityClass security, string id)
		{
			return GasboyChannelHelper.MakeCall<IGasboyDevices, Guid>(gasboyDevicesService => gasboyDevicesService.GetIdentityGuid(security, null, id));
		}

		/// <summary>
		/// Modify the provided Gasboy Device's siteGuid. This is used for entity ownership changes.
		/// </summary>
		/// <param name="security">Contains Security Information</param>
		/// <param name="guid">Identifies the Gasboy Device we want to modify</param>
		/// <param name="siteGuid">Identifies the site the Gasboy Device should be owned by</param>
		void IEntityDiscovery.SetSiteGuid(SecurityClass security, Guid guid, Guid siteGuid)
		{
			GasboyDevice gasboyDevice = GasboyChannelHelper.MakeCall<IGasboyDevices, GasboyDevice>(gasboyDevicesService => gasboyDevicesService.Get(security, guid));

			gasboyDevice.SiteGuid = siteGuid;
			GasboyChannelHelper.MakeCall<IGasboyDevices>(gasboyDevicesService => gasboyDevicesService.Modify(security, gasboyDevice));
		}

		#endregion Entity Assignment and Ownership Support
	}

	/// <summary>
	/// Contains the search parameters last used by the user when searching from the Gasboy Device Summary page
	/// </summary>
	[Serializable]
	public class GasboyDeviceSummarySearchParameters
	{
		/// <summary>
		/// The beginning date / time of the date range search parameter
		/// </summary>
		public DateTimeOffset BeginDateTime { get; set; }

		/// <summary>
		/// The ending date / time of the date range search parameter
		/// </summary>
		public DateTimeOffset EndDateTime { get; set; }

		/// <summary>
		/// The card number search parameter value
		/// </summary>
		public string CardNumber { get; set; }
	 
		/// <summary>
		/// The Station to display failed transactions for. An empty value indicates all stations
		/// </summary>
		public Guid GasboyStationGuid { get; set; }
	}
}

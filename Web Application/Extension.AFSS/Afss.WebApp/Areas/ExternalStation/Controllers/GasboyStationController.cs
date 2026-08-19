// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExternalStationController.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Controller for the External Station functionality. This includes the External Station Detail Page,
//   The External Station Summary Page, The External Station Operations Page, and the External Station Operations Page.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.Afss.WebApp.Areas.ExternalStation.Controllers
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.IO;
	using System.Linq;
	using System.Threading.Tasks;
	using System.Web;
	using System.Web.Mvc;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;
	using FMBusinessObjects.UtilityObjects;

	using BusinessObjects.Constants;
	using FuelsManager.Areas.Controllers;
	using Module.Gasboy.BusinessObjects.BusinessInterfaces;
	using Module.Gasboy.BusinessObjects.ChannelFactories;
	using Module.Gasboy.BusinessObjects.DataObjects;
	using Module.Gasboy.BusinessObjects.ServiceProcessInterfaces;
	using Models;

	/// <summary>
	/// Controller for the External Station functionality. This includes the External Station Detail Page,
	/// The External Station Summary Page, The External Station Operations Page, The External Station Failed Transactions Page,
	/// and the External Station Operations Page.
	/// </summary>
	[RouteArea("ExternalStationArea")]
	[RoutePrefix("ExternalStation")]
	public class GasboyStationController : FMBaseController, IDataDictionary, IEntityDiscovery
	{
		/// <summary>
		/// The value of the Test Connection button. This is used to determine if the Test Connection button was pressed.
		/// </summary>
		public const string TestConnectionButtonValue = "TestConnection";

		/// <summary>
		/// The value of the Download Products button. This is used to determine if the Download Products button was pressed.
		/// </summary>
		public const string DownloadProductsButtonValue = "DownloadProducts";

		/// <summary>
		/// The maximum number of transactions you can attempt to download from the operations page
		/// </summary>
		private const long MaximumNumberOfTransactionsToDownload = 250;

		/// <summary>
		/// The type of file accepted by the data import screen
		/// </summary>
		private const string PermittedImportFileExtension = "csv";

		/// <summary>
		/// The session key used to store the user's last search parameters on the failed transaction summary form.
		/// </summary>
		private const string FailedTransactionSummarySearchParametersSessionKey = "GasboyStationFailedTransactionSummarySearchParameters";

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
					   "Gasboy Fuel Service Station Configuration",
					   "General",
					   "Product Mapping",
					   "OK",
					   "Cancel",
					   "Test Connection",
					   "Edit",
					   "Delete",
					   "Add",
					   "Download Products from Station",
					   "ID",
					   "Billing ID",
					   "IP Address",
					   "Site Code",
					   "User Name",
					   "Password",
					   "Confirm Password",
					   "Download Transactions Automatically",
					   "Station Product ID",
					   "FuelsManager Product",
					   "Gasboy Fuel Service Stations Configuration",
					   "Refresh",
					   "Find Text",
					   "Select",
					   "Gasboy Station",
					   "Status",
					   "Last Successful Connection",
					   "Last Connection Attempt",
					   "Last Transaction ID",
					   "Gasboy Fuel Service Station Operations",
					   "Specify Transaction ID Range",
					   "Transaction IDs",
					   "to",
					   "Download Transactions",
					   "Download Events",
					   "Gasboy Fuel Service Station Data Import",
					   "Import File",
					   "Import",
					   "Import Results",
					   "Gasboy Fuel Service Station Failed Transactions",
					   "Transaction ID",
					   "Receive Date",
					   "{All}",
					   "Date Range",
					   "Gasboy Fuel Service Station Failed Transaction",
					   "Fleet ID",
					   "Fleet Name",
					   "Fleet Code",
					   "Product Name",
					   "Product Code",
					   "Mean ID",
					   "Mean Name",
					   "Fueling Vehicle Plate",
					   "Driver Mean ID",
					   "Driver Plate",
					   "Driver Tag",
					   "External Authorization #",
					   "Density",
					   "Temperature",
					   "Engine Hours",
					   "Pump ID",
					   "Pump",
					   "Nozzle ID",
					   "Nozzle",
					   "Pump ID",
					   "Pump",
					   "Nozzle ID",
					   "Nozzle",
					   "Hose Number",
					   "Tank Name",
					   "Shift ID",
					   "Odometer",
					   "Quantity",
					   "Price Per Volume",
					   "Total Price",
					   "Proxy Device ID",
					   "Transaction Timestamp",
					   "Transaction Type",
					   "Track Data 1",
					   "Track Data 2",
					   "Tag",
					   "Cash Customer ID",
					   "Error",
					   "Error Messages",
					   "Gasboy Fuel Service Station General Configuration",
					   "Retail Sale Transaction Alias",
					   "Transaction Download Interval (minutes)",
					   "Event Download Interval (minutes)",
				   };
		}

		#region External Station Summary Page Actions

		/// <summary>
		/// Populates the model with the external stations for the site and returns the view based on that model.
		/// </summary>
		/// <returns>A view with a model populated with external stations for the site</returns>
		[HttpGet]
		public ActionResult ExternalStationSummaryIndex()
		{
			var model = new GasboyStationSummaryModel();

			try
			{
				model.IsEditable = this.Security.HasRight(RIGHT.MODIFY_EXTERNAL_STATION);
				model.SiteGuid = this.Security.SiteGuid;
				model.ExternalStations = GasboyChannelHelper.MakeCall<IGasboyStations, List<GasboyStation>>(externalStationsService => externalStationsService.EnumerateAndFilter(this.Security, model.FindText));
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}

			return this.View(model);
		}

		/// <summary>
		/// The Post action for the external station summary page, which handles things like deletes or filtering on the find text
		/// </summary>
		/// <param name="model">
		/// The model, which contains the find text provided by the user
		/// </param>
		/// <param name="deleteButton">
		/// The Guid of the external station the user clicked delete for, if any
		/// </param>
		/// <returns>
		/// A view with a model populated with external stations for the site after the delete or filter is applied
		/// </returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult GasboyStationSummaryIndex(GasboyStationSummaryModel model, Guid? deleteButton)
		{
			try
			{
				model.IsEditable = this.Security.HasRight(RIGHT.MODIFY_EXTERNAL_STATION);
				model.SiteGuid = this.Security.SiteGuid;

				if (this.ModelState.IsValid)
				{
					if (deleteButton.HasValue)
					{
						GasboyChannelHelper.MakeCall<IGasboyStations>(externalStationsService => externalStationsService.Purge(this.Security, deleteButton.Value));
					}

					model.ExternalStations = GasboyChannelHelper.MakeCall<IGasboyStations, List<GasboyStation>>(externalStationsService => externalStationsService.EnumerateAndFilter(this.Security, model.FindText));
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}

			return this.View(model);
		}

		#endregion

		#region External Station Detail Page Actions

		/// <summary>
		/// Gets the external station identified by the provided guid and returns a view of the station
		/// If the guid is empty, a new external station will be created
		/// </summary>
		/// <param name="externalStationGuid">The external station to get</param>
		/// <returns>A view of the station identified by externalStationGuid</returns>
		[HttpGet]
		public ActionResult GasboyStation(Guid externalStationGuid)
		{
			GasboyStationDetailModel model = new GasboyStationDetailModel();

			try
			{
				// Get products to display in the product drop down in the product mapping grid
				model.Products = FMChannelHelper.MakeCall<IProducts, ProductCollectionClass>(products => products.Enumerate(this.Security)); 

				// Create a new external station if the guid is empty, otherwise load the external station identified by the provided guid
				if (externalStationGuid == Guid.Empty)
				{
					model.ExternalStation = new GasboyStation { SiteGuid = this.Security.SiteGuid };
				}
				else
				{
					model.ExternalStation = GasboyChannelHelper.MakeCall<IGasboyStations, GasboyStation>(
							externalStationsService => externalStationsService.Get(this.Security, externalStationGuid));
				}

				// We want to show the users something in the password boxes if there is currently a password configured for the station.
				// However, we don't want to show the actual password for security reasons. So if there is a password, so some dummy text.
				// If the dummy text is submitted to the service class as the password the password will not be changed.
				if (!string.IsNullOrEmpty(model.ExternalStation.Password))
				{
					model.ExternalStation.Password = Module.Gasboy.BusinessObjects.DataObjects.GasboyStation.PasswordDefaultValue;
				}

				model.IsEditable = this.IsEditable(model.ExternalStation.SiteGuid);
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}

			return this.View(model);
		}

		/// <summary>
		/// The POST action for the External Station detail page. Depending on which button was pressed, 
		/// we either test the connection or add/modify the external station
		/// </summary>
		/// <param name="model">Contains model information including the External Station</param>
		/// <param name="submitButton">Identifies the button that was pressed</param>
		/// <returns>A view of the external station detail page or the external station summary depending on which button was pressed and whether the save was successful</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult GasboyStation(GasboyStationDetailModel model, string submitButton)
		{
			try
			{
				model.IsEditable = this.IsEditable(model.ExternalStation.SiteGuid);

				// Get products to display in the product drop down in the product mapping grid
				model.Products = FMChannelHelper.MakeCall<IProducts, ProductCollectionClass>(products => products.Enumerate(this.Security)); 

				// If the user pressed the test connection button, test the connection to the station with the provided information
				if (submitButton == TestConnectionButtonValue)
				{
					if (model.ExternalStation.Password != model.ConfirmPasswordText)
					{
						// Reset the password to the empty string when the passwords don't match, otherwise 
						// the screen will display the error message and then make the confirm password text match the password.
						model.ExternalStation.Password = string.Empty;
						model.ConfirmPasswordText = string.Empty;
						throw new Exception("The same password must be provided in the password and confirm password fields.");
					}

					string result;
					bool resetPassword = false;
			   
					// If the user hasn't modified the password, the value in the model will be masked. We need to send a real password to test connection, 
					// so if the password is masked, re-get the station from the DB so we can send a real password
					if (model.ExternalStation.Password == Module.Gasboy.BusinessObjects.DataObjects.GasboyStation.PasswordDefaultValue
						&& model.ExternalStation.IdentityGuid != Guid.Empty)
					{
						resetPassword = true;

						GasboyStation station = GasboyChannelHelper.MakeCall<IGasboyStations, GasboyStation>(
							externalStationsService =>
							externalStationsService.Get(this.Security, model.ExternalStation.IdentityGuid));

						model.ExternalStation.Password = station.Password;
					}

					try
					{
						result =
							GasboyManagerChannelHelper.MakeCall<IGasboyStationServices, string>(
								externalStationsService =>
								externalStationsService.TestConnection(this.Security, model.ExternalStation));
					}
					finally
					{
						if (resetPassword)
						{
							model.ExternalStation.Password = Module.Gasboy.BusinessObjects.DataObjects.GasboyStation.PasswordDefaultValue;
						}
					}

					this.ViewBag.TestConnectionResult = HttpUtility.JavaScriptStringEncode(result);
				   
					return this.View("GasboyStation", model);
				}             

				if (this.ModelState.IsValid)
				{
					if (model.ExternalStation.Password != model.ConfirmPasswordText)
					{
						// Reset the password to the empty string when the passwords don't match, otherwise 
						// the screen will display the error message and then make the confirm password text match the password.
						model.ExternalStation.Password = string.Empty;
						model.ConfirmPasswordText = string.Empty;
						throw new Exception("The same password must be provided in the password and confirm password fields.");
					}

					if (submitButton == DownloadProductsButtonValue)
					{
						// Make sure that we've checked that the model is valid before adding any new product mappings.
						// If we don't, and the user adds a product mapping with no products specified,
						// We'll get a null reference error when attempting to determine if the mapping already exists.
						this.UpdateProductMappingsUsingStationData(model);
						return this.View("GasboyStation", model);
					}

					if (model.ExternalStation.IdentityGuid == Guid.Empty)
					{
						GasboyChannelHelper.MakeCall<IGasboyStations>(externalStationsService => externalStationsService.Add(this.Security, model.ExternalStation));
					}
					else
					{
						GasboyChannelHelper.MakeCall<IGasboyStations>(externalStationsService => externalStationsService.Modify(this.Security, model.ExternalStation));
					}
				}
				else
				{
					string errors = HttpUtility.JavaScriptStringEncode(string.Join(Environment.NewLine, ModelState.Values.SelectMany(value => value.Errors.Select(error => error.ErrorMessage))));
					throw new Exception(errors);
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			  
				return this.View("GasboyStation", model);
			}

			return this.RedirectToAction("GasboyStationSummaryIndex", "GasboyStation");
		}

		/// <summary>
		/// Get a partial view representing a new table row in the product mapping table to support adding a record to the grid.
		/// The use of the OutputCache attribute is to ensure that this view is not cached. If a cached view is returned, 
		/// the list binding of the grid will be broken and will result in product mappings being duplicated
		/// </summary>
		/// <returns>A partial view representing a new table row in the product mapping table to support adding a record to the grid</returns>
		[HttpGet]
		[OutputCache(VaryByParam = "*", Duration = 0, NoStore = true)] 
		public ActionResult AddNewRow()
		{
			// Because there's a drop down list in the row populated with products, we have to get the products so the drop down can use them
			this.ViewBag.products = FMChannelHelper.MakeCall<IProducts, ProductCollectionClass>(products => products.Enumerate(this.Security));

			// When returning the partial view, we have to populate the identityGuid and ExternalStationGuid to avoid field validation errors when the user presses OK.
			return this.PartialView("EditorTemplates/GasboyStationProductMapping", new GasboyStationProductMapping { IdentityGuid = Guid.Empty, ExternalStationGuid = Guid.Empty });
		}

		/// <summary>
		/// Request products from the station and add them to the product mapping grid unless they are
		/// already mapped.
		/// </summary>
		/// <param name="model">The model for the external station detail screen to add products to</param>
		private void UpdateProductMappingsUsingStationData(GasboyStationDetailModel model)
		{
			// Make sure the product mapping tab displays when the screen is reloaded.
			this.ViewBag.ActiveTabIndex = 1;

			bool resetPassword = false;

			// If the user hasn't modified the password, the value in the model will be masked. We need to send a real password, 
			// so if the password is masked, re-get the station from the DB so we can send a real password
			if (model.ExternalStation.Password == Module.Gasboy.BusinessObjects.DataObjects.GasboyStation.PasswordDefaultValue
				&& model.ExternalStation.IdentityGuid != Guid.Empty)
			{
				resetPassword = true;

				GasboyStation station = GasboyChannelHelper.MakeCall<IGasboyStations, GasboyStation>(
					externalStationsService =>
					externalStationsService.Get(this.Security, model.ExternalStation.IdentityGuid));

				model.ExternalStation.Password = station.Password;
			}

			List<GasboyStationProduct> stationProducts;

			try
			{
				stationProducts =
					GasboyManagerChannelHelper.MakeCall<IGasboyStationServices, List<GasboyStationProduct>>(
						externalStationsService =>
						externalStationsService.GetStationProductList(this.Security, model.ExternalStation));
			}
			finally
			{
				if (resetPassword)
				{
					model.ExternalStation.Password = Module.Gasboy.BusinessObjects.DataObjects.GasboyStation.PasswordDefaultValue;
				}
			}

			if (stationProducts != null)
			{
				foreach (GasboyStationProduct stationProduct in stationProducts)
				{
					// Make sure that the product isn't already mapped - otherwise we'd be adding a duplicate
					if (model.ExternalStation.ProductMappings.Find(productMapping => productMapping.ID.Equals(stationProduct.Name, StringComparison.InvariantCultureIgnoreCase)) == null)
					{
						model.ExternalStation.ProductMappings.Add(
							new GasboyStationProductMapping
								{
									ID = stationProduct.Name,
									IdentityGuid = Guid.Empty,
									ExternalStationGuid = Guid.Empty
								});
					}
				}
			}
		}

		#endregion

		#region External Station Data Import Page Actions

		/// <summary>
		/// Get a view of the External Station Data Import page
		/// </summary>
		/// <returns>A view of the External Station Data Import page</returns>
		[HttpGet]
		public ActionResult GasboyStationDataImport()
		{
			var model = new GasboyStationDataImportModel();
			return this.View(model);
		}

		/// <summary>
		/// The post action for the External Station Data Import page. 
		/// </summary>
		/// <param name="model">Contains the file to import</param>
		/// <returns>A view representing the External Station Data Import page with the results of the import</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult GasboyStationDataImport(GasboyStationDataImportModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					if (!model.File.FileName.EndsWith(PermittedImportFileExtension, StringComparison.InvariantCultureIgnoreCase))
					{
						throw new Exception("The file to import must be a " + PermittedImportFileExtension + " file");
					}

					using (MemoryStream memoryStream = new MemoryStream())
					{
						model.File.InputStream.CopyTo(memoryStream);
					}

					model.ImportResults = "Import Successful!";
				}
				else
				{
					string errors = HttpUtility.JavaScriptStringEncode(string.Join(Environment.NewLine, ModelState.Values.SelectMany(value => value.Errors.Select(error => error.ErrorMessage))));
					throw new Exception(errors);
				}
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}

			return this.View(model);
		}

		#endregion

		#region External Station Operations Page Actions

		/// <summary>
		/// Gets a view representing the External Station Operations Page
		/// </summary>
		/// <returns>A view representing the External Station Operations Page</returns>
		[HttpGet]
		public ActionResult GasboyStationOperationsIndex()
		{
			var model = new GasboyStationOperationsModel();

			try
			{
				model.IsEditable = this.Security.HasRight(RIGHT.MODIFY_EXTERNAL_STATION);
				this.GetDateFormatInformation(model);
				model.ExternalStations = GasboyChannelHelper.MakeCall<IGasboyStations, List<GasboyStation>>(externalStationsService => externalStationsService.Enumerate(this.Security));
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		  
			return this.View(model);
		}

		/// <summary>
		/// The post action for the external station operations page. This handles things like the user pressing refresh or initiating a download request
		/// </summary>
		/// <param name="model">Cotanins information displayed on the screen</param>
		/// <param name="submitButton">The button the user pressed</param>
		/// <returns>If a download was requested, the model along with the results of the download. Otherwise, a refreshed model</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult GasboyStationOperationsIndex(GasboyStationOperationsModel model, string submitButton)
		{
			try
			{
				// ModelState.IsValid is not checked here - binding to a list of stations is a bit of a pain, and if you don't do that 
				// then you'll get errors about required fields not being present when you check ModelState.IsValid
				model.IsEditable = this.Security.HasRight(RIGHT.MODIFY_EXTERNAL_STATION);

				if (!string.IsNullOrEmpty(submitButton) && submitButton.Equals("DownloadTransactions"))
				{
					this.ViewBag.DownloadRequestResult = HttpUtility.JavaScriptStringEncode(this.InitiateTransactionDownloadRequest(model));

					return this.View(model);
				}
				else if (!string.IsNullOrEmpty(submitButton) && submitButton.Equals("DownloadEvents"))
				{
					this.ViewBag.DownloadRequestResult = HttpUtility.JavaScriptStringEncode(this.InitiateEventDownloadRequest(model));

					return this.View(model);
				}

				model.ExternalStations = GasboyChannelHelper.MakeCall<IGasboyStations, List<GasboyStation>>(externalStationsService => externalStationsService.Enumerate(this.Security));
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}

			return this.View(model);
		}

		#endregion

		#region Failed Transactions Actions

		/// <summary>
		/// Gets a view representing the External Station Failed Transactions Summary Page
		/// </summary>
		/// <returns>A view representing the External Station Failed Transactions Summary Page</returns>
		[HttpGet]
		public ActionResult GasboyStationFailedTransactionSummaryIndex()
		{
			var model = new GasboyStationFailedTransactionSummaryModel();

			try
			{
				SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(sites => sites.Get(
					this.Security,
					this.Security.SiteGuid,
					getMemberSites: false,
					getSchedulesAndProcessVariables: false,
					bGetAssociatedAliases: false));

				model.IsEditable = this.Security.HasRight(RIGHT.MODIFY_EXTERNAL_STATION);

				model.ExternalStations = GasboyChannelHelper.MakeCall<IGasboyStations, List<GasboyStation>>(externalStationsService => externalStationsService.Enumerate(this.Security));

				model.ShortDatePattern = site.ShortDatePattern;
				model.TimePattern = site.TimePattern;

				// Get the search parameters last used by the user on this screen from Session
				GasboyStationFailedTransactionSummarySearchParameters searchParameters = this.Session[FailedTransactionSummarySearchParametersSessionKey] as GasboyStationFailedTransactionSummarySearchParameters;
				
				// If there are search parameters stored in Session, use them
				if (searchParameters != null)
				{
					DateTimeOffset sessionBeginDate = searchParameters.BeginDateTime;
					model.BeginDate = sessionBeginDate.Date.ToString(site.ShortDatePattern);
					model.BeginTime = sessionBeginDate.ToString(site.TimePattern);

					DateTimeOffset sessionEndDate = searchParameters.EndDateTime;
					model.EndDate = sessionEndDate.Date.ToString(site.ShortDatePattern);
					model.EndTime = sessionEndDate.ToString(site.TimePattern);

					model.TransactionID = searchParameters.TransactionID;

					// Populate the selected external station, but only if it exists in the collection of stations. Keep in mind that the site may change
					// and along with it the stations configured for the site.
					if (searchParameters.ExternalStationGuid != Guid.Empty
						&& model.ExternalStations.ToList().Find(externalStation => externalStation.IdentityGuid == searchParameters.ExternalStationGuid) != null)
					{
						model.SelectedExternalStationGuid = searchParameters.ExternalStationGuid;
					}
				}
				else
				{
					var converter = new SiteTimeConverter(site);
					string todaysDate = converter.Today().Date.ToString(site.ShortDatePattern);
					model.BeginDate = converter.Today().Date.Subtract(new TimeSpan(30, 0, 0, 0)).ToString(site.ShortDatePattern);
					model.BeginTime = TimeConverter.MinFMTime.ToString(site.TimePattern);
					model.EndDate = todaysDate;
					model.EndTime = TimeConverter.MaxFMTime.ToString(site.TimePattern); 
				}            

				model.FailedTransactions = this.GetFailedTransactions(model);
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}

			return this.View(model);
		}

		/// <summary>
		/// The post method for the external station failed transaction screen, which handles the Refresh operation for the screen
		/// </summary>
		/// <param name="model">The model being displayed on the screen</param>
		/// <param name="deleteButton">If provided, identifies the external station transaction to delete</param>
		/// <returns>A refreshed view of failed transactions</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult GasboyStationFailedTransactionSummaryIndex(GasboyStationFailedTransactionSummaryModel model, Guid? deleteButton)
		{
			try
			{
				model.IsEditable = this.Security.HasRight(RIGHT.MODIFY_EXTERNAL_STATION);
		   
				if (deleteButton.HasValue)
				{
					var transaction =
						GasboyChannelHelper.MakeCall<IGasboyTransactionProcessor, GasboyStationTransaction>(
							svc => svc.GetFailedTransaction(this.Security, deleteButton.Value));
					
					transaction.ExternalStationTransactionFailedStatus =
						ExternalStationTransactionFailedStatus.Suppressed;

					var transactions = new List<GasboyStationTransaction>() { transaction };

					GasboyChannelHelper.MakeCall<IGasboyTransactionProcessor>(
						svc =>
						svc.UpdateTransactionFailedStatuses(
							this.Security,
							transactions));
				}

				model.ExternalStations = GasboyChannelHelper.MakeCall<IGasboyStations, List<GasboyStation>>(externalStationsService => externalStationsService.Enumerate(this.Security));
				model.FailedTransactions = this.GetFailedTransactions(model);
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}

			return this.View(model);
		}

		/// <summary>
		/// Get a view of the failed transaction identified by the provided guid
		/// </summary>
		/// <param name="externalStationFailedTransactionGuid">Identifies the failed transaction to display</param>
		/// <returns>A view of the failed transaction identified by the provided guid</returns>
		[HttpGet]
		public ActionResult GasboyStationFailedTransaction(Guid externalStationFailedTransactionGuid)
		{
			var model = new GasboyStationFailedTransactionModel();

			try
			{
				model.FailedTransaction = GasboyChannelHelper.MakeCall<IGasboyTransactionProcessor, GasboyStationTransaction>(svc => svc.GetFailedTransaction(this.Security, externalStationFailedTransactionGuid));
				model.IsEditable = this.IsEditable(model.FailedTransaction.SiteGuid);
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}

			return this.View(model);
		}

		/// <summary>
		/// The POST action for the External Station Failed transaction detail page. We try to resubmit the transaction to FuelsManager with corrected data
		/// </summary>
		/// <param name="model">Contains model information including the failed transaction</param>
		/// <returns>If reprocessing the transaction was successful, a view of the failed transaction summary page. Otherwise, a view of the failed transaction detail page</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult GasboyStationFailedTransaction(GasboyStationFailedTransactionModel model)
		{
			try
			{
				model.IsEditable = this.IsEditable(model.FailedTransaction.SiteGuid);

				if (this.ModelState.IsValid)
				{
					string result = GasboyChannelHelper.MakeCall<IGasboyTransactionProcessor, string>(svc => svc.ProcessCorrectedTransaction(this.Security, model.FailedTransaction));
					if (!string.IsNullOrEmpty(result))
					{
						this.ViewBag.SaveTransactionErrors = HttpUtility.JavaScriptStringEncode(result);
						return this.View("GasboyStationFailedTransaction", model);
					}
				}
				else
				{
					string errors = HttpUtility.JavaScriptStringEncode(string.Join(Environment.NewLine, ModelState.Values.SelectMany(value => value.Errors.Select(error => error.ErrorMessage))));
					throw new Exception(errors);
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);

				return this.View("GasboyStationFailedTransaction", model);
			}

			return this.RedirectToAction("GasboyStationFailedTransactionSummaryIndex", "GasboyStation");
		}

		/// <summary>
		/// Retrieve failed transactions from the database using the search values provided in the model
		/// </summary>
		/// <param name="model">Contains values to filter the failed transactions on</param>
		/// <returns>Failed transactions matching the search parameters provided</returns>
		[NonAction]
		private List<GasboyStationTransaction> GetFailedTransactions(GasboyStationFailedTransactionSummaryModel model)
		{
			DateTimeOffset beginDate;
			DateTimeOffset endDate;

			SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(sites => sites.Get(
						this.Security,
						this.Security.SiteGuid,
						getMemberSites: false,
						getSchedulesAndProcessVariables: false,
						bGetAssociatedAliases: false));

			// Make sure the dates and times are provided. If only the date or only the time is provided, TryParse will actually succeed
			// but will use today's date or midnight, which might not be intuitive.
			if (string.IsNullOrEmpty(model.BeginDate))
			{
				throw new Exception("Begin Date must be provided");
			}

			if (string.IsNullOrEmpty(model.BeginTime))
			{
				throw new Exception("Begin Time must be provided");
			}

			if (string.IsNullOrEmpty(model.EndDate))
			{
				throw new Exception("End Date must be provided");
			}

			if (string.IsNullOrEmpty(model.EndTime))
			{
				throw new Exception("End Time must be provided");
			}

			if (!DateTimeOffset.TryParse(model.BeginDate + " " + model.BeginTime, site.GetDateTimeFormatInfo(), DateTimeStyles.None, out beginDate))
			{
				throw new Exception("Begin Date must be a valid date and time");
			}

			if (!DateTimeOffset.TryParse(model.EndDate + " " + model.EndTime, site.GetDateTimeFormatInfo(), DateTimeStyles.None, out endDate))
			{
				throw new Exception("End Date must be a valid date and time");
			}

			if (beginDate > endDate)
			{
				throw new Exception("The Ending Date and Time must be greater than or equal to the Beginning Date and Time");
			}

			this.Session[FailedTransactionSummarySearchParametersSessionKey] = new GasboyStationFailedTransactionSummarySearchParameters
			{
				BeginDateTime = beginDate,
				EndDateTime = endDate,
				ExternalStationGuid = model.SelectedExternalStationGuid,
				TransactionID = model.TransactionID
			};

			return GasboyChannelHelper.MakeCall<IGasboyTransactionProcessor, List<GasboyStationTransaction>>(
					svc =>
					svc.EnumerateFailedTransactions(
						this.Security,
						model.SelectedExternalStationGuid,
						beginDate,
						endDate,
						model.TransactionID));
		}

		#endregion

		/// <summary>
		/// Check if the user has rights to modify external stations and the external station is owned by the current site
		/// </summary>
		/// <param name="externalStationSiteGuid">
		/// The external station's owning site.
		/// </param>
		/// <returns>
		/// True if the user has rights to modify external stations and the external station is owned by the current site. False otherwise
		/// </returns>
		[NonAction]
		private bool IsEditable(Guid externalStationSiteGuid)
		{
			return this.Security.HasRight(RIGHT.MODIFY_EXTERNAL_STATION) && this.Security.SiteGuid == externalStationSiteGuid;
		}

		/// <summary>
		/// Get information needed to format dates according to the site configuration
		/// </summary>
		/// <param name="model">The model which contains date fields.</param>
		[NonAction]
		private void GetDateFormatInformation(GasboyStationOperationsModel model)
		{
			SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(sites => sites.Get(
					this.Security,
					this.Security.SiteGuid,
					getMemberSites: false,
					getSchedulesAndProcessVariables: false,
					bGetAssociatedAliases: false));

			model.ShortDatePattern = site.ShortDatePattern;
			model.TimePattern = site.TimePattern;
		}

		/// <summary>
		/// Attempt to download transactions from the selected external stations
		/// </summary>
		/// <param name="model">
		/// Indicates which external stations are selected and the transaction ID range to use, if any
		/// </param>
		/// <returns>
		/// A string with the results of the download for each of the stations
		/// </returns>
		[NonAction]
		private string InitiateTransactionDownloadRequest(GasboyStationOperationsModel model)
		{
			if (model.ExternalStations == null
				|| model.ExternalStations.FindIndex(externalStation => externalStation.IsSelected) < 0)
			{
				throw new Exception("You must select at least one External Station to initiate a download request for");
			}

			List<Guid> selectedExternalStations = model.ExternalStations.FindAll(externalStation => externalStation.IsSelected).Select(selectedExternalStation => selectedExternalStation.IdentityGuid).ToList();

			if (model.TransactionIDRangeEnd.HasValue && !model.TransactionIDRangeStart.HasValue)
			{
				throw new Exception("If the ending transaction id range is provided the beginning must be provided as well");
			}

			if (!model.TransactionIDRangeEnd.HasValue && model.TransactionIDRangeStart.HasValue)
			{
				throw new Exception("If the beginning transaction id range is provided the ending must be provided as well");
			}

			if (model.TransactionIDRangeEnd.HasValue && model.TransactionIDRangeStart.HasValue && model.TransactionIDRangeEnd.Value < model.TransactionIDRangeStart.Value)
			{
				throw new Exception("The beginning transaction id range must be less than or equal to the ending transaction id range");
			}

			if (model.TransactionIDRangeEnd.HasValue && model.TransactionIDRangeStart.HasValue && (model.TransactionIDRangeEnd.Value - model.TransactionIDRangeStart.Value > MaximumNumberOfTransactionsToDownload))
			{
				throw new Exception("You may only attempt to download " + MaximumNumberOfTransactionsToDownload + " transactions at once");
			}

			var results = new Dictionary<Guid, string>();

			if (!model.TransactionIDRangeStart.HasValue && !model.TransactionIDRangeEnd.HasValue)
			{
				results = GasboyManagerChannelHelper.MakeCall<IGasboyStationServices, Dictionary<Guid, string>>(
					externalStationsService =>
					externalStationsService.DownloadNewTransactionsForStations(this.Security, selectedExternalStations));
			}
			else
			{
				Guid stationGuid = selectedExternalStations[0];

				results.Add(
					stationGuid,
					GasboyManagerChannelHelper.MakeCall<IGasboyStationServices, string>(
						externalStationsService =>
						externalStationsService.DownloadSelectedTransaction(
							this.Security,
							selectedExternalStations[0],
							model.TransactionIDRangeStart,
							model.TransactionIDRangeEnd)));
			}

			Dictionary<string, string> userFriendlyResults = this.TranslateDownloadResults(model, results);

			return string.Join(Environment.NewLine, userFriendlyResults);
		}

		/// <summary>
		/// Attempt to download events from the selected external stations
		/// </summary>
		/// <param name="model">
		/// Indicates which external stations are selected 
		/// </param>
		/// <returns>
		/// A string with the results of the event download for each of the stations
		/// </returns>
		[NonAction]
		private string InitiateEventDownloadRequest(GasboyStationOperationsModel model)
		{
			if (model.ExternalStations == null
				|| model.ExternalStations.FindIndex(externalStation => externalStation.IsSelected) < 0)
			{
				throw new Exception("You must select at least one External Station to initiate a download request for");
			}

			List<Guid> selectedExternalStations = model.ExternalStations.FindAll(externalStation => externalStation.IsSelected).Select(selectedExternalStation => selectedExternalStation.IdentityGuid).ToList();

			Dictionary<Guid, string> serviceResults = GasboyManagerChannelHelper.MakeCall<IGasboyStationServices, Dictionary<Guid, string>>(
				externalStationsService => externalStationsService.GetNewEventsForStations(
					this.Security,
					selectedExternalStations));

			Dictionary<string, string> userFriendlyResults = this.TranslateDownloadResults(model, serviceResults);

			return string.Join(Environment.NewLine, userFriendlyResults);
		}

		/// <summary>
		/// Take the dictionary of Station guids and download results and replace the guid with the station ID.
		/// The guid is used behind the scenes for processing, but won't mean much to users.
		/// </summary>
		/// <param name="model">Contains the stations we will look up IDs for</param>
		/// <param name="serviceResults">The results of a download, containing a dictionary of station guids and the result</param>
		/// <returns>A dictionary of station IDs and results</returns>
		[NonAction]
		private Dictionary<string, string> TranslateDownloadResults(GasboyStationOperationsModel model, Dictionary<Guid, string> serviceResults)
		{
			Dictionary<string, string> userFriendlyResults = new Dictionary<string, string>();

			foreach (KeyValuePair<Guid, string> serviceResult in serviceResults)
			{
				GasboyStation station =
					model.ExternalStations.Find(matchingStation => matchingStation.IdentityGuid == serviceResult.Key);

				if (station == null)
				{
					userFriendlyResults.Add(serviceResult.Key.ToString(), serviceResult.Value);
				}
				else
				{
					userFriendlyResults.Add(station.ID, serviceResult.Value);
				}
			}

			return userFriendlyResults;
		}

		/// <summary>
		/// The Get action for the external station general configuration page
		/// </summary>
		/// <returns>A view for the external station general configuration page</returns>
		[HttpGet]
		public ActionResult GasboyStationGeneralConfiguration()
		{
			var model = new GasboyStationGeneralConfigurationModel();

			try
			{
				model.TransactionAliasNames = FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasNameCollectionClass>(transactionAliases => transactionAliases.EnumerateNamesOnly(this.Security, false));
				model.GeneralConfiguration = GasboyChannelHelper.MakeCall<IGasboyStations, GasboyStationGeneralConfiguration>(externalStationsService => externalStationsService.GetGeneralConfigurationBySiteGuid(this.Security, this.Security.SiteGuid))
											 ?? new GasboyStationGeneralConfiguration();
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}

			return this.View(model);
		}

		/// <summary>
		/// The post action for the external station general configuration page. Saves the record to the database.
		/// </summary>
		/// <param name="model">The model for the external station general configuration page, which contains the information we want to save</param>
		/// <returns>If an error occurs, a view for the external station general configuration page.</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult GasboyStationGeneralConfiguration(GasboyStationGeneralConfigurationModel model)
		{
			try
			{
				model.TransactionAliasNames = FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasNameCollectionClass>(transactionAliases => transactionAliases.EnumerateNamesOnly(this.Security, false));

				if (ModelState.IsValid)
				{
					if (model.GeneralConfiguration.IdentityGuid == Guid.Empty)
					{
						GasboyChannelHelper.MakeCall<IGasboyStations>(
							externalStationsService =>
							externalStationsService.AddGeneralConfiguration(this.Security, model.GeneralConfiguration));
					}
					else
					{
						GasboyChannelHelper.MakeCall<IGasboyStations>(
							externalStationsService =>
							externalStationsService.ModifyGeneralConfiguration(this.Security, model.GeneralConfiguration));
					}
				}
				else
				{
					string errors = HttpUtility.JavaScriptStringEncode(string.Join(Environment.NewLine, ModelState.Values.SelectMany(value => value.Errors.Select(error => error.ErrorMessage))));
					throw new Exception(errors);
				}
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
				return this.View(model);
			}

			return this.RedirectWithPleaseWait("../../FMWebApp/FuelsManagerForm.aspx");
		}

		#region Entity Assignment and Ownership Support

		/// <summary>
		/// Can you assign External Stations to sites other than the site which owns the Station? 
		/// No, you can't. A station should exist at one and only one site, otherwise we won't know
		/// which site to save transactions downloaded from the station in.
		/// </summary>
		bool IEntityDiscovery.EntityAssignable
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Get the service class which supports entity assignment for External Stations.
		/// This doesn't appear to be used in any meaningful way but must be implemented to satisfy IEntityDiscovery
		/// </summary>
		Type IEntityDiscovery.EntityEngineType
		{
			get
			{
				return typeof(IGasboyStations);
			}
		}

		/// <summary>
		/// The type of entity we are supporting entity assignment for (External Stations)
		/// </summary>
		ENTITY_TYPE IEntityDiscovery.EntityType
		{
			get
			{
				return ENTITY_TYPE.EXTERNAL_STATION;
			}
		}

		/// <summary>
		/// Enumerate entity maps for External Stations. This is used by the entity ownership form to show External Stations
		/// owned by the site.
		/// </summary>
		/// <param name="security">Contains security information.</param>
		/// <param name="type">The type of entity assignment to enumerate. For External Stations this appears to only be OWNED, which is 
		/// used by the entity ownership form</param>
		/// <returns>Entity to site mappings for External Stations depending on the type provided.</returns>
		EntityToSiteMapCollectionClass IEntityDiscovery.EnumerateEntityMaps(SecurityClass security, ENTITY_ASSIGNMENT_TYPE type)
		{
			List<GasboyStation> externalStations = GasboyChannelHelper.MakeCall<IGasboyStations, List<GasboyStation>>(
																	 externalStationsService =>
																	 externalStationsService.Enumerate(security));

			EntityToSiteMapCollectionClass entityToSiteMapCollection = new EntityToSiteMapCollectionClass();

			if (!security.HasRight(RIGHT.MODIFY_EXTERNAL_STATION))
			{
				return entityToSiteMapCollection;
			}

			foreach (GasboyStation externalStation in externalStations)
			{
				if (type == ENTITY_ASSIGNMENT_TYPE.ASSIGNED)
				{
					if (security.SiteGuid == externalStation.SiteGuid)
					{
						continue;
					}

					if (security.LoginSiteGuid != externalStation.SiteGuid)
					{
						continue;
					}
				}
				else
				{
					if (security.SiteGuid != externalStation.SiteGuid)
					{
						continue;
					}
				}

				EntityToSiteMapClass entityToSiteMap = new EntityToSiteMapClass(externalStation);
				entityToSiteMapCollection.Add(entityToSiteMap);
			}

			return entityToSiteMapCollection;
		}

		/// <summary>
		/// Get the primary key (aka Identity Guid) of the external station matching the provided ID
		/// </summary>
		/// <param name="security">Contains Security Information</param>
		/// <param name="id">Identifies the external station to retrieve</param>
		/// <returns>The primary key (aka Identity Guid) of the external station matching the provided ID</returns>
		Guid IEntityDiscovery.GetIdentityGuid(SecurityClass security, string id)
		{
			return GasboyChannelHelper.MakeCall<IGasboyStations, Guid>(externalStationsService => externalStationsService.GetIdentityGuid(security, id));
		}

		/// <summary>
		/// Modify the provided external station's siteGuid. This is used for entity ownership changes.
		/// </summary>
		/// <param name="security">Contains Security Information</param>
		/// <param name="guid">Identifies the external station we want to modify</param>
		/// <param name="siteGuid">Identifies the site the external station should be owned by</param>
		void IEntityDiscovery.SetSiteGuid(SecurityClass security, Guid guid, Guid siteGuid)
		{
			GasboyStation externalStation = GasboyChannelHelper.MakeCall<IGasboyStations, GasboyStation>(externalStationsService => externalStationsService.Get(security, guid));

			externalStation.SiteGuid = siteGuid;
			GasboyChannelHelper.MakeCall<IGasboyStations>(externalStationsService => externalStationsService.Modify(security, externalStation));
		}

		#endregion
	}

	/// <summary>
	/// Contains the search parameters last used by the user when searching from the External Station Failed Transaction Summary page
	/// </summary>
	[Serializable]
	public class GasboyStationFailedTransactionSummarySearchParameters
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
		/// The transaction ID search parameter value
		/// </summary>
		public string TransactionID { get; set; }
	 
		/// <summary>
		/// The Station to display failed transactions for. An empty value indicates all stations
		/// </summary>
		public Guid ExternalStationGuid { get; set; }
	}
}

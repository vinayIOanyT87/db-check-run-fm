/// <summary>
/// File name:	TFMSImportForm.aspx.cs
/// Purpose:	To display page to allow the user to import ground fuel transactions.
///				
///	Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 
///				2000.  This file shall not be copied or reproduced in any form 
///				without the express written consent of Endress+Hauser.
///				
///	Author(s):	Richard R. Panachida
///	Version:	1.0.0  Current version
///	
///	Modification History:
///		Date:			   By:						Reason:
///		----------		--------------------	----------------------------------
///		2007-11-15		Richard Panachida		Fixed problem with alias type and index
///												      not being stored in the database.
///			
///		2008-09-30		Bill Dimovski			Added Excel template download,
///												      Modified import to work with Excel Worksheets, 
///												      Updated Transaction objects. (CSI 385)
///												      
///     2008-12-17      Bill Dimovski           Updated to save the uploaded Excel file onto the server 
///	                                            and then process the worksheet information from the saved file.
///     2009-03-13      Bill Dimovski           Updated to check if upload directory exists, if not attempt to create it. 
///     2009-03-26      Bill Dimovski           Updated retrieval of OWNER & MANAGER details to use the CompanyCollectionClass class. 
/// </summary>

using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;
using Accounting;
using EngineeringUnitsLibrary;
using System.Configuration;
using System.Collections.Generic;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.Exceptions;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.ServiceRequests;
using FMBusinessObjects.UtilityObjects;

namespace ADFWebApp
{
	public partial class TFMSImportForm : AccountingWebFormView
	{
		#region Private data members
		private string aliasName;
		private int aliasIndex;
		private int siteIndex;
		private TransactionTypes aliasTransType;
		private const string WORKSHEET_DIRECT_FUEL_PURCHASE = "Direct Fuel Purchase";
		private const string WORKSHEET_COMM_DIRECT_FUEL_PURCHASE = "Comm Direct Fuel Purchase";
		private const string DIRECT_FUEL_PURCHASE_ALIAS_NAME = "Direct Fuel Purchase";
		private const string COMM_DIRECT_FUEL_PURCHASE_ALIAS_NAME = "Commercial";
		private const string UPLOAD_DIRECTORY_PATH = "~/App_Data/DFP/";
		#endregion

		#region Page load
		/// <summary>
		/// This is the main entry point for the page.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (Session["Security"] == null)
				base.ErrorHandler(new FMSessionInvalidException());

			if (Page.IsPostBack == false)
			{
				this.MakeVisible(false);

				if (this.CheckRights() == true)
				{
					this.EnableControls(true);
				}
				else
				{
					this.EnableControls(false);
				}
			}
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method will determine if the user has the appropriate rights to perform
		/// the import functionality. It will return true if the user has the rights. Otherwise,
		/// it returns false.
		/// </summary>
		/// <returns></returns>
		private bool CheckRights()
		{
			bool okay = false;

			if ((base.security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA) == true)
				&& (base.security.HasRight(RIGHT.INTERFACE_IMPORT) == true)
				&& (base.security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT) == true))
			{
				okay = true;
			}

			return okay;
		}

		/// <summary>
		/// This method will make the result controls visible.
		/// </summary>
		/// <param name="visible"></param>
		private void MakeVisible(bool visible)
		{
			this.ResultsTextBox.Text = "";
			this.ResultsLabel.Visible = visible;
			this.ResultsTextBox.Visible = visible;
		}

		/// <summary>
		/// This method will enable or disable controls (ImportButton).
		/// </summary>
		/// <param name="enable"></param>
		private void EnableControls(bool enable)
		{
			this.ImportButton.Enabled = enable;

			if (this.GetExcelFileName().Equals("") == false)
			{
				this.ExcelTemplateHyperLink.Text += this.GetExcelFileName();
			}
			else
			{
				this.ExcelTemplateHyperLink.Visible = false;
			}
		}

		/// <summary>
		/// This method will find and set the appropriate alias details to use for creating
		/// transactions from the ground fuel transactions.
		/// </summary>
		private void FindAndSetAliasDetails()
		{
			//Direct Fuel Purchase and Commercial transactions both have TransTypeID's of 12
			this.aliasTransType = TransactionTypes.T12_InventoryNotAffected;

			TransactionAliasCollectionClass aliasList = null;

			aliasList = FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasCollectionClass>(
																	 x =>
																	 x.EnumerateByTransTypeID(base.security, this.aliasTransType)
																);


			if ((aliasList == null) || (aliasList.Count <= 0))
			{
				throw new Exception("No aliases found");
			}
			else
			{
				bool foundAlias = false;

				//Check to see if the Alias Name exists in the collection
				foreach (TransactionAliasClass transAlias in aliasList)
				{
					int index = transAlias.ID.ToUpper().IndexOf(this.aliasName.ToUpper());

					if (index >= 0)
					{
						this.aliasIndex = transAlias.Index;
						foundAlias = true;
						break;
					}
				}

				if (foundAlias == false)
				{
					throw new Exception("Alias " + this.aliasName + " not found\n");
				}
			}
		}

		/// <summary>
		/// This method will attempt to match the Location in the spreadsheet
		/// and if found set the Site.Index details.
		/// </summary>
		/// <returns></returns>
		private void FindAndSetSiteDetails(string locationName)
		{
			bool validLocation = false;

			// If the Location in the spreadsheet matches the current selected Site name
			if (locationName.Trim().ToUpper().Equals(base.security.SiteID.ToUpper()))
			{
				this.siteIndex = base.security.SiteIndex;
				validLocation = true;
			}

			// Check if Location in the spreadsheet exists/matches a site in within the collection
			else
			{
				SiteCollectionClass SiteCollection = FMChannelHelper.MakeCall<ISites, SiteCollectionClass>(
																	 x =>
																	 x.EnumerateByParentSite(base.security, base.security.SiteGuid)
																);

				foreach (SiteClass site in SiteCollection)
				{
					if (site.ID.ToUpper().Equals(locationName.Trim().ToUpper()))
					{
						this.siteIndex = /*site.IdentityGuid*/BaseDataObject.DUMMY_INDEX;
						validLocation = true;
						break;
					}
				}

				if (validLocation == false)
				{
					throw new Exception("Location '" + locationName + "' not found\n");
				}
			}
		}

		/// <summary>
		/// This method will create transactions for each of the Imported ground fuel transaction items.
		/// </summary>
		/// <param name="tfmsList"></param>
		/// <param name="fileName"></param>
		private void CreateTransactions(ArrayList tfmsList, string fileName)
		{
			int transCount = 0;

			if ((tfmsList == null) || (tfmsList.Count <= 0))
			{
				this.ResultsTextBox.Text += "No data found in worksheet\n";
			}
			else
			{
				DFPCommercialCurrencyValidation dfpCurrencyValidation = new DFPCommercialCurrencyValidation(base.security);
				TransactionDO transDO = null;
				LineItemDO lineItemDO = null;
				ProductClass product = null;

				SaveTransactionsResultDO resultDO = null;
				SaveTransactionsSR serviceRequest = new SaveTransactionsSR();

				serviceRequest.Security = base.security;
				serviceRequest.CurrentSiteGuid = base.security.SiteGuid;

				SiteClass currentSite = FMChannelHelper.MakeCall<ISites, SiteClass>(
																	 x =>
																	 x.Get(security, security.SiteGuid, false, false, false)
																);

				SiteTimeConverter converter = new SiteTimeConverter(currentSite);

				// JS20101020 WI-19089 allows us to check for duplicate purchase order numbers within the import file
				List<string> importPurchaseNumberList = new List<string>();

				foreach (TFMSDO tfmsDO in tfmsList)
				{
					transDO = new TransactionDO();
					lineItemDO = new LineItemDO();

					// Search for matching Alias and set associated details
					this.FindAndSetAliasDetails();

					// Search for matching Location/Site and set associated details				
					this.FindAndSetSiteDetails(tfmsDO.Location);

					// Search for matching Product		
					product = this.FindProductInfo(tfmsDO.Product);

					// Product is a required field.
					if (product == null)
					{
						throw new Exception("Could not save records. " + tfmsDO.Product + " is an invalid product\n");
					}

					// Populate transaction header information.		
					transDO.Alias = this.aliasName;
					transDO.TransactionAliasGuid = BaseDataObject.DUMMY_GUID;	// this.aliasIndex;
					transDO.TransTypeID = this.aliasTransType;

					// Retrieve Manager and Owner details.
					CompanyCollectionClass Managers = FMChannelHelper.MakeCall<ICompanies, CompanyCollectionClass>(
																	 x =>
																	 x.EnumerateByRole(this.security, COMPANY_ROLE.MANAGER, true, false)
																);


					if (Managers.Count == 1)
					{
						transDO.ManagerID = Managers[0].ID;
						transDO.ManagerCode = Managers[0].Code;
						transDO.ManagerCompanyGuid = Managers[0].MasterRecordGuid;
					}
					else
					{
						throw new Exception("\n Manager details could not be determined. ");
					}

					CompanyCollectionClass Owners = FMChannelHelper.MakeCall<ICompanies, CompanyCollectionClass>(
																	 x =>
																	 x.EnumerateByRole(this.security, COMPANY_ROLE.OWNER, true, false)
																);

					if (Owners.Count == 1)
					{
						transDO.OwnerID = Owners[0].ID;
						transDO.OwnerCode = Owners[0].Code;
						transDO.OwnerCompanyGuid = Owners[0].MasterRecordGuid;
					}
					else
					{
						throw new Exception("\n Owner details could not be determined. ");
					}

					transDO.ShipperID = null;
					transDO.ShipperCode = null;
					transDO.ShipperCompanyGuid = Guid.Empty;
					transDO.BillToID = null;
					transDO.BillToCode = null;
					transDO.BillToCompanyGuid = Guid.Empty;
					transDO.Site = tfmsDO.Location;
					transDO.SiteGuid = BaseDataObject.DUMMY_GUID;	// this.siteIndex;
					transDO.InventoryDate = tfmsDO.DateTime.Value;
					transDO.LoadID = null;
					transDO.TransactionDateTime = tfmsDO.DateTime.Value;
					// JS20100922 WI-17822 must convert UTC to site time
					transDO.Date03 = converter.ConvertToSiteTime(DateTime.UtcNow);        // Date03 is the field Actual Date & Time

					if (tfmsDO.FuelCardNumber != null)
					{
						transDO.FuelCardID = tfmsDO.FuelCardNumber;
					}

					// Country is not required
					if (string.IsNullOrEmpty(tfmsDO.Country) == false)
					{
						transDO.UserData[TransactionDO.USER_DATA_KEY_03] = tfmsDO.Country;
					}

					// Perform validations on the direct fuel purchase number.  It is a required
					// field an must be unique within the system.
					if (string.IsNullOrEmpty(tfmsDO.PurchaseNumber) == false)
					{
						// JS20101020 WI-19089 check for unique-ness in the import file as well
						bool isUnique = !importPurchaseNumberList.Contains(tfmsDO.PurchaseNumber);
						if (isUnique)
						{
							importPurchaseNumberList.Add(tfmsDO.PurchaseNumber);
						}
						else
						{
							throw new Exception("Direct Fuel Purchase Number is not unique within the import file.\n");
						}

						// Js20101020 Check for uniqueness in the database
						isUnique = FMChannelHelper.MakeCall<ITFMSServices, bool>(
																	 x =>
																	 x.IsDirectPurchaseNumberUnique(base.security, tfmsDO.PurchaseNumber)
																);

						if (isUnique == true)
						{
							transDO.PONumber = tfmsDO.PurchaseNumber;
						}
						else
						{
							throw new Exception("Direct Fuel Purchase Number is not unique.\n");
						}
					}
					else
					{
						throw new Exception("Direct Fuel Purchase Number is required.\n");
					}

					// The only validation on Notes is the field length. It cannot be
					// over 1000 characters.
					if (string.IsNullOrEmpty(tfmsDO.Notes) == false)
					{
						if (tfmsDO.Notes.Length > 1000)
						{
							this.ResultsTextBox.Text += "Truncated notes to 1000 for Direct Fuel Purchase number: " + tfmsDO.PurchaseNumber + "\n";
							transDO.Notes = tfmsDO.Notes.Substring(0, 1000);
						}
						else
						{
							transDO.Notes = tfmsDO.Notes;
						}
					}

					// Perform validation for on Customer ID if present.
					// This field in not required.
					if (string.IsNullOrEmpty(tfmsDO.Customer) == false)
					{
						CompanyClass company = this.FindCompanyInfo(tfmsDO.Customer);
						if (company != null)
						{
							transDO.ShipToCode = company.Code;
							transDO.ShipToID = company.ID;
							transDO.ShipToCompanyGuid = company.MasterRecordGuid;
						}
						else
						{
							throw new Exception("Invalid customer (not in system): " + tfmsDO.Customer + ".\n");
						}
					}

					// Perform validation for on Supplier ID if present.
					// This field in not required.
					if (string.IsNullOrEmpty(tfmsDO.Supplier) == false)
					{
						CompanyClass company = this.FindCompanyInfo(tfmsDO.Supplier);
						if (company != null)
						{
							transDO.SupplierCode = company.Code;
							transDO.SupplierID = company.ID;
							transDO.SupplierCompanyGuid = company.MasterRecordGuid;
						}
						else
						{
							throw new Exception("Invalid supplier (not in system): " + tfmsDO.Supplier + ".\n");
						}
					}

					// Perform validation for the Defense Asset ID.  It is required.
					dfpCurrencyValidation.ClearErrorMessage();
					if (dfpCurrencyValidation.DefenseAssetValidation(tfmsDO) == true)
					{
						if (tfmsDO.DefenseAssetID.Length >= 4)
						{
							int length = tfmsDO.DefenseAssetID.Length;
							transDO.DestinationEQ1.EquipmentRefID = tfmsDO.DefenseAssetID.Substring(length - 4, 4);
						}

						transDO.DestinationEQ1.EquipmentGuid = BaseDataObject.DUMMY_GUID;// dfpCurrencyValidation.EquipmentIndex.Value;
						transDO.DestinationEQ1.RegistrationID = tfmsDO.DefenseAssetID;
					}
					else
					{
						throw new Exception(dfpCurrencyValidation.ErrorMsg);
					}

					// Populate the line item fields.
					lineItemDO.LineNumber = 1;
					lineItemDO.Product = product.ID;
					lineItemDO.ProductCode = product.Code;
					lineItemDO.ProductType = ProductClass.ProductTypeID(product.ProductType);
					lineItemDO.ProductGuid = product.Index;

					// Quantity is a required field. Convert the quantity which is in liters to cubic meters.
					if (tfmsDO.Quantity != null)
					{
						double? siValue = this.ConvertToSpecifiedUnit(tfmsDO.Quantity.Value, (int)ENGINEERING_UNIT.FMV_Litre, ENGINEERING_UNIT.FMV_Meter3);
						lineItemDO.Quantity.Gross = siValue.Value;
						lineItemDO.Quantity.Net = siValue.Value;
						//lineItemDO.Volume.Gross = tfmsDO.Quantity.Value * 1.0E-03;
						//lineItemDO.Volume.Net   = tfmsDO.Quantity.Value * 1.0E-03;
					}
					else if (tfmsDO.UOMQuantity != null)
					{
						// Check for the existence of an UOM quantity. If it exists, then ensure the UOM
						// unit is a valid standard volume unit.
						dfpCurrencyValidation.ClearErrorMessage();
						if (dfpCurrencyValidation.UOMQuantityValidation(tfmsDO) == false)
						{
							throw new Exception(dfpCurrencyValidation.ErrorMsg);
						}
						else
						{
							lineItemDO.AlternativeGrossVolume = tfmsDO.UOMQuantity.Value;
							lineItemDO.AlternativeNetVolume = tfmsDO.UOMQuantity.Value;
							lineItemDO.AlternativeUnits = dfpCurrencyValidation.FoundCUUnitIndex.Value;

							// Convert the alternative volumes from its unit into SI unit and save in the
							// Gross and Net fields.
							double? siValue = this.ConvertToSpecifiedUnit(tfmsDO.UOMQuantity.Value, dfpCurrencyValidation.FoundCUUnitIndex, ENGINEERING_UNIT.FMV_Meter3);

							if (siValue != null)
							{
								lineItemDO.Quantity.Gross = siValue.Value;
								lineItemDO.Quantity.Net = siValue.Value;

								// Since the quantity is null, we have to set the converted UOM quantity so that the pricing is 
								// calculated.
								double? litreValue = this.ConvertToSpecifiedUnit(siValue.Value, (int)ENGINEERING_UNIT.FMV_Meter3, ENGINEERING_UNIT.FMV_Litre);

								// JS20100914 WI-17681 an UOM quantity now becomes quantity as is
								//tfmsDO.Quantity = litreValue.Value;
								tfmsDO.Quantity = tfmsDO.UOMQuantity;
							}
							else
							{
								throw new Exception("Cannot convert UOM Quantity value to SI.\n");
							}
						}
					}
					else
					{
						throw new Exception("Invalid quantity or UOM quantity.\n");
					}

					// Only perform pricing validation if one of the fields (Foreign Price, Foreign Currency Unit,
					// Total Foreign Price, Fuel Price AUD, or Total Fuel Price AUD) have a value. If not,
					// then ignore validation.
					if (((tfmsDO.ForeignCurrencyPrice == null)
						&& (string.IsNullOrEmpty(tfmsDO.ForeignCurrencyUnit) == true)
						&& (tfmsDO.TotalForeignCurrencyPrice == null)
						&& (tfmsDO.TotalPriceAUD == null)
						&& (tfmsDO.FuelPriceAUD == null)) == false)
					{
						// Must be a domestic price if there is no foreign currency unit.
						if (string.IsNullOrEmpty(tfmsDO.ForeignCurrencyUnit) == true)
						{
							dfpCurrencyValidation.ClearErrorMessage();
							Guid supplierGuid = Guid.Empty;

							if (transDO.SupplierCompanyGuid != Guid.Empty)
							{
								supplierGuid = transDO.SupplierCompanyGuid;
							}

							if (dfpCurrencyValidation.DomesticCurrencyValidation(tfmsDO, BaseDataObject.DUMMY_GUID, BaseDataObject.DUMMY_GUID /*, supplierIndex, product.Index*/) == false)
							{
								throw new Exception(dfpCurrencyValidation.ErrorMsg);
							}
						}
						else
						{
							dfpCurrencyValidation.ClearErrorMessage();

							if (dfpCurrencyValidation.ForeignCurrencyValidation(tfmsDO) == false)
							{
								throw new Exception(dfpCurrencyValidation.ErrorMsg);
							}
							else
							{
								if (dfpCurrencyValidation.CurrencyGuid != Guid.Empty)
								{
									lineItemDO.CurrencyGuid = dfpCurrencyValidation.CurrencyGuid;
								}
							}
						}
					}

					if (tfmsDO.FuelPriceAUD != null)
					{
						lineItemDO.ProductPrice = tfmsDO.FuelPriceAUD.Value;
					}

					if (tfmsDO.ForeignCurrencyPrice != null)
					{
						lineItemDO.NonDomesticPrice = tfmsDO.ForeignCurrencyPrice.Value;
					}

					if (tfmsDO.GST != null)
					{
						lineItemDO.Tax2 = tfmsDO.GST.Value;
					}

					if (tfmsDO.Excise != null)
					{
						lineItemDO.Tax1 = tfmsDO.Excise.Value;
					}

					if (tfmsDO.TotalPriceAUD != null)
					{
						lineItemDO.UserData[TransactionDO.USER_DATA_LINE_ITEM_KEY_02] = tfmsDO.TotalPriceAUD.ToString();
						lineItemDO.TotalPriceWithTax = tfmsDO.TotalPriceAUD.Value;
					}

					if (tfmsDO.TotalForeignCurrencyPrice != null)
					{
						lineItemDO.UserData[TransactionDO.USER_DATA_LINE_ITEM_KEY_03] = tfmsDO.TotalForeignCurrencyPrice.ToString();
					}

					// Add the line item to the transaction
					transDO.LineItems.Add(lineItemDO);

					// Add transaction
					serviceRequest.Transactions.Add(transDO);
					transCount++;
				}

				try
				{
					resultDO = FMChannelHelper.MakeCall<ISaveTransactionsProcessor, SaveTransactionsResultDO>(
																	 x =>
																	 x.SaveTransactions(serviceRequest)
																);
				}
				catch (SaveTransactionsException ex)
				{
					if ((ex.Results.Count >= 1)
						&& (typeof(TransactionValidationResult).IsInstanceOfType(ex.Results[0]) == true)
						&& (((TransactionValidationResult)ex.Results[0]).ErrorList.Count >= 1))
					{
						throw new Exception(((TransactionValidationResult)ex.Results[0]).ErrorList[0]);
					}
					else
					{
						throw new Exception("Unknown SaveTransactionException");
					}
				}
				catch (Exception ex)
				{
					throw new Exception("Could not save Transactions: " + ex.InnerException.Message + " " + ex.Message + "\n");
				}

				this.ResultsTextBox.Text += "Successfully imported " + transCount.ToString() + " " + this.aliasName +
											" ground fuel transactions from file " + fileName + "\n";
			}
		}

		/// <summary>
		/// This method will retrieve the product that matches the product ID. It will
		/// return null if the product does not exist.
		/// </summary>
		/// <param name="productID"></param>
		/// <returns></returns>
		private ProductClass FindProductInfo(string productID)
		{
			if (string.IsNullOrEmpty(productID) == true)
			{
				return null;
			}

			ProductClass product = FMChannelHelper.MakeCall<IProducts, productClass>(
																	 x =>
																	 x.GetByID(base.security, productID)
																);


			if ((product == null) || (product.Index == 0))
			{
				return null;
			}

			return product;
		}

		/// <summary>
		/// This method will return the company class for the given ID. It will
		/// return null if not found.
		/// </summary>
		/// <param name="companyID"></param>
		/// <returns></returns>
		private CompanyClass FindCompanyInfo(string companyID)
		{
			if (string.IsNullOrEmpty(companyID) == true)
			{
				return null;
			}

			Guid companyGuid = FMChannelHelper.MakeCall<ICompanies, Guid>(
																	 x =>
																	 x.GetIdentityGuid(base.security, companyID)
																);


			if (companyGuid == Guid.Empty)
			{
				return null;
			}

			CompanyClass company = companies.Get(base.security, companyGuid);

			return company;
		}

		/// <summary>
		/// This method will retrieve the the filename of the Excel template to be made available
		/// for download. It will only return the filename if 1 x Excel file exists in path
		/// </summary>
		/// <returns></returns>
		private string GetExcelFileName()
		{
			string excelFileName = "";
			DirectoryInfo dir = new DirectoryInfo(Server.MapPath("."));
			FileInfo[] excelFiles = dir.GetFiles("*.xls");

			if (excelFiles.Length == 1)
			{
				excelFileName = excelFiles[0].Name;
			}
			return excelFileName;
		}

		/// <summary>
		/// This method will open a new TFMS Import log file if the system has been
		/// configured with a log directory path. It will also write the results of the
		/// import to the log file.  The file name is be named as follows:
		/// "TFMSImportLogResults_yyyy-MM-dd HH:mm:ss"
		/// </summary>
		/// <param name="results"></param>
		private void LogResults(string results)
		{
			if (string.IsNullOrEmpty(results) == false)
			{
				string errorMsg = "";
				string currentDateStr = String.Format("{0:yyyy_MM_dd_HH_mm_ss}", DateTime.Now);
				string fileName = "TFMSImportLogResults_" + currentDateStr + ".txt";
				string logDirectory = this.FindLogDirectory();

				if (string.IsNullOrEmpty(logDirectory) == false)
				{
					string logFileName = logDirectory + fileName;

					try
					{
						StreamWriter writer = File.CreateText(logFileName);

						if (writer != null)
						{
							try
							{
								writer.Write(results);
								writer.Close();
							}
							catch (Exception)
							{
								writer.Close();
								errorMsg = "\nUnable to write to file: " + logFileName;
								throw new Exception(errorMsg);
							}
						}
					}
					catch (Exception)
					{
						errorMsg = "\nCannot find path or access denied: " + logDirectory;
						throw new Exception(errorMsg);
					}
				}
			}
		}

		/// <summary>
		/// This method will return the TFMS Import log directory from the Application Settings
		/// (fuelsmanager.config). It will return null if not found.
		/// </summary>
		/// <returns></returns>
		private string FindLogDirectory()
		{
			string logDirectory = null;

			if (ConfigurationManager.AppSettings["TFMSImportLogFileDirPath"] != null)
			{
				logDirectory = (string)ConfigurationManager.AppSettings["TFMSImportLogFileDirPath"];
			}

			return logDirectory;
		}
		#endregion

		#region Event handlers
		/// <summary>
		/// This method will handle the import button event. It will start the process of
		/// reading and parsing the selected ground fuel transaction file and storing it as either
		/// an "Direct Fuel Purchase" or "Commercial" transaction.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ImportBtnCommand(object sender, System.Web.UI.WebControls.CommandEventArgs e)
		{
			this.MakeVisible(true);

			try
			{
				if (Request.Files.AllKeys.Length != 0)
				{
					HttpPostedFile file = this.FileUpload1.PostedFile;
					string path = "";

					if ((file.FileName != "") && (file.ContentLength != 0))
					{
						ExcelImport excelReader = new ExcelImport();
						try
						{
							// Check if upload directory exists, if not attempt to create it
							if (Directory.Exists(HttpContext.Current.Server.MapPath(UPLOAD_DIRECTORY_PATH)) == false)
							{
								Directory.CreateDirectory(HttpContext.Current.Server.MapPath(UPLOAD_DIRECTORY_PATH));
							}

							// Initial the Unit Converter.

							// Save the uploaded file in ~/App_Data/DFP using a unique filename
							path = HttpContext.Current.Server.MapPath(UPLOAD_DIRECTORY_PATH) +
									System.Guid.NewGuid().ToString() + ".xls";
							file.SaveAs(path);

							//Process DFP
							this.aliasName = DIRECT_FUEL_PURCHASE_ALIAS_NAME;
							this.ResultsTextBox.Text += "Attempting to Import: " + this.aliasName + "\n";
							excelReader.ReadWorksheet(path, WORKSHEET_DIRECT_FUEL_PURCHASE);
							this.CreateTransactions(excelReader.TFMSCollection, file.FileName);

							//Process Commercial
							this.aliasName = COMM_DIRECT_FUEL_PURCHASE_ALIAS_NAME;
							this.ResultsTextBox.Text += "\nAttempting to Import: " + this.aliasName + "\n";
							excelReader.ReadWorksheet(path, WORKSHEET_COMM_DIRECT_FUEL_PURCHASE);
							this.CreateTransactions(excelReader.TFMSCollection, file.FileName);

							//Delete the uploaded spreadsheet as no longer required after processing
							File.Delete(path);

							// Log results to a file on the server
							try
							{
								this.LogResults(this.ResultsTextBox.Text);
							}
							catch (Exception ex)
							{
								this.ResultsTextBox.Text += ex.Message + "\n" + "Import terminated with errors.";
							}
						}
						catch (Exception ex)
						{
							if (File.Exists(path))
							{
								File.Delete(path);
							}
							this.ResultsTextBox.Text += ex.Message + "\n" + "Import terminated with errors.";

							// Log results to a file on the server
							try
							{
								this.LogResults(this.ResultsTextBox.Text);
							}
							catch (Exception except)
							{
								this.ResultsTextBox.Text += except.Message + "\n" + "Import terminated with errors.";
							}
						}
					}
					else
					{
						this.MakeVisible(false);
						throw new Exception("Select a file to import");
					}
				}
			}
			catch (Exception except)
			{
				this.MakeVisible(false);
				base.ErrorHandler(except);
			}
		}

		/// <summary>
		/// This method will convert the UOM quantity value to SI value. It will
		/// return null if the conversion failed.
		/// </summary>
		/// <param name="fromVolume"></param>
		/// <param name="fromUnitIndex"></param>
		/// <param name="toUnits"></param>
		/// <returns></returns>
		private double? ConvertToSpecifiedUnit(double fromVolume, int? fromUnitIndex, ENGINEERING_UNIT toUnits)
		{
			double? siValue = null;

			if (fromUnitIndex != null)
			{
				try
				{
					double toValue = 0;
					double specialParam = 0;

					EngineeringUnits.Convert(fromVolume, (ENGINEERING_UNIT)fromUnitIndex, ref toValue, toUnits, specialParam);
					siValue = toValue;
				}
				catch (Exception)
				{
					// Do nothing.
				}
			}

			return siValue;
		}
		#endregion

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			InitializeComponent();
			base.init();
			base.OnInit(e);
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.ImportButton.Command += new System.Web.UI.WebControls.CommandEventHandler(this.ImportBtnCommand);
		}
		#endregion
	}
}


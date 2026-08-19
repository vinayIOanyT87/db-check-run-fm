
/// <summary>
///	File name:	PIDXBOLProcessor.cs
///	Purpose:		The purpose of this class is to query the Transaction PIDX queue for
///					PIDX BOLs (BB and CB) to transmit to the appropriate PIDX external service.
///					
///	Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 
///					2000.  This file shall not be copied or reproduced in any form 
///					without the express written consent of Endress+Hauser.
///					
///	Author(s):	Richard R. Panachida
///	Version:		1.0.0  Current version
///	
///	Modification History:
///		Date:			By:						Reason:
///		----------	--------------------	----------------------------------
///		2007-03-20	I.Orndorff				- Modified "CreateCB()" to create BOLCB
///													based on PIDX Type. This fixes CSI #5598.
///													- Modified "GetProductInfo()" to handle BOLCB
///													based on PIDX Type. This fixes CSI #5598.
///		2008-04-03	C. Knight				- Modified GetProductInfo() to treat "Posted"
///													transactions as "Completed"
///						
///		2008-06-04	I.Orndorff				7.4.5.0 - Modified "UpdateSentStatus()" to use
///													  the SentFlag in "TransactionPIDXDO".
///													  
///		2008-06-10	W.Gray					Modified TruncateDocNumber to return 0 if docNumber is ""
///													so that Canceled transactions will be processed
///
///		2008-06-13	I.Orndorff				7.4.5.2 - Modified "GetPIDXProductCode()" to RightPad the Product
///													  code before left padding it.
///													  
///		2008-06-27	I.Orndorff				7.4.5.3 - Modified "GetProductInfo()" to use TransactionDO.Reversal instead of
///													  gross to set the creditIndicator flag.
///		
///		2008-07-09	I.Orndorff				7.4.5.4 - Modified "GetProductInfog()" to remove check for completed/posted 
///													  line items to ensure all line items are sent PIDX. This fixes CSI #6019.
///
///		2008-07-30	W.Gray					7.4.5.5 - Revised to convert TransDateTime from UTC to SiteTime. (CSI 6045)
///		
///		2008-08-18	W.Gray					7.4.5.6 - Revised to improve performance by retrieving product only once
///													with false for Authorized Companies parameter. (CSI 6101)
///													
///		2008-10-14	W.Gray					7.4.6.0 - Correction to recoginise ReversalType of TransactionDO.ReversalWithUpdate
///		
///		2008-11-07	W.Gray					7.4.6.1 - Revision to send only Completed Line  Items (CSI 6281)
///		
///		2008-11-18	W.Gray					7.4.6.2 - Revised to throw exception in GetTransaction if BrokenBlend
///															
/// </summary>
namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Collections;
	using System.Diagnostics;
	using System.Globalization;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.LogClient;
	using FMBusinessObjects.PIDXTransactions;
	using FMBusinessObjects.ServiceRequests;
	using FMBusinessObjects.UtilityObjects;

	using FMCore;

	using Varec.CommonComponents.EngineeringUnitsLibrary;

	public class PidxBolProcessorClass : IPidxBolProcessor
	{
		#region Private data members

		private SecurityClass security;
		private TransactionPIDXSR serviceRequest;
		private ArrayList transPidxDOList;

		private readonly Logger logger;

		private SiteClass site;
		private TransactionDO transDO;

		private const string ErrMsg001 = "Error parsing SPLC/Terminal Operator string or converting one of the " +
											"following into a number: Authorization Number, Seller ID, SPLC, " +
											"Terminal Operator ID, or BOL number.";
		private const string ErrMsg002 = "Error parsing SPLC/Terminal Operator string or converting one of the " +
											"following into a number: Final Shipper ID, Seller ID, SPLC, Terminal " +
											"Operator ID, Trans Date Time, or BOL number.";
		private const string ErrMsg003 = "Could not retrieve PIDX Profile for guid of: ";
		private const string ErrMsg004 = "Could not retrieve transaction for trans ID: ";
		private const string ErrMsg005 = "Not able to send BOL with trans/authorization: ";
		private const string ErrMsg006 = "Could not convert product units for product type: ";
		/*
				private const string ErrMsg007 = "Product guid is null";
		* /
				  private const string ErrMsg008 = "Could not convert product density";

				  /// <summary>
				  /// The err msg 009.
				  /// </summary>
				  private const string ErrMsg009 = "Could not convert product temperature";

		/ *
				private const string Msg001 = "Company guid is null for profile guid of ";
		*/
		/*
				private const string Msg002 = "Product guid is null for profile guid";
		*/
		/// <summary>
		/// The bol record.
		/// </summary>
		private BOLBase bolRecord;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the transaction PIDX processor class.
		/// It must initialize the accounting service implemention class in the base class
		/// and set the request command with the service request string.
		/// </summary>
		public PidxBolProcessorClass()
		{
			this.logger = new Logger("PIDXBOLProcessor");
		}
		#endregion

		/// <summary>
		/// The bol types.
		/// </summary>
		private enum BolTypes
		{
			// "Inconsistent" names chosen to match actual record definition
			// ReSharper disable InconsistentNaming
			BB,
			CB,
			BL

			// ReSharper restore InconsistentNaming
		}

		/// <summary>
		/// This method will return the date (mmddyy) as an integer.
		/// </summary>
		/// <param name="shipDate"></param>
		/// <returns></returns>
		private string GetShipMMDDYY(DateTimeOffset shipDate)
		{
			//int month = shipDate.Month;
			//int day = shipDate.Day;
			//int year = shipDate.Year;

			string tempStr;

			//if (month < 10)
			//{
			//   tempStr = tempStr + "0" + month.ToString();
			//}
			//else
			//{
			//   tempStr = tempStr + month.ToString();
			//}

			//if (day < 10)
			//{
			//   tempStr = tempStr + "0" + day.ToString();
			//}
			//else
			//{
			//   tempStr = tempStr + day.ToString();
			//}

			//year = Convert.ToInt32(year.ToString().Substring(2));

			//if (year < 10)
			//{
			//   tempStr = tempStr + "0" + year.ToString();
			//}
			//else
			//{
			//   tempStr = tempStr + year.ToString();
			//}
			tempStr = shipDate.ToString("MMddyy");

			return tempStr;
		}

		#region Public Methods
		/// <summary>
		/// This method is the entry point of the processor from the client.
		/// </summary>
		/// <param name="sr"></param>
		/// <returns></returns>
		public void Process(TransactionBolPidxSR sr)
		{
			this.security = sr.Security;
			this.serviceRequest = new TransactionPIDXSR { Security = sr.Security };
			this.transPidxDOList = null;
			this.bolRecord = null;

			try
			{
				SitesClass sites = new SitesClass();
				this.site = sites.GetUsingGuid(this.security, this.security.SiteGuid);

				this.SendBoLs();
			}
			catch (Exception ex)
			{
				this.LogErrors(ex.Message);
			}
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method is the main entry point for the process of creating PIDX BOLs.
		/// </summary>
		private void SendBoLs()
		{
			bool sentSuccessful = false;

			if (this.ReadQueue())
			{
				// Loop through the list of transaction PIDX BOL entries in the queue that
				// have to be sent to the PIDX services.
				foreach (TransactionPIDXDO pidxDO in this.transPidxDOList)
				{
					// Get the associated profile for the transaction PIDX entry
					PIDXProfileClass profile = this.GetPIDXProfile(pidxDO.PIDXProfileGuid);

					if (!profile.Enabled)
					{
						// Do not attempt to send transactions for disabled profiles.
						continue;
					}

					// Create a BB BOL when the authorization number exists. Otherwise, create
					// a CB BOL.
					if (!string.IsNullOrEmpty(pidxDO.AuthorizationNumber))
					{
						bool createOk = false;

						if (profile.Version == PIDXVersion.OneDotZeroTwo)
						{
							createOk = this.CreateBb(profile, pidxDO);
						}
						else if (profile.Version == PIDXVersion.FourDotZeroOne)
						{
							createOk = this.CreateBl(profile, pidxDO);
						}

						if (createOk)
						{
							sentSuccessful = this.SendToService(profile);
						}
					}
					else
					{
						bool createOk = false;

						if (profile.Version == PIDXVersion.OneDotZeroTwo)
						{
							createOk = this.CreateCb(profile, pidxDO);
						}
						else if (profile.Version == PIDXVersion.FourDotZeroOne)
						{
							createOk = this.CreateBl(profile, pidxDO);
						}

						if (createOk)
						{
							sentSuccessful = this.SendToService(profile);
						}
					}

					// On successful transmission of the BOL, update the corresponding BOL sent flag
					// in the consolidated DB.
					if (sentSuccessful)
					{
						try
						{
							// Update the sent flag for the transaction PIDX record.
							pidxDO.SentFlag = true;
							this.UpdateSentFlag(pidxDO);

							// Update the status for the the transaction.
							this.UpdateTransaction();
						}
						catch (Exception ex)
						{
							this.LogErrors(ex.Message);
						}
					}
					else
					{
						this.LogErrors(ErrMsg005 + pidxDO.TransactionGuid + "/" + pidxDO.AuthorizationNumber);
					}
				}
			}
		}

		/// <summary>
		/// This method will send the appropriate BOL record to the appropriate service.
		/// </summary>
		/// <param name="profile"></param>
		/// <param name="bolType"></param>
		/// <returns></returns>
		private bool SendToService(PIDXProfileClass profile)
		{
			bool successful = false;

			switch (profile.Type)
			{
				case PIDXType.Tds:
					{
						var tds = new TcpCommTds
						{
							HostName = profile.IPAddress,
							Port = profile.Port,
							LoginName = profile.UserID,
							LoginPassword = profile.Password,
							PidxRecord = this.bolRecord,
							Version = profile.Version,
							LogFileNameandPath = profile.LoggingEnabled ? profile.LogFilePath : string.Empty
						};

						successful = tds.SendTransaction();
						break;
					}

				case PIDXType.Dtn:
					{
						var dtn = new TcpCommDtn
						{
							HostName = profile.IPAddress,
							Port = profile.Port,
							LoginName = profile.UserID,
							LoginPassword = profile.Password,
							PidxRecord = this.bolRecord,
							Version = profile.Version,
							LogFileNameandPath = profile.LoggingEnabled ? profile.LogFilePath : string.Empty
						};

						successful = dtn.SendTransaction();
						break;
					}
			}

			return successful;
		}

		/// <summary>
		/// This method will create the BOL BB record for given profile and
		/// transaction PIDX record.
		/// </summary>
		/// <param name="profile">
		/// PIDX provider whom to send the transaction
		/// </param>
		/// <param name="pidxDO">
		/// transaction to send
		/// </param>
		/// <returns>
		/// success status
		/// </returns>
		private bool CreateBb(PIDXProfileClass profile, TransactionPIDXDO pidxDO)
		{
			bool successful = true;
			this.bolRecord = new BOLBBRecord();

			try
			{
				this.GetTransaction(pidxDO.TransactionGuid);

				try
				{
					PIDXProfileCompanyMapsClass pidxProfileCompanyMaps = new PIDXProfileCompanyMapsClass();
					PIDXProfileCompanyMapClass pidxProfileCompanyMap = pidxProfileCompanyMaps.Get(this.security, pidxDO.PIDXProfileGuid, pidxDO.CompanyPersonnelToShipToBillToGuid);
					string sellerID = pidxProfileCompanyMap.SellerID;
					string splc = this.site.SPLCCode;
					string termOp = profile.TerminalID;
					DateTimeOffset? transactionDateTime = this.transDO.TransactionDateTime;
					if (transactionDateTime != null)
					{
						DateTimeOffset transDate = TimeConverter.ToSiteTime(this.site, transactionDateTime.Value);

						((BOLBBRecord)this.bolRecord).AuthorizationNumberDigit = Convert.ToInt32(pidxDO.AuthorizationNumber);
						this.bolRecord.SellerIDDigit = Convert.ToInt32(sellerID);
						this.bolRecord.SPLCCodeDigit = Convert.ToInt32(splc);
						this.bolRecord.TerminalOperatorDigit = Convert.ToInt32(termOp);
						this.bolRecord.ShipDayDigit = transDate.Day;
						this.bolRecord.BOLNumberDigit = this.TruncateDocNumber(this.transDO.DocumentNumber);

						// Gets the product information and stores it in the BB record.
						this.GetProductInfo(BolTypes.BB);
					}
					else
					{
						this.LogErrors(ErrMsg002);
						successful = false;
					}
				}
				catch (Exception)
				{
					this.LogErrors(ErrMsg001);
					successful = false;
				}
			}
			catch (Exception ex)
			{
				this.LogErrors(ErrMsg004 + pidxDO.TransID + " " + ex.Message);
				successful = false;
			}

			return successful;
		}

		/// <summary>
		/// This method will create the BOL CB record for given profile and
		/// transaction PIDX record.
		/// </summary>
		/// <param name="profile">
		/// PIDX provider whom to send the transaction
		/// </param>
		/// <param name="pidxDO">
		/// transaction to send
		/// </param>
		/// <returns>
		/// success status
		/// </returns>
		private bool CreateCb(PIDXProfileClass profile, TransactionPIDXDO pidxDO)
		{
			bool successful = true;

			switch (profile.Type)
			{
				case PIDXType.Tds:
					{
						this.bolRecord = new BOLCBRecord();
						break;
					}

				case PIDXType.Dtn:
					{
						this.bolRecord = new BOLCBRecordDTN();
						break;
					}
			}

			try
			{
				this.GetTransaction(pidxDO.TransactionGuid);

				try
				{
					PIDXProfileCompanyMapsClass pidxProfileCompanyMaps = new PIDXProfileCompanyMapsClass();
					PIDXProfileCompanyMapClass pidxProfileCompanyMap = pidxProfileCompanyMaps.Get(this.security,
																									pidxDO.PIDXProfileGuid,
																									pidxDO.CompanyPersonnelToShipToBillToGuid);

					string sellerID = pidxProfileCompanyMap.SellerID;
					string consigneeNumber = pidxProfileCompanyMap.ConsigneeNumber;
					string shipperID = pidxProfileCompanyMap.ShipperID;
					string carrierScacCode = this.transDO.SCACCode.DefaultIfNull(string.Empty).PadLeft(8, '0');
					string splc = this.site.SPLCCode;
					string termOp = profile.TerminalID;
					DateTimeOffset? transactionDateTime = this.transDO.TransactionDateTime;
					if (transactionDateTime != null)
					{
						DateTimeOffset transDate = TimeConverter.ToSiteTime(this.site, transactionDateTime.Value);

						this.bolRecord.SellerIDDigit = Convert.ToInt32(sellerID);
						this.bolRecord.ConsigneeNumber = consigneeNumber;
						this.bolRecord.FinalShipperIDDigit = Convert.ToInt32(shipperID);
						this.bolRecord.CarrierID = carrierScacCode;
						this.bolRecord.TruckNumberDigit = 0;
						this.bolRecord.SPLCCodeDigit = Convert.ToInt32(splc);
						this.bolRecord.TerminalOperatorDigit = Convert.ToInt32(termOp);
						this.bolRecord.ShippedDate = this.GetShipMMDDYY(transDate);
						this.bolRecord.BOLNumberDigit = this.TruncateDocNumber(this.transDO.DocumentNumber);

						// Gets the product information and stores it in the CB record.
						bool hasProducts = this.GetProductInfo(BolTypes.CB);

						// If there are no products associated with the CB, then delete the
						// item from the queue.
						if (hasProducts == false)
						{
							this.serviceRequest.Security = this.security;
							this.serviceRequest.PIDXRequestType = TransactionPIDXSR.PIDX_REQUEST_TYPES.DELETE_PIDX;
							this.serviceRequest.TransPIDXDO = pidxDO;

							TransactionPIDXProcessorClass txPIDXProcessor = new TransactionPIDXProcessorClass();
							txPIDXProcessor.Process(this.serviceRequest);
							successful = false;
						}
					}
					else
					{
						this.LogErrors(ErrMsg002);
					}
				}
				catch (Exception)
				{
					this.LogErrors(ErrMsg002);
					successful = false;
				}
			}
			catch (Exception ex)
			{
				this.LogErrors(ErrMsg004 + pidxDO.TransID + " " + ex.Message);
				successful = false;
			}

			return successful;

		}

		/// <summary>
		/// This method will create the BOL BL record for given profile and
		/// transaction PIDX record.
		/// </summary>
		/// <param name="profile">
		/// PIDX provider whom to send the transaction
		/// </param>
		/// <param name="pidxDO">
		/// transaction to send
		/// </param>
		/// <returns>
		/// success status
		/// </returns>
		private bool CreateBl(PIDXProfileClass profile, TransactionPIDXDO pidxDO)
		{
			bool successful = true;
			switch (profile.Type)
			{
				case PIDXType.Tds:
					{
						this.bolRecord = new BOLBLRecord();
						break;
					}

				case PIDXType.Dtn:
					{
						this.bolRecord = new BOLBLRecordDTN();
						break;
					}
			}

			try
			{
				this.GetTransaction(pidxDO.TransactionGuid);

				var timeConverter = new SiteTimeConverter(this.site);

				try
				{
					var pidxProfileCompanyMaps = new PIDXProfileCompanyMapsClass();
					PIDXProfileCompanyMapClass pidxProfileCompanyMap = pidxProfileCompanyMaps.Get(this.security, pidxDO.PIDXProfileGuid, pidxDO.CompanyPersonnelToShipToBillToGuid);
					DateTimeOffset transDate = timeConverter.ConvertToSiteTime(this.transDO.TransactionDateTime.Value);

					this.bolRecord.SellerIDDigit = Convert.ToInt32(pidxProfileCompanyMap.SellerID);
					this.bolRecord.FinalShipperIDDigit = Convert.ToInt32(pidxProfileCompanyMap.ShipperID);
					this.bolRecord.SPLCCodeDigit = Convert.ToInt32(this.site.SPLCCode);
					this.bolRecord.TerminalOperatorDigit = Convert.ToInt32(profile.TerminalID);
					this.bolRecord.TerminalControlNumber = this.site.TerminalControlNumber;
					this.bolRecord.ShipDayDigit = transDate.Day;
					this.bolRecord.BOLNumberDigit = this.TruncateDocNumber(this.transDO.DocumentNumber);

					// ReSharper disable PossibleNullReferenceException
					if (!string.IsNullOrEmpty(pidxDO.AuthorizationNumber))
					{
						(this.bolRecord as BOLBLRecord).AuthorizationNumberDigit = Convert.ToInt32(pidxDO.AuthorizationNumber);
						(this.bolRecord as BOLBLRecord).AuthorizedLoadDigit = 0;
					}
					else
					{
						(this.bolRecord as BOLBLRecord).AuthorizationNumberDigit = 0;
						(this.bolRecord as BOLBLRecord).AuthorizedLoadDigit = 1;
					}

					(this.bolRecord as BOLBLRecord).BOLVersionDigit = pidxDO.BOLVersion;

					if (this.transDO.RouteSchedule.FST != null)
					{
						(this.bolRecord as BOLBLRecord).StartLoadDateTime = timeConverter.ConvertToSiteTime(this.transDO.RouteSchedule.FST.Value);
					}
					else if (this.transDO.TimeIn != null)
					{
						(this.bolRecord as BOLBLRecord).StartLoadDateTime = timeConverter.ConvertToSiteTime(this.transDO.TimeIn.Value);
					}
					else
					{
						(this.bolRecord as BOLBLRecord).StartLoadDateTime = transDate;
					}

					if (this.transDO.TimeEnd != null)
					{
						(this.bolRecord as BOLBLRecord).EndLoadDateTime = timeConverter.ConvertToSiteTime(this.transDO.TimeEnd.Value);
					}
					else
					{
						(this.bolRecord as BOLBLRecord).StartLoadDateTime = transDate;
					}

					this.bolRecord.ConsigneeNumber = pidxProfileCompanyMap.ConsigneeNumber;

					var companies = new CompaniesClass();
					CompanyClass shipTo = companies.Get(this.security, this.transDO.ShipToCompanyGuid);
					(this.bolRecord as BOLBLRecord).DestinationState = shipTo.State;
					(this.bolRecord as BOLBLRecord).DestinationCity = shipTo.City;
					(this.bolRecord as BOLBLRecord).DestinationZipCode = shipTo.Zip;

					this.bolRecord.CarrierID = this.transDO.SCACCode;
					this.bolRecord.RackDriverID = this.transDO.OperatorID;
					(this.bolRecord as BOLBLRecord).VehicleNumber = this.transDO.DestinationEQ1.CompanyEquipmentID;
					(this.bolRecord as BOLBLRecord).VehicleType = EquipmentTypeClass.Type(this.transDO.DestinationEQ1.EquipmentType);
					if (this.transDO.DestinationEQ2.EquipmentGuid != Guid.Empty)
					{
						switch ((this.bolRecord as BOLBLRecord).VehicleType)
						//EQUIPMENT_TYPE.TRACTOR_TYPE)
						{
							case EQUIPMENT_TYPE.TANKER_TYPE:
							case EQUIPMENT_TYPE.TRACTOR_TYPE:
								(this.bolRecord as BOLBLRecord).ContainerNumber1 = this.transDO.DestinationEQ2.CompanyEquipmentID;
								if (this.transDO.DestinationEQ3.EquipmentGuid != Guid.Empty)
								{
									(this.bolRecord as BOLBLRecord).ContainerNumber2 = this.transDO.DestinationEQ3.CompanyEquipmentID;
								}

								if (this.transDO.DestinationEQ4.EquipmentGuid != Guid.Empty)
								{
									(this.bolRecord as BOLBLRecord).ContainerNumber3 = this.transDO.DestinationEQ4.CompanyEquipmentID;
								}
								break;
							default:
								// Do nothing
								break;
						}
					}

					(this.bolRecord as BOLBLRecord).PurchaseOrderNumber = this.transDO.PONumber ?? string.Empty;

					// ReSharper restore PossibleNullReferenceException

					// Gets the product information and stores it in the BL record.
					this.GetProductInfo(BolTypes.BL);
				}
				catch (Exception)
				{
					this.LogErrors(ErrMsg001);
					successful = false;
				}
			}
			catch (Exception ex)
			{
				this.LogErrors(ErrMsg004 + pidxDO.TransID + " " + ex.Message);
				successful = false;
			}

			return successful;
		}

		/// <summary>
		/// This method will truncate the document number to fit for the BOL.
		/// </summary>
		/// <param name="docNumber">
		/// Document number to possibly trim
		/// </param>
		/// <returns>
		/// Integer representation of document number
		/// </returns>
		private int TruncateDocNumber(string docNumber)
		{
			if (string.IsNullOrEmpty(docNumber))
			{
				return 0;
			}

			string maxBol = this.site.AutomaticBOLEndNumber;
			string bolNumber = docNumber;

			if (docNumber.Length > maxBol.Length)
			{
				bolNumber = docNumber.Remove(0, docNumber.Length - maxBol.Length);
			}

			return Convert.ToInt32(bolNumber);
		}

		/// <summary>
		/// This method gets PIDX Quantity Units
		/// </summary>
		/// <param name="units">
		/// Units to retrieve the PIDX indicator for
		/// </param>
		/// <returns>
		/// string measurementType
		/// </returns>
		private string GetPIDXMeasurementType(EngineeringUnit units)
		{
			string measurementType = "   "; // Default to something which will at least fill the space.
			switch (units)
			{
				case EngineeringUnit.FmvBlLiq:
				case EngineeringUnit.FmvBlOil:
					measurementType = "BBL";
					break;

				case EngineeringUnit.FmvImpGal:
				case EngineeringUnit.FmvUsGal:
					measurementType = "GAL";
					break;

				case EngineeringUnit.FmvLitre:
					measurementType = "LTR";
					break;
			}

			return measurementType;
		}

		/// <summary>
		/// This method gets PIDX Temperature Units
		/// </summary>
		/// <param name="units">
		/// Units to retrieve the PIDX indicator for
		/// </param>
		/// <returns>
		/// string measurementType
		/// </returns>
		private string GetPIDXTemperatureMeasurementType(EngineeringUnit units)
		{
			string temperatureMeasurementType = " ";
			switch (units)
			{
				case EngineeringUnit.FmtDegC:
					temperatureMeasurementType = "C";
					break;

				case EngineeringUnit.FmtDegF:
					temperatureMeasurementType = "F";
					break;
			}

			return temperatureMeasurementType;
		}

		/// <summary>
		/// This method gets all the product information from the transaction line items
		/// and adds the data to the BOL record object.
		/// </summary>
		/// <param name="bolType">
		/// Record type to generate
		/// </param>
		/// <returns>
		/// true:  the BOL record has products
		/// false: no products on this BOL
		/// </returns>
		private bool GetProductInfo(BolTypes bolType)
		{
			int blendOrAlterationIndicator = 0;                 // No blend or alteration
			bool hasProducts = false;
			int finishedProductBatchID = 0;

			if (this.transDO.LineItems != null)
			{
				foreach (LineItemDO lineItem in this.transDO.LineItems)
				{
					if (lineItem.Status != TransactionStatus.Completed)
					{
						continue;
					}

					if (lineItem.ProductGuid == null)
					{
						continue;
					}

					// Need to retrieve the product object for the selected product.
					ProductsClass products = new ProductsClass();
					ProductClass product = products.GetByProductAuthorizedCompanies(this.security, lineItem.ProductGuid, false);


					double gross = this.ConversionProductVolume(product, lineItem.Quantity.Gross);
					double net = this.ConversionProductVolume(product, lineItem.Quantity.Net);
					double temperature = this.ConversionProductTemperature(product, lineItem.Temperature.Value);
					double density;
					try
					{
						density = this.ConversionProductDensity(lineItem.Density.Value, product);
					}
					catch (Exception)
					{
						density = 0.0;
					}

					switch (bolType)
					{
						case BolTypes.BB:
							{
								const int NetTemperatureFlagDigit = 1; // Indicates net gallons

								// ReSharper disable once PossibleNullReferenceException
								(this.bolRecord as BOLBBRecord).AddBOLProduct(
									 this.GetPIDXProductCode(product),
									 blendOrAlterationIndicator,
									 gross,
									 net,
									 NetTemperatureFlagDigit);
								hasProducts = true;
								break;
							}

						case BolTypes.CB:
							{
								int creditIndicator = 0;  // Indicates a positive value
								const int NetTemperatureFlagDigit = 1; // Indicates net gallons

								if (this.transDO.ReversalType == TransactionDO.Reversal
								|| this.transDO.ReversalType == TransactionDO.ReversalWithUpdate /*
							|| this.transDO.ReversalType == TransactionDO.ExternalReverse*/)
								{
									creditIndicator = 1;  // Indicates a negative value
								}

							// ReSharper disable once PossibleNullReferenceException
							(this.bolRecord as BOLCBRecord).AddBolProduct(
								 this.GetPIDXProductCode(product),
								 blendOrAlterationIndicator,
								 gross,
								 net,
								 NetTemperatureFlagDigit,
								 creditIndicator);

								hasProducts = true;
								break;
							}

						case BolTypes.BL:
							{
								string grossCreditSign = " "; // Indicates a positive value
								string netCreditSign = " ";

								if (this.transDO.ReversalType == TransactionDO.Reversal
											|| this.transDO.ReversalType == TransactionDO.ReversalWithUpdate /*
                                    || this.transDO.ReversalType == TransactionDO.ExternalReverse*/)
								{
									grossCreditSign = "-"; // Indicates a negative value
									netCreditSign = "-";
								}

								finishedProductBatchID++;
								string componentContractNumber = string.Empty;
								string subCompanyID = string.Empty;

								// Finished Product
								string productCodeType = "F";

								if (string.IsNullOrEmpty(lineItem.AdditiveProfileID))
								{
									if (lineItem.ProductType == ProductClass.ProductTypeID(ProductType.ComponentProduct))
									{
										blendOrAlterationIndicator = 2;
									}
									else
									{
										blendOrAlterationIndicator = 3;
									}
								}
								else
								{
									if (lineItem.ProductType == ProductClass.ProductTypeID(ProductType.ComponentProduct))
									{
										blendOrAlterationIndicator = 0;
									}
									else
									{
										blendOrAlterationIndicator = 1;
									}
								}

								var transactionAliases = new TransactionAliasesClass();
								TransactionAliasClass transactionAlias = transactionAliases.Get(this.security, this.transDO.TransactionAliasGuid, false);

								if ( // ReSharper disable once PossibleNullReferenceException
									 !(this.bolRecord as BOLBLRecord).AddBOLProduct(
										  this.GetPIDXProductCode(product),
										  productCodeType,
										  string.Empty,
										  blendOrAlterationIndicator,
										  gross,
										  grossCreditSign,
										  net,
										  netCreditSign,
										  temperature,
												 this.GetPIDXTemperatureMeasurementType(transactionAlias.TemperatureUnits == EngineeringUnit.FmSiteUnits ? this.site.TemperatureUnits : transactionAlias.TemperatureUnits),
										  density,
												 this.GetPIDXMeasurementType(transactionAlias.VolumeUnits == EngineeringUnit.FmSiteUnits ? this.site.VolumeUnits : transactionAlias.VolumeUnits),
										  finishedProductBatchID,
										  componentContractNumber,
										  subCompanyID))
								{
									continue;
								}

								hasProducts = true;

								foreach (SubLineItemDO subLineItem in lineItem.SubLineItems)
								{
									product = products.Get(this.security, subLineItem.ProductGuid, false);

									gross = this.ConversionProductVolume(product, subLineItem.Quantity.Gross);
									net = this.ConversionProductVolume(product, subLineItem.Quantity.Net);
									try
									{
										temperature = this.ConversionProductTemperature(product, subLineItem.Temperature.Value);
									}
									catch (Exception ex)
									{
										_ = ex;
										temperature = 0.0;
									}
									try
									{
										density = this.ConversionProductDensity(subLineItem.Density.Value, product);
									}
									catch (Exception ex)
									{ 
										_ = ex;
										density = 0.0;
									}

									blendOrAlterationIndicator = 0;

									switch (product.ProductType)
									{
										case ProductType.ComponentProduct:
											productCodeType = "C";
											break;
										case ProductType.AdditizedProduct:
											productCodeType = "A";
											break;
									}

									string pidxMeasurementType =
										 this.GetPIDXMeasurementType(
											  (product.ProductType == ProductType.ComponentProduct)
														  ? (transactionAlias.VolumeUnits == EngineeringUnit.FmSiteUnits ? this.site.VolumeUnits : transactionAlias.VolumeUnits)
														  : (transactionAlias.AdditiveVolumeUnits == EngineeringUnit.FmSiteUnits ? this.site.AdditiveVolumeUnits : transactionAlias.AdditiveVolumeUnits));
									// ReSharper disable once PossibleNullReferenceException
									(this.bolRecord as BOLBLRecord).AddBOLProduct(
										 (product.ProductType == ProductType.AdditiveProduct) ? "ADD" : this.GetPIDXProductCode(product),
										 productCodeType,
										 (product.ProductType == ProductType.AdditiveProduct) ? product.ID : string.Empty,
									blendOrAlterationIndicator,
									gross,
									grossCreditSign,
									net,
									netCreditSign,
									temperature,
												this.GetPIDXTemperatureMeasurementType(transactionAlias.TemperatureUnits == EngineeringUnit.FmSiteUnits ? this.site.TemperatureUnits : transactionAlias.TemperatureUnits),
									density,
												pidxMeasurementType,
									finishedProductBatchID,
									componentContractNumber,
									subCompanyID);
								}

								break;
							}
					}
				}
			}

			// Set the product code to 0ZZZ if there were no products.
			if (hasProducts == false)
			{
				if (bolType == BolTypes.BB)
				{
					const int NetTemperatureFlagDigit = 1; // Indicates net gallons

					// ReSharper disable once PossibleNullReferenceException
					(this.bolRecord as BOLBBRecord).AddBOLProduct(
						 "ZZZ",
						 blendOrAlterationIndicator,
						 0.0,
						 0.0,
						 NetTemperatureFlagDigit);
				}
				else if (bolType == BolTypes.BL)
				{
					const double Gross = 0.0;
					const double Net = 0.0;
					const double Temperature = 0.0;
					const double Density = 0.0;
					const string ProductCodeType = "F";
					const string GrossCreditSign = " "; // Indicates a positive value
					const string NetCreditSign = " ";
					string componentContractNumber = string.Empty;
					string subCompanyID = string.Empty;

					// ReSharper disable once PossibleNullReferenceException
					(this.bolRecord as BOLBLRecord).AddBOLProduct(
						 "ZZZ",
						 ProductCodeType,
						 string.Empty,
					 blendOrAlterationIndicator,
					 Gross,
					 GrossCreditSign,
					 Net,
					 NetCreditSign,
					 Temperature,
					 this.GetPIDXTemperatureMeasurementType(this.site.TemperatureUnits),
					 Density,
					 this.GetPIDXMeasurementType(this.site.VolumeUnits),
					 finishedProductBatchID,
					 componentContractNumber,
					 subCompanyID);
				}
			}

			return hasProducts;
		}

		/// <summary>
		/// This method will convert the product volume from SI to whatever the transaction alias
		/// or, if defaulted, the site settings
		/// are set to. It will return the converted volume or if an error occurs, then the
		/// original volume is returned.
		/// </summary>
		/// <param name="productType">
		/// The product Type.
		/// </param>
		/// <param name="volume">
		/// volume from the transaction
		/// </param>
		/// <returns>
		/// The <see cref="double"/>.
		/// volume in units to be sent to PIDX
		/// </returns>
		private double ConversionProductVolume(ProductClass product, double volume)
		{
			double convertedVolume = volume;

			try
			{
				EngineeringUnit units;
				NumberFormatInfo format;
				var transactionAliases = new TransactionAliasesClass();
				TransactionAliasClass transactionAlias = transactionAliases.Get(this.security, this.transDO.TransactionAliasGuid, false);

				if (product.ProductType == ProductType.AdditiveProduct)
				{
					units = product.VolumeUnits;
					units = units == EngineeringUnit.FmSiteUnits ? transactionAlias.AdditiveVolumeUnits : units; // If product doesn't specify units, fall back to transaction alias
					units = units == EngineeringUnit.FmSiteUnits ? this.site.AdditiveVolumeUnits : units; // if alias doesn't specify units, fall back to site
					format = this.site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.ADDITIVE_VOLUME);
				}
				else
				{
					units = product.VolumeUnits;
					units = units == EngineeringUnit.FmSiteUnits ? transactionAlias.VolumeUnits : units; // If product doesn't specify units, fall back to transaction alias
					units = units == EngineeringUnit.FmSiteUnits ? this.site.VolumeUnits : units; // if alias doesn't specify units, fall back to site
					format = this.site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME);
				}

				var siVolume = new SIDouble(units, format, volume);

				convertedVolume = siVolume.Value;
			}
			catch (Exception ex)
			{
				this.LogErrors(ErrMsg006 + product.ProductType + " " + ex.Message);
			}

			return convertedVolume;
		}

		/// <summary>
		/// The conversion product temperature.
		/// </summary>
		/// <param name="temperature">
		/// The temperature.
		/// </param>
		/// <returns>
		/// The <see cref="double"/>.
		/// </returns>
		private double ConversionProductTemperature(ProductClass product, double temperature)
		{
			double convertedTemperature = temperature;

			try
			{
				var transactionAliases = new TransactionAliasesClass();
				TransactionAliasClass transactionAlias = transactionAliases.Get(this.security, this.transDO.TransactionAliasGuid, false);

				EngineeringUnit units;
				units = product.TemperatureUnits;
				units = units == EngineeringUnit.FmSiteUnits ? transactionAlias.TemperatureUnits : units; // If product doesn't specify units, fall back to transaction alias
				units = units == EngineeringUnit.FmSiteUnits ? this.site.TemperatureUnits : units; // if alias doesn't specify units, fall back to site

				NumberFormatInfo format = this.site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.TEMPERATURE);

				var siTemperature = new SIDouble(units, format, temperature);

				convertedTemperature = siTemperature.Value;
			}
			catch (Exception ex)
			{
				this.LogErrors(ErrMsg006 + " " + ex.Message);
			}

			return convertedTemperature;
		}

		/// <summary>
		/// This method will convert the product density from SI to Specific Gravity
		/// </summary>
		/// <param name="density">
		/// Density as actually stored in the database (in kg/l)
		/// </param>
		/// <returns>
		/// Density converted to Specific Gravity
		/// </returns>
		/// <remarks>
		/// The PIDX 4.01 specification specifies that the density be provided in Specific Gravity
		/// It also should be to three decimal places.
		/// </remarks>
		private double ConversionProductDensity(double density, ProductClass product)
		{
			double convertedDensity = density;

			try
			{
				var transactionAliases = new TransactionAliasesClass();
				TransactionAliasClass transactionAlias = transactionAliases.Get(this.security, this.transDO.TransactionAliasGuid, false);
				EngineeringUnit units = product.DensityUnits == EngineeringUnit.FmSiteUnits ? (transactionAlias.DensityUnits == EngineeringUnit.FmSiteUnits ? this.site.DensityUnits : transactionAlias.DensityUnits) : product.DensityUnits;
				NumberFormatInfo format = this.site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DENSITY);
				format.NumberDecimalDigits = 2;

				var siDensity = new SIDouble(units, format, density) { ReferenceTemperature = 28.0 * 5.0 / 9.0 };
				convertedDensity = siDensity.Value;
			}
			catch (Exception ex)
			{
				this.LogErrors(ErrMsg006 + " " + ex.Message);
			}

			return convertedDensity;
		}

		/// <summary>
		/// This method returns the PIDX product code for a given product.
		/// It will return an empty string if the PIDX product code is null.
		/// </summary>
		/// <param name="product"></param>
		/// <returns></returns>
		private string GetPIDXProductCode(ProductClass product)
		{
			if (string.IsNullOrEmpty(product.PIDXCode))
			{
				return "";
			}

			string paddedproduct = product.PIDXCode.PadRight(3, ' ');
			return paddedproduct.PadLeft(6, '0');
		}

		/// <summary>
		/// This method will return the transaction data object for a given transaction
		/// ID>
		/// </summary>
		/// <param name="transID"></param>
		/// <returns></returns>
		private void GetTransaction(Guid transactionGuid)
		{
			TransactionSR sr = new TransactionSR { Security = this.security, TransactionGuid = transactionGuid, ConvertUnits = false };

			TransactionProcessorClass transProcessor = new TransactionProcessorClass();
			this.transDO = transProcessor.Process(sr);

			foreach (LineItemDO lineItem in this.transDO.LineItems)
			{
				if (lineItem.BrokenBlend.HasValue && lineItem.BrokenBlend.Value)
				{
					EventLog eventLog = new EventLog("Application", ".", "PIDXBOLProcessor");
					eventLog.WriteEntry("Transaction LineItem has Broken Blend TransID = " + this.transDO.TransID, EventLogEntryType.Error);

					throw new Exception("LineItem has Broken Blend");
				}
			}

			foreach (TransactionPIDXDO transactionPidxDo in this.transDO.TransPIDXCollection)
			{
				if (transactionPidxDo.BrokenBlend)
				{
					EventLog eventLog = new EventLog("Application", ".", "PIDXBOLProcessor");
					eventLog.WriteEntry("Transaction PIDXDO has Broken Blend TransID = " + this.transDO.TransID, EventLogEntryType.Error);

					throw new Exception("PIDXDO has Broken Blend");
				}
			}
		}


		/// <summary>
		/// This method will updatee transaction data object for a given transaction
		/// ID>
		/// </summary>
		/// <returns></returns>
		private void UpdateTransaction()
		{
			if (this.transDO.Status == TransactionStatus.Posted)
			{
				return;
			}

			this.transDO.Status = TransactionStatus.Posted;

			SaveTransactionsSR sr = new SaveTransactionsSR { UseAutoComplete = true, Security = this.security };
			sr.Transactions.Add(this.transDO);
			sr.ConvertUnits = false;

			SaveTransactionsProcessor saveTxProcessor = new SaveTransactionsProcessor();
			saveTxProcessor.SaveTransactions(sr);
		}



		/// <summary>
		/// This method will read the transaction PIDX queue for any records
		/// that has not been sent to the PIDX services. It returns true if there
		/// are records to be sent.  Otherwise, it returns false.
		/// </summary>
		/// <returns></returns>
		private bool ReadQueue()
		{
			bool hasData = false;
			TransactionPIDXCollectionDO transPidxCollection = null;

			this.serviceRequest.Security = this.security;
			this.serviceRequest.PIDXRequestType = TransactionPIDXSR.PIDX_REQUEST_TYPES.GET_PIDX_BOL;

			try
			{
				TransactionPIDXProcessorClass transPIDXProcessor = new TransactionPIDXProcessorClass();
				transPidxCollection = transPIDXProcessor.Process(this.serviceRequest);
			}
			catch (Exception ex)
			{
				this.LogErrors(ex.Message);
			}

			if (transPidxCollection != null)
			{
				this.transPidxDOList = transPidxCollection.TransactionPIDXDOList;

				if (this.transPidxDOList.Count > 0)
				{
					hasData = true;
				}
			}

			return hasData;
		}

		/// <summary>
		/// This method will update the queue sent flag for the PIDX BOLs that
		/// were sent to the service.
		/// </summary>
		/// <param name="pidxDO"></param>
		private void UpdateSentFlag(TransactionPIDXDO pidxDO)
		{
			TransactionPIDXCollectionDO transPidxCollection = new TransactionPIDXCollectionDO();
			transPidxCollection.Add(pidxDO);

			this.serviceRequest.Security = this.security;
			this.serviceRequest.PIDXRequestType = TransactionPIDXSR.PIDX_REQUEST_TYPES.UPDATE_SENT;
			this.serviceRequest.TransactionPidxDOCollection = transPidxCollection;

			TransactionPIDXProcessorClass transPIDXProcessor = new TransactionPIDXProcessorClass();
			transPIDXProcessor.Process(this.serviceRequest);
		}

		/// <summary>
		/// This method will retrieve the associated profile for the PIDX BOL being sent.
		/// </summary>
		/// <param name="profileGuid"></param>
		/// <returns></returns>
		private PIDXProfileClass GetPIDXProfile(Guid profileGuid)
		{
			PIDXProfileClass profile;

			try
			{
				PIDXProfilesClass profiles = new PIDXProfilesClass();
				profile = profiles.Get(this.security, profileGuid, false);
			}
			catch (Exception ex)
			{
				throw new Exception(ErrMsg003 + profileGuid.ToString() + " " + ex.Message);
			}

			return profile;
		}

		/// <summary>
		/// This method will log errors in the accounting log file and/or event log.
		/// </summary>
		/// <param name="msg"></param>
		private void LogErrors(string msg)
		{
			this.logger.Error(msg);
		}
		#endregion
	}
}
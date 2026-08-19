using System;
using System.Text;
using System.Data;
using System.Xml;
using System.Reflection;
using System.Diagnostics;
using System.Collections;

namespace FuelsManager.Accounting
{
    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;
    using FMBusinessObjects.UtilityObjects;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text.RegularExpressions;
	using Varec.CommonComponents.EngineeringUnitsLibrary;

    internal enum NameHeader //store the NameHeader in the XML for each transaction type number
    {
        UnDefined = 0,
        PrimaryAdjustment = 1,  // Adjustment
        SecondaryAdjustment = 2,
        PrimaryDefuel = 3,
        SecondaryDefuel = 4, // Defuel/LR Receipt
        PrimaryDisbursement = 5,  // Issue/Bulk Issue
        SecondaryDisbursement = 6,
        FillStand = 7,  // Load Rack
        Receipt = 8,  // Receipt
        Request = 9,
        Unload = 10,
        CustomerTransfer = 11,
        MeterMovement = 12,  // Rotation
        OwnerTransfer = 13,
        PhysicalInventory = 14,
        PrimaryRegrade = 15
    };

	internal class WriteRecordToXml
    {
		//TimeSpan utcOffset;
		private SecurityClass security;
		private SiteClass currentSite;
		static protected DateTime BeginningOfTime = new DateTime(1900, 1, 1);
		protected DateTime CurrentTime;
		protected TimeZone CurrentTimeZone;
		protected TimeSpan OffsetFromLocalTimeZoneToGMT;

		static Regex regExHMM = new Regex(@"^\d{3}$");
		static Regex regExHHMM = new Regex(@"^\d{4}$");
		static Regex regExHColonMM = new Regex(@"^\d:\d\d$");
		static Regex regExHHColonMM = new Regex(@"^\d\d:\d\d$");

		private Dictionary<string, string> volumeUnitsTranslation;
		private Dictionary<string, string> temperatureUnitsTranslation;
		private Dictionary<string, string> densityUnitsTranslation;
		private Dictionary<string, string> massUnitsTranslation;

		public WriteRecordToXml(SecurityClass security)
		{
			this.security = security;
			this.currentSite = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(this.security, this.security.SiteGuid, false, false, false));

			this.volumeUnitsTranslation = new Dictionary<string, string>()
			{
				{ "L", EngineeringUnit.FmvLitre.ToString() },
				{ "G", EngineeringUnit.FmvUsGal.ToString() }
			};

			this.temperatureUnitsTranslation = new Dictionary<string, string>()
			{
				{ "C", EngineeringUnit.FmtDegC.ToString() },
				{ "F", EngineeringUnit.FmtDegF.ToString() }
			};

			this.densityUnitsTranslation = new Dictionary<string, string>()
			{
				{ "dAPI", EngineeringUnit.FmdDegApi.ToString() },
				{ "kgm3", EngineeringUnit.FmdKgM3.ToString() },
				{ "lbg", EngineeringUnit.FmdUsLbGal.ToString() }
			};

			this.massUnitsTranslation = new Dictionary<string, string>()
			{
				{ "lbs", EngineeringUnit.FmmLb.ToString() },
				{ "kg", EngineeringUnit.FmmKg.ToString() }
			};

			this.CurrentTime = DateTime.Now;
			this.CurrentTimeZone = TimeZone.CurrentTimeZone;
			this.OffsetFromLocalTimeZoneToGMT = this.CurrentTimeZone.GetUtcOffset(this.CurrentTime);
		}

		public void WriteRecord(Stream xmlStream, string feedName, DataSet oDataSetRecord, SortedList allProducts, Dictionary<string, string> allTransactions, List<TransactionValidationResult> parseValidationResults, List<string> duplicateTransactions)
        {
			var aliasNameList = FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasNameCollectionClass>(x => x.EnumerateNamesOnly(this.security, false));

			TransactionValidationResult validationResult;

			var oXmlSettings = new XmlWriterSettings()
			{
				Indent = true
			};
			var oXML = XmlWriter.Create(xmlStream, oXmlSettings);

            try
            {
                oXML.WriteStartDocument();
				this.WriteTransactionHeader(oXML, feedName);

                oXML.WriteStartElement("TrxnS");

                oXML.WriteAttributeString("Count", null, (oDataSetRecord.Tables[0].Rows.Count).ToString());

                //write each record to the file
                foreach (DataTable thisTable in oDataSetRecord.Tables)
                {
                    foreach (DataRow dbRow in thisTable.Rows)
                    {
                        try
						{
                            // Get TransID if provided, if not, generate GUID (TransID has to be unique in DB)
                            var transId = string.IsNullOrEmpty(dbRow["ID"].ToString()) ? FuelsManagerId.NewId().ToString() : dbRow["ID"].ToString();

                            // Get Document/Ticket Number temporarily to pass to TRANSACTION_TYPE validation
                            // Use GBL as DocumentNumber if type is Receipt
                            string documentNumber = string.IsNullOrEmpty(dbRow["TICKET_NUMBER"].ToString()) || dbRow["TRANSACTION_TYPE"].ToString().ToUpper() == "RECEIPT" ? dbRow["GBL"].ToString() : dbRow["TICKET_NUMBER"].ToString();

                            // Set validationResult with TransID (DocumentNumber used instead to show in results window)
                            validationResult = new TransactionValidationResult
                            {
                                // use DocumentNumber as the key for results information
                                TransID = documentNumber
                            };

                            // Add TransID and DocumentNumber to all transaction dictionary, used for results view
                            allTransactions.Add(transId, documentNumber);

                            // Validation: Need to ensure a DocumentNumber has been provided
                            if (string.IsNullOrEmpty(documentNumber))
                            {
                                validationResult.ErrorList.Add("Ticket Number or GBL required");
                                if (!string.IsNullOrEmpty(dbRow["ID"].ToString()))
                                {
                                    validationResult.TransID = transId;
                                }
                                parseValidationResults.Add(validationResult);
                                continue;
                            }

                            // Validation: Need to ensure that site is valid
                            var site = dbRow["SITE"].ToString();
                            var siteGuid = FMChannelHelper.MakeCall<ISites, Guid>(sites => sites.GetIdentityGuid(this.security, site));
                            if (siteGuid == Guid.Empty)
                            {
                                validationResult.ErrorList.Add("Unrecognized site '" + site + "'");
                                parseValidationResults.Add(validationResult);
                                continue;
                            }

                            // Validation: Need to ensure that logged in user has access to the site
                            if (FMChannelHelper.MakeCall<IEntityToSiteMaps, bool>(entityToSiteChannel =>
                                    entityToSiteChannel.IsAssigned(this.security,
                                        ENTITY_TYPE.USER,
                                        siteGuid, this.security.UserGuid)) == false)
                            {
                                validationResult.ErrorList.Add("Logged in user is not authorized for site '" + site + "'");
                                parseValidationResults.Add(validationResult);
                                continue;
                            }

                            // Validation: Need to ensure the transaction type id valid
                            var recordAlias = dbRow["TRANSACTION_TYPE"].ToString();
                            var aliasName = aliasNameList.Find(x => x.AliasName.ToUpper() == recordAlias.ToUpper());
                            if (aliasName == null)
                            {
                                validationResult.ErrorList.Add("Unrecognized transaction type '" + recordAlias + "'");
                                parseValidationResults.Add(validationResult);
                                continue;
                            }
                            // Get TransactionType
                            var transTypeId = aliasName.TransTypeID;
                            NameHeader nTransType = (NameHeader)((int)transTypeId);

                            // Reversal Type
                            var reversalType = string.IsNullOrEmpty(dbRow["REVERSAL_TYPE"].ToString()) ? "O" : dbRow["REVERSAL_TYPE"].ToString().ToUpper();

                            // Delete Flag
                            var deleteFlag = dbRow["DELETE_FLAG"].ToString().ToUpper() == "T" ? "T" : "F";

                            // Validation: Need to ensure transaction doesn't exist by SiteGUID/DocumentNumber key or TransID
                            var getTransactionSr = new GetTransactionSR
                            {
                                Request = GetTransactionRequest.SITE_DOCUMENTNUMBER,
                                Security = this.security,
                                Site = siteGuid.ToString(),
                                DocumentNumber = documentNumber,
                                TransId = transId
                            };
                            var getTransactionDo = FMChannelHelper.MakeCall<IGetTransactionProcessor, GetTransactionDO>(x => x.Process(getTransactionSr));
                            // A transaction should be rejected as duplicate if it already exists in the database,
                            //    the reversal type is 'O' for 'Original',
                            //    and the deletion flag is 'F'
                            // Passing in 'R', 'U', or DeleteFlag='T' should check for existing transaction, else fail
                            //
                            // Check for existing transaction
                            bool duplicateTran = getTransactionDo.TransactionDataSet.Tables[0].Rows.Count > 0;
                            if (duplicateTran)
                            {
                                // If TransID exists but DocumentNumber is different then add to fail list and end processing
                                if (documentNumber != getTransactionDo.TransactionDataSet.Tables[0].Rows[0]["DocumentNumber"].ToString())
                                {
                                    validationResult.ErrorList.Add("An existing ID was found with a different Ticket Number or GBL");
                                    parseValidationResults.Add(validationResult);
                                    continue;
                                }

                                // If duplicate transaction, get the TransID from DB and assign back to this transaction
                                if (string.IsNullOrEmpty(dbRow["ID"].ToString()))
                                {
                                    transId = getTransactionDo.TransactionDataSet.Tables[0].Rows[0]["TransID"].ToString();
                                }
                                else
                                {
                                    // same DocumentNumber provided but TransID was different, add to fail list and end processing
                                    if (transId != getTransactionDo.TransactionDataSet.Tables[0].Rows[0]["TransID"].ToString())
                                    {
                                        validationResult.ErrorList.Add("An existing transation was found with a different ID");
                                        parseValidationResults.Add(validationResult);
                                        continue;
                                    }
                                }

                                // Transaction is a duplicate, add to the duplicate list and end processing
                                if (reversalType == "O" && deleteFlag == "F")
                                {
                                    duplicateTransactions.Add(documentNumber);
                                    continue;
                                }
                            }
                            else
                            {
                                // If reversal type or delete flag, it requires an existing transaction, add to fail list and end processing 
                                if ((reversalType == "R" || reversalType == "U" || deleteFlag == "T"))
                                {
                                    validationResult.ErrorList.Add((deleteFlag == "T" ? "Updating Delete Flag" : "Reversal Type '" + reversalType + "'") + " requires an existing transaction");
                                    parseValidationResults.Add(validationResult);
                                    continue;
                                }
                            }

                            // Reversal/Update types.
                            // If 'R', soft delete the transaction, if 'U', overwrite the data
                            if (reversalType == "R")
                            {
                                deleteFlag = "T";
                                reversalType = "O";
                            }
                            if (reversalType == "U")
                            {
                                reversalType = "O";
                            }

							oXML.WriteStartElement(nTransType.ToString());
							try
							{
                                // If TransID is blank, generate and use GUID
                                oXML.WriteElementString("ID", transId);

								// Site
								oXML.WriteElementString("Site", site);

								// Transaction Status.  Assume that only Completed transactions will be uploaded
								oXML.WriteElementString("TransactionStatus", "Completed");

								// Transaction Alias
								oXML.WriteElementString("TransactionAlias", recordAlias);

								// Transaction Type
								oXML.WriteElementString("TransTypeID", string.Format("Transaction Type {0:D}", (int)transTypeId));

								// Inventory Date
								var inventoryDateString = dbRow["INVENTORY_DATE"].ToString();
                                // Just pass the string straight through and let the XMLImportProcessor deal with it
                                var dateTimeFormatInfo = new DateTimeFormatInfo
                                {
                                    ShortDatePattern = this.currentSite.ShortDatePattern,
                                    ShortTimePattern = this.currentSite.TimePattern
                                };
                                DateTime inventoryDate;
								try
								{
									inventoryDate = DateTime.Parse(inventoryDateString, dateTimeFormatInfo);
								}
								catch (FormatException)
								{
									inventoryDate = DateTime.Today.Date;
								}
								this.WriteElementDate(oXML, "InventoryDate", inventoryDate);

								// Transaction Date/Time
								var transDateTime = dbRow["TRANSACTION_DATE"].ToString();

								// Just pass the string straight through and let the XML Import Processor deal with it.
								this.WriteElementDateTime(oXML, "TrxnDateTime", transDateTime, inventoryDate);

                                if (transTypeId == TransactionTypes.T8_Receipt)
                                {
                                    oXML.WriteElementString("DocumentNumber", dbRow["GBL"].ToString());
                                }
                                else
                                {
                                    oXML.WriteElementString("DocumentNumber", dbRow["TICKET_NUMBER"].ToString());
                                }

                                // reversal transaction ID
                                oXML.WriteElementString("ReversedTransactionID", "");
                                oXML.WriteElementString("ReversalType", reversalType);

                                // Manager
                                var manager = dbRow["MANAGER"].ToString();
								var toManager = dbRow["TO_MANAGER"].ToString();
								switch (transTypeId)
								{
									case TransactionTypes.T13_OwnerTransfer:
										oXML.WriteStartElement("FromManager");
										oXML.WriteElementString("Name", manager);
										oXML.WriteElementString("Code", "");
										oXML.WriteEndElement(); // "FromOwner"
										oXML.WriteStartElement("ToManager");
										oXML.WriteElementString("Name", toManager);
										oXML.WriteElementString("Code", "");
										oXML.WriteEndElement(); // "ToOwner"
										break;
									default:
										oXML.WriteStartElement("Manager");
										oXML.WriteElementString("Name", manager);
										oXML.WriteElementString("Code", "");
										oXML.WriteEndElement(); // "Owner"
										break;
								}

								// Notes
								var notes = dbRow["NOTES"].ToString();
								oXML.WriteElementString("Notes", notes);

								// User fields
								oXML.WriteStartElement("UserFields");
								try
								{
									this.WriteUserDataElementWithValue(oXML, dbRow["FLIGHT_TYPE"].ToString(), 2);
									switch (transTypeId)
									{
										case TransactionTypes.T3_PrimaryDefuel:
										case TransactionTypes.T4_SecondaryDefuel:
										case TransactionTypes.T5_PrimaryDisbursement:
										case TransactionTypes.T6_SecondaryDisbursement:
											this.WriteUserDataElementWithValue(oXML, dbRow["REQUIRED_LOAD"].ToString(), 4);
											this.WriteUserDataElementWithValue(oXML, dbRow["ARRIVAL_LOAD"].ToString(), 5);
											this.WriteUserDataElementWithValue(oXML, dbRow["FINAL_LOAD"].ToString(), 6);
											this.WriteUserDataElementWithValue(oXML, dbRow["FUELING_START_TIME"].ToString(), 7);
											this.WriteUserDataElementWithValue(oXML, dbRow["FUELING_STOP_TIME"].ToString(), 8);
											break;
										case TransactionTypes.T8_Receipt:
											this.WriteUserDataElementWithValue(oXML, dbRow["SHIPPING_MODE"].ToString(), 4);
											break;
									}
									this.WriteUserDataElementWithValue(oXML, dbRow["CONTRACT_NUMBER"].ToString(), 16);
								}
								finally
								{
									oXML.WriteEndElement(); // "UserFields"
								}

								// Owner
								var owner = dbRow["OWNER"].ToString();
								var toOwner = dbRow["TO_OWNER"].ToString();
								switch (transTypeId)
								{
									case TransactionTypes.T14_PhysicalInventory:
										// Owner does not apply for Physical Inventory
										break;
									case TransactionTypes.T13_OwnerTransfer:
										oXML.WriteStartElement("FromOwner");
										oXML.WriteElementString("Name", owner);
										oXML.WriteElementString("Code", "");
										oXML.WriteEndElement(); // "FromOwner"
										oXML.WriteStartElement("ToOwner");
										oXML.WriteElementString("Name", toOwner);
										oXML.WriteElementString("Code", "");
										oXML.WriteEndElement(); // "ToOwner"
										break;
									default:
										oXML.WriteStartElement("Owner");
										oXML.WriteElementString("Name", owner);
										oXML.WriteElementString("Code", "");
										oXML.WriteEndElement(); // "Owner"
										break;
								}

								// Ship To/Consumer
								var shipTo = dbRow["CONSUMER"].ToString();
								oXML.WriteStartElement("ShipTo");
								oXML.WriteElementString("Name", shipTo);
								oXML.WriteElementString("Code", "");
								oXML.WriteEndElement(); // "ShipTo"

								// Carrier/Vendor
								var carrier = dbRow["VENDOR"].ToString();
								switch (transTypeId)
								{
									case TransactionTypes.T14_PhysicalInventory:
										// Carrier does not apply for Physical Inventory
										break;
									case TransactionTypes.T13_OwnerTransfer:
										// Import currently does not support from/to carriers on transfers
										oXML.WriteStartElement("FromCarrier");
										oXML.WriteElementString("Name", carrier);
										oXML.WriteElementString("Code", "");
										oXML.WriteEndElement(); // "FromCarrier"
										oXML.WriteStartElement("ToCarrier");
										oXML.WriteElementString("Name", carrier);
										oXML.WriteElementString("Code", "");
										oXML.WriteEndElement(); // "ToCarrier"
										break;
									default:
										oXML.WriteStartElement("Carrier");
										oXML.WriteElementString("Name", carrier);
										oXML.WriteElementString("Code", "");
										oXML.WriteEndElement(); // "Carrier"
										break;
								}

								// Supplier
								var supplier = dbRow["SUPPLIER"].ToString();
								oXML.WriteStartElement("Supplier");
								oXML.WriteElementString("Name", supplier);
								oXML.WriteElementString("Code", "");
								oXML.WriteEndElement(); // "Supplier"

								// Route data/International flagging
								var routeIndicator = dbRow["INTERNATIONAL_INDICATION"].ToString().ToUpper();
								oXML.WriteStartElement("RouteData");
                                oXML.WriteElementString("RouteID", dbRow["FLIGHT_NUMBER"].ToString());
                                oXML.WriteElementString("RouteOriginationDate", dbRow["FLIGHT_DATE"].ToString());
                                oXML.WriteElementString("InternationalRoute", (routeIndicator == "DOM" ? "False" : "True"));
								oXML.WriteElementString("NextStation", dbRow["DESTINATION"].ToString());
								oXML.WriteEndElement(); // "RouteData"

								// Location
								oXML.WriteElementString("Location", "");

								// Write out delete flag
								oXML.WriteElementString("DeleteFlag", (deleteFlag == "T" ? "True" : "False"));

								// Aviation Gauge Readings
								if (transTypeId == TransactionTypes.T5_PrimaryDisbursement || transTypeId == TransactionTypes.T6_SecondaryDisbursement)
								{
									oXML.WriteStartElement("AviationGaugeReadings");
									try
									{
										oXML.WriteStartElement("Compartment");
										try
										{
											oXML.WriteElementString("Name", "");
											oXML.WriteElementString("BeginQuantity", dbRow["ARRIVAL_LOAD"].ToString());
											oXML.WriteElementString("RequestedQuantity", dbRow["REQUIRED_LOAD"].ToString());
											oXML.WriteElementString("FinalQuantity", dbRow["FINAL_LOAD"].ToString());
										}
										finally
										{
											oXML.WriteEndElement(); // Compartment
										}

										string massUnit;
										try
										{
											massUnit = this.massUnitsTranslation[dbRow["LOAD_UNITS"].ToString()];
										}
										catch (KeyNotFoundException)
										{
											massUnit = EngineeringUnit.FmSiteUnits.ToString();
										}
										oXML.WriteElementString("QuantityUnits", massUnit);
									}
									finally
									{
										oXML.WriteEndElement(); // AviationGaugeReadings
									}
								}

                                // Write OPERATOR at transaction header level as well. The importer only looks for this one
                                oXML.WriteElementString("Operator", dbRow["OPERATOR"].ToString());

                                // Line Items
                                // Note that this importer only supports single line item transactions
                                oXML.WriteStartElement("LineItems");
								try
								{
									oXML.WriteStartElement("LineItem");

									try
									{
										// Only one line item, so sequence is always "1"
										oXML.WriteElementString("SequenceNumber", "1");

										oXML.WriteStartElement("AccountingData");
										try
										{
											oXML.WriteStartElement("Quantity");
											try
											{
												oXML.WriteElementString("Gross", dbRow["GROSS_VOLUME"].ToString());
												oXML.WriteElementString("Net", dbRow["NET_VOLUME"].ToString());
												string volumeUnit;
												try
												{
													volumeUnit = this.volumeUnitsTranslation[dbRow["VOLUME_UNITS"].ToString()];
												}
												catch (KeyNotFoundException)
												{
													volumeUnit = EngineeringUnit.FmSiteUnits.ToString();
												}
												oXML.WriteElementString("Units", volumeUnit);

											}
											finally
											{
												oXML.WriteEndElement(); // "Quantity"
											}	

											oXML.WriteElementString("VCF", dbRow["VCF"].ToString());

											oXML.WriteElementString("Temperature", dbRow["TEMPERATURE"].ToString());
											string temperatureUnit;
											try
											{
												temperatureUnit = this.temperatureUnitsTranslation[dbRow["TEMPERATURE_UNIT"].ToString()];
											}
											catch (KeyNotFoundException)
											{
												temperatureUnit = EngineeringUnit.FmSiteUnits.ToString();
											}
											oXML.WriteElementString("TemperatureUnits", temperatureUnit);

											oXML.WriteElementString("Density", dbRow["GRAVITY"].ToString());
											string densityUnit;
											try
											{
												densityUnit = this.densityUnitsTranslation[dbRow["GRAVITY_UNITS"].ToString()];
											}
											catch (KeyNotFoundException)
											{
												densityUnit = EngineeringUnit.FmSiteUnits.ToString();
											}
											oXML.WriteElementString("DensityUnits", densityUnit);

											oXML.WriteElementString("Custom", "");
										}
										finally
										{
											oXML.WriteEndElement(); // "AccountingData"
										}										

										oXML.WriteStartElement("ProductInfo");
										try
										{
											oXML.WriteElementString("ProductCode", dbRow["PRODUCT_ID"].ToString());
											string productString;
											try
											{
												productString = allProducts[dbRow["PRODUCT_ID"].ToString()].ToString();
											}
											catch (NullReferenceException)
											{
												productString = string.Empty;
											}
											oXML.WriteElementString("Product", productString);
										}
										finally
										{
											oXML.WriteEndElement(); // ProductInfo
										}

										oXML.WriteElementString("Operator", dbRow["OPERATOR"].ToString());

                                        oXML.WriteElementString("DocumentNumber", documentNumber);

										oXML.WriteStartElement("ToEquipment");
										try
										{
											oXML.WriteStartElement("Equipment1");
											try
											{
												oXML.WriteElementString("RegistrationID", dbRow["AIRCRAFT_REGISTRATION_ID"].ToString());
                                                if (transTypeId != TransactionTypes.T3_PrimaryDefuel && transTypeId != TransactionTypes.T4_SecondaryDefuel)
                                                {
                                                    oXML.WriteElementString("EquipmentModel", dbRow["AIRCRAFT_TYPE"].ToString());
                                                }
											}
											finally
											{
												oXML.WriteEndElement(); // Equipment1
											}			
										}
										finally
										{
											oXML.WriteEndElement(); // ToEquipment
										}		

										oXML.WriteStartElement("FromEquipment");
										try
										{
											oXML.WriteStartElement("Equipment1");
											try
											{
												oXML.WriteElementString("RegistrationID", dbRow["REGISTRATION_ID"].ToString());
                                                if (transTypeId == TransactionTypes.T3_PrimaryDefuel || transTypeId == TransactionTypes.T4_SecondaryDefuel)
                                                {
                                                    oXML.WriteElementString("EquipmentModel", dbRow["AIRCRAFT_TYPE"].ToString());
                                                }
                                            }
											finally
											{
												oXML.WriteEndElement(); // Equipment1
											}		
										}
										finally
										{
											oXML.WriteEndElement(); // FromEquipment
										}	

										oXML.WriteStartElement("MeterReadings");
										try
										{
											oXML.WriteElementString("MeterStart", dbRow["METER_START"].ToString());
											oXML.WriteElementString("MeterStop", dbRow["METER_STOP"].ToString());
											oXML.WriteElementString("StartDateTime", "");
											oXML.WriteElementString("StopDateTime", "");
										}
										finally
										{
											oXML.WriteEndElement(); // MeterReadings
										}		
									}
									finally
									{
										oXML.WriteEndElement(); // "LineItem"
									}
								}
								finally
								{
									oXML.WriteEndElement(); // "LineItems"
								}	
							}
							finally
							{
								oXML.WriteEndElement(); // $"{nTransType.ToString()}"
								oXML.Flush();
							}
						}
						catch (Exception ex)
						{
                            validationResult = new TransactionValidationResult();
                            validationResult.ErrorList.Add(ex.Message);
							parseValidationResults.Add(validationResult);
                            continue;
						}
					}
                }
            }
            catch (Exception writexmlError)
            {
                Console.WriteLine("Error: " + writexmlError.InnerException);
            }
            finally
            {
                oXML.WriteEndElement(); // TrxnS
				oXML.WriteEndDocument();
                oXML.Flush();
                oXML.Close();
            }
        }

        private void WriteTransactionHeader(XmlWriter oXML, string fileName)
        {
			TimeZoneInfo timeZoneInfo;
			try
			{
				timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(this.currentSite.TimeZone);
			}
			catch (Exception)
			{
				timeZoneInfo = TimeZoneInfo.Local;
			}

			oXML.WriteStartElement("TransactionFeed");
            oXML.WriteAttributeString("DataVersion", null, "9.2");

            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            oXML.WriteAttributeString("StandardTransactionImportInterfaceVersion", null, fvi.FileVersion);

            oXML.WriteStartElement("Header");
            oXML.WriteElementString("FeedName", fileName);

            oXML.WriteStartElement("ContactInfo");
            oXML.WriteElementString("Name", "");
            oXML.WriteElementString("Telephone", "");
            oXML.WriteElementString("Email", "");
            oXML.WriteEndElement(); // ContactInfo

            oXML.WriteElementString("DateTimeSent", "");
            oXML.WriteElementString("HoursDifferenceFromGMT", timeZoneInfo.GetUtcOffset(DateTime.Now).Hours.ToString());
            oXML.WriteElementString("DaylightSavingsIndicator", (timeZoneInfo.IsDaylightSavingTime(DateTime.Now) ? 1 : 0).ToString());
            oXML.WriteEndElement(); // Header
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="elementName">Output XML element name</param>
		/// <param name="fromColumn">column name for a column with a type of DateTime, DateTimeOffset or String, assumes a limited number or formats for string</param>
		/// <param name="dbRow"></param>
		/// <param name="defColumn"></param> PGT 3.23.16 Default field to use in case fromColumn is empty. Must be the same field type as fromColumn field.
		public void WriteElementDate(XmlWriter xmlWriter, string elementName, object dataValue)
		{
			if (dataValue != null && dataValue != DBNull.Value)
			{
				try
				{
					if (dataValue is DateTime)
					{
						DateTime localTime = (DateTime)dataValue;
						this.WriteElementDate(xmlWriter, elementName, localTime);
						return;
					}
					if (dataValue is DateTimeOffset)
					{
						DateTimeOffset dateTimeOffset = (DateTimeOffset)dataValue;
						DateTime tm = new DateTime(dateTimeOffset.Year, dateTimeOffset.Month, dateTimeOffset.Day, dateTimeOffset.Hour, dateTimeOffset.Minute, dateTimeOffset.Second);
						this.WriteElementDate(xmlWriter, elementName, tm);
						return;
					}
					if (dataValue is string)
					{
						DateTime date;
						if (StandardizeDate(dataValue as string, out date))
						{
							this.WriteElementDate(xmlWriter, elementName, date);
							return;
						}
					}
				}
				catch (Exception e)
				{
					Debug.WriteLine(e.ToString());
					// whatever was in that column was not a date, just leave it blank;
				}
			}
			//   No need to output an empty date time value
			//  WriteElementString(elementName, "");
		}

		/// <summary>
		/// Formats and write XML element for DateTime elements
		/// </summary>
		/// <param name="elementName">XML element name</param>
		/// <param name="dateTime">value of element</param>
		/// <param name="timeZoneOffset">offset to GMT written as an attribute</param>
		protected void WriteElementDate(XmlWriter xmlWriter, string elementName, DateTime date)
		{
			xmlWriter.WriteStartElement(elementName);
			if (date >= BeginningOfTime)
			{
				string formattedDate = date.ToString("yyyy-MM-dd");
				xmlWriter.WriteString(formattedDate);
			}
			xmlWriter.WriteEndElement();
		}

		/// <summary>
		/// Parse out a string with a time and/or date from selected formats
		/// </summary>
		/// <param name="timeString">INput time</param>
		/// <param name="defaultDate">When timeString does not contain a date, use the date portion of this as a default</param>
		/// <param name="timezoneOffset">Time zone offset betweeen return value and GMT</param>
		/// <returns>True if timeString was successfully parsed</returns>
		internal static bool StandardizeDate(string timeString, out DateTime date)
		{
			// Is timeString a date and time or only a time?  
			// To be treated as a date and time it has to have date on or after the beginning of time and a colon
			if (DateTime.TryParse(timeString, out date) && date >= BeginningOfTime)
			{
				// no conversion needed
			}
			else
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="elementName">Output XML element name</param>
		/// <param name="fromColumn">column name for a column with a type of DateTime, DateTimeOffset or String, assumes a limited number or formats for string</param>
		/// <param name="dbRow"></param>
		/// <param name="defColumn"></param> PGT 3.23.16 Default field to use in case fromColumn is empty. Must be the same field type as fromColumn field.
		public void WriteElementDateTime(XmlWriter xmlWriter, string elementName, object dataValue, DateTime defaultDate)
		{
			if (dataValue != null && dataValue != DBNull.Value)
			{
				try
				{
					if (dataValue is DateTime)
					{
						DateTime localTime = (DateTime)dataValue;
						this.WriteElementDateTime(xmlWriter, elementName, localTime.ToUniversalTime(), this.TimeZoneOffset(localTime));
						return;
					}
					if (dataValue is DateTimeOffset)
					{
						DateTimeOffset dateTimeOffset = (DateTimeOffset)dataValue;
						DateTime tm = new DateTime(dateTimeOffset.Year, dateTimeOffset.Month, dateTimeOffset.Day, dateTimeOffset.Hour, dateTimeOffset.Minute, dateTimeOffset.Second);
						this.WriteElementDateTime(xmlWriter, elementName, tm, dateTimeOffset.Offset);
						return;
					}
					if (dataValue is string)
					{
						DateTime dateTimeUniversal;
						TimeSpan tzOffset;
						if (StandardizeTime(dataValue as string, defaultDate, out dateTimeUniversal, out tzOffset))
						{
							this.WriteElementDateTime(xmlWriter, elementName, dateTimeUniversal, tzOffset);
							return;
						}
					}
				}
				catch (Exception e)
				{
					Debug.WriteLine(e.ToString());
					// whatever was in that column was not a date, just leave it blank;
				}
			}
			//   No need to output an empty date time value
			//  WriteElementString(elementName, "");
		}

		/// <summary>
		/// Return the offset to GMT, considers time zone and dates of Day Light Savings
		/// </summary>
		/// <param name="dateTime">time/date in local timezone</param>
		/// <returns></returns>
		protected TimeSpan TimeZoneOffset(DateTime dateTime)
		{
			bool isCurrentlyDayLightSavings = this.CurrentTime.IsDaylightSavingTime();
			bool parameterIsDayLightSavings = dateTime.IsDaylightSavingTime();

			if (isCurrentlyDayLightSavings == parameterIsDayLightSavings)
			{
				return this.OffsetFromLocalTimeZoneToGMT;
			}

			TimeSpan retVal = new TimeSpan(this.OffsetFromLocalTimeZoneToGMT.Hours - (isCurrentlyDayLightSavings ? 1 : 0) + (parameterIsDayLightSavings ? 1 : 0), 0, 0);
			return retVal;
		}

		/// <summary>
		/// Formats and write XML element for DateTime elements
		/// </summary>
		/// <param name="elementName">XML element name</param>
		/// <param name="dateTime">value of element</param>
		/// <param name="timeZoneOffset">offset to GMT written as an attribute</param>
		protected void WriteElementDateTime(XmlWriter xmlWriter, string elementName, DateTime dateTime, TimeSpan timeZoneOffset)
		{
			xmlWriter.WriteStartElement(elementName);
			if (dateTime >= BeginningOfTime)
			{
				// for TimeSpan lowercase hh is 00-23
				//  timezoneOffset.ToString() does not follow spec: https://msdn.microsoft.com/en-us/library/1ecy8h51(v=vs.110).aspx
				string val = string.Format("{0:00}:{1:00}", timeZoneOffset.Hours, timeZoneOffset.Minutes);
				xmlWriter.WriteAttributeString("TimeZoneOffset", val);
				string formattedTime = dateTime.ToString("yyyy-MM-dd HH:mm:ss") + "Z";
				xmlWriter.WriteString(formattedTime);
				//Debug.WriteLine(string.Format("TimeZoneOffset=\"{0}\" {1}", val, formattedTime));
			}
			xmlWriter.WriteEndElement();
		}

		/// <summary>
		/// Parse out a string with a time and/or date from selected formats
		/// </summary>
		/// <param name="timeString">INput time</param>
		/// <param name="defaultDate">When timeString does not contain a date, use the date portion of this as a default</param>
		/// <param name="timezoneOffset">Time zone offset betweeen return value and GMT</param>
		/// <returns>True if timeString was successfully parsed</returns>
		internal static bool StandardizeTime(string timeString, DateTime defaultDate, out DateTime dateTimeInGMT, out TimeSpan timezoneOffset)
		{
			dateTimeInGMT = DateTime.MinValue;
			timezoneOffset = TimeSpan.MinValue;
			timeString = timeString.Trim();

			// Is timeString a date and time or only a time?  
			// To be treated as a date and time it has to have date on or after the beginning of time and a colon
			if (timeString.IndexOf(':') > 0 && DateTime.TryParse(timeString, out dateTimeInGMT) && dateTimeInGMT >= BeginningOfTime)
			{
				// no conversion needed
			}
			else if (regExHHMM.IsMatch(timeString))
			{
				dateTimeInGMT = new DateTime(defaultDate.Year
					, defaultDate.Month
					, defaultDate.Day
					, int.Parse(timeString.Substring(0, 2))
					, int.Parse(timeString.Substring(2, 2))
					, 0);
			}
			else if (regExHColonMM.IsMatch(timeString))
			{
				dateTimeInGMT = new DateTime(defaultDate.Year
					, defaultDate.Month
					, defaultDate.Day
					, int.Parse(timeString.Substring(0, 1))
					, int.Parse(timeString.Substring(2, 2))
					, 0);
			}
			else if (regExHHColonMM.IsMatch(timeString))
			{
				dateTimeInGMT = new DateTime(defaultDate.Year
					, defaultDate.Month
					, defaultDate.Day
					, int.Parse(timeString.Substring(0, 2))
					, int.Parse(timeString.Substring(3, 2))
					, 0);
			}
			else if (regExHMM.IsMatch(timeString))
			{
				dateTimeInGMT = new DateTime(defaultDate.Year
					, defaultDate.Month
					, defaultDate.Day
					, int.Parse(timeString.Substring(0, 1))
					, int.Parse(timeString.Substring(1, 2))
					, 0);
			}
			else
			{
				return false;
			}

			timezoneOffset = TimeZone.CurrentTimeZone.GetUtcOffset(dateTimeInGMT);
			dateTimeInGMT = dateTimeInGMT.ToUniversalTime();
			return true;
		}

		/// <summary>
		/// created a UserField element from a transaction column value within this there is an element named UserData# where # is the UserDataIndex
		/// </summary>
		/// <param name="dbRow"></param>
		/// <param name="fromColumn">column within the transaction where the input value is</param>
		/// <param name="userDataIndex">index of userdata 1..24</param>
		public void WriteUserDataElementWithValue(XmlWriter xmlWriter, string data, int userDataIndex)
		{
			xmlWriter.WriteStartElement("UserField");
			xmlWriter.WriteElementString("Name", string.Format("UserData{0:D}", userDataIndex));
			xmlWriter.WriteElementString("Value", string.IsNullOrEmpty(data) ? " " : data);
			xmlWriter.WriteEndElement();
		}
	}
}
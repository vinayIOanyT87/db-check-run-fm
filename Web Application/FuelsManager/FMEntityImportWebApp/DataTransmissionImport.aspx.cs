// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataTransmissionImport.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the DataTransmissionImport type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMEntityImportWebApp
{
	using System;
	using System.IO;
	using System.ServiceModel;
	using System.Text;
	using System.Web;
	using System.Xml;
	using System.Xml.Serialization;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;
	using FMBusinessObjects.ServiceRequests;
	using FMBusinessObjects.UtilityObjects;

	using FuelsManager.FMWebApp;

	public partial class DataTransmissionImport : FMFormBase
	{
		#region Constants and Fields

		private const string ErrMsg001 = "Import file selection is required";

/*
		private const string ErrMsg002 = "Import file is not a valid file";
*/

		#endregion

		#region Public Methods and Operators
		public void WriteFileToImportArchiveDir(Stream fstream, String strImportFilePathAndName)
		{
			// If the Import Archive Directory exist save the file to the directory.

			SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
																	 x =>
																	 x.GetUsingGuid(this.Security, this.Security.SiteGuid)
																);
		    string strPath = site.ImportArchiveDir?.Trim();

		    if (strPath?.Length > 0)
		    {
		        var directoryInfo = new DirectoryInfo(strPath);
		        if (!directoryInfo.Exists)
		        {
		            throw new Exception("Archive Directory Error, check Site configuration.");
		        }

		        const string StrBackSlash = "\\";

		        if (!(strPath.EndsWith(StrBackSlash)))
		        {
		            strPath += StrBackSlash;
		        }

		        char[] slashes = { '\\', '/' };
		        int lastIndexOfSlash = strImportFilePathAndName.LastIndexOfAny(slashes);

		        int lenghtOfFileName = ((strImportFilePathAndName.Length - 1) - lastIndexOfSlash);
		        string strJustFileName = strImportFilePathAndName.Substring(lastIndexOfSlash + 1, lenghtOfFileName);

		        string strImportArchiveDirAndFileName = strPath + strJustFileName;
		        FileStream outFileStream = File.Open(strImportArchiveDirAndFileName, FileMode.Create);

		        var buffer = new byte[1024];
		        int length;
		        fstream.Seek(0, SeekOrigin.Begin);

		        while ((length = fstream.Read(buffer, 0, buffer.Length)) != 0)
		        {
		            outFileStream.Write(buffer, 0, length);
		        }

		        outFileStream.Flush();
		        outFileStream.Close();
		    }
		}
		#endregion

		#region Methods
		protected void ImportBtnClick(object sender, EventArgs e)
		{
			SecurityClass importSecurity = this.Security.Clone();
			importSecurity.EnableChangeTracking = false;

			var accountingSite = FMChannelHelper.MakeCall<IAccountingSites, AccountingSite>(
																	 x =>
																	 x.LoadSiteInfo(this.Security, this.Security.SiteGuid)
																);

			var stats = new ImportStatisticsClass();

			// get the path and file name from the browse control
			try
			{
				if (this.Request.Files.AllKeys.Length == 0)
				{
					throw new Exception(ErrMsg001);
				}

				HttpPostedFile file = this.Request.Files[0];

				if ((file.FileName == "") || (file.ContentLength == 0))
				{
					throw new Exception(ErrMsg001);
				}

				// Check if the file has already been processed and raise an error
				// if the user has not selected the "allow import" checkbox
				this.CheckFileProcessed(file.FileName);

				DateTimeOffset startTime = DateTimeOffset.Now;
				this.FMTextBoxResults.Text = string.Empty;
				this.FMTextBoxResults.Text = "Import Started at " + startTime;
				this.FMTextBoxResults.Text += "\n";

				// get certificate name
				string certificateName =
					FMChannelHelper.MakeCall<IConfigurationSettings, string>(
						x => x.GetKeyValueByKey(this.Security, ConfigurationSettingDOClass.Key_InstallDetails_EnterpriseCertificateName));

				// Decrypt and decompress the file
				var decryption = new Decryption(Encoding.Unicode) { CertificateName = certificateName };
				DecompressionProcessor decompressor = new DecompressionProcessor();

				while (file.InputStream.Position < file.InputStream.Length)
				{
					byte[] decrypted = decryption.Unpackage(file.InputStream);
					byte[] decompressed = decompressor.Decompress(decrypted);

					var memoryStream = new MemoryStream(decompressed);

					var xmlSerializer = new XmlSerializer(typeof(DataTransmissionRecordCollectionClass));
					xmlSerializer.UnknownElement += this.SerializerUnknownElement;
					var importCollection = (DataTransmissionRecordCollectionClass)xmlSerializer.Deserialize(memoryStream);

					// process the objects that have been serialized
					foreach (DataTransmissionRecordClass record in importCollection)
					{
						SiteClass importSite = this.GetSiteByID(this.Security, record.OriginatingSiteID, false);

						// If we did not find the originating site skip
						if (importSite.IdentityGuid.IsEmpty())
						{
							continue;
						}

						importSecurity.SiteGuid = importSite.IdentityGuid;
						importSecurity.SiteID = importSite.ID;

						switch (record.ChangeQueueRecord.RecordType)
						{
							case ChangeQueueRecordType.Companies:
							{
								if (record.ChangeQueueRecord.IsDeletion)
								{
									Guid companyGuid = this.GetIdentityGuidForCompany(importSecurity, record.ChangeQueueRecord.RecordID);

									if (companyGuid != Guid.Empty)
									{
										stats.CompaniesPurged++;
										CompanyClass company = this.GetCompanies(this.Security, companyGuid, false);

										if (company.SiteGuid != this.Security.SiteGuid)
										{
											var entityToSiteMap = new EntityToSiteMapClass(company) { SiteGuid = this.Security.SiteGuid };
											this.PurgeEntityToSiteMaps(this.Security, entityToSiteMap);
										}
										else
										{
											this.PurgeCompanies(importSecurity, companyGuid);
										}
									}
								}
								else
								{
									stats.CompaniesImported++;
									record.Company.SiteGuid = importSecurity.SiteGuid;
									Handle803Company(record.Company);
									this.ImportCompanies(importSecurity, record.Company);
								}

								break;
							}

							case ChangeQueueRecordType.Equipment:
							{
								if (record.ChangeQueueRecord.IsDeletion)
								{
									Guid equipmentGuid = this.GetIdentityGuidEquipments(importSecurity, record.ChangeQueueRecord.RecordID);

									if (equipmentGuid != Guid.Empty)
									{
										stats.EquipmentPurged++;
										EquipmentClass equipment = this.GetEquipments(this.Security, equipmentGuid);

										if (equipment.SiteGuid != this.Security.SiteGuid)
										{
										    var entityToSiteMap = new EntityToSiteMapClass(equipment) { SiteGuid = this.Security.SiteGuid };
										    FMChannelHelper.MakeCall<IEntityToSiteMaps>(
												x =>
													x.Purge(this.Security, entityToSiteMap)
												);
										}
										else
										{
											FMChannelHelper.MakeCall<IEquipments>(
												x =>
													x.Purge(this.Security, equipmentGuid)
												);
										}
									}
								}
								else
								{
									stats.EquipmentImported++;
									record.Equipment.SiteGuid = importSecurity.SiteGuid;
									Handle803Equipment(record.Equipment);
									this.ImportEquipments(importSecurity, record.Equipment);
								}

								break;
							}

							case ChangeQueueRecordType.FuelCards:
							{
								if (record.ChangeQueueRecord.IsDeletion)
								{
									Guid fuelCardGuid = this.GetIdentityGuidForFuelCards(importSecurity, record.ChangeQueueRecord.RecordID);

									if (fuelCardGuid != Guid.Empty)
									{
										stats.FuelCardsPurged++;
										FuelCardClass fuelCard = this.GetFuelcards(this.Security, fuelCardGuid, false);

										if (fuelCard.SiteGuid != this.Security.SiteGuid)
										{
										    var entityToSiteMap = new EntityToSiteMapClass(fuelCard) { SiteGuid = this.Security.SiteGuid };
										    this.PurgeEntityToSiteMaps(this.Security, entityToSiteMap);
										}
										else
										{
											this.PurgeFuelCards(importSecurity, fuelCardGuid);
										}
									}
								}
								else
								{
									stats.FuelCardsImported++;
									record.FuelCard.SiteGuid = importSecurity.SiteGuid;
									this.ImportFuelCards(importSecurity, record.FuelCard);
								}

								break;
							}

							case ChangeQueueRecordType.Personnel:
							{
								if (record.ChangeQueueRecord.IsDeletion)
								{
									Guid personnelGuid = this.GetGuidByIDForPersonnel(importSecurity, record.ChangeQueueRecord.RecordID);

									if (!personnelGuid.IsEmpty())
									{
										stats.PersonnelPurged++;
										PersonClass person = this.GetPersonnel(personnelGuid);

										if (person.SiteGuid != this.Security.SiteGuid)
										{
											var entityToSiteMap = new EntityToSiteMapClass(person) { SiteGuid = this.Security.SiteGuid };
											this.PurgeEntityToSiteMaps(this.Security, entityToSiteMap);
										}
										else
										{
											this.PurgePersonnel(importSecurity, personnelGuid);
										}
									}
								}
								else
								{
									stats.PersonnelImported++;

									// the person class adds the schedule as part of the reset method. During deserialization seven more are added
									// we need to remap these and get rid of the old ones
									if (record.Person.AccessScheduleCollection.Count == 14)
									{
										record.Person.AccessScheduleCollection[0] = record.Person.AccessScheduleCollection[7];
										record.Person.AccessScheduleCollection[1] = record.Person.AccessScheduleCollection[8];
										record.Person.AccessScheduleCollection[2] = record.Person.AccessScheduleCollection[9];
										record.Person.AccessScheduleCollection[3] = record.Person.AccessScheduleCollection[10];
										record.Person.AccessScheduleCollection[4] = record.Person.AccessScheduleCollection[11];
										record.Person.AccessScheduleCollection[5] = record.Person.AccessScheduleCollection[12];
										record.Person.AccessScheduleCollection[6] = record.Person.AccessScheduleCollection[13];

										// remove the bad indexes
										record.Person.AccessScheduleCollection.RemoveAt(13);
										record.Person.AccessScheduleCollection.RemoveAt(12);
										record.Person.AccessScheduleCollection.RemoveAt(11);
										record.Person.AccessScheduleCollection.RemoveAt(10);
										record.Person.AccessScheduleCollection.RemoveAt(9);
										record.Person.AccessScheduleCollection.RemoveAt(8);
										record.Person.AccessScheduleCollection.RemoveAt(7);
									}

									record.Person.SiteGuid = importSecurity.SiteGuid;
									Handle803Person(record.Person);
									this.ImportPersonnel(importSecurity, record.Person);
								}
								break;
							}

							case ChangeQueueRecordType.Products:
							{
								if (record.ChangeQueueRecord.IsDeletion)
								{
									Guid productGuid = this.GetIdentityGuidForProduct(importSecurity, record.ChangeQueueRecord.RecordID);

									if (productGuid != Guid.Empty)
									{
										stats.ProductsPurged++;
										ProductClass product = this.GetByProductAuthorizedCompaniesForProducts(this.Security, productGuid, true);

										if (product.SiteGuid != this.Security.SiteGuid)
										{
											var entityToSiteMap = new EntityToSiteMapClass(product) { SiteGuid = this.Security.SiteGuid };
											this.PurgeEntityToSiteMaps(this.Security, entityToSiteMap);
										}
										else
										{
											this.PurgeProducts(importSecurity, productGuid);
										}
									}
								}
								else
								{
									stats.ProductsImported++;
									record.Product.SiteGuid = importSecurity.SiteGuid;
									Handle803Product(record.Product);
									this.ImportProducts(importSecurity, record.Product);
								}

								break;
							}

							case ChangeQueueRecordType.Transactions:
							{
								try
								{
									Handle803Transaction(record.Transaction);

									var transactionimportSR = new TransactionImportSR(importSecurity, record.Transaction, accountingSite, false);
									stats.TransactionsImported++;

									FMChannelHelper.MakeCall<ITransactionImportProcessor>(x => x.Process(transactionimportSR));
								}
								catch (FaultException<SaveTransactionsException> saveExcept)
								{
									string errorMessage = "\n\rSave Transaction Failed!";
									errorMessage += "\n\rTransID: " + record.Transaction.TransID;

									foreach (TransactionValidationResult result in saveExcept.Detail.Results)
									{
										foreach (string error in result.ErrorList)
										{
											errorMessage += "\n\r" + error;
										}
									}

									errorMessage += "\n\r============================";
									this.FMTextBoxResults.Text += errorMessage;
								}

								break;
							}

							case ChangeQueueRecordType.Groups:
							{
								this.ImportGroupRecord(importSecurity, record, stats);
								break;
							}

							case ChangeQueueRecordType.TransactionAliases:
							{
								this.ImportAliasRecord(importSecurity, record, stats);
								break;
							}

							case ChangeQueueRecordType.CloseoutDO:
							{
								try
								{
									this.ImportCloseoutDORecord(record, stats);
								}
								catch (Exception exception)
								{
									string errorMessage = "\n\rImport Closeout Failed : " + exception.Message;
									errorMessage += "\n\r============================";
									this.FMTextBoxResults.Text += errorMessage;
								}
								break;
							}
						}

						// Entity Assignment, only applicable to Insert
						if (this.Security.SiteGuid != importSite.IdentityGuid && record.ChangeQueueRecord.EventType == "I")
						{
							var entityToSiteMap = new EntityToSiteMapClass { IdentityGuid = this.GetEntityGuid(importSecurity, record) };

							if (entityToSiteMap.IdentityGuid == Guid.Empty)
							{
								continue;
							}

							entityToSiteMap.ID = record.ChangeQueueRecord.RecordID;
							entityToSiteMap.TypeID = record.ChangeQueueRecord.GetEntityType();
							entityToSiteMap.SiteGuid = this.Security.SiteGuid;

							this.AddEntityToSiteMap(importSecurity, entityToSiteMap, this.GetEntityTypeEngine(record).GUID);
						}
					}
				}

				this.WriteFileToImportArchiveDir(file.InputStream, file.FileName);

				this.FMTextBoxResults.Text += stats.StatisticsText();

				DateTimeOffset endTime = DateTimeOffset.Now;
				this.FMTextBoxResults.Text += "Import of Data Complete at " + endTime + "\n";

				var dt = new DataTransmission(this.Security.SiteID, this.Security.UserID);
				AlarmAndEventLogClass alarmAndEventLog = dt.TransmissionImportEventLog;

				FMChannelHelper.MakeCall<IAlarmAndEventLogs>(x => x.Add(this.Security, alarmAndEventLog));
			}
			catch (Exception except)
			{
				this.FMTextBoxResults.Text += "Error Importing Data: " + except.Message + "\n";
				this.ErrorHandler(except);
			}
		}

		private SiteClass GetSiteByID(SecurityClass securityClass, string p1, bool p2)
		{
			return FMChannelHelper.MakeCall<ISites, SiteClass>(
																	 x =>
																	 x.GetByID(securityClass, p1, p2)
																);
		}

		private void AddEntityToSiteMap(SecurityClass securityClass, EntityToSiteMapClass entityToSiteMap, Guid guid)
		{
			FMChannelHelper.MakeCall<IEntityToSiteMaps>(
																	 x =>
																	 x.Add(securityClass, entityToSiteMap, guid)
																);
		}

		private void PurgeEntityToSiteMaps(SecurityClass securityClass, EntityToSiteMapClass entityToSiteMap)
		{
			FMChannelHelper.MakeCall<IEntityToSiteMaps>(
																	 x =>
																	 x.Purge(securityClass, entityToSiteMap)
																);
		}

		private void ImportProducts(SecurityClass importSecurity, ProductClass productClass)
		{
			FMChannelHelper.MakeCall<IProducts>(
																	 x =>
																	 x.Import(importSecurity, productClass)
																);
		}

		private void PurgeProducts(SecurityClass importSecurity, Guid productGuid)
		{
			FMChannelHelper.MakeCall<IProducts>(
																	 x =>
																	 x.Purge(importSecurity, productGuid)
																);
		}

		private ProductClass GetByProductAuthorizedCompaniesForProducts(SecurityClass securityClass, Guid productGuid, bool p)
		{
			return FMChannelHelper.MakeCall<IProducts, ProductClass>(
																	 x =>
																	 x.GetByProductAuthorizedCompanies(securityClass, productGuid, p)
																);
		}

		private Guid GetIdentityGuidForProduct(SecurityClass importSecurity, string key)
		{
			return FMChannelHelper.MakeCall<IProducts, Guid>(
																	 x =>
																	 x.GetIdentityGuid(importSecurity, key)
																);
		}

		private void ImportPersonnel(SecurityClass importSecurity, PersonClass personClass)
		{
			FMChannelHelper.MakeCall<IPersonnel>(
																	 x =>
																	 x.Import(importSecurity, personClass)
																);
		}

		private void PurgePersonnel(SecurityClass importSecurity, Guid personnelGuid)
		{
			FMChannelHelper.MakeCall<IPersonnel>(
																	 x =>
																	 x.Purge(importSecurity, personnelGuid)
																);
		}

		private PersonClass GetPersonnel(Guid personnelGuid)
		{
			return FMChannelHelper.MakeCall<IPersonnel, PersonClass>(
																	 x =>
																	 x.Get(this.Security, personnelGuid)
																);
		}

		private Guid GetGuidByIDForPersonnel(SecurityClass importSecurity, string key)
		{
			return FMChannelHelper.MakeCall<IPersonnel, Guid>(
																	 x =>
																	 x.GetGuidByID(importSecurity, key)
																);
		}

		private void ImportFuelCards(SecurityClass importSecurity, FuelCardClass fuelCardClass)
		{
			FMChannelHelper.MakeCall<IFuelCards>(
																	 x =>
																	 x.Import(importSecurity, fuelCardClass)
																);
		}

		private void PurgeFuelCards(SecurityClass importSecurity, Guid fuelCardGuid)
		{
			FMChannelHelper.MakeCall<IFuelCards>(
																	 x =>
																	 x.Purge(importSecurity, fuelCardGuid)
																);
		}

		private FuelCardClass GetFuelcards(SecurityClass securityClass, Guid fuelCardGuid, bool key)
		{
			return FMChannelHelper.MakeCall<IFuelCards, FuelCardClass>(
																	 x =>
																	 x.Get(securityClass, fuelCardGuid, key)
																);

		}

		private Guid GetIdentityGuidForFuelCards(SecurityClass importSecurity, string key)
		{
			return FMChannelHelper.MakeCall<IFuelCards, Guid>(
																	 x =>
																	 x.GetIdentityGuid(importSecurity, key)
																);
		}

		private void ImportEquipments(SecurityClass importSecurity, EquipmentClass equipmentClass)
		{
			FMChannelHelper.MakeCall<IEquipments>(
																	 x =>
																	 x.Import(importSecurity, equipmentClass)
																);
		}

		private EquipmentClass GetEquipments(SecurityClass securityClass, Guid equipmentGuid)
		{
			return FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(
																	 x =>
																	 x.Get(securityClass, equipmentGuid)
																);
		}

		private Guid GetIdentityGuidEquipments(SecurityClass importSecurity, string key)
		{
			return FMChannelHelper.MakeCall<IEquipments, Guid>(
																	 x =>
																	 x.GetIdentityGuid(importSecurity, key)
																);
		}

		private void ImportCompanies(SecurityClass importSecurity, CompanyClass companyClass)
		{
			FMChannelHelper.MakeCall<ICompanies>(
																	 x =>
																	 x.Import(importSecurity, companyClass)
																);
		}

		private void PurgeCompanies(SecurityClass importSecurity, Guid companyGuid)
		{
			FMChannelHelper.MakeCall<ICompanies>(
																	 x =>
																	 x.Purge(importSecurity, companyGuid)
																);
		}

		private CompanyClass GetCompanies(SecurityClass securityClass, Guid companyGuid, bool boolParam)
		{
			return FMChannelHelper.MakeCall<ICompanies, CompanyClass>(
																	 x =>
																	 x.Get(securityClass, companyGuid, boolParam)
																);
		}

		private Guid GetIdentityGuidForCompany(SecurityClass importSecurity, string key)
		{
			return FMChannelHelper.MakeCall<ICompanies, Guid>(
																	 x =>
																	 x.GetIdentityGuid(importSecurity, key)
																);
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (this.Page.IsPostBack == false)
				{
					this.FMTextBoxResults.Text = "";
				}

				this.ImportBtn.Attributes.Add(
					"onclick", "this.disabled=true;" + this.ClientScript.GetPostBackEventReference(this.ImportBtn, string.Empty));
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void CheckFileProcessed(string fileName)
		{
			if (this.AllowReprocessCheckBox.Checked)
			{
				this.AllowReprocessCheckBox.Checked = false;
				return;
			}

			SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
																	 x =>
																	 x.GetUsingGuid(this.Security, this.Security.SiteGuid)
																);

			// If the import archive directory is not set, there is nothing to check
			if (string.IsNullOrEmpty(site.ImportArchiveDir))
			{
				throw new Exception("Import Archive Directory Error, check Site configuration.");
			}

			// Extract the filename
			var fileInfo = new FileInfo(fileName);

			// Build up the full path file name string
			string fullPathName = site.ImportArchiveDir + "\\" + fileInfo.Name;

			// Check to see if the fileName already exists in the archive directory
			fileInfo = new FileInfo(fullPathName);

			if (fileInfo.Exists)
			{
				throw new FMFileAlreadyImportedException();
			}
		}

		private Guid GetEntityGuid(SecurityClass security, DataTransmissionRecordClass record)
		{
			switch (record.ChangeQueueRecord.RecordType)
			{
				case ChangeQueueRecordType.Companies:
					return FMChannelHelper.MakeCall<ICompanies, Guid>(
																	 x =>
																	 x.GetIdentityGuid(security, record.ChangeQueueRecord.RecordID)
																);
				case ChangeQueueRecordType.Equipment:
					return FMChannelHelper.MakeCall<IEquipments, Guid>(
																	 x =>
																	 x.GetIdentityGuid(security, record.ChangeQueueRecord.RecordID)
																);
				case ChangeQueueRecordType.FuelCards:
					return FMChannelHelper.MakeCall<IFuelCards, Guid>(
																	 x =>
																	 x.GetIdentityGuid(security, record.ChangeQueueRecord.RecordID)
																);
				case ChangeQueueRecordType.Groups:
					return FMChannelHelper.MakeCall<IGroups, Guid>(
																	 x =>
																	 x.GetIdentityGuid(security, record.ChangeQueueRecord.RecordID)
																);
				case ChangeQueueRecordType.Personnel:
					return FMChannelHelper.MakeCall<IPersonnel, Guid>(
																	 x =>
																	 x.GetGuidByID(security, record.ChangeQueueRecord.RecordID)
																);
				case ChangeQueueRecordType.Products:
					return FMChannelHelper.MakeCall<IProducts, Guid>(
																	 x =>
																	 x.GetIdentityGuid(security, record.ChangeQueueRecord.RecordID)
																);
				case ChangeQueueRecordType.TransactionAliases:
					return FMChannelHelper.MakeCall<ITransactionAliases, Guid>(
																	 x =>
																	 x.GetIdentityGuid(security, record.ChangeQueueRecord.RecordID)
																);


				// WI#23813: Import was throwing exception on CloseoutDO.
				// Should return 0, so it can continue to next ChangeQueue record
				// CloseoutDO is a supported type, but does not need EntityToSiteMap records
				case ChangeQueueRecordType.CloseoutDO:
					return Guid.Empty;
				
				default:
					throw new NotImplementedException("Unsupported change queue record type.");
			}
		}

		private Type GetEntityTypeEngine(DataTransmissionRecordClass record)
		{
			switch (record.ChangeQueueRecord.RecordType)
			{
				case ChangeQueueRecordType.Companies:
					return FMChannelHelper.MakeCall<ICompanies, Type>(
																	 x =>
																	 x.GetType());
				case ChangeQueueRecordType.Equipment:
					return FMChannelHelper.MakeCall<IEquipments, Type>(
																	 x =>
																	 x.GetType());
				case ChangeQueueRecordType.FuelCards:
					return FMChannelHelper.MakeCall<FuelCardClass, Type>(
																	 x =>
																	 x.GetType());
				case ChangeQueueRecordType.Groups:
					return FMChannelHelper.MakeCall<IGroups, Type>(
																	 x =>
																	 x.GetType());
				case ChangeQueueRecordType.Personnel:
					return FMChannelHelper.MakeCall<IPersonnel, Type>(
																	 x =>
																	 x.GetType());
				case ChangeQueueRecordType.Products:
					return FMChannelHelper.MakeCall<IProducts, Type>(
																	 x =>
																	 x.GetType());
				case ChangeQueueRecordType.TransactionAliases:
					return FMChannelHelper.MakeCall<ITransactionAliases, Type>(
																	 x =>
																	 x.GetType());
				default:
					throw new NotImplementedException("Unsupported change queue record type.");
			}
		}

		private void ImportAliasRecord(SecurityClass importSecurity, DataTransmissionRecordClass record, ImportStatisticsClass stats)
		{
			if (record.ChangeQueueRecord.IsDeletion)
			{
				Guid transactionAliasGuid = this.GetIdentityGuidForAlias(importSecurity, record.ChangeQueueRecord.RecordID);

				if (transactionAliasGuid != Guid.Empty)
				{
					stats.AliasesPurged++;
					TransactionAliasClass transactionAlias = this.GetTransactionAlias(this.Security, transactionAliasGuid, false);
                    transactionAlias.IdentityGuid = transactionAlias.MasterRecordGuid;
                    transactionAliasGuid = transactionAlias.MasterRecordGuid;

					if (transactionAlias.SiteGuid != this.Security.SiteGuid)
					{
						var entityToSiteMap = new EntityToSiteMapClass(transactionAlias) { SiteGuid = this.Security.SiteGuid };
						this.PurgeEntityToSiteMaps(this.Security, entityToSiteMap);
					}
					else
					{
						this.PurgeTransactionAlias(importSecurity, transactionAliasGuid);
					}
				}
			}
			else
			{
				stats.AliasesImported++;
				record.TransactionAlias.SiteGuid = importSecurity.SiteGuid;
				Handle803TransactionAlias(record.TransactionAlias);
				this.ImportTransactionAlias(importSecurity, record.TransactionAlias);
			}
		}

		private void ImportTransactionAlias(SecurityClass importSecurity, TransactionAliasClass transactionAliasClass)
		{
			FMChannelHelper.MakeCall<ITransactionAliases>(
																	 x =>
																	 x.Import(importSecurity, transactionAliasClass)
																);
		}

		private void PurgeTransactionAlias(SecurityClass importSecurity, Guid transactionAliasGuid)
		{
			FMChannelHelper.MakeCall<ITransactionAliases>(
																	 x =>
																	 x.Purge(importSecurity, transactionAliasGuid)
																);
		}

		private TransactionAliasClass GetTransactionAlias(SecurityClass securityClass, Guid transactionAliasGuid, bool param)
		{
			return FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasClass>(
																	 x =>
																	 x.Get(securityClass, transactionAliasGuid, param)
																);
		}

		private Guid GetIdentityGuidForAlias(SecurityClass importSecurity, string key)
		{
			return FMChannelHelper.MakeCall<ITransactionAliases, Guid>(
																	 x =>
																	 x.GetIdentityGuid(importSecurity, key)
																);
		}

		private void ImportCloseoutDORecord(DataTransmissionRecordClass record, ImportStatisticsClass stats)
		{
			if (record.ChangeQueueRecord.IsDeletion)
			{
				// Do nothing I don't believe there is supposed to be a deletion of a close out record.
				stats.CloseoutDOPurged++;
			}
			else
			{
				var closeoutSrvcRqst = new CloseoutSR
				                       {
					                       Security = this.Security,
					                       Closeout = record.Closeout,
					                       CloseoutCommand = CloseoutSR.CloseoutType.SAVE_TO_IMPORT
				                       };

				closeoutSrvcRqst.CloseoutCommand = CloseoutSR.CloseoutType.SAVE_TO_IMPORT;

				FMChannelHelper.MakeCall<ICloseoutProcessor>(x => x.Process(closeoutSrvcRqst));
				stats.CloseoutDOImported++;
			}
		}

		private void ImportGroupRecord(SecurityClass importSecurity, DataTransmissionRecordClass record, ImportStatisticsClass stats)
		{
			if (record.ChangeQueueRecord.IsDeletion)
			{
				Guid groupGuid = this.GetIdentityGuidForGroups(importSecurity, record.ChangeQueueRecord.RecordID);

				if (!groupGuid.IsEmpty())
				{
					stats.GroupsPurged++;
					GroupClass group = this.GetGroups(this.Security, groupGuid);

					if (group.SiteGuid != this.Security.SiteGuid)
					{
					    var entityToSiteMap = new EntityToSiteMapClass(group) { SiteGuid = this.Security.SiteGuid };
					    this.PurgeEntityToSiteMaps(this.Security, entityToSiteMap);
					}
					else
					{
						this.PurgeGroup(importSecurity, groupGuid);
					}
				}
			}
			else
			{
				stats.GroupsImported++;
				record.Group.SiteGuid = importSecurity.SiteGuid;
				this.ImportGroup(importSecurity, record.Group);
			}
		}

		private void ImportGroup(SecurityClass importSecurity, GroupClass groupClass)
		{
			FMChannelHelper.MakeCall<IGroups>(
																	 x =>
																	 x.Import(importSecurity, groupClass)
																);
		}

		private void PurgeGroup(SecurityClass importSecurity, Guid groupGuid)
		{
			FMChannelHelper.MakeCall<IGroups>(
																	 x =>
																	 x.Purge(importSecurity, groupGuid)
																);
		}

		private GroupClass GetGroups(SecurityClass securityClass, Guid groupGuid)
		{
			return FMChannelHelper.MakeCall<IGroups, GroupClass>(
																	 x =>
																	 x.Get(securityClass, groupGuid)
																);
		}

		private Guid GetIdentityGuidForGroups(SecurityClass importSecurity, string key)
		{
			return FMChannelHelper.MakeCall<IGroups, Guid>(
																	 x =>
																	 x.GetIdentityGuid(importSecurity, key)
																);
		}

		/// <summary>
		/// To handle unknown xml elements.  This method is to handle 803 xml elements relocated to different nodes in 804.
		/// This is TFMD specific.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="eventArg"></param>
		private void SerializerUnknownElement(object sender, XmlElementEventArgs eventArg)
		{
			if (eventArg.Element.Name == "UserDataList")	// 803 User Data Node
			{
				TransactionDO trxDO = eventArg.ObjectBeingDeserialized as TransactionDO;

				if (trxDO != null)
				{
					XmlNodeList dataListNode = eventArg.Element.ChildNodes;

					// go through each user data
					foreach (XmlNode currentNode in dataListNode)
					{
						// Get the key and value and put them in keyString and valueString
						string keyString = string.Empty;
						string valueString = string.Empty;
						XmlNode keyNode = currentNode.SelectSingleNode("Key");
						XmlNode valueNode = currentNode.SelectSingleNode("Value");

						if (keyNode != null)
						{
							keyString = keyNode.InnerText;

							if (valueNode != null)
							{
								valueString = valueNode.InnerText;
							}
						}

						if (string.IsNullOrEmpty(valueString) == false)
						{
							// in TFMD, we only have 3 user data fields defined.
							switch (keyString)
							{
								case TransactionDO.USER_DATA_KEY_01:
									trxDO.UserData1 = valueString;
									break;
								case TransactionDO.USER_DATA_KEY_02:
									trxDO.UserData2 = valueString;
									break;
								case TransactionDO.USER_DATA_KEY_03:
									trxDO.UserData3 = valueString;
									break;
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Translate 803 &lt;None&gt; value to 804 {Unassigned} value
		/// </summary>
		/// <param name="oldValue"></param>
		/// <returns></returns>
		private static string TranslateUnassignedValue(string oldValue)
		{
			return oldValue == "<Unassigned>" ? "{Unassigned}" : oldValue;
		}

		/// <summary>
		/// Translate 803 &lt; &gt; value to 804 { } value
		/// </summary>
		/// <param name="oldValue"></param>
		/// <returns></returns>
		private static string TranslateLtGtValue(string oldValue)
		{
			if (string.IsNullOrEmpty(oldValue) == false)
			{
				oldValue = oldValue.Replace("<", "{").Replace(">", "}");
			}

			return oldValue;
		}

		/// <summary>
		/// Translate 803 &lt;None&gt; value to 804 {Unassigned} value
		/// </summary>
		/// <param name="oldValue"></param>
		/// <returns></returns>
		private static string TranslateNoneValue(string oldValue)
		{
			return oldValue == "<None>" ? "{None}" : oldValue;
		}

		/// <summary>
		/// Handle translation of &lt;Unassigned&gt;, &lt;None&gt; to {Unassigned} and {None} for Equpment
		/// </summary>
		/// <param name="equip">Equipment Data</param>
		private static void Handle803Equipment(EquipmentClass equip)
		{
			equip.CompanyID = TranslateUnassignedValue(equip.CompanyID);
			equip.ProductID = TranslateUnassignedValue(equip.ProductID);
			equip.FuelCardID = TranslateUnassignedValue(equip.FuelCardID);
		}

		/// <summary>
		/// Handle translation of &lt;Unassigned&gt;, &lt;None&gt; to {Unassigned} and {None} for Person
		/// </summary>
		/// <param name="person">Person Data</param>
		private static void Handle803Person(PersonClass person)
		{
			person.CompanyID = TranslateUnassignedValue(person.CompanyID);
			person.AssignedEquipmentID = TranslateUnassignedValue(person.AssignedEquipmentID);
		}

		/// <summary>
		/// Handle translation of &lt;Unassigned&gt;, &lt;None&gt; to {Unassigned} and {None} for Company
		/// </summary>
		/// <param name="company">Company Data</param>
		private static void Handle803Company(CompanyClass company)
		{
			company.IATAID = TranslateNoneValue(company.IATAID);
			company.ShipperTypeID = TranslateNoneValue(company.ShipperTypeID);
			company.CustomerBillToTypeID = TranslateNoneValue(company.CustomerBillToTypeID);
			company.CustomerShipToTypeID = TranslateNoneValue(company.CustomerShipToTypeID);
		}

		/// <summary>
		/// Handle translation of &lt;Unassigned&gt;, &lt;None&gt; to {Unassigned} and {None} for Product
		/// </summary>
		/// <param name="product">Product Data</param>
		private static void Handle803Product(ProductClass product)
		{
			product.TrackingProductID = TranslateNoneValue(product.TrackingProductID);
		}

		/// <summary>
		/// Handle translation of &lt;Unassigned&gt;, &lt;None&gt; to {Unassigned} and {None} for TransactionAlias
		/// </summary>
		/// <param name="transactionAlias">TransactionAlias Data</param>
		private static void Handle803TransactionAlias(TransactionAliasClass transactionAlias)
		{
			transactionAlias.AssociatedAlias = TranslateNoneValue(transactionAlias.AssociatedAlias);
		}

		/// <summary>
		/// Handle translation of &lt;, &gt;, to {, } for TransactionAlias
		/// </summary>
		/// <param name="transaction">Transaction Data</param>
		private static void Handle803Transaction(TransactionDO transaction)
		{
			transaction.OperatorID = TranslateLtGtValue(transaction.OperatorID);
			transaction.OperatorName = TranslateLtGtValue(transaction.OperatorName);
		}
		#endregion

		private class ImportStatisticsClass
		{
			#region Constants and Fields
			public int AliasesImported;
			public int AliasesPurged;
			public int CloseoutDOImported;
			public int CloseoutDOPurged;
			public int CompaniesImported;
			public int CompaniesPurged;
			public int EquipmentImported;
			public int EquipmentPurged;
			public int FuelCardsImported;
			public int FuelCardsPurged;
			public int GroupsImported;
			public int GroupsPurged;
			public int PersonnelImported;
			public int PersonnelPurged;
			public int ProductsImported;
			public int ProductsPurged;
			public int TransactionsImported;
			#endregion

			#region Public Methods and Operators
			public string StatisticsText()
			{
				var sb = new StringBuilder();

				sb.AppendLine();

				if (this.CompaniesImported > 0)
				{
					sb.AppendLine("Companies Imported: " + this.CompaniesImported);
				}

				if (this.CompaniesPurged > 0)
				{
					sb.AppendLine("Companies Purged: " + this.CompaniesPurged);
				}

				if (this.EquipmentImported > 0)
				{
					sb.AppendLine("Equipment Imported: " + this.EquipmentImported);
				}

				if (this.EquipmentPurged > 0)
				{
					sb.AppendLine("Equipment Purged: " + this.EquipmentPurged);
				}

				if (this.PersonnelImported > 0)
				{
					sb.AppendLine("Personnel Imported: " + this.PersonnelImported);
				}

				if (this.PersonnelPurged > 0)
				{
					sb.AppendLine("Personnel Purged: " + this.PersonnelPurged);
				}

				if (this.FuelCardsImported > 0)
				{
					sb.AppendLine("Fuel Cards Imported: " + this.FuelCardsImported);
				}

				if (this.FuelCardsPurged > 0)
				{
					sb.AppendLine("Fuel Cards Purged: " + this.FuelCardsPurged);
				}

				if (this.ProductsImported > 0)
				{
					sb.AppendLine("Products Imported: " + this.ProductsImported);
				}

				if (this.ProductsPurged > 0)
				{
					sb.AppendLine("Products Purged: " + this.ProductsPurged);
				}

				if (this.GroupsImported > 0)
				{
					sb.AppendLine("User Groups Imported: " + this.GroupsImported);
				}

				if (this.GroupsPurged > 0)
				{
					sb.AppendLine("User Groups Purged: " + this.GroupsPurged);
				}

				if (this.AliasesImported > 0)
				{
					sb.AppendLine("Transaction Aliases Imported: " + this.AliasesImported);
				}

				if (this.AliasesPurged > 0)
				{
					sb.AppendLine("Transaction Aliases Purged: " + this.GroupsPurged);
				}

				if (this.TransactionsImported > 0)
				{
					sb.AppendLine("Transactions Imported: " + this.TransactionsImported);
				}

				if (this.CloseoutDOImported > 0)
				{
					sb.AppendLine("CloseoutDO Imported: " + this.CloseoutDOImported);
				}

				if (this.CloseoutDOPurged > 0)
				{
					sb.AppendLine("CloseoutDO Purged: " + this.CloseoutDOPurged);
				}

				sb.AppendLine();

				return sb.ToString();
			}
			#endregion
		}
	}
}
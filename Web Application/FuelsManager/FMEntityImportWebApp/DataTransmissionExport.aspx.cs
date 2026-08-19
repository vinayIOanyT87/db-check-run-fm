// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataTransmissionExport.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the DataTransmissionExport type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMEntityImportWebApp
{
	using System;
	using System.IO;
	using System.Xml.Serialization;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;
	using FMBusinessObjects.UtilityObjects;

	using FuelsManager.FMWebApp;

	public partial class DataTransmissionExport : FMFormBase
	{
		#region Constants and Fields
		public static string AllChangesText = "All changes since last export";

		private readonly CloseoutDO closeout = new CloseoutDO();
		private AccountingSite accountingSite;
		private ChangeQueueRecordCollection recordCollection;
		#endregion

		#region Methods
		protected void ExportBtnClick(object sender, EventArgs e)
		{
			try
			{
				// What type of export are we doing?
				if (this.FMRadioButtonList1.SelectedValue.Equals(AllChangesText))
				{
					var dt = new DataTransmission(this.Security.SiteID, this.Security.UserID);
					AlarmAndEventLogClass alarmAndEventLog = dt.TransmissionExportEventLog;

					this.ExportAllChanges(alarmAndEventLog);
				}
				else
				{
					var dt = new DataTransmission(this.Security.SiteID, this.Security.UserID, this.FMDateFromDate.Text);
					AlarmAndEventLogClass alarmAndEventLog = dt.TransmissionExportReProcessEventLog;

					this.ExportSelectedDateRange(alarmAndEventLog);
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void FMRadioButtonList1SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.FMRadioButtonList1.SelectedIndex == 1)
			{
				this.FMLabelFromDate.Enabled = true;
				this.FMDateFromDate.Enabled = true;
				this.FMLabelToDate.Enabled = true;
				this.FMDateToDate.Enabled = true;
			}
			else
			{
				this.FMLabelFromDate.Enabled = false;
				this.FMDateFromDate.Enabled = false;
				this.FMLabelToDate.Enabled = false;
				this.FMDateToDate.Enabled = false;
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.accountingSite = new AccountingSite();

				this.GetSecurity();

				if (this.Page.IsPostBack == false)
				{
					this.FMRadioButtonList1.SelectedValue = AllChangesText;
					this.FMLabelFromDate.Enabled = false;
					this.FMDateFromDate.Enabled = false;
					this.FMLabelToDate.Enabled = false;
					this.FMDateToDate.Enabled = false;

					SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
																	 x =>
																	 x.GetUsingGuid(this.Security, this.Security.SiteGuid)
																);

					var timeConverter = new SiteTimeConverter(site);
					this.FMDateFromDate.Text = timeConverter.Today().ToString(site.GetDateTimeFormatInfo());
					this.FMDateToDate.Text = timeConverter.Today().AddDays(1).ToString(site.GetDateTimeFormatInfo());

					// Get any update text from the DataDictionary
					// Dictionary keys can only be up to 100 characters inclusive.
					const string StrOriginalTxt1 = "This page provides a method for manually triggering an export of"; // parm 0
					const string StrOriginalTxt2 = "Companies"; // parm 1
					const string StrOriginalTxt3 = "Equipment"; // parm 2
					const string StrOriginalTxt4 = "Fuel Cards"; // parm 3
					const string StrOriginalTxt5 = "Personnel"; // parm 4
					const string StrOriginalTxt6 = "Products"; // parm 5
					const string StrOriginalTxt7 = "Price List"; // parm 6
					const string StrOriginalTxt8 = "Equipment Types"; // parm 7
					const string StrOriginalTxt9 = "Delivery Locations"; // parm 8
					const string StrOriginalTxt10 = "Transaction"; // parm 9
					const string StrOriginalTxt11 = "Data"; // parm 10
					const string StrOriginalTxt12 = "for transfer from this site to an enterprise site group."; // parm 11
					const string StrOriginalTxt13 = "Select the desired type below and click the Export button."; // parm 12
					const string StrOriginalTxt14 = "Do not press the Cancel button on the file dialog."; // parm 13
					const string StrOriginalTxt15 = "If Cancel button is pressed, must then use 'Re-process from date' option."; // parm 14

					// This page provides a method for manually triggering an export of companies, equipment, fuel cards, personnel,
					// products, price list, equipment types, delivery locations, and transaction data for transfer from this site to
					// an enterprise site group. Select the desired type below and click the Export button.
					this.PageDescriptionExport.Text =
						string.Format(
							this.PageDescriptionExport.Text =
							string.Format(
								"{0} {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, and {9} {10} {11} {12} {13} {14}",
								this.GetTranslatedText(StrOriginalTxt1),
								this.GetTranslatedText(StrOriginalTxt2),
								this.GetTranslatedText(StrOriginalTxt3),
								this.GetTranslatedText(StrOriginalTxt4),
								this.GetTranslatedText(StrOriginalTxt5),
								this.GetTranslatedText(StrOriginalTxt6),
								this.GetTranslatedText(StrOriginalTxt7),
								this.GetTranslatedText(StrOriginalTxt8),
								this.GetTranslatedText(StrOriginalTxt9),
								this.GetTranslatedText(StrOriginalTxt10),
								this.GetTranslatedText(StrOriginalTxt11),
								this.GetTranslatedText(StrOriginalTxt12),
								this.GetTranslatedText(StrOriginalTxt13),
								this.GetTranslatedText(StrOriginalTxt14),
								this.GetTranslatedText(StrOriginalTxt15)));
				}

				this.ExportBtn.Attributes.Add(
					"onclick", "this.disabled=true;" + this.ClientScript.GetPostBackEventReference(this.ExportBtn, string.Empty));
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void AddAliasRecord(DataTransmissionRecordCollectionClass transmissionCollection, ChangeQueueRecordClass record, DataTransmissionRecordClass transmissionRecord)
		{
			Guid aliasGuid = FMChannelHelper.MakeCall<ITransactionAliases, Guid>(
																	 x =>
																	 x.GetIdentityGuid(this.Security, record.RecordID)
																);
			if (aliasGuid == record.RecordGuid)
			{
				transmissionRecord.TransactionAlias = FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasClass>(
																	 x =>
																	 x.Get(this.Security, aliasGuid, false)
																);
				transmissionRecord.OriginatingSiteID = FMChannelHelper.MakeCall<ISites, string>(
																	 x =>
																	 x.GetIDNoRefresh(this.Security, transmissionRecord.TransactionAlias.SiteGuid)
																);
				transmissionCollection.Add(transmissionRecord);
			}
		}

		private void AddCloseoutRecord(	DataTransmissionRecordCollectionClass transmissionCollection,
										ChangeQueueRecordClass record,
										DataTransmissionRecordClass transmissionRecord)
		{
			var closeoutSrvcRqst = new CloseoutSR
			                       {
				                       Security = this.Security,
				                       CloseoutInventoryGuid = record.RecordGuid,
				                       CloseoutCommand = CloseoutSR.CloseoutType.GET_TO_EXPORT
			                       };

			transmissionRecord.Closeout = FMChannelHelper.MakeCall<ICloseoutProcessor, CloseoutDO>(
																	 x =>
																	 x.Process(closeoutSrvcRqst)
																);
			if (transmissionRecord.Closeout != null)
			{
				transmissionRecord.OriginatingSiteID = FMChannelHelper.MakeCall<ISites, string>(
																	 x =>
																	 x.GetIDNoRefresh(this.Security, transmissionRecord.Closeout.SiteGuid)
																);
				transmissionCollection.Add(transmissionRecord);
			}
		}

		private void AddCompanyRecord(DataTransmissionRecordCollectionClass transmissionCollection, ChangeQueueRecordClass record, DataTransmissionRecordClass transmissionRecord)
		{
			Guid companyGuid = FMChannelHelper.MakeCall<ICompanies, Guid>(
																	 x =>
																	 x.GetIdentityGuid(this.Security, record.RecordID)
																);
			if (companyGuid == record.RecordGuid)
			{
				transmissionRecord.Company = FMChannelHelper.MakeCall<ICompanies, CompanyClass>(
																	 x =>
																	 x.Get(this.Security, companyGuid)
																);
				transmissionRecord.OriginatingSiteID = FMChannelHelper.MakeCall<ISites, string>(
																	 x =>
																	 x.GetIDNoRefresh(this.Security, transmissionRecord.Company.SiteGuid)
																);

				transmissionCollection.Add(transmissionRecord);
			}
		}

		private void AddEquipmentRecord(DataTransmissionRecordCollectionClass transmissionCollection, ChangeQueueRecordClass record, DataTransmissionRecordClass transmissionRecord)
		{
			Guid equipmentGuid = FMChannelHelper.MakeCall<IEquipments, Guid>(
																	 x =>
																	 x.GetIdentityGuid(this.Security, record.RecordID)
																);
			if (equipmentGuid == record.RecordGuid)
			{
				transmissionRecord.Equipment = FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(
																	 x =>
																	 x.Get(this.Security, equipmentGuid)
																);

				if (string.IsNullOrEmpty(transmissionRecord.Equipment.FuelCardID) == false)
				{
					transmissionRecord.Equipment.ExportUseFuelCard = FMChannelHelper.MakeCall<IFuelCards, FuelCardClass>(
																	 x =>
																	 x.Get(this.Security, transmissionRecord.Equipment.FuelCardGuid, false)
																);
				}

				transmissionRecord.OriginatingSiteID = FMChannelHelper.MakeCall<ISites, string>(
																	 x =>
																	 x.GetIDNoRefresh(this.Security, transmissionRecord.Equipment.SiteGuid)
																);

				transmissionCollection.Add(transmissionRecord);
			}
		}

		private void AddFuelCardRecord(DataTransmissionRecordCollectionClass transmissionCollection, ChangeQueueRecordClass record, DataTransmissionRecordClass transmissionRecord)
		{
			Guid identityGuid = FMChannelHelper.MakeCall<IFuelCards, Guid>(
																	 x =>
																	 x.GetIdentityGuid(this.Security, record.RecordID)
																);
			if (identityGuid == record.RecordGuid)
			{
				transmissionRecord.FuelCard = FMChannelHelper.MakeCall<IFuelCards, FuelCardClass>(
																	 x =>
																	 x.Get(this.Security, identityGuid, true)
																);

				transmissionRecord.OriginatingSiteID = FMChannelHelper.MakeCall<ISites, string>(
																	 x =>
																	 x.GetIDNoRefresh(this.Security, transmissionRecord.FuelCard.SiteGuid)
																);
				transmissionCollection.Add(transmissionRecord);
			}
		}

		private void AddGroupRecord(DataTransmissionRecordCollectionClass transmissionCollection, ChangeQueueRecordClass record, DataTransmissionRecordClass transmissionRecord)
		{
			Guid groupGuid = FMChannelHelper.MakeCall<IGroups, Guid>(
																	 x =>
																	 x.GetIdentityGuid(this.Security, record.RecordID)
																);
			if (groupGuid == record.RecordGuid)
			{
				transmissionRecord.Group = FMChannelHelper.MakeCall<IGroups, GroupClass>(
																	 x =>
																	 x.Get(this.Security, groupGuid)
																);
				transmissionRecord.OriginatingSiteID = FMChannelHelper.MakeCall<ISites, string>(
																	 x =>
																	 x.GetIDNoRefresh(this.Security, transmissionRecord.Group.SiteGuid)
																);

                // Export Group with UserGroupMaps associated with owner site because this is the context
                // in which it will be imported.  Site User Group functionality is not supported
                // to implement support will require Change Tracking on UserGroupMaps
                if (transmissionRecord.Group.SiteGuid != this.Security.SiteGuid)
                {
                    transmissionRecord.Group.UserGroupMapCollection = FMChannelHelper.MakeCall<IUserGroupMaps, UserGroupMapCollectionClass>(
                                                                       x => x.EnumerateByGroupAndSite(this.Security, groupGuid, transmissionRecord.Group.SiteGuid));
                }

                transmissionCollection.Add(transmissionRecord);
			}
		}

		private void AddPersonRecord(DataTransmissionRecordCollectionClass transmissionCollection, ChangeQueueRecordClass record, DataTransmissionRecordClass transmissionRecord)
		{
			Guid personGuid = FMChannelHelper.MakeCall<IPersonnel, Guid>(
																	 x =>
																	 x.GetGuidByID(this.Security, record.RecordID)
																);
			if (personGuid == record.RecordGuid)
			{
				PersonClass person = FMChannelHelper.MakeCall<IPersonnel, PersonClass>(
																	 x =>
																	 x.Get(this.Security, personGuid)
																);
				transmissionRecord.Person = FMChannelHelper.MakeCall<IPersonnel, PersonClass>(
																	 x =>
																	 x.PrepareForExport(this.Security, person)
																);
				transmissionRecord.OriginatingSiteID = FMChannelHelper.MakeCall<ISites, string>(
																	 x =>
																	 x.GetIDNoRefresh(this.Security, transmissionRecord.Person.SiteGuid)
																);
				transmissionCollection.Add(transmissionRecord);
			}
		}

		private void AddProductRecord(DataTransmissionRecordCollectionClass transmissionCollection, ChangeQueueRecordClass record, DataTransmissionRecordClass transmissionRecord)
		{
			Guid identityGuid = FMChannelHelper.MakeCall<IProducts, Guid>(
																	 x =>
																	 x.GetIdentityGuid(this.Security, record.RecordID)
																);
			if (identityGuid == record.RecordGuid)
			{
				transmissionRecord.Product = FMChannelHelper.MakeCall<IProducts, ProductClass>(
																	 x =>
																	 x.Get(this.Security, identityGuid)
																);

				transmissionRecord.OriginatingSiteID = FMChannelHelper.MakeCall<ISites, string>(
																	 x =>
																	 x.GetIDNoRefresh(this.Security, transmissionRecord.Product.SiteGuid)
																);
				transmissionCollection.Add(transmissionRecord);
			}
		}

		private void AddTransactionRecord(	DataTransmissionRecordCollectionClass transmissionCollection,
											ChangeQueueRecordClass record,
											DataTransmissionRecordClass transmissionRecord)
		{
			var sr = new TransactionSR
			         {
				         Security = this.Security,
				         TransID = record.RecordID,
				         ConvertUnits = false,
				         AllowCrossSiteTransactions = true,
				         AccountingSite = this.accountingSite
			         };

			transmissionRecord.Transaction = FMChannelHelper.MakeCall<ITransactionProcessor, TransactionDO>(
																	 x =>
																	 x.Process(sr)
																);

			if (transmissionRecord.Transaction != null && transmissionRecord.Transaction.SiteGuid == this.accountingSite.CurrentSiteGuid)
			{
				transmissionRecord.OriginatingSiteID = FMChannelHelper.MakeCall<ISites, string>(
																		x =>
																		x.GetIDNoRefresh(this.Security, transmissionRecord.Transaction.SiteGuid)
																	);

				transmissionCollection.Add(transmissionRecord);
			}
		}

		private bool CheckEntityAssignmentStatus(ChangeQueueRecordClass record)
		{
			if (record.SiteGuid != this.Security.SiteGuid)
			{
				switch (record.RecordType)
				{
					case ChangeQueueRecordType.Companies:
						if (FMChannelHelper.MakeCall<ICompanies, Guid>(
								x =>
								x.GetIdentityGuid(this.Security, record.RecordID)) == Guid.Empty)
						{
							return false;
						}
						break;

					case ChangeQueueRecordType.Equipment:
						if (FMChannelHelper.MakeCall<IEquipments, Guid>(
																	 x =>
																	 x.GetIdentityGuid(this.Security, record.RecordID)) == Guid.Empty)
						{
							return false;
						}
						break;

					case ChangeQueueRecordType.FuelCards:
						if (FMChannelHelper.MakeCall<IFuelCards, Guid>(
                                                    x =>
                                                    x.GetIdentityGuid(this.Security, record.RecordID)) == Guid.Empty)
						{
							return false;
						}
						break;

					case ChangeQueueRecordType.Groups:
						if (FMChannelHelper.MakeCall<IGroups, bool>(
                                                    x =>
                                                    x.GetIdentityGuid(this.Security, record.RecordID).IsEmpty() ))
						{
							return false;
						}
						break;

					case ChangeQueueRecordType.Personnel:
						if (FMChannelHelper.MakeCall<IPersonnel, bool>(
                                                    x =>
                                                    x.GetGuidByID(this.Security, record.RecordID).IsEmpty()) )
						{
							return false;
						}
						break;

					case ChangeQueueRecordType.Products:
						if (FMChannelHelper.MakeCall<IProducts, Guid>(
                                                    x =>
                                                    x.GetIdentityGuid(this.Security, record.RecordID)) == Guid.Empty)
						{
							return false;
						}
						break;

					case ChangeQueueRecordType.TransactionAliases:
						if (FMChannelHelper.MakeCall<ITransactionAliases, Guid>(
                                                    x =>
                                                    x.GetIdentityGuid(this.Security, record.RecordID)) == Guid.Empty)
						{
							return false;
						}
						break;

					case ChangeQueueRecordType.Transactions:
						break; // do nothing since transactions cannot be "assigned"
				}
			}

			return true;
		}

		private MemoryStream EncryptStream(MemoryStream stream)
		{
			// get certificate name
			string certificateName =
				FMChannelHelper.MakeCall<IConfigurationSettings, string>(
					x => x.GetKeyValueByKey(this.Security, ConfigurationSettingDOClass.Key_InstallDetails_EnterpriseCertificateName));

			// Compress and encrypt the stream
			var compressionProcessor = new CompressionProcessor();
			var encryption = new Encryption(System.Text.Encoding.Unicode) { CertificateName = certificateName };
			byte [] data = compressionProcessor.Compress(stream.ToArray());

			return encryption.Package(data);
		}

		private void ExportAllChanges(AlarmAndEventLogClass alarmAndEventLog)
		{
			// Get a list of the changes to process
			this.recordCollection =
				FMChannelHelper.MakeCall<IChangeQueueRecordsClass, ChangeQueueRecordCollection>(
					x => x.EnumerateIncompleteRecords(this.Security));

			SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
															 x =>
															 x.GetUsingGuid(this.Security, this.Security.SiteGuid)
														);

			SiteTimeConverter timeConverter = new SiteTimeConverter(site);

			string fileName = site.ID + "_FMDT_" +
				  timeConverter.Now().ToString("yyyyMMdd_HHmmss") +
				  ".vcef";

			this.ProcessRecords(this.recordCollection, fileName, site);

			// Set records to completion 
			if (this.recordCollection.Count > 0)
			{
				FMChannelHelper.MakeCall<IChangeQueueRecordsClass>(
					x =>
					x.SetAllCompleted(
						this.Security,
						this.recordCollection[0].EventIndex,
						this.recordCollection[this.recordCollection.Count - 1].EventIndex));
			}
		}

		private void ExportSelectedDateRange(AlarmAndEventLogClass alarmAndEventLog)
		{
			// Get date from form
			DateTimeOffset startDateTime = this.FMDateFromDate.CurrentValue;
			DateTimeOffset endDateTime = this.FMDateToDate.CurrentValue;

			if (startDateTime > endDateTime)
			{
				throw new Exception("Start Date must not exceed End Date");
			}

			SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
												 x =>
												 x.GetUsingGuid(this.Security, this.Security.SiteGuid)
											);

			string fileName =	site.ID + "_FMDT_" +
								startDateTime.ToString("yyyyMMdd_HHmmss") + "_TO_" +
								endDateTime.ToString("yyyyMMdd_HHmmss") +
								".vcef";


			// Get the change records starting at the indicated date
			ChangeQueueRecordCollection queueRecordCollection =
											FMChannelHelper.MakeCall<IChangeQueueRecordsClass, ChangeQueueRecordCollection>(
												x => x.EnumerateByDate(this.Security, startDateTime, endDateTime));

			// Use that collection to process the output file
			this.ProcessRecords(queueRecordCollection, fileName, site);

			// Set records to completion 
			if (queueRecordCollection.Count > 0)
			{
				FMChannelHelper.MakeCall<IChangeQueueRecordsClass>(
									x =>
									x.SetAllCompleted(
										this.Security, queueRecordCollection[0].EventIndex, queueRecordCollection[queueRecordCollection.Count - 1].EventIndex));
			}
		}

		private void ProcessRecords(ChangeQueueRecordCollection inRecordCollection, string fileName, SiteClass site)
		{
			if (inRecordCollection.Count == 0)
			{
				throw new ApplicationException("No changes found to export.");
			}

			FileStream exportFileStream = null;

			try
			{
				this.accountingSite = FMChannelHelper.MakeCall<IAccountingSites, AccountingSite>(
																		 x =>
																		 x.LoadSiteInfo(this.Security, this.Security.SiteGuid)
																	);



				// If the Export Archive Directory exist save the file to the directory.
				if (!string.IsNullOrEmpty(site.ExportArchiveDir))
				{
					string strPath = site.ExportArchiveDir.Trim();
					var directoryInfo = new DirectoryInfo(strPath);

					if (!directoryInfo.Exists)
					{
						throw new Exception("Export Archive Directory Error, check Site configuration.");
					}

					const string StrBackSlash = "\\";

					if (!(strPath.EndsWith(StrBackSlash)))
					{
						strPath += StrBackSlash;
					}

					string strExportArchiveDirAndFileName = strPath + fileName;
					exportFileStream = new FileStream(strExportArchiveDirAndFileName, FileMode.Create);
				}
				else
				{
					throw new Exception("Export Archive Directory Error, check Site configuration.");
				}

				// Don't cross the streams
				this.Response.ClearContent();
				this.Response.ClearHeaders();

				this.Response.AddHeader("Content-disposition", "attachment; filename=" + fileName);
				this.Response.Buffer = false;
				this.Response.ContentType = "application/octet-stream";
				this.Response.AddHeader("cache-control", "private, max-age=0");
				this.Response.AddHeader("Connection", "Keep-Alive");
	
				var transmissionCollection = new DataTransmissionRecordCollectionClass();

				foreach (ChangeQueueRecordClass record in inRecordCollection)
				{
					if (record.Duplicate)
					{
						continue;
					}

					// If the record is not from the current site, check to make sure it is assigned.
					// If it is not assigned, we can skip it.
					if (this.CheckEntityAssignmentStatus(record) == false)
					{
						continue;
					}

					var transmissionRecord = new DataTransmissionRecordClass();

					if (record.IsDeletion)
					{
						// Add the change queue record as a delete indicator
						transmissionRecord.ChangeQueueRecord = record;
						transmissionRecord.OriginatingSiteID = FMChannelHelper.MakeCall<ISites, string>(
																		 x =>
																		 x.GetIDNoRefresh(this.Security, record.SiteGuid)
																	);

						transmissionCollection.Add(transmissionRecord);
					}
					else
					{
						transmissionRecord.ChangeQueueRecord = record;

						// Get the specified entity and add it to the export 
						switch (record.RecordType)
						{
							case ChangeQueueRecordType.Companies:
								this.AddCompanyRecord(transmissionCollection, record, transmissionRecord);
								break;

							case ChangeQueueRecordType.Equipment:
								this.AddEquipmentRecord(transmissionCollection, record, transmissionRecord);
								break;

							case ChangeQueueRecordType.FuelCards:
								this.AddFuelCardRecord(transmissionCollection, record, transmissionRecord);
								break;

							case ChangeQueueRecordType.Personnel:
								this.AddPersonRecord(transmissionCollection, record, transmissionRecord);
								break;

							case ChangeQueueRecordType.Products:
								this.AddProductRecord(transmissionCollection, record, transmissionRecord);
								break;

							case ChangeQueueRecordType.Transactions:
								this.AddTransactionRecord(transmissionCollection, record, transmissionRecord);
								break;

							case ChangeQueueRecordType.Groups:
								this.AddGroupRecord(transmissionCollection, record, transmissionRecord);
								break;

							case ChangeQueueRecordType.TransactionAliases:
								this.AddAliasRecord(transmissionCollection, record, transmissionRecord);
								break;

							case ChangeQueueRecordType.CloseoutDO:
								this.AddCloseoutRecord(transmissionCollection, record, transmissionRecord);
								break;
						}
					}

					if (transmissionCollection.Count > 1000)
					{
						var stream = new MemoryStream();

						var serializer = new XmlSerializer(transmissionCollection.GetType());
						serializer.Serialize(stream, transmissionCollection);
						transmissionCollection.Clear();

						MemoryStream encryptedStream = this.EncryptStream(stream);

						encryptedStream.WriteTo(this.Response.OutputStream);

						this.Response.Flush();

						encryptedStream.WriteTo(exportFileStream);
					}
				}

				if (transmissionCollection.Count != 0)
				{
					try
					{
						var stream = new MemoryStream();

						var serializer = new XmlSerializer(transmissionCollection.GetType());
						serializer.Serialize(stream, transmissionCollection);
						transmissionCollection.Clear();

						MemoryStream encryptedStream = this.EncryptStream(stream);

						encryptedStream.WriteTo(this.Response.OutputStream);
						this.Response.Flush();

						encryptedStream.WriteTo(exportFileStream);
					}
					catch (Exception e)
					{
						throw new Exception("Export Error Serializing an Decrypting Stream: " + e.Message);
					}
				}


				// Complete request and stop more than the file from rendering to the client
				this.Response.SuppressContent = true;
			}
			finally
			{
				if (exportFileStream != null)
				{
					exportFileStream.Flush();
					exportFileStream.Close();
				}
			}
		}
		#endregion
	}
}
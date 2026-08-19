using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Xml.Serialization;
using Accounting;
using System.Diagnostics;
using System.Xml;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.ServiceRequests;
using FMBusinessObjects.UtilityObjects;


namespace FuelsManagerEnterpriseExportWndSvs
{

	public class DataExport
	{
		private AccountingSite accountingSite;
		private AlarmAndEventLogClass alarmAndEventLog;
		private ChangeQueueRecordCollection recordCollection;
		private CloseoutDO closeout;
		private ICompanies companies;
		private IApplicationStrings applicationStrings;
		private ApplicationStringClass applicationString = new ApplicationStringClass();
		private IEquipments equipments;
		private IFuelCards fuelCards;
		private IGroups groups;
		private IPersonnel personnel;
		private IProducts products;
		private IPIDXProfiles PIDXprofile;
		private EnterpriseExportImportUtility EEICommon;
		private SecurityClass security;
		private ISites sites;
		private SiteClass site;
		private ITransactionAliases aliases;
		private string eventSource;

		public DataExport()
		{
			//Initialize Settings				
			security = new SecurityClass();

			FMChannelFactory<IDBAccess> dbAccessClient = new FMChannelFactory<IDBAccess>();
			IDBAccess DBAccess = dbAccessClient.CreateProxy();
			security.UserID = DBAccess.ServiceLogin(security);
			security.UserGuid = Guid.Empty;

			this.eventSource = "FuelsManager Enterprise Export Window Service";
			this.EEICommon = new EnterpriseExportImportUtility(security, this.eventSource);
			security.SiteGuid = Guid.Parse(this.EEICommon.ExportingSiteGuid);

			FMChannelFactory<ISites> sitesClient = new FMChannelFactory<ISites>();
			sites = sitesClient.CreateProxy();
			site = sites.Get(security, security.SiteGuid, false, false, false);

			//Rights
			security.RightCollection.Add(RIGHT.VIEW_SITES_AND_SITE_GROUPS);
			//1 Companies
			//11 CompanyMaps
			security.RightCollection.Add(RIGHT.VIEW_COMPANY_DATA);
			security.RightCollection.Add(RIGHT.MODIFY_COMPANY_DATA);
			// 2 Equipment
			security.RightCollection.Add(RIGHT.VIEW_EQUIPMENT_DATA);
			security.RightCollection.Add(RIGHT.MODIFY_EQUIPMENT_DATA);
			// 3 Fuel Cards
			security.RightCollection.Add(RIGHT.VIEW_FUEL_CARD_DATA);
			security.RightCollection.Add(RIGHT.MODIFY_FUEL_CARD_DATA);
			//4 Personnel
			security.RightCollection.Add(RIGHT.VIEW_PERSONNEL_DATA);
			security.RightCollection.Add(RIGHT.MODIFY_PERSONNEL_DATA);
			//5 Products
			security.RightCollection.Add(RIGHT.VIEW_PRODUCTS);
			security.RightCollection.Add(RIGHT.MODIFY_PRODUCTS);
			//7 Groups
			security.RightCollection.Add(RIGHT.VIEW_USER_GROUPS);
			security.RightCollection.Add(RIGHT.MODIFY_USER_GROUPS);
			//8 TransactionAliases
			security.RightCollection.Add(RIGHT.VIEW_TRANSACTION_ALIASES);
			security.RightCollection.Add(RIGHT.MODIFY_TRANSACTION_ALIASES);
			// 9 CloseoutRecord
			security.RightCollection.Add(RIGHT.VIEW_CLOSEOUT_DATA);
			//10 ProductMaps
			security.RightCollection.Add(RIGHT.VIEW_PRODUCTS);
			//14 PiDXProfiles
			//15 PIDXProfixeCompanyMaps
			security.RightCollection.Add(RIGHT.VIEW_PIDX_PROFILES);
			security.RightCollection.Add(RIGHT.MODIFY_PIDX_PROFILES);
			//17 TransactionSubLineItem
			//18 TransactionNotes
			//19 TransactionLineItem
			security.RightCollection.Add(RIGHT.VIEW_TRANSACTION_DATA);
			security.RightCollection.Add(RIGHT.MODIFY_TRANSACTION_DATA);
			security.RightCollection.Add(RIGHT.VIEW_FINANCIAL_DATA);
			security.RightCollection.Add(RIGHT.VIEW_ALARM_EVENT_LOGS);

			security.RightCollection.Add(RIGHT.VIEW_USERS);
			security.RightCollection.Add(RIGHT.MODIFY_USERS);

			security.RightCollection.Add(RIGHT.VIEW_ALLOCATIONS);
			security.RightCollection.Add(RIGHT.MODIFY_ALLOCATIONS);

			FMChannelFactory<IAccountingSites> accountingSitesClient = new FMChannelFactory<IAccountingSites>();
			IAccountingSites accountingSites = accountingSitesClient.CreateProxy();

			accountingSite = accountingSites.LoadSiteInfo(security, security.SiteGuid);
			accountingSite.HasViewPermissionForAllCompanies = true; // need this for transactions 			

			DataTransmission dt = new DataTransmission(security.SiteID, security.UserID);
			alarmAndEventLog = dt.TransmissionExportEventLog;
			closeout = new CloseoutDO();

			FMChannelFactory<ICompanies> companiesClient = new FMChannelFactory<ICompanies>();
			companies = companiesClient.CreateProxy();

			FMChannelFactory<IApplicationStrings> appClient = new FMChannelFactory<IApplicationStrings>();
			applicationStrings = appClient.CreateProxy();

			FMChannelFactory<IEquipments> equipClient = new FMChannelFactory<IEquipments>();
			equipments = equipClient.CreateProxy();

			FMChannelFactory<IFuelCards> fcClient = new FMChannelFactory<IFuelCards>();
			fuelCards = fcClient.CreateProxy();

			FMChannelFactory<IGroups> groupsClient = new FMChannelFactory<IGroups>();
			groups = groupsClient.CreateProxy();

			FMChannelFactory<IPersonnel> personnelClient = new FMChannelFactory<IPersonnel>();
			personnel = personnelClient.CreateProxy();

			FMChannelFactory<IProducts> productsClient = new FMChannelFactory<IProducts>();
			products = productsClient.CreateProxy();

			FMChannelFactory<ITransactionAliases> aliasesClient = new FMChannelFactory<ITransactionAliases>();
			aliases = aliasesClient.CreateProxy();

			FMChannelFactory<IPIDXProfiles> profilesClient = new FMChannelFactory<IPIDXProfiles>();
			PIDXprofile = profilesClient.CreateProxy();
		}


		public void Export()
		{
			string strFunctionName = "Export()";
			try
			{
				//ExportAllChanges(alarmAndEventLog);

				Boolean bWhereAllSuccessful = false;

				for (int i = 0; i < this.EEICommon.EnterpriseDataSendAttempts; i++)  // start
				{
					string msg = String.Format("Starting exporting data run: {0} out of {1} tries.", (i + 1).ToString(), EEICommon.EnterpriseDataSendAttempts.ToString());
					this.EEICommon.WriteToEventLogs(msg, EventLogEntryType.Information);

					bWhereAllSuccessful = ExportChangesOneRecAtATime();

					if (bWhereAllSuccessful)
					{
						break;
					}

					System.Threading.Thread.Sleep(this.EEICommon.EnterpriseDataIntervalBetweenSendAttemptsInSeconds * 1000);  // change seconds to mil seconds.

				}
			}
			catch (Exception ex)
			{
				String strAdditionalMessage = String.Format("Exception in object: {0}, Function {1}, Message: {2}.", this.ToString(), strFunctionName, ex.Message);

				if (ex.InnerException != null)
				{
					strAdditionalMessage += String.Format(" InnerException:{0}", ex.InnerException.ToString());
				}

				this.EEICommon.WriteToEventLogs(strAdditionalMessage, EventLogEntryType.Error);
				throw (ex);
			}
		}

		public void Export(ref MemoryStream stream, DateTimeOffset dtFromDate)
		{
			string strFunctionName = "Export(DateTimeOffset dtFromDate)";
			try
			{
				DataTransmission dt = new DataTransmission(security.SiteID, security.UserID, dtFromDate.ToString("d"));
				AlarmAndEventLogClass AlarmAndEventLog = dt.TransmissionExportReProcessEventLog;
				ExportSinceSelectedDate(ref stream, AlarmAndEventLog, dtFromDate);

				// Set records to completion 
				FMChannelFactory<IChangeQueueRecordsClass> recordsClient = new FMChannelFactory<IChangeQueueRecordsClass>();
				IChangeQueueRecordsClass records = recordsClient.CreateProxy();

				if (recordCollection != null)
				{
					records.SetAllCompletedByCollection(security, recordCollection);
				}
			}
			catch (Exception ex)
			{
				String strAdditionalMessage = String.Format("Exception in object: {0}, Function {1}, Message: {2}.", this.ToString(), strFunctionName, ex.Message);

				if (ex.InnerException != null)
				{
					strAdditionalMessage += String.Format(" InnerException:{0}", ex.InnerException.ToString());
				}

				this.EEICommon.WriteToEventLogs(strAdditionalMessage, EventLogEntryType.Error);
				throw (ex);
			}
		}

		public void ExportAllChanges(AlarmAndEventLogClass alarmAndEventLog)
		{
			string strFunctionName = "ExportAllChanges(AlarmAndEventLogClass alarmAndEventLog)";
			MemoryStream stream = null;
			try
			{
				// Get the primary export stream
				stream = new MemoryStream();
				GenerateExportStream(ref stream);
			}
			catch (Exception ex)
			{
				String strAdditionalMessage = String.Format("Exception in object: {0}, Function {1}, Message: {2}.",
															  this.ToString(), strFunctionName, ex.Message);

				if (ex.InnerException != null)
				{
					strAdditionalMessage += String.Format(" InnerException:{0}", ex.InnerException.ToString());
				}

				this.EEICommon.WriteToEventLogs(strAdditionalMessage, EventLogEntryType.Error);
				throw (ex);
			}
			finally
			{
				if (stream != null)
				{
					stream.Flush();
					stream.Close();
				}
			}
		}

		public Boolean ExportChangesOneRecAtATime()
		{
			string strFunctionName = "ExportChangesOneRecAtATime()";
			Boolean bWhereAllSuccessful = false;
			MemoryStream stream = null;
			int nFails = 0;
			int nSuccesses = 0;
			int nTotalRecsToProcess = 0;
			int nAttempts = 0;
			int nInvalidRecords = 0;

			try
			{
				FMChannelFactory<IChangeQueueRecordsClass> recordsClient = new FMChannelFactory<IChangeQueueRecordsClass>();
				IChangeQueueRecordsClass records = recordsClient.CreateProxy();

				recordCollection = records.EnumerateIncompleteRecords(security);
				ChangeQueueRecordCollection chngQueueCollectionWithOneRec = new ChangeQueueRecordCollection();

				nTotalRecsToProcess = recordCollection.Count;
				string strStartMsg = String.Format("Starting export of {0} records at: {1}.", nTotalRecsToProcess, DateTimeOffset.Now.ToString("u").Replace("Z", ""));
				this.EEICommon.WriteToEventLogs(strStartMsg, EventLogEntryType.Information);

				foreach (ChangeQueueRecordClass chngQueueRec in recordCollection)
				{
					try
					{
						nAttempts++;

						// Get the primary export stream
						if (stream != null)
						{
							stream.Flush();
							stream.Close();
						}

						stream = new MemoryStream();

						// Create a record collection with only one record at a time to pass as parameter. 
						chngQueueCollectionWithOneRec.Add(chngQueueRec);
						this.ProcessRecordsIntoExportStream(ref stream, records, chngQueueCollectionWithOneRec);

						if (stream.Length == 0)
						{
							String strMsg = String.Format("Unable to export record! Record type:{0}, RecordID: {1}, EventType {2}, RecordGuid: {3}",
								chngQueueRec.RecordType.ToString(), chngQueueRec.RecordID, chngQueueRec.EventType, chngQueueRec.RecordGuid);
							this.EEICommon.WriteToEventLogs(strMsg, EventLogEntryType.Warning);
							nInvalidRecords++;
						}

						//need to make web service connection as import the record here.
						XmlTextWriter xmlTextWriter = new XmlTextWriter(stream, Encoding.UTF8);
						UTF8Encoding encoding = new UTF8Encoding();
						String XmlizedString = encoding.GetString(stream.ToArray());

						if (XmlizedString.Length != 0)
						{

							FuelsManagerEnterpriseExportWndSvs.ImportEnterpriseData.ImportService webServiceToImport = new FuelsManagerEnterpriseExportWndSvs.ImportEnterpriseData.ImportService();
							///http://localhost/AccountingImportExport/ImportService.asmx
							webServiceToImport.Url = EEICommon.URLofEnterpriseDataWebService;
							webServiceToImport.Timeout = 600000;  // 10 minutes in milseconds.

							FuelsManagerEnterpriseExportWndSvs.ImportEnterpriseData.EntityDataImportResponseDO response = new FuelsManagerEnterpriseExportWndSvs.ImportEnterpriseData.EntityDataImportResponseDO();

							try
							{
								response = webServiceToImport.ImportEntityData(XmlizedString);
							}
							catch (Exception ex)
							{
								String strAdditionalMessage = String.Format("Error recieved from call to webServiceToImport.ImportEntityData(XmlizedString), Message: {0} in object:{1}, Function {2}, . ", ex.Message, this.ToString(), strFunctionName);

								if (ex.InnerException != null)
								{
									strAdditionalMessage += String.Format(" InnerException:{0}", ex.InnerException.ToString());
								}

								// write the message and keep going.
								this.EEICommon.WriteToEventLogs(strAdditionalMessage, EventLogEntryType.Error);

							}

							ChangeQueueRecordCollection responseCollection; ;

							if (response != null)
							{

								if (response.Status == FuelsManagerEnterpriseExportWndSvs.ImportEnterpriseData.ResponseStatus.SUCCESS)
								{
									nSuccesses += response.ProcessedChangeQueueRecords.Length;
									responseCollection = ConvertChangeQueueRecord(response);

									// mark as sent successful.
									records.SetAllCompletedByCollection(security, responseCollection);
									responseCollection.Clear();
								}
								else
								{
									nFails++;

									if (response.Status == FuelsManagerEnterpriseExportWndSvs.ImportEnterpriseData.ResponseStatus.FAIL)
									{
										ChangeQueueRecordType RecType = chngQueueRec.RecordType;
										string strRecType = chngQueueRec.RecordType.ToString();
										String Name = chngQueueRec.RecordID; //Name				
										String strEventType = chngQueueRec.EventType;
										Guid recordGuid = chngQueueRec.RecordGuid; // Guid in the records table
										string message = String.Format("Response from import: {0}, RecordID: {1}, RecordType: {2}, Event: {3}, Reason: {4}", response.Status.ToString(), chngQueueRec.RecordID, strRecType, strEventType, response.ErrorMessage);

										this.EEICommon.WriteToEventLogs(message, EventLogEntryType.Error);// information from the imort web service.
									}
								}
							}
							else  // response is null
							{
								nFails++;
							}
							// Write the stream out to archive directory

							this.EEICommon.WriteStreamToFile(stream, EEICommon.ExportArchiveDir);
						}

					}
					catch (Exception ex)
					{
						String strAdditionalMessage = String.Format("Exception in object: {0}, Function {1}, Message: {2}.", this.ToString(), strFunctionName, ex.Message);

						if (ex.InnerException != null)
						{
							strAdditionalMessage += String.Format(" InnerException:{0}", ex.InnerException.ToString());
						}

						// write the message and keep going.
						this.EEICommon.WriteToEventLogs(strAdditionalMessage, EventLogEntryType.Error);
					}

					chngQueueCollectionWithOneRec.Clear(); // Clear to get the next record
				}

				string strStopMsg = String.Format("Stopping export of: {0} records, Succeed: {1}, Failed: {2}, InvalidRecords: {3} Attempts: {4}.", nTotalRecsToProcess, nSuccesses, nFails, nInvalidRecords, nAttempts);
				this.EEICommon.WriteToEventLogs(strStopMsg, EventLogEntryType.Information);

				if ((nFails == 0) && (nSuccesses == nTotalRecsToProcess))
				{
					bWhereAllSuccessful = true;
				}
				return bWhereAllSuccessful;

			}
			catch (Exception ex)
			{
				string strStopMsg = String.Format("Stopping export of: {0} records, Succeed: {1}, Failed: {2}, InvalidRecords: {3} Attempts: {4}.", nTotalRecsToProcess, nSuccesses, nFails, nInvalidRecords, nAttempts);
				this.EEICommon.WriteToEventLogs(strStopMsg, EventLogEntryType.Information);

				String strAdditionalMessage = String.Format("Exception in object: {0}, Function {1}, Message: {2}.", this.ToString(), strFunctionName, ex.Message);
				if (ex.InnerException != null)
				{
					strAdditionalMessage += String.Format(" InnerException:{0}", ex.InnerException.ToString());
				}

				this.EEICommon.WriteToEventLogs(strAdditionalMessage, EventLogEntryType.Error);

				if (stream != null)
				{
					stream.Flush();
					stream.Close();
				}

				throw (ex);
			}
			finally
			{
				if (stream != null)
				{
					stream.Flush();
					stream.Close();
				}
			}
		}

		private ChangeQueueRecordCollection ConvertChangeQueueRecord(FuelsManagerEnterpriseExportWndSvs.ImportEnterpriseData.EntityDataImportResponseDO response)
		{

			string strFunctionName = "ChangeQueueRecordCollection ConvertChangeQueueRecord (BaseExport.ImportEnterpriseData.EntityDataImportResponseDO response)";
			ChangeQueueRecordCollection chngQueueCollection = new ChangeQueueRecordCollection();

			try
			{

				for (int i = 0; i < response.ProcessedChangeQueueRecords.Length; i++)
				{
					ChangeQueueRecordClass chngQRec = new ChangeQueueRecordClass();
					chngQRec.Completed = response.ProcessedChangeQueueRecords[i].Completed;
					chngQRec.EventIndex = response.ProcessedChangeQueueRecords[i].EventIndex;
					chngQRec.EventType = response.ProcessedChangeQueueRecords[i].EventType;
					chngQRec.ID = response.ProcessedChangeQueueRecords[i].ID;
					chngQRec.IdentityGuid = response.ProcessedChangeQueueRecords[i].IdentityGuid;
					chngQRec.RecordID = response.ProcessedChangeQueueRecords[i].RecordID;
					chngQRec.RecordGuid = response.ProcessedChangeQueueRecords[i].RecordGuid;
					chngQRec.SiteID = response.ProcessedChangeQueueRecords[i].SiteID;
					chngQRec.SiteGuid = response.ProcessedChangeQueueRecords[i].SiteGuid;
					string strRectype = response.ProcessedChangeQueueRecords[i].RecordType.ToString();
					ChangeQueueRecordType cqrt = (ChangeQueueRecordType)Enum.Parse(typeof(ChangeQueueRecordType), strRectype);
					chngQueueCollection.Add(chngQRec);
				}
			}
			catch (Exception ex)
			{
				String strAdditionalMessage = String.Format("Exception in object: {0}, Function {1}, Message: {2}.", this.ToString(), strFunctionName, ex.Message);

				if (ex.InnerException != null)
				{
					strAdditionalMessage += String.Format(" InnerException:{0}", ex.InnerException.ToString());
				}

				this.EEICommon.WriteToEventLogs(strAdditionalMessage, EventLogEntryType.Error);
				throw ex;
			}

			return chngQueueCollection;
		}


		private void GenerateExportStream(ref MemoryStream stream)
		{
			string strFunctionName = "GenerateExportStream()";
			try
			{
				// Get a list of the changes to process
				FMChannelFactory<IChangeQueueRecordsClass> recordsClient = new FMChannelFactory<IChangeQueueRecordsClass>();
				IChangeQueueRecordsClass records = recordsClient.CreateProxy();

				recordCollection = records.EnumerateIncompleteRecords(security);
				ProcessRecordsIntoExportStream(ref stream, records, recordCollection);
			}
			catch (Exception ex)
			{
				String strAdditionalMessage = String.Format("Exception in object: {0}, Function {1}, Message: {2}.", this.ToString(), strFunctionName, ex.Message);

				if (ex.InnerException != null)
				{
					strAdditionalMessage += String.Format(" InnerException:{0}", ex.InnerException.ToString());
				}

				this.EEICommon.WriteToEventLogs(strAdditionalMessage, EventLogEntryType.Error);
				throw ex;
			}
		}
		private void ProcessRecordsIntoExportStream(ref MemoryStream stream,
													IChangeQueueRecordsClass records,
													ChangeQueueRecordCollection recordCollection)
		{
			string strFunctionName = "ProcessRecordsIntoExportStream(ChangeQueueRecordsClass records, ChangeQueueRecordCollection recordCollection)";

			try
			{
				FMChannelFactory<IAccountingSites> accountingSitesClient = new FMChannelFactory<IAccountingSites>();
				IAccountingSites accountingSites = accountingSitesClient.CreateProxy();

				this.accountingSite = accountingSites.LoadSiteInfo(security, security.SiteGuid);

				DataTransmissionRecordCollectionClass transmissionCollection = new DataTransmissionRecordCollectionClass();
				foreach (ChangeQueueRecordClass record in recordCollection)
				{
					if (record.Duplicate)
					{

						String strAdditionalMessage = String.Format("Duplicate change queue record, Recordtype: {0}, RecordGuid: {1}, RecordID: {2} ",
																					record.RecordType.ToString(), record.RecordGuid.ToString(), record.RecordID.ToString());
						this.EEICommon.WriteToEventLogs(strAdditionalMessage, EventLogEntryType.Information);
						continue;
					}

					//// If the record is not from the current site, check to make sure it is assigned.
					// If it is not assigned, we can skip it.
					if (CheckEntityAssignmentStatus(record) == false)
					{
						String strTmpMsg = String.Format("Entity Assignment Status is false for: Record type:{0}, RecordID: {1}, EventType {2}, RecordGuid: {3}",
																		record.RecordType.ToString(), record.RecordID, record.EventType, record.RecordGuid);
						this.EEICommon.WriteToEventLogs(strTmpMsg, EventLogEntryType.Information);
						continue;
					}

					DataTransmissionRecordClass transmissionRecord = new DataTransmissionRecordClass();

					if (record.IsDeletion)
					{
						// Add the change queue record as a delete indicator
						transmissionRecord.ChangeQueueRecord = record;
						transmissionRecord.OriginatingSiteID = sites.GetIDNoRefresh(security, record.SiteGuid);
						transmissionCollection.Add(transmissionRecord);
					}
					else
					{
						transmissionRecord.ChangeQueueRecord = record;
						// Get the specified entity and add it to the export 

						switch (record.RecordType)
						{
							case ChangeQueueRecordType.Companies:
								{
									this.AddCompanyRecord(companies, transmissionCollection, record, transmissionRecord);
									break;
								}
							case ChangeQueueRecordType.Equipment:
								{
									this.AddEquipmentRecord(equipments, transmissionCollection, record, transmissionRecord);
									break;
								}
							case ChangeQueueRecordType.Personnel:
								{
									this.AddPersonRecord(personnel, transmissionCollection, record, transmissionRecord);
									break;
								}
							case ChangeQueueRecordType.Products:
								{
									this.AddProductRecord(products, transmissionCollection, record, transmissionRecord);
									break;
								}
							case ChangeQueueRecordType.Transactions:
								{
									this.AddTransactionRecord(transmissionCollection, record, transmissionRecord);
									break;
								}
							case ChangeQueueRecordType.Groups:
								{
									this.AddGroupRecord(groups, transmissionCollection, record, transmissionRecord);
									break;
								}
							case ChangeQueueRecordType.TransactionAliases:
								{
									this.AddAliasRecord(aliases, transmissionCollection, record, transmissionRecord);
									break;
								}
							case ChangeQueueRecordType.ApplicationStrings:
								{
									this.AddApplicationStringsRecord(applicationString, transmissionCollection, record, transmissionRecord);
									break;
								}
							case ChangeQueueRecordType.CloseoutDO:
								{
									this.AddCloseoutRecord(closeout, transmissionCollection, record, transmissionRecord);
									break;
								}

							case ChangeQueueRecordType.PIDXProfiles:
								{
									this.AddPIDXProfileRecord(PIDXprofile, transmissionCollection, record, transmissionRecord);
									break;
								}
							default:
								{
									ChangeQueueRecordType notProcessedRecType = record.RecordType;
									String strAdditionalMessage = String.Format("Warning in object: {0}, Function: {1}, Message: Record type {2} was not added to the transmission collection.", this.ToString(), strFunctionName, notProcessedRecType.ToString());
									this.EEICommon.WriteToEventLogs(strAdditionalMessage, EventLogEntryType.Information);
									break;
								}
						}
					}
				}
				if (transmissionCollection.Count > 0)
				{
					Type objType = transmissionCollection.GetType();
					XmlSerializer serializer = new XmlSerializer(objType);
					serializer.Serialize(stream, transmissionCollection);
				}
			}
			catch (Exception ex)
			{
				String strAdditionalMessage = String.Format("Exception in object: {0}, Function {1}, Message: {2}.", this.ToString(), strFunctionName, ex.Message);
				if (ex.InnerException != null)
				{
					strAdditionalMessage += String.Format(" InnerException: {0}", ex.InnerException.ToString());
				}
				this.EEICommon.WriteToEventLogs(strAdditionalMessage, EventLogEntryType.Error);
				throw ex;
			}
		}

		private bool CheckEntityAssignmentStatus(ChangeQueueRecordClass record)
		{
			string strFunctionName = "CheckEntityAssignmentStatus(ChangeQueueRecordClass record)";
			try
			{
				if (record.SiteGuid != security.SiteGuid)
				{
					switch (record.RecordType)
					{
						case ChangeQueueRecordType.Companies:
							if (companies.GetIdentityGuid(security, record.RecordID) == Guid.Empty)
							{
								return false;
							}
							break;
						case ChangeQueueRecordType.Equipment:
							if (equipments.GetIdentityGuid(security, record.RecordID) == Guid.Empty)
							{
								return false;
							}
							break;
						case ChangeQueueRecordType.Personnel:
							if (personnel.GetGuidByID(security, record.RecordID).IsEmpty())
							{
								return false;
							}
							break;
						case ChangeQueueRecordType.Products:
							if (products.GetIdentityGuid(security, record.RecordID) == Guid.Empty)
							{
								return false;
							}
							break;
						case ChangeQueueRecordType.Transactions:
							break; // do nothing since transactions cannot be "assigned"
						case ChangeQueueRecordType.Groups:
							if (groups.GetIdentityGuid(security, record.RecordID) == Guid.Empty)
							{
								return false;
							}
							break;
						case ChangeQueueRecordType.TransactionAliases:
							if (aliases.GetIdentityGuid(security, record.RecordID) == Guid.Empty)
							{
								return false;
							}
							break;

						case ChangeQueueRecordType.ApplicationStrings:
							if (this.applicationStrings.GetIdentityGuid(security, this.applicationString.Type, record.RecordID) == Guid.Empty)
							{
								return false;
							}
							break;

						case ChangeQueueRecordType.CloseoutDO:
							//if (closeout.GetIdentityGuid(security, record.RecordID) == Guid.Empty)
							//{
							//		return false;
							//}
							break;

						case ChangeQueueRecordType.PIDXProfiles:
							if (PIDXprofile.GetIdentityGuid(security, record.RecordID) == Guid.Empty)
							{
								return false;
							}
							break;
					}
				}
			}
			catch (Exception ex)
			{
				String strAdditionalMessage = String.Format("Exception in object: {0}, Function {1}, Message: {2}.", this.ToString(), strFunctionName, ex.Message);

				if (ex.InnerException != null)
				{
					strAdditionalMessage += String.Format(" InnerException:{0}", ex.InnerException.ToString());
				}

				this.EEICommon.WriteToEventLogs(strAdditionalMessage, EventLogEntryType.Error);
				throw ex;
			}
			return true;
		}

		private void AddAliasRecord(ITransactionAliases aliases,
									DataTransmissionRecordCollectionClass transmissionCollection,
									ChangeQueueRecordClass record,
									DataTransmissionRecordClass transmissionRecord)
		{
			string strFunctionName = "AddAliasRecord(TransactionAliasesClass aliases, DataTransmissionRecordCollectionClass transmissionCollection, ChangeQueueRecordClass record, DataTransmissionRecordClass transmissionRecord)";
			try
			{
				Guid aliasGuid = aliases.GetIdentityGuid(security, record.RecordID);

				if (aliasGuid == record.RecordGuid)
				{
					transmissionRecord.TransactionAlias = aliases.Get(security, aliasGuid, false);
					transmissionRecord.OriginatingSiteID = sites.GetIDNoRefresh(security, transmissionRecord.TransactionAlias.SiteGuid);
					transmissionCollection.Add(transmissionRecord);
				}
				else
				{
					String strTmpMsg = String.Format("Cannot export record, TransactionAlias GetIdentityGuid function returned different record Guid. There is a possible duplicate TransactionAlias with same ID {0} in the database.", record.RecordID);
					this.EEICommon.WriteToEventLogs(strTmpMsg, EventLogEntryType.Warning);
				}

			}
			catch (Exception ex)
			{
				String strAdditionalMessage = String.Format("Exception in object: {0}, Function {1}, Message: {2}.", this.ToString(), strFunctionName, ex.Message);
				if (ex.InnerException != null)
				{
					strAdditionalMessage += String.Format(" InnerException:{0}", ex.InnerException.ToString());
				}

				this.EEICommon.WriteToEventLogs(strAdditionalMessage, EventLogEntryType.Error);
				throw ex;
			}
		}

		private void AddGroupRecord(IGroups groups,
									DataTransmissionRecordCollectionClass transmissionCollection,
									ChangeQueueRecordClass record,
									DataTransmissionRecordClass transmissionRecord)
		{
			string strFunctionName = "AddGroupRecord(GroupsClass groups, DataTransmissionRecordCollectionClass transmissionCollection, " +
									 "ChangeQueueRecordClass record, DataTransmissionRecordClass transmissionRecord)";
			try
			{
				Guid groupGuid = groups.GetIdentityGuid(security, record.RecordID);

				if (groupGuid == record.RecordGuid)
				{
					transmissionRecord.Group = groups.Get(security, groupGuid);
					transmissionRecord.OriginatingSiteID = sites.GetIDNoRefresh(security, transmissionRecord.Group.SiteGuid);
					transmissionCollection.Add(transmissionRecord);
				}
				else
				{
					String strTmpMsg = String.Format("Cannot export record, Groups GetIdentityGuid function returned different record Guid. There is a possible duplicate Groups with same ID {0} in the database.", record.RecordID);
					this.EEICommon.WriteToEventLogs(strTmpMsg, EventLogEntryType.Warning);
				}
			}
			catch (Exception ex)
			{
				String strAdditionalMessage = String.Format("Exception in object: {0}, Function {1}, Message: {2}.", this.ToString(), strFunctionName, ex.Message);
				if (ex.InnerException != null)
				{
					strAdditionalMessage += String.Format(" InnerException:{0}", ex.InnerException.ToString());
				}

				this.EEICommon.WriteToEventLogs(strAdditionalMessage, EventLogEntryType.Error);
				throw ex;
			}
		}

		private void AddTransactionRecord(DataTransmissionRecordCollectionClass transmissionCollection,
											ChangeQueueRecordClass record,
											DataTransmissionRecordClass transmissionRecord)
		{
			string strFunctionName = "AddTransactionRecord(DataTransmissionRecordCollectionClass transmissionCollection, " +
									 "ChangeQueueRecordClass record, DataTransmissionRecordClass transmissionRecord)";

			try
			{
				TransactionSR sr = new TransactionSR();
				sr.Security = security;
				sr.TransID = record.RecordID;
				sr.AccountingSite = this.accountingSite;

				FMChannelFactory<ITransactionProcessor> transClient = new FMChannelFactory<ITransactionProcessor>();
				ITransactionProcessor transProcessor = transClient.CreateProxy();
				transmissionRecord.Transaction = transProcessor.Process(sr);

				if (transmissionRecord.Transaction != null
					&& transmissionRecord.Transaction.SiteGuid == accountingSite.CurrentSiteGuid)
				{
					transmissionRecord.OriginatingSiteID = sites.GetIDNoRefresh(security, transmissionRecord.Transaction.SiteGuid);
					transmissionCollection.Add(transmissionRecord);
				}
			}
			catch (Exception ex)
			{
				String strAdditionalMessage = String.Format("Exception in object: {0}, Function {1}, Message: {2}.", this.ToString(), strFunctionName, ex.Message);
				if (ex.InnerException != null)
				{
					strAdditionalMessage += String.Format(" InnerException:{0}", ex.InnerException.ToString());
				}
				this.EEICommon.WriteToEventLogs(strAdditionalMessage, EventLogEntryType.Error);
				throw ex;
			}
		}

		private void AddCloseoutRecord(CloseoutDO closeout,
										DataTransmissionRecordCollectionClass transmissionCollection,
										ChangeQueueRecordClass record,
										DataTransmissionRecordClass transmissionRecord)
		{
			string strFunctionName = "AddCloseoutRecord(CloseoutDO closeout, DataTransmissionRecordCollectionClass transmissionCollection, ChangeQueueRecordClass record, DataTransmissionRecordClass transmissionRecord)";

			try
			{
				CloseoutSR closeoutSrvcRqst = new CloseoutSR();
				closeoutSrvcRqst.Security = security;
				closeoutSrvcRqst.CloseoutInventoryGuid = record.RecordGuid;
				closeoutSrvcRqst.CloseoutCommand = CloseoutSR.CloseoutType.GET_TO_EXPORT;

				FMChannelFactory<ICloseoutProcessor> closeoutClient = new FMChannelFactory<ICloseoutProcessor>();
				ICloseoutProcessor closeoutProcessor = closeoutClient.CreateProxy();
				transmissionRecord.Closeout = closeoutProcessor.Process(closeoutSrvcRqst);

				if (transmissionRecord.Closeout != null)
				{
					transmissionRecord.OriginatingSiteID = sites.GetIDNoRefresh(security, transmissionRecord.Closeout.SiteGuid);
					transmissionCollection.Add(transmissionRecord);
				}
			}
			catch (Exception ex)
			{
				String strAdditionalMessage = String.Format("Exception in object: {0}, Function {1}, Message: {2}.", this.ToString(), strFunctionName, ex.Message);
				if (ex.InnerException != null)
				{
					strAdditionalMessage += String.Format(" InnerException:{0}", ex.InnerException.ToString());
				}
				this.EEICommon.WriteToEventLogs(strAdditionalMessage, EventLogEntryType.Error);
				throw ex;
			}
		}

		private void AddProductRecord(IProducts products, DataTransmissionRecordCollectionClass transmissionCollection, ChangeQueueRecordClass record, DataTransmissionRecordClass transmissionRecord)
		{
			string strFunctionName = "AddProductRecord(ProductsClass products, DataTransmissionRecordCollectionClass transmissionCollection, ChangeQueueRecordClass record, DataTransmissionRecordClass transmissionRecord)";
			try
			{
				Guid identityGuid = products.GetIdentityGuid(security, record.RecordID);
				if (identityGuid == record.RecordGuid)
				{
					transmissionRecord.Product = products.Get(security, identityGuid);
					transmissionRecord.OriginatingSiteID = sites.GetIDNoRefresh(security, transmissionRecord.Product.SiteGuid);
					transmissionCollection.Add(transmissionRecord);
				}
				else
				{
					String strTmpMsg = String.Format("Cannot export record, Products GetIdentityGuid function returned different record identity guid. There is a possible duplicate Products with same ID {0} in the database.", record.RecordID);
					this.EEICommon.WriteToEventLogs(strTmpMsg, EventLogEntryType.Warning);
				}
			}
			catch (Exception ex)
			{
				String strAdditionalMessage = String.Format("Exception in object: {0}, Function {1}, Message: {2}.", this.ToString(), strFunctionName, ex.Message);
				if (ex.InnerException != null)
				{
					strAdditionalMessage += String.Format(" InnerException:{0}", ex.InnerException.ToString());
				}
				this.EEICommon.WriteToEventLogs(strAdditionalMessage, EventLogEntryType.Error);
				throw ex;
			}
		}

		private void AddPersonRecord(IPersonnel personnel, DataTransmissionRecordCollectionClass transmissionCollection, ChangeQueueRecordClass record, DataTransmissionRecordClass transmissionRecord)
		{
			string strFunctionName = "AddPersonRecord(PersonnelClass personnel, DataTransmissionRecordCollectionClass transmissionCollection, ChangeQueueRecordClass record, DataTransmissionRecordClass transmissionRecord)";
			try
			{
				Guid personGuid = personnel.GetGuidByID(security, record.RecordID);

				if (personGuid == record.IdentityGuid)
				{
					PersonClass person = personnel.Get(security, personGuid);
					transmissionRecord.Person = personnel.PrepareForExport(security, person);
					transmissionRecord.OriginatingSiteID = sites.GetIDNoRefresh(security, transmissionRecord.Person.SiteGuid);
					transmissionCollection.Add(transmissionRecord);
				}
				else
				{
					String strTmpMsg = String.Format("Cannot export record, Personnel GetGuidByID function returned different record Guid. There is a possible duplicate Personnel with same ID {0} in the database.", record.RecordID);
					this.EEICommon.WriteToEventLogs(strTmpMsg, EventLogEntryType.Warning);
				}
			}
			catch (Exception ex)
			{
				String strAdditionalMessage = String.Format("Exception in object: {0}, Function {1}, Message: {2}.", this.ToString(), strFunctionName, ex.Message);
				if (ex.InnerException != null)
				{
					strAdditionalMessage += String.Format(" InnerException:{0}", ex.InnerException.ToString());
				}
				this.EEICommon.WriteToEventLogs(strAdditionalMessage, EventLogEntryType.Error);
				throw ex;
			}
		}

		private void AddEquipmentRecord(IEquipments equipments,
										DataTransmissionRecordCollectionClass transmissionCollection,
										ChangeQueueRecordClass record,
										DataTransmissionRecordClass transmissionRecord)
		{
			string strFunctionName = "AddEquipmentRecord(EquipmentsClass equipments, DataTransmissionRecordCollectionClass transmissionCollection, ChangeQueueRecordClass record, DataTransmissionRecordClass transmissionRecord)";
			try
			{
				Guid equipmentGuid = equipments.GetIdentityGuid(security, record.RecordID);

				if (equipmentGuid == record.RecordGuid)
				{
					transmissionRecord.Equipment = equipments.Get(security, equipmentGuid);

					if (string.IsNullOrEmpty(transmissionRecord.Equipment.FuelCardID) == false)
					{
						transmissionRecord.Equipment.ExportUseFuelCard = this.fuelCards.Get(security, transmissionRecord.Equipment.FuelCardGuid, true);
					}
					transmissionRecord.OriginatingSiteID = sites.GetIDNoRefresh(security, transmissionRecord.Equipment.SiteGuid);
					transmissionCollection.Add(transmissionRecord);
				}
				else
				{
					String strTmpMsg = String.Format("Cannot export record, Equipment GetIdentityGuid function returned different record identity guid. There is a possible duplicate Equipment with ID {0} in the database.", record.RecordID);
					this.EEICommon.WriteToEventLogs(strTmpMsg, EventLogEntryType.Warning);
				}
			}
			catch (Exception ex)
			{
				String strAdditionalMessage = String.Format("Exception in object: {0}, Function {1}, Message: {2}.", this.ToString(), strFunctionName, ex.Message);
				if (ex.InnerException != null)
				{
					strAdditionalMessage += String.Format(" InnerException:{0}", ex.InnerException.ToString());
				}
				this.EEICommon.WriteToEventLogs(strAdditionalMessage, EventLogEntryType.Error);
				throw ex;
			}
		}

		private void AddCompanyRecord(ICompanies companies, DataTransmissionRecordCollectionClass transmissionCollection, ChangeQueueRecordClass record, DataTransmissionRecordClass transmissionRecord)
		{
			string strFunctionName = "AddCompanyRecord(CompaniesClass companies, DataTransmissionRecordCollectionClass transmissionCollection, ChangeQueueRecordClass record, DataTransmissionRecordClass transmissionRecord)";
			try
			{
				Guid companyGuid = companies.GetIdentityGuid(security, record.RecordID);
				if (companyGuid == record.RecordGuid)
				{
					transmissionRecord.Company = companies.Get(security, companyGuid);
					transmissionRecord.OriginatingSiteID = sites.GetIDNoRefresh(security, transmissionRecord.Company.SiteGuid);
					transmissionCollection.Add(transmissionRecord);
				}
				else
				{
					String strTmpMsg = String.Format("Cannot export record, Companies GetIdentityGuid function returned different record IdentityGuid. There is a possible duplicate Company with same ID {0} in the database.", record.RecordID);
					this.EEICommon.WriteToEventLogs(strTmpMsg, EventLogEntryType.Warning);
				}
			}
			catch (Exception ex)
			{
				String strAdditionalMessage = String.Format("Exception in object: {0}, Function {1}, Message: {2}.", this.ToString(), strFunctionName, ex.Message);
				if (ex.InnerException != null)
				{
					strAdditionalMessage += String.Format(" InnerException:{0}", ex.InnerException.ToString());
				}
				this.EEICommon.WriteToEventLogs(strAdditionalMessage, EventLogEntryType.Error);
				throw ex;
			}
		}
		private void ExportSinceSelectedDate(ref MemoryStream stream, AlarmAndEventLogClass alarmAndEventLog, DateTimeOffset dtStartDate)
		{
			string strFunctionName = "ExportSinceSelectedDate(AlarmAndEventLogClass alarmAndEventLog, DateTimeOffset dtStartDate)";
			try
			{
				// Get the change records starting at the indicated date
				FMChannelFactory<IChangeQueueRecordsClass> recordsClient = new FMChannelFactory<IChangeQueueRecordsClass>();
				IChangeQueueRecordsClass records = recordsClient.CreateProxy();

				ChangeQueueRecordCollection recordCollection = records.EnumerateByDate(security, dtStartDate);

				// Use that collection to process the output file
				this.ProcessRecordsIntoExportStream(ref stream, records, recordCollection);
			}
			catch (Exception ex)
			{
				String strAdditionalMessage = String.Format("Exception in object: {0}, Function {1}, Message: {2}.", this.ToString(), strFunctionName, ex.Message);
				if (ex.InnerException != null)
				{
					strAdditionalMessage += String.Format(" InnerException:{0}", ex.InnerException.ToString());
				}
				this.EEICommon.WriteToEventLogs(strAdditionalMessage, EventLogEntryType.Error);
				throw ex;
			}
		}

		private void AddApplicationStringsRecord(ApplicationStringClass applicationString,
													DataTransmissionRecordCollectionClass transmissionCollection,
													ChangeQueueRecordClass record,
													DataTransmissionRecordClass transmissionRecord)
		{
			string strFunctionName = "AddApplicationStringsRecord(ApplicationStringClass ApplicationString, DataTransmissionRecordCollectionClass transmissionCollection, ChangeQueueRecordClass record, DataTransmissionRecordClass transmissionRecord)";
			try
			{
				string tempID = applicationString.ID; // note id is much larger than the changequeue recordID 
				Guid identityGuid = this.products.GetIdentityGuid(security, record.RecordID);

				if (identityGuid == record.RecordGuid)
				{
					transmissionRecord.ApplicationString = this.applicationStrings.Get(security, identityGuid);
					transmissionRecord.OriginatingSiteID = sites.GetIDNoRefresh(security, transmissionRecord.ApplicationString.SiteGuid);
					transmissionCollection.Add(transmissionRecord);
				}
				else
				{
					String strTmpMsg = String.Format("Cannot export record, ApplicationString GetIdentityGuid function returned different record identity guid. There is a possible duplicate ApplicationString with same ID {0} in the database.", record.RecordID);
					this.EEICommon.WriteToEventLogs(strTmpMsg, EventLogEntryType.Warning);
				}
			}
			catch (Exception ex)
			{
				String strAdditionalMessage = String.Format("Exception in object: {0}, Function {1}, Message: {2}.", this.ToString(), strFunctionName, ex.Message);

				if (ex.InnerException != null)
				{
					strAdditionalMessage += String.Format(" InnerException:{0}", ex.InnerException.ToString());
				}

				this.EEICommon.WriteToEventLogs(strAdditionalMessage, EventLogEntryType.Error);
				throw ex;
			}
		}

		private void AddPIDXProfileRecord(IPIDXProfiles pidxProfile,
											DataTransmissionRecordCollectionClass transmissionCollection,
											ChangeQueueRecordClass record,
											DataTransmissionRecordClass transmissionRecord)
		{
			string strFunctionName = "AddPIDXProfileRecord(PIDXProfilesClass pidxProfile, DataTransmissionRecordCollectionClass transmissionCollection, ChangeQueueRecordClass record, DataTransmissionRecordClass transmissionRecord)";
			try
			{
				Guid identityGuid = pidxProfile.GetIdentityGuid(security, record.RecordID);

				if (identityGuid == record.RecordGuid)
				{

					transmissionRecord.PidxProfile = pidxProfile.Get(security, identityGuid, true);
					transmissionRecord.OriginatingSiteID = sites.GetIDNoRefresh(security, transmissionRecord.PidxProfile.SiteGuid);
					transmissionCollection.Add(transmissionRecord);
				}
				else
				{
					String strTmpMsg = String.Format("Cannot export record, PidxProfile GetIdentityGuid function returned different record identityGuid. There is a possible duplicate PidxProfiles with same ID {0} in the database.", record.RecordID);
					this.EEICommon.WriteToEventLogs(strTmpMsg, EventLogEntryType.Warning);
				}

			}
			catch (Exception ex)
			{
				String strAdditionalMessage = String.Format("Exception in object: {0}, Function {1}, Message: {2}.", this.ToString(), strFunctionName, ex.Message);

				if (ex.InnerException != null)
				{
					strAdditionalMessage += String.Format(" InnerException:{0}", ex.InnerException.ToString());
				}

				this.EEICommon.WriteToEventLogs(strAdditionalMessage, EventLogEntryType.Error);
				throw ex;
			}
		}
	}
}
